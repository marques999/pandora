Public Class LoginForm1
    Private Sub OK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK.Click
        If UsernameTextBox.Text = My.Resources.UserName And PasswordTextBox.Text = My.Resources.Password Then
            Me.Close()
            Dialog2.ShowDialog()
        Else
            MsgBox("Login inválido. A aplicação irá terminar", MsgBoxStyle.Critical, "Erro")
            Application.Exit()
        End If
        Me.Close()
    End Sub
    Private Sub Cancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel.Click
        Me.Close()
    End Sub
    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        If UsernameTextBox.Text = "" Or PasswordTextBox.Text = "" Then
            OK.Enabled = False
        Else
            OK.Enabled = True
        End If
    End Sub
    Private Sub LoginForm1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Timer1.Start()
    End Sub
    Private Sub LoginForm1_FormClosing(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing
        Timer1.Stop()
    End Sub
End Class