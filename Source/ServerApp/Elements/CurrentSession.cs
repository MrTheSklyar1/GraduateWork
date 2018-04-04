using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ServerApp.SupportClasses;
using ServerApp.SystemClasses;
using Telegram.Bot.Types;

namespace ServerApp.Elements
{
    public class CurrentSession : BaseElement
    {
        public long TelegramID;
        public int State;
        public Guid? ChoosenDocType;
        public int DocumentTypesPage;
        public Guid? ChoosenRole;
        public int PersonalRolesPage;
        public int CurrentTasksPage;
        public int HistoryPage;
        public string Commentary;

        public CurrentSession(long id)
        {
            try
            {
                using (var con = new SqlConnection(SystemSingleton.Configuration.ConnectionString))
                {
                    SystemSingleton.Configuration.SqlConnections.Add(con);
                    using (var command = new SqlCommand(SqlCommands.LoadUserCommand, con))
                    {
                        command.Parameters.Add("@ID", SqlDbType.BigInt);
                        command.Parameters["@ID"].Value = id;
                        EnvironmentHelper.SendLogSQL(command.CommandText);
                        con.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                TelegramID = id;
                                ID = reader.GetGuid(0);
                                State = reader.GetInt32(1);
                                ChoosenDocType = reader.IsDBNull(2) ? (Guid?) null : reader.GetGuid(2);
                                DocumentTypesPage = reader.GetInt32(3);
                                ChoosenRole = reader.IsDBNull(4) ? (Guid?)null : reader.GetGuid(4);
                                PersonalRolesPage = reader.GetInt32(5);
                                CurrentTasksPage = reader.GetInt32(6);
                                HistoryPage = reader.GetInt32(7);
                                Commentary = reader.IsDBNull(8) ? "" : reader.GetString(8);
                                HasValue = true;
                            }
                            else
                            {
                                HasValue = false;
                                TelegramID = id;
                                State = 0;
                            }
                        }
                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                EnvironmentHelper.SendFatalLog(ex.Message + "\n\n"+ ex.StackTrace);
            }
        }

        public bool Login(string password, string login, bool checkuser, long telegramID)
        {
            if (State != 0 ) return false;
            var hash = "";
            var hashfromsql = "";
            using (var md5Hash = MD5.Create())
            {
                var data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                var sBuilder = new StringBuilder();
                foreach (var item in data)
                {
                    sBuilder.Append(item.ToString("x2"));
                }
                hash = sBuilder.ToString();
            }
            try
            {
                using (var con = new SqlConnection(SystemSingleton.Configuration.ConnectionString))
                {
                    SystemSingleton.Configuration.SqlConnections.Add(con);
                    using (var command = new SqlCommand(SqlCommands.LoginCommand, con))
                    {
                        command.Parameters.Add("@Login", SqlDbType.NVarChar);
                        command.Parameters["@Login"].Value = login;
                        EnvironmentHelper.SendLogSQL(command.CommandText);
                        con.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                ID = reader.GetGuid(0);
                                hashfromsql = reader.GetString(1);
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
            if (checkuser)
            {
                return ID.HasValue;
            }
            if (hash != hashfromsql)
            {
                EnvironmentHelper.SendLog("Login failed, user: " + login);
                return false;
            }
            try
            {
                using (var con = new SqlConnection(SystemSingleton.Configuration.ConnectionString))
                {
                    string tempCommand = @"update BotStat set State=1 where ID = '" + ID.Value + "'";

                    SystemSingleton.Configuration.SqlConnections.Add(con);
                    con.Open();
                    using (var command = new SqlCommand(tempCommand, con))
                    {
                        EnvironmentHelper.SendLogSQL(command.CommandText);
                        command.ExecuteNonQuery();
                    }
                    tempCommand = @"update PersonalRoles set TelegramID="+ telegramID + " where ID = '" + ID.Value + "'";
                    using (var command = new SqlCommand(tempCommand, con))
                    {
                        EnvironmentHelper.SendLogSQL(command.CommandText);
                        command.ExecuteNonQuery();
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                EnvironmentHelper.SendFatalLog(ex.Message + "\n\n" + ex.StackTrace);
            }
            EnvironmentHelper.SendLog("Log In - " + login);
            return true;
        }

        public void CloseSession()
        {
            if (HasValue)
            {
                try
                {
                    using (var con = new SqlConnection(SystemSingleton.Configuration.ConnectionString))
                    {
                        string tempCommand = @"update BotStat set State=" + State + ", ";
                        tempCommand += "ChoosenDocType=" + (ChoosenDocType.HasValue ? "'" + ChoosenDocType.Value + "'," : "null,");
                        tempCommand += "DocumentTypesPage=" + DocumentTypesPage+",";
                        tempCommand += "ChoosenRole=" + (ChoosenRole.HasValue ? "'" + ChoosenRole.Value + "'," : "null,");
                        tempCommand += "PersonalRolesPage=" + PersonalRolesPage+ ",";
                        tempCommand += "CurrentTasksPage=" + CurrentTasksPage+ ",";
                        tempCommand += "HistoryPage=" + HistoryPage;
                        tempCommand += "Commentary=" + (Commentary != "" ? "'" + Commentary + "'," : "null,");
                        tempCommand += " where ID = '" + ID.Value + "'";

                        SystemSingleton.Configuration.SqlConnections.Add(con);
                        using (var command = new SqlCommand(tempCommand, con))
                        {
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
}
