using ClientApp.SupportClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ClientApp.Elements;

namespace ClientApp.SystemClasses
{
    public static class CardFactory
    {
        public static STabCard CreateTab(Guid CardID)
        {
            var result = new STabCard();
            result.Card = new Card(CardID);
            if (result.Card.HasValue)
            {
                result.TabItem = new TabItem
                {
                    Header = result.Card.Task.Number,
                    FontSize = 15,
                    Height = 40
                };
                FillMainStackPanelToTab(ref result);
                FillFirstLine(ref result);
                return result;
            }
            else
            {
                Error(CardID);
                return null;
            }
        }

        private static void Error(Guid CardID)
        {
            EnvironmentHelper.SendDialogBox(
                (string)SystemSingleton.Configuration.mainWindow.FindResource("m_CardViewNotCreated") + "\n" + CardID.ToString(),
                "Card Error"
            );
        }

        private static void FillMainStackPanelToTab(ref STabCard sTabCard)
        {
            var MainStackPanel = new StackPanel
            {
                Margin = new Thickness(10)
            };
            sTabCard.StackPanels.Add(CardViewStruct.MainStackPanel, MainStackPanel);
            sTabCard.TabItem.Content = MainStackPanel;
        }

        private static void FillFirstLine(ref STabCard sTabCard)
        {
            //Сама панель
            var FirstLineDockPanel = new DockPanel
            {
                Margin = new Thickness(5, 0, 5, 10)
            };
            sTabCard.DocPanels.Add(CardViewStruct.FirstLineDockPanel, FirstLineDockPanel);
            sTabCard.StackPanels[CardViewStruct.MainStackPanel].Children.Add(FirstLineDockPanel);
            //Новый контрол!
            //Border data
            var CreatedDateBorder = new Border
            {
                CornerRadius = new CornerRadius(6),
                BorderBrush = new SolidColorBrush(Colors.LightGray),
                BorderThickness = new Thickness(2),
                Margin = new Thickness(0, 0, 5, 0),
                HorizontalAlignment = HorizontalAlignment.Left
            };
            sTabCard.Borders.Add(CardViewStruct.CreatedDateBorder, CreatedDateBorder);
            sTabCard.DocPanels[CardViewStruct.FirstLineDockPanel].Children.Add(CreatedDateBorder);
            //Вспомогательная панель даты
            var CreatedDateDockPanel = new DockPanel();
            sTabCard.DocPanels.Add(CardViewStruct.CreatedDateDockPanel, CreatedDateDockPanel);
            sTabCard.Borders[CardViewStruct.CreatedDateBorder].Child = CreatedDateDockPanel;
            //Текстовый блок Дата
            var CreatedDateTextBlock = new TextBlock
            {
                Text = (string)SystemSingleton.Configuration.mainWindow.FindResource("c_CreationDate"),
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Left,
                Width = 100,
                FontSize = 14,
                Margin = new Thickness(5,0,0,0)
            };
            sTabCard.TextBlocks.Add(CardViewStruct.CreatedDateTextBlock, CreatedDateTextBlock);
            sTabCard.DocPanels[CardViewStruct.CreatedDateDockPanel].Children.Add(CreatedDateTextBlock);
            //Контрол блока даты
            var CreatedDateTextBox = new TextBox
            {
                Text = sTabCard.Card.Task.Date.ToString("G"),
                VerticalContentAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Left,
                MinWidth = 100,
                MaxWidth = 200,
                FontSize = 14,
                Height = 40,
                IsReadOnly = true
            };
            sTabCard.TextBoxes.Add(CardViewStruct.CreatedDateTextBox, CreatedDateTextBox);
            sTabCard.DocPanels[CardViewStruct.CreatedDateDockPanel].Children.Add(CreatedDateTextBox);
            //Новый контрол!
            //Border DocType
            var DocTypeBorder = new Border
            {
                CornerRadius = new CornerRadius(6),
                BorderBrush = new SolidColorBrush(Colors.LightGray),
                BorderThickness = new Thickness(2),
                Margin = new Thickness(5, 0, 5, 0),
                HorizontalAlignment = HorizontalAlignment.Left
            };
            sTabCard.Borders.Add(CardViewStruct.DocTypeBorder, DocTypeBorder);
            sTabCard.DocPanels[CardViewStruct.FirstLineDockPanel].Children.Add(DocTypeBorder);
            //Вспомогательная панель типа документа
            var DocTypeDockPanel = new DockPanel();
            sTabCard.DocPanels.Add(CardViewStruct.DocTypeDockPanel, DocTypeDockPanel);
            sTabCard.Borders[CardViewStruct.DocTypeBorder].Child = DocTypeDockPanel;
            //Текстовый блок типа документа
            var DocTypeTextBlock = new TextBlock
            {
                Text = (string)SystemSingleton.Configuration.mainWindow.FindResource("c_DocType"),
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Left,
                Width = 100,
                FontSize = 14,
                Margin = new Thickness(5, 0, 0, 0)
            };
            sTabCard.TextBlocks.Add(CardViewStruct.DocTypeTextBlock, DocTypeTextBlock);
            sTabCard.DocPanels[CardViewStruct.DocTypeDockPanel].Children.Add(DocTypeTextBlock);
            //Контрол блока даты
            var DocTypeTextBox = new TextBox
            {
                Text = sTabCard.Card.DocType.Caption,
                VerticalContentAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Left,
                MinWidth = 150,
                MaxWidth = 300,
                FontSize = 14,
                Height = 40,
                IsReadOnly = true
            };
            sTabCard.TextBoxes.Add(CardViewStruct.DocTypeTextBox, DocTypeTextBox);
            sTabCard.DocPanels[CardViewStruct.DocTypeDockPanel].Children.Add(DocTypeTextBox);
            //Новый контрол!
            //Border State
            var StateBorder = new Border
            {
                CornerRadius = new CornerRadius(6),
                BorderBrush = new SolidColorBrush(Colors.LightGray),
                BorderThickness = new Thickness(2),
                Margin = new Thickness(5, 0, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Right
            };
            sTabCard.Borders.Add(CardViewStruct.StateBorder, StateBorder);
            sTabCard.DocPanels[CardViewStruct.FirstLineDockPanel].Children.Add(StateBorder);
            //Вспомогательная панель состояния
            var StateDockPanel = new DockPanel();
            sTabCard.DocPanels.Add(CardViewStruct.StateDockPanel, StateDockPanel);
            sTabCard.Borders[CardViewStruct.StateBorder].Child = StateDockPanel;
            //Текстовый блок состояния
            var StateTextBlock = new TextBlock
            {
                Text = (string)SystemSingleton.Configuration.mainWindow.FindResource("c_State"),
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Left,
                Width = 75,
                FontSize = 14,
                Margin = new Thickness(5, 0, 0, 0)
            };
            sTabCard.TextBlocks.Add(CardViewStruct.StateTextBlock, StateTextBlock);
            sTabCard.DocPanels[CardViewStruct.StateDockPanel].Children.Add(StateTextBlock);
            //Контрол блока состояния
            var StateTextBox = new TextBox
            {
                Text = (string)SystemSingleton.Configuration.mainWindow.FindResource(sTabCard.Card.State.Caption),
                VerticalContentAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Left,
                Width = 100,
                FontSize = 14,
                Height = 40,
                IsReadOnly = true
            };
            sTabCard.TextBoxes.Add(CardViewStruct.StateTextBox, StateTextBox);
            sTabCard.DocPanels[CardViewStruct.StateDockPanel].Children.Add(StateTextBox);
        }
    }
}
