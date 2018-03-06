using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ClientApp.Elements;
using ClientApp.SupportClasses;

namespace ClientApp.SystemClasses
{
    public static class SystemSingleton
    {
        public static class CurrentSession
        {
            public static void CloseSession()
            {
                Login = "";
                ID = Guid.Empty;
                TelegramID = 0;
                FirstName = "";
                LastName = "";
                FullName = "";
                UserRoles = new List<Role>();
                TabItems = new Dictionary<string,STabItem>();
                TabCards = new Dictionary<string, STabCard>();
                CertPassword = "";
            }
            public static string Login;
            public static Guid ID;
            public static int TelegramID;
            public static string FirstName;
            public static string LastName;
            public static string FullName;
            public static List<Role> UserRoles = new List<Role>();
            public static Dictionary<string,STabItem> TabItems = new Dictionary<string, STabItem>();
            public static Dictionary<string, STabCard> TabCards = new Dictionary<string, STabCard>();
            public static string CertPassword = "";
            public static bool SetCaptionToGrid(Window window, KeyValuePair<string, STabItem> item)
            {
                try
                {
                    item.Value.DataGrid.Columns[1].Header =
                        (String)window.FindResource("m_column_Number");
                    item.Value.DataGrid.Columns[2].Header =
                        (String) window.FindResource("m_column_Date");
                    item.Value.DataGrid.Columns[3].Header =
                        (String) window.FindResource("m_column_DocType");
                    item.Value.DataGrid.Columns[4].Header =
                        (String) window.FindResource("m_column_FromPersonalName");
                    if (item.Key == StaticTypes.CompletedWorkTab)
                    {
                        item.Value.DataGrid.Columns[5].Header =
                            (String)window.FindResource("m_column_ToRoleName");
                        item.Value.DataGrid.Columns[6].Header =
                            (String)window.FindResource("m_column_State");
                        item.Value.DataGrid.Columns[7].Header =
                            (String)window.FindResource("m_column_CompletedBy");
                        item.Value.DataGrid.Columns[8].Header =
                            (String)window.FindResource("m_column_CompleteDate");
                    }
                    if (item.Key == StaticTypes.CurrentWorkTab)
                    {
                        item.Value.DataGrid.Columns[5].Header =
                            (String)window.FindResource("m_column_ToRoleName");
                    }
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
        public static class Configuration
        {
            public static string ConnectionString { get; set; }
            public static bool SQLLog { get; set; }
            public static bool SignVisible { get; set; }
            public static string FilesPath { get; set; }
            public static string CertificatePath { get; set; }
            public static string CompanyName { get; set; }
            public static string CompanyLocation { get; set; }
            public static MainWindow mainWindow { get; set; }
            public static TabControl tabControl { get; set; }
        }

        public static class BotomTab
        {
            public static string CurrentBottomBarLabelContent { get; set; }
            public static Brush CurrentBottomBarLabelBrush { get; set; }
        }
    }
}
