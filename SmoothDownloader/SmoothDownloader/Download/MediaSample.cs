namespace SmoothDownloader.Download
{
    /// <summary>
    /// </summary>
    internal class MediaSample
    {
        /// <summary>
        /// </summary>
        public bool IsKeyFrame;

        /// <summary>
        /// </summary>
        public int Length;

        /// <summary>
        /// </summary>
        public long Offset;

        /// <summary>
        /// </summary>
        public ulong StartTime;

        /// <summary>
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="startTime"></param>
        /// <param name="isKeyFrame"></param>
        public MediaSample(long offset, int length, ulong startTime, bool isKeyFrame)
        {
            Offset = offset;
            Length = length;
            StartTime = startTime;
            IsKeyFrame = isKeyFrame;
        }
    }
}