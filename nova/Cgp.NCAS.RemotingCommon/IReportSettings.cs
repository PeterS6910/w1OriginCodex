using System;
using System.Collections.Generic;

using Contal.Cgp.NCAS.Server.Beans;
using WcfServiceNovaConnection;

namespace Contal.Cgp.NCAS.RemotingCommon
{
    public interface IReportSettings
    {
        ReportSetting LoadReportSetting(out bool allowEdit);
        bool SaveReportSetting(ReportSetting ReportSetting, out Exception error);
        void EditEnd();
        void ReloadObjectForEdit(out Exception error);
        bool? IsConnected();
        bool HasAccessView();
        Int64 LastId { get; set; }
        ICollection<ReportErrorEvent> GeReportErrorEvents();
        Dictionary<int, TransitionAddResult> TryResendReportErrorEvents(ICollection<ReportErrorEvent> ReportErrorEvents);
        void RemoveReportErrorEvent(ICollection<int> ids);
    }
}
