using System;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Microsoft.Extensions.Logging;
using AmIHackedBot.Sharp;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AmIHackedBot.Emails;

namespace AmIHackedBot
{
    class Program
    {
        /// <summary>
        /// Bot client
        /// </summary>
        private static TelegramBotClient Bot = BotClient.Instance;

        private static EmailManager _emailManager;
        private static UpdateService _updateService;
        public static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();
        private async Task MainAsync()
        {
            _emailManager = new EmailManager(StaticUtils.Logger);
            _updateService = new UpdateService(_emailManager);
            Bot.OnMessage += Bot_OnMessage;
            _updateService.Start();
            Console.WriteLine("Start bot!");
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
               
                if (e.Message.Text.StartsWith("/start")||e.Message.Text.StartsWith(Commands.HELP))
                {
                    var msg = new StringBuilder();
                    msg.AppendLine("Welcome!");
                    msg.AppendLine("This bot works with www.haveibeenpwned.com");
                    msg.AppendLine("Enter your email address to see if you are hacked.");
                    msg.AppendLine("Commands:");
                    msg.AppendLine("start - start bot");
                    msg.AppendLine("add - add email to subscribe list");
                    msg.AppendLine("remove - remove email from subscribe list");
                    msg.AppendLine("showemails - show subscribed emails");
                    msg.AppendLine("--------------------------------------");
                    msg.AppendLine(@"Using this bot, you agree with the law on personal data of the Russian Federation.
The bot can store and process your email and your telegram id.
This data is only needed to notify you via telegrams and will not be used to send spam.");
                    msg.AppendLine("--------------------------------------");
                    msg.AppendLine("\n\n");
                    msg.AppendLine("--------------------------------------");
                    msg.AppendLine(@"Используя данный бот, вы соглашаетесь с законом о персональных данных Российской Федерации.
Бот может хранить и обрабатывать ваш емайл и ваш телеграм ид.
Эти данные нужны только для оповещения Вас через телеграм и не будут использоваться для рассылки спама.");
                    msg.AppendLine("--------------------------------------");
                    /*
                start - start bot
add - add email to subscribe list
remove - remove email from subscribe list
showemails - show subscribed emails
                 */
                    BotClient.Instance.SendTextMessageAsync(e.Message.From.Id, msg.ToString(), Telegram.Bot.Types.Enums.ParseMode.Markdown, false, false, e.Message.MessageId);
                    return;
                }
                else if (e.Message.Text.StartsWith("/email"))
                {
                    var msg = "Enter your email address to see if you are hacked.";
                    BotClient.Instance.SendTextMessageAsync(e.Message.From.Id, msg, Telegram.Bot.Types.Enums.ParseMode.Markdown, false, false);
                    return;
                }
                else if (e.Message.Text.StartsWith(Commands.ADD_EMAIL_COMMAND))
                {
                    var email = e.Message.Text.Remove(0, e.Message.Text.IndexOf(Commands.ADD_EMAIL_COMMAND) + Commands.ADD_EMAIL_COMMAND.Length).Trim();
                    if (!string.IsNullOrEmpty(email))
                    {
                        var rgUtils = new RegexUtilities();
                        if (!rgUtils.IsValidEmail(email))
                        {
                            BotClient.Instance.SendTextMessageAsync(e.Message.From.Id, $"'{email}' is not valid email!", Telegram.Bot.Types.Enums.ParseMode.Markdown, false, false);
                            return;
                        }
                        var client = new HaveIBeenPwnedRestClient();
                        var response = client.GetAccountBreaches(email).Result;
                        var emailObj = new Email(email, response);
                        _emailManager.AddOrUpdateEmail(e.Message.From.Id, emailObj);
                        BotClient.Instance.SendTextMessageAsync(e.Message.From.Id, $"Add email '{email}' to subscribe list", Telegram.Bot.Types.Enums.ParseMode.Markdown, false, false);
                        SendBreaches(e.Message.From.Id, response);
                    }
                    return;
                }
                else if (e.Message.Text.StartsWith(Commands.REMOVE_EMAIL_COMMAND))
                {
                    var email = e.Message.Text.Remove(0, 
                        e.Message.Text.IndexOf(Commands.REMOVE_EMAIL_COMMAND) + Commands.REMOVE_EMAIL_COMMAND.Length).Trim();
                    if (!string.IsNullOrEmpty(email))
                    {
                        var client = new HaveIBeenPwnedRestClient();
                        var response = client.GetAccountBreaches(email).Result;
                        var emailObj = new Email(email, response);
                        _emailManager.RemoveEmail(e.Message.From.Id, emailObj);
                        BotClient.Instance.SendTextMessageAsync(e.Message.From.Id, $"Remove email '{email}' from subscribe list", Telegram.Bot.Types.Enums.ParseMode.Markdown, false, false);
                        return;
                    }                    
                    BotClient.Instance.SendTextMessageAsync(e.Message.From.Id, $"Can not remove email  from subscribe list", Telegram.Bot.Types.Enums.ParseMode.Markdown, false, false);
                    return;
                }
                else if (e.Message.Text.StartsWith(Commands.SHOW_EMAIL_LIST))
                {
                    var emailList = _emailManager.GetEmails(e.Message.From.Id);
                    BotClient.Instance.SendTextMessageAsync(e.Message.From.Id, $"Your subscribe list:", Telegram.Bot.Types.Enums.ParseMode.Markdown, false, false);
                    Thread.Sleep(50);
                    foreach (var em in emailList)
                    {
                        BotClient.Instance.SendTextMessageAsync(e.Message.From.Id, em.Name, Telegram.Bot.Types.Enums.ParseMode.Markdown, false, false);
                        Thread.Sleep(50);
                    }
                    if(!emailList.Any())
                        BotClient.Instance.SendTextMessageAsync(e.Message.From.Id, "No emails in list yet", Telegram.Bot.Types.Enums.ParseMode.Markdown, false, false);
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

                SendBreaches(e);
            }
            catch (Exception ex)
            {
                StaticUtils.Logger.LogError(ex, ex.Message);
            }
        }

        /// <summary>
        /// send all breaches to client
        /// </summary>
        /// <param name="e">arg</param>
        private static void SendBreaches(Telegram.Bot.Args.MessageEventArgs e)
        {
            var client = new HaveIBeenPwnedRestClient();
            var response = client.GetAccountBreaches(e.Message.Text).Result;

            SendBreaches(e.Message.From.Id, response);
        }

        private static void SendBreaches(long telegramId, List<Breach> response)
        {
            var breachesColl = new Dictionary<DateTime, Breach>(response.Count);

            foreach (Breach x in response)
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
            foreach (var date in dates)
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
                BotClient.Instance.SendTextMessageAsync(telegramId, msg.ToString(), Telegram.Bot.Types.Enums.ParseMode.Html, false, false);
                Thread.Sleep(100);
            }


            if (response.Count == 0)
            {
                BotClient.Instance.SendTextMessageAsync(telegramId, $"E-mail is clean!", Telegram.Bot.Types.Enums.ParseMode.Html, false, false);
            }
        }
    }
}
