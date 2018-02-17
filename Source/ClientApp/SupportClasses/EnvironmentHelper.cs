using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;

namespace ClientApp.SupportClasses
{
    public static class EnvironmentHelper
    {
        public static void SendLog(string log)
        {
            using (StreamWriter sw = File.AppendText("log.txt"))
            {
                sw.WriteLine(DateTime.UtcNow + " -- " + log);
            }
        }
        public static void FindAllRoles()
        {
            using (var con = new SqlConnection(Configuration.ConnectionString))
            {
                using (var command = new SqlCommand("select ru.RoleID, r.Name, r.Caption from RoleUsers ru join Roles r on ru.RoleID=r.ID where ru.ID='" + CurrentSession.ID + "';", con))
                {
                    con.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var temp = new IDandName();
                            temp.ID = reader.GetGuid(0);
                            temp.Name = reader.GetString(1);
                            temp.Caption = reader.GetString(2);
                            CurrentSession.UserRoles.Add(temp);
                        }
                    }
                    con.Close();
                }
            }
        }
        public static void SetWorkingPlace(TabControl tabControl)
        {
            foreach (var item in CurrentSession.UserRoles)
            {
                var newTabItem = new TabItem
                {
                    Header = item.Caption,
                    Name = item.Name,
                    Height = 40,
                    FontSize = 14
                };
                tabControl.Items.Add(newTabItem);
            }
            foreach (var item in tabControl.Items)
            {
                var temp = (TabItem)item;
                var table = new DataGrid();
                if (temp.Name == "CurrentWorkTab")
                {
                    //TODO: Сделать контент
                }
                else if (temp.Name == "CompletedWorkTab")
                {

                }
                else
                {

                }
            }
        }
    }
}
