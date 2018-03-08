using System;
using System.Data;
using System.Data.SqlClient;
using ClientApp.BaseClasses;
using ClientApp.SupportClasses;
using ClientApp.SystemClasses;

namespace ClientApp.Elements
{
    public class Role : BaseElement
    {
        public string Name;
        public string Caption;
        public Role() { }
        public Role(Guid RoleID)
        {
            try
            {
                using (var con = new SqlConnection(SystemSingleton.Configuration.ConnectionString))
                {
                    SystemSingleton.Configuration.SqlConnections.Add(con);
                    using (var command = new SqlCommand(SqlCommands.LoadStaticRoleCommand, con))
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
                                Caption = reader.GetString(1);
                                HasValue = true;
                            }
                            else
                            {
                                EnvironmentHelper.SendDialogBox(
                                    (string)SystemSingleton.Configuration.mainWindow.FindResource("m_RoleNotFound") + "\n\n" + RoleID.ToString(),
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
