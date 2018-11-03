using System;

namespace SmoothDownloader.Mp4
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    public class TrackFragmentHeaderBox : Box
    {
        /// <summary>
        /// </summary>
        public ulong BaseDataOffset;

        /// <summary>
        /// </summary>
        public bool BaseDataOffsetPresent;

        /// <summary>
        /// </summary>
        public uint DefaultSampleDuration;

        /// <summary>
        /// </summary>
        public bool DefaultSampleDurationPresent;

        /// <summary>
        /// </summary>
        public uint DefaultSampleFlags;

        /// <summary>
        /// </summary>
        public bool DefaultSampleFlagsPresent;

        /// <summary>
        /// </summary>
        public uint DefaultSampleSize;

        /// <summary>
        /// </summary>
        public bool DefaultSampleSizePresent;

        /// <summary>
        /// </summary>
        public bool DurationIsEmpty;

        /// <summary>
        /// </summary>
        public uint SampleDescriptionIndex;

        /// <summary>
        /// </summary>
        public bool SampleDescriptionIndexPresent;

        /// <summary>
        /// </summary>
        public uint TfFlags;

        /// <summary>
        /// </summary>
        public uint TrackId;

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="boxBytes"></param>
        /// <param name="start"></param>
        /// <param name="finish"></param>
        public TrackFragmentHeaderBox(byte[] boxBytes, int start, int finish) : base(Mp4Identifier.Tfhd)
        {
            TfFlags = BitConverter.ToUInt32(Mp4Utils.ReadReverseBytes(boxBytes, 4, ref start, finish), 0);
            BaseDataOffsetPresent = (1u & TfFlags) != 0u;
            SampleDescriptionIndexPresent = (2u & TfFlags) != 0u;
            DefaultSampleDurationPresent = (8u & TfFlags) != 0u;
            DefaultSampleSizePresent = (16u & TfFlags) != 0u;
            DefaultSampleFlagsPresent = (32u & TfFlags) != 0u;
            DurationIsEmpty = (65536u & TfFlags) != 0u;
            TrackId = BitConverter.ToUInt32(Mp4Utils.ReadReverseBytes(boxBytes, 4, ref start, finish), 0);

            if (BaseDataOffsetPresent)
            {
                BaseDataOffset = BitConverter.ToUInt64(Mp4Utils.ReadReverseBytes(boxBytes, 8, ref start, finish), 0);
            }

            if (SampleDescriptionIndexPresent)
            {
                SampleDescriptionIndex = BitConverter.ToUInt32(Mp4Utils.ReadReverseBytes(boxBytes, 4, ref start, finish), 0);
            }

            if (DefaultSampleDurationPresent)
            {
                DefaultSampleDuration = BitConverter.ToUInt32(Mp4Utils.ReadReverseBytes(boxBytes, 4, ref start, finish), 0);
            }

            if (DefaultSampleSizePresent)
            {
                DefaultSampleSize = BitConverter.ToUInt32(Mp4Utils.ReadReverseBytes(boxBytes, 4, ref start, finish), 0);
            }

            if (DefaultSampleFlagsPresent)
            {
                DefaultSampleFlags = BitConverter.ToUInt32(Mp4Utils.ReadReverseBytes(boxBytes, 4, ref start, finish), 0);
            }
        }
    }
}