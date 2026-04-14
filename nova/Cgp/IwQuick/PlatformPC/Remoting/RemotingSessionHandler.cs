using System;
using System.Collections.Generic;
using System.Threading;

namespace Contal.IwQuick.Remoting
{
    /// <summary>
    /// session handler and provider
    /// </summary>
    public class RemotingSessionHandler
    {
        private class SessionRecord
        {
            private Timer _timeout;
            private RemotingSession _session;

            public SessionRecord(RemotingSession session, Timer timeout)
            {
                _session = session;
                _timeout = timeout;
            }

            public RemotingSession Session
            {
                get 
                {
                    return _session;
                }
            }

            public void Clear()
            {
                if (null != _timeout)
                {
                    try
                    {
                        _timeout.Change(-1, -1);
                        _timeout.Dispose();
                    }
                    catch
                    {
                    }

                    _timeout = null;
                }


                if (null == _session) 
                    return;

                _session.Clear();
                _session = null;
            }

            public void RefreshSessionTimeout(long sessionTimeout)
            {
                _timeout.Change(sessionTimeout, -1);
            }
        }

        public static string CallingSessionId
        {
            get
            {
                return ClientIdentificationServerSink.CallingSessionId;
            }
        }
        
        private volatile Dictionary<object, SessionRecord> _sessions = 
            new Dictionary<object, SessionRecord>();
        
        private static volatile RemotingSessionHandler _singleton;
        private static readonly object _syncRoot = new object();

        public static RemotingSessionHandler Singleton
        {
            get
            {
                if (null == _singleton) // optimization only
                    lock (_syncRoot) // atomicity
                        if (_singleton == null)
                            _singleton = new RemotingSessionHandler();

                return _singleton;
            }
        }

        /// <summary>
        /// returns true, if the session identified by specified Id exists
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public bool SessionExists(object sessionId)
        {
            Validator.CheckForNull(
                sessionId,
                "sessionId");

            lock (_sessions)
            {
                return _sessions.ContainsKey(sessionId);
            }
        }


        /// <summary>
        /// returns the RemotingSession for the 
        /// </summary>
        /// <param name="sessionId"></param>
        /// <exception cref="InvalidOperationException">if the connection ID is below zero</exception>
        public RemotingSession this[object sessionId]
        {
            get
            {
                SessionRecord sr;

                lock (_sessions)
                {
                    _sessions.TryGetValue(sessionId, out sr);
                }

                return sr != null ? sr.Session : null;
            }
        }

        protected internal void RegisterSession(object sessionId)
        {
            Validator.CheckForNull(
                sessionId, 
                "sessionId");

            SessionRecord sr;

            lock (_sessions)
            {

                if (_sessions.TryGetValue(sessionId, out sr))
                    sr.Clear();

                sr = 
                    new SessionRecord(
                        new RemotingSession(sessionId),
                        StartSessionTimeout(sessionId));

                _sessions[sessionId] = sr;
            }

        }

        private Timer StartSessionTimeout(object sessionId)
        {
            // starting timeout
            return new Timer(OnSessionTimeout, sessionId, _sessionTimeout, -1);
        }

        /// <summary>
        /// unsets and removes the RemotingSession identified by the connection ID
        /// </summary>
        /// <param name="sessionId"></param>
        protected internal bool Unset(object sessionId)
        {
            if (sessionId == null)
                return false;

            SessionRecord sr;
            bool removed = false;

            lock (_sessions) // atomic exclusion with stopping timer in RefreshSessionTimeout
                if (_sessions.TryGetValue(sessionId, out sr))
                {
                    _sessions.Remove(sessionId);
                    removed = true;
                }

            if (sr != null)
                sr.Clear();

            return removed;
        }

        private int _sessionTimeout = 300000;

        /// <summary>
        /// session timeout in miliseconds
        /// implicit is 300 000 ms ( 5 minutes)
        /// </summary>
        public int SessionTimeout
        {
            get 
            { 
                return _sessionTimeout; 
            }
            set
            {
                if (value > 0)
                    _sessionTimeout = value;
            }
        }

        protected internal bool RefreshSessionTimeout(object sessionId)
        {
            if (Validator.IsNull(sessionId))
                return false;

            SessionRecord sr;

            lock (_sessions) // this makes atomic exclusion with the timer, with the actual Unset call inside
                if (_sessions.TryGetValue(sessionId, out sr))
                    sr.RefreshSessionTimeout(_sessionTimeout);

            return sr != null;
        }

        DObject2Void _delegateSessionTimeOut;
        public void AddDelegateSessionTimeOut(DObject2Void delegateSessionTimeOut)
        {
            _delegateSessionTimeOut += delegateSessionTimeOut;
        }

        private void OnSessionTimeout(object state)
        {
            string sessionId = state as string;

            if (Unset(sessionId))
                if (_delegateSessionTimeOut != null)
                    try
                    {
                        _delegateSessionTimeOut(sessionId);
                    }
                    catch
                    {
                    }
        }
    }


}
