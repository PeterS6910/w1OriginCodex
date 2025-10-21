using System;
using System.Collections.Generic;

using Contal.Cgp.NCAS.Server.Beans;
using WcfServiceNovaConnection;

namespace Contal.Cgp.NCAS.RemotingCommon
{
    public interface ITimetecSettings
    {
        TimetecSetting LoadTimetecSetting(out bool allowEdit);
        bool SaveTimetecSetting(TimetecSetting timetecSetting, out Exception error);
        void EditEnd();
        void ReloadObjectForEdit(out Exception error);
        bool? IsConnected();
        bool HasAccessView();
        Int64 LastEventId { get; set; }
        ICollection<TimetecErrorEvent> GeTimetecErrorEvents();
        Dictionary<int, TransitionAddResult> TryResendTimetecErrorEvents(ICollection<TimetecErrorEvent> timetecErrorEvents);
        void RemoveTimetecErrorEvent(ICollection<int> ids);
    }
}
