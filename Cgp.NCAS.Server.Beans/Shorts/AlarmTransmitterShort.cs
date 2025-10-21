using System;
using System.Drawing;

using Contal.Cgp.Globals;
using Contal.Cgp.Server.Beans;

namespace Contal.Cgp.NCAS.Server.Beans.Shorts
{
    [Serializable]
    public class AlarmTransmitterShort : IShortObject
    {
        public const string COLUMN_ID_ALARM_TERMINAL = "IdAlarmTransmitter";
        public const string COLUMN_NAME = "Name";
        public const string COLUMN_IP_ADDRESS = "IpAddress";
        public const string COLUMN_ONLINE_STATE = "OnlineState";
        public const string COLUMN_DESCRIPTION = "Description";
        public const string COLUMN_SYMBOL = "Symbol";

        public Guid IdAlarmTerminal { get; set; }
        public string Name { get; private set; }
        public string IpAddress { get; private set; }
        public OnlineState OnlineState { get; set; }
        public string Description { get; private set; }
        public Image Symbol { get; set; }

        public AlarmTransmitterShort(
            AlarmTransmitter alarmTransmitter,
            OnlineState onlineState)
        {
            IdAlarmTerminal = alarmTransmitter.IdAlarmTransmitter;
            Name = alarmTransmitter.ToString();
            IpAddress = alarmTransmitter.IpAddress;
            OnlineState = onlineState;
            Description = alarmTransmitter.Description;
        }

        public override string ToString()
        {
            return Name;
        }

        #region IShortObject Members

        public ObjectType ObjectType { get { return ObjectType.AlarmTransmitter; } }
        
        public string GetSubTypeImageString(object value)
        {
            return string.Empty;
        }

        public object Id { get { return IdAlarmTerminal; } }

        #endregion
    }
}
