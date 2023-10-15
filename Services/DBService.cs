using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Dapper;
using Npgsql;
using Telegram.Bot.Types;


namespace SvetilkaBot.Services
{
    internal class DBService
    {
        public static void InitializeChat(long chatID)
        {
            var query = @"
                        insert into botuser (chatid, menustate, ledstate, rgbstate, brightnessstate)
                        select @chatid, @menustate, @ledstate, @rgbstate, @brightnessstate
                        where not exists (select 1 from botuser where chatid = @chatid)
                        ";

            using (var conn = new NpgsqlConnection(Config.SqlConnectionString))
            {
                conn.Execute(query, new { chatid = chatID, menustate = "NoState", ledstate = "NoState", rgbstate = "NoState", brightnessstate = "NoState" });
            }

        }
        public static void SetMenuState(long chatID, string state)
        {
            var query = @"update botuser set menustate = @menustate where chatid = @chatid";

            using (var conn = new NpgsqlConnection(Config.SqlConnectionString))
            {
                conn.Execute(query, new { chatid = chatID, menustate = state});
            }
        }

        public static void SetLedState(long chatID, string state)
        {
            var query = @"update botuser set ledstate = @ledstate where chatid = @chatid";

            using (var conn = new NpgsqlConnection(Config.SqlConnectionString))
            {
                conn.Execute(query, new { chatid = chatID, ledstate = state });
            }
        }

        private static void SetRgbState(long chatID, string state)
        {
            var query = @"update botuser set rgbstate = @rgbstate where chatid = @chatid";

            using (var conn = new NpgsqlConnection(Config.SqlConnectionString))
            {
                conn.Execute(query, new { chatid = chatID, rgbstate = state });
            }
        }

        public static string GetMenuState(long chatID)
        {
            var query = @"select menustate from botuser where chatid = @chatid";
            using (var conn = new NpgsqlConnection(Config.SqlConnectionString))
            {
                return conn.QueryFirstOrDefault<string>(query, new { chatid = chatID });
            }
        }

        public static string GetLedState(long chatID)
        {
            var query = @"select ledstate from botuser where chatid = @chatid";
            using (var conn = new NpgsqlConnection(Config.SqlConnectionString))
            {
                return conn.QueryFirstOrDefault<string>(query, new { chatid = chatID });
            }
        }
        private string GetRgbState(long chatID)
        {
            var query = @"select rgbstate from botuser where chatid = @chatid";
            using (var conn = new NpgsqlConnection(Config.SqlConnectionString))
            {
                return conn.QueryFirstOrDefault<string>(query, new { chatid = chatID });
            }
        }
    }
}
