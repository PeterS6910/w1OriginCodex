using System.Collections.Generic;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.Alarms;
using Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Menu.Common;
using Contal.Cgp.NCAS.CCU.EventParameters;
using Contal.Drivers.CardReader;

using CrSceneFrameworkCF;
using CrSceneFrameworkCF.Scenes;

using JetBrains.Annotations;

namespace Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Menu.AlarmAreas
{
    internal class QuickSetUnsetSceneGroup :
            ASceneAuthorizationSceneGroupByEntrySL<QuickSetUnsetSceneGroup>,
            IAuthorizedSceneGroup
    {
        private class AuthorizationProcessClass :
            ASceneAuthorizationProcessByEntrySL<QuickSetUnsetSceneGroup>
        {
            public AuthorizationProcessClass(
                IInstanceProvider<QuickSetUnsetSceneGroup> sceneGroupProvider)
                : base(sceneGroupProvider)
            {
            }

            protected override bool AuthorizeByCardInternal()
            {
                var implicitAlarmAreaInfo = SceneGroupProvider.Instance._crAlarmAreaInfo;

                var idAlarmArea = implicitAlarmAreaInfo.IdAlarmArea;

                return
                    implicitAlarmAreaInfo.IsUnset
                        ? implicitAlarmAreaInfo.IsSettable
                          && AlarmAreaAccessRightsManager.Singleton.CheckRigthsToSet(
                              AccessData,
                              idAlarmArea)
                        : implicitAlarmAreaInfo.IsUnsettable
                          && (AlarmAreaAccessRightsManager.Singleton.CheckRigthsToUnset(
                              AccessData,
                              idAlarmArea)
                              || implicitAlarmAreaInfo.IsTimeBuyingPossible
                              && AlarmAreaAccessRightsManager.Singleton.CheckRigthsToTimeBuying(
                                  AccessData,
                                  idAlarmArea));
            }

            protected override void OnAccessDeniedNoRightsForCard()
            {
                var idCardReader = CardReaderSettings.Id;

                if (BlockedAlarmsManager.Singleton.ProcessEvent(
                    AlarmType.CardReader_AccessDenied,
                    new IdAndObjectType(
                        idCardReader,
                        ObjectType.CardReader)))
                {
                    var alarmAreaInfo = SceneGroupProvider.Instance._crAlarmAreaInfo;

                    Events.ProcessEvent(
                        alarmAreaInfo.IsUnset
                            ? (EventParameters.EventParameters)
                                new EventCrAccessDeniedSetAlarmAreaNoRights(
                                    idCardReader,
                                    AccessData,
                                    alarmAreaInfo.IdAlarmArea)
                            : new EventCrAccessDeniedUnsetAlarmAreaNoRights(
                                idCardReader,
                                AccessData,
                                alarmAreaInfo.IdAlarmArea));
                }

                AlarmsManager.Singleton.AddAlarm(
                    new CrAccessDeniedAlarm(
                        idCardReader,
                        AccessData));
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
                    var alarmAreaInfo = SceneGroupProvider.Instance._crAlarmAreaInfo;

                    Events.ProcessEvent(
                        alarmAreaInfo.IsUnset
                            ? (EventParameters.EventParameters)
                                new EventAccessDeniedSetAlarmAreaInvalidCode(
                                    idCardReader,
                                    alarmAreaInfo.IdAlarmArea)
                            : new EventAccessDeniedUnsetAlarmAreaInvalidCode(
                                idCardReader,
                                alarmAreaInfo.IdAlarmArea));
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
                    var alarmAreaInfo = SceneGroupProvider.Instance._crAlarmAreaInfo;

                    Events.ProcessEvent(
                        alarmAreaInfo.IsUnset
                            ? (EventParameters.EventParameters)
                                new EventCrAccessDeniedSetAlarmAreaInvalidPin(
                                    idCardReader,
                                    AccessData,
                                    alarmAreaInfo.IdAlarmArea)
                            : new EventCrAccessDeniedUnsetAlarmAreaInvalidPin(
                                idCardReader,
                                AccessData,
                                alarmAreaInfo.IdAlarmArea));
                }


                AlarmsManager.Singleton.AddAlarm(
                    new CrInvalidPinAlarm(
                        idCardReader,
                        AccessData.IdCard));
            }

            protected override bool AuthorizeByPersonInternal()
            {
                var implicitAlarmAreaInfo = SceneGroupProvider.Instance._crAlarmAreaInfo;

                var idAlarmArea = implicitAlarmAreaInfo.IdAlarmArea;

                return
                    implicitAlarmAreaInfo.IsUnset
                        ? implicitAlarmAreaInfo.IsSettable
                          && AlarmAreaAccessRightsManager.Singleton.CheckRigthsToSet(
                              AccessData,
                              idAlarmArea)
                        : implicitAlarmAreaInfo.IsUnsettable
                          && (AlarmAreaAccessRightsManager.Singleton.CheckRigthsToUnset(
                              AccessData,
                              idAlarmArea)
                              || implicitAlarmAreaInfo.IsTimeBuyingPossible
                              && AlarmAreaAccessRightsManager.Singleton.CheckRigthsToTimeBuying(
                                  AccessData,
                                  idAlarmArea));
            }

            protected override void OnAccessDeniedNoRightsForPerson()
            {
                var idCardReader = CardReaderSettings.Id;

                if (BlockedAlarmsManager.Singleton.ProcessEvent(
                    AlarmType.CardReader_AccessDenied,
                    new IdAndObjectType(
                        idCardReader,
                        ObjectType.CardReader)))
                {
                    var alarmAreaInfo = SceneGroupProvider.Instance._crAlarmAreaInfo;

                    Events.ProcessEvent(
                        alarmAreaInfo.IsUnset
                            ? (EventParameters.EventParameters)
                                new EventCrAccessDeniedSetAlarmAreaNoRights(
                                    idCardReader,
                                    AccessData,
                                    alarmAreaInfo.IdAlarmArea)
                            : new EventCrAccessDeniedUnsetAlarmAreaNoRights(
                                idCardReader,
                                AccessData,
                                alarmAreaInfo.IdAlarmArea));
                }

                AlarmsManager.Singleton.AddAlarm(
                    new CrAccessDeniedAlarm(
                        idCardReader,
                        AccessData));
            }
        }

        private class AuthorizationSceneGroup :
            AAuthorizationSceneGroup
        {
            private new abstract class AAuthorizationScene : AAuthorizationSceneGroup.AAuthorizationScene
            {
                protected AAuthorizationScene(AuthorizationSceneGroup sceneGroup)
                    : base(sceneGroup)
                {
                }

                protected override CRBottomMenu GetBottomMenu()
                {
                    return new CRBottomMenu
                    {
                        Button1 = CRMenuButtonLook.Clear,
                        Button4 = CRMenuButtonLook.Clear
                    };
                }
            }

            private class WaitingForPinScene : AAuthorizationScene
            {
                public WaitingForPinScene(AuthorizationSceneGroup sceneGroup)
                    : base(sceneGroup)
                {
                }

                protected override void ShowInternal(CardReader cardReader)
                {
                    if (CcuCardReaders.IsPinConfirmationObligatory)
                        cardReader.AccessCommands.WaitingForPINToSetUnsetAlarmArea(
                            cardReader,
                            SceneGroup.ParentSceneGroup._crAlarmAreaInfo.IsUnset,
                            CcuCardReaders.MinimalPinLength,
                            CcuCardReaders.MaximalPinLength,
                            SceneGroup.ParentSceneGroup.SceneAuthorizationProcess.PinLength,
                            true);
                    else
                        cardReader.AccessCommands.WaitingForPINToSetUnsetAlarmArea(
                            cardReader,
                            SceneGroup.ParentSceneGroup._crAlarmAreaInfo.IsUnset,
                            SceneGroup.ParentSceneGroup.SceneAuthorizationProcess.PinLength,
                            SceneGroup.ParentSceneGroup.SceneAuthorizationProcess.PinLength,
                            SceneGroup.ParentSceneGroup.SceneAuthorizationProcess.PinLength,
                            false);
                }
            }

            private class WaitingForCodeScene : AAuthorizationSceneForcedGinCodeLedPresentation
            {
                public WaitingForCodeScene(AuthorizationSceneGroup sceneGroup)
                    : base(sceneGroup)
                {
                }

                protected override void ShowInternal(CardReader cardReader)
                {
                    cardReader.AccessCommands.WaitingForCode(
                        cardReader
                        //,CcuCardReaders.MaximalCodeLength
                        );
                }
            }

            private class WaitingForCardScene : AWaitingForCardScene
            {
                public WaitingForCardScene(AuthorizationSceneGroup sceneGroup)
                    : base(sceneGroup)
                {
                }

                protected override void ShowInternal(CardReader cardReader)
                {
                    cardReader.AccessCommands.WaitingForCardToSetUnsetAlarmArea(
                        cardReader,
                        SceneGroup.ParentSceneGroup._crAlarmAreaInfo.IsUnset);
                }

            }

            public AuthorizationSceneGroup(QuickSetUnsetSceneGroup parentSceneGroup)
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

        private class SetTypeQuestionSceneGroup
            : CrSimpleSceneGroup
            , IAuthorizedSceneGroup
        {
            private class MenuItemsProvider :
                ACrMenuSceneItemsProvider<MenuItemsProvider>,
                ICcuMenuItemsProvider,
                ISetUnsetAlarmAreaContext
            {
                private readonly SetTypeQuestionSceneGroup _setTypeQuestionSceneGroup;

                private class SetMenuItem : ALocalizedMenuItem<MenuItemsProvider>
                {
                    private readonly bool _noPrewarning;

                    public SetMenuItem(
                        bool noPrewarning,
                        IInstanceProvider<ACrSceneRoute> defaultRouteProvider)
                        : base(
                            new SetUnsetRouteProviderAdapter<MenuItemsProvider>(
                                new SetAlarmAreaRouteProvider(
                                    noPrewarning,
                                    defaultRouteProvider)))
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
                }

                public MenuItemsProvider(
                    SetTypeQuestionSceneGroup setTypeQuestionSceneGroup,
                    IInstanceProvider<CrMenuScene> menuSceneProvider)
                    : base(menuSceneProvider)
                {
                    _setTypeQuestionSceneGroup = setTypeQuestionSceneGroup;
                }

                protected override MenuItemsProvider This
                {
                    get { return this; }
                }

                protected override IEnumerable<ACrMenuSceneItem<MenuItemsProvider>> OnEnteredInternal()
                {
                    yield return
                        new SetMenuItem(
                            false,
                            _setTypeQuestionSceneGroup.DefaultGroupExitRoute);

                    yield return
                        new SetMenuItem(
                            true,
                            _setTypeQuestionSceneGroup.DefaultGroupExitRoute);
                }

                public ACardReaderSettings CardReaderSettings
                {
                    get { return _setTypeQuestionSceneGroup.CardReaderSettings; }
                }

                public IAuthorizedSceneGroup AuthorizedSceneGroup
                {
                    get { return _setTypeQuestionSceneGroup; }
                }

                public CrAlarmAreasManager.CrAlarmAreaInfo CrAlarmAreaInfo
                {
                    get { return _setTypeQuestionSceneGroup._parentSceneGroup._crAlarmAreaInfo; }
                }
            }

            private class Scene : CrMenuScene
            {
                public Scene(
                    MenuItemsProvider menuItemsProvider,
                    [NotNull] CrSceneGroupExitRoute defaultExitRoute)
                    : base(
                        menuItemsProvider,
                        MenuConfigurations.GetEditAlarmAreaMenuConfiguration(menuItemsProvider.CardReaderSettings.CardReader),
                        defaultExitRoute,
                        defaultExitRoute)
                {
                }
            }

            private readonly QuickSetUnsetSceneGroup _parentSceneGroup;

            public SetTypeQuestionSceneGroup(
                QuickSetUnsetSceneGroup parentSceneGroup)
                : this(
                    new DelayedCrMenuSceneProvider(),
                    parentSceneGroup)
            {
            }

            private SetTypeQuestionSceneGroup(
                [NotNull] DelayedCrMenuSceneProvider sceneProvider,
                QuickSetUnsetSceneGroup parentSceneGroup)
                : base(
                    sceneProvider,
                    parentSceneGroup.DefaultGroupExitRoute)
            {
                _parentSceneGroup = parentSceneGroup;

                var menuItemsProvider =
                    new MenuItemsProvider(
                        this,
                        sceneProvider);

                sceneProvider.Instance =
                    new Scene(
                        menuItemsProvider,
                        DefaultGroupExitRoute);

                TimeOutGroupExitRoute = new CrSceneGroupExitRoute(
                    this,
                    parentSceneGroup.TimeOutGroupExitRoute);
            }

            public ACardReaderSettings CardReaderSettings
            {
                get { return _parentSceneGroup.CardReaderSettings; }
                
            }

            public AccessDataBase AccessData
            {
                get { return _parentSceneGroup.AccessData; }
            }

            public CrSceneGroupExitRoute TimeOutGroupExitRoute
            {
                get; private set;
            }
        }

        private class ActionScene : CrBaseScene
        {
            private readonly QuickSetUnsetSceneGroup _quickSetUnsetSceneGroup;

            public ActionScene(QuickSetUnsetSceneGroup quickSetUnsetSceneGroup)
            {
                _quickSetUnsetSceneGroup = quickSetUnsetSceneGroup;
            }

            private ACrSceneRoute GetPlannedActionRoute()
            {
                var alarmAreaInfo = _quickSetUnsetSceneGroup._crAlarmAreaInfo;

                if (alarmAreaInfo.IsSet)
                    return
                        new UnsetAlarmAreaRouteProvider(_quickSetUnsetSceneGroup.DefaultGroupExitRoute)
                            .GetInstance(
                                _quickSetUnsetSceneGroup._crAlarmAreaInfo, 
                                _quickSetUnsetSceneGroup);

                if (_quickSetUnsetSceneGroup.CardReaderSettings.IsPremium
                    && alarmAreaInfo.AlarmArea.PreWarning
                    && (_quickSetUnsetSceneGroup.AccessData.EntryViaCard
                        || _quickSetUnsetSceneGroup.AccessData.EntryViaPersonalCode)
                    && AlarmAreaAccessRightsManager.Singleton.CheckRigthsToUnconditionalSet(
                        _quickSetUnsetSceneGroup.AccessData,
                        alarmAreaInfo.IdAlarmArea))
                {
                    return
                        new SetTypeQuestionSceneGroup(_quickSetUnsetSceneGroup)
                            .EnterRoute;
                }

                return
                    new SetAlarmAreaRouteProvider(
                            false,
                            _quickSetUnsetSceneGroup.DefaultGroupExitRoute)
                        .GetInstance(
                            _quickSetUnsetSceneGroup._crAlarmAreaInfo,
                            _quickSetUnsetSceneGroup);
            }

            public override bool OnEntered(ACrSceneContext crSceneContext)
            {
                crSceneContext.PlanDelayedRouteFollowing(
                    this,
                    _quickSetUnsetSceneGroup,
                    GetPlannedActionRoute());

                return true;
            }
        }

        private readonly CrAlarmAreasManager.CrAlarmAreaInfo _crAlarmAreaInfo;

        public QuickSetUnsetSceneGroup(
            CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo,
            ICcuSceneGroup rootSceneGroup)
            : this(
                new DelayedInitReference<QuickSetUnsetSceneGroup>(), 
                crAlarmAreaInfo,
                rootSceneGroup)
        {
        }

        private QuickSetUnsetSceneGroup(
            DelayedInitReference<QuickSetUnsetSceneGroup> delayedInitReference,
            CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo,
            ICcuSceneGroup rootSceneGroup)
            : this(
                new AuthorizationProcessClass(delayedInitReference), 
                crAlarmAreaInfo,
                rootSceneGroup)
        {
            delayedInitReference.Instance = this;
        }

        private QuickSetUnsetSceneGroup(
            AuthorizationProcessClass authorizationProcess,
            CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo,
            ICcuSceneGroup rootSceneGroup)
            : base(
                authorizationProcess,
                rootSceneGroup.DefaultGroupExitRoute,
                rootSceneGroup.CardReaderSettings)
        {
            _crAlarmAreaInfo = crAlarmAreaInfo;
        }

        protected override AAuthorizationSceneGroup CreateInnerAuthorizationSceneGroup()
        {
            return new AuthorizationSceneGroup(this);
        }

        public AccessDataBase AccessData { get; private set; }

        public CrSceneGroupExitRoute TimeOutGroupExitRoute
        {
            get { return DefaultGroupExitRoute; }
        }

        protected override ICrScene CreateAuthorizedScene()
        {
            if (SceneAuthorizationProcess.AuthorizationProcessState
                    == AuthorizationProcessState.Rejected)
            {
                return new RejectedScene(DefaultGroupExitRoute);
            }

            AccessData = SceneAuthorizationProcess.AccessData;

            return new ActionScene(this);
        }
    }
}
