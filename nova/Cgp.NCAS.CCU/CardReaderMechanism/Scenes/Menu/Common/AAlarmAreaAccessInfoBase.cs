namespace Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Menu.Common
{
    internal abstract class AAlarmAreaAccessInfoBase
    {
        public CrAlarmAreasManager.CrAlarmAreaInfo CrAlarmAreaInfo
        {
            get;
            private set;
        }

        protected AAlarmAreaAccessInfoBase(
            CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo,
            IAlarmAreaAccessManager alarmAreaAccessManager)
        {
            CrAlarmAreaInfo = crAlarmAreaInfo;

            UpdateVisibility(alarmAreaAccessManager);
        }

        public bool IsVisible
        {
            get;
            private set;
        }

        public void UpdateVisibility(
            IAlarmAreaAccessManager accessManager)
        {
            var accessData = accessManager.AccessData;

            IsVisible =
                (!accessData.EntryViaCard
                 && !accessData.EntryViaPersonalCode)
                || CheckPersonAccessRights(accessManager);
        }

        protected abstract bool CheckPersonAccessRights(IAlarmAreaAccessManager accessManager);
    }
}