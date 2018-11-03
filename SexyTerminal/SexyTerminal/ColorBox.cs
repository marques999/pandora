using System.Drawing;
using System.Drawing.Drawing2D;

namespace SexyTerminal
{
    /// <summary>
    /// </summary>
    public sealed class ColorBox : ColorUserControl
    {
        /// <summary>
        /// </summary>
        public ColorBox()
        {
            Initialize();
        }

        /// <summary>
        /// </summary>
        protected override void Initialize()
        {
            Name = "ColorBox";
            ClientSize = new Size(256, 256);
            base.Initialize();
        }

        /// <summary>
        /// </summary>
        /// <param name="graphics"></param>
        protected override void DrawCrosshair(Graphics graphics)
        {
            DrawCrosshair(graphics, Pens.Black, 6);
            DrawCrosshair(graphics, Pens.White, 5);
        }

        /// <summary>
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="pen"></param>
        /// <param name="size"></param>
        private void DrawCrosshair(Graphics graphics, Pen pen, int size)
        {
            graphics.DrawEllipse(pen, new Rectangle(new Point(lastPosition.X - size, lastPosition.Y - size), new Size(size * 2, size * 2)));
        }

        /// <summary>
        /// </summary>
        protected override void DrawHue()
        {
            using (var graphics = Graphics.FromImage(bmp))
            {
                var start = new Hsb(SelectedColor.Hsb.Hue, 0.0, 0.0, SelectedColor.Rgba.Alpha);
                var end = new Hsb(SelectedColor.Hsb.Hue, 1.0, 0.0, SelectedColor.Rgba.Alpha);

                for (var y = 0; y < clientHeight; y++)
                {
                    start.Brightness = end.Brightness = 1.0 - (double)y / (clientHeight - 1);

                    using (var brush = new LinearGradientBrush(new Rectangle(0, 0, clientWidth, 1), start, end, LinearGradientMode.Horizontal))
                    {
                        graphics.FillRectangle(brush, new Rectangle(0, y, clientWidth, 1));
                    }
                }
            }
        }

        /// <summary>
        /// </summary>
        protected override void DrawSaturation()
        {
            using (var graphics = Graphics.FromImage(bmp))
            {
                var start = new Hsb(0.0, SelectedColor.Hsb.Saturation, 1.0, SelectedColor.Rgba.Alpha);
                var end = new Hsb(0.0, SelectedColor.Hsb.Saturation, 0.0, SelectedColor.Rgba.Alpha);

                for (var x = 0; x < clientWidth; x++)
                {
                    start.Hue = end.Hue = (double)x / (clientHeight - 1);

                    using (var brush = new LinearGradientBrush(new Rectangle(0, 0, 1, clientHeight), start, end, LinearGradientMode.Vertical))
                    {
                        graphics.FillRectangle(brush, new Rectangle(x, 0, 1, clientHeight));
                    }
                }
            }
        }

        /// <summary>
        /// </summary>
        protected override void DrawBrightness()
        {
            using (var g = Graphics.FromImage(bmp))
            {
                var start = new Hsb(0.0, 1.0, SelectedColor.Hsb.Brightness, SelectedColor.Rgba.Alpha);
                var end = new Hsb(0.0, 0.0, SelectedColor.Hsb.Brightness, SelectedColor.Rgba.Alpha);

                for (var x = 0; x < clientWidth; x++)
                {
                    start.Hue = end.Hue = (double)x / (clientHeight - 1);

                    using (var brush = new LinearGradientBrush(new Rectangle(0, 0, 1, clientHeight), start, end, LinearGradientMode.Vertical))
                    {
                        g.FillRectangle(brush, new Rectangle(x, 0, 1, clientHeight));
                    }
                }
            }
        }

        /// <summary>
        /// </summary>
        protected override void DrawRed()
        {
            using (var g = Graphics.FromImage(bmp))
            {
                var start = new Rgba(SelectedColor.Rgba.Red, 0, 0, SelectedColor.Rgba.Alpha);
                var end = new Rgba(SelectedColor.Rgba.Red, 0, 255, SelectedColor.Rgba.Alpha);

                for (var y = 0; y < clientHeight; y++)
                {
                    start.Green = end.Green = Round(255 - 255 * (double)y / (clientHeight - 1));

                    using (var brush = new LinearGradientBrush(new Rectangle(0, 0, clientWidth, 1), start, end, LinearGradientMode.Horizontal))
                    {
                        g.FillRectangle(brush, new Rectangle(0, y, clientWidth, 1));
                    }
                }
            }
        }

        /// <summary>
        /// </summary>
        protected override void DrawGreen()
        {
            using (var graphics = Graphics.FromImage(bmp))
            {
                var start = new Rgba(0, SelectedColor.Rgba.Green, 0, SelectedColor.Rgba.Alpha);
                var end = new Rgba(0, SelectedColor.Rgba.Green, 255, SelectedColor.Rgba.Alpha);

                for (var y = 0; y < clientHeight; y++)
                {
                    start.Red = end.Red = Round(255 - 255 * (double)y / (clientHeight - 1));

                    using (var brush = new LinearGradientBrush(new Rectangle(0, 0, clientWidth, 1), start, end, LinearGradientMode.Horizontal))
                    {
                        graphics.FillRectangle(brush, new Rectangle(0, y, clientWidth, 1));
                    }
                }
            }
        }

        /// <summary>
        /// </summary>
        protected override void DrawBlue()
        {
            using (var graphics = Graphics.FromImage(bmp))
            {
                var start = new Rgba(0, 0, SelectedColor.Rgba.Blue, SelectedColor.Rgba.Alpha);
                var end = new Rgba(255, 0, SelectedColor.Rgba.Blue, SelectedColor.Rgba.Alpha);

                for (var y = 0; y < clientHeight; y++)
                {
                    start.Green = end.Green = Round(255 - 255 * (double)y / (clientHeight - 1));

                    using (var brush = new LinearGradientBrush(new Rectangle(0, 0, clientWidth, 1), start, end, LinearGradientMode.Horizontal))
                    {
                        graphics.FillRectangle(brush, new Rectangle(0, y, clientWidth, 1));
                    }
                }
            }
        }
    }
}