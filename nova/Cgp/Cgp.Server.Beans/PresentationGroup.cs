using System;
using System.Collections.Generic;

using Contal.IwQuick;
using Contal.Cgp.Globals;
using System.Drawing;

namespace Contal.Cgp.Server.Beans
{
    [Serializable]
    public class PresentationGroup : AOrmObject
    {
        public enum ExpirationType { notset, lifetime, datetime, cyclecount, unicitygroup };
        public const string COLUMNIDGROUP = "IdGroup";
        public const string COLUMNGROUPNAME = "GroupName";
        public const string COLUMNEMAIL = "Email";
        public const string COLUMNSMS = "Sms";
        public const string COLUMNCISCLASS = "cisClass";
        public const string COLUMNCISPRIORITY = "cisPriority";
        public const string COLUMNCISEXPIRATIONTYPE = "cisExpirationType";
        public const string COLUMNCISLIFETIME = "cisLifetime";
        public const string COLUMNCISDATETIME = "cisDatetime";
        public const string COLUMNCISCYCLECOUNT = "cisCyclecount";
        public const string COLUMNCISUNICITYGROUP = "cisUnicitygroup";
        public const string COLUMNCISUNICITYGROUPTIMEOUT = "cisUnicitygroupTimeout";
        public const string COLUMNCISSHOWTIME = "cisShowtime";
        public const string COLUMNCISIDLE = "cisIdle";
        public const string COLUMNCISFULLSCREEN = "cisFullscreen";
        public const string COLUMNCISIMMEDIATE = "cisImmediate";
        public const string COLUMNCISNOSCROLLING = "cisNoscrolling";
        public const string COLUMNPRESENTATIONFORMATER = "PresentationFormatter";
        public const string COLUMNCISNG = "CisNG";
        public const string COLUMNSYSTEMEVENTS = "SystemEvents";
        public const string COLUMNDESCRIPTION = "Description";
        public const string COLUMNCISNGGROUP = "CisNGGroup";
        public const string COLUMNRESPONSEALARM = "ResponseAlarm";
        public const string COLUMNRESPONSEALARMNOTACK = "ResponseAlarmNotAck";
        public const string COLUMNRESPONSENORMALNOTACK = "ResponseNormalNotAck";
        public const string COLUMNRESPONSENORMAL = "ResponseNormal";
        public const string COLUMNRESPONSENORMALOFFACK = "ResponseNormalOffAck";
        public const string COLUMN_INHERITED_EMAIL_SUBJECT = "InheritedEmailSubject";
        public const string COLUMN_EMAIL_SUBJECT = "EmailSubject";
        public const string COLUMNOBJECTTYPE = "ObjectType";

        public virtual Guid IdGroup { get; set; }
        public virtual string GroupName { get; set; }
        public virtual string Email { get; set; }
        public virtual string Sms { get; set; }
        //CisNG values
        public virtual string cisClass { get; set; }
        public virtual int? cisPriority { get; set; }
        public virtual int cisExpirationType { get; set; }
        public virtual TimeSpan? cisLifetime { get; set; }
        public virtual DateTime? cisDatetime { get; set; }
        public virtual int cisCyclecount { get; set; }
        public virtual string cisUnicitygroup { get; set; }
        public virtual int? cisUnicitygroupTimeout { get; set; }
        public virtual int cisShowtime { get; set; }
        public virtual bool cisIdle { get; set; }
        public virtual bool cisFullscreen { get; set; }
        public virtual bool cisImmediate { get; set; }
        public virtual bool cisNoscrolling { get; set; }

        public virtual PresentationFormatter PresentationFormatter { get; set; }
        public virtual ICollection<SystemEvent> SystemEvents { get; set; }
        public virtual ICollection<CisNG> CisNG { get; set; }
        public virtual ICollection<CisNGGroup> CisNGGroup { get; set; }

        public virtual bool ResponseAlarm { get; set; }
        public virtual bool ResponseAlarmNotAck { get; set; }
        public virtual bool ResponseNormalNotAck { get; set; }
        public virtual bool ResponseNormal { get; set; }
        public virtual bool ResponseNormalOffAck { get; set; }
        public virtual bool ResponseDateTimeUpdate { get; set; }
        public virtual bool InheritedEmailSubject { get; set; }
        public virtual string EmailSubject { get; set; }
        public virtual string Description { get; set; }
        public virtual byte ObjectType { get; set; }

        //cat sms values
        public virtual Guid AlarmTransmitterId { get; set; }
        public virtual string PhoneNumbers { get; set; }
        public virtual string MessagesPrefix { get; set; }

        public PresentationGroup()
        {
            ObjectType = (byte)Globals.ObjectType.PresentationGroup;
        }

        public PresentationGroup(string name, string email, string sms)
        {
            Validator.CheckNullString(name);

            GroupName = name;
            Email = email;
            Sms = sms;
            ObjectType = (byte)Globals.ObjectType.PresentationGroup;
        }

        public override string ToString()
        {
            return GroupName;
        }

        public override bool Compare(object obj)
        {
            var presentationGroup = obj as PresentationGroup;

            return 
                presentationGroup != null && 
                presentationGroup.IdGroup == IdGroup;
        }

        public override bool Contains(string expression)
        {
            expression = expression.ToLower();

            return 
                GroupName.ToLower().Contains(expression) || 
                    Description != null &&
                    Description.ToLower().Contains(expression);
        }

        public override string GetIdString()
        {
            return IdGroup.ToString();
        }

        public override object GetId()
        {
            return IdGroup;
        }

        public override IModifyObject CreateModifyObject()
        {
            return new PresentationGroupModifyObj(this);
        }

        public override ObjectType GetObjectType()
        {
            return Globals.ObjectType.PresentationGroup;
        }
    }

    [Serializable]
    public class PresentationGroupShort : IShortObject
    {
        public const string COLUMNIDGROUP = "IdGroup";
        public const string COLUMNGROUPNAME = "GroupName";
        public const string COLUMNEMAIL = "Email";
        public const string COLUMNCISCLASS = "cisClass";
        public const string COLUMNDESCRIPTION = "Description";
        public const string COLUMN_SYMBOL = "Symbol";

        public Guid IdGroup { get; set; }
        public string GroupName { get; set; }
        public string Email { get; set; }
        public string cisClass { get; set; }
        public string Description { get; set; }
        public Image Symbol { get; set; }

        public PresentationGroupShort(PresentationGroup presentationGroup)
        {
            IdGroup = presentationGroup.IdGroup;
            GroupName = presentationGroup.GroupName;
            Email = presentationGroup.Email;
            cisClass = presentationGroup.cisClass;
            Description = presentationGroup.Description;
        }

        public override string ToString()
        {
            return GroupName;
        }

        #region IShortObject Members

        public object Id { get { return IdGroup; } }
        public string Name { get { return GroupName; } }

        public ObjectType ObjectType { get { return ObjectType.PresentationGroup; } }

        public string GetSubTypeImageString(object value)
        {
            return string.Empty;
        }

        #endregion
    }

    [Serializable]
    public class PresentationGroupModifyObj : AModifyObject
    {
        public override ObjectType GetOrmObjectType { get { return ObjectType.PresentationGroup; } }

        public PresentationGroupModifyObj(PresentationGroup presentationGroup)
        {
            Id = presentationGroup.IdGroup;
            FullName = presentationGroup.GroupName;
            Description = presentationGroup.Description;
        }
    }
}
