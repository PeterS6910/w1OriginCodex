using System;
using System.Collections.Generic;

using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.NCAS.Server.DB;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Server.DB;
using Contal.Cgp.Globals;
using Contal.Cgp.BaseLib;
using TimeZone = Contal.Cgp.Server.Beans.TimeZone;

namespace Contal.Cgp.NCAS.Server.CcuDataReplicator
{
    public partial class DataReplicationManager
    {
        private partial class CcuDataReplicator :
            IDisposable
        {
            private class Initializer
            {
                private CcuDataReplicator _ccuDataReplicator;
                private IDictionary<IdAndObjectType, HashSet<Guid>> _cacheOfParentCcuIdsForObjects;
                private IDictionary<IdAndObjectType, ICollection<AOrmObject>> _referencedObjectsCache;


                public Initializer(CcuDataReplicator ccuDataReplicator)
                {
                    _ccuDataReplicator = ccuDataReplicator;
                    _cacheOfParentCcuIdsForObjects = new Dictionary<IdAndObjectType, HashSet<Guid>>();
                    _referencedObjectsCache = new Dictionary<IdAndObjectType, ICollection<AOrmObject>>();
                }

                public void ReadObjectsForCcu(CCU ccu)
                {
                    try
                    {
                        if (!_ccuDataReplicator.UpdateObjectInfo(
							ccu,
                            _cacheOfParentCcuIdsForObjects,
                            _referencedObjectsCache))
                        {
                            return;
                        }

                        if (ccu.Inputs != null)
                            foreach (var input in ccu.Inputs)
                                SaveInput(
                                    ccu,
                                    input);

                        if (ccu.Outputs != null)
                            foreach (var output in ccu.Outputs)
                                SaveOutput(
                                    ccu,
                                    output);

                        if (ccu.DoorEnvironments != null)
                            foreach (var doorEnvironment in ccu.DoorEnvironments)
                                SaveDoorEnvironment(
                                    ccu,
                                    doorEnvironment);

                        var hashSetAllCCUCardReaders = new HashSet<Guid>();

                        if (ccu.CardReaders != null)
                            foreach (var cardReader in ccu.CardReaders)
                            {
                                SaveCardReader(
                                    ccu,
                                    cardReader);

                                hashSetAllCCUCardReaders.Add(cardReader.IdCardReader);
                            }

                        if (ccu.DCUs != null)
                            foreach (var dcu in ccu.DCUs)
                                SaveDCU(
                                    ccu,
                                    dcu,
                                    hashSetAllCCUCardReaders);

                        if (hashSetAllCCUCardReaders.Count > 0)
                        {
                            SaveAccessControlLists(
                                ccu,
                                hashSetAllCCUCardReaders);

                            SaveAccesZonessObjectTypeNotCardReader(
                                ccu,
                                hashSetAllCCUCardReaders);
                        }

                        var alarmAreas = AlarmAreas.Singleton.List();

                        if (alarmAreas != null)
                            foreach (var alarmArea in alarmAreas)
                                if (SaveAlarmAreaToCCU(
                                    alarmArea,
                                    ccu))
                                {
                                    SaveAlarmArea(
                                        ccu,
                                        alarmArea);
                                }

                        var das =
                            DevicesAlarmSettings.Singleton
                                .GetDevicesAlarmSetting();

                        if (das != null)
                            SaveDevicesAlarmSetting(
                                ccu,
                                das);

                        var cardSystemCollection = CardSystems.Singleton.List();

                        foreach (var cardSystem in cardSystemCollection)
                            if (cardSystem.ExplicitSmartCardDataPopulation)
                                _ccuDataReplicator.UpdateObjectInfo(
									cardSystem,
                                    _cacheOfParentCcuIdsForObjects,
                                    _referencedObjectsCache);

                        var antiPassBackZonesForCardReaders =
                            AntiPassBackZones.Singleton.GetAntiPassBackZonesForCardReaders(
                                hashSetAllCCUCardReaders);

                        foreach (var antiPassBackZone in antiPassBackZonesForCardReaders)
                            SaveAntiPassBackZone(antiPassBackZone);

                        foreach (var multiDoor in MultiDoors.Singleton.
                            GetMultiDoorsForCardReaders(hashSetAllCCUCardReaders))
                        {
                            SaveMultiDoor(
                                ccu,
                                multiDoor);
                        }

                        SaveOnOffObject(
                            ccu,
                            ccu.ObjBlockAlarmTamperObjectType,
                            ccu.ObjBlockAlarmTamperId);

                        SaveOnOffObject(
                            ccu,
                            ccu.ObjBlockAlarmPrimaryPowerMissingObjectType,
                            ccu.ObjBlockAlarmPrimaryPowerMissingId);

                        SaveOnOffObject(
                            ccu,
                            ccu.ObjBlockAlarmBatteryIsLowObjectType,
                            ccu.ObjBlockAlarmBatteryIsLowId);

                        SaveOnOffObject(
                            ccu,
                            ccu.ObjBlockAlarmUpsOutputFuseObjectType,
                            ccu.ObjBlockAlarmUpsOutputFuseId);

                        SaveOnOffObject(
                            ccu,
                            ccu.ObjBlockAlarmUpsBatteryFaultObjectType,
                            ccu.ObjBlockAlarmUpsBatteryFaultId);

                        SaveOnOffObject(
                            ccu,
                            ccu.ObjBlockAlarmUpsBatteryFuseObjectType,
                            ccu.ObjBlockAlarmUpsBatteryFuseId);

                        SaveOnOffObject(
                            ccu,
                            ccu.ObjBlockAlarmUpsOvertemperatureObjectType,
                            ccu.ObjBlockAlarmUpsOvertemperatureId);

                        SaveOnOffObject(
                            ccu,
                            ccu.ObjBlockAlarmUpsTamperSabotageObjectType,
                            ccu.ObjBlockAlarmUpsTamperSabotageId);

                        SaveOnOffObject(
                            ccu,
                            ccu.ObjBlockAlarmFuseOnExtensionBoardObjectType,
                            ccu.ObjBlockAlarmFuseOnExtensionBoardId);

                        SaveAlarmTransmitter(ccu.AlarmTransmitter);

                        if (ccu.CcuAlarmArcs != null)
                            foreach (var ccuAlarmArc in ccu.CcuAlarmArcs)
                                SaveAlarmArc(ccuAlarmArc.AlarmArc);
                    }
                    catch
                    {
                    }
                }

                private void SaveAntiPassBackZone(AntiPassBackZone antiPassBackZone)
                {
                    if (!_ccuDataReplicator.UpdateObjectInfo(
						antiPassBackZone,
                        _cacheOfParentCcuIdsForObjects,
                        _referencedObjectsCache))
                    {
                        return;
                    }

                    var destinationApbzAfterTimeout = antiPassBackZone.DestinationAPBZAfterTimeout;

                    if (destinationApbzAfterTimeout != null)
                        SaveAntiPassBackZone(destinationApbzAfterTimeout);
                }

                private void SaveMultiDoor(
                    CCU ccu,
                    MultiDoor multiDoor)
                {
                    if (!_ccuDataReplicator.UpdateObjectInfo(
						multiDoor,
                        _cacheOfParentCcuIdsForObjects,
                        _referencedObjectsCache))
                    {
                        return;
                    }

                    if (multiDoor.Doors == null)
                        return;

                    foreach (var multiDoorElement in multiDoor.Doors)
                        SaveMultiDoorElement(
                            ccu,
                            multiDoorElement);
                }

                private void SaveMultiDoorElement(
                    CCU ccu,
                    MultiDoorElement multiDoorElement)
                {
                    if (!_ccuDataReplicator.UpdateObjectInfo(
						multiDoorElement,
                        _cacheOfParentCcuIdsForObjects,
                        _referencedObjectsCache))
                    {
                        return;
                    }

                    if (multiDoorElement.BlockOnOffObjectType != null
                        && multiDoorElement.BlockOnOffObjectType != ObjectType.Input
                        && multiDoorElement.BlockOnOffObjectType != ObjectType.Output
                        && multiDoorElement.BlockOnOffObjectId != null)
                    {
                        SaveOnOffObject(
                            ccu,
                            (byte)multiDoorElement.BlockOnOffObjectType.Value,
                            multiDoorElement.BlockOnOffObjectId);
                    }
                }

                private void SaveDevicesAlarmSetting(
                    CCU ccu,
                    DevicesAlarmSetting devicesAlarmSetting)
                {
                    if (ccu == null
                        || devicesAlarmSetting == null)
                    {
                        return;
                    }

                    if (!_ccuDataReplicator.UpdateObjectInfo(
						devicesAlarmSetting,
                        _cacheOfParentCcuIdsForObjects,
                        _referencedObjectsCache))
                    {
                        return;
                    }

                    SaveOnOffObject(
                        ccu,
                        devicesAlarmSetting.ObjBlockAlarmCCUTamperSabotageObjectType,
                        devicesAlarmSetting.ObjBlockAlarmCCUTamperSabotageId);

                    SaveOnOffObject(
                        ccu,
                        devicesAlarmSetting.ObjBlockAlarmPrimaryPowerMissingObjectType,
                        devicesAlarmSetting.ObjBlockAlarmPrimaryPowerMissingId);

                    SaveOnOffObject(
                        ccu,
                        devicesAlarmSetting.ObjBlockAlarmBatteryIsLowObjectType,
                        devicesAlarmSetting.ObjBlockAlarmBatteryIsLowId);

                    SaveOnOffObject(
                        ccu,
                        devicesAlarmSetting.ObjBlockAlarmUpsOutputFuseObjectType,
                        devicesAlarmSetting.ObjBlockAlarmUpsOutputFuseId);

                    SaveOnOffObject(
                        ccu,
                        devicesAlarmSetting.ObjBlockAlarmUpsBatteryFaultObjectType,
                        devicesAlarmSetting.ObjBlockAlarmUpsBatteryFaultId);

                    SaveOnOffObject(
                        ccu,
                        devicesAlarmSetting.ObjBlockAlarmUpsBatteryFuseObjectType,
                        devicesAlarmSetting.ObjBlockAlarmUpsBatteryFuseId);

                    SaveOnOffObject(
                        ccu,
                        devicesAlarmSetting.ObjBlockAlarmUpsOvertemperatureObjectType,
                        devicesAlarmSetting.ObjBlockAlarmUpsOvertemperatureId);

                    SaveOnOffObject(
                        ccu,
                        devicesAlarmSetting.ObjBlockAlarmUpsTamperSabotageObjectType,
                        devicesAlarmSetting.ObjBlockAlarmUpsTamperSabotageId);

                    SaveOnOffObject(
                        ccu,
                        devicesAlarmSetting.ObjBlockAlarmFuseOnExtensionBoardObjectType,
                        devicesAlarmSetting.ObjBlockAlarmFuseOnExtensionBoardId);

                    SaveOnOffObject(
                        ccu,
                        devicesAlarmSetting.ObjBlockAlarmDCUOfflineObjectType,
                        devicesAlarmSetting.ObjBlockAlarmDCUOfflineId);

                    SaveOnOffObject(
                        ccu,
                        devicesAlarmSetting.ObjBlockAlarmDCUTamperSabotageObjectType,
                        devicesAlarmSetting.ObjBlockAlarmDCUTamperSabotageId);

                    SaveOnOffObject(
                        ccu,
                        devicesAlarmSetting.ObjBlockAlarmDEDoorAjarObjectType,
                        devicesAlarmSetting.ObjBlockAlarmDEDoorAjarId);

                    SaveOnOffObject(
                        ccu,
                        devicesAlarmSetting.ObjBlockAlarmDEIntrusionObjectType,
                        devicesAlarmSetting.ObjBlockAlarmDEIntrusionId);

                    SaveOnOffObject(
                        ccu,
                        devicesAlarmSetting.ObjBlockAlarmDESabotageObjectType,
                        devicesAlarmSetting.ObjBlockAlarmDESabotageId);

                    if (devicesAlarmSetting.SecurityDailyPlanForEnterToMenu != null)
                        SaveSecurityDailyPlan(devicesAlarmSetting.SecurityDailyPlanForEnterToMenu);

                    if (devicesAlarmSetting.SecurityTimeZoneForEnterToMenu != null)
                        SaveSecurityTimeZone(devicesAlarmSetting.SecurityTimeZoneForEnterToMenu);

                    SaveOnOffObject(
                        ccu,
                        devicesAlarmSetting.ObjBlockAlarmCROfflineObjectType,
                        devicesAlarmSetting.ObjBlockAlarmCROfflineId);

                    SaveOnOffObject(
                        ccu,
                        devicesAlarmSetting.ObjBlockAlarmCRTamperSabotageObjectType,
                        devicesAlarmSetting.ObjBlockAlarmCRTamperSabotageId);

                    SaveOnOffObject(
                        ccu,
                        devicesAlarmSetting.ObjBlockAlarmCrAccessDeniedObjectType,
                        devicesAlarmSetting.ObjBlockAlarmCrAccessDeniedId);

                    SaveOnOffObject(
                        ccu,
                        devicesAlarmSetting.ObjBlockAlarmCrUnknownCardObjectType,
                        devicesAlarmSetting.ObjBlockAlarmCrUnknownCardId);

                    SaveOnOffObject(
                        ccu,
                        devicesAlarmSetting.ObjBlockAlarmCrCardBlockedOrInactiveObjectType,
                        devicesAlarmSetting.ObjBlockAlarmCrCardBlockedOrInactiveId);

                    SaveOnOffObject(
                        ccu,
                        devicesAlarmSetting.ObjBlockAlarmCrInvalidPinObjectType,
                        devicesAlarmSetting.ObjBlockAlarmCrInvalidPinId);

                    SaveOnOffObject(
                        ccu,
                        devicesAlarmSetting.ObjBlockAlarmCrInvalidGinObjectType,
                        devicesAlarmSetting.ObjBlockAlarmCrInvalidGinId);

                    SaveOnOffObject(
                        ccu,
                        devicesAlarmSetting.ObjBlockAlarmCrInvalidEmergencyCodeObjectType,
                        devicesAlarmSetting.ObjBlockAlarmCrInvalidEmergencyCodeId);

                    SaveOnOffObject(
                        ccu,
                        devicesAlarmSetting.ObjBlockAlarmCrAccessPermittedObjectType,
                        devicesAlarmSetting.ObjBlockAlarmCrAccessPermittedId);

                    SaveOnOffObject(
                        ccu,
                        devicesAlarmSetting.ObjBlockAlarmInvalidPinRetriesLimitReachedObjectType,
                        devicesAlarmSetting.ObjBlockAlarmInvalidPinRetriesLimitReachedId);

                    SaveOnOffObject(
                        ccu,
                        devicesAlarmSetting.ObjBlockAlarmInvalidGinRetriesLimitReachedObjectType,
                        devicesAlarmSetting.ObjBlockAlarmInvalidGinRetriesLimitReachedId);

                    SaveOnOffObject(
                        ccu,
                        devicesAlarmSetting.ObjBlockAlarmAreaSetByOnOffObjectFailedObjectType,
                        devicesAlarmSetting.ObjBlockAlarmAreaSetByOnOffObjectFailedId);

                    if (devicesAlarmSetting.DevicesAlarmSettingAlarmArcs != null)
                    {
                        foreach (
                            var devicesAlarmSettingAlarmArc in
                                devicesAlarmSetting.DevicesAlarmSettingAlarmArcs)
                        {
                            SaveAlarmArc(devicesAlarmSettingAlarmArc.AlarmArc);
                        }
                    }
                }

                private void SaveDCU(
                    CCU ccu,
                    DCU dcu,
                    HashSet<Guid> hashSetAllCCUCardReaders)
                {
                    if (!_ccuDataReplicator.UpdateObjectInfo(
						dcu,
                        _cacheOfParentCcuIdsForObjects,
                        _referencedObjectsCache))
                    {
                        return;
                    }

                    if (dcu.CardReaders != null)
                        foreach (var cardReader in dcu.CardReaders)
                        {
                            SaveCardReader(
                                ccu,
                                cardReader);

                            hashSetAllCCUCardReaders.Add(cardReader.IdCardReader);
                        }

                    if (dcu.Inputs != null)
                        foreach (var input in dcu.Inputs)
                            SaveInput(
                                ccu,
                                input);

                    if (dcu.Outputs != null)
                    {
                        foreach (var output in dcu.Outputs)
                            SaveOutput(
                                ccu,
                                output);
                    }

                    if (dcu.DoorEnvironments != null)
                        foreach (var doorEnvironment in dcu.DoorEnvironments)
                            SaveDoorEnvironment(
                                ccu,
                                doorEnvironment);

                    SaveOnOffObject(
                        ccu,
                        dcu.ObjBlockAlarmOfflineObjectType,
                        dcu.ObjBlockAlarmOfflineId);

                    SaveOnOffObject(
                        ccu,
                        dcu.ObjBlockAlarmTamperObjectType,
                        dcu.ObjBlockAlarmTamperId);

                    if (dcu.DcuAlarmArcs == null)
                        return;

                    foreach (var dcuAlarmArc in dcu.DcuAlarmArcs)
                        SaveAlarmArc(dcuAlarmArc.AlarmArc);
                }

                private void SaveDoorEnvironment(
                    CCU ccu,
                    DoorEnvironment doorEnvironment)
                {
                    if (doorEnvironment == null
                        || ccu == null)
                    {
                        return;
                    }

                    if (!_ccuDataReplicator.UpdateObjectInfo(
						doorEnvironment,
                        _cacheOfParentCcuIdsForObjects,
                        _referencedObjectsCache))
                    {
                        return;
                    }

                    SaveOnOffObject(
                        ccu,
                        doorEnvironment.ObjBlockAlarmDoorAjarObjectType,
                        doorEnvironment.ObjBlockAlarmDoorAjarId);

                    SaveOnOffObject(
                        ccu,
                        doorEnvironment.ObjBlockAlarmIntrusionObjectType,
                        doorEnvironment.ObjBlockAlarmIntrusionId);

                    SaveOnOffObject(
                        ccu,
                        doorEnvironment.ObjBlockAlarmSabotageObjectType,
                        doorEnvironment.ObjBlockAlarmSabotageId);

                    if (doorEnvironment.DoorEnvironmentAlarmArcs == null)
                        return;

                    foreach (var doorEnvironmentAlarmArc in doorEnvironment.DoorEnvironmentAlarmArcs)
                        SaveAlarmArc(doorEnvironmentAlarmArc.AlarmArc);
                }

                private static bool SaveAlarmAreaToCCU(
                    AlarmArea alarmArea,
                    CCU ccu)
                {
                    if (ccu == null
                        || alarmArea == null)
                    {
                        return false;
                    }

                    var implicitCCU =
                        CCUConfigurationHandler.Singleton.GetImplicitCCUForAlarmArea(
                            alarmArea.IdAlarmArea);

                    return ccu.Compare(implicitCCU);
                }

                private void SaveACLSetting(ACLSetting aclSetting)
                {
                    if (_ccuDataReplicator.UpdateObjectInfo(
							aclSetting,
                            _cacheOfParentCcuIdsForObjects,
                            _referencedObjectsCache)
                        && aclSetting.TimeZone != null)
                    {
                        SaveTimeZone(aclSetting.TimeZone);
                    }
                }

                private void SaveAccessControlList(
                    CCU ccu,
                    AccessControlList accessControlList,
                    HashSet<Guid> hashSetAllCCUCardReaders)
                {
                    if (!_ccuDataReplicator.UpdateObjectInfo(
						accessControlList,
                        _cacheOfParentCcuIdsForObjects,
                        _referencedObjectsCache))
                    {
                        return;
                    }

                    IList<FilterSettings> filterSettings =
                        new List<FilterSettings>();

                    var filterSetting =
                        new FilterSettings(
                            ACLPerson.COLUMNACCESSCONTROLLIST,
                            accessControlList,
                            ComparerModes.EQUALL);

                    filterSettings.Add(filterSetting);

                    var listACLPerson =
                        ACLPersons.Singleton.SelectByCriteria(filterSettings);

                    if (listACLPerson != null)
                        foreach (var aclPerson in listACLPerson)
                            SaveACLPerson(aclPerson);

                    if (accessControlList.ACLSettings != null)
                        foreach (var aclSetting in accessControlList.ACLSettings)
                            if (SaveACLSettingToCCU(
                                ccu,
                                hashSetAllCCUCardReaders,
                                aclSetting))
                            {
                                SaveACLSetting(aclSetting);
                            }

                    if (accessControlList.ACLSettingAAs != null)
                        foreach (var aclSettingAA in accessControlList.ACLSettingAAs)
                            if (SaveACLSettingAAToCCU(
                                ccu,
                                hashSetAllCCUCardReaders,
                                aclSettingAA))
                            {
                                SaveACLSettingAA(
                                    ccu,
                                    aclSettingAA);
                            }
                }

                private void SaveACLSettingAA(
                    CCU ccu,
                    ACLSettingAA aclSettingAA)
                {
                    if (aclSettingAA.AlarmArea != null
                        && SaveAlarmAreaToCCU(
                            aclSettingAA.AlarmArea,
                            ccu))
                    {
                        if (_ccuDataReplicator.UpdateObjectInfo(
							aclSettingAA,
                            _cacheOfParentCcuIdsForObjects,
                            _referencedObjectsCache))
                        {
                            SaveAlarmArea(
                                ccu,
                                aclSettingAA.AlarmArea);
                        }
                    }
                }

                private void SaveAlarmArea(
                    CCU ccu,
                    AlarmArea alarmArea)
                {
                    var actAlarmArea =
                        AlarmAreas.Singleton
                            .GetById(alarmArea.IdAlarmArea);

                    if (actAlarmArea == null)
                        return;

                    if (!_ccuDataReplicator.UpdateObjectInfo(
						actAlarmArea,
                        _cacheOfParentCcuIdsForObjects,
                        _referencedObjectsCache))
                    {
                        return;
                    }

                    if (actAlarmArea.AACardReaders != null)
                        foreach (var aaCardReader in actAlarmArea.AACardReaders)
                        {
                            var cardReader = aaCardReader.CardReader;

                            if (cardReader != null
                                && (ccu.Compare(cardReader.CCU) ||
                                    (cardReader.DCU != null
                                     && ccu.Compare(cardReader.DCU.CCU))))
                            {
                                _ccuDataReplicator.UpdateObjectInfo(
									aaCardReader,
                                    _cacheOfParentCcuIdsForObjects,
                                    _referencedObjectsCache);
                            }
                        }

                    if (actAlarmArea.AAInputs != null)
                        foreach (var aaInput in actAlarmArea.AAInputs)
                        {
                            var input = aaInput.Input;

                            if ((input.CCU != null && ccu.Compare(input.CCU))
                                || (input.DCU != null && ccu.Compare(input.DCU.CCU)))
                            {
                                SaveAAInput(aaInput);
                            }
                        }

                    if (actAlarmArea.ObjForAutomaticAct != null
                        && OnOffObjectShouldBeStoredOnCcu(
                            ccu.IdCCU,
                            actAlarmArea.ObjForAutomaticAct))
                    {
                        SaveOnOffObject(
                            ccu,
                            actAlarmArea.ObjForAutomaticAct);
                    }

                    if (actAlarmArea.ObjForForcedTimeBuying != null)
                        SaveOnOffObject(
                            ccu,
                            actAlarmArea.ObjForForcedTimeBuying);

                    SaveOnOffObject(
                        ccu,
                        actAlarmArea.ObjBlockAlarmAreaSetByOnOffObjectFailedObjectType,
                        actAlarmArea.ObjBlockAlarmAreaSetByOnOffObjectFailedId);

                    if (actAlarmArea.AlarmAreaAlarmArcs != null)
                        foreach (var alarmAreaAlarmArc in actAlarmArea.AlarmAreaAlarmArcs)
                            SaveAlarmArc(alarmAreaAlarmArc.AlarmArc);
                }

                private void SaveACLPerson(ACLPerson aclPerson)
                {
                    if (aclPerson == null)
                        return;

                    if (_ccuDataReplicator.UpdateObjectInfo(
						aclPerson,
                        _cacheOfParentCcuIdsForObjects,
                        _referencedObjectsCache))
                    {
                        SavePerson(aclPerson.Person);
                    }
                }

                private void SaveAccessZone(AccessZone accessZone)
                {
                    if (!_ccuDataReplicator.UpdateObjectInfo(
						accessZone,
                        _cacheOfParentCcuIdsForObjects,
                        _referencedObjectsCache))
                    {
                        return;
                    }

                    SavePerson(accessZone.Person);

                    if (accessZone.Person != null
                        && accessZone.TimeZone != null)
                    {
                        SaveTimeZone(accessZone.TimeZone);
                    }
                }

                private void SavePerson(Person person)
                {
                    if (person == null)
                        return;

                    if (!_ccuDataReplicator.UpdateObjectInfo(
						person,
                        _cacheOfParentCcuIdsForObjects,
                        _referencedObjectsCache))
                    {
                        return;
                    }

                    if (person.Cards != null)
                        foreach (var card in person.Cards)
                            SaveCard(card);
                }

                private void SaveCard(Card card)
                {
                    if (!_ccuDataReplicator.UpdateObjectInfo(
						card,
                        _cacheOfParentCcuIdsForObjects,
                        _referencedObjectsCache))
                    {
                        return;
                    }

                    if (card.CardSystem != null)
                        _ccuDataReplicator.UpdateObjectInfo(
							card.CardSystem,
                            _cacheOfParentCcuIdsForObjects,
                            _referencedObjectsCache);
                }

                private void SaveDailyPlan(DailyPlan dailyPlan)
                {
                    _ccuDataReplicator.UpdateObjectInfo(
						dailyPlan,
                        _cacheOfParentCcuIdsForObjects,
                        _referencedObjectsCache);
                }

                private void SaveSecurityDailyPlan(SecurityDailyPlan securityDailyPlan)
                {
                    _ccuDataReplicator.UpdateObjectInfo(
						securityDailyPlan,
                        _cacheOfParentCcuIdsForObjects,
                        _referencedObjectsCache);
                }

                private void SaveTimeZone(TimeZone timeZone)
                {
                    if (_ccuDataReplicator.UpdateObjectInfo(
						timeZone,
                        _cacheOfParentCcuIdsForObjects,
                        _referencedObjectsCache))
                    {
                        if (timeZone.Calendar != null)
                        {
                            if (_ccuDataReplicator.UpdateObjectInfo(
								timeZone.Calendar,
                                _cacheOfParentCcuIdsForObjects,
                                _referencedObjectsCache))
                            {
                                if (timeZone.Calendar.DateSettings != null)
                                {
                                    foreach (var dateSetting in timeZone.Calendar.DateSettings)
                                    {
                                        if (_ccuDataReplicator.UpdateObjectInfo(
											dateSetting,
                                            _cacheOfParentCcuIdsForObjects,
                                            _referencedObjectsCache))
                                        {
                                            if (dateSetting.DayType != null)
                                                _ccuDataReplicator.UpdateObjectInfo(
													dateSetting.DayType,
                                                    _cacheOfParentCcuIdsForObjects,
                                                    _referencedObjectsCache);
                                        }
                                    }
                                }
                            }
                        }

                        if (timeZone.DateSettings != null)
                        {
                            foreach (var dateSetting in timeZone.DateSettings)
                            {
                                if (_ccuDataReplicator.UpdateObjectInfo(
									dateSetting,
                                    _cacheOfParentCcuIdsForObjects,
                                    _referencedObjectsCache))
                                {
                                    if (dateSetting.DayType != null)
                                        _ccuDataReplicator.UpdateObjectInfo(
											dateSetting.DayType,
                                            _cacheOfParentCcuIdsForObjects,
                                            _referencedObjectsCache);

                                    if (dateSetting.DailyPlan != null)
                                        SaveDailyPlan(dateSetting.DailyPlan);
                                }
                            }
                        }
                    }
                }

                private void SaveSecurityTimeZone(SecurityTimeZone securityTimeZone)
                {
                    if (_ccuDataReplicator.UpdateObjectInfo(
						securityTimeZone,
                        _cacheOfParentCcuIdsForObjects,
                        _referencedObjectsCache))
                    {
                        if (securityTimeZone.Calendar != null)
                        {
                            if (_ccuDataReplicator.UpdateObjectInfo(
								securityTimeZone.Calendar,
                                _cacheOfParentCcuIdsForObjects,
                                _referencedObjectsCache))
                            {
                                if (securityTimeZone.Calendar.DateSettings != null)
                                {
                                    foreach (var dateSetting in securityTimeZone.Calendar.DateSettings)
                                    {
                                        if (_ccuDataReplicator.UpdateObjectInfo(
											dateSetting,
                                            _cacheOfParentCcuIdsForObjects,
                                            _referencedObjectsCache))
                                        {
                                            if (dateSetting.DayType != null)
                                                _ccuDataReplicator.UpdateObjectInfo(
													dateSetting.DayType,
                                                    _cacheOfParentCcuIdsForObjects,
                                                    _referencedObjectsCache);
                                        }
                                    }
                                }
                            }
                        }

                        if (securityTimeZone.DateSettings != null)
                        {
                            foreach (var dateSetting in securityTimeZone.DateSettings)
                            {
                                if (_ccuDataReplicator.UpdateObjectInfo(
									dateSetting,
                                    _cacheOfParentCcuIdsForObjects,
                                    _referencedObjectsCache))
                                {
                                    if (dateSetting.DayType != null)
                                        _ccuDataReplicator.UpdateObjectInfo(
											dateSetting.DayType,
                                            _cacheOfParentCcuIdsForObjects,
                                            _referencedObjectsCache);

                                    if (dateSetting.SecurityDailyPlan != null)
                                        SaveSecurityDailyPlan(dateSetting.SecurityDailyPlan);
                                }
                            }
                        }
                    }
                }

                private void SaveInput(
                    CCU ccu,
                    Input input)
                {
                    var actInput = Inputs.Singleton.GetById(input.IdInput);

                    if (actInput != null)
                    {
                        if (_ccuDataReplicator.UpdateObjectInfo(
							input,
                            _cacheOfParentCcuIdsForObjects,
                            _referencedObjectsCache))
                        {
                            if (actInput.OnOffObject != null
                                && OnOffObjectShouldBeStoredOnCcu(
                                    ccu.IdCCU,
                                    actInput.OnOffObject))
                            {
                                SaveOnOffObject(
                                    ccu,
                                    actInput.OnOffObject);
                            }
                        }
                    }
                }

                private void SaveAAInput(AAInput aaInput)
                {
                    _ccuDataReplicator.UpdateObjectInfo(
						aaInput,
                        _cacheOfParentCcuIdsForObjects,
                        _referencedObjectsCache);
                }

                private void SaveOutput(
                    CCU ccu,
                    Output output)
                {
                    var actOutput = Outputs.Singleton.GetById(output.IdOutput);

                    if (actOutput != null)
                    {
                        if (_ccuDataReplicator.UpdateObjectInfo(
                            actOutput,
                            _cacheOfParentCcuIdsForObjects,
                            _referencedObjectsCache))
                        {
                            if (actOutput.OnOffObject != null
                                && OnOffObjectShouldBeStoredOnCcu(
                                    ccu.IdCCU,
                                    actOutput.OnOffObject))
                            {
                                SaveOnOffObject(
                                    ccu,
                                    actOutput.OnOffObject);
                            }
                        }
                    }
                }

                private void SaveCardReader(
                    CCU ccu,
                    CardReader cardReader)
                {
                    if (cardReader == null)
                        return;

                    if (!_ccuDataReplicator.UpdateObjectInfo(
						cardReader,
                        _cacheOfParentCcuIdsForObjects,
                        _referencedObjectsCache))
                    {
                        return;
                    }

                    if (cardReader.SecurityTimeZone != null)
                        SaveSecurityTimeZone(cardReader.SecurityTimeZone);

                    if (cardReader.SecurityDailyPlan != null)
                        SaveSecurityDailyPlan(cardReader.SecurityDailyPlan);

                    if (cardReader.SecurityDailyPlanForEnterToMenu != null)
                        SaveSecurityDailyPlan(cardReader.SecurityDailyPlanForEnterToMenu);

                    if (cardReader.SecurityTimeZoneForEnterToMenu != null)
                        SaveSecurityTimeZone(cardReader.SecurityTimeZoneForEnterToMenu);

                    if (cardReader.OnOffObject != null)
                        SaveOnOffObject(
                            ccu,
                            cardReader.OnOffObject);

                    //save access zone

                    var listAccessZone =
                        AccessZones.Singleton.SelectByCriteria(
                            new[]
                            {
                            new FilterSettings(
                                AccessZone.COLUMNGUIDCARDREADEROBJECT,
                                cardReader.IdCardReader,
                                ComparerModes.EQUALL),
                            new FilterSettings(
                                ACLSetting.COLUMN_CARD_READER_OBJECT_TYPE,
                                (byte)ObjectType.CardReader,
                                ComparerModes.EQUALL)
                            });

                    if (listAccessZone != null)
                        foreach (var accessZone in listAccessZone)
                            SaveAccessZone(accessZone);

                    SaveOnOffObject(
                        ccu,
                        cardReader.ObjBlockAlarmOfflineObjectType,
                        cardReader.ObjBlockAlarmOfflineId);

                    SaveOnOffObject(
                        ccu,
                        cardReader.ObjBlockAlarmTamperObjectType,
                        cardReader.ObjBlockAlarmTamperId);

                    SaveOnOffObject(
                        ccu,
                        cardReader.ObjBlockAlarmAccessDeniedObjectType,
                        cardReader.ObjBlockAlarmAccessDeniedId);

                    SaveOnOffObject(
                        ccu,
                        cardReader.ObjBlockAlarmUnknownCardObjectType,
                        cardReader.ObjBlockAlarmUnknownCardId);

                    SaveOnOffObject(
                        ccu,
                        cardReader.ObjBlockAlarmCardBlockedOrInactiveObjectType,
                        cardReader.ObjBlockAlarmCardBlockedOrInactiveId);

                    SaveOnOffObject(
                        ccu,
                        cardReader.ObjBlockAlarmInvalidPinObjectType,
                        cardReader.ObjBlockAlarmInvalidPinId);

                    SaveOnOffObject(
                        ccu,
                        cardReader.ObjBlockAlarmInvalidGinObjectType,
                        cardReader.ObjBlockAlarmInvalidGinId);

                    SaveOnOffObject(
                        ccu,
                        cardReader.ObjBlockAlarmInvalidEmergencyCodeObjectType,
                        cardReader.ObjBlockAlarmInvalidEmergencyCodeId);

                    SaveOnOffObject(
                        ccu,
                        cardReader.ObjBlockAlarmAccessPermittedObjectType,
                        cardReader.ObjBlockAlarmAccessPermittedId);

                    SaveOnOffObject(
                        ccu,
                        cardReader.ObjBlockAlarmInvalidGinRetriesLimitReachedObjectType,
                        cardReader.ObjBlockAlarmInvalidGinRetriesLimitReachedId);

                    if (cardReader.FunctionKey1 != null)
                        SaveOnOffObject(
                            ccu,
                            cardReader.FunctionKey1.IsUsedTimeZone
                                ? (byte)ObjectType.TimeZone
                                : (byte)ObjectType.DailyPlan,
                            cardReader.FunctionKey1.IdTimeZoneOrDailyPlan);

                    if (cardReader.FunctionKey2 != null)
                        SaveOnOffObject(
                            ccu,
                            cardReader.FunctionKey2.IsUsedTimeZone
                                ? (byte)ObjectType.TimeZone
                                : (byte)ObjectType.DailyPlan,
                            cardReader.FunctionKey2.IdTimeZoneOrDailyPlan);

                    if (cardReader.CardReaderAlarmArcs != null)
                        foreach (var cardReaderAlarmArc in cardReader.CardReaderAlarmArcs)
                            SaveAlarmArc(cardReaderAlarmArc.AlarmArc);
                }

                /// <summary>
                /// Check all access control list and save reference for the CCU
                /// </summary>
                /// <param name="ccu"></param>
                /// <param name="hashSetAllCCUCardReaders"></param>
                private void SaveAccessControlLists(
                    CCU ccu,
                    HashSet<Guid> hashSetAllCCUCardReaders)
                {
                    if (ccu == null
                        || hashSetAllCCUCardReaders == null
                        || hashSetAllCCUCardReaders.Count <= 0)
                    {
                        return;
                    }

                    var accessControlLists = AccessControlLists.Singleton.List();

                    if (accessControlLists == null)
                        return;

                    foreach (var accessControlList in accessControlLists)
                        if (SaveAccessControlListToCCU(
                            ccu,
                            hashSetAllCCUCardReaders,
                            accessControlList))
                        {
                            SaveAccessControlList(
                                ccu,
                                accessControlList,
                                hashSetAllCCUCardReaders);
                        }
                }

                /// <summary>
                /// Verify that send this access control list to the CCU
                /// </summary>
                /// <param name="ccu"></param>
                /// <param name="hashSetAllCCUCardReaders"></param>
                /// <param name="accessControlList"></param>
                /// <returns></returns>
                private static bool SaveAccessControlListToCCU(
                    CCU ccu,
                    HashSet<Guid> hashSetAllCCUCardReaders,
                    AccessControlList accessControlList)
                {
                    if (ccu == null
                        || hashSetAllCCUCardReaders == null
                        || hashSetAllCCUCardReaders.Count <= 0
                        || accessControlList == null)
                    {
                        return false;
                    }

                    if (accessControlList.ACLSettings != null)
                        foreach (var aclSetting in accessControlList.ACLSettings)
                            if (SaveACLSettingToCCU(
                                ccu,
                                hashSetAllCCUCardReaders,
                                aclSetting))
                            {
                                return true;
                            }

                    if (accessControlList.ACLSettingAAs != null)
                        foreach (var aclSettingAA in accessControlList.ACLSettingAAs)
                            if (SaveACLSettingAAToCCU(
                                ccu,
                                hashSetAllCCUCardReaders,
                                aclSettingAA))
                            {
                                return true;
                            }

                    return false;
                }

                /// <summary>
                /// Verify that send this ACLSetting to the CCU
                /// </summary>
                /// <param name="ccu"></param>
                /// <param name="hashSetAllCCUCardReaders"></param>
                /// <param name="aclSetting"></param>
                /// <returns></returns>
                private static bool SaveACLSettingToCCU(
                    CCU ccu,
                    HashSet<Guid> hashSetAllCCUCardReaders,
                    ACLSetting aclSetting)
                {
                    if (ccu == null
                        || hashSetAllCCUCardReaders == null
                        || hashSetAllCCUCardReaders.Count <= 0
                        || aclSetting == null)
                    {
                        return false;
                    }

                    Guid? parentCcuId;

                    switch (aclSetting.CardReaderObjectType)
                    {
                        case (byte)ObjectType.CardReader:

                            var cardReader =
                                CardReaders.Singleton.GetById(aclSetting.GuidCardReaderObject);
                            return hashSetAllCCUCardReaders.Contains(cardReader.IdCardReader);

                        case (byte)ObjectType.DoorEnvironment:

                            var doorEnvironment =
                                DoorEnvironments.Singleton.GetById(aclSetting.GuidCardReaderObject);

                            return HasDoorEnvironmentCardReaderFromThisList(
                                doorEnvironment,
                                hashSetAllCCUCardReaders);

                        case (byte)ObjectType.AlarmArea:

                            var alarmArea =
                                AlarmAreas.Singleton.GetById(aclSetting.GuidCardReaderObject);

                            return HasAlarmAreaCardReaderFromThisList(
                                alarmArea,
                                hashSetAllCCUCardReaders);

                        case (byte)ObjectType.DCU:

                            var dcu =
                                DCUs.Singleton.GetById(aclSetting.GuidCardReaderObject);

                            return dcu != null && ccu.Compare(dcu.CCU);

                        case (byte)ObjectType.MultiDoor:

                            parentCcuId =
                                MultiDoors.Singleton.GetParentCcuId(
                                    aclSetting.GuidCardReaderObject);

                            return ccu.IdCCU == parentCcuId;

                        case (byte)ObjectType.MultiDoorElement:

                            parentCcuId =
                                MultiDoorElements.Singleton
                                    .GetParentCcuId(aclSetting.GuidCardReaderObject);

                            return ccu.IdCCU == parentCcuId;

                        case (byte)ObjectType.Floor:

                            var parentCcuIds =
                                Floors.Singleton
                                    .GetParentCcusId(aclSetting.GuidCardReaderObject);

                            return parentCcuIds != null &&
                                   parentCcuIds.Contains(ccu.IdCCU);
                    }

                    return false;
                }

                /// <summary>
                /// Verify that send this ACLSettingAA to the CCU
                /// </summary>
                /// <param name="ccu"></param>
                /// <param name="hashSetAllCCUCardReaders"></param>
                /// <param name="aclSettingAA"></param>
                /// <returns></returns>
                private static bool SaveACLSettingAAToCCU(
                    CCU ccu,
                    HashSet<Guid> hashSetAllCCUCardReaders,
                    ACLSettingAA aclSettingAA)
                {
                    if (ccu != null
                        && hashSetAllCCUCardReaders != null
                        && hashSetAllCCUCardReaders.Count > 0
                        &&
                        aclSettingAA != null
                        && aclSettingAA.AlarmArea != null)
                    {
                        var implicitCCU =
                            CCUConfigurationHandler.Singleton.GetImplicitCCUForAlarmArea(
                                aclSettingAA.AlarmArea.IdAlarmArea);

                        if (ccu.Compare(implicitCCU))
                        {
                            return true;
                        }
                    }

                    return false;
                }

                private void SaveAccesZonessObjectTypeNotCardReader(
                    CCU ccu,
                    HashSet<Guid> hashSetAllCCUCardReaders)
                {
                    if (ccu == null)
                        return;

                    IList<FilterSettings> filterSettings = new List<FilterSettings>();
                    var filterSetting = new FilterSettings(
                        AccessZone.COLUMNCARDREADEROBJECTTYPE,
                        (byte)ObjectType.CardReader,
                        ComparerModes.NOTEQUALL);
                    filterSettings.Add(filterSetting);

                    var listAccessZone = AccessZones.Singleton.SelectByCriteria(filterSettings);
                    if (listAccessZone != null)
                    {
                        foreach (var accessZone in listAccessZone)
                        {
                            if (accessZone.CardReaderObjectType == (byte)ObjectType.DoorEnvironment)
                            {
                                var doorEnvironment =
                                    DoorEnvironments.Singleton.GetById(accessZone.GuidCardReaderObject);
                                if (HasDoorEnvironmentCardReaderFromThisList(
                                    doorEnvironment,
                                    hashSetAllCCUCardReaders))
                                {
                                    SaveAccessZone(accessZone);
                                }
                            }
                            else
                                if (accessZone.CardReaderObjectType == (byte)ObjectType.AlarmArea)
                            {
                                var alarmArea =
                                    AlarmAreas.Singleton.GetById(accessZone.GuidCardReaderObject);
                                if (HasAlarmAreaCardReaderFromThisList(
                                    alarmArea,
                                    hashSetAllCCUCardReaders))
                                {
                                    SaveAccessZone(accessZone);
                                }
                            }
                            else
                                    if (accessZone.CardReaderObjectType == (byte)ObjectType.DCU)
                            {
                                var dcu = DCUs.Singleton.GetById(
                                    accessZone.GuidCardReaderObject);
                                if (dcu != null)
                                {
                                    if (ccu.Compare(dcu.CCU))
                                        SaveAccessZone(accessZone);
                                }
                            }
                            else
                                        if (accessZone.CardReaderObjectType
                                            == (byte)ObjectType.MultiDoor)
                            {
                                var ccuId =
                                    MultiDoors.Singleton.GetParentCcuId(
                                        accessZone.GuidCardReaderObject);
                                if (ccu.IdCCU == ccuId)
                                {
                                    SaveAccessZone(accessZone);
                                }
                            }
                            else
                                            if (accessZone.CardReaderObjectType
                                                == (byte)ObjectType.MultiDoorElement)
                            {
                                var ccuId =
                                    MultiDoorElements.Singleton.GetParentCcuId(
                                        accessZone.GuidCardReaderObject);
                                if (ccu.IdCCU == ccuId)
                                {
                                    SaveAccessZone(accessZone);
                                }
                            }
                            else
                                                if (accessZone.CardReaderObjectType
                                                    == (byte)ObjectType.Floor)
                            {
                                var ccusId =
                                    Floors.Singleton.GetParentCcusId(
                                        accessZone.GuidCardReaderObject);
                                if (ccusId != null
                                    &&
                                    ccusId.Contains(ccu.IdCCU))
                                {
                                    SaveAccessZone(accessZone);
                                }
                            }
                        }
                    }
                }

                private void SaveOnOffObject(
					CCU ccu,
					AOnOffObject onOffObject)
                {
                    var dailyPlan = onOffObject as DailyPlan;

                    if (dailyPlan != null)
                    {
                        SaveDailyPlan(dailyPlan);
                        return;
                    }

                    var timeZone = onOffObject as TimeZone;

                    if (timeZone != null)
                    {
                        SaveTimeZone(timeZone);
                        return;
                    }

                    var input = onOffObject as Input;

                    if (input != null)
                    {
                        SaveInput(
                            ccu,
                            input);
                        return;
                    }

                    var output = onOffObject as Output;

                    if (output != null)
                        SaveOutput(
                            ccu,
                            output);
                }

                private void SaveOnOffObject(
                    CCU ccu,
                    byte? onOffObjectType,
                    Guid? onOffObjectGuid)
                {
                    if (onOffObjectType == null
                        || onOffObjectGuid == null)
                        return;

                    switch (onOffObjectType.Value)
                    {
                        case (byte)ObjectType.DailyPlan:
                            var dailyPlan = DailyPlans.Singleton.GetById(onOffObjectGuid.Value);

                            if (dailyPlan != null)
                            {
                                SaveDailyPlan(dailyPlan);
                            }
                            break;
                        case (byte)ObjectType.TimeZone:
                            var timeZone = TimeZones.Singleton.GetById(onOffObjectGuid.Value);

                            if (timeZone != null)
                            {
                                SaveTimeZone(timeZone);
                            }
                            break;
                        case (byte)ObjectType.Input:
                            var input = Inputs.Singleton.GetById(onOffObjectGuid.Value);

                            if (input != null)
                            {
                                SaveInput(
                                    ccu,
                                    input);
                            }
                            break;
                        case (byte)ObjectType.Output:
                            var output = Outputs.Singleton.GetById(onOffObjectGuid.Value);

                            if (output != null)
                            {
                                SaveOutput(
                                    ccu,
                                    output);
                            }
                            break;
                    }
                }

                private void SaveAlarmTransmitter(AlarmTransmitter alarmTransmitter)
                {
                    if (alarmTransmitter == null)
                        return;

                    _ccuDataReplicator.UpdateObjectInfo(
						alarmTransmitter,
                        _cacheOfParentCcuIdsForObjects,
                        _referencedObjectsCache);
                }

                private void SaveAlarmArc(AlarmArc alarmArc)
                {
                    _ccuDataReplicator.UpdateObjectInfo(
						alarmArc,
                        _cacheOfParentCcuIdsForObjects,
                        _referencedObjectsCache);
                }
            }
        }
    }
}
