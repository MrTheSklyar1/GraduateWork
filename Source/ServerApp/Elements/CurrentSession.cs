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
    public class CurrentSession : BaseElement
    {
        public long TelegramID;
        public int State;
        public Guid ChoosenDocType;
        public int DocumentTypesPage;
        public Guid ChoosenRole;
        public int PersonalRolesPage;
        public int CurrentTasksPage;
        public int HistoryPage;

        public CurrentSession(Guid id)
        {
            try
            {
                using (var con = new SqlConnection(SystemSingleton.Configuration.ConnectionString))
                {
                    SystemSingleton.Configuration.SqlConnections.Add(con);
                    using (var command = new SqlCommand(SqlCommands.LoadUserCommand, con))
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
                                State = reader.GetInt32(1);
                                ChoosenDocType = reader.GetString(2);
                                DocumentTypesPage = (reader.GetInt64(3) == 0) ? (long?)null : reader.GetInt64(3);
                                FullName = reader.GetString(4);
                                FirstName = reader.GetString(5);
                                LastName = reader.GetString(6);
                                isAdmin = reader.GetBoolean(7);
                                WorkingTypeID = reader.GetGuid(8);
                                HasValue = true;
                            }
                            else
                            {
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

        public void CloseSession()
        {

        }
    }
}
