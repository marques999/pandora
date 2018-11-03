namespace SmoothDownloader.Mkv
{
    /// <summary>
    /// </summary>
    public interface IChunkStartTimeReceiver
    {
        /// <summary>
        /// </summary>
        /// <param name="trackIndex"></param>
        /// <param name="chunkIndex"></param>
        /// <returns></returns>
        ulong GetChunkStartTime(int trackIndex, int chunkIndex);

        /// <summary>
        /// </summary>
        /// <param name="trackIndex"></param>
        /// <param name="chunkIndex"></param>
        /// <param name="startTime"></param>
        void SetChunkStartTime(int trackIndex, int chunkIndex, ulong startTime);
    }
}