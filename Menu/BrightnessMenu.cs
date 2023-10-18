using SvetilkaBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace SvetilkaBot.Menu
{
    internal class BrightnessMenu : IMenu
    {
        private readonly ITelegramBotClient _botClient;
        private readonly CancellationToken _cancellationToken;
        private readonly Chat _chat;
        private InlineKeyboardMarkup _inlineKeyboard;
        private MqttService _mqttService;
        private string _brightnessState;


        public BrightnessMenu(ITelegramBotClient botClient, Chat chat, MqttService mqttService, CancellationToken cancellationToken)
        {
            _botClient = botClient;
            _chat = chat;
            _cancellationToken = cancellationToken;
            _mqttService = mqttService;
            _brightnessState = DBService.GetBrightnessState(_chat.Id);
            GenerateInlineKeyboard();
        }

        public async Task CallbackQueryHandle(CallbackQuery callbackQuery)
        {
            _brightnessState = callbackQuery.Data;
            DBService.SetBrightnessState(_chat.Id, _brightnessState);
            _mqttService.SendBrightnessStateMQTT(_brightnessState);
            GenerateInlineKeyboard();
            PrintMenu(callbackQuery.Message.MessageId);
        }

        public async Task PrintMenu(int messageId, bool alternativeMode = false)
        {
            await _botClient.EditMessageTextAsync(
                chatId: _chat.Id,
                messageId: messageId,
                text: "Выберите яркость для отображения символа на RGB матрице",
                replyMarkup: _inlineKeyboard,
                cancellationToken: _cancellationToken);
        }
        private void GenerateInlineKeyboard()
        {
            var buttonsDescription = new string[] { "25%", "50%", "75%", "100%" };
            var buttonsArray = new InlineKeyboardButton[2][];
            buttonsArray[0] = new InlineKeyboardButton[buttonsDescription.Length];
            for (int i = 0; i < buttonsArray[0].Length; i++)
            {
                buttonsArray[0][i] = InlineKeyboardButton.WithCallbackData(ButtonStatusHandle(buttonsDescription[i]), buttonsDescription[i]);
            }
            buttonsArray[1] = new InlineKeyboardButton[1];
            buttonsArray[1][0] = InlineKeyboardButton.WithCallbackData($"Вернуться назад \u21A9", "AsciiMenu");

            _inlineKeyboard = new(buttonsArray);
        }

        private string ButtonStatusHandle(string brightness)
        {
            if (_brightnessState == brightness)
            {
                return $"|| {brightness} ||";
            }
            return $"{brightness}";
        }
    }
}