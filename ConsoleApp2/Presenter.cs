using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Newtonsoft.Json.Linq;
using Telegram.Bot.Polling;
using Telegram.Bot.Exceptions;
using System.Text;

namespace ConsoleApp2
{
    class Presenter
    {
        private static ITelegramBotClient _botClient;
        private static ReceiverOptions _receiverOptions;
        private static MatrixCreator matrixCreator;
        private static Model model;
        private static bool InputCities = false;
        private static bool InlineKeyaboard = false;

        static async Task Main()
        {
            _botClient = new TelegramBotClient("6414840375:AAHV2TVGdbGYuRZVgsKs3xkwplEsw1NANLQ");
            model = new Model();
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

            try 
            { 
                var me = await _botClient.GetMeAsync(); 
                Console.WriteLine($"{me.FirstName} запущен!");
            }catch(RequestException ex)
            {
                Console.WriteLine($"{ex.Message}", ex);
            }

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

                                        if (message.Text == "/map")
                                        {
                                            // Определите заголовок, описание и URL вашей игры
                                            string title = "My Game";
                                            string description = "This is my first game!";
                                            string gameUrl = "http://127.0.0.1:5500/index.html";

                                            // Создайте кнопку для запуска игры
                                            InlineKeyboardButton playButton = new InlineKeyboardButton("Play Now!")
                                            {
                                                Url = gameUrl
                                            };

                                            // Создайте разметку клавиатуры и добавьте кнопку
                                            InlineKeyboardMarkup markup = new InlineKeyboardMarkup(new[] { new[] { playButton } });

                                            // Отправьте сообщение с кнопкой пользователю
                                            await botClient.SendTextMessageAsync(chat.Id, description, replyMarkup: markup);
                                        }

                                        if(message.Text == "/help")
                                        {
                                            await botClient.SendTextMessageAsync(
                                                chat.Id,
                                                "С помощью этой инструкции ты сможешь найти оптимальный маршрут:\n" +
                                                "1. Введите команду /start для ввода названий городов\n" +
                                                "2. Выберите условие нахождения нужного маршрута"
                                            );
                                            return;
                                        }

                                        if (message.Text == "Самый быстрый маршрут")
                                        {
                                            InlineKeyaboard = false;
                                            Console.WriteLine("Вычисляется самый быстрый маршрут ...");

                                            await botClient.SendTextMessageAsync(
                                                chat.Id,
                                                "Происходит нахождение самого быстрого маршрута, пожалуйста подождите ..."
                                            );

                                            var resultRouteInfo = await model.FindFastestRouteAsync();

                                            await botClient.SendTextMessageAsync(
                                                chat.Id,
                                                "Самый быстрый путь займет: " + resultRouteInfo.Item1 + " ч." + "\n" +
                                                "Маршрут: " + resultRouteInfo.Item2
                                            );

                                        }

                                        if (message.Text == "Самый короткий маршрут")
                                        {
                                            InlineKeyaboard = false;
                                            Console.WriteLine("Вычисляется самый короткий маршрут ...");

                                            await botClient.SendTextMessageAsync(
                                                chat.Id,
                                                "Происходит нахождение самого короткого маршрута, пожалуйста подождите ..."
                                            );

                                            var resultRouteInfo = await model.FindShortestRouteAsync();

                                            await botClient.SendTextMessageAsync(
                                                chat.Id,
                                                "Кратчайший путь займет: " + resultRouteInfo.Item1 + " км." + "\n" +
                                                "Маршрут: " + resultRouteInfo.Item2
                                            );


                                        }

                                        if (InlineKeyaboard)
                                        {
                                            await botClient.SendTextMessageAsync(chat.Id, "Ошибка ввода ! \n" +
                                                "Выбери условие нахождения маршрута с помощью inline клавиатуры ...");
                                        }

                                        if (InputCities)
                                        {
                                            InputCities = false;

                                            //await botClient.SendTextMessageAsync(
                                            //    chat.Id,
                                            //    "Сейчас проверю что ты там отправил ..."
                                            //);

                                            var validationTuple = await InputValidator.CheckCities(message.Text);

                                            InlineKeyaboard = validationTuple.Item1.Count >= 3 ? true : false;

                                            if(validationTuple.Item2 != "")
                                            {
                                                await botClient.SendTextMessageAsync(
                                                chat.Id,
                                                validationTuple.Item2
                                                );
                                            }

                                            if (!InlineKeyaboard)
                                            {
                                                await botClient.SendTextMessageAsync(
                                                chat.Id,
                                                "Необходимо минимум 3 города для нахождения маршрута, введи команду /start еще раз и заполни больше городов"
                                                );
                                            }
                                            else
                                            {
                                                model.cities = validationTuple.Item1;

                                                var replyKeyboard = new ReplyKeyboardMarkup(
                                                new List<KeyboardButton[]>()
                                                {
                                                    new KeyboardButton[]
                                                    {
                                                        new KeyboardButton("Самый быстрый маршрут")
                                                    },
                                                    new KeyboardButton[]
                                                    {
                                                        new KeyboardButton("Самый короткий маршрут")
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

                                            }
                                        }

                                        break;
                                    }
                                default:
                                    {
                                        await botClient.SendTextMessageAsync(
                                            chat.Id,
                                            "Используй только текст!");
                                        break;
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

        private static void PrintMatrix(int[,] matrix)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    Console.Write(matrix[i, j] + "\t");
                }
                Console.WriteLine();
            }
        }


    }

    

    class InputValidator
    {
        private static string twoGisApiKey = "00419301-8df8-46e6-83f1-83cd25e7a8c1";
        public static async Task<(List<string>,string)> CheckCities(string mes)
        {
            List<string> userInputCities = new List<string>();
            List<string> cities = new List<string>();

            StringBuilder notCities = new StringBuilder();
            userInputCities.AddRange(mes.Trim().Split(' '));
            foreach (string cityName in userInputCities)
            {
                using var client = new HttpClient();
                HttpResponseMessage response = (await client.GetAsync($"https://catalog.api.2gis.com/3.0/items?q={cityName}&key={twoGisApiKey}"));
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    JObject json = JObject.Parse(responseBody);
                    
                    if ((int)json["result"]["total"] >= 1)
                    {
                        cities.Add(cityName);
                        Console.WriteLine($"{cityName} является городом");
                    }
                    else
                    {
                        Console.WriteLine($"{cityName} не является городом");
                        notCities.Append($"{cityName} не является городом" + "\n");
                    }
                    
                }
                else
                {
                    Console.WriteLine($"Error: {response.StatusCode}");
                }
            }

            var result = (cities, notCities.ToString());
            return result;
        }


        
    }

}
        
            
