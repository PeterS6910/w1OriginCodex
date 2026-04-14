using System;
using Contal.Cgp.Globals;
using Contal.IwQuick.Data;

namespace Contal.Cgp.Server.Beans
{
    [Serializable()]
    [LwSerialize(201)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    [LwSerializeNoParent]
    public class CalendarDateSetting : AOrmObjectWithVersion
    {
        public const string COLUMNIDCALENDARDATESETTING = "IdCalendarDateSetting";
        public const string COLUMNYEARFROM = "SetYear";
        public const string COLUMNYEAR = "Year";
        public const string COLUMNMONTH = "Month";
        public const string COLUMNWEEK = "Week";
        public const string COLUMNDAY = "Day";
        public const string COLUMNDESCRIPTION = "Description";
        public const string COLUMNCALENDAR = "Calendar";
        public const string COLUMNDAYTYPE = "DayType";
        public const string COLUMNGUIDDAYTYPE = "GuidDayType";
        public const string ColumnVersion = "Version";

        [LwSerializeAttribute()]
        public virtual Guid IdCalendarDateSetting { get; set; }
        [LwSerializeAttribute()]
        public virtual short? SetYear { get; set; }
        [LwSerializeAttribute()]
        public virtual byte Year { get; set; }
        [LwSerializeAttribute()]
        public virtual byte Month { get; set; }
        [LwSerializeAttribute()]
        public virtual byte Week { get; set; }
        [LwSerializeAttribute()]
        public virtual byte Day { get; set; }
        public virtual Calendar Calendar { get; set; }
        public virtual DayType DayType { get; set; }
        private Guid _guidDayType = Guid.Empty;
        [LwSerializeAttribute()]
        public virtual Guid GuidDayType
        {
            get
            {
                return _guidDayType;
            }
            set
            {
                _guidDayType = value;
            }
        }
        public virtual string Description { get; set; }

        public override bool Compare(object obj)
        {
            if (obj == null)
                return false;

            if (obj is CalendarDateSetting)
            {
                return (obj as CalendarDateSetting).IdCalendarDateSetting == IdCalendarDateSetting;
            }
            else
            {
                return false;
            }
        }
        
        public virtual bool IsActual(DateTime dateTime)
        {
            TimeZoneYears year = TimeZoneYears.GetTimeZoneYear(null, Year);
            if (year == null)
            {
                return false;
            }
            else
            {
                if (year.Value != YearSelection.AllYears)
                {
                    if (year.Value == YearSelection.Year)
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
                if (month.Value != MonthType.AllMonths)
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

        public override bool Contains(string expression)
        {
            expression = expression.ToLower();
            if (this.Calendar.CalendarName.ToString().ToLower().Contains(expression)) return true;
            if (this.Description != null)
            {
                if (this.Description.ToLower().Contains(expression)) return true;
            }
            return false;
        }

        public virtual void PrepareToSend()
        {
            if (DayType != null)
            {
                GuidDayType = DayType.IdDayType;
            }
            else
            {
                GuidDayType = Guid.Empty;
            }
        }

        public override string GetIdString()
        {
            return IdCalendarDateSetting.ToString();
        }

        public override object GetId()
        {
            return IdCalendarDateSetting;
        }

        public override IModifyObject CreateModifyObject()
        {
            return null;
        }

        public override Contal.Cgp.Globals.ObjectType GetObjectType()
        {
            return Contal.Cgp.Globals.ObjectType.CalendarDateSetting;
        }
    }
}
