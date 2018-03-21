using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServerApp.Elements;
using ServerApp.LanguageWorker;
using ServerApp.SupportClasses;
using ServerApp.SystemClasses;

namespace ServerApp
{
    class Program
    {
        private static readonly BackgroundWorker workerConnectionToBase = new BackgroundWorker();
        private static readonly BackgroundWorker workerBot = new BackgroundWorker();
        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(Closed);
            SystemSingleton.Configuration.SqlConnections = new List<SqlConnection>();
            if (!XMLConfiguration.Load("settings.xml"))
            {
                EnvironmentHelper.SendFatalLog("Broken Settings File");
            }
            if (!XMLLanguageConditions.Load("lang."+SystemSingleton.Configuration.LangInfo+".xml"))
            {
                EnvironmentHelper.SendFatalLog("Broken Language File");
            }
            workerConnectionToBase.DoWork += WorkerConnectionToBaseOnDoWork;
            workerConnectionToBase.RunWorkerAsync();
            workerBot.DoWork += WorkerBotOnDoWork;
        }

        private static void Closed(object sender, EventArgs e)
        {
            EnvironmentHelper.CloseAllConnections();
        }
        private static void WorkerConnectionToBaseOnDoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                using (var con = new SqlConnection(SystemSingleton.Configuration.ConnectionString))
                {
                    SystemSingleton.Configuration.SqlConnections.Add(con);
                    con.Open();
                    con.Close();
                }
                workerBot.RunWorkerAsync();
            }
            catch (ArgumentException ex)
            {
                EnvironmentHelper.SendFatalLog(ex.Message);
            }
            catch (Exception ex)
            {
                EnvironmentHelper.SendFatalLog(ex.Message);
            }
        }
        private static void WorkerBotOnDoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                SystemSingleton.Configuration.Bot = new TBot();
            }
            catch (Telegram.Bot.Exceptions.ApiRequestException ex)
            {
                EnvironmentHelper.SendFatalLog(ex.Message);
            }
        }
    }
}
