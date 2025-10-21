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
    public interface ICrSceneGroup
    {
        void OnEntered(ACrSceneContext crSceneContext);
        void OnReturned(ACrSceneContext crSceneContext);

        void OnExiting(ACrSceneContext crSceneContext);
        void OnDescending(ACrSceneContext crSceneContext);

        void OnCardSwiped(ACrSceneContext crSceneContext, string cardData, int cardSystemNumber);

        void OnCodeSpecified(ACrSceneContext crSceneContext, string codeData);
        void OnCodeTimedOut(ACrSceneContext crSceneContext);

        void OnMenuCancelled(ACrSceneContext crSceneContext, bool byOtherCommand);
        void OnMenuTimeOut(ACrSceneContext crSceneContext);
        void OnMenuItemSelected(ACrSceneContext crSceneContext, int itemIndex);

        void OnFnBoxTimedOut(ACrSceneContext aCrSceneContext);

        void OnNumericKeyPressed(ACrSceneContext crSceneContext, byte numeric);
        void OnSpecialKeyPressed(ACrSceneContext crSceneContext, CRSpecialKey specialKey);

        bool IsActive
        {
            get;
        }

        bool IsTopMost
        {
            get;
        }

        void Advance(ACrSceneContext crSceneContext);

        CrSceneGroupExitRoute DefaultGroupExitRoute
        {
            get;
        }

        bool IsCurrentScene(ICrScene requiredScene);
    }
}