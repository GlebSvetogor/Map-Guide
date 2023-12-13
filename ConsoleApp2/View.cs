using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;

namespace ConsoleApp2
{
    class View
    {
        public ITelegramBotClient client = new TelegramBotClient("6414840375:AAHV2TVGdbGYuRZVgsKs3xkwplEsw1NANLQ");

        public async Task SendMessage(long chatId,string response)
        {
            await client.SendTextMessageAsync(chatId, response);
        }
    }
}
