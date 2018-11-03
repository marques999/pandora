namespace XameteoTest.API
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    internal class ApixuByAirport : Apixu
    {
        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="iataCode"></param>
        public ApixuByAirport(string iataCode) : base("iata:" + iataCode)
        {
        }
    }
}