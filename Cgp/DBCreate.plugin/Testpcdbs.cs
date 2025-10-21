using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Contal.Cgp.BaseLib;
using Contal.Cgp.DBSCreator;
using System.Reflection;

namespace DBCreate
{
    public class Testpcdbs : ACgpPlugin
    {
        public Testpcdbs():base()
        {
            //Manager mng = new Manager();
            ////mng.OrmSchema = Assembly.GetCallingAssembly();
            //mng.RunSettings();
        }

        public override string Description
        {
            get { throw new NotImplementedException(); }
        }

        public override void OnDispose()
        {
        }

        private static Contal.IwQuick.ExtendedVersion _version = new Contal.IwQuick.ExtendedVersion(typeof(Testpcdbs), true, "");
        public override Contal.IwQuick.ExtendedVersion Version
        {
            get { return _version; }
        }

        public override void Initialize()
        {
            
        }

        public override string[] FriendPlugins
        {
            get { throw new NotImplementedException(); }
        }

        public override string FriendlyName
        {
            get { return "Testcpdbs"; }
        }
    }
}
