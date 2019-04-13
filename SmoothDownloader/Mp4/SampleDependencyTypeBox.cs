using System;

namespace SmoothDownloader.Mp4
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    public partial class SampleDependencyTypeBox : Box
    {
        /// <summary>
        /// </summary>
        public Element[] Array;

        /// <summary>
        /// </summary>
        public uint Version;

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="start"></param>
        /// <param name="finish"></param>
        public SampleDependencyTypeBox(byte[] bytes, int start, int finish) : base(Mp4Identifier.Sdtp)
        {
            Array = new Element[finish - start];
            Version = BitConverter.ToUInt32(Mp4Utils.ReadReverseBytes(bytes, 4, ref start, finish), 0);

            for (var index = 0; index < finish - start; index++)
            {
                var value = Mp4Utils.ReadReverseBytes(bytes, 1, ref start, finish)[0];
                var reserved = (byte)(value >> 6);

                value <<= 2;

                var sampleDependsOn = (byte)(value >> 6);

                value <<= 2;
                Array[index] = new Element(reserved, sampleDependsOn, (byte)(value >> 6), (byte)((value << 2) >> 6));
            }
        }
    }
}