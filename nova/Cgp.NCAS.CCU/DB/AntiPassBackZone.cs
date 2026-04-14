using System;
using System.Collections.Generic;

using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Globals;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.CCU.DB
{
    [LwSerialize(308)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    public class AntiPassBackZone : IDbObject
    {
        [LwSerialize]
        public Guid IdAntiPassBackZone { get; set; }

        [LwSerialize]
        public bool ProhibitAccessForCardNotPresent { get; set; }

        [LwSerialize]
        public Guid GuidDestinationAPBZAfterTimeout { get; set; }

        [LwSerialize]
        public int Timeout { get; set; }

        [LwSerialize]
        public Dictionary<Guid, ApbzCardReaderEntryExitBy> GuidEntryCardReaders { get; set; }

        [LwSerialize]
        public Dictionary<Guid, ApbzCardReaderEntryExitBy> GuidExitCardReaders { get; set; }

        public Guid GetGuid()
        {
            return IdAntiPassBackZone;
        }

        public ObjectType GetObjectType()
        {
            return ObjectType.AntiPassBackZone;
        }
    }
}
