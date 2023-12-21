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
        private static bool InputCities = false;
        private static bool InilineKeyboard = false;
        static HomeModel model;
        static InputValidator validator;

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
            model = new HomeModel();
            validator = new InputValidator();

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
                                                "1. Введи команду /start для ввода названий городов" +
                                                "2. Выбери алгоритм нахождения оптимального маршрута"
                                            );
                                            return;
                                        }

                                        if (message.Text == "Самый дешевый маршрут")
                                        {
                                            InilineKeyboard = false;

                                            return;
                                        }

                                        if (message.Text == "Самый короткий маршрут")
                                        {
                                            InilineKeyboard = false;

                                            return;
                                        }

                                        if (message.Text == "Оптимальный маршрут")
                                        {
                                            InilineKeyboard = false;

                                            return;
                                        }

                                        if (InputCities)
                                        {
                                            InputCities = false;
                                            await botClient.SendTextMessageAsync(
                                                chat.Id,
                                                validator.CheckCities(message.Text) ?? "Все хорошо"
                                            );
                                            InilineKeyboard = validator.checkCitiesListLength;
                                            if(InilineKeyboard != true)
                                            {
                                                await botClient.SendTextMessageAsync(
                                                chat.Id,
                                                "Необходимо минимум 3 города, введите команду /start еще раз и заполните больше городов"
                                                );
                                            }
                                        }

                                        if (InilineKeyboard == true)
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
                                                "Выберите условия нахождения маршрута",
                                                replyMarkup: replyKeyboard); 

                                            return;

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
            }
            
            catch (Exception ex)
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

    class InputValidator
    {
        static APIRequest request;
        static string username = "demo654";
        public InputValidator() 
        {
            request = new APIRequest();
        }

        public static List<string> cities 
        {
            get
            {
                return cities;
            }
        }
            
        public bool checkCitiesListLength
        {
            get
            {
                return cities.Count >= 3 ? true : false;
            }
        }

        public string CheckCities(string mes)
        {
            List<string> userCitiesInput = new List<string>();
            userCitiesInput.AddRange(mes.Trim().Split(' '));
            var response = new StringBuilder();
            foreach (string city in userCitiesInput)
            {
                string url = $"http://api.geonames.org/searchJSON?q={city}&maxRows=1&username={username}&featureClass=P&featureCode=PPL&isNameRequired=true";
                string responseBody = Convert.ToString(request.MakeRequest(url));
                JObject json = JObject.Parse(responseBody);
                int totalResultsCount = (int)json["totalResultsCount"];
                Console.WriteLine(responseBody);

                if (totalResultsCount > 1)
                {
                    cities.Add(city);
                }
                else
                {
                    response.Append($"{city} не является городом");
                }
            }
            return response.ToString();
        }


    }

}
        
            
