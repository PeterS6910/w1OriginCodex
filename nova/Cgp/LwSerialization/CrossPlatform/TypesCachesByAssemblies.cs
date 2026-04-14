using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
    internal class TypesByAssemblies : ACache<Assembly[], TypeCache>
    {
        public static readonly TypesByAssemblies Instance;

        static TypesByAssemblies()
        {
            Instance = new TypesByAssemblies();
        }

        private class AssemblyCacheEqualityComparer
            : IEqualityComparer<Assembly[]>
        {
            public bool Equals(Assembly[] x, Assembly[] y)
            {
                return x.SequenceEqual(y);
            }

            public int GetHashCode(Assembly[] obj)
            {
                return obj.Aggregate(
                    0,
                    (seed, assembly) => seed ^ assembly.GetHashCode());
            }
        }

        private TypesByAssemblies()
            : base(new AssemblyCacheEqualityComparer())
        {

        }

        protected override TypeCache CreateValue(Assembly[] assemblies)
        {
            return 
                new TypeCache(
                    assemblies,
                    LwBinarySerializationFunctions.SupportedSystemTypes);
        }
    }
}
