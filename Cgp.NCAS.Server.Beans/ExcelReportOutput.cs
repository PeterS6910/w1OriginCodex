using System;
using Contal.Cgp.Globals;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.Server.Beans
{
    [Serializable()]
    public class ExcelReportOutput : AOrmObject
    {
        public const string COLUMN_ID = "Id";
        public const string COLUMN_ISENABLED = "IsEnabled";
        public const string COLUMN_INTERVAL = "Interval";
        public const string COLUMN_TYPE = "Type";
        public const string COLUMN_OUTPUT = "Output";
        public const string COLUMN_FILENAME = "Filename";
        public const string COLUMN_ADDPARAM = "Param";
        public const string COLUMN_IDTIMEZONE = "IdTimeZone";

        public virtual Guid Id { get; set; }
        public virtual bool IsEnabled { get; set; }
        public virtual int Interval { get; set; }
        public virtual string Type { get; set; }
        public virtual string Output { get; set; }
        public virtual string Filename { get; set; }
        public virtual string Param { get; set; }

        public virtual Cgp.Server.Beans.TimeZone TimeZone { get; set; }

        public override string ToString()
        {
            return $"Filename: {Filename}, Server Directory: {Output}";
        }

        public override bool Compare(object obj)
        {
            if (obj == null)
                return false;

            if (obj is ExcelReportOutput)
            {
                return (obj as ExcelReportOutput).Id == Id;
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
            return ObjectType.ExcelReportOutput;
        }
    }
}
