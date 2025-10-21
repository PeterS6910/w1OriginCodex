using System.Collections;
using System.Runtime.Remoting.Channels;

namespace Contal.IwQuick.Remoting
{
    class ClientIdentificationServerSinkProvider : IServerChannelSinkProvider
    {
        private IServerChannelSinkProvider _nextProvider;

        public ClientIdentificationServerSinkProvider()
        {
        }

        public ClientIdentificationServerSinkProvider(
                IDictionary properties,
                ICollection providerData)
        {
        }

        public IServerChannelSinkProvider Next
        {
            get { return _nextProvider; }
            set { _nextProvider = value; }
        }

        public IServerChannelSink CreateSink(IChannelReceiver channel)
        {
            IServerChannelSink nextSink = null;

            if (_nextProvider != null)
            {
                nextSink = _nextProvider.CreateSink(channel);
            }
            return new ClientIdentificationServerSink(nextSink);
        }

        public void GetChannelData(IChannelDataStore channelData)
        {
        }


    }
}
