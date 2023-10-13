using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bots.Types.Inline;

namespace SvetilkaBot.Services
{
    internal class TelegramBotService
    {
        private readonly TelegramBotClient _botClient;
        private Chat _chat;
        private IMenu _menu;
        private StateService _stateService;
        private MqttService _mqttService;

        public TelegramBotService(TelegramBotClient botClient, MqttService mqttService)
        {
            _botClient = botClient;
            _stateService = new StateService();
            _mqttService = mqttService;
        }

        public async Task StartService(CancellationTokenSource cancellationToken)
        {
            ReceiverOptions receiverOptions = new()
            {
                AllowedUpdates = Array.Empty<UpdateType>()
            };

            // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
            _botClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: cancellationToken.Token
            );
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

        private async Task UpdateMessageHandle(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
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
                var button = InlineKeyboardButton.WithCallbackData("Продолжить \u27a1", "StartingMenu");
                var inline = new InlineKeyboardMarkup(button);
                await botClient.SendTextMessageAsync(
                    chatId: _chat.Id,
                    text: "Приветсвую!\nДанный бот пока что умеет предавать только ASCII символы для отображения на светодиодной матрице (в целях упрощения примера)",
                    replyMarkup: inline,
                    cancellationToken: cancellationToken);

                await Task.Delay(1000);
            }
        }

        private async Task UpdateCallbackQueryHandle(ITelegramBotClient botClient, CallbackQuery callbackQuery, CancellationToken cancellationToken)
        {
            switch (callbackQuery.Data)
            {
                case "StartingMenu":
                    switch (_stateService.GetMenuState(_chat.Id))
                    {
                        case UserState.NoState:
                            _menu = new StartingMenu(botClient, _chat, cancellationToken);
                            await _menu.PrintMenu(callbackQuery.Message.MessageId, true);
                            _stateService.SetMenuState(_chat.Id, UserState.StartingMenu);
                            break;
                        default:
                            _menu = new StartingMenu(botClient, _chat, cancellationToken);
                            await _menu.PrintMenu(callbackQuery.Message.MessageId);
                            _stateService.SetMenuState(_chat.Id, UserState.StartingMenu);
                            break;
                    }
                    break;
                case "ASCIIMenu":
                    _menu = new ASCIIMenu(botClient, _chat, _stateService, _mqttService, cancellationToken);
                    await _menu.PrintMenu(callbackQuery.Message.MessageId);
                    _stateService.SetMenuState(_chat.Id, UserState.ASCII);
                    break;
                default:
                    await _menu.CallbackQueryHandle(callbackQuery);
                    break;
            }
        }
    }
}
