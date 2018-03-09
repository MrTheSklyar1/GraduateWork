using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdminApp.BaseClasses;
using AdminApp.SupportClasses;
using AdminApp.SystemClasses;

namespace AdminApp.Elements
{
    public class StaticRoleCard : BaseElement
    {
        public string Name;
        public string Caption;
        public bool isEditingNow;
        public bool rolesChanged;

        public Dictionary<Guid,PersonalRoleCard> PersonalRoleCards = new Dictionary<Guid, PersonalRoleCard>();
        public Dictionary<Guid, PersonalRoleControl> PersonalControls = new Dictionary<Guid, PersonalRoleControl>();
        public List<Guid> DeletedPersons = new List<Guid>();
        public List<Guid> AddedToBasePersons = new List<Guid>();
        public Dictionary<Guid, PersonalRoleCard> NewPersonalRoles = new Dictionary<Guid, PersonalRoleCard>();
        public Dictionary<Guid, PersonalRoleControl> NewPersonalControls = new Dictionary<Guid, PersonalRoleControl>();

        public StaticRoleCard()
        {
            ID = Guid.NewGuid();
            Name = "";
            Caption = "";
            isEditingNow = false;
            HasValue = true;
        }

        public StaticRoleCard(Guid id)
        {
            try
            {
                using (var con = new SqlConnection(SystemSingleton.Configuration.ConnectionString))
                {
                    SystemSingleton.Configuration.SqlConnections.Add(con);
                    using (var command = new SqlCommand(SqlCommands.LoadStaticRoleCommand, con))
                    {
                        command.Parameters.Add("@RoleID", SqlDbType.UniqueIdentifier);
                        command.Parameters["@RoleID"].Value = id;
                        EnvironmentHelper.SendLogSQL(command.CommandText);
                        con.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                ID = id;
                                Name = reader.GetString(1);
                                Caption = reader.GetString(2);
                                isEditingNow = reader.GetBoolean(3);
                                HasValue = true;
                            }
                            else
                            {
                                EnvironmentHelper.SendDialogBox(
                                    (string)SystemSingleton.Configuration.mainWindow.FindResource("m_RoleNotFound") + "\n\n" + id.ToString(),
                                    "Role Error"
                                );
                                HasValue = false;
                            }
                        }
                        con.Close();
                    }
                    using (var command = new SqlCommand(SqlCommands.LoadRoleUsers, con))
                    {
                        command.Parameters.Add("@RoleID", SqlDbType.UniqueIdentifier);
                        command.Parameters["@RoleID"].Value = id;
                        EnvironmentHelper.SendLogSQL(command.CommandText);
                        con.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Guid Id = reader.GetGuid(0);
                                PersonalRoleCards.Add(Id, new PersonalRoleCard(Id));
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
