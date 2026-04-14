using System;

using Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Menu;

using CrSceneFrameworkCF;
using CrSceneFrameworkCF.Scenes;

using JetBrains.Annotations;

namespace Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Access
{
    internal class AccessSceneGroup :
        ACrAuthorizationSceneGroup,
        ICcuSceneGroup,
        IInstanceProvider<AccessSceneGroup>
    {
        private class WaitingForPinScene : CrSceneDecorator
        {
            private readonly ACardReaderSettings _cardReaderSettings;

            public WaitingForPinScene(AccessSceneGroup accessSceneGroup)
                : this(
                    accessSceneGroup, 
                    accessSceneGroup._rootSceneGroup.CardReaderSettings)
            {
            }

            private WaitingForPinScene(
                AccessSceneGroup accessSceneGroup,
                ACardReaderSettings cardReaderSettings)
                : base(
                    CcuCardReaders.IsPinConfirmationObligatory
                        ? new CrWaitForPinScene(
                            CcuCardReaders.MinimalPinLength,
                            CcuCardReaders.MaximalPinLength,
                            accessSceneGroup._accessAuthorizationProcess.PinLength,
                            true,
                            new HomeMenuSceneGroup(
                                cardReaderSettings,
                                accessSceneGroup.DefaultGroupExitRoute).EnterRoute)
                        : new CrWaitForPinScene(
                            accessSceneGroup._accessAuthorizationProcess.PinLength,
                            accessSceneGroup._accessAuthorizationProcess.PinLength,
                            accessSceneGroup._accessAuthorizationProcess.PinLength,
                            false,
                            new HomeMenuSceneGroup(
                                cardReaderSettings,
                                accessSceneGroup.DefaultGroupExitRoute).EnterRoute))
            {
                _cardReaderSettings = cardReaderSettings;
            }

            public override bool OnEntered(ACrSceneContext crSceneContext)
            {
                var doorEnvironmentAdapter = _cardReaderSettings.DoorEnvironmentAdapter;

                if (doorEnvironmentAdapter != null)
                    doorEnvironmentAdapter.SuppressCardReader();

                return base.OnEntered(crSceneContext);
            }
        }

        private readonly AAccessAuthorizationProcess _accessAuthorizationProcess;

        private readonly RootSceneGroup _rootSceneGroup;

        public AccessSceneGroup(
            RootSceneGroup rootSceneGroup)
            : this(
                rootSceneGroup.CardReaderSettings.DoorEnvironmentAdapter
                    .CreateAccessAuthorizationProcess(),
                rootSceneGroup)
        {
        }

        private AccessSceneGroup(
            [NotNull] AAccessAuthorizationProcess authorizationProcess,
            RootSceneGroup rootSceneGroup)
            : base(
                authorizationProcess,
                rootSceneGroup.DefaultGroupExitRoute,
                CrSceneAdvanceRoute.Default, 
                CrSceneAdvanceRoute.Default, 
                rootSceneGroup.DefaultGroupExitRoute,
                rootSceneGroup.DefaultGroupExitRoute)
        {
            _rootSceneGroup = rootSceneGroup;

            _accessAuthorizationProcess = authorizationProcess;

            GrantedGroupExitRoute.BeforeExit += OnGrantedRouteExiting;

            CancelledGroupExitRoute.BeforeExit += OnCancelledRouteExiting;
        }

        private void OnCancelledRouteExiting(ACrSceneContext obj)
        {
            if (_accessAuthorizationProcess.AccessData == null
                || _accessAuthorizationProcess.AccessData.IdCard == Guid.Empty)
            {
                _rootSceneGroup.CardReaderSettings.DoorEnvironmentAdapter.ForceLooseCardReader();
            }
        }

        public AuthorizationProcessState AuthorizationProcessState
        {
            get { return _accessAuthorizationProcess.AuthorizationProcessState; }
        }

        private void OnGrantedRouteExiting(ACrSceneContext crSceneContext)
        {
            _rootSceneGroup.CardReaderSettings.DoorEnvironmentAdapter
                .OnAccessGranted(_accessAuthorizationProcess.AccessData);
        }

        protected override ICrScene GetSceneWaitingForCard()
        {
            return
                new ImplicitCodeScene(
                    _rootSceneGroup.CardReaderSettings,
                    new BaseRootScene(this));
        }

        protected override ICrScene GetSceneWaitingForCode()
        {
            return
                new ImplicitCodeScene(
                    _rootSceneGroup.CardReaderSettings,
                    new BaseRootScene(this));
        }

        protected override ICrScene GetSceneWaitingForPin()
        {
            _supportsCancelBySpecialKey = true;

            return new WaitingForPinScene(this);
        }

        private bool _supportsCancelBySpecialKey;

        public override bool SupportsCancelBySpecialKey
        {
            get { return _supportsCancelBySpecialKey; }
        }

        AccessSceneGroup IInstanceProvider<AccessSceneGroup>.Instance
        {
            get { return this; }
        }

        public ACardReaderSettings CardReaderSettings
        {
            get { return _rootSceneGroup.CardReaderSettings; }
        }

        public override bool IsTopMost
        {
            get { return true; }
        }
    }
}
