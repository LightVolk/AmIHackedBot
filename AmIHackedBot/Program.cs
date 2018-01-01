using System;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Microsoft.Extensions.Logging;
using AmIHackedBot.Sharp;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace AmIHackedBot
{
    class Program
    {
        private static TelegramBotClient Bot = BotClient.Instance;
        public static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();
        private async Task MainAsync()
        {
            Bot.OnMessage += Bot_OnMessage;
            Console.WriteLine("Hello World!");
            Bot.StartReceiving();
            await Task.Delay(-1);
        }


        /// <summary>
        /// Handle messages from user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Bot_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            try
            {
                if (e.Message.Text.StartsWith("/start"))
                {
                    var msg = new StringBuilder();
                    msg.AppendLine("Welcome!");
                    msg.AppendLine("This bot works with www.haveibeenpwned.com");
                    msg.AppendLine("Type your email to find are your hacked");

                    BotClient.Instance.SendTextMessageAsync(e.Message.From.Id, msg.ToString(), Telegram.Bot.Types.Enums.ParseMode.Markdown, false, false, e.Message.MessageId);
                    return;
                }
                else if (e.Message.Text.StartsWith("/email"))
                {
                    var msg = "Type your email to find are your hacked";
                    BotClient.Instance.SendTextMessageAsync(e.Message.From.Id, msg, Telegram.Bot.Types.Enums.ParseMode.Markdown, false, false);
                    return;
                }

                if (string.IsNullOrEmpty(e.Message.Text))
                    return;

                var regexUtils = new RegexUtilities();
                if (!regexUtils.IsValidEmail(e.Message.Text))
                {
                    BotClient.Instance.SendTextMessageAsync(e.Message.From.Id, $"'{e.Message.Text}' is not valid email!", Telegram.Bot.Types.Enums.ParseMode.Markdown, false, false);
                    return;
                }

                var client = new HaveIBeenPwnedRestClient();
                var response = client.GetAccountBreaches(e.Message.Text).Result;

                var breachesColl = new Dictionary<DateTime, Breach>(response.Count);

                ; foreach (Breach x in response)
                {
                    try
                    {
                        breachesColl.Add(DateTime.Parse(x.BreachDate), x);
                    }
                    catch (Exception ex)
                    {
                        StaticUtils.Logger.LogError(ex, ex.Message);
                    }                    
                }

                var dates = breachesColl.OrderBy(x => x.Key).ToList();
                foreach(var date in dates)
                {
                    var msg = new StringBuilder();
                    msg.AppendLine($"Domain: {date.Value.Domain}");
                    msg.AppendLine($"Breached Data:{date.Key.ToShortDateString()}");
                    msg.AppendLine($"Description: {date.Value.Description}");

                    if (date.Value.DataClasses.Count != 0)
                    {
                        var dataClasses = new StringBuilder();
                        foreach (var dc in date.Value.DataClasses)
                        {
                            dataClasses.Append($"{dc}\t");
                        }
                        msg.AppendLine(dataClasses.ToString());
                    }
                    BotClient.Instance.SendTextMessageAsync(e.Message.From.Id, msg.ToString(), Telegram.Bot.Types.Enums.ParseMode.Html, false, false);
                    Thread.Sleep(100);
                }


                if (response.Count == 0)
                {
                    BotClient.Instance.SendTextMessageAsync(e.Message.From.Id, $"E-mail is clean!", Telegram.Bot.Types.Enums.ParseMode.Html, false, false);
                }
            }
            catch (Exception ex)
            {
                StaticUtils.Logger.LogError(ex, ex.Message);
            }
        }
    }
}
