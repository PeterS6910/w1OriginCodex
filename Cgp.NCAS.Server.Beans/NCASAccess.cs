using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Contal.Cgp.BaseLib;
using Contal.Cgp.Server.Beans;

namespace Contal.Cgp.NCAS.Server.Beans
{
    public static class AccessNcasGroups
    {
        public const string CARD_READERS = "CardReaders";
        public const string CCUS = "Ccus";
        public const string DCUS = "Dcus";
        public const string INPUTS = "Inputs";
        public const string OUTPUTS = "Outputs";
        public const string DOOR_ENVIRONMENTS = "DoorEnvironments";
        public const string ACCESS_CONTROL_LISTS = "AccessControlLists";
        public const string ACL_GROUPS = "AclGroups";
        public const string ALARM_AREAS = "AlarmAreas";
        public const string SDPS_STZS = "SdpsStzs";
        public const string ALARM_SETTINGS = "AlarmSettings";
        public const string WPF_GRAPHICS = "WpfGraphics";
        public const string ANTI_PASS_BACK_ZONES = "AntiPassBackZones";
        public const string MultiDoors = "MultiDoors";
        public const string MultiDoorElements = "MultiDoorElements";
        public const string Floors = "Floors";
        public const string AlarmTransmitters = "AlarmTransmitters";
        public const string AlarmArcs = "AlarmArcs";
        public const string LprCameras = "LprCameras";
    }
    
    public enum AccessNCAS : int
    {
        // Old devices management
        [Access(NCASAccess.SOURCE, null, AccessConstans.NULL_ACCESS_REFERENCE)]
        OldDevicesManagementView = 0,
        [Access(NCASAccess.SOURCE, null, AccessConstans.NULL_ACCESS_REFERENCE)]
        OldDevicesManagementAdmin = 1,

        // Card readers
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.CARD_READERS, AccessConstans.NULL_ACCESS_REFERENCE, null, (int)CardReadersInsertDeletePerform, AccessConstans.PERFORM_DESCRIPTION, 0)]
        CardReadersInsertDelete = 100,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.CARD_READERS, (int)OldDevicesManagementAdmin, new[] { (int)CardReadersSettingsAdmin }, null)]
        CardReadersInsertDeletePerform = 101,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.CARD_READERS, (int)CardReadersSettingsView, (int)CardReadersSettingsAdmin, 1)]
        CardReadersSettings = 102,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.CARD_READERS, (int)OldDevicesManagementView)]
        CardReadersSettingsView = 103,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.CARD_READERS, (int)OldDevicesManagementAdmin, null, new[] { (int)CardReadersInsertDeletePerform })]
        CardReadersSettingsAdmin = 104,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.CARD_READERS, (int)CardReadersInformationView, (int)CardReadersInformationAdmin ,2)]
        CardReadersInformation = 105,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.CARD_READERS, (int)OldDevicesManagementView)]
        CardReadersInformationView = 106,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.CARD_READERS, (int)OldDevicesManagementAdmin)]
        CardReadersInformationAdmin = 107,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.CARD_READERS, (int)CardReadersAlarmAreasView, AccessConstans.NULL_ACCESS_REFERENCE, 3)]
        CardReadersAlarmAreas = 108,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.CARD_READERS, (int)OldDevicesManagementView)]
        CardReadersAlarmAreasView = 109,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.CARD_READERS, (int)CardReadersAlarmSettingsView, (int)CardReadersAlarmSettingsAdmin, 4)]
        CardReadersAlarmSettings = 110,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.CARD_READERS, (int)OldDevicesManagementView)]
        CardReadersAlarmSettingsView = 111,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.CARD_READERS, (int)OldDevicesManagementAdmin)]
        CardReadersAlarmSettingsAdmin = 112,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.CARD_READERS, (int)CardReadersLanguageSettingsView, (int)CardReadersLanguageSettingsAdmin, 5)]
        CardReadersLanguageSettings = 113,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.CARD_READERS, (int)OldDevicesManagementView)]
        CardReadersLanguageSettingsView = 114,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.CARD_READERS, (int)OldDevicesManagementAdmin)]
        CardReadersLanguageSettingsAdmin = 115,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.CARD_READERS, (int)CardReadersActionPresentationSettingsView, (int)CardReadersActionPresentationSettingsAdmin, 6)]
        CardReadersActionPresentationSettings = 116,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.CARD_READERS, (int)OldDevicesManagementView)]
        CardReadersActionPresentationSettingsView = 117,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.CARD_READERS, (int)OldDevicesManagementAdmin)]
        CardReadersActionPresentationSettingsAdmin = 118,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.CARD_READERS, (int)CardReadersSpecialOutputsView, (int)CardReadersSpecialOutputsAdmin, 7)]
        CardReadersSpecialOutputs = 119,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.CARD_READERS, (int)OldDevicesManagementView)]
        CardReadersSpecialOutputsView = 120,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.CARD_READERS, (int)OldDevicesManagementAdmin)]
        CardReadersSpecialOutputsAdmin = 121,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.CARD_READERS, (int)CardReadersReferencedByView, AccessConstans.NULL_ACCESS_REFERENCE, 8)]
        CardReadersReferencedBy = 122,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.CARD_READERS, (int)OldDevicesManagementView)]
        CardReadersReferencedByView = 123,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.CARD_READERS, (int)CardReadersDescriptionView, (int)CardReadersDescriptionAdmin, 9)]
        CardReadersDescription = 124,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.CARD_READERS, (int)OldDevicesManagementView)]
        CardReadersDescriptionView = 125,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.CARD_READERS, (int)OldDevicesManagementAdmin)]
        CardReadersDescriptionAdmin = 126,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.CARD_READERS, (int)CardReadersLocalAlarmInstructionsView, (int)CardReadersLocalAlarmInstructionsAdmin, 10)]
        CardReadersLocalAlarmInstructions = 127,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.CARD_READERS, (int)OldDevicesManagementView)]
        CardReadersLocalAlarmInstructionsView = 128,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.CARD_READERS, (int)OldDevicesManagementAdmin)]
        CardReadersLocalAlarmInstructionsAdmin = 129,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.CARD_READERS, (int)CardReadersFunctionKeysSettingsView, (int)CardReadersFunctionKeysSettingsAdmin, 11)]
        CardReadersFunctionKeysSettings = 130,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.CARD_READERS, (int)OldDevicesManagementView)]
        CardReadersFunctionKeysSettingsView = 131,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.CARD_READERS, (int)OldDevicesManagementAdmin)]
        CardReadersFunctionKeysSettingsAdmin = 132,

        // LPR cameras
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.LprCameras, (int)LprCamerasView, AccessConstans.NULL_ACCESS_REFERENCE, 1)]
        LprCameras = 150,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.LprCameras, (int)OldDevicesManagementView)]
        LprCamerasView = 151,

        // CCUs
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.CCUS, AccessConstans.NULL_ACCESS_REFERENCE, null, (int)CcusInsertDeletePerform, AccessConstans.PERFORM_DESCRIPTION, 1)]
        CcusInsertDelete = 200,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.CCUS, (int)OldDevicesManagementAdmin, new[] { (int)CcusInformationAdmin }, null)]
        CcusInsertDeletePerform = 201,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.CCUS, (int)CcusInformationView, (int)CcusInformationAdmin, 2)]
        CcusInformation = 202,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.CCUS, (int)OldDevicesManagementView)]
        CcusInformationView = 203,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.CCUS, (int)OldDevicesManagementAdmin, null, new[] { (int)CcusInsertDeletePerform })]
        CcusInformationAdmin = 204,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.CCUS, (int)CcusTimeSettingsView, (int)CcusTimeSettingsAdmin, 3)]
        CcusTimeSettings = 205,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.CCUS, (int)OldDevicesManagementView)]
        CcusTimeSettingsView = 206,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.CCUS, (int)OldDevicesManagementAdmin)]
        CcusTimeSettingsAdmin = 207,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.CCUS, (int)CcusInputsOutputsView, AccessConstans.NULL_ACCESS_REFERENCE, 4)]
        CcusInputsOutputs = 208,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.CCUS, (int)OldDevicesManagementView)]
        CcusInputsOutputsView = 209,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.CCUS, (int)CcusControlView, (int)CcusControlAdmin, 5)]
        CcusControl = 210,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.CCUS, (int)OldDevicesManagementView)]
        CcusControlView = 211,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.CCUS, (int)OldDevicesManagementAdmin)]
        CcusControlAdmin = 212,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.CCUS, (int)CcusSettingsView, (int)CcusSettingsAdmin, 6)]
        CcusSettings = 213,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.CCUS, (int)OldDevicesManagementView)]
        CcusSettingsView = 214,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.CCUS, (int)OldDevicesManagementAdmin)]
        CcusSettingsAdmin = 215,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.CCUS, (int)CcusDoorEnvironmentsView, AccessConstans.NULL_ACCESS_REFERENCE, 7)]
        CcusDoorEnvironments = 216,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.CCUS, (int)OldDevicesManagementView)]
        CcusDoorEnvironmentsView = 217,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.CCUS, AccessConstans.NULL_ACCESS_REFERENCE, null, (int)CcusCrUpgradePerform, AccessConstans.PERFORM_DESCRIPTION, 8)]
        CcusCrUpgrade = 218,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.CCUS, (int)OldDevicesManagementAdmin)]
        CcusCrUpgradePerform = 219,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.CCUS, (int)CcusIpSettingsView, (int)CcusIpSettingsAdmin, 9)]
        CcusIpSettings = 220,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.CCUS, (int)OldDevicesManagementView)]
        CcusIpSettingsView = 221,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.CCUS, (int)OldDevicesManagementAdmin)]
        CcusIpSettingsAdmin = 222,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.CCUS, AccessConstans.NULL_ACCESS_REFERENCE, null, (int)CcusDcuUpgradePerform, AccessConstans.PERFORM_DESCRIPTION, 10)]
        CcusDcuUpgrade = 223,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.CCUS, (int)OldDevicesManagementAdmin)]
        CcusDcuUpgradePerform = 224,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.CCUS, (int)CcusAlarmSettingsView, (int)CcusAlarmSettingsAdmin, 11)]
        CcusAlarmSettings = 225,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.CCUS, (int)OldDevicesManagementView)]
        CcusAlarmSettingsView = 226,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.CCUS, (int)OldDevicesManagementAdmin)]
        CcusAlarmSettingsAdmin = 227,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.CCUS, (int)CcusUpsMonitorView, AccessConstans.NULL_ACCESS_REFERENCE, 12)]
        CcusUpsMonitor = 228,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.CCUS, (int)OldDevicesManagementView)]
        CcusUpsMonitorView = 229,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.CCUS, (int)CcusPerformanceStatisticsView, AccessConstans.NULL_ACCESS_REFERENCE, 13)]
        CcusPerformanceStatistics = 230,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.CCUS, (int)OldDevicesManagementView)]
        CcusPerformanceStatisticsView = 231,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.CCUS, (int)CcusCommunicationStatisticsView, (int)CcusCommunicationStatisticsAdmin, 14)]
        CcusCommunicationStatistics = 232,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.CCUS, (int)OldDevicesManagementView)]
        CcusCommunicationStatisticsView = 233,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.CCUS, (int)OldDevicesManagementAdmin)]
        CcusCommunicationStatisticsAdmin = 234,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.CCUS, (int)CcusThreadMapView, AccessConstans.NULL_ACCESS_REFERENCE, 15)]
        CcusThreadMap = 235,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.CCUS, (int)OldDevicesManagementView)]
        CcusThreadMapView = 236,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.CCUS, (int)CcusStartsCountCopprocessorVersionView, (int)CcusStartsCountCopprocessorVersionAdmin, 16)]
        CcusStartsCountCopprocessorVersion = 237,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.CCUS, (int)OldDevicesManagementView)]
        CcusStartsCountCopprocessorVersionView = 238,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.CCUS, (int)OldDevicesManagementAdmin)]
        CcusStartsCountCopprocessorVersionAdmin = 239,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.CCUS, (int)CcusBlockFilesInformationsView, (int)CcusBlockFilesInformationsAdmin, 17)]
        CcusBlockFilesInformations = 240,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.CCUS, (int)OldDevicesManagementView)]
        CcusBlockFilesInformationsView = 241,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.CCUS, (int)OldDevicesManagementAdmin)]
        CcusBlockFilesInformationsAdmin = 242,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.CCUS, AccessConstans.NULL_ACCESS_REFERENCE, null, (int)CcusTestingPerform, AccessConstans.PERFORM_DESCRIPTION, 18)]
        CcusTesting = 243,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.CCUS, (int)OldDevicesManagementAdmin)]
        CcusTestingPerform = 244,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.CCUS, (int)CcusReferencedByView, AccessConstans.NULL_ACCESS_REFERENCE, 19)]
        CcusReferencedBy = 245,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.CCUS, (int)OldDevicesManagementView)]
        CcusReferencedByView = 246,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.CCUS, (int)CcusDescriptionView, (int)CcusDescriptionAdmin, 20)]
        CcusDescription = 247,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.CCUS, (int)OldDevicesManagementView)]
        CcusDescriptionView = 248,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.CCUS, (int)OldDevicesManagementAdmin)]
        CcusDescriptionAdmin = 249,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.CCUS, (int)CcusLocalAlarmInstructionsView, (int)CcusLocalAlarmInstructionsAdmin, 21)]
        CcusLocalAlarmInstructions = 250,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.CCUS, (int)OldDevicesManagementView)]
        CcusLocalAlarmInstructionsView = 251,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.CCUS, (int)OldDevicesManagementAdmin)]
        CcusLocalAlarmInstructionsAdmin = 252,

        // DCUs
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.DCUS, AccessConstans.NULL_ACCESS_REFERENCE, null, (int)DcusInsertDeletePerform, AccessConstans.PERFORM_DESCRIPTION, 1)]
        DcusInsertDelete = 300,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.DCUS, (int)OldDevicesManagementAdmin, new[] { (int)DcusSettingsAdmin }, null)]
        DcusInsertDeletePerform = 301,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.DCUS, (int)DcusSettingsView, (int)DcusSettingsAdmin, 2)]
        DcusSettings = 302,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.DCUS, (int)OldDevicesManagementView)]
        DcusSettingsView = 303,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.DCUS, (int)OldDevicesManagementAdmin, null, new[] { (int)DcusInsertDeletePerform })]
        DcusSettingsAdmin = 304,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.DCUS, (int)DcusInputsOutputsView, AccessConstans.NULL_ACCESS_REFERENCE, 3)]
        DcusInputsOutputs = 305,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.DCUS, (int)OldDevicesManagementView)]
        DcusInputsOutputsView = 306,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.DCUS, (int)DcusControlView, AccessConstans.NULL_ACCESS_REFERENCE, 4)]
        DcusControl = 307,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.DCUS, (int)OldDevicesManagementView, null, new[] { (int)DcusResetPerform })]
        DcusControlView = 308,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.DCUS, AccessConstans.NULL_ACCESS_REFERENCE, null, (int)DcusResetPerform, AccessConstans.PERFORM_DESCRIPTION, 5)]
        DcusReset = 311,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.DCUS, (int)OldDevicesManagementAdmin, new[] { (int)DcusControlView }, null)]
        DcusResetPerform = 312,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.DCUS, AccessConstans.NULL_ACCESS_REFERENCE, null, (int)DcusCrUpgradePerform, AccessConstans.PERFORM_DESCRIPTION, 6)]
        DcusCrUpgrade = 313,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.DCUS, (int)OldDevicesManagementAdmin)]
        DcusCrUpgradePerform = 314,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.DCUS, (int)DcusAlarmSettingsView, (int)DcusAlarmSettingsAdmin, 7)]
        DcusAlarmSettings = 315,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.DCUS, (int)OldDevicesManagementView)]
        DcusAlarmSettingsView = 316,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.DCUS, (int)OldDevicesManagementAdmin)]
        DcusAlarmSettingsAdmin = 317,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.DCUS, (int)DcusSpecialOutputsView, (int)DcusSpecialOutputsAdmin, 8)]
        DcusSpecialOutputs = 318,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.DCUS, (int)OldDevicesManagementView)]
        DcusSpecialOutputsView = 319,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.DCUS, (int)OldDevicesManagementAdmin)]
        DcusSpecialOutputsAdmin = 320,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.DCUS, (int)DcusReferencedByView, AccessConstans.NULL_ACCESS_REFERENCE, 9)]
        DcusReferencedBy = 321,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.DCUS, (int)OldDevicesManagementView)]
        DcusReferencedByView = 322,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.DCUS, (int)DcusDescriptionView, (int)DcusDescriptionAdmin, 10)]
        DcusDescription = 323,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.DCUS, (int)OldDevicesManagementView)]
        DcusDescriptionView = 324,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.DCUS, (int)OldDevicesManagementAdmin)]
        DcusDescriptionAdmin = 325,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.DCUS, (int)DcusLocalAlarmInstructionsView, (int)DcusLocalAlarmInstructionsAdmin, 11)]
        DcusLocalAlarmInstructions = 326,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.DCUS, (int)OldDevicesManagementView)]
        DcusLocalAlarmInstructionsView = 327,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.DCUS, (int)OldDevicesManagementAdmin)]
        DcusLocalAlarmInstructionsAdmin = 328,

        // Inputs
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.INPUTS, AccessConstans.NULL_ACCESS_REFERENCE, null, (int)InputsInsertDeletePerform, AccessConstans.PERFORM_DESCRIPTION, 1)]
        InputsInsertDelete = 400,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.INPUTS, (int)OldDevicesManagementAdmin, new[] { (int)InputsSettingsAdmin }, null)]
        InputsInsertDeletePerform = 401,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.INPUTS, (int)InputsSettingsView, (int)InputsSettingsAdmin, 2)]
        InputsSettings = 402,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.INPUTS, (int)OldDevicesManagementView)]
        InputsSettingsView = 403,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.INPUTS, (int)OldDevicesManagementAdmin, null, new[] { (int)InputsInsertDeletePerform })]
        InputsSettingsAdmin = 404,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.INPUTS, (int)InputsAlarmSettingsView, (int)InputsAlarmSettingsAdmin, 3)]
        InputsAlarmSettings = 405,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.INPUTS, (int)OldDevicesManagementView)]
        InputsAlarmSettingsView = 406,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.INPUTS, (int)OldDevicesManagementAdmin)]
        InputsAlarmSettingsAdmin = 407,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.INPUTS, (int)InputsInputControlView, (int)InputsInputControlAdmin, 4)]
        InputsInputControl = 408,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.INPUTS, (int)OldDevicesManagementView)]
        InputsInputControlView = 409,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.INPUTS, (int)OldDevicesManagementAdmin)]
        InputsInputControlAdmin = 410,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.INPUTS, (int)InputsReferencedByView, AccessConstans.NULL_ACCESS_REFERENCE, 5)]
        InputsReferencedBy = 411,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.INPUTS, (int)OldDevicesManagementView)]
        InputsReferencedByView = 412,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.INPUTS, (int)InputsDescriptionView, (int)InputsDescriptionAdmin, 6)]
        InputsDescription = 413,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.INPUTS, (int)OldDevicesManagementView)]
        InputsDescriptionView = 414,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.INPUTS, (int)OldDevicesManagementAdmin)]
        InputsDescriptionAdmin = 415,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.INPUTS, (int)InputsLocalAlarmInstructionsView, (int)InputsLocalAlarmInstructionsAdmin, 7)]
        InputsLocalAlarmInstructions = 416,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.INPUTS, (int)OldDevicesManagementView)]
        InputsLocalAlarmInstructionsView = 417,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.INPUTS, (int)OldDevicesManagementAdmin)]
        InputsLocalAlarmInstructionsAdmin = 418,

        // Outputs
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.OUTPUTS, AccessConstans.NULL_ACCESS_REFERENCE, null, (int)OutputsInsertDeletePerform, AccessConstans.PERFORM_DESCRIPTION, 1)]
        OutputsInsertDelete = 500,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.OUTPUTS, (int)OldDevicesManagementAdmin, new[] { (int)OutputsSettingsAdmin }, null)]
        OutputsInsertDeletePerform = 501,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.OUTPUTS, (int)OutputsSettingsView, (int)OutputsSettingsAdmin, 2)]
        OutputsSettings = 502,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.OUTPUTS, (int)OldDevicesManagementView)]
        OutputsSettingsView = 503,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.OUTPUTS, (int)OldDevicesManagementAdmin, null, new[] { (int)OutputsInsertDeletePerform })]
        OutputsSettingsAdmin = 504,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.OUTPUTS, (int)OutputsAlarmSettingsView, (int)OutputsAlarmSettingsAdmin, 3)]
        OutputsAlarmSettings = 505,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.OUTPUTS, (int)OldDevicesManagementView)]
        OutputsAlarmSettingsView = 506,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.OUTPUTS, (int)OldDevicesManagementAdmin)]
        OutputsAlarmSettingsAdmin = 507,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.OUTPUTS, (int)OutputsOutputControlView, (int)OutputsOutputControlAdmin, 4)]
        OutputsOutputControl = 508,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.OUTPUTS, (int)OldDevicesManagementView)]
        OutputsOutputControlView = 509,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.OUTPUTS, (int)OldDevicesManagementAdmin)]
        OutputsOutputControlAdmin = 510,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.OUTPUTS, (int)OutputsReferencedByView, AccessConstans.NULL_ACCESS_REFERENCE, 5)]
        OutputsReferencedBy = 511,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.OUTPUTS, (int)OldDevicesManagementView)]
        OutputsReferencedByView = 512,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.OUTPUTS, (int)OutputsDescriptionView, (int)OutputsDescriptionAdmin, 6)]
        OutputsDescription = 513,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.OUTPUTS, (int)OldDevicesManagementView)]
        OutputsDescriptionView = 514,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.OUTPUTS, (int)OldDevicesManagementAdmin)]
        OutputsDescriptionAdmin = 515,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.OUTPUTS, (int)OutputsLocalAlarmInstructionsView, (int)OutputsLocalAlarmInstructionsAdmin, 7)]
        OutputsLocalAlarmInstructions = 516,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.OUTPUTS, (int)OldDevicesManagementView)]
        OutputsLocalAlarmInstructionsView = 517,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.OUTPUTS, (int)OldDevicesManagementAdmin)]
        OutputsLocalAlarmInstructionsAdmin = 518,

        // Door environments
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.DOOR_ENVIRONMENTS, AccessConstans.NULL_ACCESS_REFERENCE, null, (int)DoorEnvironmentsInsertDeletePerform, AccessConstans.PERFORM_DESCRIPTION, 1)]
        DoorEnvironmentsInsertDelete = 600,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.DOOR_ENVIRONMENTS, (int)OldDevicesManagementAdmin, new[] { (int)DoorEnvironmentsDoorTimingAdmin }, null)]
        DoorEnvironmentsInsertDeletePerform = 601,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.DOOR_ENVIRONMENTS, (int)DoorEnvironmentsDoorTimingView, (int)DoorEnvironmentsDoorTimingAdmin, 2)]
        DoorEnvironmentsDoorTiming = 602,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.DOOR_ENVIRONMENTS, (int)OldDevicesManagementView)]
        DoorEnvironmentsDoorTimingView = 603,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.DOOR_ENVIRONMENTS, (int)OldDevicesManagementAdmin, null, new[] { (int)DoorEnvironmentsInsertDeletePerform })]
        DoorEnvironmentsDoorTimingAdmin = 604,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.DOOR_ENVIRONMENTS, (int)DoorEnvironmentsApasView, (int)DoorEnvironmentsApasAdmin, 3)]
        DoorEnvironmentsApas = 605,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.DOOR_ENVIRONMENTS, (int)OldDevicesManagementView)]
        DoorEnvironmentsApasView = 606,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.DOOR_ENVIRONMENTS, (int)OldDevicesManagementAdmin)]
        DoorEnvironmentsApasAdmin = 607,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.DOOR_ENVIRONMENTS, AccessConstans.NULL_ACCESS_REFERENCE, null, (int)DoorEnvironmentsAccessGrantedPerform, AccessConstans.PERFORM_DESCRIPTION, 4)]
        DoorEnvironmentsAccessGranted = 608,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.DOOR_ENVIRONMENTS, (int)OldDevicesManagementAdmin)]
        DoorEnvironmentsAccessGrantedPerform = 609,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.DOOR_ENVIRONMENTS, (int)DoorEnvironmentsAlarmSettingsView, (int)DoorEnvironmentsAlarmSettingsAdmin, 5)]
        DoorEnvironmentsAlarmSettings = 610,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.DOOR_ENVIRONMENTS, (int)OldDevicesManagementView)]
        DoorEnvironmentsAlarmSettingsView = 611,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.DOOR_ENVIRONMENTS, (int)OldDevicesManagementAdmin)]
        DoorEnvironmentsAlarmSettingsAdmin = 612,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.DOOR_ENVIRONMENTS, (int)DoorEnvironmentsSpecialOutputsView, (int)DoorEnvironmentsSpecialOutputsAdmin, 6)]
        DoorEnvironmentsSpecialOutputs = 613,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.DOOR_ENVIRONMENTS, (int)OldDevicesManagementView)]
        DoorEnvironmentsSpecialOutputsView = 614,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.DOOR_ENVIRONMENTS, (int)OldDevicesManagementAdmin)]
        DoorEnvironmentsSpecialOutputsAdmin = 615,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.DOOR_ENVIRONMENTS, (int)DoorEnvironmentsReferencedByView, AccessConstans.NULL_ACCESS_REFERENCE, 7)]
        DoorEnvironmentsReferencedBy = 616,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.DOOR_ENVIRONMENTS, (int)OldDevicesManagementView)]
        DoorEnvironmentsReferencedByView = 617,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.DOOR_ENVIRONMENTS, (int)DoorEnvironmentsDescriptionView, (int)DoorEnvironmentsDescriptionAdmin, 8)]
        DoorEnvironmentsDescription = 618,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.DOOR_ENVIRONMENTS, (int)OldDevicesManagementView)]
        DoorEnvironmentsDescriptionView = 619,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.DOOR_ENVIRONMENTS, (int)OldDevicesManagementAdmin)]
        DoorEnvironmentsDescriptionAdmin = 620,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.DOOR_ENVIRONMENTS, (int)DoorEnvironmentsLocalAlarmInstructionsView, (int)DoorEnvironmentsLocalAlarmInstructionsAdmin, 9)]
        DoorEnvironmentsLocalAlarmInstructions = 621,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.DOOR_ENVIRONMENTS, (int)OldDevicesManagementView)]
        DoorEnvironmentsLocalAlarmInstructionsView = 622,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.DOOR_ENVIRONMENTS, (int)OldDevicesManagementAdmin)]
        DoorEnvironmentsLocalAlarmInstructionsAdmin = 623,

        // Old access control lists
        [Access(NCASAccess.SOURCE, null, AccessConstans.NULL_ACCESS_REFERENCE)]
        OldAcceessControlListsView = 2,
        [Access(NCASAccess.SOURCE, null, AccessConstans.NULL_ACCESS_REFERENCE)]
        OldAaccessControlListsAdmin = 3,
        
        // Access control lists
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.ACCESS_CONTROL_LISTS, AccessConstans.NULL_ACCESS_REFERENCE, null, (int)AclsInsertDeletePerform, AccessConstans.PERFORM_DESCRIPTION, 1)]
        AclsInsertDelete = 700,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.ACCESS_CONTROL_LISTS, (int)OldAaccessControlListsAdmin, new[] { (int)AclsSettingsAdmin }, null)]
        AclsInsertDeletePerform = 701,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.ACCESS_CONTROL_LISTS, (int)AclsSettingsView, (int)AclsSettingsAdmin, 2)]
        AclsSettings = 702,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.ACCESS_CONTROL_LISTS, (int)OldAcceessControlListsView)]
        AclsSettingsView = 703,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.ACCESS_CONTROL_LISTS, (int)OldAaccessControlListsAdmin, null, new[] { (int)AclsInsertDeletePerform })]
        AclsSettingsAdmin = 704,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.ACCESS_CONTROL_LISTS, (int)AclsAlarmAreasView, (int)AclsAlarmAreasAdmin, 3)]
        AclsAlarmAreas = 705,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.ACCESS_CONTROL_LISTS, (int)OldAcceessControlListsView)]
        AclsAlarmAreasView = 706,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.ACCESS_CONTROL_LISTS, (int)OldAaccessControlListsAdmin)]
        AclsAlarmAreasAdmin = 707,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.ACCESS_CONTROL_LISTS, (int)AclsReferencedByView, AccessConstans.NULL_ACCESS_REFERENCE, 4)]
        AclsReferencedBy = 708,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.ACCESS_CONTROL_LISTS, (int)OldAcceessControlListsView)]
        AclsReferencedByView = 709,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.ACCESS_CONTROL_LISTS, (int)AclsDescriptionView, (int)AclsDescriptionAdmin, 5)]
        AclsDescription = 710,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.ACCESS_CONTROL_LISTS, (int)OldAcceessControlListsView)]
        AclsDescriptionView = 711,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.ACCESS_CONTROL_LISTS, (int)OldAaccessControlListsAdmin)]
        AclsDescriptionAdmin = 712,

        // ACL groups
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.ACL_GROUPS, AccessConstans.NULL_ACCESS_REFERENCE, null, (int)AclGroupsInsertDeletePerform, AccessConstans.PERFORM_DESCRIPTION, 1)]
        AclGroupssInsertDelete = 720,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.ACL_GROUPS, (int)OldAaccessControlListsAdmin)]
        AclGroupsInsertDeletePerform = 721,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.ACL_GROUPS, (int)AclGroupsImplicitAssigningConfigurationView, (int)AclGroupsImplicitAssigningConfigurationAdmin, 2)]
        AclGroupsImplicitAssigningConfiguration = 722,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.ACL_GROUPS, (int)OldAcceessControlListsView)]
        AclGroupsImplicitAssigningConfigurationView = 723,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.ACL_GROUPS, (int)OldAaccessControlListsAdmin)]
        AclGroupsImplicitAssigningConfigurationAdmin = 724,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.ACL_GROUPS, (int)AclGroupsDepartmentsView, (int)AclGroupsDepartmentsAdmin, 3)]
        AclGroupsDepartments = 725,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.ACL_GROUPS, (int)OldAcceessControlListsView)]
        AclGroupsDepartmentsView = 726,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.ACL_GROUPS, (int)OldAaccessControlListsAdmin)]
        AclGroupsDepartmentsAdmin = 727,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.ACL_GROUPS, (int)AclGroupsAclsView, (int)AclGroupsAclsAdmin, 4)]
        AclGroupsAcls = 728,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.ACL_GROUPS, (int)OldAcceessControlListsView)]
        AclGroupsAclsView = 729,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.ACL_GROUPS, (int)OldAaccessControlListsAdmin)]
        AclGroupsAclsAdmin = 730,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.ACL_GROUPS, (int)AclGroupsReferencedByView, AccessConstans.NULL_ACCESS_REFERENCE, 5)]
        AclGroupsReferencedBy = 731,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.ACL_GROUPS, (int)OldAcceessControlListsView)]
        AclGroupsReferencedByView = 732,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.ACL_GROUPS, (int)AclGroupsDescriptionView, (int)AclGroupsDescriptionAdmin, 6)]
        AclGroupsDescription = 733,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.ACL_GROUPS, (int)OldAcceessControlListsView)]
        AclGroupsDescriptionView = 734,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.ACL_GROUPS, (int)OldAaccessControlListsAdmin)]
        AclGroupsDescriptionAdmin = 735,

        // Old alarm areas
        [Access(NCASAccess.SOURCE, null, AccessConstans.NULL_ACCESS_REFERENCE)]
        OldAlarmAreasView = 4,
        [Access(NCASAccess.SOURCE, null, AccessConstans.NULL_ACCESS_REFERENCE)]
        OldAlarmAreasAdmin = 5,

        // Alarm areas
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.ALARM_AREAS, AccessConstans.NULL_ACCESS_REFERENCE, null, (int)AlarmAreasInsertDeletePerform, AccessConstans.PERFORM_DESCRIPTION, 1)]
        AlarmAreasInsertDelete = 800,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.ALARM_AREAS, (int)OldAlarmAreasAdmin, new[] { (int)AlarmAreasBasicSettingsAdmin }, null)]
        AlarmAreasInsertDeletePerform = 801,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.ALARM_AREAS, (int)AlarmAreasBasicSettingsView, (int)AlarmAreasBasicSettingsAdmin, 2)]
        AlarmAreasBasicSettings = 802,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.ALARM_AREAS, (int)OldAlarmAreasView, null, new[] { (int)AlarmAreasSetPerform, (int)AlarmAreasUnsetPerform, (int)AlarmAreasUnconditionalSetPerform })]
        AlarmAreasBasicSettingsView = 803,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.ALARM_AREAS, (int)OldAlarmAreasAdmin, null, new[] { (int)AlarmAreasInsertDeletePerform })]
        AlarmAreasBasicSettingsAdmin = 804,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.ALARM_AREAS, AccessConstans.NULL_ACCESS_REFERENCE, null, (int)AlarmAreasSetPerform, AccessConstans.PERFORM_DESCRIPTION, 3)]
        AlarmAreasSet = 805,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.ALARM_AREAS, (int)OldAlarmAreasAdmin, new[] { (int)AlarmAreasBasicSettingsView }, null)]
        AlarmAreasSetPerform = 806,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.ALARM_AREAS, AccessConstans.NULL_ACCESS_REFERENCE, null, (int)AlarmAreasUnconditionalSetPerform, AccessConstans.PERFORM_DESCRIPTION, 4)]
        AlarmAreasUnconditionalSet = 809,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.ALARM_AREAS, (int)OldAlarmAreasAdmin, new[] { (int)AlarmAreasBasicSettingsView }, null)]
        AlarmAreasUnconditionalSetPerform = 810,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.ALARM_AREAS, AccessConstans.NULL_ACCESS_REFERENCE, null, (int)AlarmAreasUnsetPerform, AccessConstans.PERFORM_DESCRIPTION, 5)]
        AlarmAreasUnset = 807,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.ALARM_AREAS, (int)OldAlarmAreasAdmin, new[] { (int)AlarmAreasBasicSettingsView }, null)]
        AlarmAreasUnsetPerform = 808,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.ALARM_AREAS, AccessConstans.NULL_ACCESS_REFERENCE, null, (int)AlarmAreasTimeBuyingPerform, AccessConstans.PERFORM_DESCRIPTION, 6)]
        AlarmAreasTimeBuying = 831,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.ALARM_AREAS, (int)OldAlarmAreasAdmin, new[] { (int)AlarmAreasBasicSettingsView }, null)]
        AlarmAreasTimeBuyingPerform = 832,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.ALARM_AREAS, (int)AlarmAreasBasicTimingView, (int)AlarmAreasBasicTimingAdmin, 7)]
        AlarmAreasBasicTiming = 811,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.ALARM_AREAS, (int)OldAlarmAreasView)]
        AlarmAreasBasicTimingView = 812,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.ALARM_AREAS, (int)OldAlarmAreasAdmin)]
        AlarmAreasBasicTimingAdmin = 813,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.ALARM_AREAS, (int)AlarmAreasAlarmSettingsView, (int)AlarmAreasAlarmSettingsAdmin, 8)]
        AlarmAreasAlarmSettings = 814,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.ALARM_AREAS, (int)OldAlarmAreasView)]
        AlarmAreasAlarmSettingsView = 815,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.ALARM_AREAS, (int)OldAlarmAreasAdmin)]
        AlarmAreasAlarmSettingsAdmin = 816,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.ALARM_AREAS, (int)AlarmAreasSpecialOutputsView, (int)AlarmAreasSpecialOutputsAdmin, 9)]
        AlarmAreasSpecialOutputs = 817,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.ALARM_AREAS, (int)OldAlarmAreasView)]
        AlarmAreasSpecialOutputsView = 818,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.ALARM_AREAS, (int)OldAlarmAreasAdmin)]
        AlarmAreasSpecialOutputsAdmin = 819,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.ALARM_AREAS, (int)AlarmAreasReferencedByView, AccessConstans.NULL_ACCESS_REFERENCE, 10)]
        AlarmAreasReferencedBy = 823,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.ALARM_AREAS, (int)OldAlarmAreasView)]
        AlarmAreasReferencedByView = 824,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.ALARM_AREAS, (int)AlarmAreasDescriptionView, (int)AlarmAreasDescriptionAdmin, 11)]
        AlarmAreasDescription = 825,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.ALARM_AREAS, (int)OldAlarmAreasView)]
        AlarmAreasDescriptionView = 826,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.ALARM_AREAS, (int)OldAlarmAreasAdmin)]
        AlarmAreasDescriptionAdmin = 827,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.ALARM_AREAS, (int)AlarmAreasLocalAlarmInstructionsView, (int)AlarmAreasLocalAlarmInstructionsAdmin, 12)]
        AlarmAreasLocalAlarmInstructions = 828,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.ALARM_AREAS, (int)OldAlarmAreasView)]
        AlarmAreasLocalAlarmInstructionsView = 829,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.ALARM_AREAS, (int)OldAlarmAreasAdmin)]
        AlarmAreasLocalAlarmInstructionsAdmin = 830,

        // Security daily plans / security time zones
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.SDPS_STZS, AccessConstans.NULL_ACCESS_REFERENCE, null, (int)SdpsStzsInsertDeletePerform, AccessConstans.PERFORM_DESCRIPTION, 1)]
        SdpsStzsInsertDelete = 900,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.SDPS_STZS, (int)OldAaccessControlListsAdmin, new[] { (int)SdpsStzsScheduleAdmin, (int)SdpsStzsDateSettingsAdmin }, null)]
        SdpsStzsInsertDeletePerform = 901,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.SDPS_STZS, (int)SdpsStzsScheduleView, (int)SdpsStzsScheduleAdmin, 2)]
        SdpsStzsSchedule = 902,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.SDPS_STZS, (int)OldAcceessControlListsView)]
        SdpsStzsScheduleView = 903,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.SDPS_STZS, (int)OldAaccessControlListsAdmin, null, new[] { (int)SdpsStzsInsertDeletePerform })]
        SdpsStzsScheduleAdmin = 904,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.SDPS_STZS, (int)SdpsStzsDateSettingsView, (int)SdpsStzsDateSettingsAdmin, 3)]
        SdpsStzsDateSettings = 905,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.SDPS_STZS, (int)OldAcceessControlListsView)]
        SdpsStzsDateSettingsView = 906,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.SDPS_STZS, (int)OldAaccessControlListsAdmin, null, new[] { (int)SdpsStzsInsertDeletePerform })]
        SdpsStzsDateSettingsAdmin = 907,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.SDPS_STZS, (int)SdpsStzsDailyStatusView, AccessConstans.NULL_ACCESS_REFERENCE, 4)]
        SdpsStzsDailyStatus = 908,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.SDPS_STZS, (int)OldAcceessControlListsView)]
        SdpsStzsDailyStatusView = 909,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.SDPS_STZS, (int)SdpsStzsReferencedByView, AccessConstans.NULL_ACCESS_REFERENCE, 5)]
        SdpsStzsReferencedBy = 910,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.SDPS_STZS, (int)OldAcceessControlListsView)]
        SdpsStzsReferencedByView = 911,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.SDPS_STZS, (int)SdpsStzsDescriptionView, (int)SdpsStzsDescriptionAdmin, 6)]
        SdpsStzsDescription = 912,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.SDPS_STZS, (int)OldAcceessControlListsView)]
        SdpsStzsDescriptionView = 913,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.SDPS_STZS, (int)OldAaccessControlListsAdmin)]
        SdpsStzsDescriptionAdmin = 914,

        // Alarm settings
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.ALARM_SETTINGS, (int)AlarmSettingsAaAlarmsView, (int)AlarmSettingsAaAlarmsAdmin, 1)]
        AlarmSettingsAaAlarms = 1000,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.ALARM_SETTINGS, (int)OldDevicesManagementView)]
        AlarmSettingsAaAlarmsView = 1001,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.ALARM_SETTINGS, (int)OldDevicesManagementAdmin)]
        AlarmSettingsAaAlarmsAdmin = 1002,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.ALARM_SETTINGS, (int)AlarmSettingsCcuAlarmsView, (int)AlarmSettingsCcuAlarmsAdmin, 2)]
        AlarmSettingsCcuAlarms = 1003,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.ALARM_SETTINGS, (int)OldDevicesManagementView)]
        AlarmSettingsCcuAlarmsView = 1004,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.ALARM_SETTINGS, (int)OldDevicesManagementAdmin)]
        AlarmSettingsCcuAlarmsAdmin = 1005,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.ALARM_SETTINGS, (int)AlarmSettingsDcuAlarmsView, (int)AlarmSettingsDcuAlarmsAdmin, 3)]
        AlarmSettingsDcuAlarms = 1006,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.ALARM_SETTINGS, (int)OldDevicesManagementView)]
        AlarmSettingsDcuAlarmsView = 1007,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.ALARM_SETTINGS, (int)OldDevicesManagementAdmin)]
        AlarmSettingsDcuAlarmsAdmin = 1008,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.ALARM_SETTINGS, (int)AlarmSettingsDeAlarmsView, (int)AlarmSettingsDeAlarmsAdmin, 4)]
        AlarmSettingsDeAlarms = 1012,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.ALARM_SETTINGS, (int)OldDevicesManagementView)]
        AlarmSettingsDeAlarmsView = 1013,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.ALARM_SETTINGS, (int)OldDevicesManagementAdmin)]
        AlarmSettingsDeAlarmsAdmin = 1014,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.ALARM_SETTINGS, (int)AlarmSettingsCrAlarmsView, (int)AlarmSettingsCrAlarmsAdmin, 5)]
        AlarmSettingsCrAlarms = 1009,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.ALARM_SETTINGS, (int)OldDevicesManagementView)]
        AlarmSettingsCrAlarmsView = 1010,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.ALARM_SETTINGS, (int)OldDevicesManagementAdmin)]
        AlarmSettingsCrAlarmsAdmin = 1011,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.ALARM_SETTINGS, (int)AlarmSettingsInvalidPinGinRetriesLimitsView, (int)AlarmSettingsInvalidPinGinRetriesLimitsdmin, 6)]
        AlarmSettingsInvalidPinGinRetriesLimits = 1015,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.ALARM_SETTINGS, (int)OldDevicesManagementView)]
        AlarmSettingsInvalidPinGinRetriesLimitsView = 1016,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.ALARM_SETTINGS, (int)OldDevicesManagementAdmin)]
        AlarmSettingsInvalidPinGinRetriesLimitsdmin = 1017,

        // WPF graphics
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.WPF_GRAPHICS, (int)WpfGraphicsView, (int)WpfGraphicsAdmin, 1)]
        WpfGraphics = 1100,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.WPF_GRAPHICS, AccessConstans.NULL_ACCESS_REFERENCE)]
        WpfGraphicsView = 1101,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.WPF_GRAPHICS, AccessConstans.NULL_ACCESS_REFERENCE)]
        WpfGraphicsAdmin = 1102,

        // Anti pass back zones
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.ANTI_PASS_BACK_ZONES, (int)AntiPassBackZonesView, (int)AntiPassBackZonesAdmin, 1)]
        AntiPassBackZones = 1200,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.ANTI_PASS_BACK_ZONES, AccessConstans.NULL_ACCESS_REFERENCE)]
        AntiPassBackZonesView = 1201,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.ANTI_PASS_BACK_ZONES, AccessConstans.NULL_ACCESS_REFERENCE)]
        AntiPassBackZonesAdmin = 1202,

        // Multi doors
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.MultiDoors, AccessConstans.NULL_ACCESS_REFERENCE, null, (int)MultiDoorsInsertDeletePerform, AccessConstans.PERFORM_DESCRIPTION, 1)]
        MultiDoorsInsertDelete = 1300,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.MultiDoors, (int)OldDevicesManagementAdmin, new[] { (int)MultiDoorsSettingsAdmin }, null)]
        MultiDoorsInsertDeletePerform = 1301,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.MultiDoors, (int)MultiDoorsSettingsView, (int)MultiDoorsSettingsAdmin, 2)]
        MultiDoorsSettings = 1302,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.MultiDoors, (int)OldDevicesManagementView)]
        MultiDoorsSettingsView = 1303,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.MultiDoors, (int)OldDevicesManagementAdmin, null, new[] { (int)MultiDoorsInsertDeletePerform })]
        MultiDoorsSettingsAdmin = 1304,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.MultiDoors, (int)MultiDoorsApasView, (int)MultiDoorsApasAdmin, 3)]
        MultiDoorsApas = 1305,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.MultiDoors, (int)OldDevicesManagementView)]
        MultiDoorsApasView = 1306,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.MultiDoors, (int)OldDevicesManagementAdmin)]
        MultiDoorsApasAdmin = 1307,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.MultiDoors, (int)MultiDoorsDoorsView, (int)MultiDoorsDoorsAdmin, 4)]
        MultiDoorsDoors = 1308,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.MultiDoors, (int)OldDevicesManagementView)]
        MultiDoorsDoorsView = 1309,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.MultiDoors, (int)OldDevicesManagementAdmin)]
        MultiDoorsDoorsAdmin = 1310,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.MultiDoors, (int)MultiDoorsReferencedByView, AccessConstans.NULL_ACCESS_REFERENCE, 5)]
        MultiDoorsReferencedBy = 1311,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.MultiDoors, (int)OldDevicesManagementAdmin)]
        MultiDoorsReferencedByView = 1312,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.MultiDoors, (int)MultiDoorsDescriptionView, (int)MultiDoorsDescriptionAdmin, 6)]
        MultiDoorsDescription = 1313,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.MultiDoors, (int)OldDevicesManagementAdmin)]
        MultiDoorsDescriptionView = 1314,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.MultiDoors, (int)OldDevicesManagementAdmin)]
        MultiDoorsDescriptionAdmin = 1315,

        // Multi door elements
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.MultiDoorElements, AccessConstans.NULL_ACCESS_REFERENCE, null, (int)MultiDoorElementsInsertDeletePerform, AccessConstans.PERFORM_DESCRIPTION, 1)]
        MultiDoorElementsInsertDelete = 1400,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.MultiDoorElements, (int)OldDevicesManagementAdmin, new[] { (int)MultiDoorElementsSettingsAdmin }, null)]
        MultiDoorElementsInsertDeletePerform = 1401,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.MultiDoorElements, (int)MultiDoorElementsSettingsView, (int)MultiDoorElementsSettingsAdmin, 2)]
        MultiDoorElementsSettings = 1402,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.MultiDoorElements, (int)OldDevicesManagementView)]
        MultiDoorElementsSettingsView = 1403,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.MultiDoorElements, (int)OldDevicesManagementAdmin, null, new[] { (int)MultiDoorElementsInsertDeletePerform })]
        MultiDoorElementsSettingsAdmin = 1404,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.MultiDoorElements, (int)MultiDoorElementsApasView, (int)MultiDoorElementsApasAdmin, 3)]
        MultiDoorElementsApas = 1405,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.MultiDoorElements, (int)OldDevicesManagementView)]
        MultiDoorElementsApasView = 1406,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.MultiDoorElements, (int)OldDevicesManagementAdmin)]
        MultiDoorElementsApasAdmin = 1407,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.MultiDoorElements, (int)MultiDoorElementsReferencedByView, AccessConstans.NULL_ACCESS_REFERENCE, 4)]
        MultiDoorElementsReferencedBy = 1408,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.MultiDoorElements, (int)OldDevicesManagementAdmin)]
        MultiDoorElementsReferencedByView = 1409,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.MultiDoorElements, (int)MultiDoorElementsDescriptionView, (int)MultiDoorElementsDescriptionAdmin, 5)]
        MultiDoorElementsDescription = 1410,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.MultiDoorElements, (int)OldDevicesManagementAdmin)]
        MultiDoorElementsDescriptionView = 1411,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.MultiDoorElements, (int)OldDevicesManagementAdmin)]
        MultiDoorElementsDescriptionAdmin = 1412,

        // Floors
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.Floors, AccessConstans.NULL_ACCESS_REFERENCE, null, (int)FloorsInsertDeletePerform, AccessConstans.PERFORM_DESCRIPTION, 1)]
        FloorsInsertDelete = 1500,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.Floors, (int)OldDevicesManagementAdmin, new[] { (int)FloorsSettingsAdmin }, null)]
        FloorsInsertDeletePerform = 1501,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.Floors, (int)FloorsSettingsView, (int)FloorsSettingsAdmin, 2)]
        FloorsSettings = 1502,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.Floors, (int)OldDevicesManagementView)]
        FloorsSettingsView = 1503,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.Floors, (int)OldDevicesManagementAdmin, null, new[] { (int)FloorsInsertDeletePerform })]
        FloorsSettingsAdmin = 1504,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.Floors, (int)FloorsDoorsView, (int)FloorsDoorsAdmin, 3)]
        FloorsDoors = 1505,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.Floors, (int)OldDevicesManagementView)]
        FloorsDoorsView = 1506,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.Floors, (int)OldDevicesManagementAdmin)]
        FloorsDoorsAdmin = 1507,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.Floors, (int)FloorsReferencedByView, AccessConstans.NULL_ACCESS_REFERENCE, 4)]
        FloorsReferencedBy = 1508,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.Floors, (int)OldDevicesManagementAdmin)]
        FloorsReferencedByView = 1509,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.Floors, (int)FloorsDescriptionView, (int)FloorsDescriptionAdmin, 5)]
        FloorsDescription = 1510,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.Floors, (int)OldDevicesManagementAdmin)]
        FloorsDescriptionView = 1511,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.Floors, (int)OldDevicesManagementAdmin)]
        FloorsDescriptionAdmin = 1512,

        // Alarm transmitters
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.AlarmTransmitters, AccessConstans.NULL_ACCESS_REFERENCE, null, (int)AlarmTransmittersInsertDeletePerform, AccessConstans.PERFORM_DESCRIPTION, 1)]
        AlarmTransmittersInsertDelete = 1600,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.AlarmTransmitters, (int)OldDevicesManagementAdmin, new[] { (int)AlarmTransmittersSettingsAdmin }, null)]
        AlarmTransmittersInsertDeletePerform = 1601,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.AlarmTransmitters, (int)AlarmTransmittersSettingsView, (int)AlarmTransmittersSettingsAdmin, 2)]
        AlarmTransmittersSettings = 1602,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.AlarmTransmitters, (int)OldDevicesManagementView)]
        AlarmTransmittersSettingsView = 1603,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.AlarmTransmitters, (int)OldDevicesManagementAdmin, null, new[] { (int)AlarmTransmittersInsertDeletePerform })]
        AlarmTransmittersSettingsAdmin = 1604,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.AlarmTransmitters, (int)AlarmTransmittersReferencedByView, AccessConstans.NULL_ACCESS_REFERENCE, 3)]
        AlarmTransmittersReferencedBy = 1605,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.AlarmTransmitters, (int)OldDevicesManagementAdmin)]
        AlarmTransmittersReferencedByView = 1606,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.AlarmTransmitters, (int)AlarmTransmittersDescriptionView, (int)AlarmTransmittersDescriptionAdmin, 4)]
        AlarmTransmittersDescription = 1607,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.AlarmTransmitters, (int)OldDevicesManagementView)]
        AlarmTransmittersDescriptionView = 1608,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.AlarmTransmitters, (int)OldDevicesManagementAdmin)]
        AlarmTransmittersDescriptionAdmin = 1609,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.AlarmTransmitters, (int)AlarmTransmittersLocalAlarmInstructionsView, (int)AlarmTransmittersLocalAlarmInstructionsAdmin, 5)]
        AlarmTransmittersLocalAlarmInstructions = 1610,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.AlarmTransmitters, (int)OldDevicesManagementView)]
        AlarmTransmittersLocalAlarmInstructionsView = 1611,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.AlarmTransmitters, (int)OldDevicesManagementAdmin)]
        AlarmTransmittersLocalAlarmInstructionsAdmin = 1612,

        // Alarm arcs
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.AlarmArcs, AccessConstans.NULL_ACCESS_REFERENCE, null, (int)AlarmArcsInsertDeletePerform, AccessConstans.PERFORM_DESCRIPTION, 1)]
        AlarmArcsInsertDelete = 1700,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.AlarmArcs, (int)OldDevicesManagementAdmin, new[] { (int)AlarmArcsSettingsAdmin }, null)]
        AlarmArcsInsertDeletePerform = 1701,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.AlarmArcs, (int)AlarmArcsSettingsView, (int)AlarmArcsSettingsAdmin, 2)]
        AlarmArcsSettings = 1702,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.AlarmArcs, (int)OldDevicesManagementView)]
        AlarmArcsSettingsView = 1703,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.AlarmArcs, (int)OldDevicesManagementAdmin, null, new[] { (int)AlarmArcsInsertDeletePerform })]
        AlarmArcsSettingsAdmin = 1704,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.AlarmArcs, (int)AlarmArcsReferencedByView, AccessConstans.NULL_ACCESS_REFERENCE, 3)]
        AlarmArcsReferencedBy = 1705,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.AlarmArcs, (int)OldDevicesManagementAdmin)]
        AlarmArcsReferencedByView = 1706,
        [AccessPresentation(NCASAccess.SOURCE, AccessNcasGroups.AlarmArcs, (int)AlarmArcsDescriptionView, (int)AlarmArcsDescriptionAdmin, 4)]
        AlarmArcsDescription = 1707,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.AlarmArcs, (int)OldDevicesManagementView)]
        AlarmArcsDescriptionView = 1708,
        [Access(NCASAccess.SOURCE, AccessNcasGroups.AlarmArcs, (int)OldDevicesManagementAdmin)]
        AlarmArcsDescriptionAdmin = 1709
    }

    public static class NCASAccess
    {
        public const string SOURCE = "Access";

        public static Access GetAccess(AccessNCAS enumAccess)
        {
            var attribs = typeof(AccessNCAS).GetField(enumAccess.ToString()).GetCustomAttributes(
                typeof(AccessAttribute), false) as AccessAttribute[];

            if (attribs != null && attribs.Length > 0)
            {
                return new Access((int) enumAccess, attribs[0].Source, attribs[0].Group, attribs[0].GeneralAccess);
            }

            return null;
        }

        public static List<Access> GetAccessesForGroup(string accessGroup)
        {
            var accesses = new List<Access>();
            foreach (AccessNCAS ncasAccess in Enum.GetValues(typeof(AccessNCAS)))
            {
                var attribs =
                    typeof(AccessNCAS).GetField(ncasAccess.ToString())
                        .GetCustomAttributes(typeof (AccessAttribute), false) as AccessAttribute[];

                if (attribs != null && attribs.Length > 0)
                {
                    if (attribs[0].Group == accessGroup)
                        accesses.Add(new Access((int) ncasAccess,
                            attribs[0].Source,
                            attribs[0].Group,
                            attribs[0].GeneralAccess));
                }
            }

            return accesses;
        }

        public static Dictionary<string, List<AccessPresentation>> GetAccessList()
        {
            var accesses = new Dictionary<string, List<AccessPresentation>>();
            foreach (AccessNCAS accessNcas in Enum.GetValues(typeof(AccessNCAS)))
            {
                var attribs =
                    typeof (AccessNCAS).GetField(accessNcas.ToString())
                        .GetCustomAttributes(typeof (AccessPresentationAttribute), false) as
                        AccessPresentationAttribute[];

                if (attribs != null && attribs.Length > 0)
                {
                    int? generalAccessView = null;
                    int[] forcedSetAccessesView = null;
                    int[] forcedUnsetAccessesView = null;
                    if (attribs[0].AccessView != null)
                    {
                        var attribsAccessView =
                            typeof(AccessNCAS).GetField(((AccessNCAS)attribs[0].AccessView.Value).ToString())
                                .GetCustomAttributes(typeof (AccessAttribute), false) as AccessAttribute[];

                        if (attribsAccessView != null && attribsAccessView.Length > 0)
                        {
                            generalAccessView = attribsAccessView[0].GeneralAccess;
                            forcedSetAccessesView = attribsAccessView[0].ForcedSetAccesses;
                            forcedUnsetAccessesView = attribsAccessView[0].ForcedUnsetAccesses;
                        }
                    }

                    int? generalAccessAdmin = null;
                    int[] forcedSetAccessesAdmin = null;
                    int[] forcedUnsetAccessesAdmin = null;
                    if (attribs[0].AccessAdmin != null)
                    {
                        var attribsAccessAdmin =
                            typeof(AccessNCAS).GetField(((AccessNCAS)attribs[0].AccessAdmin.Value).ToString())
                                .GetCustomAttributes(typeof (AccessAttribute), false) as AccessAttribute[];

                        if (attribsAccessAdmin != null && attribsAccessAdmin.Length > 0)
                        {
                            generalAccessAdmin = attribsAccessAdmin[0].GeneralAccess;
                            forcedSetAccessesAdmin = attribsAccessAdmin[0].ForcedSetAccesses;
                            forcedUnsetAccessesAdmin = attribsAccessAdmin[0].ForcedUnsetAccesses;
                        }
                    }

                    var accessPresentation = new AccessPresentation(
                        accessNcas.ToString(),
                        attribs[0].Source,
                        attribs[0].Group,
                        attribs[0].AccessView,
                        generalAccessView,
                        forcedSetAccessesView,
                        forcedUnsetAccessesView,
                        attribs[0].CustomAccessViewDescription,
                        attribs[0].AccessAdmin,
                        generalAccessAdmin,
                        forcedSetAccessesAdmin,
                        forcedUnsetAccessesAdmin,
                        attribs[0].CustomAccessAdminDescription,
                        attribs[0].ViewIndex);

                    List<AccessPresentation> list;
                    if (!accesses.TryGetValue(accessPresentation.Group, out list))
                    {
                        list = new List<AccessPresentation>();
                        accesses.Add(accessPresentation.Group, list);
                    }

                    list.Add(accessPresentation);
                }
            }

            return accesses;
        }        
    }
}
