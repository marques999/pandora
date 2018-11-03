namespace VgmPlayer.Ym2612
{
    /// <summary>
    /// </summary>
    internal class Ym2612Opn
    {
        /// <summary>
        /// </summary>
        public readonly uint[] FnTable = new uint[4096];

        /// <summary>
        /// </summary>
        public readonly uint[] Pan = new uint[6 * 2];

        /// <summary>
        /// </summary>
        public readonly Ym2612ThreeSlot Sl3 = new Ym2612ThreeSlot();

        /// <summary>
        /// </summary>
        public readonly Ym2612St St = new Ym2612St();

        /// <summary>
        /// </summary>
        public uint EgCounter;

        /// <summary>
        /// </summary>
        public uint EgTimer;

        /// <summary>
        /// </summary>
        public uint EgTimerAdd;

        /// <summary>
        /// </summary>
        public uint EgTimerOverflow;

        /// <summary>
        /// </summary>
        public uint FnMax;

        /// <summary>
        /// </summary>
        public uint LfoAm;

        /// <summary>
        /// </summary>
        public byte LfoCount;

        /// <summary>
        /// </summary>
        public uint LfoPm;

        /// <summary>
        /// </summary>
        public uint LfoTimer;

        /// <summary>
        /// </summary>
        public uint LfoTimerOverflow;

        /// <summary>
        /// </summary>
        public uint LfoTimerStep;
    }
}