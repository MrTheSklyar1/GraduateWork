using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using AdminApp.SupportClasses;
using AdminApp.SystemClasses;

namespace AdminApp.Elements
{
    public static class PersonalRoleCardFactory
    {
        public static STabCard CreateTab(Guid PersonalID)
        {
            var result = new STabCard();
            result.Card = new PersonalRoleCard(PersonalID);
            result.CardType = StaticTypes.PersonalRole;
            if (((PersonalRoleCard) result.Card).HasValue)
            {
                result.TabItem = new TabItem
                {
                    Header = ((PersonalRoleCard)result.Card).FullName,
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
                catch
                {
                    EnvironmentHelper.Error(PersonalID);
                    return null;
                }
                return result;
            }
            else
            {
                EnvironmentHelper.Error(PersonalID);
                return null;
            }
        }
        private static void FillMainStackPanelToTab(STabCard sTabCard)
        {
            var MainStackPanel = new StackPanel
            {
                Margin = new Thickness(10)
            };
            sTabCard.StackPanels.Add(PersonalRoleCardViewStruct.MainStackPanel, MainStackPanel);
            sTabCard.TabItem.Content = MainStackPanel;
        }

        private static void FillFirstLine(STabCard sTabCard)
        {

            #region Основная панель

            var FirstLineDockPanel = new DockPanel
            {
                Margin = new Thickness(5, 0, 5, 10)
            };
            sTabCard.DocPanels.Add(PersonalRoleCardViewStruct.FirstLineDockPanel, FirstLineDockPanel);
            sTabCard.StackPanels[PersonalRoleCardViewStruct.MainStackPanel].Children.Add(FirstLineDockPanel);

            #endregion

            #region Контрол Логина

            //Border
            var LoginBorder = new Border
            {
                CornerRadius = new CornerRadius(6),
                BorderBrush = new SolidColorBrush(Colors.LightGray),
                BorderThickness = new Thickness(2),
                Margin = new Thickness(0, 0, 5, 0),
                HorizontalAlignment = HorizontalAlignment.Left
            };
            sTabCard.Borders.Add(PersonalRoleCardViewStruct.LoginBorder, LoginBorder);
            sTabCard.DocPanels[PersonalRoleCardViewStruct.FirstLineDockPanel].Children.Add(LoginBorder);
            //Вспомогательная панель
            var LoginDockPanel = new DockPanel();
            sTabCard.DocPanels.Add(PersonalRoleCardViewStruct.LoginDockPanel, LoginDockPanel);
            sTabCard.Borders[PersonalRoleCardViewStruct.LoginBorder].Child = LoginDockPanel;
            //Текстовый блок
            var LoginTextBlock = new TextBlock
            {
                Text = (string)SystemSingleton.Configuration.mainWindow.FindResource("m_tab_LogIn_LogIn"),
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Left,
                FontSize = 14,
                Margin = new Thickness(5, 0, 5, 0)
            };
            sTabCard.TextBlocks.Add(PersonalRoleCardViewStruct.LoginTextBlock, LoginTextBlock);
            sTabCard.DocPanels[PersonalRoleCardViewStruct.LoginDockPanel].Children.Add(LoginTextBlock);
            //Контрол блока 
            var LoginTextBox = new TextBox
            {
                Text = ((PersonalRoleCard)sTabCard.Card).Login,
                VerticalContentAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Left,
                MinWidth = 150,
                MaxWidth = 300,
                FontSize = 14,
                Height = 40,
                MaxLength = 50,
                IsReadOnly = ((PersonalRoleCard)sTabCard.Card).isEditingNow
            };
            sTabCard.TextBoxes.Add(PersonalRoleCardViewStruct.LoginTextBox, LoginTextBox);
            sTabCard.DocPanels[PersonalRoleCardViewStruct.LoginDockPanel].Children.Add(LoginTextBox);

            #endregion

            #region Контрол Telegram ID

            //Border
            var TelegramBorder = new Border
            {
                CornerRadius = new CornerRadius(6),
                BorderBrush = new SolidColorBrush(Colors.LightGray),
                BorderThickness = new Thickness(2),
                Margin = new Thickness(0, 0, 5, 0),
                HorizontalAlignment = HorizontalAlignment.Right
            };
            sTabCard.Borders.Add(PersonalRoleCardViewStruct.TelegramBorder, TelegramBorder);
            sTabCard.DocPanels[PersonalRoleCardViewStruct.FirstLineDockPanel].Children.Add(TelegramBorder);
            //Вспомогательная панель 
            var TelegramDockPanel = new DockPanel();
            sTabCard.DocPanels.Add(PersonalRoleCardViewStruct.TelegramDockPanel, TelegramDockPanel);
            sTabCard.Borders[PersonalRoleCardViewStruct.TelegramBorder].Child = TelegramDockPanel;
            //Текстовый блок типа документа
            var TelegramTextBlock = new TextBlock
            {
                Text = (string)SystemSingleton.Configuration.mainWindow.FindResource("c_TelegramID"),
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Left,
                FontSize = 14,
                Margin = new Thickness(5, 0, 5, 0)
            };
            sTabCard.TextBlocks.Add(PersonalRoleCardViewStruct.TelegramTextBlock, TelegramTextBlock);
            sTabCard.DocPanels[PersonalRoleCardViewStruct.TelegramDockPanel].Children.Add(TelegramTextBlock);
            //Контрол блока 
            var TelegramTextBox = new TextBox
            {
                Text = ((PersonalRoleCard) sTabCard.Card).TelegramID?.ToString() ?? (string)SystemSingleton.Configuration.mainWindow.FindResource("c_TelegramIDNULL"),
                VerticalContentAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Left,
                MinWidth = 150,
                MaxWidth = 300,
                FontSize = 14,
                Height = 40,
                MaxLength = 20,
                IsReadOnly = ((PersonalRoleCard)sTabCard.Card).isEditingNow
            };
            sTabCard.TextBoxes.Add(PersonalRoleCardViewStruct.TelegramTextBox, TelegramTextBox);
            sTabCard.DocPanels[PersonalRoleCardViewStruct.TelegramDockPanel].Children.Add(TelegramTextBox);

            #endregion
        }

        private static void FillSecondLine(STabCard sTabCard)
        {
            #region Основная панель

            var SecondLineDockPanel = new DockPanel
            {
                Margin = new Thickness(5, 0, 5, 10)
            };
            sTabCard.DocPanels.Add(PersonalRoleCardViewStruct.SecondLineDockPanel, SecondLineDockPanel);
            sTabCard.StackPanels[PersonalRoleCardViewStruct.MainStackPanel].Children.Add(SecondLineDockPanel);

            #endregion

            #region Контрол Пароля

            //Border
            var PasswordBorder = new Border
            {
                CornerRadius = new CornerRadius(6),
                BorderBrush = new SolidColorBrush(Colors.LightGray),
                BorderThickness = new Thickness(2),
                Margin = new Thickness(0, 0, 5, 0),
                HorizontalAlignment = HorizontalAlignment.Left
            };
            sTabCard.Borders.Add(PersonalRoleCardViewStruct.PasswordBorder, PasswordBorder);
            sTabCard.DocPanels[PersonalRoleCardViewStruct.SecondLineDockPanel].Children.Add(PasswordBorder);
            //Вспомогательная панель
            var PasswordDockPanel = new DockPanel();
            sTabCard.DocPanels.Add(PersonalRoleCardViewStruct.PasswordDockPanel, PasswordDockPanel);
            sTabCard.Borders[PersonalRoleCardViewStruct.PasswordBorder].Child = PasswordDockPanel;
            //Текстовый блок
            var PasswordTextBlock = new TextBlock
            {
                Text = (string)SystemSingleton.Configuration.mainWindow.FindResource("m_tab_LogIn_PassWord"),
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Left,
                FontSize = 14,
                Margin = new Thickness(5, 0, 5, 0)
            };
            sTabCard.TextBlocks.Add(PersonalRoleCardViewStruct.PasswordTextBlock, PasswordTextBlock);
            sTabCard.DocPanels[PersonalRoleCardViewStruct.PasswordDockPanel].Children.Add(PasswordTextBlock);
            //Контрол блока 
            var PasswordPasswordBox = new PasswordBox
            {
                Password = ((PersonalRoleCard)sTabCard.Card).PassWord,
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Left,
                MinWidth = 150,
                MaxWidth = 300,
                FontSize = 14,
                Height = 40,
                MaxLength = 512,
                IsEnabled = !((PersonalRoleCard)sTabCard.Card).isEditingNow
            };
            sTabCard.PasswordBoxes.Add(PersonalRoleCardViewStruct.PasswordPasswordBox, PasswordPasswordBox);
            sTabCard.DocPanels[PersonalRoleCardViewStruct.PasswordDockPanel].Children.Add(PasswordPasswordBox);

            #endregion

            #region Контрол повтора Пароля

            //Border
            var RepeatPasswordBorder = new Border
            {
                CornerRadius = new CornerRadius(6),
                BorderBrush = new SolidColorBrush(Colors.LightGray),
                BorderThickness = new Thickness(2),
                Margin = new Thickness(0, 0, 5, 0),
                HorizontalAlignment = HorizontalAlignment.Left
            };
            sTabCard.Borders.Add(PersonalRoleCardViewStruct.RepeatPasswordBorder, RepeatPasswordBorder);
            sTabCard.DocPanels[PersonalRoleCardViewStruct.SecondLineDockPanel].Children.Add(RepeatPasswordBorder);
            //Вспомогательная панель
            var RepeatPasswordDockPanel = new DockPanel();
            sTabCard.DocPanels.Add(PersonalRoleCardViewStruct.RepeatPasswordDockPanel, RepeatPasswordDockPanel);
            sTabCard.Borders[PersonalRoleCardViewStruct.RepeatPasswordBorder].Child = RepeatPasswordDockPanel;
            //Текстовый блок
            var RepeatPasswordTextBlock = new TextBlock
            {
                Text = (string)SystemSingleton.Configuration.mainWindow.FindResource("c_RepeatPassword"),
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Left,
                FontSize = 14,
                Margin = new Thickness(5, 0, 5, 0)
            };
            sTabCard.TextBlocks.Add(PersonalRoleCardViewStruct.RepeatPasswordTextBlock, RepeatPasswordTextBlock);
            sTabCard.DocPanels[PersonalRoleCardViewStruct.RepeatPasswordDockPanel].Children.Add(RepeatPasswordTextBlock);
            //Контрол блока 
            var RepeatPasswordPasswordBox = new PasswordBox
            {
                Password = ((PersonalRoleCard)sTabCard.Card).PassWord,
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Left,
                MinWidth = 150,
                MaxWidth = 300,
                FontSize = 14,
                Height = 40,
                MaxLength = 512,
                IsEnabled = !((PersonalRoleCard)sTabCard.Card).isEditingNow
            };
            sTabCard.PasswordBoxes.Add(PersonalRoleCardViewStruct.RepeatPasswordPasswordBox, RepeatPasswordPasswordBox);
            sTabCard.DocPanels[PersonalRoleCardViewStruct.RepeatPasswordDockPanel].Children.Add(RepeatPasswordPasswordBox);

            #endregion
        }

        private static void FillThirdLine(STabCard sTabCard)
        {
            #region Основная панель

            var ThirdLineDockPanel = new DockPanel
            {
                Margin = new Thickness(5, 0, 5, 10)
            };
            sTabCard.DocPanels.Add(PersonalRoleCardViewStruct.ThirdLineDockPanel, ThirdLineDockPanel);
            sTabCard.StackPanels[PersonalRoleCardViewStruct.MainStackPanel].Children.Add(ThirdLineDockPanel);

            #endregion

            #region Контрол Фамилии

            //Border
            var LastNameBorder = new Border
            {
                CornerRadius = new CornerRadius(6),
                BorderBrush = new SolidColorBrush(Colors.LightGray),
                BorderThickness = new Thickness(2),
                Margin = new Thickness(0, 0, 5, 0),
                HorizontalAlignment = HorizontalAlignment.Left
            };
            sTabCard.Borders.Add(PersonalRoleCardViewStruct.LastNameBorder, LastNameBorder);
            sTabCard.DocPanels[PersonalRoleCardViewStruct.ThirdLineDockPanel].Children.Add(LastNameBorder);
            //Вспомогательная панель
            var LastNameDockPanel = new DockPanel();
            sTabCard.DocPanels.Add(PersonalRoleCardViewStruct.LastNameDockPanel, LastNameDockPanel);
            sTabCard.Borders[PersonalRoleCardViewStruct.LastNameBorder].Child = LastNameDockPanel;
            //Текстовый блок
            var LastNameTextBlock = new TextBlock
            {
                Text = (string)SystemSingleton.Configuration.mainWindow.FindResource("c_LastName"),
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Left,
                FontSize = 14,
                Margin = new Thickness(5, 0, 5, 0)
            };
            sTabCard.TextBlocks.Add(PersonalRoleCardViewStruct.LastNameTextBlock, LastNameTextBlock);
            sTabCard.DocPanels[PersonalRoleCardViewStruct.LastNameDockPanel].Children.Add(LastNameTextBlock);
            //Контрол блока 
            var LastNameTextBox = new TextBox
            {
                Text = ((PersonalRoleCard)sTabCard.Card).LastName,
                VerticalContentAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Left,
                MinWidth = 150,
                MaxWidth = 300,
                FontSize = 14,
                Height = 40,
                MaxLength = 50,
                IsReadOnly = ((PersonalRoleCard)sTabCard.Card).isEditingNow
            };
            LastNameTextBox.LostKeyboardFocus += (sender, args) =>
            {
                NameTextBox_LostKeyboardFocus(sTabCard);
            };
            sTabCard.TextBoxes.Add(PersonalRoleCardViewStruct.LastNameTextBox, LastNameTextBox);
            sTabCard.DocPanels[PersonalRoleCardViewStruct.LastNameDockPanel].Children.Add(LastNameTextBox);

            #endregion

            #region Контрол Имени

            //Border
            var FirstNameBorder = new Border
            {
                CornerRadius = new CornerRadius(6),
                BorderBrush = new SolidColorBrush(Colors.LightGray),
                BorderThickness = new Thickness(2),
                Margin = new Thickness(0, 0, 5, 0),
                HorizontalAlignment = HorizontalAlignment.Left
            };
            sTabCard.Borders.Add(PersonalRoleCardViewStruct.FirstNameBorder, FirstNameBorder);
            sTabCard.DocPanels[PersonalRoleCardViewStruct.ThirdLineDockPanel].Children.Add(FirstNameBorder);
            //Вспомогательная панель
            var FirstNameDockPanel = new DockPanel();
            sTabCard.DocPanels.Add(PersonalRoleCardViewStruct.FirstNameDockPanel, FirstNameDockPanel);
            sTabCard.Borders[PersonalRoleCardViewStruct.FirstNameBorder].Child = FirstNameDockPanel;
            //Текстовый блок
            var FirstNameTextBlock = new TextBlock
            {
                Text = (string)SystemSingleton.Configuration.mainWindow.FindResource("c_FirstName"),
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Left,
                FontSize = 14,
                Margin = new Thickness(5, 0, 5, 0)
            };
            sTabCard.TextBlocks.Add(PersonalRoleCardViewStruct.FirstNameTextBlock, FirstNameTextBlock);
            sTabCard.DocPanels[PersonalRoleCardViewStruct.FirstNameDockPanel].Children.Add(FirstNameTextBlock);
            //Контрол блока 
            var FirstNameTextBox = new TextBox
            {
                Text = ((PersonalRoleCard)sTabCard.Card).FirstName,
                VerticalContentAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Left,
                MinWidth = 150,
                MaxWidth = 300,
                FontSize = 14,
                Height = 40,
                MaxLength = 50,
                IsReadOnly = ((PersonalRoleCard)sTabCard.Card).isEditingNow
            };
            FirstNameTextBox.LostKeyboardFocus += (sender, args) =>
            {
                NameTextBox_LostKeyboardFocus(sTabCard);
            };
            sTabCard.TextBoxes.Add(PersonalRoleCardViewStruct.FirstNameTextBox, FirstNameTextBox);
            sTabCard.DocPanels[PersonalRoleCardViewStruct.FirstNameDockPanel].Children.Add(FirstNameTextBox);

            #endregion

            #region Контрол Полного имени

            //Border
            var FullNameBorder = new Border
            {
                CornerRadius = new CornerRadius(6),
                BorderBrush = new SolidColorBrush(Colors.LightGray),
                BorderThickness = new Thickness(2),
                Margin = new Thickness(0, 0, 5, 0),
                HorizontalAlignment = HorizontalAlignment.Right
            };
            sTabCard.Borders.Add(PersonalRoleCardViewStruct.FullNameBorder, FullNameBorder);
            sTabCard.DocPanels[PersonalRoleCardViewStruct.ThirdLineDockPanel].Children.Add(FullNameBorder);
            //Вспомогательная панель
            var FullNameDockPanel = new DockPanel();
            sTabCard.DocPanels.Add(PersonalRoleCardViewStruct.FullNameDockPanel, FullNameDockPanel);
            sTabCard.Borders[PersonalRoleCardViewStruct.FullNameBorder].Child = FullNameDockPanel;
            //Текстовый блок
            var FullNameTextBlock = new TextBlock
            {
                Text = (string)SystemSingleton.Configuration.mainWindow.FindResource("c_FullName"),
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Left,
                FontSize = 14,
                Margin = new Thickness(5, 0, 5, 0)
            };
            sTabCard.TextBlocks.Add(PersonalRoleCardViewStruct.FullNameTextBlock, FullNameTextBlock);
            sTabCard.DocPanels[PersonalRoleCardViewStruct.FullNameDockPanel].Children.Add(FullNameTextBlock);
            //Контрол блока 
            var FullNameTextBox = new TextBox
            {
                Text = ((PersonalRoleCard)sTabCard.Card).FullName,
                VerticalContentAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Left,
                MinWidth = 150,
                MaxWidth = 300,
                FontSize = 14,
                Height = 40,
                MaxLength = 100,
                IsReadOnly = true
            };
            sTabCard.TextBoxes.Add(PersonalRoleCardViewStruct.FullNameTextBox, FullNameTextBox);
            sTabCard.DocPanels[PersonalRoleCardViewStruct.FullNameDockPanel].Children.Add(FullNameTextBox);

            #endregion

        }

        private static void FillFourthLine(STabCard sTabCard)
        {
            #region Основная панель

            var FourthLineDockPanel = new DockPanel
            {
                Margin = new Thickness(5, 0, 5, 10)
            };
            sTabCard.DocPanels.Add(PersonalRoleCardViewStruct.FourthLineDockPanel, FourthLineDockPanel);
            sTabCard.StackPanels[PersonalRoleCardViewStruct.MainStackPanel].Children.Add(FourthLineDockPanel);

            #endregion

            #region Контрол состояния

            //Border 
            var WorkingTypeBorder = new Border
            {
                CornerRadius = new CornerRadius(6),
                BorderBrush = new SolidColorBrush(Colors.LightGray),
                BorderThickness = new Thickness(2),
                Margin = new Thickness(0, 0, 5, 0),
                HorizontalAlignment = HorizontalAlignment.Left
            };
            sTabCard.Borders.Add(PersonalRoleCardViewStruct.WorkingTypeBorder, WorkingTypeBorder);
            sTabCard.DocPanels[PersonalRoleCardViewStruct.FourthLineDockPanel].Children.Add(WorkingTypeBorder);
            //Вспомогательная панель
            var WorkingTypeDockPanel = new DockPanel();
            sTabCard.DocPanels.Add(PersonalRoleCardViewStruct.WorkingTypeDockPanel, WorkingTypeDockPanel);
            sTabCard.Borders[PersonalRoleCardViewStruct.WorkingTypeBorder].Child = WorkingTypeDockPanel;
            //Текстовый блок
            var WorkingTypeTextBlock = new TextBlock
            {
                Text = (string)SystemSingleton.Configuration.mainWindow.FindResource("m_column_WorkingTypeID"),
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Left,
                FontSize = 14,
                Margin = new Thickness(5, 0, 5, 0)
            };
            sTabCard.TextBlocks.Add(PersonalRoleCardViewStruct.WorkingTypeTextBlock, WorkingTypeTextBlock);
            sTabCard.DocPanels[PersonalRoleCardViewStruct.WorkingTypeDockPanel].Children.Add(WorkingTypeTextBlock);
            //Контрол блока
            var WorkingTypeComboBox = new ComboBox()
            {
                HorizontalContentAlignment = HorizontalAlignment.Left,
                VerticalContentAlignment = VerticalAlignment.Center,
                FontSize = 14,
                Height = 40,
                IsEnabled = !((PersonalRoleCard)sTabCard.Card).isEditingNow
            };
            WorkingTypeComboBox.SelectionChanged += (sender, args) =>
            {
                foreach (var st in ((PersonalRoleCard)sTabCard.Card).AllWorkingTypes.States)
                {
                    if (("States" + st.Name) == ((TextBlock)WorkingTypeComboBox.SelectedItem).Name)
                    {
                        //TODO: sTabCard.StateChanged(st);
                        break;
                    }
                }
            };
            sTabCard.ComboBoxes.Add(PersonalRoleCardViewStruct.WorkingTypeComboBox, WorkingTypeComboBox);
            sTabCard.DocPanels[PersonalRoleCardViewStruct.WorkingTypeDockPanel].Children.Add(WorkingTypeComboBox);
            //Вкладки для комбобокса
            foreach (var item in ((PersonalRoleCard)sTabCard.Card).AllWorkingTypes.States)
            {
                sTabCard.TextBlocks.Add("States" + item.Name, new TextBlock
                {
                    VerticalAlignment = VerticalAlignment.Center,
                    TextAlignment = TextAlignment.Left,
                    FontSize = 14,
                    Text = (string)SystemSingleton.Configuration.mainWindow.FindResource(item.Caption),
                    Name = "States" + item.Name
                });
                sTabCard.ComboBoxes[PersonalRoleCardViewStruct.WorkingTypeComboBox].Items.Add(sTabCard.TextBlocks["States" + item.Name]);
                if (item.ID.Value == ((PersonalRoleCard) sTabCard.Card).WorkingTypeID)
                {
                    WorkingTypeComboBox.SelectedItem = sTabCard.TextBlocks["States" + item.Name];
                }
            }

            #endregion

            #region Контрол isAdmin

            //Border 
            var IsAdminBorder = new Border
            {
                CornerRadius = new CornerRadius(6),
                BorderBrush = new SolidColorBrush(Colors.LightGray),
                BorderThickness = new Thickness(2),
                Margin = new Thickness(5, 0, 5, 0),
                HorizontalAlignment = HorizontalAlignment.Right
            };
            sTabCard.Borders.Add(PersonalRoleCardViewStruct.IsAdminBorder, IsAdminBorder);
            sTabCard.DocPanels[PersonalRoleCardViewStruct.FourthLineDockPanel].Children.Add(IsAdminBorder);
            //Вспомогательная панель
            var IsAdminDockPanel = new DockPanel();
            sTabCard.DocPanels.Add(PersonalRoleCardViewStruct.IsAdminDockPanel, IsAdminDockPanel);
            sTabCard.Borders[PersonalRoleCardViewStruct.IsAdminBorder].Child = IsAdminDockPanel;
            //Текстовый блок
            var IsAdminTextBlock = new TextBlock
            {
                Text = (string)SystemSingleton.Configuration.mainWindow.FindResource("c_IsAdmin"),
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Left,
                FontSize = 14,
                Margin = new Thickness(5, 0, 5, 0)
            };
            sTabCard.TextBlocks.Add(PersonalRoleCardViewStruct.IsAdminTextBlock, IsAdminTextBlock);
            sTabCard.DocPanels[PersonalRoleCardViewStruct.IsAdminDockPanel].Children.Add(IsAdminTextBlock);
            //Контрол блока
            var IsAdminCheckBox = new CheckBox()
            {
                HorizontalContentAlignment = HorizontalAlignment.Left,
                VerticalContentAlignment = VerticalAlignment.Center,
                Margin = new Thickness(5,0,5,0),
                IsChecked = ((PersonalRoleCard)sTabCard.Card).isAdmin,
                IsEnabled = !((PersonalRoleCard)sTabCard.Card).isEditingNow
            };
            sTabCard.CheckBoxes.Add(PersonalRoleCardViewStruct.IsAdminCheckBox, IsAdminCheckBox);
            sTabCard.DocPanels[PersonalRoleCardViewStruct.IsAdminDockPanel].Children.Add(IsAdminCheckBox);

            #endregion
        }

        private static void FillFifthLine(STabCard sTabCard)
        {
            #region Основная панель

            var FifthLineDockPanel = new DockPanel
            {
                Margin = new Thickness(5, 0, 5, 10)
            };
            sTabCard.DocPanels.Add(PersonalRoleCardViewStruct.FifthLineDockPanel, FifthLineDockPanel);
            sTabCard.StackPanels[PersonalRoleCardViewStruct.MainStackPanel].Children.Add(FifthLineDockPanel);

            #endregion

            #region Контрол кнопок

            //Border
            var FifthLineBorder = new Border
            {
                CornerRadius = new CornerRadius(6),
                BorderBrush = new SolidColorBrush(Colors.LightGray),
                BorderThickness = new Thickness(2),
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(5, 0, 5, 0)
            };
            sTabCard.Borders.Add(PersonalRoleCardViewStruct.FifthLineBorder, FifthLineBorder);
            sTabCard.DocPanels[PersonalRoleCardViewStruct.FifthLineDockPanel].Children.Add(FifthLineBorder);
            //Вспомогательная панель
            var FifthLineStackPanel = new StackPanel();
            sTabCard.StackPanels.Add(PersonalRoleCardViewStruct.FifthLineStackPanel, FifthLineStackPanel);
            sTabCard.Borders[PersonalRoleCardViewStruct.FifthLineBorder].Child = FifthLineStackPanel;
            //Кнопка сохранить
            var SaveButton = new Button
            {
                Content = (string)SystemSingleton.Configuration.mainWindow.FindResource("c_SaveCard"),
                Width = 145,
                Height = 25,
                FontSize = 14,
                Margin = new Thickness(5)
            };
            SaveButton.Click += (sender, args) =>
            {
                //sTabCard.SaveUpdatedCard();
            };
            sTabCard.Buttons.Add(PersonalRoleCardViewStruct.SaveButton, SaveButton);
            sTabCard.StackPanels[PersonalRoleCardViewStruct.FifthLineStackPanel].Children.Add(SaveButton);
            //Кнопка delete
            var DeleteButton = new Button
            {
                Content = (string)SystemSingleton.Configuration.mainWindow.FindResource("c_DeleteCard"),
                Width = 145,
                Height = 25,
                FontSize = 14,
                Margin = new Thickness(5)
            };
            DeleteButton.Click += (sender, args) =>
            {
                var dialogResult = MessageBox.Show((string)SystemSingleton.Configuration.mainWindow.FindResource("m_DeleteTaskQ"),
                    (string)SystemSingleton.Configuration.mainWindow.FindResource("m_AttentionHeader"),
                    MessageBoxButton.YesNo);
                if (dialogResult == MessageBoxResult.Yes)
                {
                    //TODO: переделать
                    //try
                    //{
                    //    using (var con = new SqlConnection(SystemSingleton.Configuration.ConnectionString))
                    //    {
                    //        using (var command = new SqlCommand(SqlCommands.DeleteTaskAndStaff, con))
                    //        {
                    //            command.Parameters.Add("@TaskID", SqlDbType.UniqueIdentifier);
                    //            command.Parameters["@TaskID"].Value = sTabCard.Card.Task.ID.Value;
                    //            EnvironmentHelper.SendLogSQL(command.CommandText);
                    //            con.Open();
                    //            int colms = command.ExecuteNonQuery();
                    //            con.Close();
                    //            if (colms == 0)
                    //            {
                    //                EnvironmentHelper.SendDialogBox(
                    //                    (string)SystemSingleton.Configuration.mainWindow.FindResource(
                    //                        "m_CantDeleteTask") + "\n\n" + sTabCard.Card.Task.ID.Value.ToString(),
                    //                    "SQL Error"
                    //                );
                    //            }
                    //        }
                    //    }
                    //    foreach (var fileID in sTabCard.Card.Files.FileDic.Keys)
                    //    {
                    //        sTabCard.RemoveFileFromHardLite(fileID);
                    //    }
                    //}
                    //catch (Exception ex)
                    //{
                    //    EnvironmentHelper.SendErrorDialogBox(ex.Message, "SQL Error", ex.StackTrace);
                    //}
                    //SystemSingleton.Configuration.tabControl.Items.Remove(sTabCard.TabItem);
                    //SystemSingleton.CurrentSession.TabCards.Remove(sTabCard.Card.Task.Number);
                    //EnvironmentHelper.UpdateView();
                }
            };
            sTabCard.Buttons.Add(PersonalRoleCardViewStruct.DeleteButton, DeleteButton);
            sTabCard.StackPanels[PersonalRoleCardViewStruct.FifthLineStackPanel].Children.Add(DeleteButton);
            //Кнопка закрыть
            var CloseButton = new Button
            {
                Content = (string)SystemSingleton.Configuration.mainWindow.FindResource("c_CloseCard"),
                Width = 145,
                Height = 25,
                FontSize = 14,
                Margin = new Thickness(5)
            };
            CloseButton.Click += (sender, args) =>
            {
                //TODO: MessageBoxResult dialogResult = MessageBoxResult.No;
                //if (sTabCard.ChangesFile || sTabCard.ChangesRespond || sTabCard.ChangesState)
                //{
                //    dialogResult = MessageBox.Show((string)SystemSingleton.Configuration.mainWindow.FindResource("m_MakeSureClosingCard"),
                //        (string)SystemSingleton.Configuration.mainWindow.FindResource("m_AttentionHeader"),
                //        MessageBoxButton.YesNo);
                //}
                //if (dialogResult == MessageBoxResult.Yes || (!sTabCard.ChangesFile && !sTabCard.ChangesRespond && !sTabCard.ChangesState))
                //{
                    SystemSingleton.Configuration.tabControl.Items.Remove(sTabCard.TabItem);
                    SystemSingleton.CurrentSession.TabCards.Remove(((PersonalRoleCard)sTabCard.Card).ID.Value);
                //}
                if (!((PersonalRoleCard)sTabCard.Card).isEditingNow)
                {
                    try
                    {
                        using (var con = new SqlConnection(SystemSingleton.Configuration.ConnectionString))
                        {
                            using (var command = new SqlCommand(SqlCommands.SetStopEditingToPersonalRole, con))
                            {
                                command.Parameters.Add("@ID", SqlDbType.UniqueIdentifier);
                                command.Parameters["@ID"].Value = ((PersonalRoleCard)sTabCard.Card).ID.Value;
                                EnvironmentHelper.SendLogSQL(command.CommandText);
                                con.Open();
                                int colms = command.ExecuteNonQuery();
                                con.Close();
                                if (colms == 0)
                                {
                                    EnvironmentHelper.SendDialogBox(
                                        (string)SystemSingleton.Configuration.mainWindow.FindResource(
                                            "m_CantSetEditing") + "\n\n" + ((PersonalRoleCard)sTabCard.Card).ID.Value.ToString(),
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
            sTabCard.Buttons.Add(PersonalRoleCardViewStruct.CloseButton, CloseButton);
            sTabCard.StackPanels[PersonalRoleCardViewStruct.FifthLineStackPanel].Children.Add(CloseButton);

            #endregion
        }

        private static void NameTextBox_LostKeyboardFocus(STabCard sTabCard)
        {
            char tempname = sTabCard.TextBoxes[PersonalRoleCardViewStruct.FirstNameTextBox].Text.Trim().Length == 0
                ? '_'
                : sTabCard.TextBoxes[PersonalRoleCardViewStruct.FirstNameTextBox].Text.Trim()[0];
            var temp = sTabCard.TextBoxes[PersonalRoleCardViewStruct.LastNameTextBox].Text.Trim() + " " +
                       tempname + ".";
            if (sTabCard.TextBoxes.ContainsKey(PersonalRoleCardViewStruct.FullNameTextBox) && 
                temp != sTabCard.TextBoxes[PersonalRoleCardViewStruct.FullNameTextBox].Text)
            {
                sTabCard.TextBoxes[PersonalRoleCardViewStruct.FullNameTextBox].Text = temp;
            }
        }
    }
}
