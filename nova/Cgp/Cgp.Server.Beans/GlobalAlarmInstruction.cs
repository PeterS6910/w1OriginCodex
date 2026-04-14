using System;

using Contal.Cgp.Globals;

namespace Contal.Cgp.Server.Beans
{
    [Serializable]
    public class GlobalAlarmInstruction : AOrmObject
    {
        public const string COLUMN_ID_GLOBAL_ALARM_INSTRUCTION = "IdGlobalAlarmInstruction";
        public const string COLUMN_NAME = "Name";
        public const string COLUMN_INSTRUCTIONS = "Instructions";
        public const string COLUMN_OBJECT_TYPE = "ObjectType";
        public const string COLUMN_DESCRIPTION = "Description";

        public virtual Guid IdGlobalAlarmInstruction { get; set; }
        public virtual string Name { get; set; }
        public virtual string Instructions { get; set; }
        public virtual byte ObjectType { get; set; }
        public virtual string Description { get { return Instructions; } }

        public GlobalAlarmInstruction()
        {
            ObjectType = (byte)Globals.ObjectType.GlobalAlarmInstruction;
        }

        public override string ToString()
        {
            return Name;
        }

        public override bool Compare(object obj)
        {
            var instruction = obj as GlobalAlarmInstruction;

            return instruction != null &&
                   instruction.IdGlobalAlarmInstruction == IdGlobalAlarmInstruction;
        }

        public override bool Contains(string expression)
        {
            expression = expression.ToLower();

            return 
                Name.ToLower().Contains(expression) || 
                    Instructions != null &&
                    Instructions.ToLower().Contains(expression);
        }

        public override string GetIdString()
        {
            return IdGlobalAlarmInstruction.ToString();
        }

        public override object GetId()
        {
            return IdGlobalAlarmInstruction;
        }

        public override IModifyObject CreateModifyObject()
        {
            return new GlobalAlarmInstructionModifyObj(this);
        }

        public override ObjectType GetObjectType()
        {
            return Globals.ObjectType.GlobalAlarmInstruction;
        }
    }

    [Serializable]
    public class GlobalAlarmInstructionShort : IShortObject
    {
        public Guid IdGlobalAlarmInstruction { get; set; }
        public string Name { get; set; }
        public string Instructions { get; set; }

        public GlobalAlarmInstructionShort(GlobalAlarmInstruction globalAlarmInstruction)
        {
            IdGlobalAlarmInstruction = globalAlarmInstruction.IdGlobalAlarmInstruction;
            Name = globalAlarmInstruction.Name;
            Instructions = globalAlarmInstruction.Instructions;
        }

        public override string ToString()
        {
            return Name;
        }

        #region IShortObject Members

        public ObjectType ObjectType
        {
            get
            {
                return ObjectType.GlobalAlarmInstruction;
            }
        }

        public string GetSubTypeImageString(object value)
        {
            return string.Empty;
        }

        public object Id { get { return IdGlobalAlarmInstruction; } }

        string IShortObject.Name { get { return Name; } }

        #endregion
    }

    [Serializable]
    public class GlobalAlarmInstructionModifyObj : AModifyObject
    {
        public override ObjectType GetOrmObjectType { get { return ObjectType.GlobalAlarmInstruction; } }


        public override string GetObjectSubType(byte option)
        {
            switch (option)
            {
                case 0: return ObjectSubTypes[0];
            }
            return ObjectSubTypes[0];
        }

        public GlobalAlarmInstructionModifyObj(GlobalAlarmInstruction globalAlarmInstruction)
        {
            Id = globalAlarmInstruction.IdGlobalAlarmInstruction;
            FullName = globalAlarmInstruction.ToString();
            Description = globalAlarmInstruction.Instructions;
        }
    }
}
