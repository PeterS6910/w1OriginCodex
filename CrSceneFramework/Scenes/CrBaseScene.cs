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
    public class CrBaseScene : ICrScene
    {
        public virtual bool OnEntered(ACrSceneContext crSceneContext)
        {
            return true;
        }

        public virtual void OnExiting(ACrSceneContext crSceneContext)
        {
        }

        public virtual void OnAdvancing(ACrSceneContext crSceneContext)
        {
        }

        public virtual void OnDescending(ACrSceneContext crSceneContext)
        {
        }

        public virtual void OnMenuCancelled(
            ACrSceneContext crSceneContext,
            bool byOtherCommand)
        {
        }

        public virtual void OnMenuTimedOut(ACrSceneContext crSceneContext)
        {
        }

        public virtual void OnMenuItemSelected(
            ACrSceneContext crSceneContext,
            int itemIndex)
        {
        }

        public virtual void OnFnBoxTimedOut(ACrSceneContext crSceneContext)
        {
        }

        public virtual void OnCardSwiped(
            ACrSceneContext crSceneContext,
            string cardData,
            int cardSystemNumber)
        {
        }

        public virtual void OnCodeSpecified(
            ACrSceneContext crSceneContext,
            string codeData)
        {
        }

        public virtual void OnReturned(ACrSceneContext crSceneContext)
        {
        }

        public virtual void OnSpecialKeyPressed(
            ACrSceneContext crSceneContext,
            CRSpecialKey specialKey)
        {
        }

        public virtual void OnNumericKeyPressed(
            ACrSceneContext crSceneContext,
            byte numeric)
        {
        }

        public virtual void OnCodeTimedOut(ACrSceneContext aCrSceneContext)
        {
        }

        public ICrScene Instance
        {
            get { return this; }
        }
    }
}
