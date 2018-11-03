using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.CompilerServices;

namespace SexyTerminal
{
    public static class GraphicsExtensions
    {
        public static Rectangle SizeOffset(this Rectangle rect, int width, int height)
        {
            return new Rectangle(rect.X, rect.Y, rect.Width + width, rect.Height + height);
        }

        public static Rectangle SizeOffset(this Rectangle rect, int offset)
        {
            return rect.SizeOffset(offset, offset);
        }
        public static void DrawRectangleProper(this Graphics g, Pen pen, Rectangle rect)
        {
            if (pen.Width == 1)
            {
                rect = rect.SizeOffset(-1);
            }

            if (rect.Width > 0 && rect.Height > 0)
            {
                g.DrawRectangle(pen, rect);
            }
        }

        public static void DrawRectangleProper(this Graphics g, Pen pen, int x, int y, int width, int height)
        {
            DrawRectangleProper(g, pen, new Rectangle(x, y, width, height));
        }

        public static void DrawRoundedRectangle(this Graphics g, Pen pen, Rectangle rect, float radius)
        {
            g.DrawRoundedRectangle(null, pen, rect, radius);
        }

        public static void DrawRoundedRectangle(this Graphics g, Brush brush, Rectangle rect, float radius)
        {
            g.DrawRoundedRectangle(brush, null, rect, radius);
        }

        public static void AddRectangleProper(this GraphicsPath graphicsPath, RectangleF rect, float penWidth = 1)
        {
            if (penWidth == 1)
            {
                rect = new RectangleF(rect.X, rect.Y, rect.Width - 1, rect.Height - 1);
            }

            if (rect.Width > 0 && rect.Height > 0)
            {
                graphicsPath.AddRectangle(rect);
            }
        }

        public static void AddRoundedRectangleProper(this GraphicsPath graphicsPath, RectangleF rect, float radius, float penWidth = 1)
        {
            if (penWidth == 1)
            {
                rect = new RectangleF(rect.X, rect.Y, rect.Width - 1, rect.Height - 1);
            }

            if (rect.Width > 0 && rect.Height > 0)
            {
                graphicsPath.AddRoundedRectangle(rect, radius);
            }
        }

        public static void AddRoundedRectangle(this GraphicsPath gp, RectangleF rect, float radius)
        {
            if (radius <= 0f)
            {
                gp.AddRectangle(rect);
            }
            else
            {
                if (radius >= (Math.Min(rect.Width, rect.Height) / 2.0f))
                {
                    gp.AddCapsule(rect);
                }
                else
                {
                    var diameter = radius * 2.0f;
                    var size = new SizeF(diameter, diameter);
                    var arc = new RectangleF(rect.Location, size);

                    // Top left arc
                    gp.AddArc(arc, 180, 90);

                    // Top right arc
                    arc.X = rect.Right - diameter;
                    gp.AddArc(arc, 270, 90);

                    // Bottom right arc
                    arc.Y = rect.Bottom - diameter;
                    gp.AddArc(arc, 0, 90);

                    // Bottom left arc
                    arc.X = rect.Left;
                    gp.AddArc(arc, 90, 90);

                    gp.CloseFigure();
                }
            }
        }

        public static void AddCapsule(this GraphicsPath gp, RectangleF rect)
        {
            float diameter;
            RectangleF arc;

            try
            {
                if (rect.Width > rect.Height)
                {
                    // Horizontal capsule
                    diameter = rect.Height;
                    var sizeF = new SizeF(diameter, diameter);
                    arc = new RectangleF(rect.Location, sizeF);
                    gp.AddArc(arc, 90, 180);
                    arc.X = rect.Right - diameter;
                    gp.AddArc(arc, 270, 180);
                }
                else if (rect.Width < rect.Height)
                {
                    // Vertical capsule
                    diameter = rect.Width;
                    var sizeF = new SizeF(diameter, diameter);
                    arc = new RectangleF(rect.Location, sizeF);
                    gp.AddArc(arc, 180, 180);
                    arc.Y = rect.Bottom - diameter;
                    gp.AddArc(arc, 0, 180);
                }
                else
                {
                    gp.AddEllipse(rect);
                }
            }
            catch
            {
                gp.AddEllipse(rect);
            }

            gp.CloseFigure();
        }

        public static void AddDiamond(this GraphicsPath graphicsPath, RectangleF rect)
        {
            graphicsPath.AddPolygon(new[]
            {
                new PointF(rect.X + (rect.Width / 2.0f), rect.Y),
                new PointF(rect.X + rect.Width, rect.Y + (rect.Height / 2.0f)),
                new PointF(rect.X + (rect.Width / 2.0f), rect.Y + rect.Height),
                new PointF(rect.X, rect.Y + (rect.Height / 2.0f))
            });
        }

        public static void AddPolygon(this GraphicsPath graphicsPath, RectangleF rect, int sideCount)
        {
            var points = new PointF[sideCount];

            float a = 0;

            for (var i = 0; i < sideCount; i++)
            {
                points[i] = new PointF(rect.X + ((rect.Width / 2.0f) * (float)Math.Cos(a)) + (rect.Width / 2.0f),
                    rect.Y + ((rect.Height / 2.0f) * (float)Math.Sin(a)) + (rect.Height / 2.0f));

                a += (float)Math.PI * 2.0f / sideCount;
            }

            graphicsPath.AddPolygon(points);
        }

        public static void DrawRoundedRectangle(this Graphics g, Brush brush, Pen pen, Rectangle rect, float radius)
        {
            using (var graphicsPath = new GraphicsPath())
            {
                graphicsPath.AddRoundedRectangleProper(rect, radius);
                if (brush != null) g.FillPath(brush, graphicsPath);
                if (pen != null) g.DrawPath(pen, graphicsPath);
            }
        }

        public static void DrawCross(this Graphics g, Pen pen, Point center, int crossSize)
        {
            if (crossSize <= 0)
            {
                return;
            }

            g.DrawLine(pen, center.X - crossSize, center.Y, center.X + crossSize, center.Y);
            g.DrawLine(pen, center.X, center.Y - crossSize, center.X, center.Y + crossSize);
        }

        public static void DrawCornerLines(this Graphics g, Rectangle rectangle, Pen pen, int lineSize)
        {
            if (rectangle.Width <= lineSize * 2)
            {
                g.DrawLine(pen, rectangle.X, rectangle.Y, rectangle.Right - 1, rectangle.Y);
                g.DrawLine(pen, rectangle.X, rectangle.Bottom - 1, rectangle.Right - 1, rectangle.Bottom - 1);
            }
            else
            {
                g.DrawLine(pen, rectangle.X, rectangle.Y, rectangle.X + lineSize, rectangle.Y);
                g.DrawLine(pen, rectangle.Right - 1, rectangle.Y, rectangle.Right - 1 - lineSize, rectangle.Y);
                g.DrawLine(pen, rectangle.X, rectangle.Bottom - 1, rectangle.X + lineSize, rectangle.Bottom - 1);
                g.DrawLine(pen, rectangle.Right - 1, rectangle.Bottom - 1, rectangle.Right - 1 - lineSize, rectangle.Bottom - 1);
            }

            if (rectangle.Height <= lineSize * 2)
            {
                g.DrawLine(pen, rectangle.X, rectangle.Y, rectangle.X, rectangle.Bottom - 1);
                g.DrawLine(pen, rectangle.Right - 1, rectangle.Y, rectangle.Right - 1, rectangle.Bottom - 1);
            }
            else
            {
                g.DrawLine(pen, rectangle.X, rectangle.Y, rectangle.X, rectangle.Y + lineSize);

                // Top right
                g.DrawLine(pen, rectangle.Right - 1, rectangle.Y, rectangle.Right - 1, rectangle.Y + lineSize);

                // Bottom left
                g.DrawLine(pen, rectangle.X, rectangle.Bottom - 1, rectangle.X, rectangle.Bottom - 1 - lineSize);
                g.DrawLine(pen, rectangle.Right - 1, rectangle.Bottom - 1, rectangle.Right - 1, rectangle.Bottom - 1 - lineSize);
            }
        }

        public static void SetHighQuality(this Graphics g)
        {
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = SmoothingMode.HighQuality;
        }

        public static void DrawTextWithOutline(this Graphics graphics, string text, PointF position, Font font, Color textColor, Color borderColor, int borderSize = 2)
        {
            var smoothingMode = graphics.SmoothingMode;

            graphics.SmoothingMode = SmoothingMode.HighQuality;

            using (var path = new GraphicsPath())
            {
                using (var stringFormat = new StringFormat())
                {
                    path.AddString(text, font.FontFamily, (int)font.Style, graphics.DpiY * font.SizeInPoints / 72, position, stringFormat);
                }

                if (borderSize > 0)
                {
                    using (var borderPen = new Pen(borderColor, borderSize) { LineJoin = LineJoin.Round })
                    {
                        graphics.DrawPath(borderPen, path);
                    }
                }

                using (Brush textBrush = new SolidBrush(textColor))
                {
                    graphics.FillPath(textBrush, path);
                }
            }

            graphics.SmoothingMode = smoothingMode;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DrawTextWithShadow(this Graphics g, string text, PointF position, Font font, Brush textBrush, Brush shadowBrush)
        {
            DrawTextWithShadow(g, text, position, font, textBrush, shadowBrush, new Point(1, 1));
        }

        public static void DrawTextWithShadow(this Graphics g, string text, PointF position, Font font, Brush textBrush, Brush shadowBrush, Point shadowOffset)
        {
            g.DrawString(text, font, shadowBrush, position.X + shadowOffset.X, position.Y + shadowOffset.Y);
            g.DrawString(text, font, textBrush, position.X, position.Y);
        }
    }
}