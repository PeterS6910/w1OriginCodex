using System;

using Contal.Cgp.Globals;
using Contal.IwQuick;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.CCU
{
    [LwSerialize(266)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class MultiDoorElementDateIntervalTimeZone
    {
        public Guid IdMultiDoorElement { get; private set; }
        public DateIntervalTimeZone DateIntervalTimeZone { get; private set; }

        public MultiDoorElementDateIntervalTimeZone()
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

    [LwSerialize(263)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    public class DateIntervalTimeZone
    {
        private Guid _guidTimeZone;
        private DateTime _dateFrom;
        private DateTime _dateTo;

        [LwSerialize]
        public Guid GuidTimeZone { get { return _guidTimeZone; } set { _guidTimeZone = value; } }
        [LwSerialize]
        public DateTime DateFrom { get { return _dateFrom; } set { _dateFrom = value; } }
        [LwSerialize]
        public DateTime DateTo { get { return _dateTo; } set { _dateTo = value; } }

        public DateIntervalTimeZone()
        {
        }

        public DateIntervalTimeZone(Guid guidTimeZone, DateTime? dateFrom, DateTime? dateTo)
        {
            _guidTimeZone = guidTimeZone;

            _dateFrom = dateFrom ?? DateTime.MinValue;
            _dateTo = dateTo ?? DateTime.MaxValue;
        }

        public bool IsOn()
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL, 
                () => "bool DateIntervalsTimeZones.IsOn()");

            DateTime now = CcuCore.LocalTime;

            bool result = 
                _dateFrom <= now && _dateTo >= now && 
                (
                    _guidTimeZone == Guid.Empty ||
                    TimeZones.Singleton.GetActualState(_guidTimeZone) == State.On);

            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL, 
                () => "bool DateIntervalsTimeZones.IsOn returns " + result);

            return result;
        }
    }
}