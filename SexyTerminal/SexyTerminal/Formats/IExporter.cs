using System.Text;

namespace SexyTerminal.Formats
{
    /// <summary>
    /// </summary>
    internal interface IExporter
    {
        /// <summary>
        /// </summary>
        bool Group { get; }

        /// <summary>
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="scheme"></param>
        void WriteEpilogue(StringBuilder builder, Scheme scheme);

        /// <summary>
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="scheme"></param>
        void WriteHeader(StringBuilder builder, Scheme scheme);

        /// <summary>
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="scheme"></param>
        /// <param name="colorIndex1"></param>
        /// <param name="colorIndex2"></param>
        void WriteColor(StringBuilder builder, Scheme scheme, int colorIndex1, int colorIndex2);
    }
}