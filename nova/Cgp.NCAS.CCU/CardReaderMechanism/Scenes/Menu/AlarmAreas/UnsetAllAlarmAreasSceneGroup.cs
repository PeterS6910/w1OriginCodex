using System;
using System.Collections.Generic;
using System.Linq;

using Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Menu.Common;
using Contal.Cgp.NCAS.CCU.EventParameters;
using Contal.Drivers.CardReader;

using CrSceneFrameworkCF;
using CrSceneFrameworkCF.Scenes;

namespace Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Menu.AlarmAreas
{
    internal class UnsetAllAlarmAreasSceneGroup :
        ASetUnsetAllAlarmAreasQuestionSceneGroup,
        TimeBuyingMenuScene.ITimeBuyingSceneGroup
    {
        private class DetailedResultMenuScene : CrMenuScene
        {
            public class MenuItemsProvider : ACrMenuSceneItemsProvider<MenuItemsProvider>
            {
                private class DisplayAlarmAreaMenuItem : ACrMenuSceneItem<MenuItemsProvider>
                {
                    private readonly CrAlarmAreasManager.CrAlarmAreaInfo _crAlarmAreaInfo;

                    private class RouteProvider : IInstanceProvider<ACrSceneRoute, MenuItemsProvider>
                    {
                        public ACrSceneRoute GetInstance(MenuItemsProvider menuItemsProvider)
                        {
                            return CrSceneAdvanceRoute.Default;
                        }
                    }

                    public DisplayAlarmAreaMenuItem(CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo)
                        : base(new RouteProvider())
                    {
                        _crAlarmAreaInfo = crAlarmAreaInfo;
                    }

                    protected override string GetText(MenuItemsProvider menuItemsProvider)
                    {
                        return _crAlarmAreaInfo.ToString();
                    }
                }

                private readonly UnsetAllAlarmAreasSceneGroup _sceneGroup;

                public MenuItemsProvider(
                    UnsetAllAlarmAreasSceneGroup sceneGroup,
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
                    var alarmAreas =
                        new LinkedList<CrAlarmAreasManager.CrAlarmAreaInfo>(
                            _sceneGroup.AlarmAreaInfos.Where(
                                alarmAreaInfo => _sceneGroup._unsetAlarmAreasProcess
                                    .IdAlarmAreasWithNonAcknowledgedAlarmDuringSetPeriod
                                    .Contains(alarmAreaInfo.IdAlarmArea)));

                    foreach (var crAlarmAreaInfo in alarmAreas)
                        yield return new DisplayAlarmAreaMenuItem(crAlarmAreaInfo);
                }
            }

            private DetailedResultMenuScene(
                UnsetAllAlarmAreasSceneGroup sceneGroup,
                DelayedInitReference<CrMenuScene> delayedInitReference)
                : base(
                    new MenuItemsProvider(
                        sceneGroup,
                        delayedInitReference),
                    MenuConfigurations.GetAvailableAlarmAreasMenuConfiguration(sceneGroup.CardReaderSettings.CardReader), 
                    sceneGroup.DefaultGroupExitRoute,
                    sceneGroup.TimedOutGroupExitRoute)
            {
                delayedInitReference.Instance = this;
            }

            public DetailedResultMenuScene(
                UnsetAllAlarmAreasSceneGroup sceneGroup)
                : this(
                    sceneGroup,
                    new DelayedInitReference<CrMenuScene>())
            {
            }
        }

        public CrSceneGroupExitRoute TimedOutGroupExitRoute
        {
            get;
            private set;
        }

        private class ResultScene : AResultScene
        {
            private readonly UnsetAllAlarmAreasSceneGroup _sceneGroup;

            private ICrScene _detailedResultMenuScene;

            public ResultScene(UnsetAllAlarmAreasSceneGroup sceneGroup)
            {
                _sceneGroup = sceneGroup;
            }

            protected override long TimeoutDuration
            {
                get
                {

                    return
                        _sceneGroup._unsetAlarmAreasProcess
                            .IdAlarmAreasWithNonAcknowledgedAlarmDuringSetPeriod.Count > 0
                            ? CardReaderConstants.SHOWQUESTIONDELAY
                            : CardReaderConstants.SHOWINFODELAYPREMIUM;
                }
            }

            protected override bool Success
            {
                get
                {
                    return
                        _sceneGroup._unsetAlarmAreasProcess.CountAlarmAreasToUnset
                            == _sceneGroup._unsetAlarmAreasProcess.CountUnsetAlarmAreas;
                }
            }

            protected override ACardReaderSettings CardReaderSettings
            {
                get { return _sceneGroup.CardReaderSettings; }
            }

            protected override void DisplaySuccessMessage(CrDisplayProcessor crDisplayProcessor)
            {
                byte top = 1;
                
                top = crDisplayProcessor.DisplayWriteText(
                    crDisplayProcessor.GetLocalizationString("UnsetSuccess"),
                    0,
                    top);
                
                //_unsetAlarmAreasProcess.

                DisplayAlarmAreasWithNonAcknowledgedAlarm(
                    crDisplayProcessor,
                    top);
            }

            private void DisplayAlarmAreasWithNonAcknowledgedAlarm(
                CrDisplayProcessor crDisplayProcessor,
                byte top)
            {
                var idAlarmAreasWithNonAcknowledgedAlarmDuringSetPeriod =
                    _sceneGroup._unsetAlarmAreasProcess
                        .IdAlarmAreasWithNonAcknowledgedAlarmDuringSetPeriod;

                if (idAlarmAreasWithNonAcknowledgedAlarmDuringSetPeriod.Count == 0)
                    return;

                ++top;

                top = crDisplayProcessor.DisplayWriteText(
                    crDisplayProcessor.GetLocalizationString("UnsetNotAcknowledgedInSetPeriodList"),
                    0,
                    top);

                var cardReader = _sceneGroup.CardReaderSettings.CardReader;

                int numToDisplay = idAlarmAreasWithNonAcknowledgedAlarmDuringSetPeriod.Count;

                foreach (var alarmAreaInfo in _sceneGroup.AlarmAreaInfos)
                {
                    if (top == 16
                        && numToDisplay > 1)
                    {
                        cardReader.DisplayCommands.DisplayText(
                            cardReader,
                            0,
                            top,
                            string.Format(
                                crDisplayProcessor
                                    .GetLocalizationString("UnsetNotAcknowledgedInSetPeriodAll"),
                                numToDisplay));

                        cardReader.MenuCommands.SetBottomMenuButtons(
                            cardReader,
                            new CRBottomMenu(
                                CRMenuButtonLook.Yes,
                                CRSpecialKey.Yes,
                                CRMenuButtonLook.Clear,
                                CRSpecialKey.No,
                                CRMenuButtonLook.Clear,
                                CRSpecialKey.No,
                                CRMenuButtonLook.No,
                                CRSpecialKey.No));

                        break;
                    }

                    if (idAlarmAreasWithNonAcknowledgedAlarmDuringSetPeriod.Contains(alarmAreaInfo.IdAlarmArea))
                    {
                        cardReader.DisplayCommands.DisplayText(
                            cardReader,
                            1,
                            top++,
                            alarmAreaInfo.ToString());

                        --numToDisplay;
                        
                    }
                }
            }

            protected override void DisplayFailureMessage(CrDisplayProcessor crDisplayProcessor)
            {
                byte top = 1;

                //if (_sceneGroup._unsetAlarmAreasProcess.CountTimeBuyingErrors > 0)
                //    top = crDisplayProcessor.DisplayWriteText(
                //        crDisplayProcessor.GetLocalizationString("TimeBuyingErrors"),
                //        0,
                //        top);

                top = crDisplayProcessor.DisplayWriteText(
                    string.Format(
                        crDisplayProcessor.GetLocalizationString("UnsetFailure"),
                        _sceneGroup._unsetAlarmAreasProcess.CountUnsetAlarmAreas),
                    0,
                    top);

                DisplayAlarmAreasWithNonAcknowledgedAlarm(
                    crDisplayProcessor,
                    top);
            }

            public override void OnSpecialKeyPressed(
                ACrSceneContext crSceneContext,
                CRSpecialKey specialKey)
            {
                if (specialKey == CRSpecialKey.Yes)
                {
                    _detailedResultMenuScene = 
                        new DetailedResultMenuScene(
                            _sceneGroup);

                    CrSceneAdvanceRoute.Default.Follow(crSceneContext);

                    return;
                }

                base.OnSpecialKeyPressed(
                    crSceneContext, 
                    specialKey);
            }

            public override ICrScene GetDetailedResultScene()
            {
                return _detailedResultMenuScene;
            }
        }
        
        private readonly UnsetAllAlarmAreasProcess _unsetAlarmAreasProcess;

        public UnsetAllAlarmAreasSceneGroup(
            IAuthorizedSceneGroup parentSceneGroup,
            IEnumerable<CrAlarmAreasManager.CrAlarmAreaInfo> alarmAreaInfos)
            : base(parentSceneGroup)
        {
            TimedOutGroupExitRoute = 
                new CrSceneGroupExitRoute(
                    this,
                    parentSceneGroup.TimeOutGroupExitRoute);

            _unsetAlarmAreasProcess = new UnsetAllAlarmAreasProcess(
                parentSceneGroup.AccessData,
                parentSceneGroup.CardReaderSettings.Id,
                alarmAreaInfos);
        }

        public override IEnumerable<CrAlarmAreasManager.CrAlarmAreaInfo> AlarmAreaInfos
        {
            get { return _unsetAlarmAreasProcess.AlarmAreaInfos; }
        }

        protected override bool SetAlarmAreas
        {
            get { return false; }
        }

        protected override AResultScene CreateResultScene()
        {
            return new ResultScene(this);
        }

        protected override ICrScene CreateExtraParametersScene()
        {
            return _unsetAlarmAreasProcess.TimeBuingRequired
                ? new TimeBuyingMenuScene(this)
                : null;
        }

        protected override void SetUnsetAllAlarmAreas()
        {
            _unsetAlarmAreasProcess.Execute();
        }

        public override void ProcessEventAccessDeniedInvalidPin()
        {
            Events.ProcessEvent(
                new EventCrAccessDeniedUnsetAlarmAreaInvalidPin(
                    ParentSceneGroup.CardReaderSettings.Id,
                    ParentSceneGroup.AccessData,
                    Guid.Empty));
        }

        public ACardReaderSettings CardReaderSettings
        {
            get { return ParentSceneGroup.CardReaderSettings; }
        }

        public int TimeToBuy
        {
            set { _unsetAlarmAreasProcess.TimeToBuy = value; }
        }

        bool TimeBuyingMenuScene.ITimeBuyingSceneGroup.MaxTimeBuyingEnabled
        {
            get { return true; }
        }

        bool TimeBuyingMenuScene.ITimeBuyingSceneGroup.IsUnsetEnabledInTimeBuyingScene
        {
            get
            {
                return _unsetAlarmAreasProcess.CountAlarmAreasToUnset > 0;
            }
        }
    }
}