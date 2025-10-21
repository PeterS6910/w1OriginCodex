using System;

namespace Contal.Cgp.NCAS.CCU
{
    public interface IDoorEnvironmentsStorage
    {
        Guid GetDoorEnvironemntIdForCardReader(Guid idCardReader);
    }
}
