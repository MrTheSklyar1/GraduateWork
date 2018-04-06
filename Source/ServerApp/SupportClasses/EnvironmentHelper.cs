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
using ServerApp.Elements;
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
        
        public static List<string> ThreeCurrentTasksByPage(ref int page, ref int pages, Guid from)
        {
            int rows = 0;
            List<string> Page = new List<string>();
            try
            {
                using (var con = new SqlConnection(SystemSingleton.Configuration.ConnectionString))
                {
                    SystemSingleton.Configuration.SqlConnections.Add(con);
                    using (var command = new SqlCommand(SqlCommands.CountCurrentTasks, con))
                    {
                        command.Parameters.Add("@ID", SqlDbType.UniqueIdentifier);
                        command.Parameters["@ID"].Value = from;
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
                        using (var command = new SqlCommand(SqlCommands.SelectThreeCurrentTasksByPage, con))
                        {
                            command.Parameters.Add("@PageStart", SqlDbType.Int);
                            command.Parameters["@PageStart"].Value = start;
                            command.Parameters.Add("@PageEnd", SqlDbType.Int);
                            command.Parameters["@PageEnd"].Value = start + 2;
                            command.Parameters.Add("@ID", SqlDbType.UniqueIdentifier);
                            command.Parameters["@ID"].Value = from;
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

        public static bool FindRoleByLastAndFirstName(string messageText, out Guid guid)
        {
            guid = Guid.Empty;
            try
            {
                using (var con = new SqlConnection(SystemSingleton.Configuration.ConnectionString))
                {
                    SystemSingleton.Configuration.SqlConnections.Add(con);
                    using (var command = new SqlCommand(SqlCommands.FindRoleByLastAndFirstName, con))
                    {
                        command.Parameters.Add("@text", SqlDbType.NVarChar);
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

        public static string PrepareMessageNewTask(CurrentSession session)
        {
            string msg = "";
            string role = "";
            string doc = "";
            string from = "";
            try
            {
                using (var con = new SqlConnection(SystemSingleton.Configuration.ConnectionString))
                {
                    SystemSingleton.Configuration.SqlConnections.Add(con);
                    using (var command = new SqlCommand(SqlCommands.FindNamesForRoleDoc, con))
                    {
                        command.Parameters.Add("@RoleID", SqlDbType.UniqueIdentifier);
                        command.Parameters["@RoleID"].Value = session.ChoosenRole;
                        command.Parameters.Add("@DocTypeID", SqlDbType.UniqueIdentifier);
                        command.Parameters["@DocTypeID"].Value = session.ChoosenDocType;
                        command.Parameters.Add("@FromID", SqlDbType.UniqueIdentifier);
                        command.Parameters["@FromID"].Value = session.ID.Value;
                        EnvironmentHelper.SendLogSQL(command.CommandText);
                        con.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            reader.Read();
                            role = reader.GetString(0);
                            reader.Read();
                            doc = reader.GetString(0);
                            reader.Read();
                            from = reader.GetString(0);
                        }
                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                SendFatalLog(ex.Message + "\n\n" + ex.StackTrace);
            }
            msg += (string) SystemSingleton.Configuration.Window.FindResource("m_BotM_ReadyPart1")+" " + from + "\n" +
                   (string) SystemSingleton.Configuration.Window.FindResource("m_BotM_ReadyPart2") + " " + role + "\n" +
                   (string) SystemSingleton.Configuration.Window.FindResource("m_BotM_ReadyPart3") + " " + doc + "\n" +
                   (string) SystemSingleton.Configuration.Window.FindResource("m_BotM_ReadyPart4") + " " + session.Commentary;
            return msg;
        }

        public static bool FindInfoTask(string messageText, out string s)
        {
            bool result = false;
            string torole = "";
            string respond = "";
            try
            {
                using (var con = new SqlConnection(SystemSingleton.Configuration.ConnectionString))
                {
                    SystemSingleton.Configuration.SqlConnections.Add(con);
                    using (var command = new SqlCommand(SqlCommands.FindInfoAboutTask, con))
                    {
                        command.Parameters.Add("@Number", SqlDbType.NVarChar);
                        command.Parameters["@Number"].Value = messageText;
                        SendLogSQL(command.CommandText);
                        con.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                torole = reader.GetString(0);
                                respond = reader.IsDBNull(1)? "": reader.GetString(1);
                                result = true;
                            }
                        }
                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                SendFatalLog(ex.Message + "\n\n" + ex.StackTrace);
            }

            var msg = (string) SystemSingleton.Configuration.Window.FindResource("m_BotM_CurrentPart1") + " " +
                      messageText + "\n" +
                      (string) SystemSingleton.Configuration.Window.FindResource("m_BotM_CurrentPart2") + " " + torole +
                      "\n" +
                      (string) SystemSingleton.Configuration.Window.FindResource("m_BotM_CurrentPart3") + " ";
            if(respond!="") msg += (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_CurrentPart4") + " " + respond;
            s = msg;
            return result;
        }

        public static List<string> ThreeHistoryTasksByPage(ref int page, ref int pages, Guid from)
        {
            int rows = 0;
            List<string> Page = new List<string>();
            try
            {
                using (var con = new SqlConnection(SystemSingleton.Configuration.ConnectionString))
                {
                    SystemSingleton.Configuration.SqlConnections.Add(con);
                    using (var command = new SqlCommand(SqlCommands.CountHistoryTasks, con))
                    {
                        command.Parameters.Add("@ID", SqlDbType.UniqueIdentifier);
                        command.Parameters["@ID"].Value = from;
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
                        using (var command = new SqlCommand(SqlCommands.SelectThreeHistoryTasksByPage, con))
                        {
                            command.Parameters.Add("@PageStart", SqlDbType.Int);
                            command.Parameters["@PageStart"].Value = start;
                            command.Parameters.Add("@PageEnd", SqlDbType.Int);
                            command.Parameters["@PageEnd"].Value = start + 2;
                            command.Parameters.Add("@ID", SqlDbType.UniqueIdentifier);
                            command.Parameters["@ID"].Value = from;
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

        public static bool FindResultTask(string messageText, out string s, out Dictionary<Guid, string> files, out string docNumber)
        {
            bool result = false;
            string torole = "";
            string respond = "";
            string state = "";
            docNumber = "";
            Guid TaskID = Guid.Empty;
            files = new Dictionary<Guid, string>();
            try
            {
                using (var con = new SqlConnection(SystemSingleton.Configuration.ConnectionString))
                {
                    SystemSingleton.Configuration.SqlConnections.Add(con);
                    using (var command = new SqlCommand(SqlCommands.FindHistoryInfoAboutTask, con))
                    {
                        command.Parameters.Add("@Number", SqlDbType.NVarChar);
                        command.Parameters["@Number"].Value = messageText;
                        SendLogSQL(command.CommandText);
                        con.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                TaskID = reader.GetGuid(0);
                                torole = reader.GetString(1);
                                Guid tempState = reader.GetGuid(2);
                                state = tempState == new Guid("3e65b0c5-f533-4e31-956d-c2073df3e58a")
                                    ? (string) SystemSingleton.Configuration.Window.FindResource("Cancelled")
                                    : (string) SystemSingleton.Configuration.Window.FindResource("Completed");
                                respond = reader.IsDBNull(3) ? "" : reader.GetString(3);
                                result = true;
                            }
                        }
                        con.Close();
                    }
                    using (var command = new SqlCommand(SqlCommands.FindFiles, con))
                    {
                        command.Parameters.Add("@TaskID", SqlDbType.UniqueIdentifier);
                        command.Parameters["@TaskID"].Value = TaskID;
                        SendLogSQL(command.CommandText);
                        con.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                files.Add(reader.GetGuid(0), reader.GetString(1));
                            }
                        }
                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                SendFatalLog(ex.Message + "\n\n" + ex.StackTrace);
            }

            var msg = (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_CurrentPart1") + " " +
                      messageText + "\n" +
                      (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_CurrentPart2") + " " + torole +
                      "\n" +
                      (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_HistoryPart3") + " " + state + " ";
            if (respond != "") msg += (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_CurrentPart4") + " " + respond;
            s = msg;
            docNumber = messageText;
            return result;
        }

        public static bool FindResultTask(Guid taskID, out string s, out Dictionary<Guid, string> files, out long ChatID, out string docNumber)
        {
            bool result = false;
            string torole = "";
            string respond = "";
            string state = "";
            string messageText = "";
            docNumber = "";
            Guid fromrole = Guid.Empty;
            files = new Dictionary<Guid, string>();
            ChatID = 0;
            try
            {
                using (var con = new SqlConnection(SystemSingleton.Configuration.ConnectionString))
                {
                    SystemSingleton.Configuration.SqlConnections.Add(con);
                    using (var command = new SqlCommand(SqlCommands.FindHistoryInfoAboutTaskByTaskID, con))
                    {
                        command.Parameters.Add("@TaskID", SqlDbType.UniqueIdentifier);
                        command.Parameters["@TaskID"].Value = taskID;
                        SendLogSQL(command.CommandText);
                        con.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                torole = reader.GetString(0);
                                Guid tempState = reader.GetGuid(1);
                                state = tempState == new Guid("3e65b0c5-f533-4e31-956d-c2073df3e58a")
                                    ? (string)SystemSingleton.Configuration.Window.FindResource("Cancelled")
                                    : (string)SystemSingleton.Configuration.Window.FindResource("Completed");
                                respond = reader.IsDBNull(2) ? "" : reader.GetString(2);
                                messageText = reader.GetString(3);
                                fromrole = reader.GetGuid(4);
                            }
                        }
                        con.Close();
                    }
                    using (var command = new SqlCommand(SqlCommands.FindFiles, con))
                    {
                        command.Parameters.Add("@TaskID", SqlDbType.UniqueIdentifier);
                        command.Parameters["@TaskID"].Value = taskID;
                        SendLogSQL(command.CommandText);
                        con.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                files.Add(reader.GetGuid(0), reader.GetString(1));
                            }
                        }
                        con.Close();
                    }
                    using (var command = new SqlCommand(SqlCommands.FindChatID, con))
                    {
                        command.Parameters.Add("@ID", SqlDbType.UniqueIdentifier);
                        command.Parameters["@ID"].Value = fromrole;
                        SendLogSQL(command.CommandText);
                        con.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                ChatID = reader.IsDBNull(0) ? 0 : reader.GetInt64(0);
                                result = true;
                            }
                        }
                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                SendFatalLog(ex.Message + "\n\n" + ex.StackTrace);
            }

            var msg = (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_CurrentPart1") + " " +
                      messageText + "\n" +
                      (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_CurrentPart2") + " " + torole +
                      "\n" +
                      (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_HistoryPart3") + " " + state + " ";
            if (respond != "") msg += (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_CurrentPart4") + " " + respond;
            s = msg;
            docNumber = messageText;
            return result;
        }

        public static List<Guid> GetCompletedID()
        {
            List<Guid> IDs = new List<Guid>();
            try
            {
                using (var con = new SqlConnection(SystemSingleton.Configuration.ConnectionString))
                {
                    SystemSingleton.Configuration.SqlConnections.Add(con);
                    using (var command = new SqlCommand(SqlCommands.SelectTaskIDsFromQueue, con))
                    {
                        EnvironmentHelper.SendLogSQL(command.CommandText);
                        con.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                IDs.Add(reader.GetGuid(0));
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

            return IDs;
        }

        public static void DeleteCompletedID(Guid task)
        {
            try
            {
                using (var con = new SqlConnection(SystemSingleton.Configuration.ConnectionString))
                {
                    SystemSingleton.Configuration.SqlConnections.Add(con);
                    using (var command = new SqlCommand(SqlCommands.DeleTaskFromQueue, con))
                    {
                        command.Parameters.Add("@ID", SqlDbType.UniqueIdentifier);
                        command.Parameters["@ID"].Value = task;
                        EnvironmentHelper.SendLogSQL(command.CommandText);
                        con.Open();
                        command.ExecuteNonQuery();
                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                EnvironmentHelper.SendFatalLog(ex.Message + "\n\n" + ex.StackTrace);
            }
        }
    }
}
