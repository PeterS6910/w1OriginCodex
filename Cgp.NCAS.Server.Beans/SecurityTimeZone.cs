using System;
using System.Collections.Generic;
using System.Drawing;

using Contal.Cgp.Globals;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick;
using Contal.IwQuick.Data;
using Contal.IwQuick.Remoting;

namespace Contal.Cgp.NCAS.Server.Beans
{
    [Serializable]
    [LwSerialize(327)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    [LwSerializeNoParent]
    public class SecurityTimeZone : AOrmObjectWithVersion
    {
        public const string COLUMNIDSECURITYTIMEZONE = "IdSecurityTimeZone";
        public const string COLUMNNAME = "Name";
        public const string COLUMNCALENDAR = "Calendar";
        public const string COLUMNGUIDCALENDAR = "GuidCalendar";
        public const string COLUMNDATESETTINGS = "DateSettings";
        public const string COLUMNGUIDDATESETTINGS = "GuidDateSettings";
        public const string COLUMNDESCRIPTION = "Description";
        public const string COLUMNOBJECTTYPE = "ObjectType";
        public const string ColumnVersion = "Version";

        [LwSerialize]
        public virtual Guid IdSecurityTimeZone { get; set; }
        public virtual string Name { get; set; }
        public virtual Calendar Calendar { get; set; }
        private Guid _guidCalendar = Guid.Empty;
        [LwSerialize]
        public virtual Guid GuidCalendar { get { return _guidCalendar; } set { _guidCalendar = value; } }
        public virtual ICollection<SecurityTimeZoneDateSetting> DateSettings { get; set; }
        private List<Guid> _guidDateSettings = new List<Guid>();
        [LwSerialize]
        public virtual List<Guid> GuidDateSettings { get { return _guidDateSettings; } set { _guidDateSettings = value; } }
        public virtual string Description { get; set; }
        public virtual byte ObjectType { get; set; }

        public SecurityTimeZone()
        {
            ObjectType = (byte)Cgp.Globals.ObjectType.SecurityTimeZone;
        }

        public override string ToString()
        {
            return Name;
        }

        public override bool Compare(object obj)
        {
            if (obj == null)
                return false;

            if (obj is SecurityTimeZone)
            {
                return (obj as SecurityTimeZone).IdSecurityTimeZone == IdSecurityTimeZone;
            }
            else
            {
                return false;
            }
        }

        public virtual bool IsActual(DateTime dateTime)
        {
            if (DateSettings != null)
            {
                foreach (SecurityTimeZoneDateSetting dateSetting in DateSettings)
                {
                    if (dateSetting.IsActual(dateTime))
                        return true;
                }
            }

            return false;
        }

        public virtual void PrepareToSend()
        {
            if (Calendar != null)
            {
                GuidCalendar = Calendar.IdCalendar;
            }
            else
            {
                GuidCalendar = Guid.Empty;
            }

            GuidDateSettings.Clear();

            if (DateSettings != null)
            {
                foreach (SecurityTimeZoneDateSetting dateSetting in DateSettings)
                {
                    GuidDateSettings.Add(dateSetting.IdSecurityTimeZoneDateSetting);
                }
            }
        }

        public override string GetIdString()
        {
            return IdSecurityTimeZone.ToString();
        }

        public override object GetId()
        {
            return IdSecurityTimeZone;
        }

        public override IModifyObject CreateModifyObject()
        {
            return new SecurityTimeZoneModifyObj(this);
        }

        public override ObjectType GetObjectType()
        {
            return Cgp.Globals.ObjectType.SecurityTimeZone;
        }
    }

    [Serializable]
    public class SecurityTimeZoneShort : IShortObject
    {
        public const string COLUMN_ID_SECURITY_TIME_ZONE = "IdSecurityTimeZone";
        public const string COLUMN_NAME = "Name";
        public const string COLUMN_DESCRIPTION = "Description";
        public const string COLUMN_SYMBOL = "Symbol";

        public Guid IdSecurityTimeZone { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Image Symbol { get; set; }

        public SecurityTimeZoneShort(SecurityTimeZone stz)
        {
            IdSecurityTimeZone = stz.IdSecurityTimeZone;
            Name = stz.Name;
            Description = stz.Description;
        }

        public override string ToString()
        {
            return Name;
        }

        #region IShortObject Members

        public ObjectType ObjectType { get { return ObjectType.SecurityTimeZone; } }

        public string GetSubTypeImageString(object value)
        {
            return string.Empty;
        }

        public object Id { get { return IdSecurityTimeZone; } }

        #endregion
    }

    [Serializable]
    public class SecurityTimeZoneModifyObj : AModifyObject
    {
        public override ObjectType GetOrmObjectType { get { return ObjectType.SecurityTimeZone; } }

        public SecurityTimeZoneModifyObj(SecurityTimeZone stz)
        {
            Id = stz.IdSecurityTimeZone;
            FullName = stz.Name;
            Description = stz.Description;
        }
    }

    public class StatusChangedSecurityTimeZoneHandler : ARemotingCallbackHandler
    {
        private static volatile StatusChangedSecurityTimeZoneHandler _singleton;
        private static readonly object _syncRoot = new object();

        private Action<Guid, byte> _statusChanged;

        public static StatusChangedSecurityTimeZoneHandler Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (_singleton == null)
                            _singleton = new StatusChangedSecurityTimeZoneHandler();
                    }

                return _singleton;
            }
        }

        public StatusChangedSecurityTimeZoneHandler()
            : base("StatusChangedHandlerSTZ")
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
}
