using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.Alarms;
using Contal.Cgp.NCAS.CCU.CardReaderMechanism;
using Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes;
using Contal.Cgp.NCAS.CCU.EventParameters;
using Contal.Cgp.NCAS.Definitions;
using Contal.Cgp.NCAS.DoorStateMachine;
using Contal.Drivers.CardReader;
using Contal.IwQuick;
using Contal.IwQuick.CrossPlatform.Data;
using Contal.IwQuick.Sys;
using Contal.IwQuick.Threads;

using JetBrains.Annotations;

namespace Contal.Cgp.NCAS.CCU
{
    internal class DoorEnvironmentSettings : AQueuedStateAndSettingsObject<
        DoorEnvironmentSettings,
        DB.DoorEnvironment>
    {
        private abstract class InputChangedListener : IInputChangedListener
        {
            protected DoorEnvironmentSettings DoorEnvironmentSettings
            {
                get;
                private set;
            }

            protected InputChangedListener(DoorEnvironmentSettings doorEnvironmentSettings)
            {
                DoorEnvironmentSettings = doorEnvironmentSettings;
            }

            public override int GetHashCode()
            {
                // ReSharper disable once ImpureMethodCallOnReadonlyValueField
                return DoorEnvironmentSettings.Id.GetHashCode();
            }

            public abstract bool Equals(IInputChangedListener other);

            public abstract void OnInputChanged(
                Guid guidInput,
                State state);
        }

        private class SensorDoorOpenChangeListener : InputChangedListener
        {
            public SensorDoorOpenChangeListener(DoorEnvironmentSettings doorEnvironmentSettings)
                : base(doorEnvironmentSettings)
            {
            }

            public override bool Equals(IInputChangedListener other)
            {
                var otherListener = other as SensorDoorOpenChangeListener;

                return otherListener != null
                       && DoorEnvironmentSettings.Id
                    // ReSharper disable once ImpureMethodCallOnReadonlyValueField
                       .Equals(otherListener.DoorEnvironmentSettings.Id);
            }

            public override void OnInputChanged(
                Guid guidInput,
                State state)
            {
                DoorEnvironmentSettings.SensorOpenDoorsChanged(
                    guidInput,
                    state);
            }
        }

        private abstract class APushButtonChangedListener : InputChangedListener
        {
            private readonly Guid _idPushButton;
            private readonly byte _inputNumber;

            protected APushButtonChangedListener(
                DoorEnvironmentSettings doorEnvironmentSettings,
                Guid idPushButton)
                : base(doorEnvironmentSettings)
            {
                _idPushButton = idPushButton;
                _inputNumber = Inputs.Singleton.GetInputNumber(_idPushButton);

                Inputs.Singleton.AddInputChangedListener(
                    _idPushButton,
                    this);
            }

            public override void OnInputChanged(
                Guid guidInput,
                State state)
            {
                DoorEnvironmentSettings.PushButtonChanged(
                    PushButtonType,
                    state);
            }

            protected abstract PushButtonType PushButtonType
            {
                get;
            }

            public void ApplyHwSetup()
            {
                var idDcu = DoorEnvironmentSettings._idDcu;

                if (idDcu != Guid.Empty)
                    DCUs.Singleton.ApplyHwSetupForPushButton(
                        idDcu,
                        PushButtonType,
                        _inputNumber);
                else
                {
                    var doorStateMachine = DoorEnvironmentSettings._doorStateMachine;

                    if (doorStateMachine == null)
                        return;

                    doorStateMachine.SetPushButton(PushButtonType);

                    DoorEnvironmentSettings.PushButtonChanged(
                        PushButtonType,
                        Inputs.Singleton.GetInputLogicalState(_idPushButton));
                }
            }

            public void Unconfigure(bool unset)
            {
                Inputs.Singleton.RemoveInputChangedListener(
                    _idPushButton,
                    this);

                if (!unset)
                    return;

                var idDcu = DoorEnvironmentSettings._idDcu;

                if (idDcu != Guid.Empty)
                    DCUs.Singleton.UnsetPushButton(
                        idDcu,
                        PushButtonType);
                else
                {
                    var doorStateMachine = DoorEnvironmentSettings._doorStateMachine;

                    if (doorStateMachine != null)
                        doorStateMachine.UnsetPushButton(PushButtonType);
                }
            }
        }

        private class PushButtonInternalChangedListnener : APushButtonChangedListener
        {
            public PushButtonInternalChangedListnener(
                DoorEnvironmentSettings doorEnvironmentSettings,
                Guid idPushButtonInternal)
                : base(
                    doorEnvironmentSettings,
                    idPushButtonInternal)
            {
            }

            public override bool Equals(IInputChangedListener other)
            {
                var x = other as PushButtonInternalChangedListnener;

                return
                    x != null
                    && DoorEnvironmentSettings.Id
                    // ReSharper disable once ImpureMethodCallOnReadonlyValueField
                        .Equals(DoorEnvironmentSettings.Id);
            }

            protected override PushButtonType PushButtonType
            {
                get { return PushButtonType.Internal; }
            }
        }

        private class PushButtonExternalChangedListnener : APushButtonChangedListener
        {
            public PushButtonExternalChangedListnener(
                DoorEnvironmentSettings doorEnvironmentSettings,
                Guid idPushButtonExternal)
                : base(
                    doorEnvironmentSettings,
                    idPushButtonExternal)
            {
            }

            public override bool Equals(IInputChangedListener other)
            {
                var otherListener = other as PushButtonExternalChangedListnener;

                return otherListener != null
                       && DoorEnvironmentSettings.Id
                    // ReSharper disable once ImpureMethodCallOnReadonlyValueField
                        .Equals(otherListener.DoorEnvironmentSettings.Id);
            }

            protected override PushButtonType PushButtonType
            {
                get { return PushButtonType.External; }
            }
        }

        public class DoorEnvironmentSettingsRequest : WaitableProcessingRequest<DoorEnvironmentSettings>
        {
            private readonly Action<DoorEnvironmentSettings> _requestAction;

            public DoorEnvironmentSettingsRequest(Action<DoorEnvironmentSettings> requestAction)
            {
                _requestAction = requestAction;
            }

            protected override void ExecuteInternal(DoorEnvironmentSettings doorEnvironmentSettings)
            {
                _requestAction(doorEnvironmentSettings);
            }
        }

        private class DoorStateMachineEventHandler : IDoorStateMachineEventHandler
        {
            public event Action<bool, DoorEnvironmentAccessTrigger> UnlockStateChangedEvent;

            void IDoorStateMachineEventHandler.UnlockStateChanged(
                bool isUnlocked,
                DoorEnvironmentAccessTrigger dsmAccessTrigger)
            {
                if (UnlockStateChangedEvent != null)
                {
                    UnlockStateChangedEvent(
                        isUnlocked,
                        dsmAccessTrigger);
                }
            }

            public event Action<bool> IntrusionStateChangedEvent;

            void IDoorStateMachineEventHandler.IntrusionStateChanged(bool isInIntrusion)
            {
                if (IntrusionStateChangedEvent != null)
                {
                    IntrusionStateChangedEvent(isInIntrusion);
                }
            }

            public event Action<bool> SabotageStateChangedEvent;

            void IDoorStateMachineEventHandler.SabotageStateChanged(bool isInSabotage)
            {
                if (SabotageStateChangedEvent != null)
                {
                    SabotageStateChangedEvent(isInSabotage);
                }
            }

            public event Action<bool> AjarStateChangedEvent;

            void IDoorStateMachineEventHandler.AjarStateChanged(bool isInAjar)
            {
                if (AjarStateChangedEvent != null)
                {
                    AjarStateChangedEvent(isInAjar);
                }
            }

            public event Action<bool> BypassAlarmStateChangedEvent;

            void IDoorStateMachineEventHandler.BypassAlarmStateChanged(bool isBypassActivated)
            {
                if (BypassAlarmStateChangedEvent != null)
                {
                    BypassAlarmStateChangedEvent(isBypassActivated);
                }
            }

            public event Action<DsmStateInfo> DsmStateChangedEvent;

            void IDoorStateMachineEventHandler.DSMStateChanged(DsmStateInfo dsmStateInfo)
            {
                if (DsmStateChangedEvent != null)
                {
                    DsmStateChangedEvent(dsmStateInfo);
                }
            }

            public void ClearAllEvents()
            {
                UnlockStateChangedEvent = null;
                IntrusionStateChangedEvent = null;
                SabotageStateChangedEvent = null;
                AjarStateChangedEvent = null;
                BypassAlarmStateChangedEvent = null;
                DsmStateChangedEvent = null;
            }
        }

        private class AccessData : AccessDataBase
        {
            private readonly Guid _idPushButton;

            public override Guid IdPushButton
            {
                get { return _idPushButton; }
            }

            public AccessData(Guid idPushButton)
            {
                _idPushButton = idPushButton;
            }
        }

        private const string ALARM_TYPE_DOOR_AJAR = "door ajar";
        private const string ALARM_TYPE_INTRUSION = "intrusion";
        private const string ALARM_TYPE_SABOTAGE = "sabotage";
        private const int DELAY_WAIT_FOR_UNLOCKED_OPENED = 1000;

        private DoorEnvironmentState _doorEnvironmentState = DoorEnvironmentState.Unknown;
        private bool _intrusionBridgeActive;

        private readonly Guid _idDcu;

        private Guid _idDoorAjarOutput;
        private Guid _idSabotageOutput;
        private Guid _idIntrusionOutput;

        private Guid _idActuatorsBypassAlarm;
        private Guid _idActuatorsElectricStrike;
        private Guid _idActuatorsExtraElectricStrike;
        private Guid _idActuatorsElectricStrikeOpposite;
        private Guid _idActuatorsExtraElectricStrikeOpposite;

        private Guid _idSensorOpenDoor;

        private int _doorDelayBeforeBreakIn;

        private DoorEnviromentType? _doorEnvironmentType;

        private volatile object _lockintrusionBridgeActive = new object();

        private Guid _accessCardReader;
        private AccessDataBase _accessData;

        private IInputChangedListener _sensorOpenDoorsInputChanged;

        private APushButtonChangedListener _pushButtonInternalChanged;
        private APushButtonChangedListener _pushButtonExternalChanged;

        public bool IntrusionBridgeActive { get { return _intrusionBridgeActive; } set { _intrusionBridgeActive = value; } }

        public bool IsForcedUnlocked { get; set; }

        public DoorEnvironmentState DoorEnviromentState { get { return _doorEnvironmentState; } }

        private readonly object _lockWaitForUnlockedOpened = new object();
        private ManualResetEvent _waitForUnlockedOpened;

        private readonly EventHandlerGroup<DoorEnvironments.IStateChangedHandler> _stateChangedHandlerGroup =
            new EventHandlerGroup<DoorEnvironments.IStateChangedHandler>();

        public bool IsDsmStartRequired
        {
            get;
            private set;
        }

        public DoorEnvironmentSettings(DB.DoorEnvironment doorEnvironment)
            : base(doorEnvironment.IdDoorEnvironment)
        {
            IsDsmStartRequired = true;
            _idDcu = doorEnvironment.GuidDCU;
        }

        protected override void ConfigureInternal(DB.DoorEnvironment doorEnvironment)
        {
            var idCardReaderInternal = doorEnvironment.GuidCardReaderInternal;
            var idCardReaderExternal = doorEnvironment.GuidCardReaderExternal;

            if (_idCardReaderInternal != Guid.Empty
                && _idCardReaderInternal != idCardReaderInternal
                && _idCardReaderInternal != idCardReaderExternal)
            {
                CardReaders.Singleton.DetachDoorEnvironmentAdapter(
                    _idCardReaderInternal,
                    Id);
            }

            if (_idCardReaderExternal != Guid.Empty
                && _idCardReaderExternal != idCardReaderInternal
                && _idCardReaderExternal != idCardReaderExternal)
            {
                CardReaders.Singleton.DetachDoorEnvironmentAdapter(
                    _idCardReaderExternal,
                    Id);
            }

            _idCardReaderInternal = idCardReaderInternal;
            _idCardReaderExternal = idCardReaderExternal;

            _idDoorAjarOutput = doorEnvironment.GuidDoorAjarOutput;
            _idIntrusionOutput = doorEnvironment.GuidIntrusionOutput;
            _idSabotageOutput = doorEnvironment.GuidSabotageOutput;

            _idActuatorsBypassAlarm = doorEnvironment.GuidActuatorsBypassAlarm;
            _idActuatorsElectricStrike = doorEnvironment.GuidActuatorsElectricStrike;
            _idActuatorsExtraElectricStrike = doorEnvironment.GuidActuatorsExtraElectricStrike;
            _idActuatorsElectricStrikeOpposite = doorEnvironment.GuidActuatorsElectricStrikeOpposite;
            _idActuatorsExtraElectricStrikeOpposite = doorEnvironment.GuidActuatorsExtraElectricStrikeOpposite;

            _idSensorOpenDoor = doorEnvironment.GuidSensorsOpenDoors;

            _doorDelayBeforeBreakIn = doorEnvironment.DoorDelayBeforeBreakIn;

            _doorEnvironmentType = (DoorEnviromentType?)doorEnvironment.ActuatorsDoorEnvironment;

            ConfigureAlarms(doorEnvironment);

            if (_doorEnvironmentType.HasValue)
            {
                if (IsDsmStartRequired)
                    AddSensorOpenDoorInputChanged();

                AddPushButtonInternalChange(doorEnvironment);
                AddPushButtonExternalChange(doorEnvironment);
            }
        }

        protected override void ApplyHwSetup(
            DB.DoorEnvironment doorEnvironment)
        {
            if (!_doorEnvironmentType.HasValue)
                return;

            if (!IsDsmStartRequired)
            {
                ApplyHwSetupForDoorEnvironmentCommon(doorEnvironment);
                ApplyHwSetupForDoorEnvironmentSpecialOutputs();

                return;
            }

            IsDsmStartRequired = false;

            //TODO is this really necessary to be called at this place ?
            SetDoorEnvironmentState(
                DoorEnvironmentState.Locked,
                DoorEnvironmentStateDetail.None);

            if (doorEnvironment.BlockedByLicence)
            {
                if (_idCardReaderExternal != Guid.Empty)
                    CardReaders.Singleton.ShowBlockedDoorEnvironmentMessage(_idCardReaderExternal);

                if (_idCardReaderInternal != Guid.Empty)
                    CardReaders.Singleton.ShowBlockedDoorEnvironmentMessage(_idCardReaderInternal);

                return;
            }

            if (_idDcu != Guid.Empty)
            {
                ApplyHwSetupForDoorEnvironmentCommon(doorEnvironment);

                ApplyHwSetupAssignCardReaders();

                ApplyHwSetupForDoorEnvironmentSpecialOutputs();

                DCUs.Singleton.ApplyHwSetupForDoorEnvironmentAndStart(doorEnvironment);

                return;
            }

            CreateCcuDoorStateMachine();

            ApplyHwSetupForDoorEnvironmentCommon(doorEnvironment);
            ApplyHwSetupAssignCardReaders();

            OnDoorEnvironmentActivationChanged(true);

            _doorStateMachine.StartDSM(_doorEnvironmentType.Value);

            SensorOpenDoorsChanged(
                _idSensorOpenDoor,
                Inputs.Singleton.GetInputLogicalState(_idSensorOpenDoor));
        }

        protected override void UnconfigureInternal(DB.DoorEnvironment newDoorEnvironment)
        {
            if (newDoorEnvironment == null)
            {
                Stop(false);

                if (_doorStateMachine != null)
                    _doorStateMachine.Dispose();

                if (_idCardReaderInternal != Guid.Empty)
                    CardReaders.Singleton.DetachDoorEnvironmentAdapter(
                        _idCardReaderInternal,
                        Id);

                if (_idCardReaderExternal != Guid.Empty)
                    CardReaders.Singleton.DetachDoorEnvironmentAdapter(
                        _idCardReaderExternal,
                        Id);

                return;
            }

            if (newDoorEnvironment.ActuatorsDoorEnvironment == null)
            {
                Stop(false);
                return;
            }

            IsDsmStartRequired =
                DsmStartRequired(
                    newDoorEnvironment);

            if (!IsDsmStartRequired)
            {
                if (_pushButtonExternalChanged != null)
                {
                    _pushButtonExternalChanged.Unconfigure(newDoorEnvironment.GuidPushButtonExternal == Guid.Empty);
                    _pushButtonExternalChanged = null;
                }

                if (_pushButtonInternalChanged != null)
                {
                    _pushButtonInternalChanged.Unconfigure(newDoorEnvironment.GuidPushButtonInternal == Guid.Empty);
                    _pushButtonInternalChanged = null;
                }

                UnsetHwSetupForDoorEnvironmentSpecialOutputs(newDoorEnvironment);
                return;
            }

            ConsiderStartIntrusionBridge(newDoorEnvironment);
            Stop(true);
        }

        private static void ConfigureAlarms(DB.DoorEnvironment doorEnvironment)
        {
            BlockedAlarmsManager.Singleton.ConfigureSpecificBlockingForObject(
                AlarmType.DoorEnvironment_DoorAjar,
                new IdAndObjectType(
                    doorEnvironment.IdDoorEnvironment,
                    ObjectType.DoorEnvironment),
                doorEnvironment.DoorAjarAlarmOn,
                doorEnvironment.BlockAlarmDoorAjar,
                doorEnvironment.ObjBlockAlarmDoorAjarId,
                doorEnvironment.ObjBlockAlarmDoorAjarObjectType,
                true);

            BlockedAlarmsManager.Singleton.ConfigureSpecificBlockingForObject(
                AlarmType.DoorEnvironment_Intrusion,
                new IdAndObjectType(
                    doorEnvironment.IdDoorEnvironment,
                    ObjectType.DoorEnvironment),
                doorEnvironment.IntrusionAlarmOn,
                doorEnvironment.BlockAlarmIntrusion,
                doorEnvironment.ObjBlockAlarmIntrusionId,
                doorEnvironment.ObjBlockAlarmIntrusionObjectType,
                true);

            BlockedAlarmsManager.Singleton.ConfigureSpecificBlockingForObject(
                AlarmType.DoorEnvironment_Sabotage,
                new IdAndObjectType(
                    doorEnvironment.IdDoorEnvironment,
                    ObjectType.DoorEnvironment),
                doorEnvironment.SabotageAlarmOn,
                doorEnvironment.BlockAlarmSabotage,
                doorEnvironment.ObjBlockAlarmSabotageId,
                doorEnvironment.ObjBlockAlarmSabotageObjectType,
                true);

            if (doorEnvironment.AlarmTypeAndIdAlarmArcs != null)
                CatAlarmsManager.Singleton.ConfigureSpecificAlarmArcs(
                    new IdAndObjectType(
                        doorEnvironment.IdDoorEnvironment,
                        ObjectType.DoorEnvironment),
                    doorEnvironment.AlarmTypeAndIdAlarmArcs);
            else
                CatAlarmsManager.Singleton.RemoveSpecificAlarmArcsConfiguration(
                    new IdAndObjectType(
                        doorEnvironment.IdDoorEnvironment,
                        ObjectType.DoorEnvironment));
        }

        public void UnsetHwSetupForDoorEnvironmentSpecialOutputs(DB.DoorEnvironment newDoorEnvironment)
        {
            if (_idDoorAjarOutput != Guid.Empty
                && newDoorEnvironment.GuidDoorAjarOutput != _idDoorAjarOutput)
            {
                if (_idDcu != Guid.Empty
                    && _idDcu == Outputs.Singleton.GetIdDcuForOutput(_idDoorAjarOutput))
                {
                    DCUs.Singleton.UnsetDsmSpecialOutput(
                        _idDcu,
                        SpecialOutputType.Ajar);
                }
                else
                    if (_doorEnvironmentState == DoorEnvironmentState.Ajar)
                        AjarStateChanged(false);

                _idDoorAjarOutput = Guid.Empty;
            }

            if (_idIntrusionOutput != Guid.Empty
                && newDoorEnvironment.GuidIntrusionOutput != _idIntrusionOutput)
            {
                if (_idDcu != Guid.Empty
                    && _idDcu == Outputs.Singleton.GetIdDcuForOutput(_idIntrusionOutput))
                {
                    DCUs.Singleton.UnsetDsmSpecialOutput(
                        _idDcu,
                        SpecialOutputType.Intrusion);
                }
                else
                    if (_doorEnvironmentState == DoorEnvironmentState.Intrusion)
                        IntrusionStateChanged(false);

                _idIntrusionOutput = Guid.Empty;
            }

            if (_idSabotageOutput != Guid.Empty
                && newDoorEnvironment.GuidDoorAjarOutput != _idSabotageOutput)
            {
                if (_idDcu != Guid.Empty
                    && _idDcu == Outputs.Singleton.GetIdDcuForOutput(_idSabotageOutput))
                {
                    DCUs.Singleton.UnsetDsmSpecialOutput(
                        _idDcu,
                        SpecialOutputType.Sabotage);
                }
                else
                    if (_doorEnvironmentState == DoorEnvironmentState.Sabotage)
                        SabotageStateChanged(false);

                _idSabotageOutput = Guid.Empty;
            }
        }

        private void ApplyHwSetupForDoorEnvironmentCommon(
            DB.DoorEnvironment doorEnvironment)
        {
            if (_doorStateMachine != null)
            {
                _doorStateMachine.SetTimmings(
                    (uint)doorEnvironment.DoorTimeUnlock*1000,
                    (uint)doorEnvironment.DoorTimeOpen*1000,
                    (uint)doorEnvironment.DoorTimePreAlarm*1000,
                    (uint)doorEnvironment.DoorTimeSirenAjar*1000,
                    0);
            }
            else
                DCUs.Singleton.ApplyHwSetupForDoorEnvironmentCommon(doorEnvironment);

            if (_pushButtonExternalChanged != null)
                _pushButtonExternalChanged.ApplyHwSetup();

            if (_pushButtonInternalChanged != null)
                _pushButtonInternalChanged.ApplyHwSetup();
        }

        private void ApplyHwSetupAssignCardReaders()
        {
            int internalCrAddress;

            ImplicitCrCodeParams internalImplicCrCodeParams =
                CardReaders.Singleton
                    .GetCurrentImplicitCrCodeParams(
                        _idCardReaderInternal,
                        out internalCrAddress);

            int externalCrAddress;

            var externalImplicCrCodeParams =
                CardReaders.Singleton
                    .GetCurrentImplicitCrCodeParams(
                        _idCardReaderExternal,
                        out externalCrAddress);

            if (_idDcu == Guid.Empty)
                ApplyHwSetupAssignCcuCardReaders(
                    internalCrAddress,
                    internalImplicCrCodeParams,
                    externalCrAddress,
                    externalImplicCrCodeParams);
            else
                DCUs.Singleton.ApplyHwSetupAssignDcuCardReaders(
                    _idDcu,
                    internalCrAddress,
                    internalImplicCrCodeParams,
                    externalCrAddress,
                    externalImplicCrCodeParams);

            if (_idCardReaderInternal != Guid.Empty)
                CardReaders.Singleton.EnterRootScene(_idCardReaderInternal);

            if (_idCardReaderExternal != Guid.Empty)
                CardReaders.Singleton.EnterRootScene(_idCardReaderExternal);
        }

        private void ApplyHwSetupForDoorEnvironmentSpecialOutputs()
        {
            if (_idDoorAjarOutput != Guid.Empty)
            {
                var idDcuAndOutputNumberForDoorAjarOutput =
                    Outputs.Singleton.GetIdDcuAndOutputNumberForOutput(_idDoorAjarOutput);

                if (idDcuAndOutputNumberForDoorAjarOutput != null
                    && _idDcu != Guid.Empty
                    && _idDcu == idDcuAndOutputNumberForDoorAjarOutput.IdDcu)
                {
                    DCUs.Singleton.SetDsmSpecialOutput(
                        _idDcu,
                        SpecialOutputType.Ajar,
                        idDcuAndOutputNumberForDoorAjarOutput.OutputNumber);
                }
                else
                    if (_doorEnvironmentState == DoorEnvironmentState.Ajar)
                        AjarStateChanged(true);

            }
            if (_idIntrusionOutput != Guid.Empty)
            {
                var idDcuAndOutputNumberForIntrusionOutput =
                    Outputs.Singleton.GetIdDcuAndOutputNumberForOutput(_idIntrusionOutput);

                if (idDcuAndOutputNumberForIntrusionOutput != null
                    && _idDcu != Guid.Empty
                    && _idDcu == idDcuAndOutputNumberForIntrusionOutput.IdDcu)
                {
                    DCUs.Singleton.SetDsmSpecialOutput(
                        _idDcu,
                        SpecialOutputType.Intrusion,
                        idDcuAndOutputNumberForIntrusionOutput.OutputNumber);
                }
                else
                    if (_doorEnvironmentState == DoorEnvironmentState.Intrusion)
                        IntrusionStateChanged(true);
            }

            if (_idSabotageOutput != Guid.Empty)
            {
                var idDcuAndOutputNumberForSabotageOutput =
                    Outputs.Singleton.GetIdDcuAndOutputNumberForOutput(_idSabotageOutput);

                if (idDcuAndOutputNumberForSabotageOutput != null
                    && _idDcu != Guid.Empty
                    && _idDcu == idDcuAndOutputNumberForSabotageOutput.IdDcu)
                {
                    DCUs.Singleton.SetDsmSpecialOutput(
                        _idDcu,
                        SpecialOutputType.Sabotage,
                        idDcuAndOutputNumberForSabotageOutput.OutputNumber);
                }
                else
                    if (_doorEnvironmentState == DoorEnvironmentState.Sabotage)
                        SabotageStateChanged(true);
            }
        }

        public void OnDoorEnvironmentActivationChanged(bool isRunning)
        {
            if (isRunning)
                ConsiderStopIntrusionBridgeTimeout();
        }

        private void CreateCcuDoorStateMachine()
        {
            if (_doorStateMachine == null)
            {
                _doorStateMachine = DefaultDsmFactory.Singleton
                    .Create(new DoorStateMachineEventHandler());
            }

            var doorStateMachineEventHandler =
                _doorStateMachine.EventHandler as DoorStateMachineEventHandler;

            if (doorStateMachineEventHandler == null)
                return;

            doorStateMachineEventHandler.UnlockStateChangedEvent += UnlockStateChanged;
            doorStateMachineEventHandler.IntrusionStateChangedEvent += IntrusionStateChanged;
            doorStateMachineEventHandler.SabotageStateChangedEvent += SabotageStateChanged;
            doorStateMachineEventHandler.AjarStateChangedEvent += AjarStateChanged;
            doorStateMachineEventHandler.BypassAlarmStateChangedEvent += BypassAlarmStateChanged;
            doorStateMachineEventHandler.DsmStateChangedEvent += OnCcuDoorEnvironmentStateChanged;
        }

        private void ApplyHwSetupAssignCcuCardReaders(
            int internalCrAddress,
            ImplicitCrCodeParams internalImplicCrCodeParams,
            int externalCrAddress,
            ImplicitCrCodeParams externalImplicCrCodeParams)
        {
            try
            {
                _doorStateMachine.AssignCardReaders(
                    CcuCardReaders.CrCommunicator,
                    internalCrAddress,
                    internalImplicCrCodeParams.ImplicitCrMessage,
                    CRControlCommands.StackedMessage(
                        internalImplicCrCodeParams.FollowingMessages,
                        (byte)internalCrAddress),
                    externalImplicCrCodeParams.IsGinOrVariations,
                    externalCrAddress,
                    externalImplicCrCodeParams.ImplicitCrMessage,
                    CRControlCommands.StackedMessage(
                        externalImplicCrCodeParams.FollowingMessages,
                        (byte)externalCrAddress),
                    externalImplicCrCodeParams.IsGinOrVariations);
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        public void SuppressCardReader(byte cardReaderAddress)
        {
            if (_idDcu != Guid.Empty)
            {
                DCUs.Singleton.SuppressCardReader(_idDcu, cardReaderAddress);
            }
            else if (_doorStateMachine != null)
            {
                _doorStateMachine.SuppressCardReader(cardReaderAddress);
            }
        }

        public void LooseCardReader(byte cardReaderAddress)
        {
            if (_idDcu != Guid.Empty)
            {
                DCUs.Singleton.LooseCardReader(_idDcu, cardReaderAddress);
            }
            else if (_doorStateMachine != null)
            {
                _doorStateMachine.LooseCardReader(cardReaderAddress);
            }
        }

        public void SetImplicitCrCode(
            int cardReaderAddress,
            CRMessage implicitCrMessage,
            IList<CRMessage> followingMessages,
            bool intrusionOnlyViaLed)
        {
            if (_idDcu != Guid.Empty)
            {
                DCUs.Singleton.SetImplicitCrCode(
                    _idDcu,
                    cardReaderAddress,
                    implicitCrMessage.MessageCode,
                    implicitCrMessage.OptionalData,
                    CRControlCommands.StackedMessage(
                        followingMessages,
                        (byte) cardReaderAddress),
                    intrusionOnlyViaLed);
            }
            else if (_doorStateMachine != null)
            {
                _doorStateMachine.SetImplicitCRCode(
                    cardReaderAddress,
                    implicitCrMessage,
                    CRControlCommands.StackedMessage(
                        followingMessages,
                        (byte) cardReaderAddress),
                    intrusionOnlyViaLed);
            }
        }

        private void AddSensorOpenDoorInputChanged()
        {
            if (_idSensorOpenDoor == Guid.Empty)
                return;

            _sensorOpenDoorsInputChanged = new SensorDoorOpenChangeListener(this);

            Inputs.Singleton.AddInputChangedListener(
                _idSensorOpenDoor,
                _sensorOpenDoorsInputChanged);
        }

        private void AddPushButtonInternalChange(DB.DoorEnvironment doorEnvironment)
        {
            var idPushButtonInternal = doorEnvironment.GuidPushButtonInternal;

            if (idPushButtonInternal != Guid.Empty)
                _pushButtonInternalChanged =
                    new PushButtonInternalChangedListnener(
                        this,
                        idPushButtonInternal);
        }

        private void AddPushButtonExternalChange(DB.DoorEnvironment doorEnvironment)
        {
            var idPushButtonExternal = doorEnvironment.GuidPushButtonExternal;

            if (idPushButtonExternal != Guid.Empty)
                _pushButtonExternalChanged =
                    new PushButtonExternalChangedListnener(
                        this,
                        idPushButtonExternal);
        }

        private void SetDoorEnvironmentState(
            DoorEnvironmentState doorEnvironmentState,
            DoorEnvironmentStateDetail doorEnvironmentStateDetail)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "void DoorEnvironmentSettings.SetDoorEnvironmentState(DoorEnvironmentState doorEnvironmentState): [{0}]",
                    Log.GetStringFromParameters(doorEnvironmentState)));

            _doorEnvironmentState = doorEnvironmentState;

            SendDoorEnvironmentState();

            _stateChangedHandlerGroup.ForEach(
                handler => 
                    handler.Execute(
                        new DoorEnvironments.DoorEnvironmentStateChangedArgs(
                            Id,
                            _accessData != null ? _accessData.IdCard : Guid.Empty,
                            _accessCardReader,
                            doorEnvironmentState,
                            doorEnvironmentStateDetail)));

            if (doorEnvironmentState == DoorEnvironmentState.Unlocked ||
                doorEnvironmentState == DoorEnvironmentState.Opened)
            {
                lock (_lockWaitForUnlockedOpened)
                    if (_waitForUnlockedOpened != null)
                        _waitForUnlockedOpened.Set();
            }
        }

        private void OnCcuDoorEnvironmentStateChanged(DsmStateInfo dsmStateInfo)
        {
            var dsmState = dsmStateInfo.DsmState;
            var dsmStateDetail = dsmStateInfo.DsmStateDetail;
            var accessTrigger = dsmStateInfo.AccessTrigger;

            EnqueueAsyncRequest(
                doorEnvironmentSettings => 
                    doorEnvironmentSettings.OnDoorEnvironmentStateChanged(
                        dsmState,
                        dsmStateDetail,
                        accessTrigger));
        }

        public void OnDoorEnvironmentStateChanged(
            DoorEnvironmentState doorEnvironmentState,
            DoorEnvironmentStateDetail doorEnvironmentStateDetail,
            DoorEnvironmentAccessTrigger agSource)
        {
            var doorEnvironment =
                Database.ConfigObjectsEngine.GetFromDatabase(
                    ObjectType.DoorEnvironment,
                    Id) as DB.DoorEnvironment;

            if (doorEnvironment != null)
                TurnOffIntrusionBridge(doorEnvironment.GuidIntrusionOutput);

            if (doorEnvironmentState == DoorEnvironmentState.Intrusion)
                DcuIntrusionStateChanged(true);

            else if (_doorEnvironmentState == DoorEnvironmentState.Intrusion)
                DcuIntrusionStateChanged(false);

            if (doorEnvironmentState == DoorEnvironmentState.Sabotage)
                DcuSabotageStateChanged(true);
            else if (_doorEnvironmentState == DoorEnvironmentState.Sabotage)
                DcuSabotageStateChanged(false);

            if (doorEnvironmentState == DoorEnvironmentState.Ajar)
                DcuAjarStateChanged(true);
            else if (_doorEnvironmentState == DoorEnvironmentState.Ajar)
                DcuAjarStateChanged(false);

            SetDoorEnvironmentState(
                doorEnvironmentState,
                doorEnvironmentStateDetail);

            switch (agSource)
            {
                case DoorEnvironmentAccessTrigger.ExternalPushButton:
                case DoorEnvironmentAccessTrigger.InternalPushButton:
                    if (doorEnvironment != null)
                    {
                        _accessCardReader = Guid.Empty;
                        _accessData = new AccessData(
                            agSource == DoorEnvironmentAccessTrigger.InternalPushButton
                                ? doorEnvironment.GuidPushButtonInternal
                                : doorEnvironment.GuidPushButtonExternal);
                    }

                    bool saveAlarm;

                    if (_accessCardReader == Guid.Empty)
                    {
                        saveAlarm = false;
                    }
                    else
                    {
                        var cardReader =
                            Database.ConfigObjectsEngine.GetFromDatabase(
                                ObjectType.CardReader,
                                _accessCardReader) as DB.CardReader;

                        saveAlarm = cardReader != null;
                    }

                    Events.ProcessEvent(
                        new EventDsmAccessPermitted(
                            Id,
                            _accessCardReader,
                            _accessData));

                    if (saveAlarm)
                        AlarmsManager.Singleton.AddAlarm(
                            new CrAccessPermittedAlarm(
                                _accessCardReader,
                                _accessData));

                    break;
            }

            switch (doorEnvironmentStateDetail)
            {
                case DoorEnvironmentStateDetail.NormalAccess:

                    Events.ProcessEvent(
                        new EventDsmNormalAccess(
                            Id,
                            _accessCardReader,
                            _accessData));

                    break;

                case DoorEnvironmentStateDetail.APASRestored:

                    Events.ProcessEvent(
                        new EventDsmApasRestored(
                            Id,
                            _accessCardReader,
                            _accessData));

                    break;

                case DoorEnvironmentStateDetail.InterruptedAccess:

                    Events.ProcessEvent(
                        new EventDsmAccessInterupted(
                            Id,
                            _accessCardReader,
                            _accessData));

                    break;

                case DoorEnvironmentStateDetail.AccessViolated:

                    Events.ProcessEvent(
                        new EventDsmAccessViolated(
                            Id,
                            doorEnvironmentState == DoorEnvironmentState.Sabotage
                                ? "sabotage"
                                : "intrusion"));

                    break;
            }
        }


        private ITimer _intrusionBridgeTimerCarrier;

        /// <summary>
        /// If an intrusion bridge in turned on, this method will consider start timer for turning it off
        /// </summary>
        public void ConsiderStopIntrusionBridgeTimeout()
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "void DoorEnvironmentSettings.ConsiderStopIntrusionBridgeTimeout()");

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (!DoorEnvironments.USE_INTRUSION_BRIDGE)
                // ReSharper disable once HeuristicUnreachableCode
                // ReSharper disable once CSharpWarnings::CS0162
                return;
            // ReSharper disable once CSharpWarnings::CS0162

            lock (_lockintrusionBridgeActive)
                if (!_intrusionBridgeActive)
                    return;

            if (_intrusionBridgeTimerCarrier != null)
                _intrusionBridgeTimerCarrier.StopTimer();

            _intrusionBridgeTimerCarrier =
                TimerManager.Static.StartTimeout(
                    _doorDelayBeforeBreakIn + DoorEnvironments.INTRUSION_BRIDGE_PROCESSING_TIME,
                    _idIntrusionOutput,
                    TurnOffIntrusionBridge);
        }

        /// <summary>
        /// Depending on the settings, it tries to turn intrusion bridge on
        /// </summary>
        /// <param name="newDoorEnvironment"></param>
        private void ConsiderStartIntrusionBridge(
            [NotNull]
            DB.DoorEnvironment newDoorEnvironment)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (!DoorEnvironments.USE_INTRUSION_BRIDGE)
                // ReSharper disable once CSharpWarnings::CS0162
                // ReSharper disable once HeuristicUnreachableCode
                return;

            if (newDoorEnvironment.ActuatorsDoorEnvironment == null)
                return;

            if (_doorEnvironmentState != DoorEnvironmentState.Intrusion)
                return;

            if (newDoorEnvironment.GuidIntrusionOutput == Guid.Empty)
                return;

            _intrusionBridgeActive = true;

            Outputs.Singleton.On(
                DoorEnvironments.Singleton.GetOutputActivatorIntrusionBridge(newDoorEnvironment.IdDoorEnvironment),
                newDoorEnvironment.GuidIntrusionOutput);
        }

        private bool TurnOffIntrusionBridge(TimerCarrier timerCarrier)
        {
            if (timerCarrier == null ||
                timerCarrier.Data == null ||
                !(timerCarrier.Data is Guid))
            {
                return true;
            }

            TurnOffIntrusionBridge((Guid)(timerCarrier.Data));

            return true;
        }

        /// <summary>
        /// Turns intrusion bridge feature off, if it was turned on
        /// </summary>
        /// <param name="outputId"></param>
        private void TurnOffIntrusionBridge(Guid outputId)
        {
            if (outputId == Guid.Empty)
                return;

            lock (_lockintrusionBridgeActive)
                if (_intrusionBridgeActive)
                    _intrusionBridgeActive = false;
                else
                    return;

            _intrusionBridgeTimerCarrier = null;

            Outputs.Singleton.Off(
                DoorEnvironments.Singleton.GetOutputActivatorIntrusionBridge(Id),
                outputId);
        }

        public void SendDoorEnvironmentState()
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => "void DoorEnvironmentSettings.SendDoorEnvironmentState()");

            DoorEnvironments.SendDoorEnvironmentState(_doorEnvironmentState, Id);
        }

        #region Sensors

        // ReSharper disable once UnusedParameter.Local
        private void SensorOpenDoorsChanged(Guid inputGuid, State state)
        {
            if (_doorStateMachine == null)
                return;

            _doorStateMachine.SetDoorSensorState(GetDSMSensorState(state));
        }

        public static DSMSensorState GetDSMSensorState(State sensorState)
        {
            switch (sensorState)
            {
                case State.Alarm:

                    return DSMSensorState.Alarm;

                case State.Short:
                case State.Break:

                    return DSMSensorState.Tamper;

                default:

                    return DSMSensorState.Normal;
            }
        }

        #endregion

        #region PushButtons

        // ReSharper disable once UnusedParameter.Local
        private void PushButtonChanged(
            PushButtonType pushButtonType, 
            State state)
        {
            if (_doorStateMachine == null)
                return;

            _doorStateMachine.SetPushButtonState(
                pushButtonType,
                GetDSMSensorState(state));
        }

        #endregion

        #region AccessGranted

        public void ClearAccessGrantedVariables()
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => "void DoorEnvironmentSettings.ClearAccessGrantedVariables()");

            _accessCardReader = Guid.Empty;
            _accessData = null;
        }

        public void CreateWaitForUnlockedOpened()
        {
            lock (_lockWaitForUnlockedOpened)
                _waitForUnlockedOpened = new ManualResetEvent(false);
        }

        public bool WaitForUnlockedOpened()
        {
            try
            {
                return _waitForUnlockedOpened.WaitOne(DELAY_WAIT_FOR_UNLOCKED_OPENED, false);
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
                return false;
            }
            finally
            {
                lock (_lockWaitForUnlockedOpened)
                {
                    if (_waitForUnlockedOpened != null)
                    {
                        _waitForUnlockedOpened.Close();
                        _waitForUnlockedOpened = null;
                    }
                }
            }
        }

        public void AccessGrantedFromClient()
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => "void DoorEnvironmentSettings.AccessGrantedFromClient()");

            OnAccessGranted(
                Guid.Empty,
                new AccessDataBase(), 
                -1,
                false);
        }


        public void OnAccessGranted(
            Guid guidCardReader,
            AccessDataBase accessData,
            int crAddress,
            bool forced)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "void DoorEnvironmentSettings.OnAccessGranted(Guid guidCardReader, Guid guidCard, Guid idPerson, Guid guidPushButton, AccessGrantedSource ags, int crAddress, bool forced): [{0}]",
                    Log.GetStringFromParameters(
                        guidCardReader,
                        accessData.IdCard,
                        accessData.IdPerson,
                        accessData.IdPushButton,
                        accessData.AccessGrantedSource,
                        crAddress,
                        forced)));

            _accessCardReader = guidCardReader;
            _accessData = accessData;

            Events.ProcessEvent(
                new EventDsmAccessPermitted(
                    Id,
                    guidCardReader,
                    accessData));

            if (guidCardReader != Guid.Empty)
                AlarmsManager.Singleton.AddAlarm(
                    new CrAccessPermittedAlarm(
                        guidCardReader,
                        accessData));

            var accessTrigger = guidCardReader == Guid.Empty
                ? DoorEnvironmentAccessTrigger.CardReader
                : _idCardReaderInternal == guidCardReader
                    ? DoorEnvironmentAccessTrigger.InternalCardReader
                    : DoorEnvironmentAccessTrigger.ExternalCardReader;

            if (_idDcu != Guid.Empty)
                DCUs.Singleton.DoorEnvironmentAccessGranted(
                    _idDcu,
                    GetAgSeverity(accessData.AccessGrantedSource),
                    accessTrigger);
            else
            {
                if (_doorStateMachine == null)
                    return;

                _doorStateMachine.SignalAccessGranted(
                    GetAgSeverity(accessData.AccessGrantedSource),
                    accessTrigger);
            }
        }

        public static DsmAccessGrantedSeverity GetAgSeverity(AccessGrantedSource ags)
        {
            switch (ags)
            {
                case AccessGrantedSource.Card:
                case AccessGrantedSource.Other:

                    return DsmAccessGrantedSeverity.NormalCard;

                case AccessGrantedSource.EmergencyCode:

                    return DsmAccessGrantedSeverity.EmergencyCode;
            }

            return DsmAccessGrantedSeverity.NormalCard;
        }

        #endregion

        #region Actuators

        private void UnlockStateChanged(
            bool isUnlocked,
            DoorEnvironmentAccessTrigger accessTrigger)
        {
            if (_doorEnvironmentType != DoorEnviromentType.Turnstile)
            {
                SetElectricStrikes(isUnlocked);
                return;
            }

            if (accessTrigger == DoorEnvironmentAccessTrigger.None
                || accessTrigger == DoorEnvironmentAccessTrigger.CardReader
                || accessTrigger == DoorEnvironmentAccessTrigger.InternalPushButton
                || accessTrigger == DoorEnvironmentAccessTrigger.InternalCardReader)
            {
                SetElectricStrikes(isUnlocked);
            }

            if (accessTrigger == DoorEnvironmentAccessTrigger.None
                || accessTrigger == DoorEnvironmentAccessTrigger.CardReader
                || accessTrigger == DoorEnvironmentAccessTrigger.ExternalPushButton
                || accessTrigger == DoorEnvironmentAccessTrigger.ExternalCardReader)
            {
                SetElectricStrikesOpposite(isUnlocked);
            }
        }

        private void SetElectricStrikes(bool isUnlocked)
        {
            if (isUnlocked)
                Outputs.Singleton.On(
                    Id.ToString(),
                    _idActuatorsElectricStrike);
            else
                Outputs.Singleton.Off(
                    Id.ToString(),
                    _idActuatorsElectricStrike);

            if (_idActuatorsExtraElectricStrike == Guid.Empty)
                return;

            if (isUnlocked)
                Outputs.Singleton.On(
                    Id.ToString(),
                    _idActuatorsExtraElectricStrike);
            else
                Outputs.Singleton.Off(
                    Id.ToString(),
                    _idActuatorsExtraElectricStrike);
        }

        private void SetElectricStrikesOpposite(bool isUnlocked)
        {
            if (isUnlocked)
                Outputs.Singleton.On(
                    Id.ToString(),
                    _idActuatorsElectricStrikeOpposite);
            else
                Outputs.Singleton.Off(
                    Id.ToString(),
                    _idActuatorsElectricStrikeOpposite);

            if (_idActuatorsExtraElectricStrikeOpposite == Guid.Empty)
                return;

            if (isUnlocked)
                Outputs.Singleton.On(
                    Id.ToString(),
                    _idActuatorsExtraElectricStrikeOpposite);
            else
                Outputs.Singleton.Off(
                    Id.ToString(),
                    _idActuatorsExtraElectricStrikeOpposite);
        }

        private void BypassAlarmStateChanged(bool isBypassActivated)
        {
            if (_idActuatorsBypassAlarm == Guid.Empty)
                return;

            if (isBypassActivated)
            {
                Outputs.Singleton.On(
                    Id.ToString(),
                    _idActuatorsBypassAlarm);
            }
            else
            {
                Outputs.Singleton.Off(
                    Id.ToString(),
                    _idActuatorsBypassAlarm);
            }
        }

        #endregion

        #region SpecialOutputs

        private void DcuIntrusionStateChanged(bool isInIntrusion)
        {
            if (_idDcu != Guid.Empty
                && _idDcu != Outputs.Singleton.GetIdDcuForOutput(_idIntrusionOutput))
            {
                IntrusionStateChanged(isInIntrusion);
            }
        }

        private readonly object _lockIntrusionStateChanged = new object();

        private void IntrusionStateChanged(bool isInIntrusion)
        {
            lock (_lockIntrusionStateChanged)
            {
                if (_idIntrusionOutput == Guid.Empty)
                    return;

                if (isInIntrusion)
                {
                    Outputs.Singleton.On(
                        string.Format(
                            "{0}{1}",
                            ALARM_TYPE_INTRUSION,
                            Id),
                        _idIntrusionOutput);
                }
                else
                {
                    Outputs.Singleton.Off(
                        string.Format("{0}{1}",
                        ALARM_TYPE_INTRUSION,
                        Id),
                        _idIntrusionOutput);
                }
            }
        }

        private void DcuAjarStateChanged(bool isInAjar)
        {
            if (_idDcu != Guid.Empty
                && _idDcu != Outputs.Singleton.GetIdDcuForOutput(_idDoorAjarOutput))
            {
                AjarStateChanged(isInAjar);
            }
        }

        private readonly object _lockAjarStateChanged = new object();
        private void AjarStateChanged(bool isInAjar)
        {
            lock (_lockAjarStateChanged)
            {
                if (_idDoorAjarOutput == Guid.Empty)
                    return;

                if (isInAjar)
                {
                    Outputs.Singleton.On(
                        string.Format("{0}{1}", ALARM_TYPE_DOOR_AJAR, Id),
                        _idDoorAjarOutput);
                }
                else
                {
                    Outputs.Singleton.Off(
                        string.Format("{0}{1}", ALARM_TYPE_DOOR_AJAR, Id),
                        _idDoorAjarOutput);
                }
            }
        }

        private void DcuSabotageStateChanged(bool isInSabotage)
        {
            if (_idDcu != Guid.Empty
                && _idDcu != Outputs.Singleton.GetIdDcuForOutput(_idSabotageOutput))
            {
                SabotageStateChanged(isInSabotage);
            }
        }

        private readonly object _lockSabotageStateChanged = new object();

        private Guid _idCardReaderInternal;
        private Guid _idCardReaderExternal;

        private IDoorStateMachine _doorStateMachine;

        private void SabotageStateChanged(bool isInSabotage)
        {
            lock (_lockSabotageStateChanged)
            {
                if (_idSabotageOutput == Guid.Empty)
                    return;

                if (isInSabotage)
                {
                    Outputs.Singleton.On(
                        string.Format("{0}{1}", ALARM_TYPE_SABOTAGE, Id),
                        _idSabotageOutput);
                }
                else
                {
                    Outputs.Singleton.Off(
                        string.Format("{0}{1}", ALARM_TYPE_SABOTAGE, Id),
                        _idSabotageOutput);
                }
            }
        }

        #endregion

        private void Stop(bool update)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format("void DoorEnvironmentSettings.Close(bool update): [{0}]",
                    Log.GetStringFromParameters(update)));

            if (_idDcu != Guid.Empty)
                DCUs.Singleton.StopDoorEnvironment(_idDcu);
            else
                if (_doorStateMachine != null)
                {
                    _doorStateMachine.StopDSM();

                    var doorStateMachineEventHandler =
                        _doorStateMachine.EventHandler as DoorStateMachineEventHandler;

                    if (doorStateMachineEventHandler != null)
                        doorStateMachineEventHandler.ClearAllEvents();
                }

            if (!update)
                OnDoorEnvironmentStateChanged(
                    DoorEnvironmentState.Unknown,
                    DoorEnvironmentStateDetail.None,
                    DoorEnvironmentAccessTrigger.None);

            if (_sensorOpenDoorsInputChanged != null)
            {
                Inputs.Singleton.RemoveInputChangedListener(
                    _idSensorOpenDoor,
                    _sensorOpenDoorsInputChanged);

                _sensorOpenDoorsInputChanged = null;
            }

            if (_pushButtonInternalChanged != null)
            {
                _pushButtonInternalChanged.Unconfigure(false);
                _pushButtonInternalChanged = null;
            }

            if (_pushButtonExternalChanged != null)
            {
                _pushButtonExternalChanged.Unconfigure(false);
                _pushButtonExternalChanged = null;
            }
        }

        public void EnqueueAsyncRequest(Action<DoorEnvironmentSettings> requestAction)
        {
            EnqueueAsyncRequest(new DoorEnvironmentSettingsRequest(requestAction));
        }

        public bool DsmStartRequired(
            DB.DoorEnvironment newDoorEnvironment)
        {
            // means that DSM settings ecxept excluded changed
            //excluded SabotageAlarmOn, IntrusionAlarmOn, DoorAjarAlarmOn, Name

            if (newDoorEnvironment == null)
                return true;

            var oldDoorEnvironment =
                Database.ConfigObjectsEngine.GetFromDatabase(
                    ObjectType.DoorEnvironment,
                    Id) as DB.DoorEnvironment;

            if (oldDoorEnvironment == null)
                return true;

            if (oldDoorEnvironment.ActuatorsDoorEnvironment !=
                    newDoorEnvironment.ActuatorsDoorEnvironment)
            {
                return true;
            }

            if (oldDoorEnvironment.ActuatorsElectricStrikeImpulse !=
                    newDoorEnvironment.ActuatorsElectricStrikeImpulse)
            {
                return true;
            }

            if (oldDoorEnvironment.ActuatorsElectricStrikeImpulseDelay !=
                    newDoorEnvironment.ActuatorsElectricStrikeImpulseDelay)
            {
                return true;
            }

            if (oldDoorEnvironment.ActuatorsElectricStrikeOppositeImpulse !=
                    newDoorEnvironment.ActuatorsElectricStrikeOppositeImpulse)
            {
                return true;
            }

            if (oldDoorEnvironment.ActuatorsElectricStrikeOppositeImpulseDelay !=
                    newDoorEnvironment.ActuatorsElectricStrikeOppositeImpulseDelay)
            {
                return true;
            }

            if (oldDoorEnvironment.ActuatorsExtraElectricStrikeImpulse !=
                    newDoorEnvironment.ActuatorsExtraElectricStrikeImpulse)
            {
                return true;
            }

            if (oldDoorEnvironment.ActuatorsExtraElectricStrikeImpulseDelay !=
                    newDoorEnvironment.ActuatorsExtraElectricStrikeImpulseDelay)
            {
                return true;
            }

            if (oldDoorEnvironment.ActuatorsExtraElectricStrikeOppositeImpulse !=
                    newDoorEnvironment.ActuatorsExtraElectricStrikeOppositeImpulse)
            {
                return true;
            }

            if (oldDoorEnvironment.ActuatorsExtraElectricStrikeOppositeImpulseDelay !=
                    newDoorEnvironment.ActuatorsExtraElectricStrikeOppositeImpulseDelay)
            {
                return true;
            }

            if (oldDoorEnvironment.DoorDelayBeforeClose !=
                    newDoorEnvironment.DoorDelayBeforeClose)
            {
                return true;
            }

            if (oldDoorEnvironment.DoorDelayBeforeLock !=
                    newDoorEnvironment.DoorDelayBeforeLock)
            {
                return true;
            }

            if (oldDoorEnvironment.DoorDelayBeforeUnlock !=
                    newDoorEnvironment.DoorDelayBeforeUnlock)
            {
                return true;
            }

            if (oldDoorEnvironment.GuidActuatorsBypassAlarm !=
                    newDoorEnvironment.GuidActuatorsBypassAlarm)
            {
                return true;
            }

            if (oldDoorEnvironment.GuidActuatorsElectricStrike !=
                    newDoorEnvironment.GuidActuatorsElectricStrike)
            {
                return true;
            }

            if (oldDoorEnvironment.GuidActuatorsElectricStrikeOpposite !=
                    newDoorEnvironment.GuidActuatorsElectricStrikeOpposite)
            {
                return true;
            }

            if (oldDoorEnvironment.GuidActuatorsExtraElectricStrike !=
                    newDoorEnvironment.GuidActuatorsExtraElectricStrike)
            {
                return true;
            }

            if (oldDoorEnvironment.GuidActuatorsExtraElectricStrikeOpposite !=
                    newDoorEnvironment.GuidActuatorsExtraElectricStrikeOpposite)
            {
                return true;
            }

            if (oldDoorEnvironment.GuidCardReaderExternal !=
                    newDoorEnvironment.GuidCardReaderExternal)
            {
                return true;
            }

            if (oldDoorEnvironment.GuidCardReaderInternal !=
                    newDoorEnvironment.GuidCardReaderInternal)
            {
                return true;
            }

            if (oldDoorEnvironment.GuidSensorsLockDoors !=
                    newDoorEnvironment.GuidSensorsLockDoors)
            {
                return true;
            }

            if (oldDoorEnvironment.GuidSensorsOpenDoors !=
                    newDoorEnvironment.GuidSensorsOpenDoors)
            {
                return true;
            }

            if (oldDoorEnvironment.GuidSensorsOpenMaxDoors !=
                    newDoorEnvironment.GuidSensorsOpenMaxDoors)
            {
                return true;
            }

            if (oldDoorEnvironment.SensorsLockDoorsBalanced !=
                    newDoorEnvironment.SensorsLockDoorsBalanced)
            {
                return true;
            }

            if (oldDoorEnvironment.SensorsLockDoorsInverted !=
                    newDoorEnvironment.SensorsLockDoorsInverted)
            {
                return true;
            }

            if (oldDoorEnvironment.SensorsOpenDoorsBalanced !=
                    newDoorEnvironment.SensorsOpenDoorsBalanced)
            {
                return true;
            }

            if (oldDoorEnvironment.SensorsOpenDoorsInverted !=
                    newDoorEnvironment.SensorsOpenDoorsInverted)
            {
                return true;
            }

            if (oldDoorEnvironment.SensorsOpenMaxDoorsBalanced !=
                    newDoorEnvironment.SensorsOpenMaxDoorsBalanced)
            {
                return true;
            }

            if (oldDoorEnvironment.SensorsOpenMaxDoorsInverted !=
                    newDoorEnvironment.SensorsOpenMaxDoorsInverted)
            {
                return true;
            }

            if (oldDoorEnvironment.BlockedByLicence !=
                newDoorEnvironment.BlockedByLicence)
            {
                return true;
            }

            return false;
        }

        public void MarkForceDsmStart()
        {
            IsDsmStartRequired = true;
        }

        public void OnDcuDisconnected()
        {
            if (DoorEnviromentState == DoorEnvironmentState.Unknown)
                return;

            SetDoorEnvironmentState(
                DoorEnvironmentState.Unknown,
                DoorEnvironmentStateDetail.None);
        }

        public void AddStateChangedHandler(DoorEnvironments.IStateChangedHandler stateChangedHandler)
        {
            _stateChangedHandlerGroup.Add(stateChangedHandler);
        }

        public void RemoveStateChangedHandler(DoorEnvironments.IStateChangedHandler stateChangedHandler)
        {
            _stateChangedHandlerGroup.Remove(stateChangedHandler);
        }

        protected override DoorEnvironmentSettings This
        {
            get { return this; }
        }
    }
}