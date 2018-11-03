using Newtonsoft.Json;

namespace XameteoTest.Model
{
    /// <summary>
    /// </summary>
    internal class ApixuForecast
    {
        /// <summary>
        /// </summary>
        [JsonProperty("current")]
        public Current Current { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty("forecast")]
        public Forecast Forecast { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty("location")]
        public Location Location { get; set; }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"\r\nCurrent {{{Current}}}\r\n\r\nLocation {{{Location}}}\r\n            \r\n{Forecast}";
    }
}