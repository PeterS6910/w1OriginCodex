using System.Collections.Generic;
using System.Linq;
using System.Text;

using Contal.Cgp.Globals;
using Contal.IwQuick.Data;

using CrSceneFrameworkCF;
using CrSceneFrameworkCF.Scenes;

namespace Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Menu.Common
{
    internal interface IAlarmAreasMenuItemsProvider : ICcuMenuItemsProvider
    {
        IAuthorizedSceneGroup SceneGroup
        {
            get;
        }

        IEnumerable<CrAlarmAreasManager.CrAlarmAreaInfo> AlarmAreaInfos
        {
            get;
        }
    }

    internal abstract class AAlarmAreasMenuItemsProviderBase<
            TMenuItemsProvider,
            TAlarmAreaAccessEventHandler,
            TAlarmAreasMenuSceneGroup> :
        ACrMenuSceneItemsProvider<TMenuItemsProvider>,
        IAlarmAreasMenuItemsProvider,
        IAlarmAreaAccessEventHandler
        where TMenuItemsProvider : AAlarmAreasMenuItemsProviderBase<
                TMenuItemsProvider,
                TAlarmAreaAccessEventHandler,
                TAlarmAreasMenuSceneGroup>
        where TAlarmAreaAccessEventHandler : class, IAlarmAreaAccessEventHandler
        where TAlarmAreasMenuSceneGroup : class, IAuthorizedSceneGroup
    {
        protected abstract class AlarmAreaMenuItemBase :
            ACcuMenuItem<TMenuItemsProvider>
        {
            protected readonly AAlarmAreaAccessInfoBase AccessInfo;

            protected abstract class ARouteProvider
                : IInstanceProvider<ACrSceneRoute, TMenuItemsProvider>
            {
                //TODO rename
                private readonly AAlarmAreaAccessInfoBase _alarmAreaAccessInfo;

                protected AAlarmAreaAccessInfoBase AlarmAreaAccessInfo
                {
                    get { return _alarmAreaAccessInfo; }
                }

                protected ARouteProvider(AAlarmAreaAccessInfoBase alarmAreaAccessInfo)
                {
                    _alarmAreaAccessInfo = alarmAreaAccessInfo;
                }

                public abstract ACrSceneRoute GetInstance(TMenuItemsProvider menuItemsProvider);
            }

            protected AlarmAreaMenuItemBase(
                AAlarmAreaAccessInfoBase alarmAreaAccessInfo,
                IInstanceProvider<ACrSceneRoute, TMenuItemsProvider> routeProvider)
                : base(routeProvider)
            {
                AccessInfo = alarmAreaAccessInfo;
            }

            protected abstract IEnumerable<CrIconSymbol> GetAdditionalInlinedIcons();

            protected sealed override IEnumerable<CrIconSymbol> GetInlinedIcons(TMenuItemsProvider menuItemsProvider)
            {
                if (menuItemsProvider.IsPremium)
                    yield return
                        AccessInfo.CrAlarmAreaInfo.IsUnset
                            ? CrIconSymbol.AlarmAreaIsUnset
                            : CrIconSymbol.AlarmAreaIsSet;

                foreach (var additionalInlinedIcon in GetAdditionalInlinedIcons())
                    yield return additionalInlinedIcon;
            }

            protected override string GetText(TMenuItemsProvider menuItemsProvider)
            {
                var alarmAreaInfo = AccessInfo.CrAlarmAreaInfo;

                var alarmAreaName = alarmAreaInfo.ToString();

                if (menuItemsProvider.IsPremium)
                    return alarmAreaName;

                var result = new StringBuilder();

                if (alarmAreaInfo.IsSet)
                    result.Append((char)177);

                if (AlarmArea.AlarmAreas.Singleton.GetAlarmAreaAlarmState(alarmAreaInfo.IdAlarmArea) == (byte)State.Alarm)
                    result.Append(" A ");

                if (alarmAreaInfo.IsUnset)
                    result.Append((char)176);

                result.Append(alarmAreaName);

                return result.ToString();
            }

            public override bool IsVisible(TMenuItemsProvider menuItemsProvider)
            {
                return menuItemsProvider.IsAlarmAreaVisible(AccessInfo);
            }
        }

        private readonly SyncDictionary<AAlarmAreaAccessInfoBase, AlarmAreaMenuItemBase> _alarmAreaItems =
            new SyncDictionary<AAlarmAreaAccessInfoBase, AlarmAreaMenuItemBase>();

        private ICollection<ACrMenuSceneItem<TMenuItemsProvider>> _generalItems;

        public IAuthorizedSceneGroup SceneGroup
        {
            get { return SceneGroupProvider.Instance; }
        }

        public IEnumerable<AAlarmAreaAccessInfoBase> AlarmAreaAccessInfos
        {
            get
            {
                return
                    _alarmAreaItems.KeysSnapshot
                        .Where(alarmAreaAccessInfo => alarmAreaAccessInfo.IsVisible);
            }
        }

        public IEnumerable<CrAlarmAreasManager.CrAlarmAreaInfo> AlarmAreaInfos
        {
            get
            {
                return
                    AlarmAreaAccessInfos
                        .Select(alarmAreaAccessInfo => alarmAreaAccessInfo.CrAlarmAreaInfo);
            }
        }

        protected AAlarmAreasMenuItemsProviderBase(
            IInstanceProvider<TAlarmAreasMenuSceneGroup> sceneGroupProvider,
            IInstanceProvider<CrMenuScene> menuSceneProvider)
            : base(menuSceneProvider)
        {
            SceneGroupProvider = sceneGroupProvider;
        }

        private bool? _isPremium;

        private bool IsPremium
        {
            get
            {
                if (!_isPremium.HasValue)
                    _isPremium = SceneGroupProvider.Instance.CardReaderSettings.IsPremium;

                return _isPremium.Value;
            }
        }

        private ACardReaderSettings _cardReaderSettings;

        protected readonly IInstanceProvider<TAlarmAreasMenuSceneGroup> SceneGroupProvider;

        protected abstract AAlarmAreaAccessManagerBase<TAlarmAreaAccessEventHandler> AlarmAreaAccessManager
        {
            get;
        }

        protected abstract TAlarmAreaAccessEventHandler ThisEventHandler
        {
            get;
        }

        protected override void OnExitedInternal()
        {
            AlarmAreaAccessManager.Detach(ThisEventHandler);
            _alarmAreaItems.Clear();
        }

        protected IEnumerable<CrAlarmAreasManager.CrAlarmAreaInfo> AlarmAreasToSet
        {
            get
            {
                return
                    AlarmAreaInfos
                        .Where(
                            alarmAreaInfo =>
                                alarmAreaInfo.IsUnset
                                && alarmAreaInfo.IsSettable)
                        .ToArray();
            }
        }

        protected IEnumerable<CrAlarmAreasManager.CrAlarmAreaInfo> AlarmAreasToUnset
        {
            get
            {
                return
                    AlarmAreaInfos
                        .Where(
                            alarmAreaInfo =>
                                alarmAreaInfo.IsSet
                                && alarmAreaInfo.IsUnsettable)
                        .ToArray();
            }
        }

        public ACardReaderSettings CardReaderSettings
        {
            get
            {
                return _cardReaderSettings
                       ?? (_cardReaderSettings =
                           SceneGroupProvider.Instance.CardReaderSettings);
            }
        }

        protected abstract AlarmAreaMenuItemBase CreateAlarmAreaMenuItem(AAlarmAreaAccessInfoBase alarmAreaAccessInfo);

        protected override IEnumerable<ACrMenuSceneItem<TMenuItemsProvider>> OnEnteredInternal()
        {
            var sortedAlarmAreaAccessInfos = AlarmAreaAccessManager.Attach(ThisEventHandler);

            _generalItems =
                new LinkedList<ACrMenuSceneItem<TMenuItemsProvider>>(
                    CreateGeneralItems());

            foreach (var crMenuSceneItem in _generalItems)
                yield return crMenuSceneItem;

            foreach (var alarmAreaAccessInfo in sortedAlarmAreaAccessInfos)
            {
                AlarmAreaMenuItemBase alarmAreaMenuItem;

                if (_alarmAreaItems.TryGetValue(
                    alarmAreaAccessInfo,
                    out alarmAreaMenuItem))
                {
                    yield return alarmAreaMenuItem;
                }
            }
        }

        protected virtual IEnumerable<ACrMenuSceneItem<TMenuItemsProvider>> CreateGeneralItems()
        {
            yield break;
        }

        protected void UpdateAlarmArea(AAlarmAreaAccessInfoBase alarmAreaAccessInfo)
        {
            AlarmAreaMenuItemBase alarmAreaMenuItem;

            if (!_alarmAreaItems.TryGetValue(
                alarmAreaAccessInfo,
                out alarmAreaMenuItem))
            {
                return;
            }

            var cardReaderMechanism =
                SceneGroupProvider.Instance.CardReaderSettings;

            UpdateItem(
                cardReaderMechanism.SceneContext,
                alarmAreaMenuItem);

            UpdateGeneralItems();
        }

        private void UpdateGeneralItems()
        {
            BeforeUpdateGeneralItems();

            var cardReaderMechanism = SceneGroupProvider.Instance.CardReaderSettings;

            foreach (var generalItem in _generalItems)
                UpdateItem(
                    cardReaderMechanism.SceneContext,
                    generalItem);
        }

        protected virtual void BeforeUpdateGeneralItems()
        {

        }

        public void OnAttached(IAlarmAreaAccessManager accessManager)
        {
            _alarmAreaItems.Clear();

            var sortedAlarmAreaInfos = accessManager.SortedAlarmAreaAccessInfos;

            foreach (var alarmAreaAccessInfo in sortedAlarmAreaInfos)
            {
                var alarmAreaMenuItem =
                    CreateAlarmAreaMenuItem(alarmAreaAccessInfo);

                _alarmAreaItems.Add(
                    alarmAreaAccessInfo,
                    alarmAreaMenuItem);
            }
        }

        public void OnAlarmAreaAdded(
            AAlarmAreaAccessInfoBase alarmAreaAccessInfo,
            AAlarmAreaAccessInfoBase predecessorAlarmAreaAccessInfo)
        {
            var cardReaderMechanism = SceneGroupProvider.Instance.CardReaderSettings;

            var alarmAreaMenuItem =
                CreateAlarmAreaMenuItem(alarmAreaAccessInfo);

            _alarmAreaItems.Add(
                alarmAreaAccessInfo,
                alarmAreaMenuItem);

            AlarmAreaMenuItemBase predecessorAlarmAreaMenuItem;

            if (predecessorAlarmAreaAccessInfo != null)
                _alarmAreaItems.TryGetValue(
                    predecessorAlarmAreaAccessInfo,
                    out predecessorAlarmAreaMenuItem);
            else
                predecessorAlarmAreaMenuItem = null;

            InsertItem(
                cardReaderMechanism.SceneContext,
                alarmAreaMenuItem,
                predecessorAlarmAreaMenuItem);

            UpdateGeneralItems();
        }

        public void OnAlarmAreaRemoved(AAlarmAreaAccessInfoBase alarmAreaAccessInfo)
        {
            _alarmAreaItems.Remove(
                alarmAreaAccessInfo,
                OnRemovePostProcessingLambda);

            UpdateGeneralItems();
        }

        private void OnRemovePostProcessingLambda(
            AAlarmAreaAccessInfoBase alarmAreaInfo,
            bool found,
            AlarmAreaMenuItemBase alarmAreaMenuItem)
        {
            if (found)
                RemoveItem(
                    SceneGroupProvider.Instance.CardReaderSettings.SceneContext,
                    alarmAreaMenuItem);
        }

        public void OnDetached()
        {
            _generalItems.Clear();

            _alarmAreaItems.Clear();
        }

        public void OnActivationStateChanged(AAlarmAreaAccessInfoBase alarmAreaAccessInfo)
        {
            UpdateAlarmArea(alarmAreaAccessInfo);
        }

        public void OnAlarmStateChanged(AAlarmAreaAccessInfoBase alarmAreaAccessInfo)
        {
            UpdateAlarmArea(alarmAreaAccessInfo);
        }

        public void OnNotAcknolwedgedStateChanged(AAlarmAreaAccessInfoBase alarmAreaAccessInfo)
        {
            UpdateAlarmArea(alarmAreaAccessInfo);
        }

        public void OnAACardReaderRightsChanged(AAlarmAreaAccessInfoBase alarmAreaAccessInfo)
        {
            UpdateAlarmArea(alarmAreaAccessInfo);
        }

        public virtual bool IsAlarmAreaVisible(AAlarmAreaAccessInfoBase accessInfo)
        {
            return accessInfo.IsVisible;
        }
    }
}
