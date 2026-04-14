using System;
using System.IO;

using Contal.IwQuick.Data;
using Contal.LwSerialization;
using Contal.Cgp.Globals;
using Contal.IwQuick.Sys;

namespace Contal.Cgp.NCAS.CCU.DB
{
    [LwSerialize(205)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    public class DailyPlan : IDbObject
    {
        [LwSerialize()]
        public virtual Guid IdDailyPlan { get; set; }
        private byte[] _arrayDayIntevals = null;
        [LwSerialize()]
        public virtual byte[] ArrayDayIntervals { get { return _arrayDayIntevals; } set { _arrayDayIntevals = value; } }

        public virtual bool IsOn(DateTime dateTime)
        {
            try
            {
                int actMinute = dateTime.Hour * 60 + dateTime.Minute;

                if (_arrayDayIntevals != null && actMinute < _arrayDayIntevals.Length)
                {
                    return _arrayDayIntevals[actMinute] == (byte)State.On;
                }

                return false;
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
                return false;
            }
        }

        public Guid GetGuid()
        {
            return IdDailyPlan;
        }

        public ObjectType GetObjectType()
        {
            return ObjectType.DailyPlan;
        }

        public State GetState(ref int nextChangeTime)
        {
            nextChangeTime = -1;

            try
            {
                DateTime dateTime = CcuCore.LocalTime;
                int actMinute = dateTime.Hour * 60 + dateTime.Minute;

                if (_arrayDayIntevals != null && actMinute < _arrayDayIntevals.Length)
                {
                    byte actualState = _arrayDayIntevals[actMinute];

                    int minute = actMinute + 1;
                    while (minute < _arrayDayIntevals.Length && _arrayDayIntevals[minute] == actualState)
                        minute++;

                    nextChangeTime = minute;
                    return (State)actualState;
                }
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }

            return State.Unknown;
        }
    }
}
