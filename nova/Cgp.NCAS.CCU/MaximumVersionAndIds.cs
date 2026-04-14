using System;
using System.Collections.Generic;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.CCU
{
    [LwSerialize(302)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    public class MaximumVersionAndIds
    {
        [LwSerialize]
        public int MaximumVersion { get; private set; }
        [LwSerialize]
        public ICollection<Guid> Ids { get; private set; }

        public MaximumVersionAndIds()
        {
        }

        public MaximumVersionAndIds(int maximumVersion, ICollection<Guid> ids)
        {
            MaximumVersion = maximumVersion;
            Ids = ids;
        }

        public override string ToString()
        {
            return string.Format(
                "maximum version: {0}, ids count: {1}",
                MaximumVersion,
                Ids.Count);
        }
    }
}
