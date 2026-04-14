
#if COMPACT_FRAMEWORK
using Contal.Drivers.CardReader;
#else
using Contal.Drivers.CardReader;
#endif

#if COMPACT_FRAMEWORK
namespace CrSceneFrameworkCF
#else
namespace CrSceneFramework
#endif
{
    public interface ICrScene : IInstanceProvider<ICrScene>
    {
        bool OnEntered(ACrSceneContext crSceneContext);

        void OnReturned(ACrSceneContext crSceneContext);

        void OnExiting(ACrSceneContext crSceneContext);

        void OnAdvancing(ACrSceneContext crSceneContext);

        void OnDescending(ACrSceneContext crSceneContext);

        void OnMenuCancelled(
            ACrSceneContext crSceneContext,
            bool byOtherCommand);

        void OnMenuTimedOut(ACrSceneContext crSceneContext);

        void OnMenuItemSelected(
            ACrSceneContext crSceneContext,
            int itemIndex);

        void OnFnBoxTimedOut(ACrSceneContext crSceneContext);

        void OnCardSwiped(
            ACrSceneContext crSceneContext,
            string cardData,
            int cardSystemNumber);

        void OnCodeSpecified(
            ACrSceneContext crSceneContext, 
            string codeData);

        void OnCodeTimedOut(ACrSceneContext crSceneContext);

        void OnSpecialKeyPressed(
            ACrSceneContext crSceneContext,
            CRSpecialKey specialKey);

        void OnNumericKeyPressed(
            ACrSceneContext crSceneContext,
            byte numeric);
    }
}
