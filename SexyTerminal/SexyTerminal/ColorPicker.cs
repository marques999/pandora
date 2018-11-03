using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SexyTerminal
{
    /// <summary>
    /// </summary>
    [DefaultEvent("ColorChanged")]
    public class ColorPicker : UserControl
    {
        /// <summary>
        /// </summary>
        private ColorBox _colorBox;

        /// <summary>
        /// </summary>
        private ColorSlider _colorSlider;

        /// <summary>
        /// </summary>
        private DrawStyle _drawStyle;

        /// <summary>
        /// </summary>
        private MyColor _userColor;

        /// <summary>
        /// </summary>
        private readonly IContainer components = null;

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public ColorPicker()
        {
            InitializeComponent();
            DrawStyle = DrawStyle.Hue;
            _colorBox.ColorChanged += ColorBox_ColorChanged;
            _colorSlider.ColorChanged += ColorSlider_ColorChanged;
        }

        /// <summary>
        /// </summary>
        public MyColor UserColor
        {
            get => _userColor;
            private set
            {
                if (_userColor == value)
                {
                    return;
                }

                _userColor = value;
                _colorBox.SelectedColor = _userColor;
                _colorSlider.SelectedColor = _userColor;
            }
        }

        /// <summary>
        /// </summary>
        public DrawStyle DrawStyle
        {
            get => _drawStyle;
            set
            {
                if (_drawStyle == value)
                {
                    return;
                }

                _drawStyle = value;
                _colorBox.DrawStyle = value;
                _colorSlider.DrawStyle = value;
            }
        }

        /// <summary>
        /// </summary>
        public bool CrosshairVisible
        {
            set
            {
                _colorBox.CrosshairVisible = value;
                _colorSlider.CrosshairVisible = value;
            }
        }

        /// <summary>
        /// </summary>
        public event ColorEventHandler ColorChanged;

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void ColorBox_ColorChanged(object sender, ColorEventArgs eventArgs)
        {
            _userColor = eventArgs.Color;
            _colorSlider.SelectedColor = UserColor;
            OnColorChanged();
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void ColorSlider_ColorChanged(object sender, ColorEventArgs eventArgs)
        {
            _userColor = eventArgs.Color;
            _colorBox.SelectedColor = UserColor;
            OnColorChanged();
        }

        /// <summary>
        /// </summary>
        /// <param name="color"></param>
        /// <param name="colorType"></param>
        public void ChangeColor(Color color, ColorType colorType = ColorType.None)
        {
            UserColor = color;
            OnColorChanged(colorType);
        }

        /// <summary>
        /// </summary>
        /// <param name="colorType"></param>
        private void OnColorChanged(ColorType colorType = ColorType.None)
        {
            ColorChanged?.Invoke(this, new ColorEventArgs(UserColor, colorType));
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// </summary>
        private void InitializeComponent()
        {
            SuspendLayout();

            _colorBox = new ColorBox
            {
                BorderStyle = BorderStyle.FixedSingle,
                DrawStyle = DrawStyle.Hue,
                Location = new Point(0, 0),
                Name = "_colorBox",
                Size = new Size(258, 258),
                TabIndex = 0
            };

            _colorSlider = new ColorSlider
            {
                BorderStyle = BorderStyle.FixedSingle,
                DrawStyle = DrawStyle.Hue,
                Location = new Point(257, 0),
                Name = "_colorSlider",
                Size = new Size(32, 258),
                TabIndex = 1
            };

            AutoSize = true;
            Controls.Add(_colorBox);
            Controls.Add(_colorSlider);
            Name = "ColorPicker";
            Size = new Size(292, 261);
            ResumeLayout(false);
        }
    }
}