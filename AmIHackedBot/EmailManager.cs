using AmIHackedBot.Emails;
using AmIHackedBot.Sharp;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace AmIHackedBot
{
    

    /// <summary>
    /// email manager
    /// </summary>
    public class EmailManager
    {
        /// <summary>
        /// store collections of subscribe emails by telegramId
        /// </summary>
        public ConcurrentDictionary<long, List<Email>> TelegramIdToEmailDict;

        public EmailManager()
        {
            TelegramIdToEmailDict = new ConcurrentDictionary<long, List<Email>>();
            var allFiles = Directory.GetFiles(SharedData.EmailsDirectory);
            if (allFiles != null)
            {
                var sw = Stopwatch.StartNew();
                foreach (var file in allFiles)
                {
                    var user = JsonConvert.DeserializeObject<User>(File.ReadAllText(file));
                    if (user != null)
                    {
                        TelegramIdToEmailDict.TryAdd(user.TelegramId, user.Emails);
                        StaticUtils.Logger.LogInformation($"Загрузили файл для игрока: {user.TelegramId}");                       
                    }
                }

                sw.Stop();
                StaticUtils.Logger.LogInformation($"Load users from DB:`{TelegramIdToEmailDict.Keys.Count}` : `{sw.ElapsedMilliseconds}` мс");
            }
        }

        /// <summary>
        /// Add email to subscribe list
        /// </summary>
        /// <param name="email">email</param>
        /// <param name="telegramId">Telegram id</param>
        public void AddOrUpdateEmail(int telegramId, Email email)
        {
            List<Email> emailColl = null;
            if (!TelegramIdToEmailDict.TryGetValue(telegramId, out emailColl))
            {
                emailColl = new List<Email>();
                emailColl.Add(email);
                TelegramIdToEmailDict[telegramId] = emailColl;
                return;
            }

            if (emailColl == null)
                emailColl = new List<Email>();

            emailColl.Add(email);
            TelegramIdToEmailDict[telegramId] = emailColl;
        }

        private Email AddUpdate(int arg1, Email arg2)
        {
            return arg2;
        }

        /// <summary>
        /// remove email to subscribe list
        /// </summary>
        /// <param name="email">email</param>
        public void RemoveEmail(int telegramId, Email email)
        {
            List<Email> emailColl = null;
            if (!TelegramIdToEmailDict.TryGetValue(telegramId, out emailColl))
            {
                return;
            }

            if (emailColl == null)
                return;
            emailColl.Remove(email);
            TelegramIdToEmailDict[telegramId] = emailColl;
        }

        /// <summary>
        /// Get all emails by telegram id
        /// </summary>
        /// <param name="telegramId">telegram id</param>
        /// <returns>collection with emails</returns>
        public IEnumerable<Email> GetEmails(int telegramId)
        {
            List<Email> emailsColl = null;
            TelegramIdToEmailDict.TryGetValue(telegramId, out emailsColl);

            if (emailsColl == null)
                emailsColl = new List<Email>();
            return emailsColl;
        }

        
    }
}

