using System;
using System.Diagnostics;

namespace Contal.IwQuick
{
    /// <summary>
    /// simplified logging class with several output backends
    /// </summary>
    public class Log
    {
        private static volatile Log _implicitInstance = null;
        private static readonly object _syncRoot = new object();

        /// <summary>
        /// 
        /// </summary>
        public static Log Implicit
        {
            get
            {
                if (null == _implicitInstance) // optimization only
                    lock (_syncRoot)    // atomicity
                    {
                        if (null == _implicitInstance)
                        {
                            string friendlyName = null;
                            try
                            {
                                friendlyName = AppDomain.CurrentDomain.FriendlyName;
                            }
                            catch
                            {
                                friendlyName = null;
                            }


                            _implicitInstance = new Log(friendlyName ?? "Log.NET", true, true, false);
                        }
                    }

                return _implicitInstance;
            }
        }

        private volatile bool _intoConsole = true;
        /// <summary>
        /// the message entry will be logged into the console; 
        /// default is true
        /// </summary>
        public bool IntoConsole
        {
            get { return _intoConsole; }
            set { _intoConsole = value; }
        }


        private volatile bool _intoEventLog = true;
        /// <summary>
        /// the message entry will be logged into the event log; 
        /// default is false
        /// </summary>
        public bool IntoEventLog
        {
            get { return _intoEventLog; }
            set { _intoEventLog = value; }
        }

        private bool _intoGUI = false;

        /// <summary>
        /// the message entry will be logged into the GUI; 
        /// default is false
        /// </summary>
        public bool IntoGUI
        {
            get { return _intoGUI; }
            set { _intoGUI = value; }
        }

        private volatile bool _prependTimeConsole = true;
        /// <summary>
        /// if the message entry will be logged into the console, the date/time will be prepended before message;
        /// default is true
        /// </summary>
        public bool PrependTimeConsole
        {
            get { return _prependTimeConsole; }
            set { _prependTimeConsole = value; }
        }

        private volatile bool _prependDateConsole = true;
        /// <summary>
        /// if the message entry will be logged into the console, the date/time will be prepended before message;
        /// default is true
        /// </summary>
        public bool PrependDateConsole
        {
            get { return _prependDateConsole; }
            set { _prependDateConsole = value; }
        }

        

        private volatile string _implicitSource = null;
        /// <summary>
        /// 
        /// </summary>
        public string ImplicitSource
        {
            get { return _implicitSource; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException();

                _implicitSource = value;
            }
        }


        /// <summary>
        /// implicit constructor; other properties can be left default or defined explicitly by properties
        /// </summary>
        public Log()
        {
        }

        /// <summary>
        /// explicit constructor with general source specification
        /// </summary>
        /// <param name="implicitSource"></param>
        public Log(string implicitSource)
        {
            Validator.CheckNullString(implicitSource);

            _implicitSource = implicitSource;
        }

        /// <summary>
        /// explicit constructor
        /// </summary>
        /// <param name="intoConsole">if true, every message entry will be implicitly shown on console</param>
        /// <param name="intoEventLog">if true, every message entry will be implicitly shown in event log</param>
        /// <param name="intoGUI">if true, every message entry will be implicitly shown as GUI dialog</param>
        public Log(bool intoConsole, bool intoEventLog, bool intoGUI)
        {
            _intoConsole = intoConsole;
            _intoEventLog = intoEventLog;
            _intoGUI = intoGUI;
        }

        /// <summary>
        /// explicit constructor
        /// </summary>
        /// <param name="implicitSource"></param>
        /// <param name="intoConsole">if true, every message entry will be implicitly shown on console</param>
        /// <param name="intoEventLog">if true, every message entry will be implicitly shown in event log</param>
        /// <param name="intoGUI">if true, every message entry will be implicitly shown as GUI dialog</param>
        public Log(string implicitSource, bool intoConsole, bool intoEventLog, bool intoGUI)
        {
            Validator.CheckNullString(implicitSource);

            _implicitSource = implicitSource;

            _intoConsole = intoConsole;
            _intoEventLog = intoEventLog;
            _intoGUI = intoGUI;
        }

        private EventLogEntryType ConvertNotificationSeverity(NotificationSeverity severity)
        {
            switch(severity) {
                case NotificationSeverity.Error:
                case NotificationSeverity.ErrorCritical:
                case NotificationSeverity.Failure:
                    return EventLogEntryType.Error;
                case NotificationSeverity.Info:
                case NotificationSeverity.Success:
                    return EventLogEntryType.Information;
                case NotificationSeverity.Warning:
                    return EventLogEntryType.Warning;
                default:
                    return EventLogEntryType.Error;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="implicitSource"></param>
        /// <param name="source"></param>
        /// <param name="entryType"></param>
        /// <param name="message"></param>
        /// <param name="explicitBackend"></param>
        /// <param name="intoConsole"></param>
        /// <param name="intoEventLog"></param>
        /// <param name="intoGUI"></param>
        /// <returns></returns>
        private bool Message(bool implicitSource, string source, NotificationSeverity entryType, string message, bool explicitBackend,bool intoConsole,bool intoEventLog,bool intoGUI)
        {
        
            if (Validator.IsNullString(message))
                return false;

            if (!implicitSource &&
                Validator.IsNullString(source))
                return false;

            if (implicitSource &&
                Validator.IsNullString(_implicitSource))
                return false;

            if (implicitSource && Validator.IsNullString(source))
                source = _implicitSource;


            try {            
                if ((explicitBackend && intoConsole) ||
                    (!explicitBackend && _intoConsole))
                {
                    string messageProjection = source + " : " + message;

                    if (_prependTimeConsole || _prependDateConsole)
                    {
                        DateTime dt = DateTime.Now;
                        string dtInfo = "[";
                        if (_prependDateConsole)
                        {
                            dtInfo += dt.ToString("dd.MM.yyy");
                            if (_prependTimeConsole)
                                dtInfo += StringConstants.SPACE;
                        }

                        if (_prependTimeConsole)
                            dtInfo += dt.ToString("HH:mm:ss.fff");

                        dtInfo += "] ";

                        messageProjection = dtInfo + messageProjection;
                    }

                    UI.ExtendedConsole.WriteLine(entryType,messageProjection);
                }
            }
            catch
            {
            }

            
            if ((explicitBackend && intoEventLog) ||
                (!explicitBackend && _intoEventLog))
                try
                {
                    EventLog.WriteEntry(source, message, ConvertNotificationSeverity(entryType));
                }
                catch
                {
                    return false;
                }

            try
            {
                if ((explicitBackend && intoGUI) ||
                    (!explicitBackend && _intoGUI))
                    switch (entryType)
                    {
                        case NotificationSeverity.Error:
                        case NotificationSeverity.ErrorCritical:
                        case NotificationSeverity.Failure:
                            UI.Dialog.Error(source, message);
                            break;
                        case NotificationSeverity.Info:
                        case NotificationSeverity.Success:
                            UI.Dialog.Info(source, message);
                            break;
                        case NotificationSeverity.Warning:
                            UI.Dialog.Warning(source, message);
                            break;
                    }

            }
            catch
            {
            }

            return true;
            
        }

        /// <summary>
        /// message with explicit source specification
        /// </summary>
        /// <param name="source"></param>
        /// <param name="entryType"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool Message(string source, NotificationSeverity entryType, string message)
        {
            return Message(false, source, entryType, message, false, false, false, false);
        }

        public bool Message(NotificationSeverity entryType, string message)
        {
            return Message(true, null, entryType, message, false, false, false, false);
        }

        public bool Message(string source, NotificationSeverity entryType, string message, bool intoConsole, bool intoEventLog, bool intoGUI)
        {
            return Message(false, source, entryType, message, true, intoConsole,intoEventLog,intoGUI);
        }

        public bool Message(NotificationSeverity entryType, string message, bool intoConsole, bool intoEventLog, bool intoGUI)
        {
            return Message(true, null, entryType, message, true, intoConsole, intoEventLog, intoGUI);
        }

        public void Error(string source, string message)
        {
            Message(false, source, NotificationSeverity.Error, message, false, false, false, false);
        }

        public void Error(string message)
        {
            Message(true, null, NotificationSeverity.Error, message, false, false, false, false);
        }

        public void Error(string source, string message, bool intoConsole, bool intoEventLog, bool intoGUI)
        {
            Message(false, source, NotificationSeverity.Error, message, true, intoConsole, intoEventLog, intoGUI);
        }

        public void Error(string message, bool intoConsole, bool intoEventLog, bool intoGUI)
        {
            Message(true, null, NotificationSeverity.Error, message, true, intoConsole, intoEventLog, intoGUI);
        }


        public void Info(string source, string message)
        {
            Message(false, source, NotificationSeverity.Info, message, false, false, false, false);
        }

        public void Info(string message)
        {
            Message(true, null, NotificationSeverity.Info, message, false, false, false, false);
        }

        public void Info(string source, string message, bool intoConsole, bool intoEventLog, bool intoGUI)
        {
            Message(false, source, NotificationSeverity.Info, message, true, intoConsole, intoEventLog, intoGUI);
        }

        public void Info(string message, bool intoConsole, bool intoEventLog, bool intoGUI)
        {
            Message(true, null, NotificationSeverity.Info, message, true, intoConsole, intoEventLog, intoGUI);
        }

        public void Warning(string source, string message)
        {
            Message(false, source, NotificationSeverity.Warning, message, false, false, false, false);
        }

        public void Warning(string message)
        {
            Message(true, null, NotificationSeverity.Warning, message, false, false, false, false);
        }

        public void Warning(string source, string message, bool intoConsole, bool intoEventLog, bool intoGUI)
        {
            Message(false, source, NotificationSeverity.Warning, message, true, intoConsole, intoEventLog, intoGUI);
        }

        public void Warning(string message, bool intoConsole, bool intoEventLog, bool intoGUI)
        {
            Message(true, null, NotificationSeverity.Warning, message, true, intoConsole, intoEventLog, intoGUI);
        }
    }
}
