using System;
using Contal.Cgp.Globals;

using Contal.Cgp.Globals.PlatformPC;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.NCAS.Server.DB;
using Contal.Cgp.Server.Alarms;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.Server.ServerAlarms
{
    [LwSerialize(802)]
    public class CcuUnconfiguredServerAlarm : ServerAlarm
    {
        private CcuUnconfiguredServerAlarm()
        {
            
        }

        private CcuUnconfiguredServerAlarm(
            CCU ccu,
            string name,
            string description)
            : base(
                new ServerAlarmCore(
                    new Alarm(
                        CreateAlarmKey(ccu.IdCCU),
                        AlarmState.Normal),
                    name,
                    ccu.Name,
                    description))
        {

        }

        public static void AddCcuUnconfiguredAlarm(CCU ccu)
        {
            AlarmsManager.Singleton.AddAlarm(
                new CcuUnconfiguredServerAlarm(
                    ccu,
                    string.Format(
                        "{0} : {1}",
                        AlarmType.CCU_Unconfigured,
                        ccu.ToString()),
                    string.Format(
                        "New unconfigure CCU: {0}",
                        ccu.Name)));
        }

        public static void AddInsufficientCcuComboLicenceCountAlarm(CCU ccu)
        {
            AlarmsManager.Singleton.AddAlarm(
                new CcuUnconfiguredServerAlarm(
                    ccu,
                    NCASServer.Singleton.LocalizationHelper.GetString("InsufficientCCUComboLicenceCount"),
                    string.Empty));
        }

        public static void RemoveAlarm(Guid idCcu)
        {
            AlarmsManager.Singleton.TryRunOnAlarmsOwner(
                serverAlarmsOwner =>
                    serverAlarmsOwner.RemoveAlarm(CreateAlarmKey(idCcu)));
        }

        private static AlarmKey CreateAlarmKey(Guid idCcu)
        {
            return new AlarmKey(
                AlarmType.CCU_Unconfigured,
                new IdAndObjectType(
                    idCcu,
                    ObjectType.CCU));
        }

        public override PresentationGroup GetPresentationGroup()
        {
            return DevicesAlarmSettings.Singleton.AlarmCcuUnconfiguredPresentationGroup();
        }
    }
}
