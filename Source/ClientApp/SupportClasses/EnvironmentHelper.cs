using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
            CurrentSession.UserRoles.Clear();
            using (var con = new SqlConnection(Configuration.ConnectionString))
            {
                using (var command = new SqlCommand("select ru.RoleID, r.Name, r.Caption from RoleUsers ru join Roles r on ru.RoleID=r.ID where ru.ID='" + CurrentSession.ID + "';", con))
                {
                    con.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var temp = new Role();
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
        public static void SetWorkingPlace(TabControl tabControl, Window window)
        {
            var CurrentWorkTab = new TabItem
            {
                Header = (String)window.FindResource("m_tab_WorkingTab_CurrentWork"),
                Name = "CurrentWorkTab",
                Height = 40,
                FontSize = 14
            };
            tabControl.Items.Add(CurrentWorkTab);
            var CompletedWorkTab = new TabItem
            {
                Header = (String)window.FindResource("m_tab_WorkingTab_CompletedWork"),
                Name = "CompletedWorkTab",
                Height = 40,
                FontSize = 14
            };
            tabControl.Items.Add(CompletedWorkTab);
            foreach (var item in CurrentSession.UserRoles)
            {
                string locName = (String)window.FindResource("m_tab_"+item.Name);
                var newTabItem = new TabItem
                {
                    Header = locName,
                    Name = item.Name,
                    Height = 40,
                    FontSize = 14
                };
                tabControl.Items.Add(newTabItem);
            }
            foreach (var item in tabControl.Items)
            {
                var temp = (TabItem)item;
                if (temp.Name == "CurrentWorkTab")
                {
                    var newGrid = new DataGrid
                    {
                        Name = "CurrentWorkGrid",
                        SelectionMode = DataGridSelectionMode.Single,
                        CanUserAddRows = false,
                        CanUserDeleteRows = false
                    };
                    SetInfoToGridWork(newGrid);
                    temp.Content = newGrid;
                }
                else if (temp.Name == "CompletedWorkTab")
                {
                    var newGrid = new DataGrid
                    {
                        Name = "CompletedWorkGrid",
                        SelectionMode = DataGridSelectionMode.Single,
                        CanUserAddRows = false,
                        CanUserDeleteRows = false
                        
                    };
                    SetInfoToGridEndWork(newGrid);
                    temp.Content = newGrid;
                }
                else
                {
                    var newGrid = new DataGrid
                    {
                        Name = temp.Name,
                        SelectionMode = DataGridSelectionMode.Single,
                        CanUserAddRows = false,
                        CanUserDeleteRows = false

                    };
                    SetInfoToGridOther(newGrid);
                    temp.Content = newGrid;
                }
            }
        }
        public static void SetInfoToGridWork(DataGrid dataGrid)
        {
            SqlConnection con = new SqlConnection(Configuration.ConnectionString);
            SqlCommand cmd = new SqlCommand("select ID, Date, DocType, FromPersonal, ToRole from Tasks where isCompleted=0;", con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable("Tasks");
            sda.Fill(dt);
            dataGrid.ItemsSource = dt.DefaultView;
        }
        public static void SetInfoToGridEndWork(DataGrid dataGrid)
        {
            SqlConnection con = new SqlConnection(Configuration.ConnectionString);
            SqlCommand cmd = new SqlCommand("select ID, Date, DocType, FromPersonal, ToRole from Tasks where isCompleted=1;", con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable("Tasks");
            sda.Fill(dt);
            dataGrid.ItemsSource = dt.DefaultView;
        }
        public static void SetInfoToGridOther(DataGrid dataGrid)
        {
            SqlConnection con = new SqlConnection(Configuration.ConnectionString);
            SqlCommand cmd = new SqlCommand("select ID, Date, DocType, FromPersonal, ToRole from Tasks where isCompleted=0 and ToRole='"+CurrentSession.ID+"';", con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable("Tasks");
            sda.Fill(dt);
            dataGrid.ItemsSource = dt.DefaultView;
            //Todo: переделать выборку
        }
        public static void UpdateView(TabControl tabControl)
        {
            
        }
        public static void UpdateSelected(TabItem tabItem)
        {

        }
    }
}
