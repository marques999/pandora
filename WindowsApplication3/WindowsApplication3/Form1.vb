Public Class Form1
    Private Sub KryptonListBox1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles KryptonListBox1.SelectedIndexChanged
        WebBrowser1.Navigate(urlString:="http://maws.mameworld.info/minimaws/set/" + KryptonListBox1.SelectedItem.ToString)
    End Sub
    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        KryptonListBox1.Focus()
        KryptonListBox1.SelectedIndex = 0
    End Sub
End Class