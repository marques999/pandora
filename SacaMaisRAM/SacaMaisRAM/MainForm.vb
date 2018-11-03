Public Class MainForm
    Private Sub Button1_Click(sender As Object, arguments As EventArgs) Handles Button1.Click
        If ComboBox1.SelectedIndex = 0 Then
            ProgressDialog.Timer1.Interval = 20
            ProgressDialog.ShowDialog()
        End If
        If ComboBox1.SelectedIndex = 1 Then
            ProgressDialog.Timer1.Interval = 40
            ProgressDialog.ShowDialog()
        End If
        If ComboBox1.SelectedIndex = 2 Then
            ProgressDialog.Timer1.Interval = 80
            ProgressDialog.ShowDialog()
        End If
        If ComboBox1.SelectedIndex = 3 Then
            ProgressDialog.Timer1.Interval = 160
            ProgressDialog.ShowDialog()
        End If
        If ComboBox1.SelectedIndex = 4 Then
            ProgressDialog.Timer1.Interval = 320
            ProgressDialog.ShowDialog()
        End If
        If ComboBox1.SelectedIndex = 5 Then
            ProgressDialog.Timer1.Interval = 640
            ProgressDialog.ShowDialog()
        End If
    End Sub
End Class