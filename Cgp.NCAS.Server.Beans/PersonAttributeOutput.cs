using System;
using Contal.Cgp.Globals;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.Server.Beans
{
    [Serializable()]
    public class PersonAttributeOutput : AOrmObject
    {
        public const string COLUMN_ID = "Id";
        public const string COLUMN_ISENABLED = "IsEnabled";
        public const string COLUMN_INTERVAL = "Interval";
        public const string COLUMN_FAILSCOUNT = "FailsCount";
        public const string COLUMN_OUTPUT = "Output";
        public const string COLUMN_LASTREPORTDATE = "LastReportDate";
        public const string COLUMN_IDTIMEZONE = "IdTimeZone";

        [LwSerialize]
        public virtual Guid Id { get; set; }
        [LwSerialize]
        public virtual bool IsEnabled { get; set; }

        [LwSerialize]
        public virtual int Interval { get; set; }

        [LwSerialize]
        public virtual int FailsCount { get; set; }

        [LwSerialize]
        public virtual string Output { get; set; }

        [LwSerialize]
        public virtual DateTime? LastReportDate { get; set; }

      //  public virtual Cgp.Server.Beans.TimeZone TimeZone { get; set; }

        [LwSerialize]
        public virtual Guid IdTimeZone { get; set; }


        public override string ToString()
        {
            return $"Fails Count: {FailsCount}, Output: {Output}";
        }

        public override bool Compare(object obj)
        {
            if (obj == null)
                return false;

            if (obj is PersonAttributeOutput)
            {
                return (obj as PersonAttributeOutput).Id == Id;
            }
            else
            {
                return false;
            }
        }

        public override string GetIdString()
        {
            return Id.ToString();
        }

        public override object GetId()
        {
            return Id;
        }

        public override IModifyObject CreateModifyObject()
        {
            return null;
        }

        public override ObjectType GetObjectType()
        {
            return ObjectType.PersonAttributeOutput;
        }
    }
}
