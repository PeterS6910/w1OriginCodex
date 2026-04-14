using Contal.Cgp.Server.Beans.Extern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contal.Cgp.RemotingCommon
{
    public interface IConsecutiveEvent
    {
        ConsecutiveEvent GetConsecutiveTriggeredEvent(Eventlog eventlog);

        IEnumerable<ConsecutiveEvent> FilterEventsByCard(DateTime dtEventlogFrom, Guid reasonId);

        bool InMinutes(DateTime dt1,  DateTime dt2, int minutes);

        void CleanConsecutiveEvents(Guid idResonSourceIds);
    }

    public interface  IExcelExportEvent
    {
        void AddExportedEvent(int id, Guid sourceId, Guid reasonId, DateTime dt);
        List<ConsecutiveEvent> GetExportedEvent();
    }
}
