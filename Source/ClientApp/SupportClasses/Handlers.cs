using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using ClientApp.SystemClasses;

namespace ClientApp.SupportClasses
{
    public static class Handlers
    {
        public static void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is DataGridRow row)
            {
                var temp = CardFactory.CreateTab(Guid.Parse(((DataRowView) row.Item).Row.ItemArray[0].ToString()));
                if (temp!=null)
                {
                    if (SystemSingleton.CurrentSession.TabCards.ContainsKey(temp.Card.Task.Number))
                    {
                        EnvironmentHelper.SendDialogBox(
                            (string)SystemSingleton.Configuration.mainWindow.FindResource("m_AlreadyOpened"),
                            "Attention"
                        );
                    }
                    else
                    {
                        SystemSingleton.CurrentSession.TabCards.Add(temp.Card.Task.Number, temp);
                        //TODO: переход на вкладку
                    }
                }
            }
        }

        public static void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MainWindow wnd = SystemSingleton.Configuration.mainWindow;
            if (wnd.TabControl.SelectedIndex > 1)
            {
                wnd.Update.Visibility = Visibility.Collapsed;
            }
            else if(wnd.TabControl.SelectedIndex == 1)
            {
                wnd.Update.Visibility = Visibility.Visible;
            }
            else
            {
                wnd.Update.Visibility = Visibility.Collapsed;
            }
        }
    }
}
