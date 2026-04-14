using System;
using System.Collections.Generic;
using Contal.IwQuick;
using Contal.IwQuick.CrossPlatform.Data;
using Contal.IwQuick.Data;

using CrSceneFrameworkCF;

namespace Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Menu.Common
{
    internal interface IAlarmAreaAccessEventHandler
    {
        void OnAttached(IAlarmAreaAccessManager accessManager);

        void OnAlarmAreaAdded(
            AAlarmAreaAccessInfoBase alarmAreaAccessInfo,
            AAlarmAreaAccessInfoBase predecessorAlarmAreaAccessInfo);

        void OnAlarmAreaRemoved(AAlarmAreaAccessInfoBase alarmAreaAccessInfo);

        void OnDetached();

        void OnActivationStateChanged(AAlarmAreaAccessInfoBase alarmAreaAccessInfo);

        void OnAlarmStateChanged(AAlarmAreaAccessInfoBase alarmAreaAccessInfo);

        void OnNotAcknolwedgedStateChanged(AAlarmAreaAccessInfoBase alarmAreaAccessInfo);

        void OnAACardReaderRightsChanged(AAlarmAreaAccessInfoBase alarmAreaAccessInfo);
    }

    internal interface IAlarmAreaAccessManager
    {
        AccessDataBase AccessData
        {
            get;
        }

        ICollection<AAlarmAreaAccessInfoBase> SortedAlarmAreaAccessInfos
        {
            get;
        }

        bool CheckRigthsToSet(Guid idAlarmArea);
        bool CheckRigthsToUnset(Guid idAlarmArea);
        bool CheckRigthsToTimeBuying(Guid idAlarmArea);
        bool CheckRigthsToSensorHandling(Guid idAlarmArea);
    }

    internal abstract class AAlarmAreaAccessManagerBase<TAlarmAreaAccessEventHandler> :
        ICrAlarmAreaEventHandler,
        IAlarmAreaAccessManager
        where TAlarmAreaAccessEventHandler : class, IAlarmAreaAccessEventHandler
    {
        private readonly IInstanceProvider<IAuthorizedSceneGroup> _sceneGroupProvider;
        private Dictionary<Guid, AlarmAreaAccessRightsManager.CachedAaRights> _aasRightsForPersonCache;

        private class Comparer : IComparer<AAlarmAreaAccessInfoBase>
        {
            public int Compare(
                AAlarmAreaAccessInfoBase x,
                AAlarmAreaAccessInfoBase y)
            {
                return
                    string.Compare(
                        x.CrAlarmAreaInfo.ToString(),
                        y.CrAlarmAreaInfo.ToString(),
                        StringComparison.CurrentCulture);
            }
        }

        protected readonly SyncDictionary<CrAlarmAreasManager.CrAlarmAreaInfo, AAlarmAreaAccessInfoBase> AlarmAreaAccessInfos =
            new SyncDictionary<CrAlarmAreasManager.CrAlarmAreaInfo, AAlarmAreaAccessInfoBase>();

        private SimpleSortedList<AAlarmAreaAccessInfoBase> _sortedAccessInfos;

        private readonly IComparer<AAlarmAreaAccessInfoBase> _comparer =
            new Comparer();

        private readonly EventHandlerGroup<TAlarmAreaAccessEventHandler> _eventHandlerGroup =
            new EventHandlerGroup<TAlarmAreaAccessEventHandler>();

        protected AAlarmAreaAccessManagerBase(IInstanceProvider<IAuthorizedSceneGroup> sceneGroupProvider)
        {
            _sceneGroupProvider = sceneGroupProvider;
        }

        public void OnAttached(ICollection<CrAlarmAreasManager.CrAlarmAreaInfo> observedAlarmAreas)
        {
            _aasRightsForPersonCache =
                AlarmAreaAccessRightsManager.Singleton.GetAlarmAreasRightsCacheForPerson(AccessData.IdPerson);

            foreach (var alarmAreaInfo in observedAlarmAreas)
            {
                AAlarmAreaAccessInfoBase alarmAreaAccessInfo =
                    CreateAlarmAreaAccessInfo(alarmAreaInfo);

                AlarmAreaAccessInfos.Add(
                    alarmAreaInfo,
                    alarmAreaAccessInfo);
            }

            _aasRightsForPersonCache = null;

            _sortedAccessInfos =
                new SimpleSortedList<AAlarmAreaAccessInfoBase>(
                    _comparer,
                    AlarmAreaAccessInfos.ValuesSnapshot);

            _eventHandlerGroup.ForEach(eventHandler => eventHandler.OnAttached(this));
        }

        protected abstract AAlarmAreaAccessInfoBase CreateAlarmAreaAccessInfo(
            CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo);


        public ICollection<AAlarmAreaAccessInfoBase> Attach(TAlarmAreaAccessEventHandler eventHandler)
        {
            _eventHandlerGroup.Add(eventHandler);

            eventHandler.OnAttached(this);

            return _sortedAccessInfos;
        }

        public void Detach(TAlarmAreaAccessEventHandler eventHandler)
        {
            _eventHandlerGroup.Remove(eventHandler);
            eventHandler.OnDetached();
        }

        void ICrAlarmAreaEventHandler.OnActivationStateChanged(CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo)
        {
            AAlarmAreaAccessInfoBase alarmAreaAccessInfo;

            if (!AlarmAreaAccessInfos.TryGetValue(
                crAlarmAreaInfo,
                out alarmAreaAccessInfo))
            {
                return;
            }

            alarmAreaAccessInfo.UpdateVisibility(this);

            _eventHandlerGroup.ForEach(
                eventHandler => eventHandler.OnActivationStateChanged(alarmAreaAccessInfo));
        }

        void ICrAlarmAreaEventHandler.OnAlarmStateChanged(CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo)
        {
            AAlarmAreaAccessInfoBase alarmAreaAccessInfo;

            if (!AlarmAreaAccessInfos.TryGetValue(
                crAlarmAreaInfo,
                out alarmAreaAccessInfo))
            {
                return;
            }

            _eventHandlerGroup.ForEach(
                eventHandler => eventHandler.OnAlarmStateChanged(alarmAreaAccessInfo));
        }

        void ICrAlarmAreaEventHandler.OnNotAcknolwedgedStateChanged(
            CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo,
            bool notAcknowledged)
        {
            AAlarmAreaAccessInfoBase alarmAreaAccessInfo;

            if (!AlarmAreaAccessInfos.TryGetValue(
                crAlarmAreaInfo,
                out alarmAreaAccessInfo))
            {
                return;
            }

            _eventHandlerGroup.ForEach(
                eventHandler => eventHandler.OnNotAcknolwedgedStateChanged(alarmAreaAccessInfo));
        }

        void ICrAlarmAreaEventHandler.OnAlarmAreaAdded(CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo)
        {
            var alarmAreaAccessInfo =
                CreateAlarmAreaAccessInfo(crAlarmAreaInfo);

            AlarmAreaAccessInfos.Add(
                crAlarmAreaInfo,
                alarmAreaAccessInfo);

            var predecessor = 
                _sortedAccessInfos.AddAndGetPredecessor(alarmAreaAccessInfo);

            _eventHandlerGroup.ForEach(
                eventHandler => eventHandler.OnAlarmAreaAdded(
                    alarmAreaAccessInfo, 
                    predecessor));
        }

        void ICrAlarmAreaEventHandler.OnAlarmAreaRemoved(CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo)
        {
            AlarmAreaAccessInfos.Remove(
                crAlarmAreaInfo,
                OnAlarmAreaRemovedPostProcess);
        }

        private void OnAlarmAreaRemovedPostProcess(
            CrAlarmAreasManager.CrAlarmAreaInfo key,
            bool found,
            AAlarmAreaAccessInfoBase value)
        {
            if (!found)
                return;

            _sortedAccessInfos.Remove(value);

            _eventHandlerGroup.ForEach(
                eventHandler => eventHandler.OnAlarmAreaRemoved(value));
        }

        void ICrAlarmAreaEventHandler.OnAACardReaderRightsChanged(CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo)
        {
            AAlarmAreaAccessInfoBase alarmAreaAccessInfo;

            if (!AlarmAreaAccessInfos.TryGetValue(
                crAlarmAreaInfo,
                out alarmAreaAccessInfo))
            {
                return;
            }

            alarmAreaAccessInfo.UpdateVisibility(this);

            _eventHandlerGroup.ForEach(
                eventHandler => eventHandler.OnAACardReaderRightsChanged(alarmAreaAccessInfo));
        }

        void ICrAlarmAreaEventHandler.OnDetached()
        {
            AlarmAreaAccessInfos.Clear();
            if (_sortedAccessInfos != null)
                _sortedAccessInfos.Clear();
        }

        void ICrAlarmAreaEventHandler.OnAlarmAreaMarkingChanged(CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo)
        {
        }

        public virtual void OnAnySensorInAlarmChanged(
            CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo,
            bool value)
        {
        }

        public virtual void OnAnySensorInTamperChanged(
            CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo,
            bool value)
        {
        }

        public virtual void OnAnySensorNotAcknowledgedChanged(
            CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo,
            bool value)
        {
        }

        public virtual void OnAnySensorTemporarilyBlockedChanged(
            CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo,
            bool value)
        {
        }

        public virtual void OnAnySensorPermanentlyBlockedChanged(
            CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo,
            bool value)
        {
        }

        public AccessDataBase AccessData
        {
            get { return _sceneGroupProvider.Instance.AccessData; }
        }

        public EventHandlerGroup<TAlarmAreaAccessEventHandler> EventHandlerGroup
        {
            get { return _eventHandlerGroup; }
        }      

        public ICollection<AAlarmAreaAccessInfoBase> SortedAlarmAreaAccessInfos
        {
            get { return _sortedAccessInfos; }
        }

        public bool CheckRigthsToSet(Guid idAlarmArea)
        {
            if (_aasRightsForPersonCache == null)
                return AlarmAreaAccessRightsManager.Singleton.CheckRigthsToSet(
                    AccessData,
                    idAlarmArea);

            AlarmAreaAccessRightsManager.CachedAaRights cachedAaRights;

            return 
                _aasRightsForPersonCache.TryGetValue(
                    idAlarmArea, 
                    out cachedAaRights) 
                && cachedAaRights.AlarmAreaSet;
        }

        public bool CheckRigthsToUnset(Guid idAlarmArea)
        {
            if (_aasRightsForPersonCache == null)
                return AlarmAreaAccessRightsManager.Singleton.CheckRigthsToUnset(
                    AccessData,
                    idAlarmArea);

            AlarmAreaAccessRightsManager.CachedAaRights cachedAaRights;

            return
                _aasRightsForPersonCache.TryGetValue(
                    idAlarmArea,
                    out cachedAaRights) 
                && cachedAaRights.AlarmAreaUnset;
        }

        public bool CheckRigthsToTimeBuying(Guid idAlarmArea)
        {
            if (_aasRightsForPersonCache == null)
                return AlarmAreaAccessRightsManager.Singleton.CheckRigthsToTimeBuying(
                    AccessData,
                    idAlarmArea);

            AlarmAreaAccessRightsManager.CachedAaRights cachedAaRights;

            return 
                _aasRightsForPersonCache.TryGetValue(
                    idAlarmArea, 
                    out cachedAaRights) 
                && cachedAaRights.AlarmAreaTimeBuying;
        }

        public bool CheckRigthsToSensorHandling(Guid idAlarmArea)
        {
            if (_aasRightsForPersonCache == null)
                return AlarmAreaAccessRightsManager.Singleton.CheckRigthsToSensorHandling(
                    AccessData,
                    idAlarmArea);

            AlarmAreaAccessRightsManager.CachedAaRights cachedAaRights;

            return 
                _aasRightsForPersonCache.TryGetValue(
                    idAlarmArea, 
                    out cachedAaRights) 
                && cachedAaRights.SensorHandling;
        }
    }
}