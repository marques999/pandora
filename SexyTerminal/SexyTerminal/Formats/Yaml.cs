using System.Text;

namespace SexyTerminal.Formats
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    internal class Yaml : IExporter
    {
        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="scheme"></param>
        public void WriteHeader(StringBuilder builder, Scheme scheme)
        {
            builder.Append($"id: {scheme.Name}\n");
            builder.Append($"foreground: '{ColorHelpers.ColorToHex(scheme.Foreground)}'\n");
            builder.Append($"background: '{ColorHelpers.ColorToHex(scheme.Background)}'\n");
            builder.Append("colors:\n");
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
            builder.Append($"  - '{ColorHelpers.ColorToHex(scheme.Colors[colorIndex1])}'\n");
        }

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
    }
}