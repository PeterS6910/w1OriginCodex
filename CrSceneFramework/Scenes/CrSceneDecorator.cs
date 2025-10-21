using JetBrains.Annotations;

#if COMPACT_FRAMEWORK
using Contal.Drivers.CardReader;
#else
using Contal.Drivers.CardReader;
#endif

#if COMPACT_FRAMEWORK
namespace CrSceneFrameworkCF.Scenes
#else
namespace CrSceneFramework.Scenes
#endif
{
    public class CrSceneDecorator : ICrScene
    {
        private readonly ICrScene _decoratedScene;

        public CrSceneDecorator(
            [NotNull]
            ICrScene decoratedScene)
        {
            _decoratedScene = decoratedScene;
        }

        public virtual bool OnEntered(ACrSceneContext crSceneContext)
        {
            return _decoratedScene.OnEntered(crSceneContext);
        }

        public virtual void OnExiting(ACrSceneContext crSceneContext)
        {
            _decoratedScene.OnExiting(crSceneContext);
        }

        public virtual void OnAdvancing(ACrSceneContext crSceneContext)
        {
            _decoratedScene.OnAdvancing(crSceneContext);
        }

        public virtual void OnDescending(ACrSceneContext crSceneContext)
        {
            _decoratedScene.OnDescending(crSceneContext);
        }

        public virtual void OnMenuCancelled(
            ACrSceneContext crSceneContext,
            bool byOtherCommand)
        {
            _decoratedScene.OnMenuCancelled(
                crSceneContext, 
                byOtherCommand);
        }

        public virtual void OnMenuTimedOut(ACrSceneContext crSceneContext)
        {
            _decoratedScene.OnMenuTimedOut(crSceneContext);
        }

        public virtual void OnMenuItemSelected(
            ACrSceneContext crSceneContext,
            int itemIndex)
        {
            _decoratedScene.OnMenuItemSelected(
                crSceneContext,
                itemIndex);
        }

        public virtual void OnFnBoxTimedOut(ACrSceneContext crSceneContext)
        {
            _decoratedScene.OnFnBoxTimedOut(crSceneContext);
        }

        public virtual void OnCardSwiped(
            ACrSceneContext crSceneContext,
            string cardData,
            int cardSystemNumber)
        {
            _decoratedScene.OnCardSwiped(
                crSceneContext,
                cardData,
                cardSystemNumber);
        }

        public virtual void OnCodeSpecified(
            ACrSceneContext crSceneContext,
            string codeData)
        {
            _decoratedScene.OnCodeSpecified(
                crSceneContext,
                codeData);
        }

        public virtual void OnReturned(ACrSceneContext crSceneContext)
        {
            _decoratedScene.OnReturned(crSceneContext);
        }

        public virtual void OnSpecialKeyPressed(
            ACrSceneContext crSceneContext,
            CRSpecialKey specialKey)
        {
            _decoratedScene.OnSpecialKeyPressed(
                crSceneContext,
                specialKey);
        }

        public virtual void OnNumericKeyPressed(
            ACrSceneContext crSceneContext,
            byte numeric)
        {
            _decoratedScene.OnNumericKeyPressed(
                crSceneContext,
                numeric);
        }

        public virtual void OnCodeTimedOut(ACrSceneContext crSceneContext)
        {
            _decoratedScene.OnCodeTimedOut(crSceneContext);
        }

        public ICrScene Instance
        {
            get { return this; }
        }
    }
}