using System.Collections.Generic;

using CrSceneFrameworkCF;

namespace Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Menu.Common
{
    internal abstract class AAlarmAreaSceneGroupBase :
        ACrSequentialSceneGroup,
        IAuthorizedSceneGroup
    {
        private readonly IAuthorizedSceneGroup _parentSceneGroup;

        public abstract CrAlarmAreasManager.CrAlarmAreaInfo CrAlarmAreaInfo
        {
            get;
        }

        protected AAlarmAreaSceneGroupBase(
            IAuthorizedSceneGroup parentSceneGroup,
            IInstanceProvider<ACrSceneRoute> parentDefaultRouteProvider)
            : base(parentDefaultRouteProvider)
        {
            _parentSceneGroup = parentSceneGroup;

            TimeOutGroupExitRoute = new CrSceneGroupExitRoute(
                this,
                _parentSceneGroup.TimeOutGroupExitRoute);
        }

        protected override IEnumerable<ICrScene> Scenes
        {
            get
            {
                yield return CreateMenuScene();
            }
        }

        protected abstract ICrScene CreateMenuScene();

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
}
