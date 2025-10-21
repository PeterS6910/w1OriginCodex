using System;
using System.IO;

using Contal.IwQuick.Data;
using Contal.LwSerialization;
using Contal.Cgp.Globals;

namespace Contal.Cgp.NCAS.CCU.DB
{
    [LwSerialize(328)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    public class SecurityTimeZoneDateSetting : IDbObject
    {
        [LwSerialize()]
        public virtual Guid IdSecurityTimeZoneDateSetting { get; set; }
        [LwSerialize()]
        public virtual short? SetYear { get; set; }
        [LwSerialize()]
        public virtual byte? Year { get; set; }
        [LwSerialize()]
        public virtual byte? Month { get; set; }
        [LwSerialize()]
        public virtual byte? Week { get; set; }
        [LwSerialize()]
        public virtual byte? Day { get; set; }
        private Guid _guidSecurityTimeZone = Guid.Empty;
        [LwSerialize()]
        public virtual Guid GuidSecurityTimeZone { get { return _guidSecurityTimeZone; } set { _guidSecurityTimeZone = value; } }
        private Guid _guidSecurityDailyPlan = Guid.Empty;
        [LwSerialize()]
        public virtual Guid GuidSecurityDailyPlan { get { return _guidSecurityDailyPlan; } set { _guidSecurityDailyPlan = value; } }
        private Guid _guidDayType = Guid.Empty;
        [LwSerialize()]
        public virtual Guid GuidDayType { get { return _guidDayType; } set { _guidDayType = value; } }
        [LwSerialize()]
        public virtual bool ExplicitSecurityDailyPlan { get; set; }

        public virtual bool IsActual(DateTime dateTime, Guid guidCalendar)
        {
            if (GuidDayType != Guid.Empty)
            {
                Calendar calendar = null;

                if (guidCalendar != Guid.Empty)
                {
                    calendar = Database.ConfigObjectsEngine.GetFromDatabase(ObjectType.Calendar, guidCalendar) as Calendar;
                }

                if (calendar != null && calendar.GuidDateSettings != null && calendar.GuidDateSettings.Count > 0)
                {
                    foreach (Guid guidCDS in calendar.GuidDateSettings)
                    {
                        CalendarDateSetting cds = Database.ConfigObjectsEngine.GetFromDatabase(ObjectType.CalendarDateSetting, guidCDS) as CalendarDateSetting;

                        if (cds != null && cds.GuidDayType == GuidDayType)
                        {
                            if (cds.IsActual(dateTime))
                                return true;
                        }
                    }
                }

                return false;
            }


            TimeZoneYears year = TimeZoneYears.GetTimeZoneYear(null, Year);
            if (year == null)
            {
                return false;
            }
            else
            {
                if (year.Value != YearSelection.allYears)
                {
                    if (year.Value == YearSelection.year)
                    {
                        if (SetYear != dateTime.Year)
                            return false;
                    }
                }
            }

            TimeZoneMonths month = TimeZoneMonths.GetTimeZoneMonth(null, Month);
            if (month == null)
            {
                return false;
            }
            else
            {
                if (month.Value != MonthSelection.allMonths)
                {
                    int ordinalNumber = month.GetOrdinalNumber();
                    if (ordinalNumber != dateTime.Month)
                        return false;
                }
            }

            TimeZoneWeeks week = TimeZoneWeeks.GetTimeZoneWeek(null, Week);
            if (week == null)
            {
                return false;
            }
            else
            {
                if (week.Value != WeekSelection.allWeeks)
                {
                    int ordinalNumber = week.GetOrdinalNumber();

                    if (ordinalNumber > 0)
                    {
                        if (ordinalNumber != WeekNumber(dateTime))
                            return false;
                    }
                    else
                    {
                        if (week.Value == WeekSelection.evenWeeks)
                        {
                            if (WeekNumber(dateTime) % 2 != 0)
                                return false;
                        }
                        else if (week.Value == WeekSelection.oddWeeks)
                        {
                            if (WeekNumber(dateTime) % 2 == 0)
                                return false;
                        }
                    }
                }
            }

            TimeZoneDays day = TimeZoneDays.GetTimeZoneDay(null, Day);
            if (day == null)
            {
                return false;
            }
            else
            {
                if (day.Value != DaySelection.allDays)
                {
                    int ordinalNumber = day.GetOrdinalNumber();

                    if (ordinalNumber > 0)
                    {
                        if (ordinalNumber != dateTime.Day)
                            return false;
                    }
                    else
                    {
                        if (day.Value == DaySelection.monday)
                        {
                            if (dateTime.DayOfWeek != DayOfWeek.Monday)
                                return false;
                        }
                        else if (day.Value == DaySelection.tuesday)
                        {
                            if (dateTime.DayOfWeek != DayOfWeek.Tuesday)
                                return false;
                        }
                        else if (day.Value == DaySelection.wednesday)
                        {
                            if (dateTime.DayOfWeek != DayOfWeek.Wednesday)
                                return false;
                        }
                        else if (day.Value == DaySelection.thursday)
                        {
                            if (dateTime.DayOfWeek != DayOfWeek.Thursday)
                                return false;
                        }
                        else if (day.Value == DaySelection.friday)
                        {
                            if (dateTime.DayOfWeek != DayOfWeek.Friday)
                                return false;
                        }
                        else if (day.Value == DaySelection.saturday)
                        {
                            if (dateTime.DayOfWeek != DayOfWeek.Saturday)
                                return false;
                        }
                        else if (day.Value == DaySelection.sunday)
                        {
                            if (dateTime.DayOfWeek != DayOfWeek.Sunday)
                                return false;
                        }
                        else if (day.Value == DaySelection.weekdays)
                        {
                            if (dateTime.DayOfWeek == DayOfWeek.Saturday || dateTime.DayOfWeek == DayOfWeek.Sunday)
                                return false;
                        }
                        else if (day.Value == DaySelection.weekend)
                        {
                            if (!(dateTime.DayOfWeek == DayOfWeek.Saturday || dateTime.DayOfWeek == DayOfWeek.Sunday))
                                return false;
                        }
                    }
                }
            }

            return true;
        }

        private int WeekNumber(DateTime fromDate)
        {
            // Get jan 1st of the year
            DateTime startOfYear = fromDate.AddDays(-fromDate.Day + 1).AddMonths(-fromDate.Month + 1);
            // Get dec 31st of the year
            DateTime endOfYear = startOfYear.AddYears(1).AddDays(-1);
            // ISO 8601 weeks start with Monday
            // The first week of a year includes the first Thursday, i.e. at least 4 days
            // DayOfWeek returns 0 for sunday up to 6 for Saturday
            int[] iso8601Correction = { 6, 7, 8, 9, 10, 4, 5 };
            int nds = fromDate.Subtract(startOfYear).Days +
            iso8601Correction[(int)startOfYear.DayOfWeek];
            int wk = nds / 7;
            switch (wk)
            {
                case 0:
                    // Return weeknumber of dec 31st of the previous year
                    return WeekNumber(startOfYear.AddDays(-1));
                case 53:
                    // If dec 31st falls before thursday it is week 01 of next year
                    if (endOfYear.DayOfWeek < DayOfWeek.Thursday)
                        return 1;
                    else
                        return wk;
                default: return wk;
            }
        }

        public Guid GetGuid()
        {
            return IdSecurityTimeZoneDateSetting;
        }

        public ObjectType GetObjectType()
        {
            return ObjectType.SecurityTimeZoneDateSetting;
        }
    }
}
