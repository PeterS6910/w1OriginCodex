using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace Cgp.NCAS.WpfGraphicsControl
{
    public enum unit
    {
        m = 2, cm = 1, mm = 0
    }

    public class ScaleInfo : UIElement
    {
        private readonly VisualCollection _children;
        private DrawingVisual _text;
        private int _lenght = 100;
        private unit _unit = unit.mm;
        private double _scale = 1.0;
        private double _scaleRatio = 1.0;

        public ScaleInfo(double scale, double scaleRatio)
        {
            _scale = scale;
            _scaleRatio = scaleRatio;
            _children = new VisualCollection(this);
            DrawingLines();
            SetScale(scale);
        }

        public void SetScaleRatio(double scaleRatio)
        {
            if (scaleRatio > 0)
                _scaleRatio = scaleRatio;
        }

        public void SetScale(double scale)
        {
            try
            {
                _scale = scale;
                _children.Clear();
                double distance = 100/scale*_scaleRatio; //in mm
                distance = Math.Round(distance, 0);
                int numberSize = distance.ToString().Length;
                string zero = string.Empty;

                for (int i = 0; i < numberSize - 1; i++)
                    zero += "0";

                int firstNumber = Int32.Parse(distance.ToString().Substring(0, 1));

                if (firstNumber <= 2)
                {
                    int a = Math.Abs(firstNumber - 1);
                    int b = Math.Abs(firstNumber - 2);

                    distance = a > b 
                        ? Double.Parse("2" + zero) 
                        : Double.Parse("1" + zero);
                }

                if (firstNumber > 2 && firstNumber <= 5)
                {
                    int a = Math.Abs(firstNumber - 2);
                    int b = Math.Abs(firstNumber - 5);

                    distance = a > b 
                        ? Double.Parse("5" + zero) 
                        : Double.Parse("2" + zero);
                }

                if (firstNumber > 5 && firstNumber < 10)
                {
                    int a = Math.Abs(firstNumber - 5);
                    int b = Math.Abs(firstNumber - 10);

                    distance = a > b 
                        ? Double.Parse("10" + zero) 
                        : Double.Parse("5" + zero);
                }

                //calculating new lenght
                _lenght = (int) (distance*_scale/_scaleRatio);
                distance = distance/1000; //in meters
                _unit = unit.m;

                if (distance < 1) //to cm
                {
                    distance *= 100;
                    _unit = unit.cm;
                }

                if (distance < 1) //to mm
                {
                    distance *= 10;
                    _unit = unit.mm;
                }

                DrawingLines();
                _children.Add(CreateDrawingVisualText(distance + " " + _unit));
            }
            catch
            {
            }
        }

        private void DrawingLines()
        {
            var drawingVisual = new DrawingVisual();
            var drawingContext = drawingVisual.RenderOpen();
            drawingContext.DrawLine(new Pen(Brushes.Black, 2), new Point(0, 32), new Point(_lenght, 32));
            drawingContext.DrawLine(new Pen(Brushes.Black, 2), new Point(0, 32 - 5), new Point(0, 32 + 5));
            drawingContext.DrawLine(new Pen(Brushes.Black, 2), new Point(_lenght, 32 - 5), new Point(_lenght, 32 + 5));
            drawingContext.Close();
            _children.Add(drawingVisual);
        }

        private DrawingVisual CreateDrawingVisualText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return null;

            _text = new DrawingVisual();
            var drawingContext = _text.RenderOpen();
            var formattedText = new FormattedText(text, CultureInfo.GetCultureInfo(1), 
                FlowDirection.LeftToRight, new Typeface("Verdana"), 20, Brushes.Black);
            drawingContext.DrawText(formattedText, new Point(0, 0));
            drawingContext.Close();
            return _text;
        }

        protected override int VisualChildrenCount
        {
            get { return _children.Count; }
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= _children.Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            return _children[index];
        }
    }
}
