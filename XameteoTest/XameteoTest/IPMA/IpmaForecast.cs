using System;
using Newtonsoft.Json;

namespace XameteoTest.IPMA
{
    /// <summary>
    /// </summary>
    internal class IpmaForecast
    {
        /// <summary>
        /// </summary>
        [JsonProperty("globalIdLocal")]
        public int IdLocal { get; private set; }

        /// <summary>
        /// </summary>
        [JsonProperty("idTipoTempo")]
        public int IdTempo { get; private set; }

        /// <summary>
        /// </summary>
        [JsonProperty("idFfxVento")]
        public int IdVento { get; private set; }

        /// <summary>
        /// </summary>
        [JsonProperty("idIntensidadePrecipita")]
        public int IdPrecipitacao { get; private set; }

        /// <summary>
        /// </summary>
        [JsonProperty("idPeriodo")]
        public int Periodo { get; private set; }

        /// <summary>
        /// </summary>
        [JsonProperty("ddVento")]
        public string DireccaoVento { get; private set; }

        /// <summary>
        /// </summary>
        [JsonProperty("ffVento")]
        public double VelocidadeVento { get; private set; }

        /// <summary>
        /// </summary>
        [JsonProperty("utci")]
        public double Temperatura { get; private set; }

        /// <summary>
        /// </summary>
        [JsonProperty("tMin")]
        public double TemperaturaMinima { get; private set; }

        /// <summary>
        /// </summary>
        [JsonProperty("tMed")]
        public double TemperaturaMedia { get; private set; }

        /// <summary>
        /// </summary>
        [JsonProperty("tMax")]
        public double TemperaturaMaxima { get; private set; }

        /// <summary>
        /// </summary>
        [JsonProperty("hR")]
        public double PressaoAtmosferica { get; private set; }

        /// <summary>
        /// </summary>
        [JsonProperty("probabilidadePrecipita")]
        public int ProbabilidadePrecipitacao { get; private set; }

        /// <summary>
        /// </summary>
        [JsonProperty("dataPrev")]
        public DateTime DataPrevisao { get; private set; }

        /// <summary>
        /// </summary>
        [JsonProperty("dataUpdate")]
        public DateTime DataAtualizacao { get; private set; }
    }
}