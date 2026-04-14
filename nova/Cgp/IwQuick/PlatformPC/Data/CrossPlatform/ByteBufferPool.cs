using System;
using System.Threading;

#if COMPACT_FRAMEWORK
namespace Contal.IwQuickCF.Data
#else
namespace Contal.IwQuick.Data
#endif
{
	public class ByteBufferPool : IByteBufferPool
	{
		private readonly byte[][] _bufferPool;
		private int _current;
		private int _last;
		private readonly int _max;
		private readonly int _bufferSize;

		private object _controlCookie = "cookie object";

		public ByteBufferPool(int maxBuffers, int bufferSize)
		{
			_max = maxBuffers;
			_bufferPool = new byte[_max][];
			_bufferSize = bufferSize;
			_current = -1;
			_last = -1;
		}

		public byte[] GetBuffer()
		{
			object obj = null;
			byte[] result;

			try
			{
			    obj = 
                    Interlocked.Exchange(
                        ref _controlCookie, 
                        null);

			    if (obj == null)
			        result = new byte[_bufferSize];
			    else
			    {
			        if (_current == -1)
			        {
			            _controlCookie = obj;
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

			            _controlCookie = obj;
			            result = array;
			        }
			    }
			}
			catch (ThreadAbortException)
			{
				if (obj != null)
				{
					_current = -1;
					_last = -1;
					_controlCookie = obj;
				}

				throw;
			}

			return result;
		}

		public void ReturnBuffer(byte[] buffer)
		{
		    if (buffer == null)
		        throw new ArgumentNullException("buffer");

		    object obj = null;

			try
			{
				obj = Interlocked.Exchange(ref _controlCookie, null);

				if (obj != null)
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
					_controlCookie = obj;
				}
			}
			catch (ThreadAbortException)
			{
				if (obj != null)
				{
					_current = -1;
					_last = -1;
					_controlCookie = obj;
				}
				throw;
			}
		}
	}
}
