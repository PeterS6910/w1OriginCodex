using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

#if COMPACT_FRAMEWORK
namespace Contal.LwSerializationCF
#else
namespace Contal.LwSerialization
#endif
{
    internal class LwBinarySerializationFunctions
    {
        public static readonly TypeMembersInfoCache TypeMembersInfoCache;

        public static readonly LwSerializeModeCache LwSerializeModeCache;

        public static readonly LwSerializeAssignCache LwSerializeAssignCache;

        static LwBinarySerializationFunctions()
        {
            TypeMembersInfoCache = new TypeMembersInfoCache();

            LwSerializeModeCache = new LwSerializeModeCache(SupportedSystemTypes);
            LwSerializeAssignCache = new LwSerializeAssignCache(SupportedSystemTypes);
        }

        public static IEnumerable<Type> SupportedSystemTypes
        {
            get
            {
                yield return typeof(DateTime);
                yield return typeof(TimeSpan);
                yield return typeof(Stopwatch);
            }
        }

        protected static bool GetPropertiesForType(
            Type type,
            ref SerializationProperties serializationProperties)
        {
            SerializationProperties result = null;

            LwSerializeModeAttribute lwSerializeModeAttribute =
                LwSerializeModeCache.GetValue(type);

            if (lwSerializeModeAttribute != null)
                result =
                    new SerializationProperties(serializationProperties)
                    {
                        LwSerializeModeAttribute = lwSerializeModeAttribute
                    };

            LwSerializeAssignAttribute lwSerializeAssignAttribute =
                LwSerializeAssignCache.GetValue(type);

            if (lwSerializeAssignAttribute != null)
            {
                if (result == null)
                    result = new SerializationProperties(serializationProperties);

                result.LwSerializeAssignAttribute = lwSerializeAssignAttribute;
            }

            if (result == null)
                return false;

            serializationProperties = result;
            return true;
        }

        /// <summary>
        /// Function for hashing string
        /// </summary>
        /// <param name="strValue"></param>
        /// <returns></returns>
        private static uint HashString(string strValue)
        {
            if (strValue == null)
                strValue = "";

            return Crc32.ComputeChecksum(strValue);
        }

        /// <summary>
        /// Function for generate name hash code
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        protected static Int16 GetNameShortHashCode(MemberInfo memberInfo)
        {
            if (null == memberInfo)
                return 0;

            uint fullHashcode = HashString(memberInfo.Name);
            byte[] hashcodeBytes = BitConverter.GetBytes(fullHashcode);

            return (Int16)(hashcodeBytes[0] * 256 + hashcodeBytes[1]);
        }

        /// <summary>
        /// Function for generate type hash code
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static byte GetTypeByteHashCode(Type type)
        {
            string strType = "";

            if (type.IsGenericType)
                GetGenericTypeName(type, ref strType);
            else
                strType = type.Name;

            return (byte)(HashString(strType) % 256);
        }

        /// <summary>
        /// Function for get type name for generic types
        /// </summary>
        /// <param name="type"></param>
        /// <param name="strType"></param>
        private static void GetGenericTypeName(Type type, ref string strType)
        {
            strType += type.Name;
            Type[] types = type.GetGenericArguments();

            foreach (Type t in types)
                if (t.IsGenericType)
                    GetGenericTypeName(t, ref strType);
                else
                    strType += t.Name;
        }

        /// <summary>
        /// Function for check that assign mode is lenient or lenient dynamic or lenient dynamic properties
        /// </summary>
        /// <param name="assingMode"></param>
        /// <returns></returns>
        protected static bool IsAssignModeLenient(AssignMode assingMode)
        {
            return assingMode != AssignMode.Strict;
        }
    }
}
