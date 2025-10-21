using System;

using Contal.Cgp.Globals;

namespace Contal.Cgp.NCAS.CCU
{
    public class GuidAndObjectType : IEquatable<GuidAndObjectType>
    {
        public Guid Guid { get; private set; }
        public ObjectType ObjectType { get; private set; }

        public GuidAndObjectType(Guid guid, ObjectType objectType)
        {
            Guid = guid;
            ObjectType = objectType;
        }

        public override int GetHashCode()
        {
            return 
                Guid.GetHashCode() ^ 
                ObjectType.GetHashCode();
        }

        public bool Equals(GuidAndObjectType other)
        {
            if (ReferenceEquals(other, null))
                return false;

            return
                Guid == other.Guid &&
                ObjectType == other.ObjectType;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as GuidAndObjectType);
        }
    }
}