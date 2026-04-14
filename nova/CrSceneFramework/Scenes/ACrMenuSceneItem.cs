using System;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;
#if COMPACT_FRAMEWORK
using Contal.Drivers.CardReader;
#else
using Contal.Drivers.CardReader;
#endif

#if COMPACT_FRAMEWORK
namespace CrSceneFrameworkCF.Scenes
#else
namespace CrSceneFramework.Scenes
#endif
{
    public abstract class ACrMenuSceneItem<TCrMenuSceneItemsProvider>
        : IEquatable<ACrMenuSceneItem<TCrMenuSceneItemsProvider>>
        where TCrMenuSceneItemsProvider : ICrMenuSceneItemsProvider
    {
        private readonly IInstanceProvider<ACrSceneRoute, TCrMenuSceneItemsProvider> _selectedRouteProvider;

        protected ACrMenuSceneItem(
            [NotNull]
            IInstanceProvider<ACrSceneRoute, TCrMenuSceneItemsProvider> selectedRouteProvider)
        {
            ReturnCode = 0xFFFF;
            _selectedRouteProvider = selectedRouteProvider;
        }

        private volatile CRMenuItem _crMenuItem;

        private readonly object _crMenuItemLock = new object();

        internal ushort ReturnCode
        {
            get;
            set;
        }

        protected virtual string GetText(TCrMenuSceneItemsProvider menuItemsProvider)
        {
            return string.Empty;
        }

        protected virtual ushort GetGraphicIndexUshort(TCrMenuSceneItemsProvider menuItemsProvider)
        {
            return CRMenuCommands.GraphicalMenuNoSymbol;
        }

        protected virtual IEnumerable<ushort> GetInlinedIconsUshort(TCrMenuSceneItemsProvider menuItemsProvider)
        {
            return null;
        }

        protected CRMenuItem CreateCRMenuItem(TCrMenuSceneItemsProvider menuItemsProvider)
        {
            var icons = GetInlinedIconsUshort(menuItemsProvider);

            return new CRMenuItem(
                GetText(menuItemsProvider),
                GetGraphicIndexUshort(menuItemsProvider),
                icons != null
                    ? icons.ToArray()
                    : null,
                ReturnCode);
        }

        public CRMenuItem GetCRMenuItem(TCrMenuSceneItemsProvider menuItemsProvider)
        {
            if (_crMenuItem == null)
                lock (_crMenuItemLock)
                    if (_crMenuItem == null)
                        _crMenuItem = CreateCRMenuItem(menuItemsProvider);

            return _crMenuItem;
        }

        public void Update(TCrMenuSceneItemsProvider menuItemsProvider)
        {
            lock (_crMenuItemLock)
                _crMenuItem = CreateCRMenuItem(menuItemsProvider);
        }

        public void OnSelected(
            ACrSceneContext aCrSceneContext,
            TCrMenuSceneItemsProvider crMenuSceneItemsProvider)
        {
            var selectedRoute =
                _selectedRouteProvider.GetInstance(crMenuSceneItemsProvider)
                    ?? CrSceneGroupReturnRoute.Default;

            if (selectedRoute != null)
                selectedRoute.Follow(aCrSceneContext);
        }

        public virtual bool IsVisible(TCrMenuSceneItemsProvider menuItemsProvider)
        {
            return true;
        }

        public bool Equals(ACrMenuSceneItem<TCrMenuSceneItemsProvider> other)
        {
            return 
                other != null &&
                ReturnCode.Equals(other.ReturnCode);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ACrMenuSceneItem<TCrMenuSceneItemsProvider>);
        }

        public override int GetHashCode()
        {
            return ReturnCode.GetHashCode();
        }
    }
}