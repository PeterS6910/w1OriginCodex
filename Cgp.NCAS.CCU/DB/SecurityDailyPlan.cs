using System;
using System.IO;

using Contal.IwQuick.Data;
using Contal.LwSerialization;
using Contal.Cgp.Globals;
using Contal.IwQuick.Sys;

namespace Contal.Cgp.NCAS.CCU.DB
{
    [LwSerialize(325)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    public class SecurityDailyPlan : IDbObject
    {
        [LwSerialize()]
        public virtual Guid IdSecurityDailyPlan { get; set; }
        private byte[] _arraySecurityDayIntevals = null;
        [LwSerialize()]
        public virtual byte[] ArraySecurityDayIntervals { get { return _arraySecurityDayIntevals; } set { _arraySecurityDayIntevals = value; } }

        public static State GetState(Guid guidSecurityDailyPlan, DateTime dateTime)
        {
            SecurityDailyPlan securityDailyPlan = Database.ConfigObjectsEngine.GetFromDatabase(ObjectType.SecurityDailyPlan, guidSecurityDailyPlan) as SecurityDailyPlan;
            if (securityDailyPlan != null)
            {
                return securityDailyPlan.GetState(dateTime);
            }

            return State.Unknown;
        }

        public static State GetState(Guid guidSecurityDailyPlan, DateTime dateTime, ref int nextChangeTime)
        {
            SecurityDailyPlan securityDailyPlan = Database.ConfigObjectsEngine.GetFromDatabase(ObjectType.SecurityDailyPlan, guidSecurityDailyPlan) as SecurityDailyPlan;
            if (securityDailyPlan != null)
            {
                return securityDailyPlan.GetState(dateTime, ref nextChangeTime);
            }

            nextChangeTime = -1;
            return State.Unknown;
        }

        public State GetState(DateTime dateTime)
        {
            try
            {
                int actMinute = dateTime.Hour * 60 + dateTime.Minute;

                if (_arraySecurityDayIntevals != null && actMinute < _arraySecurityDayIntevals.Length)
                {
                    return (State)_arraySecurityDayIntevals[actMinute];
                }
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }

            return State.Unknown;
        }

        public State GetState(DateTime dateTime, ref int nextChangeTime)
        {
            nextChangeTime = -1;

            try
            {
                int actMinute = dateTime.Hour * 60 + dateTime.Minute;

                if (_arraySecurityDayIntevals != null && actMinute < _arraySecurityDayIntevals.Length)
                {
                    byte actualState = _arraySecurityDayIntevals[actMinute];

                    int minute = actMinute + 1;
                    while (minute < _arraySecurityDayIntevals.Length && _arraySecurityDayIntevals[minute] == actualState)
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

        public Guid GetGuid()
        {
            return IdSecurityDailyPlan;
        }

        public ObjectType GetObjectType()
        {
            return ObjectType.SecurityDailyPlan;
        }
    }
}
