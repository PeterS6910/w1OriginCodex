using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contal.Cgp.Globals.CrossPlatform
{
    /// <summary>
    /// Provides a very small helper for writing diagnostic messages to a log file.
    /// The implementation intentionally avoids any external dependencies so it can
    /// be used from legacy code paths where only quick error tracing is required.
    /// </summary>
    public static class FileLogger
    {
        private static readonly object SyncRoot = new object();
        private static string _logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");

        /// <summary>
        /// Gets or sets the directory where log files are written. When not specified,
        /// a "Logs" folder next to the application binaries is used.
        /// </summary>
        public static string LogDirectory
        {
            get => _logDirectory;
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _logDirectory = value;
                }
            }
        }

        /// <summary>
        /// Writes an exception to the log, including an optional context message.
        /// Log entries are grouped by day and appended to the same file for easy review.
        /// </summary>
        public static void LogException(Exception exception, string context = null)
        {
            if (exception == null)
            {
                return;
            }

            WriteEntry(BuildExceptionMessage(exception, context));
        }

        /// <summary>
        /// Writes an arbitrary diagnostic message to the log.
        /// </summary>
        public static void LogMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            WriteEntry(FormatEntry(message) + Environment.NewLine);
        }

        private static void WriteEntry(string entry)
        {
            try
            {
                string directory = _logDirectory;
                Directory.CreateDirectory(directory);

                string filePath = Path.Combine(directory, $"{DateTime.UtcNow:yyyyMMdd}.log");

                lock (SyncRoot)
                {
                    File.AppendAllText(filePath, entry, Encoding.UTF8);
                }
            }
            catch
            {
                // Never allow logging failures to crash the application.
            }
        }

        private static string BuildExceptionMessage(Exception exception, string context)
        {
            StringBuilder builder = new StringBuilder();
            string header = string.IsNullOrWhiteSpace(context) ? "Unhandled exception" : context;
            builder.AppendLine(FormatEntry(header));
            builder.AppendLine(exception.ToString());
            builder.AppendLine(new string('-', 80));
            return builder.ToString();
        }

        private static string FormatEntry(string message)
        {
            string text = string.IsNullOrWhiteSpace(message) ? "(no context)" : message;
            return $"[{DateTime.UtcNow:O}] {text}";
        }
    }
}
