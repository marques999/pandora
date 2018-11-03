namespace VgmPlayer.Ym2612
{
    internal class Ym2612Channel
    {
        public readonly long[] Op1Output = new long[2];

        public readonly Ym2612Slot[] Slot =
        {
            new Ym2612Slot(),
            new Ym2612Slot(),
            new Ym2612Slot(),
            new Ym2612Slot()
        };

        public LongPointer Connect1;
        public LongPointer Connect2;
        public LongPointer Connect3;
        public LongPointer Connect4;
        public LongPointer MemoryConnect;

        public byte Ams;
        public byte KeyCode;
        public byte Feedback;
        public byte Algorithm;

        public uint Fc;
        public uint BlockFnum;

        public long Pms;
        public long MemoryValue;
    }
}