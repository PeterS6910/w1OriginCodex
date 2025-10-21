using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#if COMPACT_FRAMEWORK
namespace Contal.LwSerializationCF
#else
namespace Contal.LwSerialization
#endif
{
    internal class TypeCache
    {
        private readonly IDictionary<byte, Type> _classesByHashCode =
            new Dictionary<byte, Type>();

        private readonly IDictionary<byte, Type> _valueTypesByHashCode =
            new Dictionary<byte, Type>();

        public TypeCache(
            IEnumerable<Assembly> assemblies, 
            IEnumerable<Type> additionalTypes)
        {
            var types =
                assemblies.SelectMany(assembly => assembly.GetTypes())
                    .Where(type => type.GetCustomAttributes(typeof(LwSerializeAttribute), false).Length > 0)
                    .Concat(additionalTypes);

            foreach (Type type in types)
                if (type.IsClass)
                {
                    byte hashCode =
                        LwBinarySerializationFunctions.GetTypeByteHashCode(
                            type);

                    if (!_classesByHashCode.ContainsKey(hashCode))
                        _classesByHashCode.Add(hashCode, type);
                }
                else
                {
                    if (type.IsPrimitive)
                        continue;

                    byte hashCode =
                        LwBinarySerializationFunctions.GetTypeByteHashCode(
                            type);

                    if (!_valueTypesByHashCode.ContainsKey(hashCode))
                        _valueTypesByHashCode.Add(hashCode, type);
                }
        }

        public Type GetClass(byte hashCode)
        {
            Type result;

            if (!_classesByHashCode.TryGetValue(hashCode, out result))
                throw new NotSupportedException(
                    string.Format(
                        "Hash code {0} of the class not found",
                        hashCode));

            return result;
        }

        public Type GetValueType(byte hashCode)
        {
            Type result;

            if (!_valueTypesByHashCode.TryGetValue(hashCode, out result))
                throw new NotSupportedException(
                    string.Format(
                        "Hash code {0} of the struct/enum not found",
                        hashCode));

            return result;
        }
    }
}
