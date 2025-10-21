using System;
using System.Collections.Generic;
using Contal.Cgp.Globals;
using Contal.Cgp.Server.Beans;

namespace Contal.Cgp.NCAS.Server.Beans
{
    [Serializable()]
    public class GraphicSymbolTemplate : AOrmObject
    {
        public virtual Guid Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        //public virtual ICollection<GraphicSymbol> GraphicSymbols { get; set; }
        /*
         * <bag name="GraphicSymbols" cascade="all">
      <key column="IdTemplate" />
      <one-to-many class="Contal.Cgp.NCAS.Server.Beans.GraphicSymbol, Cgp.NCAS.Server.Beans"/>
    </bag>*/

        public override bool Compare(object obj)
        {
            if (obj == null)
                return false;

            if (obj is GraphicSymbolTemplate)
            {
                return (obj as GraphicSymbolTemplate).Id == Id;
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
            return Contal.Cgp.Globals.ObjectType.GraphicSymbolTemplate;
        }

        public override object GetId()
        {
            return Id;
        }

        public override IModifyObject CreateModifyObject()
        {
            return null;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
