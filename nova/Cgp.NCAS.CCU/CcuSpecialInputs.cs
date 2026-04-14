using System;

using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.Alarms;
using Contal.Cgp.NCAS.CCU.EventParameters;
using Contal.Drivers.LPC3250;
using Contal.IwQuick;

namespace Contal.Cgp.NCAS.CCU
{
    public class CcuSpecialInputs
    {
        private const string CCU_SABOTAGE = "CCUSabotage";
        private const string CCU_PRIMARY_POWER_MISSING = "CCUPrimaryPowerMissing";
        private const string CCU_BATTERY_IS_LOW = "CCUBatteryIsLow";
        private const string CCU_EXT_FUSE = "CCUExtFuse";
        private const string UPS_OUTPUT_FUSE = "UpsOutputFuse";
        private const string UPS_BATTERY_FAULT = "UpsBatteryFault";
        private const string UPS_BATTERY_FUSE = "UpsBatteryFuse";
        private const string UPS_OVERTEMPERATURE = "UpsOvertemperature";
        private const string UPS_TAMPER_SABOTAGE = "UpsTamperSabotage";


        private static volatile CcuSpecialInputs _singleton = null;
        private static readonly object _syncRoot = new object();

        private bool _isInTamper;
        private Guid _outputTamper = Guid.Empty;

        private bool _checkPrimaryPowerMissing;
        private bool _isPrimaryPowerMissing;
        private Guid _outputPrimaryPowerMissing = Guid.Empty;

        private bool _isBatteryLow;
        private Guid _outputIsBatteryLow = Guid.Empty;

        private bool _isUpsOutputFuse;
        private Guid _outputUpsOutputFuse = Guid.Empty;

        private bool _isUpsBatteryFault;
        private Guid _outputUpsBatteryFault = Guid.Empty;

        private bool _isUpsBatteryFuse;
        private Guid _outputUpsBatteryFuse = Guid.Empty;

        private bool _isUpsOvertemperature;
        private Guid _outputUpsOvertemperature = Guid.Empty;

        private bool _isUpsTamperSabotage;
        private Guid _outputUpsTamperSabotage = Guid.Empty;

        private bool _isFuseOnExtensionBoard;
        private Guid _otuputIsFuseOnExtensionBoard = Guid.Empty;

        private bool _isTamperInitialized;

        public static CcuSpecialInputs Singleton
        {
            get
            {
                if (null == _singleton)
                    lock (_syncRoot)
                    {
                        if (null == _singleton)
                            _singleton = new CcuSpecialInputs();
                    }

                return _singleton;
            }
        }

        public bool Tamper
        {
            get { return _isInTamper; }
        }

        public void RegisterSpecialInputChanged()
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "void CcuSpecialInputs.RegisterSpecialInputChanged()");
            IOControl.SpecialInputChanged += IOControlSpecialInputChanged;
        }

        private void IOControlSpecialInputChanged(IOControl.SpecialInput specInput, bool param2)
        {
            switch (specInput)
            {
                case IOControl.SpecialInput.Tamper:
                    _isInTamper = param2;
                    Tampers.SendCCUTamper(true, _isInTamper);
                    ActuateOutputsBoundToTamper();
                    _isTamperInitialized = true;
                    break;
                case IOControl.SpecialInput.AcOk:
                    // After connecting UPS the first state come false. State true to this moment is not alarm.
                    if (!param2)
                        _checkPrimaryPowerMissing = true;

                    if (_checkPrimaryPowerMissing)
                        ActuateOutputBoundToPrimaryPowerMissing(param2);

                    break;
                case IOControl.SpecialInput.BatteryLow:
                    // Inverted. Alarm is false, true is normal state
                    var isBatteryLow = !param2;

                    // If first state is false (UPS is connected) then check AcOk
                    if (isBatteryLow && _isPrimaryPowerMissing == false)
                    {
                        _checkPrimaryPowerMissing = true;

                        if (IOControl.GetSpecialInputState(IOControl.SpecialInput.AcOk) == InputState.Alarm)
                        {
                            ActuateOutputBoundToPrimaryPowerMissing(true);
                        }
                    }

                    ActuateOutputBoundToBatteryIsLow(isBatteryLow);
                    break;
                case IOControl.SpecialInput.Fuse:
                    _isFuseOnExtensionBoard = param2;
                    ActuateOutputBoundToFuseOnExtensionBoard(true);
                    break;
            }
        }

        private void ActuateOutputsBoundToTamper()
        {
            if (_outputTamper != Guid.Empty)
            {
                DB.CCU ccu = Ccus.Singleton.GetCCU();

                if (ccu != null)
                {
                    if (_isInTamper)
                        Outputs.Singleton.On(
                            ccu.IdCCU + CCU_SABOTAGE, 
                            _outputTamper);
                    else
                        Outputs.Singleton.Off(
                            ccu.IdCCU + CCU_SABOTAGE, 
                            _outputTamper);
                }
            }
        }

        private bool _evaluatingAlarmsPrimaryPowerMissingBatteryIsLow = true;
        /// <summary>
        /// UPS monitor is connected and stop eveluting alarm states from special inputs
        /// </summary>
        public void StopEvaluatingAlarmsPrimaryPowerMissingBatteryIsLow()
        {
            _evaluatingAlarmsPrimaryPowerMissingBatteryIsLow = false;
            
            ActuateOutputBoundToPrimaryPowerMissing(false, true);
            ActuateOutputBoundToBatteryIsLow(false, true);
        }

        /// <summary>
        /// UPS monitor is disconected and start eveluting alarm states from special inputs
        /// </summary>
        public void StartEvaluatingAlarmsPrimaryPowerMissingBatteryIsLow()
        {
            _evaluatingAlarmsPrimaryPowerMissingBatteryIsLow = true;
            ActuateOutputBoundToPrimaryPowerMissing(false);
            ActuateOutputBoundToBatteryIsLow(false);
            ActuateOutputsBoundToUpsOutputFuse(false);
            ActuateOutputBoundToUpsBatteryFault(false);
            ActuateOutputBoundToUpsBatteryFuse(false);
            ActuateOutputBoundToUpsOvertemperature(false);
            ActuateOutputBoundToUpsTamperSabotage(false);
        }

        private void ActuateOutputBoundToPrimaryPowerMissing(bool isPrimaryPowerMissing)
        {
            ActuateOutputBoundToPrimaryPowerMissing(isPrimaryPowerMissing, false);
        }

        private void ActuateOutputBoundToPrimaryPowerMissing(bool isPrimaryPowerMissing, bool changeStateFromUPSMonitor)
        {
            if (!_evaluatingAlarmsPrimaryPowerMissingBatteryIsLow && !changeStateFromUPSMonitor)
                return;

            _isPrimaryPowerMissing = isPrimaryPowerMissing;

            DB.CCU ccu = Ccus.Singleton.GetCCU();

            if (ccu != null)
            {
                SendAlarm(
                    EventType.AlarmCCUPrimaryPowerMissing,
                    isPrimaryPowerMissing, ccu);

                ActuateOutputBoundToPrimaryPowerMissing(
                    ccu.IdCCU,
                    isPrimaryPowerMissing);
            }
        }

        private void ActuateOutputBoundToPrimaryPowerMissing(
            Guid idCcu,
            bool isPrimaryPowerMissing)
        {
            if (_outputPrimaryPowerMissing != Guid.Empty)
            {
                if (isPrimaryPowerMissing)
                    Outputs.Singleton.On(
                        idCcu + CCU_PRIMARY_POWER_MISSING,
                        _outputPrimaryPowerMissing);
                else
                    Outputs.Singleton.Off(
                        idCcu + CCU_PRIMARY_POWER_MISSING,
                        _outputPrimaryPowerMissing);
            }
        }

        /// <summary>
        /// Set state primary power missing from UPS Monitor
        /// </summary>
        /// <param name="isPrimaryPowerMissing"></param>
        public void ActuateOutputBoundToPrimaryPowerMissingFromUpsMonitor(bool isPrimaryPowerMissing)
        {
            ActuateOutputBoundToPrimaryPowerMissing(isPrimaryPowerMissing, true);
        }

        private void ActuateOutputBoundToBatteryIsLow(bool isBatteryLow)
        {
            ActuateOutputBoundToBatteryIsLow(isBatteryLow, false);
        }

        private void ActuateOutputBoundToBatteryIsLow(bool isBatteryLow, bool changeStateFromUPSMonitor)
        {
            if (!_evaluatingAlarmsPrimaryPowerMissingBatteryIsLow && !changeStateFromUPSMonitor)
                return;

            _isBatteryLow = isBatteryLow;

            DB.CCU ccu = Ccus.Singleton.GetCCU();

            if (ccu != null)
            {
                SendAlarm(
                    EventType.AlarmCCUBatteryIsLow,
                    isBatteryLow,
                    ccu);

                ActuateOutputBoundToBatteryIsLow(
                    ccu.IdCCU,
                    isBatteryLow);
            }
        }

        private void ActuateOutputBoundToBatteryIsLow(
            Guid idCcu,
            bool isBatteryLow)
        {
            if (_outputIsBatteryLow != Guid.Empty)
            {
                if (isBatteryLow)
                    Outputs.Singleton.On(
                        idCcu + CCU_BATTERY_IS_LOW,
                        _outputIsBatteryLow);
                else
                    Outputs.Singleton.Off(
                        idCcu + CCU_BATTERY_IS_LOW,
                        _outputIsBatteryLow);
            }
        }

        /// <summary>
        /// Set state battery is low from UPS Monitor
        /// </summary>
        /// <param name="isBatteryLow"></param>
        public void ActuateOutputBoundToBatteryIsLowFromUpsMonitor(bool isBatteryLow)
        {
            ActuateOutputBoundToBatteryIsLow(isBatteryLow, true);
        }


        public void ActuateOutputsBoundToUpsOutputFuse(bool isUpsOutputFuse)
        {
            _isUpsOutputFuse = isUpsOutputFuse;

            DB.CCU ccu = Ccus.Singleton.GetCCU();

            if (ccu != null)
            {
                SendAlarm(
                    EventType.AlarmCcuUpsOutputFuse,
                    isUpsOutputFuse,
                    ccu);

                ActuateOutputBoundToUpsOutputFuse(
                    ccu.IdCCU,
                    isUpsOutputFuse);
            }
        }

        private void ActuateOutputBoundToUpsOutputFuse(
            Guid idCcu,
            bool isUpsOutputFuse)
        {
            if (_outputUpsOutputFuse != Guid.Empty)
            {
                if (isUpsOutputFuse)
                    Outputs.Singleton.On(
                        idCcu + UPS_OUTPUT_FUSE,
                        _outputUpsOutputFuse);
                else
                    Outputs.Singleton.Off(
                        idCcu + UPS_OUTPUT_FUSE,
                        _outputUpsOutputFuse);
            }
        }

        public void ActuateOutputBoundToUpsBatteryFault(bool isUpsBatteryFault)
        {
            _isUpsBatteryFault = isUpsBatteryFault;

            DB.CCU ccu = Ccus.Singleton.GetCCU();

            if (ccu != null)
            {
                SendAlarm(
                    EventType.AlarmCcuUpsBatteryFault,
                    isUpsBatteryFault,
                    ccu);

                ActuateOutputBoundToUpsBatteryFault(
                    ccu.IdCCU,
                    isUpsBatteryFault);
            }
        }

        private void ActuateOutputBoundToUpsBatteryFault(
            Guid idCcu,
            bool isUpsBatteryFault)
        {
            if (_outputUpsBatteryFault != Guid.Empty)
            {
                if (isUpsBatteryFault)
                    Outputs.Singleton.On(
                        idCcu + UPS_BATTERY_FAULT,
                        _outputUpsBatteryFault);
                else
                    Outputs.Singleton.Off(
                        idCcu + UPS_BATTERY_FAULT,
                        _outputUpsBatteryFault);
            }
        }

        public void ActuateOutputBoundToUpsBatteryFuse(bool isUpsBatteryFuse)
        {
            _isUpsBatteryFuse = isUpsBatteryFuse;

            DB.CCU ccu = Ccus.Singleton.GetCCU();

            if (ccu != null)
            {
                SendAlarm(
                    EventType.AlarmCcuUpsBatteryFuse,
                    isUpsBatteryFuse,
                    ccu);

                ActuateOutputBoundToUpsBatteryFuse(
                    ccu.IdCCU,
                    isUpsBatteryFuse);
            }
        }

        private void ActuateOutputBoundToUpsBatteryFuse(
            Guid idCcu,
            bool isUpsBatteryFuse)
        {
            if (_outputUpsBatteryFuse != Guid.Empty)
            {
                if (isUpsBatteryFuse)
                    Outputs.Singleton.On(
                        idCcu + UPS_BATTERY_FUSE,
                        _outputUpsBatteryFuse);
                else
                    Outputs.Singleton.Off(
                        idCcu + UPS_BATTERY_FUSE,
                        _outputUpsBatteryFuse);
            }
        }

        public void ActuateOutputBoundToUpsOvertemperature(bool isUpsOvertemperature)
        {
            _isUpsOvertemperature = isUpsOvertemperature;

            DB.CCU ccu = Ccus.Singleton.GetCCU();

            if (ccu != null)
            {
                SendAlarm(
                    EventType.AlarmCcuUpsOvertemperature,
                    isUpsOvertemperature,
                    ccu);

                ActuateOutputBoundToUpsOvertemperature(
                    ccu.IdCCU,
                    isUpsOvertemperature);
            }
        }

        private void ActuateOutputBoundToUpsOvertemperature(
            Guid idCcu,
            bool isUpsOvertemperature)
        {
            if (_outputUpsOvertemperature != Guid.Empty)
            {
                if (isUpsOvertemperature)
                    Outputs.Singleton.On(
                        idCcu + UPS_OVERTEMPERATURE,
                        _outputUpsOvertemperature);
                else
                    Outputs.Singleton.Off(
                        idCcu + UPS_OVERTEMPERATURE,
                        _outputUpsOvertemperature);
            }
        }

        public void ActuateOutputBoundToUpsTamperSabotage(bool isUpsTamperSabotage)
        {
            _isUpsTamperSabotage = isUpsTamperSabotage;

            DB.CCU ccu = Ccus.Singleton.GetCCU();

            if (ccu != null)
            {
                SendAlarm(
                    EventType.AlarmCcuUpsTamperSabotage,
                    isUpsTamperSabotage,
                    ccu);

                ActuateOutputBoundToUpsTamperSabotage(
                    ccu.IdCCU,
                    isUpsTamperSabotage);
            }
        }

        private void ActuateOutputBoundToUpsTamperSabotage(
            Guid idCcu,
            bool isUpsTamperSabotage)
        {
            if (_outputUpsTamperSabotage != Guid.Empty)
            {
                if (isUpsTamperSabotage)
                    Outputs.Singleton.On(
                        idCcu + UPS_TAMPER_SABOTAGE,
                        _outputUpsTamperSabotage);
                else
                    Outputs.Singleton.Off(
                        idCcu + UPS_TAMPER_SABOTAGE,
                        _outputUpsTamperSabotage);
            }
        }

        private void ActuateOutputBoundToFuseOnExtensionBoard(bool sendAlarm)
        {
            DB.CCU ccu = Ccus.Singleton.GetCCU();

            if (ccu != null)
            {
                if (sendAlarm)
                    SendAlarm(EventType.AlarmCCUExtFuse, _isFuseOnExtensionBoard, ccu);

                if (_otuputIsFuseOnExtensionBoard != Guid.Empty)
                {
                    if (_isFuseOnExtensionBoard)
                        Outputs.Singleton.On(
                            ccu.IdCCU + CCU_EXT_FUSE,
                            _otuputIsFuseOnExtensionBoard);
                    else
                        Outputs.Singleton.Off(
                            ccu.IdCCU + CCU_EXT_FUSE, 
                            _otuputIsFuseOnExtensionBoard);
                }
            }
        }

        public void SetSpecialOutputs()
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "void CcuSpecialInputs.SetSpecialOutputs()");
            DB.CCU ccu = Ccus.Singleton.GetCCU();

            if (ccu != null)
            {
                if (ccu.GuidOutputTamper != _outputTamper)
                {
                    if (_outputTamper != Guid.Empty && _isInTamper)
                    {
                        Outputs.Singleton.Off(
                            ccu.IdCCU + CCU_SABOTAGE, 
                            _outputTamper);
                    }

                    _outputTamper = ccu.GuidOutputTamper;
                    ActuateOutputsBoundToTamper();
                }

                if (ccu.GuidOutputPrimaryPowerMissing != _outputPrimaryPowerMissing)
                {
                    if (_outputPrimaryPowerMissing != Guid.Empty && _isPrimaryPowerMissing)
                        ActuateOutputBoundToPrimaryPowerMissing(
                            ccu.IdCCU,
                            false);

                    _outputPrimaryPowerMissing = ccu.GuidOutputPrimaryPowerMissing;

                    if (_isPrimaryPowerMissing)
                        ActuateOutputBoundToPrimaryPowerMissing(
                            ccu.IdCCU,
                            true);
                }

                if (ccu.GuidOutputBatteryIsLow != _outputIsBatteryLow)
                {
                    if (_outputIsBatteryLow != Guid.Empty && _isBatteryLow)
                        ActuateOutputBoundToBatteryIsLow(
                            ccu.IdCCU,
                            false);

                    _outputIsBatteryLow = ccu.GuidOutputBatteryIsLow;

                    if (_isBatteryLow)
                        ActuateOutputBoundToBatteryIsLow(
                            ccu.IdCCU,
                            true);
                }

                if (ccu.GuidOutputUpsOutputFuse != _outputUpsOutputFuse)
                {
                    if (_outputUpsOutputFuse != Guid.Empty && _isUpsOutputFuse)
                        ActuateOutputBoundToUpsOutputFuse(
                            ccu.IdCCU,
                            false);

                    _outputUpsOutputFuse = ccu.GuidOutputUpsOutputFuse;

                    if (_isUpsOutputFuse)
                        ActuateOutputBoundToUpsOutputFuse(
                            ccu.IdCCU,
                            true);
                }

                if (ccu.GuidOutputUpsBatteryFault != _outputUpsBatteryFault)
                {
                    if (_outputUpsBatteryFault != Guid.Empty && _isUpsBatteryFault)
                        ActuateOutputBoundToUpsBatteryFault(
                            ccu.IdCCU,
                            false);

                    _outputUpsBatteryFault = ccu.GuidOutputUpsBatteryFault;

                    if (_isUpsBatteryFault)
                        ActuateOutputBoundToUpsBatteryFault(
                            ccu.IdCCU,
                            true);
                }

                if (ccu.GuidOutputUpsBatteryFuse != _outputUpsBatteryFuse)
                {
                    if (_outputUpsBatteryFuse != Guid.Empty && _isUpsBatteryFuse)
                        ActuateOutputBoundToUpsBatteryFuse(
                            ccu.IdCCU,
                            true);

                    _outputUpsBatteryFuse = ccu.GuidOutputUpsBatteryFuse;

                    if (_isUpsBatteryFuse)
                        ActuateOutputBoundToUpsBatteryFuse(
                            ccu.IdCCU,
                            true);
                }

                if (ccu.GuidOutputUpsOvertemperature != _outputUpsOvertemperature)
                {
                    if (_outputUpsOvertemperature != Guid.Empty && _isUpsOvertemperature)
                        ActuateOutputBoundToUpsOvertemperature(
                            ccu.IdCCU,
                            false);

                    _outputUpsOvertemperature = ccu.GuidOutputUpsOvertemperature;

                    if (_isUpsOvertemperature)
                        ActuateOutputBoundToUpsOvertemperature(
                            ccu.IdCCU,
                            true);
                }

                if (ccu.GuidOutputUpsTamperSabotage != _outputUpsTamperSabotage)
                {
                    if (_outputUpsTamperSabotage != Guid.Empty && _isUpsTamperSabotage)
                        ActuateOutputBoundToUpsTamperSabotage(
                            ccu.IdCCU,
                            false);

                    _outputUpsTamperSabotage = ccu.GuidOutputUpsTamperSabotage;

                    if (_isUpsTamperSabotage)
                        ActuateOutputBoundToUpsTamperSabotage(
                            ccu.IdCCU,
                            true);
                }

                if (ccu.GuidOutputFuseOnExtensionBoard != _otuputIsFuseOnExtensionBoard)
                {
                    if (_otuputIsFuseOnExtensionBoard != Guid.Empty && _isFuseOnExtensionBoard)
                    {
                        Outputs.Singleton.Off(
                            ccu.IdCCU + CCU_EXT_FUSE, 
                            _otuputIsFuseOnExtensionBoard);
                    }

                    _otuputIsFuseOnExtensionBoard = ccu.GuidOutputFuseOnExtensionBoard;
                }

                ActuateOutputBoundToFuseOnExtensionBoard(false);
            }
        }

        private static void SendAlarm(EventType eventType, bool isOn, DB.CCU ccu)
        {
            if (ccu == null)
                return;

            switch (eventType)
            {
                case EventType.AlarmCCUPrimaryPowerMissing:
                    CcuPrimaryPowerMissingAlarm.Update(
                        ccu.IdCCU,
                        isOn);

                    if (BlockedAlarmsManager.Singleton.ProcessEvent(
                        AlarmType.CCU_PrimaryPowerMissing,
                        new IdAndObjectType(
                            ccu.IdCCU,
                            ObjectType.CCU)))
                    {
                        Events.ProcessEvent(
                            new EventAlarmCcuPrimaryPowerMissing(isOn));
                    }

                    return;

                case EventType.AlarmCCUBatteryIsLow:
                    CcuBatteryLowAlarm.Update(
                        ccu.IdCCU,
                        isOn);

                    if (BlockedAlarmsManager.Singleton.ProcessEvent(
                        AlarmType.CCU_BatteryLow,
                        new IdAndObjectType(
                            ccu.IdCCU,
                            ObjectType.CCU)))
                    {
                        Events.ProcessEvent(
                            new EventAlarmCcuBatteryIsLow(isOn));
                    }

                    return;

                case EventType.AlarmCcuUpsOutputFuse:
                    CcuUpsOutputFuseAlarm.Update(
                        ccu.IdCCU,
                        isOn);

                    if (BlockedAlarmsManager.Singleton.ProcessEvent(
                        AlarmType.Ccu_Ups_OutputFuse,
                        new IdAndObjectType(
                            ccu.IdCCU,
                            ObjectType.CCU)))
                    {
                        Events.ProcessEvent(
                            new EventAlarmCcuUpsOutputFuse(isOn));
                    }

                    return;

                case EventType.AlarmCcuUpsBatteryFault:
                    CcuUpsBatteryFaultAlarm.Update(
                        ccu.IdCCU,
                        isOn);

                    if (BlockedAlarmsManager.Singleton.ProcessEvent(
                        AlarmType.Ccu_Ups_BatteryFault,
                        new IdAndObjectType(
                            ccu.IdCCU,
                            ObjectType.CCU)))
                    {
                        Events.ProcessEvent(
                            new EventAlarmCcuUpsBatteryFault(isOn));
                    }

                    return;

                case EventType.AlarmCcuUpsBatteryFuse:
                    CcuUpsBatteryFuseAlarm.Update(
                        ccu.IdCCU,
                        isOn);

                    if (BlockedAlarmsManager.Singleton.ProcessEvent(
                        AlarmType.Ccu_Ups_BatteryFuse,
                        new IdAndObjectType(
                            ccu.IdCCU,
                            ObjectType.CCU)))
                    {
                        Events.ProcessEvent(
                            new EventAlarmCcuUpsBatteryFuse(isOn));
                    }

                    return;

                case EventType.AlarmCcuUpsOvertemperature:
                    CcuUpsOvertemperatureAlarm.Update(
                        ccu.IdCCU,
                        isOn);

                    if (BlockedAlarmsManager.Singleton.ProcessEvent(
                        AlarmType.Ccu_Ups_Overtemperature,
                        new IdAndObjectType(
                            ccu.IdCCU,
                            ObjectType.CCU)))
                    {
                        Events.ProcessEvent(
                            new EventAlarmCcuUpsOvertemperature(isOn));
                    }

                    return;

                case EventType.AlarmCcuUpsTamperSabotage:
                    CcuUpsTamperSabotageAlarm.Update(
                        ccu.IdCCU,
                        isOn);

                    if (BlockedAlarmsManager.Singleton.ProcessEvent(
                        AlarmType.Ccu_Ups_TamperSabotage,
                        new IdAndObjectType(
                            ccu.IdCCU,
                            ObjectType.CCU)))
                    {
                        Events.ProcessEvent(
                            new EventAlarmCcuUpsTamperSabotage(isOn));
                    }

                    return;

                case EventType.AlarmCCUExtFuse:
                    CcuExtFuseAlarm.Update(
                        ccu.IdCCU,
                        isOn);

                    if (BlockedAlarmsManager.Singleton.ProcessEvent(
                        AlarmType.CCU_ExtFuse,
                        new IdAndObjectType(
                            ccu.IdCCU,
                            ObjectType.CCU)))
                    {
                        Events.ProcessEvent(
                            new EventAlarmCcuExtFuse(isOn));
                    }

                    return;
            }
        }

        public void SendCcuTamper()
        {
            if (!_isTamperInitialized)
                return;

            Tampers.SendCCUTamper(
                true,
                _isInTamper);
        }
    }
}
