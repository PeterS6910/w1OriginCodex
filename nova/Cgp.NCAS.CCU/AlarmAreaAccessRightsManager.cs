using System;
using System.Collections.Generic;
using System.Linq;
using Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes;
using Contal.Cgp.NCAS.CCU.DB;
using Contal.IwQuick;
using Contal.IwQuick.Data;
using System.Text;
using JetBrains.Annotations;

namespace Contal.Cgp.NCAS.CCU
{
    public sealed class AlarmAreaAccessRightsManager :
        ASingleton<AlarmAreaAccessRightsManager>
    {
        public class CachedAaRights
        {
            public bool AlarmAreaSet { get; private set; }
            public bool AlarmAreaUnset { get; private set; }
            public bool AlarmAreaUnconditionalSet { get; private set; }
            public bool AlarmAreaAlarmAcknowledge { get; private set; }
            public bool SensorHandling { get; private set; }
            public bool CREventLogHandling { get; private set; }
            public bool AlarmAreaTimeBuying { get; private set; }

            public void CumulateRights([NotNull] DB.ACLSettingAA aclSettingAA)
            {
                AlarmAreaSet = AlarmAreaSet || aclSettingAA.AlarmAreaSet;
                AlarmAreaUnset = AlarmAreaUnset || aclSettingAA.AlarmAreaUnset;
                AlarmAreaUnconditionalSet = AlarmAreaUnconditionalSet || aclSettingAA.AlarmAreaUnconditionalSet;
                AlarmAreaAlarmAcknowledge = AlarmAreaAlarmAcknowledge || aclSettingAA.AlarmAreaAlarmAcknowledge;
                SensorHandling = SensorHandling || aclSettingAA.SensorHandling;
                CREventLogHandling = CREventLogHandling || aclSettingAA.CREventLogHandling;
                AlarmAreaTimeBuying = AlarmAreaTimeBuying || aclSettingAA.AlarmAreaTimeBuying;
            }
        }

        public enum AlarmAreaActivationRights
        {
            Set = 0,
            Unset = 1,
            UnconditionalSet = 2,
            TimeBuying = 5,
            //checks if you have rights to sensor handling to specific alarm area 
            SensorHandling = 3,
            //checks if you have any rights to sensor handling 
            SensorHandlingAny = 4,
        }

        public const int ACLP_CACHE_TIMEOUT = 1000;

        private AlarmAreaAccessRightsManager() : base(null)
        {
        }

        #region ACL check rights methods

        private static bool CheckACLSettingAA(
            Guid guidAlarmArea, 
            DB.ACLSettingAA aclSettingAA, 
            AlarmAreaActivationRights alarmAreaActivationRights)
        {
            if (aclSettingAA == null) return false;

            switch (alarmAreaActivationRights)
            {
                case AlarmAreaActivationRights.Set: 
                    return aclSettingAA.GuidAlarmArea == guidAlarmArea && aclSettingAA.AlarmAreaSet;

                case AlarmAreaActivationRights.Unset: 
                    return aclSettingAA.GuidAlarmArea == guidAlarmArea && aclSettingAA.AlarmAreaUnset;

                case AlarmAreaActivationRights.UnconditionalSet: 
                    return aclSettingAA.GuidAlarmArea == guidAlarmArea && aclSettingAA.AlarmAreaUnconditionalSet;

                case AlarmAreaActivationRights.TimeBuying:
                    return aclSettingAA.GuidAlarmArea == guidAlarmArea && aclSettingAA.AlarmAreaTimeBuying;

                case AlarmAreaActivationRights.SensorHandling: 
                    return aclSettingAA.GuidAlarmArea == guidAlarmArea && aclSettingAA.SensorHandling;

                case AlarmAreaActivationRights.SensorHandlingAny: 
                    return aclSettingAA.SensorHandling;
            }

            return false;
        }

        private bool CheckAlarmAreaActivationRights(
            Guid idPerson, 
            Guid guidAlarmArea, 
            AlarmAreaActivationRights alarmAreaActivationRights)
        {
            if (idPerson == Guid.Empty) 
                return false;

            var aclSettingAAs = Database.ConfigObjectsEngine.AclSettingAasStorage.GetAclSettingAAs(
                idPerson,
                guidAlarmArea);

            return aclSettingAAs.Any(
                aclsAA => CheckACLSettingAA(
                    guidAlarmArea,
                    aclsAA,
                    alarmAreaActivationRights));
        }

        public ICollection<Guid> GetAlarmAreasGuid(AccessDataBase accessData)
        {
            var aclSettingAAs = Database.ConfigObjectsEngine.AclSettingAasStorage.GetAclSettingAAs(
                accessData.IdPerson);

            var alarmAreasGuid = new HashSet<Guid>();

            foreach (var aclsAA in aclSettingAAs)
            {
                if (aclsAA.CREventLogHandling)
                    alarmAreasGuid.Add(aclsAA.GuidAlarmArea);
            }

            return alarmAreasGuid;
        }

        public string CheckAlarmAreaActivationRightsTest(Guid guidPerson, Guid guidAlarmArea)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL, () => string.Format("string AccessControlListsManager.CheckAlarmAreaActivationRightsTest(Guid guidPerson, Guid guidAlarmArea): [{0}]",
                Log.GetStringFromParameters(guidPerson, guidAlarmArea)));

            var results = new bool[5];

            var aclSettingAAs = Database.ConfigObjectsEngine.AclSettingAasStorage.GetAclSettingAAs(
                guidPerson,
                guidAlarmArea);

            foreach (var aclsAA in aclSettingAAs)
            {
                if (CheckACLSettingAA(guidAlarmArea, aclsAA, AlarmAreaActivationRights.Set))
                    results[0] = true;

                if (CheckACLSettingAA(guidAlarmArea, aclsAA, AlarmAreaActivationRights.Unset))
                    results[1] = true;

                if (CheckACLSettingAA(guidAlarmArea, aclsAA, AlarmAreaActivationRights.UnconditionalSet))
                    results[2] = true;

                if (CheckACLSettingAA(guidAlarmArea, aclsAA, AlarmAreaActivationRights.SensorHandling))
                    results[3] = true;

                if (CheckACLSettingAA(guidAlarmArea, aclsAA, AlarmAreaActivationRights.TimeBuying))
                    results[4] = true;
            }

            string result = GetStringResults(results);

            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL, 
                () => string.Format(
                    "string AccessControlListsManager.CheckAlarmAreaActivationRightsTest return {0}", 
                    Log.GetStringFromParameter(results)));

            return result;
        }

        private static string GetStringResults(bool[] results)
        {
            var resultsInfo = new StringBuilder();

            resultsInfo.AppendLine(
                results[0]
                    ? "Set: true"
                    : "Set: false");

            resultsInfo.AppendLine(
                results[1]
                    ? "Unset: true"
                    : "Unset: false");

            resultsInfo.AppendLine(
                results[2]
                    ? "Unconditional set: true"
                    : "Unconditional set: false");

            resultsInfo.AppendLine(
                results[3]
                    ? "Sensor handling: true"
                    : "Sensor handling: false");

            resultsInfo.AppendLine(
                results[4]
                    ? "Time buying: true"
                    : "Time buying: false");

            return resultsInfo.ToString();
        }

        public bool CheckRigthsToSet(
            AccessDataBase accessData,
            Guid guidAlarmArea)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "bool AccessControlListsManager.CheckRigthsToSet(Guid idPerson, Guid guidAlarmArea): [{0}]",
                    Log.GetStringFromParameters(
                    accessData.IdPerson,
                    guidAlarmArea)));

            bool result =
                CheckAlarmAreaActivationRights(
                    accessData.IdPerson,
                    guidAlarmArea,
                    AlarmAreaActivationRights.Set);

            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "bool AccessControlListsManager.CheckRigthsToSet return {0}",
                    result));

            return result;
        }

        public bool CheckRigthsToUnset(
            AccessDataBase accessData,
            Guid guidAlarmArea)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "bool AccessControlListsManager.CheckRigthsToUnset(Guid idPerson, Guid guidAlarmArea): [{0}]",
                    Log.GetStringFromParameters(
                        accessData.IdPerson,
                        guidAlarmArea)));

            bool result =
                CheckAlarmAreaActivationRights(
                    accessData.IdPerson,
                    guidAlarmArea,
                    AlarmAreaActivationRights.Unset);

            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "bool AccessControlListsManager.CheckRigthsToUnset return {0}",
                    result));

            return result;
        }

        public bool CheckRigthsToUnset(
            Guid idPerson,
            Guid guidAlarmArea)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "bool AccessControlListsManager.CheckRigthsToUnset(Guid idPerson, Guid guidAlarmArea): [{0}]",
                    Log.GetStringFromParameters(
                        idPerson,
                        guidAlarmArea)));

            bool result =
                CheckAlarmAreaActivationRights(
                    idPerson,
                    guidAlarmArea,
                    AlarmAreaActivationRights.Unset);

            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "bool AccessControlListsManager.CheckRigthsToUnset return {0}",
                    result));

            return result;
        }

        public bool CheckRigthsToUnconditionalSet(
            AccessDataBase accessData,
            Guid guidAlarmArea)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "bool AccessControlListsManager.CheckRigthsToUnconditionalSet(Guid idPerson, Guid guidAlarmArea): [{0}]",
                    Log.GetStringFromParameters(
                        accessData.IdPerson,
                        guidAlarmArea)));

            bool result =
                CheckAlarmAreaActivationRights(
                    accessData.IdPerson,
                    guidAlarmArea,
                    AlarmAreaActivationRights.UnconditionalSet);

            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "bool AccessControlListsManager.CheckRigthsToUnconditionalSet return {0}",
                    result));

            return result;
        }

        public bool CheckRigthsToTimeBuying(
            AccessDataBase accessData,
            Guid guidAlarmArea)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "bool AccessControlListsManager.CheckRigthsToTimeBuying(Guid idPerson, Guid guidAlarmArea): [{0}]",
                    Log.GetStringFromParameters(
                        accessData.IdPerson,
                        guidAlarmArea)));

            bool result =
                CheckAlarmAreaActivationRights(
                    accessData.IdPerson,
                    guidAlarmArea,
                    AlarmAreaActivationRights.TimeBuying);

            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "bool AccessControlListsManager.CheckRigthsToTimeBuying return {0}",
                    result));

            return result;
        }

        public bool CheckRigthsToTimeBuying(
            Guid idPerson,
            Guid guidAlarmArea)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "bool AccessControlListsManager.CheckRigthsToTimeBuying(Guid idPerson, Guid guidAlarmArea): [{0}]",
                    Log.GetStringFromParameters(
                        idPerson,
                        guidAlarmArea)));

            bool result =
                CheckAlarmAreaActivationRights(
                    idPerson,
                    guidAlarmArea,
                    AlarmAreaActivationRights.TimeBuying);

            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "bool AccessControlListsManager.CheckRigthsToTimeBuying return {0}",
                    result));

            return result;
        }

        public bool CheckRigthsToSensorHandling(AccessDataBase accessData, Guid guidAlarmArea)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "bool AccessControlListsManager.CheckRigthsToSensorHandling(Guid idPerson, Guid guidAlarmArea): [{0}]",
                    Log.GetStringFromParameters(
                    accessData.IdPerson,
                    guidAlarmArea)));

            bool result =
                CheckAlarmAreaActivationRights(
                    accessData.IdPerson,
                    guidAlarmArea,
                    AlarmAreaActivationRights.SensorHandling);

            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "bool AccessControlListsManager.CheckRigthsToSensorHandling return {0}",
                    result));

            return result;
        }

        public Dictionary<Guid, CachedAaRights> GetAlarmAreasRightsCacheForPerson(Guid idPerson)
        {
            var result = new Dictionary<Guid, CachedAaRights>();

            foreach (var aclSettingAA in Database.ConfigObjectsEngine.AclSettingAasStorage.GetAclSettingAAs(idPerson))
            {
                var idAlarmArea = aclSettingAA.GuidAlarmArea;
                CachedAaRights cachedAaRights;

                if (!result.TryGetValue(idAlarmArea, out cachedAaRights))
                {
                    cachedAaRights = new CachedAaRights();

                    result.Add(
                        idAlarmArea, 
                        cachedAaRights);
                }

                cachedAaRights.CumulateRights(aclSettingAA);
            }

            return result;
        }

        #endregion
    }
}