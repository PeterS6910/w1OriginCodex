using System;


namespace Contal.IwQuick.Crypto
{
    enum dsKeyTable
    {
        // Service key crypto table
        dSKeyTable0 = 0x1A,
        dSKeyTable1 = 0xC8,
        dSKeyTable2 = 0xD0,
        dSKeyTable3 = 0x65,
        dSKeyTable4 = 0x98,
        dSKeyTable5 = 0xE4,
        dSKeyTable6 = 0x3D,
        dSKeyTable7 = 0x83,
        dSKeyTable8 = 0xAE,
        dSKeyTable9 = 0xFB,
        dSKeyTableA = 0xBC,
        dSKeyTableB = 0x29,
        dSKeyTableC = 0x44,
        dSKeyTableD = 0x57,
        dSKeyTableE = 0x83,
        dSKeyTableF = 0x71,
        // Service key
        dSKey0 = 0x3D,
        dSKey1 = 0x5B,
        dSKey2 = 0x7A,
        dSKey3 = 0x63,
        dSKey4 = 0x45,
        dSKey5 = 0xE1,
        dSKey6 = 0x2C,
        dSKey7 = 0x16,
        dSKey8 = 0x87,
        dSKey9 = 0x9A,
        // Service Xkey
        dSXKey0 = 0xE8,
        dSXKey1 = 0x4F,
        dSXKey2 = 0x84,
        dSXKey3 = 0x62,
        dSXKey4 = 0xFA,
        dSXKey5 = 0x31,
        dSXKey6 = 0x7D,
        dSXKey7 = 0xA9,
        dSXKey8 = 0x25,
        dSXKey9 = 0x5C,
        dSXKeyA = 0xD7,
        dSXKeyB = 0x98,
        dSXKeyC = 0xB6,
        dSXKeyD = 0x13,
        dSXKeyE = 0xCD,
        dSXKeyF = 0x7B,
    };

    /// <summary>
    /// 
    /// </summary>
    public class XTEA
    {
        const int dXKey0 = (int)dsKeyTable.dSXKey0;
        const int dXKey1 = (int)dsKeyTable.dSXKey1;
        const int dXKey2 = (int)dsKeyTable.dSXKey2;
        const int dXKey3 = (int)dsKeyTable.dSXKey3;
        const int dXKey4 = (int)dsKeyTable.dSXKey4;
        const int dXKey5 = (int)dsKeyTable.dSXKey5;
        const int dXKey6 = (int)dsKeyTable.dSXKey6;
        const int dXKey7 = (int)dsKeyTable.dSXKey7;
        const int dXKey8 = (int)dsKeyTable.dSXKey8;
        const int dXKey9 = (int)dsKeyTable.dSXKey9;
        const int dXKeyA = (int)dsKeyTable.dSXKeyA;
        const int dXKeyB = (int)dsKeyTable.dSXKeyB;
        const int dXKeyC = (int)dsKeyTable.dSXKeyC;
        const int dXKeyD = (int)dsKeyTable.dSXKeyD;
        const int dXKeyE = (int)dsKeyTable.dSXKeyE;
        const int dXKeyF = (int)dsKeyTable.dSXKeyF;

        private readonly byte[] _userXteaKey =                // XTEA USER Key - decryption index
        {
            0x08, 0x0E, 0x06, 0x09, 0x0C, 0x07, 0x0A, 0x05, 0x0D, 0x04, 0x02, 0x00, 0x03, 0x0F, 0x02, 0x01
        };

        private readonly byte[] _sKeyTable =
        {
            (byte)dsKeyTable.dSKeyTable0,
            (byte)dsKeyTable.dSKeyTable1,
            (byte)dsKeyTable.dSKeyTable2,
            (byte)dsKeyTable.dSKeyTable3,
            (byte)dsKeyTable.dSKeyTable4,
            (byte)dsKeyTable.dSKeyTable5,
            (byte)dsKeyTable.dSKeyTable6,
            (byte)dsKeyTable.dSKeyTable7,
            (byte)dsKeyTable.dSKeyTable8,
            (byte)dsKeyTable.dSKeyTable9,
            (byte)dsKeyTable.dSKeyTableA,
            (byte)dsKeyTable.dSKeyTableB,
            (byte)dsKeyTable.dSKeyTableC,
            (byte)dsKeyTable.dSKeyTableD,
            (byte)dsKeyTable.dSKeyTableE,
            (byte)dsKeyTable.dSKeyTableF
        };

        //private byte[] dXKey = new byte[]
        //{
        //    (byte)dsKeyTable.dSXKey0^(byte)dsKeyTable.dSKeyTable5,
        //    (byte)dsKeyTable.dSXKey1^(byte)dsKeyTable.dSKeyTable3,
        //    (byte)dsKeyTable.dSXKey2^(byte)dsKeyTable.dSKeyTableD,
        //    (byte)dsKeyTable.dSXKey3^(byte)dsKeyTable.dSKeyTable7,
        //    (byte)dsKeyTable.dSXKey4^(byte)dsKeyTable.dSKeyTable8,
        //    (byte)dsKeyTable.dSXKey5^(byte)dsKeyTable.dSKeyTableC,
        //    (byte)dsKeyTable.dSXKey6^(byte)dsKeyTable.dSKeyTableA,
        //    (byte)dsKeyTable.dSXKey7^(byte)dsKeyTable.dSKeyTable0,
        //    (byte)dsKeyTable.dSXKey8^(byte)dsKeyTable.dSKeyTable1,
        //    (byte)dsKeyTable.dSXKey9^(byte)dsKeyTable.dSKeyTable4,
        //    (byte)dsKeyTable.dSXKeyA^(byte)dsKeyTable.dSKeyTableF,
        //    (byte)dsKeyTable.dSXKeyB^(byte)dsKeyTable.dSKeyTable2,
        //    (byte)dsKeyTable.dSXKeyC^(byte)dsKeyTable.dSKeyTableE,
        //    (byte)dsKeyTable.dSXKeyD^(byte)dsKeyTable.dSKeyTable6,
        //    (byte)dsKeyTable.dSXKeyE^(byte)dsKeyTable.dSKeyTable9,
        //    (byte)dsKeyTable.dSXKeyF^(byte)dsKeyTable.dSKeyTableB
        //};

        private readonly byte[] _dXKey =
        {
            (byte)dsKeyTable.dSXKey0^(byte)dsKeyTable.dSKeyTable8,
            (byte)dsKeyTable.dSXKey1^(byte)dsKeyTable.dSKeyTableE,
            (byte)dsKeyTable.dSXKey2^(byte)dsKeyTable.dSKeyTable6,
            (byte)dsKeyTable.dSXKey3^(byte)dsKeyTable.dSKeyTable9,
            (byte)dsKeyTable.dSXKey4^(byte)dsKeyTable.dSKeyTableC,
            (byte)dsKeyTable.dSXKey5^(byte)dsKeyTable.dSKeyTable7,
            (byte)dsKeyTable.dSXKey6^(byte)dsKeyTable.dSKeyTableA,
            (byte)dsKeyTable.dSXKey7^(byte)dsKeyTable.dSKeyTable5,
            (byte)dsKeyTable.dSXKey8^(byte)dsKeyTable.dSKeyTableD,
            (byte)dsKeyTable.dSXKey9^(byte)dsKeyTable.dSKeyTable4,
            (byte)dsKeyTable.dSXKeyA^(byte)dsKeyTable.dSKeyTable2,
            (byte)dsKeyTable.dSXKeyB^(byte)dsKeyTable.dSKeyTable0,
            (byte)dsKeyTable.dSXKeyC^(byte)dsKeyTable.dSKeyTable3,
            (byte)dsKeyTable.dSXKeyD^(byte)dsKeyTable.dSKeyTableF,
            (byte)dsKeyTable.dSXKeyE^(byte)dsKeyTable.dSKeyTable2,
            (byte)dsKeyTable.dSXKeyF^(byte)dsKeyTable.dSKeyTable1
        };

        private readonly byte[] _dSJSKey = //new byte[10]
        {
            (byte)dsKeyTable.dSKey0^(byte)dsKeyTable.dSKeyTableD,
            (byte)dsKeyTable.dSKey1^(byte)dsKeyTable.dSKeyTable3,
            (byte)dsKeyTable.dSKey2^(byte)dsKeyTable.dSKeyTable7,
            (byte)dsKeyTable.dSKey3^(byte)dsKeyTable.dSKeyTable1,
            (byte)dsKeyTable.dSKey4^(byte)dsKeyTable.dSKeyTable0,
            (byte)dsKeyTable.dSKey5^(byte)dsKeyTable.dSKeyTableF,
            (byte)dsKeyTable.dSKey6^(byte)dsKeyTable.dSKeyTableA,
            (byte)dsKeyTable.dSKey7^(byte)dsKeyTable.dSKeyTableD,
            (byte)dsKeyTable.dSKey8^(byte)dsKeyTable.dSKeyTable8,
            (byte)dsKeyTable.dSKey9^(byte)dsKeyTable.dSKeyTable4
        };

        private readonly byte[] _dSXKey = new byte[16]
        {
            (byte)dsKeyTable.dSXKey0^(byte)dsKeyTable.dSKeyTable5,
            (byte)dsKeyTable.dSXKey1^(byte)dsKeyTable.dSKeyTable3,
            (byte)dsKeyTable.dSXKey2^(byte)dsKeyTable.dSKeyTableD,
            (byte)dsKeyTable.dSXKey3^(byte)dsKeyTable.dSKeyTable7,
            (byte)dsKeyTable.dSXKey4^(byte)dsKeyTable.dSKeyTable8,
            (byte)dsKeyTable.dSXKey5^(byte)dsKeyTable.dSKeyTableC,
            (byte)dsKeyTable.dSXKey6^(byte)dsKeyTable.dSKeyTableA,
            (byte)dsKeyTable.dSXKey7^(byte)dsKeyTable.dSKeyTable0,
            (byte)dsKeyTable.dSXKey8^(byte)dsKeyTable.dSKeyTable1,
            (byte)dsKeyTable.dSXKey9^(byte)dsKeyTable.dSKeyTable4,
            (byte)dsKeyTable.dSXKeyA^(byte)dsKeyTable.dSKeyTableF,
            (byte)dsKeyTable.dSXKeyB^(byte)dsKeyTable.dSKeyTable2,
            (byte)dsKeyTable.dSXKeyC^(byte)dsKeyTable.dSKeyTableE,
            (byte)dsKeyTable.dSXKeyD^(byte)dsKeyTable.dSKeyTable6,
            (byte)dsKeyTable.dSXKeyE^(byte)dsKeyTable.dSKeyTable9,
            (byte)dsKeyTable.dSXKeyF^(byte)dsKeyTable.dSKeyTableB
        };

        const int dCryptoDis = 0x55;            // without encryption
        const int dCryptoEn = 0x3C;            // normal encryption
        const int dCryptoSer = 0x24;            // service key used
        const int dCryptoTagPos = 0;
        const int dCryptoNTagPos = 1;
        const int dCryptoLenPos = 2;
        const int dPaddingPos = 3;
        const int dCryptoFrLn = 5;              // additional data (exc. padding, but include checksumm)

        const int dXteaBlMax = 4;               // maximum of XTEA block (one block = 8bytes)
        // Xtea Message struc
        const int dXtSumInd = 0;
        const int dXtLenInd = 1;
        const int dXtPadInd = 2;

        //------------------------------------------------------------------------------
        const int dSJKeyLn = 10;
        const int dSKeyTableLn = 16;
        const int dXKeyLn = 16;
        const int dAKeyLn = 16;

        private const int CYCLES = 32;
// ReSharper disable once ConvertToConstant.Local
        private readonly uint DELTA = 0x9e3779b9;
        const int BLOCKSIZE = 8;

        private byte[] _xKey = new byte[dXKeyLn];

        private CryptoFixKey _fixKey;

        /// <summary>
        /// 
        /// </summary>
        public void XTeaInit()
        {
            _fixKey = new CryptoFixKey(_dXKey, _dSJSKey, _dSXKey);
            for (byte i = 0; i < dXKeyLn; i++)
            {
                _xKey[i] = (byte)(_fixKey.XKey[i] ^ _sKeyTable[_userXteaKey[i]]);
            }
        }

        /// <summary>
        /// Clears an XTEA key
        /// </summary>
        public void XTeaKeyClear()
        {
            for (int i = 0; i < _xKey.Length; i++)
            {
                _xKey[i] = 0;
            }
        }

        /// <summary>
        /// Sets key for XTEA algorithm
        /// </summary>
        /// <param name="keyData">32 bytes representing key value</param>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="InvalidLengthException"></exception>
        public void SetKey(byte[] keyData)
        {
            if (keyData == null) throw new NullReferenceException();
            if (keyData.Length != 16) throw new InvalidLengthException();
            _xKey = keyData;
        }

        /// <summary>
        /// Encrypts one block (8 bytes) data
        /// </summary>
        /// <param name="blockData">Data to be encrypted (8 bytes)</param>
        /// <returns>Encrypted block (8 bytes)</returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="InvalidLengthException"></exception>
        public byte[] BlockEncrypt(byte[] blockData)
        {
            //input handling
            if (blockData == null) throw new NullReferenceException();
            if (blockData.Length != 8) throw new InvalidLengthException();

            //data are processed as a 4 bytes numbers
            uint y = BitConverter.ToUInt32(blockData, 0);
            uint z = BitConverter.ToUInt32(blockData, 4);

            uint sum = 0;
            for (byte i = 0; i < CYCLES; i++)
            {
                y += ((z << 4 ^ z >> 5) + z) ^ (sum + BitConverter.ToUInt32(_xKey, (int)((sum & 3) * 4)));
                sum += DELTA;
                z += ((y << 4 ^ y >> 5) + y) ^ (sum + BitConverter.ToUInt32(_xKey, (int)((sum >> 11 & 3) * 4)));
            }

            //creating return value
            byte[] returnValue = new byte[BLOCKSIZE];
            Buffer.BlockCopy(BitConverter.GetBytes(y), 0, returnValue, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(z), 0, returnValue, 4, 4);
            return returnValue;
        }

        /// <summary>
        /// Decrypts one block (8 bytes) data
        /// </summary>
        /// <param name="blockData">Data to be decrypted (8 bytes)</param>
        /// <returns>Decrypted block (8 bytes)</returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="InvalidLengthException"></exception>
        public byte[] BlockDecrypt(byte[] blockData)
        {                        
            //input handling
            if (blockData == null) throw new NullReferenceException();
            if (blockData.Length != 8) throw new InvalidLengthException();

            //data are processed as a 4 bytes numbers
            uint y = BitConverter.ToUInt32(blockData, 0);
            uint z = BitConverter.ToUInt32(blockData, 4);

            uint sum = DELTA * CYCLES;
            for (byte i = 0; i < CYCLES; i++)
            {
                z -= ((y << 4 ^ y >> 5) + y) ^ sum + BitConverter.ToUInt32(_xKey, (int)(sum >> 11 & 3) * 4);
                sum -= DELTA;
                y -= ((z << 4 ^ z >> 5) + z) ^ sum + BitConverter.ToUInt32(_xKey, (int)(sum & 3) * 4);
            }

            //creating return value
            byte[] returnValue = new byte[BLOCKSIZE];
            Buffer.BlockCopy(BitConverter.GetBytes(y), 0, returnValue, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(z), 0, returnValue, 4, 4);
            return returnValue;
        }

        /// <summary>
        /// Decrypt Cipher Block Chained data
        /// </summary>
        /// <param name="blocksData">Bytes representing blocks to be decrypted (length must be divisible by 8)</param>
        /// <returns>Decrypted data blocks</returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="InvalidLengthException"></exception>
        public byte[] XTeaCbcDec(byte[] blocksData)
        {                               
            //check input data
            if (blocksData == null) throw new NullReferenceException();
            if (blocksData.Length == 0 || (blocksData.Length % BLOCKSIZE != 0)) throw new InvalidLengthException();            

            int blocksCount = blocksData.Length / BLOCKSIZE;

            //last block decrypt as first
            int actualPosition = (blocksCount - 1) * BLOCKSIZE;            
            int previousPosition = actualPosition;
            int blocksProcessed = 0;

            //decrypt block after block
            while (true)
            {                
                byte[] block = new byte[BLOCKSIZE];
                Buffer.BlockCopy(blocksData, actualPosition, block, 0, BLOCKSIZE);
                Buffer.BlockCopy(BlockDecrypt(block), 0, blocksData, actualPosition, BLOCKSIZE);
                blocksProcessed++;

                //if all blocked are processed, stop decryption process
                if (blocksProcessed >= blocksCount) break;

                //next block deinitialization
                actualPosition -= BLOCKSIZE;
                for (int i = 0; i < BLOCKSIZE; i++)
                {
                    //XORing previous (decrypted) block with actual block (not decrypted yet)
                    blocksData[previousPosition + i] ^= blocksData[actualPosition + i];
                }
                previousPosition = actualPosition;
            }

            //return decrypted data blocks
            return blocksData;
        }

        /// <summary>
        /// Encrypt data blocks
        /// </summary>
        /// <returns>Encrypted Cipher Block Chained data</returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="InvalidLengthException"></exception>
        public byte[] XTeaCbcEnc(byte[] blocksData)
        {
            //check input data
            if (blocksData == null) throw new NullReferenceException();
            if (blocksData.Length == 0 || (blocksData.Length % BLOCKSIZE != 0)) throw new InvalidLengthException();

            //start from first block
            int actualPosition = 0;
            int previousPosition = 0;
            int blocks = blocksData.Length / BLOCKSIZE;
            for (int i = 0; i < blocks; i++)
            {
                byte[] block = new byte[BLOCKSIZE];

                //XOR new block bytes with previous block bytes (except of first block)
                if (i != 0)
                {
                    for (int j = 0; j < BLOCKSIZE; j++)
                    {
                        blocksData[actualPosition + j] ^= blocksData[previousPosition + j];
                    }
                    previousPosition = actualPosition;
                }

                //store input block into block variable
                Buffer.BlockCopy(blocksData, actualPosition, block, 0, BLOCKSIZE);

                //copy encrypted block into main buffer
                Buffer.BlockCopy(BlockEncrypt(block), 0, blocksData, actualPosition, BLOCKSIZE);
                actualPosition += BLOCKSIZE;
            }
            return blocksData;
        }

        /// <summary>
        /// Decrypts XTEA frame
        /// </summary>
        /// <param name="frameBytes">Bytes representing XTEA frame (must be divisible by 8)</param>
        /// <returns>Data bytes (no padding and info data)</returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="InvalidLengthException"></exception>
        /// <exception cref="MaxDataExceededException"></exception>
        /// <exception cref="WrongChecksumException"></exception>
        public byte[] XTeaFrameDec(byte[] frameBytes)
        {            
            //input check
            if (frameBytes == null) throw new NullReferenceException();

            //check buffer length (must be divisible by BLOCKSIZE)
            int bufferLength = frameBytes.Length;
            if (((bufferLength % BLOCKSIZE) != 0) || (bufferLength == 0)) throw new InvalidLengthException(); 
           
            //check blocks count (can not exceed dXteaBlMax)
            int byBlocks = bufferLength / BLOCKSIZE;
            if (byBlocks > dXteaBlMax) throw new MaxDataExceededException();
            
            //decrypt buffer            
            frameBytes = XTeaCbcDec(frameBytes);           

            // frame checking
            // ckeck, if checksum is correct
            int checkSum = 0;
            for (int i = 0; i < bufferLength; i++)
            {
                checkSum = (byte)(checkSum + frameBytes[i]);
            }
            if (checkSum != 0) throw new WrongChecksumException();
            
            //count padded data (randomly generated data)
            int byPad = bufferLength - frameBytes[dXtLenInd];

            // get users data from buffer
            bufferLength = frameBytes[dXtLenInd];

            //checks number red from buffer (this number determines users data size)
            if (bufferLength > (dXteaBlMax * BLOCKSIZE)) throw new MaxDataExceededException();
            
            //move the users data to begin from first position
            for (int i = 0; i < bufferLength; i++)
            {
                frameBytes[i] = frameBytes[i + byPad];
            }

            //create and fill new buffer with no useless data bytes
            byte[] newBuffer = new byte[bufferLength];
            Buffer.BlockCopy(frameBytes, 0, newBuffer, 0, bufferLength);

            return newBuffer;
        }

        /// <summary>
        /// Encapsulates data bytes into frame and encrypts these bytes
        /// </summary>
        /// <param name="dataBytes"></param>
        /// <returns></returns>
        public byte[] XTeaFrameEnc(byte[] dataBytes)
        {
            //create message frame
            dataBytes = CreateMessageFrame(dataBytes);
            //encode blocks
            return XTeaCbcEnc(dataBytes);
        }

        /// <summary>
        /// Creates message frame from input data
        /// </summary>
        /// <param name="inputData">Input data bytes</param>
        /// <returns>Frame bytes (divisible by 8)</returns>
        public byte[] CreateMessageFrame(byte[] inputData)
        {
            //there must be at least 2 padding bytes (SUM and LENGTH)
            int paddingCount = 2;
            //this means that padding is needed
            if ((inputData.Length + 2) % BLOCKSIZE != 0)
            {
                //how many bytes will be added
                paddingCount += BLOCKSIZE - ((inputData.Length + 2) % BLOCKSIZE);
            }
            byte[] newBuffer = new byte[inputData.Length + paddingCount];
            //old data transfer
            Buffer.BlockCopy(inputData, 0, newBuffer, paddingCount, inputData.Length);
            if (paddingCount > 2)
            {
                //generate padding data
                byte[] randomPaddingData = new byte[paddingCount - 2];
                Random random = new Random();
                random.NextBytes(randomPaddingData);
                //copy padding data
                Buffer.BlockCopy(randomPaddingData, 0, newBuffer, 2, randomPaddingData.Length);
            }
            //copy length (means data length)
            Buffer.BlockCopy(new byte[] { Convert.ToByte(inputData.Length) }, 0, newBuffer, 1, 1);
            byte checkSum = 0;
            for (int i = 1; i < newBuffer.Length; i++)
            {
                checkSum = (byte)(checkSum + newBuffer[i]);
            }
            checkSum = (byte)-checkSum;
            //add checksum as a first byte
            Buffer.BlockCopy(new byte[] { checkSum }, 0, newBuffer, 0, 1);
            return newBuffer;
        }

        /// <summary>
        /// 
        /// </summary>
        public class MaxDataExceededException : Exception
        {
            /// <summary>
            /// 
            /// </summary>
            public MaxDataExceededException()
            { }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="message"></param>
            public MaxDataExceededException(string message)
                : base(message)
            { }
        }

        /// <summary>
        /// 
        /// </summary>
        public class WrongChecksumException : Exception
        {
            /// <summary>
            /// 
            /// </summary>
            public WrongChecksumException()
            { }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="message"></param>
            public WrongChecksumException(string message)
                : base(message)
            { }
        }

        /// <summary>
        /// 
        /// </summary>
        public class InvalidLengthException : Exception
        {
            /// <summary>
            /// 
            /// </summary>
            public InvalidLengthException()
            { }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="message"></param>
            public InvalidLengthException(string message)
                : base(message)
            { }
        }

        /// <summary>
        /// 
        /// </summary>
        public class CryptoFixKey
        {
            private byte[] _xKey;

            /// <summary>
            /// 
            /// </summary>
            public byte[] XKey
            {
                get { return _xKey; }
                set { _xKey = value; }
            }
            private byte[] _sJSKey;

            /// <summary>
            /// 
            /// </summary>
            public byte[] SJSKey
            {
                get { return _sJSKey; }
                set { _sJSKey = value; }
            }
            private byte[] _sXKey;

            /// <summary>
            /// 
            /// </summary>
            public byte[] SXKey
            {
                get { return _sXKey; }
                set { _sXKey = value; }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="xKey"></param>
            /// <param name="sJSKey"></param>
            /// <param name="sXKey"></param>
            public CryptoFixKey(byte[] xKey, byte[] sJSKey, byte[] sXKey)
            {
                _xKey = new byte[xKey.Length];
                xKey.CopyTo(_xKey, 0);

                _sJSKey = new byte[sJSKey.Length];
                sJSKey.CopyTo(_sJSKey, 0);

                _sXKey = new byte[sXKey.Length];
                sXKey.CopyTo(_sXKey, 0);
            }
        }
    }
}
