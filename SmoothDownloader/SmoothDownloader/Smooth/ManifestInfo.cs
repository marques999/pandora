using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace SmoothDownloader.Smooth
{
    /// <summary>
    /// </summary>
    internal class ManifestInfo
    {
        /// <summary>
        /// </summary>
        public IDictionary<string, string> Attributes;

        /// <summary>
        /// </summary>
        public IList<StreamInfo> AvailableStreams = new List<StreamInfo>();

        /// <summary>
        /// </summary>
        public ulong Duration;

        /// <summary>
        /// </summary>
        public bool IsLive;

        /// <summary>
        /// </summary>
        public uint MajorVersion;

        /// <summary>
        /// </summary>
        public uint MinorVersion;

        /// <summary>
        /// </summary>
        public IList<StreamInfo> SelectedStreams = new List<StreamInfo>();

        /// <summary>
        /// </summary>
        public ulong TimeScale;

        /// <summary>
        /// </summary>
        public ulong TotalTicks;

        /// <summary>
        /// </summary>
        public Uri Uri;

        /// <summary>
        /// </summary>
        /// <param name="element"></param>
        /// <param name="uri"></param>
        private ManifestInfo(XmlNode element, Uri uri)
        {
            if (element.Name != "SmoothStreamingMedia")
            {
                throw new Exception();
            }

            Uri = uri;
            TimeScale = 10000000uL;
            Attributes = Parser.Attributes(element);
            Duration = Parser.UInt64Attribute(Attributes, "Duration");
            MajorVersion = Parser.UInt32Attribute(Attributes, "MajorVersion");
            MinorVersion = Parser.UInt32Attribute(Attributes, "MinorVersion");

            if (Attributes.ContainsKey("IsLive"))
            {
                IsLive = Parser.BoolAttribute(Attributes, "IsLive");
            }

            if (Attributes.ContainsKey("TimeScale"))
            {
                TimeScale = Parser.UInt64Attribute(Attributes, "TimeScale");
            }

            foreach (XmlNode streamIndex in element.SelectNodes("StreamIndex"))
            {
                AvailableStreams.Add(new StreamInfo(streamIndex, Uri));
            }

            foreach (var stream in AvailableStreams)
            {
                if (stream.Type != MediaStreamType.Script)
                {
                    SelectedStreams.Add(stream);
                }
            }

            if (IsLive)
            {
                TotalTicks = 0;
            }
            else
            {
                TotalTicks = (ulong)(Duration / TimeScale * 10000000.0);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="manifestStream"></param>
        /// <param name="manifestUri"></param>
        /// <returns></returns>
        public static ManifestInfo ParseManifest(Stream manifestStream, Uri manifestUri)
        {
            var xmlDocument = new XmlDocument();

            xmlDocument.Load(manifestStream);

            var xmlNode = xmlDocument.SelectSingleNode("SmoothStreamingMedia");

            if (xmlNode == null)
            {
                throw new Exception();
            }

            return new ManifestInfo(xmlNode, manifestUri);
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public string GetDescription()
        {
            var builder = new StringBuilder();

            builder.AppendLine("Manifest:");
            builder.Append("  Duration: ");

            if (IsLive)
            {
                builder.AppendLine("LIVE");
            }
            else
            {
                builder.AppendLine(new TimeSpan((long)TotalTicks).ToString());
            }

            for (var position = 0; position < AvailableStreams.Count; position++)
            {
                builder.Append("  Stream ");
                builder.Append(position + 1);
                builder.Append(": ");
                builder.AppendLine(AvailableStreams[position].Type.ToString());

                switch (AvailableStreams[position].Type)
                {
                case MediaStreamType.Audio:
                case MediaStreamType.Video:

                    foreach (var trackInfo in AvailableStreams[position].AvailableTracks)
                    {
                        builder.Append("    ");
                        builder.Append(trackInfo.Description);

                        if (AvailableStreams[position].SelectedTracks.Contains(trackInfo))
                        {
                            builder.Append(" [selected]");
                        }

                        builder.AppendLine();
                    }

                    break;

                case MediaStreamType.Script:

                    builder.AppendLine("    Script ignored.");

                    break;

                default:

                    builder.Append("    WARNING: Unsupported track of type ");
                    builder.AppendLine(AvailableStreams[position].Type.ToString());

                    break;
                }
            }

            return builder.ToString();
        }
    }
}