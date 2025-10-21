using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Contal.Cgp.NCAS.WpfGraphicsControl;

namespace Cgp.NCAS.WpfGraphicsControl
{
    public class GraphicsObjects
    {
        private static GraphicsObjects _singleton;

        public static GraphicsObjects Singleton
        {
            get { return _singleton ?? (_singleton = new GraphicsObjects()); }
        }

        public LinkedList<IGraphicsObject> GetGraphicsObjects(Canvas canvas)
        {
            if (canvas == null)
                return null;

            var graphicsObjects = new LinkedList<IGraphicsObject>();

            foreach (var element in canvas.Children)
            {
                var graphicsObject = element as IGraphicsObject;

                if (graphicsObject != null)
                    graphicsObjects.AddLast(graphicsObject);
            }

            return graphicsObjects;
        }

        public LinkedList<IGraphicsObject> GetGraphicsObjectsByLayer(Canvas canvas, Guid idLayer)
        {
            if (canvas == null || idLayer == Guid.Empty)
                return null;

            var graphicsObjects = new LinkedList<IGraphicsObject>();

            foreach (var element in canvas.Children)
            {
                var graphicsObject = element as IGraphicsObject;

                if (graphicsObject != null
                    && graphicsObject.GetLayerID() == idLayer)
                    graphicsObjects.AddLast(graphicsObject);
            }

            return graphicsObjects;
        }

        public void SetCursorForAllGraphicsObject(Canvas canvas, Cursor cursor)
        {
            canvas.Cursor = cursor;

            foreach (var grObj in GetGraphicsObjects(canvas))
            {
                var element = (FrameworkElement) grObj;
                element.Cursor = cursor;
            }
        }

        public void LockCursorChanged(Canvas canvas)
        {
            foreach (var grObj in GetGraphicsObjects(canvas))
            {
                grObj.LockCursorChange();
            }
        }

        public void UnlockCursorChanged(Canvas canvas)
        {
            foreach (var grObj in GetGraphicsObjects(canvas))
            {
                grObj.UnlockCursorChange();
            }
        }
    }
}
