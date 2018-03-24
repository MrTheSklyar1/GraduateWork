using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using ServerApp.Elements;
using Telegram.Bot;

namespace ServerApp.SystemClasses
{
    public static class SystemSingleton
    {
        public static class Configuration
        {
            public static string ConnectionString { get; set; }
            public static bool SQLLog { get; set; }
            public static bool ConsoleLog { get; set; }
            public static string FilesPath { get; set; }
            public static string ApiKey { get; set; }
            public static string LangInfo { get; set; }
            public static List<SqlConnection> SqlConnections { get; set; }
            public static TBot Bot { get; set; }
            public static TextBox ConsoleBox { get; set; }
            public static MainWindow Window { get; set; }
        }
    }
}
