using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Contal.Cgp.Globals;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.Server.Beans
{
    [Serializable()]
    [LwSerialize(319)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    [LwSerializeNoParent]
    public class Input : AOnOffObject, IOrmObjectWithAlarmInstructions, IComparable, IGetDcu, ISetFullTextSearchString
    {
        public const string COLUMNIDIMPUT = "IdInput";
        public const string COLUMNNAME = "Name";
        public const string COLUMNALTERNATENAME = "AlternateName";
        public const string COLUMNINPUTNUMBER = "InputNumber";
        public const string COLUMNNICKNAME = "NickName";
        public const string COLUMNINPUTTYPE = "InputType";
        public const string COLUMNINVERTED = "Inverted";
        public const string COLUMNHIGHPRIORITY = "HighPriority";
        public const string COLUMNOFFACK = "OffACK";
        public const string COLUMNDELAYTOON = "DelayToOn";
        public const string COLUMNDELAYTOOFF = "DelayToOff";
        public const string COLUMNALARMPRESENTATIONGROUP = "AlarmPresentationGroup";
        public const string COLUMNALARMPGPRESENTATIONSTATEINPENDENTOFALARM = "AlarmPGPresentationStateInpendentOfAlarm";
        public const string COLUMNTAMPERALARMPRESENTATIONGROUP = "TamperAlarmPresentationGroup";
        public const string COLUMNTAMPERDELAYTOON = "TamperDelayToOn";
        public const string COLUMNDCUSDCU = "DCU";
        public const string COLUMNCCU = "CCU";
        public const string COLUMNGUIDDCU = "GuidDCU";
        public const string COLUMNGUIDCCU = "GuidCCU";
        public const string COLUMNDESCRIPTION = "Description";
        public const string COLUMNSTATE = "State";
        public const string COLUMNFILTERALARMON = "AlarmOn";
        public const string COLUMNFILTERALARMTAMPER = "AlarmTamper";
        public const string COLUMNSTATUSONOFF = "StatusOnOff";
        public const string COLUMNSTATUSACTIVATED = "StatusActivated";
        public const string COLUMNALAAINPUTS = "AAInputs";
        public const string COLUMNOBJECTTYPE = "ObjectType";
        public const string COLUMNCKUNIQUE = "CkUnique";
        public const string COLUMNENABLEPARENTINFULLNAME = "EnableParentInFullName";
        public const string COLUMNBLOCKINGTYPE = "BlockingType";
        public const string COLUMNONOFFOBJECTTYPE = "OnOffObjectType";
        public const string COLUMNONOFFOBJECTOBJECTTYPE = "OnOffObjectObjectType";
        public const string COLUMNONOFFOBJECTID = "OnOffObjectId";
        public const string COLUMNONOFFOBJECT = "OnOffObject";
        public const string COLUMNISSETPRESENTATIONGROUP = "IsSetPresentationGroup";
        public const string COLUMNLOCALALARMINSTRUCTION = "LocalAlarmInstruction";
        public const string COLUMN_ALTERNATE_NAME = "AlternateName";
        public const string COLUMN_FULLNAME = "FullName";
        public const string COLUMN_FULL_TEXT_SEARCH_STRING = "FullTextSearchString";
        public const string COLUMN_OTHER_FULL_TEXT_SEARCH_STRINGS = "OtherFullTextSearchStrings";
        public const string ColumnVersion = "Version";

        [LwSerializeAttribute()]
        public virtual Guid IdInput { get; set; }
        public virtual string Name { get; set; }
        public virtual string AlternateName { get; set; }
        [LwSerializeAttribute()]
        public virtual byte InputNumber { get; set; }
        [LwSerializeAttribute()]
        public virtual string NickName { get; set; }
        [LwSerializeAttribute()]
        public virtual byte InputType { get; set; }
        [LwSerializeAttribute()]
        public virtual bool Inverted { get; set; }
        public virtual bool HighPriority { get; set; }
        [LwSerializeAttribute()]
        public virtual bool OffACK { get; set; }
        [LwSerializeAttribute()]
        public virtual int DelayToOn { get; set; }
        [LwSerializeAttribute()]
        public virtual int DelayToOff { get; set; }
        public virtual PresentationGroup AlarmPresentationGroup { get; set; }
        public virtual bool AlarmPGPresentationStateInpendentOfAlarm { get; set; }
        public virtual PresentationGroup TamperAlarmPresentationGroup { get; set; }
        [LwSerializeAttribute()]
        public virtual int TamperDelayToOn { get; set; }
        public virtual DCU DCU { get; set; }
        private Guid _guidDCU = Guid.Empty;
        [LwSerializeAttribute()]
        public virtual Guid GuidDCU { get { return _guidDCU; } set { _guidDCU = value; } }
        public virtual CCU CCU { get; set; }
        private Guid _guidCCU = Guid.Empty;
        [LwSerializeAttribute()]
        public virtual Guid GuidCCU { get { return _guidCCU; } set { _guidCCU = value; } }

        [LwSerializeAttribute()]
        public virtual bool AlarmOn { get; set; }
        [LwSerializeAttribute()]
        public virtual bool AlarmTamper { get; set; }

        public virtual string Description { get; set; }
        public virtual ICollection<AAInput> AAInputs { get; set; }
        private InputState _state;
        public override bool State { get { return !(_state == InputState.Normal); } }
        public virtual byte ObjectType { get; set; }
        public virtual Guid CkUnique { get; set; }
        public virtual bool EnableParentInFullName { get; set; }
        [LwSerializeAttribute()]
        public virtual byte BlockingType { get; set; }
        public virtual string OnOffObjectType { get; set; }
        private ObjectType _onOffObjectObjectType;
        [LwSerializeAttribute()]
        public virtual ObjectType OnOffObjectObjectType { get { return _onOffObjectObjectType; } set { _onOffObjectObjectType = value; } }
        [LwSerializeAttribute()]
        public virtual Guid? OnOffObjectId { get; set; }
        private AOnOffObject _onOffObject = null;
        public virtual AOnOffObject OnOffObject { get { return _onOffObject; } set { _onOffObject = value; } }
        public virtual string LocalAlarmInstruction { get; set; }

        public Input()
        {
            ObjectType = (byte)Cgp.Globals.ObjectType.Input;
            CkUnique = Guid.NewGuid();
            EnableParentInFullName = Support.EnableParentInFullName;
        }

        public virtual void ActualizeAlternateName()
        {
            AlternateName = GetSectionIdsString();
        }

        public virtual string GetSectionIdsString()
        {
            if (AAInputs == null
                || AAInputs.Count == 0)
                return null;

            var sectionIdsArray = AAInputs.Select(obj => obj.SectionId).ToArray();
            string sectionIdsStr = string.Empty;

            for (int i = 0; i < sectionIdsArray.Length; i++)
            {
                sectionIdsStr += sectionIdsArray[i];

                if (i + 1 < sectionIdsArray.Length)
                    sectionIdsStr += ",";
            }

            return sectionIdsStr;
        }

        public virtual string FullName
        {
            get
            {
                string fullName = string.Empty;

                if (EnableParentInFullName)
                {
                    if (CCU == null)
                    {
                        if (DCU != null)
                        {
                            if (DCU.CCU != null)
                                fullName += DCU.CCU.Name + StringConstants.SLASHWITHSPACES;
                            fullName += DCU.Name + StringConstants.SLASHWITHSPACES;
                        }
                    }
                    else
                    {
                        fullName += CCU.Name + StringConstants.SLASHWITHSPACES;
                    }
                }

                fullName += Name;

                return fullName;
            }
        }

        public override string ToString()
        {
            return string.IsNullOrEmpty(AlternateName)
                ? FullName
                : string.Format("{0} - {1}", AlternateName, FullName);
        }

        public override bool Compare(object obj)
        {
            if (obj == null)
                return false;

            if (obj is Input)
            {
                return (obj as Input).IdInput == IdInput;
            }

            return false;
        }

        public override object GetId()
        {
            return IdInput;
        }

        public override IModifyObject CreateModifyObject()
        {
            return new InputModifyObj(this);
        }

        public override Contal.Cgp.Globals.ObjectType GetObjectType()
        {
            return Contal.Cgp.Globals.ObjectType.Input;
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

        public virtual void SetState(InputState inputState)
        {
            _state = inputState;
        }

        public override string GetIdString()
        {
            return IdInput.ToString();
        }

        public override string GetDeviceShortName()
        {
            return Name;
        }

        #region IComparable Members

        public virtual int CompareTo(object obj)
        {
            if (obj == null) return 1;

            if (obj is Input)
            {
                Input inputCom = obj as Input;

                if (inputCom.CCU != null && this.CCU != null)
                {
                    int i = this.CCU.Name.CompareTo(inputCom.CCU.Name);
                    if (i != 0)
                        return i;
                }

                if (inputCom.DCU != null && this.DCU != null)
                {
                    int i = this.DCU.Name.CompareTo(inputCom.DCU.Name);
                    if (i != 0)
                        return i;
                }

                return this.InputNumber.CompareTo(inputCom.InputNumber);
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

        //#region IComparable<Input> Members

        //public virtual int CompareTo(Input other)
        //{
        //    if (other.CCU != null && this.CCU != null)
        //    {
        //        int i = other.CCU.Name.CompareTo(this.CCU.Name);
        //        if (i != 0)
        //            return i;
        //    }

        //    if (other.DCU != null && this.DCU != null)
        //    {
        //        int i = other.DCU.Name.CompareTo(this.DCU.Name);
        //        if (i != 0)
        //            return i;
        //    }

        //    return other.InputNumber.CompareTo(this.InputNumber);
        //}

        //#endregion

        #region ISetFullTextSearchString Members

        public virtual string FullTextSearchString { get; set; }

        public virtual IEnumerable<string> OtherFullTextSearchStrings
        {
            get
            {
                return Enumerable.Repeat(
                    ToString(),
                    1)
                    .Concat(
                        Enumerable.Repeat(
                            NickName,
                            1));
            }
        }

        #endregion
    }

    [Serializable()]
    public class InputShort : IShortObject
    {
        public const string COLUMNIDIMPUT = "IdInput";
        public const string COLUMNSECTIONIDS = "SectionIds";
        public const string COLUMNFULLNAME = "FullName";
        public const string COLUMNNAME = "Name";
        public const string COLUMNNICKNAME = "NickName";
        public const string COLUMNSTATE = "State";
        public const string COLUMNSTATUSONOFF = "StatusOnOff";
        public const string COLUMNDESCRIPTION = "Description";
        public const string COLUMN_SYMBOL = "Symbol";

        public Guid IdInput { get; set; }
        public string SectionIds { get; set; }
        public string FullName { get; set; }
        public string Name { get; set; }
        public string NickName { get; set; }
        public InputState State { get; set; }
        public string StatusOnOff { get; set; }
        public string Description { get; set; }
        public Image Symbol { get; set; }

        public InputShort(Input input)
        {
            IdInput = input.IdInput;
            SectionIds = input.GetSectionIdsString();
            FullName = input.FullName;
            Name = input.FullName;
            NickName = input.NickName;
            //input.EnableParentInFullName = true;
            
            Description = input.Description;
        }

        public override string ToString()
        {
            return FullName;
        }

        #region IShortObject Members

        string IShortObject.Name { get { return FullName; } }

        public ObjectType ObjectType { get { return ObjectType.Input; } }

        public string GetSubTypeImageString(object value)
        {
            return string.Empty;
        }

        public object Id { get { return IdInput; } }

        #endregion
    }

    [Serializable()]
    public class InputModifyObj : AModifyObject
    {
        public override ObjectType GetOrmObjectType { get { return ObjectType.Input; } }

        public InputModifyObj(Input input)
        {
            Id = input.IdInput;           
            FullName = input.ToString();
            Description = input.Description;
        }
    }

    public enum InputState : byte
    {
        Normal = 0,
        Alarm = 1,
        Short = 2,
        Break = 3,
        UsedByAnotherAplication = 4,
        OutOfRange = 5,
        Unknown = 0xFF
    }

    /*
    public enum InputType : byte
    {
        [Name("DI")]
        DI = 0,
        [Name("BSI")]
        BSI = 1
    }

    public class InputTypeStates
    {
        private InputType _value;
        public InputType Value
        {
            get { return _value; }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
        }

        public InputTypeStates(InputType value, string name)
        {
            _value = value;
            _name = name;
        }

        public override string ToString()
        {
            return Name;
        }

        public static IList<InputTypeStates> GetInputStatesList(Contal.IwQuick.Localization.LocalizationHelper localizationHelper)
        {
            IList<InputTypeStates> list = new List<InputTypeStates>();
            FieldInfo[] fieldsInfo = typeof(InputType).GetFields();
            foreach (FieldInfo fieldInfo in fieldsInfo)
            {
                NameAttribute[] attribs =
                    fieldInfo.GetCustomAttributes(typeof(NameAttribute), false) as NameAttribute[];

                if (attribs.Length > 0)
                {
                    list.Add(new InputTypeStates((InputType)fieldInfo.GetValue(fieldInfo), localizationHelper.GetString("InputTypeStates_" + attribs[0].Name)));
                }
            }

            return list;
        }

        public static InputTypeStates GetInputTypeState(Contal.IwQuick.Localization.LocalizationHelper localizationHelper, IList<InputTypeStates> listInputTypeStates, byte inputTypeState)
        {
            if (listInputTypeStates == null)
            {
                return GetInputTypeState(localizationHelper, inputTypeState);
            }
            else
            {
                foreach (InputTypeStates listInputTypeState in listInputTypeStates)
                {
                    if ((byte)listInputTypeState.Value == inputTypeState)
                        return listInputTypeState;
                }
            }

            return null;
        }

        public static InputTypeStates GetInputTypeState(Contal.IwQuick.Localization.LocalizationHelper localizationHelper, byte inputTypeState)
        {
            FieldInfo[] fieldsInfo = typeof(InputType).GetFields();
            foreach (FieldInfo fieldInfo in fieldsInfo)
            {
                NameAttribute[] attribs = fieldInfo.GetCustomAttributes(
                typeof(NameAttribute), false) as NameAttribute[];

                if (attribs.Length > 0)
                {
                    if ((byte)fieldInfo.GetValue(fieldInfo) == inputTypeState)
                        return (new InputTypeStates((InputType)fieldInfo.GetValue(fieldInfo), localizationHelper.GetString("InputTypeStates_" + attribs[0].Name)));
                }
            }

            return null;
        }
    }*/

    public class StateChangedInputHandler : Contal.IwQuick.Remoting.ARemotingCallbackHandler
    {
        private static volatile StateChangedInputHandler _singleton = null;
        private static object _syncRoot = new object();

        private Action<Guid, byte, Guid> _stateChanged;

        public static StateChangedInputHandler Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (null == _singleton)
                            _singleton = new StateChangedInputHandler();
                    }

                return _singleton;
            }
        }

        public StateChangedInputHandler()
            : base("StateChangedInputHandler")
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

    public class InputEditChangedHandler : Contal.IwQuick.Remoting.ARemotingCallbackHandler
    {
        private static volatile InputEditChangedHandler _singleton = null;
        private static object _syncRoot = new object();

        private Action<Guid, Guid> _inputEditChanged;

        public static InputEditChangedHandler Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (_singleton == null)
                            _singleton = new InputEditChangedHandler();
                    }

                return _singleton;
            }
        }

        public InputEditChangedHandler()
            : base("InputEditChangedHandler")
        {
        }

        public void RegisterInputEditChanged(Action<Guid, Guid> inputEditChanged)
        {
            _inputEditChanged += inputEditChanged;
        }

        public void UnregisterInputEditChanged(Action<Guid, Guid> inputEditChanged)
        {
            _inputEditChanged -= inputEditChanged;
        }

        public void RunEvent(Guid idParentObject, Guid idInput)
        {
            if (_inputEditChanged != null)
                _inputEditChanged(idParentObject, idInput);
        }
    }
}