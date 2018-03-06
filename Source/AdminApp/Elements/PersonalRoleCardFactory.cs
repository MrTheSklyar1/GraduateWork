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
                    //FillSecondLine(result);
                    //FillThirdLine(result);
                    //FillFourthLine(result);
                    //FillFifthLine(result);
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

    }
}
