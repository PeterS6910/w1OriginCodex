using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;

using Contal.Cgp.Globals;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.Server.Beans
{
    [Serializable()]
    [LwSerialize(324)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    [LwSerializeNoParent]
    public class Output : AOnOffObject, IOrmObjectWithAlarmInstructions, IComparable, IGetDcu
    {
        public const string COLUMNIDOUTPUT = "IdOutput";
        public const string COLUMNNAME = "Name";
        public const string COLUMNOUTPUTNUMBER = "OutputNumber";
        public const string COLUMNOUTPUTTYPE = "OutputType";
        public const string COLUMNSETTINGSFORCEDTOOFF = "SettingsForcedToOff";
        public const string COLUMNSETTINGSDELAYTOON = "SettingsDelayToOn";
        public const string COLUMNSETTINGSDELAYTOOFF = "SettingsDelayToOff";
        public const string COLUMNSETTINGSPULSELENGTH = "SettingsPulseLength";
        public const string COLUMNSETTINGSPULSEDELAY = "SettingsPulseDelay";
        public const string COLUMNDELAYTOON = "DelayToOn";
        public const string COLUMNDELAYTOOFF = "DelayToOff";
        public const string COLUMNINVERTED = "Inverted";
        public const string COLUMNALARMPRESENTATIONGROUP = "AlarmPresentationGroup";
        public const string COLUMNDCU = "DCU";
        public const string COLUMNCCU = "CCU";
        public const string COLUMNGUIDDCU = "GuidDCU";
        public const string COLUMNGUIDCCU = "GuidCCU";
        public const string COLUMNDESCRIPTION = "Description";
        public const string COLUMNCONTROLTYPE = "ControlType";
        public const string COLUMNONOFFOBJECTTYPE = "OnOffObjectType";
        public const string COLUMNONOFFOBJECTOBJECTTYPE = "OnOffObjectObjectType";
        public const string COLUMNONOFFOBJECTID = "OnOffObjectId";
        public const string COLUMNONOFFOBJECT = "OnOffObject";
        public const string COLUMNSTATE = "State";
        public const string COLUMNALARMCONROLBYOBJON = "AlarmControlByObjOn";
        public const string COLUMNSTATUSONOFF = "StatusOnOff";
        public const string COLUMNREALSTATUSONOFF = "RealStatusOnOff";
        public const string COLUMNSTATUSACTIVATED = "StatusActivated";
        public const string REALSTATECHANGES = "RealStateChanges";
        public const string COLUMNOBJECTTYPE = "ObjectType";
        public const string COLUMNCKUNIQUE = "CkUnique";
        public const string COLUMNENABLEPARENTINFULLNAME = "EnableParentInFullName";
        public const string COLUMNLOCALALARMINSTRUCTION = "LocalAlarmInstruction";
        public const string ColumnVersion = "Version";

        [LwSerializeAttribute()]
        public virtual Guid IdOutput { get; set; }
        public virtual string Name { get; set; }
        [LwSerializeAttribute()]
        public virtual byte OutputNumber { get; set; }
        [LwSerializeAttribute()]
        public virtual byte OutputType { get; set; }
        [LwSerializeAttribute()]
        public virtual bool SettingsForcedToOff { get; set; }
        [LwSerializeAttribute()]
        public virtual int SettingsDelayToOn { get; set; }
        [LwSerializeAttribute()]
        public virtual int SettingsDelayToOff { get; set; }
        [LwSerializeAttribute()]
        public virtual int SettingsPulseLength { get; set; }
        [LwSerializeAttribute()]
        public virtual int SettingsPulseDelay { get; set; }
        [LwSerializeAttribute()]
        public virtual bool Inverted { get; set; }
        public virtual PresentationGroup AlarmPresentationGroup { get; set; }
        public virtual DCU DCU { get; set; }
        private Guid _guidDCU = Guid.Empty;
        [LwSerializeAttribute()]
        public virtual Guid GuidDCU { get { return _guidDCU; } set { _guidDCU = value; } }
        public virtual CCU CCU { get; set; }
        private Guid _guidCCU = Guid.Empty;
        [LwSerializeAttribute()]
        public virtual Guid GuidCCU { get { return _guidCCU; } set { _guidCCU = value; } }
        [LwSerializeAttribute()]
        public virtual byte ControlType { get; set; }
        public virtual string OnOffObjectType { get; set; }
        private ObjectType _onOffObjectObjectType;
        [LwSerializeAttribute()]
        public virtual ObjectType OnOffObjectObjectType { get { return _onOffObjectObjectType; } set { _onOffObjectObjectType = value; } }
        [LwSerializeAttribute()]
        public virtual Guid? OnOffObjectId { get; set; }
        private AOnOffObject _onOffObject = null;
        public virtual AOnOffObject OnOffObject { get { return _onOffObject; } set { _onOffObject = value; } }
        [LwSerializeAttribute()]
        public virtual bool AlarmControlByObjOn { get; set; }
        [LwSerializeAttribute()]
        public virtual bool RealStateChanges { get; set; }
        public virtual string Description { get; set; }
        public virtual byte ObjectType { get; set; }
        public virtual Guid CkUnique { get; set; }
        public virtual bool EnableParentInFullName { get; set; }
        public virtual string LocalAlarmInstruction { get; set; }

        public Output()
        {
            ObjectType = (byte)Cgp.Globals.ObjectType.Output;
            CkUnique = System.Guid.NewGuid();
            EnableParentInFullName = Support.EnableParentInFullName;
        }

        public override string ToString()
        {
            string name = string.Empty;
            if (EnableParentInFullName)
            {
                if (CCU == null)
                {
                    if (DCU != null)
                    {
                        if (DCU.CCU != null)
                            name += DCU.CCU.Name + StringConstants.SLASHWITHSPACES;
                        name += DCU.Name + StringConstants.SLASHWITHSPACES;
                    }
                }
                else
                {
                    name += CCU.Name + StringConstants.SLASHWITHSPACES;
                }
                //name += String.Format("Output{0}", (OutputNumber + 1).ToString("D2"));
            }
            //else
                name += Name;

            return name;
        }

        public override bool Compare(object obj)
        {
            if (obj == null)
                return false;

            if (obj is Output)
            {
                return (obj as Output).IdOutput == IdOutput;
            }
            else
            {
                return false;
            }
        }

        public override bool State
        {
            get { return false; }
        }

        public override object GetId()
        {
            return IdOutput;
        }

        public override IModifyObject CreateModifyObject()
        {
            return new OutputModifyObj(this);
        }

        public override Contal.Cgp.Globals.ObjectType GetObjectType()
        {
            return Contal.Cgp.Globals.ObjectType.Output;
        }

        public virtual void PrepareToSend()
        {
            if (DCU != null)
            {
                GuidDCU = DCU.IdDCU;
            }
            else
            {
                GuidDCU = Guid.Empty;
            }

            if (CCU != null)
            {
                GuidCCU = CCU.IdCCU;
            }
            else
            {
                GuidCCU = Guid.Empty;
            }

            if (OnOffObject != null)
            {
                OnOffObjectObjectType = OnOffObject.GetObjectType();
            }
        }

        public override string GetIdString()
        {
            return IdOutput.ToString();
        }

        public override string GetDeviceShortName()
        {
            return Name;
        }

        #region IComparable Members

        public virtual int CompareTo(object obj)
        {
            if (obj == null) return 1;

            if (obj is Output)
            {
                Output outputCom = obj as Output;
                if (outputCom.CCU != null && this.CCU != null)
                {
                    int i = this.CCU.Name.CompareTo(outputCom.CCU.Name);
                    if (i != 0)
                        return i;
                }
                if (outputCom.DCU != null && this.DCU != null)
                {
                    int i = this.DCU.Name.CompareTo(outputCom.DCU.Name);
                    if (i != 0)
                        return i;
                }
                return this.OutputNumber.CompareTo(outputCom.OutputNumber);
            }
            else
            {
                return 1;
            }
        }

        #endregion

        public virtual string GetLocalAlarmInstruction()
        {
            return LocalAlarmInstruction;
        }

        public virtual DCU GetDcu()
        {
            return DCU;
        }
    }

    [Serializable()]
    public class OutputShort : IShortObject
    {
        public const string COLUMNIDOUTPUT = "IdOutput";
        public const string COLUMNFULLNAME = "FullName";
        public const string COLUMNNAME = "Name";
        public const string COLUMNSTATE = "State";
        public const string COLUMNREALSTATE = "RealState";
        public const string COLUMNREALSTATECHANGES = "RealStateChanges";
        public const string COLUMNSTATUSONOFF = "StatusOnOff";
        public const string COLUMNREALSTATUSONOFF = "RealStatusOnOff";
        public const string COLUMNDESCRIPTION = "Description";
        public const string COLUMN_SYMBOL = "Symbol";

        public Guid IdOutput { get; set; }
        public string FullName { get; set; }
        public string Name { get; set; }
        public OutputState State { get; set; }
        public OutputState RealState { get; set; }
        public bool RealStateChanges { get; set; }
        public string StatusOnOff { get; set; }
        public string RealStatusOnOff { get; set; }
        public string Description { get; set; }
        public Image Symbol { get; set; }

        public OutputShort(Output output)
        {
            IdOutput = output.IdOutput;
            Name = output.Name;
            //output.EnableParentInFullName = true;
            FullName = output.ToString();
            Description = output.Description;
            RealStateChanges = output.RealStateChanges;
        }

        public override string ToString()
        {
            return FullName;
        }

        #region IShortObject Members

        string IShortObject.Name { get { return FullName; } }

        public ObjectType ObjectType { get { return ObjectType.Output; } }

        public string GetSubTypeImageString(object value)
        {
            return string.Empty;
        }

        public object Id { get { return IdOutput; } }

        #endregion
    }

    [Serializable()]
    public class OutputModifyObj : AModifyObject
    {
        public override ObjectType GetOrmObjectType { get { return ObjectType.Output; } }

        public OutputModifyObj(Output output)
        {
            Id = output.IdOutput;
            FullName = output.ToString();
            Description = output.Description;
        }
    }

    public enum OutputControl : byte
    {
        [Name("unblocked")] //neblokovany
        unblocked = 0,
        [Name("manualBlocked")] // manualne blokovany
        manualBlocked = 1,
        [Name("forcedOn")] //vnuteny
        forcedOn = 2,
        [Name("controledByObject")] //riadeny objektom
        controledByObject = 3,
        [Name("controledByDoorEnvironment")] //riadeny dvernym automatom
        controledByDoorEnvironment = 4,
        [Name("watchdog")] //watchdog server connection
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
        [Name("level")]
        level = 0,
        [Name("pulsed")]
        pulsed = 1,
        [Name("frequency")]
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

    public enum OutputState : byte
    {
        Unknown = 0xFF,
        On = 1,
        Off = 0,
        UsedByAnotherAplication = 2,
        OutOfRange = 3,
    }

    public class StateChangedOutputHandler : Contal.IwQuick.Remoting.ARemotingCallbackHandler
    {
        private static volatile StateChangedOutputHandler _singleton = null;
        private static object _syncRoot = new object();

        private Action<Guid, byte, Guid> _stateChanged;

        public static StateChangedOutputHandler Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (null == _singleton)
                            _singleton = new StateChangedOutputHandler();
                    }

                return _singleton;
            }
        }

        public StateChangedOutputHandler()
            : base("StateChangedOutputHandler")
        {
        }

        public void RegisterStateChanged(Action<Guid, byte, Guid> stateChanged)
        {
            _stateChanged += stateChanged;
        }

        public void UnregisterStateChanged(Action<Guid, byte, Guid> stateChanged)
        {
            _stateChanged -= stateChanged;
        }

        public void RunEvent(Guid id, byte State, Guid parent)
        {
            if (_stateChanged != null)
                _stateChanged(id, State, parent);
        }
    }

    public class RealStateChangedOutputHandler : Contal.IwQuick.Remoting.ARemotingCallbackHandler
    {
        private static volatile RealStateChangedOutputHandler _singleton = null;
        private static object _syncRoot = new object();

        private Action<Guid, byte, Guid> _stateChanged;

        public static RealStateChangedOutputHandler Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (null == _singleton)
                            _singleton = new RealStateChangedOutputHandler();
                    }

                return _singleton;
            }
        }

        public RealStateChangedOutputHandler()
            : base("RealStateChangedOutputHandler")
        {
        }

        public void RegisterStateChanged(Action<Guid, byte, Guid> stateChanged)
        {
            _stateChanged += stateChanged;
        }

        public void UnregisterStateChanged(Action<Guid, byte, Guid> stateChanged)
        {
            _stateChanged -= stateChanged;
        }

        public void RunEvent(Guid id, byte State, Guid parent)
        {
            if (_stateChanged != null)
                _stateChanged(id, State, parent);
        }
    }

    public class OutputEditChangedHandler : Contal.IwQuick.Remoting.ARemotingCallbackHandler
    {
        private static volatile OutputEditChangedHandler _singleton = null;
        private static object _syncRoot = new object();

        private Action<Guid, Guid> _outputEditChanged;

        public static OutputEditChangedHandler Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (_singleton == null)
                            _singleton = new OutputEditChangedHandler();
                    }

                return _singleton;
            }
        }

        public OutputEditChangedHandler()
            : base("OutputEditChangedHandler")
        {
        }

        public void RegisterOutputEditChanged(Action<Guid, Guid> outputEditChanged)
        {
            _outputEditChanged += outputEditChanged;
        }

        public void UnregisterOutputEditChanged(Action<Guid, Guid> outputEditChanged)
        {
            _outputEditChanged -= outputEditChanged;
        }

        public void RunEvent(Guid idParentObject, Guid idOutput)
        {
            if (_outputEditChanged != null)
                _outputEditChanged(idParentObject, idOutput);
        }
    }
}
