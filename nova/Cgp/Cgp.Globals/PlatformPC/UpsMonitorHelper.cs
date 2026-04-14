using System;

using Contal.IwQuick.Data;

namespace Contal.Cgp.Globals
{
    class UpsMonitorHelper
    {
    }

    [Serializable]
    [LwSerialize(113)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    public class CUps2750Values : IDisposable
    {
        [LwSerializeAttribute()]
        public float _diVoltageInput;
        [LwSerializeAttribute()]
        public float _diVoltageOutput;
        [LwSerializeAttribute()]
        public float _diVoltageBatery;
        [LwSerializeAttribute()]
        public float _diCurrentBattery;
        [LwSerializeAttribute()]
        public float _diCurrentLoad;
        [LwSerializeAttribute()]
        public float _diEstimatedBatteryCapacity;
        [LwSerializeAttribute()]
        public float _diTemperature;
        [LwSerializeAttribute()]
        public bool m_bOutputFuse;
        [LwSerializeAttribute()]
        public bool m_bOutputPowerOutOfTolerance;
        [LwSerializeAttribute()]
        public bool m_bPrimaryPowerMissing;
        [LwSerializeAttribute()]
        public bool m_bBatteryFault;
        [LwSerializeAttribute()]
        public bool m_bBatteryEmpty;
        [LwSerializeAttribute()]
        public bool m_bBatteryFuse;
        [LwSerializeAttribute()]
        public bool m_bOvertemperature;
        [LwSerializeAttribute()]
        public bool m_bTamper;
        [LwSerializeAttribute()]
        public string _byMode;
        [LwSerializeAttribute()]
        public int m_iResets;



        #region IDisposable Members

        public void Dispose()
        {

        }

        #endregion
    }
}
