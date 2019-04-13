using System;
using System.Collections.Generic;
using System.Xml;

using SmoothDownloader.Mkv;

namespace SmoothDownloader.Smooth
{
    /// <summary>
    /// </summary>
    internal class TrackInfo
    {
        /// <summary>
        /// </summary>
        public IDictionary<string, string> Attributes;

        /// <summary>
        /// </summary>
        public uint Bitrate;

        /// <summary>
        /// </summary>
        public IDictionary<string, string> CustomAttributes;

        /// <summary>
        /// </summary>
        public string Description;

        /// <summary>
        /// </summary>
        public uint Position;

        /// <summary>
        /// </summary>
        public StreamInfo Stream;

        /// <summary>
        /// </summary>
        public TrackEntry TrackEntry;

        /// <summary>
        /// </summary>
        /// <param name="element"></param>
        /// <param name="position"></param>
        /// <param name="stream"></param>
        public TrackInfo(XmlNode element, uint position, StreamInfo stream)
        {
            if (element.Name != "QualityLevel")
            {
                throw new Exception();
            }

            Position = position;
            Attributes = Parser.Attributes(element);
            CustomAttributes = Parser.CustomAttributes(element);

            if (Attributes.ContainsKey("Index"))
            {
                Position = Parser.UInt32Attribute(Attributes, "Index");
            }

            if (Position != position)
            {
                throw new Exception();
            }

            Stream = stream;
            Bitrate = Parser.UInt32Attribute(Attributes, "Bitrate");
        }
    }
}