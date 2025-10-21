using Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Menu.Common;

using CrSceneFrameworkCF;

namespace Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Menu.Sensors
{
    internal class CategorizedSensorsAccessManager : 
        AAlarmAreaAccessManagerBase<CategorizedSensorsAccessManager.IEventHandler>
    {
        public interface IEventHandler
            : IAlarmAreaAccessEventHandler
        {
            void OnAnySensorInAlarmChanged(
                AAlarmAreaAccessInfoBase alarmAreaAccessInfo,
                bool value);

            void OnAnySensorInTamperChanged(
                AAlarmAreaAccessInfoBase alarmAreaAccessInfo,
                bool value);

            void OnAnySensorNotAcknowledgedChanged(
                AAlarmAreaAccessInfoBase alarmAreaAccessInfo,
                bool value);

            void OnAnySensorPermanentlyBlockedChanged(
                AAlarmAreaAccessInfoBase alarmAreaAccessInfo,
                bool value);

            void OnAnySensorTemporarilyBlockedChanged(
                AAlarmAreaAccessInfoBase alarmAreaAccessInfo,
                bool value);
        }

        public CategorizedSensorsAccessManager(IInstanceProvider<IAuthorizedSceneGroup> sceneGroupProvider)
            : base(sceneGroupProvider)
        {
        }

        protected override AAlarmAreaAccessInfoBase CreateAlarmAreaAccessInfo(
            CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo)
        {
            return new CategorizedSensorsAccessInfo(
                crAlarmAreaInfo,
                this);
        }

        public override void OnAnySensorInAlarmChanged(
            CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo,
            bool value)
        {
            AAlarmAreaAccessInfoBase alarmAreaAccessInfo;

            if (!AlarmAreaAccessInfos.TryGetValue(
                crAlarmAreaInfo,
                out alarmAreaAccessInfo))
            {
                return;
            }

            EventHandlerGroup.ForEach(
                eventHandler => eventHandler.OnAnySensorInAlarmChanged(
                    alarmAreaAccessInfo,
                    value));
        }

        public override void OnAnySensorInTamperChanged(
            CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo,
            bool value)
        {
            AAlarmAreaAccessInfoBase alarmAreaAccessInfo;

            if (!AlarmAreaAccessInfos.TryGetValue(
                crAlarmAreaInfo,
                out alarmAreaAccessInfo))
            {
                return;
            }

            EventHandlerGroup.ForEach(
                eventHandler => eventHandler.OnAnySensorInTamperChanged(
                    alarmAreaAccessInfo,
                    value));
        }

        public override void OnAnySensorNotAcknowledgedChanged(
            CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo,
            bool value)
        {
            AAlarmAreaAccessInfoBase alarmAreaAccessInfo;

            if (!AlarmAreaAccessInfos.TryGetValue(
                crAlarmAreaInfo,
                out alarmAreaAccessInfo))
            {
                return;
            }

            EventHandlerGroup.ForEach(
                eventHandler => eventHandler.OnAnySensorNotAcknowledgedChanged(
                    alarmAreaAccessInfo,
                    value));
        }

        public override void OnAnySensorPermanentlyBlockedChanged(
            CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo,
            bool value)
        {
            AAlarmAreaAccessInfoBase alarmAreaAccessInfo;

            if (!AlarmAreaAccessInfos.TryGetValue(
                crAlarmAreaInfo,
                out alarmAreaAccessInfo))
            {
                return;
            }

            EventHandlerGroup.ForEach(
                eventHandler => eventHandler.OnAnySensorPermanentlyBlockedChanged(
                    alarmAreaAccessInfo,
                    value));
        }

        public override void OnAnySensorTemporarilyBlockedChanged(
            CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo,
            bool value)
        {
            AAlarmAreaAccessInfoBase alarmAreaAccessInfo;

            if (!AlarmAreaAccessInfos.TryGetValue(
                crAlarmAreaInfo,
                out alarmAreaAccessInfo))
            {
                return;
            }

            EventHandlerGroup.ForEach(
                eventHandler => eventHandler.OnAnySensorTemporarilyBlockedChanged(
                    alarmAreaAccessInfo,
                    value));
        }
    }
}