using System.Collections.Generic;

using Contal.Cgp.NCAS.CCU.Alarms;
using Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Menu.Common;
using Contal.Drivers.CardReader;
using CrSceneFrameworkCF;
using CrSceneFrameworkCF.Scenes;

namespace Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Menu.AlarmAreas
{
    internal class AlarmAreaEditSceneGroup 
        : AAlarmAreaSceneGroupBase
    {
        private readonly AAlarmAreaAccessManagerBase<IAlarmAreaAccessEventHandler> _alarmAreaAccessManager;

        private readonly CrAlarmAreasManager.CrAlarmAreaInfo _crAlarmAreaInfo;

        private class SetAlarmAreaMenuItem : ASetAlarmAreaMenuItemBase<MenuItemsProvider>
        {
            private readonly bool _noPrewarning;

            public SetAlarmAreaMenuItem(bool noPrewarning)
                : base(
                    new SetAlarmAreaRouteProvider(
                        noPrewarning,
                        CrSceneGroupReturnRoute.Default))
            {
                _noPrewarning = noPrewarning;
            }

            protected override string GetLocalizationKey(MenuItemsProvider menuItemsProvider)
            {
                return
                    _noPrewarning
                        ? CardReaderConstants.MENUSETAANOPREWARNING
                        : CardReaderConstants.MENUSETAA;
            }

            public override bool IsVisible(MenuItemsProvider menuItemsProvider)
            {
                var crAlarmAreaInfo = menuItemsProvider.SceneGroup.CrAlarmAreaInfo;

                return crAlarmAreaInfo.IsUnset
                       && crAlarmAreaInfo.IsSettable
                       && (!_noPrewarning || menuItemsProvider.SetNoPrewarningEnabled);
            }
        }

        private class UnsetAlarmAreaMenuItem : ALocalizedMenuItem<MenuItemsProvider>
        {
            public UnsetAlarmAreaMenuItem()
                : this(new UnsetAlarmAreaRouteProvider(CrSceneGroupReturnRoute.Default))
            {
            }

            protected UnsetAlarmAreaMenuItem(UnsetAlarmAreaRouteProvider routeProvider)
                : base(new SetUnsetRouteProviderAdapter<MenuItemsProvider>(routeProvider))
            {
            }

            protected override string GetLocalizationKey(MenuItemsProvider menuItemsProvider)
            {
                return CardReaderConstants.MENUUNSETAA;
            }

            protected override IEnumerable<CrIconSymbol> GetInlinedIcons(MenuItemsProvider menuItemsProvider)
            {
                yield return CrIconSymbol.UnsetAlarmArea;
            }

            public override bool IsVisible(MenuItemsProvider menuItemsProvider)
            {
                var crAlarmAreaInfo = menuItemsProvider.SceneGroup.CrAlarmAreaInfo;

                return crAlarmAreaInfo.IsSet
                       && crAlarmAreaInfo.IsUnsettable;
            }
        }

        private class UnsetAndAcknowledgeAlarmAreaMenuItem : UnsetAlarmAreaMenuItem
        {
            private class RouteProvider : UnsetAlarmAreaRouteProvider
            {
                public RouteProvider()
                    : base(CrSceneGroupReturnRoute.Default)
                {
                }

                public override ACrSceneRoute GetInstance(
                    CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo, 
                    IAuthorizedSceneGroup parentSceneGroup)
                {
                    var alarmAreasMenuSceneGroup = parentSceneGroup;

                    AlarmsManager.Singleton.AcknowledgeAlarm(
                        AlarmAreaAlarm.CreateAlarmKey(crAlarmAreaInfo.IdAlarmArea),
                        new AlarmsManager.ActionFromCcuSources(
                            alarmAreasMenuSceneGroup.CardReaderSettings.Id,
                            alarmAreasMenuSceneGroup.AccessData));

                    return base.GetInstance(
                        crAlarmAreaInfo, 
                        parentSceneGroup);
                }
            }

            public UnsetAndAcknowledgeAlarmAreaMenuItem()
                : base(new RouteProvider())
            {
            }

            protected override string GetLocalizationKey(MenuItemsProvider menuItemsProvider)
            {
                return CardReaderConstants.MENUUNSETANDACKNOWLEDGEAA;
            }

            protected override IEnumerable<CrIconSymbol> GetInlinedIcons(MenuItemsProvider menuItemsProvider)
            {
                yield return CrIconSymbol.UnsetAlarmArea;
                yield return CrIconSymbol.AcknowledgeAlarmArea;
            }

            public override bool IsVisible(MenuItemsProvider menuItemsProvider)
            {
                var alarmAreaInfo = menuItemsProvider.SceneGroup.CrAlarmAreaInfo;

                return
                    alarmAreaInfo.IsSet
                    && alarmAreaInfo.IsUnsettable
                    && menuItemsProvider.AlarmNotAcknowledged;
            }
        }

        private class AcknowledgeAlarmAreaMenuItem : ALocalizedMenuItem<MenuItemsProvider>
        {
            private class RouteProvider : IInstanceProvider<ACrSceneRoute, MenuItemsProvider>
            {
                public ACrSceneRoute GetInstance(MenuItemsProvider menuItemsProvider)
                {
                    var sceneGroup = menuItemsProvider.SceneGroup;

                    AlarmsManager.Singleton.AcknowledgeAlarm(
                        AlarmAreaAlarm.CreateAlarmKey(sceneGroup.CrAlarmAreaInfo.IdAlarmArea),
                        new AlarmsManager.ActionFromCcuSources(
                            sceneGroup.CardReaderSettings.Id,
                            sceneGroup.AccessData));

                    return CrSceneGroupReturnRoute.Default;
                }
            }

            public AcknowledgeAlarmAreaMenuItem()
                : base(new RouteProvider())
            {
            }

            protected override string GetLocalizationKey(MenuItemsProvider menuItemsProvider)
            {
                return CardReaderConstants.MENUACKNOWLEDGEAA;
            }

            protected override IEnumerable<CrIconSymbol> GetInlinedIcons(MenuItemsProvider menuItemsProvider)
            {
                yield return CrIconSymbol.AlarmAreaIsNotAcknowledged;
                yield return CrIconSymbol.AcknowledgeAlarmArea;
            }

            public override bool IsVisible(MenuItemsProvider menuItemsProvider)
            {
                return
                    menuItemsProvider.AlarmNotAcknowledged;
            }
        }

        private class MenuItemsProvider :
            ACrMenuSceneItemsProvider<MenuItemsProvider>,
            IAlarmAreaAccessEventHandler,
            ICcuMenuItemsProvider,
            ISetUnsetAlarmAreaContext
        {
            public AlarmAreaEditSceneGroup SceneGroup
            {
                get;
                private set;
            }

            private readonly ACardReaderSettings _cardReaderSettings;

            private readonly ICollection<ACrMenuSceneItem<MenuItemsProvider>> _menuItems =
                new LinkedList<ACrMenuSceneItem<MenuItemsProvider>>();

            public MenuItemsProvider(
                AlarmAreaEditSceneGroup sceneGroup,
                IInstanceProvider<CrMenuScene> menuSceneProvider)
                : base(menuSceneProvider)
            {
                SceneGroup = sceneGroup;
                _cardReaderSettings = sceneGroup.CardReaderSettings;
            }

            protected override MenuItemsProvider This
            {
                get { return this; }
            }

            protected override IEnumerable<ACrMenuSceneItem<MenuItemsProvider>> OnEnteredInternal()
            {
                SceneGroup._alarmAreaAccessManager.Attach(this);

                _menuItems.Add(new SetAlarmAreaMenuItem(false));

                _menuItems.Add(new SetAlarmAreaMenuItem(true));

                _menuItems.Add(new UnsetAlarmAreaMenuItem());

                _menuItems.Add(new UnsetAndAcknowledgeAlarmAreaMenuItem());

                _menuItems.Add(new AcknowledgeAlarmAreaMenuItem());

                return _menuItems;
            }

            public ACardReaderSettings CardReaderSettings
            {
                get { return _cardReaderSettings; }
            }

            public void OnAttached(IAlarmAreaAccessManager accessManager)
            {
                EvaluateAlarmAreaState(SceneGroup.CrAlarmAreaInfo);
            }

            public void OnAlarmAreaAdded(
                AAlarmAreaAccessInfoBase alarmAreaAccessInfo,
                AAlarmAreaAccessInfoBase predecessorAlarmAreaAccessInfo)
            {
            }

            public void OnAlarmAreaRemoved(AAlarmAreaAccessInfoBase alarmAreaAccessInfo)
            {
            }

            private void UpdateAlarmArea(AAlarmAreaAccessInfoBase alarmAreaAccessInfo)
            {
                var alarmAreaInfo = alarmAreaAccessInfo.CrAlarmAreaInfo;

                if (!alarmAreaInfo.Equals(SceneGroup.CrAlarmAreaInfo))
                    return;

                EvaluateAlarmAreaState(alarmAreaInfo);

                foreach (var menuItem in _menuItems)
                    UpdateItem(
                        CardReaderSettings.SceneContext,
                        menuItem);
            }

            private void EvaluateAlarmAreaState(CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo)
            {
                var idAlarmArea = crAlarmAreaInfo.IdAlarmArea;

                var accessData = SceneGroup.AccessData;

                SetNoPrewarningEnabled =
                    crAlarmAreaInfo.AlarmArea.PreWarning
                    && (accessData.EntryViaCard
                        || accessData.EntryViaPersonalCode)
                    && AlarmAreaAccessRightsManager.Singleton.CheckRigthsToUnconditionalSet(
                        accessData,
                        idAlarmArea);

                AlarmNotAcknowledged =
                    AlarmArea.AlarmAreas.Singleton.AlarmAreaAlarmNotAcknowledged(idAlarmArea);
            }

            public void OnDetached()
            {
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

            public bool SetNoPrewarningEnabled
            {
                get;
                private set;
            }

            public bool AlarmNotAcknowledged
            {
                get;
                private set;
            }

            protected override void OnExitedInternal()
            {
                SceneGroup._alarmAreaAccessManager.Detach(this);
                _menuItems.Clear();
            }

            IAuthorizedSceneGroup ISetUnsetAlarmAreaContext.AuthorizedSceneGroup
            {
                get { return SceneGroup; }
            }

            CrAlarmAreasManager.CrAlarmAreaInfo ISetUnsetAlarmAreaContext.CrAlarmAreaInfo
            {
                get { return SceneGroup.CrAlarmAreaInfo; }
            }
        }

        private class Scene : CrMenuScene
        {
            private readonly CrDisplayProcessor _crDisplayProcessor;

            public Scene(AlarmAreaEditSceneGroup sceneGroup)
                : this(
                    sceneGroup,
                    new DelayedInitReference<CrMenuScene>())
            {
            }

            private Scene(
                AlarmAreaEditSceneGroup sceneGroup,
                DelayedInitReference<CrMenuScene> delayedInitReference)
                : base(
                    new MenuItemsProvider(sceneGroup, delayedInitReference),
                    MenuConfigurations.GetEditAlarmAreaMenuConfiguration(sceneGroup.CardReaderSettings.CardReader),
                    sceneGroup.DefaultGroupExitRoute,
                    sceneGroup.TimeOutGroupExitRoute)
            {
                delayedInitReference.Instance = this;
                _crDisplayProcessor = sceneGroup.CardReaderSettings.CrDisplayProcessor;
            }

            protected override void ShowNoMenuItems(CardReader cardReader)
            {
                _crDisplayProcessor.DisplayWriteText(
                    _crDisplayProcessor.GetLocalizationString(
                        "NoItemsToDisplay"),
                    0,
                    0);
            }
        }

        public AlarmAreaEditSceneGroup(
            CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo,
            IAuthorizedSceneGroup parentSceneGroup,
            AAlarmAreaAccessManagerBase<IAlarmAreaAccessEventHandler> alarmAreaAccessManager,
            IInstanceProvider<ACrSceneRoute> parentDefaultRouteProvider)
            : base(
                parentSceneGroup,
                parentDefaultRouteProvider)
        {
            _alarmAreaAccessManager = alarmAreaAccessManager;
            _crAlarmAreaInfo = crAlarmAreaInfo;
        }

        public override CrAlarmAreasManager.CrAlarmAreaInfo CrAlarmAreaInfo
        {
            get { return _crAlarmAreaInfo; }
        }

        protected override ICrScene CreateMenuScene()
        {
            return new Scene(this);
        }
    }
}
