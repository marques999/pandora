using System.Text;

namespace SexyTerminal.Formats
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    internal class Xshell : IExporter
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
            builder.Append("\n[Names]\n");
            builder.Append($"name0={scheme.Name}\n");
            builder.Append("count=1\n");
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="scheme"></param>
        public void WriteHeader(StringBuilder builder, Scheme scheme)
        {
            builder.Append($"[{scheme.Name}]\n");
            builder.Append($"text={ColorHelpers.ColorToHexLowercase(scheme.Foreground)}\n");
            builder.Append($"text(bold)={ColorHelpers.ColorToHexLowercase(scheme.Foreground)}\n");
            builder.Append($"background={ColorHelpers.ColorToHexLowercase(scheme.Background)}\n");
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
            builder.Append($"{Labels[colorIndex1]}={ColorHelpers.ColorToHexLowercase(scheme.Colors[colorIndex1])}\n");
            builder.Append($"{Labels[colorIndex1]}(bold)={ColorHelpers.ColorToHexLowercase(scheme.Colors[colorIndex2])}\n");
        }
    }
}