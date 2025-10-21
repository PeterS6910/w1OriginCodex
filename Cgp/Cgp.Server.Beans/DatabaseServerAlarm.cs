using System;

namespace Contal.Cgp.Server.Beans
{
    public class DatabaseServerAlarm
    {
        public virtual Guid IdOwner { get; set; }
        public virtual Guid IdAlarm { get; set; }
        public virtual byte[] RawData { get; set; }

        public override bool Equals(object obj)
        {
            var databaseServerAlarm = obj as DatabaseServerAlarm;

            if (databaseServerAlarm == null)
                return false;

            return databaseServerAlarm.IdOwner == IdOwner
                   && databaseServerAlarm.IdAlarm == IdAlarm;
        }

        public override int GetHashCode()
        {
            return IdOwner.GetHashCode() ^ IdAlarm.GetHashCode();
        }
    }
}
