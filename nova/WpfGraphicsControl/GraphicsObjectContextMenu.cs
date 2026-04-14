using System;
using System.Windows;
using System.Windows.Controls;
using Contal.Cgp.Client;
using Contal.Cgp.NCAS.Globals;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.NCAS.WpfGraphicsControl;
using ContextMenu = System.Windows.Controls.ContextMenu;
using MenuItem = System.Windows.Controls.MenuItem;

namespace Cgp.NCAS.WpfGraphicsControl
{
    public enum ContextMenuAction
    {
        EditObject, AddNewPoint, RemovePoint, AddLabelForObject, TimeBuying
    }

    public class GraphicsObjectContextMenu
    {
        private UIElement _element ;
        private ContextMenu _contextMenu;
        private Canvas _canvas;
        private bool _editMode;

        public delegate void ShapeSettingsClickDelegate(UIElement sender);
        public event ShapeSettingsClickDelegate ShapeSettingsClick;
        public delegate void MenuItemClickDelegate(UIElement sender, ContextMenuAction action);
        public event MenuItemClickDelegate MenuItemClick;

        public static bool IsDisabled { get; set; }

        public GraphicsObjectContextMenu(UIElement element, bool editMode)
        {
            if (element == null)
                return;

            _element = element;
            _editMode = editMode;
            _canvas = (_element as IGraphicsObject).GetCanvas();
            _contextMenu = new ContextMenu();
            _contextMenu.Visibility = Visibility.Collapsed;
            _contextMenu.Opened += _contextMenu_Opened;
            _contextMenu.Closed += _contextMenu_Closed;
        }

        void _contextMenu_Closed(object sender, RoutedEventArgs e)
        {
            _contextMenu.Visibility = Visibility.Collapsed;
        }

        void _contextMenu_Opened(object sender, RoutedEventArgs e)
        {
            if (_editMode)
                CreateContextMenuForEditMode();
            else
                CreateContextMenuForViewMode();

            if (_contextMenu.Items.Count == 0
                || IsDisabled)
                _contextMenu.IsOpen = false;
            else
            {
                _contextMenu.Visibility = Visibility.Visible;
                _contextMenu.IsOpen = true;
            }
        }

        private void CreateContextMenuForEditMode()
        {
            _contextMenu.Items.Clear();
            var deleteMenuItem = new MenuItem();
            deleteMenuItem.Header = GraphicsScene.LocalizationHelper.GetString("General_bDelete");
            deleteMenuItem.Click += deleteMenuItem_Click;
            _contextMenu.Items.Add(deleteMenuItem);
            var shape = _element as IGraphicsObject;

            if (shape == null)
                return;

            var polygon = _element as GraphicsPolygon;

            if (polygon == null)
            {
                var rotateToLeftBy90 = new MenuItem();
                rotateToLeftBy90.Header = GraphicsScene.LocalizationHelper.GetString("ContextMenuItem_RotateToLeftBy90");
                rotateToLeftBy90.Click += rotateToLeftBy90_Click;
                _contextMenu.Items.Add(rotateToLeftBy90);

                var rotateToRightBy90 = new MenuItem();
                rotateToRightBy90.Header = GraphicsScene.LocalizationHelper.GetString("ContextMenuItem_RotateToRightBy90");
                rotateToRightBy90.Click += rotateToRightBy90_Click;
                _contextMenu.Items.Add(rotateToRightBy90);

                var rotateToDefaultAngle = new MenuItem();
                rotateToDefaultAngle.Header = GraphicsScene.LocalizationHelper.GetString("ContextMenuItem_ResetRrotateAngle");
                rotateToDefaultAngle.Click += rotateToDefaultAngle_Click;
                _contextMenu.Items.Add(rotateToDefaultAngle);
            }
            else if (polygon.GetPolygonMode() != PolygonMode.Line)
            {
                var addNewPoint = new MenuItem();
                addNewPoint.Header = GraphicsScene.LocalizationHelper.GetString("ContextMenuItem_AddNewPoint");
                addNewPoint.Click += addNewPoint_Click;
                _contextMenu.Items.Add(addNewPoint);

                int minPointsValue = polygon.GetPolygonMode() == PolygonMode.AlarmArea ? 3 : 2;

                if (polygon.Points.Count > minPointsValue)
                {
                    var removePoint = new MenuItem();
                    removePoint.Header = GraphicsScene.LocalizationHelper.GetString("ContextMenuItem_RemovePoint");
                    removePoint.Click += removePoint_Click;
                    _contextMenu.Items.Add(removePoint);
                }
            }

            if (polygon == null
                || polygon.GetPolygonMode() != PolygonMode.AlarmArea)
            {
                if (shape.GetZIndex() < 99)
                {
                    var zIndexUpMenuItem = new MenuItem();
                    zIndexUpMenuItem.Header =
                        GraphicsScene.LocalizationHelper.GetString("ContextMenuItem_ZIndexUpTo") +
                        (shape.GetZIndex() + 1).ToString();
                    zIndexUpMenuItem.Click += zIndexUpMenuItem_Click;
                    _contextMenu.Items.Add(zIndexUpMenuItem);
                }

                if (shape.GetZIndex() > 0)
                {
                    var zIndexDownMenuItem = new MenuItem();
                    zIndexDownMenuItem.Header =
                        GraphicsScene.LocalizationHelper.GetString("ContextMenuItem_ZIndexDownTo") +
                        (shape.GetZIndex() - 1).ToString();
                    zIndexDownMenuItem.Click += zIndexDownMenuItem_Click;
                    _contextMenu.Items.Add(zIndexDownMenuItem);
                }
            }

            var liveObject = _element as ILiveObject;

            if (liveObject != null
                && liveObject.GetObjectGuid() != Guid.Empty
                && liveObject.GetLabel() == null)
            {
                var AddLabelMenuItem = new MenuItem();
                AddLabelMenuItem.Header = GraphicsScene.LocalizationHelper.GetString("ContextMenuItem_AddLabel");
                AddLabelMenuItem.Click += AddLabelMenuItem_Click;
                _contextMenu.Items.Add(AddLabelMenuItem);
            }

            var SettingsMenuItem = new MenuItem();
            SettingsMenuItem.Header = GraphicsScene.LocalizationHelper.GetString("ContextMenuItem_Settings");
            SettingsMenuItem.Click += SettingsMenuItem_Click;
            _contextMenu.Items.Add(SettingsMenuItem);   
        }

        void AddLabelMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (MenuItemClick != null)
                MenuItemClick(_element, ContextMenuAction.AddLabelForObject);
        }

        void removePoint_Click(object sender, RoutedEventArgs e)
        {
            if (MenuItemClick != null)
                MenuItemClick(_element, ContextMenuAction.RemovePoint);
        }

        void addNewPoint_Click(object sender, RoutedEventArgs e)
        {
            if (MenuItemClick != null)
                MenuItemClick(_element, ContextMenuAction.AddNewPoint);
        }

        void rotateToDefaultAngle_Click(object sender, RoutedEventArgs e)
        {
            if ((_element as IGraphicsObject) != null)
                (_element as IGraphicsObject).ResetRotateAngle();
        }

        void rotateToRightBy90_Click(object sender, RoutedEventArgs e)
        {
            if ((_element as IGraphicsObject) != null)
                (_element as IGraphicsObject).RotateByAngle(90);
        }

        void rotateToLeftBy90_Click(object sender, RoutedEventArgs e)
        {
            if ((_element as IGraphicsObject) != null)
                (_element as IGraphicsObject).RotateByAngle(-90);
        }

        private void CreateContextMenuForViewMode()
        {
            var liveObject = _element as ILiveObject;

            if (liveObject == null || liveObject.GetObjectGuid() == Guid.Empty)
                return;

            _contextMenu.Items.Clear();
            var editObjectMenuItem = new MenuItem();
            editObjectMenuItem.Header = GraphicsScene.LocalizationHelper.GetString("ContextMenuItem_EditObject");
            editObjectMenuItem.Click += MenuItem_Click;
            _contextMenu.Items.Add(editObjectMenuItem);

            try
            {
                if (_element is GraphicsPolygon)
                {
                    var alarmArea =
                                GraphicsScene.MainServerProvider.AlarmAreas.GetObjectById(
                                    liveObject.GetObjectGuid());

                    if (GraphicsScene.MainServerProvider.AlarmAreas.GetAlarmAreaActivationState(
                            liveObject.GetObjectGuid()) == ActivationState.Unset)
                    {
                        if (CgpClient.Singleton.MainServerProvider.HasAccess(
                            NCASAccess.GetAccess(AccessNCAS.AlarmAreasSetPerform)))
                        {
                            var setMenuItem = new MenuItem();
                            setMenuItem.Header = GraphicsScene.LocalizationHelper.GetString("ContextMenuItem_Set");
                            setMenuItem.Click += setMenuItem_Click;
                            _contextMenu.Items.Add(setMenuItem);

                            if (alarmArea != null && alarmArea.PreWarning)
                            {
                                var setWithNoPrewarningMenuItem = new MenuItem();
                                setWithNoPrewarningMenuItem.Header =
                                    GraphicsScene.LocalizationHelper.GetString(
                                        "ContextMenuItem_SetWithNoPrewarningMenuItem");
                                setWithNoPrewarningMenuItem.Click += setWithNoPrewarningMenuItem_Click;
                                _contextMenu.Items.Add(setWithNoPrewarningMenuItem);
                            }
                        }

                        if (CgpClient.Singleton.MainServerProvider.HasAccess(
                            NCASAccess.GetAccess(AccessNCAS.AlarmAreasUnconditionalSetPerform)))
                        {
                            var unconditionalSetMenuItem = new MenuItem();
                            unconditionalSetMenuItem.Header = GraphicsScene.LocalizationHelper.GetString("ContextMenuItem_UnconditionalSet");
                            unconditionalSetMenuItem.Click += unconditionalSetMenuItem_Click;
                            _contextMenu.Items.Add(unconditionalSetMenuItem);
                        }
                    }

                    var aaActivationState = GraphicsScene.MainServerProvider.AlarmAreas.GetAlarmAreaActivationState(
                        liveObject.GetObjectGuid());

                    if (aaActivationState == ActivationState.Set
                        || aaActivationState == ActivationState.Prewarning
                        || aaActivationState == ActivationState.TemporaryUnsetEntry
                        || aaActivationState == ActivationState.TemporaryUnsetExit)
                    {
                        if (CgpClient.Singleton.MainServerProvider.HasAccess(
                            NCASAccess.GetAccess(AccessNCAS.AlarmAreasUnsetPerform)))
                        {
                            var unsetMenuItem = new MenuItem();
                            unsetMenuItem.Header = GraphicsScene.LocalizationHelper.GetString("ContextMenuItem_Unset");
                            unsetMenuItem.Click += unsetMenuItem_Click;
                            _contextMenu.Items.Add(unsetMenuItem);
                        }

                        if (alarmArea != null && alarmArea.TimeBuyingEnabled)
                        {
                            var timeBuyingMenuItem = new MenuItem();
                            timeBuyingMenuItem.Header = GraphicsScene.LocalizationHelper.GetString("ContextMenuItem_TimeBuying");
                            timeBuyingMenuItem.Click += timeBuyingMenuItem_Click;
                            _contextMenu.Items.Add(timeBuyingMenuItem);
                        }
                    }
                }
                else if (_element is GraphicsIO)
                {
                    if ((_element as GraphicsIO).IOType == IOType.Input)
                    {
                        var input =
                            GraphicsScene.MainServerProvider.Inputs.GetObjectById(
                                liveObject.GetObjectGuid());

                        if (input == null || input.BlockingType == (byte) BlockingType.BlockedByObject
                            || GraphicsScene.MainServerProvider.DoorEnvironments.IsInputInDoorEnvironments(input.IdInput))
                            return;

                        if (CgpClient.Singleton.MainServerProvider.HasAccess(
                            NCASAccess.GetAccess(AccessNCAS.InputsInputControlAdmin)))
                            return;

                        if (input.BlockingType == (byte) BlockingType.NotBlocked)
                        {
                            var blockForcefullyMenuItem = new MenuItem();
                            blockForcefullyMenuItem.Header = GraphicsScene.LocalizationHelper.GetString("ContextMenuItem_BlockForcefully");
                            blockForcefullyMenuItem.Click += blockForcefullyMenuItem_Click;
                            _contextMenu.Items.Add(blockForcefullyMenuItem);
                        }

                        if (input.BlockingType == (byte) BlockingType.NotBlocked
                            || input.BlockingType == (byte) BlockingType.ForcefullyBlocked)
                        {
                            var setForcefullyMenuItem = new MenuItem();
                            setForcefullyMenuItem.Header = GraphicsScene.LocalizationHelper.GetString("ContextMenuItem_SetForcefully");
                            setForcefullyMenuItem.Click += setForcefullyMenuItem_Click;
                            _contextMenu.Items.Add(setForcefullyMenuItem);
                        }

                        if (input.BlockingType == (byte) BlockingType.ForcefullySet
                            || input.BlockingType == (byte) BlockingType.ForcefullyBlocked)
                        {
                            var unblockMenuItem = new MenuItem();
                            unblockMenuItem.Header = GraphicsScene.LocalizationHelper.GetString("ContextMenuItem_Unblock");
                            unblockMenuItem.Click += unblockMenuItem_Click;
                            _contextMenu.Items.Add(unblockMenuItem);
                        }
                    }
                    else
                    {
                        var output =
                            GraphicsScene.MainServerProvider.Outputs.GetObjectById(liveObject.GetObjectGuid());
                        
                        if (output == null || output.ControlType == (byte) OutputControl.controledByObject
                            || GraphicsScene.MainServerProvider.DoorEnvironments.IsOutputInDoorEnvironmentsActuators(output))
                            return;

                        if (CgpClient.Singleton.MainServerProvider.HasAccess(
                            NCASAccess.GetAccess(AccessNCAS.OutputsOutputControlAdmin)))
                            return;

                        if (output.ControlType != (byte) OutputControl.manualBlocked)
                        {
                            var blockManuallyMenuItem = new MenuItem();
                            blockManuallyMenuItem.Header = GraphicsScene.LocalizationHelper.GetString("ContextMenuItem_BlockManually");
                            blockManuallyMenuItem.Click += blockManuallyMenuItem_Click;
                            _contextMenu.Items.Add(blockManuallyMenuItem);
                        }

                        if (output.ControlType != (byte) OutputControl.forcedOn)
                        {
                            var setForcefullyMenuItem = new MenuItem();
                            setForcefullyMenuItem.Header = GraphicsScene.LocalizationHelper.GetString("ContextMenuItem_SetForcefully");
                            setForcefullyMenuItem.Click += setForcefullyOutputMenuItem_Click;
                            _contextMenu.Items.Add(setForcefullyMenuItem);
                        }

                        if (output.ControlType != (byte) OutputControl.unblocked)
                        {
                            var unblockOutputMenuItem = new MenuItem();
                            unblockOutputMenuItem.Header = GraphicsScene.LocalizationHelper.GetString("ContextMenuItem_Unblock");
                            unblockOutputMenuItem.Click += unblockOutputMenuItem_Click;
                            _contextMenu.Items.Add(unblockOutputMenuItem);
                        }
                    }
                }
                else if (_element is GraphicsDoorEnvironment)
                {
                    if (GraphicsScene.MainServerProvider.DoorEnvironments.HasAccessToAccessGranted(
                        liveObject.GetObjectGuid()))
                    {
                        var accessGrantedMenuItem = new MenuItem();
                        accessGrantedMenuItem.Header = GraphicsScene.LocalizationHelper.GetString("ContextMenuItem_AccessGranted");
                        accessGrantedMenuItem.Click += accessGrantedMenuItem_Click;
                        _contextMenu.Items.Add(accessGrantedMenuItem);
                    }
                }
            }
            catch (Exception)
            {
                CgpClient.Singleton.IsConnectionLost(true);
            }
        }

        void timeBuyingMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (MenuItemClick != null)
                MenuItemClick(_element, ContextMenuAction.TimeBuying);
        }

        void setWithNoPrewarningMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var result =
                    GraphicsScene.MainServerProvider.AlarmAreas.SetAlarmArea(
                        (_element as ILiveObject).GetObjectGuid(),
                        true);

                if (result != AlarmAreaActionResult.Success)
                {
                    SetSetAlarmAreaResult(result);
                }
            }
            catch (Exception)
            {
                CgpClient.Singleton.IsConnectionLost(true);
            }
        }

        private void SetSetAlarmAreaResult(AlarmAreaActionResult result)
        {
            if (result == AlarmAreaActionResult.FailedNoImplicitManager)
            {
                 Contal.IwQuick.UI.Dialog.Error(GraphicsScene.LocalizationHelper.GetString("NCASAlarmAreaEditForm_SetFailedNoImplicitManager"));
            }
            else if (result == AlarmAreaActionResult.FailedInputInAlarm)
            {
                 Contal.IwQuick.UI.Dialog.Error(GraphicsScene.LocalizationHelper.GetString("NCASAlarmAreaEditForm_SetFailedInputInAlarm"));
            }
            else if (result == AlarmAreaActionResult.FailedCCUOffline)
            {
                 Contal.IwQuick.UI.Dialog.Error(GraphicsScene.LocalizationHelper.GetString("NCASAlarmAreaEditForm_SetFailedCCUOffline"));
            }
            else if (result == AlarmAreaActionResult.SetUnsetNotConfirm)
            {
                 Contal.IwQuick.UI.Dialog.Error(GraphicsScene.LocalizationHelper.GetString("NCASAlarmAreaEditForm_SetFailedNotConfirm"));
            }
            else
            {
                Contal.IwQuick.UI.Dialog.Error(GraphicsScene.LocalizationHelper.GetString("NCASAlarmAreaEditForm_SetFailed"));
            }
        }

        void accessGrantedMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DoorEnvironment doorEnvironment =
                    GraphicsScene.MainServerProvider.DoorEnvironments.GetObjectById(
                        (_element as ILiveObject).GetObjectGuid());

                if (doorEnvironment != null)
                    GraphicsScene.MainServerProvider.DoorEnvironments.DoorEnvironmentAccessGranted(doorEnvironment);
            }
            catch (Exception)
            {
                CgpClient.Singleton.IsConnectionLost(true);
            }
        }

        void unblockOutputMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool editAllowed;
                Exception ex;
                var output =
                    GraphicsScene.MainServerProvider.Outputs.GetObjectForEditById(
                        (_element as ILiveObject).GetObjectGuid(), out editAllowed);
                if (output.ControlType != (byte) OutputControl.unblocked && editAllowed)
                {
                    output.ControlType = (byte) OutputControl.unblocked;
                    GraphicsScene.MainServerProvider.Outputs.Update(output, out ex);

                    if (ex != null)
                    {
                        Contal.IwQuick.UI.Dialog.Error(GraphicsScene.LocalizationHelper.GetString("Error"));
                    }
                }
            }
            catch (Exception)
            {
                CgpClient.Singleton.IsConnectionLost(true);
            }
        }

        void setForcefullyOutputMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool editAllowed;
                Exception ex;
                var output =
                    GraphicsScene.MainServerProvider.Outputs.GetObjectForEditById(
                        (_element as ILiveObject).GetObjectGuid(), out editAllowed);
                if (output.ControlType != (byte) OutputControl.forcedOn && editAllowed)
                {
                    output.ControlType = (byte) OutputControl.forcedOn;
                    GraphicsScene.MainServerProvider.Outputs.Update(output, out ex);

                    if (ex != null)
                    {
                        Contal.IwQuick.UI.Dialog.Error(GraphicsScene.LocalizationHelper.GetString("Error"));
                    }
                }
            }
            catch (Exception)
            {
                CgpClient.Singleton.IsConnectionLost(true);
            }
        }

        void blockManuallyMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool editAllowed;
                Exception ex;
                var output =
                    GraphicsScene.MainServerProvider.Outputs.GetObjectForEditById(
                        (_element as ILiveObject).GetObjectGuid(), out editAllowed);
                if (output.ControlType != (byte) OutputControl.manualBlocked && editAllowed)
                {
                    output.ControlType = (byte) OutputControl.manualBlocked;
                    GraphicsScene.MainServerProvider.Outputs.Update(output, out ex);

                    if (ex != null)
                    {
                        Contal.IwQuick.UI.Dialog.Error(GraphicsScene.LocalizationHelper.GetString("Error"));
                    }
                }
            }
            catch (Exception)
            {
                CgpClient.Singleton.IsConnectionLost(true);
            }
        }

        void unblockMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool editAllowed;
                Exception ex;
                var input =
                    GraphicsScene.MainServerProvider.Inputs.GetObjectForEditById(
                        (_element as ILiveObject).GetObjectGuid(), out editAllowed);
                if ((input.BlockingType == (byte) BlockingType.ForcefullySet
                     || input.BlockingType == (byte) BlockingType.ForcefullyBlocked)
                    && editAllowed)
                {
                    input.BlockingType = (byte) BlockingType.NotBlocked;
                    GraphicsScene.MainServerProvider.Inputs.Update(input, out ex);

                    if (ex != null)
                    {
                        Contal.IwQuick.UI.Dialog.Error(GraphicsScene.LocalizationHelper.GetString("Error"));
                    }
                }
            }
            catch (Exception)
            {
                CgpClient.Singleton.IsConnectionLost(true);
            }
        }

        void setForcefullyMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool editAllowed;
                Exception ex;
                var input =
                    GraphicsScene.MainServerProvider.Inputs.GetObjectForEditById(
                        (_element as ILiveObject).GetObjectGuid(), out editAllowed);

                if ((input.BlockingType == (byte) BlockingType.NotBlocked
                     || input.BlockingType == (byte) BlockingType.ForcefullyBlocked)
                    && editAllowed)
                {
                    input.BlockingType = (byte) BlockingType.ForcefullySet;
                    GraphicsScene.MainServerProvider.Inputs.Update(input, out ex);

                    if (ex != null)
                    {
                        Contal.IwQuick.UI.Dialog.Error(GraphicsScene.LocalizationHelper.GetString("Error"));
                    }
                }
            }
            catch (Exception)
            {
                CgpClient.Singleton.IsConnectionLost(true);
            }
        }

        void blockForcefullyMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool editAllowed;
                Exception ex;
                var input =
                    GraphicsScene.MainServerProvider.Inputs.GetObjectForEditById(
                        (_element as ILiveObject).GetObjectGuid(), out editAllowed);

                if (input.BlockingType == (byte) BlockingType.NotBlocked && editAllowed)
                {
                    input.BlockingType = (byte) BlockingType.ForcefullyBlocked;
                    GraphicsScene.MainServerProvider.Inputs.Update(input, out ex);

                    if (ex != null)
                    {
                        Contal.IwQuick.UI.Dialog.Error(GraphicsScene.LocalizationHelper.GetString("Error"));
                    }
                }
            }
            catch (Exception)
            {
                CgpClient.Singleton.IsConnectionLost(true);
            }
        }

        void unsetMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                GraphicsScene.MainServerProvider.AlarmAreas.UnsetAlarmArea(
                    (_element as ILiveObject).GetObjectGuid(),
                    0);
            }
            catch (Exception)
            {
                CgpClient.Singleton.IsConnectionLost(true);
            }
        }

        void unconditionalSetMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                GraphicsScene.MainServerProvider.AlarmAreas.UnconditionalSetAlarmArea(
                    (_element as ILiveObject).GetObjectGuid(),
                    false);
            }
            catch (Exception)
            {
                CgpClient.Singleton.IsConnectionLost(true);
            }
        }

        void setMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = GraphicsScene.MainServerProvider.AlarmAreas.SetAlarmArea(
                    (_element as ILiveObject).GetObjectGuid(),
                    false);

                if (result != AlarmAreaActionResult.Success)
                {
                    SetSetAlarmAreaResult(result);
                }
            }
            catch (Exception)
            {
                CgpClient.Singleton.IsConnectionLost(true);
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (MenuItemClick != null)
                MenuItemClick(_element, ContextMenuAction.EditObject);
        }

        void SettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (ShapeSettingsClick != null)
                ShapeSettingsClick(_element);
        }

        public ContextMenu GetContextMenu()
        {
            return _contextMenu;
        }

        void zIndexDownMenuItem_Click(object sender, RoutedEventArgs e)
        {
            int zIndex = (_element as IGraphicsObject).GetZIndex();

            if (zIndex > 0)
                (_element as IGraphicsObject).SetZIndex(--zIndex);
        }

        void zIndexUpMenuItem_Click(object sender, RoutedEventArgs e)
        {
            int zIndex = (_element as IGraphicsObject).GetZIndex();

            if (zIndex < 99)
                (_element as IGraphicsObject).SetZIndex( ++zIndex);
        }

        void deleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            (_element as IGraphicsObject).UnSelect();
            _canvas.Children.Remove(_element);
        }
    }
}
