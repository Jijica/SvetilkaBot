﻿using System;
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
    internal class ASCIIMenu : IMenu
    {
        private readonly ITelegramBotClient _botClient;
        private readonly CancellationToken _cancellationToken;
        private readonly Chat _chat;
        private InlineKeyboardMarkup _inlineKeyboard;

        public ASCIIMenu(ITelegramBotClient botClient, Chat chat, CancellationToken cancellationToken)
        {
            _botClient = botClient;
            _chat = chat;
            _cancellationToken = cancellationToken;
            GenerateInlineKeyboard();
        }

        public async Task PrintMenu(int messageId, bool alternativeMode)
        {
            await _botClient.EditMessageTextAsync(
                chatId: _chat.Id,
                messageId: messageId,
                text: "Выберите символ для отображения на RGB матрице",
                replyMarkup: _inlineKeyboard,
                cancellationToken: _cancellationToken);
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
            buttonsArray[11][0] = InlineKeyboardButton.WithCallbackData($"Вернуться назад \u21A9", $"StartingMenu");

            _inlineKeyboard = new(buttonsArray);
        }
    }
}
