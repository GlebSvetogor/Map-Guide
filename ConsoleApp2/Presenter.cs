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

    internal class Presenter
    {
        static Model model = new Model();
        static View view = new View();
        static bool inputCities = false;
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;
            view.client.StartReceiving(Update, Error);
            Console.ReadLine();
        }

        async static Task Update(ITelegramBotClient botClient, Update update, CancellationToken token)
        {
            var message = update.Message;
            var chatId = message.Chat.Id;
            switch (message.Text.ToLower())
            {
                case "/start":
                    inputCities = true;
                    await view.SendMessage(chatId, "Введите название городов через запятую:");
                    break;
                case "/help":
                    await view.SendMessage(chatId, "Я могу помочь тебе найти оптимальный маршрут: " +
                         "\n\n1. Введи команду /start чтобы начать работу." +
                         "\n2. Напиши название каждого города." +
                         "\n3. После того как ты перечислил все города,напиши команду /putRequires и выбери нужные тебе требования" +
                         "\n4. Нажми на кнопку найти маршрут");
                    break;
                default:
                    if (inputCities)
                    {
                        await view.SendMessage(chatId , model.CheckCities(message.Text));
                        inputCities = false;
                        model.CountDistanceBetweenCities();
                        //int minCost = MinimumSpanningTree(distanceBetweenCities);

                        //for (int i = 0; i < indexPath.Count; i++)
                        //{
                        //    Console.WriteLine(indexPath[i]);
                        //}
                        //Console.WriteLine($"mincost = {minCost}");
                    }
                    else
                    {
                        await view.SendMessage(chatId, "Привет! Я твой телеграм бот Map-Guid." +
                            "\n\n1.Команда /help - для получения инструкции" +
                            "\n2. Команда /start - чтобы начать работу.");
                    }
                    break;

            }

        }

        private static Task Error(ITelegramBotClient client, Exception exception, CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }

}
        
            
