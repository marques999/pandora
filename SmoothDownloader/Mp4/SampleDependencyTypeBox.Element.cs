namespace SmoothDownloader.Mp4
{
    /// <summary>
    /// </summary>
    public partial class SampleDependencyTypeBox
    {
        /// <summary>
        /// </summary>
        public class Element
        {
            /// <summary>
            /// </summary>
            public byte Reserved;

            /// <summary>
            /// </summary>
            public byte SampleDependsOn;

            /// <summary>
            /// </summary>
            public byte SampleHasRedundancy;

            /// <summary>
            /// </summary>
            public byte SampleIsDependedOn;

            /// <summary>
            /// </summary>
            /// <param name="reserved"></param>
            /// <param name="sampleDependsOn"></param>
            /// <param name="sampleIsDependedOn"></param>
            /// <param name="sampleHasRedundancy"></param>
            public Element(byte reserved, byte sampleDependsOn, byte sampleIsDependedOn, byte sampleHasRedundancy)
            {
                Reserved = reserved;
                SampleDependsOn = sampleDependsOn;
                SampleIsDependedOn = sampleIsDependedOn;
                SampleHasRedundancy = sampleHasRedundancy;
            }
        }
    }
}