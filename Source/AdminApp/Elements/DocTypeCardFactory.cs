using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using AdminApp.SupportClasses;
using AdminApp.SystemClasses;

namespace AdminApp.Elements
{
    public static class DocTypeCardFactory
    {
        public static STabCard CreateTab(Guid RoleID)
        {
            var result = new STabCard();
            result.Card = new DocTypeCard(RoleID);
            result.CardType = StaticTypes.DocType;
            result.isNew = false;
            if (((DocTypeCard)result.Card).HasValue)
            {
                result.TabItem = new TabItem
                {
                    Header = ((DocTypeCard)result.Card).Caption,
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
                }
                catch (Exception ex)
                {
                    EnvironmentHelper.Error(RoleID);
                    return null;
                }

                return result;
            }
            else
            {
                EnvironmentHelper.Error(RoleID);
                return null;
            }
        }

        public static STabCard CreateTab()
        {
            var result = new STabCard();
            result.Card = new DocTypeCard();
            result.CardType = StaticTypes.DocType;
            result.isNew = true;
            if (((DocTypeCard)result.Card).HasValue)
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
                    SetButtonsNew(result);
                }
                catch (Exception ex)
                {
                    EnvironmentHelper.Error(((DocTypeCard)result.Card).ID.Value);
                    return null;
                }

                return result;
            }
            else
            {
                EnvironmentHelper.Error(((DocTypeCard)result.Card).ID.Value);
                return null;
            }
        }

        private static void NameTextBox_LostKeyboardFocus(STabCard sTabCard)
        {
            if (((DocTypeCard)sTabCard.Card).isEditingNow) return;
            string temp = sTabCard.TextBoxes[DocTypeCardViewStruct.NameTextBox].Text;
            for (int i = 0; i < temp.Length; i++)
            {
                if (!Char.IsLetterOrDigit(temp[i]))
                {
                    temp = temp.Remove(i, 1);
                    i--;
                }
            }

            sTabCard.TextBoxes[DocTypeCardViewStruct.NameTextBox].Text = temp;
        }

        private static void RebuildView(STabCard sTabCard)
        {
            sTabCard.StackPanels[DocTypeCardViewStruct.FourthLineStackPanel].Children
                .Remove(sTabCard.Buttons[DocTypeCardViewStruct.CloseButton]);
            sTabCard.Buttons.Remove(DocTypeCardViewStruct.CloseButton);
            sTabCard.StackPanels[DocTypeCardViewStruct.FourthLineStackPanel].Children
                .Remove(sTabCard.Buttons[DocTypeCardViewStruct.SaveButton]);
            sTabCard.Buttons.Remove(DocTypeCardViewStruct.SaveButton);
            sTabCard.Borders[DocTypeCardViewStruct.FourthLineBorder].Child = null;
            sTabCard.StackPanels.Remove(DocTypeCardViewStruct.FourthLineStackPanel);
            sTabCard.StackPanels[DocTypeCardViewStruct.MainStackPanel].Children
                .Remove(sTabCard.Borders[DocTypeCardViewStruct.FourthLineBorder]);
            sTabCard.Borders.Remove(DocTypeCardViewStruct.FourthLineBorder);
            FillFourthLine(sTabCard);
        }

        private static void FillFourthLine(STabCard sTabCard)
        {
            #region Контрол кнопок

            //Border
            var FourthLineBorder = new Border
            {
                CornerRadius = new CornerRadius(6),
                BorderBrush = new SolidColorBrush(Colors.LightGray),
                BorderThickness = new Thickness(2),
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(5, 0, 5, 0)
            };
            sTabCard.Borders.Add(DocTypeCardViewStruct.FourthLineBorder, FourthLineBorder);
            sTabCard.StackPanels[DocTypeCardViewStruct.MainStackPanel].Children.Add(FourthLineBorder);
            //Вспомогательная панель
            var FourthLineStackPanel = new StackPanel();
            sTabCard.StackPanels.Add(DocTypeCardViewStruct.FourthLineStackPanel, FourthLineStackPanel);
            sTabCard.Borders[DocTypeCardViewStruct.FourthLineBorder].Child = FourthLineStackPanel;
            if (!((DocTypeCard)sTabCard.Card).isEditingNow)
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
                        int num = 0;
                        string commandtext = PrepareSaveCommandWithoutWhere(sTabCard, ref num);
                        if (commandtext == "")
                        {
                            return;
                        }
                        if (num > 0)
                        {
                            dialogResult = MessageBox.Show((string)SystemSingleton.Configuration.mainWindow.FindResource("m_MakeSureSavingCard"),
                                (string)SystemSingleton.Configuration.mainWindow.FindResource("m_AttentionHeader"),
                                MessageBoxButton.YesNo);
                        }
                        if (dialogResult == MessageBoxResult.Yes)
                        {
                            commandtext += "where ID='" + ((DocTypeCard)sTabCard.Card).ID.Value.ToString() + "';";
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
                            sTabCard.Card = new DocTypeCard(((DocTypeCard)sTabCard.Card).ID.Value);
                            ((DocTypeCard)sTabCard.Card).isEditingNow = false;
                            EnvironmentHelper.UpdateView();
                        }
                    }
                    catch (Exception ex)
                    {
                        EnvironmentHelper.SendErrorDialogBox(ex.Message, "SQL Error", ex.StackTrace);
                    }

                };
                sTabCard.Buttons.Add(DocTypeCardViewStruct.SaveButton, SaveButton);
                sTabCard.StackPanels[DocTypeCardViewStruct.FourthLineStackPanel].Children.Add(SaveButton);
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
                    if (CheckDelete(((DocTypeCard)sTabCard.Card).ID.Value))
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
                                    using (var command = new SqlCommand(SqlCommands.DeleteDocType, con))
                                    {
                                        command.Parameters.Add("@ID", SqlDbType.UniqueIdentifier);
                                        command.Parameters["@ID"].Value = ((DocTypeCard)sTabCard.Card).ID.Value;
                                        EnvironmentHelper.SendLogSQL(command.CommandText);
                                        con.Open();
                                        int colms = command.ExecuteNonQuery();
                                        con.Close();
                                        if (colms == 0)
                                        {
                                            EnvironmentHelper.SendDialogBox(
                                                (string)SystemSingleton.Configuration.mainWindow.FindResource(
                                                    "m_CantDeleteCard") + "\n\n" + ((DocTypeCard)sTabCard.Card).ID.Value.ToString(),
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
                            SystemSingleton.CurrentSession.TabCards.Remove(((DocTypeCard)sTabCard.Card).ID.Value);
                            EnvironmentHelper.UpdateView();
                        }
                    }
                    else
                    {
                        EnvironmentHelper.SendDialogBox(
                            (string)SystemSingleton.Configuration.mainWindow.FindResource(
                                "m_CantDeleteCardTasksCardReview") + "\n\n" + ((DocTypeCard)sTabCard.Card).ID.Value.ToString(),
                            "Card Error"
                        );
                    }
                };
                sTabCard.Buttons.Add(DocTypeCardViewStruct.DeleteButton, DeleteButton);
                sTabCard.StackPanels[DocTypeCardViewStruct.FourthLineStackPanel].Children.Add(DeleteButton);
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
                if (((DocTypeCard)sTabCard.Card).isEditingNow)
                {
                    SystemSingleton.Configuration.tabControl.Items.Remove(sTabCard.TabItem);
                    SystemSingleton.CurrentSession.TabCards.Remove(((DocTypeCard)sTabCard.Card).ID.Value);
                }
                else
                {
                    MessageBoxResult dialogResult = MessageBoxResult.No;
                    int num = 0;
                    PrepareSaveCommandWithoutWhere(sTabCard, ref num);
                    if (num > 0)
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
                    SystemSingleton.CurrentSession.TabCards.Remove(((DocTypeCard)sTabCard.Card).ID.Value);
                    try
                    {
                        using (var con = new SqlConnection(SystemSingleton.Configuration.ConnectionString))
                        {
                            SystemSingleton.Configuration.SqlConnections.Add(con);
                            using (var command = new SqlCommand(SqlCommands.SetStopEditingToDocType, con))
                            {
                                command.Parameters.Add("@ID", SqlDbType.UniqueIdentifier);
                                command.Parameters["@ID"].Value = ((DocTypeCard)sTabCard.Card).ID.Value;
                                EnvironmentHelper.SendLogSQL(command.CommandText);
                                con.Open();
                                int colms = command.ExecuteNonQuery();
                                con.Close();
                                if (colms == 0)
                                {
                                    EnvironmentHelper.SendDialogBox(
                                        (string)SystemSingleton.Configuration.mainWindow.FindResource(
                                            "m_CantSetEditing") + "\n\n" + ((DocTypeCard)sTabCard.Card).ID.Value.ToString(),
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
            sTabCard.Buttons.Add(DocTypeCardViewStruct.CloseButton, CloseButton);
            sTabCard.StackPanels[DocTypeCardViewStruct.FourthLineStackPanel].Children.Add(CloseButton);

            #endregion
        }

        private static void FillMainStackPanelToTab(STabCard sTabCard)
        {
            var MainStackPanel = new StackPanel
            {
                Margin = new Thickness(10)
            };
            sTabCard.StackPanels.Add(DocTypeCardViewStruct.MainStackPanel, MainStackPanel);
            sTabCard.TabItem.Content = MainStackPanel;
        }

        private static void FillFirstLine(STabCard sTabCard)
        {

            #region Основная панель

            var FirstLineDockPanel = new DockPanel
            {
                Margin = new Thickness(5, 0, 5, 10)
            };
            sTabCard.DockPanels.Add(DocTypeCardViewStruct.FirstLineDockPanel, FirstLineDockPanel);
            sTabCard.StackPanels[DocTypeCardViewStruct.MainStackPanel].Children.Add(FirstLineDockPanel);

            #endregion

            #region Контрол Названия

            //Border
            var NameBorder = new Border
            {
                CornerRadius = new CornerRadius(6),
                BorderBrush = new SolidColorBrush(Colors.LightGray),
                BorderThickness = new Thickness(2),
                Margin = new Thickness(0, 0, 5, 0),
                HorizontalAlignment = HorizontalAlignment.Left
            };
            sTabCard.Borders.Add(DocTypeCardViewStruct.NameBorder, NameBorder);
            sTabCard.DockPanels[DocTypeCardViewStruct.FirstLineDockPanel].Children.Add(NameBorder);
            //Вспомогательная панель
            var NameDockPanel = new DockPanel();
            sTabCard.DockPanels.Add(DocTypeCardViewStruct.NameDockPanel, NameDockPanel);
            sTabCard.Borders[DocTypeCardViewStruct.NameBorder].Child = NameDockPanel;
            //Текстовый блок
            var NameTextBlock = new TextBlock
            {
                Text = (string)SystemSingleton.Configuration.mainWindow.FindResource("m_NameRole"),
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Left,
                FontSize = 14,
                Margin = new Thickness(5, 0, 5, 0)
            };
            sTabCard.TextBlocks.Add(DocTypeCardViewStruct.NameTextBlock, NameTextBlock);
            sTabCard.DockPanels[DocTypeCardViewStruct.NameDockPanel].Children.Add(NameTextBlock);
            //Контрол блока 
            var NameTextBox = new TextBox
            {
                Text = ((DocTypeCard)sTabCard.Card).Name,
                VerticalContentAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Left,
                MinWidth = 150,
                MaxWidth = 300,
                FontSize = 14,
                Height = 40,
                MaxLength = 50,
                IsReadOnly = ((DocTypeCard)sTabCard.Card).isEditingNow
            };
            NameTextBox.LostKeyboardFocus += (sender, args) => { NameTextBox_LostKeyboardFocus(sTabCard); };
            sTabCard.TextBoxes.Add(DocTypeCardViewStruct.NameTextBox, NameTextBox);
            sTabCard.DockPanels[DocTypeCardViewStruct.NameDockPanel].Children.Add(NameTextBox);

            #endregion

            #region Контрол надписи

            //Border
            var CaptionBorder = new Border
            {
                CornerRadius = new CornerRadius(6),
                BorderBrush = new SolidColorBrush(Colors.LightGray),
                BorderThickness = new Thickness(2),
                Margin = new Thickness(5, 0, 5, 0),
                HorizontalAlignment = HorizontalAlignment.Left
            };
            sTabCard.Borders.Add(DocTypeCardViewStruct.CaptionBorder, CaptionBorder);
            sTabCard.DockPanels[DocTypeCardViewStruct.FirstLineDockPanel].Children.Add(CaptionBorder);
            //Вспомогательная панель 
            var CaptionDockPanel = new DockPanel();
            sTabCard.DockPanels.Add(DocTypeCardViewStruct.CaptionDockPanel, CaptionDockPanel);
            sTabCard.Borders[DocTypeCardViewStruct.CaptionBorder].Child = CaptionDockPanel;
            //Текстовый блок
            var CaptionTextBlock = new TextBlock
            {
                Text = (string)SystemSingleton.Configuration.mainWindow.FindResource("m_CaptionStatic"),
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Left,
                FontSize = 14,
                Margin = new Thickness(5, 0, 5, 0)
            };
            sTabCard.TextBlocks.Add(DocTypeCardViewStruct.CaptionTextBlock, CaptionTextBlock);
            sTabCard.DockPanels[DocTypeCardViewStruct.CaptionDockPanel].Children.Add(CaptionTextBlock);
            //Контрол блока 
            var CaptionTextBox = new TextBox
            {
                Text = ((DocTypeCard)sTabCard.Card).Caption,
                VerticalContentAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Left,
                MinWidth = 150,
                MaxWidth = 300,
                FontSize = 14,
                Height = 40,
                MaxLength = 50,
                IsReadOnly = ((DocTypeCard)sTabCard.Card).isEditingNow
            };
            CaptionTextBox.LostKeyboardFocus += (sender, args) =>
            {
                sTabCard.TextBoxes[DocTypeCardViewStruct.CaptionTextBox].Text =
                    sTabCard.TextBoxes[DocTypeCardViewStruct.CaptionTextBox].Text.Trim();
            };
            sTabCard.TextBoxes.Add(DocTypeCardViewStruct.CaptionTextBox, CaptionTextBox);
            sTabCard.DockPanels[DocTypeCardViewStruct.CaptionDockPanel].Children.Add(CaptionTextBox);

            #endregion
        }

        private static void FillSecondLine(STabCard sTabCard)
        {
            #region Основной Border

            var SecondLineBorder = new Border
            {
                CornerRadius = new CornerRadius(6),
                BorderBrush = new SolidColorBrush(Colors.LightGray),
                BorderThickness = new Thickness(2),
                Margin = new Thickness(5, 0, 5, 10)
            };
            sTabCard.Borders.Add(DocTypeCardViewStruct.SecondLineBorder, SecondLineBorder);
            sTabCard.StackPanels[DocTypeCardViewStruct.MainStackPanel].Children.Add(SecondLineBorder);

            #endregion

            #region Контрол текста

            //Вспомогательная панель
            var SecondLineStackPanel = new StackPanel();
            sTabCard.StackPanels.Add(DocTypeCardViewStruct.SecondLineStackPanel, SecondLineStackPanel);
            sTabCard.Borders[DocTypeCardViewStruct.SecondLineBorder].Child = SecondLineStackPanel;
            //Текстовый блок
            var SecondLineTextBlock = new TextBlock
            {
                Text = (string)SystemSingleton.Configuration.mainWindow.FindResource("c_Tags"),
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Left,
                HorizontalAlignment = HorizontalAlignment.Left,
                FontSize = 14,
                Width = 200,
                Margin = new Thickness(5, 0, 0, 0)
            };
            sTabCard.TextBlocks.Add(DocTypeCardViewStruct.SecondLineTextBlock, SecondLineTextBlock);
            sTabCard.StackPanels[DocTypeCardViewStruct.SecondLineStackPanel].Children.Add(SecondLineTextBlock);
            //Контрол блока
            var SecondLineTextBox = new TextBox
            {
                Text = ((DocTypeCard)sTabCard.Card).TagWords,
                VerticalScrollBarVisibility = ScrollBarVisibility.Visible,
                TextAlignment = TextAlignment.Left,
                AcceptsReturn = true,
                MinWidth = 725,
                FontSize = 14,
                MinHeight = 40,
                MaxHeight = 100,
                Margin = new Thickness(5),
                IsReadOnly = ((DocTypeCard)sTabCard.Card).isEditingNow
            };
            sTabCard.TextBoxes.Add(DocTypeCardViewStruct.SecondLineTextBox, SecondLineTextBox);
            sTabCard.StackPanels[DocTypeCardViewStruct.SecondLineStackPanel].Children.Add(SecondLineTextBox);

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
                Margin = new Thickness(5, 0, 5, 10),
                HorizontalAlignment = HorizontalAlignment.Left
            };
            sTabCard.Borders.Add(DocTypeCardViewStruct.ThirdLineBorder, ThirdLineBorder);
            sTabCard.StackPanels[DocTypeCardViewStruct.MainStackPanel].Children.Add(ThirdLineBorder);

            #endregion

            #region Контрол текста

            //Вспомогательная панель
            var ThirdLineDockPanel = new DockPanel();
            sTabCard.DockPanels.Add(DocTypeCardViewStruct.ThirdLineDockPanel, ThirdLineDockPanel);
            sTabCard.Borders[DocTypeCardViewStruct.ThirdLineBorder].Child = ThirdLineDockPanel;
            //Текстовый блок
            var ThirdLineTextBlock = new TextBlock
            {
                Text = (string)SystemSingleton.Configuration.mainWindow.FindResource("c_ForRole"),
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Left,
                HorizontalAlignment = HorizontalAlignment.Left,
                FontSize = 14,
                Margin = new Thickness(5, 0, 5, 0)
            };
            sTabCard.TextBlocks.Add(DocTypeCardViewStruct.ThirdLineTextBlock, ThirdLineTextBlock);
            sTabCard.DockPanels[DocTypeCardViewStruct.ThirdLineDockPanel].Children.Add(ThirdLineTextBlock);
            //Контрол блока 
            var ThirdLineTextBox = new TextBox
            {
                Text = ((DocTypeCard)sTabCard.Card).RoleCard.Name ?? "",
                VerticalContentAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Left,
                MinWidth = 50,
                MaxWidth = 300,
                FontSize = 14,
                Height = 40,
                MaxLength = 50,
                IsReadOnly = true
            };
            sTabCard.TextBoxes.Add(DocTypeCardViewStruct.ThirdLineTextBox, ThirdLineTextBox);
            sTabCard.DockPanels[DocTypeCardViewStruct.ThirdLineDockPanel].Children.Add(ThirdLineTextBox);
            //Кнопка
            var ThirdLineButton = new Button()
            {
                Content = "...",
                Margin = new Thickness(5),
                FontSize = 14,
                Width = 30,
                IsEnabled = !((DocTypeCard)sTabCard.Card).isEditingNow
            };
            ThirdLineButton.Click += (sender, args) =>
            {
                var Window = new ChoseFromAllRolesView();
                SystemSingleton.CurrentSession.ViewChoose = Window;
                SystemSingleton.CurrentSession.ViewChoose.ShowDialog();
                SystemSingleton.CurrentSession.ViewChoose = null;
                if (SystemSingleton.CurrentSession.ChosenIDForDocType == Guid.Empty)
                {
                    return;
                }
                else
                {
                    ((DocTypeCard)sTabCard.Card).RoleCard = new RoleCard(SystemSingleton.CurrentSession.ChosenIDForDocType);
                    if (((DocTypeCard)sTabCard.Card).RoleCard.HasValue)
                        ((DocTypeCard)sTabCard.Card).NewRoleCard = ((DocTypeCard)sTabCard.Card).RoleCard.ID.Value;
                    sTabCard.TextBoxes[DocTypeCardViewStruct.ThirdLineTextBox].Text =
                        ((DocTypeCard)sTabCard.Card).RoleCard.Name;
                    SystemSingleton.CurrentSession.ChosenIDForStaticRole = Guid.Empty;
                }
            };
            sTabCard.Buttons.Add(DocTypeCardViewStruct.ThirdLineButton, ThirdLineButton);
            sTabCard.DockPanels[DocTypeCardViewStruct.ThirdLineDockPanel].Children.Add(ThirdLineButton);
            #endregion
        }

        private static void SetButtonsNew(STabCard sTabCard)
        {
            #region Контрол кнопок

            //Border
            var FourthLineBorder = new Border
            {
                CornerRadius = new CornerRadius(6),
                BorderBrush = new SolidColorBrush(Colors.LightGray),
                BorderThickness = new Thickness(2),
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(5, 0, 5, 0)
            };
            sTabCard.Borders.Add(DocTypeCardViewStruct.FourthLineBorder, FourthLineBorder);
            sTabCard.StackPanels[DocTypeCardViewStruct.MainStackPanel].Children.Add(FourthLineBorder);
            //Вспомогательная панель
            var FourthLineStackPanel = new StackPanel();
            sTabCard.StackPanels.Add(DocTypeCardViewStruct.FourthLineStackPanel, FourthLineStackPanel);
            sTabCard.Borders[DocTypeCardViewStruct.FourthLineBorder].Child = FourthLineStackPanel;
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
                    string commandtext = PrepareInsertCommand(sTabCard, ref commandInt);
                    if (commandtext == "")
                    {
                        EnvironmentHelper.SendDialogBox(
                            (string)SystemSingleton.Configuration.mainWindow.FindResource(
                                "m_NullToSave"), "Fileds Info");
                        return;
                    }
                    if (commandInt > 3)
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
                        sTabCard.Card = new DocTypeCard(((DocTypeCard)sTabCard.Card).ID.Value);
                        sTabCard.isNew = false;
                        ((DocTypeCard)sTabCard.Card).isEditingNow = false;
                        EnvironmentHelper.UpdateView();
                        RebuildView(sTabCard);
                        sTabCard.TabItem.Header = ((DocTypeCard)sTabCard.Card).Caption;
                    }
                }
                catch (Exception ex)
                {
                    EnvironmentHelper.SendErrorDialogBox(ex.Message, "SQL Error", ex.StackTrace);
                }

            };
            sTabCard.Buttons.Add(DocTypeCardViewStruct.SaveButton, SaveButton);
            sTabCard.StackPanels[DocTypeCardViewStruct.FourthLineStackPanel].Children.Add(SaveButton);
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
                if (((DocTypeCard)sTabCard.Card).isEditingNow)
                {
                    SystemSingleton.Configuration.tabControl.Items.Remove(sTabCard.TabItem);
                    SystemSingleton.CurrentSession.TabCards.Remove(((DocTypeCard)sTabCard.Card).ID.Value);
                }
                else
                {
                    MessageBoxResult dialogResult = MessageBoxResult.No;
                    int commandInt = 0;
                    PrepareInsertCommand(sTabCard, ref commandInt);
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
                    SystemSingleton.CurrentSession.TabCards.Remove(((DocTypeCard)sTabCard.Card).ID.Value);
                }
            };
            sTabCard.Buttons.Add(DocTypeCardViewStruct.CloseButton, CloseButton);
            sTabCard.StackPanels[DocTypeCardViewStruct.FourthLineStackPanel].Children.Add(CloseButton);

            #endregion
        }

        private static bool CheckNULL(STabCard sTabCard)
        {
            if (sTabCard.TextBoxes[DocTypeCardViewStruct.NameTextBox].Text == "" ||
                sTabCard.TextBoxes[DocTypeCardViewStruct.CaptionTextBox].Text == "" ||
                sTabCard.TextBoxes[DocTypeCardViewStruct.SecondLineTextBox].Text == "" ||
                sTabCard.TextBoxes[DocTypeCardViewStruct.ThirdLineTextBox].Text == "")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static string PrepareInsertCommand(STabCard sTabCard, ref int num)
        {
            num = 0;
            if (CheckNULL(sTabCard))
            {
                EnvironmentHelper.SendDialogBox(
                    (string)SystemSingleton.Configuration.mainWindow.FindResource(
                        "m_NullFieldsDoc"), "Fileds Info");
                return "";
            }
            string commandtext = "insert into DocTypes (ID, ";
            string commandvalues = "values ('" + ((DocTypeCard)sTabCard.Card).ID.Value.ToString() + "', ";
            string futureNewName = ((DocTypeCard)sTabCard.Card).Name;
            string futureNewCaption = ((DocTypeCard)sTabCard.Card).Caption;
            string futureNewTags = ((DocTypeCard)sTabCard.Card).TagWords;
            Guid futureNewRole = ((DocTypeCard)sTabCard.Card).RoleTypeID;
            if (sTabCard.TextBoxes[DocTypeCardViewStruct.NameTextBox].Text != futureNewName)
            {
                futureNewName = sTabCard.TextBoxes[DocTypeCardViewStruct.NameTextBox].Text;
                commandtext += "Name, ";
                commandvalues += "'" + futureNewName + "', ";
                num++;
            }
            if (sTabCard.TextBoxes[DocTypeCardViewStruct.CaptionTextBox].Text != futureNewCaption)
            {
                futureNewCaption = sTabCard.TextBoxes[DocTypeCardViewStruct.CaptionTextBox].Text;
                commandtext += "Caption, ";
                commandvalues += "'" + futureNewCaption + "', ";
                num++;
            }
            if (sTabCard.TextBoxes[DocTypeCardViewStruct.SecondLineTextBox].Text != futureNewTags)
            {
                futureNewTags = sTabCard.TextBoxes[DocTypeCardViewStruct.SecondLineTextBox].Text;
                commandtext += "TagWords, ";
                commandvalues += "'" + futureNewTags + "', ";
                num++;
            }
            if (((DocTypeCard)sTabCard.Card).NewRoleCard != futureNewRole)
            {
                futureNewRole = ((DocTypeCard)sTabCard.Card).NewRoleCard;
                commandtext += "RoleTypeID, ";
                commandvalues += "'" + futureNewRole + "', ";
                num++;
            }
            commandtext += "isEditingNow) ";
            commandvalues += "1);";
            return commandtext + commandvalues;
        }
        private static bool CheckDelete(Guid ID)
        {
            try
            {
                using (var con = new SqlConnection(SystemSingleton.Configuration.ConnectionString))
                {
                    SystemSingleton.Configuration.SqlConnections.Add(con);
                    using (var command = new SqlCommand(SqlCommands.CheckDeleteDocType, con))
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
        private static string PrepareSaveCommandWithoutWhere(STabCard sTabCard, ref int num)
        {
            num = 0;
            if (CheckNULL(sTabCard))
            {
                EnvironmentHelper.SendDialogBox(
                    (string)SystemSingleton.Configuration.mainWindow.FindResource(
                        "m_NullFieldsDoc"), "Fileds Info");
                return "";
            }

            string commandtext = "update DocTypes set ";
            string futureNewName = ((DocTypeCard)sTabCard.Card).Name;
            string futureNewCaption = ((DocTypeCard)sTabCard.Card).Caption;
            string futureNewTags = ((DocTypeCard)sTabCard.Card).TagWords;
            Guid futureNewRole = ((DocTypeCard)sTabCard.Card).RoleTypeID;
            if (sTabCard.TextBoxes[DocTypeCardViewStruct.NameTextBox].Text != futureNewName)
            {
                futureNewName = sTabCard.TextBoxes[DocTypeCardViewStruct.NameTextBox].Text;
                commandtext += "Name='" + futureNewName + "', ";
                num++;
            }

            if (sTabCard.TextBoxes[DocTypeCardViewStruct.CaptionTextBox].Text != futureNewCaption)
            {
                futureNewCaption = sTabCard.TextBoxes[DocTypeCardViewStruct.CaptionTextBox].Text;
                commandtext += "Caption='" + futureNewCaption + "', ";
                num++;
            }

            if (sTabCard.TextBoxes[DocTypeCardViewStruct.SecondLineTextBox].Text != futureNewTags)
            {
                futureNewTags = sTabCard.TextBoxes[DocTypeCardViewStruct.SecondLineTextBox].Text;
                commandtext += "TagWords='" + futureNewTags + "', ";
                num++;
            }

            if (((DocTypeCard)sTabCard.Card).NewRoleCard != futureNewRole)
            {
                futureNewRole = ((DocTypeCard)sTabCard.Card).NewRoleCard;
                commandtext += "RoleTypeID='" + futureNewRole + "', ";
                num++;
            }

            if (commandtext[commandtext.Length - 2] == ',') commandtext = commandtext.Remove(commandtext.Length - 2, 1);
            return commandtext;
        }
    }
}
