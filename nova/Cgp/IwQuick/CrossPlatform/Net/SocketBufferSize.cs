namespace Contal.IwQuick.Net
{
    public class SocketBufferSize
    {
        protected int _bufferSize;


        public const int MinimalBufferSize = 1500;
        public const int DefaultBufferSize = 32768;

        public virtual int Value
        {
            get { return _bufferSize; }
            set {
                _bufferSize = value < MinimalBufferSize ? MinimalBufferSize : value;
            }
        }
    }
}
