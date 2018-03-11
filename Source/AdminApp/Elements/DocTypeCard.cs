using AdminApp.BaseClasses;
using AdminApp.SupportClasses;
using AdminApp.SystemClasses;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminApp.Elements
{
    public class DocTypeCard : BaseElement
    {
        public string Name;
        public string Caption;
        public string TagWords;
        public Guid RoleTypeID;
        public bool isEditingNow;

        public RoleCard RoleCard;
        public Guid NewRoleCard;

        public DocTypeCard()
        {
            ID = Guid.NewGuid();
            Name = "";
            Caption = "";
            TagWords = "";
            RoleTypeID = Guid.Empty;
            NewRoleCard = Guid.Empty;;
            isEditingNow = false;
            RoleCard = new RoleCard(RoleTypeID);
            HasValue = true;
        }

        public DocTypeCard(Guid id)
        {
            try
            {
                using (var con = new SqlConnection(SystemSingleton.Configuration.ConnectionString))
                {
                    SystemSingleton.Configuration.SqlConnections.Add(con);
                    using (var command = new SqlCommand(SqlCommands.LoadDocTypeCard, con))
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
                                TagWords = reader.GetString(3);
                                RoleTypeID = reader.GetGuid(4);
                                NewRoleCard = RoleTypeID;
                                isEditingNow = reader.GetBoolean(5);
                                HasValue = true;
                                RoleCard = new RoleCard(RoleTypeID);
                            }
                            else
                            {
                                EnvironmentHelper.SendDialogBox(
                                    (string)SystemSingleton.Configuration.mainWindow.FindResource("m_UserNotFound") + "\n\n" + id.ToString(),
                                    "Doc Type Error"
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
