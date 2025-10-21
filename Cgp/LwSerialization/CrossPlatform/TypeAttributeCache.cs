using System;
using System.Collections.Generic;
using System.Linq;

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
    internal class TypeAttributeCache<TAttribute> : ACache<Type, TAttribute>
        where TAttribute : Attribute
    {
        public TypeAttributeCache(
            IEnumerable<Type> preinitializedTypes,
            TAttribute attributeValue) : 
            base(
                preinitializedTypes
                    .Select(type => 
                        new KeyValuePair<Type, TAttribute>(
                            type, 
                            attributeValue)))
        {
        }

        protected override TAttribute CreateValue(Type type)
        {
            object[] attributes =
                type.GetCustomAttributes(
                    typeof(TAttribute),
                    false);

            return 
                attributes.Length > 0
                    ? (TAttribute)attributes[0]
                    : null;
        }
    }
}
