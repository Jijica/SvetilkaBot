using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Server;
using SvetilkaBot;
using System.Windows.Markup;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using SvetilkaBot.Services;

namespace SvetilkaBot
{
    class Program
    {
        static async Task Main()
        {
            var envVar = Environment.GetEnvironmentVariable("SvetilkaBotToken", EnvironmentVariableTarget.User);
            var botClient = new TelegramBotClient(envVar);
            using CancellationTokenSource cancellationToken = new();

            envVar = Environment.GetEnvironmentVariable("MQTT_ID", EnvironmentVariableTarget.User);
            var mqttClientOptions = new MqttClientOptionsBuilder()
                .WithClientId(envVar)
                .WithTcpServer("dev.rightech.io", 1883)
                .Build();
            var mqttClient = new MqttService(mqttClientOptions);

            var tgBotService = new TelegramBotService(botClient,mqttClient);

            tgBotService.StartService(cancellationToken);
            mqttClient.StartService();

            var me = await botClient.GetMeAsync();

            Console.WriteLine($"Start listening for @{me.Username}");
            Console.ReadLine();

            cancellationToken.Cancel();
        }
    }
}