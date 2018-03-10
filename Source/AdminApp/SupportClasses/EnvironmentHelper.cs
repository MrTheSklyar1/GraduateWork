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
                    var temppers = PersonalRoleCardFactory.CreateTab();
                    if (temppers != null)
                    {
                        if (SystemSingleton.CurrentSession.TabCards.ContainsKey(((PersonalRoleCard)temppers.Card).ID.Value))
                        {
                            EnvironmentHelper.SendDialogBox(
                                (string)SystemSingleton.Configuration.mainWindow.FindResource("m_AlreadyOpened"),
                                "Attention"
                            );
                        }
                        else
                        {
                            SystemSingleton.CurrentSession.TabCards.Add(((PersonalRoleCard)temppers.Card).ID.Value, temppers);
                            SystemSingleton.Configuration.tabControl.Items.Add(temppers.TabItem);
                        }
                    }
                    break;
                case 1://StaticRole
                    var tempstat = StaticRoleCardFactory.CreateTab();
                    if (tempstat != null)
                    {
                        if (SystemSingleton.CurrentSession.TabCards.ContainsKey(((StaticRoleCard)tempstat.Card).ID.Value))
                        {
                            EnvironmentHelper.SendDialogBox(
                                (string)SystemSingleton.Configuration.mainWindow.FindResource("m_AlreadyOpened"),
                                "Attention"
                            );
                        }
                        else
                        {
                            SystemSingleton.CurrentSession.TabCards.Add(((StaticRoleCard)tempstat.Card).ID.Value, tempstat);
                            SystemSingleton.Configuration.tabControl.Items.Add(tempstat.TabItem);
                        }
                    }
                    break;
                case 2://DocTypes
                    MessageBox.Show("DTN");
                    break;
            }
        }

        public static void CloseAllConnections()
        {
            foreach (var item in SystemSingleton.Configuration.SqlConnections)
            {
                if(item.State==ConnectionState.Closed) continue;
                item.Close();
            }
        }

        public static void SendErrorDialogBox(string message, string header, string trace)
        {
            MessageBox.Show(message, header, MessageBoxButton.OK, MessageBoxImage.Error);
            using (StreamWriter sw = File.AppendText("log.txt"))
            {
                sw.WriteLine(DateTime.UtcNow + " -- " + message + "\n\n" + trace);
            }
            EnvironmentHelper.CloseAllEditingTabs();
            CloseAllConnections();
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

        public static void CloseAllEditingTabs()
        {
            foreach (var item in SystemSingleton.CurrentSession.TabCards)
            {
                if (item.Value.isNew) continue;
                if (item.Value.CardType == StaticTypes.PersonalRole)
                {
                    try
                    {
                        using (var con = new SqlConnection(SystemSingleton.Configuration.ConnectionString))
                        {
                            SystemSingleton.Configuration.SqlConnections.Add(con);
                            using (var command = new SqlCommand(SqlCommands.SetStopEditingToPersonalRole, con))
                            {
                                command.Parameters.Add("@ID", SqlDbType.UniqueIdentifier);
                                command.Parameters["@ID"].Value = ((PersonalRoleCard)item.Value.Card).ID.Value;
                                EnvironmentHelper.SendLogSQL(command.CommandText);
                                con.Open();
                                int colms = command.ExecuteNonQuery();
                                con.Close();
                                if (colms == 0)
                                {
                                    EnvironmentHelper.SendDialogBox(
                                        (string)SystemSingleton.Configuration.mainWindow.FindResource(
                                            "m_CantSetEditing") + "\n\n" + ((PersonalRoleCard)item.Value.Card).ID.Value.ToString(),
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
                                "m_CantSetEditing") + "\n\n" + ((PersonalRoleCard)item.Value.Card).ID.Value.ToString(),
                            "SQL Error"
                        );
                    }
                }
                else if (item.Value.CardType == StaticTypes.StaticRole)
                {
                    try
                    {
                        using (var con = new SqlConnection(SystemSingleton.Configuration.ConnectionString))
                        {
                            SystemSingleton.Configuration.SqlConnections.Add(con);
                            using (var command = new SqlCommand(SqlCommands.SetStopEditingToStaticRole, con))
                            {
                                command.Parameters.Add("@ID", SqlDbType.UniqueIdentifier);
                                command.Parameters["@ID"].Value = ((StaticRoleCard)item.Value.Card).ID.Value;
                                SendLogSQL(command.CommandText);
                                con.Open();
                                int colms = command.ExecuteNonQuery();
                                con.Close();
                                if (colms == 0)
                                {
                                    SendDialogBox(
                                        (string)SystemSingleton.Configuration.mainWindow.FindResource(
                                            "m_CantSetEditing") + "\n\n" + ((StaticRoleCard)item.Value.Card).ID.Value.ToString(),
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
                                "m_CantSetEditing") + "\n\n" + ((StaticRoleCard)item.Value.Card).ID.Value.ToString(),
                            "SQL Error"
                        );
                    }
                }
                else if (item.Value.CardType == StaticTypes.DocType)
                {
                    try
                    {
                        using (var con = new SqlConnection(SystemSingleton.Configuration.ConnectionString))
                        {
                            SystemSingleton.Configuration.SqlConnections.Add(con);
                            using (var command = new SqlCommand(SqlCommands.SetStopEditingToDocType, con))
                            {
                                command.Parameters.Add("@ID", SqlDbType.UniqueIdentifier);
                                command.Parameters["@ID"].Value = ((DocTypeCard)item.Value.Card).ID.Value;
                                SendLogSQL(command.CommandText);
                                con.Open();
                                int colms = command.ExecuteNonQuery();
                                con.Close();
                                if (colms == 0)
                                {
                                    SendDialogBox(
                                        (string)SystemSingleton.Configuration.mainWindow.FindResource(
                                            "m_CantSetEditing") + "\n\n" + ((DocTypeCard)item.Value.Card).ID.Value.ToString(),
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
                                "m_CantSetEditing") + "\n\n" + ((DocTypeCard)item.Value.Card).ID.Value.ToString(),
                            "SQL Error"
                        );
                    }
                }
            }
        }
    }
}
