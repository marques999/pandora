using Newtonsoft.Json;

namespace XameteoTest.Model
{
    /// <summary>
    /// </summary>
    internal class Day
    {
        /// <summary>
        /// </summary>
        [JsonProperty("maxtemp_c")]
        public double Maximum { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty("mintemp_c")]
        public double Minimum { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty("avgtemp_c")]
        public double Average { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty("maxwind_kph")]
        public double WindVelocity { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty("totalprecip_mm")]
        public double Precipitation { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty("avgvis_km")]
        public double Visibility { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty("avghumidity")]
        public double Humidity { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty("condition")]
        public Condition Condition { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty("uv")]
        public double Ultraviolet { get; set; }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $@"
  Maximum = {Maximum}
  Minimum = {Minimum}
  Average = {Average}
  WindVelocity = {WindVelocity}
  Precipitation = {Precipitation}
  Visibility = {Visibility}
  Humidity = {Humidity}
  Ultraviolet = {Ultraviolet}
  Condition = {Condition}
";
    }
}