using System;
using System.Collections.Generic;
using System.Linq;
using Contal.CatCom;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.EventParameters;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.CCU.Alarms
{
    [LwSerialize(758)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class AlarmAreaSetByOnOffObjectFailedAlarm : 
        Alarm, 
        ICreateEventAlarmOccured, 
        ICreateEventAlarmChanged,
        ICreateCatAlarms
    {
        public AlarmAreaSetByOnOffObjectFailedAlarm()
        {
        }

        public AlarmAreaSetByOnOffObjectFailedAlarm(
            Guid idAlarmArea,
            IdAndObjectType onOffObject)
            : base(
                CreateAlarmKey(
                    idAlarmArea,
                    onOffObject),
                AlarmState.Alarm)
        {

        }

        public static AlarmKey CreateAlarmKey(
            Guid idAlarmArea,
            IdAndObjectType onOffObject)
        {
            return new AlarmKey(
                AlarmType.AlarmArea_SetByOnOffObjectFailed,
                new IdAndObjectType(
                    idAlarmArea,
                    ObjectType.AlarmArea),
                Enumerable.Repeat(
                    onOffObject,
                    1));
        }

        public EventParameters.EventParameters CreateEventAlarmOccured()
        {
            return new EventAlarmOccuredOrChangedAlarmAreaSetByOnOffObjectFailed(
                true,
                this);
        }

        public EventParameters.EventParameters CreateEventAlarmChanged()
        {
            return new EventAlarmOccuredOrChangedAlarmAreaSetByOnOffObjectFailed(
                false,
                this);
        }

        public ICollection<CatAlarm> CreateCatAlarms(bool alarmAcknowledged)
        {
            if (AlarmKey == null
                || AlarmKey.AlarmObject == null
                || AlarmKey.ExtendedObjects == null)
            {
                return null;
            }

            var alarmArea = Database.ConfigObjectsEngine.GetFromDatabase(
                ObjectType.AlarmArea,
                (Guid)AlarmKey.AlarmObject.Id) as DB.AlarmArea;

            if (alarmArea == null)
                return null;

            var alarmArcs = CatAlarmsManager.Singleton.GetAlarmArcs(
                AlarmType.AlarmArea_Alarm,
                new IdAndObjectType(
                    alarmArea.IdAlarmArea,
                    ObjectType.AlarmArea));

            if (alarmArcs == null)
                return null;

            if (IsAcknowledged && AlarmState == AlarmState.Normal)
                return null;

            return new[]
            {
                CatAlarmsManager.CreateAaSetByOnOffObjectFailedAlarm(
                    this,
                    !IsAcknowledged && AlarmState == AlarmState.Alarm,
                    alarmArea,
                    alarmArcs)
            };
        }
    }
}
