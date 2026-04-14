using Contal.IwQuick.Data;
using System;

namespace Contal.IwQuick.UI
{
    public interface IUiStatusReport
    {
        void AddStatusMessage(StatusMessage data);

        void AddStatusMessage(string message);

        void SetVisible(bool visible);
    }

    public static class StatusReport
    {
        public static string AppName = "Contal Nova";

        private static IUiStatusReport reporter;

        public static void Register(IUiStatusReport aReporter)
        {
            reporter = aReporter;

            if (reporter != null)
                reporter.SetVisible(false);
        }

        public static void Info(string message)
        {
            if (reporter != null)
                reporter.AddStatusMessage(message);
        }

        public static void Info(string source, string message)
        {
            if (reporter != null)
                reporter.AddStatusMessage(new StatusMessage(source, message));
        }

        public static void Warning(string message)
        {
            if (reporter != null)
                reporter.AddStatusMessage(new StatusMessage(AppName, message, NotificationSeverity.Warning));
        }

        public static void Warning(string source, string message)
        {
            if (reporter != null)
                reporter.AddStatusMessage(new StatusMessage(source, message, NotificationSeverity.Warning));
        }

        public static void Error(string message)
        {
            if (reporter != null)
                reporter.AddStatusMessage(new StatusMessage(AppName, message, NotificationSeverity.Error));
        }

        public static void Error(string source, string message)
        {
            if (reporter != null)
                reporter.AddStatusMessage(new StatusMessage(source, message, NotificationSeverity.Error));
        }

        public static void Error(Exception exception)
        {
            if (reporter != null)
                reporter.AddStatusMessage(new StatusMessage(AppName, exception.Message, NotificationSeverity.ErrorCritical));
        }

        public static void SetVisible(bool visible)
        {
            if (reporter != null)
                reporter.SetVisible(visible);
        }
    }
}

