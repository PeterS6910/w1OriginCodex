using System;
using System.Collections.Generic;
using Contal.Cgp.Globals;

namespace Contal.Cgp.NCAS.CCU
{
    public interface IAclSettingsStorage
    {
        IEnumerable<Guid> GetTimeZoneIdsForCardReaderObject(
            ObjectType objectType,
            Guid idCardReaderObject,
            Guid idAccessControlList);
        IEnumerable<Guid> GetTimeZoneIdsForAlarmAreas(Guid idCardReader, Guid idAccessControlList);
        IEnumerable<Guid> GetTimeZoneIdsForMultiDoorElement(Guid idMultiDoorElement, Guid idAccessControlList);
        IEnumerable<Guid> GetTimeZoneIdsForFloor(Guid idMultiDooElement, Guid idAccessControlList);
    }
}
