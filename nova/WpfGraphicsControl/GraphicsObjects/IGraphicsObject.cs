using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Contal.Cgp.NCAS.WpfGraphicsControl
{
    public interface IGraphicsObject
    {
        void MoveLeft(double left);
        void MoveTop(double top);
        void SetLeft(double left);
        void SetTop(double top);
        void SetWidth(double width);
        void SetHeight(double height);
        void SetLineWidth(double lineWidth);
        void UnSelect();
        void Select(bool resizing);
        void SetLayerID(Guid LayerID);
        void SetZIndex(int zIndex);
        double GetLeft();
        double GetTop();
        double GetWidth();
        double GetHeight();
        double GetLineWidth();
        bool isSelected();
        Canvas GetCanvas();
        Guid GetLayerID();
        int GetZIndex();
        List<GraphicsThumb> GetThumbs();
        double GetRotateAngle();
        void RotateByAngle(double angle);
        void ResetRotateAngle();
        GraphicsObjectType GetGraphicsObjectType();
        object Serialize();
        void LockCursorChange();
        void UnlockCursorChange();
        event Action ObjectChanged;
    }

    public enum GraphicsObjectType : byte
    {
        RectangleAndEllipse,
        Line,
        Polyline,
        AlarmArea,
        Symbol,
        Text,
        GraphicsSceneBox
    }
}
