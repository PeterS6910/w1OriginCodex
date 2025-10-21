using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.AlarmArea;
using Contal.Cgp.NCAS.CCU.Alarms;
using Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes;
using Contal.Cgp.NCAS.CCU.EventParameters;
using Contal.Cgp.NCAS.Definitions;
using Contal.Cgp.NCAS.Globals;
using Contal.Drivers.LPC3250;
using Contal.IwQuick;
using Contal.IwQuick.Data;
using Contal.IwQuick.Net;
using Contal.IwQuick.Sys;
using Contal.IwQuick.Sys.Microsoft;
using Contal.IwQuick.Threads;
using Contal.LwRemoting2;

namespace Contal.Cgp.NCAS.CCU
{
    public class CcuCoreRemotingProvider : ALwRemotingService
    {
        public const string SIGNATURE_FILE = @"\Storage Card\nova.sig";

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

        private CcuCoreRemotingProvider()
        {
        }

        public static object[] AuthenticateClient(ISimpleTcpConnection connection, object[] parameters)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("object[] CcuCoreRemotingProvider.AuthenticateClient(ISimpleTcpConnection connection, object[] parameters): [{0}]",
                Log.GetStringFromParameters(connection, parameters)));
            if (parameters.Length > 0 && parameters[0] is string)
            {
                string serverHashCode = parameters[0] as string;

                if ((CCUConfigurationState.Singleton.IsConfigured && CCUConfigurationState.Singleton.ServerHashCode == serverHashCode))
                {
                    if (parameters.Length > 1)
                    {
                        object[] newParameters = new object[parameters.Length - 1];
                        for (int i = 1; i < parameters.Length; i++)
                        {
                            newParameters[i - 1] = parameters[i];
                        }

                        CcuCore.Singleton.ClientAuthenticated(connection);
                        CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("object[] CcuCoreRemotingProvider.AuthenticateClient return {0}", Log.GetStringFromParameters(newParameters)));
                        return newParameters;
                    }
                    CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, "object[] CcuCoreRemotingProvider.AuthenticateClient return null");
                    return null;
                }
            }

            throw new Exception();
        }

        [AuthenticateNeeded(false)]
        public byte GetIsConfigured(string serverHashCode)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "byte CcuCoreRemotingProvider.GetIsConfigured(string serverHashCode) :[" + Log.GetStringFromParameter(serverHashCode) + "]");
            byte result;

            if (CCUConfigurationState.Singleton.IsUpgrading)
            {
                result = (byte)DB.CCUConfigurationState.Upgrading;
            }
            else if (!CCUConfigurationState.Singleton.IsConfigured)
            {
                result = (byte)DB.CCUConfigurationState.Unconfigured;
            }
            else if (CCUConfigurationState.Singleton.ServerHashCode == serverHashCode)
            {
                result = (byte)DB.CCUConfigurationState.ConfiguredForThisServer;
            }
            else
            {
                result = (byte)DB.CCUConfigurationState.ConfiguredForAnotherServer;
            }

            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "byte CcuCoreRemotingProvider.GetIsConfigured return " + result);
            return result;
        }

        [AuthenticateNeeded(false)]
        public bool IsCat12Combo(string serverHashCode)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool CcuCoreRemotingProvider.IsCat12Combo()");

            var result = CcuCore.Singleton.IsCat12Combo;

            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool CcuCoreRemotingProvider.IsCat12Combo() return " + result);

            return result;
        }

        [AuthenticateNeeded(false)]
        public bool HasCat12ComboLicence(string serverHashCode)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool CcuCoreRemotingProvider.HasCat12ComboLicence()");

            var result = CcuCore.HasCat12ComboLicence();

            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool CcuCoreRemotingProvider.HasCat12ComboLicence() return " + result);

            return result;
        }

        [AuthenticateNeeded(true)]
        public void SetCat12ComboLicence(bool hasComboLicence)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "void CcuCoreRemotingProvider.SetCa12ComboCredit(bool hasComboLicence): [" + Log.GetStringFromParameter(hasComboLicence) + "]");
            
            if (hasComboLicence)
                CcuCore.Singleton.TryStopUnconfigureTimer();

            CcuCore.SetCat12ComboLicence(hasComboLicence);
        }

        [AuthenticateNeeded(true)]
        public bool UnsetUpgradeMode()
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool CcuCoreRemotingProvider.UnsetUpgradeMode()");
            bool stopped = CcuCore.Singleton.StopUpgradeMode();
            if (stopped)
                CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool CcuCoreRemotingProvider.UnsetUpgradeMode return true");
            else
                CcuCore.DebugLog.Warning(Log.LOW_LEVEL, "bool CcuCoreRemotingProvider.UnsetUpgradeMode return false");

            return stopped;
        }

        [AuthenticateNeeded(true)]
        public bool Unconfigure()
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool CcuCoreRemotingProvider.Unconfigure()");
            CcuCore.Singleton.Unconfigure();
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool CcuCoreRemotingProvider.Unconfigure return true");

            return true;
        }

        [AuthenticateNeeded(false)]
        public bool ConfigureForAnotherClient(string serverHashCode)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool CcuCoreRemotingProvider.ConfigureForAnotherClient(string serverHashCode): [" + Log.GetStringFromParameter(serverHashCode) + "]");
            if (!CcuCore.Singleton.IsAuthenticatedClientConnected())
            {
                Unconfigure();
                CCUConfigurationState.Singleton.SetServer(serverHashCode);
                CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool CcuCoreRemotingProvider.ConfigureForAnotherClient return true");
                return true;
            }

            CcuCore.DebugLog.Warning(Log.LOW_LEVEL, "bool CcuCoreRemotingProvider.ConfigureForAnotherClient return false");
            return false;
        }

        [AuthenticateNeeded(false)]
        public bool IsCcuConfiguredServerOnline(string serverHashCode)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool CcuCoreRemotingProvider.IsCcuConfiguredServerOnline(string serverHashCode): [" + Log.GetStringFromParameter(serverHashCode) + "]");

            bool result = CcuCore.Singleton.IsAuthenticatedClientConnected();
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("bool CcuCoreRemotingProvider.IsCcuConfiguredServerOnline return {0}", result));

            return result;
        }

        [AuthenticateNeeded(true)]
        public bool Reset()
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool CcuCoreRemotingProvider.Reset()");
            TimerManager.Static.StartTimeout(2000, RebootTick);

            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool CcuCoreRemotingProvider.Reset return true");
            return true;
        }

        [AuthenticateNeeded(true)]
        public bool SoftReset()
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool CcuCoreRemotingProvider.SoftReset()");
            TimerManager.Static.StartTimeout(2000, SoftRebootTick);

            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool CcuCoreRemotingProvider.SoftReset return true");
            return true;
        }

        private static bool SoftRebootTick(TimerCarrier timerCarrier)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("bool CcuCoreRemotingProvider.SoftRebootTick(TimerCarrier timerCarrier): [{0}]", Log.GetStringFromParameters(timerCarrier)));
            CcuCore.Singleton.SoftRebootCCU();
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool CcuCoreRemotingProvider.SoftRebootTick return true");
            return true;
        }

        [AuthenticateNeeded(true)]
        public int[] CommunicationStatistic()
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "int[] CcuCoreRemotingProvider.CommunicationStatistic()");
            int[] result = CcuCore.Singleton.CommunicationStatistic();

            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "int[] CcuCoreRemotingProvider.CommunicationStatistic return [" + Log.GetStringFromParameter(result) + "]");
            return result;
        }

        [AuthenticateNeeded(true)]
        public void ResetSended()
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "void CcuCoreRemotingProvider.ResetSended()");
            CcuCore.Singleton.ResetSended();
        }

        [AuthenticateNeeded(true)]
        public void ResetDeserializationError()
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "void CcuCoreRemotingProvider.ResetDeserializationError()");
            CcuCore.Singleton.ResetDeserializationError();
        }

        [AuthenticateNeeded(true)]
        public void ResetReceived()
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "void CcuCoreRemotingProvider.ResetReceived()");
            CcuCore.Singleton.ResetReceived();
        }

        [AuthenticateNeeded(true)]
        public void ResetReceivedError()
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "void CcuCoreRemotingProvider.ResetReceivedError()");
            CcuCore.Singleton.ResetReceivedError();
        }

        [AuthenticateNeeded(true)]
        public void ResetCommunicationStatistic()
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "void CcuCoreRemotingProvider.ResetCommunicationStatistic()");
            CcuCore.Singleton.ResetAll();
        }

        [AuthenticateNeeded(true)]
        public void ResetMsgRetry()
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "void CcuCoreRemotingProvider.ResetMsgRetry()");
            CcuCore.Singleton.ResetMsgRetry();
        }

        /// <summary>
        /// Resets CCU command timeouts counter
        /// </summary>
        [AuthenticateNeeded(true)]
        public void ResetCCUCommandTimeouts()
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "void CcuCoreRemotingProvider.ResetCCUCommandTimeouts()");
            CcuCore.Singleton.ResetCCUCommandTimeouts();
        }

        [AuthenticateNeeded(true)]
        public object[] GetCcuStartsCount()
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "object[] CcuCoreRemotingProvider.GetCcuStartsCount()");
            object[] result = CcuCore.Singleton.GetCcuStartsCount();
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("object[] CcuCoreRemotingProvider.GetCcuStartsCount return {0}", Log.GetStringFromParameters(result)));
            return result;
        }

        [AuthenticateNeeded(true)]
        public void ResetCcuStartCounter()
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "void CcuCoreRemotingProvider.ResetCcuStartCounter()");
            CcuCore.Singleton.ResetCcuStartCounter();
        }

        [AuthenticateNeeded(true)]
        public string WinCEImageVersion()
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "string CcuCoreRemotingProvider.WinCEImageVersion()");
            string result = CcuCore.Singleton.GetWinCEImageVersion();
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("string CcuCoreRemotingProvider.WinCEImageVersion return {0}", Log.GetStringFromParameters(result)));
            return result;
        }

        [AuthenticateNeeded(true)]
        public string[] CoprocessorBuildNumberStatistics()
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "string[] CcuCoreRemotingProvider.CoprocessorBuildNumberStatistics()");
            string[] result = CcuCore.Singleton.CoprocessorBuildNumber();
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "string[] CcuCoreRemotingProvider.CoprocessorBuildNumberStatistics return "+ Log.GetStringFromParameterList(result));
            return result;
        }

        [AuthenticateNeeded(true)]
        public string ResultSimulationCardSwiped(
            ObjectType objectType,
            Guid idObject,
            string cardNumber,
            string pin,
            int pinLength)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL, () =>
                    string.Format(
                        "string CcuCoreRemotingProvider.ResultSimulationCardSwiped(ObjectType objectType, Guid idObject, string cardNumber, string pin, int pinLength)",
                        Log.GetStringFromParameters(
                            objectType,
                            idObject,
                            cardNumber,
                            pin,
                            pinLength)));

            string result = CcuCore.Singleton.SimulationCardSwiped(
                objectType,
                idObject,
                cardNumber,
                pin,
                pinLength);

            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL, () =>
                    string.Format("string CcuCoreRemotingProvider.ResultSimulationCardSwiped return {0}",
                        Log.GetStringFromParameters(result)));

            return result;
        }


        [AuthenticateNeeded(true)]
        public string ResultCheckAlarmAreaRights(Guid giudPerson, Guid guidAlarmArea)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("string CcuCoreRemotingProvider.ResultCheckAlarmAreaRights(Guid giudPerson, Guid guidAlarmArea)", Log.GetStringFromParameters(giudPerson, guidAlarmArea)));
            string result = AlarmAreaAccessRightsManager.Singleton.CheckAlarmAreaActivationRightsTest(giudPerson, guidAlarmArea);
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("string CcuCoreRemotingProvider.ResultCheckAlarmAreaRights return {0}", Log.GetStringFromParameters(result)));
            return result;
        }

        private static bool RebootTick(TimerCarrier timerCarrier)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("bool CcuCoreRemotingProvider.RebootTick(TimerCarrier timerCarrier)", Log.GetStringFromParameters(timerCarrier)));
            WatchdogControl.Reboot();
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool CcuCoreRemotingProvider.RebootTick return true");
            return true;
        }

        [AuthenticateNeeded(true)]
        public bool ExistsPathOrFile(string fullName)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool CcuCoreRemotingProvider.ExistsPathOrFile(string fullName): [" + Log.GetStringFromParameter(fullName) + "]");
            if (string.IsNullOrEmpty(fullName))
            {
                CcuCore.DebugLog.Warning(Log.LOW_LEVEL, "bool CcuCoreRemotingProvider.ExistsPathOrFile return false[1]");
                return false;
            }
            bool result = (File.Exists(fullName) || Directory.Exists(fullName));
            if (result)
                CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool CcuCoreRemotingProvider.ExistsPathOrFile return true");
            else
                CcuCore.DebugLog.Warning(Log.LOW_LEVEL, "bool CcuCoreRemotingProvider.ExistsPathOrFile return false[2]");

            return result;
        }

        [AuthenticateNeeded(true)]
        public bool ForceReconfiguration()
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool CcuCoreRemotingProvider.ForceReconfiguration()");
            CcuCore.Singleton.DeleteAllFromDatabase();
            AlarmAreas.Singleton.ClearAAToCRsReporting();
            AlarmAreas.Singleton.DeleteAllAlarmAreasAndTimeBuyingStates();

            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool CcuCoreRemotingProvider.ForceReconfiguration return true");
            return true;
        }

        public string DummyMethod(string input)
        {
            return "Dummy method called with input \"" + (input ?? "NULL") + "\"";
        }

        [AuthenticateNeeded(true)]
        public bool SendAllStatesToServer()
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool CcuCoreRemotingProvider.SendAllStatesToServer()");
            CcuCore.Singleton.SendAllStatesToServer();

            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool CcuCoreRemotingProvider.SendAllStatesToServer retun true");
            return true;
        }

        /// <summary>
        /// Save table object to database
        /// </summary>
        /// <param name="obj">the table object</param>
        /// <returns>true on success</returns>
        [AuthenticateNeeded(true)]
        public bool SaveObject(object obj)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool CcuCoreRemotingProvider.SaveObject(object obj): [" + Log.GetStringFromParameter(obj) + "]");

            var objectsToSend = obj as ObjectsToSend;

            if (null == objectsToSend)
            {
                CcuCore.DebugLog.Warning(Log.LOW_LEVEL, "bool CcuCoreRemotingProvider.SaveObject return false[1]");
                return false;
            }

            string objCaption = objectsToSend.ObjectType.ToString();
            CcuCore.DebugLog.Info(objCaption + " save to database");

            Stopwatch sw = Stopwatch.StartNew();

            bool retValue = Database.SaveToDatabase(objectsToSend);

            long period = sw.ElapsedMilliseconds;

            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "Saving object " + objCaption + " took " + period + "ms");

            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("bool CcuCoreRemotingProvider.SaveObject return {0}[2]", Log.GetStringFromParameters(retValue)));
            return retValue;
        }

        [AuthenticateNeeded(true)]
        public MaximumVersionAndIds ReadMaximumVersionAndIds(byte objectType)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => "int CcuCoreRemotingProvider.ReadMaximumVersionAndIds(byte objectType): [" + objectType + "]");

            var maximumVersionAndIds = Database.ConfigObjectsEngine.GetMaximumVersionAndIds((ObjectType)objectType);
            
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => "int CcuCoreRemotingProvider.ReadMaximumVersionAndIds return " + maximumVersionAndIds);

            return maximumVersionAndIds;
        }

        [AuthenticateNeeded(true)]
        public bool SaveMaxObjectTypeVersion(ObjectType objectType, int version)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => "bool CcuCoreRemotingProvider.SaveMaxObjectTypeVersion(ObjectType objectType, int version): [" +
                    Log.GetStringFromParameters(objectType, version) + "]");

            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => "SaveMaxObjectTypeVersion for objectType " + objectType + " " + version);

            Database.ConfigObjectsEngine.SaveMaxObjectTypeVersion(
                objectType, 
                version);

            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => "bool CcuCoreRemotingProvider.SaveMaxObjectTypeVersion return true");

            return true;
        }

        [AuthenticateNeeded(true)]
        public bool DeleteObject(byte objectType, ICollection<Guid> objectGuids)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool CcuCoreRemotingProvider.DeleteObject(byte objectType, Guid[] objectGuids): [" +
                Log.GetStringFromParameters(objectType, objectGuids) + "]");

            try
            {
                
                CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "Delete from database " + (ObjectType)objectType);
                Database.ConfigObjectsEngine.DeleteFromDatabase(
                    (ObjectType)objectType, 
                    objectGuids);
                CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool CcuCoreRemotingProvider.DeleteObject return true");
                return true;
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
                CcuCore.DebugLog.Warning(Log.LOW_LEVEL, "bool CcuCoreRemotingProvider.DeleteObject return false");
                return false;
            }
        }

        [AuthenticateNeeded(true)]
        public bool ApplyChanges()
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool CcuCoreRemotingProvider.ApplyChanges()");
            CcuCore.Singleton.ApplyChanges();
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool CcuCoreRemotingProvider.ApplyChanges return true");

            try
            {
                DCUs.Singleton.Start();
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }

            return true;
        }

        [AuthenticateNeeded(true)]
        public bool DeleteEvents(UInt64[] eventIds)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool CcuCoreRemotingProvider.DeleteEvents(int[] eventIds): [" + Log.GetStringFromParameter(eventIds) + "[");
            SendEventsToServerDispatcher.Singleton.DeleteEvents(eventIds);
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool CcuCoreRemotingProvider.DeleteEvents return true");
            return true;
        }

        /// <summary>
        /// Event SaveEventLog to invoke EventLog on CGP Server
        /// </summary>
        public event DObjects2Void SaveEvent;

        /// <summary>
        /// Call event SaveEventLog
        /// </summary>
        /// <param name="eventsParameters">params as atribure called method</param>
        public bool DoSaveEvent(ICollection<EventParameters.EventParameters> eventsParameters)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () =>
                    string.Format(
                        "void CcuCoreRemotingProvider.DoSaveEvent(ICollection<EventParameters.EventParameters> eventParameters): [{0}]",
                        Log.GetStringFromParameter(eventsParameters)));

            if (SaveEvent == null)
            {
                CcuCore.DebugLog.Info(
                    Log.NORMAL_LEVEL,
                    () =>
                        "No events for DoSaveEvent");

                return false;
            }

            try
            {
                CcuCore.DebugLog.Info(
                    Log.NORMAL_LEVEL,
                    () =>
                        "DoSaveEvent reported to event");

                SaveEvent(eventsParameters);

                return true;
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
                
                CcuCore.DebugLog.Error(
                    Log.NORMAL_LEVEL,
                    () =>
                        "DoSaveEvent failed");
            }

            return false;
        }

        public event DObjects2Void SaveAlarmEvent;

        public bool DoSaveAlarmEvent(ICollection<AlarmEvent> alarmEvents)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () =>
                    string.Format(
                        "void CcuCoreRemotingProvider.DoSaveAlarmEvent(ICollection<AlarmEvent> alarmEvents): [{0}]",
                        Log.GetStringFromParameter(alarmEvents)));

            if (SaveAlarmEvent == null)
            {
                CcuCore.DebugLog.Info(
                    Log.NORMAL_LEVEL,
                    () =>
                        "No events for DoSaveAlarmEvent");

                return false;
            }

            try
            {
                CcuCore.DebugLog.Info(
                    Log.NORMAL_LEVEL,
                    () =>
                        "DoSaveAlarmEvent reported to event");

                SaveAlarmEvent(alarmEvents);

                return true;
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);

                CcuCore.DebugLog.Error(
                    Log.NORMAL_LEVEL,
                    () =>
                        "DoSaveAlarmEvent failed");
            }

            return false;
        }

        [AuthenticateNeeded(true)]
        public ICollection<Alarm> GetAlarms(
            ICollection<AlarmIndividualBlockingChangeInPending> individualBlockingChangesInPending,
            ICollection<AlarmAcknowledgeInPending> alarmsAcknowledgeInPending)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL,
                () =>
                    "bool CcuCoreRemotingProvider.GetAlarms()");

            return AlarmsManager.Singleton.GetAlarms(
                individualBlockingChangesInPending,
                alarmsAcknowledgeInPending);
        }

        [AuthenticateNeeded(true)]
        public bool ProcessAlarmEvent(AlarmEvent alarmEvent)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL,
                () =>
                    string.Format(
                        "bool CcuCoreRemotingProvider.ProcessAlarmEvent(AlarmEvent alarmEvent): [{0}]",
                        Log.GetStringFromParameter(alarmEvent)));

            return AlarmsManager.Singleton.ProcessAlarmEventsFromServer(
                alarmEvent);
        }

        [AuthenticateNeeded(true)]
        public int[] GetAccessibleOutputsInputs()
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "int[] CcuCoreRemotingProvider.GetAccessibleOutputsInputs()");

            int[] result = null;
            var accessAbleInputs = SharedResources.Singleton.GetAccessibleInputs();
            if (accessAbleInputs != null)
            {
                result = accessAbleInputs.Select(srInputInfo => srInputInfo.InputIndex).ToArray();
            }

            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "int[] CcuCoreRemotingProvider.GetAccessibleOutputsInputs return " + result);
            return result;
        }

        [AuthenticateNeeded(false)]
        public object GetMacAddress(string serverHashCode)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "object CcuCoreRemotingProvider.GetMacAddress(string serverHashCode): [" + Log.GetStringFromParameter(serverHashCode) + "]");
            object result = CcuCore.Singleton.CeNetworkManagement.GetMAC();
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "object CcuCoreRemotingProvider.GetMacAddress return " + result);

            return result;
        }

        [AuthenticateNeeded(false)]
        public object GetMainBoardType(string serverHashCode)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "object CcuCoreRemotingProvider.GetMainBoardType(string serverHashCode): [" + Log.GetStringFromParameter(serverHashCode) + "]");
            byte mainBoardType = (byte)MainBoard.Variant;
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "object CcuCoreRemotingProvider.GetMainBoardType return " + mainBoardType);
            return mainBoardType;
        }

        [AuthenticateNeeded(false)]
        public object GetCcuFirmwareVersion(string serverHashCode)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "object CcuCoreRemotingProvider.GetCcuFirmwareVersion(string serverHashCode): [" + Log.GetStringFromParameter(serverHashCode) + "]");
            ExtendedVersion fwVersion = CcuCore.Singleton.Version;
            if (fwVersion != null)
            {
                CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "object CcuCoreRemotingProvider.GetCcuFirmwareVersion return " + fwVersion);
                return fwVersion.ToString();
            }
            CcuCore.DebugLog.Warning(Log.BELOW_NORMAL_LEVEL, () => string.Format("object CcuCoreRemotingProvider.GetCcuFirmwareVersion return {0}", Log.GetStringFromParameters(fwVersion)));
            return string.Empty;
        }

        [AuthenticateNeeded(true)]
        public byte GetInputState(Guid inputGuid)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "byte CcuCoreRemotingProvider.GetInputState(Guid inputGuid): [" + Log.GetStringFromParameter(inputGuid) + "]");
            byte result = (byte)Inputs.Singleton.GetInputLogicalState(inputGuid);
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "byte CcuCoreRemotingProvider.GetInputState return " + result);

            return result;
        }

        [AuthenticateNeeded(true)]
        public int[] GetAccessibleOutputsOutputs()
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "int[] CcuCoreRemotingProvider.GetAccessibleOutputs()");

            int[] result = null;
            var accessAbleOutputs = SharedResources.Singleton.GetAccessibleOutputs();
            if (accessAbleOutputs != null)
            {
                result = accessAbleOutputs.Select(srOutputInfo => srOutputInfo.OutputIndex).ToArray();
            }

            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "int[] CcuCoreRemotingProvider.GetAccessibleOutputs return " + result);

            return result;
        }

        [AuthenticateNeeded(true)]
        public int GetDoorEnvironmentsCount()
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "int CcuCoreRemotingProvider.GetDoorEnvironmentsCount()");
            int result = DoorEnvironments.Singleton.GetDoorEnvironmentsCount();
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "int CcuCoreRemotingProvider.GetDoorEnvironmentsCount return " + result);

            return result;
        }

        [AuthenticateNeeded(true)]
        public bool DoorEnvironmentAccessGranted(Guid doorEnvironmentGuid)
        {

            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool CcuCoreRemotingProvider.DoorEnvironmentAccessGranted(Guid doorEnvironmentGuid): [" + Log.GetStringFromParameter(doorEnvironmentGuid) + "]");
            bool retValue = DoorEnvironments.Singleton.AccessGrantedFromClient(doorEnvironmentGuid);
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("bool CcuCoreRemotingProvider.DoorEnvironmentAccessGranted return {0}", retValue));
            return retValue;
        }

        [AuthenticateNeeded(true)]
        public byte GetAlarmAreaActualState(Guid alarmAreaGuid)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "byte CcuCoreRemotingProvider.GetAlarmAreaActualState(Guid alarmAreaGuid): [" + Log.GetStringFromParameter(alarmAreaGuid) + "]");
            byte result = AlarmAreas.Singleton.GetAlarmAreaAlarmState(alarmAreaGuid);
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "byte CcuCoreRemotingProvider.GetAlarmAreaActualState return " + result);

            return result;
        }

        [AuthenticateNeeded(true)]
        public byte GetAlarmAreaActivationState(Guid alarmAreaGuid)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "byte CcuCoreRemotingProvider.GetAlarmAreaActivationState(Guid alarmAreaGuid): [" + Log.GetStringFromParameter(alarmAreaGuid) + "]");
            byte result = AlarmAreas.Singleton.GetAlarmAreaActivationState(alarmAreaGuid);
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "byte CcuCoreRemotingProvider.GetAlarmAreaActivationState return " + result);

            return result;
        }

        [AuthenticateNeeded(true)]
        public bool SetAlarmArea(
            Guid alarmAreaGuid,
            Guid guidLogin,
            bool noPrewarning)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () =>
                    "bool CcuCoreRemotingProvider.SetAlarmArea(Guid alarmAreaGuid, Guid guidLogin, bool noPrewarning): [" +
                    Log.GetStringFromParameters(
                        alarmAreaGuid,
                        guidLogin,
                        noPrewarning) + "]");

            var dateTime = UniqueDateTime.GetDateTime;

            bool result = AlarmAreas.Singleton.SetAlarmArea(
                alarmAreaGuid,
                false,
                new AlarmAreas.SetUnsetParams(
                    Guid.Empty,
                    new AccessDataBase(),
                    Guid.Empty,
                    false,
                    noPrewarning));

            if (result)
            {
                Events.ProcessEvent(
                    new EventSetAlarmAreaFromClient(
                        alarmAreaGuid,
                        guidLogin,
                        noPrewarning));

                CcuCore.DebugLog.Info(
                    Log.NORMAL_LEVEL,
                    () =>
                        "bool CcuCoreRemotingProvider.SetAlarmArea return true");

                return true;
            }

            CcuCore.DebugLog.Warning(
                Log.LOW_LEVEL,
                () =>
                    "bool CcuCoreRemotingProvider.SetAlarmArea return false");

            return false;
        }

        [AuthenticateNeeded(true)]
        public bool UnconditionalSetAlarmArea(
            Guid alarmAreaGuid,
            Guid guidLogin,
            bool noPrewarning)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () =>
                    "bool CcuCoreRemotingProvider.UnconditionalSetAlarmArea(Guid alarAreaGuid, Guid guidLogin, bool noPrewarning): [" +
                    Log.GetStringFromParameters(
                        alarmAreaGuid,
                        guidLogin,
                        noPrewarning) + "]");

            Events.ProcessEvent(
                new EventSetAlarmAreaFromClient(
                    alarmAreaGuid,
                    guidLogin,
                    noPrewarning));

            AlarmAreas.Singleton.SetAlarmArea(
                alarmAreaGuid,
                false,
                new AlarmAreas.SetUnsetParams(
                    Guid.Empty,
                    new AccessDataBase(),
                    Guid.Empty,
                    true,
                    noPrewarning));

            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL,
                () =>
                    "bool CcuCoreRemotingProvider.UnconditionalSetAlarmArea return true");
            
            return true;
        }

        private class UnsetAlarmAreaResult : AlarmAreaStateAndSettings.IAlarmAreaUnsetResult
        {
            private readonly Guid _alarmAreaGuid;
            private readonly Guid _guidLogin;

            public UnsetAlarmAreaResult(Guid alarmAreaGuid, Guid guidLogin)
            {
                _alarmAreaGuid = alarmAreaGuid;
                _guidLogin = guidLogin;
            }

            public void OnFailed(AlarmAreaActionResult alarmAreaActionResult, int timeToBuy, int remainingTime)
            {
            }

            public void OnSucceded(int timeToBuy,
                int remainingTime,
                bool nonAcknowledgedAlarmDuringSetPeriod)
            {
                Events.ProcessEvent(
                    new EventUnsetAlarmAreaFromClient(
                        _alarmAreaGuid,
                        _guidLogin,
                        timeToBuy));
            }
        }

        [AuthenticateNeeded(true)]
        public bool UnsetAlarmArea(
            Guid alarmAreaGuid,
            Guid guidLogin,
            Guid guidPerson,
            int timeToBuy)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () =>
                    "bool CcuCoreRemotingProvider.UnsetAlarmArea(Guid alarAreaGuid, Guid guidLogin, Guid guidPerson, int timeToBuy): [" +
                    Log.GetStringFromParameters(
                        alarmAreaGuid,
                        guidLogin,
                        guidPerson,
                        timeToBuy) + "]");

            var alarmAreaUnsetResult = new UnsetAlarmAreaResult(
                alarmAreaGuid,
                guidLogin);

            AlarmAreas.Singleton.UnsetAlarmArea(
                alarmAreaUnsetResult,
                true,
                alarmAreaGuid,
                guidLogin,
                guidPerson,
                timeToBuy,
                new AlarmAreas.SetUnsetParams(
                    Guid.Empty,
                    new AccessDataBase(), 
                    Guid.Empty,
                    false,
                    false));

            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () =>
                    "bool CcuCoreRemotingProvider.UnsetAlarmArea return true");

            return true;
        }

        [AuthenticateNeeded(true)]
        public bool DeleteAllEvents()
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool CcuCoreRemotingProvider.DeleteAllEvents()");
            SendEventsToServerDispatcher.Singleton.DeleteEvents();
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool CcuCoreRemotingProvider.DeleteAllEvents return true");
            return true;
        }

        [AuthenticateNeeded(true)]
        public bool FileExists(string fullFileName)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool CcuCoreRemotingProvider.FileExists(string fullFileName): [" + Log.GetStringFromParameter(fullFileName) + "]");
            bool result = File.Exists(fullFileName);
            if (result)
                CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool CcuCoreRemotingProvider.FileExists return true");
            else
                CcuCore.DebugLog.Warning(Log.LOW_LEVEL, "bool CcuCoreRemotingProvider.FileExists return false");

            return result;
        }

        [AuthenticateNeeded(true)]
        public object[] GetAllBlockFilesInfo()
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "object[] CcuCoreRemotingProvider.GetAllBlockFilesInfo()");
            BlockFileInfo[] info = CcuCore.Singleton.GetBlockFilesInfo();
            List<object> result = new List<object>();
            foreach (BlockFileInfo item in info)
            {
                result.Add(item);
            }

            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "object[] CcuCoreRemotingProvider.GetAllBlockFilesInfo return " + Log.GetStringFromParameter(result));
            return result.ToArray();
        }

        [AuthenticateNeeded(true)]
        public bool ResetBlockFilesInfo()
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool CcuCoreRemotingProvider.ResetBlockFilesInfo()");
            bool result = CcuCore.Singleton.ResetBlockFilesInfo();
            if (result)
                CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool CcuCoreRemotingProvider.ResetBlockFilesInfo return true");
            else
                CcuCore.DebugLog.Warning(Log.LOW_LEVEL, "bool CcuCoreRemotingProvider.ResetBlockFilesInfo return false");

            return result;
        }

        [AuthenticateNeeded(true)]
        public string ContainsDCUUpgradePackageVersion(string searchInDirectory, string dcuUpgradeVersion)
        {

            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "string CcuCoreRemotingProvider.ContainsDCUUpgradePackageVersion(string searchInDirectory, string dcuUpgradeVersion): [" +
                Log.GetStringFromParameters(searchInDirectory, dcuUpgradeVersion) + "]");
            if (string.IsNullOrEmpty(dcuUpgradeVersion))
            {
                CcuCore.DebugLog.Warning(Log.BELOW_NORMAL_LEVEL, () => string.Format("string CcuCoreRemotingProvider.ContainsDCUUpgradePackageVersion return {0}[1]", Log.GetStringFromParameters(string.Empty)));
                return string.Empty;
            }
            if (!Directory.Exists(searchInDirectory))
            {
                CcuCore.DebugLog.Warning(Log.BELOW_NORMAL_LEVEL, () => string.Format("string CcuCoreRemotingProvider.ContainsDCUUpgradePackageVersion return {0}[2]", Log.GetStringFromParameters(string.Empty)));
                return string.Empty;
            }

            foreach (string fullFileName in Directory.GetFiles(searchInDirectory))
            {
                Exception ex;
                string[] headerParameters = FilePacker.TryGetHeaderParameters(fullFileName, out ex);
                if (headerParameters == null || headerParameters.Length < 5 || headerParameters[0].ToLower() != DeviceType.DCU.ToString().ToLower() || string.Format("({0}){1}", headerParameters[1], headerParameters[2]) != dcuUpgradeVersion)
                    continue;

// ReSharper disable once AccessToForEachVariableInClosure
                CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "string CcuCoreRemotingProvider.ContainsDCUUpgradePackageVersion return " + Log.GetStringFromParameter(fullFileName));
                return fullFileName;
            }
            return string.Empty;
        }

        [AuthenticateNeeded(true)]
        public string[] GetAvailableDCUUpgrades(string fullDirectoryName)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "string[] CcuCoreRemotingProvider.GetAvailableDCUUpgrades(string fullDirectoryName): [" + Log.GetStringFromParameter(fullDirectoryName) + "]");
            if (fullDirectoryName == null || !Directory.Exists(fullDirectoryName))
            {
                CcuCore.DebugLog.Warning(Log.BELOW_NORMAL_LEVEL, () => string.Format("string[] CcuCoreRemotingProvider.GetAvailableDCUUpgrades return {0}[1]", Log.GetStringFromParameters(null)));
                return null;
            }
            List<string> result = new List<string>();

            foreach (string fileName in Directory.GetFiles(fullDirectoryName))
            {
                Exception ex;
                string[] headerParameters = FilePacker.TryGetHeaderParameters(fileName, out ex);
                if (headerParameters == null || headerParameters.Length < 4 || headerParameters[0].ToLower() != DeviceType.DCU.ToString().ToLower())
                    continue;

                result.Add(headerParameters[1]);
            }

            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "string[] CcuCoreRemotingProvider.GetAvailableDCUUpgrades return " + Log.GetStringFromParameter(result));
            return result.ToArray();
        }

        [AuthenticateNeeded(true)]
        public bool EnsureDirectory(string fullDirectoryName)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool CcuCoreRemotingProvider.EnsureDirectory(string fullDirectoryName): [" + Log.GetStringFromParameter(fullDirectoryName) + "]");
            if (!Directory.Exists(fullDirectoryName))
            {
                try
                {
                    Directory.CreateDirectory(fullDirectoryName);
                }
                catch (Exception error)
                {
                    HandledExceptionAdapter.Examine(error);
                    CcuCore.DebugLog.Warning(Log.LOW_LEVEL, "bool CcuCoreRemotingProvider.EnsureDirectory return false");
                    return false;
                }
            }
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool CcuCoreRemotingProvider.EnsureDirectory return true");
            return true;
        }

        [AuthenticateNeeded(true)]
        public bool SetDefaultDCUUpgradeVersion(string upgradeVersion)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool CcuCoreRemotingProvider.SetDefaultDCUUpgradeVersion(string upgradeVersion): [" + Log.GetStringFromParameter(upgradeVersion) + "]");
            bool result = DCUs.Singleton.UpdateUpgradeInfoFile(upgradeVersion);
            if (result)
                CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool CcuCoreRemotingProvider.SetDefaultDCUUpgradeVersion return true");
            else
                CcuCore.DebugLog.Warning(Log.LOW_LEVEL, "bool CcuCoreRemotingProvider.SetDefaultDCUUpgradeVersion return false");

            return result;
        }

        [AuthenticateNeeded(true)]
        public bool UpgradeDCUs(string packageFullName)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool CcuCoreRemotingProvider.UpgradeDCUs(string packageFullName): [" + Log.GetStringFromParameter(packageFullName) + "]");
            UnpackPackageFailedCode errorCode;
            byte[] nodesWaiting;
            if (!DCUs.Singleton.ProcessUpgradePackage(packageFullName, null, out errorCode, out nodesWaiting))
            {
                Events.ProcessEvent(
                    new EventProcessCrUpgradePackageFailed(
                        nodesWaiting,
                        (byte) errorCode));
            }
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool CcuCoreRemotingProvider.UpgradeDCUs return true");
            return true;
        }

        /// <summary>
        /// Registers DCUs for upgrade with specific version
        /// </summary>
        /// <param name="nodeLogicalAddresses"></param>
        /// <param name="upgradeVersion"></param>
        /// <returns>Registered DCUs logical addresses</returns>
        [AuthenticateNeeded(true)]
        public byte[] RegisterDCUsForUpgradeVersion(byte[] nodeLogicalAddresses, string upgradeVersion)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "byte[] CcuCoreRemotingProvider.RegisterDCUsForUpgradeVersion(byte[] nodeLogicalAddresses, string upgradeVersion): [" +
                Log.GetStringFromParameters(nodeLogicalAddresses, upgradeVersion) + "]");
            byte[] result = DCUs.Singleton.RegisterDCUsForUpgradeVersion(nodeLogicalAddresses, upgradeVersion);
            if (result == null)
                CcuCore.DebugLog.Warning(Log.LOW_LEVEL, "byte[] CcuCoreRemotingProvider.RegisterDCUsForUpgradeVersion return null");
            else
                CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "byte[] CcuCoreRemotingProvider.RegisterDCUsForUpgradeVersion return " + Log.GetStringFromParameter(result));

            return result;
        }

        [AuthenticateNeeded(true)]
        public bool RegisterCRsForUpgradeVersion(byte dcuLogicalAddress, byte[] cardReaderAddresses, string upgradeVersion)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL, 
                () => "bool CcuCoreRemotingProvider.RegisterCRsForUpgradeVersion(byte dcuLogicalAddress, byte[] cardReaderAddresses, string upgradeVersion): [" +
                Log.GetStringFromParameters(dcuLogicalAddress, cardReaderAddresses, upgradeVersion) + "]");

            bool result = 
                DCUs.Singleton.RegisterCRsForUpgradeVersion(
                    dcuLogicalAddress, 
                    cardReaderAddresses, 
                    upgradeVersion);

            if (result)
                CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool CcuCoreRemotingProvider.RegisterCRsForUpgradeVersion return true");
            else
                CcuCore.DebugLog.Warning(Log.LOW_LEVEL, "bool CcuCoreRemotingProvider.RegisterCRsForUpgradeVersion return false");

            return result;
        }

        [AuthenticateNeeded(true)]
        public bool RegisterCCUsCRsForUpgradeVersion(Guid[] cardReaders, string upgradeVersion)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL, 
                () => string.Format(
                    "bool CcuCoreRemotingProvider.RegisterCCUsCRsForUpgradeVersion(Guid[] cardReaders, string upgradeVersion): [{0}]",
                    Log.GetStringFromParameters(
                        cardReaders, 
                        upgradeVersion)));

            foreach (var idCardReader in cardReaders)
                CardReaders.Singleton.RegisterCRsForUpgradeVersion(
                    idCardReader,
                    upgradeVersion);

            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL, 
                () => "bool CcuCoreRemotingProvider.RegisterCCUsCRsForUpgradeVersion return true");

            return true;
        }

        [AuthenticateNeeded(false)]
        public object GetIPSettings(string serverHashCode)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "object CcuCoreRemotingProvider.GetIPSettings(string serverHashCode): [" + Log.GetStringFromParameter(serverHashCode) + "]");
            object result = IPSettings.GetIPSettings();
            if (result == null)
                CcuCore.DebugLog.Warning(Log.LOW_LEVEL, "object CcuCoreRemotingProvider.GetIPSettings return null");
            else
                CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "object CcuCoreRemotingProvider.GetIPSettings returm " + result);

            return result;
        }


        [AuthenticateNeeded(true)]
        public bool ResetDcu(byte dcuLogicalAddress)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool CcuCoreRemotingProvider.ResetDcu(byte dcuLogicalAddress): [" + Log.GetStringFromParameter(dcuLogicalAddress) + "]");
            bool result = DCUs.Singleton.ResetDcu(dcuLogicalAddress);
            if (result)
                CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool CcuCoreRemotingProvider.ResetDcu return true");
            else
                CcuCore.DebugLog.Warning(Log.LOW_LEVEL, "bool CcuCoreRemotingProvider.ResetDcu return false");

            return result;
        }

        [AuthenticateNeeded(true)]
        public void RequestDcuMemoryLoad(byte dcuLogicalAddress)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "void CcuCoreRemotingProvider.RequestDcuMemoryLoad(byte dcuLogicalAddress): [" + Log.GetStringFromParameter(dcuLogicalAddress) + "]");
            DCUs.Singleton.RequestDcuMemoryLoad(dcuLogicalAddress);
        }

        [AuthenticateNeeded(true)]
        public bool SetIPSettings(bool isDHCP, string ipAddress, string subnetMask, string gateway)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool CcuCoreRemotingProvider.SetIPSettings(bool isDHCP, string ipAddress, string subnetMask, string gateway): [" +
                Log.GetStringFromParameters(isDHCP, ipAddress, subnetMask, gateway) + "]");
            bool result = IPSettings.SetIPSettings(isDHCP, ipAddress, subnetMask, gateway);
            if (result)
                CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool CcuCoreRemotingProvider.SetIPSettings return true");
            else
                CcuCore.DebugLog.Warning(Log.LOW_LEVEL, "bool CcuCoreRemotingProvider.SetIPSettings return false");

            return result;
        }

        [AuthenticateNeeded(false)]
        public bool IsCCU0(string serverHashCode)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool CcuCoreRemotingProvider.IsCCU0(string serverHashCode): [" + Log.GetStringFromParameter(serverHashCode) + "]");

            var result = Ccus.Singleton.IsCcu0();

            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () =>
                string.Format(
                    "bool CcuCoreRemotingProvider.IsCCU0 return {0}",
                    result
                        ? "true"
                        : "false"));

            return result;
        }

        [AuthenticateNeeded(false)]
        public bool IsCCUUpgrader(string serverHashCode)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool CcuCoreRemotingProvider.IsCCUUpgrader(string serverHashCode): [" + Log.GetStringFromParameter(serverHashCode) + "]");
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool CcuCoreRemotingProvider.IsCCUUpgrader return false");
            return false;
        }

        [AuthenticateNeeded(false)]
        public DateTime? GetCurrentTime(string serverHashCode)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "DateTime? CcuCoreRemotingProvider.GetCurrentTime(string serverHashCode): [" + Log.GetStringFromParameter(serverHashCode) + "]");
            DateTime? result = CcuCore.LocalTime;
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "DateTime? CcuCoreRemotingProvider.GetCurrentTime return " + result);

            return result;
        }

        [AuthenticateNeeded(false)]
        public object AcknowledgeAllowed(string serverHashCode)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "object CcuCoreRemotingProvider.AcknowledgeAllowed(string serverHashCode): [" + Log.GetStringFromParameter(serverHashCode) + "]");
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "object CcuCoreRemotingProvider.AcknowledgeAllowed return true");
            return true;
        }

        [AuthenticateNeeded(true)]
        public void SaveConfigurePassword(string password)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "void CcuCoreRemotingProvider.SaveConfigurePassword(string password): [" + Log.GetStringFromParameter(password) + "]");
            ConfigurePass cp = new ConfigurePass();
            cp.SaveConfigurePassword(password);
        }

        [AuthenticateNeeded(false)]
        public bool CompareConfigurePassword(string serverHashCode, string password)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool CcuCoreRemotingProvider.CompareConfigurePassword(string serverHashCode, string password): [" +
                Log.GetStringFromParameters(serverHashCode, password) + "]");
            ConfigurePass cp = new ConfigurePass();
            bool result = cp.CompareConfigurePassword(password);
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("bool CcuCoreRemotingProvider.CompareConfigurePassword return {0}", result));

            return result;
        }

        [AuthenticateNeeded(false)]
        public bool IsSetConfigurePassword(string serverHashCode)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool CcuCoreRemotingProvider.IsSetConfigurePassword(string serverHashCode): [" + Log.GetStringFromParameter(serverHashCode) + "]");
            ConfigurePass cp = new ConfigurePass();
            bool success = cp.IsUsedCcuConfigurePassword();
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("bool CcuCoreRemotingProvider.IsSetConfigurePassword return {0}", success));

            return success;
        }

        [AuthenticateNeeded(true)]
        public bool SetNTPTimeDifferenceTolerance(int interval)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool CcuCoreRemotingprovider.SetNTPTimeDifferenceTolerance(int interval): [" + Log.GetStringFromParameter(interval) + "]");

            CcuCore.Singleton.SetSynchronizationTimeIterval(interval);
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool CcuCoreRemotingprovider.SetNTPTimeDifferenceTolerance return true");
            return true;
        }


        [AuthenticateNeeded(true)]
        public void AAToCRsReportingChanged(bool isOn)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "void CcuCoreRemotingProvider.AAToCRsReportingChanged(bool isOn): [" + Log.GetStringFromParameter(isOn) + "]");
            DevicesAlarmSettings.Singleton.AllowAAToCRsReporting = isOn;
            AlarmAreas.Singleton.RunGeneralAlarmAreasReportingToCRChanged();

            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "Remoting: AAToCRsReportingChanged(bool isOn); isOn [" + isOn + "]");
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "AAs: reporting to CR's [" + isOn + "]");
        }


        [AuthenticateNeeded(true)]
        public bool SetOldEventsSettings(bool[] settings)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool CcuCoreRemotingProvider.SetOldEventsSettings(bool[] settings): [" + Log.GetStringFromParameter(settings) + "]");
            bool result = SendEventsToServerDispatcher.Singleton.SetOldEventsSettings(settings);
            if (result)
                CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool CcuCoreRemotingProvider.SetOldEventsSettings return true");
            else
                CcuCore.DebugLog.Warning(Log.LOW_LEVEL, "bool CcuCoreRemotingProvider.SetOldEventsSettings return false");

            return result;
        }

        [AuthenticateNeeded(true)]
        public byte GetTimeZonesDailyPlansState(byte objectType, Guid objectGuid)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "byte CcuCoreRemotingProvider.GetTimeZonesDailyPlansState(byte objectType, Guid objectGuid): [" +
                Log.GetStringFromParameters(objectType, objectGuid) + "]");
            byte result;
            switch (objectType)
            {
                case (byte)ObjectType.DailyPlan:
                    result = (byte)DailyPlans.Singleton.GetActualState(objectGuid);
                    break;
                case (byte)ObjectType.SecurityDailyPlan:
                    result = (byte)SecurityDailyPlans.Singleton.GetActualState(objectGuid);
                    break;
                case (byte)ObjectType.TimeZone:
                    result = (byte)TimeZones.Singleton.GetActualState(objectGuid);
                    break;
                case (byte)ObjectType.SecurityTimeZone:
                    result = (byte)SecurityTimeZones.Singleton.GetActualState(objectGuid);
                    break;
                default:
                    result = (byte)State.Unknown;
                    break;
            }

            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "byte CcuCoreRemotingProvider.GetTimeZonesDailyPlansState return " + result);
            return result;
        }

        [AuthenticateNeeded(true)]
        public bool SendBindings(byte[] bindingsData)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool CcuCoreRemotingProvider.SendBindings(byte[] bindingsData): [" + Log.GetStringFromParameter(bindingsData) + "]");
            InterCcuCommunication.Singleton.SaveBindings(bindingsData);
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool CcuCoreRemotingProvider.SendBindings return true");
            return true;
        }

        public event DObjects2Void CcuConfiguredStateChanged;
        public void DoCcuConfiguredStateChanged(params object[] param)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "void CcuCoreRemotingProvider.DoCcuConfiguredStateChanged(params object[] param): [" + Log.GetStringFromParameter(param) + "]");
            if (CcuConfiguredStateChanged != null)
            {
                try
                {
                    CcuConfiguredStateChanged(param);
                    CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "CcuConfiguredStateChanged reported to event");
                }
                catch (Exception error)
                {
                    HandledExceptionAdapter.Examine(error);
                    CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "CcuConfiguredStateChanged event failed");
                }
            }
            else
            {
                CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "No events for CcuConfiguredStateChanged");
            }
        }

        [AuthenticateNeeded(true)]
        public void SendUpsMonitorData()
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "void CcuCoreRemotingProvider.SendUpsMonitorData()");
            UPSMonitor.Singleton.SendUpsMonitorMsg = true;
        }

        [AuthenticateNeeded(true)]
        public void StopSendUpsMonitorData()
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "void CcuCoreRemotingProvider.StopSendUpsMonitorData()");
            UPSMonitor.Singleton.SendUpsMonitorMsg = false;
        }

        [AuthenticateNeeded(true)]
        public int[] GetOtherStatistics()
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "int[] CcuCoreRemotingProvider.GetOtherStatistics()");
            int[] result = CcuCore.Singleton.GetOtherStatistics();
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("int[] CcuCoreRemotingProvider.GetOtherStatistics return {0}", Log.GetStringFromParameters(result)));
            return result;
        }

        [AuthenticateNeeded(true)]
        public object[] ObtainDcuRunningTestStates()
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "object[] CcuCoreRemotingProvider.ObtainDcuRunningTestStates()");
            object[] result = DCUs.Singleton.GetActualDcuRunningTest();
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("object[] CcuCoreRemotingProvider.ObtainDcuRunningTestStates return {0}", Log.GetStringFromParameters(result)));
            return result;
        }

        [AuthenticateNeeded(true)]
        public void SendDcuRoutinTestConfiguration(object values)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL, 
                () => string.Format(
                    "void CcuCoreRemotingProvider.SendDcuRoutinTestConfiguration(object values): [{0}]",
                    Log.GetStringFromParameter(values)));

            DCUs.Singleton.SetDcuRunningTest(values);
        }

        [AuthenticateNeeded(true)]
        public bool RemoveCardsFromAntiPassBackZone(
            Guid guidAntiPassBackZone,
            Guid[] guidCards)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => 
                    string.Format(
                        "bool CcuCoreRemotingProvider.RemoveCardsFromAntiPassBackZone(Guid guidAntiPassBackZone, Guid[] guidCards) [{0}]",
                        Log.GetStringFromParameters(guidAntiPassBackZone, guidCards)));

            AntiPassBackZones.Singleton.RemoveCardsFromAntiPassBackZone(
                guidAntiPassBackZone,
                guidCards);

            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL, 
                () => "bool CcuCoreRemotingProvider.RemoveCardsFromAntiPassBackZone return true");

            return true;
        }

        [AuthenticateNeeded(true)]
        public object[] AddCardsToAntiPassBackZone(
            Guid guidAntiPassBackZone,
            Guid[] guidCards)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () =>
                    string.Format(
                        "object[] CcuCoreRemotingProvider.AddCardsToAntiPassBackZone(Guid guidAntiPassBackZone, Guid[] guidCards) [{0}]",
                        Log.GetStringFromParameters(guidAntiPassBackZone, guidCards)));

            var result = AntiPassBackZones.Singleton.AddCardsToAntiPassBackZone(
                guidAntiPassBackZone,
                guidCards);

            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () =>
                    string.Format(
                        "object[] CcuCoreRemotingProvider.AddCardsToAntiPassBackZone return {0}",
                        Log.GetStringFromParameter(result)));

            return result
                .Cast<object>()
                .ToArray();
        }

        [AuthenticateNeeded(true)]
        public object[] GetCardsInZone(
            Guid guidAntiPassBackZone)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () =>
                    string.Format(
                        "public CCUCardInAntiPassBackZone[] GetCardsInZone(Guid guidAntiPassBackZone) [{0}]",
                        Log.GetStringFromParameters(guidAntiPassBackZone)));

            object[] result = 
                AntiPassBackZones.Singleton
                    .GetCardsInZone(guidAntiPassBackZone)
                    .Cast<object>()
                    .ToArray();

            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () =>
                    string.Format(
                        "public CCUCardInAntiPassBackZone[] GetCardsInZone return {0}",
                        Log.GetStringFromParameter(result)));

            return result;
        }

        [AuthenticateNeeded(true)]
        public bool ResetCardReader(Guid cardReaderGuid)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool CcuCoreRemotingProvider.ResetCardReader(Guid cardReaderGuid): [" + Log.GetStringFromParameter(cardReaderGuid) + "]");
            CcuCore.Singleton.ResetCardReader(cardReaderGuid);
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool CcuCoreRemotingProvider.ResetCardReader return true");
            return true;
        }

        [AuthenticateNeeded(true)]
        public bool SetTimeManually(DateTime utcDateTime)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool CcuCoreRemotingProvider.SetTimeManually(DateTime utcDateTime): [" + Log.GetStringFromParameter(utcDateTime) + "]");
            if (!SystemTime.UseDateTimeNow)
            {
                var dateTime = utcDateTime.ToLocalTime();

                SystemTime.SetLocalTime(
                    dateTime.Year,
                    dateTime.Month,
                    dateTime.Day,
                    dateTime.Hour,
                    dateTime.Minute,
                    dateTime.Second,
                    dateTime.Millisecond);
            }
            else
            {
                SystemTime.SetSystemTime(
                    utcDateTime.Year,
                    utcDateTime.Month,
                    utcDateTime.Day,
                    utcDateTime.Hour,
                    utcDateTime.Minute,
                    utcDateTime.Second,
                    utcDateTime.Millisecond);
            }

            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool CcuCoreRemotingProvider.SetTimeManually return true");
            return true;
        }

        [AuthenticateNeeded(true)]
        public bool SetEnableLoggingSDPSTZChanges(bool enableLoggingSDPSTZChanges)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool CcuCoreRemotingProvider.SetEnableLoggingSDPSTZChanges(bool enableLoggingSDPSTZChanges): [" + Log.GetStringFromParameter(enableLoggingSDPSTZChanges) + "]");
            SecurityDailyPlans.Singleton.SetEnableLoggingSDPSTZChanges(enableLoggingSDPSTZChanges);
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool CcuCoreRemotingProvider.SetEnableLoggingSDPSTZChanges return true");
            return true;
        }

        [AuthenticateNeeded(true)]
        public bool SetAlarmAreaRestrictivePolicyForTimeBuying(bool alarmAreaRestrictivePolicyForTimeBuying)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () =>
                    "bool CcuCoreRemotingProvider.SetAlarmAreaRestrictivePolicyForTimeBuying(bool alarmAreaRestrictivePolicyForTimeBuying): ["
                    + Log.GetStringFromParameter(alarmAreaRestrictivePolicyForTimeBuying) + "]");

            AlarmAreas.Singleton.SetAlarmAreaRestrictivePolicyForTimeBuying(alarmAreaRestrictivePolicyForTimeBuying);

            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => "bool CcuCoreRemotingProvider.SetAlarmAreaRestrictivePolicyForTimeBuying return true");

            return true;
        }

        /// <summary>
        /// Save whether time is synchronized from the server
        /// </summary>
        /// <param name="syncingTimeFromServer"></param>
        /// <returns></returns>
        [AuthenticateNeeded(true)]
        public bool SetTimeSyncingFromServer(bool syncingTimeFromServer)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool CcuCoreRemotingProvider.SetTimeSyncingFromServer(bool syncingTimeFromServer): [" + Log.GetStringFromParameter(syncingTimeFromServer) + "]");

            CcuCore.Singleton.SetTimeSyncingFromServer(syncingTimeFromServer);
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool CcuCoreRemotingProvider.SetTimeSyncingFromServer return true");
            return true;
        }

        /// <summary>
        /// Sync CCU time by time from server
        /// </summary>
        /// <param name="actUtcServerDateTime"></param>
        /// <param name="periodicTimeSyncTolerance"></param>
        /// <returns></returns>
        [AuthenticateNeeded(true)]
        public bool SetTimeFromServer(DateTime actUtcServerDateTime, int periodicTimeSyncTolerance)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool CcuCoreRemotingProvider.SetTimeFromServer(DateTime actDateTimeFromSever, int periodicTimeSyncTolerance): [" +
                Log.GetStringFromParameters(actUtcServerDateTime, periodicTimeSyncTolerance) + "]");

            var actCcuDateTime = SystemTime.UseDateTimeNow
                ? DateTime.Now
                : SystemTime.GetLocalTime();

            DateTime actServerDateTime = actUtcServerDateTime.ToLocalTime();

            TimeSpan difference;

            if (actServerDateTime > actCcuDateTime)
                difference = actServerDateTime - actCcuDateTime;
            else
                difference = actCcuDateTime - actServerDateTime;

            if (difference.TotalSeconds > periodicTimeSyncTolerance)
            {
                CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "Time: synchronize from server [" + actServerDateTime + ":0]" +
                    " - ccu time [" + actCcuDateTime + ":" + actCcuDateTime.Millisecond + "] - difference [" + difference.TotalMilliseconds / 1000 + "] s.");

                if (!SystemTime.UseDateTimeNow)
                {
                    SystemTime.SetLocalTime(
                        actServerDateTime.Year,
                        actServerDateTime.Month,
                        actServerDateTime.Day,
                        actServerDateTime.Hour,
                        actServerDateTime.Minute,
                        actServerDateTime.Second,
                        0);
                }
                else
                {
                    SystemTime.SetSystemTime(
                        actUtcServerDateTime.Year,
                        actUtcServerDateTime.Month,
                        actUtcServerDateTime.Day,
                        actUtcServerDateTime.Hour,
                        actUtcServerDateTime.Minute,
                        actUtcServerDateTime.Second,
                        0);    
                }
                

                Events.ProcessEvent(
                    new EventCcuTimeAdjusted(
                        actCcuDateTime,
                        actServerDateTime));
            }

            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool CcuCoreRemotingProvider.SetTimeFromServer return true");
            return true;
        }

        /// <summary>
        /// Gets information about running threads
        /// </summary>
        /// <returns>collection of thread information</returns>
        [AuthenticateNeeded(true)]
        public object[] GetThreadsInfo()
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "object[] CcuCoreRemotingProvider.GetThreadsInfo()");

            List<object> result = new List<object>();

            DllToolHelp.THREADENTRY32[] entries = ProcessInfo.GetThreadEntries((uint)Process.GetCurrentProcess().Id);
            if (entries == null || entries.Length == 0)
            {
                CcuCore.DebugLog.Warning(Log.LOW_LEVEL, "object[] CcuCoreRemotingProvider.GetThreadsInfo return null");
                return null;
            }

            foreach (DllToolHelp.THREADENTRY32 entry in entries)
            {
                string name = string.Empty;
                bool? isBackground = null;

                ASafeThreadBase thread = ASafeThreadBase.GetThreadById((int)entry.th32ThreadID);
                if (thread != null)
                {
                    name = thread.Name;
                    if (thread.Thread != null)
                        isBackground = thread.Thread.IsBackground;
                }

                result.Add(new ThreadInfo
                {
                    Id = entry.th32ThreadID,
                    Priority = entry.tpBasePri,
                    Name = name,
                    IsBackground = isBackground,
                });
            }

            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("object[] CcuCoreRemotingProvider.GetThreadsInfo return {0}", Log.GetStringFromParameters(result)));
            return result.ToArray();
        }

        
        /// <summary>
        /// Sets the Log verbosity
        /// </summary>
        /// <param name="level">verbosity level</param>
        /// <returns>result of setting</returns>
        [AuthenticateNeeded(true)]
        public bool SetVerbosityLevel(byte level)
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("bool CcuCoreRemotingProvider.SetVerbosityLevel(byte level): [{0}]", Log.GetStringFromParameters(level)));
            CcuCore.DebugLog.VerbosityLevel = level;
            CcuCore.Singleton.SetRegistryLogVerbosityLevel(level);

            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool CcuCoreRemotingProvider.SetVerbosityLevel return true");
            return true;
        }

        /// <summary>
        /// Gets the  current Log verbosity
        /// </summary>
        /// <returns>verbosity</returns>
        [AuthenticateNeeded(true)]
        public byte GetVerbosityLevel()
        {
            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("byte CcuCoreRemotingProvider.GetVerbosityLevel()"));

            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => string.Format("byte CcuCoreRemotingProvider.GetVerbosityLevel return {0}", Log.GetStringFromParameters(CcuCore.DebugLog.VerbosityLevel)));
            return CcuCore.DebugLog.VerbosityLevel;
        }

        /// <summary>
        /// Set allow pin caching in card reader menu
        /// </summary>
        /// <param name="allowPINCaching"></param>
        [AuthenticateNeeded(true)]
        public void SetAllowPINCachingInCardReaderMenu(bool allowPINCaching)
        {
            CcuCardReaders.SetAllowPINCachingInCardReaderMenu(allowPINCaching);
        }

        [AuthenticateNeeded(true)]
        public void SetAllowCodeLength(
            int minimalCodeLength,
            int maximalCodeLength)
        {
            CcuCardReaders.SetAllowCodeLength(
                minimalCodeLength,
                maximalCodeLength);
        }

        [AuthenticateNeeded(true)]
        public void SetPinConfirmationObligatory(bool isPinConfirmationObligatory)
        {
            CcuCardReaders.SetPinConfirmationObligatory(isPinConfirmationObligatory);
        }

        /// <summary>
        /// Runs GC.Collect
        /// </summary>
        /// <returns></returns>
        [AuthenticateNeeded(true)]
        public bool RunGcCollect()
        {
            GC.Collect();
            return true;
        }

        [AuthenticateNeeded(true)]
        public void SetSensorBlockingType(
            Guid idAlarmArea,
            Guid idInput,
            SensorBlockingType sensorBlockingType)
        {
            AlarmAreas.Singleton.SetSensorBlockingType(
                idAlarmArea,
                idInput,
                sensorBlockingType);
        }

        [AuthenticateNeeded(true)]
        public void UnblockCardReader(Guid idCardReader)
        {
            CardReaders.Singleton.StopTimeoutAndResetInvalidGinRetriesLimitReached(idCardReader);
        }

        [AuthenticateNeeded(true)]
        public string[] GetDebugFiles()
        {
            return CcuCore.Singleton.GetDebugFiles();
        }

        [AuthenticateNeeded(true)]
        public ICollection<MemoryInfo> GetMemoryReport()
        {
            var memoryInfos = new LinkedList<MemoryInfo>();

            var pools = AObjectPool.MakeReport();

            if (pools != null)
                foreach (var aObjectPool in pools)
                {
                    memoryInfos.AddLast(
                        new MemoryInfo(
                            aObjectPool.ToString(),
                            aObjectPool.Count));
                }

            var pQueues = AProcessingQueue.MakeReport();

            if (pQueues != null)
                foreach (var aProcessingQueue in pQueues)
                {
                    memoryInfos.AddLast(
                        new MemoryInfo(
                            aProcessingQueue.ToString(),
                            aProcessingQueue.Count));
                }

            return memoryInfos;
        }

        private ProcessInfo GetCkmProcessInfo()
        {
            return ProcessInfo.GetAllProcessInfos().FirstOrDefault(
                inf => inf.Name.ToLower().Contains("cekickstart"));
        }

        [AuthenticateNeeded(true)]
        public bool StopCKM()
        {
            try
            {
                var ckmProcessInfo = GetCkmProcessInfo();

                if (ckmProcessInfo == null)
                    return true;

                ckmProcessInfo.ProcessInstance.Kill();
                return true;
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
                return false;
            }
        }

        [AuthenticateNeeded(true)]
        public bool StartCKM(bool isImplicity)
        {
            try
            {
                var ckmProcessInfo = GetCkmProcessInfo();

                if (ckmProcessInfo != null)
                    return true;

                var proccess = new Process
                {
                    StartInfo =
                    {
                        FileName = isImplicity
                            ? "\\Windows\\CeKickstart.exe"
                            : "\\NandFlash\\CeKickstart\\CeKickstart.exe",
                        Arguments = "avoid-apploader"
                    }
                };

                return proccess.Start();
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
                return false;
            }
        }

        [AuthenticateNeeded(true)]
        public bool RunProccess(string cmd)
        {
            try
            {
                var cmdParts = cmd.Split(' ');
                string arguments = string.Empty;

                if (cmdParts.Length > 1)
                {
                    for (int i = 1; i < cmdParts.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(cmdParts[i]))
                        {
                            arguments = cmdParts[i];
                            break;
                        }
                    }
                }

                var proccess = new Process
                {
                    StartInfo =
                    {
                        FileName = cmdParts[0],
                        Arguments = arguments
                    }
                };

                return proccess.Start();
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
                return false;
            }
        }
    }
}
