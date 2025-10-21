using System;

using Contal.Cgp.Globals;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.CCU.DB
{
    [LwSerialize(217)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class Person : IDbObject
    {
        public Guid IdPerson { get; set; }
        public string PersonalCodeHash { get; set; }

        public Guid GetGuid()
        {
            return IdPerson;
        }

        public ObjectType GetObjectType()
        {
            return ObjectType.Person;
        }
    }
}
