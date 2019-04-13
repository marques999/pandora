using Newtonsoft.Json;

namespace XameteoTest.Model
{
    /// <summary>
    /// </summary>
    internal class ApixuCurrent
    {
        /// <summary>
        /// </summary>
        [JsonProperty("current")]
        public Current Current { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty("location")]
        public Location Location { get; set; }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"\r\nCurrent {{\r\n{Current}\r\n}}\r\n\r\nLocation {{\r\n{Location}\r\n}}";
    }
}