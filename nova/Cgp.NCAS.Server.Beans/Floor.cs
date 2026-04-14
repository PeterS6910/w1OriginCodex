using System;
using System.Collections.Generic;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Server.Beans.ModifyObjects;
using Contal.Cgp.Server.Beans;

namespace Contal.Cgp.NCAS.Server.Beans
{
    [Serializable]
    public class Floor :
        AOrmObject,
        ICardReaderObject
    {
        public const string ColumnIdFloor = "IdFloor";
        public const string ColumnName = "Name";
        public const string ColumnObjectType = "ObjectType";       
        public const string ColumnFloorNumber = "DoorIndex";
        public const string ColumnBlockAlarmArea = "BlockAlarmArea";
        public const string ColumnDoors = "Doors";
        public const string ColumnDescription = "Description";

        public virtual Guid IdFloor { get; set; }

        public virtual string Name { get; set; }

        public virtual byte ObjectType
        {
            get { return (byte) Cgp.Globals.ObjectType.Floor; }
            set { }
        }

        public virtual int FloorNumber { get; set; }
        public virtual AlarmArea BlockAlarmArea { get; set; }

        public virtual ICollection<MultiDoorElement> Doors { get; set; }

        public virtual string Description { get; set; }

        public override bool Compare(object obj)
        {
            var floor = obj as Floor;

            if (floor == null)
                return false;

            return floor.IdFloor == IdFloor;
        }

        public override string GetIdString()
        {
            return IdFloor.ToString();
        }

        public virtual IEnumerable<ICardReaderObject> GetChildObjects()
        {
            if (Doors == null)
                yield break;

            foreach (var door in Doors)
            {
                yield return door;
            }
        }

        public override Cgp.Globals.ObjectType GetObjectType()
        {
            return Cgp.Globals.ObjectType.Floor;
        }

        public override object GetId()
        {
            return IdFloor;
        }

        public override IModifyObject CreateModifyObject()
        {
            return new FloorModifyObj(this);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
