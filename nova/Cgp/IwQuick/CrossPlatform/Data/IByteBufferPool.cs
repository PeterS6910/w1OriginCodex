namespace Contal.IwQuick.Data
{
    public interface IByteBufferPool
    {
        byte[] GetBuffer();
        void ReturnBuffer(byte[] buffer);
    }
}