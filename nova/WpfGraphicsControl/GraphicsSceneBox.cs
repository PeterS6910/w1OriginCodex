using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using Cgp.NCAS.WpfGraphicsControl;
using Contal.Cgp.NCAS.Server.Beans;
using System.Windows.Input;

namespace Contal.Cgp.NCAS.WpfGraphicsControl
{
    public class GraphicsSceneBox : StackPanel, IGraphicsObject, ISceneLink
    {
        private readonly Canvas _canvas;
        private bool _editMode;
        private bool _isSelected;
        private readonly Rectangle _border = new Rectangle();
        private readonly GraphicsScene _graphicsScene;
        private Scene _scene;
        private Guid _idScene;
        private bool _move;
        private double _lastMousePosX;
        private double _lastMousePosY;
        public readonly TextBlock _textBlock = new TextBlock();
        private string _name;
        private const int _snipingDistance = 8;
        private readonly HashSet<GraphicsSceneBox> _snipingSceneBoxsTop = new HashSet<GraphicsSceneBox>(); 
        private readonly HashSet<GraphicsSceneBox> _snipingSceneBoxsBottom = new HashSet<GraphicsSceneBox>();
        private readonly HashSet<GraphicsSceneBox> _snipingSceneBoxsLeft = new HashSet<GraphicsSceneBox>();
        private readonly HashSet<GraphicsSceneBox> _snipingSceneBoxsRight = new HashSet<GraphicsSceneBox>(); 
        private readonly System.Windows.Shapes.Line _snipingHorizontalLine = new System.Windows.Shapes.Line();
        private readonly System.Windows.Shapes.Line _snipingVerticalLine = new System.Windows.Shapes.Line();
        private int _delayForSnipingVertical, _delayForSnipingHorizontal;
        private readonly Timer _timerForHorizontalLineHide = new Timer(3000);
        private readonly Timer _timerForVerticalLineHide = new Timer(3000);
        private bool _allowedSniping;
        private int _borderWidth;
        private SerializableColor _borderColor;
        private readonly Rectangle _mainBorder = new Rectangle();
        private Scene _baseScene;

        public readonly List<GraphicsThumb> Thumbs = new List<GraphicsThumb>();
        public double RightDistance { get; set; }
        public double BottomDistance { get; set; }
        public double PercentuallyWidth { get; set; }
        public double PercentuallyHeight { get; set; }
        public bool EnablePercentuallyWidth { get; set; }
        public bool EnablePercentuallyHeight { get; set; }
        public int ZIndex { get; set; }
        public GraphicsBoxHorizontalAlignment HAlignment { get; set; }
        public GraphicsBoxVerticalAlignment VAlignment { get; set; }

        public delegate void DSceneChanged(GraphicsSceneBox sender, Scene oldScene, Scene newScene);
        public event DSceneChanged SceneChanged; 

        public int BorderWidth 
        {
            get { return _borderWidth; }
            set
            {
                _borderWidth = value;
                _mainBorder.StrokeThickness = _borderWidth;

                if (_borderWidth > 0)
                    CreateBorder();
                else if (_canvas.Children.Contains(_mainBorder))
                    _canvas.Children.Remove(_mainBorder);

                SetMainBorderPosition();
            }
        }

        public SerializableColor BorderColor
        {
            get { return _borderColor; }
            set
            {
                _borderColor = value;

                var color = _borderColor != null
                    ? _borderColor.GetColor()
                    : Colors.Black;

                _mainBorder.Stroke = new SolidColorBrush(color);
                CreateBorder();
            }
        }

        private void CreateBorder()
        {
            if (_canvas != null
                && !_canvas.Children.Contains(_mainBorder))
            {
                _canvas.Children.Add(_mainBorder);
            }
        }

        public GraphicsScene GetGraphicsScene()
        {
            return _graphicsScene;
        }

        public delegate void DPositionChanged(double left, double top);
        public event DPositionChanged PositionChanged;

        public bool EditMode
        {
            get { return _editMode; }
            set
            {
                _editMode = value;

                if (_editMode)
                {
                    if (Children.Contains(_graphicsScene))
                        Children.Remove(_graphicsScene);

                    Background = new SolidColorBrush(Colors.White);
                    _graphicsScene.Visibility = Visibility.Collapsed;
                    _textBlock.Text = string.IsNullOrEmpty(BoxName) ? "SceneBox" : BoxName;
                    Children.Add(_textBlock);
                    _textBlock.TextAlignment = TextAlignment.Center;
                    _textBlock.VerticalAlignment = VerticalAlignment.Center;
                    _textBlock.HorizontalAlignment = HorizontalAlignment.Center;
                    _textBlock.Width = GetWidth();
                }
                else if (!Children.Contains(_graphicsScene))
                {
                    Children.Add(_graphicsScene);
                }
            }
        }

        public Scene Scene
        {
            get
            {
                return _scene;
            }
            set
            {
                if (value == null)
                    return;

                if (_baseScene == null)
                    _baseScene = value;

                if (_scene != null 
                    && value.IdScene == _scene.IdScene)
                    return;

                var lastScene = _scene;
                _scene = value;
                _graphicsScene.Width = Width;
                _graphicsScene.Height = Height;
                _graphicsScene.Scene = _scene;

                if (SceneChanged != null)
                    SceneChanged(this, lastScene, _scene);
            }
        }

        public string BoxName
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                _textBlock.Text = _name;
            }
        }

        public bool AllowedSniping
        {
            get { return _allowedSniping; }
            set
            {
                _allowedSniping = value;
                _delayForSnipingHorizontal = 0;
                _delayForSnipingHorizontal = 0;
            }
        }

        public string SceneLinkTarget { get; set; }

        public void ReturnToHome()
        {
            Scene = _baseScene;
        }

        public override string ToString()
        {
            return BoxName;
        }

        public GraphicsSceneBox(Canvas canvas)
        {
            _graphicsScene = new GraphicsScene(true);
            Unloaded += GraphicsSceneBox_Unloaded;
            _canvas = canvas;
            HAlignment = GraphicsBoxHorizontalAlignment.Left;
            VAlignment = GraphicsBoxVerticalAlignment.Top;
            AllowedSniping = true;
            Orientation = Orientation.Horizontal;
            MouseDown += GraphicsSceneBox_MouseDown;
            MouseMove += GraphicsSceneBox_MouseMove;
            MouseUp += GraphicsSceneBox_MouseUp;
            MouseLeave += GraphicsSceneBox_MouseLeave;
            SizeChanged += GraphicsSceneBox_SizeChanged;
            LayoutUpdated += GraphicsSceneBox_LayoutUpdated;
            _graphicsScene.NeedSceneChanged += _graphicsScene_NeedSceneChanged;
            _canvas.SizeChanged += _canvas_SizeChanged;
            _canvas.MouseUp += _canvas_MouseUp;
            _timerForHorizontalLineHide.Elapsed += _timerForHorizontalLineHide_Elapsed;
            _timerForHorizontalLineHide.Enabled = false;
            _timerForVerticalLineHide.Elapsed += _timerForVerticalLineHide_Elapsed;
            _timerForVerticalLineHide.Enabled = false;
            BorderColor = new SerializableColor(Colors.Black); //default color
            BorderWidth = 0; //default border width
        }

        void GraphicsSceneBox_LayoutUpdated(object sender, EventArgs e)
        {
        }

        void GraphicsSceneBox_Unloaded(object sender, RoutedEventArgs e)
        {
            _graphicsScene.UnRegisterEvents();
        }

        void _canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _delayForSnipingHorizontal = 0;
            _delayForSnipingVertical = 0;
        }

        void _graphicsScene_NeedSceneChanged(Scene scene)
        {
            if (string.IsNullOrEmpty(SceneLinkTarget))
            {
                Scene = scene;
                return;
            }

            var sceneBox = GetSceneBoxByName(SceneLinkTarget);

            if (sceneBox != null)
                sceneBox.Scene = scene;
        }

        void _timerForVerticalLineHide_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle,
                new Action(HideVerticalSnipingLine));
            
            _timerForVerticalLineHide.Enabled = false;
        }

        void _timerForHorizontalLineHide_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle,
                new Action(HideHorizontalSnipingLine));

            _timerForHorizontalLineHide.Enabled = false;
        }

        void _canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _delayForSnipingHorizontal = 0;
            _delayForSnipingVertical = 0;
            
            if (HAlignment == GraphicsBoxHorizontalAlignment.Stretch)
            {
                double newWidth;

                if (EnablePercentuallyWidth)
                {
                    newWidth = _canvas.ActualWidth * PercentuallyWidth;
                    double newLeft = GetLeft() * (newWidth / GetWidth());
                    SetLeft(newLeft);
                }
                else
                {
                    newWidth = e.NewSize.Width - GetLeft() - RightDistance;
                }

                Width = newWidth < 0 ? 0 : newWidth;
            }
            else if (HAlignment == GraphicsBoxHorizontalAlignment.Right)
            {
                double newLeft;

                if (EnablePercentuallyWidth)
                {
                    double newWidth = _canvas.ActualWidth * PercentuallyWidth;
                    newLeft = e.NewSize.Width - RightDistance - newWidth;
                    Width = newWidth < 0 ? 0 : newWidth;
                }
                else
                {
                    newLeft = e.NewSize.Width - RightDistance - GetWidth();
                }

                SetLeft(newLeft);
            }
            else if (EnablePercentuallyWidth)
            {
                double newWidth = _canvas.ActualWidth * PercentuallyWidth;
                Width = newWidth < 0 ? 0 : newWidth;
            }

            if (VAlignment == GraphicsBoxVerticalAlignment.Stretch)
            {
                double newHeight;

                if (EnablePercentuallyHeight)
                {
                    newHeight = _canvas.ActualHeight * PercentuallyHeight;
                    double newTop = GetTop() * (newHeight / GetHeight());
                    SetTop(newTop);
                }
                else
                {
                    newHeight = e.NewSize.Height - GetTop() - BottomDistance;
                }

                Height = newHeight < 0 ? 0 : newHeight;
            }
            else if (VAlignment == GraphicsBoxVerticalAlignment.Bottom)
            {
                double newTop;

                if (EnablePercentuallyHeight)
                {
                    double newHeight = _canvas.ActualHeight * PercentuallyHeight;
                    newTop = e.NewSize.Height - BottomDistance - newHeight;
                    Height = newHeight < 0 ? 0 : newHeight;
                }
                else
                {
                    newTop = e.NewSize.Height - BottomDistance - GetHeight();
                }

                SetTop(newTop);
            }
            else if (EnablePercentuallyHeight)
            {
                double newHeight = _canvas.ActualHeight * PercentuallyHeight;
                Height = newHeight < 0 ? 0 : newHeight;
            }

            if (_isSelected)
            {
                UnSelect();
                Select();
            }
            else
                SetMainBorderPosition();
        }

        void GraphicsSceneBox_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_editMode)
            {
                _textBlock.Width = GetWidth();
            }
            else
            {
                _graphicsScene.Width = GetWidth();
                _graphicsScene.Height = GetHeight();
            }
        }

        void GraphicsSceneBox_MouseLeave(object sender, MouseEventArgs e)
        {
            _move = false;
        }

        void GraphicsSceneBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                _move = false;
                _delayForSnipingVertical = 0;
            }
        }

        void GraphicsSceneBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (_move)
            {
                var position = e.GetPosition(_canvas);
                MoveLeft(position.X - _lastMousePosX);
                MoveTop(position.Y - _lastMousePosY);
                _lastMousePosX = position.X;
                _lastMousePosY = position.Y;
            }
        }

        void GraphicsSceneBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                var position = e.GetPosition(_canvas);
                _move = true;
                _lastMousePosX = position.X;
                _lastMousePosY = position.Y;
            }
        }

        public void Delete()
        {
            UnSelect();
            _canvas.Children.Remove(this);
            _canvas.Children.Remove(_mainBorder);
            _canvas.SizeChanged -= _canvas_SizeChanged;
            _canvas.MouseUp -= _canvas_MouseUp;
        }

        public void Select()
        {
            if (!EditMode)
                return;

            if (!_isSelected)
            {
                _isSelected = true;
                SetZIndex(this, 1);
                SetThumbsAndBorderPosition();
                _border.Stroke = Brushes.Blue;
                _border.StrokeThickness = 1 / _canvas.LayoutTransform.Value.M11;
                _canvas.Children.Add(_border);
                SetZIndex(_border, 100);
                SetZIndex(_mainBorder, 100);
                GraphicsOperations.CreateThumbsForRectange(_canvas, this, false, thumb_DragDelta);
            }
        }

        void thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            GraphicsOperations.ComputeThumbOperations(_canvas, sender as GraphicsThumb, e);
            SetThumbsAndBorderPosition();
            _graphicsScene.Width = Width;
            _graphicsScene.Height = Height;
            Sniping(false);
        }

        public void UnSelect()
        {
            if (_isSelected)
            {
                SetZIndex(this, 0);

                foreach (var thumb in Thumbs)
                    _canvas.Children.Remove(thumb);

                Thumbs.Clear();
                _canvas.Children.Remove(_border);
                SetZIndex(_mainBorder, 0);
                _isSelected = false;
            }
        }

        private void SetThumbsAndBorderPosition()
        {
            foreach (GraphicsThumb thumb in Thumbs)
            {
                thumb.SetPosition();
            }

            //move _border
            Canvas.SetLeft(_border, Canvas.GetLeft(this) - 1);
            Canvas.SetTop(_border, Canvas.GetTop(this) - 1);
            _border.StrokeThickness = 1;
            _border.Width = Width + 2;
            _border.Height = Height + 2;
            SetMainBorderPosition();
        }

        private void SetMainBorderPosition()
        {
            Canvas.SetLeft(_mainBorder, Canvas.GetLeft(this) - BorderWidth);
            Canvas.SetTop(_mainBorder, Canvas.GetTop(this) - BorderWidth);
            _mainBorder.Width = Width + BorderWidth * 2;
            _mainBorder.Height = Height + BorderWidth * 2;
        }

        private void SetRightDistance()
        {
            RightDistance = _canvas.ActualWidth - GetLeft() - GetWidth();

            if (RightDistance < 0)
                RightDistance = 0;
        }

        private void SetBottomDistance()
        {
            BottomDistance = _canvas.ActualHeight - GetTop() - GetHeight();

            if (BottomDistance < 0)
                BottomDistance = 0;
        }

        public LinkedList<GraphicsSceneBox> GetAllSceneBoxs()
        {
            var sceneBoxs = new LinkedList<GraphicsSceneBox>();

            foreach (var sceneBox in _canvas.Children.OfType<GraphicsSceneBox>())
                sceneBoxs.AddLast(sceneBox);

            return sceneBoxs;
        }

        private GraphicsSceneBox GetSceneBoxByName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            return _canvas.Children.OfType<GraphicsSceneBox>().FirstOrDefault(box => box.BoxName == name);
        }

        private void ShowSnipingHorizontalLine(double x1, double y1, double x2, double y2)
        {
            _snipingHorizontalLine.StrokeThickness = 0.5;
            _snipingHorizontalLine.Stroke = new SolidColorBrush(Colors.Black);
            SetZIndex(_snipingHorizontalLine, 100);
            _snipingHorizontalLine.X1 = x1;
            _snipingHorizontalLine.Y1 = y1;
            _snipingHorizontalLine.X2 = x2;
            _snipingHorizontalLine.Y2 = y2;

            if (!_canvas.Children.Contains(_snipingHorizontalLine))
                _canvas.Children.Add(_snipingHorizontalLine);
        }

        private void HideHorizontalSnipingLine()
        {
            if (_canvas.Children.Contains(_snipingHorizontalLine))
                _canvas.Children.Remove(_snipingHorizontalLine);
        }

        private void ShowSnipingVerticalLine(double x1, double y1, double x2, double y2)
        {
            _snipingVerticalLine.StrokeThickness = 0.5;
            _snipingVerticalLine.Stroke = new SolidColorBrush(Colors.Black);
            SetZIndex(_snipingVerticalLine, 100);
            _snipingVerticalLine.X1 = x1;
            _snipingVerticalLine.Y1 = y1;
            _snipingVerticalLine.X2 = x2;
            _snipingVerticalLine.Y2 = y2;

            if (!_canvas.Children.Contains(_snipingVerticalLine))
                _canvas.Children.Add(_snipingVerticalLine);
        }

        private void HideVerticalSnipingLine()
        {
            if (_canvas.Children.Contains(_snipingVerticalLine))
                _canvas.Children.Remove(_snipingVerticalLine);
        }

        private int _useVerticalLine;
        private int _useHorizontalLine;

        private void Sniping(bool setOnlyPosition)
        {
            if (!_allowedSniping)
                return;

            var sceneBoxs = GetAllSceneBoxs();

            foreach (var sceneBox in sceneBoxs)
            {
                if (sceneBox.Equals(this))
                    continue;

                SnipingVertical(sceneBox, setOnlyPosition);
                SetBottomDistance();
                SnipingHorizontal(sceneBox, setOnlyPosition);
                SetRightDistance();
            }
        }

        private void SnipingHorizontal(GraphicsSceneBox sceneBox, bool setOnlyPosition)
        {
            //sniping left-left
            if (sceneBox.GetLeft() - sceneBox.BorderWidth + _snipingDistance > GetLeft() - BorderWidth
                && sceneBox.GetLeft() - sceneBox.BorderWidth - _snipingDistance < GetLeft() - BorderWidth)
            {
                if (!_snipingSceneBoxsLeft.Contains(sceneBox)
                    && !(GetLeft() - BorderWidth).Equals(sceneBox.GetLeft() - sceneBox.BorderWidth)
                    && _delayForSnipingHorizontal == 0)
                {
                    double newLeft = sceneBox.GetLeft() - sceneBox.BorderWidth + BorderWidth;
                    
                    if (setOnlyPosition)
                    {
                        SetLeft(newLeft);
                        _delayForSnipingHorizontal = 20;
                    }
                    else
                    {
                        double oldDistanceRight = _canvas.ActualWidth - GetLeft() - GetWidth();
                        SetLeft(newLeft);
                        SetWidth(_canvas.ActualWidth - GetLeft() - oldDistanceRight);
                        _delayForSnipingHorizontal = 40;
                    }

                    ShowSnipingVerticalLine(GetLeft() - BorderWidth,
                        GetTop() > sceneBox.GetTop() ? sceneBox.GetTop() : GetTop(),
                        GetLeft() - BorderWidth,
                        GetTop() > sceneBox.GetTop() ? GetTop() + GetHeight() : sceneBox.GetTop() + sceneBox.GetHeight());

                    _timerForVerticalLineHide.Enabled = true;
                    _useVerticalLine++;
                    _snipingSceneBoxsLeft.Add(sceneBox);
                    return;
                }
            } //sniping left-right
            else if (sceneBox.GetLeft() + sceneBox.GetWidth() + sceneBox.BorderWidth + _snipingDistance > GetLeft() - BorderWidth
                && sceneBox.GetLeft() + sceneBox.GetWidth() + sceneBox.BorderWidth - _snipingDistance < GetLeft() - BorderWidth)
            {
                if (!_snipingSceneBoxsLeft.Contains(sceneBox)
                    && !(GetLeft() - BorderWidth).Equals(sceneBox.GetLeft() + sceneBox.GetWidth() + sceneBox.BorderWidth)
                    && _delayForSnipingHorizontal == 0)
                {
                    double newLeft = sceneBox.GetLeft() + sceneBox.GetWidth() + sceneBox.BorderWidth + BorderWidth;

                    if (setOnlyPosition)
                    {
                        SetLeft(newLeft);
                        _delayForSnipingHorizontal = 20;
                    }
                    else
                    {
                        double oldDistanceRight = _canvas.ActualWidth - GetLeft() - GetWidth();
                        SetLeft(newLeft);
                        SetWidth(_canvas.ActualWidth - GetLeft() - oldDistanceRight);
                        _delayForSnipingHorizontal = 40;
                    }

                    ShowSnipingVerticalLine(GetLeft() - BorderWidth,
                        GetTop() > sceneBox.GetTop() ? sceneBox.GetTop() : GetTop(),
                        GetLeft() - BorderWidth,
                        GetTop() > sceneBox.GetTop() ? GetTop() + GetHeight() : sceneBox.GetTop() + sceneBox.GetHeight());

                    _timerForVerticalLineHide.Enabled = true;
                    _useVerticalLine++;
                    _snipingSceneBoxsLeft.Add(sceneBox);
                    return;
                }
            }
            else
            {
                if (_snipingSceneBoxsLeft.Remove(sceneBox))
                {
                    _useVerticalLine--;

                    if (_useVerticalLine <= 0)
                        HideHorizontalSnipingLine();
                }
            }

            //sniping right - right
            if (sceneBox.GetLeft() + sceneBox.GetWidth() + sceneBox.BorderWidth + _snipingDistance > GetLeft() + GetWidth() + BorderWidth
                && sceneBox.GetLeft() + sceneBox.GetWidth() + sceneBox.BorderWidth - _snipingDistance < GetLeft() + GetWidth() + BorderWidth)
            {
                if (!_snipingSceneBoxsRight.Contains(sceneBox)
                    && !(GetLeft() + GetWidth() + BorderWidth).Equals(sceneBox.GetLeft() + sceneBox.GetWidth() + sceneBox.BorderWidth)
                    && _delayForSnipingHorizontal == 0)
                {
                    double newLeft = sceneBox.GetLeft() + sceneBox.GetWidth() + sceneBox.BorderWidth - GetWidth() - BorderWidth;

                    if (setOnlyPosition)
                    {
                        SetLeft(newLeft);
                        _delayForSnipingHorizontal = 20;
                    }
                    else
                    {
                        SetWidth(sceneBox.GetLeft() + sceneBox.GetWidth() + sceneBox.BorderWidth - GetLeft() - BorderWidth);
                        _delayForSnipingHorizontal = 40;
                    }

                    ShowSnipingVerticalLine(GetLeft() + GetWidth() + BorderWidth,
                        GetTop() > sceneBox.GetTop() ? sceneBox.GetTop() : GetTop(),
                        GetLeft() + GetWidth() + BorderWidth,
                        GetTop() > sceneBox.GetTop() ? GetTop() + GetHeight() : sceneBox.GetTop() + sceneBox.GetHeight());

                    _timerForVerticalLineHide.Enabled = true;
                    _useVerticalLine++;
                    _snipingSceneBoxsRight.Add(sceneBox);
                }
            }
            //sniping right - left
            else if (sceneBox.GetLeft() - sceneBox.BorderWidth + _snipingDistance > GetLeft() + GetWidth() + BorderWidth
                && sceneBox.GetLeft() - sceneBox.BorderWidth - _snipingDistance < GetLeft() + GetWidth() + BorderWidth)
            {
                if (!_snipingSceneBoxsRight.Contains(sceneBox)
                    && !(GetLeft() + GetWidth() + BorderWidth).Equals(sceneBox.GetLeft() + sceneBox.BorderWidth)
                    && _delayForSnipingHorizontal == 0)
                {
                    double newLeft = sceneBox.GetLeft() - sceneBox.BorderWidth - GetWidth() - BorderWidth;

                    if (setOnlyPosition)
                    {
                        SetLeft(newLeft);
                        _delayForSnipingHorizontal = 20;
                    }
                    else
                    {
                        SetWidth(sceneBox.GetLeft() - sceneBox.BorderWidth - GetLeft() - BorderWidth);
                        _delayForSnipingHorizontal = 40;
                    }

                    ShowSnipingVerticalLine(GetLeft() + GetWidth() + BorderWidth,
                        GetTop() > sceneBox.GetTop() ? sceneBox.GetTop() : GetTop(),
                        GetLeft() + GetWidth() + BorderWidth,
                        GetTop() > sceneBox.GetTop() ? GetTop() + GetHeight() : sceneBox.GetTop() + sceneBox.GetHeight());

                    _timerForVerticalLineHide.Enabled = true;
                    _useVerticalLine++;
                    _snipingSceneBoxsRight.Add(sceneBox);
                }
            }
            else
            {
                if (_snipingSceneBoxsRight.Remove(sceneBox))
                {
                    _useVerticalLine--;

                    if (_useVerticalLine <= 0)
                        HideHorizontalSnipingLine();
                }
            }
        }

        private void SnipingVertical(GraphicsSceneBox sceneBox, bool setOnlyPosition)
        {
            //sniping top - top
            if (sceneBox.GetTop() - sceneBox.BorderWidth + _snipingDistance > GetTop() - BorderWidth
                && sceneBox.GetTop() - sceneBox.BorderWidth - _snipingDistance < GetTop() - BorderWidth)
            {
                if (!_snipingSceneBoxsTop.Contains(sceneBox)
                    && !(GetTop() - BorderWidth).Equals(sceneBox.GetTop() - sceneBox.BorderWidth)
                    && _delayForSnipingVertical == 0)
                {
                    double newTop = sceneBox.GetTop() - sceneBox.BorderWidth + BorderWidth;

                    if (setOnlyPosition)
                    {
                        SetTop(newTop);
                        _delayForSnipingVertical = 20;
                    }
                    else
                    {
                        double oldBottomDistance = _canvas.ActualHeight - GetTop() - GetHeight();
                        SetTop(newTop);
                        SetHeight(_canvas.ActualHeight - GetTop() - oldBottomDistance);
                        _delayForSnipingVertical = 40;
                    }

                    ShowSnipingHorizontalLine(
                        GetLeft() >= sceneBox.GetLeft() ? sceneBox.GetLeft() : GetLeft(),
                        GetTop() - BorderWidth,
                        GetLeft() >= sceneBox.GetLeft() ? GetLeft() + GetWidth() : sceneBox.GetLeft() + sceneBox.GetWidth(),
                        GetTop() - BorderWidth);

                    _timerForHorizontalLineHide.Enabled = true;
                    _useHorizontalLine++;
                    _snipingSceneBoxsTop.Add(sceneBox);
                    return;
                }
            } //sniping top - bottom
            else if (sceneBox.GetTop() + sceneBox.BorderWidth + _snipingDistance + sceneBox.GetHeight() > GetTop() - BorderWidth
                && sceneBox.GetTop() + sceneBox.BorderWidth - _snipingDistance + sceneBox.GetHeight() < GetTop() - BorderWidth)
            {
                if (!_snipingSceneBoxsTop.Contains(sceneBox)
                    && !(GetTop() - BorderWidth).Equals(sceneBox.GetTop() + sceneBox.GetHeight() + sceneBox.BorderWidth)
                    && _delayForSnipingVertical == 0)
                {
                    double newTop = sceneBox.GetTop() + sceneBox.GetHeight() + sceneBox.BorderWidth + BorderWidth;

                    if (setOnlyPosition)
                    {
                        SetTop(newTop);
                        _delayForSnipingVertical = 20;
                    }
                    else
                    {
                        double oldBottomDistance = _canvas.ActualHeight - GetTop() - GetHeight();
                        SetTop(newTop);
                        SetHeight(_canvas.ActualHeight - GetTop() - oldBottomDistance);
                        _delayForSnipingVertical = 40;
                    }

                    ShowSnipingHorizontalLine(
                        GetLeft() >= sceneBox.GetLeft() ? sceneBox.GetLeft() : GetLeft(),
                        GetTop() - BorderWidth,
                        GetLeft() >= sceneBox.GetLeft() ? GetLeft() + GetWidth() : sceneBox.GetLeft() + sceneBox.GetWidth(),
                        GetTop() - BorderWidth);

                    _timerForHorizontalLineHide.Enabled = true;
                    _useHorizontalLine++;
                    _snipingSceneBoxsTop.Add(sceneBox);
                    return;
                }
            }
            else
            {
                if (_snipingSceneBoxsTop.Remove(sceneBox))
                {
                    _useHorizontalLine--;

                    if (_useHorizontalLine <= 0)
                        HideVerticalSnipingLine();
                }
            }

            //sniping bottom - bottom
            if (sceneBox.GetTop() + sceneBox.GetHeight() + sceneBox.BorderWidth + _snipingDistance > GetTop() + GetHeight() + BorderWidth
                && sceneBox.GetTop() + sceneBox.GetHeight() + sceneBox.BorderWidth - _snipingDistance < GetTop() + GetHeight() + BorderWidth)
            {
                if (!_snipingSceneBoxsBottom.Contains(sceneBox)
                    && !(GetTop() + GetHeight() + BorderWidth).Equals(sceneBox.GetTop() + sceneBox.GetHeight() + sceneBox.BorderWidth)
                    && _delayForSnipingVertical == 0)
                {
                    double newTop = sceneBox.GetTop() + sceneBox.GetHeight() + sceneBox.BorderWidth - GetHeight() -
                                    BorderWidth;

                    if (setOnlyPosition)
                    {
                        SetTop(newTop);
                        _delayForSnipingVertical = 20;
                    }
                    else
                    {
                        SetHeight(sceneBox.GetTop() + sceneBox.GetHeight() + sceneBox.BorderWidth - GetTop() - BorderWidth);
                        _delayForSnipingVertical = 40;
                    }

                    ShowSnipingHorizontalLine(
                        GetLeft() >= sceneBox.GetLeft() ? sceneBox.GetLeft() : GetLeft(),
                        GetTop() + GetHeight() + BorderWidth,
                        GetLeft() >= sceneBox.GetLeft() ? GetLeft() + GetWidth() : sceneBox.GetLeft() + sceneBox.GetWidth(),
                        GetTop() + GetHeight() + BorderWidth);

                    _timerForHorizontalLineHide.Enabled = true;
                    _useHorizontalLine++;
                    _snipingSceneBoxsBottom.Add(sceneBox);
                }
            }
            //sniping bottom - top
            else if (sceneBox.GetTop() - sceneBox.BorderWidth + _snipingDistance > GetTop() + GetHeight() + BorderWidth
                && sceneBox.GetTop() - sceneBox.BorderWidth - _snipingDistance < GetTop() + GetHeight() + BorderWidth)
            {
                if (!_snipingSceneBoxsBottom.Contains(sceneBox)
                    && !(GetTop() + GetHeight() + BorderWidth).Equals(sceneBox.GetTop() - sceneBox.BorderWidth)
                    && _delayForSnipingVertical == 0)
                {
                    double newTop = sceneBox.GetTop() - sceneBox.BorderWidth - GetHeight() - BorderWidth;

                    if (setOnlyPosition)
                    {
                        SetTop(newTop);
                        _delayForSnipingVertical = 20;
                    }
                    else
                    {
                        SetHeight(sceneBox.GetTop() - sceneBox.BorderWidth - GetTop() - BorderWidth);
                        _delayForSnipingVertical = 40;
                    }

                    ShowSnipingHorizontalLine(
                        GetLeft() >= sceneBox.GetLeft() ? sceneBox.GetLeft() : GetLeft(),
                        GetTop() + GetHeight() + BorderWidth,
                        GetLeft() >= sceneBox.GetLeft() ? GetLeft() + GetWidth() : sceneBox.GetLeft() + sceneBox.GetWidth(),
                        GetTop() + GetHeight() + BorderWidth);

                    _timerForHorizontalLineHide.Enabled = true;
                    _useHorizontalLine++;
                    _snipingSceneBoxsBottom.Add(sceneBox);
                }
            }
            else
            {
                if (_snipingSceneBoxsBottom.Remove(sceneBox))
                {
                    _useHorizontalLine--;

                    if (_useHorizontalLine <= 0)
                        HideVerticalSnipingLine();
                }
            }
        }

        public void MoveLeft(double left)
        {
            SetLeft(Canvas.GetLeft(this) + left);
            SetThumbsAndBorderPosition();
            SetRightDistance();
            Sniping(true);

            if (PositionChanged != null)
                PositionChanged(GetLeft(), GetTop());
        }

        public void MoveTop(double top)
        {
            SetTop(Canvas.GetTop(this) + top);
            SetThumbsAndBorderPosition();
            SetBottomDistance();
            Sniping(true);

            if (PositionChanged != null)
                PositionChanged(GetLeft(), GetTop());
        }

        public void SetLeft(double left)
        {
            if (_delayForSnipingHorizontal > 0)
            {
                _delayForSnipingHorizontal--;
                return;
            }

            //if (left - _borderWidth < 0)
            //    left = _borderWidth;

            //if (left + GetWidth() + _borderWidth > _canvas.ActualWidth)
            //    left = _canvas.ActualWidth - GetWidth() - _borderWidth;

            Canvas.SetLeft(this, left);
            SetThumbsAndBorderPosition();

            if (PositionChanged != null)
                PositionChanged(GetLeft(), GetTop());
        }

        public void SetTop(double top)
        {
            if (_delayForSnipingVertical > 0)
            {
                _delayForSnipingVertical--;
                return;
            }

            //if (top - _borderWidth < 0)
            //    top = _borderWidth;

            //if (top + GetHeight() + _borderWidth > _canvas.ActualHeight)
            //    top = _canvas.ActualHeight - GetHeight() - _borderWidth;

            Canvas.SetTop(this, top);
            SetThumbsAndBorderPosition();

            if (PositionChanged != null)
                PositionChanged(GetLeft(), GetTop());
        }

        public void SetWidth(double width)
        {
            if (_delayForSnipingHorizontal > 0)
            {
                _delayForSnipingHorizontal--;
                return;
            }

            //if (GetLeft() + width + _borderWidth > _canvas.ActualWidth
            //    && _canvas.ActualWidth > 0)
            //{
            //    width = _canvas.ActualWidth - GetLeft() - _borderWidth;
            //}

            Width = width > 0 ? width : 0;
            UpdateLayout();
            SetRightDistance();
            SetThumbsAndBorderPosition();
            PercentuallyWidth = GetWidth() / _canvas.ActualWidth;
            _textBlock.Width = GetWidth();
        }

        public void SetHeight(double height)
        {
            if (_delayForSnipingVertical > 0)
            {
                _delayForSnipingVertical--;
                return;
            }

            //if (GetTop() + height + _borderWidth > _canvas.ActualHeight
            //    && _canvas.ActualWidth > 0)
            //{
            //    height = _canvas.ActualHeight - GetTop() - _borderWidth;
            //}

            Height = height > 0 ? height : 0;
            SetBottomDistance();
            SetThumbsAndBorderPosition();
            PercentuallyHeight = GetHeight() / _canvas.ActualHeight;
        }

        public void SetLineWidth(double lineWidth)
        {
        }

        public void Select(bool resizing)
        {
            Select();
        }

        public void SetLayerID(Guid LayerID)
        {
        }

        public void SetZIndex(int zIndex)
        {
            ZIndex = zIndex;
            SetZIndex(this, zIndex);
        }

        public double GetLeft()
        {
            return Canvas.GetLeft(this);
        }

        public double GetTop()
        {
            return Canvas.GetTop(this);
        }

        public double GetWidth()
        {
            return Width;
        }

        public double GetHeight()
        {
            return Height;
        }

        public double GetLineWidth()
        {
            return 0;
        }

        public bool isSelected()
        {
            return _isSelected;
        }

        public Canvas GetCanvas()
        {
            return _canvas;
        }

        public Guid GetLayerID()
        {
            return Guid.Empty;
        }

        public int GetZIndex()
        {
            return ZIndex;
        }

        public List<GraphicsThumb> GetThumbs()
        {
            return Thumbs;
        }

        public double GetRotateAngle()
        {
            return 0;
        }

        public void RotateByAngle(double angle)
        {
        }

        public void ResetRotateAngle()
        {
        }

        public GraphicsObjectType GetGraphicsObjectType()
        {
            return GraphicsObjectType.GraphicsSceneBox;
        }

        public object Serialize()
        {
            return new SerializableSceneBox(this);
        }

        public void LockCursorChange()
        {
        }

        public void UnlockCursorChange()
        {
        }

        public event Action ObjectChanged;

        public void SetSceneId(Guid idScene)
        {
            _idScene = idScene;
        }

        public Guid GetSceneId()
        {
            return _idScene;
        }
    }

    [Serializable]
    public class SerializableSceneBox : SerializableObject, IGraphicsDeserializeObject
    {
        public Guid IdScene { get; set; }
        public string Name { get; set; }
        public GraphicsBoxHorizontalAlignment HAlignment { get; set; }
        public GraphicsBoxVerticalAlignment VAlignment { get; set; }
        public double RightDistance { get; set; }
        public double BottomDistance { get; set; }
        public double PercentuallyWidth { get; set; }
        public double PercentuallyHeight { get; set; }
        public bool EnablePercentuallyWidth { get; set; }
        public bool EnablePercentuallyHeight { get; set; }
        public string SceneLinkTarget { get; set; }
        public int BorderWidth { get; set; }
        public SerializableColor BorderColor { get; set; }

        public SerializableSceneBox(GraphicsSceneBox shape)
        {
            Serialize(shape);
            IdScene = shape.GetSceneId();
            Name = shape.BoxName;
            HAlignment = shape.HAlignment;
            VAlignment = shape.VAlignment;
            RightDistance = shape.RightDistance;
            BottomDistance = shape.BottomDistance;
            SceneLinkTarget = shape.SceneLinkTarget;
            PercentuallyWidth = shape.PercentuallyWidth;
            PercentuallyHeight = shape.PercentuallyHeight;
            EnablePercentuallyWidth = shape.EnablePercentuallyWidth;
            EnablePercentuallyHeight = shape.EnablePercentuallyHeight;
            BorderWidth = shape.BorderWidth;
            BorderColor = shape.BorderColor;
        }

        public GraphicsSceneBox GetGraphicsSceneBox(Canvas canvas)
        {
            var shape = new GraphicsSceneBox(canvas);
            Deserialize(shape);    
            shape.SetSceneId(IdScene);
            shape.BoxName = Name;
            shape.HAlignment = HAlignment;
            shape.VAlignment = VAlignment;
            shape.RightDistance = RightDistance;
            shape.BottomDistance = BottomDistance;
            shape.SceneLinkTarget = SceneLinkTarget;
            shape.PercentuallyWidth = PercentuallyWidth;
            shape.PercentuallyHeight = PercentuallyHeight;
            shape.EnablePercentuallyWidth = EnablePercentuallyWidth;
            shape.EnablePercentuallyHeight = EnablePercentuallyHeight;
            shape.BorderWidth = BorderWidth;
            shape.BorderColor = BorderColor;
            canvas.Children.Add(shape);
            return shape;
        }

        public IGraphicsObject Deserialize(Canvas canvas, Dictionary<string, SymbolParemeter> symbols)
        {
            return GetGraphicsSceneBox(canvas);
        }
    }

    [Serializable]
    public enum GraphicsBoxHorizontalAlignment : byte
    {
        Left = 0, 
        Right = 1,
        Stretch = 2,
    }

    [Serializable]
    public enum GraphicsBoxVerticalAlignment : byte
    {
        Top = 0,
        Bottom = 1,
        Stretch = 2,
    }
}
