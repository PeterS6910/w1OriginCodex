using System;
using System.Collections.Generic;


using Contal.IwQuick.Data;
using Contal.IwQuick.Remoting;
using Contal.Cgp.Globals;
using System.Drawing;

namespace Contal.Cgp.Server.Beans
{
    public class StatusChangedTimeZoneHandler : ARemotingCallbackHandler
    {
        private static volatile StatusChangedTimeZoneHandler _singleton;
        private static readonly object _syncRoot = new object();

        private Action<Guid, byte> _statusChanged;

        public static StatusChangedTimeZoneHandler Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (_singleton == null)
                            _singleton = new StatusChangedTimeZoneHandler();
                    }

                return _singleton;
            }
        }

        public StatusChangedTimeZoneHandler()
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
    [LwSerialize(210)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    [LwSerializeNoParent]
    public class TimeZone : AOnOffObject
    {
        public const string COLUMNIDTIMEZONE = "IdTimeZone";
        public const string COLUMNNAME = "Name";
        public const string COLUMNCALENDAR = "Calendar";
        public const string COLUMNGUIDCALENDAR = "GuidCalendar";
        public const string COLUMNDATESETTINGS = "DateSettings";
        public const string COLUMNGUIDDATESETTINGS = "GuidDateSettings";
        public const string COLUMNDESCRIPTION = "Description";
        public const string COLUMNSTATE = "State";
        public const string COLUMNOBJECTTYPE = "ObjectType";
        public const string ColumnVersion = "Version";

        [LwSerialize]
        public virtual Guid IdTimeZone { get; set; }
        public virtual string Name { get; set; }
        public virtual Calendar Calendar { get; set; }
        private Guid _guidCalendar = Guid.Empty;
        [LwSerialize]
        public virtual Guid GuidCalendar { get { return _guidCalendar; } set { _guidCalendar = value; } }
        public virtual ICollection<TimeZoneDateSetting> DateSettings { get; set; }
        private List<Guid> _guidDateSettings = new List<Guid>();
        [LwSerialize]
        public virtual List<Guid> GuidDateSettings { get { return _guidDateSettings; } set { _guidDateSettings = value; } }
        public virtual string Description { get; set; }
        public virtual byte ObjectType { get; set; }

        public TimeZone()
        {
            ObjectType = (byte)Globals.ObjectType.TimeZone;
        }

        public override string ToString()
        {
            return Name;
        }

        public override bool Compare(object obj)
        {
            var timeZone = obj as TimeZone;

            return 
                timeZone != null && 
                timeZone.IdTimeZone == IdTimeZone;
        }

        public virtual bool IsOn(DateTime dateTime)
        {
            if (DateSettings != null)
            {
                ICollection<TimeZoneDateSetting> listExplicitDateSetting = new List<TimeZoneDateSetting>();
                foreach (TimeZoneDateSetting dateSetting in DateSettings)
                {
                    if (dateSetting.IsActual(dateTime) && dateSetting.ExplicitDailyPlan)
                    {
                        listExplicitDateSetting.Add(dateSetting);
                    }
                }

                if (listExplicitDateSetting.Count != 0)
                {
                    foreach (TimeZoneDateSetting dateSetting in listExplicitDateSetting)
                    {
                        if (dateSetting.DailyPlan.IsOn(dateTime))
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    foreach (TimeZoneDateSetting dateSetting in DateSettings)
                    {
                        if (dateSetting.IsActual(dateTime))
                        {
                            if (dateSetting.DailyPlan.IsOn(dateTime))
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        public override bool State
        {
            get { return IsOn(DateTime.Now); }
        }

        public override object GetId()
        {
            return IdTimeZone;
        }

        public override IModifyObject CreateModifyObject()
        {
            return new TimeZoneModifyObj(this);
        }

        public override ObjectType GetObjectType()
        {
            return Globals.ObjectType.TimeZone;
        }

        public virtual void PrepareToSend()
        {
            GuidCalendar = 
                Calendar != null
                    ? Calendar.IdCalendar
                    : Guid.Empty;

            GuidDateSettings.Clear();

            if (DateSettings == null)
                return;

            foreach (TimeZoneDateSetting dateSetting in DateSettings)
                GuidDateSettings.Add(dateSetting.IdTimeZoneDateSetting);
        }

        public override bool Contains(string expression)
        {
            expression = expression.ToLower();

            return 
                Name.ToLower().Contains(expression) || 
                    Description != null && 
                    Description.ToLower().Contains(expression);
        }

        public override string GetIdString()
        {
            return IdTimeZone.ToString();
        }
    }

    [Serializable]
    public class TimeZoneShort : IShortObject
    {
        public const string COLUMNIDTIMEZONE = "IdTimeZone";
        public const string COLUMNNAME = "Name";
        public const string COLUMNDESCRIPTION = "Description";
        public const string COLUMN_SYMBOL = "Symbol";

        public Guid IdTimeZone { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Image Symbol { get; set; }

        public TimeZoneShort(TimeZone timeZone)
        {
            IdTimeZone = timeZone.IdTimeZone;
            Name = timeZone.Name;
            Description = timeZone.Description;
        }

        public override string ToString()
        {
            return Name;
        }

        #region IShortObject Members

        public object Id { get { return IdTimeZone; } }
        string IShortObject.Name { get { return Name; } }

        public ObjectType ObjectType { get { return ObjectType.TimeZone; } }

        public string GetSubTypeImageString(object value)
        {
            return string.Empty;
        }

        #endregion
    }

    [Serializable]
    public class TimeZoneModifyObj : AModifyObject
    {
        public override ObjectType GetOrmObjectType { get { return ObjectType.TimeZone; } }

        public TimeZoneModifyObj(TimeZone timeZone)
        {
            Id = timeZone.IdTimeZone;
            FullName = timeZone.ToString();
            Description = timeZone.Description;
        }
    }
}
