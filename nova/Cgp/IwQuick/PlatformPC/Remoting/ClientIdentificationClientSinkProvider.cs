#if TRACE_REMOTING
using System.Diagnostics;
#endif
using System.Runtime.Remoting.Channels;

namespace Contal.IwQuick.Remoting
{
    public class ClientIdentificationClientSinkProvider : IClientChannelSinkProvider
    {

        private IClientChannelSinkProvider _next;

        #region IClientChannelSinkProvider Members

        public IClientChannelSink CreateSink(IChannelSender channel, string url, object remoteChannelData)
        {
            IClientChannelSink ics = _next.CreateSink(channel, url, remoteChannelData);

#if TRACE_REMOTING
            Trace.WriteLine(
                string.Format(
                    "ClientIdentificationClientSinkProvider.CreateSink(url={0})",
                    url));
#endif
            return new ClientIdentificationClientSink(ics);
        }

        public IClientChannelSinkProvider Next
        {
            get
            {
                return _next;
            }
            set
            {
                _next = value;
            }
        }

        #endregion
    }
}
