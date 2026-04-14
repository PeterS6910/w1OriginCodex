using System;
using Contal.Cgp.Globals;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.Server.Beans
{
    [Serializable]
    [LwSerialize(306)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    [LwSerializeNoParent]
    public class ACLSettingAA : AOrmObjectWithVersion
    {
        public const string COLUMNIDACLSETTING = "IdACLSettingAA";
        public const string COLUMNACCESSCONTROLLIST = "AccessControlList";
        public const string COLUMNALARMAREA = "AlarmArea";
        public const string COLUMNGUIDALARMAREA = "GuidAlarmArea";
        public const string COLUMNALARMAREASET = "AlarmAreaSet";
        public const string COLUMNALARMAREAUNSET = "AlarmAreaUnset";
        public const string COLUMNSENSORHANDLING = "SensorHandling";
        public const string COLUMNEVENTLOGHANDLING = "CREventLogHandling";
        public const string COLUMNTIMEBUYING = "TimeBuying";
        public const string COLUMNALARMAREAUNCONDITIONALSET = "AlarmAreaUnconditionalSet";
        public const string COLUMNALARMAREAALARMACKNOWLEDGE = "AlarmAreaAlarmAcknowledge";
        public const string ColumnVersion = "Version";

        [LwSerialize]
        public virtual Guid IdACLSettingAA { get; set; }
        public virtual AccessControlList AccessControlList { get; set; }
        [LwSerialize]
        public virtual Guid GuidAccessControlList { get; set; }
        public virtual AlarmArea AlarmArea { get; set; }
        private Guid _guidAlarmArea = Guid.Empty;
        [LwSerialize]
        public virtual Guid GuidAlarmArea { get { return _guidAlarmArea; } set { _guidAlarmArea = value; } }
        [LwSerialize]
        public virtual bool AlarmAreaSet { get; set; }
        [LwSerialize]
        public virtual bool AlarmAreaUnset { get; set; }
        [LwSerialize]
        public virtual bool AlarmAreaUnconditionalSet { get; set; }
        [LwSerialize]
        public virtual bool AlarmAreaAlarmAcknowledge { get; set; }
        [LwSerialize]
        public virtual bool SensorHandling { get; set; }
        [LwSerialize]
        public virtual bool CREventLogHandling { get; set; }
        [LwSerialize]
        public virtual bool AlarmAreaTimeBuying { get; set; }

        public override bool Compare(object obj)
        {
            var other = obj as ACLSettingAA;

            return 
                other != null && 
                other.IdACLSettingAA == IdACLSettingAA;
        }

        public virtual void PrepareToSend()
        {
            GuidAlarmArea = 
                AlarmArea != null 
                    ? AlarmArea.IdAlarmArea 
                    : Guid.Empty;

            GuidAccessControlList =
                AccessControlList != null
                ? AccessControlList.IdAccessControlList
                : Guid.Empty;
        }

        public override string GetIdString()
        {
            return IdACLSettingAA.ToString();
        }

        public override object GetId()
        {
            return IdACLSettingAA;
        }

        public override IModifyObject CreateModifyObject()
        {
            return null;
        }

        public override Cgp.Globals.ObjectType GetObjectType()
        {
            return Cgp.Globals.ObjectType.ACLSettingAA;
        }

        public virtual bool Equals(ACLSettingAA other)
        {
            return
                other != null
                && IdACLSettingAA == other.IdACLSettingAA
                && AlarmArea.IdAlarmArea == other.AlarmArea.IdAlarmArea
                && AlarmAreaSet == other.AlarmAreaSet
                && AlarmAreaUnset == other.AlarmAreaUnset
                && AlarmAreaUnconditionalSet == other.AlarmAreaUnconditionalSet
                && AlarmAreaAlarmAcknowledge == other.AlarmAreaAlarmAcknowledge
                && SensorHandling == other.SensorHandling
                && CREventLogHandling == other.CREventLogHandling
                && AlarmAreaTimeBuying == other.AlarmAreaTimeBuying;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ACLSettingAA);
        }
    }
}
