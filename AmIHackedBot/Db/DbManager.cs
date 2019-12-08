using AmIHackedBot.Emails;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace AmIHackedBot.Db
{
    /// <summary>
    /// manage work with db
    /// </summary>
    public class DbManager
    {
        private ILogger _logger;

        private string _emailDirectory;
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="logger"></param>
        public DbManager(String emailDirectory,ILogger logger)
        {
            _logger = logger;
            _emailDirectory = emailDirectory;
        }

        /// <summary>
        /// add or save into db
        /// </summary>
        /// <param name="id"></param>
        /// <param name="emails"></param>
        public void AddOrUpdate(long id,IEnumerable<Email> emails)
        {
            try
            {
                var json = JsonSerializer.Serialize(emails);
                File.WriteAllText(Path.Combine(_emailDirectory, $"{id}.txt"), json);
            }
            catch(Exception ex)
            {
                _logger.LogError($"{ex.Message} {ex.StackTrace}");
            }
        }
    }
}
