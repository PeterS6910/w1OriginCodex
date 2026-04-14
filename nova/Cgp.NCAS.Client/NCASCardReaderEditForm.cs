using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using Contal.Cgp.Client;
using Contal.Cgp.Client.PluginSupport;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick.Crypto;
using Contal.IwQuick.Data;
using Contal.IwQuick.Localization;
using Contal.Cgp.Globals;
using Contal.Drivers.CardReader;
using Contal.IwQuick.Parsing;
using Contal.IwQuick;
using Contal.IwQuick.PlatformPC.UI.Accordion;
using Contal.IwQuick.Threads;
using Contal.IwQuick.UI;
using CardReader = Contal.Cgp.NCAS.Server.Beans.CardReader;
using CRLanguage = Contal.Drivers.CardReader.CRLanguage;
using TimeZone = Contal.Cgp.Server.Beans.TimeZone;

namespace Contal.Cgp.NCAS.Client
{
    public sealed partial class NCASCardReaderEditForm :
#if DESIGNER
    Form
#else
 ACgpPluginEditFormWithAlarmInstructions<CardReader>
#endif
    {
        private const int ALARMAREASCOMBOBOXWIDTH = 110;
        private const int ALARMAREASHEIGHT = 17;
        private const int ALARMAREASLEFTMARGIN = 5;
        private const int ALARMAREASRIGHTMARGIN = 15;
        private const int ALARMAREASSTARTY = 5;
        private const int ALARMAREASLINESPACE = 0;
        private const string DEFAULT_PASSWORD = "********";

        private SecurityDailyPlan _actSecurityDailyPlan;
        private SecurityTimeZone _actSecurityTimeZone;
        private AOnOffObject _actOnOffObject;
        private AlarmArea _implicitAlarmArea;
        private CRHWVersion _crHWVersion;

        private Action<Guid, byte, Guid> _eventCRStateChanged;
        private Action<Guid, byte> _eventCRCommandChanged;

        private Output _outputOffline;
        private Output _outputSabotage;

        private bool _isEnabledSDP = true;

        private FunctionKey _functionKey1 = new FunctionKey();
        private FunctionKey _functionKey2 = new FunctionKey();

        private bool _hasAccessAdminLanguageSettings;
        private bool _hasAccessAdminFunctionKeysSettings;

        private readonly ControlAlarmTypeSettings _catsCrOffline;
        private readonly ControlModifyAlarmArcs _cmaaCrOffline;
        private readonly ControlAlarmTypeSettings _catsCrTamperSabotage;
        private readonly ControlModifyAlarmArcs _cmaaCrTamperSabotage;
        private readonly ControlAlarmTypeSettings _catsCrAccessDenied;
        private readonly ControlModifyAlarmArcs _cmaaCrAccessDenied;
        private readonly ControlAlarmTypeSettings _catsCrUnknownCard;
        private readonly ControlModifyAlarmArcs _cmaaCrUnknownCard;
        private readonly ControlAlarmTypeSettings _catsCrCardBlockedOrInactive;
        private readonly ControlModifyAlarmArcs _cmaaCrCardBlockedOrInactive;
        private readonly ControlAlarmTypeSettings _catsCrInvalidPin;
        private readonly ControlModifyAlarmArcs _cmaaCrInvalidPin;
        private readonly ControlAlarmTypeSettings _catsCrInvalidGin;
        private readonly ControlModifyAlarmArcs _cmaaCrInvalidGin;
        private readonly ControlAlarmTypeSettings _catsCrInvalidEmergencyCode;
        private readonly ControlModifyAlarmArcs _cmaaCrInvalidEmergencyCode;
        private readonly ControlAlarmTypeSettings _catsCrAccessPermitted;
        private readonly ControlModifyAlarmArcs _cmaaCrAccessPermitted;
        private readonly ControlAlarmTypeSettings _catsCrInvalidGinRetriesLimitReached;
        private readonly ControlModifyAlarmArcs _cmaaCrInvalidGinRetriesLimitReached;

        private readonly Dictionary<AlarmType, Action> _openAndScrollToControlByAlarmType;

        private bool _settingDefaultPassword;

        public NCASCardReaderEditForm(
                CardReader cardReader,
                ShowOptionsEditForm showOption,
                PluginMainForm<NCASClient> myTableForm)
            : base(
                cardReader,
                showOption,
                CgpClientMainForm.Singleton,
                myTableForm,
                NCASClient.LocalizationHelper)
        {
            _openAndScrollToControlByAlarmType = new Dictionary<AlarmType, Action>
            {
                {
                    AlarmType.CardReader_Offline,
                    () =>
                    {
                        _tcCardReader.SelectedTab = _tpAlarmSettings;
                        _accordionAlarmSettings.OpenAndScrollToControl(_catsCrOffline);
                    }
                },
                {
                    AlarmType.CardReader_TamperSabotage,
                    () =>
                    {
                        _tcCardReader.SelectedTab = _tpAlarmSettings;
                        _accordionAlarmSettings.OpenAndScrollToControl(_catsCrTamperSabotage);
                    }
                },
                {
                    AlarmType.CardReader_AccessDenied,
                    () =>
                    {
                        _tcCardReader.SelectedTab = _tpAlarmSettings;
                        _accordionAlarmSettings.OpenAndScrollToControl(_catsCrAccessDenied);
                    }
                },
                {
                    AlarmType.CardReader_UnknownCard,
                    () =>
                    {
                        _tcCardReader.SelectedTab = _tpAlarmSettings;
                        _accordionAlarmSettings.OpenAndScrollToControl(_catsCrUnknownCard);
                    }
                },
                {
                    AlarmType.CardReader_CardBlockedOrInactive,
                    () =>
                    {
                        _tcCardReader.SelectedTab = _tpAlarmSettings;
                        _accordionAlarmSettings.OpenAndScrollToControl(_catsCrCardBlockedOrInactive);
                    }
                },
                {
                    AlarmType.CardReader_InvalidPIN,
                    () =>
                    {
                        _tcCardReader.SelectedTab = _tpAlarmSettings;
                        _accordionAlarmSettings.OpenAndScrollToControl(_catsCrInvalidPin);
                    }
                },
                {
                    AlarmType.CardReader_InvalidCode,
                    () =>
                    {
                        _tcCardReader.SelectedTab = _tpAlarmSettings;
                        _accordionAlarmSettings.OpenAndScrollToControl(_catsCrInvalidGin);
                    }
                },
                {
                    AlarmType.CardReader_InvalidEmergencyCode,
                    () =>
                    {
                        _tcCardReader.SelectedTab = _tpAlarmSettings;
                        _accordionAlarmSettings.OpenAndScrollToControl(_catsCrInvalidEmergencyCode);
                    }
                },
                {
                    AlarmType.CardReader_AccessPermitted,
                    () =>
                    {
                        _tcCardReader.SelectedTab = _tpAlarmSettings;
                        _accordionAlarmSettings.OpenAndScrollToControl(_catsCrAccessPermitted);
                    }
                },
                {
                    AlarmType.CardReader_Invalid_Code_Retries_Limit_Reached,
                    () =>
                    {
                        _tcCardReader.SelectedTab = _tpAlarmSettings;
                        _accordionAlarmSettings.OpenAndScrollToControl(_catsCrInvalidGinRetriesLimitReached);
                    }
                }
            };

            InitializeComponent();

            _catsCrOffline = new ControlAlarmTypeSettings
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_catsCrOffline",
                TabIndex = 0,
                Plugin = Plugin,
                ThreeStateCheckBoxes = true,
                AlarmEnabledCheckState = null,
                BlockAlarmCheckState = null,
                EventlogDuringBlockedAlarmVisible = false,
                PresentationGroupVisible = false
            };

            _catsCrOffline.EditTextChanger += () => EditTextChanger(null, null);
            _catsCrOffline.EditTextChangerOnlyInDatabase += () => EditTextChangerOnlyInDatabase(null, null);

            _cmaaCrOffline = new ControlModifyAlarmArcs
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_cmaaCrOffline",
                TabIndex = 1,
                Plugin = Plugin
            };

            _cmaaCrOffline.EditTextChanger += () => EditTextChanger(null, null);

            _catsCrOffline.AddControl(_cmaaCrOffline);

            _accordionAlarmSettings.Add(_catsCrOffline, "Card reader offline").Name = "_chbCrOffline";

            _catsCrTamperSabotage = new ControlAlarmTypeSettings
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_catsCrTamperSabotage",
                TabIndex = 2,
                Plugin = Plugin,
                ThreeStateCheckBoxes = true,
                AlarmEnabledCheckState = null,
                BlockAlarmCheckState = null,
                EventlogDuringBlockedAlarmCheckState = null,
                PresentationGroupVisible = false
            };

            _catsCrTamperSabotage.EditTextChanger += () => EditTextChanger(null, null);
            _catsCrTamperSabotage.EditTextChangerOnlyInDatabase += () => EditTextChangerOnlyInDatabase(null, null);

            _cmaaCrTamperSabotage = new ControlModifyAlarmArcs
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_cmaaCrTamperSabotage",
                TabIndex = 3,
                Plugin = Plugin
            };

            _cmaaCrTamperSabotage.EditTextChanger += () => EditTextChanger(null, null);

            _catsCrTamperSabotage.AddControl(_cmaaCrTamperSabotage);

            _accordionAlarmSettings.Add(_catsCrTamperSabotage, "Card reader tamper/sabotage").Name = "_chbCrTamperSabotage";

            _catsCrAccessDenied = new ControlAlarmTypeSettings
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_catsCrAccessDenied",
                TabIndex = 4,
                Plugin = Plugin,
                ThreeStateCheckBoxes = true,
                AlarmEnabledCheckState = null,
                BlockAlarmCheckState = null,
                EventlogDuringBlockedAlarmCheckState = null,
                PresentationGroupVisible = false
            };

            _catsCrAccessDenied.EditTextChanger += () => EditTextChanger(null, null);
            _catsCrAccessDenied.EditTextChangerOnlyInDatabase += () => EditTextChangerOnlyInDatabase(null, null);

            _cmaaCrAccessDenied = new ControlModifyAlarmArcs
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_cmaaCrAccessDenied",
                TabIndex = 5,
                Plugin = Plugin
            };

            _cmaaCrAccessDenied.EditTextChanger += () => EditTextChanger(null, null);

            _catsCrAccessDenied.AddControl(_cmaaCrAccessDenied);

            _accordionAlarmSettings.Add(_catsCrAccessDenied, "Access denied").Name = "_chbCrAccessDenied";

            _catsCrUnknownCard = new ControlAlarmTypeSettings
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_catsCrUnknownCard",
                TabIndex = 6,
                Plugin = Plugin,
                ThreeStateCheckBoxes = true,
                AlarmEnabledCheckState = null,
                BlockAlarmCheckState = null,
                EventlogDuringBlockedAlarmCheckState = null,
                PresentationGroupVisible = false
            };

            _catsCrUnknownCard.EditTextChanger += () => EditTextChanger(null, null);
            _catsCrUnknownCard.EditTextChangerOnlyInDatabase += () => EditTextChangerOnlyInDatabase(null, null);

            _cmaaCrUnknownCard = new ControlModifyAlarmArcs
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_cmaaCrUnknownCard",
                TabIndex = 7,
                Plugin = Plugin
            };

            _cmaaCrUnknownCard.EditTextChanger += () => EditTextChanger(null, null);

            _catsCrUnknownCard.AddControl(_cmaaCrUnknownCard);

            _accordionAlarmSettings.Add(_catsCrUnknownCard, "Unknown card").Name = "_chbCrUnknownCard";

            _catsCrCardBlockedOrInactive = new ControlAlarmTypeSettings
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_catsCrCardBlockedOrInactive",
                TabIndex = 8,
                Plugin = Plugin,
                ThreeStateCheckBoxes = true,
                AlarmEnabledCheckState = null,
                BlockAlarmCheckState = null,
                EventlogDuringBlockedAlarmCheckState = null,
                PresentationGroupVisible = false
            };

            _catsCrCardBlockedOrInactive.EditTextChanger += () => EditTextChanger(null, null);
            _catsCrCardBlockedOrInactive.EditTextChangerOnlyInDatabase += () => EditTextChangerOnlyInDatabase(null, null);

            _cmaaCrCardBlockedOrInactive = new ControlModifyAlarmArcs
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_cmaaCrCardBlockedOrInactive",
                TabIndex = 9,
                Plugin = Plugin
            };

            _cmaaCrCardBlockedOrInactive.EditTextChanger += () => EditTextChanger(null, null);

            _catsCrCardBlockedOrInactive.AddControl(_cmaaCrCardBlockedOrInactive);

            _accordionAlarmSettings.Add(_catsCrCardBlockedOrInactive, "Card blocked or inactive").Name = "_chbCrCardBlockedOrInactive";

            _catsCrInvalidPin = new ControlAlarmTypeSettings
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_catsCrInvalidPin",
                TabIndex = 10,
                Plugin = Plugin,
                ThreeStateCheckBoxes = true,
                AlarmEnabledCheckState = null,
                BlockAlarmCheckState = null,
                EventlogDuringBlockedAlarmCheckState = null,
                PresentationGroupVisible = false
            };

            _catsCrInvalidPin.EditTextChanger += () => EditTextChanger(null, null);
            _catsCrInvalidPin.EditTextChangerOnlyInDatabase += () => EditTextChangerOnlyInDatabase(null, null);

            _cmaaCrInvalidPin = new ControlModifyAlarmArcs
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_cmaaCrInvalidPin",
                TabIndex = 11,
                Plugin = Plugin
            };

            _cmaaCrInvalidPin.EditTextChanger += () => EditTextChanger(null, null);

            _catsCrInvalidPin.AddControl(_cmaaCrInvalidPin);

            _accordionAlarmSettings.Add(_catsCrInvalidPin, "Invalid PIN").Name = "_chbCrInvalidPin";

            _catsCrInvalidGin = new ControlAlarmTypeSettings
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_catsCrInvalidGin",
                TabIndex = 12,
                Plugin = Plugin,
                ThreeStateCheckBoxes = true,
                AlarmEnabledCheckState = null,
                BlockAlarmCheckState = null,
                EventlogDuringBlockedAlarmCheckState = null,
                PresentationGroupVisible = false
            };

            _catsCrInvalidGin.EditTextChanger += () => EditTextChanger(null, null);
            _catsCrInvalidGin.EditTextChangerOnlyInDatabase += () => EditTextChangerOnlyInDatabase(null, null);

            _cmaaCrInvalidGin = new ControlModifyAlarmArcs
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_cmaaCrInvalidGin",
                TabIndex = 13,
                Plugin = Plugin
            };

            _cmaaCrInvalidGin.EditTextChanger += () => EditTextChanger(null, null);

            _catsCrInvalidGin.AddControl(_cmaaCrInvalidGin);

            _accordionAlarmSettings.Add(_catsCrInvalidGin, "Invalid GIN").Name = "_chbCrInvalidGin";

            _catsCrInvalidEmergencyCode = new ControlAlarmTypeSettings
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_catsCrInvalidEmergencyCode",
                TabIndex = 14,
                Plugin = Plugin,
                ThreeStateCheckBoxes = true,
                AlarmEnabledCheckState = null,
                BlockAlarmCheckState = null,
                EventlogDuringBlockedAlarmCheckState = null,
                PresentationGroupVisible = false
            };

            _catsCrInvalidEmergencyCode.EditTextChanger += () => EditTextChanger(null, null);
            _catsCrInvalidEmergencyCode.EditTextChangerOnlyInDatabase += () => EditTextChangerOnlyInDatabase(null, null);

            _cmaaCrInvalidEmergencyCode = new ControlModifyAlarmArcs
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_cmaaCrInvalidEmergencyCode",
                TabIndex = 15,
                Plugin = Plugin
            };

            _cmaaCrInvalidEmergencyCode.EditTextChanger += () => EditTextChanger(null, null);

            _catsCrInvalidEmergencyCode.AddControl(_cmaaCrInvalidEmergencyCode);

            _accordionAlarmSettings.Add(_catsCrInvalidEmergencyCode, "Invalid emergency code").Name = "_chbCrInvalidEmergencyCode";

            _catsCrAccessPermitted = new ControlAlarmTypeSettings
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_catsCrAccessPermitted",
                TabIndex = 16,
                Plugin = Plugin,
                ThreeStateCheckBoxes = true,
                AlarmEnabledCheckState = null,
                BlockAlarmCheckState = null,
                EventlogDuringBlockedAlarmVisible = false,
                PresentationGroupVisible = false
            };

            _catsCrAccessPermitted.EditTextChanger += () => EditTextChanger(null, null);
            _catsCrAccessPermitted.EditTextChangerOnlyInDatabase += () => EditTextChangerOnlyInDatabase(null, null);

            _cmaaCrAccessPermitted = new ControlModifyAlarmArcs
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_cmaaCrAccessPermitted",
                TabIndex = 17,
                Plugin = Plugin
            };

            _cmaaCrAccessPermitted.EditTextChanger += () => EditTextChanger(null, null);

            _catsCrAccessPermitted.AddControl(_cmaaCrAccessPermitted);

            _accordionAlarmSettings.Add(_catsCrAccessPermitted, "Access permitted").Name = "_chbCrAccessPermitted";

            _catsCrInvalidGinRetriesLimitReached = new ControlAlarmTypeSettings
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_catsCrInvalidGinRetriesLimitReached",
                TabIndex = 18,
                Plugin = Plugin,
                ThreeStateCheckBoxes = true,
                AlarmEnabledCheckState = null,
                BlockAlarmCheckState = null,
                EventlogDuringBlockedAlarmVisible = false,
                PresentationGroupVisible = false
            };

            _catsCrInvalidGinRetriesLimitReached.EditTextChanger += () => EditTextChanger(null, null);
            _catsCrInvalidGinRetriesLimitReached.EditTextChangerOnlyInDatabase += () => EditTextChangerOnlyInDatabase(null, null);

            _cmaaCrInvalidGinRetriesLimitReached = new ControlModifyAlarmArcs
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_cmaaCrInvalidGinRetriesLimitReached",
                TabIndex = 19,
                Plugin = Plugin
            };

            _cmaaCrInvalidGinRetriesLimitReached.EditTextChanger += () => EditTextChanger(null, null);

            _catsCrInvalidGinRetriesLimitReached.AddControl(_cmaaCrInvalidGinRetriesLimitReached);

            _accordionAlarmSettings.Add(_catsCrInvalidGinRetriesLimitReached, "Invalid GIN retries limit reached").Name = "_chbCrInvalidGinRetriesLimitReached";

            _accordionAlarmSettings.OpenedAllChnged +=
                isOpenAll => SetOpenAllCheckState(
                    _cbOpenAllAlarmSettings,
                    isOpenAll);

            SetOpenAllCheckState(_cbOpenAllAlarmSettings, _accordionAlarmSettings.IsOpenAll);

            DoubleBuffered = true;
            _lFirmwareInformation.Text = string.Empty;
            _lProtocolVersionInformation.Text = string.Empty;
            _eState.Text = GetString("Unknown");
            _eState.BackColor = Color.Yellow;
            _lTypeCRInformation.Text = string.Empty;
            _eLastCard.Text = string.Empty;
            _bCreate.Enabled = false;
            _lKeyboardInformation.Text = string.Empty;
            _lDisplayInformation.Text = string.Empty;
            WheelTabContorol = _tcCardReader;
            ShowHidePresentationActionSettings();

            MinimumSize = new Size(Width, Height);

            _cbForcedSL.MouseWheel += ControlMouseWheel;
            _cbSecurityLevel.MouseWheel += ControlMouseWheel;
            _cbSLForEnterToMenu.MouseWheel += ControlMouseWheel;
            _eDescription.MouseWheel += ControlMouseWheel;
            _lbUserFolders.MouseWheel += ControlMouseWheel;
            SetReferenceEditColors();

            HideDisableTabPageLanguageSettings(
                CgpClient.Singleton.MainServerProvider.HasAccess(
                    NCASAccess.GetAccess(AccessNCAS.CardReadersLanguageSettingsView)),
                CgpClient.Singleton.MainServerProvider.HasAccess(
                    NCASAccess.GetAccess(AccessNCAS.CardReadersLanguageSettingsAdmin)));

            HideDisableTabPageFunctionKeysSettings(
                CgpClient.Singleton.MainServerProvider.HasAccess(
                    NCASAccess.GetAccess(AccessNCAS.CardReadersFunctionKeysSettingsView)),
                CgpClient.Singleton.MainServerProvider.HasAccess(
                    NCASAccess.GetAccess(AccessNCAS.CardReadersFunctionKeysSettingsAdmin)));

            SafeThread.StartThread(HideDisableTabPages);

            ObjectsPlacementHandler.AddImageList(_lbUserFolders);
        }

        private static void SetOpenAllCheckState(CheckBox checkBox, bool? isOpenAll)
        {
            checkBox.CheckState = isOpenAll.HasValue
                ? (isOpenAll.Value
                    ? CheckState.Checked
                    : CheckState.Unchecked)
                : CheckState.Indeterminate;
        }

        protected override void RegisterEvents()
        {
            _eventCRStateChanged = ChangeState;
            StateChangedCardReaderHandler.Singleton.RegisterStateChanged(_eventCRStateChanged);
            _eventCRCommandChanged = ChangeCRCommand;
            CommandChangedCardReaderHandler.Singleton.RegisterCommandChanged(_eventCRCommandChanged);
            _eventBlockedStateChangedCardReader = BlockedStateChangedCardReader;
            BlockedStateChangedCardReaderHandler.Singleton.RegisterBlockedStateChanged(_eventBlockedStateChangedCardReader);
        }

        protected override void UnregisterEvents()
        {
            StateChangedCardReaderHandler.Singleton.UnregisterStateChanged(_eventCRStateChanged);
            CommandChangedCardReaderHandler.Singleton.UnregisterCommandChanged(_eventCRCommandChanged);
            BlockedStateChangedCardReaderHandler.Singleton.UnregisterBlockedStateChanged(_eventBlockedStateChangedCardReader);
        }

        private void HideDisableTabPages()
        {
            try
            {
                HideDisableTabPageSettings(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.CardReadersSettingsView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.CardReadersSettingsAdmin)),
                    NCASSecurityDailyPlansForm.Singleton.HasAccessView());

                HideDisableTabPageInformation(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.CardReadersInformationView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.CardReadersInformationAdmin)),
                    CardsForm.Singleton.HasAccessInsert(),
                    PersonsForm.Singleton.HasAccessView());

                HideDisableTabPageAlarmAreas(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.CardReadersAlarmAreasView)),
                    NCASAlarmAreasForm.Singleton.HasAccessUpdate());

                HideDisableTabPageAlarmSettings(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.CardReadersAlarmSettingsView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.CardReadersAlarmSettingsAdmin)));

                HideDisableTabPageActionPresentationSettings(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.CardReadersActionPresentationSettingsView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.CardReadersActionPresentationSettingsAdmin)));

                HideDisableTabPageSpecialOutputs(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.CardReadersSpecialOutputsView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.CardReadersSpecialOutputsAdmin)));

                HideDisableTabFoldersSructure(ObjectsPlacementHandler.HasAccessView());

                HideDisableTabReferencedBy(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.CardReadersReferencedByView)));

                HideDisableTabPageDescription(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.CardReadersDescriptionView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.CardReadersDescriptionAdmin)));
            }
            catch
            {
            }
        }

        private void HideDisableTabPageSettings(bool view, bool admin, bool viewSecurityDailyPlan)
        {
            this.InvokeInUI(() =>
            {
                if (!admin)
                {
                    _eName.ReadOnly = true;
                }

                if (!view && !admin)
                {
                    _tcCardReader.TabPages.Remove(_tpSettings);
                    return;
                }

                _tpSettings.Enabled = admin;

                if (!viewSecurityDailyPlan)
                {
                    _isEnabledSDP = false;
                    _tbmDailyPlan.Enabled = false;
                    _tbmSecurityDailyPlanTimeZoneForEnterToMenu.Enabled = false;
                }
                else
                {
                    _isEnabledSDP = true;
                }
            });
        }

        private void HideDisableTabPageInformation(bool view, bool admin, bool accessInsertCard, bool accessViewPerson)
        {
            this.InvokeInUI(()=>
            {
                if (!view && !admin)
                {
                    _tcCardReader.TabPages.Remove(_tpInformation);
                    return;
                }

                _tpInformation.Enabled = admin;

                if (!accessInsertCard)
                    _bCreate.Visible = false;

                if (!accessViewPerson)
                    _itbPerson.Enabled = false;
            });
        }

        private void HideDisableTabPageAlarmAreas(bool view, bool admin)
        {
            this.InvokeInUI(() =>
            {
                if (!view && !admin)
                {
                    _tcCardReader.TabPages.Remove(_tpAlarmAreas);
                    return;
                }

                _tpAlarmAreas.Enabled = admin;
            });
        }

        private void HideDisableTabPageAlarmSettings(bool view, bool admin)
        {
            this.InvokeInUI(()=>
            {
                if (!view && !admin)
                {
                    _tcCardReader.TabPages.Remove(_tpAlarmSettings);
                    return;
                }

                _tpAlarmSettings.Enabled = admin;
            }
            );
        }

        private void HideDisableTabPageLanguageSettings(bool view, bool admin)
        {
            this.InvokeInUI(()=>
            {
                _hasAccessAdminLanguageSettings = admin;

                if (!view && !admin)
                {
                    _tcCardReader.TabPages.Remove(_tpLanguagesettings);
                    return;
                }

                _tpLanguagesettings.Enabled = admin;
            });
        }

        private void HideDisableTabPageActionPresentationSettings(bool view, bool admin)
        {
            this.InvokeInUI(() =>
            {
                if (!view && !admin)
                {
                    _tcCardReader.TabPages.Remove(_tpEventSettings);
                    return;
                }

                _tpEventSettings.Enabled = admin;
            });
        }

        private void HideDisableTabPageSpecialOutputs(bool view, bool admin)
        {
            this.InvokeInUI(() =>
            {
                if (!view && !admin)
                {
                    _tcCardReader.TabPages.Remove(_tpSpecialOutputs);
                    return;
                }

                _tpSpecialOutputs.Enabled = admin;
            });
        }

        private void HideDisableTabFoldersSructure(bool view)
        {
            this.InvokeInUI(() =>
            {
                if (!view)
                {
                    _tcCardReader.TabPages.Remove(_tpUserFolders);
                }
            });
        }

        private void HideDisableTabReferencedBy(bool view)
        {
            this.InvokeInUI(() =>
            {
                if (!view)
                {
                    _tcCardReader.TabPages.Remove(_tpReferencedBy);
                }
            });
        }

        private void HideDisableTabPageDescription(bool view, bool admin)
        {
            this.InvokeInUI(() =>
            {
                if (!view && !admin)
                {
                    _tcCardReader.TabPages.Remove(_tpDescription);
                    return;
                }

                _tpDescription.Enabled = admin;
            });
        }

        private void HideDisableTabPageFunctionKeysSettings(bool view, bool admin)
        {
            this.InvokeInUI(() =>
            {
                _hasAccessAdminFunctionKeysSettings = admin;

                if (!view && !admin)
                {
                    _tcCardReader.TabPages.Remove(_tpFunctionKeysSettings);
                    return;
                }

                _tpFunctionKeysSettings.Enabled = admin;
            });
        }

        private void ShowHidePresentationActionSettings()
        {
            var hardwareVersion = Plugin.MainServerProvider.CardReaders.GetHardwareVersion(_editingObject);
            try
            {
                if (Validator.IsNotNullString(hardwareVersion))
                    _crHWVersion = (CRHWVersion)Enum.Parse(typeof(CRHWVersion), hardwareVersion);
                else
                    _crHWVersion = CRHWVersion.Unknown;

                _chbTamperExternalBuzzer.Visible = _crHWVersion == CRHWVersion.SmartPremiumCCR;

                // FIX FOR NOW, WHERE CCR'S do not provide this function
                switch (_crHWVersion)
                {
                    /*case CRHWVersion.SmartPremiumCCR:
                    case CRHWVersion.SmartSlimCCR:
                    case CRHWVersion.ProximityPremiumCCR:
                    case CRHWVersion.ProximitySlimCCR:
                    case CRHWVersion.SmartMiniCCR:
                    case CRHWVersion.ProximityMiniCCR:*/
                    case CRHWVersion.Unknown:
                        HideTabPresentationActionSettings();
                        break;
                    default:
                        ShowTabPresentationActionSettings();
                        break;
                }
            }
            catch
            {
                ShowTabPresentationActionSettings();
            }
        }

        private void ShowTabPresentationActionSettings()
        {
            this.BeginInvokeInUI(()=>
            {
                if (!_tcCardReader.TabPages.Contains(_tpEventSettings))
                {
                    _tcCardReader.TabPages.Add(_tpEventSettings);
                }
            });
        }

        private void HideTabPresentationActionSettings()
        {
            this.BeginInvokeInUI(() =>
            {
                _tcCardReader.TabPages.Remove(_tpEventSettings);
            });
        }

        private const string CARDREADERSMARTPREMIUMCCR = "SmartPremiumCCR";
        private const string CARDREADERPROXIMITYPREMIUMCCR = "ProximityPremiumCCR";

        private void SetInformation()
        {
            var firmware = Plugin.MainServerProvider.CardReaders.GetFirmwareVersion(_editingObject);
            ShowFirmwareInformation(firmware);
            var protocolVersion = Plugin.MainServerProvider.CardReaders.GetProtocolVersion(_editingObject);
            ShowProtocolVersionInformation(protocolVersion);
            var hardwareVersion = Plugin.MainServerProvider.CardReaders.GetHardwareVersion(_editingObject);
            if (hardwareVersion != string.Empty)
            {
                var cardReaderHardware = LocalizationHelper.GetString(hardwareVersion);
                ShowHardwareInformation(cardReaderHardware);
            }

            if (_tcCardReader.TabPages.Contains(_tpFunctionKeysSettings))
            {
                _tpFunctionKeysSettings.Enabled = (hardwareVersion == CARDREADERSMARTPREMIUMCCR
                    || hardwareVersion == CARDREADERPROXIMITYPREMIUMCCR) && _hasAccessAdminFunctionKeysSettings;
            }

            var strLastCard = Plugin.MainServerProvider.CardReaders.GetLastCard(_editingObject.IdCardReader);
            RunShowLastCardInformation(strLastCard);
            var hasKeyboard = Plugin.MainServerProvider.CardReaders.GetHasKeyboard(_editingObject.IdCardReader);
            ShowKeyboardInformation(hasKeyboard);
            var hasDisplay = Plugin.MainServerProvider.CardReaders.GetHasDisplay(_editingObject.IdCardReader);
            ShowDisplayInformation(hasDisplay);
            if (hasDisplay == false)
            {
                _chbEmergencyCode.Invoke(new MethodInvoker(delegate
                {
                    _chbEmergencyCode.Visible = false;
                    _eEmergencyCode.Visible = false;
                }));
            }
            else
            {
                _chbEmergencyCode.Invoke(new MethodInvoker(delegate
                {
                    _chbEmergencyCode.Visible = true;
                    _eEmergencyCode.Visible = true;
                }));
            }
        }

        private void ShowFirmwareInformation(string firmware)
        {
            this.BeginInvokeInUI(() =>
            {
                _lFirmwareInformation.Text = firmware;
            });
        }

        private void ShowProtocolVersionInformation(string version)
        {
            this.BeginInvokeInUI(() =>
            {
                _lProtocolVersionInformation.Text = version;
            });
        }

        private void ShowHardwareInformation(string hardwareVersion)
        {
            this.BeginInvokeInUI(() =>
            {
                _lTypeCRInformation.Text = hardwareVersion;
            });
        }

        private void ShowLastCardInformation(string fullCardNumber)
        {
            SafeThread<string>.StartThread(RunShowLastCardInformation, fullCardNumber);
        }

        private void RunShowLastCardInformation(string fullCardNumber)
        {
            Card lastCard = null;
            if (!string.IsNullOrEmpty(fullCardNumber))
            {
                ConnectionLost();
                lastCard = CgpClient.Singleton.MainServerProvider.Cards.GetCardByFullNumber(fullCardNumber);
            }

            ShowLastCardInformation(lastCard, fullCardNumber);
        }

        Card _lastCard;
        Person _lastCardPerson;
        private void ShowLastCardInformation(Card lastCard, string fullCardNumber)
        {
            this.BeginInvokeInUI(() =>
            {
                _lastCard = lastCard;
                _eLastCard.Text = fullCardNumber;

                if (_lastCard != null)
                {
                    _bCreate.Text = GetString("General_bEdit");
                    _lastCardPerson = _lastCard.Person;
                }
                else
                {
                    _bCreate.Text = GetString("General_bCreate");
                    _lastCardPerson = null;

                }

                if (_lastCardPerson != null)
                {
                    _lPerson.Visible = true;
                    _itbPerson.Visible = true;
                    _itbPerson.Text = _lastCardPerson.ToString();
                    _itbPerson.Image = Plugin.GetImageForAOrmObject(_lastCardPerson);
                }
                else
                {
                    _lPerson.Visible = false;
                    _itbPerson.Visible = false;
                }

                _bCreate.Enabled = _eLastCard.Text != string.Empty;
            });
        }

        private void ShowKeyboardInformation(bool? hasKeyboard)
        {
            this.BeginInvokeInUI(() =>
            {
                if (hasKeyboard.HasValue)
                {
                    if (hasKeyboard.Value)
                    {
                        _lKeyboardInformation.Text = GetString("HasKeyboard");
                        _lKeyboardInformation.ForeColor = Color.Green;
                    }
                    else
                    {
                        _lKeyboardInformation.Text = GetString("HasNotKeyboard");
                        _lKeyboardInformation.ForeColor = Color.Red;
                    }
                }
                else
                {
                    _lKeyboardInformation.Text = string.Empty;
                }
            });
        }

        private void ShowDisplayInformation(bool? hasDisplay)
        {
            this.BeginInvokeInUI(() =>
            {
                if (hasDisplay != null)
                {
                    if (hasDisplay.Value)
                    {
                        _lDisplayInformation.Text = GetString("HasDisplay");
                        _lDisplayInformation.ForeColor = Color.Green;

                        if (_tcCardReader.TabPages.Contains(_tpLanguagesettings))
                        {
                            _tpLanguagesettings.Enabled = _hasAccessAdminLanguageSettings;
                        }
                    }
                    else
                    {
                        _lDisplayInformation.Text = GetString("HasNotDisplay");
                        _lDisplayInformation.ForeColor = Color.Red;

                        if (_tcCardReader.TabPages.Contains(_tpLanguagesettings))
                        {
                            _tpLanguagesettings.Enabled = false;
                        }
                    }
                }
                else
                {
                    _lDisplayInformation.Text = string.Empty;
                }
            });
        }

        protected override void AfterTranslateForm()
        {
            if (Insert)
            {
                Text = GetString("CardReaderEditFormInsertText");
            }

            _bCreate.Text = GetString(_lastCard != null
                ? "General_bEdit"
                : "General_bCreate");

            CgpClientMainForm.Singleton.SetTextOpenWindow(this);
        }

        protected override void BeforeInsert()
        {
            NCASCardReadersForm.Singleton.BeforeInsert(this);
        }

        protected override void BeforeEdit()
        {
            NCASCardReadersForm.Singleton.BeforeEdit(this, _editingObject);
        }

        protected override void AfterInsert()
        {
            NCASCardReadersForm.Singleton.AfterInsert();
        }

        protected override void AfterEdit()
        {
            NCASCardReadersForm.Singleton.AfterEdit(_editingObject);
        }

        public override void ReloadEditingObject(out bool allowEdit)
        {
            Exception error;
            var obj = Plugin.MainServerProvider.CardReaders.GetObjectForEdit(_editingObject.IdCardReader, out error);
            if (error != null)
            {
                if (error is AccessDeniedException)
                {
                    allowEdit = false;
                    Plugin.MainServerProvider.CardReaders.GetObjectById(_editingObject.IdCardReader);
                }

                throw error;
            }

            allowEdit = true;

            _editingObject = obj;
        }

        public override void ReloadEditingObjectWithEditedData()
        {
            Exception error;
            Plugin.MainServerProvider.CardReaders.RenewObjectForEdit(_editingObject.IdCardReader, out error);
            if (error != null)
            {
                throw error;
            }
        }

        protected override void SetValuesInsert()
        {
            AlarmAreasTextsSetPosition();
            FillSecurityLevelComboBox();
            FillSymbolComboBox();

            _eCrLogicalAddress.Visible = false;
            _nudLogicalAddress.Left = _eCrLogicalAddress.Left;
            _nudLogicalAddress.Visible = true;

            if (_editingObject.Address > 0)
            {
                if (_editingObject.Address >= 16)
                {
                    _nudLogicalAddress.Minimum = 0;
                    _nudLogicalAddress.Maximum = 15;
                    _editingObject.Address = 0;
                    _nudLogicalAddress.Value = 0;
                }
                else
                {
                    _nudLogicalAddress.Minimum = 0;
                    _nudLogicalAddress.Maximum = _editingObject.Address;
                    _nudLogicalAddress.Value = 1;
                    _editingObject.Address = 1;
                }
            }

            EditTextChanger(_nudLogicalAddress, null);
            LoadSecurityLevelState();
            AddSlForEnterToMenu();

            _rbCRLanguageEnglish.Checked = true;
            _chbCardAppliedLED.Checked = true;
            _chbCardAppliedKeyboardLight.Checked = true;
            _chbCardAppliedInternalBuzzer.Checked = true;
            _chbCardAppliedExternalBuzzer.Checked = true;
            _chbTamperLED.Checked = true;
            _chbTamperKeyboardLight.Checked = true;
            _chbTamperInternalBuzzer.Checked = true;
            _chbTamperExternalBuzzer.Checked = true;
            _chbResetLED.Checked = true;
            _chbResetKeyboardLight.Checked = true;
            _chbResetInternalBuzzer.Checked = true;
            _chbResetExternalBuzzer.Checked = true;
            _chbKeyPressedLED.Checked = true;
            _chbKeyPressedKeyboardLight.Checked = true;
            _chbKeyPressedInternalBuzzer.Checked = true;
            _chbKeyPressedExternalBuzzer.Checked = true;
            _chbInternalBuzzerKillswitch.Checked = false;

            _eCrLogicalAddress.Text = string.Empty;
            _itbCrParentObject.Text = string.Empty;

            _chbInvalidGinRetriesLimitEnabled.Checked = true;

            if (_editingObject.CCU != null)
            {
                _itbCrParentObject.Text = _editingObject.CCU.Name;
                _itbCrParentObject.Image = Plugin.GetImageForAOrmObject(_editingObject.CCU);
            }
            else if (_editingObject.DCU != null)
            {
                _itbCrParentObject.Text = _editingObject.DCU.ToString();
                _itbCrParentObject.Image = Plugin.GetImageForAOrmObject(_editingObject.DCU);
            }

            SetDoorEnvironmentReference();
            LoadAlarmAreas();

            _bApply.Enabled = false;
            _bReboot.Enabled = false;
            _chbDisableScreensaver.Checked = false;
            _chbSlCodeLed.Checked = true;
        }

        private void SetDoorEnvironmentReference()
        {
            var doorEnvironment = GetCrDoorEnvironment();

            if (doorEnvironment != null)
            {
                _itbDoorEnvironment.Text = doorEnvironment.Name;
                _itbDoorEnvironment.Image = Plugin.GetImageForAOrmObject(doorEnvironment);
            }
        }

        private DoorEnvironment GetCrDoorEnvironment()
        {
            ICollection<DoorEnvironment> doorEnvironments = null;

            if (_editingObject.DCU != null)
            {
                var dcu = Plugin.MainServerProvider.DCUs.GetObjectById(_editingObject.DCU.IdDCU);

                if (dcu != null
                    && dcu.DoorEnvironments != null
                    && dcu.DoorEnvironments.Count > 0)
                {

                    doorEnvironments = dcu.DoorEnvironments;
                }
            } 
            else if (_editingObject.CCU != null)
            {
                var ccu = Plugin.MainServerProvider.CCUs.GetObjectById(_editingObject.CCU.IdCCU);

                if (ccu != null
                    && ccu.DoorEnvironments != null
                    && ccu.DoorEnvironments.Count > 0)
                {
                    doorEnvironments = ccu.DoorEnvironments;
                }
            }

            if (doorEnvironments == null)
                return null;

            return doorEnvironments.FirstOrDefault(
                doorEnvironment =>
                    (doorEnvironment.CardReaderInternal != null
                     && doorEnvironment.CardReaderInternal.IdCardReader.Equals(_editingObject.IdCardReader))
                    || (doorEnvironment.CardReaderExternal != null
                        && doorEnvironment.CardReaderExternal.IdCardReader.Equals(_editingObject.IdCardReader)));
        }

        private bool HasKeyboard()
        {
            var hasKeyboard = Plugin.MainServerProvider.CardReaders.GetHasKeyboard(_editingObject.IdCardReader);

            return hasKeyboard.HasValue && hasKeyboard.Value;
        }

        private void AddSlForEnterToMenu()
        {
            ICollection<ItemSlForEnterToMenu> list;

            if (!HasKeyboard())
            {
                list = ItemSlForEnterToMenu.GetListSmallCr(
                    true,
                    LocalizationHelper);
            }
            else
            {
                list = ItemSlForEnterToMenu.GetList(
                    true,
                    LocalizationHelper);
            }

            if (list != null && list.Count > 0)
            {
                _cbSLForEnterToMenu.BeginUpdate();
                _cbSLForEnterToMenu.Items.Clear();
                foreach (var itemSlForEnterToMenu in list)
                {
                    _cbSLForEnterToMenu.Items.Add(itemSlForEnterToMenu);

                    if (!itemSlForEnterToMenu.SlForEntetToMenu.HasValue)
                    {
                        if (_editingObject.SLForEnterToMenu == null)
                            _cbSLForEnterToMenu.SelectedItem = itemSlForEnterToMenu;

                        continue;
                    }

                    if (_editingObject.SLForEnterToMenu == (byte)itemSlForEnterToMenu.SlForEntetToMenu)
                        _cbSLForEnterToMenu.SelectedItem = itemSlForEnterToMenu;
                }
                _cbSLForEnterToMenu.EndUpdate();
            }
        }

        protected override void SetValuesEdit()
        {
            AlarmAreasTextsSetPosition();

            SafeThread.StartThread(SetInformation);
            LoadSecurityLevelState();
            _eName.Text = _editingObject.Name;

            _actSecurityTimeZone = _editingObject.SecurityTimeZone;
            if (_actSecurityTimeZone != null)
            {
                _tbmDailyPlan.Text = _editingObject.SecurityTimeZone.ToString();
                _tbmDailyPlan.TextImage = Plugin.GetImageForAOrmObject(_editingObject.SecurityTimeZone);
            }

            _actSecurityDailyPlan = _editingObject.SecurityDailyPlan;
            if (_actSecurityDailyPlan != null)
            {
                _tbmDailyPlan.Text = _editingObject.SecurityDailyPlan.ToString();
                _tbmDailyPlan.TextImage = Plugin.GetImageForAOrmObject(_editingObject.SecurityDailyPlan);
            }
            _chbEmergencyCode.Checked = _editingObject.IsEmergencyCode;
            _chbForcedSL.Checked = _editingObject.IsForcedSecurityLevel;
            _actOnOffObject = _editingObject.OnOffObject;
            _eDescription.Text = _editingObject.Description;

            if (_editingObject.AACardReaders != null)
            {
                foreach (var aaCardReader in _editingObject.AACardReaders)
                {
                    if (aaCardReader.AlarmArea != null)
                    {
                        aaCardReader.AlarmArea = Plugin.MainServerProvider.AlarmAreas.GetObjectById(aaCardReader.AlarmArea.IdAlarmArea);
                    }
                }
            }

            var alarmArcsByAlarmType = new SyncDictionary<AlarmType, ICollection<AlarmArc>>();

            if (_editingObject.CardReaderAlarmArcs != null)
            {
                foreach (var cardReaderAlarmArc in _editingObject.CardReaderAlarmArcs)
                {
                    var alarmArc = cardReaderAlarmArc.AlarmArc;

                    alarmArcsByAlarmType.GetOrAddValue(
                        (AlarmType)cardReaderAlarmArc.AlarmType,
                        key =>
                            new LinkedList<AlarmArc>(),
                        (key, value, newlyAdded) =>
                            value.Add(alarmArc));
                }
            }

            _catsCrOffline.AlarmEnabledCheckState = _editingObject.AlarmOffline;
            _catsCrOffline.BlockAlarmCheckState = _editingObject.BlockAlarmOffline;
            _catsCrOffline.ObjectForBlockingAlarm = GetBlockAlarmObject(
                _editingObject.ObjBlockAlarmOfflineObjectType,
                _editingObject.ObjBlockAlarmOfflineId);

            _cmaaCrOffline.AlarmArcs = GetAlarmArcs(
                alarmArcsByAlarmType,
                AlarmType.CardReader_Offline);

            _catsCrTamperSabotage.AlarmEnabledCheckState = _editingObject.AlarmTamper;
            _catsCrTamperSabotage.BlockAlarmCheckState = _editingObject.BlockAlarmTamper;
            _catsCrTamperSabotage.EventlogDuringBlockedAlarmCheckState = _editingObject.EventlogDuringBlockAlarmTamper;
            _catsCrTamperSabotage.ObjectForBlockingAlarm = GetBlockAlarmObject(
                _editingObject.ObjBlockAlarmTamperObjectType,
                _editingObject.ObjBlockAlarmTamperId);

            _cmaaCrTamperSabotage.AlarmArcs = GetAlarmArcs(
                alarmArcsByAlarmType,
                AlarmType.CardReader_TamperSabotage);

            _catsCrAccessDenied.AlarmEnabledCheckState = _editingObject.AlarmAccessDenied;
            _catsCrAccessDenied.BlockAlarmCheckState = _editingObject.BlockAlarmAccessDenied;
            _catsCrAccessDenied.EventlogDuringBlockedAlarmCheckState = _editingObject.EventlogDuringBlockAlarmAccessDenied;
            _catsCrAccessDenied.ObjectForBlockingAlarm = GetBlockAlarmObject(
                _editingObject.ObjBlockAlarmAccessDeniedObjectType,
                _editingObject.ObjBlockAlarmAccessDeniedId);

            _cmaaCrAccessDenied.AlarmArcs = GetAlarmArcs(
                alarmArcsByAlarmType,
                AlarmType.CardReader_AccessDenied);

            _catsCrUnknownCard.AlarmEnabledCheckState = _editingObject.AlarmUnknownCard;
            _catsCrUnknownCard.BlockAlarmCheckState = _editingObject.BlockAlarmUnknownCard;
            _catsCrUnknownCard.EventlogDuringBlockedAlarmCheckState = _editingObject.EventlogDuringBlockAlarmUnknownCard;
            _catsCrUnknownCard.ObjectForBlockingAlarm = GetBlockAlarmObject(
                _editingObject.ObjBlockAlarmUnknownCardObjectType,
                _editingObject.ObjBlockAlarmUnknownCardId);

            _cmaaCrUnknownCard.AlarmArcs = GetAlarmArcs(
                alarmArcsByAlarmType,
                AlarmType.CardReader_UnknownCard);

            _catsCrCardBlockedOrInactive.AlarmEnabledCheckState = _editingObject.AlarmCardBlockedOrInactive;
            _catsCrCardBlockedOrInactive.BlockAlarmCheckState = _editingObject.BlockAlarmCardBlockedOrInactive;
            _catsCrCardBlockedOrInactive.EventlogDuringBlockedAlarmCheckState = _editingObject.EventlogDuringBlockAlarmCardBlockedOrInactive;
            _catsCrCardBlockedOrInactive.ObjectForBlockingAlarm = GetBlockAlarmObject(
                _editingObject.ObjBlockAlarmCardBlockedOrInactiveObjectType,
                _editingObject.ObjBlockAlarmCardBlockedOrInactiveId);

            _cmaaCrCardBlockedOrInactive.AlarmArcs = GetAlarmArcs(
                alarmArcsByAlarmType,
                AlarmType.CardReader_CardBlockedOrInactive);

            _catsCrInvalidPin.AlarmEnabledCheckState = _editingObject.AlarmInvalidPIN;
            _catsCrInvalidPin.BlockAlarmCheckState = _editingObject.BlockAlarmInvalidPin;
            _catsCrInvalidPin.EventlogDuringBlockedAlarmCheckState = _editingObject.EventlogDuringBlockAlarmInvalidPin;
            _catsCrInvalidPin.ObjectForBlockingAlarm = GetBlockAlarmObject(
                _editingObject.ObjBlockAlarmInvalidPinObjectType,
                _editingObject.ObjBlockAlarmInvalidPinId);

            _cmaaCrInvalidPin.AlarmArcs = GetAlarmArcs(
                alarmArcsByAlarmType,
                AlarmType.CardReader_InvalidPIN);

            _catsCrInvalidGin.AlarmEnabledCheckState = _editingObject.AlarmInvalidGIN;
            _catsCrInvalidGin.BlockAlarmCheckState = _editingObject.BlockAlarmInvalidGin;
            _catsCrInvalidGin.EventlogDuringBlockedAlarmCheckState = _editingObject.EventlogDuringBlockAlarmInvalidGin;
            _catsCrInvalidGin.ObjectForBlockingAlarm = GetBlockAlarmObject(
                _editingObject.ObjBlockAlarmInvalidGinObjectType,
                _editingObject.ObjBlockAlarmInvalidGinId);

            _cmaaCrInvalidGin.AlarmArcs = GetAlarmArcs(
                alarmArcsByAlarmType,
                AlarmType.CardReader_InvalidCode);

            _catsCrInvalidEmergencyCode.AlarmEnabledCheckState = _editingObject.AlarmInvalidEmergencyCode;
            _catsCrInvalidEmergencyCode.BlockAlarmCheckState = _editingObject.BlockAlarmInvalidEmergencyCode;
            _catsCrInvalidEmergencyCode.EventlogDuringBlockedAlarmCheckState = _editingObject.EventlogDuringBlockAlarmInvalidEmergencyCode;
            _catsCrInvalidEmergencyCode.ObjectForBlockingAlarm = GetBlockAlarmObject(
                _editingObject.ObjBlockAlarmInvalidEmergencyCodeObjectType,
                _editingObject.ObjBlockAlarmInvalidEmergencyCodeId);

            _cmaaCrInvalidEmergencyCode.AlarmArcs = GetAlarmArcs(
                alarmArcsByAlarmType,
                AlarmType.CardReader_InvalidEmergencyCode);

            _catsCrAccessPermitted.AlarmEnabledCheckState = _editingObject.AlarmAccessPermitted;
            _catsCrAccessPermitted.BlockAlarmCheckState = _editingObject.BlockAlarmAccessPermitted;
            _catsCrAccessPermitted.ObjectForBlockingAlarm = GetBlockAlarmObject(
                _editingObject.ObjBlockAlarmAccessPermittedObjectType,
                _editingObject.ObjBlockAlarmAccessPermittedId);

            _cmaaCrAccessPermitted.AlarmArcs = GetAlarmArcs(
                alarmArcsByAlarmType,
                AlarmType.CardReader_AccessPermitted);

            _catsCrInvalidGinRetriesLimitReached.AlarmEnabledCheckState = _editingObject.AlarmInvalidGinRetriesLimitReached;
            _catsCrInvalidGinRetriesLimitReached.BlockAlarmCheckState = _editingObject.BlockAlarmInvalidGinRetriesLimitReached;
            _catsCrInvalidGinRetriesLimitReached.ObjectForBlockingAlarm = GetBlockAlarmObject(
                _editingObject.ObjBlockAlarmInvalidGinRetriesLimitReachedObjectType,
                _editingObject.ObjBlockAlarmInvalidGinRetriesLimitReachedId);

            _cmaaCrInvalidGinRetriesLimitReached.AlarmArcs = GetAlarmArcs(
                alarmArcsByAlarmType,
                AlarmType.CardReader_Invalid_Code_Retries_Limit_Reached);

            Guid? idParentCcu = GetIdParentCcu();

            if (idParentCcu != null)
            {
                _catsCrOffline.SetParentCcu(idParentCcu.Value);
                _catsCrTamperSabotage.SetParentCcu(idParentCcu.Value);
                _catsCrAccessDenied.SetParentCcu(idParentCcu.Value);
                _catsCrUnknownCard.SetParentCcu(idParentCcu.Value);
                _catsCrCardBlockedOrInactive.SetParentCcu(idParentCcu.Value);
                _catsCrInvalidPin.SetParentCcu(idParentCcu.Value);
                _catsCrInvalidGin.SetParentCcu(idParentCcu.Value);
                _catsCrInvalidEmergencyCode.SetParentCcu(idParentCcu.Value);
                _catsCrAccessPermitted.SetParentCcu(idParentCcu.Value);
                _catsCrInvalidGinRetriesLimitReached.SetParentCcu(idParentCcu.Value);
            }

            SetLanguage();
            //_rbCRLanguageEnglish.Checked = true;

            _chbCardAppliedLED.Checked = _editingObject.CardAppliedLED;

            _chbCardAppliedKeyboardLight.Checked = _editingObject.CardAppliedKeyboardLight;

            _chbCardAppliedInternalBuzzer.Checked = _editingObject.CardAppliedInternalBuzzer;

            _chbCardAppliedExternalBuzzer.Checked = _editingObject.CardAppliedExternalBuzzer;

            _chbTamperLED.Checked = _editingObject.TamperLED;

            _chbTamperKeyboardLight.Checked = _editingObject.TamperKeyboardLight;

            _chbTamperInternalBuzzer.Checked = _editingObject.TamperInternalBuzzer;

            _chbTamperExternalBuzzer.Checked = _editingObject.TamperExternalBuzzer;

            _chbResetLED.Checked = _editingObject.ResetLED;

            _chbResetKeyboardLight.Checked = _editingObject.ResetKeyboardLight;

            _chbResetInternalBuzzer.Checked = _editingObject.ResetInternalBuzzer;

            _chbResetExternalBuzzer.Checked = _editingObject.ResetExternalBuzzer;

            _chbKeyPressedLED.Checked = _editingObject.KeyPressedLED;

            _chbKeyPressedKeyboardLight.Checked = _editingObject.KeyPressedKeyboardLight;

            _chbKeyPressedInternalBuzzer.Checked = _editingObject.KeyPressedInternalBuzzer;

            _chbKeyPressedExternalBuzzer.Checked = _editingObject.KeyPressedExternalBuzzer;

            _chbInternalBuzzerKillswitch.Checked = _editingObject.InternalBuzzerKillswitch;

            RefreshState((byte)Plugin.MainServerProvider.CardReaders.GetOnlineStates(_editingObject.IdCardReader));

            ChangeCRCommand(_editingObject.IdCardReader, (byte)Plugin.MainServerProvider.CardReaders.GetCardReaderCommand(_editingObject.IdCardReader));

            _eCrLogicalAddress.Text = _editingObject.Address.ToString();

            if (_editingObject.CCU != null)
            {
                _itbCrParentObject.Text = _editingObject.CCU.Name;
                _itbCrParentObject.Image = Plugin.GetImageForAOrmObject(_editingObject.CCU);
            }
            else if (_editingObject.DCU != null)
            {
                _itbCrParentObject.Text = _editingObject.DCU.ToString();
                _itbCrParentObject.Image = Plugin.GetImageForAOrmObject(_editingObject.DCU);
            }

            if (!string.IsNullOrEmpty(_editingObject.GIN))
            {
                _settingDefaultPassword = true;
                _eGin.Text = DEFAULT_PASSWORD;
                _settingDefaultPassword = false;
            }

            if (!string.IsNullOrEmpty(_editingObject.EmergencyCode))
            {
                _settingDefaultPassword = true;
                _eEmergencyCode.Text = DEFAULT_PASSWORD;
                _settingDefaultPassword = false;
            }

            if (!string.IsNullOrEmpty(_editingObject.GinForEnterToMenu))
            {
                _settingDefaultPassword = true;
                _eGinForEnterToMenu.Text = DEFAULT_PASSWORD;
                _settingDefaultPassword = false;
            }

            _chbUseAccessGinForEnterToMenu.Checked = _editingObject.UseAccessGinForEnterToMenu;

            if (_editingObject.SecurityDailyPlanForEnterToMenu != null)
            {
                _tbmSecurityDailyPlanTimeZoneForEnterToMenu.Text = _editingObject.SecurityDailyPlanForEnterToMenu.ToString();
                _tbmSecurityDailyPlanTimeZoneForEnterToMenu.TextImage = Plugin.GetImageForAOrmObject(_editingObject.SecurityDailyPlanForEnterToMenu);
            }

            if (_editingObject.SecurityTimeZoneForEnterToMenu != null)
            {
                _tbmSecurityDailyPlanTimeZoneForEnterToMenu.Text = _editingObject.SecurityTimeZoneForEnterToMenu.ToString();
                _tbmSecurityDailyPlanTimeZoneForEnterToMenu.TextImage = Plugin.GetImageForAOrmObject(_editingObject.SecurityTimeZoneForEnterToMenu);
            }

            if (_editingObject.SpecialOutputForOffline != null)
            {
                _outputOffline = _editingObject.SpecialOutputForOffline;
                _tbmOutputOffline.Text = _outputOffline.ToString();
                _tbmOutputOffline.TextImage = Plugin.GetImageForAOrmObject(_outputOffline);
            }
            else
            {
                _tbmOutputOffline.Text = string.Empty;
            }

            if (_editingObject.SpecialOutputForTamper != null)
            {
                _outputSabotage = _editingObject.SpecialOutputForTamper;
                _tbmOutputSabotage.Text = _outputSabotage.ToString();
                _tbmOutputSabotage.TextImage = Plugin.GetImageForAOrmObject(_outputSabotage);
            }
            else
            {
                _tbmOutputSabotage.Text = string.Empty;
            }

            //set values for function keys
            FillSecurityLevelComboBox();
            FillSymbolComboBox();

            if (_editingObject.FunctionKey1 != null)
            {
                _functionKey1 = _editingObject.FunctionKey1;
                _chbEnableFK1.Checked = _functionKey1.isEnable;

                if (!string.IsNullOrEmpty(_functionKey1.GIN))
                {
                    _settingDefaultPassword = true;
                    _tbGinFK1.Text = DEFAULT_PASSWORD;
                    _settingDefaultPassword = false;
                }

                var output = Plugin.MainServerProvider.Outputs.GetObjectById(_functionKey1.IdOutput);

                if (output != null)
                {
                    _tbObjectRuleFK1.Text = output.Name;
                    _tbObjectRuleFK1.TextImage = Plugin.GetImageForAOrmObject(output);
                }

                switch (_functionKey1.ObjectAction)
                {
                    case ObjectAction.Activate:
                        _rbActivateFK1.Checked = true;
                        break;

                    case ObjectAction.Deactivate:
                        _rbDeactivateFK1.Checked = true;
                        break;

                    case ObjectAction.ActivateDeactivate:
                        _rbActivateDeactivateFK1.Checked = true;
                        break;
                }

                _tbTextFK1.Text = _functionKey1.Text;
                FillTimeZoneOrDailyPlanTextBoxs(_functionKey1, _tbTimeZoneFK1);
            }

            if (_editingObject.FunctionKey2 != null)
            {
                _functionKey2 = _editingObject.FunctionKey2;
                _chbEnableFK2.Checked = _functionKey2.isEnable;

                if (!string.IsNullOrEmpty(_functionKey2.GIN))
                {
                    _settingDefaultPassword = true;
                    _tbGinFK2.Text = DEFAULT_PASSWORD;
                    _settingDefaultPassword = false;
                }

                var output = Plugin.MainServerProvider.Outputs.GetObjectById(_functionKey2.IdOutput);

                if (output != null)
                {
                    _tbObjectRuleFK2.Text = output.Name;
                    _tbObjectRuleFK2.TextImage = Plugin.GetImageForAOrmObject(output);
                }

                switch (_functionKey2.ObjectAction)
                {
                    case ObjectAction.Activate:
                        _rbActivateFK2.Checked = true;
                        break;

                    case ObjectAction.Deactivate:
                        _rbDeactivateFK2.Checked = true;
                        break;

                    case ObjectAction.ActivateDeactivate:
                        _rbActivateDeactivateFK2.Checked = true;
                        break;
                }

                _tbTextFK2.Text = _functionKey2.Text;
                FillTimeZoneOrDailyPlanTextBoxs(_functionKey2, _tbTimeZoneFK2);
            }

            _chbInvalidGinRetriesLimitEnabled.CheckState =
                ConvertToCheckState(_editingObject.InvalidGinRetriesLimitEnabled);

            _chbInvalidPinRetriesLimitEnabled.CheckState =
                ConvertToCheckState(_editingObject.InvalidPinRetriesLimitEnabled);

            LoadAlarmAreas();
            AddSlForEnterToMenu();
            RefreshOnOffObject();
            SetReferencedBy();
            _bApply.Enabled = false;

            _bUnblock.Visible =
                Plugin.MainServerProvider.CardReaders.GetBlockedState(_editingObject.IdCardReader);

            _chbDisableScreensaver.Checked = _editingObject.DisableScreensaver;
            _chbSlCodeLed.Checked = _editingObject.SlCodeLedPresentation;
            SetDoorEnvironmentReference();
        }

        private Guid? GetIdParentCcu()
        {
            Guid? idParentCcu;
            idParentCcu = null;
            if (_editingObject.DCU != null)
            {
                if (_editingObject.DCU.CCU != null)
                    idParentCcu = _editingObject.DCU.CCU.IdCCU;
            }
            else
                if (_editingObject.CCU != null)
                    idParentCcu = _editingObject.CCU.IdCCU;
            return idParentCcu;
        }

        private static CheckState ConvertToCheckState(bool? valueFromDatabase)
        {
            if (!valueFromDatabase.HasValue)
                return CheckState.Indeterminate;

            return valueFromDatabase.Value
                ? CheckState.Checked
                : CheckState.Unchecked;
        }

        private static ICollection<AlarmArc> GetAlarmArcs(
            IDictionary<AlarmType, ICollection<AlarmArc>> alarmArcsByAlarmType,
            AlarmType alarmType)
        {
            ICollection<AlarmArc> alarmArcsForAlarmType;

            if (!alarmArcsByAlarmType.TryGetValue(
                alarmType,
                out alarmArcsForAlarmType))
            {
                return null;
            }

            return alarmArcsForAlarmType;
        }

        private void FillSecurityLevelComboBox()
        {
            _cbSecurityLevelFK1.Items.Clear();
            _cbSecurityLevelFK2.Items.Clear();

            foreach (SecurityLevelForSpecialKey obj in Enum.GetValues(typeof(SecurityLevelForSpecialKey)))
            {
                var securityLevel = new SecurityLevelForFunctionKeyItem(obj, LocalizationHelper);
                _cbSecurityLevelFK1.Items.Add(securityLevel);

                _cbSecurityLevelFK2.Items.Add(securityLevel);

                if (_editingObject.FunctionKey1 != null && _editingObject.FunctionKey1.SecurityLevel == obj)
                    _cbSecurityLevelFK1.SelectedItem = securityLevel;

                if (_editingObject.FunctionKey2 != null && _editingObject.FunctionKey2.SecurityLevel == obj)
                    _cbSecurityLevelFK2.SelectedItem = securityLevel;
            }

            
            if (_cbSecurityLevelFK1.Items.Count > 0 && _cbSecurityLevelFK1.SelectedItem == null)
                _cbSecurityLevelFK1.SelectedIndex = 0;

            if (_cbSecurityLevelFK2.Items.Count > 0 && _cbSecurityLevelFK2.SelectedItem == null)
                _cbSecurityLevelFK2.SelectedIndex = 0;
        }

        private void FillSymbolComboBox()
        {
            _cbSymbolFK1.Items.Clear();
            _cbSymbolFK2.Items.Clear();

            foreach (FunctionKeySymbol obj in Enum.GetValues(typeof(FunctionKeySymbol)))
            {
                var symbol = new SymbolForFunctionKeyItem(obj);
                _cbSymbolFK1.Items.Add(symbol);
                _cbSymbolFK2.Items.Add(symbol);

                if (_editingObject.FunctionKey1 != null && _editingObject.FunctionKey1.Symbol == obj)
                    _cbSymbolFK1.SelectedItem = symbol;

                if (_editingObject.FunctionKey2 != null && _editingObject.FunctionKey2.Symbol == obj)
                    _cbSymbolFK2.SelectedItem = symbol;
            }

            if (_cbSymbolFK1.Items.Count > 0 && _cbSymbolFK1.SelectedItem == null)
                _cbSymbolFK1.SelectedIndex = 0;

            if (_cbSymbolFK2.Items.Count > 0 && _cbSymbolFK2.SelectedItem == null)
                _cbSymbolFK2.SelectedIndex = 0;
        }

        private void FillTimeZoneOrDailyPlanTextBoxs(FunctionKey FK, TextBoxMenu tbm)
        {
            if (FK == null || tbm == null)
                return;

            if (FK.IsUsedTimeZone)
            {
                var tz =
                    CgpClient.Singleton.MainServerProvider.TimeZones.GetObjectById(FK.IdTimeZoneOrDailyPlan);

                if (tz != null)
                {
                    tbm.Text = tz.Name;
                    tbm.TextImage = Plugin.GetImageForAOrmObject(tz);
                }
            }
            else
            {
                var dp =
                    CgpClient.Singleton.MainServerProvider.DailyPlans.GetObjectById(FK.IdTimeZoneOrDailyPlan);

                if (dp != null)
                {
                    tbm.Text = dp.DailyPlanName;
                    tbm.TextImage = Plugin.GetImageForAOrmObject(dp);
                }
            }
        }

        private AOnOffObject GetBlockAlarmObject(byte? objectType, Guid? objectGuid)
        {
            if (!CgpClientMainForm.Singleton.MainIsConnectionLost(true) && objectType != null && objectGuid != null)
            {
                switch (objectType.Value)
                {
                    case (byte)ObjectType.DailyPlan:
                        return CgpClient.Singleton.MainServerProvider.DailyPlans.GetObjectById(objectGuid.Value);
                    case (byte)ObjectType.TimeZone:
                        return CgpClient.Singleton.MainServerProvider.TimeZones.GetObjectById(objectGuid.Value);
                    case (byte)ObjectType.Input:
                        return Plugin.MainServerProvider.Inputs.GetObjectById(objectGuid.Value);
                    case (byte)ObjectType.Output:
                        return Plugin.MainServerProvider.Outputs.GetObjectById(objectGuid.Value);
                }
            }

            return null;
        }

        private void SetLanguage()
        {
            switch (_editingObject.CRLanguage)
            {
                case (byte)CRLanguage.English:
                    _rbCRLanguageEnglish.Checked = true;
                    break;
                case (byte)CRLanguage.Slovak:
                    _rbCRLanguageSlovak.Checked = true;
                    break;
                case (byte)CRLanguage.Swedish:
                    _rbCRLanguageSwedish.Checked = true;
                    break;
                case (byte)CRLanguage.Finnish:
                    _rbCRLanguageFinnish.Checked = true;
                    break;
                case (byte)CRLanguage.Dannish:
                    _rbCRLanguageDanish.Checked = true;
                    break;
                case (byte)CRLanguage.Norwegian:
                    _rbCRLanguageNorwegian.Checked = true;
                    break;
            }
        }

        private void GetLanguage()
        {
            if (_rbCRLanguageEnglish.Checked)
            {
                _editingObject.CRLanguage = (byte)CRLanguage.English;
            }
            else if (_rbCRLanguageSlovak.Checked)
            {
                _editingObject.CRLanguage = (byte)CRLanguage.Slovak;
            }
            else if (_rbCRLanguageSwedish.Checked)
            {
                _editingObject.CRLanguage = (byte)CRLanguage.Swedish;
            }
            else if (_rbCRLanguageFinnish.Checked)
            {
                _editingObject.CRLanguage = (byte)CRLanguage.Finnish;
            }
            else if (_rbCRLanguageDanish.Checked)
            {
                _editingObject.CRLanguage = (byte)CRLanguage.Dannish;
            }
            else if (_rbCRLanguageNorwegian.Checked)
            {
                _editingObject.CRLanguage = (byte)CRLanguage.Norwegian;
            }
        }

        protected override void DisableForm()
        {
            base.DisableForm();
            _bCancel.Enabled = true;
        }

        protected override bool GetValues()
        {
            try
            {
                if (Insert)
                {
                    _editingObject.Address = (byte)_nudLogicalAddress.Value;
                }

                _editingObject.Name = _eName.Text;

                if (!string.IsNullOrEmpty(_eGin.Text))
                {
                    if (_eGin.Text != DEFAULT_PASSWORD)
                    {
                        _editingObject.GIN = QuickHashes.GetCRC32String(_eGin.Text);
                        _editingObject.GinLength = (byte)_eGin.Text.Length;
                    }
                }
                else
                {
                    _editingObject.GIN = string.Empty;
                    _editingObject.GinLength = 0;
                }

                _editingObject.SecurityTimeZone = _actSecurityTimeZone;
                _editingObject.SecurityDailyPlan = _actSecurityDailyPlan;

                _editingObject.IsEmergencyCode = _chbEmergencyCode.Checked;

                if (!string.IsNullOrEmpty(_eEmergencyCode.Text) && _eEmergencyCode.Text != DEFAULT_PASSWORD)
                {
                    _editingObject.EmergencyCode = QuickHashes.GetCRC32String(_eEmergencyCode.Text);
                    _editingObject.EmergencyCodeLength = (byte)_eEmergencyCode.Text.Length;
                }

                _editingObject.SecurityLevel = (byte)((SecurityLevelStates)_cbSecurityLevel.SelectedItem).Value;

                _editingObject.SLForEnterToMenu =
                    (byte?)((ItemSlForEnterToMenu)_cbSLForEnterToMenu.SelectedItem).SlForEntetToMenu;

                _editingObject.UseAccessGinForEnterToMenu = _chbUseAccessGinForEnterToMenu.Checked;

                if (_chbUseAccessGinForEnterToMenu.Checked
                    || string.IsNullOrEmpty(_eGinForEnterToMenu.Text))
                {
                    _editingObject.GinForEnterToMenu = null;
                    _editingObject.GinLengthForEnterToMenu = 0;
                }
                else if (_eGinForEnterToMenu.Text != DEFAULT_PASSWORD)
                {
                    _editingObject.GinForEnterToMenu = QuickHashes.GetCRC32String(_eGinForEnterToMenu.Text);
                    _editingObject.GinLengthForEnterToMenu = (byte)_eGinForEnterToMenu.Text.Length;
                }

                _editingObject.Description = _eDescription.Text;

                if (_chbForcedSL.Checked)
                {
                    _editingObject.IsForcedSecurityLevel = true;
                    _editingObject.ForcedSecurityLevel = (byte)((SecurityLevelStates)_cbForcedSL.SelectedItem).Value;
                    _editingObject.OnOffObject = _actOnOffObject;
                }
                else
                {
                    _editingObject.IsForcedSecurityLevel = false;
                    _editingObject.ForcedSecurityLevel = (byte)((SecurityLevelStates)_cbForcedSL.SelectedItem).Value;
                    _editingObject.OnOffObject = null;
                }

                _editingObject.CardAppliedLED = _chbCardAppliedLED.Checked;
                _editingObject.CardAppliedKeyboardLight = _chbCardAppliedKeyboardLight.Checked;
                _editingObject.CardAppliedInternalBuzzer = _chbCardAppliedInternalBuzzer.Checked;
                _editingObject.CardAppliedExternalBuzzer = _chbCardAppliedExternalBuzzer.Checked;
                _editingObject.TamperLED = _chbTamperLED.Checked;
                _editingObject.TamperKeyboardLight = _chbTamperKeyboardLight.Checked;
                _editingObject.TamperInternalBuzzer = _chbTamperInternalBuzzer.Checked;
                _editingObject.TamperExternalBuzzer = _chbTamperExternalBuzzer.Checked;
                _editingObject.ResetLED = _chbResetLED.Checked;
                _editingObject.ResetKeyboardLight = _chbResetKeyboardLight.Checked;
                _editingObject.ResetInternalBuzzer = _chbResetInternalBuzzer.Checked;
                _editingObject.ResetExternalBuzzer = _chbResetExternalBuzzer.Checked;
                _editingObject.KeyPressedLED = _chbKeyPressedLED.Checked;
                _editingObject.KeyPressedKeyboardLight = _chbKeyPressedKeyboardLight.Checked;
                _editingObject.KeyPressedInternalBuzzer = _chbKeyPressedInternalBuzzer.Checked;
                _editingObject.KeyPressedExternalBuzzer = _chbKeyPressedExternalBuzzer.Checked;
                _editingObject.InternalBuzzerKillswitch = _chbInternalBuzzerKillswitch.Checked;
                SaveAlarmAreas();
                GetLanguage();

                _editingObject.AlarmOffline = _catsCrOffline.AlarmEnabledCheckState;
                _editingObject.BlockAlarmOffline = _catsCrOffline.BlockAlarmCheckState;

                if (_catsCrOffline.ObjectForBlockingAlarm != null)
                {
                    _editingObject.ObjBlockAlarmOfflineObjectType =
                        (byte)_catsCrOffline.ObjectForBlockingAlarm.GetObjectType();

                    _editingObject.ObjBlockAlarmOfflineId =
                        (Guid)_catsCrOffline.ObjectForBlockingAlarm.GetId();
                }
                else
                {
                    _editingObject.ObjBlockAlarmOfflineObjectType = null;
                    _editingObject.ObjBlockAlarmOfflineId = null;
                }

                if (_editingObject.CardReaderAlarmArcs != null)
                    _editingObject.CardReaderAlarmArcs.Clear();
                else
                    _editingObject.CardReaderAlarmArcs = new List<CardReaderAlarmArc>();

                SaveAlarmArcs(
                    _cmaaCrOffline.AlarmArcs,
                    AlarmType.CardReader_Offline);

                _editingObject.AlarmTamper = _catsCrTamperSabotage.AlarmEnabledCheckState;
                _editingObject.BlockAlarmTamper = _catsCrTamperSabotage.BlockAlarmCheckState;
                _editingObject.EventlogDuringBlockAlarmTamper = _catsCrTamperSabotage.EventlogDuringBlockedAlarmCheckState;

                if (_catsCrTamperSabotage.ObjectForBlockingAlarm != null)
                {
                    _editingObject.ObjBlockAlarmTamperObjectType =
                        (byte)_catsCrTamperSabotage.ObjectForBlockingAlarm.GetObjectType();

                    _editingObject.ObjBlockAlarmTamperId =
                        (Guid)_catsCrTamperSabotage.ObjectForBlockingAlarm.GetId();
                }
                else
                {
                    _editingObject.ObjBlockAlarmTamperObjectType = null;
                    _editingObject.ObjBlockAlarmTamperId = null;
                }

                SaveAlarmArcs(
                    _cmaaCrTamperSabotage.AlarmArcs,
                    AlarmType.CardReader_TamperSabotage);

                _editingObject.AlarmAccessDenied = _catsCrAccessDenied.AlarmEnabledCheckState;
                _editingObject.BlockAlarmAccessDenied = _catsCrAccessDenied.BlockAlarmCheckState;
                _editingObject.EventlogDuringBlockAlarmAccessDenied = _catsCrAccessDenied.EventlogDuringBlockedAlarmCheckState;

                if (_catsCrAccessDenied.ObjectForBlockingAlarm != null)
                {
                    _editingObject.ObjBlockAlarmAccessDeniedObjectType =
                        (byte)_catsCrAccessDenied.ObjectForBlockingAlarm.GetObjectType();

                    _editingObject.ObjBlockAlarmAccessDeniedId =
                        (Guid)_catsCrAccessDenied.ObjectForBlockingAlarm.GetId();
                }
                else
                {
                    _editingObject.ObjBlockAlarmAccessDeniedObjectType = null;
                    _editingObject.ObjBlockAlarmAccessDeniedId = null;
                }

                SaveAlarmArcs(
                    _cmaaCrAccessDenied.AlarmArcs,
                    AlarmType.CardReader_AccessDenied);


                _editingObject.AlarmUnknownCard = _catsCrUnknownCard.AlarmEnabledCheckState;
                _editingObject.BlockAlarmUnknownCard = _catsCrUnknownCard.BlockAlarmCheckState;
                _editingObject.EventlogDuringBlockAlarmUnknownCard = _catsCrUnknownCard.EventlogDuringBlockedAlarmCheckState;

                if (_catsCrUnknownCard.ObjectForBlockingAlarm != null)
                {
                    _editingObject.ObjBlockAlarmUnknownCardObjectType =
                        (byte)_catsCrUnknownCard.ObjectForBlockingAlarm.GetObjectType();

                    _editingObject.ObjBlockAlarmUnknownCardId =
                        (Guid)_catsCrUnknownCard.ObjectForBlockingAlarm.GetId();
                }
                else
                {
                    _editingObject.ObjBlockAlarmUnknownCardObjectType = null;
                    _editingObject.ObjBlockAlarmUnknownCardId = null;
                }

                SaveAlarmArcs(
                    _cmaaCrUnknownCard.AlarmArcs,
                    AlarmType.CardReader_UnknownCard);

                _editingObject.AlarmCardBlockedOrInactive = _catsCrCardBlockedOrInactive.AlarmEnabledCheckState;
                _editingObject.BlockAlarmCardBlockedOrInactive = _catsCrCardBlockedOrInactive.BlockAlarmCheckState;
                _editingObject.EventlogDuringBlockAlarmCardBlockedOrInactive = _catsCrCardBlockedOrInactive.EventlogDuringBlockedAlarmCheckState;

                if (_catsCrCardBlockedOrInactive.ObjectForBlockingAlarm != null)
                {
                    _editingObject.ObjBlockAlarmCardBlockedOrInactiveObjectType =
                        (byte)_catsCrCardBlockedOrInactive.ObjectForBlockingAlarm.GetObjectType();

                    _editingObject.ObjBlockAlarmCardBlockedOrInactiveId =
                        (Guid)_catsCrCardBlockedOrInactive.ObjectForBlockingAlarm.GetId();
                }
                else
                {
                    _editingObject.ObjBlockAlarmCardBlockedOrInactiveObjectType = null;
                    _editingObject.ObjBlockAlarmCardBlockedOrInactiveId = null;
                }

                SaveAlarmArcs(
                    _cmaaCrCardBlockedOrInactive.AlarmArcs,
                    AlarmType.CardReader_CardBlockedOrInactive);

                _editingObject.AlarmInvalidPIN = _catsCrInvalidPin.AlarmEnabledCheckState;
                _editingObject.BlockAlarmInvalidPin = _catsCrInvalidPin.BlockAlarmCheckState;
                _editingObject.EventlogDuringBlockAlarmInvalidPin = _catsCrInvalidPin.EventlogDuringBlockedAlarmCheckState;

                if (_catsCrInvalidPin.ObjectForBlockingAlarm != null)
                {
                    _editingObject.ObjBlockAlarmInvalidPinObjectType =
                        (byte)_catsCrInvalidPin.ObjectForBlockingAlarm.GetObjectType();

                    _editingObject.ObjBlockAlarmInvalidPinId =
                        (Guid)_catsCrInvalidPin.ObjectForBlockingAlarm.GetId();
                }
                else
                {
                    _editingObject.ObjBlockAlarmInvalidPinObjectType = null;
                    _editingObject.ObjBlockAlarmInvalidPinId = null;
                }

                SaveAlarmArcs(
                    _cmaaCrInvalidPin.AlarmArcs,
                    AlarmType.CardReader_InvalidPIN);

                _editingObject.AlarmInvalidGIN = _catsCrInvalidGin.AlarmEnabledCheckState;
                _editingObject.BlockAlarmInvalidGin = _catsCrInvalidGin.BlockAlarmCheckState;
                _editingObject.EventlogDuringBlockAlarmInvalidGin = _catsCrInvalidGin.EventlogDuringBlockedAlarmCheckState;

                if (_catsCrInvalidGin.ObjectForBlockingAlarm != null)
                {
                    _editingObject.ObjBlockAlarmInvalidGinObjectType =
                        (byte)_catsCrInvalidGin.ObjectForBlockingAlarm.GetObjectType();

                    _editingObject.ObjBlockAlarmInvalidGinId =
                        (Guid)_catsCrInvalidGin.ObjectForBlockingAlarm.GetId();
                }
                else
                {
                    _editingObject.ObjBlockAlarmInvalidGinObjectType = null;
                    _editingObject.ObjBlockAlarmInvalidGinId = null;
                }

                SaveAlarmArcs(
                    _cmaaCrInvalidGin.AlarmArcs,
                    AlarmType.CardReader_InvalidCode);

                _editingObject.AlarmInvalidEmergencyCode = _catsCrInvalidEmergencyCode.AlarmEnabledCheckState;
                _editingObject.BlockAlarmInvalidEmergencyCode = _catsCrInvalidEmergencyCode.BlockAlarmCheckState;
                _editingObject.EventlogDuringBlockAlarmInvalidEmergencyCode = _catsCrInvalidEmergencyCode.EventlogDuringBlockedAlarmCheckState;

                if (_catsCrInvalidEmergencyCode.ObjectForBlockingAlarm != null)
                {
                    _editingObject.ObjBlockAlarmInvalidEmergencyCodeObjectType =
                        (byte)_catsCrInvalidEmergencyCode.ObjectForBlockingAlarm.GetObjectType();

                    _editingObject.ObjBlockAlarmInvalidEmergencyCodeId =
                        (Guid)_catsCrInvalidEmergencyCode.ObjectForBlockingAlarm.GetId();
                }
                else
                {
                    _editingObject.ObjBlockAlarmInvalidEmergencyCodeObjectType = null;
                    _editingObject.ObjBlockAlarmInvalidEmergencyCodeId = null;
                }

                SaveAlarmArcs(
                    _cmaaCrInvalidEmergencyCode.AlarmArcs,
                    AlarmType.CardReader_InvalidEmergencyCode);

                _editingObject.AlarmAccessPermitted = _catsCrAccessPermitted.AlarmEnabledCheckState;
                _editingObject.BlockAlarmAccessPermitted = _catsCrAccessPermitted.BlockAlarmCheckState;

                if (_catsCrAccessPermitted.ObjectForBlockingAlarm != null)
                {
                    _editingObject.ObjBlockAlarmAccessPermittedObjectType =
                        (byte)_catsCrAccessPermitted.ObjectForBlockingAlarm.GetObjectType();

                    _editingObject.ObjBlockAlarmAccessPermittedId =
                        (Guid)_catsCrAccessPermitted.ObjectForBlockingAlarm.GetId();
                }
                else
                {
                    _editingObject.ObjBlockAlarmAccessPermittedObjectType = null;
                    _editingObject.ObjBlockAlarmAccessPermittedId = null;
                }

                SaveAlarmArcs(
                    _cmaaCrAccessPermitted.AlarmArcs,
                    AlarmType.CardReader_AccessPermitted);

                _editingObject.AlarmInvalidGinRetriesLimitReached = _catsCrInvalidGinRetriesLimitReached.AlarmEnabledCheckState;
                _editingObject.BlockAlarmInvalidGinRetriesLimitReached = _catsCrInvalidGinRetriesLimitReached.BlockAlarmCheckState;

                if (_catsCrInvalidGinRetriesLimitReached.ObjectForBlockingAlarm != null)
                {
                    _editingObject.ObjBlockAlarmInvalidGinRetriesLimitReachedObjectType =
                        (byte)_catsCrInvalidGinRetriesLimitReached.ObjectForBlockingAlarm.GetObjectType();

                    _editingObject.ObjBlockAlarmInvalidGinRetriesLimitReachedId =
                        (Guid)_catsCrInvalidGinRetriesLimitReached.ObjectForBlockingAlarm.GetId();
                }
                else
                {
                    _editingObject.ObjBlockAlarmInvalidGinRetriesLimitReachedObjectType = null;
                    _editingObject.ObjBlockAlarmInvalidGinRetriesLimitReachedId = null;
                }

                SaveAlarmArcs(
                    _cmaaCrInvalidGinRetriesLimitReached.AlarmArcs,
                    AlarmType.CardReader_Invalid_Code_Retries_Limit_Reached);

                //get values for function keys
                if (_functionKey1 != null)
                {
                    _editingObject.FunctionKey1 = _functionKey1;
                    _functionKey1.isEnable = _chbEnableFK1.Checked;
                    _functionKey1.SecurityLevel =
                        ((SecurityLevelForFunctionKeyItem)_cbSecurityLevelFK1.SelectedItem).SecurityLevel;

                    if (!string.IsNullOrEmpty(_tbGinFK1.Text))
                    {
                        if (_tbGinFK1.Text != DEFAULT_PASSWORD)
                        {
                            _functionKey1.GinLenght = _tbGinFK1.Text.Length;
                            _functionKey1.GIN = QuickHashes.GetCRC32String(_tbGinFK1.Text);
                        }
                    }
                    else
                    {
                        _functionKey1.GinLenght = 0;
                        _functionKey1.GIN = null;
                    }

                    if (_rbActivateFK1.Checked)
                        _functionKey1.ObjectAction = ObjectAction.Activate;

                    if (_rbDeactivateFK1.Checked)
                        _functionKey1.ObjectAction = ObjectAction.Deactivate;

                    if (_rbActivateDeactivateFK1.Checked)
                        _functionKey1.ObjectAction = ObjectAction.ActivateDeactivate;

                    _functionKey1.Text = _tbTextFK1.Text;
                    _functionKey1.Symbol = ((SymbolForFunctionKeyItem)_cbSymbolFK1.SelectedItem).Symbol;
                }

                if (_functionKey2 != null)
                {
                    _editingObject.FunctionKey2 = _functionKey2;
                    _functionKey2.isEnable = _chbEnableFK2.Checked;
                    _functionKey2.SecurityLevel =
                        ((SecurityLevelForFunctionKeyItem)_cbSecurityLevelFK2.SelectedItem).SecurityLevel;

                    if (!string.IsNullOrEmpty(_tbGinFK2.Text))
                    {
                        if (_tbGinFK2.Text != DEFAULT_PASSWORD)
                        {
                            _functionKey2.GinLenght = _tbGinFK2.Text.Length;
                            _functionKey2.GIN = QuickHashes.GetCRC32String(_tbGinFK2.Text);
                        }
                    }
                    else
                    {
                        _functionKey2.GinLenght = 0;
                        _functionKey2.GIN = null;
                    }

                    if (_rbActivateDeactivateFK2.Checked)
                        _functionKey2.ObjectAction = ObjectAction.Activate;

                    if (_rbDeactivateFK2.Checked)
                        _functionKey2.ObjectAction = ObjectAction.Deactivate;

                    if (_rbActivateDeactivateFK2.Checked)
                        _functionKey2.ObjectAction = ObjectAction.ActivateDeactivate;

                    _functionKey2.Text = _tbTextFK2.Text;
                    _functionKey2.Symbol = ((SymbolForFunctionKeyItem)_cbSymbolFK2.SelectedItem).Symbol;
                }

                _editingObject.SpecialOutputForOffline = _outputOffline;
                _editingObject.SpecialOutputForTamper = _outputSabotage;
                _editingObject.LocalAlarmInstruction = GetNewLocalAlarmInstruction();

                _editingObject.InvalidGinRetriesLimitEnabled = ConvertFromCheckState(_chbInvalidGinRetriesLimitEnabled.CheckState);
                _editingObject.InvalidPinRetriesLimitEnabled = ConvertFromCheckState(_chbInvalidPinRetriesLimitEnabled.CheckState);

                _editingObject.DisableScreensaver = _chbDisableScreensaver.Checked;
                _editingObject.SlCodeLedPresentation = _chbSlCodeLed.Checked;

                return true;
            }
            catch
            {
                Dialog.Error(GetString("ErrorGetValuesFailed"));
                return false;
            }
        }

        private static bool? ConvertFromCheckState(CheckState checkState)
        {
            switch (checkState)
            {
                case CheckState.Indeterminate:
                    return null;
                case CheckState.Checked:
                    return true;
                default:
                    return false;
            }
        }

        private void SaveAlarmArcs(
            IEnumerable<AlarmArc> alarmArcs,
            AlarmType alarmType)
        {
            if (alarmArcs != null)
            {
                foreach (var alarmArc in alarmArcs)
                {
                    _editingObject.CardReaderAlarmArcs.Add(
                        new CardReaderAlarmArc(
                            _editingObject,
                            alarmArc,
                            alarmType));
                }
            }
        }

        private bool IsKeyboardNecessary(SecurityDailyPlan securityDailyPlan, SecurityTimeZone securityTimeZone)
        {
            IEnumerable<SecurityDayInterval> securityDayIntervals = null;

            if (securityTimeZone != null)
            {
                var securityTimeZoneFromDatabase =
                    Plugin.MainServerProvider.SecurityTimeZones.GetObjectById(securityTimeZone.IdSecurityTimeZone);

                securityDayIntervals = securityTimeZoneFromDatabase != null
                    ? securityTimeZoneFromDatabase.DateSettings
                        .Select(
                            dateSetting =>
                                Plugin.MainServerProvider.SecurityTimeZoneDateSettings.GetObjectById(
                                    dateSetting.IdSecurityTimeZoneDateSetting))
                        .Where(
                            dateSetting =>
                                dateSetting != null
                                && dateSetting.SecurityDailyPlan != null)
                        .Select(
                            dateSetting =>
                                Plugin.MainServerProvider.SecurityDailyPlans.GetObjectById(
                                    dateSetting.SecurityDailyPlan.IdSecurityDailyPlan))
                        .Where(
                            dailyPlan =>
                                dailyPlan != null)
                        .SelectMany(
                            dailyPlan =>
                                dailyPlan.SecurityDayIntervals)
                    : null;
            }
            else if (securityDailyPlan != null)
            {
                var securityDailyPlanFromDatabase =
                    Plugin.MainServerProvider.SecurityDailyPlans.GetObjectById(securityDailyPlan.IdSecurityDailyPlan);

                securityDayIntervals = securityDailyPlanFromDatabase != null
                    ? securityDailyPlanFromDatabase.SecurityDayIntervals
                    : null;
            }

            return securityDayIntervals != null
                   && securityDayIntervals.Any(
                       dayInterval =>
                           dayInterval.IntervalType == (byte)SecurityLevel4SLDP.cardpin
                           || dayInterval.IntervalType == (byte)SecurityLevel4SLDP.code
                           || dayInterval.IntervalType == (byte)SecurityLevel4SLDP.codeorcard
                           || dayInterval.IntervalType == (byte)SecurityLevel4SLDP.codeorcardpin
                           || dayInterval.IntervalType == (byte)SecurityLevel4SLDP.togglecardpin);
        }

        protected override bool CheckValues()
        {
            var crHasKeyboard = HasKeyboard();

            if (string.IsNullOrEmpty(_eName.Text))
            {
                ErrorNotificationOverControl(
                    _eName,
                    "ErrorInsertCardReaderName");
                _eName.Focus();
                return false;
            }
            if (!(_cbSecurityLevel.SelectedItem is SecurityLevelStates))
            {
                ErrorNotificationOverControl(
                        _cbSecurityLevel,
                        "ErrorSelectSecurityLevel");
                _cbSecurityLevel.Focus();
                return false;
            }

            var slForEnterToMenu = ((ItemSlForEnterToMenu)_cbSLForEnterToMenu.SelectedItem).SlForEntetToMenu;

            if (slForEnterToMenu == SecurityLevel.SecurityTimeZoneSecurityDailyPlan)
            {
                if (_editingObject.SecurityDailyPlanForEnterToMenu == null
                    && _editingObject.SecurityTimeZoneForEnterToMenu == null)
                {
                    ErrorNotificationOverControl(
                        _tbmSecurityDailyPlanTimeZoneForEnterToMenu.ImageTextBox,
                        "ErrorInsertSecTimeZoneSecDailyPlan");

                    _tcCardReader.SelectedTab = _tpSettings;
                    _tbmSecurityDailyPlanTimeZoneForEnterToMenu.Focus();

                    return false;
                }

                if (!HasKeyboard()
                    && IsKeyboardNecessary(
                        _editingObject.SecurityDailyPlanForEnterToMenu,
                        _editingObject.SecurityTimeZoneForEnterToMenu))
                {
                    ErrorNotificationOverControl(
                        _tbmSecurityDailyPlanTimeZoneForEnterToMenu.ImageTextBox,
                        _editingObject.SecurityTimeZoneForEnterToMenu != null
                            ? "ErrorSecTimeZoneUnsupportedIntervals"
                            : "ErrorSecDailyPlanUnsupportedIntervals");

                    _tcCardReader.SelectedTab = _tpSettings;
                    _tbmSecurityDailyPlanTimeZoneForEnterToMenu.Focus();

                    return false;
                }
            }

            if (!string.IsNullOrEmpty(_tbGinFK1.Text))
            {
                bool fk1codeLengthError = false;

                if (_tbGinFK1.Text == DEFAULT_PASSWORD)
                {
                    if (PersonEditForm.CodeHasWrongLength(_editingObject.FunctionKey1.GinLenght))
                    {
                        fk1codeLengthError = true;
                    }
                }
                else
                {
                    if (PersonEditForm.CodeHasWrongLength(_tbGinFK1.Text.Length))
                    {
                        fk1codeLengthError = true;
                    }

#if !DEBUG
                    if (PersonEditForm.CodeHasLowSecurityStrength(_tbGinFK1.Text))
                    {
                        ErrorNotificationOverControl(
                            _tbGinFK1,
                            "ErrorGinHasLowSecurityStrength");

                        _tcCardReader.SelectedTab = _tpFunctionKeysSettings;
                        _tbGinFK1.Focus();

                        return false;
                    }
#endif


                }

                if (fk1codeLengthError)
                {
                    ErrorNotificationOverControl(
                            _tbGinFK1,
                            "ErrorWrongGinLength",
                                GeneralOptionsForm.Singleton.MinimalCodeLength,
                                GeneralOptionsForm.Singleton.MaximalCodeLength);

                    _tcCardReader.SelectedTab = _tpFunctionKeysSettings;
                    _tbGinFK1.Focus();

                    return false;
                }
            }

            // ReSharper disable UnusedVariable
            var securityLevelForSpecialKey =
                // ReSharper restore UnusedVariable
            ((SecurityLevelForFunctionKeyItem)
            _cbSecurityLevelFK2.SelectedItem)
                .SecurityLevel;

            if (!string.IsNullOrEmpty(_tbGinFK2.Text))
            {
                bool fk2codeLengthError = false;

                if (_tbGinFK2.Text == DEFAULT_PASSWORD)
                {
                    if (PersonEditForm.CodeHasWrongLength(_editingObject.FunctionKey2.GinLenght))
                    {
                        fk2codeLengthError = true;
                    }
                }
                else
                {
                    if (PersonEditForm.CodeHasWrongLength(_tbGinFK2.Text.Length))
                    {
                        fk2codeLengthError = true;
                    }

#if !DEBUG
                    if (PersonEditForm.CodeHasLowSecurityStrength(_tbGinFK2.Text))
                    {
                        ErrorNotificationOverControl(
                            _tbGinFK2,
                            "ErrorGinHasLowSecurityStrength");

                        _tcCardReader.SelectedTab = _tpFunctionKeysSettings;
                        _tbGinFK2.Focus();

                        return false;
                    }
#endif
                }

                if (fk2codeLengthError)
                {
                    ErrorNotificationOverControl(
                            _tbGinFK2,
                            "ErrorWrongGinLength",
                                GeneralOptionsForm.Singleton.MinimalCodeLength,
                                GeneralOptionsForm.Singleton.MaximalCodeLength);

                    _tcCardReader.SelectedTab = _tpFunctionKeysSettings;
                    _tbGinFK2.Focus();

                    return false;
                }
            }

            if (_chbEnableFK1.Checked)
            {
                if (string.IsNullOrEmpty(_tbObjectRuleFK1.Text))
                {
                    ErrorNotificationOverControl(_tbObjectRuleFK1,"ErrorObjectRule");
                    if (_tcCardReader.SelectedTab != _tpFunctionKeysSettings)
                    {
                        _tcCardReader.SelectedTab = _tpFunctionKeysSettings;
                    }
                    _tbObjectRuleFK1.Focus();
                    return false;
                }

                if (string.IsNullOrEmpty(_tbTextFK1.Text))
                {
                    ErrorNotificationOverControl(_tbTextFK1,"ErrorWrongFunctionKeyText");
                    if (_tcCardReader.SelectedTab != _tpFunctionKeysSettings)
                    {
                        _tcCardReader.SelectedTab = _tpFunctionKeysSettings;
                    }
                    _tbTextFK1.Focus();
                    return false;
                }
            }

            if (_chbEnableFK2.Checked)
            {
                if (string.IsNullOrEmpty(_tbObjectRuleFK2.Text))
                {
                    ErrorNotificationOverControl(_tbObjectRuleFK2,"ErrorObjectRule");
                    if (_tcCardReader.SelectedTab != _tpFunctionKeysSettings)
                    {
                        _tcCardReader.SelectedTab = _tpFunctionKeysSettings;
                    }
                    _tbObjectRuleFK2.Focus();
                    return false;
                }

                if (string.IsNullOrEmpty(_tbTextFK2.Text))
                {
                    ErrorNotificationOverControl(_tbTextFK2,"ErrorWrongFunctionKeyText");
                    if (_tcCardReader.SelectedTab != _tpFunctionKeysSettings)
                    {
                        _tcCardReader.SelectedTab = _tpFunctionKeysSettings;
                    }
                    _tbTextFK2.Focus();
                    return false;
                }
            }

            if (!string.IsNullOrEmpty(_eGin.Text))
            {
                var codeHasWrongLength = false;

                if (_eGin.Text == DEFAULT_PASSWORD)
                {
                    if (PersonEditForm.CodeHasWrongLength(_editingObject.GinLength))
                    {
                        codeHasWrongLength = true;
                    }
                }
                else
                {
                    if (PersonEditForm.CodeHasWrongLength(_eGin.Text.Length))
                    {
                        codeHasWrongLength = true;
                    }

#if !DEBUG
                    if (PersonEditForm.CodeHasLowSecurityStrength(_eGin.Text))
                    {
                        ErrorNotificationOverControl(
                            _eGin,
                            "ErrorGinHasLowSecurityStrength");

                        _tcCardReader.SelectedTab = _tpSettings;
                        _eGin.Focus();

                        return false;
                    }
#endif
                }

                if (codeHasWrongLength)
                {
                    ErrorNotificationOverControl(
                            _eGin,
                            "ErrorWrongGinLength",
                                GeneralOptionsForm.Singleton.MinimalCodeLength,
                                GeneralOptionsForm.Singleton.MaximalCodeLength);

                    _tcCardReader.SelectedTab = _tpSettings;
                    _eGin.Focus();

                    return false;
                }
            }

            if (!string.IsNullOrEmpty(_eGinForEnterToMenu.Text))
            {
                bool notifyWrongLengthError = false;
                bool notifyInsufficientCodeSecurity = false;
                try
                {
                    if (_eGinForEnterToMenu.Text == DEFAULT_PASSWORD)
                    {
                        if (_editingObject.GinForEnterToMenu != null &&
                            PersonEditForm.CodeHasWrongLength(_editingObject.GinLengthForEnterToMenu))
                        {
                            notifyWrongLengthError = true;

                            return false;
                        }
                    }
                    else
                        if (!_chbUseAccessGinForEnterToMenu.Checked)
                        {
                            if (PersonEditForm.CodeHasWrongLength(_eGinForEnterToMenu.Text.Length))
                            {
                                notifyWrongLengthError = true;

                                return false;
                            }

                            //#if !DEBUG
                            if (PersonEditForm.CodeHasLowSecurityStrength(_eGinForEnterToMenu.Text))
                            {
                                notifyInsufficientCodeSecurity = true;

                                return false;
                            }
                            //#endif
                        }
                    
                }
                finally
                {
                    string errorMessage = null;

                    
                    if (notifyWrongLengthError)
                    {
                        errorMessage =
                                    string.Format(
                                        GetString("ErrorWrongGinLength"),
                                        GeneralOptionsForm.Singleton.MinimalCodeLength,
                                        GeneralOptionsForm.Singleton.MaximalCodeLength);

                    }
                    else if (notifyInsufficientCodeSecurity)
                    {
                        errorMessage = GetString("ErrorGinHasLowSecurityStrength");
                    }

                    if (errorMessage != null)
                    {
                        ErrorNotificationOverControl(
                            errorMessage,
                            _eGinForEnterToMenu
                            );

                        _tcCardReader.SelectedTab = _tpSettings;
                        _eGinForEnterToMenu.Focus();
                    }
                }
            }

            if (_cbSecurityLevel.SelectedItem == null)
            {
                ErrorNotificationOverControl(_cbSecurityLevel,"ErrorInsertSecurityLevel");
                if (_tcCardReader.SelectedTab != _tpSettings)
                {
                    _tcCardReader.SelectedTab = _tpSettings;
                }
                _cbSecurityLevel.Focus();
                return false;
            }

            if (_chbEmergencyCode.Checked && string.IsNullOrEmpty(_eEmergencyCode.Text))
            {
                ErrorNotificationOverControl(_eEmergencyCode,"ErrorInsertEmergencyCode");
                if (_tcCardReader.SelectedTab != _tpSettings)
                {
                    _tcCardReader.SelectedTab = _tpSettings;
                }
                _eEmergencyCode.Focus();
                return false;
            }

            if (!string.IsNullOrEmpty(_eEmergencyCode.Text) && _eEmergencyCode.Text != DEFAULT_PASSWORD)
            {
                if (_eEmergencyCode.Text.Length < GeneralOptionsForm.Singleton.MinimalCodeLength
                    || _eEmergencyCode.Text.Length > GeneralOptionsForm.Singleton.MaximalCodeLength)
                {
                    ErrorNotificationOverControl(
                        _eEmergencyCode,
                        "ErrorWrongEmergencyCodeLength",
                        GeneralOptionsForm.Singleton.MinimalCodeLength,
                        GeneralOptionsForm.Singleton.MaximalCodeLength);

                    _tcCardReader.SelectedTab = _tpSettings;
                    _eEmergencyCode.Focus();

                    return false;
                }

#if !DEBUG
                if (PersonEditForm.CodeHasLowSecurityStrength(_eEmergencyCode.Text))
                {
                    ErrorNotificationOverControl(
                        _eEmergencyCode,
                        "ErrorEmergencyCodeHasLowSecurityStrength");

                    _tcCardReader.SelectedTab = _tpSettings;
                    _eEmergencyCode.Focus();

                    return false;
                }
#endif
            }

            if ((_cbSecurityLevel.SelectedItem as SecurityLevelStates).Value == SecurityLevel.SecurityTimeZoneSecurityDailyPlan)
            {
                if (_actSecurityTimeZone == null && _actSecurityDailyPlan == null)
                {
                    ErrorNotificationOverControl(
                        _tbmDailyPlan.ImageTextBox,
                        "ErrorInsertSecTimeZoneSecDailyPlan");
                    if (_tcCardReader.SelectedTab != _tpSettings)
                    {
                        _tcCardReader.SelectedTab = _tpSettings;
                    }
                    _tbmDailyPlan.Focus();
                    return false;
                }

                if (!crHasKeyboard
                    && IsKeyboardNecessary(
                        _actSecurityDailyPlan,
                        _actSecurityTimeZone))
                {
                    ErrorNotificationOverControl(_tbmDailyPlan.ImageTextBox,
                        _actSecurityTimeZone != null
                            ? "ErrorSecTimeZoneUnsupportedIntervals"
                            : "ErrorSecDailyPlanUnsupportedIntervals");
                    if (_tcCardReader.SelectedTab != _tpSettings)
                    {
                        _tcCardReader.SelectedTab = _tpSettings;
                    }
                    _tbmDailyPlan.Focus();
                    return false;
                }
            }

            if (_chbForcedSL.Checked)
            {
                if (_cbForcedSL.SelectedItem == null)
                {
                    ErrorNotificationOverControl(_cbForcedSL,"ErrorInsertForcedSecurityLevel");
                    if (_tcCardReader.SelectedTab != _tpSettings)
                    {
                        _tcCardReader.SelectedTab = _tpSettings;
                    }
                    _cbForcedSL.Focus();
                    return false;
                }
                if (_actOnOffObject == null)
                {
                    ErrorNotificationOverControl(_tbmObjectOnOff.ImageTextBox,"ErrorInsertObjectOnOff");
                    if (_tcCardReader.SelectedTab != _tpSettings)
                    {
                        _tcCardReader.SelectedTab = _tpSettings;
                    }
                    _tbmObjectOnOff.ImageTextBox.Focus();
                    return false;
                }
            }

            if (!_catsCrOffline.ControlValues(
                control =>
                    _openAndScrollToControlByAlarmType[AlarmType.CardReader_Offline]()))
            {
                return false;
            }

            if (!_catsCrTamperSabotage.ControlValues(
                control =>
                    _openAndScrollToControlByAlarmType[AlarmType.CardReader_TamperSabotage]()))
            {
                return false;
            }

            if (!_catsCrAccessDenied.ControlValues(
                control =>
                    _openAndScrollToControlByAlarmType[AlarmType.CardReader_AccessDenied]()))
            {
                return false;
            }

            if (!_catsCrUnknownCard.ControlValues(
                control =>
                    _openAndScrollToControlByAlarmType[AlarmType.CardReader_UnknownCard]()))
            {
                return false;
            }

            if (!_catsCrCardBlockedOrInactive.ControlValues(
                control =>
                    _openAndScrollToControlByAlarmType[AlarmType.CardReader_CardBlockedOrInactive]()))
            {
                return false;
            }

            if (!_catsCrInvalidPin.ControlValues(
                control =>
                    _openAndScrollToControlByAlarmType[AlarmType.CardReader_InvalidPIN]()))
            {
                return false;
            }

            if (!_catsCrInvalidGin.ControlValues(
                control =>
                    _openAndScrollToControlByAlarmType[AlarmType.CardReader_InvalidCode]()))
            {
                return false;
            }

            if (!_catsCrInvalidEmergencyCode.ControlValues(
                control =>
                    _openAndScrollToControlByAlarmType[AlarmType.CardReader_InvalidEmergencyCode]()))
            {
                return false;
            }

            if (!_catsCrAccessPermitted.ControlValues(
                control =>
                    _openAndScrollToControlByAlarmType[AlarmType.CardReader_AccessPermitted]()))
            {
                return false;
            }

            if (!_catsCrInvalidGinRetriesLimitReached.ControlValues(
                control =>
                    _openAndScrollToControlByAlarmType[AlarmType.CardReader_Invalid_Code_Retries_Limit_Reached]()))
            {
                return false;
            }

            return true;
        }

        private void CancelClick(object sender, EventArgs e)
        {
            Cancel_Click();
        }

        private void OkClick(object sender, EventArgs e)
        {
            Ok_Click();
        }

        private const string ErrorCodeAlreadyUsed = "ErrorCodeAlreadyUsed";

        protected override bool SaveToDatabaseInsert()
        {
            Exception error;
            var retValue = Plugin.MainServerProvider.CardReaders.Insert(ref _editingObject, out error);
            if (!retValue && error != null)
            {
                if (error is SqlUniqueException && error.Message.Contains(CardReader.COLUMNNAME))
                {
                    ErrorNotificationOverControl(_eName,"ErrorUsedCardReaderName");
                    _eName.Focus();
                }
                else if (error is SqlUniqueException && error.Message.Contains(CardReader.COLUMNADDRESS))
                {
                    ErrorNotificationOverControl(_nudLogicalAddress,"ErrorUsedLogicalAddress");
                    _nudLogicalAddress.Focus();
                }
                else if (error is SqlUniqueException && error.Message == CardReader.COLUMNGIN)
                {
                    ErrorNotificationOverControl(_eGin, ErrorCodeAlreadyUsed);

                    _tcCardReader.SelectedTab = _tpSettings;
                    _eGin.Focus();

                    return false;
                }
                else if (error is SqlUniqueException && error.Message == CardReader.COLUMN_GIN_FOR_ENTER_TO_MENU)
                {
                    ErrorNotificationOverControl(_eGinForEnterToMenu,ErrorCodeAlreadyUsed);

                    _tcCardReader.SelectedTab = _tpSettings;
                    _eGinForEnterToMenu.Focus();

                    return false;
                }
                else if (error is SqlUniqueException && error.Message == CardReader.COLUMNFUNCTIONKEY1)
                {
                    ErrorNotificationOverControl(_tbGinFK1,ErrorCodeAlreadyUsed);

                    _tcCardReader.SelectedTab = _tpFunctionKeysSettings;
                    _tbGinFK1.Focus();

                    return false;
                }
                else if (error is SqlUniqueException && error.Message == CardReader.COLUMNFUNCTIONKEY2)
                {
                    ErrorNotificationOverControl(_tbGinFK2,ErrorCodeAlreadyUsed);

                    _tcCardReader.SelectedTab = _tpFunctionKeysSettings;
                    _tbGinFK2.Focus();

                    return false;
                }
                else
                    throw error;
            }

            return retValue;
        }

       

        protected override bool SaveToDatabaseEdit()
        {
            return SaveToDatabaseEditCore(false);
        }

        protected override bool SaveToDatabaseEditOnlyInDatabase()
        {
            return SaveToDatabaseEditCore(true);
        }

        private bool SaveToDatabaseEditCore(bool OnlyInDatabase)
        {
            Exception error;

            bool retValue =
                OnlyInDatabase
                    ? Plugin.MainServerProvider.CardReaders.UpdateOnlyInDatabase(_editingObject, out error)
                    : Plugin.MainServerProvider.CardReaders.Update(_editingObject, out error);

            if (!retValue && error != null)
            {
                if (error is SqlUniqueException && error.Message.Contains(CardReader.COLUMNNAME))
                {
                    ErrorNotificationOverControl(
                        _eName,"ErrorUsedCardReaderName");

                    _eName.Focus();

                    return false;
                }
                else if (error is SqlUniqueException && error.Message == CardReader.COLUMNGIN)
                {
                    ErrorNotificationOverControl(
                        _eGin,
                        ErrorCodeAlreadyUsed);

                    _tcCardReader.SelectedTab = _tpSettings;
                    _eGin.Focus();

                    return false;
                }
                else if (error is SqlUniqueException && error.Message == CardReader.COLUMN_GIN_FOR_ENTER_TO_MENU)
                {
                    ErrorNotificationOverControl(_eGinForEnterToMenu,ErrorCodeAlreadyUsed);

                    _tcCardReader.SelectedTab = _tpSettings;
                    _eGinForEnterToMenu.Focus();

                    return false;
                }
                else if (error is SqlUniqueException && error.Message == CardReader.COLUMNFUNCTIONKEY1)
                {
                    ErrorNotificationOverControl(_tbGinFK1,ErrorCodeAlreadyUsed);

                    _tcCardReader.SelectedTab = _tpFunctionKeysSettings;
                    _tbGinFK1.Focus();

                    return false;
                }
                else if (error is SqlUniqueException && error.Message == CardReader.COLUMNFUNCTIONKEY2)
                {
                    ErrorNotificationOverControl(_tbGinFK2,ErrorCodeAlreadyUsed);

                    _tcCardReader.SelectedTab = _tpFunctionKeysSettings;
                    _tbGinFK2.Focus();

                    return false;
                }
                else
                    throw error;
            }

            return retValue;
        }

        protected override void EditTextChanger(object sender, EventArgs e)
        {
            if (!_settingDefaultPassword
                && (sender == _eGin
                || sender == _eGinForEnterToMenu
                || sender == _tbGinFK1
                || sender == _tbGinFK2
                || sender == _eEmergencyCode))
            {
                var textBox = (TextBox)sender;

                if (textBox.Text == DEFAULT_PASSWORD)
                    textBox.Text = string.Empty;

                var validText = QuickParser.GetValidDigitString(textBox.Text);

                if (textBox.Text != validText)
                {
                    textBox.Text = validText;
                    textBox.SelectionStart = textBox.TextLength;
                    textBox.SelectionLength = 0;
                }

                var maximalCodeLength = GeneralOptionsForm.Singleton.MaximalCodeLength;

                if (textBox.Text.Length > maximalCodeLength)
                {
                    textBox.Text = textBox.Text.Substring(0, maximalCodeLength);
                    textBox.SelectionStart = textBox.Text.Length;
                    textBox.SelectionLength = 0;
                }
            }

            if (sender == _nudLogicalAddress)
            {
                if (_editingObject.EnableParentInFullName)
                    _eName.Text = "CR" + ((int)_nudLogicalAddress.Value);
                else
                {
                    if (_editingObject.CCU != null)
                        _eName.Text = _editingObject.CCU.Name + StringConstants.SLASHWITHSPACES;
                    if (_editingObject.DCU != null)
                    {
                        if (_editingObject.DCU.CCU != null)
                            _eName.Text = _editingObject.DCU.CCU.Name + StringConstants.SLASHWITHSPACES;

                        if (_editingObject.DCU.Name.Contains(StringConstants.SLASH[0]))
                            _eName.Text += _editingObject.DCU.Name.Split(StringConstants.SLASH[0])[1] + StringConstants.SLASHWITHSPACES;
                        else
                            _eName.Text += _editingObject.DCU.Name + StringConstants.SLASHWITHSPACES;
                    }
                    _eName.Text += "CR" + ((int)_nudLogicalAddress.Value);
                }
            }

            if (sender == _cbSLForEnterToMenu)
                SecurityLevelForEnterToMenuChanged();

            if (sender == _chbUseAccessGinForEnterToMenu)
                _eGinForEnterToMenu.Enabled = !_chbUseAccessGinForEnterToMenu.Checked;

            if (!Insert)
                _bApply.Enabled = true;

            base.EditTextChanger(sender, e);
        }

        protected override void EditTextChangerOnlyInDatabase(object sender, EventArgs e)
        {
            base.EditTextChangerOnlyInDatabase(sender, e);

            if (!Insert)
                _bApply.Enabled = true;
        }

        protected override void EditEnd()
        {
            if (Plugin.MainServerProvider != null && Plugin.MainServerProvider.CardReaders != null)
                Plugin.MainServerProvider.CardReaders.EditEnd(_editingObject);
        }

        private void LoadSecurityLevelState()
        {
            var hasKeyboard = HasKeyboard();

            IList<SecurityLevelStates> securityLevelStates = !hasKeyboard
                ? SecurityLevelStates.GetCardStatesListSmallCR(LocalizationHelper)
                : SecurityLevelStates.GetCardStatesList(LocalizationHelper);

            byte securityLevel;
            if (!Insert)
            {
                securityLevel = _editingObject.SecurityLevel;
            }
            else
            {
                if (!hasKeyboard)
                {
                    securityLevel = (byte)SecurityLevel.CARD;
                }
                else
                {
                    securityLevel = (byte)SecurityLevel.CARDPIN;
                }
            }

            byte forcedSecurityLevel = 0;
            if (!Insert)
            {
                forcedSecurityLevel = _editingObject.ForcedSecurityLevel;
            }

            _cbSecurityLevel.BeginUpdate();
            _cbSecurityLevel.Items.Clear();
            _cbForcedSL.Items.Clear();
            foreach (var slState in securityLevelStates)
            {
                _cbSecurityLevel.Items.Add(slState);

                if (securityLevel == (byte)slState.Value)
                    _cbSecurityLevel.SelectedItem = slState;

                if (slState.Value != SecurityLevel.SecurityTimeZoneSecurityDailyPlan)
                {
                    _cbForcedSL.Items.Add(slState);

                    if (forcedSecurityLevel == (byte)slState.Value)
                        _cbForcedSL.SelectedItem = slState;
                }
            }

            _cbSecurityLevel.EndUpdate();
        }

        private void SecurityLevelChanged()
        {
            if (((SecurityLevelStates)_cbSecurityLevel.SelectedItem).Value == SecurityLevel.SecurityTimeZoneSecurityDailyPlan)
            {
                if (_isEnabledSDP)
                    _tbmDailyPlan.Enabled = true;
            }
            else
            {
                _tbmDailyPlan.Enabled = false;
            }
        }

        private void SecurityLevelForEnterToMenuChanged()
        {
            if (((ItemSlForEnterToMenu)_cbSLForEnterToMenu.SelectedItem).SlForEntetToMenu == SecurityLevel.SecurityTimeZoneSecurityDailyPlan)
            {
                if (_isEnabledSDP)
                    _tbmSecurityDailyPlanTimeZoneForEnterToMenu.Enabled = true;
            }
            else
            {
                _tbmSecurityDailyPlanTimeZoneForEnterToMenu.Enabled = false;
            }
        }

        private void _cbSecurityLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_oldSecurityLevelState != null && _editingObject != null && _editingObject.IdCardReader != Guid.Empty)
            {
                var ss = ((SecurityLevelStates)_cbSecurityLevel.SelectedItem).Value;
                if (_oldSecurityLevelState.Value != SecurityLevel.Unlocked && ss == SecurityLevel.Unlocked &&
                    Plugin.MainServerProvider.CardReaders.IsUsedInDoorEnvironmentByGuid(_editingObject.IdCardReader))
                {
                    if (!ConfirmSelectSlUnlocked())
                    {
                        _cbSecurityLevel.SelectedItem = _oldSecurityLevelState;
                        return;
                    }
                    _oldSecurityLevelState = (_cbSecurityLevel.SelectedItem as SecurityLevelStates);
                }
            }
            EditTextChanger(sender, e);
            SecurityLevelChanged();
        }

        private bool ConfirmSelectSlUnlocked()
        {
            var secondCardReaderSecurityLevel = Plugin.MainServerProvider.DoorEnvironments.GetSecondCardReaderSecurityLevelUsedInDoorEnvironmentsWithCardReader(_editingObject.IdCardReader);
            if (secondCardReaderSecurityLevel != null && secondCardReaderSecurityLevel.Value == SecurityLevel.Locked)
            {
                return Dialog.WarningQuestion(GetString("WarningQuestionUnlockLockedDoorEnvironment"));
            }

            if (_implicitAlarmArea != null)
            {
                var activationState = Plugin.MainServerProvider.AlarmAreas.GetAlarmAreaActivationState(_implicitAlarmArea.IdAlarmArea);
                if (activationState == ActivationState.Set)
                {
                    return Dialog.WarningQuestion(GetString("WarningQuestionUnlockLockedDoorEnvironmentByAA"));
                }
            }

            return Dialog.WarningQuestion(GetString("WarningQuestionUnlockDoorEnvironment"));
        }

        SecurityLevelStates _oldSecurityLevelState;
        private void _cbSecurityLevel_DropDown(object sender, EventArgs e)
        {
            if (_cbSecurityLevel.SelectedItem is SecurityLevelStates)
            {
                _oldSecurityLevelState = (_cbSecurityLevel.SelectedItem as SecurityLevelStates);
            }
        }

        private void _chbEmergencyCode_CheckedChanged(object sender, EventArgs e)
        {
            EditTextChanger(sender, e);
            _eEmergencyCode.Enabled = _chbEmergencyCode.Checked;
        }

        private void _chbForcedSL_CheckedChanged(object sender, EventArgs e)
        {
            EditTextChanger(sender, e);
            if (_chbForcedSL.Checked)
            {
                _cbForcedSL.Enabled = true;
                _tbmObjectOnOff.Enabled = true;
            }
            else
            {
                _cbForcedSL.Enabled = false;
                _tbmObjectOnOff.Enabled = false;
            }
        }
        private void ModifyTimeZoneDaily()
        {
            if (CgpClient.Singleton.IsConnectionLost(true)) return;

            try
            {
                var listModObj = new List<IModifyObject>();
                Exception error;
                var listSecurityDailyPlansFromDatabase = Plugin.MainServerProvider.SecurityDailyPlans.ListModifyObjects(out error);
                if (error != null) throw error;
                listModObj.AddRange(listSecurityDailyPlansFromDatabase);
                var listSecurityTimeZonesFromDatabase = Plugin.MainServerProvider.SecurityTimeZones.ListModifyObjects(out error);
                if (error != null) throw error;
                listModObj.AddRange(listSecurityTimeZonesFromDatabase);

                var formAdd = new ListboxFormAdd(listModObj, GetString("NCASCardReaderEditFormModifySecTimeZoneSecDailyPlan"));
                IModifyObject outModObj;
                formAdd.ShowDialog(out outModObj);
                if (outModObj != null)
                {

                    if (outModObj.GetOrmObjectType == ObjectType.SecurityTimeZone)
                    {
                        var securityTimeZone = Plugin.MainServerProvider.SecurityTimeZones.GetObjectById(outModObj.GetId);
                        SetTimeZone(securityTimeZone);
                        Plugin.AddToRecentList(_actSecurityTimeZone);
                    }
                    else
                    {
                        var securityDailyPlan = Plugin.MainServerProvider.SecurityDailyPlans.GetObjectById(outModObj.GetId);
                        SetDailyPlan(securityDailyPlan);
                        Plugin.AddToRecentList(_actSecurityDailyPlan);
                    }
                }
            }
            catch
            {
            }
        }

        private void AddTimeZoneDailyPlan(object newSecurityTimeZoneSecurityDailyPlan)
        {
            try
            {
                if (newSecurityTimeZoneSecurityDailyPlan is SecurityTimeZone)
                {
                    SetTimeZone(newSecurityTimeZoneSecurityDailyPlan as SecurityTimeZone);
                    Plugin.AddToRecentList(newSecurityTimeZoneSecurityDailyPlan);
                }
                else if (newSecurityTimeZoneSecurityDailyPlan is SecurityDailyPlan)
                {
                    SetDailyPlan(newSecurityTimeZoneSecurityDailyPlan as SecurityDailyPlan);
                    Plugin.AddToRecentList(newSecurityTimeZoneSecurityDailyPlan);
                }
                else
                {
                    ErrorNotificationOverControl(_tbmDailyPlan.ImageTextBox,"ErrorWrongObjectType");
                }
            }
            catch
            { }
        }

        private void SetDailyPlan(SecurityDailyPlan objSecurityDailyPlan)
        {
            EditTextChanger(null, null);
            _actSecurityTimeZone = null;
            _actSecurityDailyPlan = objSecurityDailyPlan;
            if (objSecurityDailyPlan == null)
            {
                _tbmDailyPlan.Text = string.Empty;
            }
            else
            {
                _tbmDailyPlan.Text = objSecurityDailyPlan.Name;
                _tbmDailyPlan.TextImage = Plugin.GetImageForAOrmObject(objSecurityDailyPlan);
            }
        }

        private void SetTimeZone(SecurityTimeZone objSecurityTimeZone)
        {
            EditTextChanger(null, null);
            _actSecurityDailyPlan = null;
            _actSecurityTimeZone = objSecurityTimeZone;
            if (objSecurityTimeZone == null)
            {
                _tbmDailyPlan.Text = string.Empty;
            }
            else
            {
                _tbmDailyPlan.Text = objSecurityTimeZone.Name;
                _tbmDailyPlan.TextImage = Plugin.GetImageForAOrmObject(objSecurityTimeZone);
            }
        }

        private Guid GetParentCCUGuid()
        {
            try
            {
                if (_editingObject != null)
                {
                    if (_editingObject.CCU != null)
                    {
                        return _editingObject.CCU.IdCCU;
                    }
                    if (_editingObject.DCU != null && _editingObject.DCU.CCU != null)
                    {
                        return _editingObject.DCU.CCU.IdCCU;
                    }
                }
            }
            catch { }

            return Guid.Empty;
        }

        private void ModifyObject()
        {
            if (CgpClient.Singleton.IsConnectionLost(true)) return;

            try
            {
                var listObjects = new List<IModifyObject>();

                Exception error;
                var listTz = CgpClient.Singleton.MainServerProvider.TimeZones.ListModifyObjects(out error);
                if (error != null) throw error;
                listObjects.AddRange(listTz);

                var listDailyPlans = CgpClient.Singleton.MainServerProvider.DailyPlans.ListModifyObjects(out error);
                if (error != null) throw error;
                DbsSupport.TranslateDailyPlans(listDailyPlans);
                listObjects.AddRange(listDailyPlans);

                var ccuGuid = GetParentCCUGuid();

                IList<IModifyObject> listInputs = ccuGuid != Guid.Empty
                    ? Plugin.MainServerProvider.Inputs.ListModifyObjectsFromCCU(ccuGuid, out error)
                    : Plugin.MainServerProvider.Inputs.ListModifyObjects(out error);

                if (error != null)
                    throw error;

                if (listInputs != null)
                    listObjects.AddRange(listInputs);

                IList<IModifyObject> listOutputs = ccuGuid != Guid.Empty
                    ? Plugin.MainServerProvider.Outputs.ListModifyObjectsFromCCU(ccuGuid, out error)
                    : Plugin.MainServerProvider.Outputs.ListModifyObjects(out error);

                if (error != null)
                    throw error;

                if (listOutputs != null)
                {
                    listObjects.AddRange(listOutputs);
                }

                var formAdd =
                    new ListboxFormAdd(
                        listObjects,
                        GetString("NCASAlaramAreaEditFormListObjectForAutomaticActivationText"));

                IModifyObject outObject;
                formAdd.ShowDialog(out outObject);

                if (outObject == null)
                    return;

                _actOnOffObject = null;

                switch (outObject.GetOrmObjectType)
                {
                    case ObjectType.TimeZone:
                        {
                            var timeZone = CgpClient.Singleton.MainServerProvider.TimeZones.GetObjectById(outObject.GetId);
                            _actOnOffObject = timeZone;
                            Plugin.AddToRecentList(_actOnOffObject);
                            break;
                        }
                    case ObjectType.DailyPlan:
                        {
                            var dailyPlan = CgpClient.Singleton.MainServerProvider.DailyPlans.GetObjectById(outObject.GetId);
                            _actOnOffObject = dailyPlan;
                            Plugin.AddToRecentList(_actOnOffObject);
                            break;
                        }
                    case ObjectType.Input:
                        {
                            var input = Plugin.MainServerProvider.Inputs.GetObjectById(outObject.GetId);
                            _actOnOffObject = input;
                            Plugin.AddToRecentList(_actOnOffObject);
                            break;
                        }
                    case ObjectType.Output:
                        {
                            var output = Plugin.MainServerProvider.Outputs.GetObjectById(outObject.GetId);
                            _actOnOffObject = output;
                            Plugin.AddToRecentList(_actOnOffObject);
                            break;
                        }
                }

                RefreshOnOffObject();
            }
            catch (Exception ex)
            {
                Dialog.Error(ex);
            }
        }

        private void AddOnOffObject(object newOnOffObject)
        {
            try
            {
                if (newOnOffObject.GetType() == typeof(TimeZone) ||
                    newOnOffObject.GetType() == typeof(DailyPlan) ||
                    newOnOffObject.GetType() == typeof(Input) || newOnOffObject.GetType() == typeof(Output))
                {
                    SetOnOffObject((AOnOffObject)newOnOffObject, true);
                    Plugin.AddToRecentList(newOnOffObject);
                }
                else
                {
                    ErrorNotificationOverControl(_tbmObjectOnOff.ImageTextBox,"ErrorWrongObjectType");
                }
            }
            catch
            { }
        }

        private void SetOnOffObject(AOnOffObject onOffobject, bool controlInputOutput)
        {
            var isCorrectInputOutput = true;
            if (controlInputOutput && (onOffobject is Input || onOffobject is Output))
            {
                var ccuGuid = GetParentCCUGuid();

                if (ccuGuid != Guid.Empty)
                {
                    var input = onOffobject as Input;

                    if (input != null)
                    {
                        if (input.CCU != null && input.CCU.IdCCU != ccuGuid
                            || input.DCU != null && input.DCU.CCU != null && input.DCU.CCU.IdCCU != ccuGuid)
                        {
                            isCorrectInputOutput = false;
                        }
                    }
                    else
                    {
                        var output = (Output)onOffobject;

                        if (output.CCU != null && output.CCU.IdCCU != ccuGuid
                            || output.DCU != null && output.DCU.CCU != null && output.DCU.CCU.IdCCU != ccuGuid)
                        {
                            isCorrectInputOutput = false;
                        }
                    }
                }
            }

            if (!isCorrectInputOutput)
            {
                ErrorNotificationOverControl(
                    _tbmObjectOnOff.ImageTextBox,
                    "ErrorCardReaderInputOutputFromCurrentCCU");

                return;
            }

            EditTextChanger(null, null);
            if (onOffobject != null)
            {
                _actOnOffObject = onOffobject;
                _tbmObjectOnOff.Text = onOffobject.ToString();
                _tbmObjectOnOff.TextImage = Plugin.GetImageForAOrmObject(onOffobject);
            }
            else
            {
                _actOnOffObject = null;
                _tbmObjectOnOff.Text = string.Empty;
            }
        }

        private void RefreshOnOffObject()
        {
            if (_actOnOffObject == null)
            {
                _tbmObjectOnOff.Text = string.Empty;
            }
            else
            {
                var listFS = new ListObjFS(_actOnOffObject);
                _tbmObjectOnOff.Text = listFS.ToString();
                _tbmObjectOnOff.TextImage = Plugin.GetImageForAOrmObject(_actOnOffObject);
            }
        }

        private void _tbmDailyPlan_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                var output = e.Data.GetFormats();

                if (output == null)
                    return;

                AddTimeZoneDailyPlan(e.Data.GetData(output[0]));
            }
            catch
            {
            }
        }

        private void _tbmDailyPlan_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void _tbmScurityDailyPlanTimeZoneForEnterToMenu_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                var output = e.Data.GetFormats();

                if (output == null)
                    return;

                var newSecurityTimeZoneSecurityDailyPlan = e.Data.GetData(output[0]);

                var newSecurityDailyPlan = newSecurityTimeZoneSecurityDailyPlan as SecurityDailyPlan;

                if (newSecurityDailyPlan != null)
                {
                    SetSecurityDailyPlanForEnterToMenu(newSecurityDailyPlan);
                    Plugin.AddToRecentList(newSecurityDailyPlan);
                    return;
                }

                var newSecurityTimeZone = newSecurityTimeZoneSecurityDailyPlan as SecurityTimeZone;

                if (newSecurityTimeZone != null)
                {
                    SetSecurityTimeZoneForEnterToMenu(newSecurityTimeZone);
                    Plugin.AddToRecentList(newSecurityTimeZone);

                    return;
                }

                ErrorNotificationOverControl(
                    _tbmSecurityDailyPlanTimeZoneForEnterToMenu.ImageTextBox,
                    "ErrorWrongObjectType");
            }
            catch
            {
            }
        }

        private void SetSecurityDailyPlanForEnterToMenu(SecurityDailyPlan securityDailyPlan)
        {
            EditTextChanger(null, null);
            _editingObject.SecurityTimeZoneForEnterToMenu = null;
            _editingObject.SecurityDailyPlanForEnterToMenu = securityDailyPlan;

            if (securityDailyPlan == null)
            {
                _tbmSecurityDailyPlanTimeZoneForEnterToMenu.Text = string.Empty;
            }
            else
            {
                _tbmSecurityDailyPlanTimeZoneForEnterToMenu.Text = securityDailyPlan.Name;
                _tbmSecurityDailyPlanTimeZoneForEnterToMenu.TextImage = Plugin.GetImageForAOrmObject(securityDailyPlan);
            }
        }

        private void SetSecurityTimeZoneForEnterToMenu(SecurityTimeZone securityTimeZone)
        {
            EditTextChanger(null, null);
            _editingObject.SecurityDailyPlanForEnterToMenu = null;
            _editingObject.SecurityTimeZoneForEnterToMenu = securityTimeZone;

            if (securityTimeZone == null)
            {
                _tbmSecurityDailyPlanTimeZoneForEnterToMenu.Text = string.Empty;
            }
            else
            {
                _tbmSecurityDailyPlanTimeZoneForEnterToMenu.Text = securityTimeZone.Name;
                _tbmSecurityDailyPlanTimeZoneForEnterToMenu.TextImage = Plugin.GetImageForAOrmObject(securityTimeZone);
            }
        }

        private void ModifySecurityDailyPlanTimeZoneForEnterToMenu()
        {
            if (CgpClient.Singleton.IsConnectionLost(true)) return;

            try
            {
                Exception error;

                var listModObj = new List<IModifyObject>();
                var listSecurityDailyPlansFromDatabase = Plugin.MainServerProvider.SecurityDailyPlans.ListModifyObjects(out error);
                if (error != null) throw error;
                listModObj.AddRange(listSecurityDailyPlansFromDatabase);
                var listSecurityTimeZonesFromDatabase = Plugin.MainServerProvider.SecurityTimeZones.ListModifyObjects(out error);
                if (error != null) throw error;
                listModObj.AddRange(listSecurityTimeZonesFromDatabase);

                var formAdd = new ListboxFormAdd(listModObj, GetString("NCASCardReaderEditFormModifySecTimeZoneSecDailyPlan"));
                IModifyObject outModObj;
                formAdd.ShowDialog(out outModObj);
                if (outModObj != null)
                {

                    if (outModObj.GetOrmObjectType == ObjectType.SecurityTimeZone)
                    {
                        var securityTimeZone = Plugin.MainServerProvider.SecurityTimeZones.GetObjectById(outModObj.GetId);
                        SetSecurityTimeZoneForEnterToMenu(securityTimeZone);
                        Plugin.AddToRecentList(securityTimeZone);
                    }
                    else
                    {
                        var securityDailyPlan = Plugin.MainServerProvider.SecurityDailyPlans.GetObjectById(outModObj.GetId);
                        SetSecurityDailyPlanForEnterToMenu(securityDailyPlan);
                        Plugin.AddToRecentList(securityDailyPlan);
                    }
                }
            }
            catch
            {
            }
        }

        private void _tbmScurityDailyPlanTimeZoneForEnterToMenu_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void _tbmObjectOnOff_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                var output = e.Data.GetFormats();
                if (output == null) return;
                AddOnOffObject(e.Data.GetData(output[0]));
            }
            catch
            {
            }
        }

        private void _tbmObjectOnOff_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void _tbmDailyPlan_DoubleClick(object sender, EventArgs e)
        {
            if (_actSecurityTimeZone != null)
            {
                NCASSecurityTimeZonesForm.Singleton.OpenEditForm(_actSecurityTimeZone);
            }
            else if (_actSecurityDailyPlan != null)
            {
                NCASSecurityDailyPlansForm.Singleton.OpenEditForm(_actSecurityDailyPlan);
            }
        }

        private void _tbmSecurityDailyPlanTimeZoneForEnterToMenu_DoubleClick(object sender, EventArgs e)
        {
            if (_editingObject.SecurityDailyPlanForEnterToMenu != null)
            {
                NCASSecurityDailyPlansForm.Singleton.OpenEditForm(_editingObject.SecurityDailyPlanForEnterToMenu);
            }
            else if (_editingObject.SecurityTimeZoneForEnterToMenu != null)
            {
                NCASSecurityTimeZonesForm.Singleton.OpenEditForm(_editingObject.SecurityTimeZoneForEnterToMenu);
            }
        }

        private void _tbmObjectOnOff_DoubleClick(object sender, EventArgs e)
        {
            if (_actOnOffObject != null)
            {
                if (_actOnOffObject is TimeZone)
                {
                    TimeZonesForm.Singleton.OpenEditForm(_actOnOffObject as TimeZone);
                }
                else if (_actOnOffObject is DailyPlan)
                {
                    DailyPlansForm.Singleton.OpenEditForm(_actOnOffObject as DailyPlan);
                }
                else if (_actOnOffObject is Input)
                {
                    NCASInputsForm.Singleton.OpenEditForm(_actOnOffObject as Input);
                }
                else if (_actOnOffObject is Output)
                {
                    NCASOutputsForm.Singleton.OpenEditForm(_actOnOffObject as Output);
                }
            }
        }

        private void AlarmAreasTextsSetPosition()
        {
            _lName2.Left = ALARMAREASLEFTMARGIN;
            _lAlarmAreasSet1.Left = _panelLinesAlarmAreas.ClientSize.Width - ALARMAREASRIGHTMARGIN - (5 * ALARMAREASCOMBOBOXWIDTH) + (ALARMAREASCOMBOBOXWIDTH - _lAlarmAreasSet1.Width) / 2;
            _lAlarmAreasSet.Left = _panelLinesAlarmAreas.ClientSize.Width - ALARMAREASRIGHTMARGIN - (5 * ALARMAREASCOMBOBOXWIDTH) + (ALARMAREASCOMBOBOXWIDTH - _lAlarmAreasSet.Width) / 2 - _gbFilterSettings.Left;
            _cbFilterSetAA.Left = _panelLinesAlarmAreas.ClientSize.Width - ALARMAREASRIGHTMARGIN - (5 * ALARMAREASCOMBOBOXWIDTH) + (ALARMAREASCOMBOBOXWIDTH - _cbFilterSetAA.Width) / 2 - _gbFilterSettings.Left;
            _lAlarmAreasUnset1.Left = _panelLinesAlarmAreas.ClientSize.Width - ALARMAREASRIGHTMARGIN - (4 * ALARMAREASCOMBOBOXWIDTH) + (ALARMAREASCOMBOBOXWIDTH - _lAlarmAreasUnset1.Width) / 2;
            _lAlarmAreasUnset.Left = _panelLinesAlarmAreas.ClientSize.Width - ALARMAREASRIGHTMARGIN - (4 * ALARMAREASCOMBOBOXWIDTH) + (ALARMAREASCOMBOBOXWIDTH - _lAlarmAreasUnset.Width) / 2 - _gbFilterSettings.Left;
            _cbFilterUnsetAA.Left = _panelLinesAlarmAreas.ClientSize.Width - ALARMAREASRIGHTMARGIN - (4 * ALARMAREASCOMBOBOXWIDTH) + (ALARMAREASCOMBOBOXWIDTH - _cbFilterUnsetAA.Width) / 2 - _gbFilterSettings.Left;
            _lAlarmAreasUnconditionalSet1.Left = _panelLinesAlarmAreas.ClientSize.Width - ALARMAREASRIGHTMARGIN - (3 * ALARMAREASCOMBOBOXWIDTH) + (ALARMAREASCOMBOBOXWIDTH - _lAlarmAreasUnconditionalSet1.Width) / 2;
            _lAlarmAreasUnconditionalSet.Left = _panelLinesAlarmAreas.ClientSize.Width - ALARMAREASRIGHTMARGIN - (3 * ALARMAREASCOMBOBOXWIDTH) + (ALARMAREASCOMBOBOXWIDTH - _lAlarmAreasUnconditionalSet.Width) / 2 - _gbFilterSettings.Left;
            _cbFilterACKAA.Left = _panelLinesAlarmAreas.ClientSize.Width - ALARMAREASRIGHTMARGIN - (3 * ALARMAREASCOMBOBOXWIDTH) + (ALARMAREASCOMBOBOXWIDTH - _cbFilterACKAA.Width) / 2 - _gbFilterSettings.Left;
            _lAlarmAreasImplicitMember1.Left = _panelLinesAlarmAreas.ClientSize.Width - ALARMAREASRIGHTMARGIN - (2 * ALARMAREASCOMBOBOXWIDTH) + (ALARMAREASCOMBOBOXWIDTH - _lAlarmAreasImplicitMember1.Width) / 2;
            _lAlarmAreasImplicitMember.Left = _panelLinesAlarmAreas.ClientSize.Width - ALARMAREASRIGHTMARGIN - (2 * ALARMAREASCOMBOBOXWIDTH) + (ALARMAREASCOMBOBOXWIDTH - _lAlarmAreasImplicitMember.Width) / 2 - _gbFilterSettings.Left;
            _cbFilterImplicitMemberAA.Left = _panelLinesAlarmAreas.ClientSize.Width - ALARMAREASRIGHTMARGIN - (2 * ALARMAREASCOMBOBOXWIDTH) + (ALARMAREASCOMBOBOXWIDTH - _cbFilterImplicitMemberAA.Width) / 2 - _gbFilterSettings.Left;

            _lCrEventlog1.Left = _panelLinesAlarmAreas.ClientSize.Width - ALARMAREASRIGHTMARGIN - ALARMAREASCOMBOBOXWIDTH + (ALARMAREASCOMBOBOXWIDTH - _lAlarmAreasImplicitMember.Width) / 2 - _gbFilterSettings.Left - 5;
            _lCrEventlog2.Left = _panelLinesAlarmAreas.ClientSize.Width - ALARMAREASRIGHTMARGIN - ALARMAREASCOMBOBOXWIDTH + (ALARMAREASCOMBOBOXWIDTH - _lAlarmAreasImplicitMember.Width) / 2 - _gbFilterSettings.Left - 5;
            _cbFilterCrEventlog.Left = _panelLinesAlarmAreas.ClientSize.Width - ALARMAREASRIGHTMARGIN - ALARMAREASCOMBOBOXWIDTH + (ALARMAREASCOMBOBOXWIDTH - _cbFilterImplicitMemberAA.Width) / 2 - _gbFilterSettings.Left;
        }

        private void LoadAlarmAreas()
        {
            _panelLinesAlarmAreas.AutoScrollPosition = new Point(0, 0);

            var top = ALARMAREASSTARTY;
            if (_alarmAreasComboBox.Count > 0)
            {
                foreach (var kvp in _alarmAreasComboBox)
                {
                    SetAlarmAreaCardReaderSettings(kvp.Value, ref top);
                }
            }
            else
            {
                if (CgpClient.Singleton.IsConnectionLost(true)) return;

                Exception error;
                var alarmAreas = Plugin.MainServerProvider.AlarmAreas.SelectByCriteria(null, out error);

                if (error != null || alarmAreas == null)
                {
                    return;
                }

                var guidParentCCU = Guid.Empty;
                if (_editingObject.DCU != null)
                {
                    if (_editingObject.DCU.CCU != null)
                        guidParentCCU = _editingObject.DCU.CCU.IdCCU;
                }
                else if (_editingObject.CCU != null)
                {
                    guidParentCCU = _editingObject.CCU.IdCCU;
                }

                foreach (var alarmArea in alarmAreas)
                {
                    var setAlarmaArea = true;
                    // ReSharper disable ConditionIsAlwaysTrueOrFalse
                    if (!NCASClient.INTER_CCU_COMMUNICATION)
                        // ReSharper restore ConditionIsAlwaysTrueOrFalse
                    {
                        var imlicitCCUForAlarmArea = Plugin.MainServerProvider.AlarmAreas.GetImplicitCCUForAlarmArea(alarmArea.IdAlarmArea);

                        if (imlicitCCUForAlarmArea == null || imlicitCCUForAlarmArea.IdCCU != guidParentCCU)
                        {
                            setAlarmaArea = false;
                        }
                    }

                    if (setAlarmaArea)
                    {
                        var alarmAreaCardReaderSetting = CreateAlarmAreaLine(alarmArea);
                        SetAlarmAreaCardReaderSettings(alarmAreaCardReaderSetting, ref top);
                    }
                }

                if (!Insert)
                {
                    if (_editingObject.AACardReaders != null)
                    {
                        foreach (var aaCardReader in _editingObject.AACardReaders)
                        {
                            var actAACardReader = Plugin.MainServerProvider.AACardReaders.GetObjectById(aaCardReader.IdAACardReader);

                            if (actAACardReader != null)
                            {
                                if (_alarmAreasComboBox.ContainsKey(actAACardReader.AlarmArea.IdAlarmArea))
                                {
                                    _alarmAreasComboBox[actAACardReader.AlarmArea.IdAlarmArea].Set(actAACardReader.AASet, actAACardReader.AAUnset,
                                                                                                   actAACardReader.AAUnconditionalSet, actAACardReader.PermanentlyUnlock, actAACardReader.EnableEventlog);

                                    if (actAACardReader.PermanentlyUnlock == false)
                                        _implicitAlarmArea = actAACardReader.AlarmArea;
                                }
                            }
                        }

                        EnableDisablePermanentlyUnlock();
                    }
                }
            }

            RefreshImplicitAlarmArea();
            _alarmAreaSettingsChanged = false;
        }

        private void EnableDisablePermanentlyUnlock()
        {
            this.BeginInvokeInUI(() =>
            {
                if (_alarmAreasComboBox != null && _alarmAreasComboBox.Count > 0)
                {
                    foreach (var alarmAreaCardReaderSetting in _alarmAreasComboBox.Values)
                    {
                        //alarmAreaCardReaderSetting.IsSet() &&
                        if (_implicitAlarmArea != null &&
                            !_implicitAlarmArea.Compare(alarmAreaCardReaderSetting.AlarmArea))
                        {
                            alarmAreaCardReaderSetting.DisablePermanentlyUnlock();
                        }
                        else
                        {
                            alarmAreaCardReaderSetting.EnablePermanentlyUnlock();
                        }
                    }
                }
            });
        }

        private void SetAlarmAreaCardReaderSettings(AlaramAreaCardReadSettings alarmAreaCardReaderSetting, ref int top)
        {
            var bShow = true;

            if (_eFilterNameAA.Text != string.Empty && !alarmAreaCardReaderSetting.GetText().ToUpper().Contains(_eFilterNameAA.Text.ToUpper()))
                bShow = false;

            if (bShow && _cbFilterSetAA.Checked && !alarmAreaCardReaderSetting.GetAlarmAreaSet())
                bShow = false;

            if (bShow && _cbFilterUnsetAA.Checked && !alarmAreaCardReaderSetting.GetAlarmAreaUnset())
                bShow = false;

            if (bShow && _cbFilterACKAA.Checked && !alarmAreaCardReaderSetting.GetAlarmAreaUnconditionalSet())
                bShow = false;

            if (bShow && _cbFilterImplicitMemberAA.Checked && !alarmAreaCardReaderSetting.GetAlarmAreaPermanentlyUnlock())
                bShow = false;

            if (bShow && _cbFilterShowOnlySetAA.Checked && !alarmAreaCardReaderSetting.IsSet())
                bShow = false;

            if (bShow)
            {
                alarmAreaCardReaderSetting.SetTop(top);
                alarmAreaCardReaderSetting.SetVisible(true);
                top += ALARMAREASHEIGHT + ALARMAREASLINESPACE;
            }
            else
            {
                alarmAreaCardReaderSetting.SetVisible(false);
            }
        }

        private readonly Dictionary<Guid, AlaramAreaCardReadSettings> _alarmAreasComboBox =
            new Dictionary<Guid, AlaramAreaCardReadSettings>();

        private AlaramAreaCardReadSettings CreateAlarmAreaLine(AlarmArea alarmArea)
        {
            if (alarmArea != null)
            {
                var panel = new Panel
                {
                    Parent = _panelLinesAlarmAreas,
                    Left = ALARMAREASLEFTMARGIN,
                    Top = ALARMAREASSTARTY,
                    Height = ALARMAREASHEIGHT,
                    Width =
                        _panelLinesAlarmAreas.Width - ALARMAREASLEFTMARGIN - ALARMAREASRIGHTMARGIN,
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
                };

                panel.MouseEnter += AlarmAreasPanelMouseEnter;
                panel.MouseLeave += AlarmAreasPanelMouseLeave;

                var name = new Label
                {
                    Parent = panel,
                    Text = alarmArea.ToString(),
                    Top = 0,
                    Left = ALARMAREASLEFTMARGIN,
                    Height = ALARMAREASHEIGHT,
                    Width = panel.Width - (5 * ALARMAREASCOMBOBOXWIDTH),
                    TextAlign = ContentAlignment.MiddleLeft,
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                    Tag = alarmArea.IdAlarmArea
                };

                name.DoubleClick += AlarmAreasNameDoubleClick;
                name.MouseLeave += AlarmAreasMouseLeave;
                name.MouseEnter += AlarmAreasMouseEnter;

                var cbSet = new CheckBox
                {
                    Parent = panel,
                    Top = 0,
                    Left = panel.Width - (5 * ALARMAREASCOMBOBOXWIDTH),
                    Height = ALARMAREASHEIGHT,
                    Width = ALARMAREASCOMBOBOXWIDTH,
                    CheckAlign = ContentAlignment.MiddleCenter,
                    Anchor = AnchorStyles.Top | AnchorStyles.Right
                };

                cbSet.CheckedChanged += AlarmAreaSettingChanged;
                cbSet.MouseLeave += AlarmAreasMouseLeave;
                cbSet.MouseEnter += AlarmAreasMouseEnter;

                var cbUnset = new CheckBox
                {
                    Parent = panel,
                    Top = 0,
                    Left = panel.Width - (4 * ALARMAREASCOMBOBOXWIDTH),
                    Height = ALARMAREASHEIGHT,
                    Width = ALARMAREASCOMBOBOXWIDTH,
                    CheckAlign = ContentAlignment.MiddleCenter,
                    Anchor = AnchorStyles.Top | AnchorStyles.Right
                };

                cbUnset.CheckedChanged += AlarmAreaSettingChanged;
                cbUnset.MouseLeave += AlarmAreasMouseLeave;
                cbUnset.MouseEnter += AlarmAreasMouseEnter;

                var cbUnconditionalSet = new CheckBox
                {
                    Parent = panel,
                    Top = 0,
                    Left = panel.Width - (3 * ALARMAREASCOMBOBOXWIDTH),
                    Height = ALARMAREASHEIGHT,
                    Width = ALARMAREASCOMBOBOXWIDTH,
                    CheckAlign = ContentAlignment.MiddleCenter,
                    Anchor = AnchorStyles.Top | AnchorStyles.Right
                };

                cbUnconditionalSet.CheckedChanged += AlarmAreaSettingChanged;
                cbUnconditionalSet.MouseLeave += AlarmAreasMouseLeave;
                cbUnconditionalSet.MouseEnter += AlarmAreasMouseEnter;

                var cbPermanentlyUnlocked = new CheckBox
                {
                    Parent = panel,
                    Top = 0,
                    Left = panel.Width - (2 * ALARMAREASCOMBOBOXWIDTH),
                    Height = ALARMAREASHEIGHT,
                    Width = ALARMAREASCOMBOBOXWIDTH,
                    CheckAlign = ContentAlignment.MiddleCenter,
                    Anchor = AnchorStyles.Top | AnchorStyles.Right
                };

                cbPermanentlyUnlocked.CheckedChanged += AlarmAreaSettingChanged;
                cbPermanentlyUnlocked.MouseLeave += AlarmAreasMouseLeave;
                cbPermanentlyUnlocked.MouseEnter += AlarmAreasMouseEnter;

                var cbEnableEventlog = new CheckBox
                {
                    Parent = panel,
                    Top = 0,
                    Left = panel.Width - ALARMAREASCOMBOBOXWIDTH,
                    Height = ALARMAREASHEIGHT,
                    Width = ALARMAREASCOMBOBOXWIDTH,
                    CheckAlign = ContentAlignment.MiddleCenter,
                    Anchor = AnchorStyles.Top | AnchorStyles.Right
                };

                cbEnableEventlog.CheckedChanged += AlarmAreaSettingChanged;
                cbEnableEventlog.MouseLeave += AlarmAreasMouseLeave;
                cbEnableEventlog.MouseEnter += AlarmAreasMouseEnter;

                var alarmAreaCardReaderSetting =
                    new AlaramAreaCardReadSettings(
                        alarmArea,
                        panel,
                        name,
                        cbSet,
                        cbUnset,
                        cbUnconditionalSet,
                        cbPermanentlyUnlocked,
                        cbEnableEventlog,
                        ChangeImplictAlarmArea);

                _alarmAreasComboBox[alarmArea.IdAlarmArea] = alarmAreaCardReaderSetting;

                return alarmAreaCardReaderSetting;
            }

            return null;
        }

        private bool _alarmAreaSettingsChanged;  

        private Action<Guid, bool> _eventBlockedStateChangedCardReader;

        private void AlarmAreaSettingChanged(object sender, EventArgs e)
        {
            EditTextChanger(null, null);
            _alarmAreaSettingsChanged = true;
        }

        private void ChangeImplictAlarmArea(AlarmArea alarmArea, bool setImplicitAlarmArea)
        {
            if (setImplicitAlarmArea)
            {
                if (_implicitAlarmArea == null)
                    _implicitAlarmArea = alarmArea;
            }
            else
            {
                if (alarmArea != null && alarmArea.Compare(_implicitAlarmArea))
                    _implicitAlarmArea = null;
            }

            EnableDisablePermanentlyUnlock();
            RefreshImplicitAlarmArea();
        }

        static void AlarmAreasMouseEnter(object sender, EventArgs e)
        {
            var control = sender as Control;

            if (control != null)
            {
                AlarmAreasPanelMouseEnter(control.Parent, e);
            }
        }

        void AlarmAreasMouseLeave(object sender, EventArgs e)
        {
            var control = sender as Control;

            if (control != null)
            {
                AlarmAreasPanelMouseLeave(control.Parent, e);
            }
        }

        void AlarmAreasPanelMouseLeave(object sender, EventArgs e)
        {
            var panel = sender as Panel;
            if (panel != null)
            {
                if (ActiveForm != this || MousePosition.X <= panel.PointToScreen(new Point(0, 0)).X || MousePosition.Y <= panel.PointToScreen(new Point(0, 0)).Y ||
                    MousePosition.X >= panel.PointToScreen(new Point(panel.Width, panel.Height)).X || MousePosition.Y >= panel.PointToScreen(new Point(panel.Width, panel.Height)).Y)
                {
                    foreach (Control control in panel.Controls)
                        control.BackColor = Color.White;
                }
            }
        }

        private static void AlarmAreasPanelMouseEnter(object sender, EventArgs e)
        {
            var panel = sender as Panel;
            if (panel != null)
            {
                foreach (Control control in panel.Controls)
                    control.BackColor = Color.LightBlue;
            }
        }

        private void AlarmAreasNameDoubleClick(object sender, EventArgs e)
        {
            if (CgpClient.Singleton.IsConnectionLost(false)) return;
            var label = sender as Label;

            if (label == null)
                return;

            if (!(label.Tag is Guid))
                return;

            var guidAlarmArea = (Guid)label.Tag;

            var alarmArea = Plugin.MainServerProvider.AlarmAreas.GetObjectById(guidAlarmArea);

            if (alarmArea != null)
                NCASAlarmAreasForm.Singleton.OpenEditForm(alarmArea);
        }

        public void SaveAlarmAreas()
        {
            if (CgpClient.Singleton.IsConnectionLost(false)) return;
            if (!_alarmAreaSettingsChanged)
                return;

            if (_editingObject.AACardReaders != null)
                _editingObject.AACardReaders.Clear();
            else
                _editingObject.AACardReaders = new List<AACardReader>();

            if (_alarmAreasComboBox != null)
            {
                foreach (var kvp in _alarmAreasComboBox)
                {
                    var alarmAreaCardReaderSetting = kvp.Value;

                    if (alarmAreaCardReaderSetting == null
                        || !alarmAreaCardReaderSetting.IsSet())
                    {
                        continue;
                    }

                    var aaCardReader = new AACardReader
                    {
                        CardReader = _editingObject,
                        AASet = alarmAreaCardReaderSetting.GetAlarmAreaSet(),
                        AAUnset = alarmAreaCardReaderSetting.GetAlarmAreaUnset(),
                        AAUnconditionalSet =
                            alarmAreaCardReaderSetting.GetAlarmAreaUnconditionalSet(),
                        PermanentlyUnlock =
                            alarmAreaCardReaderSetting.GetAlarmAreaPermanentlyUnlock(),
                        EnableEventlog =
                            alarmAreaCardReaderSetting.GetAlarmAreaCardReaderEnbaleEvnetlog()
                    };

                    var alarmArea =
                        Plugin.MainServerProvider.AlarmAreas
                            .GetObjectById(kvp.Key);

                    if (alarmArea == null)
                        continue;
                    aaCardReader.AlarmArea = alarmArea;
                    _editingObject.AACardReaders.Add(aaCardReader);
                }
            }
        }

        private void AlarmAreaFilterChanged(object sender, EventArgs e)
        {
            LoadAlarmAreas();
        }

        private void _bFilterClear_Click(object sender, EventArgs e)
        {
            _eFilterNameAA.Text = string.Empty;
            _cbFilterSetAA.Checked = false;
            _cbFilterUnsetAA.Checked = false;
            _cbFilterACKAA.Checked = false;
            _cbFilterImplicitMemberAA.Checked = false;
            _cbFilterShowOnlySetAA.Checked = false;

            LoadAlarmAreas();
        }

        private void _panelLinesAlarmAreas_Resize(object sender, EventArgs e)
        {
            AlarmAreasTextsSetPosition();
        }

        private void BlockedStateChangedCardReader(
            Guid idCardReader,
            bool isBlocked)
        {
            if (idCardReader != _editingObject.IdCardReader)
                return;

            this.BeginInvokeInUI(() => _bUnblock.Visible = isBlocked);
        }

        private void ChangeState(Guid idCardReader, byte state, Guid parent)
        {
            if (idCardReader == _editingObject.IdCardReader)
            {
                RefreshState(state);
            }
        }

        private void RefreshState(byte state)
        {
            this.BeginInvokeInUI(() =>
            {
                switch ((OnlineState) state)
                {
                    case OnlineState.Offline:
                        _eState.Text = GetString("Offline");
                        _eState.BackColor = Color.Red;
                        break;
                    case OnlineState.Online:
                        _eState.Text = GetString("Online");
                        _eState.BackColor = Color.LightGreen;
                        break;
                    case OnlineState.Upgrading:
                        _eState.Text = GetString("Upgrading");
                        _eState.BackColor = Color.Orange;
                        break;
                    default:
                        _eState.Text = GetString("Unknown");
                        _eState.BackColor = Color.Yellow;
                        break;
                }

                _bReboot.Enabled = (OnlineState) state == OnlineState.Online && IsEditAllowed;
            });
        }

        // SB
        private string GetTranslatedActualCRState(bool isUsedInDoorEnvironment, CardReaderSceneType crCommand)
        {
            if (isUsedInDoorEnvironment)
            {
                return GetString("CardReaderCommands_" + crCommand);
            }
            else
            {
                switch (crCommand)
                {
                    case CardReaderSceneType.Unknown:
                        return string.Empty;
                    case CardReaderSceneType.OutOfOrder:
                        return GetString("CardReaderCommands_" + crCommand);
                    default:
                        return GetString("CardReaderCommands_AlarmPanelState");
                }
            }
        }

        private void ChangeCRCommand(Guid cardReader, byte command)
        {
            this.BeginInvokeInUI(() =>
            {
                if (cardReader == _editingObject.IdCardReader)
                {
                    // SB
                    var isUsedInDoorEnvironment = Plugin.MainServerProvider.CardReaders.IsUsedInDoorEnvironment(_editingObject);
                    var cardReaderCommand = (CardReaderSceneType)command;

                    _eActualState.Text = GetTranslatedActualCRState(isUsedInDoorEnvironment, cardReaderCommand);
                }
            });
        }

        private void _bRefresh_Click(object sender, EventArgs e)
        {
            SafeThread.StartThread(SetInformation);
        }

        private void _itbCrParentObject_DoubleClick(object sender, EventArgs e)
        {
            if (_editingObject.CCU != null)
            {
                NCASCCUsForm.Singleton.OpenEditForm(_editingObject.CCU);
            }
            else if (_editingObject.DCU != null)
            {
                NCASDCUsForm.Singleton.OpenEditForm(_editingObject.DCU);
            }
        }

        private void _itbDoorEnvironment_DoubleClick(object sender, EventArgs e)
        {
            var doorEnvironment = GetCrDoorEnvironment();

            if (doorEnvironment != null)
                NCASDoorEnvironmentsForm.Singleton.OpenEditForm(doorEnvironment);
        }

        private void _eMarterAA_DoubleClick(object sender, EventArgs e)
        {
            if (_implicitAlarmArea != null)
            {
                NCASAlarmAreasForm.Singleton.OpenEditForm(_implicitAlarmArea);
            }
        }

        private void RefreshImplicitAlarmArea()
        {
            _eMarterAA.Text = _implicitAlarmArea != null
                ? _implicitAlarmArea.ToString()
                : string.Empty;
        }

        private void Password_KeyPress(object sender, KeyPressEventArgs e)
        {
            var textBox = sender as TextBox;

            if (textBox == null)
                return;

            if (textBox.Text == DEFAULT_PASSWORD)
            {
                textBox.Text = string.Empty;
            }

            if (!(char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar)))
                e.Handled = true;
        }

        private void SetReferencedBy()
        {
            _tpReferencedBy.Controls.Add(new ReferencedByForm(GetListReferencedObjects, NCASClient.LocalizationHelper,
                ObjectImageList.Singleton.GetAllObjectImages()));
        }

        IList<AOrmObject> GetListReferencedObjects()
        {
            return Plugin.MainServerProvider.CardReaders.
                GetReferencedObjects(_editingObject.IdCardReader, CgpClient.Singleton.GetListLoadedPlugins());
        }

        private void _lbUserFolders_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ObjectsPlacementHandler.UserFolders_MouseDoubleClick(_lbUserFolders, _editingObject);
        }

        private void _tpUserFolders_Enter(object sender, EventArgs e)
        {
            ObjectsPlacementHandler.UserFolders_Enter(_lbUserFolders, _editingObject);
        }

        private void _bRefresh1_Click(object sender, EventArgs e)
        {
            ObjectsPlacementHandler.UserFolders_Enter(_lbUserFolders, _editingObject);
        }

        private void _tbmDailyPlan_ButtonPopupMenuItemClick(ToolStripItem item, int index)
        {
            if (item.Name == "_tsiModify1")
            {
                ModifyTimeZoneDaily();
            }
            else if (item.Name == "_tsiRemove1")
            {
                if (_actSecurityTimeZone != null)
                {
                    SetTimeZone(null);
                }
                else
                {
                    SetDailyPlan(null);
                }
            }
            else if (item.Name == "_tsiCreateSecTimeZone")
            {
                var securityTimeZone = new SecurityTimeZone();
                if (NCASSecurityTimeZonesForm.Singleton.OpenInsertDialg(ref securityTimeZone))
                {
                    SetTimeZone(securityTimeZone);
                }
            }
            else if (item.Name == "_tsiCreateSecDailyPlan")
            {
                var securityDailyPlan = new SecurityDailyPlan();
                if (NCASSecurityDailyPlansForm.Singleton.OpenInsertDialg(ref securityDailyPlan))
                {
                    SetDailyPlan(securityDailyPlan);
                }
            }
        }

        private void _tbmSecurityDailyPlanTimeZoneForEnterToMenu_ButtonPopupMenuItemClick(ToolStripItem item, int index)
        {
            if (item == _tsiModify5)
            {
                ModifySecurityDailyPlanTimeZoneForEnterToMenu();
                return;
            }

            if (item == _tsiRemove5)
            {
                if (_editingObject.SecurityDailyPlanForEnterToMenu != null)
                {
                    SetSecurityDailyPlanForEnterToMenu(null);
                }
                else
                {
                    SetSecurityTimeZoneForEnterToMenu(null);
                }

                return;
            }

            if (item == _tsiCreateSecDailyPlan5)
            {
                var securityDailyPlan = new SecurityDailyPlan();
                if (NCASSecurityDailyPlansForm.Singleton.OpenInsertDialg(ref securityDailyPlan))
                {
                    SetSecurityDailyPlanForEnterToMenu(securityDailyPlan);
                }

                return;
            }

            if (item == _tsiCreateSecTimeZone5)
            {
                var securityTimeZone = new SecurityTimeZone();
                if (NCASSecurityTimeZonesForm.Singleton.OpenInsertDialg(ref securityTimeZone))
                {
                    SetSecurityTimeZoneForEnterToMenu(securityTimeZone);
                }
            }
        }

        private void _tbmObjectOnOff_ButtonPopupMenuItemClick(ToolStripItem item, int index)
        {
            if (item.Name == "_tsiModify2")
            {
                ModifyObject();
            }
            else if (item.Name == "_tsiRemove2")
            {
                SetOnOffObject(null, false);
            }
        }

        private void _bCreate_Click_1(object sender, EventArgs e)
        {
            if (_lastCard != null)
            {
                CardsForm.Singleton.OpenEditForm(_lastCard, AfterEditCard);
            }
            else
            {
                _bCreate.Enabled = false;
                SafeThread.StartThread(DoCreateCard);
            }
        }

        private void AfterEditCard(object obj)
        {
            this.BeginInvokeInUI(() =>
            {
                ShowLastCardInformation(_eLastCard.Text);
            });
        }

        private void DoCreateCard()
        {
            try
            {
                CardsForm.Singleton.InsertWithData(_eLastCard.Text, AfterEditCard);
            }
            catch { }

            DoAfterShowCardForm();
        }

        private void DoAfterShowCardForm()
        {
            this.InvokeInUI(() =>
            {
                _bCreate.Enabled = true;
            });
        }

        private void _cbForcedSL_SelectedIndexChanged(object sender, EventArgs e)
        {
            EditTextChanger(sender, e);
        }

        private void _bApply_Click(object sender, EventArgs e)
        {
            if (Apply_Click())
            {
                SetValuesEdit();
                ResetWasChangedValues();
                _bApply.Enabled = false;
                _alarmAreaSettingsChanged = false;
            }
        }

        private IEnumerable<IModifyObject> GetAvailableOutputs()
        {
            var idCCU = Guid.Empty;
            var ccu = _editingObject.CCU;

            if (ccu == null)
            {
                var dcu = _editingObject.DCU;

                if (dcu != null)
                    ccu = dcu.CCU;
            }

            if (ccu != null)
                idCCU = ccu.IdCCU;

            return idCCU != Guid.Empty
                ? Plugin.MainServerProvider.CardReaders.GetSpecialOutputs(idCCU)
                : null;
        }

        private static bool CheckOutputAvailability(Output output, IEnumerable<IModifyObject> outputsModifyObjects)
        {
            return outputsModifyObjects.Any(item => item.GetId == output.IdOutput);
        }

        private void ModifySpecialOutput(byte type)
        {
            if (CgpClient.Singleton.IsConnectionLost(false))
                return;

            var formModify = 
                new ListboxFormAdd(GetAvailableOutputs(), GetString("ModifyOutput"));

            object outObject;
            formModify.ShowDialog(out outObject);

            if (outObject == null)
                return;

            var output = Plugin.MainServerProvider.Outputs.GetObjectById(((OutputModifyObj)outObject).Id);

            if (output == null)
                return;

            SetSpecialOutput(type, output);
            Plugin.AddToRecentList(output);
        }

        private void SetSpecialOutput(byte type, Output value)
        {
            switch (type)
            {
                case 1:
                    {
                        _outputOffline = value;
                        _tbmOutputOffline.Text = (value == null ? string.Empty : value.ToString());
                        _tbmOutputOffline.TextImage = Plugin.GetImageForAOrmObject(value);
                    }
                    break;
                case 2:
                    {
                        _outputSabotage = value;
                        _tbmOutputSabotage.Text = (value == null ? string.Empty : value.ToString());
                        _tbmOutputSabotage.TextImage = Plugin.GetImageForAOrmObject(value);
                    }
                    break;
            }
        }

        private void RemoveSpecialOutput(byte type)
        {
            SetSpecialOutput(type, null);
        }

        private void _tbmOutputOffline_ButtonPopupMenuItemClick(ToolStripItem item, int index)
        {
            if (item.Name == "_tsiModify3")
            {
                ModifySpecialOutput(1);
            }
            else if (item.Name == "_tsiRemove3")
            {
                RemoveSpecialOutput(1);
            }
        }

        private void _tbmOutputOffline_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void _tbmOutputOffline_DragDrop(object sender, DragEventArgs e)
        {
            var output = e.Data.GetFormats();
            if (output == null) return;
            AddOutputOffline(e.Data.GetData(output[0]));
        }

        private void AddOutputOffline(object newObject)
        {
            try
            {
                var output = newObject as Output;

                if (output != null)
                {
                    if (CheckOutputAvailability(
                        output,
                        GetAvailableOutputs()))
                    {
                        _tbmOutputOffline.Text = newObject.ToString();
                        _tbmOutputOffline.TextImage =
                            Plugin.GetImageForAOrmObject((AOrmObject)newObject);

                        _outputOffline = output;
                        Plugin.AddToRecentList(newObject);
                    }
                    else
                        ErrorNotificationOverControl(
                            _tbmOutputOffline.ImageTextBox,
                            "ErrorOutputAlreadyUsed");
                }
                else
                    ErrorNotificationOverControl(
                        _tbmOutputOffline.ImageTextBox,
                        "ErrorWrongObjectType");
            }
            catch
            { }
        }

        private void _tbmOutputOffline_ImageTextBox_DoubleClick(object sender, EventArgs e)
        {
            if (_outputOffline != null)
                NCASOutputsForm.Singleton.OpenEditForm(_outputOffline);
        }

        private void _tbmOutputSabotage_ButtonPopupMenuItemClick(ToolStripItem item, int index)
        {
            switch (item.Name)
            {
                case "_tsiModify4":

                    ModifySpecialOutput(2);
                    break;

                case "_tsiRemove4":

                    RemoveSpecialOutput(2);
                    break;
            }
        }

        private void _tbmOutputSabotage_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void _tbmOutputSabotage_DragDrop(object sender, DragEventArgs e)
        {
            var output = e.Data.GetFormats();
            if (output == null) return;
            AddOutputSabotage(e.Data.GetData(output[0]));
        }

        private void AddOutputSabotage(object newObject)
        {
            try
            {
                var output = newObject as Output;
                if (output != null)
                {
                    if (CheckOutputAvailability(
                        output,
                        GetAvailableOutputs()))
                    {
                        _tbmOutputSabotage.Text = newObject.ToString();
                        _tbmOutputSabotage.TextImage =
                            Plugin.GetImageForAOrmObject((AOrmObject)newObject);

                        _outputSabotage = output;
                        Plugin.AddToRecentList(newObject);
                    }
                    else
                        ErrorNotificationOverControl(
                            _tbmOutputSabotage.ImageTextBox,
                            "ErrorOutputAlreadyUsed");
                }
                else
                    ErrorNotificationOverControl(
                        _tbmOutputSabotage.ImageTextBox,
                        "ErrorWrongObjectType");
            }
            catch
            { }
        }

        private void _tbmOutputSabotage_ImageTextBox_DoubleClick(object sender, EventArgs e)
        {
            if (_outputSabotage != null)
            {
                NCASOutputsForm.Singleton.OpenEditForm(_outputSabotage);
            }
        }

        #region AlarmInstructions

        protected override bool LocalAlarmInstructionsView()
        {
            return
                CgpClient.Singleton.MainServerProvider.HasAccess(
                    NCASAccess.GetAccess(AccessNCAS.CardReadersLocalAlarmInstructionsView));
        }

        protected override bool LocalAlarmInstructionsAdmin()
        {
            return
                CgpClient.Singleton.MainServerProvider.HasAccess(
                    NCASAccess.GetAccess(AccessNCAS.CardReadersLocalAlarmInstructionsAdmin));
        }

        protected override string GetLocalAlarmInstruction()
        {
            return _editingObject.LocalAlarmInstruction;
        }

        #endregion

        private void _bReboot_Click(object sender, EventArgs e)
        {
            if (CgpClient.Singleton.IsConnectionLost(true)) return;

            if (Dialog.Question(GetString("QuestionResetCardReader")))
                SafeThread.StartThread(ResetCardReader);
        }

        private void ResetCardReader()
        {
            Plugin.MainServerProvider.CardReaders.Reset(_editingObject.IdCardReader);
        }

        private void _itbPerson_DoubleClick(object sender, EventArgs e)
        {
            if (_lastCardPerson != null)
            {
                PersonsForm.Singleton.OpenEditForm(_lastCardPerson);
            }
        }

        private void _tbObjectRuleFK1_ButtonPopupMenuItemClick(ToolStripItem item, int index)
        {
            if (item.Text == "Modify")
            {
                ModifyObjectRule(_tbObjectRuleFK1, _functionKey1);
            }
            else
            {
                _tbObjectRuleFK1.Text = string.Empty;
                _tbObjectRuleFK1.TextImage = null;
                _functionKey1.IdOutput = Guid.Empty;
            }
        }

        private void _tbObjectRuleFK2_ButtonPopupMenuItemClick(ToolStripItem item, int index)
        {
            if (item.Text == "Modify")
            {
                ModifyObjectRule(_tbObjectRuleFK2, _functionKey2);
            }
            else
            {
                _tbObjectRuleFK2.Text = string.Empty;
                _tbObjectRuleFK2.TextImage = null;
                _functionKey2.IdOutput = Guid.Empty;
            }
        }

        private void _tbTimeZoneFK1_ButtonPopupMenuItemClick(ToolStripItem item, int index)
        {
            if (item.Text == "Modify")
            {
                ModifyTimeZoneOrDailyPlan(_tbTimeZoneFK1, _functionKey1);
            }
            else
            {
                _functionKey1.IdTimeZoneOrDailyPlan = Guid.Empty;
                _tbTimeZoneFK1.Text = string.Empty;
                _tbTimeZoneFK1.TextImage = null;
            }
        }

        private void _tbTimeZoneFK2_ButtonPopupMenuItemClick(ToolStripItem item, int index)
        {
            if (item.Text == "Modify")
            {
                ModifyTimeZoneOrDailyPlan(_tbTimeZoneFK2, _functionKey2);
            }
            else
            {
                _functionKey2.IdTimeZoneOrDailyPlan = Guid.Empty;
                _tbTimeZoneFK2.Text = string.Empty;
                _tbTimeZoneFK2.TextImage = null;
            }
        }

        private void ModifyObjectRule(TextBoxMenu sender, FunctionKey FK)
        {
            if (CgpClient.Singleton.IsConnectionLost(true) || FK == null)
                return;

            try
            {
                var ccuGuid = GetParentCCUGuid();

                Exception error;

                var listOutputs = ccuGuid != Guid.Empty
                    ? Plugin.MainServerProvider.Outputs.ListModifyObjectsFromCCU(ccuGuid, out error)
                    : Plugin.MainServerProvider.Outputs.ListModifyObjects(out error);

                if (error != null)
                    throw error;


                var formAdd =
                    new ListboxFormAdd(
                        listOutputs,
                        GetString("NCASAlaramAreaEditFormListObjectForAutomaticActivationText"));

                object outObject;
                formAdd.ShowDialog(out outObject);

                if (outObject == null)
                    return;

                _actOnOffObject = null;

                var outputModifyObj = outObject as OutputModifyObj;

                if (outputModifyObj == null)
                    return;

                var output =
                    Plugin.MainServerProvider.Outputs.GetObjectById(outputModifyObj.Id);

                Plugin.AddToRecentList(_actOnOffObject);

                sender.Text = output.Name;
                sender.TextImage = Plugin.GetImageForAOrmObject(output);

                FK.IdOutput = output.IdOutput;
            }
            catch (Exception ex)
            {
                Dialog.Error(ex);
            }
        }

        private void ModifyTimeZoneOrDailyPlan(TextBoxMenu sender, FunctionKey FK)
        {
            if (CgpClient.Singleton.IsConnectionLost(true) || FK == null)
                return;

            try
            {
                var listObjects = new List<IModifyObject>();

                Exception error;

                var listTz =
                    CgpClient.Singleton.MainServerProvider.TimeZones
                        .ListModifyObjects(out error);

                if (error != null)
                    throw error;

                listObjects.AddRange(listTz);

                var listDailyPlans =
                    CgpClient.Singleton.MainServerProvider.DailyPlans
                    .ListModifyObjects(out error);

                if (error != null)
                    throw error;

                DbsSupport.TranslateDailyPlans(listDailyPlans);
                listObjects.AddRange(listDailyPlans);

                var formAdd =
                    new ListboxFormAdd(
                        listObjects,
                        GetString("NCASAlaramAreaEditFormListObjectForAutomaticActivationText"));

                IModifyObject outObject;
                formAdd.ShowDialog(out outObject);

                if (outObject == null)
                    return;

                switch (outObject.GetOrmObjectType)
                {
                    case ObjectType.TimeZone:
                        {
                            var timeZone = CgpClient.Singleton.MainServerProvider.TimeZones.GetObjectById(outObject.GetId);
                            Plugin.AddToRecentList(_actOnOffObject);
                            sender.Text = timeZone.Name;
                            sender.TextImage = Plugin.GetImageForAOrmObject(timeZone);
                            FK.IdTimeZoneOrDailyPlan = timeZone.IdTimeZone;
                            FK.IsUsedTimeZone = true;
                            break;
                        }
                    case ObjectType.DailyPlan:
                        {
                            var dailyPlan = CgpClient.Singleton.MainServerProvider.DailyPlans.GetObjectById(outObject.GetId);
                            Plugin.AddToRecentList(_actOnOffObject);
                            sender.Text = dailyPlan.DailyPlanName;
                            sender.TextImage = Plugin.GetImageForAOrmObject(dailyPlan);
                            FK.IdTimeZoneOrDailyPlan = dailyPlan.IdDailyPlan;
                            FK.IsUsedTimeZone = false;
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                Dialog.Error(ex);
            }
        }

        private void OpenOutputEditForm(FunctionKey functionKey)
        {
            if (functionKey == null)
                return;

            if (functionKey.IdOutput == Guid.Empty)
                return;

            Output output = Plugin.MainServerProvider.Outputs.GetObjectById(functionKey.IdOutput);

            if (output != null)
                NCASOutputsForm.Singleton.OpenEditForm(output);
        }

        private void _tbObjectRuleFK1_ImageTextBox_DoubleClick(object sender, EventArgs e)
        {
            OpenOutputEditForm(_functionKey1);
        }

        private void _tbObjectRuleFK2_ImageTextBox_DoubleClick(object sender, EventArgs e)
        {
            OpenOutputEditForm(_functionKey2);
        }

        private static void OpenTimeZoneOrDailyPlanForm(FunctionKey functionKey)
        {
            if (functionKey == null)
                return;

            if (functionKey.IdTimeZoneOrDailyPlan == Guid.Empty)
                return;

            if (functionKey.IsUsedTimeZone)
            {
                TimeZone timeZone =
                    CgpClient.Singleton.MainServerProvider.TimeZones.GetObjectById(
                        functionKey.IdTimeZoneOrDailyPlan);

                if (timeZone != null)
                    TimeZonesForm.Singleton.OpenEditForm(timeZone);
            }
            else
            {
                DailyPlan dailyPlan =
                    CgpClient.Singleton.MainServerProvider.DailyPlans.GetObjectById(
                        functionKey.IdTimeZoneOrDailyPlan);

                if (dailyPlan != null)
                    DailyPlansForm.Singleton.OpenEditForm(dailyPlan);
            }
        }

        private void _tbTimeZoneFK1_ImageTextBox_DoubleClick(object sender, EventArgs e)
        {
            OpenTimeZoneOrDailyPlanForm(_functionKey1);
        }

        private void _tbTimeZoneFK2_ImageTextBox_DoubleClick(object sender, EventArgs e)
        {
            OpenTimeZoneOrDailyPlanForm(_functionKey2);
        }

        private void _cbOpenAllAlarmSettings_CheckedChanged(object sender, EventArgs e)
        {
            OpenAllCheckStateChanged(
                _accordionAlarmSettings,
                _cbOpenAllAlarmSettings.CheckState);

            _accordionAlarmSettings.Focus();
        }

        private static void OpenAllCheckStateChanged(Accordion accordion, CheckState checkState)
        {
            if (checkState == CheckState.Indeterminate)
                return;

            if (checkState == CheckState.Checked)
            {
                accordion.OpenAll();

                return;
            }

            accordion.CloseAll();
        }

        private void UnblockAsync()
        {
            var idCcu = GetIdParentCcu();

            if (idCcu != null)
                Plugin.MainServerProvider.CardReaders.Unblock(
                    idCcu.Value,
                    _editingObject.IdCardReader);

            BeginInvoke(new Action(() => _bUnblock.Enabled = true));
        }

        private void _bUnblock_Click(object sender, EventArgs e)
        {
            _bUnblock.Enabled = false;
            SafeThread.StartThread(UnblockAsync);
        }
    }

    public class AlaramAreaCardReadSettings
    {
        private readonly AlarmArea _alarmArea;
        private readonly Panel _panel;
        private readonly Label _lName;
        private readonly CheckBox _cbSet;
        private readonly CheckBox _cbUnset;
        private readonly CheckBox _cbUnconditionalSet;
        private readonly CheckBox _cbPermanentlyUnlock;
        private readonly CheckBox _cbEnableEventlog;
        private readonly Action<AlarmArea, bool> _changeImplictAlarmArea;

        public AlarmArea AlarmArea { get { return _alarmArea; } }

        public AlaramAreaCardReadSettings(
            AlarmArea alarmArea, 
            Panel panel, 
            Label lName, 
            CheckBox cbSet, 
            CheckBox cbUnset, 
            CheckBox cbUnconditionalSet, 
            CheckBox cbPermanentlyUnlocked, 
            CheckBox cbEnableEventlog, 
            Action<AlarmArea, bool> ChangeImplictAlarmArea)
        {
            _alarmArea = alarmArea;
            _panel = panel;
            _lName = lName;
            _cbSet = cbSet;
            _cbSet.CheckedChanged += CheckedChanged;
            _cbUnset = cbUnset;
            _cbUnset.CheckedChanged += CheckedChanged;
            _cbEnableEventlog = cbEnableEventlog;
            _cbEnableEventlog.CheckedChanged += CheckedChanged;
            _cbUnconditionalSet = cbUnconditionalSet;
            _cbUnconditionalSet.CheckedChanged += CheckedChanged;
            _cbPermanentlyUnlock = cbPermanentlyUnlocked;
            _cbPermanentlyUnlock.CheckedChanged += CheckedChanged;
            _changeImplictAlarmArea = ChangeImplictAlarmArea;
        }

        void CheckedChanged(object sender, EventArgs e)
        {
            ChangeImplicitAlarArea(
                AlarmArea,
                IsSet() && _cbPermanentlyUnlock.Checked);

            if (sender != _cbPermanentlyUnlock)
            {
                if (IsSet())
                {
                    if (_cbPermanentlyUnlock.Enabled == false && _cbPermanentlyUnlock.Checked)
                        _cbPermanentlyUnlock.Checked = !true;
                }
                else
                {
                    if (_cbPermanentlyUnlock.Checked != true)
                        _cbPermanentlyUnlock.Checked = false;
                }
            }
            //EnableDisablePermanentlyUnlock();
        }

        private void ChangeImplicitAlarArea(AlarmArea alarmArea, bool setImplicitAlarmArea)
        {
            if (_changeImplictAlarmArea != null)
                _changeImplictAlarmArea(alarmArea, setImplicitAlarmArea);
        }

        public void Set(bool alarmAreaSet, bool alarmAreaUnset, bool alarmAreaACK, bool alarmPermanentlyUnlock, bool enableEventlog)
        {
            _cbSet.Checked = alarmAreaSet;
            _cbUnset.Checked = alarmAreaUnset;
            _cbUnconditionalSet.Checked = alarmAreaACK;
            _cbPermanentlyUnlock.Checked = !alarmPermanentlyUnlock;
            _cbEnableEventlog.Checked = enableEventlog;
        }

        public string GetText()
        {
            if (_lName != null)
                return _lName.Text;
            return string.Empty;
        }

        public void SetTop(int top)
        {
            if (_panel != null)
                _panel.Top = top;
        }

        public void SetVisible(bool visible)
        {
            if (_panel != null)
                _panel.Visible = visible;
        }

        public bool GetAlarmAreaSet()
        {
            if (_cbSet != null)
                return _cbSet.Checked;

            return false;
        }

        public bool GetAlarmAreaUnset()
        {
            if (_cbUnset != null)
                return _cbUnset.Checked;

            return false;
        }

        public bool GetAlarmAreaUnconditionalSet()
        {
            if (_cbUnconditionalSet != null)
                return _cbUnconditionalSet.Checked;

            return false;
        }

        public bool GetAlarmAreaPermanentlyUnlock()
        {
            if (_cbPermanentlyUnlock != null)
                return !_cbPermanentlyUnlock.Checked;

            return false;
        }

        public bool GetAlarmAreaCardReaderEnbaleEvnetlog()
        {
            if (_cbEnableEventlog != null)
                return _cbEnableEventlog.Checked;

            return false;
        }

        public bool IsSet()
        {
            return (_cbSet != null && _cbSet.Checked) ||
                (_cbUnset != null && _cbUnset.Checked) ||
                (_cbUnconditionalSet != null && _cbUnconditionalSet.Checked) ||
                (_cbPermanentlyUnlock != null && _cbPermanentlyUnlock.Checked);
        }

        public void EnablePermanentlyUnlock()
        {
            _cbPermanentlyUnlock.Enabled = true;
        }

        public void DisablePermanentlyUnlock()
        {
            //if ((_cbSet != null && _cbSet.Checked) ||
            //    (_cbUnset != null && _cbUnset.Checked) ||
            //    (_cbUnconditionalSet != null && _cbUnconditionalSet.Checked))
            //{
            //    _cbPermanentlyUnlock.Enabled = false;
            //}
            //else
            //{
            //    _cbPermanentlyUnlock.Enabled = true;
            //}
            _cbPermanentlyUnlock.Enabled = false;
        }
    }

    public class SecurityLevelForFunctionKeyItem
    {
        public SecurityLevelForSpecialKey SecurityLevel { get; set; }
        public LocalizationHelper LocalizationHelper { get; set; }

        public SecurityLevelForFunctionKeyItem(
            SecurityLevelForSpecialKey securityLevel,
            LocalizationHelper localizationHelper)
        {
            SecurityLevel = securityLevel;
            LocalizationHelper = localizationHelper;
        }

        public override string ToString()
        {
            return LocalizationHelper.GetString(SecurityLevel.ToString().ToLower());
        }
    }

    public class SymbolForFunctionKeyItem
    {
        public FunctionKeySymbol Symbol { get; set; }

        public SymbolForFunctionKeyItem(FunctionKeySymbol Symbol)
        {
            this.Symbol = Symbol;
        }

        public override string ToString()
        {
            return Symbol.ToString();
        }
    }

    public class ItemSlForEnterToMenu
    {
        private readonly SecurityLevel? _slForEntrToMenu;
        private readonly LocalizationHelper _localizationHelper;

        public SecurityLevel? SlForEntetToMenu { get { return _slForEntrToMenu; } }

        public ItemSlForEnterToMenu(SecurityLevel? slForEnterToMenu, LocalizationHelper localizationHelper)
        {
            _slForEntrToMenu = slForEnterToMenu;
            _localizationHelper = localizationHelper;
        }

        public override string ToString()
        {
            if (_slForEntrToMenu.HasValue)
                return _localizationHelper.GetString(
                    string.Format("SecurityLevelStates_{0}",
                        _slForEntrToMenu));

            return _localizationHelper.GetString("InheritFromAlarmSettings");
        }

        public static ICollection<ItemSlForEnterToMenu> GetList(bool allowInheriting, LocalizationHelper localizationHelper)
        {
            return new LinkedList<ItemSlForEnterToMenu>(
                (allowInheriting
                    ? Enumerable.Repeat(
                        new ItemSlForEnterToMenu(null, localizationHelper),
                        1)
                    : Enumerable.Empty<ItemSlForEnterToMenu>())
                    .Concat(
                        EnumHelper.ListAllValuesWithProcessing<SecurityLevel, ItemSlForEnterToMenu>(
                            value =>
                            {
                                if (value == SecurityLevel.Unlocked
                                    || value == SecurityLevel.Locked)
                                {
                                    return null;
                                }

                                return new ItemSlForEnterToMenu(value, localizationHelper);
                            })));
        }

        public static ICollection<ItemSlForEnterToMenu> GetListSmallCr(bool allowInheriting, LocalizationHelper localizationHelper)
        {
            return new LinkedList<ItemSlForEnterToMenu>(
                (allowInheriting
                    ? Enumerable.Repeat(
                        new ItemSlForEnterToMenu(null, localizationHelper),
                        1)
                    : Enumerable.Empty<ItemSlForEnterToMenu>())
                    .Concat(
                        EnumHelper.ListAllValuesWithProcessing<SecurityLevel, ItemSlForEnterToMenu>(
                            value =>
                            {
                                if (value == SecurityLevel.Unlocked
                                    || value == SecurityLevel.Locked)
                                {
                                    return null;
                                }

                                if (value.ToString().ToUpper().Contains("PIN")
                                    || value.ToString().ToUpper().Contains("GIN"))
                                {
                                    return null;
                                }

                                return new ItemSlForEnterToMenu(value, localizationHelper);
                            })));
        }
    }
}
