using Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Menu.Common;

using CrSceneFrameworkCF;

namespace Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Menu.AlarmAreas
{
    internal class AlarmAreaAccessManager : 
        AAlarmAreaAccessManagerBase<IAlarmAreaAccessEventHandler>
    {
        public AlarmAreaAccessManager(IInstanceProvider<IAuthorizedSceneGroup> sceneGroupProvider)
            : base(sceneGroupProvider)
        {
        }

        protected override AAlarmAreaAccessInfoBase CreateAlarmAreaAccessInfo(CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo)
        {
            return new AlarmAreaAccessInfo(
                crAlarmAreaInfo,
                this);
        }
    }
}