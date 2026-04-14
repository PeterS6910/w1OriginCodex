#if COMPACT_FRAMEWORK
namespace Contal.LwSerializationCF
#else
namespace Contal.LwSerialization
#endif
{
    /// <summary>
    /// Class for member attributes
    /// </summary>
    internal class SerializationProperties
    {
        private LwSerializeModeAttribute _lwSerializeModeAttribute;
        private LwSerializeAssignAttribute _lwSerializeAssignAttribute;

        public LwSerializeModeAttribute LwSerializeModeAttribute
        {
            set { _lwSerializeModeAttribute = value; }
        }

        public LwSerializeAssignAttribute LwSerializeAssignAttribute
        {
            set { _lwSerializeAssignAttribute = value; }
        }

        public LwSerializationMode Mode
        {
            get
            {
                return
                    _lwSerializeModeAttribute != null
                        ? _lwSerializeModeAttribute.LwSerializationMode
                        : LwSerializationMode.Direct;
            }
        }

        public DirectMemberType DirectMemberType
        {
            get
            {
                return
                    _lwSerializeModeAttribute != null
                        ? _lwSerializeModeAttribute.DirectMemberType
                        : DirectMemberType.Public;
            }
        }

        public AssignMode AssignMode
        {
            get
            {
                return 
                    _lwSerializeAssignAttribute != null
                        ? _lwSerializeAssignAttribute.AssignMode
                        : AssignMode.Strict;
            }
        }

        public SerializationProperties()
        {
        }

        public SerializationProperties(SerializationProperties otherSerializationProperties)
        {
            _lwSerializeModeAttribute = otherSerializationProperties._lwSerializeModeAttribute;
            _lwSerializeAssignAttribute = otherSerializationProperties._lwSerializeAssignAttribute;
        }
    }
}