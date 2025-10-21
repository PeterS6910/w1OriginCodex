using System.Text;

using Contal.Cgp.Globals;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.CCU.EventParameters
{
    [LwSerialize(550)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    class EventUpsOnlineStateChanged : EventParameters
    {
        public byte OnlineState { get; private set; }

        public EventUpsOnlineStateChanged(byte onlineState)
            : base(
                EventType.UpsOnlineStateChanged)
        {
            OnlineState = onlineState;
        }

        public EventUpsOnlineStateChanged()
        {
            
        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            parameters.Append(
                string.Format(
                    ", Online state: {0}",
                    OnlineState));
        }
    }

    [LwSerialize(551)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    class EventUpsAlarmStateChanged : EventParameters
    {
        public bool IsAlarm { get; private set; }

        public EventUpsAlarmStateChanged(bool isAlarm)
            : base(
                EventType.UpsAlarmStateChanged)
        {
            IsAlarm = isAlarm;
        }

        public EventUpsAlarmStateChanged()
        {
            
        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            parameters.Append(
                string.Format(
                    ", Is alarm: {0}",
                    IsAlarm));
        }
    }

    [LwSerialize(552)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    class EventUpsValuesChanged : EventParameters
    {
        public CUps2750Values UpsValues { get; private set; }

        public EventUpsValuesChanged(CUps2750Values upsValues)
            : base(
                EventType.UpsValuesChanged)
        {
            UpsValues = upsValues;
        }

        public EventUpsValuesChanged()
        {
            
        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            parameters.Append(
                string.Format(
                    ", UPS values: {0}",
                    UpsValues));
        }
    }

    [LwSerialize(553)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmCcuUpsOutputFuse : EventForSpecialInputs
    {
        public EventAlarmCcuUpsOutputFuse(
            bool isOn)
            : base(
                EventType.AlarmCcuUpsOutputFuse,
                isOn)
        {

        }

        public EventAlarmCcuUpsOutputFuse()
        {

        }
    }

    [LwSerialize(554)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmCcuUpsBatteryFault : EventForSpecialInputs
    {
        public EventAlarmCcuUpsBatteryFault(
            bool isOn)
            : base(
                EventType.AlarmCcuUpsBatteryFault,
                isOn)
        {

        }

        public EventAlarmCcuUpsBatteryFault()
        {

        }
    }

    [LwSerialize(555)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmCcuUpsBatteryFuse : EventForSpecialInputs
    {
        public EventAlarmCcuUpsBatteryFuse(
            bool isOn)
            : base(
                EventType.AlarmCcuUpsBatteryFuse,
                isOn)
        {

        }

        public EventAlarmCcuUpsBatteryFuse()
        {

        }
    }

    [LwSerialize(556)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmCcuUpsOvertemperature : EventForSpecialInputs
    {
        public EventAlarmCcuUpsOvertemperature(
            bool isOn)
            : base(
                EventType.AlarmCcuUpsOvertemperature,
                isOn)
        {

        }

        public EventAlarmCcuUpsOvertemperature()
        {

        }
    }

    [LwSerialize(557)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmCcuUpsTamperSabotage : EventForSpecialInputs
    {
        public EventAlarmCcuUpsTamperSabotage(
            bool isOn)
            : base(
                EventType.AlarmCcuUpsOutputFuse,
                isOn)
        {

        }

        public EventAlarmCcuUpsTamperSabotage()
        {

        }
    }

    public static class TestUpcMonitorEvents
    {
        public static void EnqueueTestEvents()
        {
            Events.ProcessEvent(new EventUpsOnlineStateChanged(
                (byte) TUpsOnlineState.Online));

            Events.ProcessEvent(new EventUpsAlarmStateChanged(
                true));

            Events.ProcessEvent(new EventUpsValuesChanged(
                new CUps2750Values()));

            Events.ProcessEvent(new EventAlarmCcuUpsOutputFuse(
                true));

            Events.ProcessEvent(new EventAlarmCcuUpsBatteryFault(
                true));

            Events.ProcessEvent(new EventAlarmCcuUpsBatteryFuse(
                true));

            Events.ProcessEvent(new EventAlarmCcuUpsOvertemperature(
                true));

            Events.ProcessEvent(new EventAlarmCcuUpsTamperSabotage(
                true));
        }
    }
}
