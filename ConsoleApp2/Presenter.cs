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
using Newtonsoft.Json;

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
        private static MapLinkCreator mapLinkCreator;

        static async Task Main()
        {
            _botClient = new TelegramBotClient("6414840375:AAHV2TVGdbGYuRZVgsKs3xkwplEsw1NANLQ");
            model = new Model();
            _receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = new[] 
                {
                    UpdateType.Message, 
                    UpdateType.CallbackQuery
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
                                            "Введите названия городов через пробел");


                                        return;
                                    }


                                        if (message.Text == "/help")
                                    {
                                        await botClient.SendTextMessageAsync(
                                            chat.Id,
                                            "С помощью этой инструкции ты сможешь найти оптимальный маршрут:\n" +
                                            "1. Введите команду /start для ввода названий городов\n" +
                                            "2. Выберите условие нахождения нужного маршрута"
                                        );
                                        return;
                                    }

                                    if (message.Text == "Самый короткий маршрут")
                                    {
                                        InlineKeyaboard = false;
                                        Console.WriteLine("Вычисляется самый короткий маршрут ...");

                                        await botClient.SendTextMessageAsync(
                                            chat.Id,
                                            "Происходит нахождение самого короткого маршрута, пожалуйста подождите ..."
                                        );

                                        string resultRouteInfo = await model.FindShortestRouteAsync();

                                            var inlineKeyboard = new InlineKeyboardMarkup(
                                                new List<InlineKeyboardButton[]>()
                                                {
                                                    new InlineKeyboardButton[]
                                                    {
                                                        InlineKeyboardButton.WithCallbackData("Получить Google карту", "button1"),
                                                    },
                                                    new InlineKeyboardButton[]
                                                    {
                                                        InlineKeyboardButton.WithCallbackData("Сохранить маршрут", "button2"),
                                                    },
                                                    new InlineKeyboardButton[]
                                                    {
                                                        InlineKeyboardButton.WithCallbackData("Найти новый маршрут", "button2"),
                                                    },
                                                });

                                            await botClient.SendTextMessageAsync(
                                            chat.Id,
                                            resultRouteInfo,
                                            replyMarkup: inlineKeyboard
                                        );

                                        }

                                    if (message.Text == "Настраиваемый маршрут")
                                    {
                                        InlineKeyaboard = false;
                                        Console.WriteLine("Вычисляется самый короткий маршрут ...");

                                        await botClient.SendTextMessageAsync(
                                            chat.Id,
                                            "Происходит нахождение самого короткого маршрута, пожалуйста подождите ..."
                                        );

                                        string resultRouteInfo = await model.FindShortestRouteAsync();

                                        await botClient.SendTextMessageAsync(
                                            chat.Id,
                                            resultRouteInfo
                                        );
                                    }

                                    if (InputCities)
                                    {
                                        InputCities = false;

                                        var validatorResponse = await InputValidator.CheckCities(message.Text);
                                            
                                        InlineKeyaboard = validatorResponse.Item2.Count >= 3 ? true : false;

                                        if(validatorResponse.Item1 != "")
                                        {
                                            await botClient.SendTextMessageAsync(
                                            chat.Id,
                                            validatorResponse.Item1
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
                                            model.cities = validatorResponse.Item2;

                                            var replyKeyboard = new ReplyKeyboardMarkup(
                                            new List<KeyboardButton[]>()
                                            {
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
                                                "Выберите вариант нахождения маршрута",
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

                    case UpdateType.CallbackQuery:
                        {
                            var callbackQuery = update.CallbackQuery;

                            var user = callbackQuery.From;

                            Console.WriteLine($"{user.FirstName} ({user.Id}) нажал на кнопку: {callbackQuery.Data}");


                            var chat = callbackQuery.Message.Chat;

                            // Добавляем блок switch для проверки кнопок
                            switch (callbackQuery.Data)
                            {
                                // Data - это придуманный нами id кнопки, мы его указывали в параметре
                                // callbackData при создании кнопок. У меня это button1, button2 и button3

                                case "button1":
                                    {
                                        mapLinkCreator = new GoogleMapLinkCreator();
                                        MapLink googleMapLink = mapLinkCreator.CreateMapLink();

                                        string link = googleMapLink.CreateMapLink(model.citiesIndexesRouteOrder, model.coordinateMatrix.Count(model.cities));

                                        await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);

                                        await botClient.SendTextMessageAsync(
                                            chat.Id,
                                            link);

                                        model.cities.Clear();

                                        return;
                                    }

                                case "button2":
                                    {
                                        // А здесь мы добавляем наш сообственный текст, который заменит слово "загрузка", когда мы нажмем на кнопку
                                        await botClient.AnswerCallbackQueryAsync(callbackQuery.Id, "Тут может быть ваш текст!");

                                        await botClient.SendTextMessageAsync(
                                            chat.Id,
                                            $"Вы нажали на {callbackQuery.Data}");

                                        model.cities.Clear();

                                        return;
                                    }

                            }

                            return;
                        }
                }
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
        private static string geonamesUsername = "demo654";

        public static async Task<(string, List<Root>)> CheckCities(string mes)
        {
            List<string> userInputCities = new List<string>();
            List<Root> cities = new List<Root>();

            StringBuilder notCities = new StringBuilder();
            userInputCities.AddRange(mes.Trim().Split(' '));
            foreach (string cityName in userInputCities)
            {
                using var client = new HttpClient();
                var response = await client.GetAsync($"http://api.geonames.org/searchJSON?q={cityName}&username={geonamesUsername}&maxRows=5");
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    JObject json = JObject.Parse(responseBody);
                    Console.WriteLine(json);
                    if ((int)json["totalResultsCount"] >= 1)
                    {
                        JArray geonames = (JArray)json["geonames"];
                        JToken geonamesFirst = geonames.First;
                        Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(geonamesFirst.ToString());
                        cities.Add(myDeserializedClass);
                        
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

            string validatorResponse = notCities.ToString();
            return (validatorResponse, cities);
        }


        
    }

}
        
            
