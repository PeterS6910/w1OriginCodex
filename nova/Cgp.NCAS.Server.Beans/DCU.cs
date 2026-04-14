using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;

using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Definitions;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.Server.Beans
{
    [Serializable()]
    [LwSerialize(312)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    [LwSerializeNoParent]
    public class DCU :
        AOrmObjectWithVersion,
        IOrmObjectWithAlarmInstructions,
        ICardReaderObject
    {
        public const string COLUMNIDDCU = "IdDCU";
        public const string COLUMNNAME = "Name";
        public const string COLUMNFULLNAME = "FullName";

        public const string COLUMNCARDREADERS = "CardReaders";
        public const string COLUMNGUIDCARDREADERS = "GuidCardReaders";

        public const string COLUMNDOORENVIRONMENTS = "DoorEnvironments";
        public const string COLUMNGUIDDOORENVIRONMENTS = "GuidDoorEnvironments";
        public const string COLUMNCCU = "CCU";
        public const string COLUMNGUIDCCU = "GuidCCU";
        public const string COLUMNDESCRIPTION = "Description";
        public const string COLUMNINPUTS = "Inputs";
        public const string COLUMNGUIDINPUTS = "GuidInputs";
        public const string COLUMNOUTPUTS = "Outputs";
        public const string COLUMNGUIDOUTPUTS = "GuidOutputs";
        public const string COLUMNLOGICALADDRESS = "LogicalAddress";
        public const string COLUMNONLINESTATE = "OnlineState";
        public const string COLUMNUPGRADEPROGRESS = "UpgradeProgress";
        public const string COLUMNSELECTUPGRADE = "Select";
        public const string COLUMNOBJECTTYPE = "ObjectType";
        public const string COLUMNCKUNIQUE = "CkUnique";

        public const string COLUMNALARMOFFLINE = "AlarmOffline";
        public const string COLUMN_BLOCK_ALARM_OFFLINE = "BlockAlarmOffline";
        public const string COLUMN_OBJ_BLOCK_ALARM_OFFLINE_OBJECT_TYPE = "ObjBlockAlarmOfflineObjectType";
        public const string COLUMN_OBJ_BLOCK_ALARM_OFFLINE_ID = "ObjBlockAlarmOfflineId";
        public const string COLUMN_OBJ_BLOCK_ALARM_OFFLINE = "ObjBlockAlarmOffline";
        public const string COLUMN_EVENTLOG_DURING_BLOCK_ALARM_OFFLINE = "EventlogDuringBlockAlarmOffline";
        public const string COLUMN_OFFLINE_PRESENTATION_GROUP = "OfflinePresentationGroup";

        public const string COLUMNALARMTAMPER = "AlarmTamper";
        public const string COLUMN_BLOCK_ALARM_TAMPER = "BlockAlarmTamper";
        public const string COLUMN_OBJ_BLOCK_ALARM_TAMPER_OBJECT_TYPE = "ObjBlockAlarmTamperObjectType";
        public const string COLUMN_OBJ_BLOCK_ALARM_TAMPER_ID = "ObjBlockAlarmTamperId";
        public const string COLUMN_OBJ_BLOCK_ALARM_TAMPER = "ObjBlockAlarmTamper";
        public const string COLUMN_EVENTLOG_DURING_BLOCK_ALARM_TAMPER = "EventlogDuringBlockAlarmTamper";
        public const string COLUMN_TAMPER_SABOTAGE_PRESENTATION_GROUP = "TamperSabotagePresentationGroup";

        public const string COLUMNENABLEPARENTINFULLNAME = "EnableParentInFullName";
        public const string COLUMNINPUTSCOUNT = "InputsCount";
        public const string COLUMNOUTPUTSCOUNT = "OutputsCount";
        public const string COLUMNDCUSABOTAGEOUTPUT = "DcuSabotageOutput";
        public const string COLUMNGUIDDCUSABOTAGEOUTPUT = "GuidDcuSabotageOutput";
        public const string COLUMNDCUOFFLINEOUTPUT = "DcuOfflineOutput";
        public const string COLUMNGUIDDCUOFFLINEOUTPUT = "GuidDcuOfflineOutput";
        public const string COLUMN_DCU_INPUTS_SABOTAGE_OUTPUT = "DcuInputsSabotageOutput";
        public const string COLUMN_GUID_DCU_INPUTS_SABOTAGE_OUTPUT = "GuidDcuInputsSabotageOutput";
        public const string COLUMNLOCALALARMINSTRUCTION = "LocalAlarmInstruction";

        public const string COLUMN_DCU_ALARM_ARCS = "DcuAlarmArcs";
        public const string COLUMN_ALARM_TYPE_AND_ID_ALARM_ARCS = "AlarmTypeAndIdAlarmArcs";

        public const string ColumnVersion = "Version";

        [LwSerializeAttribute()]
        public virtual Guid IdDCU { get; set; }
        public virtual string Name { get; set; }

        public virtual ICollection<CardReader> CardReaders { get; set; }
        private List<Guid> _guidCardReaders = new List<Guid>();
        [LwSerializeAttribute()]
        public virtual List<Guid> GuidCardReaders { get { return _guidCardReaders; } set { _guidCardReaders = value; } }

        public virtual ICollection<DoorEnvironment> DoorEnvironments { get; set; }
        private List<Guid> _guidDoorEnvironments = new List<Guid>();
        [LwSerializeAttribute()]
        public virtual List<Guid> GuidDoorEnvironments { get { return _guidDoorEnvironments; } set { _guidDoorEnvironments = value; } }

        public virtual CCU CCU { get; set; }
        private Guid _guidCCU = Guid.Empty;
        [LwSerializeAttribute()]
        public virtual Guid GuidCCU { get { return _guidCCU; } set { _guidCCU = value; } }
        public virtual ICollection<Input> Inputs { get; set; }
        private List<Guid> _guidInputs = new List<Guid>();
        [LwSerializeAttribute()]
        public virtual List<Guid> GuidInputs { get { return _guidInputs; } set { _guidInputs = value; } }
        public virtual ICollection<Output> Outputs { get; set; }
        private List<Guid> _guidOutputs = new List<Guid>();
        [LwSerializeAttribute()]
        public virtual List<Guid> GuidOutputs { get { return _guidOutputs; } set { _guidOutputs = value; } }
        [LwSerializeAttribute()]
        public virtual byte LogicalAddress { get; set; }
        public virtual string Description { get; set; }
        public virtual byte ObjectType { get; set; }
        public virtual Guid CkUnique { get; set; }

        [LwSerialize]
        public virtual bool? AlarmOffline { get; set; }
        [LwSerialize]
        public virtual bool? BlockAlarmOffline { get; set; }
        [LwSerialize]
        public virtual byte? ObjBlockAlarmOfflineObjectType { get; set; }
        [LwSerialize]
        public virtual Guid? ObjBlockAlarmOfflineId { get; set; }

        public virtual PresentationGroup OfflinePresentationGroup { get; set; }

        [LwSerializeAttribute()]
        public virtual bool? AlarmTamper { get; set; }
        [LwSerializeAttribute()]
        public virtual bool? BlockAlarmTamper { get; set; }
        [LwSerializeAttribute()]
        public virtual byte? ObjBlockAlarmTamperObjectType { get; set; }
        [LwSerializeAttribute()]
        public virtual Guid? ObjBlockAlarmTamperId { get; set; }
        [LwSerializeAttribute()]
        public virtual bool? EventlogDuringBlockAlarmTamper { get; set; }

        public virtual PresentationGroup TamperSabotagePresentationGroup { get; set; }

        public virtual bool EnableParentInFullName { get; set; }

        public virtual int? InputsCount { get; set; }
        public virtual int? OutputsCount { get; set; }

        public virtual Output DcuSabotageOutput { get; set; }
        private Guid _guidDcuSabotageOutput = Guid.Empty;
        [LwSerializeAttribute()]
        public virtual Guid GuidDcuSabotageOutput { get { return _guidDcuSabotageOutput; } set { _guidDcuSabotageOutput = value; } }
        public virtual Output DcuOfflineOutput { get; set; }
        private Guid _guidDcuOfflineOutput = Guid.Empty;
        [LwSerializeAttribute()]
        public virtual Guid GuidDcuOfflineOutput { get { return _guidDcuOfflineOutput; } set { _guidDcuOfflineOutput = value; } }
        public virtual Output DcuInputsSabotageOutput { get; set; }
        private Guid _guidDcuInputsSabotageOutput = Guid.Empty;
        [LwSerializeAttribute()]
        public virtual Guid GuidDcuInputsSabotageOutput { get { return _guidDcuInputsSabotageOutput; } set { _guidDcuInputsSabotageOutput = value; } }      

        public virtual string LocalAlarmInstruction { get; set; }

        public virtual ICollection<DcuAlarmArc> DcuAlarmArcs { get; set; }
        [LwSerialize]
        public virtual List<AlarmTypeAndIdAlarmArc> AlarmTypeAndIdAlarmArcs { get; set; }

        public DCU()
        {
            ObjectType = (byte)Cgp.Globals.ObjectType.DCU;
            CkUnique = System.Guid.NewGuid();
            EnableParentInFullName = Support.EnableParentInFullName;
        }

        public override string ToString()
        {
            string name = string.Empty;
            if (EnableParentInFullName)
            {
                if (CCU != null)
                {
                    name += CCU.Name + StringConstants.SLASHWITHSPACES;
                }
            }
            name += Name;
            return name;
        }

        public override bool Compare(object obj)
        {
            if (obj == null)
                return false;

            if (obj is DCU)
            {
                return (obj as DCU).IdDCU == IdDCU;
            }
            else
            {
                return false;
            }
        }

        public virtual void PrepareToSend()
        {
            if (CCU != null)
            {
                GuidCCU = CCU.IdCCU;
            }
            else
            {
                GuidCCU = Guid.Empty;
            }

            GuidCardReaders.Clear();
            if (CardReaders != null)
            {
                foreach (CardReader cardReader in CardReaders)
                {
                    GuidCardReaders.Add(cardReader.IdCardReader);
                }
            }

            GuidInputs.Clear();
            if (Inputs != null)
            {
                foreach (Input input in Inputs)
                {
                    GuidInputs.Add(input.IdInput);
                }
            }

            GuidOutputs.Clear();
            if (Outputs != null)
            {
                foreach (Output output in Outputs)
                {
                    GuidOutputs.Add(output.IdOutput);
                }
            }

            GuidDoorEnvironments.Clear();
            if (DoorEnvironments != null)
            {
                foreach (DoorEnvironment doorEnvironment in DoorEnvironments)
                {
                    GuidDoorEnvironments.Add(doorEnvironment.IdDoorEnvironment);
                }
            }

            if (DcuSabotageOutput != null)
                GuidDcuSabotageOutput = DcuSabotageOutput.IdOutput;
            else
                GuidDcuSabotageOutput = Guid.Empty;

            if (DcuOfflineOutput != null)
                GuidDcuOfflineOutput = DcuOfflineOutput.IdOutput;
            else
                GuidDcuOfflineOutput = Guid.Empty;

            GuidDcuInputsSabotageOutput = DcuInputsSabotageOutput != null
                ? DcuInputsSabotageOutput.IdOutput
                : Guid.Empty;

            AlarmTypeAndIdAlarmArcs = DcuAlarmArcs == null || DcuAlarmArcs.Count == 0
                ? null
                : new List<AlarmTypeAndIdAlarmArc>(
                    DcuAlarmArcs.Select(
                        dcuAlarmArc =>
                            new AlarmTypeAndIdAlarmArc(
                                (AlarmType)dcuAlarmArc.AlarmType,
                                dcuAlarmArc.IdAlarmArc)));
        }

        public override string GetIdString()
        {
            return IdDCU.ToString();
        }

        public override object GetId()
        {
            return IdDCU;
        }

        public override IModifyObject CreateModifyObject()
        {
            return new DcuModifyObj(this);
        }

        public virtual IEnumerable<ICardReaderObject> GetChildObjects()
        {
            if (CardReaders == null)
                yield break;

            foreach (var cardReader in CardReaders)
            {
                yield return cardReader;
            }
        }

        public override Contal.Cgp.Globals.ObjectType GetObjectType()
        {
            return Contal.Cgp.Globals.ObjectType.DCU;
        }

        public virtual string GetLocalAlarmInstruction()
        {
            return LocalAlarmInstruction;
        }
    }

    [Serializable()]
    public class DCUShort : IShortObject
    {
        public const string COLUMN_ID_DCU = "IdDCU";
        public const string COLUMN_FULL_NAME = "FullName";
        public const string COLUMN_STATE = "OnlineState";
        public const string COLUMN_DOOR_ENVIRONMENT_STATE = "StateDoorEnvironment";
        public const string COLUMN_ID_DOOR_ENVIRONMENT = "IdDoorEnvironment";
        public const string COLUMN_STRING_STATE = "StringOnlineState";
        public const string COLUMN_STRING_DOOR_ENVIRONMENT_STATE = "StringDoorEnvironmentState";
        public const string COLUMN_DESCRIPTION = "Description";
        public const string COLUMN_SYMBOL = "Symbol";

        public Guid IdDCU { get; set; }
        public string FullName { get; set; }
        public OnlineState OnlineState { get; set; }
        public DoorEnvironmentState StateDoorEnvironment { get; set; }
        public Guid IdDoorEnvironment { get; set; }
        public string StringOnlineState { get; set; }
        public string StringDoorEnvironmentState { get; set; }
        public string Description { get; set; }
        public Image Symbol { get; set; }

        public DCUShort(DCU dcu)
        {
            IdDCU = dcu.IdDCU;
            FullName = dcu.ToString();
            if (dcu.DoorEnvironments != null && dcu.DoorEnvironments.Count > 0)
            {
                IdDoorEnvironment = dcu.DoorEnvironments.ElementAt(0).IdDoorEnvironment;
            }
            else
            {
                IdDoorEnvironment = Guid.Empty;
            }
            Description = dcu.Description;
        }

        public override string ToString()
        {
            return FullName;
        }

        #region IShortObject Members

        public string Name { get { return FullName; } }

        public ObjectType ObjectType { get { return ObjectType.DCU; } }

        public string GetSubTypeImageString(object value)
        {
            if (value is OnlineState)
            {
                try
                {
                    if ((OnlineState)value == OnlineState.Online) return ObjectType.DCU.ToString();
                    return ObjTypeHelper.DCUOffline;
                }
                catch { }
            }
            return string.Empty;
        }

        public object Id { get { return IdDCU; } }

        #endregion
    }

    [Serializable()]
    public class DcuModifyObj : AModifyObject
    {
        public override ObjectType GetOrmObjectType { get { return ObjectType.DCU; } }

        public DcuModifyObj(DCU dcu)
        {
            Id = dcu.IdDCU;
            FullName = dcu.ToString();
            Description = dcu.Description;
        }
    }

    [Serializable()]
    public class DcuListObj
    {
        public virtual Guid Id { get; set; }
        public virtual string Name { get; set; }

        public DcuListObj(DCU dcu)
        {
            Id = dcu.IdDCU;
            Name = string.Empty;
            if (dcu.CCU != null)
            {
                Name += dcu.CCU.Name + StringConstants.SLASH;
            }
            Name += dcu.Name;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public class DCUOnlineStateChangedHandler : IwQuick.Remoting.ARemotingCallbackHandler
    {
        private static volatile DCUOnlineStateChangedHandler _singleton = null;
        private static object _syncRoot = new object();

        private Action<Guid, OnlineState, Guid> _stateChanged;

        public static DCUOnlineStateChangedHandler Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (null == _singleton)
                            _singleton = new DCUOnlineStateChangedHandler();
                    }

                return _singleton;
            }
        }

        public DCUOnlineStateChangedHandler()
            : base("DCUOnlineStateChangedHandler")
        {
        }

        public void RegisterStateChanged(Action<Guid, OnlineState, Guid> stateChanged)
        {
            _stateChanged += stateChanged;
        }

        public void UnregisterStateChanged(Action<Guid, OnlineState, Guid> stateChanged)
        {
            _stateChanged -= stateChanged;
        }

        public void RunEvent(Guid id, OnlineState State, Guid parentGuid)
        {
            if (_stateChanged != null)
                _stateChanged(id, State, parentGuid);
        }
    }

    public class DCUPhysicalAddressChangedHandler : Contal.IwQuick.Remoting.ARemotingCallbackHandler
    {
        private static volatile DCUPhysicalAddressChangedHandler _singleton = null;
        private static object _syncRoot = new object();

        private Action<Guid, string> _physicalAddressChanged;

        public static DCUPhysicalAddressChangedHandler Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (_singleton == null)
                            _singleton = new DCUPhysicalAddressChangedHandler();
                    }

                return _singleton;
            }
        }

        public DCUPhysicalAddressChangedHandler()
            : base("DCUPhysicalAddressChangedHandler")
        {
        }

        public void RegisterPhysicalAddressChanged(Action<Guid, string> physicalAddressChanged)
        {
            _physicalAddressChanged += physicalAddressChanged;
        }

        public void UnregisterPhysicalAddressChanged(Action<Guid, string> physicalAddressChanged)
        {
            _physicalAddressChanged -= physicalAddressChanged;
        }

        public void RunEvent(Guid id, string physicalAddress)
        {
            if (_physicalAddressChanged != null)
                _physicalAddressChanged(id, physicalAddress);
        }
    }

    public class DcuInputsSabotageStateChangedHandler : Contal.IwQuick.Remoting.ARemotingCallbackHandler
    {
        private static volatile DcuInputsSabotageStateChangedHandler _singleton = null;
        private static readonly object _syncRoot = new object();

        private Action<Guid, State> _stateChanged;

        public static DcuInputsSabotageStateChangedHandler Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (null == _singleton)
                            _singleton = new DcuInputsSabotageStateChangedHandler();
                    }

                return _singleton;
            }
        }

        public DcuInputsSabotageStateChangedHandler()
            : base("DcuInputsSabotageStateChangedHandler")
        {
        }

        public void RegisterStateChanged(Action<Guid, State> stateChanged)
        {
            _stateChanged += stateChanged;
        }

        public void UnregisterStateChanged(Action<Guid, State> stateChanged)
        {
            _stateChanged -= stateChanged;
        }

        public void RunEvent(Guid id, State state)
        {
            if (_stateChanged != null)
                _stateChanged(id, state);
        }
    }

    public class CreatedDCUHandler : Contal.IwQuick.Remoting.ARemotingCallbackHandler
    {
        private static volatile CreatedDCUHandler _singleton = null;
        private static object _syncRoot = new object();

        private Contal.IwQuick.DVoid2Void _createdDCUEvent;

        public static CreatedDCUHandler Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (_singleton == null)
                            _singleton = new CreatedDCUHandler();
                    }

                return _singleton;
            }
        }

        public CreatedDCUHandler()
            : base("CreatedDCUHandler")
        {
        }

        public void RegisterCreatedDCUEvent(Contal.IwQuick.DVoid2Void createDCUEvent)
        {
            _createdDCUEvent += createDCUEvent;
        }

        public void UnregisterCreatedDCUEvent(Contal.IwQuick.DVoid2Void createDCUEvent)
        {
            _createdDCUEvent -= createDCUEvent;
        }

        public void RunEvent()
        {
            if (_createdDCUEvent != null)
                _createdDCUEvent();
        }
    }

    public class DCUMemoryWarningChangedHandler : Contal.IwQuick.Remoting.ARemotingCallbackHandler
    {
        private static volatile DCUMemoryWarningChangedHandler _singleton = null;
        private static object _syncRoot = new object();

        private Action<Guid, byte> _dcuMemoryWarningChanged;

        public static DCUMemoryWarningChangedHandler Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (_singleton == null)
                            _singleton = new DCUMemoryWarningChangedHandler();
                    }

                return _singleton;
            }
        }

        public DCUMemoryWarningChangedHandler()
            : base("DCUMemoryWarningChangedHandler")
        {
        }

        public void RegisterMemoryWarningChanged(Action<Guid, byte> dcuMemoryWarningChanged)
        {
            _dcuMemoryWarningChanged += dcuMemoryWarningChanged;
        }

        public void UnregisterMemoryWarningChanged(Action<Guid, byte> dcuMemoryWarningChanged)
        {
            _dcuMemoryWarningChanged -= dcuMemoryWarningChanged;
        }

        public void RunEvent(Guid idDcu, byte memoryLoad)
        {
            if (_dcuMemoryWarningChanged != null)
                _dcuMemoryWarningChanged(idDcu, memoryLoad);
        }
    }

    public class DCUUpgradeProgressChangedHandler : Contal.IwQuick.Remoting.ARemotingCallbackHandler
    {
        private static volatile DCUUpgradeProgressChangedHandler _singleton = null;
        private static object _syncRoot = new object();

        private Action<DCUUpgradeState> _upgradeProgressChanged;

        public static DCUUpgradeProgressChangedHandler Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (_singleton == null)
                            _singleton = new DCUUpgradeProgressChangedHandler();
                    }

                return _singleton;
            }
        }

        public DCUUpgradeProgressChangedHandler()
            : base("DCUUpgradeProgressChangedHandler")
        {
        }

        public void RegisterUpgradeProgressChanged(Action<DCUUpgradeState> stateChanged)
        {
            _upgradeProgressChanged += stateChanged;
        }

        public void UnregisterUpgradeProgressChanged(Action<DCUUpgradeState> stateChanged)
        {
            _upgradeProgressChanged -= stateChanged;
        }

        public void RunEvent(DCUUpgradeState upgradeState)
        {
            if (_upgradeProgressChanged != null)
                _upgradeProgressChanged(upgradeState);
        }
    }

    [Serializable()]
    public class DCUUpgradeState
    {
        public Guid CCUGuid = Guid.Empty;
        public byte[] DCUsLogicalAddresses = new byte[] { };
        public int? Percents = null;
        public byte? UnpackErrorCode = null;
        public byte[] OnlineDCUs = new byte[] { };

        public DCUUpgradeState(Guid ccuGuid, byte[] dcusLogicalAddresses, int? percents, byte? unpackErrorCode)
        {
            CCUGuid = ccuGuid;
            DCUsLogicalAddresses = dcusLogicalAddresses;
            Percents = percents;
            UnpackErrorCode = unpackErrorCode;
            OnlineDCUs = dcusLogicalAddresses;
        }

        public DCUUpgradeState(Guid ccuGuid, byte[] dcusLogicalAddresses, byte[] onlineDCUs, int? percents, byte? unpackErrorCode)
        {
            CCUGuid = ccuGuid;
            DCUsLogicalAddresses = dcusLogicalAddresses;
            Percents = percents;
            UnpackErrorCode = unpackErrorCode;
            OnlineDCUs = onlineDCUs;
        }
    }

    [Serializable]
    public class RegisterDcuUpgradeVersionException : Exception
    {
        public RegisterDcuUpgradeVersionException()
            : base() { }
        public RegisterDcuUpgradeVersionException(string message)
            : base(message) { }
        [SecurityPermissionAttribute(SecurityAction.Demand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public RegisterDcuUpgradeVersionException(SerializationInfo serialInfo, StreamingContext context)
            : base(serialInfo, context)
        {
        }
    }

    [Serializable()]
    public class DcuTestRoutineDataGridObj
    {
        public const string COLUMN_NAME = "Name";
        public const string COLUMN_ADC = "ADC";
        public const string COLUMN_ADC_MIN = "AdcMinTime";
        public const string COLUMN_ADC_MAX = "AdcMaxTime";
        public const string COLUMN_CARD = "Card";
        public const string COLUMN_CARD_MIN = "CardMinTime";
        public const string COLUMN_CARD_MAX = "CardMaxTime";
        public const string COLUMN_CARDTYPE = "CardType";

        Guid _idDcu;
        byte _address;
        string _name;
        bool _online;
        bool _toggleCard;
        bool _toggleADC;
        int _toggleAdcMinTime = 3;
        int _toggleAdcMaxTime = 5;
        int _toggleCardMinTime = 3;
        int _toggleCardMaxTime = 5;
        GeneratedCardType _toggleCardGeneratedCardType = GeneratedCardType.MifareCSN;
        CbGeneratedCardType _cbCardType;

        public string Name { get { return _name; } }
        public bool Card { get { return _toggleCard; } set { _toggleCard = value; } }
        public int AdcMinTime { get { return _toggleAdcMinTime; } set { _toggleAdcMinTime = value; } }
        public int AdcMaxTime { get { return _toggleAdcMaxTime; } set { _toggleAdcMaxTime = value; } }
        public bool ADC { get { return _toggleADC; } set { _toggleADC = value; } }
        public int CardMinTime { get { return _toggleCardMinTime; } set { _toggleCardMinTime = value; } }
        public int CardMaxTime { get { return _toggleCardMaxTime; } set { _toggleCardMaxTime = value; } }
        public CbGeneratedCardType CardType { get { return _cbCardType; } set { _cbCardType = value; } }
        public GeneratedCardType ToggleCardGeneratedCardType { get { return GeneratedCardType.MifareCSN; } }

        public DcuTestRoutineDataGridObj(DCU dcu)
        {
            _idDcu = dcu.IdDCU;
            _address = dcu.LogicalAddress;
            _name = dcu.Name;
            _online = true;
            _toggleCard = false;
            _toggleADC = false;
        }

        public DcuTestRoutineDataGridObj(DcuRunningTest testDcu, DCU dcu)
        {
            _idDcu = dcu.IdDCU;
            _address = dcu.LogicalAddress;
            _name = dcu.Name;
            _online = testDcu._online;
            _toggleCard = testDcu._toggleCard;
            _toggleADC = testDcu._toggleADC;
            _toggleAdcMinTime = testDcu._toggleAdcMinTime;
            _toggleAdcMaxTime = testDcu._toggleAdcMaxTime;
            _toggleCardMinTime = testDcu._toggleCardMinTime;
            _toggleCardMaxTime = testDcu._toggleCardMaxTime;
            _toggleCardGeneratedCardType = (GeneratedCardType)testDcu._toggleCardGeneratedCardType;
        }

        public bool IsOnline()
        {
            return _online;
        }

        public DcuRunningTest GetObjectForCcu()
        {
            DcuRunningTest result = new DcuRunningTest();
            result._idDcu = _idDcu;
            result._address = _address;
            result._online = _online;
            result._toggleCard = _toggleCard;
            result._toggleADC = _toggleADC;
            result._toggleAdcMinTime = _toggleAdcMinTime;
            result._toggleAdcMaxTime = _toggleAdcMaxTime;
            result._toggleCardMinTime = _toggleCardMinTime;
            result._toggleCardMaxTime = _toggleCardMaxTime;
            if (_cbCardType == null)
                result._toggleCardGeneratedCardType = 0;
            else
                result._toggleCardGeneratedCardType = (byte)_cbCardType.CardType;
            return result;
        }
    }

    [Serializable()]
    [LwSerialize(313)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    public class DcuRunningTest
    {
        [LwSerialize()]
        public Guid _idDcu;
        [LwSerializeAttribute()]
        public byte _address;
        [LwSerializeAttribute()]
        public bool _online;
        [LwSerializeAttribute()]
        public bool _toggleCard;
        [LwSerializeAttribute()]
        public bool _toggleADC;
        [LwSerializeAttribute()]
        public int _toggleAdcMinTime;
        [LwSerializeAttribute()]
        public int _toggleAdcMaxTime;
        [LwSerializeAttribute()]
        public int _toggleCardMinTime;
        [LwSerializeAttribute()]
        public int _toggleCardMaxTime;
        [LwSerializeAttribute()]
        public byte _toggleCardGeneratedCardType;

        public DcuRunningTest()
        {
        }
    }

    [Serializable()]
    public class CbGeneratedCardType
    {
        private GeneratedCardType _cardType;
        private string _cardTypeName;

        public GeneratedCardType CardType { get { return _cardType; } set { _cardType = value; } }
        public string LocalizeGeneratedCardType { get { return _cardTypeName; } set { _cardTypeName = value; } }

        public CbGeneratedCardType(GeneratedCardType cardType)
        {
            _cardType = cardType;
            _cardTypeName = cardType.ToString();
        }

        public override string ToString()
        {
            return _cardTypeName;
        }

        public CbGeneratedCardType Self
        {
            get { return this; }
        }
    }
}
