using System;
using System.Collections.Generic;

using Contal.Cgp.BaseLib;
using Contal.Cgp.Server.Beans.Extern;

namespace Contal.Cgp.RemotingCommon
{
    public interface IEventlogs
    {
        bool InsertEvent(string type, DateTime date, string cgpSource, Guid[] eventSourceObjectGuids, string description, IEnumerable<EventlogParameter> parameters);
        bool InsertEvent(string type, string cgpSource, Guid[] eventSourceObjectGuids, string description, IEnumerable<EventlogParameter> parameters);
        bool InsertEvent(string type, string cgpSource, Guid[] eventSourceObjectGuids, string description);
        bool InsertEvent(string type, string cgpSource, Guid[] eventSourceObjectGuids, string description, params EventlogParameter[] parameters);
        void InsertEventClientLogin(string loginUserName);
        void InsertEventClientLogout(string loginUserName);

        ICollection<Eventlog> SelectRangeByCriteria(
            IList<FilterSettings> filterSettings,
            ICollection<int> subSitesFilter,
            int firstResult,
            int maxResults);

        long GenerateDataForExcelByCriteria(IList<FilterSettings> filterSettings, ICollection<int> subSitesFilter, string dateFormat,
            string sessionId, string emailToSend);

        int GetInsertCounter();

        int SelectCount(
            IList<FilterSettings> filterSettings,
            ICollection<int> subSitesFilter,
            out Exception error);

        Dictionary<long, ICollection<string>> GetEventSourceNames(IList<long> eventlogsId);
        bool HasAccessView();
        bool HasAccessViewForObject(Eventlog eventlog);
        bool HasAccessExport();
        Eventlog GetObjectById(long id);
        List<string> GetEventLogTypes();
        void AbortCurrentQuery();
        // Create csv export lines from evenlogs
        IList<string> CreateCSVExportLines(IList<FilterSettings> filterSettings, IList<string> columns, string separator, string dateFormat);
    }
}
