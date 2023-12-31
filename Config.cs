﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SvetilkaBot
{
    internal class Config
    {
        public readonly static string SqlConnectionString;
        public readonly static string MqttId;
        public readonly static string TelegramToken;

        static Config()
        {
            var envVar = Environment.GetEnvironmentVariable("SqlConnectionStringElephant", EnvironmentVariableTarget.User);
            SqlConnectionString = envVar;
            envVar = Environment.GetEnvironmentVariable("MQTT_ID", EnvironmentVariableTarget.User);
            MqttId = envVar;
            envVar = Environment.GetEnvironmentVariable("SvetilkaBotToken", EnvironmentVariableTarget.User);
            TelegramToken = envVar;
        }
    }
}
