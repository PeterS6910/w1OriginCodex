using System;
using System.Collections.Generic;

#if COMPACT_FRAMEWORK
namespace Contal.LwSerializationCF
#else
namespace Contal.LwSerialization
#endif
{
    internal class LwSerializeModeCache : TypeAttributeCache<LwSerializeModeAttribute>
    {
        private static readonly LwSerializeModeAttribute DefaultForPreinitializedTypes = 
            new LwSerializeModeAttribute(
                LwSerializationMode.Direct,
                DirectMemberType.PrivateAndProtected);

        private static readonly LwSerializeModeAttribute DefaultForEnums =
            new LwSerializeModeAttribute(
                LwSerializationMode.Direct,
                DirectMemberType.Public);

        public LwSerializeModeCache(
            IEnumerable<Type> preinitializedTypes)
            : base(
                preinitializedTypes,
                DefaultForPreinitializedTypes)
        {
        }

        protected override LwSerializeModeAttribute CreateValue(Type type)
        {
            return 
                type.IsEnum
                    ? DefaultForEnums 
                    : base.CreateValue(type);
        }
    }
}