using System;
using System.Collections.Generic;
using System.Xml;

namespace SmoothDownloader.Smooth
{
    /// <summary>
    /// </summary>
    internal class ChunkInfo
    {
        /// <summary>
        /// </summary>
        public IDictionary<string, string> Attributes;

        /// <summary>
        /// </summary>
        public ulong Duration;

        /// <summary>
        /// </summary>
        public uint Index;

        /// <summary>
        /// </summary>
        public ulong StartTime;

        /// <summary>
        /// </summary>
        /// <param name="element"></param>
        /// <param name="index"></param>
        /// <param name="starttime"></param>
        public ChunkInfo(XmlNode element, uint index, ulong starttime)
        {
            if (element.Name != "c")
            {
                throw new Exception();
            }

            Index = index;
            Attributes = Parser.Attributes(element);

            if (Attributes.ContainsKey("n"))
            {
                Index = Parser.UInt32Attribute(Attributes, "n");
            }

            if (Index != index)
            {
                throw new Exception();
            }

            StartTime = starttime;

            if (Attributes.ContainsKey("t"))
            {
                StartTime = Parser.UInt64Attribute(Attributes, "t");
            }

            if (Attributes.ContainsKey("d"))
            {
                Duration = Parser.UInt64Attribute(Attributes, "d");
            }
        }
    }
}