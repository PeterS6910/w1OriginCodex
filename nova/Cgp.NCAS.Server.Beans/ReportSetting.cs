using System;
using System.Collections.Generic;

using Contal.Cgp.Globals;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Server.Beans.Extern;

namespace Contal.Cgp.NCAS.Server.Beans
{
    [Serializable]
    public class ReportSetting : AOrmObject
    {
        public virtual int IdReportSetting { get; set; }        
        public virtual DateTime? DateFrom { get; set; }
        public virtual DateTime? DateTo { get; set; }
        public virtual int LastId { get; set; }


        public override bool Compare(object obj)
        {
            var ReportSetting = obj as ReportSetting;

            return ReportSetting != null
                   && IdReportSetting.Equals(ReportSetting.IdReportSetting);
        }

        public override string GetIdString()
        {
            return IdReportSetting.ToString();
        }

        public override ObjectType GetObjectType()
        {
            return ObjectType.ReportSetting;
        }

        public override object GetId()
        {
            return IdReportSetting;
        }

        public override IModifyObject CreateModifyObject()
        {
            return null;
        }
    }

    public class ReportCommunicationOnlineStateChangedHandler : IwQuick.Remoting.ARemotingCallbackHandler
    {
        private static volatile ReportCommunicationOnlineStateChangedHandler _singleton;
        private static readonly object _syncRoot = new object();

        private Action<bool?> _stateChanged;

        public static ReportCommunicationOnlineStateChangedHandler Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (null == _singleton)
                            _singleton = new ReportCommunicationOnlineStateChangedHandler();
                    }

                return _singleton;
            }
        }

        public ReportCommunicationOnlineStateChangedHandler()
            : base("ReportCommunicationOnlineStateChangedHandler")
        {
        }

        public void RegisterStateChanged(Action<bool?> stateChanged)
        {
            _stateChanged += stateChanged;
        }

        public void UnregisterStateChanged(Action<bool?> stateChanged)
        {
            _stateChanged -= stateChanged;
        }

        public void RunEvent(bool? isConnected)
        {
            if (_stateChanged != null)
                _stateChanged(isConnected);
        }
    }

    [Serializable]
    public class ReportErrorEvent
    {
        public int Id { get; private set; }
        public Eventlog ErrorEventlog { get; private set; }
        public Eventlog SourceEventlog { get; private set; }

        public ReportErrorEvent(
            int id,
            Eventlog errorEventlog,
            Eventlog sourceEventlog)
        {
            Id = id;
            ErrorEventlog = errorEventlog;
            SourceEventlog = sourceEventlog;
        }
    }
}
