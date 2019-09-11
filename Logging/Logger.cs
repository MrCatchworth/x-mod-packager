using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.IO;

namespace XModPackager.Logging
{
    public static class Logger
    {
        public static LogCategory MinLogLevel {get; set;} = LogCategory.Info;

        private static readonly Regex LogHeaderPositionRegex = new Regex("^|\n|\r\n|\r");

        private static readonly Dictionary<LogCategory, string> LogHeaders = new Dictionary<LogCategory, string>() {
            [LogCategory.Debug] = "Debug",
            [LogCategory.Info] = "Info",
            [LogCategory.Warning] = "WARNING",
            [LogCategory.Error] = "ERROR",
            [LogCategory.Fatal] = "FATAL",
        };

        public static readonly List<TextWriter> OutputStreams = new List<TextWriter>() {
            Console.Out
        };

        private static bool ShouldLog(LogCategory level)
        {
            return level >= MinLogLevel;
        }

        private static void WriteToStreams(string message)
        {
            foreach (var stream in OutputStreams)
            {
                stream.WriteLine(message);
            }
        }

        public static string AppendLogHeaders(LogCategory level, string message)
        {
            var headerText = "[" + LogHeaders[level] + "] ";

            return LogHeaderPositionRegex.Replace(message, match => match + headerText);
        }

        public static void Log(LogCategory level, string message)
        {
            if (!ShouldLog(level)) return;

            WriteToStreams(AppendLogHeaders(level, message));
        }
    }
}