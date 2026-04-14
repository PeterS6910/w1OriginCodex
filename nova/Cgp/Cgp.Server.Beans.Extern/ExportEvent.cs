using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using Contal.Cgp.Globals;
using System.Diagnostics;

namespace Contal.Cgp.Server.Beans.Extern
{
    /// <summary>
    /// SourceId is IdCard
    /// ReasonId is IdCardReader
    /// </summary>
    [Serializable]
    public class ExportEvent: AOrmObject
    {
        public const string COLUMN_ID = "Id";
        public const string COLUMN_LASTEVENT_ID = "LastEventlogId";
        public const string COLUMN_REASON_ID = "ReasonId";
        public const string COLUMN_SOURCE_ID = "SourceId";
        public const string COLUMN_LASTEVENTDATETIME = "LastEventDateTime";

        public long Id { get; private set; } = 0;
        public long? LastEventLogId { get; private set; }
        public virtual bool AccessDeniedType { get; private set; }
        public Guid SourceId { get; private set; } 
        public Guid ReasonId { get; private set; } 
        public DateTime? EventDateTime { get; private set; }

        public ExportEvent(long lastEventId,string type, Guid sourceId, Guid reasonId, DateTime eventDateTime)
        {
            Id = 0; //not registred in db
            LastEventLogId = lastEventId;
            SourceId= sourceId;
            ReasonId = reasonId;
            AccessDeniedType = IsAccessDeniedType(type);
            EventDateTime = eventDateTime;
        }
        public ExportEvent(long id, long lastEventId, Guid sourceId, Guid reasonId)
        {
            Id = id;
            LastEventLogId = lastEventId;
            SourceId = sourceId;
            ReasonId = reasonId;
            AccessDeniedType = true;
            EventDateTime = null;
        }

        public ExportEvent(long id,  Guid sourceId, Guid reasonId, DateTime eventDateTime)
        {
            Id = id;
            LastEventLogId = null;
            SourceId = sourceId;
            ReasonId = reasonId;
            AccessDeniedType = true;
            EventDateTime = eventDateTime;
        }

        public override bool Compare(object obj)
        {
            if (obj == null)
                return false;

            return obj is ExportEvent && (obj as ExportEvent).Id == Id;
        }

        public override string GetIdString()
        {
           return Id.ToString();
        }

        public override ObjectType GetObjectType()
        {
            return ObjectType.ConsecutiveEvent;
        }

        public override object GetId()
        {
            return Id;
        }

        public override IModifyObject CreateModifyObject()
        {
            return null;
        }

        static public bool IsAccessDeniedTypeMultiple(string type)
        {
           if (
              (type != Eventlog.TYPEACCESSDENIED) &&
              (type != Eventlog.TYPEACCESSDENIEDCARDBLOCKEDORINACTIVE) &&
              (type != Eventlog.TYPE_ACCESS_DENIED_ENTER_TO_AA_MENU_INVALID_CODE) &&
              (type != Eventlog.TYPE_ACCESS_DENIED_ENTER_TO_AA_MENU_INVALID_PIN) &&
              (type != Eventlog.TYPE_ACCESS_DENIED_ENTER_TO_EVENTLOGS_MENU_INVALID_CODE) &&
              (type != Eventlog.TYPE_ACCESS_DENIED_ENTER_TO_EVENTLOGS_MENU_INVALID_PIN) &&
              (type != Eventlog.TYPE_ACCESS_DENIED_ENTER_TO_SENSORS_MENU_INVALID_CODE) &&
              (type != Eventlog.TYPE_ACCESS_DENIED_ENTER_TO_SENSORS_MENU_INVALID_PIN) &&
              (type != Eventlog.TYPEACCESSDENIEDINVALIDCODE) &&
              (type != Eventlog.TYPEACCESSDENIEDINVALIDEMERGENCYCODE) &&
              (type != Eventlog.TYPEACCESSDENIEDINVALIDPIN) &&
              (type != Eventlog.TYPEACCESSDENIEDSETALARMAREAINVALIDCODE) &&
              (type != Eventlog.TYPEACCESSDENIEDSETALARMAREAINVALIDPIN) &&
              (type != Eventlog.TYPEACCESSDENIEDSETALARMAREANORIGHTS) &&
              (type != Eventlog.TYPEUNKNOWNCARD) &&
              (type != Eventlog.TYPEACCESSDENIEDUNSETALARMAREAINVALIDCODE) &&
              (type != Eventlog.TYPEACCESSDENIEDUNSETALARMAREAINVALIDPIN) &&
              (type != Eventlog.TYPEACCESSDENIEDUNSETALARMAREANORIGHTS) &&
              (type != Eventlog.TYPEDSMACCESSINTERRUPTED) &&
              (type != Eventlog.TYPEDSMACCESSPERMITTED) &&
              (type != Eventlog.TYPEDSMACCESSRESTRICTED) &&
              (type != Eventlog.TYPEDSMNORMALACCESS)
           )
           {
                return false;
           }
            return true;
        }

        static public bool IsAccessDeniedType(string type)
        {
            return type == Eventlog.TYPEACCESSDENIED;
        }

        public void UpdateDateTime(DateTime eventDateTime)
        {
            EventDateTime = eventDateTime;
        }
    }
}
