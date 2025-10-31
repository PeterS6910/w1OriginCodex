using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Contal.Cgp.Globals.PlatformPC;
using Contal.Cgp.NCAS.RemotingCommon;
using Contal.Cgp.NCAS.Server.DB;
using Contal.Cgp.Server.Alarms;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick.Remoting;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.IwQuick.Sys;
using Contal.Cgp.NCAS.Globals;
using Contal.Cgp.Server;
using Contal.Drivers.CardReader;
using System.Windows;
using Contal.Cgp.BaseLib;
using System.Data;
using Contal.Cgp.NCAS.Server.ExportData;

namespace Contal.Cgp.NCAS.Server
{
    public class NCASServerRemotingProvider : ARemotingService, ICgpNCASRemotingProvider
    {
        public NCASServerRemotingProvider()
        {
            base.SetRemotingAuthentication(CgpServerRemotingProvider.Singleton.RemotingAuthentication);
        }

        protected static volatile NCASServerRemotingProvider _singleton = null;
        private static object _syncRoot = new object();

        public static NCASServerRemotingProvider Singleton
        {
            get
            {
                if (null == _singleton)
                    lock (_syncRoot)
                    {
                        if (_singleton == null)
                            _singleton = new NCASServerRemotingProvider();
                    }

                return _singleton;
            }
        }

        string ICgpNCASRemotingProvider.ToString()
        {
            return base.ToString();
        }

        public ICardReaders CardReaders
        {
            get
            {
                try
                {
                    ValidateSession();
                    return DB.CardReaders.Singleton;
                }
                catch
                {
                    return null;
                }
            }
        }

        public ILprCameras LprCameras
        {
            get
            {
                try
                {
                    ValidateSession();
                    return NcasDbs.GetTableOrm(ObjectType.LprCamera) as ILprCameras;
                }
                catch
                {
                    return null;
                }
            }
        }

        public IScenes Scenes
        {
            get
            {
                try
                {
                    ValidateSession();
                    return DB.Scenes.Singleton;
                }
                catch
                {
                    return null;
                }
            }
        }

        public ITimetecSettings TimetecSettings
        {
            get
            {
                try
                {
                    ValidateSession();
                    return DB.TimetecSettings.Singleton;
                }
                catch
                {
                    return null;
                }
            }
        }

        public IGraphicSymbols GraphicSymbols
        {
            get
            {
                try
                {
                    ValidateSession();
                    return DB.GraphicSymbols.Singleton;
                }
                catch
                {
                    return null;
                }
            }
        }

        public IGraphicSymbolRawDatas GraphicSymbolRawDatas
        {
            get
            {
                try
                {
                    ValidateSession();
                    return DB.GraphicSymbolRawDatas.Singleton;
                }
                catch
                {
                    return null;
                }
            }
        }

        public IGraphicSymbolTemplates GraphicSymbolTemplates
        {
            get
            {
                try
                {
                    ValidateSession();
                    return DB.GraphicSymbolTemplates.Singleton;
                }
                catch
                {
                    return null;
                }
            }
        }

        public IGraphicsViews GraphicsViews
        {
            get
            {
                try
                {
                    ValidateSession();
                    return DB.GraphicsViews.Singleton;
                }
                catch
                {
                    return null;
                }
            }
        }

        public ICCUs CCUs
        {
            get
            {
                try
                {
                    ValidateSession();
                    return DB.CCUs.Singleton;
                }
                catch
                {
                    return null;
                }
            }
        }

        public IDCUs DCUs
        {
            get
            {
                try
                {
                    ValidateSession();
                    return DB.DCUs.Singleton;
                }
                catch
                {
                    return null;
                }
            }
        }

        public IAccessControlLists AccessControlLists
        {
            get
            {
                try
                {
                    ValidateSession();
                    return DB.AccessControlLists.Singleton;
                }
                catch
                {
                    return null;
                }
            }
        }

        public IACLSettings ACLSettings
        {
            get
            {
                try
                {
                    ValidateSession();
                    return DB.ACLSettings.Singleton;
                }
                catch
                {
                    return null;
                }
            }
        }

        public IACLSettingAAs ACLSettingAAs
        {
            get
            {
                try
                {
                    ValidateSession();
                    return DB.ACLSettingAAs.Singleton;
                }
                catch
                {
                    return null;
                }
            }
        }

        public IInputs Inputs
        {
            get
            {
                try
                {
                    ValidateSession();
                    return DB.Inputs.Singleton;
                }
                catch
                {
                    return null;
                }
            }
        }

        public IACLPersons ACLPersons
        {
            get
            {
                try
                {
                    ValidateSession();
                    return DB.ACLPersons.Singleton;
                }
                catch
                {
                    return null;
                }
            }
        }

        public IOutputs Outputs
        {
            get
            {
                try
                {
                    ValidateSession();
                    return DB.Outputs.Singleton;
                }
                catch
                {
                    return null;
                }
            }
        }

        public IAlarmAreas AlarmAreas
        {
            get
            {
                try
                {
                    ValidateSession();
                    return DB.AlarmAreas.Singleton;
                }
                catch
                {
                    return null;
                }
            }
        }

        public IAACardReaders AACardReaders
        {
            get
            {
                try
                {
                    ValidateSession();
                    return DB.AACardReaders.Singleton;
                }
                catch
                {
                    return null;
                }
            }
        }
        public ISecurityDailyPlans SecurityDailyPlans
        {
            get
            {
                try
                {
                    ValidateSession();
                    return DB.SecurityDailyPlans.Singleton;
                }
                catch
                {
                    return null;
                }
            }
        }

        public ISecurityDayIntervals SecurityDayIntervals
        {
            get
            {
                try
                {
                    ValidateSession();
                    return DB.SecurityDayIntervals.Singleton;
                }
                catch
                {
                    return null;
                }
            }
        }

        public ISecurityTimeZones SecurityTimeZones
        {
            get
            {
                try
                {
                    ValidateSession();
                    return DB.SecurityTimeZones.Singleton;
                }
                catch
                {
                    return null;
                }
            }
        }

        public ISecurityTimeZoneDateSettings SecurityTimeZoneDateSettings
        {
            get
            {
                try
                {
                    ValidateSession();
                    return DB.SecurityTimeZoneDateSettings.Singleton;
                }
                catch
                {
                    return null;
                }
            }
        }

        public IAccessZones AccessZones
        {
            get
            {
                try
                {
                    ValidateSession();
                    return DB.AccessZones.Singleton;
                }
                catch
                {
                    return null;
                }
            }
        }

        public IDoorEnvironments DoorEnvironments
        {
            get
            {
                try
                {
                    ValidateSession();
                    return DB.DoorEnvironments.Singleton;
                }
                catch
                {
                    return null;
                }
            }
        }

        public IBaseOrmTable<DevicesAlarmSetting> DevicesAlarmSettings
        {
            get
            {
                try
                {
                    ValidateSession();
                    return DB.DevicesAlarmSettings.Singleton;
                }
                catch
                {
                    return null;
                }
            }
        }

        public IACLGroups AclGroups
        {
            get
            {
                try
                {
                    ValidateSession();
                    return ACLGroups.Singleton;
                }
                catch
                {
                    return null;
                }
            }
        }

        public IAntiPassBackZones AntiPassBackZones
        {
            get
            {
                try
                {
                    ValidateSession();
                    return DB.AntiPassBackZones.Singleton;
                }
                catch
                {
                    return null;
                }
            }
        }

        public IMultiDoors MultiDoors
        {
            get
            {
                try
                {
                    ValidateSession();
                    return DB.MultiDoors.Singleton;
                }
                catch
                {
                    return null;
                }
            }
        }

        public IMultiDoorElements MultiDoorElements
        {
            get
            {
                try
                {
                    ValidateSession();
                    return DB.MultiDoorElements.Singleton;
                }
                catch
                {
                    return null;
                }
            }
        }

        public IFloors Floors
        {
            get
            {
                try
                {
                    ValidateSession();
                    return DB.Floors.Singleton;
                }
                catch
                {
                    return null;
                }
            }
        }

        public IAlarmTransmitters AlarmTransmitters
        {
            get
            {
                try
                {
                    ValidateSession();
                    return DB.AlarmTransmitters.Singleton;
                }
                catch
                {
                    return null;
                }
            }
        }

        public IAlarmArcs AlarmArcs
        {
            get
            {
                try
                {
                    ValidateSession();
                    return DB.AlarmArcs.Singleton;
                }
                catch
                {
                    return null;
                }
            }
        }

        // SB
        public IPersonAttributes PersonAttributes
        {
            get
            {
                try
                {
                    ValidateSession();
                    return DB.PersonAttributes.Singleton;
                }
                catch
                {
                    return null;
                }
            }
        }

        public IReportSettings ReportSettings
        {
            get
            {
                try
                {
                    ValidateSession();
                    return DB.ReportSettings.Singleton;
                }
                catch
                {
                    return null;
                }
            }
        }

        public IPersonAttributeOutput PersonAttributeOutputs
        {
            get
            {
                try
                {
                    ValidateSession();
                    return DB.PersonAttributeOutputs.Singleton;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }


        public IExcelReportOutput ExcelReportOutputs
        {
            get
            {
                try
                {
                    ValidateSession();
                    return DB.ExcelReportOutputs.Singleton;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        public byte GetSecurityTimeZoneActualStatus(Guid idSecurityTimeZone)
        {
            return SecurityTimeAxis.Singleton.GetActualStatusSecurityTZ(idSecurityTimeZone);
        }

        public byte GetSecurityDailyPlanActualStatus(Guid idSecurityDailyPlan)
        {
            return SecurityTimeAxis.Singleton.GetActualStatusSecurityDP(idSecurityDailyPlan);
        }

        public IList<string> GetAvailableUpgrades()
        {
            return NCASServer.Singleton.GetAvailableUpgrades();
        }

        public bool StartUpgradeCCU(string fileVersion, string ipAddress, Guid upgradeID, out Exception ex)
        {
            return NCASServer.Singleton.StartUpgradeCCU(fileVersion, ipAddress, upgradeID, out ex);
        }

        public bool IsCCUUpgrading(Guid ccuGuid)
        {
            return NCASServer.Singleton.IsCCUUpgrading(ccuGuid);
        }

        public bool CanUpgradeDCU(Guid ccuGuid, byte dcuLogicalAddress)
        {
            return NCASServer.Singleton.CanUpgradeDCU(ccuGuid, dcuLogicalAddress);
        }

        public bool CanStop(Guid upgradeID, Guid ccuGuid)
        {
            return NCASServer.Singleton.CanStop(upgradeID, ccuGuid);
        }

        public bool StopUnpackUpgradeProcess(Guid upgradeID, Guid guidCCU)
        {
            return NCASServer.Singleton.StopUnpackUpgradeProcess(upgradeID, guidCCU);
        }

        public bool StopTransferUpgradeProcess(Guid upgradeID, Guid ccuGuid)
        {
            return NCASServer.Singleton.StopTransferUpgradeProcess(upgradeID, ccuGuid);
        }

        public void UpgradeDCU(string selectedVersion, Guid upgradeID, string ccuIpAddress, byte dcuLogicalAddress, out Exception ex, out List<byte> registeredDCUs)
        {
            NCASServer.Singleton.UpgradeDCU(selectedVersion, upgradeID, ccuIpAddress, dcuLogicalAddress, out ex, out registeredDCUs);
        }

        public void UpgradeDCUs(string selectedVersion, Guid upgradeID, string ccuIpAddress, byte[] dcuLogicalAddresses, out Exception ex, out List<byte> registeredDCUs)
        {
            NCASServer.Singleton.UpgradeDCUs(selectedVersion, upgradeID, ccuIpAddress, dcuLogicalAddresses, out ex, out registeredDCUs);
        }

        public IList<string> GetAvailableDCUUpgrades(Guid ccuGuid, out string delimeter)
        {
            return NCASServer.Singleton.GetAvailableDCUUpgrades(ccuGuid, out delimeter);
        }

        public IList<string> GetAvailableCRUpgrades(out string delimeter)
        {
            return NCASServer.Singleton.GetAvailableCRUpgrades(out delimeter);
        }

        public IList<string[]> GetAvailableCEUpgrades()
        {
            return NCASServer.Singleton.GetAvailableCEUpgrades();
        }

        public bool SetDefaultDCUUpgradeVersion(Guid ccuGuid, string upgradeVersion)
        {
            return NCASServer.Singleton.SetDefaultDCUUpgradeVersion(ccuGuid, upgradeVersion);
        }

        public bool ExistsPathOrFile(Guid ccuGuid, string fullName)
        {
            return DB.CCUs.Singleton.ExistsPathOrFile(ccuGuid, fullName);
        }

        public bool StartUpgradeCE(string selectedFileName, Guid upgradeID, string ipAddress, out Exception ex)
        {
            return NCASServer.Singleton.UpgradeCE(selectedFileName, upgradeID, ipAddress, out ex);
        }

        public bool GetLicencePropertyInfo(string propertyName, out string localisedPropertyName, out object propertyValue)
        {
            return NCASServer.Singleton.GetLicencePropertyInfo(propertyName, out localisedPropertyName, out propertyValue);
        }

        public bool UpgradeCRs(
            string selectedVersion, 
            byte crHwVersion, 
            Guid upgradeID, 
            string ccuIpAddress, 
            List<Guid> ccusCardReadersToUpgrade, 
            Dictionary<byte, List<byte>> dcusCardReadersToUpgrade, 
            out Exception ex)
        {
            return NCASServer.Singleton.UpgradeCRs(
                selectedVersion, 
                crHwVersion, 
                upgradeID,
                ccuIpAddress,
                ccusCardReadersToUpgrade,
                dcusCardReadersToUpgrade,
                out ex);
        }

        public bool GetEnableParentInFullName()
        {
            return Support.EnableParentInFullName;
        }

        public void InsertLookupedCCUs(
            List<string> ipAddresses,
            int? idStructuredSubSite)
        {
            CCUConfigurationHandler.Singleton.CreateCCUs(
                ipAddresses,
                idStructuredSubSite);
        }

        public void CreateLookupedLprCameras(
            ICollection<LookupedLprCamera> lookupedCameras,
            int? idStructuredSubSite)
        {
            DB.LprCameras.Singleton.CreateLookupedLprCameras(
                lookupedCameras,
                idStructuredSubSite);
        }

        public string CheckAlarmAreaActivationRights(Guid guidCCU, Guid guidPerson, Guid guidAlarmArea)
        {
            return CCUConfigurationHandler.Singleton.CheckAlarmAreaActivationRights(guidCCU, guidPerson, guidAlarmArea).ToString();
        }

        public void DeviceAlarmSettingsChanged(Dictionary<AlarmType, bool> changedSettings)
        {
            foreach (KeyValuePair<AlarmType, bool> item in changedSettings)
            {
                switch (item.Key)
                {
                    case AlarmType.CCU_Offline: CCUConfigurationHandler.Singleton.CreateAlarmsCCUOffline();
                        break;
                    case AlarmType.CCU_Unconfigured: CCUConfigurationHandler.Singleton.CreateAlarmsCCUUnconfigured();
                        break;
                    case AlarmType.DCU_Offline: CCUAlarms.UpdateAlarmsDcuOfflineDueCcuOffline();
                        break;
                    case AlarmType.CardReader_Offline: CCUAlarms.UpdateAlarmsCrOfflineDueCcuOffline();
                        break;
                    case AlarmType.AlarmArea_ReportingToCR: CCUConfigurationHandler.Singleton.AlarmsAreaReportingToCRChanged(item.Value);
                        break;
                }
            }
        }

        public void GetCCUSupportedVersions(out string minCCUSupportedVersion, out string maxCCUSupportedVersion, out string minCCUVersionForWinCeChecking, out string minCESupportedVersion)
        {
            minCCUSupportedVersion = NCASServer.MINIMAL_FIRMWARE_VERSION_FOR_CCU.ToString();
            maxCCUSupportedVersion = NCASServer.Singleton.MaximalFirmwareVersionForCCU.ToString();
            minCESupportedVersion = NCASServer.MINIMAL_FIRMWARE_VERSION_FOR_CE.ToString();
            minCCUVersionForWinCeChecking = NCASServer.MINIMAL_FIRMWARE_VERSION_FOR_CE_CHECKING.ToString();
        }

        public bool ExistFileForUpgrade(UpgradeType upgradeType, string fileName)
        {
            switch (upgradeType)
            {
                case UpgradeType.CCUUpgrade:
                    return File.Exists(QuickPath.AssemblyStartupPath + @"\" + NCASServer.CCU_UPGRADES_DIRECTORY_NAME + @"\" + fileName);
                case UpgradeType.CEUpgrade:
                    return File.Exists(QuickPath.AssemblyStartupPath + @"\" + NCASServer.CE_UPGRADES_DIRECTORY_NAME + @"\" + fileName);
                case UpgradeType.DCUUpgrade:
                    return File.Exists(QuickPath.AssemblyStartupPath + @"\" + NCASServer.DCU_UPGRADES_DIRECTORY_NAME + @"\" + fileName);
                case UpgradeType.CRUpgrade:
                    return File.Exists(QuickPath.AssemblyStartupPath + @"\" + NCASServer.CR_UPGRADES_DIRECTORY_NAME + @"\" + fileName);
            }

            return false;
        }

        public bool SendFileForUpgrade(UpgradeType upgradeType, string fileName, byte[] data)
        {
            if (data == null || data.Length == 0 || string.IsNullOrEmpty(fileName))
                return false;

            bool retValue = true;
            Stream fileStream = null;
            string fullFilePath = string.Empty;

            try
            {
                switch (upgradeType)
                {
                    case UpgradeType.CCUUpgrade:
                        fullFilePath = QuickPath.AssemblyStartupPath + @"\" + NCASServer.CCU_UPGRADES_DIRECTORY_NAME + @"\" + fileName;
                        break;
                    case UpgradeType.CEUpgrade:
                        fullFilePath = QuickPath.AssemblyStartupPath + @"\" + NCASServer.CE_UPGRADES_DIRECTORY_NAME + @"\" + fileName;
                        break;
                    case UpgradeType.DCUUpgrade:
                        fullFilePath = QuickPath.AssemblyStartupPath + @"\" + NCASServer.DCU_UPGRADES_DIRECTORY_NAME + @"\" + fileName;
                        break;
                    case UpgradeType.CRUpgrade:
                        fullFilePath = QuickPath.AssemblyStartupPath + @"\" + NCASServer.CR_UPGRADES_DIRECTORY_NAME + @"\" + fileName;
                        break;
                }

                if (fullFilePath != string.Empty)
                {
                    fileStream = new FileStream(fullFilePath, FileMode.Create, FileAccess.Write, FileShare.None);
                    fileStream.Write(data, 0, data.Length);
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                retValue = false;
            }
            finally
            {
                if (fileStream != null)
                    try
                    {
                        fileStream.Close();
                        fileStream.Dispose();
                    }
                    catch { }
            }

            if (retValue
                && upgradeType == UpgradeType.CEUpgrade)
            {
                return NCASServer.Singleton.AddCeUpgradeFile(fullFilePath);
            }

            return retValue;
        }

        public bool RemoveCCUEvents(Guid guidCCU)
        {
            if (guidCCU == Guid.Empty)
                return false;

            return CCUConfigurationHandler.Singleton.RemoveCCUEvents(guidCCU);
        }

        public BlockFileInfo[] GetBlockFilesInfo(Guid guidCCU)
        {
            if (guidCCU == Guid.Empty)
                return null;

            return CCUConfigurationHandler.Singleton.GetBlockFilesInfo(guidCCU);
        }

        public bool ResetBlockFilesInfo(Guid guidCCU)
        {
            if (guidCCU == Guid.Empty)
                return false;

            return CCUConfigurationHandler.Singleton.ResetBlockFilesInfo(guidCCU);
        }

        public IList<ThreadInfo> GetThreadsInfo(Guid guidCCU)
        {
            if (guidCCU == Guid.Empty)
                return null;

            return CCUConfigurationHandler.Singleton.GetThreadsInfo(guidCCU);
        }

        public IList<VerbosityLevelInfo> GetVerbosityLevelsInfo(Guid guidCCU)
        {
            if (guidCCU == Guid.Empty)
                return null;

            return CCUConfigurationHandler.Singleton.GetVerbosityLevelsInfo(guidCCU);
        }

        public bool SetVerbosityLevel(Guid guidCCU, byte verbosityLevel)
        {
            if (guidCCU == Guid.Empty)
                return false;

            return CCUConfigurationHandler.Singleton.SetVerbosityLevel(guidCCU, verbosityLevel);
        }

        public byte GetCurrentVerbosity(Guid guidCCU)
        {
            if (guidCCU == Guid.Empty)
                return 128;

            return CCUConfigurationHandler.Singleton.GetVerbosityLevel(guidCCU);
        }

        #region ICgpNCASRemotingProvider Members


        public bool DeleteUpgradeFile(UpgradeType upgradeType, string version)
        {
            string fileName = string.Empty;
            string directoryPath = string.Empty;
            string HwType = string.Empty;

            switch (upgradeType)
            {
                case UpgradeType.CCUUpgrade:
                    directoryPath = QuickPath.AssemblyStartupPath + @"\" + NCASServer.CCU_UPGRADES_DIRECTORY_NAME;
                    break;
                case UpgradeType.CEUpgrade:
                    directoryPath = QuickPath.AssemblyStartupPath + @"\" + NCASServer.CE_UPGRADES_DIRECTORY_NAME;
                    fileName = string.Format("{0}\\{1}", directoryPath, version);
                    break;
                case UpgradeType.DCUUpgrade:
                    directoryPath = QuickPath.AssemblyStartupPath + @"\" + NCASServer.DCU_UPGRADES_DIRECTORY_NAME;
                    break;
                case UpgradeType.CRUpgrade:
                    directoryPath = QuickPath.AssemblyStartupPath + @"\" + NCASServer.CR_UPGRADES_DIRECTORY_NAME;
                    break;
            }

            if (upgradeType == UpgradeType.DCUUpgrade || upgradeType == UpgradeType.CRUpgrade)
            {
                string[] versionInfo = version.Split('(');
                version = versionInfo[0].Split(' ')[0];
                HwType = versionInfo[1].Substring(0, versionInfo[1].Length - 1);
            }

            if (upgradeType != UpgradeType.CEUpgrade)
            {
                foreach (string file in Directory.GetFiles(directoryPath))
                {
                    Exception ex;
                    string[] headerProperties = FilePacker.TryGetHeaderParameters(file, out ex);

                    if (headerProperties == null || headerProperties.Length == 0)
                        continue;

                    if (!string.IsNullOrEmpty(fileName))
                        break;

                    switch (headerProperties[0].ToLower())
                    {
                        case "ccu":
                            if (headerProperties.Length > 1 && headerProperties[1] == version)
                                fileName = file;
                            break;
                        case "dcu":
                            if (headerProperties.Length < 5)
                            {
                                try
                                {
                                    File.Delete(file);
                                }
                                catch (Exception)
                                {
                                }
                            }
                            if (headerProperties.Length > 4 
                                &&  ((DCUHWVersion)Int32.Parse(headerProperties[1])).ToString() == HwType 
                                && headerProperties[2] == version)
                                fileName = file;
                            break;
                        case "cr":
                            if (headerProperties.Length > 4
                                && headerProperties[1] == version
                                && ((CRHWVersion)int.Parse(headerProperties[2], System.Globalization.NumberStyles.HexNumber)).ToString() == HwType)
                                fileName = file;
                            break;
                    }
                }
            }

            if (string.IsNullOrEmpty(fileName))
                return false;

            try
            {
                File.Delete(fileName);
            }
            catch (Exception)
            {
                return false;
            }

            if (upgradeType == UpgradeType.CEUpgrade)
                NCASServer.Singleton.RemoveCeUpgradeFile(Path.GetFileName(fileName));

            return true;
        }

        #endregion

        public State GetRealOnOffObjectState(ObjectType type, Guid id)
        {
            try
            {
                switch (type)
                {
                    case ObjectType.Input:
                        InputState inputState = Inputs.GetActualStatesByGuid(id);
                        return inputState == InputState.Normal ? State.Off
                            : (inputState == InputState.Alarm ? State.On : State.Unknown);

                    case ObjectType.Output:
                        OutputState outputState = Outputs.GetActualStatesByGuid(id);
                        return outputState == OutputState.Off ? State.Off
                            : (outputState == OutputState.On ? State.On : State.Unknown);

                    case ObjectType.TimeZone:
                        byte stateTZ = CgpServerRemotingProvider.Singleton.GetTimeZoneActualStatus(id);
                        return stateTZ == 0 ? State.Off
                            : (stateTZ == 1 ? State.On : State.Unknown);

                    case ObjectType.DailyPlan:
                        byte stateDP = CgpServerRemotingProvider.Singleton.GetDailyPlanActualStatus(id);
                        return stateDP == 0 ? State.Off
                            : (stateDP == 1 ? State.On : State.Unknown);

                    default:
                        return State.Unknown;
                }
            }
            catch
            {
                return State.Unknown;
            }
        }

        protected override IEnumerable<string> GetSessionsForUser(string userIdentifier)
        {
            try
            {
                Guid userId = new Guid(userIdentifier);
                return RemotingAuthentication
                    .GetSessionsForUser(userId).Select(item => item.ToString());
            }
            catch (FormatException)
            {
                return new string[0];
            }
            catch (OverflowException)
            {
                return new string[0];
            }

        }

        public ICollection<ServerAlarmCore> GetAlarms(AlarmType alarmType, IdAndObjectType alarmObject)
        {
            return AlarmsManager.Singleton.GetAlarms(
                alarmType,
                alarmObject);
        }

        public ICollection<MemoryInfo> GetMemoryReport(Guid idCcu)
        {
            if (idCcu == Guid.Empty)
                return null;

            return CCUConfigurationHandler.Singleton.GetMemoryReport(idCcu);
        }

        public DataTable ExportDataLogs(IList<FilterSettings> filterSettings,  bool bCardReader, out bool bFillSection)

        {
            if (bCardReader)
                return ExportTableNCASFactory.Generate(ExportTableNCASFactory.Type._CardReader, filterSettings, out bFillSection);
            else
                return ExportTableNCASFactory.Generate(ExportTableNCASFactory.Type._AccessControlList, filterSettings, out bFillSection);
        }
    }
}
