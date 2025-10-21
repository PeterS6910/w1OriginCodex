using CrSceneFrameworkCF;
using CrSceneFrameworkCF.Scenes;

namespace Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Menu.Common
{
    internal class DelayedCrMenuSceneProvider
        : DelayedInitReference<CrMenuScene>,
            IInstanceProvider<ICrScene>
    {
        ICrScene IInstanceProvider<ICrScene>.Instance
        {
            get { return Instance; }
        }
    }

    internal interface IAuthorizedSceneGroup
    {
        ACardReaderSettings CardReaderSettings
        {
            get;
        }

        AccessDataBase AccessData
        {
            get;
        }

        CrSceneGroupExitRoute DefaultGroupExitRoute
        {
            get;
        }

        CrSceneGroupExitRoute TimeOutGroupExitRoute
        {
            get;
        }
    }
}
