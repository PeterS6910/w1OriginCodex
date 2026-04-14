using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;

using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Definitions;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick;
using Contal.IwQuick.Data;
using Contal.IwQuick.Remoting;

namespace Contal.Cgp.NCAS.Server.Beans
{
    public enum CCUOnlineState : byte
    {
        Unknown = 0xFF,
        Offline = 0,
        Online = 1
    }

    public enum CCUConfigurationState : byte
    {
        Unconfigured = 0,
        ConfiguredForThisServer = 1,
        ConfiguredForAnotherServer = 2,
        Upgrading = 3,
        Unknown = 0xFF,
        ForceReconfiguration = 4,
        ConfiguredForThisServerUpgradeOnly = 5,
        ConfiguredForThisServerUpgradeOnlyWinCe = 6
    }

    [Serializable]
    [LwSerialize(310)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    [LwSerializeNoParent]
    public class CCU
        : AOrmObjectWithVersion,
        IOrmObjectWithAlarmInstructions,
        ISetFullTextSearchString
    {
        public const string COLUMNIDCCU = "IdCCU";
        public const string COLUMNINDEXCCU = "IndexCCU";
        public const string COLUMNNAME = "Name";
        public const string COLUMNIPADDRESS = "IPAddress";
        public const string COLUMNTIMEZONEINFO = "TimeZoneInfo";
        public const string COLUMNTIMESHIFT = "TimeShift";
        public const string COLUMNDCUS = "DCUs";
        public const string COLUMNINPUTS = "Inputs";
        public const string COLUMNOUTPUTS = "Outputs";
        public const string COLUMNSHORTNORMAL = "ShortNormal";
        public const string COLUMNNORMALALARM = "NormalAlarm";
        public const string COLUMNALARMBREAK = "AlarmBreak";
        public const string COLUMNDOORENVIRONMENTS = "DoorEnvironments";
        public const string COLUMNCARDREADERS = "CardReaders";
        public const string COLUMNDESCRIPTION = "Description";
        public const string COLUMNPORTBAUDRATECOM = "PortBaudRateCom";
        public const string COLUMNENABLEDCOMPORT = "EnabledComPort";
        public const string COLUMNSNTPIPADDRESSES = "SNTPIpAddresses";
        public const string COLUMNSNTPHOSTNAME = "SNTPHostNames";
        public const string COLUMNOBJECTTYPE = "ObjectType";
        public const string COLUMNWATCHDOGOUTPUT = "WatchdogOutput";
        public const string COLUMNGUIDWATCHDOGOUTPUT = "GuidWatchdogOutput";

        public const string COLUMNALARMOFFLINE = "AlarmOffline";
        public const string COLUMN_BLOCK_ALARM_OFFLINE = "BlockAlarmOffline";
        public const string COLUMN_OBJ_BLOCK_ALARM_OFFLINE_OBJECT_TYPE = "ObjBlockAlarmOfflineObjectType";
        public const string COLUMN_OBJ_BLOCK_ALARM_OFFLINE_ID = "ObjBlockAlarmOfflineId";
        public const string COLUMN_OBJ_BLOCK_ALARM_OFFLINE = "ObjBlockAlarmOffline";
        public const string COLUMN_EVENTLOG_DURING_BLOCK_ALARM_OFFLINE = "EventlogDuringBlockAlarmOffline";

        public const string COLUMNALARMTAMPER = "AlarmTamper";
        public const string COLUMN_BLOCK_ALARM_TAMPER = "BlockAlarmTamper";
        public const string COLUMN_OBJ_BLOCK_ALARM_TAMPER_OBJECT_TYPE = "ObjBlockAlarmTamperObjectType";
        public const string COLUMN_OBJ_BLOCK_ALARM_TAMPER_ID = "ObjBlockAlarmTamperId";
        public const string COLUMN_OBJ_BLOCK_ALARM_TAMPER = "ObjBlockAlarmTamper";
        public const string COLUMN_EVENTLOG_DURING_BLOCK_ALARM_TAMPER = "EventlogDuringBlockAlarmTamper";

        public const string COLUMNOUTPUTTAMPER = "OutputTamper";
        public const string COLUMN_GUID_OUTPUT_TAMPER = "GuidOutputTamper";

        public const string COLUMNALARMPRIMARYPOWERMISSING = "AlarmPrimaryPowerMissing";
        public const string COLUMN_BLOCK_ALARM_PRIMARY_POWER_MISSING = "BlockAlarmPrimaryPowerMissing";
        public const string COLUMN_OBJ_BLOCK_ALARM_PRIMARY_POWER_MISSING_OBJECT_TYPE = "ObjBlockAlarmPrimaryPowerMissingObjectType";
        public const string COLUMN_OBJ_BLOCK_ALARM_PRIMARY_POWER_MISSING_ID = "ObjBlockAlarmPrimaryPowerMissingId";
        public const string COLUMN_OBJ_BLOCK_ALARM_PRIMARY_POWER_MISSING = "ObjBlockAlarmPrimaryPowerMissing";
        public const string COLUMN_EVENTLOG_DURING_BLOCK_ALARM_PRIMARY_POWER_MISSING = "EventlogDuringBlockAlarmPrimaryPowerMissing";

        public const string COLUMNOUTPUTPRIMARYPOWERMISSING = "OutputPrimaryPowerMissing";
        public const string COLUMNGUIDOUTPUTPRIMARYPOWERMISSING = "GuidOutputPrimaryPowerMissing";

        public const string COLUMNALARMBATTERYISLOW = "AlarmBatteryIsLow";
        public const string COLUMN_BLOCK_ALARM_BATTERY_IS_LOW = "BlockAlarmBatteryIsLow";
        public const string COLUMN_OBJ_BLOCK_ALARM_BATTERY_IS_LOW_OBJECT_TYPE = "ObjBlockAlarmBatteryIsLowObjectType";
        public const string COLUMN_OBJ_BLOCK_ALARM_BATTERY_IS_LOW_ID = "ObjBlockAlarmBatteryIsLowId";
        public const string COLUMN_OBJ_BLOCK_ALARM_BATTERY_IS_LOW = "ObjBlockAlarmBatteryIsLow";
        public const string COLUMN_EVENTLOG_DURING_BLOCK_ALARM_BATTERY_IS_LOW = "EventlogDuringBlockAlarmBatteryIsLow";

        public const string COLUMNOUTPUTBATTERYISLOW = "OutputBatteryIsLow";
        public const string COLUMNGUIDOUTPUTBATTERYISLOW = "GuidOutputBatteryIsLow";

        public const string COLUMNALARMFUSEONEXTENSIONBOARD = "AlarmFuseOnExtensionBoard";
        public const string COLUMN_BLOCK_ALARM_FUSE_ON_EXTENSION_BOARD = "BlockAlarmFuseOnExtensionBoard";
        public const string COLUMN_OBJ_BLOCK_ALARM_FUSE_ON_EXTENSION_BOARD_OBJECT_TYPE = "ObjBlockAlarmFuseOnExtensionBoardObjectType";
        public const string COLUMN_OBJ_BLOCK_ALARM_FUSE_ON_EXTENSION_BOARD_ID = "ObjBlockAlarmFuseOnExtensionBoardId";
        public const string COLUMN_OBJ_BLOCK_ALARM_FUSE_ON_EXTENSION_BOARD = "ObjBlockAlarmFuseOnExtensionBoard";
        public const string COLUMN_EVENTLOG_DURING_BLOCK_ALARM_FUSE_ON_EXTENSION_BOARD = "EventlogDuringBlockAlarmFuseOnExtensionBoard";

        public const string COLUMNOUTPUTFUSEONEXTENSIONBOARD = "OutputFuseOnExtensionBoard";
        public const string COLUMNGUIDOUTPUTFUSEONEXTENSIONBOARD = "GuidOutputFuseOnExtensionBoard";

        public const string COLUMN_ALARM_CCU_CAT_UNREACHABLE = "AlarmCcuCatUnreachable";
        public const string COLUMN_ALARM_CCU_CAT_UNREACHABLE_PRESENATION_GROUP = "AlarmCcuCatUnreachablePresentationGroup";

        public const string COLUMN_ALARM_CCU_TRANSFER_TO_ARC_TIMED_OUT = "AlarmCcuTransferToArcTimedOut";
        public const string COLUMN_ALARM_CCU_TRANSFER_TO_ARC_TIMED_OUT_PRESENATION_GROUP = "AlarmCcuTransferToArcTimedOutPresentationGroup";

        public const string COLUMNISCONFIGURED = "IsConfigured";
        public const string COLUMNMACADDRESS = "MACAddress";
        public const string COLUMNMAXNODELOOKUPSEQUENCE = "MaxNodeLookupSequence";
        public const string COLUMNMAINBOARDTYPE = "CcuMainboardType";
        public const string COLUMNINPUTSCOUNT = "InputsCount";
        public const string COLUMNOUTPUTSCOUNT = "OutputsCount";
        public const string COLUMNLOCALALARMINSTRUCTION = "LocalAlarmInstruction";
        public const string COLUMNSYNCINGTIMEFROMSERVER = "SyncingTimeFromServer";

        public const string COLUMN_HAS_CAT12_COMBO_LICENCE = "HasCat12ComboLicence";

        public const string COLUMN_ALARM_TRANSMITTER = "AlarmTransmitter";
        public const string COLUMN_ID_ALARM_TRANSMITTER = "IdAlarmTransmitter";

        public const string COLUMN_CCU_ALARM_ARCS = "CcuAlarmArcs";
        public const string COLUMN_ALARM_TYPE_AND_ID_ALARM_ARCS = "AlarmTypeAndIdAlarmArcs";

        public const string ColumnVersion = "Version";

        [LwSerialize]
        public virtual Guid IdCCU { get; set; }
        [LwSerialize]
        public virtual int IndexCCU { get; set; }
        public virtual string Name { get; set; }
        public virtual string IPAddress { get; set; }
        [LwSerialize]
        public virtual string TimeZoneInfo { get; set; }
        [LwSerialize]
        public virtual int TimeShift { get; set; }
        public virtual ICollection<DCU> DCUs { get; set; }

        public virtual ICollection<Input> Inputs { get; set; }
        public virtual ICollection<Output> Outputs { get; set; }
        public virtual ICollection<DoorEnvironment> DoorEnvironments { get; set; }

        [LwSerialize]
        public virtual double? ShortNormal { get; set; }
        [LwSerialize]
        public virtual double? NormalAlarm { get; set; }
        [LwSerialize]
        public virtual double? AlarmBreak { get; set; }

        public virtual string R1 { get; set; }
        public virtual string R2 { get; set; }
        public virtual ICollection<CardReader> CardReaders { get; set; }
        [LwSerialize]
        public virtual int PortBaudRateCom { get; set; }
        [LwSerialize]
        public virtual bool EnabledComPort { get; set; }
        [LwSerialize]
        public virtual string SNTPIpAddresses { get; set; }
        public virtual bool InheritGeneralNtpSettings { get; set; }
        public virtual string Description { get; set; }
        public virtual byte ObjectType { get; set; }
        public virtual Output WatchdogOutput { get; set; }
        private Guid _guidWatchdogOutput = Guid.Empty;
        [LwSerialize]
        public virtual Guid GuidWatchdogOutput { get { return _guidWatchdogOutput; } set { _guidWatchdogOutput = value; } }

        public virtual bool? AlarmOffline { get; set; }
        public virtual bool? BlockAlarmOffline { get; set; }
        public virtual byte? ObjBlockAlarmOfflineObjectType { get; set; }
        public virtual Guid? ObjBlockAlarmOfflineId { get; set; }

        [LwSerialize]
        public virtual bool? AlarmTamper { get; set; }
        [LwSerialize]
        public virtual bool? BlockAlarmTamper { get; set; }
        [LwSerialize]
        public virtual byte? ObjBlockAlarmTamperObjectType { get; set; }
        [LwSerialize]
        public virtual Guid? ObjBlockAlarmTamperId { get; set; }
        [LwSerialize]
        public virtual bool? EventlogDuringBlockAlarmTamper { get; set; }

        public virtual Output OutputTamper { get; set; }
        private Guid _guidOutputTamper = Guid.Empty;
        [LwSerialize]
        public virtual Guid GuidOutputTamper { get { return _guidOutputTamper; } set { _guidOutputTamper = value; } }

        [LwSerialize]
        public virtual bool? AlarmPrimaryPowerMissing { get; set; }
        [LwSerialize]
        public virtual bool? BlockAlarmPrimaryPowerMissing { get; set; }
        [LwSerialize]
        public virtual byte? ObjBlockAlarmPrimaryPowerMissingObjectType { get; set; }
        [LwSerialize]
        public virtual Guid? ObjBlockAlarmPrimaryPowerMissingId { get; set; }
        [LwSerialize]
        public virtual bool? EventlogDuringBlockAlarmPrimaryPowerMissing { get; set; }

        public virtual Output OutputPrimaryPowerMissing { get; set; }
        private Guid _guidOutputPrimaryPowerMissing = Guid.Empty;
        [LwSerialize]
        public virtual Guid GuidOutputPrimaryPowerMissing { get { return _guidOutputPrimaryPowerMissing; } set { _guidOutputPrimaryPowerMissing = value; } }

        [LwSerialize]
        public virtual bool? AlarmBatteryIsLow { get; set; }
        [LwSerialize]
        public virtual bool? BlockAlarmBatteryIsLow { get; set; }
        [LwSerialize]
        public virtual byte? ObjBlockAlarmBatteryIsLowObjectType { get; set; }
        [LwSerialize]
        public virtual Guid? ObjBlockAlarmBatteryIsLowId { get; set; }
        [LwSerialize]
        public virtual bool? EventlogDuringBlockAlarmBatteryIsLow { get; set; }

        public virtual Output OutputBatteryIsLow { get; set; }
        private Guid _guidOutputBatteryIsLow = Guid.Empty;
        [LwSerialize]
        public virtual Guid GuidOutputBatteryIsLow { get { return _guidOutputBatteryIsLow; } set { _guidOutputBatteryIsLow = value; } }

        [LwSerialize]
        public virtual bool? AlarmUpsOutputFuse { get; set; }
        [LwSerialize]
        public virtual bool? BlockAlarmUpsOutputFuse { get; set; }
        [LwSerialize]
        public virtual byte? ObjBlockAlarmUpsOutputFuseObjectType { get; set; }
        [LwSerialize]
        public virtual Guid? ObjBlockAlarmUpsOutputFuseId { get; set; }
        [LwSerialize]
        public virtual bool? EventlogDuringBlockAlarmUpsOutputFuse { get; set; }

        public virtual Output OutputUpsOutputFuse { get; set; }
        private Guid _guidOutputUpsOutputFuse = Guid.Empty;
        [LwSerialize]
        public virtual Guid GuidOutputUpsOutputFuse { get { return _guidOutputUpsOutputFuse; } set { _guidOutputUpsOutputFuse = value; } }

        [LwSerialize]
        public virtual bool? AlarmUpsBatteryFault { get; set; }
        [LwSerialize]
        public virtual bool? BlockAlarmUpsBatteryFault { get; set; }
        [LwSerialize]
        public virtual byte? ObjBlockAlarmUpsBatteryFaultObjectType { get; set; }
        [LwSerialize]
        public virtual Guid? ObjBlockAlarmUpsBatteryFaultId { get; set; }
        [LwSerialize]
        public virtual bool? EventlogDuringBlockAlarmUpsBatteryFault { get; set; }

        public virtual Output OutputUpsBatteryFault { get; set; }
        private Guid _guidOutputUpsBatteryFault = Guid.Empty;
        [LwSerialize]
        public virtual Guid GuidOutputUpsBatteryFault { get { return _guidOutputUpsBatteryFault; } set { _guidOutputUpsBatteryFault = value; } }

        [LwSerialize]
        public virtual bool? AlarmUpsBatteryFuse { get; set; }
        [LwSerialize]
        public virtual bool? BlockAlarmUpsBatteryFuse { get; set; }
        [LwSerialize]
        public virtual byte? ObjBlockAlarmUpsBatteryFuseObjectType { get; set; }
        [LwSerialize]
        public virtual Guid? ObjBlockAlarmUpsBatteryFuseId { get; set; }
        [LwSerialize]
        public virtual bool? EventlogDuringBlockAlarmUpsBatteryFuse { get; set; }

        public virtual Output OutputUpsBatteryFuse { get; set; }
        private Guid _guidOutputUpsBatteryFuse = Guid.Empty;
        [LwSerialize]
        public virtual Guid GuidOutputUpsBatteryFuse { get { return _guidOutputUpsBatteryFuse; } set { _guidOutputUpsBatteryFuse = value; } }

        [LwSerialize]
        public virtual bool? AlarmUpsOvertemperature { get; set; }
        [LwSerialize]
        public virtual bool? BlockAlarmUpsOvertemperature { get; set; }
        [LwSerialize]
        public virtual byte? ObjBlockAlarmUpsOvertemperatureObjectType { get; set; }
        [LwSerialize]
        public virtual Guid? ObjBlockAlarmUpsOvertemperatureId { get; set; }
        [LwSerialize]
        public virtual bool? EventlogDuringBlockAlarmUpsOvertemperature { get; set; }

        public virtual Output OutputUpsOvertemperature { get; set; }
        private Guid _guidOutputUpsOvertemperature = Guid.Empty;
        [LwSerialize]
        public virtual Guid GuidOutputUpsOvertemperature { get { return _guidOutputUpsOvertemperature; } set { _guidOutputUpsOvertemperature = value; } }

        [LwSerialize]
        public virtual bool? AlarmUpsTamperSabotage { get; set; }
        [LwSerialize]
        public virtual bool? BlockAlarmUpsTamperSabotage { get; set; }
        [LwSerialize]
        public virtual byte? ObjBlockAlarmUpsTamperSabotageObjectType { get; set; }
        [LwSerialize]
        public virtual Guid? ObjBlockAlarmUpsTamperSabotageId { get; set; }
        [LwSerialize]
        public virtual bool? EventlogDuringBlockAlarmUpsTamperSabotage { get; set; }

        public virtual Output OutputUpsTamperSabotage { get; set; }
        private Guid _guidOutputUpsTamperSabotage = Guid.Empty;
        [LwSerialize]
        public virtual Guid GuidOutputUpsTamperSabotage { get { return _guidOutputUpsTamperSabotage; } set { _guidOutputUpsTamperSabotage = value; } }

        [LwSerialize]
        public virtual bool? AlarmFuseOnExtensionBoard { get; set; }
        [LwSerialize]
        public virtual bool? BlockAlarmFuseOnExtensionBoard { get; set; }
        [LwSerialize]
        public virtual byte? ObjBlockAlarmFuseOnExtensionBoardObjectType { get; set; }
        [LwSerialize]
        public virtual Guid? ObjBlockAlarmFuseOnExtensionBoardId { get; set; }
        [LwSerialize]
        public virtual bool? EventlogDuringBlockAlarmFuseOnExtensionBoard { get; set; }

        public virtual Output OutputFuseOnExtensionBoard { get; set; }
        private Guid _guidOutputFuseOnExtensionBoard = Guid.Empty;
        [LwSerialize]
        public virtual Guid GuidOutputFuseOnExtensionBoard { get { return _guidOutputFuseOnExtensionBoard; } set { _guidOutputFuseOnExtensionBoard = value; } }

        [LwSerialize]
        public virtual bool? AlarmCcuCatUnreachable { get; set; }
        public virtual PresentationGroup AlarmCcuCatUnreachablePresentationGroup { get; set; }

        [LwSerialize]
        public virtual bool? AlarmCcuTransferToArcTimedOut { get; set; }
        public virtual PresentationGroup AlarmCcuTransferToArcTimedOutPresentationGroup { get; set; }

        public virtual bool IsConfigured { get; set; }
        public virtual string MACAddress { get; set; }
        [LwSerialize]
        public virtual byte MaxNodeLookupSequence { get; set; }
        public virtual byte? CcuMainboardType { get; set; }

        public virtual byte[] Cat12ComboLicence { get; set; }

        public virtual int? InputsCount { get; set; }
        public virtual int? OutputsCount { get; set; }
        public virtual string LocalAlarmInstruction { get; set; }
        [LwSerialize]
        public virtual bool? SyncingTimeFromServer { get; set; }

        [LwSerialize]
        public virtual int CrEventlogSize { get; set; }
        [LwSerialize]
        public virtual int CrLastEventTimeForMarkAlarmArea { get; set; }

        public virtual AlarmTransmitter AlarmTransmitter { get; set; }
        [LwSerialize]
        public virtual Guid IdAlarmTransmitter { get; set; }

        public virtual ICollection<CcuAlarmArc> CcuAlarmArcs { get; set; }
        [LwSerialize]
        public virtual List<AlarmTypeAndIdAlarmArc> AlarmTypeAndIdAlarmArcs { get; set; }

        public CCU()
        {
            // ReSharper disable DoNotCallOverridableMethodsInConstructor
            ShortNormal = (double)NCASConstants.DefaultBalancingLevels.ToLevel1;
            NormalAlarm = (double)NCASConstants.DefaultBalancingLevels.ToLevel2;
            AlarmBreak = (double)NCASConstants.DefaultBalancingLevels.ToLevel3;
            TimeZoneInfo = System.TimeZoneInfo.Local.Id;
            TimeShift = (int)System.TimeZoneInfo.Local.BaseUtcOffset.TotalMinutes;
            MaxNodeLookupSequence = 31;
            ObjectType = (byte)Cgp.Globals.ObjectType.CCU;
            InheritGeneralNtpSettings = true;
            CrEventlogSize = 100;
            //MaxNodeLookupSequence = 1;
            // ReSharper restore DoNotCallOverridableMethodsInConstructor
        }

        public override string ToString()
        {
            return string.Format("{0:000} - {1}", IndexCCU, Name);
        }

        public override bool Compare(object obj)
        {
            if (obj == null)
                return false;

            CCU ccu = obj as CCU;

            return ccu != null 
                && ccu.IdCCU == IdCCU;
        }

        public override string GetIdString()
        {
            return IdCCU.ToString();
        }

        public override object GetId()
        {
            return IdCCU;
        }

        public override IModifyObject CreateModifyObject()
        {
            return new CcuModifyObj(this);
        }

        public override ObjectType GetObjectType()
        {
            return Cgp.Globals.ObjectType.CCU;
        }

        public virtual void PrepareToSend()
        {
            GuidWatchdogOutput = WatchdogOutput != null
                ? WatchdogOutput.IdOutput
                : Guid.Empty;

            GuidOutputTamper = OutputTamper != null
                ? OutputTamper.IdOutput
                : Guid.Empty;

            GuidOutputPrimaryPowerMissing = OutputPrimaryPowerMissing != null
                ? OutputPrimaryPowerMissing.IdOutput
                : Guid.Empty;

            GuidOutputBatteryIsLow = OutputBatteryIsLow != null
                ? OutputBatteryIsLow.IdOutput
                : Guid.Empty;

            GuidOutputUpsOutputFuse = OutputUpsOutputFuse != null
                ? OutputUpsOutputFuse.IdOutput
                : Guid.Empty;

            GuidOutputUpsBatteryFault = OutputUpsBatteryFault != null
                ? OutputUpsBatteryFault.IdOutput
                : Guid.Empty;

            GuidOutputUpsBatteryFuse = OutputUpsBatteryFuse != null
                ? OutputUpsBatteryFuse.IdOutput
                : Guid.Empty;

            GuidOutputUpsOvertemperature = OutputUpsOvertemperature != null
                ? OutputUpsOvertemperature.IdOutput
                : Guid.Empty;

            GuidOutputUpsTamperSabotage = OutputUpsTamperSabotage != null
                ? OutputUpsTamperSabotage.IdOutput
                : Guid.Empty;

            GuidOutputFuseOnExtensionBoard = OutputFuseOnExtensionBoard != null
                ? OutputFuseOnExtensionBoard.IdOutput
                : Guid.Empty;

            IdAlarmTransmitter = AlarmTransmitter != null
                ? AlarmTransmitter.IdAlarmTransmitter
                : Guid.Empty;

            AlarmTypeAndIdAlarmArcs = CcuAlarmArcs == null || CcuAlarmArcs.Count == 0
                ? null
                : new List<AlarmTypeAndIdAlarmArc>(
                    CcuAlarmArcs.Select(
                        ccuAlarmArc =>
                            new AlarmTypeAndIdAlarmArc(
                                (AlarmType)ccuAlarmArc.AlarmType,
                                ccuAlarmArc.IdAlarmArc)));
        }

        public virtual string GetLocalAlarmInstruction()
        {
            return LocalAlarmInstruction;
        }

        #region ISetFullTextSearchString Members

        public virtual string FullTextSearchString { get; set; }

        public virtual string AlternateName
        {
            get { return null; }
        }

        public virtual IEnumerable<string> OtherFullTextSearchStrings
        {
            get
            {
                return Enumerable.Repeat(
                    ToString(),
                    1);
            }
        }

        #endregion
    }

    [Serializable]
    public class CCUShort : IShortObject
    {
        public const string COLUMN_ID_CCU = "IdCCU";
        public const string COLUMN_INDEX_CCU = "IndexCCU";
        public const string COLUMN_NAME = "Name";
        public const string COLUMN_IP_ADDRESS = "IPAddress";
        public const string COLUMN_MAC_ADDRESS = "MacAddress";
        public const string COLUMN_MAINBOARD_TYPE = "MainboardType";
        public const string COLUMN_ONLINE_STATE = "OnlineState";
        public const string COLUMN_STRING_ONLINE_STATE = "StringOnlineState";
        public const string COLUMN_CONFIGURATION_STATE = "ConfigurationState";
        public const string COLUMN_STRING_CONFIGURATION_STATE = "StringConfigurationState";
        public const string COLUMN_DESCRIPTION = "Description";
        public const string COLUMN_SYMBOL = "Symbol";

        public Guid IdCCU { get; set; }
        public string IndexCCU { get; set; }
        public string Name { get; set; }
        public string IPAddress { get; set; }
        public string MacAddress { get; set; }
        public string MainboardType { get; set; }
        public CCUOnlineState OnlineState { get; set; }
        public string StringOnlineState { get; set; }
        public CCUConfigurationState ConfigurationState { get; set; }
        public string StringConfigurationState { get; set; }
        public string Description { get; set; }
        public Image Symbol { get; set; }

        public CCUShort(CCU ccu)
        {
            IdCCU = ccu.IdCCU;
            IndexCCU = ccu.IndexCCU.ToString("D3");
            Name = ccu.Name;
            IPAddress = ccu.IPAddress;
            Description = ccu.Description;
            MacAddress = ccu.MACAddress;
        }

        public override string ToString()
        {
            return string.Format("{0:000} - {1}", IndexCCU, Name);
        }

        #region IShortObject Members

        public ObjectType ObjectType { get { return ObjectType.CCU; } }

        public string GetSubTypeImageString(object value)
        {
            if (value != null)
            {
                try
                {
                    if (value.ToString() == Cgp.Server.Beans.OnlineState.Online.ToString()) return ObjectType.CCU.ToString();
                    return ObjTypeHelper.CCUOffline;
                }
                catch { }
            }
            return string.Empty;
        }

        public object Id { get { return IdCCU; } }

        #endregion
    }

    [Serializable]
    public class CcuModifyObj : AModifyObject
    {
        public override ObjectType GetOrmObjectType { get { return ObjectType.CCU; } }

        public CcuModifyObj(CCU ccu)
        {
            Id = ccu.IdCCU;
            FullName = ccu.ToString();
            Description = ccu.Description;
        }
    }

    [Serializable]
    public class CcuListObj
    {
        public virtual Guid Id { get; set; }
        public virtual string Name { get; set; }

        public CcuListObj(CCU ccu)
        {
            // ReSharper disable once DoNotCallOverridableMethodsInConstructor
            Id = ccu.IdCCU;
            // ReSharper disable once DoNotCallOverridableMethodsInConstructor
            Name = ccu.Name;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    [Serializable]
    public class LookupedCcu
    {
        public const string COLUMN_CHECKED = "IsChecked";
        public const string COLUMN_IP_ADDRESS = "IPAddress";
        public const string COLUMN_MAINBOARD_TYPE = "MainboardType";
        public const string COLUMN_CCU_VERSION = "CcuVersion";
        public const string COLUMN_CE_VERSION = "CeVersion";

        public virtual bool IsChecked { get; set; }
        public virtual string IPAddress { get; set; }
        public virtual string MainboardType { get; set; }
        public virtual string CcuVersion { get; set; }
        public virtual string CeVersion { get; set; }

        public LookupedCcu(string ipAddress)
        {
            // ReSharper disable DoNotCallOverridableMethodsInConstructor
            IsChecked = true;
            IPAddress = ipAddress;
            // ReSharper restore DoNotCallOverridableMethodsInConstructor
        }

        public override string ToString()
        {
            return IPAddress;
        }
    }

    public class StateChangedCCUHandler : ARemotingCallbackHandler
    {
        private static volatile StateChangedCCUHandler _singleton;
        private static readonly object _syncRoot = new object();

        private Action<Guid, byte> _stateChanged;

        public static StateChangedCCUHandler Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (_singleton == null)
                            _singleton = new StateChangedCCUHandler();
                    }

                return _singleton;
            }
        }

        public StateChangedCCUHandler()
            : base("StateChangedCCUHandler")
        {
        }

        public void RegisterStateChanged(Action<Guid, byte> stateChanged)
        {
            _stateChanged += stateChanged;
        }

        public void UnregisterStateChanged(Action<Guid, byte> stateChanged)
        {
            _stateChanged -= stateChanged;
        }

        public void RunEvent(Guid id, byte State)
        {
            if (_stateChanged != null)
                _stateChanged(id, State);
        }
    }

    public class ConfiguredChangedCCUHandler : ARemotingCallbackHandler
    {
        private static volatile ConfiguredChangedCCUHandler _singleton;
        private static readonly object _syncRoot = new object();

        private Action<Guid, byte, bool, bool> _configuredChanged;

        public static ConfiguredChangedCCUHandler Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (_singleton == null)
                            _singleton = new ConfiguredChangedCCUHandler();
                    }

                return _singleton;
            }
        }

        public ConfiguredChangedCCUHandler()
            : base("ConfiguredChangedCCUHandler")
        {
        }

        public void RegisterConfiguredChanged(Action<Guid, byte, bool, bool> configuredChanged)
        {
            _configuredChanged += configuredChanged;
        }

        public void UnregisterConfiguredChanged(Action<Guid, byte, bool, bool> configuredChanged)
        {
            _configuredChanged -= configuredChanged;
        }

        public void RunEvent(Guid id, byte configured, bool isCCU0, bool blockedByLicence)
        {
            if (_configuredChanged != null)
                _configuredChanged(id, configured, isCCU0, blockedByLicence);
        }
    }

    public class MACAddressChangedCCUHandler : ARemotingCallbackHandler
    {
        private static volatile MACAddressChangedCCUHandler _singleton;
        private static readonly object _syncRoot = new object();

        private Action<Guid, string> _macAddressChanged;

        public static MACAddressChangedCCUHandler Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (_singleton == null)
                            _singleton = new MACAddressChangedCCUHandler();
                    }

                return _singleton;
            }
        }

        public MACAddressChangedCCUHandler()
            : base("MACAddressChangedCCUHandler")
        {
        }

        public void RegisterMACAddressChanged(Action<Guid, string> macAddressChanged)
        {
            _macAddressChanged += macAddressChanged;
        }

        public void UnregisterLogicalAddressChanged(Action<Guid, string> macAddressChanged)
        {
            _macAddressChanged -= macAddressChanged;
        }

        public void RunEvent(Guid id, string macAddress)
        {
            if (_macAddressChanged != null)
                _macAddressChanged(id, macAddress);
        }
    }

    public class CreatedCCUHandler : ARemotingCallbackHandler
    {
        private static volatile CreatedCCUHandler _singleton;
        private static readonly object _syncRoot = new object();

        private DVoid2Void _createdCCUEvent;

        public static CreatedCCUHandler Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (_singleton == null)
                            _singleton = new CreatedCCUHandler();
                    }

                return _singleton;
            }
        }

        public CreatedCCUHandler()
            : base("CreatedCCUHandler")
        {
        }

        public void RegisterCreatedCCUEvent(DVoid2Void createCCUEvent)
        {
            _createdCCUEvent += createCCUEvent;
        }

        public void UnregisterCreatedCCUEvent(DVoid2Void createCCUEvent)
        {
            _createdCCUEvent -= createCCUEvent;
        }

        public void RunEvent()
        {
            if (_createdCCUEvent != null)
                _createdCCUEvent();
        }
    }

    public class TFTPFileTransferProgressChangedHandler : ARemotingCallbackHandler
    {
        private static volatile TFTPFileTransferProgressChangedHandler _singleton;
        private static readonly object _syncRoot = new object();

        private Action<IPAddress, int, byte> _tftpFileTransferProgressChanged;

        public static TFTPFileTransferProgressChangedHandler Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (_singleton == null)
                            _singleton = new TFTPFileTransferProgressChangedHandler();
                    }

                return _singleton;
            }
        }

        public TFTPFileTransferProgressChangedHandler()
            : base("TFTPFileTransferProgressChangedHandler")
        {
        }

        public void RegisterTFTPFileTransferProgressChanged(Action<IPAddress, int, byte> transferProgressChanged)
        {
            _tftpFileTransferProgressChanged += transferProgressChanged;
        }

        public void UnregisterTFTPFileTransferProgressChanged(Action<IPAddress, int, byte> transferProgressChanged)
        {
            _tftpFileTransferProgressChanged -= transferProgressChanged;
        }

        public void RunEvent(IPAddress destinationIPAddress, int percents, byte transferPurpose)
        {
            if (_tftpFileTransferProgressChanged != null)
                _tftpFileTransferProgressChanged(destinationIPAddress, percents, transferPurpose);
        }
    }

    public class CEUpgradeProgressChangedHandler : ARemotingCallbackHandler
    {
        private static volatile CEUpgradeProgressChangedHandler _singleton;
        private static readonly object _syncRoot = new object();

        private Action<IPAddress, string> _ceUpgradeProgressChanged;

        public static CEUpgradeProgressChangedHandler Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (_singleton == null)
                            _singleton = new CEUpgradeProgressChangedHandler();
                    }

                return _singleton;
            }
        }

        public CEUpgradeProgressChangedHandler()
            : base("CEUpgradeProgressChangedHandler")
        {
        }

        public void RegisterCEUpgradeProgressChanged(Action<IPAddress, string> ceUpgradeProgressChanged)
        {
            _ceUpgradeProgressChanged += ceUpgradeProgressChanged;
        }

        public void UnregisterCEUpgradeProgressChanged(Action<IPAddress, string> ceUpgradeProgressChanged)
        {
            _ceUpgradeProgressChanged -= ceUpgradeProgressChanged;
        }

        public void RunEvent(IPAddress ccuIPAddress, string state)
        {
            if (_ceUpgradeProgressChanged != null)
                _ceUpgradeProgressChanged(ccuIPAddress, state);
        }
    }

    public class CCUUpgradeProgressChangedHandler : ARemotingCallbackHandler
    {
        private static volatile CCUUpgradeProgressChangedHandler _singleton;
        private static readonly object _syncRoot = new object();

        private Action<IPAddress, int> _ccuUpgradeProgressChanged;

        public static CCUUpgradeProgressChangedHandler Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (_singleton == null)
                            _singleton = new CCUUpgradeProgressChangedHandler();
                    }

                return _singleton;
            }
        }

        public CCUUpgradeProgressChangedHandler()
            : base("CCUUpgradeProgressChangedHandler")
        {
        }

        public void RegisterCCUUpgradeProgressChanged(Action<IPAddress, int> ccuUpgradeProgressChanged)
        {
            _ccuUpgradeProgressChanged += ccuUpgradeProgressChanged;
        }

        public void UnregisterCCUUpgradeProgressChanged(Action<IPAddress, int> ccuUpgradeProgressChanged)
        {
            _ccuUpgradeProgressChanged -= ccuUpgradeProgressChanged;
        }

        public void RunEvent(IPAddress ccuIPAddress, int percents)
        {
            if (_ccuUpgradeProgressChanged != null)
                _ccuUpgradeProgressChanged(ccuIPAddress, percents);
        }
    }

    public class CCUMakeLogDumpProgressChangedHandler : ARemotingCallbackHandler
    {
        private static volatile CCUMakeLogDumpProgressChangedHandler _singleton;
        private static readonly object _syncRoot = new object();

        private Action<Guid, int> _ccuMakeLogDumpProgressChanged;

        public static CCUMakeLogDumpProgressChangedHandler Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (_singleton == null)
                            _singleton = new CCUMakeLogDumpProgressChangedHandler();
                    }

                return _singleton;
            }
        }

        public CCUMakeLogDumpProgressChangedHandler()
            : base("CCUMakeLogDumpProgressChangedHandler")
        {
        }

        public void RegisterCCUMakeLogDumpProgressChanged(Action<Guid, int> ccuUpgradeProgressChanged)
        {
            _ccuMakeLogDumpProgressChanged += ccuUpgradeProgressChanged;
        }

        public void UnregisterCCUMakeLogDumpProgressChanged(Action<Guid, int> ccuUpgradeProgressChanged)
        {
            _ccuMakeLogDumpProgressChanged -= ccuUpgradeProgressChanged;
        }

        public void RunEvent(Guid idCcu, int percents)
        {
            if (_ccuMakeLogDumpProgressChanged != null)
                _ccuMakeLogDumpProgressChanged(idCcu, percents);
        }
    }

    public class CCUUpgradeFinishedHandler : ARemotingCallbackHandler
    {
        private static volatile CCUUpgradeFinishedHandler _singleton;
        private static readonly object _syncRoot = new object();

        private Action<IPAddress, bool> _ccuUpgradeFinished;

        public static CCUUpgradeFinishedHandler Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (_singleton == null)
                            _singleton = new CCUUpgradeFinishedHandler();
                    }

                return _singleton;
            }
        }

        public CCUUpgradeFinishedHandler()
            : base("CCUUpgradeFinishedHandler")
        {
        }

        public void RegisterCCUUpgradeFinished(Action<IPAddress, bool> ccuUpgradeFinished)
        {
            _ccuUpgradeFinished += ccuUpgradeFinished;
        }

        public void UnregisterCCUUpgradeFinished(Action<IPAddress, bool> ccuUpgradeFinished)
        {
            _ccuUpgradeFinished -= ccuUpgradeFinished;
        }

        public void RunEvent(IPAddress ccuIPAddress, bool success)
        {
            if (_ccuUpgradeFinished != null)
                _ccuUpgradeFinished(ccuIPAddress, success);
        }
    }

    public class CCUKillFailedHandler : ARemotingCallbackHandler
    {
        private static volatile CCUKillFailedHandler _singleton;
        private static readonly object _syncRoot = new object();

        private Action<IPAddress> _ccuKillFailed;

        public static CCUKillFailedHandler Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (_singleton == null)
                            _singleton = new CCUKillFailedHandler();
                    }

                return _singleton;
            }
        }

        public CCUKillFailedHandler()
            : base("CCUKillFailedHandler")
        {
        }

        public void RegisterCCUKillFailed(Action<IPAddress> ccuKillFailed)
        {
            _ccuKillFailed += ccuKillFailed;
        }

        public void UnregisterCCUKillFailed(Action<IPAddress> ccuKillFailed)
        {
            _ccuKillFailed -= ccuKillFailed;
        }

        public void RunEvent(IPAddress ccuIPAddress)
        {
            if (_ccuKillFailed != null)
                _ccuKillFailed(ccuIPAddress);
        }
    }

    public class CCULookupFinishedHandler : ARemotingCallbackHandler
    {
        private static volatile CCULookupFinishedHandler _singleton;
        private static readonly object _syncRoot = new object();

        private Action<ICollection<LookupedCcu>, ICollection<Guid>> _lookupFinished;

        public static CCULookupFinishedHandler Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (null == _singleton)
                            _singleton = new CCULookupFinishedHandler();
                    }

                return _singleton;
            }
        }

        public CCULookupFinishedHandler()
            : base("CCULookupFinishedHandler")
        {
        }

        public void RegisterLookupFinished(Action<ICollection<LookupedCcu>, ICollection<Guid>> lookupFinished)
        {
            _lookupFinished += lookupFinished;
        }

        public void UnregisterLookupFinished(Action<ICollection<LookupedCcu>, ICollection<Guid>> lookupFinished)
        {
            _lookupFinished -= lookupFinished;
        }

        public void RunEvent(ICollection<LookupedCcu> lookupedCcus, ICollection<Guid> lookupingClients)
        {
            if (_lookupFinished != null)
                _lookupFinished(lookupedCcus, lookupingClients);
        }
    }

    public class CCUUpsMonitorValuesChangedHandler : ARemotingCallbackHandler
    {
        private static volatile CCUUpsMonitorValuesChangedHandler _singleton;
        private static readonly object _syncRoot = new object();

        private Action<string, CUps2750Values> _upsMonitorValuesChanged;

        public static CCUUpsMonitorValuesChangedHandler Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (null == _singleton)
                            _singleton = new CCUUpsMonitorValuesChangedHandler();
                    }

                return _singleton;
            }
        }

        public CCUUpsMonitorValuesChangedHandler()
            : base("CCUUpsMonitorValuesChangedHandler")
        {
        }

        public void RegisterUpsMonitorValuesChanged(Action<string, CUps2750Values> upsMonitorValuesChanged)
        {
            _upsMonitorValuesChanged += upsMonitorValuesChanged;
        }

        public void UnregisterUpsMonitorValuesChanged(Action<string, CUps2750Values> upsMonitorValuesChanged)
        {
            _upsMonitorValuesChanged -= upsMonitorValuesChanged;
        }

        public void RunEvent(string ccuIpAddress, CUps2750Values upsValues)
        {
            if (_upsMonitorValuesChanged != null)
                _upsMonitorValuesChanged(ccuIpAddress, upsValues);
        }
    }

    public class CCUUpsMonitorOnlineStateChangedHandler : ARemotingCallbackHandler
    {
        private static volatile CCUUpsMonitorOnlineStateChangedHandler _singleton;
        private static readonly object _syncRoot = new object();

        private Action<string, byte> _upsMonitorOnlineState;

        public static CCUUpsMonitorOnlineStateChangedHandler Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (null == _singleton)
                            _singleton = new CCUUpsMonitorOnlineStateChangedHandler();
                    }

                return _singleton;
            }
        }

        public CCUUpsMonitorOnlineStateChangedHandler()
            : base("CCUUpsMonitorOnlineStateChangedHandler")
        {
        }

        public void RegisterUpsMonitorOnlineStateChanged(Action<string, byte> upsMonitorOnlineState)
        {
            _upsMonitorOnlineState += upsMonitorOnlineState;
        }

        public void UnregisterUpsMonitorOnlineStateChanged(Action<string, byte> upsMonitorOnlineState)
        {
            _upsMonitorOnlineState -= upsMonitorOnlineState;
        }

        public void RunEvent(string ccuIpAddress, byte upsOnlineState)
        {
            if (_upsMonitorOnlineState != null)
                _upsMonitorOnlineState(ccuIpAddress, upsOnlineState);
        }
    }

    public class CCUUpsMonitorAlarmStateChangedHandler : ARemotingCallbackHandler
    {
        private static volatile CCUUpsMonitorAlarmStateChangedHandler _singleton;
        private static readonly object _syncRoot = new object();

        private Action<string, bool> _upsMonitorAlarmState;

        public static CCUUpsMonitorAlarmStateChangedHandler Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (null == _singleton)
                            _singleton = new CCUUpsMonitorAlarmStateChangedHandler();
                    }

                return _singleton;
            }
        }

        public CCUUpsMonitorAlarmStateChangedHandler()
            : base("CCUUpsMonitorAlarmStateChangedHandler")
        {
        }

        public void RegisterUpsMonitorAlarmStateChanged(Action<string, bool> upsMonitorAlarmState)
        {
            _upsMonitorAlarmState += upsMonitorAlarmState;
        }

        public void UnregisterUpsMonitorAlarmStateChanged(Action<string, bool> upsMonitorAlarmState)
        {
            _upsMonitorAlarmState -= upsMonitorAlarmState;
        }

        public void RunEvent(string ccuIpAddress, bool upsAlarmState)
        {
            if (_upsMonitorAlarmState != null)
                _upsMonitorAlarmState(ccuIpAddress, upsAlarmState);
        }
    }
}
