using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
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
                Console.WriteLine(logMessage);
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
                Console.WriteLine(logMessage);
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
