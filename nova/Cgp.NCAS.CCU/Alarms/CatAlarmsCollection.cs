using System;
using System.Collections.Generic;
using System.Text;
using Contal.CatCom;
using Contal.IwQuick.CrossPlatform.Data;
using Contal.IwQuick.Sys;

namespace Contal.Cgp.NCAS.CCU.Alarms
{
    internal class CatAlarmsCollection : IProcessingQueueRequest
    {
        private readonly CatAlarmsManager _catAlarmsManager;
        private readonly ICollection<CatAlarm> _catAlarms;

        public CatAlarmsCollection(CatAlarmsManager catAlarmsManager, ICollection<CatAlarm> catAlarms)
        {
            _catAlarmsManager = catAlarmsManager;
            _catAlarms = catAlarms;
        }

        public void Execute()
        {
            if (_catAlarms == null)
                return;

            var ipAddress = _catAlarmsManager.GetAlarmTransmitterIpAddress();

            if (ipAddress == null)
                return;

            foreach (var catAlarm in _catAlarms)
            {
                try
                {
#if DEBUG
                    var stringArcList = new StringBuilder();

                    foreach (var arcName in catAlarm.ArcList)
                    {
                        if (stringArcList.Length > 0)
                            stringArcList.Append(", ");

                        stringArcList.Append(arcName);
                    }

                    Console.WriteLine(
                        string.Format(
                            "Sending CAT ALARM: Event code: {0}, Section: {1}, Transmitter area: {2}, Alarm state: {3}, Arc list: {4}, Additional info: {5}",
                            catAlarm.AlarmEventCode,
                            catAlarm.Section,
                            catAlarm.TransmitterArea,
                            catAlarm.AlarmState,
                            stringArcList,
                            catAlarm.AdditionalInfo));
#endif

                    CatComClient.Singleton.SendAlarm(
                        ipAddress,
                        catAlarm);
                }
                catch (Exception error)
                {
                    HandledExceptionAdapter.Examine(error);
                }
            }
        }

        public void OnError(Exception error)
        {
            HandledExceptionAdapter.Examine(error);
        }
    }
}