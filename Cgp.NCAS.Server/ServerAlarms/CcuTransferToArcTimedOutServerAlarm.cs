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
    [LwSerialize(814)]
    public class CcuTransferToArcTimedOutServerAlarm : ServerAlarm
    {
        private CcuTransferToArcTimedOutServerAlarm()
        {

        }

        private CcuTransferToArcTimedOutServerAlarm(
            Guid idOwner,
            Alarm alarm,
            ICollection<IdAndObjectType> extendedObjects,
            string name,
            string parentObject,
            string description)
            : base(
                new ServerAlarmCore(
                    idOwner,
                    alarm,
                    null,
                    extendedObjects,
                    name,
                    parentObject,
                    description))
        {

        }

        public static CcuTransferToArcTimedOutServerAlarm CreateCcuCatUnreachableServerAlarm(
            CCU ccu,
            Alarm alarm,
            string alarmTransmitterIpAddress,
            string arcName)
        {
            ICollection<IdAndObjectType> extendedObjects;
            string name;
            string description;

            GetParameters(
                alarmTransmitterIpAddress,
                arcName,
                out extendedObjects,
                out name,
                out description);

            return new CcuTransferToArcTimedOutServerAlarm(
                ccu != null
                    ? ccu.IdCCU
                    : Guid.Empty,
                alarm,
                extendedObjects,
                name,
                ccu != null
                    ? ccu.Name
                    : "Contal Nova Server",
                description
                );
        }

        private static void GetParameters(
            string alarmTransmitterIpAddress,
            string arcName,
            out ICollection<IdAndObjectType> extendedObjects,
            out string name,
            out string description)
        {
            var alarmTransmitter = AlarmTransmitters.Singleton.GetAlarmTransmitterByIpAddress(alarmTransmitterIpAddress);

            var alarmArc = AlarmArcs.Singleton.GetAlarmArcByName(arcName);

            if (alarmTransmitter != null
                || alarmArc != null)
            {
                var enumerableExtenedObjects = alarmTransmitter != null
                    ? Enumerable.Repeat(
                        new IdAndObjectType(
                            alarmTransmitter.IdAlarmTransmitter,
                            ObjectType.AlarmTransmitter),
                        1)
                    : Enumerable.Empty<IdAndObjectType>();

                if (alarmArc != null)
                    enumerableExtenedObjects = enumerableExtenedObjects.Concat(
                        Enumerable.Repeat(
                            new IdAndObjectType(
                                alarmArc.IdAlarmArc,
                                ObjectType.AlarmArc),
                            1));

                extendedObjects = new List<IdAndObjectType>(enumerableExtenedObjects);
            }
            else
            {
                extendedObjects = null;
            }

            name = GetName(arcName);

            description = string.Format(
                "Alarm not trasmitted to ARC: {0}",
                arcName);
        }

        public static void AddAlarm(
            string alarmTransmitterIpAddress,
            string arcName,
            ServerAlarm referencedAlarm)
        {
            var alarm = new Alarm(
                new AlarmKey(
                    AlarmType.Ccu_TransferToArcTimedOut,
                    null,
                    Enumerable.Repeat(
                        new AlarmParameter(
                            ParameterType.CatIpAddress,
                            alarmTransmitterIpAddress),
                        1)
                        .Concat(Enumerable.Repeat(
                            new AlarmParameter(
                                ParameterType.ArcName,
                                arcName),
                            1))),
                AlarmState.Normal);

            AlarmsManager.Singleton.TryRunOnAlarmsOwner(
                serverAlarmsOwner =>
                    serverAlarmsOwner.AddAlarm(
                        CreateCcuCatUnreachableServerAlarm(
                            null,
                            alarm,
                            alarmTransmitterIpAddress,
                            arcName),
                        referencedAlarm));
        }

        public static string GetName(string arcName)
        {
            return string.Format(
                "{0} : {1}",
                AlarmType.Ccu_TransferToArcTimedOut,
                arcName);
        }

        public static IEnumerable<IdAndObjectType> GetRelatedObjects(
            Guid idCcu,
            AlarmTransmitter alarmTransmitter,
            AlarmArc alarmArc)
        {
            var relatedObjects = Enumerable.Repeat(
                new IdAndObjectType(
                    idCcu,
                    ObjectType.CCU),
                1);

            if (alarmTransmitter != null)
                relatedObjects = relatedObjects.Concat(
                    Enumerable.Repeat(
                        new IdAndObjectType(
                            alarmTransmitter.IdAlarmTransmitter,
                            ObjectType.AlarmTransmitter),
                        1));

            if (alarmArc != null)
                relatedObjects = relatedObjects.Concat(
                    Enumerable.Repeat(
                        new IdAndObjectType(
                            alarmArc.IdAlarmArc,
                            ObjectType.AlarmArc),
                        1));

            return relatedObjects;
        }

        public override PresentationGroup GetPresentationGroup()
        {
            var ccu = CCUs.Singleton.GetById(ServerAlarmCore.Alarm.AlarmKey.AlarmObject.Id);

            return ccu != null && ccu.AlarmCcuTransferToArcTimedOutPresentationGroup != null
                ? ccu.AlarmCcuTransferToArcTimedOutPresentationGroup
                : DevicesAlarmSettings.Singleton.AlarmCcuTransferToArcTimedOutPresentationGroup();
        }
    }
}
