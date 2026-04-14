using System;
using System.Diagnostics;
using System.Text;
using System.Net;
using System.Threading;
using Contal.IwQuick.CrossPlatform.Data;
using Contal.IwQuick.CrossPlatform.Threads;
using Contal.IwQuick.Net;
using Contal.IwQuick.Data;
using System.Collections;
using System.Collections.Generic;
using Contal.IwQuick.Sys;

namespace Contal.IwQuick
{
    /// <summary>
    /// simplified logging class with several output backends
    /// </summary>
    public class Log
    {
        private static volatile Log _singleton = null;
        private static readonly object _syncRoot = new object();

        /// <summary>
        /// 
        /// </summary>
        public static Log Singleton
        {
            get
            {
                if (_singleton == null) // optimisation
                    lock (_syncRoot)    // atomicity
                    {
                        if (_singleton == null)
                            _singleton = new Log("Log.CF", true, false, null, 0);
                    }

                return _singleton;
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


        private volatile bool _intoDebugConsole = false;
        /// <summary>
        /// the message entry will be logged into the event log; 
        /// default is false
        /// </summary>
        public bool IntoDebugConsole
        {
            get { return _intoDebugConsole; }
            set { _intoDebugConsole = value; }
        }

        private volatile bool _showThreadId = false;
        /// <summary>
        /// the message entry will be logged into the event log; 
        /// default is false
        /// </summary>
        public bool ShowThreadId
        {
            get { return _showThreadId; }
            set { _showThreadId = value; }
        }

        private volatile Log _intoNestedLog = null;

        /// <summary>
        /// Log into which this Log instance can post all of the messages
        /// </summary>
        public Log NestedLog
        {
            get { return _intoNestedLog; }
            set
            {
                // cyrcling not allowed
                if (value == this ||
                    (value != null && value.NestedLog == this))
                    _intoNestedLog = null;
                else
                    _intoNestedLog = value;
            }
        }

        private volatile bool _reevaluateLevelInNestedLog = true;

        /// <summary>
        /// if true (by default), the nested log (if set) reevaluates the level of the message
        /// even though already evaluted at the level of the first log
        /// </summary>
        public bool ReevaluateLevelInNestedLog
        {
            get { return _reevaluateLevelInNestedLog; }
            set { _reevaluateLevelInNestedLog = value; }
        }

        private volatile bool _reevaluateMessageLambdaInNestedLog = true;

        /// <summary>
        /// if true (by default), the nested log (if set) reevalutes the message by the lambda delegate
        /// </summary>
        public bool ReevaluateMessageLambdaInNestedLog
        {
            get { return _reevaluateMessageLambdaInNestedLog; }
            set { _reevaluateMessageLambdaInNestedLog = value; }
        }


        private volatile IPEndPoint _udpDestination = null;

        /// <summary>
        /// the message entry will be logged into the event log; 
        /// default is false
        /// </summary>
        public bool IntoUDP
        {
            get { return _udpDestination != null; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public void SetIntoUDP(string ip, int port)
        {
            if (null == ip)
                _udpDestination = null;
            else
            {
                IPAddress ipAddress = IPHelper.CheckValidity(ip);

                TcpUdpPort.CheckValidity(port);
                _udpDestination = new IPEndPoint(ipAddress, port);
            }
        }

        private volatile RotaryFileLog _intoRotaryFileLog = null;

        /// <summary>
        /// if true, logging via rotary file log is enabled
        /// </summary>
        public bool IntoFileLog
        {
            get { return _intoRotaryFileLog != null; }
        }

        /// <summary>
        /// enables logging via rotary file log instance class
        /// </summary>
        /// <param name="rotaryFileLog">if null, disables logging by this backend</param>
        public void SetIntoFileLog(RotaryFileLog rotaryFileLog)
        {
            _intoRotaryFileLog = rotaryFileLog;
        }

        /// <summary>
        /// 
        /// </summary>
        public const byte HIGHEST_LEVEL = 255;
        /// <summary>
        /// 
        /// </summary>
        public const byte DEBUG_LEVEL = 224;
        /// <summary>
        /// 
        /// </summary>
        public const byte HIGH_LEVEL = 192;
        /// <summary>
        /// 
        /// </summary>
        public const byte ABOVE_NORMAL_LEVEL = 160;
        /// <summary>
        /// 
        /// </summary>
        public const byte NORMAL_LEVEL = 128;
        /// <summary>
        /// 
        /// </summary>
        public const byte BELOW_NORMAL_LEVEL = 96;
        /// <summary>
        /// 
        /// </summary>
        public const byte LOW_LEVEL = 64;
        /// <summary>
        /// 
        /// </summary>
        public const byte PERFORMANCE_LEVEL = 32;
        /// <summary>
        /// 
        /// </summary>
        public const byte CALM_LEVEL = 0;

        // SB
#if DEBUG
        private volatile byte _verbosityLevel = NORMAL_LEVEL;
        //private volatile byte _verbosityLevel = HIGHEST_LEVEL;
#else
        private volatile byte _verbosityLevel = NORMAL_LEVEL;
#endif

        /// <summary>
        /// defines trigger for messages with defined verbosity level ; 
        /// only messages with levelOfMessage lower or equal than this property will be logged
        /// </summary>
        public byte VerbosityLevel
        {
            get { return _verbosityLevel; }
            set
            {
                //#if !DEBUG
                _verbosityLevel = value;
                //#endif
            }
        }

        private readonly object _verbosityLevelMapSync = new object();
        private volatile Dictionary<byte, byte> _verbosityLevelMap = new Dictionary<byte, byte>(16);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="verbosityLevels"></param>
        public void SetVerbosityLevelMap(params byte[] verbosityLevels)
        {
            lock (_verbosityLevelMapSync)
            {
                _verbosityLevelMap.Clear();

                if (verbosityLevels == null) return;
                foreach (byte verbosityLevel in verbosityLevels)
                    _verbosityLevelMap[verbosityLevel] = 1;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="verbosityLevel"></param>
        /// <param name="enable"></param>
        public void AugmentVerbosityLevelMap(byte verbosityLevel, bool enable)
        {
            if (enable)
                lock (_verbosityLevelMapSync)
                {
                    _verbosityLevelMap[verbosityLevel] = 1;
                }
            else
                lock (_verbosityLevelMapSync)
                {
                    _verbosityLevelMap.Remove(verbosityLevel);
                }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public byte[] GetVerbosityLevelMap()
        {
            byte[] map;

            lock (_verbosityLevelMapSync)
            {
                map = new byte[_verbosityLevelMap.Count];

                _verbosityLevelMap.Keys.CopyTo(map, 0);
            }

            return map;
        }


        private volatile VerbosityLevelCondition _verbosityLevelCondition = VerbosityLevelCondition.LowerOrEqual;
        /// <summary>
        /// 
        /// </summary>
        public VerbosityLevelCondition VerbosityLevelMatchingCondition
        {
            get { return _verbosityLevelCondition; }
            set { _verbosityLevelCondition = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public enum VerbosityLevelCondition
        {
            /// <summary>
            /// 
            /// </summary>
            LowerOrEqual,
            /// <summary>
            /// 
            /// </summary>
            Precise,
            /// <summary>
            /// 
            /// </summary>
            HigherOrEqual,
            /// <summary>
            /// 
            /// </summary>
            BitMatch,
            /// <summary>
            /// 
            /// </summary>
            LevelMapMatch
        }


        private bool _prependTimeConsole = true;
        /// <summary>
        /// if the message entry will be logged into the console, the date/time will be prepended before message;
        /// default is true
        /// </summary>
        public bool PrependTimeConsole
        {
            get { return _prependTimeConsole; }
            set { _prependTimeConsole = value; }
        }



        private string _implicitSource = null;
        /// <summary>
        /// 
        /// </summary>
        public string ImplicitSource
        {
            get { return _implicitSource; }
            set
            {
                Validator.CheckNullString(value);

                _implicitSource = value;
            }
        }


        private readonly ThreadPoolQueueWithLimit<LogMessageCarrier, Log> _processingQueueForLines;

        private const int DEFAULT_QUEUE_LIMIT = 2000;



        /// <summary>
        /// implicit constructor; other properties can be left default or defined explicitly by properties
        /// </summary>
        private Log()
        {
            _processingQueueForLines =
                new ThreadPoolQueueWithLimit<LogMessageCarrier, Log>(
                    ThreadPoolGetter.Get(),
                    DEFAULT_QUEUE_LIMIT,
                    this);
        }

        /// <summary>
        /// explicit constructor with general source specification
        /// </summary>
        /// <param name="implicitSource"></param>
        public Log(string implicitSource)
            : this()
        {
            Validator.CheckNullString(implicitSource);

            _implicitSource = implicitSource;
        }

        /// <summary>
        /// explicit constructor
        /// </summary>
        /// <param name="intoConsole"></param>
        /// <param name="intoDebugConsole"></param>
        /// <param name="intoUdpIp"></param>
        /// <param name="intoUdpPort"></param>
        public Log(bool intoConsole, bool intoDebugConsole, string intoUdpIp, int intoUdpPort)
            : this()
        {
            _intoConsole = intoConsole;
            _intoDebugConsole = intoDebugConsole;
            SetIntoUDP(intoUdpIp, intoUdpPort);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="implicitSource"></param>
        /// <param name="intoConsole"></param>
        /// <param name="intoDebugConsole"></param>
        /// <param name="intoUdpIp"></param>
        /// <param name="intoUdpPort"></param>
        public Log(string implicitSource, bool intoConsole, bool intoDebugConsole, string intoUdpIp, int intoUdpPort)
            : this()
        {
            Validator.CheckNullString(implicitSource);

            _implicitSource = implicitSource;

            _intoConsole = intoConsole;
            _intoDebugConsole = intoDebugConsole;
            SetIntoUDP(intoUdpIp, intoUdpPort);
        }


        private SimpleUdpPeer _udpPeer = null;

        private bool EnsureUdpPeer()
        {
            if (null == _udpPeer)
            {
                _udpPeer = new SimpleUdpPeer(false);
                try
                {
                    _udpPeer.Start();
                }
                catch
                {
                    _udpPeer = null;
                    return false;
                }
            }

            return true;

        }

        private class LogMessageCarrier :
               IProcessingQueueRequest<Log>
        {
            public string _source;
            public NotificationSeverity _severity;
            public string _message;
            public DateTime _dateTime = DateTime.Now;
            public bool _intoConsoleExplicitly;
            public bool _intoDebugConsoleExplicitly;
            public IPEndPoint _intoUdpDestination;
            public int _callerThreadId;
            public int _levelOfMessage;

            /// <summary>
            /// performing the actual logging via processing queue
            /// </summary>  
            public void Execute(Log log)
            {
                if (!log.ValidateVerbosityLevelCompliance(_levelOfMessage))
                    return;

                var messageProjection = CreateMessageProjection(
                    _dateTime, 
                    _source, 
                    _message, 
                    _severity, 
                    _callerThreadId, 
                    log._prependTimeConsole, 
                    log._showThreadId);

                try
                {
                    if (log._udpDestination != null)
                    {
                        if (log.EnsureUdpPeer())
                        {
                            try
                            {
                                log._udpPeer.Send
                                (
                                    log._udpDestination,
                                    new ByteDataCarrier(messageProjection + StringConstants.CR_LF)
                                );
                            }
                            catch
                            {
                            }
                        }
                    }
                }
                catch
                {
                }

                try
                {
                    if (_intoUdpDestination != null)
                    {
                        try
                        {
                            if (log.EnsureUdpPeer())
                            {
                                try
                                {
                                    log._udpPeer.Send(
                                        _intoUdpDestination,
                                        new ByteDataCarrier(messageProjection + StringConstants.CR_LF));
                                }
                                catch
                                {
                                }
                            }
                        }
                        catch
                        {
                        }
                    }
                }
                catch
                {
                }

                try
                {
                    if (log._intoConsole || _intoConsoleExplicitly)
                    {
                        lock (log._consoleLocker) // avoid interleaving
                        {
                            Console.WriteLine(messageProjection);
                        }
                    }
                }
                catch
                {
                }

                try
                {
                    if (_intoDebugConsoleExplicitly || log._intoDebugConsole)
                    {
                        lock (log._consoleLocker)
                        {
                            Debug.WriteLine(messageProjection);
                        }
                    }
                }
                catch
                {
                }

                if (log._intoRotaryFileLog != null)
                    try
                    {
                        log._intoRotaryFileLog.WriteLine(messageProjection);
                    }
                    catch
                    {
                        // NullReferenceException can be produced if somebody executes in "proper" time
                    }
            }

            internal static string CreateMessageProjection(
                DateTime dateTime,
                string source,
                string message,
                NotificationSeverity severity,
                int callerThreadId,
                bool prependTimeConsole,
                bool showThreadId)
            {
                string messageProjection;

                string severityPrefix;

                switch (severity)
                {
                    case NotificationSeverity.Error:
                    case NotificationSeverity.Failure:
                    case NotificationSeverity.ErrorCritical:
                        severityPrefix = "ERROR   ";
                        break;
                    case NotificationSeverity.Warning:
                        severityPrefix = "WARNING ";
                        break;
                    default:
                        severityPrefix = String.Empty;
                        break;
                }

                if (Validator.IsNullString(source))
                    messageProjection = message;
                else
                    messageProjection = source + " : " + message;

                if (prependTimeConsole)
                    messageProjection = "[" + dateTime.ToString("dd.MM. HH:mm:ss.fff") + "] " + messageProjection;

                messageProjection = severityPrefix + messageProjection;

                if (showThreadId)
                    try
                    {
                        messageProjection = string.Format("{0:X} ", callerThreadId) + messageProjection;
                    }
                    catch
                    {
                    }
                return messageProjection;
            }

            public void OnError(Log param, Exception error)
            {
            }
        }

        private const string UNKNOWN_TAG = "UNKNOWN";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="levelOfMessage"></param>
        /// <param name="implicitSource"></param>
        /// <param name="source"></param>
        /// <param name="severity"></param>
        /// <param name="message"></param>
        /// <param name="messageLambdaDelegate"></param>
        /// <param name="intoConsoleExplicitly"></param>
        /// <param name="intoDebugConsoleExplicitly"></param>
        /// <param name="intoUdpDestination"></param>
        /// <returns></returns>
        private bool MessageCore(
            int levelOfMessage,
            bool implicitSource,
            string source,
            NotificationSeverity severity,
            string message,
            DVoid2String messageLambdaDelegate,
            bool intoConsoleExplicitly,
            bool intoDebugConsoleExplicitly,
            IPEndPoint intoUdpDestination)
        {
            if (!_enableEnqueue)
                return false;

            // verified at two occassions, at synchronous processing to lower amount of messageLambdaDelegate executions
            // and the within LogMessageCarrier.Execute to flush all enqueued messages if verbosity lowered
            if (!ValidateVerbosityLevelCompliance(levelOfMessage))
                return false;

            string originalMessage = message;
            if (message == null && messageLambdaDelegate != null)
                try
                {
                    message = messageLambdaDelegate();
                }
                catch
                {
                    return false;
                }

            var dateTime = DateTime.Now;

            if (Validator.IsNullString(message))
                message = UNKNOWN_TAG;

            source = FillSource(implicitSource, source);

            MessageCoreSynchronous(
                levelOfMessage, 
                source, 
                severity, 
                message, 
                messageLambdaDelegate, 
                intoConsoleExplicitly, 
                intoDebugConsoleExplicitly, 
                intoUdpDestination, 
                originalMessage, 
                dateTime);

            // conditioning moved, because everything can be already disabled, but nested log enabled
            if (!_intoConsole &&
                !_intoDebugConsole &&
                _udpDestination == null &&
                !intoConsoleExplicitly &&
                !intoDebugConsoleExplicitly &&
                null == intoUdpDestination)
                return false;

            var logMessageCarrier = new LogMessageCarrier
            {
                _levelOfMessage = levelOfMessage,
                _intoConsoleExplicitly = intoConsoleExplicitly,
                _intoDebugConsoleExplicitly = intoDebugConsoleExplicitly,
                _message = message,
                _dateTime = dateTime,
                _severity = severity,
                _source = source,
                _intoUdpDestination = intoUdpDestination,
                _callerThreadId = Thread.CurrentThread.ManagedThreadId
            };

            _processingQueueForLines.Enqueue(logMessageCarrier);

            return true;
        }

        private void MessageCoreSynchronous(int levelOfMessage, string source, NotificationSeverity severity, string message,
            DVoid2String messageLambdaDelegate, bool intoConsoleExplicitly, bool intoDebugConsoleExplicitly,
            IPEndPoint intoUdpDestination, string originalMessage, DateTime dateTime)
        {
            if (_intoNestedLog != null)
                try
                {
                    _intoNestedLog.MessageCore(
                        // verbosity already evaluated here, so not posted to the nested Log
                        _reevaluateLevelInNestedLog ? levelOfMessage : -1,
                        // source already evaluated by implicitSource bool
                        false, source,
                        severity,
                        _reevaluateMessageLambdaInNestedLog ? originalMessage : message,
                        //no need to reevaluate lambda, should be already part of the message string
                        _reevaluateMessageLambdaInNestedLog ? messageLambdaDelegate : null,
                        intoConsoleExplicitly, intoDebugConsoleExplicitly, intoUdpDestination);
                }
                catch
                {
                }

#if DEBUG
            // Redirect to Visual Studio Output window
            if (intoDebugConsoleExplicitly || _intoDebugConsole)
                Trace.WriteLine(">> " + LogMessageCarrier.CreateMessageProjection(dateTime, source, message, severity,
                    Thread.CurrentThread.ManagedThreadId, _prependTimeConsole, _showThreadId));
#endif
        }

        private string FillSource(bool implicitSource, string source)
        {
            if (!implicitSource &&
                Validator.IsNullString(source))
                source = UNKNOWN_TAG;
            else
            {
                if (implicitSource &&
                    Validator.IsNullString(_implicitSource))
                    source = UNKNOWN_TAG;
            }

            if (implicitSource && Validator.IsNullString(source))
                source = _implicitSource;
            return source;
        }

        private bool ValidateVerbosityLevelCompliance(int levelOfMessage)
        {
            if (levelOfMessage >= 0)
            {
                switch (_verbosityLevelCondition)
                {
                    case VerbosityLevelCondition.LowerOrEqual:
                        if (levelOfMessage > _verbosityLevel)
                            return false;
                        break;
                    case VerbosityLevelCondition.Precise:
                        if (levelOfMessage != _verbosityLevel)
                            return false;
                        break;
                    case VerbosityLevelCondition.HigherOrEqual:
                        if (levelOfMessage < _verbosityLevel)
                            return false;
                        break;
                    case VerbosityLevelCondition.BitMatch:
                        if ((levelOfMessage & _verbosityLevel) != levelOfMessage)
                            return false;

                        break;
                    case VerbosityLevelCondition.LevelMapMatch:
                        bool found;
                        lock (_verbosityLevelMapSync)
                        {
                            found = _verbosityLevelMap.ContainsKey((byte)levelOfMessage);
                        }

                        if (!found)
                            return false;

                        break;
                }
            }
            return true;
        }

        private readonly object _consoleLocker = new object();




        #region General messages

        /// <summary>
        /// message with explicit source specification
        /// </summary>
        /// <param name="source"></param>
        /// <param name="severity"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool Message(string source, NotificationSeverity severity, string message)
        {
            return MessageCore(-1, false, source, severity, message, null, false, false, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="levelOfMessage"></param>
        /// <param name="source"></param>
        /// <param name="severity"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool Message(byte levelOfMessage, string source, NotificationSeverity severity, string message)
        {
            return MessageCore(levelOfMessage, false, source, severity, message, null, false, false, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="levelOfMessage"></param>
        /// <param name="source"></param>
        /// <param name="severity"></param>
        /// <param name="messageLambdaDelegate"></param>
        /// <returns></returns>
        public bool Message(byte levelOfMessage, string source, NotificationSeverity severity, DVoid2String messageLambdaDelegate)
        {
            return MessageCore(levelOfMessage, false, source, severity, null, messageLambdaDelegate,
                false, false, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="severity"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool Message(NotificationSeverity severity, string message)
        {
            return MessageCore(-1, true, null, severity, message, null,
                false, false, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="levelOfMessage"></param>
        /// <param name="severity"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool Message(byte levelOfMessage, NotificationSeverity severity, string message)
        {
            return MessageCore(levelOfMessage, true, null, severity, message, null,
                false, false, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="severity"></param>
        /// <param name="message"></param>
        /// <param name="intoConsole"></param>
        /// <param name="intoDebugConsole"></param>
        /// <returns></returns>
        public bool Message(string source, NotificationSeverity severity, string message, bool intoConsole, bool intoDebugConsole)
        {
            return MessageCore(-1, false, source, severity, message, null, intoConsole, intoDebugConsole, null);
        }

        /*
        public bool Message(NotificationSeverity severity, string message, bool intoConsole, bool intoEventLog, bool intoGUI)
        {
            return Message(true, null, severity, message, true, intoConsole, intoEventLog, intoGUI);
        }*/
        #endregion

        #region Errors
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="message"></param>
        public void Error(string source, string message)
        {
            MessageCore(-1, false, source, NotificationSeverity.Error, message, null,
                false, false, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="levelOfMessage"></param>
        /// <param name="source"></param>
        /// <param name="message"></param>
        public void Error(byte levelOfMessage, string source, string message)
        {
            MessageCore(levelOfMessage, false, source, NotificationSeverity.Error, message, null,
                false, false, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="levelOfMessage"></param>
        /// <param name="source"></param>
        /// <param name="messageLambdaDelegate"></param>
        public void Error(byte levelOfMessage, string source, DVoid2String messageLambdaDelegate)
        {
            MessageCore(levelOfMessage, false, source, NotificationSeverity.Error, null, messageLambdaDelegate,
                false, false, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void Error(string message)
        {
            MessageCore(-1, true, null, NotificationSeverity.Error, message, null,
                false, false, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="verbosityLevel"></param>
        /// <param name="message"></param>
        public void Error(byte verbosityLevel, string message)
        {
            MessageCore(verbosityLevel, true, null, NotificationSeverity.Error, message, null,
                false, false, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="verbosityLevel"></param>
        /// <param name="messageLambdaDelegate"></param>
        public void Error(byte verbosityLevel, DVoid2String messageLambdaDelegate)
        {
            MessageCore(verbosityLevel, true, null, NotificationSeverity.Error, null, messageLambdaDelegate,
                false, false, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="verbosityLevel"></param>
        /// <param name="formattedString"></param>
        /// <param name="formatArguments"></param>
        public void ErrorFormatted(byte verbosityLevel, string formattedString, params object[] formatArguments)
        {
            DVoid2String d = () => (string.Format(formattedString, formatArguments));
            MessageCore(verbosityLevel, true, null, NotificationSeverity.Error, null, d,
                false, false, null);
        }

        private IPEndPoint PrepareEndpoint(string ip, int port)
        {
            IPEndPoint udpDestEndpoint = null;
            IPAddress ipA = null;
            if (!string.IsNullOrEmpty(ip))
            {
                try
                {
                    ipA = IPAddress.Parse(ip);
                    if (!TcpUdpPort.IsValid(port))
                        ipA = null;
                }
                catch
                {
                }
            }

            if (null != ipA)
                try
                {
                    udpDestEndpoint = new IPEndPoint(ipA, port);
                }
                catch
                {
                }

            return udpDestEndpoint;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="message"></param>
        /// <param name="intoConsole"></param>
        /// <param name="intoDebugConsole"></param>
        /// <param name="intoUdpDestination"></param>
        /// <param name="intoUdpPort"></param>
        public void Error(string source, string message, bool intoConsole, bool intoDebugConsole, string intoUdpDestination, int intoUdpPort)
        {
            IPEndPoint udpDestEndpoint = PrepareEndpoint(intoUdpDestination, intoUdpPort);

            MessageCore(
                -1, false, source, NotificationSeverity.Error, message, null, intoConsole,
                intoDebugConsole,
                udpDestEndpoint);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="intoUdpDestination"></param>
        /// <param name="intoUdpPort"></param>
        public void Error(string message, string intoUdpDestination, int intoUdpPort)
        {
            IPEndPoint udpDestEndpoint = PrepareEndpoint(intoUdpDestination, intoUdpPort);

            MessageCore(
                -1, true, null, NotificationSeverity.Error, message, null,
                false, false, udpDestEndpoint);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="intoConsole"></param>
        /// <param name="intoDebugConsole"></param>
        public void Error(string message, bool intoConsole, bool intoDebugConsole)
        {
            MessageCore(-1, true, null, NotificationSeverity.Error, message, null,
                intoConsole,
                intoDebugConsole,
                null);
        }
        #endregion

        #region Infos
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="message"></param>
        public void Info(string source, string message)
        {
            MessageCore(-1, false, source, NotificationSeverity.Info, message, null,
                false, false, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="importanceLevel"></param>
        /// <param name="source"></param>
        /// <param name="message"></param>
        public void Info(byte importanceLevel, string source, string message)
        {
            MessageCore(importanceLevel, false, source, NotificationSeverity.Info, message, null,
                false, false, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="importanceLevel"></param>
        /// <param name="source"></param>
        /// <param name="messageLambdaDelegate"></param>
        public void Info(byte importanceLevel, string source, DVoid2String messageLambdaDelegate)
        {
            MessageCore(importanceLevel, false, source, NotificationSeverity.Info, null, messageLambdaDelegate,
                false, false, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void Info(string message)
        {
            MessageCore(-1, true, null, NotificationSeverity.Info, message, null,
                false, false, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="importanceLevel"></param>
        /// <param name="message"></param>
        public void Info(byte importanceLevel, string message)
        {
            MessageCore(importanceLevel, true, null, NotificationSeverity.Info, message, null,
                false, false, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="importanceLevel"></param>
        /// <param name="messageLambdaDelegate"></param>
        public void Info(byte importanceLevel, DVoid2String messageLambdaDelegate)
        {
            MessageCore(importanceLevel, true, null, NotificationSeverity.Info, null, messageLambdaDelegate,
                false, false, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="message"></param>
        /// <param name="intoConsole"></param>
        /// <param name="intoDebugConsole"></param>
        /// <param name="intoUdpDestination"></param>
        /// <param name="intoUdpPort"></param>
        public void Info(string source, string message, bool intoConsole, bool intoDebugConsole, string intoUdpDestination, int intoUdpPort)
        {
            IPEndPoint udpDestEndpoint = PrepareEndpoint(intoUdpDestination, intoUdpPort);

            MessageCore(-1, false, source, NotificationSeverity.Info, message, null,
                intoConsole,
                intoDebugConsole,
                udpDestEndpoint);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="intoUdpDestination"></param>
        /// <param name="intoUdpPort"></param>
        public void Info(string message, string intoUdpDestination, int intoUdpPort)
        {
            IPEndPoint udpDestEndpoint = PrepareEndpoint(intoUdpDestination, intoUdpPort);

            MessageCore(-1, true, null, NotificationSeverity.Info, message, null,
                false, false, udpDestEndpoint);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="intoConsole"></param>
        /// <param name="intoDebugConsole"></param>
        public void Info(string message, bool intoConsole, bool intoDebugConsole)
        {
            MessageCore(-1, true, null, NotificationSeverity.Info, message, null,
                intoConsole,
                intoDebugConsole,
                null);
        }
        #endregion
        // ===
        #region Warnings
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="message"></param>
        public void Warning(string source, string message)
        {
            MessageCore(-1, false, source, NotificationSeverity.Warning, message, null,
                false, false, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="importanceLevel"></param>
        /// <param name="source"></param>
        /// <param name="message"></param>
        public void Warning(byte importanceLevel, string source, string message)
        {
            MessageCore(importanceLevel, false, source, NotificationSeverity.Warning, message, null,
                false, false, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="importanceLevel"></param>
        /// <param name="source"></param>
        /// <param name="messageLambdaDelegate"></param>
        public void Warning(byte importanceLevel, string source, DVoid2String messageLambdaDelegate)
        {
            MessageCore(importanceLevel, false, source, NotificationSeverity.Warning, null, messageLambdaDelegate,
                false, false, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void Warning(string message)
        {
            MessageCore(-1, true, null, NotificationSeverity.Warning, message, null,
                false, false, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="importanceLevel"></param>
        /// <param name="message"></param>
        public void Warning(byte importanceLevel, string message)
        {
            MessageCore(importanceLevel, true, null, NotificationSeverity.Warning, message, null,
                false, false, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="importanceLevel"></param>
        /// <param name="messageLambdaDelegate"></param>
        public void Warning(byte importanceLevel, DVoid2String messageLambdaDelegate)
        {
            MessageCore(importanceLevel, true, null, NotificationSeverity.Warning, null, messageLambdaDelegate,
                false, false, null);
        }


        /*public void BlockingWarning(string message)
        {
            Message(true, true, null, NotificationSeverity.Warning, message, null, null, null, 0);
        }*/

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="message"></param>
        /// <param name="intoConsole"></param>
        /// <param name="intoDebugConsole"></param>
        /// <param name="intoUdpDestination"></param>
        /// <param name="intoUdpPort"></param>
        public void Warning(string source, string message, bool intoConsole, bool intoDebugConsole, string intoUdpDestination, int intoUdpPort)
        {
            IPEndPoint udpDestEndpoint = PrepareEndpoint(intoUdpDestination, intoUdpPort);

            MessageCore(-1, false, source, NotificationSeverity.Warning, message, null,
                intoConsole, intoDebugConsole, udpDestEndpoint);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="intoUdpDestination"></param>
        /// <param name="intoUdpPort"></param>
        public void Warning(string message, string intoUdpDestination, int intoUdpPort)
        {
            IPEndPoint udpDestEndpoint = PrepareEndpoint(intoUdpDestination, intoUdpPort);

            MessageCore(-1, true, null, NotificationSeverity.Warning, message, null,
                false, false, udpDestEndpoint);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="intoConsole"></param>
        /// <param name="intoDebugConsole"></param>
        public void Warning(string message, bool intoConsole, bool intoDebugConsole)
        {
            MessageCore(-1, true, null, NotificationSeverity.Warning, message, null,
                intoConsole, intoDebugConsole, null);
        }
        #endregion


        private volatile bool _enableEnqueue = true;
        /// <summary>
        /// 
        /// </summary>
        public bool EnableEnqueue
        {
            get { return _enableEnqueue; }
            set { _enableEnqueue = value; }
        }

        /// <summary>
        /// Creates sting from parameters
        /// </summary>
        /// <param name="parameters">parameters for output string</param>
        /// <returns>parameters string</returns>
        public static string GetStringFromParameters(params object[] parameters)
        {
            return GetStringFromParametersCustom(false, StringConstants.COMMA, parameters);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static string GetStringFromParameterList(IEnumerable parameters)
        {
            return GetStringFromCollectionCustom(false, StringConstants.COMMA, parameters);
        }

        /// <summary>
        /// Creates a string from parameters
        /// </summary>
        /// <param name="inRows">specify, if each parameter string will be in a new line</param>
        /// <param name="delimeter">delimeter between parameter strings (only when inRows is set to FALSE</param>
        /// <param name="parameters">parameters for output string</param>
        /// <returns>parameters string</returns>
        public static string GetStringFromParametersCustom(bool inRows, string delimeter, params object[] parameters)
        {
            return GetStringFromCollectionCustom(inRows, delimeter, parameters);
        }

        /// <summary>
        /// Creates a string from collection
        /// </summary>
        /// <param name="inRows">specify, if each parameter string will be in a new line</param>
        /// <param name="delimeter">delimeter between strings (only when inRows is set to FALSE</param>
        /// <param name="collection">collection for output string</param>
        /// <returns>collection string</returns>
        private static string GetStringFromCollectionCustom(bool inRows, string delimeter, IEnumerable collection)
        {
            if (collection == null)
                return StringConstants.NULL;

            var result = new StringBuilder();

            foreach (var obj in collection)
            {
                string actualParameter;
                if (ReferenceEquals(obj, null))
                    actualParameter = StringConstants.NULL;
                else
                {
                    if (obj is IEnumerable)
                    {
                        actualParameter =
                            StringConstants.BRACKET_LEFT +
                            GetStringFromCollectionCustom(inRows, delimeter, obj as IEnumerable) +
                            StringConstants.BRACKET_RIGHT;
                    }
                    else
                        actualParameter = obj.ToString();
                }

                if (inRows)
                    result.AppendLine(actualParameter);
                else
                {
                    if (result.Length != 0)
                        result.Append(delimeter);
                    result.Append(actualParameter);
                }

            }

            return result.ToString();
        }

        /// <summary>
        /// Creates a parameter string
        /// </summary>
        /// <param name="parameter">parameter for output string</param>
        /// <returns>parameter string</returns>
        public static string GetStringFromParameter(object parameter)
        {
            if (parameter == null)
                return StringConstants.NULL;

            if (parameter is ICollection)
                return GetStringFromParameters(parameter);

            return parameter.ToString();
        }

        /// <summary>
        /// count of all logged messages that havent yet been processed
        /// </summary>
        public int Count
        {
            get { return _processingQueueForLines.Count; }
        }


    }
}
