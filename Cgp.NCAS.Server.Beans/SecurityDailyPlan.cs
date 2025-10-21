using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;

using Contal.Cgp.Globals;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.Server.Beans
{
    [Serializable()]
    [LwSerialize(325)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    [LwSerializeNoParent]
    public class SecurityDailyPlan : AOrmObjectWithVersion
    {
        public const string COLUMNIDDAILYPLAN = "IdSecurityDailyPlan";
        public const string COLUMNNAME = "Name";
        public const string COLUMNDESCRIPTION = "Description";
        public const string COLUMNSECURITYDAYINTERVALS = "SecurityDayIntervals";
        public const string COLUMNGUIDSECURITYDAYINTERVALS = "GuidSecurityDayIntervals";
        public const string COLUMNOBJECTTYPE = "ObjectType";
        public const string ColumnVersion = "Version";

        [LwSerializeAttribute()]
        public virtual Guid IdSecurityDailyPlan { get; set; }
        public virtual string Name { get; set; }
        public virtual ICollection<SecurityDayInterval> SecurityDayIntervals { get; set; }
        private byte[] _arraySecurityDayIntevals = null;
        [LwSerializeAttribute()]
        public virtual byte[] ArraySecurityDayIntervals { get { return _arraySecurityDayIntevals; } set { _arraySecurityDayIntevals = value; } }
        public virtual string Description { get; set; }
        public virtual byte ObjectType { get; set; }

        public SecurityDailyPlan()
        {
            ObjectType = (byte)Cgp.Globals.ObjectType.SecurityDailyPlan;
        }

        public override string ToString()
        {
            return Name;
        }

        public override bool Compare(object obj)
        {
            if (obj == null)
                return false;

            if (obj is SecurityDailyPlan)
            {
                return (obj as SecurityDailyPlan).IdSecurityDailyPlan == IdSecurityDailyPlan;
            }
            else
            {
                return false;
            }
        }

        public virtual void PrepareToSend()
        {
            if (_arraySecurityDayIntevals == null)
                _arraySecurityDayIntevals = new byte[1440];

            for (int actualMinute = 0; actualMinute < _arraySecurityDayIntevals.Length; actualMinute++)
            {
                _arraySecurityDayIntevals[actualMinute] = (byte)State.Unknown;
            }

            if (SecurityDayIntervals != null)
            {
                foreach (SecurityDayInterval securityDayInterval in SecurityDayIntervals)
                {
                    if (securityDayInterval != null)
                    {
                        State actualState = State.Unknown;
                        SecurityLevel4SLDP actualSecurityLevel4SLDP = (SecurityLevel4SLDP)securityDayInterval.IntervalType;

                        switch (actualSecurityLevel4SLDP)
                        {
                            case SecurityLevel4SLDP.unlocked:
                                actualState = State.unlocked;
                                break;
                            case SecurityLevel4SLDP.locked:
                                actualState = State.locked;
                                break;
                            case SecurityLevel4SLDP.card:
                                actualState = State.card;
                                break;
                            case SecurityLevel4SLDP.cardpin:
                                actualState = State.cardpin;
                                break;
                            case SecurityLevel4SLDP.code:
                                actualState = State.code;
                                break;
                            case SecurityLevel4SLDP.codeorcard:
                                actualState = State.codecard;
                                break;
                            case SecurityLevel4SLDP.codeorcardpin:
                                actualState = State.codecardpin;
                                break;
                            case SecurityLevel4SLDP.togglecard:
                                actualState = State.togglecard;
                                break;
                            case SecurityLevel4SLDP.togglecardpin:
                                actualState = State.togglecardpin;
                                break;
                        }

                        int minutesFrom = securityDayInterval.MinutesFrom;
                        if (minutesFrom < 0)
                            minutesFrom = 0;

                        int minutesTo = securityDayInterval.MinutesTo + 1;
                        if (minutesTo > _arraySecurityDayIntevals.Length)
                            minutesTo = _arraySecurityDayIntevals.Length;

                        if (minutesTo > minutesFrom)
                        {
                            for (int actualMinute = minutesFrom; actualMinute < minutesTo; actualMinute++)
                            {
                                _arraySecurityDayIntevals[actualMinute] = (byte)actualState;
                            }
                        }
                    }
                }
            }
        }

        public override string GetIdString()
        {
            return IdSecurityDailyPlan.ToString();
        }

        public override object GetId()
        {
            return IdSecurityDailyPlan;
        }

        public override IModifyObject CreateModifyObject()
        {
            return new SecurityDailyPlanModifyObj(this);
        }

        public override Contal.Cgp.Globals.ObjectType GetObjectType()
        {
            return Contal.Cgp.Globals.ObjectType.SecurityDailyPlan;
        }
    }

    [Serializable()]
    public class SecurityDailyPlanShort : IShortObject
    {
        public const string COLUMNIDDAILYPLAN = "IdSecurityDailyPlan";
        public const string COLUMNNAME = "Name";
        public const string COLUMNDESCRIPTION = "Description";
        public const string COLUMN_SYMBOL = "Symbol";

        public Guid IdSecurityDailyPlan { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Image Symbol { get; set; }

        public SecurityDailyPlanShort(SecurityDailyPlan sdp)
        {
            IdSecurityDailyPlan = sdp.IdSecurityDailyPlan;
            Name = sdp.Name;
            Description = sdp.Description;
        }

        public override string ToString()
        {
            return Name;
        }

        #region IShortObject Members

        public ObjectType ObjectType { get { return ObjectType.SecurityDailyPlan; } }

        public string GetSubTypeImageString(object value)
        {
            return string.Empty;
        }

        public object Id { get { return IdSecurityDailyPlan; } }

        #endregion
    }

    [Serializable()]
    public class SecurityDailyPlanModifyObj : AModifyObject
    {
        public override ObjectType GetOrmObjectType { get { return ObjectType.SecurityDailyPlan; } }

        public SecurityDailyPlanModifyObj(SecurityDailyPlan sdp)
        {
            Id = sdp.IdSecurityDailyPlan;
            FullName = sdp.Name;
            Description = sdp.Description;
        }
    } 


    public enum SecurityLevel4SLDP : byte
    {
        [Name("unlocked")]
        unlocked = 0,
        [Name("locked")]
        locked = 1,
        [Name("card")]
        card = 2,
        [Name("cardpin")]
        cardpin = 3,
        [Name("code")]
        code = 4,
        [Name("codeorcard")]
        codeorcard = 5,
        [Name("codeorcardpin")]
        codeorcardpin = 6,
        [Name("togglecard")]
        togglecard = 7,
        [Name("togglecardpin")]
        togglecardpin = 8        
    }

    public class SecurityLevelTypes
    {
        private SecurityLevel4SLDP _value;
        public SecurityLevel4SLDP Value
        {
            get { return _value; }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
        }

        public SecurityLevelTypes(SecurityLevel4SLDP value, string name)
        {
            _value = value;
            _name = name;
        }

        public override string ToString()
        {
            return Name;
        }

        public static IList<SecurityLevelTypes> GetSecurityLevelTypesList(Contal.IwQuick.Localization.LocalizationHelper localizationHelper)
        {
            IList<SecurityLevelTypes> list = new List<SecurityLevelTypes>();
            FieldInfo[] fieldsInfo = typeof(SecurityLevel4SLDP).GetFields();
            foreach (FieldInfo fieldInfo in fieldsInfo)
            {
                NameAttribute[] attribs = fieldInfo.GetCustomAttributes(
                typeof(NameAttribute), false) as NameAttribute[];

                if (attribs.Length > 0)
                {
                    list.Add(new SecurityLevelTypes((SecurityLevel4SLDP)fieldInfo.GetValue(fieldInfo), localizationHelper.GetString("SecurityLevelTypes_" + attribs[0].Name)));
                }
            }

            return list;
        }

        public static SecurityLevelTypes GetSecurityLevelType(Contal.IwQuick.Localization.LocalizationHelper localizationHelper, IList<SecurityLevelTypes> listSecurityLevelTypes, byte securityLevelType)
        {
            if (listSecurityLevelTypes == null)
            {
                return GetSecurityLevelType(localizationHelper, securityLevelType);
            }
            else
            {
                foreach (SecurityLevelTypes listSecurityLevelType in listSecurityLevelTypes)
                {
                    if ((byte)listSecurityLevelType.Value == securityLevelType)
                        return listSecurityLevelType;
                }
            }
            return null;
        }

        public static SecurityLevelTypes GetSecurityLevelType(Contal.IwQuick.Localization.LocalizationHelper localizationHelper, byte securityLevelType)
        {
            FieldInfo[] fieldsInfo = typeof(SecurityLevel4SLDP).GetFields();
            foreach (FieldInfo fieldInfo in fieldsInfo)
            {
                NameAttribute[] attribs = fieldInfo.GetCustomAttributes(
                typeof(NameAttribute), false) as NameAttribute[];

                if (attribs.Length > 0)
                {
                    if ((byte)fieldInfo.GetValue(fieldInfo) == securityLevelType)
                        return (new SecurityLevelTypes((SecurityLevel4SLDP)fieldInfo.GetValue(fieldInfo), localizationHelper.GetString("SecurityLevelTypes_" + attribs[0].Name)));
                }
            }
            return null;
        }
    }

    public class StatusChangedSecurityDailyPlanHandler : Contal.IwQuick.Remoting.ARemotingCallbackHandler
    {
        private static volatile StatusChangedSecurityDailyPlanHandler _singleton = null;
        private static object _syncRoot = new object();


        private Action<Guid, byte> _statusChanged;

        public static StatusChangedSecurityDailyPlanHandler Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (_singleton == null)
                            _singleton = new StatusChangedSecurityDailyPlanHandler();
                    }

                return _singleton;
            }
        }

        public StatusChangedSecurityDailyPlanHandler()
            : base("StatusChangedHandlerSDP")
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
