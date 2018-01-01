using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AmIHackedBot
{
    public static class StaticUtils
    {
        private static ILoggerFactory LoggerFactory;

        public static ILogger Logger;

        static StaticUtils()
        {
            var loggerPath = Path.Combine(Directory.GetCurrentDirectory(), "logger.txt");
            LoggerFactory = new LoggerFactory();
            LoggerFactory.AddFile(loggerPath);            
            Logger = StaticUtils.LoggerFactory.CreateLogger<Program>();
        }
    }
}
