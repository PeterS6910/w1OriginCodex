using System;
using System.Collections.Generic;

namespace Contal.Cgp.NCAS.CCU
{
    public interface IAaCardReadersStorage
    {
        ICollection<DB.AACardReader> GetAaCardReadersByIdCardReader(Guid idCardReader);
    }
}
