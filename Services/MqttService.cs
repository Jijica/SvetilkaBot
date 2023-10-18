using MQTTnet.Client;
using MQTTnet;

namespace SvetilkaBot.Services
{
    internal class MqttService
    {
        private readonly IMqttClient _mqttClient;
        private readonly MqttClientOptions _options;
        public MqttService(MqttClientOptions options)
        {
            var mqttFactory = new MqttFactory();
            _mqttClient = mqttFactory.CreateMqttClient();
            _options = options;
        }

        public async Task StartService()
        {
            _mqttClient.ConnectAsync(_options);
        }

        public async Task SendAsciiStateMQTT(string message)
        {
            if (_mqttClient.IsConnected)
            {
                var mqttMessage = new MqttApplicationMessageBuilder()
                    .WithTopic("base/state/ascii")
                    .WithPayload(message)
                    .Build();

                await _mqttClient.PublishAsync(mqttMessage);
                Console.WriteLine($"Message '{message}' sent to MQTT broker.");
            }
            else
            {
                Console.WriteLine("MQTT client is not connected.");
            }
        }

        public async Task SendColourStateMQTT(string message)
        {
            if (_mqttClient.IsConnected)
            {
                var mqttMessage = new MqttApplicationMessageBuilder()
                    .WithTopic("base/state/colour")
                    .WithPayload(message)
                    .Build();

                await _mqttClient.PublishAsync(mqttMessage);
                Console.WriteLine($"Message '{message}' sent to MQTT broker.");
            }
            else
            {
                Console.WriteLine("MQTT client is not connected.");
            }
        }

        public async Task SendBrightnessStateMQTT(string message)
        {
            if (_mqttClient.IsConnected)
            {
                var mqttMessage = new MqttApplicationMessageBuilder()
                    .WithTopic("base/state/brightness")
                    .WithPayload(message)
                    .Build();

                await _mqttClient.PublishAsync(mqttMessage);
                Console.WriteLine($"Message '{message}' sent to MQTT broker.");
            }
            else
            {
                Console.WriteLine("MQTT client is not connected.");
            }
        }
    }
}