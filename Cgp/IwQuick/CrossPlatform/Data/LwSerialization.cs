using System;

namespace Contal.IwQuick.Data
{
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
        Selective = 0x2,

        /// <summary>
        /// specifies, that members of the serialized object will 
        /// be serialized according to marking by the LwSerializeAttribute
        /// and object will contain all members of ICollection
        /// </summary>
        SelectiveWithCollectionOptimization = 0x4,

        /// <summary>
        /// specifies, that members of the serialized object will not 
        /// be serialized according to marking by the LwSerializeAttribute
        /// </summary>
        InvertedSelective = 0x8,

        /// <summary>
        /// specifies, that members of the serialized object will
        /// be serialized according to interface inherited from ILwSerialize
        /// </summary>
        SelectiveByInterface = 0x10
    }

    [FlagsAttribute]
    public enum DirectMemberType : byte
    {
        Public = 0x0,
        PrivateAndProtected = 0x1,
        Static = 0x2,
        GetProperties = 0x4,
        All = 0xFF
    }

    /// <summary>
    /// Attribute used for LwSerialization marking. Class attribute have 
    /// to use constructor with code definition.
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class LwSerializeAttribute : Attribute
    {
        public ushort Code { get; private set;}

        public LwSerializeAttribute()
        {
            Code = 0;
        }

        public LwSerializeAttribute(ushort code)
        {
            Code = code;
        }

        public const int MaxCodeValue = 0x3FF;
    }

    /// <summary>
    /// Attribute used for LwSerialization marking.
    /// Members with this attribute will not be serialized
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class LwNotSerializedAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class LwSerializeModeAttribute : Attribute
    {
        public LwSerializationMode LwSerializationMode { get; private set; }

        public DirectMemberType DirectMemberType { get; private set; }

        public LwSerializeModeAttribute(LwSerializationMode mode, DirectMemberType memberType)
        {
            LwSerializationMode = mode;
            DirectMemberType = memberType;
        }

        public LwSerializeModeAttribute(LwSerializationMode mode)
        {
            LwSerializationMode = mode;
            DirectMemberType = DirectMemberType.Public;
        }
    }

    /// <summary>
    /// This attribute will remove parent object from serialized data
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class LwSerializeNoParentAttribute : Attribute
    {
    }

    /// <summary>
    /// Child interfaces of this class will be used to select properties to serialize
    /// </summary>
    public interface ILwSerialize
    {
    }
}