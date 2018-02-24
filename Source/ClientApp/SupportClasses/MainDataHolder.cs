using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace ClientApp.SupportClasses
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
            UserRoles = new List<Role>();
        }
        public static string Login;
        public static Guid ID;
        public static int TelegramID;
        public static string FirstName;
        public static string LastName;
        public static List<Role> UserRoles = new List<Role>();
    }
    public struct Role
    {
        public Guid ID;
        public string Name;
        public string Caption;
    }
    public static class Configuration
    {
        public static string ConnectionString { get; set; }
        public static string CurrentBottomBarLabelContent { get; set; }
        public static Brush CurrentBottomBarLabelBrush { get; set; }
    }
}
