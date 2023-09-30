using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SvetilkaBot
{
    internal interface IMenu
    {
        public Task PrintMenu(int messageId, bool alternativeMode = false);
        public Task CallbackQueryHandle(CallbackQuery callbackQuery);
    }
}
