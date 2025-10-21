using System.Collections.Generic;

namespace Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Menu.AlarmAreas
{
    internal abstract class ASetAlarmAreaMenuItemBase<TMenuItemsProvider> :
        ALocalizedMenuItem<TMenuItemsProvider>
        where TMenuItemsProvider : ICcuMenuItemsProvider, ISetUnsetAlarmAreaContext
    {
        protected ASetAlarmAreaMenuItemBase(
            ASetAlarmAreaRouteProviderBase routeProvider)
            : base(new SetUnsetRouteProviderAdapter<TMenuItemsProvider>(routeProvider))
        {
        }

        protected override IEnumerable<CrIconSymbol> GetInlinedIcons(TMenuItemsProvider menuItemsProvider)
        {
            yield return CrIconSymbol.SetAlarmArea;
        }
    }
}
