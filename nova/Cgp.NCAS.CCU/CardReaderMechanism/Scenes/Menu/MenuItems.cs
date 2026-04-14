using System.Collections.Generic;
using System.Linq;

using CrSceneFrameworkCF;
using CrSceneFrameworkCF.Scenes;

using JetBrains.Annotations;

namespace Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Menu
{
    internal abstract class ACcuMenuItem<TMenuItemsProvider> : ACrMenuSceneItem<TMenuItemsProvider>
        where TMenuItemsProvider : ICrMenuSceneItemsProvider
    {
        protected ACcuMenuItem(
            [NotNull] IInstanceProvider<ACrSceneRoute, TMenuItemsProvider> selectedRouteProvider)
            : base(selectedRouteProvider)
        {
        }

        protected sealed override ushort GetGraphicIndexUshort(TMenuItemsProvider menuItemsProvider)
        {
            return (ushort)GetGraphicIndex(menuItemsProvider);
        }

        protected virtual CrIconSymbol GetGraphicIndex(TMenuItemsProvider menuItemsProvider)
        {
            return CrIconSymbol.NoSymbol;
        }

        protected sealed override IEnumerable<ushort> GetInlinedIconsUshort(TMenuItemsProvider menuItemsProvider)
        {
            var result = GetInlinedIcons(menuItemsProvider);

            return result != null
                ? result.Cast<ushort>()
                : null;
        }

        protected virtual IEnumerable<CrIconSymbol> GetInlinedIcons(TMenuItemsProvider menuItemsProvider)
        {
            return null;
        }
    }

    internal interface ICcuMenuItemsProvider : ICrMenuSceneItemsProvider
    {
        ACardReaderSettings CardReaderSettings
        {
            get;
        }
    }

    internal abstract class ALocalizedMenuItem<TMenuItemsProvider> : ACcuMenuItem<TMenuItemsProvider>
        where TMenuItemsProvider : ICcuMenuItemsProvider
    {
        protected ALocalizedMenuItem(
            [NotNull] IInstanceProvider<ACrSceneRoute, TMenuItemsProvider> selectedRouteProvider)
            : base(selectedRouteProvider)
        {
        }

        protected abstract string GetLocalizationKey(TMenuItemsProvider menuItemsProvider);

        protected override string GetText(TMenuItemsProvider menuItemsProvider)
        {
            return CcuCore.Singleton.LocalizationHelper.GetString(
                GetLocalizationKey(menuItemsProvider),
                menuItemsProvider.CardReaderSettings.CardReader.Language.ToString());
        }
    }
}
