using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

namespace XameteoTest.Model
{
    /// <summary>
    /// </summary>
    internal class Forecast
    {
        /// <summary>
        /// </summary>
        [JsonProperty("forecastday")]
        public List<ForecastDaily> Days { get; set; }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public override string ToString() => string.Join("\r\n", Days.SelectMany(day => day.ToString()));
    }
}