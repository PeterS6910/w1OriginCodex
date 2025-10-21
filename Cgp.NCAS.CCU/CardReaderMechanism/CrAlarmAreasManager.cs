using System;
using System.Collections.Generic;

using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.AlarmArea;
using Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes;
using Contal.IwQuick;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.CCU.CardReaderMechanism
{
    internal class CrAlarmAreasManager :
        ADisposable,
        IAlarmAreaEventHandler
    {
        public class CrAlarmAreaInfo :
            ADisposable,
            IEquatable<CrAlarmAreaInfo>,
            ACardReaderSettings.IAlarmAreaStateProvider
        {
            public DB.AlarmArea AlarmArea
            {
                get
                {
                    return _alarmAreaStateAndSettings.DbObject;
                }
            }

            public Guid IdAlarmArea
            {
                get;
                private set;
            }

            private readonly CrAlarmAreasManager _crAlarmAreasManager;

            private readonly IAlarmAreaStateAndSettings _alarmAreaStateAndSettings;

            private bool _isSettable;
            private bool _isUnconditionalSettable;
            private bool _isUnsettable;
            private bool _isEventlogEnabled;
            private bool _isImplicit;

            public bool IsEventlogEnabled
            {
                get
                {
                    return
                        _isEventlogEnabled
                        && AlarmArea.EnableEventlogsInCR;
                }
            }

            public bool IsImplicit
            {
                get { return _isImplicit; }
            }


            public State ActivationState
            {
                get { return _alarmAreaStateAndSettings.ActivationState; }
            }

            public State AlarmState
            {
                get { return _alarmAreaStateAndSettings.AlarmState; }
            }

            public bool IsSet
            {
                get { return AlarmAreas.AlarmAreaIsSet(ActivationState); }
            }

            public bool IsUnset
            {
                get { return !AlarmAreas.AlarmAreaIsSet(ActivationState); }
            }

            public CrAlarmAreaInfo(
                CrAlarmAreasManager crAlarmAreasManager,
                IAlarmAreaStateAndSettings alarmAreaStateAndSettings,
                DB.AACardReader aaCardReader)
            {
                _crAlarmAreasManager = crAlarmAreasManager;
                _alarmAreaStateAndSettings = alarmAreaStateAndSettings;

                IdAlarmArea = alarmAreaStateAndSettings.Id;

                _isSettable = aaCardReader.AASet || aaCardReader.AAUnconditionalSet;
                _isUnconditionalSettable = aaCardReader.AAUnconditionalSet;
                _isUnsettable = aaCardReader.AAUnset;
                _isEventlogEnabled = aaCardReader.EnableEventlog;
                _isImplicit = !aaCardReader.PermanentlyUnlock;

                if (_isImplicit)
                    _crAlarmAreasManager._implicitCrAlarmAreaInfo = this;

                _crAlarmAreasManager._crAlarmAreaEventHandler.OnAlarmAreaAdded(this);
            }

            public bool OnAACardReaderChanged(
                DB.AACardReader aaCardReader, 
                out bool isImplicitChanged)
            {
                bool change = false;
                var isSettable = aaCardReader.AASet || aaCardReader.AAUnconditionalSet;

                if (_isSettable != isSettable)
                {
                    _isSettable = isSettable;
                    change = true;
                }

                var isUnconditionalSettable = aaCardReader.AAUnconditionalSet;

                if (_isUnconditionalSettable != isUnconditionalSettable)
                {
                    _isUnconditionalSettable = isUnconditionalSettable;
                    change = true;
                }

                var isUnsettable = aaCardReader.AAUnset;

                if (_isUnsettable != isUnsettable)
                {
                    _isUnsettable = isUnsettable;
                    change = true;
                }

                var isEventlogEnabled = aaCardReader.EnableEventlog;

                if (_isEventlogEnabled != isEventlogEnabled)
                {
                    _isEventlogEnabled = isEventlogEnabled;
                    change = true;
                }

                var isImplicit = !aaCardReader.PermanentlyUnlock;

                if (_isImplicit == isImplicit)
                {
                    isImplicitChanged = false;
                    return change;
                }

                isImplicitChanged = true;
                _isImplicit = isImplicit;

                return true;
            }

            public bool IsTimeBuyingPossible
            {
                get
                {
                    var alarmArea = AlarmArea;

                    return
                        alarmArea != null
                        && alarmArea.TimeBuyingEnabled
                        && (!alarmArea.TimeBuyingOnlyInPrewarning
                            || ActivationState == State.Prewarning);
                }
            }

            public bool IsCrReportingEnabled
            {
                get
                {
                    return AlarmAreas.Singleton.IsCrReportingEnabled(IdAlarmArea);
                }
            }

            protected override void InternalDispose(bool isExplicitDispose)
            {
                if (!isExplicitDispose)
                    return;

                _crAlarmAreasManager._crAlarmAreaEventHandler.OnAlarmAreaRemoved(this);
            }

            public bool Equals(CrAlarmAreaInfo otherCrAlarmAreaInfo)
            {
                return
                    IdAlarmArea.Equals(otherCrAlarmAreaInfo.IdAlarmArea);
            }

            public override int GetHashCode()
            {
                return
                    IdAlarmArea.GetHashCode()
                    ^ _crAlarmAreasManager.IdCardReader.GetHashCode();
            }

            public override string ToString()
            {
                return _alarmAreaStateAndSettings.AlarmAreaName ?? string.Empty;
            }

            public bool IsUnsettable
            {
                get { return _isUnsettable; }
            }

            public bool IsSettable
            {
                get { return _isSettable; }
            }

            public bool IsUnconditionalSettable
            {
                get { return _isUnconditionalSettable; }
            }

            public bool IsMarked
            {
                get;
                set;
            }

            public bool IsAnySensorInAlarm
            {
                get
                {
                    var anySensorInAlarm = _alarmAreaStateAndSettings.AnySensorInAlarm;

                    return anySensorInAlarm != null
                           && anySensorInAlarm.State == true;
                }
            }

            public bool IsInSabotage
            {
                get
                {
                    var anySensorInTamper = _alarmAreaStateAndSettings.InSabotage;

                    return anySensorInTamper != null
                           && anySensorInTamper.State == true;
                }
            }

            public bool IsAnySensorNotAcknowledged
            {
                get
                {
                    var anySensorNotAcknowledged = _alarmAreaStateAndSettings.AnySensorNotAcknowledged;

                    return anySensorNotAcknowledged != null
                           && anySensorNotAcknowledged.State == true;
                }
            }

            public bool IsAnySensorPermanentlyBlocked
            {
                get
                {
                    var anySensorPermanentlyBlocked = _alarmAreaStateAndSettings.AnySensorPermanentlyBlocked;

                    return anySensorPermanentlyBlocked != null
                           && anySensorPermanentlyBlocked.State == true;
                }
            }

            public bool IsAnySensorTemporarilyBlocked
            {
                get
                {
                    var anySensorTemporarilyBlocked = _alarmAreaStateAndSettings.AnySensorTemporarilyBlocked;

                    return anySensorTemporarilyBlocked != null
                           && anySensorTemporarilyBlocked.State == true;
                }
            }

            public bool IsNotAcknowledged
            {
                get
                {
                    var notAcknowledged = _alarmAreaStateAndSettings.NotAcknowledged;

                    return notAcknowledged != null
                           && notAcknowledged.State == true;
                }
            }

            public void AddSensorsEventHandler(IAlarmAreaSensorsEventHandler sensorsEventHandler)
            {
                _alarmAreaStateAndSettings.AddSensorsEventHandler(sensorsEventHandler);
            }

            public void RemoveSensorsEventHandler(IAlarmAreaSensorsEventHandler sensorsEventHandler)
            {
                _alarmAreaStateAndSettings.RemoveSensorsEventHandler(sensorsEventHandler);
            }

            public void AcknowledgeAllSensorAlarms(
                Guid idCardReader,
                AccessDataBase accessData)
            {
                _alarmAreaStateAndSettings.AcknowledgeAllSensorAlarms(
                    idCardReader,
                   accessData);
            }
        }

        private readonly ACardReaderSettings _cardReaderSettings;

        public Guid IdCardReader
        {
            get;
            private set;
        }

        public CrAlarmAreaInfo ImplicitCrAlarmAreaInfo
        {
            get { return _implicitCrAlarmAreaInfo; }
        }

        public int AlarmAreaCount
        {
            get { return _observedAlarmAreas.Count; }
        }

        private ICrAlarmAreaEventHandler _crAlarmAreaEventHandler;

        private readonly SyncDictionary<Guid, CrAlarmAreaInfo> _observedAlarmAreas =
            new SyncDictionary<Guid, CrAlarmAreaInfo>();

        private CrAlarmAreaInfo _implicitCrAlarmAreaInfo;

        public CrAlarmAreasManager(
            ACardReaderSettings cardReaderSettings,
            IEnumerable<DB.AACardReader> aaCardReaders)
        {
            _cardReaderSettings = cardReaderSettings;
            _crAlarmAreaEventHandler = new DummyEventHandler();

            IdCardReader = cardReaderSettings.Id;

            AddAlarmAreas(aaCardReaders);

            EventsForCardReadersDispatcher.Singleton.AlarmAreaMarkingChanged +=
                OnAlarmAreaMarkingChanged;

            AlarmAreas.Singleton.EventHandlerGroup.Add(this);
        }

        private void OnAlarmAreaMarkingChanged(
            Guid idAlarmArea,
            bool isMarked)
        {
            _observedAlarmAreas.TryGetValue(
                idAlarmArea,
                (key, found, alarmAreaInfo) =>
                {
                    if (found)
                    {
                        alarmAreaInfo.IsMarked = isMarked;
                        _crAlarmAreaEventHandler.OnAlarmAreaMarkingChanged(alarmAreaInfo);
                    }
                });
        }

        public void UnconfigureAaCardReader(
            DB.AACardReader aaCardReader,
            bool update)
        {
            _observedAlarmAreas.Remove(
                aaCardReader.GuidAlarmArea,
                (Guid key, CrAlarmAreaInfo alarmAreaInfo, out bool continueInRemove) =>
                {
                    continueInRemove = !update;
                },
                UnconfigureAaCardReaderInternal);
        }

        private void UnconfigureAaCardReaderInternal(
            Guid key,
            bool removed,
            CrAlarmAreaInfo removedValue)
        {
            if (!removed)
                return;

            if (_implicitCrAlarmAreaInfo != null
                && _implicitCrAlarmAreaInfo.IdAlarmArea.Equals(removedValue.IdAlarmArea))
            {
                _implicitCrAlarmAreaInfo = null;
            }

            removedValue.Dispose();
        }

        public void ConfigureAaCardReader(DB.AACardReader aaCardReader)
        {
            var idAlarmArea = aaCardReader.GuidAlarmArea;

            _observedAlarmAreas.GetOrAddValue(
                idAlarmArea,
                key => new CrAlarmAreaInfo(
                    this,
                    AlarmAreas.Singleton.GetAlarmAreaSettings(idAlarmArea),
                    aaCardReader),
                (key, alarmAreaInfo, newlyAdded) =>
                {
                    if (newlyAdded)
                        return;

                    bool changedIsImplicit;

                    if (!alarmAreaInfo.OnAACardReaderChanged(
                        aaCardReader,
                        out changedIsImplicit))
                    {
                        return;
                    }

                    if (changedIsImplicit)
                        if (alarmAreaInfo.IsImplicit)
                            _implicitCrAlarmAreaInfo = alarmAreaInfo;
                        else
                            if (_implicitCrAlarmAreaInfo != null
                                && _implicitCrAlarmAreaInfo.IdAlarmArea.Equals(idAlarmArea))
                            {
                                _implicitCrAlarmAreaInfo = null;
                            }

                    _crAlarmAreaEventHandler.OnAACardReaderRightsChanged(alarmAreaInfo);
                });
        }

        private void AddAlarmAreas(IEnumerable<DB.AACardReader> aaCardReaders)
        {
            if (aaCardReaders == null)
                return;

            foreach (var aaCardReader in aaCardReaders)
            {
                _observedAlarmAreas.Add(
                    aaCardReader.GuidAlarmArea,
                    new CrAlarmAreaInfo(
                        this,
                        AlarmAreas.Singleton.GetAlarmAreaSettings(aaCardReader.GuidAlarmArea),
                        aaCardReader));
            }
        }

        public CrAlarmAreaInfo GetAlarmAreaInfo(Guid idAlarmArea)
        {
            CrAlarmAreaInfo result;

            _observedAlarmAreas.TryGetValue(
                idAlarmArea,
                out result);

            return result;
        }

        public void Attach(ICrAlarmAreaEventHandler crAlarmAreaEventHandler)
        {
            if (ReferenceEquals(
                _crAlarmAreaEventHandler,
                crAlarmAreaEventHandler))
            {
                return;
            }

            _crAlarmAreaEventHandler.OnDetached();

            _crAlarmAreaEventHandler = crAlarmAreaEventHandler;

            crAlarmAreaEventHandler.OnAttached(_observedAlarmAreas.ValuesSnapshot);
        }

        public void Detach(ICrAlarmAreaEventHandler crAlarmAreaEventHandler)
        {
            _crAlarmAreaEventHandler.OnDetached();

            _crAlarmAreaEventHandler = new DummyEventHandler();
        }

        protected override void InternalDispose(bool isExplicitDispose)
        {
            if (!isExplicitDispose)
                return;

            AlarmAreas.Singleton.EventHandlerGroup.Remove(this);

            EventsForCardReadersDispatcher.Singleton.AlarmAreaMarkingChanged -=
                OnAlarmAreaMarkingChanged;

            _implicitCrAlarmAreaInfo = null;

            _crAlarmAreaEventHandler.OnDetached();
            _crAlarmAreaEventHandler = null;

            foreach (var alarmAreaInfo in _observedAlarmAreas.ValuesSnapshot)
                alarmAreaInfo.Dispose();

            _observedAlarmAreas.Clear();
        }

        public bool Equals(CrAlarmAreasManager otherCrAlarmAreasManager)
        {
            return
                otherCrAlarmAreasManager != null
                && otherCrAlarmAreasManager.IdCardReader
                    .Equals(IdCardReader);
        }

        public class DummyEventHandler : ICrAlarmAreaEventHandler
        {
            public void OnAttached(ICollection<CrAlarmAreaInfo> observedAlarmAreas)
            {
            }

            public void OnActivationStateChanged(CrAlarmAreaInfo crAlarmAreaInfo)
            {
            }

            public void OnAlarmStateChanged(CrAlarmAreaInfo crAlarmAreaInfo)
            {
            }

            public void OnNotAcknolwedgedStateChanged(
                CrAlarmAreaInfo crAlarmAreaInfo,
                bool notAcknowledged)
            {
            }

            public void OnAlarmAreaAdded(CrAlarmAreaInfo crAlarmAreaInfo)
            {
            }

            public void OnAlarmAreaRemoved(CrAlarmAreaInfo crAlarmAreaInfo)
            {
            }

            public void OnAACardReaderRightsChanged(CrAlarmAreaInfo crAlarmAreaInfo)
            {
            }

            public void OnDetached()
            {
            }

            public void OnAlarmAreaMarkingChanged(CrAlarmAreaInfo crAlarmAreaInfo)
            {
            }

            public void OnAnySensorInAlarmChanged(
                CrAlarmAreaInfo crAlarmAreaInfo,
                bool value)
            {
            }

            public void OnAnySensorInTamperChanged(
                CrAlarmAreaInfo crAlarmAreaInfo,
                bool value)
            {
            }

            public void OnAnySensorNotAcknowledgedChanged(
                CrAlarmAreaInfo crAlarmAreaInfo,
                bool value)
            {
            }

            public void OnAnySensorTemporarilyBlockedChanged(
                CrAlarmAreaInfo crAlarmAreaInfo,
                bool value)
            {
            }

            public void OnAnySensorPermanentlyBlockedChanged(
                CrAlarmAreaInfo crAlarmAreaInfo,
                bool value)
            {
            }
        }

        bool IEquatable<IAlarmAreaEventHandler>.Equals(IAlarmAreaEventHandler other)
        {
            return Equals(other as CrAlarmAreasManager);
        }

        void IAlarmAreaEventHandler.OnActivationStateChanged(
            IAlarmAreaStateAndSettings alarmAreaStateAndSettings,
            State activationState)
        {
            CrAlarmAreaInfo crAlarmAreaInfo;

            if (!_observedAlarmAreas.TryGetValue(
                alarmAreaStateAndSettings.Id,
                out crAlarmAreaInfo))
            {
                return;
            }

            _crAlarmAreaEventHandler.OnActivationStateChanged(crAlarmAreaInfo);

            if (crAlarmAreaInfo.IsImplicit)
                _cardReaderSettings
                    .OnImplicitAlarmAreaActivationStateChanged(crAlarmAreaInfo);
        }

        void IAlarmAreaEventHandler.OnAlarmStateChanged(
            IAlarmAreaStateAndSettings alarmAreaStateAndSettings,
            State alarmState)
        {
            CrAlarmAreaInfo crAlarmAreaInfo;

            if (_observedAlarmAreas.TryGetValue(
                alarmAreaStateAndSettings.Id,
                out crAlarmAreaInfo))
            {
                _crAlarmAreaEventHandler.OnAlarmStateChanged(crAlarmAreaInfo);

                if (crAlarmAreaInfo.IsImplicit)
                    _cardReaderSettings
                        .UpdateRootScene();
            }
        }

        void IAlarmAreaEventHandler.OnSpecificCrReportingChanged(
            IAlarmAreaStateAndSettings alarmAreaStateAndSettings,
            bool? isCrReportingEnabled)
        {
            if (_implicitCrAlarmAreaInfo.IdAlarmArea.Equals(alarmAreaStateAndSettings.Id))
                _cardReaderSettings.UpdateRootScene();
        }

        void IAlarmAreaEventHandler.OnNotAcknolwedgedStateChanged(
            IAlarmAreaStateAndSettings alarmAreaStateAndSettings,
            bool notAcknowledged)
        {
            CrAlarmAreaInfo crAlarmAreaInfo;

            if (!_observedAlarmAreas.TryGetValue(
                alarmAreaStateAndSettings.Id,
                out crAlarmAreaInfo))
            {
                return;
            }

            _crAlarmAreaEventHandler.OnNotAcknolwedgedStateChanged(
                crAlarmAreaInfo,
                notAcknowledged);
        }

        void IAlarmAreaEventHandler.OnSetUnsetTimeout(
            IAlarmAreaStateAndSettings alarmAreaStateAndSettings,
            bool failedOperationIsSet)
        {
            if (_implicitCrAlarmAreaInfo != null &&
                _implicitCrAlarmAreaInfo.IdAlarmArea.Equals(alarmAreaStateAndSettings.Id))
                _cardReaderSettings
                    .SetImplicitExternalAlarmAreaHandshakeState(
                        failedOperationIsSet
                            ? ExternalAlarmAreaHandshakeState.FailureToSet
                            : ExternalAlarmAreaHandshakeState.FailureToUnset);
        }

        void IAlarmAreaEventHandler.OnSetUnsetStarted(
            IAlarmAreaStateAndSettings alarmAreaStateAndSettings,
            bool operationStartedIsSet)
        {
            if (_implicitCrAlarmAreaInfo != null &&
                _implicitCrAlarmAreaInfo.IdAlarmArea.Equals(alarmAreaStateAndSettings.Id))
                _cardReaderSettings
                    .SetImplicitExternalAlarmAreaHandshakeState(
                        operationStartedIsSet
                            ? ExternalAlarmAreaHandshakeState.WaitingForSet
                            : ExternalAlarmAreaHandshakeState.WaitingForUnset);
        }

        public void OnAnySensorInAlarmChanged(
            IAlarmAreaStateAndSettings alarmAreaStateAndSettings,
            bool value)
        {
            CrAlarmAreaInfo crAlarmAreaInfo;

            if (!_observedAlarmAreas.TryGetValue(
                alarmAreaStateAndSettings.Id,
                out crAlarmAreaInfo))
            {
                return;
            }

            _crAlarmAreaEventHandler.OnAnySensorInAlarmChanged(
                    crAlarmAreaInfo,
                    value);
        }

        public void OnAnySensorInTamperChanged(
            IAlarmAreaStateAndSettings alarmAreaStateAndSettings,
            bool value)
        {
            CrAlarmAreaInfo crAlarmAreaInfo;

            if (!_observedAlarmAreas.TryGetValue(
                alarmAreaStateAndSettings.Id,
                out crAlarmAreaInfo))
            {
                return;
            }

            _crAlarmAreaEventHandler.OnAnySensorInTamperChanged(
                    crAlarmAreaInfo,
                    value);
        }

        public void OnAnySensorNotAcknowledgedChanged(
            IAlarmAreaStateAndSettings alarmAreaStateAndSettings,
            bool value)
        {
            CrAlarmAreaInfo crAlarmAreaInfo;

            if (!_observedAlarmAreas.TryGetValue(
                alarmAreaStateAndSettings.Id,
                out crAlarmAreaInfo))
            {
                return;
            }

            _crAlarmAreaEventHandler.OnAnySensorNotAcknowledgedChanged(
                crAlarmAreaInfo,
                value);
        }

        public void OnAnySensorTemporarilyBlockedChanged(
            IAlarmAreaStateAndSettings alarmAreaStateAndSettings,
            bool value)
        {
            CrAlarmAreaInfo crAlarmAreaInfo;

            if (!_observedAlarmAreas.TryGetValue(
                alarmAreaStateAndSettings.Id,
                out crAlarmAreaInfo))
            {
                return;
            }

            _crAlarmAreaEventHandler.OnAnySensorTemporarilyBlockedChanged(
                crAlarmAreaInfo,
                value);
        }

        public void OnAnySensorPermanentlyBlockedChanged(
            IAlarmAreaStateAndSettings alarmAreaStateAndSettings,
            bool value)
        {
            CrAlarmAreaInfo crAlarmAreaInfo;

            if (!_observedAlarmAreas.TryGetValue(
                alarmAreaStateAndSettings.Id,
                out crAlarmAreaInfo))
            {
                return;
            }

            _crAlarmAreaEventHandler.OnAnySensorPermanentlyBlockedChanged(
                crAlarmAreaInfo,
                value);
        }
    }
}