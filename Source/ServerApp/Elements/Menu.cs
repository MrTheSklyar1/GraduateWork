using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerApp.SupportClasses;
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

        public static ReplyKeyboardMarkup DocTypesKeyBoard(ref int page, ref int pages)
        {
            var Page = EnvironmentHelper.ThreeDocTypesByPage(ref page, ref pages);
            if (Page == null || Page.Count == 0)
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
            else
            if (Page.Count == 1)
            {
                return new ReplyKeyboardMarkup
                {
                    Keyboard = new[] {
                        new[]
                        {
                            new Telegram.Bot.Types.KeyboardButton("<--"),
                            new Telegram.Bot.Types.KeyboardButton((string)SystemSingleton.Configuration.Window.FindResource("m_BotB_GoBack")),
                            new Telegram.Bot.Types.KeyboardButton("-->"),
                        },
                        new[]
                        {
                            new Telegram.Bot.Types.KeyboardButton(Page[0]),
                        },
                    },
                    OneTimeKeyboard = false,
                    ResizeKeyboard = true
                };
            }
            else if (Page.Count == 2)
            {
                return new ReplyKeyboardMarkup
                {
                    Keyboard = new[] {
                        new[]
                        {
                            new Telegram.Bot.Types.KeyboardButton("<--"),
                            new Telegram.Bot.Types.KeyboardButton((string)SystemSingleton.Configuration.Window.FindResource("m_BotB_GoBack")),
                            new Telegram.Bot.Types.KeyboardButton("-->"),
                        },
                        new[]
                        {
                            new Telegram.Bot.Types.KeyboardButton(Page[0]),
                        },
                        new[]
                        {
                            new Telegram.Bot.Types.KeyboardButton(Page[1]),
                        },
                    },
                    OneTimeKeyboard = false,
                    ResizeKeyboard = true
                };
            }
            else
            {
                return new ReplyKeyboardMarkup
                {
                    Keyboard = new[] {
                        new[]
                        {
                            new Telegram.Bot.Types.KeyboardButton("<--"),
                            new Telegram.Bot.Types.KeyboardButton((string)SystemSingleton.Configuration.Window.FindResource("m_BotB_GoBack")),
                            new Telegram.Bot.Types.KeyboardButton("-->"),
                        },
                        new[]
                        {
                            new Telegram.Bot.Types.KeyboardButton(Page[0]),
                        },
                        new[]
                        {
                            new Telegram.Bot.Types.KeyboardButton(Page[1]),
                        },
                        new[]
                        {
                            new Telegram.Bot.Types.KeyboardButton(Page[2]),
                        },
                    },
                    OneTimeKeyboard = false,
                    ResizeKeyboard = true
                };
            }
        }

        public static ReplyKeyboardMarkup PersRolesKeyBoard(ref int page, ref int pages, Guid roleID)
        {
            var Page = EnvironmentHelper.ThreePersRolesByPage(ref page, ref pages, roleID);
            if (Page == null || Page.Count == 0)
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
            else
            if (Page.Count == 1)
            {
                return new ReplyKeyboardMarkup
                {
                    Keyboard = new[] {
                        new[]
                        {
                            new Telegram.Bot.Types.KeyboardButton("<--"),
                            new Telegram.Bot.Types.KeyboardButton((string)SystemSingleton.Configuration.Window.FindResource("m_BotB_GoBack")),
                            new Telegram.Bot.Types.KeyboardButton("-->"),
                        },
                        new[]
                        {
                            new Telegram.Bot.Types.KeyboardButton(Page[0]),
                        },
                    },
                    OneTimeKeyboard = false,
                    ResizeKeyboard = true
                };
            }
            else if (Page.Count == 2)
            {
                return new ReplyKeyboardMarkup
                {
                    Keyboard = new[] {
                        new[]
                        {
                            new Telegram.Bot.Types.KeyboardButton("<--"),
                            new Telegram.Bot.Types.KeyboardButton((string)SystemSingleton.Configuration.Window.FindResource("m_BotB_GoBack")),
                            new Telegram.Bot.Types.KeyboardButton("-->"),
                        },
                        new[]
                        {
                            new Telegram.Bot.Types.KeyboardButton(Page[0]),
                        },
                        new[]
                        {
                            new Telegram.Bot.Types.KeyboardButton(Page[1]),
                        },
                    },
                    OneTimeKeyboard = false,
                    ResizeKeyboard = true
                };
            }
            else
            {
                return new ReplyKeyboardMarkup
                {
                    Keyboard = new[] {
                        new[]
                        {
                            new Telegram.Bot.Types.KeyboardButton("<--"),
                            new Telegram.Bot.Types.KeyboardButton((string)SystemSingleton.Configuration.Window.FindResource("m_BotB_GoBack")),
                            new Telegram.Bot.Types.KeyboardButton("-->"),
                        },
                        new[]
                        {
                            new Telegram.Bot.Types.KeyboardButton(Page[0]),
                        },
                        new[]
                        {
                            new Telegram.Bot.Types.KeyboardButton(Page[1]),
                        },
                        new[]
                        {
                            new Telegram.Bot.Types.KeyboardButton(Page[2]),
                        },
                    },
                    OneTimeKeyboard = false,
                    ResizeKeyboard = true
                };
            }
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
                        new Telegram.Bot.Types.KeyboardButton((string)SystemSingleton.Configuration.Window.FindResource("m_BotB_GoToMainMenu")),
                    },
                },
                OneTimeKeyboard = false,
                ResizeKeyboard = true
            };
        }
    }
}
