using System;

using Contal.IwQuick;
using Contal.IwQuick.Remoting;

namespace Contal.Cgp.Client.Common
{
    public class PluginRemotingProxyKeeper<TPluginRemotingProviderInterface>
        where TPluginRemotingProviderInterface : class
    {
        private readonly ICgpClientPlugin _plugin;
        private readonly Action<ICgpClientPlugin> _proxyGained;
        private readonly Action<ICgpClientPlugin> _proxyLost;

        private RemotingProxyKeeper<TPluginRemotingProviderInterface> _remotingProxyKeeper;

        public PluginRemotingProxyKeeper(
            TcpRemotingPeer remotingPeer, 
            ICgpClientPlugin plugin, 
            Action<ICgpClientPlugin> proxyGained,
            Action<ICgpClientPlugin> proxyLost)
        {
            _plugin = plugin;
            _proxyGained = proxyGained;
            _proxyLost = proxyLost;

            _remotingProxyKeeper = 
                new RemotingProxyKeeper<TPluginRemotingProviderInterface>(remotingPeer);

            _remotingProxyKeeper.ProxyGained += _pluginProviderKeeper_ProxyGained;
            _remotingProxyKeeper.ProxyLost += _pluginProviderKeeper_ProxyLost;

            _remotingProxyKeeper.Start();
        }

        void _pluginProviderKeeper_ProxyGained(TPluginRemotingProviderInterface parameter)
        {
            _plugin.SetRemotingProviderInterface(parameter);

            if (_proxyGained != null)
                _proxyGained(_plugin);
        }

        void _pluginProviderKeeper_ProxyLost(Type parameter)
        {
            _plugin.SetRemotingProviderInterface(null);

            if (_proxyLost != null)
                _proxyLost(_plugin);
        }
    }
}