using System;
using System.Text;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace XameteoTest.Model
{
    /// <summary>
    /// </summary>
    internal class ForecastDaily
    {
        /// <summary>
        /// </summary>
        [JsonProperty("date")]
        public DateTime Date { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty("day")]
        public Day Day { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty("astro")]
        public Astrology Astro { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty("hour")]
        public List<Hourly> Hours { get; set; }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.Append("\n+============+\n| ");
            builder.Append(Date.ToShortDateString());
            builder.Append(" |");
            builder.Append("\n+============+\n");
            builder.Append("\nDay {");
            builder.Append(Day);
            builder.Append("}\n\nAstro {");
            builder.Append(Astro);
            builder.Append("}\n");

            foreach (var hour in Hours)
            {
                builder.Append("\n[");
                builder.Append(hour.Date.ToShortTimeString());
                builder.Append("] {");
                builder.Append(hour);
                builder.Append("}\n");
            }

            return builder.ToString();
        }
    }
}