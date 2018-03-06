using AdminApp.Elements;
using AdminApp.SystemClasses;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AdminApp.SupportClasses
{
    public static class EnvironmentHelper
    {
        public static void CreateNew()
        {
            switch (SystemSingleton.Configuration.mainWindow.TabWorkControl.SelectedIndex)
            {
                case 0://PersonalRole
                    MessageBox.Show("PRN");
                    break;
                case 1://StaticRole
                    MessageBox.Show("SRN");
                    break;
                case 2://DocTypes
                    MessageBox.Show("DTN");
                    break;
            }
        }

        public static void SendErrorDialogBox(string message, string header, string trace)
        {
            MessageBox.Show(message, header, MessageBoxButton.OK, MessageBoxImage.Error);
            using (StreamWriter sw = File.AppendText("log.txt"))
            {
                sw.WriteLine(DateTime.UtcNow + " -- " + message + "\n\n" + trace);
            }
            //TODO: Закрытие работы со вкладками
            Application.Current.Shutdown(1);
        }

        public static void SendDialogBox(string message, string header)
        {
            MessageBox.Show(message, header, MessageBoxButton.OK, MessageBoxImage.Information);
            using (StreamWriter sw = File.AppendText("log.txt"))
            {
                sw.WriteLine(DateTime.UtcNow + " -- " + message);
            }
        }
        public static void SendLog(string log)
        {
            using (StreamWriter sw = File.AppendText("log.txt"))
            {
                sw.WriteLine(DateTime.UtcNow + " -- " + log);
            }
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
        public static void Error(Guid CardID)
        {
            SendDialogBox(
                (string)SystemSingleton.Configuration.mainWindow.FindResource("m_CardViewNotCreated") + "\n\n" + CardID.ToString(),
                "Card Error"
            );
        }
        public static void SetWorkingPlace(TabControl tabControl, Window window)
        {
            var tempItem = new STabItem();
            tempItem.TabItem = new TabItem
            {
                Header = (String)window.FindResource("m_tab_PersonalRole"),
                Name = StaticTypes.PersonalRoleTab,
                Height = 40,
                FontSize = 14
            };
            tempItem.DataGrid = new DataGrid
            {
                Name = StaticTypes.PersonalRoleGrid,
                SelectionMode = DataGridSelectionMode.Single,
                CanUserAddRows = false,
                CanUserDeleteRows = false,
                IsReadOnly = true
            };
            SetInfoToGridPersonalRole(ref tempItem.DataGrid);
            tempItem.TabItem.Content = tempItem.DataGrid;
            SystemSingleton.CurrentSession.TabItems.Add(StaticTypes.PersonalRoleTab, tempItem);
            tabControl.Items.Add(tempItem.TabItem);
            

            tempItem = new STabItem();
            tempItem.TabItem = new TabItem
            {
                Header = (String)window.FindResource("m_tab_StaticRole"),
                Name = StaticTypes.StaticRoleTab,
                Height = 40,
                FontSize = 14
            };
            tempItem.DataGrid = new DataGrid
            {
                Name = StaticTypes.StaticRoleGrid,
                SelectionMode = DataGridSelectionMode.Single,
                CanUserAddRows = false,
                CanUserDeleteRows = false,
                IsReadOnly = true
            };
            SetInfoToGridStaticRole(ref tempItem.DataGrid);
            tempItem.TabItem.Content = tempItem.DataGrid;
            SystemSingleton.CurrentSession.TabItems.Add(StaticTypes.StaticRoleTab, tempItem);
            tabControl.Items.Add(tempItem.TabItem);

            tempItem = new STabItem();
            tempItem.TabItem = new TabItem
            {
                Header = (String)window.FindResource("m_tab_DocType"),
                Name = StaticTypes.DocTypeTab,
                Height = 40,
                FontSize = 14
            };
            tempItem.DataGrid = new DataGrid
            {
                Name = StaticTypes.DocTypeGrid,
                SelectionMode = DataGridSelectionMode.Single,
                CanUserAddRows = false,
                CanUserDeleteRows = false,
                IsReadOnly = true
            };
            SetInfoToGridDocType(ref tempItem.DataGrid);
            tempItem.TabItem.Content = tempItem.DataGrid;
            SystemSingleton.CurrentSession.TabItems.Add(StaticTypes.DocTypeTab, tempItem);
            tabControl.Items.Add(tempItem.TabItem);

            foreach (var item in SystemSingleton.CurrentSession.TabItems)
            {
                if (item.Key == StaticTypes.PersonalRoleTab)
                {
                    Style rowStyle = new Style(typeof(DataGridRow));
                    rowStyle.Setters.Add(new EventSetter(DataGridRow.MouseDoubleClickEvent,
                        new MouseButtonEventHandler(Handlers.RowPersonalRole_DoubleClick)));
                    item.Value.DataGrid.RowStyle = rowStyle;
                }
                else if (item.Key == StaticTypes.StaticRoleTab)
                {
                    Style rowStyle = new Style(typeof(DataGridRow));
                    rowStyle.Setters.Add(new EventSetter(DataGridRow.MouseDoubleClickEvent,
                        new MouseButtonEventHandler(Handlers.RowStaticRole_DoubleClick)));
                    item.Value.DataGrid.RowStyle = rowStyle;
                }
                else if (item.Key == StaticTypes.DocTypeTab)
                {
                    Style rowStyle = new Style(typeof(DataGridRow));
                    rowStyle.Setters.Add(new EventSetter(DataGridRow.MouseDoubleClickEvent,
                        new MouseButtonEventHandler(Handlers.RowDocType_DoubleClick)));
                    item.Value.DataGrid.RowStyle = rowStyle;
                }
                item.Value.DataGrid.AutoGeneratedColumns += (sender, args) =>
                {
                    item.Value.DataGrid.Columns[0].Visibility = Visibility.Collapsed;
                    SystemSingleton.CurrentSession.SetCaptionToGrid(window, item);
                };
            }
        }

        public static void SetInfoToGridPersonalRole(ref DataGrid dataGrid)
        {
            try
            {
                SqlConnection con = new SqlConnection(SystemSingleton.Configuration.ConnectionString);
                SqlCommand cmd = new SqlCommand(SqlCommands.SetInfoToGridPersonalRoles, con);
                EnvironmentHelper.SendLogSQL(cmd.CommandText);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("PersonalRoles");
                sda.Fill(dt);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    dt.Rows[i][5] = (string)SystemSingleton.Configuration.mainWindow.FindResource((string)dt.Rows[i][5]);
                }
                dataGrid.ItemsSource = dt.DefaultView;
            }
            catch (Exception ex)
            {
                EnvironmentHelper.SendErrorDialogBox(ex.Message, "SQL Error", ex.StackTrace);
            }
        }
        public static void SetInfoToGridStaticRole(ref DataGrid dataGrid)
        {
            try
            {
                SqlConnection con = new SqlConnection(SystemSingleton.Configuration.ConnectionString);
                SqlCommand cmd = new SqlCommand(SqlCommands.SetInfoToGridStaticRoles, con);
                EnvironmentHelper.SendLogSQL(cmd.CommandText);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("StaticRoles");
                sda.Fill(dt);
                dataGrid.ItemsSource = dt.DefaultView;
            }
            catch (Exception ex)
            {
                EnvironmentHelper.SendErrorDialogBox(ex.Message, "SQL Error", ex.StackTrace);
            }
        }
        public static void SetInfoToGridDocType(ref DataGrid dataGrid)
        {
            try
            {
                SqlConnection con = new SqlConnection(SystemSingleton.Configuration.ConnectionString);
                SqlCommand cmd = new SqlCommand(SqlCommands.SetInfoToGridDocTypes, con);
                EnvironmentHelper.SendLogSQL(cmd.CommandText);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("DocTypes");
                sda.Fill(dt);
                dataGrid.ItemsSource = dt.DefaultView;
            }
            catch (Exception ex)
            {
                EnvironmentHelper.SendErrorDialogBox(ex.Message, "SQL Error", ex.StackTrace);
            }
        }
        public static void UpdateView()
        {
            SetInfoToGridPersonalRole(ref SystemSingleton.CurrentSession.TabItems[StaticTypes.PersonalRoleTab].DataGrid);
            SetInfoToGridStaticRole(ref SystemSingleton.CurrentSession.TabItems[StaticTypes.StaticRoleTab].DataGrid);
            SetInfoToGridDocType(ref SystemSingleton.CurrentSession.TabItems[StaticTypes.DocTypeTab].DataGrid);
        }
    }
}
