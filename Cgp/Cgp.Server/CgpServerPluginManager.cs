using Contal.Cgp.BaseLib;

namespace Contal.Cgp.Server
{
    public class CgpServerPluginManager : 
        CgpPluginManager<
            ACgpServerPlugin, 
            CgpPluginDescriptor<ACgpServerPlugin>>
    {
        public void InitDatabaseDefaults()
        {
            foreach (ACgpServerPlugin serverPlugin in GetLoadedPlugins())
                serverPlugin.InitDatabaseDefaults();
        }

        public void EnsureUpgradeDirectories()
        {
            foreach (ACgpServerPlugin serverPlugin in GetLoadedPlugins())
                serverPlugin.EnsureUpgradeDirectories();
        }

        public void RunDBTest()
        {
            foreach (ACgpServerPlugin serverPlugin in GetLoadedPlugins())
                serverPlugin.RunDBTest();
        }
    }
}
