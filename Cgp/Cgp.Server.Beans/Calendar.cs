using System;
using System.Collections.Generic;

using Contal.IwQuick.Data;
using Contal.Cgp.Globals;
using System.Drawing;

namespace Contal.Cgp.Server.Beans
{
    [Serializable]
    [LwSerialize(200)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    [LwSerializeNoParent]
    public class Calendar : AOrmObjectWithVersion
    {
        public const string COLUMNIDCALENDAR = "IdCalendar";
        public const string COLUMNCALENDARNAME = "CalendarName";
        public const string COLUMNDATESETTINGS = "DateSettings";
        public const string COLUMNGUIDDATESETTINGS = "GuidDateSettings";
        public const string COLUMNTIMEZONEDATESETTING = "TimeZoneDateSetting";
        public const string COLUMNDESCRIPTION = "Description";
        public const string COLUMNTIMEZONE = "TimeZone";
        public const string COLUMNOBJECTTYPE = "ObjectType";
        public const string ColumnVersion = "Version";

        public const string IMPLICIT_CALENDAR_DEFAULT = "implicitCalendarDefaultCalendar";

        [LwSerializeAttribute]
        public virtual Guid IdCalendar { get; set; }
        public virtual string CalendarName { get; set; }
        public virtual ICollection<CalendarDateSetting> DateSettings { get; set; }
        private List<Guid> _guidDateSettings = new List<Guid>();
        [LwSerializeAttribute]
        public virtual List<Guid> GuidDateSettings { get { return _guidDateSettings; } set { _guidDateSettings = value; } }
        public virtual ICollection<TimeZone> TimeZone { get; set; }
        public virtual string Description { get; set; }
        public virtual byte ObjectType { get; set; }

        public Calendar()
        {
            ObjectType = (byte)Cgp.Globals.ObjectType.Calendar;
        }

        public override string ToString()
        {
            return CalendarName;
        }

        public override bool Compare(object obj)
        {
            if (obj == null)
                return false;

            if (obj is Calendar)
            {
                return (obj as Calendar).IdCalendar == IdCalendar;
            }
            else
            {
                return false;
            }
        }

        public virtual void PrepareToSend()
        {
            GuidDateSettings.Clear();

            if (DateSettings != null)
            {
                foreach (CalendarDateSetting dateSetting in DateSettings)
                {
                    GuidDateSettings.Add(dateSetting.IdCalendarDateSetting);
                }
            }
        }

        public override bool Contains(string expression)
        {
            expression = expression.ToLower();
            if (CalendarName != IMPLICIT_CALENDAR_DEFAULT)
            {
                if (this.CalendarName.ToString().ToLower().Contains(expression)) return true;
            }
            else
            {
                return true;
            }
            if (this.Description != null)
            {
                if (this.Description.ToLower().Contains(expression)) return true;
            }
            return false;
        }

        public override string GetIdString()
        {
            return IdCalendar.ToString();
        }

        public override object GetId()
        {
            return IdCalendar;
        }

        public override IModifyObject CreateModifyObject()
        {
            return new CalendarModifyObj(this);
        }

        public override Contal.Cgp.Globals.ObjectType GetObjectType()
        {
            return Contal.Cgp.Globals.ObjectType.Calendar;
        }
    }

    [Serializable]
    public class CalendarShort : IShortObject
    {
        public const string COLUMNIDCALENDAR = "IdCalendar";
        public const string COLUMNCALENDARNAME = "CalendarName";
        public const string COLUMNDESCRIPTION = "Description";
        public const string COLUMN_SYMBOL = "Symbol";

        public Guid IdCalendar { get; set; }
        public string CalendarName { get; set; }
        public string Description { get; set; }
        public Image Symbol { get; set; }

        public CalendarShort(Calendar calendar)
        {
            IdCalendar = calendar.IdCalendar;
            CalendarName = calendar.CalendarName;
            Description = calendar.Description;
        }

        public override string ToString()
        {
            return CalendarName;
        }

        #region IShortObject Members

        public object Id { get { return IdCalendar; } }

        public ObjectType ObjectType { get { return ObjectType.Calendar; } }

        public string Name { get { return CalendarName; } }

        public string GetSubTypeImageString(object value)
        {
            return string.Empty;
        }

        #endregion
    }

    [Serializable]
    public class CalendarModifyObj : AModifyObject
    {
        public override ObjectType GetOrmObjectType { get { return ObjectType.Calendar; } }
        private bool _isDefaultCalendar = false;

        public CalendarModifyObj(Calendar calendar)
        {
            Id = calendar.IdCalendar;
            FullName = calendar.ToString();
            Description = calendar.Description;
            if (calendar.CalendarName == Calendar.IMPLICIT_CALENDAR_DEFAULT)
                _isDefaultCalendar = true;
        }

        public override bool Contains(string expression)
        {
            if (String.IsNullOrEmpty(expression)) return true;
            if (_isDefaultCalendar) return true;
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

