using System;
using System.Collections.Generic;
using Contal.BoolExpressions.CrossPlatform;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.Alarms;
using Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes;
using Contal.Cgp.NCAS.CCU.EventParameters;
using Contal.Cgp.NCAS.Globals;
using Contal.Drivers.LPC3250;
using Contal.IwQuick;
using Contal.IwQuick.Data;
using Contal.IwQuick.Sys;
using JetBrains.Annotations;

namespace Contal.Cgp.NCAS.CCU.AlarmArea
{
    internal class AlarmAreaStateAndSettings : 
        AStateAndSettingsObject<DB.AlarmArea>,
        IAlarmAreaStateAndSettings
    {
        public IAlarmAreaController AlarmAreaController
        {
            get;
            private set;
        }

        public bool AutomaticDeactive
        {
            get;
            private set;
        }

        public IBoolExpression InAlarm
        {
            get
            {
                return
                    AlarmAreaController != null
                        ? AlarmAreaController.InAlarm
                        : null;
            }
        }

        public IBoolExpression SirenOutput
        {
            get
            {
                return
                    AlarmAreaController != null
                        ? AlarmAreaController.SirenOutput
                        : null;
            }
        }

        public IBoolExpression InSabotage
        {
            get
            {
                return
                    AlarmAreaController != null
                        ? AlarmAreaController.InSabotage
                        : null;
            }
        }

        public IBoolExpression AlarmNotAcknowledged
        {
            get
            {
                return
                    AlarmAreaController != null
                        ? AlarmAreaController.AlarmNotAcknowledged
                        : null;
            }
        }

        public IBoolExpression AnySensorInAlarm
        {
            get
            {
                return
                    AlarmAreaController != null
                        ? AlarmAreaController.AnySensorInAlarm
                        : null;
            }
        }

        public IBoolExpression AnySensorNotAcknowledged
        {
            get
            {
                return
                    AlarmAreaController != null
                        ? AlarmAreaController.AnySensorNotAcknowledged
                        : null;
            }
        }

        public IBoolExpression AnySensorPermanentlyBlocked
        {
            get
            {
                return
                    AlarmAreaController != null
                        ? AlarmAreaController.AnySensorPermanentlyBlocked
                        : null;
            }
        }

        public IBoolExpression AnySensorTemporarilyBlocked
        {
            get
            {
                return
                    AlarmAreaController != null
                        ? AlarmAreaController.AnySensorTemporarilyBlocked
                        : null;
            }
        }

        public IBoolExpression NotAcknowledged
        {
            get
            {
                return
                    AlarmAreaController != null
                        ? AlarmAreaController.NotAcknowledged
                        : null;
            }
        }

        private Guid _guidOutputActivation = Guid.Empty;
        private Guid _guidOutputPrewarning = Guid.Empty;
        private Guid _guidOutputTmpUnsetEntry = Guid.Empty;
        private Guid _guidOutputTmpUnsetExit = Guid.Empty;
        private Guid _guidOutputAAlarm = Guid.Empty;

        private Guid _idOutputSetByObjectForAaFailed;
        private int _outputSetNotCalmAaByObjectForAaOnPeriod;
        private bool _isSpecialOutputSetByObjectForAaFailedOn;

        private volatile object _lockSetNotCalmAaByObjectForAaOnPeriodTimer = new object();
        private NativeTimer _setNotCalmAaByObjectForAaOnPeriodTimer;

        private readonly HashSet<Guid> _activatedSensorsDuringAlarmState = new HashSet<Guid>();

        private readonly EventHandlerGroup<IAlarmAreaActivationEventHandler> _activationEventHandlerGroup =
            new EventHandlerGroup<IAlarmAreaActivationEventHandler>();

        public readonly EventHandlerGroup<IAlarmAreaSensorsEventHandler> SensorsEventHandlerGroup =
            new EventHandlerGroup<IAlarmAreaSensorsEventHandler>();

        public AlarmAreaAutomaticActivationMode AutomaticActivationMode
        {
            get;
            private set;
        }

        public Guid GuidAlarmArea
        {
            get { return Id; }
        }

        public State ActivationState
        {
            get
            {
                if (AlarmAreaController != null)
                    return AlarmAreaController.ActivationState;

                var alarmAreaStates = AlarmAreas.Singleton.GetAlarmAreaStates(Id);

                return alarmAreaStates != null
                    ? alarmAreaStates.ActivationState
                    : State.Unknown;
            }
        }

        public State AlarmState
        {
            get
            {
                if (AlarmAreaController != null)
                    return 
                        AlarmAreaController.AlarmState;

                var alarmAreaStates = AlarmAreas.Singleton.GetAlarmAreaStates(Id);

                return
                    alarmAreaStates != null
                        ? alarmAreaStates.AlarmState
                        : State.Normal;
            }
        }

        public AlarmAreaStateAndSettings(DB.AlarmArea alarmArea)
            : base(alarmArea.IdAlarmArea)
        {
            _guidOutputActivation = alarmArea.GuidOutputActivation;
            _guidOutputPrewarning = alarmArea.GuidOutputPrewarning;
            _guidOutputTmpUnsetEntry = alarmArea.GuidOutputTmpUnsetEntry;
            _guidOutputTmpUnsetExit = alarmArea.GuidOutputTmpUnsetExit;
            _guidOutputAAlarm = alarmArea.GuidOutputAAlarm;

            _idOutputSetByObjectForAaFailed = alarmArea.IdOutputSetByObjectForAaFailed;
            _outputSetNotCalmAaByObjectForAaOnPeriod = alarmArea.OutputSetNotCalmAaByObjectForAaOnPeriod;

            AutomaticActivationMode = alarmArea.AutomaticActivationMode;
            AutomaticDeactive = alarmArea.AutomaticDeactive;
        }

        public void OnSabotageStateChanged(bool isInSabotage)
        {
            SendEventSabotageStateChanged(isInSabotage, true);
        }

        public void OnNotAcknowledgedStateChanged(bool isNotAcknowledged)
        {
            if (!isNotAcknowledged)
            {
                _notAcknowledgedAlarmDuringSetPeriod = false;
            }

            AlarmAreas.Singleton.EventHandlerGroup.ForEach(
                eventHandler => eventHandler.OnNotAcknolwedgedStateChanged(
                    this,
                    isNotAcknowledged));
        }

        public void SendEventSabotageStateChanged(bool isInSabotage, bool stateChanged)
        {
            var sabotageState = isInSabotage ? State.Alarm : State.Normal;

            Events.ProcessEvent(
                stateChanged
                    ? (EventParameters.EventParameters)
                        new EventAlarmAreaSabotageStateChanged(
                            Id,
                            sabotageState)
                    : new EventAlarmAreaSabotageStateInfo(
                        Id,
                        sabotageState));
        }

        /// <summary>
        /// Runs event on specific alarm area resporting to CR's change
        /// </summary>
        /// <param name="report"></param>
        public void OnAlarmAreaReportingToCRChanged(bool? report)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "void AlarmAreaSettings.RunAlarmAreaReportingToCRChanged(bool? report): [{0}]",
                    Log.GetStringFromParameters(report)));

            try
            {
                AlarmAreas.Singleton.EventHandlerGroup.ForEach(
                    eventHandler => eventHandler.OnSpecificCrReportingChanged(
                        this,
                        report));
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        public interface IAlarmAreaUnsetResult
        {
            void OnFailed(
                AlarmAreaActionResult alarmAreaActionResult, 
                int timeToBuy, 
                int remainingTime);

            void OnSucceded(
                int timeToBuy,
                int remainingTime,
                bool nonAcknowledgedAlarmDuringSetPeriod);
        }

        public void OnAlarmStateChanged(State alarmState)
        {
            if (alarmState == State.Alarm)
                _notAcknowledgedAlarmDuringSetPeriod = true;

            try
            {
                AlarmAreas.Singleton.EventHandlerGroup.ForEach(
                    eventHandler => eventHandler.OnAlarmStateChanged(
                        this,
                        AlarmState));
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        public void SendActivationStateToServer(
            bool eventLog,
            [CanBeNull]
            AlarmAreas.SetUnsetParams setUnsetParams)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "void AlarmAreaSettings.SendActivationStateToServer(bool eventLog, AlarmAreas.SetUnsetParams setUnsetParams): [{0}]",
                    Log.GetStringFromParameters(
                        eventLog, 
                        setUnsetParams)));

            Events.ProcessEvent(
                new EventAlarmAreaActivationStateChanged(
                    ActivationState,
                    Id,
                    setUnsetParams != null
                        ? setUnsetParams.IdCardReader
                        : Guid.Empty,
                    setUnsetParams != null
                        ? setUnsetParams.IdCard
                        : Guid.Empty,
                    setUnsetParams != null
                        ? setUnsetParams.IdPerson
                        : Guid.Empty));
        }

        public void OnActivationStateChanged(
            State activationState,
            [CanBeNull]
            AlarmAreas.SetUnsetParams setUnsetParams)
        {
            try
            {
                if (activationState == State.Set
                    || activationState == State.TemporaryUnsetEntry
                    || activationState == State.TemporaryUnsetExit)
                {
                    SetSpecialOutputActivationStateOn();
                }
                else
                    SetSpecialOutputActivationStateOff();

                if (activationState == State.Prewarning)
                    SetSpecialOutputPrewarningOn();
                else
                    SetSpecialOutputPrewarningOff();

                if (activationState == State.TemporaryUnsetEntry)
                    SetSpecialOutputTmpUnsetEntryOn();
                else
                    SetSpecialOutputTmpUnsetEntryOff();

                if (activationState == State.TemporaryUnsetExit)
                    SetSpecialOutputTmpUnsetExitOn();
                else
                    SetSpecialOutputTmpUnsetExitOff();

                SendActivationStateToServer(
                    true, 
                    setUnsetParams);

                _activationEventHandlerGroup.ForEach(
                    eventHandler => eventHandler.OnActivationStateChanged(
                        ActivationState));

                AlarmAreas.Singleton.EventHandlerGroup.ForEach(
                    eventHandler => eventHandler.OnActivationStateChanged(
                        this,
                        ActivationState));
            }
            catch (Exception ex)
            {
                HandledExceptionAdapter.Examine(ex);
            }
        }

        #region Special outputs

        /// <summary>
        /// Replace special output
        /// </summary>
        /// <param name="activator"></param>
        /// <param name="guidOldOutput"></param>
        /// <param name="guidNewOutput"></param>
        /// <param name="isNowOn"></param>
        private static void ResetSpecialOutput(
            string activator,
            ref Guid guidOldOutput,
            Guid guidNewOutput,
            bool isNowOn)
        {
            if (guidOldOutput == guidNewOutput)
                return;

            if (guidOldOutput != Guid.Empty)
                Outputs.Singleton.Off(
                    activator,
                    guidOldOutput);

            if (guidNewOutput != Guid.Empty && isNowOn)
                Outputs.Singleton.On(
                    activator,
                    guidNewOutput);

            guidOldOutput = guidNewOutput;
        }

        public void SetSpecialOutputActivationStateOn()
        {
            if (_guidOutputActivation != Guid.Empty)
                Outputs.Singleton.On(
                    Id + AlarmAreas.ACTIVATIONONOFF,
                    _guidOutputActivation);
        }

        private void SetSpecialOutputActivationStateOff()
        {
            if (_guidOutputActivation != Guid.Empty)
                Outputs.Singleton.Off(
                    Id + AlarmAreas.ACTIVATIONONOFF,
                    _guidOutputActivation);
        }

        private void SetSpecialOutputPrewarningOn()
        {
            if (_guidOutputPrewarning != Guid.Empty)
                Outputs.Singleton.On(
                    Id + AlarmAreas.PREWARNINGONOFF,
                    _guidOutputPrewarning);
        }

        private void SetSpecialOutputPrewarningOff()
        {
            if (_guidOutputPrewarning != Guid.Empty)
                Outputs.Singleton.Off(
                    Id + AlarmAreas.PREWARNINGONOFF,
                    _guidOutputPrewarning);
        }

        private void SetSpecialOutputTmpUnsetEntryOn()
        {
            if (_guidOutputTmpUnsetEntry != Guid.Empty)
                Outputs.Singleton.On(
                    Id + AlarmAreas.TMPUNSETENTRYONOFF,
                    _guidOutputTmpUnsetEntry);
        }

        private void SetSpecialOutputTmpUnsetEntryOff()
        {
            if (_guidOutputTmpUnsetEntry != Guid.Empty)
                Outputs.Singleton.Off(
                    Id + AlarmAreas.TMPUNSETENTRYONOFF,
                    _guidOutputTmpUnsetEntry);
        }

        private void SetSpecialOutputTmpUnsetExitOn()
        {
            if (_guidOutputTmpUnsetExit != Guid.Empty)
                Outputs.Singleton.On(
                    Id + AlarmAreas.TMPUNSETEXITONOFF,
                    _guidOutputTmpUnsetExit);
        }

        private void SetSpecialOutputTmpUnsetExitOff()
        {
            if (_guidOutputTmpUnsetExit != Guid.Empty)
                Outputs.Singleton.Off(
                    Id + AlarmAreas.TMPUNSETEXITONOFF,
                    _guidOutputTmpUnsetExit);
        }

        private bool _isSpecialOutputAAlarmOn;

        private bool _notAcknowledgedAlarmDuringSetPeriod;

        public void SetSpecialOutputAAlarmOn()
        {
            if (_guidOutputAAlarm != Guid.Empty)
                Outputs.Singleton.On(
                    Id + AlarmAreas.AALARMONOFF,
                    _guidOutputAAlarm);

            _isSpecialOutputAAlarmOn = true;
        }

        public void SetSpecialOutputAAlarmOff()
        {
            if (_guidOutputAAlarm != Guid.Empty)
                Outputs.Singleton.Off(
                    Id + AlarmAreas.AALARMONOFF,
                    _guidOutputAAlarm);

            _isSpecialOutputAAlarmOn = false;
        }

        public void SetSpecialOutputSetByObjectForAaFailedOn()
        {
            if (_idOutputSetByObjectForAaFailed != Guid.Empty)
                Outputs.Singleton.On(
                    Id + AlarmAreas.OUTPUT_SET_BY_ON_OFF_OBJECT_FAILED_ON_OFF,
                    _idOutputSetByObjectForAaFailed);

            _isSpecialOutputSetByObjectForAaFailedOn = true;
        }

        public void SetSpecialOutputSetByObjecForAaFailedOff()
        {
            if (_idOutputSetByObjectForAaFailed != Guid.Empty)
                Outputs.Singleton.Off(
                    Id + AlarmAreas.OUTPUT_SET_BY_ON_OFF_OBJECT_FAILED_ON_OFF,
                    _idOutputSetByObjectForAaFailed);

            _isSpecialOutputSetByObjectForAaFailedOn = false;
        }

        public void SetSpecialOutputSetNotCalmAaByObjectForAaOn()
        {
            if (_idOutputSetByObjectForAaFailed != Guid.Empty)
                Outputs.Singleton.On(
                    Id + AlarmAreas.OUTPUT_SET_BY_ON_OFF_OBJECT_FAILED_ON_OFF,
                    _idOutputSetByObjectForAaFailed);

            _isSpecialOutputSetByObjectForAaFailedOn = true;

            lock (_lockSetNotCalmAaByObjectForAaOnPeriodTimer)
            {
                _setNotCalmAaByObjectForAaOnPeriodTimer = NativeTimerManager.StartTimeout(
                    _outputSetNotCalmAaByObjectForAaOnPeriod,
                    OnSetNotCalmAaByObjectForAaOnPeriodTimer);
            }
        }

        private bool OnSetNotCalmAaByObjectForAaOnPeriodTimer(NativeTimer timer)
        {
            lock (_lockSetNotCalmAaByObjectForAaOnPeriodTimer)
                _setNotCalmAaByObjectForAaOnPeriodTimer = null;

            SetSpecialOutputSetByObjecForAaFailedOff();

            return true;
        }

        #endregion

        public bool EnabledCrEventlogs()
        {
            return DbObject.EnableEventlogsInCR;
        }

        public bool EnabledCrEventlogsAlarmAreaSet()
        {
            return DbObject.AaSet;
        }

        public bool EnabledCrEventlogsAlarmAreaUnconditionalSet()
        {
            return DbObject.AaUnconditionalSet;
        }

        public bool EnabledCrEventlogsAlarmAreaUnset()
        {
            return DbObject.AaUnset;
        }

        public bool EnabledCrEventlogsAlarmAreaAlarm()
        {
            return DbObject.AaAlarm;
        }

        public bool EnabledCrEventlogsAlarmAreaNormal()
        {
            return DbObject.AaNormal;
        }

        public bool EnabledCrEventlogsAlarmAreaAcknowledged()
        {
            return DbObject.AaAcknowledged;
        }

        public bool EnabledCrEventlogsSensorsAlarm()
        {
            return DbObject.SensorAlarm;
        }

        public bool EnabledCrEventlogsSensorsNormal()
        {
            return DbObject.SensorNormal;
        }

        public bool EnabledCrEventlogsSensorsAcknowledged()
        {
            return DbObject.SensorAcknowledged;
        }

        public bool AddActivatedSensorDuringAlarmState(Guid idInput)
        {
            lock (_activatedSensorsDuringAlarmState)
            {
                return _activatedSensorsDuringAlarmState.Add(idInput);
            }
        }

        public bool RemoveActivatedSensorDuringAlarmState(Guid idInput)
        {
            lock (_activatedSensorsDuringAlarmState)
                return _activatedSensorsDuringAlarmState.Remove(idInput);
        }

        public IEnumerable<Guid> GetActivatedSensorsDuringAlarmStateAndClear()
        {
            lock (_activatedSensorsDuringAlarmState)
            {
                var activatedSensorsDuringAlarmState = new LinkedList<Guid>(_activatedSensorsDuringAlarmState);

                _activatedSensorsDuringAlarmState.Clear();

                return activatedSensorsDuringAlarmState.Count > 0
                    ? activatedSensorsDuringAlarmState
                    : null;
            }
        }

        private readonly WeakReference _alarmAreaWeakReference = new WeakReference(null);

        public DB.AlarmArea DbObject
        {
            get
            {
                lock (_alarmAreaWeakReference)
                {
                    var alarmArea = _alarmAreaWeakReference.Target;

                    if (alarmArea == null)
                    {
                        alarmArea = Database.ConfigObjectsEngine.GetFromDatabase(
                            ObjectType.AlarmArea,
                            Id);

                        _alarmAreaWeakReference.Target = alarmArea;
                    }

                    return (DB.AlarmArea) alarmArea;
                }
            }
        }

        public string AlarmAreaName
        {
            get; 
            private set;
        }

        protected override void OnObjectSaved(DB.AlarmArea newDbObject)
        {
            lock (_alarmAreaWeakReference)
                _alarmAreaWeakReference.Target = newDbObject;

            AlarmAreaName = newDbObject.ToString();
        }

        public ICollection<ISensorStateAndSettings> GetSensors()
        {
            return AlarmAreaController != null
                ? AlarmAreaController.GetSensors()
                : new ISensorStateAndSettings[]{};
        }

        void IAlarmAreaStateAndSettings.AddSensorsEventHandler(IAlarmAreaSensorsEventHandler sensorsEventHandler)
        {
            SensorsEventHandlerGroup.Add(sensorsEventHandler);
            sensorsEventHandler.OnAttached(GetSensors());
        }

        void IAlarmAreaStateAndSettings.RemoveSensorsEventHandler(IAlarmAreaSensorsEventHandler sensorsEventHandler)
        {
            SensorsEventHandlerGroup.Remove(sensorsEventHandler);
        }

        public void AcknowledgeAllSensorAlarms(
            Guid idCardReader,
            AccessDataBase accessData)
        {
            if (AlarmAreaController != null)
                AlarmAreaController.AcknowledgeAllSensorAlarms(
                    idCardReader,
                    accessData);
        }

        public int? TemporaryUnsetEntryDuration
        {
            get;
            private set;
        }

        public int? PrewarningDuration
        {
            get;
            private set;
        }

        public int? TemporaryUnsetExitDuration
        {
            get;
            private set;
        }

        public bool TimeBuyingOnlyInPrewarning
        {
            get;
            private set;
        }

        public bool ABAlarmHandling
        {
            get;
            private set;
        }

        public int SensorsCountToAAlarm
        {
            get;
            private set;
        }

        public bool NotAcknowledgedAlarmDuringSetPeriod
        {
            get { return _notAcknowledgedAlarmDuringSetPeriod; }
        }

        private void ConfigureCommon(DB.AlarmArea alarmArea)
        {
            AutomaticActivationMode = alarmArea.AutomaticActivationMode;
            AutomaticDeactive = alarmArea.AutomaticDeactive;

            PrewarningDuration =
                alarmArea.PreWarning
                    ? alarmArea.PreWarningDuration != null && alarmArea.PreWarningDuration > 0
                        ? alarmArea.PreWarningDuration
                        : null
                    : null;

            TemporaryUnsetEntryDuration =
                alarmArea.PreAlarm
                    ? alarmArea.PreAlarmDuration != null && alarmArea.PreAlarmDuration > 0
                        ? alarmArea.PreAlarmDuration
                        : null
                    : null;

            TemporaryUnsetExitDuration =
                    alarmArea.TemporaryUnsetDuration > 0
                        ? (int?)alarmArea.TemporaryUnsetDuration
                        : null;

            TimeBuyingOnlyInPrewarning =
                alarmArea.TimeBuyingOnlyInPrewarning;

            ABAlarmHandling = alarmArea.ABAlarmHandling;

            SensorsCountToAAlarm = alarmArea.PercentageSensorsToAAlarm;

            _outputSetNotCalmAaByObjectForAaOnPeriod = alarmArea.OutputSetNotCalmAaByObjectForAaOnPeriod;

            BlockedAlarmsManager.Singleton.ConfigureSpecificBlockingForObject(
                AlarmType.AlarmArea_SetByOnOffObjectFailed,
                new IdAndObjectType(
                    alarmArea.IdAlarmArea,
                    ObjectType.AlarmArea),
                alarmArea.AlarmAreaSetByOnOffObjectFailed,
                alarmArea.BlockAlarmAreaSetByOnOffObjectFailed,
                alarmArea.ObjBlockAlarmAreaSetByOnOffObjectFailedId,
                alarmArea.ObjBlockAlarmAreaSetByOnOffObjectFailedObjectType,
                true);

            if (alarmArea.AlarmTypeAndIdAlarmArcs != null)
                CatAlarmsManager.Singleton.ConfigureSpecificAlarmArcs(
                    new IdAndObjectType(
                        alarmArea.IdAlarmArea,
                        ObjectType.AlarmArea),
                    alarmArea.AlarmTypeAndIdAlarmArcs);
            else
                CatAlarmsManager.Singleton.RemoveSpecificAlarmArcsConfiguration(
                    new IdAndObjectType(
                        alarmArea.IdAlarmArea,
                        ObjectType.AlarmArea));
        }

        protected override void ConfigureInternal(DB.AlarmArea alarmArea)
        {
            if (ConfigurationState == ConfigurationState.ConfiguringNew)
            {
                lock (_alarmAreaWeakReference)
                    _alarmAreaWeakReference.Target = alarmArea;

                AlarmAreaName = alarmArea.ToString();
            }

            if (AlarmAreaController == null)
            {
                _guidOutputActivation = alarmArea.GuidOutputActivation;
                _guidOutputPrewarning = alarmArea.GuidOutputPrewarning;
                _guidOutputTmpUnsetEntry = alarmArea.GuidOutputTmpUnsetEntry;
                _guidOutputTmpUnsetExit = alarmArea.GuidOutputTmpUnsetExit;
                _guidOutputAAlarm = alarmArea.GuidOutputAAlarm;

                _idOutputSetByObjectForAaFailed = alarmArea.IdOutputSetByObjectForAaFailed;

                ConfigureCommon(alarmArea);

                AlarmAreaController =
                    alarmArea.UseEIS
                        ? new ExternalAlarmAreaController(this)
                        : (IAlarmAreaController)new CcuAlarmAreaController(this);

                AlarmAreaController.Init(alarmArea);
            }
            else
            {
                var activationState = AlarmAreaController.ActivationState;

                ResetSpecialOutput(
                    Id + AlarmAreas.ACTIVATIONONOFF,
                    ref _guidOutputActivation,
                    alarmArea.GuidOutputActivation,
                    activationState == State.Set
                    || activationState == State.TemporaryUnsetEntry
                    || activationState == State.TemporaryUnsetExit);

                ResetSpecialOutput(
                    Id + AlarmAreas.PREWARNINGONOFF,
                    ref _guidOutputPrewarning,
                    alarmArea.GuidOutputPrewarning,
                    activationState == State.Prewarning);

                ResetSpecialOutput(
                    Id + AlarmAreas.TMPUNSETENTRYONOFF,
                    ref _guidOutputTmpUnsetEntry,
                    alarmArea.GuidOutputTmpUnsetEntry,
                    activationState == State.TemporaryUnsetEntry);

                ResetSpecialOutput(
                    Id + AlarmAreas.TMPUNSETEXITONOFF,
                    ref _guidOutputTmpUnsetExit,
                    alarmArea.GuidOutputTmpUnsetExit,
                    activationState == State.TemporaryUnsetExit);

                ResetSpecialOutput(
                    Id + AlarmAreas.AALARMONOFF,
                    ref _guidOutputAAlarm,
                    alarmArea.GuidOutputAAlarm,
                    _isSpecialOutputAAlarmOn);

                ResetSpecialOutput(
                    Id + AlarmAreas.OUTPUT_SET_BY_ON_OFF_OBJECT_FAILED_ON_OFF,
                    ref _idOutputSetByObjectForAaFailed,
                    alarmArea.IdOutputSetByObjectForAaFailed,
                    _isSpecialOutputSetByObjectForAaFailedOn);

                ConfigureCommon(alarmArea);

                AlarmAreaController.Update(alarmArea);
            }
        }

        protected override void ApplyHwSetup(DB.AlarmArea dbObject)
        {
        }

        protected override void UnconfigureInternal(DB.AlarmArea newDbObject)
        {
            var unconfiguringDueToDelete = newDbObject == null;

            if (unconfiguringDueToDelete || newDbObject.UseEIS != DbObject.UseEIS)
            {
                AlarmAreas.Singleton.TryCancelBoughtTime(Id);

                AlarmAreaController.Dispose();
                AlarmAreaController = null;

                SetSpecialOutputActivationStateOff();
                SetSpecialOutputPrewarningOff();
                SetSpecialOutputTmpUnsetEntryOff();
                SetSpecialOutputTmpUnsetExitOff();
                SetSpecialOutputAAlarmOff();

                lock (_lockSetNotCalmAaByObjectForAaOnPeriodTimer)
                {
                    if (_setNotCalmAaByObjectForAaOnPeriodTimer != null)
                        try
                        {
                            _setNotCalmAaByObjectForAaOnPeriodTimer.StopTimer();
                        }
                        catch (Exception error)
                        {
                            HandledExceptionAdapter.Examine(error);
                        }

                    _setNotCalmAaByObjectForAaOnPeriodTimer = null;
                }

                SetSpecialOutputSetByObjecForAaFailedOff();
            }
            else
                AlarmAreaController.Unconfigure(newDbObject);

            LogicalConfigurator.LogicalConfigurator.Singleton.Unconfigure(
                new IdAndObjectType(
                    Id,
                    ObjectType.AlarmArea),
                newDbObject);
        }

        public void OnUnset()
        {
            _notAcknowledgedAlarmDuringSetPeriod = false;

            lock (_lockSetNotCalmAaByObjectForAaOnPeriodTimer)
                if (_setNotCalmAaByObjectForAaOnPeriodTimer != null)
                {
                    try
                    {
                        _setNotCalmAaByObjectForAaOnPeriodTimer.StopTimer();
                    }
                    catch (Exception error)
                    {
                        HandledExceptionAdapter.Examine(error);
                    }

                    _setNotCalmAaByObjectForAaOnPeriodTimer = null;
                    SetSpecialOutputSetByObjecForAaFailedOff();
                }
        }

        public void AddAlarmAreaActivationEventHandler(IAlarmAreaActivationEventHandler alarmAreaActivationEventHandler)
        {
            _activationEventHandlerGroup.Add(alarmAreaActivationEventHandler);
        }

        public void RemoveAlarmAreaActivationEventHandler(IAlarmAreaActivationEventHandler alarmAreaActivationEventHandler)
        {
            _activationEventHandlerGroup.Remove(alarmAreaActivationEventHandler);
        }

        public void SendAllStates()
        {
            Events.ProcessEvent(
                new EventAlarmAreaAlarmStateInfo(
                    Id,
                    AlarmState));

            SendActivationStateToServer(
                false,
                new AlarmAreas.SetUnsetParams(
                    Guid.Empty,
                    new AccessDataBase(),
                    Guid.Empty,
                    false,
                    false));

            var alarmAreaController = AlarmAreaController;

            if (alarmAreaController == null)
                return;

            alarmAreaController.SendSabotageStateInfoToServer();
            alarmAreaController.SendBlockingTypeForAllSensors();
            alarmAreaController.SendStateForAllSensors();
            alarmAreaController.SendTimeBuyingMatrixStateToServer();
        }

        public bool CheckUnsetRights(Guid idPerson)
        {
            return AlarmAreaAccessRightsManager.Singleton.CheckRigthsToUnset(
                idPerson, 
                Id)
                || (AlarmAreaAccessRightsManager.Singleton.CheckRigthsToTimeBuying(
                        idPerson, 
                        Id)
                    && AlarmAreaController != null
                    && AlarmAreaController.ProvideUnsetForOnlyTimeBuyingAccessRightsSync());
        }
    }
}