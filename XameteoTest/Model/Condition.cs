using Newtonsoft.Json;

namespace XameteoTest.Model
{
    /// <summary>
    /// </summary>
    internal class Condition
    {
        /// <summary>
        /// </summary>
        [JsonProperty("code")]
        public int Id { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty("text")]
        public string Text { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty("icon")]
        public string Icon { get; set; }
    }
}