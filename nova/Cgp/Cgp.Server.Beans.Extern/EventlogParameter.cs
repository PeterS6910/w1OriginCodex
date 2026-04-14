using System;

using Contal.Cgp.Globals;

namespace Contal.Cgp.Server.Beans.Extern
{
    [Serializable]
    public class EventlogParameter
    {
        public const string COLUMN_TYPE = "Type";
        public const string COLUMN_TYPE_GUID = "TypeGuid";
        public const string COLUMN_TYPE_OBJECT_TYPE = "TypeObjectType";
        public const string COLUMN_VALUE = "Value";
        public const string COLUMN_ID_EVENTLOG = "IdEventLog";
        public const string COLUMN_ID_EVENTLOG_PARAMETER = "IdEventlogParameter";

        public const string TYPECLIENTIP = "Client IP";
        public const string TYPECLIENTFRIENDLYNAME = "Client friendly name";
        public const string TYPEUSERNAME = "User name";
        public const string TYPEPLUGINNAME = "Plugin name";
        public const string TYPECCU = "CCU ip adress";
        public const string TYPECARDNUMBER = "Card number";
        public const string TYPEPERSONNAME = "Person name";
        public const string TYPEPEALARMAREANAME = "Alarm area name";
        public const string TYPEPEDCU = "DCU";
        public const string TYPEDATETIME = "Date and time";
        public const string TYPECARDSERIALNUMBER = "Card serial number";
        public const string TYPE_STREAM_NAME = "Stream name";
        public const string TYPE_DURATION = "Duration";
        public const string TYPE_ONLINE_STATE = "Online state";
        public const string TYPE_CCU_MEMORY_LOAD = "CCU memory load";
        public const string TYPE_STACKTRACE = "Stack trace";
        public const string TYPE_THREAD_ID = "Thread id";
        public const string TYPE_FILE_NAME = "File name";
        public const string TYPE_FILE_OPERATION = "File operation";
        public const string TYPE_REASON = "Reason";
        public const string TYPE_COMMAND = "Command";
        public const string TYPE_CCU_VERSION = "CCU version";
        public const string TYPE_CE_VERSION = "CE version";
        public const string TYPE_SERVER_VERSION = "Server version";

        public EventlogParameter(string type, string value)
        {
            Type = type;
            Value = value;
        }

        public EventlogParameter(string type, string value, Guid id, ObjectType objType)
        {
            Type = type;
            Value = value;
            TypeGuid = id;
            TypeObjectType = (byte)objType;
        }

        public long IdEventlogParameter { get; set; }
        public string Type { get; set; }
        public Guid TypeGuid { get; set; }
        public byte TypeObjectType { get; set; }
        public string Value { get; set; }

        public Eventlog Eventlog { get; set; }

        public override string ToString()
        {
            return Type + ": " + Value;
        }
    }
}
