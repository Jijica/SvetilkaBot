using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bots.Types.Inline;

namespace SvetilkaBot
{
    internal class TelegramBotService
    {
        private readonly TelegramBotClient _botClient;
        private Chat _chat;
        private MainMenu _menu;

        public TelegramBotService(TelegramBotClient botClient)
        {
            _botClient = botClient;
        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            try
            {
                switch (update.Type)
            {
                case UpdateType.Message:
                    await UpdateMessageHandle(_botClient, update, cancellationToken);
                    break;
                case UpdateType.CallbackQuery:
                    await UpdateCallbackQueryHandle(_botClient, update.CallbackQuery, cancellationToken);
                    break;
            }
            }
            catch (Exception exception)
            {
                await HandlePollingErrorAsync(botClient, exception, cancellationToken);
            }
        }

        public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }

        private async Task UpdateMessageHandle(ITelegramBotClient botClient,Update update, CancellationToken cancellationToken)
        {
            if (update.Message is null)
                return;
            var message = update.Message;

            if (message.Text is null)
                return;
            var messageText = message.Text;

            _chat = message.Chat;

            Console.WriteLine($"Received a '{messageText}' message in chat {_chat.Id}.");
            var text = update.Message.Text;

            if (text == "/start")
            {
                var button = InlineKeyboardButton.WithCallbackData("Продолжить \u27a1", "greeting_next");
                var inline = new InlineKeyboardMarkup(button);
                await botClient.SendTextMessageAsync(
                    chatId: _chat.Id,
                    text: "Приветсвую!\nДанный бот пока что умеет предавать только ASCII символы для отображения на светодиодной матрице (в целях упрощения примера)",
                    replyMarkup: inline,
                    cancellationToken: cancellationToken);
            }
        }

        private async Task UpdateCallbackQueryHandle(ITelegramBotClient botClient, CallbackQuery callbackQuery, CancellationToken cancellationToken)
        {
            switch (callbackQuery.Data)
            {
                case "greeting_next":
                    _menu = new MainMenu(botClient, _chat, cancellationToken);
                    await _menu.PrintMenu();
                    break;
                default:
                    if (_menu != null)
                    {
                        await _menu.UpdateCallbackQueryHandle(botClient, callbackQuery, cancellationToken);
                    }
                    break;
            }
        }
    }
}
