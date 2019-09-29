using System.IO;
using System.Net;
using System.Threading.Tasks;

using Newtonsoft.Json;

using XameteoTest.Model;

namespace XameteoTest.API
{
    /// <summary>
    /// </summary>
    internal class Apixu
    {
        /// <summary>
        /// </summary>
        private const string ApiUrl = "http://api.apixu.com/v1";

        /// <summary>
        /// </summary>
        private const string ApiKey = "7e4b1cf5c63a4a2e876183000173011";

        /// <summary>
        /// </summary>
        private readonly string _parameters;

        /// <summary>
        /// </summary>
        /// <param name="parameters"></param>
        public Apixu(string parameters)
        {
            _parameters = parameters;
        }

        /// <summary>
        /// </summary>
        /// <param name="days"></param>
        /// <returns></returns>
        public Task<ApixuForecast> GetForecast(int days)
        {
            return HttpGetAsync<ApixuForecast>($"{ApiUrl}/forecast.json?key={ApiKey}&q={_parameters}&days={days}");
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uri"></param>
        /// <returns></returns>
        private static async Task<T> HttpGetAsync<T>(string uri)
        {
            if (!(WebRequest.Create(uri) is HttpWebRequest httpWebRequest))
            {
                return default;
            }

            httpWebRequest.Method = "GET";
            httpWebRequest.ContentType = "application/json";

            using var response = await httpWebRequest.GetResponseAsync();

            if (!(response is HttpWebResponse httpResponse))
            {
                return default;
            }

            var statusCode = httpResponse.StatusCode;

            if (statusCode != HttpStatusCode.OK)
            {
                return default;
            }

            var buffer = response.GetResponseStream();

            if (buffer == null)
            {
                return default;
            }

            return JsonConvert.DeserializeObject<T>(new StreamReader(buffer).ReadToEnd());
        }
    }
}