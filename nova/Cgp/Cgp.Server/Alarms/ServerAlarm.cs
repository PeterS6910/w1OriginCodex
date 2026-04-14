using System;
using Contal.Cgp.Globals.PlatformPC;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick.Data;

namespace Contal.Cgp.Server.Alarms
{
    [LwSerialize(800)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class ServerAlarm :
        IEquatable<ServerAlarm>,
        IGetPresentationGroup
    {
        public ServerAlarmCore ServerAlarmCore { get; private set; }

        protected ServerAlarm()
        {
            
        }

        public ServerAlarm(ServerAlarmCore serverAlarmCore)
        {
            ServerAlarmCore = serverAlarmCore;
        }

        #region IEquatable<ServerAlarm> Members

        public bool Equals(ServerAlarm other)
        {
            return ServerAlarmCore.Alarm.Equals(other.ServerAlarmCore.Alarm);
        }

        public static bool operator !=(ServerAlarm alarm1, ServerAlarm alarm2)
        {
            if (ReferenceEquals(alarm1, null))
                return (!ReferenceEquals(alarm2, null));

            if (ReferenceEquals(alarm2, null))
                return true; // expression (!ReferenceEquals(v1, null)) always true

            return !alarm1.Equals(alarm2);
        }

        public static bool operator ==(ServerAlarm alarm1, ServerAlarm alarm2)
        {
            if (ReferenceEquals(alarm1, null))
                return (ReferenceEquals(alarm2, null));

            if (ReferenceEquals(alarm2, null))
                return false; // expression(ReferenceEquals(v1, null)) always false if reached this point

            return alarm1.Equals(alarm2);
        }

        #endregion

        public override bool Equals(object obj)
        {
            return Equals(obj as ServerAlarm);
        }

        public override int GetHashCode()
        {
            return ServerAlarmCore.Alarm.GetHashCode();
        }

        public virtual PresentationGroup GetPresentationGroup()
        {
            return null;
        }

        public virtual  string GetPresentationDescription()
        {
            return ServerAlarmCore.Description;
        }
    }
}
