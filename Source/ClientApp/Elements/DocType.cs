using System;
using System.Data;
using System.Data.SqlClient;
using ClientApp.BaseClasses;
using ClientApp.SupportClasses;
using ClientApp.SystemClasses;

namespace ClientApp.Elements
{
    public class DocType : BaseElement
    {
        public string Name;
        public string Caption;
        public DocType() { }
        public DocType(Guid TypeID)
        {
            try
            {
                using (var con = new SqlConnection(SystemSingleton.Configuration.ConnectionString))
                {
                    SystemSingleton.Configuration.SqlConnections.Add(con);
                    using (var command = new SqlCommand(SqlCommands.LoadDocTypeCommand, con))
                    {
                        command.Parameters.Add("@TypeID", SqlDbType.UniqueIdentifier);
                        command.Parameters["@TypeID"].Value = TypeID;
                        EnvironmentHelper.SendLogSQL(command.CommandText);
                        con.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                ID = TypeID;
                                Name = reader.GetString(1);
                                Caption = reader.GetString(2);
                                HasValue = true;
                            }
                            else
                            {
                                EnvironmentHelper.SendDialogBox(
                                    (string)SystemSingleton.Configuration.mainWindow.FindResource("m_TypeNotFound") + "\n\n" + TypeID.ToString(),
                                    "Document Error"
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
