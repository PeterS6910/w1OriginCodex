using System;
using System.Collections.Generic;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Server.Beans.ModifyObjects;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.Server.Beans
{
    [Serializable]
    [LwSerialize(322)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    [LwSerializeNoParent]
    public class MultiDoorElement :
        AOrmObjectWithVersion,
        ICardReaderObject
    {
        public const string ColumnIdMultiDoorElement = "IdMultiDoorElement";
        public const string ColumnName = "Name";
        public const string ColumnObjectType = "ObjectType";
        public const string ColumnCkUnique = "CkUnique";
        public const string ColumnMultiDoor = "MultiDoor";
        public const string ColumnFloor = "Floor";
        public const string ColumnDoorIndex = "DoorIndex";
        public const string ColumnSensorOpenDoor = "SensorOpenDoor";
        public const string ColumnSensorsOpenDoorsInverted = "SensorsOpenDoorsInverted";
        public const string ColumnSensorsOpenDoorsBalanced = "SensorsOpenDoorsBalanced";
        public const string ColumnElectricStrike = "ElectricStrike";
        public const string ColumnElectricStrikeImpulse = "ElectricStrikeImpulse";
        public const string ColumnElectricStrikeImpulseDelay = "ElectricStrikeImpulseDelay";
        public const string ColumnExtraElectricStrike = "ExtraElectricStrike";
        public const string ColumnExtraElectricStrikeImpulse = "ExtraElectricStrikeImpulse";
        public const string ColumnExtraElectricStrikeImpulseDelay = "ExtraElectricStrikeImpulseDelay";
        public const string ColumnBlockAlarmArea = "BlockAlarmArea";
        public const string ColumnBlockOnOffObjectType = "BlockOnOffObjectType";
        public const string ColumnBlockOnOffObjectId = "BlockOnOffObjectId";
        public const string ColumnDescription = "Description";
        public const string ColumnVersion = "Version";

        [LwSerializeAttribute]
        public virtual Guid IdMultiDoorElement { get; set; }

        public virtual string Name { get; set; }

        public virtual byte ObjectType
        {
            get { return (byte) Cgp.Globals.ObjectType.MultiDoorElement; }
            set { }
        }

        public virtual Guid CkUnique { get; set; }

        [LwSerializeAttribute]
        public virtual Guid MultiDoorId { get; set; }
        public virtual MultiDoor MultiDoor { get; set; }
        [LwSerializeAttribute]
        public virtual Guid FloorId { get; set; }
        public virtual Floor Floor { get; set; }
        public virtual int DoorIndex { get; set; }

        [LwSerializeAttribute]
        public virtual Guid? SensorOpenDoorId { get; set; }
        public virtual Input SensorOpenDoor { get; set; }

        [LwSerializeAttribute]
        public virtual Guid ElectricStrikeId { get; set; }
        public virtual Output ElectricStrike { get; set; }

        [LwSerializeAttribute]
        public virtual Guid? ExtraElectricStrikeId { get; set; }
        public virtual Output ExtraElectricStrike { get; set; }

        [LwSerializeAttribute]
        public virtual Guid? BlockAlarmAreaId { get; set; }
        public virtual AlarmArea BlockAlarmArea { get; set; }

        [LwSerializeAttribute]
        public virtual Cgp.Globals.ObjectType? BlockOnOffObjectType { get; set; }

        [LwSerializeAttribute]
        public virtual Guid? BlockOnOffObjectId { get; set; }

        public virtual string Description { get; set; }

        public MultiDoorElement()
        {
            CkUnique = Guid.NewGuid();
        }

        public override bool Compare(object obj)
        {
            var noCardReadersDoorEnvironment = obj as MultiDoorElement;

            if (noCardReadersDoorEnvironment == null)
                return false;

            return noCardReadersDoorEnvironment.IdMultiDoorElement == IdMultiDoorElement;
        }

        public override string GetIdString()
        {
            return IdMultiDoorElement.ToString();
        }

        public virtual IEnumerable<ICardReaderObject> GetChildObjects()
        {
            yield break;
        }

        public override Cgp.Globals.ObjectType GetObjectType()
        {
            return Cgp.Globals.ObjectType.MultiDoorElement;
        }

        public override object GetId()
        {
            return IdMultiDoorElement;
        }

        public override IModifyObject CreateModifyObject()
        {
            return new MultiDoorElementModObj(this);
        }

        public override string ToString()
        {
            return string.Format("{0}{1}{2}", MultiDoor.Name, StringConstants.SLASHWITHSPACES, Name);
        }

        public virtual void PrepareToSend()
        {
            MultiDoorId = MultiDoor != null ? MultiDoor.IdMultiDoor : Guid.Empty;
            FloorId = Floor != null ? Floor.IdFloor : Guid.Empty;
            SensorOpenDoorId = SensorOpenDoor != null ? (Guid?) SensorOpenDoor.IdInput : null;
            ElectricStrikeId = ElectricStrike != null ? ElectricStrike.IdOutput : Guid.Empty;
            ExtraElectricStrikeId = ExtraElectricStrike != null ? (Guid?) ExtraElectricStrike.IdOutput : null;
            BlockAlarmAreaId = BlockAlarmArea != null ? (Guid?) BlockAlarmArea.IdAlarmArea : null;
        }
    }
}
