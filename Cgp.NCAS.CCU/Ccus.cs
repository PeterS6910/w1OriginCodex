using System;
using System.Linq;

using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.Alarms;
using Contal.Cgp.NCAS.CCU.DB;
using Contal.Cgp.NCAS.Definitions;
using Contal.Drivers.LPC3250;
using Contal.IwQuick;
using Contal.IwQuick.Sys;

namespace Contal.Cgp.NCAS.CCU
{
    public class Ccus
    {
        private const int MinCrEventlogCount = 10;
        
        Guid? _lastIdCCU;
        //string _lastTimeZoneInfo;
        //int _lastTimeShift;
        double? _shortNormal;
        double? _normalAlarm;
        double? _alarmBreak;
        //int _lastPortBaudRateCom1;
        //int _lastPortBaudRateCom4;
        private Guid _watchdogOutput;

        private static volatile Ccus _singleton;
        private static readonly object _syncRoot = new object();

        private volatile bool _isWatchdogOutputOn;
        private volatile object _lockWatchdogOuptut = new object();

        private int _crEventlogMaxCounts = 100;
        private int _crTimeForMarkAlarArea;

        public static Ccus Singleton
        {
            get
            {
                if (null == _singleton)
                    lock (_syncRoot)
                    {
                        if (_singleton == null)
                            _singleton = new Ccus();
                    }

                return _singleton;
            }
        }

        public int GetCrEventLogSize()
        {
            return _crEventlogMaxCounts;
        }

        public int GetTimeForMarkAlarArea()
        {
            return _crTimeForMarkAlarArea;
        }

        public uint[] GetBsiLevels()
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => "void Inputs.GetBSILevels(out int level1, out int level2, out int level3)");

            return
                new[]
                {
                    (uint)IOControl.VoltsToLevel(
                            _shortNormal ?? (double)NCASConstants.DefaultBalancingLevels.ToLevel1),
                    (uint)IOControl.VoltsToLevel(
                            _normalAlarm ?? (double)NCASConstants.DefaultBalancingLevels.ToLevel2),
                    (uint)IOControl.VoltsToLevel(
                            _alarmBreak ?? (double)NCASConstants.DefaultBalancingLevels.ToLevel3)
                };
        }

        public void Configure()
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => "void CCUs.Configure()");

            DB.CCU ccu = GetCCU();

            if (ccu == null)
                return;

            Configure(ccu);
            ApplyHwSetup(ccu);
        }

        public bool IsCcu0()
        {
            switch ((MainBoardVariant) MainBoard.Variant)
            {
                case MainBoardVariant.CCU0_RS485:
                case MainBoardVariant.CCU0_ECHELON:
                    return true;
                default:
                    return false;
            }
        }

        private void ApplyHwSetup(DB.CCU ccu)
        {
            if (_shortNormal == ccu.ShortNormal
                && _normalAlarm == ccu.NormalAlarm
                && _alarmBreak == ccu.AlarmBreak)
            {
                return;
            }

            _shortNormal = ccu.ShortNormal;
            _normalAlarm = ccu.NormalAlarm;
            _alarmBreak = ccu.AlarmBreak;

            uint[] bsiLevels = GetBsiLevels();

            if (IsCcu0())
                DCUs.Singleton.SetDcuInputBsiLevels(bsiLevels);
            else
                Inputs.Singleton.SetCcuInputsBsiLevels(bsiLevels);
        }

        private void Configure(DB.CCU ccu)
        {
            if (_lastIdCCU != ccu.IdCCU)
            {
                _lastIdCCU = ccu.IdCCU;

                CcuSpecialInputs.Singleton.SendCcuTamper();
            }

            _watchdogOutput = ccu.GuidWatchdogOutput;

            _crEventlogMaxCounts = ccu.CrEventlogSize > MinCrEventlogCount
                ? ccu.CrEventlogSize
                : MinCrEventlogCount;

            _crTimeForMarkAlarArea = ccu.CrLastEventTimeForMarkAlarmArea;

            var idAndObjectType =
                new IdAndObjectType(
                    ccu.IdCCU,
                    ObjectType.CCU);

            BlockedAlarmsManager.Singleton.ConfigureSpecificBlockingForObject(
                AlarmType.CCU_TamperSabotage,
                idAndObjectType,
                ccu.AlarmTamper,
                ccu.BlockAlarmTamper,
                ccu.ObjBlockAlarmTamperId,
                ccu.ObjBlockAlarmTamperObjectType,
                ccu.EventlogDuringBlockAlarmTamper);

            BlockedAlarmsManager.Singleton.ConfigureSpecificBlockingForObject(
                AlarmType.CCU_PrimaryPowerMissing,
                idAndObjectType,
                ccu.AlarmPrimaryPowerMissing,
                ccu.BlockAlarmPrimaryPowerMissing,
                ccu.ObjBlockAlarmPrimaryPowerMissingId,
                ccu.ObjBlockAlarmPrimaryPowerMissingObjectType,
                ccu.EventlogDuringBlockAlarmPrimaryPowerMissing);

            BlockedAlarmsManager.Singleton.ConfigureSpecificBlockingForObject(
                AlarmType.CCU_BatteryLow,
                idAndObjectType,
                ccu.AlarmBatteryIsLow,
                ccu.BlockAlarmBatteryIsLow,
                ccu.ObjBlockAlarmBatteryIsLowId,
                ccu.ObjBlockAlarmBatteryIsLowObjectType,
                ccu.EventlogDuringBlockAlarmBatteryIsLow);

            BlockedAlarmsManager.Singleton.ConfigureSpecificBlockingForObject(
                AlarmType.Ccu_Ups_OutputFuse,
                idAndObjectType,
                ccu.AlarmUpsOutputFuse,
                ccu.BlockAlarmUpsOutputFuse,
                ccu.ObjBlockAlarmUpsOutputFuseId,
                ccu.ObjBlockAlarmUpsOutputFuseObjectType,
                ccu.EventlogDuringBlockAlarmUpsOutputFuse);

            BlockedAlarmsManager.Singleton.ConfigureSpecificBlockingForObject(
                AlarmType.Ccu_Ups_BatteryFault,
                idAndObjectType,
                ccu.AlarmUpsBatteryFault,
                ccu.BlockAlarmUpsBatteryFault,
                ccu.ObjBlockAlarmUpsBatteryFaultId,
                ccu.ObjBlockAlarmUpsBatteryFaultObjectType,
                ccu.EventlogDuringBlockAlarmUpsBatteryFault);

            BlockedAlarmsManager.Singleton.ConfigureSpecificBlockingForObject(
                AlarmType.Ccu_Ups_BatteryFuse,
                idAndObjectType,
                ccu.AlarmUpsBatteryFuse,
                ccu.BlockAlarmUpsBatteryFuse,
                ccu.ObjBlockAlarmUpsBatteryFuseId,
                ccu.ObjBlockAlarmUpsBatteryFuseObjectType,
                ccu.EventlogDuringBlockAlarmUpsBatteryFuse);

            BlockedAlarmsManager.Singleton.ConfigureSpecificBlockingForObject(
                AlarmType.Ccu_Ups_Overtemperature,
                idAndObjectType,
                ccu.AlarmUpsOvertemperature,
                ccu.BlockAlarmUpsOvertemperature,
                ccu.ObjBlockAlarmUpsOvertemperatureId,
                ccu.ObjBlockAlarmUpsOvertemperatureObjectType,
                ccu.EventlogDuringBlockAlarmUpsOvertemperature);

            BlockedAlarmsManager.Singleton.ConfigureSpecificBlockingForObject(
                AlarmType.Ccu_Ups_TamperSabotage,
                idAndObjectType,
                ccu.AlarmUpsTamperSabotage,
                ccu.BlockAlarmUpsTamperSabotage,
                ccu.ObjBlockAlarmUpsTamperSabotageId,
                ccu.ObjBlockAlarmUpsTamperSabotageObjectType,
                ccu.EventlogDuringBlockAlarmUpsTamperSabotage);

            BlockedAlarmsManager.Singleton.ConfigureSpecificBlockingForObject(
                AlarmType.CCU_ExtFuse,
                idAndObjectType,
                ccu.AlarmFuseOnExtensionBoard,
                ccu.BlockAlarmFuseOnExtensionBoard,
                ccu.ObjBlockAlarmFuseOnExtensionBoardId,
                ccu.ObjBlockAlarmFuseOnExtensionBoardObjectType,
                ccu.EventlogDuringBlockAlarmFuseOnExtensionBoard);

            BlockedAlarmsManager.Singleton.ConfigureSpecificBlockingForObject(
                AlarmType.Ccu_CatUnreachable,
                idAndObjectType,
                ccu.AlarmCcuCatUnreachable,
                false,
                null,
                null,
                true);

            BlockedAlarmsManager.Singleton.ConfigureSpecificBlockingForObject(
                AlarmType.Ccu_TransferToArcTimedOut,
                idAndObjectType,
                ccu.AlarmCcuTransferToArcTimedOut,
                false,
                null,
                null,
                true);

            if (ccu.AlarmTypeAndIdAlarmArcs != null)
                CatAlarmsManager.Singleton.ConfigureSpecificAlarmArcs(
                    idAndObjectType,
                    ccu.AlarmTypeAndIdAlarmArcs);
            else
                CatAlarmsManager.Singleton.RemoveSpecificAlarmArcsConfiguration(
                    idAndObjectType);
        }

        public DB.CCU GetCCU()
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "DB.CCU Ccus.GetCCU()");
            try
            {
                var ccuGuids = Database.ConfigObjectsEngine.GetPrimaryKeysForObjectType(ObjectType.CCU);

                if (ccuGuids != null)
                {
                    Guid ccuId = ccuGuids.FirstOrDefault();

                    if (ccuId != Guid.Empty)
                    {
                        var result = Database.ConfigObjectsEngine.GetFromDatabase(ObjectType.CCU, ccuId) as DB.CCU;
                    
                        CcuCore.DebugLog.Info(
                            Log.NORMAL_LEVEL,
                            () => string.Format("DB.CCU Ccus.GetCCU return {0}", Log.GetStringFromParameters(result)));

                        return result;
                    }
                }
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }

            CcuCore.DebugLog.Warning(Log.LOW_LEVEL,"DB.CCU Ccus.GetCCU return null");
            return null;
        }

        public Guid? GetCcuId()
        {
            var ccuGuids = Database.ConfigObjectsEngine.GetPrimaryKeysForObjectType(ObjectType.CCU);

            if (ccuGuids == null) 
                return null;

            var ccuId = ccuGuids.FirstOrDefault();

            return ccuId != Guid.Empty
                ? ccuId
                : (Guid?)null;
        }

        public void WatchdogOutputSetToOff()
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "void Ccus.WatchdogOutputSetToOff()");

            lock (_lockWatchdogOuptut)
            {
                if (_watchdogOutput == Guid.Empty)
                    return;

                _isWatchdogOutputOn = false;

                Outputs.Singleton.Off(
                    OutputStateAndSettings.WatchdogGuid.ToString(), 
                    _watchdogOutput);
            }
        }

        public void WatchdogOutputSetToOn()
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "void Ccus.WatchdogOutputSetToOn()");

            lock (_lockWatchdogOuptut)
            {
                if (_watchdogOutput == Guid.Empty)
                    return;

                _isWatchdogOutputOn = true;

                Outputs.Singleton.On(
                    OutputStateAndSettings.WatchdogGuid.ToString(),
                    _watchdogOutput);
            }
        }

        public void WatchdogOutputActivator(Guid outputId)
        {
            lock (_lockWatchdogOuptut)
            {
                if (!_isWatchdogOutputOn)
                    return;

                if (_watchdogOutput == outputId)
                {
                    WatchdogOutputSetToOn();
                }
            }
        }

        public int GetIndex()
        {
            var ccu = GetCCU();

            return ccu != null
                ? ccu.IndexCCU
                : -1;
        }
    }
}
