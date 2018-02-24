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

namespace ClientApp.View
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
        }

        #region Основные минорные функции LogIn
        private void SendAttentionToBottomBar(string placeholder)
        {
            Configuration.CurrentBottomBarLabelContent = placeholder;
            Configuration.CurrentBottomBarLabelBrush = Brushes.Red;
            Dispatcher.BeginInvoke(new ThreadStart(delegate
            {
                BottomBarLabel.Content = (String)FindResource(placeholder);
                BottomBarLabel.Foreground = Brushes.Red;
            }));
        }
        private void SendInfoToBottomBar(string placeholder)
        {
            Configuration.CurrentBottomBarLabelContent = placeholder;
            Configuration.CurrentBottomBarLabelBrush = Brushes.Black;
            Dispatcher.BeginInvoke(new ThreadStart(delegate
            {
                BottomBarLabel.Content = (String)FindResource(placeholder);
                BottomBarLabel.Foreground = Brushes.Black;
            }));
        }
        private void ClearBottomBar()
        {
            Configuration.CurrentBottomBarLabelContent = "";
            Configuration.CurrentBottomBarLabelBrush = Brushes.Black;
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
                using (var con = new SqlConnection(Configuration.ConnectionString))
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
        private void ChangeLanguageClick(Object sender, EventArgs e)
        {
            MenuItem mi = sender as MenuItem;
            if (mi != null)
            {
                CultureInfo lang = mi.Tag as CultureInfo;
                if (lang != null)
                {
                    App.Language = lang;
                }
            }

        }
        private void LanguageChanged(Object sender, EventArgs e)
        {
            CultureInfo currLang = App.Language;

            //Отмечаем нужный пункт смены языка как выбранный язык
            foreach (MenuItem i in menuLanguage.Items)
            {
                CultureInfo ci = i.Tag as CultureInfo;
                i.IsChecked = ci != null && ci.Equals(currLang);
            }
            BottomBarLabel.Content = Configuration.CurrentBottomBarLabelContent=="" ? "" : (String)FindResource(Configuration.CurrentBottomBarLabelContent);
            BottomBarLabel.Foreground = Configuration.CurrentBottomBarLabelBrush;
        }
        private void LogOff(object sender, RoutedEventArgs e)
        {
            if (CurrentSession.Login != "")
            {
                LoginTab.Visibility = Visibility.Visible;
                TabControl.SelectedIndex = 0;
                SendInfoToBottomBar("m_tab_LogIn_LogOffCompleted");
                EnvironmentHelper.SendLog("Log Off - " + CurrentSession.Login);
                CurrentSession.CloseSession();
                //TODO: Отключать остальные вкладки, все
                LogOffItem.Visibility = Visibility.Collapsed;
                WorkingTab.Visibility = Visibility.Collapsed;
                menuLanguage.Visibility = Visibility.Visible;
                Update.Visibility = Visibility.Collapsed;
                Open.Visibility = Visibility.Collapsed;
                TabWorkControl.Items.Clear();
            }
        }
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (LoginBox.Text != "" && PassBox.Password != "")
            {
                string hash = "";
                string hashfromsql = "";
                using (MD5 md5Hash = MD5.Create())
                {
                    byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(PassBox.Password));
                    StringBuilder sBuilder = new StringBuilder();
                    for (int i = 0; i < data.Length; i++)
                    {
                        sBuilder.Append(data[i].ToString("x2"));
                    }
                    hash = sBuilder.ToString();
                }
                using (var con = new SqlConnection(Configuration.ConnectionString))
                {
                    using (var command = new SqlCommand("select ID, PassWord, isnull(TelegramID, 0), FirstName, LastName from PersonalRoles where Login='" + LoginBox.Text + "';", con))
                    {
                        con.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                CurrentSession.ID = reader.GetGuid(0);
                                hashfromsql = reader.GetString(1);
                                CurrentSession.TelegramID = reader.GetInt32(2);
                                CurrentSession.FirstName = reader.GetString(3);
                                CurrentSession.LastName = reader.GetString(4);
                            }
                        }
                        con.Close();
                    }
                }
                if (CurrentSession.ID == Guid.Empty || hash != hashfromsql)
                {
                    SendAttentionToBottomBar("m_tab_LogIn_PasWrong");
                    CurrentSession.CloseSession();
                }
                else
                {
                    CurrentSession.Login = LoginBox.Text;
                    PassBox.Clear();
                    ClearBottomBar();
                    LogOffItem.Visibility = Visibility.Visible;
                    WorkingTab.Visibility = Visibility.Visible;
                    Update.Visibility = Visibility.Visible;
                    TabControl.SelectedIndex = 1;
                    LoginTab.Visibility = Visibility.Collapsed;
                    menuLanguage.Visibility = Visibility.Collapsed;
                    Open.Visibility = Visibility.Visible;
                    EnvironmentHelper.SendLog("Log In - " + CurrentSession.Login);
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
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            
        }
        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (TabControl.SelectedIndex == 1)
            {
                EnvironmentHelper.UpdateView(TabControl);
            }
            else
            {
                EnvironmentHelper.UpdateSelected((TabItem)TabControl.Items.GetItemAt(TabControl.SelectedIndex));
            }
        }
        private void Open_Click(object sender, RoutedEventArgs e)
        {
            if (TabControl.SelectedIndex == 1)
            {
                TabItem item = (TabItem)TabWorkControl.Items.GetItemAt(TabWorkControl.SelectedIndex);
                DataGrid grid = (DataGrid)item.Content;
                try
                {
                    DataRowView drv = (DataRowView)grid.SelectedItem;
                    String result = (drv["Data"]).ToString();
                    MessageBox.Show(result);
                }
                catch
                {
                    //TODO: error
                }
            }
        }
        #endregion


    }
}
