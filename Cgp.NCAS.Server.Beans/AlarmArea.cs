using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;

using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Globals;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick.Data;
using Contal.IwQuick;
using Contal.IwQuick.Localization;
using Contal.IwQuick.Remoting;

namespace Contal.Cgp.NCAS.Server.Beans
{
    public enum AlarmAreaAlarmState : byte
    {
        Normal = 0,
        Alarm = 1,
        Unknown = 0xFF
    }

    public class AlarmAreaAlarmStates
    {
        private readonly AlarmAreaAlarmState _value;
        public AlarmAreaAlarmState Value
        {
            get { return _value; }
        }

        private readonly string _name;
        public string Name
        {
            get { return _name; }
        }

        public AlarmAreaAlarmStates(AlarmAreaAlarmState value, string name)
        {
            _value = value;
            _name = name;
        }

        public override string ToString()
        {
            return Name;
        }

        public static IList<AlarmAreaAlarmStates> GetActualSatesList(LocalizationHelper localizationHelper)
        {
            return EnumHelper.ListAllValuesWithProcessing<AlarmAreaAlarmState, AlarmAreaAlarmStates>(
                value => new AlarmAreaAlarmStates(
                    value, 
                    localizationHelper.GetString("AlarmAreaActualStates_" + value))
                );
        }

        public static AlarmAreaAlarmStates GetActualStates(LocalizationHelper localizationHelper, byte state)
        {
            return new AlarmAreaAlarmStates(
                (AlarmAreaAlarmState)state, 
                localizationHelper.GetString("AlarmAreaActualStates_"  + ((AlarmAreaAlarmState)state).ToString()));
        }
    }

    public enum RequestActivationState : byte
    {
        Set = 0,
        UnconditionalSet = 1,
        Unset = 2,
        Unknown = 0xFF
    }

    public enum ActivationState : byte
    {
        Set = 0,
        Unset = 1,
        Prewarning = 2,
        TemporaryUnsetExit = 3,
        TemporaryUnsetEntry = 4,
        UnsetBoughtTime = 5,
        Unknown = 0xFF
    }

    public class AlarmAreaActivationStates
    {
        private readonly ActivationState _value;
        public ActivationState Value
        {
            get { return _value; }
        }

        private readonly string _name;
        public string Name
        {
            get { return _name; }
        }

        public AlarmAreaActivationStates(ActivationState value, string name)
        {
            _value = value;
            _name = name;
        }

        public override string ToString()
        {
            return Name;
        }

        public static IList<AlarmAreaActivationStates> GetActivationSatesList(LocalizationHelper localizationHelper)
        {
            return EnumHelper.ListAllValuesWithProcessing<ActivationState, AlarmAreaActivationStates>(value => 
                new AlarmAreaActivationStates(
                    value, 
                    localizationHelper.GetString("AlarmAreaActivationStates_" + value.ToString())
                ));
        }

        public static AlarmAreaActivationStates GetActivationStates(LocalizationHelper localizationHelper, byte state)
        {
            var value = (ActivationState)state;
            return new AlarmAreaActivationStates(value, localizationHelper.GetString("AlarmAreaActivationStates_" + value.ToString()));
        }
    }

    [Serializable]
    [LwSerialize(307)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    [LwSerializeNoParent]
    public class AlarmArea :
        AOrmObjectWithVersion,
        IOrmObjectWithAlarmInstructions,
        ICardReaderObject,
        ISetFullTextSearchString
    {
        public const string COLUMNIDALARMAREA = "IdAlarmArea";
        public const string COLUMNID = "Id";
        public const string COLUMNNAME = "Name";
        public const string COLUMNSHORTNAME = "ShortName";
        public const string COLUMN_PURPOSE = "Purpose";
        public const string COLUMNAAINPUTS = "AAInputs";
        public const string COLUMNGUIDAAINPUTS = "GuidAAInputs";
        public const string COLUMNAACARDREADERS = "AACardReaders";
        public const string COLUMNOBJFORAUTOMATICACTTYPE = "ObjForAutomaticActType";
        public const string COLUMNOBJFORAUTOMATICACTOBJECTTYPE = "ObjForAutomaticActObjectType";
        public const string COLUMNOBJFORAUTOMATICACTID = "ObjForAutomaticActId";
        public const string COLUMNOBJFORAUTOMATICACT = "ObjForAutomaticAct";
        public const string COLUMNAUTOMATICDEACTIVE = "AutomaticDeactive";
        public const string COLUMNISINVERTEDOBJFORAUTOMATICACT = "IsInvertedObjForAutomaticAct";
        public const string COLUMNPREALARM = "PreAlarm";
        public const string COLUMNPREALARMDURATION = "PreAlarmDuration";
        public const string COLUMNTEMPORARYUNSETDURATION = "TemporaryUnsetDuration";
        public const string COLUMNPREWARNING = "PreWarning";
        public const string COLUMNPREWARNINGDURATION = "PreWarningDuration";
        public const string COLUMNTIMEBUYINGENABLED = "TimeBuyingEnabled";
        public const string COLUMNMAXTIMEBUYINGDURATION = "TimeBuyingMaxDuration";
        public const string COLUMNMAXTOTALTIMEBUYING = "TimeBuyingTotalMax";
        public const string COLUMNPRESENTATIONGROUP = "PresentationGroup";
        public const string COLUMNGUIDPRESENTATIONGROUP = "GuidPresentationGroup";
        public const string COLUMNACKNOWLEDGEOFF = "AcknowledgeOFF";
        public const string COLUMNDESCRIPTION = "Description";
        public const string COLUMNOUTPUTACTIVATION = "OutputActivation";
        public const string COLUMNGUIDOUTPUTACTIVATION = "GuidOutputActivation";
        public const string COLUMNOUTPUTALARMSTATE = "OutputAlarmState";
        public const string COLUMNGUIDOUTPUTALARMSTATE = "GuidOutputAlarmState";
        public const string COLUMNOUTPUTPREWARNING = "OutputPrewarning";
        public const string COLUMNGUIDOUTPUTPREWARNING = "GuidOutputPrewarning";
        public const string COLUMNOUTPUTTMPUNSETENTRY = "OutputTmpUnsetEntry";
        public const string COLUMNGUIDOUTPUTTMPUNSETENTRY = "GuidOutputTmpUnsetEntry";
        public const string COLUMNOUTPUTTMPUNSETEXIT = "OutputTmpUnsetExit";
        public const string COLUMNGUIDOUTPUTTMPUNSETEXIT = "GuidOutputTmpUnsetExit";
        public const string COLUMNABALARMHANDLING = "ABAlarmHandling";
        public const string COLUMNPERCENTAGESENSORSTOAALARM = "PercentageSensorsToAAlarm";
        public const string COLUMNOUTPUTAALARM = "OutputAAlarm";
        public const string COLUMNGUIDOUTPUTAALARM = "GuidOutputAAlarm";
        public const string COLUMNOBJECTTYPE = "ObjectType";
        public const string COLUMNALLOWSENDSTATETOCRS = "AllowSendingStateToCRs";
        public const string COLUMNLOCALALARMINSTRUCTION = "LocalAlarmInstruction";
        public const string COLUMN_OUTPUT_SIREN = "OutputSiren";
        public const string COLUMN_GUID_OUTPUT_SIREN = "GuidOutputSiren";
        public const string COLUMN_SIREN_MAX_ON_PERIOD = "SirenMaxOnPeriod";
        public const string COLUMN_OUTPUT_SABOTAGE = "OutputSabotage";
        public const string COLUMN_GUID_OUTPUT_SABOTAGE = "GuidOutputSabotage";
        public const string COLUMN_OUTPUT_NOT_ACKNOWLEDGED = "OutputNotAcknowledged";
        public const string COLUMN_GUID_OUTPUT_NOT_ACKNOWLEDGED = "GuidOutputNotAcknowledged";
        public const string COLUMN_ALARM_AREA_ALARM_ARCS = "AlarmAreaAlarmArcs";
        public const string COLUMN_ALARM_TYPE_AND_ID_ALARM_ARCS = "AlarmTypeAndIdAlarmArcs";
        public const string COLUMN_ALARM_AREA_SET_BY_ON_OFF_OBJECT_FAILED_PRESENTATION_GROUP = "AlarmAreaSetByOnOffObjectFailedPresentationGroup";
        public const string COLUMN_SENSOR_ALARM_PRESENTATION_GROUP = "SensorAlarmPresentationGroup";
        public const string COLUMN_SENSOR_TAMPER_ALARM_PRESENTATION_GROUP = "SensorTamperAlarmPresentationGroup";
        public const string COLUMN_OUTPUT_SET_BY_OBJECT_FOR_AA_FAILED = "OutputSetByObjectForAaFailed";
        public const string COLUMN_ID_OUTPUT_SET_BY_OBJECT_FOR_AA_FAILED = "IdOutputSetByObjectForAaFailed";
        public const string COLUMN_OUTPUT_SET_NOT_CALM_AA_BY_OBJECT_FOR_AA_ON_PERIOD = "OutputSetNotCalmAaByObjectForAaOnPeriod";
        public const string COLUMN_VERSION = "Version";

        [LwSerialize]
        public virtual Guid IdAlarmArea { get; set; }
        [LwSerialize]
        public virtual int Id { get; set; }

        public virtual string SectionId
        {
            get { return Id.ToString("D2"); }
        }

        [LwSerialize]
        public virtual string Name { get; set; }
        
        #region ISetFullTextSearchString Members

        public virtual string FullTextSearchString { set; get; }

        public virtual string AlternateName 
        {
            get { return ShortName; }
        }

        public virtual IEnumerable<string> OtherFullTextSearchStrings
        {
            get
            {
                return Enumerable.Repeat(
                    ToString(),
                    1);
            }
        }

        #endregion

        [LwSerialize]
        public virtual string ShortName { get; set; }
        
        [LwSerialize]
        public virtual SensorPurpose Purpose { get; set; }

        public virtual ICollection<AAInput> AAInputs { get; set; }
        private List<Guid> _guidAAInputs = new List<Guid>();
        [LwSerialize]
        public virtual List<Guid> GuidAAInputs { get { return _guidAAInputs; } set { _guidAAInputs = value; } }
        public virtual ICollection<AACardReader> AACardReaders { get; set; }

        #region Object for automatic activation

        public virtual string ObjForAutomaticActType { get; set; }

        private ObjectType _objForAutomaticActObjectType;

        [LwSerialize]
        public virtual ObjectType ObjForAutomaticActObjectType
        {
            get { return _objForAutomaticActObjectType; } 
            set { _objForAutomaticActObjectType = value; }
        }

        [LwSerialize]
        public virtual Guid? ObjForAutomaticActId { get; set; }

        private AOnOffObject _objForAutomaticAct;

        public virtual AOnOffObject ObjForAutomaticAct { get { return _objForAutomaticAct; } set { _objForAutomaticAct = value; } }
        [LwSerialize]
        public virtual bool AutomaticDeactive { get; set; }

        [LwSerialize]
        public virtual bool IsInvertedObjForAutomaticAct { get; set; }

        #endregion

        #region Object for forced time buying

        public virtual string ObjForForcedTimeBuyingType { get; set; }

        private ObjectType _objForForcedTimeBuyingObjectType;

        [LwSerialize]
        public virtual ObjectType ObjForForcedTimeBuyingObjectType
        {
            get { return _objForForcedTimeBuyingObjectType; } 
            set { _objForForcedTimeBuyingObjectType = value; }
        }

        [LwSerialize]
        public virtual Guid? ObjForForcedTimeBuyingId { get; set; }

        private AOnOffObject _objForForcedTimeBuying;

        public virtual AOnOffObject ObjForForcedTimeBuying
        {
            get { return _objForForcedTimeBuying; } 
            set { _objForForcedTimeBuying = value; }
        }
        [LwSerialize]
        public virtual bool IsInvertedObjForForcedTimeBuying { get; set; }

        [LwSerialize]
        public virtual bool NotForcedTimeBuyingProvideOnlyUnset { get; set; }

        #endregion

        [LwSerialize]
        public virtual bool PreAlarm { get; set; }
        [LwSerialize]
        public virtual int? PreAlarmDuration { get; set; }
        [LwSerialize]
        public virtual int TemporaryUnsetDuration { get; set; }
        [LwSerialize]
        public virtual bool PreWarning { get; set; }
        [LwSerialize]
        public virtual int? PreWarningDuration { get; set; }
        [LwSerialize]
        public virtual bool TimeBuyingEnabled { get; set; }
        [LwSerialize]
        public virtual int? TimeBuyingMaxDuration { get; set; }
        [LwSerialize]
        public virtual int? TimeBuyingTotalMax { get; set; }
        [LwSerialize]
        public virtual bool TimeBuyingOnlyInPrewarning { get; set; }
        public virtual PresentationGroup PresentationGroup { get; set; }
        [LwSerialize]
        public virtual bool AcknowledgeOFF { get; set; }
        [LwSerialize]
        public virtual bool ABAlarmHandling { get; set; }
        [LwSerialize]
        public virtual int PercentageSensorsToAAlarm { get; set; }
        public virtual string Description { get; set; }

        public virtual Output OutputActivation { get; set; }
        private Guid _guidOutputActivation = Guid.Empty;
        [LwSerialize]
        public virtual Guid GuidOutputActivation { get { return _guidOutputActivation; } set { _guidOutputActivation = value; } }

        public virtual Output OutputAlarmState { get; set; }
        private Guid _guidOutputAlarmState = Guid.Empty;
        [LwSerialize]
        public virtual Guid GuidOutputAlarmState { get { return _guidOutputAlarmState; } set { _guidOutputAlarmState = value; } }

        public virtual Output OutputPrewarning { get; set; }
        private Guid _guidOutputPrewarning = Guid.Empty;
        [LwSerialize]
        public virtual Guid GuidOutputPrewarning { get { return _guidOutputPrewarning; } set { _guidOutputPrewarning = value; } }

        public virtual Output OutputTmpUnsetEntry { get; set; }
        private Guid _guidOutputTmpUnsetEntry = Guid.Empty;
        [LwSerialize]
        public virtual Guid GuidOutputTmpUnsetEntry { get { return _guidOutputTmpUnsetEntry; } set { _guidOutputTmpUnsetEntry = value; } }

        public virtual Output OutputTmpUnsetExit { get; set; }
        private Guid _guidOutputTmpUnsetExit = Guid.Empty;
        [LwSerialize]
        public virtual Guid GuidOutputTmpUnsetExit { get { return _guidOutputTmpUnsetExit; } set { _guidOutputTmpUnsetExit = value; } }

        public virtual Output OutputAAlarm { get; set; }
        private Guid _guidOutputAAlarm = Guid.Empty;
        [LwSerialize]
        public virtual Guid GuidOutputAAlarm { get { return _guidOutputAAlarm; } set { _guidOutputAAlarm = value; } }

        public virtual Output OutputSiren { get; set; }
        private Guid _guidOutputSiren = Guid.Empty;
        [LwSerialize]
        public virtual Guid GuidOutputSiren { get { return _guidOutputSiren; } set { _guidOutputSiren = value; } }

        [LwSerialize]
        public virtual int? SirenMaxOnPeriod { get; set; }

        public virtual Output OutputSabotage { get; set; }
        private Guid _guidOutputSabotage = Guid.Empty;
        [LwSerialize]
        public virtual Guid GuidOutputSabotage { get { return _guidOutputSabotage; } set { _guidOutputSabotage = value; } }

        public virtual Output OutputNotAcknowledged { get; set; }
        private Guid _guidOutputNotAcknowledged = Guid.Empty;
        [LwSerialize]
        public virtual Guid GuidOutputNotAcknowledged { get { return _guidOutputNotAcknowledged; } set { _guidOutputNotAcknowledged = value; } }

        public virtual Output OutputMotion { get; set; }
        private Guid _guidOutpuMotion = Guid.Empty;
        [LwSerialize]
        public virtual Guid GuidOutputMotion { get { return _guidOutpuMotion; } set { _guidOutpuMotion = value; } }

        public virtual Output OutputSetByObjectForAaFailed { get; set; }
        private Guid _idOutputSetByObjectForAaFailed = Guid.Empty;

        [LwSerialize]
        public virtual Guid IdOutputSetByObjectForAaFailed
        {
            get { return _idOutputSetByObjectForAaFailed; }
            set { _idOutputSetByObjectForAaFailed = value; }
        }

        [LwSerialize]
        public virtual int OutputSetNotCalmAaByObjectForAaOnPeriod { get; set; }

        public virtual byte ObjectType { get; set; }
        [LwSerialize]
        public virtual bool? AllowSendingStateToCRs { get; set; }

        [LwSerialize]
        public virtual bool UseEIS { get; set; }
        [LwSerialize]
        public virtual int? FilterTimeEIS { get; set; }
        [LwSerialize]
        public virtual Guid? ActivationStateInputEIS { get; set; }
        [LwSerialize]
        public virtual Guid? SetUnsetOutputEIS { get; set; }

        public virtual string LocalAlarmInstruction { get; set; }

        [LwSerialize]
        public virtual bool EnableEventlogsInCR { get; set; }
        
        [LwSerialize]
        public virtual bool AaSet { get; set; }
        [LwSerialize]
        public virtual bool AaUnset { get; set; }
        [LwSerialize]
        public virtual bool AaAlarm { get; set; }
        [LwSerialize]
        public virtual bool AaNormal { get; set; }
        [LwSerialize]
        public virtual bool AaAcknowledged { get; set; }
        [LwSerialize]
        public virtual bool AaUnconditionalSet { get; set; }

        [LwSerialize]
        public virtual bool SensorAlarm { get; set; }
        [LwSerialize]
        public virtual bool SensorNormal { get; set; }
        [LwSerialize]
        public virtual bool SensorAcknowledged { get; set; }
        [LwSerialize]
        public virtual bool SensorUnblocked { get; set; }
        [LwSerialize]
        public virtual bool SensorTemporarilyBlocked { get; set; }
        [LwSerialize]
        public virtual bool SensorPermanentlyBlocked { get; set; }

        public virtual ICollection<AlarmAreaAlarmArc> AlarmAreaAlarmArcs { get; set; }
        [LwSerialize]
        public virtual List<AlarmTypeAndIdAlarmArc> AlarmTypeAndIdAlarmArcs { get; set; }

        [LwSerialize]
        public virtual AlarmAreaAutomaticActivationMode AutomaticActivationMode { get; set; }
        [LwSerialize]
        public virtual bool? AlarmAreaSetByOnOffObjectFailed { get; set; }
        [LwSerialize]
        public virtual bool? BlockAlarmAreaSetByOnOffObjectFailed { get; set; }
        [LwSerialize]
        public virtual byte? ObjBlockAlarmAreaSetByOnOffObjectFailedObjectType { get; set; }
        [LwSerialize]
        public virtual Guid? ObjBlockAlarmAreaSetByOnOffObjectFailedId { get; set; }
        [LwSerialize]
        public virtual bool AlwaysProvideUnsetForTimeBuying { get; set; }

        public virtual PresentationGroup AlarmAreaSetByOnOffObjectFailedPresentationGroup { get; set; }

        public virtual PresentationGroup SensorAlarmPresentationGroup { get; set; }
        public virtual PresentationGroup SensorTamperAlarmPresentationGroup { get; set; }

        public AlarmArea()
        {
            ObjectType = (byte)Cgp.Globals.ObjectType.AlarmArea;
        }

        public override bool Compare(object obj)
        {
            if (obj == null)
                return false;

            if (obj is AlarmArea)
            {
                return (obj as AlarmArea).IdAlarmArea == IdAlarmArea;
            }
            return false;
        }

        public override string ToString()
        {
            return string.Format("{0:00} - {1}", SectionId, Name);
        }

        public virtual void PrepareToSend()
        {
            GuidAAInputs.Clear();

            if (AAInputs != null)
            {
                foreach (AAInput aaInput in AAInputs)
                {
                    GuidAAInputs.Add(aaInput.IdAAInput);
                }
            }

            if (ObjForAutomaticAct != null)
            {
                ObjForAutomaticActObjectType = ObjForAutomaticAct.GetObjectType();
            }

            if (ObjForForcedTimeBuying != null)
            {
                ObjForForcedTimeBuyingObjectType = ObjForForcedTimeBuying.GetObjectType();
            }

            GuidOutputActivation = OutputActivation != null ? OutputActivation.IdOutput : Guid.Empty;
            GuidOutputAlarmState = OutputAlarmState != null ? OutputAlarmState.IdOutput : Guid.Empty;
            GuidOutputPrewarning = OutputPrewarning != null ? OutputPrewarning.IdOutput : Guid.Empty;
            GuidOutputTmpUnsetEntry = OutputTmpUnsetEntry != null ? OutputTmpUnsetEntry.IdOutput : Guid.Empty;
            GuidOutputTmpUnsetExit = OutputTmpUnsetExit != null ? OutputTmpUnsetExit.IdOutput : Guid.Empty;
            GuidOutputAAlarm = OutputAAlarm != null ? OutputAAlarm.IdOutput : Guid.Empty;
            GuidOutputSiren = OutputSiren != null ? OutputSiren.IdOutput : Guid.Empty;
            GuidOutputSabotage = OutputSabotage != null ? OutputSabotage.IdOutput : Guid.Empty;
            GuidOutputNotAcknowledged = OutputNotAcknowledged != null ? OutputNotAcknowledged.IdOutput : Guid.Empty;
            GuidOutputMotion = OutputMotion != null ? OutputMotion.IdOutput : Guid.Empty;
            
            IdOutputSetByObjectForAaFailed = OutputSetByObjectForAaFailed != null
                ? OutputSetByObjectForAaFailed.IdOutput
                : Guid.Empty;

            AlarmTypeAndIdAlarmArcs = AlarmAreaAlarmArcs == null || AlarmAreaAlarmArcs.Count == 0
                ? null
                : new List<AlarmTypeAndIdAlarmArc>(
                    AlarmAreaAlarmArcs.Select(
                        alarmAreaAlarmArc =>
                            new AlarmTypeAndIdAlarmArc(
                                (AlarmType) alarmAreaAlarmArc.AlarmType,
                                alarmAreaAlarmArc.IdAlarmArc)));
        }

        public override string GetIdString()
        {
            return IdAlarmArea.ToString();
        }

        public override object GetId()
        {
            return IdAlarmArea;
        }

        public override IModifyObject CreateModifyObject()
        {
            return new AlarmAreaModifyObj(this);
        }

        public virtual IEnumerable<ICardReaderObject> GetChildObjects()
        {
            if (AACardReaders == null)
                yield break;

            foreach (var aaCardReader in AACardReaders)
            {
                yield return aaCardReader.CardReader;
            }
        }

        public override ObjectType GetObjectType()
        {
            return Cgp.Globals.ObjectType.AlarmArea;
        }

        public virtual string GetLocalAlarmInstruction()
        {
            return LocalAlarmInstruction;
        }
    }

    [Serializable]
    public class AlarmAreaShort : IShortObject
    {
        public const string COLUMN_ID_ALARM_AREA = "IdAlarmArea";
        public const string COLUMN_SECTION_ID = "SectionId";
        public const string COLUMN_NAME = "Name";
        public const string COLUMN_SHORT_NAME = "ShortName";
        public const string COLUMN_ALARM_STATE = "AlarmState";
        public const string COLUMN_STRING_ALARM_STATE = "StringAlarmState";
        public const string COLUMN_ACTIVATION_STATE = "ActivationState";
        public const string COLUMN_STRING_ACTIVATION_STATE = "StringActivationState";
        public const string COLUMN_REQUEST_ACTIVATION_STATE = "RequestActivationState";
        public const string COLUMN_STRING_REQUEST_ACTIVATION_STATE = "StringRequestActivationState";
        public const string COLUMN_DESCRIPTION = "Description";
        public const string COLUMN_SYMBOL = "Symbol";

        public Guid IdAlarmArea { get; set; }
        public string SectionId { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public AlarmAreaAlarmState AlarmState { get; set; }
        public string StringAlarmState { get; set; }
        public ActivationState ActivationState { get; set; }
        public string StringActivationState { get; set; }
        public RequestActivationState RequestActivationState { get; set; }
        public string StringRequestActivationState { get; set; }
        public string Description { get; set; }
        public Image Symbol { get; set; }

        public AlarmAreaShort(AlarmArea alarmArea)
        {
            SectionId = alarmArea.SectionId;
            IdAlarmArea = alarmArea.IdAlarmArea;
            Name = alarmArea.Name;
            ShortName = alarmArea.ShortName;
            Description = alarmArea.Description;
        }

        public override string ToString()
        {
            return string.Format("{0:00} - {1}", SectionId, Name);
        }

        #region IShortObject Members

        public ObjectType ObjectType { get { return ObjectType.AlarmArea; } }

        public string GetSubTypeImageString(object value)
        {
            return string.Empty;
        }

        public object Id { get { return IdAlarmArea; } }

        #endregion
    }

    [Serializable]
    public class AlarmAreaModifyObj : AModifyObject
    {
        public override ObjectType GetOrmObjectType { get { return ObjectType.AlarmArea; } }

        public AlarmAreaModifyObj(AlarmArea alarmArea)
        {
            Id = alarmArea.IdAlarmArea;
            FullName = alarmArea.ToString();
            Description = alarmArea.Description;
        }
    }


    public class AlarmStateChangedAlarmAreaHandler : ARemotingCallbackHandler
    {
        private static volatile AlarmStateChangedAlarmAreaHandler _singleton;
        private static readonly object _syncRoot = new object();

        private Action<Guid, byte> _stateChanged;

        public static AlarmStateChangedAlarmAreaHandler Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (null == _singleton)
                            _singleton = new AlarmStateChangedAlarmAreaHandler();
                    }

                return _singleton;
            }
        }

        public AlarmStateChangedAlarmAreaHandler()
            : base("AlarmStateChangedAlarmAreaHandler")
        {
            Debug.WriteLine(">" + GetHashCode() + " " + Assembly.GetCallingAssembly() + "\r\n" + Assembly.GetExecutingAssembly());
        }

        public void RegisterStateChanged(Action<Guid, byte> stateChanged)
        {
            _stateChanged += stateChanged;
        }

        public void UnregisterStateChanged(Action<Guid, byte> stateChanged)
        {
            _stateChanged -= stateChanged;
        }

        public void RunEvent(Guid id, byte State)
        {
            if (_stateChanged != null)
                _stateChanged(id, State);
        }
    }

    public class TimeBuyingMatrixStateChangedAlarmAreaHandler : ARemotingCallbackHandler
    {
        private static volatile TimeBuyingMatrixStateChangedAlarmAreaHandler _singleton;
        private static readonly object _syncRoot = new object();

        private Action<Guid, TimeBuyingMatrixState> _stateChanged;

        public static TimeBuyingMatrixStateChangedAlarmAreaHandler Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (null == _singleton)
                            _singleton = new TimeBuyingMatrixStateChangedAlarmAreaHandler();
                    }

                return _singleton;
            }
        }

        public TimeBuyingMatrixStateChangedAlarmAreaHandler()
            : base("TimeBuyingMatrixStateChangedAlarmAreaHandler")
        {
            Debug.WriteLine(">" + GetHashCode() + " " + Assembly.GetCallingAssembly() + "\r\n" + Assembly.GetExecutingAssembly());
        }

        public void RegisterStateChanged(Action<Guid, TimeBuyingMatrixState> stateChanged)
        {
            _stateChanged += stateChanged;
        }

        public void UnregisterStateChanged(Action<Guid, TimeBuyingMatrixState> stateChanged)
        {
            _stateChanged -= stateChanged;
        }

        public void RunEvent(Guid id, TimeBuyingMatrixState State)
        {
            if (_stateChanged != null)
                _stateChanged(id, State);
        }
    }

    public class ActivationStateChangedAlarmAreaHandler : ARemotingCallbackHandler
    {
        private static volatile ActivationStateChangedAlarmAreaHandler _singleton;
        private static readonly object _syncRoot = new object();

        private Action<Guid, byte> _stateChanged;

        public static ActivationStateChangedAlarmAreaHandler Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (null == _singleton)
                            _singleton = new ActivationStateChangedAlarmAreaHandler();
                    }

                return _singleton;
            }
        }

        private ActivationStateChangedAlarmAreaHandler()
            : base("ActivationStateChangedAlarmAreaHandler")
        {
        }

        public void RegisterStateChanged(Action<Guid, byte> stateChanged)
        {
            _stateChanged += stateChanged;
        }

        public void UnregisterStateChanged(Action<Guid, byte> stateChanged)
        {
            _stateChanged -= stateChanged;
        }

        public void RunEvent(Guid id, byte State)
        {
            if (_stateChanged != null)
                _stateChanged(id, State);
        }
    }

    public class RequestActivationStateChangedAlarmAreaHandler : ARemotingCallbackHandler
    {
        private static volatile RequestActivationStateChangedAlarmAreaHandler _singleton;
        private static readonly object _syncRoot = new object();

        private Action<Guid, byte, bool> _stateChanged;

        public static RequestActivationStateChangedAlarmAreaHandler Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (null == _singleton)
                            _singleton = new RequestActivationStateChangedAlarmAreaHandler();
                    }

                return _singleton;
            }
        }

        private RequestActivationStateChangedAlarmAreaHandler()
            : base("RequestActivationStateChangedAlarmAreaHandler")
        {
        }

        public void RegisterStateChanged(Action<Guid, byte, bool> stateChanged)
        {
            _stateChanged += stateChanged;
        }

        public void UnregisterStateChanged(Action<Guid, byte, bool> stateChanged)
        {
            _stateChanged -= stateChanged;
        }

        public void RunEvent(Guid id, byte State, bool setUnsetNotConfirm)
        {
            if (_stateChanged != null)
                _stateChanged(id, State, setUnsetNotConfirm);
        }
    }

    public class SabotageStateChangedAlarmAreaHandler : ARemotingCallbackHandler
    {
        private static volatile SabotageStateChangedAlarmAreaHandler _singleton;
        private static readonly object _syncRoot = new object();

        private Action<Guid, byte> _stateChanged;

        public static SabotageStateChangedAlarmAreaHandler Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (null == _singleton)
                            _singleton = new SabotageStateChangedAlarmAreaHandler();
                    }

                return _singleton;
            }
        }

        private SabotageStateChangedAlarmAreaHandler()
            : base("SabotageStateChangedAlarmAreaHandler")
        {
        }

        public void RegisterStateChanged(Action<Guid, byte> stateChanged)
        {
            _stateChanged += stateChanged;
        }

        public void UnregisterStateChanged(Action<Guid, byte> stateChanged)
        {
            _stateChanged -= stateChanged;
        }

        public void RunEvent(Guid id, byte state)
        {
            if (_stateChanged != null)
                _stateChanged(id, state);
        }
    }

    public class AlarmAreaSensorBlockingTypeChangedHandler : ARemotingCallbackHandler
    {
        private static volatile AlarmAreaSensorBlockingTypeChangedHandler _singleton;
        private static readonly object _syncRoot = new object();

        private Action<Guid, Guid, SensorBlockingType?> _stateChanged;

        public static AlarmAreaSensorBlockingTypeChangedHandler Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (null == _singleton)
                            _singleton = new AlarmAreaSensorBlockingTypeChangedHandler();
                    }

                return _singleton;
            }
        }

        private AlarmAreaSensorBlockingTypeChangedHandler()
            : base("AlarmAreaSensorBlockingTypeChangedHandler")
        {
        }

        public void RegisterStateChanged(Action<Guid, Guid, SensorBlockingType?> stateChanged)
        {
            _stateChanged += stateChanged;
        }

        public void UnregisterStateChanged(Action<Guid, Guid, SensorBlockingType?> stateChanged)
        {
            _stateChanged -= stateChanged;
        }

        public void RunEvent(
            Guid idAlarmArea,
            Guid idInput,
            SensorBlockingType? sensorBlockingType)
        {
            if (_stateChanged != null)
                _stateChanged(
                    idAlarmArea,
                    idInput,
                    sensorBlockingType);
        }
    }

    public class AlarmAreaSensorStateChangedHandler : ARemotingCallbackHandler
    {
        private static volatile AlarmAreaSensorStateChangedHandler _singleton;
        private static readonly object _syncRoot = new object();

        private Action<Guid, Guid, State?> _stateChanged;

        public static AlarmAreaSensorStateChangedHandler Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (null == _singleton)
                            _singleton = new AlarmAreaSensorStateChangedHandler();
                    }

                return _singleton;
            }
        }

        private AlarmAreaSensorStateChangedHandler()
            : base("AlarmAreaSensorStateChangedHandler")
        {
        }

        public void RegisterStateChanged(Action<Guid, Guid, State?> stateChanged)
        {
            _stateChanged += stateChanged;
        }

        public void UnregisterStateChanged(Action<Guid, Guid, State?> stateChanged)
        {
            _stateChanged -= stateChanged;
        }

        public void RunEvent(
            Guid idAlarmArea,
            Guid idInput,
            State? sensorState)
        {
            if (_stateChanged != null)
                _stateChanged(
                    idAlarmArea,
                    idInput,
                    sensorState);
        }
    }

    public class AlarmAreaTimeBuyingHandler : ARemotingCallbackHandler
    {
        private static volatile AlarmAreaTimeBuyingHandler _singleton;
        private static readonly object _syncRoot = new object();

        public static AlarmAreaTimeBuyingHandler Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (null == _singleton)
                            _singleton = new AlarmAreaTimeBuyingHandler();
                    }

                return _singleton;
            }
        }

        private AlarmAreaTimeBuyingHandler()
            : base("AlarmAreaTimeBuyingHandler")
        {
        }

        #region BoughtTimeChanged

        private Action<Guid, string, int, int> _boughtTimeChanged;

        public void RegisterBoughtTimeChanged(Action<Guid, string, int, int> stateChanged)
        {
            _boughtTimeChanged += stateChanged;
        }

        public void UnregisterBoughtTimeChanged(Action<Guid, string, int, int> stateChanged)
        {
            _boughtTimeChanged -= stateChanged;
        }

        public void RunBoughtTimeChanged(Guid idAlarmArea, string idLogin, int used, int remaining)
        {
            if (_boughtTimeChanged != null)
                _boughtTimeChanged(idAlarmArea, idLogin, used, remaining);
        }

        #endregion

        #region BoughtTimeExpired

        private volatile Action<Guid, int, int> _boughtTimeExpired;

        public void RegisterBoughtTimeExpired(Action<Guid, int, int> stateChanged)
        {
            _boughtTimeExpired += stateChanged;
        }

        public void UnregisterBoughtTimeExpired(Action<Guid, int, int> stateChanged)
        {
            _boughtTimeExpired -= stateChanged;
        }

        public void RunBoughtTimeExpired(Guid idAlarmArea, int lastBoughtTime, int totalBoughtTime)
        {
            if (_boughtTimeExpired != null)
                _boughtTimeExpired(idAlarmArea, lastBoughtTime, totalBoughtTime);
        }

        #endregion

        #region TimeBuyingFailed

        private volatile Action<Guid, byte, int, int> _timeBuyingFailed;

        /// <summary>
        /// Time buying event with parameters idAlarmArea, reason, timeToBuy, remainingTime
        /// </summary>
        /// <param name="stateChanged"></param>
        public void RegisterTimeBuyingFailed(Action<Guid, byte, int, int> stateChanged)
        {
            _timeBuyingFailed += stateChanged;
        }

        public void UnregisterTimeBuyingFailed(Action<Guid, byte, int, int> stateChanged)
        {
            _timeBuyingFailed -= stateChanged;
        }

        public void RunTimeBuyingFailed(Guid idAlarmArea, byte reason, int timeToBuy, int remainingTime)
        {
            if (_timeBuyingFailed != null)
                _timeBuyingFailed(idAlarmArea, reason, timeToBuy, remainingTime);
        }

        #endregion
    }
}
