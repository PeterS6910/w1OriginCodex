namespace Contal.IwQuick.Threads
{
    public class DefaultSafeThreadContextFactory:ISafeThreadContextFactory
    {
        public static readonly ISafeThreadContextFactory Singleton = new DefaultSafeThreadContextFactory();

        public ISafeThreadContext Create()
        {
            return new SafeThreadContext();
        }
    }
}
