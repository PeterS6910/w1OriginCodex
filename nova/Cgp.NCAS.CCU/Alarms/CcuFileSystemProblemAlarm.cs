using System;
using System.Collections.Generic;
using System.Linq;
using Contal.CatCom;
using Contal.Cgp.NCAS.CCU.EventParameters;
using Contal.IwQuick.Data;
using Contal.Cgp.Globals;

namespace Contal.Cgp.NCAS.CCU.Alarms
{
    [LwSerialize(730)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    class CcuFileSystemProblemAlarm :
        Alarm,
        ICreateEventAlarmOccured,
        ICreateEventAlarmChanged,
        ICreateCatAlarms,
        IAlarmLoadFromDatabasePostProcessing
    {
        public string FileOperation { get; private set; }

        public CcuFileSystemProblemAlarm()
        {

        }

        public CcuFileSystemProblemAlarm(
            Guid idCcu,
            string fileName,
            string fileOperation)
            : base(
                CreateAlarmKey(
                    idCcu,
                    fileName),
                AlarmState.Alarm)
        {
            FileOperation = fileOperation;
        }

        public static AlarmKey CreateAlarmKey(
            Guid idCcu,
            string fileName)
        {
            return new AlarmKey(
                AlarmType.CCU_FilesystemProblem,
                new IdAndObjectType(
                    idCcu,
                    ObjectType.CCU),
                Enumerable.Repeat(
                    new AlarmParameter(
                        ParameterType.FileName,
                        fileName),
                    1));
        }

        public EventParameters.EventParameters CreateEventAlarmOccured()
        {
            return new EventAlarmOccuredOrChangedCcuFileSystemProblem(
                true,
                this);
        }

        public EventParameters.EventParameters CreateEventAlarmChanged()
        {
            return new EventAlarmOccuredOrChangedCcuFileSystemProblem(
                false,
                this);
        }

        ICollection<CatAlarm> ICreateCatAlarms.CreateCatAlarms(bool alarmAcknowledged)
        {
            if (alarmAcknowledged
                || AlarmKey == null)
            {
                return null;
            }

            var alarmArcs = CatAlarmsManager.Singleton.GetAlarmArcs(
                AlarmKey.AlarmType,
                AlarmKey.AlarmObject);

            if (alarmArcs == null)
                return null;

            return new[]
            {
                CatAlarmsManager.CreateCcuAlarm(
                    AlarmEventCode.DeviceFailure,
                    this,
                    AlarmState,
                    AlarmKey.AlarmType,
                    alarmArcs)
            };
        }

        public void LoadFromDatabasePostProcessing()
        {
            AlarmsManager.Singleton.StopAlarm(AlarmKey);
        }
    }
}
