using System;

namespace SmoothDownloader.Mp4
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    public class TfxdBox : Box
    {
        /// <summary>
        /// </summary>
        public byte[] Flags;

        /// <summary>
        /// </summary>
        public ulong FragmentAbsoluteTime;

        /// <summary>
        /// </summary>
        public ulong FragmentDuration;

        /// <summary>
        /// </summary>
        public byte Version;

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="start"></param>
        /// <param name="finish"></param>
        public TfxdBox(byte[] bytes, int start, int finish) : base(Mp4Identifier.Tfxd)
        {
            Version = Mp4Utils.ReadReverseBytes(bytes, 1, ref start, finish)[0];
            Flags = Mp4Utils.ReadReverseBytes(bytes, 3, ref start, finish);

            if (Version == 0)
            {
                FragmentAbsoluteTime = BitConverter.ToUInt32(Mp4Utils.ReadReverseBytes(bytes, 4, ref start, finish), 0);
                FragmentDuration = BitConverter.ToUInt32(Mp4Utils.ReadReverseBytes(bytes, 4, ref start, finish), 0);
            }
            else if (Version == 1)
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