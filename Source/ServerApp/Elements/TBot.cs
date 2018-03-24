using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServerApp.SupportClasses;
using ServerApp.SystemClasses;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace ServerApp.Elements
{
    public class TBot
    {
        public TelegramBotClient Bot;
        public TBot()
        {
            Bot = new TelegramBotClient(SystemSingleton.Configuration.ApiKey);
            MainLoop();
        }
        private async void MainLoop()
        {
            try
            {
                await Bot.SetWebhookAsync("");
            }
            catch (Exception ex)
            {
                EnvironmentHelper.SendFatalLog("No connection on startup");
            }
            int offset = 0;
            while (true)
            {
                Update[] updates;
                decimal conAttempts = 1;
                decimal allwaitingseconds = 0;
                while (true)
                {
                    try
                    {
                        updates = await Bot.GetUpdatesAsync(offset);
                    }
                    catch (Exception ex)
                    {
                        allwaitingseconds += (conAttempts / 10);
                        EnvironmentHelper.SendLog("No connection for the last " + allwaitingseconds + " seconds, attempt = " + conAttempts++);
                        Thread.Sleep((int)conAttempts * 100);
                        continue;
                    }
                    break;
                }
                foreach (var update in updates)
                {
                    ResolveUpdate(update);
                    offset = update.Id + 1;
                }
            }
        }

        private async void ResolveUpdate(Update update)
        {
            EnvironmentHelper.SendLog("from -- " + update.Message.From.Id + " -- " + update.Message.Text);
            var Session = new CurrentSession(update.Message.From.Id);
            if (Session.HasValue && Session.State > 0)
            {
                //TODO: логика взаимодействия после входа
                await Bot.SendTextMessageAsync(update.Message.Chat.Id, "Ыыыыыы", Telegram.Bot.Types.Enums.ParseMode.Default, false, false, 0, Menu.RemoveKeyBoard());
            }
            else if (Session.HasValue && Session.State == 0)
            {
                if (SystemSingleton.Configuration.Waiters.ContainsKey(update.Message.From.Id))
                {
                    var waiter = SystemSingleton.Configuration.Waiters[update.Message.From.Id];
                    if (update.Message.Text == (string)SystemSingleton.Configuration.Window.FindResource("m_BotB_GoToStart"))
                    {
                        GoToStart(update);
                    }
                    else
                    {
                        Login(update, Session, waiter);
                    }
                }
                else
                {
                    SystemSingleton.Configuration.Waiters.Add(
                        update.Message.From.Id,
                        new Waiter
                        {
                            Login = EnvironmentHelper.GetLogin(update.Message.From.Id),
                            TelegramID = update.Message.From.Id,
                            State = LoginWaitersState.WaitForPassword
                        });
                    EnvironmentHelper.SendLog("to -- " + update.Message.From.Id + " -- " + EnvironmentHelper.GetLogin(update.Message.From.Id) + " " + (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_EnterPass"));
                    await Bot.SendTextMessageAsync(update.Message.Chat.Id, EnvironmentHelper.GetLogin(update.Message.From.Id) +" "+ (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_EnterPass"), Telegram.Bot.Types.Enums.ParseMode.Default, false, false, 0, Menu.RemoveKeyBoard());
                }
            }
            else
            {
                if (SystemSingleton.Configuration.Waiters.ContainsKey(update.Message.From.Id))
                {
                    var waiter = SystemSingleton.Configuration.Waiters[update.Message.From.Id];
                    switch (waiter.State)
                    {
                        case LoginWaitersState.WaitForLogin:
                            if(Session.Login("", update.Message.Text, true, 0))
                            {
                                waiter.Login = update.Message.Text;
                                waiter.State = LoginWaitersState.WaitForPassword;
                                EnvironmentHelper.SendLog("to -- " + update.Message.From.Id + " -- " + (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_EnterPass"));
                                await Bot.SendTextMessageAsync(update.Message.Chat.Id, 
                                    (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_EnterPass"), 
                                    Telegram.Bot.Types.Enums.ParseMode.Default, false, false, 0, Menu.RemoveKeyBoard());
                            }
                            else
                            {
                                EnvironmentHelper.SendLog("to -- " + update.Message.From.Id + " -- " + (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_IncorrectLogin"));
                                await Bot.SendTextMessageAsync(update.Message.Chat.Id, 
                                    (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_IncorrectLogin"), 
                                    Telegram.Bot.Types.Enums.ParseMode.Default, false, false, 0, Menu.RemoveKeyBoard());
                            }
                            break;
                        case LoginWaitersState.WaitForPassword:
                            if(update.Message.Text == (string)SystemSingleton.Configuration.Window.FindResource("m_BotB_GoToStart"))
                            {
                                GoToStart(update);
                            }
                            else
                            {
                                Login(update, Session, waiter);
                            }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                else
                {
                    if (update.Message.Text == (string)SystemSingleton.Configuration.Window.FindResource("m_BotB_LogIn"))
                    {
                        SystemSingleton.Configuration.Waiters.Add(
                        update.Message.From.Id,
                        new Waiter
                        {
                            Login = "",
                            TelegramID = update.Message.From.Id,
                            State = LoginWaitersState.WaitForLogin
                        });
                        EnvironmentHelper.SendLog("to -- " + update.Message.From.Id + " -- " + (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_EnterLogin"));
                        await Bot.SendTextMessageAsync(update.Message.Chat.Id, (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_EnterLogin"), Telegram.Bot.Types.Enums.ParseMode.Default, false, false, 0, Menu.RemoveKeyBoard());
                    }
                    else
                    {
                        EnvironmentHelper.SendLog("to -- " + update.Message.From.Id + " -- " + (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_TelegramIdNotFound"));
                        await Bot.SendTextMessageAsync(update.Message.Chat.Id, (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_TelegramIdNotFound"), Telegram.Bot.Types.Enums.ParseMode.Default, false, false, 0, Menu.LogInKeyBoard());
                    }
                }
            }
        }

        private async void Login(Update update, CurrentSession Session, Waiter waiter)
        {
            if (Session.Login(update.Message.Text, waiter.Login, false, update.Message.From.Id))
            {
                SystemSingleton.Configuration.Waiters.Remove(update.Message.From.Id);
                //TODO: клавиатура
                EnvironmentHelper.SendLog("to -- " + update.Message.From.Id + " -- " + (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_LoginOK"));
                await Bot.SendTextMessageAsync(update.Message.Chat.Id,
                    (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_LoginOK"),
                    ParseMode.Default, false, false, 0, Menu.RemoveKeyBoard());
            }
            else
            {
                EnvironmentHelper.SendLog("to -- " + update.Message.From.Id + " -- " + (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_IncorrectPassword"));
                await Bot.SendTextMessageAsync(update.Message.Chat.Id,
                    (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_IncorrectPassword"),
                    ParseMode.Default, false, false, 0, Menu.GoBackFromPasswordKeyBoard());
            }
        }

        private async void GoToStart(Update update)
        {
            SystemSingleton.Configuration.Waiters.Remove(update.Message.From.Id);
            EnvironmentHelper.SendLog("to -- " + update.Message.From.Id + " -- " + (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_StartPage"));
            await Bot.SendTextMessageAsync(update.Message.Chat.Id,
                (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_StartPage"),
                Telegram.Bot.Types.Enums.ParseMode.Default, false, false, 0, Menu.LogInKeyBoard());
        }
    }
}
