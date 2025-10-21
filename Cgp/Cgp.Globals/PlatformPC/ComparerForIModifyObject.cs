using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Contal.Cgp.Globals
{
    public class ComparerForIModifyObject : IComparer<IModifyObject>
    {
        public int Compare(IModifyObject modifyObject1, IModifyObject modifyObject2)
        {
            var objectType1 = modifyObject1.GetOrmObjectType;
            var objectType2 = modifyObject2.GetOrmObjectType;

            if (objectType1 != objectType2)
                return objectType1.CompareTo(objectType2);

            return modifyObject1.ToString().CompareTo(modifyObject2.ToString());
        }
    }
}
