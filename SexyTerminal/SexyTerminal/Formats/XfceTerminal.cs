using System.Text;

namespace SexyTerminal.Formats
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    internal class XfceTerminal : IExporter
    {
        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public bool Group { get; } = false;

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="scheme"></param>
        public void WriteEpilogue(StringBuilder builder, Scheme scheme)
        {
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="scheme"></param>
        public void WriteHeader(StringBuilder builder, Scheme scheme)
        {
            builder.Append("[Configuration]\n");
            builder.Append($"ColorCursor={ColorHelpers.ColorToHexDouble(scheme.Foreground)}\n");
            builder.Append($"ColorForeground={ColorHelpers.ColorToHexDouble(scheme.Foreground)}\n");
            builder.Append($"ColorBackground={ColorHelpers.ColorToHexDouble(scheme.Background)}\n");
            builder.Append("ColorPalette=");
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="scheme"></param>
        /// <param name="colorIndex1"></param>
        /// <param name="colorIndex2"></param>
        public void WriteColor(StringBuilder builder, Scheme scheme, int colorIndex1, int colorIndex2)
        {
            builder.Append($"{ColorHelpers.ColorToHexDouble(scheme.Colors[colorIndex1])};");
        }
    }
}