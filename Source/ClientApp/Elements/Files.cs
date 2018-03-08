using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ClientApp.BaseClasses;
using ClientApp.SupportClasses;
using ClientApp.SystemClasses;

namespace ClientApp.Elements
{
    public class Files : BaseElement
    {
        public Dictionary<Guid, string> FileDic;

        public Files()
        {
            FileDic = new Dictionary<Guid, string>();
        }
        public Files(Guid TaskID)
        {
            FileDic = new Dictionary<Guid, string>();
            try
            {
                using (var con = new SqlConnection(SystemSingleton.Configuration.ConnectionString))
                {
                    SystemSingleton.Configuration.SqlConnections.Add(con);
                    using (var command = new SqlCommand(SqlCommands.LoadFilesCommand, con))
                    {
                        command.Parameters.Add("@TaskID", SqlDbType.UniqueIdentifier);
                        command.Parameters["@TaskID"].Value = TaskID;
                        EnvironmentHelper.SendLogSQL(command.CommandText);
                        con.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            ID = TaskID;
                            while (reader.Read())
                            {
                                FileDic.Add(reader.GetGuid(0), reader.GetString(1));
                            }
                            HasValue = FileDic.Count != 0;
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
