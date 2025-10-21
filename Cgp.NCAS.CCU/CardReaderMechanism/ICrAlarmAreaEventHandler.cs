using System.Collections.Generic;

namespace Contal.Cgp.NCAS.CCU.CardReaderMechanism
{
    internal interface ICrAlarmAreaEventHandler
    {
        void OnAttached(ICollection<CrAlarmAreasManager.CrAlarmAreaInfo> observedAlarmAreas);

        void OnActivationStateChanged(CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo);
        void OnAlarmStateChanged(CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo);

        void OnNotAcknolwedgedStateChanged(
            CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo,
            bool notAcknowledged);

        void OnAlarmAreaAdded(CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo);
        void OnAlarmAreaRemoved(CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo);

        void OnAACardReaderRightsChanged(CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo);

        void OnDetached();

        void OnAlarmAreaMarkingChanged(CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo);

        void OnAnySensorInAlarmChanged(
            CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo,
            bool value);

        void OnAnySensorInTamperChanged(
            CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo,
            bool value);

        void OnAnySensorNotAcknowledgedChanged(
            CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo,
            bool value);

        void OnAnySensorTemporarilyBlockedChanged(
            CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo,
            bool value);

        void OnAnySensorPermanentlyBlockedChanged(
            CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo,
            bool value);
    }
}