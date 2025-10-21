using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

using Contal.IwQuick;
using Contal.IwQuick.Remoting;

using System.Reflection;
using System.Text;
using JetBrains.Annotations;

namespace Contal.Cgp.BaseLib
{
    /// <summary>
    /// internal plugin destription structure
    /// </summary>
    public class CgpPluginDescriptor<TCgpPlugin>
        where TCgpPlugin : class, ICgpPlugin
    {
        public TCgpPlugin _plugin;
        public AppDomain _appDomain;
        public ExtendedVersion _assemblyVersion;
        public string _assemblyPath;
        public string _assemblyShortname;
        public Assembly _assembly;

        public void Clear()
        {
            _plugin = null;
            _appDomain = null;
            _assemblyVersion = null;
            _assemblyPath = null;
            _assemblyShortname = null;
            _assembly = null;
        }
    }

    /// <summary>
    /// implicit manager of the CGP plugins
    /// </summary>
    [Serializable]
    public class CgpPluginManager<TCgpPlugin, TCgpPluginDescriptor> : 
        MarshalByRefObject, 
        IEnumerable<TCgpPluginDescriptor>
        where TCgpPlugin : class, ICgpPlugin
        where TCgpPluginDescriptor : CgpPluginDescriptor<TCgpPlugin>, new()
    {
        private readonly Dictionary<string, TCgpPluginDescriptor> _plugins = 
            new Dictionary<string, TCgpPluginDescriptor>(4);

        private LinkedList<Assembly> hbmAssemblies = new LinkedList<Assembly>();

        public CgpPluginManager()
        {
            // for assembly reflectionOnly loading
            Thread.GetDomain().ReflectionOnlyAssemblyResolve += CgpPluginManager_ReflectionOnlyAssemblyResolve;
        }

        /// <summary>
        /// event called if single plugin loaded
        /// </summary>
        public event Action<TCgpPlugin> PluginLoaded;

        /// <summary>
        /// event called if single plugin loading failed
        /// </summary>
        public event DString2Void PluginFailed;

        /// <summary>
        /// event called if plugin designation does not match with that in license file
        /// </summary>
        public event DString2Void PluginUnlicensed;

        /// <summary>
        /// event called if single plugin loaded
        /// </summary>
        public event DString2Void PluginUnloaded;

        /// <summary>
        /// event called if set of plugins loaded
        /// </summary>
        public event Action<TCgpPlugin[]> PluginSetLoaded;

        /// <summary>
        /// event called if set of plugins loaded
        /// </summary>
        public event Action<TCgpPlugin[]> PluginSetChanged;


        private void InvokePluginLoaded(TCgpPlugin plugin)
        {
            plugin.LoadSuccessful();

            if (null != PluginLoaded)
                try
                {
                    PluginLoaded(plugin);
                }
                catch
                {
                }
        }

        private void InvokePluginFailed(string description)
        {
            if (null != PluginFailed)
            {
                try { PluginFailed(description); }
                catch { }
            }
        }

        private void InvokePluginUnlicensed(string description)
        {
            if (null != PluginUnlicensed)
                try { PluginUnlicensed(description); }
                catch { }
        }

        private void InvokePluginSetLoaded(LinkedList<TCgpPlugin> plugins)
        {
            if (null != PluginSetLoaded)
            {
                TCgpPlugin[] tmpPs;
                if (null == plugins)
                    tmpPs = new TCgpPlugin[0];
                else
                {
                    tmpPs = new TCgpPlugin[plugins.Count];
                    plugins.CopyTo(tmpPs, 0);
                }

                try
                {
                    PluginSetLoaded(tmpPs);
                }
                catch { }
            }
        }

        private void InvokePluginUnloaded(string cgpDesignation)
        {
            if (null != PluginUnloaded)
            {
                try { PluginUnloaded(cgpDesignation); }
                catch { }
            }
        }

        private void InvokePluginSetChanged()
        {
            if (null != PluginSetChanged)
            {
                TCgpPlugin[] tmpPs;
                lock (_plugins)
                {
                    tmpPs = new TCgpPlugin[_plugins.Count];

                    int i = 0;
                    foreach (TCgpPluginDescriptor cpd in _plugins.Values)
                    {
                        tmpPs[i++] = cpd._plugin;
                    }

                }

                try
                {
                    PluginSetChanged(tmpPs);
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// overall unhandled exception handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void CgpPluginManager_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {

            if (e.ExceptionObject is Exception)
            {
            }

            if (sender is AppDomain)
            {
                //AppDomain.Unload((AppDomain)sender);
            }
        }

        /// <summary>
        /// searches for the plugin with specified assembly
        /// </summary>
        /// <param name="assemblyPath"></param>
        /// <returns></returns>
        private TCgpPluginDescriptor FindPluginByAssembly(string assemblyPath)
        {
            if (Validator.IsNullString(assemblyPath))
                return null;

            lock (_plugins)
            {
                foreach (TCgpPluginDescriptor pd in _plugins.Values)
                {
                    if (null == pd)
                        continue;

                    if (pd._assemblyPath == assemblyPath)
                        return pd;
                }

                return null;
            }
        }

        private static bool IsAssignableFrom(
            [NotNull] Type requestedType, 
            [NotNull] Type comparedType)
        {
            Validator.CheckForNull(requestedType, "requestedType");
            Validator.CheckForNull(comparedType,"comparedType");

            if (requestedType.IsInterface)
                return comparedType.GetInterface(requestedType.FullName) != null;

            Type currentLevel = comparedType;

            while (currentLevel != null)
            {
                if (currentLevel.FullName == requestedType.FullName)
                    return true;

                currentLevel = currentLevel.BaseType;
            }

            return false;
        }

        private bool ExaminePluginAssembly(
            string path, 
            ref string assemblyName, 
            ref LinkedList<string> pluginTypes)
        {
            // check for assembly unicity
            if (FindPluginByAssembly(path) != null)
                return false;

            try
            {

                Assembly possiblePlugin = Assembly.ReflectionOnlyLoadFrom(path);


                Type[] types = possiblePlugin.GetExportedTypes();

                Type requestedType = typeof(TCgpPlugin);

                foreach (Type t in types)
                    try
                    {
                        if (!IsAssignableFrom(requestedType, t))
                            continue;

                        if (null == pluginTypes)
                            pluginTypes = new LinkedList<string>();

                        pluginTypes.AddLast(t.FullName);
                    }
                    catch (Exception ex)
                    {
                        Exception ex1 = ex;
                    }

                assemblyName = possiblePlugin.FullName;
                return (pluginTypes != null && pluginTypes.Count > 0);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// loads dependent assemblies if reflecting the plugin
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        static Assembly CgpPluginManager_ReflectionOnlyAssemblyResolve(object sender, ResolveEventArgs args)
        {
            return Assembly.ReflectionOnlyLoad(args.Name);
        }

        /// <summary>
        /// loads single plugin file;
        /// returns number of plugins successfuly loaded
        /// </summary>
        /// <param name="path">path to the assembly file</param>
        /// <param name="appDomainSeparation"></param>
        /// <param name="loadedPlugins">list to be filled with loaded plugins,
        /// can be null and then will created</param>
        /// <param name="failedPlugins">list to be filled with failed plugins,
        /// can be null and then won't be used</param>
        /// <param name="serverDomain"></param>
        /// <param name="demoLicence"></param>
        /// <exception cref="AlreadyExistsException"/>
        private int LoadPluginFile(
            string path, 
            bool appDomainSeparation,
            ref LinkedList<TCgpPlugin> loadedPlugins, 
            ref LinkedList<string> failedPlugins, 
            AppDomain serverDomain, 
            bool demoLicence)
        {
            Validator.CheckFileExists(path);

            string assemblyName = null;
            LinkedList<string> pluginTypes = null;

            string failedDesc;

            if (loadedPlugins == null)
                loadedPlugins = new LinkedList<TCgpPlugin>();

            if (ExaminePluginAssembly(path, ref assemblyName, ref pluginTypes))
            {
                lock (_plugins)
                {

                    //aName = aName.Substring(0,aName.IndexOf(','));
                    foreach (string typeFullname in pluginTypes)
                    {
                        try
                        {
                            // count on unicity of the specified path

                            AppDomain ad = 
                                appDomainSeparation 
                                    ? AppDomain.CreateDomain(path) 
                                    : AppDomain.CurrentDomain;

                            ad.UnhandledException += CgpPluginManager_UnhandledException;

                            var plugin = (TCgpPlugin)ad.CreateInstanceAndUnwrap(assemblyName, typeFullname);
                            
                            plugin.SetCgpServerDomain(serverDomain);

                            if (null == plugin)
                                throw new ArgumentNullException();

                            string cgpDesignation = null;
                            try
                            {
                                cgpDesignation = plugin.CgpDesignation;
                                StringBuilder computedDesignation = new StringBuilder("Contal.");
                                computedDesignation.Append(Path.GetFileNameWithoutExtension(path));
                                computedDesignation.Length = computedDesignation.ToString().IndexOf(".plugin");
                                if (!demoLicence && computedDesignation.ToString() != cgpDesignation.Remove(cgpDesignation.LastIndexOf('.')))
                                {
                                    failedDesc = path + "/" + cgpDesignation ?? "unknown";
                                    InvokePluginUnlicensed(failedDesc);

                                    if (null != failedPlugins)
                                        failedPlugins.AddLast(failedDesc);
                                    continue;
                                }

                                if (_plugins.ContainsKey(cgpDesignation))
                                {
                                    // it's duplicate anyway
                                    plugin.Unload();

                                    failedDesc = path + "/" + cgpDesignation ?? "unknown";
                                    // do not invoke here
                                    // might be useful to distinguish the error from duplicities
                                    //InvokePluginFailed(failedDesc);

                                    if (null != failedPlugins)
                                        failedPlugins.AddLast(failedDesc);
                                }
                                else
                                {
                                    // successful branch

                                    var pd = 
                                        new TCgpPluginDescriptor
                                        {
                                            _appDomain = ad,
                                            _assemblyPath = path,
                                            _assemblyShortname = assemblyName,
                                            _assemblyVersion = plugin.Version,
                                            _plugin = plugin,
                                            _assembly = plugin.GetType().Assembly
                                        };

                                    _plugins[cgpDesignation] = pd;

                                    loadedPlugins.AddLast(plugin);

                                    plugin.AssignSharedData(CgpPluginSharedData.Singleton);


                                    //Contal.IwQuick.Threads.SafeThread<ACgpPlugin>.StartThread(InitializePlugin, plugin);

                                    // let it pend here
                                    InitializePlugin(plugin);
                                }
                            }
                            catch (Exception ex)
                            {
                                failedDesc = path + "/" + (cgpDesignation ?? "unknown");
                                InvokePluginFailed(failedDesc + " (" + ex.GetType().FullName + ": " + ex.Message + ")");

                                if (null != failedPlugins)
                                    failedPlugins.AddLast(failedDesc);
                            }
                        }
                        catch (Exception ex)
                        {
                            InvokePluginFailed(path + " (" + ex.GetType().FullName + ": " + ex.Message + ")");
                            if (null != failedPlugins)
                                failedPlugins.AddLast(path);
                        }

                    }
                }
            }
            else
            {
                InvokePluginFailed(path);
                if (null != failedPlugins)
                    failedPlugins.AddLast(path);
            }


            return 
                null != loadedPlugins 
                    ? loadedPlugins.Count
                    : 0;
        }

        protected virtual void InitializePluginInternal(TCgpPlugin plugin)
        {
            plugin.Initialize();
        }

        private void InitializePlugin([NotNull] TCgpPlugin plugin)
        {
            Validator.CheckForNull(plugin,"plugin");

            InitializePluginInternal(plugin);

            InvokePluginLoaded(plugin);
        }

        /// <summary>
        /// loads single plugin binary;
        /// returns number of plugins successfuly loaded
        /// </summary>
        /// <param name="path">file path to the binary</param>
        /// <param name="appDomainSeparation"></param>
        /// <param name="serverDomain"></param>
        public void LoadPluginFile(
            string path, 
            bool appDomainSeparation, 
            AppDomain serverDomain)
        {
            var loadedPlugins = new LinkedList<TCgpPlugin>();
            LinkedList<string> failedPlugins = null;

            LoadPluginFile(
                path, 
                appDomainSeparation, 
                ref loadedPlugins, 
                ref failedPlugins, 
                serverDomain, 
                false);

            InvokePluginSetLoaded(loadedPlugins);
            if (null != loadedPlugins && loadedPlugins.Count > 0)
                InvokePluginSetChanged();
        }

        /// <summary>
        /// loads all plugin in specified directory by searching pattern
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="searchPatterns">variable list of search patterns to be used; if empty, or null, the *.dll will be used</param>
        public int LoadPlugins(string directory, bool appDomainSeparation, AppDomain serverDomain, bool demoLicence, IDictionary<string, bool> licencedPlugins, params string[] searchPatterns)
        {
            Validator.CheckNullString(directory);

            if (searchPatterns == null || searchPatterns.Length == 0)
                searchPatterns = new[] { "*.dll" };

            var _newPlugins = new LinkedList<TCgpPlugin>();
            LinkedList<string> _failed = null;

            int count = 0;

            foreach (string sp in searchPatterns)
            {
                if (Validator.IsNullString(sp))
                    continue;

                string[] paths = Directory.GetFiles(
                    directory, sp, SearchOption.TopDirectoryOnly);

                foreach (string path in paths)
                {
                    try
                    {
#if DEBUG
                        count += 
                            LoadPluginFile(
                                path, 
                                appDomainSeparation, 
                                ref _newPlugins, 
                                ref _failed, 
                                serverDomain, 
                                true);
#else
                        if (demoLicence || 
                                licencedPlugins.ContainsKey(Path.GetFileNameWithoutExtension(path)) && 
                                licencedPlugins[Path.GetFileNameWithoutExtension(path)])
                        {
                            count += 
                                LoadPluginFile(
                                    path, 
                                    appDomainSeparation, 
                                    ref _newPlugins, 
                                    ref _failed, 
                                    serverDomain, 
                                    demoLicence);
                        }
#endif
                    }
                    catch
                    {
                    }
                }
            }

            if (null != _newPlugins && _newPlugins.Count > 0)
            {
                InvokePluginSetLoaded(_newPlugins);
                InvokePluginSetChanged();
            }

            return count;
        }

        /// <summary>
        /// just to clear the plugin related data
        /// </summary>
        /// <param name="pluginDescriptor"></param>
        private static void UnloadPlugin(TCgpPluginDescriptor pluginDescriptor)
        {
            if (null == pluginDescriptor)
                return;

            try
            {
                pluginDescriptor._plugin.Unload();
            }
            catch
            {
            }

            try
            {
                AppDomain.Unload(pluginDescriptor._appDomain);
            }
            catch
            {
            }

            pluginDescriptor.Clear();
        }

        /// <summary>
        /// unloads the plugin by the CGP designation
        /// </summary>
        /// <param name="cgpDesignation"></param>
        public void UnloadPlugin(string cgpDesignation)
        {
            Validator.CheckNullString(cgpDesignation);

            lock (_plugins)
            {
                TCgpPluginDescriptor pd;

                if (_plugins.TryGetValue(cgpDesignation, out pd))
                {
                    try
                    {
                        _plugins.Remove(cgpDesignation);

                        UnloadPlugin(pd);

                        InvokePluginSetChanged();
                    }
                    catch
                    {
                    }
                }
            }
        }

        /// <summary>
        /// unloads plugin by his direct referrence
        /// </summary>
        /// <param name="plugin"></param>
        public void UnloadPlugin([NotNull] TCgpPlugin plugin)
        {
            Validator.CheckForNull(plugin,"plugin");

            TCgpPluginDescriptor pd;
            if (_plugins.TryGetValue(plugin.CgpDesignation, out pd))
                UnloadPlugin(pd);
        }

        /// <summary>
        /// unloads all plugins associated
        /// </summary>
        public void UnloadAllPlugins()
        {
            lock (_plugins)
            {
                int oldCount = _plugins.Count;

                foreach (TCgpPluginDescriptor pd in _plugins.Values)
                {
                    UnloadPlugin(pd);
                }


                _plugins.Clear();

                if (oldCount > 0)
                    InvokePluginSetChanged();
            }
        }


        /// <summary>
        /// returns all assemblies loaded via plugin loading process
        /// </summary>
        public LinkedList<Assembly> GetAssemblies()
        {
            var assemblies = new LinkedList<Assembly>();

            lock (_plugins)
            {
                try
                {
                    foreach (TCgpPluginDescriptor pd in _plugins.Values)
                    {
                        try
                        {
                            if (!assemblies.Contains(pd._assembly))
                                assemblies.AddLast(pd._assembly);
                        }
                        catch { }
                    }
                }
                catch
                {
                }
            }

            return assemblies;
        }

        public LinkedList<Assembly> GetHbmAssemblies()
        {
            return null;
        }


        #region IEnumerable<CgpPluginDescriptor> Members

        public IEnumerator<TCgpPluginDescriptor> GetEnumerator()
        {
            lock (_plugins)
            {
                return _plugins.Values.GetEnumerator();
            }
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            lock (_plugins)
            {
                return _plugins.Values.GetEnumerator();
            }
        }

        #endregion

        /// <summary>
        /// returns CGP Plugin descriptor for internal use
        /// </summary>
        /// <param name="cgpDesignation"></param>
        /// <returns></returns>
        private TCgpPluginDescriptor Get(string cgpDesignation)
        {
            if (Validator.IsNullString(cgpDesignation))
                return null;

            try
            {
                TCgpPluginDescriptor pd;
                return _plugins.TryGetValue(cgpDesignation, out pd) ? pd : null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// returns the instance of the plugin by it's CGP designation; 
        /// null if not found
        /// </summary>
        /// <param name="cgpDesignation">CGP string designation of the plugin</param>
        /// <exception cref="ArgumentNullException">if CGP designation is null or empty</exception>
        public TCgpPlugin this[string cgpDesignation]
        {
            get
            {
                Validator.CheckNullString(cgpDesignation);

                TCgpPluginDescriptor pd = Get(cgpDesignation);
                return null != pd ? pd._plugin : null;
            }
        }

        /// <summary>
        /// returns plugin's assembly if loaded;
        /// null if the plugin was not found by it's designation
        /// </summary>
        /// <param name="cgpDesignation"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">if CGP designation is null or empty</exception>
        public Assembly GetPluginAssembly(string cgpDesignation)
        {
            Validator.CheckNullString(cgpDesignation);

            TCgpPluginDescriptor pd = Get(cgpDesignation);
            return null != pd ? pd._assembly : null;
        }

        public Dictionary<string, List<AccessPresentation>> GetPluginsListAccess()
        {
            var accesses = new Dictionary<string, List<AccessPresentation>>();

            lock (_plugins)
            {
                foreach (TCgpPluginDescriptor pd in _plugins.Values)
                {
                    if (pd != null && pd._plugin != null)
                    {
                        var pluginAccesses = pd._plugin.GetPluginAccessList();
                        if (pluginAccesses != null)
                        {
                            foreach (var pluginAccessPresentationGroup in pluginAccesses)
                                accesses.Add(pluginAccessPresentationGroup.Key, pluginAccessPresentationGroup.Value);
                        }
                    }
                }
            }

            return accesses;
        }

        public void PluginsProvideObject(ref TcpRemotingPeer remotingPeer)
        {
            foreach (KeyValuePair<string, TCgpPluginDescriptor> kvp in _plugins)
            {
                try
                {
                    if (kvp.Value != null)
                    {
                        TCgpPlugin plugin = kvp.Value._plugin;

                        if (plugin != null)
                        {
                            Type remotingProviderType = plugin.GetType().Assembly.GetType(plugin.GetType() + "RemotingProvider");
                            var remotingProvider = (MarshalByRefObject)remotingProviderType.GetProperty("Singleton").GetValue(null, null);
                            remotingPeer.ProvideObject(remotingProvider, plugin.GetRemotingProviderInterfaceType());
                        }
                    }
                }
                catch
                {
                }
            }
        }

        public TCgpPlugin GetLoadedPlugin(string pluginFriendlyName)
        {
            foreach (KeyValuePair<string, TCgpPluginDescriptor> kvp in _plugins)
            {
                try
                {
                    if (kvp.Value != null && kvp.Value._plugin != null)
                    {
                        if (kvp.Value._plugin.FriendlyName == pluginFriendlyName)
                            return kvp.Value._plugin;
                    }
                }
                catch
                {
                }
            }

            return null;
        }

        public IEnumerable<TCgpPlugin> GetLoadedPlugins()
        {
            return
                _plugins.Values
                    .Select(pluginDescriptor => pluginDescriptor._plugin)
                    .Where(plugin => plugin != null)
                    .ToList();
        }
    }
}
