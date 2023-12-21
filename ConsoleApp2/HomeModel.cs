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

namespace ConsoleApp2
{
    class HomeModel
    {
        
    }

    class DistanceCounter
    {
        static string username = "demo654";
        static APIRequest request;

        public DistanceCounter() { }

        public void CountDistatance(List<string> cities)
        {
            int[,] distanceBetweenCities = new int[cities.Count, cities.Count];
            for (int i = 0; i < cities.Count; i++)
            {
                for (int j = 0; j < cities.Count; j++)
                {
                    if (i == j)
                    {
                        distanceBetweenCities[i, j] = 0;
                        continue;
                    }
                    distanceBetweenCities[i, j] = CoordinatesRequest(cities[i], cities[j]).Result;
                }
            }
            Console.WriteLine("Output from distance matric:");

            for (int i = 0; i < cities.Count; i++)
            {
                for (int j = 0; j < cities.Count; j++)
                {
                    Console.WriteLine($"distance between {cities[i]} and {cities[j]} = {distanceBetweenCities[i, j]}");
                }
            }
        }

        static async Task<int> CoordinatesRequest(string city1, string city2)
        {
            string url1 = $"http://api.geonames.org/searchJSON?q={city1}&maxRows=1&username={username}&featureClass=P";
            string url2 = $"http://api.geonames.org/searchJSON?q={city2}&maxRows=1&username={username}&featureClass=P";

            
            string responseBody2 = Convert.ToString(request.MakeRequest(url2));
            string responseBody1 = Convert.ToString(request.MakeRequest(url1));

            JObject json1 = JObject.Parse(responseBody1);
            JObject json2 = JObject.Parse(responseBody2);
            JArray geonames1 = (JArray)json1["geonames"];
            JArray geonames2 = (JArray)json2["geonames"];
            double lat1 = (double)geonames1[0]["lat"];
            double lon1 = (double)geonames1[0]["lng"];
            double lat2 = (double)geonames2[0]["lat"];
            double lon2 = (double)geonames2[0]["lng"];
            int distance = (int)MeasureDistanceByCoordinates(lat1, lon1, lat2, lon2);

            return distance;
            
        }

        private static double MeasureDistanceByCoordinates(double lat1, double lon1, double lat2, double lon2)
        {
            double R = 6371; // Radius of the earth in km
            double dLat = Deg2Rad(lat2 - lat1);  // deg2rad below
            double dLon = Deg2Rad(lon2 - lon1);
            double a =
              Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
              Math.Cos(Deg2Rad(lat1)) * Math.Cos(Deg2Rad(lat2)) *
              Math.Sin(dLon / 2) * Math.Sin(dLon / 2)
              ;
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double d = R * c; // Distance in km
            return d;
        }

        private static double Deg2Rad(double deg)
        {
            return deg * (Math.PI / 180);
        }
    }

    class APIRequest
    {
        public APIRequest() { }

        public async Task<string> MakeRequest(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                return responseBody;
            }
        }
    }
}
