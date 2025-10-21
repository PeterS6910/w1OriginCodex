using System;
using System.Collections.Generic;
using System.Text;

using System.IO;
using System.Runtime.InteropServices;
using System.Globalization;
using JetBrains.Annotations;

namespace Contal.IwQuick.Data
{
    /// <summary>
    /// carrier for byte-array data usually transfered via sockets
    /// </summary>
    public class ByteDataCarrier: IDisposable,ICollection<byte>
    {
        // byte-array buffer
        private volatile byte[] _buffer = null;

        // ofset of the buffer
        private volatile int _offset = 0;

        // actual size of the information in the buffer
        private volatile int _actualSize = 0;

        /// <summary>
        /// encapsulated byte-array buffer
        /// read-only
        /// </summary>
        public byte[] Buffer
        {
            get
            {
                CheckBufferDisposed();
                return _buffer;
            }
        }

        /// <summary>
        /// checks whether the buffer was not disposed by destructor or explitic Dispose call
        /// </summary>
        /// <exception cref="ObjectDisposedException">if the internal buffer is already disposed</exception>
        private void CheckBufferDisposed()
        {
            if (ReferenceEquals(null,_buffer))
                throw new ObjectDisposedException("internal buffer is already disposed");
        }
        

        /// <summary>
        /// actual size of the information in the byte-array buffer
        /// </summary>
        public int ActualSize
        {
            get
            {
                CheckBufferDisposed();
                return _actualSize;
            }
            set
            {
                CheckBufferDisposed();
                if (value < 0)
                {
                    _actualSize = 0;
                    return;
                }

                if (_offset + value > _buffer.Length)
                {
                    _actualSize = _buffer.Length - _offset;
                    return;
                }

                _actualSize = value;
            }
        }

        /// <summary>
        /// actual size of the information in the byte-array buffer
        /// </summary>
        public int Length
        {
            get
            {
                return _actualSize;
            }
        }
       

        /// <summary>
        /// offset in the byte-array
        /// </summary>
        public int Offset
        {
            get
            {
                CheckBufferDisposed();
                return _offset;
            }
            set
            {
                CheckBufferDisposed();
                if (value < 0)
                {
                    _offset = 0;
                    return;
                }

                _offset = 
                    value >= _buffer.Length
                        // TODO : allow offset out of indexing for special scenarios ?
                        ? _buffer.Length 
                        : value;
            }
        }

        /// <summary>
        /// maximum size of the byte-array buffer
        /// </summary>
        public int Size
        {
            get
            {
                CheckBufferDisposed();
                return _buffer.Length;
            }
        }

        /// <summary>
        /// resizes the underlying buffer to newSize
        /// </summary>
        /// <param name="newSize">if new size is less than current, nothing happens</param>
        /// <param name="preserveOriginalData">if false, old data are not being copied and ActualSize reverts to 0</param>
        public bool Grow(int newSize,bool preserveOriginalData) 
        {
            if (newSize <= _buffer.Length)
                return false;

            var newBuffer = new byte[newSize];
            
            if (preserveOriginalData) 
                Array.Copy(_buffer,newBuffer,ActualSize);
            else
                ActualSize = 0;
            
            _buffer = newBuffer;

            return true;
        }

        /// <summary>
        /// COPYING constructor
        /// </summary>
        /// <param name="inputBuffer">data buffer to copy from</param>
        /// <param name="offsetInInputBuffer">offset of data</param>
        /// <param name="lengthOfInputBuffer">actual count of bytes to be copied</param>
        /// <exception cref="ArgumentNullException">if inputBuffer is null</exception>
        public ByteDataCarrier(
            [NotNull] byte[] inputBuffer, 
            int offsetInInputBuffer, 
            int lengthOfInputBuffer)
        {
            Validator.CheckForNull(inputBuffer,"inputBuffer");

            if (offsetInInputBuffer < 0)
                offsetInInputBuffer = 0;

            if (offsetInInputBuffer + lengthOfInputBuffer > inputBuffer.Length)
                lengthOfInputBuffer = inputBuffer.Length - offsetInInputBuffer;            
                

            _buffer = new byte[lengthOfInputBuffer];

            if (lengthOfInputBuffer > 0)
                Array.Copy(inputBuffer, offsetInInputBuffer, _buffer, 0, lengthOfInputBuffer);

            Offset = 0;
            ActualSize = lengthOfInputBuffer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputBuffer"></param>
        /// <param name="offset"></param>
        /// <param name="actualSize"></param>
        /// <param name="encapsulate">if true, the inputBuffer will be encapsulated, otherwise it'll be copied</param>
        public ByteDataCarrier(
            [NotNull] byte[] inputBuffer, 
            int offset, 
            int actualSize, 
            bool encapsulate)
        {
            if (ReferenceEquals(inputBuffer,null))
                throw new ArgumentNullException("inputBuffer");

            if (encapsulate)
            {
                _buffer = inputBuffer;
            }
            else
            {
                _buffer = (byte[])inputBuffer.Clone();
            }

            // NO need to do this here, will be done by ActualSize = actualSize assignment
            //if (actualSize > encapsulatedBuffer.Length)
            //    actualSize = encapsulatedBuffer.Length

            Offset = offset;
            ActualSize = actualSize;
                
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nativeInputBuffer"></param>
        /// <param name="length"></param>
        public ByteDataCarrier(IntPtr nativeInputBuffer, int length)
        {
            if (Validator.IsNull(nativeInputBuffer))
                throw new ArgumentNullException("nativeInputBuffer");

            Validator.CheckNegativeOrZeroInt(length);
           
            _buffer = new byte[length];

            Marshal.Copy(nativeInputBuffer, _buffer, 0, length);

            Offset = 0;
            ActualSize = length;
        }

        /// <summary>
        /// COPYING constructor
        /// </summary>
        /// <param name="inputBuffer">data buffer to copy</param>
        /// <param name="actualSize">actual count of bytes to be copied</param>
        public ByteDataCarrier(
            [NotNull] byte[] inputBuffer, 
            int actualSize)
            : this(inputBuffer,0,actualSize)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="separateBytes"></param>
        public ByteDataCarrier(params byte[] separateBytes)
            :this(separateBytes,0,separateBytes.Length)
        {

        }

        /// <summary>
        /// constructor WITHOUT data encapsulation
        /// </summary>
        /// <param name="preallocatedBufferSize">currently maximum size of the buffer to allocate, still changeable by Append methods in future</param>
        /// <param name="actualSize">actual size of the information in current allocated buffer</param>
        /// <exception cref="ArgumentException">if the preallocatedBufferSize is less or equal to 0</exception>
        public ByteDataCarrier(int preallocatedBufferSize, int actualSize)
        {
            if (preallocatedBufferSize <= 0)
                throw new ArgumentException("preallocatedBufferSize");

            if (actualSize < 0)
                actualSize = 0;

            if (actualSize > preallocatedBufferSize)
                actualSize = preallocatedBufferSize;
            
            _buffer = new byte[preallocatedBufferSize];

            ActualSize = actualSize;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="charArray"></param>
        public ByteDataCarrier([NotNull] char[] charArray)
        {
            if (ReferenceEquals(charArray,null))
                throw new ArgumentNullException("charArray");

            // do not use StringToUtf8... because bytes are needed after conversion anyway
            _buffer = Encoding.UTF8.GetBytes(charArray);

            ActualSize = _buffer.Length;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputChar"></param>
        public ByteDataCarrier(char inputChar)
        {
            // do not use StringToUtf8... because bytes are needed after conversion anyway
            _buffer = Encoding.UTF8.GetBytes(new[] { inputChar });

            ActualSize = _buffer.Length;
        }

        /// <summary>
        /// constructor WITHOUT data encapsulation
        /// </summary>
        /// <param name="preallocatedBufferSize">maximum size of the buffer to allocate</param>
        public ByteDataCarrier(int preallocatedBufferSize)
            :this(preallocatedBufferSize,0)
        {
        }

        /// <summary>
        /// constructor WITHOUT data encapsulation; 
        /// </summary>
        /// <param name="preallocatedBufferSize">maximum size of the buffer to allocate</param>
        /// <param name="maximizeActualSize">if true, actual size is set to maximum; otherwise set to 0</param>
        public ByteDataCarrier(int preallocatedBufferSize,bool maximizeActualSize)
            : this(preallocatedBufferSize, maximizeActualSize ? preallocatedBufferSize : 0)
        {
        }

        #region Cascaded size byte buffer pooling

        private class BufferPoolInfo
        {
            internal readonly int BufferSize;
            private readonly int _maxBuffers ;
            private volatile ByteBufferPool _bufferPool;

            /// <summary>
            /// instantiation on demand
            /// </summary>
            [NotNull]
            internal ByteBufferPool BufferPool
            {
                get
                {
                    if (_bufferPool == null)
                        lock(this)
                            if (_bufferPool == null)
                                _bufferPool = new ByteBufferPool(_maxBuffers,BufferSize);

                    return _bufferPool;
                }
            }



            internal BufferPoolInfo(int bufferSize, int maxBuffers)
            {
                _maxBuffers = maxBuffers;
                BufferSize = bufferSize;
                
            }
        }

        /// <summary>
        /// not expected to contain many elemnts
        /// 
        /// MUST BE PRE-SORTED BY BUFFER SIZE FROM LOWEST TO HIGHEST
        /// </summary>
        private static readonly BufferPoolInfo[] _cascadedBufferPools =
        {
            new BufferPoolInfo(64, 64),
            new BufferPoolInfo(256, 64),
            new BufferPoolInfo(512, 64),
            new BufferPoolInfo(1024, 64),
            new BufferPoolInfo(2048, 64),
            new BufferPoolInfo(8192, 64), 
            new BufferPoolInfo(16384, 64), 
            new BufferPoolInfo(32768, 64), 
            new BufferPoolInfo(65536, 64)
        };

        /// <summary>
        /// 
        /// </summary>
        [NotNull]
        public static byte[] GetPoolableBuffer(
            int expectedSize, 
            [CanBeNull] out ByteBufferPool bufferPool)
        {
            bufferPool = null;
            foreach (var bufferPoolInfo in _cascadedBufferPools)
            {
                if (bufferPoolInfo.BufferSize >= expectedSize)
                {
                    // makes also on-demand instantiation of the ByteBufferPool
                    bufferPool = bufferPoolInfo.BufferPool;
                    break;
                }
            }

            if (bufferPool == null)
                // no pooling 
                return new byte[expectedSize];
            
            return bufferPool.GetBuffer();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expectedSize"></param>
        /// <param name="lambdaForProcessingBuffer"></param>
        public static void ProcessPoolableBuffer(
            int expectedSize,
            [NotNull] Action<byte[], int> lambdaForProcessingBuffer)
        {
            Validator.CheckForNull(lambdaForProcessingBuffer,"lambdaForProcessingBuffer");

            ByteBufferPool bufferPool;
            var buffer = GetPoolableBuffer(expectedSize, out bufferPool);

            try
            {
                lambdaForProcessingBuffer(buffer, expectedSize);
            }
            finally
            {
                if (bufferPool != null)
                    try
                    {
                        bufferPool.ReturnBuffer(buffer);
                    }
                    catch
                    {
                        
                    }
            }
        }

        

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer">can contain random data after the actualBufferSize position !!!</param>
        /// <param name="actualBufferSize"></param>
        public delegate void DStringBytesLambda(
            [NotNull] byte[] buffer, 
            int actualBufferSize);

        /// <summary>
        /// general purpose string to byte-array convertor
        /// with pooled buffers for conversion
        /// </summary>
        /// <param name="inputString"></param>
        /// <param name="encoding">if null, UTF8 encoder will be used</param>
        /// <param name="stringBytesLambda"></param>
        public static void StringToBytes(
            [NotNull] string inputString,
            [CanBeNull] Encoding encoding,
            [NotNull] DStringBytesLambda stringBytesLambda
            )
        {

            Validator.CheckForNull(inputString, "inputString");
            Validator.CheckForNull(stringBytesLambda, "stringBytesLambda");

            if (ReferenceEquals(null, encoding))
                encoding = Encoding.UTF8;

            ByteBufferPool bufferPool = null;
            byte[] buffer = null;

            try
            {
                buffer = GetPoolableBuffer(encoding.GetByteCount(inputString), out bufferPool);

                int actualBufferSize = encoding.GetBytes(inputString, 0, inputString.Length, buffer, 0);

                stringBytesLambda(buffer, actualBufferSize);
            }
            finally
            {
                if (bufferPool != null && buffer !=null)
                    try
                    {
                        bufferPool.ReturnBuffer(buffer);
                    }
                    catch
                    {

                    }
            }

        }

        /// <summary>
        /// constructor with encapsulation
        /// </summary>
        /// <param name="inputString">string to encapsulate in byte UTF8 encoding</param>
        public ByteDataCarrier([NotNull] string inputString)
        {
            Validator.CheckForNull(inputString,"inputString");

            // do not use StringToUtf8... because bytes are needed after conversion anyway
            _buffer = Encoding.UTF8.GetBytes(inputString);
            
            _actualSize = _buffer.Length;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputString"></param>
        /// <param name="preallocatedBufferSize"></param>
        public ByteDataCarrier(
            [NotNull] string inputString,
            int preallocatedBufferSize
            )
        {
            Validator.CheckForNull(inputString, "inputString");

            StringToBytes(inputString, null,
                (buffer, actualBufferSize) =>
                {
                    int newSize = preallocatedBufferSize;

                    if (actualBufferSize > preallocatedBufferSize)
                        newSize = actualBufferSize;

                    _buffer = new byte[newSize];

                    Array.Copy(buffer, _buffer, actualBufferSize);

                    _actualSize = actualBufferSize;
                });
        }


        /// <summary>
        /// constructor with encapsulation
        /// </summary>
        /// <param name="inputStringBuilder">string to encapsulate in byte UTF8 encoding</param>
        public ByteDataCarrier([NotNull] StringBuilder inputStringBuilder)
        {
            if (ReferenceEquals(inputStringBuilder,null))
                throw new ArgumentNullException("inputStringBuilder");

            // do not use StringToUtf8... because bytes are needed after conversion anyway
            _buffer = Encoding.UTF8.GetBytes(inputStringBuilder.ToString());
            _actualSize = _buffer.Length;
        }

        /// <summary>
        /// copying constructor
        /// </summary>
        /// <param name="inputStream"></param>
        /// <param name="offset"></param>
        /// <param name="length">count of bytes to copy ;  
        /// if equal or less than zero, original stream length is supplied</param>
        public ByteDataCarrier(
            [NotNull] Stream inputStream,
            int offset, 
            int length)
        {
            if (ReferenceEquals(inputStream,null))
                throw new ArgumentNullException("inputStream");

            if (offset < 0)
                offset = 0;

            inputStream.Seek(offset, SeekOrigin.Begin);

            var inputLength = (int)inputStream.Length;
            if (length > 0 && length <= inputStream.Length)
                inputLength = length;

            _buffer = new byte[inputLength];
            _actualSize = inputStream.Read(_buffer, 0, inputLength);
           
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputStream"></param>
        public ByteDataCarrier([NotNull] Stream inputStream)
            :this(inputStream, 0, -1)
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Stream ToStream()
        {
            return new MemoryStream(_buffer, 0, _actualSize, true);
        }

        /// <summary>
        /// copy/re-encapsulation constructor 
        /// </summary>
        /// <param name="dataCarrier">data to clone</param>
        /// <param name="isCopy">if true, the data will be copied; otherwise the internal buffer will be encapsulated also by this instance</param>
        public ByteDataCarrier(
            [NotNull] ByteDataCarrier dataCarrier,
            bool isCopy)
        {
            if (ReferenceEquals(dataCarrier,null))
                throw new ArgumentNullException("dataCarrier");

            if (isCopy)
            {
                _buffer = new byte[dataCarrier._buffer.Length];

                Array.Copy(dataCarrier._buffer, _buffer, dataCarrier._actualSize);
                //Contal.IwQuick.Data.CByteArray.Copy(dataCarrier._buffer, _buffer, dataCarrier._actualSize);
            }
            else
            {
                _buffer = dataCarrier._buffer;
            }
        
            ActualSize = dataCarrier._actualSize;
            Offset = dataCarrier._offset;
        
        }

        /// <summary>
        /// returns the string representation of the buffer in UTF8 encoding
        /// </summary>
        /// <exception cref="ObjectDisposedException">object disposed by external Dispose call</exception>
        public string GetUTF8String()
        {
            CheckBufferDisposed();

            return Encoding.UTF8.GetString(_buffer, _offset, _actualSize);
        }

        /// <summary>
        /// sets the string into internally allocated buffer; string will be cropped, if exceeds max size
        /// </summary>
        /// <param name="inputString">string to copy into the buffer</param>
        public void SetUTF8String([CanBeNull] string inputString)
        {
            CheckBufferDisposed();

            if (string.IsNullOrEmpty(inputString))
            {
                //_offset = 0;
                _actualSize = 0;
                return;
            }

// ReSharper disable once AssignNullToNotNullAttribute
            StringToBytes(inputString, null,
                (buffer, actualBufferSize) =>
                {
                    // THIS IS DONE LIKE THIS CAUSE UTF8 CAN EXPAND THE BYTE SIZE OVER THE LENGTH OF THE STRING
                    int actualSize = actualBufferSize > _buffer.Length
                        ? _buffer.Length 
                        : actualBufferSize;

                    Array.Copy(buffer, _buffer, actualSize);

                    _actualSize = actualSize;
                });
           

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputString"></param>
        public void SetUTF8String(StringBuilder inputString)
        {
            SetUTF8String(inputString != null ? inputString.ToString() : String.Empty);
        }

        /// <summary>
        /// appends data from another ByteDataCarrier structure, enlarges the internal buffer if necessary
        /// </summary>
        /// <param name="inputData">data to append</param>
        /// <returns>true, if inputData are not empty</returns>
        /// <exception cref="ObjectDisposedException">object disposed by external Dispose call</exception>
        public bool Append([NotNull] ByteDataCarrier inputData)
        {
            return Append(inputData, AppendMode.EnlargeIfNecessary) > 0;
        }

        public enum AppendMode
        {
            EnlargeIfNecessary,
            TrimAppendedDataToFit,
            ReturnZeroIfCannotFit,
            ThrowExceptionIfCannotFit
        }

        /// <summary>
        /// appends data from another ByteDataCarrier structure, enlarges the internal buffer if necessary
        /// </summary>
        /// <param name="inputData">data to append</param>
        /// <param name="appendMode">defines mode of operation if data to be appended cannot fit into existing buffer</param>
        /// <exception cref="ObjectDisposedException">object disposed by external Dispose call</exception>
        /// <exception cref="ArgumentException">if the inputData cannot fit into currently preallocated buffer</exception>
        /// <returns>-1 if error; 0 if appended buffer cannot fit; amount of appended bytes</returns>
        public int Append(
            [NotNull] ByteDataCarrier inputData,
            AppendMode appendMode
            )
        {
            CheckBufferDisposed();

// ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (ReferenceEquals(inputData,null) ||
                inputData.ActualSize == 0)
                return -1;

            if (ActualSize + inputData.ActualSize > Size)
            {
                switch (appendMode)
                {
                    case AppendMode.EnlargeIfNecessary:
                        // some preallocation during growing
                        Grow(ActualSize + 2 * inputData.ActualSize, true);
                        break;
                    case AppendMode.ThrowExceptionIfCannotFit:
                        throw new ArgumentOutOfRangeException(
                            "Appended inputData["+inputData.ActualSize+"] cannot fit into preallocated size ["+
                            ActualSize + "/"+Size+"]");
                    case AppendMode.ReturnZeroIfCannotFit:
                        return 0;
                    case AppendMode.TrimAppendedDataToFit:
                        int trimmedCountToCopy = Size - ActualSize;
                        if (trimmedCountToCopy > 0)
                            Array.Copy(
                                inputData.Buffer,
                                0,
                                _buffer,
                                ActualSize,
                                trimmedCountToCopy
                                );

                        ActualSize = Size;

                        return trimmedCountToCopy;
                }

            }

            var inputDataActualSize = inputData.ActualSize; 
            
            Array.Copy(
                inputData._buffer, 
                0,
                _buffer, 
                ActualSize,
                inputDataActualSize);

            ActualSize += inputDataActualSize;

            return inputDataActualSize;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputString"></param>
        /// <param name="enlargeIfNecessary"></param>
        /// <returns></returns>
        public bool Append(
            [NotNull] string inputString,
            bool enlargeIfNecessary
            )
        {
// ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (string.IsNullOrEmpty(inputString))
                return false;

            bool result = false;

            StringToBytes(inputString, null,
                (buffer, actualBufferSize) =>
                {
                    result = Append(buffer,0, actualBufferSize, enlargeIfNecessary);
                });

            return result;
        }

        /// <summary>
        /// appends data from input buffer
        /// </summary>
        /// <param name="inputData">input buffer</param>
        /// <param name="enlargeIfNecessary">if true, the maximum size will be enlarged as necessary</param>
        /// <returns>true, if append succeeded ; false if inputData was empty or null</returns>
        /// <exception cref="ObjectDisposedException">if the buffer is already disposed</exception>
        public bool Append(
            [NotNull] byte[] inputData,
            bool enlargeIfNecessary)
        {
            return Append(inputData, 0, -1, enlargeIfNecessary);
        }

        /// <summary>
        /// appends data from input buffer
        /// </summary>
        /// <param name="inputData">input buffer</param>
        /// <param name="inputDataActualSize"></param>
        /// <param name="enlargeIfNecessary">if true, the maximum size will be enlarged as necessary</param>
        /// <returns>true, if append succeeded ; false if inputData was empty or null</returns>
        /// <exception cref="ObjectDisposedException">if the buffer is already disposed</exception>
        public bool Append(
            [NotNull] byte[] inputData,
            int inputDataActualSize,
            bool enlargeIfNecessary)
        {
            return Append(inputData, 0, inputDataActualSize, enlargeIfNecessary);
        }

        /// <summary>
        /// appends data from input buffer
        /// </summary>
        /// <param name="inputData">input buffer</param>
        /// <param name="inputDataOffset"></param>
        /// <param name="countToCopy"></param>
        /// <param name="enlargeIfNecessary">if true, the maximum size will be enlarged as necessary</param>
        /// <returns>true, if append succeeded ; false if inputData was empty or null</returns>
        /// <exception cref="ObjectDisposedException">if the buffer is already disposed</exception>
        public bool Append(
            [NotNull] byte[] inputData,
            int inputDataOffset,
            int countToCopy,
            bool enlargeIfNecessary)
        {
            CheckBufferDisposed();

// ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (ReferenceEquals(inputData,null)
                || inputData.Length == 0
                || countToCopy == 0 
                || inputDataOffset < 0 
                || inputDataOffset >= inputData.Length)
                return false;

            if (countToCopy < 0 
                || inputDataOffset + countToCopy > inputData.Length)
                countToCopy = inputData.Length - inputDataOffset;

            if (ActualSize + countToCopy > Size)
            {
                if (enlargeIfNecessary)
                {
                    // some preallocation during growing
                    Grow(ActualSize + 2*countToCopy,true);

                    /*
                    byte[] newBuffer = new byte[ActualSize + inputDataActualSize];
                    Array.Copy(_buffer, newBuffer, ActualSize);
                    _buffer = newBuffer;
                     */

                }
                else
                {
                    int remainingSpace = Size - ActualSize;
                    if (remainingSpace > 0)
                        Array.Copy(
                            inputData,
                            inputDataOffset,
                            _buffer,
                            ActualSize,
                            remainingSpace);

                    ActualSize = Size;

                    return true;
                }

            }

            Array.Copy(
                inputData,
                inputDataOffset,
                _buffer,
                ActualSize,
                countToCopy);


            ActualSize += countToCopy;

            return true;
            
        }

        /// <summary>
        /// appends data from input buffer, without enlarging buffer over maximum size ; 
        /// the overflow of data will not be appended
        /// </summary>
        /// <param name="inputData">input buffer</param>
        /// <returns>true, if append succeeded ; false if inputData was empty or null</returns>
        /// <exception cref="ObjectDisposedException">if the buffer is already disposed</exception>
        public bool Append([NotNull] byte[] inputData)
        {
            return Append(inputData,0, -1, true);
        }

        /// <summary>
        /// searches for the byte-array pattern in the haystack
        /// </summary>
        /// <param name="haystack">byte-address to search in</param>
        /// <param name="haystackOffset">offset to start search in haystack ; 
        /// if zero, negative or out of haystack , replaced by 0
        /// </param>
        /// <param name="haystackActualSize">count of bytes to be searched inside the haystack from offset ; 
        /// if zero, negative or bigger than haystack.Length, replaced by haystack.Length
        /// </param>
        /// <param name="pattern">the pattern to search for</param>
        /// <returns>position of start of pattern or -1 if not found</returns>
        /// <exception cref="ArgumentNullException">if haystack or pattern is null</exception>
        public static int IndexOf(
            [NotNull] byte[] haystack, 
            int haystackOffset, 
            int haystackActualSize,
            [NotNull] byte[] pattern)
        {
            return IndexOf(haystack, haystackOffset, haystackActualSize, pattern, -1);
        }

        /// <summary>
        /// searches for the byte-array pattern in the haystack
        /// </summary>
        /// <param name="haystack">byte-address to search in</param>
        /// <param name="haystackOffset">offset to start search in haystack ; 
        /// if zero, negative or out of haystack , replaced by 0
        /// </param>
        /// <param name="haystackActualSize">count of bytes to be searched inside the haystack from offset ; 
        /// if zero, negative or bigger than haystack.Length, replaced by haystack.Length
        /// </param>
        /// <param name="pattern">the pattern to search for</param>
        /// <param name="patternActualSize"></param>
        /// <exception cref="OutOfRangeException">if the haystackOffset is out of haystack's range</exception>
        /// <returns>position of start of pattern or -1 if not found</returns>
        /// <exception cref="ArgumentNullException">if haystack or pattern is null</exception>
        public static int IndexOf(
            [NotNull] byte[] haystack, 
            int haystackOffset, 
            int haystackActualSize,
            [NotNull] byte[] pattern,
            int patternActualSize)
        {
            if (ReferenceEquals(haystack,null))
                throw new ArgumentNullException("haystack");

            if (ReferenceEquals(pattern, null))
                throw new ArgumentNullException("pattern");

            if (haystackOffset >= haystack.Length)
                throw new OutOfRangeException(haystackOffset, 0, haystack.Length - 1);

            int posFound = -1;

            if (haystack.Length == 0
                || pattern.Length == 0
                || patternActualSize == 0)
                return posFound;

            if (patternActualSize < 1 || patternActualSize > pattern.Length)
                patternActualSize = pattern.Length;

            if (haystackOffset < 0)
                haystackOffset = 0;

            if (haystackActualSize <= 0)
                haystackActualSize = haystack.Length;
            else 
                if (haystackOffset + haystackActualSize > haystack.Length)
                    haystackActualSize = haystack.Length - haystackOffset;


            // TODO : http://en.wikipedia.org/wiki/Boyer%E2%80%93Moore_string_search_algorithm
            for (
                var i = haystackOffset; 
                i < haystackOffset + haystackActualSize - patternActualSize + 1;
                i++
                )
            {
                var subCounter = 0;
                for (var j = 0; j < patternActualSize; j++)
                {
                    if (i + j >= haystackOffset + haystackActualSize)
                        break;

                    if (haystack[i + j] == pattern[j])
                        subCounter++;
                    else
                        break;
                }

                if (subCounter == patternActualSize)
                {
                    posFound = i;
                    break;
                }
            }

            return posFound;

        }

        /// <summary>
        /// searches for the byte-array pattern
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="haystackOffset"></param>
        /// <returns>position of beginning of the pattern or -1 if not found</returns>
        public int IndexOf([NotNull] byte[] pattern, int haystackOffset)
        {
            CheckBufferDisposed();

            return IndexOf(_buffer, haystackOffset, _actualSize, pattern, -1);
        }

        /// <summary>
        /// searches for the byte-array pattern
        /// </summary>
        /// <param name="pattern"></param>
        /// <exception cref="InvalidOperationException">if pattern is null or zero-sized array</exception>
        /// <returns>position of beginning of the pattern or -1 if not found</returns>
        /// <exception cref="ArgumentNullException">if input data is null or empty, or if encoding is null</exception>
        /// <exception cref="ObjectDisposedException">object disposed by external Dispose call</exception>
        public int IndexOf([NotNull] byte[] pattern)
        {
            CheckBufferDisposed();

            return IndexOf(_buffer, 0, _actualSize, pattern, -1);
        }

        /// <summary>
        /// searches for the string pattern with specified encoding in the encapsulated buffer
        /// </summary>
        /// <param name="stringPattern">string pattern to search for</param>
        /// <param name="encoding">encoding, by which the string pattern is interpreted ; if null, UTF8 will be used</param>
        /// <returns>position of the beginning of the pattern or -1 if not found</returns>
        /// <exception cref="ArgumentNullException">if input data is null or empty</exception>
        /// <exception cref="ObjectDisposedException">object disposed by external Dispose call</exception>
        public int IndexOf(
            [NotNull] string stringPattern,
            Encoding encoding)
        {
            CheckBufferDisposed();
            Validator.CheckNullString(stringPattern,"stringPattern");

            int result = -1;

            StringToBytes(stringPattern, encoding,
                (buffer, actualBufferSize) =>
                {
                    result = IndexOf(_buffer, 0, _actualSize, buffer,actualBufferSize);
                }
                );

            return result;
        }

        /// <summary>
        /// searches for the string pattern with UTF8 encoding in the encapsulated buffer;
        /// returns position of the buffer or -1 if not found
        /// </summary>
        /// <param name="stringPattern">string pattern to search</param>
        /// <returns></returns>
        /// <exception cref="ObjectDisposedException">object disposed by external Dispose call</exception>
        public int IndexOf([NotNull] string stringPattern)
        {
            CheckBufferDisposed();
            return IndexOf(stringPattern, null); // means by UTF8 
        }

        /// <summary>
        /// sets the actual size to 0; optionally zeroes the buffer
        /// </summary>
        /// <param name="fillWithNulls">if true, zeroes the currently allocated buffer</param>
        /// <exception cref="ObjectDisposedException">object disposed by external Dispose call</exception>
        public void Clear(bool fillWithNulls)
        {
            CheckBufferDisposed();
            if (fillWithNulls)
                for (int i = 0; i < _buffer.Length; i++)
                    _buffer[i] = byte.MinValue;

            ActualSize = 0;
            Offset = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add(byte item)
        {
            Append(new[] {item});
        }

        /// <summary>
        /// sets the actual size to 0
        /// </summary>
        /// <exception cref="ObjectDisposedException">object disposed by external Dispose call</exception>
        public void Clear()
        {
            Clear(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(byte item)
        {
            return IndexOf(new[] {item}) >= 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(
            [NotNull] byte[] array, 
            int arrayIndex)
        {
            CopyTo(ref array,arrayIndex,0,-1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(byte item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        public int Count
        {
            get { return ActualSize; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="destinationBuffer">if supplied by not-null buffer, contents will be copied ; if supplied null, output buffer will be created</param>
        public void CopyTo([CanBeNull] ref byte[] destinationBuffer)
        {
            if (ReferenceEquals(destinationBuffer,null))
            {
                destinationBuffer = new byte[_actualSize];
                Array.Copy(_buffer, destinationBuffer, _actualSize);
            }
            else
            {
                Array.Copy(_buffer, destinationBuffer,
                    _actualSize <= destinationBuffer.Length ? _actualSize : destinationBuffer.Length);
            }
        }

        /// <summary>
        /// clones or copies the buffer according to the ActualSize
        /// </summary>
        /// <param name="destinationBuffer">if null, it's going to be created, otherwise it'll be used</param>
        /// <param name="sourceIndex"></param>
        /// <param name="sourceLength">if less than 0, the whole size according to source index is copied</param>
        public void CopyTo(
            [CanBeNull] ref byte[] destinationBuffer,
            int sourceIndex,
            int sourceLength)
        {
            CopyTo(ref destinationBuffer, 0, sourceIndex, sourceLength);
        }

        /// <summary>
        /// clones or copies the buffer according to the ActualSize
        /// </summary>
        /// <param name="destinationBuffer">if null, it's going to be created, otherwise it'll be used</param>
        /// <param name="destinationIndex"></param>
        /// <param name="sourceIndex"></param>
        /// <param name="sourceLength">if less than 0, the whole size according to source index is copied</param>
        public void CopyTo(
            [CanBeNull] ref byte[] destinationBuffer,
            int destinationIndex,
            int sourceIndex,
            int sourceLength)
        {
            if (sourceIndex < 0 ||
                sourceIndex >= _actualSize)
                sourceIndex = 0;

            if (destinationIndex < 0 ||
                (destinationBuffer!=null && destinationIndex >= destinationBuffer.Length) ||
                (destinationBuffer==null && destinationIndex >= sourceLength))
                destinationIndex = 0;

            if (sourceLength < 0)
            {
                // similar to copy all
                sourceLength = _actualSize - sourceIndex;
                
            }
            else
            {
                if (sourceIndex + sourceLength > _actualSize)
                    sourceLength = _actualSize - sourceIndex;
            }


            if (ReferenceEquals(null, destinationBuffer))
            {
                destinationBuffer = new byte[destinationIndex+sourceLength];

                Array.Copy(
                    _buffer, 
                    sourceIndex, 
                    destinationBuffer, 
                    destinationIndex, 
                    sourceLength);
            }
            else
            {
                Array.Copy(
                    _buffer, 
                    sourceIndex, 
                    destinationBuffer, 
                    destinationIndex,
                    destinationIndex + sourceLength <= destinationBuffer.Length 
                        ? sourceLength 
                        : destinationBuffer.Length - destinationIndex
                    );
            }
        }

        /// <summary>
        /// returns the byte value at specified position
        /// </summary>
        /// <param name="index">position at which to retrieve the value</param>
        /// <param name="ignoreActualSize"></param>
        /// <param name="autoSpanOnSet"></param>
        /// <returns></returns>
        /// <exception cref="ObjectDisposedException">object disposed by external Dispose call</exception>
        /// <exception cref="IndexOutOfRangeException">if the specified position is below 0 or exceeds actual size</exception>
        public byte this[int index,bool ignoreActualSize,bool autoSpanOnSet]
        {
            get
            {
                CheckBufferDisposed();
                Validator.CheckIntegerRange(index, 0, _buffer.Length - 1);

                if (!ignoreActualSize)
                    Validator.CheckIntegerRange(index, 0, _actualSize - 1);

                return _buffer[index];
            }
            set
            {
                CheckBufferDisposed();

                Validator.CheckIntegerRange(index, 0, _buffer.Length - 1);

                if (!ignoreActualSize)
                    Validator.CheckIntegerRange(index, 0, _actualSize - 1);
                else
                    if (autoSpanOnSet)
                    {
                        if (index > _actualSize - 1)
                            _actualSize = index + 1;
                    }

                _buffer[index] = value;
            }
        }

        /// <summary>
        /// reverses the data defined within _actualSize range
        /// </summary>
        public void Reverse()
        {
            int center = _actualSize/2;
           
            for (int i = 0; i < center; i++)
            {
                var tmp = _buffer[i];
                _buffer[i] = _buffer[_actualSize - i - 1];
                _buffer[_actualSize - i - 1] = tmp;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public byte this[int index]
        {
            get
            {
                return this[index, false, false];
            }
            set
            {
                this[index, false, false] = value;
            }
        }


        #region IEnumerable<byte> Members

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<byte> GetEnumerator()
        {
            return (IEnumerator<byte>)_buffer.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _buffer.GetEnumerator();
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// the buffer is disposed
        /// </summary>
        public void Dispose()
        {
            _buffer = null;
        }

        #endregion

        /// <summary>
        /// the buffer is disposed
        /// </summary>
        ~ByteDataCarrier()
        {
            _buffer = null;
        }

        /// <summary>
        /// returns string representation of the buffer in UTF8 encoding
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return GetUTF8String();
        }

#region Hex Dump
        private const string DefaultSeparator = " ";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data">if null or empty, String.Empty is being returned</param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="separator"></param>
        /// <param name="upperCase"></param>
        /// <returns></returns>
        [NotNull]
        public static string HexDump(
            [CanBeNull] byte[] data, 
            int offset, 
            int length,
            [CanBeNull] string separator, 
            bool upperCase)
        {
            if (ReferenceEquals(data,null) ||
                data.Length == 0)
                return String.Empty;

            if (offset < 0)
                offset = 0;

            if (offset >= data.Length)
                return String.Empty;

            if (offset + length > data.Length)
                length = data.Length - offset;

            int separatorLen = separator != null ? separator.Length : 0;

            try
            {
                int requiredSbLength = data.Length*(2 + separatorLen);

                StringBuilder sb;
                // more probable it'll be pooled
                bool pooled = true;
                string stringToReturn;

                if (requiredSbLength <= StringBuilderPool.Implicit2k.ImplicitCapacity)
                {
                    sb = StringBuilderPool.Implicit2k.Get();
                }
                else
                {
                    sb = new StringBuilder(requiredSbLength);
                    pooled = false;
                }

                try
                {
                    for (int i = offset; i < offset + length; i++)
                    {
                        if (separatorLen > 0)
                        {
                            if (i > offset)
                                sb.Append(separator);
                        }

                        sb.Append(upperCase
                            ? string.Format("{0:X2}", data[i])
                            : string.Format("{0:x2}", data[i]));
                    }

                    stringToReturn = sb.ToString();
                }
                finally
                {
                    if (pooled)
                        StringBuilderPool.Implicit2k.Return(sb);
                }

                return stringToReturn;
            }
            catch
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string HexDump(
            [CanBeNull] byte[] data, 
            int offset, 
            int length)
        {
            return HexDump(data, offset, length, DefaultSeparator, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [NotNull]
        public static string HexDump([CanBeNull] byte[] data)
        {
            if (ReferenceEquals(data, null))
                return string.Empty;

            return HexDump(data, 0, data.Length, DefaultSeparator, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="separator"></param>
        /// <param name="upperCase"></param>
        /// <returns></returns>
        [NotNull]
        public static string HexDump(
            [CanBeNull] byte[] data,
            [CanBeNull] string separator, 
            bool upperCase)
        {
            if (ReferenceEquals(data, null))
                return string.Empty;

            return HexDump(data, 0, data.Length, separator, upperCase);
        }

        /// <summary>
        /// returns space-separated list of hexadecimals interpreting the bytes of internal buffer
        /// </summary>
        /// <returns></returns>
        [NotNull]
        public string HexDump()
        {
            return HexDump(_buffer, _offset, _actualSize, DefaultSeparator, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="buffer"></param>
        /// <param name="parsedByteCount"></param>
        /// <returns></returns>
        [NotNull]
        public static byte[] ParseHexStringToByteArray(
            [CanBeNull] string input,
            [CanBeNull] byte[] buffer,
            out int parsedByteCount)
        {
            if (string.IsNullOrEmpty(input))
            {
                parsedByteCount = 0;
                return new byte[0]; // dummy/empty BDC
            }

// ReSharper disable once PossibleNullReferenceException
            input = input.Replace(StringConstants.SPACE, String.Empty);
            input = input.Replace(StringConstants.COLON, String.Empty);
            input = input.Replace("0x", String.Empty);
            input = input.Replace("h", String.Empty);

            if (buffer == null)
                buffer = new byte[input.Length / 2];

            int actualCount = 0;

            for (int i = 0; i < buffer.Length && i*2 < input.Length; i++)
            {
                try
                {
                    byte b = byte.Parse(input.Substring(i * 2, 2), NumberStyles.HexNumber);
                    buffer[actualCount++] = b;
                }
                catch
                {
                }
            }

            parsedByteCount = actualCount;
            return buffer;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="parsedByteCount"></param>
        /// <returns></returns>
        [NotNull]
        public static byte[] ParseHexStringToByteArray(
            [CanBeNull] string input,
            out int parsedByteCount)
        {
            return ParseHexStringToByteArray(input, null, out parsedByteCount);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [NotNull]
        public static byte[] ParseHexStringToByteArray([CanBeNull] string input)
        {
            int parsedByteCount;
            byte[] retOrTmp = ParseHexStringToByteArray(input, null, out parsedByteCount);

            if (retOrTmp.Length <= parsedByteCount)
                return retOrTmp;
            
            var newRet = new byte[parsedByteCount];
            Array.Copy(retOrTmp,newRet,parsedByteCount);
            return newRet;
        }

        /// <summary>
        /// parses byte data from string which is set of hexadecimal double-digits separated by space or colon
        /// </summary>
        /// <param name="input">string to parse</param>
        /// <returns></returns>
        [NotNull]
        public static ByteDataCarrier ParseFromHexString([CanBeNull] string input)
        {
            if (string.IsNullOrEmpty(input))
                return new ByteDataCarrier(1,0); // dummy/empty BDC

            int actualSize;
            var bufferToEncapsulate = ParseHexStringToByteArray(input, out actualSize);

            return new ByteDataCarrier(bufferToEncapsulate, 0, actualSize,true);
                
        }

#endregion

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [NotNull]
        public byte[] ToByteArray()
        {
            var newBuffer = new byte[_actualSize];

            Array.Copy(_buffer, newBuffer, _actualSize);

            return newBuffer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static explicit operator byte[](ByteDataCarrier data)
        {
            if (ReferenceEquals(data,null))
                return null;
            
            return data.Buffer;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class ByteBufferExtensions
    {
        /// <summary>
        /// makes a X2 per byte hex dump of the array
        /// </summary>
        /// <param name="byteArray"></param>
        /// <returns>String.Empty if array is empty</returns>
        [NotNull]
        public static string HexDump([NotNull] this byte[] byteArray)
        {
            return ByteDataCarrier.HexDump(byteArray);
        }


        /// <summary>
        /// searches for the byte-array pattern in the haystack
        /// </summary>
        /// <param name="haystack">byte-address to search in</param>
        /// <param name="offset">offset to start search in haystack ; 
        /// if zero, negative or out of haystack , replaced by 0
        /// </param>
        /// <param name="actualSize">count of bytes to be searched inside the haystack from offset ; 
        /// if zero, negative or bigger than haystack.Length, replaced by haystack.Length
        /// </param>
        /// <param name="pattern">the pattern to search for</param>
        /// <param name="patternActualSize"></param>
        /// <returns>position of start of pattern or -1 if not found</returns>
        /// <exception cref="ArgumentNullException">if haystack or pattern is null</exception>
        public static int IndexOf(
            [NotNull] this byte[] haystack,
            int offset,
            int actualSize,
            [NotNull] byte[] pattern,
            int patternActualSize)
        {
            return ByteDataCarrier.IndexOf(haystack, offset, actualSize, pattern, patternActualSize);
        }

        /// <summary>
        /// searches for the byte-array pattern in the haystack
        /// </summary>
        /// <param name="haystack">byte-address to search in</param>
        /// <param name="offset">offset to start search in haystack ; 
        /// if zero, negative or out of haystack , replaced by 0
        /// </param>
        /// <param name="actualSize">count of bytes to be searched inside the haystack from offset ; 
        /// if zero, negative or bigger than haystack.Length, replaced by haystack.Length
        /// </param>
        /// <param name="pattern">the pattern to search for</param>
        /// <returns>position of start of pattern or -1 if not found</returns>
        /// <exception cref="ArgumentNullException">if haystack or pattern is null</exception>
        public static int IndexOf(
            [NotNull] this byte[] haystack,
            int offset,
            int actualSize,
            [NotNull] byte[] pattern)
        {
            return IndexOf(haystack, offset, actualSize, pattern, -1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static int Parse(
            [NotNull] this byte[] buffer,
            [CanBeNull] string input
            )
        {
            int parsedByteCount;
            ByteDataCarrier.ParseHexStringToByteArray(input, buffer, out parsedByteCount);

            return parsedByteCount;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="bufferPool"></param>
        /// <returns></returns>
        public static bool TryReturnTo(
            this byte[] buffer,
            [CanBeNull] ByteBufferPool bufferPool)
        {
            if (buffer == null)
                return false;

            if (bufferPool == null)
                return false;

            try
            {
                bufferPool.ReturnBuffer(buffer);
                return true;
            }
            catch
            {
                return false;
            }

        }

    }
}
