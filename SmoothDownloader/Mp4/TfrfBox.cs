namespace SmoothDownloader.Mp4
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    public partial class TfrfBox : Box
    {
        /// <summary>
        /// </summary>
        public Element[] Array;

        /// <summary>
        /// </summary>
        public byte[] Flags;

        /// <summary>
        /// </summary>
        public byte Version;

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="start"></param>
        /// <param name="finish"></param>
        public TfrfBox(byte[] bytes, int start, int finish) : base(Mp4Identifier.Tfrf)
        {
            Version = Mp4Utils.ReadReverseBytes(bytes, 1, ref start, finish)[0];
            Flags = Mp4Utils.ReadReverseBytes(bytes, 3, ref start, finish);

            var length = (int)Mp4Utils.ReadReverseBytes(bytes, 1, ref start, finish)[0];

            Array = new Element[length];

            for (var index = 0; index < length; index++)
            {
                Array[index] = new Element(bytes, Version, ref start, finish);
            }
        }
    }
}