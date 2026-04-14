using System;

#if COMPACT_FRAMEWORK
namespace Contal.LwSerializationCF
#else
namespace Contal.LwSerialization
#endif
{
    internal enum Types : byte
    {
        tNone = 0,
        tBool = 1,
        tByte = 2,
        tChar = 3,
        tInt16 = 4,
        tInt32 = 5,
        tInt64 = 6,
        tFloat = 7,
        tDouble = 8,
        tString = 9,
        tArray = 10,
        tArrayObject = 11,
        tStructEnum = 12,
        tClass = 13,
        tFieldAccessException = 14,
        tDictionary = 15,
        tCollection = 16,
        tKeyValuePair = 17,
        tException = 18,
        tNull = 19,
        tGuid = 20,
        tUInt64 = 21,
        tUInt32 = 22,
        tUInt16 = 23,
        tGuid2 = 24
    }

    /// <summary>
    /// Enum for second byte of serialization type.
    /// </summary>
    internal enum ETypesSecondByte : byte
    {
        tNoArray = 0,
        tArray = 1
    }

    internal enum Direction : byte
    {
        Serialization,
        Deserialization
    }

    public enum LwSerializationMode : byte
    {
        /// <summary>
        /// specifies, that members of the serialized object will
        /// be serialized according to their member type (private,public,properties,...)
        /// </summary>
        Direct = 0x1,

        /// <summary>
        /// specifies, that members of the serialized object will 
        /// be serialized according to marking by the LwSerializeAttribute
        /// </summary>
        Selective = 0x2
    }

    [FlagsAttribute]
    public enum DirectMemberType : byte
    {

        Public = 0x0,
        PrivateAndProtected = 0x1,
        Static = 0x2,
        Properties = 0x4,
        GetProperties = 0x8,
        All = 0xFF
    }

    public enum AssignMode : byte
    {
        Strict = 0x1,
        Lenient = 0x2,
        LenientDynamic = 0x3,
        LenientDynamicProperties = 0x4
    }
}