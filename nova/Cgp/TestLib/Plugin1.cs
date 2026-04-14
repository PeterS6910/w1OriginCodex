using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;

using Contal.Cgp.BaseLib;
using Contal.IwQuick;
using Contal.IwQuick.UI;
using Contal.IwQuick.Threads;

namespace TestLib
{
    public class Plugin1: ACgpPlugin
    {
        //protected 

        int x = (int)DateTime.Now.Ticks;



        private void SeparateThread(int z)
        {
            
            Thread.Sleep(2000);
            Dialog.Info("Plugin thread "+z+" "+x+" "+DateTime.Now);

            /*Thread t = new Thread(new ThreadStart(SeparateThread2));
            t.IsBackground = true;
            t.Start();*/


            throw new ArgumentNullException("fake exception");
        }

        private void SeparateThread2()
        {
            Thread.Sleep(2000);
            Dialog.Info("Plugin thread " + " " + x + " " + DateTime.Now);
            throw new ArgumentNullException("fake exception 2");
        }

        public override string Description
        {
            get { throw new NotImplementedException(); }
        }


        private static Contal.IwQuick.ExtendedVersion _version = new Contal.IwQuick.ExtendedVersion(typeof(Plugin1), true, "");
        public override Contal.IwQuick.ExtendedVersion Version
        {
            get { return _version; }
        }

       

        public override void OnDispose()
        {
            Dialog.Info("Unloading ...");

            Dialog.Info("Destructing");
        }

        public override void Initialize()
        {


            AppDomain acd = Thread.GetDomain();
            string x = acd.FriendlyName;


            /*

            Thread.Sleep(1000);*/

            SafeThread<int> st = new SafeThread<int>(SeparateThread);

            st.Start(2);
        }

        public override string[] FriendPlugins
        {
            get { throw new NotImplementedException(); }
        }

        public override string FriendlyName
        {
            get { return "Plugin One"; }
        }

        public override Type GetRemotingProviderInterfaceType()
        {
            //throw new NotImplementedException();
            return null;
        }
    }
}
