namespace VgmPlayer.Ym2612
{
    /// <summary>
    /// </summary>
    internal class Ym2612
    {
        /// <summary>
        /// </summary>
        private readonly Ym2612Core _ym2612 = new Ym2612Core();

        /// <summary>
        /// </summary>
        /// <param name="clock"></param>
        /// <param name="rate"></param>
        public void Initialize(double clock, int rate)
        {
            _ym2612.Initialize(clock, rate);
            _ym2612.Reset();
        }

        /// <summary>
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="length"></param>
        public void Update(int[] buffer, int length)
        {
            _ym2612.Update(buffer, length);
        }

        /// <summary>
        /// </summary>
        /// <param name="address"></param>
        /// <param name="value"></param>
        public void WritePort0(int address, int value)
        {
            _ym2612.Write(0, (uint)address);
            _ym2612.Write(1, (uint)value);
        }

        /// <summary>
        /// </summary>
        /// <param name="address"></param>
        /// <param name="value"></param>
        public void WritePort1(int address, int value)
        {
            _ym2612.Write(2, (uint)address);
            _ym2612.Write(3, (uint)value);
        }
    }
}