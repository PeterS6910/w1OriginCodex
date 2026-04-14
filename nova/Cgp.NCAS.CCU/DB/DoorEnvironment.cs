using System;
using System.Collections.Generic;
using System.IO;

using Contal.IwQuick.Data;
using Contal.LwSerialization;

namespace Contal.Cgp.NCAS.CCU.DB
{
    [LwSerialize(315)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    public class DoorEnvironment : IDbObject
    {
        [LwSerialize]
        public virtual Guid IdDoorEnvironment { get; set; }
        [LwSerialize]
        public virtual string Name { get; set; }
        private Guid _guidCCU = Guid.Empty;
        [LwSerialize]
        public virtual Guid GuidCCU { get { return _guidCCU; } set { _guidCCU = value; } }
        private Guid _guidDCU = Guid.Empty;
        [LwSerialize]
        public virtual Guid GuidDCU { get { return _guidDCU; } set { _guidDCU = value; } }
        [LwSerialize]
        public virtual byte Number { get; set; }
        [LwSerialize]
        public virtual byte DoorTimeUnlock { get; set; }
        [LwSerialize]
        public virtual int DoorTimeOpen { get; set; }
        [LwSerialize]
        public virtual byte DoorTimePreAlarm { get; set; }
        [LwSerialize]
        public virtual int DoorTimeSirenAjar { get; set; }

        [LwSerialize]
        public virtual int DoorDelayBeforeUnlock { get; set; }
        [LwSerialize]
        public virtual int DoorDelayBeforeLock { get; set; }
        [LwSerialize]
        public virtual int DoorDelayBeforeClose { get; set; }
        [LwSerialize]
        public virtual int DoorDelayBeforeBreakIn { get; set; }
        [LwSerialize]

        private Guid _guidSensorsLockDoors = Guid.Empty;
        [LwSerialize]
        public virtual Guid GuidSensorsLockDoors { get { return _guidSensorsLockDoors; } set { _guidSensorsLockDoors = value; } }

        private Guid _guidSensorsOpenDoors = Guid.Empty;
        [LwSerialize]
        public virtual Guid GuidSensorsOpenDoors { get { return _guidSensorsOpenDoors; } set { _guidSensorsOpenDoors = value; } }

        private Guid _guidSensorsOpenMaxDoors = Guid.Empty;
        [LwSerialize]
        public virtual Guid GuidSensorsOpenMaxDoors { get { return _guidSensorsOpenMaxDoors; } set { _guidSensorsOpenMaxDoors = value; } }

        [LwSerialize]
        public virtual bool SensorsLockDoorsInverted { get; set; }
        [LwSerialize]
        public virtual bool SensorsOpenDoorsInverted { get; set; }
        [LwSerialize]
        public virtual bool SensorsOpenMaxDoorsInverted { get; set; }
        [LwSerialize]
        public virtual bool SensorsLockDoorsBalanced { get; set; }
        [LwSerialize]
        public virtual bool SensorsOpenDoorsBalanced { get; set; }
        [LwSerialize]
        public virtual bool SensorsOpenMaxDoorsBalanced { get; set; }

        [LwSerialize]
        public virtual byte? ActuatorsDoorEnvironment { get; set; }

        private Guid _guidActuatorsElectricStrike = Guid.Empty;
        [LwSerialize]
        public virtual Guid GuidActuatorsElectricStrike { get { return _guidActuatorsElectricStrike; } set { _guidActuatorsElectricStrike = value; } }

        private Guid _guidActuatorsElectricStrikeOpposite = Guid.Empty;
        [LwSerialize]
        public virtual Guid GuidActuatorsElectricStrikeOpposite { get { return _guidActuatorsElectricStrikeOpposite; } set { _guidActuatorsElectricStrikeOpposite = value; } }

        private Guid _guidActuatorsExtraElectricStrike = Guid.Empty;
        [LwSerialize]
        public virtual Guid GuidActuatorsExtraElectricStrike { get { return _guidActuatorsExtraElectricStrike; } set { _guidActuatorsExtraElectricStrike = value; } }

        private Guid _guidActuatorsExtraElectricStrikeOpposite = Guid.Empty;
        [LwSerialize]
        public virtual Guid GuidActuatorsExtraElectricStrikeOpposite { get { return _guidActuatorsExtraElectricStrikeOpposite; } set { _guidActuatorsExtraElectricStrikeOpposite = value; } }

        private Guid _guidActuatorsBypassAlarm = Guid.Empty;
        [LwSerialize]
        public virtual Guid GuidActuatorsBypassAlarm { get { return _guidActuatorsBypassAlarm; } set { _guidActuatorsBypassAlarm = value; } }

        [LwSerialize]
        public virtual bool ActuatorsElectricStrikeImpulse { get; set; }
        [LwSerialize]
        public virtual int ActuatorsElectricStrikeImpulseDelay { get; set; }
        [LwSerialize]
        public virtual bool ActuatorsElectricStrikeOppositeImpulse { get; set; }
        [LwSerialize]
        public virtual int ActuatorsElectricStrikeOppositeImpulseDelay { get; set; }
        [LwSerialize]
        public virtual bool ActuatorsExtraElectricStrikeImpulse { get; set; }
        [LwSerialize]
        public virtual int ActuatorsExtraElectricStrikeImpulseDelay { get; set; }
        [LwSerialize]
        public virtual bool ActuatorsExtraElectricStrikeOppositeImpulse { get; set; }
        [LwSerialize]
        public virtual int ActuatorsExtraElectricStrikeOppositeImpulseDelay { get; set; }

        [LwSerialize]
        public virtual bool? IntrusionAlarmOn { get; set; }
        [LwSerialize]
        public virtual bool? BlockAlarmIntrusion { get; set; }
        [LwSerialize]
        public virtual byte? ObjBlockAlarmIntrusionObjectType { get; set; }
        [LwSerialize]
        public virtual Guid? ObjBlockAlarmIntrusionId { get; set; }

        private Guid _guidIntrusionOutput = Guid.Empty;
        [LwSerialize]
        public virtual Guid GuidIntrusionOutput { get { return _guidIntrusionOutput; } set { _guidIntrusionOutput = value; } }

        [LwSerialize]
        public virtual bool? DoorAjarAlarmOn { get; set; }
        [LwSerialize]
        public virtual bool? BlockAlarmDoorAjar { get; set; }
        [LwSerialize]
        public virtual byte? ObjBlockAlarmDoorAjarObjectType { get; set; }
        [LwSerialize]
        public virtual Guid? ObjBlockAlarmDoorAjarId { get; set; }

        private Guid _guidDoorAjarOutput = Guid.Empty;
        [LwSerialize]
        public virtual Guid GuidDoorAjarOutput { get { return _guidDoorAjarOutput; } set { _guidDoorAjarOutput = value; } }

        [LwSerialize]
        public virtual bool? SabotageAlarmOn { get; set; }
        [LwSerialize]
        public virtual bool? BlockAlarmSabotage { get; set; }
        [LwSerialize]
        public virtual byte? ObjBlockAlarmSabotageObjectType { get; set; }
        [LwSerialize]
        public virtual Guid? ObjBlockAlarmSabotageId { get; set; }

        private Guid _guidSabotageOutput = Guid.Empty;
        [LwSerialize]
        public virtual Guid GuidSabotageOutput { get { return _guidSabotageOutput; } set { _guidSabotageOutput = value; } }

        private Guid _guidCardReaderInternal = Guid.Empty;
        [LwSerialize]
        public virtual Guid GuidCardReaderInternal { get { return _guidCardReaderInternal; } set { _guidCardReaderInternal = value; } }

        private Guid _guidPushButtonInternal = Guid.Empty;
        [LwSerialize]
        public virtual Guid GuidPushButtonInternal { get { return _guidPushButtonInternal; } set { _guidPushButtonInternal = value; } }

        private Guid _guidCardReaderExternal = Guid.Empty;
        [LwSerialize]
        public virtual Guid GuidCardReaderExternal { get { return _guidCardReaderExternal; } set { _guidCardReaderExternal = value; } }

        private Guid _guidPushButtonExternal = Guid.Empty;
        [LwSerialize]
        public virtual Guid GuidPushButtonExternal { get { return _guidPushButtonExternal; } set { _guidPushButtonExternal = value; } }

        private bool _blockedByLicence = false;
        [LwSerialize]
        public virtual bool BlockedByLicence { get { return _blockedByLicence; } set { _blockedByLicence = value; } }

        [LwSerialize]
        public List<AlarmTypeAndIdAlarmArc> AlarmTypeAndIdAlarmArcs { get; set; }

        public Guid GetGuid()
        {
            return IdDoorEnvironment;
        }

        public Contal.Cgp.Globals.ObjectType GetObjectType()
        {
            return Contal.Cgp.Globals.ObjectType.DoorEnvironment;
        }
    }
}
