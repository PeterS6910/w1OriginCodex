using Contal.IwQuick;
using System;

namespace Contal.IwQuick.Data
{
    public class StatusMessage
    {
        public StatusMessage(DateTime time, string source, string message, NotificationSeverity type = NotificationSeverity.Info)
        {
            this.Time = time;
            this.Source = source;
            this.Message = message;
            this.Type = type;
        }

        public StatusMessage(string source, string message, NotificationSeverity type = NotificationSeverity.Info)
            : this(DateTime.Now, source, message, type)
        {
        }

        public StatusMessage(string message)
            : this(DateTime.Now, String.Empty, message)
        {
        }

        public DateTime Time { get; private set; }

        public NotificationSeverity Type { get; private set; }

        public string Source { get; private set; }

        public string Message { get; private set; }

        public override string ToString()
        {
            return String.Format("{0} {1}: {2} - {3}", Time, Type, Source, Message);
        }
    }
}