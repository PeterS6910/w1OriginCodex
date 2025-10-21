using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Contal.Cgp.NCAS.WpfGraphicsControl
{
    public class GraphicsThumb : Shape
    {
        private Point _originThumbPoint;
        private bool _isDragging;

        public Point FixedPoint;
        public Point RotatePoint;
        public int Count { private set; get; }
        public static bool DeleteMode { get; set; }
        public event DragDeltaEventHandler DragDelta;
        public delegate void ThumbClickDelegate(GraphicsThumb sender, int count);
        public static event ThumbClickDelegate ThumbClick;

        public GraphicsThumb(object shape, int x)
        {
            Panel.SetZIndex(this, 102);
            Fill = Brushes.Red;
            Stroke = Brushes.Black;
            DataContext = shape;
            MouseEnter += myThumb_MouseEnter;
            MouseLeave += myThumb_MouseLeave;
            Count = x;
            SetSize();
            RotatePoint = new Point(((IGraphicsObject) DataContext).GetWidth() / 2, 
                ((IGraphicsObject) DataContext).GetHeight() / 2);
            SetCursor();

            if (shape is GraphicsPolygon)
                Cursor = Cursors.SizeAll;

            MouseDown += myThumb_DragStarted;
            MouseMove += myThumb_MouseMove;
            MouseUp += myThumb_MouseUp;
            SetPosition();
        }

        private void SetCursor()
        {
            if (DeleteMode)
            {
                Cursor = Cursors.No;
            }
            else
            {
                if (DataContext is GraphicsPolygon)
                {
                    Cursor = Cursors.SizeAll;
                    return;
                }

                switch (Count)
                {
                    case 1:
                        Cursor = Cursors.SizeNWSE;
                        break;
                    case 2:
                        Cursor = Cursors.SizeNESW;
                        break;
                    case 3:
                        Cursor = Cursors.SizeNWSE;
                        break;
                    case 4:
                        Cursor = Cursors.SizeNESW;
                        break;
                    case 5:
                        Cursor = Cursors.SizeAll;
                        break;
                }
            }
        }

        void myThumb_MouseLeave(object sender, MouseEventArgs e)
        {
            Fill = Brushes.Red;
        }

        void myThumb_MouseEnter(object sender, MouseEventArgs e)
        {
            SetCursor();
            Fill = Brushes.Blue;
        }

        void myThumb_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging)
            {
                var item = (UIElement) DataContext;
                var rotateTransform = item.RenderTransform as RotateTransform;
                var canvas = ((IGraphicsObject) DataContext).GetCanvas();
                var mousePoint = e.GetPosition(canvas);

                if (rotateTransform != null && Count != 5)
                {
                    var centerPoint = new Point
                    {
                        X = Canvas.GetLeft(item) + rotateTransform.CenterX,
                        Y = Canvas.GetTop(item) + rotateTransform.CenterY
                    };

                    var newRotateTransform = new RotateTransform(-rotateTransform.Angle, centerPoint.X, centerPoint.Y);
                    mousePoint = newRotateTransform.Transform(mousePoint);
                }

                var delta = new DragDeltaEventArgs(mousePoint.X - (_originThumbPoint.X + Canvas.GetLeft(this)),
                    mousePoint.Y - (_originThumbPoint.Y + Canvas.GetTop(this)));
                DragDelta(this, delta);
            }
        }

        void myThumb_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (ThumbClick != null)
            {
                ThumbClick(this, Count);
            }

            _isDragging = false;
            ReleaseMouseCapture();

            //computed new position for center point which is in center of shape
            var shape = (IGraphicsObject) DataContext;
            var transform = ((UIElement) DataContext).RenderTransform as RotateTransform;

            if (transform != null)
            {
                var newCenterPoint = new Point(shape.GetLeft() + shape.GetWidth() / 2 - shape.GetLineWidth() / 2,
                    shape.GetTop() + shape.GetHeight() / 2 - shape.GetLineWidth() / 2);

                var newTransform = new RotateTransform(transform.Angle, transform.CenterX + shape.GetLeft(),
                    transform.CenterY + shape.GetTop());

                newCenterPoint = newTransform.Transform(newCenterPoint);
                shape.SetLeft(newCenterPoint.X - shape.GetWidth() / 2 + shape.GetLineWidth() / 2);
                shape.SetTop(newCenterPoint.Y - shape.GetHeight() / 2 + shape.GetLineWidth() / 2);
                transform.CenterX = shape.GetWidth() / 2 - shape.GetLineWidth() / 2;
                transform.CenterY = shape.GetHeight() / 2 - shape.GetLineWidth() / 2;
                shape.UnSelect();
                shape.Select(true);
            }
        }

        protected override Geometry DefiningGeometry
        {
            get
            {
                var mainRect = new Rect(new Point(0, 0), new Size(Width, Height));
                Geometry geometry;

                if (Count == 5 && (DataContext as GraphicsPolygon) == null)
                {   
                    geometry = new EllipseGeometry(mainRect);                    
                }
                else
                {
                    geometry = new RectangleGeometry(mainRect);
                }

                return geometry;
            }
        }

        public void SetSize()
        {
            object shape = DataContext;
            double zoom = ((IGraphicsObject) shape).GetCanvas().LayoutTransform.Value.M11;
            Height = Width = 10.0 / zoom;
            StrokeThickness = Height/3;
            StrokeThickness = 0;
        }

        public void SetPosition()
        {
            var item = (IGraphicsObject) DataContext;

            if (item.GetGraphicsObjectType() == GraphicsObjectType.Text
                || item.GetGraphicsObjectType() == GraphicsObjectType.GraphicsSceneBox
                || item.GetGraphicsObjectType() == GraphicsObjectType.RectangleAndEllipse
                || item.GetGraphicsObjectType() == GraphicsObjectType.Symbol)
            {
                var rotateTransform = ((FrameworkElement) item).RenderTransform as RotateTransform;
                var newRotateTransform = new RotateTransform();

                if (rotateTransform != null
                    && rotateTransform.Angle != 0)
                {
                    newRotateTransform.Angle = rotateTransform.Angle;
                    newRotateTransform.CenterX = rotateTransform.CenterX + item.GetLineWidth() / 2;
                    newRotateTransform.CenterY = rotateTransform.CenterY + item.GetLineWidth() / 2;
                }

                switch (Count)
                {
                    case 1:
                        Canvas.SetLeft(this, item.GetLeft() - item.GetLineWidth() / 2);
                        Canvas.SetTop(this, item.GetTop() - item.GetLineWidth() / 2);
                        
                        if (rotateTransform == null)
                            break;

                        RenderTransform = newRotateTransform;
                        break;
                    case 2:
                        Canvas.SetLeft(this, item.GetLeft() + item.GetWidth() - Width - item.GetLineWidth() / 2);
                        Canvas.SetTop(this, item.GetTop() - item.GetLineWidth() / 2);
                        
                        if (rotateTransform == null)
                            break;

                        newRotateTransform.CenterX = -(item.GetWidth() - newRotateTransform.CenterX) + Width;
                        newRotateTransform.CenterY = newRotateTransform.CenterY;
                        break;
                    case 3:
                        Canvas.SetLeft(this, item.GetLeft() + item.GetWidth() - Width - item.GetLineWidth() / 2);
                        Canvas.SetTop(this, item.GetTop() + item.GetHeight() - Height - item.GetLineWidth() / 2);
                        
                        if (rotateTransform == null)
                            break;

                        newRotateTransform.CenterX = -(item.GetWidth() - newRotateTransform.CenterX) + Width;
                        newRotateTransform.CenterY = -(item.GetHeight() - newRotateTransform.CenterY) + Height;
                        break;
                    case 4:
                        Canvas.SetLeft(this, item.GetLeft() - item.GetLineWidth() / 2);
                        Canvas.SetTop(this, item.GetTop() + item.GetHeight() - Height - item.GetLineWidth() / 2);
                        
                        if (rotateTransform == null)
                            break;

                        newRotateTransform.CenterX = newRotateTransform.CenterX;
                        newRotateTransform.CenterY = -(item.GetHeight() - newRotateTransform.CenterY) + Height;
                        break;
                    case 5:
                        Canvas.SetLeft(this, item.GetLeft() + item.GetWidth() / 2 - item.GetLineWidth() / 2 - Width / 2);
                        Canvas.SetTop(this, item.GetTop() - item.GetHeight() * 0.1 - 2*Height - item.GetLineWidth() / 2);
                        
                        if (rotateTransform == null)
                            break;

                        newRotateTransform.CenterX += - item.GetWidth() / 2 + Width / 2;
                        newRotateTransform.CenterY += item.GetTop() - Canvas.GetTop(this) - item.GetLineWidth() / 2;
                        break;
                }

                if (Count != 1)
                    RenderTransform = newRotateTransform;
            }
            else
            {
                var polygon = item as GraphicsPolygon;

                if (polygon != null)
                {
                    Canvas.SetLeft(this, polygon.Points[Count].X - Width / 2 + item.GetLeft());
                    Canvas.SetTop(this, polygon.Points[Count].Y - Height / 2 + item.GetTop());
                }
            }
        }

        void myThumb_DragStarted(object sender, MouseButtonEventArgs e)
        {
            if (e.MouseDevice.LeftButton == MouseButtonState.Pressed)
            {
                CaptureMouse();
                _isDragging = true;
                _originThumbPoint = new Point(e.GetPosition(this).X, e.GetPosition(this).Y);
                var item = (IGraphicsObject)DataContext;
                var transform = ((UIElement) item).RenderTransform as RotateTransform;
                double sx = 0, sy = 0;

                if (transform != null)
                {
                    sx = transform.CenterX;
                    sy = transform.CenterY;
                }

                RotatePoint = new Point(sx, sy);

                switch (Count)
                {
                    case 1:
                        FixedPoint = new Point(item.GetLeft() + item.GetWidth(), item.GetTop() + item.GetHeight());
                        break;
                    case 2:
                        FixedPoint = new Point(item.GetLeft(), item.GetTop() + item.GetHeight());
                        break;
                    case 3:
                        FixedPoint = new Point(item.GetLeft(), item.GetTop());
                        break;
                    case 4:
                        FixedPoint = new Point(item.GetLeft() + item.GetWidth(), item.GetTop());
                        break;
                    case 5:
                        FixedPoint = new Point(item.GetLeft() + item.GetWidth() / 2 - item.GetLineWidth() / 2,
                            item.GetTop() + item.GetHeight() / 2 - item.GetLineWidth() / 2); //center point of element
                        break;
                }
            }
        }
    }
}
