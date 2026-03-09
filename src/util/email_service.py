"""Email sending via SMTP and LDAP contact lookup."""

import logging
import smtplib
from email.mime.text import MIMEText
from email.mime.multipart import MIMEMultipart
from email.mime.base import MIMEBase
from email import encoders
from pathlib import Path
from datetime import datetime
from typing import Optional

logger = logging.getLogger(__name__)


class LdapLookup:
    """Search Active Directory for user names and email addresses."""

    def __init__(self, server: str, base_dn: str, port: int = 389):
        self._server = server
        self._base_dn = base_dn
        self._port = port
        self._conn = None

    def _connect(self):
        """Establish LDAP connection (lazy, reusable)."""
        if self._conn and self._conn.bound:
            return
        try:
            import ldap3
            server = ldap3.Server(self._server, port=self._port, get_info=ldap3.NONE)
            self._conn = ldap3.Connection(server, auto_bind=True)
        except Exception as e:
            logger.warning(f"LDAP connect failed: {e}")
            self._conn = None

    def search(self, query: str, limit: int = 15) -> list[dict]:
        """Search AD for users matching a partial name.

        Returns list of {"name": "John Doe", "email": "john.doe@landisgyr.com"}.
        """
        if not query or len(query) < 2:
            return []

        self._connect()
        if not self._conn:
            return []

        try:
            import ldap3
            search_filter = (
                f"(&(objectClass=person)"
                f"(|(cn=*{query}*)(sAMAccountName=*{query}*)(mail=*{query}*)))"
            )
            self._conn.search(
                self._base_dn,
                search_filter,
                search_scope=ldap3.SUBTREE,
                attributes=["cn", "mail", "displayName"],
                size_limit=limit,
            )
            results = []
            for entry in self._conn.entries:
                name = str(entry.displayName) if hasattr(entry, "displayName") and entry.displayName else str(entry.cn)
                email = str(entry.mail) if hasattr(entry, "mail") and entry.mail else ""
                if name and email:
                    results.append({"name": name, "email": email})
            return results
        except Exception as e:
            logger.warning(f"LDAP search failed: {e}")
            return []

    def close(self):
        if self._conn:
            try:
                self._conn.unbind()
            except Exception:
                pass
            self._conn = None


class EmailService:
    """Send email notifications via SMTP."""

    def __init__(self, smtp_server: str, smtp_port: int = 25,
                 use_tls: bool = False, from_address: str = ""):
        self._smtp_server = smtp_server
        self._smtp_port = smtp_port
        self._use_tls = use_tls
        self._from_address = from_address

    def _send(self, to: list[str], cc: list[str], subject: str,
              body: str, attachment_path: Optional[str] = None):
        """Send an email via SMTP."""
        if not self._smtp_server:
            logger.error("SMTP server not configured — email not sent")
            return False

        msg = MIMEMultipart()
        msg["From"] = self._from_address
        msg["To"] = "; ".join(to)
        if cc:
            msg["Cc"] = "; ".join(cc)
        msg["Subject"] = subject
        msg.attach(MIMEText(body, "plain"))

        if attachment_path:
            path = Path(attachment_path)
            if path.exists():
                with open(path, "rb") as f:
                    part = MIMEBase("application", "octet-stream")
                    part.set_payload(f.read())
                encoders.encode_base64(part)
                part.add_header(
                    "Content-Disposition",
                    f"attachment; filename={path.name}",
                )
                msg.attach(part)

        all_recipients = to + cc
        try:
            with smtplib.SMTP(self._smtp_server, self._smtp_port, timeout=30) as server:
                if self._use_tls:
                    server.starttls()
                server.sendmail(self._from_address, all_recipients, msg.as_string())
            logger.info(f"Email sent: {subject} -> {all_recipients}")
            return True
        except Exception as e:
            logger.error(f"Failed to send email: {e}")
            return False

    def send_test_complete(self, to: list[str], cc: list[str],
                           operator: str, serial: str, project: str,
                           notes: str, metrics: dict,
                           log_path: str, start_time: str, end_time: str) -> bool:
        """Send test-complete notification email with log attachment."""
        now = datetime.now().strftime("%Y-%m-%d %H:%M:%S")
        subject = f"Temperature Rise Test Complete at {now}"

        body_parts = [
            "Hello,",
            "",
            f"The Temperature Rise Test started at {start_time} "
            f"is now complete at {end_time}.",
            "The log file is attached to this e-mail.",
            "",
            f"Operator: {operator}",
            f"Meter Serial Number: {serial if serial else 'N/A'}",
            f"Project Number: {project if project else 'N/A'}",
            f"Note: {notes}",
            "=-=-=-=-=-=-=-=-=-=-=",
            f"Voltage: {metrics.get('volt', 'N/A')} V",
            f"Current: {metrics.get('amp', 'N/A')} A",
            f"Frequency: {metrics.get('frequency', 'N/A')} Hz",
            f"Phase: {metrics.get('phase', 'N/A')}",
            "",
            "Regards,",
            "Current & Temperature Program",
        ]
        body = "\n".join(body_parts)
        return self._send(to, cc, subject, body, attachment_path=log_path)

    def send_threshold_warning(self, to: list[str], cc: list[str],
                               operator: str, serial: str, project: str,
                               warning_body: str, metrics: dict) -> bool:
        """Send threshold warning email."""
        subject = "Temperature Rise Test Warning"
        now = datetime.now().strftime("%Y-%m-%d %H:%M:%S")

        body_parts = [
            "Hello,",
            "",
            f"At {now}, the threshold is reached. See details below.",
            "",
            warning_body,
            "",
            "The program will check the temperature difference every "
            "10 minutes thereafter until the temperature difference is "
            "less than the threshold or the test is complete.",
            "",
            f"Operator: {operator}",
            f"Meter Serial Number: {serial if serial else 'N/A'}",
            f"Project Number: {project if project else 'N/A'}",
            "=-=-=-=-=-=-=-=-=-=-=",
            f"Voltage: {metrics.get('volt', 'N/A')} V",
            f"Current: {metrics.get('amp', 'N/A')} A",
            f"Frequency: {metrics.get('frequency', 'N/A')} Hz",
            f"Phase: {metrics.get('phase', 'N/A')}",
            "",
            "Regards,",
            "Current & Temperature Program",
        ]
        body = "\n".join(body_parts)
        return self._send(to, cc, subject, body)
