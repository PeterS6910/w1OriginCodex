using System;

#if COMPACT_FRAMEWORK
namespace Contal.LwSerializationCF
#else
namespace Contal.LwSerialization
#endif
{
    internal class LwBinaryDeserializeSkipper : IDeserializationBuilder
    {
        private readonly IDeserializationBuilder _parent;
        private byte _currentType;

        public LwBinaryDeserializeSkipper(
            IDeserializationBuilder parent,
            StreamTypeInfo streamTypeInfo)
        {
            _parent = parent;
            _currentType = (byte) streamTypeInfo.Type;
        }

        private byte[] _buffer;

        public byte[] GetBuffer(int size)
        {
            _buffer = _parent.GetBuffer(size);
            return _buffer;
        }

        public IDeserializationBuilder SetupStreamTypeInfo(
            StreamTypeInfo streamTypeInfo)
        {
            _currentType = (byte) streamTypeInfo.Type;
            return this;
        }

        public byte[] GetLengthAndBuffer(out int length)
        {
            length = BitConverter.ToInt32(_buffer, 0);
            return GetBuffer(length);
        }

        public int GetLength()
        {
            return BitConverter.ToInt32(_buffer, 0);
        }

        public byte[] GetFixedLengthBuffer(out int length)
        {
            length = LwBinaryDeserialize.FixedBufferLengthByType[_currentType];
            return GetBuffer(length);
        }

        public void Commit()
        {
        }

        public bool VersionRequired { get { return false; } }
        public Version Version { set {} }
    }
}