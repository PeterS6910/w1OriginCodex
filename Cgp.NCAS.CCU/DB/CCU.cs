using System;
using System.Collections.Generic;
using Contal.IwQuick.Data;
using Contal.LwSerialization;
using System.IO;

namespace Contal.Cgp.NCAS.CCU.DB
{
    public enum CCUConfigurationState : byte
    {
        Unconfigured = 0,
        ConfiguredForThisServer = 1,
        ConfiguredForAnotherServer = 2,
        Upgrading = 3,
        Unknown = 0xFF
    }

    [LwSerialize(310)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    public class CCU : IDbObject
    {
        [LwSerialize]
        public Guid IdCCU { get; set; }
        [LwSerialize]
        public int IndexCCU { get; set; }
        [LwSerialize]
        public string TimeZoneInfo { get; set; }
        [LwSerialize]
        public int TimeShift { get; set; }
        [LwSerialize]
        public double? ShortNormal { get; set; }
        [LwSerialize]
        public double? NormalAlarm { get; set; }
        [LwSerialize]
        public double? AlarmBreak { get; set; }
        [LwSerialize]
        public int PortBaudRateCom { get; set; }
        [LwSerialize]
        public bool EnabledComPort { get; set; }
        [LwSerialize]
        public string SNTPIpAddresses { get; set; }       
        [LwSerialize]
        public Guid GuidWatchdogOutput { get; set; }

        [LwSerialize]
        public bool? AlarmTamper { get; set; }
        [LwSerialize]
        public bool? BlockAlarmTamper { get; set; }
        [LwSerialize]
        public byte? ObjBlockAlarmTamperObjectType { get; set; }
        [LwSerialize]
        public Guid? ObjBlockAlarmTamperId { get; set; }
        [LwSerialize]
        public bool? EventlogDuringBlockAlarmTamper { get; set; }

        private Guid _guidOutputTamper = Guid.Empty;
        [LwSerialize]
        public Guid GuidOutputTamper { get { return _guidOutputTamper; } set { _guidOutputTamper = value; } }

        [LwSerialize]
        public bool? AlarmPrimaryPowerMissing { get; set; }
        [LwSerialize]
        public bool? BlockAlarmPrimaryPowerMissing { get; set; }
        [LwSerialize]
        public byte? ObjBlockAlarmPrimaryPowerMissingObjectType { get; set; }
        [LwSerialize]
        public Guid? ObjBlockAlarmPrimaryPowerMissingId { get; set; }
        [LwSerialize]
        public bool? EventlogDuringBlockAlarmPrimaryPowerMissing { get; set; }

        [LwSerialize]
        public Guid GuidOutputPrimaryPowerMissing { get; set; }

        [LwSerialize]
        public bool? AlarmBatteryIsLow { get; set; }
        [LwSerialize]
        public bool? BlockAlarmBatteryIsLow { get; set; }
        [LwSerialize]
        public byte? ObjBlockAlarmBatteryIsLowObjectType { get; set; }
        [LwSerialize]
        public Guid? ObjBlockAlarmBatteryIsLowId { get; set; }
        [LwSerialize]
        public bool? EventlogDuringBlockAlarmBatteryIsLow { get; set; }

        [LwSerialize]
        public Guid GuidOutputBatteryIsLow { get; set; }

        [LwSerialize]
        public bool? AlarmUpsOutputFuse { get; set; }
        [LwSerialize]
        public bool? BlockAlarmUpsOutputFuse { get; set; }
        [LwSerialize]
        public byte? ObjBlockAlarmUpsOutputFuseObjectType { get; set; }
        [LwSerialize]
        public Guid? ObjBlockAlarmUpsOutputFuseId { get; set; }
        [LwSerialize]
        public bool? EventlogDuringBlockAlarmUpsOutputFuse { get; set; }

        [LwSerialize]
        public Guid GuidOutputUpsOutputFuse { get; set; }

        [LwSerialize]
        public bool? AlarmUpsBatteryFault { get; set; }
        [LwSerialize]
        public bool? BlockAlarmUpsBatteryFault { get; set; }
        [LwSerialize]
        public byte? ObjBlockAlarmUpsBatteryFaultObjectType { get; set; }
        [LwSerialize]
        public Guid? ObjBlockAlarmUpsBatteryFaultId { get; set; }
        [LwSerialize]
        public bool? EventlogDuringBlockAlarmUpsBatteryFault { get; set; }

        [LwSerialize]
        public Guid GuidOutputUpsBatteryFault { get; set; }

        [LwSerialize]
        public bool? AlarmUpsBatteryFuse { get; set; }
        [LwSerialize]
        public bool? BlockAlarmUpsBatteryFuse { get; set; }
        [LwSerialize]
        public byte? ObjBlockAlarmUpsBatteryFuseObjectType { get; set; }
        [LwSerialize]
        public Guid? ObjBlockAlarmUpsBatteryFuseId { get; set; }
        [LwSerialize]
        public bool? EventlogDuringBlockAlarmUpsBatteryFuse { get; set; }

        [LwSerialize]
        public Guid GuidOutputUpsBatteryFuse { get; set; }

        [LwSerialize]
        public bool? AlarmUpsOvertemperature { get; set; }
        [LwSerialize]
        public bool? BlockAlarmUpsOvertemperature { get; set; }
        [LwSerialize]
        public byte? ObjBlockAlarmUpsOvertemperatureObjectType { get; set; }
        [LwSerialize]
        public Guid? ObjBlockAlarmUpsOvertemperatureId { get; set; }
        [LwSerialize]
        public bool? EventlogDuringBlockAlarmUpsOvertemperature { get; set; }

        [LwSerialize]
        public Guid GuidOutputUpsOvertemperature { get; set; }

        [LwSerialize]
        public bool? AlarmUpsTamperSabotage { get; set; }
        [LwSerialize]
        public bool? BlockAlarmUpsTamperSabotage { get; set; }
        [LwSerialize]
        public byte? ObjBlockAlarmUpsTamperSabotageObjectType { get; set; }
        [LwSerialize]
        public Guid? ObjBlockAlarmUpsTamperSabotageId { get; set; }
        [LwSerialize]
        public bool? EventlogDuringBlockAlarmUpsTamperSabotage { get; set; }

        [LwSerialize]
        public Guid GuidOutputUpsTamperSabotage { get; set; }

        [LwSerialize]
        public bool? AlarmFuseOnExtensionBoard { get; set; }
        [LwSerialize]
        public bool? BlockAlarmFuseOnExtensionBoard { get; set; }
        [LwSerialize]
        public byte? ObjBlockAlarmFuseOnExtensionBoardObjectType { get; set; }
        [LwSerialize]
        public Guid? ObjBlockAlarmFuseOnExtensionBoardId { get; set; }
        [LwSerialize]
        public bool? EventlogDuringBlockAlarmFuseOnExtensionBoard { get; set; }

        [LwSerialize]
        public Guid GuidOutputFuseOnExtensionBoard { get; set; }

        [LwSerialize]
        public virtual bool? AlarmCcuCatUnreachable { get; set; }

        [LwSerialize]
        public virtual bool? AlarmCcuTransferToArcTimedOut { get; set; }

        [LwSerialize]
        public byte MaxNodeLookupSequence { get; set; }

        [LwSerialize]
        public bool? SyncingTimeFromServer { get; set; }

        [LwSerialize]
        public int CrEventlogSize { get; set; }

        [LwSerialize]
        public int CrLastEventTimeForMarkAlarmArea { get; set; }

        [LwSerialize]
        public Guid IdAlarmTransmitter { get; set; }

        [LwSerialize]
        public List<AlarmTypeAndIdAlarmArc> AlarmTypeAndIdAlarmArcs { get; set; }

        public Guid GetGuid()
        {
            return IdCCU;
        }

        public Contal.Cgp.Globals.ObjectType GetObjectType()
        {
            return Contal.Cgp.Globals.ObjectType.CCU;
        }
    }
}
