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

                                            CoordinateMatrix coordinateMatrix = new CoordinateMatrix();

                                            matrixCreator = new TimeMatrixCreator();
                                            Matrix matrix = matrixCreator.CreateMatrix();

                                            var TimeMatrix = matrix.Count(await coordinateMatrix.Count(InputValidator.cities));

                                            Console.WriteLine("TimeMatrix:");

                                            for (int i = 0; i < TimeMatrix.GetLength(0); i++)
                                            {
                                                for (int j = 0; j < TimeMatrix.GetLength(0); j++)
                                                {
                                                    Console.WriteLine(TimeMatrix[i, j] + "    ");
                                                }
                                                Console.WriteLine("\n");
                                            }
                                            var result = TSP.Start(TimeMatrix);

                                            Console.WriteLine("Самый быстрый путь займет: " + result.Item2);
                                            Console.Write("Маршрут: ");
                                            for (int i = 0; i < TimeMatrix.GetLength(0); i++)
                                            {
                                                Console.Write(InputValidator.cities[result.Item1[i]] + " ");
                                            }

                                        }

                                        if (message.Text == "Самый короткий маршрут")
                                        {
                                            InlineKeyaboard = false;
                                            Console.WriteLine("Это самый короткий маршрут ...");

                                            CoordinateMatrix coordinateMatrix = new CoordinateMatrix();
                                            
                                            matrixCreator = new DistanceMatrixCreator();
                                            Matrix matrix = matrixCreator.CreateMatrix();

                                            var distanceMatrix = matrix.Count(await coordinateMatrix.Count(InputValidator.cities));

                                            Console.WriteLine("distanceMatrix:");

                                            for(int i = 0; i< distanceMatrix.GetLength(0); i++)
                                            {
                                                for(int j = 0; j < distanceMatrix.GetLength(0); j++)
                                                {
                                                    Console.WriteLine(distanceMatrix[i, j] + "    ");
                                                }
                                                Console.WriteLine("\n");
                                            }
                                            var result = TSP.Start(distanceMatrix);

                                            Console.WriteLine("Кратчайший путь: " + result.Item2);
                                            Console.Write("Маршрут: ");
                                            for (int i = 0; i < distanceMatrix.GetLength(0); i++)
                                            {
                                                Console.Write(InputValidator.cities[result.Item1[i]] + " ");
                                            }

                                        } 

                                        if (message.Text == "Настраиваемый маршрут")
                                        {
                                            InlineKeyaboard = false;
                                            Console.WriteLine("Это самый оптимальный маршрут ...");
                                            //_algorithmCreator = new ShortestAlgorithmCreator();
                                        }

                                        if (InlineKeyaboard)
                                        {
                                            await botClient.SendTextMessageAsync(chat.Id, "Ошибка ввода ! \n" +
                                                "Выбери условие нахождения маршрута с помощью inline клавиатуры ...");
                                        }

                                        if (InputCities)
                                        {
                                            InputCities = false;
                                            await botClient.SendTextMessageAsync(
                                                chat.Id,
                                                "Сейчас проверю что ты там отправил ..."
                                            );

                                            var validationTuple = await InputValidator.CheckCities(message.Text);
                                            InlineKeyaboard = validationTuple.Item1;
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
                                                await botClient.SendTextMessageAsync(
                                                chat.Id,
                                                "Всё в порядке"
                                                );
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
                                                    },
                                                    new KeyboardButton[]
                                                    {
                                                        new KeyboardButton("Настраиваемый маршрут")
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
    }

    class InputValidator
    {
        private static string twoGisApiKey = "00419301-8df8-46e6-83f1-83cd25e7a8c1";
        public static List<string> cities = new List<string>();
        public static async Task<(bool,string)> CheckCities(string mes)
        {
            List<string> userInputCities = new List<string>();
            cities.Clear();
            userInputCities.AddRange(mes.Trim().Split(' '));
            StringBuilder validationResponse = new StringBuilder();
            foreach (string cityName in userInputCities)
            {
                using var client = new HttpClient();
                HttpResponseMessage response = (await client.GetAsync($"https://catalog.api.2gis.com/3.0/items?q={cityName}&key={twoGisApiKey}"));
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    JObject json = JObject.Parse(responseBody);
                    Console.WriteLine(json);
                    if ((int)json["meta"]["code"] == 200)
                    {
                        //JArray items = (JArray)(json["result"]["items"]);
                        if ((int)json["result"]["total"] >= 1)
                        {
                            cities.Add(cityName);
                            Console.WriteLine($"{cityName} является городом");
                        }
                        else
                        {
                            Console.WriteLine($"{cityName} не является городом");
                            validationResponse.Append($"{cityName} не является городом" + "\n");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"{cityName} не является городом");
                        validationResponse.Append($"{cityName} не является городом" + "\n");
                    }
                    
                }
                else
                {
                    Console.WriteLine($"Error: {response.StatusCode}");
                }
            }

            var result = (cities.Count >= 3 ? true : false, validationResponse != null ? validationResponse.ToString() : "");
            return result;
        }


        
    }

}
        
            
