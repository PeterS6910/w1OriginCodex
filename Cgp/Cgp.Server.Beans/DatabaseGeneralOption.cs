using Contal.Cgp.Globals;

namespace Contal.Cgp.Server.Beans
{
    public class DatabaseGeneralOption : AOrmObject
    {
        public virtual int IdDatabaseGeneralOption { get; set; }
        public virtual int? IntValue { get; set; }
        public virtual string StringValue { get; set; }

        public override bool Compare(object obj)
        {
            if (obj == null)
                return false;

            if (obj is DatabaseGeneralOption)
            {
                return (obj as DatabaseGeneralOption).IdDatabaseGeneralOption == IdDatabaseGeneralOption;
            }
            else
            {
                return false;
            }
        }

        public override string GetIdString()
        {
            return IdDatabaseGeneralOption.ToString();
        }

        public override object GetId()
        {
            return IdDatabaseGeneralOption;
        }

        public override IModifyObject CreateModifyObject()
        {
            return null;
        }

        public override Contal.Cgp.Globals.ObjectType GetObjectType()
        {
            return Contal.Cgp.Globals.ObjectType.NotSupport;
        }
    }

    public enum DatabaseGeneralOptionType : int
    {
        SavedValuesFromRegistryToDatabase = 0,
        AutoStartDHCP = 100,
        SmtpServer = 200,
        SmtpPort = 201,
        SmtpSourceEmailAddress = 202,
        SmtpSubject = 203,
        SerialPort = 300,
        SerialPortBaudRate = 301,
        SerialPortDataBits = 302,
        SerialPortParity = 303,
        SerialPortStopBits = 304,
        SerialPortFlowControl = 305,
        SerialPortParityCheck = 306,
        SerialPortCarrierDetect = 307,
        SerialPortPin = 308,
        DatabaseBackupTimeZone = 400,
        DatabaseBackupPath = 401,
        EventlogsExpirationDays = 500,
        EventlogsMaxCountValue = 501,
        EventlogsMaxCountExponent = 502,
        EventlogsExpirationTimeZone = 503,
        EventlogInputStateChanged = 504,
        EventlogOutputStateChanged = 505,
        EventlogAlarmAreaAlarmStateChanged = 506,
        EventlogAlarmAreaActivationStateChanged = 507,
        EventlogCardReaderOnlineStateChanged = 508,
        EventSourcesReverseOrder = 509,
        CgpDragDropColorBackground = 600,
        CgpDragDropColorText = 601,
        CgpReferenceObjectColorBackground = 602,
        CgpReferenceObjectColorText = 603,
        CgpAlarmNotAcknowledgedColorText = 604,
        CgpAlarmNotAcknowledgedColorBackground = 605,
        CgpAlarmColorText = 606,
        CgpAlarmColorBackground = 607,
        CgpNormalNotAcknowledgedColorText = 608,
        CgpNormalNotAcknowledgedColorBackground = 609,
        CgpNormalColorText = 610,
        CgpNormalColorBackground = 611,
        CgpNoAlarmsInQueueColorText = 612,
        CgpNoAlarmsInQueueColorBackground = 613,
        CgpAutoCloseTurnedOn = 700,
        CgpAutoCloseTimeout = 701,
        CgpChangePasswordDay = 702,
        CgpRequirePINCardLogin = 703,
        CgpUniqueNotNullPersonalKey = 704,
        CgpLockClientApplication = 705,
        CcuConfigurationToServerByPassword = 706,
        CgpRequredSecurePin = 707,
        DisableCcuPnPAssigmnet = 708,
        ListOnlyUnassignedCardsInPersonForm = 709,
        DelayToSaveAlarmsFromCardReaders = 710,
        UniqueAKeyCSRestriction = 711,
        CardReadersAllowPINCachingInMenu = 712,
        MinimalCodeLength = 713,
        MaximalCodeLength = 714,
        IsPinConfirmationObligatory = 715,
        AdvancedDeviceSettingsSyncingTimeFromServer = 803,
        AdvancedDeviceSettingsPeriodOfTimeSyncWithoutStratum = 804,
        AdvancedDeviceSettingsPeriodicTimeSyncTolerance = 805,
        AdvancedDeviceSettingsEnableLoggingSDPSTZChanges = 806,
        AdvancedSettingsMaxEventsCountForInsert = 901,
        AdvancedSettingsDelayForSaveEvents = 902,
        AdvancedSettingsClientSessionTimeout = 903,
        AdvancedSettingsCorrectDeserializationFailures = 904,
        AdvancedSettingsDelayForSendingChangesToCcu = 905,
        CustomerCompanyName = 1000,
        CustomerDeliveryAddress = 1001,
        CustomerZipCode = 1002,
        CustomerCityState = 1003,
        CustomerCountry = 1004,
        CustomerPhone = 1005,
        CustomerWebsite = 1006,
        CustomerContactPerson = 1007,
        SupplierCompanyName = 1008,
        SupplierDeliveryAddress = 1010,
        SupplierZipCode = 1012,
        SupplierCityState = 1013,
        SupplierCountry = 1014,
        SupplierPhone = 1015,
        SupplierWebsite = 1016,
        SupplierContactPerson = 1017,
        AlarmListSuspendedRefreshTimeout = 1018,
        AlarmAreaRestrictivePolicyForTimeBuying = 1019,
        EventlogReportsTimezoneGuidString = 1020,
        EventlogReportsEmails = 1021,
        SmtpCredentials = 1022,
        SmtpSsl = 1023
    }
}
