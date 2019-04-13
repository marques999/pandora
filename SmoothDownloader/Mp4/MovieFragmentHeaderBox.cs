using System;

namespace SmoothDownloader.Mp4
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    public class MovieFragmentHeaderBox : Box
    {
        /// <summary>
        /// </summary>
        public uint SequenceNumber;

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="start"></param>
        /// <param name="finish"></param>
        public MovieFragmentHeaderBox(byte[] bytes, int start, int finish) : base(Mp4Identifier.Mfhd)
        {
            if (finish - start == 8)
            {
                start += 4;
                SequenceNumber = BitConverter.ToUInt32(Mp4Utils.ReadReverseBytes(bytes, 4, ref start, finish), 0);
            }
            else
            {
                throw new Exception();
            }
        }
    }
}