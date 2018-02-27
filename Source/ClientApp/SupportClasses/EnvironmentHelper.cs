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
using System.Windows.Documents;
using System.Windows.Input;
using ClientApp.MainClasses;

namespace ClientApp.SupportClasses
{
    public static class EnvironmentHelper
    {
        public static void SendErrorDialogBox(string message, string header, string trace)
        {
            MessageBox.Show(message, header, MessageBoxButton.OK, MessageBoxImage.Error);
            using (StreamWriter sw = File.AppendText("log.txt"))
            {
                sw.WriteLine(DateTime.UtcNow + " -- " + message + "\n\n" + trace);
            }
            Application.Current.Shutdown(1);
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
        public static void FindAllRoles()
        {
            SystemSingleton.CurrentSession.UserRoles.Clear();
            try
            {
                using (var con = new SqlConnection(SystemSingleton.Configuration.ConnectionString))
                {
                    using (var command = new SqlCommand("select ru.RoleID, r.Name, r.Caption from RoleUsers ru join StaticRoles r on ru.RoleID=r.ID where ru.PersonID='" + SystemSingleton.CurrentSession.ID + "';", con))
                    {
                        EnvironmentHelper.SendLogSQL(command.CommandText);
                        con.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var temp = new Role();
                                temp.ID = reader.GetGuid(0);
                                temp.Name = reader.GetString(1);
                                temp.Caption = reader.GetString(2);
                                SystemSingleton.CurrentSession.UserRoles.Add(temp);
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
        }
        public static void SetWorkingPlace(TabControl tabControl, Window window)
        {
            var tempItem = new STabItem();
            tempItem.TabItem = new TabItem
            {
                Header = (String)window.FindResource("m_tab_WorkingTab_CurrentWork"),
                Name = StaticTypes.CurrentWorkTab,
                Height = 40,
                FontSize = 14
            };
            tempItem.DataGrid = new DataGrid
            {
                Name = StaticTypes.CurrentWorkGrid,
                SelectionMode = DataGridSelectionMode.Single,
                CanUserAddRows = false,
                CanUserDeleteRows = false,
                IsReadOnly = true
            };
            SetInfoToGridWork(ref tempItem.DataGrid);
            tempItem.TabItem.Content = tempItem.DataGrid;
            SystemSingleton.CurrentSession.TabItems.Add(StaticTypes.CurrentWorkTab, tempItem);
            tabControl.Items.Add(tempItem.TabItem);

            tempItem = new STabItem();
            tempItem.TabItem = new TabItem
            {
                Header = (String)window.FindResource("m_tab_WorkingTab_CompletedWork"),
                Name = StaticTypes.CompletedWorkTab,
                Height = 40,
                FontSize = 14
            };
            tempItem.DataGrid = new DataGrid
            {
                Name = StaticTypes.CompletedWorkGrid,
                SelectionMode = DataGridSelectionMode.Single,
                CanUserAddRows = false,
                CanUserDeleteRows = false,
                IsReadOnly = true
            };
            SetInfoToGridEndWork(ref tempItem.DataGrid);
            tempItem.TabItem.Content = tempItem.DataGrid;
            SystemSingleton.CurrentSession.TabItems.Add(StaticTypes.CompletedWorkTab, tempItem);
            tabControl.Items.Add(tempItem.TabItem);

            foreach (var item in SystemSingleton.CurrentSession.UserRoles)
            {
                tempItem = new STabItem();
                if (item.Name == StaticTypes.PersonalRole)
                {
                    tempItem.TabItem = new TabItem
                    {
                        Header = (String)window.FindResource("m_tab_" + StaticTypes.PersonalRole),
                        Name = item.Name,
                        Height = 40,
                        FontSize = 14
                    };
                }
                else
                {
                    tempItem.TabItem = new TabItem
                    {
                        Header = item.Caption,
                        Name = item.Name,
                        Height = 40,
                        FontSize = 14
                    };
                }
                tempItem.DataGrid = new DataGrid
                {
                    Name = item.Name,
                    SelectionMode = DataGridSelectionMode.Single,
                    CanUserAddRows = false,
                    CanUserDeleteRows = false,
                    IsReadOnly = true
                };
                SetInfoToGridOther(ref tempItem.DataGrid, item.ID);
                tempItem.TabItem.Content = tempItem.DataGrid;
                SystemSingleton.CurrentSession.TabItems.Add(item.Name, tempItem);
                tabControl.Items.Add(tempItem.TabItem);
            }
            foreach (var item in SystemSingleton.CurrentSession.TabItems)
            {
                Style rowStyle = new Style(typeof(DataGridRow));
                rowStyle.Setters.Add(new EventSetter(DataGridRow.MouseDoubleClickEvent,
                                         new MouseButtonEventHandler(Row_DoubleClick)));
                item.Value.DataGrid.RowStyle = rowStyle;
                item.Value.DataGrid.AutoGeneratedColumns += (sender, args) =>
                {
                    item.Value.DataGrid.Columns[0].Visibility = Visibility.Collapsed;
                    SystemSingleton.CurrentSession.SetCaptionToGrid(window, item);
                };
            }
        }

        public static void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGridRow row = sender as DataGridRow;
            MessageBox.Show(((DataRowView)row.Item).Row.ItemArray[0].ToString());
        }

        public static void SetInfoToGridWork(ref DataGrid dataGrid)
        {
            try
            {

                SqlConnection con = new SqlConnection(SystemSingleton.Configuration.ConnectionString);
                SqlCommand cmd = new SqlCommand("select t.ID, t.Date, d.Caption, t.FromPersonalName, t.ToRoleName from Tasks t join DocTypes d on t.DocType=d.ID where isCompleted=0;", con);
                EnvironmentHelper.SendLogSQL(cmd.CommandText);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("Tasks");
                sda.Fill(dt);
                dataGrid.ItemsSource = dt.DefaultView;
            }
            catch (Exception ex)
            {
                EnvironmentHelper.SendErrorDialogBox(ex.Message, "SQL Error", ex.StackTrace);
            }
        }
        public static void SetInfoToGridEndWork(ref DataGrid dataGrid)
        {
            try
            {

                SqlConnection con = new SqlConnection(SystemSingleton.Configuration.ConnectionString);
                SqlCommand cmd = new SqlCommand("select ID, Date, DocType, FromPersonalName, ToRoleName from Tasks where isCompleted=1;", con);
                EnvironmentHelper.SendLogSQL(cmd.CommandText);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("Tasks");
                sda.Fill(dt);
                dataGrid.ItemsSource = dt.DefaultView;
            }
            catch (Exception ex)
            {
                EnvironmentHelper.SendErrorDialogBox(ex.Message, "SQL Error", ex.StackTrace);
            }
        }
        public static void SetInfoToGridOther(ref DataGrid dataGrid, Guid roleID)
        {
            try
            {

                SqlConnection con = new SqlConnection(SystemSingleton.Configuration.ConnectionString);
                SqlCommand cmd = new SqlCommand("select ID, Date, DocType, FromPersonalName from Tasks where isCompleted=0 and ToRoleID='" + roleID + "';", con);
                EnvironmentHelper.SendLogSQL(cmd.CommandText);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("Tasks");
                sda.Fill(dt);
                dataGrid.ItemsSource = dt.DefaultView;
            }
            catch (Exception ex)
            {
                EnvironmentHelper.SendErrorDialogBox(ex.Message, "SQL Error", ex.StackTrace);
            }
        }
        public static void UpdateView(TabControl tabControl)
        {

        }
        public static void UpdateSelected(TabItem tabItem)
        {

        }
    }
}
