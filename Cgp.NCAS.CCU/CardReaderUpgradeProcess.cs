using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

using Contal.Cgp.NCAS.CCU.EventParameters;
using Contal.Cgp.NCAS.Globals;
using Contal.Drivers.CardReader;
using Contal.Drivers.ClspDrivers;
using Contal.IwQuick;
using Contal.IwQuick.Data;
using Contal.IwQuick.Sys;

namespace Contal.Cgp.NCAS.CCU
{
    public static class CardReaderUpgradeProcess
    {
        private class CardReaderUpgrader
        {
            public ExtendedVersion UpgradeFwVersion
            {
                get;
                private set;
            }

            private readonly Dictionary<Guid, CardReader> _cardReadersById =
                new Dictionary<Guid, CardReader>();

            public byte[] CardReaderAddresses
            {
                get
                {
                    return _cardReadersById.Values
                        .Select(cardReader => cardReader.Address)
                        .ToArray();
                }
            }

            public IEnumerable<Guid> IdCardReaders
            {
                get { return _cardReadersById.Keys.ToArray(); }
            }

            public CardReaderUpgrader(ExtendedVersion upgradeFwVersion)
            {
                UpgradeFwVersion = upgradeFwVersion;
            }

            public void AddCardReader(
                Guid idCardReader,
                CardReader cardReader)
            {
                _cardReadersById.Add(
                    idCardReader,
                    cardReader);
            }

            public bool RemoveCardReader(Guid idCardReader)
            {
                _cardReadersById.Remove(idCardReader);
                return _cardReadersById.Count == 0;
            }

            public CrUpgradeFailureInfo OnUpgradePackageReceived(
                string filename,
                Stream stream,
                string crc32,
                string unpackFilePath)
            {
                var fp = new FilePacker();

                if (!fp.TryUnpack(stream, Path.GetDirectoryName(filename)))
                {
                    Log.Singleton.Error("Failed to unpack upgrade package");

                    return
                        new CrUpgradeFailureInfo(
                            UnpackPackageFailedCode.UnpackFailed,
                            CardReaderAddresses);
                }

                uint rightChecksum;

                try
                {
                    rightChecksum = UInt32.Parse(crc32);
                }
                catch (Exception error)
                {
                    HandledExceptionAdapter.Examine(error);

                    Log.Singleton.Error("Failed to get checksum from header");

                    return
                        new CrUpgradeFailureInfo(
                            UnpackPackageFailedCode.GetChecksumFailed,
                            CardReaderAddresses);
                }

                if (!CcuCore.ChecksumMatches(
                    rightChecksum,
                    unpackFilePath))
                {
                    Log.Singleton.Error("Checksum does not match");

                    return
                        new CrUpgradeFailureInfo(
                            UnpackPackageFailedCode.ChecksumDoesNotMatch,
                            CardReaderAddresses);
                }

                return null;
            }

            public void Upgrade(
                string hwVersion,
                string unpackFilePath)
            {
                var parsedHardwareVersion = CRHWVersion.Unknown;

                if (hwVersion.ToLower().StartsWith("0x"))
                    hwVersion = hwVersion.Substring(2);

                try
                {
                    parsedHardwareVersion = (CRHWVersion)Enum.Parse(
                        typeof(CRHWVersion),
                        byte.Parse(
                            hwVersion,
                            NumberStyles.HexNumber)
                            .ToString(CultureInfo.InvariantCulture),
                        true);
                }
                catch (Exception error)
                {
                    HandledExceptionAdapter.Examine(error);
                }

                foreach (var idAndCardReader in _cardReadersById)
                {
                    var cardReader = idAndCardReader.Value;

                    if ((byte)cardReader.HardwareVersion != 1
                        && cardReader.ProtocolVersion != "1.0"
                        && parsedHardwareVersion != cardReader.HardwareVersion)
                    {
                        var clspNode =
                            cardReader.ParentCommunicator.IndirectParentObject as IClspNode;

                        Events.ProcessEvent(
                            new EventCrUpgradeResultSet(
                                clspNode != null
                                    ? clspNode.LogicalAddress
                                    : -1,
                                cardReader.Address,
                                (byte)CRUpgradeResult.IncorrectCardReaderType));

                        continue;
                    }

                    cardReader.UpgradeCommands.StartUpgrade(
                        cardReader,
                        unpackFilePath);

                    var crFwVersion = new ExtendedVersion(
                        cardReader.FirmwareMainVersion,
                        cardReader.FirmwareRevision,
                        0,
                        0,
                        string.Empty);

                    CardSystemData.Singleton.DeleteCardReaderSecRedData(idAndCardReader.Key);

                    Events.ProcessEvent(
                        new EventCardReaderUpgradingState(
                            idAndCardReader.Key));
                }
            }
        }

        private static readonly IDictionary<Guid, CardReaderUpgrader> _cardReaderUpgradersById =
            new Dictionary<Guid, CardReaderUpgrader>();

        private static readonly SyncDictionary<ExtendedVersion, CardReaderUpgrader> _cardReaderUpgraders =
            new SyncDictionary<ExtendedVersion, CardReaderUpgrader>();

        public static void RegisterCardReaderForUpgrade(
            Guid idCardReader,
            CardReader cardReader,
            string upgradeVersion)
        {
            _cardReaderUpgraders.GetOrAddValue(
                new ExtendedVersion(upgradeVersion),
                key => new CardReaderUpgrader(key),
                (key, value, newlyAdded) =>
                {
                    CardReaderUpgrader previousUpgrader;

                    if (_cardReaderUpgradersById.TryGetValue(
                        idCardReader,
                        out previousUpgrader))
                    {
                        var previousUpgradeFwVersion = 
                            previousUpgrader.UpgradeFwVersion;

                        if (previousUpgradeFwVersion.Equals(key))
                            return;

                        if (previousUpgrader.RemoveCardReader(idCardReader))
                            _cardReaderUpgraders.Remove(previousUpgradeFwVersion);
                    }

                    _cardReaderUpgradersById[idCardReader] = value;

                    value.AddCardReader(
                        idCardReader, 
                        cardReader);
                });
        }

        public class CrUpgradeFailureInfo
        {
            public CrUpgradeFailureInfo(
                UnpackPackageFailedCode errorCode,
                byte[] crAddresses)
            {
                ErrorCode = errorCode;
                CrAddresses = crAddresses;
            }

            public UnpackPackageFailedCode ErrorCode
            {
                get;
                private set;
            }

            public byte[] CrAddresses
            {
                get;
                private set;
            }
        }

        public static CrUpgradeFailureInfo UpgradeCardReaders(
            string filename, 
            Stream stream,
            string[] headerInfos)
        {
            if (headerInfos == null || headerInfos.Length < 5)
            {
                Log.Singleton.Error("Upgrade package has unsupported header format");

                return 
                    new CrUpgradeFailureInfo(
                        UnpackPackageFailedCode.UnsupportedHeaderFormat, 
                        null);
            }

            string unpackFilePath =
                Path.Combine(
                    Path.GetDirectoryName(filename),
                    headerInfos[3]);

            CrUpgradeFailureInfo crUpgradeFailureInfo = null;

            _cardReaderUpgraders.Remove(
                new ExtendedVersion(headerInfos[1]),
                (ExtendedVersion key, CardReaderUpgrader value, out bool continueInRemove) =>
                {
                    crUpgradeFailureInfo = value.OnUpgradePackageReceived(
                        filename,
                        stream,
                        headerInfos[4],
                        unpackFilePath);

                    continueInRemove = crUpgradeFailureInfo == null;
                },
                (key, removed, value) =>
                {
                    if (!removed)
                        return;

                    try
                    {
                        value.Upgrade(
                            headerInfos[2], 
                            unpackFilePath);
                    }
                    catch (Exception)
                    {
                        crUpgradeFailureInfo = 
                            new CrUpgradeFailureInfo(
                                UnpackPackageFailedCode.Other, 
                                value.CardReaderAddresses);

                        return;
                    }

                    foreach (var idCardReader in value.IdCardReaders)
                        _cardReaderUpgradersById.Remove(idCardReader);
                });

            return crUpgradeFailureInfo;
        }
    }
}