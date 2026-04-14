using System;
using System.Collections.Generic;
using System.Net;
using Contal.CatCom;
using Contal.Cgp.Globals;

namespace Contal.Cgp.NCAS.CCU.Alarms
{
    public class CatComClientEventHandler : ICatComClientEventHandler
    {
        public void CatUnreachable(
            IPEndPoint remoteEndPoint,
            Exception error,
            CatAlarm alarm)
        {
            var referencedAlarm = alarm != null
                ? alarm.Tag as Alarm
                : null;

            var ccu = Ccus.Singleton.GetCCU();

            if (ccu == null)
                return;

            if (referencedAlarm != null)
            {
                AlarmsManager.Singleton.AddAlarm(
                    new CcuCatUnreachableAlarm(
                        ccu.IdCCU,
                        remoteEndPoint.Address.ToString(),
                        AlarmState.Normal),
                    referencedAlarm);
            }

            AlarmsManager.Singleton.AddAlarm(
                new CcuCatUnreachableAlarm(
                    ccu.IdCCU,
                    remoteEndPoint.Address.ToString(),
                    AlarmState.Alarm));
        }

        public void CatReachable(IPEndPoint remoteEndPoint)
        {
            var ccu = Ccus.Singleton.GetCCU();

            if (ccu == null)
                return;

            AlarmsManager.Singleton.StopAlarm(
                CcuCatUnreachableAlarm.CreateAlarmKey(
                    ccu.IdCCU,
                    remoteEndPoint.Address.ToString()));
        }

        public void TransferToArcTimedOut(IPEndPoint remoteEndPoint, string arcName, CatAlarm alarm)
        {
            var referencedAlarm = alarm.Tag as Alarm;

            if (referencedAlarm == null)
                return;

            var ccu = Ccus.Singleton.GetCCU();

            if (ccu == null)
                return;

            AlarmsManager.Singleton.AddAlarm(
                new CcuTransferToArcTimedOutAlarm(
                    ccu.IdCCU,
                    remoteEndPoint.Address.ToString(),
                    arcName),
                referencedAlarm);
        }

        public void AllArcNamesReceived(IPEndPoint remoteEndPoint, IEnumerable<string> arcNames)
        {
            
        }

        public void ArcPresenceReceived(IPEndPoint remoteEndPoint, string arcName, bool arcExists)
        {
            
        }
    }
}