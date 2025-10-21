using System;

#if COMPACT_FRAMEWORK
namespace Contal.LwSerializationCF
#else
namespace Contal.LwSerialization
#endif
{
    public class LwSerializeAttribute : Attribute
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

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class LwSerializeAssignAttribute : Attribute
    {
        public AssignMode AssignMode { get; private set; }

        public LwSerializeAssignAttribute(AssignMode assignMode)
        {
            AssignMode = assignMode;
        }
    }

    public class LwSerializeVersionAttribute : Attribute
    {
        private readonly Version _version;

        public Version Version
        {
            get { return _version; }
        }

        public LwSerializeVersionAttribute(string strVersion)
        {
            _version = new Version(strVersion);
        }
    }
}