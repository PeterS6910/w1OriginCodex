using System;
using System.Collections.Generic;

using Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Menu.Common;
using Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Menu.Sensors;
using Contal.Cgp.NCAS.CCU.EventParameters;

using CrSceneFrameworkCF;
using CrSceneFrameworkCF.Scenes;

namespace Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Menu.AlarmAreas
{
    internal class UnconditionalSetAlarmAreaSceneGroup :
        CrSimpleSceneGroup,
        IAuthorizedSceneGroup
    {
        private class Scene : CrMenuScene
        {
            private class MenuItemsProvider :
                ACrMenuSceneItemsProvider<MenuItemsProvider>,
                ICcuMenuItemsProvider,
                ISetUnsetAlarmAreaContext
            {
                private readonly UnconditionalSetAlarmAreaSceneGroup _sceneGroup;

                private class ShowSensorsMenuItem : ALocalizedMenuItem<MenuItemsProvider>
                {
                    private class RouteProvider : IInstanceProvider<ACrSceneRoute, MenuItemsProvider>
                    {
                        public ACrSceneRoute GetInstance(
                            MenuItemsProvider menuItemsProvider)
                        {
                            var sceneGroup = menuItemsProvider._sceneGroup;

                            var crAlarmAreaInfo = sceneGroup._crAlarmAreaInfo;

                            return new SensorsInAlarmOrSabotageForAlarmAreaSceneGroup(
                                new AlarmAreaSensorListener(crAlarmAreaInfo),
                                sceneGroup).EnterRoute;
                        }
                    }

                    public ShowSensorsMenuItem()
                        : base(new RouteProvider())
                    {
                    }

                    protected override string GetLocalizationKey(MenuItemsProvider menuItemsProvider)
                    {
                        return CardReaderConstants.MENUSHOWSENSORS;
                    }

                    protected override IEnumerable<CrIconSymbol> GetInlinedIcons(MenuItemsProvider menuItemsProvider)
                    {
                        yield return CrIconSymbol.ShowSensors;
                    }
                }

                private class UnconditionalSetMenuItem : ASetAlarmAreaMenuItemBase<MenuItemsProvider>
                {
                    private class RouteProvider : ASetAlarmAreaRouteProviderBase
                    {
                        public RouteProvider(
                            bool noPrewarning,
                            IInstanceProvider<ACrSceneRoute> defaultRouteProvider)
                            : base(
                                noPrewarning,
                                defaultRouteProvider)
                        {
                        }

                        protected override ACrSceneRoute TryRecoverFromFailure(
                            CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo,
                            IAuthorizedSceneGroup authorizedSceneGroup)
                        {
                            return new UnconditionalSetAlarmAreaFailedSceneGroup(
                                crAlarmAreaInfo,
                                authorizedSceneGroup.CardReaderSettings.CrDisplayProcessor,
                                DefaultRouteProvider).EnterRoute;
                        }

                        protected override ACrSceneRoute TrySetAlarmArea(
                            CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo,
                            IAuthorizedSceneGroup authorizedSceneGroup)
                        {
                            var idAlarmArea = crAlarmAreaInfo.IdAlarmArea;

                            var cardReaderMechanism = authorizedSceneGroup.CardReaderSettings;

                            var idCardReader = cardReaderMechanism.Id;

                            var accessData = authorizedSceneGroup.AccessData;

                            if (!AlarmArea.AlarmAreas.Singleton.SetAlarmArea(
                                idAlarmArea,
                                false,
                                new AlarmArea.AlarmAreas.SetUnsetParams(
                                    idCardReader,
                                    accessData,
                                    Guid.Empty,
                                    true,
                                    NoPrewarning)))
                            {
                                return null;
                            }

                            Events.ProcessEvent(
                                new EventSetAlarmAreaFromCardReader(
                                    idCardReader,
                                    accessData,
                                    idAlarmArea));

                            return new UnconditionalSetAlarmAreaSucceededSceneGroup(
                                crAlarmAreaInfo,
                                cardReaderMechanism.CrDisplayProcessor,
                                DefaultRouteProvider).EnterRoute;
                        }

                        protected override bool CheckRightsForThisAction(
                            CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo,
                            AccessDataBase accessData,
                            ACardReaderSettings cardReaderSettings)
                        {
                            if (!crAlarmAreaInfo.IsUnconditionalSettable)
                            {
                                return false;
                            }

                            return
                                accessData.IdPerson == Guid.Empty
                                || AlarmAreaAccessRightsManager.Singleton.CheckRigthsToUnconditionalSet(
                                    accessData,
                                    crAlarmAreaInfo.IdAlarmArea);
                        }
                    }

                    public UnconditionalSetMenuItem(
                        bool noPrewarning,
                        IInstanceProvider<ACrSceneRoute> defaultRouteProvider)
                        : base(
                            new RouteProvider(
                                noPrewarning,
                                defaultRouteProvider))
                    {
                    }

                    protected override string GetLocalizationKey(MenuItemsProvider menuItemsProvider)
                    {
                        return CardReaderConstants.MENUUNCONDINTIONALSET;
                    }
                }

                public MenuItemsProvider(
                    UnconditionalSetAlarmAreaSceneGroup sceneGroup,
                    IInstanceProvider<CrMenuScene> menuSceneProvider)
                    : base(menuSceneProvider)
                {
                    _sceneGroup = sceneGroup;
                }

                protected override MenuItemsProvider This
                {
                    get { return this; }
                }

                protected override IEnumerable<ACrMenuSceneItem<MenuItemsProvider>> OnEnteredInternal()
                {
                    IList<ACrMenuSceneItem<MenuItemsProvider>> result = new List<ACrMenuSceneItem<MenuItemsProvider>>();

                    result.Add(
                        new ShowSensorsMenuItem());

                    result.Add(
                        new UnconditionalSetMenuItem(
                            _sceneGroup._noPrewarning,
                            _sceneGroup.DefaultGroupExitRoute));

                    return result;
                }

                public ACardReaderSettings CardReaderSettings
                {
                    get { return _sceneGroup.CardReaderSettings; }
                }

                public IAuthorizedSceneGroup AuthorizedSceneGroup
                {
                    get { return _sceneGroup; }
                }

                public CrAlarmAreasManager.CrAlarmAreaInfo CrAlarmAreaInfo
                {
                    get { return _sceneGroup._crAlarmAreaInfo; }
                }
            }


            public Scene(UnconditionalSetAlarmAreaSceneGroup sceneGroup)
                : this(
                    sceneGroup,
                    new DelayedInitReference<CrMenuScene>())
            {
            }

            private Scene(
                UnconditionalSetAlarmAreaSceneGroup sceneGroup,
                DelayedInitReference<CrMenuScene> delayedInitReference)
                : base(
                    new MenuItemsProvider(
                        sceneGroup,
                        delayedInitReference),
                    MenuConfigurations.GetUnconditionalSetMenuConfiguration(sceneGroup.CardReaderSettings.CardReader),
                    sceneGroup.DefaultGroupExitRoute,
                    sceneGroup.TimeOutGroupExitRoute)
            {
                delayedInitReference.Instance = this;
            }
        }

        private readonly CrAlarmAreasManager.CrAlarmAreaInfo _crAlarmAreaInfo;
        private readonly IAuthorizedSceneGroup _parentSceneGroup;

        private readonly bool _noPrewarning;

        public UnconditionalSetAlarmAreaSceneGroup(
            CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo,
            IAuthorizedSceneGroup parentSceneGroup,
            bool noPrewarning,
            IInstanceProvider<ACrSceneRoute> parentDefaultRouteProvider)
            : this(
                new DelayedInitReference<ICrScene>(),
                crAlarmAreaInfo,
                parentSceneGroup,
                noPrewarning,
                parentDefaultRouteProvider)
        {
        }

        private UnconditionalSetAlarmAreaSceneGroup(
            DelayedInitReference<ICrScene> sceneProvider,
            CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo,
            IAuthorizedSceneGroup parentSceneGroup,
            bool noPrewarning,
            IInstanceProvider<ACrSceneRoute> parentDefaultRouteProvider)
            : base(
                sceneProvider,
                parentDefaultRouteProvider)
        {
            _noPrewarning = noPrewarning;

            _crAlarmAreaInfo = crAlarmAreaInfo;
            _parentSceneGroup = parentSceneGroup;

            TimeOutGroupExitRoute = new CrSceneGroupExitRoute(
                this,
                _parentSceneGroup.TimeOutGroupExitRoute);

            sceneProvider.Instance = new Scene(this);
        }

        public ACardReaderSettings CardReaderSettings
        {
            get { return _parentSceneGroup.CardReaderSettings; }
        }

        public AccessDataBase AccessData
        {
            get { return _parentSceneGroup.AccessData; }

        }

        public CrSceneGroupExitRoute TimeOutGroupExitRoute { get; private set; }
    }
}
