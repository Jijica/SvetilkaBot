using MQTTnet.Client;
using SvetilkaBot.Services;
using Telegram.Bot;

namespace SvetilkaBot
{
    class Program
    {
        static async Task Main()
        {
            var botClient = new TelegramBotClient(Config.TelegramToken);
            using CancellationTokenSource cancellationToken = new();

            var mqttClientOptions = new MqttClientOptionsBuilder()
                .WithClientId(Config.MqttId)
                .WithTcpServer("dev.rightech.io", 1883)
                .Build();
            var mqttClient = new MqttService(mqttClientOptions);

            var tgBotService = new TelegramBotService(botClient, mqttClient);

            tgBotService.StartService(cancellationToken);
            mqttClient.StartService();

            var me = await botClient.GetMeAsync();

            Console.WriteLine($"Start listening for @{me.Username}");
            Console.ReadLine();

            cancellationToken.Cancel();
        }
    }
}