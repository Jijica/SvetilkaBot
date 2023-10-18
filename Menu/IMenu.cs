using Telegram.Bot.Types;

namespace SvetilkaBot.Menu
{
    internal interface IMenu
    {
        public async Task PrintMenu(int messageId, bool alternativeMode = false) { }
        public async Task CallbackQueryHandle(CallbackQuery callbackQuery) { }
    }
}
