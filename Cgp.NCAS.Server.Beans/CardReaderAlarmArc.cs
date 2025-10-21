using System;

using Contal.Cgp.Globals;
using Contal.Cgp.Server.Beans;

namespace Contal.Cgp.NCAS.Server.Beans
{
    [Serializable]
    public class CardReaderAlarmArc :
        AOrmObject,
        IAlarmArcForAlarmType
    {
        public const string COLUMN_ID_CARD_READER_ALARM_ARC = "IdCardReaderAlarmArc";
        public const string COLUMN_CARD_READER = "CardReader";
        public const string COLUMN_ALARM_TYPE = "AlarmType";
        public const string COLUMN_ALARM_ARC = "AlarmArc";
        public const string COLUMN_ID_ALARM_ARC = "IdAlarmArc";

        public virtual Guid IdCardReaderAlarmArc { get; set; }
        public virtual CardReader CardReader { get; set; }
        public virtual byte AlarmType { get; set; }
        public virtual AlarmArc AlarmArc { get; set; }

        public virtual Guid IdAlarmArc
        {
            get
            {
                return AlarmArc != null
                    ? AlarmArc.IdAlarmArc
                    : Guid.Empty;
            }
        }

        public CardReaderAlarmArc()
        {

        }

        public CardReaderAlarmArc(
            CardReader cardReader,
            AlarmArc alarmArc,
            AlarmType alarmType)
        {
            CardReader = cardReader;
            AlarmArc = alarmArc;
            AlarmType = (byte)alarmType;
        }

        public override bool Compare(object obj)
        {
            var cardReaderAlarmArc = obj as CardReaderAlarmArc;

            if (cardReaderAlarmArc == null)
                return false;

            return cardReaderAlarmArc.IdCardReaderAlarmArc.Equals(
                IdCardReaderAlarmArc);
        }

        public override string GetIdString()
        {
            return IdCardReaderAlarmArc.ToString();
        }

        public override ObjectType GetObjectType()
        {
            return ObjectType.CardReaderAlarmArc;
        }

        public override object GetId()
        {
            return IdCardReaderAlarmArc;
        }

        public override IModifyObject CreateModifyObject()
        {
            return null;
        }
    }
}
