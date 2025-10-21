using System;
using System.Collections.Generic;
using Contal.Cgp.Globals;

using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.CCU
{
    [Serializable()]
    [LwSerialize(264)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    public class ObjectsToSend
    {
        [LwSerialize()]
        public ObjectType ObjectType { get; private set; }

        [LwSerialize()]
        public LinkedList<object> Objects { get; private set; }

        public ObjectsToSend(
            ObjectType objectType,
            LinkedList<object> objects)
        {
            ObjectType = objectType;
            Objects = objects;
        }

        public ObjectsToSend()
        {
            ObjectType = ObjectType.NotSupport;
            Objects = null;
        }
    }
}
