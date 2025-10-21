using System;
using System.Net;

using Contal.CatCom;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.Alarms;
using Contal.Cgp.NCAS.CCU.DB;
using Contal.IwQuick;
using Contal.IwQuick.Sys;

namespace Contal.Cgp.NCAS.CCU
{
    public sealed class AlarmTransmitters : 
        ASingleton<AlarmTransmitters>,
        IDbObjectChangeListener<DB.AlarmTransmitter>
    {
        private IPAddress _ipAddress;

        private AlarmTransmitters()
            : base(null)
        {

        }

        public void ValidateCat(Guid alarmTransmitterId)
        {

            var alarmTransmitter = Database.ConfigObjectsEngine.GetFromDatabase(
                ObjectType.AlarmTransmitter,
                alarmTransmitterId) as AlarmTransmitter;

            if (alarmTransmitter == null)
                return;

            try
            {
                _ipAddress = IPAddress.Parse(alarmTransmitter.IpAddress);

                CatComClient.Singleton.ValidateCat(_ipAddress);
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        public void PrepareObjectUpdate(
            Guid idObject,
            AlarmTransmitter newObject)
        {
            if (_ipAddress != null)
            {
                CatComClient.Singleton.RemoveCatValidation(_ipAddress);

                var ccu = Ccus.Singleton.GetCCU();

                if (ccu != null)
                    AlarmsManager.Singleton.RemoveAlarm(
                        CcuCatUnreachableAlarm.CreateAlarmKey(
                            ccu.IdCCU,
                            _ipAddress.ToString()));
            }
        }

        public void OnObjectSaved(
            Guid idObject,
            AlarmTransmitter newObject)
        {
        }

        public void PrepareObjectDelete(Guid idObject)
        {
            if (_ipAddress != null)
            {
                CatComClient.Singleton.RemoveCatValidation(_ipAddress);

                var ccu = Ccus.Singleton.GetCCU();

                if (ccu != null)
                    AlarmsManager.Singleton.RemoveAlarm(
                        CcuCatUnreachableAlarm.CreateAlarmKey(
                            ccu.IdCCU,
                            _ipAddress.ToString()));
            }
        }
    }
}
