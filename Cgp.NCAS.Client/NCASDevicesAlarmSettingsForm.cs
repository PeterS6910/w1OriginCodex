using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using Contal.Cgp.Client.PluginSupport;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.Client;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Globals;
using Contal.IwQuick;
using Contal.IwQuick.Crypto;
using Contal.IwQuick.Data;
using Contal.IwQuick.Parsing;
using Contal.IwQuick.PlatformPC.UI.Accordion;
using Contal.IwQuick.Sys;
using Contal.IwQuick.Threads;
using Contal.IwQuick.UI;
using Contal.Cgp.NCAS.RemotingCommon;
using System.Diagnostics;
using System.IO;
using Contal.Cgp.Server.DB;
using System.Web.UI.WebControls.WebParts;

namespace Contal.Cgp.NCAS.Client
{
    public sealed partial class NCASDevicesAlarmSettingsForm :

#if DESIGNER
        Form
#else
        ACgpPluginTableForm<NCASClient, DevicesAlarmSetting, DevicesAlarmSetting>
#endif
    {
        private const string DEFAULT_PASSWORD = "********";

        private readonly ControlAlarmTypeSettings _catsDcuOffline;
        private readonly ControlModifyAlarmArcs _cmaaDcuOffline;
        private readonly ControlModifyAlarmArcs _cmaaDcuOfflineDueCcuOffline;
        private readonly ControlAlarmTypeSettings _catsDcuTamperSabotage;
        private readonly ControlModifyAlarmArcs _cmaaDcuTamperSabotage;
        private readonly ControlModifyAlarmArcs _cmaaDcuOutdatedFirmware;

        private readonly ControlAlarmTypeSettings _catsDsmIntrusion;
        private readonly ControlModifyAlarmArcs _cmaaDsmIntrusion;
        private readonly ControlAlarmTypeSettings _catsDsmDoorAjar;
        private readonly ControlModifyAlarmArcs _cmaaDsmDoorAjar;
        private readonly ControlAlarmTypeSettings _catsDsmSabotage;
        private readonly ControlModifyAlarmArcs _cmaaDsmSabotage;

        private readonly ControlAlarmTypeSettings _catsCrOffline;
        private readonly ControlModifyAlarmArcs _cmaaCrOffline;
        private readonly ControlModifyAlarmArcs _cmaaCrOfflineDueCcuOffline;
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
        private readonly ControlAlarmTypeSettings _catsCrInvalidPinRetriesLimitReached;
        private readonly ControlModifyAlarmArcs _cmaaCrInvalidPinRetriesLimitReached;
        private readonly ControlAlarmTypeSettings _catsCrInvalidGinRetriesLimitReached;
        private readonly ControlModifyAlarmArcs _cmaaCrInvalidGinRetriesLimitReached;

        private readonly ControlAlarmTypeSettings _catsCcuOffline;
        private readonly ControlModifyAlarmArcs _cmaaCcuOffline;
        private readonly ControlAlarmTypeSettings _catsCcuUpsBatteryFault;
        private readonly ControlModifyAlarmArcs _cmaaCcuUpsBatteryFault;
        private readonly ControlAlarmTypeSettings _catsCcuTamperSabotage;
        private readonly ControlModifyAlarmArcs _cmaaCcuTamperSabotage;
        private readonly ControlAlarmTypeSettings _catsCcuUnconfigured;
        private readonly ControlModifyAlarmArcs _cmaaCcuUnconfigured;
        private readonly ControlAlarmTypeSettings _catsCcuUpsOutputFuse;
        private readonly ControlModifyAlarmArcs _cmaaCcuUpsOutputFuse;
        private readonly ControlAlarmTypeSettings _catsCcuClockUnsynchronized;
        private readonly ControlModifyAlarmArcs _cmaaCcuClockUnsynchronized;
        private readonly ControlAlarmTypeSettings _catsCcuPrimaryPowerMissing;
        private readonly ControlModifyAlarmArcs _cmaaCcuPrimaryPowerMissing;
        private readonly ControlAlarmTypeSettings _catsCcuBatteryIsLow;
        private readonly ControlModifyAlarmArcs _cmaaCcuBatteryIsLow;
        private readonly ControlAlarmTypeSettings _catsCcuFuseOnExtensionBoard;
        private readonly ControlModifyAlarmArcs _cmaaCcuFuseOnExtensionBoard;
        private readonly ControlAlarmTypeSettings _catsCcuUpsTamper;
        private readonly ControlModifyAlarmArcs _cmaaCcuUpsTamper;
        private readonly ControlAlarmTypeSettings _catsCcuUpsOvertemperature;
        private readonly ControlModifyAlarmArcs _cmaaCcuUpsOvertemperature;
        private readonly ControlAlarmTypeSettings _catsCcuUpsBatteryFuse;
        private readonly ControlModifyAlarmArcs _cmaaCcuUpsBatteryFuse;
        private readonly ControlModifyAlarmArcs _cmaaCcuOutdatedFirmware;
        private readonly ControlModifyAlarmArcs _cmaaICcuPortAlreadyUsed;
        private readonly ControlModifyAlarmArcs _cmaaICcuSendingOfObjectStateFailed;
        private readonly ControlModifyAlarmArcs _cmaaCcuSdCardNotFound;
        private readonly ControlModifyAlarmArcs _cmaaCcuFilesystemProblem;
        private readonly ControlModifyAlarmArcs _cmaaCcuHighMemoryLoad;
        private readonly ControlModifyAlarmArcs _cmaaCcuCoprocessorFailure;
        private readonly ControlModifyAlarmArcs _cmaaCcuDataChannelDistrupted;
        private readonly ControlAlarmTypeSettings _catsCcuCatUnreachable;
        private readonly ControlAlarmTypeSettings _catsCcuTransferToArcTimedOut;

        private readonly Dictionary<AlarmType, Action> _openAndScrollToControlByAlarmType;

        private readonly ControlModifyAlarmArcs _cmaaAlarmReceptionCenterSettings;
        private readonly ControlAlarmTypeSettings _catsAlarmAreaAlarm;
        private readonly ControlAlarmTypeSettings _catsAlarmAreaSetByOnOffObjectFailed;
        private readonly ControlAlarmTypeSettings _catsSensorAlarm;
        private readonly ControlAlarmTypeSettings _catsSensorTamperAlarm;

        private SecurityDailyPlan _actSecurityDailyPlanForEnterToMenu;
        private SecurityTimeZone _actSecurityTimeZoneForEnterToMenu;
        private bool _settingDefaultPassword;


        public NCASDevicesAlarmSettingsForm()
            : base(
                CgpClientMainForm.Singleton,
                NCASClient.Singleton,
                NCASClient.LocalizationHelper)
        {
            FormImage = ResourceGlobal.AlarmSettingsNew48;

            _openAndScrollToControlByAlarmType = new Dictionary<AlarmType, Action>
            {
                {
                    AlarmType.CCU_Offline,
                    () =>
                    {
                        _tcDevicesAlarmSetting.SelectedTab = _tpCcuAlarms;
                        _accordionCcuAlarms.OpenAndScrollToControl(_catsCcuOffline);
                    }
                },
                {
                    AlarmType.CCU_Unconfigured,
                    () =>
                    {
                        _tcDevicesAlarmSetting.SelectedTab = _tpCcuAlarms;
                        _accordionCcuAlarms.OpenAndScrollToControl(_catsCcuUnconfigured);
                    }
                },
                {
                    AlarmType.CCU_ClockUnsynchronized,
                    () =>
                    {
                        _tcDevicesAlarmSetting.SelectedTab = _tpCcuAlarms;
                        _accordionCcuAlarms.OpenAndScrollToControl(_catsCcuClockUnsynchronized);
                    }
                },
                {
                    AlarmType.CCU_TamperSabotage,
                    () =>
                    {
                        _tcDevicesAlarmSetting.SelectedTab = _tpCcuAlarms;
                        _accordionCcuAlarms.OpenAndScrollToControl(_catsCcuTamperSabotage);
                    }
                },
                {
                    AlarmType.CCU_PrimaryPowerMissing,
                    () =>
                    {
                        _tcDevicesAlarmSetting.SelectedTab = _tpCcuAlarms;
                        _accordionCcuAlarms.OpenAndScrollToControl(_catsCcuPrimaryPowerMissing);
                    }
                },
                {
                    AlarmType.CCU_BatteryLow,
                    () =>
                    {
                        _tcDevicesAlarmSetting.SelectedTab = _tpCcuAlarms;
                        _accordionCcuAlarms.OpenAndScrollToControl(_catsCcuBatteryIsLow);
                    }
                },
                {
                    AlarmType.Ccu_Ups_OutputFuse,
                    () =>
                    {
                        _tcDevicesAlarmSetting.SelectedTab = _tpCcuAlarms;
                        _accordionCcuAlarms.OpenAndScrollToControl(_catsCcuUpsOutputFuse);
                    }
                },
                {
                    AlarmType.Ccu_Ups_BatteryFault,
                    () =>
                    {
                        _tcDevicesAlarmSetting.SelectedTab = _tpCcuAlarms;
                        _accordionCcuAlarms.OpenAndScrollToControl(_catsCcuUpsBatteryFault);
                    }
                },
                {
                    AlarmType.Ccu_Ups_BatteryFuse,
                    () =>
                    {
                        _tcDevicesAlarmSetting.SelectedTab = _tpCcuAlarms;
                        _accordionCcuAlarms.OpenAndScrollToControl(_catsCcuUpsBatteryFuse);
                    }
                },
                {
                    AlarmType.Ccu_Ups_Overtemperature,
                    () =>
                    {
                        _tcDevicesAlarmSetting.SelectedTab = _tpCcuAlarms;
                        _accordionCcuAlarms.OpenAndScrollToControl(_catsCcuUpsOvertemperature);
                    }
                },
                {
                    AlarmType.Ccu_Ups_TamperSabotage,
                    () =>
                    {
                        _tcDevicesAlarmSetting.SelectedTab = _tpCcuAlarms;
                        _accordionCcuAlarms.OpenAndScrollToControl(_catsCcuUpsTamper);
                    }
                },
                {
                    AlarmType.CCU_ExtFuse,
                    () =>
                    {
                        _tcDevicesAlarmSetting.SelectedTab = _tpCcuAlarms;
                        _accordionCcuAlarms.OpenAndScrollToControl(_catsCcuFuseOnExtensionBoard);
                    }
                },
                {
                    AlarmType.CCU_OutdatedFirmware,
                    () =>
                    {
                        _tcDevicesAlarmSetting.SelectedTab = _tpCcuAlarms;
                        _accordionCcuAlarms.OpenAndScrollToControl(_cmaaCcuOutdatedFirmware);
                    }
                },
                {
                    AlarmType.CCU_DataChannelDistrupted,
                    () =>
                    {
                        _tcDevicesAlarmSetting.SelectedTab = _tpCcuAlarms;
                        _accordionCcuAlarms.OpenAndScrollToControl(_cmaaCcuDataChannelDistrupted);
                    }
                },
                {
                    AlarmType.CCU_CoprocessorFailure,
                    () =>
                    {
                        _tcDevicesAlarmSetting.SelectedTab = _tpCcuAlarms;
                        _accordionCcuAlarms.OpenAndScrollToControl(_cmaaCcuCoprocessorFailure);
                    }
                },
                {
                    AlarmType.CCU_HighMemoryLoad,
                    () =>
                    {
                        _tcDevicesAlarmSetting.SelectedTab = _tpCcuAlarms;
                        _accordionCcuAlarms.OpenAndScrollToControl(_cmaaCcuHighMemoryLoad);
                    }
                },
                {
                    AlarmType.CCU_FilesystemProblem,
                    () =>
                    {
                        _tcDevicesAlarmSetting.SelectedTab = _tpCcuAlarms;
                        _accordionCcuAlarms.OpenAndScrollToControl(_cmaaCcuFilesystemProblem);
                    }
                },
                {
                    AlarmType.CCU_SdCardNotFound,
                    () =>
                    {
                        _tcDevicesAlarmSetting.SelectedTab = _tpCcuAlarms;
                        _accordionCcuAlarms.OpenAndScrollToControl(_cmaaCcuSdCardNotFound);
                    }
                },
                {
                    AlarmType.ICCU_SendingOfObjectStateFailed,
                    () =>
                    {
                        _tcDevicesAlarmSetting.SelectedTab = _tpCcuAlarms;
                        _accordionCcuAlarms.OpenAndScrollToControl(_cmaaICcuSendingOfObjectStateFailed);
                    }
                },
                {
                    AlarmType.ICCU_PortAlreadyUsed,
                    () =>
                    {
                        _tcDevicesAlarmSetting.SelectedTab = _tpCcuAlarms;
                        _accordionCcuAlarms.OpenAndScrollToControl(_cmaaICcuPortAlreadyUsed);
                    }
                },
                {
                    AlarmType.Ccu_CatUnreachable,
                    () =>
                    {
                        _tcDevicesAlarmSetting.SelectedTab = _tpCcuAlarms;
                        _accordionCcuAlarms.OpenAndScrollToControl(_catsCcuCatUnreachable);
                    }
                },
                {
                    AlarmType.Ccu_TransferToArcTimedOut,
                    () =>
                    {
                        _tcDevicesAlarmSetting.SelectedTab = _tpCcuAlarms;
                        _accordionCcuAlarms.OpenAndScrollToControl(_catsCcuTransferToArcTimedOut);
                    }
                },
                {
                    AlarmType.DCU_Offline,
                    () =>
                    {
                        _tcDevicesAlarmSetting.SelectedTab = _tpDcuAlarms;
                        _accordionDcuAlarms.OpenAndScrollToControl(_catsDcuOffline);
                    }
                },
                {
                    AlarmType.DCU_Offline_Due_CCU_Offline,
                    () =>
                    {
                        _tcDevicesAlarmSetting.SelectedTab = _tpDcuAlarms;
                        _accordionDcuAlarms.OpenAndScrollToControl(_cmaaDcuOfflineDueCcuOffline);
                    }
                },
                {
                    AlarmType.DCU_TamperSabotage,
                    () =>
                    {
                        _tcDevicesAlarmSetting.SelectedTab = _tpDcuAlarms;
                        _accordionDcuAlarms.OpenAndScrollToControl(_catsDcuTamperSabotage);
                    }
                },
                {
                    AlarmType.DCU_OutdatedFirmware,
                    () =>
                    {
                        _tcDevicesAlarmSetting.SelectedTab = _tpDcuAlarms;
                        _accordionDcuAlarms.OpenAndScrollToControl(_cmaaDcuOutdatedFirmware);
                    }
                },
                {
                    AlarmType.DoorEnvironment_DoorAjar,
                    () =>
                    {
                        _tcDevicesAlarmSetting.SelectedTab = _tpDoorEnvironmentAlarms;
                        _accordionDsmAlarms.OpenAndScrollToControl(_catsDsmDoorAjar);
                    }
                },
                {
                    AlarmType.DoorEnvironment_Intrusion,
                    () =>
                    {
                        _tcDevicesAlarmSetting.SelectedTab = _tpDoorEnvironmentAlarms;
                        _accordionDsmAlarms.OpenAndScrollToControl(_catsDsmIntrusion);
                    }
                },
                {
                    AlarmType.DoorEnvironment_Sabotage,
                    () =>
                    {
                        _tcDevicesAlarmSetting.SelectedTab = _tpDoorEnvironmentAlarms;
                        _accordionDsmAlarms.OpenAndScrollToControl(_catsDsmSabotage);
                    }
                },
                {
                    AlarmType.CardReader_Offline,
                    () =>
                    {
                        _tcDevicesAlarmSetting.SelectedTab = _tpCrAlarms;
                        _accordionCrAlarms.OpenAndScrollToControl(_catsCrOffline);
                    }
                },
                {
                    AlarmType.CardReader_Offline_Due_CCU_Offline,
                    () =>
                    {
                        _tcDevicesAlarmSetting.SelectedTab = _tpCrAlarms;
                        _accordionCrAlarms.OpenAndScrollToControl(_cmaaCrOfflineDueCcuOffline);
                    }
                },
                {
                    AlarmType.CardReader_TamperSabotage,
                    () =>
                    {
                        _tcDevicesAlarmSetting.SelectedTab = _tpCrAlarms;
                        _accordionCrAlarms.OpenAndScrollToControl(_catsCrTamperSabotage);
                    }
                },
                {
                    AlarmType.CardReader_AccessDenied,
                    () =>
                    {
                        _tcDevicesAlarmSetting.SelectedTab = _tpCrAlarms;
                        _accordionCrAlarms.OpenAndScrollToControl(_catsCrAccessDenied);
                    }
                },
                {
                    AlarmType.CardReader_UnknownCard,
                    () =>
                    {
                        _tcDevicesAlarmSetting.SelectedTab = _tpCrAlarms;
                        _accordionCrAlarms.OpenAndScrollToControl(_catsCrUnknownCard);
                    }
                },
                {
                    AlarmType.CardReader_CardBlockedOrInactive,
                    () =>
                    {
                        _tcDevicesAlarmSetting.SelectedTab = _tpCrAlarms;
                        _accordionCrAlarms.OpenAndScrollToControl(_catsCrCardBlockedOrInactive);
                    }
                },
                {
                    AlarmType.CardReader_InvalidPIN,
                    () =>
                    {
                        _tcDevicesAlarmSetting.SelectedTab = _tpCrAlarms;
                        _accordionCrAlarms.OpenAndScrollToControl(_catsCrInvalidPin);
                    }
                },
                {
                    AlarmType.CardReader_InvalidCode,
                    () =>
                    {
                        _tcDevicesAlarmSetting.SelectedTab = _tpCrAlarms;
                        _accordionCrAlarms.OpenAndScrollToControl(_catsCrInvalidGin);
                    }
                },
                {
                    AlarmType.CardReader_InvalidEmergencyCode,
                    () =>
                    {
                        _tcDevicesAlarmSetting.SelectedTab = _tpCrAlarms;
                        _accordionCrAlarms.OpenAndScrollToControl(_catsCrInvalidEmergencyCode);
                    }
                },
                {
                    AlarmType.CardReader_AccessPermitted,
                    () =>
                    {
                        _tcDevicesAlarmSetting.SelectedTab = _tpCrAlarms;
                        _accordionCrAlarms.OpenAndScrollToControl(_catsCrAccessPermitted);
                    }
                },
                {
                    AlarmType.CardReader_Invalid_Pin_Retries_Limit_Reached,
                    () =>
                    {
                        _tcDevicesAlarmSetting.SelectedTab = _tpCrAlarms;
                        _accordionCrAlarms.OpenAndScrollToControl(_catsCrInvalidPinRetriesLimitReached);
                    }
                },
                {
                    AlarmType.CardReader_Invalid_Code_Retries_Limit_Reached,
                    () =>
                    {
                        _tcDevicesAlarmSetting.SelectedTab = _tpCrAlarms;
                        _accordionCrAlarms.OpenAndScrollToControl(_catsCrInvalidGinRetriesLimitReached);
                    }
                },
                {
                    AlarmType.AlarmArea_Alarm,
                    () =>
                    {
                        _tcDevicesAlarmSetting.SelectedTab = _tpAlarmAreaAlarms;
                        _accordionAlarmAreaAlarms.OpenAndScrollToControl(_cmaaAlarmReceptionCenterSettings);
                    }
                },
                {
                    AlarmType.AlarmArea_SetByOnOffObjectFailed,
                    () =>
                    {
                        _tcDevicesAlarmSetting.SelectedTab = _tpAlarmAreaAlarms;
                        _accordionAlarmAreaAlarms.OpenAndScrollToControl(_catsAlarmAreaSetByOnOffObjectFailed);
                    }
                }
            };

            InitializeComponent();

            _tcDevicesAlarmSetting.MouseWheel += _tcDevicesAlarmSetting_MouseWheel;

            //Alarm areas alarms
            _cmaaAlarmReceptionCenterSettings = new ControlModifyAlarmArcs
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_cmaaAlarmReceptionCenterSettings",
                TabIndex = 0,
                Plugin = Plugin
            };

            _cmaaAlarmReceptionCenterSettings.EditTextChanger += () => ValueChanged(null, null);
            _accordionAlarmAreaAlarms.Add(_cmaaAlarmReceptionCenterSettings, "Alarm reception center settings").Name = "_chbAlarmReceptionCenterSettings";

            _catsAlarmAreaAlarm = new ControlAlarmTypeSettings
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_catsAlarmAreaAlarm",
                TabIndex = 1,
                Plugin = Plugin,
                BlockAlarmVisible = false,
                AlarmEnableVisible = false
            };

            _catsAlarmAreaAlarm.EditTextChanger += () => ValueChanged(null, null);
            _catsAlarmAreaAlarm.EditTextChangerOnlyInDatabase += () => ValueChanged(null, null);
            _accordionAlarmAreaAlarms.Add(_catsAlarmAreaAlarm, "Alarm area alarm").Name = "_chbAlarmAreaAlarm";

            _catsAlarmAreaSetByOnOffObjectFailed = new ControlAlarmTypeSettings
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_catsAlarmAreaSetByOnOffObjectFailed",
                TabIndex = 2,
                Plugin = Plugin,
                EventlogDuringBlockedAlarmVisible = false
            };

            _catsAlarmAreaSetByOnOffObjectFailed.EditTextChanger += () => ValueChanged(null, null);
            _catsAlarmAreaSetByOnOffObjectFailed.EditTextChangerOnlyInDatabase += () => ValueChanged(null, null);
            _accordionAlarmAreaAlarms.Add(_catsAlarmAreaSetByOnOffObjectFailed, "Set by on/off object failed").Name = "_chbSetByOnOffObjectFailed";

            _catsSensorAlarm = new ControlAlarmTypeSettings
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_catsSensorAlarm",
                TabIndex = 3,
                Plugin = Plugin,
                BlockAlarmVisible = false,
                AlarmEnableVisible = false
            };

            _catsSensorAlarm.EditTextChanger += () => ValueChanged(null, null);
            _catsSensorAlarm.EditTextChangerOnlyInDatabase += () => ValueChanged(null, null);
            _accordionAlarmAreaAlarms.Add(_catsSensorAlarm, "Sensor alarm").Name = "_chbSensorAlarm";

            _catsSensorTamperAlarm = new ControlAlarmTypeSettings
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_catsSensorTamperAlarm",
                TabIndex = 4,
                Plugin = Plugin,
                BlockAlarmVisible = false,
                AlarmEnableVisible = false
            };

            _catsSensorTamperAlarm.EditTextChanger += () => ValueChanged(null, null);
            _catsSensorTamperAlarm.EditTextChangerOnlyInDatabase += () => ValueChanged(null, null);
            _accordionAlarmAreaAlarms.Add(_catsSensorTamperAlarm, "Sensor tamper alarm").Name = "_chbSensorTamperAlarm";

            //DCU alarms
            _catsDcuOffline = new ControlAlarmTypeSettings
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_catsDcuOffline",
                TabIndex = 0,
                Plugin = Plugin,
                EventlogDuringBlockedAlarmVisible = false
            };

            _catsDcuOffline.EditTextChanger += () => ValueChanged(null, null);
            _catsDcuOffline.EditTextChangerOnlyInDatabase += () => ValueChanged(null, null);

            _cmaaDcuOffline = new ControlModifyAlarmArcs
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_cmaaDcuOffline",
                TabIndex = 1,
                Plugin = Plugin
            };

            _cmaaDcuOffline.EditTextChanger += () => ValueChanged(null, null);

            _catsDcuOffline.AddControl(_cmaaDcuOffline);

            _accordionDcuAlarms.Add(_catsDcuOffline, "DCU offline").Name = "_chbDcuOffline";

            _cmaaDcuOfflineDueCcuOffline = new ControlModifyAlarmArcs
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_cmaaDcuOfflineDueCcuOffline",
                TabIndex = 1,
                Plugin = Plugin
            };

            _cmaaDcuOfflineDueCcuOffline.EditTextChanger += () => ValueChanged(null, null);

            _accordionDcuAlarms.Add(_cmaaDcuOfflineDueCcuOffline, "DCU offline due CCU offline").Name = "_chbDcuOfflineDueCcuOffline";

            _catsDcuTamperSabotage = new ControlAlarmTypeSettings
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_catsDcuTamperSabotage",
                TabIndex = 2,
                Plugin = Plugin
            };

            _catsDcuTamperSabotage.EditTextChanger += () => ValueChanged(null, null);
            _catsDcuTamperSabotage.EditTextChangerOnlyInDatabase += () => ValueChanged(null, null);

            _cmaaDcuTamperSabotage = new ControlModifyAlarmArcs
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_cmaaDcuTamperSabotage",
                TabIndex = 1,
                Plugin = Plugin
            };

            _cmaaDcuTamperSabotage.EditTextChanger += () => ValueChanged(null, null);

            _catsDcuTamperSabotage.AddControl(_cmaaDcuTamperSabotage);

            _accordionDcuAlarms.Add(_catsDcuTamperSabotage, "DCU tamper/sabotage").Name = "_chbDcuTamperSabotage";

            _cmaaDcuOutdatedFirmware = new ControlModifyAlarmArcs
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_cmaaDcuOutdatedFirmware",
                TabIndex = 3,
                Plugin = Plugin
            };

            _cmaaDcuOutdatedFirmware.EditTextChanger += () => ValueChanged(null, null);

            _accordionDcuAlarms.Add(_cmaaDcuOutdatedFirmware, "DCU outdated firmware").Name = "_chbDcuOutdatedFirmware";

            _accordionDcuAlarms.OpenedAllChnged +=
                isOpenAll => SetOpenAllCheckState(
                    _cbOpenAllDcuAlarms,
                    isOpenAll);

            SetOpenAllCheckState(_cbOpenAllDcuAlarms, _accordionDcuAlarms.IsOpenAll);

            //DSM alarm
            _catsDsmDoorAjar = new ControlAlarmTypeSettings
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_catsDsmDoorAjar",
                TabIndex = 0,
                Plugin = Plugin,
                EventlogDuringBlockedAlarmVisible = false
            };

            _catsDsmDoorAjar.EditTextChanger += () => ValueChanged(null, null);
            _catsDsmDoorAjar.EditTextChangerOnlyInDatabase += () => ValueChanged(null, null);

            _cmaaDsmDoorAjar = new ControlModifyAlarmArcs
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_cmaaDsmDoorAjar",
                TabIndex = 1,
                Plugin = Plugin
            };

            _cmaaDsmDoorAjar.EditTextChanger += () => ValueChanged(null, null);

            _catsDsmDoorAjar.AddControl(_cmaaDsmDoorAjar);

            _accordionDsmAlarms.Add(_catsDsmDoorAjar, "DSM door ajar").Name = "_chbDsmDoorAjar";

            _catsDsmIntrusion = new ControlAlarmTypeSettings
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_catsDsmIntrusion",
                TabIndex = 1,
                Plugin = Plugin,
                EventlogDuringBlockedAlarmVisible = false
            };

            _catsDsmIntrusion.EditTextChanger += () => ValueChanged(null, null);
            _catsDsmIntrusion.EditTextChangerOnlyInDatabase += () => ValueChanged(null, null);

            _cmaaDsmIntrusion = new ControlModifyAlarmArcs
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_cmaaDsmIntrusion",
                TabIndex = 1,
                Plugin = Plugin
            };

            _cmaaDsmIntrusion.EditTextChanger += () => ValueChanged(null, null);

            _catsDsmIntrusion.AddControl(_cmaaDsmIntrusion);

            _accordionDsmAlarms.Add(_catsDsmIntrusion, "DSM intrusion").Name = "_chbDsmIntrusion";

            _catsDsmSabotage = new ControlAlarmTypeSettings
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_catsDsmSabotage",
                TabIndex = 2,
                Plugin = Plugin,
                EventlogDuringBlockedAlarmVisible = false
            };

            _catsDsmSabotage.EditTextChanger += () => ValueChanged(null, null);
            _catsDsmSabotage.EditTextChangerOnlyInDatabase += () => ValueChanged(null, null);

            _cmaaDsmSabotage = new ControlModifyAlarmArcs
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_cmaaDsmSabotage",
                TabIndex = 1,
                Plugin = Plugin
            };

            _cmaaDsmSabotage.EditTextChanger += () => ValueChanged(null, null);

            _catsDsmSabotage.AddControl(_cmaaDsmSabotage);

            _accordionDsmAlarms.Add(_catsDsmSabotage, "DSM sabotage").Name = "_chbDsmSabotage";

            _accordionDsmAlarms.OpenedAllChnged +=
                isOpenAll => SetOpenAllCheckState(
                    _cbOpenAllDsmAlarms,
                    isOpenAll);

            SetOpenAllCheckState(_cbOpenAllDsmAlarms, _accordionDsmAlarms.IsOpenAll);

            //CR alarms
            _catsCrOffline = new ControlAlarmTypeSettings
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_catsCrOffline",
                TabIndex = 0,
                Plugin = Plugin,
                EventlogDuringBlockedAlarmVisible = false
            };

            _catsCrOffline.EditTextChanger += () => ValueChanged(null, null);
            _catsCrOffline.EditTextChangerOnlyInDatabase += () => ValueChanged(null, null);

            _cmaaCrOffline = new ControlModifyAlarmArcs
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_cmaaCrOffline",
                TabIndex = 1,
                Plugin = Plugin
            };

            _cmaaCrOffline.EditTextChanger += () => ValueChanged(null, null);

            _catsCrOffline.AddControl(_cmaaCrOffline);

            _accordionCrAlarms.Add(_catsCrOffline, "Card reader offline").Name = "_chbCrOffline";

            _cmaaCrOfflineDueCcuOffline = new ControlModifyAlarmArcs
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_cmaaCrOfflineDueCcuOffline",
                TabIndex = 1,
                Plugin = Plugin
            };

            _cmaaCrOfflineDueCcuOffline.EditTextChanger += () => ValueChanged(null, null);

            _accordionCrAlarms.Add(_cmaaCrOfflineDueCcuOffline, "Card reader offline due CCU offline").Name = "_chbCrOfflineDueCcuOffline";

            _catsCrTamperSabotage = new ControlAlarmTypeSettings
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_catsCrTamperSabotage",
                TabIndex = 2,
                Plugin = Plugin
            };

            _catsCrTamperSabotage.EditTextChanger += () => ValueChanged(null, null);
            _catsCrTamperSabotage.EditTextChangerOnlyInDatabase += () => ValueChanged(null, null);

            _cmaaCrTamperSabotage = new ControlModifyAlarmArcs
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_cmaaCrTamperSabotage",
                TabIndex = 1,
                Plugin = Plugin
            };

            _cmaaCrTamperSabotage.EditTextChanger += () => ValueChanged(null, null);

            _catsCrTamperSabotage.AddControl(_cmaaCrTamperSabotage);

            _accordionCrAlarms.Add(_catsCrTamperSabotage, "Card reader tamper/sabotage").Name = "_chbCrTamperSabotage";

            _catsCrAccessDenied = new ControlAlarmTypeSettings
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_catsCrAccessDenied",
                TabIndex = 3,
                Plugin = Plugin
            };

            _catsCrAccessDenied.EditTextChanger += () => ValueChanged(null, null);
            _catsCrAccessDenied.EditTextChangerOnlyInDatabase += () => ValueChanged(null, null);

            _cmaaCrAccessDenied = new ControlModifyAlarmArcs
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_cmaaCrAccessDenied",
                TabIndex = 1,
                Plugin = Plugin
            };

            _cmaaCrAccessDenied.EditTextChanger += () => ValueChanged(null, null);

            _catsCrAccessDenied.AddControl(_cmaaCrAccessDenied);

            _accordionCrAlarms.Add(_catsCrAccessDenied, "Access denied").Name = "_chbCrAccessDenied";

            _catsCrUnknownCard = new ControlAlarmTypeSettings
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_catsCrUnknownCard",
                TabIndex = 4,
                Plugin = Plugin
            };

            _catsCrUnknownCard.EditTextChanger += () => ValueChanged(null, null);
            _catsCrUnknownCard.EditTextChangerOnlyInDatabase += () => ValueChanged(null, null);

            _cmaaCrUnknownCard = new ControlModifyAlarmArcs
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_cmaaCrUnknownCard",
                TabIndex = 1,
                Plugin = Plugin
            };

            _cmaaCrUnknownCard.EditTextChanger += () => ValueChanged(null, null);

            _catsCrUnknownCard.AddControl(_cmaaCrUnknownCard);

            _accordionCrAlarms.Add(_catsCrUnknownCard, "Unknown card").Name = "_chbCrUnknownCard";

            _catsCrCardBlockedOrInactive = new ControlAlarmTypeSettings
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_catsCrCardBlockedOrInactive",
                TabIndex = 5,
                Plugin = Plugin
            };

            _catsCrCardBlockedOrInactive.EditTextChanger += () => ValueChanged(null, null);
            _catsCrCardBlockedOrInactive.EditTextChangerOnlyInDatabase += () => ValueChanged(null, null);

            _cmaaCrCardBlockedOrInactive = new ControlModifyAlarmArcs
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_cmaaCrCardBlockedOrInactive",
                TabIndex = 1,
                Plugin = Plugin
            };

            _cmaaCrCardBlockedOrInactive.EditTextChanger += () => ValueChanged(null, null);

            _catsCrCardBlockedOrInactive.AddControl(_cmaaCrCardBlockedOrInactive);

            _accordionCrAlarms.Add(_catsCrCardBlockedOrInactive, "Card blocked or inactive").Name = "_chbCrCardBlockedOrInactive";

            _catsCrInvalidPin = new ControlAlarmTypeSettings
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_catsCrInvalidPin",
                TabIndex = 6,
                Plugin = Plugin
            };

            _catsCrInvalidPin.EditTextChanger += () => ValueChanged(null, null);
            _catsCrInvalidPin.EditTextChangerOnlyInDatabase += () => ValueChanged(null, null);

            _cmaaCrInvalidPin = new ControlModifyAlarmArcs
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_cmaaCrInvalidPin",
                TabIndex = 1,
                Plugin = Plugin
            };

            _cmaaCrInvalidPin.EditTextChanger += () => ValueChanged(null, null);

            _catsCrInvalidPin.AddControl(_cmaaCrInvalidPin);

            _accordionCrAlarms.Add(_catsCrInvalidPin, "Invalid PIN").Name = "_chbCrInvalidPin";

            _catsCrInvalidGin = new ControlAlarmTypeSettings
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_catsCrInvalidGin",
                TabIndex = 7,
                Plugin = Plugin
            };

            _catsCrInvalidGin.EditTextChanger += () => ValueChanged(null, null);
            _catsCrInvalidGin.EditTextChangerOnlyInDatabase += () => ValueChanged(null, null);

            _cmaaCrInvalidGin = new ControlModifyAlarmArcs
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_cmaaCrInvalidGin",
                TabIndex = 1,
                Plugin = Plugin
            };

            _cmaaCrInvalidGin.EditTextChanger += () => ValueChanged(null, null);

            _catsCrInvalidGin.AddControl(_cmaaCrInvalidGin);

            _accordionCrAlarms.Add(_catsCrInvalidGin, "Invalid GIN").Name = "_chbCrInvalidGin";

            _catsCrInvalidEmergencyCode = new ControlAlarmTypeSettings
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_catsCrInvalidEmergencyCode",
                TabIndex = 8,
                Plugin = Plugin
            };

            _catsCrInvalidEmergencyCode.EditTextChanger += () => ValueChanged(null, null);
            _catsCrInvalidEmergencyCode.EditTextChangerOnlyInDatabase += () => ValueChanged(null, null);

            _cmaaCrInvalidEmergencyCode = new ControlModifyAlarmArcs
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_cmaaCrInvalidEmergencyCode",
                TabIndex = 1,
                Plugin = Plugin
            };

            _cmaaCrInvalidEmergencyCode.EditTextChanger += () => ValueChanged(null, null);

            _catsCrInvalidEmergencyCode.AddControl(_cmaaCrInvalidEmergencyCode);

            _accordionCrAlarms.Add(_catsCrInvalidEmergencyCode, "Invalid emergency code").Name = "_chbCrInvalidEmergencyCode";

            _catsCrAccessPermitted = new ControlAlarmTypeSettings
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_catsCrAccessPermitted",
                TabIndex = 9,
                Plugin = Plugin,
                EventlogDuringBlockedAlarmVisible = false
            };

            _catsCrAccessPermitted.EditTextChanger += () => ValueChanged(null, null);
            _catsCrAccessPermitted.EditTextChangerOnlyInDatabase += () => ValueChanged(null, null);

            _cmaaCrAccessPermitted = new ControlModifyAlarmArcs
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_cmaaCrAccessPermitted",
                TabIndex = 1,
                Plugin = Plugin
            };

            _cmaaCrAccessPermitted.EditTextChanger += () => ValueChanged(null, null);

            _catsCrAccessPermitted.AddControl(_cmaaCrAccessPermitted);

            _accordionCrAlarms.Add(_catsCrAccessPermitted, "Access permitted").Name = "_chbCrAccessPermitted";

            _catsCrInvalidPinRetriesLimitReached = new ControlAlarmTypeSettings
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_catsCrInvalidPinRetriesLimitReached",
                TabIndex = 10,
                Plugin = Plugin,
                EventlogDuringBlockedAlarmVisible = false
            };

            _catsCrInvalidPinRetriesLimitReached.EditTextChanger += () => ValueChanged(null, null);
            _catsCrInvalidPinRetriesLimitReached.EditTextChangerOnlyInDatabase += () => ValueChanged(null, null);

            _cmaaCrInvalidPinRetriesLimitReached = new ControlModifyAlarmArcs
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_cmaaCrInvalidPinRetriesLimitReached",
                TabIndex = 1,
                Plugin = Plugin
            };

            _cmaaCrInvalidPinRetriesLimitReached.EditTextChanger += () => ValueChanged(null, null);

            _catsCrInvalidPinRetriesLimitReached.AddControl(_cmaaCrInvalidPinRetriesLimitReached);

            _accordionCrAlarms.Add(_catsCrInvalidPinRetriesLimitReached, "Invalid PIN retries limit reached").Name = "_chbCrInvalidPinRetriesLimitReached";

            _catsCrInvalidGinRetriesLimitReached = new ControlAlarmTypeSettings
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_catsCrInvalidGinRetriesLimitReached",
                TabIndex = 11,
                Plugin = Plugin,
                EventlogDuringBlockedAlarmVisible = false
            };

            _catsCrInvalidGinRetriesLimitReached.EditTextChanger += () => ValueChanged(null, null);
            _catsCrInvalidGinRetriesLimitReached.EditTextChangerOnlyInDatabase += () => ValueChanged(null, null);

            _cmaaCrInvalidGinRetriesLimitReached = new ControlModifyAlarmArcs
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_cmaaCrInvalidGinRetriesLimitReached",
                TabIndex = 1,
                Plugin = Plugin
            };

            _cmaaCrInvalidGinRetriesLimitReached.EditTextChanger += () => ValueChanged(null, null);

            _catsCrInvalidGinRetriesLimitReached.AddControl(_cmaaCrInvalidGinRetriesLimitReached);

            _accordionCrAlarms.Add(_catsCrInvalidGinRetriesLimitReached, "Invalid GIN retries limit reached").Name = "_chbCrInvalidGinRetriesLimitReached";

            _accordionCrAlarms.OpenedAllChnged +=
                isOpenAll => SetOpenAllCheckState(
                    _cbOpenAllCrAlarms,
                    isOpenAll);

            SetOpenAllCheckState(_cbOpenAllCrAlarms, _accordionCrAlarms.IsOpenAll);

            //CCU alarms
            _catsCcuOffline = new ControlAlarmTypeSettings
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_catsCcuOffline",
                TabIndex = 0,
                Plugin = Plugin,
                EventlogDuringBlockedAlarmVisible = false
            };

            _catsCcuOffline.EditTextChanger += () => ValueChanged(null, null);
            _catsCcuOffline.EditTextChangerOnlyInDatabase += () => ValueChanged(null, null);

            _cmaaCcuOffline = new ControlModifyAlarmArcs
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_cmaaCcuOffline",
                TabIndex = 1,
                Plugin = Plugin
            };

            _cmaaCcuOffline.EditTextChanger += () => ValueChanged(null, null);

            _catsCcuOffline.AddControl(_cmaaCcuOffline);

            _accordionCcuAlarms.Add(_catsCcuOffline, "CCU offline").Name = "_chbCcuOffline";

            _catsCcuUnconfigured = new ControlAlarmTypeSettings
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_catsCcuUnconfigured",
                TabIndex = 1,
                Plugin = Plugin,
                EventlogDuringBlockedAlarmVisible = false
            };

            _catsCcuUnconfigured.EditTextChanger += () => ValueChanged(null, null);
            _catsCcuUnconfigured.EditTextChangerOnlyInDatabase += () => ValueChanged(null, null);

            _cmaaCcuUnconfigured = new ControlModifyAlarmArcs
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_cmaaCcuUnconfigured",
                TabIndex = 1,
                Plugin = Plugin
            };

            _cmaaCcuUnconfigured.EditTextChanger += () => ValueChanged(null, null);

            _catsCcuUnconfigured.AddControl(_cmaaCcuUnconfigured);

            _accordionCcuAlarms.Add(_catsCcuUnconfigured, "CCU unconfigured").Name = "_chbCcuUnconfigured";

            _catsCcuClockUnsynchronized = new ControlAlarmTypeSettings
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_catsCrInval_catsCcuClockUnsynchronizedidGinRetriesLimitReached",
                TabIndex = 2,
                Plugin = Plugin,
                EventlogDuringBlockedAlarmVisible = false
            };

            _catsCcuClockUnsynchronized.EditTextChanger += () => ValueChanged(null, null);
            _catsCcuClockUnsynchronized.EditTextChangerOnlyInDatabase += () => ValueChanged(null, null);

            _cmaaCcuClockUnsynchronized = new ControlModifyAlarmArcs
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_cmaaCcuClockUnsynchronized",
                TabIndex = 1,
                Plugin = Plugin
            };

            _cmaaCcuClockUnsynchronized.EditTextChanger += () => ValueChanged(null, null);

            _catsCcuClockUnsynchronized.AddControl(_cmaaCcuClockUnsynchronized);

            _accordionCcuAlarms.Add(_catsCcuClockUnsynchronized, "CCU clock unsynchronized").Name = "_chbCcuClockUnsynchronized";

            _catsCcuTamperSabotage = new ControlAlarmTypeSettings
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_catsCcuTamperSabotage",
                TabIndex = 3,
                Plugin = Plugin
            };

            _catsCcuTamperSabotage.EditTextChanger += () => ValueChanged(null, null);
            _catsCcuTamperSabotage.EditTextChangerOnlyInDatabase += () => ValueChanged(null, null);

            _cmaaCcuTamperSabotage = new ControlModifyAlarmArcs
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_cmaaCcuTamperSabotage",
                TabIndex = 1,
                Plugin = Plugin
            };

            _cmaaCcuTamperSabotage.EditTextChanger += () => ValueChanged(null, null);

            _catsCcuTamperSabotage.AddControl(_cmaaCcuTamperSabotage);

            _accordionCcuAlarms.Add(_catsCcuTamperSabotage, "CCU tamper/sabotage").Name = "_chbCcuTamperSabotage";

            _catsCcuPrimaryPowerMissing = new ControlAlarmTypeSettings
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_catsCcuPrimaryPowerMissing",
                TabIndex = 4,
                Plugin = Plugin
            };

            _catsCcuPrimaryPowerMissing.EditTextChanger += () => ValueChanged(null, null);
            _catsCcuPrimaryPowerMissing.EditTextChangerOnlyInDatabase += () => ValueChanged(null, null);

            _cmaaCcuPrimaryPowerMissing = new ControlModifyAlarmArcs
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_cmaaCcuPrimaryPowerMissing",
                TabIndex = 1,
                Plugin = Plugin
            };

            _cmaaCcuPrimaryPowerMissing.EditTextChanger += () => ValueChanged(null, null);

            _catsCcuPrimaryPowerMissing.AddControl(_cmaaCcuPrimaryPowerMissing);

            _accordionCcuAlarms.Add(_catsCcuPrimaryPowerMissing, "CCU primary power missing").Name = "_chbCcuPrimaryPowerMissing";

            _catsCcuBatteryIsLow = new ControlAlarmTypeSettings
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_catsCcuBatteryIsLow",
                TabIndex = 5,
                Plugin = Plugin
            };

            _catsCcuBatteryIsLow.EditTextChanger += () => ValueChanged(null, null);
            _catsCcuBatteryIsLow.EditTextChangerOnlyInDatabase += () => ValueChanged(null, null);

            _cmaaCcuBatteryIsLow = new ControlModifyAlarmArcs
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_cmaaCcuBatteryIsLow",
                TabIndex = 1,
                Plugin = Plugin
            };

            _cmaaCcuBatteryIsLow.EditTextChanger += () => ValueChanged(null, null);

            _catsCcuBatteryIsLow.AddControl(_cmaaCcuBatteryIsLow);

            _accordionCcuAlarms.Add(_catsCcuBatteryIsLow, "CCU battery is low").Name = "_chbCcuBatteryIsLow";

            _catsCcuUpsOutputFuse = new ControlAlarmTypeSettings
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_catsCcuUpsOutputFuse",
                TabIndex = 6,
                Plugin = Plugin
            };

            _catsCcuUpsOutputFuse.EditTextChanger += () => ValueChanged(null, null);
            _catsCcuUpsOutputFuse.EditTextChangerOnlyInDatabase += () => ValueChanged(null, null);

            _cmaaCcuUpsOutputFuse = new ControlModifyAlarmArcs
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_cmaaCcuUpsOutputFuse",
                TabIndex = 1,
                Plugin = Plugin
            };

            _cmaaCcuUpsOutputFuse.EditTextChanger += () => ValueChanged(null, null);

            _catsCcuUpsOutputFuse.AddControl(_cmaaCcuUpsOutputFuse);

            _accordionCcuAlarms.Add(_catsCcuUpsOutputFuse, "UPS output fuse").Name = "_chbCcuUpsOutputFuse";

            _catsCcuUpsBatteryFault = new ControlAlarmTypeSettings
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_catsCcuUpsBatteryFault",
                TabIndex = 7,
                Plugin = Plugin
            };

            _catsCcuUpsBatteryFault.EditTextChanger += () => ValueChanged(null, null);
            _catsCcuUpsBatteryFault.EditTextChangerOnlyInDatabase += () => ValueChanged(null, null);

            _cmaaCcuUpsBatteryFault = new ControlModifyAlarmArcs
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_cmaaCcuUpsBatteryFault",
                TabIndex = 1,
                Plugin = Plugin
            };

            _cmaaCcuUpsBatteryFault.EditTextChanger += () => ValueChanged(null, null);

            _catsCcuUpsBatteryFault.AddControl(_cmaaCcuUpsBatteryFault);

            _accordionCcuAlarms.Add(_catsCcuUpsBatteryFault, "UPS battery fault").Name = "_chbCcuUpsBatteryFault";

            _catsCcuUpsBatteryFuse = new ControlAlarmTypeSettings
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_catsCcuUpsBatteryFuse",
                TabIndex = 8,
                Plugin = Plugin
            };

            _catsCcuUpsBatteryFuse.EditTextChanger += () => ValueChanged(null, null);
            _catsCcuUpsBatteryFuse.EditTextChangerOnlyInDatabase += () => ValueChanged(null, null);

            _cmaaCcuUpsBatteryFuse = new ControlModifyAlarmArcs
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_cmaaCcuUpsBatteryFuse",
                TabIndex = 1,
                Plugin = Plugin
            };

            _cmaaCcuUpsBatteryFuse.EditTextChanger += () => ValueChanged(null, null);

            _catsCcuUpsBatteryFuse.AddControl(_cmaaCcuUpsBatteryFuse);

            _accordionCcuAlarms.Add(_catsCcuUpsBatteryFuse, "UPS battery fuse").Name = "_chbCcuUpsBatteryFuse";

            _catsCcuUpsOvertemperature = new ControlAlarmTypeSettings
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_catsCcuUpsOvertemperature",
                TabIndex = 9,
                Plugin = Plugin
            };

            _catsCcuUpsOvertemperature.EditTextChanger += () => ValueChanged(null, null);
            _catsCcuUpsOvertemperature.EditTextChangerOnlyInDatabase += () => ValueChanged(null, null);

            _cmaaCcuUpsOvertemperature = new ControlModifyAlarmArcs
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_cmaaCcuUpsOvertemperature",
                TabIndex = 1,
                Plugin = Plugin
            };

            _cmaaCcuUpsOvertemperature.EditTextChanger += () => ValueChanged(null, null);

            _catsCcuUpsOvertemperature.AddControl(_cmaaCcuUpsOvertemperature);

            _accordionCcuAlarms.Add(_catsCcuUpsOvertemperature, "UPS overtemperature").Name = "_chbCcuUpsOvertemperature";

            _catsCcuUpsTamper = new ControlAlarmTypeSettings
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_catsCcuUpsTamper",
                TabIndex = 10,
                Plugin = Plugin
            };

            _catsCcuUpsTamper.EditTextChanger += () => ValueChanged(null, null);
            _catsCcuUpsTamper.EditTextChangerOnlyInDatabase += () => ValueChanged(null, null);

            _cmaaCcuUpsTamper = new ControlModifyAlarmArcs
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_cmaaCcuUpsTamper",
                TabIndex = 1,
                Plugin = Plugin
            };

            _cmaaCcuUpsTamper.EditTextChanger += () => ValueChanged(null, null);

            _catsCcuUpsTamper.AddControl(_cmaaCcuUpsTamper);

            _accordionCcuAlarms.Add(_catsCcuUpsTamper, "UPS tamper/sabotage").Name = "_chbCcuUpsTamper";

            _catsCcuFuseOnExtensionBoard = new ControlAlarmTypeSettings
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_catsCcuFuseOnExtensionBoard",
                TabIndex = 11,
                Plugin = Plugin
            };

            _catsCcuFuseOnExtensionBoard.EditTextChanger += () => ValueChanged(null, null);
            _catsCcuFuseOnExtensionBoard.EditTextChangerOnlyInDatabase += () => ValueChanged(null, null);

            _cmaaCcuFuseOnExtensionBoard = new ControlModifyAlarmArcs
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_cmaaCcuFuseOnExtensionBoard",
                TabIndex = 1,
                Plugin = Plugin
            };

            _cmaaCcuFuseOnExtensionBoard.EditTextChanger += () => ValueChanged(null, null);

            _catsCcuFuseOnExtensionBoard.AddControl(_cmaaCcuFuseOnExtensionBoard);

            _accordionCcuAlarms.Add(_catsCcuFuseOnExtensionBoard, "CCU fuse on extension board").Name = "_chbCcuFuseOnExtensionBoard";

            _cmaaCcuOutdatedFirmware = new ControlModifyAlarmArcs
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_cmaaCcuOutdatedFirmware",
                TabIndex = 12,
                Plugin = Plugin
            };

            _cmaaCcuOutdatedFirmware.EditTextChanger += () => ValueChanged(null, null);

            _accordionCcuAlarms.Add(_cmaaCcuOutdatedFirmware, "CCU outdated firmware")
                .Name = "_chbCcuOutdatedFirmware";

            _cmaaCcuDataChannelDistrupted = new ControlModifyAlarmArcs
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_cmaaCcuDataChannelDistrupted",
                TabIndex = 13,
                Plugin = Plugin
            };

            _cmaaCcuDataChannelDistrupted.EditTextChanger += () => ValueChanged(null, null);

            _accordionCcuAlarms.Add(_cmaaCcuDataChannelDistrupted, "CCU data channel distrupted")
                .Name = "_chbCcuDataChannelDistrupted";

            _cmaaCcuCoprocessorFailure = new ControlModifyAlarmArcs
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_cmaaCcuCoprocessorFailure",
                TabIndex = 14,
                Plugin = Plugin
            };

            _cmaaCcuCoprocessorFailure.EditTextChanger += () => ValueChanged(null, null);

            _accordionCcuAlarms.Add(_cmaaCcuCoprocessorFailure, "CCU coprocessor failure")
                .Name = "_chbCcuCoprocessorFailure";

            _cmaaCcuHighMemoryLoad = new ControlModifyAlarmArcs
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_cmaaCcuHighMemoryLoad",
                TabIndex = 15,
                Plugin = Plugin
            };

            _cmaaCcuHighMemoryLoad.EditTextChanger += () => ValueChanged(null, null);

            _accordionCcuAlarms.Add(_cmaaCcuHighMemoryLoad, "CCU high memory load")
                .Name = "_chbCcuHighMemoryLoad";

            _cmaaCcuFilesystemProblem = new ControlModifyAlarmArcs
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_cmaaCcuFilesystemProblem",
                TabIndex = 16,
                Plugin = Plugin
            };

            _cmaaCcuFilesystemProblem.EditTextChanger += () => ValueChanged(null, null);

            _accordionCcuAlarms.Add(_cmaaCcuFilesystemProblem, "CCU filesystem problem")
                .Name = "_chbCcuFilesystemProblem";

            _cmaaCcuSdCardNotFound = new ControlModifyAlarmArcs
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_cmaaCcuSdCardNotFound",
                TabIndex = 17,
                Plugin = Plugin
            };

            _cmaaCcuSdCardNotFound.EditTextChanger += () => ValueChanged(null, null);

            _accordionCcuAlarms.Add(_cmaaCcuSdCardNotFound, "CCU SD card not found")
                .Name = "_chbCcuSdCardNotFound";

            _cmaaICcuSendingOfObjectStateFailed = new ControlModifyAlarmArcs
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_cmaaICcuSendingOfObjectStateFailed",
                TabIndex = 18,
                Plugin = Plugin
            };

            _cmaaICcuSendingOfObjectStateFailed.EditTextChanger += () => ValueChanged(null, null);

            _accordionCcuAlarms.Add(_cmaaICcuSendingOfObjectStateFailed, "ICCU sending of object state failed")
                .Name = "_chbICcuSendingOfObjectStateFailed";

            _cmaaICcuPortAlreadyUsed = new ControlModifyAlarmArcs
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_cmaaICcuPortAlreadyUsed",
                TabIndex = 19,
                Plugin = Plugin
            };

            _cmaaICcuPortAlreadyUsed.EditTextChanger += () => ValueChanged(null, null);

            _accordionCcuAlarms.Add(_cmaaICcuPortAlreadyUsed, "ICCU port already used")
                .Name = "_chbICcuPortAlreadyUsed";

            _catsCcuCatUnreachable = new ControlAlarmTypeSettings
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_catsCcuCatUnreachable",
                TabIndex = 20,
                Plugin = Plugin,
                BlockAlarmVisible = false
            };

            _catsCcuCatUnreachable.EditTextChanger += () => ValueChanged(null, null);
            _catsCcuCatUnreachable.EditTextChangerOnlyInDatabase += () => ValueChanged(null, null);

            _accordionCcuAlarms.Add(_catsCcuCatUnreachable, "CAT offline").Name = "_chbCcuCatUnreachable";

            _catsCcuTransferToArcTimedOut = new ControlAlarmTypeSettings
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_catsCcuTransferToArcTimedOut",
                TabIndex = 21,
                Plugin = Plugin,
                BlockAlarmVisible = false
            };

            _catsCcuTransferToArcTimedOut.EditTextChanger += () => ValueChanged(null, null);
            _catsCcuTransferToArcTimedOut.EditTextChangerOnlyInDatabase += () => ValueChanged(null, null);

            _accordionCcuAlarms.Add(_catsCcuTransferToArcTimedOut, "Alarm not transmitted to ARC").Name = "_chbCcuTransferToArcTimedOut";

            _accordionCcuAlarms.OpenedAllChnged +=
                isOpenAll => SetOpenAllCheckState(
                    _cbOpenAllCcuAlarms,
                    isOpenAll);

            SetOpenAllCheckState(_cbOpenAllCcuAlarms, _accordionCcuAlarms.IsOpenAll);

            _tbmSecurityDailyPlanTimeZoneForEnterToMenu.ImageTextBox.ForeColor = CgpClientMainForm.Singleton.GetDragDropTextColor;
            _tbmSecurityDailyPlanTimeZoneForEnterToMenu.ImageTextBox.BackColor = CgpClientMainForm.Singleton.GetDragDropBackgroundColor;

            // SB ?? no support for databinding ??
            /*
            var checkedListBox = (ListBox)checkedListBoxPersons;

            checkedListBox.DisplayMember = "Person.FullName";
            checkedListBox.ValueMember = "IsChecked";            
            */
        }

        private void SetOpenAllCheckState(CheckBox checkBox, bool? isOpenAll)
        {
            checkBox.CheckState = isOpenAll.HasValue
                ? (isOpenAll.Value
                    ? CheckState.Checked
                    : CheckState.Unchecked)
                : CheckState.Indeterminate;
        }

        private void _tcDevicesAlarmSetting_MouseWheel(object sender, MouseEventArgs e)
        {
            if (_tcDevicesAlarmSetting.Focused)
            {
                var selectedTab = _tcDevicesAlarmSetting.SelectedTab;

                if (selectedTab == _tpCcuAlarms)
                {
                    _accordionCcuAlarms.Focus();
                    return;
                }

                if (selectedTab == _tpDcuAlarms)
                {
                    _accordionDcuAlarms.Focus();
                    return;
                }

                if (selectedTab == _tpDoorEnvironmentAlarms)
                {
                    _accordionDsmAlarms.Focus();
                    return;
                }

                if (selectedTab == _tpCrAlarms)
                    _accordionCrAlarms.Focus();
            }
        }

        protected override DevicesAlarmSetting GetObjectForEdit(DevicesAlarmSetting listObj, out bool editAllowed)
        {
            editAllowed = HasAccessView();
            return listObj;
        }

        protected override DevicesAlarmSetting GetFromShort(DevicesAlarmSetting listObj)
        {
            return listObj;
        }

        DevicesAlarmSetting _devicesAlarmSetting;

        private bool _textChanged;

        private static volatile NCASDevicesAlarmSettingsForm _singleton;
        private Cgp.Server.Beans.TimeZone _actReportTimeZone;
        private Cgp.Server.Beans.TimeZone _actDoorEnvirometReportTimeZone;
        private static readonly object _syncRoot = new object();

        public static NCASDevicesAlarmSettingsForm Singleton
        {
            get
            {
                if (null == _singleton)
                    lock (_syncRoot)
                    {
                        if (_singleton == null)
                        {
                            _singleton =
                                new NCASDevicesAlarmSettingsForm
                                {
                                    MdiParent = CgpClientMainForm.Singleton
                                };
                        }
                    }

                return _singleton;
            }
        }

        protected override ICollection<DevicesAlarmSetting> GetData()
        {
            Exception error;
            var list = Plugin.MainServerProvider.DevicesAlarmSettings.SelectByCriteria(FilterSettings, out error);

            if (error != null)
                throw (error);

            SafeThread.StartThread(HideDisableTabPages);
            SetEnableForApply();

            return list;
        }

        private void HideDisableTabPages()
        {
            try
            {
                HideDisableTabPageAlarmAreaAlarms(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.AlarmSettingsAaAlarmsView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.AlarmSettingsAaAlarmsAdmin)));

                HideDisableTabPageCcuAlarms(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.AlarmSettingsCcuAlarmsView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.AlarmSettingsCcuAlarmsAdmin)));

                HideDisableTabPageDcuAlarms(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.AlarmSettingsDcuAlarmsView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.AlarmSettingsDcuAlarmsAdmin)));

                HideDisableTabPageDoorEnvironemntAlarms(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.AlarmSettingsDeAlarmsView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.AlarmSettingsDeAlarmsAdmin)));

                HideDisableTabPageCrAlarms(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.AlarmSettingsCrAlarmsView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.AlarmSettingsCrAlarmsAdmin)));

                HideDisableTabPageInvalidPinGinRetriesLimits(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.AlarmSettingsInvalidPinGinRetriesLimitsView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.AlarmSettingsInvalidPinGinRetriesLimitsdmin)));

                HideTabPagePersons(
                    CgpClient.Singleton.MainServerProvider.CheckTimetecLicense());
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        private void HideDisableTabPageAlarmAreaAlarms(bool view, bool admin)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool, bool>(HideDisableTabPageAlarmAreaAlarms),
                    view,
                    admin);
            }
            else
            {
                if (!view && !admin)
                {
                    _tcDevicesAlarmSetting.TabPages.Remove(_tpAlarmAreaAlarms);
                    return;
                }

                _tpAlarmAreaAlarms.Enabled = admin;
            }
        }

        private void HideDisableTabPageCcuAlarms(bool view, bool admin)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool, bool>(HideDisableTabPageCcuAlarms),
                    view,
                    admin);
            }
            else
            {
                if (!view && !admin)
                {
                    _tcDevicesAlarmSetting.TabPages.Remove(_tpCcuAlarms);
                    return;
                }

                _tpCcuAlarms.Enabled = admin;
            }
        }

        private void HideDisableTabPageDcuAlarms(bool view, bool admin)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool, bool>(HideDisableTabPageDcuAlarms),
                    view,
                    admin);
            }
            else
            {
                if (!view && !admin)
                {
                    _tcDevicesAlarmSetting.TabPages.Remove(_tpDcuAlarms);
                    return;
                }

                _tpDcuAlarms.Enabled = admin;
            }
        }

        private void HideDisableTabPageDoorEnvironemntAlarms(bool view, bool admin)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool, bool>(HideDisableTabPageDoorEnvironemntAlarms),
                    view,
                    admin);
            }
            else
            {
                if (!view && !admin)
                {
                    _tcDevicesAlarmSetting.TabPages.Remove(_tpDoorEnvironmentAlarms);
                    return;
                }

                _tpDoorEnvironmentAlarms.Enabled = admin;
            }
        }

        private void HideDisableTabPageCrAlarms(bool view, bool admin)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool, bool>(HideDisableTabPageCrAlarms),
                    view,
                    admin);
            }
            else
            {
                if (!view && !admin)
                {
                    _tcDevicesAlarmSetting.TabPages.Remove(_tpCrAlarms);
                    return;
                }

                _tpCrAlarms.Enabled = admin;
            }
        }

        private void HideDisableTabPageInvalidPinGinRetriesLimits(bool view, bool admin)
        {
            if (InvokeRequired)
            {
                Invoke(
                    new Action<bool, bool>(HideDisableTabPageInvalidPinGinRetriesLimits),
                    view,
                    admin);
            }
            else
            {
                if (!view && !admin)
                {
                    _tcDevicesAlarmSetting.TabPages.Remove(_tpInvalidPinGinRetriesLimits);
                    return;
                }

                _tpInvalidPinGinRetriesLimits.Enabled = admin;
            }
        }

        private void HideTabPagePersons(bool view)
        {
            if (InvokeRequired)
            {
                Invoke(
                    new Action<bool>(HideTabPagePersons),
                    view);
            }
            else
            {
                if (!view)
                {
                    _tcDevicesAlarmSetting.TabPages.Remove(_tpPersons);
                    return;
                }
            }
        }

        private void SetEnableForApply()
        {
            _bApply.Enabled = _textChanged;
        }

        protected override bool Compare(DevicesAlarmSetting obj1, DevicesAlarmSetting obj2)
        {
            return obj1.Compare(obj2);
        }

        protected override bool CombareByGuid(DevicesAlarmSetting obj, Guid idObj)
        {
            return obj.IdDevicesAlarmSetting == idObj;
        }

        #region ShowData

        protected override void ModifyGridView(BindingSource bindingSource)
        {
            Exception error;
            var list = Plugin.MainServerProvider.DevicesAlarmSettings.SelectByCriteria(FilterSettings, out error);
            if (list != null && list.Count == 1)
            {
                _devicesAlarmSetting = Plugin.MainServerProvider.DevicesAlarmSettings.GetObjectForEdit(list.ElementAt(0).IdDevicesAlarmSetting, out error);
            }
            else
            {
                var das =
                    new DevicesAlarmSetting
                    {
                        Name = "DeviceAlarmSetting",
                        AlarmCCUOffline = true,
                        AlarmDCUOffline = true,
                        AlarmCROffline = true,
                        AlarmCCUTamperSabotage = true,
                        AlarmDCUTamperSabotage = true,
                        AlarmCRTamperSabotage = true,
                        AlarmCCUClockUnsynchronized = true,
                        AllowAAToCRsReporting = true,
                        AlarmInvalidPinRetriesLimitReached = true,
                        AlarmInvalidGinRetriesLimitReached = true,
                        InvalidPinRetriesLimitEnabled = true,
                        InvalidPinRetriesCount = 3,
                        InvalidPinRetriesLimitReachedTimeout = 5,
                        InvalidGinRetriesLimitEnabled = true,
                        InvalidGinRetriesCount = 3,
                        InvalidGinRetriesLimitReachedTimeout = 5,
                        AlarmCcuCatUnreachable = true,
                        AlarmCcuTransferToArcTimedOut = true,
                        SecurityLevelForEnterToMenu = (byte)SecurityLevel.CARDPIN
                    };
                Plugin.MainServerProvider.DevicesAlarmSettings.Insert(ref das, out error);
                _devicesAlarmSetting = Plugin.MainServerProvider.DevicesAlarmSettings.GetObjectById(das.IdDevicesAlarmSetting);
            }
            ShowDeviceAlarmSetting();

            // SB
            LoadPersonAttributes();
            LoadDoorEnvoromentReportsSettings()
;        }

        private void ShowDeviceAlarmSetting()
        {
            if (_devicesAlarmSetting == null) return;
            if (_textChanged) return;

            var alarmArcsByAlarmType = new SyncDictionary<AlarmType, ICollection<AlarmArc>>();

            foreach (var devicesAlarmSettingAlarmArc in _devicesAlarmSetting.DevicesAlarmSettingAlarmArcs)
            {
                var alarmArc = devicesAlarmSettingAlarmArc.AlarmArc;

                alarmArcsByAlarmType.GetOrAddValue(
                    (AlarmType)devicesAlarmSettingAlarmArc.AlarmType,
                    key =>
                        new LinkedList<AlarmArc>(),
                    (key, value, newlyAdded) =>
                        value.Add(alarmArc));
            }

            //CCU alarms
            _catsCcuOffline.AlarmEnabledChecked = _devicesAlarmSetting.AlarmCCUOffline;
            _catsCcuOffline.BlockAlarmChecked = _devicesAlarmSetting.BlockAlarmCCUOffline;
            _catsCcuOffline.ObjectForBlockingAlarm = GetBlockAlarmObject(
                _devicesAlarmSetting.ObjBlockAlarmCCUOfflineObjectType,
                _devicesAlarmSetting.ObjBlockAlarmCCUOfflineId);
            _catsCcuOffline.PresentationGroup = _devicesAlarmSetting.AlarmCCUOfflinePresentationGroup;
            _cmaaCcuOffline.AlarmArcs = GetAlarmArcs(
                alarmArcsByAlarmType,
                AlarmType.CCU_Offline);

            _catsCcuUnconfigured.AlarmEnabledChecked = _devicesAlarmSetting.AlarmCCUUnconfigured;
            _catsCcuUnconfigured.BlockAlarmChecked = _devicesAlarmSetting.BlockAlarmCCUUnconfigured;
            _catsCcuUnconfigured.ObjectForBlockingAlarm = GetBlockAlarmObject(
                _devicesAlarmSetting.ObjBlockAlarmCCUUnconfiguredObjectType,
                _devicesAlarmSetting.ObjBlockAlarmCCUUnconfiguredId);
            _catsCcuUnconfigured.PresentationGroup = _devicesAlarmSetting.AlarmCCUUnconfiguredPresentationGroup;
            _cmaaCcuUnconfigured.AlarmArcs = GetAlarmArcs(
                alarmArcsByAlarmType,
                AlarmType.CCU_Unconfigured);

            _catsCcuClockUnsynchronized.AlarmEnabledChecked = _devicesAlarmSetting.AlarmCCUClockUnsynchronized;
            _catsCcuClockUnsynchronized.BlockAlarmChecked = _devicesAlarmSetting.BlockAlarmCCUClockUnsynchronized;
            _catsCcuClockUnsynchronized.ObjectForBlockingAlarm = GetBlockAlarmObject(
                _devicesAlarmSetting.ObjBlockAlarmCCUClockUnsynchronizedObjectType,
                _devicesAlarmSetting.ObjBlockAlarmCCUClockUnsynchronizedId);
            _catsCcuClockUnsynchronized.PresentationGroup =
                _devicesAlarmSetting.AlarmCCUClockUnsynchronizedPresentationGroup;
            _cmaaCcuClockUnsynchronized.AlarmArcs = GetAlarmArcs(
                alarmArcsByAlarmType,
                AlarmType.CCU_ClockUnsynchronized);

            _catsCcuTamperSabotage.AlarmEnabledChecked = _devicesAlarmSetting.AlarmCCUTamperSabotage;
            _catsCcuTamperSabotage.BlockAlarmChecked = _devicesAlarmSetting.BlockAlarmCCUTamperSabotage;
            _catsCcuTamperSabotage.EventlogDuringBlockedAlarmChecked =
                _devicesAlarmSetting.EventlogDuringBlockAlarmCCUTamperSabotage;
            _catsCcuTamperSabotage.ObjectForBlockingAlarm = GetBlockAlarmObject(
                _devicesAlarmSetting.ObjBlockAlarmCCUTamperSabotageObjectType,
                _devicesAlarmSetting.ObjBlockAlarmCCUTamperSabotageId);
            _catsCcuTamperSabotage.PresentationGroup = _devicesAlarmSetting.AlarmCCUTamperSabotagePresentationGroup;
            _cmaaCcuTamperSabotage.AlarmArcs = GetAlarmArcs(
                alarmArcsByAlarmType,
                AlarmType.CCU_TamperSabotage);

            _catsCcuPrimaryPowerMissing.AlarmEnabledChecked = _devicesAlarmSetting.AlarmPrimaryPowerMissing;
            _catsCcuPrimaryPowerMissing.BlockAlarmChecked = _devicesAlarmSetting.BlockAlarmPrimaryPowerMissing;
            _catsCcuPrimaryPowerMissing.EventlogDuringBlockedAlarmChecked =
                _devicesAlarmSetting.EventlogDuringBlockAlarmPrimaryPowerMissing;
            _catsCcuPrimaryPowerMissing.ObjectForBlockingAlarm = GetBlockAlarmObject(
                _devicesAlarmSetting.ObjBlockAlarmPrimaryPowerMissingObjectType,
                _devicesAlarmSetting.ObjBlockAlarmPrimaryPowerMissingId);
            _catsCcuPrimaryPowerMissing.PresentationGroup =
                _devicesAlarmSetting.AlarmCCUPrimaryPowerMissingPresentationGroup;
            _cmaaCcuPrimaryPowerMissing.AlarmArcs = GetAlarmArcs(
                alarmArcsByAlarmType,
                AlarmType.CCU_PrimaryPowerMissing);

            _catsCcuBatteryIsLow.AlarmEnabledChecked = _devicesAlarmSetting.AlarmBatteryIsLow;
            _catsCcuBatteryIsLow.BlockAlarmChecked = _devicesAlarmSetting.BlockAlarmBatteryIsLow;
            _catsCcuBatteryIsLow.EventlogDuringBlockedAlarmChecked =
                _devicesAlarmSetting.EventlogDuringBlockAlarmBatteryIsLow;
            _catsCcuBatteryIsLow.ObjectForBlockingAlarm = GetBlockAlarmObject(
                _devicesAlarmSetting.ObjBlockAlarmBatteryIsLowObjectType,
                _devicesAlarmSetting.ObjBlockAlarmBatteryIsLowId);
            _catsCcuBatteryIsLow.PresentationGroup = _devicesAlarmSetting.AlarmCCUBatteryIsLowPresentationGroup;
            _cmaaCcuBatteryIsLow.AlarmArcs = GetAlarmArcs(
                alarmArcsByAlarmType,
                AlarmType.CCU_BatteryLow);

            _catsCcuUpsOutputFuse.AlarmEnabledChecked = _devicesAlarmSetting.AlarmUpsOutputFuse;
            _catsCcuUpsOutputFuse.BlockAlarmChecked = _devicesAlarmSetting.BlockAlarmUpsOutputFuse;
            _catsCcuUpsOutputFuse.EventlogDuringBlockedAlarmChecked =
                _devicesAlarmSetting.EventlogDuringBlockAlarmUpsOutputFuse;
            _catsCcuUpsOutputFuse.ObjectForBlockingAlarm = GetBlockAlarmObject(
                _devicesAlarmSetting.ObjBlockAlarmUpsOutputFuseObjectType,
                _devicesAlarmSetting.ObjBlockAlarmUpsOutputFuseId);
            _catsCcuUpsOutputFuse.PresentationGroup = _devicesAlarmSetting.AlarmCcuUpsOutputFusePresentationGroup;
            _cmaaCcuUpsOutputFuse.AlarmArcs = GetAlarmArcs(
                alarmArcsByAlarmType,
                AlarmType.Ccu_Ups_OutputFuse);

            _catsCcuUpsBatteryFault.AlarmEnabledChecked = _devicesAlarmSetting.AlarmUpsBatteryFault;
            _catsCcuUpsBatteryFault.BlockAlarmChecked = _devicesAlarmSetting.BlockAlarmUpsBatteryFault;
            _catsCcuUpsBatteryFault.EventlogDuringBlockedAlarmChecked =
                _devicesAlarmSetting.EventlogDuringBlockAlarmUpsBatteryFault;
            _catsCcuUpsBatteryFault.ObjectForBlockingAlarm = GetBlockAlarmObject(
                _devicesAlarmSetting.ObjBlockAlarmUpsBatteryFaultObjectType,
                _devicesAlarmSetting.ObjBlockAlarmUpsBatteryFaultId);
            _catsCcuUpsBatteryFault.PresentationGroup =
                _devicesAlarmSetting.AlarmCcuUpsBatteryFaultPresentationGroup;
            _cmaaCcuUpsBatteryFault.AlarmArcs = GetAlarmArcs(
                alarmArcsByAlarmType,
                AlarmType.Ccu_Ups_BatteryFault);

            _catsCcuUpsBatteryFuse.AlarmEnabledChecked = _devicesAlarmSetting.AlarmUpsBatteryFuse;
            _catsCcuUpsBatteryFuse.BlockAlarmChecked = _devicesAlarmSetting.BlockAlarmUpsBatteryFuse;
            _catsCcuUpsBatteryFuse.EventlogDuringBlockedAlarmChecked =
                _devicesAlarmSetting.EventlogDuringBlockAlarmUpsBatteryFuse;
            _catsCcuUpsBatteryFuse.ObjectForBlockingAlarm = GetBlockAlarmObject(
                _devicesAlarmSetting.ObjBlockAlarmUpsBatteryFuseObjectType,
                _devicesAlarmSetting.ObjBlockAlarmUpsBatteryFuseId);
            _catsCcuUpsBatteryFuse.PresentationGroup = _devicesAlarmSetting.AlarmCcuUpsBatteryFusePresentationGroup;
            _cmaaCcuUpsBatteryFuse.AlarmArcs = GetAlarmArcs(
                alarmArcsByAlarmType,
                AlarmType.Ccu_Ups_BatteryFuse);

            _catsCcuUpsOvertemperature.AlarmEnabledChecked = _devicesAlarmSetting.AlarmUpsOvertemperature;
            _catsCcuUpsOvertemperature.BlockAlarmChecked = _devicesAlarmSetting.BlockAlarmUpsOvertemperature;
            _catsCcuUpsOvertemperature.EventlogDuringBlockedAlarmChecked =
                _devicesAlarmSetting.EventlogDuringBlockAlarmUpsOvertemperature;
            _catsCcuUpsOvertemperature.ObjectForBlockingAlarm = GetBlockAlarmObject(
                _devicesAlarmSetting.ObjBlockAlarmUpsOvertemperatureObjectType,
                _devicesAlarmSetting.ObjBlockAlarmUpsOvertemperatureId);
            _catsCcuUpsOvertemperature.PresentationGroup =
                _devicesAlarmSetting.AlarmCcuUpsOvertemperaturePresentationGroup;
            _cmaaCcuUpsOvertemperature.AlarmArcs = GetAlarmArcs(
                alarmArcsByAlarmType,
                AlarmType.Ccu_Ups_Overtemperature);

            _catsCcuUpsTamper.AlarmEnabledChecked = _devicesAlarmSetting.AlarmUpsTamperSabotage;
            _catsCcuUpsTamper.BlockAlarmChecked = _devicesAlarmSetting.BlockAlarmUpsTamperSabotage;
            _catsCcuUpsTamper.EventlogDuringBlockedAlarmChecked =
                _devicesAlarmSetting.EventlogDuringBlockAlarmUpsTamperSabotage;
            _catsCcuUpsTamper.ObjectForBlockingAlarm = GetBlockAlarmObject(
                _devicesAlarmSetting.ObjBlockAlarmUpsTamperSabotageObjectType,
                _devicesAlarmSetting.ObjBlockAlarmUpsTamperSabotageId);
            _catsCcuUpsTamper.PresentationGroup = _devicesAlarmSetting.AlarmCcuUpsTamperSabotagePresentationGroup;
            _cmaaCcuUpsTamper.AlarmArcs = GetAlarmArcs(
                alarmArcsByAlarmType,
                AlarmType.Ccu_Ups_TamperSabotage);

            _catsCcuFuseOnExtensionBoard.AlarmEnabledChecked = _devicesAlarmSetting.AlarmFuseOnExtensionBoard;
            _catsCcuFuseOnExtensionBoard.BlockAlarmChecked = _devicesAlarmSetting.BlockAlarmFuseOnExtensionBoard;
            _catsCcuFuseOnExtensionBoard.EventlogDuringBlockedAlarmChecked =
                _devicesAlarmSetting.EventlogDuringBlockAlarmFuseOnExtensionBoard;
            _catsCcuFuseOnExtensionBoard.ObjectForBlockingAlarm = GetBlockAlarmObject(
                _devicesAlarmSetting.ObjBlockAlarmFuseOnExtensionBoardObjectType,
                _devicesAlarmSetting.ObjBlockAlarmFuseOnExtensionBoardId);
            _catsCcuFuseOnExtensionBoard.PresentationGroup =
                _devicesAlarmSetting.AlarmCCUFuseOnExtensionBoardPresentationGroup;
            _cmaaCcuFuseOnExtensionBoard.AlarmArcs = GetAlarmArcs(
                alarmArcsByAlarmType,
                AlarmType.CCU_ExtFuse);

            _cmaaCcuOutdatedFirmware.AlarmArcs = GetAlarmArcs(
                alarmArcsByAlarmType,
                AlarmType.CCU_OutdatedFirmware);

            _cmaaCcuDataChannelDistrupted.AlarmArcs = GetAlarmArcs(
                alarmArcsByAlarmType,
                AlarmType.CCU_DataChannelDistrupted);

            _cmaaCcuCoprocessorFailure.AlarmArcs = GetAlarmArcs(
                alarmArcsByAlarmType,
                AlarmType.CCU_CoprocessorFailure);

            _cmaaCcuHighMemoryLoad.AlarmArcs = GetAlarmArcs(
                alarmArcsByAlarmType,
                AlarmType.CCU_HighMemoryLoad);

            _cmaaCcuFilesystemProblem.AlarmArcs = GetAlarmArcs(
                alarmArcsByAlarmType,
                AlarmType.CCU_FilesystemProblem);

            _cmaaCcuSdCardNotFound.AlarmArcs = GetAlarmArcs(
                alarmArcsByAlarmType,
                AlarmType.CCU_SdCardNotFound);

            _cmaaICcuSendingOfObjectStateFailed.AlarmArcs = GetAlarmArcs(
                alarmArcsByAlarmType,
                AlarmType.ICCU_SendingOfObjectStateFailed);

            _cmaaICcuPortAlreadyUsed.AlarmArcs = GetAlarmArcs(
                alarmArcsByAlarmType,
                AlarmType.ICCU_PortAlreadyUsed);

            _catsCcuCatUnreachable.PresentationGroup =
                _devicesAlarmSetting.AlarmCcuCatUnreachablePresentationGroup;

            _catsCcuTransferToArcTimedOut.PresentationGroup =
                _devicesAlarmSetting.AlarmCcuTransferToArcTimedOutPresentationGroup;

            _catsCcuCatUnreachable.AlarmEnabledChecked = _devicesAlarmSetting.AlarmCcuCatUnreachable;

            _catsCcuTransferToArcTimedOut.AlarmEnabledChecked = _devicesAlarmSetting.AlarmCcuTransferToArcTimedOut;

            //DCU alarms
            _catsDcuOffline.AlarmEnabledChecked = _devicesAlarmSetting.AlarmDCUOffline;
            _catsDcuOffline.BlockAlarmChecked = _devicesAlarmSetting.BlockAlarmDCUOffline;
            _catsDcuOffline.ObjectForBlockingAlarm = GetBlockAlarmObject(
                _devicesAlarmSetting.ObjBlockAlarmDCUOfflineObjectType,
                _devicesAlarmSetting.ObjBlockAlarmDCUOfflineId);
            _catsDcuOffline.PresentationGroup = _devicesAlarmSetting.AlarmDCUOfflinePresentationGroup;
            _cmaaDcuOffline.AlarmArcs = GetAlarmArcs(
                alarmArcsByAlarmType,
                AlarmType.DCU_Offline);

            _cmaaDcuOfflineDueCcuOffline.AlarmArcs = GetAlarmArcs(
                alarmArcsByAlarmType,
                AlarmType.DCU_Offline_Due_CCU_Offline);

            _catsDcuTamperSabotage.AlarmEnabledChecked = _devicesAlarmSetting.AlarmDCUTamperSabotage;
            _catsDcuTamperSabotage.BlockAlarmChecked = _devicesAlarmSetting.BlockAlarmDCUTamperSabotage;
            _catsDcuTamperSabotage.ObjectForBlockingAlarm = GetBlockAlarmObject(
                _devicesAlarmSetting.ObjBlockAlarmDCUTamperSabotageObjectType,
                _devicesAlarmSetting.ObjBlockAlarmDCUTamperSabotageId);
            _catsDcuTamperSabotage.EventlogDuringBlockedAlarmChecked =
                _devicesAlarmSetting.EventlogDuringBlockAlarmDCUTamperSabotage;
            _catsDcuTamperSabotage.PresentationGroup = _devicesAlarmSetting.AlarmDCUTamperSabotagePresentationGroup;
            _cmaaDcuTamperSabotage.AlarmArcs = GetAlarmArcs(
                alarmArcsByAlarmType,
                AlarmType.DCU_TamperSabotage);

            _cmaaDcuOutdatedFirmware.AlarmArcs = GetAlarmArcs(
                alarmArcsByAlarmType,
                AlarmType.DCU_OutdatedFirmware);

            //DSM alarms
            _catsDsmDoorAjar.AlarmEnabledChecked = _devicesAlarmSetting.AlarmDEDoorAjar;
            _catsDsmDoorAjar.BlockAlarmChecked = _devicesAlarmSetting.BlockAlarmDEDoorAjar;
            _catsDsmDoorAjar.ObjectForBlockingAlarm = GetBlockAlarmObject(
                _devicesAlarmSetting.ObjBlockAlarmDEDoorAjarObjectType,
                _devicesAlarmSetting.ObjBlockAlarmDEDoorAjarId);
            _catsDsmDoorAjar.PresentationGroup = _devicesAlarmSetting.AlarmDEDoorAjarPresentationGroup;
            _cmaaDsmDoorAjar.AlarmArcs = GetAlarmArcs(
                alarmArcsByAlarmType,
                AlarmType.DoorEnvironment_DoorAjar);

            _catsDsmIntrusion.AlarmEnabledChecked = _devicesAlarmSetting.AlarmDEIntrusion;
            _catsDsmIntrusion.BlockAlarmChecked = _devicesAlarmSetting.BlockAlarmDEIntrusion;
            _catsDsmIntrusion.ObjectForBlockingAlarm = GetBlockAlarmObject(
                _devicesAlarmSetting.ObjBlockAlarmDEIntrusionObjectType,
                _devicesAlarmSetting.ObjBlockAlarmDEIntrusionId);
            _catsDsmIntrusion.PresentationGroup = _devicesAlarmSetting.AlarmDEIntrusionPresentationGroup;
            _cmaaDsmIntrusion.AlarmArcs = GetAlarmArcs(
                alarmArcsByAlarmType,
                AlarmType.DoorEnvironment_Intrusion);

            _catsDsmSabotage.AlarmEnabledChecked = _devicesAlarmSetting.AlarmDESabotage;
            _catsDsmSabotage.BlockAlarmChecked = _devicesAlarmSetting.BlockAlarmDESabotage;
            _catsDsmSabotage.ObjectForBlockingAlarm = GetBlockAlarmObject(
                _devicesAlarmSetting.ObjBlockAlarmDESabotageObjectType,
                _devicesAlarmSetting.ObjBlockAlarmDESabotageId);
            _catsDsmSabotage.PresentationGroup = _devicesAlarmSetting.AlarmDESabotagePresentationGroup;
            _cmaaDsmSabotage.AlarmArcs = GetAlarmArcs(
                alarmArcsByAlarmType,
                AlarmType.DoorEnvironment_Sabotage);

            //CR SL for enter to menu
            AddSlForEnterToMenu();

            if (!string.IsNullOrEmpty(_devicesAlarmSetting.GinForEnterToMenu))
            {
                _settingDefaultPassword = true;
                _eGinForEnterToMenu.Text = DEFAULT_PASSWORD;
                _settingDefaultPassword = false;
            }

            if (_devicesAlarmSetting.SecurityDailyPlanForEnterToMenu != null)
            {
                _actSecurityDailyPlanForEnterToMenu = _devicesAlarmSetting.SecurityDailyPlanForEnterToMenu;
                _tbmSecurityDailyPlanTimeZoneForEnterToMenu.Text = _actSecurityDailyPlanForEnterToMenu.ToString();
                _tbmSecurityDailyPlanTimeZoneForEnterToMenu.TextImage =
                    Plugin.GetImageForAOrmObject(_actSecurityDailyPlanForEnterToMenu);
            }

            if (_devicesAlarmSetting.SecurityTimeZoneForEnterToMenu != null)
            {
                _actSecurityTimeZoneForEnterToMenu = _devicesAlarmSetting.SecurityTimeZoneForEnterToMenu;
                _tbmSecurityDailyPlanTimeZoneForEnterToMenu.Text = _actSecurityTimeZoneForEnterToMenu.ToString();
                _tbmSecurityDailyPlanTimeZoneForEnterToMenu.TextImage =
                    Plugin.GetImageForAOrmObject(_actSecurityTimeZoneForEnterToMenu);
            }

            //CR alarms
            _catsCrOffline.AlarmEnabledChecked = _devicesAlarmSetting.AlarmCROffline;
            _catsCrOffline.BlockAlarmChecked = _devicesAlarmSetting.BlockAlarmCROffline;
            _catsCrOffline.ObjectForBlockingAlarm = GetBlockAlarmObject(
                _devicesAlarmSetting.ObjBlockAlarmCROfflineObjectType,
                _devicesAlarmSetting.ObjBlockAlarmCROfflineId);
            _catsCrOffline.PresentationGroup = _devicesAlarmSetting.AlarmCROfflinePresentationGroup;
            _cmaaCrOffline.AlarmArcs = GetAlarmArcs(
                alarmArcsByAlarmType,
                AlarmType.CardReader_Offline);

            _cmaaCrOfflineDueCcuOffline.AlarmArcs = GetAlarmArcs(
                alarmArcsByAlarmType,
                AlarmType.CardReader_Offline_Due_CCU_Offline);

            _catsCrTamperSabotage.AlarmEnabledChecked = _devicesAlarmSetting.AlarmCRTamperSabotage;
            _catsCrTamperSabotage.BlockAlarmChecked = _devicesAlarmSetting.BlockAlarmCRTamperSabotage;
            _catsCrTamperSabotage.EventlogDuringBlockedAlarmChecked =
                _devicesAlarmSetting.EventlogDuringBlockAlarmCRTamperSabotage;
            _catsCrTamperSabotage.ObjectForBlockingAlarm = GetBlockAlarmObject(
                _devicesAlarmSetting.ObjBlockAlarmCRTamperSabotageObjectType,
                _devicesAlarmSetting.ObjBlockAlarmCRTamperSabotageId);
            _catsCrTamperSabotage.PresentationGroup = _devicesAlarmSetting.AlarmCRTamperSabotagePresentationGroup;
            _cmaaCrTamperSabotage.AlarmArcs = GetAlarmArcs(
                alarmArcsByAlarmType,
                AlarmType.CardReader_TamperSabotage);

            _catsCrAccessDenied.AlarmEnabledChecked = _devicesAlarmSetting.AlarmCrAccessDenied;
            _catsCrAccessDenied.BlockAlarmChecked = _devicesAlarmSetting.BlockAlarmCrAccessDenied;
            _catsCrAccessDenied.EventlogDuringBlockedAlarmChecked =
                _devicesAlarmSetting.EventlogDuringBlockAlarmCrAccessDenied;
            _catsCrAccessDenied.ObjectForBlockingAlarm = GetBlockAlarmObject(
                _devicesAlarmSetting.ObjBlockAlarmCrAccessDeniedObjectType,
                _devicesAlarmSetting.ObjBlockAlarmCrAccessDeniedId);
            _catsCrAccessDenied.PresentationGroup = _devicesAlarmSetting.AlarmCRAccessDeniedPresentationGroup;
            _cmaaCrAccessDenied.AlarmArcs = GetAlarmArcs(
                alarmArcsByAlarmType,
                AlarmType.CardReader_AccessDenied);

            _catsCrUnknownCard.AlarmEnabledChecked = _devicesAlarmSetting.AlarmCrUnknownCard;
            _catsCrUnknownCard.BlockAlarmChecked = _devicesAlarmSetting.BlockAlarmCrUnknownCard;
            _catsCrUnknownCard.EventlogDuringBlockedAlarmChecked =
                _devicesAlarmSetting.EventlogDuringBlockAlarmCrUnknownCard;
            _catsCrUnknownCard.ObjectForBlockingAlarm = GetBlockAlarmObject(
                _devicesAlarmSetting.ObjBlockAlarmCrUnknownCardObjectType,
                _devicesAlarmSetting.ObjBlockAlarmCrUnknownCardId);
            _catsCrUnknownCard.PresentationGroup = _devicesAlarmSetting.AlarmCRUnknownCardPresentationGroup;
            _cmaaCrUnknownCard.AlarmArcs = GetAlarmArcs(
                alarmArcsByAlarmType,
                AlarmType.CardReader_UnknownCard);

            _catsCrCardBlockedOrInactive.AlarmEnabledChecked = _devicesAlarmSetting.AlarmCrCardBlockedOrInactive;
            _catsCrCardBlockedOrInactive.BlockAlarmChecked = _devicesAlarmSetting.BlockAlarmCrCardBlockedOrInactive;
            _catsCrCardBlockedOrInactive.EventlogDuringBlockedAlarmChecked =
                _devicesAlarmSetting.EventlogDuringBlockAlarmCrCardBlockedOrInactive;
            _catsCrCardBlockedOrInactive.ObjectForBlockingAlarm = GetBlockAlarmObject(
                _devicesAlarmSetting.ObjBlockAlarmCrCardBlockedOrInactiveObjectType,
                _devicesAlarmSetting.ObjBlockAlarmCrCardBlockedOrInactiveId);
            _catsCrCardBlockedOrInactive.PresentationGroup =
                _devicesAlarmSetting.AlarmCRCardBlockedOrInactivePresentationGroup;
            _cmaaCrCardBlockedOrInactive.AlarmArcs = GetAlarmArcs(
                alarmArcsByAlarmType,
                AlarmType.CardReader_CardBlockedOrInactive);

            _catsCrInvalidPin.AlarmEnabledChecked = _devicesAlarmSetting.AlarmCrInvalidPin;
            _catsCrInvalidPin.BlockAlarmChecked = _devicesAlarmSetting.BlockAlarmCrInvalidPin;
            _catsCrInvalidPin.EventlogDuringBlockedAlarmChecked =
                _devicesAlarmSetting.EventlogDuringBlockAlarmCrInvalidPin;
            _catsCrInvalidPin.ObjectForBlockingAlarm = GetBlockAlarmObject(
                _devicesAlarmSetting.ObjBlockAlarmCrInvalidPinObjectType,
                _devicesAlarmSetting.ObjBlockAlarmCrInvalidPinId);
            _catsCrInvalidPin.PresentationGroup = _devicesAlarmSetting.AlarmCRInvalidPINPresentationGroup;
            _cmaaCrInvalidPin.AlarmArcs = GetAlarmArcs(
                alarmArcsByAlarmType,
                AlarmType.CardReader_InvalidPIN);

            _catsCrInvalidGin.AlarmEnabledChecked = _devicesAlarmSetting.AlarmCrInvalidGin;
            _catsCrInvalidGin.BlockAlarmChecked = _devicesAlarmSetting.BlockAlarmCrInvalidGin;
            _catsCrInvalidGin.EventlogDuringBlockedAlarmChecked =
                _devicesAlarmSetting.EventlogDuringBlockAlarmCrInvalidGin;
            _catsCrInvalidGin.ObjectForBlockingAlarm = GetBlockAlarmObject(
                _devicesAlarmSetting.ObjBlockAlarmCrInvalidGinObjectType,
                _devicesAlarmSetting.ObjBlockAlarmCrInvalidGinId);
            _catsCrInvalidGin.PresentationGroup = _devicesAlarmSetting.AlarmCRInvalidGINPresentationGroup;
            _cmaaCrInvalidGin.AlarmArcs = GetAlarmArcs(
                alarmArcsByAlarmType,
                AlarmType.CardReader_InvalidCode);

            _catsCrInvalidEmergencyCode.AlarmEnabledChecked = _devicesAlarmSetting.AlarmCrInvalidEmergencyCode;
            _catsCrInvalidEmergencyCode.BlockAlarmChecked = _devicesAlarmSetting.BlockAlarmCrInvalidEmergencyCode;
            _catsCrInvalidEmergencyCode.EventlogDuringBlockedAlarmChecked =
                _devicesAlarmSetting.EventlogDuringBlockAlarmCrInvalidEmergencyCode;
            _catsCrInvalidEmergencyCode.ObjectForBlockingAlarm = GetBlockAlarmObject(
                _devicesAlarmSetting.ObjBlockAlarmCrInvalidEmergencyCodeObjectType,
                _devicesAlarmSetting.ObjBlockAlarmCrInvalidEmergencyCodeId);
            _catsCrInvalidEmergencyCode.PresentationGroup =
                _devicesAlarmSetting.AlarmCRInvalidEmergencyCodePresentationGroup;
            _cmaaCrInvalidEmergencyCode.AlarmArcs = GetAlarmArcs(
                alarmArcsByAlarmType,
                AlarmType.CardReader_InvalidEmergencyCode);

            _catsCrAccessPermitted.AlarmEnabledChecked = _devicesAlarmSetting.AlarmCrAccessPermitted;
            _catsCrAccessPermitted.BlockAlarmChecked = _devicesAlarmSetting.BlockAlarmCrAccessPermitted;
            _catsCrAccessPermitted.ObjectForBlockingAlarm = GetBlockAlarmObject(
                _devicesAlarmSetting.ObjBlockAlarmCrAccessPermittedObjectType,
                _devicesAlarmSetting.ObjBlockAlarmCrAccessPermittedId);
            _catsCrAccessPermitted.PresentationGroup = _devicesAlarmSetting.AlarmCRAccessPermittedPresentationGroup;
            _cmaaCrAccessPermitted.AlarmArcs = GetAlarmArcs(
                alarmArcsByAlarmType,
                AlarmType.CardReader_AccessPermitted);

            _catsCrInvalidPinRetriesLimitReached.AlarmEnabledChecked =
                _devicesAlarmSetting.AlarmInvalidPinRetriesLimitReached;
            _catsCrInvalidPinRetriesLimitReached.BlockAlarmChecked =
                _devicesAlarmSetting.BlockAlarmInvalidPinRetriesLimitReached;
            _catsCrInvalidPinRetriesLimitReached.ObjectForBlockingAlarm = GetBlockAlarmObject(
                _devicesAlarmSetting.ObjBlockAlarmInvalidPinRetriesLimitReachedObjectType,
                _devicesAlarmSetting.ObjBlockAlarmInvalidPinRetriesLimitReachedId);
            _catsCrInvalidPinRetriesLimitReached.PresentationGroup =
                _devicesAlarmSetting.PgAlarmInvalidPinRetriesLimitReached;
            _cmaaCrInvalidPinRetriesLimitReached.AlarmArcs = GetAlarmArcs(
                alarmArcsByAlarmType,
                AlarmType.CardReader_Invalid_Pin_Retries_Limit_Reached);

            _catsCrInvalidGinRetriesLimitReached.AlarmEnabledChecked =
                _devicesAlarmSetting.AlarmInvalidGinRetriesLimitReached;
            _catsCrInvalidGinRetriesLimitReached.BlockAlarmChecked =
                _devicesAlarmSetting.BlockAlarmInvalidGinRetriesLimitReached;
            _catsCrInvalidGinRetriesLimitReached.ObjectForBlockingAlarm = GetBlockAlarmObject(
                _devicesAlarmSetting.ObjBlockAlarmInvalidGinRetriesLimitReachedObjectType,
                _devicesAlarmSetting.ObjBlockAlarmInvalidGinRetriesLimitReachedId);
            _catsCrInvalidGinRetriesLimitReached.PresentationGroup =
                _devicesAlarmSetting.PgAlarmInvalidGinRetriesLimitReached;
            _cmaaCrInvalidGinRetriesLimitReached.AlarmArcs = GetAlarmArcs(
                alarmArcsByAlarmType,
                AlarmType.CardReader_Invalid_Code_Retries_Limit_Reached);

            //AA alarms
            _cmaaAlarmReceptionCenterSettings.AlarmArcs = GetAlarmArcs(
                alarmArcsByAlarmType,
                AlarmType.AlarmArea_Alarm);

            _catsAlarmAreaAlarm.PresentationGroup =
                _devicesAlarmSetting.AlarmAreaAlarmPresentationGroup;

            _catsAlarmAreaSetByOnOffObjectFailed.AlarmEnabledChecked =
                _devicesAlarmSetting.AlarmAreaSetByOnOffObjectFailed;

            _catsAlarmAreaSetByOnOffObjectFailed.BlockAlarmChecked =
                _devicesAlarmSetting.BlockAlarmAreaSetByOnOffObjectFailed;

            _catsAlarmAreaSetByOnOffObjectFailed.ObjectForBlockingAlarm = GetBlockAlarmObject(
                _devicesAlarmSetting.ObjBlockAlarmAreaSetByOnOffObjectFailedObjectType,
                _devicesAlarmSetting.ObjBlockAlarmAreaSetByOnOffObjectFailedId);

            _catsAlarmAreaSetByOnOffObjectFailed.PresentationGroup =
                _devicesAlarmSetting.AlarmAreaSetByOnOffObjectFailedPresentationGroup;

            _catsSensorAlarm.PresentationGroup =
                _devicesAlarmSetting.SensorAlarmPresentationGroup;

            _catsSensorTamperAlarm.PresentationGroup =
                _devicesAlarmSetting.SensorTamperAlarmPresentationGroup;

            _chbAlarmAreaReportingToCRs.Checked = _devicesAlarmSetting.AllowAAToCRsReporting;

            _chbInvalidPinRetriesLimitEnabled.Checked = _devicesAlarmSetting.InvalidPinRetriesLimitEnabled;
            _eInvalidPinRetriesCount.Value = _devicesAlarmSetting.InvalidPinRetriesCount;
            _eInvalidPinRetriesLimitReachedTimeout.Value = _devicesAlarmSetting.InvalidPinRetriesLimitReachedTimeout;

            _chbInvalidGinRetriesLimitEnabled.Checked = _devicesAlarmSetting.InvalidGinRetriesLimitEnabled;
            _eInvalidGinRetriesCount.Value = _devicesAlarmSetting.InvalidGinRetriesCount;
            _eInvalidGinRetriesLimitReachedTimeout.Value = _devicesAlarmSetting.InvalidGinRetriesLimitReachedTimeout;

            _textChanged = false;
            SetEnableForApply();
        }

        private ICollection<AlarmArc> GetAlarmArcs(
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

        private void AddSlForEnterToMenu()
        {
            var list = ItemSlForEnterToMenu.GetList(
                false,
                LocalizationHelper);

            if (list != null && list.Count > 0)
            {
                _cbSlForEnterToMenu.BeginUpdate();

                _cbSlForEnterToMenu.Items.Clear();

                foreach (var itemSlForEnterToMenu in list)
                {
                    _cbSlForEnterToMenu.Items.Add(itemSlForEnterToMenu);

                    if (itemSlForEnterToMenu.SlForEntetToMenu.HasValue)
                    {
                        if (_devicesAlarmSetting.SecurityLevelForEnterToMenu ==
                            (byte)itemSlForEnterToMenu.SlForEntetToMenu)
                        {
                            _cbSlForEnterToMenu.SelectedItem = itemSlForEnterToMenu;
                        }
                    }
                }

                _cbSlForEnterToMenu.EndUpdate();
            }
        }

        protected override void RemoveGridView()
        {
        }

        #endregion

        protected override ACgpPluginEditForm<NCASClient, DevicesAlarmSetting> CreateEditForm(DevicesAlarmSetting obj, ShowOptionsEditForm showOption)
        {
            return null;
        }

        protected override void DeleteObj(DevicesAlarmSetting obj)
        {
            Exception error;
            if (!Plugin.MainServerProvider.DevicesAlarmSettings.Delete(obj, out error))
                throw error;
        }


        protected override void SetFilterSettings()
        {
        }

        protected override void ClearFilterEdits()
        {
        }

        protected override void FilterValueChanged(object sender, EventArgs e)
        {
        }

        protected override void FilterKeyDown(object sender, KeyEventArgs e)
        {
        }

        public override bool HasAccessView()
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return Plugin.MainServerProvider.DevicesAlarmSettings.HasAccessView();
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }

            return false;
        }

        private void _bSave_Click(object sender, EventArgs e)
        {
            if (SaveAlarmDeviceSettings())
            {
                Close();
            }
        }

        private void _bClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private readonly DevicesAlarmSetting _previousDevicesAlarmSettings = new DevicesAlarmSetting();

        private void SetChoosenPreviousDevicesAlarmSettings()
        {
            _previousDevicesAlarmSettings.AlarmCCUOffline = _devicesAlarmSetting.AlarmCCUOffline;
            _previousDevicesAlarmSettings.AlarmCCUUnconfigured = _devicesAlarmSetting.AlarmCCUUnconfigured;
            _previousDevicesAlarmSettings.AlarmCCUClockUnsynchronized = _devicesAlarmSetting.AlarmCCUClockUnsynchronized;
            _previousDevicesAlarmSettings.AlarmCCUTamperSabotage = _devicesAlarmSetting.AlarmCCUTamperSabotage;
            _previousDevicesAlarmSettings.AlarmDCUOffline = _devicesAlarmSetting.AlarmDCUOffline;
            _previousDevicesAlarmSettings.AlarmDCUTamperSabotage = _devicesAlarmSetting.AlarmDCUTamperSabotage;
            _previousDevicesAlarmSettings.AlarmCROffline = _devicesAlarmSetting.AlarmCROffline;
            _previousDevicesAlarmSettings.AlarmCRTamperSabotage = _devicesAlarmSetting.AlarmCRTamperSabotage;
            _previousDevicesAlarmSettings.AllowAAToCRsReporting = _devicesAlarmSetting.AllowAAToCRsReporting;
        }

        private bool SaveAlarmDeviceSettings()
        {
            SetChoosenPreviousDevicesAlarmSettings();

            if (_devicesAlarmSetting == null) return true;

            var onlyInDatabase = true;

            if (_devicesAlarmSetting.DevicesAlarmSettingAlarmArcs != null)
                _devicesAlarmSetting.DevicesAlarmSettingAlarmArcs.Clear();
            else
                _devicesAlarmSetting.DevicesAlarmSettingAlarmArcs = new List<DevicesAlarmSettingAlarmArc>();

            //CCU alarms
            if (!SaveCcuRelatedSettings(ref onlyInDatabase)) return false;

            //DCU alarms
            if (!SaveDcuRelatedSettings(ref onlyInDatabase)) return false;

            //DSM alarms
            if (!SaveDsmRelatedSettings(ref onlyInDatabase)) return false;

            //CR SL for enter to menu
            if (!SaveCrSecurityLevelRelatedSettings(ref onlyInDatabase)) return false;

            //CR alarms
            if (!SaveCrRelatedSettings(ref onlyInDatabase)) return false;

            //AA alarms
            if (!SaveAaRelatedSettings(ref onlyInDatabase)) return false;

            SaveToDatabaseDevicesAlarmSettings(onlyInDatabase);

            // SB
            SavePersonAttributes();

            SaveDoorEnvoromentReportsSettings();

            return true;
        }

        private bool SaveCrSecurityLevelRelatedSettings(ref bool onlyInDatabase)
        {
            if (_eGinForEnterToMenu.Text.Length > 0)
            {
                bool codeErrorToBeNotified = false;

                if (_eGinForEnterToMenu.Text != DEFAULT_PASSWORD)
                {

                    if (PersonEditForm.CodeHasWrongLength(_eGinForEnterToMenu.Text.Length))
                    {
                        codeErrorToBeNotified = true;

                    }
                }
                else
                {
                    if (PersonEditForm.CodeHasWrongLength(_devicesAlarmSetting.GinLengthForEnterToMenu))
                        codeErrorToBeNotified = true;
                }

                if (codeErrorToBeNotified)
                {
                    ErrorNotificationOverControl(
                            _eGinForEnterToMenu,
                            "ErrorWrongGinLength",
                            GeneralOptionsForm.Singleton.MinimalCodeLength,
                            GeneralOptionsForm.Singleton.MaximalCodeLength);

                    _tcDevicesAlarmSetting.SelectedTab = _tpCrAlarms;
                    _eGinForEnterToMenu.Focus();

                    return false;
                }
            }

            var slForEntetToMenu = ((ItemSlForEnterToMenu)_cbSlForEnterToMenu.SelectedItem).SlForEntetToMenu;
            if (slForEntetToMenu != null)
            {
                var slForEnterToMenu = (byte)slForEntetToMenu;

                if (slForEnterToMenu == (byte)SecurityLevel.SecurityTimeZoneSecurityDailyPlan)
                {
                    if (_actSecurityDailyPlanForEnterToMenu == null
                        && _actSecurityTimeZoneForEnterToMenu == null)
                    {
                        ErrorNotificationOverControl(
                            _tbmSecurityDailyPlanTimeZoneForEnterToMenu.ImageTextBox,
                            "ErrorInsertSecTimeZoneSecDailyPlan"
                            );

                        _tcDevicesAlarmSetting.SelectedTab = _tpCrAlarms;
                        _tbmSecurityDailyPlanTimeZoneForEnterToMenu.Focus();

                        return false;
                    }
                }

                if (slForEnterToMenu != _devicesAlarmSetting.SecurityLevelForEnterToMenu)
                {
                    _devicesAlarmSetting.SecurityLevelForEnterToMenu = slForEnterToMenu;
                    onlyInDatabase = false;
                }
            }


            if (ReferenceEquals(_actSecurityDailyPlanForEnterToMenu, null) !=
                ReferenceEquals(_devicesAlarmSetting.SecurityDailyPlanForEnterToMenu, null)
                || (_actSecurityDailyPlanForEnterToMenu != null &&
                    !_actSecurityDailyPlanForEnterToMenu.Compare(_devicesAlarmSetting.SecurityDailyPlanForEnterToMenu)))
            {
                _devicesAlarmSetting.SecurityDailyPlanForEnterToMenu = _actSecurityDailyPlanForEnterToMenu;
                onlyInDatabase = false;
            }

            if (ReferenceEquals(_actSecurityTimeZoneForEnterToMenu, null) !=
                ReferenceEquals(_devicesAlarmSetting.SecurityTimeZoneForEnterToMenu, null)
                || (_actSecurityTimeZoneForEnterToMenu != null &&
                    !_actSecurityTimeZoneForEnterToMenu.Compare(_devicesAlarmSetting.SecurityTimeZoneForEnterToMenu)))
            {
                _devicesAlarmSetting.SecurityTimeZoneForEnterToMenu = _actSecurityTimeZoneForEnterToMenu;
                onlyInDatabase = false;
            }

            if (!string.IsNullOrEmpty(_eGinForEnterToMenu.Text))
            {
                if (_eGinForEnterToMenu.Text != DEFAULT_PASSWORD)
                {
                    _devicesAlarmSetting.GinForEnterToMenu = QuickHashes.GetCRC32String(_eGinForEnterToMenu.Text);
                    _devicesAlarmSetting.GinLengthForEnterToMenu = (byte)_eGinForEnterToMenu.Text.Length;
                    onlyInDatabase = false;
                }
            }
            else if (_devicesAlarmSetting.GinForEnterToMenu != null)
            {
                _devicesAlarmSetting.GinForEnterToMenu = null;
                _devicesAlarmSetting.GinLengthForEnterToMenu = 0;
                onlyInDatabase = false;
            }
            return true;
        }

        private bool SaveAaRelatedSettings(ref bool onlyInDatabase)
        {
            SaveAlarmArcs(
                _cmaaAlarmReceptionCenterSettings.AlarmArcs,
                AlarmType.AlarmArea_Alarm);

            if (_cmaaAlarmReceptionCenterSettings.AlarmArcsWasChanged)
                onlyInDatabase = false;

            _devicesAlarmSetting.AlarmAreaAlarmPresentationGroup =
                _catsAlarmAreaAlarm.PresentationGroup;

            if (!_catsAlarmAreaSetByOnOffObjectFailed.ControlValues(
                control =>
                    _openAndScrollToControlByAlarmType[AlarmType.AlarmArea_SetByOnOffObjectFailed]()))
            {
                return false;
            }

            if (_devicesAlarmSetting.AlarmAreaSetByOnOffObjectFailed !=
                _catsAlarmAreaSetByOnOffObjectFailed.AlarmEnabledChecked)
            {
                _devicesAlarmSetting.AlarmAreaSetByOnOffObjectFailed =
                    _catsAlarmAreaSetByOnOffObjectFailed.AlarmEnabledChecked;
                onlyInDatabase = false;
            }

            if (_devicesAlarmSetting.BlockAlarmAreaSetByOnOffObjectFailed !=
                _catsAlarmAreaSetByOnOffObjectFailed.BlockAlarmChecked)
            {
                _devicesAlarmSetting.BlockAlarmAreaSetByOnOffObjectFailed =
                    _catsAlarmAreaSetByOnOffObjectFailed.BlockAlarmChecked;
                onlyInDatabase = false;
            }

            byte? blockObjectAlarmAreaSetByOnOffObjectFailedType = null;
            Guid? blockObjectAlarmAreaSetByOnOffObjectFailedId = null;

            if (_catsAlarmAreaSetByOnOffObjectFailed.ObjectForBlockingAlarm != null)
            {
                blockObjectAlarmAreaSetByOnOffObjectFailedId =
                    (Guid)_catsAlarmAreaSetByOnOffObjectFailed.ObjectForBlockingAlarm.GetId();

                blockObjectAlarmAreaSetByOnOffObjectFailedType =
                    (byte)_catsAlarmAreaSetByOnOffObjectFailed.ObjectForBlockingAlarm.GetObjectType();
            }

            if (_devicesAlarmSetting.ObjBlockAlarmAreaSetByOnOffObjectFailedId != blockObjectAlarmAreaSetByOnOffObjectFailedId
                ||
                _devicesAlarmSetting.ObjBlockAlarmAreaSetByOnOffObjectFailedObjectType !=
                blockObjectAlarmAreaSetByOnOffObjectFailedType)
            {
                _devicesAlarmSetting.ObjBlockAlarmAreaSetByOnOffObjectFailedId =
                    blockObjectAlarmAreaSetByOnOffObjectFailedId;

                _devicesAlarmSetting.ObjBlockAlarmAreaSetByOnOffObjectFailedObjectType =
                    blockObjectAlarmAreaSetByOnOffObjectFailedType;

                onlyInDatabase = false;
            }

            _devicesAlarmSetting.AlarmAreaSetByOnOffObjectFailedPresentationGroup =
                _catsAlarmAreaSetByOnOffObjectFailed.PresentationGroup;

            _devicesAlarmSetting.SensorAlarmPresentationGroup =
                _catsSensorAlarm.PresentationGroup;

            _devicesAlarmSetting.SensorTamperAlarmPresentationGroup =
                _catsSensorTamperAlarm.PresentationGroup;

            if (_devicesAlarmSetting.AllowAAToCRsReporting != _chbAlarmAreaReportingToCRs.Checked)
            {
                _devicesAlarmSetting.AllowAAToCRsReporting = _chbAlarmAreaReportingToCRs.Checked;
                onlyInDatabase = false;
            }

            if (_devicesAlarmSetting.InvalidPinRetriesLimitEnabled != _chbInvalidPinRetriesLimitEnabled.Checked)
            {
                _devicesAlarmSetting.InvalidPinRetriesLimitEnabled = _chbInvalidPinRetriesLimitEnabled.Checked;
                onlyInDatabase = false;
            }

            var invalidPinRetriesCount = (byte)_eInvalidPinRetriesCount.Value;

            if (_devicesAlarmSetting.InvalidPinRetriesCount != invalidPinRetriesCount)
            {
                _devicesAlarmSetting.InvalidPinRetriesCount = invalidPinRetriesCount;
                onlyInDatabase = false;
            }

            var invalidPinRetriesLimitReachedTimeout = (byte)_eInvalidPinRetriesLimitReachedTimeout.Value;

            if (_devicesAlarmSetting.InvalidPinRetriesLimitReachedTimeout != invalidPinRetriesLimitReachedTimeout)
            {
                _devicesAlarmSetting.InvalidPinRetriesLimitReachedTimeout =
                    invalidPinRetriesLimitReachedTimeout;

                onlyInDatabase = false;
            }

            if (_devicesAlarmSetting.InvalidGinRetriesLimitEnabled != _chbInvalidGinRetriesLimitEnabled.Checked)
            {
                _devicesAlarmSetting.InvalidGinRetriesLimitEnabled = _chbInvalidGinRetriesLimitEnabled.Checked;
                onlyInDatabase = false;
            }

            var invalidGinRetriesCount = (byte)_eInvalidGinRetriesCount.Value;

            if (_devicesAlarmSetting.InvalidGinRetriesCount != invalidGinRetriesCount)
            {
                _devicesAlarmSetting.InvalidGinRetriesCount = invalidGinRetriesCount;
                onlyInDatabase = false;
            }

            var invalidGinRetriesLimitReachedTimeout = (byte)_eInvalidGinRetriesLimitReachedTimeout.Value;

            if (_devicesAlarmSetting.InvalidGinRetriesLimitReachedTimeout != invalidGinRetriesLimitReachedTimeout)
            {
                _devicesAlarmSetting.InvalidGinRetriesLimitReachedTimeout =
                    invalidGinRetriesLimitReachedTimeout;

                onlyInDatabase = false;
            }
            return true;
        }

        private bool SaveCrRelatedSettings(ref bool onlyInDatabase)
        {
            if (!_catsCrOffline.ControlValues(
                control =>
                    _openAndScrollToControlByAlarmType[AlarmType.CardReader_Offline]()))
            {
                return false;
            }

            if (_devicesAlarmSetting.AlarmCROffline != _catsCrOffline.AlarmEnabledChecked)
            {
                _devicesAlarmSetting.AlarmCROffline = _catsCrOffline.AlarmEnabledChecked;
                onlyInDatabase = false;
            }

            if (_devicesAlarmSetting.BlockAlarmCROffline != _catsCrOffline.BlockAlarmChecked)
            {
                _devicesAlarmSetting.BlockAlarmCROffline = _catsCrOffline.BlockAlarmChecked;
                onlyInDatabase = false;
            }

            byte? blockObjectAlarmCrOfflineType = null;
            Guid? blockObjectAlarmCrOfflineId = null;

            if (_catsCrOffline.ObjectForBlockingAlarm != null)
            {
                blockObjectAlarmCrOfflineType =
                    (byte)_catsCrOffline.ObjectForBlockingAlarm.GetObjectType();

                blockObjectAlarmCrOfflineId =
                    (Guid)_catsCrOffline.ObjectForBlockingAlarm.GetId();
            }

            if (_devicesAlarmSetting.ObjBlockAlarmCROfflineObjectType != blockObjectAlarmCrOfflineType
                || _devicesAlarmSetting.ObjBlockAlarmCROfflineId != blockObjectAlarmCrOfflineId)
            {
                _devicesAlarmSetting.ObjBlockAlarmCROfflineObjectType = blockObjectAlarmCrOfflineType;
                _devicesAlarmSetting.ObjBlockAlarmCROfflineId = blockObjectAlarmCrOfflineId;

                onlyInDatabase = false;
            }

            _devicesAlarmSetting.AlarmCROfflinePresentationGroup = _catsCrOffline.PresentationGroup;

            SaveAlarmArcs(
                _cmaaCrOffline.AlarmArcs,
                AlarmType.CardReader_Offline);

            if (_cmaaCrOffline.AlarmArcsWasChanged)
                onlyInDatabase = false;

            SaveAlarmArcs(
                _cmaaCrOfflineDueCcuOffline.AlarmArcs,
                AlarmType.CardReader_Offline_Due_CCU_Offline);

            if (!_catsCrTamperSabotage.ControlValues(
                control =>
                    _openAndScrollToControlByAlarmType[AlarmType.CardReader_TamperSabotage]()))
            {
                return false;
            }

            if (_devicesAlarmSetting.AlarmCRTamperSabotage != _catsCrTamperSabotage.AlarmEnabledChecked)
            {
                _devicesAlarmSetting.AlarmCRTamperSabotage = _catsCrTamperSabotage.AlarmEnabledChecked;
                onlyInDatabase = false;
            }

            if (_devicesAlarmSetting.BlockAlarmCRTamperSabotage != _catsCrTamperSabotage.BlockAlarmChecked)
            {
                _devicesAlarmSetting.BlockAlarmCRTamperSabotage = _catsCrTamperSabotage.BlockAlarmChecked;
                onlyInDatabase = false;
            }

            if (_devicesAlarmSetting.EventlogDuringBlockAlarmCRTamperSabotage !=
                _catsCrTamperSabotage.EventlogDuringBlockedAlarmChecked)
            {
                _devicesAlarmSetting.EventlogDuringBlockAlarmCRTamperSabotage =
                    _catsCrTamperSabotage.EventlogDuringBlockedAlarmChecked;

                onlyInDatabase = false;
            }

            byte? blockObjectAlarmCrTamperSabotageType = null;
            Guid? blockObjectAlarmCrTamperSabotageId = null;

            if (_catsCrTamperSabotage.ObjectForBlockingAlarm != null)
            {
                blockObjectAlarmCrTamperSabotageType =
                    (byte)_catsCrTamperSabotage.ObjectForBlockingAlarm.GetObjectType();

                blockObjectAlarmCrTamperSabotageId =
                    (Guid)_catsCrTamperSabotage.ObjectForBlockingAlarm.GetId();
            }

            if (_devicesAlarmSetting.ObjBlockAlarmCRTamperSabotageObjectType !=
                blockObjectAlarmCrTamperSabotageType
                || _devicesAlarmSetting.ObjBlockAlarmCRTamperSabotageId !=
                blockObjectAlarmCrTamperSabotageId)
            {
                _devicesAlarmSetting.ObjBlockAlarmCRTamperSabotageObjectType = blockObjectAlarmCrTamperSabotageType;
                _devicesAlarmSetting.ObjBlockAlarmCRTamperSabotageId = blockObjectAlarmCrTamperSabotageId;

                onlyInDatabase = false;
            }

            _devicesAlarmSetting.AlarmCRTamperSabotagePresentationGroup = _catsCrTamperSabotage.PresentationGroup;

            SaveAlarmArcs(
                _cmaaCrTamperSabotage.AlarmArcs,
                AlarmType.CardReader_TamperSabotage);

            if (_cmaaCrTamperSabotage.AlarmArcsWasChanged)
                onlyInDatabase = false;

            if (!_catsCrAccessDenied.ControlValues(
                control =>
                    _openAndScrollToControlByAlarmType[AlarmType.CardReader_AccessDenied]()))
            {
                return false;
            }

            if (_devicesAlarmSetting.AlarmCrAccessDenied != _catsCrAccessDenied.AlarmEnabledChecked)
            {
                _devicesAlarmSetting.AlarmCrAccessDenied = _catsCrAccessDenied.AlarmEnabledChecked;
                onlyInDatabase = false;
            }

            if (_devicesAlarmSetting.BlockAlarmCrAccessDenied != _catsCrAccessDenied.BlockAlarmChecked)
            {
                _devicesAlarmSetting.BlockAlarmCrAccessDenied = _catsCrAccessDenied.BlockAlarmChecked;
                onlyInDatabase = false;
            }

            if (_devicesAlarmSetting.EventlogDuringBlockAlarmCrAccessDenied !=
                _catsCrAccessDenied.EventlogDuringBlockedAlarmChecked)
            {
                _devicesAlarmSetting.EventlogDuringBlockAlarmCrAccessDenied =
                    _catsCrAccessDenied.EventlogDuringBlockedAlarmChecked;

                onlyInDatabase = false;
            }

            byte? blockObjectAlarmAccessDeniedType = null;
            Guid? blockObjectAlarmAccessDeniedId = null;

            if (_catsCrAccessDenied.ObjectForBlockingAlarm != null)
            {
                blockObjectAlarmAccessDeniedType =
                    (byte)_catsCrAccessDenied.ObjectForBlockingAlarm.GetObjectType();

                blockObjectAlarmAccessDeniedId =
                    (Guid)_catsCrAccessDenied.ObjectForBlockingAlarm.GetId();
            }

            if (_devicesAlarmSetting.ObjBlockAlarmCrAccessDeniedObjectType != blockObjectAlarmAccessDeniedType
                || _devicesAlarmSetting.ObjBlockAlarmCrAccessDeniedId != blockObjectAlarmAccessDeniedId)
            {
                _devicesAlarmSetting.ObjBlockAlarmCrAccessDeniedObjectType = blockObjectAlarmAccessDeniedType;
                _devicesAlarmSetting.ObjBlockAlarmCrAccessDeniedId = blockObjectAlarmAccessDeniedId;

                onlyInDatabase = false;
            }

            _devicesAlarmSetting.AlarmCRAccessDeniedPresentationGroup = _catsCrAccessDenied.PresentationGroup;

            SaveAlarmArcs(
                _cmaaCrAccessDenied.AlarmArcs,
                AlarmType.CardReader_AccessDenied);

            if (_cmaaCrAccessDenied.AlarmArcsWasChanged)
                onlyInDatabase = false;

            if (!_catsCrUnknownCard.ControlValues(
                control =>
                    _openAndScrollToControlByAlarmType[AlarmType.CardReader_UnknownCard]()))
            {
                return false;
            }

            if (_devicesAlarmSetting.AlarmCrUnknownCard != _catsCrUnknownCard.AlarmEnabledChecked)
            {
                _devicesAlarmSetting.AlarmCrUnknownCard = _catsCrUnknownCard.AlarmEnabledChecked;
                onlyInDatabase = false;
            }

            if (_devicesAlarmSetting.BlockAlarmCrUnknownCard != _catsCrUnknownCard.BlockAlarmChecked)
            {
                _devicesAlarmSetting.BlockAlarmCrUnknownCard = _catsCrUnknownCard.BlockAlarmChecked;
                onlyInDatabase = false;
            }

            if (_devicesAlarmSetting.EventlogDuringBlockAlarmCrUnknownCard !=
                _catsCrUnknownCard.EventlogDuringBlockedAlarmChecked)
            {
                _devicesAlarmSetting.EventlogDuringBlockAlarmCrUnknownCard =
                    _catsCrUnknownCard.EventlogDuringBlockedAlarmChecked;

                onlyInDatabase = false;
            }

            byte? blockObjectAlarmUnknownCardType = null;
            Guid? blockObjectAlarmUnknownCardId = null;

            if (_catsCrUnknownCard.ObjectForBlockingAlarm != null)
            {
                blockObjectAlarmUnknownCardType =
                    (byte)_catsCrUnknownCard.ObjectForBlockingAlarm.GetObjectType();

                blockObjectAlarmUnknownCardId =
                    (Guid)_catsCrUnknownCard.ObjectForBlockingAlarm.GetId();
            }

            if (_devicesAlarmSetting.ObjBlockAlarmCrUnknownCardObjectType != blockObjectAlarmUnknownCardType
                || _devicesAlarmSetting.ObjBlockAlarmCrUnknownCardId != blockObjectAlarmUnknownCardId)
            {
                _devicesAlarmSetting.ObjBlockAlarmCrUnknownCardObjectType = blockObjectAlarmUnknownCardType;
                _devicesAlarmSetting.ObjBlockAlarmCrUnknownCardId = blockObjectAlarmUnknownCardId;

                onlyInDatabase = false;
            }

            _devicesAlarmSetting.AlarmCRUnknownCardPresentationGroup = _catsCrUnknownCard.PresentationGroup;

            SaveAlarmArcs(
                _cmaaCrUnknownCard.AlarmArcs,
                AlarmType.CardReader_UnknownCard);

            if (_cmaaCrUnknownCard.AlarmArcsWasChanged)
                onlyInDatabase = false;

            if (!_catsCrCardBlockedOrInactive.ControlValues(
                control =>
                    _openAndScrollToControlByAlarmType[AlarmType.CardReader_CardBlockedOrInactive]()))
            {
                return false;
            }

            if (_devicesAlarmSetting.AlarmCrCardBlockedOrInactive != _catsCrCardBlockedOrInactive.AlarmEnabledChecked)
            {
                _devicesAlarmSetting.AlarmCrCardBlockedOrInactive = _catsCrCardBlockedOrInactive.AlarmEnabledChecked;
                onlyInDatabase = false;
            }

            if (_devicesAlarmSetting.BlockAlarmCrCardBlockedOrInactive != _catsCrCardBlockedOrInactive.BlockAlarmChecked)
            {
                _devicesAlarmSetting.BlockAlarmCrCardBlockedOrInactive = _catsCrCardBlockedOrInactive.BlockAlarmChecked;
                onlyInDatabase = false;
            }

            if (_devicesAlarmSetting.EventlogDuringBlockAlarmCrCardBlockedOrInactive !=
                _catsCrCardBlockedOrInactive.EventlogDuringBlockedAlarmChecked)
            {
                _devicesAlarmSetting.EventlogDuringBlockAlarmCrCardBlockedOrInactive =
                    _catsCrCardBlockedOrInactive.EventlogDuringBlockedAlarmChecked;

                onlyInDatabase = false;
            }

            byte? blockObjectAlarmCardBlockedOrInactiveType = null;
            Guid? blockObjectAlarmCardBlockedOrInactiveId = null;

            if (_catsCrCardBlockedOrInactive.ObjectForBlockingAlarm != null)
            {
                blockObjectAlarmCardBlockedOrInactiveType =
                    (byte)_catsCrCardBlockedOrInactive.ObjectForBlockingAlarm.GetObjectType();

                blockObjectAlarmCardBlockedOrInactiveId =
                    (Guid)_catsCrCardBlockedOrInactive.ObjectForBlockingAlarm.GetId();
            }

            if (_devicesAlarmSetting.ObjBlockAlarmCrCardBlockedOrInactiveObjectType !=
                blockObjectAlarmCardBlockedOrInactiveType
                || _devicesAlarmSetting.ObjBlockAlarmCrCardBlockedOrInactiveId !=
                blockObjectAlarmCardBlockedOrInactiveId)
            {
                _devicesAlarmSetting.ObjBlockAlarmCrCardBlockedOrInactiveObjectType =
                    blockObjectAlarmCardBlockedOrInactiveType;

                _devicesAlarmSetting.ObjBlockAlarmCrCardBlockedOrInactiveId =
                    blockObjectAlarmCardBlockedOrInactiveId;

                onlyInDatabase = false;
            }

            _devicesAlarmSetting.AlarmCRCardBlockedOrInactivePresentationGroup = _catsCrCardBlockedOrInactive.PresentationGroup;

            SaveAlarmArcs(
                _cmaaCrCardBlockedOrInactive.AlarmArcs,
                AlarmType.CardReader_CardBlockedOrInactive);

            if (_cmaaCrCardBlockedOrInactive.AlarmArcsWasChanged)
                onlyInDatabase = false;

            if (!_catsCrInvalidPin.ControlValues(
                control =>
                    _openAndScrollToControlByAlarmType[AlarmType.CardReader_InvalidPIN]()))
            {
                return false;
            }

            if (_devicesAlarmSetting.AlarmCrInvalidPin != _catsCrInvalidPin.AlarmEnabledChecked)
            {
                _devicesAlarmSetting.AlarmCrInvalidPin = _catsCrInvalidPin.AlarmEnabledChecked;
                onlyInDatabase = false;
            }

            if (_devicesAlarmSetting.BlockAlarmCrInvalidPin != _catsCrInvalidPin.BlockAlarmChecked)
            {
                _devicesAlarmSetting.BlockAlarmCrInvalidPin = _catsCrInvalidPin.BlockAlarmChecked;
                onlyInDatabase = false;
            }

            if (_devicesAlarmSetting.EventlogDuringBlockAlarmCrInvalidPin !=
                _catsCrInvalidPin.EventlogDuringBlockedAlarmChecked)
            {
                _devicesAlarmSetting.EventlogDuringBlockAlarmCrInvalidPin =
                    _catsCrInvalidPin.EventlogDuringBlockedAlarmChecked;

                onlyInDatabase = false;
            }

            byte? blockObjectAlarmInvalidPinType = null;
            Guid? blockObjectAlarmInvalidPinId = null;

            if (_catsCrInvalidPin.ObjectForBlockingAlarm != null)
            {
                blockObjectAlarmInvalidPinType =
                    (byte)_catsCrInvalidPin.ObjectForBlockingAlarm.GetObjectType();

                blockObjectAlarmInvalidPinId =
                    (Guid)_catsCrInvalidPin.ObjectForBlockingAlarm.GetId();
            }

            if (_devicesAlarmSetting.ObjBlockAlarmCrInvalidPinObjectType != blockObjectAlarmInvalidPinType
                || _devicesAlarmSetting.ObjBlockAlarmCrInvalidPinId != blockObjectAlarmInvalidPinId)
            {
                _devicesAlarmSetting.ObjBlockAlarmCrInvalidPinObjectType = blockObjectAlarmInvalidPinType;
                _devicesAlarmSetting.ObjBlockAlarmCrInvalidPinId = blockObjectAlarmInvalidPinId;

                onlyInDatabase = false;
            }

            _devicesAlarmSetting.AlarmCRInvalidPINPresentationGroup = _catsCrInvalidPin.PresentationGroup;

            SaveAlarmArcs(
                _cmaaCrInvalidPin.AlarmArcs,
                AlarmType.CardReader_InvalidPIN);

            if (_cmaaCrInvalidPin.AlarmArcsWasChanged)
                onlyInDatabase = false;

            if (!_catsCrInvalidGin.ControlValues(
                control =>
                    _openAndScrollToControlByAlarmType[AlarmType.CardReader_InvalidCode]()))
            {
                return false;
            }

            if (_devicesAlarmSetting.AlarmCrInvalidGin != _catsCrInvalidGin.AlarmEnabledChecked)
            {
                _devicesAlarmSetting.AlarmCrInvalidGin = _catsCrInvalidGin.AlarmEnabledChecked;
                onlyInDatabase = false;
            }

            if (_devicesAlarmSetting.BlockAlarmCrInvalidGin != _catsCrInvalidGin.BlockAlarmChecked)
            {
                _devicesAlarmSetting.BlockAlarmCrInvalidGin = _catsCrInvalidGin.BlockAlarmChecked;
                onlyInDatabase = false;
            }

            if (_devicesAlarmSetting.EventlogDuringBlockAlarmCrInvalidGin !=
                _catsCrInvalidGin.EventlogDuringBlockedAlarmChecked)
            {
                _devicesAlarmSetting.EventlogDuringBlockAlarmCrInvalidGin =
                    _catsCrInvalidGin.EventlogDuringBlockedAlarmChecked;

                onlyInDatabase = false;
            }

            byte? blockObjectAlarmInvalidGinType = null;
            Guid? blockObjectAlarmInvalidGinId = null;

            if (_catsCrInvalidGin.ObjectForBlockingAlarm != null)
            {
                blockObjectAlarmInvalidGinType =
                    (byte)_catsCrInvalidGin.ObjectForBlockingAlarm.GetObjectType();

                blockObjectAlarmInvalidGinId =
                    (Guid)_catsCrInvalidGin.ObjectForBlockingAlarm.GetId();
            }

            if (_devicesAlarmSetting.ObjBlockAlarmCrInvalidGinObjectType != blockObjectAlarmInvalidGinType
                || _devicesAlarmSetting.ObjBlockAlarmCrInvalidGinId != blockObjectAlarmInvalidGinId)
            {
                _devicesAlarmSetting.ObjBlockAlarmCrInvalidGinObjectType = blockObjectAlarmInvalidGinType;
                _devicesAlarmSetting.ObjBlockAlarmCrInvalidGinId = blockObjectAlarmInvalidGinId;

                onlyInDatabase = false;
            }

            _devicesAlarmSetting.AlarmCRInvalidGINPresentationGroup = _catsCrInvalidGin.PresentationGroup;

            SaveAlarmArcs(
                _cmaaCrInvalidGin.AlarmArcs,
                AlarmType.CardReader_InvalidCode);

            if (_cmaaCrInvalidGin.AlarmArcsWasChanged)
                onlyInDatabase = false;

            if (!_catsCrInvalidEmergencyCode.ControlValues(
                control =>
                    _openAndScrollToControlByAlarmType[AlarmType.CardReader_InvalidEmergencyCode]()))
            {
                return false;
            }

            if (_devicesAlarmSetting.AlarmCrInvalidEmergencyCode != _catsCrInvalidEmergencyCode.AlarmEnabledChecked)
            {
                _devicesAlarmSetting.AlarmCrInvalidEmergencyCode = _catsCrInvalidEmergencyCode.AlarmEnabledChecked;
                onlyInDatabase = false;
            }

            if (_devicesAlarmSetting.BlockAlarmCrInvalidEmergencyCode != _catsCrInvalidEmergencyCode.BlockAlarmChecked)
            {
                _devicesAlarmSetting.BlockAlarmCrInvalidEmergencyCode = _catsCrInvalidEmergencyCode.BlockAlarmChecked;
                onlyInDatabase = false;
            }

            if (_devicesAlarmSetting.EventlogDuringBlockAlarmCrInvalidEmergencyCode !=
                _catsCrInvalidEmergencyCode.EventlogDuringBlockedAlarmChecked)
            {
                _devicesAlarmSetting.EventlogDuringBlockAlarmCrInvalidEmergencyCode =
                    _catsCrInvalidEmergencyCode.EventlogDuringBlockedAlarmChecked;

                onlyInDatabase = false;
            }

            byte? blockObjectAlarmInvalidEmergencyCodeType = null;
            Guid? blockObjectAlarmInvalidEmergencyCodeId = null;

            if (_catsCrInvalidEmergencyCode.ObjectForBlockingAlarm != null)
            {
                blockObjectAlarmInvalidEmergencyCodeType =
                    (byte)_catsCrInvalidEmergencyCode.ObjectForBlockingAlarm.GetObjectType();

                blockObjectAlarmInvalidEmergencyCodeId =
                    (Guid)_catsCrInvalidEmergencyCode.ObjectForBlockingAlarm.GetId();
            }

            if (_devicesAlarmSetting.ObjBlockAlarmCrInvalidEmergencyCodeObjectType !=
                blockObjectAlarmInvalidEmergencyCodeType
                || _devicesAlarmSetting.ObjBlockAlarmCrInvalidEmergencyCodeId !=
                blockObjectAlarmInvalidEmergencyCodeId)
            {
                _devicesAlarmSetting.ObjBlockAlarmCrInvalidEmergencyCodeObjectType =
                    blockObjectAlarmInvalidEmergencyCodeType;

                _devicesAlarmSetting.ObjBlockAlarmCrInvalidEmergencyCodeId =
                    blockObjectAlarmInvalidEmergencyCodeId;

                onlyInDatabase = false;
            }

            _devicesAlarmSetting.AlarmCRInvalidEmergencyCodePresentationGroup = _catsCrInvalidEmergencyCode.PresentationGroup;

            SaveAlarmArcs(
                _cmaaCrInvalidEmergencyCode.AlarmArcs,
                AlarmType.CardReader_InvalidEmergencyCode);

            if (_cmaaCrInvalidEmergencyCode.AlarmArcsWasChanged)
                onlyInDatabase = false;

            if (!_catsCrAccessPermitted.ControlValues(
                control =>
                    _openAndScrollToControlByAlarmType[AlarmType.CardReader_AccessPermitted]()))
            {
                return false;
            }

            if (_devicesAlarmSetting.AlarmCrAccessPermitted != _catsCrAccessPermitted.AlarmEnabledChecked)
            {
                _devicesAlarmSetting.AlarmCrAccessPermitted = _catsCrAccessPermitted.AlarmEnabledChecked;
                onlyInDatabase = false;
            }

            if (_devicesAlarmSetting.BlockAlarmCrAccessPermitted != _catsCrAccessPermitted.BlockAlarmChecked)
            {
                _devicesAlarmSetting.BlockAlarmCrAccessPermitted = _catsCrAccessPermitted.BlockAlarmChecked;
                onlyInDatabase = false;
            }

            byte? blockObjectAlarmAccessPermittedType = null;
            Guid? blockObjectAlarmAccessPermittedId = null;

            if (_catsCrAccessPermitted.ObjectForBlockingAlarm != null)
            {
                blockObjectAlarmAccessPermittedType =
                    (byte)_catsCrAccessPermitted.ObjectForBlockingAlarm.GetObjectType();

                blockObjectAlarmAccessPermittedId =
                    (Guid)_catsCrAccessPermitted.ObjectForBlockingAlarm.GetId();
            }

            if (_devicesAlarmSetting.ObjBlockAlarmCrAccessPermittedObjectType !=
                blockObjectAlarmAccessPermittedType
                || _devicesAlarmSetting.ObjBlockAlarmCrAccessPermittedId !=
                blockObjectAlarmAccessPermittedId)
            {
                _devicesAlarmSetting.ObjBlockAlarmCrAccessPermittedObjectType =
                    blockObjectAlarmAccessPermittedType;

                _devicesAlarmSetting.ObjBlockAlarmCrAccessPermittedId =
                    blockObjectAlarmAccessPermittedId;

                onlyInDatabase = false;
            }

            _devicesAlarmSetting.AlarmCRAccessPermittedPresentationGroup = _catsCrAccessPermitted.PresentationGroup;

            SaveAlarmArcs(
                _cmaaCrAccessPermitted.AlarmArcs,
                AlarmType.CardReader_AccessPermitted);

            if (_cmaaCrAccessPermitted.AlarmArcsWasChanged)
                onlyInDatabase = false;

            if (!_catsCrInvalidPinRetriesLimitReached.ControlValues(
                control =>
                    _openAndScrollToControlByAlarmType[AlarmType.CardReader_Invalid_Pin_Retries_Limit_Reached]()))
            {
                return false;
            }

            if (_devicesAlarmSetting.AlarmInvalidPinRetriesLimitReached !=
                _catsCrInvalidPinRetriesLimitReached.AlarmEnabledChecked)
            {
                _devicesAlarmSetting.AlarmInvalidPinRetriesLimitReached =
                    _catsCrInvalidPinRetriesLimitReached.AlarmEnabledChecked;

                onlyInDatabase = false;
            }

            if (_devicesAlarmSetting.BlockAlarmInvalidPinRetriesLimitReached !=
                _catsCrInvalidPinRetriesLimitReached.BlockAlarmChecked)
            {
                _devicesAlarmSetting.BlockAlarmInvalidPinRetriesLimitReached =
                    _catsCrInvalidPinRetriesLimitReached.BlockAlarmChecked;

                onlyInDatabase = false;
            }

            byte? blockObjectAlarmInvalidPinRetriesLimitReachedType = null;
            Guid? blockObjectAlarmInvalidPinRetriesLimitReachedId = null;

            if (_catsCrInvalidPinRetriesLimitReached.ObjectForBlockingAlarm != null)
            {
                blockObjectAlarmInvalidPinRetriesLimitReachedType =
                    (byte)_catsCrInvalidPinRetriesLimitReached.ObjectForBlockingAlarm.GetObjectType();

                blockObjectAlarmInvalidPinRetriesLimitReachedId =
                    (Guid)_catsCrInvalidPinRetriesLimitReached.ObjectForBlockingAlarm.GetId();
            }

            if (_devicesAlarmSetting.ObjBlockAlarmInvalidPinRetriesLimitReachedObjectType !=
                blockObjectAlarmInvalidPinRetriesLimitReachedType
                || _devicesAlarmSetting.ObjBlockAlarmInvalidPinRetriesLimitReachedId !=
                blockObjectAlarmInvalidPinRetriesLimitReachedId)
            {
                _devicesAlarmSetting.ObjBlockAlarmInvalidPinRetriesLimitReachedObjectType =
                    blockObjectAlarmInvalidPinRetriesLimitReachedType;

                _devicesAlarmSetting.ObjBlockAlarmInvalidPinRetriesLimitReachedId =
                    blockObjectAlarmInvalidPinRetriesLimitReachedId;

                onlyInDatabase = false;
            }

            _devicesAlarmSetting.PgAlarmInvalidPinRetriesLimitReached = _catsCrInvalidPinRetriesLimitReached.PresentationGroup;

            SaveAlarmArcs(
                _cmaaCrInvalidPinRetriesLimitReached.AlarmArcs,
                AlarmType.CardReader_Invalid_Pin_Retries_Limit_Reached);

            if (_cmaaCrInvalidPinRetriesLimitReached.AlarmArcsWasChanged)
                onlyInDatabase = false;

            if (!_catsCrInvalidGinRetriesLimitReached.ControlValues(
                control =>
                    _openAndScrollToControlByAlarmType[AlarmType.CardReader_Invalid_Code_Retries_Limit_Reached]()))
            {
                return false;
            }

            if (_devicesAlarmSetting.AlarmInvalidGinRetriesLimitReached !=
                _catsCrInvalidGinRetriesLimitReached.AlarmEnabledChecked)
            {
                _devicesAlarmSetting.AlarmInvalidGinRetriesLimitReached =
                    _catsCrInvalidGinRetriesLimitReached.AlarmEnabledChecked;

                onlyInDatabase = false;
            }

            if (_devicesAlarmSetting.BlockAlarmInvalidGinRetriesLimitReached !=
                _catsCrInvalidGinRetriesLimitReached.BlockAlarmChecked)
            {
                _devicesAlarmSetting.BlockAlarmInvalidGinRetriesLimitReached =
                    _catsCrInvalidGinRetriesLimitReached.BlockAlarmChecked;

                onlyInDatabase = false;
            }

            byte? blockObjectAlarmInvalidGinRetriesLimitReachedType = null;
            Guid? blockObjectAlarmInvalidGinRetriesLimitReachedId = null;

            if (_catsCrInvalidGinRetriesLimitReached.ObjectForBlockingAlarm != null)
            {
                blockObjectAlarmInvalidGinRetriesLimitReachedType =
                    (byte)_catsCrInvalidGinRetriesLimitReached.ObjectForBlockingAlarm.GetObjectType();

                blockObjectAlarmInvalidGinRetriesLimitReachedId =
                    (Guid)_catsCrInvalidGinRetriesLimitReached.ObjectForBlockingAlarm.GetId();
            }

            if (_devicesAlarmSetting.ObjBlockAlarmInvalidGinRetriesLimitReachedObjectType !=
                blockObjectAlarmInvalidGinRetriesLimitReachedType
                || _devicesAlarmSetting.ObjBlockAlarmInvalidGinRetriesLimitReachedId !=
                blockObjectAlarmInvalidGinRetriesLimitReachedId)
            {
                _devicesAlarmSetting.ObjBlockAlarmInvalidGinRetriesLimitReachedObjectType =
                    blockObjectAlarmInvalidGinRetriesLimitReachedType;

                _devicesAlarmSetting.ObjBlockAlarmInvalidGinRetriesLimitReachedId =
                    blockObjectAlarmInvalidGinRetriesLimitReachedId;

                onlyInDatabase = false;
            }

            _devicesAlarmSetting.PgAlarmInvalidGinRetriesLimitReached = _catsCrInvalidGinRetriesLimitReached.PresentationGroup;

            SaveAlarmArcs(
                _cmaaCrInvalidGinRetriesLimitReached.AlarmArcs,
                AlarmType.CardReader_Invalid_Code_Retries_Limit_Reached);

            if (_cmaaCrInvalidGinRetriesLimitReached.AlarmArcsWasChanged)
                onlyInDatabase = false;
            return true;
        }

        private bool SaveDsmRelatedSettings(ref bool onlyInDatabase)
        {
            if (!_catsDsmDoorAjar.ControlValues(
                control =>
                    _openAndScrollToControlByAlarmType[AlarmType.DoorEnvironment_DoorAjar]()))
            {
                return false;
            }


            if (_devicesAlarmSetting.AlarmDEDoorAjar != _catsDsmDoorAjar.AlarmEnabledChecked)
            {
                _devicesAlarmSetting.AlarmDEDoorAjar = _catsDsmDoorAjar.AlarmEnabledChecked;
                onlyInDatabase = false;
            }

            if (_devicesAlarmSetting.BlockAlarmDEDoorAjar != _catsDsmDoorAjar.BlockAlarmChecked)
            {
                _devicesAlarmSetting.BlockAlarmDEDoorAjar = _catsDsmDoorAjar.BlockAlarmChecked;
                onlyInDatabase = false;
            }

            byte? blockObjectAlarmDsmDoorAjarType = null;
            Guid? blockObjectAlarmDsmDoorAjarId = null;

            if (_catsDsmDoorAjar.ObjectForBlockingAlarm != null)
            {
                blockObjectAlarmDsmDoorAjarType =
                    (byte)_catsDsmDoorAjar.ObjectForBlockingAlarm.GetObjectType();

                blockObjectAlarmDsmDoorAjarId =
                    (Guid)_catsDsmDoorAjar.ObjectForBlockingAlarm.GetId();
            }

            if (_devicesAlarmSetting.ObjBlockAlarmDEDoorAjarObjectType
                != blockObjectAlarmDsmDoorAjarType
                || _devicesAlarmSetting.ObjBlockAlarmDEDoorAjarId
                != blockObjectAlarmDsmDoorAjarId)
            {
                _devicesAlarmSetting.ObjBlockAlarmDEDoorAjarObjectType =
                    blockObjectAlarmDsmDoorAjarType;

                _devicesAlarmSetting.ObjBlockAlarmDEDoorAjarId =
                    blockObjectAlarmDsmDoorAjarId;

                onlyInDatabase = false;
            }

            _devicesAlarmSetting.AlarmDEDoorAjarPresentationGroup =
                _catsDsmDoorAjar.PresentationGroup;

            SaveAlarmArcs(
                _cmaaDsmDoorAjar.AlarmArcs,
                AlarmType.DoorEnvironment_DoorAjar);

            if (_cmaaDsmDoorAjar.AlarmArcsWasChanged)
                onlyInDatabase = false;

            if (!_catsDsmIntrusion.ControlValues(
                control =>
                    _openAndScrollToControlByAlarmType[AlarmType.DoorEnvironment_Intrusion]()))
            {
                return false;
            }

            if (_devicesAlarmSetting.AlarmDEIntrusion != _catsDsmIntrusion.AlarmEnabledChecked)
            {
                _devicesAlarmSetting.AlarmDEIntrusion = _catsDsmIntrusion.AlarmEnabledChecked;
                onlyInDatabase = false;
            }

            if (_devicesAlarmSetting.BlockAlarmDEIntrusion != _catsDsmIntrusion.BlockAlarmChecked)
            {
                _devicesAlarmSetting.BlockAlarmDEIntrusion = _catsDsmIntrusion.BlockAlarmChecked;
                onlyInDatabase = false;
            }

            byte? blockObjectAlarmDsmIntrusionType = null;
            Guid? blockObjectAlarmDsmIntrusionId = null;

            if (_catsDsmIntrusion.ObjectForBlockingAlarm != null)
            {
                blockObjectAlarmDsmIntrusionType =
                    (byte)_catsDsmIntrusion.ObjectForBlockingAlarm.GetObjectType();

                blockObjectAlarmDsmIntrusionId =
                    (Guid)_catsDsmIntrusion.ObjectForBlockingAlarm.GetId();
            }

            if (_devicesAlarmSetting.ObjBlockAlarmDEIntrusionObjectType !=
                blockObjectAlarmDsmIntrusionType
                || _devicesAlarmSetting.ObjBlockAlarmDEIntrusionId !=
                blockObjectAlarmDsmIntrusionId)
            {
                _devicesAlarmSetting.ObjBlockAlarmDEIntrusionObjectType =
                    blockObjectAlarmDsmIntrusionType;

                _devicesAlarmSetting.ObjBlockAlarmDEIntrusionId =
                    blockObjectAlarmDsmIntrusionId;

                onlyInDatabase = false;
            }

            _devicesAlarmSetting.AlarmDEIntrusionPresentationGroup =
                _catsDsmIntrusion.PresentationGroup;

            SaveAlarmArcs(
                _cmaaDsmIntrusion.AlarmArcs,
                AlarmType.DoorEnvironment_Intrusion);

            if (_cmaaDsmIntrusion.AlarmArcsWasChanged)
                onlyInDatabase = false;

            if (!_catsDsmSabotage.ControlValues(
                control =>
                    _openAndScrollToControlByAlarmType[AlarmType.DoorEnvironment_Sabotage]()))
            {
                return false;
            }

            if (_devicesAlarmSetting.AlarmDESabotage != _catsDsmSabotage.AlarmEnabledChecked)
            {
                _devicesAlarmSetting.AlarmDESabotage = _catsDsmSabotage.AlarmEnabledChecked;
                onlyInDatabase = false;
            }

            if (_devicesAlarmSetting.BlockAlarmDESabotage != _catsDsmSabotage.BlockAlarmChecked)
            {
                _devicesAlarmSetting.BlockAlarmDESabotage = _catsDsmSabotage.BlockAlarmChecked;
                onlyInDatabase = false;
            }

            byte? blockObjectAlarmDsmSabotageType = null;
            Guid? blockObjectAlarmDsmSabotageId = null;

            if (_catsDsmSabotage.ObjectForBlockingAlarm != null)
            {
                blockObjectAlarmDsmSabotageType =
                    (byte)_catsDsmSabotage.ObjectForBlockingAlarm.GetObjectType();

                blockObjectAlarmDsmSabotageId =
                    (Guid)_catsDsmSabotage.ObjectForBlockingAlarm.GetId();
            }

            if (_devicesAlarmSetting.ObjBlockAlarmDESabotageObjectType !=
                blockObjectAlarmDsmSabotageType
                || _devicesAlarmSetting.ObjBlockAlarmDESabotageId !=
                blockObjectAlarmDsmSabotageId)
            {
                _devicesAlarmSetting.ObjBlockAlarmDESabotageObjectType =
                    blockObjectAlarmDsmSabotageType;

                _devicesAlarmSetting.ObjBlockAlarmDESabotageId =
                    blockObjectAlarmDsmSabotageId;

                onlyInDatabase = false;
            }

            _devicesAlarmSetting.AlarmDESabotagePresentationGroup =
                _catsDsmSabotage.PresentationGroup;

            SaveAlarmArcs(
                _cmaaDsmSabotage.AlarmArcs,
                AlarmType.DoorEnvironment_Sabotage);

            if (_cmaaDsmSabotage.AlarmArcsWasChanged)
                onlyInDatabase = false;
            return true;
        }

        private bool SaveDcuRelatedSettings(ref bool onlyInDatabase)
        {
            if (!_catsDcuOffline.ControlValues(
                control =>
                    _openAndScrollToControlByAlarmType[AlarmType.DCU_Offline]()))
            {
                return false;
            }

            if (_devicesAlarmSetting.AlarmDCUOffline != _catsDcuOffline.AlarmEnabledChecked)
            {
                _devicesAlarmSetting.AlarmDCUOffline = _catsDcuOffline.AlarmEnabledChecked;
                onlyInDatabase = false;
            }

            if (_devicesAlarmSetting.BlockAlarmDCUOffline != _catsDcuOffline.BlockAlarmChecked)
            {
                _devicesAlarmSetting.BlockAlarmDCUOffline = _catsDcuOffline.BlockAlarmChecked;
                onlyInDatabase = false;
            }

            byte? blockObjectAlarmDcuOfflineType = null;
            Guid? blockObjectAlarmDcuOfflineId = null;

            if (_catsDcuOffline.ObjectForBlockingAlarm != null)
            {
                blockObjectAlarmDcuOfflineType = (byte)_catsDcuOffline.ObjectForBlockingAlarm.GetObjectType();
                blockObjectAlarmDcuOfflineId = (Guid)_catsDcuOffline.ObjectForBlockingAlarm.GetId();
            }

            if (_devicesAlarmSetting.ObjBlockAlarmDCUOfflineObjectType != blockObjectAlarmDcuOfflineType
                || _devicesAlarmSetting.ObjBlockAlarmDCUOfflineId != blockObjectAlarmDcuOfflineId)
            {
                _devicesAlarmSetting.ObjBlockAlarmDCUOfflineObjectType = blockObjectAlarmDcuOfflineType;
                _devicesAlarmSetting.ObjBlockAlarmDCUOfflineId = blockObjectAlarmDcuOfflineId;
                onlyInDatabase = false;
            }

            _devicesAlarmSetting.AlarmDCUOfflinePresentationGroup = _catsDcuOffline.PresentationGroup;

            SaveAlarmArcs(
                _cmaaDcuOffline.AlarmArcs,
                AlarmType.DCU_Offline);

            if (_cmaaDcuOffline.AlarmArcsWasChanged)
                onlyInDatabase = false;

            SaveAlarmArcs(
                _cmaaDcuOfflineDueCcuOffline.AlarmArcs,
                AlarmType.DCU_Offline_Due_CCU_Offline);

            if (!_catsDcuTamperSabotage.ControlValues(
                control =>
                    _openAndScrollToControlByAlarmType[AlarmType.DCU_TamperSabotage]()))
            {
                return false;
            }

            if (_devicesAlarmSetting.AlarmDCUTamperSabotage != _catsDcuTamperSabotage.AlarmEnabledChecked)
            {
                _devicesAlarmSetting.AlarmDCUTamperSabotage = _catsDcuTamperSabotage.AlarmEnabledChecked;
                onlyInDatabase = false;
            }

            if (_devicesAlarmSetting.BlockAlarmDCUTamperSabotage != _catsDcuTamperSabotage.BlockAlarmChecked)
            {
                _devicesAlarmSetting.BlockAlarmDCUTamperSabotage = _catsDcuTamperSabotage.BlockAlarmChecked;
                onlyInDatabase = false;
            }

            if (_devicesAlarmSetting.EventlogDuringBlockAlarmDCUTamperSabotage !=
                _catsDcuTamperSabotage.EventlogDuringBlockedAlarmChecked)
            {
                _devicesAlarmSetting.EventlogDuringBlockAlarmDCUTamperSabotage =
                    _catsDcuTamperSabotage.EventlogDuringBlockedAlarmChecked;

                onlyInDatabase = false;
            }

            byte? blockObjectAlarmDcuTamperSabotageType = null;
            Guid? blockObjectAlarmDcuTamperSabotageId = null;

            if (_catsDcuTamperSabotage.ObjectForBlockingAlarm != null)
            {
                blockObjectAlarmDcuTamperSabotageType =
                    (byte)_catsDcuTamperSabotage.ObjectForBlockingAlarm.GetObjectType();

                blockObjectAlarmDcuTamperSabotageId =
                    (Guid)_catsDcuTamperSabotage.ObjectForBlockingAlarm.GetId();
            }

            if (_devicesAlarmSetting.ObjBlockAlarmDCUTamperSabotageObjectType !=
                blockObjectAlarmDcuTamperSabotageType
                || _devicesAlarmSetting.ObjBlockAlarmDCUTamperSabotageId !=
                blockObjectAlarmDcuTamperSabotageId)
            {
                _devicesAlarmSetting.ObjBlockAlarmDCUTamperSabotageObjectType =
                    blockObjectAlarmDcuTamperSabotageType;

                _devicesAlarmSetting.ObjBlockAlarmDCUTamperSabotageId =
                    blockObjectAlarmDcuTamperSabotageId;

                onlyInDatabase = false;
            }

            _devicesAlarmSetting.AlarmDCUTamperSabotagePresentationGroup =
                _catsDcuTamperSabotage.PresentationGroup;

            SaveAlarmArcs(
                _cmaaDcuTamperSabotage.AlarmArcs,
                AlarmType.DCU_TamperSabotage);

            if (_cmaaDcuTamperSabotage.AlarmArcsWasChanged)
                onlyInDatabase = false;

            SaveAlarmArcs(
                _cmaaDcuOutdatedFirmware.AlarmArcs,
                AlarmType.DCU_OutdatedFirmware);
            return true;
        }

        private bool SaveCcuRelatedSettings(ref bool onlyInDatabase)
        {
            if (!_catsCcuOffline.ControlValues(
                control =>
                    _openAndScrollToControlByAlarmType[AlarmType.CCU_Offline]()))
            {
                return false;
            }

            _devicesAlarmSetting.AlarmCCUOffline = _catsCcuOffline.AlarmEnabledChecked;
            _devicesAlarmSetting.BlockAlarmCCUOffline = _catsCcuOffline.BlockAlarmChecked;

            byte? blockObjectAlarmCcuOfflineType = null;
            Guid? blockObjectAlarmCcuOfflineId = null;

            if (_catsCcuOffline.ObjectForBlockingAlarm != null)
            {
                blockObjectAlarmCcuOfflineType =
                    (byte)_catsCcuOffline.ObjectForBlockingAlarm.GetObjectType();

                blockObjectAlarmCcuOfflineId =
                    (Guid)_catsCcuOffline.ObjectForBlockingAlarm.GetId();
            }

            _devicesAlarmSetting.ObjBlockAlarmCCUOfflineObjectType = blockObjectAlarmCcuOfflineType;
            _devicesAlarmSetting.ObjBlockAlarmCCUOfflineId = blockObjectAlarmCcuOfflineId;

            _devicesAlarmSetting.AlarmCCUOfflinePresentationGroup = _catsCcuOffline.PresentationGroup;

            SaveAlarmArcs(
                _cmaaCcuOffline.AlarmArcs,
                AlarmType.CCU_Offline);

            if (!_catsCcuUnconfigured.ControlValues(
                control =>
                    _openAndScrollToControlByAlarmType[AlarmType.CCU_Unconfigured]()))
            {
                return false;
            }

            _devicesAlarmSetting.AlarmCCUUnconfigured = _catsCcuUnconfigured.AlarmEnabledChecked;
            _devicesAlarmSetting.BlockAlarmCCUUnconfigured = _catsCcuUnconfigured.BlockAlarmChecked;

            byte? blockObjectAlarmCcuUnconfiguredType = null;
            Guid? blockObjectAlarmCcuUnconfiguredId = null;

            if (_catsCcuUnconfigured.ObjectForBlockingAlarm != null)
            {
                blockObjectAlarmCcuUnconfiguredType =
                    (byte)_catsCcuUnconfigured.ObjectForBlockingAlarm.GetObjectType();

                blockObjectAlarmCcuUnconfiguredId =
                    (Guid)_catsCcuUnconfigured.ObjectForBlockingAlarm.GetId();
            }

            _devicesAlarmSetting.ObjBlockAlarmCCUUnconfiguredObjectType = blockObjectAlarmCcuUnconfiguredType;
            _devicesAlarmSetting.ObjBlockAlarmCCUUnconfiguredId = blockObjectAlarmCcuUnconfiguredId;

            _devicesAlarmSetting.AlarmCCUUnconfiguredPresentationGroup = _catsCcuUnconfigured.PresentationGroup;

            SaveAlarmArcs(
                _cmaaCcuUnconfigured.AlarmArcs,
                AlarmType.CCU_Unconfigured);

            if (!_catsCcuClockUnsynchronized.ControlValues(
                control =>
                    _openAndScrollToControlByAlarmType[AlarmType.CCU_ClockUnsynchronized]()))
            {
                return false;
            }

            _devicesAlarmSetting.AlarmCCUClockUnsynchronized = _catsCcuClockUnsynchronized.AlarmEnabledChecked;
            _devicesAlarmSetting.BlockAlarmCCUClockUnsynchronized = _catsCcuClockUnsynchronized.BlockAlarmChecked;

            byte? blockObjectAlarmCcuClockUnsynchronizedType = null;
            Guid? blockObjectAlarmCcuClockUnsynchronizedId = null;

            if (_catsCcuClockUnsynchronized.ObjectForBlockingAlarm != null)
            {
                blockObjectAlarmCcuClockUnsynchronizedType =
                    (byte)_catsCcuClockUnsynchronized.ObjectForBlockingAlarm.GetObjectType();

                blockObjectAlarmCcuClockUnsynchronizedId =
                    (Guid)_catsCcuClockUnsynchronized.ObjectForBlockingAlarm.GetId();
            }

            _devicesAlarmSetting.ObjBlockAlarmCCUClockUnsynchronizedObjectType = blockObjectAlarmCcuClockUnsynchronizedType;
            _devicesAlarmSetting.ObjBlockAlarmCCUClockUnsynchronizedId = blockObjectAlarmCcuClockUnsynchronizedId;

            _devicesAlarmSetting.AlarmCCUClockUnsynchronizedPresentationGroup = _catsCcuClockUnsynchronized.PresentationGroup;

            SaveAlarmArcs(
                _cmaaCcuClockUnsynchronized.AlarmArcs,
                AlarmType.CCU_ClockUnsynchronized);

            if (!_catsCcuTamperSabotage.ControlValues(
                control =>
                    _openAndScrollToControlByAlarmType[AlarmType.CCU_TamperSabotage]()))
            {
                return false;
            }

            if (_devicesAlarmSetting.AlarmCCUTamperSabotage != _catsCcuTamperSabotage.AlarmEnabledChecked)
            {
                _devicesAlarmSetting.AlarmCCUTamperSabotage = _catsCcuTamperSabotage.AlarmEnabledChecked;
                onlyInDatabase = false;
            }

            if (_devicesAlarmSetting.BlockAlarmCCUTamperSabotage != _catsCcuTamperSabotage.BlockAlarmChecked)
            {
                _devicesAlarmSetting.BlockAlarmCCUTamperSabotage = _catsCcuTamperSabotage.BlockAlarmChecked;
                onlyInDatabase = false;
            }

            if (_devicesAlarmSetting.EventlogDuringBlockAlarmCCUTamperSabotage !=
                _catsCcuTamperSabotage.EventlogDuringBlockedAlarmChecked)
            {
                _devicesAlarmSetting.EventlogDuringBlockAlarmCCUTamperSabotage =
                    _catsCcuTamperSabotage.EventlogDuringBlockedAlarmChecked;

                onlyInDatabase = false;
            }

            byte? blockObjectAlarmCcuTamperSabotageType = null;
            Guid? blockObjectAlarmCcuTamperSabotageId = null;

            if (_catsCcuTamperSabotage.ObjectForBlockingAlarm != null)
            {
                blockObjectAlarmCcuTamperSabotageType =
                    (byte)_catsCcuTamperSabotage.ObjectForBlockingAlarm.GetObjectType();

                blockObjectAlarmCcuTamperSabotageId =
                    (Guid)_catsCcuTamperSabotage.ObjectForBlockingAlarm.GetId();
            }

            if (_devicesAlarmSetting.ObjBlockAlarmCCUTamperSabotageObjectType !=
                blockObjectAlarmCcuTamperSabotageType
                || _devicesAlarmSetting.ObjBlockAlarmCCUTamperSabotageId !=
                blockObjectAlarmCcuTamperSabotageId)
            {
                _devicesAlarmSetting.ObjBlockAlarmCCUTamperSabotageObjectType = blockObjectAlarmCcuTamperSabotageType;
                _devicesAlarmSetting.ObjBlockAlarmCCUTamperSabotageId = blockObjectAlarmCcuTamperSabotageId;

                onlyInDatabase = false;
            }

            _devicesAlarmSetting.AlarmCCUTamperSabotagePresentationGroup = _catsCcuTamperSabotage.PresentationGroup;

            SaveAlarmArcs(
                _cmaaCcuTamperSabotage.AlarmArcs,
                AlarmType.CCU_TamperSabotage);

            if (_cmaaCcuTamperSabotage.AlarmArcsWasChanged)
                onlyInDatabase = false;

            if (!_catsCcuPrimaryPowerMissing.ControlValues(
                control =>
                    _openAndScrollToControlByAlarmType[AlarmType.CCU_PrimaryPowerMissing]()))
            {
                return false;
            }

            if (_devicesAlarmSetting.AlarmPrimaryPowerMissing != _catsCcuPrimaryPowerMissing.AlarmEnabledChecked)
            {
                _devicesAlarmSetting.AlarmPrimaryPowerMissing = _catsCcuPrimaryPowerMissing.AlarmEnabledChecked;
                onlyInDatabase = false;
            }

            if (_devicesAlarmSetting.BlockAlarmPrimaryPowerMissing != _catsCcuPrimaryPowerMissing.BlockAlarmChecked)
            {
                _devicesAlarmSetting.BlockAlarmPrimaryPowerMissing = _catsCcuPrimaryPowerMissing.BlockAlarmChecked;
                onlyInDatabase = false;
            }

            if (_devicesAlarmSetting.EventlogDuringBlockAlarmPrimaryPowerMissing !=
                _catsCcuPrimaryPowerMissing.EventlogDuringBlockedAlarmChecked)
            {
                _devicesAlarmSetting.EventlogDuringBlockAlarmPrimaryPowerMissing =
                    _catsCcuPrimaryPowerMissing.EventlogDuringBlockedAlarmChecked;

                onlyInDatabase = false;
            }

            byte? blockObjectAlarmPrimaryPowerMissingType = null;
            Guid? blockObjectAlarmPrimaryPowerMissingId = null;

            if (_catsCcuPrimaryPowerMissing.ObjectForBlockingAlarm != null)
            {
                blockObjectAlarmPrimaryPowerMissingType =
                    (byte)_catsCcuPrimaryPowerMissing.ObjectForBlockingAlarm.GetObjectType();

                blockObjectAlarmPrimaryPowerMissingId =
                    (Guid)_catsCcuPrimaryPowerMissing.ObjectForBlockingAlarm.GetId();
            }

            if (_devicesAlarmSetting.ObjBlockAlarmPrimaryPowerMissingObjectType !=
                blockObjectAlarmPrimaryPowerMissingType
                || _devicesAlarmSetting.ObjBlockAlarmPrimaryPowerMissingId !=
                blockObjectAlarmPrimaryPowerMissingId)
            {
                _devicesAlarmSetting.ObjBlockAlarmPrimaryPowerMissingObjectType =
                    blockObjectAlarmPrimaryPowerMissingType;

                _devicesAlarmSetting.ObjBlockAlarmPrimaryPowerMissingId =
                    blockObjectAlarmPrimaryPowerMissingId;

                onlyInDatabase = false;
            }

            _devicesAlarmSetting.AlarmCCUPrimaryPowerMissingPresentationGroup = _catsCcuPrimaryPowerMissing.PresentationGroup;

            SaveAlarmArcs(
                _cmaaCcuPrimaryPowerMissing.AlarmArcs,
                AlarmType.CCU_PrimaryPowerMissing);

            if (_cmaaCcuPrimaryPowerMissing.AlarmArcsWasChanged)
                onlyInDatabase = false;

            if (!_catsCcuBatteryIsLow.ControlValues(
                control =>
                    _openAndScrollToControlByAlarmType[AlarmType.CCU_BatteryLow]()))
            {
                return false;
            }

            if (_devicesAlarmSetting.AlarmBatteryIsLow != _catsCcuBatteryIsLow.AlarmEnabledChecked)
            {
                _devicesAlarmSetting.AlarmBatteryIsLow = _catsCcuBatteryIsLow.AlarmEnabledChecked;
                onlyInDatabase = false;
            }

            if (_devicesAlarmSetting.BlockAlarmBatteryIsLow != _catsCcuBatteryIsLow.BlockAlarmChecked)
            {
                _devicesAlarmSetting.BlockAlarmBatteryIsLow = _catsCcuBatteryIsLow.BlockAlarmChecked;
                onlyInDatabase = false;
            }

            if (_devicesAlarmSetting.EventlogDuringBlockAlarmBatteryIsLow !=
                _catsCcuBatteryIsLow.EventlogDuringBlockedAlarmChecked)
            {
                _devicesAlarmSetting.EventlogDuringBlockAlarmBatteryIsLow =
                    _catsCcuBatteryIsLow.EventlogDuringBlockedAlarmChecked;

                onlyInDatabase = false;
            }

            byte? blockObjectAlarmBatteryIsLowType = null;
            Guid? blockObjectAlarmBatteryIsLowId = null;

            if (_catsCcuBatteryIsLow.ObjectForBlockingAlarm != null)
            {
                blockObjectAlarmBatteryIsLowType =
                    (byte)_catsCcuBatteryIsLow.ObjectForBlockingAlarm.GetObjectType();

                blockObjectAlarmBatteryIsLowId =
                    (Guid)_catsCcuBatteryIsLow.ObjectForBlockingAlarm.GetId();
            }

            if (_devicesAlarmSetting.ObjBlockAlarmBatteryIsLowObjectType !=
                blockObjectAlarmBatteryIsLowType
                || _devicesAlarmSetting.ObjBlockAlarmBatteryIsLowId !=
                blockObjectAlarmBatteryIsLowId)
            {
                _devicesAlarmSetting.ObjBlockAlarmBatteryIsLowObjectType =
                    blockObjectAlarmBatteryIsLowType;

                _devicesAlarmSetting.ObjBlockAlarmBatteryIsLowId =
                    blockObjectAlarmBatteryIsLowId;

                onlyInDatabase = false;
            }

            _devicesAlarmSetting.AlarmCCUBatteryIsLowPresentationGroup = _catsCcuBatteryIsLow.PresentationGroup;

            SaveAlarmArcs(
                _cmaaCcuBatteryIsLow.AlarmArcs,
                AlarmType.CCU_BatteryLow);

            if (_cmaaCcuBatteryIsLow.AlarmArcsWasChanged)
                onlyInDatabase = false;

            if (!_catsCcuUpsOutputFuse.ControlValues(
                control =>
                    _openAndScrollToControlByAlarmType[AlarmType.Ccu_Ups_OutputFuse]()))
            {
                return false;
            }

            if (_devicesAlarmSetting.AlarmUpsOutputFuse != _catsCcuUpsOutputFuse.AlarmEnabledChecked)
            {
                _devicesAlarmSetting.AlarmUpsOutputFuse = _catsCcuUpsOutputFuse.AlarmEnabledChecked;
                onlyInDatabase = false;
            }

            if (_devicesAlarmSetting.BlockAlarmUpsOutputFuse != _catsCcuUpsOutputFuse.BlockAlarmChecked)
            {
                _devicesAlarmSetting.BlockAlarmUpsOutputFuse = _catsCcuUpsOutputFuse.BlockAlarmChecked;
                onlyInDatabase = false;
            }

            if (_devicesAlarmSetting.EventlogDuringBlockAlarmUpsOutputFuse !=
                _catsCcuUpsOutputFuse.EventlogDuringBlockedAlarmChecked)
            {
                _devicesAlarmSetting.EventlogDuringBlockAlarmUpsOutputFuse =
                    _catsCcuUpsOutputFuse.EventlogDuringBlockedAlarmChecked;

                onlyInDatabase = false;
            }

            byte? blockObjectAlarmUpsOutputFuseType = null;
            Guid? blockObjectAlarmUpsOutputFuseId = null;

            if (_catsCcuUpsOutputFuse.ObjectForBlockingAlarm != null)
            {
                blockObjectAlarmUpsOutputFuseType =
                    (byte)_catsCcuUpsOutputFuse.ObjectForBlockingAlarm.GetObjectType();

                blockObjectAlarmUpsOutputFuseId =
                    (Guid)_catsCcuUpsOutputFuse.ObjectForBlockingAlarm.GetId();
            }

            if (_devicesAlarmSetting.ObjBlockAlarmUpsOutputFuseObjectType != blockObjectAlarmUpsOutputFuseType
                || _devicesAlarmSetting.ObjBlockAlarmUpsOutputFuseId != blockObjectAlarmUpsOutputFuseId)
            {
                _devicesAlarmSetting.ObjBlockAlarmUpsOutputFuseObjectType = blockObjectAlarmUpsOutputFuseType;
                _devicesAlarmSetting.ObjBlockAlarmUpsOutputFuseId = blockObjectAlarmUpsOutputFuseId;

                onlyInDatabase = false;
            }

            _devicesAlarmSetting.AlarmCcuUpsOutputFusePresentationGroup = _catsCcuUpsOutputFuse.PresentationGroup;

            SaveAlarmArcs(
                _cmaaCcuUpsOutputFuse.AlarmArcs,
                AlarmType.Ccu_Ups_OutputFuse);

            if (_cmaaCcuUpsOutputFuse.AlarmArcsWasChanged)
                onlyInDatabase = false;

            if (!_catsCcuUpsBatteryFault.ControlValues(
                control =>
                    _openAndScrollToControlByAlarmType[AlarmType.Ccu_Ups_BatteryFault]()))
            {
                return false;
            }

            if (_devicesAlarmSetting.AlarmUpsBatteryFault != _catsCcuUpsBatteryFault.AlarmEnabledChecked)
            {
                _devicesAlarmSetting.AlarmUpsBatteryFault = _catsCcuUpsBatteryFault.AlarmEnabledChecked;
                onlyInDatabase = false;
            }

            if (_devicesAlarmSetting.BlockAlarmUpsBatteryFault != _catsCcuUpsBatteryFault.BlockAlarmChecked)
            {
                _devicesAlarmSetting.BlockAlarmUpsBatteryFault = _catsCcuUpsBatteryFault.BlockAlarmChecked;
                onlyInDatabase = false;
            }

            if (_devicesAlarmSetting.EventlogDuringBlockAlarmUpsBatteryFault !=
                _catsCcuUpsBatteryFault.EventlogDuringBlockedAlarmChecked)
            {
                _devicesAlarmSetting.EventlogDuringBlockAlarmUpsBatteryFault =
                    _catsCcuUpsBatteryFault.EventlogDuringBlockedAlarmChecked;

                onlyInDatabase = false;
            }

            byte? blockObjectAlarmUpsBatteryFaultType = null;
            Guid? blockObjectAlarmUpsBatteryFaultId = null;

            if (_catsCcuUpsBatteryFault.ObjectForBlockingAlarm != null)
            {
                blockObjectAlarmUpsBatteryFaultType =
                    (byte)_catsCcuUpsBatteryFault.ObjectForBlockingAlarm.GetObjectType();

                blockObjectAlarmUpsBatteryFaultId =
                    (Guid)_catsCcuUpsBatteryFault.ObjectForBlockingAlarm.GetId();
            }

            if (_devicesAlarmSetting.ObjBlockAlarmUpsBatteryFaultObjectType != blockObjectAlarmUpsBatteryFaultType
                || _devicesAlarmSetting.ObjBlockAlarmUpsBatteryFaultId != blockObjectAlarmUpsBatteryFaultId)
            {
                _devicesAlarmSetting.ObjBlockAlarmUpsBatteryFaultObjectType = blockObjectAlarmUpsBatteryFaultType;
                _devicesAlarmSetting.ObjBlockAlarmUpsBatteryFaultId = blockObjectAlarmUpsBatteryFaultId;

                onlyInDatabase = false;
            }

            _devicesAlarmSetting.AlarmCcuUpsBatteryFaultPresentationGroup = _catsCcuUpsBatteryFault.PresentationGroup;

            SaveAlarmArcs(
                _cmaaCcuUpsBatteryFault.AlarmArcs,
                AlarmType.Ccu_Ups_BatteryFault);

            if (_cmaaCcuUpsBatteryFault.AlarmArcsWasChanged)
                onlyInDatabase = false;

            if (!_catsCcuUpsBatteryFuse.ControlValues(
                control =>
                    _openAndScrollToControlByAlarmType[AlarmType.Ccu_Ups_BatteryFuse]()))
            {
                return false;
            }

            if (_devicesAlarmSetting.AlarmUpsBatteryFuse != _catsCcuUpsBatteryFuse.AlarmEnabledChecked)
            {
                _devicesAlarmSetting.AlarmUpsBatteryFuse = _catsCcuUpsBatteryFuse.AlarmEnabledChecked;
                onlyInDatabase = false;
            }

            if (_devicesAlarmSetting.BlockAlarmUpsBatteryFuse != _catsCcuUpsBatteryFuse.BlockAlarmChecked)
            {
                _devicesAlarmSetting.BlockAlarmUpsBatteryFuse = _catsCcuUpsBatteryFuse.BlockAlarmChecked;
                onlyInDatabase = false;
            }

            if (_devicesAlarmSetting.EventlogDuringBlockAlarmUpsBatteryFuse !=
                _catsCcuUpsBatteryFuse.EventlogDuringBlockedAlarmChecked)
            {
                _devicesAlarmSetting.EventlogDuringBlockAlarmUpsBatteryFuse =
                    _catsCcuUpsBatteryFuse.EventlogDuringBlockedAlarmChecked;

                onlyInDatabase = false;
            }

            byte? blockObjectAlarmUpsBatteryFuseType = null;
            Guid? blockObjectAlarmUpsBatteryFuseId = null;

            if (_catsCcuUpsBatteryFuse.ObjectForBlockingAlarm != null)
            {
                blockObjectAlarmUpsBatteryFuseType =
                    (byte)_catsCcuUpsBatteryFuse.ObjectForBlockingAlarm.GetObjectType();

                blockObjectAlarmUpsBatteryFuseId =
                    (Guid)_catsCcuUpsBatteryFuse.ObjectForBlockingAlarm.GetId();
            }

            if (_devicesAlarmSetting.ObjBlockAlarmUpsBatteryFuseObjectType != blockObjectAlarmUpsBatteryFuseType
                || _devicesAlarmSetting.ObjBlockAlarmUpsBatteryFuseId != blockObjectAlarmUpsBatteryFuseId)
            {
                _devicesAlarmSetting.ObjBlockAlarmUpsBatteryFuseObjectType = blockObjectAlarmUpsBatteryFuseType;
                _devicesAlarmSetting.ObjBlockAlarmUpsBatteryFuseId = blockObjectAlarmUpsBatteryFuseId;

                onlyInDatabase = false;
            }

            _devicesAlarmSetting.AlarmCcuUpsBatteryFusePresentationGroup = _catsCcuUpsBatteryFuse.PresentationGroup;

            SaveAlarmArcs(
                _cmaaCcuUpsBatteryFuse.AlarmArcs,
                AlarmType.Ccu_Ups_BatteryFuse);

            if (_cmaaCcuUpsBatteryFuse.AlarmArcsWasChanged)
                onlyInDatabase = false;

            if (!_catsCcuUpsOvertemperature.ControlValues(
                control =>
                    _openAndScrollToControlByAlarmType[AlarmType.Ccu_Ups_OutputFuse]()))
            {
                return false;
            }

            if (_devicesAlarmSetting.AlarmUpsOvertemperature != _catsCcuUpsOvertemperature.AlarmEnabledChecked)
            {
                _devicesAlarmSetting.AlarmUpsOvertemperature = _catsCcuUpsOvertemperature.AlarmEnabledChecked;
                onlyInDatabase = false;
            }

            if (_devicesAlarmSetting.BlockAlarmUpsOvertemperature != _catsCcuUpsOvertemperature.BlockAlarmChecked)
            {
                _devicesAlarmSetting.BlockAlarmUpsOvertemperature = _catsCcuUpsOvertemperature.BlockAlarmChecked;
                onlyInDatabase = false;
            }

            if (_devicesAlarmSetting.EventlogDuringBlockAlarmUpsOvertemperature !=
                _catsCcuUpsOvertemperature.EventlogDuringBlockedAlarmChecked)
            {
                _devicesAlarmSetting.EventlogDuringBlockAlarmUpsOvertemperature =
                    _catsCcuUpsOvertemperature.EventlogDuringBlockedAlarmChecked;

                onlyInDatabase = false;
            }

            byte? blockObjectAlarmUpsOvertemperatureType = null;
            Guid? blockObjectAlarmUpsOvertemperatureId = null;

            if (_catsCcuUpsOvertemperature.ObjectForBlockingAlarm != null)
            {
                blockObjectAlarmUpsOvertemperatureType =
                    (byte)_catsCcuUpsOvertemperature.ObjectForBlockingAlarm.GetObjectType();

                blockObjectAlarmUpsOvertemperatureId =
                    (Guid)_catsCcuUpsOvertemperature.ObjectForBlockingAlarm.GetId();
            }

            if (_devicesAlarmSetting.ObjBlockAlarmUpsOvertemperatureObjectType != blockObjectAlarmUpsOvertemperatureType
                || _devicesAlarmSetting.ObjBlockAlarmUpsOvertemperatureId != blockObjectAlarmUpsOvertemperatureId)
            {
                _devicesAlarmSetting.ObjBlockAlarmUpsOvertemperatureObjectType = blockObjectAlarmUpsOvertemperatureType;
                _devicesAlarmSetting.ObjBlockAlarmUpsOvertemperatureId = blockObjectAlarmUpsOvertemperatureId;

                onlyInDatabase = false;
            }

            _devicesAlarmSetting.AlarmCcuUpsOvertemperaturePresentationGroup = _catsCcuUpsOvertemperature.PresentationGroup;

            SaveAlarmArcs(
                _cmaaCcuUpsOvertemperature.AlarmArcs,
                AlarmType.Ccu_Ups_Overtemperature);

            if (_cmaaCcuUpsOvertemperature.AlarmArcsWasChanged)
                onlyInDatabase = false;

            if (!_catsCcuUpsTamper.ControlValues(
                control =>
                    _openAndScrollToControlByAlarmType[AlarmType.Ccu_Ups_TamperSabotage]()))
            {
                return false;
            }

            if (_devicesAlarmSetting.AlarmUpsTamperSabotage != _catsCcuUpsTamper.AlarmEnabledChecked)
            {
                _devicesAlarmSetting.AlarmUpsTamperSabotage = _catsCcuUpsTamper.AlarmEnabledChecked;
                onlyInDatabase = false;
            }

            if (_devicesAlarmSetting.BlockAlarmUpsTamperSabotage != _catsCcuUpsTamper.BlockAlarmChecked)
            {
                _devicesAlarmSetting.BlockAlarmUpsTamperSabotage = _catsCcuUpsTamper.BlockAlarmChecked;
                onlyInDatabase = false;
            }

            if (_devicesAlarmSetting.EventlogDuringBlockAlarmUpsTamperSabotage !=
                _catsCcuUpsTamper.EventlogDuringBlockedAlarmChecked)
            {
                _devicesAlarmSetting.EventlogDuringBlockAlarmUpsTamperSabotage =
                    _catsCcuUpsTamper.EventlogDuringBlockedAlarmChecked;

                onlyInDatabase = false;
            }

            byte? blockObjectAlarmUpsTamperSabotageType = null;
            Guid? blockObjectAlarmUpsTamperSabotageId = null;

            if (_catsCcuUpsTamper.ObjectForBlockingAlarm != null)
            {
                blockObjectAlarmUpsTamperSabotageType =
                    (byte)_catsCcuUpsTamper.ObjectForBlockingAlarm.GetObjectType();

                blockObjectAlarmUpsTamperSabotageId =
                    (Guid)_catsCcuUpsTamper.ObjectForBlockingAlarm.GetId();
            }

            if (_devicesAlarmSetting.ObjBlockAlarmUpsTamperSabotageObjectType != blockObjectAlarmUpsTamperSabotageType
                || _devicesAlarmSetting.ObjBlockAlarmUpsTamperSabotageId != blockObjectAlarmUpsTamperSabotageId)
            {
                _devicesAlarmSetting.ObjBlockAlarmUpsTamperSabotageObjectType = blockObjectAlarmUpsTamperSabotageType;
                _devicesAlarmSetting.ObjBlockAlarmUpsTamperSabotageId = blockObjectAlarmUpsTamperSabotageId;

                onlyInDatabase = false;
            }

            _devicesAlarmSetting.AlarmCcuUpsTamperSabotagePresentationGroup = _catsCcuUpsTamper.PresentationGroup;

            SaveAlarmArcs(
                _cmaaCcuUpsTamper.AlarmArcs,
                AlarmType.Ccu_Ups_TamperSabotage);

            if (_cmaaCcuUpsTamper.AlarmArcsWasChanged)
                onlyInDatabase = false;

            if (!_catsCcuFuseOnExtensionBoard.ControlValues(
                control =>
                    _openAndScrollToControlByAlarmType[AlarmType.CCU_ExtFuse]()))
            {
                return false;
            }

            if (_devicesAlarmSetting.AlarmFuseOnExtensionBoard != _catsCcuFuseOnExtensionBoard.AlarmEnabledChecked)
            {
                _devicesAlarmSetting.AlarmFuseOnExtensionBoard = _catsCcuFuseOnExtensionBoard.AlarmEnabledChecked;
                onlyInDatabase = false;
            }

            if (_devicesAlarmSetting.BlockAlarmFuseOnExtensionBoard != _catsCcuFuseOnExtensionBoard.BlockAlarmChecked)
            {
                _devicesAlarmSetting.BlockAlarmFuseOnExtensionBoard = _catsCcuFuseOnExtensionBoard.BlockAlarmChecked;
                onlyInDatabase = false;
            }

            if (_devicesAlarmSetting.EventlogDuringBlockAlarmFuseOnExtensionBoard !=
                _catsCcuFuseOnExtensionBoard.EventlogDuringBlockedAlarmChecked)
            {
                _devicesAlarmSetting.EventlogDuringBlockAlarmFuseOnExtensionBoard =
                    _catsCcuFuseOnExtensionBoard.EventlogDuringBlockedAlarmChecked;

                onlyInDatabase = false;
            }

            byte? blockObjectAlarmFuseOnExtensionBoardType = null;
            Guid? blockObjectAlarmFuseOnExtensionBoardId = null;

            if (_catsCcuFuseOnExtensionBoard.ObjectForBlockingAlarm != null)
            {
                blockObjectAlarmFuseOnExtensionBoardType =
                    (byte)_catsCcuFuseOnExtensionBoard.ObjectForBlockingAlarm.GetObjectType();

                blockObjectAlarmFuseOnExtensionBoardId =
                    (Guid)_catsCcuFuseOnExtensionBoard.ObjectForBlockingAlarm.GetId();
            }

            if (_devicesAlarmSetting.ObjBlockAlarmFuseOnExtensionBoardObjectType !=
                blockObjectAlarmFuseOnExtensionBoardType
                || _devicesAlarmSetting.ObjBlockAlarmFuseOnExtensionBoardId !=
                blockObjectAlarmFuseOnExtensionBoardId)
            {
                _devicesAlarmSetting.ObjBlockAlarmFuseOnExtensionBoardObjectType =
                    blockObjectAlarmFuseOnExtensionBoardType;

                _devicesAlarmSetting.ObjBlockAlarmFuseOnExtensionBoardId = blockObjectAlarmFuseOnExtensionBoardId;

                onlyInDatabase = false;
            }

            _devicesAlarmSetting.AlarmCCUFuseOnExtensionBoardPresentationGroup = _catsCcuFuseOnExtensionBoard.PresentationGroup;

            SaveAlarmArcs(
                _cmaaCcuFuseOnExtensionBoard.AlarmArcs,
                AlarmType.CCU_ExtFuse);

            if (_cmaaCcuFuseOnExtensionBoard.AlarmArcsWasChanged)
                onlyInDatabase = false;

            SaveAlarmArcs(
                _cmaaCcuOutdatedFirmware.AlarmArcs,
                AlarmType.CCU_OutdatedFirmware);

            SaveAlarmArcs(
                _cmaaCcuDataChannelDistrupted.AlarmArcs,
                AlarmType.CCU_DataChannelDistrupted);

            SaveAlarmArcs(
                _cmaaCcuCoprocessorFailure.AlarmArcs,
                AlarmType.CCU_CoprocessorFailure);

            if (_cmaaCcuCoprocessorFailure.AlarmArcsWasChanged)
                onlyInDatabase = false;

            SaveAlarmArcs(
                _cmaaCcuHighMemoryLoad.AlarmArcs,
                AlarmType.CCU_HighMemoryLoad);

            if (_cmaaCcuHighMemoryLoad.AlarmArcsWasChanged)
                onlyInDatabase = false;

            SaveAlarmArcs(
                _cmaaCcuFilesystemProblem.AlarmArcs,
                AlarmType.CCU_FilesystemProblem);

            if (_cmaaCcuFilesystemProblem.AlarmArcsWasChanged)
                onlyInDatabase = false;

            SaveAlarmArcs(
                _cmaaCcuSdCardNotFound.AlarmArcs,
                AlarmType.CCU_SdCardNotFound);

            if (_cmaaCcuSdCardNotFound.AlarmArcsWasChanged)
                onlyInDatabase = false;

            SaveAlarmArcs(
                _cmaaICcuSendingOfObjectStateFailed.AlarmArcs,
                AlarmType.ICCU_SendingOfObjectStateFailed);

            if (_cmaaICcuSendingOfObjectStateFailed.AlarmArcsWasChanged)
                onlyInDatabase = false;

            SaveAlarmArcs(
                _cmaaICcuPortAlreadyUsed.AlarmArcs,
                AlarmType.ICCU_PortAlreadyUsed);

            if (_cmaaICcuPortAlreadyUsed.AlarmArcsWasChanged)
                onlyInDatabase = false;

            _devicesAlarmSetting.AlarmCcuCatUnreachablePresentationGroup =
                _catsCcuCatUnreachable.PresentationGroup;

            _devicesAlarmSetting.AlarmCcuTransferToArcTimedOutPresentationGroup =
                _catsCcuTransferToArcTimedOut.PresentationGroup;

            if (_devicesAlarmSetting.AlarmCcuCatUnreachable != _catsCcuCatUnreachable.AlarmEnabledChecked)
            {
                _devicesAlarmSetting.AlarmCcuCatUnreachable = _catsCcuCatUnreachable.AlarmEnabledChecked;
                onlyInDatabase = false;
            }

            if (_devicesAlarmSetting.AlarmCcuTransferToArcTimedOut != _catsCcuTransferToArcTimedOut.AlarmEnabledChecked)
            {
                _devicesAlarmSetting.AlarmCcuTransferToArcTimedOut = _catsCcuTransferToArcTimedOut.AlarmEnabledChecked;
                onlyInDatabase = false;
            }
            return true;
        }

        private void SaveAlarmArcs(
            IEnumerable<AlarmArc> alarmArcs,
            AlarmType alarmType)
        {
            if (alarmArcs != null)
            {
                foreach (var alarmArc in alarmArcs)
                {
                    _devicesAlarmSetting.DevicesAlarmSettingAlarmArcs.Add(
                        new DevicesAlarmSettingAlarmArc(
                            _devicesAlarmSetting,
                            alarmArc,
                            alarmType));
                }
            }
        }

        private void SaveToDatabaseDevicesAlarmSettings(bool onlyInDatabase)
        {
            Exception error;

            bool retValue =
                onlyInDatabase
                    ? Plugin.MainServerProvider.DevicesAlarmSettings.UpdateOnlyInDatabase(_devicesAlarmSetting, out error)
                    : Plugin.MainServerProvider.DevicesAlarmSettings.Update(_devicesAlarmSetting, out error);

            if (!retValue && error != null)
            {
                if (error is SqlUniqueException && error.Message == DevicesAlarmSetting.COLUMN_GIN_FOR_ENTER_TO_MENU)
                {
                    ErrorNotificationOverControl(_eGinForEnterToMenu, "ErrorCodeAlreadyUsed");

                    _tcDevicesAlarmSetting.SelectedTab = _tpCrAlarms;
                    _eGinForEnterToMenu.Focus();
                }
            }
            else
            {
                SafeThread<DevicesAlarmSetting>.StartThread(GenerateDeviceAlarmSettingsChanged, _devicesAlarmSetting);
            }
        }

        private void GenerateDeviceAlarmSettingsChanged(DevicesAlarmSetting currentDeviceAlarmSettings)
        {
            var changedDeviceAlarmSettings = new Dictionary<AlarmType, bool>();

            if (_previousDevicesAlarmSettings.AlarmCCUOffline != currentDeviceAlarmSettings.AlarmCCUOffline)
            {
                changedDeviceAlarmSettings.Add(AlarmType.CCU_Offline, currentDeviceAlarmSettings.AlarmCCUOffline);
            }

            if (_previousDevicesAlarmSettings.AlarmCCUUnconfigured != currentDeviceAlarmSettings.AlarmCCUUnconfigured)
            {
                changedDeviceAlarmSettings.Add(AlarmType.CCU_Unconfigured, currentDeviceAlarmSettings.AlarmCCUUnconfigured);
            }

            if (_previousDevicesAlarmSettings.AlarmCCUTamperSabotage != currentDeviceAlarmSettings.AlarmCCUTamperSabotage)
            {
                changedDeviceAlarmSettings.Add(AlarmType.CCU_TamperSabotage, currentDeviceAlarmSettings.AlarmCCUTamperSabotage);
            }

            if (_previousDevicesAlarmSettings.AlarmDCUOffline != currentDeviceAlarmSettings.AlarmDCUOffline)
            {
                changedDeviceAlarmSettings.Add(AlarmType.DCU_Offline, currentDeviceAlarmSettings.AlarmDCUOffline);
            }

            if (_previousDevicesAlarmSettings.AlarmDCUTamperSabotage != currentDeviceAlarmSettings.AlarmDCUTamperSabotage)
            {
                changedDeviceAlarmSettings.Add(AlarmType.DCU_TamperSabotage, currentDeviceAlarmSettings.AlarmDCUTamperSabotage);
            }

            if (_previousDevicesAlarmSettings.AlarmCROffline != currentDeviceAlarmSettings.AlarmCROffline)
            {
                changedDeviceAlarmSettings.Add(AlarmType.CardReader_Offline, currentDeviceAlarmSettings.AlarmCROffline);
            }

            if (_previousDevicesAlarmSettings.AlarmCRTamperSabotage != currentDeviceAlarmSettings.AlarmCRTamperSabotage)
            {
                changedDeviceAlarmSettings.Add(AlarmType.CardReader_TamperSabotage, currentDeviceAlarmSettings.AlarmCRTamperSabotage);
            }
            if (_previousDevicesAlarmSettings.AllowAAToCRsReporting != currentDeviceAlarmSettings.AllowAAToCRsReporting)
            {
                changedDeviceAlarmSettings.Add(AlarmType.AlarmArea_ReportingToCR, currentDeviceAlarmSettings.AllowAAToCRsReporting);
            }
            if (_previousDevicesAlarmSettings.AlarmPrimaryPowerMissing != currentDeviceAlarmSettings.AlarmPrimaryPowerMissing)
            {
                changedDeviceAlarmSettings.Add(AlarmType.CCU_PrimaryPowerMissing, currentDeviceAlarmSettings.AlarmPrimaryPowerMissing);
            }
            if (_previousDevicesAlarmSettings.AlarmBatteryIsLow != currentDeviceAlarmSettings.AlarmBatteryIsLow)
            {
                changedDeviceAlarmSettings.Add(AlarmType.CCU_BatteryLow, currentDeviceAlarmSettings.AlarmBatteryIsLow);
            }
            if (_previousDevicesAlarmSettings.AlarmFuseOnExtensionBoard != currentDeviceAlarmSettings.AlarmFuseOnExtensionBoard)
            {
                changedDeviceAlarmSettings.Add(AlarmType.CCU_ExtFuse, currentDeviceAlarmSettings.AlarmFuseOnExtensionBoard);
            }
            if (_previousDevicesAlarmSettings.AlarmCcuCatUnreachable != currentDeviceAlarmSettings.AlarmCcuCatUnreachable)
            {
                changedDeviceAlarmSettings.Add(AlarmType.Ccu_CatUnreachable, currentDeviceAlarmSettings.AlarmCcuCatUnreachable);
            }
            if (_previousDevicesAlarmSettings.AlarmCcuTransferToArcTimedOut != currentDeviceAlarmSettings.AlarmCcuTransferToArcTimedOut)
            {
                changedDeviceAlarmSettings.Add(AlarmType.Ccu_TransferToArcTimedOut, currentDeviceAlarmSettings.AlarmCcuTransferToArcTimedOut);
            }

            if (changedDeviceAlarmSettings.Count > 0)
            {
                Plugin.MainServerProvider.DeviceAlarmSettingsChanged(changedDeviceAlarmSettings);
            }
        }

        // SB
        private void ValueChanged(object sender, EventArgs e)
        {
            _textChanged = true;

            SetEnableForApply();

            if (!_settingDefaultPassword && sender == _eGinForEnterToMenu)
            {
                var textBox = sender as TextBox;
                Debug.Assert(textBox != null);

                if (textBox.Text == DEFAULT_PASSWORD)
                {
                    textBox.Text = string.Empty;
                }
                else
                {
                    var validText = QuickParser.GetValidDigitString(textBox.Text);

                    var textLength = validText.Length;
                    var maximalCodeLength = GeneralOptionsForm.Singleton.MaximalCodeLength;

                    if (textLength > maximalCodeLength)
                    {
                        validText = validText.Substring(0, maximalCodeLength);
                        textLength = maximalCodeLength;
                    }

                    if (textBox.Text != validText)
                    {
                        textBox.Text = validText;
                        textBox.SelectionStart = textLength;
                        textBox.SelectionLength = 0;
                    }
                }
            }

            if (sender == _cbSlForEnterToMenu)
            {
                _tbmSecurityDailyPlanTimeZoneForEnterToMenu.Enabled =
                    ((ItemSlForEnterToMenu)_cbSlForEnterToMenu.SelectedItem).SlForEntetToMenu == SecurityLevel.SecurityTimeZoneSecurityDailyPlan;
            }
        }

        protected override void RegisterEvents()
        {
        }

        protected override void UnregisterEvents()
        {
        }

        private void _bApply_Click(object sender, EventArgs e)
        {
            if (SaveAlarmDeviceSettings())
            {
                _textChanged = false;
                SetEnableForApply();
                ModifyGridView(null);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _textChanged = false;
            SetEnableForApply();
            base.OnFormClosing(e);
        }

        private void OpenAllCheckStateChanged(Accordion accordion, CheckState checkState)
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

        private void _cbOpenAllDcuAlarms_CheckStateChanged(object sender, EventArgs e)
        {
            OpenAllCheckStateChanged(
                _accordionDcuAlarms,
                _cbOpenAllDcuAlarms.CheckState);

            _accordionDcuAlarms.Focus();
        }

        private void _cbOpenAllDsmAlarms_CheckStateChanged(object sender, EventArgs e)
        {
            OpenAllCheckStateChanged(
                _accordionDsmAlarms,
                _cbOpenAllDsmAlarms.CheckState);

            _accordionDsmAlarms.Focus();
        }

        private void _cbOpenAllCrAlarms_CheckStateChanged(object sender, EventArgs e)
        {
            OpenAllCheckStateChanged(
                _accordionCrAlarms,
                _cbOpenAllCrAlarms.CheckState);

            _accordionCrAlarms.Focus();
        }

        private void _cbOpenAllCcuAlarms_CheckStateChanged(object sender, EventArgs e)
        {
            OpenAllCheckStateChanged(
                _accordionCcuAlarms,
                _cbOpenAllCcuAlarms.CheckState);

            _accordionCcuAlarms.Focus();
        }

        public void Show(AlarmType alarmType)
        {
            Show();

            Action openAndScrollToControl;

            if (!_openAndScrollToControlByAlarmType.TryGetValue(
                alarmType,
                out openAndScrollToControl))
            {
                return;
            }

            AfterShow(openAndScrollToControl);
        }

        private void AfterShow(Action openAndScrollToControl)
        {
            if (InvokeRequired)
            {
                Invoke(
                    new Action<Action>(AfterShow),
                    openAndScrollToControl);

                return;
            }

            openAndScrollToControl();
        }

        private void _tbmSecurityDailyPlanTimeZoneForEnterToMenu_DoubleClick(object sender, EventArgs e)
        {
            if (_actSecurityDailyPlanForEnterToMenu != null)
            {
                NCASSecurityDailyPlansForm.Singleton.OpenEditForm(_actSecurityDailyPlanForEnterToMenu);
            }
            else if (_actSecurityTimeZoneForEnterToMenu != null)
            {
                NCASSecurityTimeZonesForm.Singleton.OpenEditForm(_actSecurityTimeZoneForEnterToMenu);
            }
        }

        private void _tbmScurityDailyPlanTimeZoneForEnterToMenu_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void _tbmScurityDailyPlanTimeZoneForEnterToMenu_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                var output = e.Data.GetFormats();
                if (output == null) return;

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

                ControlNotification.Singleton.Error(
                    NotificationPriority.JustOne,
                    _tbmSecurityDailyPlanTimeZoneForEnterToMenu.ImageTextBox,
                    GetString("ErrorWrongObjectType"),
                    ControlNotificationSettings.Default);
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        private void SetSecurityDailyPlanForEnterToMenu(SecurityDailyPlan securityDailyPlan)
        {
            ValueChanged(null, null);
            _actSecurityTimeZoneForEnterToMenu = null;
            _actSecurityDailyPlanForEnterToMenu = securityDailyPlan;

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
            ValueChanged(null, null);
            _actSecurityDailyPlanForEnterToMenu = null;
            _actSecurityTimeZoneForEnterToMenu = securityTimeZone;

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

        private void _tbmSecurityDailyPlanTimeZoneForEnterToMenu_ButtonPopupMenuItemClick(ToolStripItem item, int index)
        {
            if (item == _tsiModify)
            {
                ModifySecurityDailyPlanTimeZoneForEnterToMenu();
                return;
            }

            if (item == _tsiRemove)
            {
                if (_actSecurityDailyPlanForEnterToMenu != null)
                {
                    SetSecurityDailyPlanForEnterToMenu(null);
                }
                else
                {
                    SetSecurityTimeZoneForEnterToMenu(null);
                }

                return;
            }

            if (item == _tsiCreateSecDailyPlan)
            {
                var securityDailyPlan = new SecurityDailyPlan();
                if (NCASSecurityDailyPlansForm.Singleton.OpenInsertDialg(ref securityDailyPlan))
                {
                    SetSecurityDailyPlanForEnterToMenu(securityDailyPlan);
                }

                return;
            }

            if (item == _tsiCreateSecTimeZone)
            {
                var securityTimeZone = new SecurityTimeZone();
                if (NCASSecurityTimeZonesForm.Singleton.OpenInsertDialg(ref securityTimeZone))
                {
                    SetSecurityTimeZoneForEnterToMenu(securityTimeZone);
                }
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
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
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

        // SP
        private void LoadPersonAttributes()
        {
            var items = checkedListBoxPersons.Items;
            var allPersonAttributes = Plugin.MainServerProvider.PersonAttributes.GetAllPersonAttributes();

            items.Clear();

            foreach (var personAttribute in allPersonAttributes)
            {
                items.Add(personAttribute, personAttribute.IsWatched);
            }

            var output = Plugin.MainServerProvider.PersonAttributeOutputs.GetPersonAttributeOutput();
            if (output != null)
            {
                _cbADEnabled.Checked = output.IsEnabled;
                nmFailsCount.Value = output.FailsCount;
                SetInterval(output.Interval, new List<RadioButton> { rbDayAD, rbMonthAD, rbYearAD });
                var parts=output.Output.Split('#');
                if(parts!=null && parts.Length > 0)
                _tbADDir.Text = parts[0];
                if (parts != null && parts.Length > 1)
                    _tbADFileName.Text = parts[1];

                if (output.IdTimeZone != null)
                {
                    _actReportTimeZone=CgpClient.Singleton.MainServerProvider.TimeZones.GetObjectById(output.IdTimeZone);
                    RefreshTimeZoneControls(_actReportTimeZone, ref _tbmReportTimeZone);
                }
            }
        }

        private void LoadDoorEnvoromentReportsSettings()
        {

            var output = Plugin.MainServerProvider.ExcelReportOutputs.GetSettings();
            if (output != null)
            {
                cbDoorReport.Checked = output.IsEnabled;
                tbDoorFilename.Text = output.Filename;
                SetInterval(output.Interval, new List<RadioButton> { rbDay2, rbMonth2, rbYear2 });
                tbDoorDir.Text = output.Output;
                if(output.TimeZone!=null && output.TimeZone.IdTimeZone != Guid.Empty)
                   _actDoorEnvirometReportTimeZone = CgpClient.Singleton.MainServerProvider.TimeZones.GetObjectById(output.TimeZone.IdTimeZone);
                RefreshTimeZoneControls(_actDoorEnvirometReportTimeZone, ref tbmTimeZoneReportDoor);
            }
        }


        private void SavePersonAttributes()
        {
            var allPersonAttributes = checkedListBoxPersons.Items.Cast<PersonAttributeInfo>().ToList();

            int index = 0;

            foreach (var personAttribute in allPersonAttributes)
            {
                personAttribute.IsWatched = checkedListBoxPersons.GetItemChecked(index++);
            }

            Plugin.MainServerProvider.PersonAttributes.UpdatePersonAttributes(allPersonAttributes);

            var fn = _tbADFileName.Text;
            int fileExtPos = fn.LastIndexOf(".");
            if (fileExtPos >= 0)
                fn = fn.Substring(0, fileExtPos);
            var output = new PersonAttributeOutput()
            {
                IsEnabled = _cbADEnabled.Checked,
                Interval = GetInterval(new List<RadioButton> { rbDayAD, rbMonthAD, rbYearAD }),
                FailsCount = ((int)nmFailsCount.Value),
                Output = _tbADDir.Text.Replace(@"\\", @"\") + "#" + fn,
                IdTimeZone = _actReportTimeZone != null ? _actReportTimeZone.IdTimeZone : Guid.Empty
            };
            Plugin.MainServerProvider.PersonAttributeOutputs.CreateOrUpdate(ref output);
        }

        private void SaveDoorEnvoromentReportsSettings()
        {
            var fn = tbDoorFilename.Text;
            int fileExtPos = fn.LastIndexOf(".");
            if (fileExtPos >= 0)
                fn = fn.Substring(0, fileExtPos);

            var output = new ExcelReportOutput()
            {
                IsEnabled = cbDoorReport.Checked,
                Interval = GetInterval(new List<RadioButton> { rbDay2, rbMonth2, rbYear2 }),
                Output = tbDoorDir.Text.Replace(@"\\", @"\"),
                Filename = fn,
                Type="DoorEnviroment_Ajar",
                TimeZone = _actDoorEnvirometReportTimeZone 
            };
            Plugin.MainServerProvider.ExcelReportOutputs.CreateOrUpdate(ref output);
        }

        private int GetInterval(List<RadioButton> rbInetrval)
        {
            if (rbInetrval[0].Checked)  return 0;
            if(rbInetrval[1].Checked) return 1;
            if(rbInetrval[2].Checked) return 2;
            return 0;
        }

        private void SetInterval(int interval, List<RadioButton> rbInetrval)
        {
            rbInetrval[interval].Checked =true;
        }

        private void ForAllPersonAttributes(bool isWatched)
        {
            var count = checkedListBoxPersons.Items.Count;

            for (int index = 0; index < count; index++)
            {
                checkedListBoxPersons.SetItemChecked(index, isWatched);
            }
        }

        private void OnPersonsSelectAll(object sender, EventArgs e)
        {
            ForAllPersonAttributes(true);
        }

        private void OnPersonsUnselectAll(object sender, EventArgs e)
        {
            ForAllPersonAttributes(false);
        }


        private void _tbmReportTimeZone_TextBox_DoubleClick(object sender, EventArgs e)
        {
            if (_actReportTimeZone != null)
                TimeZonesForm.Singleton.OpenEditForm(_actReportTimeZone);

        }

        private void _tbmReportTimeZone_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                string[] output = e.Data.GetFormats();
                if (output == null) return;
                AddTimeZoneToControl((object)e.Data.GetData(output[0]), ref _actReportTimeZone, ref _tbmReportTimeZone);
            }
            catch
            {
            }
        }

        private void AddTimeZoneToControl(object newTimeZone, ref  Cgp.Server.Beans.TimeZone refTimeZone, ref TextBoxMenu tbm)
        {
            try
            {
                if (newTimeZone.GetType() == typeof(Cgp.Server.Beans.TimeZone))
                {
                    Cgp.Server.Beans.TimeZone timeZone = newTimeZone as Cgp.Server.Beans.TimeZone;
                    refTimeZone = timeZone;
                    RefreshTimeZoneControls(refTimeZone, ref tbm);
                    CgpClientMainForm.Singleton.AddToRecentList(newTimeZone);
                }
                else
                {
                        ControlNotification.Singleton.Error(NotificationPriority.JustOne, _tbmReportTimeZone.ImageTextBox,
                       CgpClient.Singleton.LocalizationHelper.GetString("ErrorWrongObjectType"), ControlNotificationSettings.Default);
                }
            }
            catch
            { }
        }

        private void RefreshTimeZoneControls(Cgp.Server.Beans.TimeZone refTimeZone, ref TextBoxMenu tbm)
        {
            if (refTimeZone != null)
            {
                tbm.Text = refTimeZone.ToString();
                tbm.TextImage = ObjectImageList.Singleton.GetImageForAOrmObject(refTimeZone);
            }

            else
                tbm.Text = string.Empty;
        }
        private void _tbmReportTimeZone_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void _tbmReportTimeZone_ButtonPopupMenuItemClick(ToolStripItem item, int index)
        {
            if (item.Name == "_tsiModify1")
            {
                ModifyReportTimeZone(ref _actReportTimeZone, ref _tbmReportTimeZone);
            }
            else if (item.Name == "_tsiRemove1")
            {
                _actReportTimeZone = null;
                RefreshTimeZoneControls(null, ref _tbmReportTimeZone);
            }
            else if (item.Name == "_tsiCreate1")
            {
                Contal.Cgp.Server.Beans.TimeZone timeZone = new Contal.Cgp.Server.Beans.TimeZone();
                if (TimeZonesForm.Singleton.OpenInsertDialg(ref timeZone))
                {
                    _actReportTimeZone = timeZone;
                    RefreshTimeZoneControls(_actReportTimeZone, ref _tbmReportTimeZone);
                }
            }
        }

        private void ModifyReportTimeZone(ref Cgp.Server.Beans.TimeZone refTimeZone, ref TextBoxMenu tbm)
        {
            if (CgpClient.Singleton.IsConnectionLost(true)) return;

            Exception error = null;
            try
            {
                List<IModifyObject> listObjects = new List<IModifyObject>();

                IList<IModifyObject> listTz = CgpClient.Singleton.MainServerProvider.TimeZones.ListModifyObjects(out error);
                if (error != null) throw error;
                listObjects.AddRange(listTz);

                ListboxFormAdd formAdd = new ListboxFormAdd(listObjects, CgpClient.Singleton.LocalizationHelper.GetString("TimeZonesFormTimeZonesForm"));
                IModifyObject outModObj;
                formAdd.ShowDialog(out outModObj);
                if (outModObj != null)
                {
                    refTimeZone = CgpClient.Singleton.MainServerProvider.TimeZones.GetObjectById(outModObj.GetId);
                    if (error != null) throw error;
                    RefreshTimeZoneControls(refTimeZone, ref tbm);
                    CgpClientMainForm.Singleton.AddToRecentList(_actReportTimeZone);
                }
            }
            catch
            {
                Contal.IwQuick.UI.Dialog.Error(error);
            }
        }

        private void tbmTimeZoneReportDoor_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                string[] output = e.Data.GetFormats();
                if (output == null) return;
                AddTimeZoneToControl((object)e.Data.GetData(output[0]), ref _actDoorEnvirometReportTimeZone, ref tbmTimeZoneReportDoor);
            }
            catch
            {
            }
        }

        private void tbmTimeZoneReportDoor_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void tbmTimeZoneReportDoor_ButtonPopupMenuItemClick(ToolStripItem item, int index)
        {
            if (item.Name == "_tsiModify2")
            {
                ModifyReportTimeZone(ref _actDoorEnvirometReportTimeZone, ref tbmTimeZoneReportDoor);
            }
            else if (item.Name == "_tsiRemove2")
            {
                _actDoorEnvirometReportTimeZone = null;
                RefreshTimeZoneControls(null, ref tbmTimeZoneReportDoor);
            }
            else if (item.Name == "_tsiCreate2")
            {
                Contal.Cgp.Server.Beans.TimeZone timeZone = new Contal.Cgp.Server.Beans.TimeZone();
                if (TimeZonesForm.Singleton.OpenInsertDialg(ref timeZone))
                {
                    _actDoorEnvirometReportTimeZone = timeZone;
                    RefreshTimeZoneControls(_actDoorEnvirometReportTimeZone, ref tbmTimeZoneReportDoor);
                }
            }
        }

        private void tbmTimeZoneReportDoor_ImageTextBox_Click(object sender, EventArgs e)
        {
            if (_actDoorEnvirometReportTimeZone != null)
                TimeZonesForm.Singleton.OpenEditForm(_actDoorEnvirometReportTimeZone);
        }

        private void _tpDoorEnvironmentAlarms_Resize(object sender, EventArgs e)
        {
            grDoorRep.Left = _accordionDsmAlarms.Left + _accordionDsmAlarms.Width + 10;
        }

        private void bDoorExportDir_Click(object sender, EventArgs e)
        {
            ServerDirectory sd = new ServerDirectory();
            string path;
            sd.ShowDialog(out path);
            if (!string.IsNullOrEmpty(path))
            {
                tbDoorDir.Text = path;
            }
        }

        private void grAccessDeniedReport_Enter(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void bADDir_Click(object sender, EventArgs e)
        {
            ServerDirectory sd = new ServerDirectory();
            string path;
            sd.ShowDialog(out path);
            if (!string.IsNullOrEmpty(path))
            {
                _tbADDir.Text = path;
            }
        }

        private void _tpCrAlarms_Click(object sender, EventArgs e)
        {

        }

        private void _tpCrAlarms_Resize(object sender, EventArgs e)
        {
            grAccessDeniedReport.Left = _accordionCrAlarms.Left + _accordionCrAlarms.Width+10;
        }

        private void _accordionCrAlarms_Resize(object sender, EventArgs e)
        {
            grAccessDeniedReport.Left = _accordionCrAlarms.Left + _accordionCrAlarms.Width+10;
        }

        private void _accordionDsmAlarms_Resize(object sender, EventArgs e)
        {
            grDoorRep.Left = _accordionDsmAlarms.Left + _accordionDsmAlarms.Width + 10;
        }
    }
}
