using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Contal.Cgp.NCAS.WpfGraphicsControl;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using Brushes = System.Windows.Media.Brushes;
using Point = System.Windows.Point;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace Cgp.NCAS.WpfGraphicsControl
{
    public class Text : TextBlock, IGraphicsObject, ISceneLink
    {
        private readonly Canvas _canvas;
        private readonly Rectangle _border = new Rectangle();
        private bool _isSelected;
        private Guid _layerID;
        private TextDefaultParameters _defaultParameters;
        private readonly TextBox _textBox = new TextBox();
        private readonly List<GraphicsThumb> _thumbs = new List<GraphicsThumb>();
        private bool _editMode;
        private Cursor _requiredCursor = Cursors.Arrow;
        private bool _lockCursorChange;

        public static bool AlowEdit { get; set; }

        public int ZIndex { get; set; }
        public ILiveObject LiveObject { get; set; }
        public Guid IdScene { get; set; }

        public bool EditMode
        {
            get { return _editMode; }
            set
            {
                _editMode = value;

                if (_editMode)
                {
                    UnSelect();
                    _isSelected = true;
                    _textBox.Visibility = Visibility.Visible;
                    double scale = _canvas.LayoutTransform.Value.M11;
                    Canvas.SetLeft(_textBox, GetLeft() - 5/scale);
                    Canvas.SetTop(_textBox, GetTop() - 3/scale);
                    Canvas.SetZIndex(_textBox, 101);
                    _textBox.Text = Text;
                    _textBox.Width = Width*scale;
                    _textBox.FontSize = FontSize*scale;
                    _canvas.Children.Add(_textBox);
                    _textBox.RenderTransform = new ScaleTransform(1/scale, 1/scale);
                    Visibility = Visibility.Hidden;

                    Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle,
                        new Action(delegate()
                        {
                            _textBox.Focus();
                            _textBox.CaretIndex = _textBox.Text.Length;
                        }));
                    _textBox.KeyUp += _textBox_KeyUp;
                }
                else
                {
                    UnSelect();
                }
            }
        }

        void _textBox_KeyUp(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }

        public Text(Canvas canvas)
        {
            Canvas.SetZIndex(_border, 99);
            SetZIndex(0);
            _canvas = canvas;
            MouseDown += Text_MouseDown;
            MouseEnter += Text_MouseEnter;
            MouseLeave += Text_MouseLeave;
            _textBox.Visibility = Visibility.Hidden;
            _textBox.TextChanged += _textBox_TextChanged;
            TextWrapping = TextWrapping.Wrap;
            LineStackingStrategy = LineStackingStrategy.BlockLineHeight;
            HorizontalAlignment = HorizontalAlignment.Stretch;
            Typography.NumeralStyle = FontNumeralStyle.OldStyle;
            Typography.SlashedZero = true;
        }

        void Text_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!_lockCursorChange)
                Cursor = Cursors.Arrow;

            _requiredCursor = Cursors.Arrow;
        }

        void Text_MouseEnter(object sender, MouseEventArgs e)
        {
            if (IdScene != Guid.Empty)
            {
                if (!_lockCursorChange)
                    Cursor = Cursors.Hand;

                _requiredCursor = Cursors.Hand;
            }
        }

        public void LoadDefaulParameters()
        {
            _defaultParameters = new TextDefaultParameters(GetLeft(), GetTop(), FontSize);
        }

        public TextDefaultParameters GetDefaultParameters()
        {
            return _defaultParameters;
        }

        void _textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Text = _textBox.Text;
        }

        void Text_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2
                && AlowEdit)
            {
                EditMode = true;
            }
        }

        void thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (ObjectChanged != null)
                ObjectChanged();

            GraphicsOperations.ComputeThumbOperations(_canvas, sender as GraphicsThumb, e);
            SetThumbsAndBorderPosition();
        }

        private void SetThumbsAndBorderPosition()
        {
            foreach (var thumb in _thumbs)
                thumb.SetPosition();

            var rotateTransform = RenderTransform as RotateTransform;

            if (rotateTransform != null)
            {
                var borderTransform = new RotateTransform(rotateTransform.Angle);
                borderTransform.CenterX = rotateTransform.CenterX;
                borderTransform.CenterY = rotateTransform.CenterY;
                _border.RenderTransform = borderTransform;
            }
            //move _border
            Canvas.SetLeft(_border, GetLeft() - 1 / _canvas.LayoutTransform.Value.M11);
            Canvas.SetTop(_border, GetTop() - 1/_canvas.LayoutTransform.Value.M11);
            _border.Width = DesiredSize.Width + 2 / _canvas.LayoutTransform.Value.M11;
            _border.Height = DesiredSize.Height + 2 / _canvas.LayoutTransform.Value.M11;
        }

        private void ComputeNewRotateTransformValues()
        {
            //computed new position for center point which is in center of shape
            var transform = RenderTransform as RotateTransform;

            if (transform != null)
            {
                var newCenterPoint = new Point(GetLeft() + GetWidth() / 2 - GetLineWidth() / 2,
                    GetTop() + GetHeight() / 2 - GetLineWidth() / 2);

                var newTransform = new RotateTransform(transform.Angle, transform.CenterX + GetLeft(),
                    transform.CenterY + GetTop());

                newCenterPoint = newTransform.Transform(newCenterPoint);
                SetLeft(newCenterPoint.X - GetWidth() / 2 + GetLineWidth() / 2);
                SetTop(newCenterPoint.Y - GetHeight() / 2 + GetLineWidth() / 2);
                transform.CenterX = GetWidth() / 2 - GetLineWidth() / 2;
                transform.CenterY = GetHeight() / 2 - GetLineWidth() / 2;
            }
        }

        public void MoveLeft(double left)
        {
            if (LiveObject == null && (Canvas.GetLeft(this) + left < 0 
                || Canvas.GetLeft(this) + left + GetWidth() > _canvas.Width))
                return;

            Canvas.SetLeft(this, Canvas.GetLeft(this) + left);
            SetThumbsAndBorderPosition();
        }

        public void MoveTop(double top)
        {
            if (LiveObject == null && (Canvas.GetTop(this) + top < 0 
                || Canvas.GetTop(this) + top + GetHeight() > _canvas.Height))
                return;

            Canvas.SetTop(this, Canvas.GetTop(this) + top);
            SetThumbsAndBorderPosition();
        }

        public void SetLeft(double left)
        {
            Canvas.SetLeft(this, left);
        }

        public void SetTop(double top)
        {
            Canvas.SetTop(this, top);
        }

        public void SetWidth(double width)
        {
        }

        public void SetHeight(double height)
        {
        }

        public void SetLineWidth(double lineWidth)
        {
        }

        public void UnSelect()
        {
            Canvas.SetZIndex(this, ZIndex);
            _isSelected = false;

            foreach (var thumb in _thumbs)
                _canvas.Children.Remove(thumb);

            _canvas.Children.Remove(_border);
            _textBox.Visibility = Visibility.Collapsed;

            if (_editMode)
            {                
                Visibility = Visibility.Visible;
                _canvas.Children.Remove(_textBox);
                ComputeNewRotateTransformValues();
            }
        }

        public void Select(bool resizing)
        {
            if (!_isSelected)
            {
                _isSelected = true;
                Canvas.SetZIndex(this, 100);
                _border.Stroke = Brushes.Blue;
                _border.StrokeThickness = 1 / _canvas.LayoutTransform.Value.M11;

                if (!_canvas.Children.Contains(_border))
                    _canvas.Children.Add(_border);

                if (!resizing)
                {
                    return;
                }

                GraphicsOperations.CreateThumbsForText(_canvas, this, thumb_DragDelta);
                SetThumbsAndBorderPosition();
            }
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
            return _layerID;
        }

        public int GetZIndex()
        {
            return ZIndex;
        }

        public List<GraphicsThumb> GetThumbs()
        {
            return _thumbs;
        }

        public double GetRotateAngle()
        {
            RotateTransform transform = RenderTransform as RotateTransform;

            if (transform != null)
                return transform.Angle;
            
            return 0;
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
            return GraphicsObjectType.Text;
        }

        public object Serialize()
        {
            return LiveObject == null
                ? new SerializableText(this)
                : null;
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
    public class SerializableText : SerializableObject, IGraphicsDeserializeObject
    {
        public SerializableColor TextColor { get; set; }
        public SerializableColor BackgroundColor { get; set; }
        public string Text { get; set; }
        public double FontSize { get; set; }
        public string FontFamilyName { get; set; }
        public bool isBold { get; set; }
        public bool isItalic { get; set; }
        public bool isUnderline { get; set; }
        public Guid IdScene { get; set; }

        public SerializableText(Text text)
        {
            Serialize(text);
            TextColor = new SerializableColor((text.Foreground as SolidColorBrush).Color);

            if (text.Background != null)
                BackgroundColor = new SerializableColor((text.Background as SolidColorBrush).Color);

            Text = text.Text;
            FontSize = text.FontSize;
            FontFamilyName = text.FontFamily.ToString();
            IdScene = text.IdScene;

            if (text.FontWeight == FontWeights.Bold)
                isBold = true;
            else
                isBold = false;

            if (text.FontStyle == FontStyles.Italic)
                isItalic = true;
            else
                isItalic = false;

            if (text.TextDecorations == TextDecorations.Underline)
                isUnderline = true;
            else
                isUnderline = false;
        }

        public Text GetText(Canvas canvas)
        {
            var text = new Text(canvas);
            Deserialize(text);
            text.Foreground = new SolidColorBrush(TextColor.GetColor());
            text.IdScene = IdScene;

            if (BackgroundColor != null)
                text.Background = new SolidColorBrush(BackgroundColor.GetColor());

            text.Text = Text;
            text.FontSize = FontSize;
            text.FontFamily = new FontFamily(FontFamilyName);

            if (isBold)
                text.FontWeight = FontWeights.Bold;

            if (isItalic)
                text.FontStyle = FontStyles.Italic;

            if (isUnderline)
                text.TextDecorations = TextDecorations.Underline;

            text.LoadDefaulParameters();
            canvas.Children.Add(text);
            return text;
        }

        public IGraphicsObject Deserialize(Canvas canvas, Dictionary<string, SymbolParemeter> symbols)
        {
            return GetText(canvas);
        }
    }

    public class TextDefaultParameters
    {
        public double Left { get; set; }
        public double Top { get; set; }
        public double FontSize { get; set; }

        public TextDefaultParameters(double left, double top, double fontSize)
        {
            Left = left;
            Top = top;
            FontSize = fontSize;
        }
    }
}
