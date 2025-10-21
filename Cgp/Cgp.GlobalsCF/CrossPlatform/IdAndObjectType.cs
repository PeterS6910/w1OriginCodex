using System;

using Contal.IwQuick.Data;

namespace Contal.Cgp.Globals
{
    [Serializable]
    [LwSerialize(703)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class IdAndObjectType : IEquatable<IdAndObjectType>
    {
        public object Id { get; private set; }
        public ObjectType ObjectType { get; private set; }

        public IdAndObjectType(object id, ObjectType objectType)
        {
            Id = id;
            ObjectType = objectType;
        }

        public IdAndObjectType(IdAndObjectType idAndObjectType)
        {
            Id = idAndObjectType.Id;
            ObjectType = idAndObjectType.ObjectType;
        }

        public IdAndObjectType()
        {

        }

        public override int GetHashCode()
        {
            return
                Id.GetHashCode() ^
                ObjectType.GetHashCode();
        }

        public bool Equals(IdAndObjectType other)
        {
            if (ReferenceEquals(other, null))
                return false;

            return
                Id.Equals(other.Id) &&
                ObjectType.Equals(other.ObjectType);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IdAndObjectType);
        }
    }
}
