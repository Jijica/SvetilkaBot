using System;
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

        static Config()
        {
            var envVar = Environment.GetEnvironmentVariable("SqlConnectionStringLocal", EnvironmentVariableTarget.User);
            SqlConnectionString = envVar;
            envVar = Environment.GetEnvironmentVariable("MQTT_ID", EnvironmentVariableTarget.User);
            MqttId = envVar;
        }
    }
}
