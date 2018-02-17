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

namespace ClientApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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
            ConnectToDatabase();
        }

        private void ConnectToDatabase()
        {
            Configuration config = PersistableObject.Load<Configuration>("settings.xml");
            if (config == null)
            {
                BottomBarLabel.Content = (String)FindResource("m_tab_LogIn_BrokenSettingsFile");
                EnvironmentHelper.SendLog("Broken Settings File");
                config = new Configuration();
                config.ConnectionString = "Type connection string here";
                config.Save<Configuration>("settings.xml");
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
        }
        private void LogOff(object sender, RoutedEventArgs e)
        {
            TabControl.SelectedIndex = 0;
            LogOffItem.Visibility = Visibility.Collapsed;
            BottomBarLabel.Content = (String)FindResource("m_tab_LogIn_LogOffCompleted");
            EnvironmentHelper.SendLog("Log Off - " + MainDataHolder.CurrentSessionLogin);
            MainDataHolder.CurrentSessionLogin = "";
            BottomBarLabel.Foreground = Brushes.Black;
            //TODO: Отключать остальные вкладки, все
        }
    }
}
