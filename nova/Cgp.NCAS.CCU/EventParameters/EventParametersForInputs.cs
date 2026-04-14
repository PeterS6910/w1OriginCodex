using System;
using System.Collections.Generic;
using System.Text;

using Contal.Cgp.Globals;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.CCU.EventParameters
{
    [LwSerialize(500)]
    [LwSerializeMode(LwSerializationMode.InvertedSelective)]
    public class EventInputStateChanged : EventParametersWithObjectIdAndState
    {
        public bool CreateEventlog { get; private set; }
        public bool AlarmWasGenerated { get; private set; }
        public List<Guid> ActivatedAlarmAreasForSensor { get; private set; }

        public EventInputStateChanged()
        {
            
        }

        public EventInputStateChanged(
            Guid idInput,
            State inputState,
            bool createEventlog,
            bool alarmWasGenerated,
            List<Guid> activatedAlarmAreasForSensor)
            : base(
                EventType.InputStateChanged,
                idInput,
                inputState)
        {
            CreateEventlog = createEventlog;
            AlarmWasGenerated = alarmWasGenerated;
            ActivatedAlarmAreasForSensor = activatedAlarmAreasForSensor;
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
    }

    [LwSerialize(501)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventInputStateInfo : EventParametersWithObjectIdAndState
    {
        public EventInputStateInfo(
            Guid idInput,
            State inputState)
            : base(
                EventType.InputStateInfo,
                idInput,
                inputState)
        {

        }

        public EventInputStateInfo()
        {
            
        }
    }

    public static class TestInputEvents
    {
        public static void EnqueueTestEvents(
            Guid idInput)
        {
            Events.ProcessEvent(new EventInputStateChanged(
                idInput,
                State.Alarm,
                true,
                false,
                null));

            Events.ProcessEvent(new EventInputStateInfo(
                idInput,
                State.Normal));
        }
    }
}
