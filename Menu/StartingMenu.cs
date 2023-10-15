using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace SvetilkaBot.Menu
{
    internal class StartingMenu : IMenu
    {
        private readonly ITelegramBotClient _botClient;
        private readonly CancellationToken _cancellationToken;
        private readonly Chat _chat;
        private InlineKeyboardMarkup _inlineKeyboard;

        public StartingMenu(ITelegramBotClient botClient, Chat chat, CancellationToken cancellationToken)
        {
            _botClient = botClient;
            _chat = chat;
            _cancellationToken = cancellationToken;

            GenerateInlineKeyboard();
        }

        public async Task PrintMenu(int messageId, bool alternativeMode)
        {
            if (alternativeMode == true)
            {
                await _botClient.SendTextMessageAsync(
                chatId: _chat.Id,
                text: "Выберите режим работы",
                replyMarkup: _inlineKeyboard,
                cancellationToken: _cancellationToken);
            }
            else
            {
                await _botClient.EditMessageTextAsync(
                chatId: _chat.Id,
                messageId: messageId,
                text: "Выберите режим работы",
                replyMarkup: _inlineKeyboard,
                cancellationToken: _cancellationToken);
            }
        }

        public async Task CallbackQueryHandle(CallbackQuery callbackQuery)
        {
            switch (callbackQuery.Data)
            {
                case "ASCIIMenu":
                    break;
                case "notImplemented":
                    await _botClient.AnswerCallbackQueryAsync(callbackQuery.Id, "Функционал отсутствует \U0001F512");
                    break;
            }
        }

        private void GenerateInlineKeyboard()
        {
            _inlineKeyboard = new(
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
        }
    }
}
