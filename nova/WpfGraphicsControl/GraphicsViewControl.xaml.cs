using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using UserControl = System.Windows.Controls.UserControl;
using VerticalAlignment = System.Windows.VerticalAlignment;

using Cgp.NCAS.WpfGraphicsControl;
using Contal.Cgp.NCAS.RemotingCommon;
using Contal.Cgp.NCAS.Server.Beans;

namespace Contal.Cgp.NCAS.WpfGraphicsControl
{
    /// <summary>
    /// Interaction logic for GraphicsView.xaml
    /// </summary>
    public partial class GraphicsViewControl : UserControl
    {
        private IGraphicsObject _selectedGraphicsObject;
        private CanvasMode _canvasMode = CanvasMode.Select;
        private IGraphicsObject _drawingGraphicsObject;
        private readonly GraphicsBoxSettings _graphicsBoxSettings = new GraphicsBoxSettings();
        private readonly LinkedList<SceneBoxHistory> _history = new LinkedList<SceneBoxHistory>(); 

        private enum CanvasMode
        {
            Select, Paint
        }

        private Point _lastMousePosition;
        private Point _fixPoint;
        private readonly CanvasSerialization _canvasSerialization = new CanvasSerialization();
        private GraphicsView _graphicsView;
        private bool _saveHistory = true;
        private GraphicsViewSettings _graphicsViewSettingsPanel;

        public GraphicsSceneEventsHelper GraphicsSceneEventsHelper { get; set; }
        public bool EditMode { get; set; }
        public static ICgpNCASRemotingProvider MainServerProvider { get; set; }

        public GraphicsView GraphicsView
        {
            get
            {
                return _graphicsView;
            }
            set
            {
                _graphicsView = value;
                LoadGraphicsView();
            }
        }

        public delegate void DSaveView();
        public event DSaveView SaveView;

        public GraphicsViewControl()
        {
            InitializeComponent();
            DisableHistoryButtons();
        }

        public void DisposeLocalizationHelper()
        {
            _graphicsBoxSettings.DisposeLocalizationHelper();
        }

        private void LoadGraphicsView()
        {
            //fix for running keys events
            Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle,
                new Action(() => Focus()));

            _canvasScenes.Children.Clear();
            _canvasSerialization.LoadFromArray(_canvasScenes, _graphicsView.RowData, null);
            GraphicsSceneEventsHelper.RemoveAllGraphicsScenes();

            foreach (var element in _canvasScenes.Children)
            {
                var sceneBox = element as GraphicsSceneBox;

                if (sceneBox != null)
                {
                    sceneBox.MouseDown += sceneBox_MouseDown;
                    sceneBox.EditMode = EditMode;   
                    GraphicsSceneEventsHelper.AddGraphicsScene(sceneBox.GetGraphicsScene());
                    
                    if (!EditMode && sceneBox.GetSceneId() != Guid.Empty)
                    {
                        var scene = MainServerProvider.Scenes.GetObjectById(sceneBox.GetSceneId());

                        if (scene != null)
                            sceneBox.Scene = scene;
                    }

                    sceneBox.SceneChanged += sceneBox_SceneChanged;
                }
            }

            _graphicsViewSettingsPanel = new GraphicsViewSettings(_graphicsView);
            _mainGrid.Children.Add(_graphicsViewSettingsPanel);
            Grid.SetColumn(_graphicsViewSettingsPanel, 0);
            Grid.SetRow(_graphicsViewSettingsPanel, 1);
            _graphicsViewSettingsPanel.VerticalAlignment = VerticalAlignment.Center;
            _graphicsViewSettingsPanel.HorizontalAlignment = HorizontalAlignment.Center;
            _graphicsViewSettingsPanel.Visibility = Visibility.Collapsed;
            _graphicsViewSettingsPanel.OkClickEvent += _graphicsViewSettingsPanel_OkClickEvent;
        }

        void _graphicsViewSettingsPanel_OkClickEvent()
        {
            Save();
        }

        private void Save()
        {
            GraphicsView.RowData = _canvasSerialization.SaveToArray(_canvasScenes);

            if (SaveView != null)
                SaveView();
        }

        void sceneBox_SceneChanged(GraphicsSceneBox sender, Scene oldScene, Scene newScene)
        {
            if (_saveHistory)
            {
                _history.AddLast(new SceneBoxHistory(sender.BoxName, oldScene));
                EnableHistoryButtons();
            }
        }

        private void _bAddScene_Click(object sender, RoutedEventArgs e)
        {
            int index = 0;

            while (GetSceneBoxName("Scene Box " + index) != null)
            {
                index++;
            }

            InsertSceneBox("Scene Box " + index);
        }

        private void InsertSceneBox(string name)
        {
            if (GetSceneBoxName(name) != null)
                return;

            _canvasMode = CanvasMode.Paint;
            var newSceneBox = new GraphicsSceneBox(_canvasScenes)
            {
                EditMode = true, 
                BoxName = name
            };

            _drawingGraphicsObject = newSceneBox;
        }

        private GraphicsSceneBox GetSceneBoxName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            return _canvasScenes.Children
                .OfType<GraphicsSceneBox>()
                .FirstOrDefault(
                    sceneBox =>
                        sceneBox.BoxName == name);
        }

        public LinkedList<GraphicsSceneBox> GetAllSceneBoxs()
        {
            var sceneBoxs = new LinkedList<GraphicsSceneBox>();

            foreach (var element in _canvasScenes.Children)
            {
                var sceneBox = element as GraphicsSceneBox;

                if (sceneBox != null)
                {
                    sceneBoxs.AddLast(sceneBox);
                }
            }

            return sceneBoxs;
        }

        private void UnSelectAll()
        {
            var sceneBoxs = GetAllSceneBoxs();

            foreach (var sceneBox in sceneBoxs)
                sceneBox.UnSelect();
        }

        void sceneBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_canvasMode == CanvasMode.Paint)
                return;

            UnSelectAll();
            var sceneBox = sender as GraphicsSceneBox;

            if (sceneBox != null)
            {
                sceneBox.Select(true);
                _selectedGraphicsObject = sceneBox;
                _graphicsBoxSettings.IsEnabled = true;
                _graphicsBoxSettings.SceneBox = sceneBox;
            }
        }

        private void _canvasScenes_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                var position = e.GetPosition(_canvasScenes);

                if (_canvasMode == CanvasMode.Select)
                {
                    var result = VisualTreeHelper.HitTest(_canvasScenes, position);

                    if (result == null)
                        return;

                    if (result.VisualHit is Canvas)
                    {
                        if (_selectedGraphicsObject != null)
                        {
                            _selectedGraphicsObject.UnSelect();
                            _graphicsBoxSettings.IsEnabled = false;
                        }

                        _selectedGraphicsObject = null;
                    }
                }
                else if (_canvasMode == CanvasMode.Paint
                    && _drawingGraphicsObject != null)
                {
                    _drawingGraphicsObject.SetLeft(position.X);
                    _drawingGraphicsObject.SetTop(position.Y);
                    _drawingGraphicsObject.SetHeight(0);
                    _drawingGraphicsObject.SetWidth(0);
                    var element = (UIElement) _drawingGraphicsObject;
                    _canvasScenes.Children.Add(element);
                    element.MouseDown += sceneBox_MouseDown;
                    _lastMousePosition = position;
                    _fixPoint = position;
                }
            }
        }

        private void _canvasScenes_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_canvasMode == CanvasMode.Paint
                && _drawingGraphicsObject != null)
            {
                var position = e.GetPosition(_canvasScenes);

                if (position.X >= _fixPoint.X)
                    _drawingGraphicsObject.SetWidth(position.X - _lastMousePosition.X);
                else
                {
                    _drawingGraphicsObject.SetLeft(position.X);
                    _drawingGraphicsObject.SetWidth(_fixPoint.X - position.X);
                }

                if (position.Y >= _fixPoint.Y)
                    _drawingGraphicsObject.SetHeight(position.Y - _lastMousePosition.Y);
                else
                {
                    _drawingGraphicsObject.SetTop(position.Y);
                    _drawingGraphicsObject.SetHeight(_fixPoint.Y - position.Y);
                }
            }
        }

        private void _canvasScenes_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_canvasMode == CanvasMode.Paint)
            {
                _canvasMode = CanvasMode.Select;

                if (_drawingGraphicsObject.GetWidth() < 50)
                    _drawingGraphicsObject.SetWidth(50);

                if (_drawingGraphicsObject.GetHeight() < 50)
                    _drawingGraphicsObject.SetHeight(50);

                UnSelectAll();
                _drawingGraphicsObject.Select(true);
                _selectedGraphicsObject = _drawingGraphicsObject;
                _graphicsBoxSettings.SceneBox = _selectedGraphicsObject as GraphicsSceneBox;
                _drawingGraphicsObject = null;
            }
        }

        private void _bSaveView_OnClick(object sender, RoutedEventArgs e)
        {
            _graphicsViewSettingsPanel.Show();
        }

        private void GraphicsViewControl_OnKeyUp(object sender, KeyEventArgs e)
        {
            var sceneBox = _selectedGraphicsObject as GraphicsSceneBox;

            if (sceneBox != null)
            {
                sceneBox.AllowedSniping = false;

                switch (e.Key)
                {
                    case Key.Up:
                        sceneBox.MoveTop(-1);
                        break;

                    case Key.Down:
                        sceneBox.MoveTop(1);
                        break;

                    case Key.Left:
                        sceneBox.MoveLeft(-1);
                        break;

                    case Key.Right:
                        sceneBox.MoveLeft(1);
                        break;

                    case Key.Delete:
                        sceneBox.Delete();
                        break;
                }

                sceneBox.AllowedSniping = true;
            }
        }

        private void GraphicsViewControl_OnLoaded(object sender, RoutedEventArgs e)
        {
            SetButtonState(EditMode);

            if (EditMode)
            {
                ShowSettingsPanel();
            }
            else
            {
                HideSettingsPanel();
            }
        }

        private void ShowSettingsPanel()
        {
            if (!_mainGrid.Children.Contains(_graphicsBoxSettings))
                _mainGrid.Children.Add(_graphicsBoxSettings);

            _mainGrid.ColumnDefinitions[1].Width = new GridLength(300);
            Grid.SetColumn(_graphicsBoxSettings, 1);
            Grid.SetRow(_graphicsBoxSettings, 1);
            _graphicsBoxSettings.VerticalAlignment = VerticalAlignment.Stretch;
            _graphicsBoxSettings.HorizontalAlignment = HorizontalAlignment.Stretch;
        }

        private void HideSettingsPanel()
        {
            if (_mainGrid.Children.Contains(_graphicsBoxSettings))
                _mainGrid.Children.Remove(_graphicsBoxSettings);

            _mainGrid.ColumnDefinitions[1].Width = new GridLength(0);
        }

        private void _bRemoveScene_OnClick(object sender, RoutedEventArgs e)
        {
            var sceneBox = _selectedGraphicsObject as GraphicsSceneBox;

            if (sceneBox != null)
                sceneBox.Delete();
        }

        private void SetButtonState(bool editMode)
        {
            if (editMode)
            {
                _bSwitchToViewMode.Visibility = Visibility.Visible;
                _bSwitchToEditMode.Visibility = Visibility.Collapsed;
                _bDelete.Visibility = Visibility.Visible;
                _bSaveScene.Visibility = Visibility.Visible;
                _bGoHome.Visibility = Visibility.Collapsed;
                _bGoBack.Visibility = Visibility.Collapsed;
                _bAddSceneBox.Visibility = Visibility.Visible;
            }
            else
            {
                _bSwitchToViewMode.Visibility = Visibility.Collapsed;
                _bSwitchToEditMode.Visibility = Visibility.Visible;
                _bDelete.Visibility = Visibility.Collapsed;
                _bSaveScene.Visibility = Visibility.Collapsed;
                _bGoHome.Visibility = Visibility.Visible;
                _bGoBack.Visibility = Visibility.Visible;
                _bAddSceneBox.Visibility = Visibility.Collapsed;
            }
        }

        private void _bSwitchToEditMode_OnClick(object sender, RoutedEventArgs e)
        {
            if (!EditMode)
            {
                EditMode = true;
                GraphicsView = _graphicsView;
                ShowSettingsPanel();
                SetButtonState(true);
            }
        }

        private void _bSwitchToRunMode_OnClick(object sender, RoutedEventArgs e)
        {
            if (EditMode)
            {
                var result = IwQuick.UI.Dialog.Question(GraphicsScene.LocalizationHelper.GetString("QuestionSaveScene"));

                if (result)
                {
                    Save();
                }

                UnSelectAll();
                EditMode = false;
                GraphicsView = _graphicsView;
                HideSettingsPanel();
                SetButtonState(false);
            }
        }

        private void _bGoHome_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (var sceneBox in GetAllSceneBoxs())
                sceneBox.ReturnToHome();

            DisableHistoryButtons();
            _history.Clear();
        }

        private void EnableHistoryButtons()
        {
            _bGoHome.IsEnabled = true;
            _bGoHome.Opacity = 1.0;
            _bGoBack.IsEnabled = true;
            _bGoBack.Opacity = 1.0;
        }

        private void DisableHistoryButtons()
        {
            _bGoHome.IsEnabled = false;
            _bGoHome.Opacity = 0.5;
            _bGoBack.IsEnabled = false;
            _bGoBack.Opacity = 0.5;
        }

        private void _bGoBack_OnClick(object sender, RoutedEventArgs e)
        {
            if (_history.Count > 0)
            {
                var sceneBox = GetSceneBoxName(_history.Last.Value.SceneBoxName);

                if (sceneBox != null)
                {
                    _saveHistory = false;
                    sceneBox.Scene = _history.Last.Value.Scene;
                    _saveHistory = true;
                    _history.RemoveLast();

                    if (_history.Count == 0)
                    {
                        DisableHistoryButtons();
                    }
                }
            }
        }
    }

    public class SceneBoxHistory
    {
        public string SceneBoxName { get; set; }
        public Scene Scene { get; set; }

        public SceneBoxHistory(string sceneBoxName, Scene scene)
        {
            SceneBoxName = sceneBoxName;
            Scene = scene;
        }
    }
}
