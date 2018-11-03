namespace VgmPlayer.Ym2612
{
    internal class Ym2612Slot
    {
        public long[] DT = new long[32];
        public byte KSR;
        public uint ar;
        public uint d1r;
        public uint d2r;
        public uint rr;
        public byte ksr;
        public uint mul;
        public uint phase;

        public long Incr;

        /* Envelope Generator */
        public byte state; /* phase type */

        public uint tl;
        public long volume;
        public uint sl;
        public uint vol_out;

        public byte eg_sh_ar;
        public byte eg_sel_ar;
        public byte eg_sh_d1r;
        public byte eg_sel_d1r;
        public byte eg_sh_d2r;
        public byte eg_sel_d2r;
        public byte eg_sh_rr;
        public byte eg_sel_rr;

        public byte ssg; /* SSG-EG waveform  */
        public byte ssgn; /* SSG-EG negated output  */

        public byte key; /* 0=last key was KEY OFF, 1=KEY ON */
        public uint AMmask; /* AM enable flag */
    };
}