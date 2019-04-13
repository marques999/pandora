namespace SmoothDownloader.Mp4
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    public class TrackFragmentBox : Box
    {
        /// <summary>
        /// </summary>
        public SampleDependencyTypeBox Sdtp;

        /// <summary>
        /// </summary>
        public TrackFragmentHeaderBox Tfhd;

        /// <summary>
        /// </summary>
        public TfrfBox Tfrf;

        /// <summary>
        /// </summary>
        public TfxdBox Tfxd;

        /// <summary>
        /// </summary>
        public TrackRunBox Trun;

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="start"></param>
        /// <param name="finish"></param>
        public TrackFragmentBox(byte[] bytes, int start, int finish) : base(Mp4Identifier.Traf)
        {
            while (start < finish)
            {
                var mp4Box = Mp4Utils.GetBox(bytes, ref start, finish);

                switch (mp4Box?.Mp4Identifier)
                {
                case null:
                    continue;
                case Mp4Identifier.Tfhd:
                    Tfhd = mp4Box as TrackFragmentHeaderBox;
                    break;
                case Mp4Identifier.Sdtp:
                    Sdtp = mp4Box as SampleDependencyTypeBox;
                    break;
                case Mp4Identifier.Trun:
                    Trun = mp4Box as TrackRunBox;
                    break;
                case Mp4Identifier.Tfrf:
                    Tfrf = mp4Box as TfrfBox;
                    break;
                case Mp4Identifier.Tfxd:
                    Tfxd = mp4Box as TfxdBox;
                    break;
                }
            }
        }
    }
}