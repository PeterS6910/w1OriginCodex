using System;
using System.Threading;
using JetBrains.Annotations;
using Contal.IwQuick.Threads;

namespace Contal.Cgp.NCAS.NodeDataProtocol
{
    class UpgradeContext
    {
        /*
         * DCU flash memory allocation
         * 
         *              Bootloader          Application                     Storage
         * ROM Area:    0 - 4FFF (20479)    5000 (20480) - EFFF (61439)     F000 (61440) - FFFF (65535)
         * Pages:       0 - 39              40 - 119                        120 - 127
         * 
         */

        
        private const int BASE_APP_ADDRESS = 0x5000;
        private const int BASE_APP_PAGE = (BASE_APP_ADDRESS / 0x200);          /* for erasing (page = 512 bytes) */
        
        /*
        private const int APP_LENGTH_CODE = 0xfffe;
        private const int APP_CHECKSUM_CODE = 0xffff;
        private const int ERROR_COUNT_TRIGGER = 5;*/

        public UpgradeContext(byte address)
        {
            _nodeAddress = address;

            _waitForReply = new ManualResetEvent(false);
            _waitForErase = new ManualResetEvent(false);
            _waitForUploader = new ManualResetEvent(false);
            _waitForBaseAddress = new ManualResetEvent(false);
            
            _upgradeInProgress = false;
        }

        public void SetToDefault()
        {
            _pageForErase = BASE_APP_PAGE;

            _upgradeTimeout = false;
            _upgradeError = false;
            _upgradeDone = false;

            _blockNumber = 0;
            _resendLastData = false;
            _eraseAgain = false;
            _arithmeticChecksum = 0;
        }

        public void CalculatePartialChecksum(byte[] data, int length)
        {
            _partialChecksum = 0;
            for (int i = 0; i < length; i++)
            {
                _arithmeticChecksum += data[i];
                _partialChecksum += data[i];
            }
        }

        private bool _upgradeInProgress = false;
        public bool UpgradeInProgress
        {
            get { return _upgradeInProgress; }
            set { _upgradeInProgress = value; }
        }

        private bool _writtingStarted = false;
        public bool WrittingRunning
        {
            get { return _writtingStarted; }
            set { _writtingStarted = value; }
        }

        private byte _nodeAddress = 0;
        public byte NodeAddress
        {
            get { return _nodeAddress; }
            set { _nodeAddress = value; }
        }

        private SafeThread<byte> _upgradeThread = null;
        public SafeThread<byte> UpgradeThread
        {
            get { return _upgradeThread; }
            set { _upgradeThread = value; }
        }

        private string _filePath;
        public string FilePath
        {
            get { return _filePath; }
            set { _filePath = value; }
        }

        [NotNull]
        private readonly ManualResetEvent _waitForReply;
        public ManualResetEvent WaitForReply
        {
            get { return _waitForReply; }
        }

        [NotNull]
        private readonly ManualResetEvent _waitForErase;
        public ManualResetEvent WaitForErase
        {
            get { return _waitForErase; }
        }

        [NotNull]
        private readonly ManualResetEvent _waitForUploader;
        public ManualResetEvent WaitForUploader
        {
            get { return _waitForUploader; }
        }

        [NotNull]
        private readonly ManualResetEvent _waitForBaseAddress;
        public ManualResetEvent WaitForBaseAddress
        {
            get { return _waitForBaseAddress; }
        }

        private int _pageForErase = BASE_APP_PAGE;
        public int PageForErase
        {
            get { return _pageForErase; }
            set { _pageForErase = value; }
        }

        private int _baseAddress = BASE_APP_ADDRESS;
        public int BaseAddress
        {
            get { return _baseAddress; }
            set 
            { 
                _baseAddress = value;
                _pageForErase = (_baseAddress / 512);
            }
        }


#pragma warning disable 169
        private string _upgradeFilePath;
#pragma warning restore 169

        private bool _eraseAgain;
        public bool EraseAgain
        {
            get { return _eraseAgain; }
            set { _eraseAgain = value; }
        }

        private UInt16 _blockNumber;
        public UInt16 BlockNumber
        {
            get { return _blockNumber; }
            set { _blockNumber = value; }
        }
        
        private bool _resendLastData;
        public bool DataAgain
        {
            get { return _resendLastData; }
            set { _resendLastData = value; }
        }
        
        private UInt32 _arithmeticChecksum;
        public UInt32 ArithmeticChecksum
        {
            get { return _arithmeticChecksum; }
            set { _arithmeticChecksum = value; }
        }

        private UInt32 _partialChecksum;
        public UInt32 PartialChecksum
        {
            get { return _partialChecksum; }
            set { _partialChecksum = value; }
        }

        private UpgradeStage _upgradeStage;
        public UpgradeStage Stage
        {
            get { return _upgradeStage; }
            set { _upgradeStage = value; }
        }

        private bool _upgradeDone = false;
        public bool Done
        {
            get { return _upgradeDone; }
            set { _upgradeDone = value; }
        }

        private bool _upgradeError = false;
        public bool Error
        {
            get { return _upgradeError; }
            set { _upgradeError = value; }
        }

        private bool _upgradeTimeout = false;
        public bool Timeout
        {
            get { return _upgradeTimeout; }
            set { _upgradeTimeout = value; }
        }

        private int _errorCount = 0;
        public int ErrorCount
        {
            get { return _errorCount; }
            set { _errorCount = value; }
        }

    }
}
