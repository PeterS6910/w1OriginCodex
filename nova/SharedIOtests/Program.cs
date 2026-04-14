using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Contal.Drivers.LPC3250;
using Contal.IwQuickCF;

namespace SharedIOtests
{
    class Program
    {
        private void Run()
        {
            var x = SharedResources.Singleton.UnlockAdminMode(" ");
            x.SetInputAccess(12345,2,SRInputAccess.FullAccess,SRSetAccessMode.BatchSet);
            x.SetInputAccess(12345,3,SRInputAccess.FullAccess,SRSetAccessMode.BatchSet);

            x.SetOutputAccess(876, 3, SROutputAccess.FullAccess, SRSetAccessMode.BatchSet);

            var irc = x.OutputRecordCount;
            DebugHelper.Keep(irc);
            var z = x.GetAllInputs();

            foreach (var v in z)
            {
                foreach(var vv in v.Value.Values)
                {
                    DebugHelper.NOP(vv,v);

                }
            }
        }

        static void Main(string[] args)
        {
            Program p = new Program();
            p.Run();
        }
    }
}
