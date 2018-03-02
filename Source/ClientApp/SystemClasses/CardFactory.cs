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
                try
                {
                    FillMainStackPanelToTab(ref result);
                    FillFirstLine(ref result);
                    FillSecondLine(ref result);
                }
                catch (Exception ex)
                {
                    Error(CardID);
                    return null;
                }
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

            #region Основная панель

            var FirstLineDockPanel = new DockPanel
            {
                Margin = new Thickness(5, 0, 5, 10)
            };
            sTabCard.DocPanels.Add(CardViewStruct.FirstLineDockPanel, FirstLineDockPanel);
            sTabCard.StackPanels[CardViewStruct.MainStackPanel].Children.Add(FirstLineDockPanel);

            #endregion

            #region Контрол даты

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
                Margin = new Thickness(5, 0, 0, 0)
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

            #endregion

            #region Контрол типа документа

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

            #endregion

            #region Контрол состояния карточки

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
                Width = 120,
                FontSize = 14,
                Height = 40,
                IsReadOnly = true
            };
            sTabCard.TextBoxes.Add(CardViewStruct.StateTextBox, StateTextBox);
            sTabCard.DocPanels[CardViewStruct.StateDockPanel].Children.Add(StateTextBox);

            #endregion

        }

        private static void FillSecondLine(ref STabCard sTabCard)
        {
            #region Основная панель

            var SecondLineDockPanel = new DockPanel
            {
                Margin = new Thickness(5, 0, 5, 10)
            };
            sTabCard.DocPanels.Add(CardViewStruct.SecondLineDockPanel, SecondLineDockPanel);
            sTabCard.StackPanels[CardViewStruct.MainStackPanel].Children.Add(SecondLineDockPanel);

            #endregion

            #region Контрол Фамилия

            //Border lastname
            var LastNameBorder = new Border
            {
                CornerRadius = new CornerRadius(6),
                BorderBrush = new SolidColorBrush(Colors.LightGray),
                BorderThickness = new Thickness(2),
                Margin = new Thickness(0, 0, 5, 0),
                HorizontalAlignment = HorizontalAlignment.Left
            };
            sTabCard.Borders.Add(CardViewStruct.LastNameBorder, LastNameBorder);
            sTabCard.DocPanels[CardViewStruct.SecondLineDockPanel].Children.Add(LastNameBorder);
            //Вспомогательная панель фамилии
            var LastNameDockPanel = new DockPanel();
            sTabCard.DocPanels.Add(CardViewStruct.LastNameDockPanel, LastNameDockPanel);
            sTabCard.Borders[CardViewStruct.LastNameBorder].Child = LastNameDockPanel;
            //Текстовый блок фамилии
            var LastNameTextBlock = new TextBlock
            {
                Text = (string)SystemSingleton.Configuration.mainWindow.FindResource("c_LastName"),
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Left,
                Width = 75,
                FontSize = 14,
                Margin = new Thickness(5, 0, 0, 0)
            };
            sTabCard.TextBlocks.Add(CardViewStruct.LastNameTextBlock, LastNameTextBlock);
            sTabCard.DocPanels[CardViewStruct.LastNameDockPanel].Children.Add(LastNameTextBlock);
            //Контрол блока фамилии
            var LastNameTextBox = new TextBox
            {
                Text = sTabCard.Card.From.LastName,
                VerticalContentAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Left,
                MinWidth = 50,
                MaxWidth = 200,
                FontSize = 14,
                Height = 40,
                IsReadOnly = true
            };
            sTabCard.TextBoxes.Add(CardViewStruct.LastNameTextBox, LastNameTextBox);
            sTabCard.DocPanels[CardViewStruct.LastNameDockPanel].Children.Add(LastNameTextBox);

            #endregion

            #region Контрол Имя

            //Border имя
            var FirstNameBorder = new Border
            {
                CornerRadius = new CornerRadius(6),
                BorderBrush = new SolidColorBrush(Colors.LightGray),
                BorderThickness = new Thickness(2),
                Margin = new Thickness(5, 0, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Left
            };
            sTabCard.Borders.Add(CardViewStruct.FirstNameBorder, FirstNameBorder);
            sTabCard.DocPanels[CardViewStruct.SecondLineDockPanel].Children.Add(FirstNameBorder);
            //Вспомогательная панель имени
            var FirstNameDockPanel = new DockPanel();
            sTabCard.DocPanels.Add(CardViewStruct.FirstNameDockPanel, FirstNameDockPanel);
            sTabCard.Borders[CardViewStruct.FirstNameBorder].Child = FirstNameDockPanel;
            //Текстовый блок имени
            var FirstNameTextBlock = new TextBlock
            {
                Text = (string)SystemSingleton.Configuration.mainWindow.FindResource("c_FirstName"),
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Left,
                Width = 75,
                FontSize = 14,
                Margin = new Thickness(5, 0, 0, 0)
            };
            sTabCard.TextBlocks.Add(CardViewStruct.FirstNameTextBlock, FirstNameTextBlock);
            sTabCard.DocPanels[CardViewStruct.FirstNameDockPanel].Children.Add(FirstNameTextBlock);
            //Контрол блока имени
            var FirstNameTextBox = new TextBox
            {
                Text = sTabCard.Card.From.FirstName,
                VerticalContentAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Left,
                MinWidth = 50,
                MaxWidth = 200,
                FontSize = 14,
                Height = 40,
                IsReadOnly = true
            };
            sTabCard.TextBoxes.Add(CardViewStruct.FirstNameTextBox, FirstNameTextBox);
            sTabCard.DocPanels[CardViewStruct.FirstNameDockPanel].Children.Add(FirstNameTextBox);

            #endregion

            #region Контрол нового состояния

            if (sTabCard.Card.State.ID == new Guid("6a52791d-7e42-42d6-a521-4252f276bb6c"))
            {
                //Border newState
                var NewStateBorder = new Border
                {
                    CornerRadius = new CornerRadius(6),
                    BorderBrush = new SolidColorBrush(Colors.LightGray),
                    BorderThickness = new Thickness(2),
                    Margin = new Thickness(5, 0, 0, 0),
                    HorizontalAlignment = HorizontalAlignment.Right
                };
                sTabCard.Borders.Add(CardViewStruct.NewStateBorder, NewStateBorder);
                sTabCard.DocPanels[CardViewStruct.SecondLineDockPanel].Children.Add(NewStateBorder);
                //Вспомогательная панель нового состояния
                var NewStateDockPanel = new DockPanel();
                sTabCard.DocPanels.Add(CardViewStruct.NewStateDockPanel, NewStateDockPanel);
                sTabCard.Borders[CardViewStruct.NewStateBorder].Child = NewStateDockPanel;
                //Текстовый блок нового состояния
                var NewStateTextBlock = new TextBlock
                {
                    Text = (string)SystemSingleton.Configuration.mainWindow.FindResource("c_NewState"),
                    VerticalAlignment = VerticalAlignment.Center,
                    TextAlignment = TextAlignment.Left,
                    Width = 120,
                    FontSize = 14,
                    Margin = new Thickness(5, 0, 0, 0)
                };
                sTabCard.TextBlocks.Add(CardViewStruct.NewStateTextBlock, NewStateTextBlock);
                sTabCard.DocPanels[CardViewStruct.NewStateDockPanel].Children.Add(NewStateTextBlock);
                //Контрол блока нового состояния
                var NewStateComboBox = new ComboBox()
                {
                    HorizontalContentAlignment = HorizontalAlignment.Left,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Width = 120,
                    FontSize = 14,
                    Height = 40
                };
                sTabCard.ComboBoxes.Add(CardViewStruct.NewStateComboBox, NewStateComboBox);
                sTabCard.DocPanels[CardViewStruct.NewStateDockPanel].Children.Add(NewStateComboBox);
                //Вкладки для комбобокса
                foreach (var item in sTabCard.Card.AllStates.States)
                {
                    sTabCard.TextBlocks.Add("States" + item.Name, new TextBlock
                    {
                        VerticalAlignment = VerticalAlignment.Center,
                        TextAlignment = TextAlignment.Left,
                        FontSize = 14,
                        Text = (string)SystemSingleton.Configuration.mainWindow.FindResource(item.Caption),
                        Name = "States" + item.Name
                    });
                    sTabCard.ComboBoxes[CardViewStruct.NewStateComboBox].Items.Add(sTabCard.TextBlocks["States" + item.Name]);
                    if (item.ID == new Guid("6a52791d-7e42-42d6-a521-4252f276bb6c"))
                    {
                        NewStateComboBox.SelectedItem = sTabCard.TextBlocks["States" + item.Name];
                    }
                }
            }

            #endregion
        }
    }
}
