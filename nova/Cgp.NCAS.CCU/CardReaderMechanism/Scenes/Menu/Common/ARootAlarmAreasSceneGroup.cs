using System.Linq;
using Contal.Drivers.CardReader;
using CrSceneFrameworkCF;
using JetBrains.Annotations;

namespace Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Menu.Common
{
    internal abstract class ARootAlarmAreasSceneGroup<TRootAlarmAreasSceneGroup, TAlarmAreaAccessEventHandler> :
        ASceneAuthorizationSceneGroupByEntrySL<TRootAlarmAreasSceneGroup>,
        IAuthorizedSceneGroup
        where TRootAlarmAreasSceneGroup : ARootAlarmAreasSceneGroup<TRootAlarmAreasSceneGroup, TAlarmAreaAccessEventHandler>
        where TAlarmAreaAccessEventHandler : class, IAlarmAreaAccessEventHandler
    {
        protected class DelayedInstanceProvider
            : DelayedInitReference<TRootAlarmAreasSceneGroup>,
            IInstanceProvider<IAuthorizedSceneGroup>
        {
            IAuthorizedSceneGroup IInstanceProvider<IAuthorizedSceneGroup>.Instance
            {
                get { return Instance; }
            }
        }

        protected abstract class AAuthorizationProcess
            : ASceneAuthorizationProcessByEntrySL<TRootAlarmAreasSceneGroup>
        {
            protected AAuthorizationProcess(IInstanceProvider<TRootAlarmAreasSceneGroup> sceneGroupProvider)
                : base(sceneGroupProvider)
            {
            }

            protected sealed override bool AuthorizeByCardInternal()
            {
                var categorizedSensorsAccessManager = SceneGroupProvider.Instance.AlarmAreaAccessManager;

                CardReaderSettings.CrAlarmAreasManager.Attach(categorizedSensorsAccessManager);

                return
                    categorizedSensorsAccessManager.SortedAlarmAreaAccessInfos
                        .Any(accessInfo => accessInfo.IsVisible);
            }

            protected sealed override bool AuthorizeByPersonInternal()
            {
                var categorizedSensorsAccessManager = SceneGroupProvider.Instance.AlarmAreaAccessManager;

                CardReaderSettings.CrAlarmAreasManager.Attach(categorizedSensorsAccessManager);

                return
                    categorizedSensorsAccessManager.SortedAlarmAreaAccessInfos
                        .Any(accessInfo => accessInfo.IsVisible);
            }
        }

        protected AAlarmAreaAccessManagerBase<TAlarmAreaAccessEventHandler> AlarmAreaAccessManager;

        protected ARootAlarmAreasSceneGroup(
            AAlarmAreaAccessManagerBase<TAlarmAreaAccessEventHandler> alarmAreaAccessManager,
            ASceneAuthorizationProcessByEntrySL<TRootAlarmAreasSceneGroup> sceneAuthorizationProcess,
            [NotNull] IInstanceProvider<ACrSceneRoute> parentDefaultRouteProvider,
            ACardReaderSettings cardReaderSettings)
            : base(
                sceneAuthorizationProcess,
                parentDefaultRouteProvider,
                cardReaderSettings)
        {
            AlarmAreaAccessManager = alarmAreaAccessManager;
        }

        public AccessDataBase AccessData
        {
            get { return SceneAuthorizationProcess.AccessData; }
        }

        public CrSceneGroupExitRoute TimeOutGroupExitRoute
        {
            get { return DefaultGroupExitRoute; }
        }

        protected class AuthorizationSceneGroup : AAuthorizationSceneGroup
        {
            private class WaitingForPinToMenuScene : AAuthorizationScene
            {
                public WaitingForPinToMenuScene(AAuthorizationSceneGroup sceneGroup)
                    : base(sceneGroup)
                {
                }

                protected override void ShowInternal(CardReader cardReader)
                {
                    if (CcuCardReaders.IsPinConfirmationObligatory)
                        cardReader.AccessCommands.WaitingForPINToMenu(
                            cardReader,
                            CcuCardReaders.MinimalPinLength,
                            CcuCardReaders.MaximalPinLength,
                            SceneGroup.ParentSceneGroup.SceneAuthorizationProcess.PinLength,
                            true);
                    else
                        cardReader.AccessCommands.WaitingForPINToMenu(
                            cardReader,
                            SceneGroup.ParentSceneGroup.SceneAuthorizationProcess.PinLength,
                            SceneGroup.ParentSceneGroup.SceneAuthorizationProcess.PinLength,
                            SceneGroup.ParentSceneGroup.SceneAuthorizationProcess.PinLength,
                            false);
                }
            }

            private class WaitingForCardToMenuScene : AWaitingForCardScene
            {
                public WaitingForCardToMenuScene(AAuthorizationSceneGroup sceneGroup)
                    : base(sceneGroup)
                {
                }

                protected override void ShowInternal(CardReader cardReader)
                {
                    cardReader.AccessCommands.WaitingForCardToMenu(cardReader);
                }
            }

            private class WaitingForCodeToMenuScene : AAuthorizationSceneForcedGinCodeLedPresentation
            {
                public WaitingForCodeToMenuScene(AAuthorizationSceneGroup sceneGroup)
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

            public AuthorizationSceneGroup(TRootAlarmAreasSceneGroup parentSceneGroup)
                : base(parentSceneGroup)
            {
            }

            protected override ICrScene GetSceneWaitingForCode()
            {
                return new WaitingForCodeToMenuScene(this);
            }

            protected override ICrScene GetSceneWaitingForCard()
            {
                return new WaitingForCardToMenuScene(this);
            }

            protected override ICrScene GetSceneWaitingForPin()
            {
                return new WaitingForPinToMenuScene(this);
            }
        }

        protected override AAuthorizationSceneGroup CreateInnerAuthorizationSceneGroup()
        {
            return new AuthorizationSceneGroup(This);
        }

        protected sealed override ICrScene CreateAuthorizedScene()
        {
            CardReaderSettings.CrAlarmAreasManager.Attach(AlarmAreaAccessManager);
            return CreateAuthorizedSceneInternal();
        }

        protected abstract ICrScene CreateAuthorizedSceneInternal();

        protected override void AfterExit(ACrSceneContext crSceneContext)
        {
            CardReaderSettings.CrAlarmAreasManager.Detach(AlarmAreaAccessManager);
        }

        protected abstract TRootAlarmAreasSceneGroup This
        {
            get;
        }
    }
}
