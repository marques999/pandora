using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace SexyTerminal
{
    [DefaultEvent("ColorChanged")]
    public class ColorButton : Button
    {
        public delegate void ColorChangedEventHandler(Color color);
        public event ColorChangedEventHandler ColorChanged;

        private Color _color;

        public Color Color
        {
            get => _color;
            set
            {
                _color = value;
                Refresh();
                OnColorChanged(_color);
            }
        }

        [DefaultValue(typeof(Color), "DarkGray")]
        public Color BorderColor { get; set; } = Color.DarkGray;

        [DefaultValue(3)]
        public int Offset { get; set; } = 3;

        [DefaultValue(false)]
        public bool HoverEffect { get; set; } = false;

        [DefaultValue(false)]
        public bool ManualButtonClick { get; set; }

        private bool _isMouseHover;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void OnColorChanged(Color color)
        {
            ColorChanged?.Invoke(color);
        }

        protected override void OnMouseClick(MouseEventArgs mevent)
        {
            base.OnMouseClick(mevent);

            if (!ManualButtonClick)
            {
                ShowColorDialog();
            }
        }

        public void ShowColorDialog()
        {
            if (ColorPickerForm.PickColor(Color, out var newColor, FindForm()))
            {
                Color = newColor;
            }
        }

        protected override void OnMouseEnter(EventArgs eventArgs)
        {
            _isMouseHover = true;
            base.OnMouseEnter(eventArgs);
        }

        protected override void OnMouseLeave(EventArgs eventArgs)
        {
            _isMouseHover = false;
            base.OnMouseLeave(eventArgs);
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            if (Offset > 0)
            {
                base.OnPaint(pevent);
            }

            var boxSize = ClientRectangle.Height - (Offset * 2);
            var boxRectangle = new Rectangle(ClientRectangle.Width - Offset - boxSize, Offset, boxSize, boxSize);

            var graphics = pevent.Graphics;

            if (Color.A < 255)
            {
                using (Image checker = ImageHelpers.CreateCheckerPattern(boxSize, boxSize))
                {
                    graphics.DrawImage(checker, boxRectangle);
                }
            }

            if (Color.A > 0)
            {
                using (Brush brush = new SolidBrush(Color))
                {
                    graphics.FillRectangle(brush, boxRectangle);
                }
            }

            if (HoverEffect && _isMouseHover)
            {
                using (Brush hoverBrush = new SolidBrush(Color.FromArgb(100, 255, 255, 255)))
                {
                    graphics.FillRectangle(hoverBrush, boxRectangle);
                }
            }

            using (var borderPen = new Pen(BorderColor))
            {
                graphics.DrawRectangleProper(borderPen, boxRectangle);
            }
        }
    }
}