using System.Collections.Generic;

using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.Alarms;
using Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Menu.Common;
using CrSceneFrameworkCF;
using CrSceneFrameworkCF.Scenes;

using CardReader = Contal.Drivers.CardReader.CardReader;

namespace Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Menu.AlarmAreas
{
    internal class AlarmAreaAccessInfo : AAlarmAreaAccessInfoBase
    {
        public AlarmAreaAccessInfo(
            CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo,
            AlarmAreaAccessManager alarmAreaAccessManager)
            : base(
                crAlarmAreaInfo,
                alarmAreaAccessManager)
        {
        }

        protected override bool CheckPersonAccessRights(IAlarmAreaAccessManager accessManager)
        {
            var idAlarmArea = CrAlarmAreaInfo.IdAlarmArea;

            return CrAlarmAreaInfo.IsUnset
                ? accessManager.CheckRigthsToSet(idAlarmArea)
                : accessManager.CheckRigthsToUnset(idAlarmArea)
                  || (CrAlarmAreaInfo.IsTimeBuyingPossible
                      && accessManager.CheckRigthsToTimeBuying(idAlarmArea));
        }
    }

    internal class AlarmAreasMenuScene<TAlarmAreasMenuSceneGroup, TMenuItemsProvider> : 
        CrMenuScene
        where TAlarmAreasMenuSceneGroup : class, IAuthorizedSceneGroup
        where TMenuItemsProvider : AlarmAreasMenuScene<
            TAlarmAreasMenuSceneGroup, 
            TMenuItemsProvider>.AMenuItemsProvider
    {
        private readonly CrDisplayProcessor _crDisplayProcessor;

        public abstract class AMenuItemsProvider :
            AAlarmAreasMenuItemsProviderBase<
                TMenuItemsProvider,
                IAlarmAreaAccessEventHandler,
                TAlarmAreasMenuSceneGroup>,
            IInstanceProvider<TMenuItemsProvider>
        {
            private class AlarmAreaMenuItem : AlarmAreaMenuItemBase
            {
                private class RouteProvider : ARouteProvider
                {
                    public RouteProvider(AAlarmAreaAccessInfoBase alarmAreaAccessInfo)
                        : base(alarmAreaAccessInfo)
                    {
                    }

                    public override ACrSceneRoute GetInstance(TMenuItemsProvider menuItemsProvider)
                    {
                        var alarmAreasMenuSceneGroup = menuItemsProvider.SceneGroupProvider.Instance;

                        return new AlarmAreaEditSceneGroup(
                            AlarmAreaAccessInfo.CrAlarmAreaInfo,
                            alarmAreasMenuSceneGroup,
                            menuItemsProvider.AlarmAreaAccessManager,
                            CrSceneGroupReturnRoute.Default).EnterRoute;
                    }
                }

                public AlarmAreaMenuItem(AAlarmAreaAccessInfoBase alarmAreaAccessInfo)
                    : base(
                        alarmAreaAccessInfo,
                        new RouteProvider(alarmAreaAccessInfo))
                {
                }

                protected override IEnumerable<CrIconSymbol> GetAdditionalInlinedIcons()
                {
                    var alarmAreaInfo = AccessInfo.CrAlarmAreaInfo;

                    if (alarmAreaInfo.AlarmState == State.Alarm)
                        yield return CrIconSymbol.AlarmAreaIsInAlarm;

                    if (alarmAreaInfo.IsNotAcknowledged)
                        yield return CrIconSymbol.NotAcknowledged;

                    bool isExternalAA;
                    bool isWaiting;
                    bool wasConfirmSetUnset;

                    AlarmArea.AlarmAreas.Singleton.GetStateOfExternalAA(
                        alarmAreaInfo.IdAlarmArea,
                        out isExternalAA,
                        out isWaiting,
                        out wasConfirmSetUnset);

                    if (isExternalAA && isWaiting)
                        yield return CrIconSymbol.Waiting;
                }
            }

            private class SetAllMenuItem : ALocalizedMenuItem<TMenuItemsProvider>
            {
                private class RouteProvider : IInstanceProvider<ACrSceneRoute, TMenuItemsProvider>
                {
                    public ACrSceneRoute GetInstance(TMenuItemsProvider menuItemsProvider)
                    {
                        return new SetAllAlarmAreasQuestionSceneGroup(
                            menuItemsProvider.SceneGroupProvider.Instance,
                            menuItemsProvider.AlarmAreasToSet).EnterRoute;
                    }
                }

                public SetAllMenuItem()
                    : base(new RouteProvider())
                {

                }

                public override bool IsVisible(TMenuItemsProvider menuItemsProvider)
                {
                    return menuItemsProvider._isAnyAaToSet;
                }

                protected override string GetLocalizationKey(TMenuItemsProvider menuItemsProvider)
                {
                    return CardReaderConstants.MENUSETALL;
                }

                protected override IEnumerable<CrIconSymbol> GetInlinedIcons(TMenuItemsProvider menuItemsProvider)
                {
                    yield return CrIconSymbol.SetAll;
                }
            }

            private class UnsetAllMenuItem : ALocalizedMenuItem<TMenuItemsProvider>
            {
                private class RouteProvider : IInstanceProvider<ACrSceneRoute, TMenuItemsProvider>
                {
                    public ACrSceneRoute GetInstance(TMenuItemsProvider menuItemsProvider)
                    {
                        return new UnsetAllAlarmAreasSceneGroup(
                            menuItemsProvider.SceneGroupProvider.Instance,
                            menuItemsProvider.AlarmAreasToUnset).EnterRoute;
                    }
                }

                public UnsetAllMenuItem()
                    : base(new RouteProvider())
                {
                }

                public override bool IsVisible(TMenuItemsProvider menuItemsProvider)
                {
                    return menuItemsProvider._isAnyAaToUnset;
                }

                protected override string GetLocalizationKey(TMenuItemsProvider menuItemsProvider)
                {
                    return CardReaderConstants.MENUUNSETALL;
                }

                protected override IEnumerable<CrIconSymbol> GetInlinedIcons(TMenuItemsProvider menuItemsProvider)
                {
                    yield return CrIconSymbol.UnsetAll;
                }
            }

            private class AcknowledgeAllMenuItem : ALocalizedMenuItem<TMenuItemsProvider>
            {
                private class RouteProvider : IInstanceProvider<ACrSceneRoute, TMenuItemsProvider>
                {
                    public ACrSceneRoute GetInstance(TMenuItemsProvider menuItemsProvider)
                    {
                        AcknowledgeAll(menuItemsProvider);

                        return CrSceneGroupReturnRoute.Default;
                    }

                    private static void AcknowledgeAll(TMenuItemsProvider menuItemsProvider)
                    {
                        var idCardReader = menuItemsProvider.CardReaderSettings.Id;
                        var accessData = menuItemsProvider.SceneGroup.AccessData;

                        foreach (var alarmAreaAccessInfo in menuItemsProvider.AlarmAreaAccessInfos)
                            AlarmsManager.Singleton.AcknowledgeAlarm(
                                AlarmAreaAlarm.CreateAlarmKey(alarmAreaAccessInfo.CrAlarmAreaInfo.IdAlarmArea),
                                new AlarmsManager.ActionFromCcuSources(
                                    idCardReader,
                                    accessData));
                    }
                }

                public AcknowledgeAllMenuItem()
                    : base(new RouteProvider())
                {
                }

                protected override string GetLocalizationKey(TMenuItemsProvider menuItemsProvider)
                {
                    return CardReaderConstants.MENUACKNOWLEDGEALL;
                }

                protected override IEnumerable<CrIconSymbol> GetInlinedIcons(TMenuItemsProvider menuItemsProvider)
                {
                    yield return CrIconSymbol.AcknowledgeAll;
                }

                public override bool IsVisible(TMenuItemsProvider menuItemsProvider)
                {
                    return menuItemsProvider.IsAnyAaNotAcknowledged;
                }
            }

            protected AMenuItemsProvider(
                IInstanceProvider<TAlarmAreasMenuSceneGroup> sceneGroupProvider,
                IInstanceProvider<CrMenuScene> menuSceneProvider)
                : base(sceneGroupProvider, menuSceneProvider)
            {
            }

            protected override AlarmAreaMenuItemBase CreateAlarmAreaMenuItem(AAlarmAreaAccessInfoBase alarmAreaAccessInfo)
            {
                return new AlarmAreaMenuItem(alarmAreaAccessInfo);
            }

            protected bool IsAnyAaInAlarmState
            {
                get;
                private set;
            }

            protected bool IsAnyAaNotAcknowledged
            {
                get;
                private set;
            }

            private bool _isAnyAaToUnset;
            private bool _isAnyAaToSet;

            private ACrMenuSceneItem<TMenuItemsProvider> _setAllMenuItem;
            private ACrMenuSceneItem<TMenuItemsProvider> _unsetAllMenuItem;
            private ACrMenuSceneItem<TMenuItemsProvider> _acknowledgeAllMenuItem;

            private void EvaluateAaStats()
            {
                IsAnyAaInAlarmState = false;
                IsAnyAaNotAcknowledged = false;
                _isAnyAaToUnset = false;
                _isAnyAaToSet = false;

                foreach (var alarmAreaAccessInfo in AlarmAreaAccessInfos)
                {
                    var alarmAreaInfo = alarmAreaAccessInfo.CrAlarmAreaInfo;

                    if (alarmAreaInfo.IsUnset)
                    {
                        if (alarmAreaInfo.IsSettable)
                            _isAnyAaToSet = true;
                    }
                    else
                    {
                        if (alarmAreaInfo.IsUnsettable)
                            _isAnyAaToUnset = true;
                    }

                    if (alarmAreaInfo.AlarmState == State.Alarm)
                        IsAnyAaInAlarmState = true;

                    if (alarmAreaAccessInfo.CrAlarmAreaInfo.IsNotAcknowledged)
                        IsAnyAaNotAcknowledged = true;
                }

            }

            protected sealed override IEnumerable<ACrMenuSceneItem<TMenuItemsProvider>> CreateGeneralItems()
            {
                EvaluateAaStats();

                _setAllMenuItem = new SetAllMenuItem();
                yield return _setAllMenuItem;

                _unsetAllMenuItem = new UnsetAllMenuItem();
                yield return _unsetAllMenuItem;

                _acknowledgeAllMenuItem = new AcknowledgeAllMenuItem();
                yield return _acknowledgeAllMenuItem;

                foreach (var crMenuSceneItem in CreateAdditionalGeneralItems())
                    yield return crMenuSceneItem;
            }

            protected virtual IEnumerable<ACrMenuSceneItem<TMenuItemsProvider>> CreateAdditionalGeneralItems()
            {
                yield break;
            }

            protected override void BeforeUpdateGeneralItems()
            {
                EvaluateAaStats();
            }

            TMenuItemsProvider IInstanceProvider<TMenuItemsProvider>.Instance
            {
                get { return This; }
            }

            protected override IAlarmAreaAccessEventHandler ThisEventHandler
            {
                get { return this; }
            }
        }

        public AlarmAreasMenuScene(
            IAuthorizedSceneGroup sceneGroup,
            TMenuItemsProvider menuItemsProvider)
            : base(
                menuItemsProvider,
                MenuConfigurations.GetAlarmStateForAlarmAreaMenuConfiguration(sceneGroup.CardReaderSettings.CardReader),
                sceneGroup.DefaultGroupExitRoute,
                sceneGroup.TimeOutGroupExitRoute)
        {
            _crDisplayProcessor = menuItemsProvider.CardReaderSettings.CrDisplayProcessor;
        }

        protected override void ShowNoMenuItems(CardReader cardReader)
        {
            _crDisplayProcessor.DisplayWriteText(
                _crDisplayProcessor.GetLocalizationString(
                    "NoAlarmAreasToDisplay"),
                0,
                0);
        }
    }
}
