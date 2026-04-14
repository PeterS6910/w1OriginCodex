using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Server.DB;
using Contal.Cgp.Server.Beans.Extern;
using Contal.Cgp.Server.DB;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.Server.EventParameters
{
    [LwSerialize(550)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    class EventUpsOnlineStateChanged : EventParameters
    {
        public byte OnlineState { get; private set; }

        public EventUpsOnlineStateChanged(
            UInt64 eventId,
            byte onlineState)
            : base(
                eventId,
                EventType.UpsOnlineStateChanged)
        {
            OnlineState = onlineState;
        }

        protected EventUpsOnlineStateChanged()
        {
            
        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            parameters.Append(
                string.Format(
                    ", Online state: {0}",
                    OnlineState));
        }

        public override void HandleEvent(CCUSettings ccuSettings)
        {
            CCUConfigurationHandler.Singleton.CcuUpsOnlineStateChanged(
                ccuSettings.IPAddressString,
                OnlineState);
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

    [LwSerialize(551)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    class EventUpsAlarmStateChanged : EventParameters
    {
        public bool IsAlarm { get; private set; }

        public EventUpsAlarmStateChanged(
            UInt64 eventId,
            bool isAlarm)
            : base(
                eventId,
                EventType.UpsAlarmStateChanged)
        {
            IsAlarm = isAlarm;
        }

        protected EventUpsAlarmStateChanged()
        {
            
        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            parameters.Append(
                string.Format(
                    ", Is alarm: {0}",
                    IsAlarm));
        }

        public override void HandleEvent(CCUSettings ccuSettings)
        {
            CCUConfigurationHandler.Singleton.CcuUpsAlarmStateChanged(
                ccuSettings.IPAddressString,
                IsAlarm);
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

    [LwSerialize(552)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    class EventUpsValuesChanged : EventParameters
    {
        public CUps2750Values UpsValues { get; private set; }

        public EventUpsValuesChanged(
            UInt64 eventId,
            CUps2750Values upsValues)
            : base(
                eventId,
                EventType.UpsValuesChanged)
        {
            UpsValues = upsValues;
        }

        protected EventUpsValuesChanged()
        {
            
        }

        protected override void GetAdditionalParametersString(StringBuilder parameters)
        {
            parameters.Append(
                string.Format(
                    ", UPS values: {0}",
                    UpsValues));
        }

        public override void HandleEvent(CCUSettings ccuSettings)
        {
            CCUConfigurationHandler.Singleton.CcuUpsValuesChanged(
                ccuSettings.IPAddressString,
                UpsValues);
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

    [LwSerialize(553)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmCcuUpsOutputFuse : EventForSpecialInputs
    {
        public EventAlarmCcuUpsOutputFuse(
            UInt64 eventId,
            bool isOn)
            : base(
                eventId,
                EventType.AlarmCCUBatteryIsLow,
                isOn)
        {

        }

        protected EventAlarmCcuUpsOutputFuse()
        {

        }

        public override void HandleEvent(CCUSettings ccuSettings)
        {

        }

        public override bool EnqueueEventlog(
            CCUSettings ccuSettings,
            out Eventlog eventlog,
            out List<EventSource> eventSources,
            out List<EventlogParameter> eventlogParameters)
        {
            var ccu = CCUs.Singleton.GetCCUFormIpAddress(ccuSettings.IPAddressString);

            var eventSourcesIds =
                new LinkedList<Guid>(
                    ccu != null
                        ? Enumerable.Repeat(ccu.IdCCU, 1)
                        : Enumerable.Empty<Guid>());

            var description =
                string.Format(
                    "Alarm UPS output fuse is in state {0}",
                    State == State.On
                        ? "alarm"
                        : "normal");

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPE_CCU_ALARM_UPS_OUTPUT_FUSE,
                DateTime,
                ccuSettings.CCUEvents.ThisAssemblyName,
                eventSourcesIds,
                description,
                out eventlog,
                out eventSources,
                out eventlogParameters);
        }
    }

    [LwSerialize(554)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmCcuUpsBatteryFault : EventForSpecialInputs
    {
        public EventAlarmCcuUpsBatteryFault(
            UInt64 eventId,
            bool isOn)
            : base(
                eventId,
                EventType.AlarmCCUBatteryIsLow,
                isOn)
        {

        }

        protected EventAlarmCcuUpsBatteryFault()
        {

        }

        public override void HandleEvent(CCUSettings ccuSettings)
        {

        }

        public override bool EnqueueEventlog(
            CCUSettings ccuSettings,
            out Eventlog eventlog,
            out List<EventSource> eventSources,
            out List<EventlogParameter> eventlogParameters)
        {
            var ccu = CCUs.Singleton.GetCCUFormIpAddress(ccuSettings.IPAddressString);

            var eventSourcesIds =
                new LinkedList<Guid>(
                    ccu != null
                        ? Enumerable.Repeat(ccu.IdCCU, 1)
                        : Enumerable.Empty<Guid>());

            var description =
                string.Format(
                    "Alarm UPS battery fault is in state {0}",
                    State == State.On
                        ? "alarm"
                        : "normal");

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPE_CCU_ALARM_UPS_BATTERY_FAULT,
                DateTime,
                ccuSettings.CCUEvents.ThisAssemblyName,
                eventSourcesIds,
                description,
                out eventlog,
                out eventSources,
                out eventlogParameters);
        }
    }

    [LwSerialize(555)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmCcuUpsBatteryFuse : EventForSpecialInputs
    {
        public EventAlarmCcuUpsBatteryFuse(
            UInt64 eventId,
            bool isOn)
            : base(
                eventId,
                EventType.AlarmCCUBatteryIsLow,
                isOn)
        {

        }

        protected EventAlarmCcuUpsBatteryFuse()
        {

        }

        public override void HandleEvent(CCUSettings ccuSettings)
        {

        }

        public override bool EnqueueEventlog(
            CCUSettings ccuSettings,
            out Eventlog eventlog,
            out List<EventSource> eventSources,
            out List<EventlogParameter> eventlogParameters)
        {
            var ccu = CCUs.Singleton.GetCCUFormIpAddress(ccuSettings.IPAddressString);

            var eventSourcesIds =
                new LinkedList<Guid>(
                    ccu != null
                        ? Enumerable.Repeat(ccu.IdCCU, 1)
                        : Enumerable.Empty<Guid>());

            var description =
                string.Format(
                    "Alarm UPS battery fuse is in state {0}",
                    State == State.On
                        ? "alarm"
                        : "normal");

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPE_CCU_ALARM_UPS_BATTERY_FUSE,
                DateTime,
                ccuSettings.CCUEvents.ThisAssemblyName,
                eventSourcesIds,
                description,
                out eventlog,
                out eventSources,
                out eventlogParameters);
        }
    }

    [LwSerialize(556)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmCcuUpsOvertemperature : EventForSpecialInputs
    {
        public EventAlarmCcuUpsOvertemperature(
            UInt64 eventId,
            bool isOn)
            : base(
                eventId,
                EventType.AlarmCCUBatteryIsLow,
                isOn)
        {

        }

        protected EventAlarmCcuUpsOvertemperature()
        {

        }

        public override void HandleEvent(CCUSettings ccuSettings)
        {

        }

        public override bool EnqueueEventlog(
            CCUSettings ccuSettings,
            out Eventlog eventlog,
            out List<EventSource> eventSources,
            out List<EventlogParameter> eventlogParameters)
        {
            var ccu = CCUs.Singleton.GetCCUFormIpAddress(ccuSettings.IPAddressString);

            var eventSourcesIds =
                new LinkedList<Guid>(
                    ccu != null
                        ? Enumerable.Repeat(ccu.IdCCU, 1)
                        : Enumerable.Empty<Guid>());

            var description =
                string.Format(
                    "Alarm UPS overtemperature is in state {0}",
                    State == State.On
                        ? "alarm"
                        : "normal");

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPE_CCU_ALARM_UPS_OVERTEMPERATURE,
                DateTime,
                ccuSettings.CCUEvents.ThisAssemblyName,
                eventSourcesIds,
                description,
                out eventlog,
                out eventSources,
                out eventlogParameters);
        }
    }

    [LwSerialize(557)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class EventAlarmCcuUpsTamperSabotage : EventForSpecialInputs
    {
        public EventAlarmCcuUpsTamperSabotage(
            UInt64 eventId,
            bool isOn)
            : base(
                eventId,
                EventType.AlarmCCUBatteryIsLow,
                isOn)
        {

        }

        protected EventAlarmCcuUpsTamperSabotage()
        {

        }

        public override void HandleEvent(CCUSettings ccuSettings)
        {

        }

        public override bool EnqueueEventlog(
            CCUSettings ccuSettings,
            out Eventlog eventlog,
            out List<EventSource> eventSources,
            out List<EventlogParameter> eventlogParameters)
        {
            var ccu = CCUs.Singleton.GetCCUFormIpAddress(ccuSettings.IPAddressString);

            var eventSourcesIds =
                new LinkedList<Guid>(
                    ccu != null
                        ? Enumerable.Repeat(ccu.IdCCU, 1)
                        : Enumerable.Empty<Guid>());

            var description =
                string.Format(
                    "Alarm UPS tamper is in state {0}",
                    State == State.On
                        ? "alarm"
                        : "normal");

            return Eventlogs.Singleton.CreateEvent(
                Eventlog.TYPE_CCU_ALARM_UPS_TAMPER,
                DateTime,
                ccuSettings.CCUEvents.ThisAssemblyName,
                eventSourcesIds,
                description,
                out eventlog,
                out eventSources,
                out eventlogParameters);
        }
    }
}
