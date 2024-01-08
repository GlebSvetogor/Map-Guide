using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace ConsoleApp2
{
    internal class APIRequest
    {
        private static string geonamesUsername = "demo654";
        private static string googleCloudApiKey = $"https://maps.googleapis.com/maps/api/directions/json?origin=coordinatesFrom&destination=coordinatesTo&key=AIzaSyBGykNf1-zcVrXeSSkuYqRc01Gc02nh0Ho";
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

        public static async Task<(double,double)> MakeDefaultRequest(string coordinatesFrom, string coordinatesTo)
        {
            string url = googleCloudApiKey.Replace("coordinatesFrom", coordinatesFrom).Replace("coordinatesTo", coordinatesTo);
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(url))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    JObject jsonResponse = JObject.Parse(apiResponse);
                    JToken routes = jsonResponse["routes"];

                    JToken fullRouteLegs = routes[0]["legs"];
                    double distance = (double)fullRouteLegs[0]["distance"]["value"] / 1000;
                    double duration = (double)fullRouteLegs[0]["duration"]["value"] / 3600;

                    return (distance,duration);
                }
            }
        }
    }
}
