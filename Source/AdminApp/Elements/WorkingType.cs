using System;
using System.Data;
using System.Data.SqlClient;
using AdminApp.BaseClasses;
using AdminApp.SupportClasses;
using AdminApp.SystemClasses;

namespace AdminApp.Elements
{
    public class WorkingType : BaseElement
    {
        public string Name;
        public string Caption;

        public WorkingType()
        {
            SetType(new Guid("642faf68-37f3-4fd6-a97d-7abfe5a9a921"));
        }
        public WorkingType(bool isEmpty) { }

        private void SetType(Guid id)
        {
            try
            {
                using (var con = new SqlConnection(SystemSingleton.Configuration.ConnectionString))
                {
                    SystemSingleton.Configuration.SqlConnections.Add(con);
                    using (var command = new SqlCommand(SqlCommands.LoadWorkingTypeCommand, con))
                    {
                        command.Parameters.Add("@ID", SqlDbType.UniqueIdentifier);
                        command.Parameters["@ID"].Value = id;
                        EnvironmentHelper.SendLogSQL(command.CommandText);
                        con.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                ID = id;
                                Name = reader.GetString(1);
                                Caption = reader.GetString(2);
                                HasValue = true;
                            }
                            else
                            {
                                EnvironmentHelper.SendDialogBox(
                                    (string)SystemSingleton.Configuration.mainWindow.FindResource("m_WorkingTypeNotFound") + "\n\n" + id.ToString(),
                                    "Working Type Error"
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
        public WorkingType(Guid id)
        {
            SetType(id);
        }
    }
}
