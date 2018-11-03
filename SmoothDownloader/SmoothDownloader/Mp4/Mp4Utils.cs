using System;

namespace SmoothDownloader.Mp4
{
    /// <summary>
    /// </summary>
    internal class Mp4Utils
    {
        /// <summary>
        /// </summary>
        private static readonly Guid TfxdGuid = new Guid("6D1D9B05-42D5-44E6-80E2-141DAFF757B2");

        /// <summary>
        /// </summary>
        private static readonly Guid TfrfGuid = new Guid("D4807EF2-CA39-4695-8E54-26CB9E46A79F");

        /// <summary>
        /// </summary>
        private static readonly Guid PiffGuid = new Guid("A2394F52-5A9B-4F14-A244-6C427C648DF4");

        /// <summary>
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="length"></param>
        /// <param name="start"></param>
        /// <param name="finish"></param>
        /// <returns></returns>
        public static byte[] ReadReverseBytes(byte[] bytes, int length, ref int start, int finish)
        {
            if (start + length > finish)
            {
                throw new Exception();
            }

            var elements = new byte[length];

            Buffer.BlockCopy(bytes, start, elements, 0, length);
            start += length;
            Array.Reverse(elements);

            return elements;
        }

        /// <summary>
        /// </summary>
        /// <param name="boxBytes"></param>
        /// <param name="start"></param>
        /// <param name="finish"></param>
        /// <returns></returns>
        public static Box GetBox(byte[] boxBytes, ref int start, int finish)
        {
            var length = BitConverter.ToInt32(ReadReverseBytes(boxBytes, 4, ref start, finish), 0);
            var identifier = BitConverter.ToUInt32(ReadReverseBytes(boxBytes, 4, ref start, finish), 0);

            if (length == 1)
            {
                length = (int)BitConverter.ToUInt64(ReadReverseBytes(boxBytes, 8, ref start, finish), 0) - 16;
            }
            else if (length == 0)
            {
                length = finish - start;
            }
            else
            {
                length -= 8;
            }

            if (length < 0)
            {
                throw new Exception();
            }

            var previous = start;

            start += length;

            if (start > finish)
            {
                throw new Exception();
            }

            switch (identifier)
            {
            case (uint)Mp4Identifier.Mdat:

                return new MediaDataBox(previous);

            case (uint)Mp4Identifier.Mfhd:

                return new MovieFragmentHeaderBox(boxBytes, previous, start);

            case (uint)Mp4Identifier.Moof:

                return new MovieFragmentBox(boxBytes, previous, start);

            case (uint)Mp4Identifier.Sdtp:

                return new SampleDependencyTypeBox(boxBytes, previous, start);

            case (uint)Mp4Identifier.Tfhd:

                return new TrackFragmentHeaderBox(boxBytes, previous, start);

            case (uint)Mp4Identifier.Traf:

                return new TrackFragmentBox(boxBytes, previous, start);

            case (uint)Mp4Identifier.Trun:

                return new TrackRunBox(boxBytes, previous, start);

            case (uint)Mp4Identifier.Uuid:

                var uuid = new Guid(ReadReverseBytes(boxBytes, 8, ref previous, start));

                if (uuid == TfxdGuid)
                {
                    return new TfxdBox(boxBytes, previous, start);
                }

                if (uuid == TfrfGuid)
                {
                    return new TfrfBox(boxBytes, previous, start);
                }

                if (uuid == PiffGuid)
                {
                    throw new Exception();
                }

                break;
            }

            return null;
        }
    }
}