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
    public class DocumentTags : BaseElement
    {
        public List<string> Tags = new List<string>();

        public DocumentTags(Guid DocID)
        {
            try
            {
                using (var con = new SqlConnection(SystemSingleton.Configuration.ConnectionString))
                {
                    SystemSingleton.Configuration.SqlConnections.Add(con);
                    using (var command = new SqlCommand(SqlCommands.LoadDocumentTagsCommand, con))
                    {
                        command.Parameters.Add("@ID", SqlDbType.UniqueIdentifier);
                        command.Parameters["@ID"].Value = DocID;
                        EnvironmentHelper.SendLogSQL(command.CommandText);
                        con.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ID = DocID;
                                string tags = reader.GetString(0);
                                string[] tempTags = tags.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries);
                                foreach (var tag in tempTags)
                                {
                                    Tags.Add(tag);
                                }
                            }
                            HasValue = Tags.Count != 0;
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
