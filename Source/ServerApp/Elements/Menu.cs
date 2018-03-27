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
                        new Telegram.Bot.Types.KeyboardButton((string)SystemSingleton.Configuration.Window.FindResource("m_BotB_LogIn")),
                    },
                },
                OneTimeKeyboard = false,
                ResizeKeyboard = true
            };
        }
        public static ReplyKeyboardMarkup GoBackFromPasswordKeyBoard()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new[] {
                    new[]
                    {
                        new Telegram.Bot.Types.KeyboardButton((string)SystemSingleton.Configuration.Window.FindResource("m_BotB_GoToStart")),
                    },
                },
                OneTimeKeyboard = false,
                ResizeKeyboard = true
            };
        }

        public static ReplyKeyboardRemove RemoveKeyBoard()
        {
            return new ReplyKeyboardRemove
            {
                RemoveKeyboard = true
            };
        }
        public static ReplyKeyboardMarkup MainMenuKeyBoard()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new[] {
                    new[]
                    {
                        new Telegram.Bot.Types.KeyboardButton((string)SystemSingleton.Configuration.Window.FindResource("m_BotB_ReqDoc")),
                    },
                    new[]
                    {
                        new Telegram.Bot.Types.KeyboardButton((string)SystemSingleton.Configuration.Window.FindResource("m_BotB_CurrentTasks")),
                    },
                    new[]
                    {
                        new Telegram.Bot.Types.KeyboardButton((string)SystemSingleton.Configuration.Window.FindResource("m_BotB_History")),
                    },
                    new[]
                    {
                        new Telegram.Bot.Types.KeyboardButton((string)SystemSingleton.Configuration.Window.FindResource("m_BotB_LogOff")),
                    },
                },
                OneTimeKeyboard = false,
                ResizeKeyboard = true
            };
        }
        public static ReplyKeyboardMarkup InputTypeKeyBoard()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new[] {
                    new[]
                    {
                        new Telegram.Bot.Types.KeyboardButton((string)SystemSingleton.Configuration.Window.FindResource("m_BotB_FromList")),
                    },
                    new[]
                    {
                        new Telegram.Bot.Types.KeyboardButton((string)SystemSingleton.Configuration.Window.FindResource("m_BotB_Manually")),
                    },
                    new[]
                    {
                        new Telegram.Bot.Types.KeyboardButton((string)SystemSingleton.Configuration.Window.FindResource("m_BotB_GoToMainMenu")),
                    },
                },
                OneTimeKeyboard = false,
                ResizeKeyboard = true
            };
        }

        public static ReplyKeyboardMarkup GoBackKeyBoard()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new[] {
                    new[]
                    {
                        new Telegram.Bot.Types.KeyboardButton((string)SystemSingleton.Configuration.Window.FindResource("m_BotB_GoBack")),
                    },
                },
                OneTimeKeyboard = false,
                ResizeKeyboard = true
            };
        }

        public static ReplyKeyboardMarkup HistoryKeyBoard(int page)
        {
            throw new NotImplementedException();
            return new ReplyKeyboardMarkup
            {
                Keyboard = new[] {
                    new[]
                    {
                        new Telegram.Bot.Types.KeyboardButton((string)SystemSingleton.Configuration.Window.FindResource("m_BotB_GoBack")),
                    },
                },
                OneTimeKeyboard = false,
                ResizeKeyboard = true
            };
        }

        public static ReplyKeyboardMarkup CurrentTasksKeyBoard(int page)
        {
            throw new NotImplementedException();
            return new ReplyKeyboardMarkup
            {
                Keyboard = new[] {
                    new[]
                    {
                        new Telegram.Bot.Types.KeyboardButton((string)SystemSingleton.Configuration.Window.FindResource("m_BotB_GoBack")),
                    },
                },
                OneTimeKeyboard = false,
                ResizeKeyboard = true
            };
        }

        public static ReplyKeyboardMarkup DocTypesKeyBoard(int page)
        {
            throw new NotImplementedException();
            return new ReplyKeyboardMarkup
            {
                Keyboard = new[] {
                    new[]
                    {
                        new Telegram.Bot.Types.KeyboardButton((string)SystemSingleton.Configuration.Window.FindResource("m_BotB_GoBack")),
                    },
                },
                OneTimeKeyboard = false,
                ResizeKeyboard = true
            };
        }

        public static ReplyKeyboardMarkup AllOrConcreteRoleKeyBoard()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new[] {
                    new[]
                    {
                        new Telegram.Bot.Types.KeyboardButton((string)SystemSingleton.Configuration.Window.FindResource("m_BotB_ToAll")),
                    },
                    new[]
                    {
                        new Telegram.Bot.Types.KeyboardButton((string)SystemSingleton.Configuration.Window.FindResource("m_BotB_ToConcrete")),
                    },
                    new[]
                    {
                        new Telegram.Bot.Types.KeyboardButton((string)SystemSingleton.Configuration.Window.FindResource("m_BotB_GoBack")),
                    },
                },
                OneTimeKeyboard = false,
                ResizeKeyboard = true
            };
        }
    }
}
