using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Cgp.NCAS.WpfGraphicsControl;

namespace Contal.Cgp.NCAS.WpfGraphicsControl
{
    public class GraphicsSymbol : Image, IGraphicsObject
    {
        private readonly Canvas _canvas;
        private readonly Rectangle _border = new Rectangle();
        private bool _isSelected;
        private Guid _layerID;
        private DefaultParameters _defaultParameters;
        private int _borderOffset;
        
        public Text Label;
        public int zIndex { get; set; }
        public List<GraphicsThumb> Thumbs = new List<GraphicsThumb>();
        public List<Category> Categories { get; set; }

        public GraphicsSymbol(Canvas canvas)
        {
            Canvas.SetZIndex(_border, 99);
            SetZIndex(0);
            _canvas = canvas;
            Width = 0;
            Height = 0;
        }

        public DefaultParameters GetDefaultParameters()
        {
            return _defaultParameters;
        }

        public void LoadDefaulParameters()
        {
            _defaultParameters = new DefaultParameters(GetLeft(), GetTop(), GetWidth(), GetHeight());
        }

        public Canvas GetCanvas()
        {
            return _canvas;
        }

        public void Select(bool resizing)
        {
            if (!_isSelected)
            {
                _isSelected = true;
                Canvas.SetZIndex(this, 100);
                _border.Stroke = Brushes.Blue;
                _border.StrokeThickness = 1/_canvas.LayoutTransform.Value.M11;
                
                if (!resizing)
                    _borderOffset = 4;

                SetThumbsAndBorderPosition();
                _canvas.Children.Add(_border);

                if (!resizing)
                {
                    return;
                }

                GraphicsOperations.CreateThumbsForRectange(_canvas, this, true, thumb_DragDelta);
            }
        }

        public void UnSelect()
        {
            if (_isSelected)
            {
                Canvas.SetZIndex(this, zIndex);

                foreach (GraphicsThumb thumb in Thumbs)
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

            double oldAngle = GetRotateAngle();
            GraphicsOperations.ComputeThumbOperations(_canvas, sender as GraphicsThumb, e);
            double deltaAngle = GetRotateAngle() - oldAngle;
            RotateLabel(deltaAngle, oldAngle);
            SetThumbsAndBorderPosition();
        }

        private void RotateLabel(double deltaAngle, double oldAngle)
        {
            if (Label != null)
            {
                Text text = Label;
                double textAngle = text.GetRotateAngle() - oldAngle;

                var textRotateTransform = new RotateTransform();
                textRotateTransform.CenterX = GetLeft() + GetWidth() / 2 - text.GetLeft();
                textRotateTransform.CenterY = GetTop() + GetHeight() / 2 - text.GetTop();
                textRotateTransform.Angle = deltaAngle;
                text.RenderTransform = textRotateTransform;
                var newCenterPoint = new Point(text.GetLeft() + text.GetWidth() / 2, text.GetTop() + text.GetHeight() / 2);

                var newTransform = new RotateTransform(deltaAngle, textRotateTransform.CenterX + text.GetLeft(),
                    textRotateTransform.CenterY + text.GetTop());
                newCenterPoint = newTransform.Transform(newCenterPoint);
                textRotateTransform.Angle = GetRotateAngle() + textAngle;
                text.SetLeft(newCenterPoint.X - text.GetWidth() / 2);
                text.SetTop(newCenterPoint.Y - text.GetHeight() / 2);
                textRotateTransform.CenterX = text.GetWidth() / 2;
                textRotateTransform.CenterY = text.GetHeight() / 2;
            }
        }

        public void MoveLeft(double left)
        {
            if (Canvas.GetLeft(this) + left < 0 || Canvas.GetLeft(this) + left + this.GetWidth() > _canvas.Width)
                return;

            Canvas.SetLeft(this, Canvas.GetLeft(this) + left);
            SetThumbsAndBorderPosition();

            if (Label != null)
                Label.MoveLeft(left);
        }

        public void MoveTop(double top)
        {
            if (Canvas.GetTop(this) + top < 0 || Canvas.GetTop(this) + top + this.GetHeight() > _canvas.Height)
                return;

            Canvas.SetTop(this, Canvas.GetTop(this) + top);
            SetThumbsAndBorderPosition();

            if (Label != null)
                Label.MoveTop(top);
        }

        private void SetThumbsAndBorderPosition()
        {
            foreach (GraphicsThumb thumb in Thumbs)
            {
                thumb.SetPosition();
            }

            double centerX = 0;
            double centerY = 0;
            //move _border
            Canvas.SetLeft(_border, GetLeft() - _borderOffset / _canvas.LayoutTransform.Value.M11);
            Canvas.SetTop(_border, GetTop() - _borderOffset / _canvas.LayoutTransform.Value.M11);
            _border.Width = Width + _borderOffset * 2 / _canvas.LayoutTransform.Value.M11;
            _border.Height = Height + _borderOffset * 2 / _canvas.LayoutTransform.Value.M11;
            var transform = RenderTransform as RotateTransform;

            if (transform != null)
            {
                centerX = transform.CenterX + _borderOffset / _canvas.LayoutTransform.Value.M11;
                centerY = transform.CenterY + _borderOffset / _canvas.LayoutTransform.Value.M11;
            }
            else
            {
                centerX = _border.Width / 2;
                centerY = _border.Height / 2;    
            }

            _border.RenderTransform = new RotateTransform(GetRotateAngle(), centerX, centerY);
        }

        #region ImyShape Members

        public void SetLeft(double left)
        {
            Canvas.SetLeft(this, left);

            foreach (GraphicsThumb thumb in Thumbs)
            {
                thumb.SetPosition();
            }
        }

        public void SetTop(double top)
        {
            Canvas.SetTop(this, top);

            foreach (GraphicsThumb thumb in Thumbs)
            {
                thumb.SetPosition();
            }
        }

        public void SetWidth(double width)
        {
            Width = width;
            double aspectRatio = Source.Width / Source.Height;
            Height = Width/aspectRatio;

            if (_isSelected)
                SetThumbsAndBorderPosition();
        }

        public void SetHeight(double height)
        {
            Height = height;
            double aspectRatio = Source.Width / Source.Height;
            Width = Height * aspectRatio;

            if (_isSelected)
                SetThumbsAndBorderPosition();
        }

        public void SetLineWidth(double lineWidth)
        {
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
            this.zIndex = zIndex;
            Canvas.SetZIndex(this, zIndex);
        }

        public int GetZIndex()
        {
            return zIndex;
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

        public ICollection<Category> GetCategories()
        {
            return new HashSet<Category>(Categories);
        }

        public void RotateByAngle(double angle)
        {
            double actualAngle = GetRotateAngle();
            double newAngle = actualAngle + angle;

            if (newAngle < 0)
                newAngle = 360 - Math.Abs(newAngle);

            if (newAngle >= 360)
                newAngle = newAngle - 360;

            RotateTransform newRotateTransform = new RotateTransform(newAngle, GetWidth() / 2, GetHeight() / 2);
            RenderTransform = newRotateTransform;
            RotateLabel(newAngle - actualAngle, actualAngle);
            UnSelect();
            Select(true);
        }

        public void ResetRotateAngle()
        {
            double actualAngle = GetRotateAngle();
            RotateTransform newRotateTransform = new RotateTransform(0, GetWidth() / 2, GetHeight() / 2);
            RenderTransform = newRotateTransform;
            RotateLabel(-actualAngle, actualAngle);
            UnSelect();
            Select(true);
        }

        public GraphicsObjectType GetGraphicsObjectType()
        {
            return GraphicsObjectType.Symbol;
        }

        #endregion

        #region IGraphicsObject Members


        public virtual object Serialize()
        {
            return null;
        }

        public void LockCursorChange()
        {
        }

        public void UnlockCursorChange()
        {
            Cursor = Cursors.Arrow;
        }

        public event Action ObjectChanged;

        #endregion
    }

    public class DefaultParameters
    {
        public double Left { get; set; }
        public double Top { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        public DefaultParameters(double left, double top, double width, double height)
        {
            Left = left;
            Top = top;
            Width = width;
            Height = height;
        }
    }
}
