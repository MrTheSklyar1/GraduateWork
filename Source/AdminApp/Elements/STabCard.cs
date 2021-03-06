﻿using System.Collections.Generic;
using System.Windows.Controls;

namespace AdminApp.Elements
{
    public class STabCard
    {
        public string CardType;
        public object Card;
        public bool isNew;
        //Не изменяемые блоки текста
        public Dictionary<string, TextBlock> TextBlocks = new Dictionary<string, TextBlock>();
        //Текст боксы
        public Dictionary<string, TextBox> TextBoxes = new Dictionary<string, TextBox>();
        //Password боксы
        public Dictionary<string, PasswordBox> PasswordBoxes = new Dictionary<string, PasswordBox>();
        //Check box
        public Dictionary<string, CheckBox> CheckBoxes = new Dictionary<string, CheckBox>();
        //Панели
        public Dictionary<string, DockPanel> DockPanels = new Dictionary<string, DockPanel>();
        //Границы
        public Dictionary<string, Border> Borders = new Dictionary<string, Border>();
        //Комбобоксы, правда он один
        public Dictionary<string, ComboBox> ComboBoxes = new Dictionary<string, ComboBox>();
        //Панели строк и другие
        public Dictionary<string, StackPanel> StackPanels = new Dictionary<string, StackPanel>();
        //Опять один
        public Dictionary<string, ListView> ListViews = new Dictionary<string, ListView>();
        //Кнопки
        public Dictionary<string, Button> Buttons = new Dictionary<string, Button>();
        //Вкладка для основной панели
        public TabItem TabItem;

    }
}
