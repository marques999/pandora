using System.Text;

namespace SexyTerminal.Formats
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    internal class Xresources : IExporter
    {
        /// <summary>
        /// </summary>
        private static readonly string[] Labels =
        {
            "black",
            "red",
            "green",
            "yellow",
            "blue",
            "magenta",
            "cyan",
            "white"
        };

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public bool Group { get; } = true;

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
            builder.Append("! special\n");
            builder.Append($"*.foreground: {ColorHelpers.ColorToHex(scheme.Foreground)}\n");
            builder.Append($"*.background: {ColorHelpers.ColorToHex(scheme.Background)}\n");
            builder.Append($"*.cursorColor: {ColorHelpers.ColorToHex(scheme.Foreground)}\n\n");
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
            builder.Append($"! {Labels[colorIndex1]}\n");
            builder.Append($"*.color{colorIndex1} {ColorHelpers.ColorToHex(scheme.Colors[colorIndex1])}\n");
            builder.Append($"*.color{colorIndex2} {ColorHelpers.ColorToHex(scheme.Colors[colorIndex2])}\n\n");
        }
    }
}