namespace VgmPlayer.Sn76489
{
    /// <summary>
    /// </summary>
    public class Sn76489
    {
        /// <summary>
        /// </summary>
        private readonly Sn76489Core _sn76489 = new Sn76489Core();

        /// <summary>
        /// </summary>
        /// <param name="clock"></param>
        public void Initialize(double clock)
        {
            _sn76489.SetClock((float)clock);
        }

        /// <summary>
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="length"></param>
        public void Update(int[] buffer, int length)
        {
            for (var index = 0; index < length; index++)
            {
                var offset = index << 1;
                var value = (short)(_sn76489.Render() * 8000);

                buffer[offset++] = value;
                buffer[offset] = value;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="value"></param>
        public void Write(int value)
        {
            _sn76489.Write(value);
        }
    }
}