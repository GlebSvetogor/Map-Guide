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

    class APIRequest
    {
        private static string username = "demo654";

        public static async Task<JObject> MakeRequest(string city)
        {
            string url = $"http://api.geonames.org/searchJSON?q={city}&maxRows=1&username={username}&featureClass=P&featureCode=PPL&isNameRequired=true";
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                JObject json = JObject.Parse(responseBody);
                return json;
            }
        }
    }
}
