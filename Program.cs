using SvetilkaBot;
using System.Windows.Markup;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

var envVar = Environment.GetEnvironmentVariable("SvetilkaBot", EnvironmentVariableTarget.User);
var botClient = new TelegramBotClient(envVar);

using CancellationTokenSource cancellationToken = new();

// StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
ReceiverOptions receiverOptions = new()
{
    AllowedUpdates = Array.Empty<UpdateType>() // receive all update types except ChatMember related updates
};

var botService = new TelegramBotService(botClient);

botClient.StartReceiving(
    updateHandler: botService.HandleUpdateAsync,
    pollingErrorHandler: botService.HandlePollingErrorAsync,
    receiverOptions: receiverOptions,
    cancellationToken: cancellationToken.Token
);


var me = await botClient.GetMeAsync();

Console.WriteLine($"Start listening for @{me.Username}");
Console.ReadLine();

// Send cancellation request to stop bot
cancellationToken.Cancel();

