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
    [LwSerialize(832)]
    public class AlarmAreaSetByOnOffObjectFailedServerAlarm : ServerAlarm
    {
        private AlarmAreaSetByOnOffObjectFailedServerAlarm()
        {
            
        }

        public AlarmAreaSetByOnOffObjectFailedServerAlarm(
            CCU ccu,
            AlarmArea alarmArea,
            IEnumerable<IdAndObjectType> parentObjectsForOnOffObject,
            Alarm alarm)
            : base(
                new ServerAlarmCore(
                    ccu.IdCCU,
                    alarm,
                    new LinkedList<IdAndObjectType>(
                        GetParentObjects(ccu.IdCCU)),
                    parentObjectsForOnOffObject != null
                        ? new LinkedList<IdAndObjectType>(parentObjectsForOnOffObject)
                        : null,
                    GetName(alarmArea),
                    ccu.Name,
                    string.Format(
                        "Set alarm area '{0}' by on/off object failed.",
                        alarmArea.ToString())))
        {

        }

        public static AlarmAreaSetByOnOffObjectFailedServerAlarm CreateAlarmAreaSetByOnOffObjectFailedServerAlarm(
            CCU ccu,
            AlarmArea alarmArea,
            IdAndObjectType onOffObject,
            Alarm alarm)
        {
            return new AlarmAreaSetByOnOffObjectFailedServerAlarm(
                ccu,
                alarmArea,
                GetParentObjectsForOnOffObject(onOffObject),
                alarm);
        }

        public static string GetName(AlarmArea alarmArea)
        {
            return string.Format(
                "{0} : {1}",
                AlarmType.AlarmArea_SetByOnOffObjectFailed,
                alarmArea.ToString());
        }

        private static IEnumerable<IdAndObjectType> GetParentObjects(
            Guid idCcu)
        {
            var parentObjects = Enumerable.Repeat(
                new IdAndObjectType(
                    idCcu,
                    ObjectType.CCU),
                1);

            return parentObjects;
        }

        private static IEnumerable<IdAndObjectType> GetParentObjectsForOnOffObject(IdAndObjectType onOffObject)
        {
            if (onOffObject.ObjectType == ObjectType.Input)
            {
                var input = Inputs.Singleton.GetById(onOffObject.Id);

                if (input == null
                    || input.DCU == null)
                {
                    return null;
                }

                return Enumerable.Repeat(
                    new IdAndObjectType(
                        input.DCU.IdDCU,
                        ObjectType.DCU),
                    1);
            }

            if (onOffObject.ObjectType == ObjectType.Output)
            {
                var output = Outputs.Singleton.GetById(onOffObject.Id);

                if (output == null
                    || output.DCU == null)
                {
                    return null;
                }

                return Enumerable.Repeat(
                    new IdAndObjectType(
                        output.DCU.IdDCU,
                        ObjectType.DCU),
                    1);
            }

            return null;
        }

        public static IEnumerable<IdAndObjectType> GetRelatedObjects(
            Guid idCcu,
            AlarmArea alarmArea,
            IdAndObjectType onOffObject)
        {
            var relatedObjects = GetParentObjects(idCcu);

            relatedObjects = relatedObjects.Concat(
                Enumerable.Repeat(
                    new IdAndObjectType(
                        alarmArea.IdAlarmArea,
                        ObjectType.AlarmArea),
                    1));

            var parentObjectsForOnOffObject = GetParentObjectsForOnOffObject(onOffObject);

            if (parentObjectsForOnOffObject != null)
                relatedObjects = relatedObjects.Concat(parentObjectsForOnOffObject);

            relatedObjects = relatedObjects.Concat(
                Enumerable.Repeat(
                    onOffObject,
                    1));

            return relatedObjects;
        }

        public override PresentationGroup GetPresentationGroup()
        {
            var alarmArea = AlarmAreas.Singleton.GetById(ServerAlarmCore.Alarm.AlarmKey.AlarmObject.Id);

            return alarmArea != null && alarmArea.AlarmAreaSetByOnOffObjectFailedPresentationGroup != null
                ? alarmArea.AlarmAreaSetByOnOffObjectFailedPresentationGroup
                : DevicesAlarmSettings.Singleton.AlarmAreaSetByOnOffObjectFailedPresentationGroup();
        }
    }
}
