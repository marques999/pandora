using System;
using System.IO;
using System.Linq;
using System.Text;

namespace CriPakTools
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    public class EndianReader : BinaryReader
    {
        /// <summary>
        /// </summary>
        private readonly byte[] _buffer = new byte[8];

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="littleEndian"></param>
        public EndianReader(Stream stream, bool littleEndian) : base(stream, Encoding.UTF8)
        {
            LittleEndian = littleEndian;
        }

        /// <summary>
        /// </summary>
        public bool LittleEndian { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <returns></returns>
        public override double ReadDouble()
        {
            if (LittleEndian)
            {
                return base.ReadDouble();
            }

            FillMyBuffer(8);

            return BitConverter.ToDouble(_buffer.Take(8).Reverse().ToArray(), 0);
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <returns></returns>
        public override short ReadInt16()
        {
            if (LittleEndian)
            {
                return base.ReadInt16();
            }

            FillMyBuffer(2);

            return BitConverter.ToInt16(_buffer.Take(2).Reverse().ToArray(), 0);
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <returns></returns>
        public override int ReadInt32()
        {
            if (LittleEndian)
            {
                return base.ReadInt32();
            }

            FillMyBuffer(4);

            return BitConverter.ToInt32(_buffer.Take(4).Reverse().ToArray(), 0);
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <returns></returns>
        public override long ReadInt64()
        {
            if (LittleEndian)
            {
                return base.ReadInt64();
            }

            FillMyBuffer(8);

            return BitConverter.ToInt64(_buffer.Take(8).Reverse().ToArray(), 0);
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <returns></returns>
        public override float ReadSingle()
        {
            if (LittleEndian)
            {
                return base.ReadSingle();
            }

            FillMyBuffer(4);

            return BitConverter.ToSingle(_buffer.Take(4).Reverse().ToArray(), 0);
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <returns></returns>
        public override ushort ReadUInt16()
        {
            if (LittleEndian)
            {
                return base.ReadUInt16();
            }

            FillMyBuffer(2);

            return BitConverter.ToUInt16(_buffer.Take(2).Reverse().ToArray(), 0);
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <returns></returns>
        public override uint ReadUInt32()
        {
            if (LittleEndian)
            {
                return base.ReadUInt32();
            }

            FillMyBuffer(4);

            return BitConverter.ToUInt32(_buffer.Take(4).Reverse().ToArray(), 0);
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <returns></returns>
        public override ulong ReadUInt64()
        {
            if (LittleEndian)
            {
                return base.ReadUInt64();
            }

            FillMyBuffer(8);

            return BitConverter.ToUInt64(_buffer.Take(8).Reverse().ToArray(), 0);
        }

        /// <summary>
        /// </summary>
        /// <param name="byteCount"></param>
        private void FillMyBuffer(int byteCount)
        {
            if (byteCount == 1)
            {
                _buffer[0] = (byte)BaseStream.ReadByte();
            }
            else
            {
                var offset = 0;

                do
                {
                    offset += BaseStream.Read(_buffer, offset, byteCount - offset);
                } while (offset < byteCount);
            }
        }
    }
}