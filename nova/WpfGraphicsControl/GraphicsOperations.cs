using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using Contal.Cgp.NCAS.WpfGraphicsControl;
using System.Windows.Controls;
using System.Windows.Media;

namespace Cgp.NCAS.WpfGraphicsControl
{
    public static class GraphicsOperations
    {
        public static void CreateThumbsForRectange(
            Canvas canvas, 
            IGraphicsObject graphicsObject,
            bool allowedRotate,
            DragDeltaEventHandler dragDeltaEvent)
        {
            if (canvas == null
                || graphicsObject == null
                || dragDeltaEvent == null)
                return;

            var thumb1 = new GraphicsThumb(graphicsObject, 1);
            thumb1.DataContext = graphicsObject;
            thumb1.DragDelta += dragDeltaEvent;
            graphicsObject.GetThumbs().Add(thumb1);
            canvas.Children.Add(thumb1);

            var thumb2 = new GraphicsThumb(graphicsObject, 2);
            thumb2.DataContext = graphicsObject;
            thumb2.DragDelta += dragDeltaEvent;
            graphicsObject.GetThumbs().Add(thumb2);
            canvas.Children.Add(thumb2);

            var thumb3 = new GraphicsThumb(graphicsObject, 3);
            thumb3.DataContext = graphicsObject;
            thumb3.DragDelta += dragDeltaEvent;
            graphicsObject.GetThumbs().Add(thumb3);
            canvas.Children.Add(thumb3);

            var thumb4 = new GraphicsThumb(graphicsObject, 4);
            thumb4.DataContext = graphicsObject;
            thumb4.DragDelta += dragDeltaEvent;
            graphicsObject.GetThumbs().Add(thumb4);
            canvas.Children.Add(thumb4);

            if (allowedRotate)
            {
                //thumb for set rotate angle
                var thumb5 = new GraphicsThumb(graphicsObject, 5);
                thumb5.DataContext = graphicsObject;
                thumb5.DragDelta += dragDeltaEvent;
                graphicsObject.GetThumbs().Add(thumb5);
                canvas.Children.Add(thumb5);
            }
        }

        public static void CreateThumbsForText(Canvas canvas, IGraphicsObject graphicsObject, DragDeltaEventHandler dragDeltaEvent)
        {
            if (canvas == null
                || graphicsObject == null
                || dragDeltaEvent == null)
                return;

            //thumb for set rotate angle
            var thumb = new GraphicsThumb(graphicsObject, 5);
            thumb.DataContext = graphicsObject;
            thumb.DragDelta += dragDeltaEvent;
            graphicsObject.GetThumbs().Add(thumb);
            canvas.Children.Add(thumb);
        }

        public static void ComputeThumbOperations(Canvas canvas, GraphicsThumb thumb, DragDeltaEventArgs e)
        {
            if (thumb == null
                || canvas == null
                || e == null)
                return;

            var element = thumb.DataContext as FrameworkElement;
            var graphicsObject = element as IGraphicsObject;

            if (element == null
                || graphicsObject == null)
                return;

            double offsetX = 0, offsetY = 0;

            switch (thumb.Count)
            {
                case 1:
                    offsetX = graphicsObject.GetLineWidth()/2;
                    offsetY = graphicsObject.GetLineWidth()/2;
                    break;
                case 2:
                    offsetX = thumb.Width + graphicsObject.GetLineWidth()/2;
                    offsetY = graphicsObject.GetLineWidth()/2;
                    break;
                case 3:
                    offsetX = thumb.Width + graphicsObject.GetLineWidth()/2;
                    offsetY = thumb.Height + graphicsObject.GetLineWidth()/2;
                    break;
                case 4:
                    offsetX = graphicsObject.GetLineWidth()/2;
                    offsetY = thumb.Height + graphicsObject.GetLineWidth()/2;
                    break;
                case 5:
                    double a = Canvas.GetTop(thumb) + e.VerticalChange - thumb.FixedPoint.Y;
                    double b = Canvas.GetLeft(thumb) + thumb.Width/2 + e.HorizontalChange - thumb.FixedPoint.X;
                    double c = Math.Pow(Math.Pow(a, 2) + Math.Pow(b, 2), 0.5);
                    double rotateAngle;

                    if (a < 0 && b > 0)
                        rotateAngle = 90 - Math.Acos(b/c)*(180/Math.PI);
                    else if (a > 0 && b > 0)
                        rotateAngle = 90 + Math.Acos(b/c)*(180/Math.PI);
                    else if (a > 0 && b < 0)
                        rotateAngle = 180 + Math.Asin(-b/c)*(180/Math.PI);
                    else
                        rotateAngle = 360 - Math.Asin(-b/c)*(180/Math.PI);

                    var rotateTransform = new RotateTransform(rotateAngle);
                    rotateTransform.CenterX = thumb.FixedPoint.X - Canvas.GetLeft(element);
                    rotateTransform.CenterY = thumb.FixedPoint.Y - Canvas.GetTop(element);
                    element.RenderTransform = rotateTransform;


                    return;
            }

            double actualLeft = Canvas.GetLeft(thumb) + offsetX + e.HorizontalChange;
            double actualTop = Canvas.GetTop(thumb) + offsetY + e.VerticalChange;
            double newWidth = thumb.FixedPoint.X - actualLeft;
            double newHeight = thumb.FixedPoint.Y - actualTop;
            double newPosX, newPosY;
            var graphicsSymbol = element as GraphicsSymbol;

            if (graphicsSymbol != null)
            {
                double aspectRation = graphicsSymbol.Source.Width/graphicsSymbol.Source.Height;

                if (newHeight > 0)
                    newHeight = Math.Abs(newWidth/aspectRation);
                else
                    newHeight = -Math.Abs(newWidth/aspectRation);

                if (newWidth < 0)
                    newPosX = thumb.FixedPoint.X;
                else
                    newPosX = thumb.FixedPoint.X - newWidth;

                if (newHeight < 0)
                    newPosY = thumb.FixedPoint.Y;
                else
                    newPosY = thumb.FixedPoint.Y - newHeight;
            }
            else
            {
                if (newHeight < 0)
                    newPosY = thumb.FixedPoint.Y;
                else
                    newPosY = actualTop;

                if (newWidth < 0)
                    newPosX = thumb.FixedPoint.X;
                else
                    newPosX = actualLeft;
            }

            if (newPosX < 0 || newPosY < 0
                || newPosX + Math.Abs(newWidth) > canvas.Width
                || newPosY + Math.Abs(newHeight) > canvas.Height)
                return;

            graphicsObject.SetLeft(newPosX);
            graphicsObject.SetTop(newPosY);
            graphicsObject.SetWidth(Math.Abs(newWidth));

            if (graphicsSymbol == null)
                graphicsObject.SetHeight(Math.Abs(newHeight));

            //calculate new rotate transformation
            var oldRotateTransform = element.RenderTransform as RotateTransform;

            if (oldRotateTransform != null)
            {
                var newRotateTransform = new RotateTransform(oldRotateTransform.Angle);

                switch (thumb.Count)
                {
                    case 1:
                        if (graphicsObject.GetLeft() < thumb.FixedPoint.X)
                            newRotateTransform.CenterX = element.Width - thumb.RotatePoint.X;
                        else
                            newRotateTransform.CenterX = -thumb.RotatePoint.X;

                        if (graphicsObject.GetTop() < thumb.FixedPoint.Y)
                            newRotateTransform.CenterY = element.Height - thumb.RotatePoint.Y;
                        else
                            newRotateTransform.CenterY = -thumb.RotatePoint.Y;
                        break;

                    case 2:
                        if (graphicsObject.GetLeft() + element.Width > thumb.FixedPoint.X)
                            newRotateTransform.CenterX = thumb.RotatePoint.X;
                        else
                            newRotateTransform.CenterX = thumb.RotatePoint.X + element.Width;

                        if (graphicsObject.GetTop() < thumb.FixedPoint.Y)
                            newRotateTransform.CenterY = element.Height - thumb.RotatePoint.Y;
                        else
                            newRotateTransform.CenterY = -thumb.RotatePoint.Y;
                        break;

                    case 3:
                        if (graphicsObject.GetLeft() + element.Width > thumb.FixedPoint.X)
                            newRotateTransform.CenterX = thumb.RotatePoint.X;
                        else
                            newRotateTransform.CenterX = thumb.RotatePoint.X + element.Width;

                        if (graphicsObject.GetTop() + element.Height > thumb.FixedPoint.Y)
                            newRotateTransform.CenterY = thumb.RotatePoint.Y;
                        else
                            newRotateTransform.CenterY = thumb.RotatePoint.Y + element.Height;
                        break;

                    case 4:
                        if (graphicsObject.GetLeft() < thumb.FixedPoint.X)
                            newRotateTransform.CenterX = element.Width - thumb.RotatePoint.X;
                        else
                            newRotateTransform.CenterX = -thumb.RotatePoint.X;

                        if (graphicsObject.GetTop() + element.Height > thumb.FixedPoint.Y)
                            newRotateTransform.CenterY = thumb.RotatePoint.Y;
                        else
                            newRotateTransform.CenterY = thumb.RotatePoint.Y + element.Height;
                        break;
                }

                element.RenderTransform = newRotateTransform;
            }
        }

        // Calculate the distance between
        // point pt and the segment p1 --> p2.
        public static double FindDistanceToSegment(Point pt, Point p1, Point p2, out Point closest)
        {
            double dx = p2.X - p1.X;
            double dy = p2.Y - p1.Y;
            if ((dx == 0) && (dy == 0))
            {
                // It's a point not a line segment.
                closest = p1;
                dx = pt.X - p1.X;
                dy = pt.Y - p1.Y;
                return Math.Sqrt(dx * dx + dy * dy);
            }

            // Calculate the t that minimizes the distance.
            double t = ((pt.X - p1.X) * dx + (pt.Y - p1.Y) * dy) /
                (dx * dx + dy * dy);

            // See if this represents one of the segment's
            // end points or a point in the middle.
            if (t < 0)
            {
                closest = new Point(p1.X, p1.Y);
                dx = pt.X - p1.X;
                dy = pt.Y - p1.Y;
            }
            else if (t > 1)
            {
                closest = new Point(p2.X, p2.Y);
                dx = pt.X - p2.X;
                dy = pt.Y - p2.Y;
            }
            else
            {
                closest = new Point(p1.X + t * dx, p1.Y + t * dy);
                dx = pt.X - closest.X;
                dy = pt.Y - closest.Y;
            }

            return Math.Sqrt(dx * dx + dy * dy);
        }
    }
}
