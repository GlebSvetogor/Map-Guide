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
                // Сразу же ставим конструкцию switch, чтобы обрабатывать приходящие Update
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
                                                "Введи названия городов");
                                            return;
                                        }

                                        if(message.Text == "/help")
                                        {
                                            await botClient.SendTextMessageAsync(
                                                chat.Id,
                                                "Спомощью этой инструкции ты сможешь найти оптимальный маршрут:\n" +
                                                "1. Введите команду /start для ввода названия городов" +
                                                "2. Выберите алгоритм нахождения оптимального маршрута"
                                            );
                                            return;
                                        }

                                        if (InputCities)
                                        {
                                            InputCities = false;
                                            await botClient.SendTextMessageAsync(
                                                chat.Id,
                                                model.CheckCities(message.Text)
                                            );
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
        
            
