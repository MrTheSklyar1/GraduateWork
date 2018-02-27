using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace ClientApp.SupportClasses
{
    public struct Role
    {
        public Guid ID;
        public string Name;
        public string Caption;
    }

    public struct STabItem
    {
        public TabItem TabItem;
        public DataGrid DataGrid;
    }

    public static class StaticTypes
    {
        public const string CurrentWorkTab = "CurrentWorkTab";
        public const string CompletedWorkTab = "CompletedWorkTab";
        public const string CurrentWorkGrid = "CurrentWorkGrid";
        public const string CompletedWorkGrid = "CompletedWorkGrid";
        public const string PersonalRole = "PersonalRole";
    }
}
