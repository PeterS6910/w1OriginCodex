using System;
using System.Collections.Generic;
using System.Net;

using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Globals;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick.Remoting;
using Contal.IwQuick.Threads;

namespace Contal.Cgp.NCAS.Server
{
    internal static class CCUCallbackRunner
    {
        internal static void RunCreatedDCUEvent(ARemotingCallbackHandler remoteHandler)
        {
            var createdDcuHandler =
                remoteHandler as CreatedDCUHandler;

            if (createdDcuHandler != null)
                createdDcuHandler.RunEvent();
        }

        public static void RunAlarmAreaRequestActivationStateChanged(
            ARemotingCallbackHandler remoteHandler,
            object[] objAA)
        {
            if (objAA == null || objAA.Length != 3)
                return;

            var requestActivationStateChangedAlarmAreaHandler =
                remoteHandler as RequestActivationStateChangedAlarmAreaHandler;

            if (requestActivationStateChangedAlarmAreaHandler != null)
                requestActivationStateChangedAlarmAreaHandler.RunEvent(
                    (Guid)objAA[0],
                    (byte)objAA[1],
                    (bool)objAA[2]);
        }

        public static void RunAlarmAreaActivationStateChanged(
            ARemotingCallbackHandler remoteHandler,
            object[] objAA)
        {
            if (objAA == null || objAA.Length != 2)
                return;

            var activationStateChangedAlarmAreaHandler =
                remoteHandler as ActivationStateChangedAlarmAreaHandler;

            if (activationStateChangedAlarmAreaHandler != null)
                activationStateChangedAlarmAreaHandler.RunEvent(
                    (Guid)objAA[0],
                    (byte)objAA[1]);
        }

        public static void RunCEUpgradeProgressChanged(ARemotingCallbackHandler remoteHandler, object[] info)
        {
            if (info == null ||
                info.Length != 2 ||
                !(info[0] is IPAddress) ||
                !(info[1] is string))
                return;

            var ipAddress = (IPAddress)info[0];
            var state = info[1] as string;

            var ceUpgradeProgressChangedHandler =
                remoteHandler as CEUpgradeProgressChangedHandler;

            if (ceUpgradeProgressChangedHandler != null)
                ceUpgradeProgressChangedHandler.RunEvent(
                    ipAddress,
                    state);
        }

        public static void RunCCUUpgradeProgressChanged(
            ARemotingCallbackHandler remoteHandler,
            object[] info)
        {
            if (info == null ||
                info.Length != 2 ||
                !(info[0] is IPAddress) ||
                !(info[1] is int))
                return;

            var ipAddress = (IPAddress)info[0];
            var percents = (int)info[1];

            var ccuUpgradeProgressChangedHandler =
                remoteHandler as CCUUpgradeProgressChangedHandler;

            if (ccuUpgradeProgressChangedHandler != null)
                ccuUpgradeProgressChangedHandler.RunEvent(
                    ipAddress,
                    percents);
        }

        public static void RunCCUMakeLogDumpProgressChanged(
            ARemotingCallbackHandler remoteHandler,
            object[] info)
        {
            if (info == null ||
                info.Length != 2 ||
                !(info[0] is Guid) ||
                !(info[1] is int))
                return;

            var idCcu = (Guid)info[0];
            var percents = (int)info[1];

            var ccuMakeLogDumpProgressChangedHandler =
                remoteHandler as CCUMakeLogDumpProgressChangedHandler;

            if (ccuMakeLogDumpProgressChangedHandler != null)
                ccuMakeLogDumpProgressChangedHandler.RunEvent(
                    idCcu,
                    percents);
        }

        public static void RunCCUUpgradeFinished(ARemotingCallbackHandler remoteHandler, object[] info)
        {
            if (info == null ||
                info.Length != 2 ||
                !(info[0] is IPAddress) ||
                !(info[1] is bool))
                return;

            var ipAddress = (IPAddress)info[0];
            var success = (bool)info[1];

            var ccuUpgradeFinishedHandler =
                remoteHandler as CCUUpgradeFinishedHandler;

            if (ccuUpgradeFinishedHandler != null)
                ccuUpgradeFinishedHandler.RunEvent(ipAddress, success);
        }

        public static void RunCCUKillFailed(ARemotingCallbackHandler remoteHandler, object[] info)
        {
            if (info == null ||
                info.Length != 1 ||
                !(info[0] is IPAddress))
                return;

            var ipAddress = info[0] as IPAddress;

            var ccuKillFailedHandler =
                remoteHandler as CCUKillFailedHandler;

            if (ccuKillFailedHandler != null)
                ccuKillFailedHandler.RunEvent(ipAddress);
        }

        public static void RunCcuUpsValuesChanged(
            ARemotingCallbackHandler remoteHandler,
            object[] objUpsValues)
        {
            if (objUpsValues == null || objUpsValues.Length != 2)
                return;

            var ccuUpsMonitorValuesChangedHandler =
                remoteHandler as CCUUpsMonitorValuesChangedHandler;

            if (ccuUpsMonitorValuesChangedHandler != null)
                ccuUpsMonitorValuesChangedHandler.RunEvent(
                    (string)objUpsValues[0],
                    (CUps2750Values)objUpsValues[1]);
        }

        public static void RunAlarmAreaAlarmStateChanged(
            ARemotingCallbackHandler remoteHandler,
            object[] objAA)
        {
            if (objAA == null || objAA.Length != 2)
                return;

            var alarmStateChangedAlarmAreaHandler =
                remoteHandler as AlarmStateChangedAlarmAreaHandler;

            if (alarmStateChangedAlarmAreaHandler != null)
                alarmStateChangedAlarmAreaHandler.RunEvent(
                    (Guid)objAA[0],
                    (byte)objAA[1]);
        }

        public static void RunTimeBuyingMatrixStateChanged(
            ARemotingCallbackHandler remoteHandler,
            object[] objAA)
        {
            if (objAA == null || objAA.Length != 2)
                return;

            var timeBuyingMatrixStateChangedAlarmAreaHandler =
                remoteHandler as TimeBuyingMatrixStateChangedAlarmAreaHandler;

            if (timeBuyingMatrixStateChangedAlarmAreaHandler != null)
                timeBuyingMatrixStateChangedAlarmAreaHandler.RunEvent(
                    (Guid)objAA[0],
                    (TimeBuyingMatrixState)objAA[1]);
        }

        public static void RunAlarmAreaSabotageStateChanged(
            ARemotingCallbackHandler remoteHandler,
            object[] objAA)
        {
            if (objAA == null || objAA.Length != 2)
                return;

            var sabotageStateChangedAlarmAreaHandler =
                remoteHandler as SabotageStateChangedAlarmAreaHandler;

            if (sabotageStateChangedAlarmAreaHandler != null)
                sabotageStateChangedAlarmAreaHandler.RunEvent(
                    (Guid)objAA[0],
                    (byte)objAA[1]);
        }

        public static void RunAlarmAreaSensorBlockingTypeChanged(
            ARemotingCallbackHandler remoteHandler,
            object[] parameters)
        {
            if (parameters == null || parameters.Length != 3)
                return;

            var alarmAreaSensorBlockingTypeChangedHandler =
                remoteHandler as AlarmAreaSensorBlockingTypeChangedHandler;

            if (alarmAreaSensorBlockingTypeChangedHandler != null)
                alarmAreaSensorBlockingTypeChangedHandler.RunEvent(
                    (Guid)parameters[0],
                    (Guid)parameters[1],
                    (SensorBlockingType?)parameters[2]);
        }

        public static void RunAlarmAreaSensorStateChanged(
            ARemotingCallbackHandler remoteHandler,
            object[] parameters)
        {
            if (parameters == null || parameters.Length != 3)
                return;

            var alarmAreaSensorStateChangedHandler =
                remoteHandler as AlarmAreaSensorStateChangedHandler;

            if (alarmAreaSensorStateChangedHandler != null)
                alarmAreaSensorStateChangedHandler.RunEvent(
                    (Guid) parameters[0],
                    (Guid) parameters[1],
                    (State?) parameters[2]);
        }

        public static void RunCreatedCCUEvent(ARemotingCallbackHandler remoteHandler)
        {
            var createdCCUHandler = remoteHandler as CreatedCCUHandler;

            if (createdCCUHandler != null)
                createdCCUHandler.RunEvent();
        }

        public static void RunCcuUpsOnlineStateChanged(
            ARemotingCallbackHandler remoteHandler,
            object[] objUpsOnline)
        {
            if (objUpsOnline == null || objUpsOnline.Length != 2)
                return;

            var ccuUpsMonitorOnlineStateChangedHandler =
                remoteHandler as CCUUpsMonitorOnlineStateChangedHandler;

            if (ccuUpsMonitorOnlineStateChangedHandler != null)
                ccuUpsMonitorOnlineStateChangedHandler.RunEvent(
                    (string)objUpsOnline[0],
                    (byte)objUpsOnline[1]);
        }

        public static void RunCcuUpsAlarmStateChanged(ARemotingCallbackHandler remoteHandler, object[] objUpsAlarm)
        {
            if (objUpsAlarm == null || objUpsAlarm.Length != 2)
                return;

            var ccuUpsMonitorAlarmStateChangedHandler =
                remoteHandler as CCUUpsMonitorAlarmStateChangedHandler;

            if (ccuUpsMonitorAlarmStateChangedHandler != null)
                ccuUpsMonitorAlarmStateChangedHandler.RunEvent(
                    (string)objUpsAlarm[0],
                    (bool)objUpsAlarm[1]);
        }

        public static void RunDcuInputsSabotageStateChanged(ARemotingCallbackHandler remoteHandler,
            object[] objDcu)
        {
            var dcuInputsSabotageStateChangedHandler =
                remoteHandler as DcuInputsSabotageStateChangedHandler;

            if (dcuInputsSabotageStateChangedHandler != null)
                dcuInputsSabotageStateChangedHandler.RunEvent(
                    (Guid)objDcu[0],
                    (State)objDcu[1]);
        }

        public static void RunDCUsUpgradeProgressChanged(
            ARemotingCallbackHandler remoteHandler,
            object[] objDcus)
        {
            if (objDcus == null || objDcus.Length != 1)
                return;

            var dcuUpgradeState = objDcus[0] as DCUUpgradeState;

            if (dcuUpgradeState == null)
                return;

            var dcuUpgradeProgressChangedHandler =
                remoteHandler as DCUUpgradeProgressChangedHandler;

            if (dcuUpgradeProgressChangedHandler != null)
                dcuUpgradeProgressChangedHandler.RunEvent(dcuUpgradeState);
        }

        public static void RunCRsUpgradeProgressChanged(
            ARemotingCallbackHandler remoteHandler,
            object[] objCRs)
        {
            if (objCRs == null || objCRs.Length != 1)
                return;

            var crUpgradeState = objCRs[0] as CRUpgradeState;
            if (crUpgradeState == null)
                return;

            var upgradeState = crUpgradeState;

            var crUpgradeProgressChangedHandler =
                remoteHandler as CRUpgradeProgressChangedHandler;

            if (crUpgradeProgressChangedHandler != null)
                crUpgradeProgressChangedHandler.RunEvent(upgradeState);
        }

        public static void RunCardReaderCommandChanged(
            ARemotingCallbackHandler remoteHandler,
            object[] objCardReader)
        {
            if (objCardReader == null || objCardReader.Length != 2)
                return;

            var commandChangedCardReaderHandler =
                remoteHandler as CommandChangedCardReaderHandler;

            if (commandChangedCardReaderHandler != null)
                commandChangedCardReaderHandler.RunEvent(
                    (Guid)objCardReader[0],
                    (byte)objCardReader[1]);
        }

        public static void RunCardReaderBlockedStatusChanged(
            ARemotingCallbackHandler remoteHandler,
            object[] objCardReader)
        {
            if (objCardReader == null || objCardReader.Length != 2)
                return;

            var blockedStateChangedCardReaderHandler =
                remoteHandler as BlockedStateChangedCardReaderHandler;

            if (blockedStateChangedCardReaderHandler != null)
                blockedStateChangedCardReaderHandler.RunEvent(
                    (Guid)objCardReader[0],
                    (bool)objCardReader[1]);
        }

        public static void RunAlarmAreaRequestActivationStateChanged(
            CCUConfigurationHandler.ActivationStateChangedCarrier item)
        {
            if (item.Data != null)
            {
                if (item.Data.Length != 3)
                return;

                NCASServerRemotingProvider.Singleton.ForeachCallbackHandler(
                    RunAlarmAreaRequestActivationStateChanged,
                    DelegateSequenceBlockingMode.OverallBlocking,
                    false,
                    typeof(RequestActivationStateChangedAlarmAreaHandler),
                    item.Data[0],
                    item.Data[1],
                    item.Data[2]);
            }
            else if (item.MethodToInvoke != null)
            {
                item.MethodToInvoke();
            }
        }

        public static void RunAlarmAreaBoughtTimeChanged(
            ARemotingCallbackHandler remoteHandler,
            object[] objData)
        {
            if (objData == null 
                || objData.Length < 4)
                return;

            var alarmAreaBoughtTimeChangedHandler =
                remoteHandler as AlarmAreaTimeBuyingHandler;

            if (alarmAreaBoughtTimeChangedHandler != null)
                alarmAreaBoughtTimeChangedHandler.RunBoughtTimeChanged(
                    (Guid)objData[0],
                    (string)objData[1],
                    (int)objData[2],
                    (int)objData[3]);
        }

        public static void RunAlarmAreaBoughtTimeExpired(
            ARemotingCallbackHandler remoteHandler,
            object[] objData)
        {
            if (objData == null
                || objData.Length < 3)
                return;

            var alarmAreaBoughtTimeChangedHandler =
                remoteHandler as AlarmAreaTimeBuyingHandler;

            if (alarmAreaBoughtTimeChangedHandler != null)
                alarmAreaBoughtTimeChangedHandler.RunBoughtTimeExpired(
                    (Guid)objData[0],
                    (int)objData[1],
                    (int)objData[2]);
        }

        public static void RunAlarmAreaTimeBuyingFailed(
            ARemotingCallbackHandler remoteHandler,
            object[] objData)
        {
            if (objData == null
                || objData.Length < 4)
                return;

            var alarmAreaBoughtTimeChangedHandler =
                remoteHandler as AlarmAreaTimeBuyingHandler;

            if (alarmAreaBoughtTimeChangedHandler != null)
                alarmAreaBoughtTimeChangedHandler.RunTimeBuyingFailed(
                    (Guid)objData[0],
                    (byte)objData[1],
                    (int)objData[2],
                    (int)objData[3]);
        }

        public static void RunOutputStateChanged(ARemotingCallbackHandler remoteHandler, object[] objOutput)
        {
            if (objOutput == null || objOutput.Length != 3)
                return;

            var stateChangedOutputHandler =
                remoteHandler as StateChangedOutputHandler;

            if (stateChangedOutputHandler != null)
                stateChangedOutputHandler.RunEvent(
                    (Guid)objOutput[0],
                    (byte)objOutput[1],
                    (Guid)objOutput[2]);
        }

        public static void RunOutputRealStateChanged(
            ARemotingCallbackHandler remoteHandler,
            object[] objOutput)
        {
            if (objOutput == null || objOutput.Length != 3)
                return;

            var realStateChangedOutputHandler =
                remoteHandler as RealStateChangedOutputHandler;

            if (realStateChangedOutputHandler != null)
                realStateChangedOutputHandler.RunEvent(
                    (Guid)objOutput[0],
                    (byte)objOutput[1],
                    (Guid)objOutput[2]);
        }

        public static void RunDoorEnvironmentStateChanged(
            ARemotingCallbackHandler remoteHandler,
            object[] objDoorEnv)
        {
            if (objDoorEnv == null || objDoorEnv.Length != 2)
                return;

            var doorEnvironmentStateChangedHandler =
                remoteHandler as DoorEnvironmentStateChangedHandler;

            if (doorEnvironmentStateChangedHandler != null)
                doorEnvironmentStateChangedHandler.RunEvent(
                    (Guid)objDoorEnv[0],
                    (byte)objDoorEnv[1]);
        }

        public static void RunCardReaderChanged(ARemotingCallbackHandler remoteHandler, object[] obj)
        {
            if (obj == null || obj.Length != 3)
                return;

            var stateChangedCardReaderHandler =
                remoteHandler as StateChangedCardReaderHandler;

            if (stateChangedCardReaderHandler != null)
                stateChangedCardReaderHandler.RunEvent(
                    (Guid)obj[0],
                    (byte)obj[1],
                    (Guid)obj[2]);
        }

        public static void RunCardReaderTamperStateChanged(ARemotingCallbackHandler remoteHandler, object[] obj)
        {
            if (obj == null || obj.Length != 3)
                return;

            var tamperStateChangedCardReaderHandler =
                remoteHandler as TamperChangedCardReaderHandler;

            if (tamperStateChangedCardReaderHandler != null)
                tamperStateChangedCardReaderHandler.RunEvent(
                    (Guid)obj[0],
                    (bool)obj[1],
                    (Guid)obj[2]);
        }

        public static void RunInputChanged(
            ARemotingCallbackHandler remoteHandler,
            object[] objInputState)
        {
            if (objInputState == null || objInputState.Length != 3)
                return;

            var stateChangedInputHandler =
                remoteHandler as StateChangedInputHandler;

            if (stateChangedInputHandler != null)
                stateChangedInputHandler.RunEvent(
                    (Guid)objInputState[0],
                    (byte)objInputState[1],
                    (Guid)objInputState[2]);
        }

        public static void RunDCUPhysicalAddressChanged(
            ARemotingCallbackHandler remoteHandler,
            object[] dcuObj)
        {
            if (dcuObj == null || dcuObj.Length != 2)
                return;

            var dcuPhysicalAddressChangedHandler =
                remoteHandler as DCUPhysicalAddressChangedHandler;

            if (dcuPhysicalAddressChangedHandler != null)
                dcuPhysicalAddressChangedHandler.RunEvent(
                    (Guid)dcuObj[0],
                    (string)dcuObj[1]);
        }

        public static void RunDcuMemoryWarning(
            ARemotingCallbackHandler remoteHandler,
            object[] dcuObj)
        {
            if (dcuObj == null || dcuObj.Length != 2)
                return;

            var dcuMemoryWarningChangedHandler =
                remoteHandler as DCUMemoryWarningChangedHandler;

            if (dcuMemoryWarningChangedHandler != null)
                dcuMemoryWarningChangedHandler.RunEvent(
                    (Guid)dcuObj[0],
                    (byte)dcuObj[1]);
        }

        public static void RunCCULookupFinished(ARemotingCallbackHandler remoteHandler, object[] lookupObjects)
        {
            var lookupList = lookupObjects[0] as ICollection<LookupedCcu>;
            if (lookupList == null)
                return;

            var lookupHashSet = lookupObjects[1] as ICollection<Guid>;
            if (lookupHashSet == null)
                return;

            var ccuLookupFinishedHandler = remoteHandler as CCULookupFinishedHandler;

            if (ccuLookupFinishedHandler != null)
                ccuLookupFinishedHandler.RunEvent(
                    lookupList,
                    lookupHashSet);
        }

        public static void RunAlarmTransmittersLookupFinished(ARemotingCallbackHandler remoteHandler, object[] lookupObjects)
        {
            var lookupedAlarmTransmitters = lookupObjects[0] as ICollection<LookupedAlarmTransmitter>;
            if (lookupedAlarmTransmitters == null)
                return;

            var lookupingClients = lookupObjects[1] as ICollection<Guid>;
            if (lookupingClients == null)
                return;

            var ccuLookupFinishedHandler = remoteHandler as AlarmTransmittersLookupFinishedHandler;

            if (ccuLookupFinishedHandler != null)
                ccuLookupFinishedHandler.RunEvent(
                    lookupedAlarmTransmitters,
                    lookupingClients);
        }

        public static void RunLprCameraLookupFinished(ARemotingCallbackHandler remoteHandler, object[] lookupObjects)
        {
            var lookupedCameras = lookupObjects[0] as ICollection<LookupedLprCamera>;
            if (lookupedCameras == null)
                return;

            var lookupingClients = lookupObjects[1] as ICollection<Guid>;
            if (lookupingClients == null)
                return;

            var lookupFinishedHandler = remoteHandler as LprCameraLookupFinishedHandler;

            lookupFinishedHandler?.RunEvent(lookupedCameras, lookupingClients);
        }


        public static void RunCcuMACAddressChanged(
            ARemotingCallbackHandler remoteHandler,
            object[] info)
        {
            var macAddressChangedCCUHandler =
                remoteHandler as MACAddressChangedCCUHandler;

            if (macAddressChangedCCUHandler != null)
                macAddressChangedCCUHandler.RunEvent(
                    (Guid)info[0],
                    (string)info[1]);
        }

        public static void RunDCUOnlineStateChanged(ARemotingCallbackHandler remoteHandler, object[] objDcu)
        {
            if (objDcu == null || objDcu.Length != 3)
                return;

            var dcuGuid = (Guid)objDcu[0];
            var dcuOnlineState = (OnlineState)objDcu[1];
            var parentGuid = (Guid)objDcu[2];

            var dcuOnlineStateChangedHandler =
                remoteHandler as DCUOnlineStateChangedHandler;

            if (dcuOnlineStateChangedHandler != null)
                dcuOnlineStateChangedHandler.RunEvent(
                    dcuGuid,
                    dcuOnlineState,
                    parentGuid);
        }
    }
}
