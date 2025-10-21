using System;

namespace Contal.Cgp.NCAS.CCU
{
    public interface IPersonsStorage
    {
        Guid GetPersonId(String personCodeHash);
    }
}
