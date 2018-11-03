using System;

namespace SmoothDownloader.Mp4
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    public partial class TrackRunBox : Box
    {
        /// <summary>
        /// </summary>
        public Element[] Array;

        /// <summary>
        /// </summary>
        public int DataOffset;

        /// <summary>
        /// </summary>
        public bool DataOffsetPresent;

        /// <summary>
        /// </summary>
        public uint FirstSampleFlags;

        /// <summary>
        /// </summary>
        public bool FirstSampleFlagsPresent;

        /// <summary>
        /// </summary>
        public bool SampleCompositionTimeOffsetsPresent;

        /// <summary>
        /// </summary>
        public uint SampleCount;

        /// <summary>
        /// </summary>
        public bool SampleDurationPresent;

        /// <summary>
        /// </summary>
        public bool SampleFlagsPresent;

        /// <summary>
        /// </summary>
        public bool SampleSizePresent;

        /// <summary>
        /// </summary>
        public uint TrFlags;

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="boxBytes"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public TrackRunBox(byte[] boxBytes, int start, int end) : base(Mp4Identifier.Trun)
        {
            TrFlags = BitConverter.ToUInt32(Mp4Utils.ReadReverseBytes(boxBytes, 4, ref start, end), 0);
            DataOffsetPresent = (1u & TrFlags) != 0u;
            FirstSampleFlagsPresent = (4u & TrFlags) != 0u;
            SampleDurationPresent = (256u & TrFlags) != 0u;
            SampleSizePresent = (512u & TrFlags) != 0u;
            SampleFlagsPresent = (1024u & TrFlags) != 0u;
            SampleCompositionTimeOffsetsPresent = (2048u & TrFlags) != 0u;
            SampleCount = BitConverter.ToUInt32(Mp4Utils.ReadReverseBytes(boxBytes, 4, ref start, end), 0);

            if (DataOffsetPresent)
            {
                DataOffset = BitConverter.ToInt32(Mp4Utils.ReadReverseBytes(boxBytes, 4, ref start, end), 0);
            }

            if (FirstSampleFlagsPresent)
            {
                FirstSampleFlags = BitConverter.ToUInt32(Mp4Utils.ReadReverseBytes(boxBytes, 4, ref start, end), 0);
            }

            Array = new Element[SampleCount];

            for (var offset = 0; offset < Array.Length; offset++)
            {
                var sampleDuration = 0u;
                var sampleSize = 0u;
                var sampleFlags = 0u;
                var sampleCompositionTimeOffset = 0u;

                if (SampleDurationPresent)
                {
                    sampleDuration = BitConverter.ToUInt32(Mp4Utils.ReadReverseBytes(boxBytes, 4, ref start, end), 0);
                }

                if (SampleSizePresent)
                {
                    sampleSize = BitConverter.ToUInt32(Mp4Utils.ReadReverseBytes(boxBytes, 4, ref start, end), 0);
                }

                if (SampleFlagsPresent)
                {
                    sampleFlags = BitConverter.ToUInt32(Mp4Utils.ReadReverseBytes(boxBytes, 4, ref start, end), 0);
                }

                if (SampleCompositionTimeOffsetsPresent)
                {
                    sampleCompositionTimeOffset = BitConverter.ToUInt32(Mp4Utils.ReadReverseBytes(boxBytes, 4, ref start, end), 0);
                }

                Array[offset] = new Element(sampleDuration, sampleSize, sampleFlags, sampleCompositionTimeOffset);
            }
        }
    }
}