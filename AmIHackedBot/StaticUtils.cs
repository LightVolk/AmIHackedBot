using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AmIHackedBot
{
    /// <summary>
    /// static utils
    /// </summary>
    public static class StaticUtils
    {
        /// <summary>
        /// logger factory
        /// </summary>
        private static ILoggerFactory LoggerFactory;
        /// <summary>
        /// logger
        /// </summary>
        public static ILogger Logger;
        /// <summary>
        /// constructor
        /// </summary>
        static StaticUtils()
        {
            var loggerPath = Path.Combine(Directory.GetCurrentDirectory(), "logger.txt");
            LoggerFactory = new LoggerFactory();
            LoggerFactory.AddFile(loggerPath);           
            Logger = StaticUtils.LoggerFactory.CreateLogger<Program>();
        }
    }
}
