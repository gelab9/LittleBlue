"""Email notification settings dialog with LDAP autocomplete."""

import logging
from functools import partial
from PyQt6.QtWidgets import (
    QDialog, QVBoxLayout, QHBoxLayout, QLabel, QLineEdit, QPushButton,
    QCheckBox, QTextEdit, QFrame, QWidget, QCompleter, QScrollArea,
    QSizePolicy,
)
from PyQt6.QtCore import Qt, QStringListModel, QTimer
from PyQt6.QtGui import QFont

from src.util.email_service import LdapLookup

logger = logging.getLogger(__name__)


class RecipientChip(QFrame):
    """A removable pill/chip widget showing an email address."""

    def __init__(self, email: str, on_remove, parent=None):
        super().__init__(parent)
        self.email = email
        self.setObjectName("recipientChip")
        layout = QHBoxLayout(self)
        layout.setContentsMargins(8, 2, 4, 2)
        layout.setSpacing(4)

        label = QLabel(email)
        label.setObjectName("chipLabel")
        layout.addWidget(label)

        remove_btn = QPushButton("\u00d7")  # multiplication sign as X
        remove_btn.setObjectName("chipRemove")
        remove_btn.setFixedSize(18, 18)
        remove_btn.setCursor(Qt.CursorShape.PointingHandCursor)
        remove_btn.clicked.connect(partial(on_remove, self))
        layout.addWidget(remove_btn)


class FlowLayout(QVBoxLayout):
    """Simple wrapper that adds chips into horizontal rows with wrapping."""
    pass


class EmailDialog(QDialog):
    """Email notification configuration dialog."""

    def __init__(self, ldap_lookup: LdapLookup | None, domain: str,
                 parent=None):
        super().__init__(parent)
        self._ldap = ldap_lookup
        self._domain = domain
        self._completer_model = QStringListModel()
        self._ldap_cache: dict[str, list[dict]] = {}
        self._ldap_timer = QTimer()
        self._ldap_timer.setSingleShot(True)
        self._ldap_timer.setInterval(300)  # debounce 300ms
        self._ldap_timer.timeout.connect(self._do_ldap_search)

        # Recipient lists
        self.recipients_to: list[str] = []
        self.recipients_cc: list[str] = []

        # Notification settings
        self.notify_test_done = True
        self.notify_threshold = False

        # Test info
        self.operator = ""
        self.serial_number = ""
        self.simulated_meter = False
        self.project_number = ""
        self.notes = ""

        self._adding_cc = False  # False = To, True = CC

        self._build_ui()
        self.setWindowTitle("Email Notifications")
        self.setMinimumWidth(520)
        self.setMinimumHeight(580)

    def _build_ui(self):
        main = QVBoxLayout(self)
        main.setSpacing(12)
        main.setContentsMargins(16, 16, 16, 16)

        # -- Header --
        header = QLabel("Email Notifications")
        header.setObjectName("dialogHeader")
        header.setFont(QFont("Consolas", 14))
        main.addWidget(header)

        # -- Recipient input row --
        input_row = QHBoxLayout()
        input_row.setSpacing(4)

        self._to_cc_btn = QPushButton("To")
        self._to_cc_btn.setObjectName("toCcToggle")
        self._to_cc_btn.setFixedWidth(36)
        self._to_cc_btn.setCursor(Qt.CursorShape.PointingHandCursor)
        self._to_cc_btn.clicked.connect(self._toggle_to_cc)
        input_row.addWidget(self._to_cc_btn)

        self._name_input = QLineEdit()
        self._name_input.setPlaceholderText("Type a name to search...")
        self._name_input.setObjectName("nameInput")
        self._name_input.textChanged.connect(self._on_name_changed)
        self._name_input.returnPressed.connect(self._add_recipient)

        # Completer
        self._completer = QCompleter()
        self._completer.setModel(self._completer_model)
        self._completer.setCaseSensitivity(Qt.CaseSensitivity.CaseInsensitive)
        self._completer.setFilterMode(Qt.MatchFlag.MatchContains)
        self._completer.setCompletionMode(QCompleter.CompletionMode.PopupCompletion)
        self._completer.activated.connect(self._on_completer_activated)
        self._name_input.setCompleter(self._completer)

        input_row.addWidget(self._name_input, 1)

        domain_label = QLabel(self._domain)
        domain_label.setObjectName("domainLabel")
        input_row.addWidget(domain_label)

        add_btn = QPushButton("Add")
        add_btn.setObjectName("addRecipientBtn")
        add_btn.clicked.connect(self._add_recipient)
        input_row.addWidget(add_btn)

        main.addLayout(input_row)

        # -- To recipients area --
        to_label = QLabel("To:")
        to_label.setObjectName("sectionLabel")
        main.addWidget(to_label)

        self._to_scroll = QScrollArea()
        self._to_scroll.setWidgetResizable(True)
        self._to_scroll.setMaximumHeight(70)
        self._to_scroll.setHorizontalScrollBarPolicy(
            Qt.ScrollBarPolicy.ScrollBarAlwaysOff)
        self._to_container = QWidget()
        self._to_flow = _WrapLayout(self._to_container)
        self._to_flow.setSpacing(4)
        self._to_scroll.setWidget(self._to_container)
        main.addWidget(self._to_scroll)

        # -- CC recipients area --
        cc_label = QLabel("CC:")
        cc_label.setObjectName("sectionLabel")
        main.addWidget(cc_label)

        self._cc_scroll = QScrollArea()
        self._cc_scroll.setWidgetResizable(True)
        self._cc_scroll.setMaximumHeight(70)
        self._cc_scroll.setHorizontalScrollBarPolicy(
            Qt.ScrollBarPolicy.ScrollBarAlwaysOff)
        self._cc_container = QWidget()
        self._cc_flow = _WrapLayout(self._cc_container)
        self._cc_flow.setSpacing(4)
        self._cc_scroll.setWidget(self._cc_container)
        main.addWidget(self._cc_scroll)

        # -- Separator --
        sep = QFrame()
        sep.setFrameShape(QFrame.Shape.HLine)
        sep.setObjectName("dialogSeparator")
        main.addWidget(sep)

        # -- Notification options --
        notify_label = QLabel("Send notifications...")
        notify_label.setObjectName("sectionLabel")
        main.addWidget(notify_label)

        self._chk_test_done = QCheckBox("When test completes")
        self._chk_test_done.setChecked(self.notify_test_done)
        main.addWidget(self._chk_test_done)

        self._chk_threshold = QCheckBox("When threshold is exceeded (every 10 min)")
        self._chk_threshold.setChecked(self.notify_threshold)
        main.addWidget(self._chk_threshold)

        # -- Separator --
        sep2 = QFrame()
        sep2.setFrameShape(QFrame.Shape.HLine)
        sep2.setObjectName("dialogSeparator")
        main.addWidget(sep2)

        # -- Test info section --
        info_label = QLabel("Test Information")
        info_label.setObjectName("sectionLabel")
        main.addWidget(info_label)

        # Operator
        op_row = QHBoxLayout()
        op_row.addWidget(QLabel("Operator:"))
        self._operator_edit = QLineEdit()
        self._operator_edit.setPlaceholderText("Operator name")
        op_row.addWidget(self._operator_edit, 1)
        main.addLayout(op_row)

        # Serial number
        serial_row = QHBoxLayout()
        serial_row.addWidget(QLabel("Serial #:"))
        self._serial_edit = QLineEdit()
        self._serial_edit.setPlaceholderText("Meter serial number")
        serial_row.addWidget(self._serial_edit, 1)
        self._sim_chk = QCheckBox("Simulated")
        self._sim_chk.stateChanged.connect(
            lambda s: self._serial_edit.setEnabled(s == 0))
        serial_row.addWidget(self._sim_chk)
        main.addLayout(serial_row)

        # Project number
        proj_row = QHBoxLayout()
        proj_row.addWidget(QLabel("Project #:"))
        self._project_edit = QLineEdit()
        self._project_edit.setPlaceholderText("Project number (optional)")
        proj_row.addWidget(self._project_edit, 1)
        main.addLayout(proj_row)

        # Notes
        main.addWidget(QLabel("Notes:"))
        self._notes_edit = QTextEdit()
        self._notes_edit.setMaximumHeight(60)
        self._notes_edit.setPlaceholderText("Additional notes for email body...")
        main.addWidget(self._notes_edit)

        # -- Buttons --
        btn_row = QHBoxLayout()
        btn_row.addStretch()
        save_btn = QPushButton("Save")
        save_btn.setObjectName("emailSaveBtn")
        save_btn.clicked.connect(self._on_save)
        close_btn = QPushButton("Close")
        close_btn.clicked.connect(self.reject)
        btn_row.addWidget(save_btn)
        btn_row.addWidget(close_btn)
        main.addLayout(btn_row)

    # -- LDAP autocomplete --

    def _on_name_changed(self, text: str):
        """Debounce LDAP search as user types."""
        if len(text) < 2:
            self._completer_model.setStringList([])
            return
        self._ldap_timer.start()

    def _do_ldap_search(self):
        """Execute LDAP search and update completer."""
        query = self._name_input.text().strip()
        if not query or len(query) < 2:
            return

        if query in self._ldap_cache:
            results = self._ldap_cache[query]
        elif self._ldap:
            results = self._ldap.search(query)
            self._ldap_cache[query] = results
        else:
            # No LDAP — offer typed text as-is
            results = []

        # Build completion list: "Display Name (email)" format
        completions = []
        for r in results:
            completions.append(f"{r['name']}  —  {r['email']}")

        # Also always offer the typed text as a direct email option
        direct = query.replace(" ", ".").lower()
        if self._domain and not any(direct in c.lower() for c in completions):
            completions.append(f"{direct}{self._domain}")

        self._completer_model.setStringList(completions)

    def _on_completer_activated(self, text: str):
        """User selected a completion — extract email and set input."""
        if "  —  " in text:
            email = text.split("  —  ")[-1].strip()
        else:
            email = text.strip()

        # Strip domain if present to show just the username part
        if email.endswith(self._domain):
            username = email[: -len(self._domain)]
            self._name_input.setText(username)
        else:
            self._name_input.setText(email)

    def _toggle_to_cc(self):
        """Toggle between To and CC mode."""
        self._adding_cc = not self._adding_cc
        self._to_cc_btn.setText("CC" if self._adding_cc else "To")

    def _add_recipient(self):
        """Add current input as a recipient."""
        text = self._name_input.text().strip()
        if not text:
            return

        # Build full email
        if "@" in text:
            email = text
        else:
            email = text.replace(" ", ".").lower() + self._domain

        target_list = self.recipients_cc if self._adding_cc else self.recipients_to
        container = self._cc_container if self._adding_cc else self._to_container
        flow = self._cc_flow if self._adding_cc else self._to_flow

        # Prevent duplicates
        if email in target_list:
            self._name_input.clear()
            return

        target_list.append(email)
        chip = RecipientChip(email, self._remove_recipient, parent=container)
        flow.addWidget(chip)
        self._name_input.clear()

    def _remove_recipient(self, chip: RecipientChip):
        """Remove a recipient chip."""
        email = chip.email
        if email in self.recipients_to:
            self.recipients_to.remove(email)
        if email in self.recipients_cc:
            self.recipients_cc.remove(email)
        chip.setParent(None)
        chip.deleteLater()

    # -- Save / Load --

    def _on_save(self):
        """Collect all values and accept the dialog."""
        self.notify_test_done = self._chk_test_done.isChecked()
        self.notify_threshold = self._chk_threshold.isChecked()
        self.operator = self._operator_edit.text().strip()
        self.serial_number = (
            "Simulated Meter" if self._sim_chk.isChecked()
            else self._serial_edit.text().strip()
        )
        self.simulated_meter = self._sim_chk.isChecked()
        self.project_number = self._project_edit.text().strip()
        self.notes = self._notes_edit.toPlainText().strip()
        self.accept()

    def load_state(self, to: list[str], cc: list[str],
                   notify_done: bool, notify_thresh: bool,
                   operator: str, serial: str, simulated: bool,
                   project: str, notes: str):
        """Populate the dialog from saved state."""
        self.recipients_to = list(to)
        self.recipients_cc = list(cc)
        self.notify_test_done = notify_done
        self.notify_threshold = notify_thresh
        self.operator = operator
        self.serial_number = serial
        self.simulated_meter = simulated
        self.project_number = project
        self.notes = notes

        # Update UI
        self._chk_test_done.setChecked(notify_done)
        self._chk_threshold.setChecked(notify_thresh)
        self._operator_edit.setText(operator)
        if simulated:
            self._sim_chk.setChecked(True)
            self._serial_edit.setEnabled(False)
        else:
            self._serial_edit.setText(serial)
        self._project_edit.setText(project)
        self._notes_edit.setPlainText(notes)

        # Rebuild chips
        for email in to:
            chip = RecipientChip(email, self._remove_recipient,
                                 parent=self._to_container)
            self._to_flow.addWidget(chip)
        for email in cc:
            chip = RecipientChip(email, self._remove_recipient,
                                 parent=self._cc_container)
            self._cc_flow.addWidget(chip)


class _WrapLayout(QHBoxLayout):
    """Horizontal layout for chips (wraps via scroll area)."""

    def __init__(self, parent=None):
        super().__init__(parent)
        self.setContentsMargins(4, 4, 4, 4)
        self.setSpacing(4)
        self.setAlignment(Qt.AlignmentFlag.AlignLeft | Qt.AlignmentFlag.AlignTop)
