using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.ServiceProcess;

namespace Contal.Cgp.Server
{
    public partial class CgpServerService : ServiceBase
    {
        public CgpServerService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            base.OnStart(args);
            CgpServer.Singleton.StartProcessing(true);
        }

        protected override void OnStop()
        {
            base.OnStop();
            CgpServer.Singleton.StopProcessing();
        }
    }
}
