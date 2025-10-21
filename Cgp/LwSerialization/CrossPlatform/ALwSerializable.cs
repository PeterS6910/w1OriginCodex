using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

#if COMPACT_FRAMEWORK
namespace Contal.LwSerializationCF
#else
namespace Contal.LwSerialization
#endif
{
    public abstract class ALwSerializable
    {
        private readonly Hashtable _markTable;

#if DEBUG
        static public
#else
        static protected internal
#endif
 object GetValue(MemberInfo memberInfo, Object fromObject)
        {
            if (memberInfo == null)
                throw new ArgumentNullException("Member info for the field or property is null");

            if (memberInfo.MemberType == MemberTypes.Field)
                return ((FieldInfo) memberInfo).GetValue(fromObject);

            if (memberInfo.MemberType != MemberTypes.Property)
                throw new ArgumentException(
                    "Member is not field neither property");

            PropertyInfo propertyInfo = (PropertyInfo)memberInfo;

            if (!propertyInfo.CanRead)
                throw new ArgumentException("Property " + propertyInfo.Name + " has no get part");

            return propertyInfo.GetValue(fromObject, null);
        }

#if DEBUG
        static public
#else
        static protected internal
#endif
        void SetValue(MemberInfo memberInfo, Object value, Object toObject)
        {
            if (null == memberInfo)
                throw new ArgumentNullException("Member info for the field or property is null");

            if (memberInfo.MemberType == MemberTypes.Field)
            {
                ((FieldInfo)memberInfo).SetValue(toObject, value);
                return;
            }

            if (memberInfo.MemberType != MemberTypes.Property)
                throw new ArgumentException(
                    "Member is not field neither property");

            PropertyInfo propertyInfo = (PropertyInfo)memberInfo;

            if (!propertyInfo.CanWrite)
                throw new ArgumentException("Property " + propertyInfo.Name + " has no set part");

            propertyInfo.SetValue(toObject, value, null);
        }

#if DEBUG
        static public
#else
        static protected internal
#endif
        Type GetTypeOfMember(MemberInfo memberInfo)
        {
            if (memberInfo.MemberType == MemberTypes.Field)
            {
                return ((FieldInfo) memberInfo).FieldType;
            }

            if (memberInfo.MemberType == MemberTypes.Property)
            {
                return ((PropertyInfo) memberInfo).PropertyType;
            }

            throw new ArgumentException("Member is not field neither property");
        }

#if DEBUG
        static public
#else
        static protected internal
#endif
        Version GetVersion(Type type)
        {
            var result = new Version(1, 0, 0, 0);
            
            object[] atributes = 
                type.GetCustomAttributes(
                    typeof(LwSerializeVersionAttribute), 
                    false);

            if (atributes.Length <= 0)
                return result;

            foreach (object atribute in atributes)
                result =
                    ((LwSerializeVersionAttribute) atribute).Version;

            return result;
        }

        protected ALwSerializable()
        {
            Type objectType = GetType();

            var lwSerializeAssignAttribute =
                LwBinarySerializationFunctions.LwSerializeAssignCache
                    .GetValue(objectType);

            if (lwSerializeAssignAttribute.AssignMode !=
                        AssignMode.LenientDynamic &&
                    lwSerializeAssignAttribute.AssignMode !=
                        AssignMode.LenientDynamicProperties)
                return;

            var lwSerializeModeAttribute =
                LwBinarySerializationFunctions.LwSerializeModeCache
                    .GetValue(objectType);

            IEnumerable<MemberInfo> membersInfo =
                LwBinarySerializationFunctions.TypeMembersInfoCache.GetValue(
                    new TypeMembersInfoCache.SerializedMembersKey(
                        Direction.Serialization,
                        objectType,
                        lwSerializeModeAttribute.LwSerializationMode, 
                        lwSerializeModeAttribute.DirectMemberType));

            _markTable = new Hashtable(membersInfo.Count());
        }

#if DEBUG
        public
#else
        protected internal
#endif
        void MarkAttribute(string attributeName)
        {
            if (_markTable != null)
            {
                _markTable.Add(attributeName.GetHashCode(), null);
            }
            else
            {
                throw new ArgumentException("Assign mode is not lenient dynamic or lenient dynamic properties");
            }
        }

#if DEBUG
        public
#else
        protected internal
#endif
        bool IsAttributeMarked(string attributeName)
        {
            if (_markTable != null)
            {
                _markTable.ContainsKey(attributeName.GetHashCode());
            }

            return false;
        }
    }
}
