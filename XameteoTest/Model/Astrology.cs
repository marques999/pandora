using System;
using Newtonsoft.Json;

namespace XameteoTest.Model
{
    /// <summary>
    /// </summary>
    internal class Astrology
    {
        /// <summary>
        /// </summary>
        [JsonProperty("sunrise")]
        public DateTime Sunrise { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty("sunset")]
        public DateTime Sunset { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty("moonrise")]
        public DateTime Moonrise { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty("moonset")]
        public DateTime Moonset { get; set; }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"\r\n  Sunrise = {Sunrise}\r\n  Sunset = {Sunset}\r\n  Moonrise = {Moonrise}\r\n  Moonset = {Moonset}\r\n";
    }
}