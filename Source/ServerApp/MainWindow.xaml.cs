using ServerApp.Elements;
using ServerApp.SupportClasses;
using ServerApp.SystemClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ServerApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SystemSingleton.Configuration.ConsoleBox = ConsoleBox;
            SystemSingleton.Configuration.Window = this;
            SystemSingleton.Configuration.SqlConnections = new List<SqlConnection>();
            if (!XMLConfiguration.Load("settings.xml"))
            {
                EnvironmentHelper.SendFatalLog("Broken Settings File");
            }
            App.Language = new CultureInfo(SystemSingleton.Configuration.Language);
            SystemSingleton.Configuration.Waiters = new Dictionary<long, Waiter>();
            workerConnectionToBase.DoWork += WorkerConnectionToBaseOnDoWork;
            workerConnectionToBase.RunWorkerAsync();
            workerBot.DoWork += WorkerBotOnDoWork;
        }
        private static readonly BackgroundWorker workerConnectionToBase = new BackgroundWorker();
        private static readonly BackgroundWorker workerBot = new BackgroundWorker();

        private async void WorkerConnectionToBaseOnDoWork(object sender, DoWorkEventArgs e)
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
        private async void WorkerBotOnDoWork(object sender, DoWorkEventArgs e)
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

        private void Window_Closed(object sender, EventArgs e)
        {
            EnvironmentHelper.CloseAllConnections();
        }

        private void About_OnClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("ServerApp - " + Assembly.GetEntryAssembly().GetName().Version.ToString() + "\n\nCreatedBy - Sklyarov Nikita\n\nOrganisation - MAI", (string)FindResource("m_menu_About"), MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
