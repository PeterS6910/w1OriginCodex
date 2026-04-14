using System;

using Contal.Cgp.Globals;

namespace Contal.Cgp.NCAS.CCU.DB
{
    public interface IDbObject
    {
        Guid GetGuid();
        ObjectType GetObjectType();
    }
}
