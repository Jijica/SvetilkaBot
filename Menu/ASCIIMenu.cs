using SvetilkaBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace SvetilkaBot.Menu
{
    internal class ASCIIMenu : IMenu
    {
        private readonly ITelegramBotClient _botClient;
        private readonly CancellationToken _cancellationToken;
        private readonly Chat _chat;
        private InlineKeyboardMarkup _inlineKeyboard;
        private MqttService _mqttService;

        public ASCIIMenu(ITelegramBotClient botClient, Chat chat, MqttService mqttService, CancellationToken cancellationToken)
        {
            _botClient = botClient;
            _chat = chat;
            _cancellationToken = cancellationToken;
            _mqttService = mqttService;
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
                if (CBQstripped == DBService.GetLedState(_chat.Id))
                {
                    DBService.SetLedState(_chat.Id, "OFF");
                }
                else
                {
                    DBService.SetLedState(_chat.Id, CBQstripped);
                }
                _mqttService.SendMessageToMQTTBroker(DBService.GetLedState(_chat.Id));
            }
            GenerateInlineKeyboard();
            PrintMenu(callbackQuery.Message.MessageId);
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
            buttonsArray[11][0] = InlineKeyboardButton.WithCallbackData($"Вернуться назад \u21A9", "StartingMenu");

            _inlineKeyboard = new(buttonsArray);
        }

        private string ButtonStatusHandle(int asciiCodeDecimal)
        {

            if (DBService.GetLedState(_chat.Id) == ((char)asciiCodeDecimal).ToString())
            {
                return $"{(char)asciiCodeDecimal} \u2705"; //u2705 //\U0001F4A1
            }
            return $"{(char)asciiCodeDecimal}";
        }
    }
}
