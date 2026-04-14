using System;
using Contal.Cgp.Globals;
using Contal.Cgp.Server.Beans;

using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.Server.Beans
{
    [Serializable()]
    [LwSerialize(326)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    [LwSerializeNoParent]
    public class SecurityDayInterval : AOrmObjectWithVersion
    {
        public const string COLUMNIDINTERVAL = "IdInterval";
        public const string COLUMNMINUTESFROM = "MinutesFrom";
        public const string COLUMNMINUTESTO = "MinutesTo";
        public const string COLUMNSECURITYDAILYPLAN = "SecurityDailyPlan";
        public const string ColumnVersion = "Version";

        public SecurityDayInterval()
        {
        }

        [LwSerializeAttribute()]
        public virtual Guid IdInterval { get; set; }
        [LwSerializeAttribute()]
        public virtual short MinutesFrom { get; set; }
        [LwSerializeAttribute()]
        public virtual short MinutesTo { get; set; }
        [LwSerializeAttribute()]
        public virtual byte IntervalType { get; set; }

        public virtual SecurityDailyPlan SecurityDailyPlan { get; set; }

        public override bool Compare(object obj)
        {
            if (obj == null)
                return false;

            if (obj is SecurityDayInterval)
            {
                return (obj as SecurityDayInterval).IdInterval == IdInterval;
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
            return ObjectType.SecurityDayInterval;
        }
    }
}
