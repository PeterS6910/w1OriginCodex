using System;

using Contal.Cgp.Globals;
using Contal.Cgp.Server.Beans;

namespace Contal.Cgp.NCAS.Server.Beans.ModifyObjects
{
    [Serializable]
    public class MultiDoorElementModObj : AModifyObject
    {
        public override ObjectType GetOrmObjectType
        {
            get { return ObjectType.MultiDoorElement; }
        }

        public MultiDoorElementModObj(MultiDoorElement multiDoorElement)
        {
            Id = multiDoorElement.IdMultiDoorElement;
            FullName = multiDoorElement.ToString();
            Description = multiDoorElement.Description;
        }
    }
}
