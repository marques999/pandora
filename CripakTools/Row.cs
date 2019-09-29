using System;

namespace CriPakTools
{
    /// <summary>
    /// </summary>
    public class Row
    {
        /// <summary>
        /// </summary>
        public int Type { get; set; } = -1;

        /// <summary>
        /// </summary>
        public byte Uint8 { get; set; }

        /// <summary>
        /// </summary>
        public ushort Uint16 { get; set; }

        /// <summary>
        /// </summary>
        public uint Uint32 { get; set; }

        /// <summary>
        /// </summary>
        public ulong Uint64 { get; set; }

        /// <summary>
        /// </summary>
        public float Ufloat { get; set; }

        /// <summary>
        /// </summary>
        public string Str { get; set; }

        /// <summary>
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// </summary>
        public long Position { get; set; }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public object GetValue()
        {
            switch (Type)
            {
            case 0x00: case 0x01:
                return Uint8;
            case 0x02: case 0x03:
                return Uint16;
            case 0x04: case 0x05:
                return Uint32;
            case 0x06: case 0x07:
                return Uint64;
            case 0x08:
                return Ufloat;
            case 0x0A:
                return Str;
            case 0x0B:
                return Data;
            default:
                return null;
            }
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public new Type GetType()
        {
            switch (Type)
            {
            case 0x00: case 0x01:
                return Uint8.GetType();
            case 0x02: case 0x03:
                return Uint16.GetType();
            case 0x04: case 0x05:
                return Uint32.GetType();
            case 0x06: case 0x07:
                return Uint64.GetType();
            case 0x08:
                return Ufloat.GetType();
            case 0x0A:
                return Str.GetType();
            case 0x0B:
                return Data.GetType();
            default:
                return null;
            }
        }
    }
}