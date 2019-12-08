using AmIHackedBot.Emails;
using AmIHackedBot.Sharp;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Collections;
using AmIHackedBot.Comparers;
using AmIHackedBot.Db;
using System.Text.Json;

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
        public ConcurrentDictionary<long, HashSet<Email>> TelegramIdToEmailDict;

        private DbManager _dbManager;
        private ILogger _logger;
        /// <summary>
        /// ctor
        /// </summary>
        public EmailManager(ILogger logger)
        {
            _logger = logger;
            TelegramIdToEmailDict = new ConcurrentDictionary<long, HashSet<Email>>();
            var allFiles = Directory.GetFiles(SharedData.EmailsDirectory);
            if (allFiles != null)
            {
                var sw = Stopwatch.StartNew();               
                foreach (var file in allFiles)
                {
                    var json = File.ReadAllText(file);
                    var loadedEmails = JsonSerializer.Deserialize<List<Email>>(json
                    );
                  
                    if (loadedEmails != null)
                    {
                        var telegramId = Int64.Parse(Path.GetFileNameWithoutExtension(file));
                        var emailsColl = new HashSet<Email>(new EmailEqualityComparer());
                        emailsColl.UnionWith(loadedEmails);
                        var user = new User(telegramId, emailsColl);
                        TelegramIdToEmailDict.TryAdd(telegramId, emailsColl);
                        StaticUtils.Logger.LogInformation($"Uploaded a file for the user: {telegramId}");
                    }
                }

                _dbManager = new DbManager(SharedData.EmailsDirectory,_logger);

                sw.Stop();
                StaticUtils.Logger.LogInformation($"Load users from DB:`{TelegramIdToEmailDict.Keys.Count}` : `{sw.ElapsedMilliseconds}` ms");
            }
        }

        /// <summary>
        /// Add email to subscribe list
        /// </summary>
        /// <param name="email">email</param>
        /// <param name="telegramId">Telegram id</param>
        public void AddOrUpdateEmail(int telegramId, Email email)
        {
            HashSet<Email> emailColl = null;
            if (!TelegramIdToEmailDict.TryGetValue(telegramId, out emailColl))
            {
                emailColl = new HashSet<Email>(new EmailEqualityComparer());
                emailColl.Add(email);
                TelegramIdToEmailDict[telegramId] = emailColl;
                _dbManager.AddOrUpdate(telegramId, emailColl);
                return;
            }

            if (emailColl == null)
                emailColl = new HashSet<Email>(new EmailEqualityComparer());
            if (!emailColl.Contains(email))
                emailColl.Add(email);
            TelegramIdToEmailDict[telegramId] = emailColl;

            _dbManager.AddOrUpdate(telegramId, emailColl);
        }


        /// <summary>
        /// remove email to subscribe list
        /// </summary>
        /// <param name="email">email</param>
        public void RemoveEmail(int telegramId, Email email)
        {
            HashSet<Email> emailColl = null;
            if (!TelegramIdToEmailDict.TryGetValue(telegramId, out emailColl))
            {
                return;
            }

            if (emailColl == null)
                return;
            emailColl.Remove(email);
            TelegramIdToEmailDict[telegramId] = emailColl;
            _dbManager.AddOrUpdate(telegramId, emailColl);
        }

        /// <summary>
        /// Get all emails by telegram id
        /// </summary>
        /// <param name="telegramId">telegram id</param>
        /// <returns>collection with emails</returns>
        public IEnumerable<Email> GetEmails(int telegramId)
        {
            HashSet<Email> emailsColl = null;
            TelegramIdToEmailDict.TryGetValue(telegramId, out emailsColl);

            if (emailsColl == null)
                emailsColl = new HashSet<Email>();
            return emailsColl;
        }


    }
   
}

