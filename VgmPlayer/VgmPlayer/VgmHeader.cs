using System.IO;

namespace VgmPlayer
{
    /// <summary>
    /// </summary>
    internal class VgmHeader
    {
        public byte Ay8910Flags;
        public byte Ay8910Type;
        public byte Ay8910Ym2203Flags;
        public byte Ay8910Ym2608Flags;
        public byte BytC140Type;
        public byte BytK054539Flags;
        public char BytLoopBase;
        public byte BytLoopModifier;
        public byte BytOki6258Flags;
        public byte BytReserved2;
        public byte BytReservedFlags;
        public byte BytVolumeModifier;
        public uint ClockAy8910;
        public uint ClockPwm;
        public uint ClockRf5C164;
        public uint ClockRf5C68;
        public uint ClockSegaPcm;
        public uint ClockY8950;
        public uint ClockYm2151;
        public uint ClockYm2203;
        public uint ClockYm2608;
        public uint ClockYm2610;
        public uint ClockYm2612;
        public uint ClockYm3526;
        public uint ClockYm3812;
        public uint ClockYmf262;
        public uint ClockYmf271;
        public uint ClockYmf278B;
        public uint ClockYmz280B;
        public int DataOffset;
        public uint EofOffset;
        public uint Gd3Offset;
        public uint LngExtraOffset;
        public uint LngHzC140;
        public uint LngHzGbdmg;
        public uint LngHzHuC6280;
        public uint LngHzK051649;
        public uint LngHzK053260;
        public uint LngHzK054539;
        public uint LngHzMultiPcm;
        public uint LngHzNesapu;
        public uint LngHzOkim6258;
        public uint LngHzOkim6295;
        public uint LngHzPokey;
        public uint LngHzQSound;
        public uint LngHzScsp;
        public uint LngHzUpd7759;
        public uint LngHzWSwan;
        public uint LoggingRate;
        public uint LoopOffset;
        public uint LoopSamples;
        public uint Sn76489Clock;
        public ushort Sn76489Feedback = 0x0009;
        public byte Sn76489Flags;
        public byte Sn76489Srw = 0x10;
        public uint SpcmIreg;
        public uint TotalSamples;
        public uint Version;
        public uint Ym2413Clock;

        /// <summary>
        /// </summary>
        /// <param name="stream"></param>
        public VgmHeader(BinaryReader stream)
        {
            EofOffset = stream.ReadUInt32();
            Version = stream.ReadUInt32();
            Sn76489Clock = stream.ReadUInt32();
            Ym2413Clock = stream.ReadUInt32();
            Gd3Offset = stream.ReadUInt32();
            TotalSamples = stream.ReadUInt32();
            LoopOffset = stream.ReadUInt32();
            LoopSamples = stream.ReadUInt32();

            if (Version >= 0x101)
            {
                LoggingRate = stream.ReadUInt32();
            }

            if (Version >= 0x110)
            {
                Sn76489Feedback = stream.ReadUInt16();
                Sn76489Srw = stream.ReadByte();
                Sn76489Flags = stream.ReadByte();
                ClockYm2612 = stream.ReadUInt32();
                ClockYm2151 = stream.ReadUInt32();
            }
            else
            {
                ClockYm2612 = Ym2413Clock;
                ClockYm2151 = Ym2413Clock;
            }

            if (Version >= 0x150)
            {
                DataOffset = stream.ReadInt32();
            }

            if (Version < 0x151)
            {
                return;
            }

            ClockSegaPcm = stream.ReadUInt32();
            SpcmIreg = stream.ReadUInt32();
            ClockRf5C68 = stream.ReadUInt32();
            ClockYm2203 = stream.ReadUInt32();
            ClockYm2608 = stream.ReadUInt32();
            ClockYm2610 = stream.ReadUInt32();
            ClockYm3812 = stream.ReadUInt32();
            ClockYm3526 = stream.ReadUInt32();
            ClockY8950 = stream.ReadUInt32();
            ClockYmf262 = stream.ReadUInt32();
            ClockYmf278B = stream.ReadUInt32();
            ClockYmf271 = stream.ReadUInt32();
            ClockYmz280B = stream.ReadUInt32();
            ClockRf5C164 = stream.ReadUInt32();
            ClockPwm = stream.ReadUInt32();
            ClockAy8910 = stream.ReadUInt32();
            Ay8910Type = stream.ReadByte();
            Ay8910Flags = stream.ReadByte();
            Ay8910Ym2203Flags = stream.ReadByte();
            Ay8910Ym2608Flags = stream.ReadByte();
        }
    }
}