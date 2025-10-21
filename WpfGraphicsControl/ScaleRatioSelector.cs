using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Cgp.NCAS.WpfGraphicsControl;
using Contal.Cgp.NCAS.WpfGraphicsControl;
using Cursors = System.Windows.Input.Cursors;

namespace Cgp.NCAS.WpfGraphicsControl
{
    class ScaleRatioSelector : Shape
    {
        private Canvas _canvas;
        private bool _isDragging = false;
        private Point _startPoint;
        private double _scale;
        private byte _mode = 0;

        public delegate void ResizeDelegate(double newSize);
        public event ResizeDelegate ChangeSize;

        public ScaleRatioSelector(Canvas canvas)
        {
            Canvas.SetZIndex(this, 102);
            this.Stroke = Brushes.Red;
            this.StrokeThickness = 4;
            _canvas = canvas;
            _scale = _canvas.LayoutTransform.Value.M11;

            this.MouseDown += new System.Windows.Input.MouseButtonEventHandler(ScaleRatioSelector_MouseDown);
            this.MouseUp += new MouseButtonEventHandler(ScaleRatioSelector_MouseUp);
            this.MouseMove += new System.Windows.Input.MouseEventHandler(ScaleRatioSelector_MouseMove);
            _canvas.LayoutUpdated += new EventHandler(_canvas_LayoutUpdated);
        }

        void _canvas_LayoutUpdated(object sender, EventArgs e)
        {
            _scale = _canvas.LayoutTransform.Value.M11;
            this.StrokeThickness = 4 / _canvas.LayoutTransform.Value.M11;
            this.InvalidateVisual();
        }

        void ScaleRatioSelector_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {  
            Point newPosition = e.GetPosition(_canvas);

            if (newPosition.X < Canvas.GetLeft(this) + 10/_scale)
            {
                this.Cursor = Cursors.SizeWE;
                
                if (_mode == 0)
                    _mode = 1;
            }
            else if (newPosition.X >= Canvas.GetLeft(this) + 10/_scale &&
                     newPosition.X <= Canvas.GetLeft(this) + this.Width - 10/_scale)
            {
                this.Cursor = Cursors.SizeAll;
                
                if (_mode == 0)
                    _mode = 2;
            }
            else
            {
                this.Cursor = Cursors.SizeWE;

                if (_mode == 0)
                    _mode = 3;
            }

            if (!_isDragging)
                return;

            double deltaX = newPosition.X - _startPoint.X;
            double deltaY = newPosition.Y - _startPoint.Y;

            switch (_mode)
            {
                case 1: //resize component, fix point is in left
                    if (this.Width - deltaX > 30 / _scale && Canvas.GetLeft(this) + deltaX >= 0)
                    {
                        this.Width -= deltaX;
                        Canvas.SetLeft(this, Canvas.GetLeft(this) + deltaX);

                        if (ChangeSize != null)
                            ChangeSize(this.Width);
                    }
                    break;

                case 2: //move component
                    if (Canvas.GetLeft(this) + deltaX > 0 && Canvas.GetLeft(this) + deltaX + this.Width < _canvas.Width
                        && Canvas.GetTop(this) + deltaY > 0 && Canvas.GetTop(this) + deltaY + (20 / _scale) < _canvas.Height)
                    {
                        Canvas.SetLeft(this, Canvas.GetLeft(this) + deltaX);
                        Canvas.SetTop(this, Canvas.GetTop(this) + deltaY);
                    }
                    break;

                case 3: //resize component, fix point is in right
                    if (this.Width + deltaX > 30 / _scale && Canvas.GetLeft(this) + this.Width + deltaX <= _canvas.Width)
                    {
                        this.Width += deltaX;

                        if (ChangeSize != null)
                            ChangeSize(this.Width);
                    }
                    break;
            }

            _startPoint = newPosition;
        }

        void ScaleRatioSelector_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                _isDragging = false;
                base.ReleaseMouseCapture();
            }
        }

        void ScaleRatioSelector_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                _startPoint = e.GetPosition(_canvas);
                _isDragging = true;
                _mode = 0;
                base.CaptureMouse();
            }
        }

        protected override Geometry DefiningGeometry
        {
            get
            {
                GeometryGroup gr = new GeometryGroup();
                Geometry geometry = gr;
                
                LineGeometry mainLine = new LineGeometry(new Point(0, 10/_scale), new Point(this.Width, 10/_scale));
                LineGeometry leftLine = new LineGeometry(new Point(0, 0), new Point(0, 20/_scale));
                LineGeometry rightLine = new LineGeometry(new Point(this.Width, 0), new Point(this.Width, 20/_scale));

                gr.Children.Add(mainLine);
                gr.Children.Add(leftLine);
                gr.Children.Add(rightLine);
                //geometry.Freeze();
                return gr;
            }
        }
    }
}
