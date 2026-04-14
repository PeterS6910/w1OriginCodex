using System;
using System.Collections.Generic;
using System.Linq;
using Contal.BoolExpressions.CrossPlatform;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.Alarms;
using Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes;
using Contal.Cgp.NCAS.CCU.EventParameters;
using Contal.Cgp.NCAS.Globals;
using Contal.IwQuick;
using Contal.IwQuick.CrossPlatform.Data;
using Contal.IwQuick.CrossPlatform.Threads;
using Contal.IwQuick.Sys;
using JetBrains.Annotations;

namespace Contal.Cgp.NCAS.CCU.AlarmArea
{
    internal abstract partial class AAlarmAreaController<TAlarmAreaController>
        : IAlarmAreaController
        where TAlarmAreaController : AAlarmAreaController<TAlarmAreaController>
    {
        //TODO remove
        protected class UnsetAlarmAreaExecutor
        {
            private int _timeToBuy;

            [NotNull]
            public AlarmAreas.SetUnsetParams SetUnsetParams { get; private set; }

            public AlarmAreaStateAndSettings.IAlarmAreaUnsetResult AlarmAreaUnsetResult { get; private set; }

            private readonly Guid _guidLogin;
            private readonly Guid _guidPerson;

            private readonly bool _unsetViaTimeBuying;

            private int _remainingTime;

            public UnsetAlarmAreaExecutor(
                AlarmAreaStateAndSettings.IAlarmAreaUnsetResult alarmAreaUnsetResult,
                Guid guidLogin,
                Guid guidPerson,
                int timeToBuy,
                [NotNull]
                AlarmAreas.SetUnsetParams setUnsetParams)
            {
                _timeToBuy = timeToBuy;
                SetUnsetParams = setUnsetParams;
                AlarmAreaUnsetResult = alarmAreaUnsetResult;

                _guidLogin = guidLogin;
                _guidPerson = guidPerson;
                _unsetViaTimeBuying = _timeToBuy > 0
                                      || _timeToBuy == -1;
                _remainingTime = 0;
            }

            private IEnumerable<Guid> GetUnsetSources()
            {
                if (SetUnsetParams.IdCardReader != Guid.Empty)
                {
                    yield return SetUnsetParams.IdCardReader;
                }

                if (SetUnsetParams.IdCard != Guid.Empty)
                {
                    yield return SetUnsetParams.IdCard;
                }

                if (SetUnsetParams.IdActivationObject != Guid.Empty)
                    yield return SetUnsetParams.IdActivationObject;
            }

            public void Execute(TAlarmAreaController alarmAreaController)
            {
                int timeToBuy = _timeToBuy;

                CcuCore.DebugLog.Info(
                    Log.NORMAL_LEVEL,
                    () => string.Format(
                        "void AlarmAreaSettings.DoUnsetAlarmArea(Guid[] sources, Guid guidLogin, Guid guidPerson, int timeToBuy): [{0}]",
                        Log.GetStringFromParameters(
                            SetUnsetParams,
                            _guidLogin,
                            _guidPerson,
                            timeToBuy)));

                var alarmAreaSettings = alarmAreaController.AlarmAreaStateAndSettings;
                var idAlarmArea = alarmAreaSettings.Id;

                try
                {
                    var alarmArea = alarmAreaSettings.DbObject;

                    if (_unsetViaTimeBuying)
                    {
                        var result =
                            AlarmAreas.Singleton.BuyTimeForAlarmArea(
                                alarmArea,
                                ref _timeToBuy,
                                out _remainingTime);

                        if (result != AlarmAreaActionResult.Success)
                        {
                            Events.ProcessEvent(
                                new EventAlarmAreaTimeBuyingFailed(
                                    idAlarmArea,
                                    (byte)result,
                                    _guidLogin,
                                    _guidPerson,
                                    _timeToBuy,
                                    _remainingTime,
                                    GetUnsetSources()
                                        .ToArray()));

                            AlarmAreaUnsetResult.OnFailed(
                                result,
                                _timeToBuy,
                                _remainingTime);

                            return;
                        }
                    }
                    else
                    {
                        // Unset without time buying will reset time buying
                        AlarmAreas.Singleton.ResetTimeBuyingForAlarmArea(idAlarmArea);
                    }

                    bool nonAcknowledgedAlarmDuringSetPeriod =
                        alarmAreaSettings.NotAcknowledgedAlarmDuringSetPeriod;

                    alarmAreaController.RetrySetAlarmAreaByObjectForAa = false;

                    alarmAreaController.OnUnset();

                    alarmAreaController.ChangeActivationState(
                        State.Unset,
                        SetUnsetParams);

                    alarmAreaSettings.OnUnset();

                    if (_unsetViaTimeBuying)
                        OnTimeBuyingSucceeded(
                            idAlarmArea,
                            nonAcknowledgedAlarmDuringSetPeriod);
                    else
                        AlarmAreaUnsetResult.OnSucceded(
                            0,
                            0,
                            nonAcknowledgedAlarmDuringSetPeriod);

                    var activatedSensorsDuringAlarmState =
                        alarmAreaSettings.GetActivatedSensorsDuringAlarmStateAndClear();

                    if (activatedSensorsDuringAlarmState != null)
                        CatAlarmsManager.Singleton.SendCatAlarms(
                            AlarmAreaAlarm.CreateCatAlarmsForActivatedSensorsDuringAlarmState(
                                alarmArea,
                                activatedSensorsDuringAlarmState));
                }
                catch (Exception e)
                {
                    HandledExceptionAdapter.Examine(e);
                    Events.SaveEventRunMethodFailed("UnsetAlarmArea", e);
                }
            }

            public bool CheckUnsetPreconditions(
                TAlarmAreaController alarmAreaController)
            {
                var alarmArea = alarmAreaController.AlarmAreaStateAndSettings.DbObject;

                var idAlarmArea = alarmArea.IdAlarmArea;

                if (alarmAreaController.ActivationState == State.Unset)
                {
                    // Unset by object for automatic activation during bought time
                    if (!AlarmAreas.Singleton.ResetTimeBuyingForAlarmArea(idAlarmArea))
                        return false;

                    // Send event to server that this alarm area was unset by OnOffObject after unset by time buying.
                    var idActivationObject = SetUnsetParams.IdActivationObject;

                    if (idActivationObject != Guid.Empty)
                    {
                        alarmAreaController.SendEventAlarmAreaSetUnsetByObjectForAa(
                            idActivationObject,
                            false);
                    }

                    return false;
                }

                if (_unsetViaTimeBuying)
                {
                    // Unset with time buying 
                    if (!alarmArea.TimeBuyingEnabled)
                    {
                        // This alarm area has disabled time buying
                        Events.ProcessEvent(
                            new EventAlarmAreaTimeBuyingFailed(
                                idAlarmArea,
                                (byte)AlarmAreaActionResult.FailedTimeBuyingNotEnabled,
                                _guidLogin,
                                _guidPerson,
                                _timeToBuy,
                                int.MinValue,
                                GetUnsetSources().ToArray()));

                        AlarmAreaUnsetResult.OnFailed(
                            AlarmAreaActionResult.FailedTimeBuyingNotEnabled,
                            _timeToBuy,
                            int.MinValue);

                        return false;
                    }

                    // Check if user have rights to unset
                    if (_guidPerson != Guid.Empty
                        && !AlarmAreaAccessRightsManager.Singleton.CheckRigthsToTimeBuying(
                            _guidPerson,
                            idAlarmArea)
                        && !AlarmAreaAccessRightsManager.Singleton.CheckRigthsToUnset(
                            _guidPerson,
                            idAlarmArea))
                    {
                        // Time buying is not enabled by ACL
                        Events.ProcessEvent(
                            new EventAlarmAreaTimeBuyingFailed(
                                idAlarmArea,
                                (byte)AlarmAreaActionResult.FailedInsufficientRights,
                                _guidLogin,
                                _guidPerson,
                                _timeToBuy,
                                int.MinValue,
                                GetUnsetSources().ToArray()));

                        AlarmAreaUnsetResult.OnFailed(
                            AlarmAreaActionResult.FailedInsufficientRights,
                            _timeToBuy,
                            int.MinValue);

                        return false;
                    }

                    if ((alarmArea.TimeBuyingMaxDuration != null
                         && _timeToBuy > alarmArea.TimeBuyingMaxDuration)
                        || (alarmArea.TimeBuyingTotalMax != null
                            && _timeToBuy > alarmArea.TimeBuyingTotalMax))
                    {
                        int missingTime =
                            alarmArea.TimeBuyingMaxDuration != null
                            && _timeToBuy > alarmArea.TimeBuyingMaxDuration.Value
                                ? alarmArea.TimeBuyingMaxDuration.Value - _timeToBuy
                            // ReSharper disable once PossibleInvalidOperationException, it is checked in condition above.
                                : alarmArea.TimeBuyingTotalMax.Value - _timeToBuy;

                        // Time to buy is bigger then max. allowed
                        Events.ProcessEvent(
                            new EventAlarmAreaTimeBuyingFailed(
                                idAlarmArea,
                                (byte)AlarmAreaActionResult.FailedBoughtTimeDurationExceeded,
                                _guidLogin,
                                _guidPerson,
                                _timeToBuy,
                                missingTime,
                                GetUnsetSources().ToArray()));

                        AlarmAreaUnsetResult.OnFailed(
                            AlarmAreaActionResult.FailedBoughtTimeDurationExceeded,
                            _timeToBuy,
                            missingTime);

                        return false;
                    }

                    return true;
                }

                if (_guidPerson != Guid.Empty)
                    // Check if user have rights to unset
                    if (!AlarmAreaAccessRightsManager.Singleton.CheckRigthsToUnset(
                        _guidPerson,
                        idAlarmArea)
                        && (!AlarmAreaAccessRightsManager.Singleton.CheckRigthsToTimeBuying(
                            _guidPerson,
                            idAlarmArea)
                            || !alarmAreaController.ProvideUnsetForOnlyTimeBuyingAccessRights))
                    {
                        // Time buying is not enabled by ACL
                        Events.ProcessEvent(
                            new EventAlarmAreaTimeBuyingFailed(
                                idAlarmArea,
                                (byte)AlarmAreaActionResult.FailedInsufficientRights,
                                _guidLogin,
                                _guidPerson,
                                _timeToBuy,
                                int.MinValue,
                                GetUnsetSources().ToArray()));

                        AlarmAreaUnsetResult.OnFailed(
                            AlarmAreaActionResult.FailedInsufficientRights,
                            _timeToBuy,
                            int.MinValue);

                        return false;
                    }

                return true;
            }

            private void OnTimeBuyingSucceeded(
                Guid idAlarmArea,
                bool nonAcknowledgedAlarmDuringSetPeriod)
            {
                if (SetUnsetParams.IdCardReader != Guid.Empty)
                    Events.ProcessEvent(
                        EventAlarmAreaBoughtTimeChanged
                            .EventAlarmAreaBoughtTimeChangedFromCardReader(
                                idAlarmArea,
                                SetUnsetParams.IdCardReader,
                                SetUnsetParams.IdCard,
                                SetUnsetParams.IdPerson,
                                _timeToBuy,
                                _remainingTime));
                else
                    Events.ProcessEvent(
                        EventAlarmAreaBoughtTimeChanged
                            .EventAlarmAreaBoughtTimeChangedFromClient(
                                idAlarmArea,
                                _guidLogin,
                                _guidPerson,
                                _timeToBuy,
                                _remainingTime));

                AlarmAreaUnsetResult.OnSucceded(
                    _timeToBuy,
                    _remainingTime,
                    nonAcknowledgedAlarmDuringSetPeriod);
            }
        }


        private class OnBoughtTimeExpiredRequest : WaitableProcessingRequest<TAlarmAreaController>
        {
            private readonly int _lastBoughtTime;
            private readonly int _totalBoughtTime;

            public OnBoughtTimeExpiredRequest(
                int lastBoughtTime,
                int totalBoughtTime)
            {
                _lastBoughtTime = lastBoughtTime;
                _totalBoughtTime = totalBoughtTime;

            }

            protected override void ExecuteInternal(TAlarmAreaController alarmAreaController)
            {
                // Check state of activation object if is present
                if (alarmAreaController.HasObjectForAutomaticActivation()
                    && !alarmAreaController.IsObjectForAutomaticActivationActivated())
                {
                    return;
                }

                var idAlarmArea = alarmAreaController.Id;

                Events.ProcessEvent(
                    new EventAlarmAreaBoughtTimeExpired(
                        idAlarmArea,
                        _lastBoughtTime,
                        _totalBoughtTime));

                alarmAreaController.SetAlarmArea(
                    false,
                    new AlarmAreas.SetUnsetParams(
                        Guid.Empty,
                        new AccessDataBase(),
                        Guid.Empty,
                        true,
                        false));

                AlarmAreas.Singleton.DeleteFromTimeBuyingRegistry(idAlarmArea);
            }
        }


        private abstract class AQueryRequest : WaitableProcessingRequest<TAlarmAreaController>
        {
            public bool Result
            {
                get;
                private set;
            }

            protected abstract bool Evaluate(TAlarmAreaController alarmAreaController);

            protected override void ExecuteInternal(TAlarmAreaController alarmAreaController)
            {
                if (alarmAreaController == null)
                    throw new ArgumentNullException("Inside AQueryRequest");

                Result = Evaluate(alarmAreaController);
            }
        }
        private class IsTimeBuyingEnabledQueryRequest : AQueryRequest
        {
            protected override bool Evaluate(TAlarmAreaController alarmAreaController)
            {
                return
                    alarmAreaController.IsObjectForForcedTimeBuyingActivated()
                    && (!alarmAreaController.HasObjectForAutomaticActivation()
                        || alarmAreaController.IsObjectForAutomaticActivationActivated());
            }
        }

        private class ProvideUnsetForOnlyTimeBuyingAccessRightsQueryRequest : AQueryRequest
        {
            protected override bool Evaluate(TAlarmAreaController alarmAreaController)
            {
                return alarmAreaController.ProvideUnsetForOnlyTimeBuyingAccessRights;
            }
        }

        private class HasInactiveObjectForForcedTimeBuyingQueryRequest : AQueryRequest
        {
            protected override bool Evaluate(TAlarmAreaController alarmAreaController)
            {
                return
                    !alarmAreaController.IsObjectForForcedTimeBuyingActivated();
            }
        }

        private class InitRequest : WaitableProcessingRequest<TAlarmAreaController>
        {
            private readonly DB.AlarmArea _alarmArea;

            public InitRequest(DB.AlarmArea alarmArea)
            {
                _alarmArea = alarmArea;
            }

            protected override void ExecuteInternal(TAlarmAreaController alarmAreaController)
            {
                Events.ProcessEvent(
                    new EventAlarmAreaActivationStateChanged(
                        State.Unset,
                        _alarmArea.IdAlarmArea,
                        Guid.Empty,
                        Guid.Empty,
                        Guid.Empty));

                Events.ProcessEvent(
                    new EventAlarmAreaAlarmStateInfo(
                        _alarmArea.IdAlarmArea,
                        State.Normal));

                alarmAreaController.BeginInit(_alarmArea);

                alarmAreaController._disabledRecalculatingTimeBuyingMatrixState = true;

                if (_alarmArea.ObjForAutomaticActId != null)
                    alarmAreaController._objForAutomaticAct.Init(
                        _alarmArea.ObjForAutomaticActId,
                        _alarmArea);

                if (_alarmArea.ObjForForcedTimeBuyingId != null)
                    alarmAreaController._objForForcedTimeBuying.Init(
                        _alarmArea.ObjForForcedTimeBuyingId,
                        _alarmArea);

                alarmAreaController._disabledRecalculatingTimeBuyingMatrixState = false;

                alarmAreaController.RecalculateTimeBuyingMatrixState();
            }
        }

        private class UpdateRequest : WaitableProcessingRequest<TAlarmAreaController>
        {
            private readonly DB.AlarmArea _alarmArea;

            public UpdateRequest(DB.AlarmArea alarmArea)
            {
                _alarmArea = alarmArea;
            }

            protected override void ExecuteInternal(TAlarmAreaController alarmAreaController)
            {
                alarmAreaController.BeginUpdate(_alarmArea);

                alarmAreaController._disabledRecalculatingTimeBuyingMatrixState = true;

                if (alarmAreaController._objForAutomaticAct.ObjId != _alarmArea.ObjForAutomaticActId
                    || alarmAreaController._objForAutomaticAct.IsInverted != _alarmArea.IsInvertedObjForAutomaticAct
                    || alarmAreaController.AlarmAreaStateAndSettings.AutomaticDeactive != _alarmArea.AutomaticDeactive)
                {
                    alarmAreaController.RetrySetAlarmAreaByObjectForAa = false;
                    alarmAreaController._objForAutomaticAct.Update(_alarmArea);
                }

                if (alarmAreaController._objForForcedTimeBuying.ObjId != _alarmArea.ObjForForcedTimeBuyingId
                    || alarmAreaController._objForForcedTimeBuying.IsInverted != _alarmArea.IsInvertedObjForForcedTimeBuying)
                {
                    alarmAreaController._objForForcedTimeBuying.Update(_alarmArea);
                }

                alarmAreaController._disabledRecalculatingTimeBuyingMatrixState = false;

                alarmAreaController.RecalculateTimeBuyingMatrixState();
            }
        }

        protected AlarmAreaStateAndSettings AlarmAreaStateAndSettings;

        private readonly AOnOffObjectHandler _objForAutomaticAct;
        private readonly AOnOffObjectHandler _objForForcedTimeBuying;

        private bool _retrySetAlarmAreaByObjectForAa;

        protected readonly ThreadPoolQueue<WaitableProcessingRequest<TAlarmAreaController>, TAlarmAreaController> _processingQueue;

        private readonly BoolVariable _activated;

        protected IBoolExpression Activated { get { return _activated; } }

        protected AAlarmAreaController(AlarmAreaStateAndSettings alarmAreaStateAndSettings)
        {
            ActivationState = State.Unset;
            AlarmAreaStateAndSettings = alarmAreaStateAndSettings;

            _activated = new BoolVariable(false);

            _objForAutomaticAct = new OnOffObjectForAutomaticActivationHandler(this);
            _objForForcedTimeBuying = new OnOffObjectForForcedTimeBuyingHandler(this);

            _processingQueue =
                new ThreadPoolQueue<WaitableProcessingRequest<TAlarmAreaController>, TAlarmAreaController>(
                    ThreadPoolGetter.Get(),
                    This);
        }

        public bool IsObjectForForcedTimeBuyingActivated()
        {
            return AlarmAreaStateAndSettings.DbObject.AlwaysProvideUnsetForTimeBuying
                   || _objForForcedTimeBuying.IsActivated();
        }

        public bool HasObjectForAutomaticActivation()
        {
            return _objForAutomaticAct.ObjId.HasValue;
        }

        public bool IsObjectForAutomaticActivationActivated()
        {
            return _objForAutomaticAct.IsActivated();
        }

        protected bool RetrySetAlarmAreaByObjectForAa
        {
            get { return _retrySetAlarmAreaByObjectForAa; }

            private set
            {
                if (_retrySetAlarmAreaByObjectForAa == value)
                    return;

                _retrySetAlarmAreaByObjectForAa = value;

                if (_objForAutomaticAct == null
                    || _objForAutomaticAct.ObjId == null)
                {
                    return;
                }

                if (_retrySetAlarmAreaByObjectForAa)
                {
                    AlarmsManager.Singleton.AddAlarm(
                        new AlarmAreaSetByOnOffObjectFailedAlarm(
                            Id,
                            new IdAndObjectType(
                                _objForAutomaticAct.ObjId,
                                _objForAutomaticAct.ObjType)));


                    AlarmAreaStateAndSettings.SetSpecialOutputSetByObjectForAaFailedOn();

                    return;
                }

                AlarmsManager.Singleton.StopAlarm(
                    AlarmAreaSetByOnOffObjectFailedAlarm.CreateAlarmKey(
                        Id,
                        new IdAndObjectType(
                            _objForAutomaticAct.ObjId,
                            _objForAutomaticAct.ObjType)));

                AlarmAreaStateAndSettings.SetSpecialOutputSetByObjecForAaFailedOff();
            }
        }

        private bool ProvideUnsetForOnlyTimeBuyingAccessRights
        {
            get
            {
                return NotForcedTimeBuyingProvideOnlyUnset
                       && ((HasObjectForAutomaticActivation()
                            && !IsObjectForAutomaticActivationActivated())
                           || (!HasObjectForAutomaticActivation()
                               && !IsObjectForForcedTimeBuyingActivated()
                               && !AlarmAreas.Singleton.AlarmAreaRestrictivePolicyForTimeBuying));
            }
        }

        public Guid Id
        {
            get { return AlarmAreaStateAndSettings.GuidAlarmArea; }
        }

        [NotNull]
        public abstract IBoolExpression InSabotage { get; }

        [NotNull]
        public abstract IBoolExpression AlarmNotAcknowledged { get; }

        [NotNull]
        public abstract IBoolExpression AnySensorInAlarm { get; }

        [NotNull]
        public abstract IBoolExpression AnySensorNotAcknowledged { get; }

        [NotNull]
        public abstract IBoolExpression AnySensorTemporarilyBlocked { get; }

        [NotNull]
        public abstract IBoolExpression AnySensorPermanentlyBlocked { get; }

        [NotNull]
        public abstract IBoolExpression NotAcknowledged { get; }

        public State ActivationState { get; private set; }

        public State AlarmState
        {
            get
            {
                return InAlarm.State == true
                    ? State.Alarm
                    : State.Normal;
            }
        }

        [NotNull]
        public abstract IBoolExpression InAlarm { get; }

        [NotNull]
        public abstract IBoolExpression SirenOutput { get; }

        public abstract void OnAlarmAcknowledged(AlarmsManager.IActionSources acknowledgeSources);

        public bool NotForcedTimeBuyingProvideOnlyUnset
        {
            get { return AlarmAreaStateAndSettings.DbObject.NotForcedTimeBuyingProvideOnlyUnset; }
        }


        public void Update(DB.AlarmArea alarmArea)
        {
            new UpdateRequest(alarmArea).EnqueueAsync(_processingQueue);
        }

        protected abstract void BeginUpdate(DB.AlarmArea alarmArea);

        public abstract void OnUnset();

        public abstract bool HasAnySensorActive();

        public abstract int GetSensorId(Guid idInput);

        public abstract SensorPurpose GetSensorPurpose(Guid idInput);

        //public abstract bool SetAlarmArea(
        //    bool setSynchronously,
        //    bool noPrewarning,
        //    [NotNull]
        //    ICollection<Guid> collection);

        public bool SetAlarmArea(
            bool setSynchronously,
            AlarmAreas.SetUnsetParams setUnsetParams)
        {
            var alarmAreaSettingsRequestItem =
                CreateSetAlarmAreaRequestItem(setUnsetParams);

            if (alarmAreaSettingsRequestItem == null)
                return false;

            Events.ProcessEvent(
                new EventAlarmAreaSendRequestedActivationState(
                    AlarmAreaStateAndSettings.GuidAlarmArea,
                    setUnsetParams.UnconditionalSet
                        ? State.UnconditionaSet
                        : State.Set));

            if (setSynchronously)
            {
                alarmAreaSettingsRequestItem.EnqueueSync(_processingQueue);
                alarmAreaSettingsRequestItem.WaitForCompletion();
            }
            else
                alarmAreaSettingsRequestItem.EnqueueAsync(_processingQueue);

            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => "bool AlarmAreaSettings.SetAlarmArea return true");

            AlarmAreas.Singleton.TryCancelBoughtTime(AlarmAreaStateAndSettings.Id);

            return true;
        }

        protected abstract WaitableProcessingRequest<TAlarmAreaController> CreateSetAlarmAreaRequestItem(AlarmAreas.SetUnsetParams setUnsetParams);

        public void OnActivatedObjectForAa()
        {
            if (_objForAutomaticAct == null
                || _objForAutomaticAct.ObjId == null)
            {
                return;
            }

            RecalculateTimeBuyingMatrixState();

            var setAlarmAreaSucceded =
                SetAlarmArea(
                    false,
                    new AlarmAreas.SetUnsetParams(
                        Guid.Empty,
                        new AccessDataBase(),
                        _objForAutomaticAct.ObjId.Value,
                        AlarmAreaStateAndSettings.AutomaticActivationMode == AlarmAreaAutomaticActivationMode.UnconditionalSet,
                        false));

            if (!setAlarmAreaSucceded)
            {
                RetrySetAlarmAreaByObjectForAa = true;
                return;
            }

            RetrySetAlarmAreaByObjectForAa = false;
        }

        public void OnDeactivatedObjectForAa()
        {
            if (_objForAutomaticAct == null
                || _objForAutomaticAct.ObjId == null)
            {
                return;
            }

            RecalculateTimeBuyingMatrixState();

            if (AlarmAreaStateAndSettings.AutomaticDeactive)
            {
                CreateUnsetAlarmAreaRequestItem(
                    new UnsetAlarmAreaExecutor(
                        new DummyAlarmAreaUnsetResult(),
                        Guid.Empty,
                        Guid.Empty,
                        0,
                        new AlarmAreas.SetUnsetParams(
                            Guid.Empty,
                            new AccessDataBase(),
                            _objForAutomaticAct.ObjId.Value,
                            false,
                            false))).EnqueueAsync(_processingQueue);

                // Send requested activation state to server
                Events.ProcessEvent(
                    new EventAlarmAreaSendRequestedActivationState(
                        AlarmAreaStateAndSettings.Id,
                        State.Unset,
                        Guid.Empty,
                        Guid.Empty));
            }

            RetrySetAlarmAreaByObjectForAa = false;
        }

        public void OnActivatedObjectForTimeBuing()
        {
            RecalculateTimeBuyingMatrixState();
        }

        public void OnDeactivatedObjectForTimeBuing()
        {
            RecalculateTimeBuyingMatrixState();
        }

        private TimeBuyingMatrixState _timeBuyingMatrixState = TimeBuyingMatrixState.O4TBA_ON;

        private bool _disabledRecalculatingTimeBuyingMatrixState;

        private void RecalculateTimeBuyingMatrixState()
        {
            if (_disabledRecalculatingTimeBuyingMatrixState)
                return;

            _timeBuyingMatrixState = RecalculateTimeBuyingMatrixStateCore();
            SendTimeBuyingMatrixStateToServer();
        }

        public void SendTimeBuyingMatrixStateToServer()
        {
            Events.ProcessEvent(
                new EventTimeBuyingMatrixStateInfo(
                    Id,
                    _timeBuyingMatrixState));
        }

        private TimeBuyingMatrixState RecalculateTimeBuyingMatrixStateCore()
        {
            if (HasObjectForAutomaticActivation())
            {
                if (IsObjectForAutomaticActivationActivated()
                    && IsObjectForForcedTimeBuyingActivated())
                {
                    return TimeBuyingMatrixState.O4AA_ON_AND_O4TBA_ON;
                }

                if (IsObjectForAutomaticActivationActivated()
                    && !IsObjectForForcedTimeBuyingActivated())
                {
                    return TimeBuyingMatrixState.O4AA_ON_AND_O4TBA_OFF;
                }

                if (!IsObjectForAutomaticActivationActivated()
                    && IsObjectForForcedTimeBuyingActivated())
                {
                    return TimeBuyingMatrixState.O4AA_OFF_AND_O4TBA_ON;
                }

                if (!IsObjectForAutomaticActivationActivated()
                    && !IsObjectForForcedTimeBuyingActivated())
                {
                    return TimeBuyingMatrixState.O4AA_OFF_AND_O4TBA_OFF;
                }
            }

            if (IsObjectForForcedTimeBuyingActivated())
                return TimeBuyingMatrixState.O4TBA_ON;

            return TimeBuyingMatrixState.O4TBA_OFF;
        }

        protected class DummyAlarmAreaUnsetResult : AlarmAreaStateAndSettings.IAlarmAreaUnsetResult
        {
            public void OnFailed(AlarmAreaActionResult alarmAreaActionResult, int timeToBuy, int remainingTime)
            {
            }

            public void OnSucceded(
                int timeToBuy,
                int remainingTime,
                bool nonAcknowledgedAlarmDuringSetPeriod)
            {
            }
        }

        public void UnsetAlarmArea(
            AlarmAreaStateAndSettings.IAlarmAreaUnsetResult alarmAreaUnsetResult,
            bool unsetSynchronously,
            Guid guidLogin,
            Guid guidPerson,
            int timeToBuy,
            AlarmAreas.SetUnsetParams setUnsetParams)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "void AlarmAreaSettings.UnsetAlarmArea(Guid guidLogin, Guid guidPerson, params Guid[] sources): [{0}]",
                    Log.GetStringFromParameters(guidLogin, guidPerson, setUnsetParams)));

            // Send requested activation state to server
            Events.ProcessEvent(
                new EventAlarmAreaSendRequestedActivationState(
                    AlarmAreaStateAndSettings.Id,
                    State.Unset,
                    guidLogin,
                    guidPerson));

            var alarmAreaSettingsRequestItem =
                CreateUnsetAlarmAreaRequestItem(
                    new UnsetAlarmAreaExecutor(
                        alarmAreaUnsetResult,
                        guidLogin,
                        guidPerson,
                        timeToBuy,
                        setUnsetParams));

            if (unsetSynchronously)
            {
                alarmAreaSettingsRequestItem.EnqueueSync(_processingQueue);
                alarmAreaSettingsRequestItem.WaitForCompletion();
            }
            else
                alarmAreaSettingsRequestItem.EnqueueAsync(_processingQueue);
        }

        protected abstract WaitableProcessingRequest<TAlarmAreaController> CreateUnsetAlarmAreaRequestItem(
            [NotNull]
            UnsetAlarmAreaExecutor unsetAlarmAreaExecutor);

        public void SendEventAlarmAreaSetUnsetByObjectForAa(Guid guidObject, bool set)
        {
            Events.ProcessEvent(
                set
                    ? (EventParameters.EventParameters)
                        new EventSetAlarmAreaByObjectForAutomaticActivation(
                            Id,
                            guidObject,
                            _objForAutomaticAct.ObjType)
                    : new EventUnsetAlarmAreaByObjectForAutomaticActivation(
                        Id,
                        guidObject,
                        _objForAutomaticAct.ObjType));
        }

        public void ChangeActivationState(
            State activationState,
            [CanBeNull]
            AlarmAreas.SetUnsetParams setUnsetParams)
        {
            ActivationState = activationState;

            _activated.SetState(ActivationState == State.TemporaryUnsetExit
                                || ActivationState == State.Set
                                || ActivationState == State.TemporaryUnsetEntry);

            //internal activation state change notification
            AlarmAreas.Singleton.SaveAlarmAreaActivationState(
                AlarmAreaStateAndSettings.Id,
                ActivationState);

            OnActivationStateChanged(setUnsetParams);
        }

        protected virtual void OnActivationStateChanged(
            [CanBeNull]
            AlarmAreas.SetUnsetParams setUnsetParams)
        {
            AlarmAreaStateAndSettings.OnActivationStateChanged(
                ActivationState,
                setUnsetParams);
        }

        public virtual void Dispose()
        {
            OnUnset();

            RetrySetAlarmAreaByObjectForAa = false;
            _objForAutomaticAct.Close();

            _objForForcedTimeBuying.Close();

            _processingQueue.Dispose();
        }

        public abstract void SendSabotageStateInfoToServer();

        protected abstract TAlarmAreaController This
        {
            get;
        }

        public void OnBoughtTimeExpired(
            int lastBoughtTime,
            int totalBoughtTime)
        {
            new OnBoughtTimeExpiredRequest(
                lastBoughtTime,
                totalBoughtTime).EnqueueAsync(_processingQueue);
        }

        public bool HasInactiveObjectForForcedTimeBuying()
        {
            var request = new HasInactiveObjectForForcedTimeBuyingQueryRequest();

            request.EnqueueSync(_processingQueue);
            request.WaitForCompletion();

            return request.Result;
        }

        public bool ProvideUnsetForOnlyTimeBuyingAccessRightsSync()
        {
            var request = new ProvideUnsetForOnlyTimeBuyingAccessRightsQueryRequest();

            request.EnqueueSync(_processingQueue);
            request.WaitForCompletion();

            return request.Result;
        }

        public bool IsTimeBuyingEnabled()
        {
            var request = new IsTimeBuyingEnabledQueryRequest();

            request.EnqueueSync(_processingQueue);
            request.WaitForCompletion();
            return request.Result;
        }

        public abstract void OnSensorAlarmAcknowledged(
            Guid idSensor,
            AlarmsManager.IActionSources acknowledgeSources);

        public abstract void OnSensorTamperAlarmAcknowledged(
            Guid idSensor,
            AlarmsManager.IActionSources acknowledgeSources);

        public abstract void Unconfigure(DB.AlarmArea newDbObject);

        protected abstract void BeginInit(DB.AlarmArea alarmArea);

        public void Init(DB.AlarmArea alarmArea)
        {
            new InitRequest(alarmArea).EnqueueAsync(_processingQueue);
        }

        public abstract ICollection<ISensorStateAndSettings> GetSensors();

        public abstract void AcknowledgeAllSensorAlarms(
            Guid idCardReader,
            AccessDataBase accessData);

        public abstract void SendBlockingTypeForAllSensors();

        public abstract void SendStateForAllSensors();

        public abstract void SetSensorBlockingType(
            Guid idInput,
            SensorBlockingType sensorBlockingType);
    }
}

