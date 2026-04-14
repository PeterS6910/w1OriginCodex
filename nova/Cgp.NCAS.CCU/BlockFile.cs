using System;

using System.Collections.Generic;
using System.IO;

using Contal.Cgp.NCAS.Globals;
using Contal.IwQuick;
using Contal.Drivers.LPC3250;
using Contal.IwQuick.Sys;
 
namespace Contal.Cgp.NCAS.CCU
{
    public class FileBlock : ADisposable
    {
        private const int GUID_SIZE = 16;
        public const int HEADER_SIZE = 6 + GUID_SIZE;

        private short _length = 0;
        internal int _nextBlockIndex = -1; // no need for long, as this number is multiplied by BLOCK_SIZE in byte-position
        internal byte[] _data = null;
        internal Guid _objectGuid = Guid.Empty;

        public FileBlock(byte[] data, short length, int nextBlockIndex)
        {
            _data = data;
            _length = length;
            _nextBlockIndex = nextBlockIndex;
            _objectGuid = Guid.Empty;
        }

        public FileBlock(byte[] data, short length, int nextBlockIndex, Guid objectGuid)
        {
            _data = data;
            _length = length;
            _nextBlockIndex = nextBlockIndex;
            _objectGuid = objectGuid;
        }

        public static int GetMaxDataSize(int blockSize)
        {
            if (blockSize > HEADER_SIZE)
            {
                return blockSize - HEADER_SIZE;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal byte[] ToByte(bool headerOnly, int blockSize)
        {
            try
            {
                byte[] data = new byte[headerOnly ? HEADER_SIZE : blockSize];
                BitConverter.GetBytes(_length).CopyTo(data, 0);
                BitConverter.GetBytes(_nextBlockIndex).CopyTo(data, 2);
                byte[] guidBytes = _objectGuid.ToByteArray();
                guidBytes.CopyTo(data, 6);

                if (!headerOnly && _data != null)
                    _data.CopyTo(data, HEADER_SIZE);

                return data;
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal byte[] ToByte(int blockSize)
        {
            return ToByte(false, blockSize);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputData"></param>
        /// <returns></returns>
        public static FileBlock FromByte(byte[] inputData, int blockSize)
        {
            if (inputData.Length != blockSize)
                return null;

            try
            {
                short length = BitConverter.ToInt16(inputData, 0);
                if (length > GetMaxDataSize(blockSize) || length < 0) // this most possibly means corruption of data
                    return null;

                int nextBlock = BitConverter.ToInt32(inputData, 2);
                byte[] guidBytes = new byte[GUID_SIZE];
                Array.Copy(inputData, 6, guidBytes, 0, guidBytes.Length);
                Guid objectGuid = new Guid(guidBytes);

                byte[] data = new byte[length];

                Array.Copy(inputData, HEADER_SIZE, data, 0, length);

                return new FileBlock(data, length, nextBlock, objectGuid);
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetAsFree()
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("void FileBlock({0}).SetAsFree()", this));
            _length = 0;
            _data = null;
            _objectGuid = Guid.Empty;
        }

        public bool IsFree()
        {
            CcuCore.DebugLog.Info(Log.DEBUG_LEVEL, () => string.Format("bool FileBlock({0}).IsFree()", this));
            bool result = _length == 0;
            CcuCore.DebugLog.Info(Log.DEBUG_LEVEL, () => string.Format("bool FileBlock({0}).IsFree return {1}", this, result));

            return result;
        }

        protected override void InternalDispose(bool isExplicitDispose)
        {
            _data = null;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class BlockFile : CcuCore.IBlockFileInfoProvider
    {
        private const int DELAY_TO_FLUSH_FILE = 5000;
        public const int FILE_STREAM_BUFFER_SIZE = 512; //8192;
        private const int DEFAULT_BLOCK_SIZE = 8192;
        public const int ACCESS_BLOCK_SIZE = 512;

        private int _blockSize;
        private string _filePath = string.Empty;
        public string FileName
        {
            get { return _filePath; }
            set { _filePath = value; }
        }

        private int _firstFreeBlockIndex = 0;
        private List<int> _deletedBlocks = new List<int>();

        private volatile Stream _fileStream = null;
        private volatile object _filestreamLock = new object();

        private bool _wasChanged = false;
        private bool _runSwapToFile = true;
        private NativeTimer _timerFlushFile = null;
        private bool _runFlushFile = true;
        private object _lockForWriteDelete = new object();

        public int DeletedBlocksRuntimeCounter = 0;
        public int InsertedBlocksRuntimeCounter = 0;

        public override string ToString()
        {
            if (_filePath != null && _filePath.Length > 0)
                return _filePath;
            else
                return base.ToString();
        }

        public int NonEmptyBlocks
        {
            get
            {
                if (_fileStream == null)
                    return 0;

                try
                {
                    return (int)((_fileStream.Length / _blockSize) - _deletedBlocks.Count);
                }
                catch (Exception ex)
                {
                    HandledExceptionAdapter.Examine(ex);
                    return 0;
                }
            }
        }

        protected event DVoid2Void AfterDeleteBlockIfFileIsEmpty = null;

        public BlockFile(string fileName, int blockSize)
        {
            if (blockSize > FileBlock.HEADER_SIZE)
            {
                _blockSize = blockSize;
            }
            else
            {
                _blockSize = DEFAULT_BLOCK_SIZE;
            }

            _filePath = CcuCore.Singleton.RootPath + Database.DATABASE_DIRECTORY_NAME + fileName;

            try
            {
                if (!Directory.Exists(CcuCore.Singleton.RootPath + Database.DATABASE_DIRECTORY_NAME))
                    Directory.CreateDirectory(CcuCore.Singleton.RootPath + Database.DATABASE_DIRECTORY_NAME);

                CreateOutpuFileStream();
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        public BlockFile(string fileName, bool runFlushFile, int blockSize)
            : this(fileName, blockSize)
        {
            _runFlushFile = runFlushFile;
        }

        public BlockFile(string fileName)
            : this(fileName, 0)
        {
        }

        public BlockFile(string fileName, bool runFlushFile)
            : this(fileName, runFlushFile, 0)
        {
        }

        private void CreateOutpuFileStream()
        {
            try
            {
                // no need for exclusive locking, FileShare.Read can be allowed
                _fileStream = PatchedFileStream.Open(
                    _filePath, 
                    FileMode.OpenOrCreate, 
                    FileAccess.ReadWrite, 
                    FileShare.Read);
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
                _fileStream = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addObjectDelegate"></param>
        /// <param name="returnStream"></param>
        /// <returns></returns>
        public bool ReadObjectsFromFile(Action<Stream, Guid, long> addObjectDelegate, bool returnStream)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("bool BlockFile({0}).ReadObjectsFromFile(Action<Stream, long> addObjectDelegate): [{1}]", this, Log.GetStringFromParameters(addObjectDelegate, returnStream)));
            bool retValue = false;
            try
            {
                int blockCount = (int)((GetFileLength()) / _blockSize);

                if (blockCount > 0)
                {
                    _firstFreeBlockIndex = blockCount;

                    // marks already read blocks, so no need to examine again
                    Dictionary<int, int> readBlockIndexes = new Dictionary<int, int>(blockCount / 2); // capacity is statistically defined

                    for (int blockPosition = 0; blockPosition < blockCount; blockPosition++)
                    {
                        if (!readBlockIndexes.ContainsKey(blockPosition))
                        {
                            FileBlock block = ReadBlock(blockPosition);

                            if (block != null)
                            {
                                if (block.IsFree())
                                {
                                    _deletedBlocks.Add(blockPosition);
                                }
                                else if (block._objectGuid != Guid.Empty)
                                {
                                    Guid objectGuid = block._objectGuid;
                                    if (returnStream)
                                    {
                                        Stream inputStream = ReadAndMark(ref block, readBlockIndexes);

                                        if (addObjectDelegate != null)
                                        {
                                            addObjectDelegate(inputStream, objectGuid, blockPosition);
                                        }
                                    }
                                    else
                                    {
                                        block.Dispose();
                                        block = null;

                                        if (addObjectDelegate != null)
                                        {
                                            addObjectDelegate(null, objectGuid, blockPosition);
                                        }
                                    }

                                    retValue = true;
                                }
                            }
                        }
                    }

                    UpdateLastBlock();
                }
            }
            catch (Exception ex)
            {
                HandledExceptionAdapter.Examine(ex);
            }

            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("bool BlockFile({0}).ReadObjectsFromFile return {1}", this, retValue));

            return retValue;
        }

        /// <summary>
        /// especially for overriding by other classes
        /// </summary>
        /// <param name="actualPosition"></param>
        /// <returns></returns>
        protected virtual long UpdateActualPosition(long actualPosition)
        {
            return actualPosition;
        }

        public long GetFileLength()
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("long BlockFile({0}).GetFileLength()", this));
            try
            {
                if (_fileStream != null)
                    lock (_filestreamLock)
                    {
                        if (_fileStream != null)
                        {
                            long result = UpdateActualPosition(_fileStream.Length);
                            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("long BlockFile({0}).GetFileLength return {1}", this, result));
                            return result;
                        }
                    }
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }

            CcuCore.DebugLog.Warning(Log.BELOW_NORMAL_LEVEL, () => string.Format("long BlockFile({0}).GetFileLength return -1", this));
            return -1;
        }

        private void CloseFileStrem()
        {
            try
            {
                if (_fileStream != null)
                    lock (_filestreamLock)
                    {
                        if (_fileStream != null)
                        {
                            _fileStream.Close();
                            _fileStream = null;
                        }
                    }
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        private int GetFreeBlock()
        {
            lock (_deletedBlocks)
            {
                if (_deletedBlocks.Count > 0)
                {
                    int freeBlock = _deletedBlocks[0];
                    _deletedBlocks.RemoveAt(0);
                    return freeBlock;
                }
                else
                {
                    int freeBlock = _firstFreeBlockIndex;
                    _firstFreeBlockIndex++;
                    return freeBlock;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputStream"></param>
        /// <param name="objectGuid"></param>
        /// <returns></returns>
        public long Write(Stream inputStream, Guid objectGuid)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("long BlockFile({0}).Write(Stream inputStream): [{1}]", this, inputStream));
            try
            {
                int position;

                lock (_lockForWriteDelete)
                {
                    int freeBlock = GetFreeBlock();
                    position = freeBlock;
                    bool isFirstBlock = true;

                    while (inputStream.Position < inputStream.Length)
                    {
                        byte[] data = new byte[FileBlock.GetMaxDataSize(_blockSize)];
                        int length = inputStream.Read(data, 0, data.Length);
                        int nextBlock = -1;

                        if (inputStream.Position < inputStream.Length)
                        {
                            nextBlock = GetFreeBlock();
                        }

                        FileBlock block;
                        if (isFirstBlock)
                        {
                            block = new FileBlock(data, (short)length, nextBlock, objectGuid);
                            isFirstBlock = false;
                        }
                        else
                        {
                            block = new FileBlock(data, (short)length, nextBlock);
                        }

                        WriteBlockToFile(freeBlock, block);
                        block.Dispose();
                        block = null;

                        freeBlock = nextBlock;
                    }
                }

                CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("long BlockFile({0}).Write return {1}", this, position));
                return position;
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
                CcuCore.DebugLog.Warning(Log.BELOW_NORMAL_LEVEL, () => string.Format("long BlockFile({0}).Write return {1}", this, -1));
                return -1;
            }
        }

        protected void Write(long filePosition, byte[] data, bool runWasChanged)
        {
            try
            {
                if (_fileStream != null)
                {
                    lock (_filestreamLock)
                    {
                        if (_fileStream != null && data != null)
                        {
                            _fileStream.Seek(filePosition, SeekOrigin.Begin);
                            _fileStream.Write(data, 0, data.Length);
                            _fileStream.SetLength(GetActualSize());
                        }
                    }
                }

                if (runWasChanged)
                    FileChanged();
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="block"></param>
        private void WriteBlockToFile(int position, FileBlock block)
        {
            try
            {
                if (_fileStream != null && block != null)
                {
                    byte[] data = block.ToByte(block.IsFree(), _blockSize); // if free, only header will be overwritten

                    lock (_filestreamLock)
                    {
                        if (_fileStream != null)
                        {
                            Seek(position);

                            _fileStream.Write(data, 0, data.Length);

                            if (block.IsFree())
                                DeletedBlocksRuntimeCounter++;
                            else
                                InsertedBlocksRuntimeCounter++;

                            _fileStream.SetLength(GetActualSize());
                        }
                    }
                }

                FileChanged();
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        private long GetActualSize()
        {
            try
            {
                return UpdateActualPosition(_firstFreeBlockIndex * _blockSize);
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }

            return -1;
        }

        /// <summary>
        /// seeks to byte counted as position*BLOCK_SIZE
        /// </summary>
        /// <param name="position"></param>
        private long Seek(int position)
        {
            if (_fileStream != null)
            {
                long newPosition = UpdateActualPosition(position * _blockSize);
                _fileStream.Seek(newPosition, SeekOrigin.Begin);
                if (_fileStream.Position != newPosition)
                    DebugHelper.NOP(newPosition);

                return newPosition;
            }
            else
                return -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="outputStream"></param>
        public void Read(int position, Stream outputStream)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("void BlockFile({0}).Read(int position, Stream outputStream): [{1}]", this, Log.GetStringFromParameters(position, outputStream)));
            try
            {
                while (position >= 0)
                {
                    FileBlock block = ReadBlock(position);
                    if (null == block)
                        return;

                    if (!block.IsFree())
                    {
                        outputStream.Write(block._data, 0, block._data.Length);
                        position = block._nextBlockIndex;
                    }
                    else
                    {
                        position = -1;
                    }

                    block.Dispose();
                    block = null;
                }
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        protected int Read(long filePosition, byte[] data)
        {
            try
            {
                if (_fileStream != null)
                    lock (_filestreamLock)
                    {
                        if (_fileStream != null && data != null)
                        {
                            _fileStream.Seek(filePosition, SeekOrigin.Begin);
                            return _fileStream.Read(data, 0, data.Length);
                        }
                    }
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }

            return -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="block"></param>
        /// <param name="readedBlocks"></param>
        /// <returns></returns>
        private Stream ReadAndMark(ref FileBlock block, IDictionary<int, int> readedBlocks)
        {
            Stream inputStream = new MemoryStream();

            while (block != null)
            {
                inputStream.Write(block._data, 0, block._data.Length);

                int nextBlock = block._nextBlockIndex;
                block.Dispose();
                block = null;

                if (nextBlock >= 0)
                {
                    readedBlocks.Add(nextBlock, 0);
                    block = ReadBlock(nextBlock);
                }
            }

            return inputStream;
        }

        private FileBlock ReadBlock(int position)
        {
            try
            {
                if (_fileStream != null)
                    lock (_filestreamLock)
                    {
                        if (_fileStream != null)
                        {
                            byte[] data = new byte[_blockSize];
                            Seek(position);

                            int readedBytesCount = 0;
                            while (readedBytesCount < _blockSize)
                            {
                                byte[] readedBytes = new byte[_blockSize - readedBytesCount];
                                int length = _fileStream.Read(readedBytes, 0, readedBytes.Length);

                                if (length == 0)
                                    return null;

                                Array.Copy(readedBytes, 0, data, readedBytesCount, length);
                                readedBytesCount += length;
                            }


                            return FileBlock.FromByte(data, _blockSize);
                        }
                    }
            }
            catch (Exception err)
            {
                HandledExceptionAdapter.Examine(err);
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        public void DeleteBlocks(int position)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("void BlockFile({0}).DeleteBlocks(int position): [{1}]", this, Log.GetStringFromParameters(position)));
            try
            {
                lock (_lockForWriteDelete)
                {
                    while (position >= 0)
                    {
                        FileBlock block = ReadBlock(position);
                        int newPosition = -1;
                        if (block != null && !block.IsFree())
                        {
                            newPosition = block._nextBlockIndex;
                            block.SetAsFree();
                            WriteBlockToFile(position, block);
                            block.Dispose();

                            lock (_deletedBlocks)
                            {
                                _deletedBlocks.Add(position);
                            }
                        }
                        block = null;

                        position = newPosition;
                    }
                }

                UpdateLastBlock();

                if (AfterDeleteBlockIfFileIsEmpty != null && _deletedBlocks.Count == 0 && _firstFreeBlockIndex == 0)
                {
                    try
                    {
                        AfterDeleteBlockIfFileIsEmpty();
                    }
                    catch (Exception error)
                    {
                        HandledExceptionAdapter.Examine(error);
                    }
                }
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        private void UpdateLastBlock()
        {
            lock (_deletedBlocks)
            {
                _deletedBlocks.Sort();

                bool shouldContinue = true;

                while (shouldContinue)
                {
                    shouldContinue = false;

                    if (_deletedBlocks.Count > 0)
                    {
                        if (_deletedBlocks[_deletedBlocks.Count - 1] == _firstFreeBlockIndex - 1)
                        {
                            _firstFreeBlockIndex--;
                            _deletedBlocks.RemoveAt(_deletedBlocks.Count - 1);
                            shouldContinue = true;
                        }
                    }

                }

            }
        }

        public void DeleteFile()
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("void BlockFile({0}).DeleteFile()", this));
            try
            {
                lock (_filestreamLock)
                {
                    DeletedBlocksRuntimeCounter += NonEmptyBlocks;
                    CloseFileStrem();
                    if (File.Exists(_filePath))
                    {
                        File.Delete(_filePath);
                        _firstFreeBlockIndex = 0;
                        _deletedBlocks.Clear();
                    }

                    if (AfterDeleteBlockIfFileIsEmpty != null)
                        AfterDeleteBlockIfFileIsEmpty();

                    CreateOutpuFileStream();
                }
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        private void StartTimerFlushFile()
        {
            if (_runFlushFile && _timerFlushFile == null)
                _timerFlushFile = NativeTimerManager.StartTimer(
                    DELAY_TO_FLUSH_FILE, 
                    true, 
                    OnTimerFlushFile, 
                    (byte)PrirotyForOnTimerEvent.FileOperations);
        }

        protected virtual void BeforeFlushToFile()
        {
        }

        private bool OnTimerFlushFile(NativeTimer timer)
        {
            try
            {
                if (_runSwapToFile)
                {
                    if (_timerFlushFile != null)
                    {
                        _timerFlushFile.StopTimer();
                        _timerFlushFile = null;
                    }

                    if (_wasChanged)
                    {
                        _wasChanged = false;

                        lock (_filestreamLock)
                        {
                            if (_fileStream != null)
                            {
                                BeforeFlushToFile();
                                _fileStream.Flush();
                            }
                        }
                    }
                }
                else
                {
                    _runSwapToFile = true;
                }
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }

            return true;
        }

        public void Flush()
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("void BlockFile({0}).Flush()", this));
            lock (_filestreamLock)
            {
                if (_fileStream != null)
                {
                    BeforeFlushToFile();
                    _fileStream.Flush();
                }
            }

            StartTimerFlushFile();
        }

        private void FileChanged()
        {
            _wasChanged = true;
            _runSwapToFile = false;
            StartTimerFlushFile();
        }

        public BlockFileInfo BlockFileInfo
        {
            get
            {
                return
                    new BlockFileInfo(FileName)
                    {
                        BlocksAddedRuntime = InsertedBlocksRuntimeCounter,
                        BlocksRemovedRuntime = DeletedBlocksRuntimeCounter,
                        FileSize = GetFileLength()
                    };
            }
        }

        public void Reset()
        {
            InsertedBlocksRuntimeCounter = 0;
            DeletedBlocksRuntimeCounter = 0;
        }
    }
}
