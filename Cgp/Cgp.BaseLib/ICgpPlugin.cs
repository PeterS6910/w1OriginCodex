using System;
using System.Collections.Generic;

using Contal.IwQuick;

namespace Contal.Cgp.BaseLib
{
    public interface ICgpPlugin : IDisposable
    {
        void Initialize();
        void LoadSuccessful();

        /// <summary>
        /// CGP designation of the plugin in form [type_fullname]/[plugin_name]
        /// </summary>
        string CgpDesignation { get; }

        string FriendlyName { get; }

        /// <summary>
        /// 
        /// </summary>
        ExtendedVersion Version { get; }

        /// <summary>
        /// description of the plugin
        /// </summary>
        string Description { get; }

        void SetCgpServerDomain(AppDomain serverDomain);

        /// <summary>
        /// unloading functionality callable by the CgpPluginManager
        /// </summary>
        void Unload();

        Type GetRemotingProviderInterfaceType();
        void AssignSharedData(CgpPluginSharedData communicator);
        Dictionary<string, List<AccessPresentation>> GetPluginAccessList();
        void SetLanguage(string language);
    }
}