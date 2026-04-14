using System;
using System.Collections.Generic;
using System.Linq;

using Contal.IwQuick.Threads;

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
    public interface ICrMenuSceneItemsProvider
    {
        void OnEntered();

        void OnExited();

        void OnSelected(
            ACrSceneContext crSceneContext,
            int index);

        int Count
        {
            get;
        }

        IEnumerable<CRMenuItem> Items
        {
            get;
        }

        void OnReturned();
    }

    public abstract class ACrMenuSceneItemsProvider<TCrMenuSceneItemsProvider> : ICrMenuSceneItemsProvider
        where TCrMenuSceneItemsProvider : ACrMenuSceneItemsProvider<TCrMenuSceneItemsProvider>
    {
        private class MenuItemInfo
        {
            public LinkedListNode<MenuItemInfo> GlobalNode;
            public LinkedListNode<MenuItemInfo> VisibleNode;

            public ACrMenuSceneItem<TCrMenuSceneItemsProvider> Item
            {
                get;
                private set;
            }

            public MenuItemInfo(
                [NotNull] ACrMenuSceneItem<TCrMenuSceneItemsProvider> item)
            {
                Item = item;
            }
        }

        private readonly IInstanceProvider<CrMenuScene> _menuSceneProvider;

        private readonly LinkedList<MenuItemInfo> _globalItems = 
            new LinkedList<MenuItemInfo>();

        private readonly LinkedList<MenuItemInfo> _visibleItems =
            new LinkedList<MenuItemInfo>();

        private readonly IDictionary<ushort, MenuItemInfo> _menuItemInfos =
            new Dictionary<ushort, MenuItemInfo>();

        private ushort _maxReturnCode;

        protected ACrMenuSceneItemsProvider(IInstanceProvider<CrMenuScene> menuSceneProvider)
        {
            _menuSceneProvider = menuSceneProvider;
            _maxReturnCode = 100;
        }

        public void OnSelected(
            ACrSceneContext crSceneContext,
            int idx)
        {
            MenuItemInfo menuItemInfo;

            if (_menuItemInfos.TryGetValue(
                (ushort)idx,
                out menuItemInfo))
            {
                menuItemInfo.Item.OnSelected(
                    crSceneContext,
                    This);
            }
        }

        public IEnumerable<CRMenuItem> Items
        {
            get
            {
                return _visibleItems.Select(menuItemInfo => menuItemInfo.Item.GetCRMenuItem(This));
            }
        }

        public virtual void OnReturned()
        {
        }

        public int Count
        {
            get { return _visibleItems.Count; }
        }

        protected abstract TCrMenuSceneItemsProvider This
        {
            get;
        }

        protected abstract IEnumerable<ACrMenuSceneItem<TCrMenuSceneItemsProvider>> OnEnteredInternal();

        public void OnEntered()
        {
            foreach (var crMenuSceneItem in OnEnteredInternal())
            {
                crMenuSceneItem.ReturnCode = ++_maxReturnCode;

                var itemInfo = new MenuItemInfo(crMenuSceneItem);

                _menuItemInfos.Add(
                    crMenuSceneItem.ReturnCode,
                    itemInfo);

                itemInfo.GlobalNode = _globalItems.AddLast(itemInfo);

                if (crMenuSceneItem.IsVisible(This))
                    itemInfo.VisibleNode = _visibleItems.AddLast(itemInfo);
            }
        }

        public void InsertItem(
            ACrSceneContext crSceneContext,
            ACrMenuSceneItem<TCrMenuSceneItemsProvider> newMenuItem,
            ACrMenuSceneItem<TCrMenuSceneItemsProvider> predecessorItem)
        {
            newMenuItem.ReturnCode = ++_maxReturnCode;

            var newMenuItemInfo = new MenuItemInfo(newMenuItem);

            MenuItemInfo predecessorMenuItemInfo;

            newMenuItemInfo.GlobalNode = 
                predecessorItem != null
                && _menuItemInfos.TryGetValue(
                       predecessorItem.ReturnCode,
                       out predecessorMenuItemInfo)
                    ? _globalItems.AddAfter(
                        predecessorMenuItemInfo.GlobalNode,
                        newMenuItemInfo)
                    : _globalItems.AddFirst(newMenuItemInfo);

            _menuItemInfos.Add(
                newMenuItem.ReturnCode, 
                newMenuItemInfo);

            if (newMenuItem.IsVisible(This))
                MakeItemVisible(
                    crSceneContext,
                    newMenuItem,
                    newMenuItemInfo);
        }

        public void RemoveItem(
            ACrSceneContext crSceneContext,
            ACrMenuSceneItem<TCrMenuSceneItemsProvider> removedMenuItem)
        {
            MenuItemInfo removedMenuItemInfo;

            if (!_menuItemInfos.TryGetValue(
                removedMenuItem.ReturnCode,
                out removedMenuItemInfo))
            {
                return;
            }

            if (removedMenuItemInfo.VisibleNode != null)
                MakeItemInvisible(
                    crSceneContext,
                    removedMenuItemInfo);

            _globalItems.Remove(removedMenuItemInfo.GlobalNode);
            _menuItemInfos.Remove(removedMenuItem.ReturnCode);
        }

        private void MakeItemVisible(
            ACrSceneContext crSceneContext,
            ACrMenuSceneItem<TCrMenuSceneItemsProvider> newMenuItem,
            MenuItemInfo newMenuItemInfo)
        {
            var predecessorNode = newMenuItemInfo.GlobalNode.Previous;

            LinkedListNode<MenuItemInfo> visibleNode = null;
            MenuItemInfo visibleItemInfo = null;

            while (predecessorNode != null)
            {
                var predecessorItemInfo = predecessorNode.Value;

                visibleNode = predecessorItemInfo.VisibleNode;

                if (visibleNode != null)
                {
                    visibleItemInfo = predecessorItemInfo;
                    break;
                }

                predecessorNode = predecessorNode.Previous;
            }

            newMenuItemInfo.VisibleNode =
                visibleNode != null
                    ? _visibleItems.AddAfter(
                        visibleNode,
                        newMenuItemInfo)
                    : _visibleItems.AddFirst(newMenuItemInfo);

            _menuSceneProvider.Instance.OnItemInserted(
                crSceneContext,
                newMenuItem.GetCRMenuItem(This),
                visibleItemInfo != null
                    ? visibleItemInfo.Item.ReturnCode
                    : -1);
        }

        public void UpdateItem(
            ACrSceneContext crSceneContext,
            ACrMenuSceneItem<TCrMenuSceneItemsProvider> updatedMenuItem)
        {
            MenuItemInfo updatedMenuItemInfo;

            if (!_menuItemInfos.TryGetValue(
                updatedMenuItem.ReturnCode,
                out updatedMenuItemInfo))
            {
                return;
            }

            if (updatedMenuItemInfo.VisibleNode == null)
            {
                if (!updatedMenuItem.IsVisible(This))
                    return;

                MakeItemVisible(
                    crSceneContext,
                    updatedMenuItem,
                    updatedMenuItemInfo);
            }
            else
            {
                if (updatedMenuItem.IsVisible(This))
                {
                    updatedMenuItem.Update(This);

                    _menuSceneProvider.Instance.OnItemUpdated(
                        crSceneContext,
                        updatedMenuItem.ReturnCode,
                        updatedMenuItem.GetCRMenuItem(This));
                }
                else
                    MakeItemInvisible(
                        crSceneContext,
                        updatedMenuItemInfo);
            }
        }

        private void MakeItemInvisible(
            ACrSceneContext crSceneContext,
            MenuItemInfo removedMenuItemInfo)
        {
            _visibleItems.Remove(removedMenuItemInfo.VisibleNode);
            removedMenuItemInfo.VisibleNode = null;

            _menuSceneProvider.Instance.OnItemRemoved(
                crSceneContext,
                removedMenuItemInfo.Item.ReturnCode);
        }

        protected virtual void OnExitedInternal()
        {
        }

        public void OnExited()
        {
            OnExitedInternal();

            _menuItemInfos.Clear();

            _globalItems.Clear();
            _visibleItems.Clear();
        }
    }

    public class CrMenuScene : 
        CrBaseScene
    {
        private class ShowNoMenuItemsData
        {
            public ACrSceneContext CrSceneContext
            {
                get;
                private set;
            }

            public ICrSceneGroup RequiredSceneGroup
            {
                get;
                private set;
            }

            public ShowNoMenuItemsData(ACrSceneContext crSceneContext)
            {
                CrSceneContext = crSceneContext;
                RequiredSceneGroup = crSceneContext.CurrentCrSceneGroup;
            }
        }

        private bool _isVisible;
        private readonly ICrMenuSceneItemsProvider _menuItemsProvider;

        private readonly CRMenuConfiguration _menuConfiguration;

        private readonly CrSceneGroupExitRoute _canceledRoute;
        private readonly CrSceneGroupExitRoute _timedOutRoute;

        private bool _showingNoMenuItems;

        private const long ShowNoMenuItemsDelay = 3000;

        protected CrMenuScene(
            [NotNull]
            ICrMenuSceneItemsProvider menuItemsProvider,
            [NotNull]
            CRMenuConfiguration menuConfiguration,
            [NotNull]
            CrSceneGroupExitRoute canceledRoute,
            [NotNull]
            CrSceneGroupExitRoute timedOutRoute)
        {
            _menuItemsProvider = menuItemsProvider;
            _menuConfiguration = menuConfiguration;

            _menuConfiguration.ReturnCodeLength = 2;

            _canceledRoute = canceledRoute;
            _timedOutRoute = timedOutRoute;
        }

        public override bool OnEntered(ACrSceneContext crSceneContext)
        {
            _menuItemsProvider.OnEntered();

            ShowMenu(crSceneContext);

            return true;
        }

        private void ShowMenu(ACrSceneContext crSceneContext)
        {
            var cardReader = crSceneContext.CardReader;

            if (_menuItemsProvider.Count != 0)
            {
                _showingNoMenuItems = false;

                cardReader.ParentCommunicator.MenuCommands.StartMenu(
                    _menuConfiguration,
                    cardReader,
                    _menuItemsProvider.Items);
            }
            else
            {
                _showingNoMenuItems = true;
                cardReader.DisplayCommands.ClearAllDisplay(cardReader);

                cardReader.MenuCommands.SetBottomMenuButtons(
                    cardReader,
                    new CRBottomMenu
                    {
                        Button1 = CRMenuButtonLook.No,
                        Button1ReturnCode = CRSpecialKey.No,
                        Button2 = CRMenuButtonLook.Clear,
                        Button3 = CRMenuButtonLook.Clear,
                        Button4 = CRMenuButtonLook.Clear
                    });

                ShowNoMenuItems(cardReader);

                TimerManager.Static.StartTimeout(
                    ShowNoMenuItemsDelay,
                    new ShowNoMenuItemsData(crSceneContext),
                    OnShowNoMenuItemsTimeout);
            }

            _isVisible = true;
        }

        private bool OnShowNoMenuItemsTimeout(TimerCarrier timerCarrier)
        {
            var data = (ShowNoMenuItemsData)timerCarrier.Data;

            data.CrSceneContext.PlanDelayedRouteFollowing(
                this,
                data.RequiredSceneGroup,
                _canceledRoute);

            return true;
        }

        protected virtual void ShowNoMenuItems(CardReader cardReader)
        {
        }

        public override void OnMenuItemSelected(
            ACrSceneContext crSceneContext,
            int itemIndex)
        {
            _isVisible = false;

            _menuItemsProvider.OnSelected(
                crSceneContext, 
                itemIndex);
        }

        public override void OnReturned(ACrSceneContext crSceneContext)
        {
            _menuItemsProvider.OnReturned();

            ShowMenu(crSceneContext);
        }

        public override void OnAdvancing(ACrSceneContext crSceneContext)
        {
            _isVisible = false;
        }

        public override void OnExiting(ACrSceneContext crSceneContext)
        {
            _menuItemsProvider.OnExited();
            _isVisible = false;
        }

        public override void OnDescending(ACrSceneContext crSceneContext)
        {
            _isVisible = false;
        }

        public override void OnSpecialKeyPressed(
            ACrSceneContext crSceneContext,
            CRSpecialKey specialKey)
        {
            _isVisible = false;

            if (_showingNoMenuItems && specialKey == CRSpecialKey.No)
                _canceledRoute.Follow(crSceneContext);
        }

        public override void OnMenuCancelled(
            ACrSceneContext crSceneContext,
            bool byOtherCommand)
        {
            _isVisible = false;

            //if (!byOtherCommand) - continue to scene cancellation either way
            _canceledRoute.Follow(crSceneContext);
        }

        public override void OnMenuTimedOut(ACrSceneContext crSceneContext)
        {
            _isVisible = false;

            _timedOutRoute.Follow(crSceneContext);
        }

        public void OnItemRemoved(
            ACrSceneContext crSceneContext,
            int idx)
        {
            if (!_isVisible)
                return;

            var cardReader = crSceneContext.CardReader;

            cardReader.MenuCommands.RemoveMenuItem(cardReader, idx);

#if DEBUG
            Console.WriteLine(
                "Removed item return code {0}",
                idx);
#endif
        }

        public void OnItemInserted(
            ACrSceneContext crSceneContext,
            CRMenuItem menuItem,
            int idx)
        {
            if (!_isVisible)
                return;

            var cardReader = crSceneContext.CardReader;

            cardReader.MenuCommands.InsertMenuItem(cardReader, idx, menuItem);

#if DEBUG
            Console.WriteLine(
                "Inserted item {0} idx {1} return code {2}",
                menuItem.Text,
                idx,
                menuItem.ReturnCode);
#endif
        }

        public void OnItemUpdated(
            ACrSceneContext crSceneContext,
            int idx,
            CRMenuItem menuItem)
        {
            if (!_isVisible)
                return;

            var cardReader = crSceneContext.CardReader;

            cardReader.MenuCommands.UpdateMenuItem(
                cardReader, 
                idx, 
                menuItem
                );

#if DEBUG
            Console.WriteLine(
                "Updated item {0} idx {1} return code {2} to text {3}",
                menuItem.Text,
                idx,
                menuItem.ReturnCode,
                menuItem.Text);
#endif
        }
    }
}