using System;
using System.Drawing;

using Contal.Cgp.Globals;

namespace Contal.Cgp.NCAS.Server.Beans.Shorts
{
    [Serializable]
    public class AlarmArcShort : IShortObject
    {
        public const string COLUMN_ID_ALARM_ARC = "IdAlarmArc";
        public const string COLUMN_NAME = "Name";
        public const string COLUMN_DESCRIPTION = "Description";
        public const string COLUMN_SYMBOL = "Symbol";

        public Guid IdAlarmArc { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public Image Symbol { get; set; }

        public AlarmArcShort(AlarmArc alarmArc)
        {
            IdAlarmArc = alarmArc.IdAlarmArc;
            Name = alarmArc.ToString();
            Description = alarmArc.Description;
        }

        public override string ToString()
        {
            return Name;
        }

        #region IShortObject Members

        public ObjectType ObjectType { get { return ObjectType.AlarmArc; } }

        public string GetSubTypeImageString(object value)
        {
            return string.Empty;
        }

        public object Id { get { return IdAlarmArc; } }

        #endregion
    }
}
