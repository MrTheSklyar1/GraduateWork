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
using System.Diagnostics;
using System.ComponentModel;
using System.IO;
using System.Data.SqlClient;
using System.Data;
using Microsoft.Win32;

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
                    FillMainStackPanelToTab(result);
                    FillFirstLine(result);
                    FillSecondLine(result);
                    FillThirdLine(result);
                    FillFourthLine(result);
                    FillFifthLine(result);
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

        private static void FillMainStackPanelToTab(STabCard sTabCard)
        {
            var MainStackPanel = new StackPanel
            {
                Margin = new Thickness(10)
            };
            sTabCard.StackPanels.Add(CardViewStruct.MainStackPanel, MainStackPanel);
            sTabCard.TabItem.Content = MainStackPanel;
        }

        private static void FillFirstLine(STabCard sTabCard)
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

        private static void FillSecondLine(STabCard sTabCard)
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

            if (sTabCard.Card.State.ID == new Guid("6a52791d-7e42-42d6-a521-4252f276bb6c") && !sTabCard.Card.Task.isEditingNow)
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
                NewStateComboBox.SelectionChanged += (sender, args) =>
                {
                    foreach (var st in sTabCard.Card.AllStates.States)
                    {
                        if (("States" + st.Name) == ((TextBlock)NewStateComboBox.SelectedItem).Name)
                        {
                            sTabCard.StateChanged(st);
                            break;
                        }
                    }
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

        private static void FillThirdLine(STabCard sTabCard)
        {
            #region Основной Border

            var ThirdLineBorder = new Border
            {
                CornerRadius = new CornerRadius(6),
                BorderBrush = new SolidColorBrush(Colors.LightGray),
                BorderThickness = new Thickness(2),
                Margin = new Thickness(5, 0, 5, 10)
            };
            sTabCard.Borders.Add(CardViewStruct.ThirdLineBorder, ThirdLineBorder);
            sTabCard.StackPanels[CardViewStruct.MainStackPanel].Children.Add(ThirdLineBorder);

            #endregion

            #region Контрол текста

            //Вспомогательная панель
            var ThirdLineStackPanel = new StackPanel();
            sTabCard.StackPanels.Add(CardViewStruct.ThirdLineStackPanel, ThirdLineStackPanel);
            sTabCard.Borders[CardViewStruct.ThirdLineBorder].Child = ThirdLineStackPanel;
            //Текстовый блок
            var ThirdLineTextBlock = new TextBlock
            {
                Text = (string)SystemSingleton.Configuration.mainWindow.FindResource("c_Comment"),
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Left,
                HorizontalAlignment = HorizontalAlignment.Left,
                Width = 100,
                FontSize = 14,
                Margin = new Thickness(5, 0, 0, 0)
            };
            sTabCard.TextBlocks.Add(CardViewStruct.ThirdLineTextBlock, ThirdLineTextBlock);
            sTabCard.StackPanels[CardViewStruct.ThirdLineStackPanel].Children.Add(ThirdLineTextBlock);
            //Контрол блока
            var ThirdLineTextBox = new TextBox
            {
                Text = sTabCard.Card.Task.Commentary,
                VerticalScrollBarVisibility = ScrollBarVisibility.Visible,
                TextAlignment = TextAlignment.Left,
                AcceptsReturn = true,
                MinWidth = 725,
                FontSize = 14,
                MinHeight = 40,
                MaxHeight = 100,
                Margin = new Thickness(5),
                IsReadOnly = true
            };
            sTabCard.TextBoxes.Add(CardViewStruct.ThirdLineTextBox, ThirdLineTextBox);
            sTabCard.StackPanels[CardViewStruct.ThirdLineStackPanel].Children.Add(ThirdLineTextBox);

            #endregion
        }

        private static void FillFourthLine(STabCard sTabCard)
        {
            #region Основной Border

            var FourthLineBorder = new Border
            {
                CornerRadius = new CornerRadius(6),
                BorderBrush = new SolidColorBrush(Colors.LightGray),
                BorderThickness = new Thickness(2),
                Margin = new Thickness(5, 0, 5, 10)
            };
            sTabCard.Borders.Add(CardViewStruct.FourthLineBorder, FourthLineBorder);
            sTabCard.StackPanels[CardViewStruct.MainStackPanel].Children.Add(FourthLineBorder);

            #endregion

            #region Контрол текста

            //Вспомогательная панель
            var FourthLineStackPanel = new StackPanel();
            sTabCard.StackPanels.Add(CardViewStruct.FourthLineStackPanel, FourthLineStackPanel);
            sTabCard.Borders[CardViewStruct.FourthLineBorder].Child = FourthLineStackPanel;
            //Текстовый блок
            var FourthLineTextBlock = new TextBlock
            {
                Text = (string)SystemSingleton.Configuration.mainWindow.FindResource("c_Respond"),
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Left,
                HorizontalAlignment = HorizontalAlignment.Left,
                Width = 100,
                FontSize = 14,
                Margin = new Thickness(5, 0, 0, 0)
            };
            sTabCard.TextBlocks.Add(CardViewStruct.FourthLineTextBlock, FourthLineTextBlock);
            sTabCard.StackPanels[CardViewStruct.FourthLineStackPanel].Children.Add(FourthLineTextBlock);
            //Контрол блока
            var FourthLineTextBox = new TextBox
            {
                Text = sTabCard.Card.Task.Respond,
                VerticalScrollBarVisibility = ScrollBarVisibility.Visible,
                TextAlignment = TextAlignment.Left,
                AcceptsReturn = true,
                MinWidth = 725,
                FontSize = 14,
                MinHeight = 40,
                MaxHeight = 100,
                Margin = new Thickness(5),
                IsReadOnly = (sTabCard.Card.Task.StateID != new Guid("6a52791d-7e42-42d6-a521-4252f276bb6c") || sTabCard.Card.Task.isEditingNow)
            };
            FourthLineTextBox.TextChanged += (sender, args) =>
            {
                sTabCard.RespondChanged(FourthLineTextBox.Text);
            };
            sTabCard.TextBoxes.Add(CardViewStruct.FourthLineTextBox, FourthLineTextBox);
            sTabCard.StackPanels[CardViewStruct.FourthLineStackPanel].Children.Add(FourthLineTextBox);

            #endregion
        }

        private static void FillFifthLine(STabCard sTabCard)
        {

            #region Основная панель

            var FifthDockPanel = new DockPanel
            {
                Margin = new Thickness(5, 0, 5, 10)
            };
            sTabCard.DocPanels.Add(CardViewStruct.FifthDockPanel, FifthDockPanel);
            sTabCard.StackPanels[CardViewStruct.MainStackPanel].Children.Add(FifthDockPanel);

            #endregion

            #region Контрол Файлы

            //Border файл
            var FilesBorder = new Border
            {
                CornerRadius = new CornerRadius(6),
                BorderBrush = new SolidColorBrush(Colors.LightGray),
                BorderThickness = new Thickness(2),
                HorizontalAlignment = HorizontalAlignment.Left
            };
            sTabCard.Borders.Add(CardViewStruct.FilesBorder, FilesBorder);
            sTabCard.DocPanels[CardViewStruct.FifthDockPanel].Children.Add(FilesBorder);
            //Вспомогательная панель даты
            var FileStackPanel = new StackPanel();
            sTabCard.StackPanels.Add(CardViewStruct.FileStackPanel, FileStackPanel);
            sTabCard.Borders[CardViewStruct.FilesBorder].Child = FileStackPanel;
            //Текстовый блок
            var FileTextBlock = new TextBlock
            {
                Text = (string)SystemSingleton.Configuration.mainWindow.FindResource("c_Files"),
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Left,
                HorizontalAlignment = HorizontalAlignment.Left,
                Width = 75,
                FontSize = 14,
                Margin = new Thickness(5, 0, 0, 0)
            };
            sTabCard.TextBlocks.Add(CardViewStruct.FileTextBlock, FileTextBlock);
            sTabCard.StackPanels[CardViewStruct.FileStackPanel].Children.Add(FileTextBlock);
            //Лист view 
            var FileListView = new ListView()
            {
                Height = 120,
                MinWidth = 300,
                MaxWidth = 400,
                Margin = new Thickness(5)
            };
            sTabCard.ListViews.Add(CardViewStruct.FileListView, FileListView);
            sTabCard.StackPanels[CardViewStruct.FileStackPanel].Children.Add(FileListView);
            //Файлы
            if (sTabCard.Card.Files.HasValue)
            {
                foreach (var item in sTabCard.Card.Files.FileDic)
                {
                    var temp = new FileControl
                    {
                        ID = item.Key,
                        DockPanel = new DockPanel(),
                        Button = new Button
                        {
                            FontSize = 14,
                            Margin = new Thickness(0,0,5,0),
                            Content = (string)SystemSingleton.Configuration.mainWindow.FindResource("c_Delete"),
                            IsEnabled = (sTabCard.Card.Task.StateID == new Guid("6a52791d-7e42-42d6-a521-4252f276bb6c") && !sTabCard.Card.Task.isEditingNow) 
                        },
                        TextBlock = new TextBlock
                        {
                            FontSize = 14,
                            VerticalAlignment = VerticalAlignment.Center,
                            Text = item.Value
                        }
                    };
                    temp.Button.Click += (sender, args) =>
                    {
                        MessageBoxResult dialogResult = MessageBox.Show((string)SystemSingleton.Configuration.mainWindow.FindResource("m_MakeSureDeletingFile"), 
                            (string)SystemSingleton.Configuration.mainWindow.FindResource("m_AttentionHeader"), 
                            MessageBoxButton.YesNo);
                        if (dialogResult == MessageBoxResult.Yes)
                        {
                            sTabCard.ListViews[CardViewStruct.FileListView].Items.Remove(sTabCard.Card.FilesControls[item.Key].DockPanel);
                            sTabCard.FileDelete(item.Key);
                        }
                    };
                    temp.TextBlock.MouseLeftButtonDown += (sender, args) =>
                    {
                        try
                        {
                            Process.Start(SystemSingleton.Configuration.FilesPath+item.Key+"\\"+item.Value);
                        }
                        catch (Exception ex)
                        {
                            EnvironmentHelper.SendDialogBox((string)SystemSingleton.Configuration.mainWindow.FindResource("m_FileNotFoundInDirectory"), "Directory Error");
                        }
                    };
                    sTabCard.Card.FilesControls.Add(item.Key, temp);
                    temp.DockPanel.Children.Add(temp.Button);
                    temp.DockPanel.Children.Add(temp.TextBlock);
                    sTabCard.ListViews[CardViewStruct.FileListView].Items.Add(temp.DockPanel);
                }
            }

            if (sTabCard.Card.Task.StateID == new Guid("6a52791d-7e42-42d6-a521-4252f276bb6c") && !sTabCard.Card.Task.isEditingNow)
            {
                //Кнопка добавить
                var FileButton = new Button
                {
                    Content = (string)SystemSingleton.Configuration.mainWindow.FindResource("c_AddFile"),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Width = 145,
                    Height = 25,
                    FontSize = 14,
                    Margin = new Thickness(5)
                };
                FileButton.Click += (senderadd, argsadd) =>
                {
                    var dlg = new OpenFileDialog();
                    dlg.FileName = "Document";
                    dlg.DefaultExt = ".pdf";
                    dlg.Filter = "PDF documents (.pdf)|*.pdf";
                    dlg.Multiselect = false;
                    var result = dlg.ShowDialog();
                    if (result != null && result.Value)
                    {
                        FileBase newFile = new FileBase(dlg.SafeFileName, dlg.FileName, Guid.NewGuid());
                        var filename = newFile.Name;
                        var temp = new FileControl
                        {
                            ID = newFile.FileID,
                            DockPanel = new DockPanel(),
                            Button = new Button
                            {
                                FontSize = 14,
                                Margin = new Thickness(0, 0, 5, 0),
                                Content = (string)SystemSingleton.Configuration.mainWindow.FindResource("c_Delete"),
                                IsEnabled = true
                            },
                            TextBlock = new TextBlock
                            {
                                FontSize = 14,
                                VerticalAlignment = VerticalAlignment.Center,
                                Text = filename
                            }
                        };
                        temp.Button.Click += (sender, args) =>
                        {
                            MessageBoxResult dialogResult = MessageBox.Show((string)SystemSingleton.Configuration.mainWindow.FindResource("m_MakeSureDeletingFile"),
                                (string)SystemSingleton.Configuration.mainWindow.FindResource("m_AttentionHeader"),
                                MessageBoxButton.YesNo);
                            if (dialogResult == MessageBoxResult.Yes)
                            {
                                sTabCard.ListViews[CardViewStruct.FileListView].Items.Remove(sTabCard.NewFileControls[newFile.FileID].DockPanel);
                                sTabCard.FileDelete(newFile.FileID);
                            }
                        };
                        temp.TextBlock.MouseLeftButtonDown += (sender, args) =>
                        {
                            try
                            {
                                Process.Start(SystemSingleton.Configuration.FilesPath + temp.ID.Value + "\\" + filename);
                            }
                            catch (Exception ex)
                            {
                                EnvironmentHelper.SendDialogBox((string)SystemSingleton.Configuration.mainWindow.FindResource("m_FileNotFoundInDirectory"), "Directory Error");
                            }
                        };
                        sTabCard.FileAdded(newFile);
                        sTabCard.NewFileControls.Add(temp.ID.Value, temp);
                        temp.DockPanel.Children.Add(temp.Button);
                        temp.DockPanel.Children.Add(temp.TextBlock);
                        sTabCard.ListViews[CardViewStruct.FileListView].Items.Add(temp.DockPanel);
                    }
                };
                sTabCard.Buttons.Add(CardViewStruct.FileButton, FileButton);
                sTabCard.StackPanels[CardViewStruct.FileStackPanel].Children.Add(FileButton);
            }

            #endregion

            #region Контрол информации о выполнении


            if (sTabCard.Card.Task.CompletedByID.HasValue)
            {
                //Border инфо
                var CompletedBorder = new Border
                {
                    CornerRadius = new CornerRadius(6),
                    BorderBrush = new SolidColorBrush(Colors.LightGray),
                    BorderThickness = new Thickness(2),
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Margin = new Thickness(10,0,10,0),
                    VerticalAlignment = VerticalAlignment.Bottom
                };
                sTabCard.Borders.Add(CardViewStruct.CompletedBorder, CompletedBorder);
                sTabCard.DocPanels[CardViewStruct.FifthDockPanel].Children.Add(CompletedBorder);
                //Вспомогательная панель
                var CompletedStackPanel = new StackPanel();
                sTabCard.StackPanels.Add(CardViewStruct.CompletedStackPanel, CompletedStackPanel);
                sTabCard.Borders[CardViewStruct.CompletedBorder].Child = CompletedStackPanel;
                //Вспомогательная панель
                var CompleteDateDockPanel = new DockPanel();
                sTabCard.DocPanels.Add(CardViewStruct.CompleteDateDockPanel, CompleteDateDockPanel);
                sTabCard.StackPanels[CardViewStruct.CompletedStackPanel].Children.Add(CompleteDateDockPanel);
                //Текстовый блок Дата выполнения
                var CompletedDateTextBlock = new TextBlock
                {
                    Text = (string)SystemSingleton.Configuration.mainWindow.FindResource("c_CompletedDate"),
                    VerticalAlignment = VerticalAlignment.Center,
                    TextAlignment = TextAlignment.Left,
                    Width = 125,
                    FontSize = 14,
                    Margin = new Thickness(5, 0, 0, 0)
                };
                sTabCard.TextBlocks.Add(CardViewStruct.CompletedDateTextBlock, CompletedDateTextBlock);
                sTabCard.DocPanels[CardViewStruct.CompleteDateDockPanel].Children.Add(CompletedDateTextBlock);
                //Контрол блока даты выполнения
                var CompletedDateTextBox = new TextBox
                {
                    Text = sTabCard.Card.Task.CompletedDate.Value.ToString("G"),
                    VerticalContentAlignment = VerticalAlignment.Center,
                    TextAlignment = TextAlignment.Left,
                    MinWidth = 100,
                    MaxWidth = 200,
                    FontSize = 14,
                    Height = 40,
                    IsReadOnly = true
                };
                sTabCard.TextBoxes.Add(CardViewStruct.CompletedDateTextBox, CompletedDateTextBox);
                sTabCard.DocPanels[CardViewStruct.CompleteDateDockPanel].Children.Add(CompletedDateTextBox);
                //Вспомогательная панель
                var CompleteByDockPanel = new DockPanel();
                sTabCard.DocPanels.Add(CardViewStruct.CompleteByDockPanel, CompleteByDockPanel);
                sTabCard.StackPanels[CardViewStruct.CompletedStackPanel].Children.Add(CompleteByDockPanel);
                //Текстовый блок Дата выполнения
                var CompletedByTextBlock = new TextBlock
                {
                    Text = (string)SystemSingleton.Configuration.mainWindow.FindResource("c_CompletedBy"),
                    VerticalAlignment = VerticalAlignment.Center,
                    TextAlignment = TextAlignment.Left,
                    Width = 125,
                    FontSize = 14,
                    Margin = new Thickness(5, 0, 0, 0)
                };
                sTabCard.TextBlocks.Add(CardViewStruct.CompletedByTextBlock, CompletedByTextBlock);
                sTabCard.DocPanels[CardViewStruct.CompleteByDockPanel].Children.Add(CompletedByTextBlock);
                //Контрол блока даты выполнения
                var CompletedByTextBox = new TextBox
                {
                    Text = sTabCard.Card.CompletedBy.FullName,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    TextAlignment = TextAlignment.Left,
                    MinWidth = 100,
                    MaxWidth = 200,
                    FontSize = 14,
                    Height = 40,
                    IsReadOnly = true
                };
                sTabCard.TextBoxes.Add(CardViewStruct.CompletedByTextBox, CompletedByTextBox);
                sTabCard.DocPanels[CardViewStruct.CompleteByDockPanel].Children.Add(CompletedByTextBox);
            }


            #endregion

            #region Контрол кнопок

            //Border
            var ButtonsBorder = new Border
            {
                CornerRadius = new CornerRadius(6),
                BorderBrush = new SolidColorBrush(Colors.LightGray),
                BorderThickness = new Thickness(2),
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(10, 0, 0, 0),
                VerticalAlignment = VerticalAlignment.Bottom
            };
            sTabCard.Borders.Add(CardViewStruct.ButtonsBorder, ButtonsBorder);
            sTabCard.DocPanels[CardViewStruct.FifthDockPanel].Children.Add(ButtonsBorder);
            //Вспомогательная панель
            var ButtonsStackPanel = new StackPanel();
            sTabCard.StackPanels.Add(CardViewStruct.ButtonsStackPanel, ButtonsStackPanel);
            sTabCard.Borders[CardViewStruct.ButtonsBorder].Child = ButtonsStackPanel;
            if(sTabCard.Card.State.ID.Value == new Guid("6a52791d-7e42-42d6-a521-4252f276bb6c") && !sTabCard.Card.Task.isEditingNow)
            {
                //Кнопка сохранить
                var ButtonsSaveButton = new Button
                {
                    Content = (string)SystemSingleton.Configuration.mainWindow.FindResource("c_SaveCard"),
                    Width = 145,
                    Height = 25,
                    FontSize = 14,
                    Margin = new Thickness(5)
                };
                ButtonsSaveButton.Click += (sender, args) =>
                {
                    sTabCard.SaveUpdatedCard();
                };
                sTabCard.Buttons.Add(CardViewStruct.ButtonsSaveButton, ButtonsSaveButton);
                sTabCard.StackPanels[CardViewStruct.ButtonsStackPanel].Children.Add(ButtonsSaveButton);
            }
            else
            {
                foreach (Role item in SystemSingleton.CurrentSession.UserRoles)
                {
                    if(item.ID==new Guid("9efcd5cd-bf54-47f3-95e3-2953cb235941") && (sTabCard.Card.Task.ToRoleID!=SystemSingleton.CurrentSession.ID || new PersonalRole(SystemSingleton.CurrentSession.ID).isAdmin) && !sTabCard.Card.Task.isEditingNow)
                    {
                        //Кнопка delete
                        var ButtonsDeleteButton = new Button
                        {
                            Content = (string)SystemSingleton.Configuration.mainWindow.FindResource("c_DeleteCard"),
                            Width = 145,
                            Height = 25,
                            FontSize = 14,
                            Margin = new Thickness(5)
                        };
                        ButtonsDeleteButton.Click += (sender, args) =>
                        {
                            //TODO: кнопка удалить
                        };
                        sTabCard.Buttons.Add(CardViewStruct.ButtonsDeleteButton, ButtonsDeleteButton);
                        sTabCard.StackPanels[CardViewStruct.ButtonsStackPanel].Children.Add(ButtonsDeleteButton);
                        break;
                    }
                }
            }
            //Кнопка закрыть
            var ButtonsCloseButton = new Button
            {
                Content = (string)SystemSingleton.Configuration.mainWindow.FindResource("c_CloseCard"),
                Width = 145,
                Height = 25,
                FontSize = 14,
                Margin = new Thickness(5)
            };
            ButtonsCloseButton.Click += (sender, args) =>
            {
                MessageBoxResult dialogResult = MessageBoxResult.No;
                if (sTabCard.ChangesFile || sTabCard.ChangesRespond || sTabCard.ChangesState)
                {
                    dialogResult = MessageBox.Show((string)SystemSingleton.Configuration.mainWindow.FindResource("m_MakeSureClosingCard"),
                        (string)SystemSingleton.Configuration.mainWindow.FindResource("m_AttentionHeader"),
                        MessageBoxButton.YesNo);
                }
                if (dialogResult == MessageBoxResult.Yes || (!sTabCard.ChangesFile && !sTabCard.ChangesRespond && !sTabCard.ChangesState))
                {
                    SystemSingleton.Configuration.tabControl.Items.Remove(sTabCard.TabItem);
                    SystemSingleton.CurrentSession.TabCards.Remove(sTabCard.Card.Task.Number);
                }
                if (!sTabCard.Card.Task.isEditingNow && sTabCard.Card.Task.StateID == new Guid("6a52791d-7e42-42d6-a521-4252f276bb6c"))
                {
                    try
                    {
                        using (var con = new SqlConnection(SystemSingleton.Configuration.ConnectionString))
                        {
                            using (var command = new SqlCommand(SqlCommands.SetStopEditingToTask, con))
                            {
                                command.Parameters.Add("@TaskID", SqlDbType.UniqueIdentifier);
                                command.Parameters["@TaskID"].Value = sTabCard.Card.Task.ID.Value;
                                EnvironmentHelper.SendLogSQL(command.CommandText);
                                con.Open();
                                int colms = command.ExecuteNonQuery();
                                con.Close();
                                if (colms == 0)
                                {
                                    EnvironmentHelper.SendDialogBox(
                                        (string)SystemSingleton.Configuration.mainWindow.FindResource(
                                            "m_CantSetEditing") + "\n" + sTabCard.Card.Task.ID.Value.ToString(),
                                        "SQL Error"
                                    );
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        EnvironmentHelper.SendErrorDialogBox(ex.Message, "SQL Error", ex.StackTrace);
                    }
                }
            };
            sTabCard.Buttons.Add(CardViewStruct.ButtonsCloseButton, ButtonsCloseButton);
            sTabCard.StackPanels[CardViewStruct.ButtonsStackPanel].Children.Add(ButtonsCloseButton);

            #endregion

        }
    }
}
