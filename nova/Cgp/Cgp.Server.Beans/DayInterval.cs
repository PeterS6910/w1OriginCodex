using System;
using Contal.Cgp.Globals;
using Contal.IwQuick.Data;

namespace Contal.Cgp.Server.Beans
{
    [Serializable()]
    public class DayInterval : AOrmObject
    {
        public const string COLUMNIDINTERVAL = "IdInterval";
        public const string COLUMNMINUTESFROM = "MinutesFrom";
        public const string COLUMNMINUTESTO = "MinutesTo";
        public const string COLUMNDAILYPLAN = "DailyPlan";

        public DayInterval()
        {
        }

        [LwSerializeAttribute()]
        public virtual Guid IdInterval { get; set; }
        [LwSerializeAttribute()]
        public virtual short MinutesFrom { get; set; }
        [LwSerializeAttribute()]
        public virtual short MinutesTo { get; set; }

        public virtual DailyPlan DailyPlan { get; set; }

        public override bool Compare(object obj)
        {
            if (obj == null)
                return false;

            if (obj is DayInterval)
            {
                return (obj as DayInterval).IdInterval == IdInterval;
            }
            else
            {
                return false;
            }
        }

        public override string GetIdString()
        {
            return IdInterval.ToString();
        }

        public override object GetId()
        {
            return IdInterval;
        }

        public override IModifyObject CreateModifyObject()
        {
            return null;
        }

        public override Contal.Cgp.Globals.ObjectType GetObjectType()
        {
            return Contal.Cgp.Globals.ObjectType.DayInterval;
        }
    }
}
