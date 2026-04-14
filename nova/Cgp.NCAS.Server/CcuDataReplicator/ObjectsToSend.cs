using System;
using System.Collections.Generic;

using Contal.Cgp.Globals;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.Server.CcuDataReplicator
{
    [Serializable]
    [LwSerialize(264)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    public class ObjectsToSend
    {
        private const int MAXIMAL_OBJECT_COUNT = 100;
        private const int MAXIMAL_ACL_PERSONS_COUNT = 1000;

        private int _maxObjectsCount;

        [LwSerialize]
        public ObjectType ObjectType { get; private set; }

        [LwSerialize]
        public LinkedList<object> Objects { get; private set; }

        public ObjectsToSend()
            : this(ObjectType.NotSupport)
        {
        }

        public ObjectsToSend(ObjectType objectType)
        {
            ObjectType = objectType;
            Objects = new LinkedList<object>();

            _maxObjectsCount = ObjectType == ObjectType.ACLPerson
                ? MAXIMAL_ACL_PERSONS_COUNT
                : MAXIMAL_OBJECT_COUNT;
        }

        public bool TryAddObject(object addObject)
        {
            if (Objects.Count >= _maxObjectsCount)
                return false;

            Objects.AddLast(addObject);

            return true;
        }
    }
}
