using System;
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
    internal class ConstructorCache : ACache<Type, ConstructorInfo>
    {
        static readonly Type[] EmptyTypes = new Type[0];

        protected override ConstructorInfo CreateValue(Type key)
        {
            return key.GetConstructor(EmptyTypes);
        }
    }
}
