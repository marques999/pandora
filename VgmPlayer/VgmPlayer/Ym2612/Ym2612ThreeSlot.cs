namespace VgmPlayer
{
    /// <summary>
    /// </summary>
    internal class Ym2612ThreeSlot
    {
        /// <summary>
        /// </summary>
        public readonly uint[] BlockFnum = new uint[3];

        /// <summary>
        /// </summary>
        public readonly uint[] Fc = new uint[3];

        /// <summary>
        /// </summary>
        public readonly byte[] KeyCode = new byte[3];

        /// <summary>
        /// </summary>
        public byte FnLatch;

        /// <summary>
        /// </summary>
        public byte KeyCsm;
    }
}