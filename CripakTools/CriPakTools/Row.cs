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
            case 0: case 1:
                return Uint8;
            case 2: case 3:
                return Uint16;
            case 4: case 5:
                return Uint32;
            case 6: case 7:
                return Uint64;
            case 8:
                return Ufloat;
            case 10:
                return Str;
            case 11:
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
            case 0: case 1:
                return Uint8.GetType();
            case 2: case 3:
                return Uint16.GetType();
            case 4: case 5:
                return Uint32.GetType();
            case 6: case 7:
                return Uint64.GetType();
            case 8:
                return Ufloat.GetType();
            case 10:
                return Str.GetType();
            case 11:
                return Data.GetType();
            default:
                return null;
            }
        }
    }
}