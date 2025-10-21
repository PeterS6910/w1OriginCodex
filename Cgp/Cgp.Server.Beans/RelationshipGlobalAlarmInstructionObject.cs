using System;
using Contal.Cgp.Globals;
using Contal.LwSerialization;

namespace Contal.Cgp.Server.Beans
{
    public class RelationshipGlobalAlarmInstructionObject : AOrmObject
    {
        public const string COLUMN_ID_RELATIONSHIP_GLOBAL_ALARM_INSTRUCTION_OBJECT = "IdRelationshipGlobalAlarmInstructionObject";
        public const string COLUMN_ID_GLOBAL_ALARM_INSTRUCTION = "IdGlobalAlarmInstruction";
        public const string COLUMN_OBJECT_ID = "ObjectId";
        public const string COLUMN_OBJECT_TYPE = "ObjectType";

        public virtual Guid IdRelationshipGlobalAlarmInstructionObject { get; set; }
        public virtual Guid IdGlobalAlarmInstruction { get; set; }
        public virtual string ObjectId { get; set; }
        public virtual byte ObjectType { get; set; }

        public override bool Compare(object obj)
        {
            if (obj == null)
                return false;

            if (obj is RelationshipGlobalAlarmInstructionObject)
            {
                return (obj as RelationshipGlobalAlarmInstructionObject).IdRelationshipGlobalAlarmInstructionObject == IdRelationshipGlobalAlarmInstructionObject;
            }
            else
            {
                return false;
            }
        }

        public override string GetIdString()
        {
            return IdRelationshipGlobalAlarmInstructionObject.ToString();
        }

        public override object GetId()
        {
            return IdRelationshipGlobalAlarmInstructionObject;
        }

        public override IModifyObject CreateModifyObject()
        {
            return null;
        }

        public override Contal.Cgp.Globals.ObjectType GetObjectType()
        {
            return Contal.Cgp.Globals.ObjectType.RelationshipGlobalAlarmInstructionObject;
        }
    }
}
