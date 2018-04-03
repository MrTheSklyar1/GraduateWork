using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerApp.SupportClasses;
using ServerApp.SystemClasses;

namespace ServerApp.Elements
{
    public class DocumentTypes
    {
        public Dictionary<Guid, DocumentTags> Types = new Dictionary<Guid, DocumentTags>();
        public Dictionary<Guid, string> TypesCaptions = new Dictionary<Guid, string>();
        public bool HasValue { get; }
        public DocumentTypes()
        {
            try
            {
                using (var con = new SqlConnection(SystemSingleton.Configuration.ConnectionString))
                {
                    SystemSingleton.Configuration.SqlConnections.Add(con);
                    using (var command = new SqlCommand(SqlCommands.LoadDocumentTypesCommand, con))
                    {
                        EnvironmentHelper.SendLogSQL(command.CommandText);
                        con.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Guid id = reader.GetGuid(0);
                                Types.Add(id, new DocumentTags(id));
                                TypesCaptions.Add(id, reader.GetString(1));
                            }
                            HasValue = Types.Count != 0;
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
    }
}
