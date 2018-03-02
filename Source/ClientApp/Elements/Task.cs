using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientApp.BaseClasses;
using ClientApp.SupportClasses;
using ClientApp.SystemClasses;

namespace ClientApp.Elements
{
    public class Task : BaseElement
    {
        public Guid? FromPersonalID;
        public string FromPersonalName;
        public Guid? ToRoleID;
        public string ToRoleName;
        public DateTime? Date;
        public Guid? DocType;
        public Guid? StateID;
        public string Commentary;
        public string Respond;
        public Guid? CompletedByID;
        public DateTime? CompletedDate;
        public Task() { }
        public Task(Guid TaskID)
        {
            try
            {
                using (var con = new SqlConnection(SystemSingleton.Configuration.ConnectionString))
                {
                    using (var command = new SqlCommand(SqlCommands.LoadTaskCommand, con))
                    {
                        command.Parameters.Add("@TaskID", SqlDbType.UniqueIdentifier);
                        command.Parameters["@TaskID"].Value = TaskID;
                        EnvironmentHelper.SendLogSQL(command.CommandText);
                        con.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                ID = TaskID;
                                FromPersonalID = reader.GetGuid(1);
                                FromPersonalName = reader.GetString(2);
                                ToRoleID = reader.GetGuid(3);
                                ToRoleName = reader.GetString(4);
                                Date = reader.GetDateTime(5);
                                DocType = reader.GetGuid(6);
                                StateID = reader.GetGuid(7);
                                Commentary = reader.GetString(8);
                                Respond = reader.GetString(9);
                                CompletedByID = reader.GetGuid(10);
                                CompletedDate = reader.GetDateTime(11);
                                HasValue = true;
                            }
                            else
                            {
                                EnvironmentHelper.SendDialogBox(
                                    (string)SystemSingleton.Configuration.mainWindow.FindResource("m_CardNotFound") + "\n" + TaskID.ToString(),
                                    "Card Error"
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
