using System;
using System.Collections.Generic;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.Server
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
    }
}
