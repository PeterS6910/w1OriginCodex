using System.Collections.Generic;

using Contal.Cgp.Globals.PlatformPC;
using Contal.Cgp.RemotingCommon;
using Contal.Cgp.Server.Alarms;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Globals;
using Contal.IwQuick.Remoting;
using Contal.IwQuick.Threads;

namespace Contal.Cgp.Server.DB
{
    public sealed class AlarmPrioritiesDbs : 
        ABaseOrmTable<AlarmPrioritiesDbs, AlarmPriorityDatabase>, 
        IAlarmPrioritiesDbs
    {
        private AlarmPrioritiesDbs() : base(null)
        {
        }

        public override object ParseId(string strObjectId)
        {
            byte result;

            return byte.TryParse(strObjectId, out result)
                ? (object)result
                : null;
        }

        public override bool HasAccessView(Login login)
        {
            return AccessChecker.HasAccessControl(BaseAccess.GetAccessesForGroup(LoginAccessGroups.ALARMS), login);
        }

        public override bool HasAccessInsert(Login login)
        {
            return AccessChecker.HasAccessControl(BaseAccess.GetAccessesForGroup(LoginAccessGroups.ALARMS), login);
        }

        public override bool HasAccessUpdate(Login login)
        {
            return AccessChecker.HasAccessControl(BaseAccess.GetAccessesForGroup(LoginAccessGroups.ALARMS), login);
        }

        public override bool HasAccessDelete(Login login)
        {
            return AccessChecker.HasAccessControl(BaseAccess.GetAccessesForGroup(LoginAccessGroups.ALARMS), login);
        }

        public override void CUDSpecial(AlarmPriorityDatabase alarmPriority, ObjectDatabaseAction objectDatabaseAction)
        {
            AlarmsManager.Singleton.ReloadAlarmPriorityFromDatabase(GenerateDictionaryForAlarmPriorities(),
                GenerateDictionaryForClosestParentObject(), GenerateDictionaryForSecondClosestParentObject());
        }

        public void LoadAlarmPrioritiesFromDatabase()
        {
            AlarmsManager.Singleton.ReloadAlarmPriorityFromDatabase(GenerateDictionaryForAlarmPriorities(),
                GenerateDictionaryForClosestParentObject(), GenerateDictionaryForSecondClosestParentObject());
        }

        public void SaveListAlarmPriorities(ICollection<AlarmPriorityDatabase> listApD)
        {
            if (listApD != null)
            {
                foreach (AlarmPriorityDatabase apD in listApD)
                {

                }
            }
        }

        private IDictionary<AlarmType, AlarmPriority> GenerateDictionaryForAlarmPriorities()
        {
            IDictionary<AlarmType, AlarmPriority> dicAlarmPriorities = new Dictionary<AlarmType, AlarmPriority>();

            ICollection<AlarmPriorityDatabase> listAp = List();
            try
            {
                if (listAp != null)
                {
                    foreach (AlarmPriorityDatabase apDatabase in listAp)
                    {
                        dicAlarmPriorities.Add(new KeyValuePair<AlarmType, AlarmPriority>(apDatabase.AlarmType, apDatabase.AlarmPriority));
                    }
                }
            }
            catch { }
            return dicAlarmPriorities;
        }

        private IDictionary<AlarmType, ObjectType?> GenerateDictionaryForClosestParentObject()
        {
            IDictionary<AlarmType, ObjectType?> dicClosestParentObject = new Dictionary<AlarmType, ObjectType?>();

            ICollection<AlarmPriorityDatabase> listAp = List();
            try
            {
                if (listAp != null)
                {
                    foreach (AlarmPriorityDatabase apDatabase in listAp)
                    {
                        if (apDatabase != null && apDatabase.ClosestParentObject != null)
                        {
                            dicClosestParentObject.Add(new KeyValuePair<AlarmType, ObjectType?>(apDatabase.AlarmType, apDatabase.ClosestParentObject));
                        }
                    }
                }
            }
            catch { }
            return dicClosestParentObject;
        }

        private IDictionary<AlarmType, ObjectType?> GenerateDictionaryForSecondClosestParentObject()
        {
            IDictionary<AlarmType, ObjectType?> dicSecondClosestParentObject = new Dictionary<AlarmType, ObjectType?>();

            ICollection<AlarmPriorityDatabase> listAp = List();
            try
            {
                if (listAp != null)
                {
                    foreach (AlarmPriorityDatabase apDatabase in listAp)
                    {
                        if (apDatabase != null && apDatabase.SecondClosestParentObject != null)
                        {
                            dicSecondClosestParentObject.Add(new KeyValuePair<AlarmType, ObjectType?>(apDatabase.AlarmType, apDatabase.SecondClosestParentObject));
                        }
                    }
                }
            }
            catch { }
            return dicSecondClosestParentObject;
        }

        public IList<AlarmPriorityDatabase> GetAlarmTypesFromDatabase(IList<AlarmType> wantedAlarmTypes)
        {
            IList<AlarmPriorityDatabase> listApDatabase = new List<AlarmPriorityDatabase>();

            foreach (AlarmType alarmType in wantedAlarmTypes)
            {
                AlarmPriorityDatabase apDatabase = GetById((byte)alarmType);
                if (apDatabase == null)
                {
                    apDatabase = new AlarmPriorityDatabase(alarmType, AlarmsManager.Singleton.GetAlarmPriority(alarmType),
                        AlarmsManager.Singleton.GetDefaultClosetParentObject(alarmType), AlarmsManager.Singleton.GetDefaultSecondClosetParentObject(alarmType));
                }
                else
                {
                    if (apDatabase.ClosestParentObject == null)
                    {
                        ObjectType? closestParentObject = AlarmsManager.Singleton.GetDefaultClosetParentObject(alarmType);
                        
                        if (closestParentObject != null)
                            apDatabase.ClosestParentObjectDb = (int)closestParentObject;
                    }

                    if (apDatabase.SecondClosestParentObject == null)
                    {
                        ObjectType? secondClosestParentObject = AlarmsManager.Singleton.GetDefaultSecondClosetParentObject(alarmType);

                        if (secondClosestParentObject != null)
                            apDatabase.SecondClosestParentObjectDb = (int)secondClosestParentObject;
                    }
                }

                listApDatabase.Add(apDatabase);
            }
            return listApDatabase;
        }

        public void SaveAlarmPrioritiesToDatabase(IList<AlarmPriorityDatabase> alarmPriorities)
        {
            foreach (AlarmPriorityDatabase alarmPriority in alarmPriorities)
            {
                SaveAlarmPriorityDbs(alarmPriority);
            }
            AlarmsManager.Singleton.ReloadAlarmPriorityFromDatabase(GenerateDictionaryForAlarmPriorities(),
                GenerateDictionaryForClosestParentObject(), GenerateDictionaryForSecondClosestParentObject());

            CgpServerRemotingProvider.Singleton.ForeachCallbackHandler(RunAlarmSettingsChanged, DelegateSequenceBlockingMode.Asynchronous, false);
        }

        public void RunAlarmSettingsChanged(ARemotingCallbackHandler remoteHandler)
        {
            if (remoteHandler is AlarmSettingsChangedHandler)
                (remoteHandler as AlarmSettingsChangedHandler).RunEvent();
        }

        private void SaveAlarmPriorityDbs(AlarmPriorityDatabase alarmPriority)
        {
            AlarmPriorityDatabase apDatabase = GetById(alarmPriority.AlarmTypeDb);
            if (apDatabase != null)
            {
                if (apDatabase.AlarmPriority != alarmPriority.AlarmPriority ||
                    apDatabase.ClosestParentObject != alarmPriority.ClosestParentObject ||
                    apDatabase.SecondClosestParentObject != alarmPriority.SecondClosestParentObject)
                {
                    UpdateAlarmPriorityInDatabase(apDatabase, alarmPriority.AlarmPriority, alarmPriority.ClosestParentObjectDb,
                        alarmPriority.SecondClosestParentObjectDb);
                }
            }
            else
            {
                CreateNewAlarmPriorityInDatabase(alarmPriority);
            }
        }

        private void UpdateAlarmPriorityInDatabase(AlarmPriorityDatabase apDatabase, AlarmPriority priority, int? closestParentObjectDb, int? secondClosestAlarmObjectDb)
        {
            try
            {
                AlarmPriorityDatabase apDbsSave = GetObjectForEdit(apDatabase.AlarmTypeDb);
                apDbsSave.AlarmPriorityDb = (byte)priority;
                apDbsSave.ClosestParentObjectDb = closestParentObjectDb;
                apDbsSave.SecondClosestParentObjectDb = secondClosestAlarmObjectDb;
                Update(apDbsSave);
            }
            catch { }
        }

        private void CreateNewAlarmPriorityInDatabase(AlarmPriorityDatabase apDatabase)
        {
            try
            {
                var apDbsSave = new AlarmPriorityDatabase(apDatabase.AlarmType, apDatabase.AlarmPriority, apDatabase.ClosestParentObject, apDatabase.SecondClosestParentObject);
                Insert(ref apDbsSave);
            }
            catch { }
        }

        public ObjectType? GetClosestParentObject(AlarmType alarmType)
        {
            return AlarmsManager.Singleton.GetClosestParentObject(alarmType);
        }

        public ObjectType? GetSecondClosestParentObject(AlarmType alarmType)
        {
            return AlarmsManager.Singleton.GetSecondClosestParentObject(alarmType);
        }

        public override Contal.Cgp.Globals.ObjectType ObjectType
        {
            get { return Contal.Cgp.Globals.ObjectType.AlarmPriority; }
        }
    }
}
