using System;
using System.Net;
using System.Net.Http;
using System.Collections.Generic;

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
        private static IpmaLocation FindClosest(double latitude, double longitude) => _locations.MinimumBy(location => location.CalculateDistance(latitude, longitude));

        /// <summary>
        /// </summary>
        private static void TestGeocoding(string address)
        {
            new HttpClient().GetAsync(QueryGeocoding(address)).ContinueWith(taskWithResponse =>
            {
                using (var jsonResponse = taskWithResponse.Result.Content.ReadAsStringAsync())
                {
                    jsonResponse.Wait();

                    var geocoding = JObject.Parse(jsonResponse.Result)["results"][0]["geometry"]["location"];

                    if (geocoding.HasValues == false)
                    {
                        return;
                    }

                    var latitude = geocoding["lat"].ToObject<double>();
                    var longitude = geocoding["lng"].ToObject<double>();

                    TestForecast(FindClosest(latitude, longitude).Id);
                    Console.WriteLine($"\nUserQuery = {address}");
                    Console.WriteLine($"\nFindClosest({latitude}, {longitude}) = " + FindClosest(latitude, longitude).Nome);
                }
            }).Wait();
        }

        /// <summary>
        /// </summary>
        public static void TestDistricts()
        {
            new HttpClient().GetAsync("http://api.ipma.pt/json/districts.json").ContinueWith(taskWithResponse =>
            {
                using (var jsonResponse = taskWithResponse.Result.Content.ReadAsStringAsync())
                {
                    jsonResponse.Wait();

                    foreach (var district in JsonConvert.DeserializeObject<List<IpmaDistrict>>(jsonResponse.Result))
                    {
                        Console.WriteLine($@"
idRegiao: {district.Regiao}
idDistrito: {district.Id}
nome: {district.Nome}");
                    }
                }
            }).Wait();
        }

        /// <summary>
        /// </summary>
        public static void TestForecast(int idLocal)
        {
            new HttpClient().GetAsync($"http://api.ipma.pt/json/alldata/{idLocal}.json").ContinueWith(taskWithResponse =>
            {
                using (var jsonResponse = taskWithResponse.Result.Content.ReadAsStringAsync())
                {
                    jsonResponse.Wait();

                    foreach (var forecast in JsonConvert.DeserializeObject<List<IpmaForecast>>(jsonResponse.Result))
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
            }).Wait();
        }

        /// <summary>
        /// </summary>
        public static void TestLocations()
        {
            new HttpClient().GetAsync("http://api.ipma.pt/json/locations.json").ContinueWith(taskWithResponse =>
            {
                using (var jsonResponse = taskWithResponse.Result.Content.ReadAsStringAsync())
                {
                    jsonResponse.Wait();

                    foreach (var forecast in JsonConvert.DeserializeObject<List<IpmaLocation>>(jsonResponse.Result))
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
            }).Wait();
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
        private static void Main()
        {
            new HttpClient().GetAsync("http://api.ipma.pt/json/locations.json").ContinueWith(taskWithResponse =>
            {
                using (var jsonResponse = taskWithResponse.Result.Content.ReadAsStringAsync())
                {
                    jsonResponse.Wait();
                    _locations = JsonConvert.DeserializeObject<List<IpmaLocation>>(jsonResponse.Result);
                }
            }).Wait();

            TestDistricts();
            Console.ReadKey(true);
            TestLocations();
            Console.ReadKey(true);
            TestCoordinates();
            Console.ReadKey(true);
            Console.WriteLine("\n+===============================================================+");
            Console.WriteLine("| Begin Geocoding Test                                          |");
            Console.WriteLine("+===============================================================+");
            TestGeocoding("R. Lugar Novo 109, Constance");
            TestGeocoding("R. Costa 176, Campo");
            TestGeocoding("Trancoso, Guarda");
            Console.WriteLine("\n+===============================================================+");
            Console.WriteLine("| Geocoding Test Finished                                       |");
            Console.WriteLine("+===============================================================+");
            Console.ReadKey(true);
            TestForecast(1131500);
            Console.ReadKey(true);
            AsyncRequest();
            Console.ReadKey(true);
        }
    }
}