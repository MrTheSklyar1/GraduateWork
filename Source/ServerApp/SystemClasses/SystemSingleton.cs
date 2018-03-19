using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerApp.SystemClasses
{
    public static class SystemSingleton
    {
        public static class CurrentSession
        {
            public static void CloseSession()
            {
                Login = "";
                ID = Guid.Empty;
                TelegramID = 0;
                FirstName = "";
                LastName = "";
                FullName = "";
            }
            public static string Login;
            public static Guid ID;
            public static long TelegramID;
            public static string FirstName;
            public static string LastName;
            public static string FullName;
        }
        public static class Configuration
        {
            public static string ConnectionString { get; set; }
            public static bool SQLLog { get; set; }
            public static bool ConsoleLog { get; set; }
            public static string FilesPath { get; set; }
            public static string ApiKey { get; set; }
            public static string LangInfo { get; set; }
            public static List<SqlConnection> SqlConnections { get; set; }
            public static Bot Bot { get; set; }
        }
    }
}
