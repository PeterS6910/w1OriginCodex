using System;
using System.IO;

using Contal.Cgp.Globals;
using Contal.IwQuick.Data;
using Contal.LwSerialization;

namespace Contal.Cgp.NCAS.CCU.DB
{
    [LwSerialize(321)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    public class MultiDoor : IDbObject
    {
        [LwSerialize]
        public virtual Guid IdMultiDoor { get; set; }

        [LwSerializeAttribute]
        public virtual Guid CardReaderId { get; set; }

        [LwSerializeAttribute]
        public virtual byte DoorTimeUnlock { get; set; }

        [LwSerializeAttribute]
        public virtual int DoorTimeOpen { get; set; }

        [LwSerializeAttribute]
        public virtual byte DoorTimePreAlarm { get; set; }

        public Guid GetGuid()
        {
            return IdMultiDoor;
        }

        public ObjectType GetObjectType()
        {
            return ObjectType.MultiDoor;
        }
    }
}
