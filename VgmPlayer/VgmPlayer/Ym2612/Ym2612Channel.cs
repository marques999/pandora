namespace VgmPlayer.Ym2612
{
    /// <summary>
    /// </summary>
    internal class Ym2612Channel
    {
        /// <summary>
        /// </summary>
        public readonly long[] Op1Output = new long[2];

        /// <summary>
        /// </summary>
        public readonly Ym2612Slot[] Slot =
        {
            new Ym2612Slot(),
            new Ym2612Slot(),
            new Ym2612Slot(),
            new Ym2612Slot()
        };

        /// <summary>
        /// </summary>
        public byte Algorithm;

        /// <summary>
        /// </summary>
        public byte Ams;

        /// <summary>
        /// </summary>
        public uint BlockFnum;

        /// <summary>
        /// </summary>
        public LongPointer Connect1;

        /// <summary>
        /// </summary>
        public LongPointer Connect2;

        /// <summary>
        /// </summary>
        public LongPointer Connect3;

        /// <summary>
        /// </summary>
        public LongPointer Connect4;

        /// <summary>
        /// </summary>
        public uint Fc;

        /// <summary>
        /// </summary>
        public byte Feedback;

        /// <summary>
        /// </summary>
        public byte KeyCode;

        /// <summary>
        /// </summary>
        public LongPointer MemoryConnect;

        /// <summary>
        /// </summary>
        public long MemoryValue;

        /// <summary>
        /// </summary>
        public long Pms;
    }
}