using System;

#if COMPACT_FRAMEWORK
namespace Contal.LwSerializationCF
#else
namespace Contal.LwSerialization
#endif
{
    internal interface IDeserializationBuilder
    {
        IDeserializationBuilder SetupStreamTypeInfo(
            StreamTypeInfo streamTypeInfo);

        byte[] GetFixedLengthBuffer(out int length);
        byte[] GetBuffer(int size);
        byte[] GetLengthAndBuffer(out int length);

        int GetLength();

        void Commit();
        bool VersionRequired { get; }
        Version Version { set; }
    }
}