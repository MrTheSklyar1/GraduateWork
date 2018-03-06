using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Globalization;
using System.IO;
using ClientApp.SupportClasses;
using System.Data.SqlClient;
using System.ComponentModel;
using System.Threading;
using System.Security.Cryptography;
using System.Data;
using ClientApp.SystemClasses;
using System.Reflection;

namespace ClientApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly BackgroundWorker worker = new BackgroundWorker();
        public MainWindow()
        {
            InitializeComponent();
            SystemSingleton.Configuration.mainWindow = this;
            SystemSingleton.Configuration.tabControl = TabControl;
            App.LanguageChanged += LanguageChanged;
            CultureInfo currLang = App.Language;
            menuLanguage.Items.Clear();
            foreach (var lang in App.Languages)
            {
                MenuItem menuLang = new MenuItem();
                menuLang.Header = lang.DisplayName;
                menuLang.Tag = lang;
                menuLang.IsChecked = lang.Equals(currLang);
                menuLang.Click += ChangeLanguageClick;
                menuLanguage.Items.Add(menuLang);
            }
            if (!XMLConfiguration.Load("settings.xml"))
            {
                SendAttentionToBottomBar("m_tab_LogIn_BrokenSettingsFile");
                EnvironmentHelper.SendLog("Broken Settings File");
                TabControl.IsEnabled = false;
                return;
            }
            worker.DoWork += worker_CheckConnection;
            WorkingTab.Visibility = Visibility.Collapsed;
            SendInfoToBottomBar("m_tab_LogIn_CheckConnection");
            TabControl.IsEnabled = false;
            worker.RunWorkerAsync();
            TabControl.SelectionChanged += Handlers.TabControl_SelectionChanged;
        }

        #region Основные минорные функции LogIn
        private void SendAttentionToBottomBar(string placeholder)
        {
            SystemSingleton.BotomTab.CurrentBottomBarLabelContent = placeholder;
            SystemSingleton.BotomTab.CurrentBottomBarLabelBrush = Brushes.Red;
            Dispatcher.BeginInvoke(new ThreadStart(delegate
            {
                BottomBarLabel.Content = (String)FindResource(placeholder);
                BottomBarLabel.Foreground = Brushes.Red;
            }));
        }
        private void SendInfoToBottomBar(string placeholder)
        {
            SystemSingleton.BotomTab.CurrentBottomBarLabelContent = placeholder;
            SystemSingleton.BotomTab.CurrentBottomBarLabelBrush = Brushes.Black;
            Dispatcher.BeginInvoke(new ThreadStart(delegate
            {
                BottomBarLabel.Content = (string)FindResource(placeholder);
                BottomBarLabel.Foreground = Brushes.Black;
            }));
        }
        private void ClearBottomBar()
        {
            SystemSingleton.BotomTab.CurrentBottomBarLabelContent = "";
            SystemSingleton.BotomTab.CurrentBottomBarLabelBrush = Brushes.Black;
            Dispatcher.BeginInvoke(new ThreadStart(delegate
            {
                BottomBarLabel.Content = "";
                BottomBarLabel.Foreground = Brushes.Black;
            }));
        }
        private void worker_CheckConnection(object sender, DoWorkEventArgs e)
        {
            try
            {
                using (var con = new SqlConnection(SystemSingleton.Configuration.ConnectionString))
                {
                    con.Open();
                    con.Close();
                }
                ClearBottomBar();
                Dispatcher.BeginInvoke(new ThreadStart(delegate
                {
                    TabControl.IsEnabled = true;
                }));
            }
            catch (ArgumentException ex)
            {
                EnvironmentHelper.SendLog(ex.Message);
                SendAttentionToBottomBar("m_tab_LogIn_BrokenSettingsFile");
            }
            catch (Exception ex)
            {
                EnvironmentHelper.SendLog(ex.Message);
                SendAttentionToBottomBar("m_tab_LogIn_NoConnection");
            }
        }
        private void ChangeLanguageClick(object sender, EventArgs e)
        {
            if (sender is MenuItem mi)
            {
                if (mi.Tag is CultureInfo lang)
                {
                    App.Language = lang;
                }
            }

        }
        private void LanguageChanged(object sender, EventArgs e)
        {
            CultureInfo currLang = App.Language;

            //Отмечаем нужный пункт смены языка как выбранный язык
            foreach (MenuItem i in menuLanguage.Items)
            {
                i.IsChecked = i.Tag is CultureInfo ci && ci.Equals(currLang);
            }
            BottomBarLabel.Content = SystemSingleton.BotomTab.CurrentBottomBarLabelContent == "" ? "" : (String)FindResource(SystemSingleton.BotomTab.CurrentBottomBarLabelContent);
            BottomBarLabel.Foreground = SystemSingleton.BotomTab.CurrentBottomBarLabelBrush;
        }
        private void LogOff(object sender, RoutedEventArgs e)
        {
            if (SystemSingleton.CurrentSession.Login != "")
            {
                LoginTab.Visibility = Visibility.Visible;
                TabControl.SelectedIndex = 0;
                SendInfoToBottomBar("m_tab_LogIn_LogOffCompleted");
                EnvironmentHelper.SendLog("Log Off - " + SystemSingleton.CurrentSession.Login);
                SystemSingleton.CurrentSession.CloseSession();
                LogOffItem.Visibility = Visibility.Collapsed;
                WorkingTab.Visibility = Visibility.Collapsed;
                menuLanguage.Visibility = Visibility.Visible;
                EnvironmentHelper.CloseAllEditingTabs();
                TabWorkControl.Items.Clear();
                while (TabControl.Items.Count > 2)
                {
                    TabControl.Items.RemoveAt(TabControl.Items.Count-1);
                }
            }
        }
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (LoginBox.Text != "" && PassBox.Password != "")
            {
                var hash = "";
                var hashfromsql = "";
                using (var md5Hash = MD5.Create())
                {
                    var data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(PassBox.Password));
                    var sBuilder = new StringBuilder();
                    foreach (var item in data)
                    {
                        sBuilder.Append(item.ToString("x2"));
                    }
                    hash = sBuilder.ToString();
                }
                try
                {
                    using (var con = new SqlConnection(SystemSingleton.Configuration.ConnectionString))
                    {
                        using (var command = new SqlCommand(SqlCommands.LoginCommand, con))
                        {
                            command.Parameters.Add("@LoginText", SqlDbType.NVarChar);
                            command.Parameters["@LoginText"].Value = LoginBox.Text;
                            EnvironmentHelper.SendLogSQL(command.CommandText);
                            con.Open();
                            using (var reader = command.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    SystemSingleton.CurrentSession.ID = reader.GetGuid(0);
                                    hashfromsql = reader.GetString(1);
                                    SystemSingleton.CurrentSession.TelegramID = reader.GetInt32(2);
                                    SystemSingleton.CurrentSession.FirstName = reader.GetString(3);
                                    SystemSingleton.CurrentSession.LastName = reader.GetString(4);
                                    SystemSingleton.CurrentSession.FullName = reader.GetString(5);
                                }
                            }
                            con.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    EnvironmentHelper.SendErrorDialogBox(ex.Message, "SQL Error", ex.StackTrace);
                }
                if (SystemSingleton.CurrentSession.ID == Guid.Empty || hash != hashfromsql)
                {
                    SendAttentionToBottomBar("m_tab_LogIn_PasWrong");
                    SystemSingleton.CurrentSession.CloseSession();
                }
                else
                {
                    SystemSingleton.CurrentSession.Login = LoginBox.Text;
                    PassBox.Clear();
                    ClearBottomBar();
                    LogOffItem.Visibility = Visibility.Visible;
                    WorkingTab.Visibility = Visibility.Visible;
                    TabControl.SelectedIndex = 1;
                    LoginTab.Visibility = Visibility.Collapsed;
                    menuLanguage.Visibility = Visibility.Collapsed;
                    EnvironmentHelper.SendLog("Log In - " + SystemSingleton.CurrentSession.Login);
                    OnLogin();
                }
            }
            else
            {
                SendAttentionToBottomBar("m_tab_LogIn_LogOrPassNotTyped");
            }
        }
        private void PassBox_EnterPressed(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LoginButton_Click(sender, e);
            }
        }
        private void OnLogin()
        {
            EnvironmentHelper.FindAllRoles();
            EnvironmentHelper.SetWorkingPlace(TabWorkControl, this);
        }
        #endregion
        #region Контекстные клавиши
        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            EnvironmentHelper.UpdateView();
        }

        #endregion

        private void Window_Closed(object sender, EventArgs e)
        {
            if (SystemSingleton.CurrentSession.Login != "")
            {
                EnvironmentHelper.CloseAllEditingTabs();
            }
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("ClientApp - "+ Assembly.GetEntryAssembly().GetName().Version.ToString() + "\n\nCreatedBy - Sklyarov Nikita\n\nOrganisation - MAI", (string)FindResource("m_menu_About"), MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
