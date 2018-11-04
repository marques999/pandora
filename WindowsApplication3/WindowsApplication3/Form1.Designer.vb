<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub
    Private components As System.ComponentModel.IContainer
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
        Me.KryptonListBox1 = New ComponentFactory.Krypton.Toolkit.KryptonListBox()
        Me.KryptonPalette1 = New ComponentFactory.Krypton.Toolkit.KryptonPalette(Me.components)
        Me.KryptonManager1 = New ComponentFactory.Krypton.Toolkit.KryptonManager(Me.components)
        Me.WebBrowser1 = New System.Windows.Forms.WebBrowser()
        Me.SuspendLayout()
        Me.KryptonListBox1.BackStyle = ComponentFactory.Krypton.Toolkit.PaletteBackStyle.InputControlCustom1
        Me.KryptonListBox1.Items.AddRange(New Object() {"10yard", "3wonders", "9ballsht"})
        Me.KryptonListBox1.Location = New System.Drawing.Point(15, 15)
        Me.KryptonListBox1.Name = "KryptonListBox1"
        Me.KryptonListBox1.Palette = Me.KryptonPalette1
        Me.KryptonListBox1.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Custom
        Me.KryptonListBox1.Size = New System.Drawing.Size(256, 490)
        Me.KryptonListBox1.Sorted = True
        Me.KryptonListBox1.StateActive.Back.Color1 = System.Drawing.Color.White
        Me.KryptonListBox1.StateCheckedPressed.Item.Back.Color1 = System.Drawing.Color.Red
        Me.KryptonListBox1.StateCheckedPressed.Item.Back.Color2 = System.Drawing.Color.Red
        Me.KryptonListBox1.StateCommon.Item.Content.LongText.Color1 = System.Drawing.Color.White
        Me.KryptonListBox1.StateCommon.Item.Content.LongText.Color2 = System.Drawing.Color.White
        Me.KryptonListBox1.StateCommon.Item.Content.ShortText.Color1 = System.Drawing.Color.White
        Me.KryptonListBox1.StateCommon.Item.Content.ShortText.Color2 = System.Drawing.Color.White
        Me.KryptonListBox1.StateCommon.Item.Content.ShortText.ColorAlign = ComponentFactory.Krypton.Toolkit.PaletteRectangleAlign.Local
        Me.KryptonListBox1.StateCommon.Item.Content.ShortText.ColorAngle = 0.0!
        Me.KryptonListBox1.StateCommon.Item.Content.ShortText.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid
        Me.KryptonListBox1.StateCommon.Item.Content.ShortText.Font = New System.Drawing.Font("Segoe UI", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.KryptonListBox1.TabIndex = 0
        Me.KryptonPalette1.BasePaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.ProfessionalSystem
        Me.KryptonPalette1.InputControlStyles.InputControlCommon.StateCommon.Content.LongText.Color1 = System.Drawing.Color.White
        Me.KryptonPalette1.InputControlStyles.InputControlCommon.StateCommon.Content.LongText.Color2 = System.Drawing.Color.White
        Me.KryptonPalette1.InputControlStyles.InputControlCommon.StateCommon.Content.ShortText.Color1 = System.Drawing.Color.White
        Me.KryptonPalette1.InputControlStyles.InputControlCommon.StateCommon.Content.ShortText.Color2 = System.Drawing.Color.White
        Me.KryptonPalette1.InputControlStyles.InputControlCustom1.StateActive.Border.DrawBorders = ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.None
        Me.KryptonPalette1.InputControlStyles.InputControlCustom1.StateCommon.Back.Color1 = System.Drawing.Color.Transparent
        Me.KryptonPalette1.InputControlStyles.InputControlCustom1.StateCommon.Back.Color2 = System.Drawing.Color.Transparent
        Me.KryptonPalette1.InputControlStyles.InputControlCustom1.StateCommon.Back.Image = CType(resources.GetObject("KryptonPalette1.InputControlStyles.InputControlCustom1.StateCommon.Back.Image"), System.Drawing.Image)
        Me.KryptonPalette1.InputControlStyles.InputControlCustom1.StateCommon.Back.ImageAlign = ComponentFactory.Krypton.Toolkit.PaletteRectangleAlign.Control
        Me.KryptonPalette1.InputControlStyles.InputControlCustom1.StateCommon.Back.ImageStyle = ComponentFactory.Krypton.Toolkit.PaletteImageStyle.CenterMiddle
        Me.KryptonPalette1.InputControlStyles.InputControlCustom1.StateCommon.Border.Color1 = System.Drawing.Color.White
        Me.KryptonPalette1.InputControlStyles.InputControlCustom1.StateCommon.Border.Color2 = System.Drawing.Color.White
        Me.KryptonPalette1.InputControlStyles.InputControlCustom1.StateCommon.Border.ColorAlign = ComponentFactory.Krypton.Toolkit.PaletteRectangleAlign.Form
        Me.KryptonPalette1.InputControlStyles.InputControlCustom1.StateCommon.Border.Draw = ComponentFactory.Krypton.Toolkit.InheritBool.[True]
        Me.KryptonPalette1.InputControlStyles.InputControlCustom1.StateCommon.Border.DrawBorders = CType((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top Or ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) _
                    Or ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) _
                    Or ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right), ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)
        Me.KryptonPalette1.InputControlStyles.InputControlCustom1.StateCommon.Border.Rounding = 0
        Me.KryptonPalette1.InputControlStyles.InputControlCustom1.StateCommon.Border.Width = 0
        Me.KryptonPalette1.InputControlStyles.InputControlCustom1.StateCommon.Content.LongText.Color1 = System.Drawing.Color.White
        Me.KryptonPalette1.InputControlStyles.InputControlCustom1.StateCommon.Content.LongText.Color2 = System.Drawing.Color.White
        Me.KryptonPalette1.InputControlStyles.InputControlCustom1.StateCommon.Content.LongText.Font = New System.Drawing.Font("Segoe UI", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.KryptonPalette1.InputControlStyles.InputControlCustom1.StateCommon.Content.ShortText.Color1 = System.Drawing.Color.White
        Me.KryptonPalette1.InputControlStyles.InputControlCustom1.StateCommon.Content.ShortText.Color2 = System.Drawing.Color.White
        Me.KryptonPalette1.InputControlStyles.InputControlCustom1.StateCommon.Content.ShortText.Font = New System.Drawing.Font("Segoe UI", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.KryptonPalette1.InputControlStyles.InputControlCustom1.StateNormal.Border.DrawBorders = ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.None
        Me.KryptonPalette1.InputControlStyles.InputControlStandalone.StateActive.Back.Color1 = System.Drawing.Color.Transparent
        Me.KryptonManager1.GlobalPalette = Me.KryptonPalette1
        Me.KryptonManager1.GlobalPaletteMode = ComponentFactory.Krypton.Toolkit.PaletteModeManager.Custom
        Me.WebBrowser1.IsWebBrowserContextMenuEnabled = False
        Me.WebBrowser1.Location = New System.Drawing.Point(277, 15)
        Me.WebBrowser1.MinimumSize = New System.Drawing.Size(20, 20)
        Me.WebBrowser1.Name = "WebBrowser1"
        Me.WebBrowser1.Size = New System.Drawing.Size(506, 490)
        Me.WebBrowser1.TabIndex = 2
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackgroundImage = CType(resources.GetObject("$this.BackgroundImage"), System.Drawing.Image)
        Me.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.ClientSize = New System.Drawing.Size(794, 512)
        Me.Controls.Add(Me.WebBrowser1)
        Me.Controls.Add(Me.KryptonListBox1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "Form1"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "MAME Browser"
        Me.ResumeLayout(False)
    End Sub
    Friend WithEvents KryptonListBox1 As ComponentFactory.Krypton.Toolkit.KryptonListBox
    Friend WithEvents KryptonPalette1 As ComponentFactory.Krypton.Toolkit.KryptonPalette
    Friend WithEvents KryptonManager1 As ComponentFactory.Krypton.Toolkit.KryptonManager
    Friend WithEvents WebBrowser1 As System.Windows.Forms.WebBrowser
End Class