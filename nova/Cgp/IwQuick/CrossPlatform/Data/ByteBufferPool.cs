using System;
using System.Threading;
using JetBrains.Annotations;

namespace Contal.IwQuick.Data
{
    /// <summary>
    /// 
    /// </summary>
	public class ByteBufferPool : IByteBufferPool
	{
		private readonly byte[][] _bufferPool;
		private int _current;
		private int _last;
		private readonly int _max;
		private readonly int _bufferSize;

		private object _controlCookie = new object();


		/// <summary>
		/// 
		/// </summary>
		/// <param name="maxBuffers"></param>
		/// <param name="bufferSize"></param>
		public ByteBufferPool(int maxBuffers, int bufferSize)
		{
			_max = maxBuffers;
			_bufferPool = new byte[_max][];
			_bufferSize = bufferSize;
			_current = -1;
			_last = -1;
		}

        /// <summary>
        /// 
        /// </summary>
        public int BufferSize
        {
            get { return _bufferSize; }
        }

        private object TakeCookie()
        {
            return Interlocked.Exchange(ref _controlCookie, null);
        }

        private void ReturnCookie([NotNull] object cookie)
        {
#if DEBUG
            Validator.CheckForNull(cookie,"cookie");
#endif

            Interlocked.Exchange(ref _controlCookie, cookie);
        }

        /// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public byte[] GetBuffer()
		{
			object cookieTaken = null;
			byte[] result;

			try
			{
			    cookieTaken = TakeCookie();
			    

			    if (cookieTaken == null)
			        result = new byte[_bufferSize];
			    else
			    {
			        if (_current == -1)
			        {
			            ReturnCookie(cookieTaken);
			            result = new byte[_bufferSize];
			        }
			        else
			        {
			            byte[] array = _bufferPool[_current];
			            _bufferPool[_current] = null;

			            _current =
			                _current == _last
			                    ? -1
			                    : (_current + 1)%_max;

			            ReturnCookie(cookieTaken);
			            result = array;
			        }
			    }
			}
			catch (ThreadAbortException)
			{
				if (cookieTaken != null)
				{
					_current = -1;
					_last = -1;
					ReturnCookie(cookieTaken);
				}

				throw;
			}

			return result;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
		public void ReturnBuffer(byte[] buffer)
		{
		    if (buffer == null)
		        throw new ArgumentNullException("buffer");

		    object cookieTaken = null;

			try
			{
			    cookieTaken = TakeCookie();

				if (cookieTaken != null)
				{
					if (_current == -1)
					{
						_bufferPool[0] = buffer;
						_current = 0;
						_last = 0;
					}
					else
					{
						int num = (_last + 1) % _max;
						if (num != _current)
						{
							_last = num;
							_bufferPool[_last] = buffer;
						}
					}
					ReturnCookie(cookieTaken);
				}
			}
			catch (ThreadAbortException)
			{
				if (cookieTaken != null)
				{
					_current = -1;
					_last = -1;
					ReturnCookie(cookieTaken);
				}
				throw;
			}
		}
	}
}
