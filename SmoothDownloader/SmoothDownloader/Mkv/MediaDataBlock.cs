using System;

namespace SmoothDownloader.Mkv
{
    /// <summary>
    /// </summary>
    public class MediaDataBlock
    {
        /// <summary>
        /// </summary>
        public ArraySegment<byte> Bytes;

        /// <summary>
        /// </summary>
        public bool IsKeyFrame;

        /// <summary>
        /// </summary>
        public ulong StartTime;

        /// <summary>
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="startTime"></param>
        /// <param name="isKeyFrame"></param>
        public MediaDataBlock(ArraySegment<byte> bytes, ulong startTime, bool isKeyFrame)
        {
            Bytes = bytes;
            StartTime = startTime;
            IsKeyFrame = isKeyFrame;
        }
    }
}