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
    internal class MainMenu
    {
        private readonly ITelegramBotClient _botClient;
        private readonly CancellationToken _cancellationToken;
        private readonly Chat _chat;
        private ASCIIMenu _asciiMenu;


        public MainMenu(ITelegramBotClient botClient, Chat chat, CancellationToken cancellationToken)
        {
            _botClient = botClient;
            _chat = chat;
            _cancellationToken = cancellationToken;
        }

        public async Task PrintMenu()
        {
            InlineKeyboardMarkup inlineKeyboard = new(
                new[] {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Передать ASCII символы", "ASCIIMenu")
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Функционал отсутствует \U0001F512", "notImplemented")
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Функционал отсутствует \U0001F512", "notImplemented")
                    }
                }
                );

            await _botClient.SendTextMessageAsync(
                _chat.Id,
                "Выберите режим работы",
                replyMarkup: inlineKeyboard);
        }

        public async Task UpdateCallbackQueryHandle(ITelegramBotClient botClient, CallbackQuery callbackQuery, CancellationToken cancellationToken)
        {
            switch (callbackQuery.Data)
            {
                case "ASCIIMenu":
                    _asciiMenu = new ASCIIMenu(botClient, _chat, cancellationToken);
                    await _asciiMenu.PrintMenu();
                    break;
                case "notImplemented":
                    await _botClient.AnswerCallbackQueryAsync(callbackQuery.Id, "Функционал отсутствует \U0001F512");
                    break;
            }
        }
    }
}
