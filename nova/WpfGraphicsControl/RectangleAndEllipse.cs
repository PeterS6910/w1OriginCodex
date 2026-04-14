using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Cgp.NCAS.WpfGraphicsControl;
using Cursor = System.Windows.Input.Cursor;
using Cursors = System.Windows.Input.Cursors;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

namespace Contal.Cgp.NCAS.WpfGraphicsControl
{
    [Serializable()]
    public enum DrawMode
    {
        rectangle, ellipse
    }

    public class RectangleAndEllipse : Shape, IGraphicsObject, ISceneLink
    {
        private readonly Canvas _canvas;
        private readonly DrawMode _drawMode;
        private bool _isSelected;
        private readonly Rectangle _border = new Rectangle();
        private Guid _layerID;
        private Cursor _requiredCursor = Cursors.Arrow;
        private bool _lockCursorChange;
        
        public int ZIndex { get; set; }
        public List<GraphicsThumb> Thumbs = new List<GraphicsThumb>();
        public byte[] BackgroundImage { get; set; }
        public List<Category> Categories { get; set; }
        public Guid IdScene { get; set; }

        public RectangleAndEllipse(Canvas canvas, DrawMode drawMode)
        {
            Canvas.SetZIndex(_border, 101);
            SetZIndex(0);
            Fill = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
            _canvas = canvas;
            _drawMode = drawMode;
            MouseEnter += RectangleAndEllipse_MouseEnter;
            MouseLeave += RectangleAndEllipse_MouseLeave;
        }

        void RectangleAndEllipse_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!_lockCursorChange)
                Cursor = Cursors.Arrow;

            _requiredCursor = Cursors.Arrow;
        }

        void RectangleAndEllipse_MouseEnter(object sender, MouseEventArgs e)
        {
            if (IdScene != Guid.Empty)
            {
                if (!_lockCursorChange)
                    Cursor = Cursors.Hand;

                _requiredCursor = Cursors.Hand;
            }
        }

        public Canvas GetCanvas()
        {
            return _canvas;
        }

        public DrawMode GetDrawMode()
        {
            return _drawMode;
        }

        public void Select(bool resizing)
        {
            if (!_isSelected)
            {
                _isSelected = true;

                Canvas.SetZIndex(this, 100);
                SetThumbsAndBorderPosition();
                _border.Stroke = Brushes.Blue;
                _border.StrokeThickness = 1 / _canvas.LayoutTransform.Value.M11;
                _canvas.Children.Add(_border);

                if (!resizing)
                    return;

                GraphicsOperations.CreateThumbsForRectange(_canvas, this, false, thumb_DragDelta);
            }
        }

        public void UnSelect()
        {
            if (_isSelected)
            {
                Canvas.SetZIndex(this, ZIndex);

                foreach (var thumb in Thumbs)
                {
                    _canvas.Children.Remove(thumb);
                }

                Thumbs.Clear();
                _canvas.Children.Remove(_border);
                _isSelected = false;
            }
        }

        void thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (ObjectChanged != null)
                ObjectChanged();

            GraphicsOperations.ComputeThumbOperations(_canvas, sender as GraphicsThumb, e);
            SetThumbsAndBorderPosition();
        }

        protected override Geometry DefiningGeometry
        {
            get
            {
                var rect = new Rect(new Point(0, 0),
                      new Point(Width - StrokeThickness, Height - StrokeThickness));

                Geometry geometry = new GeometryGroup();

                switch (_drawMode)
                {
                    case DrawMode.ellipse:
                        geometry = new EllipseGeometry(rect);
                        break;
                    case DrawMode.rectangle:
                        geometry = new RectangleGeometry(rect);
                        break;
                }

                SetThumbsAndBorderPosition();
                return geometry;
            }
        }

        public void MoveLeft(double left)
        {
            if (Canvas.GetLeft(this) + left < 0 || Canvas.GetLeft(this) + left + GetWidth() > _canvas.Width)
                return;

            Canvas.SetLeft(this, Canvas.GetLeft(this) + left);
            SetThumbsAndBorderPosition();
        }

        public void MoveTop(double top)
        {
            if (Canvas.GetTop(this) + top < 0 || Canvas.GetTop(this) + top + GetHeight() > _canvas.Height)
                return;

            Canvas.SetTop(this, Canvas.GetTop(this) + top);
            SetThumbsAndBorderPosition();
        }

        private void SetThumbsAndBorderPosition()
        {
            foreach (GraphicsThumb thumb in Thumbs)
            {
                thumb.SetPosition();
            }

            var rotateTransform = RenderTransform as RotateTransform;
            if (rotateTransform != null)
            {
                var borderTransform = new RotateTransform(rotateTransform.Angle);
                borderTransform.CenterX = rotateTransform.CenterX + StrokeThickness / 2;
                borderTransform.CenterY = rotateTransform.CenterY + StrokeThickness / 2;
                _border.RenderTransform = borderTransform;
            }

            //move _border
            Canvas.SetLeft(_border, GetLeft() - GetLineWidth() / 2);
            Canvas.SetTop(_border, GetTop() - GetLineWidth() / 2);
            _border.Width = Width;
            _border.Height = Height;
        }

        public void SetLeft(double left)
        {
            Canvas.SetLeft(this, left);

            foreach (var thumb in Thumbs)
            {
                thumb.SetPosition();
            }
        }

        public void SetTop(double top)
        {
            Canvas.SetTop(this, top);

            foreach (var thumb in Thumbs)
            {
                thumb.SetPosition();
            }
        }

        public void SetWidth(double width)
        {
            Width = width;
        }

        public void SetHeight(double height)
        {
            Height = height;
        }

        public void SetLineWidth(double lineWidth)
        {
            StrokeThickness = lineWidth;
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
            return ActualWidth;
        }

        public double GetHeight()
        {
            return ActualHeight;
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
            this.ZIndex = zIndex;
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
            RotateTransform transform = RenderTransform as RotateTransform;

            if (transform != null)
                return transform.Angle;
            
            return 0;
        }

        public void SetCategories(List<Category> categories)
        {
            Categories = categories;
        }

        public List<Category> GetCategories()
        {
            return Categories;
        }

        public void RotateByAngle(double angle)
        {
            double actualAngle = GetRotateAngle();
            double newAngle = actualAngle + angle;

            if (newAngle < 0)
                newAngle = 360 - Math.Abs(newAngle);

            if (newAngle >= 360)
                newAngle = newAngle - 360;

            var newRotateTransform = new RotateTransform(newAngle, GetWidth() / 2, GetHeight() / 2);
            RenderTransform = newRotateTransform;
            UnSelect();
            Select(true);
        }

        public void ResetRotateAngle()
        {
            var newRotateTransform = new RotateTransform(0, GetWidth() / 2, GetHeight() / 2);
            RenderTransform = newRotateTransform;
            UnSelect();
            Select(true);
        }

        public GraphicsObjectType GetGraphicsObjectType()
        {
            return GraphicsObjectType.RectangleAndEllipse;
        }

        public object Serialize()
        {
            return new SerializableRectangleAndEllipse(this);
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
    public class SerializableRectangleAndEllipse : SerializableObject, IGraphicsDeserializeObject
    {
        public DrawMode drawMode { get; set; }
        public double lineWidth { get; set; }
        public SerializableColor color { get; set; }
        public SerializableColor lineColor { get; set; }
        public byte[] BackgroundImage { get; set; }
        public Guid IdScene { get; set; }

        public SerializableRectangleAndEllipse(RectangleAndEllipse shape)
        {
            Serialize(shape);
            drawMode = shape.GetDrawMode();
            lineWidth = shape.StrokeThickness;
            IdScene = shape.IdScene;

            if ((shape.Fill as SolidColorBrush) != null)
                color = new SerializableColor((shape.Fill as SolidColorBrush).Color);
            else
                color = new SerializableColor(Color.FromArgb(0, 0, 0, 0));

            lineColor = new SerializableColor((shape.Stroke as SolidColorBrush).Color);

            if (shape.BackgroundImage != null)
                BackgroundImage = shape.BackgroundImage;
        }

        public RectangleAndEllipse GetGraphicsRectangleAndEllipse(Canvas canvas)
        {
            var shape = new RectangleAndEllipse(canvas, drawMode);
            Deserialize(shape);
            shape.Stroke = new SolidColorBrush(lineColor.GetColor());
            shape.Fill = new SolidColorBrush(color.GetColor());
            shape.StrokeThickness = lineWidth;
            shape.BackgroundImage = BackgroundImage;
            shape.IdScene = IdScene;

            if (BackgroundImage != null)
            {
                try
                {
                    var src = new BitmapImage();
                    src.BeginInit();
                    src.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                    src.CacheOption = BitmapCacheOption.OnLoad;
                    src.StreamSource = new MemoryStream(BackgroundImage);
                    src.EndInit();

                    var brush = new ImageBrush(src);
                    shape.Fill = brush;
                }
                catch
                {
                }  
            }

            canvas.Children.Add(shape);
            return shape;
        }

        public IGraphicsObject Deserialize(Canvas canvas, Dictionary<string, SymbolParemeter> symbols)
        {
            return GetGraphicsRectangleAndEllipse(canvas);
        }
    }
}
