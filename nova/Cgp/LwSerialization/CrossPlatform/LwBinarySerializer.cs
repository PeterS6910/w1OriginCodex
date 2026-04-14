using System;
using System.IO;
using System.Reflection;

#if COMPACT_FRAMEWORK
using Contal.IwQuickCF;
#else
using Contal.IwQuick;
#endif

#if COMPACT_FRAMEWORK
namespace Contal.LwSerializationCF
#else
namespace Contal.LwSerialization
#endif
{
    public sealed class LwBinarySerializer<T> : ASingleton<LwBinarySerializer<T>>
    {
        private class LwBinaryDeserializePool : AObjectPool<LwBinaryDeserialize<T>>
        {
            protected override LwBinaryDeserialize<T> CreateObject()
            {
                return new LwBinaryDeserialize<T>();
            }
        }

        private readonly LwBinaryDeserializePool _lwBinaryDeserializePool =
            new LwBinaryDeserializePool();

        private readonly LwBinarySerialize<T> _lwBinarySerialize =
            new LwBinarySerialize<T>();

        private static readonly Assembly[] TypeAssembly = { typeof (T).Assembly };

        private LwBinarySerializer() : base(null)
        {
        }

        /// <summary>
        /// Function for serialization.
        /// </summary>
        /// <param name="outputStream"></param>
        /// <param name="instanceToSerialize"></param>
        public void Serialize(Stream outputStream, T instanceToSerialize)
        {
            _lwBinarySerialize.Serialize(
                outputStream, 
                instanceToSerialize);
        }

        /// <summary>
        /// Function for deserialization
        /// </summary>
        /// <param name="inputStream"></param>
        /// <returns></returns>
        public T Deserialize(Stream inputStream)
        {
            return Deserialize(inputStream, null);
        }

        /// <summary>
        /// Function for deserialization
        /// </summary>
        /// <param name="inputStream"></param>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        public T Deserialize(
            Stream inputStream, 
            Assembly[] assemblies)
        {
            long inputStreamPosition = 0;

            LwBinaryDeserialize<T> deserialize;
            T result;

            try
            {
                inputStreamPosition = inputStream.Position;

                deserialize = _lwBinaryDeserializePool.Get();

                result =
                    deserialize.Deserialize(
                        inputStream,
                        TypesByAssemblies
                            .Instance
                            .GetValue(assemblies ?? TypeAssembly));

            }
            catch
            {
                inputStream.Seek(inputStreamPosition, SeekOrigin.Begin);
                throw;
            }

            _lwBinaryDeserializePool.Return(deserialize);
            return result;
        }

        /// <summary>
        /// Function for deserialization with compare versions
        /// </summary>
        /// <param name="inputStream"></param>
        /// <param name="isSameVersion"></param>
        /// <param name="fileVersion"></param>
        /// <returns></returns>
        public T Deserialize(
            Stream inputStream, 
            out bool isSameVersion, 
            out Version fileVersion)
        {
            return Deserialize(
                inputStream, 
                null, 
                out isSameVersion, 
                out fileVersion);
        }

        /// <summary>
        /// Function for deserialization with compare versions
        /// </summary>
        /// <param name="inputStream"></param>
        /// <param name="assemblies"></param>
        /// <param name="isSameVersion"></param>
        /// <param name="fileVersion"></param>
        /// <returns></returns>
        public T Deserialize(
            Stream inputStream, 
            Assembly[] assemblies, 
            out bool isSameVersion, 
            out Version fileVersion)
        {
            LwBinaryDeserialize<T> deserialize =
                _lwBinaryDeserializePool.Get();

            T newInstance = 
                deserialize.Deserialize(
                    inputStream, 
                    TypesByAssemblies.Instance.GetValue(assemblies ?? TypeAssembly));

            isSameVersion = deserialize.IsSameVersion;
            fileVersion = deserialize.FileVersion;

            _lwBinaryDeserializePool.Return(deserialize);

            return newInstance;
        }
    }
}