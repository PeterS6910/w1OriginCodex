using System;
using Contal.Cgp.NCAS.CCU.EventParameters;
using Contal.IwQuick;
using Contal.IwQuick.Net;
using Contal.IwQuick.Data;
using System.IO.Ports;

using Contal.IwQuick.Sys;
using Contal.Drivers.LPC3250;
using Contal.Cgp.NCAS.Definitions;

namespace Contal.Cgp.NCAS.CCU
{
    public class UPSMonitor
    {
        public const string IMPLICIT_CAT12CE_UPS_SERIAL_PORT = "COM4";
        public const string IMPLICIT_CCUx_UPS_SERIAL_PORT = "COM3";
        SimpleSerialPort _serialPort;
        UpsProto2750 _upsProto = new UpsProto2750();
        private bool _sendUpsMonitorMsg = false;
        private static UPSMonitor _singleton = null;
        private static object _syncRoot = new object();
        TUpsOnlineState _lastOnlineState;
        public bool SendUpsMonitorMsg 
        {
            get { return _sendUpsMonitorMsg; }
            set 
            {
                _sendUpsMonitorMsg = value; 
                if (value)
                {
                    Events.ProcessEvent(
                        new EventUpsOnlineStateChanged(
                            (byte) _lastOnlineState));
                }
            }
        }

        public static UPSMonitor Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (null == _singleton)
                            _singleton = new UPSMonitor();
                    }

                return _singleton;
            }
        }

        public void InitUpsMonitor()
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "void UPSMonitorInitUpsMonitor()");
            try
            {
                _upsProto.OnlineStateChanged += new DUpsOnlineStateChanged(_cUpsProto_OnlineStateChanged);
                _upsProto.AlarmStateChanged += new DUpsAlarmChanged(_cUpsProto_AlarmStateChanged);
                _upsProto.ValuesChanged += new DUpsValuesChanged(_cUpsProto_ValuesChanged);

                _serialPort = new SimpleSerialPort(true);

                switch ((MainBoardVariant)MainBoard.Variant)
                {
                    case MainBoardVariant.CAT12CE:
                        _serialPort.PortName = IMPLICIT_CAT12CE_UPS_SERIAL_PORT;
                        break;
                    default:
                        _serialPort.PortName = IMPLICIT_CCUx_UPS_SERIAL_PORT;
                        break;
                }

                _serialPort.BaudRate = 9600;
                _serialPort.Parity = Parity.Even;
                _serialPort.DataBits = 8;
                _serialPort.StopBits = StopBits.One;
                _serialPort.DataReceived += new DSerialDataReceived(_serialPort_DataReceived);
                _serialPort.Start();
            }
            catch (Exception ex)
            {
                HandledExceptionAdapter.Examine(ex);
                Console.WriteLine(ex.ToString());
            }
        }

        void _cUpsProto_ValuesChanged(CUps2750Values i_aValues)
        {
            if (_sendUpsMonitorMsg)
            {
                Events.ProcessEvent(
                    new EventUpsValuesChanged(
                        i_aValues));
            }
        }

        void _cUpsProto_AlarmStateChanged(TUpsAlarm i_aAlarm, bool i_bCurrentValue)
        {
            switch (i_aAlarm)
            {
                case TUpsAlarm.PrimaryPowerMissing:
                    CcuSpecialInputs.Singleton.ActuateOutputBoundToPrimaryPowerMissingFromUpsMonitor(i_bCurrentValue);
                    break;

                case TUpsAlarm.BatteryEmpty:
                    CcuSpecialInputs.Singleton.ActuateOutputBoundToBatteryIsLowFromUpsMonitor(i_bCurrentValue);
                    break;

                case TUpsAlarm.OutputFuse:
                    CcuSpecialInputs.Singleton.ActuateOutputsBoundToUpsOutputFuse(i_bCurrentValue);
                    break;

                case TUpsAlarm.BatteryFault:
                    CcuSpecialInputs.Singleton.ActuateOutputBoundToUpsBatteryFault(i_bCurrentValue);
                    break;

                case TUpsAlarm.BatteryFuse:
                    CcuSpecialInputs.Singleton.ActuateOutputBoundToUpsBatteryFuse(i_bCurrentValue);
                    break;

                case TUpsAlarm.Overtemperature:
                    CcuSpecialInputs.Singleton.ActuateOutputBoundToUpsOvertemperature(i_bCurrentValue);
                    break;

                case TUpsAlarm.Tamper:
                    CcuSpecialInputs.Singleton.ActuateOutputBoundToUpsTamperSabotage(i_bCurrentValue);
                    break;
            }

            if (_sendUpsMonitorMsg)
            {
                Events.ProcessEvent(
                    new EventUpsAlarmStateChanged(
                        i_bCurrentValue));
            }
        }

        void _cUpsProto_OnlineStateChanged(TUpsOnlineState i_aOnlineState)
        {
            _lastOnlineState = i_aOnlineState;

            if (i_aOnlineState == TUpsOnlineState.Online)
            {
                CcuSpecialInputs.Singleton.StopEvaluatingAlarmsPrimaryPowerMissingBatteryIsLow();
            }
            else
            {
                CcuSpecialInputs.Singleton.StartEvaluatingAlarmsPrimaryPowerMissingBatteryIsLow();
            }

            if (_sendUpsMonitorMsg)
            {
                Events.ProcessEvent(
                    new EventUpsOnlineStateChanged(
                        (byte) i_aOnlineState));
            }
        }

        void _serialPort_DataReceived(
            ISimpleSerialPort peer, 
            ByteDataCarrier data, 
            int optionalDataLength,
            int timeStamp)
        {
            _upsProto.PushData(data);
        }
    }
}
