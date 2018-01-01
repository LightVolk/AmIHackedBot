using System;
using System.Collections.Generic;
using System.Text;

namespace AmIHackedBot.Emails
{
    public class User
    {
        /// <summary>
        /// telegram user id
        /// </summary>
        public long TelegramId { get; private set; }
        /// <summary>
        /// user emails
        /// </summary>
        public List<Email> Emails { get; private set; }

        public User(long telegramId,List<Email> emails)
        {
            TelegramId = telegramId;
            Emails = emails;
        }
    }
}
