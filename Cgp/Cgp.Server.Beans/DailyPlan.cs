using System;
using System.Collections.Generic;

using Contal.IwQuick;
using Contal.IwQuick.Data;
using Contal.IwQuick.Remoting;
using Contal.Cgp.Globals;
using System.Drawing;

namespace Contal.Cgp.Server.Beans
{
    public class StatusChangedDailyPlainHandler : ARemotingCallbackHandler
    {
        private static volatile StatusChangedDailyPlainHandler _singleton;
        private static readonly object _syncRoot = new object();

        private Action<Guid, byte> _statusChanged;

        public static StatusChangedDailyPlainHandler Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (_singleton == null)
                            _singleton = new StatusChangedDailyPlainHandler();
                    }

                return _singleton;
            }
        }

        public StatusChangedDailyPlainHandler()
            : base("StatusChangedHandler")
        {
        }

        public void RegisterStatusChanged(Action<Guid, byte> statusChanged)
        {
            _statusChanged += statusChanged;
        }

        public void UnregisterStatusChanged(Action<Guid, byte> statusChanged)
        {
            _statusChanged -= statusChanged;
        }

        public void RunEvent(Guid id, byte Status)
        {
            if (_statusChanged != null)
                _statusChanged(id, Status);
        }
    }

    [Serializable]
    [LwSerialize(205)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    [LwSerializeNoParent]
    public class DailyPlan : AOnOffObject
    {
        public const string COLUMNIDDAILYPLAN = "IdDailyPlan";
        public const string COLUMNDAILYPLANNAME = "DailyPlanName";
        public const string COLUMNDESCRIPTION = "Description";
        public const string COLUMNDAYINTERVALS = "DayIntervals";
        public const string COLUMNGUIDDAYINTERVALS = "GuidDayIntervals";
        public const string COLUMNSTATE = "State";
        public const string COLUMNOBJECTTYPE = "ObjectType";
        public const string ColumnVersion = "Version";

        public const string IMPLICIT_DP_ALWAYS_ON = "implicitDailyPlanAlwaysON";
        public const string IMPLICIT_DP_ALWAYS_OFF = "implicitDailyPlanAlwaysOFF";
        public const string IMPLICIT_DP_DAYLIGHT_ON = "implicitDailyPlanDaylightON";
        public const string IMPLICIT_DP_NIGHT_ON = "implicitDailyPlanNightON";


        [LwSerialize]
        public virtual Guid IdDailyPlan { get; set; }
        public virtual string DailyPlanName { get; set; }
        public virtual ICollection<DayInterval> DayIntervals { get; set; }
        private byte[] _arrayDayIntevals;
        [LwSerialize]
        public virtual byte[] ArrayDayIntervals { get { return _arrayDayIntevals; } set { _arrayDayIntevals = value; } }
        public virtual string Description { get; set; }
        public virtual byte ObjectType { get; set; }

        public DailyPlan()
        {
            ObjectType = (byte)Globals.ObjectType.DailyPlan;
        }

        public override string ToString()
        {
            return DailyPlanName;
        }

        public override bool Compare(object obj)
        {
            var dailyPlan = obj as DailyPlan;

            return 
                dailyPlan != null && 
                dailyPlan.IdDailyPlan == IdDailyPlan;
        }

        public virtual bool IsOn(DateTime dateTime)
        {
            try
            {
                int actMinute = dateTime.Hour * 60 + dateTime.Minute;

                if (DayIntervals == null || DayIntervals.Count == 0) return false;
                foreach (DayInterval interval in DayIntervals)
                {
                    if (interval.MinutesFrom <= actMinute &&
                        interval.MinutesTo >= actMinute)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public override bool State
        {
            get { return IsOn(DateTime.Now); }
        }

        public override object GetId()
        {
            return IdDailyPlan;
        }

        public override IModifyObject CreateModifyObject()
        {
            return new DailyPlanModifyObj(this);
        }

        public override ObjectType GetObjectType()
        {
            return Globals.ObjectType.DailyPlan;
        }

        public virtual void PrepareToSend()
        {
            if (_arrayDayIntevals == null)
                _arrayDayIntevals = new byte[1440];

            for (int actualMinute = 0; actualMinute < _arrayDayIntevals.Length; actualMinute++)
                _arrayDayIntevals[actualMinute] = (byte)Globals.State.Off;

            if (DayIntervals != null)
            {
                foreach (DayInterval dayInterval in DayIntervals)
                {
                    if (dayInterval != null)
                    {
                        int minutesFrom = dayInterval.MinutesFrom;
                        if (minutesFrom < 0)
                            minutesFrom = 0;

                        int minutesTo = dayInterval.MinutesTo + 1;
                        if (minutesTo > _arrayDayIntevals.Length)
                            minutesTo = _arrayDayIntevals.Length;

                        if (minutesTo > minutesFrom)
                        {
                            for (int actualMinute = minutesFrom; actualMinute < minutesTo; actualMinute++)
                            {
                                _arrayDayIntevals[actualMinute] = (byte)Globals.State.On;
                            }
                        }
                    }
                }
            }
        }

        public override bool Contains(string expression)
        {
            expression = expression.ToLower();

            if (DailyPlanName == IMPLICIT_DP_ALWAYS_ON ||
                DailyPlanName == IMPLICIT_DP_ALWAYS_OFF ||
                DailyPlanName == IMPLICIT_DP_DAYLIGHT_ON ||
                DailyPlanName == IMPLICIT_DP_NIGHT_ON)
            {
                return true;
            }

            return 
                DailyPlanName.ToLower().Contains(expression) || 
                    Description != null &&
                    Description.ToLower().Contains(expression);
        }

        public override string GetIdString()
        {
            return IdDailyPlan.ToString();
        }
    }

    [Serializable]
    public class DailyPlanShort : IShortObject
    {
        public const string COLUMNIDDAILYPLAN = "IdDailyPlan";
        public const string COLUMNDAILYPLANNAME = "DailyPlanName";
        public const string COLUMNDESCRIPTION = "Description";
        public const string COLUMN_SYMBOL = "Symbol";

        public Guid IdDailyPlan { get; set; }
        public string DailyPlanName { get; set; }
        public string Description { get; set; }
        public Image Symbol { get; set; }

        public DailyPlanShort(DailyPlan dailyPlan)
        {
            IdDailyPlan = dailyPlan.IdDailyPlan;
            DailyPlanName = dailyPlan.DailyPlanName;
            Description = dailyPlan.Description;
        }

        public override string ToString()
        {
            return DailyPlanName;
        }

        #region IShortObject Members

        public object Id { get { return IdDailyPlan; } }

        public string Name { get { return DailyPlanName; } }

        public ObjectType ObjectType { get { return ObjectType.DailyPlan; } }

        public string GetSubTypeImageString(object value)
        {
            return string.Empty;
        }

        #endregion
    }

    [Serializable]
    public class DailyPlanModifyObj : AModifyObject
    {
        private bool _isDefaultDailyPlan;
        public override ObjectType GetOrmObjectType { get { return ObjectType.DailyPlan; } }

        public DailyPlanModifyObj(DailyPlan dailyPlan)
        {
            Id = dailyPlan.IdDailyPlan;
            FullName = dailyPlan.ToString();
            Description = dailyPlan.Description;

            if (dailyPlan.DailyPlanName == DailyPlan.IMPLICIT_DP_ALWAYS_ON ||
                dailyPlan.DailyPlanName == DailyPlan.IMPLICIT_DP_ALWAYS_OFF ||
                dailyPlan.DailyPlanName == DailyPlan.IMPLICIT_DP_DAYLIGHT_ON ||
                dailyPlan.DailyPlanName == DailyPlan.IMPLICIT_DP_NIGHT_ON)
            {
                _isDefaultDailyPlan = true;
            }
        }

        public override bool Contains(string expression)
        {
            if (String.IsNullOrEmpty(expression)) return true;
            if (_isDefaultDailyPlan) return true;
            if (AOrmObject.RemoveDiacritism(FullName).ToLower().Contains(AOrmObject.RemoveDiacritism(expression).ToLower()))
            {
                return true;
            }

            if (Description != null)
            {
                return AOrmObject.RemoveDiacritism(Description).ToLower().Contains(AOrmObject.RemoveDiacritism(expression).ToLower());
            }

            return false;
        }
    }
}