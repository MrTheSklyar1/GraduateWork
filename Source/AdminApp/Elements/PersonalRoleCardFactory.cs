using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using AdminApp.SupportClasses;
using AdminApp.SystemClasses;

namespace AdminApp.Elements
{
    public static class PersonalRoleCardFactory
    {
        public static WorkingType SelectedType = new WorkingType();
        public static STabCard CreateTab(Guid PersonalID)
        {
            var result = new STabCard();
            result.Card = new PersonalRoleCard(PersonalID);
            result.CardType = StaticTypes.PersonalRole;
            result.isNew = false;
            if (((PersonalRoleCard)result.Card).HasValue)
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
                    if (result.isNew)
                    {
                        SetButtonsNew(result);
                    }
                    else
                    {
                        FillFifthLine(result);
                    }
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
        public static STabCard CreateTab()
        {
            var result = new STabCard();
            result.Card = new PersonalRoleCard();
            result.CardType = StaticTypes.PersonalRole;
            result.isNew = true;
            if (((PersonalRoleCard)result.Card).HasValue)
            {
                result.TabItem = new TabItem
                {
                    Header = (string)SystemSingleton.Configuration.mainWindow.FindResource("m_NewCard"),
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
                    if (result.isNew)
                    {
                        SetButtonsNew(result);
                    }
                    else
                    {
                        FillFifthLine(result);
                    }
                }
                catch
                {
                    EnvironmentHelper.Error(((PersonalRoleCard)result.Card).ID.Value);
                    return null;
                }
                return result;
            }
            else
            {
                EnvironmentHelper.Error(((PersonalRoleCard)result.Card).ID.Value);
                return null;
            }
        }

        private static void SetButtonsNew(STabCard sTabCard)
        {
            #region Основная панель

            var FifthLineDockPanel = new DockPanel
            {
                Margin = new Thickness(5, 0, 5, 10)
            };
            sTabCard.DockPanels.Add(PersonalRoleCardViewStruct.FifthLineDockPanel, FifthLineDockPanel);
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
            sTabCard.DockPanels[PersonalRoleCardViewStruct.FifthLineDockPanel].Children.Add(FifthLineBorder);
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
                NameTextBox_LostKeyboardFocus(sTabCard);
                MessageBoxResult dialogResult = MessageBoxResult.No;
                try
                {
                    int commandInt = 0;
                    string commandtext = PrepareInsertCommand(sTabCard, ref commandInt, false);
                    if (commandtext == "")
                    {
                        EnvironmentHelper.SendDialogBox(
                            (string)SystemSingleton.Configuration.mainWindow.FindResource(
                                "m_NullToSave"), "Fileds Info");
                        return;
                    }
                    if (commandInt > 5)
                    {
                        dialogResult = MessageBox.Show((string)SystemSingleton.Configuration.mainWindow.FindResource("m_MakeSureSavingCard"),
                            (string)SystemSingleton.Configuration.mainWindow.FindResource("m_AttentionHeader"),
                            MessageBoxButton.YesNo);
                    }
                    if (dialogResult == MessageBoxResult.Yes)
                    {
                        using (var con = new SqlConnection(SystemSingleton.Configuration.ConnectionString))
                        {
                            SystemSingleton.Configuration.SqlConnections.Add(con);
                            con.Open();
                            SqlTransaction transaction = con.BeginTransaction();
                            SqlCommand command = con.CreateCommand();
                            command.Transaction = transaction;
                            try
                            {
                                command.CommandText = commandtext;
                                EnvironmentHelper.SendLogSQL(command.CommandText);
                                command.ExecuteNonQuery();
                                transaction.Commit();
                                con.Close();
                            }
                            catch (Exception ex)
                            {
                                EnvironmentHelper.SendDialogBox(
                                    (string)SystemSingleton.Configuration.mainWindow.FindResource(
                                        "m_CantSaveTransaction") + " \n\n " + ex.Message,
                                    "SQL Error"
                                );
                                transaction.Rollback();
                                con.Close();
                                return;
                            }
                        }
                        sTabCard.Card = new PersonalRoleCard(((PersonalRoleCard)sTabCard.Card).ID.Value);
                        sTabCard.isNew = false;
                        ((PersonalRoleCard)sTabCard.Card).isEditingNow = false;
                        EnvironmentHelper.UpdateView();
                        RebuildView(sTabCard);
                        sTabCard.TabItem.Header = ((PersonalRoleCard)sTabCard.Card).FullName;
                    }

                }
                catch (Exception ex)
                {
                    EnvironmentHelper.SendErrorDialogBox(ex.Message, "SQL Error", ex.StackTrace);
                }

            };
            sTabCard.Buttons.Add(PersonalRoleCardViewStruct.SaveButton, SaveButton);
            sTabCard.StackPanels[PersonalRoleCardViewStruct.FifthLineStackPanel].Children.Add(SaveButton);
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
                if (((PersonalRoleCard)sTabCard.Card).isEditingNow)
                {
                    SystemSingleton.Configuration.tabControl.Items.Remove(sTabCard.TabItem);
                    SystemSingleton.CurrentSession.TabCards.Remove(((PersonalRoleCard)sTabCard.Card).ID.Value);
                }
                else
                {
                    MessageBoxResult dialogResult = MessageBoxResult.No;
                    int commandInt = 0;
                    PrepareInsertCommand(sTabCard, ref commandInt, true);
                    if (commandInt > 0)
                    {
                        dialogResult = MessageBox.Show((string)SystemSingleton.Configuration.mainWindow.FindResource("m_MakeSureClosingCard"),
                            (string)SystemSingleton.Configuration.mainWindow.FindResource("m_AttentionHeader"),
                            MessageBoxButton.YesNo);
                        if (dialogResult == MessageBoxResult.No)
                        {
                            return;
                        }
                    }
                    SystemSingleton.Configuration.tabControl.Items.Remove(sTabCard.TabItem);
                    SystemSingleton.CurrentSession.TabCards.Remove(((PersonalRoleCard)sTabCard.Card).ID.Value);
                }
            };
            sTabCard.Buttons.Add(PersonalRoleCardViewStruct.CloseButton, CloseButton);
            sTabCard.StackPanels[PersonalRoleCardViewStruct.FifthLineStackPanel].Children.Add(CloseButton);

            #endregion
        }

        private static void RebuildView(STabCard sTabCard)
        {
            sTabCard.StackPanels[PersonalRoleCardViewStruct.FifthLineStackPanel].Children.Remove(sTabCard.Buttons[PersonalRoleCardViewStruct.CloseButton]);
            sTabCard.Buttons.Remove(PersonalRoleCardViewStruct.CloseButton);
            sTabCard.StackPanels[PersonalRoleCardViewStruct.FifthLineStackPanel].Children.Remove(sTabCard.Buttons[PersonalRoleCardViewStruct.SaveButton]);
            sTabCard.Buttons.Remove(PersonalRoleCardViewStruct.SaveButton);
            sTabCard.Borders[PersonalRoleCardViewStruct.FifthLineBorder].Child = null;
            sTabCard.StackPanels.Remove(PersonalRoleCardViewStruct.FifthLineStackPanel);
            sTabCard.DockPanels[PersonalRoleCardViewStruct.FifthLineDockPanel].Children.Remove(sTabCard.Borders[PersonalRoleCardViewStruct.FifthLineBorder]);
            sTabCard.Borders.Remove(PersonalRoleCardViewStruct.FifthLineBorder);
            sTabCard.StackPanels[PersonalRoleCardViewStruct.MainStackPanel].Children.Remove(sTabCard.DockPanels[PersonalRoleCardViewStruct.FifthLineDockPanel]);
            sTabCard.DockPanels.Remove(PersonalRoleCardViewStruct.FifthLineDockPanel);
            FillFifthLine(sTabCard);
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
            sTabCard.DockPanels.Add(PersonalRoleCardViewStruct.FirstLineDockPanel, FirstLineDockPanel);
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
            sTabCard.DockPanels[PersonalRoleCardViewStruct.FirstLineDockPanel].Children.Add(LoginBorder);
            //Вспомогательная панель
            var LoginDockPanel = new DockPanel();
            sTabCard.DockPanels.Add(PersonalRoleCardViewStruct.LoginDockPanel, LoginDockPanel);
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
            sTabCard.DockPanels[PersonalRoleCardViewStruct.LoginDockPanel].Children.Add(LoginTextBlock);
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
            LoginTextBox.LostKeyboardFocus += (sender, args) =>
            {
                string temp = sTabCard.TextBoxes[PersonalRoleCardViewStruct.LoginTextBox].Text;
                for (int i = 0; i < temp.Length; i++)
                {
                    if (!Char.IsLetterOrDigit(temp[i]))
                    {
                        temp = temp.Remove(i, 1);
                        i--;
                    }
                }
                sTabCard.TextBoxes[PersonalRoleCardViewStruct.LoginTextBox].Text = temp;
            };
            sTabCard.TextBoxes.Add(PersonalRoleCardViewStruct.LoginTextBox, LoginTextBox);
            sTabCard.DockPanels[PersonalRoleCardViewStruct.LoginDockPanel].Children.Add(LoginTextBox);

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
            sTabCard.DockPanels[PersonalRoleCardViewStruct.FirstLineDockPanel].Children.Add(TelegramBorder);
            //Вспомогательная панель 
            var TelegramDockPanel = new DockPanel();
            sTabCard.DockPanels.Add(PersonalRoleCardViewStruct.TelegramDockPanel, TelegramDockPanel);
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
            sTabCard.DockPanels[PersonalRoleCardViewStruct.TelegramDockPanel].Children.Add(TelegramTextBlock);
            //Контрол блока 
            var TelegramTextBox = new TextBox
            {
                Text = ((PersonalRoleCard)sTabCard.Card).TelegramID?.ToString() ?? (string)SystemSingleton.Configuration.mainWindow.FindResource("c_TelegramIDNULL"),
                VerticalContentAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Left,
                MinWidth = 150,
                MaxWidth = 300,
                FontSize = 14,
                Height = 40,
                MaxLength = 14,
                IsReadOnly = ((PersonalRoleCard)sTabCard.Card).isEditingNow
            };
            TelegramTextBox.PreviewTextInput += (sender, args) =>
            {
                Regex regex = new Regex("[^0-9]+");
                args.Handled = regex.IsMatch(args.Text);
            };
            TelegramTextBox.LostKeyboardFocus += (sender, args) =>
            {
                sTabCard.TextBoxes[PersonalRoleCardViewStruct.TelegramTextBox].Text =
                    sTabCard.TextBoxes[PersonalRoleCardViewStruct.TelegramTextBox].Text.Trim();
            };
            sTabCard.TextBoxes.Add(PersonalRoleCardViewStruct.TelegramTextBox, TelegramTextBox);
            sTabCard.DockPanels[PersonalRoleCardViewStruct.TelegramDockPanel].Children.Add(TelegramTextBox);

            #endregion
        }

        private static void FillSecondLine(STabCard sTabCard)
        {
            #region Основная панель

            var SecondLineDockPanel = new DockPanel
            {
                Margin = new Thickness(5, 0, 5, 10)
            };
            sTabCard.DockPanels.Add(PersonalRoleCardViewStruct.SecondLineDockPanel, SecondLineDockPanel);
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
            sTabCard.DockPanels[PersonalRoleCardViewStruct.SecondLineDockPanel].Children.Add(PasswordBorder);
            //Вспомогательная панель
            var PasswordDockPanel = new DockPanel();
            sTabCard.DockPanels.Add(PersonalRoleCardViewStruct.PasswordDockPanel, PasswordDockPanel);
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
            sTabCard.DockPanels[PersonalRoleCardViewStruct.PasswordDockPanel].Children.Add(PasswordTextBlock);
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
            sTabCard.DockPanels[PersonalRoleCardViewStruct.PasswordDockPanel].Children.Add(PasswordPasswordBox);

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
            sTabCard.DockPanels[PersonalRoleCardViewStruct.SecondLineDockPanel].Children.Add(RepeatPasswordBorder);
            //Вспомогательная панель
            var RepeatPasswordDockPanel = new DockPanel();
            sTabCard.DockPanels.Add(PersonalRoleCardViewStruct.RepeatPasswordDockPanel, RepeatPasswordDockPanel);
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
            sTabCard.DockPanels[PersonalRoleCardViewStruct.RepeatPasswordDockPanel].Children.Add(RepeatPasswordTextBlock);
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
            sTabCard.DockPanels[PersonalRoleCardViewStruct.RepeatPasswordDockPanel].Children.Add(RepeatPasswordPasswordBox);

            #endregion
        }

        private static void FillThirdLine(STabCard sTabCard)
        {
            #region Основная панель

            var ThirdLineDockPanel = new DockPanel
            {
                Margin = new Thickness(5, 0, 5, 10)
            };
            sTabCard.DockPanels.Add(PersonalRoleCardViewStruct.ThirdLineDockPanel, ThirdLineDockPanel);
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
            sTabCard.DockPanels[PersonalRoleCardViewStruct.ThirdLineDockPanel].Children.Add(LastNameBorder);
            //Вспомогательная панель
            var LastNameDockPanel = new DockPanel();
            sTabCard.DockPanels.Add(PersonalRoleCardViewStruct.LastNameDockPanel, LastNameDockPanel);
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
            sTabCard.DockPanels[PersonalRoleCardViewStruct.LastNameDockPanel].Children.Add(LastNameTextBlock);
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
            sTabCard.DockPanels[PersonalRoleCardViewStruct.LastNameDockPanel].Children.Add(LastNameTextBox);

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
            sTabCard.DockPanels[PersonalRoleCardViewStruct.ThirdLineDockPanel].Children.Add(FirstNameBorder);
            //Вспомогательная панель
            var FirstNameDockPanel = new DockPanel();
            sTabCard.DockPanels.Add(PersonalRoleCardViewStruct.FirstNameDockPanel, FirstNameDockPanel);
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
            sTabCard.DockPanels[PersonalRoleCardViewStruct.FirstNameDockPanel].Children.Add(FirstNameTextBlock);
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
            sTabCard.DockPanels[PersonalRoleCardViewStruct.FirstNameDockPanel].Children.Add(FirstNameTextBox);

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
            sTabCard.DockPanels[PersonalRoleCardViewStruct.ThirdLineDockPanel].Children.Add(FullNameBorder);
            //Вспомогательная панель
            var FullNameDockPanel = new DockPanel();
            sTabCard.DockPanels.Add(PersonalRoleCardViewStruct.FullNameDockPanel, FullNameDockPanel);
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
            sTabCard.DockPanels[PersonalRoleCardViewStruct.FullNameDockPanel].Children.Add(FullNameTextBlock);
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
            sTabCard.DockPanels[PersonalRoleCardViewStruct.FullNameDockPanel].Children.Add(FullNameTextBox);

            #endregion

        }

        private static void FillFourthLine(STabCard sTabCard)
        {
            #region Основная панель

            var FourthLineDockPanel = new DockPanel
            {
                Margin = new Thickness(5, 0, 5, 10)
            };
            sTabCard.DockPanels.Add(PersonalRoleCardViewStruct.FourthLineDockPanel, FourthLineDockPanel);
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
            sTabCard.DockPanels[PersonalRoleCardViewStruct.FourthLineDockPanel].Children.Add(WorkingTypeBorder);
            //Вспомогательная панель
            var WorkingTypeDockPanel = new DockPanel();
            sTabCard.DockPanels.Add(PersonalRoleCardViewStruct.WorkingTypeDockPanel, WorkingTypeDockPanel);
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
            sTabCard.DockPanels[PersonalRoleCardViewStruct.WorkingTypeDockPanel].Children.Add(WorkingTypeTextBlock);
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
                        SelectedType = st;
                        break;
                    }
                }
            };
            sTabCard.ComboBoxes.Add(PersonalRoleCardViewStruct.WorkingTypeComboBox, WorkingTypeComboBox);
            sTabCard.DockPanels[PersonalRoleCardViewStruct.WorkingTypeDockPanel].Children.Add(WorkingTypeComboBox);
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
                if (item.ID.Value == ((PersonalRoleCard)sTabCard.Card).WorkingTypeID)
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
            sTabCard.DockPanels[PersonalRoleCardViewStruct.FourthLineDockPanel].Children.Add(IsAdminBorder);
            //Вспомогательная панель
            var IsAdminDockPanel = new DockPanel();
            sTabCard.DockPanels.Add(PersonalRoleCardViewStruct.IsAdminDockPanel, IsAdminDockPanel);
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
            sTabCard.DockPanels[PersonalRoleCardViewStruct.IsAdminDockPanel].Children.Add(IsAdminTextBlock);
            //Контрол блока
            var IsAdminCheckBox = new CheckBox()
            {
                HorizontalContentAlignment = HorizontalAlignment.Left,
                VerticalContentAlignment = VerticalAlignment.Center,
                Margin = new Thickness(5, 0, 5, 0),
                IsChecked = ((PersonalRoleCard)sTabCard.Card).isAdmin,
                IsEnabled = !((PersonalRoleCard)sTabCard.Card).isEditingNow
            };
            sTabCard.CheckBoxes.Add(PersonalRoleCardViewStruct.IsAdminCheckBox, IsAdminCheckBox);
            sTabCard.DockPanels[PersonalRoleCardViewStruct.IsAdminDockPanel].Children.Add(IsAdminCheckBox);

            #endregion
        }

        private static void FillFifthLine(STabCard sTabCard)
        {
            #region Основная панель

            var FifthLineDockPanel = new DockPanel
            {
                Margin = new Thickness(5, 0, 5, 10)
            };
            sTabCard.DockPanels.Add(PersonalRoleCardViewStruct.FifthLineDockPanel, FifthLineDockPanel);
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
            sTabCard.DockPanels[PersonalRoleCardViewStruct.FifthLineDockPanel].Children.Add(FifthLineBorder);
            //Вспомогательная панель
            var FifthLineStackPanel = new StackPanel();
            sTabCard.StackPanels.Add(PersonalRoleCardViewStruct.FifthLineStackPanel, FifthLineStackPanel);
            sTabCard.Borders[PersonalRoleCardViewStruct.FifthLineBorder].Child = FifthLineStackPanel;
            if (!((PersonalRoleCard)sTabCard.Card).isEditingNow)
            {
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
                    NameTextBox_LostKeyboardFocus(sTabCard);
                    MessageBoxResult dialogResult = MessageBoxResult.No;
                    try
                    {
                        string commandtext = PrepareSaveCommandWithoutWhere(sTabCard, false);
                        if (commandtext == "")
                        {
                            return;
                        }
                        if (commandtext.Length > 30)
                        {
                            dialogResult = MessageBox.Show((string)SystemSingleton.Configuration.mainWindow.FindResource("m_MakeSureSavingCard"),
                                (string)SystemSingleton.Configuration.mainWindow.FindResource("m_AttentionHeader"),
                                MessageBoxButton.YesNo);
                        }
                        if (dialogResult == MessageBoxResult.Yes)
                        {
                            commandtext += "where ID='" + ((PersonalRoleCard)sTabCard.Card).ID.Value.ToString() + "';";
                            using (var con = new SqlConnection(SystemSingleton.Configuration.ConnectionString))
                            {
                                SystemSingleton.Configuration.SqlConnections.Add(con);
                                con.Open();
                                SqlTransaction transaction = con.BeginTransaction();
                                SqlCommand command = con.CreateCommand();
                                command.Transaction = transaction;
                                try
                                {
                                    command.CommandText = commandtext;
                                    EnvironmentHelper.SendLogSQL(command.CommandText);
                                    command.ExecuteNonQuery();
                                    transaction.Commit();
                                    con.Close();
                                }
                                catch (Exception ex)
                                {
                                    EnvironmentHelper.SendDialogBox(
                                        (string)SystemSingleton.Configuration.mainWindow.FindResource(
                                            "m_CantSaveTransaction") + " \n\n " + ex.Message,
                                        "SQL Error"
                                    );
                                    transaction.Rollback();
                                    con.Close();
                                    return;
                                }
                            }
                            sTabCard.Card = new PersonalRoleCard(((PersonalRoleCard)sTabCard.Card).ID.Value);
                            EnvironmentHelper.UpdateView();
                        }

                    }
                    catch (Exception ex)
                    {
                        EnvironmentHelper.SendErrorDialogBox(ex.Message, "SQL Error", ex.StackTrace);
                    }

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
                    if (CheckDelete(((PersonalRoleCard)sTabCard.Card).ID.Value))
                    {
                        var dialogResult = MessageBox.Show((string)SystemSingleton.Configuration.mainWindow.FindResource("m_DeleteTaskQ"),
                        (string)SystemSingleton.Configuration.mainWindow.FindResource("m_AttentionHeader"),
                        MessageBoxButton.YesNo);
                        if (dialogResult == MessageBoxResult.Yes)
                        {
                            try
                            {
                                using (var con = new SqlConnection(SystemSingleton.Configuration.ConnectionString))
                                {
                                    SystemSingleton.Configuration.SqlConnections.Add(con);
                                    using (var command = new SqlCommand(SqlCommands.DeletePersonalRole, con))
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
                                                    "m_CantDeleteCard") + "\n\n" + ((PersonalRoleCard)sTabCard.Card).ID.Value.ToString(),
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
                            SystemSingleton.Configuration.tabControl.Items.Remove(sTabCard.TabItem);
                            SystemSingleton.CurrentSession.TabCards.Remove(((PersonalRoleCard)sTabCard.Card).ID.Value);
                            EnvironmentHelper.UpdateView();
                        }
                    }
                    else
                    {
                        EnvironmentHelper.SendDialogBox(
                            (string)SystemSingleton.Configuration.mainWindow.FindResource(
                                "m_CantDeleteCardTasks") + "\n\n" + ((PersonalRoleCard)sTabCard.Card).ID.Value.ToString(),
                            "Card Error"
                        );
                    }
                };
                sTabCard.Buttons.Add(PersonalRoleCardViewStruct.DeleteButton, DeleteButton);
                sTabCard.StackPanels[PersonalRoleCardViewStruct.FifthLineStackPanel].Children.Add(DeleteButton);
            }
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
                if (((PersonalRoleCard)sTabCard.Card).isEditingNow)
                {
                    SystemSingleton.Configuration.tabControl.Items.Remove(sTabCard.TabItem);
                    SystemSingleton.CurrentSession.TabCards.Remove(((PersonalRoleCard)sTabCard.Card).ID.Value);
                }
                else
                {
                    MessageBoxResult dialogResult = MessageBoxResult.No;
                    if (PrepareSaveCommandWithoutWhere(sTabCard, true).Length > 30)
                    {
                        dialogResult = MessageBox.Show((string)SystemSingleton.Configuration.mainWindow.FindResource("m_MakeSureClosingCard"),
                            (string)SystemSingleton.Configuration.mainWindow.FindResource("m_AttentionHeader"),
                            MessageBoxButton.YesNo);
                        if (dialogResult == MessageBoxResult.No)
                        {
                            return;
                        }
                    }
                    SystemSingleton.Configuration.tabControl.Items.Remove(sTabCard.TabItem);
                    SystemSingleton.CurrentSession.TabCards.Remove(((PersonalRoleCard)sTabCard.Card).ID.Value);
                    try
                    {
                        using (var con = new SqlConnection(SystemSingleton.Configuration.ConnectionString))
                        {
                            SystemSingleton.Configuration.SqlConnections.Add(con);
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

        private static bool CheckDelete(Guid ID)
        {
            try
            {
                using (var con = new SqlConnection(SystemSingleton.Configuration.ConnectionString))
                {
                    SystemSingleton.Configuration.SqlConnections.Add(con);
                    using (var command = new SqlCommand(SqlCommands.CheckDeletePersonalRole, con))
                    {
                        command.Parameters.Add("@ID", SqlDbType.UniqueIdentifier);
                        command.Parameters["@ID"].Value = ID;
                        EnvironmentHelper.SendLogSQL(command.CommandText);
                        con.Open();
                        int colms = Convert.ToInt32(command.ExecuteScalar());
                        con.Close();
                        if (colms == 0)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EnvironmentHelper.SendErrorDialogBox(ex.Message, "SQL Error", ex.StackTrace);
                return false;
            }
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

        private static string PrepareSaveCommandWithoutWhere(STabCard sTabCard, bool closing)
        {
            if (CheckNULL(sTabCard) && !closing)
            {
                EnvironmentHelper.SendDialogBox(
                    (string)SystemSingleton.Configuration.mainWindow.FindResource(
                        "m_NullFields"), "Fileds Info");
                return "";
            }
            string temppassword =
                            sTabCard.PasswordBoxes[PersonalRoleCardViewStruct.PasswordPasswordBox].Password;
            string temprepeatpassword =
                sTabCard.PasswordBoxes[PersonalRoleCardViewStruct.RepeatPasswordPasswordBox].Password;
            string temppasswordhash = "";
            Int64 tempTelegramID = 0;
            using (var md5Hash = MD5.Create())
            {
                var data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(temppassword));
                var sBuilder = new StringBuilder();
                foreach (var item in data)
                {
                    sBuilder.Append(item.ToString("x2"));
                }

                temppasswordhash = sBuilder.ToString();
            }

            bool newPassword = false;
            if (temppasswordhash != ((PersonalRoleCard)sTabCard.Card).PassWord && temppassword != ((PersonalRoleCard)sTabCard.Card).PassWord)
            {
                newPassword = true;
                if (temppassword != temprepeatpassword)
                {
                    EnvironmentHelper.SendDialogBox(
                        (string)SystemSingleton.Configuration.mainWindow.FindResource(
                            "m_PasswordsNotEq"), "Password Info");
                    return "";
                }
            }

            if (sTabCard.TextBoxes[PersonalRoleCardViewStruct.TelegramTextBox].Text ==
                (string)SystemSingleton.Configuration.mainWindow.FindResource("c_TelegramIDNULL"))
            {
                tempTelegramID = -1;
            }
            else if (sTabCard.TextBoxes[PersonalRoleCardViewStruct.TelegramTextBox].Text != "")
            {
                try
                {
                    tempTelegramID =
                        Convert.ToInt64(sTabCard.TextBoxes[PersonalRoleCardViewStruct.TelegramTextBox]
                            .Text);
                }
                catch
                {
                    EnvironmentHelper.SendDialogBox(
                        (string)SystemSingleton.Configuration.mainWindow.FindResource(
                            "m_CantConvert"), "Telegram Info");
                    return "";
                }
            }
            else
            {
                tempTelegramID = 0;
            }


            string commandtext = "update PersonalRoles set ";
            string futureNewLogin = ((PersonalRoleCard)sTabCard.Card).Login;
            string futureNewPassword = ((PersonalRoleCard)sTabCard.Card).PassWord;
            long? futureNewTelegramID = ((PersonalRoleCard)sTabCard.Card).TelegramID.HasValue
                ? null
                : ((PersonalRoleCard)sTabCard.Card).TelegramID;
            string futureNewLastName = ((PersonalRoleCard)sTabCard.Card).LastName;
            string futureNewFirstName = ((PersonalRoleCard)sTabCard.Card).FirstName;
            Guid futureNewWorkingTypeID = ((PersonalRoleCard)sTabCard.Card).WorkingTypeID;
            bool futureNewIsAdmin = ((PersonalRoleCard)sTabCard.Card).isAdmin;
            if (sTabCard.TextBoxes[PersonalRoleCardViewStruct.LoginTextBox].Text != futureNewLogin)
            {
                futureNewLogin = sTabCard.TextBoxes[PersonalRoleCardViewStruct.LoginTextBox].Text;
                if (!CheckLogin(futureNewLogin))
                {
                    return "";
                }
                commandtext += "Login='" + futureNewLogin + "', ";
            }

            if (newPassword)
            {
                futureNewPassword = temppasswordhash;
                commandtext += "PassWord='" + futureNewPassword + "', ";
            }

            if (tempTelegramID > 0)
            {
                futureNewTelegramID = tempTelegramID;
                commandtext += "TelegramID=" + futureNewTelegramID.Value + ", ";
            }
            else if (tempTelegramID == 0)
            {
                commandtext += "TelegramID=NULL, ";
            }

            if (sTabCard.TextBoxes[PersonalRoleCardViewStruct.LastNameTextBox].Text != futureNewLastName ||
                sTabCard.TextBoxes[PersonalRoleCardViewStruct.FirstNameTextBox].Text != futureNewFirstName)
            {
                futureNewLastName = sTabCard.TextBoxes[PersonalRoleCardViewStruct.LastNameTextBox].Text;
                commandtext += "LastName='" + futureNewLastName + "', ";
                futureNewFirstName = sTabCard.TextBoxes[PersonalRoleCardViewStruct.FirstNameTextBox].Text;
                commandtext += "FirstName='" + futureNewFirstName + "', ";
                commandtext += "FullName='" + sTabCard.TextBoxes[PersonalRoleCardViewStruct.FullNameTextBox].Text + "', ";
            }

            if (futureNewWorkingTypeID != SelectedType.ID.Value)
            {
                futureNewWorkingTypeID = SelectedType.ID.Value;
                commandtext += "WorkingTypeID='" + futureNewWorkingTypeID + "', ";
            }

            var isChecked = sTabCard.CheckBoxes[PersonalRoleCardViewStruct.IsAdminCheckBox].IsChecked;
            if (isChecked != null && futureNewIsAdmin != isChecked.Value)
            {
                futureNewIsAdmin = sTabCard.CheckBoxes[PersonalRoleCardViewStruct.IsAdminCheckBox]
                    .IsChecked.Value;
                int isadmin = futureNewIsAdmin ? 1 : 0;
                commandtext += "isAdmin=" + isadmin + ", ";
            }

            if (commandtext[commandtext.Length - 2] == ',') commandtext = commandtext.Remove(commandtext.Length - 2, 1);
            return commandtext;
        }
        private static string PrepareInsertCommand(STabCard sTabCard, ref int num, bool closing)
        {
            num = 0;
            if (CheckNULL(sTabCard) && !closing)
            {
                EnvironmentHelper.SendDialogBox(
                    (string)SystemSingleton.Configuration.mainWindow.FindResource(
                        "m_NullFields"), "Fileds Info");
                return "";
            }
            string temppassword =
                            sTabCard.PasswordBoxes[PersonalRoleCardViewStruct.PasswordPasswordBox].Password;
            string temprepeatpassword =
                sTabCard.PasswordBoxes[PersonalRoleCardViewStruct.RepeatPasswordPasswordBox].Password;
            string temppasswordhash = "";
            Int64 tempTelegramID = 0;
            using (var md5Hash = MD5.Create())
            {
                var data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(temppassword));
                var sBuilder = new StringBuilder();
                foreach (var item in data)
                {
                    sBuilder.Append(item.ToString("x2"));
                }

                temppasswordhash = sBuilder.ToString();
            }

            bool newPassword = false;
            if (sTabCard.PasswordBoxes[PersonalRoleCardViewStruct.PasswordPasswordBox].Password != "")
            {
                newPassword = true;
                if (temppassword != temprepeatpassword)
                {
                    EnvironmentHelper.SendDialogBox(
                        (string)SystemSingleton.Configuration.mainWindow.FindResource(
                            "m_PasswordsNotEq"), "Password Info");
                    return "";
                }
            }

            if (sTabCard.TextBoxes[PersonalRoleCardViewStruct.TelegramTextBox].Text ==
                (string)SystemSingleton.Configuration.mainWindow.FindResource("c_TelegramIDNULL"))
            {
                tempTelegramID = -1;
            }
            else if (sTabCard.TextBoxes[PersonalRoleCardViewStruct.TelegramTextBox].Text != "")
            {
                try
                {
                    tempTelegramID =
                        Convert.ToInt64(sTabCard.TextBoxes[PersonalRoleCardViewStruct.TelegramTextBox]
                            .Text);
                }
                catch
                {
                    EnvironmentHelper.SendDialogBox(
                        (string)SystemSingleton.Configuration.mainWindow.FindResource(
                            "m_CantConvert"), "Telegram Info");
                    return "";
                }
            }
            else
            {
                tempTelegramID = 0;
            }


            string commandtext = "insert into PersonalRoles (ID, ";
            string commandvalues = "values ('" + ((PersonalRoleCard)sTabCard.Card).ID.Value.ToString() + "', ";
            string futureNewLogin = ((PersonalRoleCard)sTabCard.Card).Login;
            string futureNewPassword = ((PersonalRoleCard)sTabCard.Card).PassWord;
            long? futureNewTelegramID = ((PersonalRoleCard)sTabCard.Card).TelegramID.HasValue
                ? null
                : ((PersonalRoleCard)sTabCard.Card).TelegramID;
            string futureNewLastName = ((PersonalRoleCard)sTabCard.Card).LastName;
            string futureNewFirstName = ((PersonalRoleCard)sTabCard.Card).FirstName;
            Guid futureNewWorkingTypeID = ((PersonalRoleCard)sTabCard.Card).WorkingTypeID;
            bool futureNewIsAdmin = ((PersonalRoleCard)sTabCard.Card).isAdmin;
            if (sTabCard.TextBoxes[PersonalRoleCardViewStruct.LoginTextBox].Text != futureNewLogin)
            {
                futureNewLogin = sTabCard.TextBoxes[PersonalRoleCardViewStruct.LoginTextBox].Text;
                if (!CheckLogin(futureNewLogin)) return "";
                commandtext += "Login, ";
                commandvalues += "'" + futureNewLogin + "', ";
                num++;
            }

            if (newPassword)
            {
                futureNewPassword = temppasswordhash;
                commandtext += "PassWord, ";
                commandvalues += "'" + futureNewPassword + "', ";
                num++;
            }

            if (tempTelegramID > 0)
            {
                futureNewTelegramID = tempTelegramID;
                commandtext += "TelegramID, ";
                commandvalues += futureNewTelegramID.Value + ", ";
                num++;

            }
            else
            {
                commandtext += "TelegramID, ";
                commandvalues += "NULL, ";
                num++;
            }

            if (sTabCard.TextBoxes[PersonalRoleCardViewStruct.LastNameTextBox].Text != futureNewLastName ||
                sTabCard.TextBoxes[PersonalRoleCardViewStruct.FirstNameTextBox].Text != futureNewFirstName)
            {
                futureNewLastName = sTabCard.TextBoxes[PersonalRoleCardViewStruct.LastNameTextBox].Text;
                commandtext += "LastName, ";
                commandvalues += "'" + futureNewLastName + "', ";
                futureNewFirstName = sTabCard.TextBoxes[PersonalRoleCardViewStruct.FirstNameTextBox].Text;
                commandtext += "FirstName, ";
                commandvalues += "'" + futureNewFirstName + "', ";
                commandtext += "FullName, ";
                commandvalues += "'" + sTabCard.TextBoxes[PersonalRoleCardViewStruct.FullNameTextBox].Text + "', ";
                num++;
            }

            if (SelectedType.ID.Value != Guid.Empty)
            {
                futureNewWorkingTypeID = SelectedType.ID.Value;
                commandtext += "WorkingTypeID, ";
                commandvalues += "'" + futureNewWorkingTypeID + "', ";
                num++;
            }

            var isChecked = sTabCard.CheckBoxes[PersonalRoleCardViewStruct.IsAdminCheckBox].IsChecked;
            if (isChecked != null)
            {
                futureNewIsAdmin = sTabCard.CheckBoxes[PersonalRoleCardViewStruct.IsAdminCheckBox]
                    .IsChecked.Value;
                int isadmin = futureNewIsAdmin ? 1 : 0;
                commandtext += "isAdmin, ";
                commandvalues += isadmin + ", ";
                num++;
            }
            else
            {
                commandtext += "isAdmin, ";
                commandvalues += "0, ";
                num++;
            }
            commandtext += "isEditingNow) ";
            commandvalues += "1);";
            string insertroles = "insert into Roles values ('" + ((PersonalRoleCard)sTabCard.Card).ID.Value.ToString() + "', '" + sTabCard.TextBoxes[PersonalRoleCardViewStruct.FullNameTextBox].Text + "');";
            string inserttostatic = "insert into RoleUsers values ('fffee627-a5a6-4345-bc55-8fba3709dc48', '" + ((PersonalRoleCard)sTabCard.Card).ID.Value.ToString() + "');";
            string insertBotStat = "";
            insertBotStat = "insert into BotStat values (" + ((PersonalRoleCard)sTabCard.Card).ID.Value + ", 0, null, 1, null, 1, 1, 1)";
            sTabCard.PasswordBoxes[PersonalRoleCardViewStruct.PasswordPasswordBox].Password = temppassword;
            sTabCard.PasswordBoxes[PersonalRoleCardViewStruct.RepeatPasswordPasswordBox].Password = temppassword;
            return insertroles + commandtext + commandvalues + inserttostatic + insertBotStat;
        }

        private static bool CheckLogin(string login)
        {
            try
            {
                using (var con = new SqlConnection(SystemSingleton.Configuration.ConnectionString))
                {
                    SystemSingleton.Configuration.SqlConnections.Add(con);
                    using (var command = new SqlCommand(SqlCommands.CheckLoginCommand, con))
                    {
                        command.Parameters.Add("@Login", SqlDbType.NVarChar);
                        command.Parameters["@Login"].Value = login;
                        EnvironmentHelper.SendLogSQL(command.CommandText);
                        con.Open();
                        int colms = Convert.ToInt32(command.ExecuteScalar());
                        con.Close();
                        if (colms > 0)
                        {
                            EnvironmentHelper.SendDialogBox(
                                (string)SystemSingleton.Configuration.mainWindow.FindResource(
                                    "m_CantSetLogin") + "\n\n" + login,
                                "Login Error"
                            );
                            return false;
                        }
                        else
                        {
                            return true;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                EnvironmentHelper.SendErrorDialogBox(ex.Message, "SQL Error", ex.StackTrace);
                return false;
            }
        }

        private static bool CheckNULL(STabCard sTabCard)
        {
            if (sTabCard.TextBoxes[PersonalRoleCardViewStruct.LoginTextBox].Text == "" ||
                sTabCard.PasswordBoxes[PersonalRoleCardViewStruct.PasswordPasswordBox].Password == "" ||
                sTabCard.PasswordBoxes[PersonalRoleCardViewStruct.RepeatPasswordPasswordBox].Password == "" ||
                sTabCard.TextBoxes[PersonalRoleCardViewStruct.LastNameTextBox].Text == "" ||
                sTabCard.TextBoxes[PersonalRoleCardViewStruct.FirstNameTextBox].Text == "")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
