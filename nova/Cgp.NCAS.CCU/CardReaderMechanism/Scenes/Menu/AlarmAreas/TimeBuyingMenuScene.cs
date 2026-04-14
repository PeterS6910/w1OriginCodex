using System.Collections.Generic;

using CrSceneFrameworkCF;
using CrSceneFrameworkCF.Scenes;

namespace Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Menu.AlarmAreas
{
    internal class TimeBuyingMenuScene : CrMenuScene
    {
        public interface ITimeBuyingSceneGroup
        {
            CrSceneGroupExitRoute DefaultGroupExitRoute
            {
                get;
            }

            ACardReaderSettings CardReaderSettings
            {
                get;
            }

            int TimeToBuy
            {
                set;
            }

            bool MaxTimeBuyingEnabled
            {
                get;
            }

            bool IsUnsetEnabledInTimeBuyingScene
            {
                get;
            }
        }

        private class MenuItemsProvider :
            ACrMenuSceneItemsProvider<MenuItemsProvider>,
            ICcuMenuItemsProvider
        {
            private readonly ITimeBuyingSceneGroup _timeBuyingSceneGroup;

            public MenuItemsProvider(
                ITimeBuyingSceneGroup timeBuyingSceneGroup,
                IInstanceProvider<CrMenuScene> menuSceneProvider)
                : base(menuSceneProvider)
            {
                _timeBuyingSceneGroup = timeBuyingSceneGroup;
            }

            private class TimeBuyingMenuItem : ALocalizedMenuItem<MenuItemsProvider>
            {
                private readonly string _localizationKey;

                private class RouteProvider : IInstanceProvider<ACrSceneRoute, MenuItemsProvider>
                {
                    private readonly int _timeToBuy;

                    public RouteProvider(
                        int timeToBuy)
                    {
                        _timeToBuy = timeToBuy;
                    }

                    public ACrSceneRoute GetInstance(MenuItemsProvider menuItemsProvider)
                    {
                        menuItemsProvider._timeBuyingSceneGroup.TimeToBuy = _timeToBuy;
                        return CrSceneAdvanceRoute.Default;
                    }
                }

                public TimeBuyingMenuItem(
                    int timeToBuy,
                    string localizationKey)
                    : base(
                        new RouteProvider(
                            timeToBuy))
                {
                    _localizationKey = localizationKey;
                }

                protected override string GetLocalizationKey(MenuItemsProvider menuItemsProvider)
                {
                    return _localizationKey;
                }

                protected override CrIconSymbol GetGraphicIndex(MenuItemsProvider menuItemsProvider)
                {
                    return CrIconSymbol.TimeBuyingLarge;
                }
            }

            private class TimeBuyingUnsetMenuItem : TimeBuyingMenuItem
            {
                public TimeBuyingUnsetMenuItem(
                    int timeToBuy,
                    string localizationKey)
                    : base(
                        timeToBuy,
                        localizationKey)
                {
                }

                protected override CrIconSymbol GetGraphicIndex(MenuItemsProvider menuItemsProvider)
                {
                    return CrIconSymbol.UnsetLarge;
                }
            }

            protected override MenuItemsProvider This
            {
                get { return this; }
            }

            protected override IEnumerable<ACrMenuSceneItem<MenuItemsProvider>> OnEnteredInternal()
            {
                IList<ACrMenuSceneItem<MenuItemsProvider>> result = new List<ACrMenuSceneItem<MenuItemsProvider>>();

#if DEBUG
                result.Add(
                    new TimeBuyingMenuItem(
                        60,
                        CardReaderConstants.MENUTIMEBUYING1MIN));
#endif

                result.Add(
                    new TimeBuyingMenuItem(
                        30 * 60,
                        CardReaderConstants.MENUTIMEBUYING30MIN));

                result.Add(
                    new TimeBuyingMenuItem(
                        60 * 60,
                        CardReaderConstants.MENUTIMEBUYING1HOD));

                if (_timeBuyingSceneGroup.MaxTimeBuyingEnabled)
                    result.Add(
                        new TimeBuyingMenuItem(
                            -1,
                            CardReaderConstants.MENUTIMEBUYINGMAX));

                if (_timeBuyingSceneGroup.IsUnsetEnabledInTimeBuyingScene)
                    result.Add(
                        new TimeBuyingUnsetMenuItem(
                            0,
                            CardReaderConstants.MENUTIMEBUYINGUNSET));

                return result;
            }

            public ACardReaderSettings CardReaderSettings
            {
                get { return _timeBuyingSceneGroup.CardReaderSettings; }
            }
        }

        public TimeBuyingMenuScene(ITimeBuyingSceneGroup timeBuyingSceneGroup)
            : this(
                timeBuyingSceneGroup,
                new DelayedInitReference<CrMenuScene>())
        {

        }

        private TimeBuyingMenuScene(
            ITimeBuyingSceneGroup timeBuyingSceneGroup,
            DelayedInitReference<CrMenuScene> delayedInitReference)
            : base(
                new MenuItemsProvider(
                    timeBuyingSceneGroup,
                    delayedInitReference),
                MenuConfigurations.GetTimeBuyingMenuConfiguration(timeBuyingSceneGroup.CardReaderSettings.CardReader),
                timeBuyingSceneGroup.DefaultGroupExitRoute,
                timeBuyingSceneGroup.DefaultGroupExitRoute)
        {
            delayedInitReference.Instance = this;
        }
    }
}
