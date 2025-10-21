using System;
using System.Drawing;
using System.IO;
using Cgp.Components;
using Contal.Cgp.RemotingCommon;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Server.DB;
using Contal.IwQuick;
using Contal.IwQuick.Remoting;
using Contal.IwQuick.Sys;
using Contal.IwQuick.Threads;

namespace Contal.Cgp.Server
{
    [Serializable]
    public class ServerGeneralOptionsProvider :
        MarshalByRefObject,
        IServerGenaralOptionsProvider
    {
        private readonly GeneralOptions _generalOptions = GeneralOptions.Singleton;

        public event DVoid2Void DatabaseBackupChanged;
        public event DVoid2Void DatabaseEventlogExpirationChanged;
        public event DVoid2Void ColorSettingsChanged;
        public event DVoid2Void AutoCloseSettingsChanged;

        private static volatile ServerGeneralOptionsProvider _singleton;
        private static readonly object SyncRoot = new object();
        private const string PHOTOS_PATH = @"\Pictures\GeneralOptions\";
        private const string PHOTOS_NAME = "SupplierLogo.jpg";
        private BinaryPhoto _supplierLogo;

        public static ServerGeneralOptionsProvider Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (SyncRoot)
                        if (_singleton == null)
                            _singleton = new ServerGeneralOptionsProvider();

                return _singleton;
            }
        }

        private ServerGeneralOptionsProvider()
        {
        }

        public void SaveToRegistrySmtp(ServerGeneralOptions serverGeneralOptions)
        {
            _generalOptions.SmtpServer =
                serverGeneralOptions.SmtpServer;

            _generalOptions.SmtpPort =
                serverGeneralOptions.SmtpPort;

            _generalOptions.SmtpSourceEmailAddress =
                serverGeneralOptions.SmtpSourceEmailAddress;

            _generalOptions.SmtpSubject =
                serverGeneralOptions.SmtpSubject;

            _generalOptions.SmtpCredentials =
                serverGeneralOptions.SmtpCredentials;

            _generalOptions.SmtpSsl =
                serverGeneralOptions.SmtpSsl;

            _generalOptions.SaveSMTP();

            CgpServerRemotingProvider.Singleton.ForeachCallbackHandler(
                RunRemoteServicesSettingsChanged,
                DelegateSequenceBlockingMode.Asynchronous,
                false);
        }

        public void SaveToRegistrySerialPort(ServerGeneralOptions serverGeneralOptions)
        {
            _generalOptions.SerialPort =
                serverGeneralOptions.SerialPort;

            _generalOptions.SerialPortBaudRate =
                serverGeneralOptions.SerialPortBaudRate;

            _generalOptions.SerialPortDataBits =
                serverGeneralOptions.SerialPortDataBits;

            _generalOptions.SerialPortParity =
                serverGeneralOptions.SerialPortParity;

            _generalOptions.SerialPortStopBits =
                serverGeneralOptions.SerialPortStopBits;

            _generalOptions.SerialPortFlowControl =
                serverGeneralOptions.SerialPortFlowControl;

            _generalOptions.SerialPortParityCheck =
                serverGeneralOptions.SerialPortParityCheck;

            _generalOptions.SerialPortCarrierDetect =
                serverGeneralOptions.SerialPortCarrierDetect;

            _generalOptions.SerialPortPin =
                serverGeneralOptions.SerialPortPin;

            _generalOptions.SaveSerialPort();

            CgpServerRemotingProvider.Singleton.ForeachCallbackHandler(
                RunSerialPortSettingsChanged,
                DelegateSequenceBlockingMode.Asynchronous,
                false);
        }

        public void SaveToRegistryDatabaseBackup(ServerGeneralOptions serverGeneralOptions)
        {
            _generalOptions.DatabaseBackupPath =
                serverGeneralOptions.DatabaseBackupPath;

            _generalOptions.TimeZoneGuidString =
                serverGeneralOptions.TimeZoneGuidString;

            _generalOptions.SaveDatabaseBackup();

            if (DatabaseBackupChanged != null)
                try
                {
                    DatabaseBackupChanged();
                }
                catch
                {
                }

            CgpServerRemotingProvider.Singleton.ForeachCallbackHandler(
                RunDatabaseBackupSettingsChanged,
                DelegateSequenceBlockingMode.Asynchronous,
                false);
        }

        public void SaveToDatabaseCustomerAndSupplierInfo(ServerGeneralOptions serverGeneralOptions)
        {
            _generalOptions.CustomerCompanyName = serverGeneralOptions.CustomerCompanyName;
            _generalOptions.CustomerContactPerson = serverGeneralOptions.CustomerContactPerson;
            _generalOptions.CustomerCityState = serverGeneralOptions.CustomerCityState;
            _generalOptions.CustomerCountry = serverGeneralOptions.CustomerCountry;
            _generalOptions.CustomerDeliveryAddress = serverGeneralOptions.CustomerDeliveryAddress;
            _generalOptions.CustomerPhone = serverGeneralOptions.CustomerPhone;
            _generalOptions.CustomerWebsite = serverGeneralOptions.CustomerWebsite;
            _generalOptions.CustomerZipCode = serverGeneralOptions.CustomerZipCode;

            _generalOptions.SupplierCompanyName = serverGeneralOptions.SupplierCompanyName;
            _generalOptions.SupplierContactPerson = serverGeneralOptions.SupplierContactPerson;
            _generalOptions.SupplierCityState = serverGeneralOptions.SupplierCityState;
            _generalOptions.SupplierCountry = serverGeneralOptions.SupplierCountry;
            _generalOptions.SupplierDeliveryAddress = serverGeneralOptions.SupplierDeliveryAddress;
            _generalOptions.SupplierPhone = serverGeneralOptions.SupplierPhone;
            _generalOptions.SupplierWebsite = serverGeneralOptions.SupplierWebsite;
            _generalOptions.SupplierZipCode = serverGeneralOptions.SupplierZipCode;

            _generalOptions.SaveDatabaseCustomerAndSupplierInfo();

            CgpServerRemotingProvider.Singleton.ForeachCallbackHandler(
                RunCustomerAndSupplierInfoChanged,
                DelegateSequenceBlockingMode.Asynchronous,
                false);
        }

        public void SaveToRegistryDatabaseEventlogExpiration(
            ServerGeneralOptions serverGeneralOptions)
        {
            _generalOptions.EventlogsExpirationDays =
                serverGeneralOptions.EventlogsExpirationDays;

            _generalOptions.EventlogsMaxCountValue =
                serverGeneralOptions.EventlogsMaxCountValue;

            _generalOptions.EventlogsMaxCountExponent =
                serverGeneralOptions.EventlogsMaxCountExponent;

            _generalOptions.EventlogTimeZoneGuidString =
                serverGeneralOptions.EventlogTimeZoneGuidString;

            _generalOptions.SaveDatabaseEventlogExpiration();

            if (DatabaseEventlogExpirationChanged != null)
                try
                {
                    DatabaseEventlogExpirationChanged();
                }
                catch
                {
                }

            CgpServerRemotingProvider.Singleton.ForeachCallbackHandler(
                RunDatabaseEventlogExpirationSettingsChanged,
                DelegateSequenceBlockingMode.Asynchronous,
                false);
        }

        public void SaveSupplierLogo(BinaryPhoto binaryPhoto)
        {
            try
            {
                if (!Directory.Exists(QuickPath.AssemblyStartupPath + PHOTOS_PATH))
                {
                    Directory.CreateDirectory(QuickPath.AssemblyStartupPath + PHOTOS_PATH);
                }

                var photoFullPath = QuickPath.AssemblyStartupPath + PHOTOS_PATH + PHOTOS_NAME;

                if (File.Exists(photoFullPath))
                    File.Delete(photoFullPath);

                if (binaryPhoto == null)
                {
                    _supplierLogo = null;
                    return;
                }

                using (var photoStream = new MemoryStream())
                {
                    photoStream.Write(binaryPhoto.BinaryData, 0, binaryPhoto.BinaryData.Length);
                    using (var imageToSave = Image.FromStream(photoStream))
                    {
                        ImageResizeUtility.SaveAsJPG(imageToSave, photoFullPath);
                    }
                }

                _supplierLogo = binaryPhoto;
            }
            catch { }
        }

        private object _lock = new object();

        public BinaryPhoto GetSupplierLogo()
        {
            lock (_lock)
            {
                if (_supplierLogo != null)
                    return _supplierLogo;

                var photoFullPath = QuickPath.AssemblyStartupPath + PHOTOS_PATH + PHOTOS_NAME;
                if (!string.IsNullOrEmpty(photoFullPath))
                {
                    Stream photoStream = null;
                    try
                    {
                        photoStream = new FileStream(photoFullPath, FileMode.Open, FileAccess.Read);
                        var binaryPhotoData = new byte[photoStream.Length];
                        photoStream.Read(binaryPhotoData, 0, binaryPhotoData.Length);
                        var binaryPhoto = new BinaryPhoto(binaryPhotoData, Path.GetExtension(photoFullPath));

                        if (binaryPhoto != null)
                            _supplierLogo = binaryPhoto;

                        return binaryPhoto;
                    }
                    catch
                    {
                    }
                    finally
                    {
                        if (photoStream != null)
                        {
                            try
                            {
                                photoStream.Close();
                            }
                            catch
                            {
                            }
                        }
                    }
                }

                return null;
            }
        }

        public void SaveToRegistryColourSettings(ServerGeneralOptions serverGeneralOptions)
        {
            _generalOptions.DragDropColorText =
                serverGeneralOptions.DragDropColorText;

            _generalOptions.DragDropColorBackground =
                serverGeneralOptions.DragDropColorBackground;

            _generalOptions.ReferenceObjectColorText =
                serverGeneralOptions.ReferenceObjectColorText;

            _generalOptions.ReferenceObjectColorBackground =
                serverGeneralOptions.ReferenceObjectColorBackground;

            _generalOptions.AlarmNotAcknowledgedColorText =
                serverGeneralOptions.AlarmNotAcknowledgedColorText;

            _generalOptions.AlarmNotAcknowledgedColorBackground =
                serverGeneralOptions.AlarmNotAcknowledgedColorBackground;

            _generalOptions.AlarmColorText =
                serverGeneralOptions.AlarmColorText;

            _generalOptions.AlarmColorBackground =
                serverGeneralOptions.AlarmColorBackground;

            _generalOptions.NormalNotAcknowledgedColorText =
                serverGeneralOptions.NormalNotAcknowledgedColorText;

            _generalOptions.NormalNotAcknowledgedColorBackground =
                serverGeneralOptions.NormalNotAcknowledgedColorBackground;

            _generalOptions.NormalColorText =
                serverGeneralOptions.NormalColorText;

            _generalOptions.NormalColorBackground =
                serverGeneralOptions.NormalColorBackground;

            _generalOptions.NoAlarmsInQueueColorText =
                serverGeneralOptions.NoAlarmsInQueueColorText;

            _generalOptions.NoAlarmsInQueueColorBackground =
                serverGeneralOptions.NoAlarmsInQueueColorBackground;

            _generalOptions.SaveColorSetting();

            if (ColorSettingsChanged != null)
                try
                {
                    ColorSettingsChanged();
                }
                catch
                {
                }

            CgpServerRemotingProvider.Singleton.ForeachCallbackHandler(
                RunColorSettingsChanged,
                DelegateSequenceBlockingMode.Asynchronous,
                false);
        }

        public void RunCustomerAndSupplierInfoChanged(ARemotingCallbackHandler remoteHandler)
        {
            var handler = remoteHandler as CustomerAndSupplierInfoChangedHandler;

            if (handler != null)
                handler.RunEvent();
        }

        public void RunColorSettingsChanged(ARemotingCallbackHandler remoteHandler)
        {
            var handler = remoteHandler as ColorSettingsChangedHandler;

            if (handler != null)
                handler.RunEvent();
        }

        public void RunDatabaseBackupSettingsChanged(ARemotingCallbackHandler remoteHandler)
        {
            var handler = remoteHandler as DatabaseBackupSettingsChangedHandler;

            if (handler != null)
                handler.RunEvent();
        }

        public void RunDatabaseEventlogExpirationSettingsChanged(ARemotingCallbackHandler remoteHandler)
        {
            var handler = remoteHandler as DatabaseExpirationEventlogSettingsChangedHandler;

            if (handler != null)
                handler.RunEvent();
        }

        public void RunSecuritySettingsChanged(ARemotingCallbackHandler remoteHandler)
        {
            var handler = remoteHandler as SecuritySettingsChangedHandler;

            if (handler != null)
                handler.RunEvent();
        }

        public void RunEventlogsChanged(ARemotingCallbackHandler remoteHandler)
        {
            var handler = remoteHandler as EventlogsChangedHandler;

            if (handler != null)
                handler.RunEvent();
        }

        public void RunDhcpServerSettingsChanged(ARemotingCallbackHandler remoteHandler)
        {
            var handler = remoteHandler as DhcpServerSettingsChangedHandler;

            if (handler != null)
                handler.RunEvent();
        }

        public void RunAdvancedAccessSettingsChanged(ARemotingCallbackHandler remoteHandler)
        {
            var handler = remoteHandler as AdvancedAccessSettingsChangedHandler;

            if (handler != null)
                handler.RunEvent();
        }

        public void RunAdvancedSettingsChanged(ARemotingCallbackHandler remoteHandler)
        {
            var handler = remoteHandler as AdvancedSettingsChangedHandler;

            if (handler != null)
                handler.RunEvent();
        }

        public void NtpDbChanged()
        {
            CgpServerRemotingProvider.Singleton.ForeachCallbackHandler(
                RunRemoteServicesSettingsChanged,
                DelegateSequenceBlockingMode.Asynchronous,
                false);
        }

        public void RunRemoteServicesSettingsChanged(ARemotingCallbackHandler remoteHandler)
        {
            var handler = remoteHandler as RemoteServicesSettingsChangedHandler;

            if (handler != null)
                handler.RunEvent();
        }

        public void RunSerialPortSettingsChanged(ARemotingCallbackHandler remoteHandler)
        {
            var handler = remoteHandler as SerialPortSettingsChangedHandler;

            if (handler != null)
                handler.RunEvent();
        }

        public void SaveToRegistryAutoCloseSettings(ServerGeneralOptions serverGeneralOptions)
        {
            _generalOptions.IsTurnedOn =
                serverGeneralOptions.IsTurnedOn;

            _generalOptions.AutoCloseTimeout =
                serverGeneralOptions.AutoCloseTimeout;

            _generalOptions.SaveAutoCloseSetting();

            if (AutoCloseSettingsChanged != null)
                try
                {
                    AutoCloseSettingsChanged();
                }
                catch
                {
                }

            CgpServerRemotingProvider.Singleton.ForeachCallbackHandler(
                RunAutocloseSettingsChanged,
                DelegateSequenceBlockingMode.Asynchronous,
                false);
        }

        public void RunAutocloseSettingsChanged(ARemotingCallbackHandler remoteHandler)
        {
            var handler = remoteHandler as AutoCloseSettingsChangedHandler;

            if (handler != null)
                handler.RunEvent();
        }

        public void SaveToRegistrySecuritySettings(ServerGeneralOptions serverGeneralOptions)
        {
            _generalOptions.ChangePassDays =
                serverGeneralOptions.ChangePassDays;

            _generalOptions.LockClientApplication =
                serverGeneralOptions.LockClientApplication;

            _generalOptions.RequirePINCardLogin =
                serverGeneralOptions.RequirePINCardLogin;

            _generalOptions.UniqueAndNotNullPersonalKey =
                serverGeneralOptions.UniqueAndNotNull;

            _generalOptions.CcuConfigurationToServerByPassword =
                serverGeneralOptions.CcuConfigurationToServerByPassword;

            _generalOptions.ReqiredSecurePin =
                serverGeneralOptions.RequiredSecurePin;

            _generalOptions.DisableCcuPnPAutomaticAssignmnet =
                serverGeneralOptions.DisableCcuPnPAutomaticAssignmnet;

            _generalOptions.ListOnlyUnassignedCardsInPersonForm =
                serverGeneralOptions.ListOnlyUnassignedCardsInPersonForm;

            _generalOptions.DelayToSaveAlarmsFromCardReaders =
                serverGeneralOptions.DelayToSaveAlarmsFromCardReaders;

            _generalOptions.UniqueAKeyCSRestriction =
                serverGeneralOptions.UniqueAKeyCSRestriction;

            _generalOptions.CardReadersAllowPINCachingInMenu =
                serverGeneralOptions.CardReadersAllowPINCachingInMenu;

            _generalOptions.MinimalCodeLength =
                serverGeneralOptions.MinimalCodeLength;

            _generalOptions.MaximalCodeLength =
                serverGeneralOptions.MaximalCodeLength;

            _generalOptions.IsPinConfirmationObligatory =
                serverGeneralOptions.IsPinConfirmationObligatory;

            _generalOptions.SaveSecuritySettings();

            CgpServerRemotingProvider.Singleton.ForeachCallbackHandler(
                RunSecuritySettingsChanged,
                DelegateSequenceBlockingMode.Asynchronous,
                false);

            CgpServer.Singleton.GeneralOptionChanged();
        }

        public void SaveToRegistryEventlogs(ServerGeneralOptions serverGeneralOptions)
        {
            _generalOptions.EventlogInputStateChanged =
                serverGeneralOptions.EventlogInputStateChanged;

            _generalOptions.EventlogOutputStateChanged =
                serverGeneralOptions.EventlogOutputStateChanged;

            _generalOptions.EventlogAlarmAreaAlarmStateChanged =
                serverGeneralOptions.EventlogAlarmAreaAlarmStateChanged;

            _generalOptions.EventlogAlarmAreaActivationStateChanged =
                serverGeneralOptions.EventlogAlarmAreaActivationStateChanged;

            _generalOptions.EventlogCardReaderOnlineStateChanged =
                serverGeneralOptions.EventlogCardReaderOnlineStateChanged;

            _generalOptions.EventSourcesReverseOrder =
                serverGeneralOptions.EventSourcesReverseOrder;

            _generalOptions.EventlogReportsTimeZoneGuidString = serverGeneralOptions.EventlogReportsTimeZoneGuidString;
            _generalOptions.EventlogReportsEmails = serverGeneralOptions.EventlogReportsEmails;

            _generalOptions.SaveEventlogs();

            CgpServerRemotingProvider.Singleton.ForeachCallbackHandler(
                RunEventlogsChanged,
                DelegateSequenceBlockingMode.Asynchronous,
                false);

            CgpServer.Singleton.GeneralOptionChanged();
        }

        public void SaveToRegistryAdvancedAccessSettings(ServerGeneralOptions serverGeneralOptions)
        {
            _generalOptions.EnableLoggingSDPSTZChanges =
                serverGeneralOptions.EnableLoggingSDPSTZChanges;

            _generalOptions.SyncingTimeFromServer =
                serverGeneralOptions.SyncingTimeFromServer;

            _generalOptions.PeriodOfTimeSyncWithoutStratum =
                serverGeneralOptions.PeriodOfTimeSyncWithoutStratum;

            _generalOptions.PeriodicTimeSyncTolerance =
                serverGeneralOptions.PeriodicTimeSyncTolerance;

            _generalOptions.AlarmAreaRestrictivePolicyForTimeBuying =
                serverGeneralOptions.AlarmAreaRestrictivePolicyForTimeBuying;

            _generalOptions.SaveAdvancedAccessSettings();

            CgpServerRemotingProvider.Singleton.ForeachCallbackHandler(
                RunAdvancedAccessSettingsChanged,
                DelegateSequenceBlockingMode.Asynchronous,
                false);

            CgpServer.Singleton.GeneralOptionChanged();
        }

        public void SaveToRegistryAdvancedSettings(ServerGeneralOptions serverGeneralOptions)
        {
            _generalOptions.MaxEventsCountForInsert =
                serverGeneralOptions.MaxEventsCountForInsert;

            _generalOptions.DelayForSaveEvents =
                serverGeneralOptions.DelayForSaveEvents;

            _generalOptions.ClientSessionTimeOut =
                serverGeneralOptions.ClientSessionTimeOut;

            _generalOptions.AlarmListSuspendedRefreshTimeout =
                serverGeneralOptions.AlarmListSuspendedRefreshTimeout;

            _generalOptions.CorrectDeserializationFailures =
                serverGeneralOptions.CorrectDeserializationFailures;

            _generalOptions.DelayForSendingChangesToCcu =
                serverGeneralOptions.DelayForSendingChangesToCcu;

            _generalOptions.SaveAdvancedSettings();

            CgpServerRemotingProvider.Singleton.ForeachCallbackHandler(
                RunAdvancedSettingsChanged,
                DelegateSequenceBlockingMode.Asynchronous,
                false);

            CgpServer.Singleton.GeneralOptionChanged();

            EventlogInsertQueue.Singleton.SetDelayForSaveEvents(
                _generalOptions.DelayForSaveEvents);
        }

        public bool IsSetSMTP()
        {
            return _generalOptions.IsSetSMTP();
        }

        public bool IsSetSerialPort()
        {
            return _generalOptions.IsSetSerialPort();
        }

        public bool IsLockCLientApplication()
        {
            return _generalOptions.LockClientApplication;
        }

        public bool TimetecCommunicationIsEnabled()
        {
            return CgpServer.Singleton.TimetecCommunicationIsEnabled;
        }

        public ServerGeneralOptions ReturnServerGeneralOptions()
        {
            try
            {
                return new ServerGeneralOptions
                {
                    SmtpServer = _generalOptions.SmtpServer,
                    SmtpPort = _generalOptions.SmtpPort,
                    SmtpSourceEmailAddress = _generalOptions.SmtpSourceEmailAddress,
                    SmtpSubject = _generalOptions.SmtpSubject,
                    SmtpCredentials = _generalOptions.SmtpCredentials,
                    SmtpSsl = _generalOptions.SmtpSsl,
                    SerialPort = _generalOptions.SerialPort,
                    SerialPortBaudRate = _generalOptions.SerialPortBaudRate,
                    SerialPortDataBits = _generalOptions.SerialPortDataBits,
                    SerialPortParity = _generalOptions.SerialPortParity,
                    SerialPortStopBits = _generalOptions.SerialPortStopBits,
                    SerialPortFlowControl = _generalOptions.SerialPortFlowControl,
                    SerialPortParityCheck = _generalOptions.SerialPortParityCheck,
                    SerialPortCarrierDetect = _generalOptions.SerialPortCarrierDetect,
                    SerialPortPin = _generalOptions.SerialPortPin,
                    TimeZoneGuidString = _generalOptions.TimeZoneGuidString,
                    DatabaseBackupPath = _generalOptions.DatabaseBackupPath,
                    EventlogsExpirationDays = _generalOptions.EventlogsExpirationDays,
                    EventlogsMaxCountValue = _generalOptions.EventlogsMaxCountValue,
                    EventlogsMaxCountExponent = _generalOptions.EventlogsMaxCountExponent,
                    EventlogTimeZoneGuidString = _generalOptions.EventlogTimeZoneGuidString,
                    DragDropColorText = _generalOptions.DragDropColorText,
                    DragDropColorBackground = _generalOptions.DragDropColorBackground,
                    ReferenceObjectColorText = _generalOptions.ReferenceObjectColorText,
                    ReferenceObjectColorBackground = _generalOptions.ReferenceObjectColorBackground,
                    AlarmNotAcknowledgedColorText = _generalOptions.AlarmNotAcknowledgedColorText,
                    AlarmNotAcknowledgedColorBackground = _generalOptions.AlarmNotAcknowledgedColorBackground,
                    AlarmColorText = _generalOptions.AlarmColorText,
                    AlarmColorBackground = _generalOptions.AlarmColorBackground,
                    NormalNotAcknowledgedColorText = _generalOptions.NormalNotAcknowledgedColorText,
                    NormalNotAcknowledgedColorBackground = _generalOptions.NormalNotAcknowledgedColorBackground,
                    NormalColorText = _generalOptions.NormalColorText,
                    NormalColorBackground = _generalOptions.NormalColorBackground,
                    NoAlarmsInQueueColorText = _generalOptions.NoAlarmsInQueueColorText,
                    NoAlarmsInQueueColorBackground = _generalOptions.NoAlarmsInQueueColorBackground,
                    IsTurnedOn = _generalOptions.IsTurnedOn,
                    AutoCloseTimeout = _generalOptions.AutoCloseTimeout,
                    ChangePassDays = _generalOptions.ChangePassDays,
                    RequirePINCardLogin = _generalOptions.RequirePINCardLogin,
                    UniqueAndNotNull = _generalOptions.UniqueAndNotNullPersonalKey,
                    LockClientApplication = _generalOptions.LockClientApplication,
                    CcuConfigurationToServerByPassword = _generalOptions.CcuConfigurationToServerByPassword,
                    RequiredSecurePin = _generalOptions.ReqiredSecurePin,
                    DisableCcuPnPAutomaticAssignmnet = _generalOptions.DisableCcuPnPAutomaticAssignmnet,
                    ListOnlyUnassignedCardsInPersonForm = _generalOptions.ListOnlyUnassignedCardsInPersonForm,
                    DelayToSaveAlarmsFromCardReaders = _generalOptions.DelayToSaveAlarmsFromCardReaders,
                    UniqueAKeyCSRestriction = _generalOptions.UniqueAKeyCSRestriction,
                    CardReadersAllowPINCachingInMenu = _generalOptions.CardReadersAllowPINCachingInMenu,
                    MinimalCodeLength = _generalOptions.MinimalCodeLength,
                    MaximalCodeLength = _generalOptions.MaximalCodeLength,
                    IsPinConfirmationObligatory = _generalOptions.IsPinConfirmationObligatory,
                    EventlogInputStateChanged = _generalOptions.EventlogInputStateChanged,
                    EventlogOutputStateChanged = _generalOptions.EventlogOutputStateChanged,
                    EventlogAlarmAreaAlarmStateChanged = _generalOptions.EventlogAlarmAreaAlarmStateChanged,
                    EventlogAlarmAreaActivationStateChanged = _generalOptions.EventlogAlarmAreaActivationStateChanged,
                    EventlogCardReaderOnlineStateChanged = _generalOptions.EventlogCardReaderOnlineStateChanged,
                    EventSourcesReverseOrder = _generalOptions.EventSourcesReverseOrder,
                    EventlogReportsTimeZoneGuidString = _generalOptions.EventlogReportsTimeZoneGuidString,
                    EventlogReportsEmails = _generalOptions.EventlogReportsEmails,
                    EnableLoggingSDPSTZChanges = _generalOptions.EnableLoggingSDPSTZChanges,
                    SyncingTimeFromServer = _generalOptions.SyncingTimeFromServer,
                    PeriodOfTimeSyncWithoutStratum = _generalOptions.PeriodOfTimeSyncWithoutStratum,
                    PeriodicTimeSyncTolerance = _generalOptions.PeriodicTimeSyncTolerance,
                    AlarmAreaRestrictivePolicyForTimeBuying = _generalOptions.AlarmAreaRestrictivePolicyForTimeBuying,
                    MaxEventsCountForInsert = _generalOptions.MaxEventsCountForInsert,
                    DelayForSaveEvents = _generalOptions.DelayForSaveEvents,
                    ClientSessionTimeOut = _generalOptions.ClientSessionTimeOut,
                    AlarmListSuspendedRefreshTimeout = _generalOptions.AlarmListSuspendedRefreshTimeout,
                    CorrectDeserializationFailures = _generalOptions.CorrectDeserializationFailures,
                    DelayForSendingChangesToCcu = _generalOptions.DelayForSendingChangesToCcu,
                    CustomerCompanyName = _generalOptions.CustomerCompanyName,
                    CustomerContactPerson = _generalOptions.CustomerContactPerson,
                    CustomerCityState = _generalOptions.CustomerCityState,
                    CustomerCountry = _generalOptions.CustomerCountry,
                    CustomerDeliveryAddress = _generalOptions.CustomerDeliveryAddress,
                    CustomerPhone = _generalOptions.CustomerPhone,
                    CustomerWebsite = _generalOptions.CustomerWebsite,
                    CustomerZipCode = _generalOptions.CustomerZipCode,

                    SupplierCompanyName = _generalOptions.SupplierCompanyName,
                    SupplierContactPerson = _generalOptions.SupplierContactPerson,
                    SupplierCityState = _generalOptions.SupplierCityState,
                    SupplierCountry = _generalOptions.SupplierCountry,
                    SupplierDeliveryAddress = _generalOptions.SupplierDeliveryAddress,
                    SupplierPhone = _generalOptions.SupplierPhone,
                    SupplierWebsite = _generalOptions.SupplierWebsite,
                    SupplierZipCode = _generalOptions.SupplierZipCode
                };
            }
            catch
            {
                return null;
            }
        }
    }
}