using System.IO;
using System.Reflection;

namespace AmIHackedBot.Emails
{
    /// <summary>
    /// Shared data
    /// </summary>
    internal static class SharedData
    {
        /// <summary>
        /// Emails files directory
        /// </summary>
        public static string EmailsDirectory { get; set; }

        static SharedData()
        {
            EmailsDirectory = $"{Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)}\\Emails";
            if (!Directory.Exists(EmailsDirectory))
                Directory.CreateDirectory(EmailsDirectory);
        }
    }
}