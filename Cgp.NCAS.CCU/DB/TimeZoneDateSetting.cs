using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;

using Contal.IwQuick.Data;
using Contal.LwSerialization;
using Contal.Cgp.Globals;

namespace Contal.Cgp.NCAS.CCU.DB
{
    class NameAttribute : Attribute
    {
        public string Name { get; protected set; }

        public NameAttribute(string value)
        {
            this.Name = value;
        }
    }

    public enum YearSelection : byte
    {
        [Name("AllYears")]
        allYears = 0,
        [Name("Year")]
        year = 1
    }

    public class TimeZoneYears
    {
        private YearSelection _value;
        public YearSelection Value
        {
            get { return _value; }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
        }

        public TimeZoneYears(YearSelection value, string name)
        {
            _value = value;
            _name = name;
        }

        public override string ToString()
        {
            return Name;
        }

        public static IList<TimeZoneYears> GetTimeZoneYearsList(Contal.IwQuick.Localization.LocalizationHelper localizationHelper)
        {
            IList<TimeZoneYears> list = new List<TimeZoneYears>();
            FieldInfo[] fieldsInfo = typeof(YearSelection).GetFields();
            foreach (FieldInfo fieldInfo in fieldsInfo)
            {
                NameAttribute[] attribs = fieldInfo.GetCustomAttributes(
                typeof(NameAttribute), false) as NameAttribute[];

                if (attribs.Length > 0)
                {
                    if (localizationHelper != null && (YearSelection)fieldInfo.GetValue(fieldInfo) != YearSelection.year)
                        list.Add(new TimeZoneYears((YearSelection)fieldInfo.GetValue(fieldInfo), localizationHelper.GetString("TimeZoneYears_" + attribs[0].Name)));
                    else
                        list.Add(new TimeZoneYears((YearSelection)fieldInfo.GetValue(fieldInfo), attribs[0].Name));
                }
            }

            return list;
        }

        public static TimeZoneYears GetTimeZoneYear(Contal.IwQuick.Localization.LocalizationHelper localizationHelper, IList<TimeZoneYears> listTimeZoneYears, byte? timeZoneYear)
        {
            if (timeZoneYear == null) return null;

            if (listTimeZoneYears == null)
            {
                return GetTimeZoneYear(localizationHelper, timeZoneYear);
            }
            else
            {
                foreach (TimeZoneYears listTimeZoneYear in listTimeZoneYears)
                {
                    if ((byte)listTimeZoneYear.Value == timeZoneYear)
                        return listTimeZoneYear;
                }
            }

            return null;
        }

        public static TimeZoneYears GetTimeZoneYear(
            Contal.IwQuick.Localization.LocalizationHelper localizationHelper, 
            byte? timeZoneYear)
        {
            if (timeZoneYear == null) return null;

            FieldInfo[] fieldsInfo = typeof(YearSelection).GetFields();
            foreach (FieldInfo fieldInfo in fieldsInfo)
            {
                NameAttribute[] attribs = fieldInfo.GetCustomAttributes(
                typeof(NameAttribute), false) as NameAttribute[];

                if (attribs.Length > 0)
                {
                    if ((byte)fieldInfo.GetValue(fieldInfo) == timeZoneYear)
                    {
                        if (localizationHelper != null && (YearSelection)fieldInfo.GetValue(fieldInfo) != YearSelection.year)
                            return (new TimeZoneYears((YearSelection)fieldInfo.GetValue(fieldInfo), localizationHelper.GetString("TimeZoneYears_" + attribs[0].Name)));
                        else
                            return (new TimeZoneYears((YearSelection)fieldInfo.GetValue(fieldInfo), attribs[0].Name));
                    }
                }
            }

            return null;
        }
    }

    public enum MonthSelection : byte
    {
        [Name("AllMonths")]
        allMonths = 0,
        [Name("January")]
        january = 10,
        [Name("February")]
        february = 11,
        [Name("March")]
        march = 12,
        [Name("April")]
        april = 13,
        [Name("May")]
        may = 14,
        [Name("June")]
        june = 15,
        [Name("July")]
        july = 16,
        [Name("August")]
        august = 17,
        [Name("September")]
        september = 18,
        [Name("October")]
        october = 19,
        [Name("November")]
        november = 20,
        [Name("December")]
        december = 21,
    }

    public class TimeZoneMonths
    {
        private MonthSelection _value;
        public MonthSelection Value
        {
            get { return _value; }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
        }

        public TimeZoneMonths(MonthSelection value, string name)
        {
            _value = value;
            _name = name;
        }

        public override string ToString()
        {
            return Name;
        }

        public static IList<TimeZoneMonths> GetTimeZoneMonthsList(Contal.IwQuick.Localization.LocalizationHelper localizationHelper)
        {
            IList<TimeZoneMonths> list = new List<TimeZoneMonths>();
            FieldInfo[] fieldsInfo = typeof(MonthSelection).GetFields();
            foreach (FieldInfo fieldInfo in fieldsInfo)
            {
                NameAttribute[] attribs = fieldInfo.GetCustomAttributes(
                typeof(NameAttribute), false) as NameAttribute[];

                if (attribs.Length > 0)
                {
                    if (localizationHelper != null)
                        list.Add(new TimeZoneMonths((MonthSelection)fieldInfo.GetValue(fieldInfo), localizationHelper.GetString("TimeZoneMonths_" + attribs[0].Name)));
                    else
                        list.Add(new TimeZoneMonths((MonthSelection)fieldInfo.GetValue(fieldInfo), attribs[0].Name));
                }
            }

            return list;
        }

        public static TimeZoneMonths GetTimeZoneMonth(Contal.IwQuick.Localization.LocalizationHelper localizationHelper, IList<TimeZoneMonths> listTimeZoneMonths, byte? timeZoneMonth)
        {
            if (timeZoneMonth == null) return null;

            if (listTimeZoneMonths == null)
            {
                return GetTimeZoneMonth(localizationHelper, timeZoneMonth);
            }
            else
            {
                foreach (TimeZoneMonths listTimeZoneMonth in listTimeZoneMonths)
                {
                    if ((byte)listTimeZoneMonth.Value == timeZoneMonth)
                        return listTimeZoneMonth;
                }
            }

            return null;
        }

        public static TimeZoneMonths GetTimeZoneMonth(Contal.IwQuick.Localization.LocalizationHelper localizationHelper, byte? timeZoneMonth)
        {
            if (timeZoneMonth == null) return null;

            FieldInfo[] fieldsInfo = typeof(MonthSelection).GetFields();
            foreach (FieldInfo fieldInfo in fieldsInfo)
            {
                NameAttribute[] attribs = fieldInfo.GetCustomAttributes(
                typeof(NameAttribute), false) as NameAttribute[];

                if (attribs.Length > 0)
                {
                    if ((byte)fieldInfo.GetValue(fieldInfo) == timeZoneMonth)
                    {
                        if (localizationHelper != null)
                            return (new TimeZoneMonths((MonthSelection)fieldInfo.GetValue(fieldInfo), localizationHelper.GetString("TimeZoneMonths_" + attribs[0].Name)));
                        else
                            return (new TimeZoneMonths((MonthSelection)fieldInfo.GetValue(fieldInfo), attribs[0].Name));
                    }
                }
            }

            return null;
        }

        public int GetOrdinalNumber()
        {
            if ((byte)Value < 10)
                return 0;
            else
                return (byte)Value - 9;
        }
    }

    public enum WeekSelection : byte
    {
        [Name("AllWeeks")]
        allWeeks = 0,
        [Name("EvenWeeks")]
        evenWeeks = 1,
        [Name("OddWeeks")]
        oddWeeks = 2,
        [Name("1.")]
        week1 = 10,
        [Name("2.")]
        week2 = 11,
        [Name("3.")]
        week3 = 12,
        [Name("4.")]
        week4 = 13,
        [Name("5.")]
        week5 = 14,
        [Name("6.")]
        week6 = 15,
        [Name("7.")]
        week7 = 16,
        [Name("8.")]
        week8 = 17,
        [Name("9.")]
        week9 = 18,
        [Name("10.")]
        week10 = 19,
        [Name("11.")]
        week11 = 20,
        [Name("12.")]
        week12 = 21,
        [Name("13.")]
        week13 = 22,
        [Name("14.")]
        week14 = 23,
        [Name("15.")]
        week15 = 24,
        [Name("16.")]
        week16 = 25,
        [Name("17.")]
        week17 = 26,
        [Name("18.")]
        week18 = 27,
        [Name("19.")]
        week19 = 28,
        [Name("20.")]
        week20 = 29,
        [Name("21.")]
        week21 = 30,
        [Name("22.")]
        week22 = 31,
        [Name("23.")]
        week23 = 32,
        [Name("24.")]
        week24 = 33,
        [Name("25.")]
        week25 = 34,
        [Name("26.")]
        week26 = 35,
        [Name("27.")]
        week27 = 36,
        [Name("28.")]
        week28 = 37,
        [Name("29.")]
        week29 = 38,
        [Name("30.")]
        week30 = 39,
        [Name("31.")]
        week31 = 40,
        [Name("32.")]
        week32 = 41,
        [Name("33.")]
        week33 = 42,
        [Name("34.")]
        week34 = 43,
        [Name("35.")]
        week35 = 44,
        [Name("36.")]
        week36 = 45,
        [Name("37.")]
        week37 = 46,
        [Name("38.")]
        week38 = 47,
        [Name("39.")]
        week39 = 48,
        [Name("40.")]
        week40 = 49,
        [Name("41.")]
        week41 = 50,
        [Name("42.")]
        week42 = 51,
        [Name("43.")]
        week43 = 52,
        [Name("44.")]
        week44 = 53,
        [Name("45.")]
        week45 = 54,
        [Name("46.")]
        week46 = 55,
        [Name("47.")]
        week47 = 56,
        [Name("48.")]
        week48 = 57,
        [Name("49.")]
        week49 = 58,
        [Name("50.")]
        week50 = 59,
        [Name("51.")]
        week51 = 60,
        [Name("52.")]
        week52 = 61,
        [Name("53.")]
        week53 = 62
    }

    public class TimeZoneWeeks
    {
        private WeekSelection _value;
        public WeekSelection Value
        {
            get { return _value; }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
        }

        public TimeZoneWeeks(WeekSelection value, string name)
        {
            _value = value;
            _name = name;
        }

        public override string ToString()
        {
            return Name;
        }

        public static IList<TimeZoneWeeks> GetTimeZoneWeeksList(Contal.IwQuick.Localization.LocalizationHelper localizationHelper)
        {
            IList<TimeZoneWeeks> list = new List<TimeZoneWeeks>();
            FieldInfo[] fieldsInfo = typeof(WeekSelection).GetFields();
            foreach (FieldInfo fieldInfo in fieldsInfo)
            {
                NameAttribute[] attribs = fieldInfo.GetCustomAttributes(
                typeof(NameAttribute), false) as NameAttribute[];

                if (attribs.Length > 0)
                {
                    string name = string.Empty;
                    
                    if ((byte)fieldInfo.GetValue(fieldInfo) < 10 && localizationHelper != null)
                        name = localizationHelper.GetString("TimeZoneWeeks_" + attribs[0].Name);
                    else
                        name = attribs[0].Name;

                    list.Add(new TimeZoneWeeks((WeekSelection)fieldInfo.GetValue(fieldInfo), name));
                }
            }

            return list;
        }

        public static TimeZoneWeeks GetTimeZoneWeek(Contal.IwQuick.Localization.LocalizationHelper localizationHelper, IList<TimeZoneWeeks> listTimeZoneWeeks, byte? timeZoneWeek)
        {
            if (timeZoneWeek == null) return null;

            if (listTimeZoneWeeks == null)
            {
                return GetTimeZoneWeek(localizationHelper, timeZoneWeek);
            }
            else
            {
                foreach (TimeZoneWeeks listTimeZoneWeek in listTimeZoneWeeks)
                {
                    if ((byte)listTimeZoneWeek.Value == timeZoneWeek)
                        return listTimeZoneWeek;
                }
            }

            return null;
        }

        public static TimeZoneWeeks GetTimeZoneWeek(Contal.IwQuick.Localization.LocalizationHelper localizationHelper, byte? timeZoneWeek)
        {
            if (timeZoneWeek == null) return null;

            FieldInfo[] fieldsInfo = typeof(WeekSelection).GetFields();
            foreach (FieldInfo fieldInfo in fieldsInfo)
            {
                NameAttribute[] attribs = fieldInfo.GetCustomAttributes(
                typeof(NameAttribute), false) as NameAttribute[];

                if (attribs.Length > 0)
                {
                    if ((byte)fieldInfo.GetValue(fieldInfo) == timeZoneWeek)
                    {
                        string name = string.Empty;

                        if ((byte)fieldInfo.GetValue(fieldInfo) < 10 && localizationHelper != null)
                            name = localizationHelper.GetString("TimeZoneWeeks_" + attribs[0].Name);
                        else
                            name = attribs[0].Name;

                        return (new TimeZoneWeeks((WeekSelection)fieldInfo.GetValue(fieldInfo), name));
                    }
                }
            }

            return null;
        }

        public int GetOrdinalNumber()
        {
            if ((byte)Value < 10)
                return 0;
            else
                return (byte)Value - 9;
        }
    }

    public enum DaySelection : byte
    {
        [Name("AllDays")]
        allDays = 0,
        [Name("Monday")]
        monday = 1,
        [Name("Tuesday")]
        tuesday = 2,
        [Name("Wednesday")]
        wednesday = 3,
        [Name("Thursday")]
        thursday = 4,
        [Name("Friday")]
        friday = 5,
        [Name("Saturday")]
        saturday = 6,
        [Name("Sunday")]
        sunday = 7,
        [Name("Weekdays")]
        weekdays = 8,
        [Name("Weekend")]
        weekend = 9,
        [Name("1")]
        day1 = 10,
        [Name("2")]
        day2 = 11,
        [Name("3")]
        day3 = 12,
        [Name("4")]
        day4 = 13,
        [Name("5")]
        day5 = 14,
        [Name("6")]
        day6 = 15,
        [Name("7")]
        day7 = 16,
        [Name("8")]
        day8 = 17,
        [Name("9")]
        day9 = 18,
        [Name("10")]
        day10 = 19,
        [Name("11")]
        day11 = 20,
        [Name("12")]
        day12 = 21,
        [Name("13")]
        day13 = 22,
        [Name("14")]
        day14 = 23,
        [Name("15")]
        day15 = 24,
        [Name("16")]
        day16 = 25,
        [Name("17")]
        day17 = 26,
        [Name("18")]
        day18 = 27,
        [Name("19")]
        day19 = 28,
        [Name("20")]
        day20 = 29,
        [Name("21")]
        day21 = 30,
        [Name("22")]
        day22 = 31,
        [Name("23")]
        day23 = 32,
        [Name("24")]
        day24 = 33,
        [Name("25")]
        day25 = 34,
        [Name("26")]
        day26 = 35,
        [Name("27")]
        day27 = 36,
        [Name("28")]
        day28 = 37,
        [Name("29")]
        day29 = 38,
        [Name("30")]
        day30 = 39,
        [Name("31")]
        day31 = 40
    }

    public class TimeZoneDays
    {
        private DaySelection _value;
        public DaySelection Value
        {
            get { return _value; }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
        }

        public TimeZoneDays(DaySelection value, string name)
        {
            _value = value;
            _name = name;
        }

        public override string ToString()
        {
            return Name;
        }

        public static IList<TimeZoneDays> GetTimeZoneDaysList(Contal.IwQuick.Localization.LocalizationHelper localizationHelper)
        {
            IList<TimeZoneDays> list = new List<TimeZoneDays>();
            FieldInfo[] fieldsInfo = typeof(DaySelection).GetFields();
            foreach (FieldInfo fieldInfo in fieldsInfo)
            {
                NameAttribute[] attribs = fieldInfo.GetCustomAttributes(
                typeof(NameAttribute), false) as NameAttribute[];

                if (attribs.Length > 0)
                {
                    string name = string.Empty;

                    if ((byte)fieldInfo.GetValue(fieldInfo) < 10 && localizationHelper != null)
                        name = localizationHelper.GetString("TimeZoneDays_" + attribs[0].Name);
                    else
                        name = attribs[0].Name;

                    list.Add(new TimeZoneDays((DaySelection)fieldInfo.GetValue(fieldInfo), name));
                }
            }

            return list;
        }

        public static TimeZoneDays GetTimeZoneDay(Contal.IwQuick.Localization.LocalizationHelper localizationHelper, IList<TimeZoneDays> listTimeZoneDays, byte? timeZoneDay)
        {
            if (timeZoneDay == null) return null;

            if (listTimeZoneDays == null)
            {
                return GetTimeZoneDay(localizationHelper, timeZoneDay);
            }
            else
            {
                foreach (TimeZoneDays listTimeZoneDay in listTimeZoneDays)
                {
                    if ((byte)listTimeZoneDay.Value == timeZoneDay)
                        return listTimeZoneDay;
                }
            }

            return null;
        }

        public static TimeZoneDays GetTimeZoneDay(Contal.IwQuick.Localization.LocalizationHelper localizationHelper, byte? timeZoneDay)
        {
            if (timeZoneDay == null) return null;

            FieldInfo[] fieldsInfo = typeof(DaySelection).GetFields();
            foreach (FieldInfo fieldInfo in fieldsInfo)
            {
                NameAttribute[] attribs = fieldInfo.GetCustomAttributes(
                typeof(NameAttribute), false) as NameAttribute[];

                if (attribs.Length > 0)
                {
                    if ((byte)fieldInfo.GetValue(fieldInfo) == timeZoneDay)
                    {
                        string name = string.Empty;

                        if ((byte)fieldInfo.GetValue(fieldInfo) < 10 && localizationHelper != null)
                            name = localizationHelper.GetString("TimeZoneDays_" + attribs[0].Name);
                        else
                            name = attribs[0].Name;

                        return (new TimeZoneDays((DaySelection)fieldInfo.GetValue(fieldInfo), name));
                    }
                }
            }

            return null;
        }

        public int GetOrdinalNumber()
        {
            if ((byte)Value < 10)
                return 0;
            else
                return (byte)Value - 9;
        }
    }

    [LwSerialize(211)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    public class TimeZoneDateSetting : IDbObject
    {
        [LwSerialize()]
        public virtual Guid IdTimeZoneDateSetting { get; set; }
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
        private Guid _guidTimeZone = Guid.Empty;
        [LwSerialize()]
        public virtual Guid GuidTimeZone { get { return _guidTimeZone; } set { _guidTimeZone = value; } }
        private Guid _guidDailyPlan = Guid.Empty;
        [LwSerialize()]
        public virtual Guid GuidDailyPlan { get { return _guidDailyPlan; } set { _guidDailyPlan = value; } }
        private Guid _guidDayType = Guid.Empty;
        [LwSerialize()]
        public virtual Guid GuidDayType { get { return _guidDayType; } set { _guidDayType = value; } }
        [LwSerialize()]
        public virtual bool ExplicitDailyPlan { get; set; }


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
                    return endOfYear.DayOfWeek < DayOfWeek.Thursday 
                        ? 1 
                        : wk;

                default: 
                    return wk;
            }
        }

        public Guid GetGuid()
        {
            return IdTimeZoneDateSetting;
        }

        public ObjectType GetObjectType()
        {
            return ObjectType.TimeZoneDateSetting;
        }
    }
}
