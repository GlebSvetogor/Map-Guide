using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;

namespace ConsoleApp2
{
    internal class APIRequest
    {
        public string geonamesApiKey { get; set; }
        string mapApiKey { get; set; }
        //private static string googleCloudApiKey = $"https://maps.googleapis.com/maps/api/directions/json?origin=coordinatesFrom&destination=coordinatesTo&key=AIzaSyBGykNf1-zcVrXeSSkuYqRc01Gc02nh0Ho";
        //private static string geonamesUsername = "demo654";

        public async Task<bool> MakeCityRequestAsync(string url, List<City> cities)
        {
            StringBuilder notCities = new StringBuilder();

            JObject json = await MakeRequestAsync(url);

            if((string)json["status"] == "ZERO_RESULTS")
            {
                Console.WriteLine($"{cities.Last().longName} не является городом");
                return false;
            }
            else
            {
                string lat = (string)json["results"][0]["geometry"]["location"]["lat"];
                string lng = (string)json["results"][0]["geometry"]["location"]["lng"];
                string longName = (string)json["results"][0]["address_components"][0]["long_name"];
                cities.Add(new City(longName, lat,lng));
                Console.WriteLine($"{cities.Last().longName} является городом");
                return true;
            }

        }

        public async Task<double> MakeDistanceRequestAsync(string url) 
        {
            JObject jsonResponse = await MakeRequestAsync(url);
            JToken routes = jsonResponse["routes"];

            JToken fullRouteLegs = routes[0]["legs"];
            double distance = (double)fullRouteLegs[0]["distance"]["value"] / 1000;
            return distance;        
        }

        public async Task<double> MakeDurationRequestAsync(string url)
        {
            JObject jsonResponse = await MakeRequestAsync(url);
            JToken routes = jsonResponse["routes"];

            JToken fullRouteLegs = routes[0]["legs"];
            double duration = (double)fullRouteLegs[0]["duration"]["value"] / 3600;
            return duration;
        }

        public async Task<JObject> MakeRequestAsync(string url)
        {
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(url))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    JObject jsonResponse = JObject.Parse(apiResponse);
                    return jsonResponse;
                }
            }
        }

        //public static async Task<(double,double)> MakeDistanceAndDurationRequestAsync(string coordinatesFrom, string coordinatesTo)
        //{
        //    string url = googleCloudApiKey.Replace("coordinatesFrom", coordinatesFrom).Replace("coordinatesTo", coordinatesTo);
        //    using (var httpClient = new HttpClient())
        //    {
        //        using (var response = await httpClient.GetAsync(url))
        //        {
        //            string apiResponse = await response.Content.ReadAsStringAsync();
        //            JObject jsonResponse = JObject.Parse(apiResponse);
        //            JToken routes = jsonResponse["routes"];

        //            JToken fullRouteLegs = routes[0]["legs"];
        //            double distance = (double)fullRouteLegs[0]["distance"]["value"] / 1000;
        //            double duration = (double)fullRouteLegs[0]["duration"]["value"] / 3600;

        //            return (distance,duration);
        //        }
        //    }
        //}
    }
}
