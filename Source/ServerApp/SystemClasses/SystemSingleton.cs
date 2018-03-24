using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using ServerApp.Elements;
using ServerApp.SupportClasses;
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
            public static string Language { get; set; }
            public static List<SqlConnection> SqlConnections { get; set; }
            public static TBot Bot { get; set; }
            public static TextBox ConsoleBox { get; set; }
            public static MainWindow Window { get; set; }
            public static Dictionary<long, Waiter> Waiters { get; set; }
        }
        public static class WaitersWorker
        {
            public static void SaveWaiters()
            {
                if (Configuration.Waiters.Count > 0)
                {
                    try
                    {
                        using (StreamWriter sw = File.CreateText("waiters.list"))
                        {
                            foreach (var item in Configuration.Waiters)
                            {
                                sw.WriteLine(item.Key + "|" + item.Value.TelegramID + "|" + item.Value.Login);
                            }
                        }
                        using (var md5Hash = MD5.Create())
                        {
                            var data = md5Hash.ComputeHash(File.ReadAllBytes("waiters.list"));
                            var sBuilder = new StringBuilder();
                            foreach (var item in data)
                            {
                                sBuilder.Append(item.ToString("x2"));
                            }
                            File.WriteAllText("waiters.hash", sBuilder.ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        EnvironmentHelper.SendLog(ex.Message);
                    }
                }
            }
        }
    }
}
