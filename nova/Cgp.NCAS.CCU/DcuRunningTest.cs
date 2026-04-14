using System;

using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.CCU
{
    [LwSerialize(313)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    public class DcuRunningTest
    {
        [LwSerialize()]
        public Guid _idDcu;
        [LwSerialize()]
        public byte _address;
        [LwSerialize()]
        public bool _online;
        [LwSerialize()]
        public bool _toggleCard = false;
        [LwSerialize()]
        public bool _toggleADC = false;
        [LwSerialize()]
        public int _toggleAdcMinTime = 3;
        [LwSerialize()]
        public int _toggleAdcMaxTime = 5;
        [LwSerialize()]
        public int _toggleCardMinTime = 3;
        [LwSerialize()]
        public int _toggleCardMaxTime = 5;
        [LwSerialize()]
        public byte _toggleCardGeneratedCardType = 0;

        internal DcuRunningTest(DCUStateAndSettings dcuState)
        {
            _idDcu = dcuState.Id;
            _online = dcuState.IsOnline;
            _address = dcuState.LogicalAddress;
            if (_online)
            {
                _toggleCard = dcuState._toggleCard;
                _toggleADC = dcuState._toggleADC;
                _toggleAdcMinTime = dcuState._toggleAdcMinTime;
                _toggleAdcMaxTime = dcuState._toggleAdcMaxTime;
                _toggleCardMinTime = dcuState._toggleCardMinTime;
                _toggleCardMaxTime = dcuState._toggleCardMaxTime;
                _toggleCardGeneratedCardType = (byte)dcuState._toggleCardGeneratedCardType;

            }
        }

        public DcuRunningTest()
        {
        }
    }
}
