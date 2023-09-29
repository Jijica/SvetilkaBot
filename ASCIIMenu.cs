using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace SvetilkaBot
{
    internal class ASCIIMenu
    {
        private readonly ITelegramBotClient _botClient;
        private readonly CancellationToken _cancellationToken;
        private readonly Chat _chat;

        public ASCIIMenu(ITelegramBotClient botClient, Chat chat, CancellationToken cancellationToken)
        {
            _botClient = botClient;
            _chat = chat;
            _cancellationToken = cancellationToken;
        }

        public async Task PrintMenu()
        {
            var buttonsArray = new InlineKeyboardButton[12][];
            // ASCII printable characters
            var asciiCodeDecimal = 33;
            for (int i = 0; i < 10; i++)
            {
                buttonsArray[i] = new InlineKeyboardButton[9];
                for (int j = 0; j < 9; j++)
                {
                    buttonsArray[i][j] = InlineKeyboardButton.WithCallbackData($"{(char)asciiCodeDecimal}", $"ASCII-{asciiCodeDecimal}");
                    asciiCodeDecimal++;
                }
            }
            buttonsArray[10] = new InlineKeyboardButton[3];
            for (int i = 0; i < 3; i++)
            {
                buttonsArray[10][i] = InlineKeyboardButton.WithCallbackData($"{(char)asciiCodeDecimal}", $"ASCII-{asciiCodeDecimal}");
                asciiCodeDecimal++;
            }
            buttonsArray[11] = new InlineKeyboardButton[1];
            buttonsArray[11][0] = InlineKeyboardButton.WithCallbackData($"Вернуться назад \u21A9", $"ASCII-previous");

            InlineKeyboardMarkup inlineKeyboard = new(buttonsArray);

            await _botClient.SendTextMessageAsync(
                _chat.Id,
                "Выберите символ для отображения на RGB матрице",
                replyMarkup: inlineKeyboard);
        }

        public async Task UpdateCallbackQueryHandle(ITelegramBotClient botClient, CallbackQuery callbackQuery, CancellationToken cancellationToken)
        {
            switch (callbackQuery.Data)
            {
                case "ASCIIMenu":

                    break;
            }
        }
    }
}
