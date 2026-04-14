using System;
using System.IO;

using Contal.Cgp.Globals;
using Contal.IwQuick.Data;
using Contal.LwSerialization;

namespace Contal.Cgp.NCAS.CCU.DB
{
    [LwSerialize(322)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    public class MultiDoorElement : IDbObject
    {
        [LwSerializeAttribute]
        public virtual Guid IdMultiDoorElement { get; set; }

        [LwSerializeAttribute]
        public virtual Guid MultiDoorId { get; set; }

        [LwSerializeAttribute]
        public virtual Guid FloorId { get; set; }

        [LwSerializeAttribute]
        public virtual Guid? SensorOpenDoorId { get; set; }

        [LwSerializeAttribute]
        public virtual Guid ElectricStrikeId { get; set; }

        [LwSerializeAttribute]
        public virtual Guid? ExtraElectricStrikeId { get; set; }

        [LwSerializeAttribute]
        public virtual Guid? BlockAlarmAreaId { get; set; }

        [LwSerializeAttribute]
        public virtual ObjectType? BlockOnOffObjectType { get; set; }

        [LwSerializeAttribute]
        public virtual Guid? BlockOnOffObjectId { get; set; }

        public Guid GetGuid()
        {
            return IdMultiDoorElement;
        }

        public ObjectType GetObjectType()
        {
            return ObjectType.MultiDoorElement;
        }
    }
}
