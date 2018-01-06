using AmIHackedBot.Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace AmIHackedBot
{
    /// <summary>
    /// Class of notification
    /// </summary>
    internal class Notification
    {
        /// <summary>
        /// Send notification to User
        /// </summary>
        /// <param name="telegramId">user's telegram id</param>
        /// <param name="oldBreaches">old breaches coll</param>
        /// <param name="newBreaches">new breaches coll</param>
        ///<param name="email">email</param>
        public void SendNotification(long telegramId,string email, List<Breach> oldBreaches, List<Breach> newBreaches)
        {
            var firstNotSecond = newBreaches.Except(oldBreaches).ToList();
            if (firstNotSecond.Any())
            {
                BotClient.Instance.SendTextMessageAsync(telegramId, $"Warning! Your mail  '{email}' has appeared in the bases of hackers!",
                    Telegram.Bot.Types.Enums.ParseMode.Markdown, false, false);
                foreach (var res in firstNotSecond)
                {
                    var msg = new StringBuilder();
                    msg.AppendLine($"Domain: {res.Domain}");
                    msg.AppendLine($"Breached Data:{res.BreachDate}");
                    msg.AppendLine($"Description: {res.Description}");
                    BotClient.Instance.SendTextMessageAsync(telegramId, $"{msg.ToString()}",
                    Telegram.Bot.Types.Enums.ParseMode.Markdown, false, false);
                    Thread.Sleep(100);
                }
            }
        }
    }
}
