using System;
using System.Collections.Generic;
using System.Linq;

using Contal.Cgp.Globals;
using Contal.Cgp.Globals.PlatformPC;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.NCAS.Server.DB;
using Contal.Cgp.Server.Alarms;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.Server.ServerAlarms
{
    [LwSerialize(836)]
    public class SensorTamperServerAlarm : ServerAlarm
    {
        private SensorTamperServerAlarm()
        {

        }

        public SensorTamperServerAlarm(
            CCU ccu,
            Input input,
            Guid idAlarmArea,
            Alarm alarm)
            : base(
                new ServerAlarmCore(
                    ccu.IdCCU,
                    alarm,
                    new LinkedList<IdAndObjectType>(
                        GetParentObjects(
                            ccu.IdCCU,
                            input)),
                    GetName(
                        input,
                        idAlarmArea),
                    ccu.Name,
                    string.Format(
                        "Sensor '{0}' is in tamper",
                        input.ToString()),
                    Enumerable.Repeat(
                        new KeyValuePair<IdAndObjectType, string>(
                            new IdAndObjectType(
                                input.IdInput,
                                ObjectType.Input),
                            SensorServerAlarm.GetSensorName(
                                input,
                                idAlarmArea)
                            ),
                        1)))
        {

        }

        public static string GetName(
            Input input,
            Guid idAlarmArea)
        {
            return string.Format(
                "{0} : {1}",
                AlarmType.Sensor_Tamper_Alarm,
                SensorServerAlarm.GetSensorName(
                    input,
                    idAlarmArea));
        }

        private static IEnumerable<IdAndObjectType> GetParentObjects(
            Guid idCcu,
            Input input)
        {
            var parentObjects = Enumerable.Repeat(
                new IdAndObjectType(
                    idCcu,
                    ObjectType.CCU),
                1);

            if (input.DCU != null)
            {
                parentObjects = parentObjects.Concat(
                    Enumerable.Repeat(
                        new IdAndObjectType(
                            input.DCU.IdDCU,
                            ObjectType.DCU),
                        1));
            }

            return parentObjects;
        }

        public static IEnumerable<IdAndObjectType> GetRelatedObjects(
            Guid idCcu,
            Input input,
            Guid idAlarmArea)
        {
            var relatedObjects = GetParentObjects(
                idCcu,
                input);

            relatedObjects = relatedObjects
                .Concat(
                    Enumerable.Repeat(
                        new IdAndObjectType(
                            input.IdInput,
                            ObjectType.Input),
                        1))
                .Concat(
                    Enumerable.Repeat(
                        new IdAndObjectType(
                            idAlarmArea,
                            ObjectType.AlarmArea),
                        1));

            return relatedObjects;
        }

        public override PresentationGroup GetPresentationGroup()
        {
            var alarmAreaIdAndObjectType =
                ServerAlarmCore.Alarm.AlarmKey.ExtendedObjects != null
                    ? ServerAlarmCore.Alarm.AlarmKey.ExtendedObjects.FirstOrDefault()
                    : null;

            if (alarmAreaIdAndObjectType != null)
            {
                var alarmArea = AlarmAreas.Singleton.GetById(alarmAreaIdAndObjectType.Id);

                if (alarmArea != null && alarmArea.SensorTamperAlarmPresentationGroup != null)
                    return alarmArea.SensorTamperAlarmPresentationGroup;
            }

            return DevicesAlarmSettings.Singleton.SensorTamperAlarmPresentationGroup();
        }
    }
}
