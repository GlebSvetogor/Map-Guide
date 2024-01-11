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
using static System.Net.WebRequestMethods;
using System.Linq;
using Telegram.Bot.Requests.Abstractions;

namespace ConsoleApp2
{
    class Presenter
    {
        private static ITelegramBotClient _botClient;
        private static ReceiverOptions _receiverOptions;
        private static Model model;
        public static APIRequest request;
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

                                        var validatorResponse = await CheckCitiesInputAsync(message.Text);
                                        string isNotCityInput = validatorResponse.Item1;
                                        bool isEnoughCities = validatorResponse.Item2;


                                        if (isEnoughCities)
                                        {
                                            if(isNotCityInput != "")
                                            {
                                                await botClient.SendTextMessageAsync(
                                                    chat.Id,
                                                    isNotCityInput
                                                );
                                                InlineKeyaboard = true;
                                            }
                                            else
                                            {
                                                InlineKeyaboard = true;
                                            }
                                        }
                                        else if(isNotCityInput != "")
                                        {
                                            await botClient.SendTextMessageAsync(
                                                chat.Id,
                                                isNotCityInput
                                            );
                                            await botClient.SendTextMessageAsync(
                                                chat.Id,
                                                "Необходимо минимум 3 города для нахождения маршрута, введи команду /start еще раз и заполни больше городов"
                                            );
                                        }
                                        else
                                        {
                                            await botClient.SendTextMessageAsync(
                                                chat.Id,
                                                "Необходимо минимум 3 города для нахождения маршрута, введи команду /start еще раз и заполни больше городов"
                                            );
                                        }

                                        if(InlineKeyaboard)
                                        {
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
                                                replyMarkup: replyKeyboard
                                            );
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
                                        string link = model.MakeMapLink();

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

        public static async Task<(string, bool)> CheckCitiesInputAsync(string mes)
        {
            List<City> cities = new List<City>();
            StringBuilder notCities = new StringBuilder();
            request = new APIRequest();
            string requestUrl = "https://maps.googleapis.com/maps/api/geocode/json?address=location&key=AIzaSyBGykNf1-zcVrXeSSkuYqRc01Gc02nh0Ho";
            string url = requestUrl;

            List<string> userInputCities = new List<string>();

            userInputCities.AddRange(mes.Trim().Split(' '));
            foreach (string cityName in userInputCities)
            {
                url = requestUrl.Replace("location", cityName);
                var isCity = await request.MakeCityRequestAsync(url, cities);
                if (isCity != true) { notCities.Append($"{cities.Last().longName} не является городом" + "\n"); }
            }

            if (cities.Count >= 3)
            {
                model.cities = cities;
                string validatorResponse = notCities.ToString();
                return (validatorResponse,true);
            }
            else
            {
                string validatorResponse = notCities.ToString();
                return (validatorResponse, false);
            }
            
        }
    }
}
        
            
