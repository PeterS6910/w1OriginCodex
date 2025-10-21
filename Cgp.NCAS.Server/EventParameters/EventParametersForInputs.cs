using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.NCAS.Server.DB;
using Contal.Cgp.Server;
using Contal.Cgp.Server.Beans.Extern;
using Contal.Cgp.Server.DB;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.Server.EventParameters
{
    [LwSerialize(500)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventInputStateChanged : EventParametersWithObjectIdAndState
    {
        public bool CreateEventlog { get; private set; }
        public bool AlarmWasGenerated { get; private set; }
        public List<Guid> ActivatedAlarmAreasForSensor { get; private set; }

        public EventInputStateChanged(
            UInt64 eventId,
            Guid idInput,
            State inputState,
            bool createEventlog,
            bool alarmWasGenerated,
            List<Guid> activatedAlarmAreasForSensor)
            : base(
                eventId,
                EventType.InputStateChanged,
                idInput,
                inputState)
        {
            CreateEventlog = createEventlog;
            AlarmWasGenerated = alarmWasGenerated;
            ActivatedAlarmAreasForSensor = activatedAlarmAreasForSensor;
        }

        protected EventInputStateChanged()
        {
            
        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            base.GetAdditionalParametersString(parameters);

            parameters.Append(
                string.Format(
                    ", Create eventlog: {0}, Alarm was generated: {1}",
                    CreateEventlog,
                    AlarmWasGenerated));

            var activatedAlarmAreasForSensorString = GetActivatedAlarmAreasForSensorString();

            if (!string.IsNullOrEmpty(activatedAlarmAreasForSensorString))
                parameters.Append(
                    string.Format(
                        ", Activated alarm areas for sensor: {0}",
                        activatedAlarmAreasForSensorString));
        }

        private string GetActivatedAlarmAreasForSensorString()
        {
            if (ActivatedAlarmAreasForSensor == null)
                return null;

            var activatedAlarmAreasForSensorEnumerator = ActivatedAlarmAreasForSensor.GetEnumerator();
            
            if (!activatedAlarmAreasForSensorEnumerator.MoveNext())
                return null;

            var activatedAlarmAreasForSensorString = new StringBuilder(
                activatedAlarmAreasForSensorEnumerator.Current.ToString());

            while (activatedAlarmAreasForSensorEnumerator.MoveNext())
            {
                activatedAlarmAreasForSensorString.Append(
                    string.Format(
                        ", {0}",
                        activatedAlarmAreasForSensorEnumerator.Current));
            }

            return activatedAlarmAreasForSensorString.ToString();
        }

        public override void HandleEvent(CCUSettings ccuSettings)
        {
            var inputState = InputState.Unknown;

            switch (State)
            {
                case State.Alarm:
                    inputState = InputState.Alarm;
                    break;

                case State.Normal:
                    inputState = InputState.Normal;
                    break;

                case State.Break:
                    inputState = InputState.Break;
                    break;

                case State.Short:
                    inputState = InputState.Short;
                    break;

                case State.UsedByAnotherAplication:
                    inputState = InputState.UsedByAnotherAplication;
                    break;

                case State.OutOfRange:
                    inputState = InputState.OutOfRange;
                    break;
            }

            CCUConfigurationHandler.Singleton.InputChanged(
                IdObject,
                DateTime,
                inputState,
                AlarmWasGenerated,
                ActivatedAlarmAreasForSensor != null
                    ? ActivatedAlarmAreasForSensor.ToList()
                    : null);
        }

        public override bool EnqueueEventlog(
            CCUSettings ccuSettings,
            out Eventlog eventlog,
            out List<EventSource> eventSources,
            out List<EventlogParameter> eventlogParameters)
        {
            if (!CreateEventlog
                || !GeneralOptions.Singleton.EventlogInputStateChanged)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            var input = Inputs.Singleton.GetById(IdObject);

            if (input == null)
            {
                eventlog = null;
                eventSources = null;
                eventlogParameters = null;

                return false;
            }

            var eventSourcesIds = Enumerable.Repeat(
                input.IdInput,
                1);

            if (input.DCU != null)
            {
                eventSourcesIds = eventSourcesIds.Concat(
                    Enumerable.Repeat(
                        input.DCU.IdDCU,
                        1));

                if (input.DCU.CCU != null)
                    eventSourcesIds = eventSourcesIds.Concat(
                        Enumerable.Repeat(
                            input.DCU.CCU.IdCCU,
                            1));
            }
            else if (input.CCU != null)
                eventSourcesIds = eventSourcesIds.Concat(
                    Enumerable.Repeat(
                        input.CCU.IdCCU,
                        1));

            if (ActivatedAlarmAreasForSensor != null)
                eventSourcesIds = eventSourcesIds.Concat(
                    ActivatedAlarmAreasForSensor);

            eventlogParameters = null;

            return Eventlogs.Singleton.CreateEvent(
                    Eventlog.TYPE_INPUT_STATE_CHANGED,
                    DateTime,
                    ccuSettings.CCUEvents.ThisAssemblyName,
                    eventSourcesIds,
                    string.Format(
                        "Input {0} changed its state to {1}",
                        input,
                        State),
                    out eventlog,
                    out eventSources);
        }
    }

    [LwSerialize(501)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventInputStateInfo : EventParametersWithObjectIdAndState
    {
        public EventInputStateInfo(
            UInt64 eventId,
            Guid idInput,
            State inputState)
            : base(
                eventId,
                EventType.InputStateInfo,
                idInput,
                inputState)
        {

        }

        protected EventInputStateInfo()
        {
            
        }

        public override void HandleEvent(CCUSettings ccuSettings)
        {
            var inputState = InputState.Unknown;

            switch (State)
            {
                case State.Alarm:
                    inputState = InputState.Alarm;
                    break;

                case State.Normal:
                    inputState = InputState.Normal;
                    break;

                case State.Break:
                    inputState = InputState.Break;
                    break;

                case State.Short:
                    inputState = InputState.Short;
                    break;

                case State.UsedByAnotherAplication:
                    inputState = InputState.UsedByAnotherAplication;
                    break;

                case State.OutOfRange:
                    inputState = InputState.OutOfRange;
                    break;
            }

            CCUConfigurationHandler.Singleton.InputChanged(
                IdObject,
                DateTime,
                inputState,
                false,
                null);
        }

        public override bool EnqueueEventlog(
            CCUSettings ccuSettings,
            out Eventlog eventlog,
            out List<EventSource> eventSources,
            out List<EventlogParameter> eventlogParameters)
        {
            eventlog = null;
            eventSources = null;
            eventlogParameters = null;

            return false;
        }
    }
}
