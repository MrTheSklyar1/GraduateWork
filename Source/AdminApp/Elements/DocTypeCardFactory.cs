using System;
using System.Collections.Generic;
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
    public static class DocTypeCardFactory
    {
        public static STabCard CreateTab(Guid RoleID)
        {
            var result = new STabCard();
            result.Card = new DocTypeCard(RoleID);
            result.CardType = StaticTypes.DocType;
            result.isNew = false;
            if (((DocTypeCard) result.Card).HasValue)
            {
                result.TabItem = new TabItem
                {
                    Header = ((DocTypeCard) result.Card).Caption,
                    FontSize = 15,
                    Height = 40
                };
                try
                {
                    FillMainStackPanelToTab(result);
                    FillFirstLine(result);
                    FillSecondLine(result);
                    FillThirdLine(result);
                    //FillFourthLine(result);
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
            if (((DocTypeCard) result.Card).HasValue)
            {
                result.TabItem = new TabItem
                {
                    Header = (string) SystemSingleton.Configuration.mainWindow.FindResource("m_NewCard"),
                    FontSize = 15,
                    Height = 40
                };
                try
                {
                    FillMainStackPanelToTab(result);
                    FillFirstLine(result);
                    FillSecondLine(result);
                    FillThirdLine(result);
                    //SetButtonsNew(result);
                }
                catch (Exception ex)
                {
                    EnvironmentHelper.Error(((DocTypeCard) result.Card).ID.Value);
                    return null;
                }

                return result;
            }
            else
            {
                EnvironmentHelper.Error(((DocTypeCard) result.Card).ID.Value);
                return null;
            }
        }

        private static void NameTextBox_LostKeyboardFocus(STabCard sTabCard)
        {
            if (((DocTypeCard) sTabCard.Card).isEditingNow) return;
            string temp = sTabCard.TextBoxes[DocTypeCardViewStruct.NameTextBox].Text;
            for (int i = 0; i < temp.Length; i++)
            {
                if (!Char.IsLetter(temp[i]))
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
            //FillFourthLine(sTabCard);
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
                Text = (string) SystemSingleton.Configuration.mainWindow.FindResource("m_NameRole"),
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
                Text = ((DocTypeCard) sTabCard.Card).Name,
                VerticalContentAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Left,
                MinWidth = 150,
                MaxWidth = 300,
                FontSize = 14,
                Height = 40,
                MaxLength = 50,
                IsReadOnly = ((DocTypeCard) sTabCard.Card).isEditingNow
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
                Text = (string) SystemSingleton.Configuration.mainWindow.FindResource("m_CaptionStatic"),
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
                Text = ((DocTypeCard) sTabCard.Card).Caption,
                VerticalContentAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Left,
                MinWidth = 150,
                MaxWidth = 300,
                FontSize = 14,
                Height = 40,
                MaxLength = 50,
                IsReadOnly = ((DocTypeCard) sTabCard.Card).isEditingNow
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
                Text = ((DocTypeCard)sTabCard.Card).RoleCard!=null ? ((DocTypeCard)sTabCard.Card).RoleCard.Name : "",
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
                    if(((DocTypeCard)sTabCard.Card).RoleCard.HasValue)
                    ((DocTypeCard) sTabCard.Card).RoleTypeID = ((DocTypeCard) sTabCard.Card).RoleCard.ID.Value;
                    sTabCard.TextBoxes[DocTypeCardViewStruct.ThirdLineTextBox].Text =
                        ((DocTypeCard) sTabCard.Card).RoleCard.Name;
                    SystemSingleton.CurrentSession.ChosenIDForStaticRole = Guid.Empty;
                }
            };
            sTabCard.Buttons.Add(DocTypeCardViewStruct.ThirdLineButton, ThirdLineButton);
            sTabCard.DockPanels[DocTypeCardViewStruct.ThirdLineDockPanel].Children.Add(ThirdLineButton);
            #endregion
        }
    }
}
