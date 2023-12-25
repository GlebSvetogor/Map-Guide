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
using Telegram.Bot.Polling;
using Telegram.Bot.Exceptions;
using System.Text;
using Telegram.Bot.Requests.Abstractions;


namespace ConsoleApp2
{

    class HomePresenter
    {
        private static ITelegramBotClient _botClient;
        private static ReceiverOptions _receiverOptions;
        private static AlgorithmCreator _algorithmCreator;
        private static MatrixCreator matrixCreator;
        private static bool InputCities = false;
        private static bool InlineKeyaboard = false;

        static async Task Main()
        {
            _botClient = new TelegramBotClient("6414840375:AAHV2TVGdbGYuRZVgsKs3xkwplEsw1NANLQ");
            _receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = new[] 
                {
                    UpdateType.Message, 
                },
                ThrowPendingUpdates = true,
            };


            using var cts = new CancellationTokenSource();

            _botClient.StartReceiving(UpdateHandler, ErrorHandler, _receiverOptions, cts.Token); 

            var me = await _botClient.GetMeAsync(); 
            Console.WriteLine($"{me.FirstName} запущен!");

            await Task.Delay(-1);
        }

        private static async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            try
            {
                switch (update.Type)
                {
                    case UpdateType.Message:
                        {
                            var message = update.Message;

                            var user = message.From;

                            var chat = message.Chat;

                            switch (message.Type)
                            {
                                case MessageType.Text:
                                    {
                                        if (message.Text == "/start")
                                        {
                                            InputCities = true;
                                            await botClient.SendTextMessageAsync(
                                                chat.Id,
                                                "Введи названия городов через пробел");
                                            return;
                                        }

                                        if(message.Text == "/help")
                                        {
                                            await botClient.SendTextMessageAsync(
                                                chat.Id,
                                                "С помощью этой инструкции ты сможешь найти оптимальный маршрут:\n" +
                                                "1. Введи команду /start для ввода названий городов\n" +
                                                "2. Выбери алгоритм нахождения оптимального маршрута"
                                            );
                                            return;
                                        }

                                        if (message.Text == "Самый быстрый маршрут")
                                        {
                                            InlineKeyaboard = false;
                                            Console.WriteLine("Это самый быстрый маршрут ...");
                                            
                                            return;
                                        }

                                        if (message.Text == "Самый короткий маршрут")
                                        {
                                            InlineKeyaboard = false;
                                            Console.WriteLine("Это самый короткий маршрут ...");
                                            matrixCreator = new DistanceMatrixCreator();
                                            Matrix matrix = matrixCreator.CreateMatrix();
                                            matrix.Count(InputValidator.cities);
                                            return;
                                        }

                                        if (message.Text == "Оптимальный маршрут")
                                        {
                                            InlineKeyaboard = false;
                                            Console.WriteLine("Это самый оптимальный маршрут ...");
                                            //_algorithmCreator = new ShortestAlgorithmCreator();
                                            return;
                                        }

                                        if (InputCities)
                                        {
                                            InputCities = false;
                                            await botClient.SendTextMessageAsync(
                                                chat.Id,
                                                "Сейчас проверю что ты там отправил ..."
                                            );

                                            await botClient.SendTextMessageAsync(
                                                chat.Id,
                                                await InputValidator.CheckCities(message.Text)
                                            );

                                            InlineKeyaboard = InputValidator.checkCitiesListLength();
                                            if (!InlineKeyaboard)
                                            {
                                                await botClient.SendTextMessageAsync(
                                                chat.Id,
                                                "Необходимо минимум 3 города для нахождения маршрута, введи команду /start еще раз и заполни больше городов"
                                                );
                                            }
                                            else
                                            {
                                                var replyKeyboard = new ReplyKeyboardMarkup(
                                                new List<KeyboardButton[]>()
                                                {
                                                    new KeyboardButton[]
                                                    {
                                                        new KeyboardButton("Самый дешевый маршрут")
                                                    },
                                                    new KeyboardButton[]
                                                    {
                                                        new KeyboardButton("Самый короткий маршрут")
                                                    },
                                                    new KeyboardButton[]
                                                    {
                                                        new KeyboardButton("Оптимальный маршрут")
                                                    }
                                                })
                                                {
                                                    ResizeKeyboard = true,
                                                    OneTimeKeyboard = true
                                                };

                                                await botClient.SendTextMessageAsync(
                                                    chat.Id,
                                                    "Выбери условие нахождения маршрута",
                                                    replyMarkup: replyKeyboard);

                                                return;
                                            }
                                        }

                                        if (InlineKeyaboard)
                                        {
                                            await botClient.SendTextMessageAsync(chat.Id, "Ошибка ввода ! \n" +
                                                "Выбери условие нахождения маршрута с помощью inline клавиатуры ...");
                                        }

                                        break;
                                    }
                                default:
                                    {
                                        await botClient.SendTextMessageAsync(
                                            chat.Id,
                                            "Используй только текст!");
                                        return;
                                    }
                            }// switch (message.Type)

                            break;

                        }// case (UpdateType.Message)
                    case UpdateType.InlineQuery:
                        {
                            break;
                        }
                }// case (UpdateType = message)
            }catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }//switch (update.Type)
        private static Task ErrorHandler(ITelegramBotClient botClient, Exception error, CancellationToken cancellationToken)
        {
            var ErrorMessage = error switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => error.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
    }

    static class InputValidator
    {
        private static string username = "demo654";
        private static StringBuilder response;
        public static List<string> cities = new List<string>();

        public static async Task<string> CheckCities(string mes)
        {
            List<string> userInputCities = new List<string>();
            userInputCities.AddRange(mes.Trim().Split(' '));
            var response = new StringBuilder();
            foreach (string city in userInputCities)
            {
                JObject json = await APIRequest.MakeRequest(city);
                int totalResultsCount = (int)json["totalResultsCount"];

                if (totalResultsCount > 1)
                {
                    cities.Add(city);
                    response.Append($"{city} является городом" + "\n");
                }
                else
                {
                    response.Append($"{city} не является городом" + "\n");
                }
            }

            return response.ToString();
        }
        public static bool checkCitiesListLength() => cities.Count >= 3 ? true : false;
    }

}
        
            
