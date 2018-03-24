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
                if (Configuration.Waiters.Count == 0) return;
                try
                {
                    using (StreamWriter sw = File.CreateText("waiters.list"))
                    {
                        foreach (var item in Configuration.Waiters)
                        {
                            sw.WriteLine(item.Key + "|" + item.Value.Login);
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

            public static void LoadWaiters()
            {
                Configuration.Waiters = new Dictionary<long, Waiter>();
                if (File.Exists("waiters.hash") && File.Exists("waiters.list"))
                {
                    var sBuilder = new StringBuilder();
                    using (var md5Hash = MD5.Create())
                    {
                        var data = md5Hash.ComputeHash(File.ReadAllBytes("waiters.list"));
                        sBuilder = new StringBuilder();
                        foreach (var item in data)
                        {
                            sBuilder.Append(item.ToString("x2"));
                        }
                    }
                    if (sBuilder.ToString() == File.ReadAllText("waiters.hash"))
                    {
                        string[] lines = File.ReadAllLines("waiters.list");
                        foreach (var item in lines)
                        {
                            string[] parsed = item.Split(new char[] {'|'}, StringSplitOptions.None);
                            Configuration.Waiters.Add(Convert.ToInt64(parsed[0]), new Waiter
                            {
                                TelegramID = Convert.ToInt64(parsed[0]),
                                Login = parsed[1],
                                State = parsed[1]!=String.Empty?LoginWaitersState.WaitForPassword:LoginWaitersState.WaitForLogin
                            });
                        }
                    }
                    else
                    {
                        EnvironmentHelper.SendLog("File waiters corrupt, waiters will be null");
                    }
                }
                else
                {
                    EnvironmentHelper.SendLog("File waiters not exists, waiters will be empty");
                }
            }
        }
    }
}
