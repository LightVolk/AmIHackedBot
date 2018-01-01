using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reactive.Linq;
using System.Text;
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

        public UpdateService(EmailManager emailManager)
        {
            _sub = Observable.Interval(TimeSpan.FromDays(1));//one request per day
            _emailManager = emailManager;
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
                using (StreamWriter file =
       new StreamWriter($"{userFile}"))
                {
                    var user = new User(telegramId, _emailManager.TelegramIdToEmailDict[telegramId]);
                    var userString = JsonConvert.SerializeObject(user);
                    if (userString != null)
                        file.WriteLine(userString);
                }
            }
            sw.Stop();
            StaticUtils.Logger.LogInformation($"Finish to update pwned emails:'{sw.ElapsedMilliseconds}' ms");
        }
    }
}
