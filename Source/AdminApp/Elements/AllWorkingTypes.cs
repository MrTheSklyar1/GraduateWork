using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdminApp.SupportClasses;
using AdminApp.SystemClasses;

namespace AdminApp.Elements
{
    public class AllWorkingTypes
    {
        public bool HasValue { get; }
        public List<WorkingType> States = new List<WorkingType>();

        public AllWorkingTypes()
        {
            try
            {
                using (var con = new SqlConnection(SystemSingleton.Configuration.ConnectionString))
                {
                    SystemSingleton.Configuration.SqlConnections.Add(con);
                    using (var command = new SqlCommand(SqlCommands.LoadAllWorkingTypesCommand, con))
                    {
                        EnvironmentHelper.SendLogSQL(command.CommandText);
                        con.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                States.Add(new WorkingType(true)
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
