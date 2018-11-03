using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace SexyTerminal
{
    public class PointInfo
    {
        public Point Position { get; set; }
        public Color Color { get; set; }
    }
    public partial class ColorPickerForm : Form
    {
        public Func<PointInfo> OpenScreenColorPicker;

        public MyColor NewColor { get; private set; }
        public MyColor OldColor { get; private set; }
        public bool IsScreenColorPickerMode { get; private set; }

        private bool _oldColorExist;
        private bool _controlChangingColor;

        public ColorPickerForm(Color currentColor, bool isScreenColorPickerMode = false)
        {
            InitializeComponent();

            IsScreenColorPickerMode = isScreenColorPickerMode;

            PrepareRecentColors();
            SetCurrentColor(currentColor, !IsScreenColorPickerMode);

            btnOK.Visible = btnCancel.Visible = !IsScreenColorPickerMode;
        }

        public void EnableScreenColorPickerButton(Func<PointInfo> openScreenColorPicker)
        {
            OpenScreenColorPicker = openScreenColorPicker;
            btnScreenColorPicker.Visible = true;
        }

        public static bool PickColor(Color currentColor, out Color newColor, Form owner = null, Func<PointInfo> openScreenColorPicker = null)
        {
            using (var dialog = new ColorPickerForm(currentColor))
            {
                if (openScreenColorPicker != null)
                {
                    dialog.EnableScreenColorPickerButton(openScreenColorPicker);
                }

                if (dialog.ShowDialog(owner) == DialogResult.OK)
                {
                    newColor = dialog.NewColor;
                    return true;
                }
            }

            newColor = currentColor;
            return false;
        }

        private void PrepareRecentColors()
        {
            var length = Math.Min(Program.RecentColors.Count, Program.RecentColorsMax);

            for (var i = 0; i < length; i++)
            {
                var colorButton = new ColorButton()
                {
                    Color = Program.RecentColors[i],
                    Size = new Size(16, 16),
                    Margin = new Padding(1),
                    BorderColor = Color.FromArgb(100, 100, 100),
                    Offset = 0,
                    HoverEffect = true,
                    ManualButtonClick = true
                };

                colorButton.Click += (sender, e) => SetCurrentColor(colorButton.Color, true);

                flpRecentColors.Controls.Add(colorButton);
                if ((i + 1) % 16 == 0) flpRecentColors.SetFlowBreak(colorButton, true);
            }
        }

        private static void AddRecentColor(Color color)
        {
            Program.RecentColors.Remove(color);

            if (Program.RecentColors.Count >= Program.RecentColorsMax)
            {
                Program.RecentColors.RemoveRange(Program.RecentColorsMax - 1, Program.RecentColors.Count - Program.RecentColorsMax + 1);
            }

            Program.RecentColors.Insert(0, color);
        }

        public void SetCurrentColor(Color currentColor, bool keepPreviousColor)
        {
            _oldColorExist = keepPreviousColor;
            lblOld.Visible = _oldColorExist;
            NewColor = OldColor = currentColor;
            colorPicker.ChangeColor(currentColor);
            nudAlpha.SetValue(currentColor.A);
            DrawPreviewColors();
        }

        private void UpdateColor(int x, int y)
        {
            UpdateColor(x, y, NumberExtensions.GetPixelColor(x, y));
        }

        private void UpdateColor(int x, int y, Color color)
        {
            txtX.Text = x.ToString();
            txtY.Text = y.ToString();
            colorPicker.ChangeColor(color);
        }

        private void UpdateControls(MyColor color, ColorType type)
        {
            DrawPreviewColors();
            _controlChangingColor = true;

            if (type != ColorType.HSB)
            {
                nudHue.SetValue((decimal)Math.Round(color.Hsb.Hue360));
                nudSaturation.SetValue((decimal)Math.Round(color.Hsb.Saturation100));
                nudBrightness.SetValue((decimal)Math.Round(color.Hsb.Brightness100));
            }

            if (type != ColorType.RGBA)
            {
                nudRed.SetValue(color.Rgba.Red);
                nudGreen.SetValue(color.Rgba.Green);
                nudBlue.SetValue(color.Rgba.Blue);
                nudAlpha.SetValue(color.Rgba.Alpha);
            }

            if (type != ColorType.CMYK)
            {
                nudCyan.SetValue((decimal)color.Cmyk.Cyan100);
                nudMagenta.SetValue((decimal)color.Cmyk.Magenta100);
                nudYellow.SetValue((decimal)color.Cmyk.Yellow100);
                nudKey.SetValue((decimal)color.Cmyk.Key100);
            }

            if (type != ColorType.Hex)
            {
                txtHex.Text = ColorHelpers.ColorToHex(color);
            }

            if (type != ColorType.Decimal)
            {
                txtDecimal.Text = ColorHelpers.ColorToDecimal(color).ToString();
            }

            _controlChangingColor = false;
        }

        private void DrawPreviewColors()
        {
            var bmp = new Bitmap(pbColorPreview.ClientSize.Width, pbColorPreview.ClientSize.Height);

            using (var g = Graphics.FromImage(bmp))
            {
                var bmpHeight = bmp.Height;

                if (_oldColorExist)
                {
                    bmpHeight /= 2;

                    using (var oldColorBrush = new SolidBrush(OldColor))
                    {
                        g.FillRectangle(oldColorBrush, new Rectangle(0, bmpHeight, bmp.Width, bmpHeight));
                    }
                }

                using (var newColorBrush = new SolidBrush(NewColor))
                {
                    g.FillRectangle(newColorBrush, new Rectangle(0, 0, bmp.Width, bmpHeight));
                }
            }

            using (bmp)
            {
                pbColorPreview.LoadImage(bmp);
            }
        }

        private void ColorPickerForm_Shown(object sender, EventArgs e)
        {
            this.ForceActivate();
        }

        private void colorPicker_ColorChanged(object sender, ColorEventArgs e)
        {
            NewColor = e.Color;
            UpdateControls(NewColor, e.ColorType);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            AddRecentColor(NewColor);
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void rbHue_CheckedChanged(object sender, EventArgs e)
        {
            if (rbHue.Checked) colorPicker.DrawStyle = DrawStyle.Hue;
        }

        private void rbSaturation_CheckedChanged(object sender, EventArgs e)
        {
            if (rbSaturation.Checked) colorPicker.DrawStyle = DrawStyle.Saturation;
        }

        private void rbBrightness_CheckedChanged(object sender, EventArgs e)
        {
            if (rbBrightness.Checked) colorPicker.DrawStyle = DrawStyle.Brightness;
        }

        private void rbRed_CheckedChanged(object sender, EventArgs e)
        {
            if (rbRed.Checked) colorPicker.DrawStyle = DrawStyle.Red;
        }

        private void rbGreen_CheckedChanged(object sender, EventArgs e)
        {
            if (rbGreen.Checked) colorPicker.DrawStyle = DrawStyle.Green;
        }

        private void rbBlue_CheckedChanged(object sender, EventArgs e)
        {
            if (rbBlue.Checked) colorPicker.DrawStyle = DrawStyle.Blue;
        }

        private void RGB_ValueChanged(object sender, EventArgs e)
        {
            if (!_controlChangingColor)
            {
                colorPicker.ChangeColor(Color.FromArgb((int)nudAlpha.Value, (int)nudRed.Value, (int)nudGreen.Value, (int)nudBlue.Value), ColorType.RGBA);
            }
        }

        private void cbTransparent_Click(object sender, EventArgs e)
        {
            if (nudAlpha.Value == 0)
            {
                nudAlpha.Value = 255;
            }
            else
            {
                nudAlpha.Value = 0;
            }
        }

        private void HSB_ValueChanged(object sender, EventArgs e)
        {
            if (!_controlChangingColor)
            {
                colorPicker.ChangeColor(new Hsb((int)nudHue.Value, (int)nudSaturation.Value, (int)nudBrightness.Value, (int)nudAlpha.Value).ToColor(), ColorType.HSB);
            }
        }

        private void CMYK_ValueChanged(object sender, EventArgs e)
        {
            if (!_controlChangingColor)
            {
                colorPicker.ChangeColor(new Cmyk((double)nudCyan.Value / 100, (double)nudMagenta.Value / 100, (double)nudYellow.Value / 100,
                    (double)nudKey.Value / 100, (int)nudAlpha.Value).ToColor(), ColorType.CMYK);
            }
        }

        private void txtHex_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!_controlChangingColor)
                {
                    colorPicker.ChangeColor(ColorHelpers.HexToColor(txtHex.Text), ColorType.Hex);
                }
            }
            catch
            {
            }
        }

        private void txtDecimal_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!_controlChangingColor && int.TryParse(txtDecimal.Text, out var dec))
                {
                    colorPicker.ChangeColor(ColorHelpers.DecimalToColor(dec), ColorType.Decimal);
                }
            }
            catch
            {
            }
        }

        private void pbColorPreview_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && _oldColorExist)
            {
                colorPicker.ChangeColor(OldColor);
            }
        }

        private void btnScreenColorPicker_Click(object sender, EventArgs e)
        {
            try
            {
                SetCurrentColor(NewColor, true);
                Hide();
                Thread.Sleep(250);

                var pointInfo = OpenScreenColorPicker();

                if (pointInfo != null)
                {
                    UpdateColor(pointInfo.Position.X, pointInfo.Position.Y, pointInfo.Color);
                }
            }
            finally
            {
                this.ForceActivate();
            }
        }
    }
}