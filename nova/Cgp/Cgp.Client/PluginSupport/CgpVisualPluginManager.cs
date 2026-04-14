using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using Contal.Cgp.Client.Common;
using Contal.IwQuick;

namespace Contal.Cgp.Client.PluginSupport
{
    public class CgpVisualPluginManager : CgpClientPluginManager
    {
        public IEnumerable<ICgpVisualPlugin> GetVisualPlugins()
        {
            return GetLoadedPlugins()
                .OfType<ICgpVisualPlugin>();
        }

        private Form _implicitParent;
        public Form ImplicitParent
        {
            get { return _implicitParent; }
            set
            {
                _implicitParent = value;
            }
        }

        protected override void InitializePluginInternal(ICgpClientPlugin plugin)
        {
            var visualPlugin = plugin as ICgpVisualPlugin;

            if (visualPlugin != null)
                _implicitParent.Invoke(
                    new Action<Form>(visualPlugin.InitializeUI),
                    _implicitParent);
            else
                base.InitializePluginInternal(plugin);
        }
    }
}