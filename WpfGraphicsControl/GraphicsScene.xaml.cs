using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

using Cgp.NCAS.WpfGraphicsControl;
using ClipArtViewer;
using Contal.Cgp.Client;
using Contal.Cgp.Globals;
using Contal.Cgp.Globals.PlatformPC;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick;
using Contal.Cgp.NCAS.Definitions;
using WW.Cad.IO;
using WW.Cad.Model;
using Contal.Cgp.BaseLib;
using Contal.Cgp.NCAS.RemotingCommon;
using Contal.IwQuick.Localization;
using WW.Cad.Model.Tables;

using Cursors = System.Windows.Input.Cursors;
using DragEventArgs = System.Windows.DragEventArgs;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MessageBox = System.Windows.MessageBox;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using UserControl = System.Windows.Controls.UserControl;
using VerticalAlignment = System.Windows.VerticalAlignment;
using Action = System.Action;

namespace Contal.Cgp.NCAS.WpfGraphicsControl
{
    public partial class GraphicsScene : UserControl
    {
        private const double FontMinSize = 0.01;
        private double _displayWidth = 50;
        private double _lastMousePosX = -1, _lastMousePosY = -1;
        private double _offsetX, _offsetY;
        private bool _isModelLoaded;
        private bool _moveAll;
        private DxfModel _model;
        private CadVisualizer _cadVisualizer;
        private BitmapImage _backgroundBitmap;
        private readonly Image _backgroundImage = new Image();
        private InterfaceMode _userInterfaceModeValue;
        private bool _allowedEdit = true;
        
        private InterfaceMode UserInterfaceMode 
        {
            get
            {
                return _userInterfaceModeValue;
            }
            set
            {
                if (value != InterfaceMode.Select)
                {
                    _canvasMyShapes.ContextMenu.IsEnabled = false;
                    GraphicsObjectContextMenu.IsDisabled = true;
                }
                else
                {
                    _canvasMyShapes.ContextMenu.IsEnabled = true;
                    GraphicsObjectContextMenu.IsDisabled = false;
                }

                _userInterfaceModeValue = value;
            }
        }

        private IGraphicsObject _selectedShapeValue;

        private IGraphicsObject _selectedShape {
            get { return _selectedShapeValue; }
            set
            {
                _selectedShapeValue = value;

                if (_shapeSettingsPanel != null
                    && ((IGraphicsObject)_shapeSettingsPanel.GetEditElement()) != _selectedShapeValue)
                {
                    _shapeSettingsPanel.Visibility = Visibility.Collapsed;
                }
                else if (_textSettingsPanel != null
                         && ((IGraphicsObject) _textSettingsPanel.GetEditText()) != _selectedShapeValue)
                {
                    _textSettingsPanel.Visibility = Visibility.Collapsed;
                }
            }
        }

        private IGraphicsObject _editingPolygonShape;
        private WriteableBitmap _writableBitmap;
        private readonly Dictionary<string, SymbolParemeter> _symbolCache = new Dictionary<string, SymbolParemeter>();
        private PaintMode _paintShape = PaintMode.Rectangle;
        private SymbolType _paintSymbolType = SymbolType.CardReader;
        private IOType _paintIoType = IOType.Input;
        private IGraphicsObject _drawingShape;
        private Point _drawingMainPoint;
        private Layer _selectedLayer;
        private SymbolSize _displayMode = SymbolSize.Variable;
        private ScaleRatioSelector _scaleRatioSelector;
        private Guid? _newObjectGuid;
        private List<Category> _newObjectCategories;
        private readonly InformationPanel _informationPanel = new InformationPanel();
        private ObjectType? _paintObjectType;
        private ILiveObject _lastSelectedShape;
        private bool _informationPanelIsVisible;
        private Scene _scene;
        private bool _firstSceneLoaded;
        private Guid _idBasicScene;
        private readonly LinkedList<Guid> _sceneHistory = new LinkedList<Guid>();
        private readonly bool _onlyViewMode;
        private bool _editMode;
        private bool _needSave;

        public Scene Scene
        {
            get
            {
                return _scene;  
            }
            set
            {
                Cursor = Cursors.Wait;
                _scene = value;
                LoadScene();
                _defaultScale = GetDefaultScale();
                
                if (SceneChanged != null)
                    SceneChanged(_scene);

                if (!_firstSceneLoaded)
                {
                    _firstSceneLoaded = true;
                    _idBasicScene = _scene.IdScene;
                }

                Cursor = Cursors.Arrow;
            }
        }

        public List<Layer> Layers
        {
            get
            {
                return CanvasSerialization.Layers.Values.ToList();
            }
        }

        public bool EditMode 
        {
            get
            {
                return _editMode;
            }
            set
            {
                _editMode = value;
                Text.AlowEdit = _editMode;
            }
        }

        public bool AllowedEdit
        {
            set
            {
                _allowedEdit = value;
                SetMainMenuVisibility();
            }
        }

        public static ICgpNCASRemotingProvider MainServerProvider { get; set; }
        public CanvasSerialization CanvasSerialization = new CanvasSerialization();
        public static LocalizationHelper LocalizationHelper { get; set; }
        public event Func<string, string> GetLocalizationString;
        public delegate void LoadedModelDelegate();
        public event LoadedModelDelegate LoadedModel;
        public delegate void ShowScenesDelegate();
        public event ShowScenesDelegate ShowScenes;
        public delegate void SaveSceneClickDelegate();
        public event SaveSceneClickDelegate SaveSceneClick;
        public delegate void ChangeEditModeDelegate(bool editMode);
        public event ChangeEditModeDelegate ChangeEditMode;
        public delegate void ButtonClickDelegate(string buttonName);
        public event ButtonClickDelegate ButtonClick;
        public delegate void EditObjectClickDelegate(ObjectType objectType, Guid id);
        public event EditObjectClickDelegate EditObjectClick;
        public delegate void DSceneChanged(Scene scene);
        public event DSceneChanged SceneChanged;
        public event DSceneChanged NeedSceneChanged;

        public bool NeedSaveScene 
        {
            get { return _needSave; }
        }

        private readonly DispatcherTimer _timerForUpdateInformation = new DispatcherTimer();

        public GraphicsScene(bool onlyViewMode)
        {
            InitializeComponent();
            EnablePainting(false);
            _onlyViewMode = onlyViewMode;
            KeyUp += GraphicsControl_KeyUp;
            GraphicsThumb.ThumbClick += Thumbs_ThumbClick;
            UserInterfaceMode = InterfaceMode.Select;
            _canvasMyShapes.ContextMenuOpening += _canvasMyShapes_ContextMenuOpening;
            _timerForUpdateInformation.Tick += TimerForUpdateInformation_Tick;
            _timerForUpdateInformation.Interval = new TimeSpan(0, 0, 1);
            _timerForUpdateInformation.Start();

            if (onlyViewMode)
            {
                EditMode = false;
                myGrid.RowDefinitions[0].Height = new GridLength(0);
                myGrid.ColumnDefinitions[0].Width = new GridLength(0);
                myGrid.ColumnDefinitions[0].MinWidth = 0;
                myGrid.ColumnDefinitions[1].Width = new GridLength(0);
                SetVisibilityForBackgroundInformationPanel(false);
                _tbMainMenu.Visibility = Visibility.Collapsed;
            }
        }

        public void SetLayerVisibility(Layer layer, bool isEnable)
        {
            if (layer == null)
                return;

            SetShapesVisibilityOnCanvasByLayer(layer.Id, isEnable);
        }

        void _canvasMyShapes_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (UserInterfaceMode == InterfaceMode.Paint
                || !EditMode
                && (e.OriginalSource as Canvas) != null)
            {
                e.Handled = true;
            }
        }

        private void PressEnterKey()
        {
            if (UserInterfaceMode == InterfaceMode.Paint)
            {
                if (_paintShape == PaintMode.Polyline
                    || _paintShape == PaintMode.AlarmAreaPolygon)
                {
                    var polygon = _drawingShape as GraphicsPolygon;

                    if (polygon == null)
                        return;

                    if (polygon.GetPolygonMode() == PolygonMode.PolyLine
                        && !polygon.IsClosed)
                    {
                        polygon.Points.Remove(polygon.Points[polygon.Points.Count - 1]);
                        polygon.Refresh();
                    }

                    if (polygon.GetPolygonMode() == PolygonMode.AlarmArea)
                    {
                        polygon.Points.Remove(polygon.Points[polygon.Points.Count - 1]);
                        polygon.IsClosed = true;
                        polygon.Refresh();
                        polygon.StrokeThickness = 0;
                    }

                    UserInterfaceMode = InterfaceMode.Select;
                    _drawingShape = null;
                }
            }
        }

        private void PressDeleteKey()
        {
            if (UserInterfaceMode == InterfaceMode.Paint
                        && (_paintShape == PaintMode.AlarmAreaPolygon
                        || _paintShape == PaintMode.Polyline))
            {
                var polygon = _drawingShape as GraphicsPolygon;

                if (polygon != null)
                {
                    if (polygon.Points.Count == 2)
                    {
                        UserInterfaceMode = InterfaceMode.Select;
                        _canvasMyShapes.Children.Remove(polygon);
                        _drawingShape = null;
                    }
                    else
                    {
                        polygon.Points.RemoveAt(polygon.Points.Count - 1);
                        polygon.Refresh();
                    }
                }
            }
        }

        private void GraphicsControl_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:

                    PressEnterKey();

                    break;

                case Key.Delete:

                    PressDeleteKey();

                    break;

                case Key.Escape:

                    if (UserInterfaceMode == InterfaceMode.PolygonRemovePoint)
                    {
                        UserInterfaceMode = InterfaceMode.Select;
                        GraphicsThumb.DeleteMode = false;
                    }

                    break;
            }
        }

        private void Thumbs_ThumbClick(GraphicsThumb sender, int count)
        {
            if (UserInterfaceMode == InterfaceMode.PolygonRemovePoint)
            {
                var polygon = _editingPolygonShape as GraphicsPolygon;

                if (polygon != null 
                    && polygon.Points.Count > count)
                {
                    polygon.Points.RemoveAt(count);
                    polygon.Refresh();
                    polygon.UnSelect();
                    polygon.Select(EditMode);
                    GraphicsThumb.DeleteMode = false;
                }

                UserInterfaceMode = InterfaceMode.Select;
            }
        }

        private void TimerForUpdateInformation_Tick(object sender, EventArgs e)
        {
            InformationPanelUpdateInfo();
        }

        private void CloseAllOpenedWindow()
        {
            if (_openedSceneSettingsWindow != null)
            {
                _openedSceneSettingsWindow.Visibility = Visibility.Collapsed;
            }

            if (_shapeSettingsPanel != null)
            {
                ShapeSettingsWindow_Closed(_shapeSettingsPanel);
            }
        }

        private void _bLoadDfxFile_OnClick(object sender, RoutedEventArgs e)
        {
            CloseAllOpenedWindow();
            ImportBackgroundMenuClick();
        }

        private void SetVisibilityForBackgroundInformationPanel(bool visible)
        {
            if (visible)
            {
                myGrid.RowDefinitions[2].Height = new GridLength(80);
                myGrid.RowDefinitions[3].Height = new GridLength(100);
                _lBackgroundEditModeInfo.Visibility = Visibility.Visible;
                _canvasMyShapes.ContextMenu.IsEnabled = false;
            }
            else
            {
                myGrid.RowDefinitions[3].Height = new GridLength(0);
                myGrid.RowDefinitions[2].Height = new GridLength(180);
                _lBackgroundEditModeInfo.Visibility = Visibility.Hidden;
                _canvasMyShapes.ContextMenu.IsEnabled = true;
            }

            UpdateLayout();
            ResizeGraphicViewer();
        }

        private void ImportBackgroundMenuClick()
        {
            var openDialog = new OpenFileDialog
            {
                Filter = "(*.bmp, *.jpg, *.png, *.dwg, *.dxf)|*.bmp;*.jpg;*.png;*.dwg;*.dxf"
            };

            if (_canvasUI.Children.Contains(_scaleInfo))
                _canvasUI.Children.Remove(_scaleInfo);

            if (openDialog.ShowDialog().Value)
            {
                EnablePainting(false);
                HideScaleRatioSelector();
                SetVisibilityForBackgroundInformationPanel(true);
                _tbFilePath.Text = openDialog.FileName;
                _cadVisualizer = null;
                _backgroundBitmap = null;
                _canvasBackground.Children.Clear();

                if (_canvasMyShapes.Children.Count > 0
                    && IwQuick.UI.Dialog.Question("Question", GetLocalizationString("QuestionRemoveSymbol")))
                {
                    _canvasMyShapes.Children.Clear();

                    if (CanvasSerialization.Layers != null)
                        CanvasSerialization.Layers.Clear();
                }

                if (_canvasMyShapes.Children.Contains(_backgroundImage))
                    _canvasMyShapes.Children.Remove(_backgroundImage);

                try
                {
                    ResetOffset();
                    string extension = Path.GetExtension(openDialog.FileName);

                    if (extension == ".dwg"
                        || extension == ".dxf")
                    {
                        _cadVisualizer = new CadVisualizer();
                        _model = extension == ".dwg" 
                            ? DwgReader.Read(openDialog.FileName)
                            : DxfReader.Read(openDialog.FileName);

                        Scene.BackgroundDataType = SymbolDataType.Vector;
                        SetInsertBackgroundModeVisibility();
                        _bSelectScaleRatio.IsEnabled = false;
                        _writableBitmap = BitmapFactory.New(Convert.ToInt32(myGrid.ColumnDefinitions[2].ActualWidth),
                            Convert.ToInt32(myGrid.RowDefinitions[1].ActualHeight +
                                            myGrid.RowDefinitions[2].ActualHeight));
                        _backgroundImage.Source = _writableBitmap;
                        _canvasBackground.Children.Add(_backgroundImage);
                        _cadVisualizer.WriteableBitmap = _writableBitmap;
                        _cadVisualizer.CalculateModelSize = true;
                        IwQuick.Threads.SafeThread<DxfModel, bool>.StartThread(LoadModel, _model, true);
                    }
                    else
                    {
                        try
                        {
                            Scene.BackgroundDataType = SymbolDataType.Raster;
                            UnsetInsertBackgroundModeVisibility();
                            _backgroundBitmap = new BitmapImage();
                            _backgroundBitmap.BeginInit();
                            _backgroundBitmap.UriSource = new Uri(openDialog.FileName);
                            _backgroundBitmap.EndInit();
                            _backgroundImage.Source = _backgroundBitmap;
                            Canvas.SetZIndex(_backgroundImage, -2);
                            _canvasMyShapes.Children.Add(_backgroundImage);

                            _lSizeInfo.Content = string.Format("{0} {1} x {2} {3}",
                                LocalizationHelper.GetString("NCASSceneEditForm_lSizeInfo"),
                                _backgroundImage.Source.Width.ToString("F0"),
                                _backgroundImage.Source.Height.ToString("F0"),
                                LocalizationHelper.GetString("General_Pixels"));

                            _canvasMyShapes.Width = _backgroundImage.Source.Width;
                            _canvasMyShapes.Height = _backgroundImage.Source.Height;
                            _canvasMyShapes.UpdateLayout();
                            _canvasMyShapes.LayoutTransform = new ScaleTransform(GetOptimalZoom(), GetOptimalZoom());
                            _cbZoomValues.Text = _canvasMyShapes.LayoutTransform.Value.M11.ToString("P");
                            _chbShowAll.IsEnabled = true;
                            LoadCanvasLayers();
                            ShowScaleRatioSelector();
                            _bSelectScaleRatio.IsEnabled = true;
                            _defaultScale = GetDefaultScale();
                            SetScale(_defaultScale);
                            SetCorrectPositionOfShapesOnCanvas();
                        }
                        catch (Exception ex)
                        {
                            _backgroundBitmap = null;
                            MessageBox.Show(ex.ToString());
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        //Load CAD model (DWG, DXF)
        private void LoadModel(DxfModel dxfModel, bool loadLayers)
        {
            if (dxfModel == null)
                return;

            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
            {
                myScrollViewer.IsEnabled = false;
                Cursor = Cursors.Wait;
            }));

            _cadVisualizer.LoadModel(dxfModel);

            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
            {
                if (loadLayers)
                    LoadDxfLayers();

                _chbShowAll.IsEnabled = true;
                _isModelLoaded = true;
                _canvasMyShapes.Width = _cadVisualizer.ModelWidth;
                _canvasMyShapes.Height = _cadVisualizer.ModelHeight;
                _canvasMyShapes.LayoutTransform = new ScaleTransform(_cadVisualizer.ActualZoom(),
                    _cadVisualizer.ActualZoom());
                _cadVisualizer.Draw();
                myScrollViewer.IsEnabled = true;
                Cursor = Cursors.Arrow;
                _cbZoomValues.Text = _canvasMyShapes.LayoutTransform.Value.M11.ToString("P");
                _lSizeInfo.Content = string.Format("Size: {0} x {1} meters",
                    (_cadVisualizer.ModelWidth/1000).ToString("F1"),
                    (_cadVisualizer.ModelHeight/1000).ToString("F1"));

                //adding model scale info
                if (!_canvasUI.Children.Contains(_scaleInfo))
                    _canvasUI.Children.Add(_scaleInfo);

                _defaultScale = GetDefaultScale();
                _scaleInfo.SetScaleRatio(1.0);
                _scaleInfo.SetScale(_defaultScale);
                SetCorrectPositionOfShapesOnCanvas();
            }));
        }

        private void SetCorrectPositionOfShapesOnCanvas()
        {
            foreach (var element in _canvasMyShapes.Children)
            {
                var graphicsShape = element as IGraphicsObject;

                if (graphicsShape == null)
                    continue;

                if (graphicsShape.GetWidth() > _canvasMyShapes.Width)
                    graphicsShape.SetWidth(_canvasMyShapes.Width);

                if (graphicsShape.GetHeight() > _canvasMyShapes.Height)
                    graphicsShape.SetHeight(_canvasMyShapes.Height);

                if (graphicsShape.GetLeft() + graphicsShape.GetWidth() > _canvasMyShapes.Width)
                    graphicsShape.MoveLeft(_canvasMyShapes.Width - graphicsShape.GetLeft() - graphicsShape.GetWidth());

                if (graphicsShape.GetTop() + graphicsShape.GetHeight() > _canvasMyShapes.Height)
                    graphicsShape.MoveTop(_canvasMyShapes.Height - graphicsShape.GetTop() - graphicsShape.GetHeight());
            }
        }

        //Load and show all CAD Layers
        private void LoadDxfLayers()
        {
            _lbLayers.Items.Clear();

            foreach (var layer in _model.Layers)
            {
                _lbLayers.Items.Add(layer);
            }
        }

        private void ShowScaleRatioSelector()
        {
            double scale = _canvasMyShapes.LayoutTransform.Value.M11;
            _scaleRatioSelector = new ScaleRatioSelector(_canvasMyShapes);
            _canvasMyShapes.Children.Add(_scaleRatioSelector);
            Canvas.SetLeft(_scaleRatioSelector, (myScrollViewer.HorizontalOffset + 100)/scale);
            Canvas.SetTop(_scaleRatioSelector, (myScrollViewer.VerticalOffset + 100)/scale);
            _scaleRatioSelector.Width = 100/scale;
            _scaleRatioSelector.ChangeSize += scaleRatioSelector_ChangeSize;

            _lCountOfPixels.Content = string.Format("{0} {1} {2}",
                LocalizationHelper.GetString("General_For"),
                _scaleRatioSelector.Width.ToString("F0"),
                LocalizationHelper.GetString("General_Pixels"));

            _bSelectScaleRatio.Visibility = Visibility.Hidden;
            _lSpecifiedScale.Visibility = Visibility.Visible;
            _tbSpecifiedScaleLength.Visibility = Visibility.Visible;
            _tbSpecifiedScaleLength.Text = string.Empty;
            _lCountOfPixels.Visibility = Visibility.Visible;
        }

        private void HideScaleRatioSelector()
        {
            if (_canvasMyShapes.Children.Contains(_scaleRatioSelector))
                _canvasMyShapes.Children.Remove(_scaleRatioSelector);

            _bSelectScaleRatio.Visibility = Visibility.Visible;
            _lSpecifiedScale.Visibility = Visibility.Hidden;
            _tbSpecifiedScaleLength.Visibility = Visibility.Hidden;
            _lCountOfPixels.Visibility = Visibility.Hidden;
        }

        //Calculating of Collision for two Circles
        private bool CircleCollision(Point center1, double radius1, Point center2, double radius2)
        {
            double a = Math.Abs(center1.X - center2.X);
            double b = Math.Abs(center1.Y - center2.Y);
            double c = Math.Pow(a*a + b*b, 0.5);

            if (c < radius1 + radius2)
                return true;
            
            return false;
        }

        private void LoadStateOfLiveObjects()
        {
            try
            {
                if (EditMode)
                {
                    SetStateOfLiveObjectsToDefault();
                    return;
                }
                _canvasMyShapes.IsEnabled = false;
                foreach (var element in _canvasMyShapes.Children)
                {
                    var liveObject = element as ILiveObject;

                    if (liveObject == null || liveObject.GetObjectGuid() == Guid.Empty)
                        continue;

                    var polygon = element as GraphicsPolygon;

                    if (polygon != null
                        && polygon.GetPolygonMode() == PolygonMode.AlarmArea)
                    {
                        polygon.ChangeState(
                            (byte)
                                MainServerProvider.AlarmAreas.GetAlarmAreaActivationState(
                                    liveObject.GetObjectGuid()));
                        continue;
                    }

                    var io = element as GraphicsIO;

                    if (io != null)
                    {
                        if (io.IOType == IOType.Output)
                            io.ChangeState(
                                (byte)
                                    MainServerProvider.Outputs.GetActualStatesByGuid(liveObject.GetObjectGuid()));
                        else
                            io.ChangeState(
                                (byte)
                                    MainServerProvider.Inputs.GetActualStatesByGuid(liveObject.GetObjectGuid()));

                        continue;
                    }

                    var doorEnvironment = element as GraphicsDoorEnvironment;

                    if (doorEnvironment != null)
                    {
                        doorEnvironment.ChangeState(
                            (byte)
                                MainServerProvider.DoorEnvironments.GetDoorEnvironmentState(
                                    liveObject.GetObjectGuid()));
                        continue;
                    }

                    var cardReader = element as GraphicsCardReader;

                    if (cardReader != null)
                    {
                        cardReader.SetOnlineState(
                            MainServerProvider.CardReaders.GetOnlineStates(liveObject.GetObjectGuid()));
                        continue;
                    }

                    var ccu = element as GraphicsCcu;

                    if (ccu != null)
                    {
                        ccu.ChangeState((byte)MainServerProvider.CCUs.GetCCUState(liveObject.GetObjectGuid()));
                        
                        var alarms = MainServerProvider.GetAlarms(
                            AlarmType.CCU_TamperSabotage, 
                            new IdAndObjectType(liveObject.GetObjectGuid(), ObjectType.CCU));

                        if (alarms == null)
                            continue;

                        foreach (var serverAlarm in  alarms)
                            ChangeAlarmState(serverAlarm);
                        
                        continue;
                    }

                    var dcu = element as GraphicsDcu;

                    if (dcu != null)
                    {
                        dcu.ChangeState((byte)MainServerProvider.DCUs.GetOnlineStates(liveObject.GetObjectGuid()));

                        var alarms = MainServerProvider.GetAlarms(
                            AlarmType.DCU_TamperSabotage,
                            new IdAndObjectType(liveObject.GetObjectGuid(), ObjectType.DCU));

                        if (alarms == null)
                            continue;

                        foreach (var serverAlarm in alarms)
                            ChangeAlarmState(serverAlarm);
                    }
                }
            }
            catch (Exception)
            {
                CgpClient.Singleton.IsConnectionLost(true);
            }
            finally
            {
                _canvasMyShapes.IsEnabled = true;
            }
        }

        private void SetStateOfLiveObjectsToDefault()
        {
            for (int i = 0; i < _canvasMyShapes.Children.Count; i++)
            {
                var liveObject = _canvasMyShapes.Children[i] as ILiveObject;

                if (liveObject == null || liveObject.GetObjectGuid() == Guid.Empty)
                    continue;

                liveObject.ChangeState((byte) State.Unknown);
            }
        }

        private void TestDistancePointFromLines(Point mousePoint)
        {
            double maxDistanceFromLine = 5.0 / _canvasMyShapes.LayoutTransform.Value.M11;
            double? nearestDistance = null;
            GraphicsPolygon polygonForSelect = null;

            foreach (var element in _canvasMyShapes.Children)
            {
                var polygon = element as GraphicsPolygon;

                if (polygon == null)
                    continue;

                if (polygon.GetPolygonMode() == PolygonMode.Line
                    || polygon.GetPolygonMode() == PolygonMode.PolyLine)
                {
                    Point closest;

                    for (int i = 0; i < polygon.Points.Count - 1; i++)
                    {
                        var pointA = new Point(polygon.Points[i].X + polygon.GetLeft(), polygon.Points[i].Y + polygon.GetTop());
                        var pointB = new Point(polygon.Points[i + 1].X + polygon.GetLeft(), polygon.Points[i + 1].Y + polygon.GetTop());
                        var distance = GraphicsOperations.FindDistanceToSegment
                            (mousePoint, pointA, pointB, out closest);

                        if (distance < maxDistanceFromLine)
                        {
                            if (nearestDistance != null
                                && nearestDistance.Value < distance)
                            {
                                continue;
                            }

                            nearestDistance = distance;
                            polygonForSelect = polygon;
                        }
                    }
                }
            }

            if (polygonForSelect != null)
            {
                polygonForSelect.Select(true);
                _selectedShape = polygonForSelect;
                _canvasMyShapes.ContextMenu.IsEnabled = false;
            }
        }

        private void _canvasMyShapes_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_moveAll 
                && e.LeftButton == MouseButtonState.Pressed
                && !EditMode)
            {
                BeginPanning(e.GetPosition(this));
                return;
            }

            if (_moveAll)
            {
                var position2 = e.GetPosition(this);
                double deltaX = 0, deltaY = 0;

                if (_lastMousePosX > -1)
                {
                    deltaX = position2.X - _lastMousePosX;
                    deltaY = position2.Y - _lastMousePosY;
                }

                MoveAll(deltaX, deltaY);
                _lastMousePosX = position2.X;
                _lastMousePosY = position2.Y;

                //if (_myDxfVisualizer != null)
                //    _lCountOfLines.Content = "Count of drawn lines: " + _myDxfVisualizer.count.ToString();
            }

            var position = e.GetPosition(_canvasMyShapes);
            
            if (UserInterfaceMode == InterfaceMode.Paint)
            {
                _needSave = true;
                if (position.X > _canvasMyShapes.Width 
                    || position.Y > _canvasMyShapes.Height)
                    return;

                var image = _drawingShape as GraphicsSymbol;

                if ((_drawingShape is RectangleAndEllipse || image != null) && e.LeftButton == MouseButtonState.Pressed)
                {
                    _drawingShape.Select(true);
                    double width = e.GetPosition(_canvasMyShapes).X - _drawingMainPoint.X;
                    double height = e.GetPosition(_canvasMyShapes).Y - _drawingMainPoint.Y;

                    if (image != null)
                    {
                        double aspectRatio = image.Source.Width/image.Source.Height;

                        if (height > 0)
                            height = Math.Abs(width/aspectRatio);
                        else
                            height = -Math.Abs(width/aspectRatio);

                        if (width < 0)
                            _drawingShape.SetLeft(_drawingMainPoint.X + width);
                        else
                            _drawingShape.SetLeft(_drawingMainPoint.X);

                        if (height < 0)
                            _drawingShape.SetTop(_drawingMainPoint.Y + height);
                        else
                            _drawingShape.SetTop(_drawingMainPoint.Y);

                        _drawingShape.SetWidth(Math.Abs(width));
                    }
                    else
                    {
                        _drawingShape.SetLeft(width < 0 ? e.GetPosition(_canvasMyShapes).X : _drawingMainPoint.X);
                        _drawingShape.SetTop(height < 0 ? e.GetPosition(_canvasMyShapes).Y : _drawingMainPoint.Y);
                        _drawingShape.SetWidth(Math.Abs(width));
                        _drawingShape.SetHeight(Math.Abs(height));
                    }
                }
                else
                {
                    _needSave = true;
                    var polygon = _drawingShape as GraphicsPolygon;

                    if (polygon != null)
                    {
                        double scale = _canvasMyShapes.LayoutTransform.Value.M11;

                        if (polygon.Points.Count > 1 
                            && _paintShape != PaintMode.Line
                            && CircleCollision(position, 1.0/scale, polygon.Points[0], 7.0/scale))
                        {
                            polygon.Points[polygon.Points.Count - 1] =
                                (_drawingShape as GraphicsPolygon).Points[0];
                        }
                        else
                            polygon.Points[polygon.Points.Count - 1] =
                                new Point(position.X, position.Y);

                        polygon.Refresh();
                    }
                }
            }
            else if (UserInterfaceMode == InterfaceMode.PolygonAddNewPoint)
            {
                _needSave = true;
                var polygon = _editingPolygonShape as GraphicsPolygon;

                if (polygon != null)
                {
                    if (_insertNewPointPos != -1)
                        polygon.Points.RemoveAt(_insertNewPointPos);

                    double posX = position.X - polygon.GetLeft();
                    double posY = position.Y - polygon.GetTop();
                    _insertNewPointPos = polygon.AddNewPoint(new Point(posX, posY));
                    polygon.Refresh();
                    polygon.UnSelect();
                    polygon.Select(EditMode);

                    if (!IsMouseCaptured)
                        (sender as Canvas).CaptureMouse();

                    _selectedShape = null;
                }
            }
            else
            {
                if (_selectedShape != null)
                {
                    _needSave = true;
                    double deltaX = 0, deltaY = 0;

                    if (_lastMousePosX > -1)
                    {
                        deltaX = position.X - _lastMousePosX;
                        deltaY = position.Y - _lastMousePosY;
                    }

                    _selectedShape.MoveLeft(deltaX);
                    _selectedShape.MoveTop(deltaY);
                    _lastMousePosX = position.X;
                    _lastMousePosY = position.Y;
                }
            }

            if (_informationPanelIsVisible 
                && !_moveAll 
                && _informationPanel.Visibility == Visibility.Collapsed)
                _informationPanel.Visibility = Visibility.Visible;

            //move information panel
            SetInformationPanelPosition(e.GetPosition(_canvasUI));
        }

        private static class CanvasMouseValues
        {
            public static Point LastMousePositionForMouseDownEvent { get; set; }
        }

        private void GoToScene(Guid idScene)
        {
            if (idScene == Guid.Empty)
                return;

            var scene = GetSceneById(idScene);

            if (scene != null)
            {
                Scene = scene;

                if (_idBasicScene != Scene.IdScene)
                {
                    _bSwitchToEditMode.IsEnabled = false;
                    _bSwitchToEditMode.Opacity = 0.5;
                    _bGoHome.IsEnabled = true;
                    _bGoHome.Opacity = 1;
                }
                else
                {
                    _bSwitchToEditMode.IsEnabled = true;
                    _bSwitchToEditMode.Opacity = 1;
                    _bGoHome.IsEnabled = false;
                    _bGoHome.Opacity = 0.5;
                }

                if (_sceneHistory.Count > 0)
                {
                    _bGoBack.IsEnabled = true;
                    _bGoBack.Opacity = 1;
                }
            }
        }

        private void _canvasMyShapes_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var position = e.GetPosition(_canvasMyShapes);
            CanvasMouseValues.LastMousePositionForMouseDownEvent = position;

            if (e.ChangedButton == MouseButton.Left)
            {
                if (UserInterfaceMode == InterfaceMode.Panning)
                {
                    BeginPanning(e.GetPosition(this));
                    return;
                }

                var result = VisualTreeHelper.HitTest(_canvasMyShapes, position);
                var element = result.VisualHit as UIElement;

                if (element == null)
                    return;

                if (e.ClickCount == 2) //double click
                {
                    var sceneLink = element as ISceneLink;

                    if (sceneLink != null
                        && sceneLink.GetSceneId() != Guid.Empty)
                    {
                        if (_onlyViewMode)
                        {
                            var scene = GetSceneById(sceneLink.GetSceneId());

                            if (scene == null)
                                return;

                            NeedSceneChanged(scene);
                            return;
                        }

                        if (EditMode)
                            return;

                        _sceneHistory.AddLast(Scene.IdScene);
                        GoToScene(sceneLink.GetSceneId());
                    }

                    return;
                }

                if ((result.VisualHit == _canvasMyShapes || result.VisualHit == _backgroundImage
                     || element.Visibility == Visibility.Hidden) && (element as Text) == null)
                {
                    UnSelectAllShapes();
                    var canvas = sender as Canvas;

                    if (canvas != null && canvas.IsMouseCaptured)
                        canvas.ReleaseMouseCapture();

                    if (EditMode)
                        TestDistancePointFromLines(position);
                }

                if (UserInterfaceMode == InterfaceMode.Paint && _selectedLayer != null)
                {
                    if (_drawingShape == null)
                    {
                        UnSelectAllShapes();

                        switch (_paintShape)
                        {
                            case PaintMode.Rectangle:
                                _drawingShape = new RectangleAndEllipse(_canvasMyShapes, DrawMode.rectangle);
                                break;
                            case PaintMode.Ellipse:
                                _drawingShape = new RectangleAndEllipse(_canvasMyShapes, DrawMode.ellipse);
                                break;
                            case PaintMode.Line:
                                _drawingShape = new GraphicsPolygon(_canvasMyShapes, PolygonMode.Line);
                                break;
                            case PaintMode.Polyline:
                                _drawingShape = new GraphicsPolygon(_canvasMyShapes, PolygonMode.PolyLine);
                                break;
                            case PaintMode.AlarmAreaPolygon:
                                _drawingShape = new GraphicsPolygon(_canvasMyShapes, PolygonMode.AlarmArea);
                                break;
                            case PaintMode.Image:
                                if (_paintSymbolType == SymbolType.CardReader)
                                    _drawingShape = new GraphicsCardReader(_canvasMyShapes, _symbolCache);
                                else if (_paintObjectType == ObjectType.Output || _paintObjectType == ObjectType.Input)
                                    _drawingShape = new GraphicsIO(_canvasMyShapes, _symbolCache, _paintSymbolType);
                                else if (_paintSymbolType == SymbolType.DoorEnviromentLeft)
                                    _drawingShape = new GraphicsDoorEnvironment(_canvasMyShapes, _symbolCache, true);
                                else if (_paintSymbolType == SymbolType.DoorEnviromentRight)
                                    _drawingShape = new GraphicsDoorEnvironment(_canvasMyShapes, _symbolCache, false);
                                else if (_paintObjectType == ObjectType.CCU)
                                    _drawingShape = new GraphicsCcu(_canvasMyShapes, _symbolCache, _paintSymbolType);
                                else if (_paintObjectType == ObjectType.DCU)
                                    _drawingShape = new GraphicsDcu(_canvasMyShapes, _symbolCache, _paintSymbolType);
                                break;
                            case PaintMode.Text:
                                return;
                        }

                        _paintObjectType = null;
                        var liveObject = _drawingShape as ILiveObject;

                        if (liveObject != null)
                        {
                            if (_newObjectGuid != null)
                                liveObject.SetObjectGuid(_newObjectGuid.Value);

                            if (_newObjectCategories != null)
                                liveObject.SetCategories(_newObjectCategories);
                            else
                            {
                                var categories = new List<Category>
                                {
                                    Category.AccessControl,
                                    Category.Accessories,
                                    Category.BurglarAlarm,
                                    Category.Fire
                                };

                                liveObject.SetCategories(categories);
                            }

                            _newObjectGuid = null;
                            _newObjectCategories = null;
                        }

                        _drawingShape.SetLeft(position.X);
                        _drawingShape.SetTop(position.Y);
                        _drawingMainPoint = new Point(position.X, position.Y);
                        element = (UIElement)_drawingShape;
                        element.MouseDown += graphicsObjects_MouseDown;
                        element.MouseUp += GraphicsControl_MouseUp;
                        element.MouseEnter += GraphicsControl_MouseEnter;
                        element.MouseLeave += GraphicsControl_MouseLeave;
                        _drawingShape.ObjectChanged += _drawingShape_ObjectChanged;
                        
                        //add context menu
                        AddContextMenu(element as FrameworkElement);
                        _drawingShape.SetLayerID(_selectedLayer.Id);
                        _canvasMyShapes.Children.Add(element);

                        var io = _drawingShape as GraphicsIO;

                        if (io != null)
                        {
                            io.IOType = _paintIoType;
                            return;
                        }

                        var polygon = _drawingShape as GraphicsPolygon;

                        if (polygon != null)
                        {
                            polygon.Stroke = new SolidColorBrush(System.Windows.Media.Colors.Black);
                            polygon.StrokeThickness = 1/_canvasMyShapes.LayoutTransform.Value.M11;
                            polygon.Points.Add(_drawingMainPoint);
                            polygon.Points.Add(_drawingMainPoint);
                            return;
                        }

                        var geometry = _drawingShape as RectangleAndEllipse; //rectangle or ellipse

                        if (geometry != null)
                        {
                            geometry.Stroke =
                                new SolidColorBrush(CanvasSerialization.CanvasSettings.defaultLineColor.GetColor());
                            geometry.StrokeThickness = CanvasSerialization.CanvasSettings.defaultLineWidth/
                                                       _canvasMyShapes.LayoutTransform.Value.M11;
                            geometry.Fill =
                                new SolidColorBrush(
                                    CanvasSerialization.CanvasSettings.defaultBackgroundColor.GetColor());
                        }
                    }
                    else
                    {
                        var polygon = _drawingShape as GraphicsPolygon;

                        if (polygon != null)
                        {
                            double scale = _canvasMyShapes.LayoutTransform.Value.M11;
                            bool isClosedPolygon = CircleCollision(position, 1.0/scale, polygon.Points[0], 7.0/scale);

                            if (polygon.Points.Count > 1
                                && (isClosedPolygon
                                || polygon.GetPolygonMode() == PolygonMode.Line))
                            {
                                if (isClosedPolygon)
                                {
                                    polygon.Points.RemoveAt(polygon.Points.Count - 1);
                                    polygon.IsClosed = true;
                                }

                                polygon.Refresh();
                                polygon.UnSelect();
                                _drawingShape = null;
                                UserInterfaceMode = InterfaceMode.Select;
                            }
                            else
                                polygon.Points.Add(new Point(position.X, position.Y));
                        }
                    }
                }
            }
            else if (e.ChangedButton == MouseButton.Middle)
            {
                BeginPanning(e.GetPosition(this));
            }
        }

        void _drawingShape_ObjectChanged()
        {
            _needSave = true;
        }

        private void BeginPanning(Point position)
        {
            GraphicsObjects.Singleton.LockCursorChanged(_canvasMyShapes);
            GraphicsObjects.Singleton.SetCursorForAllGraphicsObject(_canvasMyShapes, Cursors.ScrollAll);
            _selectedShape = null;
            _moveAll = true;
            _informationPanel.Visibility = Visibility.Collapsed;
            _lastMousePosX = position.X;
            _lastMousePosY = position.Y;
        }

        private void GraphicsControl_MouseLeave(object sender, MouseEventArgs e)
        {
            if (EditMode)
                return;

            _informationPanel.Visibility = Visibility.Collapsed;
            ((IGraphicsObject) sender).UnSelect();
            _lastSelectedShape = null;
            _informationPanelIsVisible = false;
        }

        private void GraphicsControl_MouseEnter(object sender, MouseEventArgs e)
        {
            var liveObject = sender as ILiveObject;

            if (liveObject == null 
                || EditMode 
                || _moveAll
                || liveObject.GetObjectGuid() == Guid.Empty)
                return;

            _informationPanelIsVisible = true;
            _lastSelectedShape = liveObject;
            ((IGraphicsObject) sender).Select(EditMode);
            string objectName = "None";
            string objectState = "Unknown";

            if (liveObject.GetObjectGuid() != Guid.Empty)
                objectState = (string) liveObject.GetState();

            var obj =
                CgpClient.Singleton.MainServerProvider.GetTableObject(liveObject.GetObjectType(),
                    liveObject.GetObjectGuid().ToString());

            _informationPanel.ObjectType = LocalizationHelper.GetString("ObjectType_" + liveObject.GetObjectType());

            if (obj != null)
                objectName = obj.ToString();

            _informationPanel.ObjectName = objectName;
            _informationPanel.ObjectState = objectState;
            _informationPanel.Visibility = Visibility.Visible;

            if (!_canvasUI.Children.Contains(_informationPanel))
                _canvasUI.Children.Add(_informationPanel);

            SetInformationPanelPosition(e.GetPosition(_canvasUI));
            _informationPanel.Draw();
        }

        private void InformationPanelUpdateInfo()
        {
            if (_lastSelectedShape == null || EditMode || _moveAll)
                return;

            string objectState = "Unknown";

            if (_lastSelectedShape.GetObjectGuid() != Guid.Empty)
                objectState = (string) _lastSelectedShape.GetState();

            _informationPanel.ObjectState = objectState;
            _informationPanel.Draw();
        }

        private void SetInformationPanelPosition(Point position)
        {
            Canvas.SetLeft(_informationPanel, position.X + 5);
            Canvas.SetTop(_informationPanel, position.Y + 5);
        }

        public static Scene GetSceneById(Guid idScene)
        {
            try
            {
                return MainServerProvider.Scenes.GetObjectById(idScene);
            }
            catch (Exception)
            {
                CgpClient.Singleton.IsConnectionLost(true);
                return null;
            }
        }

        private void GraphicsControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //var contextMenu = (sender as FrameworkElement).ContextMenu;

            //if (!EditMode && (sender is ILiveObject) && contextMenu != null && e.ChangedButton == MouseButton.Left)
            //{
            //    contextMenu.PlacementTarget = this;
            //    contextMenu.IsOpen = true;
            //}
        }

        private ShapeSettings _shapeSettingsPanel;
        private TextSettings _textSettingsPanel;

        private void cm_ShapeSettingsClick(UIElement sender)
        {
            var text = sender as Text;

            if (text != null)
            {
                if (_textSettingsPanel == null)
                {
                    _textSettingsPanel = new TextSettings();
                    _textSettingsPanel.Closed += _textSettingsPanel_Closed;
                    _textSettingsPanel.ApplyClick += new Action(_textSettingsPanel_ApplyClick);
                    _textSettingsPanel.HorizontalAlignment = HorizontalAlignment.Right;
                    _textSettingsPanel.VerticalAlignment = VerticalAlignment.Stretch;
                    myGrid.Children.Add(_textSettingsPanel);
                    Grid.SetColumn(_textSettingsPanel, 2);
                    Grid.SetRow(_textSettingsPanel, 1);
                    Grid.SetRowSpan(_textSettingsPanel, 2);
                    Grid.SetZIndex(_textSettingsPanel, 99);
                }

                _textSettingsPanel.LoadTextSettings(text, CanvasSerialization);
                _textSettingsPanel.Visibility = Visibility.Visible;

                if (_shapeSettingsPanel != null && _shapeSettingsPanel.Visibility == Visibility.Visible)
                    _shapeSettingsPanel.Visibility = Visibility.Collapsed;

                return;
            }

            if (_shapeSettingsPanel == null)
            {
                _shapeSettingsPanel = new ShapeSettings();
                _shapeSettingsPanel.Closed += ShapeSettingsWindow_Closed;
                _shapeSettingsPanel.ApplyClick += _shapeSettingsPanel_ApplyClick;
                _shapeSettingsPanel.HorizontalAlignment = HorizontalAlignment.Right;
                _shapeSettingsPanel.VerticalAlignment = VerticalAlignment.Stretch;
                myGrid.Children.Add(_shapeSettingsPanel);
                Grid.SetColumn(_shapeSettingsPanel, 2);
                Grid.SetRow(_shapeSettingsPanel, 1);
                Grid.SetRowSpan(_shapeSettingsPanel, 2);
                Grid.SetZIndex(_shapeSettingsPanel, 99);
            }

            _shapeSettingsPanel.LoadSettings(sender, CanvasSerialization);
            _shapeSettingsPanel.Visibility = Visibility.Visible;

            if (_textSettingsPanel != null && _textSettingsPanel.Visibility == Visibility.Visible)
                _textSettingsPanel.Visibility = Visibility.Collapsed;
        }

        void _textSettingsPanel_ApplyClick()
        {
            _needSave = true;
        }

        void _shapeSettingsPanel_ApplyClick()
        {
            _needSave = true;
        }

        private void _textSettingsPanel_Closed(TextSettings sender)
        {
            sender.Visibility = Visibility.Collapsed;
        }

        private void ShapeSettingsWindow_Closed(ShapeSettings sender)
        {
            sender.Visibility = Visibility.Collapsed;
        }

        private int _insertNewPointPos = -1;

        private void _canvasMyShapes_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            _moveAll = false;
            _lastMousePosX = -1;
            _lastMousePosY = -1;
            _canvasMyShapes.Cursor = Cursors.Arrow;
            GraphicsObjects.Singleton.UnlockCursorChanged(_canvasMyShapes);

            if (e == null && UserInterfaceMode != InterfaceMode.PolygonAddNewPoint)
            {
                _drawingShape = null;
                _selectedShape = null;
                return;
            }

            var position = e.GetPosition(_canvasMyShapes);

            if (e.ChangedButton == MouseButton.Left)
            {
                if (UserInterfaceMode == InterfaceMode.Paint)
                {
                    if (_paintShape == PaintMode.Text)
                    {
                        var text = new Text(_canvasMyShapes);
                        text.SetLeft(position.X);
                        text.SetTop(position.Y);
                        text.Text = "TEXT";
                        text.SetLayerID(_selectedLayer.Id);
                        double fontSize = 12/_canvasMyShapes.LayoutTransform.Value.M11;

                        if (fontSize < FontMinSize)
                            fontSize = FontMinSize;

                        text.FontSize = fontSize;
                        UserInterfaceMode = InterfaceMode.Select;
                        _canvasMyShapes.Children.Add(text);
                        text.MouseDown += graphicsObjects_MouseDown;
                        AddContextMenu(text);
                    }
                    else if (_drawingShape is RectangleAndEllipse || _drawingShape is GraphicsSymbol)
                    {
                        double zoom = _canvasMyShapes.LayoutTransform.Value.M11;

                        if (_drawingShape.GetWidth()*zoom < _displayWidth)
                            _drawingShape.SetWidth(_displayWidth/zoom);

                        _drawingShape.Select(true);
                        UserInterfaceMode = InterfaceMode.Select;
                        _drawingShape = null;
                    }
                }
                else if (UserInterfaceMode == InterfaceMode.PolygonAddNewPoint)
                {
                    UserInterfaceMode = InterfaceMode.Select;
                    ((Canvas) sender).ReleaseMouseCapture();
                    _insertNewPointPos = -1;
                }
                else
                {
                    _selectedShape = null;
                }
            }
            else if (e.ChangedButton == MouseButton.Right)
            {
                var selectedShapes = GetSelectedShapes(_canvasMyShapes);

                if (selectedShapes.Count > 0)
                {
                    var element = selectedShapes[selectedShapes.Count - 1] as FrameworkElement;

                    if (element != null)
                    {
                        element.ContextMenu.IsOpen = true;
                    }
                }
            }
        }

        public ICollection<Guid> GetObjectsGuid()
        {
            var guids = new HashSet<Guid>();

            foreach (var element in _canvasMyShapes.Children)
            {
                var liveObject = element as ILiveObject;

                if (liveObject != null)
                    guids.Add(liveObject.GetObjectGuid());
            }

            return guids;
        }

        public void SaveScene()
        {
            if (_cadVisualizer != null || _backgroundBitmap != null)
            {
                //save the screen of scene
                Point lastScrollPos = new Point(myScrollViewer.ContentHorizontalOffset,
                    myScrollViewer.ContentVerticalOffset);
                double lastScale = _canvasMyShapes.LayoutTransform.Value.M11;
                double requiredHeight = 240;
                double scale = requiredHeight/_canvasMyShapes.Height;
                SetScale(scale);
                Scene.SceneScreenRowData = CanvasSerialization.SaveScreen(_canvasBackground, _canvasMyShapes).ToArray();
                SetScale(lastScale);
                myScrollViewer.ScrollToHorizontalOffset(lastScrollPos.X);
                myScrollViewer.ScrollToVerticalOffset(lastScrollPos.Y);
            }

            if (_cadVisualizer != null)
            {
                Scene.RowDataBackground = _cadVisualizer.SaveToArray();
                Scene.BackgroundDataType = SymbolDataType.Vector;
            }
            else if (_backgroundBitmap != null && _backgroundBitmap.UriSource != null)
            {
                byte[] data = File.ReadAllBytes(_backgroundBitmap.UriSource.LocalPath);
                Scene.RowDataBackground = data;
                Scene.BackgroundDataType = SymbolDataType.Raster;
            }

            Scene.RowData = CanvasSerialization.SaveToArray(_canvasMyShapes);
        }

        private readonly ScaleInfo _scaleInfo = new ScaleInfo(1.0, 1.0);

        private void LoadModel()
        {
            if (Scene.RowDataBackground == null)
                return;

            //Load background RawData
            if (Scene.BackgroundDataType == SymbolDataType.Vector)
            {
                _cadVisualizer = new CadVisualizer();
                var wbmWidth = Convert.ToInt32(myGrid.ColumnDefinitions[2].ActualWidth);
                var wbmHeight =
                    Convert.ToInt32(myGrid.RowDefinitions[1].ActualHeight + myGrid.RowDefinitions[2].ActualHeight);

                _writableBitmap = BitmapFactory.New(wbmWidth == 0 ? 320 : wbmWidth, wbmHeight == 0 ? 240 : wbmHeight);
                _cadVisualizer.WriteableBitmap = _writableBitmap;
                _backgroundImage.Source = _writableBitmap;
                _canvasBackground.Children.Add(_backgroundImage);
            }
            else if (Scene.BackgroundDataType == SymbolDataType.Raster)
            {
                using (var stream = new MemoryStream(Scene.RowDataBackground))
                {
                    _backgroundBitmap = new BitmapImage();
                    _backgroundBitmap.BeginInit();
                    _backgroundBitmap.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                    _backgroundBitmap.CacheOption = BitmapCacheOption.OnLoad;
                    _backgroundBitmap.StreamSource = stream;
                    _backgroundBitmap.EndInit();
                }
                _backgroundImage.Source = _backgroundBitmap;
            }

            if (_cadVisualizer == null && _backgroundBitmap == null)
                return;

            ResetOffset();
            _canvasUI.Children.Add(_scaleInfo);
            CanvasSerialization.LoadCanvasSettings(Scene.RowData);
            LoadSymbols();

            if (Scene.BackgroundDataType == SymbolDataType.Vector 
                && _cadVisualizer != null)
            {
                _cadVisualizer.LoadFromArray(Scene.RowDataBackground);
                _canvasMyShapes.Width = _cadVisualizer.ModelWidth;
                _canvasMyShapes.Height = _cadVisualizer.ModelHeight;
                _canvasMyShapes.LayoutTransform = new ScaleTransform(_cadVisualizer.ActualZoom(),
                    _cadVisualizer.ActualZoom());
                _scaleInfo.SetScaleRatio(1.0);
                _scaleInfo.SetScale(_cadVisualizer.ActualZoom());
                _cadVisualizer.Draw();
            }
            else if (Scene.BackgroundDataType == SymbolDataType.Raster 
                && _backgroundBitmap != null)
            {
                _canvasMyShapes.Children.Add(_backgroundImage);
                Canvas.SetZIndex(_backgroundImage, -2);
                _canvasMyShapes.Width = _backgroundImage.Source.Width;
                _canvasMyShapes.Height = _backgroundImage.Source.Height;
                _canvasMyShapes.LayoutTransform = new ScaleTransform(GetOptimalZoom(), GetOptimalZoom());
                _scaleInfo.SetScaleRatio(CanvasSerialization.CanvasSettings.ModelLength*1000/_backgroundBitmap.Width);
                _scaleInfo.SetScale(_canvasMyShapes.LayoutTransform.Value.M11);
            }

            //Load symbols
            if (CanvasSerialization.LoadFromArray(_canvasMyShapes, Scene.RowData, _symbolCache))
            {
                foreach (UIElement shape in _canvasMyShapes.Children)
                {
                    if ((shape as IGraphicsObject) == null)
                        continue;
                    
                    //add event for mouse click
                    shape.MouseDown += graphicsObjects_MouseDown;
                    shape.MouseUp += GraphicsControl_MouseUp;
                    shape.MouseEnter += GraphicsControl_MouseEnter;
                    shape.MouseLeave += GraphicsControl_MouseLeave;
                    ((IGraphicsObject) shape).ObjectChanged += _drawingShape_ObjectChanged;
                    
                    //add context menu
                    AddContextMenu(shape as FrameworkElement);
                }

                SetShapesVisibilityOnCanvasByLayers();
                SetSymbolSizeMode();
                SetAccessForLiveObject();
            }

            LoadCanvasLayers();
            _isModelLoaded = true;
            EnablePainting(true);
            _chbShowAll.IsEnabled = true;
            UnsetInsertBackgroundModeVisibility();
            LoadStateOfLiveObjects();

            if (LoadedModel != null)
                LoadedModel();
        }

        private void EnablePainting(bool isEnable)
        {
            double opacity = isEnable 
                ? 1.0 
                : 0.5;

            _bUndo.IsEnabled = isEnable;
            _bUndo.Opacity = opacity;
            _bDelete.IsEnabled = isEnable;
            _bDelete.Opacity = opacity;
            _bPaintLine.IsEnabled = isEnable;
            _bPaintLine.Opacity = opacity;
            _bPaintPolyline.IsEnabled = isEnable;
            _bPaintPolyline.Opacity = opacity;
            _bPaintRectangle.IsEnabled = isEnable;
            _bPaintRectangle.Opacity = opacity;
            _bPaintEllipse.IsEnabled = isEnable;
            _bPaintEllipse.Opacity = opacity;
            _bInsertText.IsEnabled = isEnable;
            _bInsertText.Opacity = opacity;
            _bPaintAlarmArea.IsEnabled = isEnable;
            _bPaintAlarmArea.Opacity = opacity;
            _bPaintSymbol.IsEnabled = isEnable;
            _bPaintSymbol.Opacity = opacity;
        }

        private void AddContextMenu(FrameworkElement shape)
        {
            if (shape == null)
                return;

            var cm = new GraphicsObjectContextMenu(shape, EditMode);
            shape.ContextMenu = cm.GetContextMenu();
            cm.ShapeSettingsClick += cm_ShapeSettingsClick;
            cm.MenuItemClick += cm_MenuItemClick;
        }

        private void SetAccessForLiveObject()
        {
            var liveObjects = GetLiveObjects(_canvasMyShapes);

            foreach (var liveObject in liveObjects)
            {
                var access = GetAccess(liveObject);

                if (access == null || !access.Value)
                {
                    var element = liveObject as FrameworkElement;

                    if (element == null)
                        continue;

                    if (access != null)
                        element.ContextMenu = null;

                    if (EditMode)
                    {
                        if (access == null)
                        {
                            liveObject.SetObjectGuid(Guid.Empty);
                        }
                        else
                        {
                            liveObject.Enable(false);
                        }
                    }
                    else
                        element.Visibility = Visibility.Collapsed;
                }
            }
        }

        private bool? GetAccess(ILiveObject obj)
        {
            if (obj == null)
                return null;

            if (CgpClient.Singleton.MainServerProvider.HasAccessView(obj.GetObjectType(), obj.GetObjectGuid()))
                return true;

            if (ObjectExists(obj.GetObjectType(), obj.GetObjectGuid()))
                return false;
            
            return null;
        }

        private bool ObjectExists(ObjectType type, Guid id)
        {
            if (id == Guid.Empty)
                return false;

            bool exist = false;

            switch (type)
            {
                case ObjectType.CardReader:
                    if (MainServerProvider.CardReaders.GetObjectById(id) != null)
                        exist = true;
                    break;

                case ObjectType.DoorEnvironment:
                    if (MainServerProvider.DoorEnvironments.GetObjectById(id) != null)
                        exist = true;
                    break;

                case ObjectType.Input:
                    if (MainServerProvider.Inputs.GetObjectById(id) != null)
                        exist = true;
                    break;

                case ObjectType.Output:
                    if (MainServerProvider.Outputs.GetObjectById(id) != null)
                        exist = true;
                    break;

                case ObjectType.AlarmArea:
                    if (MainServerProvider.AlarmAreas.GetObjectById(id) != null)
                        exist = true;
                    break;
            }

            return exist;
        }

        private void cm_MenuItemClick(UIElement sender, ContextMenuAction action)
        {
            var liveObject = sender as ILiveObject;
            var graphicsObject = sender as IGraphicsObject;

            switch (action)
            {
                case ContextMenuAction.EditObject:
                    if (EditObjectClick != null)
                    {
                        if (liveObject != null)
                            EditObjectClick(liveObject.GetObjectType(), liveObject.GetObjectGuid());
                    }
                    break;

                case ContextMenuAction.AddNewPoint:
                    _editingPolygonShape = graphicsObject;
                    UserInterfaceMode = InterfaceMode.PolygonAddNewPoint;
                    break;

                case ContextMenuAction.RemovePoint:
                    _editingPolygonShape = graphicsObject;
                    UserInterfaceMode = InterfaceMode.PolygonRemovePoint;
                    GraphicsThumb.DeleteMode = true;
                    break;

                case ContextMenuAction.AddLabelForObject:

                    if (liveObject == null)
                        return;

                    string name = CgpClient.Singleton.MainServerProvider.GetTableObject(liveObject.GetObjectType(),
                        liveObject.GetObjectGuid().ToString()).ToString();

                    if (!string.IsNullOrEmpty(name))
                    {
                        var shape = sender as IGraphicsObject;

                        if (shape == null)
                            return;

                        var text = new Text(_canvasMyShapes);
                        text.Text = name;
                        text.SetLayerID(shape.GetLayerID());
                        liveObject.SetLabel(text);
                        text.LiveObject = liveObject;

                        double fontSize = 12/_canvasMyShapes.LayoutTransform.Value.M11;

                        if (fontSize < FontMinSize)
                            fontSize = FontMinSize;

                        text.FontSize = fontSize;
                        _canvasMyShapes.Children.Add(text);
                        text.UpdateLayout();
                        double left;
                        double top;
                        var graphicsPolygon = sender as GraphicsPolygon;

                        if (graphicsPolygon != null)
                        {
                            left = graphicsPolygon.GetMinimumLeftPoint() + shape.GetLeft() +
                                   shape.GetWidth()/2 - text.GetWidth()/2;
                            top = graphicsPolygon.GetMinimumTopPoint() + shape.GetTop() +
                                  shape.GetHeight()/2 - text.GetHeight()/2;
                        }
                        else
                        {
                            left = shape.GetLeft() + shape.GetWidth()/2 - text.GetWidth()/2;
                            top = shape.GetTop() + shape.GetHeight();
                        }

                        if (left < 0)
                            left = 0;

                        if (top > _canvasMyShapes.ActualHeight)
                            top = _canvasMyShapes.ActualHeight - text.GetHeight();

                        text.SetLeft(left);
                        text.SetTop(top);
                        var rotateTransform = sender.RenderTransform as RotateTransform;

                        if (rotateTransform != null)
                        {
                            var textRotateTransform = new RotateTransform();
                            textRotateTransform.CenterX = shape.GetLeft() + shape.GetWidth()/2 - left;
                            textRotateTransform.CenterY = shape.GetTop() + shape.GetHeight()/2 - top;
                            textRotateTransform.Angle = rotateTransform.Angle;
                            text.RenderTransform = textRotateTransform;

                            var newCenterPoint = new Point(text.GetLeft() + text.GetWidth()/2,
                                text.GetTop() + text.GetHeight()/2);
                            var newTransform = new RotateTransform(textRotateTransform.Angle,
                                textRotateTransform.CenterX + text.GetLeft(),
                                textRotateTransform.CenterY + text.GetTop());

                            newCenterPoint = newTransform.Transform(newCenterPoint);
                            text.SetLeft(newCenterPoint.X - text.GetWidth()/2);
                            text.SetTop(newCenterPoint.Y - text.GetHeight()/2);
                            textRotateTransform.CenterX = text.GetWidth()/2;
                            textRotateTransform.CenterY = text.GetHeight()/2;
                        }

                        text.MouseDown += graphicsObjects_MouseDown;
                        AddContextMenu(text);
                    }
                    break;

                case ContextMenuAction.TimeBuying:
                    OpenTimeBuyingPanel(sender);
                    break;
            }
        }

        private void OpenTimeBuyingPanel(UIElement element)
        {
            var polygon = element as GraphicsPolygon;

            if (polygon == null)
                return;

            var timeBuyingPanel = new TimeBuyingControl(polygon.GetObjectGuid());
            timeBuyingPanel.HorizontalAlignment = HorizontalAlignment.Left;
            timeBuyingPanel.VerticalAlignment = VerticalAlignment.Top;
            double scale = _canvasMyShapes.LayoutTransform.Value.M11;

            double left = (CanvasMouseValues.LastMousePositionForMouseDownEvent.X -
                           myScrollViewer.ContentHorizontalOffset/scale)*scale;
            double top = (CanvasMouseValues.LastMousePositionForMouseDownEvent.Y -
                          myScrollViewer.ContentVerticalOffset/scale)*scale;

            if (left + timeBuyingPanel.Width > myGrid.ColumnDefinitions[2].ActualWidth)
                left = myGrid.ColumnDefinitions[2].ActualWidth - timeBuyingPanel.Width - 5;

            if (top + timeBuyingPanel.Height > myGrid.RowDefinitions[1].ActualHeight + myGrid.RowDefinitions[2].ActualHeight)
                top = myGrid.RowDefinitions[1].ActualHeight + myGrid.RowDefinitions[2].ActualHeight - timeBuyingPanel.Height - 5;

            timeBuyingPanel.Margin = new Thickness(left, top, 0, 0);
            timeBuyingPanel.Closed += timeBuyingPanel_Closed;
            myGrid.Children.Add(timeBuyingPanel);
            Grid.SetColumn(timeBuyingPanel, 2);
            Grid.SetRow(timeBuyingPanel, 1);
            Grid.SetRowSpan(timeBuyingPanel, 2);
            Grid.SetZIndex(timeBuyingPanel, 99);
            _canvasMyShapes.IsEnabled = false;
        }

        void timeBuyingPanel_Closed(TimeBuyingControl sender)
        {
            myGrid.Children.Remove(sender);
            _canvasMyShapes.IsEnabled = true;
        }

        private void FillListOfCategories()
        {
            if (CanvasSerialization.Categories == null || CanvasSerialization.Categories.Count == 0)
            {
                if (CanvasSerialization.Categories == null)
                    CanvasSerialization.Categories = new Dictionary<Category, bool>();

                CanvasSerialization.Categories.Add(Category.AccessControl, true);
                CanvasSerialization.Categories.Add(Category.Accessories, true);
                CanvasSerialization.Categories.Add(Category.BurglarAlarm, true);
                CanvasSerialization.Categories.Add(Category.Fire, true);
            }

            var categories = new List<CategoryInfo>();
            categories.Add(new CategoryInfo(Category.AccessControl,
                CanvasSerialization.Categories[Category.AccessControl]));
            categories.Add(new CategoryInfo(Category.Accessories,
                CanvasSerialization.Categories[Category.Accessories]));
            categories.Add(new CategoryInfo(Category.BurglarAlarm,
                CanvasSerialization.Categories[Category.BurglarAlarm]));
            categories.Add(new CategoryInfo(Category.Fire,
                CanvasSerialization.Categories[Category.Fire]));
            _lbCategories.ItemsSource = categories;
        }

        private void SetSymbolSizeMode()
        {
            if (EditMode)
                return;

            switch (CanvasSerialization.CanvasSettings.DefaultSymbolSize)
            {
                case SymbolSize.Variable:
                    _displayMode = SymbolSize.Variable;
                    SetSizeForVariableSizeMode();
                    _lSizeOfSymbolInFixedSizeMode.Visibility = Visibility.Hidden;
                    _sliderSizeOfSymbol.Visibility = Visibility.Hidden;
                    break;
                case SymbolSize.Fixed:
                    _displayMode = SymbolSize.Fixed;
                    SetSizeForFixedSizeMode();
                    _lSizeOfSymbolInFixedSizeMode.Visibility = Visibility.Visible;
                    _sliderSizeOfSymbol.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void LoadCanvasLayers()
        {
            if (CanvasSerialization.Layers == null || CanvasSerialization.Layers.Count == 0)
            {
                CanvasSerialization.Layers = new Dictionary<Guid, Layer>();
                var newLayer = new Layer() {Name = "default"};
                CanvasSerialization.Layers.Add(newLayer.Id, newLayer);
            }

            _lbLayers.Items.Clear();

            foreach (var layer in CanvasSerialization.Layers.Values)
            {
                _lbLayers.Items.Add(layer);
            }

            if (_lbLayers.Items.Count > 0)
                _selectedLayer = _lbLayers.Items[0] as Layer;
        }

        public void SetShapesVisibilityOnCanvasByLayer(Guid idLayer, bool isEnable)
        {
            Layer layer;
            CanvasSerialization.Layers.TryGetValue(idLayer, out layer);

            if (layer == null)
                return;

            layer.Enabled = isEnable;
            var graphicsObjects = GraphicsObjects.Singleton.GetGraphicsObjectsByLayer(_canvasMyShapes, idLayer);

            foreach (var graphicsObject in graphicsObjects)
            {
                bool isVisibleByCategory = true;
                var liveObject = graphicsObject as ILiveObject;

                if (liveObject != null)
                    isVisibleByCategory = IsShapeVisibleByCategories(liveObject);

                if (layer.Enabled && isVisibleByCategory)
                    (graphicsObject as UIElement).Visibility = Visibility.Visible;
                else
                {
                    graphicsObject.UnSelect();
                    (graphicsObject as UIElement).Visibility = Visibility.Collapsed;
                }
            }
        }

        public void SetShapesVisibilityOnCanvasByLayers()
        {
            var graphicsObjects = GraphicsObjects.Singleton.GetGraphicsObjects(_canvasMyShapes);

            foreach (var graphicsObject in graphicsObjects)
            {
                if (CanvasSerialization.Layers == null)
                    continue;

                Layer layer;
                CanvasSerialization.Layers.TryGetValue(graphicsObject.GetLayerID(), out layer);

                if (layer == null)
                    continue;

                bool isVisibleByCategory = true;
                var liveObject = graphicsObject as ILiveObject;

                if (liveObject != null)
                    isVisibleByCategory = IsShapeVisibleByCategories(liveObject);

                if (layer.Enabled && isVisibleByCategory)
                    (graphicsObject as UIElement).Visibility = Visibility.Visible;
                else
                {
                    graphicsObject.UnSelect();
                    (graphicsObject as UIElement).Visibility = Visibility.Collapsed;
                }
            }
        }

        private bool IsShapeVisibleByCategories(ILiveObject shape)
        {
            if (shape == null)
                return false;

            var enableCategories = CanvasSerialization.GetEnableCategories();

            if (enableCategories == null)
                return false;

            var shapeCategories = shape.GetCategories();

            return enableCategories.Any(shapeCategories.Contains);
        }

        private void _chbShowAll_OnClick(object sender, RoutedEventArgs e)
        {
            bool isCadLayer = _lbLayers.Items[0] is DxfLayer;

            foreach (var item in _lbLayers.Items)
            {
                if (isCadLayer)
                {
                    var dxfLayer = item as DxfLayer;
                    dxfLayer.Enabled = _chbShowAll.IsChecked.Value;
                }
                else
                {
                    var layer = item as Layer;
                    layer.Enabled = _chbShowAll.IsChecked.Value;
                }
            }

            _lbLayers.Items.Refresh();

            if (isCadLayer)
            {
                ResetOffset();
                IwQuick.Threads.SafeThread<DxfModel, bool>.StartThread(LoadModel, _model, false);
            }
            else
                SetShapesVisibilityOnCanvasByLayers();
        }

        private void ResetOffset()
        {
            _lastMousePosX = _lastMousePosY = -1;
            _offsetX = _offsetY = 0;

            if (Scene.BackgroundDataType == SymbolDataType.Vector 
                && _cadVisualizer != null)
                _cadVisualizer.SetOffset(Convert.ToInt32(_offsetX), Convert.ToInt32(_offsetY));
        }

        private void Zoom(int percent, Point zoomPoint)
        {
            if (_cadVisualizer == null && _backgroundBitmap == null)
                return;

            double zoom = percent/100d;
            double newScale = _canvasMyShapes.LayoutTransform.Value.M11*zoom;

            if (newScale < _defaultScale)
            {
                newScale = _defaultScale;
                zoom = _defaultScale/_canvasMyShapes.LayoutTransform.Value.M11;
            }

            _cbZoomValues.SelectedIndex = -1;
            List<IGraphicsObject> myShapes = GetSelectedShapes(_canvasMyShapes);

            if (myShapes == null)
                return;

            foreach (IGraphicsObject shape in myShapes)
                shape.UnSelect();

            //zoom CAD model
            if (Scene.BackgroundDataType == SymbolDataType.Vector 
                && _cadVisualizer != null)
            {
                _offsetX = _offsetX*zoom;
                _offsetY = _offsetY*zoom;
                _cadVisualizer.SetOffset(_offsetX, _offsetY);
                _cadVisualizer.SetZoom(zoom);
            }

            //zoom canvas
            double lastCanvasWidth = _canvasMyShapes.ActualWidth;
            double lastCanvasHeight = _canvasMyShapes.ActualHeight;

            var scale = new ScaleTransform(newScale, newScale);
            _canvasMyShapes.LayoutTransform = scale;

            _canvasMyShapes.Width = lastCanvasWidth;
            _canvasMyShapes.Height = lastCanvasHeight;
            _cbZoomValues.Text = _canvasMyShapes.LayoutTransform.Value.M11.ToString("P");

            if (Scene.BackgroundDataType == SymbolDataType.Vector 
                && _cadVisualizer != null)
                _scaleInfo.SetScale(_cadVisualizer.ActualZoom());
            else
                _scaleInfo.SetScale(_canvasMyShapes.LayoutTransform.Value.M11);

            double offsetX = (myScrollViewer.HorizontalOffset + zoomPoint.X)*zoom;
            double offsetY = (myScrollViewer.VerticalOffset + zoomPoint.Y)*zoom;
            myScrollViewer.ScrollToHorizontalOffset(offsetX - zoomPoint.X);
            myScrollViewer.ScrollToVerticalOffset(offsetY - zoomPoint.Y);

            if (_displayMode == SymbolSize.Fixed)
                SetSizeForFixedSizeMode();

            foreach (var shape in myShapes)
                shape.Select(EditMode);
        }

        private void SetScale(double scale)
        {
            if (_cadVisualizer == null && _backgroundBitmap == null)
                return;

            if (scale <= 0)
                return;

            var myShapes = GetSelectedShapes(_canvasMyShapes);

            if (myShapes == null)
                return;

            foreach (var shape in myShapes)
                shape.UnSelect();


            //zoom CAD model
            if (Scene.BackgroundDataType == SymbolDataType.Vector 
                && _cadVisualizer != null)
            {
                _offsetX = _offsetX*scale;
                _offsetY = _offsetY*scale;
                _cadVisualizer.SetOffset(_offsetX, _offsetY);
                _cadVisualizer.SetScale(scale);
            }

            //zoom canvas
            double lastCanvasWidth = _canvasMyShapes.ActualWidth;
            double lastCanvasHeight = _canvasMyShapes.ActualHeight;

            var newScale = new ScaleTransform(scale, scale);
            _canvasMyShapes.LayoutTransform = newScale;
            _canvasMyShapes.Width = lastCanvasWidth;
            _canvasMyShapes.Height = lastCanvasHeight;
            _cbZoomValues.Text = scale.ToString("P");

            if (Scene.BackgroundDataType == SymbolDataType.Vector 
                && _cadVisualizer != null)
                _scaleInfo.SetScale(_cadVisualizer.ActualZoom());
            else if (Scene.BackgroundDataType == SymbolDataType.Raster 
                && _backgroundImage != null 
                && _backgroundBitmap != null)
                _scaleInfo.SetScale(_canvasMyShapes.LayoutTransform.Value.M11);

            myScrollViewer.UpdateLayout();
            myScrollViewer.ScrollToHorizontalOffset(myScrollViewer.ScrollableWidth/2);
            myScrollViewer.ScrollToVerticalOffset(myScrollViewer.ScrollableHeight/2);

            if (_displayMode == SymbolSize.Fixed)
                SetSizeForFixedSizeMode();

            foreach (IGraphicsObject shape in myShapes)
                shape.Select(EditMode);
        }

        //Move scene
        private void MoveAll(double deltaX, double deltaY)
        {
            double offsetX = myScrollViewer.HorizontalOffset;
            double offsetY = myScrollViewer.VerticalOffset;
            myScrollViewer.ScrollToHorizontalOffset(offsetX - deltaX);
            myScrollViewer.ScrollToVerticalOffset(offsetY - deltaY);
        }

        private void UnSelectAllShapes()
        {
            GraphicsThumb.DeleteMode = false;

            if (UserInterfaceMode == InterfaceMode.PolygonAddNewPoint
                || UserInterfaceMode == InterfaceMode.PolygonRemovePoint)
            {
                UserInterfaceMode = InterfaceMode.Select;
            }

            for (int i = 0; i < _canvasMyShapes.Children.Count; i++)
            {
                var shape = _canvasMyShapes.Children[i] as IGraphicsObject;

                if (shape != null)
                    shape.UnSelect();
            }

            if (_userInterfaceModeValue == InterfaceMode.Select)
                _canvasMyShapes.ContextMenu.IsEnabled = true;
        }

        private void graphicsObjects_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2
                || UserInterfaceMode == InterfaceMode.Panning) //doubleclick
                return;

            if (sender is IGraphicsObject)
            {
                if (UserInterfaceMode == InterfaceMode.Paint)
                    return;

                if (e.ChangedButton == MouseButton.Left || e.ChangedButton == MouseButton.Right)
                {
                    if ((sender is ILiveObject) && (!(sender as ILiveObject).isEnable()))
                        return;

                    UnSelectAllShapes();
                    (sender as IGraphicsObject).Select(EditMode);

                    if (e.ChangedButton == MouseButton.Left && EditMode)
                        _selectedShape = (sender as IGraphicsObject);

                    _canvasMyShapes.ContextMenu.IsEnabled = false;
                }
            }
        }

        private void _bSelectMode_OnClick(object sender, RoutedEventArgs e)
        {
            UserInterfaceMode = InterfaceMode.Select;
            //_canvasMyShapes.Cursor = Cursors.Arrow;
            //GraphicsObjects.Singleton.UnlockCursorChanged(_canvasMyShapes);
        }

        private void _bUndo_OnClick(object sender, RoutedEventArgs e)
        {
            if (_canvasMyShapes.Children.Count > 0)
            {
                _canvasMyShapes.Children.RemoveAt(_canvasMyShapes.Children.Count - 1);
            }
        }

        private void _bDelete_OnClick(object sender, RoutedEventArgs e)
        {
            DeleteSelectedShapes();
        }

        private void myScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (Scene == null 
                || Scene.BackgroundDataType != SymbolDataType.Vector
                || _cadVisualizer == null)
                return;

            double offsetX = myScrollViewer.ContentHorizontalOffset;
            double offsetY = myScrollViewer.ContentVerticalOffset;
            _cadVisualizer.SetOffset(-offsetX, -offsetY);
            _cadVisualizer.Draw();
        }

        private double GetOptimalZoom()
        {
            if (_backgroundImage == null || _backgroundImage.Source == null)
                return -1;

            double defaultZoom;

            if (_backgroundImage.Source.Width > _backgroundImage.Source.Height)
            {
                defaultZoom = (myGrid.ColumnDefinitions[2].ActualWidth - 20)/_backgroundImage.Source.Width;

                if (_backgroundImage.Source.Height*defaultZoom >
                    (myGrid.RowDefinitions[1].ActualHeight + myGrid.RowDefinitions[2].ActualHeight) - 20)
                    defaultZoom = ((myGrid.RowDefinitions[1].ActualHeight + myGrid.RowDefinitions[2].ActualHeight) - 20)/
                                  _backgroundImage.Source.Height;
            }
            else
            {
                defaultZoom = ((myGrid.RowDefinitions[1].ActualHeight + myGrid.RowDefinitions[2].ActualHeight) - 20)/
                              _backgroundImage.Source.Height;

                if (_backgroundImage.Source.Width*defaultZoom > (myGrid.ColumnDefinitions[2].ActualWidth - 20))
                    defaultZoom = (myGrid.ColumnDefinitions[2].ActualWidth - 20)/_backgroundImage.Source.Width;
            }

            return defaultZoom;
        }

        private bool _selectSybolSizeModeEnable;
        private SceneSettings _openedSceneSettingsWindow;

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            FillListOfCategories();

            Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, 
                new Action(
                    () => LocalizationHelper.TranslateWpfControl("NCASSceneEditForm", this)));
        }

        private void LoadScene()
        {
            _selectSybolSizeModeEnable = false;

            if (_shapeSettingsPanel != null)
                _shapeSettingsPanel.Visibility = Visibility.Collapsed;
            
            if (_textSettingsPanel != null)
                _textSettingsPanel.Visibility = Visibility.Collapsed;
            
            if (_openedSceneSettingsWindow != null)
                _openedSceneSettingsWindow.Visibility = Visibility.Collapsed;
            
            _canvasBackground.Children.Clear();
            _canvasMyShapes.Children.Clear();
            _canvasUI.Children.Clear();

            if (CanvasSerialization.Layers != null)
                CanvasSerialization.Layers.Clear();

            LoadTemplates();

            if (string.IsNullOrEmpty(Scene.Name))
            {
                //set default settings
                CanvasSerialization.CanvasSettings.UseTemplateId = _templates[0].Id;
                LoadSymbols();
                ShowSceneSettings();
            }

            SetVisibilityForBackgroundInformationPanel(false);
            _canvasMyShapes.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0, 0, 0, 0));
            LoadModel();

            //insert all symbol types into selectbox
            _cbSizeMode.Items.Clear();

            foreach (SymbolSize symbolSize in Enum.GetValues(typeof (SymbolSize)))
            {
                var item = new SymbolSizeMode(symbolSize);
                _cbSizeMode.Items.Add(item);

                if (symbolSize == CanvasSerialization.CanvasSettings.DefaultSymbolSize)
                    _cbSizeMode.SelectedItem = item;
                else
                    _cbSizeMode.SelectedIndex = 0;
            }

            _selectSybolSizeModeEnable = true;
            SetMainMenuVisibility();
        }

        private void SetMainMenuVisibility()
        {
            if (!EditMode)
            {
                _bLoadDfxFile.Visibility = Visibility.Collapsed;
                _bSceneSettings.Visibility = Visibility.Collapsed;
                _bSelectMode.Visibility = Visibility.Collapsed;
                _bUndo.Visibility = Visibility.Collapsed;
                _bDelete.Visibility = Visibility.Collapsed;
                _bPaintRectangle.Visibility = Visibility.Collapsed;
                _bPaintLine.Visibility = Visibility.Collapsed;
                _bPaintPolyline.Visibility = Visibility.Collapsed;
                _bPaintEllipse.Visibility = Visibility.Collapsed;
                _bPaintAlarmArea.Visibility = Visibility.Collapsed;
                _bPaintSymbol.Visibility = Visibility.Collapsed;
                _panelSizeMode.Visibility = Visibility.Visible;
                _panelSizeOfSymbol.Visibility = Visibility.Visible;
                _separator1.Visibility = Visibility.Collapsed;
                _separator2.Visibility = Visibility.Collapsed;
                _bRemove.Visibility = Visibility.Collapsed;
                _lAddLayer.Visibility = Visibility.Collapsed;
                _tbLayerName.Visibility = Visibility.Collapsed;
                _bAddLayer.Visibility = Visibility.Collapsed;
                _bSaveScene.Visibility = Visibility.Collapsed;

                if (_allowedEdit)
                {
                    _bSwitchToEditMode.Visibility = Visibility.Visible;
                    _bSwitchToViewMode.Visibility = Visibility.Collapsed;
                }
                else
                {
                    _bSwitchToEditMode.Visibility = Visibility.Collapsed;
                    _bSwitchToViewMode.Visibility = Visibility.Collapsed;
                }

                _lbLayers.Margin = new Thickness(5, 5, 1, 35);
                _chbShowAll.Margin = new Thickness(0, 0, 6, 10);
                _canvasMyShapes.ContextMenu.IsEnabled = false;
                _bShowScenes.Visibility = Visibility.Visible;
                _bInsertText.Visibility = Visibility.Collapsed;
                _bGoHome.Visibility = Visibility.Visible;
                _bGoHome.Opacity = 0.5;
                _bGoHome.IsEnabled = false;
                _bGoBack.Visibility = Visibility.Visible;
                _bGoBack.Opacity = 0.5;
                _bGoBack.IsEnabled = false;
                _bPanningMode.Visibility = Visibility.Collapsed;
#if DEBUG
                _bInsertSymbol.Visibility = Visibility.Collapsed;
#endif       
            }
            else
            {
                _bLoadDfxFile.Visibility = Visibility.Visible;
                _bSceneSettings.Visibility = Visibility.Visible;
                _bSelectMode.Visibility = Visibility.Visible;
                _bUndo.Visibility = Visibility.Visible;
                _bDelete.Visibility = Visibility.Visible;
                _bPaintRectangle.Visibility = Visibility.Visible;
                _bPaintEllipse.Visibility = Visibility.Visible;
                _bPaintLine.Visibility = Visibility.Visible;
                _bPaintPolyline.Visibility = Visibility.Visible;
                _bPaintAlarmArea.Visibility = Visibility.Visible;
                _bPaintSymbol.Visibility = Visibility.Visible;
                _panelSizeMode.Visibility = Visibility.Collapsed;
                _panelSizeOfSymbol.Visibility = Visibility.Collapsed;
                _separator1.Visibility = Visibility.Visible;
                _separator2.Visibility = Visibility.Visible;
                _bRemove.Visibility = Visibility.Visible;
                _lAddLayer.Visibility = Visibility.Visible;
                _tbLayerName.Visibility = Visibility.Visible;
                _bAddLayer.Visibility = Visibility.Visible;
                _bSaveScene.Visibility = Visibility.Visible;

                if (_allowedEdit)
                {
                    _bSwitchToEditMode.Visibility = Visibility.Collapsed;
                    _bSwitchToViewMode.Visibility = Visibility.Visible;
                }
                else
                {
                    _bSwitchToEditMode.Visibility = Visibility.Collapsed;
                    _bSwitchToViewMode.Visibility = Visibility.Collapsed;
                }

                _lbLayers.Margin = new Thickness(5, 5, 1, 55);
                _chbShowAll.Margin = new Thickness(0, 0, 6, 35);
                _canvasMyShapes.ContextMenu.IsEnabled = true;
                _bShowScenes.Visibility = Visibility.Collapsed;
                _bInsertText.Visibility = Visibility.Visible;
                _bGoHome.Visibility = Visibility.Collapsed;
                _bGoBack.Visibility = Visibility.Collapsed;
                _bPanningMode.Visibility = Visibility.Visible;
#if DEBUG
                _bInsertSymbol.Visibility = Visibility.Visible;
#endif
            }
        }

        private List<T> GetLiveObjectsById<T>(Guid id, Canvas canvas) where T : UIElement
        {
            if (id == Guid.Empty || canvas == null)
                return null;

            var myShape = new List<T>();

            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
            {
                foreach (var element in canvas.Children)
                {
                    var liveObject = (element as T) as ILiveObject;

                    if (liveObject == null)
                        continue;

                    if (liveObject.GetObjectGuid() == id)
                    {
                        myShape.Add((T)liveObject);
                    }
                }
            }));

            return myShape;
        }

        private IEnumerable<ILiveObject> GetLiveObjects(Canvas canvas)
        {
            if (canvas == null)
                return null;

            var liveObjects = new List<ILiveObject>();

            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
            {
                foreach (var element in canvas.Children)
                {
                    var liveObject = element as ILiveObject;

                    if (liveObject == null)
                        continue;

                    if (liveObject.GetObjectGuid() != Guid.Empty)
                    {
                        liveObjects.Add(liveObject);
                    }
                }
            }));

            return liveObjects;
        }

        private IEnumerable<ILiveObject> GetLiveObjectsById(Guid id, Canvas canvas)
        {
            if (canvas == null)
                return null;

            var liveObjects = new List<ILiveObject>();

            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
            {
                foreach (var element in canvas.Children)
                {
                    var liveObject = element as ILiveObject;

                    if (liveObject == null)
                        continue;

                    if (liveObject.GetObjectGuid().Equals(id))
                    {
                        liveObjects.Add(liveObject);
                    }
                }
            }));

            return liveObjects;
        }

        private bool _insertBackgroundMode;

        private void SetInsertBackgroundModeVisibility()
        {
            _insertBackgroundMode = true;
            _lAddLayer.Visibility = Visibility.Hidden;
            _bAddLayer.Visibility = Visibility.Hidden;
            _bRemove.Visibility = Visibility.Hidden;
            _tbLayerName.Visibility = Visibility.Hidden;
        }

        private void UnsetInsertBackgroundModeVisibility()
        {
            _insertBackgroundMode = false;
            _lAddLayer.Visibility = Visibility.Visible;
            _bAddLayer.Visibility = Visibility.Visible;
            _bRemove.Visibility = Visibility.Visible;
            _tbLayerName.Visibility = Visibility.Visible;
        }

        private readonly  Dictionary<string, string> _filterKeysOfSymbols = new Dictionary<string, string>();
        private List<GraphicSymbolTemplate> _templates;

        private void LoadTemplates()
        {
            try
            {
                if (MainServerProvider == null)
                    return;

                Exception ex;

                _templates = MainServerProvider.GraphicSymbolTemplates.List(out ex).ToList();

                if (ex != null)
                    throw ex;
            }
            catch (Exception)
            {
                CgpClient.Singleton.IsConnectionLost(true);
            }
        }

        public static ImageSource NoSource;

        //Load symbols from SVG, BMP, PNG, JPEG files
        private void LoadSymbols()
        {
            try
            {
                NoSource = GetImageSourceFromSvg(
                    new MemoryStream(
                        global::Cgp.NCAS.WpfGraphicsControl.Properties.Resources.NoSource));

                if (MainServerProvider == null)
                    return;

                _symbolCache.Clear();
                _filterKeysOfSymbols.Clear();
                Exception ex;
                Guid? templateId;

                if (CanvasSerialization.CanvasSettings.UseTemplateId == null)
                    templateId = _templates[0].Id;
                else
                    templateId = CanvasSerialization.CanvasSettings.UseTemplateId;

                if (MainServerProvider.GraphicSymbolTemplates.GetObjectById(templateId) == null)
                {
                    templateId = _templates[0].Id;
                }

                IList<FilterSettings> fs = new List<FilterSettings>();
                fs.Add(new FilterSettings(GraphicSymbol.COLUMN_IDTEMPLATE, templateId, ComparerModes.EQUALL));

                ICollection<GraphicSymbol> graphicSymbols = MainServerProvider.GraphicSymbols.SelectByCriteria(fs,
                    out ex);

                if (ex != null)
                    throw ex;

                if (graphicSymbols.Count == 0)
                    return;

                foreach (var gs in graphicSymbols)
                {
                    var rawData = MainServerProvider.GraphicSymbolRawDatas.GetObjectById(gs.IdRawData);

                    if (rawData == null)
                        continue;

                    byte[] RawData = rawData.RawData;

                    using (var stream = new MemoryStream(RawData))
                    {
                        switch (rawData.DataType)
                        {
                            case SymbolDataType.Vector:

                                if (!_symbolCache.ContainsKey(gs.SymbolType + gs.SymbolState.ToString().ToLower()))
                                {
                                    _symbolCache.Add(gs.SymbolType + gs.SymbolState.ToString().ToLower(),
                                        new SymbolParemeter(gs.SymbolType, gs.SymbolState, GetImageSourceFromSvg(stream)));
                                }

                                break;

                            case SymbolDataType.Raster:

                                if (!_symbolCache.ContainsKey(gs.SymbolType + gs.SymbolState.ToString().ToLower()))
                                {
                                    _symbolCache.Add(gs.SymbolType + gs.SymbolState.ToString().ToLower(),
                                        new SymbolParemeter(gs.SymbolType, gs.SymbolState,
                                            GetImageSourceFromBitmap(stream)));
                                }

                                break;
                        }

                        if (!_filterKeysOfSymbols.ContainsKey(gs.SymbolType + gs.SymbolState.ToString().ToLower()))
                        {
                            _filterKeysOfSymbols.Add(gs.SymbolType + gs.SymbolState.ToString().ToLower(),
                                gs.FilterKey);
                        }
                    }
                }
            }
            catch (Exception)
            {
                CgpClient.Singleton.IsConnectionLost(true);
            }
        }

        private DrawingImage GetImageSourceFromSvg(MemoryStream stream)
        {
            var svgRender = new SVGRender();
            var dg = svgRender.LoadDrawing(stream);
            return new DrawingImage(dg);
        }

        private BitmapImage GetImageSourceFromBitmap(MemoryStream stream)
        {
            var bmpSymbol = new BitmapImage();
            bmpSymbol.BeginInit();
            bmpSymbol.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
            bmpSymbol.CacheOption = BitmapCacheOption.OnLoad;
            bmpSymbol.StreamSource = stream;
            bmpSymbol.EndInit();

            return bmpSymbol;
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResizeGraphicViewer();
        }

        private double _defaultScale;

        private void ResizeGraphicViewer()
        {
            int newWbmWidth = Convert.ToInt32(myGrid.ColumnDefinitions[2].ActualWidth);
            int newWbmHeight =
                Convert.ToInt32(myGrid.RowDefinitions[1].ActualHeight + myGrid.RowDefinitions[2].ActualHeight);

            if (newWbmWidth < 100)
                newWbmWidth = 100;

            if (newWbmHeight < 100)
                newWbmHeight = 100;

            if (Scene == null)
                return;

            if (Scene.BackgroundDataType == SymbolDataType.Vector 
                && _cadVisualizer != null)
            {
                _writableBitmap = BitmapFactory.New(newWbmWidth, newWbmHeight);
                _cadVisualizer.WriteableBitmap = _writableBitmap;
                _cadVisualizer.Draw();
                _cadVisualizer.SetOptimalZoom();

                if (_isModelLoaded)
                    _canvasMyShapes.LayoutTransform = new ScaleTransform(_cadVisualizer.ActualZoom(),
                        _cadVisualizer.ActualZoom());

                _backgroundImage.Source = _writableBitmap;
                _cadVisualizer.Draw();
            }
            else if (Scene.BackgroundDataType == SymbolDataType.Raster 
                && _backgroundBitmap != null 
                && _canvasMyShapes.LayoutTransform.Value.M11 < GetOptimalZoom())
            {
                _canvasMyShapes.LayoutTransform = new ScaleTransform(GetOptimalZoom(), GetOptimalZoom());
            }

            _defaultScale = GetDefaultScale();

            if (_cadVisualizer != null)
                _scaleInfo.SetScale(_cadVisualizer.ActualZoom());
            else
                _scaleInfo.SetScale(_canvasMyShapes.LayoutTransform.Value.M11);
        }

        private void _canvasMyShapes_MouseLeave(object sender, MouseEventArgs e)
        {
            (sender as Canvas).Cursor = Cursors.Arrow;
        }

        private void myScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            Point position = e.GetPosition(myScrollViewer);

            if (e.Delta > 0)
                Zoom(110, position);
            else
                Zoom(90, position);

            e.Handled = true;
        }

        private readonly HashSet<ObjectType> _supportedObjectTypes = new HashSet<ObjectType>(new ObjectType[]
        {
            ObjectType.CardReader,
            ObjectType.AlarmArea,
            ObjectType.Input,
            ObjectType.Output,
            ObjectType.DoorEnvironment
        });

        private void _canvasMyShapes_Drop(object sender, DragEventArgs e)
        {
            if (!EditMode)
                return;

            try
            {
                var output = e.Data.GetFormats();

                if (output == null)
                    return;

                var armObject = e.Data.GetData(output[0]) as AOrmObject;

                if (armObject == null
                    || !_supportedObjectTypes.Contains(armObject.GetObjectType()))
                    return;

                var shape = InsertSymbolDialog(armObject);

                if (shape == null)
                    return;

                var graphicsObject = shape as IGraphicsObject;

                if (graphicsObject == null)
                    return;

                graphicsObject.SetLayerID(_selectedLayer.Id);
                Canvas.SetLeft(shape, e.GetPosition(_canvasMyShapes).X);
                Canvas.SetTop(shape, e.GetPosition(_canvasMyShapes).Y);
                double width = _canvasMyShapes.Width/CanvasSerialization.CanvasSettings.ImplicityScaleOfInsertedSymbols;
                graphicsObject.SetWidth(width);

                var alarmAreaPolygon = shape as GraphicsPolygon;

                if (alarmAreaPolygon != null)
                {
                    alarmAreaPolygon.Points.Add(new Point(0, 0));
                    alarmAreaPolygon.Points.Add(new Point(width, 0));
                    alarmAreaPolygon.Points.Add(new Point(width, width));
                    alarmAreaPolygon.Points.Add(new Point(0, width));
                    alarmAreaPolygon.IsClosed = true;
                }

                _canvasMyShapes.Children.Add(shape);
                shape.MouseDown += graphicsObjects_MouseDown;
                shape.MouseUp += GraphicsControl_MouseUp;
                shape.MouseEnter += GraphicsControl_MouseEnter;
                shape.MouseLeave += GraphicsControl_MouseLeave;
                //add context menu
                AddContextMenu(shape as FrameworkElement);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private UIElement InsertSymbolDialog(AOrmObject existingObject)
        {
            UIElement shape = null;

            var windowSelectDialog = new SelectSymbolDialog(_symbolCache, _filterKeysOfSymbols);
            windowSelectDialog.ExistingObject = existingObject;

            if (windowSelectDialog.ShowDialog() == true)
            {
                switch (windowSelectDialog.SelectedObjectType)
                {
                    case ObjectType.CardReader:
                        shape = new GraphicsCardReader(_canvasMyShapes, _symbolCache);
                        break;

                    case ObjectType.Input:
                        shape = new GraphicsIO(_canvasMyShapes, _symbolCache, windowSelectDialog.SelectedSymbol);
                        (shape as GraphicsIO).IOType = IOType.Input;
                        break;

                    case ObjectType.Output:
                        shape = new GraphicsIO(_canvasMyShapes, _symbolCache, windowSelectDialog.SelectedSymbol);
                        (shape as GraphicsIO).IOType = IOType.Output;
                        break;

                    case ObjectType.DoorEnvironment:
                        shape = windowSelectDialog.SelectedSymbol == SymbolType.DoorEnviromentLeft 
                            ? new GraphicsDoorEnvironment(_canvasMyShapes, _symbolCache, true) 
                            : new GraphicsDoorEnvironment(_canvasMyShapes, _symbolCache, false);
                        break;

                    case ObjectType.AlarmArea:
                        shape = new GraphicsPolygon(_canvasMyShapes, PolygonMode.AlarmArea);
                        break;
                }

                if (windowSelectDialog.ExistingObject != null)
                {
                    if (windowSelectDialog.SelectedObjectGuid != null)
                        (shape as ILiveObject).SetObjectGuid(windowSelectDialog.SelectedObjectGuid.Value);

                    (shape as ILiveObject).SetCategories(windowSelectDialog.Categories);
                }
            }

            return shape;
        }

        private void _bAddLayer_Click(object sender, RoutedEventArgs e)
        {
            if (CanvasSerialization.Layers == null || string.IsNullOrEmpty(_tbLayerName.Text))
                return;

            var newLayer = new Layer();
            newLayer.Name = _tbLayerName.Text;

            bool exist = false;
            foreach (var layer in CanvasSerialization.Layers.Values)
            {
                if (layer.Name == newLayer.Name)
                {
                    exist = true;
                    break;
                }
            }

            if (!exist)
            {
                CanvasSerialization.Layers.Add(newLayer.Id, newLayer);
                LoadCanvasLayers();
                _needSave = true;
            }
            else
                IwQuick.UI.Dialog.Error(LocalizationHelper.GetString("ErrorLayerAlreadyExists"));
        }

        private void SetSizeForVariableSizeMode()
        {
            var myShapes = GetSelectedShapes(_canvasMyShapes);

            foreach (var myShape in myShapes)
                myShape.UnSelect();

            foreach (var element in _canvasMyShapes.Children)
            {
                var symbol = element as GraphicsSymbol;

                if (symbol != null)
                {
                    var defaultParameters = symbol.GetDefaultParameters();
                    symbol.SetLeft(defaultParameters.Left);
                    symbol.SetTop(defaultParameters.Top);
                    symbol.SetWidth(defaultParameters.Width);

                    //change rotate transform parameters to default values
                    var rotateTransform = symbol.RenderTransform as RotateTransform;
                    if (rotateTransform != null)
                    {
                        rotateTransform.CenterX = Math.Abs(defaultParameters.Width/2);
                        rotateTransform.CenterY = Math.Abs(defaultParameters.Height/2);
                    }

                    var label = (symbol as ILiveObject).GetLabel();

                    if (label != null)
                    {
                        label.SetLeft(label.GetDefaultParameters().Left);
                        label.SetTop(label.GetDefaultParameters().Top);
                        label.FontSize = label.GetDefaultParameters().FontSize;
                        label.UpdateLayout();

                        var labelRotateTransform = label.RenderTransform as RotateTransform;

                        if (labelRotateTransform != null)
                        {
                            labelRotateTransform.CenterX = label.GetWidth()/2;
                            labelRotateTransform.CenterY = label.GetHeight()/2;
                        }
                    }
                }
            }

            foreach (IGraphicsObject myShape in myShapes)
                myShape.Select(EditMode);
        }

        private void SetSizeForFixedSizeMode()
        {
            if (EditMode)
                return;

            try
            {
                List<IGraphicsObject> myShapes = GetSelectedShapes(_canvasMyShapes);
                double scale = _canvasMyShapes.LayoutTransform.Value.M11;

                foreach (IGraphicsObject myShape in myShapes)
                    myShape.UnSelect();

                foreach (UIElement element in _canvasMyShapes.Children)
                {
                    var symbol = element as GraphicsSymbol;

                    if (symbol != null)
                    {
                        //set fixed size for shapes
                        Dispatcher.Invoke(
                            DispatcherPriority.Background,
                            new Action(() =>
                            {
                                //get center position of symbol
                                double centerX = symbol.GetLeft() + symbol.GetWidth()/2;
                                double centerY = symbol.GetTop() + symbol.GetHeight()/2;

                                //set new size
                                symbol.SetWidth(_displayWidth/scale);

                                //set new position by center position
                                Canvas.SetLeft(element, centerX - symbol.GetWidth()/2);
                                Canvas.SetTop(element, centerY - symbol.GetHeight()/2);

                                //change rotate transform parameters
                                var rotateTransform = element.RenderTransform as RotateTransform;
                                if (rotateTransform != null)
                                {
                                    rotateTransform.CenterX = Math.Abs(centerX - symbol.GetLeft());
                                    rotateTransform.CenterY = Math.Abs(centerY - symbol.GetTop());
                                }

                                //set size and position for label of symbol
                                var label = (symbol as ILiveObject).GetLabel();

                                if (label != null)
                                {
                                    double x = centerX - (centerX - label.GetDefaultParameters().Left)/scale;
                                    double y = centerY - (centerY - label.GetDefaultParameters().Top)/scale;
                                    label.FontSize = _displayWidth/scale/5;
                                    label.UpdateLayout();
                                    Canvas.SetLeft(label, x);
                                    Canvas.SetTop(label, y);

                                    var labelRotateTransform = label.RenderTransform as RotateTransform;

                                    if (labelRotateTransform != null)
                                    {
                                        labelRotateTransform.CenterX = Math.Abs(label.GetWidth()/2);
                                        labelRotateTransform.CenterY = Math.Abs(label.GetHeight()/2);
                                    }
                                }
                            }));
                    }
                }

                foreach (var myShape in myShapes)
                    myShape.Select(EditMode);
            }
            catch
            {
            }
        }

        private List<IGraphicsObject> GetSelectedShapes(Canvas canvas)
        {
            if (canvas == null)
                return null;

            var myShapes = new List<IGraphicsObject>();

            foreach (UIElement element in canvas.Children)
            {
                var shape = element as IGraphicsObject;

                if (shape != null)
                {
                    if (shape.isSelected())
                        myShapes.Add(shape);
                }
            }

            return myShapes;
        }

        private void myScrollViewer_KeyUp(object sender, KeyEventArgs e)
        {
            if (!EditMode)
                return;

            switch (e.Key)
            {
                case Key.Delete:
                    DeleteSelectedShapes();
                    break;
            }
        }

        private void DeleteSelectedShapes()
        {
            _needSave = true;
            var mySelectedShapes = GetSelectedShapes(_canvasMyShapes);

            if (mySelectedShapes == null)
                return;

            foreach (var shape in mySelectedShapes)
            {
                shape.UnSelect();
                var liveObject = shape as ILiveObject;

                if (liveObject != null && liveObject.GetLabel() != null)
                {
                    liveObject.GetLabel().LiveObject = null;

                    if (_textSettingsPanel != null
                        && _textSettingsPanel.GetEditText() == liveObject.GetLabel())
                    {
                        _textSettingsPanel_Closed(_textSettingsPanel);
                    }

                    _canvasMyShapes.Children.Remove(liveObject.GetLabel());
                    liveObject.SetLabel(null);

                    if (_shapeSettingsPanel != null
                        && _shapeSettingsPanel.GetEditElement() == shape as UIElement)
                    {
                        ShapeSettingsWindow_Closed(_shapeSettingsPanel);
                    }
                }
                else
                {
                    var text = shape as Text;

                    if (text != null)
                    {
                        if (_textSettingsPanel != null &&
                            _textSettingsPanel.GetEditText() == text)
                        {
                            _textSettingsPanel_Closed(_textSettingsPanel);
                        }

                        if (text.LiveObject != null)
                        {
                            text.LiveObject.SetLabel(null);
                            text.LiveObject = null;
                        }
                    }
                    else
                    {
                        if (_shapeSettingsPanel != null
                            && _shapeSettingsPanel.GetEditElement() == shape)
                        {
                            ShapeSettingsWindow_Closed(_shapeSettingsPanel);
                        }
                    }
                }

                _canvasMyShapes.Children.Remove(shape as UIElement);
            }
        }

        private void _lbLayers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedLayer = _lbLayers.SelectedItem as Layer;
            _bRemove.IsEnabled = (_selectedLayer == null || _selectedLayer.Name == "default") ? false : true;
        }

        private void _bInsertBackground_Click(object sender, RoutedEventArgs e)
        {
            if (_bSelectScaleRatio.Visibility == Visibility.Hidden)
            {
                try
                {
                    double length = double.Parse(_tbSpecifiedScaleLength.Text);
                    CanvasSerialization.CanvasSettings.ModelLength = _backgroundBitmap.Width/_scaleRatioSelector.Width*length;
                }
                catch (Exception)
                {
                    IwQuick.UI.Dialog.Error(LocalizationHelper.GetString("ErrorLengthFailed"));
                    return;
                }
            }

            UnsetInsertBackgroundModeVisibility();
            LoadCanvasLayers();
            SetVisibilityForBackgroundInformationPanel(false);

            //adding model scale info
            if ( _backgroundBitmap != null &&
                _bSelectScaleRatio.Visibility == Visibility.Hidden)
            {
                _scaleInfo.SetScaleRatio(CanvasSerialization.CanvasSettings.ModelLength * 1000 / _backgroundBitmap.Width);
                _scaleInfo.SetScale(_canvasMyShapes.LayoutTransform.Value.M11);

                if (!_canvasUI.Children.Contains(_scaleInfo))
                    _canvasUI.Children.Add(_scaleInfo);
            }

            HideScaleRatioSelector();
            EnablePainting(true);
            _needSave = true;
        }

        private void scaleRatioSelector_ChangeSize(double newSize)
        {
            _lCountOfPixels.Content = string.Format("for {0} pixels", newSize.ToString("F0"));
        }

        private void _bRemove_Click(object sender, RoutedEventArgs e)
        {
            var layer = _lbLayers.SelectedItem as Layer;

            if (layer == null)
                return;

            var myShapes = GetShapesByLayerID(layer.Id, _canvasMyShapes);

            foreach (var myShape in myShapes)
            {
                myShape.UnSelect();
                _canvasMyShapes.Children.Remove(myShape as UIElement);
            }

            CanvasSerialization.Layers.Remove((_lbLayers.SelectedItem as Layer).Id);
            _lbLayers.Items.Remove(_lbLayers.SelectedItem as Layer);

            _needSave = true;
        }

        private List<IGraphicsObject> GetShapesByLayerID(Guid LayerID, Canvas canvas)
        {
            if (LayerID == Guid.Empty && canvas == null)
                return null;

            var myShapes = new List<IGraphicsObject>();

            foreach (UIElement shape in canvas.Children)
            {
                if (shape is IGraphicsObject && (shape as IGraphicsObject).GetLayerID() == LayerID)
                    myShapes.Add(shape as IGraphicsObject);
            }

            return myShapes;
        }

        private void MenuItemRectangle_OnClick(object sender, RoutedEventArgs e)
        {
            UserInterfaceMode = InterfaceMode.Paint;
            _paintShape = PaintMode.Rectangle;
        }

        private void MenuItemEllipse_OnClick(object sender, RoutedEventArgs e)
        {
            UserInterfaceMode = InterfaceMode.Paint;
            _paintShape = PaintMode.Ellipse;
        }

        private void MenuItemPolygon_OnClick(object sender, RoutedEventArgs e)
        {
            UserInterfaceMode = InterfaceMode.Paint;
            _paintShape = PaintMode.AlarmAreaPolygon;
        }

        private void MenuItemCardReader_OnClick(object sender, RoutedEventArgs e)
        {
            UserInterfaceMode = InterfaceMode.Paint;
            _paintShape = PaintMode.Image;
            _paintSymbolType = SymbolType.CardReader;
        }

        private void MenuItemImportBackground_OnClick(object sender, RoutedEventArgs e)
        {
            ImportBackgroundMenuClick();
        }

        private void MenuItemSceneSettings_OnClick(object sender, RoutedEventArgs e)
        {
            ShowSceneSettings();
        }

        private void ShowSceneSettings()
        {
            if (_openedSceneSettingsWindow == null)
            {
                _openedSceneSettingsWindow = new SceneSettings(_templates, CanvasSerialization);
                _openedSceneSettingsWindow.SceneName = Scene.Name;
                _openedSceneSettingsWindow.SceneDescription = Scene.Description;
                _openedSceneSettingsWindow.SceneSettingsCloseClick += sceneSettingsWindow_SceneSettingsCloseClick;
                _openedSceneSettingsWindow.SceneSettingsOkClick += sceneSettingsWindow_SceneSettingsOkClick;

                _openedSceneSettingsWindow.HorizontalAlignment = HorizontalAlignment.Stretch;
                _openedSceneSettingsWindow.VerticalAlignment = VerticalAlignment.Stretch;
                myGrid.Children.Add(_openedSceneSettingsWindow);
                Grid.SetColumn(_openedSceneSettingsWindow, 2);
                Grid.SetRow(_openedSceneSettingsWindow, 1);
                Grid.SetRowSpan(_openedSceneSettingsWindow, 2);
                Grid.SetZIndex(_openedSceneSettingsWindow, 100);
            }

            if (_backgroundBitmap != null) //is raster
                _openedSceneSettingsWindow.InsertScaleSpecified = true;
            else
                _openedSceneSettingsWindow.InsertScaleSpecified = false;

            _openedSceneSettingsWindow.Show();
        }

        private void sceneSettingsWindow_SceneSettingsCloseClick(SceneSettings sender)
        {
        }

        private void sceneSettingsWindow_SceneSettingsOkClick(SceneSettings sender)
        {
            _needSave = true;
            if (_backgroundBitmap != null)
                sender.InsertScaleSpecified = true;

            Scene.Name = sender.SceneName;
            Scene.Description = sender.SceneDescription;
            SetSymbolSizeMode();
            LoadSymbols();

            if (_backgroundBitmap != null
                && _scene.BackgroundDataType == SymbolDataType.Raster)
                _scaleInfo.SetScaleRatio(CanvasSerialization.CanvasSettings.ModelLength*1000/_backgroundBitmap.Width);

            sender.Visibility = Visibility.Collapsed;

            if (_backgroundBitmap == null && _cadVisualizer == null)
            {
                //_firstLoad = false;
                _bLoadDfxFile_OnClick(null, null);
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
        }

        public void ChangeAlarmAreaState(Guid AlarmAreaGuid, ActivationState state)
        {
            if (EditMode)
                return;

            var polygons = GetLiveObjectsById<GraphicsPolygon>(AlarmAreaGuid, _canvasMyShapes)
                .Cast<ILiveObject>();

            ChangeStates(polygons, (byte) state);
        }

        public void ChangeAlarmAreaAlarmState(Guid AlarmAreaGuid, AlarmAreaAlarmState state)
        {
            if (EditMode)
                return;

            var alarmAreas = GetLiveObjectsById<GraphicsPolygon>(AlarmAreaGuid,
                _canvasMyShapes);

            if (alarmAreas == null || !alarmAreas.Any())
                return;

            foreach (GraphicsPolygon alarmArea in alarmAreas)
            {
                alarmArea.ChangeAlarmState(state);
            }
        }

        public void ChangeOutputState(Guid outputGuid, State state)
        {
            if (EditMode)
                return;

            var outputs = GetLiveObjectsById<GraphicsIO>(outputGuid, _canvasMyShapes)
                .Cast<ILiveObject>();

            ChangeStates(outputs, (byte) state);
        }

        public void ChangeCcuState(Guid ccuId, CCUOnlineState state)
        {
            if (EditMode)
                return;

            var ccus = GetLiveObjectsById<GraphicsCcu>(ccuId, _canvasMyShapes)
                .Cast<ILiveObject>();

            ChangeStates(ccus, (byte)state);
        }

        public void ChangeDcuState(Guid ccuId, OnlineState state)
        {
            if (EditMode)
                return;

            var ccus = GetLiveObjectsById<GraphicsDcu>(ccuId, _canvasMyShapes)
                .Cast<ILiveObject>();

            ChangeStates(ccus, (byte)state);
        }

        public void ChangeDoorEnvironmentState(Guid doorEnvironmentGuid, DoorEnvironmentState state)
        {
            if (EditMode)
                return;

            var doorEnvironments = GetLiveObjectsById<GraphicsDoorEnvironment>(doorEnvironmentGuid,
                _canvasMyShapes).Cast<ILiveObject>();

            ChangeStates(doorEnvironments, (byte) state);
        }

        public void ChangeCardReaderState(Guid cardReaderGuid, State state)
        {
            if (EditMode)
                return;

            var cardReaders = GetLiveObjectsById<GraphicsCardReader>(cardReaderGuid,
                _canvasMyShapes).Cast<ILiveObject>();

            ChangeStates(cardReaders, (byte) state);
        }

        private void ChangeStates(IEnumerable<ILiveObject> objects, byte state)
        {
            if (objects == null)
                return;

            foreach (var liveObject in objects)
                liveObject.ChangeState(state);
        }

        public void ChangeCardReaderOnlineState(Guid cardReaderGuid, OnlineState state)
        {
            if (EditMode)
                return;

            var cardReaders = GetLiveObjectsById<GraphicsCardReader>(cardReaderGuid,
                _canvasMyShapes);

            foreach (var cardReader in cardReaders)
                cardReader.SetOnlineState(state);
        }

        public void ChangeAlarmState(ServerAlarmCore serverAlarm)
        {
            if (EditMode)
                return;

            var state = (serverAlarm.IsAcknowledged
                           || serverAlarm.IsBlocked
                           || serverAlarm.Alarm.AlarmState == AlarmState.Normal)
                ? State.Normal
                : State.Alarm;

            switch (serverAlarm.Alarm.AlarmKey.AlarmType)
            {
                case AlarmType.CCU_TamperSabotage:

                    foreach (
                        var ccu in
                            GetLiveObjectsById<GraphicsCcu>((Guid) serverAlarm.Alarm.AlarmKey.AlarmObject.Id,
                                _canvasMyShapes))
                    {
                        ccu.ChangeAlarmState((byte) state);
                    }

                    break;

                case AlarmType.DCU_TamperSabotage:

                    foreach (
                        var dcu in
                            GetLiveObjectsById<GraphicsDcu>((Guid) serverAlarm.Alarm.AlarmKey.AlarmObject.Id,
                                _canvasMyShapes))
                    {
                        dcu.ChangeAlarmState((byte) state);
                    }

                    break;
            }
        }

        public void ChangeInputState(Guid inputGuid, State state)
        {
            if (EditMode)
                return;

            var inputs = GetLiveObjectsById<GraphicsIO>(inputGuid,
                _canvasMyShapes).Cast<ILiveObject>();

            ChangeStates(inputs, (byte) state);
        }

        private void _bOpenFile_Click(object sender, RoutedEventArgs e)
        {
            ImportBackgroundMenuClick();
        }

        private double GetDefaultScale()
        {
            double defaultScale = 0;

            if (Scene == null)
                return defaultScale;

            if (_cadVisualizer != null
                && _scene.BackgroundDataType == SymbolDataType.Vector)
            {
                defaultScale = _cadVisualizer.DefaultZoom;
            }
            else if (_backgroundImage != null
                     && _backgroundImage.Source != null)
            {
                defaultScale = GetOptimalZoom();
            }

            return defaultScale;
        }

        private void _cbZoomValues_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var selected = (_cbZoomValues.SelectedItem as System.Windows.Controls.ComboBoxItem);

                if (selected == null)
                    return;

                string value = selected.Content.ToString();
                double scale;

                if (value == "Window")
                {
                    _defaultScale = GetDefaultScale();

                    if (_defaultScale > 0)
                        SetScale(_defaultScale);
                }
                else
                {
                    scale = Double.Parse(value.Substring(0, value.Length - 1))/100;
                    SetScale(scale);
                }
            }
            catch
            {
            }
        }

        private void _cbZoomValues_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;

            string value = _cbZoomValues.Text;
            try
            {
                _cbZoomValues.SelectedItem = null;
                double scale = Double.Parse(value)/100;
                SetScale(scale);
            }
            catch
            {
            }
        }
        
        private void BackgroundSettingsMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            CloseAllOpenedWindow();
            HideScaleRatioSelector();
            SetVisibilityForBackgroundInformationPanel(true);

            if (_backgroundBitmap == null)
                _bSelectScaleRatio.IsEnabled = false;
            else
                _bSelectScaleRatio.IsEnabled = true;
        }

        private void _bSelectScaleRatio_OnClick(object sender, RoutedEventArgs e)
        {
            ShowScaleRatioSelector();
        }

        private void _bPaintRectangle_OnClick(object sender, RoutedEventArgs e)
        {
            UserInterfaceMode = InterfaceMode.Paint;
            _paintShape = PaintMode.Rectangle;
        }

        private void _bPaintEllipse_OnClick(object sender, RoutedEventArgs e)
        {
            UserInterfaceMode = InterfaceMode.Paint;
            _paintShape = PaintMode.Ellipse;
        }

        private void _bPaintAlarmArea_OnClick(object sender, RoutedEventArgs e)
        {
            UserInterfaceMode = InterfaceMode.Paint;
            _paintShape = PaintMode.AlarmAreaPolygon;
        }

        private void _bSceneSettings_OnClick(object sender, RoutedEventArgs e)
        {
            ShowSceneSettings();
        }

        private void _bPaintSymbol_OnClick(object sender, RoutedEventArgs e)
        {
            var windowSelectDialog = new SelectSymbolDialog(_symbolCache, _filterKeysOfSymbols);

            if (windowSelectDialog.ShowDialog() == true)
            {
                UserInterfaceMode = InterfaceMode.Paint;
                _paintObjectType = windowSelectDialog.SelectedObjectType;

                if (windowSelectDialog.SelectedSymbol == SymbolType.AlarmArea)
                    _paintShape = PaintMode.AlarmAreaPolygon;
                else
                {
                    _paintShape = PaintMode.Image;
                    _paintSymbolType = windowSelectDialog.SelectedSymbol;

                    if (windowSelectDialog.SelectedObjectType == ObjectType.Input)
                        _paintIoType = IOType.Input;

                    if (windowSelectDialog.SelectedObjectType == ObjectType.Output)
                        _paintIoType = IOType.Output;
                }

                _newObjectGuid = windowSelectDialog.SelectedObjectGuid;
                _newObjectCategories = windowSelectDialog.Categories;
            }

            LocalizationHelper.TranslateWpfControl("NCASSceneEditForm", this);
        }

        private void _sliderSizeOfSymbol_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int size = (int) (sender as Slider).Value;
            _displayWidth = size;
        }

        private void _sliderSizeOfSymbol_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SetSizeForFixedSizeMode();
        }

        private void _cbSizeMode_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_selectSybolSizeModeEnable)
                return;

            if (_cbSizeMode.SelectedItem != null)
            {
                CanvasSerialization.CanvasSettings.DefaultSymbolSize = ((SymbolSizeMode) _cbSizeMode.SelectedItem).Value;
                SetSymbolSizeMode();
            }
        }
        
        private void _bShowScenes_OnClick(object sender, RoutedEventArgs e)
        {
            ShowScenes();
        }
        
        private void _bSaveScene_OnClick(object sender, RoutedEventArgs e)
        {
            SaveSceneClick();
            _needSave = false;
        }

        private void _bSwitchToEditMode_OnClick(object sender, RoutedEventArgs e)
        {
            SetEditMode(true);
        }

        private void SetEditMode(bool editMode)
        {
            //Reload scene
            EditMode = editMode;
            LoadScene();

            if (Scene.BackgroundDataType == SymbolDataType.Vector
                && _cadVisualizer != null)
                _cadVisualizer.Draw();

            ChangeEditMode(EditMode);
        }

        private void ToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            if (_insertBackgroundMode)
            {
                if (_lbLayers.Items[0] is DxfLayer)
                {
                    ResetOffset();
                    IwQuick.Threads.SafeThread<DxfModel, bool>.StartThread(LoadModel, _model, false);
                }
                else
                    SetShapesVisibilityOnCanvasByLayers();
            }
            else
            {
                foreach (CategoryInfo categInfo in _lbCategories.ItemsSource)
                    CanvasSerialization.Categories[categInfo.Category] = categInfo.IsEnable;

                SetShapesVisibilityOnCanvasByLayers();
            }
        }

        private void _bInsertSymbol_OnClick(object sender, RoutedEventArgs e)
        {
            if (ButtonClick != null)
                ButtonClick((sender as Button).Name);
        }

        private void _bInsertText_OnClick(object sender, RoutedEventArgs e)
        {
            UserInterfaceMode = InterfaceMode.Paint;
            _paintShape = PaintMode.Text;
        }

        private void _miDoorEnvironment_OnClick(object sender, RoutedEventArgs e)
        {
            UserInterfaceMode = InterfaceMode.Paint;
            _paintShape = PaintMode.Image;
            _paintSymbolType = SymbolType.DoorEnviromentLeft;
        }

        private void UIElement_OnLostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = (TextBlock)((Grid)((TextBox)sender).Parent).Children[1];
            textBox.Text = ((TextBox)sender).Text;
            textBox.Visibility = Visibility.Visible;
            ((TextBox)sender).Visibility = Visibility.Collapsed;
        }

        private void _lbLayers_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!EditMode)
                return;

            var listView = sender as ListView;

            if (listView.SelectedIndex == -1)
                return;

            var item = listView.ItemContainerGenerator.ContainerFromIndex(listView.SelectedIndex) as ListViewItem;

            if (item != null)
            {
                var templateParent = GetFrameworkElementByName<ContentPresenter>(item);
                var dataTemplate = listView.ItemTemplate;

                if (dataTemplate != null && templateParent != null)
                {
                    var textBlock = dataTemplate.FindName("textBlock", templateParent) as TextBlock;
                    var textBox = dataTemplate.FindName("textBox", templateParent) as TextBox;

                    if (textBox != null && textBlock != null)
                    {
                        textBox.Visibility = Visibility.Visible;
                        Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle,
                                    new Action(delegate()
                                    {
                                        textBox.Focus();
                                        textBox.CaretIndex = textBox.Text.Length;
                                    }));
                        textBlock.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        private static T GetFrameworkElementByName<T>(FrameworkElement referenceElement) where T : FrameworkElement
        {
            FrameworkElement child = null;

            for (Int32 i = 0; i < VisualTreeHelper.GetChildrenCount(referenceElement); i++)
            {
                child = VisualTreeHelper.GetChild(referenceElement, i) as FrameworkElement;
                System.Diagnostics.Debug.WriteLine(child);

                if (child != null && child.GetType() == typeof(T))
                    break;

                if (child != null)
                {
                    child = GetFrameworkElementByName<T>(child);

                    if (child != null && child.GetType() == typeof(T))
                        break;
                }
            }

            return child as T;
        }

        private void _bPaintPolyline_OnClick(object sender, RoutedEventArgs e)
        {
            UserInterfaceMode = InterfaceMode.Paint;
            _paintShape = PaintMode.Polyline;
        }

        private void _bPaintLine_OnClick(object sender, RoutedEventArgs e)
        {
            UserInterfaceMode = InterfaceMode.Paint;
            _paintShape = PaintMode.Line;
        }

        private void _canvasMyShapes_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2
                && UserInterfaceMode == InterfaceMode.Paint)
            {
                PressEnterKey();
            }
        }

        private void _canvasMyShapes_OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            PressDeleteKey();
        }

        private void _bSwitchToViewMode_OnClick(object sender, RoutedEventArgs e)
        {
            if (Scene.RowDataBackground == null
                || string.IsNullOrEmpty(Scene.Name)
                || Scene.IdScene == Guid.Empty)
            {
                IwQuick.UI.Dialog.Error(LocalizationHelper.GetString("ErrorCreateScene"));
                return;
            }

            if (_needSave
                && IwQuick.UI.Dialog.Question(LocalizationHelper.GetString("QuestionSaveScene")))
            {
                SaveSceneClick();
            }

            _needSave = false;
            SetEditMode(false);
        }

        private void _bGoHome_OnClick(object sender, RoutedEventArgs e)
        {
            if (Scene.IdScene == _idBasicScene)
                return;

            _sceneHistory.Clear();
            GoToScene(_idBasicScene);
        }

        private void _bGoBack_OnClick(object sender, RoutedEventArgs e)
        {
            if (_sceneHistory.Count == 0)
                return;

            GoToScene(_sceneHistory.Last.Value);
            _sceneHistory.RemoveLast();

            if (_sceneHistory.Count == 0)
            {
                _bGoBack.IsEnabled = false;
                _bGoBack.Opacity = 0.5;
            }
        }

        public void UnRegisterEvents()
        {
            if (_textSettingsPanel != null)
                _textSettingsPanel.DisposeLocalizationHelper();
           
            if (_openedSceneSettingsWindow != null)
                _openedSceneSettingsWindow.DisposeLocalizationHelper();

            if (_shapeSettingsPanel != null)
                _shapeSettingsPanel.DisposeLocalizationHelper();

            GraphicsThumb.ThumbClick -= Thumbs_ThumbClick;
            _timerForUpdateInformation.Stop();
            _timerForUpdateInformation.Tick -= TimerForUpdateInformation_Tick;
        }

        private double _lastWidthOfLayersPanel;

        private void _bShowAndHidePanel_OnClick(object sender, RoutedEventArgs e)
        {
            string path = "Resources/hide12H.png";

            if (myGrid.ColumnDefinitions[0].Width.Value == 0)
            {
                myGrid.ColumnDefinitions[0].Width = new GridLength(_lastWidthOfLayersPanel);
                myGrid.ColumnDefinitions[0].MinWidth = 220;
                gridSplitter1.Visibility = Visibility.Visible;
            }
            else
            {
                _lastWidthOfLayersPanel = myGrid.ColumnDefinitions[0].Width.Value;
                myGrid.ColumnDefinitions[0].Width = new GridLength(0);
                myGrid.ColumnDefinitions[0].MinWidth = 0;
                gridSplitter1.Visibility = Visibility.Hidden;
                path = "Resources/show12H.png";
            }

            var source = new BitmapImage();
            source.BeginInit();
            source.UriSource = new Uri(path, UriKind.Relative);
            source.EndInit();

            ((Image)_bShowAndHidePanel.Content).Source = source;
        }

        private void _bPanningMode_OnClick(object sender, RoutedEventArgs e)
        {
            UserInterfaceMode = InterfaceMode.Panning;
        }

        public class SymbolSizeMode
        {
            public SymbolSize Value { get; private set; }

            public SymbolSizeMode(SymbolSize value)
            {
                Value = value;
            }

            public override string ToString()
            {
                return LocalizationHelper.GetString("SymbolSize_" + Value);
            }
        }
    }

    internal enum InterfaceMode
    {
        Paint,
        Select,
        Panning,
        PolygonAddNewPoint,
        PolygonRemovePoint
    }

    public enum PaintMode
    {
        Rectangle,
        Ellipse,
        AlarmAreaPolygon,
        Image,
        Text,
        Polyline,
        Line
    }

    public class SymbolParemeter
    {
        public SymbolType SymbolType { get; set; }
        public State SymbolState { get; set; }
        public ImageSource ImageSource { get; set; }

        public SymbolParemeter(SymbolType type, State state, ImageSource source)
        {
            SymbolType = type;
            SymbolState = state;
            ImageSource = source;
        }
    }

    public class CategoryInfo
    {
        public bool IsEnable { get; set; }
        public string Name { get; private set; }
        public Category Category { get; set; }

        public CategoryInfo(Category category, bool isEnable)
        {
            Category = category;
            IsEnable = isEnable;
            Name = GraphicsScene.LocalizationHelper.GetString(string.Format("Category_{0}", category.ToString()));
        }
    }
}
