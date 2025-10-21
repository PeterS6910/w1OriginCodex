using System;

using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Server.Beans.ModifyObjects;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.Server.Beans
{
    [Serializable]
    [LwSerialize(331)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    [LwSerializeNoParent]
    public class AlarmArc : AOrmObjectWithVersion
    {
        public const string COLUMN_ID_ALARM_ARC = "IdAlarmArc";
        public const string COLUMN_OBJECT_TYPE = "ObjectType";
        public const string COLUMN_NAME = "Name";
        public const string COLUMN_DESCRIPTION = "Description";
        public const string ColumnVersion = "Version";

        [LwSerialize]
        public virtual Guid IdAlarmArc { get; set; }
        public virtual byte ObjectType { get; set; }
        [LwSerialize]
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }

        public AlarmArc()
        {
            ObjectType = (byte) Cgp.Globals.ObjectType.AlarmArc;
        }

        public override bool Compare(object obj)
        {
            var alarmArc = obj as AlarmArc;

            if (alarmArc == null)
                return false;

            return alarmArc.IdAlarmArc.Equals(IdAlarmArc);
        }

        public override string GetIdString()
        {
            return IdAlarmArc.ToString();
        }

        public override ObjectType GetObjectType()
        {
            return Cgp.Globals.ObjectType.AlarmArc;
        }

        public override object GetId()
        {
            return IdAlarmArc;
        }

        public override IModifyObject CreateModifyObject()
        {
            return new AlarmArcModifyObj(this);
        }

        public override string ToString()
        {
            return Name;
        }
    }

    [LwSerialize(332)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    public class AlarmTypeAndIdAlarmArc
    {
        [LwSerialize]
        public AlarmType AlarmType{ get; private set; }

        [LwSerialize]
        public Guid IdAlarmArc { get; private set; }

        public AlarmTypeAndIdAlarmArc()
        {
            
        }

        public AlarmTypeAndIdAlarmArc(
            AlarmType alarmType,
            Guid idAlarmArc)
        {
            AlarmType = alarmType;
            IdAlarmArc = idAlarmArc;
        }
    }
}
