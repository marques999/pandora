using System;
using System.Collections.Generic;
using System.Xml;

namespace SmoothDownloader.Smooth
{
    /// <summary>
    /// </summary>
    internal class StreamInfo
    {
        /// <summary>
        /// </summary>
        private readonly string _pureUrl;

        /// <summary>
        /// </summary>
        public IDictionary<string, string> Attributes;

        /// <summary>
        /// </summary>
        public IList<TrackInfo> AvailableTracks;

        /// <summary>
        /// </summary>
        public int ChunkCount;

        /// <summary>
        /// </summary>
        public IList<ChunkInfo> ChunkList;

        /// <summary>
        /// </summary>
        public IDictionary<string, string> CustomAttributes;

        /// <summary>
        /// </summary>
        public IList<TrackInfo> SelectedTracks;

        /// <summary>
        /// </summary>
        public string Subtype;

        /// <summary>
        /// </summary>
        public MediaStreamType Type;

        /// <summary>
        /// </summary>
        /// <param name="element"></param>
        /// <param name="manifestUri"></param>
        public StreamInfo(XmlNode element, Uri manifestUri)
        {
            if (element.Name != "StreamIndex")
            {
                throw new Exception();
            }

            Attributes = Parser.Attributes(element);
            CustomAttributes = Parser.CustomAttributes(element);
            Type = Parser.MediaStreamTypeAttribute(Attributes, "Type");

            if (Attributes.ContainsKey("Subtype"))
            {
                Subtype = Parser.StringAttribute(Attributes, "Subtype");
            }
            else
            {
                Subtype = "";
            }

            if (Attributes.ContainsKey("Url"))
            {
                CheckUrlAttribute();
            }

            AvailableTracks = new List<TrackInfo>();

            var xmlNodeList = element.SelectNodes("QualityLevel");

            for (var position = 0; position < xmlNodeList.Count; ++position)
            {
                TrackInfo trackInfo;

                if (Type == MediaStreamType.Audio)
                {
                    trackInfo = new AudioTrackInfo(xmlNodeList[position], Attributes, (uint)position, this);
                }
                else if (Type == MediaStreamType.Video)
                {
                    trackInfo = new VideoTrackInfo(xmlNodeList[position], Attributes, (uint)position, this);
                }
                else
                {
                    continue;
                }

                var index = 0;

                while (index < AvailableTracks.Count && AvailableTracks[index].Bitrate > trackInfo.Bitrate)
                {
                    index++;
                }

                AvailableTracks.Insert(index, trackInfo);
            }

            ChunkList = new List<ChunkInfo>();

            var num2 = 0uL;
            var xmlNodeList2 = element.SelectNodes("c");

            for (var index = 0; index < xmlNodeList2.Count; index++)
            {
                var chunkInfo = new ChunkInfo(xmlNodeList2[index], (uint)index, num2);
                ChunkList.Add(chunkInfo);
                num2 += chunkInfo.Duration;
            }

            if (Attributes.ContainsKey("Chunks"))
            {
                var chunkCount = Parser.UInt32Attribute(Attributes, "Chunks");

                if (ChunkList.Count <= 0 || ChunkList.Count == chunkCount)
                {
                    ChunkCount = (int)chunkCount;
                }
                else
                {
                    throw new Exception();
                }
            }
            else
            {
                ChunkCount = ChunkList.Count;
            }

            _pureUrl = manifestUri.AbsoluteUri;
            _pureUrl = _pureUrl.Substring(0, _pureUrl.LastIndexOf('/'));
            SelectedTracks = new List<TrackInfo>();

            if (AvailableTracks.Count > 0)
            {
                SelectedTracks.Add(AvailableTracks[0]);
            }
        }

        /// <summary>
        /// </summary>
        private void CheckUrlAttribute()
        {
            var urlPattern = Parser.StringAttribute(Attributes, "Url").Split('/');

            if (urlPattern.Length != 2)
            {
                throw new Exception();
            }

            var text2 = urlPattern[0];
            var text3 = urlPattern[1];

            urlPattern = text2.Split('(', ')');

            if (urlPattern.Length != 3 || urlPattern[2].Length != 0)
            {
                throw new Exception();
            }

            if (urlPattern[0] != "QualityLevels")
            {
                throw new Exception();
            }

            var text4 = urlPattern[1];

            urlPattern = text4.Split(',');

            if (urlPattern.Length > 2)
            {
                throw new Exception();
            }

            if (urlPattern[0] != "{bitrate}" && urlPattern[0] != "{Bitrate}")
            {
                throw new Exception();
            }

            if (urlPattern.Length == 2 && urlPattern[1] != "{CustomAttributes}")
            {
                throw new Exception();
            }

            urlPattern = text3.Split('(', ')');

            if (urlPattern.Length != 3 || urlPattern[2].Length != 0)
            {
                throw new Exception();
            }

            if (urlPattern[0] != "Fragments")
            {
                throw new Exception();
            }

            var text5 = urlPattern[1];

            urlPattern = text5.Split('=');

            if (urlPattern.Length != 2)
            {
                throw new Exception();
            }

            if (Attributes.ContainsKey("Name"))
            {
                if (urlPattern[0] != Parser.StringAttribute(Attributes, "Name"))
                {
                    throw new Exception();
                }
            }
            else if (urlPattern[0] != Parser.StringAttribute(Attributes, "Type"))
            {
                throw new Exception();
            }

            if (urlPattern[1] != "{start time}" && urlPattern[1] != "{start_time}")
            {
                throw new Exception();
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="bitrate"></param>
        /// <param name="startTime"></param>
        /// <returns></returns>
        public string GetChunkUrl(uint bitrate, ulong startTime)
        {
            return _pureUrl + "/" + Parser.StringAttribute(Attributes, "Url")
                       .Replace("{bitrate}", bitrate.ToString())
                       .Replace("{Bitrate}", bitrate.ToString())
                       .Replace("{start time}", startTime.ToString())
                       .Replace("{start_time}", startTime.ToString());
        }
    }
}