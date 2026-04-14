using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.WpfGraphicsControl;
using Contal.Cgp.Server.Beans;
using Brushes = System.Windows.Media.Brushes;
using Pen = System.Windows.Media.Pen;
using Point = System.Windows.Point;

namespace Cgp.NCAS.WpfGraphicsControl
{
    public class InformationPanel : UIElement
    {
        public string ObjectType { get; set; }
        public string ObjectName { get; set; }
        public string ObjectState { get; set; }

        private readonly VisualCollection _children;

        public InformationPanel()
        {
            ObjectType = "";
            ObjectName = "";
            _children = new VisualCollection(this);
        }

        public void Draw()
        {
            double length;
            _children.Clear();
            var drawingVisual = new DrawingVisual();
            var drawingContext = drawingVisual.RenderOpen();
            var formattedTextObjectType = new FormattedText(ObjectType, CultureInfo.GetCultureInfo(1),
                FlowDirection.LeftToRight, new Typeface("Verdana"), 12, Brushes.White);
            var formattedTextName = new FormattedText(GraphicsScene.LocalizationHelper.GetString("ColumnName") 
                + ": " + ObjectName, CultureInfo.GetCultureInfo(1),
                FlowDirection.LeftToRight, new Typeface("Verdana"), 12, Brushes.White);
            var formattedTextState = new FormattedText(GraphicsScene.LocalizationHelper.GetString("ColumnState")
                + ": " + GraphicsScene.LocalizationHelper.GetString("ObjectState_" + ObjectState), CultureInfo.GetCultureInfo(1),
                FlowDirection.LeftToRight, new Typeface("Verdana"), 12, Brushes.White);

            if (formattedTextName.Width > formattedTextState.Width)
                length = formattedTextName.Width + 10;
            else
                length = formattedTextState.Width + 10;

            if (length + 10 < 140)
                length = 140;

            if ((!string.IsNullOrEmpty(ObjectState)) && (ObjectState.ToLower() == State.Tamper.ToString().ToLower() ||
                ObjectState.ToLower() == OnlineState.Offline.ToString().ToLower() ||
                ObjectState.ToLower() == State.Alarm.ToString().ToLower() ||
                ObjectState.ToLower() == State.Break.ToString().ToLower() ||
                ObjectState.ToLower() == State.On.ToString().ToLower() ||
                ObjectState.ToLower() == State.Short.ToString().ToLower() ||
                ObjectState.ToLower() == State.ajar.ToString().ToLower() ||
                ObjectState.ToLower() == State.intrusion.ToString().ToLower() ||
                ObjectState.ToLower() == State.sabotage.ToString().ToLower() ||
                ObjectState.ToLower() == State.Set.ToString().ToLower()))
            {
                drawingContext.DrawRectangle(Brushes.Red, new Pen(Brushes.Black, 1),
                    new Rect(0, 0, length, 60));
            }
            else
            {
                drawingContext.DrawRectangle(Brushes.Blue, new Pen(Brushes.Black, 1),
                    new Rect(0, 0, length, 60));
            }

            drawingContext.DrawText(formattedTextObjectType, new Point(5, 5));  
            drawingContext.DrawText(formattedTextName, new Point(5, 20));  
            drawingContext.DrawText(formattedTextState, new Point(5, 35));

            drawingContext.Close();
            _children.Add(drawingVisual);
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
