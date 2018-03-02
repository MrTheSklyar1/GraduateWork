using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using ClientApp.BaseClasses;

namespace ClientApp.Elements
{
    public class STabCard
    {
        //Информация из бд
        public Card Card;
        //Не изменяемые блоки текста
        public Dictionary<string, TextBlock> TextBlocks = new Dictionary<string, TextBlock>();
        //Текст боксы
        public Dictionary<string, TextBox> TextBoxes = new Dictionary<string, TextBox>();
        //Панели
        public Dictionary<string, DockPanel> DocPanels = new Dictionary<string, DockPanel>();
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
