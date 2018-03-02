﻿using ClientApp.MainClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace ClientApp.SupportClasses
{
    public class Role
    {
        public Guid ID;
        public string Name;
        public string Caption;
    }

    public class STabItem
    {
        public TabItem TabItem;
        public DataGrid DataGrid;
        public Guid ToRole;
    }

    public class STabCard
    {
        public Card Card; //Информация из бд
        public Dictionary<string, TextBlock> TextBlocks; //Не изменяемые блоки текста
        public Dictionary<string, TextBox> TextBoxes; //Текст боксы
        public Dictionary<string, DockPanel> DocPanels; //Панели
        public Dictionary<string, Border> Borders; //Границы
        public Dictionary<string, ComboBox> ComboBoxes; //Комбобоксы, правда он один
        public Dictionary<string, StackPanel> StackPanels; //Панели строк и другие
        public Dictionary<string, ListView> ListViews;//Опять один
        public Dictionary<string, Button> Buttons; //Кнопки
        public TabItem TabItem; //Вкладка для основной панели
    }

    public class PersonalRole
    {
        public Guid ID;
        public string Login;
        public string PassWord;
        public int TelegramID;
        public string FullName;
        public string FirstName;
        public string LastName;
        public bool isAdmin;
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
