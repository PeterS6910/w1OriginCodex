using System;
using System.Collections.Generic;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Globals;
using Contal.Cgp.NCAS.Server.Beans.ModifyObjects;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.Server.Beans
{
    [Serializable]
    [LwSerialize(321)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    [LwSerializeNoParent]
    public class MultiDoor :
        AOrmObjectWithVersion,
        ICardReaderObject
    {
        public const string ColumnIdMultiDoorEnvironment = "IdMultiDoor";
        public const string ColumnName = "Name";
        public const string ColumnObjectType = "ObjectType";
        public const string ColumnCardReader = "CardReader";
        public const string ColumnType = "Type";
        public const string ColumnBlockAlarmArea = "BlockAlarmArea";
        public const string ColumnDoorTimeUnlock = "DoorTimeUnlock";
        public const string ColumnDoorTimeOpen = "DoorTimeOpen";
        public const string ColumnDoorTimePreAlarm = "DoorTimePreAlarm";
        public const string ColumnDoorDelayBeforeUnlock = "DoorDelayBeforeUnlock";
        public const string ColumnDoorDelayBeforeLock = "DoorDelayBeforeLock";
        public const string ColumnDoorDelayBeforeClose = "DoorDelayBeforeClose";
        public const string ColumnDoors = "Doors";
        public const string ColumnDescription = "Description";
        public const string ColumnVersion = "Version";

        [LwSerializeAttribute]
        public virtual Guid IdMultiDoor { get; set; }

        public virtual string Name { get; set; }

        public virtual byte ObjectType
        {
            get { return (byte)Cgp.Globals.ObjectType.MultiDoor; }
            set { }
        }

        [LwSerializeAttribute]
        public virtual Guid CardReaderId { get; set; }
        public virtual CardReader CardReader { get; set; }

        public virtual MultiDoorType Type { get; set; }
        public virtual AlarmArea BlockAlarmArea { get; set; }

        [LwSerializeAttribute]
        public virtual byte DoorTimeUnlock { get; set; }
        [LwSerializeAttribute]
        public virtual int DoorTimeOpen { get; set; }
        [LwSerializeAttribute]
        public virtual byte DoorTimePreAlarm { get; set; }
        
        public virtual int DoorDelayBeforeUnlock { get; set; }
        public virtual int DoorDelayBeforeLock { get; set; }
        public virtual int DoorDelayBeforeClose { get; set; }
        
        public virtual bool SensorsOpenDoorsInverted { get; set; }
        public virtual bool SensorsOpenDoorsBalanced { get; set; }

        public virtual bool ElectricStrikeImpulse { get; set; }
        public virtual int ElectricStrikeImpulseDelay { get; set; }

        public virtual bool ExtraElectricStrikeImpulse { get; set; }
        public virtual int ExtraElectricStrikeImpulseDelay { get; set; }

        public virtual ICollection<MultiDoorElement> Doors { get; set; }
        
        public virtual string Description { get; set; }

        public override bool Compare(object obj)
        {
            var elevator = obj as MultiDoor;

            if (elevator == null)
                return false;

            return elevator.IdMultiDoor == IdMultiDoor;
        }

        public override string GetIdString()
        {
            return IdMultiDoor.ToString();
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
            return Cgp.Globals.ObjectType.MultiDoor;
        }

        public override object GetId()
        {
            return IdMultiDoor;
        }

        public override IModifyObject CreateModifyObject()
        {
            return new MultiDoorModifyObj(this);
        }

        public override string ToString()
        {
            return Name;
        }

        public virtual void PrepareToSend()
        {
            CardReaderId = CardReader != null ? CardReader.IdCardReader : Guid.Empty;
        }
    }
}
