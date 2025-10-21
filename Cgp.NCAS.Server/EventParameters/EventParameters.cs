using System;
using System.Collections.Generic;
using System.Text;

using Contal.Cgp.Globals;
using Contal.Cgp.Server;
using Contal.Cgp.Server.Beans.Extern;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.Server.EventParameters
{
    [LwSerialize(400)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public abstract class EventParameters
    {
        public UInt64 EventId { get; private set; }

        public EventType EventType { get; private set; }

        public DateTime DateTime { get; private set; }

        public uint Priority { get; set; }

        public VersionsInfo VersionsInfo { get; set; }

        protected virtual void GetAdditionalParametersString(StringBuilder parameters)
        {

        }

        protected EventParameters(
            UInt64 eventid,
            EventType eventType,
            DateTime dateTime)
        {
            EventId = eventid;
            DateTime = dateTime;
            EventType = eventType;
        }

        protected EventParameters(
            UInt64 eventId,
            EventType eventType)
            : this(
                eventId,
                eventType,
                DateTime.Now)
        {

        }

        protected EventParameters()
        {
            
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

        public virtual void Initialize()
        {
            if (VersionsInfo == null)
                return;

            VersionsInfo.ServerVersion = CgpServer.Singleton.Version.ToString();
        }

        public abstract void HandleEvent(CCUSettings ccuSettings);

        public abstract bool EnqueueEventlog(
            CCUSettings ccuSettings,
            out Eventlog eventlog,
            out List<EventSource> eventSources,
            out List<EventlogParameter> eventlogParameters);
    }

    [LwSerialize(401)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public abstract class EventParametersWithObjectId : EventParameters
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
            UInt64 eventId,
            EventType eventType,
            Guid idObject)
            : base(
                eventId,
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
    public abstract class EventParametersWithState : EventParameters
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
            UInt64 eventId,
            EventType eventType,
            State state)
            : base(
                eventId,
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
    public abstract class EventParametersWithObjectIdAndState : EventParametersWithObjectId
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
            UInt64 eventId,
            EventType eventType,
            Guid idObject,
            State state)
            : base(
                eventId,
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
    [LwSerializeMode(LwSerializationMode.Selective)]
    public class VersionsInfo
    {
        [LwSerialize]
        public string CcuVersion { get; private set; }

        [LwSerialize]
        public string CeVersion { get; private set; }

        public string ServerVersion { get; set; }

        private VersionsInfo()
        {

        }

        public void GetAdditionalParametersString(StringBuilder parameters)
        {
            parameters.Append(
                string.Format(
                    ", CCU version: {0}, CE version: {1}, Server version: {2}",
                    CcuVersion,
                    CeVersion,
                    ServerVersion));
        }
    }
}