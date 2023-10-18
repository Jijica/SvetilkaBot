using SvetilkaBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;


namespace SvetilkaBot.Menu
{
    internal class AsciiMenu : IMenu
    {
        private readonly ITelegramBotClient _botClient;
        private readonly CancellationToken _cancellationToken;
        private readonly Chat _chat;
        private InlineKeyboardMarkup _inlineKeyboard;
        private MqttService _mqttService;
        private string _asciiState;

        public AsciiMenu(ITelegramBotClient botClient, Chat chat, MqttService mqttService, CancellationToken cancellationToken)
        {
            _botClient = botClient;
            _chat = chat;
            _cancellationToken = cancellationToken;
            _mqttService = mqttService;
            _asciiState = DBService.GetLedState(_chat.Id);
            GenerateInlineKeyboard();
        }

        public async Task PrintMenu(int messageId, bool alternativeMode = false)
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
            if (callbackQuery.Data.StartsWith("ASCII-"))
            {
                var CBQstripped = callbackQuery.Data.Substring(6);
                if (CBQstripped == _asciiState)
                {
                    _asciiState = "OFF";
                }
                else
                {
                    _asciiState = CBQstripped;
                }
                DBService.SetLedState(_chat.Id, _asciiState);
                _mqttService.SendAsciiStateMQTT(_asciiState);
                GenerateInlineKeyboard();
                PrintMenu(callbackQuery.Message.MessageId);
            }
        }

        private void GenerateInlineKeyboard()
        {
            var buttonsArray = new InlineKeyboardButton[14][];
            // ASCII printable characters
            var asciiCodeDecimal = 33;
            for (int i = 0; i < 10; i++)
            {
                buttonsArray[i] = new InlineKeyboardButton[9];
                for (int j = 0; j < 9; j++)
                {
                    buttonsArray[i][j] = InlineKeyboardButton.WithCallbackData($"{ButtonStatusHandle(asciiCodeDecimal)}", $"ASCII-{(char)asciiCodeDecimal}");
                    asciiCodeDecimal++;
                }
            }
            buttonsArray[10] = new InlineKeyboardButton[3];
            for (int i = 0; i < 3; i++)
            {
                buttonsArray[10][i] = InlineKeyboardButton.WithCallbackData($"{ButtonStatusHandle(asciiCodeDecimal)}", $"ASCII-{(char)asciiCodeDecimal}");
                asciiCodeDecimal++;
            }
            buttonsArray[11] = new InlineKeyboardButton[1];
            buttonsArray[11][0] = InlineKeyboardButton.WithCallbackData($"Изменить цвет \U0001F308", "ColourMenu");
            buttonsArray[12] = new InlineKeyboardButton[1];
            buttonsArray[12][0] = InlineKeyboardButton.WithCallbackData($"Изменить яркость \U0001F506", "BrightnessMenu");
            buttonsArray[13] = new InlineKeyboardButton[1];
            buttonsArray[13][0] = InlineKeyboardButton.WithCallbackData($"Вернуться назад \u21A9", "StartingMenu");

            _inlineKeyboard = new(buttonsArray);
        }

        private string ButtonStatusHandle(int asciiCodeDecimal)
        {
            if (_asciiState == ((char)asciiCodeDecimal).ToString())
            {
                return $"{(char)asciiCodeDecimal} \u2705"; //u2705 //\U0001F4A1
            }
            return $"{(char)asciiCodeDecimal}";
        }
    }
}
