using System;

using Contal.Cgp.Globals.PlatformPC;
using Contal.IwQuick.Remoting;

namespace Contal.Cgp.Server.Beans
{
    public class AlarmStateChangedHandler : ARemotingCallbackHandler
    {
        private static volatile AlarmStateChangedHandler _singleton;
        private static readonly object _syncRoot = new object();

        private Action<ServerAlarmCore> _alarmStateChanged;

        public static AlarmStateChangedHandler Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (null == _singleton)
                            _singleton = new AlarmStateChangedHandler();
                    }

                return _singleton;
            }
        }

        public AlarmStateChangedHandler()
            : base("AlarmStateChangedHandler")
        {
        }

        public void RegisterChangeAlarms(Action<ServerAlarmCore> alarmStateChanged)
        {
            _alarmStateChanged += alarmStateChanged;
        }

        public void UnregisterChangeAlarm(Action<ServerAlarmCore> alarmStateChanged)
        {
            _alarmStateChanged -= alarmStateChanged;
        }

        public void RunEvent(ServerAlarmCore alarmCore)
        {
            if (_alarmStateChanged != null)
                _alarmStateChanged(alarmCore);
        }
    }
}
