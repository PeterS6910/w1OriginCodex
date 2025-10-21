using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using Contal.Cgp.Globals;
using Contal.Cgp.Globals.PlatformPC;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.NCAS.Server.DB;
using Contal.Cgp.Server.Alarms;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.Server.ServerAlarms
{
    [LwSerialize(813)]
    public class CcuCatUnreachableServerAlarm : ServerAlarm
    {
        private CcuCatUnreachableServerAlarm()
        {

        }

        private CcuCatUnreachableServerAlarm(
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

        public static CcuCatUnreachableServerAlarm CreateCcuCatUnreachableServerAlarm(
            CCU ccu,
            Alarm alarm,
            string alarmTransmitterIpAddress)
        {
            ICollection<IdAndObjectType> extendedObjects;
            string name;
            string description;

            GetParameters(
                alarmTransmitterIpAddress,
                out extendedObjects,
                out name,
                out description);

            return new CcuCatUnreachableServerAlarm(
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
            out ICollection<IdAndObjectType> extendedObjects,
            out string name,
            out string description)
        {
            var alarmTransmitter = AlarmTransmitters.Singleton.GetAlarmTransmitterByIpAddress(alarmTransmitterIpAddress);

            extendedObjects = alarmTransmitter != null
                ? new LinkedList<IdAndObjectType>(
                    Enumerable.Repeat(
                        new IdAndObjectType(
                            alarmTransmitter.IdAlarmTransmitter,
                            ObjectType.AlarmTransmitter),
                        1))
                : null;

            var alarmTransmitterNameOrIpAddress = alarmTransmitter != null
                ? alarmTransmitter.ToString()
                : alarmTransmitterIpAddress;

            name = GetName(alarmTransmitterNameOrIpAddress);
            description = string.Format(
                "CAT offline: {0}",
                alarmTransmitterNameOrIpAddress);
        }

        public static void AddAlarm(
            string alarmTransmitterIpAddress,
            ServerAlarm referencedAlarm)
        {
            var alarm = new Alarm(
                CreateAlarmKey(alarmTransmitterIpAddress),
                referencedAlarm == null
                    ? AlarmState.Alarm
                    : AlarmState.Normal);

            AlarmsManager.Singleton.TryRunOnAlarmsOwner(
                serverAlarmsOwner =>
                    serverAlarmsOwner.AddAlarm(
                        CreateCcuCatUnreachableServerAlarm(
                            null,
                            alarm,
                            alarmTransmitterIpAddress),
                        referencedAlarm));
        }

        public static AlarmKey CreateAlarmKey(string alarmTransmitterIpAddress)
        {
            return new AlarmKey(
                AlarmType.Ccu_CatUnreachable,
                null,
                Enumerable.Repeat(
                    new AlarmParameter(
                        ParameterType.CatIpAddress,
                        alarmTransmitterIpAddress),
                    1));
        }

        public static void StopAlarm(string alarmTransmitterIpAddress)
        {
            AlarmsManager.Singleton.TryRunOnAlarmsOwner(
                serverAlarmsOwner =>
                    serverAlarmsOwner.StopAlarm(CreateAlarmKey(alarmTransmitterIpAddress)));
        }

        public static string GetName(string alarmTransmitterNameOrIpAddress)
        {
            return string.Format(
                "{0} : {1}",
                AlarmType.Ccu_CatUnreachable,
                alarmTransmitterNameOrIpAddress);
        }

        public static IEnumerable<IdAndObjectType> GetRelatedObjects(
            Guid idCcu,
            AlarmTransmitter alarmTransmitter)
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

            return relatedObjects;
        }

        public override PresentationGroup GetPresentationGroup()
        {
            if (ServerAlarmCore.Alarm.AlarmKey.AlarmObject == null)
                return DevicesAlarmSettings.Singleton.AlarmCcuCatUnreachablePresentationGroup();

            var ccu = CCUs.Singleton.GetById(ServerAlarmCore.Alarm.AlarmKey.AlarmObject.Id);

            return ccu != null && ccu.AlarmCcuCatUnreachablePresentationGroup != null
                ? ccu.AlarmCcuCatUnreachablePresentationGroup
                : DevicesAlarmSettings.Singleton.AlarmCcuCatUnreachablePresentationGroup();
        }
    }
}
