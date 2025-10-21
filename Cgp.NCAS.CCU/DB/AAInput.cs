using System;

using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Globals;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.CCU.DB
{
    [LwSerialize(301)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    public class AAInput : IDbObject
    {
        [LwSerialize]
        public Guid IdAAInput { get; set; }
        [LwSerialize]
        public int Id { get; set; }
        private Guid _guidInput = Guid.Empty;
        [LwSerialize]
        public Guid GuidInput { get { return _guidInput; } set { _guidInput = value; } }
        [LwSerialize]
        public bool NoCriticalInput { get; set; }
        [LwSerialize]
        public byte BlockTemporarilyUntil { get; set; }
        [LwSerialize]
        public virtual SensorPurpose? Purpose { get; set; }

        public Guid GetGuid()
        {
            return IdAAInput;
        }

        public ObjectType GetObjectType()
        {
            return ObjectType.AAInput;
        }
    }

    public enum BlockTemporarilyUntilType : byte
    {
        AreaUnset = 0,
        SensorStateNormal = 1
    }
}
