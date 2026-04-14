using System;

using Contal.IwQuick;
using Contal.IwQuick.Remoting;
using Contal.IwQuick.Sys;

namespace Contal.Cgp.Globals.PlatformPC
{
    public class ChangeAlarmsHandler : ARemotingCallbackHandler
    {
        private static volatile ChangeAlarmsHandler _singleton;
        private static readonly object _syncRoot = new object();

        private DVoid2Void _ChangeAlarm;
        private Action<IdServerAlarm> _deletedAlarm;

        public static ChangeAlarmsHandler Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (null == _singleton)
                            _singleton = new ChangeAlarmsHandler();
                    }

                return _singleton;
            }
        }

        public ChangeAlarmsHandler()
            : base("ChangeAlarmsHandler")
        {
        }

        public void RegisterChangeAlarms(DVoid2Void changeAlarm)
        {
            _ChangeAlarm += changeAlarm;
        }

        public void UnregisterChangeAlarm(DVoid2Void changeAlarm)
        {
            _ChangeAlarm -= changeAlarm;
        }

        public void RegisterDeletedAlarm(Action<IdServerAlarm> deletedAlarm)
        {
            _deletedAlarm += deletedAlarm;
        }

        public void UnregisterDeletedAlarm(Action<IdServerAlarm> deletedAlarm)
        {
            _deletedAlarm -= deletedAlarm;
        }

        public void RunEvent()
        {
            if (_ChangeAlarm != null)
                _ChangeAlarm();
        }

        public void RunDeletedAlarm(IdServerAlarm deletedIdServerAlarm)
        {
            if (_deletedAlarm != null)
            {
                try
                {
                    _deletedAlarm(deletedIdServerAlarm);
                }
                catch (Exception error)
                {
                    HandledExceptionAdapter.Examine(error);
                }
            }
        }
    }
}
