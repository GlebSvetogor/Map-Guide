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

namespace ConsoleApp2
{
    internal class Program
    {
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
                            cities.AddRange(message.Text.Trim().Split(','));
                            inputCities = false;
                            await botClient.SendTextMessageAsync(chatId, "Ты ввел следующие города:");
                            foreach (string city in cities)
                            {
                                await botClient.SendTextMessageAsync(chatId, city);
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

        private static async Task IsCityAsync(string city)
        {

            using (var client = new HttpClient())
            {
                var response = await client.GetAsync($"http://api.geonames.org/searchJSON?q={city}&maxRows=1&username=demo");
                var result = ReadContentAsStringAsync(response);
                if (GetTotalResultCountAsync(result) > 0)
                {
                    Console.WriteLine($"{city} является городом.");
                }
                else
                {
                    Console.WriteLine($"{city} не является городом.");
                }
            }
        }

        public static async Task<int> GetTotalResultCountAsync(HttpResponseMessage response)
        {
            string content = await response.Content.ReadAsStringAsync();
            GeoNamesResponse data = JsonConvert.DeserializeObject<GeoNamesResponse>(content);
            return data.totalResultsCount;
        }

        public static async Task<string> ReadContentAsStringAsync(HttpResponseMessage response)
        {
            return await response.Content.ReadAsStringAsync();
        }

        private static Task Error(ITelegramBotClient client, Exception exception, CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }

    public class GeoNamesResponse
    {
        public int totalResultsCount { get; set; }
    }
}
        
            
