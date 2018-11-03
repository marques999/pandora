using SmoothDownloader.Smooth;

namespace SmoothDownloader.Mkv
{
    /// <summary>
    /// </summary>
    public struct CuePoint
    {
        /// <summary>
        /// </summary>
        private readonly ulong _cueTime;

        /// <summary>
        /// </summary>
        private readonly ulong _cueTrack;

        /// <summary>
        /// </summary>
        public ulong CueClusterPosition;

        /// <summary>
        /// </summary>
        /// <param name="cueTime"></param>
        /// <param name="cueTrack"></param>
        /// <param name="cueClusterPosition"></param>
        public CuePoint(ulong cueTime, ulong cueTrack, ulong cueClusterPosition)
        {
            _cueTime = cueTime;
            _cueTrack = cueTrack;
            CueClusterPosition = cueClusterPosition;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes() => MkvUtils.GetEeBytes(MkvIdentifier.CuePoint, Utils.CombineBytes(
            MkvUtils.GetEeBytes(MkvIdentifier.CueTime, MkvUtils.GetVintBytes(_cueTime)),
            MkvUtils.GetEeBytes(MkvIdentifier.CueTrackPositions, Utils.CombineBytes(
            MkvUtils.GetEeBytes(MkvIdentifier.CueTrack, MkvUtils.GetVintBytes(_cueTrack)),
            MkvUtils.GetEeBytes(MkvIdentifier.CueClusterPosition, MkvUtils.GetVintBytes(CueClusterPosition)))))
        );
    }
}