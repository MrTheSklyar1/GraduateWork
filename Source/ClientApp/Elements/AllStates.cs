using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using ClientApp.SupportClasses;
using ClientApp.SystemClasses;

namespace ClientApp.Elements
{
    public class AllStates
    {
        public bool HasValue { get; }
        public List<State> States = new List<State>();
        public AllStates()
        {
            try
            {
                using (var con = new SqlConnection(SystemSingleton.Configuration.ConnectionString))
                {
                    using (var command = new SqlCommand(SqlCommands.LoadAllStatesCommand, con))
                    {
                        EnvironmentHelper.SendLogSQL(command.CommandText);
                        con.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                States.Add(new State
                                {
                                    ID = reader.GetGuid(0),
                                    Name = reader.GetString(1),
                                    Caption = reader.GetString(2)
                                });
                            }
                            if (States.Count == 0)
                            {
                                EnvironmentHelper.SendDialogBox(
                                    (string)SystemSingleton.Configuration.mainWindow.FindResource("m_StatesNotFound"),
                                    "States Error"
                                );
                                HasValue = false;
                            }
                            else
                            {
                                HasValue = true;
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
