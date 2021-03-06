﻿using System;
using System.Collections.Generic;
using System.IO;
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
        private System.Timers.Timer Timer;
        public TelegramBotClient Bot;
        public TBot()
        {
            Bot = new TelegramBotClient(SystemSingleton.Configuration.ApiKey);
            Timer = new System.Timers.Timer();
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
            int seconds = 30;
            Timer.AutoReset = true;
            Timer.Interval = seconds * 1000;
            Timer.Elapsed += CheckCompletedTasks;
            Timer.Enabled = true;
            Timer.Start();
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

        private async void CheckCompletedTasks(object sender, EventArgs e)
        {
            decimal conAttempts = 1;
            decimal allwaitingseconds = 0;
            while (true)
            {
                try
                {
                    await Bot.TestApiAsync();
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
            List<Guid> taskID = EnvironmentHelper.GetCompletedID();
            foreach (var task in taskID)
            {
                if (EnvironmentHelper.FindResultTask(task, out string infomsg, out Dictionary<Guid, string> files, out long ChatID, out string docNumber))
                {
                    EnvironmentHelper.SendLog("to chat id -- " + ChatID + " -- " + infomsg);
                    if (ChatID == 0)
                    {
                        EnvironmentHelper.SendLog("Chat id " + ChatID + " not found, can't send result of task!");
                        continue;
                    }
                    else
                    {
                        await Bot.SendTextMessageAsync(ChatID, infomsg);
                    }
                    foreach (var item in files)
                    {
                        if (System.IO.File.Exists(SystemSingleton.Configuration.FilesPath + item.Key + "\\" + item.Value))
                        {
                            if (new System.IO.FileInfo(SystemSingleton.Configuration.FilesPath + item.Key + "\\" + item.Value).Length > 50000000)//На всякий случай возьмем 47 Мб
                            {
                                EnvironmentHelper.SendLog("to chat id -- " + ChatID + " -- " + item.Value + " " + (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_MoreThan50"));
                                await Bot.SendTextMessageAsync(ChatID, docNumber+" "+ item.Value + " " + (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_MoreThan50"));
                            }
                            else
                            {
                                using (var fileStream = new FileStream(SystemSingleton.Configuration.FilesPath + item.Key + "\\" + item.Value, FileMode.Open, FileAccess.Read, FileShare.Read))
                                {
                                    EnvironmentHelper.SendLog("to chat id -- " + ChatID + " -- " + item.Value + " sended");
                                    await Bot.SendDocumentAsync(ChatID,new FileToSend(SystemSingleton.Configuration.FilesPath + item.Key + "\\" + item.Value,fileStream), docNumber);
                                }
                            }
                        }
                        else
                        {
                            EnvironmentHelper.SendLog(item.Key + "\\" + item.Value + " --- not found!");
                        }
                    }

                    EnvironmentHelper.DeleteCompletedID(task);
                }
            }
        }

        private void ResolveUpdate(Update update)
        {
            EnvironmentHelper.SendLog("from -- " + update.Message.From.Id + " -- " + update.Message.Text);
            var Session = new CurrentSession(update.Message.From.Id, update.Message.Chat.Id);
            if (Session.HasValue)
            {
                switch (Session.State)
                {
                    case 0:
                        ResolveStateZero(update, Session);
                        break;
                    case 1:
                        ResolveStateOne(update, Session);
                        break;
                    case 2:
                        ResolveStateTwo(update, Session);
                        break;
                    case 3:
                        ResolveStateThree(update, Session);
                        break;
                    case 4:
                        ResolveStateFourth(update, Session);
                        break;
                    case 5:
                        ResolveStateFive(update, Session);
                        break;
                    case 6:
                        ResolveStateSix(update, Session);
                        break;
                    case 7:
                        ResolveStateSeven(update, Session);
                        break;
                    case 8:
                        ResolveStateEight(update, Session);
                        break;
                    case 9:
                        ResolveStateNine(update, Session);
                        break;
                    case 10:
                        ResolveStateTen(update, Session);
                        break;
                    default:
                        throw new Exception("Status error, user " + Session.ID);
                }
            }
            else
            {
                ResolveNoState(update, Session);
            }
        }

        private async void ResolveNoState(Update update, CurrentSession session)
        {
            if (SystemSingleton.Configuration.Waiters.ContainsKey(update.Message.From.Id))
            {
                var waiter = SystemSingleton.Configuration.Waiters[update.Message.From.Id];
                switch (waiter.State)
                {
                    case LoginWaitersState.WaitForLogin:
                        if (session.Login("", update.Message.Text, true, 0))
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
                        if (update.Message.Text == (string)SystemSingleton.Configuration.Window.FindResource("m_BotB_GoToStart"))
                        {
                            GoToStart(update);
                        }
                        else
                        {
                            Login(update, session, waiter);
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

        private async void ResolveStateZero(Update update, CurrentSession session)
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
                    Login(update, session, waiter);
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
                await Bot.SendTextMessageAsync(update.Message.Chat.Id, EnvironmentHelper.GetLogin(update.Message.From.Id) + " " + (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_EnterPass"), Telegram.Bot.Types.Enums.ParseMode.Default, false, false, 0, Menu.RemoveKeyBoard());
            }
        }

        private async void ResolveStateOne(Update update, CurrentSession session)
        {
            if (update.Message.Text == (string) SystemSingleton.Configuration.Window.FindResource("m_BotB_LogOff"))
            {
                session.State = 0;
                session.ChatID = update.Message.Chat.Id;
                session.CloseSession();
                EnvironmentHelper.SendLog("to -- " + update.Message.From.Id + " -- " + (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_EnterLogin"));
                await Bot.SendTextMessageAsync(update.Message.Chat.Id,
                    (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_EnterLogin"),
                    ParseMode.Default, false, false, 0, Menu.GoBackFromPasswordKeyBoard());
            }
            else if (update.Message.Text == (string) SystemSingleton.Configuration.Window.FindResource("m_BotB_ReqDoc"))
            {
                session.State = 2;
                session.ChatID = update.Message.Chat.Id;
                session.DocumentTypesPage = 1;
                session.ChoosenRole = null;
                session.ChoosenDocType = null;
                session.CloseSession();
                EnvironmentHelper.SendLog("to -- " + update.Message.From.Id + " -- " + (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_ChoseInputVariant"));
                await Bot.SendTextMessageAsync(update.Message.Chat.Id,
                    (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_ChoseInputVariant"),
                    ParseMode.Default, false, false, 0, Menu.InputTypeKeyBoard());
            }
            else if (update.Message.Text == (string)SystemSingleton.Configuration.Window.FindResource("m_BotB_History"))
            {
                session.State = 10;
                session.ChatID = update.Message.Chat.Id;
                session.HistoryPage = 1;
                int pages = 0;
                var keyboard = Menu.HistoryKeyBoard(ref session.HistoryPage, ref pages, session.ID.Value);
                session.CloseSession();
                EnvironmentHelper.SendLog("to -- " + update.Message.From.Id + " -- " + (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_HistoryChoose"));
                await Bot.SendTextMessageAsync(update.Message.Chat.Id,
                    (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_HistoryChoose"),
                    ParseMode.Default, false, false, 0, keyboard);
            }
            else if (update.Message.Text == (string)SystemSingleton.Configuration.Window.FindResource("m_BotB_CurrentTasks"))
            {
                session.State = 9;
                session.ChatID = update.Message.Chat.Id;
                session.CurrentTasksPage = 1;
                int pages = 0;
                var keyboard = Menu.CurrentTasksKeyBoard(ref session.CurrentTasksPage, ref pages, session.ID.Value);
                session.CloseSession();
                EnvironmentHelper.SendLog("to -- " + update.Message.From.Id + " -- " + (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_CurrentTasksChoose"));
                await Bot.SendTextMessageAsync(update.Message.Chat.Id,
                    (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_CurrentTasksChoose"),
                    ParseMode.Default, false, false, 0, keyboard);
            }
            else
            {
                EnvironmentHelper.SendLog("to -- " + update.Message.From.Id + " -- " + (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_NotCorrectMSG"));
                await Bot.SendTextMessageAsync(update.Message.Chat.Id,
                    (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_NotCorrectMSG"),
                    ParseMode.Default, false, false, 0, Menu.MainMenuKeyBoard());
            }
        }

        private async void ResolveStateTwo(Update update, CurrentSession session)
        {
            if (update.Message.Text == (string)SystemSingleton.Configuration.Window.FindResource("m_BotB_FromList"))
            {
                DocumentTypes documentTypes = new DocumentTypes();
                if (!documentTypes.HasValue)
                {
                    session.State = 1;
                    session.ChatID = update.Message.Chat.Id;
                    session.CloseSession();
                    EnvironmentHelper.SendLog("to -- " + update.Message.From.Id + " -- " + (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_NoOneDocType"));
                    await Bot.SendTextMessageAsync(update.Message.Chat.Id,
                        (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_NoOneDocType"),
                        ParseMode.Default, false, false, 0, Menu.MainMenuKeyBoard());
                }
                else
                {
                    session.State = 3;
                    session.ChatID = update.Message.Chat.Id;
                    session.DocumentTypesPage = 1;
                    int pages = 0;
                    var keyboard = Menu.DocTypesKeyBoard(ref session.DocumentTypesPage,ref pages);
                    session.CloseSession();
                    EnvironmentHelper.SendLog("to -- " + update.Message.From.Id + " -- " + (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_DocTypesChoose"));
                    await Bot.SendTextMessageAsync(update.Message.Chat.Id,
                        (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_DocTypesChoose"),
                        ParseMode.Default, false, false, 0, keyboard);
                    
                }
            }
            else if (update.Message.Text == (string)SystemSingleton.Configuration.Window.FindResource("m_BotB_Manually"))
            {
                DocumentTypes documentTypes = new DocumentTypes();
                if (!documentTypes.HasValue)
                {
                    session.State = 1;
                    session.ChatID = update.Message.Chat.Id;
                    session.CloseSession();
                    EnvironmentHelper.SendLog("to -- " + update.Message.From.Id + " -- " + (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_NoOneDocType"));
                    await Bot.SendTextMessageAsync(update.Message.Chat.Id,
                        (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_NoOneDocType"),
                        ParseMode.Default, false, false, 0, Menu.MainMenuKeyBoard());
                }
                else
                {
                    session.State = 4;
                    session.ChatID = update.Message.Chat.Id;
                    session.CloseSession();
                    EnvironmentHelper.SendLog("to -- " + update.Message.From.Id + " -- " + (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_InputTag"));
                    await Bot.SendTextMessageAsync(update.Message.Chat.Id,
                        (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_InputTag"),
                        ParseMode.Default, false, false, 0, Menu.RemoveKeyBoard());
                }
            }
            else if (update.Message.Text == (string) SystemSingleton.Configuration.Window.FindResource("m_BotB_GoToMainMenu"))
            {
                session.State = 1;
                session.ChatID = update.Message.Chat.Id;
                session.CloseSession();
                EnvironmentHelper.SendLog("to -- " + update.Message.From.Id + " -- " + (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_MainMenu"));
                await Bot.SendTextMessageAsync(update.Message.Chat.Id,
                    (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_MainMenu"),
                    ParseMode.Default, false, false, 0, Menu.MainMenuKeyBoard());
            }
            else
            {
                EnvironmentHelper.SendLog("to -- " + update.Message.From.Id + " -- " + (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_NotCorrectMSG"));
                await Bot.SendTextMessageAsync(update.Message.Chat.Id,
                    (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_NotCorrectMSG"),
                    ParseMode.Default, false, false, 0, Menu.InputTypeKeyBoard());
            }
        }

        private async void ResolveStateThree(Update update, CurrentSession session)
        {
            DocumentTypes documentTypes = new DocumentTypes();
            int pages=0;
            if (update.Message.Text == (string)SystemSingleton.Configuration.Window.FindResource("m_BotB_GoBack"))
            {
                session.State = 2;
                session.ChatID = update.Message.Chat.Id;
                session.DocumentTypesPage = 1;
                session.ChoosenRole = null;
                session.ChoosenDocType = null;
                session.CloseSession();
                EnvironmentHelper.SendLog("to -- " + update.Message.From.Id + " -- " + (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_ChoseInputVariant"));
                await Bot.SendTextMessageAsync(update.Message.Chat.Id,
                    (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_ChoseInputVariant"),
                    ParseMode.Default, false, false, 0, Menu.InputTypeKeyBoard());
            }else
            if (update.Message.Text == "-->")
            {
                session.State = 3;
                session.ChatID = update.Message.Chat.Id;
                session.DocumentTypesPage++;
                var keyboard = Menu.DocTypesKeyBoard(ref session.DocumentTypesPage, ref pages);
                session.CloseSession();
                EnvironmentHelper.SendLog("to -- " + update.Message.From.Id + " -- " + (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_Page") + " " + session.DocumentTypesPage + "/" + pages);
                await Bot.SendTextMessageAsync(update.Message.Chat.Id,
                    (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_Page") +" "+ session.DocumentTypesPage+"/"+pages,
                    ParseMode.Default, false, false, 0, keyboard);
            }
            else
            if (update.Message.Text == "<--")
            {
                session.State = 3;
                session.ChatID = update.Message.Chat.Id;
                session.DocumentTypesPage--;
                var keyboard = Menu.DocTypesKeyBoard(ref session.DocumentTypesPage, ref pages);
                session.CloseSession();
                EnvironmentHelper.SendLog("to -- " + update.Message.From.Id + " -- " + (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_Page") + " " + session.DocumentTypesPage + "/" + pages);
                await Bot.SendTextMessageAsync(update.Message.Chat.Id,
                    (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_Page") + " " + session.DocumentTypesPage + "/" + pages,
                    ParseMode.Default, false, false, 0, keyboard);
            }
            else
            {
                foreach (var item in documentTypes.TypesCaptions)
                {
                    if (item.Value==update.Message.Text)
                    {
                        if (EnvironmentHelper.IsDocContainsStaticRole(item.Key))
                        {
                            session.State = 5;
                            session.ChatID = update.Message.Chat.Id;
                            session.ChoosenDocType = item.Key;
                            session.CloseSession();
                            EnvironmentHelper.SendLog("to -- " + update.Message.From.Id + " -- " + (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_DocTypeFounded"));
                            await Bot.SendTextMessageAsync(update.Message.Chat.Id,
                                (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_DocTypeFounded"),
                                ParseMode.Default, false, false, 0, Menu.AllOrConcreteRoleKeyBoard());
                        }
                        else
                        {
                            session.State = 7;
                            session.ChatID = update.Message.Chat.Id;
                            session.ChoosenDocType = item.Key;
                            session.CloseSession();
                            EnvironmentHelper.SendLog("to -- " + update.Message.From.Id + " -- " + (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_DocTypeFoundedComment"));
                            await Bot.SendTextMessageAsync(update.Message.Chat.Id,
                                (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_DocTypeFoundedComment"),
                                ParseMode.Default, false, false, 0, Menu.RemoveKeyBoard());
                        }
                        return;
                    }
                }
                EnvironmentHelper.SendLog("to -- " + update.Message.From.Id + " -- " + (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_DocTypeNotFounded"));
                await Bot.SendTextMessageAsync(update.Message.Chat.Id,
                    (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_DocTypeNotFounded"),
                    ParseMode.Default, false, false, 0, Menu.GoBackKeyBoard());
            }
        }

        private async void ResolveStateFourth(Update update, CurrentSession session)
        {
            DocumentTypes documentTypes = new DocumentTypes();
            if (update.Message.Text == (string)SystemSingleton.Configuration.Window.FindResource("m_BotB_GoBack"))
            {
                session.State = 2;
                session.ChatID = update.Message.Chat.Id;
                session.DocumentTypesPage = 1;
                session.ChoosenRole = null;
                session.ChoosenDocType = null;
                session.CloseSession();
                EnvironmentHelper.SendLog("to -- " + update.Message.From.Id + " -- " + (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_ChoseInputVariant"));
                await Bot.SendTextMessageAsync(update.Message.Chat.Id,
                    (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_ChoseInputVariant"),
                    ParseMode.Default, false, false, 0, Menu.InputTypeKeyBoard());
            }
            else
            {
                foreach (var item in documentTypes.Types)
                {
                    foreach (var tag in item.Value.Tags)
                    {
                        if(tag.ToLower().Contains(update.Message.Text.ToLower()) || update.Message.Text.ToLower().Contains(tag.ToLower()))
                        {
                            if (EnvironmentHelper.IsDocContainsStaticRole(item.Key))
                            {
                                session.State = 5;
                                session.ChatID = update.Message.Chat.Id;
                                session.ChoosenDocType = item.Key;
                                session.DocumentTypesPage = 1;
                                session.CloseSession();
                                EnvironmentHelper.SendLog("to -- " + update.Message.From.Id + " -- " + (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_DocTypeFounded"));
                                await Bot.SendTextMessageAsync(update.Message.Chat.Id,
                                    (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_DocTypeFounded"),
                                    ParseMode.Default, false, false, 0, Menu.AllOrConcreteRoleKeyBoard());
                            }
                            else
                            {
                                session.State = 7;
                                session.ChatID = update.Message.Chat.Id;
                                session.ChoosenDocType = item.Key;
                                session.DocumentTypesPage = 1;
                                session.CloseSession();
                                EnvironmentHelper.SendLog("to -- " + update.Message.From.Id + " -- " + (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_DocTypeFoundedComment"));
                                await Bot.SendTextMessageAsync(update.Message.Chat.Id,
                                    (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_DocTypeFoundedComment"),
                                    ParseMode.Default, false, false, 0, Menu.RemoveKeyBoard());
                            }
                            return;
                        }
                    }
                }
                EnvironmentHelper.SendLog("to -- " + update.Message.From.Id + " -- " + (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_DocTypeNotFounded"));
                await Bot.SendTextMessageAsync(update.Message.Chat.Id,
                    (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_DocTypeNotFounded"),
                    ParseMode.Default, false, false, 0, Menu.GoBackKeyBoard());
            }
        }

        private async void ResolveStateFive(Update update, CurrentSession session)
        {
            if (update.Message.Text == (string)SystemSingleton.Configuration.Window.FindResource("m_BotB_ToAll"))
            {
                session.State = 7;
                session.ChatID = update.Message.Chat.Id;
                session.ChoosenRole = EnvironmentHelper.GetRoleFromDocType(session.ChoosenDocType.Value);
                session.CloseSession();
                EnvironmentHelper.SendLog("to -- " + update.Message.From.Id + " -- " + (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_SetComment"));
                await Bot.SendTextMessageAsync(update.Message.Chat.Id,
                    (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_SetComment"),
                    ParseMode.Default, false, false, 0, Menu.RemoveKeyBoard());
            }
            else if (update.Message.Text == (string)SystemSingleton.Configuration.Window.FindResource("m_BotB_ToConcrete"))
            {
                session.State = 6;
                session.ChatID = update.Message.Chat.Id;
                session.ChoosenRole = EnvironmentHelper.GetRoleFromDocType(session.ChoosenDocType.Value);
                session.PersonalRolesPage = 1;
                int pages = 0;
                var keyboard = Menu.PersRolesKeyBoard(ref session.PersonalRolesPage, ref pages, session.ChoosenRole.Value);
                session.CloseSession();
                EnvironmentHelper.SendLog("to -- " + update.Message.From.Id + " -- " + (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_ChoosePersRole"));
                await Bot.SendTextMessageAsync(update.Message.Chat.Id,
                    (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_ChoosePersRole"),
                    ParseMode.Default, false, false, 0, keyboard);
            }
            else if (update.Message.Text == (string)SystemSingleton.Configuration.Window.FindResource("m_BotB_GoToMainMenu"))
            {
                session.State = 1;
                session.ChatID = update.Message.Chat.Id;
                session.ChoosenDocType = null;
                session.ChoosenRole = null;
                session.CloseSession();
                EnvironmentHelper.SendLog("to -- " + update.Message.From.Id + " -- " + (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_MainMenu"));
                await Bot.SendTextMessageAsync(update.Message.Chat.Id,
                    (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_MainMenu"),
                    ParseMode.Default, false, false, 0, Menu.MainMenuKeyBoard());
            }
            else
            {
                EnvironmentHelper.SendLog("to -- " + update.Message.From.Id + " -- " + (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_NotCorrectMSG"));
                await Bot.SendTextMessageAsync(update.Message.Chat.Id,
                    (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_NotCorrectMSG"),
                    ParseMode.Default, false, false, 0, Menu.InputTypeKeyBoard());
            }
        }

        private async void ResolveStateSix(Update update, CurrentSession session)
        {
            int pages = 0;
            if (update.Message.Text == (string)SystemSingleton.Configuration.Window.FindResource("m_BotB_GoBack"))
            {
                session.State = 5;
                session.ChatID = update.Message.Chat.Id;
                session.PersonalRolesPage = 1;
                session.CloseSession();
                EnvironmentHelper.SendLog("to -- " + update.Message.From.Id + " -- " + (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_ChoseGoingVariant"));
                await Bot.SendTextMessageAsync(update.Message.Chat.Id,
                    (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_ChoseGoingVariant"),
                    ParseMode.Default, false, false, 0, Menu.AllOrConcreteRoleKeyBoard());
            }
            else
            if (update.Message.Text == "-->")
            {
                session.State = 6;
                session.ChatID = update.Message.Chat.Id;
                session.PersonalRolesPage++;
                var keyboard = Menu.PersRolesKeyBoard(ref session.PersonalRolesPage, ref pages, session.ChoosenRole.Value);
                session.CloseSession();
                EnvironmentHelper.SendLog("to -- " + update.Message.From.Id + " -- " + (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_Page") + " " + session.PersonalRolesPage + "/" + pages);
                await Bot.SendTextMessageAsync(update.Message.Chat.Id,
                    (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_Page") + " " + session.PersonalRolesPage + "/" + pages,
                    ParseMode.Default, false, false, 0, keyboard);
            }
            else
            if (update.Message.Text == "<--")
            {
                session.State = 6;
                session.ChatID = update.Message.Chat.Id;
                session.PersonalRolesPage--;
                var keyboard = Menu.PersRolesKeyBoard(ref session.PersonalRolesPage, ref pages, session.ChoosenRole.Value);
                session.CloseSession();
                EnvironmentHelper.SendLog("to -- " + update.Message.From.Id + " -- " + (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_Page") + " " + session.PersonalRolesPage + "/" + pages);
                await Bot.SendTextMessageAsync(update.Message.Chat.Id,
                    (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_Page") + " " + session.PersonalRolesPage + "/" + pages,
                    ParseMode.Default, false, false, 0, keyboard);
            }
            else
            {
                if (EnvironmentHelper.FindRoleByLastAndFirstName(update.Message.Text, out Guid role))
                {
                    session.State = 7;
                    session.ChatID = update.Message.Chat.Id;
                    session.ChoosenRole = role;
                    session.PersonalRolesPage = 1;
                    session.CloseSession();
                    EnvironmentHelper.SendLog("to -- " + update.Message.From.Id + " -- " + (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_SetComment"));
                    await Bot.SendTextMessageAsync(update.Message.Chat.Id,
                        (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_SetComment"),
                        ParseMode.Default, false, false, 0, Menu.RemoveKeyBoard());
                }
                else
                {
                    EnvironmentHelper.SendLog("to -- " + update.Message.From.Id + " -- " + (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_PersonalRoleNotFounded"));
                    await Bot.SendTextMessageAsync(update.Message.Chat.Id,
                        (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_PersonalRoleNotFounded"),
                        ParseMode.Default, false, false, 0, Menu.GoBackKeyBoard());
                }
            }
        }

        private async void ResolveStateSeven(Update update, CurrentSession session)
        {
            session.State = 8;
            session.ChatID = update.Message.Chat.Id;
            session.Commentary = update.Message.Text;
            session.CloseSession();
            string msg = EnvironmentHelper.PrepareMessageNewTask(session);
            EnvironmentHelper.SendLog("to -- " + update.Message.From.Id + " -- " + (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_ReadyToSend") + "\n\n" +
                                      msg);
            await Bot.SendTextMessageAsync(update.Message.Chat.Id,
                (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_ReadyToSend") + "\n\n" +
                msg, ParseMode.Default, false, false, 0, Menu.ReadyToSend());
        }

        private async void ResolveStateEight(Update update, CurrentSession session)
        {
            if (update.Message.Text == (string)SystemSingleton.Configuration.Window.FindResource("m_BotB_SendTask"))
            {
                string taskNumber = session.FormAndSendTask();
                session.State = 1;
                session.ChatID = update.Message.Chat.Id;
                session.ChoosenDocType = null;
                session.ChoosenRole = null;
                session.Commentary = "";
                session.CloseSession();
                EnvironmentHelper.SendLog("to -- " + update.Message.From.Id + " -- " + taskNumber + " " + (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_Sended"));
                await Bot.SendTextMessageAsync(update.Message.Chat.Id,
                    taskNumber +" "+ (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_Sended"),
                    ParseMode.Default, false, false, 0, Menu.MainMenuKeyBoard());
            }
            else if (update.Message.Text == (string)SystemSingleton.Configuration.Window.FindResource("m_BotB_DontSendTask"))
            {
                session.State = 1;
                session.ChatID = update.Message.Chat.Id;
                session.ChoosenDocType = null;
                session.ChoosenRole = null;
                session.Commentary = "";
                session.CloseSession();
                EnvironmentHelper.SendLog("to -- " + update.Message.From.Id + " -- " + (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_MainMenu"));
                await Bot.SendTextMessageAsync(update.Message.Chat.Id,
                    (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_MainMenu"),
                    ParseMode.Default, false, false, 0, Menu.MainMenuKeyBoard());
            }
            else
            {
                EnvironmentHelper.SendLog("to -- " + update.Message.From.Id + " -- " + (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_NotCorrectMSG"));
                await Bot.SendTextMessageAsync(update.Message.Chat.Id,
                    (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_NotCorrectMSG"),
                    ParseMode.Default, false, false, 0, Menu.ReadyToSend());
            }
        }

        private async void ResolveStateNine(Update update, CurrentSession session)
        {
            int pages = 0;
            if (update.Message.Text == (string)SystemSingleton.Configuration.Window.FindResource("m_BotB_GoBack"))
            {
                session.State = 1;
                session.ChatID = update.Message.Chat.Id;
                session.CurrentTasksPage = 1;
                session.CloseSession();
                EnvironmentHelper.SendLog("to -- " + update.Message.From.Id + " -- " + (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_MainMenu"));
                await Bot.SendTextMessageAsync(update.Message.Chat.Id,
                    (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_MainMenu"),
                    ParseMode.Default, false, false, 0, Menu.MainMenuKeyBoard());
            }
            else
            if (update.Message.Text == "-->")
            {
                session.State = 9;
                session.ChatID = update.Message.Chat.Id;
                session.CurrentTasksPage++;
                var keyboard = Menu.CurrentTasksKeyBoard(ref session.CurrentTasksPage, ref pages, session.ID.Value);
                session.CloseSession();
                EnvironmentHelper.SendLog("to -- " + update.Message.From.Id + " -- " + (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_Page") + " " + session.CurrentTasksPage + "/" + pages);
                await Bot.SendTextMessageAsync(update.Message.Chat.Id,
                    (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_Page") + " " + session.CurrentTasksPage + "/" + pages,
                    ParseMode.Default, false, false, 0, keyboard);
            }
            else
            if (update.Message.Text == "<--")
            {
                session.State = 9;
                session.ChatID = update.Message.Chat.Id;
                session.CurrentTasksPage--;
                var keyboard = Menu.CurrentTasksKeyBoard(ref session.CurrentTasksPage, ref pages, session.ID.Value);
                session.CloseSession();
                EnvironmentHelper.SendLog("to -- " + update.Message.From.Id + " -- " + (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_Page") + " " + session.CurrentTasksPage + "/" + pages);
                await Bot.SendTextMessageAsync(update.Message.Chat.Id,
                    (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_Page") + " " + session.CurrentTasksPage + "/" + pages,
                    ParseMode.Default, false, false, 0, keyboard);
            }
            else
            {
                if (EnvironmentHelper.FindInfoTask(update.Message.Text, out string infomsg))
                {
                    session.State = 9;
                    session.ChatID = update.Message.Chat.Id;
                    var keyboard = Menu.CurrentTasksKeyBoard(ref session.CurrentTasksPage, ref pages, session.ID.Value);
                    session.CloseSession();
                    EnvironmentHelper.SendLog("to -- " + update.Message.From.Id + " -- " + infomsg);
                    await Bot.SendTextMessageAsync(update.Message.Chat.Id,
                        infomsg,
                        ParseMode.Default, false, false, 0, keyboard);
                }
                else
                {
                    var keyboard = Menu.CurrentTasksKeyBoard(ref session.CurrentTasksPage, ref pages, session.ID.Value);
                    EnvironmentHelper.SendLog("to -- " + update.Message.From.Id + " -- " + (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_TaskNotFound"));
                    await Bot.SendTextMessageAsync(update.Message.Chat.Id,
                        (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_TaskNotFound"),
                        ParseMode.Default, false, false, 0, keyboard);
                }
            }
        }

        private async void ResolveStateTen(Update update, CurrentSession session)
        {
            int pages = 0;
            if (update.Message.Text == (string)SystemSingleton.Configuration.Window.FindResource("m_BotB_GoBack"))
            {
                session.State = 1;
                session.ChatID = update.Message.Chat.Id;
                session.HistoryPage = 1;
                session.CloseSession();
                EnvironmentHelper.SendLog("to -- " + update.Message.From.Id + " -- " + (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_MainMenu"));
                await Bot.SendTextMessageAsync(update.Message.Chat.Id,
                    (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_MainMenu"),
                    ParseMode.Default, false, false, 0, Menu.MainMenuKeyBoard());
            }
            else
            if (update.Message.Text == "-->")
            {
                session.State = 10;
                session.ChatID = update.Message.Chat.Id;
                session.HistoryPage++;
                var keyboard = Menu.HistoryKeyBoard(ref session.HistoryPage, ref pages, session.ID.Value);
                session.CloseSession();
                EnvironmentHelper.SendLog("to -- " + update.Message.From.Id + " -- " + (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_Page") + " " + session.HistoryPage + "/" + pages);
                await Bot.SendTextMessageAsync(update.Message.Chat.Id,
                    (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_Page") + " " + session.HistoryPage + "/" + pages,
                    ParseMode.Default, false, false, 0, keyboard);
            }
            else
            if (update.Message.Text == "<--")
            {
                session.State = 10;
                session.ChatID = update.Message.Chat.Id;
                session.HistoryPage--;
                var keyboard = Menu.HistoryKeyBoard(ref session.HistoryPage, ref pages, session.ID.Value);
                session.CloseSession();
                EnvironmentHelper.SendLog("to -- " + update.Message.From.Id + " -- " + (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_Page") + " " + session.HistoryPage + "/" + pages);
                await Bot.SendTextMessageAsync(update.Message.Chat.Id,
                    (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_Page") + " " + session.HistoryPage + "/" + pages,
                    ParseMode.Default, false, false, 0, keyboard);
            }
            else
            {
                if (EnvironmentHelper.FindResultTask(update.Message.Text, out string infomsg, out Dictionary<Guid, string> files, out string docNumber))
                {
                    session.State = 10;
                    session.ChatID = update.Message.Chat.Id;
                    var keyboard = Menu.HistoryKeyBoard(ref session.HistoryPage, ref pages, session.ID.Value);
                    session.CloseSession();
                    EnvironmentHelper.SendLog("to -- " + update.Message.From.Id + " -- " + infomsg);
                    await Bot.SendTextMessageAsync(update.Message.Chat.Id,infomsg,ParseMode.Default, false, false, 0, keyboard);
                    foreach (var item in files)
                    {
                        if (System.IO.File.Exists(SystemSingleton.Configuration.FilesPath + item.Key + "\\" + item.Value))
                        {
                            if(new System.IO.FileInfo(SystemSingleton.Configuration.FilesPath + item.Key + "\\" + item.Value).Length>50000000)//На всякий случай возьмем 47 Мб
                            {
                                EnvironmentHelper.SendLog("to -- " + update.Message.From.Id + " -- " + item.Value + " " + (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_MoreThan50"));
                                await Bot.SendTextMessageAsync(update.Message.Chat.Id, docNumber+" "+ item.Value + " " + (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_MoreThan50"));
                            }
                            else
                            {
                                using (var fileStream = new FileStream(SystemSingleton.Configuration.FilesPath + item.Key + "\\" + item.Value, FileMode.Open, FileAccess.Read, FileShare.Read))
                                {
                                    EnvironmentHelper.SendLog("to -- " + update.Message.From.Id + " -- " + item.Value + " sended");
                                    await Bot.SendDocumentAsync(update.Message.Chat.Id,
                                        new FileToSend(
                                            SystemSingleton.Configuration.FilesPath + item.Key + "\\" + item.Value,
                                            fileStream), docNumber);
                                }
                            }
                        }
                        else
                        {
                            EnvironmentHelper.SendLog(item.Key + "\\" + item.Value + " --- not found!" );
                        }
                    }
                }
                else
                {
                    var keyboard = Menu.HistoryKeyBoard(ref session.HistoryPage, ref pages, session.ID.Value);
                    EnvironmentHelper.SendLog("to -- " + update.Message.From.Id + " -- " + (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_TaskNotFound"));
                    await Bot.SendTextMessageAsync(update.Message.Chat.Id,
                        (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_TaskNotFound"),
                        ParseMode.Default, false, false, 0, keyboard);
                }
            }
        }

        private async void Login(Update update, CurrentSession Session, Waiter waiter)
        {
            if (Session.Login(update.Message.Text, waiter.Login, false, update.Message.From.Id))
            {
                SystemSingleton.Configuration.Waiters.Remove(update.Message.From.Id);
                EnvironmentHelper.SendLog("to -- " + update.Message.From.Id + " -- " + (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_LoginOK"));
                await Bot.SendTextMessageAsync(update.Message.Chat.Id,
                    (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_LoginOK"),
                    ParseMode.Default, false, false, 0, Menu.MainMenuKeyBoard());
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
