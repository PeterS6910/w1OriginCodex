using System;

#if COMPACT_FRAMEWORK
namespace Contal.LwSerializationCF
#else
namespace Contal.LwSerialization
#endif
{
    /// <summary>
    /// Class for types reading from stream.
    /// </summary>
    internal class StreamTypeInfo
    {
        public Types Type { get; private set; }
        public byte TypeSecondByte { get; private set; }
        public AssignMode AssignMode { get; private set; }
        public short NameHashCode { get; private set; }

        public StreamTypeInfo(
            Types type,
            byte typeSecondByte,
            AssignMode assignMode,
            Int16 nameHashCode)
        {
            Type = type;
            TypeSecondByte = typeSecondByte;
            AssignMode = assignMode;
            NameHashCode = nameHashCode;
        }
    }
}