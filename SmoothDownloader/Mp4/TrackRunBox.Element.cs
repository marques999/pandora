namespace SmoothDownloader.Mp4
{
    public partial class TrackRunBox
    {
        /// <summary>
        /// </summary>
        public struct Element
        {
            /// <summary>
            /// </summary>
            public uint SampleSize;

            /// <summary>
            /// </summary>
            public uint SampleFlags;

            /// <summary>
            /// </summary>
            public uint SampleDuration;

            /// <summary>
            /// </summary>
            public uint SampleCompositionTimeOffset;

            /// <summary>
            /// </summary>
            /// <param name="sampleDuration"></param>
            /// <param name="sampleSize"></param>
            /// <param name="sampleFlags"></param>
            /// <param name="sampleCompositionTimeOffset"></param>
            public Element(uint sampleDuration, uint sampleSize, uint sampleFlags, uint sampleCompositionTimeOffset)
            {
                SampleDuration = sampleDuration;
                SampleSize = sampleSize;
                SampleFlags = sampleFlags;
                SampleCompositionTimeOffset = sampleCompositionTimeOffset;
            }
        }
    }
}