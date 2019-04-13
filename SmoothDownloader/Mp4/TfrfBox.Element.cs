using System;

namespace SmoothDownloader.Mp4
{
    public partial class TfrfBox
    {
        /// <summary>
        /// </summary>
        public class Element
        {
            /// <summary>
            /// </summary>
            public ulong FragmentAbsoluteTime;

            /// <summary>
            /// </summary>
            public ulong FragmentDuration;

            /// <summary>
            /// </summary>
            /// <param name="bytes"></param>
            /// <param name="version"></param>
            /// <param name="start"></param>
            /// <param name="finish"></param>
            public Element(byte[] bytes, byte version, ref int start, int finish)
            {
                if (version == 0)
                {
                    FragmentAbsoluteTime = BitConverter.ToUInt32(Mp4Utils.ReadReverseBytes(bytes, 4, ref start, finish), 0);
                    FragmentDuration = BitConverter.ToUInt32(Mp4Utils.ReadReverseBytes(bytes, 4, ref start, finish), 0);
                }
                else if (version == 1)
                {
                    FragmentAbsoluteTime = BitConverter.ToUInt64(Mp4Utils.ReadReverseBytes(bytes, 8, ref start, finish), 0);
                    FragmentDuration = BitConverter.ToUInt64(Mp4Utils.ReadReverseBytes(bytes, 8, ref start, finish), 0);
                }
                else
                {
                    throw new Exception();
                }
            }
        }
    }
}