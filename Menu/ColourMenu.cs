using SvetilkaBot.Services;
using System.Collections.Immutable;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace SvetilkaBot.Menu
{
    internal class ColourMenu : IMenu
    {
        private readonly ITelegramBotClient _botClient;
        private readonly CancellationToken _cancellationToken;
        private readonly Chat _chat;
        private InlineKeyboardMarkup _inlineKeyboard;
        private MqttService _mqttService;
        private string _colourState;

        public ColourMenu(ITelegramBotClient botClient, Chat chat, MqttService mqttService, CancellationToken cancellationToken)
        {
            _botClient = botClient;
            _chat = chat;
            _cancellationToken = cancellationToken;
            _mqttService = mqttService;
            _colourState = DBService.GetColourState(_chat.Id);
            GenerateInlineKeyboard();
        }

        public async Task CallbackQueryHandle(CallbackQuery callbackQuery)
        {
            _colourState = callbackQuery.Data;
            DBService.SetColourState(_chat.Id, _colourState);
            _mqttService.SendColourStateMQTT(_colourState);
            GenerateInlineKeyboard();
            PrintMenu(callbackQuery.Message.MessageId);
        }

        public async Task PrintMenu(int messageId, bool alternativeMode = false)
        {
            await _botClient.EditMessageTextAsync(
                chatId: _chat.Id,
                messageId: messageId,
                text: "Выберите цвет символа для отображения на RGB матрице",
                replyMarkup: _inlineKeyboard,
                cancellationToken: _cancellationToken);
        }
        private void GenerateInlineKeyboard()
        {
            var buttonsDescription = new string[,]
            {
                {"White", "\U00002B1C"},
                {"Red", "\U0001f7e5" },
                {"Green", "\U0001F7E9"},
                {"Blue", "\U0001F7E6"}
            };
            var buttonsArray = new InlineKeyboardButton[2][];
            buttonsArray[0] = new InlineKeyboardButton[buttonsDescription.GetLength(0)];
            for (int i = 0; i < buttonsArray[0].Length; i++)
            {
                buttonsArray[0][i] = InlineKeyboardButton.WithCallbackData(
                    ButtonStatusHandle
                    (
                    colour: buttonsDescription[i, 0],
                    unicode: buttonsDescription[i, 1]
                    ),
                    buttonsDescription[i, 0]
                    );
            }
            buttonsArray[1] = new InlineKeyboardButton[1];
            buttonsArray[1][0] = InlineKeyboardButton.WithCallbackData($"Вернуться назад \u21A9", "AsciiMenu");

            _inlineKeyboard = new(buttonsArray);
        }

        private string ButtonStatusHandle(string colour, string unicode)
        {
            if (_colourState == colour)
            {
                return $"|| {unicode} ||";
            }
            return $"{unicode}";
        }
    }
}