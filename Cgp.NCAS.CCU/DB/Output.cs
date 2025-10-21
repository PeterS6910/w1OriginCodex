using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;

using Contal.IwQuick.Data;
using Contal.LwSerialization;
using Contal.Cgp.Globals;

namespace Contal.Cgp.NCAS.CCU.DB
{
    [LwSerialize(324)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    public class Output : IDbObject
    {
        [LwSerialize()]
        public virtual Guid IdOutput { get; set; }
        [LwSerialize()]
        public virtual byte OutputNumber { get; set; }
        [LwSerialize()]
        public virtual byte OutputType { get; set; }
        [LwSerialize()]
        public virtual bool SettingsForcedToOff { get; set; }
        [LwSerialize()]
        public virtual int SettingsDelayToOn { get; set; }
        [LwSerialize()]
        public virtual int SettingsDelayToOff { get; set; }
        [LwSerialize()]
        public virtual int SettingsPulseLength { get; set; }
        [LwSerialize()]
        public virtual int SettingsPulseDelay { get; set; }
        [LwSerialize()]
        public virtual bool Inverted { get; set; }
        private Guid _guidDCU = Guid.Empty;
        [LwSerialize()]
        public virtual Guid GuidDCU { get { return _guidDCU; } set { _guidDCU = value; } }
        private Guid _guidCCU = Guid.Empty;
        [LwSerialize()]
        public virtual Guid GuidCCU { get { return _guidCCU; } set { _guidCCU = value; } }
        [LwSerialize()]
        public virtual byte ControlType { get; set;}
        private ObjectType _onOffObjectObjectType;
        [LwSerialize()]
        public virtual ObjectType OnOffObjectObjectType { get { return _onOffObjectObjectType; } set { _onOffObjectObjectType = value; } }
        [LwSerialize()]
        public virtual Guid? OnOffObjectId { get; set; }
        [LwSerialize()]
        public virtual bool AlarmControlByObjOn { get; set; }
        [LwSerialize()]
        public virtual bool RealStateChanges { get; set; }

        public Guid GetGuid()
        {
            return IdOutput;
        }

        public Contal.Cgp.Globals.ObjectType GetObjectType()
        {
            return Contal.Cgp.Globals.ObjectType.Output;
        }

        public override string ToString()
        {
            return "Output" + OutputNumber;
        }
    }

    public enum OutputControl : byte
    {
        [Name("unblocked")] //neblokovany
        unblocked = 0,
        [Name("manualBlocked")] // manualne blokovany
        ManuallyBlocked = 1,
        [Name("forcedOn")] //vnuteny
        ForcedOn = 2,
        [Name("controledByObject")] //riadeny objektom
        ControlledByObject = 3,
        [Name("controledByDoorEnvironment")] //riadeny dvernym automatom
        ControlledByDSM = 4,
        [Name("watchdog")]
        watchdog = 5
    }

    public class OutputControlTypes
    {
        private OutputControl _value;
        public OutputControl Value
        {
            get { return _value; }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
        }

        public OutputControlTypes(OutputControl value, string name)
        {
            _value = value;
            _name = name;
        }

        public override string ToString()
        {
            return Name;
        }

        public static IList<OutputControlTypes> GetOutputControlTypesList(Contal.IwQuick.Localization.LocalizationHelper localizationHelper)
        {
            IList<OutputControlTypes> list = new List<OutputControlTypes>();
            FieldInfo[] fieldsInfo = typeof(OutputControl).GetFields();
            foreach (FieldInfo fieldInfo in fieldsInfo)
            {
                NameAttribute[] attribs = fieldInfo.GetCustomAttributes(
                typeof(NameAttribute), false) as NameAttribute[];

                if (attribs.Length > 0)
                {
                    list.Add(new OutputControlTypes((OutputControl)fieldInfo.GetValue(fieldInfo), localizationHelper.GetString("OutputControlTypes_" + attribs[0].Name)));
                }
            }

            return list;
        }

        public static OutputControlTypes GetOutputControlType(Contal.IwQuick.Localization.LocalizationHelper localizationHelper, IList<OutputControlTypes> listOutputControlTypes, byte outputControlType)
        {
            if (listOutputControlTypes == null)
            {
                return GetOutputControlType(localizationHelper, outputControlType);
            }
            else
            {
                foreach (OutputControlTypes listoutputControlType in listOutputControlTypes)
                {
                    if ((byte)listoutputControlType.Value == outputControlType)
                        return listoutputControlType;
                }
            }
            return null;
        }

        public static OutputControlTypes GetOutputControlType(Contal.IwQuick.Localization.LocalizationHelper localizationHelper, byte outputControlType)
        {
            FieldInfo[] fieldsInfo = typeof(OutputControl).GetFields();
            foreach (FieldInfo fieldInfo in fieldsInfo)
            {
                NameAttribute[] attribs = fieldInfo.GetCustomAttributes(
                typeof(NameAttribute), false) as NameAttribute[];

                if (attribs.Length > 0)
                {
                    if ((byte)fieldInfo.GetValue(fieldInfo) == outputControlType)
                        return (new OutputControlTypes((OutputControl)fieldInfo.GetValue(fieldInfo), localizationHelper.GetString("OutputControlTypes_" + attribs[0].Name)));
                }
            }
            return null;
        }
    }

    public enum OutputCharacteristic : byte
    {
        Unconfigured = 0xFF,
        level = 0,
        pulsed = 1,
        frequency = 2,
    }

    public class OutputTypes
    {
        private OutputCharacteristic _value;
        public OutputCharacteristic Value
        {
            get { return _value; }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
        }

        public OutputTypes(OutputCharacteristic value, string name)
        {
            _value = value;
            _name = name;
        }

        public override string ToString()
        {
            return Name;
        }

        public static IList<OutputTypes> GetOutputTypesList(Contal.IwQuick.Localization.LocalizationHelper localizationHelper)
        {
            IList<OutputTypes> list = new List<OutputTypes>();
            FieldInfo[] fieldsInfo = typeof(OutputCharacteristic).GetFields();
            foreach (FieldInfo fieldInfo in fieldsInfo)
            {
                NameAttribute[] attribs = fieldInfo.GetCustomAttributes(
                typeof(NameAttribute), false) as NameAttribute[];

                if (attribs.Length > 0)
                {
                    list.Add(new OutputTypes((OutputCharacteristic)fieldInfo.GetValue(fieldInfo), localizationHelper.GetString("OutputTypes_" + attribs[0].Name)));
                }
            }

            return list;
        }

        public static OutputTypes GetOutputType(Contal.IwQuick.Localization.LocalizationHelper localizationHelper, IList<OutputTypes> listOutputTypes, byte outputType)
        {
            if (listOutputTypes == null)
            {
                return GetOutputType(localizationHelper, outputType);
            }
            else
            {
                foreach (OutputTypes listoutputType in listOutputTypes)
                {
                    if ((byte)listoutputType.Value == outputType)
                        return listoutputType;
                }
            }
            return null;
        }

        public static OutputTypes GetOutputType(Contal.IwQuick.Localization.LocalizationHelper localizationHelper, byte outputType)
        {
            FieldInfo[] fieldsInfo = typeof(OutputCharacteristic).GetFields();
            foreach (FieldInfo fieldInfo in fieldsInfo)
            {
                NameAttribute[] attribs = fieldInfo.GetCustomAttributes(
                typeof(NameAttribute), false) as NameAttribute[];

                if (attribs.Length > 0)
                {
                    if ((byte)fieldInfo.GetValue(fieldInfo) == outputType)
                        return (new OutputTypes((OutputCharacteristic)fieldInfo.GetValue(fieldInfo), localizationHelper.GetString("OutputTypes_" + attribs[0].Name)));
                }
            }
            return null;
        }
    }
}
