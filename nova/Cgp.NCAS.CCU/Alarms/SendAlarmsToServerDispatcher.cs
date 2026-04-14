using System;
using System.Collections.Generic;

using Contal.Cgp.Globals;
using Contal.IwQuick;
using Contal.IwQuick.CrossPlatform;
using Contal.IwQuick.Data;
using Contal.IwQuick.Sys;

namespace Contal.Cgp.NCAS.CCU.Alarms
{
    [LwSerialize(710)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class AlarmEvent
    {
        protected AlarmEvent()
        {
            
        }

        public virtual bool ProcessEvent()
        {
            return false;
        }

        public virtual void SendEventToServerSucceded()
        {
            
        }
    }

    [LwSerialize(711)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class AlarmEventWithId : AlarmEvent
    {
        public Guid IdAlarm { get; private set; }

        protected AlarmEventWithId()
        {

        }

        public AlarmEventWithId(
            Guid idAlarm)
        {
            IdAlarm = idAlarm;
        }
    }

    [LwSerialize(712)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class AlarmAdded : AlarmEvent
    {
        public Alarm Alarm { get; private set; }

        protected AlarmAdded()
        {
            
        }

        public AlarmAdded(Alarm alarm)
        {
            Alarm = alarm;
        }
    }

    [LwSerialize(713)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class AlarmStopped : AlarmEventWithId
    {
        protected AlarmStopped()
        {

        }

        public AlarmStopped(Guid idAlarm)
            : base(idAlarm)
        {
        }
    }

    [LwSerialize(714)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class AlarmAcknowledged : AlarmEventWithId
    {
        protected AlarmAcknowledged()
        {

        }

        public AlarmAcknowledged(
            Guid idAlarm)
            : base(idAlarm)
        {

        }

        public override bool ProcessEvent()
        {
            return AlarmsManager.Singleton.AcknowledgeAlarmFromServer(
                IdAlarm);
        }
    }

    [LwSerialize(715)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class AlarmRemoved : AlarmEventWithId
    {
        protected AlarmRemoved()
        {
            
        }

        public AlarmRemoved(
            Guid idAlarm)
            : base(idAlarm)
        {
            
        }

        public override void SendEventToServerSucceded()
        {
            AlarmsManager.Singleton.SendEventAlarmRemovedToServerSucceded(IdAlarm);
        }
    }

    [LwSerialize(716)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class AlarmGeneralBlockingChanged : AlarmEventWithId
    {
        public bool AlarmBlocked { get; private set; }

        protected AlarmGeneralBlockingChanged()
        {

        }

        public AlarmGeneralBlockingChanged(
            Guid idAlarm,
            bool alarmBlocked)
            : base(idAlarm)
        {
            AlarmBlocked = alarmBlocked;
        }
    }

    [LwSerialize(717)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class AlarmIndividualBlockingChanged : AlarmEventWithId
    {
        public bool AlarmBlocked { get; private set; }
        public DateTime UtcDateTime { get; private set; }

        protected AlarmIndividualBlockingChanged()
        {

        }

        public AlarmIndividualBlockingChanged(
            Guid idAlarm,
            bool alarmBlocked,
            DateTime utcDateTime)
            : base(idAlarm)
        {
            AlarmBlocked = alarmBlocked;
            UtcDateTime = utcDateTime;
        }

        public override bool ProcessEvent()
        {
            return AlarmBlocked
                ? AlarmsManager.Singleton.BlockAlarmIndividualFromServer(
                    IdAlarm,
                    UtcDateTime)
                : AlarmsManager.Singleton.UnblockAlarmIndividualFromServer(
                    IdAlarm,
                    UtcDateTime);
        }
    }

    [LwSerialize(718)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class AlarmAcknowledgeInPending
    {
        public Guid IdAlarm { get; private set; }
        public DateTime CreatedDateTime { get; private set; }

        protected AlarmAcknowledgeInPending()
        {
            
        }

        public AlarmAcknowledgeInPending(
            Guid idAlarm,
            DateTime createdDateTime)
        {
            IdAlarm = idAlarm;
            CreatedDateTime = createdDateTime;
        }
    }

    [LwSerialize(719)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class AlarmIndividualBlockingChangeInPending
    {
        public Guid IdAlarm { get; private set; }
        public bool AlarmBlocked { get; private set; }
        public DateTime UtcDateTime { get; private set; }

        protected AlarmIndividualBlockingChangeInPending()
        {

        }

        public AlarmIndividualBlockingChangeInPending(
            Guid idAlarm,
            bool alarmBlocked,
            DateTime utcDateTime)
        {
            IdAlarm = idAlarm;
            AlarmBlocked = alarmBlocked;
            UtcDateTime = utcDateTime;
        }
    }

    public sealed class SendAlarmsToServerDispatcher : ASingleton<SendAlarmsToServerDispatcher>
    {
        private class SendToServerBatchExecutor : IBatchExecutor<AlarmEvent>
        {
            public int Execute(ICollection<AlarmEvent> requests)
            {
                var alarmEventsToSend = new LinkedList<AlarmEvent>(requests);

                if (!CcuCoreRemotingProvider.Singleton.DoSaveAlarmEvent(alarmEventsToSend))
                    return 0;

                try
                {
                    foreach (var alarmEventToSend in alarmEventsToSend)
                        alarmEventToSend.SendEventToServerSucceded();
                }
                catch (Exception error)
                {
                    HandledExceptionAdapter.Examine(error);
                }

                return alarmEventsToSend.Count;
            }
        }

        private const int MAX_ALARM_EVENTS_COUNT = 100;
        private const int DELAY_BETWEEN_SENDING = 200;

        private readonly BatchWorker<AlarmEvent> _sendToServerBatchWorker;

        private SendAlarmsToServerDispatcher()
            : base(null)
        {
            _sendToServerBatchWorker = new BatchWorker<AlarmEvent>(
                new SendToServerBatchExecutor(),
                DELAY_BETWEEN_SENDING,
                MAX_ALARM_EVENTS_COUNT);
        }

        public void SendAlarmEvent(AlarmEvent alarmEvent)
        {
            _sendToServerBatchWorker.Add(alarmEvent);
        }
        
        public void SendAlarmEvents(IEnumerable<AlarmEvent> alarmEvents)
        {
            _sendToServerBatchWorker.Add(alarmEvents);
        }

        public void Clear()
        {
            _sendToServerBatchWorker.Clear();
        }
    }
}
