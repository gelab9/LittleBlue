Public Class frmCI501TCA_Defaults

    Private Sub CI501TCA_Defaults_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        cbMaximumVoltage.SelectedIndex = My.Settings.iCI501TAC_MAX_VOLTAGE
        txtControllerDeadBand.Text = My.Settings.strCI501TAC_CONTROLLER_DEADBAND
        txtAccuracyDeadband.Text = My.Settings.strCI501TAC_ACCURACY_DEADBAND
        txtNominalVoltage.Text = My.Settings.strCI501TAC_NOMINAL_VOLTAGE
        chkCopyValuesOnSave.Checked = My.Settings.bCI501TAC_CopyValuesOnSave
        cbDefaultCurrent.SelectedIndex = My.Settings.iCI501TAC_Default_Current
    End Sub


    Private Sub btnDiscard_Click(sender As System.Object, e As System.EventArgs) Handles btnDiscard.Click
        Me.Close()
    End Sub

    Private Sub btnCI501TAC_DefaultSave_Click(sender As System.Object, e As System.EventArgs) Handles btnCI501TAC_DefaultSave.Click

        My.Settings.iCI501TAC_MAX_VOLTAGE = cbMaximumVoltage.SelectedItem
        My.Settings.strCI501TAC_CONTROLLER_DEADBAND = txtControllerDeadBand.Text
        My.Settings.strCI501TAC_ACCURACY_DEADBAND = txtAccuracyDeadband.Text
        My.Settings.strCI501TAC_NOMINAL_VOLTAGE = txtNominalVoltage.Text
        My.Settings.bCI501TAC_CopyValuesOnSave = chkCopyValuesOnSave.Checked
        My.Settings.iCI501TAC_Default_Current = cbDefaultCurrent.SelectedItem

        If cbMaximumVoltage.SelectedItem = 0 Then
            My.Settings.strCI501TAC_MaxVoltage = "135.0"
        Else
            My.Settings.strCI501TAC_MaxVoltage = "270.0"
        End If




        Me.Close()

    End Sub

End Class