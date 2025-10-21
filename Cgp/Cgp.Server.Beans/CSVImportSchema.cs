using System;
using System.Collections.Generic;

using Contal.Cgp.Globals;

namespace Contal.Cgp.Server.Beans
{
    [Serializable()]
    public class CSVImportSchema : AOrmObject
    {
        public virtual Guid IdCSVImportSchema { get; set; }
        public virtual string CSVImportSchemaName { get; set; }
        public virtual string DateFormat { get; set; }
        public virtual string DepartmentFolder { get; set; }
        public virtual char Separator { get; set; }
        public virtual bool ParseBirthDateFromPersonId { get; set; }
        public virtual ICollection<CSVImportColumn> CSVImportColumns { get; set; }
        public virtual byte ObjectType { get; set; }
        public virtual bool AllowCompositPersonalId { get; set; }
        public virtual int CompositPersonalIdFirstColumn { get; set; }
        public virtual int CompositPersonalIdSecondColumn { get; set; }
        public virtual byte? PersonalIdValidationType { get; set; }

        public CSVImportSchema()
        {
            ObjectType = (byte)Cgp.Globals.ObjectType.CSVImportSchema;
        }

        public override bool Compare(object obj)
        {
            if (obj == null)
                return false;

            if (obj is CSVImportColumn)
            {
                return (obj as CSVImportColumn).IdCSVImportColumn == IdCSVImportSchema;
            }
            else
            {
                return false;
            }
        }

        public override bool Contains(string expression)
        {
            expression = expression.ToLower();
            return false;
        }

        public override string GetIdString()
        {
            return IdCSVImportSchema.ToString();
        }

        public override object GetId()
        {
            return IdCSVImportSchema;
        }

        public override IModifyObject CreateModifyObject()
        {
            return null;
        }

        public override ObjectType GetObjectType()
        {
            return Contal.Cgp.Globals.ObjectType.CSVImportSchema;
        }

        public override string ToString()
        {
            return CSVImportSchemaName;
        }
    }
}
