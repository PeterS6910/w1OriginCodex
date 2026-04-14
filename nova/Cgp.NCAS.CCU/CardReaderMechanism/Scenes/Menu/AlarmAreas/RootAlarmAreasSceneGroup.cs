using System.Collections.Generic;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.Alarms;
using Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Menu.Common;
using Contal.Cgp.NCAS.CCU.EventParameters;

using CrSceneFrameworkCF;
using CrSceneFrameworkCF.Scenes;

namespace Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Menu.AlarmAreas
{
    internal class RootAlarmAreasSceneGroup :
        ARootAlarmAreasSceneGroup<RootAlarmAreasSceneGroup, IAlarmAreaAccessEventHandler>,
        IInstanceProvider<RootAlarmAreasSceneGroup>
    {
        private class AuthorizationProcessClass
            : AAuthorizationProcess
        {
            public AuthorizationProcessClass(IInstanceProvider<RootAlarmAreasSceneGroup> sceneGroupProvider)
                : base(sceneGroupProvider)
            {
            }

            protected override void OnAccessDeniedInvalidCode()
            {
                var idCardReader = CardReaderSettings.Id;

                if (BlockedAlarmsManager.Singleton.ProcessEvent(
                    AlarmType.CardReader_InvalidCode,
                    new IdAndObjectType(
                        idCardReader,
                        ObjectType.CardReader)))
                {
                    Events.ProcessEvent(
                        new EventCrAccessDeniedEnterToAaMenuInvalidCode(idCardReader));
                }

                AlarmsManager.Singleton.AddAlarm(
                    new CrInvalidCodeAlarm(idCardReader));
            }

            protected override void OnAccessDeniedInvalidPin()
            {
                var idCardReader = CardReaderSettings.Id;

                if (BlockedAlarmsManager.Singleton.ProcessEvent(
                    AlarmType.CardReader_InvalidPIN,
                    new IdAndObjectType(
                        idCardReader,
                        ObjectType.CardReader)))
                {
                    Events.ProcessEvent(
                        new EventCrAccessDeniedEnterToAaMenuInvalidPin(
                            idCardReader,
                            AccessData));
                }

                AlarmsManager.Singleton.AddAlarm(
                    new CrInvalidPinAlarm(
                        idCardReader,
                        AccessData.IdCard));
            }
        }

        private class MenuItemsProvider : 
            AlarmAreasMenuScene<
                RootAlarmAreasSceneGroup, 
                MenuItemsProvider>.AMenuItemsProvider
        {
            private class AlarmAreasInAlarmStateMenuItem : ALocalizedMenuItem<MenuItemsProvider>
            {
                private class RouteProvider : IInstanceProvider<ACrSceneRoute, MenuItemsProvider>
                {
                    public ACrSceneRoute GetInstance(MenuItemsProvider menuItemsProvider)
                    {
                        return
                            new AlarmAreasInAlarmStateSceneGroup(
                                menuItemsProvider.SceneGroupProvider.Instance,
                                menuItemsProvider.AlarmAreaAccessManager).EnterRoute;
                    }
                }

                public AlarmAreasInAlarmStateMenuItem()
                    : base(new RouteProvider())
                {
                }

                public override bool IsVisible(MenuItemsProvider menuItemsProvider)
                {
                    return menuItemsProvider.IsAnyAaInAlarmState;
                }

                protected override string GetLocalizationKey(MenuItemsProvider menuItemsProvider)
                {
                    return CardReaderConstants.MENUALARMAREASINALARMSTATE;
                }

                protected override IEnumerable<CrIconSymbol> GetInlinedIcons(MenuItemsProvider menuItemsProvider)
                {
                    yield return CrIconSymbol.AlarmAreasInAlarm;
                }
            }

            private class AlarmAreasNotAcknowledgedMenuItem : ALocalizedMenuItem<MenuItemsProvider>
            {
                private class RouteProvider : IInstanceProvider<ACrSceneRoute, MenuItemsProvider>
                {
                    public ACrSceneRoute GetInstance(MenuItemsProvider menuItemsProvider)
                    {
                        return new AlarmAreasNotAcknowledgedSceneGroup(
                            menuItemsProvider.SceneGroupProvider.Instance,
                            menuItemsProvider.AlarmAreaAccessManager).EnterRoute;
                    }
                }

                public AlarmAreasNotAcknowledgedMenuItem()
                    : base(new RouteProvider())
                {
                }

                public override bool IsVisible(MenuItemsProvider menuItemsProvider)
                {
                    return menuItemsProvider.IsAnyAaNotAcknowledged;
                }

                protected override string GetLocalizationKey(MenuItemsProvider menuItemsProvider)
                {
                    return CardReaderConstants.MENUALARMAREASNOTACKNOWLEDGED;
                }

                protected override IEnumerable<CrIconSymbol> GetInlinedIcons(MenuItemsProvider menuItemsProvider)
                {
                    yield return CrIconSymbol.AlarmAreasNotAcknowledged;
                }
            }

            public MenuItemsProvider(
                RootAlarmAreasSceneGroup sceneGroup,
                IInstanceProvider<CrMenuScene> menuSceneProvider)
                : base(
                    sceneGroup,
                    menuSceneProvider)
            {
            }

            protected override IEnumerable<ACrMenuSceneItem<MenuItemsProvider>> CreateAdditionalGeneralItems()
            {
                yield return new AlarmAreasInAlarmStateMenuItem();
                yield return new AlarmAreasNotAcknowledgedMenuItem();
            }

            protected override MenuItemsProvider This
            {
                get { return this; }
            }

            protected override AAlarmAreaAccessManagerBase<IAlarmAreaAccessEventHandler> AlarmAreaAccessManager
            {
                get { return SceneGroupProvider.Instance.AlarmAreaAccessManager; }
            }
        }

        public RootAlarmAreasSceneGroup(
            ACardReaderSettings cardReaderSettings)
            : this(
                new DelayedInstanceProvider(),
                cardReaderSettings)
        {
        }

        private RootAlarmAreasSceneGroup(
            DelayedInstanceProvider sceneGroupProvider,
            ACardReaderSettings cardReaderSettings)
            : base(
                new AlarmAreaAccessManager(sceneGroupProvider), 
                new AuthorizationProcessClass(sceneGroupProvider),
                CrSceneGroupReturnRoute.Default,
                cardReaderSettings)
        {
            sceneGroupProvider.Instance = this;
        }

        protected override ICrScene CreateAuthorizedSceneInternal()
        {
            var menuSceneProvider =
                new DelayedInitReference<CrMenuScene>();

            var result =
                new AlarmAreasMenuScene<RootAlarmAreasSceneGroup, MenuItemsProvider>(
                    this,
                    new MenuItemsProvider(
                        this,
                        menuSceneProvider));

            menuSceneProvider.Instance = result;

            return result;
        }

        RootAlarmAreasSceneGroup IInstanceProvider<RootAlarmAreasSceneGroup>.Instance
        {
            get { return this; }
        }

        protected override RootAlarmAreasSceneGroup This
        {
            get { return this; }
        }
    }
}
