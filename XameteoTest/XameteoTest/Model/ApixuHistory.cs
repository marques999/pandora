using Newtonsoft.Json;

namespace XameteoTest.Model
{
    /// <summary>
    /// </summary>
    internal class ApixuHistory
    {
        /// <summary>
        /// </summary>
        [JsonProperty("location")]
        public Location Location { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty("forecast")]
        public Forecast Forecast { get; set; }
    }
}