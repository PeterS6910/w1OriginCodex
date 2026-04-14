namespace Contal.IwQuick.Data
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