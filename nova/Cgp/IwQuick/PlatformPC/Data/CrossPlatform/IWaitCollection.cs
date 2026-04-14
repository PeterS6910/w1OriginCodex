#if !COMPACT_FRAMEWORK
namespace Contal.IwQuick.Data
#else
namespace Contal.IwQuickCF.Data
#endif
{
    /// <summary>
    /// 
    /// </summary>
    public interface IWaitCollection
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool WaitWhileEmpty(int timeout);

        /// <summary>
        /// 
        /// </summary>
        void WaitWhileEmpty();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        bool WaitUntilEmpty(int timeout);

        /// <summary>
        /// 
        /// </summary>
        void WaitUntilEmpty();
    }
}