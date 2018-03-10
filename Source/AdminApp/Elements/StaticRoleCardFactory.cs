using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using AdminApp.SupportClasses;
using AdminApp.SystemClasses;
using Microsoft.Win32;

namespace AdminApp.Elements
{
    public class StaticRoleCardFactory
    {
        public static STabCard CreateTab(Guid RoleID)
        {
            var result = new STabCard();
            result.Card = new StaticRoleCard(RoleID);
            result.CardType = StaticTypes.StaticRole;
            result.isNew = false;
            if (((StaticRoleCard)result.Card).HasValue)
            {
                result.TabItem = new TabItem
                {
                    Header = ((StaticRoleCard)result.Card).Caption,
                    FontSize = 15,
                    Height = 40
                };
                try
                {
                    FillMainStackPanelToTab(result);
                    FillFirstLine(result);
                    FillSecondLine(result);
                    FillThirdLine(result);
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
            result.Card = new StaticRoleCard();
            result.CardType = StaticTypes.StaticRole;
            result.isNew = true;
            if (((StaticRoleCard)result.Card).HasValue)
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
                    SetButtonsNew(result);
                }
                catch (Exception ex)
                {
                    EnvironmentHelper.Error(((StaticRoleCard)result.Card).ID.Value);
                    return null;
                }
                return result;
            }
            else
            {
                EnvironmentHelper.Error(((StaticRoleCard)result.Card).ID.Value);
                return null;
            }
        }

        private static void SetButtonsNew(STabCard sTabCard)
        {
            #region Основная панель

            var ThirdLineDockPanel = new DockPanel
            {
                Margin = new Thickness(5, 0, 5, 10)
            };
            sTabCard.DockPanels.Add(StaticRoleCardViewStruct.ThirdLineDockPanel, ThirdLineDockPanel);
            sTabCard.StackPanels[StaticRoleCardViewStruct.MainStackPanel].Children.Add(ThirdLineDockPanel);

            #endregion

            #region Контрол кнопок

            //Border
            var ThirdLineBorder = new Border
            {
                CornerRadius = new CornerRadius(6),
                BorderBrush = new SolidColorBrush(Colors.LightGray),
                BorderThickness = new Thickness(2),
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(5, 0, 5, 0)
            };
            sTabCard.Borders.Add(StaticRoleCardViewStruct.ThirdLineBorder, ThirdLineBorder);
            sTabCard.DockPanels[StaticRoleCardViewStruct.ThirdLineDockPanel].Children.Add(ThirdLineBorder);
            //Вспомогательная панель
            var ThirdLineStackPanel = new StackPanel();
            sTabCard.StackPanels.Add(StaticRoleCardViewStruct.ThirdLineStackPanel, ThirdLineStackPanel);
            sTabCard.Borders[StaticRoleCardViewStruct.ThirdLineBorder].Child = ThirdLineStackPanel;
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
                    if (commandInt > 1 || ((StaticRoleCard)sTabCard.Card).rolesChanged)
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
                                foreach (var item in ((StaticRoleCard)sTabCard.Card).NewPersonalRoles)
                                {
                                    command.CommandText = SqlCommands.AddRoleRoleUsers;
                                    command.Parameters.Add("@RoleID", SqlDbType.UniqueIdentifier);
                                    command.Parameters["@RoleID"].Value = ((StaticRoleCard)sTabCard.Card).ID.Value;
                                    command.Parameters.Add("@PersonID", SqlDbType.UniqueIdentifier);
                                    command.Parameters["@PersonID"].Value = item.Key;
                                    EnvironmentHelper.SendLogSQL(command.CommandText);
                                    command.ExecuteNonQuery();
                                    command.Parameters.Clear();
                                }
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
                        sTabCard.Card = new StaticRoleCard(((StaticRoleCard)sTabCard.Card).ID.Value);
                        sTabCard.isNew = false;
                        ((StaticRoleCard)sTabCard.Card).isEditingNow = false;
                        EnvironmentHelper.UpdateView();
                        RebuildView(sTabCard);
                        sTabCard.TabItem.Header = ((StaticRoleCard)sTabCard.Card).Caption;
                    }

                }
                catch (Exception ex)
                {
                    EnvironmentHelper.SendErrorDialogBox(ex.Message, "SQL Error", ex.StackTrace);
                }

            };
            sTabCard.Buttons.Add(StaticRoleCardViewStruct.SaveButton, SaveButton);
            sTabCard.StackPanels[StaticRoleCardViewStruct.ThirdLineStackPanel].Children.Add(SaveButton);
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
                if (((StaticRoleCard)sTabCard.Card).isEditingNow)
                {
                    SystemSingleton.Configuration.tabControl.Items.Remove(sTabCard.TabItem);
                    SystemSingleton.CurrentSession.TabCards.Remove(((StaticRoleCard)sTabCard.Card).ID.Value);
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
                    SystemSingleton.CurrentSession.TabCards.Remove(((StaticRoleCard)sTabCard.Card).ID.Value);
                }
            };
            sTabCard.Buttons.Add(StaticRoleCardViewStruct.CloseButton, CloseButton);
            sTabCard.StackPanels[StaticRoleCardViewStruct.ThirdLineStackPanel].Children.Add(CloseButton);

            #endregion
        }

        private static void NameTextBox_LostKeyboardFocus(STabCard sTabCard)
        {
            if (((StaticRoleCard)sTabCard.Card).isEditingNow) return;
            string temp = sTabCard.TextBoxes[StaticRoleCardViewStruct.NameTextBox].Text;
            for (int i = 0; i < temp.Length; i++)
            {
                if (!Char.IsLetter(temp[i]))
                {
                    temp = temp.Remove(i, 1);
                    i--;
                }
            }
            sTabCard.TextBoxes[StaticRoleCardViewStruct.NameTextBox].Text = temp;
        }

        private static void RebuildView(STabCard sTabCard)
        {
            sTabCard.StackPanels[StaticRoleCardViewStruct.ThirdLineStackPanel].Children.Remove(sTabCard.Buttons[StaticRoleCardViewStruct.CloseButton]);
            sTabCard.Buttons.Remove(StaticRoleCardViewStruct.CloseButton);
            sTabCard.StackPanels[StaticRoleCardViewStruct.ThirdLineStackPanel].Children.Remove(sTabCard.Buttons[StaticRoleCardViewStruct.SaveButton]);
            sTabCard.Buttons.Remove(StaticRoleCardViewStruct.SaveButton);
            sTabCard.Borders[StaticRoleCardViewStruct.ThirdLineBorder].Child = null;
            sTabCard.StackPanels.Remove(StaticRoleCardViewStruct.ThirdLineStackPanel);
            sTabCard.DockPanels[StaticRoleCardViewStruct.ThirdLineDockPanel].Children.Remove(sTabCard.Borders[StaticRoleCardViewStruct.ThirdLineBorder]);
            sTabCard.Borders.Remove(StaticRoleCardViewStruct.ThirdLineBorder);
            sTabCard.StackPanels[StaticRoleCardViewStruct.MainStackPanel].Children.Remove(sTabCard.DockPanels[StaticRoleCardViewStruct.ThirdLineDockPanel]);
            sTabCard.DockPanels.Remove(StaticRoleCardViewStruct.ThirdLineDockPanel);
            FillThirdLine(sTabCard);
        }

        private static void FillMainStackPanelToTab(STabCard sTabCard)
        {
            var MainStackPanel = new StackPanel
            {
                Margin = new Thickness(10)
            };
            sTabCard.StackPanels.Add(StaticRoleCardViewStruct.MainStackPanel, MainStackPanel);
            sTabCard.TabItem.Content = MainStackPanel;
        }

        private static void FillFirstLine(STabCard sTabCard)
        {

            #region Основная панель

            var FirstLineDockPanel = new DockPanel
            {
                Margin = new Thickness(5, 0, 5, 10)
            };
            sTabCard.DockPanels.Add(StaticRoleCardViewStruct.FirstLineDockPanel, FirstLineDockPanel);
            sTabCard.StackPanels[StaticRoleCardViewStruct.MainStackPanel].Children.Add(FirstLineDockPanel);

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
            sTabCard.Borders.Add(StaticRoleCardViewStruct.NameBorder, NameBorder);
            sTabCard.DockPanels[StaticRoleCardViewStruct.FirstLineDockPanel].Children.Add(NameBorder);
            //Вспомогательная панель
            var NameDockPanel = new DockPanel();
            sTabCard.DockPanels.Add(StaticRoleCardViewStruct.NameDockPanel, NameDockPanel);
            sTabCard.Borders[StaticRoleCardViewStruct.NameBorder].Child = NameDockPanel;
            //Текстовый блок
            var NameTextBlock = new TextBlock
            {
                Text = (string)SystemSingleton.Configuration.mainWindow.FindResource("m_NameRole"),
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Left,
                FontSize = 14,
                Margin = new Thickness(5, 0, 5, 0)
            };
            sTabCard.TextBlocks.Add(StaticRoleCardViewStruct.NameTextBlock, NameTextBlock);
            sTabCard.DockPanels[StaticRoleCardViewStruct.NameDockPanel].Children.Add(NameTextBlock);
            //Контрол блока 
            var NameTextBox = new TextBox
            {
                Text = ((StaticRoleCard)sTabCard.Card).Name,
                VerticalContentAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Left,
                MinWidth = 150,
                MaxWidth = 300,
                FontSize = 14,
                Height = 40,
                MaxLength = 50,
                IsReadOnly = ((StaticRoleCard)sTabCard.Card).isEditingNow
            };
            NameTextBox.LostKeyboardFocus += (sender, args) =>
            {
                NameTextBox_LostKeyboardFocus(sTabCard);
            };
            sTabCard.TextBoxes.Add(StaticRoleCardViewStruct.NameTextBox, NameTextBox);
            sTabCard.DockPanels[StaticRoleCardViewStruct.NameDockPanel].Children.Add(NameTextBox);

            #endregion

            #region Контрол надписи

            //Border
            var CaptionBorder = new Border
            {
                CornerRadius = new CornerRadius(6),
                BorderBrush = new SolidColorBrush(Colors.LightGray),
                BorderThickness = new Thickness(2),
                Margin = new Thickness(0, 0, 5, 0),
                HorizontalAlignment = HorizontalAlignment.Right
            };
            sTabCard.Borders.Add(StaticRoleCardViewStruct.CaptionBorder, CaptionBorder);
            sTabCard.DockPanels[StaticRoleCardViewStruct.FirstLineDockPanel].Children.Add(CaptionBorder);
            //Вспомогательная панель 
            var CaptionDockPanel = new DockPanel();
            sTabCard.DockPanels.Add(StaticRoleCardViewStruct.CaptionDockPanel, CaptionDockPanel);
            sTabCard.Borders[StaticRoleCardViewStruct.CaptionBorder].Child = CaptionDockPanel;
            //Текстовый блок
            var CaptionTextBlock = new TextBlock
            {
                Text = (string)SystemSingleton.Configuration.mainWindow.FindResource("m_CaptionStatic"),
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Left,
                FontSize = 14,
                Margin = new Thickness(5, 0, 5, 0)
            };
            sTabCard.TextBlocks.Add(StaticRoleCardViewStruct.CaptionTextBlock, CaptionTextBlock);
            sTabCard.DockPanels[StaticRoleCardViewStruct.CaptionDockPanel].Children.Add(CaptionTextBlock);
            //Контрол блока 
            var CaptionTextBox = new TextBox
            {
                Text = ((StaticRoleCard)sTabCard.Card).Caption,
                VerticalContentAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Left,
                MinWidth = 150,
                MaxWidth = 300,
                FontSize = 14,
                Height = 40,
                MaxLength = 50,
                IsReadOnly = ((StaticRoleCard)sTabCard.Card).isEditingNow
            };
            CaptionTextBox.LostKeyboardFocus += (sender, args) =>
            {
                sTabCard.TextBoxes[StaticRoleCardViewStruct.CaptionTextBox].Text =
                    sTabCard.TextBoxes[StaticRoleCardViewStruct.CaptionTextBox].Text.Trim();
            };
            sTabCard.TextBoxes.Add(StaticRoleCardViewStruct.CaptionTextBox, CaptionTextBox);
            sTabCard.DockPanels[StaticRoleCardViewStruct.CaptionDockPanel].Children.Add(CaptionTextBox);

            #endregion
        }

        private static void FillSecondLine(STabCard sTabCard)
        {
            #region Основная панель

            var SecondLineDockPanel = new DockPanel
            {
                Margin = new Thickness(5, 0, 5, 10)
            };
            sTabCard.DockPanels.Add(StaticRoleCardViewStruct.SecondLineDockPanel, SecondLineDockPanel);
            sTabCard.StackPanels[StaticRoleCardViewStruct.MainStackPanel].Children.Add(SecondLineDockPanel);

            #endregion

            #region Контрол Файлы

            //Border 
            var RolesBorder = new Border
            {
                CornerRadius = new CornerRadius(6),
                BorderBrush = new SolidColorBrush(Colors.LightGray),
                BorderThickness = new Thickness(2),
                HorizontalAlignment = HorizontalAlignment.Left
            };
            sTabCard.Borders.Add(StaticRoleCardViewStruct.RolesBorder, RolesBorder);
            sTabCard.DockPanels[StaticRoleCardViewStruct.SecondLineDockPanel].Children.Add(RolesBorder);
            //Вспомогательная панель 
            var RolesStackPanel = new StackPanel();
            sTabCard.StackPanels.Add(StaticRoleCardViewStruct.RolesStackPanel, RolesStackPanel);
            sTabCard.Borders[StaticRoleCardViewStruct.RolesBorder].Child = RolesStackPanel;
            //Текстовый блок
            var RolesTextBlock = new TextBlock
            {
                Text = (string)SystemSingleton.Configuration.mainWindow.FindResource("c_RolesInRole"),
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Left,
                HorizontalAlignment = HorizontalAlignment.Left,
                FontSize = 14,
                Margin = new Thickness(5, 0, 0, 0)
            };
            sTabCard.TextBlocks.Add(StaticRoleCardViewStruct.RolesTextBlock, RolesTextBlock);
            sTabCard.StackPanels[StaticRoleCardViewStruct.RolesStackPanel].Children.Add(RolesTextBlock);
            //Лист view 
            var RolesListView = new ListView()
            {
                Height = 200,
                MinWidth = 500,
                MaxWidth = 1100,
                Margin = new Thickness(5)
            };
            sTabCard.ListViews.Add(StaticRoleCardViewStruct.RolesListView, RolesListView);
            sTabCard.StackPanels[StaticRoleCardViewStruct.RolesStackPanel].Children.Add(RolesListView);
            //Роли
            if (((StaticRoleCard)sTabCard.Card).PersonalRoleCards.Count > 0)
            {
                foreach (var item in ((StaticRoleCard)sTabCard.Card).PersonalRoleCards)
                {
                    AddRoleToList(item.Value, sTabCard, false);
                }
            }

            if (!((StaticRoleCard)sTabCard.Card).isEditingNow)
            {
                //Кнопка добавить
                var RolesButton = new Button
                {
                    Content = (string)SystemSingleton.Configuration.mainWindow.FindResource("c_AddRole"),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Height = 25,
                    FontSize = 14,
                    Margin = new Thickness(5)
                };
                RolesButton.Click += (senderadd, argsadd) =>
                {
                    var Window = new ChosePersonalRoleView();
                    SystemSingleton.CurrentSession.ViewChoose = Window;
                    SystemSingleton.CurrentSession.ViewChoose.ShowDialog();
                    SystemSingleton.CurrentSession.ViewChoose = null;
                    if (SystemSingleton.CurrentSession.ChosenIDForStaticRole == Guid.Empty)
                    {
                        return;
                    }
                    else if (((StaticRoleCard)sTabCard.Card).PersonalControls.ContainsKey(SystemSingleton.CurrentSession.ChosenIDForStaticRole))
                    {
                        EnvironmentHelper.SendDialogBox(
                            (string)SystemSingleton.Configuration.mainWindow.FindResource(
                                "m_AlreadyIn") + "\n\n" + SystemSingleton.CurrentSession.ChosenIDForStaticRole,
                            "Card Info");
                    }
                    else
                    {
                        AddRoleToList(new PersonalRoleCard(SystemSingleton.CurrentSession.ChosenIDForStaticRole), sTabCard, true);
                        SystemSingleton.CurrentSession.ChosenIDForStaticRole = Guid.Empty;
                        ((StaticRoleCard)sTabCard.Card).rolesChanged = true;
                    }
                };
                sTabCard.Buttons.Add(StaticRoleCardViewStruct.RolesButton, RolesButton);
                sTabCard.StackPanels[StaticRoleCardViewStruct.RolesStackPanel].Children.Add(RolesButton);
            }

            #endregion
        }

        private static void AddRoleToList(PersonalRoleCard item, STabCard sTabCard, bool inwork)
        {
            var temp = new PersonalRoleControl()
            {
                ID = item.ID.Value,
                DockPanel = new DockPanel(),
                Button = new Button
                {
                    FontSize = 14,
                    Margin = new Thickness(0, 0, 5, 0),
                    Content = (string)SystemSingleton.Configuration.mainWindow.FindResource("c_Delete"),
                    IsEnabled = !((StaticRoleCard)sTabCard.Card).isEditingNow
                },
                TextBlock = new TextBlock
                {
                    FontSize = 14,
                    VerticalAlignment = VerticalAlignment.Center,
                    Text = item.FullName
                }
            };
            temp.Button.Click += (sender, args) =>
            {
                MessageBoxResult dialogResult = MessageBox.Show((string)SystemSingleton.Configuration.mainWindow.FindResource("m_MakeSureDeletingRoleFromStatic"),
                    (string)SystemSingleton.Configuration.mainWindow.FindResource("m_AttentionHeader"),
                    MessageBoxButton.YesNo);
                if (dialogResult == MessageBoxResult.Yes)
                {
                    if (inwork)
                    {
                        sTabCard.ListViews[StaticRoleCardViewStruct.RolesListView].Items.Remove(((StaticRoleCard)sTabCard.Card).NewPersonalControls[item.ID.Value].DockPanel);
                    }
                    else
                    {
                        sTabCard.ListViews[StaticRoleCardViewStruct.RolesListView].Items.Remove(((StaticRoleCard)sTabCard.Card).PersonalControls[item.ID.Value].DockPanel);
                    }
                    PersonalRoleDelete(item.ID.Value, sTabCard);
                }
                ((StaticRoleCard)sTabCard.Card).rolesChanged = true;
            };
            temp.TextBlock.MouseLeftButtonDown += (sender, args) =>
            {
                if (item.ID.Value ==
            SystemSingleton.CurrentSession.ID)
                {
                    EnvironmentHelper.SendDialogBox(
                        (string)SystemSingleton.Configuration.mainWindow.FindResource("m_CantEditMySelf"),
                        "Attention"
                    );
                    return;
                }
                var tempR = PersonalRoleCardFactory.CreateTab(item.ID.Value);
                if (tempR != null)
                {
                    if (SystemSingleton.CurrentSession.TabCards.ContainsKey(((PersonalRoleCard)tempR.Card).ID.Value))
                    {
                        EnvironmentHelper.SendDialogBox(
                            (string)SystemSingleton.Configuration.mainWindow.FindResource("m_AlreadyOpened"),
                            "Attention"
                        );
                    }
                    else
                    {
                        if (((PersonalRoleCard)tempR.Card).isEditingNow)
                        {
                            EnvironmentHelper.SendDialogBox(
                                (string)SystemSingleton.Configuration.mainWindow.FindResource("m_AlreadyEditing"),
                                "Attention"
                            );
                        }
                        else
                        {
                            try
                            {
                                using (var con = new SqlConnection(SystemSingleton.Configuration.ConnectionString))
                                {
                                    SystemSingleton.Configuration.SqlConnections.Add(con);
                                    using (var command = new SqlCommand(SqlCommands.SetEditingToPersonalRole, con))
                                    {
                                        command.Parameters.Add("@ID", SqlDbType.UniqueIdentifier);
                                        command.Parameters["@ID"].Value = ((PersonalRoleCard)tempR.Card).ID.Value;
                                        EnvironmentHelper.SendLogSQL(command.CommandText);
                                        con.Open();
                                        int colms = command.ExecuteNonQuery();
                                        con.Close();
                                        if (colms == 0)
                                        {
                                            EnvironmentHelper.SendDialogBox(
                                                (string)SystemSingleton.Configuration.mainWindow.FindResource(
                                                    "m_CantSetEditing") + "\n\n" + ((PersonalRoleCard)tempR.Card).ID.Value.ToString(),
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
                        SystemSingleton.CurrentSession.TabCards.Add(((PersonalRoleCard)tempR.Card).ID.Value, tempR);
                        SystemSingleton.Configuration.tabControl.Items.Add(tempR.TabItem);
                    }
                }
            };
            if (inwork)
            {
                ((StaticRoleCard)sTabCard.Card).NewPersonalRoles.Add(item.ID.Value, item);
                ((StaticRoleCard)sTabCard.Card).NewPersonalControls.Add(item.ID.Value, temp);
            }
            else
            {
                ((StaticRoleCard)sTabCard.Card).PersonalControls.Add(item.ID.Value, temp);
            }
            temp.DockPanel.Children.Add(temp.Button);
            temp.DockPanel.Children.Add(temp.TextBlock);
            sTabCard.ListViews[StaticRoleCardViewStruct.RolesListView].Items.Add(temp.DockPanel);
        }

        private static void PersonalRoleDelete(Guid idValue, STabCard sTabCard)
        {
            ((StaticRoleCard)sTabCard.Card).DeletedPersons.Add(idValue);
            if (((StaticRoleCard) sTabCard.Card).NewPersonalRoles.ContainsKey(idValue))
            {
                ((StaticRoleCard) sTabCard.Card).NewPersonalRoles.Remove(idValue);
                ((StaticRoleCard)sTabCard.Card).NewPersonalControls.Remove(idValue);
            }
            else if(((StaticRoleCard)sTabCard.Card).PersonalRoleCards.ContainsKey(idValue))
            {
                ((StaticRoleCard)sTabCard.Card).PersonalControls.Remove(idValue);
            }
            
        }

        private static void FillThirdLine(STabCard sTabCard)
        {
            #region Основная панель

            var ThirdLineDockPanel = new DockPanel
            {
                Margin = new Thickness(5, 0, 5, 10)
            };
            sTabCard.DockPanels.Add(StaticRoleCardViewStruct.ThirdLineDockPanel, ThirdLineDockPanel);
            sTabCard.StackPanels[StaticRoleCardViewStruct.MainStackPanel].Children.Add(ThirdLineDockPanel);

            #endregion

            #region Контрол кнопок

            //Border
            var ThirdLineBorder = new Border
            {
                CornerRadius = new CornerRadius(6),
                BorderBrush = new SolidColorBrush(Colors.LightGray),
                BorderThickness = new Thickness(2),
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(5, 0, 5, 0)
            };
            sTabCard.Borders.Add(StaticRoleCardViewStruct.ThirdLineBorder, ThirdLineBorder);
            sTabCard.DockPanels[StaticRoleCardViewStruct.ThirdLineDockPanel].Children.Add(ThirdLineBorder);
            //Вспомогательная панель
            var ThirdLineStackPanel = new StackPanel();
            sTabCard.StackPanels.Add(StaticRoleCardViewStruct.ThirdLineStackPanel, ThirdLineStackPanel);
            sTabCard.Borders[StaticRoleCardViewStruct.ThirdLineBorder].Child = ThirdLineStackPanel;
            if (!((StaticRoleCard)sTabCard.Card).isEditingNow)
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
                        string commandtext = PrepareSaveCommandWithoutWhere(sTabCard);
                        if (commandtext == "")
                        {
                            return;
                        }
                        if (commandtext.Length > 25 || ((StaticRoleCard)sTabCard.Card).rolesChanged)
                        {
                            dialogResult = MessageBox.Show((string)SystemSingleton.Configuration.mainWindow.FindResource("m_MakeSureSavingCard"),
                                (string)SystemSingleton.Configuration.mainWindow.FindResource("m_AttentionHeader"),
                                MessageBoxButton.YesNo);
                        }
                        if (dialogResult == MessageBoxResult.Yes)
                        {
                            commandtext += "where ID='" + ((StaticRoleCard)sTabCard.Card).ID.Value.ToString() + "';";
                            using (var con = new SqlConnection(SystemSingleton.Configuration.ConnectionString))
                            {
                                SystemSingleton.Configuration.SqlConnections.Add(con);
                                con.Open();
                                SqlTransaction transaction = con.BeginTransaction();
                                SqlCommand command = con.CreateCommand();
                                command.Transaction = transaction;
                                try
                                {
                                    if (commandtext.Length > 75)
                                    {
                                        command.CommandText = commandtext;
                                        EnvironmentHelper.SendLogSQL(command.CommandText);
                                        command.ExecuteNonQuery();
                                    }
                                    foreach (var item in ((StaticRoleCard)sTabCard.Card).DeletedPersons)
                                    {
                                        if (((StaticRoleCard)sTabCard.Card).PersonalRoleCards.ContainsKey(item))
                                        {
                                            command.CommandText = SqlCommands.DeleteRoleRoleUsers;
                                            command.Parameters.Add("@RoleID", SqlDbType.UniqueIdentifier);
                                            command.Parameters["@RoleID"].Value = ((StaticRoleCard)sTabCard.Card).ID.Value;
                                            command.Parameters.Add("@PersonID", SqlDbType.UniqueIdentifier);
                                            command.Parameters["@PersonID"].Value = item;
                                            EnvironmentHelper.SendLogSQL(command.CommandText);
                                            command.ExecuteNonQuery();
                                            command.Parameters.Clear();
                                        }
                                    }
                                    foreach (var item in ((StaticRoleCard)sTabCard.Card).NewPersonalRoles)
                                    {
                                        command.CommandText = SqlCommands.AddRoleRoleUsers;
                                        command.Parameters.Add("@RoleID", SqlDbType.UniqueIdentifier);
                                        command.Parameters["@RoleID"].Value = ((StaticRoleCard)sTabCard.Card).ID.Value;
                                        command.Parameters.Add("@PersonID", SqlDbType.UniqueIdentifier);
                                        command.Parameters["@PersonID"].Value = item.Key;
                                        EnvironmentHelper.SendLogSQL(command.CommandText);
                                        command.ExecuteNonQuery();
                                        command.Parameters.Clear();
                                    }
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
                            sTabCard.Card = new StaticRoleCard(((StaticRoleCard)sTabCard.Card).ID.Value);
                            ((StaticRoleCard)sTabCard.Card).isEditingNow = false;
                            EnvironmentHelper.UpdateView();
                        }
                    }
                    catch (Exception ex)
                    {
                        EnvironmentHelper.SendErrorDialogBox(ex.Message, "SQL Error", ex.StackTrace);
                    }

                };
                sTabCard.Buttons.Add(StaticRoleCardViewStruct.SaveButton, SaveButton);
                sTabCard.StackPanels[StaticRoleCardViewStruct.ThirdLineStackPanel].Children.Add(SaveButton);
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
                    if (CheckDelete(((StaticRoleCard)sTabCard.Card).ID.Value))
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
                                    using (var command = new SqlCommand(SqlCommands.DeleteStaticRole, con))
                                    {
                                        command.Parameters.Add("@ID", SqlDbType.UniqueIdentifier);
                                        command.Parameters["@ID"].Value = ((StaticRoleCard)sTabCard.Card).ID.Value;
                                        EnvironmentHelper.SendLogSQL(command.CommandText);
                                        con.Open();
                                        int colms = command.ExecuteNonQuery();
                                        con.Close();
                                        if (colms == 0)
                                        {
                                            EnvironmentHelper.SendDialogBox(
                                                (string)SystemSingleton.Configuration.mainWindow.FindResource(
                                                    "m_CantDeleteCard") + "\n\n" + ((StaticRoleCard)sTabCard.Card).ID.Value.ToString(),
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
                            SystemSingleton.CurrentSession.TabCards.Remove(((StaticRoleCard)sTabCard.Card).ID.Value);
                            EnvironmentHelper.UpdateView();
                        }
                    }
                    else
                    {
                        EnvironmentHelper.SendDialogBox(
                            (string)SystemSingleton.Configuration.mainWindow.FindResource(
                                "m_CantDeleteCardTasksCardReview") + "\n\n" + ((StaticRoleCard)sTabCard.Card).ID.Value.ToString(),
                            "Card Error"
                        );
                    }
                };
                sTabCard.Buttons.Add(StaticRoleCardViewStruct.DeleteButton, DeleteButton);
                sTabCard.StackPanels[StaticRoleCardViewStruct.ThirdLineStackPanel].Children.Add(DeleteButton);
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
                if (((StaticRoleCard)sTabCard.Card).isEditingNow)
                {
                    SystemSingleton.Configuration.tabControl.Items.Remove(sTabCard.TabItem);
                    SystemSingleton.CurrentSession.TabCards.Remove(((StaticRoleCard)sTabCard.Card).ID.Value);
                }
                else
                {
                    MessageBoxResult dialogResult = MessageBoxResult.No;
                    if (PrepareSaveCommandWithoutWhere(sTabCard).Length > 25)
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
                    SystemSingleton.CurrentSession.TabCards.Remove(((StaticRoleCard)sTabCard.Card).ID.Value);
                    try
                    {
                        using (var con = new SqlConnection(SystemSingleton.Configuration.ConnectionString))
                        {
                            SystemSingleton.Configuration.SqlConnections.Add(con);
                            using (var command = new SqlCommand(SqlCommands.SetStopEditingToStaticRole, con))
                            {
                                command.Parameters.Add("@ID", SqlDbType.UniqueIdentifier);
                                command.Parameters["@ID"].Value = ((StaticRoleCard)sTabCard.Card).ID.Value;
                                EnvironmentHelper.SendLogSQL(command.CommandText);
                                con.Open();
                                int colms = command.ExecuteNonQuery();
                                con.Close();
                                if (colms == 0)
                                {
                                    EnvironmentHelper.SendDialogBox(
                                        (string)SystemSingleton.Configuration.mainWindow.FindResource(
                                            "m_CantSetEditing") + "\n\n" + ((StaticRoleCard)sTabCard.Card).ID.Value.ToString(),
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
            sTabCard.Buttons.Add(StaticRoleCardViewStruct.CloseButton, CloseButton);
            sTabCard.StackPanels[StaticRoleCardViewStruct.ThirdLineStackPanel].Children.Add(CloseButton);

            #endregion
        }

        private static bool CheckDelete(Guid ID)
        {
            try
            {
                using (var con = new SqlConnection(SystemSingleton.Configuration.ConnectionString))
                {
                    SystemSingleton.Configuration.SqlConnections.Add(con);
                    using (var command = new SqlCommand(SqlCommands.CheckDeleteStaticRole, con))
                    {
                        command.Parameters.Add("@ID", SqlDbType.UniqueIdentifier);
                        command.Parameters["@ID"].Value = ID;
                        EnvironmentHelper.SendLogSQL(command.CommandText);
                        con.Open();
                        int colms = Convert.ToInt32(command.ExecuteScalar());
                        con.Close();
                        if (colms == 0 && ID!=new Guid("9efcd5cd-bf54-47f3-95e3-2953cb235941"))
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

        private static string PrepareSaveCommandWithoutWhere(STabCard sTabCard)
        {
            if (CheckNULL(sTabCard))
            {
                EnvironmentHelper.SendDialogBox(
                    (string)SystemSingleton.Configuration.mainWindow.FindResource(
                        "m_NullFieldsRole"), "Fileds Info");
                return "";
            }

            string commandtext = "update StaticRoles set ";
            string futureNewName = ((StaticRoleCard)sTabCard.Card).Name;
            string futureNewCaption = ((StaticRoleCard)sTabCard.Card).Caption;
            if (sTabCard.TextBoxes[StaticRoleCardViewStruct.NameTextBox].Text != futureNewName)
            {
                futureNewName = sTabCard.TextBoxes[StaticRoleCardViewStruct.NameTextBox].Text;
                commandtext += "Name='" + futureNewName + "', ";
            }

            if (sTabCard.TextBoxes[StaticRoleCardViewStruct.CaptionTextBox].Text != futureNewCaption)
            {
                futureNewCaption = sTabCard.TextBoxes[StaticRoleCardViewStruct.CaptionTextBox].Text;
                commandtext += "Caption='" + futureNewCaption + "', ";
            }

            if (commandtext[commandtext.Length - 2] == ',') commandtext = commandtext.Remove(commandtext.Length - 2, 1);
            return commandtext;
        }
        private static string PrepareInsertCommand(STabCard sTabCard, ref int num)
        {
            num = 0;
            if (CheckNULL(sTabCard))
            {
                EnvironmentHelper.SendDialogBox(
                    (string)SystemSingleton.Configuration.mainWindow.FindResource(
                        "m_NullFieldsRole"), "Fileds Info");
                return "";
            }
            string commandtext = "insert into StaticRoles (ID, ";
            string commandvalues = "values ('" + ((StaticRoleCard)sTabCard.Card).ID.Value.ToString() + "', ";
            string futureNewName = ((StaticRoleCard)sTabCard.Card).Name;
            string futureNewCaption = ((StaticRoleCard)sTabCard.Card).Caption;
            if (sTabCard.TextBoxes[StaticRoleCardViewStruct.NameTextBox].Text != futureNewName)
            {
                futureNewName = sTabCard.TextBoxes[StaticRoleCardViewStruct.NameTextBox].Text;
                commandtext += "Name, ";
                commandvalues += "'" + futureNewName + "', ";
                num++;
            }
            if (sTabCard.TextBoxes[StaticRoleCardViewStruct.CaptionTextBox].Text != futureNewCaption)
            {
                futureNewCaption = sTabCard.TextBoxes[StaticRoleCardViewStruct.CaptionTextBox].Text;
                commandtext += "Caption, ";
                commandvalues += "'" + futureNewCaption + "', ";
                num++;
            }
            commandtext += "isEditingNow) ";
            commandvalues += "1);";
            string insertroles = "insert into Roles values ('" + ((StaticRoleCard)sTabCard.Card).ID.Value.ToString() + "', '" + sTabCard.TextBoxes[StaticRoleCardViewStruct.CaptionTextBox].Text + "');";
            return insertroles + commandtext + commandvalues;
        }
        private static bool CheckNULL(STabCard sTabCard)
        {
            if (sTabCard.TextBoxes[StaticRoleCardViewStruct.NameTextBox].Text == "" ||
                sTabCard.TextBoxes[StaticRoleCardViewStruct.CaptionTextBox].Text == "")
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
