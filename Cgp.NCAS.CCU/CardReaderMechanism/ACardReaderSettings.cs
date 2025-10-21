using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.Alarms;
using Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes;
using Contal.Cgp.NCAS.CCU.EventParameters;
using Contal.Drivers.CardReader;
using Contal.Drivers.LPC3250;
using Contal.IwQuick;
using Contal.IwQuick.CrossPlatform.Data;
using Contal.IwQuick.CrossPlatform.Threads;
using Contal.IwQuick.Data;
using Contal.IwQuick.Sys;
using Contal.IwQuick.Threads;
using CrSceneFrameworkCF;

using JetBrains.Annotations;
using CardReader = Contal.Drivers.CardReader.CardReader;

namespace Contal.Cgp.NCAS.CCU.CardReaderMechanism
{
    public class CardReaderConstants
    {

        public const string MENUSETALL = "MENUSETALL";

        public const string MENUUNSETALL = "MENUUNSETALL";

        public const string MENUACKNOWLEDGEALL = "MENUACKNOWLEDGEALL";

        public const string MENUALARMAREASINALARMSTATE = "MENUALARMAREASINALARMSTATE";

        public const string MENUALARMAREASNOTACKNOWLEDGED = "MENUALARMAREASNOTACKNOWLEDGED";

        public const string MENUSETAA = "MENUSETAA";

        public const string MENUSETAANOPREWARNING = "MENUSETAANOPREWARNING";

        public const string MENUUNCONDINTIONALSET = "MENUUNCONDINTIONALSET";

        public const string MENUSHOWSENSORS = "MENUSHOWSENSORS";

        public const string MENUUNSETAA = "MENUUNSETAA";

        public const string MENUUNSETANDACKNOWLEDGEAA = "MENUUNSETANDACKNOWLEDGEAA";

        public const string MENUACKNOWLEDGEAA = "MENUACKNOWLEDGEAA";

        public const string MENUSET = "MENUSET";

        public const string MENUUNSET = "MENUUNSET";

        public const string MENUALARMAREAS = "MENUALARMAREAS";

        public const string MENUALARMAREAEDIT = "MENUALARMAREAEDIT";

        public const string MENUALARMAREASPIN = "MENUALARMAREASPIN";

        public const string MENUALARMAREASGIN = "MENUALARMAREASGIN";

        public const string MENUSENSORS = "MENUSENSORS";

        public const string MENUSENSORSPIN = "MENUSENSORSPIN";

        public const string MENUSENSORSGIN = "MENUSENSORSGIN";

        public const string MENUEMERGENCYCODE = "MENUEMERGENCYCODE";

        public const string MENUEVENTLOGS = "MENUEVENTLOGS";

        public const string MENUSHOWEVENTLOGS = "MENUSHOWEVENTLOGS";

        public const string MENUEVENTLOGEDIT = "MENUEVENTLOGEDIT";

        public const string MENUBLOCKSENSORTEMPORARILYAREAUNSET =
            "MENUBLOCKSENSORTEMPORARILYAREAUNSET";

        public const string MENUBLOCKSENSORTEMPORARILYSENSORSTATENORMAL =
            "MENUBLOCKSENSORTEMPORARILYSENSORSTATENORMAL";

        public const string MENUBLOCKSENSORPERMANENTLY = "MENUBLOCKSENSORPERMANENTLY";

        public const string MENUUNBLOCKINPUT = "MENUUNBLOCKINPUT";

        public const string MENUACKNOWLEDGESENSORALARM = "MENUACKNOWLEDGESENSORALARM";

        public const string MENUACKNOWLEDGEANDTEMPORARILYBLOCKSENSORALARM =
            "MENUACKNOWLEDGEANDTEMPORARILYBLOCKSENSORALARM";

        public const string MENUACKNOWLEDGEANDPERMANENTLYBLOCKSENSORALARM =
            "MENUACKNOWLEDGEANDPERMANENTLYBLOCKSENSORALARM";

        public const string MENUSENSORINALARM = "MENUSENSORINALARM";

        public const string MENUSENSORNOTACKNOWLEDGED = "MENUSENSORNOTACKNOWLEDGED";

        public const string MENUSENSORTEMPORARILYBLOCKED = "MENUSENSORTEMPORARILYBLOCKED";

        public const string MENUSENSORPERMANENTLYBLOCKED = "MENUSENSORPERMANENTLYBLOCKED";

        public const string MENUSENSORINSABOTAGE = "MENUSENSORINSABOTAGE";

        public const string MENUSENSORSINALARM = "MENUSENSORSINALARM";

        public const string MENUSENSORSNOTACKNOWLEDGED = "MENUSENSORSNOTACKNOWLEDGED";

        public const string MENUSENSORSTEMPORARILYBLOCKED = "MENUSENSORSTEMPORARILYBLOCKED";

        public const string MENUSENSORSPERMANENTLYBLOCKED = "MENUSENSORSPERMANENTLYBLOCKED";

        public const string MENUSENSORSINSABOTAGE = "MENUSENSORSINSABOTAGE";

        public const string MENUSENSORBLOCKALL = "MENUSENSORBLOCKALL";

        public const string MENUSENSORACKNOWLEDGEALL = "MENUSENSORACKNOWLEDGEALL";

        public const string MENUSENSORACKNOWLEDGEALLALARMAREAS =
            "MENUSENSORACKNOWLEDGEALLALARMAREAS";

        public const string MENUSENSORACKNOWLEDGEANDBLOCKALL = "MENUSENSORACKNOWLEDGEANDBLOCKALL";

        public const string MENUSENSORUNBLOCKALL = "MENUSENSORUNBLOCKALL";

#if DEBUG
        public const string MENUTIMEBUYING1MIN = "MENUTIMEBUYING1MIN";
#endif

        public const string MENUTIMEBUYING30MIN = "MENUTIMEBUYING30MIN";

        public const string MENUTIMEBUYING1HOD = "MENUTIMEBUYING1HOD";

        public const string MENUTIMEBUYINGMAX = "MENUTIMEBUYINGMAX";

        public const string MENUTIMEBUYINGUNSET = "MENUTIMEBUYINGUNSET";

        public const string PROCESSING_QUEUE_NAME_SEND_COMMAND = "CardReaderMechanism: send command";

        public const string INFOBEFORESHOWMENUUNSETAA = "Info before show menu unset AA";

        public const long SHOWINFODELAYPREMIUM = 7000;

        public const long SHOWQUESTIONDELAY = 15000;

        public const long SHOWINFODELAYSLIM = 2000;

        public const long SHOWEVENTDELAY = 30000;

        public const long AUTHORIZATIONDELAY = 10000;

        public const string MenuAlarmAreaPanel = "MenuAlarmAreaPanel";
    }

    public enum ExternalAlarmAreaHandshakeState
    {
        Ready,
        WaitingForSet,
        WaitingForUnset,
        FailureToSet,
        FailureToUnset
    }

    public class ImplicitCrCodeParams
    {
        public CRMessage ImplicitCrMessage;

        public IList<CRMessage> FollowingMessages
        {
            get;
            private set;
        }

        public bool IsGinOrVariations
        {
            get;
            private set;
        }

        public DB.SecurityLevel SecurityLevel
        {
            get;
            private set;
        }

        public ImplicitCrCodeParams(
            [NotNull] CRMessage implicitCrMessage,
            [CanBeNull] IList<CRMessage> followingMessages,
            bool isGinOrVariations,
            DB.SecurityLevel securityLevel)
        {
            ImplicitCrMessage = implicitCrMessage;
            FollowingMessages = followingMessages;
            IsGinOrVariations = isGinOrVariations;
            SecurityLevel = securityLevel;
        }
    }

    internal abstract partial class ACardReaderSettings :
        AQueuedStateAndSettingsObject<ACardReaderSettings, DB.CardReader>,
        IInstanceProvider<ACardReaderSettings>,
        ISingleCardReaderEventHandler
    {
        private class SceneContextClass : ACrSceneContext
        {
            private readonly ACardReaderSettings _cardReaderSettings;

            public SceneContextClass(ACardReaderSettings cardReaderSettings)
            {
                _cardReaderSettings = cardReaderSettings;
            }

            public override CardReader CardReader
            {
                get { return _cardReaderSettings.CardReader; }
            }

            public override long ShowInfoDelay
            {
                get { return _cardReaderSettings.ShowInfoDelay; }
            }

            protected override ACrSceneGroup RootSceneGroupInstance
            {
                get { return new RootSceneGroup(_cardReaderSettings); }
            }

            protected override void NotifyCardSwiped(
                string cardData,
                int cardSystemNumber)
            {
                Events.ProcessEvent(
                    new EventCardReaderLastCardChanged(
                        _cardReaderSettings.Id,
                        cardData));
            }
        }

        private class CardReaderCommandChangedValues : IProcessingQueueRequest
        {
            private readonly Guid _guidCardReader;

            private readonly CardReaderSceneType _sceneType;

            private Guid GuidCardReader
            {
                get { return _guidCardReader; }
            }

            private CardReaderSceneType SceneType
            {
                get { return _sceneType; }
            }

            public CardReaderCommandChangedValues(
                Guid guidCardReader,
                CardReaderSceneType sceneType)
            {
                _guidCardReader = guidCardReader;
                _sceneType = sceneType;
            }

            public void Execute()
            {
                Events.ProcessEvent(
                    new EventCardReaderCommandChanged(
                        GuidCardReader,
                        (byte)SceneType));
            }

            public void OnError(Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        private class CardReaderSettingsRequest : WaitableProcessingRequest<ACardReaderSettings>
        {
            private readonly Action<ACardReaderSettings> _requestAction;

            public CardReaderSettingsRequest(Action<ACardReaderSettings> requestAction)
            {
                _requestAction = requestAction;
            }

            protected override void ExecuteInternal(ACardReaderSettings cardReaderSettings)
            {
                _requestAction(cardReaderSettings);
            }
        }

        private class TamperChangedRequest : WaitableProcessingRequest<ACardReaderSettings>
        {
            private readonly bool _isTamper;

            public TamperChangedRequest(bool isTamper)
            {
                _isTamper = isTamper;
            }

            protected override void ExecuteInternal(ACardReaderSettings cardReaderSettings)
            {
                cardReaderSettings.OnTamperChanged(_isTamper);
            }
        }

        private static ThreadPoolQueue<CardReaderCommandChangedValues> _queueDoSendCardReaderCommandChanged;

        public static readonly ExtendedVersion MinimalCrVersionForStg =
            new ExtendedVersion(
                55,
                86,
                0,
                0,
                null);

        private IDoorEnvironmentAdapter _doorEnvironmentAdapter;

        public CrEventlogProcessor CREventlogProcessor
        {
            get;
            private set;
        }

        private bool _isInTamper;

        public bool IsInTamper
        {
            get { return _isInTamper; }

            protected set
            {
                if (_isInTamper != value)
                {
                    _isInTamper = value;

                    Tampers.SendCardReaderTamper(
                        true,
                        _isInTamper,
                        Id);
                }

                if (_specialOutputForTamper == Guid.Empty)
                    return;

                if (_isInTamper)
                    Outputs.Singleton.On(
                        Id.ToString(),
                        _specialOutputForTamper);
                else
                    Outputs.Singleton.Off(
                        Id.ToString(),
                        _specialOutputForTamper);
            }
        }

        public void OnTamperChanged(bool isTamper)
        {
            IsInTamper = isTamper;

            if (!isTamper)
                ShowRootScene();
        }

        public long ShowInfoDelay
        {
            get
            {
                return IsPremium
                    ? CardReaderConstants.SHOWINFODELAYPREMIUM
                    : CardReaderConstants.SHOWINFODELAYSLIM;
            }
        }

        public ACrSceneContext SceneContext
        {
            get;
            private set;
        }

        private DB.SecurityLevel? _forcedSecurityLevel;

        private Guid _specialOutputForTamper = Guid.Empty;

        private Guid _specialOutputForOffline = Guid.Empty;

        //Driver card reader
        private CardReader _cardReader;

        //Database object card reader
        protected DB.CardReader _cardReaderDB;

        private Action<State> _eventSecurityDailyPlanChanged;

        private Action<State> _eventSecurityTimeZoneChanged;

        private DVoid2Void _eventGeneralAAReportingToCRChanged;

        private Action<Guid, State> _eventObjectForForcedSecurityLevelChanged;

        private IInputChangedListener _inputForForcedSecurityLevelChangedListener;

        private Action<Guid, State> _eventOutputForForcedSecurityLevelChanged;

        private readonly object _lockInvalidGinRetriesLimitReachedTimeout = new object();
        private ITimer _invalidGinRetriesLimitReachedTimeout;

        public CrDisplayProcessor CrDisplayProcessor
        {
            get;
            private set;
        }

        public bool InvalidCodeRetriesLimitReached
        {
            get { return _invalidCodeRetriesLimitReached; }

            private set
            {
                if (_invalidCodeRetriesLimitReached == value)
                    return;

                _invalidCodeRetriesLimitReached = value;
                    
                Events.ProcessEvent(
                    new EventCardReaderBlockedStateChanged(
                        Id,
                        _invalidCodeRetriesLimitReached));
            }
        }

        public bool InvalidCodeRetriesLimitEnabled
        {
            get
            {
                return 
                    CardReaderDb.InvalidGinRetriesLimitEnabled 
                    ?? DevicesAlarmSettings.Singleton.InvalidGinRetriesLimitEnabled;
            }
        }

        public bool InvalidPinRetriesLimitEnabled
        {
            get
            {
                return 
                    CardReaderDb.InvalidPinRetriesLimitEnabled 
                    ?? DevicesAlarmSettings.Singleton.InvalidPinRetriesLimitEnabled;
            }
        }

        public ImplicitCrCodeParams CurrentImplicitCrCodeParams
        {
            get;
            private set;
        }

        private int _numConsecutiveWrongCodes;

        public IDoorEnvironmentAdapter DoorEnvironmentAdapter
        {
            get { return _doorEnvironmentAdapter; }
        }

        public CardReader CardReader
        {
            get { return _cardReader; }
        }

        public DB.CardReader CardReaderDb
        {
            get { return _cardReaderDB; }
        }

        public DB.SecurityLevel? ForcedSecurityLevel
        {
            get { return _forcedSecurityLevel; }
        }

        public byte Address
        {
            get
            {
                return _cardReaderDB.Address;
            }
        }

        public abstract void DisplayText(
            byte left,
            byte top,
            string text);

        protected abstract string SerialPortName
        {
            get;
        }

        [NotNull]
        private ImplicitCrCodeParams CreateImplicitCrCodeParamsForDoorEnvironmentAccess(
            IAlarmAreaStateProvider alarmAreaStateProvider)
        {
            var securityLevel = SecurityLevel;

            if (securityLevel == DB.SecurityLevel.Unlocked
                && _doorEnvironmentAdapter != null)
            {
                _doorEnvironmentAdapter.ClearAccessGrantedVariables();
            }

            var crMessage = GetImplicitCrMessageFromSecurityLevel(securityLevel);

            bool isGinOrVariations = crMessage.MessageCode == CRMessageCode.WAITING_FOR_CODE;

            var stackedMessages = new List<CRMessage>();

            var crMessageLowMenuButtons =
                GetImplicitLowMenuButtonsMessage(
                    alarmAreaStateProvider);

            if (crMessageLowMenuButtons != null)
                stackedMessages.Add(crMessageLowMenuButtons);

            if (_implicitExternalAlarmAreaHandshakeState == ExternalAlarmAreaHandshakeState.WaitingForSet ||
                _implicitExternalAlarmAreaHandshakeState == ExternalAlarmAreaHandshakeState.WaitingForUnset)
            {
                stackedMessages.Add(
                    CRDisplayCommands.DisplayTextMessage(
                        null,
                        0,
                        15,
                        GetLocalizationString("EISWaiting")));

                stackedMessages.Add(
                    CRControlCommands.IndicatorAnnouncementMessage(
                        IndicatorMode.LowFrequency,
                        IndicatorMode.LowFrequency,
                        IndicatorMode.NoChange,
                        IndicatorMode.NoChange));
            }

            return new ImplicitCrCodeParams(
                crMessage,
                stackedMessages,
                isGinOrVariations,
                securityLevel);
        }

        protected ACardReaderSettings(DB.CardReader cardReaderDb) : base(cardReaderDb.IdCardReader)
        {
            _cardReaderDB = cardReaderDb;
            
            if (_queueDoSendCardReaderCommandChanged == null)
            {
                _queueDoSendCardReaderCommandChanged =
                    new ThreadPoolQueue<CardReaderCommandChangedValues>(ThreadPoolGetter.Get());
            }

            CrDisplayProcessor =
                new CrDisplayProcessor(this);

            CREventlogProcessor =
                new CrEventlogProcessor(CrDisplayProcessor);

            SceneContext = new SceneContextClass(this);
        }

        private void DailyPlansStateChanged(
            Guid idDailyPlan,
            State state)
        {
            if (_cardReaderDB.FunctionKey1 != null
                    && idDailyPlan == _cardReaderDB.FunctionKey1.IdTimeZoneOrDailyPlan
                || _cardReaderDB.FunctionKey2 != null
                    && idDailyPlan == _cardReaderDB.FunctionKey2.IdTimeZoneOrDailyPlan)
            {
                UpdateRootScene();
            }
        }

        private void TimeZonesStateChanged(
            Guid idTimeZone,
            State state)
        {
            if (_cardReaderDB.FunctionKey1 != null
                    && idTimeZone == _cardReaderDB.FunctionKey1.IdTimeZoneOrDailyPlan
                || _cardReaderDB.FunctionKey2 != null
                    && idTimeZone == _cardReaderDB.FunctionKey2.IdTimeZoneOrDailyPlan)
            {
                UpdateRootScene();
            }
        }

        private ExternalAlarmAreaHandshakeState _implicitExternalAlarmAreaHandshakeState;

        public void SetImplicitExternalAlarmAreaHandshakeState(ExternalAlarmAreaHandshakeState externalAlarmAreaHandshakeState)
        {
            _implicitExternalAlarmAreaHandshakeState = externalAlarmAreaHandshakeState;
            UpdateRootScene();
        }

        public string GetLocalizationString(string text)
        {
            return
                CcuCore.Singleton.LocalizationHelper.GetString(
                    text,
                    _cardReader.Language.ToString());
        }

        protected override void ApplyHwSetup(DB.CardReader dbObject)
        {
            if (ConfigurationState == ConfigurationState.ApplyingHwSetupNew)
            {
                var crComm = CrCommunicator;

                ApplyHwSetupInternal(
                    crComm != null
                        ? crComm.GetCardReader(Address)
                        : null);

                return;
            }

            ApplyHwSetupInternal(_cardReader);
        }

        private void ApplyHwSetupInternal(CardReader cardReader)
        {
            if (_cardReader != null)
                _cardReader.EventHandler.Remove(this);

            bool wasOffline = _cardReader == null;

            _cardReader =
                cardReader != null && cardReader.IsOnline
                    ? cardReader
                    : null;

            if (_cardReader != null)
                _cardReader.EventHandler.Add(this);

            var dcuLogicalAddress = DcuLogicalAddress;

            if (_cardReader == null)
            {
                if (!wasOffline)
                    Events.ProcessEvent(
                        new EventCardReaderOnlineStateChanged(
                            false,
                            dcuLogicalAddress,
                            SerialPortName,
                            Address,
                            string.Empty,
                            string.Empty,
                            string.Empty,
                            0));

                IsInTamper = false;

                if (_specialOutputForOffline != Guid.Empty)
                    OnSpecialOutputForOffline();

                CrOfflineAlarm.Update(
                    CardReaderDb,
                    false);

                SetImplicitCrCode();

                return;
            }

            if (wasOffline)
                Events.ProcessEvent(
                    new EventCardReaderOnlineStateChanged(
                        true,
                        dcuLogicalAddress,
                        SerialPortName,
                        Address,
                        _cardReader.ProtocolVersion,
                        _cardReader.FirmwareVersion,
                        ((byte)_cardReader.HardwareVersion).ToString(CultureInfo.InvariantCulture),
                        _cardReader.ProtocolVersionHigh));

            IsInTamper = _cardReader.IsInSabotage;

            if (_specialOutputForOffline != Guid.Empty)
                OffSpecialOutputForOffline();

            CrOfflineAlarm.Update(
                CardReaderDb,
                true);

            if (IsPresentationMaskChanged(_cardReaderDB))
            {
                _cardReader.ApplyPresentationMask();

                if (_doorEnvironmentAdapter != null)
                    _doorEnvironmentAdapter.ForceLooseCardReader();
            }

            _cardReader.ApplyTimeSettings();

            SetImplicitCrCode();

            CardReader.ControlCommands.SetScreensaver(
                !_cardReaderDB.DisableScreensaver);

            if (_cardReader.ApplyLanguageSettings((CRLanguage) _cardReaderDB.CRLanguage))
                return;

            if (_cardReader.IsMifare)
            {
                CardSystemData.Singleton
                    .CardReaderToOnlineState(Id);

                _cardReader.SectorDataEncoding = CardSystemData.Singleton.Encoding;
            }

            if (_doorEnvironmentAdapter == null 
                || (!_doorEnvironmentAdapter.IsCrStartScheduled 
                    && !_doorEnvironmentAdapter.IsCardReaderSuppressed))
                {
                    SceneContext.EnterRootScene();
                }
        }

        public void SetCardReaderEncoding(CRSectorDataEncoding encoding)
        {
            if (_cardReader != null
                && _cardReader.IsMifare
                && _cardReader.SectorDataEncoding != encoding)
            {
                _cardReader.SectorDataEncoding = encoding;
            }
        }

        private volatile NativeTimer _timerChangeSpecialOutputForOffline;

        private readonly object _lockTimerChangeSpecialOutputForOffline =
            new object();

        private bool _setSpecialOutputForOfflineToOn;

        private const int FILTER_TIME_SPECIAL_OUTPUT_FOR_OFFLINE = 500;

        private void OnSpecialOutputForOffline()
        {
            lock (_lockTimerChangeSpecialOutputForOffline)
            {
                if (_timerChangeSpecialOutputForOffline == null)
                {
                    _setSpecialOutputForOfflineToOn = true;

                    _timerChangeSpecialOutputForOffline =
                        NativeTimerManager.StartTimeout(
                            FILTER_TIME_SPECIAL_OUTPUT_FOR_OFFLINE,
                            OnTimerChangeSpecialOutputForOffline,
                            (byte)PrirotyForOnTimerEvent.CardReaders);
                }
                else
                {
                    if (!_setSpecialOutputForOfflineToOn)
                        StopTimerChangeSpecialOutputForOffline();
                }
            }
        }

        private void OffSpecialOutputForOffline()
        {
            lock (_lockTimerChangeSpecialOutputForOffline)
            {
                if (_timerChangeSpecialOutputForOffline == null)
                {
                    _setSpecialOutputForOfflineToOn = false;

                    _timerChangeSpecialOutputForOffline =
                        NativeTimerManager.StartTimeout(
                            FILTER_TIME_SPECIAL_OUTPUT_FOR_OFFLINE,
                            OnTimerChangeSpecialOutputForOffline,
                            (byte)PrirotyForOnTimerEvent.CardReaders);
                }
                else
                {
                    if (_setSpecialOutputForOfflineToOn)
                        StopTimerChangeSpecialOutputForOffline();
                }
            }
        }

        private void StopTimerChangeSpecialOutputForOffline()
        {
            lock (_lockTimerChangeSpecialOutputForOffline)
            {
                if (_timerChangeSpecialOutputForOffline == null)
                    return;

                try
                {
                    _timerChangeSpecialOutputForOffline.StopTimer();
                }
                catch
                {
                }

                _timerChangeSpecialOutputForOffline = null;
            }
        }

        private bool OnTimerChangeSpecialOutputForOffline(NativeTimer timer)
        {
            lock (_lockTimerChangeSpecialOutputForOffline)
            {
                _timerChangeSpecialOutputForOffline = null;

                if (_setSpecialOutputForOfflineToOn)
                    Outputs.Singleton.On(
                        Id.ToString(),
                        _specialOutputForOffline);
                else
                    Outputs.Singleton.Off(
                        Id.ToString(),
                        _specialOutputForOffline);
            }

            return true;
        }

        private bool IsPresentationMaskChanged(DB.CardReader cardReader)
        {
            var changed = false;

            if (_cardReader.KeyPressedOnExternalBuzzer != cardReader.KeyPressedExternalBuzzer)
            {
                _cardReader.KeyPressedOnExternalBuzzer = cardReader.KeyPressedExternalBuzzer;
                changed = true;
            }

            if (_cardReader.KeyPressedOnInternalBuzzer != cardReader.KeyPressedInternalBuzzer)
            {
                _cardReader.KeyPressedOnInternalBuzzer = cardReader.KeyPressedInternalBuzzer;
                changed = true;
            }

            if (_cardReader.KeyPressedOnLED != cardReader.KeyPressedLED)
            {
                _cardReader.KeyPressedOnLED = cardReader.KeyPressedLED;
                changed = true;
            }

            if (_cardReader.KeyPressedOnKeyboardLight != cardReader.KeyPressedKeyboardLight)
            {
                _cardReader.KeyPressedOnKeyboardLight = cardReader.KeyPressedKeyboardLight;
                changed = true;
            }

            if (_cardReader.TamperOnLED != cardReader.TamperLED)
            {
                _cardReader.TamperOnLED = cardReader.TamperLED;
                changed = true;
            }

            if (_cardReader.TamperOnKeyboardLight != cardReader.TamperKeyboardLight)
            {
                _cardReader.TamperOnKeyboardLight = cardReader.TamperKeyboardLight;
                changed = true;
            }

            if (_cardReader.TamperOnInternalBuzzer != cardReader.TamperInternalBuzzer)
            {
                _cardReader.TamperOnInternalBuzzer = cardReader.TamperInternalBuzzer;
                changed = true;
            }

            if (_cardReader.TamperOnExternalBuzzer != cardReader.TamperExternalBuzzer)
            {
                _cardReader.TamperOnExternalBuzzer = cardReader.TamperExternalBuzzer;
                changed = true;
            }

            if (_cardReader.ResetOnLED != cardReader.ResetLED)
            {
                _cardReader.ResetOnLED = cardReader.ResetLED;
                changed = true;
            }

            if (_cardReader.ResetOnKeyboardLight != cardReader.ResetKeyboardLight)
            {
                _cardReader.ResetOnKeyboardLight = cardReader.ResetKeyboardLight;
                changed = true;
            }

            if (_cardReader.ResetOnInternalBuzzer != cardReader.ResetInternalBuzzer)
            {
                _cardReader.ResetOnInternalBuzzer = cardReader.ResetInternalBuzzer;
                changed = true;
            }

            if (_cardReader.ResetOnExternalBuzzer != cardReader.ResetExternalBuzzer)
            {
                _cardReader.ResetOnExternalBuzzer = cardReader.ResetExternalBuzzer;
                changed = true;
            }

            if (_cardReader.CardAppliedOnLED != cardReader.CardAppliedLED)
            {
                _cardReader.CardAppliedOnLED = cardReader.CardAppliedLED;
                changed = true;
            }

            if (_cardReader.CardAppliedOnKeyboardLight != cardReader.CardAppliedKeyboardLight)
            {
                _cardReader.CardAppliedOnKeyboardLight = cardReader.CardAppliedKeyboardLight;
                changed = true;
            }

            if (_cardReader.CardAppliedOnInternalBuzzer != cardReader.CardAppliedInternalBuzzer)
            {
                _cardReader.CardAppliedOnInternalBuzzer = cardReader.CardAppliedInternalBuzzer;
                changed = true;
            }

            if (_cardReader.CardAppliedOnExternalBuzzer != cardReader.CardAppliedExternalBuzzer)
            {
                _cardReader.CardAppliedOnExternalBuzzer = cardReader.CardAppliedExternalBuzzer;
                changed = true;
            }

            if (_cardReader.InternalBuzzerKillSwitch != cardReader.InternalBuzzerKillswitch)
            {
                _cardReader.InternalBuzzerKillSwitch = cardReader.InternalBuzzerKillswitch;
                changed = true;
            }

            if (_cardReader.SlGinCodeLedPresentation != cardReader.SlCodeLedPresentation)
            {
                _cardReader.SlGinCodeLedPresentation = cardReader.SlCodeLedPresentation;
                changed = true;
            }

            return changed;
        }

        /// <summary>
        /// Unbinds events according to card reader object
        /// </summary>
        protected void DetachEvents()
        {
            if (_eventTimeZonesStateChanged != null)
            {
                TimeZones.Singleton.TimeZonesStateChanged -= _eventTimeZonesStateChanged;
                _eventTimeZonesStateChanged = null;
            }

            if (_eventDailyPlansStateChanged != null)
            {
                DailyPlans.Singleton.DailyPlansStateChanged -= _eventDailyPlansStateChanged;
                _eventDailyPlansStateChanged = null;
            }

            if (_eventObjectForForcedSecurityLevelChanged != null)
            {
                if (_cardReaderDB != null
                    && _cardReaderDB.OnOffObjectId != null)
                {
                    switch (_cardReaderDB.OnOffObjectObjectType)
                    {
                        case ObjectType.TimeZone:

                            TimeZones.Singleton.RemoveEvent(
                                _cardReaderDB.OnOffObjectId.Value,
                                _eventObjectForForcedSecurityLevelChanged);

                            break;

                        case ObjectType.DailyPlan:

                            DailyPlans.Singleton.RemoveEvent(
                                _cardReaderDB.OnOffObjectId.Value,
                                _eventObjectForForcedSecurityLevelChanged);

                            break;
                    }
                }

                _eventObjectForForcedSecurityLevelChanged = null;
                _forcedSecurityLevel = null;
            }

            if (_inputForForcedSecurityLevelChangedListener != null)
            {
                if (_cardReaderDB != null
                    && _cardReaderDB.OnOffObjectId != null)
                {
                    Inputs.Singleton.RemoveInputChangedListener(
                        _cardReaderDB.OnOffObjectId.Value,
                        _inputForForcedSecurityLevelChangedListener);
                }

                _inputForForcedSecurityLevelChangedListener = null;
                _forcedSecurityLevel = null;
            }

            if (_eventOutputForForcedSecurityLevelChanged != null)
            {
                if (_cardReaderDB != null
                    && _cardReaderDB.OnOffObjectId != null)
                {
                    Outputs.Singleton.RemoveOutputChanged(
                        _cardReaderDB.OnOffObjectId.Value,
                        _eventOutputForForcedSecurityLevelChanged);
                }

                _eventOutputForForcedSecurityLevelChanged = null;
                _forcedSecurityLevel = null;
            }

            if (_eventSecurityTimeZoneChanged != null)
            {
                if (_cardReaderDB != null)
                    SecurityTimeZones.Singleton.RemoveEvent(
                        _cardReaderDB.GuidSecurityTimeZone,
                        _eventSecurityTimeZoneChanged);

                _eventSecurityTimeZoneChanged = null;
            }

            if (_eventSecurityDailyPlanChanged != null)
            {
                if (_cardReaderDB != null)
                    SecurityDailyPlans.Singleton.RemoveEvent(
                        _cardReaderDB.GuidSecurityDailyPlan,
                        _eventSecurityDailyPlanChanged);

                _eventSecurityDailyPlanChanged = null;
            }

            if (_eventGeneralAAReportingToCRChanged != null)
            {
                AlarmArea.AlarmAreas.Singleton.GeneralAlarmAreasReportingToCRChanged -=
                    _eventGeneralAAReportingToCRChanged;

                _eventGeneralAAReportingToCRChanged = null;
            }

            if (_doorEnvironmentAdapter != null)
                _doorEnvironmentAdapter.DetachEvents();
        }

        public void RemoveSpecialOutputsForOfflineAndTamper(DB.CardReader newCardReader)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "void CardReaderMechanism.RemoveSpecialOutputsForOfflineAndTamper(DB.CardReader newCardReader): [{0}]",
                    Log.GetStringFromParameters(newCardReader)));

            if (_specialOutputForOffline != Guid.Empty)
            {
                if (newCardReader == null
                    || _specialOutputForOffline != newCardReader.GuidSpecialOutputForOffline)
                {
                    Outputs.Singleton.Off(
                        Id.ToString(),
                        _specialOutputForOffline);

                    _specialOutputForOffline = Guid.Empty;
                }
            }

            if (_specialOutputForTamper != Guid.Empty)
            {
                if (newCardReader == null
                    || _specialOutputForTamper != newCardReader.GuidSpecialOutputForTamper)
                {
                    Outputs.Singleton.Off(
                        Id.ToString(),
                        _specialOutputForTamper);

                    _specialOutputForTamper = Guid.Empty;
                }
            }
        }

        public void SendActualQueryDbStamp()
        {
            if (_cardReader != null)
                _cardReader.MifareCommands.QueryDbStamp(_cardReader);
        }

        /// <summary>
        /// Binds events according to card reader object
        /// </summary>
        private void AttachEvents()
        {
            try
            {
                if (_doorEnvironmentAdapter != null)
                    _doorEnvironmentAdapter.AttachEvents();

                if (_cardReaderDB.SecurityLevel
                    == (byte)DB.SecurityLevel.SecurityTimeZoneSecurityDailyPlan
                    && _cardReaderDB.GuidSecurityTimeZone != Guid.Empty)
                {
                    _eventSecurityTimeZoneChanged =
                        SecurityTimeZoneSecurityDailyPlan_ChangeState;

                    SecurityTimeZones.Singleton.AddEvent(
                        _cardReaderDB.GuidSecurityTimeZone,
                        _eventSecurityTimeZoneChanged);
                }

                if (_cardReaderDB.SecurityLevel
                    == (byte)DB.SecurityLevel.SecurityTimeZoneSecurityDailyPlan
                    && _cardReaderDB.GuidSecurityDailyPlan != Guid.Empty)
                {
                    _eventSecurityDailyPlanChanged =
                        SecurityTimeZoneSecurityDailyPlan_ChangeState;

                    SecurityDailyPlans.Singleton.AddEvent(
                        _cardReaderDB.GuidSecurityDailyPlan,
                        _eventSecurityDailyPlanChanged);
                }

                if (_cardReaderDB.IsForcedSecurityLevel
                    && _cardReaderDB.OnOffObjectId != null)
                {
                    switch (_cardReaderDB.OnOffObjectObjectType)
                    {
                        case ObjectType.TimeZone:

                            _eventObjectForForcedSecurityLevelChanged =
                                ObjectForForcedSecurityLevelChanged;

                            TimeZones.Singleton.AddEvent(
                                _cardReaderDB.OnOffObjectId.Value,
                                _eventObjectForForcedSecurityLevelChanged);

                            break;

                        case ObjectType.DailyPlan:

                            _eventObjectForForcedSecurityLevelChanged =
                                ObjectForForcedSecurityLevelChanged;

                            DailyPlans.Singleton.AddEvent(
                                _cardReaderDB.OnOffObjectId.Value,
                                _eventObjectForForcedSecurityLevelChanged);

                            break;

                        case ObjectType.Input:

                            _inputForForcedSecurityLevelChangedListener =
                                new InputForForcedSecurityLevelChangedListener(this);

                            InputForForcedSecurityLevelChanged(
                                _cardReaderDB.OnOffObjectId.Value,
                                Inputs.Singleton.GetInputLogicalState(_cardReaderDB.OnOffObjectId.Value));

                            Inputs.Singleton.AddInputChangedListener(
                                _cardReaderDB.OnOffObjectId.Value,
                                _inputForForcedSecurityLevelChangedListener);

                            break;

                        case ObjectType.Output:

                            _eventOutputForForcedSecurityLevelChanged =
                                OutputForForcedSecurityLevelChanged;

                            OutputForForcedSecurityLevelChanged(
                                _cardReaderDB.OnOffObjectId.Value,
                                Outputs.Singleton.GetOutputState(_cardReaderDB.OnOffObjectId.Value));

                            Outputs.Singleton.AddOutputChanged(
                                _cardReaderDB.OnOffObjectId.Value,
                                _eventOutputForForcedSecurityLevelChanged);

                            break;
                    }
                }

                _eventTimeZonesStateChanged = TimeZonesStateChanged;

                TimeZones.Singleton.TimeZonesStateChanged +=
                    _eventTimeZonesStateChanged;

                _eventDailyPlansStateChanged = DailyPlansStateChanged;

                DailyPlans.Singleton.DailyPlansStateChanged +=
                    _eventDailyPlansStateChanged;

                _eventGeneralAAReportingToCRChanged =
                    GeneralAlarmAreaReportingToCRsChanged;

                AlarmArea.AlarmAreas.Singleton.GeneralAlarmAreasReportingToCRChanged +=
                    _eventGeneralAAReportingToCRChanged;
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        private void GeneralAlarmAreaReportingToCRsChanged()
        {
            UpdateRootScene();
        }

        [NotNull]
        private ImplicitCrCodeParams CreateImplicitCrCodeParamsForSetImplicitAlarmArea(
            [NotNull]
            CrAlarmAreasManager.CrAlarmAreaInfo implicitCrAlarmAreaInfo)
        {
            var stackedMessages = new List<CRMessage>();

            var securityLevel = SecurityLevel;

            var subMessageLowMenuButtons =
                GetImplicitLowMenuButtonsMessage(
                    implicitCrAlarmAreaInfo);

            if (subMessageLowMenuButtons != null)
                stackedMessages.Add(subMessageLowMenuButtons);

            if (_implicitExternalAlarmAreaHandshakeState == ExternalAlarmAreaHandshakeState.WaitingForSet
                || _implicitExternalAlarmAreaHandshakeState == ExternalAlarmAreaHandshakeState.WaitingForUnset)
            {
                stackedMessages.Add(
                    CRDisplayCommands.DisplayTextMessage(
                        null,
                        0,
                        15,
                        GetLocalizationString("EISWaiting")));

                stackedMessages.Add(
                    CRControlCommands.IndicatorAnnouncementMessage(
                        IndicatorMode.LowFrequency,
                        IndicatorMode.LowFrequency,
                        IndicatorMode.NoChange,
                        IndicatorMode.NoChange));
            }

            var alarmState = implicitCrAlarmAreaInfo.AlarmState;
            var activationState = implicitCrAlarmAreaInfo.ActivationState;

            int optionalData = -1;

            bool setCrMessageCodeFromAlarmAreaState = true;

            bool isCrReportingEnabled =
                implicitCrAlarmAreaInfo.IsCrReportingEnabled;

            switch (activationState)
            {
                case State.Set:

                    if (alarmState == State.Alarm && isCrReportingEnabled)
                        optionalData = (int) CRAATmpUnsetFlag.DEFAULT_APPEARANCE;

                    break;

                case State.Prewarning:

                    if (isCrReportingEnabled)
                        stackedMessages.Add(
                            CRControlCommands.IndicatorAnnouncementMessage(
                                IndicatorMode.UltraLowFrequency,
                                IndicatorMode.NoChange,
                                IndicatorMode.UltraLowFrequency,
                                IndicatorMode.NoChange));

                    setCrMessageCodeFromAlarmAreaState = false;

                    break;

                case State.TemporaryUnsetExit:
                case State.TemporaryUnsetEntry:
                    {
                        var crAATmpUnsetFlag =
                            activationState == State.TemporaryUnsetEntry
                                ? CRAATmpUnsetFlag.IS_ENTRY
                                : 0x00;

                        switch (securityLevel)
                        {
                            case DB.SecurityLevel.Unlocked:

                                setCrMessageCodeFromAlarmAreaState = false;
                                crAATmpUnsetFlag |= CRAATmpUnsetFlag.USUAL_UNLOCKED;

                                break;

                            case DB.SecurityLevel.Locked:

                                setCrMessageCodeFromAlarmAreaState = false;
                                crAATmpUnsetFlag |= CRAATmpUnsetFlag.USUAL_LOCKED;

                                break;

                            case DB.SecurityLevel.Code:

                                setCrMessageCodeFromAlarmAreaState = false;
                                crAATmpUnsetFlag |= CRAATmpUnsetFlag.USUAL_GIN;

                                break;

                            case DB.SecurityLevel.CodeOrCard:
                            case DB.SecurityLevel.CodeOrCardPin:

                                setCrMessageCodeFromAlarmAreaState = false;
                                crAATmpUnsetFlag |= CRAATmpUnsetFlag.USUAL_GIN_CARD;

                                break;
                        }

                        if (setCrMessageCodeFromAlarmAreaState)
                            optionalData =
                                (int)
                                (crAATmpUnsetFlag |
                                    (isCrReportingEnabled
                                        ? CRAATmpUnsetFlag.DEFAULT_APPEARANCE
                                        : CRAATmpUnsetFlag.INDICATORS));
                        else
                            stackedMessages.Add(
                                CRAlarmAreaCommands.AlarmAreaTmpUnsetMessage(
                                    isCrReportingEnabled
                                        ? crAATmpUnsetFlag | CRAATmpUnsetFlag.TEXT
                                        : crAATmpUnsetFlag));

                        break;
                    }
            }

            CRMessage crMessage =
                setCrMessageCodeFromAlarmAreaState
                    ? GetImplicitCrMessageFromAlarmAreaState(
                        implicitCrAlarmAreaInfo,
                        optionalData,
                        isCrReportingEnabled)
                    : GetImplicitCrMessageFromSecurityLevel(securityLevel);

            bool isGinOrVariations = crMessage.MessageCode == CRMessageCode.WAITING_FOR_CODE;

            return new ImplicitCrCodeParams(
                crMessage,
                stackedMessages,
                isGinOrVariations,
                securityLevel);
        }

        public void DoorEnvironmentStateChanged(
            DoorEnvironments.DoorEnvironmentStateChangedArgs doorEnvironmentStateChangedArgs)
        {
            // the CRM (and respective APBZ) is interested in the DoorEnviromentStateChanged event
            // only if the event's stimulus occurred on the current CR.

            if (doorEnvironmentStateChangedArgs.GuidCardReaderAccessed == Id)
                CardReaders.Singleton.FireAccessViaCardReader(doorEnvironmentStateChangedArgs);
        }

        public void DetachDoorEnvironmentAdapter(Guid idDoorEnvironment)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "void CardReaderMechanism.DetachDoorEnvironmentAdapter(Guid idDoorEnvironment): [{0}]",
                    Log.GetStringFromParameters(idDoorEnvironment)));

            if (_doorEnvironmentAdapter == null
                || !_doorEnvironmentAdapter.IsAssociatedWithDoorEnvironment(idDoorEnvironment))
            {
                return;
            }

            DetachAdapterCore();
        }

        private void DetachAdapterCore()
        {
            _doorEnvironmentAdapter.DetachEvents();
            _doorEnvironmentAdapter = null;

            try
            {
                ShowRootScene();
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        public void DetachMultiDoorAdapter(Guid idMultiDoor)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "void CardReaderMechanism.DetachMultiDoorAdapter(Guid idMultiDoor) : [{0}]",
                    Log.GetStringFromParameters(idMultiDoor)));

            if (_doorEnvironmentAdapter == null
                || !_doorEnvironmentAdapter.IsAssociatedWithMultiDoor(idMultiDoor))
            {
                return;
            }

            DetachAdapterCore();
        }

        internal void UpdateRootScene()
        {
            SetImplicitCrCode();

            SceneContext.UpdateTopMostScene();
        }

        public void ShowRootScene()
        {
            SetImplicitCrCode();

            SceneContext.EnterRootScene();
        }

        private void SetImplicitCrCode()
        {
            CurrentImplicitCrCodeParams = CreateImplicitCrCodeParams();

            if (_cardReader != null)
                SendCurrentCardReaderCommand();

            if (CurrentImplicitCrCodeParams == null)
                return;

            if (_doorEnvironmentAdapter != null
                && !_doorEnvironmentAdapter.IsCrStartScheduled)
            {
                _doorEnvironmentAdapter.SetImplicitCrCode(
                    CurrentImplicitCrCodeParams.ImplicitCrMessage,
                    CurrentImplicitCrCodeParams.FollowingMessages,
                    CurrentImplicitCrCodeParams.IsGinOrVariations);
            }
        }

        [CanBeNull]
        private ImplicitCrCodeParams CreateImplicitCrCodeParams()
        {
            if (_doorEnvironmentAdapter == null)
                return null;

            var implicitAlarmAreaInfo = ImplicitCrAlarmAreaInfo;

            return implicitAlarmAreaInfo != null && implicitAlarmAreaInfo.IsSet
                ? CreateImplicitCrCodeParamsForSetImplicitAlarmArea(implicitAlarmAreaInfo)
                : CreateImplicitCrCodeParamsForDoorEnvironmentAccess(implicitAlarmAreaInfo);
        }

        private static bool IsCrForcedSecurityLevelObjectOnInternal(DB.CardReader cardReader)
        {
            if (cardReader.OnOffObjectId == null)
                return false;

            switch (cardReader.OnOffObjectObjectType)
            {
                case ObjectType.DailyPlan:

                    return DailyPlans.Singleton.GetActualState(cardReader.OnOffObjectId.Value) == State.On;

                case ObjectType.TimeZone:

                    return TimeZones.Singleton.GetActualState(cardReader.OnOffObjectId.Value) == State.On;

                case ObjectType.Output:

                    return Outputs.Singleton.GetOutputState(cardReader.OnOffObjectId.Value) == State.On;

                case ObjectType.Input:

                    return Inputs.Singleton.GetInputLogicalState(cardReader.OnOffObjectId.Value) == State.Alarm;
            }

            return false;
        }

        public DB.SecurityLevel SecurityLevel
        {
            get
            {
                var securityLevel =
                    _forcedSecurityLevel
                    ?? (DB.SecurityLevel)_cardReaderDB.SecurityLevel;

                var result =
                    securityLevel != DB.SecurityLevel.SecurityTimeZoneSecurityDailyPlan
                        ? securityLevel
                        : GetSecurityLevelFromStzOrSdp(_cardReaderDB);

                if (_cardReader == null || _cardReader.HasKeyboard)
                    return result;

                switch (result)
                {
                    case DB.SecurityLevel.Locked:
                    case DB.SecurityLevel.Unlocked:

                        return result;

                    default:

                        return DB.SecurityLevel.Card;
                }
            }
        }

        private static DB.SecurityLevel GetSecurityLevelFromStzOrSdp(
            [NotNull]
            DB.CardReader cardReaderDb)
        {
            if (cardReaderDb.GuidSecurityTimeZone != Guid.Empty)
                return
                    GetSecurityLevelFromStzOrSdpState(
                        SecurityTimeZones.Singleton
                            .GetActualState(cardReaderDb.GuidSecurityTimeZone));

            if (cardReaderDb.GuidSecurityDailyPlan != Guid.Empty)
                return
                    GetSecurityLevelFromStzOrSdpState(
                        SecurityDailyPlans.Singleton
                            .GetActualState(cardReaderDb.GuidSecurityDailyPlan));

            return DB.SecurityLevel.Locked;
        }

        public static DB.SecurityLevel GetSecurityLevelFromStzOrSdpState(State state)
        {
            switch (state)
            {
                case State.card:

                    return DB.SecurityLevel.Card;

                case State.cardpin:

                    return DB.SecurityLevel.CardPIN;

                case State.code:

                    return DB.SecurityLevel.Code;

                case State.codecard:

                    return DB.SecurityLevel.CodeOrCard;

                case State.codecardpin:

                    return DB.SecurityLevel.CodeOrCardPin;

                case State.unlocked:

                    return DB.SecurityLevel.Unlocked;

                default:

                    return DB.SecurityLevel.Locked;
            }
        }

        private void SecurityTimeZoneSecurityDailyPlan_ChangeState(State state)
        {
            UpdateRootScene();
        }

        private void ObjectForForcedSecurityLevelChanged(
            Guid initiatedBy,
            State state)
        {
            switch (state)
            {
                case State.On:

                    if (_cardReaderDB != null)
                        _forcedSecurityLevel =
                            (DB.SecurityLevel)
                                _cardReaderDB.ForcedSecurityLevel;

                    break;

                case State.Off:

                    _forcedSecurityLevel = null;

                    break;
            }

            ShowRootScene();
        }

        private void InputForForcedSecurityLevelChanged(
            Guid inputGuid,
            State inputState)
        {
            if (_cardReaderDB == null
                || _cardReaderDB.OnOffObjectId != inputGuid)
            {
                return;
            }

            switch (inputState)
            {
                case State.Alarm:

                    if (_cardReaderDB != null)
                        _forcedSecurityLevel =
                            (DB.SecurityLevel)
                                _cardReaderDB.ForcedSecurityLevel;

                    break;

                case State.Normal:

                    _forcedSecurityLevel = null;

                    break;
            }

            ShowRootScene();
        }

        private void OutputForForcedSecurityLevelChanged(
            Guid outputGuid,
            State outputState)
        {
            switch (outputState)
            {
                case State.On:

                    if (_cardReaderDB != null)
                        _forcedSecurityLevel =
                            (DB.SecurityLevel)
                                _cardReaderDB.ForcedSecurityLevel;

                    break;

                case State.Off:

                    _forcedSecurityLevel = null;

                    break;
            }

            ShowRootScene();
        }

        public const int BEGIN_MENU_CR_COMMANDS = 30;

        public const int END_MENU_CR_COMMANDS = 69;

        public const int BEGIN_MENU_EVENTLOGS_CR_COMMANDS = 80;

        public const int END_MENU_EVENTLOGS_CR_COMMANDS = 91;

        public const int BEGIN_QUICK_SET_AA_CR_COMMANDS = 16;

        public const int END_QUICK_SET_AA_CR_COMMANDS = 28;

        public const int BEGIN_TIME_BUYING_AA_CR_COMMANDS = 92;

        public const int END_TIME_BUYING_AA_CR_COMMANDS = 97;

        public CRMessage GetImplicitLowMenuButtonsMessage(
            IAlarmAreaStateProvider alarmAreaStateProvider)
        {
            return CreateImplicitLowMenuButtonsMessage(
                _cardReader,
                alarmAreaStateProvider,
                _cardReaderDB.FunctionKey1,
                _cardReaderDB.FunctionKey2);
        }

        public static bool HasAccessByTimeZoneOrDailyPlan(DB.FunctionKey functionKey)
        {
            if (functionKey == null)
                return false;

            if (functionKey.IdTimeZoneOrDailyPlan == Guid.Empty)
                return true;

            var state =
                functionKey.IsUsedTimeZone
                    ? TimeZones.Singleton.GetActualState(functionKey.IdTimeZoneOrDailyPlan)
                    : DailyPlans.Singleton.GetActualState(functionKey.IdTimeZoneOrDailyPlan);

            return state == State.On;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cr"></param>
        /// <param name="implicitAlarmAreaStateProvider"></param>
        /// <param name="functionKey1"></param>
        /// <param name="functionKey2"></param>
        /// <returns></returns>
        private static CRMessage CreateImplicitLowMenuButtonsMessage(
            CardReader cr, 
            IAlarmAreaStateProvider implicitAlarmAreaStateProvider, 
            DB.FunctionKey functionKey1,
            DB.FunctionKey functionKey2)
        {
            if (cr == null)
                return null;

            var functionKey1Button = CRMenuButtonLook.Clear;
            var functionKey2Button = CRMenuButtonLook.Clear;
            var aaButtonSpecialKey = CRSpecialKey.Down;

            if (functionKey1 != null
                && functionKey1.isEnable
                && HasAccessByTimeZoneOrDailyPlan(functionKey1))
            {
                functionKey1Button =
                    cr.FwVersion >= MinimalCrVersionForStg
                        ? CRMenuButtonLook.F1
                        : CRMenuButtonLook.Up;
            }

            if (functionKey2 != null
                && functionKey2.isEnable
                && HasAccessByTimeZoneOrDailyPlan(functionKey2))
            {
                functionKey2Button =
                    cr.FwVersion >= MinimalCrVersionForStg
                        ? CRMenuButtonLook.F2
                        : CRMenuButtonLook.Down;
            }

            var alarmAreaButton = CRMenuButtonLook.Clear;

            if (implicitAlarmAreaStateProvider != null)
            {
                if (implicitAlarmAreaStateProvider.IsSet && implicitAlarmAreaStateProvider.IsUnsettable)
                    alarmAreaButton = CRMenuButtonLook.Unlock;
                else if (implicitAlarmAreaStateProvider.IsUnset && implicitAlarmAreaStateProvider.IsSettable)
                    alarmAreaButton = CRMenuButtonLook.Lock;
            }

            switch (alarmAreaButton)
            {
                case CRMenuButtonLook.Unlock:
                    aaButtonSpecialKey = CRSpecialKey.Unlock;
                    break;

                case CRMenuButtonLook.Lock:
                    aaButtonSpecialKey = CRSpecialKey.Lock;
                    break;
            }

            var bottomMenu = new CRBottomMenu();

            bottomMenu.SetButton(
                0,
                functionKey1Button,
                CRSpecialKey.FunctionKey1);

            bottomMenu.SetButton(
                1,
                functionKey2Button,
                CRSpecialKey.FunctionKey2);

            bottomMenu.SetButton(
                2,
                alarmAreaButton,
                aaButtonSpecialKey);

            return
                CRMenuCommands.BottomMenuButtonsMessage(
                    cr,
                    bottomMenu);
        }

        public void SendCurrentCardReaderCommand()
        {
            DB.SecurityLevel securityLevel;
            CRMessageCode crMessageCode;

            if (CurrentImplicitCrCodeParams == null)
            {
                if (CrAlarmAreasManager.ImplicitCrAlarmAreaInfo != null
                    || CrAlarmAreasManager.AlarmAreaCount > 0)
                {
                    _queueDoSendCardReaderCommandChanged.Enqueue(
                        new CardReaderCommandChangedValues(
                            Id,
                            CardReaderSceneType.AlarmPanelState));

                    return;
                }

                securityLevel = SecurityLevel;
                crMessageCode = CRMessageCode.OUT_OF_ORDER;
            }
            else
            {
                securityLevel = CurrentImplicitCrCodeParams.SecurityLevel;
                crMessageCode = CurrentImplicitCrCodeParams.ImplicitCrMessage.MessageCode;
            }

            SendCardReaderCommandChanged(
                crMessageCode, 
                securityLevel);
        }

        private void SendCardReaderCommandChanged(
            CRMessageCode crMessageCode,
            DB.SecurityLevel securityLevel)
        {
            var command = CardReaderSceneType.Unknown;

            switch (crMessageCode)
            {
                case CRMessageCode.OUT_OF_ORDER:

                    command = CardReaderSceneType.OutOfOrder;
                    break;

                case CRMessageCode.DOOR_LOCKED:

                    command = CardReaderSceneType.DoorLocked;
                    break;

                case CRMessageCode.DOOR_UNLOCKED:

                    command = CardReaderSceneType.DoorUnlocked;
                    break;

                case CRMessageCode.WAITING_FOR_CARD:

                    switch (securityLevel)
                    {
                        case DB.SecurityLevel.Card:

                            command = CardReaderSceneType.WaitingForCard;
                            break;

                        case DB.SecurityLevel.CardPIN:

                            command = CardReaderSceneType.WaitingForCardPin;
                            break;
                    }

                    break;

                case CRMessageCode.WAITING_FOR_CODE:

                    switch (securityLevel)
                    {
                        case DB.SecurityLevel.Code:

                            command = CardReaderSceneType.WaitingForCode;
                            break;

                        case DB.SecurityLevel.CodeOrCard:

                            command = CardReaderSceneType.WaitingForCodeOrCard;
                            break;

                        case DB.SecurityLevel.CodeOrCardPin:

                            command = CardReaderSceneType.WaitingForCodeOrCardPin;
                            break;
                    }

                    break;

                case CRMessageCode.ALARM_AREA_IS_SET:

                    command = CardReaderSceneType.AlarmAreaSet;
                    break;

                case CRMessageCode.ALARM_AREA_TMP_UNSET_ENTRY:

                    command = CardReaderSceneType.AlarmAreaUnsetEntry;
                    break;

                case CRMessageCode.ALARM_AREA_TMP_UNSET_EXIT:

                    command = CardReaderSceneType.AlarmAreaUnsetExit;
                    break;

                case CRMessageCode.ALARM_AREA_IN_ALARM:

                    command = CardReaderSceneType.AlarmAreaAlarm;
                    break;
            }

           _queueDoSendCardReaderCommandChanged.Enqueue(new CardReaderCommandChangedValues(
               Id,
               command));
        }


        public void ShowInfoBlockedByLicence()
        {
            //TODO scene
        }

        private TimeZones.TimeZonesStateChangedDelegate _eventTimeZonesStateChanged;

        private DailyPlans.DailyPlansStateChangedDelegate _eventDailyPlansStateChanged;

        private bool _invalidCodeRetriesLimitReached;

        public CrAlarmAreasManager CrAlarmAreasManager
        {
            get;
            private set;
        }

        public void UnconfigureAaCardReader(
            DB.AACardReader aaCardReader,
            bool update)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                "void CardReaderMechanism.UnconfigureAaCardReader()");

            if (CrAlarmAreasManager != null)
                CrAlarmAreasManager.UnconfigureAaCardReader(
                    aaCardReader,
                    update);

            UpdateRootScene();
        }

        private CRMessage GetImplicitCrMessageFromSecurityLevel(DB.SecurityLevel securityLevel)
        {
            switch (securityLevel)
            {
                case DB.SecurityLevel.Card:
                case DB.SecurityLevel.CardPIN:

                    if (_cardReader != null)
                        return CRAccessCommands.WaitingForCardMessage(_cardReader);

                    return CRAccessCommands.WaitingForCardMessage(_cardReaderDB.Address);

                case DB.SecurityLevel.Code:
                case DB.SecurityLevel.CodeOrCard:
                case DB.SecurityLevel.CodeOrCardPin:

                    if (_cardReader != null)
                        return CRAccessCommands.WaitingForCodeMessage(
                            _cardReader,
                            CcuCardReaders.MinimalCodeLength,
                            CcuCardReaders.MaximalCodeLength,
                            true);

                    return CRAccessCommands.WaitingForCodeMessage(
                            _cardReaderDB.Address,
                            CcuCardReaders.MinimalCodeLength,
                            CcuCardReaders.MaximalCodeLength,
                            true);

                case DB.SecurityLevel.Unlocked:

                    if (_cardReader != null)
                        return CRAccessCommands.DoorUnlockedMessage(_cardReader);

                    return CRAccessCommands.DoorUnlockedMessage(_cardReaderDB.Address);

                default:

                    if (_cardReader != null)
                        return CRAccessCommands.DoorLockedMessage(_cardReader);

                    return CRAccessCommands.DoorLockedMessage(_cardReaderDB.Address);
            }
        }

        private CRMessage GetImplicitCrMessageFromAlarmAreaState(
            IAlarmAreaStateProvider alarmAreaStateProvider,
            int optionalData,
            bool isCrReportingEnabled)
        {
            var crMessageCode = CRMessageCode.OUT_OF_ORDER;

            switch (alarmAreaStateProvider.ActivationState)
            {
                case State.Set:

                    if (alarmAreaStateProvider.AlarmState == State.Alarm
                        && isCrReportingEnabled)
                    {
                        crMessageCode = CRMessageCode.ALARM_AREA_IN_ALARM;
                    }
                    else
                    {
                        if (_cardReader != null)
                            return CRAlarmAreaCommands.AlarmAreaIsSetMessage(_cardReader);

                        return CRAlarmAreaCommands.AlarmAreaIsSetMessage(_cardReaderDB.Address);
                    }

                    break;

                case State.TemporaryUnsetEntry:
                case State.TemporaryUnsetExit:

                    crMessageCode = CRMessageCode.ALARM_AREA_TMP_UNSET;

                    break;
            }

            return
                optionalData != -1
                    ? new CRMessage(
                        _cardReaderDB.Address,
                        crMessageCode,
                        (byte) optionalData)
                    : new CRMessage(
                        _cardReader.Address,
                        crMessageCode);
        }

        ACardReaderSettings IInstanceProvider<ACardReaderSettings>.Instance
        {
            get { return this; }
        }

        public bool IsAccessAuthorizationEnabled
        {
            get
            {
                if (CurrentImplicitCrCodeParams == null)
                    return false;

                var implicitAlarmAreaInfo = ImplicitCrAlarmAreaInfo;

                if (implicitAlarmAreaInfo != null
                    && implicitAlarmAreaInfo.ActivationState == State.Set)
                {
                    return false;
                }

                var securityLevel = 
                    CurrentImplicitCrCodeParams.SecurityLevel;

                return
                    securityLevel != DB.SecurityLevel.Unlocked
                    && securityLevel != DB.SecurityLevel.Locked;
            }
        }

        public CrAlarmAreasManager.CrAlarmAreaInfo ImplicitCrAlarmAreaInfo
        {
            get
            {
                return
                    CrAlarmAreasManager != null
                        ? CrAlarmAreasManager.ImplicitCrAlarmAreaInfo
                        : null;
            }
        }

        public void ReportWrongAccessCode()
        {
            if (!InvalidCodeRetriesLimitEnabled)
                return;

            if (++_numConsecutiveWrongCodes
                >= DevicesAlarmSettings.Singleton.InvalidGinRetriesCount)
            {
                lock (_lockInvalidGinRetriesLimitReachedTimeout)
                {
                    InvalidCodeRetriesLimitReached = true;

                    _invalidGinRetriesLimitReachedTimeout = TimerManager.Static.StartTimeout(
                        DevicesAlarmSettings.Singleton.InvalidGinRetriesLimitReachedTimeout*60000,
                        OnInvalidGinRetriesLimitReachedTimeout);

                    AlarmsManager.Singleton.AddAlarm(
                        new CrInvalidCodeRetriesLimitReached(Id));
                }
            }
        }

        private bool OnInvalidGinRetriesLimitReachedTimeout(TimerCarrier timercarrier)
        {
            lock (_lockInvalidGinRetriesLimitReachedTimeout)
            {
                _invalidGinRetriesLimitReachedTimeout = null;
                ResetInvalidGinRetriesLimitReached();
            }

            return true;
        }

        private void ResetInvalidGinRetriesLimitReached()
        {
            _numConsecutiveWrongCodes = 0;
            InvalidCodeRetriesLimitReached = false;

            AlarmsManager.Singleton.StopAlarm(
                CrInvalidCodeRetriesLimitReached.CreateAlarmKey(Id));
        }

        public void ValidateCardSystem(byte[] data)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () =>
                    string.Format(
                        "void ACardReaderSettings.ValidateCardSystem(byte[] data): [{0}]",
                        Log.GetStringFromParameters(data)));

            if (_cardReader != null)
                _cardReader.MifareCommands.ValidateCardSystem(
                    _cardReader,
                    data);
        }

        public void ReportCorrectAccessCode()
        {
            _numConsecutiveWrongCodes = 0;
        }

        public void ConfigureAaCardReaders(ICollection<DB.AACardReader> aaCardReaders)
        {
            if (CrAlarmAreasManager != null)
                foreach (var aaCardReader in aaCardReaders)
                    CrAlarmAreasManager.ConfigureAaCardReader(aaCardReader);

            UpdateRootScene();
        }

        protected override void UnconfigureInternal(DB.CardReader newCardReader)
        {
            DetachEvents();

            RemoveSpecialOutputsForOfflineAndTamper(newCardReader);

            if (newCardReader != null)
                return;

            var crAlarmAreasManager = CrAlarmAreasManager;

            if (crAlarmAreasManager != null)
            {
                CrAlarmAreasManager = null;
                crAlarmAreasManager.Dispose();
            }

            if (CardReader != null)
                CardReader.EventHandler.Remove(this);

            SceneContext.Dispose();

            _doorEnvironmentAdapter = null;
        }

        public bool IsPremium
        {
            get
            {
                var hardwareVersion = _cardReader.HardwareVersion;

                return
                    hardwareVersion == CRHWVersion.ProximityPremiumCCR
                    || hardwareVersion == CRHWVersion.SmartPremiumCCR;
            }
        }

        public void EnqueueAsyncRequest(
            Action<ACardReaderSettings> requestAction)
        {
            var request =
                new CardReaderSettingsRequest(
                    requestAction);

            EnqueueAsyncRequest(request);
        }

        internal interface IAlarmAreaStateProvider
        {
            State ActivationState
            {
                get;
            }

            State AlarmState
            {
                get;
            }

            bool IsUnsettable
            {
                get;
            }

            bool IsSettable
            {
                get;
            }

            bool IsSet
            {
                get;
            }

            bool IsUnset
            {
                get;
            }
        }

        public void PrepareDoorEnvironmentAdapter(Guid idDoorEnvironment)
        {
            if (_doorEnvironmentAdapter != null)
            {
                if (_doorEnvironmentAdapter.IsAssociatedWithDoorEnvironment(idDoorEnvironment))
                    return;

                _doorEnvironmentAdapter.DetachEvents();
            }

            _doorEnvironmentAdapter =
                new DoorEnvironmentAdapter(
                    this,
                    idDoorEnvironment);

            if (ConfigurationState == ConfigurationState.ConfigurationDone)
                SetImplicitCrCode();
        }

        public void PrepareMultiDoorAdapter(Guid idMultiDoor)
        {
            if (_doorEnvironmentAdapter != null)
            {
                if (_doorEnvironmentAdapter.IsAssociatedWithMultiDoor(idMultiDoor))
                    return;

                _doorEnvironmentAdapter.DetachEvents();
            }

            _doorEnvironmentAdapter =
                new MultiDoorAdapter(
                    this,
                    idMultiDoor);

            if (ConfigurationState == ConfigurationState.ConfigurationDone)
                SetImplicitCrCode();
        }

        protected abstract CRCommunicator CrCommunicator
        {
            get;
        }

        protected override void ConfigureInternal(DB.CardReader dbObject)
        {
            if (ConfigurationState == ConfigurationState.ConfiguringExisting)
                _cardReaderDB = dbObject;
            else
            {
                CrAlarmAreasManager =
                    new CrAlarmAreasManager(
                        this,
                        Database.ConfigObjectsEngine.AaCardReadersStorage.GetAaCardReadersByIdCardReader(
                            dbObject.IdCardReader));
            }

            AttachEvents();

            _specialOutputForOffline =
                _cardReaderDB.GuidSpecialOutputForOffline;

            _specialOutputForTamper = _cardReaderDB.GuidSpecialOutputForTamper;

            CardReaders.ConfigureAlarmsBlockingAndAlarmArcs(CardReaderDb);

            _forcedSecurityLevel =
                IsCrForcedSecurityLevelObjectOnInternal(CardReaderDb)
                    ? (DB.SecurityLevel?)(DB.SecurityLevel)CardReaderDb.ForcedSecurityLevel
                    : null;

            if (!InvalidCodeRetriesLimitEnabled)
                StopTimeoutAndResetInvalidGinRetriesLimitReached();
        }

        public void StopTimeoutAndResetInvalidGinRetriesLimitReached()
        {
            lock (_lockInvalidGinRetriesLimitReachedTimeout)
            {
                if (_invalidGinRetriesLimitReachedTimeout != null)
                {
                    try
                    {
                        _invalidGinRetriesLimitReachedTimeout.StopTimer();
                    }
                    catch (Exception error)
                    {
                        HandledExceptionAdapter.Examine(error);
                    }

                    _invalidGinRetriesLimitReachedTimeout = null;
                }

                ResetInvalidGinRetriesLimitReached();
            }
        }


        public void OnOnlineStateChanged(CardReader cardReader)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "void ACardReaderSettings.ChangedOnlineState(CardReader cardReader): [{0}]",
                    Log.GetStringFromParameters(cardReader)));

            if ((cardReader == null || !cardReader.IsOnline) && _cardReader == null)
                return;

            if (ConfigurationState != ConfigurationState.ConfigurationDone)
                return;

            if (_doorEnvironmentAdapter != null)
                _doorEnvironmentAdapter.OnCrOnlineStateChanged(cardReader != null && cardReader.IsOnline);

            ApplyHwSetupInternal(cardReader);
        }

        public void RewriteCardReaderSectorReading(byte[] validCardSystem)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () =>
                    string.Format(
                        "void ACardReaderSettings.RewriteCardReaderSectorReading(byte[] validCardSystem): [{0}]",
                        Log.GetStringFromParameters(validCardSystem)));

            if (_cardReader != null && _cardReader.IsMifare)
                _cardReader.MifareCommands.ValidateCardSystem(
                    _cardReader,
                    validCardSystem);
        }

        public void SetCardSystem(byte[] cardSystemData)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () =>
                    string.Format(
                        "void ACardReaderSettings.SetCardSystem(byte[] cardSystemData): [{0}]",
                        Log.GetStringFromParameters(cardSystemData)));

            if (_cardReader != null)
                _cardReader.MifareCommands.SetCardSystem(
                    _cardReader,
                    cardSystemData);
        }

        public void Reset()
        {
            if (_cardReader != null)
                _cardReader.ControlCommands.Reset();
        }

        #region ISingleCardReaderEventHandler

        void ISingleCardReaderEventHandler.CardReaderFnBoxTimedOut()
        {
            if (SceneContext != null)
                SceneContext.CardReaderFnBoxTimedOut();
        }

        void ISingleCardReaderEventHandler.CardReaderCodeTimedOut()
        {
            if (SceneContext != null)
                SceneContext.CardReaderCodeTimedOut();
        }

        void ISingleCardReaderEventHandler.CardReaderCodeSpecified(string codeData)
        {
            if (SceneContext != null)
                SceneContext.CardReaderCodeSpecified(codeData);
        }

        void ISingleCardReaderEventHandler.CardReaderMenuCancelled(bool byOtherCommand)
        {
            if (SceneContext != null)
                SceneContext.CardReaderMenuCancelled(byOtherCommand);
        }

        void ISingleCardReaderEventHandler.CardReaderMenuTimedOut()
        {
            if (SceneContext != null)
                SceneContext.CardReaderMenuTimedOut();
        }

        void ISingleCardReaderEventHandler.CardReaderMenuItemSelected(
            int itemIndex,
            string itemText)
        {
            if (SceneContext != null)
                SceneContext.CardReaderMenuItemSelected(
                    itemIndex,
                    itemText);
        }

        void ISingleCardReaderEventHandler.CardReaderMenuItemSelected(int itemReturnCode)
        {
            if (SceneContext != null)
                SceneContext.CardReaderMenuItemSelected(itemReturnCode);
        }

        void ISingleCardReaderEventHandler.CardReaderNumericKeyPressed(byte numeric)
        {
            if (SceneContext != null)
                SceneContext.CardReaderNumericKeyPressed(numeric);
        }

        void ISingleCardReaderEventHandler.CardReaderSpecialKeyPressed(CRSpecialKey specialKey)
        {
            if (SceneContext != null)
                SceneContext.CardReaderSpecialKeyPressed(specialKey);
        }

        void ISingleCardReaderEventHandler.CardReaderCardSwiped(
            string cardData,
            int cardSystemNumber)
        {
            if (SceneContext != null)
                SceneContext.CardReaderCardSwiped(
                    cardData,
                    cardSystemNumber);
        }

        void ISingleCardReaderEventHandler.CardReaderIndirectMessageToSend(
            CRMessage message,
            bool highPriority)
        {
        }

        void ISingleCardReaderEventHandler.CardReaderSabotageStateChanged(bool tamperOn)
        {
            EnqueueAsyncRequest(new TamperChangedRequest(tamperOn));
        }

        void ISingleCardReaderEventHandler.CardReaderOnlineStateChanged(bool isOnline)
        {
        }

        void ISingleCardReaderEventHandler.CardReaderQueryDbStampResponse(byte[] queryDbStamp)
        {
            CardSystemData.Singleton.CardReaderCSDataResponse(
                Id,
                queryDbStamp,
                _cardReader);
        }

        void ISingleCardReaderEventHandler.CardReaderConfigurationResponse(
            CRConfigurationResult result,
            byte dbStamp)
        {
        }

        void ISingleCardReaderEventHandler.CardReaderConfigurationResponse(
            CRConfigurationResult result,
            uint crUniqueId,
            ushort configId)
        {
        }

        void ISingleCardReaderEventHandler.CardReaderResetOccured()
        {
        }

        void ISingleCardReaderEventHandler.CardReaderCommandFailed(CRMessageCode messageCode)
        {
        }

        void ISingleCardReaderEventHandler.CardReaderCommandTimedOut(
            CRMessageCode messageCode,
            int retryToCome)
        {
        }

        void ISingleCardReaderEventHandler.CardReaderServiceResultOccured(CRServiceResult serviceResult)
        {
        }

        void ISingleCardReaderEventHandler.CardReaderUpgradeResult(
            CRUpgradeResult result,
            Exception error)
        {
            
        }

        void ISingleCardReaderEventHandler.CardReaderUpgradeProgress(int progress)
        {
           
        }

        void ISingleCardReaderEventHandler.CardReaderCountryCodeConfirmed()
        {
            try
            {
                if (_cardReader.IsMifare)
                {
                    CardSystemData.Singleton
                        .CardReaderToOnlineState(Id);

                    _cardReader.SectorDataEncoding = CardSystemData.Singleton.Encoding;
        }

                if (DoorEnvironmentAdapter != null)
                    DoorEnvironmentAdapter.ForceLooseCardReader();

                SceneContext.EnterRootScene();
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        void ISingleCardReaderEventHandler.CardReaderGraphicalMenuUpdated()
        {
        }

        void ISingleCardReaderEventHandler.CardReaderModeChanged(CRMode readerMode)
        {
        }

        void ISingleCardReaderEventHandler.CardReaderCardWriterResponse(MifareCardWriteResult writeResult, MifareCardWriteError? writeError)
        {
        }

        #endregion

        public abstract int DcuLogicalAddress
        {
            get;
        }

        public bool WasImplicitExternalAlarmAreaHandshakeFailureToSet
        {
            get
            {
                return _implicitExternalAlarmAreaHandshakeState == ExternalAlarmAreaHandshakeState.FailureToSet;
            }
        }

        public bool WasImplicitExternalAlarmAreaHandshakeFailureToUnset
        {
            get
            {
                return _implicitExternalAlarmAreaHandshakeState == ExternalAlarmAreaHandshakeState.FailureToUnset;
            }
        }

        public void OnImplicitAlarmAreaActivationStateChanged(CrAlarmAreasManager.CrAlarmAreaInfo implicitCrAlarmAreaInfo)
        {
            bool isExternalAA;
            bool isWaiting;
            bool wasConfirmed;

            AlarmArea.AlarmAreas.Singleton.GetStateOfExternalAA(
                implicitCrAlarmAreaInfo.IdAlarmArea,
                out isExternalAA,
                out isWaiting,
                out wasConfirmed);

            if (isExternalAA && !isWaiting && wasConfirmed)
                _implicitExternalAlarmAreaHandshakeState =
                    ExternalAlarmAreaHandshakeState.Ready;

            UpdateRootScene();
        }

        protected override ACardReaderSettings This
        {
            get { return this; }
        }

        public void MaximalCodeLengthChanged()
        {
            if (CurrentImplicitCrCodeParams != null
                && CurrentImplicitCrCodeParams.ImplicitCrMessage.MessageCode == CRMessageCode.WAITING_FOR_CODE)
            {
                SetImplicitCrCode();
            }
        }
    }
}

