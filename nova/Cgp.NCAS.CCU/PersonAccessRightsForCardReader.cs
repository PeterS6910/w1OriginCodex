using System;
using System.Collections.Generic;
using System.Linq;

using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.CCU
{
    [LwSerialize(262)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    public class PersonAccessRightsForCardReader
    {
        [LwSerialize]
        public string Key { get; private set; }
        [LwSerialize]
        private List<DateIntervalTimeZone> _dateIntervalsTimeZones;
        [LwSerialize]
        private List<MultiDoorElementDateIntervalTimeZone> _multiDoorElementsDateIntervalsTimeZones;

        public PersonAccessRightsForCardReader()
        {
            Key = string.Empty;
            _dateIntervalsTimeZones = new List<DateIntervalTimeZone>();
            _multiDoorElementsDateIntervalsTimeZones = new List<MultiDoorElementDateIntervalTimeZone>();
        }

        public PersonAccessRightsForCardReader(
            string key,
            List<DateIntervalTimeZone> dateIntervalsTimeZones)
        {
            Key = key;
            _dateIntervalsTimeZones = dateIntervalsTimeZones;
            _multiDoorElementsDateIntervalsTimeZones = new List<MultiDoorElementDateIntervalTimeZone>();
        }

        public bool HasAccess()
        {
            if (_dateIntervalsTimeZones == null)
            {
                return false;
            }

            return
                _dateIntervalsTimeZones.Any(
                    dateIntervalTimeZone =>
                        dateIntervalTimeZone.IsOn());
        }

        public bool HasAccessTest(ref string additionalInfo)
        {
            if (_dateIntervalsTimeZones != null
                && _dateIntervalsTimeZones.Any(
                    dateIntervalTimeZone =>
                        dateIntervalTimeZone.IsOn()))
            {
                return true;
            }

            additionalInfo = "No enabled date interval time zone found.";
            return false;
        }

        public ICollection<Guid> HasAccessMultiDoor()
        {
            if (_multiDoorElementsDateIntervalsTimeZones == null)
            {
                return null;
            }

            var mutiDoorElements = new LinkedList<Guid>();

            foreach (var multiDoorElementDateIntervalTimeZone in _multiDoorElementsDateIntervalsTimeZones)
            {
                if (multiDoorElementDateIntervalTimeZone.DateIntervalTimeZone.IsOn())
                    mutiDoorElements.AddLast(multiDoorElementDateIntervalTimeZone.IdMultiDoorElement);
            }

            return mutiDoorElements.Count > 0
                ? mutiDoorElements
                : null;
        }

        public bool HasAccessMultiDoorTest(
            Guid idMultiDoorElement,
            ref string additionalInfo)
        {
            var existsAnyMultiDoorElementDateIntervalTimeZone = false;

            if (_multiDoorElementsDateIntervalsTimeZones != null)
            {
                foreach (var multiDoorElementDateIntervalTimeZone in _multiDoorElementsDateIntervalsTimeZones
                    .Where(
                        multiDoorElementDateIntervalTimeZone =>
                            idMultiDoorElement.Equals(multiDoorElementDateIntervalTimeZone.IdMultiDoorElement)))
                {
                    existsAnyMultiDoorElementDateIntervalTimeZone = true;

                    if (multiDoorElementDateIntervalTimeZone.DateIntervalTimeZone.IsOn())
                    {
                        return true;
                    }
                }
            }

            additionalInfo = existsAnyMultiDoorElementDateIntervalTimeZone
                ? "No enabled date interval time zone found."
                : "DateIntervalsTimeZones was not found for multi door element.";

            return false;
        }
    }
}