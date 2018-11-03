using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SexyTerminal
{
    /// <summary>
    /// </summary>
    public static partial class NumberExtensions
    {
        /// <summary>
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hWnd);

        /// <summary>
        /// </summary>
        /// <param name="lpNativePoint"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetCursorPos(out NativePoint lpNativePoint);

        public static void SetValue(this NumericUpDown nud, decimal number)
        {
            nud.Value = number.Between(nud.Minimum, nud.Maximum);
        }

        public static Point GetCursorPosition()
        {
            if (GetCursorPos(out var point)) return (Point) point;

            return Point.Empty;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color GetPixelColor()
        {
            return GetPixelColor(GetCursorPosition());
        }

        [DllImport("user32.dll")]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll")]
        public static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);

        public static Color GetPixelColor(int x, int y)
        {
            var hdc = GetDC(IntPtr.Zero);
            var pixel = GetPixel(hdc, x, y);
            ReleaseDC(IntPtr.Zero, hdc);
            return Color.FromArgb((int) (pixel & 0x000000FF), (int) (pixel & 0x0000FF00) >> 8,
                (int) (pixel & 0x00FF0000) >> 16);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color GetPixelColor(Point position)
        {
            return GetPixelColor(position.X, position.Y);
        }

        public static bool CheckPixelColor(int x, int y, Color color)
        {
            var targetColor = GetPixelColor(x, y);

            return targetColor.R == color.R && targetColor.G == color.G && targetColor.B == color.B;
        }

        public static bool CheckPixelColor(int x, int y, Color color, byte variation)
        {
            var targetColor = GetPixelColor(x, y);

            return targetColor.R.IsBetween(color.R - variation, color.R + variation) &&
                   targetColor.G.IsBetween(color.G - variation, color.G + variation) &&
                   targetColor.B.IsBetween(color.B - variation, color.B + variation);
        }


        public static void ForceActivate(this Form form)
        {
            if (form.Visible == false)
            {
                form.Show();
            }

            if (form.WindowState == FormWindowState.Minimized)
            {
                form.WindowState = FormWindowState.Normal;
            }

            form.BringToFront();
            form.Activate();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Min(this int num, int min)
        {
            return num < min ? min : num;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Max(this int num, int max)
        {
            return num > max ? max : num;
        }

        public static int Between(this int num, int min, int max)
        {
            return num <= min ? min : (num >= max ? max : num);
        }

        public static float Between(this float num, float min, float max)
        {
            return num <= min ? min : (num >= max ? max : num);
        }

        public static double Between(this double num, double min, double max)
        {
            return num <= min ? min : (num >= max ? max : num);
        }

        public static byte Between(this byte num, byte min, byte max)
        {
            return num <= min ? min : (num >= max ? max : num);
        }

        public static decimal Between(this decimal num, decimal min, decimal max)
        {
            return num <= min ? min : (num >= max ? max : num);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBetween(this int num, int min, int max)
        {
            return num >= min && num <= max;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBetween(this byte num, int min, int max)
        {
            return num >= min && num <= max;
        }

        public static int BetweenOrDefault(this int num, int min, int max, int defaultValue = 0)
        {
            return num.IsBetween(min, max) ? num : defaultValue;
        }

        public static float Remap(this float value, float from1, float to1, float from2, float to2)
        {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }

        public static string ToDecimalString(this double number, int decimalPlaces)
        {
            var format = "0";
            if (decimalPlaces > 0) format += "." + new string('0', decimalPlaces);
            return number.ToString(format);
        }

        public static string ToBase(this int value, int radix, string digits)
        {
            if (string.IsNullOrEmpty(digits))
            {
                throw new ArgumentNullException(nameof(digits));
            }

            radix = Math.Abs(radix);

            if (radix > digits.Length || radix < 2)
            {
                throw new ArgumentOutOfRangeException(nameof(radix));
            }

            var result = "";
            var quotient = Math.Abs(value);

            while (quotient > 0)
            {
                var temp = quotient % radix;
                result = digits[temp] + result;
                quotient /= radix;
            }

            return result;
        }
    }
}