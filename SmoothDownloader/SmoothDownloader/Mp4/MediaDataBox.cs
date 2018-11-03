namespace SmoothDownloader.Mp4
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    public class MediaDataBox : Box
    {
        /// <summary>
        /// </summary>
        public int Start;

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="start"></param>
        public MediaDataBox(int start) : base(Mp4Identifier.Mdat)
        {
            Start = start;
        }
    }
}