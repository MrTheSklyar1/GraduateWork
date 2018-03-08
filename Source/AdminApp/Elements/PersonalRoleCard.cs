using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdminApp.BaseClasses;
using AdminApp.SupportClasses;
using AdminApp.SystemClasses;

namespace AdminApp.Elements
{
    public class PersonalRoleCard : BaseElement
    {
        public string Login;
        public string PassWord;
        public long? TelegramID;
        public string FullName;
        public string FirstName;
        public string LastName;
        public bool isAdmin;
        public Guid WorkingTypeID;
        public bool isEditingNow;

        public AllWorkingTypes AllWorkingTypes = new AllWorkingTypes();
        public WorkingType WorkingType;

        public PersonalRoleCard()
        {
            ID=Guid.NewGuid();
            WorkingType = new WorkingType();
            WorkingTypeID = WorkingType.ID.Value;
            Login = "";
            PassWord = "";
            TelegramID = null;
            FullName = "_.";
            FirstName = "";
            LastName = "";
            isAdmin = false;
            isEditingNow = false;
            HasValue = true;
        }

        public PersonalRoleCard(Guid id)
        {
            try
            {
                using (var con = new SqlConnection(SystemSingleton.Configuration.ConnectionString))
                {
                    SystemSingleton.Configuration.SqlConnections.Add(con);
                    using (var command = new SqlCommand(SqlCommands.LoadPersonalRoleCommand, con))
                    {
                        command.Parameters.Add("@RoleID", SqlDbType.UniqueIdentifier);
                        command.Parameters["@RoleID"].Value = id;
                        EnvironmentHelper.SendLogSQL(command.CommandText);
                        con.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                ID = id;
                                Login = reader.GetString(1);
                                PassWord = reader.GetString(2);
                                TelegramID = (reader.GetInt64(3) == 0) ? (long?)null : reader.GetInt64(3);
                                FullName = reader.GetString(4);
                                FirstName = reader.GetString(5);
                                LastName = reader.GetString(6);
                                isAdmin = reader.GetBoolean(7);
                                WorkingTypeID = reader.GetGuid(8);
                                isEditingNow = reader.GetBoolean(9);
                                HasValue = true;
                                WorkingType = new WorkingType(WorkingTypeID);
                            }
                            else
                            {
                                EnvironmentHelper.SendDialogBox(
                                    (string)SystemSingleton.Configuration.mainWindow.FindResource("m_UserNotFound") + "\n\n" + id.ToString(),
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
