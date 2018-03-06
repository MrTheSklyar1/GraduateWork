using AdminApp.SystemClasses;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AdminApp.SupportClasses
{
    public static class Handlers
    {

        public static void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SystemSingleton.Configuration.mainWindow.Update.Visibility =
                SystemSingleton.Configuration.tabControl.SelectedIndex == 1 ?
                    Visibility.Visible : Visibility.Collapsed;
        }
    }
}
