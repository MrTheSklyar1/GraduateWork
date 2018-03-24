using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerApp.SystemClasses;
using Telegram.Bot.Types.ReplyMarkups;

namespace ServerApp.Elements
{
    public static class Menu
    {
        public static ReplyKeyboardMarkup LogInKeyBoard()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new[] {
                    new[]
                    {
                        new Telegram.Bot.Types.KeyboardButton((string)SystemSingleton.Configuration.Window.FindResource("m_BotM_LogIn")),
                    },
                },
                OneTimeKeyboard = false,
                ResizeKeyboard = true
            };
        }
    }
}
