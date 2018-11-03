using System;

namespace SexyTerminal
{
    /// <summary>
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="arguments"></param>
    public delegate void ColorEventHandler(object sender, ColorEventArgs arguments);

    /// <inheritdoc />
    /// <summary>
    /// </summary>
    public class ColorEventArgs : EventArgs
    {
        /// <summary>
        /// </summary>
        public MyColor Color;

        /// <summary>
        /// </summary>
        public ColorType ColorType;

        /// <summary>
        /// </summary>
        public DrawStyle DrawStyle;

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="color"></param>
        /// <param name="colorType"></param>
        public ColorEventArgs(MyColor color, ColorType colorType)
        {
            Color = color;
            ColorType = colorType;
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="color"></param>
        /// <param name="drawStyle"></param>
        public ColorEventArgs(MyColor color, DrawStyle drawStyle)
        {
            Color = color;
            DrawStyle = drawStyle;
        }
    }
}