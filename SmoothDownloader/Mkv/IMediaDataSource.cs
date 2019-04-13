namespace SmoothDownloader.Mkv
{
    /// <summary>
    /// </summary>
    public interface IMediaDataSource
    {
        /// <summary>
        /// </summary>
        /// <returns></returns>
        int GetTrackCount();

        /// <summary>
        /// </summary>
        /// <param name="trackIndex"></param>
        /// <returns></returns>
        ulong GetTrackEndTime(int trackIndex);

        /// <summary>
        /// </summary>
        /// <param name="startTimeReceiver"></param>
        void StartChunks(IChunkStartTimeReceiver startTimeReceiver);

        /// <summary>
        /// </summary>
        /// <param name="trackIndex"></param>
        /// <returns></returns>
        MediaDataBlock PeekBlock(int trackIndex);

        /// <summary>
        /// </summary>
        /// <param name="trackIndex"></param>
        void ConsumeBlock(int trackIndex);

        /// <summary>
        /// </summary>
        /// <param name="trackIndex"></param>
        /// <param name="startTime"></param>
        void ConsumeBlocksUntil(int trackIndex, ulong startTime);
    }
}