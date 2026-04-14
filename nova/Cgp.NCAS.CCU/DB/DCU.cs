using System;

using System.Collections.Generic;

using Contal.IwQuick.Data;
using Contal.LwSerialization;
using System.IO;

namespace Contal.Cgp.NCAS.CCU.DB
{
    [LwSerialize(312)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    public class DCU : IDbObject
    {
        [LwSerialize()]
        public virtual Guid IdDCU { get; set; }
        private List<Guid> _guidCardReaders = new List<Guid>();
        [LwSerialize()]
        public virtual List<Guid> GuidCardReaders { get { return _guidCardReaders; } set { _guidCardReaders = value; } }
        private List<Guid> _guidDoorEnvironments = new List<Guid>();
        [LwSerialize()]
        public virtual List<Guid> GuidDoorEnvironments { get { return _guidDoorEnvironments; } set { _guidDoorEnvironments = value; } }
        private Guid _guidCCU = Guid.Empty;
        [LwSerialize()]
        public virtual Guid GuidCCU { get { return _guidCCU; } set { _guidCCU = value; } }
        private List<Guid> _guidInputs = new List<Guid>();
        [LwSerialize()]
        public virtual List<Guid> GuidInputs { get { return _guidInputs; } set { _guidInputs = value; } }
        private List<Guid> _guidOutputs = new List<Guid>();
        [LwSerialize()]
        public virtual List<Guid> GuidOutputs { get { return _guidOutputs; } set { _guidOutputs = value; } }
        [LwSerialize()]
        public virtual byte LogicalAddress { get; set; }

        [LwSerialize]
        public virtual bool? AlarmOffline { get; set; }
        [LwSerialize]
        public virtual bool? BlockAlarmOffline { get; set; }
        [LwSerialize]
        public virtual byte? ObjBlockAlarmOfflineObjectType { get; set; }
        [LwSerialize]
        public virtual Guid? ObjBlockAlarmOfflineId { get; set; }

        [LwSerialize()]
        public virtual bool? AlarmTamper { get; set; }
        [LwSerialize()]
        public virtual bool? BlockAlarmTamper { get; set; }
        [LwSerialize()]
        public virtual byte? ObjBlockAlarmTamperObjectType { get; set; }
        [LwSerialize()]
        public virtual Guid? ObjBlockAlarmTamperId { get; set; }
        [LwSerialize()]
        public virtual bool? EventlogDuringBlockAlarmTamper { get; set; }

        private Guid _guidDcuSabotageOutput = Guid.Empty;
        [LwSerialize()]
        public virtual Guid GuidDcuSabotageOutput { get { return _guidDcuSabotageOutput; } set { _guidDcuSabotageOutput = value; } }
        private Guid _guidDcuOfflineOutput = Guid.Empty;
        [LwSerialize()]
        public virtual Guid GuidDcuOfflineOutput { get { return _guidDcuOfflineOutput; } set { _guidDcuOfflineOutput = value; } }
        private Guid _guidDcuInputsSabotageOutput = Guid.Empty;
        [LwSerialize()]
        public virtual Guid GuidDcuInputsSabotageOutput { get { return _guidDcuInputsSabotageOutput; } set { _guidDcuInputsSabotageOutput = value; } }

        [LwSerialize]
        public List<AlarmTypeAndIdAlarmArc> AlarmTypeAndIdAlarmArcs { get; set; }

        public Guid GetGuid()
        {
            return IdDCU;
        }

        public Cgp.Globals.ObjectType GetObjectType()
        {
            return Cgp.Globals.ObjectType.DCU;
        }

        public override string ToString()
        {
            return "DCU" + LogicalAddress;
        }
    }
}
