using System.Drawing;
using System.Runtime.InteropServices;

namespace SexyTerminal
{
    /// <summary>
    /// </summary>
    public static partial class NumberExtensions
    {
        /// <summary>
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct NativePoint
        {
            /// <summary>
            /// </summary>
            public int X;

            /// <summary>
            /// </summary>
            public int Y;

            /// <summary>
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            public NativePoint(int x, int y)
            {
                X = x;
                Y = y;
            }

            /// <summary>
            /// </summary>
            /// <param name="point"></param>
            public static explicit operator Point(NativePoint point)
            {
                return new Point(point.X, point.Y);
            }

            /// <summary>
            /// </summary>
            /// <param name="point"></param>
            public static explicit operator NativePoint(Point point)
            {
                return new NativePoint(point.X, point.Y);
            }
        }
    }
}