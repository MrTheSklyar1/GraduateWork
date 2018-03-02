using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ClientApp.Elements
{
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
}
