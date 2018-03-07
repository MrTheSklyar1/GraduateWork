using System;
using System.Data;
using System.Data.SqlClient;
using ClientApp.BaseClasses;
using ClientApp.SupportClasses;
using ClientApp.SystemClasses;

namespace ClientApp.Elements
{
    public class Task : BaseElement
    {
        public long MainNumber;
        public string Number;
        public Guid FromPersonalID;
        public string FromPersonalName;
        public Guid ToRoleID;
        public string ToRoleName;
        public DateTime Date;
        public Guid DocType;
        public Guid StateID;
        public string Commentary;
        public string Respond;
        public Guid? CompletedByID;
        public DateTime? CompletedDate;
        public bool isEditingNow;
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
                                Number = reader.GetString(1);
                                FromPersonalID = reader.GetGuid(2);
                                FromPersonalName = reader.GetString(3);
                                ToRoleID = reader.GetGuid(4);
                                ToRoleName = reader.GetString(5);
                                Date = reader.GetDateTime(6);
                                DocType = reader.GetGuid(7);
                                StateID = reader.GetGuid(8);
                                Commentary = reader.GetString(9);
                                Respond = reader.GetString(10);
                                CompletedByID = (reader.GetGuid(11) == Guid.Empty) ? (Guid?)null : reader.GetGuid(11);
                                CompletedDate = (reader.GetDateTime(12).Year == 2000) ? (DateTime?)null : reader.GetDateTime(12);
                                MainNumber = reader.GetInt64(13);
                                isEditingNow = reader.GetBoolean(14);
                                HasValue = true;
                            }
                            else
                            {
                                EnvironmentHelper.SendDialogBox(
                                    (string)SystemSingleton.Configuration.mainWindow.FindResource("m_CardNotFound") + "\n\n" + TaskID.ToString(),
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
