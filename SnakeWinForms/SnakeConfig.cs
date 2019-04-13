using System.Drawing;

namespace SnakeWinForms
{
    /// <summary>
    /// </summary>
    internal class SnakeConfig
    {
        /// <summary>
        /// </summary>
        public const int CellSize = 32;

        /// <summary>
        /// </summary>
        public const int ColumnCount = 16;

        /// <summary>
        /// </summary>
        public const int RowCount = 16;

        /// <summary>
        /// </summary>
        public const float RowPercent = 100.0F / RowCount;

        /// <summary>
        /// </summary>
        public const float ColumnPercent = 100.0F / ColumnCount;

        /// <summary>
        /// </summary>
        public static readonly Color SnakeHeadColor = Color.ForestGreen;

        /// <summary>
        /// </summary>
        public static readonly Color SnakeNormalColor = Color.LawnGreen;

        /// <summary>
        /// </summary>
        public static readonly Color AppleColor = Color.DarkRed;

        /// <summary>
        /// </summary>
        public static readonly Color BlankColor = Color.Black;
    }
}