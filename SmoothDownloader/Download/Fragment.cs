using SmoothDownloader.Mp4;

namespace SmoothDownloader.Download
{
    /// <summary>
    /// </summary>
    public class Fragment
    {
        /// <summary>
        /// </summary>
        public MediaDataBox MediaDataBox;

        /// <summary>
        /// </summary>
        public MovieFragmentBox MovieFragmentBox;

        /// <summary>
        /// </summary>
        /// <param name="boxBytes"></param>
        /// <param name="start"></param>
        /// <param name="finish"></param>
        public Fragment(byte[] boxBytes, int start, int finish)
        {
            while (start < finish)
            {
                var mp4Box = Mp4Utils.GetBox(boxBytes, ref start, finish);

                switch (mp4Box?.Mp4Identifier)
                {
                case null:
                    continue;
                case Mp4Identifier.Mdat:
                    MediaDataBox = mp4Box as MediaDataBox;
                    break;
                case Mp4Identifier.Moof:
                    MovieFragmentBox = mp4Box as MovieFragmentBox;
                    break;
                }
            }
        }
    }
}