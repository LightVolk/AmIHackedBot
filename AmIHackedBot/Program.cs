using System;
using System.Text;
using Telegram.Bot;

namespace AmIHackedBot
{
    class Program
    {
        private static TelegramBotClient Bot = BotClient.Instance;
        static void Main(string[] args)
        {
            Bot.OnMessage += Bot_OnMessage;
            Console.WriteLine("Hello World!");
        }

        /// <summary>
        /// Handle messages from user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Bot_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            if (e.Message.Text.StartsWith("/start"))
            {
                var msg = new StringBuilder();
                msg.AppendLine("Welcome!");
                msg.AppendLine("This bot works with www.haveibeenpwned.com");
                msg.AppendLine("Type your email to find are your hacked");

                BotClient.Instance.SendTextMessageAsync(e.Message.From.Id, msg.ToString(), Telegram.Bot.Types.Enums.ParseMode.Markdown, false, false, e.Message.MessageId);
            }
            else if(e.Message.Text.StartsWith("/email"))
            {
                var msg = "Type your email to find are your hacked";
                BotClient.Instance.SendTextMessageAsync(e.Message.From.Id, msg, Telegram.Bot.Types.Enums.ParseMode.Markdown, false, false, e.Message.MessageId);
            }
        }
    }
}
