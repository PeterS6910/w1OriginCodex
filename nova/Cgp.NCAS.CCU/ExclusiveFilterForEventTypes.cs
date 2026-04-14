using System.Collections.Generic;

using Contal.Cgp.Globals;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.CCU
{
    public class ExclusiveFilterForEventTypes
    {
        private readonly ICollection<EventType> _excludedEventTypes;

        public ExclusiveFilterForEventTypes(params EventType[] excludedEventTypes)
        {
            _excludedEventTypes = excludedEventTypes != null
                ? new HashSet<EventType>(excludedEventTypes)
                : new HashSet<EventType>();
        }

        public bool IsEventTypeExcluded(EventType eventType)
        {
            return _excludedEventTypes.Contains(eventType);
        }
    }
}
