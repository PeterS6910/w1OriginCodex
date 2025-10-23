using System;
using System.Collections.Generic;

using Contal.Cgp.Client.Common;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.RemotingCommon;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.IwQuick.Localization;

namespace Contal.Cgp.NCAS.Client.Common
{
    public class NCASClientCore
    {
        private readonly ICgpClientBase _cgpClientBase;

        private readonly CardReaderCommunication _cardReaderCommunication;

        public NCASClientCore(
            ICgpClientBase cgpClientBase, 
            LocalizationHelper localizationHelper)
        {
            _cgpClientBase = cgpClientBase;
            _localizationHelper = localizationHelper;
            _cardReaderCommunication = new CardReaderCommunication(cgpClientBase);
        }

        public Type GetRemotingProviderInterfaceType()
        {
            return typeof(ICgpNCASRemotingProvider);
        }

        private readonly LocalizationHelper _localizationHelper;

        public IList<AlarmType> GetPluginAlarmTypes()
        {
            IList<AlarmType> pluginAlarmTypes = new List<AlarmType>();

            pluginAlarmTypes.Add(AlarmType.CCU_Offline);
            pluginAlarmTypes.Add(AlarmType.CCU_Unconfigured);
            pluginAlarmTypes.Add(AlarmType.CCU_TamperSabotage);
            pluginAlarmTypes.Add(AlarmType.CCU_ClockUnsynchronized);
            pluginAlarmTypes.Add(AlarmType.CCU_OutdatedFirmware);
            pluginAlarmTypes.Add(AlarmType.DCU_Offline);
            pluginAlarmTypes.Add(AlarmType.DCU_Offline_Due_CCU_Offline);
            pluginAlarmTypes.Add(AlarmType.DCU_TamperSabotage);
            pluginAlarmTypes.Add(AlarmType.DCU_WaitingForUpgrade);
            pluginAlarmTypes.Add(AlarmType.DCU_OutdatedFirmware);
            pluginAlarmTypes.Add(AlarmType.Input_Alarm);
            pluginAlarmTypes.Add(AlarmType.Input_Tamper);
            pluginAlarmTypes.Add(AlarmType.Output_Alarm);
            pluginAlarmTypes.Add(AlarmType.DoorEnvironment_Intrusion);
            pluginAlarmTypes.Add(AlarmType.DoorEnvironment_DoorAjar);
            pluginAlarmTypes.Add(AlarmType.DoorEnvironment_Sabotage);
            pluginAlarmTypes.Add(AlarmType.AlarmArea_Alarm);
            pluginAlarmTypes.Add(AlarmType.AlarmArea_AAlarm);
            pluginAlarmTypes.Add(AlarmType.CardReader_Offline);
            pluginAlarmTypes.Add(AlarmType.CardReader_Offline_Due_CCU_Offline);
            pluginAlarmTypes.Add(AlarmType.CardReader_TamperSabotage);
            pluginAlarmTypes.Add(AlarmType.CardReader_AccessDenied);
            pluginAlarmTypes.Add(AlarmType.CardReader_UnknownCard);
            pluginAlarmTypes.Add(AlarmType.CardReader_CardBlockedOrInactive);
            pluginAlarmTypes.Add(AlarmType.CardReader_InvalidPIN);
            pluginAlarmTypes.Add(AlarmType.CardReader_InvalidCode);
            pluginAlarmTypes.Add(AlarmType.CardReader_AccessPermitted);
            pluginAlarmTypes.Add(AlarmType.CardReader_InvalidEmergencyCode);
            pluginAlarmTypes.Add(AlarmType.CCU_PrimaryPowerMissing);
            pluginAlarmTypes.Add(AlarmType.CCU_BatteryLow);
            pluginAlarmTypes.Add(AlarmType.Ccu_Ups_OutputFuse);
            pluginAlarmTypes.Add(AlarmType.Ccu_Ups_BatteryFault);
            pluginAlarmTypes.Add(AlarmType.Ccu_Ups_BatteryFuse);
            pluginAlarmTypes.Add(AlarmType.Ccu_Ups_Overtemperature);
            pluginAlarmTypes.Add(AlarmType.Ccu_Ups_TamperSabotage);
            pluginAlarmTypes.Add(AlarmType.CCU_ExtFuse);
            pluginAlarmTypes.Add(AlarmType.CCU_DataChannelDistrupted);
            pluginAlarmTypes.Add(AlarmType.CCU_CoprocessorFailure);
            pluginAlarmTypes.Add(AlarmType.ICCU_SendingOfObjectStateFailed);
            pluginAlarmTypes.Add(AlarmType.ICCU_PortAlreadyUsed);
            pluginAlarmTypes.Add(AlarmType.CCU_HighMemoryLoad);
            pluginAlarmTypes.Add(AlarmType.CCU_FilesystemProblem);
            pluginAlarmTypes.Add(AlarmType.CCU_SdCardNotFound);
            pluginAlarmTypes.Add(AlarmType.Ccu_CatUnreachable);
            pluginAlarmTypes.Add(AlarmType.Ccu_TransferToArcTimedOut);
            pluginAlarmTypes.Add(AlarmType.AlarmArea_SetByOnOffObjectFailed);
            pluginAlarmTypes.Add(AlarmType.Sensor_Alarm);
            pluginAlarmTypes.Add(AlarmType.Sensor_Tamper_Alarm);

            return pluginAlarmTypes;
        }

        private ICgpNCASRemotingProvider _remotingProviderInterface;

        public ICgpNCASRemotingProvider MainServerProvider
        {
            get { return _remotingProviderInterface; }
        }

        public void PreRegisterAttachCallbackHandlers()
        {
            if (_remotingProviderInterface != null)
            {
                _remotingProviderInterface.AttachCallbackHandler(StatusChangedSecurityDailyPlanHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(StatusChangedSecurityTimeZoneHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(StateChangedInputHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(StateChangedOutputHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(RealStateChangedOutputHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(StateChangedCCUHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(ConfiguredChangedCCUHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(MACAddressChangedCCUHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(CreatedCCUHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(AlarmStateChangedAlarmAreaHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(ActivationStateChangedAlarmAreaHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(RequestActivationStateChangedAlarmAreaHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(SabotageStateChangedAlarmAreaHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(DoorEnvironmentStateChangedHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(StateChangedCardReaderHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(TamperChangedCardReaderHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(CommandChangedCardReaderHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(BlockedStateChangedCardReaderHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(IPSettingsChangedHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(DCUOnlineStateChangedHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(DCUPhysicalAddressChangedHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(DcuInputsSabotageStateChangedHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(CreatedDCUHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(DCUUpgradeProgressChangedHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(DCUMemoryWarningChangedHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(InputEditChangedHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(OutputEditChangedHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(TFTPFileTransferProgressChangedHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(CEUpgradeProgressChangedHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(CCUUpgradeProgressChangedHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(CCUMakeLogDumpProgressChangedHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(CCUUpgradeFinishedHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(CRUpgradeProgressChangedHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(CCULookupFinishedHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(AlarmTransmittersLookupFinishedHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(CCUUpsMonitorValuesChangedHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(CCUUpsMonitorOnlineStateChangedHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(CCUUpsMonitorAlarmStateChangedHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(CCUKillFailedHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(AlarmAreaTimeBuyingHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(AlarmTransmitterOnlineStateChangedHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(AlarmAreaSensorBlockingTypeChangedHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(AlarmAreaSensorStateChangedHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(TimetecCommunicationOnlineStateChangedHandler.Singleton);
            }
        }

        public void SetRemotingProviderInterface(object remotingProviderInterface)
        {
            if (remotingProviderInterface == null)
                _remotingProviderInterface = null;
            else if (remotingProviderInterface is ICgpNCASRemotingProvider)
            {
                _remotingProviderInterface = (remotingProviderInterface as ICgpNCASRemotingProvider);
                _remotingProviderInterface.AttachCallbackHandler(StatusChangedSecurityDailyPlanHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(StatusChangedSecurityTimeZoneHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(StateChangedInputHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(StateChangedOutputHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(RealStateChangedOutputHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(StateChangedCCUHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(ConfiguredChangedCCUHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(MACAddressChangedCCUHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(CreatedCCUHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(AlarmStateChangedAlarmAreaHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(TimeBuyingMatrixStateChangedAlarmAreaHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(ActivationStateChangedAlarmAreaHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(RequestActivationStateChangedAlarmAreaHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(SabotageStateChangedAlarmAreaHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(DoorEnvironmentStateChangedHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(StateChangedCardReaderHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(TamperChangedCardReaderHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(CommandChangedCardReaderHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(BlockedStateChangedCardReaderHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(IPSettingsChangedHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(DCUOnlineStateChangedHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(DCUPhysicalAddressChangedHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(DcuInputsSabotageStateChangedHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(CreatedDCUHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(DCUUpgradeProgressChangedHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(DCUMemoryWarningChangedHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(InputEditChangedHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(OutputEditChangedHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(TFTPFileTransferProgressChangedHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(CEUpgradeProgressChangedHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(CCUUpgradeProgressChangedHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(CCUUpgradeFinishedHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(CRUpgradeProgressChangedHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(CCULookupFinishedHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(AlarmTransmittersLookupFinishedHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(CCUUpsMonitorValuesChangedHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(CCUUpsMonitorOnlineStateChangedHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(CCUUpsMonitorAlarmStateChangedHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(CCUKillFailedHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(AlarmAreaTimeBuyingHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(AlarmTransmitterOnlineStateChangedHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(AlarmAreaSensorBlockingTypeChangedHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(AlarmAreaSensorStateChangedHandler.Singleton);
                _remotingProviderInterface.AttachCallbackHandler(TimetecCommunicationOnlineStateChangedHandler.Singleton);
            }
        }

        public ICollection<IModifyObject> GetIModifyObjects(ObjectType objectType)
        {
            if (MainServerProvider == null)
                return null;

            Exception error;

            switch (objectType)
            {
                case ObjectType.CCU:
                    return MainServerProvider.CCUs.ListModifyObjects(out error);

                case ObjectType.DCU:
                    return MainServerProvider.DCUs.ListModifyObjects(out error);

                case ObjectType.AlarmArea:
                    return MainServerProvider.AlarmAreas.ListModifyObjects(out error);

                case ObjectType.CardReader:
                    return MainServerProvider.CardReaders.ListModifyObjects(true, out error);

                case ObjectType.Input:
                    return MainServerProvider.Inputs.ListModifyObjects(out error);

                case ObjectType.Output:
                    return MainServerProvider.Outputs.ListModifyObjects(out error);

                case ObjectType.SecurityDailyPlan:
                    return MainServerProvider.SecurityDailyPlans.ListModifyObjects(out error);

                case ObjectType.SecurityTimeZone:
                    return MainServerProvider.SecurityTimeZones.ListModifyObjects(out error);

                case ObjectType.DoorEnvironment:
                    return MainServerProvider.DoorEnvironments.ListModifyObjects(out error);

                case ObjectType.AccessControlList:
                    return MainServerProvider.AccessControlLists.ListModifyObjects(out error);

                case ObjectType.Scene:
                    return MainServerProvider.Scenes.ListModifyObjects(out error);
            }

            return null;
        }

        public void RestartCardReaderCommunication()
        {
            _cardReaderCommunication.RunCardReaderCommunication();
        }

        public void SendCardReaderCommand(CardReaderSceneType crCommanad)
        {
            _cardReaderCommunication.ShowCommandCardReader(crCommanad);
        }

        public void SetLanguage(string language)
        {
            _localizationHelper.SetLanguage(language);
        }

        public string GetTranslateString(string name, params object[] args)
        {
            return _localizationHelper.GetString(name, args);
        }
    }
}
