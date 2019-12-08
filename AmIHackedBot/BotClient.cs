using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Telegram.Bot;

namespace AmIHackedBot
{
    public sealed class BotClient : TelegramBotClient
    {
        private static readonly Lazy<BotClient> lazy =
            new Lazy<BotClient>(() => new BotClient(File.ReadAllText(Path.Combine(StaticUtils.GetExecutingDirectoryName(), "Config.txt"))));

        public static BotClient Instance { get { return lazy.Value; } }

        private BotClient(string token)
        : base(token)
        {
        }

        
    }
}
