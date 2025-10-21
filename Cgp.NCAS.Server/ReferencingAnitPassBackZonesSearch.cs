using System;
using System.Collections.Generic;

using Contal.Cgp.NCAS.Server.Beans;

namespace Contal.Cgp.NCAS.Server
{
    public class ReferencingAnitPassBackZonesSearch
    {
        private readonly Guid _searchRoot;

        private readonly IDictionary<Guid, ICollection<Guid>> _directlyReferencingZones = 
            new Dictionary<Guid, ICollection<Guid>>();

        private readonly ICollection<Guid> _zonesReferencingRoot =
            new HashSet<Guid>();

        public ReferencingAnitPassBackZonesSearch(Guid searchRoot)
        {
            _searchRoot = searchRoot;
        }

        public ICollection<Guid> Execute(IEnumerable<AntiPassBackZone> list)
        {
            DetermineReferencingZones(list);

            CollectDescendantsReferencingRoot(_searchRoot);

            return _zonesReferencingRoot;
        }

        private void CollectDescendantsReferencingRoot(Guid currentZone)
        {
            if (_zonesReferencingRoot.Contains(currentZone))
                return;

            _zonesReferencingRoot.Add(currentZone);

            ICollection<Guid> referencingZones;

            if (!_directlyReferencingZones.TryGetValue(
                currentZone, 
                out referencingZones))
            {
                return;
            }

            foreach (var referencingZone in referencingZones)
                CollectDescendantsReferencingRoot(referencingZone);
        }

        private void DetermineReferencingZones(IEnumerable<AntiPassBackZone> list)
        {
            foreach (var antiPassBackZone in list)
            {
                AntiPassBackZone expirationTarget =
                    antiPassBackZone.DestinationAPBZAfterTimeout;

                if (expirationTarget == null)
                    continue;

                var expirationTargetGuid = expirationTarget.IdAntiPassBackZone;

                ICollection<Guid> referencingZones;

                if (!_directlyReferencingZones.TryGetValue(
                    expirationTargetGuid,
                    out referencingZones))
                {
                    referencingZones = new LinkedList<Guid>();

                    _directlyReferencingZones.Add(
                        expirationTargetGuid,
                        referencingZones);
                }

                referencingZones.Add(antiPassBackZone.IdAntiPassBackZone);
            }
        }
    }
}