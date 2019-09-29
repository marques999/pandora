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

            FillMyBuffer(0x08);

            return BitConverter.ToDouble(_buffer.Take(0x08).Reverse().ToArray(), 0x00);
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

            FillMyBuffer(0x02);

            return BitConverter.ToInt16(_buffer.Take(0x02).Reverse().ToArray(), 0x00);
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

            FillMyBuffer(0x04);

            return BitConverter.ToInt32(_buffer.Take(0x04).Reverse().ToArray(), 0x00);
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

            FillMyBuffer(0x08);

            return BitConverter.ToInt64(_buffer.Take(0x08).Reverse().ToArray(), 0x00);
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

            FillMyBuffer(0x04);

            return BitConverter.ToSingle(_buffer.Take(0x04).Reverse().ToArray(), 0x00);
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

            FillMyBuffer(0x02);

            return BitConverter.ToUInt16(_buffer.Take(0x02).Reverse().ToArray(), 0x00);
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

            FillMyBuffer(0x04);

            return BitConverter.ToUInt32(_buffer.Take(0x04).Reverse().ToArray(), 0x00);
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

            FillMyBuffer(0x08);

            return BitConverter.ToUInt64(_buffer.Take(0x08).Reverse().ToArray(), 0x00);
        }

        /// <summary>
        /// </summary>
        /// <param name="byteCount"></param>
        private void FillMyBuffer(int byteCount)
        {
            var offset = 0;

            if (byteCount == 1)
            {
                _buffer[0] = (byte)BaseStream.ReadByte();
            }
            else
            {
                do
                {
                    offset += BaseStream.Read(_buffer, offset, byteCount - offset);
                } while (offset < byteCount);
            }
        }
    }
}