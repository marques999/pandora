Public Class Form1
    Dim isclicked2 As New Boolean
    Dim isclicked1 As New Boolean
    Dim isclicked3 As New Boolean
    Private Sub PictureBox1_MouseEnter(ByVal sender As Object, ByVal e As EventArgs) Handles PictureBox1.MouseEnter
        If isclicked1 = True Then
        Else
            PictureBox1.Image = My.Resources.steamokhover
        End If
    End Sub
    Private Sub PictureBox1_MouseLeave(ByVal sender As Object, ByVal e As EventArgs) Handles PictureBox1.MouseLeave
        If isclicked1 = True Then
        Else
            PictureBox1.Image = My.Resources.steamoknormal
        End If
    End Sub
    Private Sub PictureBox2_MouseEnter(ByVal sender As Object, ByVal e As EventArgs) Handles PictureBox2.MouseEnter
        If isclicked2 = True Then
        Else
            PictureBox2.Image = My.Resources.steamcancel
        End If
    End Sub
    Private Sub PictureBox2_MouseLeave(ByVal sender As Object, ByVal e As EventArgs) Handles PictureBox2.MouseLeave
        If isclicked2 = True Then
        Else
            PictureBox2.Image = My.Resources.steamnormal
        End If
    End Sub
    Private Sub PictureBox2_Click(ByVal sender As Object, ByVal e As EventArgs) Handles PictureBox2.Click
        PictureBox2.Image = My.Resources.steamclick
        PictureBox1.Image = My.Resources.steamoknormal
        isclicked2 = True
        Application.Exit()
    End Sub
    Private Sub PictureBox1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles PictureBox1.Click
        PictureBox1.Image = My.Resources.steamokclick
        isclicked1 = True
        PictureBox2.Image = My.Resources.steamnormal
        If CheckBox1.Checked = False Then
            MsgBox("You must agree to the Steam Subscriber Agreement first!", MsgBoxStyle.Critical, "Steam")
        Else
            Timer1.Start()
        End If
    End Sub
    Private Sub Form1_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        isclicked2 = False
        isclicked1 = False
    End Sub
    Private Sub PictureBox3_MouseEnter(ByVal sender As Object, ByVal e As EventArgs) Handles PictureBox3.MouseEnter
        If isclicked3 = True Then
        Else
            PictureBox3.Image = My.Resources.steamcheckhover
        End If
    End Sub
    Private Sub PictureBox3_MouseLeave(ByVal sender As Object, ByVal e As EventArgs) Handles PictureBox3.MouseLeave
        If isclicked3 = True Then
        Else
            PictureBox3.Image = My.Resources.steamchecknormal
        End If
    End Sub
    Private Sub PictureBox3_Click(ByVal sender As Object, ByVal e As EventArgs) Handles PictureBox3.Click
        If CheckBox1.Checked = True Then
            isclicked3 = False
            PictureBox3.Image = My.Resources.steamchecknormal
            CheckBox1.Checked = False
        Else
            isclicked3 = True
            PictureBox3.Image = My.Resources.steamcheckclick
            CheckBox1.Checked = True
        End If
    End Sub
    Private Sub Timer1_Tick(ByVal sender As Object, ByVal e As EventArgs) Handles Timer1.Tick
        MsgBox("HAHA GOTCHA! THIS PRANK WAS MADE BY MARQUES", MsgBoxStyle.Critical, "Steam")
    End Sub
    Private Sub PictureBox4_Click(ByVal sender As Object, ByVal e As EventArgs) Handles PictureBox4.Click
        Application.Exit()
    End Sub
    Private Sub PictureBox5_Click(ByVal sender As Object, ByVal e As EventArgs) Handles PictureBox5.Click
        WindowState = FormWindowState.Minimized
    End Sub
End Class