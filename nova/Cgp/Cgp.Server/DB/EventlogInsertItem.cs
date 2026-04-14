using System.Collections.Generic;

using Contal.Cgp.Server.Beans.Extern;

namespace Contal.Cgp.Server.DB
{
    public class EventlogInsertItem : ADbBatchWorker<EventlogInsertItem>.DbItem
    {
        public readonly Eventlog Eventlog;
        public readonly IEnumerable<EventSource> EventSources;
        public readonly IEnumerable<EventlogParameter> EventlogParameters;

        public virtual bool Active
        {
            get { return true; }
        }

        public EventlogInsertItem(
            Eventlog eventlog,
            IEnumerable<EventSource> eventSources,
            IEnumerable<EventlogParameter> eventlogParameters)
        {
            Eventlog = eventlog;
            EventSources = eventSources;
            EventlogParameters = eventlogParameters;
        }
    }
}