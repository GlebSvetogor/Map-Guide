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
        static bool inputCities = false;

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
                            foreach (string city in cities)
                            {
                                Console.WriteLine(city);
                            }

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
            string username = "demo654";
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
                string adminCode1 = (string)geonames[0]["adminCode1"];

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

        private static Task Error(ITelegramBotClient client, Exception exception, CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }

}
        
            
