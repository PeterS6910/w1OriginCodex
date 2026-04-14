using System;
using System.Collections.Generic;
using Contal.Cgp.Globals;

namespace Contal.Cgp.NCAS.CCU
{
    public interface IAccessZonesStorage
    {
        IEnumerable<Guid> GetTimeZoneIdsForCrObject(Guid idPerson, ObjectType objectType, Guid idCardReaderObject);
        IEnumerable<Guid> GetTimeZoneIdsForAlarmAreas(Guid idPerson, Guid idCardReader);
        IEnumerable<Guid> GetTimeZoneIdsForMultiDoorElement(Guid idPerson, Guid idMultiDoorElement);
        IEnumerable<Guid> GetTimeZoneIdsForFloor(Guid idPerson, Guid idMultiDoorElement);
        bool ExistsAccessZonesForPerson(Guid idPerson);
    }
}