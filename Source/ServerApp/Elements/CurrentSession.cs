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
        public string InputtedLogin;
        public Guid? ChoosenDocType;
        public int DocumentTypesPage;
        public Guid? ChoosenRole;
        public int PersonalRolesPage;
        public int CurrentTasksPage;
        public int HistoryPage;

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
                                InputtedLogin = reader.GetString(2);
                                ChoosenDocType = reader.GetGuid(3);
                                DocumentTypesPage = reader.GetInt32(4);
                                ChoosenRole = reader.GetGuid(5);
                                PersonalRolesPage = reader.GetInt32(6);
                                CurrentTasksPage = reader.GetInt32(7);
                                HistoryPage = reader.GetInt32(8);
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

        public bool Login(string password)
        {
            if (State != 0 || InputtedLogin == "") return false;
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
                        command.Parameters.Add("@LoginText", SqlDbType.NVarChar);
                        command.Parameters["@LoginText"].Value = InputtedLogin;
                        EnvironmentHelper.SendLogSQL(command.CommandText);
                        con.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                ID = reader.GetGuid(0);
                                hashfromsql = reader.GetString(1);
                                TelegramID = reader.GetInt64(2);
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
            if (hash != hashfromsql)
            {
                EnvironmentHelper.SendLog("Login failed, user: " + InputtedLogin);
                InputtedLogin = "";
                return false;
            }
            else
            {
                EnvironmentHelper.SendLog("Log In - " + InputtedLogin);
                return true;
            }
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
