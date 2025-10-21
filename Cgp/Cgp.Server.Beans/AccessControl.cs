using System;
using System.Collections.Generic;

using Contal.Cgp.BaseLib;
using Contal.Cgp.Globals;

namespace Contal.Cgp.Server.Beans
{
    public static class LoginAccessGroups
    {
        public const string LOGINS_LOGIN_GROUPS = "LoginsLoginGroups";
        public const string GENERAL_OPTIONS = "GeneralOptions";
        public const string PERSONS = "Persons";
        public const string EVENTLOGS = "Eventlogs";
        public const string SYSTEM_EVENTS = "SystemEvents";
        public const string DAILY_PLANS_TIME_ZONES = "DailyPlansTimeZones";
        public const string CALENDARS_DAY_TYPES = "CalendarsDayTypes";
        public const string CARDS = "Cards";
        public const string CARD_SYSTEMS = "CardSystems";
        public const string ID_MANAGEMENT = "IdManagement";
        public const string PRESENT_GROUPS_FORMATTERS = "PresentGroupsFormatters";
        public const string CIS_NGS_CIS_NG_GROUPS = "CisNgsCisNgGroups";
        public const string ALARMS = "Alarms";
        public const string GLOBAL_ALARM_INSTRUCTIONS = "GlobalAlarmInstructions";
        public const string UNLOCK_CLIENT_APLICATION = "UnlockClientAplication";
        public const string SEARCH = "Search";
        public const string FOLDERS_SRUCTURE = "FoldersStructure";
        public const string LOCAL_SOUNDS = "LocalSounds";
        public const string STRUCTURED_SUB_SITE = "StructuredSubSite";
    }

    public enum LoginAccess : int
    {
        // Super admin
        [Access(null, null, AccessConstans.NULL_ACCESS_REFERENCE)]
        SuperAdmin = 0,

        // Old generalOptions
        [Access(null, null, AccessConstans.NULL_ACCESS_REFERENCE)]
        OldGeneralOptionsAdmin = 3,

        // General options
        [AccessPresentation(null, LoginAccessGroups.GENERAL_OPTIONS, (int)GeneralOptionsLicenseInfoView, (int)GeneralOptionsLicenseInfoAdmin, 1)]
        GeneralOptionsLicenseInfo = 100,
        [Access(null, LoginAccessGroups.GENERAL_OPTIONS, AccessConstans.NULL_ACCESS_REFERENCE)]
        GeneralOptionsLicenseInfoView = 101,
        [Access(null, LoginAccessGroups.GENERAL_OPTIONS, (int)OldGeneralOptionsAdmin)]
        GeneralOptionsLicenseInfoAdmin = 102,
        [AccessPresentation(null, LoginAccessGroups.GENERAL_OPTIONS, (int)GeneralOptionsServerControlView, (int)GeneralOptionsServerControlAdmin, 2)]
        GeneralOptionsServerControl = 103,
        [Access(null, LoginAccessGroups.GENERAL_OPTIONS, AccessConstans.NULL_ACCESS_REFERENCE)]
        GeneralOptionsServerControlView = 104,
        [Access(null, LoginAccessGroups.GENERAL_OPTIONS, (int)OldGeneralOptionsAdmin)]
        GeneralOptionsServerControlAdmin = 105,
        [AccessPresentation(null, LoginAccessGroups.GENERAL_OPTIONS, (int)GeneralOptionsRemoteServiceSettingsView, (int)GeneralOptionsRemoteServiceSettingsAdmin, 3)]
        GeneralOptionsRemoteServiceSettings = 106,
        [Access(null, LoginAccessGroups.GENERAL_OPTIONS, AccessConstans.NULL_ACCESS_REFERENCE)]
        GeneralOptionsRemoteServiceSettingsView = 107,
        [Access(null, LoginAccessGroups.GENERAL_OPTIONS, (int)OldGeneralOptionsAdmin)]
        GeneralOptionsRemoteServiceSettingsAdmin = 108,
        [AccessPresentation(null, LoginAccessGroups.GENERAL_OPTIONS, (int)GeneralOptionsDhcpServerView, (int)GeneralOptionsDhcpServerAdmin, 4)]
        GeneralOptionsDhcpServer = 109,
        [Access(null, LoginAccessGroups.GENERAL_OPTIONS, AccessConstans.NULL_ACCESS_REFERENCE)]
        GeneralOptionsDhcpServerView = 110,
        [Access(null, LoginAccessGroups.GENERAL_OPTIONS, (int)OldGeneralOptionsAdmin)]
        GeneralOptionsDhcpServerAdmin = 111,
        [AccessPresentation(null, LoginAccessGroups.GENERAL_OPTIONS, (int)GeneralOptionsDatabaseSettingsView, (int)GeneralOptionsDatabaseSettingsAdmin, 5)]
        GeneralOptionsDatabaseSettings = 112,
        [Access(null, LoginAccessGroups.GENERAL_OPTIONS, AccessConstans.NULL_ACCESS_REFERENCE)]
        GeneralOptionsDatabaseSettingsView = 113,
        [Access(null, LoginAccessGroups.GENERAL_OPTIONS, (int)OldGeneralOptionsAdmin)]
        GeneralOptionsDatabaseSettingsAdmin = 114,
        [AccessPresentation(null, LoginAccessGroups.GENERAL_OPTIONS, (int)GeneralOptionsSecuritySettingsView, (int)GeneralOptionsSecuritySettingsAdmin, 6)]
        GeneralOptionsSecuritySettings = 115,
        [Access(null, LoginAccessGroups.GENERAL_OPTIONS, AccessConstans.NULL_ACCESS_REFERENCE)]
        GeneralOptionsSecuritySettingsView = 116,
        [Access(null, LoginAccessGroups.GENERAL_OPTIONS, (int)OldGeneralOptionsAdmin)]
        GeneralOptionsSecuritySettingsAdmin = 117,
        [AccessPresentation(null, LoginAccessGroups.GENERAL_OPTIONS, (int)GeneralOptionsAlarmSettingsView, (int)GeneralOptionsAlarmSettingsAdmin, 7)]
        GeneralOptionsAlarmSettings = 118,
        [Access(null, LoginAccessGroups.GENERAL_OPTIONS, AccessConstans.NULL_ACCESS_REFERENCE)]
        GeneralOptionsAlarmSettingsView = 119,
        [Access(null, LoginAccessGroups.GENERAL_OPTIONS, (int)OldGeneralOptionsAdmin)]
        GeneralOptionsAlarmSettingsAdmin = 120,
        [AccessPresentation(null, LoginAccessGroups.GENERAL_OPTIONS, (int)GeneralOptionsUiSettingsView, (int)GeneralOptionsUiSettingsAdmin, 8)]
        GeneralOptionsUiSettings = 121,
        [Access(null, LoginAccessGroups.GENERAL_OPTIONS, AccessConstans.NULL_ACCESS_REFERENCE)]
        GeneralOptionsUiSettingsView = 122,
        [Access(null, LoginAccessGroups.GENERAL_OPTIONS, (int)OldGeneralOptionsAdmin)]
        GeneralOptionsUiSettingsAdmin = 123,
        [AccessPresentation(null, LoginAccessGroups.GENERAL_OPTIONS, (int)GeneralOptionsEventlogsView, (int)GeneralOptionsEventlogsAdmin, 9)]
        GeneralOptionsEventlogs = 124,
        [Access(null, LoginAccessGroups.GENERAL_OPTIONS, AccessConstans.NULL_ACCESS_REFERENCE)]
        GeneralOptionsEventlogsView = 125,
        [Access(null, LoginAccessGroups.GENERAL_OPTIONS, (int)OldGeneralOptionsAdmin)]
        GeneralOptionsEventlogsAdmin = 126,
        [AccessPresentation(null, LoginAccessGroups.GENERAL_OPTIONS, (int)GeneralOptionsLanguageSettingsView, (int)GeneralOptionsLanguageSettingsAdmin, 10)]
        GeneralOptionsLanguageSettings = 127,
        [Access(null, LoginAccessGroups.GENERAL_OPTIONS, AccessConstans.NULL_ACCESS_REFERENCE)]
        GeneralOptionsLanguageSettingsView = 128,
        [Access(null, LoginAccessGroups.GENERAL_OPTIONS, (int)OldGeneralOptionsAdmin)]
        GeneralOptionsLanguageSettingsAdmin = 129,
        [AccessPresentation(null, LoginAccessGroups.GENERAL_OPTIONS, (int)GeneralOptionsAdvancedAccessSettingsView, (int)GeneralOptionsAdvancedAccessSettingsAdmin, 11)]
        GeneralOptionsAdvancedAccessSettings = 130,
        [Access(null, LoginAccessGroups.GENERAL_OPTIONS, AccessConstans.NULL_ACCESS_REFERENCE)]
        GeneralOptionsAdvancedAccessSettingsView = 131,
        [Access(null, LoginAccessGroups.GENERAL_OPTIONS, (int)OldGeneralOptionsAdmin)]
        GeneralOptionsAdvancedAccessSettingsAdmin = 132,
        [AccessPresentation(null, LoginAccessGroups.GENERAL_OPTIONS, (int)GeneralOptionsAdvancedSettingsView, (int)GeneralOptionsAdvancedSettingsAdmin, 12)]
        GeneralOptionsAdvancedSettings = 133,
        [Access(null, LoginAccessGroups.GENERAL_OPTIONS, AccessConstans.NULL_ACCESS_REFERENCE)]
        GeneralOptionsAdvancedSettingsView = 134,
        [Access(null, LoginAccessGroups.GENERAL_OPTIONS, (int)OldGeneralOptionsAdmin)]
        GeneralOptionsAdvancedSettingsAdmin = 135,

        // Old local sound
        [Access(null, null, AccessConstans.NULL_ACCESS_REFERENCE)]
        OldLocalSoundsAdmin = 17,

        // Local sounds
        [AccessPresentation(null, LoginAccessGroups.LOCAL_SOUNDS, (int)LocalSoundsView, (int)LocalSoundsAdmin, 1)]
        LocalSounds = 200,
        [Access(null, LoginAccessGroups.LOCAL_SOUNDS, AccessConstans.NULL_ACCESS_REFERENCE)]
        LocalSoundsView = 201,
        [Access(null, LoginAccessGroups.LOCAL_SOUNDS, (int)OldLocalSoundsAdmin)]
        LocalSoundsAdmin = 202,

        // Search
        [AccessPresentation(null, LoginAccessGroups.SEARCH, (int)SearchView, AccessConstans.NULL_ACCESS_REFERENCE, 1)]
        Search = 300,
        [Access(null, LoginAccessGroups.SEARCH, AccessConstans.NULL_ACCESS_REFERENCE)]
        SearchView = 301,

        // Folders structure
        [AccessPresentation(null, LoginAccessGroups.FOLDERS_SRUCTURE, (int)FoldersStructureView, (int)FoldersStructureAdmin, 1)]
        FoldersStructure = 400,
        [Access(null, LoginAccessGroups.FOLDERS_SRUCTURE, AccessConstans.NULL_ACCESS_REFERENCE)]
        FoldersStructureView = 401,
        [Access(null, LoginAccessGroups.FOLDERS_SRUCTURE, AccessConstans.NULL_ACCESS_REFERENCE)]
        FoldersStructureAdmin = 402,

        // Old presons
        [Access(null, null, AccessConstans.NULL_ACCESS_REFERENCE)]
        OldPersonsView = 4,
        [Access(null, null, AccessConstans.NULL_ACCESS_REFERENCE)]
        OldPersonsAdmin = 5,

        // Persons
        [AccessPresentation(null, LoginAccessGroups.PERSONS, AccessConstans.NULL_ACCESS_REFERENCE, null, (int)PersonsInsertDeletePerform, AccessConstans.PERFORM_DESCRIPTION, 1)]
        PersonsInsertDelete = 500,
        [Access(null, LoginAccessGroups.PERSONS, (int)OldPersonsAdmin, new int[] { (int)PersonsInformationAdmin }, null)]
        PersonsInsertDeletePerform = 501,
        [AccessPresentation(null, LoginAccessGroups.PERSONS, AccessConstans.NULL_ACCESS_REFERENCE, null, (int)PersonsCsvImportPerform, AccessConstans.PERFORM_DESCRIPTION, 2)]
        PersonsCsvImport = 502,
        [Access(null, LoginAccessGroups.PERSONS, (int)OldPersonsAdmin)]
        PersonsCsvImportPerform = 503,
        [AccessPresentation(null, LoginAccessGroups.PERSONS, (int)PersonsInformationView, (int)PersonsInformationAdmin, 3)]
        PersonsInformation = 504,
        [Access(null, LoginAccessGroups.PERSONS, (int)OldPersonsView, null, new int[] { (int)PersonsPersonalCodeView, (int)PersonsPersonalCodeAdmin })]
        PersonsInformationView = 505,
        [Access(null, LoginAccessGroups.PERSONS, (int)OldPersonsAdmin, null, new int[] { (int)PersonsInsertDeletePerform })]
        PersonsInformationAdmin = 506,
        [AccessPresentation(null, LoginAccessGroups.PERSONS, (int)PersonsPersonalCodeView, (int)PersonsPersonalCodeAdmin, 4)]
        PersonsPersonalCode = 526,
        [Access(null, LoginAccessGroups.PERSONS, AccessConstans.NULL_ACCESS_REFERENCE, new int[] { (int)PersonsInformationView }, null)]
        PersonsPersonalCodeView = 527,
        [Access(null, LoginAccessGroups.PERSONS, AccessConstans.NULL_ACCESS_REFERENCE, new int[] { (int)PersonsInformationView }, null)]
        PersonsPersonalCodeAdmin = 528,
        [AccessPresentation(null, LoginAccessGroups.PERSONS, (int)PersonsLoginsView, (int)PersonsLoginsAdmin, 5)]
        PersonsLogins = 507,
        [Access(null, LoginAccessGroups.PERSONS, (int)OldPersonsView)]
        PersonsLoginsView = 508,
        [Access(null, LoginAccessGroups.PERSONS, (int)OldPersonsAdmin)]
        PersonsLoginsAdmin = 509,
        [AccessPresentation(null, LoginAccessGroups.PERSONS, (int)PersonsCardsView, (int)PersonsCardsAdmin, 6)]
        PersonsCards = 510,
        [Access(null, LoginAccessGroups.PERSONS, (int)OldPersonsView)]
        PersonsCardsView = 511,
        [Access(null, LoginAccessGroups.PERSONS, (int)OldPersonsAdmin)]
        PersonsCardsAdmin = 512,
        [AccessPresentation(null, LoginAccessGroups.PERSONS, (int)PersonsAccessControlListsAccessZonesView, (int)PersonsAccessControlListsAccessZonesAdmin, 7)]
        PersonsAccessControlListsAccessZones = 513,
        [Access(null, LoginAccessGroups.PERSONS, (int)OldPersonsView)]
        PersonsAccessControlListsAccessZonesView = 514,
        [Access(null, LoginAccessGroups.PERSONS, (int)OldPersonsAdmin)]
        PersonsAccessControlListsAccessZonesAdmin = 515,
        [AccessPresentation(null, LoginAccessGroups.PERSONS, (int)PersonsActualAccessListView, AccessConstans.NULL_ACCESS_REFERENCE, 8)]
        PersonsActualAccessList = 516,
        [Access(null, LoginAccessGroups.PERSONS, (int)OldPersonsView)]
        PersonsActualAccessListView = 517,
        [AccessPresentation(null, LoginAccessGroups.PERSONS, (int)PersonsReferencedByView, AccessConstans.NULL_ACCESS_REFERENCE, 9)]
        PersonsReferencedBy = 518,
        [Access(null, LoginAccessGroups.PERSONS, (int)OldPersonsView)]
        PersonsReferencedByView = 519,
        [AccessPresentation(null, LoginAccessGroups.PERSONS, (int)PersonsDescriptionView, (int)PersonsDescriptionAdmin, 10)]
        PersonsDescription = 520,
        [Access(null, LoginAccessGroups.PERSONS, (int)OldPersonsView)]
        PersonsDescriptionView = 521,
        [Access(null, LoginAccessGroups.PERSONS, (int)OldPersonsAdmin)]
        PersonsDescriptionAdmin = 522,
        [AccessPresentation(null, LoginAccessGroups.PERSONS, (int)PersonsLocalAlarmInstructionView, (int)PersonsLocalAlarmInstructionAdmin, 11)]
        PersonsLocalAlarmInstruction = 523,
        [Access(null, LoginAccessGroups.PERSONS, (int)OldPersonsView)]
        PersonsLocalAlarmInstructionView = 524,
        [Access(null, LoginAccessGroups.PERSONS, (int)OldPersonsAdmin)]
        PersonsLocalAlarmInstructionAdmin = 525,

        // Old logins
        [Access(null, null, AccessConstans.NULL_ACCESS_REFERENCE)]
        OldLoginsView = 1,
        [Access(null, null, AccessConstans.NULL_ACCESS_REFERENCE)]
        OldLoginsAdmin = 2,

        // Logins / Login groups
        [AccessPresentation(null, LoginAccessGroups.LOGINS_LOGIN_GROUPS, (int)LoginsLoginGroupsView, (int)LoginsLoginGroupsAdmin, 1)]
        LoginsLoginGroups = 600,
        [Access(null, LoginAccessGroups.LOGINS_LOGIN_GROUPS, (int)OldLoginsView)]
        LoginsLoginGroupsView = 601,
        [Access(null, LoginAccessGroups.LOGINS_LOGIN_GROUPS, (int)OldLoginsAdmin)]
        LoginsLoginGroupsAdmin = 602,
       
        // Old eventlogs
        [Access(null, null, AccessConstans.NULL_ACCESS_REFERENCE)]
        OldEventlogsAdmin = 6,

        // Eventlogs
        [AccessPresentation(null, LoginAccessGroups.EVENTLOGS, (int)EventlogsView, null, (int)EventlogsPerform, AccessConstans.PERFORM_DESCRIPTION, 1)]
        Eventlogs = 700,
        [Access(null, LoginAccessGroups.EVENTLOGS, AccessConstans.NULL_ACCESS_REFERENCE)]
        EventlogsView = 701,
        [Access(null, LoginAccessGroups.EVENTLOGS, (int)OldEventlogsAdmin)]
        EventlogsPerform = 702,

        // Old system events
        [Access(null, null, AccessConstans.NULL_ACCESS_REFERENCE)]
        OldSystemEventsView = 7,
        [Access(null, null, AccessConstans.NULL_ACCESS_REFERENCE)]
        OldSystemEventsAdmin = 8,

        // System events
        [AccessPresentation(null, LoginAccessGroups.SYSTEM_EVENTS, (int)SystemEventsView, (int)SystemEventsAdmin, 1)]
        SystemEvents = 800,
        [Access(null, LoginAccessGroups.SYSTEM_EVENTS, (int)OldSystemEventsView)]
        SystemEventsView = 801,
        [Access(null, LoginAccessGroups.SYSTEM_EVENTS, (int)OldSystemEventsAdmin)]
        SystemEventsAdmin = 802,

        // Old alarms management
        [Access(null, null, AccessConstans.NULL_ACCESS_REFERENCE)]
        OldAlarmsManagementAdmin = 16,

        // Alarms
        [AccessPresentation(null, LoginAccessGroups.ALARMS, (int)AlarmsAlarmsView, (int)AlarmsAlarmsAdmin, 1)]
        AlarmsAlarms = 900,
        [Access(null, LoginAccessGroups.ALARMS, AccessConstans.NULL_ACCESS_REFERENCE, null, new int[] { (int)AlarmsBlockedAlarmsAdmin })]
        AlarmsAlarmsView = 901,
        [Access(null, LoginAccessGroups.ALARMS, (int)OldAlarmsManagementAdmin)]
        AlarmsAlarmsAdmin = 902,
        [AccessPresentation(null, LoginAccessGroups.ALARMS, (int)AlarmsBlockedAlarmsView, (int)AlarmsBlockedAlarmsAdmin, 2)]
        AlarmsBlockedAlarms = 903,
        [Access(null, LoginAccessGroups.ALARMS, AccessConstans.NULL_ACCESS_REFERENCE)]
        AlarmsBlockedAlarmsView = 904,
        [Access(null, LoginAccessGroups.ALARMS, (int)OldAlarmsManagementAdmin, new int[] { (int)AlarmsAlarmsView }, null)]
        AlarmsBlockedAlarmsAdmin = 905,

        // Global alarm instructions
        [AccessPresentation(null, LoginAccessGroups.GLOBAL_ALARM_INSTRUCTIONS, AccessConstans.NULL_ACCESS_REFERENCE, null, (int)GlobalAlarmInstructionsInsertDeletePerform, AccessConstans.PERFORM_DESCRIPTION, 1)]
        GlobalAlarmInstructionsInsertDelete = 1000,
        [Access(null, LoginAccessGroups.GLOBAL_ALARM_INSTRUCTIONS, (int)OldAlarmsManagementAdmin, new int[] { (int)GlobalAlarmInstructionsAlarmInstructionsAdmin }, null)]
        GlobalAlarmInstructionsInsertDeletePerform = 1001,
        [AccessPresentation(null, LoginAccessGroups.GLOBAL_ALARM_INSTRUCTIONS, (int)GlobalAlarmInstructionsAlarmInstructionsView, (int)GlobalAlarmInstructionsAlarmInstructionsAdmin, 2)]
        GlobalAlarmInstructionsAlarmInstructions = 1002,
        [Access(null, LoginAccessGroups.GLOBAL_ALARM_INSTRUCTIONS, AccessConstans.NULL_ACCESS_REFERENCE)]
        GlobalAlarmInstructionsAlarmInstructionsView = 1003,
        [Access(null, LoginAccessGroups.GLOBAL_ALARM_INSTRUCTIONS, (int)OldAlarmsManagementAdmin, null, new int[] { (int)GlobalAlarmInstructionsInsertDeletePerform })]
        GlobalAlarmInstructionsAlarmInstructionsAdmin = 1004,
        [AccessPresentation(null, LoginAccessGroups.GLOBAL_ALARM_INSTRUCTIONS, (int)GlobalAlarmInstructionsReferencedByView, AccessConstans.NULL_ACCESS_REFERENCE, 3)]
        GlobalAlarmInstructionsReferencedBy = 1005,
        [Access(null, LoginAccessGroups.GLOBAL_ALARM_INSTRUCTIONS, (int)OldAlarmsManagementAdmin)]
        GlobalAlarmInstructionsReferencedByView = 1006,

        // Old time management
        [Access(null, null, AccessConstans.NULL_ACCESS_REFERENCE)]
        OldTimeManagementView = 9,
        [Access(null, null, AccessConstans.NULL_ACCESS_REFERENCE)]
        OldTimeManagementAdmin = 10,

        // Daily plans / time zones
        [AccessPresentation(null, LoginAccessGroups.DAILY_PLANS_TIME_ZONES, AccessConstans.NULL_ACCESS_REFERENCE, null, (int)DailyPlansTimeZonesInsertDeletePerform, AccessConstans.PERFORM_DESCRIPTION, 1)]
        DailyPlansTimeZonesInsertDelete = 1100,
        [Access(null, LoginAccessGroups.DAILY_PLANS_TIME_ZONES, (int)OldTimeManagementAdmin, new int[] { (int)DailyPlansTimeZonesScheduleAdmin, (int)DailyPlansTimeZonesDateSettingsAdmin }, null)]
        DailyPlansTimeZonesInsertDeletePerform = 1101,
        [AccessPresentation(null, LoginAccessGroups.DAILY_PLANS_TIME_ZONES, (int)DailyPlansTimeZonesScheduleView, (int)DailyPlansTimeZonesScheduleAdmin, 2)]
        DailyPlansTimeZonesSchedule = 1102,
        [Access(null, LoginAccessGroups.DAILY_PLANS_TIME_ZONES, (int)OldTimeManagementView)]
        DailyPlansTimeZonesScheduleView = 1103,
        [Access(null, LoginAccessGroups.DAILY_PLANS_TIME_ZONES, (int)OldTimeManagementAdmin, null, new int[] { (int)DailyPlansTimeZonesInsertDeletePerform })]
        DailyPlansTimeZonesScheduleAdmin = 1104,
        [AccessPresentation(null, LoginAccessGroups.DAILY_PLANS_TIME_ZONES, (int)DailyPlansTimeZonesDateSettings, (int)DailyPlansTimeZonesDateSettingsAdmin, 3)]
        DailyPlansTimeZonesDateSettings = 1105,
        [Access(null, LoginAccessGroups.DAILY_PLANS_TIME_ZONES, (int)OldTimeManagementView)]
        DailyPlansTimeZonesDateSettingsView = 1106,
        [Access(null, LoginAccessGroups.DAILY_PLANS_TIME_ZONES, (int)OldTimeManagementAdmin, null, new int[] { (int)DailyPlansTimeZonesInsertDeletePerform })]
        DailyPlansTimeZonesDateSettingsAdmin = 1107,
        [AccessPresentation(null, LoginAccessGroups.DAILY_PLANS_TIME_ZONES, (int)DailyPlansTimeZonesReferencedByView, AccessConstans.NULL_ACCESS_REFERENCE, 4)]
        DailyPlansTimeZonesReferencedBy = 1108,
        [Access(null, LoginAccessGroups.DAILY_PLANS_TIME_ZONES, (int)OldTimeManagementView)]
        DailyPlansTimeZonesReferencedByView = 1109,
        [AccessPresentation(null, LoginAccessGroups.DAILY_PLANS_TIME_ZONES, (int)DailyPlansTimeZonesDescriptionView, (int)DailyPlansTimeZonesDescriptionAdmin, 5)]
        DailyPlansTimeZonesDescription = 1110,
        [Access(null, LoginAccessGroups.DAILY_PLANS_TIME_ZONES, (int)OldTimeManagementView)]
        DailyPlansTimeZonesDescriptionView = 1111,
        [Access(null, LoginAccessGroups.DAILY_PLANS_TIME_ZONES, (int)OldTimeManagementAdmin)]
        DailyPlansTimeZonesDescriptionAdmin = 1112,
        
        // Calendars / day types
        [AccessPresentation(null, LoginAccessGroups.CALENDARS_DAY_TYPES, AccessConstans.NULL_ACCESS_REFERENCE, null, (int)CalendarsDayTypesInsertDeletePerform, AccessConstans.PERFORM_DESCRIPTION, 1)]
        CalendarsDayTypesInsertDelete = 1200,
        [Access(null, LoginAccessGroups.CALENDARS_DAY_TYPES, (int)OldTimeManagementAdmin, new int[] { (int)CalendarsDayTypesCalendarSettingsAdmin }, null)]
        CalendarsDayTypesInsertDeletePerform = 1201,
        [AccessPresentation(null, LoginAccessGroups.CALENDARS_DAY_TYPES, (int)CalendarsDayTypesCalendarSettingsView, (int)CalendarsDayTypesCalendarSettingsAdmin, 2)]
        CalendarsDayTypesCalendarSettings = 1202,
        [Access(null, LoginAccessGroups.CALENDARS_DAY_TYPES, (int)OldTimeManagementView)]
        CalendarsDayTypesCalendarSettingsView = 1203,
        [Access(null, LoginAccessGroups.CALENDARS_DAY_TYPES, (int)OldTimeManagementAdmin, null, new int[] { (int)CalendarsDayTypesInsertDeletePerform })]
        CalendarsDayTypesCalendarSettingsAdmin = 1204,
        [AccessPresentation(null, LoginAccessGroups.CALENDARS_DAY_TYPES, (int)CalendarsDayTypesCalendarStatusView, (int)CalendarsDayTypesCalendarStatusAdmin, 3)]
        CalendarsDayTypesCalendarStatus = 1205,
        [Access(null, LoginAccessGroups.CALENDARS_DAY_TYPES, (int)OldTimeManagementView)]
        CalendarsDayTypesCalendarStatusView = 1206,
        [Access(null, LoginAccessGroups.CALENDARS_DAY_TYPES, (int)OldTimeManagementAdmin)]
        CalendarsDayTypesCalendarStatusAdmin = 1207,
        [AccessPresentation(null, LoginAccessGroups.CALENDARS_DAY_TYPES, (int)CalendarsDayTypesReferencedByView, AccessConstans.NULL_ACCESS_REFERENCE, 4)]
        CalendarsDayTypesReferencedBy = 1208,
        [Access(null, LoginAccessGroups.CALENDARS_DAY_TYPES, (int)OldTimeManagementView)]
        CalendarsDayTypesReferencedByView = 1209,
        [AccessPresentation(null, LoginAccessGroups.CALENDARS_DAY_TYPES, (int)CalendarsDayTypesDescriptionView, (int)CalendarsDayTypesDescriptionAdmin, 5)]
        CalendarsDayTypesDescription = 12010,
        [Access(null, LoginAccessGroups.CALENDARS_DAY_TYPES, (int)OldTimeManagementView)]
        CalendarsDayTypesDescriptionView = 12011,
        [Access(null, LoginAccessGroups.CALENDARS_DAY_TYPES, (int)OldTimeManagementAdmin)]
        CalendarsDayTypesDescriptionAdmin = 1212,

        // Old cards management
        [Access(null, null, AccessConstans.NULL_ACCESS_REFERENCE)]
        OldCardsManagementView = 11,
        [Access(null, null, AccessConstans.NULL_ACCESS_REFERENCE)]
        OldCardsManagementAdmin = 12,

        // Cards
        [AccessPresentation(null, LoginAccessGroups.CARDS, AccessConstans.NULL_ACCESS_REFERENCE, null, (int)CardsInsertDeletePerform, AccessConstans.PERFORM_DESCRIPTION, 1)]
        CardsInsertDelete = 1300,
        [Access(null, LoginAccessGroups.CARDS, (int)OldCardsManagementAdmin, new int[] { (int)CardsPropertiesAdmin }, null)]
        CardsInsertDeletePerform = 1301,
        [AccessPresentation(null, LoginAccessGroups.CARDS, (int)CardsPropertiesView, (int)CardsPropertiesAdmin, 2)]
        CardsProperties = 1302,
        [Access(null, LoginAccessGroups.CARDS, (int)OldCardsManagementView)]
        CardsPropertiesView = 1303,
        [Access(null, LoginAccessGroups.CARDS, (int)OldCardsManagementAdmin, null, new int[] { (int)CardsInsertDeletePerform, (int)CardsPropertiesPinAdmin, (int)CardsPropertiesCardStateAdmin })]
        CardsPropertiesAdmin = 1304,
        [AccessPresentation(null, LoginAccessGroups.CARDS, AccessConstans.NULL_ACCESS_REFERENCE, (int)CardsPropertiesPinAdmin, 3)]
        CardsPropertiesPin = 1305,
        [Access(null, LoginAccessGroups.CARDS, (int)OldCardsManagementAdmin, new int[] { (int)CardsPropertiesAdmin }, null)]
        CardsPropertiesPinAdmin = 1306,
        [AccessPresentation(null, LoginAccessGroups.CARDS, AccessConstans.NULL_ACCESS_REFERENCE, (int)CardsPropertiesCardStateAdmin, 4)]
        CardsPropertiesCardState = 1307,
        [Access(null, LoginAccessGroups.CARDS, (int)OldCardsManagementAdmin, new int[] { (int)CardsPropertiesAdmin }, null)]
        CardsPropertiesCardStateAdmin = 1308,
        [AccessPresentation(null, LoginAccessGroups.CARDS, (int)CardsReferencedByView, AccessConstans.NULL_ACCESS_REFERENCE, 5)]
        CardsReferencedBy = 1309,
        [Access(null, LoginAccessGroups.CARDS, (int)OldCardsManagementView)]
        CardsReferencedByView = 1310,
        [AccessPresentation(null, LoginAccessGroups.CARDS, (int)CardsDescriptionView, (int)CardsDescriptionAdmin, 6)]
        CardsDescription = 1311,
        [Access(null, LoginAccessGroups.CARDS, (int)OldCardsManagementView)]
        CardsDescriptionView = 1312,
        [Access(null, LoginAccessGroups.CARDS, (int)OldCardsManagementAdmin)]
        CardsDescriptionAdmin = 1313,
        [AccessPresentation(null, LoginAccessGroups.CARDS, (int)CardsLocalAlarmInstructionView, (int)CardsLocalAlarmInstructionAdmin, 7)]
        CardsLocalAlarmInstruction = 1314,
        [Access(null, LoginAccessGroups.CARDS, (int)OldCardsManagementView)]
        CardsLocalAlarmInstructionView = 1315,
        [Access(null, LoginAccessGroups.CARDS, (int)OldCardsManagementAdmin)]
        CardsLocalAlarmInstructionAdmin = 1316,

        // Old ID management
        [Access(null, null, AccessConstans.NULL_ACCESS_REFERENCE)]
        OldIdManagementPerform = 18,
        [Access(null, null, AccessConstans.NULL_ACCESS_REFERENCE)]
        OldIdManagementAdmin = 19,

        // ID management
        [AccessPresentation(null, LoginAccessGroups.ID_MANAGEMENT, (int)IdManagementCardTemplatesView, (int)IdManagementCardTemplatesAdmin, 1)]
        IdManagementCardTemplates = 1400,
        [Access(null, LoginAccessGroups.ID_MANAGEMENT, (int)OldIdManagementPerform)]
        IdManagementCardTemplatesView = 1401,
        [Access(null, LoginAccessGroups.ID_MANAGEMENT, (int)OldIdManagementAdmin)]
        IdManagementCardTemplatesAdmin = 1402,
        [AccessPresentation(null, LoginAccessGroups.ID_MANAGEMENT, (int)IdManagementCardPrintPerform, AccessConstans.PERFORM_DESCRIPTION, (int)IdManagementCardPrintAdmin, null, 2)]
        IdManagementCardPrint = 1403,
        [Access(null, LoginAccessGroups.ID_MANAGEMENT, (int)OldIdManagementPerform)]
        IdManagementCardPrintPerform = 1404,
        [Access(null, LoginAccessGroups.ID_MANAGEMENT, (int)OldIdManagementAdmin)]
        IdManagementCardPrintAdmin = 1405,

        // Card systems
        [AccessPresentation(null, LoginAccessGroups.CARD_SYSTEMS, AccessConstans.NULL_ACCESS_REFERENCE, null, (int)CardSystemsInsertDeletePerform, AccessConstans.PERFORM_DESCRIPTION, 1)]
        CardSystemsInsertDelete = 1500,
        [Access(null, LoginAccessGroups.CARD_SYSTEMS, (int)OldCardsManagementAdmin, new int[] { (int)CardSystemsPropertiesAdmin }, null)]
        CardSystemsInsertDeletePerform = 1501,
        [AccessPresentation(null, LoginAccessGroups.CARD_SYSTEMS, (int)CardSystemsPropertiesView, (int)CardSystemsPropertiesAdmin, 2)]
        CardSystemsProperties = 1502,
        [Access(null, LoginAccessGroups.CARD_SYSTEMS, (int)OldCardsManagementView)]
        CardSystemsPropertiesView = 1503,
        [Access(null, LoginAccessGroups.CARD_SYSTEMS, (int)OldCardsManagementAdmin, null, new int[] { (int)CardSystemsInsertDeletePerform })]
        CardSystemsPropertiesAdmin = 1504,
        [AccessPresentation(null, LoginAccessGroups.CARD_SYSTEMS, (int)CardSystemsSmartCardDataView, (int)CardSystemsSmartCardDataAdmin, 3)]
        CardSystemsSmartCardData = 1505,
        [Access(null, LoginAccessGroups.CARD_SYSTEMS, (int)OldCardsManagementView)]
        CardSystemsSmartCardDataView = 1506,
        [Access(null, LoginAccessGroups.CARD_SYSTEMS, (int)OldCardsManagementAdmin)]
        CardSystemsSmartCardDataAdmin = 1507,
        [AccessPresentation(null, LoginAccessGroups.CARD_SYSTEMS, (int)CardSystemsCardSerieView, (int)CardSystemsCardSerieAdmin, 4)]
        CardSystemsCardSerie = 1508,
        [Access(null, LoginAccessGroups.CARD_SYSTEMS, (int)OldCardsManagementView)]
        CardSystemsCardSerieView = 1509,
        [Access(null, LoginAccessGroups.CARD_SYSTEMS, (int)OldCardsManagementAdmin)]
        CardSystemsCardSerieAdmin = 1510,
        [AccessPresentation(null, LoginAccessGroups.CARD_SYSTEMS, (int)CardSystemsReferencedByView, AccessConstans.NULL_ACCESS_REFERENCE, 5)]
        CardSystemsReferencedBy = 1511,
        [Access(null, LoginAccessGroups.CARD_SYSTEMS, (int)OldCardsManagementView)]
        CardSystemsReferencedByView = 1512,
        [AccessPresentation(null, LoginAccessGroups.CARD_SYSTEMS, (int)CardSystemsDescriptionView, (int)CardSystemsDescriptionAdmin, 6)]
        CardSystemsDescription = 1513,
        [Access(null, LoginAccessGroups.CARD_SYSTEMS, (int)OldCardsManagementView)]
        CardSystemsDescriptionView = 1514,
        [Access(null, LoginAccessGroups.CARD_SYSTEMS, (int)OldCardsManagementAdmin)]
        CardSystemsDescriptionAdmin = 1515,
        [AccessPresentation(null, LoginAccessGroups.CARD_SYSTEMS, (int)CardSystemsLocalAlarmInstructionView, (int)CardSystemsLocalAlarmInstructionAdmin, 7)]
        CardSystemsLocalAlarmInstruction = 1516,
        [Access(null, LoginAccessGroups.CARD_SYSTEMS, (int)OldCardsManagementView)]
        CardSystemsLocalAlarmInstructionView = 1517,
        [Access(null, LoginAccessGroups.CARD_SYSTEMS, (int)OldCardsManagementAdmin)]
        CardSystemsLocalAlarmInstructionAdmin = 1518,

        // Old presentations management
        [Access(null, null, AccessConstans.NULL_ACCESS_REFERENCE)]
        OldPresentationsManagementView = 13,
        [Access(null, null, AccessConstans.NULL_ACCESS_REFERENCE)]
        OldPresentationsManagementAdmin = 14,

        // Presentation groups / presentation formatters
        [AccessPresentation(null, LoginAccessGroups.PRESENT_GROUPS_FORMATTERS, AccessConstans.NULL_ACCESS_REFERENCE, null, (int)PresentGroupsFormattersInsertDeletePerform, AccessConstans.PERFORM_DESCRIPTION, 1)]
        PresentGroupsFormattersInsertDelete = 1600,
        [Access(null, LoginAccessGroups.PRESENT_GROUPS_FORMATTERS, (int)OldPresentationsManagementAdmin, new int[] { (int)PresentGroupsFormattersAlarmSettingsAdmin }, null)]
        PresentGroupsFormattersInsertDeletePerform = 1601,
        [AccessPresentation(null, LoginAccessGroups.PRESENT_GROUPS_FORMATTERS, (int)PresentGroupsFormattersAlarmSettingsView, (int)PresentGroupsFormattersAlarmSettingsAdmin, 2)]
        PresentGroupsFormattersAlarmSettings = 1602,
        [Access(null, LoginAccessGroups.PRESENT_GROUPS_FORMATTERS, (int)OldPresentationsManagementView)]
        PresentGroupsFormattersAlarmSettingsView = 1603,
        [Access(null, LoginAccessGroups.PRESENT_GROUPS_FORMATTERS, (int)OldPresentationsManagementAdmin, null, new int[] { (int)PresentGroupsFormattersInsertDeletePerform })]
        PresentGroupsFormattersAlarmSettingsAdmin = 1604,
        [AccessPresentation(null, LoginAccessGroups.PRESENT_GROUPS_FORMATTERS, (int)PresentGroupsFormattersEmailView, (int)PresentGroupsFormattersEmailAdmin, 3)]
        PresentGroupsFormattersEmail = 1605,
        [Access(null, LoginAccessGroups.PRESENT_GROUPS_FORMATTERS, (int)OldPresentationsManagementView)]
        PresentGroupsFormattersEmailView = 1606,
        [Access(null, LoginAccessGroups.PRESENT_GROUPS_FORMATTERS, (int)OldPresentationsManagementAdmin)]
        PresentGroupsFormattersEmailAdmin = 1607,
        [AccessPresentation(null, LoginAccessGroups.PRESENT_GROUPS_FORMATTERS, (int)PresentGroupsFormattersCisNgView, (int)PresentGroupsFormattersCisNgAdmin, 4)]
        PresentGroupsFormattersCisNg = 1608,
        [Access(null, LoginAccessGroups.PRESENT_GROUPS_FORMATTERS, (int)OldPresentationsManagementView)]
        PresentGroupsFormattersCisNgView = 1609,
        [Access(null, LoginAccessGroups.PRESENT_GROUPS_FORMATTERS, (int)OldPresentationsManagementAdmin)]
        PresentGroupsFormattersCisNgAdmin = 16010,
        [AccessPresentation(null, LoginAccessGroups.PRESENT_GROUPS_FORMATTERS, (int)PresentGroupsFormattersReferencedByView, AccessConstans.NULL_ACCESS_REFERENCE, 5)]
        PresentGroupsFormattersReferencedBy = 16011,
        [Access(null, LoginAccessGroups.PRESENT_GROUPS_FORMATTERS, (int)OldPresentationsManagementView)]
        PresentGroupsFormattersReferencedByView = 1612,
        [AccessPresentation(null, LoginAccessGroups.PRESENT_GROUPS_FORMATTERS, (int)PresentGroupsFormattersDescriptionView, (int)PresentGroupsFormattersDescriptionAdmin, 6)]
        PresentGroupsFormattersDescription = 1613,
        [Access(null, LoginAccessGroups.PRESENT_GROUPS_FORMATTERS, (int)OldPresentationsManagementView)]
        PresentGroupsFormattersDescriptionView = 1614,
        [Access(null, LoginAccessGroups.PRESENT_GROUPS_FORMATTERS, (int)OldPresentationsManagementAdmin)]
        PresentGroupsFormattersDescriptionAdmin = 1615,

        // CisNGs / CisNG groups
        [AccessPresentation(null, LoginAccessGroups.CIS_NGS_CIS_NG_GROUPS, AccessConstans.NULL_ACCESS_REFERENCE, null, (int)CisNgsCisNgGroupsInsertDeletePerfrom, AccessConstans.PERFORM_DESCRIPTION, 1)]
        CisNgsCisNgGroupsInsertDelete = 1700,
        [Access(null, LoginAccessGroups.CIS_NGS_CIS_NG_GROUPS, (int)OldPresentationsManagementAdmin, new int[] { (int)CisNgsCisNgGroupsCisNgAdmin, (int)CisNgsCisNgGroupsSettingsAdmin }, null)]
        CisNgsCisNgGroupsInsertDeletePerfrom = 1701,
        [AccessPresentation(null, LoginAccessGroups.CIS_NGS_CIS_NG_GROUPS, (int)CisNgsCisNgGroupsCisNgView, (int)CisNgsCisNgGroupsCisNgAdmin, 2)]
        CisNgsCisNgGroupsCisNg = 1702,
        [Access(null, LoginAccessGroups.CIS_NGS_CIS_NG_GROUPS, (int)OldPresentationsManagementView)]
        CisNgsCisNgGroupsCisNgView = 1703,
        [Access(null, LoginAccessGroups.CIS_NGS_CIS_NG_GROUPS, (int)OldPresentationsManagementAdmin, null, new int[] { (int)CisNgsCisNgGroupsInsertDeletePerfrom })]
        CisNgsCisNgGroupsCisNgAdmin = 1704,
        [AccessPresentation(null, LoginAccessGroups.CIS_NGS_CIS_NG_GROUPS, (int)CisNgsCisNgGroupsSettingsView, (int)CisNgsCisNgGroupsSettingsAdmin, 3)]
        CisNgsCisNgGroupsSettings = 1705,
        [Access(null, LoginAccessGroups.CIS_NGS_CIS_NG_GROUPS, (int)OldPresentationsManagementView)]
        CisNgsCisNgGroupsSettingsView = 1706,
        [Access(null, LoginAccessGroups.CIS_NGS_CIS_NG_GROUPS, (int)OldPresentationsManagementAdmin, null, new int[] { (int)CisNgsCisNgGroupsInsertDeletePerfrom })]
        CisNgsCisNgGroupsSettingsAdmin = 1707,
        [AccessPresentation(null, LoginAccessGroups.CIS_NGS_CIS_NG_GROUPS, (int)CisNgsCisNgGroupsSendMessageView, (int)CisNgsCisNgGroupsSendMessageAdmin, 4)]
        CisNgsCisNgGroupsSendMessage = 1708,
        [Access(null, LoginAccessGroups.CIS_NGS_CIS_NG_GROUPS, (int)OldPresentationsManagementView)]
        CisNgsCisNgGroupsSendMessageView = 1709,
        [Access(null, LoginAccessGroups.CIS_NGS_CIS_NG_GROUPS, (int)OldPresentationsManagementAdmin)]
        CisNgsCisNgGroupsSendMessageAdmin = 1710,
        [AccessPresentation(null, LoginAccessGroups.CIS_NGS_CIS_NG_GROUPS, (int)CisNgsCisNgGroupsReferencedByView, AccessConstans.NULL_ACCESS_REFERENCE, 5)]
        CisNgsCisNgGroupsReferencedBy = 1711,
        [Access(null, LoginAccessGroups.CIS_NGS_CIS_NG_GROUPS, (int)OldPresentationsManagementView)]
        CisNgsCisNgGroupsReferencedByView = 1712,
        [AccessPresentation(null, LoginAccessGroups.CIS_NGS_CIS_NG_GROUPS, (int)CisNgsCisNgGroupsDescriptionView, (int)CisNgsCisNgGroupsDescriptionAdmin, 6)]
        CisNgsCisNgGroupsDescription = 1713,
        [Access(null, LoginAccessGroups.CIS_NGS_CIS_NG_GROUPS, (int)OldPresentationsManagementView)]
        CisNgsCisNgGroupsDescriptionView = 1714,
        [Access(null, LoginAccessGroups.CIS_NGS_CIS_NG_GROUPS, (int)OldPresentationsManagementAdmin)]
        CisNgsCisNgGroupsDescriptionAdmin = 1715,
        [AccessPresentation(null, LoginAccessGroups.CIS_NGS_CIS_NG_GROUPS, (int)CisNgsCisNgGroupsLocalAlarmInstructionView, (int)CisNgsCisNgGroupsLocalAlarmInstructionAdmin, 7)]
        CisNgsCisNgGroupsLocalAlarmInstruction = 1716,
        [Access(null, LoginAccessGroups.CIS_NGS_CIS_NG_GROUPS, (int)OldPresentationsManagementView)]
        CisNgsCisNgGroupsLocalAlarmInstructionView = 1717,
        [Access(null, LoginAccessGroups.CIS_NGS_CIS_NG_GROUPS, (int)OldPresentationsManagementAdmin)]
        CisNgsCisNgGroupsLocalAlarmInstructionAdmin = 1718,

        // Old unlock client aplication
        [Access(null, null, AccessConstans.NULL_ACCESS_REFERENCE)]
        OldUnlockClientAplicationAdmin = 15,

        // Unlock client aplication
        [AccessPresentation(null, LoginAccessGroups.UNLOCK_CLIENT_APLICATION, AccessConstans.NULL_ACCESS_REFERENCE, null, (int)UnlockClientAplicationPerform, AccessConstans.PERFORM_DESCRIPTION, 1)]
        UnlockClientAplication = 1800,
        [Access(null, LoginAccessGroups.UNLOCK_CLIENT_APLICATION, (int)OldUnlockClientAplicationAdmin)]
        UnlockClientAplicationPerform = 1801,

        // Structured sub site
        [AccessPresentation(null, LoginAccessGroups.STRUCTURED_SUB_SITE, (int)StructuredSubSiteView, (int)StructuredSubSiteAdmin, 1)]
        StructuredSubSite = 1900,
        [Access(null, LoginAccessGroups.STRUCTURED_SUB_SITE, AccessConstans.NULL_ACCESS_REFERENCE)]
        StructuredSubSiteView = 1901,
        [Access(null, LoginAccessGroups.STRUCTURED_SUB_SITE, AccessConstans.NULL_ACCESS_REFERENCE)]
        StructuredSubSiteAdmin = 1902,
    }

    public static class BaseAccess
    {
        private const string SOURCECUSTOMACCESSCONTROL = "CustomAccessControl";

        public static Access GetAccess(LoginAccess enumAccess)
        {
            var attribs = typeof(LoginAccess).GetField(enumAccess.ToString()).GetCustomAttributes(
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
            foreach (LoginAccess loginAccess in Enum.GetValues(typeof (LoginAccess)))
            {
                var attribs =
                    typeof (LoginAccess).GetField(loginAccess.ToString())
                        .GetCustomAttributes(typeof (AccessAttribute), false) as AccessAttribute[];

                if (attribs != null && attribs.Length > 0)
                {
                    if (attribs[0].Group == accessGroup)
                        accesses.Add(new Access((int) loginAccess,
                            attribs[0].Source,
                            attribs[0].Group,
                            attribs[0].GeneralAccess));
                }
            }

            return accesses;
        }

        public static Dictionary<string, List<AccessPresentation>> GetAccessList(Dictionary<string, List<AccessPresentation>> pluginsListAccess)
        {
            var accesses = new Dictionary<string, List<AccessPresentation>>();
            foreach (LoginAccess loginAccess in Enum.GetValues(typeof (LoginAccess)))
            {
                var attribs =
                    typeof (LoginAccess).GetField(loginAccess.ToString())
                        .GetCustomAttributes(typeof(AccessPresentationAttribute), false) as AccessPresentationAttribute[];

                if (attribs != null && attribs.Length > 0)
                {
                    int? generalAccessView = null;
                    int[] forcedSetAccessesView = null;
                    int[] forcedUnsetAccessesView = null;
                    if (attribs[0].AccessView != null)
                    {
                        var attribsAccessView =
                            typeof (LoginAccess).GetField(((LoginAccess) attribs[0].AccessView.Value).ToString())
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
                            typeof (LoginAccess).GetField(((LoginAccess) attribs[0].AccessAdmin.Value).ToString())
                                .GetCustomAttributes(typeof (AccessAttribute), false) as AccessAttribute[];

                        if (attribsAccessAdmin != null && attribsAccessAdmin.Length > 0)
                        {
                            generalAccessAdmin = attribsAccessAdmin[0].GeneralAccess;
                            forcedSetAccessesAdmin = attribsAccessAdmin[0].ForcedSetAccesses;
                            forcedUnsetAccessesAdmin = attribsAccessAdmin[0].ForcedUnsetAccesses;
                        }
                    }

                    var accessPresentation = new AccessPresentation(
                        loginAccess.ToString(),
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

            if (pluginsListAccess != null && pluginsListAccess.Count > 0)
            {
                foreach (KeyValuePair<string, List<AccessPresentation>> accessPresentationGroup in pluginsListAccess)
                    accesses.Add(accessPresentationGroup.Key, accessPresentationGroup.Value);
            }

            foreach (List<AccessPresentation> accessPresentations in accesses.Values)
                accessPresentations.Sort(
                    (accessPresentation1, accessPresentation2) =>
                        accessPresentation1.ViewIndex.CompareTo(
                            accessPresentation2.ViewIndex));

            return accesses;
        }
    }

    [Serializable()]
    public class AccessControl : AOrmObject
    {
        public const string COLUMNIDACCESSCONTROL = "IdAccessControl";
        public const string COLUMNLOGIN = "Login";
        public const string COLUMNLOGINGROUP = "LoginGroup";
        public const string COLUMNACCESS = "Access";

        public virtual Guid IdAccessControl { get; set; }
        public virtual Login Login { get; set; }
        public virtual LoginGroup LoginGroup { get; set; }
        public virtual string Source { get; set; }
        public virtual int Access { get; set; }


        public override bool Compare(object obj)
        {
            if (obj == null)
                return false;

            if (obj is AccessControl)
            {
                return (obj as AccessControl).IdAccessControl == IdAccessControl;
            }
            else
            {
                return false;
            }
        }

        public override string GetIdString()
        {
            return IdAccessControl.ToString();
        }

        public override object GetId()
        {
            return IdAccessControl;
        }

        public override IModifyObject CreateModifyObject()
        {
            return null;
        }

        public override ObjectType GetObjectType()
        {
            return ObjectType.AccessControl;
        }
    }
}
