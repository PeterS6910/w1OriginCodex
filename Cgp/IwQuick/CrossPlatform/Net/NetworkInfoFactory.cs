namespace Contal.IwQuick.Net
{
    public class NetworkInfoFactory
    {
        public static readonly NetworkInfoFactory Singleton = new NetworkInfoFactory();

        private NetworkInfoFactory()
        {
            
        }

        // Obtain method to be extended by platform-dependent implementation
    }
}
