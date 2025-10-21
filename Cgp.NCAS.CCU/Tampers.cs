using System;
using System.Collections.Generic;

using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.Alarms;
using Contal.IwQuick;

namespace Contal.Cgp.NCAS.CCU
{
    public class Tampers
    {
        private static bool SendTamper(ObjectType objectType, Guid objectGuid)
        {
            AlarmType alarmType;

            switch (objectType)
            {
                case ObjectType.CCU:
                    alarmType = AlarmType.CCU_TamperSabotage;
                    break;

                case ObjectType.DCU:
                    alarmType = AlarmType.DCU_TamperSabotage;
                    break;

                case ObjectType.CardReader:
                    alarmType = AlarmType.CardReader_TamperSabotage;
                    break;

                default:
                    return false;
            }

            return BlockedAlarmsManager.Singleton.ProcessEvent(
                alarmType,
                new IdAndObjectType(
                    objectGuid,
                    objectType));
        }

        public static void SendAllTampers()
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL, 
                "void Tampers.SendAllTampers()");

            SendCCUTamper(
                false, 
                CcuSpecialInputs.Singleton.Tamper);

            DCUs.Singleton.SendDcuTamperStates();

            SendTamperStateForCardReaders();

            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL, 
                "All tampers sended to CCU");
        }

        public static void SendTamperStateForCardReaders()
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => "void Tampers.SendTamperStateForCardReaders()");

            var crTampers = 
                CardReaders.Singleton.GetCardReaderTamperState();

            if (crTampers == null)
                return;

            foreach (KeyValuePair<Guid, bool> cr in crTampers)
                SendCardReaderTamper(
                    false,
                    cr.Value,
                    cr.Key);
        }

        #region Sending tampers to NCAS Server

        public static void SendCCUTamper(bool tamperChanged, bool isTamper)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () =>
                    string.Format("void Tampers.SendCCUTamper(bool isTamper): [{0}]",
                        Log.GetStringFromParameters(isTamper)));

            DB.CCU ccu = Ccus.Singleton.GetCCU();

            if (ccu != null)
            {
                CcuCore.Singleton.SendTamper(
                    tamperChanged,
                    isTamper,
                    ccu.IdCCU,
                    ObjectType.CCU,
                    SendTamper(
                        ObjectType.CCU,
                        ccu.IdCCU));
            }

            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "Sended CCU tamper with state: " + isTamper);
        }

        public static void SendDcuTamper(
            bool tamperChanged,
            bool isTamper,
            Guid dcuGuid)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL,
                () =>
                    string.Format("void Tampers.SendDCUTamper(bool isTamper, Guid dcuGuid): [{0}]",
                        Log.GetStringFromParameters(isTamper, dcuGuid)));

            CcuCore.Singleton.SendTamper(
                tamperChanged,
                isTamper,
                dcuGuid,
                ObjectType.DCU,
                SendTamper(
                    ObjectType.DCU,
                    dcuGuid));

            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL, 
                () => "Sended DCU tamper with state: " + isTamper);
        }

        public static void SendCardReaderTamper(
            bool tamperChanged,
            bool isTamper,
            Guid crGuid)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL,
                () =>
                    string.Format("void Tampers.SendCardReaderTamper(bool isTamper, Guid crGuid): [{0}]",
                        Log.GetStringFromParameters(isTamper, crGuid)));

            if (crGuid == Guid.Empty)
                return;

            CcuCore.Singleton.SendTamper(
                tamperChanged,
                isTamper,
                crGuid,
                ObjectType.CardReader,
                SendTamper(
                    ObjectType.CardReader,
                    crGuid));

            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL, 
                () => string.Format(
                    "Sended Card Reader tamper with state: {0}",
                    isTamper));
        }

        #endregion
    }
}
