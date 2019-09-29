using System;
using System.Net;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using XameteoTest.API;
using XameteoTest.IPMA;

namespace XameteoTest
{
    /// <summary>
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// </summary>
        private static IEnumerable<IpmaLocation> _locations;

        /// <summary>
        /// </summary>
        private const string GeocodingKey = "AIzaSyC7BMV163HG2n8_Wo4Esn5VEjCxLkaSbmc";

        /// <summary>
        /// </summary>
        private const string GeocodingApi = "https://maps.googleapis.com/maps/api/geocode/json";

        /// <summary>
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        private static IpmaLocation FindClosest(double latitude, double longitude)
        {
            return _locations.MinimumBy(location => location.CalculateDistance(latitude, longitude));
        }

        /// <summary>
        /// </summary>
        private static void TestGeocoding(HttpClient httpClient, string address)
        {
            httpClient.GetAsync(QueryGeocoding(address)).ContinueWith(taskWithResponse =>
            {
                using var jsonData = taskWithResponse.Result.Content.ReadAsStringAsync();

                jsonData.Wait();

                var geocoding = JObject.Parse(jsonData.Result)["results"][0]["geometry"]["location"];

                if (geocoding.HasValues == false)
                {
                    return;
                }

                var latitude = geocoding["lat"].ToObject<double>();
                var longitude = geocoding["lng"].ToObject<double>();

                TestForecast(httpClient, FindClosest(latitude, longitude).Id);
                Console.WriteLine($"\nUserQuery = {address}");
                Console.WriteLine($"\nFindClosest({latitude}, {longitude}) = " + FindClosest(latitude, longitude).Nome);
            }).Wait();
        }

        /// <summary>
        /// </summary>
        public static void TestDistricts(HttpClient httpClient)
        {
            httpClient.GetAsync("http://api.ipma.pt/json/districts.json").ContinueWith(HandleDistricts).Wait();
        }

        /// <summary>
        /// </summary>
        /// <param name="response"></param>
        private static void HandleDistricts(Task<HttpResponseMessage> response)
        {
            using var jsonData = response.Result.Content.ReadAsStringAsync();

            jsonData.Wait();

            foreach (var district in JsonConvert.DeserializeObject<List<IpmaDistrict>>(jsonData.Result))
            {
                Console.WriteLine($@"
idRegiao: {district.Regiao}
idDistrito: {district.Id}
nome: {district.Nome}");
            }
        }

        /// <summary>
        /// </summary>
        public static void TestForecast(HttpClient httpClient, int idLocal)
        {
            httpClient.GetAsync($"http://api.ipma.pt/json/alldata/{idLocal}.json").ContinueWith(HandleForecast).Wait();
        }

        /// <summary>
        /// </summary>
        /// <param name="response"></param>
        private static void HandleForecast(Task<HttpResponseMessage> response)
        {
            using var jsonData = response.Result.Content.ReadAsStringAsync();

            jsonData.Wait();

            foreach (var forecast in JsonConvert.DeserializeObject<List<IpmaForecast>>(jsonData.Result))
            {
                Console.WriteLine($@"
idTipoTempo: {forecast.IdTempo}
probabilidadePrecipita: {forecast.ProbabilidadePrecipitacao}
tMax: {forecast.TemperaturaMaxima}
utci: {forecast.Temperatura}
ddVento: {forecast.DireccaoVento}
tMed: {forecast.TemperaturaMedia}
tMin: {forecast.TemperaturaMinima}
hR: {forecast.PressaoAtmosferica}
dataUpdate: {forecast.DataAtualizacao}
ffVento: {forecast.VelocidadeVento}
idIntensidadePrecipita: {forecast.IdPrecipitacao}
globalIdLocal: {forecast.IdLocal}
idPeriodo: {forecast.Periodo}
dataPrev: {forecast.DataPrevisao}
idFfxVento: {forecast.IdVento}");
            }
        }

        /// <summary>
        /// </summary>
        public static void TestLocations(HttpClient httpClient)
        {
            httpClient.GetAsync("http://api.ipma.pt/json/locations.json").ContinueWith(HandleLocations).Wait();
        }

        /// <summary>
        /// </summary>
        /// <param name="response"></param>
        private static void HandleLocations(Task<HttpResponseMessage> response)
        {
            using var jsonData = response.Result.Content.ReadAsStringAsync();

            jsonData.Wait();

            foreach (var forecast in JsonConvert.DeserializeObject<List<IpmaLocation>>(jsonData.Result))
            {
                Console.WriteLine($@"
idRegiao: {forecast.Regiao}
idAreaAviso: {forecast.AreaAviso}
idConcelho: {forecast.Concelho}
globalIdLocal: {forecast.Id}
latitude: {forecast.Latitude}
idDistrito: {forecast.Distrito}
local: {forecast.Nome}
longitude: {forecast.Longitude}");
            }
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        private static void TestCoordinates()
        {
            Console.WriteLine("\n+===============================================================+");
            Console.WriteLine("| Begin Coordinates Test                                        |");
            Console.WriteLine("+===============================================================+");
            Console.WriteLine("\nFindClosest(41.209366, -8.1763036) = " + FindClosest(41.209366, -8.1763036).Nome);
            Console.WriteLine("\n+===============================================================+");
            Console.WriteLine("| Coordinates Test Finished                                     |");
            Console.WriteLine("+===============================================================+");
        }

        /// <summary>
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        private static string QueryGeocoding(string address)
        {
            return $"{GeocodingApi}?address={WebUtility.UrlEncode(address)}&key={GeocodingKey}";
        }

        /// <summary>
        /// </summary>
        private static async void AsyncRequest()
        {
            try
            {
                Console.WriteLine(await new ApixuByCity("Valongo, Porto").GetForecast(1));
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="response"></param>
        private static void HandleInitial(Task<HttpResponseMessage> response)
        {
            using var jsonData = response.Result.Content.ReadAsStringAsync();

            jsonData.Wait();
            _locations = JsonConvert.DeserializeObject<List<IpmaLocation>>(jsonData.Result);
        }

        /// <summary>
        /// </summary>
        private static void Main()
        {
            using var httpClient = new HttpClient();

            httpClient.GetAsync("http://api.ipma.pt/json/locations.json").ContinueWith(HandleInitial).Wait();
            TestDistricts(httpClient);
            Console.ReadKey(true);
            TestLocations(httpClient);
            Console.ReadKey(true);
            TestCoordinates();
            Console.ReadKey(true);
            TestForecast(httpClient, 1131500);
            Console.ReadKey(true);
            Console.WriteLine("\n+===============================================================+");
            Console.WriteLine("| Begin Geocoding Test                                          |");
            Console.WriteLine("+===============================================================+");
            TestGeocoding(httpClient, "R. Lugar Novo 109, Constance");
            TestGeocoding(httpClient, "R. Costa 176, Campo");
            TestGeocoding(httpClient, "Trancoso, Guarda");
            Console.WriteLine("\n+===============================================================+");
            Console.WriteLine("| Geocoding Test Finished                                       |");
            Console.WriteLine("+===============================================================+");
            Console.ReadKey(true);
            AsyncRequest();
            Console.ReadKey(true);
        }
    }
}