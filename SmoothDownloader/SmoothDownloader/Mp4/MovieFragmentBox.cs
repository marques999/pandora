namespace SmoothDownloader.Mp4
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    public class MovieFragmentBox : Box
    {
        /// <summary>
        /// </summary>
        public MovieFragmentHeaderBox MovieFragmentHeaderBox;

        /// <summary>
        /// </summary>
        public TrackFragmentBox TrackFragmentBox;

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="start"></param>
        /// <param name="finish"></param>
        public MovieFragmentBox(byte[] bytes, int start, int finish) : base(Mp4Identifier.Moof)
        {
            while (start < finish)
            {
                var mp4Box = Mp4Utils.GetBox(bytes, ref start, finish);

                switch (mp4Box?.Mp4Identifier)
                {
                case null:
                    continue;
                case Mp4Identifier.Traf:
                    TrackFragmentBox = mp4Box as TrackFragmentBox;
                    break;
                case Mp4Identifier.Mfhd:
                    MovieFragmentHeaderBox = mp4Box as MovieFragmentHeaderBox;
                    break;
                }
            }
        }
    }
}