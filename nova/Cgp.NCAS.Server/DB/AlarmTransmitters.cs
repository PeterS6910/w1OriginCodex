using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Contal.CatCom;
using Contal.Cgp.BaseLib;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.RemotingCommon;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.NCAS.Server.Beans.ModifyObjects;
using Contal.Cgp.NCAS.Server.Beans.Shorts;
using Contal.Cgp.NCAS.Server.CcuDataReplicator;
using Contal.Cgp.NCAS.Server.ServerAlarms;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Server.DB;
using Contal.IwQuick.Data;
using Contal.IwQuick.Remoting;
using Contal.IwQuick.Sys;
using Contal.IwQuick.Threads;

namespace Contal.Cgp.NCAS.Server.DB
{
    public sealed class AlarmTransmitters :
        ANcasBaseOrmTableWithAlarmInstruction<AlarmTransmitters, AlarmTransmitter>,
        IAlarmTransmitters
    {
        private readonly SyncDictionary<string, OnlineState> _onlineStates = new SyncDictionary<string, OnlineState>();

        private AlarmTransmitters()
            : base(
                  null,
                  new CudPreparationForObjectWithVersion<AlarmTransmitter>())
        {
        }

        public void ValidateAlarmTransmitters()
        {
            var alarmTransmitters = List();

            if (alarmTransmitters == null)
                return;

            foreach (var alarmTransmitter in alarmTransmitters)
            {
                CatComClient.Singleton.ValidateCat(IPAddress.Parse(alarmTransmitter.IpAddress));
            }
        }

        protected override IEnumerable<AlarmTransmitter> GetObjectsWithLocalAlarmInstruction()
        {
            return SelectLinq<AlarmTransmitter>(
                alarmTransmitter =>
                    alarmTransmitter.LocalAlarmInstruction != null
                    && alarmTransmitter.LocalAlarmInstruction != string.Empty);
        }

        public override void AfterInsert(AlarmTransmitter alarmTransmitter)
        {
            base.AfterInsert(alarmTransmitter);

            CatComClient.Singleton.ValidateCat(IPAddress.Parse(alarmTransmitter.IpAddress));
        }

        public override void AfterUpdate(AlarmTransmitter newObject, AlarmTransmitter oldObjectBeforUpdate)
        {
            base.AfterUpdate(newObject, oldObjectBeforUpdate);

            if (newObject.IpAddress == oldObjectBeforUpdate.IpAddress)
                return;

            CatComClient.Singleton.RemoveCatValidation(IPAddress.Parse(oldObjectBeforUpdate.IpAddress));
            _onlineStates.Remove(oldObjectBeforUpdate.IpAddress);

            NCASServer.Singleton.GetAlarmsQueue().ServerAlarmsOwner.RemoveAlarm(
                CcuCatUnreachableServerAlarm.CreateAlarmKey(oldObjectBeforUpdate.IpAddress));

            CatComClient.Singleton.ValidateCat(IPAddress.Parse(newObject.IpAddress));
        }

        public override void AfterDelete(AlarmTransmitter alarmTransmitter)
        {
            base.AfterDelete(alarmTransmitter);

            CatComClient.Singleton.RemoveCatValidation(IPAddress.Parse(alarmTransmitter.IpAddress));
            _onlineStates.Remove(alarmTransmitter.IpAddress);

            NCASServer.Singleton.GetAlarmsQueue().ServerAlarmsOwner.RemoveAlarm(
                CcuCatUnreachableServerAlarm.CreateAlarmKey(alarmTransmitter.IpAddress));
        }

        public override void CUDSpecial(AlarmTransmitter alarmTransmitter, ObjectDatabaseAction objectDatabaseAction)
        {
            if (alarmTransmitter == null)
                return;

            if (objectDatabaseAction == ObjectDatabaseAction.Update)
            {
                DataReplicationManager.Singleton.SendModifiedObjectToCcus(alarmTransmitter);
            }
        }

        public override bool HasAccessView(Login login)
        {
            return AccessChecker.HasAccessControl(
                NCASAccess.GetAccessesForGroup(AccessNcasGroups.AlarmTransmitters),
                login);
        }

        public override bool HasAccessInsert(Login login)
        {
            return AccessChecker.HasAccessControl(
                NCASAccess.GetAccess(AccessNCAS.AlarmTransmittersInsertDeletePerform),
                login);
        }

        public override bool HasAccessUpdate(Login login)
        {
            return AccessChecker.HasAccessControl(
                NCASAccess.GetAccessesForGroup(AccessNcasGroups.AlarmTransmitters),
                login);
        }

        public override bool HasAccessDelete(Login login)
        {
            return AccessChecker.HasAccessControl(
                NCASAccess.GetAccess(AccessNCAS.AlarmTransmittersInsertDeletePerform),
                login);
        }

        public ICollection<AlarmTransmitterShort> ShortSelectByCriteria(
            IList<FilterSettings> filterSettings,
            out Exception error)
        {
            var list = SelectByCriteria(filterSettings, out error);

            if (list != null)
            {
                return
                    new LinkedList<AlarmTransmitterShort>(
                        list.Select(alarmTransmitter => new AlarmTransmitterShort(
                            alarmTransmitter,
                            GetOnlineState(alarmTransmitter.IpAddress))));
            }

            return null;
        }

        public ICollection<IModifyObject> ListModifyObjects(out Exception error)
        {
            var alarmTransmitters = List(out error);

            if (alarmTransmitters == null)
                return null;

            return
                new LinkedList<IModifyObject>(
                    alarmTransmitters.Select(alarmTransmitter => new AlarmTransmitterModifyObj(alarmTransmitter))
                        .OrderBy(alarmArcModifyObj => alarmArcModifyObj.ToString())
                        .Cast<IModifyObject>());
        }

        #region SearchFunctions

        public override ICollection<AOrmObject> GetSearchList(string name, bool single)
        {
            if (single && string.IsNullOrEmpty(name))
            {
                var alarmTransmitters = List();

                return alarmTransmitters != null
                    ? new LinkedList<AOrmObject>(
                        alarmTransmitters.OrderBy(alarmTransmitter => alarmTransmitter.Name).Cast<AOrmObject>())
                    : null;
            }

            if (string.IsNullOrEmpty(name))
                return null;

            var linqResult =
                single
                    ? SelectLinq<AlarmTransmitter>(alarmTransmitter => alarmTransmitter.Name.IndexOf(name) >= 0)
                    : SelectLinq<AlarmTransmitter>(
                        alarmTransmitter =>
                            alarmTransmitter.Name.IndexOf(name) >= 0 ||
                            alarmTransmitter.Description.IndexOf(name) >= 0);

            return linqResult != null
                ? new LinkedList<AOrmObject>(
                    linqResult.OrderBy(alarmTransmitter => alarmTransmitter.Name).Cast<AOrmObject>())
                : null;
        }

        public override ICollection<AOrmObject> FulltextSearch(string name)
        {
            ICollection<AlarmTransmitter> linqResult;

            if (!string.IsNullOrEmpty(name))
            {
                linqResult =
                    SelectLinq<AlarmTransmitter>(
                        alarmTransmitter =>
                            alarmTransmitter.Name.IndexOf(name) >= 0 ||
                            alarmTransmitter.Description.IndexOf(name) >= 0);
            }
            else
            {
                linqResult = null;
            }

            return linqResult != null
                ? new LinkedList<AOrmObject>(
                    linqResult.OrderBy(alarmTransmitter => alarmTransmitter.Name).Cast<AOrmObject>())
                : new LinkedList<AOrmObject>();
        }

        public override ICollection<AOrmObject> ParametricSearch(string name)
        {
            var linqResult =
                string.IsNullOrEmpty(name)
                    ? List()
                    : SelectLinq<AlarmTransmitter>(
                        alarmTransmitter => alarmTransmitter.Name.IndexOf(name) >= 0);

            return linqResult != null
                ? new LinkedList<AOrmObject>(
                    linqResult.OrderBy(alarmTransmitter => alarmTransmitter.Name).Cast<AOrmObject>())
                : new LinkedList<AOrmObject>();
        }

        public override bool ImplementsSearchFunctions()
        {
            return true;
        }

        #endregion

        public override ObjectType ObjectType
        {
            get { return ObjectType.AlarmTransmitter; }
        }

        protected override IList<AOrmObject> GetReferencedObjectsInternal(object idObj)
        {
            var alarmTransmitter = GetById(idObj);

            if (alarmTransmitter == null)
                return null;

            var referencedObjects = new List<AOrmObject>();

            if (alarmTransmitter.Ccus != null)
                referencedObjects.AddRange(
                    alarmTransmitter.Ccus.Cast<AOrmObject>());

            var presentationGroups = SelectLinq<PresentationGroup>(
                presentationGroup =>
                    presentationGroup.AlarmTransmitterId.Equals(alarmTransmitter.IdAlarmTransmitter));

            if (presentationGroups != null)
                referencedObjects.AddRange(
                    presentationGroups.Cast<AOrmObject>());

            return referencedObjects;
        }

        public AlarmTransmitter GetAlarmTransmitterForPresentationGroup(PresentationGroup presentationGroup)
        {
            if (presentationGroup == null
                || presentationGroup.AlarmTransmitterId == Guid.Empty)
            {
                return null;
            }

            return GetById(presentationGroup.AlarmTransmitterId);
        }

        public void AlarmTransmittersLookup(Guid clientId)
        {
            CCUConfigurationHandler.Singleton.AlarmTransmittersLookup(clientId);
        }

        public void CreateLookupedAlarmTransmitters(
            ICollection<string> ipAddresses,
            int? idStructuredSubSite)
        {
            if (ipAddresses == null)
                return;

            foreach (var ipAddress in ipAddresses)
            {
                var alarmTramsmitter = new AlarmTransmitter
                {
                    IpAddress = ipAddress,
                    Name = string.Format(
                        "{0} {1}",
                        NCASServer.Singleton.LocalizationHelper.GetString("TextAlarmTransmitter"),
                        ipAddress)
                };

                Insert(
                    ref alarmTramsmitter,
                    idStructuredSubSite);
            }
        }

        public void ChangeOnlineState(
            string ipAddress,
            OnlineState onlineState)
        {
            OnlineState oldOnlineState;

            if (_onlineStates.TryGetValue(
                    ipAddress,
                    out oldOnlineState)
                && oldOnlineState == onlineState)
            {
                return;
            }

            _onlineStates[ipAddress] = onlineState;

            NCASServerRemotingProvider.Singleton.ForeachCallbackHandler(
                RunAlarmTransmitterOnlineStateChanged,
                DelegateSequenceBlockingMode.Asynchronous,
                false,
                new object[]
                {
                    ipAddress,
                    onlineState
                });

            if (onlineState == OnlineState.Online)
                CatComClient.Singleton.RequestAllArcNames(IPAddress.Parse(ipAddress));
        }

        private void RunAlarmTransmitterOnlineStateChanged(ARemotingCallbackHandler remoteHandler, object[] info)
        {
            var alarmTransmitterOnlineStateChangedHandler =
                remoteHandler as AlarmTransmitterOnlineStateChangedHandler;

            if (alarmTransmitterOnlineStateChangedHandler == null)
                return;

            string ipAddress;
            OnlineState onlineState;

            try
            {
                ipAddress = (string)info[0];
                onlineState = (OnlineState) info[1];
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
                return;
            }

            alarmTransmitterOnlineStateChangedHandler.RunEvent(
                ipAddress,
                onlineState);
        }

        public OnlineState GetOnlineState(string ipAddress)
        {
            OnlineState onlineState;

            if (_onlineStates.TryGetValue(
                ipAddress,
                out onlineState))
            {
                return onlineState;
            }

            return OnlineState.Unknown;
        }

        public AlarmTransmitter GetAlarmTransmitterByIpAddress(string ipAddress)
        {
            var alarmTransmitters = SelectLinq<AlarmTransmitter>(
                alarmTransmitter =>
                    alarmTransmitter.IpAddress.Equals(ipAddress));

            if (alarmTransmitters == null)
                return null;

            return alarmTransmitters.FirstOrDefault();
        }

        public void AllArcNamesReceived(string alarmTransmitterIpAddress, IEnumerable<string> arcNames)
        {
            var alarmTransmitter = GetAlarmTransmitterByIpAddress(alarmTransmitterIpAddress);

            if (alarmTransmitter == null)
                return;

            var globalSiteInfo = StructuredSubSites.Singleton.GlobalEvaluator.GetGlobalSiteInfoOfObject(
                alarmTransmitter.IdAlarmTransmitter,
                ObjectType.AlarmTransmitter);

            AlarmArcs.Singleton.CreateNotExistsAlarmArcs(
                arcNames,
                globalSiteInfo == null
                || globalSiteInfo.Id == -1
                    ? null
                    : (int?) globalSiteInfo.Id);
        }
    }
}
