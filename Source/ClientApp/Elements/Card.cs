using ClientApp.SupportClasses;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using ClientApp.BaseClasses;
using ClientApp.SystemClasses;


namespace ClientApp.Elements
{
    public class Card : BaseElement
    {
        public Task Task;
        public PersonalRole From;
        public Role To;
        public DocType DocType;
        public Files Files;
        public State State;
        public AllStates AllStates;
        public Card(Guid ID)
        {
            Task = new Task(ID);
            if (Task.HasValue)
            {
                From = new PersonalRole(Task.FromPersonalID);
                To = new Role(Task.ToRoleID);
                DocType = new DocType(Task.DocType);
                Files = new Files(ID);
                State = new State(Task.StateID);
                AllStates = new AllStates();
                if (From.HasValue && To.HasValue && DocType.HasValue && State.HasValue && AllStates.HasValue)
                {
                    HasValue = true;
                    return;
                }
            }
            EnvironmentHelper.SendDialogBox(
                (string)SystemSingleton.Configuration.mainWindow.FindResource("m_CardNotCreated") + "\n" + ID.ToString(),
                "Card Error"
            );
            HasValue = false;
        }
    }
}
