using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Contal.Cgp.BaseLib;

namespace TestVisual.plugin
{
    public class TestVisual: ACgpVisualPlugin
    {
        private TestVisualForm _mainForm = null;

        public override string FriendlyName
        {
            get { return "TestVisual dummy plugin"; }
        }

        public override string Description
        {
            get { return "Helo"; }
        }

        public override Contal.IwQuick.ExtendedVersion Version
        {
            get { return null; }
        }

        public override void OnDispose()
        {
            //throw new NotImplementedException();
        }

        public override void Initialize()
        {
            
        }

        public override string[] FriendPlugins
        {
            get { throw new NotImplementedException(); }
        }

        protected override PluginMainForm CreateMainForm()
        {
            return new TestVisualForm();
        }

        public override Type GetRemotingProviderInterfaceType()
        {
            //throw new NotImplementedException();
            return null;
        }

        //public override Get
    }
}
