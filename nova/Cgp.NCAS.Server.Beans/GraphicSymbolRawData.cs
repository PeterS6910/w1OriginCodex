using System;
using System.Collections.Generic;
using Contal.Cgp.Globals;
using Contal.Cgp.Server.Beans;

namespace Contal.Cgp.NCAS.Server.Beans
{
    [Serializable()]
    public enum SymbolDataType
    {
        Vector, Raster
    }

    [Serializable()]
    public class GraphicSymbolRawData : AOrmObject
    {
        public const string COLUMN_ID = "Id";

        public virtual Guid Id { get; set; }
        public virtual byte[] RawData { get; set; }
        public virtual SymbolDataType DataType { get; set; }
        //public virtual ICollection<GraphicSymbol> GraphicSymbols { get; set; }
        /*
          <bag name="GraphicSymbols" cascade="all">
      <key column="IdRawData" />
      <one-to-many class="Contal.Cgp.NCAS.Server.Beans.GraphicSymbol, Cgp.NCAS.Server.Beans"/>
    </bag>*/
        public override bool Compare(object obj)
        {
            if (obj == null)
                return false;

            if (obj is GraphicSymbolRawData)
            {
                return (obj as GraphicSymbolRawData).Id == Id;
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

        public override Contal.Cgp.Globals.ObjectType GetObjectType()
        {
            return Contal.Cgp.Globals.ObjectType.GraphicSymbolRawData;
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
