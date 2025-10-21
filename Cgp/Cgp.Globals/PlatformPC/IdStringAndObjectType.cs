using System;

namespace Contal.Cgp.Globals
{
    public class IdStringAndObjectType : IEquatable<IdStringAndObjectType>
    {
        public string IdString { get; private set; }
        public ObjectType ObjectType { get; private set; }

        public IdStringAndObjectType(string idString, ObjectType objectType)
        {
            IdString = idString;
            ObjectType = objectType;
        }

        public bool Equals(IdStringAndObjectType other)
        {
            return
                IdString == other.IdString &&
                ObjectType == other.ObjectType;
        }

        public override bool Equals(object obj)
        {
            var other =
                obj as IdStringAndObjectType;

            return other != null && Equals(other);
        }

        public override int GetHashCode()
        {
            return
                IdString.GetHashCode() ^
                ObjectType.GetHashCode();
        }
    }
}