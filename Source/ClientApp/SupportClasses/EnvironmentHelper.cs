using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ClientApp.Elements;
using ClientApp.SystemClasses;

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
            CloseAllEditingTabs();
            Application.Current.Shutdown(1);
        }

        public static void CloseAllEditingTabs()
        {
            foreach (var item in SystemSingleton.CurrentSession.TabCards)
            {
                if (item.Value.Card.Task.isEditingNow ||
                    item.Value.Card.Task.StateID != new Guid("6a52791d-7e42-42d6-a521-4252f276bb6c")) continue;
                try
                {
                    using (var con = new SqlConnection(SystemSingleton.Configuration.ConnectionString))
                    {
                        using (var command = new SqlCommand(SqlCommands.SetStopEditingToTask, con))
                        {
                            command.Parameters.Add("@TaskID", SqlDbType.UniqueIdentifier);
                            command.Parameters["@TaskID"].Value = item.Value.Card.Task.ID.Value;
                            EnvironmentHelper.SendLogSQL(command.CommandText);
                            con.Open();
                            int colms = command.ExecuteNonQuery();
                            con.Close();
                            if (colms == 0)
                            {
                                EnvironmentHelper.SendDialogBox(
                                    (string)SystemSingleton.Configuration.mainWindow.FindResource(
                                        "m_CantSetEditing") + "\n" + item.Value.Card.Task.ID.Value.ToString(),
                                    "SQL Error"
                                );
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    EnvironmentHelper.SendDialogBox(
                        (string)SystemSingleton.Configuration.mainWindow.FindResource(
                            "m_CantSetEditing") + "\n" + item.Value.Card.Task.ID.Value.ToString(),
                        "SQL Error"
                    );
                }
            }
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
        public static void FindAllRoles()
        {
            SystemSingleton.CurrentSession.UserRoles.Clear();
            try
            {
                using (var con = new SqlConnection(SystemSingleton.Configuration.ConnectionString))
                {
                    using (var command = new SqlCommand(SqlCommands.FindAllRolesCommand, con))
                    {
                        command.Parameters.Add("@UserID", SqlDbType.UniqueIdentifier);
                        command.Parameters["@UserID"].Value = SystemSingleton.CurrentSession.ID;
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
                if (item.ID == new Guid("9efcd5cd-bf54-47f3-95e3-2953cb235941")) continue;
                tempItem = new STabItem();
                tempItem.ToRole = item.ID.Value;
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
                if (item.Name == StaticTypes.PersonalRole)
                {
                    SetInfoToGridPersonal(ref tempItem.DataGrid);
                }
                else
                {
                    SetInfoToGridOther(ref tempItem);
                }
                tempItem.TabItem.Content = tempItem.DataGrid;
                SystemSingleton.CurrentSession.TabItems.Add(item.Name, tempItem);
                tabControl.Items.Add(tempItem.TabItem);
            }
            foreach (var item in SystemSingleton.CurrentSession.TabItems)
            {
                Style rowStyle = new Style(typeof(DataGridRow));
                rowStyle.Setters.Add(new EventSetter(DataGridRow.MouseDoubleClickEvent,
                                         new MouseButtonEventHandler(Handlers.Row_DoubleClick)));
                item.Value.DataGrid.RowStyle = rowStyle;
                item.Value.DataGrid.AutoGeneratedColumns += (sender, args) =>
                {
                    item.Value.DataGrid.Columns[0].Visibility = Visibility.Collapsed;
                    SystemSingleton.CurrentSession.SetCaptionToGrid(window, item);
                };
            }
        }

        public static void SetInfoToGridWork(ref DataGrid dataGrid)
        {
            try
            {
                SqlConnection con = new SqlConnection(SystemSingleton.Configuration.ConnectionString);
                SqlCommand cmd = new SqlCommand(SqlCommands.SetInfoToGridWorkCommand, con);
                cmd.Parameters.Add("@UserID", SqlDbType.UniqueIdentifier);
                cmd.Parameters["@UserID"].Value = SystemSingleton.CurrentSession.ID;
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
                SqlCommand cmd = new SqlCommand(SqlCommands.SetInfoToGridEndWorkCommand, con);
                cmd.Parameters.Add("@UserID", SqlDbType.UniqueIdentifier);
                cmd.Parameters["@UserID"].Value = SystemSingleton.CurrentSession.ID;
                EnvironmentHelper.SendLogSQL(cmd.CommandText);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("Tasks");
                sda.Fill(dt);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    dt.Rows[i][6] = (string)SystemSingleton.Configuration.mainWindow.FindResource((string)dt.Rows[i][6]);
                }
                dataGrid.ItemsSource = dt.DefaultView;
            }
            catch (Exception ex)
            {
                EnvironmentHelper.SendErrorDialogBox(ex.Message, "SQL Error", ex.StackTrace);
            }
        }
        public static void SetInfoToGridPersonal(ref DataGrid dataGrid)
        {
            try
            {
                SqlConnection con = new SqlConnection(SystemSingleton.Configuration.ConnectionString);
                SqlCommand cmd = new SqlCommand(SqlCommands.SetInfoToGridPersonalCommand, con);
                cmd.Parameters.Add("@UserID", SqlDbType.UniqueIdentifier);
                cmd.Parameters["@UserID"].Value = SystemSingleton.CurrentSession.ID;
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
        public static void SetInfoToGridOther(ref STabItem tabitem)
        {
            try
            {
                SqlConnection con = new SqlConnection(SystemSingleton.Configuration.ConnectionString);
                SqlCommand cmd = new SqlCommand(SqlCommands.SetInfoToGridOtherCommand, con);
                cmd.Parameters.Add("@RoleID", SqlDbType.UniqueIdentifier);
                cmd.Parameters["@RoleID"].Value = tabitem.ToRole;
                EnvironmentHelper.SendLogSQL(cmd.CommandText);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("Tasks");
                sda.Fill(dt);
                tabitem.DataGrid.ItemsSource = dt.DefaultView;
            }
            catch (Exception ex)
            {
                EnvironmentHelper.SendErrorDialogBox(ex.Message, "SQL Error", ex.StackTrace);
            }
        }
        public static void UpdateView()
        {
            SetInfoToGridWork(ref SystemSingleton.CurrentSession.TabItems[StaticTypes.CurrentWorkTab].DataGrid);
            SetInfoToGridEndWork(ref SystemSingleton.CurrentSession.TabItems[StaticTypes.CompletedWorkTab].DataGrid);
            SetInfoToGridPersonal(ref SystemSingleton.CurrentSession.TabItems[StaticTypes.PersonalRole].DataGrid);
            foreach (var item in SystemSingleton.CurrentSession.TabItems)
            {
                if (item.Key != StaticTypes.CurrentWorkTab &&
                    item.Key != StaticTypes.CompletedWorkTab &&
                    item.Key != StaticTypes.PersonalRole)
                {
                    var temp = item.Value;
                    SetInfoToGridOther(ref temp);
                }
            }
        }
    }
}
