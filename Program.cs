using MQTTnet.Client;
using SvetilkaBot.Services;
using Telegram.Bot;

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