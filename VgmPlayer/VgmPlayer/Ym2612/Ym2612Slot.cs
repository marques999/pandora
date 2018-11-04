namespace VgmPlayer.Ym2612
{
    internal class Ym2612Slot
    {
        public uint AMmask;
        public uint Ar;
        public uint DecayRate;
        public long[] Detune = new long[32];
    
        public byte eg_sel_ar;
        public byte eg_sel_d1r;
        public byte eg_sel_d2r;
        public byte eg_sel_rr;
        public byte eg_sh_ar;
        public byte eg_sh_d1r;
        public byte eg_sh_d2r;
        public byte eg_sh_rr;

        public long Increment;
        public byte Key;
        public byte ksr;
        public byte KSR;
        public uint Multiply;
        public uint Phase;
        public uint ReleaseRate;
        public uint Sl;
        public byte Ssg;
        public byte SsgN;
        public byte State;
        public uint SustainRate;
        public uint Tl;
        public long Volume;
        public uint VolumeFinal;
    }
}