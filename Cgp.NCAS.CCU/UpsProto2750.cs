using System;
using System.Globalization;
using Contal.IwQuick.Threads;
using Contal.IwQuick.Data;
using Contal.IwQuick.Sys;
/**
 * 
 * Comm. params: 9600 8-E-1
 * 
 * sync word:                      0x8787        - - -
 * Vinp:                           0xXXXX          |
 * Vout:                           0xXXXX          |
 * Vbat:                           0xXXXX          |
 * Ibat:                           0xXXXX          |
 * Iload:                          0xXXXX          |
 * Estimated Battery capacity      0xXXXX          |
 * Temp:                           0xXXXX          |
 * Alarm:                          0xXXXX          |
 * Mode:                           0x00XX        \   /
 * Resets:                         0xXXXX         \ /
 * 8bit CHsumm:                    0x00XX          V
 * 
 * CHsumm=LOW(sync)+LOW(Vinp)+LOW(Vout)+LOW(Vbat)+LOW(Ibat)+LOW(Iload)+LOW(EBC)+LOW(Temp)+LOW(Alarm)+LOW(Mode)+LOW(Resets)
 * */
using JetBrains.Annotations;

namespace Contal.Cgp.NCAS.CCU
{
    public enum TUpsOnlineState
    {
        Unknown,
        Online,
        Offline
    }

    public enum TUpsAlarm
    {
        OutputFuse,
        OutputPowerOutOfTolerance,
        PrimaryPowerMissing,
        BatteryFault,
        BatteryEmpty,
        BatteryFuse,
        Overtemperature,
        Tamper,
    }

    [LwSerialize(113)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    public class CUps2750Values : IDisposable
    {
        //[LwSerialize()]
        public decimal m_diVoltageInput;
        //[LwSerialize()]
        public decimal m_diVoltageOutput;
        //[LwSerialize()]
        public decimal m_diVoltageBatery;
        //[LwSerialize()]
        public decimal m_diCurrentBattery;
        //[LwSerialize()]
        public decimal m_diCurrentLoad;
        //[LwSerialize()]
        public decimal m_diEstimatedBatteryCapacity;
        //[LwSerialize()]
        public decimal m_diTemperature;

        [LwSerialize]
        public float _diVoltageInput { get { return (float)m_diVoltageInput; } }
        [LwSerialize]
        public float _diVoltageOutput { get { return (float)m_diVoltageOutput; } }
        [LwSerialize]
        public float _diVoltageBatery { get { return (float)m_diVoltageBatery; } }
        [LwSerialize]
        public float _diCurrentBattery { get { return (float)m_diCurrentBattery; } }
        [LwSerialize]
        public float _diCurrentLoad { get { return (float)m_diCurrentLoad; } }
        [LwSerialize]
        public float _diEstimatedBatteryCapacity { get { return (float)m_diEstimatedBatteryCapacity; } }
        [LwSerialize]
        public float _diTemperature { get { return (float)m_diTemperature; } }

        [LwSerialize]
        public bool m_bOutputFuse;
        [LwSerialize]
        public bool m_bOutputPowerOutOfTolerance;
        [LwSerialize]
        public bool m_bPrimaryPowerMissing;
        [LwSerialize]
        public bool m_bBatteryFault;
        [LwSerialize]
        public bool m_bBatteryEmpty;
        [LwSerialize]
        public bool m_bBatteryFuse;
        [LwSerialize]
        public bool m_bOvertemperature;
        [LwSerialize]
        public bool m_bTamper;
        public byte m_byMode;
        [LwSerialize]
        string _byMode { get { return m_byMode.ToString(CultureInfo.InvariantCulture); } }
        [LwSerialize]
        public int m_iResets;

        #region IDisposable Members

        public void Dispose()
        {

        }

        #endregion
    }

    public delegate void DUpsOnlineStateChanged(TUpsOnlineState i_aOnlineState);
    public delegate void DUpsValuesChanged(CUps2750Values i_aValues);
    public delegate void DUpsAlarmChanged(TUpsAlarm i_aAlarm, bool i_bCurrentValue);

    public class UpsProto2750
    {
        private static readonly TimerManager _timers = new TimerManager();

        private const byte SYNC_BYTE = 0x87;
        private const int VALUE_LENGTH = 11;
        private const int DATA_LENGTH = VALUE_LENGTH * sizeof(short);
        // data length withou checksum
        private const int PURE_DATA_LENGTH = (VALUE_LENGTH - 1) * sizeof(short);

        private const int OFFLINE_PERIOD = 10; // in seconds
        public static int EnsuredOfflinePeriod
        {
            get
            {
                return OFFLINE_PERIOD * 1000;
            }
        }

        private int m_iReceivedRaw = 0;
        public int ReceivedRaw { get { return m_iReceivedRaw; } }

        private int m_iReceivedPackets = 0;
        public int ReceivedPackets { get { return m_iReceivedPackets; } }

        private int m_iReceivedCorrectPackets = 0;
        public int ReceivedCorrectPackets { get { return m_iReceivedCorrectPackets; } }

        private TUpsOnlineState m_aOnlineState = TUpsOnlineState.Unknown;
        public TUpsOnlineState OnlineState
        {
            get { return m_aOnlineState; }
            private set
            {
                if (value == TUpsOnlineState.Online ||
                    value == TUpsOnlineState.Unknown)
                    RestartOnlineStatusTimeout();

                if (value != m_aOnlineState)
                {
                    m_aOnlineState = value;

                    FireOnlineStateChanged();
                }
            }
        }

        public event DUpsOnlineStateChanged OnlineStateChanged;
        public event DUpsAlarmChanged AlarmStateChanged;

        /*
         * fields
         * */
        private const int m_iVoltageInput = 0;
        private const int m_iVoltageOutput = 1;
        private const int m_iVoltageBatery = 2;
        private const int m_iCurrentBattery = 3;
        private const int m_iCurrentLoad = 4;
        private const int m_iEstimatedBatteryCapacity = 5;
        private const int m_iTemperature = 6;
        private const int m_iAlarm = 7;
        private const int m_iMode = 8;
        private const int m_iResets = 9;

        public void PushData(ByteDataCarrier i_aData)
        {
            if (null == i_aData)
                return;

            m_iReceivedRaw++;


            int iFound = -1;
            for (int i = 1; i < i_aData.Length; i++)
            {
                if (i_aData[i] == SYNC_BYTE &&
                    i_aData[i - 1] == SYNC_BYTE)
                {
                    if (i_aData.Length - i - 1 >= DATA_LENGTH)
                    {
                        m_iReceivedPackets++;

                        if (IsChecksumCorrect(i_aData, i + 1))
                        {
                            // the received packet is also correct to parse and populate
                            OnlineState = TUpsOnlineState.Online;
                            m_iReceivedCorrectPackets++;
                            iFound = i + 1;
                            break;
                        }
                    }
                }
            }

            if (iFound >= 0)
            {
                int iUpperBound = iFound + DATA_LENGTH;
                short[] arValues = new short[VALUE_LENGTH];
                for (int i = iFound, j = 0; i < iUpperBound; i += 2, j++)
                {
                    arValues[j] = (short)(i_aData[i] | i_aData[i + 1] << 8);
                }

                FireValuesReceived(ref arValues);
                //arValues = null;
            }
        }

        public event DUpsValuesChanged ValuesChanged;

        private bool IsChecksumCorrect([NotNull] ByteDataCarrier data, int packetStart)
        {

            if (data.Length - packetStart < DATA_LENGTH)
                return false;

            int iCountedChecksum = SYNC_BYTE;
            // summmary of all low values from the packet
            for (int i = 0; i < PURE_DATA_LENGTH; i += 2)
            {
                iCountedChecksum += data[packetStart + i];
            }

            iCountedChecksum = iCountedChecksum & 255;

            byte byCarriedChecksum = data[packetStart + PURE_DATA_LENGTH];

            return byCarriedChecksum == iCountedChecksum;
        }

        private byte m_byOldAlarmValue = 0;
        private CUps2750Values ConvertValues([NotNull] ref short[] _rawValues)
        {
            CUps2750Values aValues = new CUps2750Values
            {
                m_diVoltageInput = (decimal) _rawValues[m_iVoltageInput]/100,
                m_diVoltageOutput = (decimal) _rawValues[m_iVoltageOutput]/100,
                m_diVoltageBatery = (decimal) _rawValues[m_iVoltageBatery]/100,
                m_diCurrentBattery = (decimal) _rawValues[m_iCurrentBattery]/100,
                m_diCurrentLoad = (decimal) _rawValues[m_iCurrentLoad]/100,
                m_diEstimatedBatteryCapacity = (decimal) _rawValues[m_iEstimatedBatteryCapacity]/100,
                m_diTemperature = (decimal) _rawValues[m_iTemperature]/100
            };

            // examination of the alarm states
            byte byAlarm = (byte)_rawValues[m_iAlarm];
            aValues.m_bOutputFuse = (byAlarm & 0x01) > 0;
            if ((byAlarm & 0x01) != (m_byOldAlarmValue & 0x01))
            {
                FireAlarmStateChanged(TUpsAlarm.OutputFuse, aValues.m_bOutputFuse);
            }

            aValues.m_bOutputPowerOutOfTolerance = (byAlarm & 0x02) > 0;
            if ((byAlarm & 0x02) != (m_byOldAlarmValue & 0x02))
            {
                FireAlarmStateChanged(TUpsAlarm.OutputPowerOutOfTolerance, aValues.m_bOutputPowerOutOfTolerance);
            }

            aValues.m_bPrimaryPowerMissing = (byAlarm & 0x04) > 0;
            if ((byAlarm & 0x04) != (m_byOldAlarmValue & 0x04))
            {
                FireAlarmStateChanged(TUpsAlarm.PrimaryPowerMissing, aValues.m_bPrimaryPowerMissing);
            }

            aValues.m_bBatteryFault = (byAlarm & 0x08) > 0;
            if ((byAlarm & 0x08) != (m_byOldAlarmValue & 0x08))
            {
                FireAlarmStateChanged(TUpsAlarm.BatteryFault, aValues.m_bBatteryFault);
            }

            aValues.m_bBatteryEmpty = (byAlarm & 0x10) > 0;
            if ((byAlarm & 0x10) != (m_byOldAlarmValue & 0x10))
            {
                FireAlarmStateChanged(TUpsAlarm.BatteryEmpty, aValues.m_bBatteryEmpty);
            }

            aValues.m_bBatteryFuse = (byAlarm & 0x20) > 0;
            if ((byAlarm & 0x20) != (m_byOldAlarmValue & 0x20))
            {
                FireAlarmStateChanged(TUpsAlarm.BatteryFuse, aValues.m_bBatteryFuse);
            }

            aValues.m_bOvertemperature = (byAlarm & 0x40) > 0;
            if ((byAlarm & 0x40) != (m_byOldAlarmValue & 0x40))
            {
                FireAlarmStateChanged(TUpsAlarm.Overtemperature, aValues.m_bOvertemperature);
            }

            aValues.m_bTamper = (byAlarm & 0x80) > 0;
            if ((byAlarm & 0x80) != (m_byOldAlarmValue & 0x80))
            {
                FireAlarmStateChanged(TUpsAlarm.Tamper, aValues.m_bTamper);
            }

            m_byOldAlarmValue = byAlarm;

            aValues.m_byMode = (byte)_rawValues[m_iMode];

            aValues.m_iResets = (byte)_rawValues[m_iResets];

            return aValues;
        }


        private void FireValuesReceived(ref short[] i_arFields)
        {
            if (null == i_arFields)
                return;

            using (CUps2750Values aValues = ConvertValues(ref i_arFields))
            {


                if (null != ValuesChanged)
                    try
                    {
                        ValuesChanged(aValues);
                    }
                    catch (Exception error)
                    {
                        HandledExceptionAdapter.Examine(error);
                    }
            }
        }

        private void FireAlarmStateChanged(TUpsAlarm i_aAlarmIndicator, bool i_bCurrentValue)
        {
            if (null != AlarmStateChanged)
            {
                try
                {
                    AlarmStateChanged(i_aAlarmIndicator, i_bCurrentValue);
                }
                catch (Exception error)
                {
                    HandledExceptionAdapter.Examine(error);
                }
            }
        }

        private void FireOnlineStateChanged()
        {
            if (null != OnlineStateChanged)
                try
                {
                    OnlineStateChanged(m_aOnlineState);
                }
                catch (Exception error)
                {
                    HandledExceptionAdapter.Examine(error);
                }
        }

        private volatile ITimer _onlineStatusTimer = null;
        private readonly Object _onlineStatusTimerLock = new Object();
        private void RestartOnlineStatusTimeout()
        {

            lock (_onlineStatusTimerLock)
            {
                if (null != _onlineStatusTimer)
                    try
                    {
                        _onlineStatusTimer.StopTimer();
                        _onlineStatusTimer = null;
                    }
                    catch
                    {
                    }

                _onlineStatusTimer = _timers.StartTimeout(OFFLINE_PERIOD*1000, OnOnlineStatusTimeout);
            }
        }

        private bool OnOnlineStatusTimeout(TimerCarrier timer)
        {
            OnlineState = TUpsOnlineState.Offline;
            _onlineStatusTimer = null;
            return true;
        }

        public void ResetToUnknown()
        {
            OnlineState = TUpsOnlineState.Unknown;
        }
    }
}
