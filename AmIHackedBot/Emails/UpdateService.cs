using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using AmIHackedBot.Sharp;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AmIHackedBot.Emails
{
    /// <summary>
    /// Update pwned data
    /// </summary>
    internal class UpdateService
    {
        /// <summary>
        /// observer
        /// </summary>
        private IObservable<long> _sub;
        /// <summary>
        /// email manager
        /// </summary>
        private EmailManager _emailManager;

        /// <summary>
        /// Client from API
        /// </summary>
        private HaveIBeenPwnedRestClient _client;
        /// <summary>
        /// Notification client
        /// </summary>
        private Notification _notification;

        public UpdateService(EmailManager emailManager)
        {
            _client = new HaveIBeenPwnedRestClient();
            _emailManager = emailManager;
            _notification = new Notification();
            _sub = Observable.Interval(TimeSpan.FromMinutes(1));//one request per day

        }

        /// <summary>
        /// Start service
        /// </summary>
        public void Start()
        {
            _sub.Subscribe(Update);
        }
        /// <summary>
        /// update
        /// </summary>
        /// <param name="task"></param>
        private void Update(long task)
        {
            Update();
        }
        /// <summary>
        /// update
        /// </summary>
        private void Update()
        {
            var sw = Stopwatch.StartNew();
            StaticUtils.Logger.LogInformation($"Start to update pwned emails");
            foreach (var telegramId in _emailManager.TelegramIdToEmailDict.Keys)
            {
                var userFile = Path.Combine($"{SharedData.EmailsDirectory}", $"{telegramId}.txt");
                var emailsColl = _emailManager.TelegramIdToEmailDict[telegramId];
                var removeEmails = new List<Email>();
                var addedEmails = new List<Email>();
                foreach (var email in emailsColl)
                {
                    try
                    {
                        StaticUtils.Logger.LogInformation($"Try to find email '{email.Name}' from Pwned databases");
                        var response = _client.GetAccountBreaches(email.Name).Result;
                        if (email.BreachColl.Count != response.Count)//need to send notification
                        {
                            _notification.SendNotification(telegramId, email.Name, email.BreachColl, response);
                            var newEmail = new Email(email.Name, response);
                            removeEmails.Add(email);
                            addedEmails.Add(newEmail);
                            Thread.Sleep(100);//wait for avoid ddos
                        }
                        StaticUtils.Logger.LogInformation($"Email '{email.Name}' Old breaches:'{email.BreachColl.Count}' New breaches:'{response.Count}'");
                    }
                    catch (Exception ex)
                    {
                        StaticUtils.Logger.LogError(ex, "Error while get result from API");
                    }
                }

                if (removeEmails.Any())
                {
                    foreach (var remEm in removeEmails)
                    {
                        _emailManager.TelegramIdToEmailDict[telegramId].Remove(remEm);
                    }
                }

                if (addedEmails.Any())
                {
                    foreach (var add in addedEmails)
                    {
                        _emailManager.TelegramIdToEmailDict[telegramId].Add(add);
                    }
                }

                using (StreamWriter file =
                     new StreamWriter($"{userFile}"))
                {
                    var forwardUser = new User(telegramId, emailsColl);
                    var userString = JsonConvert.SerializeObject(forwardUser);
                    if (userString != null)
                        file.WriteLine(userString);
                }


                sw.Stop();
                StaticUtils.Logger.LogInformation($"Finish to update pwned emails:'{sw.ElapsedMilliseconds}' ms");
            }
        }
    }
}
