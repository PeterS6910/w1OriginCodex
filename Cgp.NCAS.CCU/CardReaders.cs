using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.Alarms;
using Contal.Cgp.NCAS.CCU.CardReaderMechanism;
using Contal.Cgp.NCAS.CCU.EventParameters;
using Contal.IwQuick;
using Contal.IwQuick.Data;
using Contal.Drivers.CardReader;

using JetBrains.Annotations;

using CardReader = Contal.Drivers.CardReader.CardReader;

namespace Contal.Cgp.NCAS.CCU
{
    internal sealed class CardReaders
        : AStateAndSettingsObjectCollection<
            CardReaders,
            ACardReaderSettings,
            DB.CardReader>
        , DB.IDbObjectChangeListener<DB.AACardReader>
    {
        private class IdDcuAndCrAddress : IEquatable<IdDcuAndCrAddress>
        {
            private readonly Guid _idDcu;
            private readonly byte _address;

            public IdDcuAndCrAddress(
                Guid idDcu,
                byte address)
            {
                _idDcu = idDcu;
                _address = address;
            }

            public bool Equals(IdDcuAndCrAddress other)
            {
                if (other == null)
                    return false;

                return
                    _idDcu.Equals(other._idDcu)
                    && _address.Equals(other._address);
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as IdDcuAndCrAddress);
            }

            public override int GetHashCode()
            {
                return _idDcu.GetHashCode() ^ _address.GetHashCode();
            }
        }

        private readonly SyncDictionary<Guid, Action<DoorEnvironments.DoorEnvironmentStateChangedArgs>> _accessByCardReader =
            new SyncDictionary<Guid, Action<DoorEnvironments.DoorEnvironmentStateChangedArgs>>();

        private readonly Dictionary<IdDcuAndCrAddress, ACardReaderSettings> _cardReaderSettingsByIdDcuAndCrAddress = 
            new Dictionary<IdDcuAndCrAddress, ACardReaderSettings>();

        private CardReaders() : base(null)
        {
        }

        private static ACardReaderSettings CreateCardReaderSettings(DB.CardReader cardReaderDb)
        {
            return cardReaderDb.GuidDCU != Guid.Empty
                ? (ACardReaderSettings)new DCUCardReaderSettings(cardReaderDb)
                : new CcuCardReadersSettings(cardReaderDb);
        }

        private void PerformAsyncRequest(
            Guid idCardReader,
            Action<ACardReaderSettings> requestAction)
        {
            _objects.TryGetValue(
                idCardReader,
                (key, found, value) =>
                {
                    if (found)
                        value.EnqueueAsyncRequest(requestAction);
                });
        }

        private void PerformForEachAsyncRequest(
            Action<ACardReaderSettings> requestAction)
        {
            _objects.ForEach(
                (key, value) => value.EnqueueAsyncRequest(requestAction));
        }

        public bool OnOnlineStateChanged(
            Guid idDcu,
            [NotNull]
            CardReader cardReader)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "void CardReaders.OnOnlineStateChanged(Guid idDcu, CardReader cardReader): [{0}]",
                    Log.GetStringFromParameters(idDcu, cardReader)));

            ACardReaderSettings cardReaderSettings;

            if (_cardReaderSettingsByIdDcuAndCrAddress.TryGetValue(
                new IdDcuAndCrAddress(
                    idDcu,
                    cardReader.Address), 
                out cardReaderSettings))
            {
                PerformAsyncRequest(
                    cardReaderSettings.Id,
                    value => value.OnOnlineStateChanged(cardReader));

                return true;
            }

            return false;
        }

        public void ShowBlockedDoorEnvironmentMessage(Guid idCardReader)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "void CardReaders.ShowBlockedDoorEnvironmentMessage(Guid idCardReader): [{0}]",
                    Log.GetStringFromParameters(idCardReader)));

            PerformAsyncRequest(
                idCardReader,
                cardReaderSettings => cardReaderSettings.ShowInfoBlockedByLicence());
        }

        public void DetachDoorEnvironmentAdapter(
            Guid idCardReader,
            Guid idDoorEnvironment)
        {
            PerformAsyncRequest(
                idCardReader,
                cardReaderSettings =>
                    cardReaderSettings.DetachDoorEnvironmentAdapter(idDoorEnvironment));
        }

        public void DetachMultiDoorAdapter(
            Guid idCardReader,
            Guid idMultiDoor)
        {
            PerformAsyncRequest(
                idCardReader,
                cardReaderSettings =>
                    cardReaderSettings.DetachMultiDoorAdapter(idMultiDoor));
        }

        public void SendAllStates(bool isNewlyConfigured)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => "void CardReaders.SendAllStates()");

            foreach (var cardReaderSettings in _objects.ValuesSnapshot)
            {
                var cardReader = cardReaderSettings.CardReader;

                if (cardReader == null)
                    continue;

                Events.ProcessEvent(
                    new EventCardReaderOnlineStateInfo(
                        cardReader.IsOnline,
                        cardReaderSettings.DcuLogicalAddress,
                        string.Empty,
                        cardReader.Address,
                        cardReader.ProtocolVersion,
                        cardReader.FirmwareVersion,
                        ((byte)cardReader.HardwareVersion).ToString(CultureInfo.InvariantCulture),
                        cardReader.ProtocolVersionHigh));

                cardReaderSettings.SendCurrentCardReaderCommand();

                Events.ProcessEvent(
                    new EventCardReaderBlockedStateChanged(
                        cardReaderSettings.Id,
                        cardReaderSettings.InvalidCodeRetriesLimitReached));
            }

            if (isNewlyConfigured)
                foreach (var cardReader in CcuCardReaders.OnlineCardReaders)
                    Events.ProcessEvent(
                        new EventCardReaderOnlineStateChanged(
                            true,
                            -1,
                            CcuCardReaders.SerialPortName,
                            cardReader.Address,
                            cardReader.ProtocolVersion,
                            cardReader.FirmwareVersion,
                            ((byte)cardReader.HardwareVersion).ToString(
                                CultureInfo.InvariantCulture),
                            cardReader.ProtocolVersionHigh));
        }

        public Dictionary<Guid, bool> GetCardReaderTamperState()
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => "Dictionary<Guid, bool> CardReaders.GetCardReaderTamperState()");

            var result =
                _objects.ValuesSnapshot.ToDictionary(
                    cardReaderSettings => cardReaderSettings.Id,
                    cardReaderSettings => cardReaderSettings.IsInTamper);

            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "Dictionary<Guid, bool> CardReaders.GetCardReaderTamperState return {0}",
                    Log.GetStringFromParameters(result)));

            return result;
        }

        public void SetCardSystem(Guid idCardReader, byte[] cypherData)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "void CardReaders.SetCardSystem(Guid idCardReader, byte[] cypherData): [{0}]",
                    Log.GetStringFromParameters(idCardReader, cypherData)));

            PerformAsyncRequest(
                idCardReader,
                cardReaderSettings => cardReaderSettings.SetCardSystem(cypherData));
        }

        public void SetCardReaderEncoding(Guid idCardReader, CRSectorDataEncoding encoding)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "void CardReaders.SetCardReaderEncoding(Guid idCardReader, CRSectorDataEncoding encoding): [{0}]",
                    Log.GetStringFromParameters(idCardReader, encoding)));

            PerformAsyncRequest(
                idCardReader,
                cardReaderSettings => cardReaderSettings.SetCardReaderEncoding(encoding));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idCardReader"></param>
        /// <exception cref="NullReferenceException"></exception>
        public void SendActualQueryDbStamp(Guid idCardReader)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "void CardReaders.SendActualQueryDbStamp(Guid idCardReader): [{0}]",
                    Log.GetStringFromParameters(idCardReader)));

            PerformAsyncRequest(
                idCardReader,
                cardReaderSettings => cardReaderSettings.SendActualQueryDbStamp());
        }

        public void CardReaderValidateCardSystem(Guid idCardReader, byte[] data)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "void CardReaders.CardReaderValidateCardSystem(Guid idCardReader, byte[] data): [{0}]",
                    Log.GetStringFromParameters(idCardReader, data)));

            PerformAsyncRequest(
                idCardReader,
                cardReaderSettings => cardReaderSettings.ValidateCardSystem(data));
        }

        public void RewriteCardReaderSectorReading(Guid idCardReader, byte[] validCs)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "void CardReaders.RewriteCardReaderSectorReading(Guid idCardReader, byte[] validCs): [{0}]",
                    Log.GetStringFromParameters(idCardReader, validCs)));

            PerformAsyncRequest(
                idCardReader,
                cardReaderSettings => cardReaderSettings.RewriteCardReaderSectorReading(validCs));
        }

        public void ResetCardReader(Guid idCardReader)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "void DCUs.ResetCardReader(Guid idCardReader): [{0}]",
                    Log.GetStringFromParameters(idCardReader)));

            PerformAsyncRequest(
                idCardReader,
                cardReaderSettings => cardReaderSettings.Reset());
        }

        public void OnDcuDisconnected(
            [NotNull]
            DB.DCU dcu)
        {
            if (dcu.GuidCardReaders == null)
                return;

            foreach (var idCardReader in dcu.GuidCardReaders)
                PerformAsyncRequest(
                    idCardReader,
                    cardReaderSettings => cardReaderSettings.OnOnlineStateChanged(null));
        }

        public void OnDirectCrCommunicatorStopped()
        {
            PerformForEachAsyncRequest(
                cardReaderSettings =>
                    cardReaderSettings.OnOnlineStateChanged(null));
        }

        public void ConfigureAaCardReaders(
            Guid idCardReader,
            ICollection<DB.AACardReader> aaCardReaders)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "void CardReaders.ConfigureAaCardReaders(Guid idCardReader, ICollection<DB.AACardReader> aaCardReaders): [{0}]",
                    Log.GetStringFromParameters(idCardReader, aaCardReaders)));

            PerformAsyncRequest(
                idCardReader,
                cardReaderSettings => cardReaderSettings.ConfigureAaCardReaders(aaCardReaders));
        }

        private ImplicitCrCodeParams GetCurrentImplicitCrCodeParams(
            Guid idCardReader,
            out int cardReaderAddress,
            out CardReader cardReader)
        {
            ACardReaderSettings cardReaderSettings;

            cardReaderAddress =
                _objects.TryGetValue(
                    idCardReader,
                    out cardReaderSettings)
                    ? cardReaderSettings.Address
                    : -1;

            cardReader =
                cardReaderSettings != null
                    ? cardReaderSettings.CardReader
                    : null;

            var result = 
                cardReaderSettings != null
                    ? cardReaderSettings.CurrentImplicitCrCodeParams
                    : null;

            return
                result
                ?? new ImplicitCrCodeParams(
                    cardReader != null
                        ? CRAccessCommands.DoorLockedMessage(cardReader)
                        : new CRMessage(
                            0,
                            CRMessageCode.DOOR_LOCKED,
                            3),
                    null,
                    false,
                    DB.SecurityLevel.Locked);
        }

        public ImplicitCrCodeParams GetCurrentImplicitCrCodeParams(
            Guid idCardReader,
            out int cardReaderAddress)
        {
            CardReader cardReader;

            return GetCurrentImplicitCrCodeParams(
                idCardReader,
                out cardReaderAddress,
                out cardReader);
        }

        public ImplicitCrCodeParams GetCurrentImplicitCrCodeParams(
            Guid idCardReader,
            out CardReader cardReader)
        {
            int cardReaderAddress;

            return GetCurrentImplicitCrCodeParams(
                idCardReader,
                out cardReaderAddress,
                out cardReader);
        }

        public void RegisterCRsForUpgradeVersion(
            Guid idDcu,
            byte address,
            string upgradeVersion)
        {
            ACardReaderSettings cardReaderSettings;

            if (!_cardReaderSettingsByIdDcuAndCrAddress.TryGetValue(
                new IdDcuAndCrAddress(
                    idDcu,
                    address), 
                out cardReaderSettings))
            {
                return;
            }

            RegisterCRsForUpgradeVersionInternal(
                cardReaderSettings,
                upgradeVersion);
        }

        private static void RegisterCRsForUpgradeVersionInternal(
            ACardReaderSettings cardReaderSettings,
            string upgradeVersion)
        {
            CardReader cardReader = cardReaderSettings.CardReader;

            if (cardReader != null)
                CardReaderUpgradeProcess.RegisterCardReaderForUpgrade(
                    cardReaderSettings.Id,
                    cardReader,
                    upgradeVersion);
        }

        public void RegisterCRsForUpgradeVersion(
            Guid idCardReader,
            string upgradeVersion)
        {
            ACardReaderSettings cardReaderSettings;

            if (!_objects.TryGetValue(
                idCardReader,
                out cardReaderSettings))
            {
                return;
            }

            RegisterCRsForUpgradeVersionInternal(
                cardReaderSettings,
                upgradeVersion);
        }

        public static void ConfigureAlarmsBlockingAndAlarmArcs(
            DB.CardReader cardReaderDb)
        {
            if (cardReaderDb == null)
                return;

            BlockedAlarmsManager.Singleton.ConfigureSpecificBlockingForObject(
                AlarmType.CardReader_Offline,
                new IdAndObjectType(
                    cardReaderDb.IdCardReader,
                    ObjectType.CardReader),
                cardReaderDb.AlarmOffline,
                cardReaderDb.BlockAlarmOffline,
                cardReaderDb.ObjBlockAlarmOfflineId,
                cardReaderDb.ObjBlockAlarmOfflineObjectType,
                true);

            BlockedAlarmsManager.Singleton.ConfigureSpecificBlockingForObject(
                AlarmType.CardReader_TamperSabotage,
                new IdAndObjectType(
                    cardReaderDb.IdCardReader,
                    ObjectType.CardReader),
                cardReaderDb.AlarmTamper,
                cardReaderDb.BlockAlarmTamper,
                cardReaderDb.ObjBlockAlarmTamperId,
                cardReaderDb.ObjBlockAlarmTamperObjectType,
                cardReaderDb.EventlogDuringBlockAlarmTamper);

            BlockedAlarmsManager.Singleton.ConfigureSpecificBlockingForObject(
                AlarmType.CardReader_AccessDenied,
                new IdAndObjectType(
                    cardReaderDb.IdCardReader,
                    ObjectType.CardReader),
                cardReaderDb.AlarmAccessDenied,
                cardReaderDb.BlockAlarmAccessDenied,
                cardReaderDb.ObjBlockAlarmAccessDeniedId,
                cardReaderDb.ObjBlockAlarmAccessDeniedObjectType,
                cardReaderDb.EventlogDuringBlockAlarmAccessDenied);

            BlockedAlarmsManager.Singleton.ConfigureSpecificBlockingForObject(
                AlarmType.CardReader_UnknownCard,
                new IdAndObjectType(
                    cardReaderDb.IdCardReader,
                    ObjectType.CardReader),
                cardReaderDb.AlarmUnknownCard,
                cardReaderDb.BlockAlarmUnknownCard,
                cardReaderDb.ObjBlockAlarmUnknownCardId,
                cardReaderDb.ObjBlockAlarmUnknownCardObjectType,
                cardReaderDb.EventlogDuringBlockAlarmUnknownCard);

            BlockedAlarmsManager.Singleton.ConfigureSpecificBlockingForObject(
                AlarmType.CardReader_CardBlockedOrInactive,
                new IdAndObjectType(
                    cardReaderDb.IdCardReader,
                    ObjectType.CardReader),
                cardReaderDb.AlarmCardBlockedOrInactive,
                cardReaderDb.BlockAlarmCardBlockedOrInactive,
                cardReaderDb.ObjBlockAlarmCardBlockedOrInactiveId,
                cardReaderDb.ObjBlockAlarmCardBlockedOrInactiveObjectType,
                cardReaderDb.EventlogDuringBlockAlarmCardBlockedOrInactive);

            BlockedAlarmsManager.Singleton.ConfigureSpecificBlockingForObject(
                AlarmType.CardReader_InvalidPIN,
                new IdAndObjectType(
                    cardReaderDb.IdCardReader,
                    ObjectType.CardReader),
                cardReaderDb.AlarmInvalidPIN,
                cardReaderDb.BlockAlarmInvalidPin,
                cardReaderDb.ObjBlockAlarmInvalidPinId,
                cardReaderDb.ObjBlockAlarmInvalidPinObjectType,
                cardReaderDb.EventlogDuringBlockAlarmInvalidPin);

            BlockedAlarmsManager.Singleton.ConfigureSpecificBlockingForObject(
                AlarmType.CardReader_InvalidCode,
                new IdAndObjectType(
                    cardReaderDb.IdCardReader,
                    ObjectType.CardReader),
                cardReaderDb.AlarmInvalidGIN,
                cardReaderDb.BlockAlarmInvalidGin,
                cardReaderDb.ObjBlockAlarmInvalidGinId,
                cardReaderDb.ObjBlockAlarmInvalidGinObjectType,
                cardReaderDb.EventlogDuringBlockAlarmInvalidGin);

            BlockedAlarmsManager.Singleton.ConfigureSpecificBlockingForObject(
                AlarmType.CardReader_InvalidEmergencyCode,
                new IdAndObjectType(
                    cardReaderDb.IdCardReader,
                    ObjectType.CardReader),
                cardReaderDb.AlarmInvalidEmergencyCode,
                cardReaderDb.BlockAlarmInvalidEmergencyCode,
                cardReaderDb.ObjBlockAlarmInvalidEmergencyCodeId,
                cardReaderDb.ObjBlockAlarmInvalidEmergencyCodeObjectType,
                cardReaderDb.EventlogDuringBlockAlarmInvalidEmergencyCode);

            BlockedAlarmsManager.Singleton.ConfigureSpecificBlockingForObject(
                AlarmType.CardReader_AccessPermitted,
                new IdAndObjectType(
                    cardReaderDb.IdCardReader,
                    ObjectType.CardReader),
                cardReaderDb.AlarmAccessPermitted,
                cardReaderDb.BlockAlarmAccessPermitted,
                cardReaderDb.ObjBlockAlarmAccessPermittedId,
                cardReaderDb.ObjBlockAlarmAccessPermittedObjectType,
                true);

            BlockedAlarmsManager.Singleton.ConfigureSpecificBlockingForObject(
                AlarmType.CardReader_Invalid_Code_Retries_Limit_Reached,
                new IdAndObjectType(
                    cardReaderDb.IdCardReader,
                    ObjectType.CardReader),
                cardReaderDb.AlarmInvalidGinRetriesLimitReached,
                cardReaderDb.BlockAlarmInvalidGinRetriesLimitReached,
                cardReaderDb.ObjBlockAlarmInvalidGinRetriesLimitReachedId,
                cardReaderDb.ObjBlockAlarmInvalidGinRetriesLimitReachedObjectType,
                true);

            if (cardReaderDb.AlarmTypeAndIdAlarmArcs != null)
                CatAlarmsManager.Singleton.ConfigureSpecificAlarmArcs(
                    new IdAndObjectType(
                        cardReaderDb.IdCardReader,
                        ObjectType.CardReader),
                    cardReaderDb.AlarmTypeAndIdAlarmArcs);
            else
                CatAlarmsManager.Singleton.RemoveSpecificAlarmArcsConfiguration(
                    new IdAndObjectType(
                        cardReaderDb.IdCardReader,
                        ObjectType.CardReader));
        }


        public void PrepareDoorEnvironmentAdapter(
            Guid idCardReader,
            Guid idDoorEnvironment)
        {
            PerformAsyncRequest(
                idCardReader,
                cardReaderSettings => 
                    cardReaderSettings
                        .PrepareDoorEnvironmentAdapter(idDoorEnvironment));
        }

        public void PrepareMultiDoorAdapter(
            Guid idCardReader,
            Guid idMultiDoor)
        {
            PerformAsyncRequest(
                idCardReader,
                cardReaderSettings =>
                    cardReaderSettings
                        .PrepareMultiDoorAdapter(idMultiDoor));
        }

        public void AddCardReaderAccessed(
            Guid guidCardReader,
            Action<DoorEnvironments.DoorEnvironmentStateChangedArgs> onAccessByCardReader)
        {
            if (_accessByCardReader.ContainsKey(guidCardReader))
                _accessByCardReader[guidCardReader] += onAccessByCardReader;
            else
                _accessByCardReader.Add(guidCardReader, onAccessByCardReader);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="guidCardReader"></param>
        /// <param name="onAccessByCardReader"></param>
        public void RemoveCardReaderAccessed(
            Guid guidCardReader,
            Action<DoorEnvironments.DoorEnvironmentStateChangedArgs> onAccessByCardReader)
        {
            if (!_accessByCardReader.ContainsKey(guidCardReader))
                return;

            // ReSharper disable once DelegateSubtraction
            _accessByCardReader[guidCardReader] -= onAccessByCardReader;

            if (_accessByCardReader[guidCardReader] == null)
                _accessByCardReader.Remove(guidCardReader);
        }

        public void FireAccessViaCardReader(
            DoorEnvironments.DoorEnvironmentStateChangedArgs doorEnvironmentStateChangedArgs)
        {
            Action<DoorEnvironments.DoorEnvironmentStateChangedArgs> onAccessByCardReader;

            if (_accessByCardReader.TryGetValue(
                doorEnvironmentStateChangedArgs.GuidCardReaderAccessed,
                out onAccessByCardReader))
            {
                if (onAccessByCardReader != null)
                    onAccessByCardReader(doorEnvironmentStateChangedArgs);
            }
        }

        public void EnterRootScene(Guid idCardReader)
        {
            PerformAsyncRequest(
                idCardReader,
                cardReaderSettings => cardReaderSettings.SceneContext.EnterRootScene());
        }

        public override ObjectType ObjectType
        {
            get { return ObjectType.CardReader; }
        }

        protected override ACardReaderSettings CreateNewStateAndSettingsObject(DB.CardReader dbObject)
        {
            var result = CreateCardReaderSettings(dbObject);

            _cardReaderSettingsByIdDcuAndCrAddress.Add(
                new IdDcuAndCrAddress(
                    result.CardReaderDb.GuidDCU,
                    result.Address),
                result);

            return result;
        }

        protected override void OnRemoved(ACardReaderSettings removedValue)
        {
            _cardReaderSettingsByIdDcuAndCrAddress.Remove(
                new IdDcuAndCrAddress(
                    removedValue.CardReaderDb.GuidDCU,
                    removedValue.Address));
        }

        public bool CardReaderExists(
            Guid idDcu,
            byte cardReaderAddress)
        {
            return 
                _cardReaderSettingsByIdDcuAndCrAddress
                    .ContainsKey(
                        new IdDcuAndCrAddress(
                            idDcu, 
                            cardReaderAddress));
        }

        public void StopTimeoutAndResetInvalidGinRetriesLimitReached()
        {
            PerformForEachAsyncRequest(
                cardReaderSettings =>
                {
                    if (!cardReaderSettings.InvalidCodeRetriesLimitEnabled)
                        cardReaderSettings.StopTimeoutAndResetInvalidGinRetriesLimitReached();
                });
        }

        public void StopTimeoutAndResetInvalidGinRetriesLimitReached(Guid idCardReader)
        {
            PerformAsyncRequest(
                idCardReader,
                cardReaderSettings => cardReaderSettings.StopTimeoutAndResetInvalidGinRetriesLimitReached());
        }

        public string GetKeyForCheckingRights(Guid idCardReader)
        {
            ACardReaderSettings cardReaderSettings;

            if (!_objects.TryGetValue(
                idCardReader,
                out cardReaderSettings))
            {
                return string.Empty;
            }

            return cardReaderSettings.DcuLogicalAddress != -1
                ? string.Format(
                    "{0}{1}",
                    cardReaderSettings.DcuLogicalAddress,
                    cardReaderSettings.Address)
                : cardReaderSettings.Address.ToString();

        }

        public void MaximalCodeLengthChanged()
        {
            PerformForEachAsyncRequest(
                cardReaderSettings =>
                    cardReaderSettings.MaximalCodeLengthChanged());
        }

        void DB.IDbObjectChangeListener<DB.AACardReader>.PrepareObjectUpdate(
            Guid idObject,
            DB.AACardReader newObject)
        {
            UnconfigureAaCardReader(
                idObject,
                true);
        }

        void DB.IDbObjectChangeListener<DB.AACardReader>.OnObjectSaved(
            Guid idObject,
            DB.AACardReader newObject)
        {
        }

        void DB.IDbObjectChangeListener<DB.AACardReader>.PrepareObjectDelete(Guid idObject)
        {
            UnconfigureAaCardReader(
                idObject,
                false);
        }

        private void UnconfigureAaCardReader(
            Guid idAaCardReader,
            bool update)
        {
            var aaCardReader =
                Database.ConfigObjectsEngine.GetFromDatabase(
                    ObjectType.AACardReader,
                    idAaCardReader) as DB.AACardReader;

            if (aaCardReader != null)
                PerformAsyncRequest(
                    aaCardReader.GuidCardReader,
                    cardReaderSettings =>
                        cardReaderSettings.UnconfigureAaCardReader(
                            aaCardReader,
                            update));
        }
    }
}
