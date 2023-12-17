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


namespace ConsoleApp2
{

    class Presenter
    {
        private static ITelegramBotClient _botClient;
        private static ReceiverOptions _receiverOptions;
        private static bool InputCities = false;
        private static bool InilineKeyboard = false;
        static Model model = new Model();

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
                            // эта переменная будет содержать в себе все связанное с сообщениями
                            var message = update.Message;

                            // From - это от кого пришло сообщение
                            var user = message.From;

                            // Chat - содержит всю информацию о чате
                            var chat = message.Chat;

                            // Добавляем проверку на тип Message
                            switch (message.Type)
                            {
                                // Тут понятно, текстовый тип
                                case MessageType.Text:
                                    {
                                        // тут обрабатываем команду /start, остальные аналогично
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
                                                model.CheckCities(message.Text)
                                            );
                                            InilineKeyboard = model.checkCitiesList;
                                            if(InilineKeyboard != true)
                                            {
                                                await botClient.SendTextMessageAsync(
                                                chat.Id,
                                                "Необходимо минимум 3 города, введите команду /start еще раз и заполните больше городов"
                                                );
                                                model.ClearFields();
                                            }
                                            await model.CountDistanceBetweenCities();
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
                                                // автоматическое изменение размера клавиатуры, если не стоит true,
                                                // тогда клавиатура растягивается чуть ли не до луны,
                                                // проверить можете сами
                                                ResizeKeyboard = true,
                                                OneTimeKeyboard = true
                                            };

                                            await botClient.SendTextMessageAsync(
                                                chat.Id,
                                                "Выберите условия нахождения маршрута",
                                                replyMarkup: replyKeyboard); // опять передаем клавиатуру в параметр replyMarkup

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
                            }
                            break;
                        }
                    case UpdateType.InlineQuery:
                        {

                            break;
                        }
                }
            }
            
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

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

}
        
            
