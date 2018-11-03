using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace SexyTerminal
{
    public static class ImageHelpers
    {
        public static Image CreateCheckerPattern(int width, int height)
        {
            return CreateCheckerPattern(width, height, SystemColors.ControlLight, SystemColors.ControlLightLight);
        }

        private static Image CreateCheckerPattern(int width, int height, Color color1, Color color2)
        {
            using (var graphics = Graphics.FromImage(new Bitmap(width * 2, height * 2)))
            using (var foregound = new SolidBrush(color1))
            using (var background = new SolidBrush(color2))
            {
                graphics.FillRectangle(foregound, 0, 0, width, height);
                graphics.FillRectangle(foregound, width, height, width, height);
                graphics.FillRectangle(background, width, 0, width, height);
                graphics.FillRectangle(background, 0, height, width, height);
            }

            return new Bitmap(width * 2, height * 2);
        }
    }

    public static class ColorHelpers
    {
        #region Convert HSB to ...

        public static Color HsbToColor(Hsb hsb)
        {
            int mid;
            var max = (int) Math.Round(hsb.Brightness * 255);
            var min = (int) Math.Round((1.0 - hsb.Saturation) * (hsb.Brightness / 1.0) * 255);
            var q = (double) (max - min) / 255;

            if (hsb.Hue >= 0 && hsb.Hue <= (double) 1 / 6)
            {
                mid = (int) Math.Round((hsb.Hue - 0) * q * 1530 + min);
                return Color.FromArgb(hsb.Alpha, max, mid, min);
            }

            if (hsb.Hue <= (double) 1 / 3)
                return Color.FromArgb(hsb.Alpha, (int) Math.Round(-((hsb.Hue - (double) 1 / 6) * q) * 1530 + max), max,
                    min);

            if (hsb.Hue <= 0.5)
            {
                mid = (int) Math.Round((hsb.Hue - (double) 1 / 3) * q * 1530 + min);
                return Color.FromArgb(hsb.Alpha, min, max, mid);
            }

            if (hsb.Hue <= (double) 2 / 3)
            {
                mid = (int) Math.Round(-((hsb.Hue - 0.5) * q) * 1530 + max);
                return Color.FromArgb(hsb.Alpha, min, mid, max);
            }

            if (hsb.Hue <= (double) 5 / 6)
            {
                mid = (int) Math.Round((hsb.Hue - (double) 2 / 3) * q * 1530 + min);
                return Color.FromArgb(hsb.Alpha, mid, min, max);
            }

            if (hsb.Hue <= 1.0)
            {
                mid = (int) Math.Round(-((hsb.Hue - (double) 5 / 6) * q) * 1530 + max);
                return Color.FromArgb(hsb.Alpha, max, min, mid);
            }

            return Color.FromArgb(hsb.Alpha, 0, 0, 0);
        }

        #endregion Convert HSB to ...

        #region Convert CMYK to ...

        public static Color CmykToColor(Cmyk cmyk)
        {
            if (cmyk.Cyan == 0 && cmyk.Magenta == 0 && cmyk.Yellow == 0 && cmyk.Key == 1)
                return Color.FromArgb(cmyk.Alpha, 0, 0, 0);

            var c = cmyk.Cyan * (1 - cmyk.Key) + cmyk.Key;
            var m = cmyk.Magenta * (1 - cmyk.Key) + cmyk.Key;
            var y = cmyk.Yellow * (1 - cmyk.Key) + cmyk.Key;

            var r = (int) Math.Round((1 - c) * 255);
            var g = (int) Math.Round((1 - m) * 255);
            var b = (int) Math.Round((1 - y) * 255);

            return Color.FromArgb(cmyk.Alpha, r, g, b);
        }

        #endregion Convert CMYK to ...

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ValidColor(double number)
        {
            return number.Between(0, 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ValidColor(int number)
        {
            return number.Between(0, 255);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ValidColor(byte number)
        {
            return number.Between(0, 255);
        }

        public static bool ParseColor(string text, out Color color)
        {
            if (!string.IsNullOrEmpty(text))
            {
                text = text.Trim();

                if (text.Length <= 20)
                {
                    var matchHex = Regex.Match(text, @"^(?:#|0x)?((?:[0-9A-F]{2}){3})$", RegexOptions.IgnoreCase);

                    if (matchHex.Success)
                    {
                        color = HexToColor(matchHex.Groups[1].Value);
                        return true;
                    }

                    var matchRgb = Regex.Match(text,
                        @"^(?:rgb\()?([1]?[0-9]?[0-9]|2[0-4][0-9]|25[0-5])(?:\s|,)+([1]?[0-9]?[0-9]|2[0-4][0-9]|25[0-5])(?:\s|,)+([1]?[0-9]?[0-9]|2[0-4][0-9]|25[0-5])\)?$");

                    if (matchRgb.Success)
                    {
                        color = Color.FromArgb(int.Parse(matchRgb.Groups[1].Value), int.Parse(matchRgb.Groups[2].Value),
                            int.Parse(matchRgb.Groups[3].Value));
                        return true;
                    }
                }
            }

            color = Color.Empty;
            return false;
        }

        public static int PerceivedBrightness(Color color)
        {
            return (int) Math.Sqrt(color.R * color.R * .299 + color.G * color.G * .587 + color.B * color.B * .114);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color VisibleColor(Color color)
        {
            return VisibleColor(color, Color.White, Color.Black);
        }

        public static Color VisibleColor(Color color, Color lightColor, Color darkColor)
        {
            return PerceivedBrightness(color) > 130 ? darkColor : lightColor;
        }

        public static Color Lerp(Color from, Color to, float amount)
        {
            return Color.FromArgb((int) Lerp(from.R, to.R, amount), (int) Lerp(from.G, to.G, amount),
                (int) Lerp(from.B, to.B, amount));
        }

        public static float Lerp(float value1, float value2, float amount)
        {
            return value1 + (value2 - value1) * amount;
        }

        public static Color DeterministicStringToColor(string text)
        {
            var hash = text.GetHashCode();
            var r = (hash & 0xFF0000) >> 16;
            var g = (hash & 0x00FF00) >> 8;
            var b = hash & 0x0000FF;
            return Color.FromArgb(r, g, b);
        }

        public static int ColorDifference(Color color1, Color color2)
        {
            return Math.Abs(color1.R - color2.R) + Math.Abs(color1.G - color2.G) + Math.Abs(color1.B - color2.B);
        }

        /// <summary>
        /// </summary>
        /// <param name="color1"></param>
        /// <param name="color2"></param>
        /// <param name="threshold"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ColorsAreClose(Color color1, Color color2, int threshold)
        {
            return ColorDifference(color1, color2) <= threshold;
        }

        /// <summary>
        /// </summary>
        /// <param name="color"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color LighterColor(Color color, float amount)
        {
            return Lerp(color, Color.White, amount);
        }

        /// <summary>
        /// </summary>
        /// <param name="color"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color DarkerColor(Color color, float amount)
        {
            return Lerp(color, Color.Black, amount);
        }

        #region Convert Color to ...

        public static string ColorToRgb(Color color)
        {
            return $"{color.R:D},{color.G:D},{color.B:D}";
        }

        public static string ColorToHex(Color color, ColorFormat format = ColorFormat.RGB)
        {
            switch (format)
            {
                default:
                case ColorFormat.RGB:
                    return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
                case ColorFormat.RGBA:
                    return $"#{color.R:X2}{color.G:X2}{color.B:X2}{color.A:X2}";
                case ColorFormat.ARGB:
                    return $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";
            }
        }

        public static string ColorToHexLowercase(Color color)
        {
            return $"{color.R:x2}{color.G:x2}{color.B:x2}";
        }

        public static string ColorToHexDouble(Color color)
        {
            return $"#{color.R:x2}{color.R:x2}{color.G:x2}{color.G:x2}{color.B:x2}{color.B:x2}";
        }

        public static int ColorToDecimal(Color color, ColorFormat format = ColorFormat.RGB)
        {
            switch (format)
            {
                default:
                case ColorFormat.RGB:
                    return (color.R << 16) | (color.G << 8) | color.B;
                case ColorFormat.RGBA:
                    return (color.R << 24) | (color.G << 16) | (color.B << 8) | color.A;
                case ColorFormat.ARGB:
                    return (color.A << 24) | (color.R << 16) | (color.G << 8) | color.B;
            }
        }

        public static Hsb ColorToHsb(Color color)
        {
            var hsb = new Hsb();

            int max, min;

            if (color.R > color.G)
            {
                max = color.R;
                min = color.G;
            }
            else
            {
                max = color.G;
                min = color.R;
            }

            if (color.B > max) max = color.B;
            else if (color.B < min) min = color.B;

            var diff = max - min;

            hsb.Brightness = (double) max / 255;

            if (max == 0) hsb.Saturation = 0;
            else hsb.Saturation = (double) diff / max;

            double q;
            if (diff == 0) q = 0;
            else q = (double) 60 / diff;

            if (max == color.R)
            {
                if (color.G < color.B) hsb.Hue = (360 + q * (color.G - color.B)) / 360;
                else hsb.Hue = q * (color.G - color.B) / 360;
            }
            else if (max == color.G)
            {
                hsb.Hue = (120 + q * (color.B - color.R)) / 360;
            }
            else if (max == color.B)
            {
                hsb.Hue = (240 + q * (color.R - color.G)) / 360;
            }
            else
            {
                hsb.Hue = 0.0;
            }

            hsb.Alpha = color.A;

            return hsb;
        }

        public static Cmyk ColorToCmyk(Color color)
        {
            if (color.R == 0 && color.G == 0 && color.B == 0) return new Cmyk(0, 0, 0, 1, color.A);

            var c = 1 - color.R / 255d;
            var m = 1 - color.G / 255d;
            var y = 1 - color.B / 255d;
            var k = Math.Min(c, Math.Min(m, y));

            c = (c - k) / (1 - k);
            m = (m - k) / (1 - k);
            y = (y - k) / (1 - k);

            return new Cmyk(c, m, y, k, color.A);
        }

        #endregion Convert Color to ...

        #region Convert Hex to ...

        public static Color HexToColor(string hex, ColorFormat format = ColorFormat.RGB)
        {
            if (string.IsNullOrEmpty(hex)) return Color.Empty;

            if (hex[0] == '#')
                hex = hex.Remove(0, 1);
            else if (hex.StartsWith("0x", StringComparison.InvariantCultureIgnoreCase)) hex = hex.Remove(0, 2);

            if ((format == ColorFormat.RGBA || format == ColorFormat.ARGB) && hex.Length != 8 ||
                format == ColorFormat.RGB && hex.Length != 6)
                return Color.Empty;

            int r, g, b, a;

            switch (format)
            {
                default:
                case ColorFormat.RGB:
                    r = HexToDecimal(hex.Substring(0, 2));
                    g = HexToDecimal(hex.Substring(2, 2));
                    b = HexToDecimal(hex.Substring(4, 2));
                    a = 255;
                    break;
                case ColorFormat.RGBA:
                    r = HexToDecimal(hex.Substring(0, 2));
                    g = HexToDecimal(hex.Substring(2, 2));
                    b = HexToDecimal(hex.Substring(4, 2));
                    a = HexToDecimal(hex.Substring(6, 2));
                    break;
                case ColorFormat.ARGB:
                    a = HexToDecimal(hex.Substring(0, 2));
                    r = HexToDecimal(hex.Substring(2, 2));
                    g = HexToDecimal(hex.Substring(4, 2));
                    b = HexToDecimal(hex.Substring(6, 2));
                    break;
            }

            return Color.FromArgb(a, r, g, b);
        }

        public static int HexToDecimal(string hex)
        {
            return Convert.ToInt32(hex, 16);
        }

        #endregion Convert Hex to ...

        #region Convert Decimal to ...

        public static Color DecimalToColor(int dec, ColorFormat format = ColorFormat.RGB)
        {
            switch (format)
            {
                default:
                case ColorFormat.RGB:
                    return Color.FromArgb((dec >> 16) & 0xFF, (dec >> 8) & 0xFF, dec & 0xFF);
                case ColorFormat.RGBA:
                    return Color.FromArgb(dec & 0xFF, (dec >> 24) & 0xFF, (dec >> 16) & 0xFF, (dec >> 8) & 0xFF);
                case ColorFormat.ARGB:
                    return Color.FromArgb((dec >> 24) & 0xFF, (dec >> 16) & 0xFF, (dec >> 8) & 0xFF, dec & 0xFF);
            }
        }

        public static string DecimalToHex(int dec)
        {
            return dec.ToString("X6");
        }

        #endregion Convert Decimal to ...
    }
}