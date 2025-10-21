using System;
using System.Text;

using Contal.Cgp.Globals;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.CCU.EventParameters
{
    [LwSerialize(400)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventParameters
    {
        public UInt64 EventId { get; set; }

        public EventType EventType { get; private set; }

        public DateTime DateTime { get; private set; }

        public uint Priority { get; set; }

        public VersionsInfo VersionsInfo { get; set; }

        protected virtual void GetAdditionalParametersString(StringBuilder parameters)
        {

        }

        protected EventParameters(
            EventType eventType,
            DateTime dateTime)
        {
            EventType = eventType;
            DateTime = dateTime;
        }

        protected EventParameters(
            EventType eventType)
            : this(
                eventType,
                UniqueDateTime.GetDateTime)
        {
        }

        protected EventParameters()
        {
            
        }

        public virtual IEventForCardReader CreateEventForCardReader()
        {
            return null;
        }

        public override string ToString()
        {
            var result = new StringBuilder(
                string.Format(
                    "Id: {0}, Type: {1}, Date: {2}",
                    EventId,
                    EventType,
                    DateTime));

            if (VersionsInfo != null)
                VersionsInfo.GetAdditionalParametersString(result);

            GetAdditionalParametersString(result);

            return result.ToString();
        }
    }

    [LwSerialize(401)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventParametersWithObjectId : EventParameters
    {
        public Guid IdObject { get; private set; }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            parameters.Append(
                string.Format(
                    ", Object id: {0}",
                    IdObject));
        }

        protected EventParametersWithObjectId(
            EventType eventType,
            Guid idObject)
            : base(
                eventType)
        {
            IdObject = idObject;
        }

        protected EventParametersWithObjectId()
        {
            
        }
    }

    [LwSerialize(402)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventParametersWithState : EventParameters
    {
        public State State { get; private set; }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            parameters.Append(
                string.Format(
                    ", State: {0}",
                    State));
        }

        protected EventParametersWithState(
            EventType eventType,
            State state)
            : base(
                eventType)
        {
            State = state;
        }

        protected EventParametersWithState()
        {
            
        }
    }

    [LwSerialize(403)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventParametersWithObjectIdAndState : EventParametersWithObjectId
    {
        public State State { get; private set; }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", State: {0}",
                    State));
        }

        protected EventParametersWithObjectIdAndState(
            EventType eventType,
            Guid idObject,
            State state)
            : base(
                eventType,
                idObject)
        {
            State = state;
        }

        protected EventParametersWithObjectIdAndState()
        {
            
        }
    }

    [LwSerialize(404)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class VersionsInfo
    {
        public string CcuVersion { get; private set; }
        public string CeVersion { get; private set; }

        public VersionsInfo(
            string ccuVersion,
            string ceVersion)
        {
            CcuVersion = ccuVersion;
            CeVersion = ceVersion;
        }

        public VersionsInfo()
        {
        }

        public void GetAdditionalParametersString(StringBuilder parameters)
        {
            parameters.Append(
                string.Format(
                    ", CCU version: {0}, CE version: {1}",
                    CcuVersion,
                    CeVersion));
        }
    }
}