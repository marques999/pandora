using System.Text;

namespace SexyTerminal.Formats
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    internal class Konsole : IExporter
    {
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
            builder.Append("# --- general options ---\n\n");
            builder.Append("[General]\n");
            builder.Append($"Description={scheme.Name}\n");
            builder.Append("Opacity=1\n");
            builder.Append("Wallpaper=\n");
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="scheme"></param>
        public void WriteHeader(StringBuilder builder, Scheme scheme)
        {
            builder.Append("# --- special colors ---\n\n");
            builder.Append($"[Background]\nColor={ColorHelpers.ColorToRgb(scheme.Background)}\n\n");
            builder.Append($"[BackgroundIntense]\nColor={ColorHelpers.ColorToRgb(scheme.Background)}\n\n");
            builder.Append($"[Foreground]\nColor={ColorHelpers.ColorToRgb(scheme.Foreground)}\n\n");
            builder.Append($"[ForegroundIntense]\nColor={ColorHelpers.ColorToRgb(scheme.Foreground)}\nBold=true\n\n");
            builder.Append("# --- standard colors ---\n\n");
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
            builder.Append($"[Color{colorIndex1}]\nColor={ColorHelpers.ColorToRgb(scheme.Colors[colorIndex1])}\n\n");
            builder.Append($"[Color{colorIndex1}Intense]\nColor={ColorHelpers.ColorToRgb(scheme.Colors[colorIndex2])}\n\n");
        }
    }
}