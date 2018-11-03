using System.Drawing;

namespace SexyTerminal
{
    /// <summary>
    /// </summary>
    public struct MyColor
    {
        public bool Equals(MyColor other)
        {
            return Rgba.Equals(other.Rgba);
        }

        public override bool Equals(object other)
        {
            return ReferenceEquals(null, other) == false && other is MyColor color && Equals(color);
        }

        public override int GetHashCode()
        {
            return Rgba.GetHashCode();
        }

        /// <summary>
        /// </summary>
        public Rgba Rgba;

        /// <summary>
        /// </summary>
        public Hsb Hsb;

        /// <summary>
        /// </summary>
        public Cmyk Cmyk;

        /// <summary>
        /// </summary>
        public bool IsTransparent => Rgba.Alpha < 255;

        /// <summary>
        /// </summary>
        /// <param name="color"></param>
        public MyColor(Color color)
        {
            Rgba = color;
            Hsb = color;
            Cmyk = color;
        }

        /// <summary>
        /// </summary>
        /// <param name="color"></param>
        public static implicit operator MyColor(Color color)
        {
            return new MyColor(color);
        }

        /// <summary>
        /// </summary>
        /// <param name="color"></param>
        public static implicit operator Color(MyColor color)
        {
            return color.Rgba;
        }

        /// <summary>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(MyColor left, MyColor right)
        {
            return left.Rgba == right.Rgba && left.Hsb == right.Hsb && left.Cmyk == right.Cmyk;
        }

        /// <summary>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(MyColor left, MyColor right)
        {
            return !(left == right);
        }

        /// <summary>
        /// </summary>
        public void RgbaUpdate()
        {
            Hsb = Rgba;
            Cmyk = Rgba;
        }

        /// <summary>
        /// </summary>
        public void HsbUpdate()
        {
            Rgba = Hsb;
            Cmyk = Hsb;
        }

        /// <summary>
        /// </summary>
        public void CmykUpdate()
        {
            Rgba = Cmyk;
            Hsb = Cmyk;
        }
    }
}