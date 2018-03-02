using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientApp.BaseClasses;
using ClientApp.SupportClasses;
using ClientApp.SystemClasses;

namespace ClientApp.Elements
{
    public class PersonalRole : BaseElement
    {
        public string Login;
        public string PassWord;
        public int? TelegramID;
        public string FullName;
        public string FirstName;
        public string LastName;
        public bool isAdmin;
        public Guid WorkingTypeID;
        public PersonalRole() { }
        public PersonalRole(Guid RoleID)
        {
            try
            {
                using (var con = new SqlConnection(SystemSingleton.Configuration.ConnectionString))
                {
                    using (var command = new SqlCommand(SqlCommands.LoadPersonalRoleCommand, con))
                    {
                        command.Parameters.Add("@RoleID", SqlDbType.UniqueIdentifier);
                        command.Parameters["@RoleID"].Value = RoleID;
                        EnvironmentHelper.SendLogSQL(command.CommandText);
                        con.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                ID = RoleID;
                                Login = reader.GetString(1);
                                PassWord = reader.GetString(2);
                                TelegramID = (reader.GetInt32(3)==0) ? (int?) null : reader.GetInt32(3);
                                FullName = reader.GetString(4);
                                FirstName = reader.GetString(5);
                                LastName = reader.GetString(6);
                                isAdmin = reader.GetBoolean(7);
                                WorkingTypeID = reader.GetGuid(8);
                                HasValue = true;
                            }
                            else
                            {
                                EnvironmentHelper.SendDialogBox(
                                    (string)SystemSingleton.Configuration.mainWindow.FindResource("m_UserNotFound") + "\n" + RoleID.ToString(),
                                    "Role Error"
                                );
                                HasValue = false;
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
    }
}
