using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

using Cgp.Components;
using Contal.Cgp.Client;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Definitions;
using Contal.Cgp.NCAS.Globals;
using Contal.Cgp.NCAS.RemotingCommon;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.Server.Beans;
using Contal.Drivers.CardReader;
using Contal.IwQuick;
using Contal.IwQuick.Crypto;
using Contal.IwQuick.Data;
using Contal.IwQuick.Net;
using Contal.IwQuick.Sys;
using Contal.IwQuick.Threads;
using Contal.IwQuick.UI;
using Contal.LwRemoting2;

using CardReader = Contal.Cgp.NCAS.Server.Beans.CardReader;
using CRHWVersion = Contal.Drivers.CardReader.CRHWVersion;
using RequiredLicenceProperties = Contal.Cgp.NCAS.Globals.RequiredLicenceProperties;
using TimeZone = Contal.Cgp.Server.Beans.TimeZone;
using Contal.Cgp.Client.PluginSupport;
using Contal.IwQuick.PlatformPC.UI.Accordion;
using System.Net.NetworkInformation;
using Contal.Cgp.NCAS.Globals.PlatformPC;

namespace Contal.Cgp.NCAS.Client
{
    public partial class NCASCCUEditForm :
#if DESIGNER
    Form
#else
    ACgpPluginEditFormWithAlarmInstructions<CCU>
#endif
    {
        private ICollection<TimeZoneInfo> tzCollection;
        private bool _firstTimeInput = true;
        private bool _firstTimeOutput = true;
        private bool _firstTimeCardReaders = true;
        private bool? _loggedAsSuperAdmin;

        private BindingSource _bindingSourceInput = new BindingSource();
        private BindingSource _bindingSourceOutput = new BindingSource();
        private BindingSource _bindingSourceDoorEnvironments = new BindingSource();
        private BindingSource _bindingSourceCardReaders = new BindingSource();
        private BindingSource _bindingSourceDCUs = new BindingSource();
        private BindingSource _bindingSourceDCUUpgrades = new BindingSource();
        private BindingSource _bindingSourceCRUpgrades = new BindingSource();
        private IList<VerbosityLevelInfo> _verbosityLevels = new List<VerbosityLevelInfo>();

        private IPSetting _ipSetting;
        private Action<Guid, byte> _eventCCUStateChanged;
        private Action<Guid, byte, bool, bool> _eventCCUConfiguredChanged;
        private Action<Guid, string> _eventCCUMACAddressChanged;
        private Action<Guid, byte, Guid> _eventInputStateChanged;
        private Action<Guid, byte, Guid> _eventOutputStateChanged;
        private Action<Guid, byte, Guid> _eventOutputRealStateChanged;
        private Action<Guid, byte, Guid> _eventCardReaderOnlineStateChanged;
        private Action<Guid, OnlineState, Guid> _eventDCUOnlineStateChanged;
        private Action<DCUUpgradeState> _eventDCUsUpgradeProgressChanged;
        private Action<CRUpgradeState> _eventCRsUpgradeProgressChanged;
        private Action<Guid, IPSetting> _eventIpSettingsChanged;
        private Action<IPAddress, int, byte> _eventFileTransferProgressChanged;
        private Action<IPAddress, string> _eventCEUpgradeProgressChanged;
        private Action<IPAddress, int> _eventCCUUpgradeProgressChanged;
        private Action<Guid, int> _eventCCUMakeLogDumpProgressChanged;
        private Action<IPAddress, bool> _eventCCUUpgradeFinished;
        private Action<string, CUps2750Values> _eventCcuUpsMonitorValuesChanged;
        private Action<string, byte> _eventCcuUpsMonitorOnlineStateChanged;
        private Action<string, bool> _eventCcuUpsMonitorAlarmStateChanged;
        private Action<ObjectType, object, bool> _cudObjectEvent;
        private Action<IPAddress> _eventCCUKillFailed;

        private readonly Color _baseBackColorResultOfAction;
        private bool _firsTimeDgShow = true;
        private bool _firsTimeDgDcuUpgradeShow = true;
        private readonly ShowOptionsEditForm _showOption = ShowOptionsEditForm.Edit;
        private AOrmObject _timeZoneDailyPlan;
        private bool _ginChanged;
        private AlarmTransmitter _alarmTransmitter;

        private readonly ControlAlarmTypeSettings _catsCcuOffline;
        private readonly ControlModifyAlarmArcs _cmaaCcuOffline;
        private readonly ControlAlarmTypeSettings _catsCcuUpsBatteryFault;
        private readonly ControlModifyAlarmArcs _cmaaCcuUpsBatteryFault;
        private readonly ControlModifySpecialOutput _cmsoCcuUpsBatteryFault;
        private readonly ControlAlarmTypeSettings _catsCcuTamperSabotage;
        private readonly ControlModifyAlarmArcs _cmaaCcuTamperSabotage;
        private readonly ControlModifySpecialOutput _cmsoCcuTamperSabotage;
        private readonly ControlAlarmTypeSettings _catsCcuUpsOutputFuse;
        private readonly ControlModifyAlarmArcs _cmaaCcuUpsOutputFuse;
        private readonly ControlModifySpecialOutput _cmsoCcuUpsOutputFuse;
        private readonly ControlAlarmTypeSettings _catsCcuPrimaryPowerMissing;
        private readonly ControlModifyAlarmArcs _cmaaCcuPrimaryPowerMissing;
        private readonly ControlModifySpecialOutput _cmsoCcuPrimaryPowerMissing;
        private readonly ControlAlarmTypeSettings _catsCcuBatteryIsLow;
        private readonly ControlModifyAlarmArcs _cmaaCcuBatteryIsLow;
        private readonly ControlModifySpecialOutput _cmsoCcuBatteryIsLow;
        private readonly ControlAlarmTypeSettings _catsCcuFuseOnExtensionBoard;
        private readonly ControlModifyAlarmArcs _cmaaCcuFuseOnExtensionBoard;
        private readonly ControlModifySpecialOutput _cmsoCcuFuseOnExtensionBoard;
        private readonly ControlAlarmTypeSettings _catsCcuUpsTamper;
        private readonly ControlModifyAlarmArcs _cmaaCcuUpsTamper;
        private readonly ControlModifySpecialOutput _cmsoCcuUpsTamper;
        private readonly ControlAlarmTypeSettings _catsCcuUpsOvertemperature;
        private readonly ControlModifyAlarmArcs _cmaaCcuUpsOvertemperature;
        private readonly ControlModifySpecialOutput _cmsoCcuUpsOvertemperature;
        private readonly ControlAlarmTypeSettings _catsCcuUpsBatteryFuse;
        private readonly ControlModifyAlarmArcs _cmaaCcuUpsBatteryFuse;
        private readonly ControlModifySpecialOutput _cmsoCcuUpsBatteryFuse;
        private readonly ControlAlarmTypeSettings _catsCcuCatUnreachable;
        private readonly ControlAlarmTypeSettings _catsCcuTransferToArcTimedOut;

        private readonly Dictionary<AlarmType, Action> _openAndScrollToControlByAlarmType;

        public bool LoggedAsSuperAdmin
        {
            get
            {
                if (_loggedAsSuperAdmin == null)
                {
                    if (!CgpClient.Singleton.IsConnectionLost(false))
                        _loggedAsSuperAdmin =
                            CgpClient.Singleton.MainServerProvider.Logins.IsLoginSuperAdmin(CgpClient.Singleton.LoggedAs);
                    else
                        _loggedAsSuperAdmin = false;
                }
                return _loggedAsSuperAdmin.Value;
            }
        }

        //processing queue for dcu upgrade percentage
        private readonly ProcessingQueue<DCUUpgradeState> _progressDCUUpgradeChanged =
            new ProcessingQueue<DCUUpgradeState>();

        private NCASInputEditForm _inputInsertForm;
        private NCASOutputEditForm _outputInsertForm;
        private readonly CCU _ccu;

        public NCASCCUEditForm(
                CCU ccu,
                ShowOptionsEditForm showOption,
                PluginMainForm<NCASClient> myTableForm)
            : base(
                ccu,
                showOption,
                CgpClientMainForm.Singleton,
                myTableForm,
                NCASClient.LocalizationHelper)
        {
            _openAndScrollToControlByAlarmType = new Dictionary<AlarmType, Action>
            {
                {
                    AlarmType.CCU_Offline,
                    () =>
                    {
                        _tcCCU.SelectedTab = _tpAlarmSettings;
                        _accordionAlarmSettings.OpenAndScrollToControl(_catsCcuOffline);
                    }
                },
                {
                    AlarmType.CCU_TamperSabotage,
                    () =>
                    {
                        _tcCCU.SelectedTab = _tpAlarmSettings;
                        _accordionAlarmSettings.OpenAndScrollToControl(_catsCcuTamperSabotage);
                    }
                },
                {
                    AlarmType.CCU_PrimaryPowerMissing,
                    () =>
                    {
                        _tcCCU.SelectedTab = _tpAlarmSettings;
                        _accordionAlarmSettings.OpenAndScrollToControl(_catsCcuPrimaryPowerMissing);
                    }
                },
                {
                    AlarmType.CCU_BatteryLow,
                    () =>
                    {
                        _tcCCU.SelectedTab = _tpAlarmSettings;
                        _accordionAlarmSettings.OpenAndScrollToControl(_catsCcuBatteryIsLow);
                    }
                },
                {
                    AlarmType.Ccu_Ups_OutputFuse,
                    () =>
                    {
                        _tcCCU.SelectedTab = _tpAlarmSettings;
                        _accordionAlarmSettings.OpenAndScrollToControl(_catsCcuUpsOutputFuse);
                    }
                },
                {
                    AlarmType.Ccu_Ups_BatteryFault,
                    () =>
                    {
                        _tcCCU.SelectedTab = _tpAlarmSettings;
                        _accordionAlarmSettings.OpenAndScrollToControl(_catsCcuUpsBatteryFault);
                    }
                },
                {
                    AlarmType.Ccu_Ups_BatteryFuse,
                    () =>
                    {
                        _tcCCU.SelectedTab = _tpAlarmSettings;
                        _accordionAlarmSettings.OpenAndScrollToControl(_catsCcuUpsBatteryFuse);
                    }
                },
                {
                    AlarmType.Ccu_Ups_Overtemperature,
                    () =>
                    {
                        _tcCCU.SelectedTab = _tpAlarmSettings;
                        _accordionAlarmSettings.OpenAndScrollToControl(_catsCcuUpsOvertemperature);
                    }
                },
                {
                    AlarmType.Ccu_Ups_TamperSabotage,
                    () =>
                    {
                        _tcCCU.SelectedTab = _tpAlarmSettings;
                        _accordionAlarmSettings.OpenAndScrollToControl(_catsCcuUpsTamper);
                    }
                },
                {
                    AlarmType.CCU_ExtFuse,
                    () =>
                    {
                        _tcCCU.SelectedTab = _tpAlarmSettings;
                        _accordionAlarmSettings.OpenAndScrollToControl(_catsCcuFuseOnExtensionBoard);
                    }
                },
                {
                    AlarmType.Ccu_CatUnreachable,
                    () =>
                    {
                        _tcCCU.SelectedTab = _tpAlarmSettings;
                        _accordionAlarmSettings.OpenAndScrollToControl(_catsCcuCatUnreachable);
                    }
                },
                {
                    AlarmType.Ccu_TransferToArcTimedOut,
                    () =>
                    {
                        _tcCCU.SelectedTab = _tpAlarmSettings;
                        _accordionAlarmSettings.OpenAndScrollToControl(_catsCcuTransferToArcTimedOut);
                    }
                }
            };

            _ccu = ccu;
            _showOption = showOption;

            InitializeComponent();

            _cmsoCcuOffline.Plugin = Plugin;
            _cmsoCcuOffline.EditTextChanger += () => EditTextChanger(null, null);

            _catsCcuOffline = new ControlAlarmTypeSettings
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_catsCcuOffline",
                TabIndex = 0,
                Plugin = Plugin,
                ThreeStateCheckBoxes = true,
                AlarmEnabledCheckState = null,
                BlockAlarmCheckState = null,
                EventlogDuringBlockedAlarmVisible = false,
                PresentationGroupVisible = false
            };

            _catsCcuOffline.EditTextChanger += () => EditTextChanger(null, null);
            _catsCcuOffline.EditTextChangerOnlyInDatabase += () => EditTextChangerOnlyInDatabase(null, null);

            _cmaaCcuOffline = new ControlModifyAlarmArcs
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_cmaaCcuOffline",
                TabIndex = 1,
                Plugin = Plugin
            };

            _cmaaCcuOffline.EditTextChanger += () => EditTextChanger(null, null);

            _catsCcuOffline.AddControl(_cmaaCcuOffline);

            _accordionAlarmSettings.Add(_catsCcuOffline, "CCU offline").Name = "_chbCcuOffline";

            _catsCcuTamperSabotage = new ControlAlarmTypeSettings
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_catsCcuTamperSabotage",
                TabIndex = 2,
                Plugin = Plugin,
                ThreeStateCheckBoxes = true,
                AlarmEnabledCheckState = null,
                BlockAlarmCheckState = null,
                EventlogDuringBlockedAlarmCheckState = null,
                PresentationGroupVisible = false
            };

            _catsCcuTamperSabotage.EditTextChanger += () => EditTextChanger(null, null);
            _catsCcuTamperSabotage.EditTextChangerOnlyInDatabase += () => EditTextChangerOnlyInDatabase(null, null);

            _cmaaCcuTamperSabotage = new ControlModifyAlarmArcs
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_cmaaCcuTamperSabotage",
                TabIndex = 3,
                Plugin = Plugin
            };

            _cmaaCcuTamperSabotage.EditTextChanger += () => EditTextChanger(null, null);

            _catsCcuTamperSabotage.AddControl(_cmaaCcuTamperSabotage);

            _cmsoCcuTamperSabotage = new ControlModifySpecialOutput
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_cmsoCcuTamperSabotage",
                TabIndex = 4,
                Plugin = Plugin
            };

            _cmsoCcuTamperSabotage.EditTextChanger += () => EditTextChanger(null, null);

            _catsCcuTamperSabotage.AddControl(_cmsoCcuTamperSabotage);

            _accordionAlarmSettings.Add(_catsCcuTamperSabotage, "CCU tamper/sabotage").Name = "_chbCcuTamperSabotage";

            _catsCcuPrimaryPowerMissing = new ControlAlarmTypeSettings
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_catsCcuPrimaryPowerMissing",
                TabIndex = 5,
                Plugin = Plugin,
                ThreeStateCheckBoxes = true,
                AlarmEnabledCheckState = null,
                BlockAlarmCheckState = null,
                EventlogDuringBlockedAlarmCheckState = null,
                PresentationGroupVisible = false
            };

            _catsCcuPrimaryPowerMissing.EditTextChanger += () => EditTextChanger(null, null);
            _catsCcuPrimaryPowerMissing.EditTextChangerOnlyInDatabase += () => EditTextChangerOnlyInDatabase(null, null);

            _cmaaCcuPrimaryPowerMissing = new ControlModifyAlarmArcs
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_cmaaCcuPrimaryPowerMissing",
                TabIndex = 6,
                Plugin = Plugin
            };

            _cmaaCcuPrimaryPowerMissing.EditTextChanger += () => EditTextChanger(null, null);

            _catsCcuPrimaryPowerMissing.AddControl(_cmaaCcuPrimaryPowerMissing);

            _cmsoCcuPrimaryPowerMissing = new ControlModifySpecialOutput
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_cmsoCcuPrimaryPowerMissing",
                TabIndex = 7,
                Plugin = Plugin
            };

            _cmsoCcuPrimaryPowerMissing.EditTextChanger += () => EditTextChanger(null, null);

            _catsCcuPrimaryPowerMissing.AddControl(_cmsoCcuPrimaryPowerMissing);

            _accordionAlarmSettings.Add(_catsCcuPrimaryPowerMissing, "CCU primary power missing").Name = "_chbCcuPrimaryPowerMissing";

            _catsCcuBatteryIsLow = new ControlAlarmTypeSettings
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_catsCcuBatteryIsLow",
                TabIndex = 8,
                Plugin = Plugin,
                ThreeStateCheckBoxes = true,
                AlarmEnabledCheckState = null,
                BlockAlarmCheckState = null,
                EventlogDuringBlockedAlarmCheckState = null,
                PresentationGroupVisible = false
            };

            _catsCcuBatteryIsLow.EditTextChanger += () => EditTextChanger(null, null);
            _catsCcuBatteryIsLow.EditTextChangerOnlyInDatabase += () => EditTextChangerOnlyInDatabase(null, null);

            _cmaaCcuBatteryIsLow = new ControlModifyAlarmArcs
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_cmaaCcuBatteryIsLow",
                TabIndex = 9,
                Plugin = Plugin
            };

            _cmaaCcuBatteryIsLow.EditTextChanger += () => EditTextChanger(null, null);

            _catsCcuBatteryIsLow.AddControl(_cmaaCcuBatteryIsLow);

            _cmsoCcuBatteryIsLow = new ControlModifySpecialOutput
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_cmsoCcuBatteryIsLow",
                TabIndex = 10,
                Plugin = Plugin
            };

            _cmsoCcuBatteryIsLow.EditTextChanger += () => EditTextChanger(null, null);

            _catsCcuBatteryIsLow.AddControl(_cmsoCcuBatteryIsLow);

            _accordionAlarmSettings.Add(_catsCcuBatteryIsLow, "CCU battery is low").Name = "_chbCcuBatteryIsLow";

            _catsCcuUpsOutputFuse = new ControlAlarmTypeSettings
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_catsCcuUpsOutputFuse",
                TabIndex = 11,
                Plugin = Plugin,
                ThreeStateCheckBoxes = true,
                AlarmEnabledCheckState = null,
                BlockAlarmCheckState = null,
                EventlogDuringBlockedAlarmCheckState = null,
                PresentationGroupVisible = false
            };

            _catsCcuUpsOutputFuse.EditTextChanger += () => EditTextChanger(null, null);
            _catsCcuUpsOutputFuse.EditTextChangerOnlyInDatabase += () => EditTextChangerOnlyInDatabase(null, null);

            _cmaaCcuUpsOutputFuse = new ControlModifyAlarmArcs
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_cmaaCcuUpsOutputFuse",
                TabIndex = 12,
                Plugin = Plugin
            };

            _cmaaCcuUpsOutputFuse.EditTextChanger += () => EditTextChanger(null, null);

            _catsCcuUpsOutputFuse.AddControl(_cmaaCcuUpsOutputFuse);

            _cmsoCcuUpsOutputFuse = new ControlModifySpecialOutput
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_cmsoCcuUpsOutputFuse",
                TabIndex = 13,
                Plugin = Plugin
            };

            _cmsoCcuUpsOutputFuse.EditTextChanger += () => EditTextChanger(null, null);

            _catsCcuUpsOutputFuse.AddControl(_cmsoCcuUpsOutputFuse);

            _accordionAlarmSettings.Add(_catsCcuUpsOutputFuse, "UPS output fuse").Name = "_chbCcuUpsOutputFuse";

            _catsCcuUpsBatteryFault = new ControlAlarmTypeSettings
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_catsCcuUpsBatteryFault",
                TabIndex = 14,
                Plugin = Plugin,
                ThreeStateCheckBoxes = true,
                AlarmEnabledCheckState = null,
                BlockAlarmCheckState = null,
                EventlogDuringBlockedAlarmCheckState = null,
                PresentationGroupVisible = false
            };

            _catsCcuUpsBatteryFault.EditTextChanger += () => EditTextChanger(null, null);
            _catsCcuUpsBatteryFault.EditTextChangerOnlyInDatabase += () => EditTextChangerOnlyInDatabase(null, null);

            _cmaaCcuUpsBatteryFault = new ControlModifyAlarmArcs
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_cmaaCcuUpsBatteryFault",
                TabIndex = 15,
                Plugin = Plugin
            };

            _cmaaCcuUpsBatteryFault.EditTextChanger += () => EditTextChanger(null, null);

            _catsCcuUpsBatteryFault.AddControl(_cmaaCcuUpsBatteryFault);

            _cmsoCcuUpsBatteryFault = new ControlModifySpecialOutput
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_cmsoCcuUpsBatteryFault",
                TabIndex = 16,
                Plugin = Plugin
            };

            _cmsoCcuUpsBatteryFault.EditTextChanger += () => EditTextChanger(null, null);

            _catsCcuUpsBatteryFault.AddControl(_cmsoCcuUpsBatteryFault);

            _accordionAlarmSettings.Add(_catsCcuUpsBatteryFault, "UPS battery fault").Name = "_chbCcuUpsBatteryFault";

            _catsCcuUpsBatteryFuse = new ControlAlarmTypeSettings
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_catsCcuUpsBatteryFuse",
                TabIndex = 17,
                Plugin = Plugin,
                ThreeStateCheckBoxes = true,
                AlarmEnabledCheckState = null,
                BlockAlarmCheckState = null,
                EventlogDuringBlockedAlarmCheckState = null,
                PresentationGroupVisible = false
            };

            _catsCcuUpsBatteryFuse.EditTextChanger += () => EditTextChanger(null, null);
            _catsCcuUpsBatteryFuse.EditTextChangerOnlyInDatabase += () => EditTextChangerOnlyInDatabase(null, null);

            _cmaaCcuUpsBatteryFuse = new ControlModifyAlarmArcs
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_cmaaCcuUpsBatteryFuse",
                TabIndex = 18,
                Plugin = Plugin
            };

            _cmaaCcuUpsBatteryFuse.EditTextChanger += () => EditTextChanger(null, null);

            _catsCcuUpsBatteryFuse.AddControl(_cmaaCcuUpsBatteryFuse);

            _cmsoCcuUpsBatteryFuse = new ControlModifySpecialOutput
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_cmsoCcuUpsBatteryFuse",
                TabIndex = 19,
                Plugin = Plugin
            };

            _cmsoCcuUpsBatteryFuse.EditTextChanger += () => EditTextChanger(null, null);

            _catsCcuUpsBatteryFuse.AddControl(_cmsoCcuUpsBatteryFuse);

            _accordionAlarmSettings.Add(_catsCcuUpsBatteryFuse, "UPS battery fuse").Name = "_chbCcuUpsBatteryFuse";

            _catsCcuUpsOvertemperature = new ControlAlarmTypeSettings
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_catsCcuUpsOvertemperature",
                TabIndex = 20,
                Plugin = Plugin,
                ThreeStateCheckBoxes = true,
                AlarmEnabledCheckState = null,
                BlockAlarmCheckState = null,
                EventlogDuringBlockedAlarmCheckState = null,
                PresentationGroupVisible = false
            };

            _catsCcuUpsOvertemperature.EditTextChanger += () => EditTextChanger(null, null);
            _catsCcuUpsOvertemperature.EditTextChangerOnlyInDatabase += () => EditTextChangerOnlyInDatabase(null, null);

            _cmaaCcuUpsOvertemperature = new ControlModifyAlarmArcs
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_cmaaCcuUpsOvertemperature",
                TabIndex = 21,
                Plugin = Plugin
            };

            _cmaaCcuUpsOvertemperature.EditTextChanger += () => EditTextChanger(null, null);

            _catsCcuUpsOvertemperature.AddControl(_cmaaCcuUpsOvertemperature);

            _cmsoCcuUpsOvertemperature = new ControlModifySpecialOutput
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_cmsoCcuUpsOvertemperature",
                TabIndex = 22,
                Plugin = Plugin
            };

            _cmsoCcuUpsOvertemperature.EditTextChanger += () => EditTextChanger(null, null);

            _catsCcuUpsOvertemperature.AddControl(_cmsoCcuUpsOvertemperature);

            _accordionAlarmSettings.Add(_catsCcuUpsOvertemperature, "UPS overtemperature").Name = "_chbCcuUpsOvertemperature";

            _catsCcuUpsTamper = new ControlAlarmTypeSettings
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_catsCcuUpsTamper",
                TabIndex = 23,
                Plugin = Plugin,
                ThreeStateCheckBoxes = true,
                AlarmEnabledCheckState = null,
                BlockAlarmCheckState = null,
                EventlogDuringBlockedAlarmCheckState = null,
                PresentationGroupVisible = false
            };

            _catsCcuUpsTamper.EditTextChanger += () => EditTextChanger(null, null);
            _catsCcuUpsTamper.EditTextChangerOnlyInDatabase += () => EditTextChangerOnlyInDatabase(null, null);

            _cmaaCcuUpsTamper = new ControlModifyAlarmArcs
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_cmaaCcuUpsTamper",
                TabIndex = 24,
                Plugin = Plugin
            };

            _cmaaCcuUpsTamper.EditTextChanger += () => EditTextChanger(null, null);

            _catsCcuUpsTamper.AddControl(_cmaaCcuUpsTamper);

            _cmsoCcuUpsTamper = new ControlModifySpecialOutput
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_cmsoCcuUpsTamper",
                TabIndex = 25,
                Plugin = Plugin
            };

            _cmsoCcuUpsTamper.EditTextChanger += () => EditTextChanger(null, null);

            _catsCcuUpsTamper.AddControl(_cmsoCcuUpsTamper);

            _accordionAlarmSettings.Add(_catsCcuUpsTamper, "UPS tamper/sabotage").Name = "_chbCcuUpsTamper";

            _catsCcuFuseOnExtensionBoard = new ControlAlarmTypeSettings
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_catsCcuFuseOnExtensionBoard",
                TabIndex = 26,
                Plugin = Plugin,
                ThreeStateCheckBoxes = true,
                AlarmEnabledCheckState = null,
                BlockAlarmCheckState = null,
                EventlogDuringBlockedAlarmCheckState = null,
                PresentationGroupVisible = false
            };

            _catsCcuFuseOnExtensionBoard.EditTextChanger += () => EditTextChanger(null, null);
            _catsCcuFuseOnExtensionBoard.EditTextChangerOnlyInDatabase += () => EditTextChangerOnlyInDatabase(null, null);

            _cmaaCcuFuseOnExtensionBoard = new ControlModifyAlarmArcs
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_cmaaCcuFuseOnExtensionBoard",
                TabIndex = 27,
                Plugin = Plugin
            };

            _cmaaCcuFuseOnExtensionBoard.EditTextChanger += () => EditTextChanger(null, null);

            _catsCcuFuseOnExtensionBoard.AddControl(_cmaaCcuFuseOnExtensionBoard);

            _cmsoCcuFuseOnExtensionBoard = new ControlModifySpecialOutput
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_cmsoCcuFuseOnExtensionBoard",
                TabIndex = 28,
                Plugin = Plugin
            };

            _cmsoCcuFuseOnExtensionBoard.EditTextChanger += () => EditTextChanger(null, null);

            _catsCcuFuseOnExtensionBoard.AddControl(_cmsoCcuFuseOnExtensionBoard);

            _accordionAlarmSettings.Add(_catsCcuFuseOnExtensionBoard, "CCU fuse on extension board").Name = "_chbCcuFuseOnExtensionBoard";

            _catsCcuCatUnreachable = new ControlAlarmTypeSettings
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_catsCcuCatUnreachable",
                TabIndex = 29,
                Plugin = Plugin,
                ThreeStateCheckBoxes = true,
                AlarmEnabledCheckState = null,
                BlockAlarmVisible = false
            };

            _catsCcuCatUnreachable.EditTextChanger += () => EditTextChanger(null, null);
            _catsCcuCatUnreachable.EditTextChangerOnlyInDatabase += () => EditTextChangerOnlyInDatabase(null, null);

            _accordionAlarmSettings.Add(_catsCcuCatUnreachable, "CAT offline").Name = "_chbCcuCatUnreachable";

            _catsCcuTransferToArcTimedOut = new ControlAlarmTypeSettings
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Name = "_catsCcuTransferToArcTimedOut",
                TabIndex = 30,
                Plugin = Plugin,
                ThreeStateCheckBoxes = true,
                AlarmEnabledCheckState = null,
                BlockAlarmVisible = false
            };

            _catsCcuTransferToArcTimedOut.EditTextChanger += () => EditTextChanger(null, null);
            _catsCcuTransferToArcTimedOut.EditTextChangerOnlyInDatabase += () => EditTextChangerOnlyInDatabase(null, null);

            _accordionAlarmSettings.Add(_catsCcuTransferToArcTimedOut, "Alarm not transmitted to ARC").Name = "_chbCcuTransferToArcTimedOut";

            _accordionAlarmSettings.OpenedAllChnged +=
                isOpenAll => SetOpenAllCheckState(
                    _cbOpenAllAlarmSettings,
                    isOpenAll);

            SetOpenAllCheckState(_cbOpenAllAlarmSettings, _accordionAlarmSettings.IsOpenAll);

            SetTemplateBsiLevel();
            _dgDCUs.VisibleChanged += DgDCUVisibleChanged;
            _dgvDCUUpgrading.VisibleChanged += DgvDCUUpgradingVisibleChanged;
            _dgvInputCCU.VisibleChanged += DgvInputCCUVisibleChanged;
            _dgvOutputCCU.VisibleChanged += DgvOutputCCUVisibleChanged;
            _dgCardReaders.VisibleChanged += DgCardReadersVisibleChanged;
            _dgvCRUpgrading.VisibleChanged += DgCRUpgradingVisibleChanged;
            _lMACCCUInformation.Text = string.Empty;
            _lMainBoardTypeInformation.Text = string.Empty;
            _baseBackColorResultOfAction = _eResultOfAction.BackColor;
            WheelTabContorol = _tcCCU;

            // TODO extract to post init => do not set in constructor
            // ReSharper disable once DoNotCallOverridableMethodsInConstructor
            MinimumSize = new Size(Width, Height);

            _eDescription.MouseWheel += ControlMouseWheel;
            _eMaximumExpectedDCUCount.MouseWheel += ControlMouseWheel;
            _cbTimeZone.MouseWheel += ControlMouseWheel;
            _eHours.MouseWheel += ControlMouseWheel;
            _eMinutes.MouseWheel += ControlMouseWheel;
            _nZeno.MouseWheel += ControlMouseWheel;
            _nShortNormal.MouseWheel += ControlMouseWheel;
            _nNormalAlarm.MouseWheel += ControlMouseWheel;
            _nAlarmBreak.MouseWheel += ControlMouseWheel;
            _nBreak.MouseWheel += ControlMouseWheel;
            _cbTemplateBsiLevel.MouseWheel += ControlMouseWheel;
            _cbPortComBaudRate.MouseWheel += ControlMouseWheel;
            _lbUserFolders.MouseWheel += ControlMouseWheel;
            _progressDCUUpgradeChanged.ItemProcessing += DoChangeDCUsUpgradeProgress;
            FormClosing += NCASCCUEditForm_FormClosing;

            _gbSimulationCardSwiped.Visible = CgpClient.Singleton.IsLoggedIn;

            _dgvTestDcu.DataError += DgvTestDcuDataError;

            _lPersonsCards.Visible = false;
            _ePersonsCards.Visible = false;
            _tbdpSetTimeManually.LocalizationHelper = LocalizationHelper;
            ControlBox = true;
            SetReferenceEditColors();
            KeyDown += NCASCCUEditForm_KeyDown;

            _bMemoryCollect.Visible = LoggedAsSuperAdmin;
            SafeThread.StartThread(HideDisableTabPages);

            ObjectsPlacementHandler.AddImageList(_lbUserFolders);

            _tcCCU.TabPages.Remove(_tpPortSettings);

            _ccuUpgradeFiles.UpgradeAction = UpgradeCcu;
            _ccuUpgradeFiles.DeleteAction = DeleteCcuUpgradeFile;
            _ccuUpgradeFiles.InsertAction = UploadCcuUpgradeFile;
            _ccuUpgradeFiles.LocalizationHelper = LocalizationHelper;

            _ceUpgradeFiles.UpgradeAction = UpgradeCe;
            _ceUpgradeFiles.DeleteAction = DeleteCeUpgradeFile;
            _ceUpgradeFiles.InsertAction = UploadCeFile;
            _ceUpgradeFiles.LocalizationHelper = LocalizationHelper;

            _crUpgradeFiles.UpgradeAction = UpgradeCr;
            _crUpgradeFiles.DeleteAction = DeleteCrUpgradeFile;
            _crUpgradeFiles.InsertAction = UploadCrUpgradeFile;
            _crUpgradeFiles.LocalizationHelper = LocalizationHelper;

            _dcuUpgradeFiles.UpgradeAction = UpgradeDcu;
            _dcuUpgradeFiles.DeleteAction = DeleteDcuUpgradeFile;
            _dcuUpgradeFiles.InsertAction = InsertDcuUpgradeFile;
            _dcuUpgradeFiles.EnableFavorite = true;
            _dcuUpgradeFiles.FavoriteAction = SetDefaultDcuUpgradeFile;
            _dcuUpgradeFiles.LocalizationHelper = LocalizationHelper;

            if (!LoggedAsSuperAdmin)
                _tcStatistics.TabPages.Remove(_tpProcesses);
        }

        private void SetOpenAllCheckState(CheckBox checkBox, bool? isOpenAll)
        {
            checkBox.CheckState = isOpenAll.HasValue
                ? (isOpenAll.Value
                    ? CheckState.Checked
                    : CheckState.Unchecked)
                : CheckState.Indeterminate;
        }

        private void HideDisableTabPages()
        {
            try
            {
                HideDisableTabPageInformation(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.CcusInformationView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.CcusInformationAdmin)));

                HideDisableTabPageTimeSettings(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.CcusTimeSettingsView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.CcusTimeSettingsAdmin)));

                HideDisableTabPageInputsOutputs(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.CcusInputsOutputsView)),
                    NCASInputsForm.Singleton.HasAccessView() &&
                    NCASOutputsForm.Singleton.HasAccessView());

                HideDisableTabPageControl(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.CcusControlView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.CcusControlAdmin)));

                HideDisableTabPageSettings(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.CcusSettingsView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.CcusSettingsAdmin)));

                HideDisableTabPageDoorEnvironments(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.CcusDoorEnvironmentsView)),
                    NCASDoorEnvironmentsForm.Singleton.HasAccessView());

                HideDisableTabPageCrUpgrade(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.CcusCrUpgradePerform)));

                HideDisableTabPageIpSettings(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.CcusIpSettingsView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.CcusIpSettingsAdmin)));

                HideDisableTabPageDcuUpgrade(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.CcusDcuUpgradePerform)));

                HideDisableTabPageAlarmSettings(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.CcusAlarmSettingsView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.CcusAlarmSettingsAdmin)));

                HideDisableTabPageUpsMonitor(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.CcusUpsMonitorView)));

                HideDisableTabPageStatistics(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.CcusPerformanceStatisticsView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.CcusCommunicationStatisticsView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.CcusCommunicationStatisticsAdmin)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.CcusThreadMapView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.CcusStartsCountCopprocessorVersionView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.CcusStartsCountCopprocessorVersionAdmin)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.CcusBlockFilesInformationsView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.CcusBlockFilesInformationsAdmin)));

                HideDisableTabPageTesting(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.CcusTestingPerform)));

                HideDisableTabFoldersSructure(ObjectsPlacementHandler.HasAccessView());

                HideDisableTabReferencedBy(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.CcusReferencedByView)));

                HideDisableTabPageDescription(
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.CcusDescriptionView)),
                    CgpClient.Singleton.MainServerProvider.HasAccess(
                        NCASAccess.GetAccess(AccessNCAS.CcusDescriptionAdmin)));
            }
            catch
            {
            }
        }

        private void HideDisableTabPageInformation(bool view, bool admin)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool, bool>(HideDisableTabPageInformation),
                    view,
                    admin);
            }
            else
            {
                if (!admin)
                {
                    _eName.ReadOnly = true;
                }

                if (!view && !admin)
                {
                    _tcCCU.TabPages.Remove(_tpInformation);
                    return;
                }

                _tpInformation.Enabled = admin;
            }
        }

        private void HideDisableTabPageTimeSettings(bool view, bool admin)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool, bool>(HideDisableTabPageTimeSettings),
                    view,
                    admin);
            }
            else
            {
                if (!view && !admin)
                {
                    _tcCCU.TabPages.Remove(_tpTimeSettings);
                    return;
                }

                _tpTimeSettings.Enabled = admin;
            }
        }

        private void HideDisableTabPageInputsOutputs(bool view, bool admin)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool, bool>(HideDisableTabPageInputsOutputs),
                    view,
                    admin);
            }
            else
            {
                if (!view && !admin)
                {
                    _tcCCU.TabPages.Remove(_tpInputOutput);
                    return;
                }

                _tpInputOutput.Enabled = admin;
            }
        }

        private void HideDisableTabPageControl(bool view, bool admin)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool, bool>(HideDisableTabPageControl),
                    view,
                    admin);
            }
            else
            {
                if (!view && !admin)
                {
                    _tcCCU.TabPages.Remove(_tpControl);
                    return;
                }

                _tpControl.Enabled = admin;
            }
        }

        private void HideDisableTabPageSettings(bool view, bool admin)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool, bool>(HideDisableTabPageSettings),
                    view,
                    admin);
            }
            else
            {
                if (!view && !admin)
                {
                    _tcCCU.TabPages.Remove(_tpLevelBSI);
                    return;
                }

                _tpLevelBSI.Enabled = admin;
            }
        }

        private void HideDisableTabPageDoorEnvironments(bool view, bool admin)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool, bool>(HideDisableTabPageDoorEnvironments),
                    view,
                    admin);
            }
            else
            {
                if (!view && !admin)
                {
                    _tcCCU.TabPages.Remove(_tpDoorEnvironments);
                    return;
                }

                _tpDoorEnvironments.Enabled = admin;
            }
        }

        private void HideDisableTabPageCrUpgrade(bool perform)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool>(HideDisableTabPageCrUpgrade),
                    perform);
            }
            else
            {
                if (!perform)
                {
                    _tcCCU.TabPages.Remove(_tpCRUpgrade);
                }
            }
        }

        private void HideDisableTabPageIpSettings(bool view, bool admin)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool, bool>(HideDisableTabPageIpSettings),
                    view,
                    admin);
            }
            else
            {
                if (!view && !admin)
                {
                    _tcCCU.TabPages.Remove(_tpIPSettings);
                    return;
                }

                _tpIPSettings.Enabled = admin;
            }
        }

        private void HideDisableTabPageDcuUpgrade(bool perform)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool>(HideDisableTabPageDcuUpgrade),
                    perform);
            }
            else
            {
                if (!perform)
                {
                    _tcCCU.TabPages.Remove(_tpDCUsUpgrade);
                }
            }
        }

        private void HideDisableTabPageAlarmSettings(bool view, bool admin)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool, bool>(HideDisableTabPageAlarmSettings),
                    view,
                    admin);
            }
            else
            {
                if (!view && !admin)
                {
                    _tcCCU.TabPages.Remove(_tpAlarmSettings);
                    return;
                }

                _tpAlarmSettings.Enabled = admin;
            }
        }

        private void HideDisableTabPageUpsMonitor(bool view)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool>(HideDisableTabPageUpsMonitor),
                    view);
            }
            else
            {
                if (!view)
                {
                    _tcCCU.TabPages.Remove(_tpUpsMonitor);
                }
            }
        }

        private delegate void D8Bool2Void(
            bool param1, bool param2, bool param3, bool param4, bool param5, bool param6, bool param7, bool param8);

        private void HideDisableTabPageStatistics(bool performanceStatisticsView, bool communicationStatisticsView,
            bool communicationStatisticsAdmin, bool threadMapView, bool ccuStartsCountCoprocessorVersionView,
            bool ccuStartsCountCoprocessorVersionAdmin, bool blockFileInformationView, bool blockFileInformationAdmin)
        {
            if (InvokeRequired)
            {
                Invoke(new D8Bool2Void(HideDisableTabPageStatistics),
                    performanceStatisticsView,
                    communicationStatisticsView,
                    communicationStatisticsAdmin,
                    threadMapView,
                    ccuStartsCountCoprocessorVersionView,
                    ccuStartsCountCoprocessorVersionAdmin,
                    blockFileInformationView,
                    blockFileInformationAdmin);
            }
            else
            {
                if (!performanceStatisticsView &&
                    !communicationStatisticsView &&
                    !communicationStatisticsAdmin &&
                    !threadMapView &&
                    !ccuStartsCountCoprocessorVersionView &&
                    !ccuStartsCountCoprocessorVersionAdmin &&
                    !blockFileInformationView &&
                    !blockFileInformationAdmin)
                {
                    _tcCCU.TabPages.Remove(_tpStatistics);
                    return;
                }

                if (!performanceStatisticsView &&
                    !communicationStatisticsView &&
                    !communicationStatisticsAdmin)
                {
                    _tcStatistics.TabPages.Remove(_tpCommunicationAndMemory);
                }
                else
                {
                    _gbOtherStatistics.Visible = performanceStatisticsView;

                    if (communicationStatisticsView ||
                        communicationStatisticsAdmin)
                    {
                        if (!communicationStatisticsAdmin)
                        {
                            DisabledControls(_gbCommunicationStatistic);
                            _bRefreshCommunicationStatistic.Enabled = true;
                        }
                    }
                    else
                    {
                        _gbCommunicationStatistic.Visible = false;
                    }
                }

                if (!threadMapView)
                {
                    _tcStatistics.TabPages.Remove(_tpThreadMap);
                }

                if (!ccuStartsCountCoprocessorVersionView &&
                    !ccuStartsCountCoprocessorVersionAdmin &&
                    !blockFileInformationView &&
                    !blockFileInformationAdmin)
                {
                    _tcStatistics.TabPages.Remove(_tpOtherStatistics);
                }
                else
                {
                    if (ccuStartsCountCoprocessorVersionView ||
                        ccuStartsCountCoprocessorVersionAdmin)
                    {
                        _bCcuStartsCountReset.Enabled = ccuStartsCountCoprocessorVersionAdmin;
                    }
                    else
                    {
                        _gbCcuStartsCount.Visible = false;
                        _gbCoprocessorVersion.Visible = false;
                    }
                }

                if (!LoggedAsSuperAdmin)
                {
                    _tcStatistics.TabPages.Remove(_tpMemory);
                }
            }
        }

        private void HideDisableTabPageTesting(bool perform)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool>(HideDisableTabPageTesting),
                    perform);
            }
            else
            {
                if (!perform)
                {
                    _tcCCU.TabPages.Remove(_tpTesting);
                }
            }
        }

        private void HideDisableTabFoldersSructure(bool view)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool>(HideDisableTabFoldersSructure), view);
            }
            else
            {
                if (!view)
                    _tcCCU.TabPages.Remove(_tpUserFolders);
            }
        }

        private void HideDisableTabReferencedBy(bool view)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool>(HideDisableTabReferencedBy), view);
            }
            else
            {
                if (!view)
                    _tcCCU.TabPages.Remove(_tpReferencedBy);
            }
        }

        private void HideDisableTabPageDescription(bool view, bool admin)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool, bool>(HideDisableTabPageDescription),
                    view,
                    admin);
            }
            else
            {
                if (!view && !admin)
                {
                    _tcCCU.TabPages.Remove(_tpDescription);
                    return;
                }

                _tpDescription.Enabled = admin;
            }
        }

        private void NCASCCUEditForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _progressDCUUpgradeChanged.ItemProcessing -= DoChangeDCUsUpgradeProgress;
        }

        private void NCASCCUEditForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Delete)
            {
                _bDeleteEvents.Visible = true;
            }
        }

        private static void DgvTestDcuDataError(object sender, DataGridViewDataErrorEventArgs e)
        {
        }

        protected override void RegisterEvents()
        {
            RegisterServerEvents();
        }

        protected override void UnregisterEvents()
        {
            UnregisterServerEvents();
        }

        private void DgvDCUUpgradingVisibleChanged(object sender, EventArgs e)
        {
            if (_firsTimeDgDcuUpgradeShow)
            {
                ShowGridDCUsUpgrade();
                _firsTimeDgDcuUpgradeShow = false;
            }
        }

        /// <summary>
        /// If CCU contains Input || Output tabPage _tpInputOutput will be displayed
        /// </summary>
        private void ShowInputOutputTab()
        {
            if ((_editingObject.Inputs != null && _editingObject.Inputs.Count > 0)
                || (_editingObject.Outputs != null && _editingObject.Outputs.Count > 0))
            {
                if (!_tcCCU.Contains(_tpInputOutput))
                    _tcCCU.TabPages.Insert(3, _tpInputOutput);
            }
            else
            {
                if (_tcCCU.Contains(_tpInputOutput))
                    _tcCCU.TabPages.Remove(_tpInputOutput);
            }
        }

        /// <summary>
        /// If CCU contains DoorEnvironments tabPage _tpDoorEnvironments will be displayed
        /// </summary>
        private void ShowDoorEnvironmentsTab()
        {
            if ((_editingObject.DoorEnvironments != null && _editingObject.DoorEnvironments.Count > 0))
            {
                if (!_tcCCU.Contains(_tpDoorEnvironments))
                    _tcCCU.TabPages.Insert(5, _tpDoorEnvironments);
            }
            else
            {
                if (_tcCCU.Contains(_tpDoorEnvironments))
                    _tcCCU.TabPages.Remove(_tpDoorEnvironments);
            }
        }

        protected override void AfterTranslateForm()
        {
            if (Insert)
            {
                Text = GetString("CcuEditFormInsertText");
            }

            CgpClientMainForm.Singleton.SetTextOpenWindow(this);
            _eState.Text = GetString("Online");
            var state = Plugin.MainServerProvider.CCUs.GetCCUConfiguredState(_editingObject.IdCCU);
            TranslateIsCCUConfigured(state);

            if ((CCUOnlineState) state != CCUOnlineState.Online)
            {
                _bCrMarkAsDead.Text = GetString("General_bRemove");
                _bDcuMarkAsDead.Text = GetString("General_bRemove");
            }
            else if (_editingObject.IsConfigured)
            {
                LocalizationHelper.TranslateControl(_bCrMarkAsDead);
                LocalizationHelper.TranslateControl(_bDcuMarkAsDead);
            }
        }

        private void TranslateIsCCUConfigured(CCUConfigurationState state)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<CCUConfigurationState>(TranslateIsCCUConfigured), state);
            }
            else
            {
                TranslateCCUConfigured(state);
            }
        }

        protected override void BeforeInsert()
        {
            NCASCCUsForm.Singleton.BeforeInsert(this);
        }

        protected override void BeforeEdit()
        {
            NCASCCUsForm.Singleton.BeforeEdit(this, _editingObject);
        }

        protected override void AfterInsert()
        {
            NCASCCUsForm.Singleton.AfterInsert();
        }

        protected override void AfterEdit()
        {
            NCASCCUsForm.Singleton.AfterEdit(_editingObject);
        }

        private void DgvInputCCUVisibleChanged(object sender, EventArgs e)
        {
            if (_firstTimeInput)
            {
                SetInputToDgvValues();
                _firstTimeInput = false;
            }
        }

        private void DgvOutputCCUVisibleChanged(object sender, EventArgs e)
        {
            if (_firstTimeOutput)
            {
                SetOutputToDgvValues();
                _firstTimeOutput = false;
            }
        }

        private void DgDCUVisibleChanged(object sender, EventArgs e)
        {
            if (_firsTimeDgShow)
            {
                ShowGridDCUs();
                _firsTimeDgShow = false;
            }
        }

        private void DgCardReadersVisibleChanged(object sender, EventArgs e)
        {
            if (_firstTimeCardReaders)
            {
                SetCRDgValues();
                _firstTimeCardReaders = false;
            }
        }

        public override void ReloadEditingObject(out bool allowEdit)
        {
            Exception error;
            var obj = Plugin.MainServerProvider.CCUs.GetObjectForEdit(_editingObject.IdCCU, out error);
            if (error != null)
            {
                if (error is AccessDeniedException)
                {
                    allowEdit = false;
                    obj = Plugin.MainServerProvider.CCUs.GetObjectById(_editingObject.IdCCU);
                }
                else
                {
                    throw error;
                }
                DisableForm();
            }
            else
            {
                allowEdit = true;
            }

            _editingObject = obj;
        }

        public override void ReloadEditingObjectWithEditedData()
        {
            Exception error;
            Plugin.MainServerProvider.CCUs.RenewObjectForEdit(_editingObject.IdCCU, out error);
            if (error != null)
            {
                throw error;
            }
        }

        protected override void SetValuesInsert()
        {
            LoadSecurityLevelSettingsForCR();

            for (var i = _tcCCU.TabPages.Count - 1; i >= 0; i--)
            {
                _tcCCU.TabPages.RemoveAt(i);
            }

            _tcCCU.TabPages.Add(_tpInformation);
            _tcCCU.TabPages.Add(_tpDescription);

            _bPrecreateDCU.Enabled = false;
            _editingObject.TimeZoneInfo = TimeZoneInfo.Local.Id;
            _editingObject.TimeShift = (int) TimeZoneInfo.Local.BaseUtcOffset.TotalMinutes;
            FillTimeZone();
            var Time = _editingObject.TimeShift;
            _eTime.Text = string.Empty;
            // ReSharper disable once PossibleLossOfFraction
            _eHours.Value = Time/60;
            _eMinutes.Value = Time%60;
            _eMaximumExpectedDCUCount.Value = _editingObject.MaxNodeLookupSequence;
            ShowInputOutputTab();
            ShowDoorEnvironmentsTab();
            SetBsiLevel();
            ShowBaudRate();
            ShowTimeZoneShift();
            _chbInheritedGeneralNtpSettings.Checked = true;
            _crCountChanged = false;

            _lMainBoardTypeInformation.Visible = false;
            _cbMainboarType.Visible = true;
            FillCbMainboardType();
            _bApply.Enabled = false;
            EnabelDisableNTPSettings();

            _eIndex.Value = Plugin.MainServerProvider.CCUs.GetNewIndex();
        }

        protected override void SetValuesEdit()
        {
            LoadSecurityLevelSettingsForCR();
            FillTimeZone();

#if DEBUG || RELEASE_SPECIAL
            _gbTestRoutineDCU.Visible = true;
#else
            this._gbTestRoutineDCU.Visible = false;
#endif

            _eName.Text = _editingObject.Name;
            _eIPAddress.Text = _editingObject.IPAddress;
            _eDescription.Text = _editingObject.Description;
            var Time = _editingObject.TimeShift;

            // ReSharper disable once PossibleLossOfFraction
            _eHours.Value = Time/60;

            _eMinutes.Value = Time%60;
            _lMACCCUInformation.Text = _editingObject.MACAddress;

            var alarmArcsByAlarmType = new SyncDictionary<AlarmType, ICollection<AlarmArc>>();

            if (_editingObject.CcuAlarmArcs != null)
            {
                foreach (var ccuAlarmArc in _editingObject.CcuAlarmArcs)
                {
                    var alarmArc = ccuAlarmArc.AlarmArc;

                    alarmArcsByAlarmType.GetOrAddValue(
                        (AlarmType) ccuAlarmArc.AlarmType,
                        key =>
                            new LinkedList<AlarmArc>(),
                        (key, value, newlyAdded) =>
                            value.Add(alarmArc));
                }
            }

            _catsCcuOffline.AlarmEnabledCheckState = _editingObject.AlarmOffline;
            _catsCcuOffline.BlockAlarmCheckState = _editingObject.BlockAlarmOffline;

            _catsCcuOffline.ObjectForBlockingAlarm = GetBlockAlarmObject(
                _editingObject.ObjBlockAlarmOfflineObjectType,
                _editingObject.ObjBlockAlarmOfflineId);

            _cmaaCcuOffline.AlarmArcs = GetAlarmArcs(
                alarmArcsByAlarmType,
                AlarmType.CCU_Offline);

            _cmsoCcuOffline.SpecialOutput = _editingObject.WatchdogOutput;
            _cmsoCcuOffline.SetParentCcu(_editingObject.IdCCU);

            _catsCcuTamperSabotage.AlarmEnabledCheckState = _editingObject.AlarmTamper;
            _catsCcuTamperSabotage.BlockAlarmCheckState = _editingObject.BlockAlarmTamper;
            _catsCcuTamperSabotage.EventlogDuringBlockedAlarmCheckState = _editingObject.EventlogDuringBlockAlarmTamper;
            _catsCcuTamperSabotage.SetParentCcu(_editingObject.IdCCU);

            _catsCcuTamperSabotage.ObjectForBlockingAlarm = GetBlockAlarmObject(
                _editingObject.ObjBlockAlarmTamperObjectType,
                _editingObject.ObjBlockAlarmTamperId);

            _cmaaCcuTamperSabotage.AlarmArcs = GetAlarmArcs(
                alarmArcsByAlarmType,
                AlarmType.CCU_TamperSabotage);

            _cmsoCcuTamperSabotage.SpecialOutput = _editingObject.OutputTamper;
            _cmsoCcuTamperSabotage.SetParentCcu(_editingObject.IdCCU);

            _catsCcuPrimaryPowerMissing.AlarmEnabledCheckState = _editingObject.AlarmPrimaryPowerMissing;
            _catsCcuPrimaryPowerMissing.BlockAlarmCheckState = _editingObject.BlockAlarmPrimaryPowerMissing;
            _catsCcuPrimaryPowerMissing.EventlogDuringBlockedAlarmCheckState = _editingObject.EventlogDuringBlockAlarmPrimaryPowerMissing;
            _catsCcuPrimaryPowerMissing.SetParentCcu(_editingObject.IdCCU);

            _catsCcuPrimaryPowerMissing.ObjectForBlockingAlarm = GetBlockAlarmObject(
                _editingObject.ObjBlockAlarmPrimaryPowerMissingObjectType,
                _editingObject.ObjBlockAlarmPrimaryPowerMissingId);

            _cmaaCcuPrimaryPowerMissing.AlarmArcs = GetAlarmArcs(
                alarmArcsByAlarmType,
                AlarmType.CCU_PrimaryPowerMissing);

            _cmsoCcuPrimaryPowerMissing.SpecialOutput = _editingObject.OutputPrimaryPowerMissing;
            _cmsoCcuPrimaryPowerMissing.SetParentCcu(_editingObject.IdCCU);

            _catsCcuBatteryIsLow.AlarmEnabledCheckState = _editingObject.AlarmBatteryIsLow;
            _catsCcuBatteryIsLow.BlockAlarmCheckState = _editingObject.BlockAlarmBatteryIsLow;
            _catsCcuBatteryIsLow.EventlogDuringBlockedAlarmCheckState = _editingObject.EventlogDuringBlockAlarmBatteryIsLow;
            _catsCcuBatteryIsLow.SetParentCcu(_editingObject.IdCCU);

            _catsCcuBatteryIsLow.ObjectForBlockingAlarm = GetBlockAlarmObject(
                _editingObject.ObjBlockAlarmBatteryIsLowObjectType,
                _editingObject.ObjBlockAlarmBatteryIsLowId);

            _cmaaCcuBatteryIsLow.AlarmArcs = GetAlarmArcs(
                alarmArcsByAlarmType,
                AlarmType.CCU_BatteryLow);

            _cmsoCcuBatteryIsLow.SpecialOutput = _editingObject.OutputBatteryIsLow;
            _cmsoCcuBatteryIsLow.SetParentCcu(_editingObject.IdCCU);

            _catsCcuUpsOutputFuse.AlarmEnabledCheckState = _editingObject.AlarmUpsOutputFuse;
            _catsCcuUpsOutputFuse.BlockAlarmCheckState = _editingObject.BlockAlarmUpsOutputFuse;
            _catsCcuUpsOutputFuse.EventlogDuringBlockedAlarmCheckState = _editingObject.EventlogDuringBlockAlarmUpsOutputFuse;
            _catsCcuUpsOutputFuse.SetParentCcu(_editingObject.IdCCU);

            _catsCcuUpsOutputFuse.ObjectForBlockingAlarm = GetBlockAlarmObject(
                _editingObject.ObjBlockAlarmUpsOutputFuseObjectType,
                _editingObject.ObjBlockAlarmUpsOutputFuseId);

            _cmaaCcuUpsOutputFuse.AlarmArcs = GetAlarmArcs(
                alarmArcsByAlarmType,
                AlarmType.Ccu_Ups_OutputFuse);

            _cmsoCcuUpsOutputFuse.SpecialOutput = _editingObject.OutputUpsOutputFuse;
            _cmsoCcuUpsOutputFuse.SetParentCcu(_editingObject.IdCCU);

            _catsCcuUpsBatteryFault.AlarmEnabledCheckState = _editingObject.AlarmUpsBatteryFault;
            _catsCcuUpsBatteryFault.BlockAlarmCheckState = _editingObject.BlockAlarmUpsBatteryFault;
            _catsCcuUpsBatteryFault.EventlogDuringBlockedAlarmCheckState = _editingObject.EventlogDuringBlockAlarmUpsBatteryFault;
            _catsCcuUpsBatteryFault.SetParentCcu(_editingObject.IdCCU);

            _catsCcuUpsBatteryFault.ObjectForBlockingAlarm = GetBlockAlarmObject(
                _editingObject.ObjBlockAlarmUpsBatteryFaultObjectType,
                _editingObject.ObjBlockAlarmUpsBatteryFaultId);

            _cmaaCcuUpsBatteryFault.AlarmArcs = GetAlarmArcs(
                alarmArcsByAlarmType,
                AlarmType.Ccu_Ups_BatteryFault);

            _cmsoCcuUpsBatteryFault.SpecialOutput = _editingObject.OutputUpsBatteryFault;
            _cmsoCcuUpsBatteryFault.SetParentCcu(_editingObject.IdCCU);

            _catsCcuUpsBatteryFuse.AlarmEnabledCheckState = _editingObject.AlarmUpsBatteryFuse;
            _catsCcuUpsBatteryFuse.BlockAlarmCheckState = _editingObject.BlockAlarmUpsBatteryFuse;
            _catsCcuUpsBatteryFuse.EventlogDuringBlockedAlarmCheckState = _editingObject.EventlogDuringBlockAlarmUpsBatteryFuse;
            _catsCcuUpsBatteryFuse.SetParentCcu(_editingObject.IdCCU);

            _catsCcuUpsBatteryFuse.ObjectForBlockingAlarm = GetBlockAlarmObject(
                _editingObject.ObjBlockAlarmUpsBatteryFuseObjectType,
                _editingObject.ObjBlockAlarmUpsBatteryFuseId);

            _cmaaCcuUpsBatteryFuse.AlarmArcs = GetAlarmArcs(
                alarmArcsByAlarmType,
                AlarmType.Ccu_Ups_BatteryFuse);

            _cmsoCcuUpsBatteryFuse.SpecialOutput = _editingObject.OutputUpsBatteryFuse;
            _cmsoCcuUpsBatteryFuse.SetParentCcu(_editingObject.IdCCU);

            _catsCcuUpsOvertemperature.AlarmEnabledCheckState = _editingObject.AlarmUpsOvertemperature;
            _catsCcuUpsOvertemperature.BlockAlarmCheckState = _editingObject.BlockAlarmUpsOvertemperature;
            _catsCcuUpsOvertemperature.EventlogDuringBlockedAlarmCheckState = _editingObject.EventlogDuringBlockAlarmUpsOvertemperature;
            _catsCcuUpsOvertemperature.SetParentCcu(_editingObject.IdCCU);

            _catsCcuUpsOvertemperature.ObjectForBlockingAlarm = GetBlockAlarmObject(
                _editingObject.ObjBlockAlarmUpsOvertemperatureObjectType,
                _editingObject.ObjBlockAlarmUpsOvertemperatureId);

            _cmaaCcuUpsOvertemperature.AlarmArcs = GetAlarmArcs(
                alarmArcsByAlarmType,
                AlarmType.Ccu_Ups_Overtemperature);

            _cmsoCcuUpsOvertemperature.SpecialOutput = _editingObject.OutputUpsOvertemperature;
            _cmsoCcuUpsOvertemperature.SetParentCcu(_editingObject.IdCCU);

            _catsCcuUpsTamper.AlarmEnabledCheckState = _editingObject.AlarmUpsTamperSabotage;
            _catsCcuUpsTamper.BlockAlarmCheckState = _editingObject.BlockAlarmUpsTamperSabotage;
            _catsCcuUpsTamper.EventlogDuringBlockedAlarmCheckState = _editingObject.EventlogDuringBlockAlarmUpsTamperSabotage;
            _catsCcuUpsTamper.SetParentCcu(_editingObject.IdCCU);

            _catsCcuUpsTamper.ObjectForBlockingAlarm = GetBlockAlarmObject(
                _editingObject.ObjBlockAlarmUpsTamperSabotageObjectType,
                _editingObject.ObjBlockAlarmUpsTamperSabotageId);

            _cmaaCcuUpsTamper.AlarmArcs = GetAlarmArcs(
                alarmArcsByAlarmType,
                AlarmType.Ccu_Ups_TamperSabotage);

            _cmsoCcuUpsTamper.SpecialOutput = _editingObject.OutputUpsTamperSabotage;
            _cmsoCcuUpsTamper.SetParentCcu(_editingObject.IdCCU);

            _catsCcuFuseOnExtensionBoard.AlarmEnabledCheckState = _editingObject.AlarmFuseOnExtensionBoard;
            _catsCcuFuseOnExtensionBoard.BlockAlarmCheckState = _editingObject.BlockAlarmFuseOnExtensionBoard;
            _catsCcuFuseOnExtensionBoard.EventlogDuringBlockedAlarmCheckState = _editingObject.EventlogDuringBlockAlarmFuseOnExtensionBoard;
            _catsCcuFuseOnExtensionBoard.SetParentCcu(_editingObject.IdCCU);

            _catsCcuFuseOnExtensionBoard.ObjectForBlockingAlarm = GetBlockAlarmObject(
                _editingObject.ObjBlockAlarmFuseOnExtensionBoardObjectType,
                _editingObject.ObjBlockAlarmFuseOnExtensionBoardId);

            _cmaaCcuFuseOnExtensionBoard.AlarmArcs = GetAlarmArcs(
                alarmArcsByAlarmType,
                AlarmType.CCU_ExtFuse);

            _cmsoCcuFuseOnExtensionBoard.SpecialOutput = _editingObject.OutputFuseOnExtensionBoard;
            _cmsoCcuFuseOnExtensionBoard.SetParentCcu(_editingObject.IdCCU);

            _catsCcuCatUnreachable.AlarmEnabledCheckState = _editingObject.AlarmCcuCatUnreachable;         
            
            _catsCcuCatUnreachable.PresentationGroup =
                _editingObject.AlarmCcuCatUnreachablePresentationGroup;

            _catsCcuTransferToArcTimedOut.AlarmEnabledCheckState = _editingObject.AlarmCcuTransferToArcTimedOut;
            
            _catsCcuTransferToArcTimedOut.PresentationGroup =
                _editingObject.AlarmCcuTransferToArcTimedOutPresentationGroup;

            _cbSyncingTimeFromServer.CheckState =
                _editingObject.SyncingTimeFromServer == null
                    ? CheckState.Indeterminate
                    : _editingObject.SyncingTimeFromServer.Value
                        ? CheckState.Checked
                        : CheckState.Unchecked;

            EnabelDisableNTPSettings();

            LoadSNTPIpAddressesHostNames(_editingObject.SNTPIpAddresses);
            _chbInheritedGeneralNtpSettings.Checked = _editingObject.InheritGeneralNtpSettings;

            SetBsiLevel();
            ShowInputOutputTab();
            ShowDoorEnvironmentsTab();

            GetInformationFromServer();

            SafeThread.StartThread(ShowCardReaders);
            SafeThread.StartThread(ShowDCUs);
            SafeThread.StartThread(ShowDCUsUpgrade);

            //IwQuick.Threads.SafeThread.StartThread(new DVoid2Void(ShowDoorEnvironments));
            if (_editingObject.MaxNodeLookupSequence <= _eMaximumExpectedDCUCount.Maximum)
            {
                _eMaximumExpectedDCUCount.Value = _editingObject.MaxNodeLookupSequence;
            }

            SafeThread.StartThread(SetFormByMainboardType);

            ShowBaudRate();

            SafeThread.StartThread(ShowTimeZoneShift);

            _alarmTransmitter = _editingObject.AlarmTransmitter;
            ShowAlarmTransmitter();

            SetReferencedBy();

            _crCountChanged = false;
            _bApply.Enabled = false;

            if (!CgpClientMainForm.Singleton.MainIsConnectionLost(false))
            {
                SafeThread.StartThread(ObtainWinCEImageVersion);
                SafeThread.StartThread(delegate
                {
                    SetRemoveEventsButtonVisibility();
                    SetThreadMapVisibility();
                });
            }

            if (_configured == CCUConfigurationState.ConfiguredForThisServerUpgradeOnly
                || _configured == CCUConfigurationState.ConfiguredForThisServerUpgradeOnlyWinCe)
                SafeThread.StartThread(ShowInfoUnsupportedCCU);

            _tbR1.Text = _editingObject.R1 ?? "1";
            _tbR2.Text = _editingObject.R2 ?? "1";

            _eIndex.Value = _editingObject.IndexCCU;
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

        private void SetVerbosityLevelControl()
        {
            lock (_verbosityLevels)
            {
                _verbosityLevels = Plugin.MainServerProvider.GetVerbosityLevelsInfo(_editingObject.IdCCU);
            }
            if (_verbosityLevels == null || _verbosityLevels.Count == 0)
                return;

            var currentVerbosity = Plugin.MainServerProvider.GetCurrentVerbosity(_editingObject.IdCCU);

            Invoke(new MethodInvoker(delegate
            {
                _cbVerbosityLevel.DataSource =
                    _verbosityLevels.Select(value => new {Value = value, Display = value.ToString()}).ToArray();
                _cbVerbosityLevel.ValueMember = "Value";
                _cbVerbosityLevel.DisplayMember = "Display";

                try
                {
                    _nudVerbosityLevel.Value = currentVerbosity;
                    SetVerbosityComboBox(currentVerbosity);
                }
                catch
                {
                }
            }));
        }

        private AOnOffObject GetBlockAlarmObject(byte? objectType, Guid? objectGuid)
        {
            if (!CgpClientMainForm.Singleton.MainIsConnectionLost(true) && objectType != null && objectGuid != null)
            {
                switch (objectType.Value)
                {
                    case (byte) ObjectType.DailyPlan:
                        return CgpClient.Singleton.MainServerProvider.DailyPlans.GetObjectById(objectGuid.Value);
                    case (byte) ObjectType.TimeZone:
                        return CgpClient.Singleton.MainServerProvider.TimeZones.GetObjectById(objectGuid.Value);
                    case (byte) ObjectType.Input:
                        return Plugin.MainServerProvider.Inputs.GetObjectById(objectGuid.Value);
                    case (byte) ObjectType.Output:
                        return Plugin.MainServerProvider.Outputs.GetObjectById(objectGuid.Value);
                }
            }

            return null;
        }

        private void SetThreadMapVisibility()
        {
            if (InvokeRequired)
                Invoke(new MethodInvoker(SetThreadMapVisibility));
            else
            {
                if (!LoggedAsSuperAdmin && _tcStatistics.TabPages.ContainsKey("_tpThreadMap"))
                    _tcStatistics.TabPages.RemoveByKey("_tpThreadMap");
            }
        }

        private ServerGeneralOptions _serverGeneralOptions;

        private void EnabelDisableNTPSettings()
        {
            SafeThread.StartThread(DoEnabelDisableNTPSettings);
        }

        private void DoEnabelDisableNTPSettings()
        {
            try
            {
                if (_serverGeneralOptions == null)
                {
                    if (CgpClientMainForm.Singleton.MainIsConnectionLost(false))
                        return;

                    _serverGeneralOptions =
                        CgpClient.Singleton.MainServerProvider.ServerGenaralOptionsProvider.ReturnServerGeneralOptions();
                }

                if (_serverGeneralOptions != null)
                {
                    Invoke((MethodInvoker) delegate
                    {
                        var enableNTPSettings = true;

                        switch (_cbSyncingTimeFromServer.CheckState)
                        {
                            case CheckState.Checked:
                                enableNTPSettings = false;
                                break;
                            case CheckState.Indeterminate:
                                enableNTPSettings = !_serverGeneralOptions.SyncingTimeFromServer;
                                break;
                        }

                        _gbNtpSettings.Enabled = enableNTPSettings;
                    });
                }
            }
            catch
            {
            }
        }

        private void SetRemoveEventsButtonVisibility()
        {
            if (InvokeRequired)
                Invoke(new MethodInvoker(SetRemoveEventsButtonVisibility));
            else
            {
                if (LoggedAsSuperAdmin)
                {
                    _bDeleteEvents.Visible = Plugin.MainServerProvider.CCUs.ShowDeleteEvents();
                }
            }
        }

        private void ShowInfoUnsupportedCCU()
        {
            if (CgpClient.Singleton.IsConnectionLost(true))
                return;

            try
            {
                var currentVersion =
                    Plugin.MainServerProvider.CCUs.GetFirmwareVersion(_editingObject.IdCCU).Split(' ')[0];
                var currnetCeVersion = int.Parse(Plugin.MainServerProvider.CCUs.WinCEImageVersion(_editingObject.IdCCU));


                string minimalSupportedCCUVersion;
                string maximalSupportedCCUVersion;
                string minimalSupportedCeVersion;
                string minimalCCUforCeChecking;

                Plugin.MainServerProvider.GetCCUSupportedVersions(
                    out minimalSupportedCCUVersion,
                    out maximalSupportedCCUVersion,
                    out minimalCCUforCeChecking,
                    out minimalSupportedCeVersion);

                var min = new Version(minimalSupportedCCUVersion);
                var max = new Version(maximalSupportedCCUVersion);
                int minCe = int.Parse(minimalSupportedCeVersion);
                var current = new Version(currentVersion);
                var ceCheckingTreshold = new Version(minimalCCUforCeChecking);

                if (current < min)
                    Dialog.Info(GetString("InfoNotSupportedCCUVersionTooOld") + Environment.NewLine
                                + GetString("MinimalCCUSupportedVersion") + ": " + minimalSupportedCCUVersion +
                                Environment.NewLine
                                + GetString("CurrentCCUVersion") + ":" + currentVersion);
                else if (current > max)
                    Dialog.Info(GetString("InfoNotSupportedCCUVersionTooNew") + Environment.NewLine
                                + GetString("MaximalCCUSupportedVersion") + ": " + maximalSupportedCCUVersion +
                                Environment.NewLine
                                + GetString("CurrentCCUVersion") + ":" + currentVersion + Environment.NewLine);
                else if (current >= ceCheckingTreshold
                         && currnetCeVersion < minCe)
                    Dialog.Info(GetString("InfoNotSupportedCeVersionTooOld") + Environment.NewLine
                                + GetString("MinimalCeSupportedVersion") + ": " + minCe + Environment.NewLine
                                + GetString("CurrentCeVersion") + ":" + currnetCeVersion + Environment.NewLine);
            }
            catch
            {
            }
        }

        private void GetInformationFromServer()
        {
            try
            {
                RefreshCCUState(Plugin.MainServerProvider.CCUs.GetCCUState(_editingObject.IdCCU));
                var configured = Plugin.MainServerProvider.CCUs.GetCCUConfiguredState(_editingObject.IdCCU);
                var isCCU0 = Plugin.MainServerProvider.CCUs.IsCCU0(_editingObject.IdCCU);
                RefreshCCUConfigured(configured, isCCU0);
                //bool isCCUUpgrader = (this.Plugin as NCASClient).MainServerProvider.CCUs.IsCCUUpgrader(_editingObject.IdCCU);
                //SetEditFormPurspose(isCCUUpgrader);
                RefreshIPSettings(Plugin.MainServerProvider.CCUs.GetIpSettings(_editingObject.IdCCU));
                SafeThread.StartThread(ObtainCurrentCcuTime);
            }
            catch
            {
            }
        }

        private void ObtainCurrentCcuTime()
        {
            object dateTime = Plugin.MainServerProvider.CCUs.GetCurrentCCUTime(_editingObject.IdCCU);

            ShowCcuTime(dateTime == null
                ? string.Empty
                : ((DateTime) dateTime).ToString(CultureInfo.InvariantCulture));
        }

        private void ShowCcuTime(string timeText)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<string>(ShowCcuTime), timeText);
            }
            else
            {
                _eTime.Text = timeText;
            }
        }

        #region Server events

        private void RegisterServerEvents()
        {
            _eventCCUStateChanged = ChangeCCUState;
            StateChangedCCUHandler.Singleton.RegisterStateChanged(_eventCCUStateChanged);
            _eventCCUConfiguredChanged = ChangeCCUConfigured;
            ConfiguredChangedCCUHandler.Singleton.RegisterConfiguredChanged(_eventCCUConfiguredChanged);
            _eventCCUMACAddressChanged = ChangeCCUMACAddress;
            MACAddressChangedCCUHandler.Singleton.RegisterMACAddressChanged(_eventCCUMACAddressChanged);

            var isCCU0 = Plugin.MainServerProvider.CCUs.IsCCU0(_editingObject.IdCCU);
            if (!isCCU0)
            {
                _eventInputStateChanged = ChangeInputState;
                StateChangedInputHandler.Singleton.RegisterStateChanged(_eventInputStateChanged);
                _eventOutputStateChanged = ChangeOutputState;
                StateChangedOutputHandler.Singleton.RegisterStateChanged(_eventOutputStateChanged);
                _eventOutputRealStateChanged = ChangeOutputRealState;
                RealStateChangedOutputHandler.Singleton.RegisterStateChanged(_eventOutputRealStateChanged);
                _eventCardReaderOnlineStateChanged = ChangeCardReaderOnlineState;
                StateChangedCardReaderHandler.Singleton.RegisterStateChanged(_eventCardReaderOnlineStateChanged);
                _eventCRsUpgradeProgressChanged = ChangeCRsUpgradeProgress;
                CRUpgradeProgressChangedHandler.Singleton.RegisterUpgradeProgressChanged(_eventCRsUpgradeProgressChanged);
            }
            else
            {
                _eventCardReaderOnlineStateChanged = ChangeCardReaderOnlineStateToUpgrade;
                StateChangedCardReaderHandler.Singleton.RegisterStateChanged(_eventCardReaderOnlineStateChanged);
                _eventCRsUpgradeProgressChanged = ChangeCRsUpgradeProgress;
                CRUpgradeProgressChangedHandler.Singleton.RegisterUpgradeProgressChanged(_eventCRsUpgradeProgressChanged);
                _eventDCUsUpgradeProgressChanged = ChangeDCUsUpgradeProgress;
                DCUUpgradeProgressChangedHandler.Singleton.RegisterUpgradeProgressChanged(
                    _eventDCUsUpgradeProgressChanged);
            }
            _eventDCUOnlineStateChanged = ChangeDCUOnlineState;
            DCUOnlineStateChangedHandler.Singleton.RegisterStateChanged(_eventDCUOnlineStateChanged);
            _eventIpSettingsChanged = ChangeIPSettings;
            IPSettingsChangedHandler.Singleton.RegisterIPSettingsChanged(_eventIpSettingsChanged);
            _eventFileTransferProgressChanged = ChangeTransferPercents;
            TFTPFileTransferProgressChangedHandler.Singleton.RegisterTFTPFileTransferProgressChanged(
                _eventFileTransferProgressChanged);
            _eventCEUpgradeProgressChanged = ChangeCeUpgradeProgress;
            CEUpgradeProgressChangedHandler.Singleton.RegisterCEUpgradeProgressChanged(_eventCEUpgradeProgressChanged);
            _eventCCUUpgradeProgressChanged = ChangeCCUUpgradeProgress;
            CCUUpgradeProgressChangedHandler.Singleton.RegisterCCUUpgradeProgressChanged(_eventCCUUpgradeProgressChanged);
            _eventCCUMakeLogDumpProgressChanged = ChangeCCUMakeLogDumpProgress;
            CCUMakeLogDumpProgressChangedHandler.Singleton.RegisterCCUMakeLogDumpProgressChanged(_eventCCUMakeLogDumpProgressChanged);
            _eventCCUUpgradeFinished = ChangeCCUUpgradeFinished;
            CCUUpgradeFinishedHandler.Singleton.RegisterCCUUpgradeFinished(_eventCCUUpgradeFinished);
            _cudObjectEvent = CudObjectEvent;
            CUDObjectHandler.Singleton.Register(_cudObjectEvent, ObjectType.CCU, ObjectType.DCU, ObjectType.CardReader,
                ObjectType.Input, ObjectType.Output, ObjectType.DoorEnvironment);

            _eventCcuUpsMonitorValuesChanged += UpsMonitorValuesChanged;
            CCUUpsMonitorValuesChangedHandler.Singleton.RegisterUpsMonitorValuesChanged(_eventCcuUpsMonitorValuesChanged);
            _eventCcuUpsMonitorOnlineStateChanged += UpsMonitorOnlineStateChanged;
            CCUUpsMonitorOnlineStateChangedHandler.Singleton.RegisterUpsMonitorOnlineStateChanged(
                _eventCcuUpsMonitorOnlineStateChanged);
            _eventCcuUpsMonitorAlarmStateChanged += UpsMonitorAlarmStateChanged;
            CCUUpsMonitorAlarmStateChangedHandler.Singleton.RegisterUpsMonitorAlarmStateChanged(
                _eventCcuUpsMonitorAlarmStateChanged);
            _eventCCUKillFailed += CCUKillFailed;
            CCUKillFailedHandler.Singleton.RegisterCCUKillFailed(_eventCCUKillFailed);
        }

        private void UnregisterServerEvents()
        {
            if (_eventCCUStateChanged != null)
                StateChangedCCUHandler.Singleton.UnregisterStateChanged(_eventCCUStateChanged);
            if (_eventCCUConfiguredChanged != null)
                ConfiguredChangedCCUHandler.Singleton.UnregisterConfiguredChanged(_eventCCUConfiguredChanged);
            if (_eventCCUMACAddressChanged != null)
                MACAddressChangedCCUHandler.Singleton.UnregisterLogicalAddressChanged(_eventCCUMACAddressChanged);
            if (_eventInputStateChanged != null)
                StateChangedInputHandler.Singleton.UnregisterStateChanged(_eventInputStateChanged);
            if (_eventCardReaderOnlineStateChanged != null)
                StateChangedCardReaderHandler.Singleton.UnregisterStateChanged(_eventCardReaderOnlineStateChanged);
            if (_eventCRsUpgradeProgressChanged != null)
                CRUpgradeProgressChangedHandler.Singleton.UnregisterUpgradeProgressChanged(
                    _eventCRsUpgradeProgressChanged);
            if (_eventDCUOnlineStateChanged != null)
                DCUOnlineStateChangedHandler.Singleton.UnregisterStateChanged(_eventDCUOnlineStateChanged);
            if (_eventIpSettingsChanged != null)
                IPSettingsChangedHandler.Singleton.UnregisterIPSettingsChanged(_eventIpSettingsChanged);
            if (_eventDCUsUpgradeProgressChanged != null)
                DCUUpgradeProgressChangedHandler.Singleton.UnregisterUpgradeProgressChanged(
                    _eventDCUsUpgradeProgressChanged);
            if (_eventFileTransferProgressChanged != null)
                TFTPFileTransferProgressChangedHandler.Singleton.UnregisterTFTPFileTransferProgressChanged(
                    _eventFileTransferProgressChanged);
            if (_eventCEUpgradeProgressChanged != null)
                CEUpgradeProgressChangedHandler.Singleton.UnregisterCEUpgradeProgressChanged(
                    _eventCEUpgradeProgressChanged);
            if (_eventCCUUpgradeProgressChanged != null)
                CCUUpgradeProgressChangedHandler.Singleton.UnregisterCCUUpgradeProgressChanged(
                    _eventCCUUpgradeProgressChanged);
            if (_eventCCUUpgradeFinished != null)
                CCUUpgradeFinishedHandler.Singleton.UnregisterCCUUpgradeFinished(_eventCCUUpgradeFinished);
            if (_cudObjectEvent != null)
                CUDObjectHandler.Singleton.Unregister(_cudObjectEvent, ObjectType.DCU, ObjectType.CardReader,
                    ObjectType.Input, ObjectType.Output, ObjectType.DoorEnvironment);
            if (_eventCcuUpsMonitorValuesChanged != null)
                CCUUpsMonitorValuesChangedHandler.Singleton.UnregisterUpsMonitorValuesChanged(
                    _eventCcuUpsMonitorValuesChanged);
            if (_eventCcuUpsMonitorOnlineStateChanged != null)
                CCUUpsMonitorOnlineStateChangedHandler.Singleton.UnregisterUpsMonitorOnlineStateChanged(
                    _eventCcuUpsMonitorOnlineStateChanged);
            if (_eventCcuUpsMonitorAlarmStateChanged != null)
                CCUUpsMonitorAlarmStateChangedHandler.Singleton.UnregisterUpsMonitorAlarmStateChanged(
                    _eventCcuUpsMonitorAlarmStateChanged);
            if (_editingObject != null && _editingObject.IdCCU != Guid.Empty)
            {
                SafeThread.StartThread(StopUpsMonitorStatistic);
            }
            if (_eventCCUKillFailed != null)
                CCUKillFailedHandler.Singleton.UnregisterCCUKillFailed(_eventCCUKillFailed);

            if (_eventCCUMakeLogDumpProgressChanged != null)
                CCUMakeLogDumpProgressChangedHandler.Singleton.UnregisterCCUMakeLogDumpProgressChanged(
                    _eventCCUMakeLogDumpProgressChanged);
        }

        private void ChangeTransferPercents(IPAddress destinationIP, int percents, byte transferPurpose)
        {
            if (destinationIP == null)
                return;

            if (!destinationIP.ToString().Equals(_editingObject.IPAddress))
                return;

            if (InvokeRequired)
                Invoke(new Action<IPAddress, int, byte>(ChangeTransferPercents), destinationIP, percents,
                    transferPurpose);
            else
            {
                var fileTransferPurpose = (CCUFileTransferPurpose) transferPurpose;
                switch (fileTransferPurpose)
                {
                    case CCUFileTransferPurpose.CCUUpgrade:
                        if (percents >= 0)
                        {
                            //_eTransferProgress.Text = percents.ToString(CultureInfo.InvariantCulture);
                            _prbTransfer.Value = percents;
                        }
                        else
                            ShowFailedMessage();
                        break;
                    case CCUFileTransferPurpose.DCUUpgrade:
                        if (percents < 0)
                            ShowDcuUpgradeTransferFailedMEssage();
                        break;
                    case CCUFileTransferPurpose.CEUpgrade:
                        if (percents >= 0)
                        {
                            //_tbCeFileTransfer.Text = percents.ToString(CultureInfo.InvariantCulture);
                            _prbTransferCe.Value = percents;
                        }
                        else
                            ShowCEFailedMessage();
                        break;
                }
            }
        }

        private void ShowDcuUpgradeTransferFailedMEssage()
        {
            if (InvokeRequired)
                BeginInvoke(new DVoid2Void(ShowDcuUpgradeTransferFailedMEssage));
            else
            {
                ControlNotification.Singleton.Error(NotificationPriority.JustOne, _dgvDCUUpgrading,
                    _editingObject.Name + ": " + GetString("FailedToTransferUpgradeFile"),
                    new ControlNotificationSettings(NotificationParts.Hint, HintPosition.LeftTop));
            }
        }

        private void ChangeCeUpgradeProgress(IPAddress ccuIPAddress, string state)
        {
            if (ccuIPAddress == null)
                return;

            if (!ccuIPAddress.ToString().Equals(_editingObject.IPAddress))
                return;

            if (InvokeRequired)
                Invoke(new Action<IPAddress, string>(ChangeCeUpgradeProgress), ccuIPAddress, state);
            else
            {
                var enumState = (ActionResultUpgrade) Enum.Parse(typeof (ActionResultUpgrade), state);
                SetCEUpgradeStage((int) enumState);
            }
        }

        private void ChangeCCUUpgradeProgress(IPAddress ccuIPAddress, int percent)
        {
            if (ccuIPAddress == null)
                return;

            if (!ccuIPAddress.ToString().Equals(_editingObject.IPAddress))
                return;

            if (InvokeRequired)
                Invoke(new Action<IPAddress, int>(ChangeCCUUpgradeProgress), ccuIPAddress, percent);
            else
            {
                SetUpgradeProgress(percent);
            }
        }

        private void ChangeCCUMakeLogDumpProgress(Guid idCcu, int percent)
        {
            if (!idCcu.Equals(_editingObject.IdCCU))
                return;

            if (InvokeRequired)
                Invoke(new Action<Guid, int>(ChangeCCUMakeLogDumpProgress), idCcu, percent);
            else
            {
                SetMakeLogDumpProgress(percent);
            }
        }

        private void ChangeCCUUpgradeFinished(IPAddress ccuIPAddress, bool success)
        {
            if (ccuIPAddress == null)
                return;

            if (!ccuIPAddress.ToString().Equals(_editingObject.IPAddress))
                return;

            if (InvokeRequired)
                Invoke(new Action<IPAddress, bool>(ChangeCCUUpgradeFinished), ccuIPAddress, success);
            else
            {
                _eUpgradeFinalisation.Text = GetString(success
                    ? "General_Succeeded"
                    : "General_Failed");

                _prbTransfer.Value = 0;
                _prbTransferCe.Value = 0;
                _prbUnpack.Value = 0;
            }
        }

        private void ChangeCCUState(Guid ccuGuid, byte state)
        {
            if (ccuGuid == _editingObject.IdCCU)
            {
                if (InvokeRequired)
                {
                    BeginInvoke(new Action<Guid, byte>(ChangeCCUState), ccuGuid, state);
                }
                else
                {

                    RefreshCCUState((CCUOnlineState) state);

                    if ((CCUOnlineState) state != CCUOnlineState.Online)
                    {
                        ResetUpsMonitor();
                        RefReshUpsOnlineState((byte) TUpsOnlineState.Unknown);

                        _bCrMarkAsDead.Text = GetString("General_bRemove");
                        _bDcuMarkAsDead.Text = GetString("General_bRemove");
                    }
                    else if (_editingObject.IsConfigured)
                    {
                        LocalizationHelper.TranslateControl(_bCrMarkAsDead);
                        LocalizationHelper.TranslateControl(_bDcuMarkAsDead);
                    }
                }
            }
        }

        private void ChangeCCUConfigured(Guid ccuGuid, byte configured, bool isCCU0, bool blockedByLicence)
        {
            if (ccuGuid == _editingObject.IdCCU)
            {
                var ccuConfigurationState = (CCUConfigurationState) configured;

                RefreshCCUConfigured(ccuConfigurationState, isCCU0);
                ChangeEnabledState(_bPrecreateCardReader);

                if (ccuConfigurationState != CCUConfigurationState.Unknown
                    && ccuConfigurationState != CCUConfigurationState.Upgrading
                    && ccuConfigurationState != CCUConfigurationState.ForceReconfiguration)
                {
                    SafeThread.StartThread(ObtainCcuFirmwareVersion);
                }
            }
        }

        private void ChangeEnabledState(Button button)
        {

            if (InvokeRequired)
            {
                BeginInvoke(new Action<Button>(ChangeEnabledState), button);
            }
            else
            {
                if (IsConfigured())
                {
                    button.Enabled = true;
                    button.Visible = true;
                }
                else
                {
                    button.Enabled = false;
                    button.Visible = true;
                }
            }
        }

        private void ChangeCCUMACAddress(Guid ccuGuid, string macAddress)
        {
            if (ccuGuid == _editingObject.IdCCU)
            {
                RefreshCcuMacAddpress(macAddress);
            }
        }

        private void RefreshCcuMacAddpress(string macAddress)
        {
            if (_lMACCCUInformation.InvokeRequired)
            {
                Invoke(new Action<string>(RefreshCcuMacAddpress), macAddress);
            }
            else
            {
                _lMACCCUInformation.Text = macAddress;
            }
        }

        private void ChangeInputState(Guid inputGuid, byte state, Guid parent)
        {
            if (_editingObject.IdCCU == parent)
                SafeThread<Guid, byte>.StartThread(DoChangeInputState, inputGuid, state);
        }

        private readonly Mutex _doChangeInputStateMutex = new Mutex();

        private void DoChangeInputState(Guid inputGuid, byte state)
        {

            if (InvokeRequired)
            {
                BeginInvoke(new Action<Guid, byte>(DoChangeInputState), inputGuid, state);
            }
            else
            {
                try
                {
                    _doChangeInputStateMutex.WaitOne();
                    RefreshInputState(inputGuid, (InputState) state);
                    _doChangeInputStateMutex.ReleaseMutex();
                }
                catch
                {
                }
            }
        }

        private void RefreshInputState(Guid inputGuid, InputState inputState)
        {
            foreach (DataGridViewRow row in _dgvInputCCU.Rows)
            {
                var input = (Input) _bindingSourceInput.List[row.Index];

                if (input != null)
                {
                    if (input.IdInput == inputGuid)
                    {
                        if (inputState == InputState.Alarm)
                        {
                            row.Cells[Input.COLUMNSTATUSONOFF].Value = GetString("InputAlarm");
                        }
                        else if (inputState == InputState.Short)
                        {
                            row.Cells[Input.COLUMNSTATUSONOFF].Value = GetString("InputShort");
                        }
                        else if (inputState == InputState.Break)
                        {
                            row.Cells[Input.COLUMNSTATUSONOFF].Value = GetString("InputBreak");
                        }
                        else if (inputState == InputState.Normal)
                        {
                            row.Cells[Input.COLUMNSTATUSONOFF].Value = GetString("InputNormal");
                        }
                        else if (inputState == InputState.UsedByAnotherAplication)
                        {
                            row.Cells[Input.COLUMNSTATUSONOFF].Value = GetString("UsedByAnotherAplication");
                        }
                        else if (inputState == InputState.OutOfRange)
                        {
                            row.Cells[Input.COLUMNSTATUSONOFF].Value = GetString("OutOfRange");
                        }
                        else
                            row.Cells[Input.COLUMNSTATUSONOFF].Value = GetString("Unknown");
                    }
                }
            }
        }

        private void ChangeOutputState(Guid outputGuid, byte state, Guid parent)
        {
            if (_editingObject.IdCCU == parent)
                SafeThread<Guid, byte>.StartThread(DoChangeOutputState, outputGuid, state);
        }

        private readonly Mutex _doChangeOutputStateMutex = new Mutex();

        private void DoChangeOutputState(Guid outputGuid, byte state)
        {

            if (InvokeRequired)
            {
                BeginInvoke(new Action<Guid, byte>(DoChangeOutputState), outputGuid, state);
            }
            else
            {
                try
                {
                    _doChangeOutputStateMutex.WaitOne();
                    RefreshOutputState(outputGuid, (OutputState) state);
                    _doChangeOutputStateMutex.ReleaseMutex();
                }
                catch
                {
                }
            }
        }

        private void RefreshOutputState(Guid outputGuid, OutputState outputState)
        {
            foreach (DataGridViewRow row in _dgvOutputCCU.Rows)
            {
                var output = (Output) _bindingSourceOutput.List[row.Index];

                if (output != null)
                {
                    if (output.IdOutput == outputGuid)
                    {
                        if (outputState == OutputState.Off)
                        {
                            row.Cells[Output.COLUMNSTATUSONOFF].Value = GetString("Off");
                        }
                        else if (outputState == OutputState.On)
                        {
                            row.Cells[Output.COLUMNSTATUSONOFF].Value = GetString("On");
                        }
                        else if (outputState == OutputState.UsedByAnotherAplication)
                        {
                            row.Cells[Output.COLUMNSTATUSONOFF].Value = GetString("UsedByAnotherAplication");
                        }
                        else if (outputState == OutputState.OutOfRange)
                        {
                            row.Cells[Output.COLUMNSTATUSONOFF].Value = GetString("OutOfRange");
                        }
                        else
                            row.Cells[Output.COLUMNSTATUSONOFF].Value = GetString("Unknown");
                    }
                }
            }
        }

        private void ChangeOutputRealState(Guid outputGuid, byte state, Guid parent)
        {
            if (_editingObject.IdCCU == parent)
                SafeThread<Guid, byte>.StartThread(DoChangeOutputRealState, outputGuid, state);
        }

        private void DoChangeOutputRealState(Guid outputGuid, byte state)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<Guid, byte>(DoChangeOutputRealState), outputGuid, state);
            }
            else
            {
                try
                {
                    _doChangeOutputStateMutex.WaitOne();
                    RefreshOutputRealState(outputGuid, (OutputState) state);
                    _doChangeOutputStateMutex.ReleaseMutex();
                }
                catch
                {
                }
            }
        }

        private void RefreshOutputRealState(Guid outputGuid, OutputState outputState)
        {
            foreach (DataGridViewRow row in _dgvOutputCCU.Rows)
            {
                var output = (Output) _bindingSourceOutput.List[row.Index];

                if (output != null)
                {
                    if (output.IdOutput == outputGuid)
                    {
                        if (!output.RealStateChanges)
                        {
                            row.Cells[Output.COLUMNREALSTATUSONOFF].Value = GetString("ReportingSupressed");
                        }
                        else
                        {
                            if (outputState == OutputState.Off)
                            {
                                row.Cells[Output.COLUMNREALSTATUSONOFF].Value = GetString("Off");
                            }
                            else if (outputState == OutputState.On)
                            {
                                row.Cells[Output.COLUMNREALSTATUSONOFF].Value = GetString("On");
                            }
                            else
                                row.Cells[Output.COLUMNREALSTATUSONOFF].Value = GetString("ReportingSupressed");
                        }
                    }
                }
            }
        }

        private void ChangeCardReaderOnlineState(Guid crGuid, byte state, Guid parent)
        {
            if (parent == _editingObject.IdCCU)
            {
                SafeThread<Guid, byte>.StartThread(DoChangeCardReaderOnlineState, crGuid, state);
            }
        }

        private void DoChangeCardReaderOnlineState(Guid crGuid, byte state)
        {
            RefreshCardReaderOnlineState(crGuid, (OnlineState) state);
            _dgCardReaders_SelectionChanged(null, null);
            RefreshCardReaderOnlineStateToUpgrade(crGuid, (OnlineState) state);
        }

        private void ChangeCardReaderOnlineStateToUpgrade(Guid crGuid, byte state, Guid parent)
        {
            var valid = _editingObject.DCUs
                .Any(dcu => dcu.IdDCU.Equals(parent));

            if (!valid)
                return;

            SafeThread<Guid, OnlineState>.StartThread(
                RefreshCardReaderOnlineStateToUpgrade, crGuid, (OnlineState) state);
        }

        private void RefreshCardReaderOnlineState(Guid crGuid, OnlineState onlineState)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<Guid, OnlineState>(RefreshCardReaderOnlineState), crGuid, onlineState);
            }
            else
            {
                var wasFound = false;

                var images = ((ICgpVisualPlugin)NCASClient.Singleton).GetPluginObjectsImages();

                foreach (DataGridViewRow row in _dgCardReaders.Rows)
                {
                    var cardReader = (CardReader) _bindingSourceCardReaders.List[row.Index];

                    if (cardReader != null)
                    {
                        if (cardReader.IdCardReader == crGuid)
                        {
                            wasFound = true;
                            row.Cells[CardReader.COLUMNONLINESTATE].Value = GetString(onlineState.ToString());

                            row.Cells[0].Value =
                                images.Images[onlineState == OnlineState.Online
                                    ? ObjectType.CardReader.ToString()
                                    : ObjTypeHelper.CardReaderBlocked];
                        }
                    }
                }

                if (!wasFound)
                {
                    var cr = Plugin.MainServerProvider.CardReaders.GetObjectById(crGuid);
                    if (cr.CCU != null && cr.CCU.IdCCU == _editingObject.IdCCU)
                    {
                        var ccu = Plugin.MainServerProvider.CCUs.GetObjectById(_editingObject.IdCCU);
                        _editingObject.CardReaders = ccu.CardReaders;
                        ShowCardReaders();
                    }
                }
            }
        }

        private void RefreshCardReaderOnlineStateToUpgrade(Guid crGuid, OnlineState onlineState)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<Guid, OnlineState>(RefreshCardReaderOnlineStateToUpgrade), crGuid, onlineState);
            }
            else
            {
                var wasFound = false;
                foreach (DataGridViewRow row in _dgvCRUpgrading.Rows)
                {
                    var cardReader = _bindingSourceCRUpgrades.List[row.Index] as CardReader;
                    if (cardReader != null)
                    {
                        if (cardReader.IdCardReader == crGuid)
                        {
                            wasFound = true;
                            ShowGridCRUpgrade();
                            //row.Cells[CardReader.COLUMNONLINESTATE].Value = GetString(onlineState.ToString());
                        }
                    }
                }

                if (!wasFound)
                {
                    ShowCRsUpgrade();
                }
            }
        }

        private void ChangeCRsUpgradeProgress(CRUpgradeState upgradeState)
        {
            if (upgradeState.DCUGuid != null)
            {
                var hasSpecificDCU = _editingObject.DCUs
                    .Any(dcu => dcu.IdDCU.Equals(upgradeState.DCUGuid));

                if (!hasSpecificDCU)
                    return;
            }
            else
            {
                if (upgradeState.CCUGuid != _editingObject.IdCCU)
                {
                    return;
                }
            }

            DoChangeCRsUpgradeProgress(upgradeState, null);
        }

        private void ChangeDCUsUpgradeProgress(DCUUpgradeState upgradeState)
        {
            if (upgradeState != null && upgradeState.CCUGuid.Equals(_editingObject.IdCCU))
                _progressDCUUpgradeChanged.Enqueue(upgradeState);
        }

        private void DoChangeDCUsUpgradeProgress(DCUUpgradeState upgradeState)
        {
            DoChangeDCUsUpgradeProgress(upgradeState, null);
        }

        private readonly Dictionary<byte, int> _lastDCUPercents = new Dictionary<byte, int>();

        private void DoChangeDCUsUpgradeProgress(DCUUpgradeState upgradeState, Exception ex)
        {
            if (upgradeState == null)
                return;

            if (InvokeRequired)
            {
                Invoke(new Action<DCUUpgradeState, Exception>(DoChangeDCUsUpgradeProgress), upgradeState, ex);
            }
            else
            {
                //handling two situations, when all DCUs falied to upgrade
                if (ex != null)
                {
                    ControlNotification.Singleton.Error(NotificationPriority.Last, _dgvDCUUpgrading,
                        GetString("AllDevicesUpgradeFailed") + ": " + ex.Message,
                        new ControlNotificationSettings(NotificationParts.Hint, HintPosition.LeftTop));
                    return;
                }

                if (upgradeState.UnpackErrorCode != null)
                {
                    var unpackFailedCode = (UnpackPackageFailedCode) upgradeState.UnpackErrorCode;
                    ControlNotification.Singleton.Error(NotificationPriority.Last, _dgvDCUUpgrading,
                        GetString("AllDevicesUpgradeFailed") + ": " +
                        GetString("UnpackUpgradePackageFailed" + unpackFailedCode),
                        new ControlNotificationSettings(NotificationParts.Hint, HintPosition.LeftTop));
                    return;
                }

                foreach (DataGridViewRow row in _dgvDCUUpgrading.Rows)
                {
                    if (row.Cells[DCU.COLUMNLOGICALADDRESS].Value != null &&
                        upgradeState.DCUsLogicalAddresses.Contains((byte) row.Cells[DCU.COLUMNLOGICALADDRESS].Value))
                    {
                        //these DCUs are online, so they are upgrading
                        if (upgradeState.OnlineDCUs.Contains((byte) row.Cells[DCU.COLUMNLOGICALADDRESS].Value))
                        {
                            var lastPercents = 0;
                            try
                            {
                                if (upgradeState.Percents != null)
                                {
                                    if (
                                        !_lastDCUPercents.TryGetValue((byte) row.Cells[DCU.COLUMNLOGICALADDRESS].Value,
                                            out lastPercents))
                                        lastPercents = 0;
                                }
                                else
                                {
                                    if (_lastDCUPercents.ContainsKey((byte) row.Cells[DCU.COLUMNLOGICALADDRESS].Value))
                                        _lastDCUPercents.Remove((byte) row.Cells[DCU.COLUMNLOGICALADDRESS].Value);
                                }
                            }
                            catch
                            {
                            }

                            if (upgradeState.Percents < 0)
                            {
                                ControlNotification.Singleton.Error(NotificationPriority.Last, _dgvDCUUpgrading,
                                    row.Cells[DCU.COLUMNFULLNAME].Value + ": " + GetString("DeviceUpgradeFailed"),
                                    new ControlNotificationSettings(NotificationParts.Hint, HintPosition.LeftTop));
                                row.Cells[DCU.COLUMNUPGRADEPROGRESS].Value = 0;
                                continue;
                            }
                            if (upgradeState.Percents == 100)
                            {
                                ControlNotification.Singleton.Info(NotificationPriority.Last, _dgvDCUUpgrading,
                                    row.Cells[DCU.COLUMNFULLNAME].Value + ": " + GetString("DeviceUpgradeSucceeded"),
                                    new ControlNotificationSettings(NotificationParts.Hint, HintPosition.LeftTop));
                                row.Cells[DCU.COLUMNUPGRADEPROGRESS].Value = 0;
                                row.Cells[DCU.COLUMNSELECTUPGRADE].Value = false;
                                continue;
                            }
                            if (upgradeState.Percents == null || upgradeState.Percents == 0 ||
                                upgradeState.Percents > lastPercents)
                            {
                                try
                                {
                                    if (upgradeState.Percents != null)
                                        _lastDCUPercents[(byte) row.Cells[DCU.COLUMNLOGICALADDRESS].Value] =
                                            upgradeState.Percents.Value;
                                }
                                catch
                                {
                                }

                                row.Cells[DCU.COLUMNUPGRADEPROGRESS].Value = upgradeState.Percents;
                            }
                        }
                            //these DCUs are not online (they were offline when upgrade process was starting)
                        else
                        {
                            ControlNotification.Singleton.Error(NotificationPriority.Last, _dgvDCUUpgrading,
                                row.Cells[DCU.COLUMNFULLNAME].Value + ": " + GetString("UpgradeErrorDeviceOffline"),
                                new ControlNotificationSettings(NotificationParts.Hint, HintPosition.LeftTop));
                        }
                    }
                }
            }
        }

        private void ChangeDCUOnlineState(Guid dcuGuid, OnlineState state, Guid parentGuid)
        {
            if (parentGuid == _editingObject.IdCCU)
            {
                SafeThread<Guid, OnlineState>.StartThread(DoChangeDCUOnlineState, dcuGuid, state);
            }
        }

        private void DoChangeDCUOnlineState(Guid dcuGuid, OnlineState state)
        {
            try
            {
                RefreshDCUOnlineState(dcuGuid, state);
                _dgDCUs_SelectionChanged(null, null);
                SetDCUsUpgradeDgValues();
            }
            catch
            {
            }
        }

        private void RefreshDCUOnlineState(Guid dcuGuid, OnlineState onlineState)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<Guid, OnlineState>(RefreshDCUOnlineState), dcuGuid, onlineState);
            }
            else
            {
                var wasFound = false;

                var images = ((ICgpVisualPlugin)NCASClient.Singleton).GetPluginObjectsImages();

                foreach (DataGridViewRow row in _dgDCUs.Rows)
                {
                    var dcu = (DCU) _bindingSourceDCUs.List[row.Index];

                    if (dcu != null)
                    {
                        if (dcu.IdDCU == dcuGuid)
                        {
                            wasFound = true;
                            row.Cells[DCU.COLUMNONLINESTATE].Value = GetString(onlineState.ToString());

                            row.Cells[0].Value =
                                images.Images[onlineState == OnlineState.Online
                                    ? ObjectType.DCU.ToString()
                                    : ObjTypeHelper.DCUOffline];
                        }
                    }
                }

                if (!wasFound)
                {
                    var ccu = Plugin.MainServerProvider.CCUs.GetObjectById(_editingObject.IdCCU);
                    _editingObject.DCUs = ccu.DCUs;
                    ShowDCUs();
                }

                wasFound = false;
                foreach (DataGridViewRow row in _dgvDCUUpgrading.Rows)
                {
                    var dcu = (DCU) _bindingSourceDCUUpgrades.List[row.Index];

                    if (dcu != null)
                    {
                        if (dcu.IdDCU == dcuGuid)
                        {
                            wasFound = true;
                            row.Cells[DCU.COLUMNONLINESTATE].Value = GetString(onlineState.ToString());

                            row.Cells[0].Value =
                                images.Images[onlineState == OnlineState.Online
                                    ? ObjectType.DCU.ToString()
                                    : ObjTypeHelper.DCUOffline];
                        }
                    }
                }

                if (!wasFound)
                {
                    var ccu = Plugin.MainServerProvider.CCUs.GetObjectById(_editingObject.IdCCU);
                    _editingObject.DCUs = ccu.DCUs;
                    ShowDCUsUpgrade();
                }
            }
        }

        private void ChangeIPSettings(Guid ccuGuid, IPSetting ipSetting)
        {
            if (ccuGuid == _editingObject.IdCCU)
                RefreshIPSettings(ipSetting);
        }

        private readonly object _cardReaderReloadLock = new object();
        private bool _cardReaderReload;
        private readonly object _dcuReloadLock = new object();
        private bool _dcuReload;
        private readonly object _inputReloadLock = new object();
        private bool _inputReload;
        private readonly object _outputReloadLock = new object();
        private bool _outputReload;
        private readonly object _doorEnvironmentReloadLock = new object();
        private bool _doorEnvironmentReload;

        private void CudObjectEvent(ObjectType objectType, object id, bool isInsert)
        {
            try
            {
                if (!(id is Guid))
                    return;

                var idGuid = (Guid) id;

                switch (objectType)
                {
                    case ObjectType.CCU:

                        if (idGuid != _editingObject.IdCCU)
                            return;

                        var newMbt = Plugin.MainServerProvider.CCUs.GetCCUMainBoardType(_editingObject.IdCCU);

                        if (newMbt != MainBoardVariant.Unknown
                            && newMbt != GetMainBoardVariant(_editingObject.CcuMainboardType))
                        {
                            string messageInfo = string.Format(GetString("InfoCcuIncorrectMbt"),
                                GetMainBoardVariant(_editingObject.CcuMainboardType), newMbt);

                            _editingObject.CcuMainboardType = (byte) newMbt;

                            Invoke((MethodInvoker) delegate
                            {
                                Dialog.Info(messageInfo);
                                Close();
                                NCASCCUsForm.Singleton.OpenEditForm(_editingObject);
                            });
                        }

                        break;

                    case ObjectType.DCU:
                        lock (_dcuReloadLock)
                        {
                            var ownDCU = false;
                            if (_editingObject.DCUs != null && _editingObject.DCUs.Count > 0)
                            {
                                if (_editingObject.DCUs
                                    .Any(dcu => dcu.IdDCU == idGuid))
                                {
                                    ownDCU = true;
                                }
                            }

                            if (ownDCU)
                            {
                                var ccu = Plugin.MainServerProvider.CCUs.GetObjectById(_editingObject.IdCCU);
                                if (ccu != null)
                                {
                                    _editingObject.DCUs = ccu.DCUs;
                                    _dcuReload = true;
                                }
                            }
                        }
                        break;
                    case ObjectType.CardReader:
                        lock (_cardReaderReloadLock)
                        {
                            var ownCardReader = false;
                            if (_editingObject.CardReaders != null && _editingObject.CardReaders.Count > 0)
                            {
                                if (_editingObject.CardReaders
                                    .Any(cardReader => cardReader.IdCardReader == idGuid))
                                {
                                    ownCardReader = true;
                                }
                            }

                            if (ownCardReader)
                            {
                                var ccu = Plugin.MainServerProvider.CCUs.GetObjectById(_editingObject.IdCCU);
                                if (ccu != null)
                                {
                                    _editingObject.CardReaders = ccu.CardReaders;
                                    _cardReaderReload = true;
                                }
                            }
                        }
                        break;
                    case ObjectType.Input:
                        lock (_inputReloadLock)
                        {
                            var owInput = false;
                            if (_editingObject.Inputs != null && _editingObject.Inputs.Count > 0)
                            {
                                if (_editingObject.Inputs
                                    .Any(input => input.IdInput == idGuid))
                                {
                                    owInput = true;
                                }
                            }

                            if (owInput)
                            {
                                var ccu = Plugin.MainServerProvider.CCUs.GetObjectById(_editingObject.IdCCU);
                                if (ccu != null)
                                {
                                    _editingObject.Inputs = ccu.Inputs;
                                    _inputReload = true;
                                }
                            }
                            if (isInsert)
                            {
                                var ccu = Plugin.MainServerProvider.CCUs.GetObjectById(_editingObject.IdCCU);
                                if (ccu != null)
                                {
                                    _editingObject.Inputs = ccu.Inputs;
                                    _inputReload = true;
                                }
                            }
                        }
                        break;
                    case ObjectType.Output:
                        lock (_outputReloadLock)
                        {
                            var owOutput = false;
                            if (_editingObject.Outputs != null && _editingObject.Outputs.Count > 0)
                            {
                                if (_editingObject.Outputs
                                    .Any(output => output.IdOutput == idGuid))
                                {
                                    owOutput = true;
                                }
                            }

                            if (owOutput)
                            {
                                var ccu = Plugin.MainServerProvider.CCUs.GetObjectById(_editingObject.IdCCU);
                                if (ccu != null)
                                {
                                    _editingObject.Outputs = ccu.Outputs;
                                    _outputReload = true;
                                }
                            }
                            if (isInsert)
                            {
                                var ccu = Plugin.MainServerProvider.CCUs.GetObjectById(_editingObject.IdCCU);
                                if (ccu != null)
                                {
                                    _editingObject.Outputs = ccu.Outputs;
                                    _outputReload = true;
                                }
                            }
                        }
                        break;
                    case ObjectType.DoorEnvironment:
                        lock (_doorEnvironmentReloadLock)
                        {
                            var ownDoorEnv = false;
                            if (_editingObject.DoorEnvironments != null && _editingObject.DoorEnvironments.Count > 0)
                            {
                                if (_editingObject.DoorEnvironments
                                    .Any(de => de.IdDoorEnvironment == idGuid))
                                {
                                    ownDoorEnv = true;
                                }
                            }

                            if (ownDoorEnv)
                            {
                                var ccu = Plugin.MainServerProvider.CCUs.GetObjectById(_editingObject.IdCCU);
                                if (ccu != null)
                                {
                                    _editingObject.DoorEnvironments = ccu.DoorEnvironments;
                                    _doorEnvironmentReload = true;
                                }
                            }
                        }
                        break;
                }
            }
            catch
            {
            }
        }

        #endregion

        private CCUOnlineState _onlineState = CCUOnlineState.Unknown;

        private void RefreshCCUState(CCUOnlineState state)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<CCUOnlineState>(RefreshCCUState), state);
            }
            else
            {
                _onlineState = state;

                if (state == CCUOnlineState.Online)
                {
                    _eState.Text = GetString("Online");
                    _eState.BackColor = Color.LightGreen;
                }
                else if (state == CCUOnlineState.Offline)
                {
                    _eState.Text = GetString("Offline");
                    _eState.BackColor = Color.Red;
                }
                else
                {
                    _eState.Text = GetString("Unknown");
                    _eState.BackColor = Color.Yellow;
                }
            }
        }


        private CCUConfigurationState _configured = CCUConfigurationState.Unknown;
        private bool _isCCU0;

        private void RefreshCCUConfigured(CCUConfigurationState configured, bool isCCU0)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<CCUConfigurationState, bool>(RefreshCCUConfigured), configured, isCCU0);
            }
            else
            {
                _configured = configured;
                _isCCU0 = isCCU0;
                RefreshCCUSettings();
                TranslateCCUConfigured(configured);
                RefreshConfigurationButtons(false);
                EnsureEnabledCcuConfigurePasswordButtons();
            }
        }

        private void EnsureEnabledCcuConfigurePasswordButtons()
        {
            SetEnabledCcuPasswordButtons(
                GeneralOptionsForm.Singleton.IsSetCcuConfigurationPassword &&
                _configured == CCUConfigurationState.ConfiguredForThisServer &&
                _onlineState == CCUOnlineState.Online);
        }

        private void SetEnabledCcuPasswordButtons(bool enabled)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DBool2Void(SetEnabledCcuPasswordButtons), enabled);
            }
            else
            {
                _bChangeConfigurePassword.Enabled = enabled;
                _bCancelConfigurationPassword.Enabled = enabled;
            }
        }

        private void TranslateCCUConfigured(CCUConfigurationState configured)
        {
            switch (configured)
            {
                case CCUConfigurationState.Unconfigured:
                {
                    _eConfigured.Text = GetString("CCUConfigured_Unconfigured");
                    _eConfigured.BackColor = Color.White;

                    if (Plugin.MainServerProvider.CCUs.IsCat12Combo(GetEditingObject().IdCCU))
                    {
                        if (Plugin.MainServerProvider.CCUs.GetFreeCat12ComboLicenceCount() == 0)
                            _eConfigured.Text += @" - " + GetString("BlockedByLicence");
                    }

                    break;
                }
                case CCUConfigurationState.ConfiguredForAnotherServer:
                {
                    _eConfigured.Text = GetString("CCUConfigured_ConfiguredForAnotherServer");
                    _eConfigured.BackColor = Color.Red;
                    break;
                }
                case CCUConfigurationState.ConfiguredForThisServer:
                {
                    _eConfigured.Text = GetString("CCUConfigured_ConfiguredForThisServer");
                    _eConfigured.BackColor = Color.LightGreen;
                    break;
                }
                case CCUConfigurationState.Upgrading:
                {
                    _eConfigured.Text = GetString("CCUConfigured_Upgrading");
                    _eConfigured.BackColor = Color.Orange;
                    break;
                }
                case CCUConfigurationState.ConfiguredForThisServerUpgradeOnly:
                case CCUConfigurationState.ConfiguredForThisServerUpgradeOnlyWinCe:
                {
                    _eConfigured.Text = GetString("CCUConfigured_ConfiguredForThisServerUpgradeOnly");
                    _eConfigured.BackColor = Color.Gray;
                    break;
                }
                case CCUConfigurationState.Unknown:
                {
                    _eConfigured.Text = string.Empty;
                    _eConfigured.BackColor = Color.White;
                    break;
                }
                case CCUConfigurationState.ForceReconfiguration:
                {
                    _eConfigured.Text = GetString("CCUConfigured_ForceReconfiguration");
                    _eConfigured.BackColor = Color.LightGreen;
                    break;
                }
            }
        }

        private bool _isDisabledConfigurationsButtons;

        private void RefreshConfigurationButtons(bool enableConfigurationsButtons)
        {
            if (enableConfigurationsButtons || !_isDisabledConfigurationsButtons)
            {
                _isDisabledConfigurationsButtons = false;
                switch (_configured)
                {
                    case CCUConfigurationState.Unconfigured:
                    case CCUConfigurationState.ConfiguredForAnotherServer:
                    {
                        _bUnconfigure.Visible = false;
                        _bForceReconfiguration.Enabled = false;
                        _bConfigureForThisServer.Visible = true;
                        _bConfigureForThisServer.Enabled = true;
                        _ccuUpgradeFiles.EnableUpgrade = false;
                        _ceUpgradeFiles.EnableUpgrade = false;
                        _lAvailableDCUUpgrades.Enabled = false;
                        _lAvailableCRUpradeVersions.Enabled = false;
                        _bStopTransfer.Enabled = false;
                        _bStopTransferCEFile.Enabled = false;
                        _bRefresh.Enabled = false;
                        _bRefresh5.Enabled = false;
                        _bPrecreateDCU.Enabled = false;
                        break;
                    }
                    case CCUConfigurationState.ConfiguredForThisServer:
                    {
                        _bUnconfigure.Visible = true;
                        _bForceReconfiguration.Enabled = true;
                        _bConfigureForThisServer.Visible = false;
                        _ccuUpgradeFiles.EnableUpgrade = true;
                        _ceUpgradeFiles.EnableUpgrade = true;
                        _ccuUpgrade.Enabled = true;
                        _ccuUpgradeFiles.Enable = true;
                        _ceUpgrade.Enabled = true;
                        _ceUpgradeFiles.Enable = true;
                        _lAvailableDCUUpgrades.Enabled = true;
                        _lAvailableCRUpradeVersions.Enabled = true;
                        _bRefresh.Enabled = true;
                        _bRefresh5.Enabled = true;
                        _bPrecreateDCU.Enabled = true;
                        break;
                    }
                    case CCUConfigurationState.ConfiguredForThisServerUpgradeOnly:
                        _bUnconfigure.Visible = true;
                        _bForceReconfiguration.Enabled = false;
                        _bConfigureForThisServer.Visible = false;
                        _ccuUpgradeFiles.EnableUpgrade = true;
                        _ceUpgradeFiles.EnableUpgrade = true;
                        _ccuUpgrade.Enabled = true;
                        _ccuUpgradeFiles.Enable = true;
                        _ceUpgrade.Enabled = true;
                        _ceUpgradeFiles.Enable = true;
                        _lAvailableDCUUpgrades.Enabled = false;
                        _lAvailableCRUpradeVersions.Enabled = true;
                        _bRefresh.Enabled = true;
                        _bRefresh5.Enabled = true;
                        _bPrecreateDCU.Enabled = false;
                        _bStopTransfer.Enabled = false;
                        _bStopTransferCEFile.Enabled = false;
                        break;

                    case CCUConfigurationState.ConfiguredForThisServerUpgradeOnlyWinCe:
                        _bUnconfigure.Visible = true;
                        _bForceReconfiguration.Enabled = false;
                        _bConfigureForThisServer.Visible = false;
                        _ccuUpgradeFiles.EnableUpgrade = true;
                        _ceUpgradeFiles.EnableUpgrade = true;
                        _ccuUpgrade.Enabled = true;
                        _ccuUpgradeFiles.Enable = true;
                        _ceUpgrade.Enabled = true;
                        _ceUpgradeFiles.Enable = true;
                        _lAvailableDCUUpgrades.Enabled = false;
                        _lAvailableCRUpradeVersions.Enabled = false;
                        _bRefresh.Enabled = true;
                        _bRefresh5.Enabled = true;
                        _bPrecreateDCU.Enabled = false;
                        _bStopTransfer.Enabled = true;
                        _bStopTransferCEFile.Enabled = true;
                        break;

                    case CCUConfigurationState.Unknown:
                    {
                        _eConfigured.Text = string.Empty;
                        _eConfigured.BackColor = Color.White;
                        _bUnconfigure.Visible = false;
                        _bForceReconfiguration.Enabled = false;
                        _bConfigureForThisServer.Visible = true;
                        _bConfigureForThisServer.Enabled = false;
                        _ccuUpgradeFiles.EnableUpgrade = false;
                        _ceUpgradeFiles.EnableUpgrade = false;
                        break;
                    }
                    case CCUConfigurationState.ForceReconfiguration:
                    {
                        _bUnconfigure.Visible = false;
                        _bForceReconfiguration.Enabled = false;
                        _bConfigureForThisServer.Visible = true;
                        _bConfigureForThisServer.Enabled = false;
                        _ccuUpgradeFiles.EnableUpgrade = false;
                        _ceUpgradeFiles.EnableUpgrade = false;
                        _ccuUpgrade.Enabled = false;
                        _ceUpgrade.Enabled = false;
                        _lAvailableDCUUpgrades.Enabled = false;
                        _lAvailableCRUpradeVersions.Enabled = false;
                        break;
                    }
                }
            }
            else
            {
                _bUnconfigure.Visible = false;
                _bForceReconfiguration.Enabled = false;
                _bConfigureForThisServer.Visible = true;
                _bConfigureForThisServer.Enabled = false;
                _ccuUpgradeFiles.EnableUpgrade = false;
                _ceUpgradeFiles.EnableUpgrade = false;
            }
        }

        private void RefreshCCUSettings()
        {
            if (_onlineState == CCUOnlineState.Offline)
            {
                DisabledCCUSettings();
                _eIPAddress.Enabled = true;
            }
            else
            {
                switch (_configured)
                {
                    case CCUConfigurationState.ConfiguredForAnotherServer:
                    case CCUConfigurationState.Upgrading:
                    case CCUConfigurationState.ForceReconfiguration:
                        DisabledCCUSettings();
                        break;

                    case CCUConfigurationState.Unconfigured:
                        DisabledCCUSettings();
                        EnabledControls(_tpControl);
                        break;

                    case CCUConfigurationState.ConfiguredForThisServerUpgradeOnly:
                    case CCUConfigurationState.ConfiguredForThisServerUpgradeOnlyWinCe:
                        DisabledCCUSettings();
                        EnabledControls(_tpControl);
                        EnabledControls(_tpStatistics);
                        SafeThread.StartThread(RefreshAvailableUpgradesList);
                        SafeThread.StartThread(RefreshAvailableCEUpgradesList);
                        break;

                    default:
                        EnabledCCUSettings();
                        break;
                }
            }

            EnsureEnabledCcuConfigurePasswordButtons();
        }

        private void DisabledCCUSettings()
        {
            DisabledControls(_tpInformation);
            _bPrecreateCardReader.Enabled = true;
            DisabledControls(_tpControl);

            _ccuUpgradeFiles.Enable = false;
            _ceUpgradeFiles.Enable = false;
            _crUpgradeFiles.Enable = false;
            _dcuUpgradeFiles.Enable = false;
            
            DisabledControls(_tpTimeSettings);
            DisabledControls(_tpLevelBSI);
            DisabledControls(_tpPortSettings);
            DisabledControls(_tpCRUpgrade);
            DisabledControls(_tpDCUsUpgrade);
            DisabledControls(_tpAlarmSettings);
            DisabledControls(_tpStatistics);
            DisabledControls(_tpTesting);
            DisabledControls(_tpUserFolders);
            _dgvDoorEnvironments.Enabled = false;
            _bPrecreateDCU.Enabled = false;
            _cbOpenAllAlarmSettings.Enabled = true;
            _accordionAlarmSettings.SetEnabled(true);
            EnabledControls(_catsCcuOffline);
            EnabledControls(_eMaximumExpectedDCUCount);

            LockChanges();
            if (_isCCU0 && _editingObject.EnabledComPort == false)
            {
                _chbEnabledComPort.Checked = _editingObject.EnabledComPort;
            }
            UnlockChanges();
        }

        private void EnabledCCUSettings()
        {
            EnabledControls(_tpInformation);
            _bPrecreateCardReader.Enabled = true;
            EnabledControls(_tpControl);

            _ccuUpgradeFiles.Enable = true;
            _ceUpgradeFiles.Enable = true;
            _crUpgradeFiles.Enable = true;
            _dcuUpgradeFiles.Enable = true;

            EnabledControls(_tpTimeSettings);
            EnabledControls(_tpLevelBSI);
            EnabledControls(_tpPortSettings);
            EnabledControls(_tpCRUpgrade);
            EnabledControls(_tpDCUsUpgrade);
            EnabledControls(_tpAlarmSettings);
            EnabledControls(_tpStatistics);
            EnabledControls(_tpTesting);
            EnabledControls(_tpUserFolders);
            _dgvDoorEnvironments.Enabled = true;
            SafeThread.StartThread(RefreshAvailableUpgradesList);
            SafeThread.StartThread(RefreshAvailableCEUpgradesList);
            SafeThread.StartThread(RefreshAvailableCRUpgradeList);
            SafeThread.StartThread(RefreshAvailableDCUUpgradeList);
            SafeThread.StartThread(SetVerbosityLevelControl);
            SafeThread.StartThread(SetAllowedStatisticsOptions);

            RefreshIPSettings(_ipSetting);

            LockChanges();
            if (_isCCU0)
            {
                _chbEnabledComPort.Checked = _editingObject.EnabledComPort;
            }
            else
            {
                _chbEnabledComPort.Checked = true;
                _chbEnabledComPort.Enabled = false;
            }
            _bStopTransfer.Enabled = false;
            _bStopTransferCEFile.Enabled = false;
            UnlockChanges();
        }

        private void SetAllowedStatisticsOptions()
        {
            if (LoggedAsSuperAdmin)
            {
                Invoke(new MethodInvoker(() =>
                {
                    //labels
                    _lServerSended.Visible = true;
                    _lServerReceived.Visible = true;
                    _lServerDeserializeError.Visible = true;
                    _lServerReceivedError.Visible = true;
                    _lServerMsgRetry.Visible = true;
                    _lCcuSended.Visible = true;
                    _lCcuReceived.Visible = true;
                    _lCcuDeserializeError.Visible = true;
                    _lCcuReceivedError.Visible = true;
                    _lCcuMsgRetry.Visible = true;
                    _lCommandTimeoutCount.Visible = true;
                    _lCCUUnprocessedEvents.Visible = true;

                    //text boxes
                    _eServerSended.Visible = true;
                    _eServerReceived.Visible = true;
                    _eServerDeserializeError.Visible = true;
                    _eServerReceivedError.Visible = true;
                    _eServerMsgRetry.Visible = true;
                    _eCcuSended.Visible = true;
                    _eCcuReceived.Visible = true;
                    _eCcuDeserializeError.Visible = true;
                    _eCcuReceivedError.Visible = true;
                    _eCcuMsgRetry.Visible = true;
                    _eCommandTimeouts.Visible = true;
                    _eCCUUnprocessedEvents.Visible = true;

                    //buttons
                    _bResetServerSended.Visible = true;
                    _bServerResetReceived.Visible = true;
                    _bServerResetDeserializeError.Visible = true;
                    _bServerResetReceivedError.Visible = true;
                    _bResetServerMsgRetry.Visible = true;
                    _bCcuResetSended.Visible = true;
                    _bCcuResetReceived.Visible = true;
                    _bCcuResetDeserializeError.Visible = true;
                    _bCcuResetReceivedError.Visible = true;
                    _bCcuMsgRetry.Visible = true;
                    _bReset1.Visible = true;
                }));
            }
        }

        private void RefreshAvailableCEUpgradesList()
        {
            if (Plugin.MainServerProvider == null)
                return;

            var files = Plugin.MainServerProvider.GetAvailableCEUpgrades().Select(obj => new CeUpgradeFile(obj));

            Invoke((MethodInvoker) delegate
            {
                _ceUpgradeFiles.Files = files.Cast<object>().ToList();
            });
        }

        private class CeUpgradeFile
        {
            public string FileName { get; private set; }
            public int Version { get; private set; }

            public CeUpgradeFile(string[] upgradeCeObject)
            {
                FileName = upgradeCeObject[0];
                Version = Int32.Parse(upgradeCeObject[1]);
            }

            public override string ToString()
            {
                return string.Format("{0} [{1}]",
                    FileName,
                    Version);
            }
        }

        //If CCU is configured doesn't matter if is in online or offline state
        private bool IsConfigured()
        {
            if (!Insert
                && !CgpClient.Singleton.IsConnectionLost(false)
                && Plugin.MainServerProvider.CCUs.GetObjectById(_editingObject.IdCCU).IsConfigured)
            {
                return true;
            }

            return false;
        }

        protected override void DisableForm()
        {
            base.DisableForm();

            _bCancel.Enabled = true;
            _bPrecreateCardReader.Enabled = true;
        }

        protected override bool GetValues()
        {
            try
            {
                if (!string.IsNullOrEmpty(_editingObject.IPAddress) 
                    && _eIPAddress.Text != _editingObject.IPAddress
                    && _eName.Text.Contains(_editingObject.IPAddress))
                {
                    _eName.Text = _eName.Text.Replace(_editingObject.IPAddress, _eIPAddress.Text);
                }

                if (_editingObject.Name != _eName.Text)
                {
                    var objectsToRename = Plugin.MainServerProvider.CCUs.GetObjectsToRename(_editingObject.IdCCU);

                    if (objectsToRename != null 
                        && objectsToRename.Count > 0)
                    {
                        var multiRenameDialog = new MultiRenameDialog(
                            Plugin.GetPluginObjectsImages(),
                            (List<IModifyObject>) objectsToRename,
                            _editingObject.Name,
                            _eName.Text,
                            NCASClient.LocalizationHelper);

                        if (multiRenameDialog.ShowDialog() == DialogResult.OK)
                        {
                            Plugin.MainServerProvider.CCUs.RenameObjects(
                                _editingObject.IdCCU,
                                _eName.Text,
                                multiRenameDialog.RenameAll
                                    ? objectsToRename
                                    : multiRenameDialog.SelectedItems);
                        }
                    }
                }

                _editingObject.Name = _eName.Text;
                _editingObject.Description = _eDescription.Text;
                _editingObject.IPAddress = _eIPAddress.Text;
                _editingObject.TimeShift = (int) (_eHours.Value*60 + _eMinutes.Value);
                if (_cbTimeZone.SelectedItem != null)
                {
                    var tzi = (TimeZoneInfo) _cbTimeZone.SelectedItem;
                    _editingObject.TimeZoneInfo = tzi.Id;
                }
                _editingObject.AlarmBreak = (double) _nAlarmBreak.Value;
                _editingObject.ShortNormal = (double) _nShortNormal.Value;
                _editingObject.NormalAlarm = (double) _nNormalAlarm.Value;
                SetBaudRate();
                _editingObject.SNTPIpAddresses = GetSNTPIpAddresses();
                _editingObject.InheritGeneralNtpSettings = _chbInheritedGeneralNtpSettings.Checked;
                _editingObject.WatchdogOutput = _cmsoCcuOffline.SpecialOutput;

                if (_editingObject.WatchdogOutput != null)
                {
                    _editingObject.WatchdogOutput.ControlType = (byte) OutputControl.watchdog;
                }

                _editingObject.AlarmOffline = _catsCcuOffline.AlarmEnabledCheckState;
                _editingObject.BlockAlarmOffline = _catsCcuOffline.BlockAlarmCheckState;

                if (_catsCcuOffline.ObjectForBlockingAlarm != null)
                {
                    _editingObject.ObjBlockAlarmOfflineObjectType =
                        (byte) _catsCcuOffline.ObjectForBlockingAlarm.GetObjectType();

                    _editingObject.ObjBlockAlarmOfflineId =
                        (Guid) _catsCcuOffline.ObjectForBlockingAlarm.GetId();
                }
                else
                {
                    _editingObject.ObjBlockAlarmOfflineObjectType = null;
                    _editingObject.ObjBlockAlarmOfflineId = null;
                }

                if (_editingObject.CcuAlarmArcs != null)
                    _editingObject.CcuAlarmArcs.Clear();
                else
                    _editingObject.CcuAlarmArcs = new List<CcuAlarmArc>();
                
                SaveAlarmArcs(
                    _cmaaCcuOffline.AlarmArcs,
                    AlarmType.CCU_Offline);

                _editingObject.AlarmTamper = _catsCcuTamperSabotage.AlarmEnabledCheckState;
                _editingObject.BlockAlarmTamper = _catsCcuTamperSabotage.BlockAlarmCheckState;
                _editingObject.EventlogDuringBlockAlarmTamper = _catsCcuTamperSabotage.EventlogDuringBlockedAlarmCheckState;

                if (_catsCcuTamperSabotage.ObjectForBlockingAlarm != null)
                {
                    _editingObject.ObjBlockAlarmTamperObjectType =
                        (byte)_catsCcuTamperSabotage.ObjectForBlockingAlarm.GetObjectType();

                    _editingObject.ObjBlockAlarmTamperId =
                        (Guid)_catsCcuTamperSabotage.ObjectForBlockingAlarm.GetId();
                }
                else
                {
                    _editingObject.ObjBlockAlarmTamperObjectType = null;
                    _editingObject.ObjBlockAlarmTamperId = null;
                }

                SaveAlarmArcs(
                    _cmaaCcuTamperSabotage.AlarmArcs,
                    AlarmType.CCU_TamperSabotage);

                _editingObject.OutputTamper = _cmsoCcuTamperSabotage.SpecialOutput;

                _editingObject.AlarmPrimaryPowerMissing = _catsCcuPrimaryPowerMissing.AlarmEnabledCheckState;
                _editingObject.BlockAlarmPrimaryPowerMissing = _catsCcuPrimaryPowerMissing.BlockAlarmCheckState;
                _editingObject.EventlogDuringBlockAlarmPrimaryPowerMissing = _catsCcuPrimaryPowerMissing.EventlogDuringBlockedAlarmCheckState;

                if (_catsCcuPrimaryPowerMissing.ObjectForBlockingAlarm != null)
                {
                    _editingObject.ObjBlockAlarmPrimaryPowerMissingObjectType =
                        (byte)_catsCcuPrimaryPowerMissing.ObjectForBlockingAlarm.GetObjectType();

                    _editingObject.ObjBlockAlarmPrimaryPowerMissingId =
                        (Guid)_catsCcuPrimaryPowerMissing.ObjectForBlockingAlarm.GetId();
                }
                else
                {
                    _editingObject.ObjBlockAlarmPrimaryPowerMissingObjectType = null;
                    _editingObject.ObjBlockAlarmPrimaryPowerMissingId = null;
                }

                SaveAlarmArcs(
                    _cmaaCcuPrimaryPowerMissing.AlarmArcs,
                    AlarmType.CCU_PrimaryPowerMissing);

                _editingObject.OutputPrimaryPowerMissing = _cmsoCcuPrimaryPowerMissing.SpecialOutput;

                _editingObject.AlarmBatteryIsLow = _catsCcuBatteryIsLow.AlarmEnabledCheckState;
                _editingObject.BlockAlarmBatteryIsLow = _catsCcuBatteryIsLow.BlockAlarmCheckState;
                _editingObject.EventlogDuringBlockAlarmBatteryIsLow = _catsCcuBatteryIsLow.EventlogDuringBlockedAlarmCheckState;

                if (_catsCcuBatteryIsLow.ObjectForBlockingAlarm != null)
                {
                    _editingObject.ObjBlockAlarmBatteryIsLowObjectType =
                        (byte)_catsCcuBatteryIsLow.ObjectForBlockingAlarm.GetObjectType();

                    _editingObject.ObjBlockAlarmBatteryIsLowId =
                        (Guid)_catsCcuBatteryIsLow.ObjectForBlockingAlarm.GetId();
                }
                else
                {
                    _editingObject.ObjBlockAlarmBatteryIsLowObjectType = null;
                    _editingObject.ObjBlockAlarmBatteryIsLowId = null;
                }

                SaveAlarmArcs(
                    _cmaaCcuBatteryIsLow.AlarmArcs,
                    AlarmType.CCU_BatteryLow);

                _editingObject.OutputBatteryIsLow = _cmsoCcuBatteryIsLow.SpecialOutput;

                _editingObject.AlarmUpsOutputFuse = _catsCcuUpsOutputFuse.AlarmEnabledCheckState;
                _editingObject.BlockAlarmUpsOutputFuse = _catsCcuUpsOutputFuse.BlockAlarmCheckState;
                _editingObject.EventlogDuringBlockAlarmUpsOutputFuse = _catsCcuUpsOutputFuse.EventlogDuringBlockedAlarmCheckState;

                if (_catsCcuUpsOutputFuse.ObjectForBlockingAlarm != null)
                {
                    _editingObject.ObjBlockAlarmUpsOutputFuseObjectType =
                        (byte)_catsCcuUpsOutputFuse.ObjectForBlockingAlarm.GetObjectType();

                    _editingObject.ObjBlockAlarmUpsOutputFuseId =
                        (Guid)_catsCcuUpsOutputFuse.ObjectForBlockingAlarm.GetId();
                }
                else
                {
                    _editingObject.ObjBlockAlarmUpsOutputFuseObjectType = null;
                    _editingObject.ObjBlockAlarmUpsOutputFuseId = null;
                }

                SaveAlarmArcs(
                    _cmaaCcuUpsOutputFuse.AlarmArcs,
                    AlarmType.Ccu_Ups_OutputFuse);

                _editingObject.OutputUpsOutputFuse = _cmsoCcuUpsOutputFuse.SpecialOutput;

                _editingObject.AlarmUpsBatteryFault = _catsCcuUpsBatteryFault.AlarmEnabledCheckState;
                _editingObject.BlockAlarmUpsBatteryFault = _catsCcuUpsBatteryFault.BlockAlarmCheckState;
                _editingObject.EventlogDuringBlockAlarmUpsBatteryFault = _catsCcuUpsBatteryFault.EventlogDuringBlockedAlarmCheckState;

                if (_catsCcuUpsBatteryFault.ObjectForBlockingAlarm != null)
                {
                    _editingObject.ObjBlockAlarmUpsBatteryFaultObjectType =
                        (byte)_catsCcuUpsBatteryFault.ObjectForBlockingAlarm.GetObjectType();

                    _editingObject.ObjBlockAlarmUpsBatteryFaultId =
                        (Guid)_catsCcuUpsBatteryFault.ObjectForBlockingAlarm.GetId();
                }
                else
                {
                    _editingObject.ObjBlockAlarmUpsBatteryFaultObjectType = null;
                    _editingObject.ObjBlockAlarmUpsBatteryFaultId = null;
                }

                SaveAlarmArcs(
                    _cmaaCcuUpsBatteryFault.AlarmArcs,
                    AlarmType.Ccu_Ups_BatteryFault);

                _editingObject.OutputUpsBatteryFault = _cmsoCcuUpsBatteryFault.SpecialOutput;

                _editingObject.AlarmUpsBatteryFuse = _catsCcuUpsBatteryFuse.AlarmEnabledCheckState;
                _editingObject.BlockAlarmUpsBatteryFuse = _catsCcuUpsBatteryFuse.BlockAlarmCheckState;
                _editingObject.EventlogDuringBlockAlarmUpsBatteryFuse = _catsCcuUpsBatteryFuse.EventlogDuringBlockedAlarmCheckState;

                if (_catsCcuUpsBatteryFuse.ObjectForBlockingAlarm != null)
                {
                    _editingObject.ObjBlockAlarmUpsBatteryFuseObjectType =
                        (byte)_catsCcuUpsBatteryFuse.ObjectForBlockingAlarm.GetObjectType();

                    _editingObject.ObjBlockAlarmUpsBatteryFuseId =
                        (Guid)_catsCcuUpsBatteryFuse.ObjectForBlockingAlarm.GetId();
                }
                else
                {
                    _editingObject.ObjBlockAlarmUpsBatteryFuseObjectType = null;
                    _editingObject.ObjBlockAlarmUpsBatteryFuseId = null;
                }

                SaveAlarmArcs(
                    _cmaaCcuUpsBatteryFuse.AlarmArcs,
                    AlarmType.Ccu_Ups_BatteryFuse);

                _editingObject.OutputUpsBatteryFuse = _cmsoCcuUpsBatteryFuse.SpecialOutput;

                _editingObject.AlarmUpsOvertemperature = _catsCcuUpsOvertemperature.AlarmEnabledCheckState;
                _editingObject.BlockAlarmUpsOvertemperature = _catsCcuUpsOvertemperature.BlockAlarmCheckState;
                _editingObject.EventlogDuringBlockAlarmUpsOvertemperature = _catsCcuUpsOvertemperature.EventlogDuringBlockedAlarmCheckState;

                if (_catsCcuUpsOvertemperature.ObjectForBlockingAlarm != null)
                {
                    _editingObject.ObjBlockAlarmUpsOvertemperatureObjectType =
                        (byte)_catsCcuUpsOvertemperature.ObjectForBlockingAlarm.GetObjectType();

                    _editingObject.ObjBlockAlarmUpsOvertemperatureId =
                        (Guid)_catsCcuUpsOvertemperature.ObjectForBlockingAlarm.GetId();
                }
                else
                {
                    _editingObject.ObjBlockAlarmUpsOvertemperatureObjectType = null;
                    _editingObject.ObjBlockAlarmUpsOvertemperatureId = null;
                }

                SaveAlarmArcs(
                    _cmaaCcuUpsOvertemperature.AlarmArcs,
                    AlarmType.Ccu_Ups_Overtemperature);

                _editingObject.OutputUpsOvertemperature = _cmsoCcuUpsOvertemperature.SpecialOutput;

                _editingObject.AlarmUpsTamperSabotage = _catsCcuUpsTamper.AlarmEnabledCheckState;
                _editingObject.BlockAlarmUpsTamperSabotage = _catsCcuUpsTamper.BlockAlarmCheckState;
                _editingObject.EventlogDuringBlockAlarmUpsTamperSabotage = _catsCcuUpsTamper.EventlogDuringBlockedAlarmCheckState;

                if (_catsCcuUpsTamper.ObjectForBlockingAlarm != null)
                {
                    _editingObject.ObjBlockAlarmUpsTamperSabotageObjectType =
                        (byte)_catsCcuUpsTamper.ObjectForBlockingAlarm.GetObjectType();

                    _editingObject.ObjBlockAlarmUpsTamperSabotageId =
                        (Guid)_catsCcuUpsTamper.ObjectForBlockingAlarm.GetId();
                }
                else
                {
                    _editingObject.ObjBlockAlarmUpsTamperSabotageObjectType = null;
                    _editingObject.ObjBlockAlarmUpsTamperSabotageId = null;
                }

                SaveAlarmArcs(
                    _cmaaCcuUpsTamper.AlarmArcs,
                    AlarmType.Ccu_Ups_TamperSabotage);

                _editingObject.OutputUpsTamperSabotage = _cmsoCcuUpsTamper.SpecialOutput;

                _editingObject.AlarmFuseOnExtensionBoard = _catsCcuFuseOnExtensionBoard.AlarmEnabledCheckState;
                _editingObject.BlockAlarmFuseOnExtensionBoard = _catsCcuFuseOnExtensionBoard.BlockAlarmCheckState;
                _editingObject.EventlogDuringBlockAlarmFuseOnExtensionBoard = _catsCcuFuseOnExtensionBoard.EventlogDuringBlockedAlarmCheckState;

                if (_catsCcuFuseOnExtensionBoard.ObjectForBlockingAlarm != null)
                {
                    _editingObject.ObjBlockAlarmFuseOnExtensionBoardObjectType =
                        (byte)_catsCcuFuseOnExtensionBoard.ObjectForBlockingAlarm.GetObjectType();

                    _editingObject.ObjBlockAlarmFuseOnExtensionBoardId =
                        (Guid)_catsCcuFuseOnExtensionBoard.ObjectForBlockingAlarm.GetId();
                }
                else
                {
                    _editingObject.ObjBlockAlarmFuseOnExtensionBoardObjectType = null;
                    _editingObject.ObjBlockAlarmFuseOnExtensionBoardId = null;
                }

                SaveAlarmArcs(
                    _cmaaCcuFuseOnExtensionBoard.AlarmArcs,
                    AlarmType.CCU_ExtFuse);

                _editingObject.OutputFuseOnExtensionBoard = _cmsoCcuFuseOnExtensionBoard.SpecialOutput;

                _editingObject.AlarmCcuCatUnreachable = _catsCcuCatUnreachable.AlarmEnabledCheckState;
                
                _editingObject.AlarmCcuCatUnreachablePresentationGroup =
                    _catsCcuCatUnreachable.PresentationGroup;

                _editingObject.AlarmCcuTransferToArcTimedOut = _catsCcuTransferToArcTimedOut.AlarmEnabledCheckState;
                
                _editingObject.AlarmCcuTransferToArcTimedOutPresentationGroup =
                    _catsCcuTransferToArcTimedOut.PresentationGroup;

                if (Insert && _cbMainboarType.SelectedItem is MainBoardVariant)
                {
                    var imbt = (MainBoardVariant) _cbMainboarType.SelectedItem;
                    _editingObject.CcuMainboardType = (byte) imbt;
                    if (imbt == MainBoardVariant.CCU40 ||
                        imbt == MainBoardVariant.CCU12 ||
                        imbt == MainBoardVariant.CCU05 ||
                        imbt == MainBoardVariant.CAT12CE)
                    {
                        _editingObject.InputsCount = (int) _nudInputCount.Value;
                        _editingObject.OutputsCount = (int) _nudOutputCount.Value;
                        GenerateCcu40Items(imbt);
                    }
                }

                _editingObject.LocalAlarmInstruction = GetNewLocalAlarmInstruction();

                switch (_cbSyncingTimeFromServer.CheckState)
                {
                    case CheckState.Checked:
                        _editingObject.SyncingTimeFromServer = true;
                        break;
                    case CheckState.Unchecked:
                        _editingObject.SyncingTimeFromServer = false;
                        break;
                    case CheckState.Indeterminate:
                        _editingObject.SyncingTimeFromServer = null;
                        break;
                }

                _editingObject.AlarmTransmitter = _alarmTransmitter;
                _editingObject.IndexCCU = (int)_eIndex.Value;

                return true;
            }
            catch
            {
                Dialog.Error(GetString("ErrorGetValuesFailed"));
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
                    _editingObject.CcuAlarmArcs.Add(
                        new CcuAlarmArc(
                            _editingObject,
                            alarmArc,
                            alarmType));
                }
            }
        }

        private void GenerateCcu40Items(MainBoardVariant mainBoardType)
        {
            //commented to avoid creating inputs from client
            //CreateInputs();            
            //CreateOutputs();
            CreateDSM(mainBoardType);
        }

/*
        private void CreateInputs()
        {
            try
            {
                _editingObject.Inputs = new List<Input>();
                _editingObject.InputsCount = (int)_nudInputCount.Value;
                for (var i = 0; i < _nudInputCount.Value; i++)
                {
                    var newInput = new Input();
                    newInput.EnableParentInFullName = Plugin.MainServerProvider.GetEnableParentInFullName();
                    newInput.InputNumber = (byte)i;

                    if (newInput.EnableParentInFullName)
                        newInput.Name = "Input" + (i + 1);
                    else
                        newInput.Name = _editingObject.Name + StringConstants.SLASHWITHSPACES + "Input" + (i + 1);

                    newInput.NickName = "Input" + (i + 1);
                    newInput.InputType = (byte)InputType.DI;
                    newInput.Inverted = false;
                    newInput.HighPriority = false;
                    newInput.OffACK = false;
                    newInput.DelayToOn = 0;
                    newInput.DelayToOff = 0;
                    newInput.TamperDelayToOn = 0;
                    _editingObject.Inputs.Add(newInput);
                }
            }
            catch { }
        }
*/

/*
        private void CreateOutputs()
        {
            try
            {
                _editingObject.Outputs = new List<Output>();
                _editingObject.OutputsCount = (int)_nudOutputCount.Value;
                for (var i = 0; i < _nudOutputCount.Value; i++)
                {
                    var newOutput = new Output();
                    newOutput.OutputNumber = (byte)i;
                    if (Plugin.MainServerProvider.GetEnableParentInFullName())
                        newOutput.Name = "Output" + (i + 1);
                    else
                        newOutput.Name = _editingObject.Name + StringConstants.SLASHWITHSPACES + "Output" + (i + 1);
                    newOutput.OutputType = 0;
                    newOutput.DelayToOn = 0;
                    newOutput.DelayToOff = 0;
                    _editingObject.Outputs.Add(newOutput);
                }
            }
            catch { }
        }
*/

        private void CreateDSM(MainBoardVariant mainBoardType)
        {
            try
            {
                _editingObject.DoorEnvironments = new List<DoorEnvironment>();

                var upperCount = 0;
                // these values might be fixed
                switch (mainBoardType)
                {
                    case MainBoardVariant.CCU40:
                    {
                        string localisedName;
                        object licenseValue;

                        upperCount = NCASConstants.CCU40_MAX_DSM_COUNT;

                        if (CgpClient.Singleton.MainServerProvider.GetLicensePropertyInfo(
                            RequiredLicenceProperties.CCU40MaxDsm.ToString(), out localisedName,
                            out licenseValue))
                        {
                            var upperCountByLicense = (int) licenseValue;
                            if (upperCountByLicense >= 0 &&
                                upperCount > upperCountByLicense)
                            {
                                upperCount = upperCountByLicense;
                            }
                        }

                        break;
                    }
                    case MainBoardVariant.CCU12:
                    {
                        string localisedName;
                        object licenseValue;

                        upperCount = NCASConstants.CCU12_MAX_DSM_COUNT;

                        if (CgpClient.Singleton.MainServerProvider.GetLicensePropertyInfo(
                            RequiredLicenceProperties.CAT12CEMaxDsm.ToString(), out localisedName,
                            out licenseValue))
                        {
                            var upperCountByLicense = (int) licenseValue;
                            if (upperCountByLicense >= 0 &&
                                upperCount > upperCountByLicense)
                            {
                                upperCount = upperCountByLicense;
                            }
                        }

                        break;
                    }
                    case MainBoardVariant.CCU05:
                    {
                        string localisedName;
                        object licenseValue;

                        upperCount = NCASConstants.CCU05_MAX_DSM_COUNT;

                        if (CgpClient.Singleton.MainServerProvider.GetLicensePropertyInfo(
                            RequiredLicenceProperties.CCU05MaxDsm.ToString(), out localisedName,
                            out licenseValue))
                        {
                            var upperCountByLicense = (int) licenseValue;
                            if (upperCountByLicense >= 0 &&
                                upperCount > upperCountByLicense)
                            {
                                upperCount = upperCountByLicense;
                            }
                        }

                        break;
                    }
                    case MainBoardVariant.CAT12CE:
                    {
                        string localisedName;
                        object licenseValue;

                        upperCount = NCASConstants.CAT12CE_MAX_DSM_COUNT;

                        if (CgpClient.Singleton.MainServerProvider.GetLicensePropertyInfo(
                            RequiredLicenceProperties.CAT12CEMaxDsm.ToString(), out localisedName,
                            out licenseValue))
                        {
                            var upperCountByLicense = (int) licenseValue;
                            if (upperCountByLicense >= 0 &&
                                upperCount > upperCountByLicense)
                            {
                                upperCount = upperCountByLicense;
                            }
                        }

                        break;
                    }
                }

                for (var i = 0; i < upperCount; i++)
                {
                    var newDoorEnvironment = new DoorEnvironment();
                    newDoorEnvironment.Number = (byte) i;
                    newDoorEnvironment.Name = "Door environmnet " + (i + 1);
                    _editingObject.DoorEnvironments.Add(newDoorEnvironment);
                }
            }
            catch
            {
            }
        }

        private bool Ping(string ipAddress, int count)
        {
            var pinger = new Ping();

            try
            {
                for (int i = 0; i < count; i++)
                {
                    var reply = pinger.Send(ipAddress, 300);

                    if (reply.Status == IPStatus.Success)
                        return true;
                }

                return false;
            }
            catch (PingException)
            {
                return false;
            }
        }

        protected override bool CheckValues()
        {
            if (string.IsNullOrEmpty(_tbCrEvetlogLimitedSizeValue.Text))
            {
                ControlNotification.Singleton.Error(NotificationPriority.JustOne,
                    _tbCrEvetlogLimitedSizeValue,
                    GetString("ErrorEventlogLimitedSize"), ControlNotificationSettings.Default);

                _tcCCU.SelectedTab = _tpAlarmSettings;

                _tbCrEvetlogLimitedSizeValue.Focus();
                return false;
            }
            var eventlogLimitedSize = Int32.Parse(_tbCrEvetlogLimitedSizeValue.Text);

            if (eventlogLimitedSize < 10 || eventlogLimitedSize > 1000)
            {
                ControlNotification.Singleton.Error(NotificationPriority.JustOne,
                    _tbCrEvetlogLimitedSizeValue,
                    GetString("ErrorEventlogLimitedSize"), ControlNotificationSettings.Default);

                _tcCCU.SelectedTab = _tpAlarmSettings;

                _tbCrEvetlogLimitedSizeValue.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(_tbLastEventTimeForMarkAlarmArea.Text))
            {
                ControlNotification.Singleton.Error(NotificationPriority.JustOne,
                    _tbLastEventTimeForMarkAlarmArea,
                    GetString("ErrorLastEventTimeForMarkAlarmArea"), ControlNotificationSettings.Default);

                _tcCCU.SelectedTab = _tpAlarmSettings;

                _tbLastEventTimeForMarkAlarmArea.Focus();
                return false;
            }
            var lastEventTime = Int32.Parse(_tbLastEventTimeForMarkAlarmArea.Text);

            if (lastEventTime > 100000)
            {
                ControlNotification.Singleton.Error(NotificationPriority.JustOne,
                    _tbLastEventTimeForMarkAlarmArea,
                    GetString("ErrorLastEventTimeForMarkAlarmArea"), ControlNotificationSettings.Default);

                _tcCCU.SelectedTab = _tpAlarmSettings;

                _tbLastEventTimeForMarkAlarmArea.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(_eName.Text))
            {
                ControlNotification.Singleton.Error(NotificationPriority.JustOne, _eName,
                    GetString("ErrorInsertCcuName"), ControlNotificationSettings.Default);
                _eName.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(_eIPAddress.Text))
            {
                ControlNotification.Singleton.Error(NotificationPriority.JustOne, _eIPAddress,
                    GetString("ErrorInsertIPAddress"), ControlNotificationSettings.Default);

                _tcCCU.SelectedTab = _tpInformation;

                _eIPAddress.Focus();

                return false;
            }
            if (!IPHelper.IsValid(_eIPAddress.Text))
            {
                ControlNotification.Singleton.Error(NotificationPriority.JustOne, _eIPAddress,
                    GetString("ErrorWrongIPAddress"), ControlNotificationSettings.Default);

                _tcCCU.SelectedTab = _tpInformation;

                _eIPAddress.Focus();
                return false;
            }
            if (_eIPAddress.Text == GeneralOptions.Singleton.ServerAddress)
            {
                ControlNotification.Singleton.Error(NotificationPriority.JustOne, _eIPAddress,
                    GetString("ErrorIPAddressTheSameAsServerIPAddress"), ControlNotificationSettings.Default);

                _tcCCU.SelectedTab = _tpInformation;

                _eIPAddress.Focus();
                return false;
            }

            if (_nShortNormal.Value >= _nNormalAlarm.Value)
            {
                ControlNotification.Singleton.Error(NotificationPriority.JustOne, _nShortNormal,
                    GetString("ErrorShortBiggerNormalValue"), ControlNotificationSettings.Default);

                _tcCCU.SelectedTab = _tpLevelBSI;

                _nShortNormal.Focus();
                return false;
            }
            if (_nNormalAlarm.Value >= _nAlarmBreak.Value)
            {
                ControlNotification.Singleton.Error(NotificationPriority.JustOne, _nNormalAlarm,
                    GetString("ErrorNormalBiggerAlarmValue"), ControlNotificationSettings.Default);

                _tcCCU.SelectedTab = _tpLevelBSI;

                _nNormalAlarm.Focus();
                return false;
            }
            if (_cmsoCcuOffline.SpecialOutput != null)
            {
                if (Plugin.MainServerProvider.DoorEnvironments.IsOutputInDoorEnvironmentsActuators(_cmsoCcuOffline.SpecialOutput))
                {
                    ControlNotification.Singleton.Error(
                        NotificationPriority.JustOne,
                        _cmsoCcuOffline,
                        GetString("ErrorOutputUsedLikeActuators"),
                        ControlNotificationSettings.Default);

                    _tcCCU.SelectedTab = _tpAlarmSettings;

                    _cmsoCcuOffline.Focus();
                    return false;
                }
            }
            if (!_catsCcuOffline.ControlValues(
                control =>
                    _openAndScrollToControlByAlarmType[AlarmType.CCU_Offline]()))
            {
                return false;
            }
            if (_cmsoCcuTamperSabotage.SpecialOutput != null)
            {
                if (Plugin.MainServerProvider.DoorEnvironments.IsOutputInDoorEnvironmentsActuators(_cmsoCcuTamperSabotage.SpecialOutput))
                {
                    _openAndScrollToControlByAlarmType[AlarmType.CCU_TamperSabotage]();

                    ControlNotification.Singleton.Error(
                        NotificationPriority.JustOne,
                        _cmsoCcuTamperSabotage,
                        GetString("ErrorOutputUsedLikeActuators"),
                        ControlNotificationSettings.Default);

                    _cmsoCcuTamperSabotage.Focus();
                    return false;
                }
            }
            if (!_catsCcuTamperSabotage.ControlValues(
                control =>
                    _openAndScrollToControlByAlarmType[AlarmType.CCU_TamperSabotage]()))
            {
                return false;
            }
            if (_cmsoCcuPrimaryPowerMissing.SpecialOutput != null)
            {
                if (Plugin.MainServerProvider.DoorEnvironments.IsOutputInDoorEnvironmentsActuators(_cmsoCcuPrimaryPowerMissing.SpecialOutput))
                {
                    _openAndScrollToControlByAlarmType[AlarmType.CCU_PrimaryPowerMissing]();

                    ControlNotification.Singleton.Error(
                        NotificationPriority.JustOne,
                        _cmsoCcuPrimaryPowerMissing,
                        GetString("ErrorOutputUsedLikeActuators"),
                        ControlNotificationSettings.Default);

                    _cmsoCcuPrimaryPowerMissing.Focus();
                    return false;
                }
            }
            if (!_catsCcuPrimaryPowerMissing.ControlValues(
                control =>
                    _openAndScrollToControlByAlarmType[AlarmType.CCU_PrimaryPowerMissing]()))
            {
                return false;
            }
            if (_cmsoCcuBatteryIsLow.SpecialOutput != null)
            {
                if (Plugin.MainServerProvider.DoorEnvironments.IsOutputInDoorEnvironmentsActuators(_cmsoCcuBatteryIsLow.SpecialOutput))
                {
                    _openAndScrollToControlByAlarmType[AlarmType.CCU_BatteryLow]();

                    ControlNotification.Singleton.Error(
                        NotificationPriority.JustOne,
                        _cmsoCcuBatteryIsLow,
                        GetString("ErrorOutputUsedLikeActuators"),
                        ControlNotificationSettings.Default);

                    _cmsoCcuBatteryIsLow.Focus();
                    return false;
                }
            }
            if (!_catsCcuBatteryIsLow.ControlValues(
                control =>
                    _openAndScrollToControlByAlarmType[AlarmType.CCU_BatteryLow]()))
            {
                return false;
            }
            if (_cmsoCcuUpsOutputFuse.SpecialOutput != null)
            {
                if (Plugin.MainServerProvider.DoorEnvironments.IsOutputInDoorEnvironmentsActuators(_cmsoCcuUpsOutputFuse.SpecialOutput))
                {
                    _openAndScrollToControlByAlarmType[AlarmType.Ccu_Ups_OutputFuse]();

                    ControlNotification.Singleton.Error(
                        NotificationPriority.JustOne,
                        _cmsoCcuUpsOutputFuse,
                        GetString("ErrorOutputUsedLikeActuators"),
                        ControlNotificationSettings.Default);

                    _cmsoCcuUpsOutputFuse.Focus();
                    return false;
                }
            }
            if (!_catsCcuUpsOutputFuse.ControlValues(
                control =>
                    _openAndScrollToControlByAlarmType[AlarmType.Ccu_Ups_OutputFuse]()))
            {
                return false;
            }
            if (_cmsoCcuUpsBatteryFault.SpecialOutput != null)
            {
                if (Plugin.MainServerProvider.DoorEnvironments.IsOutputInDoorEnvironmentsActuators(_cmsoCcuUpsBatteryFault.SpecialOutput))
                {
                    _openAndScrollToControlByAlarmType[AlarmType.Ccu_Ups_BatteryFault]();

                    ControlNotification.Singleton.Error(
                        NotificationPriority.JustOne,
                        _cmsoCcuUpsBatteryFault,
                        GetString("ErrorOutputUsedLikeActuators"),
                        ControlNotificationSettings.Default);

                    _cmsoCcuUpsBatteryFault.Focus();
                    return false;
                }
            }
            if (!_catsCcuUpsBatteryFault.ControlValues(
                control =>
                    _openAndScrollToControlByAlarmType[AlarmType.Ccu_Ups_BatteryFault]()))
            {
                return false;
            }
            if (_cmsoCcuUpsBatteryFuse.SpecialOutput != null)
            {
                if (Plugin.MainServerProvider.DoorEnvironments.IsOutputInDoorEnvironmentsActuators(_cmsoCcuUpsBatteryFuse.SpecialOutput))
                {
                    _openAndScrollToControlByAlarmType[AlarmType.Ccu_Ups_BatteryFuse]();

                    ControlNotification.Singleton.Error(
                        NotificationPriority.JustOne,
                        _cmsoCcuUpsBatteryFuse,
                        GetString("ErrorOutputUsedLikeActuators"),
                        ControlNotificationSettings.Default);

                    _cmsoCcuUpsBatteryFuse.Focus();
                    return false;
                }
            }
            if (!_catsCcuUpsBatteryFuse.ControlValues(
                control =>
                    _openAndScrollToControlByAlarmType[AlarmType.Ccu_Ups_BatteryFuse]()))
            {
                return false;
            }
            if (_cmsoCcuUpsOvertemperature.SpecialOutput != null)
            {
                if (Plugin.MainServerProvider.DoorEnvironments.IsOutputInDoorEnvironmentsActuators(_cmsoCcuUpsOvertemperature.SpecialOutput))
                {
                    _openAndScrollToControlByAlarmType[AlarmType.Ccu_Ups_Overtemperature]();

                    ControlNotification.Singleton.Error(
                        NotificationPriority.JustOne,
                        _cmsoCcuUpsOvertemperature,
                        GetString("ErrorOutputUsedLikeActuators"),
                        ControlNotificationSettings.Default);

                    _cmsoCcuUpsOvertemperature.Focus();
                    return false;
                }
            }
            if (!_catsCcuUpsOvertemperature.ControlValues(
                control =>
                    _openAndScrollToControlByAlarmType[AlarmType.Ccu_Ups_Overtemperature]()))
            {
                return false;
            }
            if (_cmsoCcuUpsTamper.SpecialOutput != null)
            {
                if (Plugin.MainServerProvider.DoorEnvironments.IsOutputInDoorEnvironmentsActuators(_cmsoCcuUpsTamper.SpecialOutput))
                {
                    _openAndScrollToControlByAlarmType[AlarmType.Ccu_Ups_TamperSabotage]();

                    ControlNotification.Singleton.Error(
                        NotificationPriority.JustOne,
                        _cmsoCcuUpsTamper,
                        GetString("ErrorOutputUsedLikeActuators"),
                        ControlNotificationSettings.Default);

                    _cmsoCcuUpsTamper.Focus();
                    return false;
                }
            }
            if (!_catsCcuUpsTamper.ControlValues(
                control =>
                    _openAndScrollToControlByAlarmType[AlarmType.Ccu_Ups_TamperSabotage]()))
            {
                return false;
            }
            if (_cmsoCcuFuseOnExtensionBoard.SpecialOutput != null)
            {
                if (Plugin.MainServerProvider.DoorEnvironments.IsOutputInDoorEnvironmentsActuators(_cmsoCcuFuseOnExtensionBoard.SpecialOutput))
                {
                    _openAndScrollToControlByAlarmType[AlarmType.CCU_ExtFuse]();

                    ControlNotification.Singleton.Error(
                        NotificationPriority.JustOne,
                        _cmsoCcuFuseOnExtensionBoard,
                        GetString("ErrorOutputUsedLikeActuators"),
                        ControlNotificationSettings.Default);

                    _cmsoCcuFuseOnExtensionBoard.Focus();
                    return false;
                }
            }
            if (!_catsCcuFuseOnExtensionBoard.ControlValues(
                control =>
                    _openAndScrollToControlByAlarmType[AlarmType.CCU_ExtFuse]()))
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
            if (_showOption == ShowOptionsEditForm.Edit && !SaveValuesForCardReaders())
                return;

            ConsiderMaxNodeLookupUpdate();
            SetSecurityLevelSettingsForCR();
            Ok_Click();
        }

        private void ConsiderMaxNodeLookupUpdate()
        {
            if (_eMaximumExpectedDCUCount.Value != _editingObject.MaxNodeLookupSequence)
            {
                if (_mbt == MainBoardVariant.CCU40 ||
                    _mbt == MainBoardVariant.CCU12 ||
                    _mbt == MainBoardVariant.CCU05 ||
                    _mbt == MainBoardVariant.CAT12CE)
                {
                    _editingObject.MaxNodeLookupSequence = (byte) _eMaximumExpectedDCUCount.Value;
                    _eMaximumExpectedDCUCount_ValueChanged(null, null);
                }
            }
        }

        protected override bool SaveToDatabaseInsert()
        {
            Exception error;
            var retValue = Plugin.MainServerProvider.CCUs.Insert(ref _editingObject, out error);
            if (!retValue && error != null)
            {
                if (error is SqlUniqueException)
                {
                    if (error.Message.Contains("Name"))
                    {
                        ControlNotification.Singleton.Error(NotificationPriority.JustOne, _eName,
                            GetString("ErrorUsedCCUName"), ControlNotificationSettings.Default);
                        _eName.Focus();
                    }
                    else if (error.Message == CCU.COLUMNIPADDRESS)
                    {
                        ControlNotification.Singleton.Error(NotificationPriority.JustOne, _eIPAddress,
                            GetString("ErrorUsedIPAddress"), ControlNotificationSettings.Default);
                        _eIPAddress.Focus();
                    }
                    else if (error.Message == CCU.COLUMNINDEXCCU)
                    {
                        ControlNotification.Singleton.Error(NotificationPriority.JustOne, _eIndex,
                            GetString("ErrorUsedCcuIndex"), CgpClient.Singleton.ClientControlNotificationSettings);
                        _eIndex.Focus();
                    }
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
            try
            {
                SaveWatchdogOutput();
            }
            catch
            {
                return false;
            }

            Exception error;
            bool retValue =
                OnlyInDatabase
                    ? Plugin.MainServerProvider.CCUs.UpdateOnlyInDatabase(_editingObject, out error)
                    : Plugin.MainServerProvider.CCUs.Update(_editingObject, out error);

            if (!retValue && error != null)
            {
                if (error is SqlUniqueException)
                {
                    if (error.Message.Contains("Name"))
                    {
                        ControlNotification.Singleton.Error(NotificationPriority.JustOne, _eName,
                            GetString("ErrorUsedCCUName"), ControlNotificationSettings.Default);
                        _eName.Focus();
                    }
                    else if (error.Message == CCU.COLUMNIPADDRESS)
                    {
                        ControlNotification.Singleton.Error(NotificationPriority.JustOne, _eIPAddress,
                            GetString("ErrorUsedIPAddress"), ControlNotificationSettings.Default);
                        _eIPAddress.Focus();
                    }
                    else if (error.Message == CCU.COLUMNINDEXCCU)
                    {
                        ControlNotification.Singleton.Error(NotificationPriority.JustOne, _eIndex,
                            GetString("ErrorUsedCcuIndex"), CgpClient.Singleton.ClientControlNotificationSettings);
                        _eIndex.Focus();
                    }
                }
                else
                    throw error;
            }

            return retValue;
        }

        private void SaveWatchdogOutput()
        {
            if (_editingObject.WatchdogOutput == null)
            {
                UnsetOtherOutputsFromWatchdog(null);
                return;
            }

            Exception error;

            Output saveWatchdogOutput =
                Plugin.MainServerProvider.Outputs.GetObjectForEdit(
                    _editingObject.WatchdogOutput.IdOutput,
                    out error);

            if (saveWatchdogOutput == null)
            {
                if (error != null)
                    throw error;

                throw new Exception();
            }

            saveWatchdogOutput.ControlType = (byte) OutputControl.watchdog;

            var success = Plugin.MainServerProvider.Outputs.Update(saveWatchdogOutput, out error);
            Plugin.MainServerProvider.Outputs.EditEnd(saveWatchdogOutput);

            if (!success)
            {
                if (error != null)
                    throw error;

                throw new Exception();
            }
            UnsetOtherOutputsFromWatchdog(saveWatchdogOutput);
        }

        /// <summary>
        /// Ensures no other output will be edited as watchdog output
        /// </summary>
        /// <param name="watchdogOutput"></param>
        private void UnsetOtherOutputsFromWatchdog(Output watchdogOutput)
        {
            Exception ex;
            Output editOutput;

            if (CgpClient.Singleton.IsConnectionLost(true))
                return;

            //CCU40 case
            foreach (var output in _editingObject.Outputs)
            {
                if (watchdogOutput != null && output.IdOutput == watchdogOutput.IdOutput)
                    continue;

                //if watchdog is not set or is different
                if ((OutputControl) output.ControlType == OutputControl.watchdog &&
                    (watchdogOutput == null || output.IdOutput != watchdogOutput.IdOutput))
                {
                    editOutput = Plugin.MainServerProvider.Outputs.GetObjectForEdit(output.IdOutput, out ex);
                    if (ex == null && editOutput != null)
                    {
                        editOutput.ControlType = (byte) OutputControl.unblocked;
                        Plugin.MainServerProvider.Outputs.Update(editOutput, out ex);
                        Plugin.MainServerProvider.Outputs.EditEnd(editOutput);
                    }
                }
            }

            //CCU0 case
            foreach (var dcu in _editingObject.DCUs)
            {
                var fullDcu = Plugin.MainServerProvider.DCUs.GetObjectForEdit(dcu.IdDCU, out ex);
                if ((ex == null) && (fullDcu != null))
                {
                    foreach (var output in fullDcu.Outputs)
                    {
                        //if watchdog is not set or is different
                        if ((OutputControl) output.ControlType == OutputControl.watchdog &&
                            (watchdogOutput == null || output.IdOutput != watchdogOutput.IdOutput))
                        {
                            editOutput = Plugin.MainServerProvider.Outputs.GetObjectForEdit(output.IdOutput, out ex);
                            if (ex == null && editOutput != null)
                            {
                                editOutput.ControlType = (byte) OutputControl.unblocked;
                                Plugin.MainServerProvider.Outputs.Update(editOutput, out ex);
                                Plugin.MainServerProvider.Outputs.EditEnd(editOutput);
                            }
                        }
                    }
                }
            }
        }

        private void UnconfigureWatchdogOutput()
        {
            if (_editingObject.WatchdogOutput == null)
                return;

            Exception error;

            Output saveWatchdogOutput =
                Plugin.MainServerProvider.Outputs.GetObjectForEdit(_editingObject.WatchdogOutput.IdOutput, out error);

            if (saveWatchdogOutput == null)
            {
                if (error != null)
                    throw error;

                throw new Exception();
            }

            saveWatchdogOutput.ControlType = (byte) OutputControl.unblocked;

            Plugin.MainServerProvider.Outputs.Update(saveWatchdogOutput, out error);
            Plugin.MainServerProvider.Outputs.EditEnd(saveWatchdogOutput);
            _cmsoCcuOffline.SpecialOutput = null;

            var ccu = Plugin.MainServerProvider.CCUs.GetObjectForEdit(_editingObject.IdCCU, out error);
            ccu.WatchdogOutput = null;
            Plugin.MainServerProvider.CCUs.Update(ccu, out error);
            Plugin.MainServerProvider.CCUs.EditEnd(ccu);
            //_editingObject = (this.Plugin as NCASClient).MainServerProvider.CCUs.GetObjectForEdit(_editingObject.IdCCU, out error);
        }

        protected override void EditTextChanger(object sender, EventArgs e)
        {
            base.EditTextChanger(sender, e);

            if (!Insert)
                _bApply.Enabled = true;
        }

        protected override void EditTextChangerOnlyInDatabase(object sender, EventArgs e)
        {
            base.EditTextChangerOnlyInDatabase(sender, e);

            if (!Insert)
                _bApply.Enabled = true;
        }

        protected override void EditEnd()
        {
            if (Plugin.MainServerProvider != null && Plugin.MainServerProvider.CCUs != null)
                Plugin.MainServerProvider.CCUs.EditEnd(_editingObject);
        }

        private void FillTimeZone()
        {
            tzCollection = TimeZoneInfo.GetSystemTimeZones();
            foreach (var timeZone in tzCollection)
            {
                _cbTimeZone.Items.Add(timeZone);
            }

            if (_editingObject.TimeZoneInfo != null)
            {
                for (var i = 0; i < _cbTimeZone.Items.Count; i++)
                {
                    var tzi = (TimeZoneInfo) _cbTimeZone.Items[i];
                    if (tzi.Id == _editingObject.TimeZoneInfo)
                    {
                        _cbTimeZone.SelectedItem = tzi;
                        return;
                    }
                }
            }
        }

        private LwRemotingClient _lwc;

        private void _bTest_Click(object sender, EventArgs e)
        {
            if (_eIPAddress.Text.Length == 0)
                return;

            try
            {
                if (null == _lwc)
                {
                    var aes = new AESSettings(
                        LwRemotingGlobals.GetSHA1(LwRemotingGlobals.LWREMOTING_KEY),
                        LwRemotingGlobals.LWREMOTING_SALT);

                    _lwc = new LwRemotingClient(aes, _eIPAddress.Text, 63002);
                    _lwc.Start();
                }

                var o = _lwc.Call("DummyMethod", DateTime.Now.ToString(CultureInfo.InvariantCulture));
                Dialog.Info("Returned :\n" + (o ?? "NULL"));
            }
            catch (Exception err)
            {
                Dialog.Error(err);
            }


        }

        private void _dgvInputCCU_DoubleClick(object sender, EventArgs e)
        {
            if (CgpClient.Singleton.IsConnectionLost(false)) return;
            if (_bindingSourceInput == null || _bindingSourceInput.Count == 0) return;
            var input = (Input) _bindingSourceInput.List[_bindingSourceInput.Position];
            input = Plugin.MainServerProvider.Inputs.GetObjectById(input.IdInput);
            NCASInputsForm.Singleton.OpenEditForm(input);
        }

        private void _dgvOutputCCU_DoubleClick(object sender, EventArgs e)
        {
            if (CgpClient.Singleton.IsConnectionLost(false)) return;
            if (_bindingSourceOutput == null || _bindingSourceOutput.Count == 0) return;
            var output = (Output) _bindingSourceOutput.List[_bindingSourceOutput.Position];
            output = Plugin.MainServerProvider.Outputs.GetObjectById(output.IdOutput);
            NCASOutputsForm.Singleton.OpenEditForm(output);
        }

        private void ShowMyInput()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DVoid2Void(ShowMyInput));
            }
            else
            {
                if (!_tcCCU.Contains(_tpInputOutput))
                    return;

                if (_editingObject == null) return;
                if (_editingObject.Inputs == null) return;
                if (_editingObject.Inputs.Count == 0) return;

                var lastPosition = 0;
                if (_bindingSourceInput != null && _bindingSourceInput.Count != 0)
                    lastPosition = _bindingSourceInput.Position;
                _bindingSourceInput = new BindingSource();
                _bindingSourceInput.DataSource = _editingObject.Inputs;
                _bindingSourceInput.AllowNew = false;
                if (lastPosition != 0) _bindingSourceInput.Position = lastPosition;

                _dgvInputCCU.DataSource = _bindingSourceInput;

                if (!_dgvInputCCU.Columns.Contains(Input.COLUMNSTATUSONOFF))
                {
                    _dgvInputCCU.Columns.Add(Input.COLUMNSTATUSONOFF, Input.COLUMNSTATUSONOFF);
                    _dgvInputCCU.Columns[Input.COLUMNSTATUSONOFF].DisplayIndex =
                        _dgvInputCCU.Columns[Input.COLUMNNICKNAME].DisplayIndex;
                    _dgvInputCCU.Columns[Input.COLUMNSTATUSONOFF].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                }

                foreach (DataGridViewRow row in _dgvInputCCU.Rows)
                {
                    row.Cells[Input.COLUMNSTATUSONOFF].Value = "";
                }

                SafeThread.StartThread(SetInputToDgvValues);

                HideColumnDgw(_dgvInputCCU, Input.COLUMNIDIMPUT);
                HideColumnDgw(_dgvInputCCU, Input.COLUMNNICKNAME);
                HideColumnDgw(_dgvInputCCU, Input.COLUMNINPUTTYPE);
                HideColumnDgw(_dgvInputCCU, Input.COLUMNINVERTED);
                HideColumnDgw(_dgvInputCCU, Input.COLUMNHIGHPRIORITY);
                HideColumnDgw(_dgvInputCCU, Input.COLUMNOFFACK);
                HideColumnDgw(_dgvInputCCU, Input.COLUMNDELAYTOON);
                HideColumnDgw(_dgvInputCCU, Input.COLUMNDELAYTOOFF);
                HideColumnDgw(_dgvInputCCU, Input.COLUMNALARMPRESENTATIONGROUP);
                HideColumnDgw(_dgvInputCCU, Input.COLUMNALARMPGPRESENTATIONSTATEINPENDENTOFALARM);
                HideColumnDgw(_dgvInputCCU, Input.COLUMNTAMPERALARMPRESENTATIONGROUP);
                HideColumnDgw(_dgvInputCCU, Input.COLUMNTAMPERDELAYTOON);
                HideColumnDgw(_dgvInputCCU, Input.COLUMNDCUSDCU);
                HideColumnDgw(_dgvInputCCU, Input.COLUMNCCU);
                HideColumnDgw(_dgvInputCCU, Input.COLUMNGUIDCCU);
                HideColumnDgw(_dgvInputCCU, Input.COLUMNGUIDDCU);
                HideColumnDgw(_dgvInputCCU, Input.COLUMNALAAINPUTS);
                HideColumnDgw(_dgvInputCCU, Input.COLUMNINPUTNUMBER);
                HideColumnDgw(_dgvInputCCU, Input.COLUMNSTATE);
                HideColumnDgw(_dgvInputCCU, Input.COLUMNFILTERALARMON);
                HideColumnDgw(_dgvInputCCU, Input.COLUMNFILTERALARMTAMPER);
                HideColumnDgw(_dgvInputCCU, Input.COLUMNCKUNIQUE);
                HideColumnDgw(_dgvInputCCU, Input.COLUMNENABLEPARENTINFULLNAME);
                HideColumnDgw(_dgvInputCCU, Input.COLUMNOBJECTTYPE);
                HideColumnDgw(_dgvInputCCU, Input.COLUMNBLOCKINGTYPE);
                HideColumnDgw(_dgvInputCCU, Input.COLUMNONOFFOBJECTTYPE);
                HideColumnDgw(_dgvInputCCU, Input.COLUMNONOFFOBJECTOBJECTTYPE);
                HideColumnDgw(_dgvInputCCU, Input.COLUMNONOFFOBJECTID);
                HideColumnDgw(_dgvInputCCU, Input.COLUMNONOFFOBJECT);
                HideColumnDgw(_dgvInputCCU, Input.COLUMNISSETPRESENTATIONGROUP);
                HideColumnDgw(_dgvInputCCU, Input.COLUMNLOCALALARMINSTRUCTION);
                HideColumnDgw(_dgvInputCCU, Input.COLUMN_ALTERNATE_NAME);
                HideColumnDgw(_dgvInputCCU, Input.COLUMN_FULLNAME);
                HideColumnDgw(_dgvInputCCU, Input.COLUMN_FULL_TEXT_SEARCH_STRING);
                HideColumnDgw(_dgvInputCCU, Input.COLUMN_OTHER_FULL_TEXT_SEARCH_STRINGS);
                HideColumnDgw(_dgvInputCCU, Input.ColumnVersion);

                if (_dgvInputCCU.Columns.Contains(Input.COLUMNDESCRIPTION))
                    _dgvInputCCU.Columns[Input.COLUMNDESCRIPTION].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                _dgvInputCCU.AutoGenerateColumns = false;
                _dgvInputCCU.AllowUserToAddRows = false;

                LocalizationHelper.TranslateDataGridViewColumnsHeaders(_dgvInputCCU);
            }
        }

        private void ShowMyOutput()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DVoid2Void(ShowMyOutput));
            }
            else
            {
                if (!_tcCCU.Contains(_tpInputOutput))
                    return;

                if (_editingObject.Outputs == null) return;
                if (_editingObject.Outputs.Count == 0) return;

                var lastPosition = 0;
                if (_bindingSourceOutput != null && _bindingSourceOutput.Count != 0)
                    lastPosition = _bindingSourceOutput.Position;
                _bindingSourceOutput =
                    new BindingSource
                    {
                        DataSource = _editingObject.Outputs,
                        AllowNew = false
                    };
                if (lastPosition != 0) _bindingSourceOutput.Position = lastPosition;

                _dgvOutputCCU.DataSource = _bindingSourceOutput;

                if (!_dgvOutputCCU.Columns.Contains(Output.COLUMNSTATUSONOFF))
                {
                    _dgvOutputCCU.Columns.Add(Output.COLUMNSTATUSONOFF, Output.COLUMNSTATUSONOFF);
                    _dgvOutputCCU.Columns[Output.COLUMNSTATUSONOFF].DisplayIndex =
                        _dgvOutputCCU.Columns[Output.COLUMNNAME].DisplayIndex + 1;
                    _dgvOutputCCU.Columns[Output.COLUMNSTATUSONOFF].AutoSizeMode =
                        DataGridViewAutoSizeColumnMode.AllCells;
                }
                if (!_dgvOutputCCU.Columns.Contains(Output.COLUMNREALSTATUSONOFF))
                {
                    _dgvOutputCCU.Columns.Add(Output.COLUMNREALSTATUSONOFF, Output.COLUMNREALSTATUSONOFF);
                    _dgvOutputCCU.Columns[Output.COLUMNREALSTATUSONOFF].DisplayIndex =
                        _dgvOutputCCU.Columns[Output.COLUMNSTATUSONOFF].DisplayIndex + 1;
                    _dgvOutputCCU.Columns[Output.COLUMNREALSTATUSONOFF].AutoSizeMode =
                        DataGridViewAutoSizeColumnMode.AllCells;
                }

                foreach (DataGridViewRow row in _dgvOutputCCU.Rows)
                {
                    row.Cells[Output.COLUMNSTATUSONOFF].Value = "";
                    row.Cells[Output.COLUMNREALSTATUSONOFF].Value = "";
                }

                SafeThread.StartThread(SetOutputToDgvValues);

                HideColumnDgw(_dgvOutputCCU, Output.COLUMNIDOUTPUT);
                HideColumnDgw(_dgvOutputCCU, Output.COLUMNOUTPUTTYPE);
                HideColumnDgw(_dgvOutputCCU, Output.COLUMNSETTINGSFORCEDTOOFF);
                HideColumnDgw(_dgvOutputCCU, Output.COLUMNSETTINGSDELAYTOON);
                HideColumnDgw(_dgvOutputCCU, Output.COLUMNSETTINGSDELAYTOOFF);
                HideColumnDgw(_dgvOutputCCU, Output.COLUMNSETTINGSPULSELENGTH);
                HideColumnDgw(_dgvOutputCCU, Output.COLUMNSETTINGSPULSEDELAY);
                HideColumnDgw(_dgvOutputCCU, Output.COLUMNDELAYTOON);
                HideColumnDgw(_dgvOutputCCU, Output.COLUMNDELAYTOOFF);
                HideColumnDgw(_dgvOutputCCU, Output.COLUMNINVERTED);
                HideColumnDgw(_dgvOutputCCU, Output.COLUMNALARMPRESENTATIONGROUP);
                HideColumnDgw(_dgvOutputCCU, Output.COLUMNDCU);
                HideColumnDgw(_dgvOutputCCU, Output.COLUMNCCU);
                HideColumnDgw(_dgvOutputCCU, Output.COLUMNGUIDCCU);
                HideColumnDgw(_dgvOutputCCU, Output.COLUMNGUIDDCU);
                HideColumnDgw(_dgvOutputCCU, Output.COLUMNONOFFOBJECTTYPE);
                HideColumnDgw(_dgvOutputCCU, Output.COLUMNONOFFOBJECTOBJECTTYPE);
                HideColumnDgw(_dgvOutputCCU, Output.COLUMNONOFFOBJECTID);
                HideColumnDgw(_dgvOutputCCU, Output.COLUMNONOFFOBJECT);
                HideColumnDgw(_dgvOutputCCU, Output.COLUMNCONTROLTYPE);
                HideColumnDgw(_dgvOutputCCU, Output.COLUMNOUTPUTNUMBER);
                HideColumnDgw(_dgvOutputCCU, Output.COLUMNSTATE);
                HideColumnDgw(_dgvOutputCCU, Output.COLUMNALARMCONROLBYOBJON);
                HideColumnDgw(_dgvOutputCCU, Output.REALSTATECHANGES);
                HideColumnDgw(_dgvOutputCCU, Output.COLUMNCKUNIQUE);
                HideColumnDgw(_dgvOutputCCU, Output.COLUMNENABLEPARENTINFULLNAME);
                HideColumnDgw(_dgvOutputCCU, Output.COLUMNOBJECTTYPE);
                HideColumnDgw(_dgvOutputCCU, Output.COLUMNLOCALALARMINSTRUCTION);
                HideColumnDgw(_dgvOutputCCU, Output.ColumnVersion);

                if (_dgvOutputCCU.Columns.Contains(Output.COLUMNDESCRIPTION))
                    _dgvOutputCCU.Columns[Output.COLUMNDESCRIPTION].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                _dgvOutputCCU.AutoGenerateColumns = false;
                _dgvOutputCCU.AllowUserToAddRows = false;

                LocalizationHelper.TranslateDataGridViewColumnsHeaders(_dgvOutputCCU);
            }
        }

        private MainBoardVariant GetMainBoardVariant(byte? variant)
        {
            return variant == null || variant.Value == 255
                ? MainBoardVariant.Unknown
                : (MainBoardVariant) variant.Value;
        }

        private void ShowCardReaders()
        {
            try
            {
                if (InvokeRequired)
                {
                    BeginInvoke(new DVoid2Void(ShowCardReaders));
                }
                else
                {
                    _dgCardReaders.RowsDefaultCellStyle.BackColor = Color.White;
                    _dgCardReaders.AlternatingRowsDefaultCellStyle.BackColor = Color.WhiteSmoke;
                    _dgCardReaders.CellBorderStyle = DataGridViewCellBorderStyle.None;

                    _bCrMarkAsDead.Enabled = false;
                    if (_editingObject == null)
                    {
                        HideDGCardReaders();
                        return;
                    }

                    MainBoardVariant ccuMainBoardVariant = GetMainBoardVariant(_editingObject.CcuMainboardType);

                    if ((_isCCU0 && _editingObject.EnabledComPort == false)
                        || (ccuMainBoardVariant == MainBoardVariant.CCU0_ECHELON
                            || ccuMainBoardVariant == MainBoardVariant.CCU0_RS485))
                        HideDGCardReaders();
                    else
                        ShowDGCardReaders();

                    if (_editingObject.CardReaders == null || _editingObject.CardReaders.Count == 0)
                    {
                        _bindingSourceCardReaders = new BindingSource();
                        _dgCardReaders.DataSource = _bindingSourceCardReaders;

                        return;
                    }

                    var lastPosition = 0;
                    if (_bindingSourceCardReaders != null && _bindingSourceCardReaders.Count != 0)
                        lastPosition = _bindingSourceCardReaders.Position;
                    _bindingSourceCardReaders =
                        new BindingSource
                        {
                            DataSource =
                                _editingObject.CardReaders
                                    .OrderBy(cardReader => cardReader.ToString())
                                    .ToList(),
                            AllowNew = false
                        };

                    if (lastPosition != 0) _bindingSourceCardReaders.Position = lastPosition;

                    _dgCardReaders.DataSource = _bindingSourceCardReaders;

                    if (!_dgCardReaders.Columns.Contains(CardReaderShort.COLUMN_SYMBOL))
                    {
                        _dgCardReaders.Columns.Insert(0, new DataGridViewImageColumn() { Name = CardReaderShort.COLUMN_SYMBOL });
                        _dgCardReaders.Columns[CardReaderShort.COLUMN_SYMBOL].AutoSizeMode =
                            DataGridViewAutoSizeColumnMode.AllCells;
                    }

                    if (!_dgCardReaders.Columns.Contains(CardReader.COLUMNONLINESTATE))
                    {
                        _dgCardReaders.Columns.Add(CardReader.COLUMNONLINESTATE, CardReader.COLUMNONLINESTATE);
                        _dgCardReaders.Columns[CardReader.COLUMNONLINESTATE].AutoSizeMode =
                            DataGridViewAutoSizeColumnMode.AllCells;
                    }

                    if (!_dgCardReaders.Columns.Contains(CardReader.COLUMNUSED))
                    {
                        _dgCardReaders.Columns.Add(CardReader.COLUMNUSED, CardReader.COLUMNUSED);
                        _dgCardReaders.Columns[CardReader.COLUMNUSED].AutoSizeMode =
                            DataGridViewAutoSizeColumnMode.AllCells;
                    }

                    HideColumnDgw(_dgCardReaders, CardReader.COLUMNIDCARDREADER);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMNGIN);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMNGINLENGTH);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMNSECURITYLEVEL);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMNISEMERGENCYCODE);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMNEMERGENCYCODE);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMNEMERGENCYCODELENGTH);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMNISFORCEDSECURITYLEVEL);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMNFORCEDSECURITYLEVEL);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMNSECURITYDAILYPLAN);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMNGUIDSECURITYDAILYPLAN);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMNSECURITYTIMEZONE);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMNGUIDSECURITYTIMEZONE);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMNDCU);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMNGUIDDCU);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMNDESCRIPTION);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMNAACARDREADERS);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMNONOFFOBJECTOBJECTTYPE);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMNONOFFOBJECTID);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMNONOFFOBJECT);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMNCCU);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMNPORT);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMNADDRESS);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMNGUIDCCU);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMNONOFFOBJECTTYPE);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMNALARMACCESSDENIED);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMN_BLOCK_ALARM_ACCESS_DENIED);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMN_OBJ_BLOCK_ALARM_ACCESS_DENIED_OBJECT_TYPE);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMN_OBJ_BLOCK_ALARM_ACCESS_DENIED_ID);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMN_OBJ_BLOCK_ALARM_ACCESS_DENIED);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMN_EVENTLOG_DURING_BLOCK_ALARM_ACCESS_DENIED);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMNALARMUNKNOWNCARD);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMN_BLOCK_ALARM_UNKNOWN_CARD);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMN_OBJ_BLOCK_ALARM_UNKNOWN_CARD_OBJECT_TYPE);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMN_OBJ_BLOCK_ALARM_UNKNOWN_CARD_ID);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMN_OBJ_BLOCK_ALARM_UNKNOWN_CARD);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMN_EVENTLOG_DURING_BLOCK_ALARM_UNKNOWN_CARD);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMNALARMCARDBLOCKEDORINACTIVE);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMN_BLOCK_ALARM_CARD_BLOCKED_OR_INACTIVE);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMN_OBJ_BLOCK_ALARM_CARD_BLOCKED_OR_INACTIVE_OBJECT_TYPE);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMN_OBJ_BLOCK_ALARM_CARD_BLOCKED_OR_INACTIVE_ID);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMN_OBJ_BLOCK_ALARM_CARD_BLOCKED_OR_INACTIVE);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMN_EVENTLOG_DURING_BLOCK_ALARM_CARD_BLOCKED_OR_INACTIVE);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMNALARMINVALIDPIN);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMN_BLOCK_ALARM_INVALID_PIN);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMN_OBJ_BLOCK_ALARM_INVALID_PIN_OBJECT_TYPE);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMN_OBJ_BLOCK_ALARM_INVALID_PIN_ID);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMN_OBJ_BLOCK_ALARM_INVALID_PIN);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMN_EVENTLOG_DURING_BLOCK_ALARM_INVALID_PIN);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMNALARMINVALIDGIN);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMN_BLOCK_ALARM_INVALID_GIN);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMN_OBJ_BLOCK_ALARM_INVALID_GIN_OBJECT_TYPE);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMN_OBJ_BLOCK_ALARM_INVALID_GIN_ID);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMN_OBJ_BLOCK_ALARM_INVALID_GIN);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMN_EVENTLOG_DURING_BLOCK_ALARM_INVALID_GIN);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMNALARMINVALIDEMERGENCYCODE);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMN_BLOCK_ALARM_INVALID_EMERGENCY_CODE);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMN_OBJ_BLOCK_ALARM_INVALID_EMERGENCY_CODE_OBJECT_TYPE);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMN_OBJ_BLOCK_ALARM_INVALID_EMERGENCY_CODE_ID);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMN_OBJ_BLOCK_ALARM_INVALID_EMERGENCY_CODE);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMN_EVENTLOG_DURING_BLOCK_ALARM_INVALID_EMERGENCY_CODE);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMNALARMACCESSPERMITTED);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMN_BLOCK_ALARM_ACCESS_PERMITTED);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMN_OBJ_BLOCK_ALARM_ACCESS_PERMITTED_OBJECT_TYPE);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMN_OBJ_BLOCK_ALARM_ACCESS_PERMITTED_ID);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMN_OBJ_BLOCK_ALARM_ACCESS_PERMITTED);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMN_EVENTLOG_DURING_BLOCK_ALARM_ACCESS_PERMITTED);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMNCRLANGUAGE);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMNCARDAPPLIEDLED);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMNCARDAPPLIEDKEYBOARDLIGHT);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMNCARDAPPLIEDINERNALBUZZER);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMNCARDAPPLIEDEXTERNALBUZER);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMNTAMPERLED);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMNTAMPERKEYBOARDLIGHT);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMNTAMPERINERNALBUZZER);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMNTAMPEREXTERNALBUZER);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMNRESETLED);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMNRESETKEYBOARDLIGHT);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMNRESETINERNALBUZZER);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMNRESETEXTERNALBUZER);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMNKEYPRESSEDLED);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMNKEYPRESSEDKEYBOARDLIGHT);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMNKEYPRESSEDINERNALBUZZER);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMNKEYPRESSEDEXTERNALBUZER);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMNINTERNALBUZERKILLSWITCH);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMNCARDREADERHARDWARE);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMNSLFORENTERTOMENU);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMN_USE_ACCESS_GIN_FOR_ENTER_TO_MENU);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMN_GIN_FOR_ENTER_TO_MENU);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMN_GIN_LENGTH_FOR_ENTER_TO_MENU);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMN_SECURITY_DAILY_PLAN_FOR_ENTER_TO_MENU);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMN_GUID_SSECURITY_DAILY_PLAN_FOR_ENTER_TO_MENU);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMN_SECURITY_TIME_ZONE_FOR_ENTER_TO_MENU);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMN_GUID_SECURITY_TIME_ZONE_FOR_ENTER_TO_MENU);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMNOBJECTTYPE);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMNALARMOFFLINE);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMN_BLOCK_ALARM_OFFLINE);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMN_OBJ_BLOCK_ALARM_OFFLINE_OBJECT_TYPE);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMN_OBJ_BLOCK_ALARM_OFFLINE_ID);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMN_OBJ_BLOCK_ALARM_OFFLINE);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMN_EVENTLOG_DURING_BLOCK_ALARM_OFFLINE);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMNALARMTAMPER);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMN_BLOCK_ALARM_TAMPER);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMN_OBJ_BLOCK_ALARM_TAMPER_OBJECT_TYPE);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMN_OBJ_BLOCK_ALARM_TAMPER_ID);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMN_OBJ_BLOCK_ALARM_TAMPER);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMN_EVENTLOG_DURING_BLOCK_ALARM_TAMPER);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMNCKUNIQUE);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMNENABLEPARENTINFULLNAME);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMNQUERYDBSTAMP);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMNSPECIALOUTPUTFORTAMPER);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMNGUIDSPECIALOUTPUTFORTAMPER);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMNSPECIALOUTPUTFOROFFLINE);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMNGUIDSPECIALOUTPUTFOROFFLINE);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMNLOCALALARMINSTRUCTION);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMNFUNCTIONKEY1);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMNFUNCTIONKEY2);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMN_INVALID_GIN_RETRIES_LIMIT_ENABLED);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMN_INVALID_PIN_RETRIES_LIMIT_ENABLED);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMN_ALARM_INVALID_GIN_RETRIES_LIMIT_REACHED);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMN_BLOCK_ALARM_INVALID_GIN_RETRIES_LIMIT_REACHED);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMN_OBJ_BLOCK_ALARM_INVALID_GIN_RETRIES_LIMIT_REACHED_OBJECT_TYPE);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMN_OBJ_BLOCK_ALARM_INVALID_GIN_RETRIES_LIMIT_REACHED_ID);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMN_CARD_READER_ALARM_ARCS);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMN_DISABLE_SCREENSAVER);
                    HideColumnDgw(_dgCardReaders, CardReader.COLUMN_REPORT_EVENTS_TO_TIMETEC);
                    HideColumnDgw(_dgCardReaders, CardReader.ColumnVersion);

                    SafeThread.StartThread(SetCRDgValues);

                    if (_dgCardReaders.Columns.Contains(CardReader.COLUMNNAME))
                        _dgCardReaders.Columns[CardReader.COLUMNNAME].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                    _dgCardReaders.AutoGenerateColumns = false;
                    _dgCardReaders.AllowUserToAddRows = false;

                    LocalizationHelper.TranslateDataGridViewColumnsHeaders(_dgCardReaders);
                }
            }
            catch
            {
            }
        }

        private void SetCRDgValues()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DVoid2Void(SetCRDgValues));
            }
            else
            {
                if (_dgCardReaders.DataSource == null) 
                    return;

                if (CgpClient.Singleton.IsConnectionLost(false)) 
                    return;

                var images = ((ICgpVisualPlugin)NCASClient.Singleton).GetPluginObjectsImages();

                foreach (DataGridViewRow row in _dgCardReaders.Rows)
                {
                    var cardReader = (CardReader) _bindingSourceCardReaders.List[row.Index];

                    OnlineState crOnlineState =
                        cardReader != null
                            ? Plugin.MainServerProvider.CardReaders.GetOnlineStates(cardReader.IdCardReader)
                            : OnlineState.Unknown;

                    if (crOnlineState == OnlineState.Online)
                    {
                        row.Cells[CardReader.COLUMNONLINESTATE].Value = GetString("Online");
                    }
                    else if (crOnlineState == OnlineState.Offline)
                    {
                        row.Cells[CardReader.COLUMNONLINESTATE].Value = GetString("Offline");
                    }
                    else if (crOnlineState == OnlineState.Unknown)
                    {
                        row.Cells[CardReader.COLUMNONLINESTATE].Value = GetString("Unknown");
                    }

                    row.Cells[CardReader.COLUMNUSED].Value =
                        GetString(
                            Plugin.MainServerProvider.CardReaders
                                .IsUsedInDoorEnvironment(cardReader)
                                ? "Yes"
                                : "No");

                    row.Cells[0].Value =
                                images.Images[crOnlineState == OnlineState.Online
                                    ? ObjectType.CardReader.ToString()
                                    : ObjTypeHelper.CardReaderBlocked];
                }
            }
        }

        private void ShowCRsUpgrade()
        {
            IEnumerable<CardReader> cardReaders;
            if (_mbt == MainBoardVariant.CCU0_ECHELON || _mbt == MainBoardVariant.CCU0_RS485)
            {
                cardReaders = GetCRsFromCCUDCUs();
            }
            else
            {
                cardReaders = GetCRsFromCCU();
            }

            ShowCRsUpgrade(cardReaders);
        }

        private void ShowCRsUpgrade(IEnumerable<CardReader> cardReaders)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<IEnumerable<CardReader>>(ShowCRsUpgrade), cardReaders);
            }
            else
            {
                _dgvCRUpgrading.RowsDefaultCellStyle.BackColor = Color.White;
                _dgvCRUpgrading.AlternatingRowsDefaultCellStyle.BackColor = Color.WhiteSmoke;
                _dgvCRUpgrading.CellBorderStyle = DataGridViewCellBorderStyle.None;

                if (_editingObject == null || _mbt == MainBoardVariant.Unknown) return;
                if ((_mbt == MainBoardVariant.CCU0_ECHELON || _mbt == MainBoardVariant.CCU0_RS485) &&
                    (_editingObject.DCUs == null || _editingObject.DCUs.Count == 0))
                {
                    _bindingSourceCRUpgrades = new BindingSource();
                    _dgvCRUpgrading.DataSource = _bindingSourceCRUpgrades;
                    return;
                }

                var lastPosition = 0;
                if (_bindingSourceCRUpgrades != null && _bindingSourceCRUpgrades.Count != 0)
                    lastPosition = _bindingSourceCRUpgrades.Position;

                _bindingSourceCRUpgrades =
                    new BindingSource
                    {
                        DataSource = cardReaders,
                        AllowNew = false
                    };

                if (lastPosition != 0) _bindingSourceCRUpgrades.Position = lastPosition;

                _dgvCRUpgrading.DataSource = _bindingSourceCRUpgrades;

                /*if (!_dgvCRUpgrading.Columns.Contains(CardReader.COLUMNIDCARDREADER))
                    _dgvCRUpgrading.Columns.Add(CardReader.COLUMNIDCARDREADER, CardReader.COLUMNIDCARDREADER);

                if (!_dgvCRUpgrading.Columns.Contains(CardReader.COLUMNDCU))
                    _dgvCRUpgrading.Columns.Add(CardReader.COLUMNDCU, CardReader.COLUMNDCU);*/

                if (!_dgvCRUpgrading.Columns.Contains(CardReader.COLUMNNAME))
                {
                    _dgvCRUpgrading.Columns.Add(CardReader.COLUMNNAME, CardReader.COLUMNNAME);
                }

                if (!_dgvCRUpgrading.Columns.Contains(CardReader.COLUMNONLINESTATE))
                {
                    _dgvCRUpgrading.Columns.Add(CardReader.COLUMNONLINESTATE, CardReader.COLUMNONLINESTATE);
                }

                if (!_dgvCRUpgrading.Columns.Contains(CardReader.COLUMNUPGRADEPROGRESS))
                {
                    var column =
                        new DataGridViewProgressColumn
                        {
                            Name = CardReader.COLUMNUPGRADEPROGRESS,
                            HeaderText = CardReader.COLUMNUPGRADEPROGRESS,
                            Selected = false
                        };
                    _dgvCRUpgrading.Columns.Add(column);
                }

                if (!_dgvCRUpgrading.Columns.Contains(CardReader.COLUMNSELECTUPGRADE))
                {
                    var column =
                        new DataGridViewCheckBoxColumn
                        {
                            Name = CardReader.COLUMNSELECTUPGRADE,
                            HeaderText = CardReader.COLUMNSELECTUPGRADE
                        };
                    _dgvCRUpgrading.Columns.Add(column);
                }

                if (!_dgvCRUpgrading.Columns.Contains("HwVersion"))
                {
                    _dgvCRUpgrading.Columns.Add("HwVersion", "HwVersion");
                }

                if (!_dgvCRUpgrading.Columns.Contains("FwVersion"))
                {
                    _dgvCRUpgrading.Columns.Add("FwVersion", "FwVersion");
                }

                foreach (DataGridViewColumn column in _dgvCRUpgrading.Columns)
                {
                    if (column.Name == CardReader.COLUMNONLINESTATE ||
                        column.Name == CardReader.COLUMNUPGRADEPROGRESS)
                    {
                        column.DisplayIndex =
                            column.Name == CardReader.COLUMNONLINESTATE
                                ? 1
                                : 3;

                        column.Visible = true;
                        column.ReadOnly = true;
                        column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    }
                    else if (column.Name == CardReader.COLUMNNAME)
                    {
                        column.DisplayIndex = 0;
                        column.Visible = true;
                        column.ReadOnly = true;
                        column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    }
                    else if (column.Name == CardReader.COLUMNSELECTUPGRADE)
                    {
                        column.DisplayIndex = 4;
                        column.Visible = true;
                        column.ReadOnly = false;
                        column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    }
                    else if (column.Name == "HwVersion" || column.Name == "FwVersion")
                    {
                        column.DisplayIndex = 2;
                        column.Visible = true;
                        column.ReadOnly = true;
                        column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    }
                    else
                        column.Visible = false;
                }

                _dgvCRUpgrading.AutoGenerateColumns = true;

                _dgvCRUpgrading.AllowUserToAddRows = false;

                LocalizationHelper.TranslateDataGridViewColumnsHeaders(_dgvCRUpgrading);

                if (!_dgvCRUpgrading.Columns.Contains(CardReaderShort.COLUMN_SYMBOL))
                {
                    _dgvCRUpgrading.Columns.Insert(0,
                        new DataGridViewImageColumn() {Name = CardReaderShort.COLUMN_SYMBOL});
                    _dgvCRUpgrading.Columns[CardReaderShort.COLUMN_SYMBOL].AutoSizeMode =
                        DataGridViewAutoSizeColumnMode.AllCells;
                }
                else
                {
                    _dgvCRUpgrading.Columns[CardReaderShort.COLUMN_SYMBOL].Visible = true;
                    _dgvCRUpgrading.Columns[CardReaderShort.COLUMN_SYMBOL].DisplayIndex = 0;
                }

                ShowGridCRUpgrade();
                _dgvCRUpgrading.Refresh();
            }
        }

        private void ShowGridCRUpgrade()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DVoid2Void(ShowGridCRUpgrade));
            }
            else
            {
                if (CgpClient.Singleton.IsConnectionLost(false)) return;
                if (_bindingSourceCRUpgrades == null) return;
                if (_dgvCRUpgrading.DataSource == null) return;

                foreach (DataGridViewRow row in _dgvCRUpgrading.Rows)
                {
                    row.Cells[CardReader.COLUMNONLINESTATE].Value = "";

                    var cardReader = (CardReader) _bindingSourceCRUpgrades[row.Index];

                    cardReader.EnableParentInFullName = false;

                    row.Cells[CardReader.COLUMNNAME].Value =
                        cardReader.ToString();

                    row.Cells["HwVersion"].Value =
                        GetString(((CRHWVersion) cardReader.CardReaderHardware).ToString());

                    row.Cells["FwVersion"].Value = "";
                }

                SafeThread.StartThread(SetCRsUpgradeDgValues);
            }
        }

        private void SetCRsUpgradeDgValues()
        {
            if (CgpClient.Singleton.IsConnectionLost(false)) 
                return;

            if (InvokeRequired)
            {
                BeginInvoke(new DVoid2Void(SetCRsUpgradeDgValues));
            }
            else
            {
                if (_dgvCRUpgrading.DataSource == null) 
                    return;

                if (CgpClient.Singleton.IsConnectionLost(false)) 
                    return;

                var images = ((ICgpVisualPlugin)NCASClient.Singleton).GetPluginObjectsImages();

                foreach (DataGridViewRow row in _dgvCRUpgrading.Rows)
                {
                    var cr = (CardReader) _bindingSourceCRUpgrades.List[row.Index];

                    OnlineState onlineState =
                        cr != null
                            ? Plugin.MainServerProvider.CardReaders.GetOnlineStates(cr.IdCardReader)
                            : OnlineState.Unknown;

                    row.Cells[CardReader.COLUMNONLINESTATE].Value = GetString(onlineState.ToString());

                    if (onlineState == OnlineState.Online)
                    {
                        var firmware =
                            Plugin.MainServerProvider.CardReaders.GetFirmwareVersion(
                                _bindingSourceCRUpgrades[row.Index] as CardReader);
                        row.Cells["FwVersion"].Value = firmware;
                    }

                    row.Cells[0].Value =
                                images.Images[onlineState == OnlineState.Online
                                    ? ObjectType.CardReader.ToString()
                                    : ObjTypeHelper.CardReaderBlocked];
                }
            }
        }

        private IEnumerable<CardReader> GetCRsFromCCUDCUs()
        {
            if (_editingObject == null || _editingObject.DCUs == null || _editingObject.DCUs.Count == 0)
                return null;

            var result = new List<CardReader>();
            foreach (var dcu in _editingObject.DCUs)
            {
                DCU fullDCU = Plugin.MainServerProvider.DCUs.GetObjectById(dcu.IdDCU);
                if (fullDCU.CardReaders != null && fullDCU.CardReaders.Count > 0)
                {
                    result.AddRange(fullDCU.CardReaders);
                }
            }

            result = result.OrderBy(cr => cr.ToString()).ToList();
            return result;
        }

        private IEnumerable<CardReader> GetCRsFromCCU()
        {
            if (_editingObject == null || _editingObject.CardReaders == null || _editingObject.CardReaders.Count == 0)
                return null;

            var result = new List<CardReader>();
            result.AddRange(_editingObject.CardReaders);

            result = result.OrderBy(cr => cr.ToString()).ToList();
            return result;
        }

        private void ShowDCUs()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DVoid2Void(ShowDCUs));
            }
            else
            {
                _dgDCUs.RowsDefaultCellStyle.BackColor = Color.White;
                _dgDCUs.AlternatingRowsDefaultCellStyle.BackColor = Color.WhiteSmoke;
                _dgDCUs.CellBorderStyle = DataGridViewCellBorderStyle.None;

                _bDcuMarkAsDead.Enabled = false;
                if (_editingObject == null) return;
                if (_editingObject.DCUs == null || _editingObject.DCUs.Count == 0)
                {
                    _bindingSourceDCUs = new BindingSource();
                    _dgDCUs.DataSource = _bindingSourceDCUs;
                    return;
                }

                var lastPosition = 0;
                if (_bindingSourceDCUs != null && _bindingSourceDCUs.Count != 0)
                    lastPosition = _bindingSourceDCUs.Position;
                _bindingSourceDCUs =
                    new BindingSource
                    {
                        DataSource = _editingObject.DCUs
                            .OrderBy(dcu => dcu.ToString())
                            .ToList(),
                        AllowNew = false
                    };

                if (lastPosition != 0) _bindingSourceDCUs.Position = lastPosition;

                _dgDCUs.DataSource = _bindingSourceDCUs;

                if (!_dgDCUs.Columns.Contains(DCU.COLUMNFULLNAME))
                {
                    _dgDCUs.Columns.Add(DCU.COLUMNFULLNAME, DCU.COLUMNFULLNAME);
                    _dgDCUs.Columns[DCU.COLUMNFULLNAME].DisplayIndex = 0;
                    _dgDCUs.Columns[DCU.COLUMNFULLNAME].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }

                if (!_dgDCUs.Columns.Contains(CardReaderShort.COLUMN_SYMBOL))
                {
                    _dgDCUs.Columns.Insert(0, new DataGridViewImageColumn() { Name = DCUShort.COLUMN_SYMBOL });
                    _dgDCUs.Columns[DCUShort.COLUMN_SYMBOL].AutoSizeMode =
                        DataGridViewAutoSizeColumnMode.AllCells;
                }

                if (!_dgDCUs.Columns.Contains(DCU.COLUMNONLINESTATE))
                {
                    _dgDCUs.Columns.Add(DCU.COLUMNONLINESTATE, DCU.COLUMNONLINESTATE);
                    _dgDCUs.Columns[DCU.COLUMNONLINESTATE].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                }

                if (_dgDCUs.Columns.Contains(DCU.COLUMNLOGICALADDRESS))
                {
                    _dgDCUs.Columns[DCU.COLUMNLOGICALADDRESS].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                }


                HideColumnDgw(_dgDCUs, DCU.COLUMNIDDCU);
                HideColumnDgw(_dgDCUs, DCU.COLUMNNAME);
                HideColumnDgw(_dgDCUs, DCU.COLUMNCARDREADERS);
                HideColumnDgw(_dgDCUs, DCU.COLUMNCCU);
                HideColumnDgw(_dgDCUs, DCU.COLUMNDESCRIPTION);
                HideColumnDgw(_dgDCUs, DCU.COLUMNDOORENVIRONMENTS);
                HideColumnDgw(_dgDCUs, DCU.COLUMNGUIDDOORENVIRONMENTS);
                HideColumnDgw(_dgDCUs, DCU.COLUMNGUIDCARDREADERS);
                HideColumnDgw(_dgDCUs, DCU.COLUMNENABLEPARENTINFULLNAME);
                HideColumnDgw(_dgDCUs, DCU.COLUMNGUIDCCU);
                HideColumnDgw(_dgDCUs, DCU.COLUMNINPUTS);
                HideColumnDgw(_dgDCUs, DCU.COLUMNGUIDINPUTS);
                HideColumnDgw(_dgDCUs, DCU.COLUMNOUTPUTS);
                HideColumnDgw(_dgDCUs, DCU.COLUMNGUIDOUTPUTS);
                HideColumnDgw(_dgDCUs, DCU.COLUMNOBJECTTYPE);
                HideColumnDgw(_dgDCUs, DCU.COLUMNCKUNIQUE);
                HideColumnDgw(_dgDCUs, DCU.COLUMNALARMTAMPER);
                HideColumnDgw(_dgDCUs, DCU.COLUMN_BLOCK_ALARM_TAMPER);
                HideColumnDgw(_dgDCUs, DCU.COLUMN_OBJ_BLOCK_ALARM_TAMPER_OBJECT_TYPE);
                HideColumnDgw(_dgDCUs, DCU.COLUMN_OBJ_BLOCK_ALARM_TAMPER_ID);
                HideColumnDgw(_dgDCUs, DCU.COLUMN_OBJ_BLOCK_ALARM_TAMPER);
                HideColumnDgw(_dgDCUs, DCU.COLUMN_EVENTLOG_DURING_BLOCK_ALARM_TAMPER);
                HideColumnDgw(_dgDCUs, DCU.COLUMN_TAMPER_SABOTAGE_PRESENTATION_GROUP);
                HideColumnDgw(_dgDCUs, DCU.COLUMNALARMOFFLINE);
                HideColumnDgw(_dgDCUs, DCU.COLUMN_BLOCK_ALARM_OFFLINE);
                HideColumnDgw(_dgDCUs, DCU.COLUMN_OBJ_BLOCK_ALARM_OFFLINE_OBJECT_TYPE);
                HideColumnDgw(_dgDCUs, DCU.COLUMN_OBJ_BLOCK_ALARM_OFFLINE_ID);
                HideColumnDgw(_dgDCUs, DCU.COLUMN_OBJ_BLOCK_ALARM_OFFLINE);
                HideColumnDgw(_dgDCUs, DCU.COLUMN_EVENTLOG_DURING_BLOCK_ALARM_OFFLINE);
                HideColumnDgw(_dgDCUs, DCU.COLUMN_OFFLINE_PRESENTATION_GROUP);
                HideColumnDgw(_dgDCUs, DCU.COLUMNINPUTSCOUNT);
                HideColumnDgw(_dgDCUs, DCU.COLUMNOUTPUTSCOUNT);
                HideColumnDgw(_dgDCUs, DCU.COLUMNDCUSABOTAGEOUTPUT);
                HideColumnDgw(_dgDCUs, DCU.COLUMNGUIDDCUSABOTAGEOUTPUT);
                HideColumnDgw(_dgDCUs, DCU.COLUMNDCUOFFLINEOUTPUT);
                HideColumnDgw(_dgDCUs, DCU.COLUMNGUIDDCUOFFLINEOUTPUT);
                HideColumnDgw(_dgDCUs, DCU.COLUMN_DCU_INPUTS_SABOTAGE_OUTPUT);
                HideColumnDgw(_dgDCUs, DCU.COLUMN_GUID_DCU_INPUTS_SABOTAGE_OUTPUT);
                HideColumnDgw(_dgDCUs, DCU.COLUMNLOCALALARMINSTRUCTION);
                HideColumnDgw(_dgDCUs, DCU.COLUMN_DCU_ALARM_ARCS);
                HideColumnDgw(_dgDCUs, DCU.ColumnVersion);

                _dgDCUs.AutoGenerateColumns = false;
                _dgDCUs.AllowUserToAddRows = false;
                LocalizationHelper.TranslateDataGridViewColumnsHeaders(_dgDCUs);

                ShowGridDCUs();
            }
        }


        private void ShowDCUsUpgrade()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DVoid2Void(ShowDCUsUpgrade));
            }
            else
            {
                _dgvDCUUpgrading.RowsDefaultCellStyle.BackColor = Color.White;
                _dgvDCUUpgrading.AlternatingRowsDefaultCellStyle.BackColor = Color.WhiteSmoke;
                _dgvDCUUpgrading.CellBorderStyle = DataGridViewCellBorderStyle.None;

                if (_editingObject == null) return;
                if (_editingObject.DCUs == null || _editingObject.DCUs.Count == 0)
                {
                    _bindingSourceDCUUpgrades = new BindingSource();
                    _dgvDCUUpgrading.DataSource = _bindingSourceDCUUpgrades;
                    return;
                }

                var lastPosition = 0;
                if (_bindingSourceDCUUpgrades != null && _bindingSourceDCUUpgrades.Count != 0)
                    lastPosition = _bindingSourceDCUUpgrades.Position;
                _bindingSourceDCUUpgrades =
                    new BindingSource
                    {
                        DataSource = _editingObject.DCUs
                            .OrderBy(dcu => dcu.LogicalAddress)
                            .ToList(),
                        AllowNew = false
                    };

                if (lastPosition != 0) _bindingSourceDCUUpgrades.Position = lastPosition;

                _dgvDCUUpgrading.DataSource = _bindingSourceDCUUpgrades;

                if (!_dgvDCUUpgrading.Columns.Contains(DCU.COLUMNFULLNAME))
                {
                    _dgvDCUUpgrading.Columns.Add(DCU.COLUMNFULLNAME, DCU.COLUMNFULLNAME);
                    _dgvDCUUpgrading.Columns[DCU.COLUMNFULLNAME].DisplayIndex = 0;
                }

                if (!_dgvDCUUpgrading.Columns.Contains(DCU.COLUMNONLINESTATE))
                {
                    _dgvDCUUpgrading.Columns.Add(DCU.COLUMNONLINESTATE, DCU.COLUMNONLINESTATE);
                }

                if (!_dgvDCUUpgrading.Columns.Contains(DCU.COLUMNUPGRADEPROGRESS))
                {
                    var column =
                        new DataGridViewProgressColumn
                        {
                            Name = DCU.COLUMNUPGRADEPROGRESS,
                            HeaderText = DCU.COLUMNUPGRADEPROGRESS,
                            Selected = false
                        };
                    _dgvDCUUpgrading.Columns.Add(column);
                }

                if (!_dgvDCUUpgrading.Columns.Contains(DCU.COLUMNSELECTUPGRADE))
                {
                    var column =
                        new DataGridViewCheckBoxColumn
                        {
                            Name = DCU.COLUMNSELECTUPGRADE,
                            HeaderText = DCU.COLUMNSELECTUPGRADE
                        };

                    _dgvDCUUpgrading.Columns.Add(column);
                }

                if (!_dgvDCUUpgrading.Columns.Contains("FwVersion"))
                    _dgvDCUUpgrading.Columns.Add(
                        "FwVersion",
                        "FwVersion");

                foreach (DataGridViewColumn column in _dgvDCUUpgrading.Columns)
                {
                    if (column.Name == DCU.COLUMNONLINESTATE ||
                        column.Name == DCU.COLUMNUPGRADEPROGRESS)
                    {
                        column.DisplayIndex =
                            column.Name == DCU.COLUMNONLINESTATE
                                ? 1
                                : 3;

                        column.Visible = true;
                        column.ReadOnly = true;
                        column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    }
                    else if (column.Name == DCU.COLUMNFULLNAME)
                    {
                        column.DisplayIndex = 0;
                        column.Visible = true;
                        column.ReadOnly = true;
                        column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    }
                    else if (column.Name == DCU.COLUMNSELECTUPGRADE)
                    {
                        column.DisplayIndex = 4;
                        column.Visible = true;
                        column.ReadOnly = false;
                        column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    }
                    else if (column.Name == "FwVersion")
                    {
                        column.DisplayIndex = 2;
                        column.Visible = true;
                        column.ReadOnly = true;
                        column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    }
                    else
                        column.Visible = false;
                }

                _dgvDCUUpgrading.AutoGenerateColumns = false;

                _dgvDCUUpgrading.AllowUserToAddRows = false;

                LocalizationHelper.TranslateDataGridViewColumnsHeaders(_dgvDCUUpgrading);

                ShowGridDCUsUpgrade();

                if (!_dgvDCUUpgrading.Columns.Contains(DCUShort.COLUMN_SYMBOL))
                {
                    _dgvDCUUpgrading.Columns.Insert(0, new DataGridViewImageColumn() { Name = DCUShort.COLUMN_SYMBOL });
                    _dgvDCUUpgrading.Columns[DCUShort.COLUMN_SYMBOL].AutoSizeMode =
                        DataGridViewAutoSizeColumnMode.AllCells;
                }
            }
        }

        private void ShowGridDCUs()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DVoid2Void(ShowGridDCUs));
            }
            else
            {
                if (CgpClient.Singleton.IsConnectionLost(false)) return;
                if (_bindingSourceDCUs == null) return;
                if (_dgDCUs.DataSource == null) return;

                foreach (DataGridViewRow row in _dgDCUs.Rows)
                {
                    row.Cells[DCU.COLUMNONLINESTATE].Value = "";
                    var dcu = (DCU) _bindingSourceDCUs[row.Index];

                    dcu.EnableParentInFullName = false;
                    row.Cells[DCU.COLUMNFULLNAME].Value = dcu.ToString();
                }

                SafeThread.StartThread(SetDCUDgValues);
            }
        }

        private void ShowGridDCUsUpgrade()
        {
            if (InvokeRequired)
            {
                Invoke(new DVoid2Void(ShowGridDCUsUpgrade));
            }
            else
            {
                if (CgpClient.Singleton.IsConnectionLost(false)) return;
                if (_bindingSourceDCUUpgrades == null) return;
                if (_dgvDCUUpgrading.DataSource == null) return;

                foreach (DataGridViewRow row in _dgvDCUUpgrading.Rows)
                {
                    row.Cells[DCU.COLUMNONLINESTATE].Value = "";
                    var dcu = (DCU) _bindingSourceDCUUpgrades[row.Index];
                    dcu.EnableParentInFullName = false;
                    row.Cells[DCU.COLUMNFULLNAME].Value = dcu.ToString();
                }

                SafeThread.StartThread(SetDCUsUpgradeDgValues);
            }
        }

        private void SetDCUDgValues()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DVoid2Void(SetDCUDgValues));
            }
            else
            {
                if (_dgDCUs.DataSource == null) return;

                if (CgpClient.Singleton.IsConnectionLost(false)) 
                    return;

                var images = ((ICgpVisualPlugin)NCASClient.Singleton).GetPluginObjectsImages();

                foreach (DataGridViewRow row in _dgDCUs.Rows)
                {
                    var dcu = (DCU) _bindingSourceDCUs.List[row.Index];
                    OnlineState onlineState;

                    if (dcu != null)
                    {
                        onlineState = Plugin.MainServerProvider.DCUs.GetOnlineStates(dcu);
                    }
                    else
                    {
                        onlineState = OnlineState.Unknown;
                    }

                    row.Cells[DCU.COLUMNONLINESTATE].Value = GetString(onlineState.ToString());

                    row.Cells[0].Value =
                                images.Images[onlineState == OnlineState.Online
                                    ? ObjectType.DCU.ToString()
                                    : ObjTypeHelper.DCUOffline];
                }
            }
        }

        private void SetDCUsUpgradeDgValues()
        {
            if (CgpClient.Singleton.IsConnectionLost(false)) 
                return;

            if (InvokeRequired)
            {
                BeginInvoke(new DVoid2Void(SetDCUsUpgradeDgValues));
            }
            else
            {
                if (_dgvDCUUpgrading.DataSource == null) 
                    return;

                var images = ((ICgpVisualPlugin)NCASClient.Singleton).GetPluginObjectsImages();

                foreach (DataGridViewRow row in _dgvDCUUpgrading.Rows)
                {
                    var dcu = (DCU) _bindingSourceDCUUpgrades.List[row.Index];

                    OnlineState onlineState = dcu != null
                        ? Plugin.MainServerProvider.DCUs.GetOnlineStates(dcu)
                        : OnlineState.Unknown;

                    row.Cells[DCU.COLUMNONLINESTATE].Value = GetString(onlineState.ToString());
                    row.Cells["FwVersion"].Value = "";

                    if (onlineState == OnlineState.Online)
                        row.Cells["FwVersion"].Value =
                            // ReSharper disable once PossibleNullReferenceException, if dcu is null, onlineState != OnlineState.Online
                            Plugin.MainServerProvider.DCUs.GetFirmwareVersion(dcu.IdDCU);

                    row.Cells[0].Value =
                                images.Images[onlineState == OnlineState.Online
                                    ? ObjectType.DCU.ToString()
                                    : ObjTypeHelper.DCUOffline];
                }
            }
        }

        private void SetInputToDgvValues()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DVoid2Void(SetInputToDgvValues));
            }
            else
            {
                if (_dgvInputCCU.DataSource == null) return;
                if (CgpClient.Singleton.IsConnectionLost(false)) return;
                foreach (DataGridViewRow row in _dgvInputCCU.Rows)
                {
                    var input = (Input) _bindingSourceInput.List[row.Index];

                    var inputState = InputState.Unknown;
                    if (input != null)
                        inputState = Plugin.MainServerProvider.Inputs.GetActualStates(input);

                    if (inputState == InputState.Alarm)
                    {
                        row.Cells[Input.COLUMNSTATUSONOFF].Value = GetString("InputAlarm");
                    }
                    else if (inputState == InputState.Short)
                    {
                        row.Cells[Input.COLUMNSTATUSONOFF].Value = GetString("InputShort");
                    }
                    else if (inputState == InputState.Break)
                    {
                        row.Cells[Input.COLUMNSTATUSONOFF].Value = GetString("InputBreak");
                    }
                    else if (inputState == InputState.Normal)
                    {
                        row.Cells[Input.COLUMNSTATUSONOFF].Value = GetString("InputNormal");
                    }
                    else if (inputState == InputState.UsedByAnotherAplication)
                    {
                        row.Cells[Input.COLUMNSTATUSONOFF].Value = GetString("UsedByAnotherAplication");
                    }
                    else if (inputState == InputState.OutOfRange)
                    {
                        row.Cells[Input.COLUMNSTATUSONOFF].Value = GetString("OutOfRange");
                    }
                    else
                        row.Cells[Input.COLUMNSTATUSONOFF].Value = GetString("Unknown");
                }
            }
        }

        private void SetOutputToDgvValues()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DVoid2Void(SetOutputToDgvValues));
            }
            else
            {
                if (_dgvOutputCCU.DataSource == null) return;
                if (CgpClient.Singleton.IsConnectionLost(false)) return;
                foreach (DataGridViewRow row in _dgvOutputCCU.Rows)
                {
                    var output = (Output) _bindingSourceOutput.List[row.Index];

                    var outputState = OutputState.Unknown;
                    var outputRealState = OutputState.Unknown;
                    if (output != null)
                    {
                        outputState = Plugin.MainServerProvider.Outputs.GetActualStates(output);
                        outputRealState = Plugin.MainServerProvider.Outputs.GetRealStates(output);
                    }

                    switch (outputState)
                    {
                        case OutputState.On:
                            row.Cells[Output.COLUMNSTATUSONOFF].Value = GetString("On");
                            break;
                        case OutputState.Off:
                            row.Cells[Output.COLUMNSTATUSONOFF].Value = GetString("Off");
                            break;
                        case OutputState.UsedByAnotherAplication:
                            row.Cells[Output.COLUMNSTATUSONOFF].Value = GetString("UsedByAnotherAplication");
                            break;
                        case OutputState.OutOfRange:
                            row.Cells[Output.COLUMNSTATUSONOFF].Value = GetString("OutOfRange");
                            break;
                        default:
                            row.Cells[Output.COLUMNSTATUSONOFF].Value = GetString("Unknown");
                            break;
                    }

                    if (output == null)
                        continue;

                    row.Cells[Output.COLUMNREALSTATUSONOFF].Value =
                        !output.RealStateChanges
                            ? GetString("ReportingSupressed")
                            : (outputRealState == OutputState.On
                                ? GetString("On")
                                : (outputRealState == OutputState.Off
                                    ? GetString("Off")
                                    : GetString("ReportingSupressed")));
                }
            }
        }

        private void SetBsiLevel()
        {
            if (_editingObject.AlarmBreak == null || _editingObject.AlarmBreak == 0)
            {
                _nAlarmBreak.Value = NCASConstants.DefaultBalancingLevels.ToLevel3;
                EditTextChanger(null, null);
            }
            else
            {
                _nAlarmBreak.Value = (decimal) _editingObject.AlarmBreak;
            }

            if (_editingObject.NormalAlarm == null || _editingObject.NormalAlarm == 0)
            {
                _nNormalAlarm.Value = NCASConstants.DefaultBalancingLevels.ToLevel2;
                EditTextChanger(null, null);
            }
            else
            {
                _nNormalAlarm.Value = (decimal) _editingObject.NormalAlarm;
            }

            if (_editingObject.ShortNormal == null || _editingObject.ShortNormal == 0)
            {
                _nShortNormal.Value = NCASConstants.DefaultBalancingLevels.ToLevel1; ;
                EditTextChanger(null, null);
            }
            else
            {
                _nShortNormal.Value = (decimal) _editingObject.ShortNormal;
            }
            ShowInfoBsiLevel();
        }

        private void SetTemplateBsiLevel()
        {
            _cbTemplateBsiLevel.Items.Clear();
            _cbTemplateBsiLevel.Items.Add(LocalizationHelper.GetString("TextTemplateContalBSISettings"));
            _cbTemplateBsiLevel.Items.Add(LocalizationHelper.GetString("TextTemplateCat12BSISettings"));
        }

        private void _bSet_Click(object sender, EventArgs e)
        {
            var selected = _cbTemplateBsiLevel.SelectedItem as string;
            if (selected == LocalizationHelper.GetString("TextTemplateContalBSISettings"))
            {
                _nShortNormal.Value = NCASConstants.DefaultBalancingLevels.ToLevel1;
                _nNormalAlarm.Value = NCASConstants.DefaultBalancingLevels.ToLevel2;
                _nAlarmBreak.Value = NCASConstants.DefaultBalancingLevels.ToLevel3;
            }
            else if (selected == LocalizationHelper.GetString("TextTemplateCat12BSISettings"))
            {
                _nShortNormal.Value = NCASConstants.BalancingCat12ce.ToLevel1;
                _nNormalAlarm.Value = NCASConstants.BalancingCat12ce.ToLevel2;
                _nAlarmBreak.Value = NCASConstants.BalancingCat12ce.ToLevel3;
            }
            ShowInfoBsiLevel();
        }

        private void ShowInfoBsiLevel(NCASConstants.InputBalancingLevels balancingLevels)
        {
            ShowInfoBsiLevel(balancingLevels.ToLevel1, balancingLevels.ToLevel2, balancingLevels.ToLevel3);
        }

        private void ShowInfoBsiLevel(decimal toLevel1,decimal toLevel2,decimal toLevel3)
        {
            _lValueShort.Text = @"< 0, " + toLevel1 + @"V )";
            _lValueNormal.Text = @"<  " + toLevel1 + @"V, " + toLevel2 + @"V )";
            _lValueAlarm.Text = @"<  " + toLevel2 + @"V, " + toLevel3 + @"V )";
            _lValueBreak.Text = @"<  " + toLevel3 + @"V, inf )";
        }

        private void ShowInfoBsiLevel()
        {
            ShowInfoBsiLevel(_nShortNormal.Value, _nNormalAlarm.Value, _nAlarmBreak.Value);
        }

        private void _cbTemplateBsiLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selected = _cbTemplateBsiLevel.SelectedItem as string;
            if (selected == LocalizationHelper.GetString("TextTemplateContalBSISettings"))
            {
                ShowInfoBsiLevel(NCASConstants.DefaultBalancingLevels);
                
            }
            else if (selected == LocalizationHelper.GetString("TextTemplateCat12BSISettings"))
            {
                ShowInfoBsiLevel(NCASConstants.BalancingCat12ce);
            }
        }

        private void _nShortNormal_ValueChanged(object sender, EventArgs e)
        {
            EditTextChanger(sender, e);
            ControlValueShortNormal();
            ControlValueNormalAlarm();
            ShowInfoBsiLevel();
        }

        private void _nNormalAlarm_ValueChanged(object sender, EventArgs e)
        {
            EditTextChanger(sender, e);
            ControlValueNormalAlarm();
            ControlValueAlarmBreak();
            ControlValueShortNormal();
            ShowInfoBsiLevel();
        }

        private void _nAlarmBreak_ValueChanged(object sender, EventArgs e)
        {
            EditTextChanger(sender, e);
            ControlValueAlarmBreak();
            ControlValueNormalAlarm();
            ShowInfoBsiLevel();
        }


        private void ControlValueShortNormal()
        {
            if (_nShortNormal.Value > _nNormalAlarm.Value)
            {
                _nShortNormal.BackColor = Color.Red;
            }
            else
            {
                _nShortNormal.BackColor = Color.White;
            }
        }

        private void ControlValueNormalAlarm()
        {
            if (_nNormalAlarm.Value < _nShortNormal.Value)
            {
                _nNormalAlarm.BackColor = Color.Red;
            }
            else if (_nNormalAlarm.Value > _nAlarmBreak.Value)
            {
                _nNormalAlarm.BackColor = Color.Red;
            }
            else
            {
                _nNormalAlarm.BackColor = Color.White;
            }
        }

        private void ControlValueAlarmBreak()
        {
            if (_nAlarmBreak.Value < _nNormalAlarm.Value)
            {
                _nAlarmBreak.BackColor = Color.Red;
            }
            else
            {
                _nAlarmBreak.BackColor = Color.White;
            }
        }

        private void NCASCCUEditForm_Load(object sender, EventArgs e)
        {
#if DEBUG
            _bTest.Visible = true;
            _bPrecreateCardReader.Enabled = false;
#endif
            _loggedAsSuperAdmin = LoggedAsSuperAdmin;

            if (_loggedAsSuperAdmin != null)
            {
                if (_loggedAsSuperAdmin.Value)
                    _gbLogging.Visible = true;
                else
                    _gbLogging.Visible = false;
            }
            else
                _gbLogging.Visible = false;

            _dgCardReaders.Top = _dgDCUs.Top;
            _dgCardReaders.Left = _dgDCUs.Left;
            _bCrMarkAsDead.Top = _bDcuMarkAsDead.Top;
            _bCrMarkAsDead.Left = _bDcuMarkAsDead.Left;
            _bPrecreateCardReader.Top = _bPrecreateDCU.Top;
            _bPrecreateCardReader.Left = _bPrecreateDCU.Left;
        }

        private void _bUnconfigure_Click(object sender, EventArgs e)
        {
            if (Dialog.Question(GetString("NCASCCUEditForm_QuestionUnconfigure")))
            {
                DisableConfigurationsButtons();
                ClearResultOfAction();
                SafeThread.StartThread(DoUnconfigure);
            }
        }

        private void DoUnconfigure()
        {
            var success = Plugin.MainServerProvider.CCUs.Unconfigure(_editingObject.IdCCU);
            SetUncofigureResult(success);
            if (success)
            {
                UnconfigureWatchdogOutput();
            }
        }

        private void SetUncofigureResult(bool success)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DBool2Void(SetUncofigureResult), success);
            }
            else
            {
                if (success)
                {
                    _eResultOfAction.Text = GetString("NCASCCUEditForm_UnconfigureSucceeded");
                    _eResultOfAction.BackColor = Color.LightGreen;
                    Close();
                }
                else
                {
                    _eResultOfAction.Text = GetString("NCASCCUEditForm_UnconfigureFailed");
                    _eResultOfAction.BackColor = Color.Red;
                }

                RefreshConfigurationButtons(true);
            }
        }

        private void DisableConfigurationsButtons()
        {
            _isDisabledConfigurationsButtons = true;
            _bUnconfigure.Visible = false;
            _bConfigureForThisServer.Visible = true;
            _bConfigureForThisServer.Enabled = false;
            _bForceReconfiguration.Enabled = false;
            _ccuUpgradeFiles.EnableUpgrade = false;
            _ceUpgradeFiles.EnableUpgrade = false;
            _bCancelConfigurationPassword.Enabled = false;
            _bChangeConfigurePassword.Enabled = false;
        }

        private void _bConfigureForThisServer_Click(object sender, EventArgs e)
        {
            _configurePassword = string.Empty;
            _setConfigurePassword = false;

            var ccuConfOptions = Plugin.MainServerProvider.CCUs.EnableConfigureThisCcu(_editingObject.IdCCU);

            switch (ccuConfOptions)
            {
                case CcuConfigurationOptions.Disable:
                    Dialog.Error(GetString("NCASCCUEditForm_ConfigureForThisServerFailed"));
                    return;
                case CcuConfigurationOptions.DiableServerSettingPwd:
                    Dialog.Error(GetString("ConfigureDisableServerConfigurePswSettings"));
                    return;
                case CcuConfigurationOptions.EnableNewPassword:
                    if (!NewCcuConfigurePassword()) return;
                    break;
                case CcuConfigurationOptions.EnableVerifyPassword:
                    if (!VerifyCcuConfigurePassword()) return;
                    break;
                case CcuConfigurationOptions.Enable:
                    _configurePassword = string.Empty;
                    _setConfigurePassword = true;
                    break;
            }

            if (Dialog.Question(GetString("NCASCCUEditForm_QuestionConfigureForThisServer")))
            {
                DisableConfigurationsButtons();
                ClearResultOfAction();
                SafeThread.StartThread(DoConfigureForThisServer);
            }
        }

        private string _configurePassword;
        private bool _setConfigurePassword;

        private bool NewCcuConfigurePassword()
        {
            string hashPassword;
            var ccp = new CcuConfigurePassword(LocalizationHelper);
            ccp.ShowDialog(out hashPassword, true);
            if (String.IsNullOrEmpty(hashPassword)) return false;
            _configurePassword = hashPassword;
            _setConfigurePassword = true;
            return true;
        }

        private bool VerifyCcuConfigurePassword()
        {
            string hashPassword;
            var ccp = new CcuConfigurePassword(LocalizationHelper);
            ccp.ShowDialog(out hashPassword, false);

            if (String.IsNullOrEmpty(hashPassword)) return false;

            if (Plugin.MainServerProvider.CCUs.ValidConfigurePassword(_editingObject.IdCCU, hashPassword))
            {
                _setConfigurePassword = false;
                return true;
            }
            Dialog.Error("Wrong configure password");
            return false;
        }

        private void DoConfigureForThisServer()
        {
            var result =
                Plugin.MainServerProvider.CCUs
                    .ConfigureForThisServer(_editingObject.IdCCU);

            SetConfigureForThisServerResult(result);

            if (result != ConfigureResult.OK)
                RefreshCCUConfigured(Plugin.MainServerProvider.CCUs.GetActualCCUConfiguredState(_editingObject.IdCCU),
                    _isCCU0);

            if (_setConfigurePassword)
            {
                Plugin.MainServerProvider.CCUs.NewConfigurePassword(_editingObject.IdCCU, _configurePassword);
            }
        }

        private void SetConfigureForThisServerResult(ConfigureResult result)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<ConfigureResult>(SetConfigureForThisServerResult), result);
            }
            else
            {
                // SB
                string resultMessageId;
                bool isErrorRusult = true;

                switch (result)
                {
                    case ConfigureResult.OK:
                        resultMessageId = "NCASCCUEditForm_ConfigureForThisServerSucceeded";
                        isErrorRusult = false;
                        break;
                    case ConfigureResult.AlreadyRunning:
                        resultMessageId = "NCASCCUEditForm_ConfigurationAlreadyRunning";
                        break;
                    case ConfigureResult.GeneralFailure:
                        resultMessageId = "NCASCCUEditForm_ConfigureForThisServerFailed";
                        break;
                    default:
                        // ConfigureResult.LicenceFailure:
                        resultMessageId = "NCASCCUEditForm_ConfigureForThisServerBlockedByLicence";
                        break;
                }

                var resultMessage = GetString(resultMessageId);

                _eResultOfAction.Text = resultMessage;
                _eResultOfAction.BackColor = isErrorRusult ? Color.Red : Color.LightGreen;

                if (isErrorRusult)
                {
                    StatusReport.Error(resultMessage);
                }
                else
                {
                    StatusReport.Info(resultMessage);
                }
                
                RefreshConfigurationButtons(true);
            }
        }

        private void _bApply_Click(object sender, EventArgs e)
        {
            SetSecurityLevelSettingsForCR();

            if (_showOption == ShowOptionsEditForm.Edit && !SaveValuesForCardReaders())
                return;

            ConsiderMaxNodeLookupUpdate();

            if (Apply_Click())
            {
                ShowDCUs();
                _bApply.Enabled = false;
            }
        }

        private bool SaveValuesForCardReaders()
        {
            if (_crCountChanged &&
                (_mbt == MainBoardVariant.CCU40 ||
                 _mbt == MainBoardVariant.CCU12 ||
                 _mbt == MainBoardVariant.CCU05 ||
                 _mbt == MainBoardVariant.CAT12CE))
            {
                if (!Dialog.Question(GetString("QuestionMaxCrChanged")))
                {
                    return false;
                }
            }
            _crCountChanged = false;
            return true;
        }

        private bool _tpDoorEnvironmentsFirstTimeEnter = true;

        private void TpDoorEnvironmentEnter(object sender, EventArgs e)
        {
            if (_tpDoorEnvironmentsFirstTimeEnter)
            {
                _tpDoorEnvironmentsFirstTimeEnter = false;
                SafeThread.StartThread(ShowDoorEnvironments);
            }
            else
            {
                lock (_doorEnvironmentReloadLock)
                {
                    if (_doorEnvironmentReload)
                    {
                        _doorEnvironmentReload = false;
                        SafeThread.StartThread(ShowDoorEnvironments);
                    }
                }
            }
        }

        private void ShowDoorEnvironments()
        {
            if (_editingObject == null || _editingObject.DoorEnvironments == null) return;
            if (InvokeRequired)
            {
                BeginInvoke(new DVoid2Void(ShowDoorEnvironments));
            }
            else
            {
                var lastPosition = 0;
                if (_bindingSourceDoorEnvironments != null && _bindingSourceDoorEnvironments.Count != 0)
                    lastPosition = _bindingSourceDoorEnvironments.Position;
                _bindingSourceDoorEnvironments =
                    new BindingSource
                    {
                        DataSource = _editingObject.DoorEnvironments,
                        AllowNew = false
                    };
                if (lastPosition != 0) _bindingSourceDoorEnvironments.Position = lastPosition;

                _dgvDoorEnvironments.DataSource = _bindingSourceDoorEnvironments;

                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMNIDDOORENVIRONMENT);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMNNUMBER);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMNCCU);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMNGUIDCCU);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMNDCU);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMNGUIDDCU);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMNDOORTIMEUNLOCK);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMNDOORTIMEOPEN);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMNDOORTIMEPREALARM);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMNDOORTIMESIRENAJAR);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMNDOORDELAYBEFOREUNLOCK);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMNDOORDELAYBEFORELOCK);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMNDOORDELAYBEFORECLOSE);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMNDOORDELAYBEFOREBREAKIN);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMNNOTINVOKEINTRUSIONALARM);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMNSENSORSLOCKDOORS);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMNSENSORSOPENDOOR);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMNSENSORSOPENMAXDOORS);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMNSENSORSLOCKDOORSINVERTED);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMNSENSORSOPENDOORINVERTED);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMNSENSORSOPENMAXDOORSINVERTED);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMNSENSORSLOCKDOORSBALANCED);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMNSENSORSOPENDOORBALANCED);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMNSENSORSOPENMAXDOORSBALANCED);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMNACTUATORSELECTRICSTRIKE);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMNACTUATORSEXTRAELECTRICSTRIKE);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMNACTUATORSELECTRICSTRIKEIMPULSE);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMNACTUATORSEXTRAELECTRICSTRIKEIMPULSE);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMNACTUATORSELECTRICSTRIKEIMPULSEDELAY);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMNACTUATORSEXTRAELECTRICSTRIKEIMPULSEDELAY);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMNACTUATORSBYPASSALARM);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMNACTUATORSDOORENVIRONMENT);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMNACTUATORSELECTRICSTRIKEOPPOSITE);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMNACTUATORSELECTRICSTRIKEOPPOSITEIMPULSE);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMNACTUATORSELECTRICSTRIKEOPPOSITEIMPULSEDELAY);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMNACTUATORSEXTRAELECTRICSTRIKEOPPOSITE);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMNACTUATORSEXTRAELECTRICSTRIKEOPPOSITEIMPULSE);
                HideColumnDgw(_dgvDoorEnvironments,
                    DoorEnvironment.COLUMNACTUATORSEXTRAELECTRICSTRIKEOPPOSITEIMPULSEDELAY);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMNGUIDSENSORSLOCKDOORS);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMNGUIDSENSORSOPENDOOR);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMNGUIDSENSORSOPENMAXDOORS);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMNGUIDACTUATORSELECTRICSTRIKE);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMNGUIDACTUATORSEXTRAELECTRICSTRIKE);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMNGUIDACTUATORSELECTRICSTRIKEOPPOSITE);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMNGUIDACTUATORSEXTRAELECTRICSTRIKEOPPOSITE);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMNGUIDACTUATORSBYPASSALARM);

                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMNDOORAJARALARMON);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMN_BLOCK_ALARM_DOOR_AJAR);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMN_OBJ_BLOCK_ALARM_DOOR_AJAR_OBJECT_TYPE);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMN_OBJ_BLOCK_ALARM_DOOR_AJAR_ID);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMN_OBJ_BLOCK_ALARM_DOOR_AJAR);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMN_EVENTLOG_DURING_BLOCK_ALARM_DOOR_AJAR);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMNDOORAJARPRESENTATIONGROUP);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMNDOORAJAROUTPUT);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMNGUIDDOORAJAROUTPUT);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMNINTRUSIONALARMON);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMN_BLOCK_ALARM_INTRUSION);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMN_OBJ_BLOCK_ALARM_INTRUSION_OBJECT_TYPE);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMN_OBJ_BLOCK_ALARM_INTRUSION_ID);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMN_OBJ_BLOCK_ALARM_INTRUSION);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMN_EVENTLOG_DURING_BLOCK_ALARM_INTRUSION);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMNINTRUSIONPRESENTATIONGROUP);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMNINTRUSIONOUTPUT);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMNGUIDINTRUSIONOUTPUT);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMNSABOTAGEALARMON);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMN_BLOCK_ALARM_SABOTAGE);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMN_OBJ_BLOCK_ALARM_SABOTAGE_OBJECT_TYPE);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMN_OBJ_BLOCK_ALARM_SABOTAGE_ID);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMN_OBJ_BLOCK_ALARM_SABOTAGE);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMN_EVENTLOG_DURING_BLOCK_ALARM_SABOTAGE);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMNSABOTAGEPRESENTATIONGROUP);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMNSABOTAGEOUTPUT);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMNGUIDSABOTAGEOUTPUT);

                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMNCARDREADERINTERNAL);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMNGUIDCARDREADERINTERNAL);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMNPUSHBUTTONINTERNAL);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMNGUIDPUSHBUTTONINTERNAL);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMNCARDREADEREXTERNAL);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMNGUIDCARDREADEREXTERNAL);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMNPUSHBUTTONEXTERNAL);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMNGUIDPUSHBUTTONEXTERNAL);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMNCKUNIQUE);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMNOBJECTTYPE);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMNLOCALALARMINSTRUCTION);

                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMN_ALARM_TYPE_AND_ID_ALARM_ARCS);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.COLUMN_DOOR_ENVIRONMENT_ALARM_ARCS);
                HideColumnDgw(_dgvDoorEnvironments, DoorEnvironment.ColumnVersion);

                if (_dgvDoorEnvironments.Columns.Contains(DoorEnvironment.COLUMNNAME))
                    _dgvDoorEnvironments.Columns[DoorEnvironment.COLUMNNAME].AutoSizeMode =
                        DataGridViewAutoSizeColumnMode.Fill;

                _dgvDoorEnvironments.AutoGenerateColumns = false;
                _dgvDoorEnvironments.AllowUserToAddRows = false;

                LocalizationHelper.TranslateDataGridViewColumnsHeaders(_dgvDoorEnvironments);
            }
        }

        private void DgvDoorEnvironmentsDoubleClick(object sender, EventArgs e)
        {
            if (CgpClient.Singleton.IsConnectionLost(false)) return;
            if (_bindingSourceDoorEnvironments == null || _bindingSourceDoorEnvironments.Count == 0) return;
            var doorEnvironment =
                (DoorEnvironment) _bindingSourceDoorEnvironments.List[_bindingSourceDoorEnvironments.Position];
            NCASDoorEnvironmentsForm.Singleton.OpenEditForm(doorEnvironment);
        }

        private void _dgCardReaders_DoubleClick(object sender, EventArgs e)
        {
            if (CgpClient.Singleton.IsConnectionLost(false)) return;

            if (_bindingSourceCardReaders == null || _bindingSourceCardReaders.Count == 0) return;

            var cardReader = _bindingSourceCardReaders.List[_bindingSourceCardReaders.Position] as CardReader;
            NCASCardReadersForm.Singleton.OpenEditForm(cardReader);
        }


        private MainBoardVariant _mbt = MainBoardVariant.Unknown;

        private void SetFormByMainboardType()
        {
            _mbt = Plugin.MainServerProvider.CCUs.GetCCUMainBoardType(_editingObject.IdCCU);
            if (_mbt == MainBoardVariant.Unknown && _onlineState != CCUOnlineState.Online)
            {
                if (_editingObject.CcuMainboardType != null)
                {
                    _mbt = (MainBoardVariant) _editingObject.CcuMainboardType;
                }
            }

            RefreshFormByMainboardType();
        }

        private void RefreshFormByMainboardType()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DVoid2Void(RefreshFormByMainboardType));
            }
            else
            {
                switch (_mbt)
                {
                    case MainBoardVariant.CCU0_ECHELON:
                        _lMainBoardTypeInformation.Text = @"CCU (Echelon)";
                        break;

                    case MainBoardVariant.CCU0_RS485:
                        _lMainBoardTypeInformation.Text = @"CCU (RS485)";
                        break;

                    case MainBoardVariant.CAT12CE:
                        _lMainBoardTypeInformation.Text =
                            Plugin.MainServerProvider.CCUs.IsCat12Combo(GetEditingObject().IdCCU)
                                ? @"CCU12 Combo"
                                : @"CCU12";
                        break;

                    default:
                        _lMainBoardTypeInformation.Text = _mbt.ToString();
                        break;
                }


                if (_mbt == MainBoardVariant.CCU40 ||
                    _mbt == MainBoardVariant.CCU12 ||
                    _mbt == MainBoardVariant.CCU05 ||
                    _mbt == MainBoardVariant.CAT12CE)
                {
                    _lMaximumExpectedDCUCount.Text = GetString("InfoMaximumCrCount");
                    _lMaximumExpectedDCUCount.Visible = true;
                    _eMaximumExpectedDCUCount.Visible = true;
                    _nudInputCount.Visible = Insert;
                    _nudOutputCount.Visible = Insert;
                    _lOutputCount.Visible = Insert;
                    _lInputCount.Visible = Insert;

                    _bPrecreateCardReader.Visible = true;

                    _bPrecreateDCU.Enabled = false;
                    _bPrecreateInput.Visible = !Insert;
                    _bPrecreateOutput.Visible = !Insert;

                    switch (_mbt)
                    {
                        case MainBoardVariant.CCU05:
                            _eMaximumExpectedDCUCount.Maximum = NCASConstants.CCU05_MAX_CR_COUNT;
                            _nudInputCount.Maximum = NCASConstants.CCU05_STANDARD_INPUT_COUNT;
                            _nudOutputCount.Maximum = NCASConstants.CCU05_STANDARD_OUTPUT_COUNT;
                            break;
                        case MainBoardVariant.CCU12:
                            _eMaximumExpectedDCUCount.Maximum = NCASConstants.CCU12_MAX_CR_COUNT;
                            _nudInputCount.Maximum = NCASConstants.CCU12_MAX_STANDARD_INPUT_COUNT;
                            _nudOutputCount.Maximum = NCASConstants.CCU12_MAX_STANDARD_OUTPUT_COUNT;
                            break;
                        case MainBoardVariant.CCU40:
                            _eMaximumExpectedDCUCount.Maximum = NCASConstants.CCU40_MAX_CR_COUNT;
                            _nudInputCount.Maximum = NCASConstants.CCU40_MAX_CAPABLE_INPUT_COUNT;
                            _nudOutputCount.Maximum = NCASConstants.CCU40_MAX_CAPABLE_OUTPUT_COUNT;
                            _bPrecreateCardReader.Enabled = true;
                            _bCrMarkAsDead.Enabled = true;
                            break;
                        case MainBoardVariant.CAT12CE:
                            _eMaximumExpectedDCUCount.Maximum = NCASConstants.CAT12CE_MAX_CR_COUNT;
                            _nudInputCount.Maximum = NCASConstants.CAT12CE_MAX_CAPABLE_INPUT_COUNT;
                            _nudOutputCount.Maximum = NCASConstants.CAT12CE_MAX_CAPABLE_OUTPUT_COUNT;
                            break;
                    }
                }
                else
                {
                    _lMaximumExpectedDCUCount.Visible = false;
                    _eMaximumExpectedDCUCount.Visible = false;
                    _nudInputCount.Visible = false;
                    _nudOutputCount.Visible = false;
                    _lOutputCount.Visible = false;
                    _lInputCount.Visible = false;
                    _bPrecreateCardReader.Visible = false;
                    if (Insert)
                        _bPrecreateDCU.Enabled = false;
                    else
                        _bPrecreateDCU.Enabled = true;
                }

                if (Insert)
                    return;

                ShowTabPortSettings();

                switch (_mbt)
                {
                    case MainBoardVariant.CCU0_RS485:
                        HideTabPortSettings();
                        if (!_tcCCU.Contains(_tpDCUsUpgrade))
                            _tcCCU.TabPages.Insert(6, _tpDCUsUpgrade);
                        if (!Insert && !_bPrecreateDCU.Visible)
                            _bPrecreateDCU.Visible = true;
                        break;
                    case MainBoardVariant.CCU0_ECHELON:
                        HideTabPortSettings();
                        if (!_tcCCU.Contains(_tpDCUsUpgrade))
                            _tcCCU.TabPages.Insert(6, _tpDCUsUpgrade);
                        if (!Insert && !_bPrecreateDCU.Visible)
                            _bPrecreateDCU.Visible = true;
                        _gbCoprocessorVersion.Enabled = false;
                        break;
                    case MainBoardVariant.CCU40:
                    case MainBoardVariant.CCU12:
                    case MainBoardVariant.CCU05:
                    case MainBoardVariant.CAT12CE:
                        if (!_tcCCU.Contains(_tpInputOutput))
                            _tcCCU.TabPages.Insert(3, _tpInputOutput);
                        if (!_tcCCU.Contains(_tpDoorEnvironments))
                            _tcCCU.TabPages.Insert(5, _tpDoorEnvironments);
                        if (_tcCCU.Contains(_tpDCUsUpgrade))
                            _tcCCU.TabPages.Remove(_tpDCUsUpgrade);
                        if (_bPrecreateDCU.Visible)
                            _bPrecreateDCU.Visible = false;
                        break;
                    default:
                        if (_tcCCU.Contains(_tpInputOutput))
                            _tcCCU.TabPages.Remove(_tpInputOutput);
                        if (_tcCCU.Contains(_tpDoorEnvironments))
                            _tcCCU.TabPages.Remove(_tpDoorEnvironments);
                        if (_tcCCU.Contains(_tpDCUsUpgrade))
                            _tcCCU.TabPages.Remove(_tpDCUsUpgrade);
                        HideTabPortSettings();
                        if (_bPrecreateDCU.Visible)
                            _bPrecreateDCU.Visible = false;
                        break;
                }

                SafeThread.StartThread(ShowCRsUpgrade);
            }
        }

        private void ShowTabPortSettings()
        {
            if (InvokeRequired)
                BeginInvoke(new DVoid2Void(ShowTabPortSettings));
            else
            {
                if (!_tcCCU.TabPages.Contains(_tpPortSettings))
                {
                    //_tcCCU.TabPages.Add(_tpPortSettings);
                }
            }
        }

        private void HideTabPortSettings()
        {
            if (InvokeRequired)
                BeginInvoke(new DVoid2Void(HideTabPortSettings));
            else
            {
                _tcCCU.TabPages.Remove(_tpPortSettings);
            }
        }

        private void ShowBaudRate()
        {
            if (!Insert)
            {
                if (_isCCU0)
                    _chbEnabledComPort.Checked = _editingObject.EnabledComPort;
                else
                    _chbEnabledComPort.Checked = true;

                if (_editingObject.PortBaudRateCom != 0)
                {
                    _cbPortComBaudRate.SelectedItem =
                        _editingObject.PortBaudRateCom.ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    _cbPortComBaudRate.SelectedItem = "19200";
                }
            }
            else
            {
                _chbEnabledComPort.Checked = true;
                _cbPortComBaudRate.SelectedItem = "19200";
            }
        }

        private void SetBaudRate()
        {
            try
            {
                if (_isCCU0)
                    _editingObject.EnabledComPort = _chbEnabledComPort.Checked;

                if (_cbPortComBaudRate.SelectedItem != null)
                {
                    _editingObject.PortBaudRateCom = Int32.Parse(_cbPortComBaudRate.Text);
                }
            }
            catch
            {
            }
        }

        private void ClearResultOfAction()
        {
            _eResultOfAction.BackColor = _baseBackColorResultOfAction;
            _eResultOfAction.Text = string.Empty;
        }

        private void _bForceReconfiguration_Click(object sender, EventArgs e)
        {
            if (Dialog.WarningQuestion(GetString("NCASCCUEditForm_WarningQuestionForceReconfiguration") +
                                       Environment.NewLine + Environment.NewLine + GetString("QuestionContinueAnyway")))
            {
                if (Dialog.ErrorQuestion(GetString("NCASCCUEditForm_ErrorQuestionForceReconfiguration") +
                                         Environment.NewLine + Environment.NewLine + GetString("QuestionContinueAnyway")))
                {
                    DisableConfigurationsButtons();
                    ClearResultOfAction();
                    SafeThread.StartThread(DoForceReconfiguration);
                }
            }
        }

        private void DoForceReconfiguration()
        {
            var result = Plugin.MainServerProvider.CCUs.ForceReconfiguration(_editingObject.IdCCU);
            SetForceReconfigurationResult(result);
        }

        private void SetForceReconfigurationResult(ConfigureResult result)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<ConfigureResult>(SetForceReconfigurationResult), result);
            }
            else
            {
                switch (result)
                {
                    case ConfigureResult.OK:
                        _eResultOfAction.Text = GetString("NCASCCUEditForm_ForceReconfigurationSucceeded");
                        _eResultOfAction.BackColor = Color.LightGreen;
                        break;

                    case ConfigureResult.AlreadyRunning:
                        _eResultOfAction.Text = GetString("NCASCCUEditForm_ConfigurationAlreadyRunning");
                        _eResultOfAction.BackColor = Color.Red;
                        break;

                    case ConfigureResult.GeneralFailure:
                        _eResultOfAction.Text = GetString("NCASCCUEditForm_ForceReconfigurationFailed");
                        _eResultOfAction.BackColor = Color.Red;
                        break;
                }

                RefreshConfigurationButtons(true);
            }
        }

        private void _cbTimeZone_SelectedIndexChanged(object sender, EventArgs e)
        {
            EditTextChanger(sender, e);
            SetTimeZoneShift();
        }

        private void SetTimeZoneShift()
        {
            if (_cbTimeZone.SelectedItem is TimeZoneInfo)
            {
                var tzi = _cbTimeZone.SelectedItem as TimeZoneInfo;
                _eHours.Value = tzi.BaseUtcOffset.Hours;
                _eMinutes.Value = tzi.BaseUtcOffset.Minutes;
                ShowTimeZoneShift();
            }
        }

        private void ShowTimeZoneShift()
        {
            SetTimeZoneShift(string.Empty);
            var ccuTZ = _cbTimeZone.SelectedItem as TimeZoneInfo;
            if (ccuTZ == null) return;

            var serverTZ = CgpClient.Singleton.MainServerProvider.GetServerTimeZoneInfo();
            var dt = DateTime.Now;

            var serverTS = serverTZ.GetUtcOffset(dt);
            var ccuTS = ccuTZ.GetUtcOffset(dt);

            var ccuManualOffset = (int) (_eHours.Value*60 + _eMinutes.Value);
            var ccuTimeZoneOffset = (int) ccuTZ.BaseUtcOffset.TotalMinutes;

            var resultManualOffser = ccuManualOffset - ccuTimeZoneOffset;

            var delta = ccuTS - serverTS;
            var deltaManual = new TimeSpan(0, resultManualOffser, 0);

            delta += deltaManual;

            SetTimeZoneShift(delta.ToString());
        }

        private void SetTimeZoneShift(string delta)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(SetTimeZoneShift), delta);
            }
            else
            {
                _lDeltaValue.Text = delta;
            }
        }

        private void _eHours_ValueChanged(object sender, EventArgs e)
        {
            EditTextChanger(sender, e);
            ShowTimeZoneShift();
        }

        private void _eMinutes_ValueChanged(object sender, EventArgs e)
        {
            EditTextChanger(sender, e);
            ShowTimeZoneShift();
        }

        private void _bAdd_Click(object sender, EventArgs e)
        {
            if (CheckIPAddress())
            {
                if (!_lSNTPIPAddresses.Items.Contains(_eSNTPIpAddress.Text))
                {
                    _lSNTPIPAddresses.Items.Add(_eSNTPIpAddress.Text);
                    EditTextChanger(null, null);
                }
                else
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _eSNTPIpAddress,
                        GetString("ErrorIpAddressAlreadyAdded"), ControlNotificationSettings.Default);
                }
            }
        }

        private void _bDelete_Click(object sender, EventArgs e)
        {
            if (_lSNTPIPAddresses.Items.Count > 0 && _lSNTPIPAddresses.SelectedItem != null)
            {
                if (Dialog.Question(GetString("QuestionDeleteIpAddressConfirm")))
                {
                    _lSNTPIPAddresses.Items.Remove(_lSNTPIPAddresses.SelectedItem);
                    EditTextChanger(null, null);
                }
            }
        }

        private bool CheckIPAddress()
        {
            if (_eSNTPIpAddress.Text != string.Empty)
            {
                if (IPHelper.IsValid4(_eSNTPIpAddress.Text))
                {
                    return true;
                }
                ControlNotification.Singleton.Error(NotificationPriority.JustOne, _eSNTPIpAddress,
                    GetString("ErrorNotValidIpAddress"), ControlNotificationSettings.Default);
            }
            else
            {
                ControlNotification.Singleton.Error(NotificationPriority.JustOne, _eSNTPIpAddress,
                    GetString("ErrorEntryIpAddress"), ControlNotificationSettings.Default);
            }

            return false;
        }

        private void _eSNTPIpAddress_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar) || e.KeyChar == '.'))
                e.Handled = true;
        }

        private void _bResolve_Click(object sender, EventArgs e)
        {
            if (CheckHostName())
            {
                EditTextChanger(null, null);
                SafeThread<string>.StartThread(ResolveDnsToIPaddress, _eNtpDnsHostName.Text);
            }
        }

        /// <summary>
        /// Obtain one IP address from DNS name and send it to edit NTP IP Address
        /// </summary>
        /// <param name="dnsName"></param>
        private void ResolveDnsToIPaddress(string dnsName)
        {
            try
            {
                IPHostEntry host;
                host = Dns.GetHostEntry(dnsName);
                if (host.AddressList != null && host.AddressList.Length > 0)
                {
                    var ipAddress = host.AddressList.ElementAt(0);
                    SetNtpIpAddress(ipAddress.ToString());
                }
            }
            catch (SocketException error)
            {
                ControlNotification.Singleton.Error(NotificationPriority.JustOne,
                    _eNtpDnsHostName, GetString("ErrorDNS") + ": " + error.Message, ControlNotificationSettings.Default);
            }
            catch (ArgumentException aEerror)
            {
                ControlNotification.Singleton.Error(NotificationPriority.JustOne,
                    _eNtpDnsHostName, GetString("ErrorDNS") + ": " + aEerror.Message,
                    ControlNotificationSettings.Default);
            }
            catch
            {
            }
        }

        private void SetNtpIpAddress(string ipAddressString)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DString2Void(SetNtpIpAddress), ipAddressString);
            }
            else
            {
                _eSNTPIpAddress.Text = ipAddressString;
            }
        }

        private void _bHardReset_Click(object sender, EventArgs e)
        {
            if (!Dialog.WarningQuestion(GetString("QuestionHardResetCCU")))
                return;

            SafeThread.StartThread(HardResetCcu);
        }

        private void HardResetCcu()
        {
            if (Plugin.MainServerProvider != null)
            {
                if (!Plugin.MainServerProvider.CCUs.ResetCCU(_editingObject.IdCCU))
                    HardResetCcuError();
            }
        }

        private void HardResetCcuError()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(HardResetCcuError));
                return;
            }

            ControlNotification.Singleton.Error(NotificationPriority.JustOne, _bHardReset,
                GetString("ErrorDeviceResetFailed"),
                new ControlNotificationSettings(NotificationParts.Hint, HintPosition.Center));
        }


        private bool CheckHostName()
        {
            if (_eNtpDnsHostName.Text != string.Empty)
            {
                if (Hostname.IsValid(_eNtpDnsHostName.Text))
                {
                    return true;
                }
                ControlNotification.Singleton.Error(NotificationPriority.JustOne, _eNtpDnsHostName,
                    GetString("ErrorNotValidDNSHostname"), ControlNotificationSettings.Default);
            }
            else
            {
                ControlNotification.Singleton.Error(NotificationPriority.JustOne, _eNtpDnsHostName,
                    GetString("ErrorEntryDNSHostname"), ControlNotificationSettings.Default);
            }

            return false;
        }

        private void LoadSNTPIpAddressesHostNames(string ipAddresses)
        {
            if (ipAddresses != null && ipAddresses != string.Empty)
            {
                var ipAddress = GetNextIpAddressHostName(ref ipAddresses);
                while (ipAddress != string.Empty)
                {
                    _lSNTPIPAddresses.Items.Add(ipAddress);
                    ipAddress = GetNextIpAddressHostName(ref ipAddresses);
                }
            }
        }

        private static string GetNextIpAddressHostName(ref string ipAddressesHostNames)
        {
            if (!string.IsNullOrEmpty(ipAddressesHostNames))
            {
                var pos = ipAddressesHostNames.IndexOf(";", StringComparison.Ordinal);
                if (pos > 0)
                {
                    var ipAddressHostName = ipAddressesHostNames.Substring(0, pos);
                    ipAddressesHostNames = ipAddressesHostNames.Substring(pos + 1);
                    return ipAddressHostName;
                }
            }

            return string.Empty;
        }

        private string GetSNTPIpAddresses()
        {
            var ipAddresses = string.Empty;

            if (_lSNTPIPAddresses.Items.Count > 0)
            {
                foreach (var item in _lSNTPIPAddresses.Items)
                {
                    ipAddresses += item + ";";
                }
            }

            return ipAddresses;
        }

        private void RefreshIPSettings(IPSetting ipSetting)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<IPSetting>(RefreshIPSettings), ipSetting);
            }
            else
            {
                _ipSetting = ipSetting;
                if (ipSetting == null)
                {
                    _rbDHCP.Checked = false;
                    _rbStatic.Checked = false;
                    _eIPSettingsIPAddress.Text = string.Empty;
                    _eIPSettingsMask.Text = string.Empty;
                    _eIPSettingsGateway.Text = string.Empty;
                    DisabledControls(_tpIPSettings);
                }
                else
                {
                    if (_configured == CCUConfigurationState.ConfiguredForThisServer)
                        EnabledControls(_tpIPSettings);
                    else
                        DisabledControls(_tpIPSettings);

                    if (ipSetting.IsDHCP)
                    {
                        _rbDHCP.Checked = true;
                    }
                    else
                    {
                        _rbStatic.Checked = true;
                    }

                    _rbStatic_CheckedChanged(null, null);

                    _eIPSettingsIPAddress.Text = ipSetting.IPAddress;
                    _eIPSettingsMask.Text = ipSetting.SubnetMask;
                    _eIPSettingsGateway.Text = ipSetting.Gateway;
                }
            }
        }

        private void _rbStatic_CheckedChanged(object sender, EventArgs e)
        {
            _bApply1.Enabled = false;
            _bTestIP.Enabled = true;

            if (_configured == CCUConfigurationState.ConfiguredForThisServer && _rbStatic.Checked)
            {
                _eIPSettingsIPAddress.Enabled = true;
                _eIPSettingsMask.Enabled = true;
                _eIPSettingsGateway.Enabled = true;
            }
            else
            {
                _eIPSettingsIPAddress.Enabled = false;
                _eIPSettingsMask.Enabled = false;
                _eIPSettingsGateway.Enabled = false;
            }
        }

        private void _bApply1_Click(object sender, EventArgs e)
        {
            if (Dialog.WarningQuestion(GetString("NCASCCUEditForm_QuestionApplyIPSettings")))
            {
                _bApply1.Enabled = false;
                ClearResultOApplyIPSettings();

                var ipSetting = new IPSetting(_rbDHCP.Checked, _eIPSettingsIPAddress.Text, _eIPSettingsMask.Text,
                    _eIPSettingsGateway.Text);

                if (ipSetting.Compare(_ipSetting))
                {
                    _eResultOfApplyIPSettings.Text = GetString("NCASCCUEditForm_ApplyIPSettingsTheSameIPSettings");
                    _eResultOfApplyIPSettings.BackColor = Color.Yellow;
                    return;
                }

                SafeThread<Action<IPSetting>>.StartThread(DoApplyIPSettings, ipSetting);
            }
        }

        private void DoApplyIPSettings(IPSetting ipSetting)
        {
            var newIpAddress = Plugin.MainServerProvider.CCUs.SetIpSettings(_editingObject.IdCCU, ipSetting);
            if (newIpAddress != string.Empty)
            {
                LockChanges();
                SetIPAddress(newIpAddress);
                UnlockChanges();
                SetApplyIPSettingsResult(true);
            }
            else
            {
                SetApplyIPSettingsResult(false);
            }
        }

        private void ClearResultOApplyIPSettings()
        {
            _eResultOfApplyIPSettings.BackColor = _baseBackColorResultOfAction;
            _eResultOfApplyIPSettings.Text = string.Empty;
        }

        private void SetIPAddress(string ipAddress)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<string>(SetIPAddress), ipAddress);
            }
            else
            {
                _eIPAddress.Text = ipAddress;
            }
        }

        private void SetApplyIPSettingsResult(bool success)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DBool2Void(SetApplyIPSettingsResult), success);
            }
            else
            {
                if (success)
                {
                    _eResultOfApplyIPSettings.Text = GetString("NCASCCUEditForm_ApplyIPSettingsSucceeded");
                    _eResultOfApplyIPSettings.BackColor = Color.LightGreen;
                }
                else
                {
                    _eResultOfApplyIPSettings.Text = GetString("NCASCCUEditForm_ApplyIPSettingsFailed");
                    _eResultOfApplyIPSettings.BackColor = Color.Red;
                    _bApply1.Enabled = true;
                }
            }
        }

        private void _chbEnabledComPort_CheckedChanged(object sender, EventArgs e)
        {
            if (IsSetValues)
                _cbPortComBaudRate.Enabled = _chbEnabledComPort.Checked;

            EditTextChanger(sender, e);
        }

        private void SetReferencedBy()
        {
            _tpReferencedBy.Controls.Add(new ReferencedByForm(GetListReferencedObjects, NCASClient.LocalizationHelper,
                ObjectImageList.Singleton.GetAllObjectImages()));
        }

        private IList<AOrmObject> GetListReferencedObjects()
        {
            return Plugin.MainServerProvider.CCUs.
                GetReferencedObjects(_editingObject.IdCCU, CgpClient.Singleton.GetListLoadedPlugins());
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

        private bool _createName;

        private void _eIPAddress_KeyUp(object sender, KeyEventArgs e)
        {
            if (_eName.Text == string.Empty)
            {
                _createName = true;
            }
            if (_createName)
            {
                _eName.Text = @"CCU " + _eIPAddress.Text;
            }
        }

        private void _dgDCUs_DoubleClick(object sender, EventArgs e)
        {
            if (CgpClient.Singleton.IsConnectionLost(false)) return;

            if (_bindingSourceDCUs == null || _bindingSourceDCUs.Count == 0) return;

            var dcu = _bindingSourceDCUs.List[_bindingSourceDCUs.Position] as DCU;
            NCASDCUsForm.Singleton.OpenEditForm(dcu);
        }

        private void _dgvDCUUpgrading_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == _dgvDCUUpgrading.Columns[DCU.COLUMNSELECTUPGRADE].Index)
                return;
            if (CgpClient.Singleton.IsConnectionLost(false)) return;

            if (_bindingSourceDCUUpgrades == null || _bindingSourceDCUUpgrades.Count == 0) return;

            var dcu = _bindingSourceDCUUpgrades.List[_bindingSourceDCUUpgrades.Position] as DCU;
            NCASDCUsForm.Singleton.OpenEditForm(dcu);
        }

        private void _bRefresh_Click(object sender, EventArgs e)
        {
            _eFirmware.Text = string.Empty;
            if (_editingObject != null && !CgpClientMainForm.Singleton.MainIsConnectionLost(false))
            {
                SafeThread.StartThread(ObtainCcuFirmwareVersion);
            }
        }

        private void ObtainCcuFirmwareVersion()
        {
            var ccuFirmwareVersion = Plugin.MainServerProvider.CCUs.GetFirmwareVersion(_editingObject.IdCCU);
            ShowCcuFirmwareVersion(ccuFirmwareVersion);
        }

        private void ShowCcuFirmwareVersion(string ccuFirmwareVersion)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DString2Void(ShowCcuFirmwareVersion), ccuFirmwareVersion);
            }
            else
            {
                _eFirmware.Text = ccuFirmwareVersion;
            }
        }

        private void _bCrMarkAsDead_Click(object sender, EventArgs e)
        {
            if (CgpClient.Singleton.IsConnectionLost(false)) return;
            if (_bindingSourceCardReaders == null || _bindingSourceCardReaders.Count == 0) return;

            var cardReader = _bindingSourceCardReaders.List[_bindingSourceCardReaders.Position] as CardReader;

            if (cardReader != null)
            {
                var crOnlineState = Plugin.MainServerProvider.CardReaders.GetOnlineStates(cardReader.IdCardReader);
                if (crOnlineState != OnlineState.Online)
                {
                    if (!NCASCardReadersForm.Singleton.DeleteAllowedNotEdited(cardReader))
                    {
                        return;
                    }

                    if (Dialog.Question(GetString("QuestionDeleteMarkAsDeadCR")))
                    {
                        Exception error;
                        Plugin.MainServerProvider.CardReaders.Delete(cardReader, out error);

                        if (error != null)
                        {
                            if (error is SqlDeleteReferenceConstraintException)
                            {
                                Dialog.Info(GetString("InfoCannotRemoveUsedObject"));
                            }
                            else
                            {
                                Dialog.Info(GetString("InfoRemoveObjectFailed"));
                            }
                        }

                        var ccu = Plugin.MainServerProvider.CCUs.GetObjectById(_editingObject.IdCCU);
                        if (ccu != null)
                        {
                            _editingObject.CardReaders = ccu.CardReaders;
                            ShowCardReaders();
                        }
                    }
                }
            }
        }

        private readonly List<Guid> _actualMarkedAsDeadDcus = new List<Guid>();

        private void _bDcuMarkAsDead_Click(object sender, EventArgs e)
        {
            if (CgpClient.Singleton.IsConnectionLost(false)) return;
            if (_bindingSourceDCUs == null || _bindingSourceDCUs.Count == 0) return;

            var dcu = _bindingSourceDCUs.List[_bindingSourceDCUs.Position] as DCU;
            if (dcu != null)
            {
                if (_actualMarkedAsDeadDcus.Contains(dcu.IdDCU))
                {
                    Dialog.Error(GetString("DcuActualDeleting"));
                    return;
                }
                _actualMarkedAsDeadDcus.Add(dcu.IdDCU);
                SafeThread<DCU>.StartThread(DoMarkAsDead, dcu);
            }
        }

        private void DoMarkAsDead(DCU dcu)
        {
            var idActMarkedDcu = dcu.IdDCU;
            try
            {
                var onlineState = Plugin.MainServerProvider.DCUs.GetOnlineStates(dcu);
                if (onlineState != OnlineState.Online)
                {
                    if (!NCASDCUsForm.Singleton.DeleteAllowedNotEdited(dcu)) return;
                    if (Dialog.Question(GetString("QuestionDeleteMarkAsDeadDCU")))
                    {
                        Exception error;
                        Plugin.MainServerProvider.DCUs.Delete(dcu, out error);

                        if (error != null)
                        {
                            if (error is SqlDeleteReferenceConstraintException)
                            {
                                Dialog.Info(GetString("InfoCannotRemoveUsedObject"));
                            }
                            else
                            {
                                Dialog.Info(GetString("InfoRemoveObjectFailed"));
                            }
                        }

                        var ccu = Plugin.MainServerProvider.CCUs.GetObjectById(_editingObject.IdCCU);
                        if (ccu != null)
                        {
                            _editingObject.DCUs = ccu.DCUs;
                            ShowDCUs();
                        }
                    }
                }
            }
            catch
            {
            }
            finally
            {
                if (_actualMarkedAsDeadDcus.Contains(idActMarkedDcu))
                {
                    _actualMarkedAsDeadDcus.Remove(idActMarkedDcu);
                }
            }
        }

        private void _dgDCUs_SelectionChanged(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<object, EventArgs>(_dgDCUs_SelectionChanged), sender, e);
            }
            else
            {
                if (_bindingSourceDCUs != null && _bindingSourceDCUs.Count > 0)
                {
                    var dcu = _bindingSourceDCUs.List[_bindingSourceDCUs.Position] as DCU;
                    if (dcu != null)
                    {
                        if (CgpClient.Singleton.IsConnectionLost(false)) return;
                        var onlineState = Plugin.MainServerProvider.DCUs.GetOnlineStates(dcu);
                        if (onlineState != OnlineState.Online)
                        {
                            if (IsEditAllowed)
                            {
                                _bDcuMarkAsDead.Enabled = true;
                            }
                        }
                        else
                        {
                            _bDcuMarkAsDead.Enabled = false;
                        }
                    }
                }
            }
        }

        private void _dgCardReaders_SelectionChanged(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<object, EventArgs>(_dgCardReaders_SelectionChanged), sender, e);
            }
            else
            {
                if (CgpClient.Singleton.IsConnectionLost(false)) return;
                if (_bindingSourceCardReaders == null || _bindingSourceCardReaders.Count == 0) return;
                var cardReader = _bindingSourceCardReaders.List[_bindingSourceCardReaders.Position] as CardReader;
                if (cardReader != null)
                {
                    //_bCrMarkAsDead.Enabled = true;
                    var crOnlineState = Plugin.MainServerProvider.CardReaders.GetOnlineStates(cardReader.IdCardReader);
                    if (crOnlineState != OnlineState.Online)
                    {
                        if (IsEditAllowed)
                        {
                            _bCrMarkAsDead.Enabled = true;
                        }
                    }
                    else
                    {
                        _bCrMarkAsDead.Enabled = false;
                    }
                }
            }
        }

        private void _bTimeRefresh_Click(object sender, EventArgs e)
        {
            ShowCcuTime(string.Empty);
            if (!CgpClientMainForm.Singleton.MainIsConnectionLost(false))
            {
                SafeThread.StartThread(ObtainCcuTime);
            }
        }

        private void ObtainCcuTime()
        {
            object dateTime = Plugin.MainServerProvider.CCUs.GetCurrentCCUTime(_editingObject.IdCCU);

            var textTime = dateTime == null
                ? ""
                : ((DateTime) dateTime).ToString(CultureInfo.InvariantCulture);

            ShowCcuTime(textTime);
        }

        private void HideDGCardReaders()
        {
            if (InvokeRequired)
                BeginInvoke(new DVoid2Void(HideDGCardReaders));
            else
            {
                _tpInformation.Text = GetString("NCASCCUEditForm_lDCUs");
                _dgDCUs.Visible = true;
                _bDcuMarkAsDead.Visible = true;
                _bPrecreateDCU.Visible = true;

                _bCrMarkAsDead.Visible = false;
                _dgCardReaders.Visible = false;
                _bPrecreateCardReader.Visible = false;
            }
        }

        private void ShowDGCardReaders()
        {
            if (InvokeRequired)
                BeginInvoke(new DVoid2Void(ShowDGCardReaders));
            else
            {
                _tpInformation.Text = GetString("NCASCCUEditForm_lDirectCardReaders");
                _dgDCUs.Visible = false;
                _bDcuMarkAsDead.Visible = false;
                _bPrecreateDCU.Visible = false;

                _bCrMarkAsDead.Visible = true;
                _dgCardReaders.Visible = true;
                _bPrecreateCardReader.Visible = true;
            }
        }

        protected override void AfterDockUndock()
        {
            _firstTimeInput = true;
            _firstTimeOutput = true;
            _firsTimeDgShow = true;
            ShowDCUs();
            ShowDCUsUpgrade();
            DgvInputCCUVisibleChanged(null, null);
            DgvOutputCCUVisibleChanged(null, null);
            ShowCRsUpgrade();
        }

        private string _ccuUpgradeFile = string.Empty;

        private void UpgradeCcu(object upgradeFile)
        {
            if (!Dialog.Question(string.Format(GetString("QuestionStartUpgrading"), "CCU", upgradeFile)))
                return;

            try
            {
                int ceImageVersion = Int32.Parse(Plugin.MainServerProvider.CCUs.WinCEImageVersion(_editingObject.IdCCU));
                int upgradeFwVersion = Int32.Parse(((string) upgradeFile).Split('.')[2]);

                if (ceImageVersion < NCASConstants.MinimalCeVersionForCcu21
                    && upgradeFwVersion >= NCASConstants.MinimalReleasedCcuVersionForNova21)
                {
                    Dialog.Warning(GetString("WarningNeedUpgradeWinCe", upgradeFwVersion,ceImageVersion));
                    return;
                }
            }
            catch (Exception err)
            {
                err.TryBreak();
            }

            ClearUpgradeState();
            //if (_lbAvailableUpgrades.SelectedItem == null)
            //    return;
            if (Plugin.MainServerProvider == null || CgpClient.Singleton.IsConnectionLost(false))
                return;

            var state = Plugin.MainServerProvider.CCUs.GetCCUConfiguredState(_editingObject.IdCCU);
            var alreadyUpgrading = Plugin.MainServerProvider.IsCCUUpgrading(_editingObject.IdCCU);

            if (state == CCUConfigurationState.ConfiguredForThisServerUpgradeOnly
                || state == CCUConfigurationState.ConfiguredForThisServerUpgradeOnlyWinCe)
                Dialog.Warning(GetString("WarningUpgradingUpgradeOnlyCCU"));

            if (state != CCUConfigurationState.Upgrading && !alreadyUpgrading)
            {
                _ccuUpgradeFile = upgradeFile as string;
                _ccuUpgradeFiles.EnableUpgrade = false;

                if (_cbVerbosityLevel.Items.Count > 0)
                    _cbVerbosityLevel.SelectedIndex = 0;

                _nudVerbosityLevel.Value = 0;
                _bSet1_Click(null, null);
                SafeThread.StartThread(StartUpgradeProcess);
            }
            else
            {
                RefreshCCUConfigured(state, _isCCU0);
                ControlNotification.Singleton.Error(NotificationPriority.JustOne, _ccuUpgrade,
                    GetString("DeviceAlreadyUpgrading"),
                    new ControlNotificationSettings(NotificationParts.Hint, HintPosition.Center));
            }
        }

        private Guid _upgradeID;

        private void StartUpgradeProcess()
        {
            _upgradeID = Guid.NewGuid();
            Exception ex;
            if (Plugin.MainServerProvider.StartUpgradeCCU(_ccuUpgradeFile, _editingObject.IPAddress, _upgradeID, out ex))
            {
                RefreshCCUConfigured(CCUConfigurationState.Upgrading, _isCCU0);
                _bStopTransfer.Enabled = true;
            }
            else
            {
                ControlNotification.Singleton.Error(NotificationPriority.JustOne, _ccuUpgrade,
                    GetString("ErrorFailedToStartUpgrade"),
                    new ControlNotificationSettings(NotificationParts.Hint, HintPosition.Center));
                _ccuUpgradeFiles.EnableUpgrade = true;
            }
        }

        private void ClearUpgradeState()
        {
            SetUpgradeProgress(0);
            SetTransferProgress(0);
            ClearFinalisingInfo();
        }

        private void ClearFinalisingInfo()
        {
            if (InvokeRequired)
                Invoke(new DVoid2Void(ClearCEUpgradeState));
            else
            {
                _eUpgradeFinalisation.Clear();
            }
        }

/*
        private void ShowSucceededMEssage()
        {
            if (_bUpgrade.InvokeRequired)
                _bUpgrade.Invoke(new DVoid2Void(ShowSucceededMEssage));
            else
            {
                ControlNotification.Singleton.Info(NotificationPriority.JustOne, _bUpgrade, _editingObject.Name + ": " + GetString("DeviceUpgradeSucceeded"),
                    new ControlNotificationSettings(NotificationParts.Hint, HintPosition.Center));
            }
        }
*/

        private void ShowFailedMessage()
        {
            if (_ccuUpgrade.InvokeRequired)
                _ccuUpgrade.Invoke(new DVoid2Void(ShowFailedMessage));
            else
                ControlNotification.Singleton.Error(NotificationPriority.JustOne, _ccuUpgrade,
                    _editingObject.Name + ": " + GetString("DeviceUpgradeFailed"),
                    new ControlNotificationSettings(NotificationParts.Hint, HintPosition.Center));
        }

        private void SetTransferProgress(int percentage)
        {
            if (InvokeRequired)
                Invoke(new DInt2Void(SetTransferProgress), percentage);
            else
            {
                _bStopTransfer.Enabled = Plugin.MainServerProvider.CanStop(_upgradeID, _editingObject.IdCCU);
                //_eTransferProgress.Text = percentage.ToString(CultureInfo.InvariantCulture);
                _prbTransfer.Value = percentage;
            }
        }

        private void SetMakeLogDumpProgress(int percentage)
        {
            if (InvokeRequired)
                Invoke(new DInt2Void(SetMakeLogDumpProgress), percentage);
            else
            {
                if (percentage < 0)
                {
                    ControlNotification.Singleton.Error(
                        NotificationPriority.JustOne, _bMakeLogDump,
                        GetString("ErrorMakingLogDumpFailed"),
                        new ControlNotificationSettings(NotificationParts.Hint, HintPosition.Center));

                    _bMakeLogDump.Enabled = true;
                    _bMakeLogDump.Visible = true;
                    _prbDebugFilesTransfer.Visible = false;

                    return;
                }

                _prbDebugFilesTransfer.Value = percentage;

                if (percentage >= 100)
                {
                    _bMakeLogDump.Text = GetString("General_Download");
                    _bMakeLogDump.Enabled = true;
                    _bMakeLogDump.Visible = true;
                    _prbDebugFilesTransfer.Visible = false;
                }
            }
        }

        private void SetUpgradeProgress(int percentage)
        {
            if (InvokeRequired)
                Invoke(new DInt2Void(SetUpgradeProgress), percentage);
            else
            {
                if (percentage < 0)
                {
                    ShowFailedMessage();
                    RefreshCCUConfigured(Plugin.MainServerProvider.CCUs.GetCCUConfiguredState(_editingObject.IdCCU),
                        _isCCU0);
                }
                else if (percentage == 0 || percentage > _prbUnpack.Value)
                {
                    _prbUnpack.Value = percentage;
                }
                if (_bStopTransfer.Enabled)
                    _bStopTransfer.Enabled = false;
            }
        }

        private void _bStopTransfer_Click(object sender, EventArgs e)
        {
            if (Plugin.MainServerProvider == null)
                return;
            if (Plugin.MainServerProvider.StopTransferUpgradeProcess(_upgradeID, _editingObject.IdCCU))
            {
                Plugin.MainServerProvider.CCUs.StopUpgradeMode(_editingObject.IdCCU, false);
                ClearCEUpgradeState();
            }
            RefreshCCUConfigured(Plugin.MainServerProvider.CCUs.GetCCUConfiguredState(_editingObject.IdCCU), _isCCU0);
        }

        private void _bPrecreateDCU_Click(object sender, EventArgs e)
        {
            var dcu = new DCU();
            dcu.EnableParentInFullName = Plugin.MainServerProvider.GetEnableParentInFullName();
            dcu.CCU = _editingObject;
            NCASDCUsForm.Singleton.OpenInsertFromEdit(ref dcu, ShowDCUs);
        }

        private void ShowDCUs(object obj)
        {
            var ccu = Plugin.MainServerProvider.CCUs.GetObjectById(_editingObject.IdCCU);
            if (ccu != null)
            {
                _editingObject.DCUs = ccu.DCUs;
                ShowDCUs();
                ShowDCUsUpgrade();
            }
        }


        private bool _tpInputOutputFirstTimeEnter = true;

        private void _tpInputOutput_Enter(object sender, EventArgs e)
        {
            if (_tpInputOutputFirstTimeEnter)
            {
                _tpInputOutputFirstTimeEnter = false;
                SafeThread.StartThread(ShowMyInput);
                SafeThread.StartThread(ShowMyOutput);
            }
            else
            {
                lock (_inputReloadLock)
                {
                    if (_inputReload)
                    {
                        _inputReload = false;
                        SafeThread.StartThread(ShowMyInput);
                    }
                }

                lock (_outputReloadLock)
                {
                    if (_outputReload)
                    {
                        _outputReload = false;
                        SafeThread.StartThread(ShowMyOutput);
                    }
                }
            }
        }

        private bool _tpControlFirstTimeEnter = true;

        private void _tpControl_Enter(object sender, EventArgs e)
        {
            if (_tpControlFirstTimeEnter)
            {
                _tpControlFirstTimeEnter = false;
                SafeThread.StartThread(ShowVersions);
            }
        }

        private void ShowVersions()
        {
            var firmwareVersion = Plugin.MainServerProvider.CCUs.GetFirmwareVersion(_editingObject.IdCCU);
            var ceImageVersion = Plugin.MainServerProvider.CCUs.WinCEImageVersion(_editingObject.IdCCU);
            if (InvokeRequired)
                Invoke((MethodInvoker) delegate
                {
                    _eFirmware.Text = firmwareVersion;
                    _eImageVersion.Text = ceImageVersion;

                    //_bStopCKM.Enabled = int.Parse(ceImageVersion) > 3600;
                    //_bStartExplicityCKM.Enabled = int.Parse(ceImageVersion) > 3600;
                    //_bStartImplicityCKM.Enabled = int.Parse(ceImageVersion) > 3600;
                });
        }

        private void RefreshAvailableUpgradesList()
        {
            if (Plugin.MainServerProvider != null)
            {
                var upgrades = Plugin.MainServerProvider.GetAvailableUpgrades();
                Invoke((MethodInvoker) delegate
                {
                    _ccuUpgradeFiles.Files = upgrades.Cast<object>().ToList();
                });
            }
        }

        private void UpgradeCe(object fileName)
        {
            var upgradeCeFile = fileName as CeUpgradeFile;

            if (upgradeCeFile == null)
                return;

            if (!Dialog.Question(string.Format(GetString("QuestionStartUpgrading"), "CE", upgradeCeFile.Version)))
                return;

            ClearCEUpgradeState();

            if (Plugin.MainServerProvider == null || CgpClient.Singleton.IsConnectionLost(false))
                return;

            var state = Plugin.MainServerProvider.CCUs.GetCCUConfiguredState(_editingObject.IdCCU);
            var alreadyUpgrading = Plugin.MainServerProvider.IsCCUUpgrading(_editingObject.IdCCU);

            if (state != CCUConfigurationState.Upgrading && !alreadyUpgrading)
            {
                if (Plugin.MainServerProvider.CCUs.IsActualWinCeImage(_editingObject.IdCCU, fileName as string))
                {
                    ControlNotification.Singleton.Info(NotificationPriority.JustOne, _ceUpgrade,
                        GetString("ErrorCcuWinCeImageIsActual"),
                        new ControlNotificationSettings(NotificationParts.Hint, HintPosition.Center));
                }
                else if (!Plugin.MainServerProvider.ExistsPathOrFile(_editingObject.IdCCU, @"\Storage card\"))
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _ceUpgrade,
                        GetString("ErrorCCUDoesNotContainSpecificPath") + @" \Storage card\",
                        new ControlNotificationSettings(NotificationParts.Hint, HintPosition.Center));
                }
                else
                {
                    SafeThread<string>.StartThread(StartCETransferProcess, upgradeCeFile.FileName);
                }
            }
            else
            {
                RefreshCCUConfigured(state, _isCCU0);
                ControlNotification.Singleton.Error(NotificationPriority.JustOne, _ceUpgrade,
                    GetString("DeviceAlreadyUpgrading"),
                    new ControlNotificationSettings(NotificationParts.Hint, HintPosition.Center));
            }
        }

        private void StartCETransferProcess(string selectedFileName)
        {
            if (Plugin.MainServerProvider == null)
                return;

            Exception ex;
            _upgradeID = Guid.NewGuid();

            if (Plugin.MainServerProvider.StartUpgradeCE(selectedFileName, _upgradeID, _editingObject.IPAddress, out ex))
            {
                RefreshCCUConfigured(CCUConfigurationState.Upgrading, _isCCU0);
                _bStopTransferCEFile.Enabled = true;
            }
            else
            {
                var exceptionMessage = ex == null ? string.Empty : " (" + ex.Message + ")";
                ControlNotification.Singleton.Error(NotificationPriority.JustOne, _ceUpgrade,
                    GetString("ErrorFailedToStartUpgrade") + exceptionMessage,
                    new ControlNotificationSettings(NotificationParts.Hint, HintPosition.Center));
            }
        }

        private void SetCEUpgradeStage(int actionResult)
        {
            if (InvokeRequired)
                Invoke(new DInt2Void(SetCEUpgradeStage), actionResult);
            else
            {
                var upgrade = (ActionResultUpgrade) actionResult;
                var newText = GetString("CE" + upgrade);
                _eCEUpgradeProgress.Text = newText;

                if (_bStopTransferCEFile.Enabled)
                    _bStopTransferCEFile.Enabled = false;

                _prbTransfer.Value = 0;
                _prbTransferCe.Value = 0;
                _prbUnpack.Value = 0;
            }
        }

/*
        private void SetCETransferProgress(int percent)
        {
            if (InvokeRequired)
                Invoke(new DInt2Void(SetCETransferProgress), percent);
            else
            {
                _bStopTransferCEFile.Enabled = Plugin.MainServerProvider.CanStop(_upgradeID, _editingObject.IdCCU);
                if (_tbCeFileTransfer.Text != percent.ToString())
                    _tbCeFileTransfer.Text = percent.ToString();
                if (percent == 100)
                    _bCEUpgrade.Enabled = false;
            }
        }
*/

        private void ShowCEFailedMessage()
        {
            if (_ceUpgrade.InvokeRequired)
                _ceUpgrade.Invoke(new DVoid2Void(ShowCEFailedMessage));
            else
                ControlNotification.Singleton.Error(NotificationPriority.JustOne, _ceUpgrade,
                    _editingObject.Name + ": " + GetString("DeviceUpgradeFailed"),
                    new ControlNotificationSettings(NotificationParts.Hint, HintPosition.Center));
        }

/*
        private void ShowCESuccessMEssage()
        {
            if (_bCEUpgrade.InvokeRequired)
                _bCEUpgrade.Invoke(new DVoid2Void(ShowCESuccessMEssage));
            else
            {
                ControlNotification.Singleton.Info(NotificationPriority.JustOne, _bCEUpgrade, _editingObject.Name + ": " + GetString("DeviceUpgradeSucceeded"),
                    new ControlNotificationSettings(NotificationParts.Hint, HintPosition.Center));
            }
        }
*/

        private void ClearCEUpgradeState()
        {
            if (InvokeRequired)
                Invoke(new DVoid2Void(ClearCEUpgradeState));
            else
            {
                //_tbCeFileTransfer.Text = @"0";
                _prbTransferCe.Value = 0;
                _eCEUpgradeProgress.Text = string.Empty;
            }
        }

        private void _bStopTransferCEFile_Click(object sender, EventArgs e)
        {
            if (Plugin.MainServerProvider == null)
                return;
            if (Plugin.MainServerProvider.StopTransferUpgradeProcess(_upgradeID, _editingObject.IdCCU))
            {
                Plugin.MainServerProvider.CCUs.StopUpgradeMode(_editingObject.IdCCU, false);
                ClearCEUpgradeState();
            }
            RefreshCCUConfigured(Plugin.MainServerProvider.CCUs.GetCCUConfiguredState(_editingObject.IdCCU), _isCCU0);
        }

        private void _bRefreshDCUs_Click(object sender, EventArgs e)
        {
            ShowGridDCUsUpgrade();
        }

        private void DgvDCUUpgradingCellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == _dgvDCUUpgrading.Columns[DCU.COLUMNSELECTUPGRADE].Index)
                {
                    if (_dgvDCUUpgrading.Rows[e.RowIndex].Cells[DCU.COLUMNONLINESTATE].Value.ToString() ==
                        GetString(OnlineState.Online.ToString()) ||
                        _dgvDCUUpgrading.Rows[e.RowIndex].Cells[DCU.COLUMNONLINESTATE].Value.ToString() ==
                        GetString(OnlineState.WaitingForUpgrade.ToString()))
                    {
                        _dgvDCUUpgrading[e.ColumnIndex, e.RowIndex].ReadOnly = false;
                    }
                    else
                    {
                        _dgvDCUUpgrading[e.ColumnIndex, e.RowIndex].ReadOnly = true;
                        _dgvDCUUpgrading[e.ColumnIndex, e.RowIndex].Value = false;
                        _dgvDCUUpgrading.EndEdit();
                        _dgvDCUUpgrading.Rows[e.RowIndex].Cells[DCU.COLUMNSELECTUPGRADE].Selected = false;
                    }
                }
            }
            catch
            {
            }
        }

        private void RefreshAvailableDCUUpgradeList()
        {
            if (_configured != CCUConfigurationState.ConfiguredForThisServer)
                return;

            if (CgpClient.Singleton.MainServerProvider == null || CgpClient.Singleton.IsConnectionLost(false))
                return;

            string delimeter;
            ICollection<string> dcuVersions = Plugin.MainServerProvider.GetAvailableDCUUpgrades(_editingObject.IdCCU,
                out delimeter);
            var dcuVersionsLocalised = new List<DCUUpgradeFileInfo>();

            if (dcuVersions != null && dcuVersions.Count > 0)
            {
                foreach (var version in dcuVersions)
                {
                    if (!version.Contains(":"))
                        continue;
                    try
                    {
                        dcuVersionsLocalised.Add(new DCUUpgradeFileInfo(version, delimeter[0]));
                    }
                    catch
                    {
                    }
                }

                var source = new BindingSource();
                source.DataSource = dcuVersionsLocalised.ToArray();
                Invoke((MethodInvoker) delegate
                {
                    _dcuUpgradeFiles.Files = dcuVersionsLocalised.Cast<object>().ToList();
                });
            }
            else
                Invoke((MethodInvoker) delegate
                {
                    //_lbAvailableDCUVersions.DataSource = null;
                    //_lbAvailableDCUVersions.Items.Clear();
                });
        }

        private void _chbSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in _dgvDCUUpgrading.Rows)
            {
                if ((row.Cells[DCU.COLUMNONLINESTATE].Value.ToString() == GetString(OnlineState.Online.ToString())) ||
                    (row.Cells[DCU.COLUMNONLINESTATE].Value.ToString() ==
                     GetString(OnlineState.WaitingForUpgrade.ToString())))
                {
                    row.Cells[DCU.COLUMNSELECTUPGRADE].Value = _chbSelectAll.Checked;
                }
                else
                {
                    row.Cells[DCU.COLUMNSELECTUPGRADE].Value = false;
                }
            }
        }

        private void UpgradeDcu(object fileName)
        {
            if (!Dialog.Question(string.Format(GetString("QuestionStartUpgrading"), "Dcu", fileName)))
                return;

            var isSelected = false;
            var upgradeFileInfo = fileName as DCUUpgradeFileInfo;
            if (fileName == null
                || upgradeFileInfo == null)
            {
                ControlNotification.Singleton.Warning(
                    NotificationPriority.JustOne,
                    _dcuUpgrade,
                    GetString("WarningDCUUpgradeVersionNotSet"),
                    new ControlNotificationSettings(
                        NotificationParts.Hint,
                        HintPosition.Center));

                return;
            }

            if (Plugin.MainServerProvider == null ||
                CgpClient.Singleton.IsConnectionLost(false))
            {
                return;
            }

            var dcusToUpgrade = new List<byte>();
            foreach (DataGridViewRow row in _dgvDCUUpgrading.Rows)
            {
                if (row.Cells[DCU.COLUMNSELECTUPGRADE].Value is bool &&
                    Convert.ToBoolean(row.Cells[DCU.COLUMNSELECTUPGRADE].Value))
                {
                    isSelected = true;

                    var dcuGuid = Guid.Empty;
                    if (row.Cells[DCU.COLUMNIDDCU].Value != null)
                    {
                        dcuGuid = ((Guid) row.Cells[DCU.COLUMNIDDCU].Value);
                    }

                    var dcuOnlineState = Plugin.MainServerProvider.DCUs.GetOnlineStates(dcuGuid);
                    if (dcuOnlineState == OnlineState.Offline)
                    {
                        ControlNotification.Singleton.Error(NotificationPriority.Last, _dgvDCUUpgrading,
                            row.Cells[DCU.COLUMNFULLNAME].Value
                            + ": " + GetString("UpgradeErrorDeviceOffline"),
                            new ControlNotificationSettings(NotificationParts.Hint, HintPosition.LeftTop));
                        continue;
                    }
                    if (dcuOnlineState == OnlineState.Upgrading)
                    {
                        ControlNotification.Singleton.Error(NotificationPriority.Last, _dgvDCUUpgrading,
                            row.Cells[DCU.COLUMNFULLNAME].Value
                            + ": " + GetString("DeviceAlreadyUpgrading"),
                            new ControlNotificationSettings(NotificationParts.Hint, HintPosition.LeftTop));
                        continue;
                    }
                    if (dcuOnlineState == OnlineState.WaitingForUpgrade ||
                        HasUpgradableVersion(Plugin.MainServerProvider.DCUs.GetFirmwareVersion(dcuGuid)))
                    {
                        if ((_mbt == MainBoardVariant.CCU0_ECHELON &&
                             upgradeFileInfo.DcuHWVersion == DCUHWVersion.Echelon) ||
                            (_mbt == MainBoardVariant.CCU0_RS485 && upgradeFileInfo.DcuHWVersion == DCUHWVersion.RS485))
                            dcusToUpgrade.Add((byte) row.Cells[DCU.COLUMNLOGICALADDRESS].Value);
                        else
                            ControlNotification.Singleton.Warning(NotificationPriority.JustOne, _dcuUpgrade,
                                GetString("DCUUpgradeResultIncorrectDCUType"),
                                new ControlNotificationSettings(NotificationParts.Hint, HintPosition.Center));
                    }
                    else
                    {
                        ControlNotification.Singleton.Error(
                            NotificationPriority.Last,
                            _dgvDCUUpgrading,
                            row.Cells[DCU.COLUMNFULLNAME].Value + ": " + GetString("ErrorDCUUnsuportedFirmwareVersion"),
                            new ControlNotificationSettings(
                                NotificationParts.Hint,
                                HintPosition.LeftTop));
                    }
                }
            }
            Exception ex;
            if (dcusToUpgrade.Count > 0)
            {
                List<byte> registeredDCUs;

                Plugin.MainServerProvider.UpgradeDCUs(
                    upgradeFileInfo.Version,
                    Guid.Empty,
                    _editingObject.IPAddress,
                    dcusToUpgrade.ToArray(),
                    out ex,
                    out registeredDCUs);

                if (ex != null || registeredDCUs == null || dcusToUpgrade.Count != registeredDCUs.Count)
                {
                    SafeThread<DCUUpgradeState, Exception>.StartThread(
                        DoChangeDCUsUpgradeProgress,
                        new DCUUpgradeState(
                            _editingObject.IdCCU,
                            dcusToUpgrade.ToArray(),
                            registeredDCUs == null ? null : registeredDCUs.ToArray(),
                            null,
                            null),
                        ex);
                    ShowGridDCUsUpgrade();
                }
            }
            else if (!isSelected)
            {
                ControlNotification.Singleton.Warning(NotificationPriority.JustOne, _dcuUpgrade,
                    GetString("WarningNoDCUSelected"),
                    new ControlNotificationSettings(NotificationParts.Hint, HintPosition.Bottom));
            }
        }

        private string MINIMAL_DCU_VERSION = "1.6.1328";

        private bool HasUpgradableVersion(string version)
        {
            try
            {
                Version currentFW;
                var minimalFW = new Version(MINIMAL_DCU_VERSION);

                if (string.IsNullOrEmpty(version))
                {
                    currentFW = new Version("1.0.0000");
                }
                else
                {
                    currentFW = new Version(version);
                }

                if (currentFW >= minimalFW)
                {
                    return true;
                }
            }
            catch
            {
            }

            return false;
        }

        private void SetDefaultDcuUpgradeFile(object fileName)
        {
            if (fileName == null)
            {
                ControlNotification.Singleton.Warning(NotificationPriority.JustOne, _crUpgrade,
                    GetString("WarningDCUUpgradeVersionNotSet"),
                    new ControlNotificationSettings(NotificationParts.Hint, HintPosition.Center));
                return;
            }
            if (CgpClient.Singleton.IsConnectionLost(false))
                return;
            if (!Plugin.MainServerProvider.SetDefaultDCUUpgradeVersion(_editingObject.IdCCU, fileName.ToString()))
                ControlNotification.Singleton.Warning(NotificationPriority.JustOne, _crUpgrade,
                    GetString("FailedToSetDefaultDCUUpgradeVersion"),
                    new ControlNotificationSettings(NotificationParts.Hint, HintPosition.Center));
            else
                ControlNotification.Singleton.Info(NotificationPriority.JustOne, _crUpgrade,
                    GetString("SetDefaultDCUUpgradeVersionSucceeded"),
                    new ControlNotificationSettings(NotificationParts.Hint, HintPosition.Center));
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ClearStatistics();
            if (!CgpClientMainForm.Singleton.MainIsConnectionLost(false))
            {
                SafeThread.StartThread(ObtainCommunicationStatistic);
            }
        }

        private void ObtainCommunicationStatistic()
        {
            var stat = Plugin.MainServerProvider.CCUs.CommunicationStatistic(_editingObject.IdCCU);
            ShowCommunicationStatistic(stat);
        }

        private void ShowCommunicationStatistic(int[] stat)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<int[]>(ShowCommunicationStatistic), stat);
            }
            else
            {
                if (stat == null)
                    return;

                if (stat.Length == 20)
                {
                    _eServerSended.Text = stat[0] + @" / " + stat[4];
                    _eServerReceived.Text = stat[1] + @" / " + stat[5];
                    _eServerDeserializeError.Text = stat[2].ToString(CultureInfo.InvariantCulture);
                    _eServerReceivedError.Text = stat[3].ToString(CultureInfo.InvariantCulture);
                    _eServerMsgRetry.Text = stat[6].ToString(CultureInfo.InvariantCulture);
                    _eCcuSended.Text = stat[7] + @" / " + stat[11];
                    _eCcuReceived.Text = stat[8] + @" / " + stat[12];
                    _eCcuDeserializeError.Text = stat[9].ToString(CultureInfo.InvariantCulture);
                    _eCcuReceivedError.Text = stat[10].ToString(CultureInfo.InvariantCulture);
                    _eCcuMsgRetry.Text = stat[13].ToString(CultureInfo.InvariantCulture);
                    _eCCUNotAcknowledgedEvents.Text = stat[14].ToString(CultureInfo.InvariantCulture);
                    _eCCUNotAcnknowledgedAutonomousEvents.Text = stat[15].ToString(CultureInfo.InvariantCulture);
                    _eCCUUnprocessedEvents.Text = stat[16].ToString(CultureInfo.InvariantCulture);
                    _eCommandTimeouts.Text = stat[17].ToString(CultureInfo.InvariantCulture);
                    _eServersNotStoredEvents.Text = stat[18].ToString(CultureInfo.InvariantCulture);
                    _eServersNotProcessedEvents.Text = stat[19].ToString(CultureInfo.InvariantCulture);
                }
            }
        }

        private void ClearStatistics()
        {
            _eServerSended.Text = string.Empty;
            _eServerReceived.Text = string.Empty;
            _eServerDeserializeError.Text = string.Empty;
            _eServerReceivedError.Text = string.Empty;
            _eServerMsgRetry.Text = string.Empty;
            _eCcuSended.Text = string.Empty;
            _eCcuReceived.Text = string.Empty;
            _eCcuDeserializeError.Text = string.Empty;
            _eCcuReceivedError.Text = string.Empty;
            _eCcuMsgRetry.Text = string.Empty;
            _eCCUNotAcknowledgedEvents.Text = string.Empty;
            _eCCUNotAcnknowledgedAutonomousEvents.Text = string.Empty;
            _eCCUUnprocessedEvents.Text = string.Empty;
            _eCommandTimeouts.Text = string.Empty;
            _eServersNotStoredEvents.Text = string.Empty;
            _eServersNotProcessedEvents.Text = string.Empty;
        }

        private void _bResetServerSended_Click(object sender, EventArgs e)
        {
            if (!CgpClientMainForm.Singleton.MainIsConnectionLost(false))
            {
                Plugin.MainServerProvider.CCUs.ResetServerSended(_editingObject.IdCCU);
            }
        }

        private void _bServerResetReceived_Click(object sender, EventArgs e)
        {
            if (!CgpClientMainForm.Singleton.MainIsConnectionLost(false))
            {
                Plugin.MainServerProvider.CCUs.ResetServerReceived(_editingObject.IdCCU);
            }
        }

        private void _bServerResetDeserializeError_Click(object sender, EventArgs e)
        {
            if (!CgpClientMainForm.Singleton.MainIsConnectionLost(false))
            {
                Plugin.MainServerProvider.CCUs.ResetServerDeserializeError(_editingObject.IdCCU);
            }
        }

        private void _bServerResetReceivedError_Click(object sender, EventArgs e)
        {
            if (!CgpClientMainForm.Singleton.MainIsConnectionLost(false))
            {
                Plugin.MainServerProvider.CCUs.ResetServerReceivedError(_editingObject.IdCCU);
            }
        }

        private void _bCcuResetSended_Click(object sender, EventArgs e)
        {
            if (!CgpClientMainForm.Singleton.MainIsConnectionLost(false))
            {
                SafeThread.StartThread(ResetCcuSended);
            }
        }

        private void ResetCcuSended()
        {
            Plugin.MainServerProvider.CCUs.ResetCcuSended(_editingObject.IdCCU);
        }

        private void _bCcuResetReceived_Click(object sender, EventArgs e)
        {
            if (!CgpClientMainForm.Singleton.MainIsConnectionLost(false))
            {
                SafeThread.StartThread(ResetCcuReceived);
            }
        }

        private void ResetCcuReceived()
        {
            Plugin.MainServerProvider.CCUs.ResetCcuReceived(_editingObject.IdCCU);
        }

        private void _bCcuResetDeserializeError_Click(object sender, EventArgs e)
        {
            if (!CgpClientMainForm.Singleton.MainIsConnectionLost(false))
            {
                SafeThread.StartThread(ResetCcuDeserializeError);
            }
        }

        private void ResetCcuDeserializeError()
        {
            Plugin.MainServerProvider.CCUs.ResetCcuDeserializeError(_editingObject.IdCCU);
        }

        private void _bCcuResetReceivedError_Click(object sender, EventArgs e)
        {
            if (!CgpClientMainForm.Singleton.MainIsConnectionLost(false))
            {
                SafeThread.StartThread(ResetCcuReceivedError);
            }
        }

        private void ResetCcuReceivedError()
        {
            Plugin.MainServerProvider.CCUs.ResetCcuReceivedError(_editingObject.IdCCU);
        }

        private void _bResetAll_Click(object sender, EventArgs e)
        {
            if (!CgpClientMainForm.Singleton.MainIsConnectionLost(false))
            {
                SafeThread.StartThread(ResetCommunicationStatistic);
            }
        }

        private void ResetCommunicationStatistic()
        {
            Plugin.MainServerProvider.CCUs.ResetCommunicationStatistic(_editingObject.IdCCU);
        }

        private void _bResetServerMsgRetry_Click(object sender, EventArgs e)
        {
            if (!CgpClientMainForm.Singleton.MainIsConnectionLost(false))
            {
                Plugin.MainServerProvider.CCUs.ResetServerMsgRetry(_editingObject.IdCCU);
            }
        }

        private void _bCcuMsgRetry_Click(object sender, EventArgs e)
        {
            if (!CgpClientMainForm.Singleton.MainIsConnectionLost(false))
            {
                SafeThread.StartThread(ResetCcuMsgRetry);
            }
        }

        private void ResetCcuMsgRetry()
        {
            Plugin.MainServerProvider.CCUs.ResetCcuMsgRetry(_editingObject.IdCCU);
        }

        private void RefreshCcuStartsCountClick(object sender, EventArgs e)
        {
            _eCcuStartsCount.Text = string.Empty;
            if (!CgpClientMainForm.Singleton.MainIsConnectionLost(false))
            {
                SafeThread.StartThread(ObtainCcuStartsCount);
            }
        }

        private void ObtainCcuStartsCount()
        {
            var statistics = Plugin.MainServerProvider.CCUs.GetCcuStartsCount(_editingObject.IdCCU);
            if (statistics != null && statistics.Length == 3 && statistics[0] is int && statistics[1] is string &&
                statistics[2] is string)
            {
                ShowCcuStartsCount((int) statistics[0], (string) statistics[1], (string) statistics[2]);
            }
            else
            {
                ShowCcuStartsCount(0, "0", "0");
            }
        }

        private void ShowCcuStartsCount(int ccuStartsCount, string ccuUptime, string ceUptime)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<int, string, string>(ShowCcuStartsCount), ccuStartsCount, ccuUptime,
                    ceUptime);
            }
            else
            {
                _eCcuStartsCount.Text = ccuStartsCount.ToString(CultureInfo.InvariantCulture);
                _eCCUUptime.Text = ccuUptime;
                _eCEUptime.Text = ceUptime;
            }
        }

        private void _bCcuStartsCountReset_Click(object sender, EventArgs e)
        {
            if (!CgpClientMainForm.Singleton.MainIsConnectionLost(false))
            {
                SafeThread.StartThread(ResetCcuStartCounter);
            }
        }

        private void ResetCcuStartCounter()
        {
            Plugin.MainServerProvider.CCUs.ResetCcuStartCounter(_editingObject.IdCCU);
        }

        private void RefreshWinCEImageVersionClick(object sender, EventArgs e)
        {
            _eImageVersion.Text = string.Empty;
            if (!CgpClientMainForm.Singleton.MainIsConnectionLost(false))
            {
                SafeThread.StartThread(ObtainWinCEImageVersion);
            }
        }

        private void ObtainWinCEImageVersion()
        {
            var winCeImageVersion = Plugin.MainServerProvider.CCUs.WinCEImageVersion(_editingObject.IdCCU);
            ShowWinCEImageVersion(winCeImageVersion);
        }

        private void ShowWinCEImageVersion(string winCeFirmwareVersion)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DString2Void(ShowWinCEImageVersion), winCeFirmwareVersion);
            }
            else
            {
                _eImageVersion.Text = winCeFirmwareVersion;
            }
        }

        private void _tpInformation_Enter(object sender, EventArgs e)
        {
            lock (_dcuReloadLock)
            {
                if (_dcuReload)
                {
                    _dcuReload = false;
                    SafeThread.StartThread(ShowDCUs);
                }
            }

            lock (_cardReaderReloadLock)
            {
                if (_cardReaderReload)
                {
                    _cardReaderReload = false;
                    SafeThread.StartThread(ShowCardReaders);
                }
            }
        }

        private void NCASCCUEditForm_Enter(object sender, EventArgs e)
        {
            if (_tcCCU.SelectedTab == _tpInputOutput)
                _tpInputOutput_Enter(sender, e);
        }

        private void _bChangeConfigurePassword_Click(object sender, EventArgs e)
        {
            if (_editingObject == null || _editingObject.IdCCU == Guid.Empty) return;
            if (!CgpClientMainForm.Singleton.MainIsConnectionLost(false))
            {
                if (Plugin.MainServerProvider.CCUs.HasAccessToChangeCcuPassword())
                {
                    string hashPassword;
                    var ccp = new CcuConfigurePassword(LocalizationHelper, _editingObject.IdCCU, Plugin, false);
                    ccp.ShowDialog(out hashPassword);
                    if (!String.IsNullOrEmpty(hashPassword))
                    {
                        SafeThread<string>.StartThread(ChangeCcuConfiguredPassword, hashPassword);
                    }
                }
                else
                {
                    Dialog.Error(GetString("NoAccessChangeCcuPassword"));
                }
            }
        }

        private void ChangeCcuConfiguredPassword(string pwd)
        {
            Plugin.MainServerProvider.CCUs.NewConfigurePassword(_editingObject.IdCCU, pwd);
        }

        private void _bCancelConfigurationPassword_Click(object sender, EventArgs e)
        {
            if (!CgpClientMainForm.Singleton.MainIsConnectionLost(false))
            {
                if (!Plugin.MainServerProvider.CCUs.HasCcuConfigurationPassword(_editingObject.IdCCU))
                {
                    Dialog.Info(GetString("InfoCcuNoConfigurationPassword"));
                    return;
                }

                if (Plugin.MainServerProvider.CCUs.HasAccessToChangeCcuPassword())
                {
                    var ccp = new CcuConfigurePassword(LocalizationHelper, _editingObject.IdCCU, Plugin, true);

                    if (ccp.ShowDialog() == DialogResult.OK)
                        Plugin.MainServerProvider.CCUs.NewConfigurePassword(_editingObject.IdCCU, string.Empty);
                }
                else
                {
                    Dialog.Error(GetString("NoAccessChangeCcuPassword"));
                }
            }
        }

        private bool _crCountChanged;
        private int _lastCrCountValue;

        private void _eMaximumExpectedDCUCount_ValueChanged(object sender, EventArgs e)
        {
            EditTextChanger(sender, e);

            if ((int) _eMaximumExpectedDCUCount.Value != _lastCrCountValue)
                _crCountChanged = true;

            _lastCrCountValue = (int) _eMaximumExpectedDCUCount.Value;
        }

        private void _bRefresh9_Click(object sender, EventArgs e)
        {
            _eCoprocessorActualBuildNumber.Text = string.Empty;
            _eCoprocessorUpgradeResult.Text = string.Empty;
            if (!CgpClientMainForm.Singleton.MainIsConnectionLost(false))
            {
                SafeThread.StartThread(ObtainCoprocessorStatistic);
            }
        }

        private void ObtainCoprocessorStatistic()
        {
            var stat = Plugin.MainServerProvider.CCUs.CoprocessorBuildNumberStatistics(_editingObject.IdCCU);
            if (stat.Length == 3)
            {
                var actualBuildNumber = stat[0];
                if (stat[0] != stat[1])
                {
                    actualBuildNumber += " Expected(" + stat[1] + ")";
                }
                ShowCoprocessorActualBuildNumber(actualBuildNumber);
                ShowCoprocessorUpgradeResult(stat[2]);
            }
        }

        private void ShowCoprocessorActualBuildNumber(string actualBuildNumber)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<string>(ShowCoprocessorActualBuildNumber), actualBuildNumber);
            }
            else
            {
                _eCoprocessorActualBuildNumber.Text = actualBuildNumber;
            }
        }

        private void ShowCoprocessorUpgradeResult(string upgradeResult)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<string>(ShowCoprocessorUpgradeResult), upgradeResult);
            }
            else
            {
                _eCoprocessorUpgradeResult.Text = upgradeResult;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!CgpClientMainForm.Singleton.MainIsConnectionLost(false))
            {
                var cardReaderObjects = new LinkedList<ICardReaderObject>();

                var cardReaders =
                    Plugin.MainServerProvider.CCUs.ActualCcuCardReaders(_editingObject.IdCCU)
                        as IEnumerable<CardReader>;

                if (cardReaders != null)
                {
                    cardReaders = cardReaders.OrderBy(
                        cardReader =>
                            cardReader.ToString());

                    foreach (var cardReader in cardReaders)
                    {
                        cardReaderObjects.AddLast(cardReader);
                    }
                }

                var multiDoorElements =
                    Plugin.MainServerProvider.MultiDoorElements.GetMultiDoorElementsForCcu(_editingObject.IdCCU)
                        as IEnumerable<MultiDoorElement>;

                if (multiDoorElements != null)
                {
                    multiDoorElements = multiDoorElements.OrderBy(
                        multiDoorElement =>
                            multiDoorElement.ToString());

                    foreach (var multiDoorElement in multiDoorElements)
                    {
                        cardReaderObjects.AddLast(multiDoorElement);
                    }
                }

                FillCardReaderCombobox(cardReaderObjects);
            }
        }

        private void FillCardReaderCombobox(IEnumerable<ICardReaderObject> cardReaderObjects)
        {
            _cbCardReaderForSimulation.Items.Clear();

            foreach (var cardReaderObject in cardReaderObjects)
            {
                _cbCardReaderForSimulation.Items.Add(cardReaderObject);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _eResultCardSwiped.Text = string.Empty;
            _eResultCardSwipedDB.Text = string.Empty;

            var cardReaderObject = _cbCardReaderForSimulation.SelectedItem as ICardReaderObject;

            if (cardReaderObject == null)
            {
                _eResultCardSwiped.Text = GetString("TextSelectCardReader");
                return;
            }

            if (_ePersonsCards.Items != null && _ePersonsCards.Visible && _ePersonsCards.Text == string.Empty)
            {
                _eResultCardSwiped.Text = GetString("TextSelectCard");
                return;
            }

            if (!CgpClientMainForm.Singleton.MainIsConnectionLost(false))
            {
                _eResultCardSwiped.Text = string.Empty;
                var crSimContainer = new CardReaderSimulationContainer();
                crSimContainer.idCcu = _editingObject.IdCCU;
                crSimContainer.cardReaderObjectType = cardReaderObject.GetObjectType();
                crSimContainer.idCardReaderObject = (Guid) cardReaderObject.GetId();

                if (_ePersonsCards.Visible)
                {
                    crSimContainer.cardNumber = _ePersonsCards.Text;
                }
                else
                {
                    crSimContainer.cardNumber = _tbmFullCardNumber.Text;
                }

                if (_ePin.Text != string.Empty)
                {
                    crSimContainer.cardHashPin = QuickHashes.GetCRC32String(_ePin.Text);
                    crSimContainer.cardPinLength = _ePin.Text.Length;
                }

                GetDatabaseAccess(
                    cardReaderObject.GetObjectType(),
                    (Guid) cardReaderObject.GetId(),
                    crSimContainer.cardNumber,
                    _ePin.Text);

                SafeThread<CardReaderSimulationContainer>
                    .StartThread(ObtainSimulationCardSwiped, crSimContainer);
            }
        }

        private void GetDatabaseAccess(
            ObjectType objectType,
            Guid idObject,
            string fullCardNunmer,
            string pin)
        {
            var card = CgpClient.Singleton.MainServerProvider.Cards.GetCardByFullNumber(fullCardNunmer);
            if (card == null)
            {
                _eResultCardSwipedDB.Text = @"Unknown card";
                return;
            }
            if (card.State == (byte) CardState.Active || card.State == (byte) CardState.HybridActive)
            {
                if (!card.IsValid)
                {
                    _eResultCardSwipedDB.Text = @"Card is not valid";
                    return;
                }

                Exception error;
                if (card.Person == null)
                {
                    _eResultCardSwipedDB.Text = @"Card not added to person";
                    return;
                }

                if (Plugin.MainServerProvider.ACLPersons.HasAccess(
                    card.Person.IdPerson,
                    objectType,
                    idObject))
                {
                    _eResultCardSwipedDB.Text = @"Access granted";
                }
                else
                {
                    _eResultCardSwipedDB.Text = @"Access DENIED";
                }

                if (!string.IsNullOrEmpty(pin))
                {
                    if (card.PinLength == pin.Length)
                    {
                        if (card.Pin == QuickHashes.GetCRC32String(pin))
                        {
                            _eResultCardSwipedDB.Text += @" PIN valid";
                        }
                        else
                        {
                            _eResultCardSwipedDB.Text += @" INVALID PIN ";
                        }
                    }
                    else
                    {
                        _eResultCardSwipedDB.Text += @" invalid PIN length";
                    }
                }
            }
            else
            {
                _eResultCardSwipedDB.Text = @"Card " + ((CardState) card.State).ToString().ToUpper();
            }
        }

        private static bool CompareCardreaders(ICollection<CardReader> cardReaders, Guid crId)
        {
            if (cardReaders != null)
            {
                foreach (var item in cardReaders)
                {
                    if (item.IdCardReader == crId)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private class CardReaderSimulationContainer
        {
            public Guid idCcu = Guid.Empty;
            public ObjectType cardReaderObjectType;
            public Guid idCardReaderObject;
            public string cardNumber = string.Empty;
            public string cardHashPin = string.Empty;
            public int cardPinLength;
        }

        private void ObtainSimulationCardSwiped(CardReaderSimulationContainer crSimContainer)
        {
            if (crSimContainer == null) return;

            var result = Plugin.MainServerProvider.CCUs.ResultSimulationCardSwiped(
                crSimContainer.idCcu,
                crSimContainer.cardReaderObjectType,
                crSimContainer.idCardReaderObject,
                crSimContainer.cardNumber,
                crSimContainer.cardHashPin,
                crSimContainer.cardPinLength);

            ShowSimulationCardSwipedCardResult(result);
        }

        private void ShowSimulationCardSwipedCardResult(string simResult)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<string>(ShowSimulationCardSwipedCardResult), simResult);
            }
            else
            {
                _eResultCardSwiped.Text = simResult;
            }
        }


        private void _tbmPerson_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                var output = e.Data.GetFormats();
                if (output == null) return;
                SetPerson(e.Data.GetData(output[0]));
            }
            catch
            {
            }
        }

        private void _tbmAlarmArea_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                var output = e.Data.GetFormats();
                if (output == null) return;
                SetAlarmArea(e.Data.GetData(output[0]));
            }
            catch
            {
            }
        }

        private void _tbmFullCardNumber_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                var output = e.Data.GetFormats();
                if (output == null) return;
                SetCard(e.Data.GetData(output[0]));
            }
            catch
            {
            }
        }

        private void _tbmFullCardNumber_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void _tbmPerson_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void _tbmAlarmArea_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void SetPerson(object newPerson)
        {
            try
            {
                var person = newPerson as Person;

                if (person != null)
                {
                    _tbmPerson.Text = person.FirstName + " " + person.Surname;
                    _tbmPerson.Tag = person.IdPerson;
                    _tbmPerson.TextImage = Plugin.GetImageForObjectType(ObjectType.Person);

                    Plugin.AddToRecentList(person);
                }
                else
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _tbmPerson.ImageTextBox,
                        CgpClient.Singleton.LocalizationHelper.GetString("ErrorWrongObjectType"),
                        ControlNotificationSettings.Default);
                }
            }
            catch
            {
            }
        }


        private void SetAlarmArea(object newAlarmArea)
        {
            try
            {
                var alarmArea = newAlarmArea as AlarmArea;
                if (alarmArea != null)
                {
                    _tbmAlarmArea.Text = alarmArea.Name;
                    _tbmAlarmArea.Tag = alarmArea.IdAlarmArea;
                    _tbmAlarmArea.TextImage = Plugin.GetImageForObjectType(ObjectType.AlarmArea);

                    Plugin.AddToRecentList(alarmArea);
                }
                else
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _tbmAlarmArea.ImageTextBox,
                        CgpClient.Singleton.LocalizationHelper.GetString("ErrorWrongObjectType"),
                        ControlNotificationSettings.Default);
                }
            }
            catch
            {
            }
        }

        private void SetCard(object newCard)
        {
            try
            {
                var card = newCard as Card;

                if (card != null)
                {
                    _tbmFullCardNumber.Text = card.FullCardNumber;
                    _tbmFullCardNumber.TextImage = Plugin.GetImageForObjectType(card.GetSubTypeImageString("State"));

                    AddPersonsCards(null);
                    Plugin.AddToRecentList(card);

                    return;
                }

                var person = newCard as Person;

                if (person != null)
                {
                    if (person.Cards != null && person.Cards.Count > 0)
                    {
                        _tbmFullCardNumber.Text = person.ToString();
                        _tbmFullCardNumber.TextImage = Plugin.GetImageForObjectType(ObjectType.Person);

                        AddPersonsCards(person);
                        Plugin.AddToRecentList(person);
                    }
                    else
                    {
                        ControlNotification.Singleton.Error(NotificationPriority.JustOne,
                            _tbmFullCardNumber.ImageTextBox,
                            GetString("ErrorPersonHasNotAddedCards"), ControlNotificationSettings.Default);
                    }

                    return;
                }

                ControlNotification.Singleton.Error(
                    NotificationPriority.JustOne,
                    _tbmFullCardNumber.ImageTextBox,
                    CgpClient.Singleton.LocalizationHelper.GetString("ErrorWrongObjectType"),
                    ControlNotificationSettings.Default);
            }
            catch
            {
            }
        }

        private void AddPerson()
        {
            if (CgpClientMainForm.Singleton.MainIsConnectionLost(false)) return;

            try
            {
                Exception error;

                IList<IModifyObject> listPersons =
                    CgpClient.Singleton.MainServerProvider.Persons
                        .ListModifyObjects(out error);

                if (error != null)
                    throw error;

                var formAdd =
                    new ListboxFormAdd(
                        listPersons,
                        CgpClient.Singleton.LocalizationHelper.GetString("PersonsFormPersonsForm"),
                        true);

                IModifyObject outPerson;
                formAdd.ShowDialog(out outPerson);

                if (outPerson != null)
                {
                    var person = CgpClient.Singleton.MainServerProvider.Persons.GetObjectById(outPerson.GetId);
                    if (person != null)
                    {
                        _tbmPerson.Text = person.FirstName + " " + person.Surname;
                        _tbmPerson.Tag = person.IdPerson;
                        _tbmPerson.TextImage = Plugin.GetImageForObjectType(ObjectType.Person);

                    }
                }
            }
            catch
            {
            }
        }

        private void AddAlarmArea()
        {
            if (CgpClientMainForm.Singleton.MainIsConnectionLost(false)) return;

            try
            {
                Exception error;
                ICollection<IModifyObject> listAlarmAreas =
                    Plugin.MainServerProvider.AlarmAreas.ListModifyObjects(out error);
                if (error != null) throw error;

                var formAdd = new ListboxFormAdd(listAlarmAreas,
                    CgpClient.Singleton.LocalizationHelper.GetString("AlarmAreasFormAlarmAreasForm"), true);
                IModifyObject outAlarmArea;
                formAdd.ShowDialog(out outAlarmArea);

                if (outAlarmArea != null)
                {
                    var alarmArea = Plugin.MainServerProvider.AlarmAreas.GetObjectById(outAlarmArea.GetId);
                    if (alarmArea != null)
                    {
                        _tbmAlarmArea.Text = alarmArea.Name;
                        _tbmAlarmArea.Tag = alarmArea.IdAlarmArea;
                        _tbmAlarmArea.TextImage = Plugin.GetImageForObjectType(ObjectType.AlarmArea);
                    }
                }
            }
            catch
            {
            }
        }

        private void AddCard()
        {
            if (CgpClientMainForm.Singleton.MainIsConnectionLost(false)) return;

            try
            {
                var listObjects = new List<IModifyObject>();
                Exception error;

                var listPersons = CgpClient.Singleton.MainServerProvider.Persons.ListModifyPersonsWithCards(out error);
                if (error != null) throw error;

                if (listPersons != null && listPersons.Count > 0)
                {
                    listObjects.AddRange(listPersons);
                }

                var listCards = CgpClient.Singleton.MainServerProvider.Cards.ListModifyObjects(out error);
                if (error != null) throw error;

                if (listCards != null && listCards.Count > 0)
                {
                    listObjects.AddRange(listCards);
                }

                var formAdd = new ListboxFormAdd(listObjects, GetString("NCASCCUEditForm_AddCardsAndPersons"), true);
                IModifyObject outObject;
                formAdd.ShowDialog(out outObject);

                if (outObject != null)
                {
                    if (outObject is CardModifyObj)
                    {
                        var card = CgpClient.Singleton.MainServerProvider.Cards.GetObjectById(outObject.GetId);
                        if (card != null)
                        {
                            _tbmFullCardNumber.Text = card.FullCardNumber;
                            _tbmFullCardNumber.TextImage =
                                Plugin.GetImageForObjectType(card.GetSubTypeImageString("State"));

                            AddPersonsCards(null);
                        }
                    }
                    else
                    {
                        var person = CgpClient.Singleton.MainServerProvider.Persons.GetObjectById(outObject.GetId);
                        if (person != null)
                        {
                            _tbmFullCardNumber.Text = person.ToString();
                            _tbmFullCardNumber.TextImage = Plugin.GetImageForObjectType(ObjectType.Person);

                            AddPersonsCards(person);
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void AddPersonsCards(Person person)
        {
            if (person != null && person.Cards != null && person.Cards.Count > 0)
            {

                _ePersonsCards.Items.Clear();

                foreach (var card in person.Cards)
                {
                    if (card != null)
                    {
                        _ePersonsCards.Items.Add(card.FullCardNumber);
                    }
                }

                _lPersonsCards.Visible = true;
                _ePersonsCards.Visible = true;
                _ePersonsCards.Text = string.Empty;
            }
            else
            {
                _ePersonsCards.Items.Clear();
                _lPersonsCards.Visible = false;
                _ePersonsCards.Visible = false;
            }
        }

        private void _tbmFullCardNumber_ButtonPopupMenuItemClick(ToolStripItem item, int index)
        {
            if (item.Name == "_tsiModify1")
            {
                AddCard();
            }
        }

        private void FillCbMainboardType()
        {
            _cbMainboarType.Items.Clear();
            foreach (MainBoardVariant mbt in Enum.GetValues(typeof (MainBoardVariant)))
            {
                _cbMainboarType.Items.Add(mbt);
            }
            _cbMainboarType.SelectedItem = MainBoardVariant.Unknown;
        }

        private void _cbMainboarType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_cbMainboarType.SelectedItem is MainBoardVariant)
            {
                _mbt = (MainBoardVariant) _cbMainboarType.SelectedItem;
                RefreshFormByMainboardType();
            }
        }

        private void RefreshAvailableCRUpgradeList()
        {
            if (_configured != CCUConfigurationState.ConfiguredForThisServer)
                return;

            if (CgpClient.Singleton.IsConnectionLost(false))
                return;

            string delimeter;
            ICollection<string> crVersions = Plugin.MainServerProvider.GetAvailableCRUpgrades(out delimeter);
            var crVersionsLocalised = new List<CRUpgradeFileInfo>();

            if (crVersions != null && crVersions.Count > 0)
            {
                foreach (var version in crVersions)
                {
                    if (!version.Contains(delimeter))
                        continue;
                    try
                    {
                        crVersionsLocalised.Add(new CRUpgradeFileInfo(version, delimeter[0]));
                    }
                    catch
                    {
                    }
                }

                var source = new BindingSource();
                source.DataSource = crVersionsLocalised.ToArray();
                Invoke((MethodInvoker) delegate
                {
                    //_lbAvailableCRUpgradeVersion.DataSource = source;
                    _crUpgradeFiles.Files = crVersionsLocalised.Cast<object>().ToList();
                });
            }
            else
                Invoke((MethodInvoker) delegate
                {
                    //_lbAvailableCRUpgradeVersion.DataSource = null;
                    //_lbAvailableCRUpgradeVersion.Items.Clear();
                });
        }

        private void _bUpgradeCRRefresh_Click(object sender, EventArgs e)
        {
            ShowCRsUpgrade();
        }

        private void UpgradeCr(object fileName)
        {
            if (!Dialog.Question(string.Format(GetString("QuestionStartUpgrading"), "CR", fileName)))
                return;

            var isSelected = false;

            if (fileName == null)
            {
                ControlNotification.Singleton.Warning(NotificationPriority.JustOne, _crUpgrade,
                    GetString("WarningCRUpgradeVersionNotSet"),
                    new ControlNotificationSettings(NotificationParts.Hint, HintPosition.Center));
                return;
            }

            if (Plugin.MainServerProvider == null || CgpClient.Singleton.IsConnectionLost(false))
                return;

            var dcusCardReadersToUpgrade = new Dictionary<byte, List<byte>>();
            var ccusCardReadersToUpgrade = new List<Guid>();
            foreach (DataGridViewRow row in _dgvCRUpgrading.Rows)
            {
                if (row.Cells[CardReader.COLUMNSELECTUPGRADE].Value is bool &&
                    Convert.ToBoolean(row.Cells[CardReader.COLUMNSELECTUPGRADE].Value))
                {
                    isSelected = true;

                    var crGuid = Guid.Empty;
                    if (row.Cells[CardReader.COLUMNIDCARDREADER].Value != null)
                    {
                        crGuid = ((Guid) row.Cells[CardReader.COLUMNIDCARDREADER].Value);
                    }

                    var crOnlineState = Plugin.MainServerProvider.CardReaders.GetOnlineStates(crGuid);
                    if (crOnlineState == OnlineState.Upgrading)
                    {
                        ControlNotification.Singleton.Error(NotificationPriority.Last, _dgvCRUpgrading,
                            row.Cells[CardReader.COLUMNNAME].Value
                            + ": " + GetString("DeviceAlreadyUpgrading"),
                            new ControlNotificationSettings(NotificationParts.Hint, HintPosition.LeftTop));
                        continue;
                    }

                    var crHwVersion = ((CardReader)_bindingSourceCRUpgrades[row.Index]).CardReaderHardware;

                    string upgradeFileFwVersion =
                        ((CRUpgradeFileInfo)fileName).Version;

                    var upgradeFileHwVersion =
                        ((CRUpgradeFileInfo)fileName).CrHWVersion;

                    if ((byte)upgradeFileHwVersion != crHwVersion)
                    {
                        Dialog.Error(GetString("ErrorCrUpgradeHwVersionIncorrect", row.Cells[CardReader.COLUMNNAME].Value.ToString()));
                        continue;
                    }

                    if (!string.IsNullOrEmpty(upgradeFileFwVersion)
                        && upgradeFileFwVersion.Length >= 2
                        && upgradeFileFwVersion.Substring(0, 2) == "56"
                        && (upgradeFileHwVersion == CRHWVersion.ProximityPremiumCCR
                        || upgradeFileHwVersion == CRHWVersion.SmartPremiumCCR))
                    {
                        string actualFwVersion = row.Cells["FwVersion"].Value.ToString();

                        if (!string.IsNullOrEmpty(actualFwVersion)
                            && actualFwVersion.Length >= 2
                            && actualFwVersion.Substring(0, 2) == "55"
                            && actualFwVersion != "55.90")
                        {
                            Dialog.Warning(GetString("WarningCrUpgradeFwVersionFailed", row.Cells[CardReader.COLUMNNAME].Value.ToString()));
                            continue;
                        }
                    }

                    var dcu = row.Cells[CardReader.COLUMNDCU].Value as DCU;

                    if (dcu != null)
                    {
                        if (!dcusCardReadersToUpgrade.ContainsKey(dcu.LogicalAddress))
                            dcusCardReadersToUpgrade.Add(dcu.LogicalAddress, new List<byte>());
                        dcusCardReadersToUpgrade[dcu.LogicalAddress].Add(
                            (byte) row.Cells[CardReader.COLUMNADDRESS].Value);
                    }
                    else
                    {
                        if (!ccusCardReadersToUpgrade.Contains(crGuid))
                        {
                            ccusCardReadersToUpgrade.Add(crGuid);
                        }
                    }
                }
            }
            if (ccusCardReadersToUpgrade.Count > 0 || dcusCardReadersToUpgrade.Count > 0)
            {
                var upgradeFileInfo = (CRUpgradeFileInfo)fileName;

                Exception ex;
                if (
                    !Plugin.MainServerProvider.UpgradeCRs(upgradeFileInfo.Version, (byte) upgradeFileInfo.CrHWVersion,
                        Guid.Empty, _editingObject.IPAddress,
                        ccusCardReadersToUpgrade, dcusCardReadersToUpgrade, out ex))
                {
                    var dcus = new Dictionary<byte, Guid>();
                    foreach (var dcuObject in _editingObject.DCUs)
                    {
                        dcus.Add(dcuObject.LogicalAddress, dcuObject.IdDCU);
                    }

                    if (dcusCardReadersToUpgrade.Count > 0)
                    {
                        foreach (var crs in dcusCardReadersToUpgrade)
                        {
                            if (crs.Value != null)
                            {
                                DoChangeCRsUpgradeProgress(
                                    new CRUpgradeState(_editingObject.IdCCU, dcus[crs.Key], crs.Value.ToArray(), null,
                                        null, null), ex);
                            }
                        }
                    }

                    if (ccusCardReadersToUpgrade.Count > 0)
                    {
                        var crAddresses = new Dictionary<Guid, byte>();
                        foreach (var cardReader in _editingObject.CardReaders)
                        {
                            crAddresses.Add(cardReader.IdCardReader, cardReader.Address);
                        }

                        var addresses = new List<byte>();
                        foreach (var crGuid in ccusCardReadersToUpgrade)
                        {
                            byte address;
                            if (crAddresses.TryGetValue(crGuid, out address))
                            {
                                addresses.Add(address);
                            }
                        }

                        if (crAddresses.Count > 0)
                        {
                            DoChangeCRsUpgradeProgress(
                                new CRUpgradeState(_editingObject.IdCCU, null, addresses.ToArray(), null, null, null),
                                ex);
                        }
                    }
                }
            }
            else if (!isSelected)
            {
                ControlNotification.Singleton.Warning(NotificationPriority.JustOne, _crUpgrade,
                    GetString("WarningNoCRSelected"),
                    new ControlNotificationSettings(NotificationParts.Hint, HintPosition.Bottom));
            }
        }

        private void DoChangeCRsUpgradeProgress(CRUpgradeState upgradeState, Exception ex)
        {
            if (upgradeState == null)
                return;

            if (InvokeRequired)
                Invoke(new Action<CRUpgradeState, Exception>(DoChangeCRsUpgradeProgress), upgradeState, ex);
            else
            {
                foreach (DataGridViewRow row in _dgvCRUpgrading.Rows)
                {
                    var editRowValues = false;
                    var cardRaderDCU = row.Cells[CardReader.COLUMNDCU].Value as DCU;

                    if ((cardRaderDCU == null && upgradeState.DCUGuid == null) ||
                        (cardRaderDCU != null && cardRaderDCU.IdDCU == upgradeState.DCUGuid))
                    {
                        editRowValues = row.Cells[CardReader.COLUMNADDRESS].Value != null &&
                                        upgradeState.CardReaderAddresses.Contains(
                                            (byte) row.Cells[CardReader.COLUMNADDRESS].Value);
                    }

                    if (editRowValues)
                    {
                        var onlineState = OnlineState.Online;

                        if (ex != null)
                        {
                            ControlNotification.Singleton.Error(NotificationPriority.Last, _dgvCRUpgrading,
                                row.Cells[CardReader.COLUMNNAME].Value + ": " + GetString("DeviceUpgradeFailed") + ":" +
                                ex.Message,
                                new ControlNotificationSettings(NotificationParts.Hint, HintPosition.LeftTop));
                        }

                        if (upgradeState.UpgradeResult != null)
                        {
                            var upgradeResult = (CRUpgradeResult) upgradeState.UpgradeResult;
                            if (upgradeResult == CRUpgradeResult.Success)
                            {
                                ControlNotification.Singleton.Info(NotificationPriority.Last, _dgvCRUpgrading,
                                    row.Cells[CardReader.COLUMNNAME].Value + ": " +
                                    GetString("DeviceUpgradeSucceeded"),
                                    new ControlNotificationSettings(NotificationParts.Hint, HintPosition.LeftTop));
                                row.Cells[CardReader.COLUMNSELECTUPGRADE].Value = false;
                            }
                            else
                            {
                                ControlNotification.Singleton.Error(NotificationPriority.Last, _dgvCRUpgrading,
                                    row.Cells[CardReader.COLUMNNAME].Value + ": " +
                                    GetString("CRUpgradeResult" + upgradeResult),
                                    new ControlNotificationSettings(NotificationParts.Hint, HintPosition.LeftTop));
                            }
                        }

                        if (upgradeState.UnpackErrorCode != null)
                        {
                            var unpackFailedCode = (UnpackPackageFailedCode) upgradeState.UnpackErrorCode;
                            ControlNotification.Singleton.Error(NotificationPriority.Last, _dgvCRUpgrading,
                                row.Cells[CardReader.COLUMNNAME].Value + ": " +
                                GetString("UnpackUpgradePackageFailed" + unpackFailedCode),
                                new ControlNotificationSettings(NotificationParts.Hint, HintPosition.LeftTop));
                        }

                        if (upgradeState.Percents != null)
                        {
                            if (upgradeState.Percents < 0)
                            {
                                ControlNotification.Singleton.Error(NotificationPriority.Last, _dgvCRUpgrading,
                                    row.Cells[CardReader.COLUMNNAME].Value + ": " + GetString("DeviceUpgradeFailed"),
                                    new ControlNotificationSettings(NotificationParts.Hint, HintPosition.LeftTop));
                                upgradeState.Percents = 0;
                                continue;
                            }
                            if (upgradeState.Percents == 100)
                            {
                                upgradeState.Percents = 0;
                            }
                            else
                                onlineState = OnlineState.Upgrading;
                        }

                        if (upgradeState.Percents == null)
                        {
                            row.Cells[CardReader.COLUMNUPGRADEPROGRESS].Value = 0;
                        }
                        else
                        {
                            if (upgradeState.Percents == 0 || row.Cells[CardReader.COLUMNUPGRADEPROGRESS].Value == null ||
                                upgradeState.Percents > (int) row.Cells[CardReader.COLUMNUPGRADEPROGRESS].Value)
                            {
                                row.Cells[CardReader.COLUMNUPGRADEPROGRESS].Value = upgradeState.Percents;
                            }
                        }

                        row.Cells[CardReader.COLUMNONLINESTATE].Value = GetString(onlineState.ToString());
                    }
                }
            }
        }

        private bool _firsTimeDgCRUpgradeShow = true;

        private void DgCRUpgradingVisibleChanged(object sender, EventArgs e)
        {
            if (_firsTimeDgCRUpgradeShow)
            {
                ShowGridCRUpgrade();
                _firsTimeDgCRUpgradeShow = false;
            }
        }

        private void _chbCRSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in _dgvCRUpgrading.Rows)
            {
                if (row.Cells[CardReader.COLUMNONLINESTATE].Value != null)
                {
                    row.Cells[CardReader.COLUMNSELECTUPGRADE].Value =
                        row.Cells[CardReader.COLUMNONLINESTATE].Value.ToString() ==
                        GetString(OnlineState.Online.ToString()) &&
                        _chbCRSelectAll.Checked;
                }
            }
        }

        private void _dgvCRUpgrading_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex < 0 || e.RowIndex < 0)
                return;

            if (e.ColumnIndex == _dgvCRUpgrading.Columns[CardReader.COLUMNSELECTUPGRADE].Index)
                return;

            if (CgpClient.Singleton.IsConnectionLost(false)) return;

            if (_bindingSourceCRUpgrades == null || _bindingSourceCRUpgrades.Count == 0 ||
                _bindingSourceCRUpgrades.Current == null)
                return;

            NCASCardReadersForm.Singleton.OpenEditForm(_bindingSourceCRUpgrades.Current as CardReader);
        }

        private void _bPrecreateCardReader_Click(object sender, EventArgs e)
        {
            var cardReader = new CardReader();
            cardReader.Address = (byte) _eMaximumExpectedDCUCount.Value;
            cardReader.CCU = _editingObject;
            cardReader.Port = "COM4";
            cardReader.EnableParentInFullName = Plugin.MainServerProvider.GetEnableParentInFullName();
            NCASCardReadersForm.Singleton.OpenInsertFromEdit(ref cardReader, ShowCardReaders);
        }

        private void ShowCardReaders(object obj)
        {
            var ccu = Plugin.MainServerProvider.CCUs.GetObjectById(_editingObject.IdCCU);
            if (ccu != null)
            {
                _editingObject.CardReaders = ccu.CardReaders;
                ShowCardReaders();
            }
        }

        private void _bSoftReset_Click(object sender, EventArgs e)
        {
            if (!Dialog.WarningQuestion(GetString("QuestionSoftResetCCU")))
                return;

            SafeThread.StartThread(SoftResetCcu);
        }

        private void SoftResetCcu()
        {
            if (Plugin.MainServerProvider != null)
            {
                if (!Plugin.MainServerProvider.CCUs.SoftResetCCU(_editingObject.IdCCU))
                    SofrResetCcuError();
            }
        }

        private void SofrResetCcuError()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(SofrResetCcuError));
                return;
            }

            ControlNotification.Singleton.Error(NotificationPriority.JustOne, _bSoftReset,
                GetString("ErrorDeviceResetFailed"),
                new ControlNotificationSettings(NotificationParts.Hint, HintPosition.Center));
        }

        private void _bPrecreateInput_Click(object sender, EventArgs e)
        {
            if (_inputInsertForm == null)
                _inputInsertForm = new NCASInputEditForm(new Input(), ShowOptionsEditForm.Insert, this);

            if (_inputInsertForm.Visible)
            {
                _inputInsertForm.Focus();
                return;
            }
            _inputInsertForm = new NCASInputEditForm(new Input(), ShowOptionsEditForm.Insert, this);

            _inputInsertForm.CcuInput = true;
            _inputInsertForm.Show();
            _inputInsertForm.SetFixedCCUParent(_editingObject);
        }

        private void _bPrecreateOutput_Click(object sender, EventArgs e)
        {
            if (_outputInsertForm == null)
                _outputInsertForm = new NCASOutputEditForm(new Output(), ShowOptionsEditForm.Insert, this);

            if (_outputInsertForm.Visible)
            {
                _outputInsertForm.Focus();
                return;
            }
            _outputInsertForm = new NCASOutputEditForm(new Output(), ShowOptionsEditForm.Insert, this);

            _outputInsertForm.CcuOutput = true;
            _outputInsertForm.Show();
            _outputInsertForm.SetFixedCCUParent(_editingObject);
        }

        private void _tbmTimeZonesDailyPlans_ButtonPopupMenuItemClick(ToolStripItem item, int index)
        {
            if (item.Name == "_tsiModify2")
            {
                AddTimeZonesDailyPlans();
            }
        }

        private void AddTimeZonesDailyPlans()
        {
            if (CgpClientMainForm.Singleton.MainIsConnectionLost(false)) return;

            try
            {
                var listTimeZonesDailyPlans = new List<IModifyObject>();
                Exception error;
                var listTimeZones = CgpClient.Singleton.MainServerProvider.TimeZones.ListModifyObjects(out error);
                if (error != null) throw error;
                listTimeZonesDailyPlans.AddRange(listTimeZones);

                var listSecurityTimeZones = Plugin.MainServerProvider.SecurityTimeZones.ListModifyObjects(out error);
                if (error != null) throw error;
                listTimeZonesDailyPlans.AddRange(listSecurityTimeZones);

                var listDailyPlans = CgpClient.Singleton.MainServerProvider.DailyPlans.ListModifyObjects(out error);
                if (error != null) throw error;
                DbsSupport.TranslateDailyPlans(listDailyPlans);
                listTimeZonesDailyPlans.AddRange(listDailyPlans);

                var listSecurityDailyPlans = Plugin.MainServerProvider.SecurityDailyPlans.ListModifyObjects(out error);
                if (error != null) throw error;
                listTimeZonesDailyPlans.AddRange(listSecurityDailyPlans);

                var formAdd = new ListboxFormAdd(listTimeZonesDailyPlans,
                    GetString("NCASCCUEditForm_AddTimeZoneDailyPlan"), true);
                IModifyObject outTimeZonesDailyPlans;
                formAdd.ShowDialog(out outTimeZonesDailyPlans);

                if (outTimeZonesDailyPlans != null)
                {
                    switch (outTimeZonesDailyPlans.GetOrmObjectType)
                    {
                        case ObjectType.TimeZone:
                            var timeZone =
                                CgpClient.Singleton.MainServerProvider.TimeZones.GetObjectById(
                                    outTimeZonesDailyPlans.GetId);
                            SetTimeZoneDailyPlan(timeZone);
                            break;
                        case ObjectType.DailyPlan:
                            var dailyPlan =
                                CgpClient.Singleton.MainServerProvider.DailyPlans.GetObjectById(
                                    outTimeZonesDailyPlans.GetId);
                            SetTimeZoneDailyPlan(dailyPlan);
                            break;
                        case ObjectType.SecurityTimeZone:
                            var securityTimeZone =
                                Plugin.MainServerProvider.SecurityTimeZones.GetObjectById(outTimeZonesDailyPlans.GetId);
                            SetTimeZoneDailyPlan(securityTimeZone);
                            break;
                        case ObjectType.SecurityDailyPlan:
                            var securityDailyPlan =
                                Plugin.MainServerProvider.SecurityDailyPlans.GetObjectById(outTimeZonesDailyPlans.GetId);
                            SetTimeZoneDailyPlan(securityDailyPlan);
                            break;
                    }
                }
            }
            catch
            {
            }
        }

        private void SetTimeZoneDailyPlan(AOrmObject timeZoneDailyPlan)
        {
            if (timeZoneDailyPlan != null)
            {
                _timeZoneDailyPlan = timeZoneDailyPlan;

                var plan = timeZoneDailyPlan as DailyPlan;
                if (plan != null)
                {
                    var actDailyPlan = plan;

                    var dpl = new DailyPlanList(actDailyPlan);
                    _tbmTimeZonesDailyPlans.Text = dpl.ToString();
                }
                else
                {
                    _tbmTimeZonesDailyPlans.Text = timeZoneDailyPlan.ToString();
                }

                _tbmTimeZonesDailyPlans.TextImage = Plugin.GetImageForObjectType(timeZoneDailyPlan.GetObjectType());
            }
        }

        private void _bGetState_Click(object sender, EventArgs e)
        {
            if (_timeZoneDailyPlan == null)
            {
                ControlNotification.Singleton.Error(NotificationPriority.JustOne, _tbmTimeZonesDailyPlans.ImageTextBox,
                    GetString("ErrorInsertTimeZoneDailyPlan"), ControlNotificationSettings.Default);

                return;
            }

            _eStateOnServer.Text = string.Empty;
            _eStateOnCCU.Text = string.Empty;

            SafeThread.StartThread(GetStateTimeZoneDailyPlan);
        }

        private void GetStateTimeZoneDailyPlan()
        {
            try
            {
                if (_timeZoneDailyPlan == null)
                    return;

                var stateOnServer = State.Unknown;

                switch (_timeZoneDailyPlan.GetObjectType())
                {
                    case ObjectType.TimeZone:
                        if (
                            CgpClient.Singleton.MainServerProvider.GetTimeZoneActualStatus(
                                (Guid) _timeZoneDailyPlan.GetId()) == 1)
                            stateOnServer = State.On;
                        else
                            stateOnServer = State.Off;
                        break;
                    case ObjectType.DailyPlan:
                        if (
                            CgpClient.Singleton.MainServerProvider.GetDailyPlanActualStatus(
                                (Guid) _timeZoneDailyPlan.GetId()) == 1)
                            stateOnServer = State.On;
                        else
                            stateOnServer = State.Off;
                        break;
                    case ObjectType.SecurityTimeZone:
                        var stateSecurityTimeZone =
                            (SecurityLevel4SLDP)
                                Plugin.MainServerProvider.GetSecurityTimeZoneActualStatus(
                                    (Guid) _timeZoneDailyPlan.GetId());
                        switch (stateSecurityTimeZone)
                        {
                            case SecurityLevel4SLDP.unlocked:
                                stateOnServer = State.unlocked;
                                break;
                            case SecurityLevel4SLDP.locked:
                                stateOnServer = State.locked;
                                break;
                            case SecurityLevel4SLDP.card:
                                stateOnServer = State.card;
                                break;
                            case SecurityLevel4SLDP.cardpin:
                                stateOnServer = State.cardpin;
                                break;
                            case SecurityLevel4SLDP.code:
                                stateOnServer = State.code;
                                break;
                            case SecurityLevel4SLDP.codeorcard:
                                stateOnServer = State.codecard;
                                break;
                            case SecurityLevel4SLDP.codeorcardpin:
                                stateOnServer = State.codecardpin;
                                break;
                            case SecurityLevel4SLDP.togglecard:
                                stateOnServer = State.togglecard;
                                break;
                            case SecurityLevel4SLDP.togglecardpin:
                                stateOnServer = State.togglecardpin;
                                break;
                        }
                        break;
                    case ObjectType.SecurityDailyPlan:
                        var stateSecurityDailyPlan =
                            (SecurityLevel4SLDP)
                                Plugin.MainServerProvider.GetSecurityDailyPlanActualStatus(
                                    (Guid) _timeZoneDailyPlan.GetId());
                        switch (stateSecurityDailyPlan)
                        {
                            case SecurityLevel4SLDP.unlocked:
                                stateOnServer = State.unlocked;
                                break;
                            case SecurityLevel4SLDP.locked:
                                stateOnServer = State.locked;
                                break;
                            case SecurityLevel4SLDP.card:
                                stateOnServer = State.card;
                                break;
                            case SecurityLevel4SLDP.cardpin:
                                stateOnServer = State.cardpin;
                                break;
                            case SecurityLevel4SLDP.code:
                                stateOnServer = State.code;
                                break;
                            case SecurityLevel4SLDP.codeorcard:
                                stateOnServer = State.codecard;
                                break;
                            case SecurityLevel4SLDP.codeorcardpin:
                                stateOnServer = State.codecardpin;
                                break;
                            case SecurityLevel4SLDP.togglecard:
                                stateOnServer = State.togglecard;
                                break;
                            case SecurityLevel4SLDP.togglecardpin:
                                stateOnServer = State.togglecardpin;
                                break;
                        }
                        break;
                }

                State stateOnCCU = Plugin.MainServerProvider.CCUs.GetTimeZoneDailyPlanState(_editingObject.IdCCU,
                    _timeZoneDailyPlan.GetObjectType(), (Guid) _timeZoneDailyPlan.GetId());

                ShowStateTimeZoneDailyPlan(stateOnServer, stateOnCCU);
            }
            catch
            {
            }
        }

        private void ShowStateTimeZoneDailyPlan(State stateOnServer, State stateOnCCU)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<State, State>(ShowStateTimeZoneDailyPlan), stateOnServer, stateOnCCU);
            }
            else
            {
                _eStateOnServer.Text = GetString("State_" + stateOnServer);
                _eStateOnCCU.Text = GetString("State_" + stateOnCCU);
            }
        }

        private void _tbmTimeZonesDailyPlans_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void _tbmTimeZonesDailyPlans_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                var output = e.Data.GetFormats();
                if (output == null) return;
                DragDropTimeZoneDailyPlan(e.Data.GetData(output[0]));
            }
            catch
            {
            }
        }

        private void DragDropTimeZoneDailyPlan(object timeZoneDailyPlan)
        {
            try
            {
                if (timeZoneDailyPlan is TimeZone || timeZoneDailyPlan is DailyPlan ||
                    timeZoneDailyPlan is SecurityTimeZone || timeZoneDailyPlan is SecurityDailyPlan)
                {
                    SetTimeZoneDailyPlan(timeZoneDailyPlan as AOrmObject);
                    Plugin.AddToRecentList(timeZoneDailyPlan);
                }
                else
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne,
                        _tbmTimeZonesDailyPlans.ImageTextBox,
                        CgpClient.Singleton.LocalizationHelper.GetString("ErrorWrongObjectType"),
                        ControlNotificationSettings.Default);
                }
            }
            catch
            {
            }
        }

        private void _tbmAlarmArea_ImageTextBox_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                var alarmArea = Plugin.MainServerProvider.AlarmAreas.GetObjectById((Guid) _tbmAlarmArea.Tag);
                NCASAlarmAreasForm.Singleton.OpenEditForm(alarmArea);
            }
            catch
            {
            }
        }

        private void _tbmPerson_ImageTextBox_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                var person = CgpClient.Singleton.MainServerProvider.Persons.GetObjectById((Guid) _tbmPerson.Tag);
                PersonsForm.Singleton.OpenEditForm(person);
            }
            catch
            {
            }
        }

        private void _tbmTimeZonesDailyPlans_ImageTextBox_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                if (_timeZoneDailyPlan == null)
                    return;

                switch (_timeZoneDailyPlan.GetObjectType())
                {
                    case ObjectType.TimeZone:
                        TimeZonesForm.Singleton.OpenEditForm(_timeZoneDailyPlan as TimeZone);
                        break;
                    case ObjectType.DailyPlan:
                        DailyPlansForm.Singleton.OpenEditForm(_timeZoneDailyPlan as DailyPlan);
                        break;
                    case ObjectType.SecurityTimeZone:
                        NCASSecurityTimeZonesForm.Singleton.OpenEditForm(_timeZoneDailyPlan as SecurityTimeZone);
                        break;
                    case ObjectType.SecurityDailyPlan:
                        NCASSecurityDailyPlansForm.Singleton.OpenEditForm(_timeZoneDailyPlan as SecurityDailyPlan);
                        break;
                }
            }
            catch
            {
            }
        }

        private void UpsMonitorValuesChanged(string ipAddress, CUps2750Values values)
        {
            if (_editingObject != null && !string.IsNullOrEmpty(_editingObject.IPAddress) &&
                _editingObject.IPAddress == ipAddress)
            {
                RefreshUpsMonitor(values);
            }
        }

        private void RefreshUpsMonitor(CUps2750Values values)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<CUps2750Values>(RefreshUpsMonitor), values);
            }
            else
            {
                LabelColorAlarm(_lOutputFuse, values.m_bOutputFuse);
                LabelColorAlarm(_lOutputOutOfTollerance, values.m_bOutputPowerOutOfTolerance);
                LabelColorAlarm(_lPrimaryPowerMissing, values.m_bPrimaryPowerMissing);
                LabelColorAlarm(_lBatteryFault, values.m_bBatteryFault);
                LabelColorAlarm(_lBatteryEmpty, values.m_bBatteryEmpty);
                LabelColorAlarm(_lBatteryFuse, values.m_bBatteryFuse);
                LabelColorAlarm(_lOvertemperature, values.m_bOvertemperature);
                LabelColorAlarm(_lTamper, values.m_bTamper);

                m_labVoltageInput.Text = values._diVoltageInput + @" V";
                m_labVoltageOutput.Text = values._diVoltageOutput + @" V";
                m_labVoltageBattery.Text = values._diVoltageBatery + @" V";
                m_labCurrentBattery.Text = values._diCurrentBattery + @" A";
                m_labCurrentLoad.Text = values._diCurrentLoad + @" A";
                if (values._diEstimatedBatteryCapacity >= 0)
                {
                    var progressBattery = (int) values._diEstimatedBatteryCapacity;
                    if (progressBattery > 100) progressBattery = 100;
                    m_pgEstimatedBatteryCapacity.Value = progressBattery;
                    m_labEstimatedBatteryCapacity.Text = progressBattery + @" %";
                }
                m_labTemperature.Text = values._diTemperature + @" °C";
                m_labMode.Text = values._byMode;
                m_labResets.Text = values.m_iResets.ToString(CultureInfo.InvariantCulture);
                //m_labSuccessRate.Text = values..ToString();
            }
        }

        private void ResetUpsMonitor()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DVoid2Void(ResetUpsMonitor));
            }
            else
            {
                LabelColorAlarm(_lOutputFuse, false);
                LabelColorAlarm(_lOutputOutOfTollerance, false);
                LabelColorAlarm(_lPrimaryPowerMissing, false);
                LabelColorAlarm(_lBatteryFault, false);
                LabelColorAlarm(_lBatteryEmpty, false);
                LabelColorAlarm(_lBatteryFuse, false);
                LabelColorAlarm(_lOvertemperature, false);
                LabelColorAlarm(_lTamper, false);

                m_labVoltageInput.Text = string.Empty;
                m_labVoltageOutput.Text = string.Empty;
                m_labVoltageBattery.Text = string.Empty;
                m_labCurrentBattery.Text = string.Empty;
                m_labCurrentLoad.Text = string.Empty;
                m_labEstimatedBatteryCapacity.Text = string.Empty;
                m_pgEstimatedBatteryCapacity.Value = 0;
                m_labTemperature.Text = string.Empty;
                m_labMode.Text = string.Empty;
                m_labResets.Text = string.Empty;
                m_labSuccessRate.Text = string.Empty;
            }
        }

        private void LabelColorAlarm(Label label, bool isAlarm)
        {
            if (isAlarm)
            {
                label.BackColor = Color.Red;
                label.ForeColor = Color.Black;
            }
            else
            {
                label.BackColor = Color.Silver;
                label.ForeColor = Color.Gray;
            }
        }

        private void UpsMonitorOnlineStateChanged(string ipAddress, byte upsOnlineState)
        {
            if (_editingObject != null && !string.IsNullOrEmpty(_editingObject.IPAddress) &&
                _editingObject.IPAddress == ipAddress)
            {
                RefReshUpsOnlineState(upsOnlineState);
            }
        }

        private void RefReshUpsOnlineState(byte upsOnlineState)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<byte>(RefReshUpsOnlineState), upsOnlineState);
            }
            else
            {
                switch ((TUpsOnlineState) upsOnlineState)
                {
                    case TUpsOnlineState.Unknown:
                        _lUpsOnlineState.Text = GetString("Unknown");
                        _lUpsOnlineState.BackColor = Color.Silver;
                        _lUpsOnlineState.ForeColor = Color.Gray;
                        break;
                    case TUpsOnlineState.Online:
                        _lUpsOnlineState.Text = GetString("Online");
                        _lUpsOnlineState.BackColor = Color.LightGreen;
                        _lUpsOnlineState.ForeColor = Color.Black;
                        break;
                    case TUpsOnlineState.Offline:
                        _lUpsOnlineState.Text = GetString("Offline");
                        _lUpsOnlineState.BackColor = Color.Red;
                        _lUpsOnlineState.ForeColor = Color.Black;
                        ResetUpsMonitor();
                        break;
                    default:
                        _lUpsOnlineState.Text = string.Empty;
                        _lUpsOnlineState.BackColor = Color.Silver;
                        _lUpsOnlineState.ForeColor = Color.Gray;
                        break;
                }
            }
        }

        private void UpsMonitorAlarmStateChanged(string ipAddress, bool upsAlarm)
        {
            if (_editingObject != null && !string.IsNullOrEmpty(_editingObject.IPAddress) &&
                _editingObject.IPAddress == ipAddress)
            {
            }
        }

        private void CCUKillFailed(IPAddress ipAddress)
        {
            if (_editingObject != null && ipAddress != null && _editingObject.IPAddress == ipAddress.ToString())
            {
                TimerManager.Static.StartTimeout(3000, ShowInfoCCUKillFailed);
            }
        }

        private bool ShowInfoCCUKillFailed(TimerCarrier timerCarrier)
        {
            Dialog.Warning(GetString("CCUKillFailedAndRestartMessage"));
            return true;
        }

        private void _tpUpsMonitor_Enter(object sender, EventArgs e)
        {
            if (_editingObject != null && _editingObject.IdCCU != Guid.Empty)
            {
                SafeThread.StartThread(StartUpsMonitorStatistic);
            }
        }

        private void StartUpsMonitorStatistic()
        {
            Plugin.MainServerProvider.CCUs.SendUpsMonitorData(_editingObject.IdCCU);
        }

        private void _tpUpsMonitor_Leave(object sender, EventArgs e)
        {
            if (_editingObject != null && _editingObject.IdCCU != Guid.Empty)
            {
                SafeThread.StartThread(StopUpsMonitorStatistic);
            }
        }

        private void StopUpsMonitorStatistic()
        {
            Plugin.MainServerProvider.CCUs.StopSendUpsMonitorData(_editingObject.IdCCU);
        }

        private void _bRefreshAll_Click(object sender, EventArgs e)
        {
            SafeThread.StartThread(RefreshCCUPerformanceStatistics);
        }

        private void RefreshCCUPerformanceStatistics()
        {
            if (Plugin.MainServerProvider == null || CgpClient.Singleton.IsConnectionLost(true))
                return;

            ClearOtherStatisticsValues();

            int threadsCount;
            int flashFreeSpace;
            int flashSize;
            bool sdCardPresent;
            int sdCardFreeSpace;
            int sdCardSize;
            int freeMemory;
            int totalMemory;
            int memoryLoad;

            if (
                !Plugin.MainServerProvider.CCUs.GetOtherCCUStatistics(_editingObject.IdCCU,
                    out threadsCount,
                    out flashFreeSpace, out flashSize,
                    out sdCardPresent, out sdCardFreeSpace, out sdCardSize,
                    out freeMemory, out totalMemory, out memoryLoad))
            {
                return;
            }

            var otherStatisticsValues = new OtherStatisticsValues(threadsCount,
                flashFreeSpace, flashSize,
                sdCardPresent, sdCardFreeSpace, sdCardSize,
                freeMemory, totalMemory, memoryLoad);

            ShowOtherCCUStatistics(otherStatisticsValues);
        }

        private void ClearOtherStatisticsValues()
        {
            if (InvokeRequired)
                Invoke(new DVoid2Void(ClearOtherStatisticsValues));
            else
            {
                _eThreads.Text = @"-";
                _eFreeTotalFlashSpace.Text = @"- / -";
                _eFreeTotalSDSpace.Text = @"- / -";
                _eFreeMemory.Text = @"-";
                _eTotalMemory.Text = @"-";
                _eMemoryLoad.Text = @"- / -";
            }
        }

        private void ShowOtherCCUStatistics(OtherStatisticsValues otherStatisticsValues)
        {
            if (otherStatisticsValues == null)
                return;

            if (InvokeRequired)
            {
                Invoke(new Action<OtherStatisticsValues>(ShowOtherCCUStatistics), otherStatisticsValues);
            }
            else
            {
                _eThreads.Text = otherStatisticsValues.ThreadsCount.ToString(CultureInfo.InvariantCulture);

                _eFreeTotalFlashSpace.Text = string.Format("{0} / {1}",
                    GetStringMBGB(otherStatisticsValues.FlashFreeSpace),
                    GetStringMBGB(otherStatisticsValues.FlashSize));

                if (otherStatisticsValues.SdCardPresent)
                {
                    _eFreeTotalSDSpace.Text = string.Format("{0} / {1}",
                        GetStringMBGB(otherStatisticsValues.SdCardFreeSpace),
                        GetStringMBGB(otherStatisticsValues.SdCardSize));
                }
                else
                {
                    _eFreeTotalSDSpace.Text = GetString("TextCardNotPresent");
                }

                _eFreeMemory.Text = GetStringMBGB(otherStatisticsValues.FreeMemory/1000);
                _eTotalMemory.Text = GetStringMBGB(otherStatisticsValues.TotalMemory/1000);

                _eMemoryLoad.Text = string.Format("{0} / {1} %",
                    otherStatisticsValues.MemoryLoad,
                    (int) (100*(1 - ((float) otherStatisticsValues.FreeMemory/otherStatisticsValues.TotalMemory))));
            }
        }

        private string GetStringMBGB(int kiloBytes)
        {
            if (kiloBytes < 0)
                return "-";

            var megaBytes = (float) kiloBytes/1000;
            if (megaBytes < 1000)
            {
                return string.Format("{0} MB", megaBytes.ToString("0.00"));
            }

            return string.Format("{0} GB", (megaBytes/1000).ToString("0.00"));
        }

        #region DCU Test

        private BindingSource _bsDcuTest;

        private void CreateDataSourceTest()
        {
            if (_editingObject == null) return;
            if (_editingObject.DCUs == null || _editingObject.DCUs.Count == 0)
            {
                _bsDcuTest = new BindingSource();
                _dgvTestDcu.DataSource = _bsDcuTest;
                return;
            }

            if (Plugin.MainServerProvider == null || CgpClient.Singleton.IsConnectionLost(true))
                return;

            var lastPosition = 0;
            if (_bsDcuTest != null && _bsDcuTest.Count != 0)
                lastPosition = _bsDcuTest.Position;
            _bsDcuTest = new BindingSource();
            _bsDcuTest.AllowNew = false;
            var cDcus = Plugin.MainServerProvider.CCUs.GetDcuTestStates(_editingObject.IdCCU);

            if (cDcus == null)
            {
                _bsDcuTest = new BindingSource();
                _dgvTestDcu.DataSource = _bsDcuTest;
                return;
            }

            foreach (var testDcu in cDcus)
            {
                testDcu.CardType = GetFromListAlarmTypeBuzzer(testDcu.ToggleCardGeneratedCardType);
            }

            _bsDcuTest.DataSource = cDcus;
            if (lastPosition != 0)
            {
                _bsDcuTest.Position = lastPosition;
            }



            ShowDcuTestDataGrid();
        }

        private readonly IList<CbGeneratedCardType> _listGeneratedCardType =
            new List<CbGeneratedCardType>();

        private void CreateListAlarmTypeBuzzer()
        {
            _listGeneratedCardType.Clear();
            foreach (GeneratedCardType cardType in Enum.GetValues(typeof (GeneratedCardType)))
            {
                _listGeneratedCardType.Add(new CbGeneratedCardType(cardType));
            }
        }

        private CbGeneratedCardType GetFromListAlarmTypeBuzzer(GeneratedCardType cardType)
        {
            foreach (var atb in _listGeneratedCardType)
            {
                if (atb.CardType == cardType)
                    return atb;
            }

            CbGeneratedCardType defaultType = null;
            foreach (var atb in _listGeneratedCardType)
            {
                if (atb.CardType == GeneratedCardType.MifareCSN)
                    defaultType = atb;
            }
            return defaultType;
        }

        private void ShowDcuTestDataGrid()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DVoid2Void(ShowDcuTestDataGrid));
            }
            else
            {
                CreateListAlarmTypeBuzzer();
                _dgvTestDcu.AutoGenerateColumns = false;
                _dgvTestDcu.DataSource = _bsDcuTest;

                if (!_dgvTestDcu.Columns.Contains(DcuTestRoutineDataGridObj.COLUMN_NAME))
                {
                    var columnTypeName = new DataGridViewTextBoxColumn();
                    columnTypeName.Name = DcuTestRoutineDataGridObj.COLUMN_NAME;
                    columnTypeName.DataPropertyName = DcuTestRoutineDataGridObj.COLUMN_NAME;
                    columnTypeName.ReadOnly = true;
                    columnTypeName.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    _dgvTestDcu.Columns.Add(columnTypeName);
                }

                if (!_dgvTestDcu.Columns.Contains(DcuTestRoutineDataGridObj.COLUMN_ADC))
                {
                    var columnAdc = new DataGridViewCheckBoxColumn();
                    columnAdc.Name = DcuTestRoutineDataGridObj.COLUMN_ADC;
                    columnAdc.DataPropertyName = DcuTestRoutineDataGridObj.COLUMN_ADC;
                    columnAdc.ReadOnly = false;
                    columnAdc.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    _dgvTestDcu.Columns.Add(columnAdc);
                }

                if (!_dgvTestDcu.Columns.Contains(DcuTestRoutineDataGridObj.COLUMN_ADC_MIN))
                {
                    var columnAdcMinTime = new DataGridViewTextBoxColumn();
                    columnAdcMinTime.Name = DcuTestRoutineDataGridObj.COLUMN_ADC_MIN;
                    columnAdcMinTime.DataPropertyName = DcuTestRoutineDataGridObj.COLUMN_ADC_MIN;
                    columnAdcMinTime.ReadOnly = false;
                    columnAdcMinTime.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    _dgvTestDcu.Columns.Add(columnAdcMinTime);
                }

                if (!_dgvTestDcu.Columns.Contains(DcuTestRoutineDataGridObj.COLUMN_ADC_MAX))
                {
                    var columnAdcMaxTime =
                        new DataGridViewTextBoxColumn
                        {
                            Name = DcuTestRoutineDataGridObj.COLUMN_ADC_MAX,
                            DataPropertyName = DcuTestRoutineDataGridObj.COLUMN_ADC_MAX,
                            ReadOnly = false,
                            AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
                        };

                    _dgvTestDcu.Columns.Add(columnAdcMaxTime);
                }

                if (!_dgvTestDcu.Columns.Contains(DcuTestRoutineDataGridObj.COLUMN_CARD))
                {
                    var columnCard = new DataGridViewCheckBoxColumn();
                    columnCard.Name = DcuTestRoutineDataGridObj.COLUMN_CARD;
                    columnCard.DataPropertyName = DcuTestRoutineDataGridObj.COLUMN_CARD;
                    columnCard.ReadOnly = false;
                    columnCard.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    _dgvTestDcu.Columns.Add(columnCard);
                }

                if (!_dgvTestDcu.Columns.Contains(DcuTestRoutineDataGridObj.COLUMN_CARD_MIN))
                {
                    var columnCardMin = new DataGridViewTextBoxColumn();
                    columnCardMin.Name = DcuTestRoutineDataGridObj.COLUMN_CARD_MIN;
                    columnCardMin.DataPropertyName = DcuTestRoutineDataGridObj.COLUMN_CARD_MIN;
                    columnCardMin.ReadOnly = false;
                    columnCardMin.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    _dgvTestDcu.Columns.Add(columnCardMin);
                }

                if (!_dgvTestDcu.Columns.Contains(DcuTestRoutineDataGridObj.COLUMN_CARD_MAX))
                {
                    var columnCardMax = new DataGridViewTextBoxColumn();
                    columnCardMax.Name = DcuTestRoutineDataGridObj.COLUMN_CARD_MAX;
                    columnCardMax.DataPropertyName = DcuTestRoutineDataGridObj.COLUMN_CARD_MAX;
                    columnCardMax.ReadOnly = false;
                    columnCardMax.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    _dgvTestDcu.Columns.Add(columnCardMax);
                }

                if (!_dgvTestDcu.Columns.Contains(DcuTestRoutineDataGridObj.COLUMN_CARDTYPE))
                {
                    var columnCardType = new DataGridViewComboBoxColumn();
                    columnCardType.DataSource = _listGeneratedCardType;
                    columnCardType.Name = DcuTestRoutineDataGridObj.COLUMN_CARDTYPE;
                    columnCardType.DataPropertyName = DcuTestRoutineDataGridObj.COLUMN_CARDTYPE;
                    columnCardType.DisplayMember = DcuTestRoutineDataGridObj.COLUMN_CARDTYPE;
                    columnCardType.ReadOnly = false;
                    columnCardType.ValueMember = "Self";
                    columnCardType.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    _dgvTestDcu.Columns.Add(columnCardType);
                }

                if (_dgvTestDcu.Columns.Contains(DcuTestRoutineDataGridObj.COLUMN_NAME))
                {
                    _dgvTestDcu.Columns[DcuTestRoutineDataGridObj.COLUMN_NAME].AutoSizeMode =
                        DataGridViewAutoSizeColumnMode.Fill;
                }
                _dgvTestDcu.AutoGenerateColumns = false;
                _dgvTestDcu.AllowUserToAddRows = false;
                //LocalizationHelper.TranslateDataGridView(_dgvTestDcu);
            }
        }

        private void _cbSelectAllToggleCard_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (_dgvTestDcu == null) return;
                if (_bsDcuTest.Count == 0) return;
                foreach (DataGridViewRow row in _dgvTestDcu.Rows)
                {
                    var dtr = (DcuTestRoutineDataGridObj) _bsDcuTest.List[row.Index];

                    if (dtr.IsOnline())
                    {
                        row.Cells[DcuTestRoutineDataGridObj.COLUMN_CARD].Value = _cbSelectAllToggleCard.Checked;
                    }
                    else
                    {
                        row.Cells[DcuTestRoutineDataGridObj.COLUMN_CARD].Value = false;
                    }
                }
            }
            catch
            {
            }
        }

        private void _cbSelectAllToggleADC_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (_dgvTestDcu == null) return;
                if (_bsDcuTest.Count == 0) return;
                foreach (DataGridViewRow row in _dgvTestDcu.Rows)
                {
                    var dtr = (DcuTestRoutineDataGridObj) _bsDcuTest.List[row.Index];
                    if (dtr.IsOnline())
                    {
                        row.Cells[DcuTestRoutineDataGridObj.COLUMN_ADC].Value = _cbSelectAllToggleADC.Checked;
                    }
                    else
                    {
                        row.Cells[DcuTestRoutineDataGridObj.COLUMN_ADC].Value = false;
                    }
                }
            }
            catch
            {
            }
        }

        private void _bTestDcu_Click(object sender, EventArgs e)
        {
            var listDcuTest = new List<DcuRunningTest>();

            foreach (DataGridViewRow row in _dgvTestDcu.Rows)
            {
                var dtr = (DcuTestRoutineDataGridObj) _bsDcuTest.List[row.Index];
                if (dtr != null)
                {
                    listDcuTest.Add(dtr.GetObjectForCcu());
                }
            }

            if (listDcuTest.Count > 0)
            {
                if (Plugin.MainServerProvider == null || CgpClient.Singleton.IsConnectionLost(true))
                    return;
                SafeThread<object[]>.StartThread(RunTest, listDcuTest.ToArray());
            }

        }

        private void RunTest(object[] cfnDcu)
        {
            Plugin.MainServerProvider.CCUs.SetDcuTest(_editingObject.IdCCU, cfnDcu);
        }

        private void _bRefreshTestDcu_Click(object sender, EventArgs e)
        {
            SafeThread.StartThread(CreateDataSourceTest);
        }

        #endregion

        private void _rtbResults_TextChanged(object sender, EventArgs e)
        {

        }

        private void _tbmPerson_ButtonPopupMenuItemClick(ToolStripItem item, int index)
        {
            if (item.Name == "_tsiModify3")
            {
                AddPerson();
            }
        }

        private void _tbmAlarmArea_ButtonPopupMenuItemClick(ToolStripItem item, int index)
        {
            if (item.Name == "_tsiModify4")
            {
                AddAlarmArea();
            }
        }

        private void _bTestAlarmAreaSimulation_Click(object sender, EventArgs e)
        {
            if (_tbmPerson.Tag != null && _tbmAlarmArea.Tag != null)
            {
                var person = CgpClient.Singleton.MainServerProvider.Persons.GetObjectById((Guid) _tbmPerson.Tag);
                var alarmArea = Plugin.MainServerProvider.AlarmAreas.GetObjectById((Guid) _tbmAlarmArea.Tag);

                if ((person != null) && (alarmArea != null))
                {
                    _rtbResultsDB.Text = Plugin.MainServerProvider.ACLPersons.
                        GetAlarmAreaActivationRights(_editingObject.IdCCU, person, alarmArea);

                    SafeThread<Guid, Guid>.StartThread(
                        CheckAlarmAreaActivationRightsOnCcu,
                        person.IdPerson,
                        alarmArea.IdAlarmArea);
                }
            }
            else
            {
                _rtbResultsDB.Text = @"Please select person and Alarm area";
            }
        }

        private void CheckAlarmAreaActivationRightsOnCcu(
            Guid idPerson,
            Guid idAlarmArea)
        {
            var result = Plugin.MainServerProvider.CheckAlarmAreaActivationRights(
                _editingObject.IdCCU,
                idPerson,
                idAlarmArea);

            CheckAlarmAreaActivationRightsOnCcuResult(result);
        }

        private void CheckAlarmAreaActivationRightsOnCcuResult(string result)
        {
            if (InvokeRequired)
            {
                BeginInvoke(
                    new Action<string>(CheckAlarmAreaActivationRightsOnCcuResult),
                    result);

                return;
            }

            _rtbResultsCCU.Text = result;
        }

        #region AlarmInstructions

        protected override bool LocalAlarmInstructionsView()
        {
            return
                CgpClient.Singleton.MainServerProvider.HasAccess(
                    NCASAccess.GetAccess(AccessNCAS.CcusLocalAlarmInstructionsView));
        }

        protected override bool LocalAlarmInstructionsAdmin()
        {
            return
                CgpClient.Singleton.MainServerProvider.HasAccess(
                    NCASAccess.GetAccess(AccessNCAS.CcusLocalAlarmInstructionsAdmin));
        }

        protected override string GetLocalAlarmInstruction()
        {
            return _editingObject.LocalAlarmInstruction;
        }

        #endregion

        private void SendFileToServer(UpgradeType upgradeType, string filePath)
        {
            if (CgpClient.Singleton.IsConnectionLost(true))
                return;

            Exception error;

            if (upgradeType == UpgradeType.CEUpgrade
                && string.IsNullOrEmpty(FilePacker.TryGetHeaderText(filePath, out error)))
            {
                int crcVersion;
                string version = WinCeUpgradeFileHelper.GetVersion(filePath, out crcVersion);

                if (string.IsNullOrEmpty(version))
                {
                    Dialog.Warning(GetString("WarningIncorrectUpgradeFile"));
                    AfterUpgrade(upgradeType, false, false, false);
                    return;
                }

                if (crcVersion == 1)
                    Dialog.Warning(GetString("WarningUseCrcV1BinaryFile"));
            }           

            var fileName = Path.GetFileName(filePath);
            if (Plugin.MainServerProvider.ExistFileForUpgrade(upgradeType, fileName))
            {
                if (!Dialog.Question(GetString("QuestionOwerriteUpgradeFile")))
                {
                    AfterUpgrade(upgradeType, false, false, false);
                    return;
                }
            }

            Stream fileStream = null;
            var sendFileSuccessed = false;
            try
            {
                fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                var data = new byte[fileStream.Length];
                fileStream.Read(data, 0, data.Length);
                sendFileSuccessed = Plugin.MainServerProvider.SendFileForUpgrade(upgradeType, fileName, data);
            }
            catch
            {
                AfterUpgrade(upgradeType, true, false, sendFileSuccessed);
            }
            finally
            {
                if (fileStream != null)
                    try
                    {
                        fileStream.Close();
                        fileStream.Dispose();
                    }
                    catch
                    {
                    }
            }

            AfterUpgrade(upgradeType, true, true, sendFileSuccessed);
        }

        private void AfterUpgrade(UpgradeType upgradeType, bool showResult, bool copyFileSuccessed,
            bool sendFileSuccessed)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<UpgradeType, bool, bool, bool>(AfterUpgrade), upgradeType, showResult,
                    copyFileSuccessed, sendFileSuccessed);
            }
            else
            {
                if (showResult)
                {
                    Control control = null;
                    switch (upgradeType)
                    {
                        case UpgradeType.CCUUpgrade:
                            control = _ccuUpgrade;
                            break;
                        case UpgradeType.CEUpgrade:
                            control = _ceUpgrade;
                            break;
                        case UpgradeType.DCUUpgrade:
                            control = _crUpgrade;
                            break;
                        case UpgradeType.CRUpgrade:
                            control = _crUpgrade;
                            break;
                    }

                    if (!copyFileSuccessed)
                    {
                        if (control != null)
                        {
                            ControlNotification.Singleton.Error(NotificationPriority.JustOne, control,
                                GetString("ErrorCopyFileForUpgradeFailed"), ControlNotificationSettings.Default);
                        }
                    }
                    else if (!sendFileSuccessed)
                    {
                        if (control != null)
                        {
                            ControlNotification.Singleton.Error(NotificationPriority.JustOne, control,
                                GetString("ErrorUpgradeFileIsUsedByAnotherClient"), ControlNotificationSettings.Default);
                        }
                    }
                    else
                    {
                        ControlNotification.Singleton.Info(NotificationPriority.JustOne, control,
                            GetString("InfoSendUpgradeFileToServerSucceeded"), ControlNotificationSettings.Default);
                    }
                }

                switch (upgradeType)
                {
                    case UpgradeType.CCUUpgrade:
                        _ccuUpgradeFiles.EnableInsertFile = true;

                        if (sendFileSuccessed)
                            RefreshAvailableUpgradesList();

                        break;
                    case UpgradeType.CEUpgrade:
                        _ceUpgradeFiles.EnableInsertFile = true;

                        if (sendFileSuccessed)
                            RefreshAvailableCEUpgradesList();

                        break;
                    case UpgradeType.DCUUpgrade:
                        _dcuUpgradeFiles.EnableInsertFile = true;

                        if (sendFileSuccessed)
                            RefreshAvailableDCUUpgradeList();

                        break;
                    case UpgradeType.CRUpgrade:
                        _crUpgradeFiles.EnableInsertFile = true;

                        if (sendFileSuccessed)
                            RefreshAvailableCRUpgradeList();

                        break;
                }
            }
        }

        private void UploadCeFile()
        {
            _ofdBrowse.DefaultExt = "bin";
            _ofdBrowse.Filter = GetString("TextFilterForUpgradeCE") + @"|NK_*.bin;*.gz;*.tar";
            _ofdBrowse.Title = GetString("TextTitleOpenFileForUpgradeCE");

            if (_ofdBrowse.ShowDialog() == DialogResult.OK)
            {
                _ceUpgradeFiles.EnableInsertFile = false;
                SafeThread<UpgradeType, string>.StartThread(SendFileToServer, UpgradeType.CEUpgrade, _ofdBrowse.FileName);
            }

        }

        private void InsertDcuUpgradeFile()
        {
            _ofdBrowse.DefaultExt = "gz";
            _ofdBrowse.Filter = GetString("TextFilterForUpgradeDCU") + @"|DCU_*.gz";
            _ofdBrowse.Title = GetString("TextTitleOpenFileForUpgradeDCU");

            if (_ofdBrowse.ShowDialog() == DialogResult.OK)
            {
                UploadDcuUpgradeFile(_ofdBrowse.FileName);
            }
        }

        private void UploadDcuUpgradeFile(string filePath)
        {
            string[] headerParameters;
            try
            {
                headerParameters = FilePacker.GetHeaderParameters(filePath);
            }
            catch (Exception)
            {
                ControlNotification.Singleton.Warning(NotificationPriority.JustOne, _dcuUpgrade,
                    GetString("ErrorUnknowFormat"),
                    new ControlNotificationSettings(NotificationParts.Hint, HintPosition.Center));
                return;
            }

            if (headerParameters == null || headerParameters.Length < 4)
            {
                ControlNotification.Singleton.Warning(NotificationPriority.JustOne, _dcuUpgrade,
                    GetString("ErrorUnknowFormat"),
                    new ControlNotificationSettings(NotificationParts.Hint, HintPosition.Center));
            }
            else if (headerParameters.Length < 5)
            {
                var selectDCUHWType = new SelectDCUHWTypeDialog();
                if (selectDCUHWType.ShowDialog() == DialogResult.OK)
                {
                    if (selectDCUHWType.SelectedDCUHWType == DCUHWVersion.Unknown)
                        return;

                    var hwVersionDCU = String.Empty;

                    switch (selectDCUHWType.SelectedDCUHWType)
                    {
                        case DCUHWVersion.RS485:
                            hwVersionDCU = "01";
                            break;
                        case DCUHWVersion.Echelon:
                            hwVersionDCU = "02";
                            break;
                    }

                    var outputDirectory = Path.GetDirectoryName(filePath);
                    var filePacker = new FilePacker();

                    Exception exception;
                    if (!filePacker.TryUnpack(
                        filePath,
                        outputDirectory,
                        out exception
                        ))
                    {
                        Dialog.Error(GetString("ErrorFailedUnpackFile"));
                    }
                    else
                    {
                        var newHeaderParameters = new[]
                        {
                            hwVersionDCU,
                            headerParameters[1],
                            headerParameters[2],
                            headerParameters[3]
                        };

                        var fileInfo = new FileInfo(filePath);
                        var lenght = fileInfo.Name.LastIndexOf('.');
                        var fileNameWithoutExtension = fileInfo.Name.Substring(0, lenght);
                        var outputFileName = String.Format("{0}.gz", fileNameWithoutExtension);

                        if (!filePacker.TryPack("DCU", Path.Combine(outputDirectory, newHeaderParameters[2]),
                            Path.Combine(outputDirectory, outputFileName), new FilePacker.PackingContext {HeaderParts = newHeaderParameters}))
                            Dialog.Error(GetString("ErrorFailedPackFile"));
                        else
                        {
                            _dcuUpgradeFiles.EnableInsertFile = false;
                            SafeThread<UpgradeType, string>.StartThread(SendFileToServer, UpgradeType.DCUUpgrade, filePath);
                        }
                    }
                }
                selectDCUHWType.Dispose();
            }
            else
            {
                _dcuUpgradeFiles.EnableInsertFile = false;
                SafeThread<UpgradeType, string>.StartThread(SendFileToServer, UpgradeType.DCUUpgrade, filePath);
            }
        }

        private void UploadCrUpgradeFile()
        {
            _ofdBrowse.DefaultExt = "gz";
            _ofdBrowse.Filter = GetString("TextFilterForUpgradeCR") + @"|CR_*.gz";
            _ofdBrowse.Title = GetString("TextTitleOpenFileForUpgradeCR");

            if (_ofdBrowse.ShowDialog() == DialogResult.OK)
            {
                _crUpgradeFiles.EnableInsertFile = false;
                SafeThread<UpgradeType, string>.StartThread(SendFileToServer, UpgradeType.CRUpgrade, _ofdBrowse.FileName);
            }
        }

        private void _tbdpSetTimeManually_TextDateChanged(object sender, EventArgs e)
        {
            _bSet2.Enabled = _tbdpSetTimeManually.Value != null;
        }

        private void _bSet2_EnabledChanged(object sender, EventArgs e)
        {
            if (_bSet2.Enabled)
                _bSet2.Enabled = _tbdpSetTimeManually.Value != null;
        }

        private void _bSet2_Click(object sender, EventArgs e)
        {
            if (_tbdpSetTimeManually.Value != null && Dialog.WarningQuestion(GetString("QuestionSetTimeManually")))
            {
                if (CgpClient.Singleton.IsConnectionLost(true))
                    return;

                SafeThread<Guid, DateTime>
                    .StartThread(SetTimeManually, _editingObject.IdCCU,
                        _tbdpSetTimeManually.Value.Value.ToUniversalTime());
            }
        }

        private void SetTimeManually(Guid ccuId, DateTime utcDateTime)
        {
            ShowResulSetTimeManually(Plugin.MainServerProvider.CCUs.SetTimeManually(ccuId, utcDateTime));
        }

        private void ShowResulSetTimeManually(bool result)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<bool>(ShowResulSetTimeManually), result);
            }
            else
            {
                if (result)
                {
                    ControlNotification.Singleton.Info(NotificationPriority.JustOne, _tbdpSetTimeManually,
                        GetString("SetTimeManuallySucceeded"), ControlNotificationSettings.Default);
                }
                else
                {
                    ControlNotification.Singleton.Info(NotificationPriority.JustOne, _tbdpSetTimeManually,
                        GetString("SetTimeManuallyFailed"), ControlNotificationSettings.Default);
                }
            }
        }

        private void _bDeleteEvents_Click(object sender, EventArgs e)
        {
            if (Plugin.MainServerProvider == null || CgpClient.Singleton.IsConnectionLost(false))
                return;

            _eResultOfAction.BackColor = _baseBackColorResultOfAction;
            _eResultOfAction.Text = string.Empty;

            if (Dialog.WarningQuestion(GetString("QuestionConfirmEventsDelete")))
            {
                SafeThread.StartThread(RunDeleteEvents);
            }
        }

        private void RunDeleteEvents()
        {
            var retValue = Plugin.MainServerProvider.RemoveCCUEvents(_editingObject.IdCCU);
            ShowDeleteEventsResult(retValue);
        }

        private void ShowDeleteEventsResult(bool retValue)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<bool>(ShowDeleteEventsResult), retValue);
            }
            else
            {
                if (retValue)
                {
                    _eResultOfAction.Text = GetString("CCUEventsRemoveSuccess");
                    _eResultOfAction.BackColor = Color.LightGreen;
                }
                else
                {
                    _eResultOfAction.Text = GetString("CCUEventsRemoveFailed");
                    _eResultOfAction.BackColor = Color.Red;
                }
            }
        }

        private class SelectedCell
        {
            public int RowIndex;
            public int ColumnIndex;
        }

        private SelectedCell[] GetDgvSelectedCells(DataGridView dataGrid)
        {
            if (dataGrid == null || dataGrid.SelectedCells == null || dataGrid.SelectedCells.Count == 0)
                return null;

            var result = new List<SelectedCell>();
            foreach (DataGridViewCell cell in dataGrid.SelectedCells)
            {
                result.Add(new SelectedCell
                {
                    RowIndex = cell.RowIndex,
                    ColumnIndex = cell.ColumnIndex
                });
            }

            return result.ToArray();
        }

        private void SetDgvSelectedCells(DataGridView dataGridView, SelectedCell[] selectedCells)
        {
            //to ensure that the firs cell will not be selected just because of focusing a control
            try
            {
                dataGridView[0, 0].Selected = false;
            }
            catch (Exception)
            {
            }

            if (selectedCells == null || selectedCells.Length == 0)
                return;

            foreach (var cell in selectedCells)
            {
                try
                {
                    dataGridView[cell.ColumnIndex, cell.RowIndex].Selected = true;
                }
                catch (Exception)
                {
                }
            }
        }

        private void _cbSyncingTimeFromServer_CheckStateChanged(object sender, EventArgs e)
        {
            EnabelDisableNTPSettings();
            EditTextChanger(null, null);
        }

        private ICollection<ThreadInfo> _threadsInfo;

        private void ShowThreadsInfo(string id, bool hasReload)
        {
            SafeThread.StartThread(delegate
            {
                if (CgpClient.Singleton.IsConnectionLost(false))
                    return;

                if (hasReload)
                    _threadsInfo = Plugin.MainServerProvider.GetThreadsInfo(_editingObject.IdCCU);

                if (_threadsInfo == null || _threadsInfo.Count == 0)
                    return;

                Invoke(new MethodInvoker(delegate
                {
                    if (_cdgvThreadMap.LocalizationHelper == null)
                        _cdgvThreadMap.LocalizationHelper = NCASClient.LocalizationHelper;

                    var firstDisplayedRow = _cdgvThreadMap.DataGrid.FirstDisplayedScrollingRowIndex;
                    var selectedRowIndex = -1;

                    if (_cdgvThreadMap.SelectedRows != null && _cdgvThreadMap.SelectedRows.Count > 0)
                        selectedRowIndex = _cdgvThreadMap.SelectedRows[0].Index;

                    IEnumerable<ThreadInfo> source = _threadsInfo;

                    if (!string.IsNullOrEmpty(id))
                        source =
                            _threadsInfo
                                .Where(x => x.Id.ToString(CultureInfo.InvariantCulture).Contains(id));

                    _cdgvThreadMap.ModifyGridView(
                        new BindingSource
                        {
                            DataSource = new SortableBindingList<ThreadInfo>(source)
                        },
                        ThreadInfo.COLUMN_ID,
                        ThreadInfo.COLUMN_PRIORITY,
                        ThreadInfo.COLUMN_NAME,
                        ThreadInfo.COLUMN_IS_BACKGROUND);

                    if (firstDisplayedRow > 0 && _cdgvThreadMap.DataGrid.RowCount > firstDisplayedRow)
                        _cdgvThreadMap.DataGrid.FirstDisplayedScrollingRowIndex = firstDisplayedRow;

                    if (selectedRowIndex > 0 && _cdgvThreadMap.DataGrid.RowCount > selectedRowIndex)
                    {
                        _cdgvThreadMap.DataGrid.Rows[0].Selected = false;
                        _cdgvThreadMap.DataGrid.Rows[selectedRowIndex].Selected = true;
                    }

                    _cdgvThreadMap.DataGrid.Columns[ThreadInfo.COLUMN_ID].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    _cdgvThreadMap.DataGrid.Columns[ThreadInfo.COLUMN_PRIORITY].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    _cdgvThreadMap.DataGrid.Columns[ThreadInfo.COLUMN_NAME].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    _cdgvThreadMap.DataGrid.Columns[ThreadInfo.COLUMN_IS_BACKGROUND].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                }));
            });
        }

        private void _bRefresh3_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_tbThreadId.Text))
            {
                uint id;
                if (UInt32.TryParse(_tbThreadId.Text, out id))
                    ShowThreadsInfo(_tbThreadId.Text, true);
            }
            else
                ShowThreadsInfo(null, true);
        }

        private void _cbVerbosityLevel_SelectedValueChanged(object sender, EventArgs e)
        {
            if (!_cbVerbosityLevel.Focused)
                return;

            if (_cbVerbosityLevel.SelectedValue is VerbosityLevelInfo)
            {
                try
                {
                    _nudVerbosityLevel.Value = (_cbVerbosityLevel.SelectedValue as VerbosityLevelInfo).Level;
                }
                catch
                {
                }
            }
        }

        private void _nudVerbosityLevel_ValueChanged(object sender, EventArgs e)
        {
            if (!_nudVerbosityLevel.Focused)
                return;

            if (_cbVerbosityLevel.Items == null || _cbVerbosityLevel.Items.Count == 0)
                return;

            SetVerbosityComboBox((byte) _nudVerbosityLevel.Value);
        }

        private void SetVerbosityComboBox(byte verbosity)
        {
            if (_verbosityLevels == null || _verbosityLevels.Count == 0)
                return;

            VerbosityLevelInfo result = null;
            lock (_verbosityLevels)
            {
                foreach (var info in _verbosityLevels)
                {
                    if (info.Level >= verbosity)
                    {
                        if (result == null)
                            result = info;
                        if (info.Level < result.Level)
                            result = info;
                    }
                }
            }

            Invoke(new MethodInvoker(delegate {
                                                  _cbVerbosityLevel.SelectedValue = result;
            }));
        }

        private void _bSet1_Click(object sender, EventArgs e)
        {
            if (CgpClient.Singleton.IsConnectionLost(false))
                return;

            SafeThread<byte>.StartThread(
                SetVerbosityLevel,
                (byte) _nudVerbosityLevel.Value);
        }

        private void SetVerbosityLevel(byte verbosityLevel)
        {
            var set = Plugin.MainServerProvider.SetVerbosityLevel(_editingObject.IdCCU, (byte)_nudVerbosityLevel.Value);

            SetVerbosityLevelResult(set);
        }

        private void SetVerbosityLevelResult(bool set)
        {
            if (InvokeRequired)
            {
                BeginInvoke(
                    new Action<bool>(SetVerbosityLevelResult),
                    set);
                
                return;
            }

            if (set)
                ControlNotification.Singleton.Info(NotificationPriority.JustOne, _bSet1, GetString("Succeeded"),
                    ControlNotificationSettings.Default);
            else
                ControlNotification.Singleton.Warning(NotificationPriority.JustOne, _bSet1, GetString("Failed"),
                    ControlNotificationSettings.Default);
        }

        private void _Reset1_Click(object sender, EventArgs e)
        {
            if (!CgpClientMainForm.Singleton.MainIsConnectionLost(false))
            {
                SafeThread.StartThread(ResetCCUCommandTimeouts);
            }
        }

        private void ResetCCUCommandTimeouts()
        {
            Plugin.MainServerProvider.CCUs.ResetCCUCommandTimeouts(_editingObject.IdCCU);
        }

        private bool IPAddressIsFree(string IPAddress)
        {
            var isFree = true;
            Exception ex;
            var CCUs = Plugin.MainServerProvider.CCUs.GetListObj(out ex);
            CCU ccu;
            foreach (var ccuListObj in CCUs)
            {
                if (_ccu.IdCCU != ccuListObj.Id)
                {
                    ccu = Plugin.MainServerProvider.CCUs.GetObjectById(ccuListObj.Id);
                    if (ccu.IPAddress == IPAddress)
                    {
                        isFree = false;
                        break;
                    }
                }
            }
            return isFree;
        }

        private void _bTestIP_Click(object sender, EventArgs e)
        {
            if (_rbStatic.Checked)
            {
                if (_eIPSettingsIPAddress.Text == string.Empty)
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _eIPSettingsIPAddress,
                        GetString("ErrorInsertIPAddress"), ControlNotificationSettings.Default);
                    return;
                }

                if (!IPHelper.IsValid4(_eIPSettingsIPAddress.Text))
                {
                    ControlNotification.Singleton.Error(
                        NotificationPriority.JustOne, _eIPSettingsIPAddress,
                        GetString("ErrorWrongIPAddress"), ControlNotificationSettings.Default);
                    return;
                }

                if (!IPAddressIsFree(_eIPSettingsIPAddress.Text))
                {
                    ControlNotification.Singleton.Error(
                        NotificationPriority.JustOne, _eIPSettingsIPAddress,
                        GetString("IPAddressIsAlreadyUsed"), ControlNotificationSettings.Default);
                    return;
                }

                if (_eIPSettingsMask.Text == string.Empty)
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _eIPSettingsMask,
                        GetString("ErrorInsertSubnetMask"), ControlNotificationSettings.Default);
                    return;
                }

                if (!IPHelper.IsValidMask4(_eIPSettingsMask.Text))
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _eIPSettingsMask,
                        GetString("ErrorWrongSubnetMask"), ControlNotificationSettings.Default);
                    return;
                }
                //GetString("ErrorWrongIPAddress")
                var maskParts = _eIPSettingsMask.Text.Split('.');

                if (Int16.Parse(maskParts[1]) == 0 && Int16.Parse(maskParts[2]) == 0
                    && Int16.Parse(maskParts[3]) == 0 && Int16.Parse(maskParts[0]) < 255)
                {
                    ControlNotification.Singleton.Error(
                        NotificationPriority.JustOne, _eIPSettingsMask,
                        GetString("ErrorSmallerNetworkMasks"), ControlNotificationSettings.Default);
                    return;
                }
                if (Int16.Parse(maskParts[0]) == 255 && Int16.Parse(maskParts[1]) == 255
                    && Int16.Parse(maskParts[2]) == 255 && Int16.Parse(maskParts[3]) >= 254)
                {
                    ControlNotification.Singleton.Error(
                        NotificationPriority.JustOne, _eIPSettingsMask,
                        GetString("ErrorVerySmallIPSpace"), ControlNotificationSettings.Default);
                    return;
                }

                if (_eIPSettingsGateway.Text != string.Empty && _eIPSettingsGateway.Text != @"0.0.0.0")
                {
                    if (!IPHelper.IsValid4(_eIPSettingsGateway.Text))
                    {
                        ControlNotification.Singleton.Error(
                            NotificationPriority.JustOne, _eIPSettingsGateway,
                            GetString("ErrorWrongIPAddress"), ControlNotificationSettings.Default);
                        return;
                    }
                    if (
                        !IPHelper.IsSameNetwork4(_eIPSettingsIPAddress.Text, _eIPSettingsMask.Text,
                            _eIPSettingsGateway.Text))
                    {
                        ControlNotification.Singleton.Error(NotificationPriority.JustOne, _eIPSettingsGateway,
                            String.Format("{0} {1} and {2}", GetString("ErrorWrongGateway"), _eIPSettingsIPAddress.Text,
                                _eIPSettingsMask.Text),
                            ControlNotificationSettings.Default);
                        return;
                    }
                }

                //ControlNotification.Singleton.Info(NotificationPriority.JustOne, _bTestIP,
                //    GetString("IPSettingsValidationSucceeded"), ControlNotificationSettings.Default);

                _bTestIP.Enabled = false;
                Cursor = Cursors.WaitCursor;
                SafeThread.StartThread(DoTestPingIpAddress);

                //_bTestIP.Enabled = false;
                //_bApply1.Enabled = true;
            }
        }

        private void DoTestPingIpAddress()
        {
            bool clientStatus = Ping(_eIPSettingsIPAddress.Text, 3);
            bool serverStatus = Plugin.MainServerProvider.CCUs.PingIpAddress(_eIPSettingsIPAddress.Text, 3);

            BeginInvoke(new Action(() =>
            {
                Cursor = Cursors.Default;

                if (!clientStatus
                    && !serverStatus)
                {
                    _bTestIP.Enabled = false;
                    _bApply1.Enabled = true;

                    ControlNotification.Singleton.Info(NotificationPriority.JustOne, _bTestIP,
                        GetString("IPSettingsValidationSucceeded"), ControlNotificationSettings.Default);
                }
                else
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _bTestIP,
                        GetString("ErrorIpAddressAlreadyUsed"), ControlNotificationSettings.Default);
                }
            }));
        }

        private void _rbDHCP_CheckedChanged(object sender, EventArgs e)
        {
            _bApply1.Enabled = true;
            _bTestIP.Enabled = false;
        }

        private void _eIPSettingsIPAddress_TextChanged(object sender, EventArgs e)
        {
            _bTestIP.Enabled = true;
            _bApply1.Enabled = false;
        }

        private void _eIPSettingsMask_TextChanged(object sender, EventArgs e)
        {
            _bTestIP.Enabled = true;
            _bApply1.Enabled = false;
        }

        private void _eIPSettingsGateway_TextChanged(object sender, EventArgs e)
        {
            _bTestIP.Enabled = true;
            _bApply1.Enabled = false;
        }

        private void NCASCCUEditForm_Shown(object sender, EventArgs e)
        {
            if (_rbDHCP.Checked)
            {
                _bApply1.Enabled = true;
                _bTestIP.Enabled = false;
            }
            else
            {
                _bApply1.Enabled = false;
                _bTestIP.Enabled = true;
            }
        }

        private void _bMemoryCollect_Click(object sender, EventArgs e)
        {
            if (CgpClient.Singleton.IsConnectionLost(true))
                return;

            ClearResultOfAction();
            SafeThread.StartThread(RunGcCollect);
        }

        private void RunGcCollect()
        {
            SetRunGcCollectResult(Plugin.MainServerProvider.CCUs.RunGcCollect(_editingObject.IdCCU));
        }

        private void SetRunGcCollectResult(bool success)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new DBool2Void(SetRunGcCollectResult), success);
            }
            else
            {
                if (success)
                {
                    _eResultOfAction.Text = GetString("NCASCCUEditForm_RunGcCollectSucceeded");
                    _eResultOfAction.BackColor = Color.LightGreen;
                }
                else
                {
                    _eResultOfAction.Text = GetString("NCASCCUEditForm_RunGcCollectFailed");
                    _eResultOfAction.BackColor = Color.Red;
                }
            }
        }

        private void DeleteCeUpgradeFile(object fileName)
        {
            var upgradeCeFile = fileName as CeUpgradeFile;

            if (upgradeCeFile == null)
                return;

            if (!Dialog.WarningQuestion(GetString("QuestionUpgradeFileDelete")))
                return;

            if (Plugin.MainServerProvider.DeleteUpgradeFile(UpgradeType.CEUpgrade, upgradeCeFile.FileName))
            {
                RefreshAvailableCEUpgradesList();
                Dialog.Info(GetString("UpgradeFileDeleted"));
            }
            else
                Dialog.Error(GetString("UpgradeFileDeletedFailed"));
        }

        private void DeleteDcuUpgradeFile(object fileName)
        {
            if (fileName == null)
                return;

            if (!Dialog.WarningQuestion(GetString("QuestionUpgradeFileDelete")))
                return;

            if (Plugin.MainServerProvider.DeleteUpgradeFile(UpgradeType.DCUUpgrade, fileName.ToString()))
            {
                RefreshAvailableDCUUpgradeList();
                Dialog.Info(GetString("UpgradeFileDeleted"));
            }
            else
                Dialog.Error(GetString("UpgradeFileDeletedFailed"));
        }

        private void DeleteCrUpgradeFile(object fileName)
        {
            if (!Dialog.WarningQuestion(GetString("QuestionUpgradeFileDelete")))
                return;

            var info = (CRUpgradeFileInfo)fileName;

            if (Plugin.MainServerProvider.DeleteUpgradeFile(
                UpgradeType.CRUpgrade,
                string.Format("{0}({1})",
                    info.Version,
                    info.CrHWVersion.ToString())))
            {
                RefreshAvailableCRUpgradeList();
                Dialog.Info(GetString("UpgradeFileDeleted"));
            }
            else
                Dialog.Error(GetString("UpgradeFileDeletedFailed"));
        }

        private void DeleteCcuUpgradeFile(object upgradeFileName)
        {
            if (!Dialog.WarningQuestion(GetString("QuestionUpgradeFileDelete")))
                return;

            if (Plugin.MainServerProvider.DeleteUpgradeFile(UpgradeType.CCUUpgrade, upgradeFileName as string))
            {
                RefreshAvailableUpgradesList();
                Dialog.Info(GetString("UpgradeFileDeleted"));
            }
            else
                Dialog.Error(GetString("UpgradeFileDeletedFailed"));
        }

        private void UploadCcuUpgradeFile()
        {
            _ofdBrowse.DefaultExt = "gz";
            _ofdBrowse.Filter = GetString("TextFilterForUpgradeCCU") + @"|CCU_*.gz";
            _ofdBrowse.Title = GetString("TextTitleOpenFileForUpgradeCCU");

            if (_ofdBrowse.ShowDialog() == DialogResult.OK)
            {
                SafeThread<UpgradeType, string>.StartThread(SendFileToServer, UpgradeType.CCUUpgrade, _ofdBrowse.FileName);
            }
        }

        private void _dgvCRUpgrading_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == _dgvCRUpgrading.Columns[CardReader.COLUMNSELECTUPGRADE].Index)
                {
                    if (_dgvCRUpgrading.Rows[e.RowIndex].Cells[CardReader.COLUMNONLINESTATE].Value.ToString() ==
                        GetString(OnlineState.Online.ToString()) ||
                        _dgvCRUpgrading.Rows[e.RowIndex].Cells[CardReader.COLUMNONLINESTATE].Value.ToString() ==
                        GetString(OnlineState.WaitingForUpgrade.ToString()))
                    {
                        _dgvCRUpgrading[e.ColumnIndex, e.RowIndex].ReadOnly = false;
                    }
                    else
                    {
                        _dgvCRUpgrading[e.ColumnIndex, e.RowIndex].ReadOnly = true;
                        _dgvCRUpgrading[e.ColumnIndex, e.RowIndex].Value = false;
                        _dgvCRUpgrading.EndEdit();
                        _dgvCRUpgrading.Rows[e.RowIndex].Cells[CardReader.COLUMNSELECTUPGRADE].Selected = false;
                    }
                }
            }
            catch
            {
            }
        }

        private void LoadSecurityLevelSettingsForCR()
        {
            _tbCrEvetlogLimitedSizeValue.Text = _editingObject.CrEventlogSize == 0
                ? "100"
                : _editingObject.CrEventlogSize.ToString(CultureInfo.InvariantCulture);
            _tbLastEventTimeForMarkAlarmArea.Text =
                _editingObject.CrLastEventTimeForMarkAlarmArea.ToString(CultureInfo.InvariantCulture);
        }

        private void SetSecurityLevelSettingsForCR()
        {
            _editingObject.CrEventlogSize = Int32.Parse(_tbCrEvetlogLimitedSizeValue.Text);
            _editingObject.CrLastEventTimeForMarkAlarmArea = Int32.Parse(_tbLastEventTimeForMarkAlarmArea.Text);
        }

        private void _tbCrEvetlogLimitedSizeValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            int isNumber;
            e.Handled = !int.TryParse(e.KeyChar.ToString(CultureInfo.InvariantCulture), out isNumber)
                        && e.KeyChar.ToString(CultureInfo.InvariantCulture) != "\b";
        }

        private void _tbThreadId_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_tbThreadId.Text))
            {
                uint id;
                if (UInt32.TryParse(_tbThreadId.Text, out id))
                    ShowThreadsInfo(_tbThreadId.Text, false);
            }
            else
                ShowThreadsInfo(null, false);
        }

        #region Alarm trnsmitter

        private void ShowAlarmTransmitter()
        {
            if (_alarmTransmitter == null)
            {
                _tbmAlarmTransmitter.Text = string.Empty;
            }
            else
            {
                _tbmAlarmTransmitter.Text = _alarmTransmitter.ToString();
                _tbmAlarmTransmitter.TextImage = Plugin.GetImageForAOrmObject(_alarmTransmitter);
            }
        }

        private void AddAlarmTransmitter(object newAlarmTransmitter)
        {
            try
            {
                var alarmTransmitter = newAlarmTransmitter as AlarmTransmitter;
                if (alarmTransmitter != null)
                {
                    SetAlarmTransmitter(alarmTransmitter);
                    Plugin.AddToRecentList(alarmTransmitter);
                }
                else
                {
                    ControlNotification.Singleton.Error(
                        NotificationPriority.JustOne,
                        _tbmAlarmTransmitter.ImageTextBox,
                        GetString("ErrorWrongObjectType"),
                        ControlNotificationSettings.Default);
                }
            }
            catch
            {
            }
        }

        private void SetAlarmTransmitter(AlarmTransmitter alarmTransmitter)
        {
            _alarmTransmitter = alarmTransmitter;
            ShowAlarmTransmitter();

            EditTextChanger(null, null);
        }

        private void ModifyAlarmTransmitter()
        {
            if (CgpClient.Singleton.IsConnectionLost(true))
                return;

            try
            {
                Exception error;
                var alarmTransmitters = Plugin.MainServerProvider.AlarmTransmitters.List(out error);

                if (alarmTransmitters == null)
                    return;

                var formAdd = new ListboxFormAdd(
                    alarmTransmitters.Cast<AOrmObject>(),
                    GetString("NCASAlarmTransmittersFormNCASAlarmTransmittersForm"));

                object selectedAlarmTransmitter;

                formAdd.ShowDialog(out selectedAlarmTransmitter);

                if (selectedAlarmTransmitter != null)
                {
                    var alarmTransmitter = selectedAlarmTransmitter as AlarmTransmitter;
                    SetAlarmTransmitter(alarmTransmitter);
                    Plugin.AddToRecentList(alarmTransmitter);
                }
            }
            catch
            {
            }
        }

        private void DoAfterCreatedAlarmTransmitter(object newAlarmTransmitter)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<object>(DoAfterCreatedAlarmTransmitter), newAlarmTransmitter);
            }
            else
            {
                var alarmTransmitter = newAlarmTransmitter as AlarmTransmitter;

                if (alarmTransmitter == null)
                    return;

                SetAlarmTransmitter(alarmTransmitter);
            }
        }

        private void _tbmAlarmTransmitter_ButtonPopupMenuItemClick(ToolStripItem item, int index)
        {
            switch (item.Name)
            {
                case "_tsiModify24":
                    ModifyAlarmTransmitter();
                    break;
                case "_tsiRemove24":
                    SetAlarmTransmitter(null);
                    break;
                case "_tsiCreate24":
                    var alarmTransmitter = new AlarmTransmitter();

                    NCASAlarmTransmittersForm.Singleton.OpenInsertFromEdit(
                        ref alarmTransmitter,
                        DoAfterCreatedAlarmTransmitter);

                    break;
            }
        }

        private void _tbmAlarmTransmitter_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void _tbmAlarmTransmitter_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                var output = e.Data.GetFormats();

                if (output == null)
                    return;

                AddAlarmTransmitter(e.Data.GetData(output[0]));
            }
            catch
            {

            }
        }

        private void _tbmAlarmTransmitter_ImageTextBox_DoubleClick(object sender, EventArgs e)
        {
            if (_alarmTransmitter == null)
                return;

            NCASAlarmTransmittersForm.Singleton.OpenEditForm(_alarmTransmitter);
        }

        #endregion

        private void _cbOpenAllAlarmSettings_CheckedChanged(object sender, EventArgs e)
        {
            OpenAllCheckStateChanged(
                _accordionAlarmSettings,
                _cbOpenAllAlarmSettings.CheckState);

            _accordionAlarmSettings.Focus();
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

        private bool isSpecialChar(char word)
        {
            return word == 'k' 
                || word == 'K' 
                || word == ',' 
                || word == '.';
        }

        private char? getSpecialChar(string text)
        {
            if (text.Contains("k"))
                return 'k';

            if (text.Contains("K"))
                return 'K';

            if (text.Contains("."))
                return '.';

            if (text.Contains(","))
                return ',';

            return null;
        }

        private void textBoxResistors_KeyPress(object sender, KeyPressEventArgs e)
        {
            string text = ((TextBox) sender).Text;

            if (Char.IsNumber(e.KeyChar)
                || (isSpecialChar(e.KeyChar)
                    && text.Length > 0
                    && getSpecialChar(text) == null)
                || e.KeyChar == '\b')
            {
                return;
            }
            
            e.Handled = true;
        }

        private float GetResistorValue(string text)
        {
            if (string.IsNullOrEmpty(text))
                return 0;

            char? specialChar = getSpecialChar(text);

            if (specialChar == null)
                return float.Parse(text);

            return float.Parse(text.Replace(specialChar.Value, '.'), CultureInfo.InvariantCulture);
        }

        private void _bRecalculate_Click(object sender, EventArgs e)
        {
            float R1 = GetResistorValue(_tbR1.Text);
            float R2 = GetResistorValue(_tbR2.Text);
            float Uref = 20;

            if (R1 < 1)
            {
                R1 = 1;
                _tbR1.Text = "1";
            }

            if (R2 < 1)
            {
                R2 = 1;
                _tbR2.Text = "1";
            }

            double normalMiddle = Math.Round(Uref*R2/(R2 + 8.2), 2);
            double alarmMiddle = Math.Round(Uref*(R1 + R2)/(R1 + R2 + 8.2), 2);
            double shortUpper = Math.Round(normalMiddle/2, 2);
            double normalUpper = Math.Round(normalMiddle + (alarmMiddle - normalMiddle)/2, 2);
            double alarmUpper = Math.Round((Uref - alarmMiddle)/2 + alarmMiddle, 2);

            _nShortNormal.Value = (decimal) shortUpper;
            _nNormalAlarm.Value = (decimal) normalUpper;
            _nAlarmBreak.Value = (decimal) alarmUpper;

            ShowInfoBsiLevel();

            _editingObject.R1 = _tbR1.Text;
            _editingObject.R2 = _tbR2.Text;
        }

        private void _bMakeLogDump_Click(object sender, EventArgs e)
        {
            if (_bMakeLogDump.Text == GetString("General_Download"))
            {
                var fileBytes = Plugin.MainServerProvider.CCUs.GetDebugFilesFromCcu(_editingObject.IPAddress);

                if (fileBytes == null)
                {
                    Dialog.Error(GetString("ErrorDuringDownloadingDebugFiles"));
                    _bMakeLogDump.Text = GetString("NCASCCUEditForm_bMakeLogDump");
                    return;
                }

                var saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "ZIP Files|*.zip";
                saveFileDialog.FileName = _editingObject.IPAddress + ".zip";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        File.WriteAllBytes(saveFileDialog.FileName, fileBytes);
                    }
                    catch (Exception)
                    {
                        Dialog.Error(GetString("ErrorDuringSaveDebugFiles"));
                        _bMakeLogDump.Text = GetString("NCASCCUEditForm_bMakeLogDump");
                        return;
                    }
                }

                _bMakeLogDump.Text = GetString("NCASCCUEditForm_bMakeLogDump");
                return;
            }

            if (!Plugin.MainServerProvider.CCUs.MakeLogDump(_editingObject.IdCCU))
            {
                Dialog.Error(GetString("ErrorMakeingLogDumpRunning"));
                return;
            }

            _bMakeLogDump.Enabled = false;
            _bMakeLogDump.Visible = false;
            _prbDebugFilesTransfer.Value = 0;
            _prbDebugFilesTransfer.Visible = true;
        }

        private void _bRefresh4_Click(object sender, EventArgs e)
        {
            if (CgpClient.Singleton.IsConnectionLost(false))
                return;

            SafeThread.StartThread(GetMemoryReport);
        }

        private void GetMemoryReport()
        {
            var memoryInfos = Plugin.MainServerProvider.GetMemoryReport(_editingObject.IdCCU);

            if (memoryInfos != null)
                ShowMeomoryInfos(memoryInfos);
        }

        private void ShowMeomoryInfos(ICollection<MemoryInfo> memoryInfos)
        {
            if (InvokeRequired)
            {
                BeginInvoke(
                    new Action<ICollection<MemoryInfo>>(ShowMeomoryInfos),
                    memoryInfos);

                return;
            }

            if (_cdgvMemory.LocalizationHelper == null)
                _cdgvMemory.LocalizationHelper = NCASClient.LocalizationHelper;

            var firstDisplayedRow = _cdgvMemory.DataGrid.FirstDisplayedScrollingRowIndex;
            var selectedRowIndex = -1;

            if (_cdgvMemory.SelectedRows != null && _cdgvMemory.SelectedRows.Count > 0)
                selectedRowIndex = _cdgvMemory.SelectedRows[0].Index;

            _cdgvMemory.ModifyGridView(
                new BindingSource
                {
                    DataSource = new SortableBindingList<MemoryInfo>(memoryInfos)
                },
                MemoryInfo.COLUMN_NAME,
                MemoryInfo.COLUMN_COUNT);

            if (firstDisplayedRow > 0 && _cdgvMemory.DataGrid.RowCount > firstDisplayedRow)
                _cdgvMemory.DataGrid.FirstDisplayedScrollingRowIndex = firstDisplayedRow;

            if (selectedRowIndex > 0 && _cdgvMemory.DataGrid.RowCount > selectedRowIndex)
            {
                _cdgvMemory.DataGrid.Rows[0].Selected = false;
                _cdgvMemory.DataGrid.Rows[selectedRowIndex].Selected = true;
            }

            _cdgvMemory.DataGrid.Columns[MemoryInfo.COLUMN_NAME].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            _cdgvMemory.DataGrid.Columns[MemoryInfo.COLUMN_COUNT].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        }

        private void AddProcTabReport(string message)
        {
            var text = DateTime.Now.ToString(DateFormatForProcReport) + "\t" + message;

            this.InvokeInUI(() =>
            {
                _lbCmdResults.Items.Add(text);
                _lbCmdResults.SelectedIndex = _lbCmdResults.Items.Count - 1;
            });
        }

        private void _bStopCKM_Click(object sender, EventArgs e)
        {
            AddProcTabReport("Request to stop CEKickstart was sent");

            SetEnableStateCmdUI(false);

            SafeThread.StartThread(() =>
            {
                bool result = Plugin.MainServerProvider.CCUs.StopCKM(_editingObject.IdCCU);

                _lbCmdResults.BeginInvoke(new Action(() =>
                {
                    var text = result
                        ? "CEKickstart stopped successfuly"
                        : "CEKickstart failed to stop";

                    AddProcTabReport(text);

                    SetEnableStateCmdUI(true);
                }));
            });
        }

        private void _bStartImplicityCKM_Click(object sender, EventArgs e)
        {
            AddProcTabReport("Request to start CEKickstart from implicit source sent");

            SetEnableStateCmdUI(false);

            SafeThread.StartThread(() =>
            {
                bool result = Plugin.MainServerProvider.CCUs.StartCKM(_editingObject.IdCCU, true);

                _lbCmdResults.BeginInvoke(new Action(() =>
                {
                    var text = result
                        ? "CEKickstart from implicit source started successfuly"
                        : "CEKickstart failed to start from implicit source";

                    AddProcTabReport(text);

                    SetEnableStateCmdUI(true);
                }));
            });
        }

        private const string DateFormatForProcReport = "dd.MM.yyyy hh:mm:ss";

        private void _bStartExplicityCKM_Click(object sender, EventArgs e)
        {
            AddProcTabReport("Request to start CEKickstart from explicit source sent");

            SetEnableStateCmdUI(false);

            SafeThread.StartThread(() =>
            {
                bool result = Plugin.MainServerProvider.CCUs.StartCKM(_editingObject.IdCCU, false);

                _lbCmdResults.BeginInvoke(new Action(() =>
                {
                    var text =result 
                        ? "CEKickstart started from explicit source successfuly" 
                        : "CEKickstart failed to start from explicit source";

                    AddProcTabReport(text);

                    SetEnableStateCmdUI(true);
                }));
            });
        }

        private void _bRunCmd_Click(object sender, EventArgs e)
        {
            AddProcTabReport("Request to run command was sent");

            SetEnableStateCmdUI(false);

            SafeThread.StartThread(() =>
            {
                bool result = Plugin.MainServerProvider.CCUs.RunProccess(_editingObject.IdCCU, _tbCommandLine.Text);

                _lbCmdResults.BeginInvoke(new Action(() =>
                {
                    var text = (result
                        ? "Command started successfuly"
                        : "Command failed to start");

                    AddProcTabReport(text);

                    SetEnableStateCmdUI(true);
                }));
            });
        }

        private void SetEnableStateCmdUI(bool isEnabled)
        {
            _bStopCKM.Enabled = isEnabled;
            _bStartImplicityCKM.Enabled = isEnabled;
            _bStartExplicityCKM.Enabled = isEnabled;
            _bRunCmd.Enabled = isEnabled;
        }

        private void _panelCmd_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            _lCommandLine.Visible = true;
            _tbCommandLine.Visible = true;
            _bRunCmd.Visible = true;
        }
    }

    public class CRUpgradeFileInfo
    {
        public CRUpgradeFileInfo(string versionFromServer, char delimeter)
        {
            if (string.IsNullOrEmpty(versionFromServer))
                return;

            var versionSubstrings = versionFromServer.Split(delimeter);
            if (versionSubstrings.Length < 2)
                return;

            _crHWVersion = (CRHWVersion)(Convert.ToByte(versionSubstrings[1], 16));
            _version = versionSubstrings[0];
            var localisedCRVersion = NCASClient.LocalizationHelper.GetString(_crHWVersion.ToString());
            _name = versionSubstrings[0] + " (" + localisedCRVersion + ")";
        }

        private readonly string _name;
        public string Name
        {
            get { return _name; }
        }

        private string _version;
        public string Version
        {
            get { return _version; }
            set { _version = value; }
        }

        private CRHWVersion _crHWVersion;
        public CRHWVersion CrHWVersion
        {
            get { return _crHWVersion; }
            set { _crHWVersion = value; }
        }

        public override string ToString()
        {
            return _name;
        }
    }

    public class DCUUpgradeFileInfo
    {
        public DCUUpgradeFileInfo(string versionFromServer, char delimeter)
        {
            if (versionFromServer == null || versionFromServer == string.Empty)
                return;

            var versionSubstrings = versionFromServer.Split(delimeter);
            if (versionSubstrings.Length < 2)
                return;

            _dcuHWVersion = (DCUHWVersion)(Convert.ToByte(versionSubstrings[1], 16));
            _version = versionSubstrings[0];
            var localisedDCUVersion = NCASClient.LocalizationHelper.GetString(_dcuHWVersion.ToString());
            _name = versionSubstrings[0] + " (" + localisedDCUVersion + ")";
        }

        private readonly string _name;
        public string Name
        {
            get { return _name; }
        }

        private string _version;
        public string Version
        {
            get { return _version; }
            set { _version = value; }
        }

        private DCUHWVersion _dcuHWVersion;
        public DCUHWVersion DcuHWVersion
        {
            get { return _dcuHWVersion; }
            set { _dcuHWVersion = value; }
        }

        public override string ToString()
        {
            return _name;
        }
    }

    public class OtherStatisticsValues
    {
        private readonly int _threadsCount;
        private readonly int _flashFreeSpace;
        private readonly int _flashSize;
        private readonly bool _sdCardPresent;
        private readonly int _sdCardFreeSpace;
        private readonly int _sdCardSize;
        private readonly int _freeMemory;
        private readonly int _totalMemory;
        private readonly int _memoryLoad;

        public int ThreadsCount
        {
            get { return _threadsCount; }
        }

        public int FlashFreeSpace
        {
            get { return _flashFreeSpace; }
        }

        public int FlashSize
        {
            get { return _flashSize; }
        }

        public bool SdCardPresent
        {
            get { return _sdCardPresent; }
        }

        public int SdCardFreeSpace
        {
            get { return _sdCardFreeSpace; }
        }

        public int SdCardSize
        {
            get { return _sdCardSize; }
        }

        public int FreeMemory
        {
            get { return _freeMemory; }
        }

        public int TotalMemory
        {
            get { return _totalMemory; }
        }

        public int MemoryLoad
        {
            get { return _memoryLoad; }
        }

        public OtherStatisticsValues(int threadsCount,
            int flashFreeSpace, int flashSize,
            bool sdCardPresent, int sdCardFreeSpace, int sdCardSize,
            int freeMemory, int totalMemory, int memoryLoad)
        {
            _threadsCount = threadsCount;
            _flashFreeSpace = flashFreeSpace;
            _flashSize = flashSize;
            _sdCardPresent = sdCardPresent;
            _sdCardFreeSpace = sdCardFreeSpace;
            _sdCardSize = sdCardSize;
            _freeMemory = freeMemory;
            _totalMemory = totalMemory;
            _memoryLoad = memoryLoad;
        }
    }
}
