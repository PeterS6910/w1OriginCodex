using System;

using Contal.Cgp.Globals;
using Contal.Cgp.Server.Beans;

namespace Contal.Cgp.NCAS.Server.Beans.ModifyObjects
{
    [Serializable]
    public class MultiDoorModifyObj : AModifyObject
    {
        public override ObjectType GetOrmObjectType
        {
            get { return ObjectType.MultiDoor; }
        }

        public MultiDoorModifyObj(MultiDoor multiDoor)
        {
            Id = multiDoor.IdMultiDoor;
            FullName = multiDoor.ToString();
            Description = multiDoor.Description;
        }
    }
}
