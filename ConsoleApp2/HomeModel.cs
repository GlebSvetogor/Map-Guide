using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Reflection.PortableExecutable;
using Telegram.Bot.Requests.Abstractions;

namespace ConsoleApp2
{
    class HomeModel
    {

    }

    public class Point
    {
        public string type { get; set; }
        public double lon { get; set; }
        public double lat { get; set; }
    }

    public class Request
    {
        public List<Point> points { get; set; }
        public string locale { get; set; }
        public string transport { get; set; }
        public string route_mode { get; set; }
        public string traffic_mode { get; set; }
    }

    class APIRequest
    {
        private static string geonamesUsername = "demo654";
        private static string twoGisApiKey = "00419301-8df8-46e6-83f1-83cd25e7a8c1";

        public static async Task<JObject> MakeCityRequest(string city)
        {
            string url = $"http://api.geonames.org/searchJSON?q={city}&maxRows=1&username={geonamesUsername}&featureClass=P&featureCode=PPL&isNameRequired=true";
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                JObject json = JObject.Parse(responseBody);
                return json;
            }
        }

        public static async Task<string> MakeCoordinateRequest(string city)
        {
            string url = $"https://catalog.api.2gis.com/3.0/items?&q={city}&fields=items.geometry.centroid&key={twoGisApiKey}"; // URL-адрес запроса
            string regex = @"POINT\((\d+\.\d+) (\d+\.\d+)\)";
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                JObject json = JObject.Parse(responseBody);
                JArray items = (JArray)(json["result"]["items"]);
                string coordinates = (string)items[0]["geometry"]["centroid"];
                Console.WriteLine($"coordinates = {coordinates}");

                Match match = Regex.Match(coordinates, regex);

                if (match.Success)
                {
                    string matchedCoordinates = match.Groups[1].Value + " " + match.Groups[2].Value;
                    return matchedCoordinates;
                }
                else
                {
                    return null;
                }
            }
        }

        public static async Task<double> MakeDistanceRequest(string coordinatesFrom, string coordinatesTo)
        {
            string url = $"http://router.project-osrm.org/route/v1/driving/{coordinatesFrom};{coordinatesTo}?overview=false&annotations=distance";
            
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(url))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    JObject jsonResponse = JObject.Parse(apiResponse);
                    double distance = (double)jsonResponse["routes"][0]["distance"] / 1000;
                    Console.WriteLine($"Общее расстояние на прохождение маршрута: **{distance:N2} км**.");
                    return distance;
                }
            }
        }

        public static async Task<double> MakeDurationRequest(string coordinatesFrom, string coordinatesTo)
        {
            string url = $"http://router.project-osrm.org/route/v1/driving/{coordinatesFrom};{coordinatesTo}?overview=false&annotations=duration";

            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(url))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    JObject jsonResponse = JObject.Parse(apiResponse);
                    double duration = (double)jsonResponse["routes"][0]["duration"] / 3600;
                    Console.WriteLine($"Общее время на прохождение маршрута: **{duration:N2}");
                    return duration;
                }
            }
        }
    }
    
}
