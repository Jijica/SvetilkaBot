using Dapper;
using Npgsql;

namespace SvetilkaBot.Services
{
    internal class DBService
    {
        public static void InitializeChat(long chatID)
        {
            var query = @"
                MERGE INTO botuser AS target
                USING (VALUES (@chatid)) AS source (chatid)
                ON target.chatid = source.chatid
                WHEN MATCHED THEN
                    UPDATE SET
                        menustate = @menustate,
                        ledstate = @ledstate,
                        rgbstate = @rgbstate,
                        brightnessstate = @brightnessstate
                WHEN NOT MATCHED THEN
                    INSERT (chatid, menustate, ledstate, rgbstate, brightnessstate)
                    VALUES (@chatid, @menustate, @ledstate, @rgbstate, @brightnessstate);
    ";

            using (var conn = new NpgsqlConnection(Config.SqlConnectionString))
            {
                conn.Execute(query, new { chatid = chatID, menustate = "NoState", ledstate = "OFF", rgbstate = "White", brightnessstate = "25%" });
            }
        }
        public static void SetMenuState(long chatID, string state)
        {
            var query = @"update botuser set menustate = @menustate where chatid = @chatid";

            using (var conn = new NpgsqlConnection(Config.SqlConnectionString))
            {
                conn.Execute(query, new { chatid = chatID, menustate = state });
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

        public static void SetColourState(long chatID, string state)
        {
            var query = @"update botuser set rgbstate = @rgbstate where chatid = @chatid";

            using (var conn = new NpgsqlConnection(Config.SqlConnectionString))
            {
                conn.Execute(query, new { chatid = chatID, rgbstate = state });
            }
        }

        public static void SetBrightnessState(long chatID, string state)
        {
            var query = @"update botuser set brightnessstate = @brightnessstate where chatid = @chatid";

            using (var conn = new NpgsqlConnection(Config.SqlConnectionString))
            {
                conn.Execute(query, new { chatid = chatID, brightnessstate = state });
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
        public static string GetColourState(long chatID)
        {
            var query = @"select rgbstate from botuser where chatid = @chatid";
            using (var conn = new NpgsqlConnection(Config.SqlConnectionString))
            {
                return conn.QueryFirstOrDefault<string>(query, new { chatid = chatID });
            }
        }

        public static string GetBrightnessState(long chatID)
        {
            var query = @"select brightnessstate from botuser where chatid = @chatid";
            using (var conn = new NpgsqlConnection(Config.SqlConnectionString))
            {
                return conn.QueryFirstOrDefault<string>(query, new { chatid = chatID });
            }
        }
    }
}