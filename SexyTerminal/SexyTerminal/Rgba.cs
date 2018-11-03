using System.Drawing;
using System.Runtime.CompilerServices;

namespace SexyTerminal
{
    /// <summary>
    /// </summary>
    public struct Rgba
    {
        private int _red, _green, _blue, _alpha;

        /// <summary>
        /// </summary>
        public int Red
        {
            get => _red;
            set => _red = ColorHelpers.ValidColor(value);
        }

        /// <summary>
        /// </summary>
        public int Green
        {
            get => _green;
            set => _green = ColorHelpers.ValidColor(value);
        }

        /// <summary>
        /// </summary>
        public int Blue
        {
            get => _blue;
            set => _blue = ColorHelpers.ValidColor(value);
        }

        /// <summary>
        /// </summary>
        public int Alpha
        {
            get => _alpha;
            set => _alpha = ColorHelpers.ValidColor(value);
        }

        /// <summary>
        /// </summary>
        public Rgba(int red, int green, int blue, int alpha = 255) : this()
        {
            Red = red;
            Green = green;
            Blue = blue;
            Alpha = alpha;
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public Rgba(Color color) : this(color.R, color.G, color.B, color.A)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="color"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Rgba(Color color)
        {
            return new Rgba(color);
        }

        /// <summary>
        /// </summary>
        /// <param name="color"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Color(Rgba color)
        {
            return color.ToColor();
        }

        /// <summary>
        /// </summary>
        /// <param name="color"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Hsb(Rgba color)
        {
            return color.ToHsb();
        }

        /// <summary>
        /// </summary>
        /// <param name="color"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Cmyk(Rgba color)
        {
            return color.ToCmyk();
        }

        public static bool operator ==(Rgba left, Rgba right)
        {
            return left.Red == right.Red && left.Green == right.Green && left.Blue == right.Blue && left.Alpha == right.Alpha;
        }

        /// <summary>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Rgba left, Rgba right)
        {
            return !(left == right);
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"R: {Red}, G: {Green}, B: {Blue}, A: {Alpha}";
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Color ToColor()
        {
            return Color.FromArgb(Alpha, Red, Green, Blue);
        }

        /// <summary>
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ToHex(ColorFormat format = ColorFormat.RGB)
        {
            return ColorHelpers.ColorToHex(this, format);
        }

        /// <summary>
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ToDecimal(ColorFormat format = ColorFormat.RGB)
        {
            return ColorHelpers.ColorToDecimal(this, format);
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Hsb ToHsb()
        {
            return ColorHelpers.ColorToHsb(this);
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Cmyk ToCmyk()
        {
            return ColorHelpers.ColorToCmyk(this);
        }
    }
}