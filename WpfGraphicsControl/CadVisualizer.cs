using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows.Media.Imaging;
using WW.Cad.Drawing;
using WW.Cad.Model;
using WW.Cad.Model.Entities;
using WW.Drawing;
using WW.Math;

namespace Contal.Cgp.NCAS.WpfGraphicsControl
{
    public class CadVisualizer
    {
        private const double PI2 = Math.PI * 2;
        private DxfModel _dxfModel;
        private bool _firstLoad = true;
        private double _zoom;
        private int _width, _height;
        private double _offsetX, _offsetY;
        private readonly List<Line> _lines = new List<Line>();
        private readonly List<Arc> _arcs = new List<Arc>();
        private readonly List<Entity> _entities = new List<Entity>();
        private readonly ManualResetEvent _waitToFinishLoadingDrawing = new ManualResetEvent(true);
        private WriteableBitmap _writeableBitmap;
        private double _defaultZoom;

        public WriteableBitmap WriteableBitmap
        {
            get
            {
                return _writeableBitmap;
            }
            set
            {
                _writeableBitmap = value;
                _width = (int)_writeableBitmap.Width;
                _height = (int)_writeableBitmap.Height;
            }
        }

        public double DefaultZoom
        {
            get
            {
                SetOptimalZoom();
                return _defaultZoom;
            }
        }

        public double ActualZoom()
        {
            return _zoom;
        }

        public double ModelWidth
        {
            get;
            private set;
        }

        public double ModelHeight
        {
            get;
            private set;
        }

        public bool CalculateModelSize
        {
            get;
            set;
        }

        public CadVisualizer()
        {
            CalculateModelSize = true;
        }

        public bool LoadModel(DxfModel model)
        {
            if (model == null)
                return false;

            lock (_waitToFinishLoadingDrawing)
            {
                _waitToFinishLoadingDrawing.WaitOne();
                _waitToFinishLoadingDrawing.Reset();
            }

            try
            {
                _firstLoad = true;
                _offsetX = 0;
                _offsetY = 0;
                _dxfModel = model;
                _lines.Clear();
                _arcs.Clear();
                _entities.Clear();
                GetModelSize();
                CalculateModelSize = false;

                for (int i = 0; i < _dxfModel.Layouts.Count - 1; i++)
                {
                    foreach (var entity in _dxfModel.Entities)
                    {
                        if (!entity.Layer.Enabled || !entity.Visible || entity.Layer.Frozen)
                            continue;

                        var insert = entity as DxfInsert;

                        if (insert != null)
                        {
                            ParseDxfInsert(entity as DxfInsert);
                            continue;
                        }

                        var lwPolyline = entity as DxfLwPolyline;

                        if (lwPolyline != null)
                        {
                            var lwEntities = lwPolyline.ExplodeIntoLinesAndArcs();

                            foreach (var ent in lwEntities)
                            {
                                _entities.Add(new Entity() { CadEntity = ent, CadInsert = null });
                            }

                            continue;
                        }

                        var polyline2D = entity as DxfPolyline2D;

                        if (polyline2D != null)
                        {
                            int count = polyline2D.Vertices.Count;

                            for (int j = 0; j < count; j++)
                            {
                                if (j + 1 == count)
                                    break;

                                var line = new DxfLine();
                                line.Start = new Point3D(polyline2D.Vertices[j].X,
                                    polyline2D.Vertices[j].Y, 0);
                                line.End = new Point3D(polyline2D.Vertices[j + 1].X,
                                    polyline2D.Vertices[j + 1].Y, 0);
                                line.Layer = polyline2D.Layer;
                                line.Visible = polyline2D.Visible;
                                _entities.Add(new Entity() { CadEntity = line, CadInsert = null });
                            }
                        }
                        else
                        {
                            _entities.Add(new Entity() { CadEntity = entity, CadInsert = null });
                        }
                    }
                }

                foreach (var entity in _entities)
                {
                    ParseDxfEntity(entity.CadEntity, entity.CadInsert);
                }

                SetOptimalZoom();
                _firstLoad = false;

                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                _waitToFinishLoadingDrawing.Set();
            }
        }

        private void ParseDxfInsert(DxfInsert insert)
        {
            if (!insert.Layer.Enabled || !insert.Visible || insert.Layer.Frozen)
                return;

            foreach (var entity in insert.Block.Entities)
            {
                var insert2 = entity as DxfInsert;

                if (insert2 != null)
                {
                    TransformInsert(entity, insert);
                    ParseDxfInsert(insert2);
                    continue;
                }

                var lwPolyline = entity as DxfLwPolyline;

                if (lwPolyline != null)
                {
                    var lwEntities = lwPolyline.ExplodeIntoLinesAndArcs();
                    int i = 0;

                    foreach (var ent in lwEntities)
                    {
                        var lwPolylineArc = ent as DxfArc;

                        if (lwPolylineArc != null)
                        {
                            double sa = lwPolylineArc.StartAngle;
                            double ea = lwPolylineArc.EndAngle;

                            if (lwPolyline.Vertices[i].Bulge >= 0)
                            {
                                lwPolylineArc.StartAngle = ea;
                                lwPolylineArc.EndAngle = sa;
                            }
                        }

                        i++;
                        _entities.Add(new Entity() { CadEntity = ent, CadInsert = insert });
                    }

                    continue;
                }

                var polyline2D = entity as DxfPolyline2D;

                if (polyline2D != null)
                {
                    int count = polyline2D.Vertices.Count;

                    for (int i = 0; i < count; i++)
                    {
                        if (i + 1 == count)
                            break;

                        var line = new DxfLine();
                        line.Start = new Point3D(polyline2D.Vertices[i].X, polyline2D.Vertices[i].Y, 0);
                        line.End = new Point3D(polyline2D.Vertices[i + 1].X, polyline2D.Vertices[i + 1].Y, 0);
                        _entities.Add(new Entity() { CadEntity = line, CadInsert = insert });
                    }
                }
                else
                {
                    _entities.Add(new Entity() { CadEntity = entity, CadInsert = insert });
                }
            }
        }

        private void TransformInsert(DxfEntity entity, DxfInsert insert)
        {
            if (insert == null || entity == null || !_firstLoad)
                return;

            var config = new TransformConfig();
            Matrix4D transform;

            if (entity is DxfInsert)
            {
                //scale entity
                transform = Transformation4D.Scaling(insert.ScaleFactor);
                entity.TransformMe(config, transform);

                //rotate entity
                transform = Transformation4D.RotateZ(insert.Rotation);
                entity.TransformMe(config, transform);

                //transform entity
                transform = Transformation4D.Translation((Vector3D)insert.InsertionPoint);
                entity.TransformMe(config, transform);
            }
        }

        private bool IsEntityInVisibleSpace(DxfEntity entity)
        {
            if (entity == null)
                return false;

            switch (entity.EntityType)
            {
                case "LINE":
                    var line = entity as DxfLine;
                    if (line.Start.X >= _minX && line.Start.X <= _maxX && line.Start.Y >= -_maxY && line.Start.Y <= -_minY)
                        return true;
                    break;
                case "CIRCLE":
                    var circle = entity as DxfCircle;
                    if (circle.Center.X >= _minX && circle.Center.X <= _maxX && circle.Center.Y >= -_maxY && circle.Center.Y <= -_minY)
                        return true;
                    break;
                case "ARC":
                    var arc = entity as DxfArc;
                    double x1 = arc.Center.X + (Math.Cos(arc.StartAngle) * arc.Radius);
                    double y1 = arc.Center.Y + (Math.Sin(arc.StartAngle) * arc.Radius);
                    if (x1 >= _minX && x1 <= _maxX && y1 >= -_maxY && y1 <= -_minY)
                        return true;
                    double x2 = arc.Center.X + (Math.Cos(arc.EndAngle) * arc.Radius);
                    double y2 = arc.Center.Y + (Math.Sin(arc.EndAngle) * arc.Radius);
                    if (x2 >= _minX && x2 <= _maxX && y2 >= -_maxY && y2 <= -_minY)
                        return true;
                    break;
            }
            return false;
        }

        private bool IsEntityInVisibleSpace(Arc arc)
        {
            if (arc == null)
                return false;

            double x1 = arc.X + (Math.Cos(arc.StartAngle) * arc.radiusX);
            double y1 = arc.Y + (Math.Sin(arc.StartAngle) * arc.radiusY);

            if (x1 >= _minX && x1 <= _maxX && y1 >= -_maxY && y1 <= -_minY)
                return true;

            double x2 = arc.X + (Math.Cos(arc.EndAngle) * arc.radiusX);
            double y2 = arc.Y + (Math.Sin(arc.EndAngle) * arc.radiusY);

            if (x2 >= _minX && x2 <= _maxX && y2 >= -_maxY && y2 <= -_minY)
                return true;

            return false;
        }

        private System.Windows.Media.Color GetArgColor(DxfEntity entity)
        {
            var argbWhite = new ArgbColor(255, 0, 0, 0);

            switch (entity.Color.ColorType)
            {
                case ColorType.ByLayer:
                    argbWhite = GraphicsConfig.AcadLikeWithWhiteBackground.IndexedColors[entity.Layer.Color.ColorIndex];
                    break;
                case ColorType.ByColorIndex:
                    argbWhite = GraphicsConfig.AcadLikeWithWhiteBackground.IndexedColors[entity.Color.ColorIndex];
                    break;
            }

            var color = new System.Windows.Media.Color();
            color.A = argbWhite.A;
            color.R = argbWhite.R;
            color.G = argbWhite.G;
            color.B = argbWhite.B;

            return color;
        }

        private void ParseDxfEntity(DxfEntity entity, DxfInsert insert)
        {
            if (!entity.Layer.Enabled || !entity.Visible || entity.Layer.Frozen)
                return;

            Matrix4D transform;
            var config = new TransformConfig();
            DxfArc newArc = null;
            DxfLine newLine = null;
            DxfEllipse newEllipse = null;
            DxfCircle newCircle = null;
            //DxfPolyline2D newPolyline = null;

            if (insert != null)
            {
                if (entity.EntityType == "ARC")
                {
                    var arc = (entity as DxfArc);
                    newArc = new DxfArc();

                    //copy Arc to newArc
                    newArc.Center = arc.Center;
                    newArc.Radius = arc.Radius;
                    newArc.StartAngle = arc.StartAngle;
                    newArc.EndAngle = arc.EndAngle;
                    newArc.Color = arc.Color;
                    newArc.Layer = arc.Layer;

                    transform = Transformation4D.Scaling(insert.ScaleFactor);
                    newArc.TransformMe(config, transform);
                    transform = Transformation4D.RotateZ(insert.Rotation);
                    newArc.TransformMe(config, transform);
                    transform = Transformation4D.Translation((Vector3D)insert.InsertionPoint);
                    newArc.TransformMe(config, transform);
                }

                if (entity is DxfLine)
                {
                    var line = (entity as DxfLine);
                    newLine = new DxfLine();

                    //copy Line to newLine
                    newLine.Start = line.Start;
                    newLine.End = line.End;
                    newLine.Color = line.Color;
                    newLine.Layer = line.Layer;

                    transform = Transformation4D.Scaling(insert.ScaleFactor);
                    newLine.TransformMe(config, transform);
                    transform = Transformation4D.RotateZ(insert.Rotation);
                    newLine.TransformMe(config, transform);
                    transform = Transformation4D.Translation((Vector3D)insert.InsertionPoint);
                    newLine.TransformMe(config, transform);
                }

                if (entity.EntityType == "ELLIPSE")
                {
                    var ellipse = entity as DxfEllipse;
                    newEllipse = new DxfEllipse();

                    //copy ellipse to newEllipse
                    newEllipse.Center = ellipse.Center;
                    newEllipse.MajorAxisEndPoint = ellipse.MajorAxisEndPoint;
                    newEllipse.MinorToMajorRatio = ellipse.MinorToMajorRatio;

                    newEllipse.StartParameter = ellipse.StartParameter;
                    newEllipse.EndParameter = ellipse.EndParameter;

                    newEllipse.Color = ellipse.Color;
                    newEllipse.Layer = ellipse.Layer;

                    transform = Transformation4D.Scaling(insert.ScaleFactor);
                    newEllipse.TransformMe(config, transform);
                    transform = Transformation4D.RotateZ(insert.Rotation);
                    newEllipse.TransformMe(config, transform);
                    transform = Transformation4D.Translation((Vector3D)insert.InsertionPoint);
                    newEllipse.TransformMe(config, transform);
                }

                if (entity.EntityType == "CIRCLE")
                {
                    var circle = entity as DxfCircle;
                    newCircle = new DxfCircle();

                    //copy circle to newCircle
                    newCircle.Center = circle.Center;
                    newCircle.Radius = circle.Radius;
                    newCircle.Color = circle.Color;
                    newCircle.Layer = circle.Layer;

                    transform = Transformation4D.Scaling(insert.ScaleFactor);
                    newCircle.TransformMe(config, transform);
                    transform = Transformation4D.RotateZ(insert.Rotation);
                    newCircle.TransformMe(config, transform);
                    transform = Transformation4D.Translation((Vector3D)insert.InsertionPoint);
                    newCircle.TransformMe(config, transform);
                }
            }

            if (entity is DxfLine)
            {
                if (newLine == null)
                    newLine = entity as DxfLine;

                if (Double.IsNaN(newLine.Start.X) || Double.IsNaN(newLine.Start.Y) ||
                    Double.IsNaN(newLine.End.X) || Double.IsNaN(newLine.End.Y))
                    return;

                if (!IsEntityInVisibleSpace(newLine))
                    return;

                _lines.Add(new Line()
                {
                    X1 = newLine.Start.X,
                    Y1 = -newLine.Start.Y,
                    X2 = newLine.End.X,
                    Y2 = -newLine.End.Y,
                    color = ConvertColor(GetArgColor(newLine))
                });
            }

            if (entity.EntityType == "ARC")
            {
                if (newArc == null)
                    newArc = entity as DxfArc;

                if (Double.IsNaN(newArc.Center.X) || Double.IsNaN(newArc.Center.Y) ||
                    Double.IsNaN(newArc.Radius) || Double.IsNaN(newArc.StartAngle) ||
                    Double.IsNaN(newArc.EndAngle))
                    return;

                if (!IsEntityInVisibleSpace(newArc))
                    return;

                _arcs.Add(new Arc()
                {
                    X = newArc.Center.X,
                    Y = -newArc.Center.Y,
                    radiusX = newArc.Radius,
                    radiusY = newArc.Radius,
                    StartAngle = newArc.StartAngle,
                    EndAngle = newArc.EndAngle,
                    color = ConvertColor(GetArgColor(newArc))
                });
            }

            if (entity.EntityType == "CIRCLE")
            {
                if (newCircle == null)
                    newCircle = entity as DxfCircle;

                if (Double.IsNaN(newCircle.Center.X) || Double.IsNaN(newCircle.Center.Y) ||
                    Double.IsNaN(newCircle.Radius))
                    return;

                if (!IsEntityInVisibleSpace(newCircle))
                    return;

                _arcs.Add(new Arc()
                {
                    X = newCircle.Center.X,
                    Y = -newCircle.Center.Y,
                    radiusX = newCircle.Radius,
                    radiusY = newCircle.Radius,
                    EndAngle = PI2,
                    StartAngle = 0,
                    color = ConvertColor(GetArgColor(newCircle))
                });
            }

            if (entity.EntityType == "ELLIPSE")
            {
                if (newEllipse == null)
                    newEllipse = entity as DxfEllipse;

                if (Double.IsNaN(newEllipse.Center.X) || Double.IsNaN(newEllipse.Center.Y) ||
                    Double.IsNaN(newEllipse.MajorAxisEndPoint.X) || Double.IsNaN(newEllipse.MajorAxisEndPoint.Y) ||
                    Double.IsNaN(newEllipse.StartParameter) || Double.IsNaN(newEllipse.EndParameter))
                    return;

                double xaxis = newEllipse.MajorAxisEndPoint.GetLength();
                double yaxis = xaxis * newEllipse.MinorToMajorRatio;
                double angle = Math.Atan2(newEllipse.MajorAxisEndPoint.Y, newEllipse.MajorAxisEndPoint.X);
                double startAngle;
                double endAngle;

                if (newEllipse.ZAxis.Z < 0)
                {
                    startAngle = PI2 - newEllipse.EndParameter + angle;
                    endAngle = PI2 - newEllipse.StartParameter + angle;
                }
                else
                {
                    startAngle = newEllipse.StartParameter + angle;
                    endAngle = newEllipse.EndParameter + angle;
                }
                
                var arc = new Arc()
                {
                    X = newEllipse.Center.X,
                    Y = -newEllipse.Center.Y,
                    radiusX = xaxis,
                    radiusY = yaxis,
                    StartAngle = startAngle,
                    EndAngle = endAngle,
                    color = ConvertColor(GetArgColor(newEllipse)),
                };

                if (!IsEntityInVisibleSpace(arc))
                    return;

                _arcs.Add(arc);
            }
        }

        private double _maxX, _maxY, _minX, _minY;

        private void GetModelSize()
        {
            if (_dxfModel == null || !CalculateModelSize)
                return;

            var calc = new BoundsCalculator();
            calc.GetBounds(_dxfModel);
            _maxX = calc.Bounds.Max.X;
            _maxY = -calc.Bounds.Min.Y;
            _minX = calc.Bounds.Min.X;
            _minY = -calc.Bounds.Max.Y;

            ModelWidth = Math.Abs(_maxX - _minX);
            ModelHeight = Math.Abs(_maxY - _minY);
        }

        public void SetOptimalZoom()
        {
            if (_firstLoad && _dxfModel != null)
            {
                foreach (var line in _lines)
                {
                    line.X1 += -_minX;
                    line.X2 += -_minX;
                    line.Y1 += -_minY;
                    line.Y2 += -_minY;
                }

                foreach (var arc in _arcs)
                {
                    arc.X += -_minX;
                    arc.Y += -_minY;
                }
            }

            if (ModelWidth <= 0 || ModelHeight <= 0)
                return;

            if (ModelWidth > ModelHeight)
            {
                _defaultZoom = (_width - 20) / ModelWidth;
                if (ModelHeight * _defaultZoom > _height - 20)
                    _defaultZoom = (_height - 20) / ModelHeight;
            }
            else
            {
                _defaultZoom = _height / ModelHeight;
                if (ModelWidth * _defaultZoom > _width - 20)
                    _defaultZoom = (_height - 20) / ModelHeight;
            }

            if (_defaultZoom > _zoom || _firstLoad)
                _zoom = _defaultZoom;
        }

        private int ConvertColor(System.Windows.Media.Color color)
        {
            var a = color.A + 1;
            var col = (color.A << 24)
                     | ((byte)((color.R * a) >> 8) << 16)
                     | ((byte)((color.G * a) >> 8) << 8)
                     | ((byte)((color.B * a) >> 8));
            return col;
        }

        public void SetOffset(double offsetX, double offsetY)
        {
            _offsetX = offsetX;
            _offsetY = offsetY;
        }

        public void SetZoom(double zoom)
        {
            _zoom = _zoom * zoom;
        }

        public void SetScale(double scale)
        {
            _zoom = scale;
        }

        private readonly List<int> _pointsOfLine = new List<int>();
        private readonly List<int> _colors = new List<int>();

        public bool Draw()
        {
            lock (_waitToFinishLoadingDrawing)
            {
                if (!_waitToFinishLoadingDrawing.WaitOne(0, false))
                    return false;

                _waitToFinishLoadingDrawing.Reset();
            }

            try
            {
                if (_writeableBitmap == null)
                    return false;

                //count = 0;
                _writeableBitmap.Lock();
                _writeableBitmap.Clear();
                _pointsOfLine.Clear();
                _colors.Clear();

                //Draw lines
                foreach (var line in _lines)
                {
                    int X1 = Convert.ToInt32(line.X1 * _zoom + _offsetX);
                    int Y1 = Convert.ToInt32(line.Y1 * _zoom + _offsetY);
                    int X2 = Convert.ToInt32(line.X2 * _zoom + _offsetX);
                    int Y2 = Convert.ToInt32(line.Y2 * _zoom + _offsetY);

                    if ((X1 < 0 && X2 < 0) || (X1 > _width && X2 > _width))
                        continue;

                    if ((Y1 < 0 && Y2 < 0) || (Y1 > _height && Y2 > _height))
                        continue;

                    _pointsOfLine.Add(X1);
                    _pointsOfLine.Add(Y1);
                    _pointsOfLine.Add(X2);
                    _pointsOfLine.Add(Y2);
                    _colors.Add(line.color);
                    //count++;
                }

                //draw Arcs and Ellipses
                foreach (var arc in _arcs)
                {
                    double startAngle = arc.StartAngle;
                    double endAngle = arc.EndAngle;

                    if (startAngle < 0)
                        startAngle = PI2 + startAngle;

                    if (endAngle < 0)
                        endAngle = PI2 + endAngle;

                    double angle = startAngle;
                    double cx = arc.X * _zoom;
                    double cy = arc.Y * _zoom;
                    double rx = arc.radiusX * _zoom;
                    double ry = arc.radiusY * _zoom;

                    if ((cx + rx + _offsetX < 0) || (cx - rx) + _offsetX > _width)
                        continue;

                    if ((cy + ry + _offsetY < 0) || (cy - ry + _offsetY > _height))
                        continue;

                    //draw center point of arc
                    if (startAngle > endAngle)
                    {
                        while (angle < PI2)
                        {
                            _colors.Add(arc.color);
                            _pointsOfLine.Add(Convert.ToInt32(cx + (Math.Cos(angle) * rx) + _offsetX));
                            _pointsOfLine.Add(Convert.ToInt32(cy - (Math.Sin(angle) * ry) + _offsetY));
                            angle += 0.05;
                            if (angle < PI2)
                            {
                                _pointsOfLine.Add(Convert.ToInt32(cx + (Math.Cos(angle) * rx) + _offsetX));
                                _pointsOfLine.Add(Convert.ToInt32(cy - (Math.Sin(angle) * ry) + _offsetY));
                            }
                            else
                            {
                                _pointsOfLine.Add(Convert.ToInt32(cx + (Math.Cos(PI2) * rx) + _offsetX));
                                _pointsOfLine.Add(Convert.ToInt32(cy - (Math.Sin(PI2) * ry) + _offsetY));
                            }
                        }
                        angle = 0;
                    }

                    while (angle < endAngle)
                    {
                        _colors.Add(arc.color);
                        _pointsOfLine.Add(Convert.ToInt32(cx + (Math.Cos(angle) * rx) + _offsetX));
                        _pointsOfLine.Add(Convert.ToInt32(cy - (Math.Sin(angle) * ry) + _offsetY));
                        angle += 0.05;
                        if (angle < endAngle)
                        {
                            _pointsOfLine.Add(Convert.ToInt32(cx + (Math.Cos(angle) * rx) + _offsetX));
                            _pointsOfLine.Add(Convert.ToInt32(cy - (Math.Sin(angle) * ry) + _offsetY));
                        }
                    }
                    if (angle < endAngle)
                    {
                        _pointsOfLine.Add(Convert.ToInt32(cx + (Math.Cos(endAngle) * rx) + _offsetX));//angle
                        _pointsOfLine.Add(Convert.ToInt32(cy - (Math.Sin(endAngle) * ry) + _offsetY));
                    }
                    //add last point
                    if (_pointsOfLine.Count % 4 > 0)
                    {
                        _pointsOfLine.Add(Convert.ToInt32(cx + (Math.Cos(endAngle) * rx) + _offsetX));
                        _pointsOfLine.Add(Convert.ToInt32(cy - (Math.Sin(endAngle) * ry) + _offsetY));
                    }
                }

                //draw all lines, arcs, ellipses into writablebitmap
                WriteableBitmapExtensions.DrawLines(_writeableBitmap, _pointsOfLine, _colors);

                ////Draw Circles
                //foreach (Circle circle in _Circles)
                //{
                //    if (circle.X*_zoom + _offsetX + circle.radius*_zoom > 2000000 ||
                //        circle.X*_zoom + _offsetX - circle.radius*_zoom < -2000000
                //        || -circle.Y*_zoom + _offsetY + circle.radius*_zoom > 2000000 ||
                //        -circle.Y*_zoom + _offsetY - circle.radius*_zoom < -2000000)
                //        continue;

                //    int xc = Convert.ToInt32(circle.X*_zoom + _offsetX);
                //    int yc = Convert.ToInt32(circle.Y*_zoom + _offsetY);
                //    int r = Convert.ToInt32(circle.radius*_zoom);

                //    if ((xc + r < 0) || (xc - r) > _width)
                //        continue;

                //    if ((yc + r < 0) || (yc - r > _height))
                //        continue;

                //    WriteableBitmapExtensions.DrawEllipseCentered(_wbm, xc, yc, r, r,
                //        ConvertColor(circle.color));
                //    count++;
                //}

                _writeableBitmap.Unlock();
                return true;
            }
            catch
            {
                if (_writeableBitmap != null)
                {
                    _writeableBitmap.Unlock();
                }

                return false;
            }
            finally
            {
                _waitToFinishLoadingDrawing.Set();
            }
        }

        public byte[] SaveToArray()
        {
            if (_lines.Count == 0 && _arcs.Count == 0)
                return null;

            var stream = new MemoryStream();
            var bFormatter = new BinaryFormatter();

            var modelInfo = new ModelInfo();
            modelInfo.modelHeight = ModelHeight;
            modelInfo.modelWidth = ModelWidth;
            bFormatter.Serialize(stream, modelInfo);

            foreach (var line in _lines)
                bFormatter.Serialize(stream, line);

            foreach (var arc in _arcs)
                bFormatter.Serialize(stream, arc);

            stream.Close();
            return stream.ToArray();
        }

        public void LoadFromArray(byte[] data)
        {
            if (data == null)
                return;

            _firstLoad = true;
            _lines.Clear();
            _arcs.Clear();
            var stream = new MemoryStream(data);

            while (stream.Position != stream.Length)
            {
                var bFormatter = new BinaryFormatter();
                var objectToSerialize = bFormatter.Deserialize(stream);

                if (objectToSerialize is ModelInfo)
                {
                    ModelWidth = (objectToSerialize as ModelInfo).modelWidth;
                    ModelHeight = (objectToSerialize as ModelInfo).modelHeight;
                }

                if (objectToSerialize is Line)
                    _lines.Add(objectToSerialize as Line);

                if (objectToSerialize is Arc)
                    _arcs.Add(objectToSerialize as Arc);
            }

            stream.Close();
            SetOptimalZoom();
            _firstLoad = false;
        }
    }

    public class Entity
    {
        public DxfEntity CadEntity { get; set; }
        public DxfInsert CadInsert { get; set; }
    }

    [Serializable()]
    public class Line
    {
        public double X1 { get; set; }
        public double Y1 { get; set; }
        public double X2 { get; set; }
        public double Y2 { get; set; }
        public int color { get; set; }
    }

    [Serializable()]
    public class Arc
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double radiusX { get; set; }
        public double radiusY { get; set; }
        public double StartAngle { get; set; }
        public double EndAngle { get; set; }
        public int color { get; set; }
    }

    [Serializable]
    public class ModelInfo
    {
        public double modelWidth;
        public double modelHeight;
    }
}