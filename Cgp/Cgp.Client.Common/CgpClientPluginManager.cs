using System;

using Contal.Cgp.BaseLib;
using Contal.IwQuick;
using Contal.IwQuick.Remoting;

namespace Contal.Cgp.Client.Common
{
    public class CgpClientPluginManager : 
        CgpPluginManager<
            ICgpClientPlugin, 
            CgpClientPluginDescriptor>
    {
        public static object CreateGeneric(Type generic, Type innerType, params object[] args)
        {
            return 
                Activator.CreateInstance(
                    generic.MakeGenericType(new[] { innerType }), 
                    args);
        }

        public void CreatePluginsProxyKeeper(
            ref TcpRemotingPeer remotingPeer, 
            Action<ICgpClientPlugin> proxyGained, 
            Action<ICgpClientPlugin> proxyLost)
        {
            foreach (var cgpClientPluginDescriptor in this)
                try
                {
                    ICgpClientPlugin plugin = cgpClientPluginDescriptor._plugin;

                    if (plugin != null)
                        cgpClientPluginDescriptor._pluginRemotingProxyKeeper =
                            CreateGeneric(
                                typeof(PluginRemotingProxyKeeper<>),
                                plugin.GetRemotingProviderInterfaceType(),
                                remotingPeer,
                                plugin,
                                proxyGained,
                                proxyLost);
                }
                catch
                {
                }
        }

        public void PreRegisterAttachCallbackHandlers()
        {
            foreach (var clientPlugin in GetLoadedPlugins())
                try
                {
                    clientPlugin.PreRegisterAttachCallbackHandlers();
                }
                catch
                {
                }
        }
    }
}
