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
                                ChoosenDocType = reader.IsDBNull(2) ? (Guid?)null : reader.GetGuid(2);
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
                EnvironmentHelper.SendFatalLog(ex.Message + "\n\n" + ex.StackTrace);
            }
        }

        public bool Login(string password, string login, bool checkuser, long telegramID)
        {
            if (State != 0) return false;
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
                    tempCommand = @"update PersonalRoles set TelegramID=" + telegramID + " where ID = '" + ID.Value + "'";
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
                        tempCommand += "DocumentTypesPage=" + DocumentTypesPage + ",";
                        tempCommand += "ChoosenRole=" + (ChoosenRole.HasValue ? "'" + ChoosenRole.Value + "'," : "null,");
                        tempCommand += "PersonalRolesPage=" + PersonalRolesPage + ",";
                        tempCommand += "CurrentTasksPage=" + CurrentTasksPage + ",";
                        tempCommand += "HistoryPage=" + HistoryPage + ",";
                        tempCommand += "Commentary=" + (Commentary != "" ? "'" + Commentary + "'" : "null");
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

        public string FormAndSendTask()
        {
            string number = "";
            try
            {
                using (var con = new SqlConnection(SystemSingleton.Configuration.ConnectionString))
                {
                    SystemSingleton.Configuration.SqlConnections.Add(con);
                    string role, from;
                    using (var command = new SqlCommand(SqlCommands.FindNamesForRoles, con))
                    {
                        command.Parameters.Add("@RoleID", SqlDbType.UniqueIdentifier);
                        command.Parameters["@RoleID"].Value = ChoosenRole;
                        command.Parameters.Add("@FromID", SqlDbType.UniqueIdentifier);
                        command.Parameters["@FromID"].Value = ID.Value;
                        EnvironmentHelper.SendLogSQL(command.CommandText);
                        con.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            reader.Read();
                            role = reader.GetString(0);
                            reader.Read();
                            from = reader.GetString(0);
                            reader.Close();
                        }
                        con.Close();
                    }
                    using (var command = new SqlCommand(SqlCommands.FindLastNumber, con))
                    {
                        EnvironmentHelper.SendLogSQL(command.CommandText);
                        con.Open();
                        number = Convert.ToString(command.ExecuteScalar());
                        con.Close();
                    }

                    string[] parsedNum = number.Split('-');
                    if (Convert.ToInt32(parsedNum[0]) == DateTime.Now.Year)
                    {
                        if (Convert.ToInt32(parsedNum[1]) == DateTime.Now.Month)
                        {
                            var temp = Convert.ToInt32(parsedNum[2]);
                            temp++;
                            if(temp>99999)
                            {
                                parsedNum[1] = DateTime.Now.AddMonths(1).Month.ToString().Length == 1
                                    ? "0" + DateTime.Now.AddMonths(1).Month
                                    : DateTime.Now.AddMonths(1).Month.ToString();
                                number = DateTime.Now.AddMonths(1).Year + "-" + parsedNum[1] + "-00001";
                            }
                            else if (temp < 10)
                                parsedNum[2] = "0000" + temp;
                            else if (temp < 100)
                                parsedNum[2] = "000" + temp;
                            else if (temp < 1000)
                                parsedNum[2] = "00" + temp;
                            else if (temp < 10000)
                                parsedNum[2] = "0" + temp;
                            else
                                parsedNum[2] = temp.ToString();
                            number = parsedNum[0] + "-" + parsedNum[1] + "-" + parsedNum[2];
                        }
                        else
                        {
                            parsedNum[1] = DateTime.Now.Month.ToString().Length == 1
                                ? "0" + DateTime.Now.Month
                                : DateTime.Now.Month.ToString();
                            number = parsedNum[0] + "-" + parsedNum[1] + "-00001";
                        }
                    }
                    else
                    {
                        parsedNum[1] = DateTime.Now.Month.ToString().Length == 1
                            ? "0" + DateTime.Now.Month
                            : DateTime.Now.Month.ToString();
                        number = DateTime.Now.Year + "-"+ parsedNum[1] + "-00001";
                    }

                    using (var command = new SqlCommand(SqlCommands.InsertTask, con))
                    {
                        command.Parameters.Add("@StringNumber", SqlDbType.NVarChar);
                        command.Parameters["@StringNumber"].Value = number;
                        command.Parameters.Add("@FromID", SqlDbType.UniqueIdentifier);
                        command.Parameters["@FromID"].Value = ID;
                        command.Parameters.Add("@FromName", SqlDbType.NVarChar);
                        command.Parameters["@FromName"].Value = from;
                        command.Parameters.Add("@ToRoleID", SqlDbType.UniqueIdentifier);
                        command.Parameters["@ToRoleID"].Value = ChoosenRole;
                        command.Parameters.Add("@TORoleCaption", SqlDbType.NVarChar);
                        command.Parameters["@TORoleCaption"].Value = role;
                        command.Parameters.Add("@DocTypeID", SqlDbType.UniqueIdentifier);
                        command.Parameters["@DocTypeID"].Value = ChoosenDocType;
                        command.Parameters.Add("@Commentary", SqlDbType.NVarChar);
                        command.Parameters["@Commentary"].Value = Commentary;
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

            return number;
        }
    }
}
