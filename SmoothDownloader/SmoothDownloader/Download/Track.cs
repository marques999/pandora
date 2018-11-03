using SmoothDownloader.Smooth;

namespace SmoothDownloader.Download
{
    /// <summary>
    /// </summary>
    internal class Track
    {
        /// <summary>
        /// </summary>
        public int DownloadedChunkCount = 0;

        /// <summary>
        /// </summary>
        public ulong NextStartTime;

        /// <summary>
        /// </summary>
        public TrackInfo TrackInfo;

        /// <summary>
        /// </summary>
        /// <param name="trackInfo"></param>
        public Track(TrackInfo trackInfo)
        {
            TrackInfo = trackInfo;
        }
    }
}