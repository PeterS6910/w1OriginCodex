using System;
using System.Collections.Generic;

using Contal.IwQuick;
using JetBrains.Annotations;

namespace Contal.Cgp.BaseLib
{
    /// <summary>
    /// abstract ancestor class for all plugins integrated into CGP
    /// </summary>
    public abstract class ACgpPlugin<TCgpPlugin> : 
        MarshalByRefObject, 
        ICgpPlugin
        where TCgpPlugin : ACgpPlugin<TCgpPlugin>
    {
        private static TCgpPlugin _singleton;
        /// <summary>
        /// non-initializing referrence to singleton-instance;
        /// can be null, if the plugin was not loaded
        /// </summary>
        public static TCgpPlugin Singleton
        {
            get { return _singleton; }
        }

        private bool _loaded;
        /// <summary>
        /// returns true, if the plugin was loaded
        /// </summary>
        public static bool IsLoaded
        {
            get { return (null != _singleton && _singleton._loaded); }
        }

        public virtual void LoadSuccessful()
        {
        }

        public abstract string FriendlyName { get; }

        /// <summary>
        /// implicit constructor; it should not proceed with any processing;
        /// for that purpose there is the Main method
        /// </summary>
        /// <exception cref="Contal.IwQuick.AlreadyExistsException">
        /// if thrown, the singleton was already instantiated; 
        /// can be retrieved over Data["singleton"] referrence
        /// </exception>
        protected ACgpPlugin()
        {
            if (null != _singleton)
            {
                Exception e = new AlreadyExistsException(_singleton.GetType().FullName);
                e.Data["singleton"] = _singleton;
                throw e;
            }

            _singleton = (TCgpPlugin)this;
            _loaded = true;

        }

        /// <summary>
        /// description of the plugin
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        /// plugins to be trusted in inter-plugin communication
        /// </summary>
        public abstract string[] FriendPlugins { get; }

        /// <summary>
        /// 
        /// </summary>
        public abstract ExtendedVersion Version { get; }

        private string _cgpDesignation;
        /// <summary>
        /// CGP designation of the plugin in form [type_fullname]/[plugin_name]
        /// </summary>
        public string CgpDesignation
        {
            get
            {
                /*if (null == Name)
                    return null;*/

                return _cgpDesignation ?? (_cgpDesignation = GetType().ToString());
            }
        }

        /// <summary>
        /// unloading functionality callable by the CgpPluginManager
        /// </summary>
        public void Unload()
        {
            Dispose();
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (_loaded)
            {
                _singleton = null;

                try { OnDispose(); }
                catch { }

                _loaded = false;
            }
        }

        public abstract void OnDispose();

        #endregion

        ~ACgpPlugin()
        {
            Dispose();
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        private CgpPluginSharedData _sharedData;

        protected CgpPluginSharedData Communicator
        {
            get { return _sharedData; }
        }

        public void AssignSharedData([NotNull] CgpPluginSharedData communicator)
        {
            Validator.CheckForNull(communicator,"communicator");

            _sharedData = communicator;
        }

        protected void RegisterData(string dataName, object data)
        {
            if (null == _sharedData)
                return;

            _sharedData.RegisterData(this, dataName, data);
        }

        protected void RegisterGlobalData(long dataKey, object data)
        {
            if (null == _sharedData)
                return;

            _sharedData.RegisterGlobalData(dataKey, data);
        }

        protected void UnregisterGlobalData(long dataKey)
        {
            if (null == _sharedData)
                return;

            _sharedData.UnregisterGlobalData(dataKey);
        }

        /// <summary>
        /// the main processing method
        /// </summary>
        public abstract void Initialize();

        public abstract Type GetRemotingProviderInterfaceType();

        public virtual Dictionary<string, List<AccessPresentation>> GetPluginAccessList()
        {
            return null;
        }

        protected AppDomain _serverDomain = null;
        public AppDomain ServerDomain { get { return _serverDomain; } }

        public void SetCgpServerDomain(AppDomain serverDomain)
        {
            _serverDomain = serverDomain;
        }

        public virtual void SetLanguage(string language)
        {
        }
    }
}
