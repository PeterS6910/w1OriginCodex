using System.IO;

using Contal.LwSerialization;
using Contal.LwRemoting2;

namespace Contal.Cgp.NCAS.CCU
{
    public static class Serialization
    {
        private readonly static BufferStream BufferStream = new BufferStream();

        private static readonly MemoryStream MemoryStream = new MemoryStream();

        public static byte[] Serialize<TObject>(TObject obj)
        {
            lock (MemoryStream)
            {
                var lwSerializer = new LwBinarySerializer<TObject>(MemoryStream);

                lwSerializer.Serialize(obj);

                var result = MemoryStream.ToArray();

                MemoryStream.SetLength(0);

                return result;
            }
        }

        public static TObject Deserialize<TObject>(byte[] data)
        {
            lock (BufferStream)
            {
                BufferStream.Write(data, 0, data.Length);

                return new LwBinaryDeserializer<TObject>(BufferStream).Deserialize();
            }
        }
    }
}
