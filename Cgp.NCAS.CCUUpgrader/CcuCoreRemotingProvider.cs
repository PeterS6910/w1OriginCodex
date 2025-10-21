using System;

using Contal.IwQuick;
using Contal.IwQuick.Net;
using Contal.LwRemoting2;
using Contal.IwQuick.Sys;

namespace Cgp.NCAS.CCUUpgrader
{
    class CcuCoreRemotingProvider : ALwRemotingService
    {
        private static volatile CcuCoreRemotingProvider _singleton;
        private static readonly object _syncRoot = new object();

        public static CcuCoreRemotingProvider Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (_singleton == null)
                            _singleton = new CcuCoreRemotingProvider();
                    }

                return _singleton;
            }
        }
        public event DObjects2Void CcuUpgradeFileUnpackProgress;
        public void DoCcuUpgradeFileUnpackProgress(params object[] param)
        {
#if DEBUG
            CcuUpgradeHandler.Log.Info("DoCcuUpgradeFileUnpackProgress");
#endif
            if (CcuUpgradeFileUnpackProgress != null)
            {
                try
                {
                    CcuUpgradeFileUnpackProgress(param);
#if DEBUG
                    CcuUpgradeHandler.Log.Info("CcuUpgradeFileUnpackProgress reported to event");
#endif
                }
                catch (Exception error)
                {
                    HandledExceptionAdapter.Examine(error);
#if DEBUG
                    CcuUpgradeHandler.Log.Info("CcuUpgradeFileUnpackProgress event failed");
#endif
                }
            }
#if DEBUG
            else
            {

                CcuUpgradeHandler.Log.Info("No events for CcuUpgradeFileUnpackProgress");

            }
#endif
        }

        public event DObjects2Void CcuUpgradeFinished;
        public void DoCcuUpgradeFinished(params object[] param)
        {
#if DEBUG
            CcuUpgradeHandler.Log.Info("DoCcuUpgradeFinished");
#endif
            if (CcuUpgradeFinished != null)
            {
                try
                {
                    CcuUpgradeFinished(param);
#if DEBUG
                    CcuUpgradeHandler.Log.Info("CcuUpgradeFinished reported to event");
#endif
                }
                catch (Exception error)
                {
                    HandledExceptionAdapter.Examine(error);
#if DEBUG
                    CcuUpgradeHandler.Log.Info("CcuUpgradeFinished event failed");
#endif
                }
            }
#if DEBUG
            else
            {

                CcuUpgradeHandler.Log.Info("No events for CcuUpgradeFinished");

            }
#endif
        }


        public static object[] AuthenticateClient(ISimpleTcpConnection param1, object[] parameters)
        {
            if (parameters.Length > 0 && parameters[0] is string)
            {

                //var serverHashCode = parameters[0] as string;

                if (parameters.Length > 1)
                {
                    var newParameters = new object[parameters.Length - 1];
                    for (int i = 1; i < parameters.Length; i++)
                    {
                        newParameters[i - 1] = parameters[i];
                    }

                    return newParameters;
                }
                return null;
            }

            throw new Exception();
        }

        [AuthenticateNeeded(false)]
        public object AcknowledgeAllowed(string serverHashCode)
        {
#if DEBUG
            CcuUpgradeHandler.Log.Info("AcknowledgeAllowed");
#endif
            return true;
        }

        [AuthenticateNeeded(false)]
        public bool IsCCUUpgrader(string serverHashCode)
        {
#if DEBUG
            CcuUpgradeHandler.Log.Info("IsCCUUpgrader");
#endif
            return true;
        }

        [AuthenticateNeeded(false)]
        public bool StopUpgrade(string serverHashCode)
        {
#if DEBUG
            CcuUpgradeHandler.Log.Info("StopUpgrade");
#endif
            return CcuUpgradeHandler.Singleton.StopUpgrade();
        }

        [AuthenticateNeeded(false)]
        public bool ConfirmUpgradeFinish(string serverHashCode)
        {
            if (!serverHashCode.Equals(CcuUpgradeHandler.Singleton.ServerHashCode))
                return false;
#if DEBUG
            CcuUpgradeHandler.Log.Info("ConfirmUpgradeFinish");
#endif
            CcuUpgradeHandler.Singleton.UpgradeFinishConfirmed();
            return true;
        }

        [AuthenticateNeeded(false)]
        public bool BindCompleted(string serverHashCode)
        {
            if (!serverHashCode.Equals(CcuUpgradeHandler.Singleton.ServerHashCode))
                return false;
#if DEBUG
            CcuUpgradeHandler.Log.Info("BindCompleted");
#endif
            CcuUpgradeHandler.Singleton.BindCompleted();
            return true;
        }

    }
}
