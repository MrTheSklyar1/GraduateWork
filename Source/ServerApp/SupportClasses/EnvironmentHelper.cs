using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using ServerApp.SystemClasses;

namespace ServerApp.SupportClasses
{
    public static class EnvironmentHelper
    {
        public static void CloseAllConnections()
        {
            foreach (var item in SystemSingleton.Configuration.SqlConnections)
            {
                if (item.State == ConnectionState.Closed) continue;
                item.Close();
            }
        }
        public static void SendLog(string log)
        {
            string logMessage = DateTime.UtcNow + " -- " + log;
            using (StreamWriter sw = File.AppendText("log.txt"))
            {
                sw.WriteLine(logMessage);
            }
            if (SystemSingleton.Configuration.ConsoleLog)
            {
                Application.Current.Dispatcher.BeginInvoke(new ThreadStart(delegate
                {
                    if(SystemSingleton.Configuration.ConsoleBox.LineCount>10000) SystemSingleton.Configuration.ConsoleBox.Clear();
                    SystemSingleton.Configuration.ConsoleBox.Text += "\n" + logMessage;
                    SystemSingleton.Configuration.ConsoleBox.ScrollToEnd();
                }));
            }
        }
        public static void SendFatalLog(string log)
        {
            string logMessage = DateTime.UtcNow + " -- " + log;
            using (StreamWriter sw = File.AppendText("log.txt"))
            {
                sw.WriteLine(logMessage);
            }
            if (SystemSingleton.Configuration.ConsoleLog)
            {
                Application.Current.Dispatcher.BeginInvoke(new ThreadStart(delegate
                {
                    if (SystemSingleton.Configuration.ConsoleBox.LineCount > 10000) SystemSingleton.Configuration.ConsoleBox.Clear();
                    SystemSingleton.Configuration.ConsoleBox.Text += "\n" + logMessage;
                    SystemSingleton.Configuration.ConsoleBox.ScrollToEnd();
                }));
            }
            CloseAllConnections();
            SystemSingleton.WaitersWorker.SaveWaiters();
            MessageBox.Show("Error: \n\n"+ logMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Environment.Exit(1);
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

        public static string GetLogin(long fromId)
        {
            string Login = "";
            try
            {
                using (var con = new SqlConnection(SystemSingleton.Configuration.ConnectionString))
                {
                    SystemSingleton.Configuration.SqlConnections.Add(con);
                    using (var command = new SqlCommand(SqlCommands.GetLoginCommand, con))
                    {
                        command.Parameters.Add("@ID", SqlDbType.BigInt);
                        command.Parameters["@ID"].Value = fromId;
                        EnvironmentHelper.SendLogSQL(command.CommandText);
                        con.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Login = reader.GetString(0);
                            }
                        }
                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                EnvironmentHelper.SendFatalLog(ex.Message + "\n\n" + ex.StackTrace);
            }
            return Login;
        }

        public static bool IsDocContainsStaticRole(Guid key)
        {
            int strings = 0;
            try
            {
                using (var con = new SqlConnection(SystemSingleton.Configuration.ConnectionString))
                {
                    SystemSingleton.Configuration.SqlConnections.Add(con);
                    using (var command = new SqlCommand(SqlCommands.FindDocStaticRoleCommand, con))
                    {
                        command.Parameters.Add("@ID", SqlDbType.UniqueIdentifier);
                        command.Parameters["@ID"].Value = key;
                        EnvironmentHelper.SendLogSQL(command.CommandText);
                        con.Open();
                        strings = Convert.ToInt32(command.ExecuteScalar());
                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                EnvironmentHelper.SendFatalLog(ex.Message + "\n\n" + ex.StackTrace);
            }
            return strings>0;
        }
        
        public static List<string> ThreePersRolesByPage(ref int page, ref int pages, Guid roleid)
        {
            int rows = 0;
            List<string> Page = new List<string>();
            try
            {
                using (var con = new SqlConnection(SystemSingleton.Configuration.ConnectionString))
                {
                    SystemSingleton.Configuration.SqlConnections.Add(con);
                    using (var command = new SqlCommand(SqlCommands.CountPersonalRolesFromStatic, con))
                    {
                        command.Parameters.Add("@ID", SqlDbType.UniqueIdentifier);
                        command.Parameters["@ID"].Value = roleid;
                        SendLogSQL(command.CommandText);
                        con.Open();
                        rows = Convert.ToInt32(command.ExecuteScalar());
                        con.Close();
                    }
                    pages = (double)rows / 3 > rows / 3 ? rows / 3 + 1 : rows / 3;
                    if (page == 0)
                    {
                        page = pages;
                    }
                    else if (pages < page)
                    {
                        page = 1;
                    }

                    var start = (page - 1) * 3 + 1;
                    if (rows > 0)
                    {
                        using (var command = new SqlCommand(SqlCommands.SelectThreePersRolesByPage, con))
                        {
                            command.Parameters.Add("@PageStart", SqlDbType.Int);
                            command.Parameters["@PageStart"].Value = start;
                            command.Parameters.Add("@PageEnd", SqlDbType.Int);
                            command.Parameters["@PageEnd"].Value = start + 2;
                            command.Parameters.Add("@ID", SqlDbType.UniqueIdentifier);
                            command.Parameters["@ID"].Value = roleid;
                            SendLogSQL(command.CommandText);
                            con.Open();
                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    Page.Add(reader.GetString(0));
                                }
                            }
                            con.Close();
                        }
                    }
                    return Page;
                }
            }
            catch (Exception ex)
            {
                SendFatalLog(ex.Message + "\n\n" + ex.StackTrace);
                return null;
            }
        }

        public static List<string> ThreeDocTypesByPage(ref int page, ref int pages)
        {
            int rows = 0;
            List<string> Page = new List<string>();
            try
            {
                using (var con = new SqlConnection(SystemSingleton.Configuration.ConnectionString))
                {
                    SystemSingleton.Configuration.SqlConnections.Add(con);
                    using (var command = new SqlCommand(SqlCommands.CountDocTypes, con))
                    {
                        SendLogSQL(command.CommandText);
                        con.Open();
                        rows = Convert.ToInt32(command.ExecuteScalar());
                        con.Close();
                    }
                    pages = (double)rows / 3 > rows / 3 ? rows / 3 + 1 : rows / 3;
                    if (page == 0)
                    {
                        page = pages;
                    }else if (pages < page)
                    {
                        page = 1;
                    }

                    var start = (page - 1) * 3 + 1;
                    if (rows > 0)
                    {
                        using (var command = new SqlCommand(SqlCommands.SelectThreeDocTypesByPage, con))
                        {
                            command.Parameters.Add("@PageStart", SqlDbType.Int);
                            command.Parameters["@PageStart"].Value = start;
                            command.Parameters.Add("@PageEnd", SqlDbType.Int);
                            command.Parameters["@PageEnd"].Value = start + 2;
                            SendLogSQL(command.CommandText);
                            con.Open();
                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    Page.Add(reader.GetString(0));
                                }
                            }
                            con.Close();
                        }
                    }
                    return Page;
                }
            }
            catch (Exception ex)
            {
                SendFatalLog(ex.Message + "\n\n" + ex.StackTrace);
                return null;
            }
        }

        public static Guid GetRoleFromDocType(Guid doctypeID)
        {
            Guid roleid = Guid.Empty;
            try
            {
                using (var con = new SqlConnection(SystemSingleton.Configuration.ConnectionString))
                {
                    SystemSingleton.Configuration.SqlConnections.Add(con);
                    using (var command = new SqlCommand(SqlCommands.GetRoleFromDocType, con))
                    {
                        command.Parameters.Add("@ID", SqlDbType.UniqueIdentifier);
                        command.Parameters["@ID"].Value = doctypeID;
                        EnvironmentHelper.SendLogSQL(command.CommandText);
                        con.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                roleid = reader.GetGuid(0);
                            }
                        }
                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                EnvironmentHelper.SendFatalLog(ex.Message + "\n\n" + ex.StackTrace);
            }
            return roleid;
        }

        public static bool FindRoleByLatAndFirstName(string messageText, out Guid guid)
        {
            guid = Guid.Empty;
            try
            {
                using (var con = new SqlConnection(SystemSingleton.Configuration.ConnectionString))
                {
                    SystemSingleton.Configuration.SqlConnections.Add(con);
                    using (var command = new SqlCommand(SqlCommands.FindRoleByLatAndFirstName, con))
                    {
                        command.Parameters.Add("@text", SqlDbType.VarChar);
                        command.Parameters["@text"].Value = messageText;
                        EnvironmentHelper.SendLogSQL(command.CommandText);
                        con.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                guid = reader.GetGuid(0);
                            }
                        }
                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                EnvironmentHelper.SendFatalLog(ex.Message + "\n\n" + ex.StackTrace);
            }
            return guid!=Guid.Empty;
        }
    }
}
