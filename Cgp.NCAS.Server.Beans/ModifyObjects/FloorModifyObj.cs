using System;
using Contal.Cgp.Globals;
using Contal.Cgp.Server.Beans;

namespace Contal.Cgp.NCAS.Server.Beans.ModifyObjects
{
    [Serializable]
    public class FloorModifyObj : AModifyObject
    {
        public override ObjectType GetOrmObjectType
        {
            get { return ObjectType.Floor; }
        }

        public FloorModifyObj(Floor floor)
        {
            Id = floor.IdFloor;
            FullName = floor.ToString();
            Description = floor.Description;
        }
    }
}
