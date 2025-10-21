using System;
using System.Collections.Generic;
using System.Linq;

using Contal.Cgp.NCAS.Server.Alarms;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.NCAS.Server.CcuDataReplicator;
using Contal.Cgp.Server.DB;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Globals;
using Contal.IwQuick.Sys;

namespace Contal.Cgp.NCAS.Server.DB
{
    public sealed class DevicesAlarmSettings :
        ANcasBaseOrmTable<DevicesAlarmSettings, DevicesAlarmSetting>
    {
        private DevicesAlarmSettings()
            : base(
                  null,
                  new CudPreparationForObjectWithVersion<DevicesAlarmSetting>())
        {
        }

        public override bool HasAccessView(Login login)
        {
            return AccessChecker.HasAccessControl(NCASAccess.GetAccessesForGroup(AccessNcasGroups.ALARM_SETTINGS), login);
        }

        public override bool HasAccessInsert(Login login)
        {
            return true;
        }

        public override bool HasAccessUpdate(Login login)
        {
            return AccessChecker.HasAccessControl(NCASAccess.GetAccessesForGroup(AccessNcasGroups.ALARM_SETTINGS), login);
        }

        public override bool HasAccessDelete(Login login)
        {
            return true;
        }

        public override void CUDSpecial(DevicesAlarmSetting devicesAlarmSetting, ObjectDatabaseAction objectDatabaseAction)
        {
            if (objectDatabaseAction == ObjectDatabaseAction.Delete)
            {
                DataReplicationManager.Singleton.DeleteObjectFroCcus(
                    new IdAndObjectType(
                        devicesAlarmSetting.GetId(),
                        devicesAlarmSetting.GetObjectType()));
            }
            else if (devicesAlarmSetting != null)
            {
                DataReplicationManager.Singleton.SendModifiedObjectToCcus(devicesAlarmSetting);
            }
        }

        public override void AfterInsert(DevicesAlarmSetting newDevicesAlarmSetting)
        {
            if (newDevicesAlarmSetting == null)
            {
                return;
            }

            CatAlarmsManager.Singleton.ConfigureGeneralAlarmArcs(
                (newDevicesAlarmSetting.DevicesAlarmSettingAlarmArcs ?? Enumerable.Empty<DevicesAlarmSettingAlarmArc>())
                    .Cast<IAlarmArcForAlarmType>());
        }

        public override void AfterUpdate(DevicesAlarmSetting newDevicesAlarmSetting,
            DevicesAlarmSetting oldDevicesAlarmSettingBeforeUpdate)
        {
            if (newDevicesAlarmSetting == null)
            {
                return;
            }

            CCUAlarms.Singleton.ChangeSettingsForBlockAlarmCCUClockUnsynchronized();
            CCUAlarms.Singleton.ChangeSettingsForBlockAlarmCCUUnconfigured();
            CCUAlarms.Singleton.ChangeSettingsForBlockAlarmCCUOffline();
            CCUAlarms.Singleton.ChangeSettingsForBlockAlarmDCUOffline();
            CCUAlarms.Singleton.ChangeSettingsForBlockAlarmCROffline();

            CatAlarmsManager.Singleton.ConfigureGeneralAlarmArcs(
                (newDevicesAlarmSetting.DevicesAlarmSettingAlarmArcs ?? Enumerable.Empty<DevicesAlarmSettingAlarmArc>())
                    .Cast<IAlarmArcForAlarmType>());

            if (oldDevicesAlarmSettingBeforeUpdate == null)
            {
                return;
            }

            if (newDevicesAlarmSetting.AlarmCCUOfflinePresentationGroup != null &&
                !newDevicesAlarmSetting.AlarmCCUOfflinePresentationGroup.Compare(
                    oldDevicesAlarmSettingBeforeUpdate.AlarmCCUOfflinePresentationGroup))
            {
                NCASServer.Singleton.PresentationGroupChanged(AlarmType.CCU_Offline);
            }

            if (newDevicesAlarmSetting.AlarmCCUUnconfiguredPresentationGroup != null &&
                !newDevicesAlarmSetting.AlarmCCUUnconfiguredPresentationGroup.Compare(
                    oldDevicesAlarmSettingBeforeUpdate.AlarmCCUUnconfiguredPresentationGroup))
            {
                NCASServer.Singleton.PresentationGroupChanged(AlarmType.CCU_Unconfigured);
            }

            if (newDevicesAlarmSetting.AlarmCCUClockUnsynchronizedPresentationGroup != null &&
                !newDevicesAlarmSetting.AlarmCCUClockUnsynchronizedPresentationGroup.Compare(
                    oldDevicesAlarmSettingBeforeUpdate.AlarmCCUClockUnsynchronizedPresentationGroup))
            {
                NCASServer.Singleton.PresentationGroupChanged(AlarmType.CCU_ClockUnsynchronized);
            }

            if (newDevicesAlarmSetting.AlarmCCUTamperSabotagePresentationGroup != null &&
                !newDevicesAlarmSetting.AlarmCCUTamperSabotagePresentationGroup.Compare(
                    oldDevicesAlarmSettingBeforeUpdate.AlarmCCUTamperSabotagePresentationGroup))
            {
                NCASServer.Singleton.PresentationGroupChanged(AlarmType.CCU_TamperSabotage);
            }

            if (newDevicesAlarmSetting.AlarmCCUPrimaryPowerMissingPresentationGroup != null &&
                !newDevicesAlarmSetting.AlarmCCUPrimaryPowerMissingPresentationGroup.Compare(
                    oldDevicesAlarmSettingBeforeUpdate.AlarmCCUPrimaryPowerMissingPresentationGroup))
            {
                NCASServer.Singleton.PresentationGroupChanged(AlarmType.CCU_PrimaryPowerMissing);
            }

            if (newDevicesAlarmSetting.AlarmCCUBatteryIsLowPresentationGroup != null &&
                !newDevicesAlarmSetting.AlarmCCUBatteryIsLowPresentationGroup.Compare(
                    oldDevicesAlarmSettingBeforeUpdate.AlarmCCUBatteryIsLowPresentationGroup))
            {
                NCASServer.Singleton.PresentationGroupChanged(AlarmType.CCU_BatteryLow);
            }

            if (newDevicesAlarmSetting.AlarmCcuUpsOutputFusePresentationGroup != null &&
                !newDevicesAlarmSetting.AlarmCcuUpsOutputFusePresentationGroup.Compare(
                    oldDevicesAlarmSettingBeforeUpdate.AlarmCcuUpsOutputFusePresentationGroup))
            {
                NCASServer.Singleton.PresentationGroupChanged(AlarmType.Ccu_Ups_OutputFuse);
            }

            if (newDevicesAlarmSetting.AlarmCcuUpsBatteryFaultPresentationGroup != null &&
                !newDevicesAlarmSetting.AlarmCcuUpsBatteryFaultPresentationGroup.Compare(
                    oldDevicesAlarmSettingBeforeUpdate.AlarmCcuUpsBatteryFaultPresentationGroup))
            {
                NCASServer.Singleton.PresentationGroupChanged(AlarmType.Ccu_Ups_BatteryFault);
            }

            if (newDevicesAlarmSetting.AlarmCcuUpsBatteryFusePresentationGroup != null &&
                !newDevicesAlarmSetting.AlarmCcuUpsBatteryFusePresentationGroup.Compare(
                    oldDevicesAlarmSettingBeforeUpdate.AlarmCcuUpsBatteryFusePresentationGroup))
            {
                NCASServer.Singleton.PresentationGroupChanged(AlarmType.Ccu_Ups_BatteryFuse);
            }

            if (newDevicesAlarmSetting.AlarmCcuUpsOvertemperaturePresentationGroup != null &&
                !newDevicesAlarmSetting.AlarmCcuUpsOvertemperaturePresentationGroup.Compare(
                    oldDevicesAlarmSettingBeforeUpdate.AlarmCcuUpsOvertemperaturePresentationGroup))
            {
                NCASServer.Singleton.PresentationGroupChanged(AlarmType.Ccu_Ups_Overtemperature);
            }

            if (newDevicesAlarmSetting.AlarmCcuUpsTamperSabotagePresentationGroup != null &&
                !newDevicesAlarmSetting.AlarmCcuUpsTamperSabotagePresentationGroup.Compare(
                    oldDevicesAlarmSettingBeforeUpdate.AlarmCcuUpsTamperSabotagePresentationGroup))
            {
                NCASServer.Singleton.PresentationGroupChanged(AlarmType.Ccu_Ups_TamperSabotage);
            }

            if (newDevicesAlarmSetting.AlarmCCUFuseOnExtensionBoardPresentationGroup != null &&
                !newDevicesAlarmSetting.AlarmCCUFuseOnExtensionBoardPresentationGroup.Compare(
                    oldDevicesAlarmSettingBeforeUpdate.AlarmCCUFuseOnExtensionBoardPresentationGroup))
            {
                NCASServer.Singleton.PresentationGroupChanged(AlarmType.CCU_ExtFuse);
            }

            if (newDevicesAlarmSetting.AlarmCcuCatUnreachablePresentationGroup != null &&
                !newDevicesAlarmSetting.AlarmCcuCatUnreachablePresentationGroup.Compare(
                    oldDevicesAlarmSettingBeforeUpdate.AlarmCcuCatUnreachablePresentationGroup))
            {
                NCASServer.Singleton.PresentationGroupChanged(AlarmType.Ccu_CatUnreachable);
            }

            if (newDevicesAlarmSetting.AlarmCcuTransferToArcTimedOutPresentationGroup != null &&
                !newDevicesAlarmSetting.AlarmCcuTransferToArcTimedOutPresentationGroup.Compare(
                    oldDevicesAlarmSettingBeforeUpdate.AlarmCcuTransferToArcTimedOutPresentationGroup))
            {
                NCASServer.Singleton.PresentationGroupChanged(AlarmType.Ccu_TransferToArcTimedOut);
            }

            if (newDevicesAlarmSetting.AlarmDCUOfflinePresentationGroup != null &&
                !newDevicesAlarmSetting.AlarmDCUOfflinePresentationGroup.Compare(
                    oldDevicesAlarmSettingBeforeUpdate.AlarmDCUOfflinePresentationGroup))
            {
                NCASServer.Singleton.PresentationGroupChanged(AlarmType.DCU_Offline);
                NCASServer.Singleton.PresentationGroupChanged(AlarmType.DCU_Offline_Due_CCU_Offline);
            }

            if (newDevicesAlarmSetting.AlarmDCUTamperSabotagePresentationGroup != null &&
                !newDevicesAlarmSetting.AlarmDCUTamperSabotagePresentationGroup.Compare(
                    oldDevicesAlarmSettingBeforeUpdate.AlarmDCUTamperSabotagePresentationGroup))
            {
                NCASServer.Singleton.PresentationGroupChanged(AlarmType.DCU_TamperSabotage);
            }

            if (newDevicesAlarmSetting.AlarmDEDoorAjarPresentationGroup != null &&
                !newDevicesAlarmSetting.AlarmDEDoorAjarPresentationGroup.Compare(
                    oldDevicesAlarmSettingBeforeUpdate.AlarmDEDoorAjarPresentationGroup))
            {
                NCASServer.Singleton.PresentationGroupChanged(AlarmType.DoorEnvironment_DoorAjar);
            }

            if (newDevicesAlarmSetting.AlarmDEIntrusionPresentationGroup != null &&
                !newDevicesAlarmSetting.AlarmDEIntrusionPresentationGroup.Compare(
                    oldDevicesAlarmSettingBeforeUpdate.AlarmDEIntrusionPresentationGroup))
            {
                NCASServer.Singleton.PresentationGroupChanged(AlarmType.DoorEnvironment_Intrusion);
            }

            if (newDevicesAlarmSetting.AlarmDESabotagePresentationGroup != null &&
                !newDevicesAlarmSetting.AlarmDESabotagePresentationGroup.Compare(
                    oldDevicesAlarmSettingBeforeUpdate.AlarmDESabotagePresentationGroup))
            {
                NCASServer.Singleton.PresentationGroupChanged(AlarmType.DoorEnvironment_Sabotage);
            }

            if (newDevicesAlarmSetting.AlarmCROfflinePresentationGroup != null &&
                !newDevicesAlarmSetting.AlarmCROfflinePresentationGroup.Compare(
                    oldDevicesAlarmSettingBeforeUpdate.AlarmCROfflinePresentationGroup))
            {
                NCASServer.Singleton.PresentationGroupChanged(AlarmType.CardReader_Offline);
                NCASServer.Singleton.PresentationGroupChanged(AlarmType.CardReader_Offline_Due_CCU_Offline);
            }

            if (newDevicesAlarmSetting.AlarmCRTamperSabotagePresentationGroup != null &&
                !newDevicesAlarmSetting.AlarmCRTamperSabotagePresentationGroup.Compare(
                    oldDevicesAlarmSettingBeforeUpdate.AlarmCRTamperSabotagePresentationGroup))
            {
                NCASServer.Singleton.PresentationGroupChanged(AlarmType.CardReader_TamperSabotage);
            }

            if (newDevicesAlarmSetting.AlarmCRAccessDeniedPresentationGroup != null &&
                !newDevicesAlarmSetting.AlarmCRAccessDeniedPresentationGroup.Compare(
                    oldDevicesAlarmSettingBeforeUpdate.AlarmCRAccessDeniedPresentationGroup))
            {
                NCASServer.Singleton.PresentationGroupChanged(AlarmType.CardReader_AccessDenied);
            }

            if (newDevicesAlarmSetting.AlarmCRUnknownCardPresentationGroup != null &&
                !newDevicesAlarmSetting.AlarmCRUnknownCardPresentationGroup.Compare(
                    oldDevicesAlarmSettingBeforeUpdate.AlarmCRUnknownCardPresentationGroup))
            {
                NCASServer.Singleton.PresentationGroupChanged(AlarmType.CardReader_UnknownCard);
            }

            if (newDevicesAlarmSetting.AlarmCRCardBlockedOrInactivePresentationGroup != null &&
                !newDevicesAlarmSetting.AlarmCRCardBlockedOrInactivePresentationGroup.Compare(
                    oldDevicesAlarmSettingBeforeUpdate.AlarmCRCardBlockedOrInactivePresentationGroup))
            {
                NCASServer.Singleton.PresentationGroupChanged(AlarmType.CardReader_CardBlockedOrInactive);
            }

            if (newDevicesAlarmSetting.AlarmCRInvalidPINPresentationGroup != null &&
                !newDevicesAlarmSetting.AlarmCRInvalidPINPresentationGroup.Compare(
                    oldDevicesAlarmSettingBeforeUpdate.AlarmCRInvalidPINPresentationGroup))
            {
                NCASServer.Singleton.PresentationGroupChanged(AlarmType.CardReader_InvalidPIN);
            }

            if (newDevicesAlarmSetting.AlarmCRInvalidGINPresentationGroup != null &&
                !newDevicesAlarmSetting.AlarmCRInvalidGINPresentationGroup.Compare(
                    oldDevicesAlarmSettingBeforeUpdate.AlarmCRInvalidGINPresentationGroup))
            {
                NCASServer.Singleton.PresentationGroupChanged(AlarmType.CardReader_InvalidCode);
            }

            if (newDevicesAlarmSetting.AlarmCRInvalidEmergencyCodePresentationGroup != null &&
                !newDevicesAlarmSetting.AlarmCRInvalidEmergencyCodePresentationGroup.Compare(
                    oldDevicesAlarmSettingBeforeUpdate.AlarmCRInvalidEmergencyCodePresentationGroup))
            {
                NCASServer.Singleton.PresentationGroupChanged(AlarmType.CardReader_InvalidEmergencyCode);
            }

            if (newDevicesAlarmSetting.AlarmCRAccessPermittedPresentationGroup != null &&
                !newDevicesAlarmSetting.AlarmCRAccessPermittedPresentationGroup.Compare(
                    oldDevicesAlarmSettingBeforeUpdate.AlarmCRAccessPermittedPresentationGroup))
            {
                NCASServer.Singleton.PresentationGroupChanged(AlarmType.CardReader_AccessPermitted);
            }

            if (newDevicesAlarmSetting.PgAlarmInvalidPinRetriesLimitReached != null &&
                !newDevicesAlarmSetting.PgAlarmInvalidPinRetriesLimitReached.Compare(
                    oldDevicesAlarmSettingBeforeUpdate.PgAlarmInvalidPinRetriesLimitReached))
            {
                NCASServer.Singleton.PresentationGroupChanged(AlarmType.CardReader_Invalid_Pin_Retries_Limit_Reached);
            }

            if (newDevicesAlarmSetting.PgAlarmInvalidGinRetriesLimitReached != null &&
                !newDevicesAlarmSetting.PgAlarmInvalidGinRetriesLimitReached.Compare(
                    oldDevicesAlarmSettingBeforeUpdate.PgAlarmInvalidGinRetriesLimitReached))
            {
                NCASServer.Singleton.PresentationGroupChanged(AlarmType.CardReader_Invalid_Code_Retries_Limit_Reached);
            }

            if (newDevicesAlarmSetting.AlarmAreaAlarmPresentationGroup != null &&
                !newDevicesAlarmSetting.AlarmAreaAlarmPresentationGroup.Compare(
                    oldDevicesAlarmSettingBeforeUpdate.AlarmAreaAlarmPresentationGroup))
            {
                NCASServer.Singleton.PresentationGroupChanged(AlarmType.AlarmArea_Alarm);
                NCASServer.Singleton.PresentationGroupChanged(AlarmType.AlarmArea_AAlarm);
            }

            if (newDevicesAlarmSetting.AlarmAreaSetByOnOffObjectFailedPresentationGroup != null &&
                !newDevicesAlarmSetting.AlarmAreaSetByOnOffObjectFailedPresentationGroup.Compare(
                    oldDevicesAlarmSettingBeforeUpdate.AlarmAreaSetByOnOffObjectFailedPresentationGroup))
            {
                NCASServer.Singleton.PresentationGroupChanged(AlarmType.AlarmArea_SetByOnOffObjectFailed);
            }

            if (newDevicesAlarmSetting.SensorAlarmPresentationGroup != null &&
                !newDevicesAlarmSetting.SensorAlarmPresentationGroup.Compare(
                    oldDevicesAlarmSettingBeforeUpdate.SensorAlarmPresentationGroup))
            {
                NCASServer.Singleton.PresentationGroupChanged(AlarmType.Sensor_Alarm);
            }

            if (newDevicesAlarmSetting.SensorTamperAlarmPresentationGroup != null &&
                !newDevicesAlarmSetting.SensorTamperAlarmPresentationGroup.Compare(
                    oldDevicesAlarmSettingBeforeUpdate.SensorTamperAlarmPresentationGroup))
            {
                NCASServer.Singleton.PresentationGroupChanged(AlarmType.Sensor_Tamper_Alarm);
            }

            if (newDevicesAlarmSetting.InvalidPinRetriesLimitReachedTimeout !=
                oldDevicesAlarmSettingBeforeUpdate.InvalidPinRetriesLimitReachedTimeout)
            {
                Cards.Singleton.InvalidPinRetriesLimitReachedTimeout =
                    TimeSpan.FromMinutes(
                        newDevicesAlarmSetting.InvalidPinRetriesLimitReachedTimeout);
            }
        }

        protected override void LoadObjectsInRelationship(DevicesAlarmSetting obj)
        {
            //CCU presentation groups
            if (obj.AlarmCCUOfflinePresentationGroup != null)
            {
                obj.AlarmCCUOfflinePresentationGroup =
                    PresentationGroups.Singleton.GetById(obj.AlarmCCUOfflinePresentationGroup.IdGroup);
            }
            if (obj.AlarmCCUUnconfiguredPresentationGroup != null)
            {
                obj.AlarmCCUUnconfiguredPresentationGroup =
                    PresentationGroups.Singleton.GetById(obj.AlarmCCUUnconfiguredPresentationGroup.IdGroup);
            }
            if (obj.AlarmCCUClockUnsynchronizedPresentationGroup != null)
            {
                obj.AlarmCCUClockUnsynchronizedPresentationGroup =
                    PresentationGroups.Singleton.GetById(obj.AlarmCCUClockUnsynchronizedPresentationGroup.IdGroup);
            }
            if (obj.AlarmCCUTamperSabotagePresentationGroup != null)
            {
                obj.AlarmCCUTamperSabotagePresentationGroup =
                    PresentationGroups.Singleton.GetById(obj.AlarmCCUTamperSabotagePresentationGroup.IdGroup);
            }
            if (obj.AlarmCCUPrimaryPowerMissingPresentationGroup != null)
            {
                obj.AlarmCCUPrimaryPowerMissingPresentationGroup =
                    PresentationGroups.Singleton.GetById(obj.AlarmCCUPrimaryPowerMissingPresentationGroup.IdGroup);
            }
            if (obj.AlarmCCUBatteryIsLowPresentationGroup != null)
            {
                obj.AlarmCCUBatteryIsLowPresentationGroup =
                    PresentationGroups.Singleton.GetById(obj.AlarmCCUBatteryIsLowPresentationGroup.IdGroup);
            }
            if (obj.AlarmCcuUpsOutputFusePresentationGroup != null)
            {
                obj.AlarmCcuUpsOutputFusePresentationGroup =
                    PresentationGroups.Singleton.GetById(obj.AlarmCcuUpsOutputFusePresentationGroup.IdGroup);
            }
            if (obj.AlarmCcuUpsBatteryFaultPresentationGroup != null)
            {
                obj.AlarmCcuUpsBatteryFaultPresentationGroup =
                    PresentationGroups.Singleton.GetById(obj.AlarmCcuUpsBatteryFaultPresentationGroup.IdGroup);
            }
            if (obj.AlarmCcuUpsBatteryFusePresentationGroup != null)
            {
                obj.AlarmCcuUpsBatteryFusePresentationGroup =
                    PresentationGroups.Singleton.GetById(obj.AlarmCcuUpsBatteryFusePresentationGroup.IdGroup);
            }
            if (obj.AlarmCcuUpsOvertemperaturePresentationGroup != null)
            {
                obj.AlarmCcuUpsOvertemperaturePresentationGroup =
                    PresentationGroups.Singleton.GetById(obj.AlarmCcuUpsOvertemperaturePresentationGroup.IdGroup);
            }
            if (obj.AlarmCcuUpsTamperSabotagePresentationGroup != null)
            {
                obj.AlarmCcuUpsTamperSabotagePresentationGroup =
                    PresentationGroups.Singleton.GetById(obj.AlarmCcuUpsTamperSabotagePresentationGroup.IdGroup);
            }
            if (obj.AlarmCCUFuseOnExtensionBoardPresentationGroup != null)
            {
                obj.AlarmCCUFuseOnExtensionBoardPresentationGroup =
                    PresentationGroups.Singleton.GetById(obj.AlarmCCUFuseOnExtensionBoardPresentationGroup.IdGroup);
            }
            if (obj.AlarmCcuCatUnreachablePresentationGroup != null)
            {
                obj.AlarmCcuCatUnreachablePresentationGroup =
                    PresentationGroups.Singleton.GetById(obj.AlarmCcuCatUnreachablePresentationGroup.IdGroup);
            }
            if (obj.AlarmCcuTransferToArcTimedOutPresentationGroup != null)
            {
                obj.AlarmCcuTransferToArcTimedOutPresentationGroup =
                    PresentationGroups.Singleton.GetById(obj.AlarmCcuTransferToArcTimedOutPresentationGroup.IdGroup);
            }
            //DCU presentation groups
            if (obj.AlarmDCUOfflinePresentationGroup != null)
            {
                obj.AlarmDCUOfflinePresentationGroup =
                    PresentationGroups.Singleton.GetById(obj.AlarmDCUOfflinePresentationGroup.IdGroup);
            }
            if (obj.AlarmDCUTamperSabotagePresentationGroup != null)
            {
                obj.AlarmDCUTamperSabotagePresentationGroup =
                    PresentationGroups.Singleton.GetById(obj.AlarmDCUTamperSabotagePresentationGroup.IdGroup);
            }
            if (obj.AlarmDEDoorAjarPresentationGroup != null)
            {
                obj.AlarmDEDoorAjarPresentationGroup =
                    PresentationGroups.Singleton.GetById(obj.AlarmDEDoorAjarPresentationGroup.IdGroup);
            }
            if (obj.AlarmDEIntrusionPresentationGroup != null)
            {
                obj.AlarmDEIntrusionPresentationGroup =
                    PresentationGroups.Singleton.GetById(obj.AlarmDEIntrusionPresentationGroup.IdGroup);
            }
            if (obj.AlarmDESabotagePresentationGroup != null)
            {
                obj.AlarmDESabotagePresentationGroup =
                    PresentationGroups.Singleton.GetById(obj.AlarmDESabotagePresentationGroup.IdGroup);
            }

            if (obj.SecurityDailyPlanForEnterToMenu != null)
                obj.SecurityDailyPlanForEnterToMenu =
                    SecurityDailyPlans.Singleton.GetById(obj.SecurityDailyPlanForEnterToMenu.IdSecurityDailyPlan);

            if (obj.SecurityTimeZoneForEnterToMenu != null)
                obj.SecurityTimeZoneForEnterToMenu =
                    SecurityTimeZones.Singleton.GetById(obj.SecurityTimeZoneForEnterToMenu.IdSecurityTimeZone);

            //Card reader presentation groups
            if (obj.AlarmCROfflinePresentationGroup != null)
            {
                obj.AlarmCROfflinePresentationGroup =
                    PresentationGroups.Singleton.GetById(obj.AlarmCROfflinePresentationGroup.IdGroup);
            }
            if (obj.AlarmCRTamperSabotagePresentationGroup != null)
            {
                obj.AlarmCRTamperSabotagePresentationGroup =
                    PresentationGroups.Singleton.GetById(obj.AlarmCRTamperSabotagePresentationGroup.IdGroup);
            }
            if (obj.AlarmCRAccessDeniedPresentationGroup != null)
            {
                obj.AlarmCRAccessDeniedPresentationGroup =
                    PresentationGroups.Singleton.GetById(obj.AlarmCRAccessDeniedPresentationGroup.IdGroup);
            }
            if (obj.AlarmCRUnknownCardPresentationGroup != null)
            {
                obj.AlarmCRUnknownCardPresentationGroup =
                    PresentationGroups.Singleton.GetById(obj.AlarmCRUnknownCardPresentationGroup.IdGroup);
            }
            if (obj.AlarmCRCardBlockedOrInactivePresentationGroup != null)
            {
                obj.AlarmCRCardBlockedOrInactivePresentationGroup =
                    PresentationGroups.Singleton.GetById(obj.AlarmCRCardBlockedOrInactivePresentationGroup.IdGroup);
            }
            if (obj.AlarmCRInvalidPINPresentationGroup != null)
            {
                obj.AlarmCRInvalidPINPresentationGroup =
                    PresentationGroups.Singleton.GetById(obj.AlarmCRInvalidPINPresentationGroup.IdGroup);
            }
            if (obj.AlarmCRInvalidGINPresentationGroup != null)
            {
                obj.AlarmCRInvalidGINPresentationGroup =
                    PresentationGroups.Singleton.GetById(obj.AlarmCRInvalidGINPresentationGroup.IdGroup);
            }
            if (obj.AlarmCRInvalidEmergencyCodePresentationGroup != null)
            {
                obj.AlarmCRInvalidEmergencyCodePresentationGroup =
                    PresentationGroups.Singleton.GetById(obj.AlarmCRInvalidEmergencyCodePresentationGroup.IdGroup);
            }
            if (obj.AlarmCRAccessPermittedPresentationGroup != null)
            {
                obj.AlarmCRAccessPermittedPresentationGroup =
                    PresentationGroups.Singleton.GetById(obj.AlarmCRAccessPermittedPresentationGroup.IdGroup);
            }
            if (obj.PgAlarmInvalidPinRetriesLimitReached != null)
            {
                obj.PgAlarmInvalidPinRetriesLimitReached =
                    PresentationGroups.Singleton.GetById(obj.PgAlarmInvalidPinRetriesLimitReached.IdGroup);
            }
            if (obj.PgAlarmInvalidGinRetriesLimitReached != null)
            {
                obj.PgAlarmInvalidGinRetriesLimitReached =
                    PresentationGroups.Singleton.GetById(obj.PgAlarmInvalidGinRetriesLimitReached.IdGroup);
            }
            if (obj.AlarmAreaAlarmPresentationGroup != null)
            {
                obj.AlarmAreaAlarmPresentationGroup =
                    PresentationGroups.Singleton.GetById(obj.AlarmAreaAlarmPresentationGroup.IdGroup);
            }
            if (obj.AlarmAreaSetByOnOffObjectFailedPresentationGroup != null)
            {
                obj.AlarmAreaSetByOnOffObjectFailedPresentationGroup =
                    PresentationGroups.Singleton.GetById(obj.AlarmAreaSetByOnOffObjectFailedPresentationGroup.IdGroup);
            }
            if (obj.SensorAlarmPresentationGroup != null)
            {
                obj.SensorAlarmPresentationGroup =
                    PresentationGroups.Singleton.GetById(obj.SensorAlarmPresentationGroup.IdGroup);
            }
            if (obj.SensorTamperAlarmPresentationGroup != null)
            {
                obj.SensorTamperAlarmPresentationGroup =
                    PresentationGroups.Singleton.GetById(obj.SensorTamperAlarmPresentationGroup.IdGroup);
            }

            if (obj.DevicesAlarmSettingAlarmArcs != null)
            {
                var devicesAlarmSettingAlarmArcs = new LinkedList<DevicesAlarmSettingAlarmArc>();

                foreach (var devicesAlarmSettingAlarmArc in obj.DevicesAlarmSettingAlarmArcs)
                {
                    devicesAlarmSettingAlarmArcs.AddLast(
                        DB.DevicesAlarmSettingAlarmArcs.Singleton.GetById(
                            devicesAlarmSettingAlarmArc.IdDevicesAlarmSettingAlarmArc));
                }

                obj.DevicesAlarmSettingAlarmArcs.Clear();

                foreach (var devicesAlarmSettingAlarmArc in devicesAlarmSettingAlarmArcs)
                {
                    obj.DevicesAlarmSettingAlarmArcs.Add(devicesAlarmSettingAlarmArc);
                }
            }
        }

        public bool AlarmCcuOffline()
        {
            var listDAS = List();
            if (listDAS == null || listDAS.Count == 0)
            {
                return true;
            }

            return listDAS.ElementAt(0).AlarmCCUOffline;
        }

        public bool BlockAlarmCCUOffline()
        {
            var listDAS = List();
            if (listDAS == null || listDAS.Count == 0)
            {
                return false;
            }

            return listDAS.ElementAt(0).BlockAlarmCCUOffline;
        }

        public byte? ObjBlockAlarmCCUOfflineObjectType()
        {
            var listDAS = List();
            if (listDAS == null || listDAS.Count == 0)
            {
                return null;
            }

            return listDAS.ElementAt(0).ObjBlockAlarmCCUOfflineObjectType;
        }

        public Guid? ObjBlockAlarmCCUOfflineId()
        {
            var listDAS = List();
            if (listDAS == null || listDAS.Count == 0)
            {
                return null;
            }

            return listDAS.ElementAt(0).ObjBlockAlarmCCUOfflineId;
        }

        public PresentationGroup AlarmCCUOfflinePresentationGroup()
        {
            var listDAS = List();
            if (listDAS == null || listDAS.Count == 0)
            {
                return null;
            }

            return listDAS.ElementAt(0).AlarmCCUOfflinePresentationGroup;
        }

        public bool AlarmCcuUnconfigured()
        {
            var listDAS = List();
            if (listDAS == null || listDAS.Count == 0)
            {
                return false;
            }

            return listDAS.ElementAt(0).AlarmCCUUnconfigured;
        }

        public bool BlockAlarmCCUUnconfigured()
        {
            var listDAS = List();
            if (listDAS == null || listDAS.Count == 0)
            {
                return false;
            }

            return listDAS.ElementAt(0).BlockAlarmCCUUnconfigured;
        }

        public byte? ObjBlockAlarmCCUUnconfiguredObjectType()
        {
            var listDAS = List();
            if (listDAS == null || listDAS.Count == 0)
            {
                return null;
            }

            return listDAS.ElementAt(0).ObjBlockAlarmCCUUnconfiguredObjectType;
        }

        public Guid? ObjBlockAlarmCCUUnconfiguredId()
        {
            var listDAS = List();
            if (listDAS == null || listDAS.Count == 0)
            {
                return null;
            }

            return listDAS.ElementAt(0).ObjBlockAlarmCCUUnconfiguredId;
        }

        public PresentationGroup AlarmCcuUnconfiguredPresentationGroup()
        {
            var listDas = List();
            if (listDas == null || listDas.Count == 0)
            {
                return null;
            }

            return listDas.ElementAt(0).AlarmCCUUnconfiguredPresentationGroup;
        }

        public bool AlarmCcuClockUnsynchronized()
        {
            var listDAS = List();
            if (listDAS == null || listDAS.Count == 0)
            {
                return false;
            }

            return listDAS.ElementAt(0).AlarmCCUClockUnsynchronized;
        }

        public bool BlockAlarmCCUClockUnsynchronized()
        {
            var listDAS = List();
            if (listDAS == null || listDAS.Count == 0)
            {
                return false;
            }

            return listDAS.ElementAt(0).BlockAlarmCCUClockUnsynchronized;
        }

        public byte? ObjBlockAlarmCCUClockUnsynchronizedObjectType()
        {
            var listDAS = List();
            if (listDAS == null || listDAS.Count == 0)
            {
                return null;
            }

            return listDAS.ElementAt(0).ObjBlockAlarmCCUClockUnsynchronizedObjectType;
        }

        public Guid? ObjBlockAlarmCCUClockUnsynchronizedId()
        {
            var listDAS = List();
            if (listDAS == null || listDAS.Count == 0)
            {
                return null;
            }

            return listDAS.ElementAt(0).ObjBlockAlarmCCUClockUnsynchronizedId;
        }

        public PresentationGroup AlarmCcuClockUnsynchronizedPresentationGroup()
        {
            var listDAS = List();
            if (listDAS == null || listDAS.Count == 0)
            {
                return null;
            }

            return listDAS.ElementAt(0).AlarmCCUClockUnsynchronizedPresentationGroup;
        }

        public bool AlarmCcuTamper()
        {
            var listDAS = List();
            if (listDAS == null || listDAS.Count == 0)
            {
                return true;
            }

            return listDAS.ElementAt(0).AlarmCCUTamperSabotage;
        }

        public PresentationGroup AlarmCcuTamperSabotagePresentationGroup()
        {
            var listDas = List();
            if (listDas == null || listDas.Count == 0)
            {
                return null;
            }

            return listDas.ElementAt(0).AlarmCCUTamperSabotagePresentationGroup;
        }

        public PresentationGroup AlarmCcuPrimaryPowerMissingPresentationGroup()
        {
            var listDas = List();
            if (listDas == null || listDas.Count == 0)
            {
                return null;
            }

            return listDas.ElementAt(0).AlarmCCUPrimaryPowerMissingPresentationGroup;
        }

        public PresentationGroup AlarmCcuBatteryIsLowPresentationGroup()
        {
            var listDas = List();
            if (listDas == null || listDas.Count == 0)
            {
                return null;
            }

            return listDas.ElementAt(0).AlarmCCUBatteryIsLowPresentationGroup;
        }

        public PresentationGroup AlarmCcuUpsOutputFusePresentationGroup()
        {
            var listDas = List();
            if (listDas == null || listDas.Count == 0)
            {
                return null;
            }

            return listDas.ElementAt(0).AlarmCcuUpsOutputFusePresentationGroup;
        }

        public PresentationGroup AlarmCcuUpsBatteryFaultPresentationGroup()
        {
            var listDas = List();
            if (listDas == null || listDas.Count == 0)
            {
                return null;
            }

            return listDas.ElementAt(0).AlarmCcuUpsBatteryFaultPresentationGroup;
        }

        public PresentationGroup AlarmCcuUpsBatteryFusePresentationGroup()
        {
            var listDas = List();
            if (listDas == null || listDas.Count == 0)
            {
                return null;
            }

            return listDas.ElementAt(0).AlarmCcuUpsBatteryFusePresentationGroup;
        }

        public PresentationGroup AlarmCcuUpsOvertemperaturePresentationGroup()
        {
            var listDas = List();
            if (listDas == null || listDas.Count == 0)
            {
                return null;
            }

            return listDas.ElementAt(0).AlarmCcuUpsOvertemperaturePresentationGroup;
        }

        public PresentationGroup AlarmCcuUpsTamperSabotagePresentationGroup()
        {
            var listDas = List();
            if (listDas == null || listDas.Count == 0)
            {
                return null;
            }

            return listDas.ElementAt(0).AlarmCcuUpsTamperSabotagePresentationGroup;
        }

        public PresentationGroup AlarmCcuFuseOnExtensionBoardPresentationGroup()
        {
            var listDas = List();
            if (listDas == null || listDas.Count == 0)
            {
                return null;
            }

            return listDas.ElementAt(0).AlarmCCUFuseOnExtensionBoardPresentationGroup;
        }

        public PresentationGroup AlarmCcuCatUnreachablePresentationGroup()
        {
            var listDas = List();
            if (listDas == null || listDas.Count == 0)
            {
                return null;
            }

            return listDas.ElementAt(0).AlarmCcuCatUnreachablePresentationGroup;
        }

        public PresentationGroup AlarmCcuTransferToArcTimedOutPresentationGroup()
        {
            var listDas = List();
            if (listDas == null || listDas.Count == 0)
            {
                return null;
            }

            return listDas.ElementAt(0).AlarmCcuTransferToArcTimedOutPresentationGroup;
        }

        public bool AlarmDcuOffline()
        {
            var listDAS = List();
            if (listDAS == null || listDAS.Count == 0)
            {
                return true;
            }

            return listDAS.ElementAt(0).AlarmDCUOffline;
        }

        public bool BlockAlarmDCUOffline()
        {
            var listDAS = List();
            if (listDAS == null || listDAS.Count == 0)
            {
                return false;
            }

            return listDAS.ElementAt(0).BlockAlarmDCUOffline;
        }

        public byte? ObjBlockAlarmDCUOfflineObjectType()
        {
            var listDAS = List();
            if (listDAS == null || listDAS.Count == 0)
            {
                return null;
            }

            return listDAS.ElementAt(0).ObjBlockAlarmDCUOfflineObjectType;
        }

        public Guid? ObjBlockAlarmDCUOfflineId()
        {
            var listDAS = List();
            if (listDAS == null || listDAS.Count == 0)
            {
                return null;
            }

            return listDAS.ElementAt(0).ObjBlockAlarmDCUOfflineId;
        }

        public PresentationGroup AlarmDcuOfflinePresentationGroup()
        {
            var listDas = List();
            if (listDas == null || listDas.Count == 0)
            {
                return null;
            }

            return listDas.ElementAt(0).AlarmDCUOfflinePresentationGroup;
        }

        public PresentationGroup AlarmDcuTamperSabotagePresentationGroup()
        {
            var listDas = List();
            if (listDas == null || listDas.Count == 0)
            {
                return null;
            }

            return listDas.ElementAt(0).AlarmDCUTamperSabotagePresentationGroup;
        }

        public PresentationGroup AlarmDeDoorAjarPresentationGroup()
        {
            var listDas = List();
            if (listDas == null || listDas.Count == 0)
            {
                return null;
            }

            return listDas.ElementAt(0).AlarmDEDoorAjarPresentationGroup;
        }

        public PresentationGroup AlarmDeIntrusionPresentationGroup()
        {
            var listDas = List();
            if (listDas == null || listDas.Count == 0)
            {
                return null;
            }

            return listDas.ElementAt(0).AlarmDEIntrusionPresentationGroup;
        }

        public PresentationGroup AlarmDeSabotagePresentationGroup()
        {
            var listDas = List();
            if (listDas == null || listDas.Count == 0)
            {
                return null;
            }

            return listDas.ElementAt(0).AlarmDESabotagePresentationGroup;
        }

        public bool AlarmCrOffline()
        {
            var listDAS = List();
            if (listDAS == null || listDAS.Count == 0)
            {
                return true;
            }

            return listDAS.ElementAt(0).AlarmCROffline;
        }

        public bool BlockAlarmCROffline()
        {
            var listDAS = List();
            if (listDAS == null || listDAS.Count == 0)
            {
                return false;
            }

            return listDAS.ElementAt(0).BlockAlarmCROffline;
        }

        public byte? ObjBlockAlarmCROfflineObjectType()
        {
            var listDAS = List();
            if (listDAS == null || listDAS.Count == 0)
            {
                return null;
            }

            return listDAS.ElementAt(0).ObjBlockAlarmCROfflineObjectType;
        }

        public Guid? ObjBlockAlarmCROfflineId()
        {
            var listDAS = List();
            if (listDAS == null || listDAS.Count == 0)
            {
                return null;
            }

            return listDAS.ElementAt(0).ObjBlockAlarmCROfflineId;
        }

        public PresentationGroup AlarmCrOfflinePresentationGroup()
        {
            var listDas = List();
            if (listDas == null || listDas.Count == 0)
            {
                return null;
            }

            return listDas.ElementAt(0).AlarmCROfflinePresentationGroup;
        }

        public PresentationGroup AlarmCrTamperSabotagePresentationGroup()
        {
            var listDas = List();
            if (listDas == null || listDas.Count == 0)
            {
                return null;
            }

            return listDas.ElementAt(0).AlarmCRTamperSabotagePresentationGroup;
        }

        public PresentationGroup AlarmCrAccessDeniedPresentationGroup()
        {
            var listDas = List();
            if (listDas == null || listDas.Count == 0)
            {
                return null;
            }

            return listDas.ElementAt(0).AlarmCRAccessDeniedPresentationGroup;
        }

        public int InvalidPinRetriesLimitReachedTimeout
        {
            get
            {
                var listDas = List();
                if (listDas == null || listDas.Count == 0)
                {
                    return 5;
                }

                return listDas.ElementAt(0)
                    .InvalidPinRetriesLimitReachedTimeout;
            }
        }

        public PresentationGroup AlarmCrUnknownCardPresentationGroup()
        {
            var listDas = List();
            if (listDas == null || listDas.Count == 0)
            {
                return null;
            }

            return listDas.ElementAt(0).AlarmCRUnknownCardPresentationGroup;
        }

        public PresentationGroup AlarmCrCardBlockedOrInactivePresentationGroup()
        {
            var listDas = List();
            if (listDas == null || listDas.Count == 0)
            {
                return null;
            }

            return listDas.ElementAt(0).AlarmCRCardBlockedOrInactivePresentationGroup;
        }

        public PresentationGroup AlarmCrInvalidPinPresentationGroup()
        {
            var listDas = List();
            if (listDas == null || listDas.Count == 0)
            {
                return null;
            }

            return listDas.ElementAt(0).AlarmCRInvalidPINPresentationGroup;
        }

        public PresentationGroup AlarmCrInvalidGinPresentationGroup()
        {
            var listDas = List();
            if (listDas == null || listDas.Count == 0)
            {
                return null;
            }

            return listDas.ElementAt(0).AlarmCRInvalidGINPresentationGroup;
        }

        public PresentationGroup AlarmCrInvalidEmergencyCodePresentationGroup()
        {
            var listDas = List();
            if (listDas == null || listDas.Count == 0)
            {
                return null;
            }

            return listDas.ElementAt(0).AlarmCRInvalidEmergencyCodePresentationGroup;
        }

        public PresentationGroup AlarmCrAccessPermittedPresentationGroup()
        {
            var listDas = List();
            if (listDas == null || listDas.Count == 0)
            {
                return null;
            }

            return listDas.ElementAt(0).AlarmCRAccessPermittedPresentationGroup;
        }

        public PresentationGroup PgAlarmInvalidPinRetriesLimitReached()
        {
            var listDas = List();
            if (listDas == null || listDas.Count == 0)
            {
                return null;
            }

            return listDas.ElementAt(0).PgAlarmInvalidPinRetriesLimitReached;
        }

        public PresentationGroup PgAlarmInvalidGinRetriesLimitReached()
        {
            var listDas = List();
            if (listDas == null || listDas.Count == 0)
            {
                return null;
            }

            return listDas.ElementAt(0).PgAlarmInvalidGinRetriesLimitReached;
        }

        public PresentationGroup AlarmAreaSetByOnOffObjectFailedPresentationGroup()
        {
            var listDas = List();
            if (listDas == null || listDas.Count == 0)
            {
                return null;
            }

            return listDas.ElementAt(0).AlarmAreaSetByOnOffObjectFailedPresentationGroup;
        }

        public PresentationGroup AlarmAreaAlarmPresentationGroup()
        {
            var listDas = List();
            if (listDas == null || listDas.Count == 0)
            {
                return null;
            }

            return listDas.ElementAt(0).AlarmAreaAlarmPresentationGroup;
        }

        public PresentationGroup SensorAlarmPresentationGroup()
        {
            var listDas = List();
            if (listDas == null || listDas.Count == 0)
            {
                return null;
            }

            return listDas.ElementAt(0).SensorAlarmPresentationGroup;
        }

        public PresentationGroup SensorTamperAlarmPresentationGroup()
        {
            var listDas = List();
            if (listDas == null || listDas.Count == 0)
            {
                return null;
            }

            return listDas.ElementAt(0).SensorTamperAlarmPresentationGroup;
        }

        public ICollection<DevicesAlarmSettingAlarmArc> DevicesAlarmSettingAlarmArcs()
        {
            var devicesAlarmSettings = List();

            if (devicesAlarmSettings == null || devicesAlarmSettings.Count == 0)
            {
                return null;
            }

            return devicesAlarmSettings.ElementAt(0).DevicesAlarmSettingAlarmArcs;
        }

        public void ConfigureGeneralAlarmArcs()
        {
            var devicesAlarmSettings = List();

            if (devicesAlarmSettings == null || devicesAlarmSettings.Count == 0)
            {
                return;
            }

            CatAlarmsManager.Singleton.ConfigureGeneralAlarmArcs(
                (devicesAlarmSettings.ElementAt(0).DevicesAlarmSettingAlarmArcs ??
                 Enumerable.Empty<DevicesAlarmSettingAlarmArc>())
                    .Cast<IAlarmArcForAlarmType>());
        }

        public bool AlarmCcuCatUnreachable()
        {
            var devicesAlarmSettings = List();

            if (devicesAlarmSettings == null
                || devicesAlarmSettings.Count == 0)
            {
                return true;
            }

            return devicesAlarmSettings.ElementAt(0).AlarmCcuCatUnreachable;
        }

        public bool AlarmCcuTransferToArcTimedOut()
        {
            var devicesAlarmSettings = List();

            if (devicesAlarmSettings == null
                || devicesAlarmSettings.Count == 0)
            {
                return true;
            }

            return devicesAlarmSettings.ElementAt(0).AlarmCcuTransferToArcTimedOut;
        }

        public SecurityDailyPlan SecurityDailyPlanForEnterToMenu()
        {
            var devicesAlarmSettings = List();

            if (devicesAlarmSettings == null
                || devicesAlarmSettings.Count == 0)
            {
                return null;
            }

            return devicesAlarmSettings.ElementAt(0).SecurityDailyPlanForEnterToMenu;
        }

        public SecurityTimeZone SecurityTimeZoneForEnterToMenu()
        {
            var devicesAlarmSettings = List();

            if (devicesAlarmSettings == null
                || devicesAlarmSettings.Count == 0)
            {
                return null;
            }

            return devicesAlarmSettings.ElementAt(0).SecurityTimeZoneForEnterToMenu;
        }

        public ICollection<DevicesAlarmSetting> GetDevicesAlarmSettingsForPg(PresentationGroup pg)
        {
            try
            {
                if (pg == null) return null;
                return SelectLinq<DevicesAlarmSetting>(
                    devicesAlarmSetting =>
                        devicesAlarmSetting.AlarmCCUOfflinePresentationGroup == pg
                        || devicesAlarmSetting.AlarmCCUUnconfiguredPresentationGroup == pg
                        || devicesAlarmSetting.AlarmCCUClockUnsynchronizedPresentationGroup == pg
                        || devicesAlarmSetting.AlarmCCUTamperSabotagePresentationGroup == pg
                        || devicesAlarmSetting.AlarmCCUPrimaryPowerMissingPresentationGroup == pg
                        || devicesAlarmSetting.AlarmCCUBatteryIsLowPresentationGroup == pg
                        || devicesAlarmSetting.AlarmCcuUpsOutputFusePresentationGroup == pg
                        || devicesAlarmSetting.AlarmCcuUpsBatteryFaultPresentationGroup == pg
                        || devicesAlarmSetting.AlarmCcuUpsBatteryFusePresentationGroup == pg
                        || devicesAlarmSetting.AlarmCcuUpsOvertemperaturePresentationGroup == pg
                        || devicesAlarmSetting.AlarmCcuUpsTamperSabotagePresentationGroup == pg
                        || devicesAlarmSetting.AlarmCCUFuseOnExtensionBoardPresentationGroup == pg
                        || devicesAlarmSetting.AlarmCcuCatUnreachablePresentationGroup == pg
                        || devicesAlarmSetting.AlarmCcuTransferToArcTimedOutPresentationGroup == pg
                        || devicesAlarmSetting.AlarmDCUOfflinePresentationGroup == pg
                        || devicesAlarmSetting.AlarmDCUTamperSabotagePresentationGroup == pg
                        || devicesAlarmSetting.AlarmDEDoorAjarPresentationGroup == pg
                        || devicesAlarmSetting.AlarmDEIntrusionPresentationGroup == pg
                        || devicesAlarmSetting.AlarmDESabotagePresentationGroup == pg
                        || devicesAlarmSetting.AlarmCRTamperSabotagePresentationGroup == pg
                        || devicesAlarmSetting.AlarmCROfflinePresentationGroup == pg
                        || devicesAlarmSetting.AlarmCRAccessDeniedPresentationGroup == pg
                        || devicesAlarmSetting.AlarmCRUnknownCardPresentationGroup == pg
                        || devicesAlarmSetting.AlarmCRCardBlockedOrInactivePresentationGroup == pg
                        || devicesAlarmSetting.AlarmCRInvalidPINPresentationGroup == pg
                        || devicesAlarmSetting.AlarmCRInvalidGINPresentationGroup == pg
                        || devicesAlarmSetting.AlarmCRInvalidEmergencyCodePresentationGroup == pg
                        || devicesAlarmSetting.AlarmCRAccessPermittedPresentationGroup == pg
                        || devicesAlarmSetting.PgAlarmInvalidPinRetriesLimitReached == pg
                        || devicesAlarmSetting.PgAlarmInvalidGinRetriesLimitReached == pg
                        || devicesAlarmSetting.AlarmAreaAlarmPresentationGroup == pg
                        || devicesAlarmSetting.AlarmAreaSetByOnOffObjectFailedPresentationGroup == pg
                        || devicesAlarmSetting.SensorAlarmPresentationGroup == pg
                        || devicesAlarmSetting.SensorTamperAlarmPresentationGroup == pg);
            }
            catch
            {
                return null;
            }
        }

        public void CreateDefaultSettings()
        {
            try
            {
                var dasList = List<DevicesAlarmSetting>();
                if (dasList == null || dasList.Count == 0)
                {
                    var das = new DevicesAlarmSetting
                    {
                        Name = "DeviceAlarmSetting",
                        AlarmCCUOffline = true,
                        AlarmDCUOffline = true,
                        AlarmCROffline = true,
                        AlarmCCUTamperSabotage = true,
                        AlarmDCUTamperSabotage = true,
                        AlarmCRTamperSabotage = true,
                        AlarmCCUClockUnsynchronized = true,
                        AllowAAToCRsReporting = true,
                        AlarmInvalidPinRetriesLimitReached = true,
                        AlarmInvalidGinRetriesLimitReached = true,
                        InvalidPinRetriesLimitEnabled = true,
                        InvalidPinRetriesCount = 3,
                        InvalidPinRetriesLimitReachedTimeout = 5,
                        InvalidGinRetriesLimitEnabled = true,
                        InvalidGinRetriesCount = 3,
                        InvalidGinRetriesLimitReachedTimeout = 5,
                        AlarmCcuCatUnreachable = true,
                        AlarmCcuTransferToArcTimedOut = true,
                        SecurityLevelForEnterToMenu = (byte)SecurityLevel.CARDPIN
                    };

                    Insert(ref das);
                }
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        public DevicesAlarmSetting GetDevicesAlarmSetting()
        {
            try
            {
                var dasList = List();

                if (dasList != null && dasList.Count > 0)
                {
                    return dasList.ElementAt(0);
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public List<AOrmObject> GetReferencedObjectsForCcuReplication(Guid guidDeviceAlarmSetting)
        {
            var objects = new List<AOrmObject>();

            var devicesAlarmSetting = GetById(guidDeviceAlarmSetting);
            if (devicesAlarmSetting != null)
            {
                if (devicesAlarmSetting.ObjBlockAlarmCCUTamperSabotageObjectType != null && devicesAlarmSetting.ObjBlockAlarmCCUTamperSabotageId != null)
                {
                    var blockingObject = DataReplicationManager.LoadOnOffObjectFromDatabase(devicesAlarmSetting.ObjBlockAlarmCCUTamperSabotageId.Value, devicesAlarmSetting.ObjBlockAlarmCCUTamperSabotageObjectType.Value);
                    if (blockingObject != null)
                    {
                        objects.Add(blockingObject);
                    }
                }

                if (devicesAlarmSetting.ObjBlockAlarmPrimaryPowerMissingObjectType != null && devicesAlarmSetting.ObjBlockAlarmPrimaryPowerMissingId != null)
                {
                    var blockingObject = DataReplicationManager.LoadOnOffObjectFromDatabase(devicesAlarmSetting.ObjBlockAlarmPrimaryPowerMissingId.Value, devicesAlarmSetting.ObjBlockAlarmPrimaryPowerMissingObjectType.Value);
                    if (blockingObject != null)
                    {
                        objects.Add(blockingObject);
                    }
                }

                if (devicesAlarmSetting.ObjBlockAlarmBatteryIsLowObjectType != null && devicesAlarmSetting.ObjBlockAlarmBatteryIsLowId != null)
                {
                    var blockingObject = DataReplicationManager.LoadOnOffObjectFromDatabase(devicesAlarmSetting.ObjBlockAlarmBatteryIsLowId.Value, devicesAlarmSetting.ObjBlockAlarmBatteryIsLowObjectType.Value);
                    if (blockingObject != null)
                    {
                        objects.Add(blockingObject);
                    }
                }

                if (devicesAlarmSetting.ObjBlockAlarmUpsOutputFuseObjectType != null && devicesAlarmSetting.ObjBlockAlarmUpsOutputFuseId != null)
                {
                    var blockingObject = DataReplicationManager.LoadOnOffObjectFromDatabase(devicesAlarmSetting.ObjBlockAlarmUpsOutputFuseId.Value, devicesAlarmSetting.ObjBlockAlarmUpsOutputFuseObjectType.Value);
                    if (blockingObject != null)
                    {
                        objects.Add(blockingObject);
                    }
                }

                if (devicesAlarmSetting.ObjBlockAlarmUpsBatteryFaultObjectType != null && devicesAlarmSetting.ObjBlockAlarmUpsBatteryFaultId != null)
                {
                    var blockingObject = DataReplicationManager.LoadOnOffObjectFromDatabase(devicesAlarmSetting.ObjBlockAlarmUpsBatteryFaultId.Value, devicesAlarmSetting.ObjBlockAlarmUpsBatteryFaultObjectType.Value);
                    if (blockingObject != null)
                    {
                        objects.Add(blockingObject);
                    }
                }

                if (devicesAlarmSetting.ObjBlockAlarmUpsBatteryFuseObjectType != null && devicesAlarmSetting.ObjBlockAlarmUpsBatteryFuseId != null)
                {
                    var blockingObject = DataReplicationManager.LoadOnOffObjectFromDatabase(devicesAlarmSetting.ObjBlockAlarmUpsBatteryFuseId.Value, devicesAlarmSetting.ObjBlockAlarmUpsBatteryFuseObjectType.Value);
                    if (blockingObject != null)
                    {
                        objects.Add(blockingObject);
                    }
                }

                if (devicesAlarmSetting.ObjBlockAlarmUpsOvertemperatureObjectType != null && devicesAlarmSetting.ObjBlockAlarmUpsOvertemperatureId != null)
                {
                    var blockingObject = DataReplicationManager.LoadOnOffObjectFromDatabase(devicesAlarmSetting.ObjBlockAlarmUpsOvertemperatureId.Value, devicesAlarmSetting.ObjBlockAlarmUpsOvertemperatureObjectType.Value);
                    if (blockingObject != null)
                    {
                        objects.Add(blockingObject);
                    }
                }

                if (devicesAlarmSetting.ObjBlockAlarmUpsTamperSabotageObjectType != null && devicesAlarmSetting.ObjBlockAlarmUpsTamperSabotageId != null)
                {
                    var blockingObject = DataReplicationManager.LoadOnOffObjectFromDatabase(devicesAlarmSetting.ObjBlockAlarmUpsTamperSabotageId.Value, devicesAlarmSetting.ObjBlockAlarmUpsTamperSabotageObjectType.Value);
                    if (blockingObject != null)
                    {
                        objects.Add(blockingObject);
                    }
                }

                if (devicesAlarmSetting.ObjBlockAlarmFuseOnExtensionBoardObjectType != null && devicesAlarmSetting.ObjBlockAlarmFuseOnExtensionBoardId != null)
                {
                    var blockingObject = DataReplicationManager.LoadOnOffObjectFromDatabase(devicesAlarmSetting.ObjBlockAlarmFuseOnExtensionBoardId.Value, devicesAlarmSetting.ObjBlockAlarmFuseOnExtensionBoardObjectType.Value);
                    if (blockingObject != null)
                    {
                        objects.Add(blockingObject);
                    }
                }

                if (devicesAlarmSetting.ObjBlockAlarmDCUOfflineObjectType != null && devicesAlarmSetting.ObjBlockAlarmDCUOfflineId != null)
                {
                    var blockingObject = DataReplicationManager.LoadOnOffObjectFromDatabase(
                        devicesAlarmSetting.ObjBlockAlarmDCUOfflineId.Value,
                        devicesAlarmSetting.ObjBlockAlarmDCUOfflineObjectType.Value);

                    if (blockingObject != null)
                    {
                        objects.Add(blockingObject);
                    }
                }

                if (devicesAlarmSetting.ObjBlockAlarmDCUTamperSabotageObjectType != null && devicesAlarmSetting.ObjBlockAlarmDCUTamperSabotageId != null)
                {
                    var blockingObject = DataReplicationManager.LoadOnOffObjectFromDatabase(devicesAlarmSetting.ObjBlockAlarmDCUTamperSabotageId.Value, devicesAlarmSetting.ObjBlockAlarmDCUTamperSabotageObjectType.Value);
                    if (blockingObject != null)
                    {
                        objects.Add(blockingObject);
                    }
                }

                if (devicesAlarmSetting.ObjBlockAlarmDEDoorAjarObjectType != null && devicesAlarmSetting.ObjBlockAlarmDEDoorAjarId != null)
                {
                    var blockingObject = DataReplicationManager.LoadOnOffObjectFromDatabase(devicesAlarmSetting.ObjBlockAlarmDEDoorAjarId.Value, devicesAlarmSetting.ObjBlockAlarmDEDoorAjarObjectType.Value);
                    if (blockingObject != null)
                    {
                        objects.Add(blockingObject);
                    }
                }

                if (devicesAlarmSetting.ObjBlockAlarmDEIntrusionObjectType != null && devicesAlarmSetting.ObjBlockAlarmDEIntrusionId != null)
                {
                    var blockingObject = DataReplicationManager.LoadOnOffObjectFromDatabase(devicesAlarmSetting.ObjBlockAlarmDEIntrusionId.Value, devicesAlarmSetting.ObjBlockAlarmDEIntrusionObjectType.Value);
                    if (blockingObject != null)
                    {
                        objects.Add(blockingObject);
                    }
                }

                if (devicesAlarmSetting.ObjBlockAlarmDESabotageObjectType != null && devicesAlarmSetting.ObjBlockAlarmDESabotageId != null)
                {
                    var blockingObject = DataReplicationManager.LoadOnOffObjectFromDatabase(devicesAlarmSetting.ObjBlockAlarmDESabotageId.Value, devicesAlarmSetting.ObjBlockAlarmDESabotageObjectType.Value);
                    if (blockingObject != null)
                    {
                        objects.Add(blockingObject);
                    }
                }

                if (devicesAlarmSetting.SecurityDailyPlanForEnterToMenu != null)
                    objects.Add(devicesAlarmSetting.SecurityDailyPlanForEnterToMenu);

                if (devicesAlarmSetting.SecurityTimeZoneForEnterToMenu != null)
                    objects.Add(devicesAlarmSetting.SecurityTimeZoneForEnterToMenu);

                if (devicesAlarmSetting.ObjBlockAlarmCROfflineObjectType != null && devicesAlarmSetting.ObjBlockAlarmCROfflineId != null)
                {
                    var blockingObject = DataReplicationManager.LoadOnOffObjectFromDatabase(
                        devicesAlarmSetting.ObjBlockAlarmCROfflineId.Value,
                        devicesAlarmSetting.ObjBlockAlarmCROfflineObjectType.Value);

                    if (blockingObject != null)
                    {
                        objects.Add(blockingObject);
                    }
                }

                if (devicesAlarmSetting.ObjBlockAlarmCRTamperSabotageObjectType != null && devicesAlarmSetting.ObjBlockAlarmCRTamperSabotageId != null)
                {
                    var blockingObject = DataReplicationManager.LoadOnOffObjectFromDatabase(devicesAlarmSetting.ObjBlockAlarmCRTamperSabotageId.Value, devicesAlarmSetting.ObjBlockAlarmCRTamperSabotageObjectType.Value);
                    if (blockingObject != null)
                    {
                        objects.Add(blockingObject);
                    }
                }

                if (devicesAlarmSetting.ObjBlockAlarmCrAccessDeniedObjectType != null && devicesAlarmSetting.ObjBlockAlarmCrAccessDeniedId != null)
                {
                    var blockingObject = DataReplicationManager.LoadOnOffObjectFromDatabase(devicesAlarmSetting.ObjBlockAlarmCrAccessDeniedId.Value, devicesAlarmSetting.ObjBlockAlarmCrAccessDeniedObjectType.Value);
                    if (blockingObject != null)
                    {
                        objects.Add(blockingObject);
                    }
                }

                if (devicesAlarmSetting.ObjBlockAlarmCrUnknownCardObjectType != null && devicesAlarmSetting.ObjBlockAlarmCrUnknownCardId != null)
                {
                    var blockingObject = DataReplicationManager.LoadOnOffObjectFromDatabase(devicesAlarmSetting.ObjBlockAlarmCrUnknownCardId.Value, devicesAlarmSetting.ObjBlockAlarmCrUnknownCardObjectType.Value);
                    if (blockingObject != null)
                    {
                        objects.Add(blockingObject);
                    }
                }

                if (devicesAlarmSetting.ObjBlockAlarmCrCardBlockedOrInactiveObjectType != null && devicesAlarmSetting.ObjBlockAlarmCrCardBlockedOrInactiveId != null)
                {
                    var blockingObject = DataReplicationManager.LoadOnOffObjectFromDatabase(devicesAlarmSetting.ObjBlockAlarmCrCardBlockedOrInactiveId.Value, devicesAlarmSetting.ObjBlockAlarmCrCardBlockedOrInactiveObjectType.Value);
                    if (blockingObject != null)
                    {
                        objects.Add(blockingObject);
                    }
                }

                if (devicesAlarmSetting.ObjBlockAlarmCrInvalidPinObjectType != null && devicesAlarmSetting.ObjBlockAlarmCrInvalidPinId != null)
                {
                    var blockingObject = DataReplicationManager.LoadOnOffObjectFromDatabase(devicesAlarmSetting.ObjBlockAlarmCrInvalidPinId.Value, devicesAlarmSetting.ObjBlockAlarmCrInvalidPinObjectType.Value);
                    if (blockingObject != null)
                    {
                        objects.Add(blockingObject);
                    }
                }

                if (devicesAlarmSetting.ObjBlockAlarmCrInvalidGinObjectType != null && devicesAlarmSetting.ObjBlockAlarmCrInvalidGinId != null)
                {
                    var blockingObject = DataReplicationManager.LoadOnOffObjectFromDatabase(devicesAlarmSetting.ObjBlockAlarmCrInvalidGinId.Value, devicesAlarmSetting.ObjBlockAlarmCrInvalidGinObjectType.Value);
                    if (blockingObject != null)
                    {
                        objects.Add(blockingObject);
                    }
                }

                if (devicesAlarmSetting.ObjBlockAlarmCrInvalidEmergencyCodeObjectType != null && devicesAlarmSetting.ObjBlockAlarmCrInvalidEmergencyCodeId != null)
                {
                    var blockingObject = DataReplicationManager.LoadOnOffObjectFromDatabase(devicesAlarmSetting.ObjBlockAlarmCrInvalidEmergencyCodeId.Value, devicesAlarmSetting.ObjBlockAlarmCrInvalidEmergencyCodeObjectType.Value);
                    if (blockingObject != null)
                    {
                        objects.Add(blockingObject);
                    }
                }

                if (devicesAlarmSetting.ObjBlockAlarmCrAccessPermittedObjectType != null && devicesAlarmSetting.ObjBlockAlarmCrAccessPermittedId != null)
                {
                    var blockingObject = DataReplicationManager.LoadOnOffObjectFromDatabase(devicesAlarmSetting.ObjBlockAlarmCrAccessPermittedId.Value, devicesAlarmSetting.ObjBlockAlarmCrAccessPermittedObjectType.Value);
                    if (blockingObject != null)
                    {
                        objects.Add(blockingObject);
                    }
                }

                if (devicesAlarmSetting.ObjBlockAlarmInvalidPinRetriesLimitReachedObjectType != null
                    && devicesAlarmSetting.ObjBlockAlarmInvalidPinRetriesLimitReachedId != null)
                {
                    var blockingObject = DataReplicationManager.LoadOnOffObjectFromDatabase(
                        devicesAlarmSetting.ObjBlockAlarmInvalidPinRetriesLimitReachedId.Value,
                        devicesAlarmSetting.ObjBlockAlarmInvalidPinRetriesLimitReachedObjectType.Value);

                    if (blockingObject != null)
                    {
                        objects.Add(blockingObject);
                    }
                }

                if (devicesAlarmSetting.ObjBlockAlarmInvalidGinRetriesLimitReachedObjectType != null
                    && devicesAlarmSetting.ObjBlockAlarmInvalidGinRetriesLimitReachedId != null)
                {
                    var blockingObject = DataReplicationManager.LoadOnOffObjectFromDatabase(
                        devicesAlarmSetting.ObjBlockAlarmInvalidGinRetriesLimitReachedId.Value,
                        devicesAlarmSetting.ObjBlockAlarmInvalidGinRetriesLimitReachedObjectType.Value);

                    if (blockingObject != null)
                    {
                        objects.Add(blockingObject);
                    }
                }

                if (devicesAlarmSetting.ObjBlockAlarmAreaSetByOnOffObjectFailedObjectType != null
                    && devicesAlarmSetting.ObjBlockAlarmAreaSetByOnOffObjectFailedId != null)
                {
                    var blockingObject = DataReplicationManager.LoadOnOffObjectFromDatabase(
                        devicesAlarmSetting.ObjBlockAlarmAreaSetByOnOffObjectFailedId.Value,
                        devicesAlarmSetting.ObjBlockAlarmAreaSetByOnOffObjectFailedObjectType.Value);

                    if (blockingObject != null)
                    {
                        objects.Add(blockingObject);
                    }
                }

                if (devicesAlarmSetting.DevicesAlarmSettingAlarmArcs != null)
                {
                    objects.AddRange(devicesAlarmSetting.DevicesAlarmSettingAlarmArcs.Select(
                        devicesAlarmSettingAlarmArc =>
                            AlarmArcs.Singleton.GetById(devicesAlarmSettingAlarmArc.IdAlarmArc))
                        .Cast<AOrmObject>());
                }
            }

            return objects;
        }

        public override ObjectType ObjectType
        {
            get { return ObjectType.DevicesAlarmSetting; }
        }

        public bool CodeAlreadyUsed(string codeHashValue)
        {
            var result = SelectLinq<DevicesAlarmSetting>(
                devicesAlrmSetting =>
                    codeHashValue.Equals(devicesAlrmSetting.GinForEnterToMenu));

            return result != null
                   && result.Count > 0;
        }

        public override bool CheckData(DevicesAlarmSetting ormObject, out Exception error)
        {
            if (!string.IsNullOrEmpty(ormObject.GinForEnterToMenu))
            {
                if (Persons.Singleton.PersonalCodeAlreadyUsed(ormObject.GinForEnterToMenu))
                {
                    error = new IwQuick.SqlUniqueException(DevicesAlarmSetting.COLUMN_GIN_FOR_ENTER_TO_MENU);
                    return false;
                }
            }

            return base.CheckData(ormObject, out error);
        }
    }
}
