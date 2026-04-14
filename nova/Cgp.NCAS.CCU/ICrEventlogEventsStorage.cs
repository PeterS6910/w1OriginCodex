using System;
using System.Collections.Generic;

namespace Contal.Cgp.NCAS.CCU
{
    public interface ICrEventlogEventsStorage
    {
        void SaveToDatabase(EventParameters.EventParameters eventParameters);
        void DeleteFromDatabase(UInt64 eventId);
        IEnumerable<EventParameters.EventParameters> GetAllEventParameters();
    }
}
