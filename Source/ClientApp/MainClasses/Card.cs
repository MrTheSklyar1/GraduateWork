using ClientApp.SupportClasses;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp.MainClasses
{
    public class Card
    {
        public Guid ID;
        public DateTime Date;
        public Guid DocType;
        public Guid StateID;
        public string Commentary;
        public string Respond;
        public PersonalRole FromPersonalRole;
        public PersonalRole CompletedByRole;
        public Role ToRole;
        public Card(Guid CardID)
        {
            try
            {
                using (var con = new SqlConnection(SystemSingleton.Configuration.ConnectionString))
                {
                    using (var command = new SqlCommand(SqlCommands.LoadTaskCommand, con))
                    {
                        command.Parameters.Add("@LoginText", SqlDbType.UniqueIdentifier);
                        command.Parameters["@LoginText"].Value = CardID;
                        EnvironmentHelper.SendLogSQL(command.CommandText);
                        con.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                ID = CardID;
                                Date = 
                            }
                            else
                            {
                                EnvironmentHelper.SendDialogBox(
                                    (string)SystemSingleton.Configuration.mainWindow.FindResource("m_CardNotFound")+"\n"+CardID.ToString(),
                                    "Card Error"
                                    );
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
