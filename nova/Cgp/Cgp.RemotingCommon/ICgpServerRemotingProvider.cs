using System;
using System.Collections.Generic;
using System.Net;

using Contal.Cgp.BaseLib;
using Contal.Cgp.Globals.PlatformPC;
using Contal.Cgp.Server.Beans;
using Contal.LwDhcp;
using Contal.IwQuick.Remoting;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.RemotingCommon;

namespace Contal.Cgp.RemotingCommon
{
    public interface ICgpServerRemotingProvider : IRemotingService
    {
        string ToString();
        IPersons Persons { get; }
        ILogins Logins { get; }
        IPresentationGroups PresentationGroups { get; }
        IPresentationFormatters PresentationFormatters { get; }
        IEventlogs Eventlogs { get; }
        IConsecutiveEvent ConsecutiveEvents { get; }
        ISystemEvents SystemEvents { get; }
        ICisNGs CisNGs { get; }
        ICards Cards { get; }
        ICardSystems CardSystems { get; }
        ICars Cars { get; }
        ICentralNameRegisters CentralNameRegisters { get; }
        IServerGenaralOptionsProvider ServerGenaralOptionsProvider { get; }
        ICisNGGroups CisNGGroups { get; }
        IDailyPlans DailyPlans { get; }
        IDayIntervals DayIntervals { get; }
        ITimeZones TimeZones { get; }
        ITimeZoneDateSettings TimeZoneDateSettings { get; }
        IDayTypes DayTypes { get; }
        ICalendars Calendars { get; }
        ICalendarDateSettings CalendarDateSettings { get; }
        ICustomAccessControls CustomAccessControls { get; }
        IUserFoldersSutructures UserFoldersSutructures { get; }
        IServerGeneralOptionsDBs ServerGeneralOptionsDBs { get; }
        IAlarmPrioritiesDbs AlarmPrioritiesDbs { get; }
        ICSVImportSchemas CSVImportSchemas { get; }
        IGlobalAlarmInstructions GlobalAlarmInstructions { get; }
        ICardTemplates CardTemplates { get; }
        ICardPairs CardPairs { get; }
        ICarCards CarCards { get; }
        ILoginGroups LoginGroups { get; }
        IStructuredSubSites StructuredSubSites { get; }
        uint DatabaseLoginCount { get; }
        bool DemoLicence { get; }

        bool ClientConnect(string friendlyName, Guid clientID);

        void ClientDisconnect(string friendlyName, Guid clientID);
        byte GetDailyPlanActualStatus(Guid idDailyPlan);
        byte GetTimeZoneActualStatus(Guid idTimeZone);
        void StopServer(out Exception error);
        void StopDBTest();
        void RunDBTest();
        string Version { get; }
        bool HasAccess(Access access);
        bool HasAccess(List<Access> accesses);
        bool HasAccessView(ObjectType objectType, object id);
        bool HasAccessView(ObjectType objectType, object id, string loginUserName);
        bool HasObjectParent(AOrmObject ormObject);
        bool IsObjectTypeRelevantForStructuredSites(ObjectType objectType);
        bool Unauthenticate(out Exception error);
        bool ResetUserSession(ARemotingAuthenticationParameters parameters);
        ICollection<ServerAlarmCore> GetAlarms();
        ServerAlarmCore AcknowledgeAlarmState(IdServerAlarm idServerAlarm);
        void ConfirmAlarmsState(IEnumerable<IdServerAlarm> idsServerAlarm);
        void BlockUnblockAlarms(IEnumerable<IdServerAlarm> idsServerAlarm, bool block);
        ServerAlarmCore BlockAlarm(IdServerAlarm idServerAlarm);
        ServerAlarmCore UnblockAlarm(IdServerAlarm idServerAlarm);

        /// <summary>
        /// Get objects that supports seach in Database, clasess which implement IDatabaseSearch
        /// </summary>
        /// <param name="plugins">plugins to search</param>
        /// <returns>TableTypeSettings</returns>
        ICollection<TableTypeSettings> GetSearchableObjectTypes(ICollection<string> plugins);
        IEnumerable<AOrmObject> SearchObjects(string name, ObjectType? objectType, ICollection<string> plugins);

        IEnumerable<AOrmObject> GetFultextSearchResult(string name, List<string> plugins);
        IEnumerable<AOrmObject> GetParameterSearchResult(string name, ObjectType? objectType, ICollection<string> plugins);
        IList<UserFoldersStructure> GetFolderStructureSearchResult(string text);

        IEnumerable<AOrmObject> GetAllOrmObjectsOfObjectType(ObjectType? objectType, List<string> plugins);

        bool TestSendMailPresentationGroup(PresentationGroup pg, string message, out Exception error);
        string GetLicencePath();
        DateTime GetLicenceExpirationDate();
        bool CheckTimetecLicense();
        Dictionary<string, string> GetLicenceProperies();
        bool GetLicensePropertyInfo(string propertyName, out string localisedName, out object value);        
        DHCPGroups GetServerDHCPGroups();
        string GetServerIpAddress(IPAddress networkAddress, IPAddress networkMask);
        void SetDHCPGroups(DHCPGroups dhcpGroups);
        void AutoStartDHCP(bool auto);
        bool RemoveDHCPGroup(DHCPGroup dhcpGroup);
        bool GetAutoDHCPStatus();
        bool StartDHCP(bool toStart);
        bool GetDHCPRunningState();
        int GenerateLicenceRequestFile(Dictionary<int, string> propertyNames, Dictionary<int, string> propertyValues,
            string commonName, string CustomerName, string customerAddress, string contactFirstName, string contactSecondName,
            string contactPhone, string contactEmail, string publisherName, string publisherKey, string filePath);

        bool InsertLicenceFile(string fileName, byte[] fileBytes, out bool isValid, out bool needRestart);
        bool LoadLicenceRequestFields(out string publisherName, out string publisherKey, out string commonName,
            out Dictionary<int, string> propertyNames,
            out Dictionary<int, string> propertyTypes, out Dictionary<int, string> propertyValues,
            out Dictionary<int, string> propertyDescriptions);
        DateTime GetServerDateTime();
        TimeZoneInfo GetServerTimeZoneInfo();
        DateTime GetServerDemoStartTime();
        double GetServerDemoMaxTime();
        bool ForceDatabaseBackup(string path);
        bool ForceEventlogCleaning(int expiratedDays, int maxRecords);
        string DatabaseBackupFileName();
        int GetSessionTimeout();
        Exception ClientAuthenticate(ARemotingAuthenticationParameters parameters);
        Exception ClientAutenticateWithCard(string card, out string loginName);
        bool ReturnIsPinCardLoginRequiered();
        byte GetCardPinLengthByFullCardNumber(string cardNumber);
        bool IsValidPinForCard(string cardNumber, string hashedPin);
        string GetLoginLanguage();
        void SetLoginLanguage(string language);
        void SendInfoClientConnected(Guid clientID);
        void SaveUserOpenedWindows(List<UserOpenedWindow> userOpenedWindows, string userName);
        ICollection<UserOpenedWindow> GetUserOpenedWindows();
        bool NovaClientUpgradeAvailable(Version callerVersion, out Version newVersion);
        byte[] GetClientUpgradeData(Version upgradeVersion, int fromFilePosition);
        byte[] GetLastEventlogReport(int fromFilePosition);
        bool IsLastEventlogReportAvailable();
        void CleanLastEventlogReport();
        long GetClientUpgradeFileLength(Version upgradeVersion);
        Dictionary<string, Type> GetAllRequiredLicenceProperties();
        bool GetLicencePropertyLocalisedName(string propertyName, string language, out string localisedName);


        bool GenerateCards(
            string fullPrefix,
            byte cardSystem,
            int numberDigits,
            decimal fromNumber,
            decimal toNumber,
            string pin,
            CardState cardState,
            int? idStructuredSubSite);

        IList<string> GetServerDirectory(string path);
        bool ExistFileOnServer(string fileName);
        bool DirectoryExitsOnServer(string directoryName);
        string[] GetAllLanguages();
        string GetCurrentLanguage();
        void SetServerLanguage(string language);
        Dictionary<string, object> GetServerInformations();
        AOrmObject GetTableObject(ObjectType objectType, string strObjectId);
        ICollection<IModifyObject> GetModifyObjects(ICollection<IdAndObjectType> idAndObjectTypes);
        ICollection<IModifyObject> GetIModifyObjects();
        ICollection<ServerAlarmCore> GetAlarms(AlarmType alarmType, IdAndObjectType alarmObject);
    }
}
