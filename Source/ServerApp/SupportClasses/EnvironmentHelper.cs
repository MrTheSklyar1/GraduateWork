using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using ServerApp.SystemClasses;

namespace ServerApp.SupportClasses
{
    public static class EnvironmentHelper
    {
        public static void CloseAllConnections()
        {
            foreach (var item in SystemSingleton.Configuration.SqlConnections)
            {
                if (item.State == ConnectionState.Closed) continue;
                item.Close();
            }
        }
        public static void SendLog(string log)
        {
            string logMessage = DateTime.UtcNow + " -- " + log;
            using (StreamWriter sw = File.AppendText("log.txt"))
            {
                sw.WriteLine(logMessage);
            }
            if (SystemSingleton.Configuration.ConsoleLog)
            {
                Application.Current.Dispatcher.BeginInvoke(new ThreadStart(delegate
                {
                    if(SystemSingleton.Configuration.ConsoleBox.LineCount>10000) SystemSingleton.Configuration.ConsoleBox.Clear();
                    SystemSingleton.Configuration.ConsoleBox.Text += "\n" + logMessage;
                    SystemSingleton.Configuration.ConsoleBox.ScrollToEnd();
                }));
            }
        }
        public static void SendFatalLog(string log)
        {
            string logMessage = DateTime.UtcNow + " -- " + log;
            using (StreamWriter sw = File.AppendText("log.txt"))
            {
                sw.WriteLine(logMessage);
            }
            if (SystemSingleton.Configuration.ConsoleLog)
            {
                Application.Current.Dispatcher.BeginInvoke(new ThreadStart(delegate
                {
                    if (SystemSingleton.Configuration.ConsoleBox.LineCount > 10000) SystemSingleton.Configuration.ConsoleBox.Clear();
                    SystemSingleton.Configuration.ConsoleBox.Text += "\n" + logMessage;
                    SystemSingleton.Configuration.ConsoleBox.ScrollToEnd();
                }));
            }
            CloseAllConnections();
            Environment.Exit(1);
        }
        public static void SendLogSQL(string log)
        {
            if (SystemSingleton.Configuration.SQLLog)
            {
                using (StreamWriter sw = File.AppendText("log-sql.txt"))
                {
                    sw.WriteLine(DateTime.UtcNow + " -- " + log);
                }
            }
        }
    }
}
