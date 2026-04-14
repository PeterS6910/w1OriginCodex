using System;

using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.Server
{
    [LwSerialize(266)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class MultiDoorElementDateIntervalTimeZone
    {
        public Guid IdMultiDoorElement { get; private set; }
        public DateIntervalTimeZone DateIntervalTimeZone { get; private set; }

        protected MultiDoorElementDateIntervalTimeZone()
        {
        }

        public MultiDoorElementDateIntervalTimeZone(
            Guid idMultiDoorElement,
            DateIntervalTimeZone dateIntervalTimeZone)
        {
            IdMultiDoorElement = idMultiDoorElement;
            DateIntervalTimeZone = dateIntervalTimeZone;
        }
    }

    [Serializable]
    [LwSerialize(263)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    public class DateIntervalTimeZone
    {
        private readonly Guid _guidTimeZone;
        private readonly DateTime _dateFrom;
        private readonly DateTime _dateTo;

        [LwSerialize]
        public Guid GuidTimeZone { get { return _guidTimeZone; } }
        [LwSerialize]
        public DateTime DateFrom { get { return _dateFrom; } }
        [LwSerialize]
        public DateTime DateTo { get { return _dateTo; } }

        protected DateIntervalTimeZone()
        {
        }

        public DateIntervalTimeZone(Cgp.Server.Beans.TimeZone timeZone, DateTime? dateFrom, DateTime? dateTo)
        {
            _guidTimeZone = 
                timeZone != null 
                    ? timeZone.IdTimeZone
                    : Guid.Empty;

            _dateFrom = dateFrom ?? DateTime.MinValue;
            _dateTo = dateTo ?? DateTime.MaxValue;
        }
    }
}