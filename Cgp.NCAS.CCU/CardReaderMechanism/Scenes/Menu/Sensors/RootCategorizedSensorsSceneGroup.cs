using System;
using System.Collections.Generic;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.Alarms;
using Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Menu.Common;
using Contal.Cgp.NCAS.CCU.EventParameters;
using Contal.IwQuick.Data;

using CrSceneFrameworkCF;
using CrSceneFrameworkCF.Scenes;

namespace Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Menu.Sensors
{
    internal class RootCategorizedSensorsSceneGroup :
        ARootAlarmAreasSceneGroup<
            RootCategorizedSensorsSceneGroup,
            CategorizedSensorsAccessManager.IEventHandler>,
        IInstanceProvider<RootCategorizedSensorsSceneGroup>
    {
        private class MenuItemsProvider :
            CategorizedSensorsMenuScene<RootCategorizedSensorsSceneGroup, MenuItemsProvider>.AMenuItemsProvider
        {
            private new class AlarmAreaMenuItem :
                CategorizedSensorsMenuScene<RootCategorizedSensorsSceneGroup, MenuItemsProvider>.AMenuItemsProvider.AlarmAreaMenuItem
            {
                private class RouteProvider : ARouteProvider
                {
                    public RouteProvider(
                        AAlarmAreaAccessInfoBase alarmAreaAccessInfo)
                        : base(alarmAreaAccessInfo)
                    {
                    }

                    public override ACrSceneRoute GetInstance(MenuItemsProvider menuItemsProvider)
                    {
                        var rootCategorizedSensorsMenuSceneGroup = menuItemsProvider.SceneGroupProvider.Instance;
                        return
                            new AllSensorsForAlarmAreaSceneGroup(
                                AlarmAreaAccessInfo.CrAlarmAreaInfo,
                                rootCategorizedSensorsMenuSceneGroup).EnterRoute;
                    }
                }

                public AlarmAreaMenuItem(
                    AAlarmAreaAccessInfoBase accessInfo)
                    : base(
                        accessInfo,
                        new RouteProvider(accessInfo))
                {
                }
            }

            private class CategorizedSensorsInAlarmMenuItem : ALocalizedMenuItem<MenuItemsProvider>
            {
                private class RouteProvider : IInstanceProvider<ACrSceneRoute, MenuItemsProvider>
                {
                    public ACrSceneRoute GetInstance(MenuItemsProvider menuItemsProvider)
                    {
                        return
                            new CategorizedSensorsInAlarmSceneGroup(
                                menuItemsProvider.SceneGroupProvider.Instance,
                                menuItemsProvider.AlarmAreaAccessManager).EnterRoute;
                    }
                }

                public CategorizedSensorsInAlarmMenuItem()
                    : base(new RouteProvider())
                {
                }

                protected override string GetLocalizationKey(MenuItemsProvider menuItemsProvider)
                {
                    return CardReaderConstants.MENUSENSORSINALARM;
                }

                protected override IEnumerable<CrIconSymbol> GetInlinedIcons(MenuItemsProvider menuItemsProvider)
                {
                    yield return CrIconSymbol.SensorsInAlarm;
                }

                public override bool IsVisible(MenuItemsProvider menuItemsProvider)
                {
                    return menuItemsProvider._alarmAreasWithAnySensorsInAlarm.Count > 0;
                }
            }

            private class GlobalAcknowledgeAllMenuItem : GlobalAcknowledgeAllMenuItemBase<MenuItemsProvider>
            {
                public override bool IsVisible(MenuItemsProvider menuItemsProvider)
                {
                    return menuItemsProvider._alarmAreasWithAnySensorsNotAcknowledged.Count > 0;
                }
            }

            private class CategorizedSensorsNotAcknowledgedMenuItem : ALocalizedMenuItem<MenuItemsProvider>
            {
                private class RouteProvider
                    : IInstanceProvider<ACrSceneRoute, MenuItemsProvider>
                {
                    public ACrSceneRoute GetInstance(MenuItemsProvider menuItemsProvider)
                    {
                        return new CategorizedSensorsNotAcknowledgedSceneGroup(
                            menuItemsProvider.SceneGroupProvider.Instance,
                            menuItemsProvider.AlarmAreaAccessManager).EnterRoute;
                    }
                }

                public CategorizedSensorsNotAcknowledgedMenuItem()
                    : base(new RouteProvider())
                {
                }

                protected override string GetLocalizationKey(MenuItemsProvider menuItemsProvider)
                {
                    return CardReaderConstants.MENUSENSORSNOTACKNOWLEDGED;
                }

                protected override IEnumerable<CrIconSymbol> GetInlinedIcons(MenuItemsProvider menuItemsProvider)
                {
                    yield return CrIconSymbol.SensorsNormalNotAcknowledged;
                }

                public override bool IsVisible(MenuItemsProvider menuItemsProvider)
                {
                    return menuItemsProvider._alarmAreasWithAnySensorsNotAcknowledged.Count > 0;
                }
            }

            private class CategorizedSensorsTemporarilyBlockedMenuItem : ALocalizedMenuItem<MenuItemsProvider>
            {
                private class RouteProvider
                    : IInstanceProvider<ACrSceneRoute, MenuItemsProvider>
                {
                    public ACrSceneRoute GetInstance(MenuItemsProvider menuItemsProvider)
                    {
                        return new CategorizedSensorsTemporarilyBlockedSceneGroup(
                            menuItemsProvider.SceneGroupProvider.Instance,
                            menuItemsProvider.AlarmAreaAccessManager).EnterRoute;
                    }
                }

                public CategorizedSensorsTemporarilyBlockedMenuItem()
                    : base(new RouteProvider())
                {
                }

                protected override string GetLocalizationKey(MenuItemsProvider menuItemsProvider)
                {
                    return CardReaderConstants.MENUSENSORSTEMPORARILYBLOCKED;
                }

                protected override IEnumerable<CrIconSymbol> GetInlinedIcons(MenuItemsProvider menuItemsProvider)
                {
                    yield return CrIconSymbol.SensorsTemporarilyBlocked;
                }

                public override bool IsVisible(MenuItemsProvider menuItemsProvider)
                {
                    return menuItemsProvider._alarmAreasWithAnySensorsTemporarilyBlocked.Count > 0;
                }
            }

            private class CategorizedSensorsPermanentlyBlockedMenuItem : ALocalizedMenuItem<MenuItemsProvider>
            {
                private class RouteProvider
                    : IInstanceProvider<ACrSceneRoute, MenuItemsProvider>
                {
                    public ACrSceneRoute GetInstance(MenuItemsProvider menuItemsProvider)
                    {
                        return new CategorizedSensorsPermanentlyBlockedSceneGroup(
                            menuItemsProvider.SceneGroupProvider.Instance,
                            menuItemsProvider.AlarmAreaAccessManager).EnterRoute;
                    }
                }

                public CategorizedSensorsPermanentlyBlockedMenuItem()
                    : base(new RouteProvider())
                {
                }

                protected override string GetLocalizationKey(MenuItemsProvider menuItemsProvider)
                {
                    return CardReaderConstants.MENUSENSORSPERMANENTLYBLOCKED;
                }

                protected override IEnumerable<CrIconSymbol> GetInlinedIcons(MenuItemsProvider menuItemsProvider)
                {
                    yield return CrIconSymbol.SensorsPermanentlyBlocked;
                }

                public override bool IsVisible(MenuItemsProvider menuItemsProvider)
                {
                    return menuItemsProvider._alarmAreasWithAnySensorsPermanentlyBlocked.Count > 0;
                }
            }

            private class CategorizedSensorsInSabotageMenuItem : ALocalizedMenuItem<MenuItemsProvider>
            {
                private class RouteProvider
                    : IInstanceProvider<ACrSceneRoute, MenuItemsProvider>
                {
                    public ACrSceneRoute GetInstance(MenuItemsProvider menuItemsProvider)
                    {
                        return new CategorizedSensorsInSabotageSceneGroup(
                            menuItemsProvider.SceneGroupProvider.Instance,
                            menuItemsProvider.AlarmAreaAccessManager).EnterRoute;
                    }
                }

                public CategorizedSensorsInSabotageMenuItem()
                    : base(new RouteProvider())
                {
                }

                protected override string GetLocalizationKey(MenuItemsProvider menuItemsProvider)
                {
                    return CardReaderConstants.MENUSENSORSINSABOTAGE;
                }

                protected override IEnumerable<CrIconSymbol> GetInlinedIcons(MenuItemsProvider menuItemsProvider)
                {
                    yield return CrIconSymbol.SensorsInSabotage;
                }

                public override bool IsVisible(MenuItemsProvider menuItemsProvider)
                {
                    return menuItemsProvider._alarmAreasWithAnySensorsInTamper.Count > 0;
                }
            }

            public MenuItemsProvider(
                RootCategorizedSensorsSceneGroup rootCategorizedSensorsSceneGroup,
                IInstanceProvider<CrMenuScene> menuSceneProvider)
                : base(
                    rootCategorizedSensorsSceneGroup,
                    menuSceneProvider)
            {
            }

            protected override AAlarmAreaAccessManagerBase<CategorizedSensorsAccessManager.IEventHandler> AlarmAreaAccessManager
            {
                get { return SceneGroupProvider.Instance.AlarmAreaAccessManager; }
            }

            protected override CategorizedSensorsAccessManager.IEventHandler ThisEventHandler
            {
                get { return this; }
            }

            protected override AlarmAreaMenuItemBase CreateAlarmAreaMenuItem(AAlarmAreaAccessInfoBase alarmAreaInfo)
            {
                return new AlarmAreaMenuItem(alarmAreaInfo);
            }

            private readonly HashSet<Guid> _alarmAreasWithAnySensorsNotAcknowledged = new HashSet<Guid>();
            private readonly HashSet<Guid> _alarmAreasWithAnySensorsInAlarm = new HashSet<Guid>();
            private readonly HashSet<Guid> _alarmAreasWithAnySensorsTemporarilyBlocked = new HashSet<Guid>();
            private readonly HashSet<Guid> _alarmAreasWithAnySensorsPermanentlyBlocked = new HashSet<Guid>();
            private readonly HashSet<Guid> _alarmAreasWithAnySensorsInTamper = new HashSet<Guid>();

            protected override IEnumerable<ACrMenuSceneItem<MenuItemsProvider>> CreateGeneralItems()
            {
                foreach (var alarmAreaAccessInfo in AlarmAreaAccessInfos)
                {
                    var crAlarmAreaInfo = alarmAreaAccessInfo.CrAlarmAreaInfo;

                    var idAlarmArea = crAlarmAreaInfo.IdAlarmArea;

                    if (crAlarmAreaInfo.IsAnySensorInAlarm)
                        _alarmAreasWithAnySensorsInAlarm.Add(idAlarmArea);

                    if (crAlarmAreaInfo.IsInSabotage)
                        _alarmAreasWithAnySensorsInTamper.Add(idAlarmArea);

                    if (crAlarmAreaInfo.IsAnySensorNotAcknowledged)
                        _alarmAreasWithAnySensorsNotAcknowledged.Add(idAlarmArea);

                    if (crAlarmAreaInfo.IsAnySensorPermanentlyBlocked)
                        _alarmAreasWithAnySensorsPermanentlyBlocked.Add(idAlarmArea);

                    if (crAlarmAreaInfo.IsAnySensorTemporarilyBlocked)
                        _alarmAreasWithAnySensorsTemporarilyBlocked.Add(idAlarmArea);
                }

                yield return new GlobalAcknowledgeAllMenuItem();
                yield return new CategorizedSensorsInAlarmMenuItem();
                yield return new CategorizedSensorsNotAcknowledgedMenuItem();
                yield return new CategorizedSensorsTemporarilyBlockedMenuItem();
                yield return new CategorizedSensorsPermanentlyBlockedMenuItem();
                yield return new CategorizedSensorsInSabotageMenuItem();
            }

            protected override void BeforeUpdateGeneralItems()
            {
            }

            public override void OnAnySensorInAlarmChanged(
                AAlarmAreaAccessInfoBase alarmAreaAccessInfo,
                bool value)
            {
                var idAlarmArea = alarmAreaAccessInfo.CrAlarmAreaInfo.IdAlarmArea;

                if (value)
                    _alarmAreasWithAnySensorsInAlarm.Add(idAlarmArea);
                else
                    _alarmAreasWithAnySensorsInAlarm.Remove(idAlarmArea);

                base.OnAnySensorInAlarmChanged(
                    alarmAreaAccessInfo,
                    value);
            }

            public override void OnAnySensorInTamperChanged(
                AAlarmAreaAccessInfoBase alarmAreaAccessInfo,
                bool value)
            {
                var idAlarmArea = alarmAreaAccessInfo.CrAlarmAreaInfo.IdAlarmArea;

                if (value)
                    _alarmAreasWithAnySensorsInTamper.Add(idAlarmArea);
                else
                    _alarmAreasWithAnySensorsInTamper.Remove(idAlarmArea);

                base.OnAnySensorInTamperChanged(
                    alarmAreaAccessInfo,
                    value);
            }

            public override void OnAnySensorNotAcknowledgedChanged(
                AAlarmAreaAccessInfoBase alarmAreaAccessInfo,
                bool value)
            {
                var idAlarmArea = alarmAreaAccessInfo.CrAlarmAreaInfo.IdAlarmArea;

                if (value)
                    _alarmAreasWithAnySensorsNotAcknowledged.Add(idAlarmArea);
                else
                    _alarmAreasWithAnySensorsNotAcknowledged.Remove(idAlarmArea);

                base.OnAnySensorNotAcknowledgedChanged(
                    alarmAreaAccessInfo,
                    value);
            }

            public override void OnAnySensorPermanentlyBlockedChanged(
                AAlarmAreaAccessInfoBase alarmAreaAccessInfo,
                bool value)
            {
                var idAlarmArea = alarmAreaAccessInfo.CrAlarmAreaInfo.IdAlarmArea;

                if (value)
                    _alarmAreasWithAnySensorsPermanentlyBlocked.Add(idAlarmArea);
                else
                    _alarmAreasWithAnySensorsPermanentlyBlocked.Remove(idAlarmArea);

                base.OnAnySensorPermanentlyBlockedChanged(
                    alarmAreaAccessInfo,
                    value);
            }

            public override void OnAnySensorTemporarilyBlockedChanged(
                AAlarmAreaAccessInfoBase alarmAreaAccessInfo,
                bool value)
            {
                var idAlarmArea = alarmAreaAccessInfo.CrAlarmAreaInfo.IdAlarmArea;

                if (value)
                    _alarmAreasWithAnySensorsTemporarilyBlocked.Add(idAlarmArea);
                else
                    _alarmAreasWithAnySensorsTemporarilyBlocked.Remove(idAlarmArea);

                base.OnAnySensorTemporarilyBlockedChanged(
                    alarmAreaAccessInfo,
                    value);
            }

            protected override MenuItemsProvider This
            {
                get { return this; }
            }
        }

        private class AuthorizationProcessClass : AAuthorizationProcess
        {
            public AuthorizationProcessClass(IInstanceProvider<RootCategorizedSensorsSceneGroup> sceneGroupProvider)
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
                        new EventCrAccessDeniedEnterToSensorsMenuInvalidCode(idCardReader));
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
                        new EventCrAccessDeniedEnterToSensorsMenuInvalidPin(
                            idCardReader,
                            AccessData));
                }

                AlarmsManager.Singleton.AddAlarm(
                    new CrInvalidPinAlarm(
                        idCardReader,
                        AccessData.IdCard));
            }
        }

        public RootCategorizedSensorsSceneGroup(
            ACardReaderSettings cardReaderSettings)
            : this(
                new DelayedInstanceProvider(),
                cardReaderSettings)
        {
        }

        private RootCategorizedSensorsSceneGroup(
            DelayedInstanceProvider delayedInitReference,
            ACardReaderSettings cardReaderSettings)
            : base(
                new CategorizedSensorsAccessManager(delayedInitReference),
                new AuthorizationProcessClass(delayedInitReference),
                CrSceneGroupReturnRoute.Default,
                cardReaderSettings)
        {
            delayedInitReference.Instance = this;
        }

        protected override ICrScene CreateAuthorizedSceneInternal()
        {
            var sceneProvider = new DelayedInitReference<CrMenuScene>();

            var result =
                new CategorizedSensorsMenuScene<RootCategorizedSensorsSceneGroup, MenuItemsProvider>(
                    this,
                    new MenuItemsProvider(
                        this,
                        sceneProvider));

            sceneProvider.Instance = result;

            return result;
        }

        RootCategorizedSensorsSceneGroup IInstanceProvider<RootCategorizedSensorsSceneGroup>.Instance
        {
            get { return this; }
        }

        protected override RootCategorizedSensorsSceneGroup This
        {
            get { return this; }
        }
    }
}