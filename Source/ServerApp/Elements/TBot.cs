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
            await Bot.SetWebhookAsync("");
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
                    conAttempts = 1;
                    break;
                }
                ResolveUpdates(updates);
            }
        }

        private async void ResolveUpdates(Update[] updates)
        {
            foreach (var update in updates)
            {
                EnvironmentHelper.SendLog("from -- "+update.Message.From.Id + " -- " + update.Message.Text);
                var Session = new CurrentSession(update.Message.From.Id);
                if (Session.HasValue && Session.State>0)
                {

                }
                else
                {
                    await Bot.SendTextMessageAsync(update.Message.Chat.Id, (string)SystemSingleton.Configuration.Window.FindResource("m_BotM_TelegramIdNotFound"), Telegram.Bot.Types.Enums.ParseMode.Default, false, false, 0, Menu.LogInKeyBoard());
                }
            }
        }
    }
}
