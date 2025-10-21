using System;
using System.Collections.Generic;

#if COMPACT_FRAMEWORK
namespace Contal.LwSerializationCF
#else
namespace Contal.LwSerialization
#endif
{
    internal class LwSerializeAssignCache
        : TypeAttributeCache<LwSerializeAssignAttribute>
    {
        private static readonly LwSerializeAssignAttribute Default = 
            new LwSerializeAssignAttribute(AssignMode.Strict);

        public LwSerializeAssignCache(
            IEnumerable<Type> preinitializedTypes)
            : base(
                preinitializedTypes, 
                Default)
        {
        }

        protected override LwSerializeAssignAttribute CreateValue(Type type)
        {
            return 
                type.IsEnum 
                    ? Default 
                    : base.CreateValue(type);
        }
    }
}