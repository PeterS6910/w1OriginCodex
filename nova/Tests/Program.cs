using System;
using System.Security.Policy;
using Contal.IwQuick.Data;
using Contal.IwQuick.Threads;
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics;
using Contal.IwQuick;
using JetBrains.Annotations;

namespace Tests
{
    public class X 
    { 
        private int _x = 1; 

        internal X()
        {
            
        }
    }

    
    
    public class Program:IDisposable
    {
        private readonly EventWaitHandlePool<ManualResetEvent> mrep =
            new EventWaitHandlePool<ManualResetEvent>();

        private void Zone([CanBeNull] out object z)
        {
            if (Environment.Version.Build == 30)
            z = new object();
            else
            {
                z = null;
            }
        }

        

        private void Run()
        {
            object xx;
            Zone(out xx);
            xx.ToString();

            var cis = typeof(X).GetConstructors();

            DebugHelper.NOP(cis);

            //LevelSortedQueueTest.Singleton.Run();

            //PQWaitingTest.Singleton.Run();

            PQBasicTest.Singleton.Run();


            mrep.Get().WaitOne();
        }

        

        static void Main(string[] args)
        {
            using (var p = new Program())
            {
                p.Run();
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            
        }

        #endregion
    }
}
