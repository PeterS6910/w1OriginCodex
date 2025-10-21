using System;
using System.Collections.Generic;

using Contal.Cgp.Globals;
using System.IO;
using Contal.Cgp.NCAS.CCU.EventParameters;
using Contal.Drivers.CardReader;
using Contal.IwQuick;
using Contal.IwQuick.Data;
using Contal.IwQuick.Sys;
using Contal.LwSerialization;
using Contal.Drivers.LPC3250;

namespace Contal.Cgp.NCAS.CCU
{
    public sealed class CardSystemData : 
        ASingleton<CardSystemData>,
        DB.IDbObjectChangeListener<DB.CardSystem>
    {
        //private const string CS_DATA_PATH = "NandFlash\\CCU\\Temp\\";
        private const string CS_DATA_FILENAME = "CardReaderSectorData.dat";

        private const long WAITDELAY = 180000;

        private readonly SyncDictionary<Guid, byte[]> _csSecurityData = 
            new SyncDictionary<Guid, byte[]>();

        private readonly Dictionary<Guid, CardReaderSectorData> _cardReaderSecRedData = 
            new Dictionary<Guid, CardReaderSectorData>();

        private byte? _encoding;

        private CardSystemData()
            : base(null)
        {
        }

        public CRSectorDataEncoding Encoding
        {
            get
            {
                if (_encoding != null)
                {
                    return (CRSectorDataEncoding)((byte)_encoding);
                }
                return CRSectorDataEncoding.BCD;
            }
        }

        public void CardReaderToOnlineState(Guid idCardReader)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () =>
                    string.Format(
                        "void CardSystemData.CardReaderToOnlineState(Guid guidCardReader): [{0}]",
                        Log.GetStringFromParameters(idCardReader)));

            if (_csSecurityData.Count == 0)
            {
                //Delete Card system data from card reader
                CardReaders.Singleton.RewriteCardReaderSectorReading(
                    idCardReader,
                    null);

                return;
            }

            if (_csSecurityData.Count != 0
                && _encoding != null)
            {
                CardReaders.Singleton.SetCardReaderEncoding(
                    idCardReader,
                    (CRSectorDataEncoding)_encoding);
            }

            lock (_cardReaderSecRedData)
            {
                CardReaderSectorData crData;
                if (_cardReaderSecRedData.TryGetValue(
                    idCardReader,
                    out crData))
                {
                    crData.ActState = CrActualState.AfterOnline;
                    crData.StartInited();
                }
                else
                {
                    crData = new CardReaderSectorData();
                    crData.StartInited();
                    _cardReaderSecRedData.Add(
                        idCardReader,
                        crData);
                }
            }

            NativeTimerManager.StartTimeout(
                WAITDELAY,
                idCardReader,
                ReInitQueryStamp,
                (byte)PrirotyForOnTimerEvent.CardReaders);

            CardReaders.Singleton.SendActualQueryDbStamp(idCardReader);
        }

        private bool ReInitQueryStamp(NativeTimer timerCarrier)
        {
            var idCardReader = (Guid)timerCarrier.Data;
            CardReaderSectorData crData;
            if (_cardReaderSecRedData.TryGetValue(idCardReader, out crData))
            {
                if (!crData.WasInited)
                {
                    NativeTimerManager.StartTimeout(
                        WAITDELAY,
                        idCardReader,
                        ReInitQueryStamp,
                        (byte)PrirotyForOnTimerEvent.CardReaders);

                    CardReaders.Singleton.SendActualQueryDbStamp(idCardReader);
                }
            }
            return true;
        }

        public void CardReaderCSDataResponse(
            Guid idCardReader,
            byte[] stamp,
            CardReader cardReader)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("void CardSystemData.CardReaderCSDataResponse(Guid idCardReader, byte[] stamp): [{0}]",
                Log.GetStringFromParameters(idCardReader, stamp)));
            CardReaderSectorData crData;
            if (_cardReaderSecRedData.TryGetValue(idCardReader, out crData))
            {
                crData.QueryDbStampAnswered();
                switch (crData.ActState)
                {
                    case CrActualState.AfterOnline:
                        if (IsCardReaderActual(
                            idCardReader,
                            stamp,
                            cardReader))
                        {
                            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "CardReaderCSDataResponse Cr is Actual");
                            crData.ActState = CrActualState.None;
                        }
                        else
                        {
                            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "CardReaderCSDataResponse Cr need to rewrite");
                            crData.ActState = CrActualState.Validate;

                            CardReaders.Singleton.CardReaderValidateCardSystem(
                                idCardReader, 
                                ValidateCardSystem());
                        }
                        break;
                    case CrActualState.AfterWrite:
                        CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "CardReaderCSDataResponse AfterWrite");
                        SaveNewCardReaderStamp(idCardReader, stamp);
                        break;
                    case CrActualState.Validate:
                        CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "CardReaderCSDataResponse start writing");
                        crData.ActState = CrActualState.AfterWrite;
                        SetCardSystemToCardReader(idCardReader, stamp);
                        break;
                }
            }
        }

        //public event Contal.IwQuick.Action<Guid, byte> eventSetCr;
        public void SetCardSystemToCardReader(Guid idCardReader, byte[] stamp)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("void CardSystemData.SetCardSystemToCardReader(Guid idCardReader, byte[] stamp): [{0}]",
                Log.GetStringFromParameters(idCardReader, stamp)));
            if (_csSecurityData.Count == 0)
            {
                SaveNewCardReaderStamp(idCardReader, stamp);
                return;
            }

            if (_encoding != null)
            {
                var sectorDataEncoding = (CRSectorDataEncoding)_encoding;
                CardReaders.Singleton.SetCardReaderEncoding(idCardReader, sectorDataEncoding);
            }

            ICollection<byte[]> securityDataCollection = _csSecurityData.ValuesSnapshot;
            if (securityDataCollection.Count == 0)
                return;

            foreach (var cypherData in securityDataCollection)
            {
                CardReaders.Singleton.SetCardSystem(idCardReader, cypherData);
            }
        }

        private bool IsCardReaderActual(
            Guid idCardReader,
            byte[] stamp,
            CardReader cardReader)
        {
            if (cardReader.HardwareVersion == CRHWVersion.SmartSiedle
                && cardReader.FirmwareMainVersion == 56)
            {
                return false;
            }

            CardReaderSectorData crData;
            if (_cardReaderSecRedData.TryGetValue(idCardReader, out crData))
            {
                if (crData.CompareStamp(stamp))
                {
                    return true;
                }
            }

            return false;
        }

        public void SaveNewCardReaderStamp(Guid idCardReader, byte[] stamp)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL,
                () =>
                    string.Format("void CardSystemData.SaveNewCardReaderStamp(Guid idCardReader, byte[] stamp): [{0}]",
                        Log.GetStringFromParameters(idCardReader, stamp)));

            lock (_cardReaderSecRedData)
            {
                CardReaderSectorData crData;
                if (_cardReaderSecRedData.TryGetValue(idCardReader, out crData))
                {
                    if (!crData.CompareStamp(stamp))
                        crData.SetActStamp(stamp);
                }
                else
                {
                    _cardReaderSecRedData.Add(idCardReader, new CardReaderSectorData(stamp));
                }
            }
            SaveDictionaryCrStampToFile();
        }

        private void LoadCardSystemsSecurityData()
        {
            _csSecurityData.Clear();
            var listGuidCs = Database.ConfigObjectsEngine.GetPrimaryKeysForObjectType(ObjectType.CardSystem);
            if (listGuidCs != null)
            {
                foreach (var guidCs in listGuidCs)
                {
                    var cs =
                        Database.ConfigObjectsEngine.GetFromDatabase(
                            ObjectType.CardSystem, 
                            guidCs) as DB.CardSystem;

                    if (cs == null
                        || cs.SmartCardDataForCCU == null)
                    {
                        continue;
                    }

                    if (cs.CardType != (byte)DB.CardType.Mifare
                        || (cs.CardSubType != (byte)DB.CardSubType.MifareStandardSectorReadin
                            && cs.CardSubType != (byte)DB.CardSubType.MifareSectorReadinWithMAD)
                        || cs.SmartCardDataForCCU.Length <= 1)
                    {
                        continue;
                    }

                    var smartDataBytes = new List<byte>(cs.SmartCardDataForCCU);

                    var encoding = smartDataBytes[0];
                    var cypherData = smartDataBytes.GetRange(1, smartDataBytes.Count - 1).ToArray();

                    _csSecurityData.Add(cs.IdCardSystem, cypherData);
                    _encoding = encoding;
                }
            }
        }

        private void LoadDictionaryCrStampFromFile()
        {
            MemoryStream memoryStream = null;
            Stream inputStream = null;

            try
            {
                if (!File.Exists(CcuCore.Singleton.RootPath + CcuCore.TEMP + CS_DATA_FILENAME))
                    return;

                inputStream = 
                    PatchedFileStream.Open(
                        CcuCore.Singleton.RootPath + CcuCore.TEMP + CS_DATA_FILENAME,
                        FileMode.Open, 
                        FileAccess.Read, 
                        FileShare.Read);

                memoryStream = new MemoryStream();
                var buffer = new byte[512];

                do
                {
                    int length;

                    try
                    {
                        length = inputStream.Read(buffer, 0, buffer.Length);
                    }
                    catch (Exception error)
                    {
                        CcuCore.Singleton.SaveEventObjectDeserializeFailed(
                            Guid.Empty,
                            ObjectType.NotSupport,
                            "CardSystemData - LoadDictionaryCrStampFromFile",
                            error.Message);

                        throw;
                    }

                    if (length == 0)
                        break;

                    memoryStream.Write(buffer, 0, length);
                }
                while (true);

                if (memoryStream.Length == 0)
                    return;

                memoryStream.Seek(0, SeekOrigin.Begin);

                Dictionary<Guid, byte[]> actualTimeStamp;

                try
                {
                    actualTimeStamp =
                        (new LwBinaryDeserializer<Dictionary<Guid, byte[]>>(memoryStream))
                            .Deserialize();
                }
                catch (Exception error)
                {
                    CcuCore.Singleton.SaveEventObjectDeserializeFailed(
                        Guid.Empty, 
                        ObjectType.NotSupport, 
                        "CardSystemData - LoadDictionaryCrStampFromFile", 
                        error.Message);

                    throw;
                }

                if (actualTimeStamp == null)
                    return;

                lock (_cardReaderSecRedData)
                {
                    _cardReaderSecRedData.Clear();

                    foreach (var kvp in actualTimeStamp)
                        if (kvp.Value != null)
                            _cardReaderSecRedData.Add(
                                kvp.Key, 
                                new CardReaderSectorData(kvp.Value));
                }
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
            finally
            {
                if (memoryStream != null)
                    try
                    {
                        memoryStream.Close();
                    }
                    catch (Exception error)
                    {
                        HandledExceptionAdapter.Examine(error);
                    }

                if (inputStream != null)
                    try
                    {
                        inputStream.Close();
                    }
                    catch (Exception error)
                    {
                        HandledExceptionAdapter.Examine(error);
                    }
            }
        }

        private readonly object _lockSaveDictionary = new object();

        private void SaveDictionaryCrStampToFile()
        {
            lock (_lockSaveDictionary)
            {
                MemoryStream memoryStream = null;
                Stream outputStream = null;

                try
                {
                    var actualTimeStamps = new Dictionary<Guid, byte[]>();

                    lock (_cardReaderSecRedData)
                        foreach (var kvp in _cardReaderSecRedData)
                            if (kvp.Value != null && kvp.Value.ActStamp != null)
                                actualTimeStamps.Add(
                                    kvp.Key,
                                    kvp.Value.ActStamp);

                    if (!Directory.Exists(CcuCore.Singleton.RootPath + CcuCore.TEMP))
                        Directory.CreateDirectory(CcuCore.Singleton.RootPath + CcuCore.TEMP);

                    memoryStream = new MemoryStream();

                    (new LwBinarySerializer<Dictionary<Guid, byte[]>>(memoryStream))
                        .Serialize(actualTimeStamps);

                    outputStream =
                        PatchedFileStream.Open(
                            string.Format(
                                "{0}{1}{2}",
                                CcuCore.Singleton.RootPath,
                                CcuCore.TEMP,
                                CS_DATA_FILENAME),
                            FileMode.Create,
                            FileAccess.Write,
                            FileShare.Read);

                    var buffer = new byte[512];

                    memoryStream.Seek(0, SeekOrigin.Begin);

                    do
                    {
                        int length;

                        try
                        {
                            length = memoryStream.Read(buffer, 0, buffer.Length);
                        }
                        catch (Exception error)
                        {
                            CcuCore.DebugLog.Warning(
                                Log.PERFORMANCE_LEVEL,
                                () =>
                                    string.Format(
                                        "CardSystemData: SaveDictionaryCrStampToFile - Tolerated file problem on : {0}, exception: {1}",
                                        CcuCore.Singleton.RootPath + CcuCore.TEMP + CS_DATA_FILENAME,
                                        error));

                            throw;
                        }

                        if (length == 0)
                            break;

                        outputStream.Write(buffer, 0, length);
                    }
                    while (true);
                }
                catch (Exception error)
                {
                    HandledExceptionAdapter.Examine(error);
                    CcuCore.DebugLog.Error("CS: " + error);
                }
                finally
                {
                    if (memoryStream != null)
                        try
                        {
                            memoryStream.Close();
                        }
                        catch (Exception error)
                        {
                            HandledExceptionAdapter.Examine(error);
                        }

                    if (outputStream != null)
                        try
                        {
                            outputStream.Close();
                        }
                        catch (Exception error)
                        {
                            HandledExceptionAdapter.Examine(error);
                        }
                }
            }
        }

        public void CardSystemChanged(ICollection<Guid> changedCardSystemGuids)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL, 
                () => string.Format(
                    "void CardSystemData.CardSystemChanged(IList<Guid> changedCardSystemGuids): [{0}]",
                    Log.GetStringFromParameters(changedCardSystemGuids)));

            var needRewrite = false;

            foreach (var id in changedCardSystemGuids)
            {
                var cs = Database.ConfigObjectsEngine.GetFromDatabase(ObjectType.CardSystem, id) as DB.CardSystem;
                if (cs != null)
                {
                    byte[] cypherData;
                    if (_csSecurityData.TryGetValue(cs.IdCardSystem, out cypherData))
                    {
                        if (cs.SmartCardDataForCCU == null)
                        {
                            needRewrite = true;
                            break;
                        }
                        if (!HaveSameCypherData(cypherData, cs.SmartCardDataForCCU))
                        {
                            needRewrite = true;
                            break;
                        }
                    }
                    else
                    {
                        if (cs.SmartCardDataForCCU != null)
                        {
                            needRewrite = true;
                            break;
                        }
                    }
                }
            }

            if (needRewrite)
            {
                lock (_cardReaderSecRedData)
                {
                    _cardReaderSecRedData.Clear();
                }

                SaveDictionaryCrStampToFile();
                LoadCardSystemsSecurityData();
                RewriteAllCardReaderSecurityData();
            }
        }

        private void UnconfigureCardSystem(Guid idDeletedCardSystem)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("void CardSystemData.DeletedCardSystem(Guid idDeletedCardSystem): [{0}]", Log.GetStringFromParameters(idDeletedCardSystem)));
            byte[] cypherData;

            if (_csSecurityData.TryGetValue(idDeletedCardSystem, out cypherData))
            {
                lock (_cardReaderSecRedData)
                {
                    _cardReaderSecRedData.Clear();
                }

                _csSecurityData.Remove(idDeletedCardSystem);
                RewriteAllCardReaderSecurityData();
            }

            Events.ProcessEvent(
                new EventSectorCardSystemRemoved(
                    idDeletedCardSystem));
        }

        public void DeleteCardReaderSecRedData(Guid idCardReader)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL,
                () =>
                    string.Format("void CardSystemData.DeleteCardReaderSecRedData(Guid idCardReader): [{0}]",
                        Log.GetStringFromParameters(idCardReader)));

            lock (_cardReaderSecRedData)
            {
                _cardReaderSecRedData.Remove(idCardReader);
            }
        }

        private void RewriteAllCardReaderSecurityData()
        {
            var listGuidCr = Database.ConfigObjectsEngine.GetPrimaryKeysForObjectType(ObjectType.CardReader);
            if (listGuidCr == null) return;
            var validCs = ValidateCardSystem();

            foreach (var idCardReader in listGuidCr)
            {
                try
                {
                    var cr =
                        Database.ConfigObjectsEngine.GetFromDatabase(ObjectType.CardReader, idCardReader) as
                            DB.CardReader;
                    
                    if (cr != null)
                    {
                        lock (_cardReaderSecRedData)
                        {
                            CardReaderSectorData crData;
                            if (_cardReaderSecRedData.TryGetValue(cr.IdCardReader, out crData))
                            {
                                crData.ActState = CrActualState.Validate;
                            }
                            else
                            {
                                _cardReaderSecRedData.Add(cr.IdCardReader,
                                    new CardReaderSectorData(CrActualState.Validate));
                            }
                        }

                        if (_encoding != null)
                        {
                            CardReaders.Singleton.SetCardReaderEncoding(
                                idCardReader, 
                                (CRSectorDataEncoding) _encoding);
                        }

                        CardReaders.Singleton.RewriteCardReaderSectorReading(
                            idCardReader,
                            validCs.Length != 0
                                ? validCs
                                : null);
                    }
                }
                catch (Exception error)
                {
                    HandledExceptionAdapter.Examine(error);
                }
            }
        }

        //mifare sector smard data consists of 1 byte (encoding) and the rest cypher data bytes
        private static bool HaveSameCypherData(byte[] cypherData, byte[] smartCardData)
        {
            if (cypherData.Length != smartCardData.Length - 1)
                return false;
            for (var i = 0; i < cypherData.Length; i++)
            {
                if (cypherData[i] != smartCardData[i + 1])
                    return false;
            }
            return true;
        }

        private byte[] ValidateCardSystem()
        {
            try
            {
                var listCsN = new List<byte>();

                ICollection<byte[]> securityDataCollection = _csSecurityData.ValuesSnapshot;
                if (securityDataCollection.Count > 0)
                {
                    foreach (var cypherData in securityDataCollection)
                    {
                        listCsN.Add(cypherData[0]);
                        listCsN.Add(cypherData[1]);
                        listCsN.Add(cypherData[2]);
                    }
                }

                CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "Still valid Card Systems:");
                foreach (var csN in listCsN)
                {
                    CcuCore.DebugLog.Info(csN.ToString());
                }
                return listCsN.ToArray();
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
                CcuCore.DebugLog.Error("catch ValidataCardSystem");
                return null;
            }

        }

        public void PrepareObjectUpdate(
            Guid idObject,
            DB.CardSystem newObject)
        {
            UnconfigureCardSystem(idObject);
        }

        public void OnObjectSaved(
            Guid idObject,
            DB.CardSystem newObject)
        {
        }

        public void PrepareObjectDelete(Guid idObject)
        {
            UnconfigureCardSystem(idObject);
        }

        public void Init()
        {
            LoadDictionaryCrStampFromFile();
            LoadCardSystemsSecurityData();
        }
    }

    public class CardReaderSectorData
    {
        byte[] _actStamp;
        CrActualState _actState = CrActualState.None;
        byte _actCsWrited;
        bool _wasInited;

        public CrActualState ActState { get { return _actState; } set { _actState = value; } }
        public byte[] ActStamp { get { return _actStamp; } }
        public byte ActCsWrited { get { return _actCsWrited; } set { _actCsWrited = value; } }
        public bool WasInited { get { return _wasInited; } }

        public CardReaderSectorData()
        {
            _actState = CrActualState.AfterOnline;
            _actCsWrited = 0;
        }

        public CardReaderSectorData(byte[] stamp)
        {
            _actStamp = stamp;
            _actState = CrActualState.None;
            _actCsWrited = 0;
        }

        public CardReaderSectorData(CrActualState crActState)
        {
            _actState = crActState;
            _actCsWrited = 0;
        }

        public void StartInited()
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "void CardReaderSectorData.StartInited()");
            _wasInited = false;
        }

        public void QueryDbStampAnswered()
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "void CardReaderSectorData.QueryDbStampAnswered()");
            _wasInited = true;
        }

        public void SetActStamp(byte[] actStamp)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("void CardReaderSectorData.SetActStamp(byte[] actStamp): [{0}]", Log.GetStringFromParameters(actStamp)));
            _actStamp = actStamp;
        }

        public bool CompareStamp(byte[] actStamp)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("bool CardReaderSectorData.CompareStamp(byte[] actStamp): [{0}]", Log.GetStringFromParameters(actStamp)));
            if (actStamp != null && _actStamp != null && actStamp.Length == _actStamp.Length)
            {
                if (actStamp.Length > 0 && actStamp[0] == 0xFF)
                {
                    CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "void CardReaderSectorData.CompareStamp return false[1]");
                    return false;
                }
                for (var i = 0; i < actStamp.Length; i++)
                {
                    if (actStamp[i] != _actStamp[i])
                    {
                        CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "void CardReaderSectorData.CompareStamp return false[2]");
                        return false;
                    }
                }

                CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "void CardReaderSectorData.CompareStamp return true");
                return true;
            }

            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "void CardReaderSectorData.CompareStamp return false[3]");
            return false;
        }
    }

    public enum CrActualState : byte
    {
        None = 0,
        AfterOnline = 1,
        AfterWrite = 2,
        Validate = 3,
        ActStamp = 4
    }
}
