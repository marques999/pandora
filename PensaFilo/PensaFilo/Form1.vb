Public Class Form1
    Dim inta As Integer
    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        TextBox1.Text = "PensaFilo, uma colecção de 1311 frases e pensamentos filosóficos em português. Para consultar as mesmas em formato PDF, carregar no botão 'Ver Todos'." + Environment.NewLine + "build 75 [241210-1236]"
    End Sub
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        inta = Math.Floor(Rnd() * 1799)
        Label1.Text = inta
        TextBox1.Text = ReadSpecifiedLine("Frases.ini", inta)
    End Sub
    Private Sub PictureBox1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox1.Click
        Application.Exit()
    End Sub
    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Try
            My.Computer.FileSystem.WriteAllBytes(My.Computer.FileSystem.SpecialDirectories.Temp + "\temp.pdf", My.Resources.F_10CT2_1, False)
            Process1.StartInfo.Arguments = Chr(34) & My.Computer.FileSystem.SpecialDirectories.Temp + "\temp.pdf" & Chr(32)
            Process1.Start()
        Catch ex As Exception
            Dialog1.ShowDialog()
        End Try
    End Sub
    Private Sub Form1_FormClosing(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing
        If My.Computer.FileSystem.FileExists(My.Computer.FileSystem.SpecialDirectories.Temp + "\temp.pdf") Then
            My.Computer.FileSystem.DeleteFile(My.Computer.FileSystem.SpecialDirectories.Temp + "\temp.pdf")
        End If
    End Sub
    Private Sub Button1_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Button1.KeyDown
        If e.KeyData = Keys.F2 Then
            LoginForm1.ShowDialog()
        End If
    End Sub
    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        If inta < 1799 Then
            inta = inta + 1
            Label1.Text = inta
            TextBox1.Text = ReadSpecifiedLine("Frases.ini", inta)
        End If
    End Sub
    Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button5.Click
        My.Computer.Clipboard.SetText(TextBox1.Text)
        MsgBox(Chr(34) & TextBox1.Text & Chr(34) & vbCrLf & vbCrLf & "Copiado para a área de tansferência com sucesso." & vbCrLf & "Agora podes partilhar com os teus maigos no Facebook :)", MsgBoxStyle.Information, "Pensamentos Filosóficos")
    End Sub
    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        If inta > 0 Then
            inta = inta - 1
            Label1.Text = inta
            TextBox1.Text = ReadSpecifiedLine("Frases.ini", inta)
        Else
        End If
    End Sub
    Public Shared Function ReadSpecifiedLine(ByVal file As String, ByVal lineNum As Integer) As String
        Dim contents As String
        Try
            Using stream As New IO.StreamReader(file)
                contents = stream.ReadToEnd().Replace(vbCr & vbLf, vbLf).Replace(vbLf & vbCr, vbLf)
                Dim linesArray As String() = contents.Split(New Char() {ControlChars.Lf})
                If linesArray.Length > 1 Then
                    If Not lineNum > linesArray.Length AndAlso Not lineNum < 0 Then
                        Return linesArray(lineNum)
                    Else
                        Return linesArray(0)
                    End If
                Else
                    Return contents
                End If
            End Using
        Catch exception As Exception
            Return exception.ToString()
        End Try
    End Function
End Class