using System;
using System.Collections.Generic;
using System.Text;

using System.IO;
using System.Runtime.InteropServices;
using System.Globalization;

#if !COMPACT_FRAMEWORK
namespace Contal.IwQuick.Data
#else
namespace Contal.IwQuickCF.Data
#endif
{
    /// <summary>
    /// carrier for byte-array data usually transfered via sockets
    /// </summary>
    public class ByteDataCarrier:IEnumerable<byte>,IDisposable
    {
        private readonly static Encoding _utf8 = Encoding.GetEncoding("UTF-8");

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
            if (null == _buffer)
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

                if (value >= _buffer.Length)
                {
                    // TODO : allow offset out of indexing for special scenarios ?
                    _offset = _buffer.Length;
                }
                else
                    _offset = value;
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
        /// COPYING constructor
        /// </summary>
        /// <param name="inputBuffer">data buffer to copy from</param>
        /// <param name="offset">offset of data</param>
        /// <param name="length">actual count of bytes to be copied</param>
        /// <exception cref="ArgumentNullException">if inputBuffer is null</exception>
        public ByteDataCarrier(byte[] inputBuffer, int offset, int length)
        {
            Validator.CheckNull(inputBuffer);
            Validator.CheckNegativeOrZeroInt(length);

            if (offset < 0)
                offset = 0;

            if (offset + length > inputBuffer.Length)
                length = inputBuffer.Length - offset;            
                

            _buffer = new byte[length];
            Array.Copy(inputBuffer, offset, _buffer, 0, length);

            Offset = 0;
            ActualSize = length;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputBuffer"></param>
        /// <param name="offset"></param>
        /// <param name="actualSize"></param>
        /// <param name="encapsulate"></param>
        public ByteDataCarrier(byte[] inputBuffer, int offset, int actualSize, bool encapsulate)
        {
            if (null == inputBuffer)
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
        public ByteDataCarrier(byte[] inputBuffer, int actualSize)
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
        /// <param name="maxSize">maximum size of the buffer to allocate</param>
        /// <param name="actualSize">actual size of the information in current allocated buffer</param>
        public ByteDataCarrier(int maxSize, int actualSize)
        {
            Validator.CheckNegativeInt(maxSize);
            Validator.CheckInvalidOperation(actualSize > maxSize || actualSize < 0);

            _buffer = new byte[maxSize];

            ActualSize = actualSize;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="charArray"></param>
        public ByteDataCarrier(char[] charArray)
        {
            if (null == charArray)
                throw new ArgumentNullException("charArray");

            _buffer = Encoding.UTF8.GetBytes(charArray);

            ActualSize = _buffer.Length;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputChar"></param>
        public ByteDataCarrier(char inputChar)
        {
            _buffer = Encoding.UTF8.GetBytes(new[] { inputChar });

            ActualSize = _buffer.Length;
        }

        /// <summary>
        /// constructor WITHOUT data encapsulation
        /// </summary>
        /// <param name="maxSize">maximum size of the buffer to allocate</param>
        public ByteDataCarrier(int maxSize)
            :this(maxSize,0)
        {
        }

        /// <summary>
        /// constructor WITHOUT data encapsulation; 
        /// </summary>
        /// <param name="maxSize">maximum size of the buffer to allocate</param>
        /// <param name="maximizeActualSize">if true, actual size is set to maximum; otherwise set to 0</param>
        public ByteDataCarrier(int maxSize,bool maximizeActualSize)
            : this(maxSize, maximizeActualSize ? maxSize : 0)
        {
        }

        /// <summary>
        /// constructor with encapsulation
        /// </summary>
        /// <param name="stringInput">string to encapsulate in byte UTF8 encoding</param>
        public ByteDataCarrier(string stringInput)
        {
            if (null == stringInput)
                throw new ArgumentNullException("stringInput");

            _buffer = _utf8.GetBytes(stringInput);
            _actualSize = _buffer.Length;
        }

        /// <summary>
        /// constructor with encapsulation
        /// </summary>
        /// <param name="stringBuilderInput">string to encapsulate in byte UTF8 encoding</param>
        public ByteDataCarrier(StringBuilder stringBuilderInput)
        {
            if (null == stringBuilderInput)
                throw new ArgumentNullException("stringBuilderInput");

            _buffer = _utf8.GetBytes(stringBuilderInput.ToString());
            _actualSize = _buffer.Length;
        }

        /// <summary>
        /// copying constructor
        /// </summary>
        /// <param name="inputStream"></param>
        /// <param name="offset"></param>
        /// <param name="length">count of bytes to copy ;  if equal or less than zero, original stream length is supplied</param>
        public ByteDataCarrier(Stream inputStream,int offset, int length)
        {
            if (null == inputStream)
                throw new ArgumentNullException("inputStream");

            if (offset < 0)
                offset = 0;

            inputStream.Seek(offset, SeekOrigin.Begin);

            int inputLength = (int)inputStream.Length;
            if (length > 0 && length <= inputStream.Length)
                inputLength = length;

            _buffer = new byte[inputLength];
            _actualSize = inputStream.Read(_buffer, 0, inputLength);
           
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        public ByteDataCarrier(Stream input)
            :this(input, 0, -1)
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
        public ByteDataCarrier(ByteDataCarrier dataCarrier,bool isCopy)
        {
            if (null == dataCarrier)
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
            return _utf8.GetString(_buffer, _offset, _actualSize);
        }

        /// <summary>
        /// sets the string into internally allocated buffer; string will be cropped, if exceeds max size
        /// </summary>
        /// <param name="inputString">string to copy into the buffer</param>
        public void SetUTF8String(string inputString)
        {
            CheckBufferDisposed();

            if (string.IsNullOrEmpty(inputString))
            {
                //_offset = 0;
                _actualSize = 0;
                return;
            }
                       
            byte[] utf8Bytes = _utf8.GetBytes(inputString);

            // DONE LIKE THIS CAUSE UTF8 CAN EXPAND THE BYTE SIZE OVER THE LENGTH OF THE STRING
            int actualSize = utf8Bytes.Length > _buffer.Length ? _buffer.Length : utf8Bytes.Length;

            Array.Copy(utf8Bytes, _buffer, actualSize);

            _actualSize = actualSize;

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
        public bool Append(ByteDataCarrier inputData)
        {
            CheckBufferDisposed();
            if (null == inputData ||
                inputData.ActualSize == 0)
                return false;

            if (ActualSize + inputData.ActualSize > Size)
            {
                // enlarging the internal buffer
                byte[] newBuffer = new byte[ActualSize + inputData.ActualSize];
                Array.Copy(_buffer, newBuffer, ActualSize);
                _buffer = newBuffer;

            }
            
            Array.Copy(inputData._buffer, 0,
                _buffer, ActualSize,
                inputData.ActualSize);

            ActualSize += inputData.ActualSize;

            return true;
        }

        /// <summary>
        /// appends data from input buffer
        /// </summary>
        /// <param name="inputData">input buffer</param>
        /// <param name="enlargeIfNecessary">if true, the maximum size will be enlarged as necessary</param>
        /// <returns>true, if append succeeded ; false if inputData was empty or null</returns>
        /// <exception cref="ObjectDisposedException">if the buffer is already disposed</exception>
        public bool Append(byte[] inputData,bool enlargeIfNecessary)
        {
            CheckBufferDisposed();
            if (null == inputData ||
                inputData.Length == 0)
                return false;

            if (ActualSize + inputData.Length > Size)
            {
                if (enlargeIfNecessary)
                {
                    byte[] newBuffer = new byte[ActualSize + inputData.Length];
                    Array.Copy(_buffer, newBuffer, ActualSize);
                    _buffer = newBuffer;

                }
                else
                {
                    Array.Copy(
                        inputData,
                        0,
                        _buffer, ActualSize, Size - ActualSize);

                    ActualSize = Size;

                    return true;
                }

            }

            Array.Copy(
                inputData,
                0,
                _buffer,
                ActualSize,
                inputData.Length);


            ActualSize += inputData.Length;

            return true;
            
        }

        /// <summary>
        /// appends data from input buffer, without enlarging buffer over maximum size ; 
        /// the overflow of data will not be appended
        /// </summary>
        /// <param name="inputData">input buffer</param>
        /// <returns>true, if append succeeded ; false if inputData was empty or null</returns>
        /// <exception cref="ObjectDisposedException">if the buffer is already disposed</exception>
        public bool Append(byte[] inputData)
        {
            return Append(inputData, true);
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
        /// <exception cref="ArgumentNullException">if haystack or pattern is either null or empty</exception>
        public static int IndexOf(byte[] haystack, int offset, int actualSize, byte[] pattern)
        {
            if (null == haystack ||
                haystack.Length == 0)
                throw new ArgumentNullException("haystack");

            if (null == pattern ||
                pattern.Length == 0)
                throw new ArgumentNullException("pattern");

            if (offset < 0 || offset >= haystack.Length)
                offset = 0;

            if (actualSize <= 0)
                actualSize = haystack.Length;

            if (offset + actualSize > haystack.Length)
                actualSize = haystack.Length - offset;

            int posFound = -1;
            for (int i = offset; i < offset+actualSize; i++)
            {
                int subCounter = 0;
                for (int j = 0; j < pattern.Length; j++)
                {
                    if (i + j >= offset + actualSize)
                        break;

                    if (haystack[i + j] == pattern[j])
                        subCounter++;
                    else
                        break;
                }

                if (subCounter == pattern.Length)
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
        /// <exception cref="InvalidOperationException">if pattern is null or zero-sized array</exception>
        /// <returns>position of beginning of the pattern or -1 if not found</returns>
        /// <exception cref="ArgumentNullException">if input data is null or empty, or if encoding is null</exception>
        /// <exception cref="ObjectDisposedException">object disposed by external Dispose call</exception>
        public int IndexOf(byte[] pattern)
        {
            CheckBufferDisposed();

            return IndexOf(_buffer, 0, _actualSize, pattern);
        }

        /// <summary>
        /// searches for the string pattern with specified encoding in the encapsulated buffer
        /// </summary>
        /// <param name="stringPattern">string pattern to search for</param>
        /// <param name="encoding">encoding, by which the string pattern is interpreted</param>
        /// <returns>position of the beginning of the pattern or -1 if not found</returns>
        /// <exception cref="ArgumentNullException">if input data is null or empty, or if encoding is null</exception>
        /// <exception cref="ObjectDisposedException">object disposed by external Dispose call</exception>
        public int IndexOf(string stringPattern,Encoding encoding)
        {
            CheckBufferDisposed();
            Validator.CheckNullString(stringPattern);
            Validator.CheckNull(encoding);

            byte[] mask = encoding.GetBytes(stringPattern);

            return IndexOf(mask);
        }

        /// <summary>
        /// searches for the string pattern with UTF8 encoding in the encapsulated buffer;
        /// returns position of the buffer or -1 if not found
        /// </summary>
        /// <param name="inputData">string pattern to search</param>
        /// <returns></returns>
        /// <exception cref="ObjectDisposedException">object disposed by external Dispose call</exception>
        public int IndexOf(string inputData)
        {
            CheckBufferDisposed();
            return IndexOf(inputData, _utf8);
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
        /// <param name="outBuffer">if supplied by not-null buffer, contents will be copied ; if supplied null, output buffer will be created</param>
        public void CopyTo(ref byte[] outBuffer)
        {
            if (null == outBuffer)
            {
                outBuffer = new byte[_actualSize];
                Array.Copy(_buffer, outBuffer, _actualSize);
            }
            else
            {
                Array.Copy(_buffer, outBuffer,
                    _actualSize <= outBuffer.Length ? _actualSize : outBuffer.Length);
            }
        }

        /// <summary>
        /// clones or copies the buffer according to the ActualSize
        /// </summary>
        /// <param name="outBuffer">if null, it's going to be created, otherwise it'll be used</param>
        /// <param name="sourceIndex"></param>
        /// <param name="sourceLength">if less than 0, the whole size according to source index is copied</param>
        public void CopyTo(ref byte[] outBuffer,int sourceIndex,int sourceLength)
        {
            if (sourceIndex < 0 || sourceIndex >= _actualSize)
                sourceIndex = 0;

            if (sourceLength < 0)
            {
                // similar to copy all
                sourceLength = _actualSize - sourceIndex;

                if (null != outBuffer && sourceLength > outBuffer.Length)
                    sourceLength = outBuffer.Length;
            }
            else
            {
                if (sourceIndex + sourceLength > _actualSize)
                    sourceLength = _actualSize - sourceIndex;

                if (null != outBuffer && sourceLength > outBuffer.Length)
                    sourceLength = outBuffer.Length;
            }

            if (null == outBuffer)
            {
                outBuffer = new byte[sourceLength];
                Array.Copy(_buffer, sourceIndex, outBuffer, 0, sourceLength);
            }
            else
            {
                Array.Copy(_buffer, sourceIndex, outBuffer, 0,
                    _actualSize <= outBuffer.Length ? _actualSize : outBuffer.Length);
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
        private const string DEFAULT_SEPARATOR = " ";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="separator"></param>
        /// <param name="upperCase"></param>
        /// <returns></returns>
        public static string HexDump(byte[] data, int offset, int length, string separator, bool upperCase)
        {
            if (null == data ||
                data.Length == 0)
                return String.Empty;

            if (offset < 0)
                offset = 0;

            if (offset >= data.Length)
                return String.Empty;

            if (offset + length > data.Length)
                length = data.Length - offset;

            try
            {
                StringBuilder sb = new StringBuilder(data.Length * 3);
                for (int i = offset; i < offset + length; i++)
                {
                    if (!string.IsNullOrEmpty(separator))
                    {
                        if (i > offset)
                            sb.Append(separator);
                    }

                    if (upperCase)
                        sb.Append(string.Format("{0:X2}", data[i]));
                    else
                        sb.Append(string.Format("{0:x2}", data[i]));
                }

                return sb.ToString();
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
        public static string HexDump(byte[] data, int offset, int length)
        {
            if (null == data)
                return String.Empty;
            return HexDump(data, offset, length, DEFAULT_SEPARATOR, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string HexDump(byte[] data)
        {
            if (null == data)
                return String.Empty;
            return HexDump(data, 0, data.Length, DEFAULT_SEPARATOR, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="separator"></param>
        /// <param name="upperCase"></param>
        /// <returns></returns>
        public static string HexDump(byte[] data, string separator, bool upperCase)
        {
            if (null == data)
                return String.Empty;
            return HexDump(data, 0, data.Length, separator, upperCase);
        }

        /// <summary>
        /// returns space-separated list of hexadecimals interpreting the bytes of internal buffer
        /// </summary>
        /// <returns></returns>
        public string HexDump()
        {
            return HexDump(_buffer, _offset, _actualSize, DEFAULT_SEPARATOR, true);
        }

        /// <summary>
        /// parses byte data from string which is set of hexadecimal double-digits separated by space or colon
        /// </summary>
        /// <param name="input">string to parse</param>
        /// <returns></returns>
        public static ByteDataCarrier ParseFromHexString(string input)
        {
            if (string.IsNullOrEmpty(input))
                return new ByteDataCarrier(4); // dummy/empty BDC

            input = input.Replace(StringConstants.SPACE, String.Empty);
            input = input.Replace(StringConstants.COLON, String.Empty);
            input = input.Replace("0x", String.Empty);
            input = input.Replace("h", String.Empty);
            
            byte[] bytesTmp = new byte[input.Length / 2];
            int actualCount = 0;

            for (int i = 0; i < bytesTmp.Length; i ++)
            {
                try
                {
                    byte b = byte.Parse(input.Substring(i*2, 2), NumberStyles.HexNumber);
                    bytesTmp[actualCount++] = b;
                }
                catch
                {
                }
            }

            return new ByteDataCarrier(bytesTmp, actualCount);
                
        }

#endregion

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public byte[] ToByteArray()
        {
            byte[] newBuffer = new byte[_actualSize];

            Array.Copy(_buffer, newBuffer, _actualSize);

            return newBuffer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static implicit operator byte[](ByteDataCarrier data)
        {
            if (null == data)
                return null;
            
            return data.Buffer;
        }
    }
}
