using System;
using System.Collections.Generic;
using System.Linq;

using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.Server
{
    [Serializable]
    [LwSerialize(262)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    public class PersonAccessRightsForCardReader
    {
        private class DateIntervalTimeZoneEqualityComparer : IEqualityComparer<DateIntervalTimeZone>
        {
            public bool Equals(DateIntervalTimeZone x, DateIntervalTimeZone y)
            {
                return
                    x.DateFrom == y.DateFrom &&
                    x.DateTo == y.DateTo &&
                    x.GuidTimeZone == y.GuidTimeZone;
            }

            public int GetHashCode(DateIntervalTimeZone obj)
            {
                return
                    obj.DateFrom.GetHashCode() ^
                    obj.DateTo.GetHashCode() ^
                    obj.GuidTimeZone.GetHashCode();
            }
        }

        [LwSerialize]
        public string Key { get; private set; }

        [LwSerialize]
        private readonly List<DateIntervalTimeZone> _dateIntervalsTimeZones;

        [LwSerialize]
        private readonly List<MultiDoorElementDateIntervalTimeZone> _multiDoorElementsDateIntervalsTimeZones;

        private readonly HashSet<DateIntervalTimeZone> _dateIntervalsTimeZonesForComparison;

        private readonly SyncDictionary<Guid, HashSet<DateIntervalTimeZone>> _multiDoorElementsDateIntervalsTimeZonesForComparison;

        public PersonAccessRightsForCardReader(string key)
        {
            Key = key;
            _dateIntervalsTimeZones = new List<DateIntervalTimeZone>();
            _multiDoorElementsDateIntervalsTimeZones = new List<MultiDoorElementDateIntervalTimeZone>();

            _dateIntervalsTimeZonesForComparison =
                new HashSet<DateIntervalTimeZone>(new DateIntervalTimeZoneEqualityComparer());

            _multiDoorElementsDateIntervalsTimeZonesForComparison = new SyncDictionary<Guid, HashSet<DateIntervalTimeZone>>();
        }

        public bool Equals(PersonAccessRightsForCardReader other)
        {
            if (_multiDoorElementsDateIntervalsTimeZonesForComparison.Count !=
                other._multiDoorElementsDateIntervalsTimeZonesForComparison.Count)
            {
                return false;
            }

            if (!_multiDoorElementsDateIntervalsTimeZonesForComparison.All(
                multiDoorElementDateIntervalsTimeZonesForComparison =>
                {
                    HashSet<DateIntervalTimeZone> otherDateIntervalsTimeZones;
                    if (!other._multiDoorElementsDateIntervalsTimeZonesForComparison.TryGetValue(
                        multiDoorElementDateIntervalsTimeZonesForComparison.Key,
                        out otherDateIntervalsTimeZones))
                    {
                        return false;
                    }

                    var dateIntervalsTimeZones = multiDoorElementDateIntervalsTimeZonesForComparison.Value;

                    if (dateIntervalsTimeZones.Count != otherDateIntervalsTimeZones.Count)
                        return false;

                    return dateIntervalsTimeZones.All(otherDateIntervalsTimeZones.Contains);
                }))
            {
                return false;
            }

            if (_dateIntervalsTimeZonesForComparison.Count != other._dateIntervalsTimeZonesForComparison.Count)
                return false;

            return _dateIntervalsTimeZonesForComparison.All(other._dateIntervalsTimeZonesForComparison.Contains);
        }

        public void AddDateIntervalTimeZone(DateIntervalTimeZone dateIntervalTimeZone)
        {
            if (!_dateIntervalsTimeZonesForComparison.Add(dateIntervalTimeZone))
                return;

            _dateIntervalsTimeZones.Add(dateIntervalTimeZone);
        }

        public void AddMultiDoorElementDateIntervalTimeZone(
            Guid idMultiDoorElement,
            DateIntervalTimeZone dateIntervalTimeZone)
        {
            _multiDoorElementsDateIntervalsTimeZonesForComparison.GetOrAddValue(
                idMultiDoorElement,
                key =>
                    new HashSet<DateIntervalTimeZone>(new DateIntervalTimeZoneEqualityComparer()),
                (key, value, newlyAdded) =>
                {
                    if (!value.Add(dateIntervalTimeZone))
                        return;

                    _multiDoorElementsDateIntervalsTimeZones.Add(
                        new MultiDoorElementDateIntervalTimeZone(
                            idMultiDoorElement,
                            dateIntervalTimeZone));
                });
        }
    }
}