using System;
using System.Data;
using System.Data.SqlClient;
using ClientApp.BaseClasses;
using ClientApp.SupportClasses;
using ClientApp.SystemClasses;

namespace ClientApp.Elements
{
    public class State : BaseElement
    {
        public string Name;
        public string Caption;
        public State() { }

        public State(Guid StateID)
        {
            try
            {
                using (var con = new SqlConnection(SystemSingleton.Configuration.ConnectionString))
                {
                    SystemSingleton.Configuration.SqlConnections.Add(con);
                    using (var command = new SqlCommand(SqlCommands.LoadStateCommand, con))
                    {
                        command.Parameters.Add("@StateID", SqlDbType.UniqueIdentifier);
                        command.Parameters["@StateID"].Value = StateID;
                        EnvironmentHelper.SendLogSQL(command.CommandText);
                        con.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                ID = StateID;
                                Name = reader.GetString(1);
                                Caption = reader.GetString(2);
                                HasValue = true;
                            }
                            else
                            {
                                EnvironmentHelper.SendDialogBox(
                                    (string)SystemSingleton.Configuration.mainWindow.FindResource("m_StateNotFound") + "\n\n" + StateID.ToString(),
                                    "State Error"
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
