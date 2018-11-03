Public Class ProgressDialog
    Private Sub OK_Button_Click(sender As Object, arguments As EventArgs)
        DialogResult = DialogResult.OK
        Close()
    End Sub

    Private Sub Cancel_Button_Click(sender As Object, arguments As EventArgs)
        DialogResult = DialogResult.Cancel
        Close()
    End Sub

    Private Sub Dialog1_Load(sender As Object, arguments As EventArgs) Handles MyBase.Load
        Timer1.Start()
    End Sub

    Private Sub Timer1_Tick(sender As Object, arguments As EventArgs) Handles Timer1.Tick
        If ProgressBar1.Value >= 100 Then
            Timer1.Stop()
        Else
            ProgressBar1.Value = ProgressBar1.Value + 1
        End If
    End Sub
End Class