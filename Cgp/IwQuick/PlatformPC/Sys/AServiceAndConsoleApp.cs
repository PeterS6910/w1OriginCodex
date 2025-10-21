using System;
using System.Threading;

using Contal.IwQuick.Threads;

namespace Contal.IwQuick.Sys
{
    public enum GeneralMode
    {
        Runtime = 0,
        Maintenance = 1,
        Testing = 2,
        Verification = 3,
    }

    public abstract class AServiceAndConsoleApp:IDisposable
    {
        private readonly int _terminationWaitTime = 10 * 1000;

        /// <summary>
        /// main mutex indicating whether the service/application is running
        /// </summary>
        private readonly ManualResetEvent _exitMutex = new ManualResetEvent(false);

        /// <summary>
        /// instance of the main processing thread
        /// </summary>        
        private SafeThread _processingThread = null;

        
        private bool _runningAsService = false;
        /// <summary>
        /// indicates, whether the process is running as console application or service
        /// </summary>
        public bool RunningAsService
        {
            get { return _runningAsService; }
        }

        protected AServiceAndConsoleApp(int terminationWaitTime)
        {
            if (terminationWaitTime > 0)
                _terminationWaitTime = terminationWaitTime;
        }

        protected AServiceAndConsoleApp()
        {
        }

        
        private GeneralMode _generalMode = GeneralMode.Runtime;
        /// <summary>
        /// indicates the mode in which the service/console instance is running
        /// </summary>
        public GeneralMode GeneralMode
        {
            get { return _generalMode; }
        }


        /// <summary>
        /// starts the ProcessingThread routine
        /// </summary>
        /// <param name="runningAsService"></param>
        public void StartProcessing(bool runningAsService)
        {
            StartProcessing(runningAsService, GeneralMode.Runtime, false);

        }

        public void StartProcessing(bool runningAsService, bool STAThread)
        {
            StartProcessing(runningAsService, GeneralMode.Runtime, STAThread);

        }

        public void StartProcessing(bool runningAsService, GeneralMode generalMode, bool STA)
        {
            if (_processingThread != null)
                return;

            _runningAsService = runningAsService;
            if (_runningAsService)
                _generalMode = GeneralMode.Runtime;
            else
                _generalMode = generalMode;

            Preprocessing();

            _processingThread = new SafeThread(ProcessingThread);
            if (STA)
            {
                _processingThread.SetApartmentState(ApartmentState.STA);
            }
            _processingThread.OnFinished += OnProcessingThreadFinished;
            _processingThread.OnException += OnProcessingThreadException;
            _processingThread.Start();
        }

        public void StartProcessing(bool runningAsService, GeneralMode generalMode)
        {
            StartProcessing(runningAsService, generalMode, false);
        }

        void OnProcessingThreadException(Exception inputError)
        {
            
        }

        void OnProcessingThreadFinished()
        {
            _processingThread = null;
        }

        protected abstract void Preprocessing();

        /// <summary>
        /// should be used to close application from the inside
        /// </summary>
        protected void RequestStop()
        {
            _exitMutex.Set();
        }

        /// <summary>
        /// stops the service/console application process, either gracefuly or forcefuly after timeout
        /// </summary>
        public void StopProcessing()
        {
            SafeThread.StartThread(DoStopProcessing);
        }

        private void DoStopProcessing()
        {           
            RequestStop();

            if (_processingThread != null)
            {
                try
                {
                    if (!_processingThread.Join(_terminationWaitTime))
                        _processingThread.Abort();
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// waits for the processing threead to exit
        /// </summary>
        public void WaitForExit()
        {
            if (_processingThread != null)
                _processingThread.Join();
        }


        protected abstract void ProcessingThread();

        #region IDisposable Members

        public virtual void Dispose()
        {

        }

        #endregion

        protected void BlockUntilStopped()
        {
            _exitMutex.WaitOne();
        }
    }
}
