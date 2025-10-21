using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Definitions;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.Server.Beans
{
    [Serializable()]
    [LwSerialize(315)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    [LwSerializeNoParent]
    public class DoorEnvironment :
        AOrmObjectWithVersion,
        IOrmObjectWithAlarmInstructions,
        IComparable,
        IGetDcu,
        ICardReaderObject
    {
        public const string COLUMNIDDOORENVIRONMENT = "IdDoorEnvironment";
        public const string COLUMNNAME = "Name";
        public const string COLUMNNUMBER = "Number";
        public const string COLUMNCCU = "CCU";
        public const string COLUMNGUIDCCU = "GuidCCU";
        public const string COLUMNDCU = "DCU";
        public const string COLUMNGUIDDCU = "GuidDCU";

        public const string COLUMNDOORTIMEUNLOCK = "DoorTimeUnlock";
        public const string COLUMNDOORTIMEOPEN = "DoorTimeOpen";
        public const string COLUMNDOORTIMEPREALARM = "DoorTimePreAlarm";
        public const string COLUMNDOORTIMESIRENAJAR = "DoorTimeSirenAjar";
        public const string COLUMNDOORDELAYBEFOREUNLOCK = "DoorDelayBeforeUnlock";
        public const string COLUMNDOORDELAYBEFORELOCK = "DoorDelayBeforeLock";
        public const string COLUMNDOORDELAYBEFORECLOSE = "DoorDelayBeforeClose";
        public const string COLUMNDOORDELAYBEFOREBREAKIN = "DoorDelayBeforeBreakIn";
        public const string COLUMNNOTINVOKEINTRUSIONALARM = "NotInvokeIntrusionAlarm";

        public const string COLUMNSENSORSLOCKDOORS = "SensorsLockDoors";
        public const string COLUMNSENSORSOPENDOOR = "SensorsOpenDoors";
        public const string COLUMNSENSORSOPENMAXDOORS = "SensorsOpenMaxDoors";

        public const string COLUMNGUIDSENSORSLOCKDOORS = "GuidSensorsLockDoors";
        public const string COLUMNGUIDSENSORSOPENDOOR = "GuidSensorsOpenDoors";
        public const string COLUMNGUIDSENSORSOPENMAXDOORS = "GuidSensorsOpenMaxDoors";

        public const string COLUMNSENSORSLOCKDOORSINVERTED = "SensorsLockDoorsInverted";
        public const string COLUMNSENSORSOPENDOORINVERTED = "SensorsOpenDoorsInverted";
        public const string COLUMNSENSORSOPENMAXDOORSINVERTED = "SensorsOpenMaxDoorsInverted";
        public const string COLUMNSENSORSLOCKDOORSBALANCED = "SensorsLockDoorsBalanced";
        public const string COLUMNSENSORSOPENDOORBALANCED = "SensorsOpenDoorsBalanced";
        public const string COLUMNSENSORSOPENMAXDOORSBALANCED = "SensorsOpenMaxDoorsBalanced";


        public const string COLUMNACTUATORSELECTRICSTRIKE = "ActuatorsElectricStrike";
        public const string COLUMNACTUATORSEXTRAELECTRICSTRIKE = "ActuatorsExtraElectricStrike";
        public const string COLUMNACTUATORSDOORENVIRONMENT = "ActuatorsDoorEnvironment";
        public const string COLUMNACTUATORSELECTRICSTRIKEOPPOSITE = "ActuatorsElectricStrikeOpposite";
        public const string COLUMNACTUATORSELECTRICSTRIKEOPPOSITEIMPULSE = "ActuatorsElectricStrikeOppositeImpulse";
        public const string COLUMNACTUATORSBYPASSALARM = "ActuatorsBypassAlarm";

        public const string COLUMNGUIDACTUATORSELECTRICSTRIKE = "GuidActuatorsElectricStrike";
        public const string COLUMNGUIDACTUATORSEXTRAELECTRICSTRIKE = "GuidActuatorsExtraElectricStrike";
        public const string COLUMNGUIDACTUATORSELECTRICSTRIKEOPPOSITE = "GuidActuatorsElectricStrikeOpposite";
        public const string COLUMNGUIDACTUATORSEXTRAELECTRICSTRIKEOPPOSITE = "GuidActuatorsExtraElectricStrikeOpposite";
        public const string COLUMNGUIDACTUATORSBYPASSALARM = "GuidActuatorsBypassAlarm";

        public const string COLUMNACTUATORSELECTRICSTRIKEIMPULSE = "ActuatorsElectricStrikeImpulse";
        public const string COLUMNACTUATORSEXTRAELECTRICSTRIKEIMPULSE = "ActuatorsExtraElectricStrikeImpulse";
        public const string COLUMNACTUATORSELECTRICSTRIKEIMPULSEDELAY = "ActuatorsElectricStrikeImpulseDelay";
        public const string COLUMNACTUATORSEXTRAELECTRICSTRIKEIMPULSEDELAY = "ActuatorsExtraElectricStrikeImpulseDelay";
        public const string COLUMNACTUATORSELECTRICSTRIKEOPPOSITEIMPULSEDELAY = "ActuatorsElectricStrikeOppositeImpulseDelay";
        public const string COLUMNACTUATORSEXTRAELECTRICSTRIKEOPPOSITE = "ActuatorsExtraElectricStrikeOpposite";
        public const string COLUMNACTUATORSEXTRAELECTRICSTRIKEOPPOSITEIMPULSE = "ActuatorsExtraElectricStrikeOppositeImpulse";
        public const string COLUMNACTUATORSEXTRAELECTRICSTRIKEOPPOSITEIMPULSEDELAY = "ActuatorsExtraElectricStrikeOppositeImpulseDelay";

        public const string COLUMNINTRUSIONALARMON = "IntrusionAlarmOn";
        public const string COLUMN_BLOCK_ALARM_INTRUSION = "BlockAlarmIntrusion";
        public const string COLUMN_OBJ_BLOCK_ALARM_INTRUSION_OBJECT_TYPE = "ObjBlockAlarmIntrusionObjectType";
        public const string COLUMN_OBJ_BLOCK_ALARM_INTRUSION_ID = "ObjBlockAlarmIntrusionId";
        public const string COLUMN_OBJ_BLOCK_ALARM_INTRUSION = "ObjBlockAlarmIntrusion";
        public const string COLUMN_EVENTLOG_DURING_BLOCK_ALARM_INTRUSION = "EventlogDuringBlockAlarmIntrusion";

        public const string COLUMNINTRUSIONPRESENTATIONGROUP = "IntrusionPresentationGroup";
        public const string COLUMNINTRUSIONOUTPUT = "IntrusionOutput";
        public const string COLUMNGUIDINTRUSIONOUTPUT = "GuidIntrusionOutput";

        public const string COLUMNDOORAJARALARMON = "DoorAjarAlarmOn";
        public const string COLUMN_BLOCK_ALARM_DOOR_AJAR = "BlockAlarmDoorAjar";
        public const string COLUMN_OBJ_BLOCK_ALARM_DOOR_AJAR_OBJECT_TYPE = "ObjBlockAlarmDoorAjarObjectType";
        public const string COLUMN_OBJ_BLOCK_ALARM_DOOR_AJAR_ID = "ObjBlockAlarmDoorAjarId";
        public const string COLUMN_OBJ_BLOCK_ALARM_DOOR_AJAR = "ObjBlockAlarmDoorAjar";
        public const string COLUMN_EVENTLOG_DURING_BLOCK_ALARM_DOOR_AJAR = "EventlogDuringBlockAlarmDoorAjar";

        public const string COLUMNDOORAJARPRESENTATIONGROUP = "DoorAjarPresentationGroup";
        public const string COLUMNDOORAJAROUTPUT = "DoorAjarOutput";
        public const string COLUMNGUIDDOORAJAROUTPUT = "GuidDoorAjarOutput";

        public const string COLUMNSABOTAGEALARMON = "SabotageAlarmOn";
        public const string COLUMN_BLOCK_ALARM_SABOTAGE = "BlockAlarmSabotage";
        public const string COLUMN_OBJ_BLOCK_ALARM_SABOTAGE_OBJECT_TYPE = "ObjBlockAlarmSabotageObjectType";
        public const string COLUMN_OBJ_BLOCK_ALARM_SABOTAGE_ID = "ObjBlockAlarmSabotageId";
        public const string COLUMN_OBJ_BLOCK_ALARM_SABOTAGE = "ObjBlockAlarmSabotage";
        public const string COLUMN_EVENTLOG_DURING_BLOCK_ALARM_SABOTAGE = "EventlogDuringBlockAlarmSabotage";

        public const string COLUMNSABOTAGEPRESENTATIONGROUP = "SabotagePresentationGroup";
        public const string COLUMNSABOTAGEOUTPUT = "SabotageOutput";
        public const string COLUMNGUIDSABOTAGEOUTPUT = "GuidSabotageOutput";

        public const string COLUMNCARDREADERINTERNAL = "CardReaderInternal";
        public const string COLUMNGUIDCARDREADERINTERNAL = "GuidCardReaderInternal";
        public const string COLUMNPUSHBUTTONINTERNAL = "PushButtonInternal";
        public const string COLUMNGUIDPUSHBUTTONINTERNAL = "GuidPushButtonInternal";
        public const string COLUMNCARDREADEREXTERNAL = "CardReaderExternal";
        public const string COLUMNGUIDCARDREADEREXTERNAL = "GuidCardReaderExternal";
        public const string COLUMNPUSHBUTTONEXTERNAL = "PushButtonExternal";
        public const string COLUMNGUIDPUSHBUTTONEXTERNAL = "GuidPushButtonExternal";

        public const string COLUMNCONFIGURED = "Configured";
        public const string COLUMNDESCRIPTION = "Description";
        public const string COLUMNOBJECTTYPE = "ObjectType";
        public const string COLUMNCKUNIQUE = "CkUnique";
        public const string COLUMNBLOCKEDBYLICENCE = "BlockedByLicence";
        public const string COLUMNLOCALALARMINSTRUCTION = "LocalAlarmInstruction";

        public const string COLUMN_DOOR_ENVIRONMENT_ALARM_ARCS = "DoorEnvironmentAlarmArcs";
        public const string COLUMN_ALARM_TYPE_AND_ID_ALARM_ARCS = "AlarmTypeAndIdAlarmArcs";

        public const string ColumnVersion = "Version";

        [LwSerializeAttribute()]
        public virtual Guid IdDoorEnvironment { get; set; }
        [LwSerializeAttribute()]
        public virtual string Name { get; set; }
        public virtual CCU CCU { get; set; }
        private Guid _guidCCU = Guid.Empty;
        [LwSerializeAttribute()]
        public virtual Guid GuidCCU { get { return _guidCCU; } set { _guidCCU = value; } }
        public virtual DCU DCU { get; set; }
        private Guid _guidDCU = Guid.Empty;
        [LwSerializeAttribute()]
        public virtual Guid GuidDCU { get { return _guidDCU; } set { _guidDCU = value; } }
        [LwSerializeAttribute()]
        public virtual byte Number { get; set; }
        [LwSerializeAttribute()]
        public virtual byte DoorTimeUnlock { get; set; }
        [LwSerializeAttribute()]
        public virtual int DoorTimeOpen { get; set; }
        [LwSerializeAttribute()]
        public virtual byte DoorTimePreAlarm { get; set; }
        [LwSerializeAttribute()]
        public virtual int DoorTimeSirenAjar { get; set; }

        [LwSerializeAttribute()]
        public virtual int DoorDelayBeforeUnlock { get; set; }
        [LwSerializeAttribute()]
        public virtual int DoorDelayBeforeLock { get; set; }
        [LwSerializeAttribute()]
        public virtual int DoorDelayBeforeClose { get; set; }
        [LwSerializeAttribute()]
        public virtual int DoorDelayBeforeBreakIn { get; set; }
        [LwSerializeAttribute()]
        public virtual bool NotInvokeIntrusionAlarm { get; set; }

        public virtual Input SensorsLockDoors { get; set; }
        private Guid _guidSensorsLockDoors = Guid.Empty;
        [LwSerializeAttribute()]
        public virtual Guid GuidSensorsLockDoors { get { return _guidSensorsLockDoors; } set { _guidSensorsLockDoors = value; } }

        public virtual Input SensorsOpenDoors { get; set; }
        private Guid _guidSensorsOpenDoors = Guid.Empty;
        [LwSerializeAttribute()]
        public virtual Guid GuidSensorsOpenDoors { get { return _guidSensorsOpenDoors; } set { _guidSensorsOpenDoors = value; } }

        public virtual Input SensorsOpenMaxDoors { get; set; }
        private Guid _guidSensorsOpenMaxDoors = Guid.Empty;
        [LwSerializeAttribute()]
        public virtual Guid GuidSensorsOpenMaxDoors { get { return _guidSensorsOpenMaxDoors; } set { _guidSensorsOpenMaxDoors = value; } }

        [LwSerializeAttribute()]
        public virtual bool SensorsLockDoorsInverted { get; set; }
        [LwSerializeAttribute()]
        public virtual bool SensorsOpenDoorsInverted { get; set; }
        [LwSerializeAttribute()]
        public virtual bool SensorsOpenMaxDoorsInverted { get; set; }
        [LwSerializeAttribute()]
        public virtual bool SensorsLockDoorsBalanced { get; set; }
        [LwSerializeAttribute()]
        public virtual bool SensorsOpenDoorsBalanced { get; set; }
        [LwSerializeAttribute()]
        public virtual bool SensorsOpenMaxDoorsBalanced { get; set; }

        [LwSerializeAttribute()]
        public virtual byte? ActuatorsDoorEnvironment { get; set; }

        public virtual Output ActuatorsElectricStrike { get; set; }
        private Guid _guidActuatorsElectricStrike = Guid.Empty;
        [LwSerializeAttribute()]
        public virtual Guid GuidActuatorsElectricStrike { get { return _guidActuatorsElectricStrike; } set { _guidActuatorsElectricStrike = value; } }

        public virtual Output ActuatorsElectricStrikeOpposite { get; set; }
        private Guid _guidActuatorsElectricStrikeOpposite = Guid.Empty;
        [LwSerializeAttribute()]
        public virtual Guid GuidActuatorsElectricStrikeOpposite { get { return _guidActuatorsElectricStrikeOpposite; } set { _guidActuatorsElectricStrikeOpposite = value; } }

        public virtual Output ActuatorsExtraElectricStrike { get; set; }
        private Guid _guidActuatorsExtraElectricStrike = Guid.Empty;
        [LwSerializeAttribute()]
        public virtual Guid GuidActuatorsExtraElectricStrike { get { return _guidActuatorsExtraElectricStrike; } set { _guidActuatorsExtraElectricStrike = value; } }

        public virtual Output ActuatorsExtraElectricStrikeOpposite { get; set; }
        private Guid _guidActuatorsExtraElectricStrikeOpposite = Guid.Empty;
        [LwSerializeAttribute()]
        public virtual Guid GuidActuatorsExtraElectricStrikeOpposite { get { return _guidActuatorsExtraElectricStrikeOpposite; } set { _guidActuatorsExtraElectricStrikeOpposite = value; } }

        public virtual Output ActuatorsBypassAlarm { get; set; }
        private Guid _guidActuatorsBypassAlarm = Guid.Empty;
        [LwSerializeAttribute()]
        public virtual Guid GuidActuatorsBypassAlarm { get { return _guidActuatorsBypassAlarm; } set { _guidActuatorsBypassAlarm = value; } }

        [LwSerializeAttribute()]
        public virtual bool ActuatorsElectricStrikeImpulse { get; set; }
        [LwSerializeAttribute()]
        public virtual int ActuatorsElectricStrikeImpulseDelay { get; set; }
        [LwSerializeAttribute()]
        public virtual bool ActuatorsElectricStrikeOppositeImpulse { get; set; }
        [LwSerializeAttribute()]
        public virtual int ActuatorsElectricStrikeOppositeImpulseDelay { get; set; }
        [LwSerializeAttribute()]
        public virtual bool ActuatorsExtraElectricStrikeImpulse { get; set; }
        [LwSerializeAttribute()]
        public virtual int ActuatorsExtraElectricStrikeImpulseDelay { get; set; }
        [LwSerializeAttribute()]
        public virtual bool ActuatorsExtraElectricStrikeOppositeImpulse { get; set; }
        [LwSerializeAttribute()]
        public virtual int ActuatorsExtraElectricStrikeOppositeImpulseDelay { get; set; }

        [LwSerializeAttribute()]
        public virtual bool? IntrusionAlarmOn { get; set; }
        [LwSerializeAttribute()]
        public virtual bool? BlockAlarmIntrusion { get; set; }
        [LwSerializeAttribute()]
        public virtual byte? ObjBlockAlarmIntrusionObjectType { get; set; }
        [LwSerializeAttribute()]
        public virtual Guid? ObjBlockAlarmIntrusionId { get; set; }

        public virtual PresentationGroup IntrusionPresentationGroup { get; set; }

        public virtual Output IntrusionOutput { get; set; }
        private Guid _guidIntrusionOutput = Guid.Empty;
        [LwSerializeAttribute()]
        public virtual Guid GuidIntrusionOutput { get { return _guidIntrusionOutput; } set { _guidIntrusionOutput = value; } }

        [LwSerializeAttribute()]
        public virtual bool? DoorAjarAlarmOn { get; set; }
        [LwSerializeAttribute()]
        public virtual bool? BlockAlarmDoorAjar { get; set; }
        [LwSerializeAttribute()]
        public virtual byte? ObjBlockAlarmDoorAjarObjectType { get; set; }
        [LwSerializeAttribute()]
        public virtual Guid? ObjBlockAlarmDoorAjarId { get; set; }

        public virtual PresentationGroup DoorAjarPresentationGroup { get; set; }

        public virtual Output DoorAjarOutput { get; set; }
        private Guid _guidDoorAjarOutput = Guid.Empty;
        [LwSerializeAttribute()]
        public virtual Guid GuidDoorAjarOutput { get { return _guidDoorAjarOutput; } set { _guidDoorAjarOutput = value; } }

        [LwSerializeAttribute()]
        public virtual bool? SabotageAlarmOn { get; set; }
        [LwSerializeAttribute()]
        public virtual bool? BlockAlarmSabotage { get; set; }
        [LwSerializeAttribute()]
        public virtual byte? ObjBlockAlarmSabotageObjectType { get; set; }
        [LwSerializeAttribute()]
        public virtual Guid? ObjBlockAlarmSabotageId { get; set; }

        public virtual PresentationGroup SabotagePresentationGroup { get; set; }

        public virtual Output SabotageOutput { get; set; }
        private Guid _guidSabotageOutput = Guid.Empty;
        [LwSerializeAttribute()]
        public virtual Guid GuidSabotageOutput { get { return _guidSabotageOutput; } set { _guidSabotageOutput = value; } }

        public virtual CardReader CardReaderInternal { get; set; }
        private Guid _guidCardReaderInternal = Guid.Empty;
        [LwSerializeAttribute()]
        public virtual Guid GuidCardReaderInternal { get { return _guidCardReaderInternal; } set { _guidCardReaderInternal = value; } }

        public virtual Input PushButtonInternal { get; set; }
        private Guid _guidPushButtonInternal = Guid.Empty;
        [LwSerializeAttribute()]
        public virtual Guid GuidPushButtonInternal { get { return _guidPushButtonInternal; } set { _guidPushButtonInternal = value; } }

        public virtual CardReader CardReaderExternal { get; set; }
        private Guid _guidCardReaderExternal = Guid.Empty;
        [LwSerializeAttribute()]
        public virtual Guid GuidCardReaderExternal { get { return _guidCardReaderExternal; } set { _guidCardReaderExternal = value; } }

        public virtual Input PushButtonExternal { get; set; }
        private Guid _guidPushButtonExternal = Guid.Empty;
        [LwSerializeAttribute()]
        public virtual Guid GuidPushButtonExternal { get { return _guidPushButtonExternal; } set { _guidPushButtonExternal = value; } }
        public virtual string Description { get; set; }
        public virtual byte ObjectType { get; set; }
        public virtual Guid CkUnique { get; set; }

        [LwSerializeAttribute()]
        public virtual bool BlockedByLicence { get; set; }
        public virtual string LocalAlarmInstruction { get; set; }

        public virtual ICollection<DoorEnvironmentAlarmArc> DoorEnvironmentAlarmArcs { get; set; }
        [LwSerialize]
        public virtual List<AlarmTypeAndIdAlarmArc> AlarmTypeAndIdAlarmArcs { get; set; }

        public DoorEnvironment()
        {
            DoorTimeUnlock = 5;
            DoorTimeOpen = 15;
            DoorTimePreAlarm = 5;
            DoorDelayBeforeUnlock = 0;
            DoorDelayBeforeLock = 0;
            DoorDelayBeforeClose = 1000;
            IntrusionAlarmOn = true;
            SabotageAlarmOn = true;
            ObjectType = (byte)Cgp.Globals.ObjectType.DoorEnvironment;
            CkUnique = System.Guid.NewGuid();
        }

        public virtual bool Configured
        {
            get
            {
                return ActuatorsDoorEnvironment != null || SensorsLockDoors != null || SensorsOpenDoors != null ||
                    SensorsOpenMaxDoors != null || ActuatorsElectricStrike != null || ActuatorsExtraElectricStrike != null ||
                    ActuatorsElectricStrikeOpposite != null || ActuatorsExtraElectricStrikeOpposite != null ||
                    ActuatorsBypassAlarm != null || CardReaderExternal != null || CardReaderInternal != null ||
                    PushButtonExternal != null || PushButtonInternal != null;
            }
        }

        public override string ToString()
        {
            string name = string.Empty;
            if (CCU != null)
            {
                name += CCU.Name + StringConstants.SLASH;
            }
            else if (DCU != null)
            {
                name += DCU.ToString() + StringConstants.SLASH;
            }

            name += Name;
            return name;
        }

        public override bool Compare(object obj)
        {
            if (obj == null)
                return false;

            if (obj is DoorEnvironment)
            {
                return (obj as DoorEnvironment).IdDoorEnvironment == IdDoorEnvironment;
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

            if (DCU != null)
            {
                GuidDCU = DCU.IdDCU;
            }
            else
            {
                GuidDCU = Guid.Empty;
            }

            if (SensorsLockDoors != null)
                GuidSensorsLockDoors = SensorsLockDoors.IdInput;
            else
                GuidSensorsLockDoors = Guid.Empty;

            if (SensorsOpenDoors != null)
                GuidSensorsOpenDoors = SensorsOpenDoors.IdInput;
            else
                GuidSensorsOpenDoors = Guid.Empty;

            if (SensorsOpenMaxDoors != null)
                GuidSensorsOpenMaxDoors = SensorsOpenMaxDoors.IdInput;
            else
                GuidSensorsOpenMaxDoors = Guid.Empty;

            if (ActuatorsElectricStrike != null)
                GuidActuatorsElectricStrike = ActuatorsElectricStrike.IdOutput;
            else
                GuidActuatorsElectricStrike = Guid.Empty;

            if (ActuatorsElectricStrikeOpposite != null)
                GuidActuatorsElectricStrikeOpposite = ActuatorsElectricStrikeOpposite.IdOutput;
            else
                GuidActuatorsElectricStrikeOpposite = Guid.Empty;

            if (ActuatorsExtraElectricStrike != null)
                GuidActuatorsExtraElectricStrike = ActuatorsExtraElectricStrike.IdOutput;
            else
                GuidActuatorsExtraElectricStrike = Guid.Empty;

            if (ActuatorsExtraElectricStrikeOpposite != null)
                GuidActuatorsExtraElectricStrikeOpposite = ActuatorsExtraElectricStrikeOpposite.IdOutput;
            else
                GuidActuatorsExtraElectricStrikeOpposite = Guid.Empty;

            if (ActuatorsBypassAlarm != null)
                GuidActuatorsBypassAlarm = ActuatorsBypassAlarm.IdOutput;
            else
                GuidActuatorsBypassAlarm = Guid.Empty;

            if (IntrusionOutput != null)
                GuidIntrusionOutput = IntrusionOutput.IdOutput;
            else
                GuidIntrusionOutput = Guid.Empty;

            if (DoorAjarOutput != null)
                GuidDoorAjarOutput = DoorAjarOutput.IdOutput;
            else
                GuidDoorAjarOutput = Guid.Empty;

            if (SabotageOutput != null)
                GuidSabotageOutput = SabotageOutput.IdOutput;
            else
                GuidSabotageOutput = Guid.Empty;

            if (CardReaderInternal != null)
                GuidCardReaderInternal = CardReaderInternal.IdCardReader;
            else
                GuidCardReaderInternal = Guid.Empty;

            if (PushButtonInternal != null)
                GuidPushButtonInternal = PushButtonInternal.IdInput;
            else
                GuidPushButtonInternal = Guid.Empty;

            if (CardReaderExternal != null)
                GuidCardReaderExternal = CardReaderExternal.IdCardReader;
            else
                GuidCardReaderExternal = Guid.Empty;

            if (PushButtonExternal != null)
                GuidPushButtonExternal = PushButtonExternal.IdInput;
            else
                GuidPushButtonExternal = Guid.Empty;

            AlarmTypeAndIdAlarmArcs = DoorEnvironmentAlarmArcs == null || DoorEnvironmentAlarmArcs.Count == 0
                ? null
                : new List<AlarmTypeAndIdAlarmArc>(
                    DoorEnvironmentAlarmArcs.Select(
                        doorEnvironemntAlarmArc =>
                            new AlarmTypeAndIdAlarmArc(
                                (AlarmType)doorEnvironemntAlarmArc.AlarmType,
                                doorEnvironemntAlarmArc.AlarmArc.IdAlarmArc)));
        }

        public override string GetIdString()
        {
            return IdDoorEnvironment.ToString();
        }

        public override object GetId()
        {
            return IdDoorEnvironment;
        }

        public override IModifyObject CreateModifyObject()
        {
            return new DoorEnvironmentModifyObj(this);
        }

        public virtual IEnumerable<ICardReaderObject> GetChildObjects()
        {
            if (CardReaderInternal != null)
                yield return CardReaderInternal;

            if (CardReaderExternal != null)
                yield return CardReaderExternal;
        }

        public override Contal.Cgp.Globals.ObjectType GetObjectType()
        {
            return Contal.Cgp.Globals.ObjectType.DoorEnvironment;
        }

        public override string GetDeviceShortName()
        {
            return Name;
        }

        #region IComparable Members

        public virtual int CompareTo(object obj)
        {
            if (obj == null) return 1;

            if (obj is DoorEnvironment)
            {
                DoorEnvironment otherDE = obj as DoorEnvironment;

                if (otherDE.CCU != null && this.CCU != null)
                {
                    int i = this.CCU.Name.CompareTo(otherDE.CCU.Name);
                    if (i != 0)
                        return i;
                }

                if (otherDE.DCU != null && this.DCU != null)
                {
                    int i = this.DCU.Name.CompareTo(otherDE.DCU.Name);
                    if (i != 0)
                        return i;
                }

                return this.Name.CompareTo(otherDE.Name);
            }
            else
            {
                return 1;
            }
        }

        #endregion

        public virtual string GetLocalAlarmInstruction()
        {
            return LocalAlarmInstruction;
        }

        public virtual DCU GetDcu()
        {
            return DCU;
        }
    }

    [Serializable()]
    public class DoorEnvironmentShort : IShortObject
    {
        public const string COLUMNIDDOORENVIRONMENT = "IdDoorEnvironment";
        public const string COLUMNNAME = "Name";
        public const string COLUMNFULLNAME = "FullName";
        public const string COLUMNSTATE = "State";
        public const string COLUMNSTRINGSTATE = "StringState";
        public const string COLUMNCONFIGURED = "Configured";
        public const string COLUMNDESCRIPTION = "Description";
        public const string COLUMN_SYMBOL = "Symbol";

        public Guid IdDoorEnvironment { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public DoorEnvironmentState State { get; set; }
        public string StringState { get; set; }
        public bool Configured { get; set; }
        public string Description { get; set; }
        public Image Symbol { get; set; }
        public Guid IdCcu { get; set; }
        public bool HasDcu { get; set; }

        public DoorEnvironmentShort(DoorEnvironment doorEnvironment)
        {
            IdDoorEnvironment = doorEnvironment.IdDoorEnvironment;
            Name = doorEnvironment.Name;
            FullName = doorEnvironment.ToString();
            Description = doorEnvironment.Description;

            if (doorEnvironment.CCU != null)
                IdCcu = doorEnvironment.CCU.IdCCU;
            else if (doorEnvironment.DCU != null)
            {
                IdCcu = doorEnvironment.DCU.CCU.IdCCU;
                HasDcu = true;
            }
        }

        public override string ToString()
        {
            return FullName;
        }

        #region IShortObject Members

        string IShortObject.Name { get { return FullName; } }

        public ObjectType ObjectType { get { return ObjectType.DoorEnvironment; } }

        public string GetSubTypeImageString(object value)
        {
            return string.Empty;
        }

        public object Id { get { return IdDoorEnvironment; } }

        #endregion
    }

    [Serializable()]
    public class DoorEnvironmentModifyObj : AModifyObject
    {
        public override ObjectType GetOrmObjectType { get { return ObjectType.DoorEnvironment; } }

        public DoorEnvironmentModifyObj(DoorEnvironment doorEnvironment)
        {
            Id = doorEnvironment.IdDoorEnvironment;
            FullName = doorEnvironment.ToString();
            Description = doorEnvironment.Description;
        }
    }

    public class DoorEnvironmentStateChangedHandler : Contal.IwQuick.Remoting.ARemotingCallbackHandler
    {
        private static volatile DoorEnvironmentStateChangedHandler _singleton = null;
        private static object _syncRoot = new object();

        private Action<Guid, byte> _stateChanged;

        public static DoorEnvironmentStateChangedHandler Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (_singleton == null)
                            _singleton = new DoorEnvironmentStateChangedHandler();
                    }

                return _singleton;
            }
        }

        public DoorEnvironmentStateChangedHandler()
            : base("DoorEnvironmentStateChangedHandler")
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
}
