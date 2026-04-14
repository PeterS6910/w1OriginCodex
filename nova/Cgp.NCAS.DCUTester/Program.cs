using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Contal.Cgp.NCAS.DCUTester
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            /*string ciid = SLA.Client.SLAClientModule.Singleton.GetClientCIID();
            ciid = SLA.Client.SLAClientModule.Singleton.GetClientCIID();

            ciid = SLA.Client.SLAClientModule.Singleton.GetExternalCIID();
            ciid = SLA.Client.SLAClientModule.Singleton.GetExternalCIID();*/

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
