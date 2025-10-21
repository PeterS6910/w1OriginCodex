using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;

using Cgp.NCAS.WpfGraphicsControl;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Server.Beans;

namespace Contal.Cgp.NCAS.WpfGraphicsControl
{
    public class GraphicsPolygon : Shape, IGraphicsObject, ILiveObject, ISceneLink
    {
        private readonly Canvas _canvas;
        private readonly PolygonMode _polygonMode;
        private bool _isSelected;
        private Guid _layerID;

        // Create a storyboard to contain the animations.
        private readonly Storyboard _storyboard = new Storyboard();
        private readonly TimeSpan _duration = new TimeSpan(0, 0, 0, 0, 500);
        private string _state;
        private bool _isEnable = true;
        private ActivationState _lastActivationState;
        private Text _label;
        private Cursor _requiredCursor = Cursors.Arrow;
        private bool _lockCursorChange;

        public List<Point> Points = new List<Point>(); 
        public List<GraphicsThumb> Thumbs = new List<GraphicsThumb>();
        public bool IsClosed { get; set; }
        public int ZIndex { get; set; }
        public Guid AlarmAreaGuid { get; set; }
        public List<Category> Categories { get; set; }
        public Guid IdScene { get; set; }

        public GraphicsPolygon(Canvas canvas, PolygonMode polygonMode)
        {
            _canvas = canvas;
            _polygonMode = polygonMode;
            IsClosed = false;

            if (_polygonMode == PolygonMode.AlarmArea)
                SetZIndex(-1);

            if (_polygonMode == PolygonMode.AlarmArea)
                Fill = new SolidColorBrush(Color.FromArgb(70, 100, 100, 100));

            StrokeThickness = 0;
            Stroke = new SolidColorBrush(Colors.Black);

            //configure animation
            var animation = new DoubleAnimation
            {
                From = 0.2,
                To = 1.0,
                AutoReverse = true,
                Duration = new Duration(_duration),
                RepeatBehavior = RepeatBehavior.Forever
            };

            Storyboard.SetTargetProperty(animation, new PropertyPath(OpacityProperty));
            _storyboard.Children.Add(animation);

            LayoutUpdated += GraphicsAlarmArea_LayoutUpdated;
            MouseEnter += GraphicsPolygon_MouseEnter;
            MouseLeave += GraphicsPolygon_MouseLeave;
        }

        void GraphicsPolygon_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!_lockCursorChange)
                Cursor = Cursors.Arrow;

            _requiredCursor = Cursors.Arrow;
        }

        void GraphicsPolygon_MouseEnter(object sender, MouseEventArgs e)
        {
            if (IdScene != Guid.Empty)
            {
                if (!_lockCursorChange)
                    Cursor = Cursors.Hand;

                _requiredCursor = Cursors.Hand;
            }
        }

        void GraphicsAlarmArea_LayoutUpdated(object sender, EventArgs e)
        {
            if (StrokeThickness > 0
                && _polygonMode == PolygonMode.AlarmArea)
            {
                StrokeThickness = 1/_canvas.LayoutTransform.Value.M11;
            }
        }

        public PolygonMode GetPolygonMode()
        {
            return _polygonMode;
        }

        public bool Enable
        {
            get { return _isEnable; }
            set
            {
                _isEnable = value;
                Fill = new SolidColorBrush(Color.FromArgb(70, 100, 100, 100));
                _state = ActivationState.Unknown.ToString();
            }
        }

        public void Refresh()
        {
            InvalidateVisual();
        }

        public Canvas GetCanvas()
        {
            return _canvas;
        }

        public void Select(bool resizing)
        {
            if (!Enable)
                return;

            if (!_isSelected)
            {
                _isSelected = true;

                if (_polygonMode == PolygonMode.AlarmArea)
                    StrokeThickness = 1 / _canvas.LayoutTransform.Value.M11;

                if (!resizing)
                    return;

                for (int i=0; i < Points.Count; i++)
                {
                    var thumb = new GraphicsThumb(this, i);
                    thumb.DataContext = this;
                    thumb.DragDelta += thumb_DragDelta;
                    Thumbs.Add(thumb);
                    _canvas.Children.Add(thumb);
                }

                InvalidateVisual();
            }
        }

        public void UnSelect()
        {
            if (_isSelected)
            {
                if (_polygonMode == PolygonMode.AlarmArea)
                    StrokeThickness = 0;

                foreach (GraphicsThumb thumb in Thumbs)
                {
                    _canvas.Children.Remove(thumb);
                }

                Thumbs.Clear();
                _isSelected = false;
                InvalidateVisual();
            }
        }

        void thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (ObjectChanged != null)
                ObjectChanged();

            var thumb = (GraphicsThumb) sender;

            if (Points[thumb.Count].X + e.HorizontalChange + GetLeft() < 0 
                || Points[thumb.Count].X + e.HorizontalChange + GetLeft() > _canvas.Width
                || Points[thumb.Count].Y + e.VerticalChange + GetTop() < 0 
                || Points[thumb.Count].Y + e.VerticalChange + GetTop() > _canvas.Height)
                return;

            Points[thumb.Count] = new Point(Points[thumb.Count].X + e.HorizontalChange, Points[thumb.Count].Y + e.VerticalChange);
            InvalidateVisual();
        }

        protected override Geometry DefiningGeometry
        {
            get
            {
                var pathFigure = new PathFigure();
                int count = Points.Count - 1;
                
                if (IsClosed)
                    count++;

                var array = new Point[count];
                pathFigure.StartPoint = Points[0];

                for (int i = 1; i < Points.Count; i++)
                {
                    array[i - 1] = Points[i];
                }

                if (IsClosed)
                    array[Points.Count - 1] = Points[0];
                
                pathFigure.Segments.Add(new PolyLineSegment(array, true));

                var geometry = new PathGeometry();
                geometry.Figures.Add(pathFigure);

                foreach (var thumb in Thumbs)
                {
                    thumb.SetPosition();
                }

                return geometry;
            }
        }

        private double? IsCorrectlyTriangle(Point newPoint, int posA, int posB)
        {
            double distanceAB = GetDistance(Points[posA], Points[posB]);
            double distanceAnewPoint = GetDistance(Points[posA], newPoint);
            double distanceBnewPoint = GetDistance(newPoint, Points[posB]);

            double cos = (-Math.Pow(distanceAnewPoint, 2) + Math.Pow(distanceBnewPoint, 2) + Math.Pow(distanceAB, 2)) /
                             (2 * distanceBnewPoint * distanceAB);

            if (Math.Acos(cos) > Math.PI/2)
                return null;

            double x = distanceBnewPoint * cos;

            if (x > distanceAB)
                return null;

            double distance = Math.Pow(Math.Pow(distanceBnewPoint, 2) - Math.Pow(x, 2), 0.5);

            if (distance > 1/_canvas.LayoutTransform.Value.M11*5)
                return null;

            return distance;
        }

        public int AddNewPoint(Point newPoint)
        {
            int insertPos = -1;
            double smallestDistance = -1;

            for (int i = 0; i < Points.Count; i++)
            {
                int j = i + 1;

                if (j == Points.Count)
                    j = 0;

                double? distance = IsCorrectlyTriangle(newPoint, i, j);

                if (distance != null)
                {
                    if (smallestDistance == -1)
                    {
                        insertPos = j;
                        smallestDistance = distance.Value;
                    }
                    else if (smallestDistance > distance)
                    {
                        insertPos = j;
                        smallestDistance = distance.Value;
                    }
                }
            }

            if (smallestDistance != -1)
                Points.Insert(insertPos, newPoint);

            return insertPos;
        }

        private double GetDistance(Point a, Point b)
        {
            return Math.Pow(Math.Pow(Math.Abs(a.X - b.X), 2) + Math.Pow(Math.Abs(a.Y - b.Y), 2), 0.5);
        }

        private bool IsPolygonOutCanvas(double deltaLeft, double deltaTop)
        {
            foreach (var point in Points)
            {
                if (point.X + deltaLeft + GetLeft() < 0 ||
                    point.X + deltaLeft + GetLeft() > _canvas.Width
                    || point.Y + deltaTop + GetTop() < 0 ||
                    point.Y + deltaTop + GetTop() > _canvas.Height)
                {
                    return true;
                }
            }
            return false;
        }

        public void ChangeState(byte state)
        {
            if (!_isEnable
                || _polygonMode != PolygonMode.AlarmArea)
                return;

            var alarmAreaState = (ActivationState) state;
            _lastActivationState = alarmAreaState;

            Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                _state = alarmAreaState.ToString();

                switch (alarmAreaState)
                {
                    case ActivationState.Unset:
                        Fill = new SolidColorBrush(Color.FromArgb(70, 0, 255, 0));
                        StartAnimation(false);
                        break;

                    case ActivationState.Set:
                        Fill = new SolidColorBrush(Color.FromArgb(70, 255, 0, 0));
                        StartAnimation(false);
                        break;

                    case ActivationState.Prewarning:
                        Fill = new SolidColorBrush(Color.FromArgb(70, 0, 255, 0));
                        StartAnimation(true);
                        break;

                    default:
                        Fill = new SolidColorBrush(Color.FromArgb(70, 100, 100, 100));
                        StartAnimation(false);
                        break;
                }
            }));
        }

        public void ChangeAlarmState(AlarmAreaAlarmState state)
        {
            if (!_isEnable)
                return;

            Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                _state = state.ToString();

                switch (state)
                {
                    case AlarmAreaAlarmState.Alarm:
                        Fill = new SolidColorBrush(Color.FromArgb(125, 255, 0, 0));
                        StartAnimation(true);
                        break;

                    default:
                        ChangeState((byte)_lastActivationState);
                        break;
                }
            }));
        }

        public double GetMinimumTopPoint()
        {
            double minTop = Points[0].Y;

            foreach (Point point in Points)
            {
                if (minTop > point.Y)
                    minTop = point.Y;
            }

            return minTop;
        }

        public double GetMaximumTopPoint()
        {
            double minTop = Points[0].Y;

            foreach (Point point in Points)
            {
                if (minTop < point.Y)
                    minTop = point.Y;
            }

            return minTop;
        }

        public double GetMinimumLeftPoint()
        {
            double minLeft = Points[0].X;

            foreach (Point point in Points)
            {
                if (minLeft > point.X)
                    minLeft = point.X;
            }

            return minLeft;
        }

        public double GetMaximumLeftPoint()
        {
            double minLeft = Points[0].X;

            foreach (Point point in Points)
            {
                if (minLeft < point.X)
                    minLeft = point.X;
            }

            return minLeft;
        }

        public void StartAnimation(bool start)
        {
            if (start)
                _storyboard.Begin(this, true);
            else
                _storyboard.Stop(this);
        }

        public void MoveLeft(double left)
        {
            if (IsPolygonOutCanvas(left, 0))
                return;

            Canvas.SetLeft(this, Canvas.GetLeft(this) + left);
            
            if (_label != null)
                _label.MoveLeft(left);

            foreach (GraphicsThumb thumb in Thumbs)
            {
                thumb.SetPosition();
            }
        }

        public void MoveTop(double top)
        {
            if (IsPolygonOutCanvas(0, top))
                return;

            Canvas.SetTop(this, Canvas.GetTop(this) + top);
            
            if (_label != null)
                _label.MoveTop(top);

            foreach (GraphicsThumb thumb in Thumbs)
            {
                thumb.SetPosition();
            }
        }

        public void SetLeft(double left)
        {
            Canvas.SetLeft(this, 0);

            foreach (GraphicsThumb thumb in Thumbs)
            {
                thumb.SetPosition();
            }
        }

        public void SetTop(double top)
        {
            Canvas.SetTop(this, 0);

            foreach (GraphicsThumb thumb in Thumbs)
            {
                thumb.SetPosition();
            }
        }

        public void SetWidth(double width)
        {
            //this.Width = width;
        }

        public void SetHeight(double height)
        {
            //this.Height = height;
        }

        public void SetLineWidth(double lineWidth)
        {
            this.StrokeThickness = lineWidth;
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
            return GetMaximumLeftPoint() - GetMinimumLeftPoint();
        }

        public double GetHeight()
        {
            return GetMaximumTopPoint() - GetMinimumTopPoint();
        }

        public double GetLineWidth()
        {
            return StrokeThickness;
        }

        public bool isSelected()
        {
            return _isSelected;
        }

        public Guid GetLayerID()
        {
            return _layerID;
        }

        public void SetLayerID(Guid LayerID)
        {
            _layerID = LayerID;
        }

        public void SetZIndex(int zIndex)
        {
            ZIndex = zIndex;
            Canvas.SetZIndex(this, zIndex);
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

        public void SetObjectGuid(Guid id)
        {
            AlarmAreaGuid = id;
        }

        public Guid GetObjectGuid()
        {
            return AlarmAreaGuid;
        }

        public void SetCategories(List<Category> categories)
        {
            Categories = categories;
        }

        public ICollection<Category> GetCategories()
        {
            return new HashSet<Category>(Categories);
        }

        public void ShowInfo()
        {
        }

        public object GetState()
        {
            return _state;
        }

        public string GetObjectTypeName()
        {
            return "Alarm Area";
        }

        public ObjectType GetObjectType()
        {
            return ObjectType.AlarmArea;
        }

        public bool isEnable()
        {
            return Enable;
        }

        void ILiveObject.Enable(bool enable)
        {
            Enable = enable;
        }

        public void SetLabel(Text label)
        {
            _label = label;
        }

        public Text GetLabel()
        {
            return _label;
        }

        public void RotateByAngle(double angle)
        {
        }

        public void ResetRotateAngle()
        {
        }

        public GraphicsObjectType GetGraphicsObjectType()
        {
            switch (_polygonMode)
            {
                case PolygonMode.Line:
                    return GraphicsObjectType.Line;

                case PolygonMode.PolyLine:
                    return GraphicsObjectType.Polyline;

                default:
                    return GraphicsObjectType.AlarmArea;
            }
        }

        public object Serialize()
        {
            return new SerializablePolygon(this);
        }

        public void LockCursorChange()
        {
            _lockCursorChange = true;
        }

        public void UnlockCursorChange()
        {
            _lockCursorChange = false;
            Cursor = _requiredCursor;
        }

        public event Action ObjectChanged;

        public void SetSceneId(Guid idScene)
        {
            IdScene = idScene;
        }

        public Guid GetSceneId()
        {
            return IdScene;
        }
    }

    [Serializable()]
    public enum PolygonMode : byte
    {
        AlarmArea = 0,
        Line = 1,
        PolyLine = 2 
    }

    [Serializable()]
    public class SerializablePolygon : IGraphicsDeserializeObject
    {
        public PolygonMode PolygonMode { get; set; }
        public Guid LayerID { get; set; }
        public List<Point> Points { get; set; }
        public bool IsClosedPolyline { get; set; }
        public double PosX { get; set; }
        public double PosY { get; set; }
        public double LineWidth { get; set; }
        public SerializableColor LineColor { get; set; }
        public SerializableColor Color { get; set; }
        public Guid AlarmAreaGuid { get; set; }
        public List<Category> Categories { get; set; }
        public SerializableText Label { get; set; }
        public Guid IdScene { get; set; }

        public SerializablePolygon(GraphicsPolygon shape)
        {
            PolygonMode = shape.GetPolygonMode();
            LayerID = (shape as IGraphicsObject).GetLayerID();
            PosX = Canvas.GetLeft(shape);
            PosY = Canvas.GetTop(shape);
            IdScene = shape.IdScene;

            if (PolygonMode != PolygonMode.AlarmArea)
            {
                LineWidth = shape.StrokeThickness;
                LineColor = new SerializableColor((shape.Stroke as SolidColorBrush).Color);
                IsClosedPolyline = shape.IsClosed;
            }

            Points = shape.Points;
            var brush = shape.Fill as SolidColorBrush;
            
            if (brush != null)
                Color = new SerializableColor(brush.Color);

            if (PolygonMode == PolygonMode.AlarmArea)
                AlarmAreaGuid = shape.AlarmAreaGuid;

            Categories = (shape as ILiveObject).GetCategories().ToList();

            if (shape.GetLabel() != null)
                Label = new SerializableText(shape.GetLabel());
        }

        public GraphicsPolygon GetGraphicsPolygon(Canvas canvas)
        {
            if (Points == null 
                || Points.Count < 2)
                return null;

            if (PolygonMode == PolygonMode.AlarmArea
                && Points.Count < 3)
                return null;

            var shape = new GraphicsPolygon(canvas, PolygonMode);
            (shape as IGraphicsObject).SetLayerID(LayerID);
            
            shape.Points = Points;
            shape.IdScene = IdScene;

            if (PolygonMode != PolygonMode.AlarmArea)
            {
                shape.Stroke = new SolidColorBrush(LineColor.GetColor());
                shape.StrokeThickness = LineWidth;
                shape.IsClosed = IsClosedPolyline;
            }

            if (Color != null)
                shape.Fill = new SolidColorBrush(Color.GetColor());

            if (PolygonMode == PolygonMode.AlarmArea)
            {
                shape.AlarmAreaGuid = AlarmAreaGuid;
                shape.IsClosed = true;
            }
            Canvas.SetLeft(shape, PosX);
            Canvas.SetTop(shape, PosY);
            (shape as ILiveObject).SetCategories(Categories);
            canvas.Children.Add(shape);

            if (Label != null)
            {
                Text text = Label.GetText(canvas);
                shape.SetLabel(text);
                text.LiveObject = shape;
            }

            return shape;
        }

        public IGraphicsObject Deserialize(Canvas canvas, Dictionary<string, SymbolParemeter> symbols)
        {
            return GetGraphicsPolygon(canvas);
        }
    }
}
