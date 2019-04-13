using System;

namespace CriPakTools
{
    /// <summary>
    /// </summary>
    public class FileEntry
    {
        /// <summary>
        /// </summary>
        public object DirectoryName { get; set; }

        /// <summary>
        /// </summary>
        public object FileName { get; set; }

        /// <summary>
        /// </summary>
        public object FileSize { get; set; }

        /// <summary>
        /// </summary>
        public long FileSizePosition { get; set; }

        /// <summary>
        /// </summary>
        public Type FileSizeType { get; set; }

        /// <summary>
        /// </summary>
        public object ExtractSize { get; set; }

        /// <summary>
        /// </summary>
        public long ExtractSizePosition { get; set; }

        /// <summary>
        /// </summary>
        public Type ExtractSizeType { get; set; }

        /// <summary>
        /// </summary>
        public ulong FileOffset { get; set; } = 0;

        /// <summary>
        /// </summary>
        public long FileOffsetPosition { get; set; }

        /// <summary>
        /// </summary>
        public Type FileOffsetType { get; set; }

        /// <summary>
        /// </summary>
        public ulong Offset { get; set; }

        /// <summary>
        /// </summary>
        public object Identifier { get; set; }

        /// <summary>
        /// </summary>
        public object UserString { get; set; }

        /// <summary>
        /// </summary>
        public object LocalDirectory { get; set; }

        /// <summary>
        /// </summary>
        public string TocName { get; set; }

        /// <summary>
        /// </summary>
        public bool Encrypted { get; set; }

        /// <summary>
        /// </summary>
        public string FileType { get; set; }
    }
}