using System;
using System.Collections.Generic;
using System.Globalization;

using Contal.Cgp.NCAS.CCU.EventParameters;
using Contal.Cgp.NCAS.Definitions;
using Contal.Drivers.CardReader;
using Contal.Drivers.LPC3250;
using Contal.IwQuick;
using Contal.IwQuick.Sys;
using Contal.IwQuick.Sys.Microsoft;

using Microsoft.Win32;

using CardReader = Contal.Drivers.CardReader.CardReader;

namespace Contal.Cgp.NCAS.CCU
{
    public static class CcuCardReaders
    {
        public const string IMPLICIT_CCUx_CR_SERIAL_PORT = "COM4";
        public const string IMPLICIT_CAT12CE_CR_SERIAL_PORT = "COM6";

        public const int MinimalPinLength = 4;
        public const int MaximalPinLength = 12;

        private static readonly string _serialPortName;
        public const string PROCESSING_QUEUE_NAME_SEND_ONLINE_STATE = "CardReaders: send online state";
        public const string PROCESSING_QUEUE_NAME_SET_ONLINE_IN_DICTIONARY = "CardReaders: set online in dictionary";

        private const string CardReaderMinimalCodeLengthRegistryKey = "CardReaderMinimalCodeLength";
        private const string CardReaderMaximalCodeLengthRegistyKey = "CardReaderMaximalCodeLength";

        private const string IsPinConfirmationObligatoryRegistryKey = "IsPinConfirmationObligatory";

        private static readonly CRLineConfiguration _crLineConfiguration;

        private static bool _allowPINCachingInMenu;

        private static readonly CardReader[] _precreatedCardReaders;

        public static bool AllowPINCaching
        {
            get
            {
                return _allowPINCachingInMenu;
            }
        }

        public static int MinimalCodeLength { get; private set; }

        public static int MaximalCodeLength { get; private set; }

        public static bool IsPinConfirmationObligatory { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        static CcuCardReaders()
        {
            CrCommunicator = new CRDirectCommunicator(null);

            CrCommunicator.OnlineStateChanged += OnOnlineStateChanged;
            CrCommunicator.UpgradeResult+=OnUpgradeResult;
            CrCommunicator.UpgradeProgress+=OnUpgradeProgress;

            _precreatedCardReaders = new CardReader[CRConstants.MaxCrCount];

            for (int i = 0; i < CRConstants.MaxCrCount; i++)
                _precreatedCardReaders[i] = new CardReader((byte)(i));

            switch ((MainBoardVariant)MainBoard.Variant)
            {

                case MainBoardVariant.CAT12CE:

                    _serialPortName = IMPLICIT_CAT12CE_CR_SERIAL_PORT;
                    break;

                case MainBoardVariant.CCU05:
                case MainBoardVariant.CCU12:
                case MainBoardVariant.CCU40:

                    _serialPortName = IMPLICIT_CCUx_CR_SERIAL_PORT;
                    break;

                default:

                    _serialPortName = IMPLICIT_CCUx_CR_SERIAL_PORT;
                    break;
            }

            _crLineConfiguration = new CRLineConfiguration(_serialPortName);

            GetRegistryAllowPINCachingInCardReaderMenu();

            GetRegistryAllowCodeLength();

            GetPinConfirmationObligatoryFromRegistry();
        }

        private static void OnUpgradeProgress(CardReader cr, int progress)
        {
            Events.ProcessEvent(
                new EventCrUpgradePercentageSet(
                    -1,
                    cr.Address,
                    progress));
        }

        private static void OnUpgradeResult(CardReader cr, CRUpgradeResult result, Exception error)
        {
            Events.ProcessEvent(
                new EventCrUpgradeResultSet(
                    -1,
                    cr.Address,
                    (byte)result));
        }



        public static void InitCardReaders(int countCR)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL, 
                () => String.Format(
                    "void CardReaders.InitCardReaders(int countCR): [{0}]", 
                    Log.GetStringFromParameters(countCR)));

            _countCr = countCR;

            try
            {
                if (_countCr <= 0 || CRConstants.MaxCrCount < _countCr)
                {
                    _countCr = CRConstants.MaxCrCount;
                    CcuCore.ValidateMaxCrCount(ref _countCr);
                }

                var cardReaderSet = new CardReader[_countCr];

                for (var i = 0; i < cardReaderSet.Length; i++)
                    cardReaderSet[i] = 
                        _countCr < CRConstants.MaxCrCount
                            ? _precreatedCardReaders[i + 1]
                            : _precreatedCardReaders[i];

                CrCommunicator.Configure(
                    cardReaderSet, 
                    _crLineConfiguration);
            }
            catch (Exception ex)
            {
                HandledExceptionAdapter.Examine(ex);
                Console.WriteLine(ex.ToString());
            }
        }

        private static volatile bool _started;
        private static readonly object _startedSync = new object();

        private static int _countCr;

        public static void StartCRCommunicator()
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL, 
                "void CardReaders.StartCRCommunicators()");

            lock (_startedSync)
            {
                if (!_started)
                {
                    if (EnabledComPort())
                    {
                        CrCommunicator.Start();
                        _started = true;
                    }
                }
            }
        }

        private static bool EnabledComPort()
        {
            var mbv = (MainBoardVariant)MainBoard.Variant;

            if (mbv != MainBoardVariant.CCU0_ECHELON &&
                mbv != MainBoardVariant.CCU0_RS485)
                return true;

            var ccu = Ccus.Singleton.GetCCU();
            return ccu != null && ccu.EnabledComPort;
        }

        public static void StopCRCommunicator()
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "void CardReaders.StopCRCommunicators()");

            lock (_startedSync)
            {
                if (_started)
                {
                    _started = false;
                    CrCommunicator.Stop();

                    CardReaders.Singleton.OnDirectCrCommunicatorStopped();
                }
            }
        }

        private static void OnOnlineStateChanged(CardReader cardReader, bool isOnline)
        {
            try
            {
                CcuCore.DebugLog.Info(
                    Log.NORMAL_LEVEL,
                    () => string.Format(
                        "CR - {0}: {1} online: {2}",
                        _serialPortName,
                        cardReader != null
                            ? cardReader.Address
                            : -1,
                        isOnline));

                if (!CardReaders.Singleton.OnOnlineStateChanged(
                    Guid.Empty,
                    cardReader))
                {
                    if (cardReader == null)
                        return;

                    Events.ProcessEvent(
                        new EventCardReaderOnlineStateChanged(
                            cardReader.IsOnline,
                            -1,
                            SerialPortName,
                            cardReader.Address,
                            cardReader.ProtocolVersion,
                            cardReader.FirmwareVersion,
                            ((byte)cardReader.HardwareVersion).ToString(
                                CultureInfo.InvariantCulture),
                            cardReader.ProtocolVersionHigh));
                }
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
                CcuCore.DebugLog.Error(_serialPortName + " online state error.");
            }
        }

        public static CRDirectCommunicator CrCommunicator
        {
            get;
            private set;
        }

        public static IEnumerable<CardReader> OnlineCardReaders
        {
            get
            {
                if (CrCommunicator == null)
                    yield break;

                for (int cardReaderIdx = 0;
                    cardReaderIdx < _countCr;
                    ++cardReaderIdx)
                {
                    var cardReader =
                        _precreatedCardReaders[
                            _countCr < CRConstants.MaxCrCount
                                ? cardReaderIdx + 1
                                : cardReaderIdx];

                    if (!cardReader.IsOnline)
                        continue;

                    yield return cardReader;
                }
            }
        }

        public static string SerialPortName
        {
            get { return _serialPortName; }
        }

        /// <summary>
        /// Set allow pin caching in card reader menu
        /// </summary>
        /// <param name="allowPINCaching"></param>
        public static void SetAllowPINCachingInCardReaderMenu(bool allowPINCaching)
        {
            _allowPINCachingInMenu = allowPINCaching;
            SetRegistryAllowPINCachingInCardReaderMenu();
        }

        private const string CARD_READER_REG_ALLOW_PIN_CACHING_IN_MENU = "CardReaderAllowPinCachingInMenu";

        private static void GetRegistryAllowPINCachingInCardReaderMenu()
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "CardReaders.GetRegistryAllowPINCachingInCardReaderMenu()");
            try
            {
                var registryKey = RegistryHelper.GetOrAddKey(CcuCore.REGISTRY_CCU_PATH);
                if (registryKey != null)
                {
                    var result = Convert.ToBoolean(registryKey.GetValue(CARD_READER_REG_ALLOW_PIN_CACHING_IN_MENU));
                    CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => String.Format("CardReaders.GetRegistryAllowPINCachingInCardReaderMenu set AllowPinCaching to {0}[1]", Log.GetStringFromParameters(result)));
                    _allowPINCachingInMenu = result;
                    return;
                }
            }
            catch
            {
                CcuCore.DebugLog.Warning(Log.BELOW_NORMAL_LEVEL, () => String.Format("CardReaders.GetRegistryAllowPINCachingInCardReaderMenu set AllowPinCaching to {0}[1]", Log.GetStringFromParameters(false)));
                _allowPINCachingInMenu = false;
                return;
            }

            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => String.Format("CardReaders.GetRegistryAllowPINCachingInCardReaderMenu set AllowPinCaching to {0}[2]", Log.GetStringFromParameters(false)));
            _allowPINCachingInMenu = false;
        }

        public static void SetAllowCodeLength(
            int minimalCodeLength,
            int maximalCodeLength)
        {
            MinimalCodeLength = minimalCodeLength;
            MaximalCodeLength = maximalCodeLength;

            SetRegistryAllowCodeLength();

            CardReaders.Singleton.MaximalCodeLengthChanged();
        }

        private static void GetRegistryAllowCodeLength()
        {
            try
            {
                var registryKey = RegistryHelper.GetOrAddKey(CcuCore.REGISTRY_CCU_PATH);
                if (registryKey != null)
                {
                    MinimalCodeLength = Convert.ToInt32(registryKey.GetValue(CardReaderMinimalCodeLengthRegistryKey));
                    MaximalCodeLength = Convert.ToInt32(registryKey.GetValue(CardReaderMaximalCodeLengthRegistyKey));

                    return;
                }
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }

            MinimalCodeLength = 4;
            MaximalCodeLength = 12;
        }

        private static void SetRegistryAllowPINCachingInCardReaderMenu()
        {
            try
            {
                var registryKey = RegistryHelper.GetOrAddKey(CcuCore.REGISTRY_CCU_PATH);
                if (registryKey != null)
                {
                    registryKey.SetValue(CARD_READER_REG_ALLOW_PIN_CACHING_IN_MENU, _allowPINCachingInMenu, RegistryValueKind.DWord);
                }
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        private static void SetRegistryAllowCodeLength()
        {
            try
            {
                var registryKey = RegistryHelper.GetOrAddKey(CcuCore.REGISTRY_CCU_PATH);
                if (registryKey != null)
                {
                    registryKey.SetValue(CardReaderMinimalCodeLengthRegistryKey, MinimalCodeLength, RegistryValueKind.DWord);
                    registryKey.SetValue(CardReaderMaximalCodeLengthRegistyKey, MaximalCodeLength, RegistryValueKind.DWord);
                }
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        public static void ApplyTimeSettings()
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL, 
                () => "void CardReaders.ApplyTimeSettings()");

            foreach (var cardReader in _precreatedCardReaders)
                if (cardReader.IsOnline)
                    cardReader.ApplyTimeSettings();
        }

        public static void SetPinConfirmationObligatory(bool isPinConfirmationObligatory)
        {
            IsPinConfirmationObligatory = isPinConfirmationObligatory;

            SetPinConfirmationObligatoryToRegistry();
        }

        private static void SetPinConfirmationObligatoryToRegistry()
        {
            try
            {
                var registryKey = RegistryHelper.GetOrAddKey(CcuCore.REGISTRY_CCU_PATH);
                if (registryKey != null)
                {
                    registryKey.SetValue(IsPinConfirmationObligatoryRegistryKey, IsPinConfirmationObligatory, RegistryValueKind.DWord);
                }
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        private static void GetPinConfirmationObligatoryFromRegistry()
        {
            try
            {
                var registryKey = RegistryHelper.GetOrAddKey(CcuCore.REGISTRY_CCU_PATH);
                if (registryKey != null)
                {
                    IsPinConfirmationObligatory = Convert.ToBoolean(registryKey.GetValue(
                        IsPinConfirmationObligatoryRegistryKey,
                        true));

                    return;
                }
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }

            IsPinConfirmationObligatory = true;
        }
    }
}