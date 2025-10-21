using System;
using System.Collections.Generic;

using Contal.Cgp.BaseLib;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Definitions;

namespace Contal.Cgp.NCAS.RemotingCommon
{
    public enum ConfigureResult
    {
        OK,
        AlreadyRunning,
        GeneralFailure,
        LicenceFailure
    }

    public interface ICCUs : IBaseOrmTable<CCU>
    {
        CCUOnlineState GetCCUState(Guid id);
        bool Unconfigure(Guid id);
        ConfigureResult ConfigureForThisServer(Guid ccuGuid);
        ConfigureResult ForceReconfiguration(Guid ccuGuid);
        CCUConfigurationState GetCCUConfiguredState(Guid id);
        CCUConfigurationState GetActualCCUConfiguredState(Guid id);
        bool IsCCU0(Guid id);
        bool IsCCUUpgrader(Guid id);
        MainBoardVariant GetCCUMainBoardType(Guid id);
        IList<DoorEnvironment> GetDoorEnvironments(CCU ccu);
        void DoCCUsLookUp(Guid clientID);
        IPSetting GetIpSettings(Guid ccuGuid);
        string SetIpSettings(Guid ccuGuid, IPSetting ipSetting);
        string GetFirmwareVersion(Guid giudCCU);
        DateTime? GetCurrentCCUTime(Guid guid);

        bool IsCat12Combo(Guid ccuGuid);
        bool HasCat12ComboLicence(Guid idCCU);
        int GetFreeCat12ComboLicenceCount();

        ICollection<CCUShort> ShortSelectByCriteria(IList<FilterSettings> filterSettings, out Exception error);

        IList<CcuListObj> GetListObj(out Exception error);

        bool StopUpgradeMode(Guid ccuGuid, bool upgraded);

        IList<IModifyObject> ListModifyObjects(out Exception error);

        bool ResetCCU(Guid ccuGuid);
        bool SoftResetCCU(Guid ccuGuid);

        int[] CommunicationStatistic(Guid ccuGuid);
        void ResetServerSended(Guid ccuGuid);
        void ResetServerReceived(Guid ccuGuid);
        void ResetServerDeserializeError(Guid ccuGuid);
        void ResetServerReceivedError(Guid ccuGuid);
        void ResetCcuSended(Guid ccuGuid);
        void ResetCcuReceived(Guid ccuGuid);
        void ResetCcuDeserializeError(Guid ccuGuid);
        void ResetCcuReceivedError(Guid ccuGuid);
        void ResetCommunicationStatistic(Guid ccuGuid);
        void ResetServerMsgRetry(Guid ccuGuid);
        void ResetCcuMsgRetry(Guid ccuGuid);

        object[] GetCcuStartsCount(Guid ccuGuid);
        void ResetCcuStartCounter(Guid ccuGuid);
        void RequestDcuMemoryLoad(Guid ccuGuid, byte logicalAddress);
        string WinCEImageVersion(Guid ccuGuid);
        CcuConfigurationOptions EnableConfigureThisCcu(Guid ccuGuid);
        bool ValidConfigurePassword(Guid ccuGuid, string password);
        bool HasCcuConfigurationPassword(Guid ccuGuid);
        void NewConfigurePassword(Guid ccuGuid, string password);
        bool IsActualWinCeImage(Guid ccuGuid, string winCeFile);
        bool HasAccessToChangeCcuPassword();
        string[] CoprocessorBuildNumberStatistics(Guid ccuGuid);

        IList<CardReader> ActualCcuCardReaders(Guid ccuGuid);
        
        string ResultSimulationCardSwiped(
            Guid idCcu,
            ObjectType objectType,
            Guid idObject,
            string cardNumber,
            string pin,
            int pinLength);
        
        State GetTimeZoneDailyPlanState(Guid idCCU, ObjectType objectType, Guid objectGuid);

        void SendUpsMonitorData(Guid idCcu);
        void StopSendUpsMonitorData(Guid idCcu);
        bool GetOtherCCUStatistics(Guid idCcu,
            out int threadsCount,
            out int flashFreeSpace, out int flashSize,
            out bool sdCardPresent, out int sdCardFreeSpace, out int sdCardSize,
            out int freeMemory, out int totalMemory, out int memoryLoad);

        IList<DcuTestRoutineDataGridObj> GetDcuTestStates(Guid guidCcu);
        void SetDcuTest(Guid guidCcu, object[] dcuTest);
        bool SetTimeManually(Guid guidCCU, DateTime utcDateTime);
        void ResetCCUCommandTimeouts(Guid guidCCU);
        bool ShowDeleteEvents();
        bool RunGcCollect(Guid idCcu);
        bool PingIpAddress(string ipAddress, int count);

        bool RenameObjects(Guid idCcu, string newCcuName, ICollection<IModifyObject> objectsToRename);
        ICollection<IModifyObject> GetObjectsToRename(Guid idCcu);
        int GetNewIndex();
        bool MakeLogDump(Guid idCcu);
        byte[] GetDebugFilesFromCcu(string ipAddress);
        bool StopCKM(Guid idCcu);
        bool StartCKM(Guid idCcu, bool isImplicity);
        bool RunProccess(Guid idCcu, string cmd);
    }
}
