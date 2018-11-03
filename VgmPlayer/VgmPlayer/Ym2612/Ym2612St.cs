namespace VgmPlayer.Ym2612
{
    /// <summary>
    /// </summary>
    internal class Ym2612St
    {
        /// <summary>
        /// </summary>
        public readonly long[][] DetuneTable =
        {
            new long[32],
            new long[32],
            new long[32],
            new long[32],
            new long[32],
            new long[32],
            new long[32],
            new long[32],
        };

        /// <summary>
        /// </summary>
        public ushort Address;

        /// <summary>
        /// </summary>
        public double Clock;

        /// <summary>
        /// </summary>
        public byte FnH;

        /// <summary>
        /// </summary>
        public uint Mode;

        /// <summary>
        /// </summary>
        public uint Rate;

        /// <summary>
        /// </summary>
        public byte Status;

        /// <summary>
        /// </summary>
        public long Ta;

        /// <summary>
        /// </summary>
        public long Tac;

        /// <summary>
        /// </summary>
        public long Tal;

        /// <summary>
        /// </summary>
        public long Tb;

        /// <summary>
        /// </summary>
        public long Tbc;

        /// <summary>
        /// </summary>
        public long Tbl;

        /// <summary>
        /// </summary>
        public long TimerBase;
    }
}