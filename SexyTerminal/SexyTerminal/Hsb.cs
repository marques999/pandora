using System.Drawing;

namespace SexyTerminal
{
    /// <summary>
    /// </summary>
    public struct Hsb
    {
        public bool Equals(Hsb other)
        {
            return _hue.Equals(other._hue) && _saturation.Equals(other._saturation) && _brightness.Equals(other._brightness) && _alpha == other._alpha;
        }

        public override bool Equals(object other)
        {
            return ReferenceEquals(null, other) == false && other is Hsb hsb && Equals(hsb);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = _hue.GetHashCode();
                hashCode = (hashCode * 397) ^ _saturation.GetHashCode();
                hashCode = (hashCode * 397) ^ _brightness.GetHashCode();
                hashCode = (hashCode * 397) ^ _alpha;
                return hashCode;
            }
        }

        /// <summary>
        /// </summary>
        private double _hue;

        /// <summary>
        /// </summary>
        private double _saturation;

        /// <summary>
        /// </summary>
        private double _brightness;

        /// <summary>
        /// </summary>
        private int _alpha;

        /// <summary>
        /// </summary>
        public double Hue
        {
            get => _hue;
            set => _hue = ColorHelpers.ValidColor(value);
        }

        /// <summary>
        /// </summary>
        public double Hue360
        {
            get => _hue * 360;
            set => _hue = ColorHelpers.ValidColor(value / 360);
        }

        public double Saturation
        {
            get => _saturation;
            set => _saturation = ColorHelpers.ValidColor(value);
        }

        public double Saturation100
        {
            get => _saturation * 100;
            set => _saturation = ColorHelpers.ValidColor(value / 100);
        }

        public double Brightness
        {
            get => _brightness;
            set => _brightness = ColorHelpers.ValidColor(value);
        }

        public double Brightness100
        {
            get => _brightness * 100;
            set => _brightness = ColorHelpers.ValidColor(value / 100);
        }

        public int Alpha
        {
            get => _alpha;
            set => _alpha = ColorHelpers.ValidColor(value);
        }

        public Hsb(double hue, double saturation, double brightness, int alpha = 255) : this()
        {
            Hue = hue;
            Saturation = saturation;
            Brightness = brightness;
            Alpha = alpha;
        }

        public Hsb(int hue, int saturation, int brightness, int alpha = 255) : this()
        {
            Hue360 = hue;
            Saturation100 = saturation;
            Brightness100 = brightness;
            Alpha = alpha;
        }

        public Hsb(Color color)
        {
            this = ColorHelpers.ColorToHsb(color);
        }

        public static implicit operator Hsb(Color color)
        {
            return ColorHelpers.ColorToHsb(color);
        }

        public static implicit operator Color(Hsb color)
        {
            return color.ToColor();
        }

        public static implicit operator Rgba(Hsb color)
        {
            return color.ToColor();
        }

        public static implicit operator Cmyk(Hsb color)
        {
            return color.ToColor();
        }

        public static bool operator ==(Hsb left, Hsb right)
        {
            return left.Hue == right.Hue && left.Saturation == right.Saturation && left.Brightness == right.Brightness;
        }

        public static bool operator !=(Hsb left, Hsb right)
        {
            return !(left == right);
        }

        public Color ToColor()
        {
            return ColorHelpers.HsbToColor(this);
        }
    }
}