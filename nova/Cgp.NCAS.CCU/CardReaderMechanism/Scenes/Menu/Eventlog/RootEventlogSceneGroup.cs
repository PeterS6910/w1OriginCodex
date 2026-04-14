using System.Collections.Generic;
using System.Linq;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.Alarms;
using Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Menu.Common;
using Contal.Cgp.NCAS.CCU.EventParameters;
using Contal.Drivers.CardReader;

using CrSceneFrameworkCF;
using CrSceneFrameworkCF.Scenes;

using CardReader = Contal.Drivers.CardReader.CardReader;

namespace Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Menu.Eventlog
{
    internal class RootEventlogSceneGroup :
        ASceneAuthorizationSceneGroupByEntrySL<RootEventlogSceneGroup>
    {
        private class MenuItemsProvider :
            ACrMenuSceneItemsProvider<MenuItemsProvider>,
            ICrAlarmAreaEventHandler,
            ICcuMenuItemsProvider
        {
            private readonly RootEventlogSceneGroup _sceneGroup;

            private class ShowAllEventlogsMenuItem : ALocalizedMenuItem<MenuItemsProvider>
            {
                private class RouteProvider : IInstanceProvider<ACrSceneRoute, MenuItemsProvider>
                {
                    public ACrSceneRoute GetInstance(MenuItemsProvider menuItemsProvider)
                    {
                        return new EventlogSceneGroup(
                            menuItemsProvider._sceneGroup,
                            null).EnterRoute;
                    }
                }

                public ShowAllEventlogsMenuItem()
                    : base(new RouteProvider())
                {
                }

                protected override string GetLocalizationKey(MenuItemsProvider menuItemsProvider)
                {
                    return CardReaderConstants.MENUSHOWEVENTLOGS;
                }

                protected override IEnumerable<CrIconSymbol> GetInlinedIcons(MenuItemsProvider menuItemsProvider)
                {
                    yield return CrIconSymbol.ShowAllEventLogs;
                }

                public override bool IsVisible(MenuItemsProvider menuItemsProvider)
                {
                    return menuItemsProvider._isAnyAlarmAreaVisible;
                }
            }

            private class AlarmAreaEventlogCategoryMenuItem : ACcuMenuItem<MenuItemsProvider>
            {
                private readonly DB.AlarmArea _alarmArea;

                private class RouteProvider : IInstanceProvider<ACrSceneRoute, MenuItemsProvider>
                {
                    private readonly IInstanceProvider<AlarmAreaEventlogCategoryMenuItem>
                        _menuItemProvider;

                    public RouteProvider(IInstanceProvider<AlarmAreaEventlogCategoryMenuItem> menuItemProvider)
                    {
                        _menuItemProvider = menuItemProvider;
                    }

                    public ACrSceneRoute GetInstance(MenuItemsProvider menuItemsProvider)
                    {
                        return
                            new EventlogSceneGroup(
                                    menuItemsProvider._sceneGroup,
                                    _menuItemProvider.Instance._alarmArea)
                                .EnterRoute;
                    }
                }

                public AlarmAreaEventlogCategoryMenuItem(DB.AlarmArea alarmArea)
                    : this(
                        alarmArea,
                        new DelayedInitReference<AlarmAreaEventlogCategoryMenuItem>())
                {
                }

                private AlarmAreaEventlogCategoryMenuItem(
                    DB.AlarmArea alarmArea,
                    DelayedInitReference<AlarmAreaEventlogCategoryMenuItem> delayedInitReference) :
                    base(new RouteProvider(delayedInitReference))
                {
                    delayedInitReference.Instance = this;

                    _alarmArea = alarmArea;
                }

                protected override IEnumerable<CrIconSymbol> GetInlinedIcons(MenuItemsProvider menuItemsProvider)
                {
                    yield return
                        EventsForCardReadersDispatcher.Singleton.GetMarkingOfAlarmArea(_alarmArea.IdAlarmArea)
                            ? CrIconSymbol.AlarmAreaIsInAlarm
                            : CrIconSymbol.AlarmAreaIsNotAcknowledged;
                }

                protected override string GetText(MenuItemsProvider menuItemsProvider)
                {
                    return _alarmArea.ToString();
                }
            }

            private bool _isAnyAlarmAreaVisible;

            private List<CrAlarmAreasManager.CrAlarmAreaInfo> _sortedAlarmAreInfos;

            private readonly IDictionary<CrAlarmAreasManager.CrAlarmAreaInfo, AlarmAreaEventlogCategoryMenuItem> _alarmAreaEventlogCategoryMenuItems =
                new Dictionary<CrAlarmAreasManager.CrAlarmAreaInfo, AlarmAreaEventlogCategoryMenuItem>();

            public MenuItemsProvider(
                RootEventlogSceneGroup sceneGroup,
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
                EventsForCardReadersDispatcher.Singleton.StartMarkingObservation();

                IList<ACrMenuSceneItem<MenuItemsProvider>> result =
                    new List<ACrMenuSceneItem<MenuItemsProvider>>();

               _sceneGroup.CardReaderSettings.CrAlarmAreasManager.Attach(this);

                result.Add(new ShowAllEventlogsMenuItem());

                foreach (var alarmAreaInfo in _sortedAlarmAreInfos)
                {
                    var menuItem = new AlarmAreaEventlogCategoryMenuItem(alarmAreaInfo.AlarmArea);

                    _alarmAreaEventlogCategoryMenuItems.Add(
                        alarmAreaInfo,
                        menuItem);

                    result.Add(menuItem);
                    _isAnyAlarmAreaVisible = true;
                }

                return result;
            }

            protected override void OnExitedInternal()
            {
                _sceneGroup.CardReaderSettings.CrAlarmAreasManager.Detach(this);
                EventsForCardReadersDispatcher.Singleton.StopMarkingObservation();
            }

            public void OnAttached(ICollection<CrAlarmAreasManager.CrAlarmAreaInfo> observedAlarmAreas)
            {
                _sortedAlarmAreInfos = new List<CrAlarmAreasManager.CrAlarmAreaInfo>(
                    observedAlarmAreas
                        .Where(alarmAreaInfo => alarmAreaInfo.IsEventlogEnabled));

                _sortedAlarmAreInfos.Sort(
                    (alarmAreaInfo1, alarmAreaInfo2) => 
                        System.String.Compare(
                            alarmAreaInfo1.ToString(), 
                            alarmAreaInfo2.ToString(), 
                            System.StringComparison.Ordinal));
            }

            public void OnActivationStateChanged(CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo)
            {
            }

            public void OnAlarmStateChanged(CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo)
            {
            }

            public void OnNotAcknolwedgedStateChanged(
                CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo,
                bool notAcknowledged)
            {
            }

            public void OnAlarmAreaAdded(CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo)
            {
                //TODO
            }

            public void OnAlarmAreaRemoved(CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo)
            {
                //TODO
            }

            public void OnAACardReaderRightsChanged(CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo)
            {
            }

            public void OnDetached()
            {
                _sortedAlarmAreInfos.Clear();
            }

            public void OnAlarmAreaMarkingChanged(CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo)
            {
                AlarmAreaEventlogCategoryMenuItem menuItem;

                if (!_alarmAreaEventlogCategoryMenuItems.TryGetValue(
                    crAlarmAreaInfo,
                    out menuItem))
                {
                    return;
                }

                UpdateItem(
                    CardReaderSettings.SceneContext,
                    menuItem);
            }

            public void OnAnySensorInAlarmChanged(
                CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo,
                bool value)
            {
            }

            public void OnAnySensorInTamperChanged(
                CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo,
                bool value)
            {
            }

            public void OnAnySensorNotAcknowledgedChanged(
                CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo,
                bool value)
            {
            }

            public void OnAnySensorTemporarilyBlockedChanged(
                CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo,
                bool value)
            {
            }

            public void OnAnySensorPermanentlyBlockedChanged(
                CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo,
                bool value)
            {
            }

            public ACardReaderSettings CardReaderSettings
            {
                get { return _sceneGroup.CardReaderSettings; }
            }
        }

        private class Scene : CrMenuScene
        {
            private readonly CrDisplayProcessor _crDisplayProcessor;

            public Scene(RootEventlogSceneGroup sceneGroup)
                : this(
                    sceneGroup,
                    sceneGroup.CardReaderSettings,
                    new DelayedInitReference<CrMenuScene>())
            {
            }

            private Scene(
                RootEventlogSceneGroup sceneGroup,
                ACardReaderSettings cardReaderSettings,
                DelayedInitReference<CrMenuScene> delayedInitReference)
                : base(
                    new MenuItemsProvider(
                        sceneGroup,
                        delayedInitReference),
                    MenuConfigurations.GetEventlogsMenuConfiguration(cardReaderSettings.CardReader),
                    sceneGroup.DefaultGroupExitRoute,
                    sceneGroup.DefaultGroupExitRoute)
            {
                delayedInitReference.Instance = this;
                _crDisplayProcessor = cardReaderSettings.CrDisplayProcessor;
            }

            protected override void ShowNoMenuItems(CardReader cardReader)
            {
                _crDisplayProcessor.DisplayWriteText(
                    _crDisplayProcessor.GetLocalizationString(
                        "NoAlarmAreasWithEventlogsToDisplay"),
                    0,
                    0);
            }
        }

        private class AuthorizationProcessClass
            : ASceneAuthorizationProcessByEntrySL<RootEventlogSceneGroup>
        {
            public AuthorizationProcessClass(
                IInstanceProvider<RootEventlogSceneGroup> sceneGroupProvider)
                : base(sceneGroupProvider)
            {
            }

            protected override bool AuthorizeByCardInternal()
            {
                return
                    CardReaderSettings.CREventlogProcessor
                        .ControlAccessToEventlog(AccessData);
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
                        new EventCrAccessDeniedEnterToEventlogsMenuInvalidCode(idCardReader));
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
                        new EventCrAccessDeniedEnterToEventlogsMenuInvalidPin(
                            idCardReader,
                            AccessData));
                }

                AlarmsManager.Singleton.AddAlarm(
                    new CrInvalidPinAlarm(
                        idCardReader,
                        AccessData.IdCard));
            }

            protected override bool AuthorizeByPersonInternal()
            {
                return
                    CardReaderSettings.CREventlogProcessor
                        .ControlAccessToEventlog(AccessData);
            }
        }

        private class AuthorizationSceneGroup : AAuthorizationSceneGroup
        {
            private class WaitingForCardScene : AWaitingForCardScene
            {
                public WaitingForCardScene(AAuthorizationSceneGroup sceneGroup)
                    : base(sceneGroup)
                {
                }

                protected override void ShowInternal(CardReader cardReader)
                {
                    cardReader.AccessCommands.WaitingForCard(cardReader);

                    cardReader.DisplayCommands.DisplaySymbol(
                        cardReader,
                        CRSymbolClass.HardDefinedPicture,
                        (byte)CrIconSymbol.EventLog,
                        true,
                        230,
                        115,
                        0,
                        0);
                }
            }

            private class WaitingForCodeScene : AAuthorizationSceneForcedGinCodeLedPresentation
            {
                public WaitingForCodeScene(AAuthorizationSceneGroup sceneGroup)
                    : base(sceneGroup)
                {
                }

                protected override void ShowInternal(CardReader cardReader)
                {
                    cardReader.AccessCommands.WaitingForCode(
                        cardReader
                        //,CcuCardReaders.MaximalCodeLength
                        );

                    cardReader.DisplayCommands.DisplaySymbol(
                        cardReader,
                        CRSymbolClass.HardDefinedPicture,
                        (byte)CrIconSymbol.EventLogSmall,
                        true,
                        250,
                        53,
                        0,
                        0);
                }
            }

            private class WaitingForPinScene : AAuthorizationScene
            {
                public WaitingForPinScene(AAuthorizationSceneGroup sceneGroup)
                    : base(sceneGroup)
                {
                }

                protected override void ShowInternal(CardReader cardReader)
                {
                    if (CcuCardReaders.IsPinConfirmationObligatory)
                        cardReader.AccessCommands.WaitingForPIN(
                            cardReader,
                            CcuCardReaders.MinimalPinLength,
                            CcuCardReaders.MaximalPinLength,
                            SceneGroup.ParentSceneGroup.SceneAuthorizationProcess.PinLength,
                            true);
                    else
                        cardReader.AccessCommands.WaitingForPIN(
                            cardReader,
                            SceneGroup.ParentSceneGroup.SceneAuthorizationProcess.PinLength,
                            SceneGroup.ParentSceneGroup.SceneAuthorizationProcess.PinLength,
                            SceneGroup.ParentSceneGroup.SceneAuthorizationProcess.PinLength,
                            false);

                    cardReader.DisplayCommands.DisplaySymbol(
                        cardReader,
                        CRSymbolClass.HardDefinedPicture,
                        (byte) CrIconSymbol.EventLog,
                        true,
                        270,
                        53,
                        0,
                        0);
                }
            }

            public AuthorizationSceneGroup(RootEventlogSceneGroup parentSceneGroup)
                : base(parentSceneGroup)
            {
            }

            protected override ICrScene GetSceneWaitingForCode()
            {
                return new WaitingForCodeScene(this);
            }

            protected override ICrScene GetSceneWaitingForCard()
            {
                return new WaitingForCardScene(this);
            }

            protected override ICrScene GetSceneWaitingForPin()
            {
                return new WaitingForPinScene(this);
            }
        }

        public RootEventlogSceneGroup(ACardReaderSettings cardReaderSettings)
            : this(
                new DelayedInitReference<RootEventlogSceneGroup>(),
                cardReaderSettings)
        {
        }

        private RootEventlogSceneGroup(
            DelayedInitReference<RootEventlogSceneGroup> delayedInitReference,
            ACardReaderSettings cardReaderSettings)
            : base(
                new AuthorizationProcessClass(delayedInitReference),
                CrSceneGroupReturnRoute.Default,
                cardReaderSettings)
        {
            delayedInitReference.Instance = this;
        }

        protected override ICrScene CreateAuthorizedScene()
        {
            return new Scene(this);
        }

        protected override AAuthorizationSceneGroup CreateInnerAuthorizationSceneGroup()
        {
            return new AuthorizationSceneGroup(this);
        }
    }
}
