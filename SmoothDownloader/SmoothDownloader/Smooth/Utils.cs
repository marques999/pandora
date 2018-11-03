using System;
using System.Collections.Generic;
using System.Linq;

namespace SmoothDownloader.Smooth
{
    /// <summary>
    /// </summary>
    public class Utils
    {
        /// <summary>
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static byte[] InplaceReverseBytes(byte[] a)
        {
            Array.Reverse(a);
            return a;
        }

        /// <summary>
        /// </summary>
        /// <param name="arrays"></param>
        /// <returns></returns>
        public static byte[] CombineByteArrays(IList<byte[]> arrays)
        {
            var position = 0;
            var total = arrays.Sum(t => t.Length);
            var array = new byte[total];

            foreach (var bytes in arrays)
            {
                Buffer.BlockCopy(bytes, 0, array, position, bytes.Length);
                position += bytes.Length;
            }

            return array;
        }

        /// <summary>
        /// </summary>
        /// <param name="contents"></param>
        /// <param name="segments"></param>
        /// <returns></returns>
        public static byte[] CombineByteArraysAndArraySegments(IList<byte[]> contents, IList<ArraySegment<byte>> segments)
        {
            var total = 0;

            if (contents != null)
            {
                total += contents.Sum(t => t.Length);
            }

            total += segments.Sum(t => t.Count);

            var position = 0;
            var buffer = new byte[total];

            if (contents != null)
            {
                foreach (var bytes in contents)
                {
                    Buffer.BlockCopy(bytes, 0, buffer, position, bytes.Length);
                    position += bytes.Length;
                }
            }

            foreach (var segment in segments)
            {
                Buffer.BlockCopy(segment.Array, segment.Offset, buffer, position, segment.Count);
                position += segment.Count;
            }

            return buffer;
        }

        /// <summary>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static byte[] CombineBytes(byte[] a, byte[] b)
        {
            var buffer = new byte[a.Length + b.Length];

            Buffer.BlockCopy(a, 0, buffer, 0, a.Length);
            Buffer.BlockCopy(b, 0, buffer, a.Length, b.Length);

            return buffer;
        }

        /// <summary>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static byte[] CombineBytes(byte[] a, byte[] b, byte[] c)
        {
            var buffer = new byte[a.Length + b.Length + c.Length];

            Buffer.BlockCopy(a, 0, buffer, 0, a.Length);
            Buffer.BlockCopy(b, 0, buffer, a.Length, b.Length);
            Buffer.BlockCopy(c, 0, buffer, a.Length + b.Length, c.Length);

            return buffer;
        }

        /// <summary>
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string HexEncodeString(byte[] bytes)
        {
            var buffer = new char[bytes.Length << 1];

            for (int index = 0, cx = 0; index < bytes.Length; ++index)
            {
                var value = (byte)(bytes[index] >> 4);

                if (value > 9)
                {
                    buffer[cx++] = (char)(value + 0x57);
                }
                else
                {
                    buffer[cx++] = (char)(value + 0x30);
                }

                value = (byte)(bytes[index] & 0x0F);
                buffer[cx++] = (char)(value > 9 ? value + 0x57 : value + 0x30);
            }

            return new string(buffer);
        }

        /// <summary>
        /// </summary>
        /// <param name="hexEncodedData"></param>
        /// <returns></returns>
        public static byte[] HexDecodeString(string hexEncodedData)
        {
            if (hexEncodedData == null)
            {
                return null;
            }

            var buffer = new byte[hexEncodedData.Length >> 1];

            for (int index = 0, sx = 0; index < buffer.Length; ++index)
            {
                var value = hexEncodedData[sx++];

                if (value - (uint)'0' > 9 && value - (uint)'A' > 5 && value - (uint)'a' > 5)
                {
                    return null;
                }

                buffer[index] = (byte)((value > '9' ? (value > 'Z' ? value - 'a' + 10 : value - 'A' + 10) : value - '0') << 4);
                value = hexEncodedData[sx++];

                if (value - (uint)'0' <= 9 || value - (uint)'A' <= 5 || value - (uint)'a' <= 5)
                {
                    buffer[index] |= (byte)(value > '9' ? (value > 'Z' ? value - 'a' + 10 : value - 'A' + 10) : value - '0');
                }
                else
                {
                    return null;
                }
            }

            return buffer;
        }

        /// <summary>
        /// </summary>
        /// <param name="hexEncodedData"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static byte[] HexDecodeBytes(byte[] hexEncodedData, int start, int end)
        {
            if (hexEncodedData == null)
            {
                return null;
            }

            if (start < 0)
            {
                start = 0;
            }

            if (end >= hexEncodedData.Length)
            {
                end = hexEncodedData.Length;
            }

            if (start >= end)
            {
                return new byte[] { };
            }

            if (((end - start) & 1) != 0)
            {
                return null;
            }

            var buffer = new byte[(end - start) >> 1];

            for (int bx = 0, sx = start; bx < buffer.Length; ++bx)
            {
                var character = hexEncodedData[sx++];

                if (character - (uint)'0' > 9 && character - (uint)'A' > 5 && character - (uint)'a' > 5)
                {
                    return null;
                }

                buffer[bx] = (byte)((character > '9' ? (character > 'Z' ? character - 'a' + 10 : character - 'A' + 10) : character - '0') << 4);
                character = hexEncodedData[sx++];

                if (character - (uint)'0' > 9 && character - (uint)'A' > 5 && character - (uint)'a' > 5)
                {
                    return null;
                }

                buffer[bx] |= (byte)(character > '9' ? (character > 'Z' ? character - 'a' + 10 : character - 'A' + 10) : character - '0');
            }

            return buffer;
        }

        /// <summary>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string EscapeString(string value)
        {
            return "\"" + value.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"";
        }

        /// <summary>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static bool ArePrefixBytesEqual(byte[] a, byte[] b, int length)
        {
            for (var index = 0; index < length; ++index)
            {
                if (a[index] != b[index])
                {
                    return false;
                }
            }

            return true;
        }
    }
}