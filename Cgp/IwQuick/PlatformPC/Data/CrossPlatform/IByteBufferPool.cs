#if COMPACT_FRAMEWORK
namespace Contal.IwQuickCF.Data
#else
namespace Contal.IwQuick.Data
#endif
{
    public interface IByteBufferPool
    {
        byte[] GetBuffer();
        void ReturnBuffer(byte[] buffer);
    }
}