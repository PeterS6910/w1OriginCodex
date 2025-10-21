using System;
using System.Collections.Generic;

using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Server.Beans;

namespace Contal.Cgp.NCAS.Server
{
    public static class CCUEventsManager
    {
        private static readonly Dictionary<EventType, Func<uint>> _priorityByEventType =
            new Dictionary<EventType, Func<uint>>();

        static CCUEventsManager()
        {
            FillPriorityByEventType();
        }

        public static IEnumerable<Guid> GetEventSourcesFromDCU(DCU dcu)
        {
            return dcu.CCU != null
                ? new[] { dcu.CCU.IdCCU, dcu.IdDCU }
                : new[] { dcu.IdDCU };
        }

        public static IEnumerable<Guid> GetEventSourcesFromDoorEnvironment(DoorEnvironment doorEnvironemnt)
        {
            return doorEnvironemnt.CCU != null
                ? new[] { doorEnvironemnt.CCU.IdCCU, doorEnvironemnt.IdDoorEnvironment }
                : (doorEnvironemnt.DCU != null && doorEnvironemnt.DCU.CCU != null
                    ? new[]
                    {
                        doorEnvironemnt.DCU.CCU.IdCCU,
                        doorEnvironemnt.DCU.IdDCU,
                        doorEnvironemnt.IdDoorEnvironment
                    }
                    : new[] { doorEnvironemnt.IdDoorEnvironment });
        }

        public static IEnumerable<Guid> GetEventSourcesFromMultiDoorElement(MultiDoorElement multiDoorElelemnt)
        {
            return multiDoorElelemnt.MultiDoor != null &&
                   multiDoorElelemnt.MultiDoor.CardReader != null &&
                   multiDoorElelemnt.MultiDoor.CardReader.CCU != null
                ? new[]
                {
                    multiDoorElelemnt.MultiDoor.CardReader.CCU.IdCCU,
                    multiDoorElelemnt.MultiDoor.IdMultiDoor,
                    multiDoorElelemnt.IdMultiDoorElement
                }
                : (multiDoorElelemnt.MultiDoor != null &&
                   multiDoorElelemnt.MultiDoor.CardReader != null &&
                   multiDoorElelemnt.MultiDoor.CardReader.DCU != null &&
                   multiDoorElelemnt.MultiDoor.CardReader.DCU.CCU != null
                    ? new[]
                    {
                        multiDoorElelemnt.MultiDoor.CardReader.DCU.CCU.IdCCU,
                        multiDoorElelemnt.MultiDoor.CardReader.DCU.IdDCU,
                        multiDoorElelemnt.MultiDoor.IdMultiDoor,
                        multiDoorElelemnt.IdMultiDoorElement
                    }
                    : new[] {multiDoorElelemnt.IdMultiDoorElement});
        }

        public static ICollection<Guid> GetEventSourcesFromCardReader(CardReader cardReader)
        {
            var result = new LinkedList<Guid>();

            if (cardReader == null)
                return result;

            if (cardReader.CCU != null)
                result.AddLast(cardReader.CCU.IdCCU);
            else if (cardReader.DCU != null && cardReader.DCU.CCU != null)
            {
                result.AddLast(cardReader.DCU.CCU.IdCCU);
                result.AddLast(cardReader.DCU.IdDCU);
            }

            result.AddLast(cardReader.IdCardReader);
            return result;
        }

        public static IEnumerable<Guid> GetEventSourcesFromOutput(Output output, params Guid[] guids)
        {
            var listEventSources = new List<Guid>();

            if (output.CCU != null)
            {
                listEventSources.Add(output.CCU.IdCCU);
                listEventSources.Add(output.IdOutput);
            }
            else
                if (output.DCU != null &&
                    output.DCU.CCU != null)
                {
                    listEventSources.Add(output.DCU.CCU.IdCCU);
                    listEventSources.Add(output.DCU.IdDCU);
                    listEventSources.Add(output.IdOutput);
                }
                else
                    listEventSources.Add(output.IdOutput);

            if (guids != null)
                listEventSources.AddRange(guids);

            return listEventSources;
        }

        public static IEnumerable<Guid> GetEventSourcesFromInput(
            Input input, 
            params Guid[] guids)
        {
            var listEventSources = new List<Guid>();

            if (input.CCU != null)
            {
                listEventSources.Add(input.CCU.IdCCU);
                listEventSources.Add(input.IdInput);
            }
            else
                if (input.DCU != null && input.DCU.CCU != null)
                {
                    listEventSources.Add(input.DCU.CCU.IdCCU);
                    listEventSources.Add(input.DCU.IdDCU);
                    listEventSources.Add(input.IdInput);
                }
                else
                    listEventSources.Add(input.IdInput);

            if (guids != null)
                foreach (var guid in guids)
                    listEventSources.Add(guid);

            return listEventSources;
        }

        private const uint HighestPriority = 10;
        private const uint HighPriority = 20;
        private const uint NormalPriority = 30;
        private const uint LowPriority = 100;

        private static void FillPriorityByEventType()
        {
            // Highest priority
            _priorityByEventType.Add(
                EventType.CcuActualTimeSent,
                () => HighestPriority);

            // High priority
            _priorityByEventType.Add(
                EventType.AlarmAreaAlarmStateChanged,
                () => HighPriority);

            _priorityByEventType.Add(
                EventType.AlarmAreaAlarmStateInfo,
                () => HighPriority);

            _priorityByEventType.Add(
                EventType.AlarmAreaActivationStateChanged,
                () => HighPriority);

            _priorityByEventType.Add(
                EventType.DSMStateChanged,
                () => HighPriority);

            _priorityByEventType.Add(
                EventType.CardReaderOnlineStateChanged,
                () => HighPriority);

            _priorityByEventType.Add(
                EventType.DcuOnlineStateChanged,
                () => HighPriority);

            _priorityByEventType.Add(
                EventType.DcuMemoryWarning,
                () => HighPriority);

            _priorityByEventType.Add(
                EventType.DcuFirmwareVersion,
                () => HighPriority);

            _priorityByEventType.Add(
                EventType.CardReaderCommandChanged,
                () => HighPriority);

            _priorityByEventType.Add(
                EventType.DcuPhysicalAddressChanged,
                () => HighPriority);

            _priorityByEventType.Add(
                EventType.DcuInputCount,
                () => HighPriority);

            _priorityByEventType.Add(
                EventType.DcuOutputCount,
                () => HighPriority);

            _priorityByEventType.Add(
                EventType.SendTamper,
                () => HighPriority);

            _priorityByEventType.Add(
                EventType.SensorInAlarmCount,
                () => HighPriority);

            _priorityByEventType.Add(
                EventType.CardReaderLastCardChanged,
                () => HighPriority);

            _priorityByEventType.Add(
                EventType.UnsetAlarmAreaByObjectForAutomaticActivation,
                () => HighPriority);

            _priorityByEventType.Add(
                EventType.AlarmCCUPrimaryPowerMissing,
                () => HighPriority);

            _priorityByEventType.Add(
                EventType.AlarmCCUBatteryIsLow,
                () => HighPriority);

            _priorityByEventType.Add(
                EventType.AlarmCCUExtFuse,
                () => HighPriority);

            _priorityByEventType.Add(
                EventType.CoprocessorFailureChanged,
                () => HighPriority);

            _priorityByEventType.Add(
                EventType.AlarmAreaRequestActivationState,
                () => HighPriority);

            _priorityByEventType.Add(
                EventType.ICCUSendingOfObjectStateFailed,
                () => HighPriority);

            _priorityByEventType.Add(
                EventType.ICCUPortAlreadyUsed,
                () => HighPriority);

            _priorityByEventType.Add(
                EventType.AlarmAreaSabotageStateChanged,
                () => HighPriority);

            _priorityByEventType.Add(
                EventType.AlarmAreaSabotageStateInfo,
                () => HighPriority);

            _priorityByEventType.Add(
                EventType.DcuInputsSabotage,
                () => HighPriority);

            _priorityByEventType.Add(
                EventType.AntiPassBackZoneCardEntered,
                () => HighPriority);

            _priorityByEventType.Add(
                EventType.AntiPassBackZoneCardExited,
                () => HighPriority);

            _priorityByEventType.Add(
                EventType.AntiPassBackZoneCardTimedOut,
                () => HighPriority);

            // Normal priority
            _priorityByEventType.Add(
                EventType.DcuUpgradePercentageSet,
                () => NormalPriority);

            _priorityByEventType.Add(
                EventType.CRUpgradePercentageSet,
                () => NormalPriority);

            _priorityByEventType.Add(
                EventType.CRUpgradeResultSet,
                () => NormalPriority);

            _priorityByEventType.Add(
                EventType.CEUpgradeFinished,
                () => NormalPriority);

            _priorityByEventType.Add(
                EventType.ProcessDCUUpgradePackageFailed,
                () => NormalPriority);

            _priorityByEventType.Add(
                EventType.ProcessCRUpgradePackageFailed,
                () => NormalPriority);

            _priorityByEventType.Add(
                EventType.CcuUpgradeFileUnpackProgress,
                () => NormalPriority);

            _priorityByEventType.Add(
                EventType.CCUUpgraderStartFailed,
                () => NormalPriority);

            _priorityByEventType.Add(
                EventType.CCUMemoryLoadStateChanged,
                () => NormalPriority);
        }

        /// <summary>
        /// Returns priority for event type
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="eventType"></param>
        /// <returns></returns>
        public static uint GetEventPriority(DateTime dateTime, EventType eventType)
        {
            Func<uint> getPriority;
            
            return _priorityByEventType.TryGetValue(eventType, out getPriority)
                ? getPriority()
                : LowPriority;
        }
    }
}