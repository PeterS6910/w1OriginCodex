using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Contal.Cgp.NCAS.WpfGraphicsControl;

namespace Cgp.NCAS.WpfGraphicsControl
{
    [Serializable()]
    public class SerializableObject
    {
        public Guid LayerID { get; set; }
        public double posX { get; set; }
        public double posY { get; set; }
        public double width { get; set; }
        public double height { get; set; }
        public double rotateAngle { get; set; }
        public int zIndex { get; set; }
        public List<Category> Categories { get; set; }

        public void Serialize(UIElement shape)
        {
            if (shape == null)
                return;

            var grObject = shape as IGraphicsObject;

            if (grObject == null)
                return;

            LayerID = grObject.GetLayerID();
            posX = Canvas.GetLeft(shape);
            posY = Canvas.GetTop(shape);
            width = grObject.GetWidth();
            height = grObject.GetHeight();
            rotateAngle = grObject.GetRotateAngle();
            zIndex = grObject.GetZIndex();

            if ((shape as ILiveObject) != null)
                Categories = (shape as ILiveObject).GetCategories().ToList();
        }

        public void Deserialize(UIElement obj)
        {
            if (obj == null)
                return;

            var grObject = obj as IGraphicsObject;

            if (grObject == null)
                return;

            grObject.SetLayerID(LayerID);
            Canvas.SetLeft(obj, posX);
            Canvas.SetTop(obj, posY);
            grObject.SetWidth(width);
            grObject.SetHeight(height);
            grObject.SetZIndex(zIndex);

            if (rotateAngle != 0)
            {
                var rotateTransform = new RotateTransform(rotateAngle, width/2, height/2);
                obj.RenderTransform = rotateTransform;
            }

            if ((obj as ILiveObject) != null)
                (obj as ILiveObject).SetCategories(Categories);
        }
    }
}
