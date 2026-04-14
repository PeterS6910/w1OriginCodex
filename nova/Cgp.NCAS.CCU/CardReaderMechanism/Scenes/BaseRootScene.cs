using Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.FunctionKeys;
using Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Menu;
using Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Menu.AlarmAreas;
using Contal.Drivers.CardReader;

using CrSceneFrameworkCF;
using CrSceneFrameworkCF.Scenes;

namespace Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes
{
    internal class BaseRootScene : CrBaseScene
    {
        private readonly ICcuSceneGroup _sceneGroup;

        public BaseRootScene(ICcuSceneGroup sceneGroup)
        {
            _sceneGroup = sceneGroup;
        }

        protected ACardReaderSettings CardReaderSettings
        {
            get { return _sceneGroup.CardReaderSettings; }
        }

        public override void OnSpecialKeyPressed(
            ACrSceneContext crSceneContext,
            CRSpecialKey specialKey)
        {
            var cardReaderMechanism = _sceneGroup.CardReaderSettings;

            switch (specialKey)
            {
                case CRSpecialKey.FunctionKey1:

                    new FunctionKeySceneGroup(
                        CrSceneGroupReturnRoute.Default,
                        cardReaderMechanism,
                        cardReaderMechanism.CardReaderDb.FunctionKey1).EnterRoute.Follow(
                            crSceneContext);

                    break;

                case CRSpecialKey.FunctionKey2:

                    new FunctionKeySceneGroup(
                        CrSceneGroupReturnRoute.Default,
                        cardReaderMechanism,
                        cardReaderMechanism.CardReaderDb.FunctionKey2).EnterRoute.Follow(
                            crSceneContext);

                    break;

                case CRSpecialKey.Up:

                    new HomeMenuSceneGroup(
                        cardReaderMechanism,
                        _sceneGroup.DefaultGroupExitRoute).EnterRoute.Follow(crSceneContext);

                    break;

                case CRSpecialKey.Yes:

                    if (!cardReaderMechanism.IsPremium)
                        StartQuickSetUnsetMenuSceneGroup(
                            crSceneContext,
                            false);

                    break;

                case CRSpecialKey.Unlock:

                    StartQuickSetUnsetMenuSceneGroup(
                        crSceneContext,
                        false);

                    break;

                case CRSpecialKey.Lock:

                    StartQuickSetUnsetMenuSceneGroup(
                        crSceneContext,
                        true);

                    break;

                case CRSpecialKey.No:

                    if (!cardReaderMechanism.IsPremium)
                        StartQuickSetUnsetMenuSceneGroup(
                            crSceneContext,
                            true);

                    break;
            }
        }

        private void StartQuickSetUnsetMenuSceneGroup(
            ACrSceneContext crSceneContext,
            bool startWhenUnset)
        {
            var cardReaderMechanism = _sceneGroup.CardReaderSettings;

            var implicitAlarmAreaInfo = cardReaderMechanism.ImplicitCrAlarmAreaInfo;

            if (implicitAlarmAreaInfo != null
                && (startWhenUnset
                    ? implicitAlarmAreaInfo.IsSettable
                      && implicitAlarmAreaInfo.IsUnset
                    : implicitAlarmAreaInfo.IsUnsettable
                      && implicitAlarmAreaInfo.IsSet))
            {
                new QuickSetUnsetSceneGroup(
                    implicitAlarmAreaInfo,
                    _sceneGroup)
                    .EnterRoute
                    .Follow(crSceneContext);
            }
        }
    }
}