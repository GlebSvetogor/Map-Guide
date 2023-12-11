using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ConsoleApp2
{
    internal class Program
    {
        static List<string> userInput = new List<string>();
        static List<string> cities = new List<string>();
        static List<int> indexPath = new List<int>();
        static List<string> path = new List<string>();
        static bool inputCities = false;
        static string username = "demo654";

        static void Main(string[] args)
        {
            var client = new TelegramBotClient("6414840375:AAHV2TVGdbGYuRZVgsKs3xkwplEsw1NANLQ");
            client.StartReceiving(Update, Error);
            Console.ReadLine();
 
        }

        async static Task Update(ITelegramBotClient botClient, Update update, CancellationToken token)
        {

            var message = update.Message;
            var chatId = message.Chat.Id;
            Console.WriteLine(message.Text);
            if(message.Text != null)
            {
                switch (message.Text.ToLower())
                {
                    case "/start":
                        inputCities = true;
                        await botClient.SendTextMessageAsync(chatId, "Введите название городов через запятую:");
                        break;
                    case "/help":
                        await botClient.SendTextMessageAsync(chatId, "Я могу помочь тебе найти оптимальный маршрут:" +
                            "\n\n1. Введи команду /start чтобы начать работу." +
                            "\n2. Напиши название каждого города." +
                            "\n3. После того как ты перечислил все города,напиши команду /putRequires и выбери нужные тебе требования" +
                            "\n4. Нажми на кнопку найти маршрут");
                        break;
                    default:
                        if (inputCities)
                        {
                            userInput.AddRange(message.Text.Trim().Split(','));
                            inputCities = false;
                            foreach (string el in userInput)
                            {
                                await IsCityAsync(el, botClient, chatId);
                            }

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
                                    distanceBetweenCities[i,j] = await DistanceBetweenCities(cities[i], cities[j], botClient,chatId);

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

                            int minCost = MinimumSpanningTree(distanceBetweenCities);

                            for (int i = 0; i < indexPath.Count; i++)
                            {
                                Console.WriteLine(indexPath[i]);
                            }
                            Console.WriteLine($"mincost = {minCost}");
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(chatId, "Привет! Я твой телеграм бот Map-Guid." +
                                "\n\n1.Команда /help - для получения инструкции" +
                                "\n2. Команда /start - чтобы начать работу.");
                        }
                        break;
                }
       
            }
        }

        private static async Task IsCityAsync(string city, ITelegramBotClient botClient, long chatId)
        {
            string url = $"http://api.geonames.org/searchJSON?q={city}&maxRows=1&username={username}&featureClass=P";
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync($"http://api.geonames.org/searchJSON?q={city}&maxRows=1&username={username}");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseBody);
                JObject json = JObject.Parse(responseBody);
                int totalResultsCount = (int)json["totalResultsCount"];

                JArray geonames = (JArray)json["geonames"];
                string adminCode1 = geonames.Count() == 0 ? "00" : (string)geonames[0]["adminCode1"];

                if (totalResultsCount > 0 && adminCode1 != "00")
                {
                    Console.WriteLine($"{city} является городом.");
                    cities.Add(city);
                }
                else
                {
                    Console.WriteLine($"{city} не является городом.");
                    await botClient.SendTextMessageAsync(chatId, $"{city} не является городом.");
                }
            }
        }
        private static async Task<int> DistanceBetweenCities(string city1, string city2, ITelegramBotClient botClient, long chatId)
        {
            string url1 = $"http://api.geonames.org/searchJSON?q={city1}&maxRows=1&username={username}&featureClass=P";
            string url2 = $"http://api.geonames.org/searchJSON?q={city2}&maxRows=1&username={username}&featureClass=P";

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response1 = await client.GetAsync(url1);
                HttpResponseMessage response2 = await client.GetAsync(url2);
                response1.EnsureSuccessStatusCode();
                response2.EnsureSuccessStatusCode();
                string responseBody1 = await response1.Content.ReadAsStringAsync();
                string responseBody2 = await response2.Content.ReadAsStringAsync();

                JObject json1 = JObject.Parse(responseBody1);
                JObject json2 = JObject.Parse(responseBody2);
                JArray geonames1 = (JArray)json1["geonames"];
                JArray geonames2 = (JArray)json2["geonames"];
                double lat1 = (double)geonames1[0]["lat"];
                double lon1 = (double)geonames1[0]["lng"];
                double lat2 = (double)geonames2[0]["lat"];
                double lon2 = (double)geonames2[0]["lng"];
                int distance = (int)Distance(lat1, lon1, lat2, lon2);

                return distance;
            }

        }

        private static double Distance(double lat1, double lon1, double lat2, double lon2)
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

        public static int MinimumSpanningTree(int[,] graph)
        {
            int n = graph.GetLength(0);
            int[] parent = new int[n];
            int[] key = new int[n];
            bool[] mstSet = new bool[n];

            for (int i = 0; i < n; i++)
            {
                key[i] = int.MaxValue;
                mstSet[i] = false;
            }

            key[0] = 0;
            parent[0] = -1;

            for (int count = 0; count < n - 1; count++)
            {
                int u = MinKey(key, mstSet);
                mstSet[u] = true;

                for (int v = 0; v < n; v++)
                {
                    if (graph[u, v] != 0 && mstSet[v] == false && graph[u, v] < key[v])
                    {
                        parent[v] = u;
                        key[v] = graph[u, v];
                    }
                }
            }

            

            int cost = 0;
            for (int i = 1; i < n; i++)
            {
                indexPath.Add(parent[i]);
                cost += graph[i, parent[i]];
            }
            indexPath.Add(0);
            cost += graph[0, parent[0]];
            Console.WriteLine($"cost = {cost}");
            for(int i = 1; i < indexPath.Count; i++)
            {
                Console.WriteLine(indexPath[i]);
            }

            return cost;
        }

        private static int MinKey(int[] key, bool[] mstSet)
        {
            int min = int.MaxValue;
            int minIndex = -1;

            for (int i = 0; i < key.Length; i++)
            {
                if (mstSet[i] == false && key[i] < min)
                {
                    min = key[i];
                    minIndex = i;
                }
            }

            return minIndex;
        }

        private static Task Error(ITelegramBotClient client, Exception exception, CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }

}
        
            
