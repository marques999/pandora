using System.Drawing;
using System.Drawing.Drawing2D;

namespace SexyTerminal
{
    public sealed class ColorSlider : ColorUserControl
    {
        public ColorSlider()
        {
            Initialize();
        }

        protected override void Initialize()
        {
            Name = "ColorSlider";
            ClientSize = new Size(30, 256);
            base.Initialize();
        }

        protected override void DrawCrosshair(Graphics g)
        {
            DrawCrosshair(g, Pens.Black, 3, 11);
            DrawCrosshair(g, Pens.White, 4, 9);
        }

        private void DrawCrosshair(Graphics g, Pen pen, int offset, int height)
        {
            g.DrawRectangleProper(pen, new Rectangle(offset, lastPosition.Y - (height / 2), clientWidth - (offset * 2), height));
        }

        protected override void DrawHue()
        {
            using (var graphics = Graphics.FromImage(bmp))
            {
                var color = new Hsb(0.0, 1.0, 1.0, SelectedColor.Rgba.Alpha);

                for (var y = 0; y < clientHeight; y++)
                {
                    color.Hue = 1.0 - ((double)y / (clientHeight - 1));

                    using (var pen = new Pen(color))
                    {
                        graphics.DrawLine(pen, 0, y, clientWidth, y);
                    }
                }
            }
        }

        protected override void DrawSaturation()
        {
            using (var graphics = Graphics.FromImage(bmp))
            {
                var start = new Hsb(SelectedColor.Hsb.Hue, 1.0, SelectedColor.Hsb.Brightness, SelectedColor.Rgba.Alpha);
                var end = new Hsb(SelectedColor.Hsb.Hue, 0.0, SelectedColor.Hsb.Brightness, SelectedColor.Rgba.Alpha);

                using (var brush = new LinearGradientBrush(new Rectangle(0, 0, clientWidth, clientHeight), start, end, LinearGradientMode.Vertical))
                {
                    graphics.FillRectangle(brush, new Rectangle(0, 0, clientWidth, clientHeight));
                }
            }
        }

        // Y = Brightness 100 -> 0
        protected override void DrawBrightness()
        {
            using (var graphics = Graphics.FromImage(bmp))
            {
                var start = new Hsb(SelectedColor.Hsb.Hue, SelectedColor.Hsb.Saturation, 1.0, SelectedColor.Rgba.Alpha);
                var end = new Hsb(SelectedColor.Hsb.Hue, SelectedColor.Hsb.Saturation, 0.0, SelectedColor.Rgba.Alpha);

                using (var brush = new LinearGradientBrush(new Rectangle(0, 0, clientWidth, clientHeight), start, end, LinearGradientMode.Vertical))
                {
                    graphics.FillRectangle(brush, new Rectangle(0, 0, clientWidth, clientHeight));
                }
            }
        }

        protected override void DrawRed()
        {
            using (var graphics = Graphics.FromImage(bmp))
            {
                var start = new Rgba(255, SelectedColor.Rgba.Green, SelectedColor.Rgba.Blue, SelectedColor.Rgba.Alpha);
                var end = new Rgba(0, SelectedColor.Rgba.Green, SelectedColor.Rgba.Blue, SelectedColor.Rgba.Alpha);

                using (var brush = new LinearGradientBrush(new Rectangle(0, 0, clientWidth, clientHeight), start, end, LinearGradientMode.Vertical))
                {
                    graphics.FillRectangle(brush, new Rectangle(0, 0, clientWidth, clientHeight));
                }
            }
        }

        protected override void DrawGreen()
        {
            using (var graphics = Graphics.FromImage(bmp))
            {
                var start = new Rgba(SelectedColor.Rgba.Red, 255, SelectedColor.Rgba.Blue, SelectedColor.Rgba.Alpha);
                var end = new Rgba(SelectedColor.Rgba.Red, 0, SelectedColor.Rgba.Blue, SelectedColor.Rgba.Alpha);

                using (var brush = new LinearGradientBrush(new Rectangle(0, 0, clientWidth, clientHeight), start, end, LinearGradientMode.Vertical))
                {
                    graphics.FillRectangle(brush, new Rectangle(0, 0, clientWidth, clientHeight));
                }
            }
        }

        protected override void DrawBlue()
        {
            using (var graphics = Graphics.FromImage(bmp))
            {
                var start = new Rgba(SelectedColor.Rgba.Red, SelectedColor.Rgba.Green, 255, SelectedColor.Rgba.Alpha);
                var end = new Rgba(SelectedColor.Rgba.Red, SelectedColor.Rgba.Green, 0, SelectedColor.Rgba.Alpha);

                using (var brush = new LinearGradientBrush(new Rectangle(0, 0, clientWidth, clientHeight), start, end, LinearGradientMode.Vertical))
                {
                    graphics.FillRectangle(brush, new Rectangle(0, 0, clientWidth, clientHeight));
                }
            }
        }
    }
}