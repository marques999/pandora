using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace SexyTerminal
{
    public abstract class ColorUserControl : UserControl
    {
        public event ColorEventHandler ColorChanged;

        public bool CrosshairVisible { get; set; } = true;

        public MyColor SelectedColor
        {
            get
            {
                return selectedColor;
            }
            set
            {
                selectedColor = value;

                if (this is ColorBox)
                {
                    SetBoxMarker();
                }
                else
                {
                    SetSliderMarker();
                }

                Refresh();
            }
        }

        public DrawStyle DrawStyle
        {
            get
            {
                return drawStyle;
            }
            set
            {
                drawStyle = value;

                if (this is ColorBox)
                {
                    SetBoxMarker();
                }
                else
                {
                    SetSliderMarker();
                }

                Refresh();
            }
        }

        protected Bitmap bmp;
        protected int clientWidth, clientHeight;
        protected DrawStyle drawStyle;
        protected MyColor selectedColor;
        protected bool mouseDown;
        protected Point lastPosition;
        protected Timer mouseMoveTimer;
        private readonly IContainer components = null;

        protected virtual void Initialize()
        {
            SuspendLayout();
            DoubleBuffered = true;
            clientWidth = ClientRectangle.Width;
            clientHeight = ClientRectangle.Height;
            bmp = new Bitmap(clientWidth, clientHeight, PixelFormat.Format32bppArgb);
            SelectedColor = Color.Red;
            DrawStyle = DrawStyle.Hue;

            mouseMoveTimer = new Timer
            {
                Interval = 10
            };

            mouseMoveTimer.Tick += MouseMoveTimer_Tick;
            ClientSizeChanged += EventClientSizeChanged;
            MouseDown += EventMouseDown;
            MouseEnter += EventMouseEnter;
            MouseUp += EventMouseUp;
            Paint += EventPaint;
            ResumeLayout(false);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
                bmp?.Dispose();
            }

            base.Dispose(disposing);
        }

        private void EventClientSizeChanged(object sender, EventArgs e)
        {
            clientWidth = ClientRectangle.Width;
            clientHeight = ClientRectangle.Height;
            bmp?.Dispose();
            bmp = new Bitmap(clientWidth, clientHeight, PixelFormat.Format32bppArgb);
            DrawColors();
        }

        private void EventMouseDown(object sender, MouseEventArgs e)
        {
            CrosshairVisible = true;
            mouseDown = true;
            mouseMoveTimer.Start();
        }

        private void EventMouseEnter(object sender, EventArgs e)
        {
            if (this is ColorBox)
            {
                Cursor = Cursors.Cross;
            }
        }

        private void EventMouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
            mouseMoveTimer.Stop();
        }

        private void EventPaint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;

            if (!mouseDown)
            {
                //if (SelectedColor.IsTransparent)
                //{
                //   if (bmp != null) bmp.Dispose();
                //  bmp = (Bitmap)ImageHelpers.DrawCheckers(clientWidth, clientHeight);
                // }

                DrawColors();
            }

            g.DrawImage(bmp, ClientRectangle);
            
            if (CrosshairVisible)
            {
                DrawCrosshair(g);
            }
        }

        private void MouseMoveTimer_Tick(object sender, EventArgs eventArgs)
        {
            var mousePosition = GetPoint(PointToClient(MousePosition));

            if (mouseDown && lastPosition != mousePosition)
            {
                GetPointColor(mousePosition);
                OnColorChanged();
                Refresh();
            }
        }

        protected void OnColorChanged()
        {
            ColorChanged?.Invoke(this, new ColorEventArgs(SelectedColor, DrawStyle));
        }

        protected void DrawColors()
        {
            switch (DrawStyle)
            {
            case DrawStyle.Hue:
                DrawHue();
                break;
            case DrawStyle.Saturation:
                DrawSaturation();
                break;
            case DrawStyle.Brightness:
                DrawBrightness();
                break;
            case DrawStyle.Red:
                DrawRed();
                break;
            case DrawStyle.Green:
                DrawGreen();
                break;
            case DrawStyle.Blue:
                DrawBlue();
                break;
            }
        }

        protected void SetBoxMarker()
        {
            switch (DrawStyle)
            {
                case DrawStyle.Hue:
                    lastPosition.X = Round((clientWidth - 1) * SelectedColor.Hsb.Saturation);
                    lastPosition.Y = Round((clientHeight - 1) * (1.0 - SelectedColor.Hsb.Brightness));
                    break;
                case DrawStyle.Saturation:
                    lastPosition.X = Round((clientWidth - 1) * SelectedColor.Hsb.Hue);
                    lastPosition.Y = Round((clientHeight - 1) * (1.0 - SelectedColor.Hsb.Brightness));
                    break;
                case DrawStyle.Brightness:
                    lastPosition.X = Round((clientWidth - 1) * SelectedColor.Hsb.Hue);
                    lastPosition.Y = Round((clientHeight - 1) * (1.0 - SelectedColor.Hsb.Saturation));
                    break;
                case DrawStyle.Red:
                    lastPosition.X = Round((clientWidth - 1) * (double)SelectedColor.Rgba.Blue / 255);
                    lastPosition.Y = Round((clientHeight - 1) * (1.0 - (double)SelectedColor.Rgba.Green / 255));
                    break;
                case DrawStyle.Green:
                    lastPosition.X = Round((clientWidth - 1) * (double)SelectedColor.Rgba.Blue / 255);
                    lastPosition.Y = Round((clientHeight - 1) * (1.0 - (double)SelectedColor.Rgba.Red / 255));
                    break;
                case DrawStyle.Blue:
                    lastPosition.X = Round((clientWidth - 1) * (double)SelectedColor.Rgba.Red / 255);
                    lastPosition.Y = Round((clientHeight - 1) * (1.0 - (double)SelectedColor.Rgba.Green / 255));
                    break;
            }

            lastPosition = GetPoint(lastPosition);
        }

        protected void GetBoxColor()
        {
            switch (DrawStyle)
            {
                case DrawStyle.Hue:
                    selectedColor.Hsb.Saturation = (double)lastPosition.X / (clientWidth - 1);
                    selectedColor.Hsb.Brightness = 1.0 - (double)lastPosition.Y / (clientHeight - 1);
                    selectedColor.HsbUpdate();
                    break;
                case DrawStyle.Saturation:
                    selectedColor.Hsb.Hue = (double)lastPosition.X / (clientWidth - 1);
                    selectedColor.Hsb.Brightness = 1.0 - (double)lastPosition.Y / (clientHeight - 1);
                    selectedColor.HsbUpdate();
                    break;
                case DrawStyle.Brightness:
                    selectedColor.Hsb.Hue = (double)lastPosition.X / (clientWidth - 1);
                    selectedColor.Hsb.Saturation = 1.0 - (double)lastPosition.Y / (clientHeight - 1);
                    selectedColor.HsbUpdate();
                    break;
                case DrawStyle.Red:
                    selectedColor.Rgba.Blue = Round(255 * (double)lastPosition.X / (clientWidth - 1));
                    selectedColor.Rgba.Green = Round(255 * (1.0 - (double)lastPosition.Y / (clientHeight - 1)));
                    selectedColor.RgbaUpdate();
                    break;
                case DrawStyle.Green:
                    selectedColor.Rgba.Blue = Round(255 * (double)lastPosition.X / (clientWidth - 1));
                    selectedColor.Rgba.Red = Round(255 * (1.0 - (double)lastPosition.Y / (clientHeight - 1)));
                    selectedColor.RgbaUpdate();
                    break;
                case DrawStyle.Blue:
                    selectedColor.Rgba.Red = Round(255 * (double)lastPosition.X / (clientWidth - 1));
                    selectedColor.Rgba.Green = Round(255 * (1.0 - (double)lastPosition.Y / (clientHeight - 1)));
                    selectedColor.RgbaUpdate();
                    break;
            }
        }

        protected void SetSliderMarker()
        {
            switch (DrawStyle)
            {
                case DrawStyle.Hue:
                    lastPosition.Y = clientHeight - 1 - Round((clientHeight - 1) * SelectedColor.Hsb.Hue);
                    break;
                case DrawStyle.Saturation:
                    lastPosition.Y = clientHeight - 1 - Round((clientHeight - 1) * SelectedColor.Hsb.Saturation);
                    break;
                case DrawStyle.Brightness:
                    lastPosition.Y = clientHeight - 1 - Round((clientHeight - 1) * SelectedColor.Hsb.Brightness);
                    break;
                case DrawStyle.Red:
                    lastPosition.Y = clientHeight - 1 - Round((clientHeight - 1) * (double)SelectedColor.Rgba.Red / 255);
                    break;
                case DrawStyle.Green:
                    lastPosition.Y = clientHeight - 1 - Round((clientHeight - 1) * (double)SelectedColor.Rgba.Green / 255);
                    break;
                case DrawStyle.Blue:
                    lastPosition.Y = clientHeight - 1 - Round((clientHeight - 1) * (double)SelectedColor.Rgba.Blue / 255);
                    break;
            }
            lastPosition = GetPoint(lastPosition);
        }

        protected void GetSliderColor()
        {
            switch (DrawStyle)
            {
                case DrawStyle.Hue:
                    selectedColor.Hsb.Hue = 1.0 - (double)lastPosition.Y / (clientHeight - 1);
                    selectedColor.HsbUpdate();
                    break;
                case DrawStyle.Saturation:
                    selectedColor.Hsb.Saturation = 1.0 - (double)lastPosition.Y / (clientHeight - 1);
                    selectedColor.HsbUpdate();
                    break;
                case DrawStyle.Brightness:
                    selectedColor.Hsb.Brightness = 1.0 - (double)lastPosition.Y / (clientHeight - 1);
                    selectedColor.HsbUpdate();
                    break;
                case DrawStyle.Red:
                    selectedColor.Rgba.Red = 255 - Round(255 * (double)lastPosition.Y / (clientHeight - 1));
                    selectedColor.RgbaUpdate();
                    break;
                case DrawStyle.Green:
                    selectedColor.Rgba.Green = 255 - Round(255 * (double)lastPosition.Y / (clientHeight - 1));
                    selectedColor.RgbaUpdate();
                    break;
                case DrawStyle.Blue:
                    selectedColor.Rgba.Blue = 255 - Round(255 * (double)lastPosition.Y / (clientHeight - 1));
                    selectedColor.RgbaUpdate();
                    break;
            }
        }

        protected abstract void DrawCrosshair(Graphics g);

        protected abstract void DrawHue();

        protected abstract void DrawSaturation();

        protected abstract void DrawBrightness();

        protected abstract void DrawRed();

        protected abstract void DrawGreen();

        protected abstract void DrawBlue();

        protected void GetPointColor(Point point)
        {
            lastPosition = point;

            if (this is ColorBox)
            {
                GetBoxColor();
            }
            else
            {
                GetSliderColor();
            }
        }

        protected Point GetPoint(Point point)
        {
            return new Point(point.X.Between(0, clientWidth - 1), point.Y.Between(0, clientHeight - 1));
        }

        protected int Round(double value)
        {
            var result = (int)value;

            if ((int)(value * 100) % 100 >= 50)
            {
                result++;
            }

            return result;
        }
    }
}