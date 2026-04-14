using System;

namespace Contal.Cgp.Server.Beans.Extern
{
    [Serializable]
    public class EventSource
    {
        public const string COLUMN_EVENTSOURCE_OBJECT_GUID = "EventSourceObjectGuid";
        public const string COLUMN_EVENTLOG_ID = "IdEventlog";

        public Guid EventSourceObjectGuid { get; private set; }
        public Eventlog Eventlog { get; private set; }

        public EventSource(Guid eventSourceObjectGuid, Eventlog eventlog)
        {
            EventSourceObjectGuid = eventSourceObjectGuid;
            Eventlog = eventlog;
        }
    }
}
