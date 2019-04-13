namespace XameteoTest.API
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    internal class ApixuByCoordinates : Apixu
    {
        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        public ApixuByCoordinates(double latitude, double longitude) : base($"{latitude},{longitude}")
        {
        }
    }
}