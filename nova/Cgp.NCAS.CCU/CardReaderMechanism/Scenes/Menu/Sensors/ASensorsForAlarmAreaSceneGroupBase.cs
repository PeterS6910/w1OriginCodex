using Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Menu.Common;
using Contal.Cgp.NCAS.CCU.EventParameters;

using CrSceneFrameworkCF;
using CrSceneFrameworkCF.Scenes;

namespace Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Menu.Sensors
{
    internal interface ISensorsForAlarmAreaSceneGroup : IAuthorizedSceneGroup
    {
        AlarmAreaSensorListener AlarmAreaSensorListener
        {
            get;
        }

        CrAlarmAreasManager.CrAlarmAreaInfo CrAlarmAreaInfo
        {
            get;
        }
    }

    internal abstract class ASensorsForAlarmAreaSceneGroupBase<TMenuItemsProvider> :
        AAlarmAreaSceneGroupBase,
        ISensorsForAlarmAreaSceneGroup
        where TMenuItemsProvider : SensorsForAlarmAreaMenuScene<TMenuItemsProvider>.AMenuItemsProvider
    {
        public AlarmAreaSensorListener AlarmAreaSensorListener
        {
            get;
            private set;
        }

        protected ASensorsForAlarmAreaSceneGroupBase(
            AlarmAreaSensorListener alarmAreaSensorListener,
            IAuthorizedSceneGroup parentSceneGroup)
            : base(
                parentSceneGroup,
                CrSceneGroupReturnRoute.Default)
        {
            AlarmAreaSensorListener = alarmAreaSensorListener;
        }

        protected abstract TMenuItemsProvider GetMenuItemsProvider(
            IInstanceProvider<CrMenuScene> menuSceneProvider);

        protected override ICrScene CreateMenuScene()
        {
            var menuSceneProvider =
                new DelayedInitReference<CrMenuScene>();

            var result =
                new SensorsForAlarmAreaMenuScene<TMenuItemsProvider>(
                    this,
                    GetMenuItemsProvider(menuSceneProvider));

            menuSceneProvider.Instance = result;

            return result;
        }

        public override CrAlarmAreasManager.CrAlarmAreaInfo CrAlarmAreaInfo
        {
            get { return AlarmAreaSensorListener.CrAlarmAreaInfo; }
        }
    }
}
