using System;

using Contal.Cgp.Globals;

namespace Contal.Cgp.Server.Beans
{
    [Serializable()]
    public class CSVImportColumn : AOrmObject
    {
        public virtual Guid IdCSVImportColumn { get; set; }
        public virtual int ColumnIndex { get; set; }
        public virtual int FirstColumn { get; set; }
        public virtual int SecondColumn { get; set; }
        public virtual char Separator { get; set; }

        public virtual CSVImportSchema CSVImportSchema { get; set; }

        public override bool Compare(object obj)
        {
            if (obj == null)
                return false;

            if (obj is CSVImportColumn)
            {
                return (obj as CSVImportColumn).IdCSVImportColumn == IdCSVImportColumn;
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
            return IdCSVImportColumn.ToString();
        }

        public override object GetId()
        {
            return IdCSVImportColumn;
        }

        public override IModifyObject CreateModifyObject()
        {
            return null;
        }

        public override ObjectType GetObjectType()
        {
            return ObjectType.CSVImportColumn;
        }
    }
}
