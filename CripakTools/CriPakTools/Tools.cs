using System.IO;
using System.Text;

namespace CriPakTools
{
    /// <summary>
    /// </summary>
    internal static class Tools
    {
        /// <summary>
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="maximumLength"></param>
        /// <param name="offset"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string ReadCString(BinaryReader reader, int maximumLength = -1, long offset = -1, Encoding encoding = null)
        {
            var current = 0;
            var maximum = maximumLength == -1 ? 255 : maximumLength;
            var initialPosition = reader.BaseStream.Position;

            if (offset > -1)
            {
                reader.BaseStream.Seek(offset, SeekOrigin.Begin);
            }

            do
            {
                if (reader.ReadByte() == 0)
                {
                    break;
                }
            } while (++current < maximum);

            if (maximumLength == -1)
            {
                maximum = current + 1;
            }
            else
            {
                maximum = maximumLength;
            }

            string result;

            reader.BaseStream.Seek(offset > -1 ? offset : initialPosition, SeekOrigin.Begin);

            if (encoding == null)
            {
                result = Encoding.GetEncoding("SJIS").GetString(reader.ReadBytes(current));
            }
            else
            {
                result = encoding.GetString(reader.ReadBytes(current));
            }

            if (offset > -1)
            {
                reader.BaseStream.Seek(initialPosition, SeekOrigin.Begin);
            }
            else
            {
                reader.BaseStream.Seek(initialPosition + maximum, SeekOrigin.Begin);
            }

            return result;
        }

        /// <summary>
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static byte[] GetData(BinaryReader reader, long offset, int length)
        {
            var position = reader.BaseStream.Position;

            reader.BaseStream.Seek(offset, SeekOrigin.Begin);

            var result = reader.ReadBytes(length);

            reader.BaseStream.Seek(position, SeekOrigin.Begin);

            return result;
        }
    }
}