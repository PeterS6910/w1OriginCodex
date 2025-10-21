using CrSceneFrameworkCF;

using JetBrains.Annotations;

namespace Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Menu.Common
{
    internal class FilteredAlarmAreasSceneGroupBase<
        TRootAlarmAreasMenuSceneGroup,
        TAlarmAreaAccessEventHandler> :
            CrSimpleSceneGroup,
            IAuthorizedSceneGroup
        where TRootAlarmAreasMenuSceneGroup : ARootAlarmAreasSceneGroup<
            TRootAlarmAreasMenuSceneGroup,
            TAlarmAreaAccessEventHandler>
        where TAlarmAreaAccessEventHandler :
            class,
            IAlarmAreaAccessEventHandler
    {
        private readonly TRootAlarmAreasMenuSceneGroup _rootAlarmAreasSceneGroup;

        protected FilteredAlarmAreasSceneGroupBase(
            TRootAlarmAreasMenuSceneGroup rootAlarmAreasSceneGroup,
            [NotNull] IInstanceProvider<ICrScene> sceneProvider)
            : base(
                sceneProvider,
                CrSceneGroupReturnRoute.Default)
        {
            _rootAlarmAreasSceneGroup = rootAlarmAreasSceneGroup;

            TimeOutGroupExitRoute = new CrSceneGroupExitRoute(
                this,
                rootAlarmAreasSceneGroup.TimeOutGroupExitRoute);
        }

        public IAuthorizedSceneGroup Instance
        {
            get { return this; }
        }

        public ACardReaderSettings CardReaderSettings
        {
            get { return _rootAlarmAreasSceneGroup.CardReaderSettings; }
        }

        public AccessDataBase AccessData
        {
            get { return _rootAlarmAreasSceneGroup.SceneAuthorizationProcess.AccessData; }
        }

        public CrSceneGroupExitRoute TimeOutGroupExitRoute { get; private set; }
    }
}
