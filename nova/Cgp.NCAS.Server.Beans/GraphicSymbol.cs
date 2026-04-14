using System;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Globals;

namespace Contal.Cgp.NCAS.Server.Beans
{
    [Serializable()]
    public class GraphicSymbol : AOrmObject
    {
        public const string COLUMN_ID = "Id";
        public const string COLUMN_SYMBOLTYPE = "SymbolType";
        public const string COLUMN_SYMBOLSTATE = "SymbolState";
        public const string COLUMN_IDTEMPLATE = "IdTemplate";
        public const string COLUMN_IDRAWDATA = "IdRawData";
        public const string COLUMN_DATATYPE = "DataType";
        public const string COLUMN_FILTERKEY = "FilterKey";

        public virtual Guid Id { get; set; }
        public virtual SymbolType SymbolType { get; set; }
        public virtual State SymbolState { get; set; }
        public virtual Guid IdTemplate { get; set; }
        public virtual Guid IdRawData { get; set; }
        public virtual string FilterKey { get; set; }

        public override string ToString()
        {
            return SymbolType.ToString();
        }

        public override bool Compare(object obj)
        {
            if (obj == null)
                return false;

            if (obj is GraphicSymbol)
            {
                return (obj as GraphicSymbol).SymbolType == SymbolType;
            }
            else
            {
                return false;
            }
        }

        public override string GetIdString()
        {
            return SymbolType.ToString();
        }

        public override Contal.Cgp.Globals.ObjectType GetObjectType()
        {
            return Contal.Cgp.Globals.ObjectType.GraphicSymbol;
        }

        public override object GetId()
        {
            return Id;
        }

        public override IModifyObject CreateModifyObject()
        {
            return null;
        }
    }
}
