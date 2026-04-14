using System;
using System.Collections.Generic;

using Contal.Cgp.Globals;
using Contal.Cgp.Globals.PlatformPC;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.NCAS.Globals;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.BaseLib;
using System.Data;

namespace Contal.Cgp.NCAS.RemotingCommon
{
    public interface ICgpNCASRemotingProvider : IwQuick.Remoting.IRemotingService
    {
        string ToString();
        ICardReaders CardReaders { get; }
        ILprCameras LprCameras { get; }
        ICCUs CCUs { get; }
        IDCUs DCUs { get; }
        IAccessControlLists AccessControlLists { get; }
        IACLSettings ACLSettings { get; }
        IACLSettingAAs ACLSettingAAs { get; }
        IInputs Inputs { get; }
        IACLPersons ACLPersons { get; }
        IOutputs Outputs { get; }
        IAlarmAreas AlarmAreas { get; }
        IAACardReaders AACardReaders { get; }
        ISecurityDailyPlans SecurityDailyPlans { get; }
        ISecurityDayIntervals SecurityDayIntervals { get; }
        ISecurityTimeZones SecurityTimeZones { get; }
        ISecurityTimeZoneDateSettings SecurityTimeZoneDateSettings { get; }
        IAccessZones AccessZones { get; }
        IDoorEnvironments DoorEnvironments { get; }
        IBaseOrmTable<DevicesAlarmSetting> DevicesAlarmSettings { get; }
        IScenes Scenes { get; }
        IGraphicSymbols GraphicSymbols { get; }
        IGraphicSymbolRawDatas GraphicSymbolRawDatas { get; }
        IGraphicSymbolTemplates GraphicSymbolTemplates { get; }
        IGraphicsViews GraphicsViews { get; }
        IACLGroups AclGroups { get; }
        IAntiPassBackZones AntiPassBackZones { get; }
        IMultiDoors MultiDoors { get; }
        IMultiDoorElements MultiDoorElements { get; }
        IFloors Floors { get; }
        IAlarmTransmitters AlarmTransmitters { get; }
        IAlarmArcs AlarmArcs { get; }
        ITimetecSettings TimetecSettings { get; }

        // SB
        IPersonAttributes PersonAttributes { get; }

        IReportSettings ReportSettings { get; }

        IPersonAttributeOutput PersonAttributeOutputs { get; }

        IExcelReportOutput ExcelReportOutputs { get; }

        byte GetSecurityTimeZoneActualStatus(Guid idSecurityTimeZone);
        byte GetSecurityDailyPlanActualStatus(Guid idSecurityDailyPlan);
        IList<string> GetAvailableUpgrades();

        bool StartUpgradeCCU(string fileVersion, string ipAddress, Guid upgradeID, out Exception ex);
        bool CanStop(Guid upgradeID, Guid ccuGuid);
        bool StopUnpackUpgradeProcess(Guid upgradeID, Guid ccuGuid);
        bool StopTransferUpgradeProcess(Guid upgradeID, Guid ccuGuid);
        void UpgradeDCU(string selectedVersion, Guid upgradeID, string ccuIpAddress, byte dcuLogicalAddress, out Exception ex, out List<byte> registredDCUs);
        void UpgradeDCUs(string selectedVersion, Guid upgradeID, string ccuIpAddress, byte[] dcuLogicalAddresses, out Exception ex, out List<byte> registredDCUs);
        IList<string> GetAvailableDCUUpgrades(Guid ccuGuid, out string delimeter);
        IList<string[]> GetAvailableCEUpgrades();
        bool IsCCUUpgrading(Guid ccuGuid);
        bool CanUpgradeDCU(Guid ccuGuid, byte dcuLogicalAddress);
        bool SetDefaultDCUUpgradeVersion(Guid ccuGuid, string upgradeVersion);
        bool ExistsPathOrFile(Guid ccuGuid, string fullName);
        bool StartUpgradeCE(string selectedFileName, Guid upgradeID, string ipAddress, out Exception ex);
        bool GetLicencePropertyInfo(string propertyName, out string localisedPropertyName, out object propertyValue);
        IList<string> GetAvailableCRUpgrades(out string delimeter);
        bool UpgradeCRs(string selectedVersion, byte crHwVersion, Guid upgradeID, string ccuIpAddress, List<Guid> ccusCardReadersToUpgrade, Dictionary<byte, List<byte>> dcusCardReadersToUpgrade, out Exception ex);
        bool GetEnableParentInFullName();
        void InsertLookupedCCUs(List<string> ipAddresses, int? idStructuredSubSite);
        void CreateLookupedLprCameras(ICollection<LookupedLprCamera> lookupedCameras, int? idStructuredSubSite);
        void DeviceAlarmSettingsChanged(Dictionary<AlarmType, bool> changedSettings);
        string CheckAlarmAreaActivationRights(Guid guidCCU, Guid guidPerson, Guid guidAlarmArea);
        void GetCCUSupportedVersions(out string minimalSupportedCCUVersion, out string maximalSupportedCCUVersion, out string minCCUVersionForWinCeChecking, out string minimalSupportedCeVersion);
        bool ExistFileForUpgrade(UpgradeType upgradeType, string fileName);
        bool SendFileForUpgrade(UpgradeType upgradeType, string fileName, byte[] data);
        bool RemoveCCUEvents(Guid guidCCU);
        BlockFileInfo[] GetBlockFilesInfo(Guid guidCCU);
        bool ResetBlockFilesInfo(Guid guidCCU);
        IList<ThreadInfo> GetThreadsInfo(Guid guidCCU);
        IList<VerbosityLevelInfo> GetVerbosityLevelsInfo(Guid guidCCU);
        bool SetVerbosityLevel(Guid guidCCU, byte verbosityLevel);
        byte GetCurrentVerbosity(Guid guidCCU);
        bool DeleteUpgradeFile(UpgradeType upgradeType, string version);
        State GetRealOnOffObjectState(ObjectType type, Guid id);
        ICollection<ServerAlarmCore> GetAlarms(AlarmType alarmType, IdAndObjectType alarmObject);
        ICollection<MemoryInfo> GetMemoryReport(Guid idCcu);
        DataTable ExportDataLogs(IList<FilterSettings> filterSettings, bool bCardReader, out bool bFillSection);
    }
}
