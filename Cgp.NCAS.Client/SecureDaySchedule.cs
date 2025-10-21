using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Contal.Cgp.NCAS.Server.Beans;


namespace Contal.Cgp.Client
{
    class SecureDaySchedule : Control
    {
        const int _minutesX = 60;
        const int _hoursY = 24;
        const int _sizeX = 8;
        const int _sizeY = 8;

        int _shiftX = 20;
        int _shiftY = 20;

        int _allSizeX = (_minutesX * _sizeX) + 20;
        int _allSizeY = (_hoursY * _sizeY) + 40;

        int _mouseDownX;
        int _mouseDownY;
        int _mouseUpX;
        int _mouseUpY;
        int _mouseActX = 0;
        int _mouseActY = 0;
        Color _mouseDownColor;

        int _positionStart;
        int _positionEnd;
        int _positionLastEnd;

        bool _isDown  = false;

        bool[] _minutes = new bool[1440];
        int _minMinutes = 0;
        int _maxMinutes = 1439;

        private Color _colorLeft = Color.LightGreen;
        private Color _colorRight = Color.Red;
            //Pink;

        public event Contal.IwQuick.DFromTT2Void<object,EventArgs> WasChanged;
        public event Contal.IwQuick.DFromTToVoid<bool[]> HoursWasChanged;

        ToolTip tTip = new ToolTip();

        public SecureDaySchedule()
        {
            AutoSize = true;

            Graphics grfx = CreateGraphics();
            float xDpi = (int)grfx.DpiX;
            float yDpi = (int)grfx.DpiY;
            double sizeDpi;
            sizeDpi = xDpi / 96.0;
            _shiftX = (int)(_shiftX * sizeDpi);
            sizeDpi = yDpi / 96.0;
            _shiftY = (int)(_shiftY * sizeDpi);
            _allSizeX = (_minutesX * _sizeX) + _shiftX;
            _allSizeY = (_hoursY * _sizeY) + _shiftY * 2;

            tTip.SetToolTip(this, " ");

        }

        public void DrawControl()
        {
            Graphics grfx = CreateGraphics();
            Brush normal = new SolidBrush(Color.Gray);
            Brush br = new SolidBrush(Color.Black);
            Brush rectangle = new SolidBrush(_colorRight);
            Pen myPen = new Pen(normal);
            Pen darkPen = new Pen(br);
            Font myFont = new System.Drawing.Font("Helvetica", 10, FontStyle.Regular);

            grfx.DrawString("00:00", myFont, br, 2, 2);
            grfx.DrawString(" 6", myFont, br, 2, 59);
            grfx.DrawString("12", myFont, br, 2, 108);
            grfx.DrawString("18", myFont, br, 2, 156);
            //grfx.DrawString("hours", myFont, br, 2, _allSizeY - _shiftY);

            grfx.DrawString("10", myFont, br, 92, 2);
            grfx.DrawString("20", myFont, br, 172, 2);
            grfx.DrawString("30", myFont, br, 252, 2);
            grfx.DrawString("40", myFont, br, 332, 2);
            grfx.DrawString("50", myFont, br, 412, 2);
            //GetString("ErrorInsertGroupName");
            //grfx.DrawString("minutes", myFont, br, _allSizeX - 48, 2);
            //grfx.DrawString((i).ToString(), myFont, br, (i * _sizeWidth) + 4, 2);

            grfx.FillRectangle(rectangle, new Rectangle(1 + _shiftX, 1 + _shiftY, (_sizeY * _minutesX) - 1, (_sizeX * _hoursY) - 1));

            for (int i = 0; i <= _minutesX; i++)
            {
                if ((i % 10) == 0)
                    grfx.DrawLine(darkPen, (i * _sizeX) + _shiftX, 0 + _shiftY, (i * _sizeX) + _shiftX, _sizeY * _hoursY + _shiftY);
                else
                    grfx.DrawLine(myPen, (i * _sizeX) + _shiftX, 0 + _shiftY, (i * _sizeX) + _shiftX, _sizeY * _hoursY + _shiftY);
            }

            for (int i = 0; i <= _hoursY; i++)
            {
                if ((i % 6) == 0)
                {
                    grfx.DrawLine(darkPen, 0 + _shiftX, i * _sizeY + _shiftY, (_minutesX * _sizeX) + _shiftX, i * _sizeY + _shiftY);
                }
                else
                    grfx.DrawLine(myPen, 1 + _shiftX, i * _sizeY + _shiftY, (_minutesX * _sizeX) + _shiftX, i * _sizeY + _shiftY);
            }

            //DrawOrigin();
            DrawTrueOrigin();

            br.Dispose();
            normal.Dispose();
            rectangle.Dispose();
            myPen.Dispose();
            darkPen.Dispose();
            myFont.Dispose();
            grfx.Dispose();
        }

        private void DrawSquare(int positionX, int positionY, Color usedColor)
        {
            Graphics grfx = CreateGraphics();
            Brush normal = new SolidBrush(usedColor);
            grfx.FillRectangle(normal, new Rectangle(positionX + 1 + _shiftX, positionY + 1 + _shiftY, _sizeY-1, _sizeX-1));
            grfx.Dispose();
            normal.Dispose();
        }

        protected override void OnPaint(PaintEventArgs args)
        {
            DrawControl();
        }

        // Required for autosizing.
        public override Size GetPreferredSize(Size sz)
        {
            return new Size(_allSizeX + 1, _allSizeY +1);
        }
        
        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (e.X < _shiftX || e.Y < _shiftY) return;
            if (e.Y > (_hoursY * _sizeY + _shiftY)) return;
                // || e.Y > _hoursY * _sizeY + _posunY)
            
            if (WasChanged != null)
            {
                try
                { WasChanged(null,null); }
                catch
                { }
            }
            
            int x = CalculatePositionX(e.X);
            int y = CalculatePositionY(e.Y);

            _mouseActX = x;
            _mouseActY = y;

            if (e.Button == MouseButtons.Left)
                DrawSquare(x * _sizeX, y * _sizeY, _colorLeft);
            else
                DrawSquare(x * _sizeX, y * _sizeY, _colorRight);

            SelectedHours();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.X < _shiftX || e.Y < _shiftY) return;
            if (e.Y > (_hoursY * _sizeY + _shiftY)) return;

            if (WasChanged != null)
            {
                try
                { WasChanged(null,null); }
                catch
                { }
            }

            _isDown = true;
            if (e.Button == MouseButtons.Left)
                _mouseDownColor = _colorLeft;
            else
                _mouseDownColor = _colorRight;
            //_mouseDownX = e.X / _sizeX;
            //_mouseDownY = e.Y / _sizeY;
            _mouseDownX = CalculatePositionX(e.X);
            _mouseDownY = CalculatePositionY(e.Y);

            _mouseActX = _mouseDownX;
            _mouseActY = _mouseDownY;

            _positionStart = _mouseDownX + _mouseDownY*_minutesX;
            _positionLastEnd = -1;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            _isDown = false;
            _mouseUpX = CalculatePositionX(e.X);
            _mouseUpY = CalculatePositionY(e.Y);
            _positionEnd = _mouseUpX + _mouseUpY * _minutesX;
            SaveSquare(_positionStart, _positionEnd, (e.Button == MouseButtons.Left));
            SelectedHours();
        }

        protected override void OnMouseMove(MouseEventArgs args)
        {
            if (_mouseActX != CalculatePositionX(args.X) || _mouseActY != CalculatePositionY(args.Y))
            {
                bool show = true;
                if (args.X < _shiftX || args.Y < _shiftY) show = false;
                if (args.Y > (_hoursY * _sizeY + _shiftY)) show = false;

                if (show)
                {
                    _mouseActX = CalculatePositionX(args.X);
                    _mouseActY = CalculatePositionY(args.Y);

                    string hour;
                    if (_mouseActY < 10)
                    {
                        hour = "0" + _mouseActY.ToString();
                    }
                    else
                    {
                        hour = _mouseActY.ToString();
                    }
                    if (_mouseActX < 10)
                    {
                        hour += ":0" + _mouseActX.ToString();
                    }
                    else
                    {
                        hour += ":" + _mouseActX.ToString();
                    }
                    tTip.Active = true;
                    tTip.SetToolTip(this, hour);
                }
                else
                {
                    tTip.Active = false;
                }
            }

            if (_isDown)
            {
                _positionEnd = CalculatePositionX(args.X) + CalculatePositionY(args.Y) * _minutesX;
                //corection
                if (_positionLastEnd != -1)
                {
                    if (_positionStart < _positionEnd)
                    {
                        if (_positionLastEnd > _positionEnd)
                        {
                            DrawOrigin(_positionEnd, _positionLastEnd);
                        }
                        if (_positionLastEnd < _positionStart)
                        {
                            DrawOrigin(_positionLastEnd, _positionStart);
                        }

                    }
                    else if (_positionStart > _positionEnd)
                    {
                        if (_positionLastEnd < _positionEnd)
                        {
                            DrawOrigin(_positionLastEnd, _positionEnd);
                        }
                        if (_positionLastEnd > _positionStart)
                        {
                            DrawOrigin(_positionStart, _positionLastEnd);
                        }
                    }
                    else if (_positionStart == _positionEnd)
                    {
                        if (_positionStart < _positionLastEnd)
                            DrawOrigin(_positionStart + 1, _positionLastEnd);
                        else
                            DrawOrigin(_positionLastEnd, _positionStart - 1);
                    }
                }
                _positionLastEnd = _positionEnd;
                DrawSelected();
            }
        }

        private int CalculatePositionX(int positionX)
        {
            if (positionX <= _shiftX)
                return 0;
            if (positionX >= (_minutesX * _sizeX + _shiftX))
                return ((_minutesX * _sizeX) / _sizeX) - 1;
            return (positionX - _shiftX) / _sizeX;
        }

        private int CalculatePositionY(int positionY)
        {
            if (positionY <= 0 || positionY < _shiftY)
                return 0;
            if (positionY >= (_hoursY * _sizeY + _shiftY))
                return ((_hoursY * _sizeY) / _sizeY) - 1;
            return (positionY - _shiftY) / _sizeY;
        }

        private void GetPosition(int cislo, out int x, out int y)
        {
            int c2 = cislo / _minutesX;
            int c1 = cislo - (c2 * _minutesX);
            x = c1 * _sizeX;
            y = c2 * _sizeY;
        }

        private void DrawSelected()
        {
            int min;
            int max;

            if (_positionEnd < _positionStart)
            {
                min = _positionEnd;
                max = _positionStart;
            }
            else
            {
                min = _positionStart;
                max = _positionEnd;
            }

            int x;
            int y;

            for (int i = min; i <= max; i++)
            {
                GetPosition(i, out  x, out y);
                DrawSquare(x,y, _mouseDownColor);
            }
        }

        private void DrawSelected(int fromPoint, int toPoint, Color usedColor)
        {
            int x;
            int y;

            for (int i = fromPoint; i <= toPoint; i++)
            {
                GetPosition(i, out  x, out y);
                DrawSquare(x, y, usedColor);
            }
        }

        private void DrawOrigin()
        {
            int x, y;
            for (int i = _minMinutes; i <= _maxMinutes; i++)
            {
                GetPosition(i, out  x, out y);
                if (_minutes[i])
                    DrawSquare(x, y, _colorLeft);
                else
                    DrawSquare(x, y, _colorRight);
            }
        }

        private void DrawTrueOrigin()
        {
            int x, y;
            for (int i = _minMinutes; i <= _maxMinutes; i++)
            {
                if (_minutes[i])
                {
                    GetPosition(i, out  x, out y);
                    DrawSquare(x, y, _colorLeft);
                }
            }
        }

        private void DrawOrigin(int from, int to)
        {
            if (from < _minMinutes)
                from = _minMinutes;
            if (to > _maxMinutes)
                to = _maxMinutes;

            int x, y;
            for (int i = from; i <= to; i++)
            {
                GetPosition(i, out  x, out y);
                if (_minutes[i])
                    DrawSquare(x, y, _colorLeft);
                else
                    DrawSquare(x, y, _colorRight);
            }
        }

        private void SaveSquare(int from, int to, bool value)
        {
            int min;
            int max;

            if (from < to)
            {
                min = from;
                max = to;
            }
            else
            {
                min = to;
                max = from;
            }

            if (min < _minMinutes)
            {
                min = _minMinutes;
            }
            if (max > _maxMinutes)
            {
                max = _maxMinutes;
            }

            for (int i = min; i <= max; i++)
            {
                _minutes[i] = value;
            }
        }


        public void PaintHour(int _hour, MouseButtons button)
        {
            if (button == MouseButtons.Left)
            {
                DrawSelected(_hour * 60, (_hour * 60) + 59, _colorLeft);
                SaveSquare(_hour * 60, (_hour * 60) + 59, true);
            }
            else
            {
                DrawSelected(_hour * 60, (_hour * 60) + 59, _colorRight);
                SaveSquare(_hour * 60, (_hour * 60) + 59, false);
            }
        }
                
        #region Save
        public List<SecurityDayInterval> GetIntervals()
        {
            bool last = false;
            int intervalStart = 0;
            int intervalEnd;

            SecurityDayInterval tmpDayInterval;
            List<SecurityDayInterval> intervals = new List<SecurityDayInterval>();

            last = _minutes[0];

            for (int i = _minMinutes; i <= _maxMinutes; i++)
            {
                if (_minutes[i])
                {
                    if (!last)
                    {
                        last = true;
                        intervalStart = i;
                    }
                    if (i == _maxMinutes)
                    {
                        //intervalEnd = _maxMinutes + 1;
                        intervalEnd = _maxMinutes;

                        tmpDayInterval = new SecurityDayInterval();
                        tmpDayInterval.MinutesFrom = (short) intervalStart;
                        tmpDayInterval.MinutesTo = (short) intervalEnd;
                        intervals.Add(tmpDayInterval);
                    }
                }
                else
                {
                    if (last)
                    {
                        last = false;
                        intervalEnd = i - 1;

                        tmpDayInterval = new SecurityDayInterval();
                        tmpDayInterval.MinutesFrom = (short)intervalStart;
                        tmpDayInterval.MinutesTo = (short)intervalEnd;
                        intervals.Add(tmpDayInterval);
                    }
                }
            }

            return intervals;
        }

        public void SetIntervals(ICollection<SecurityDayInterval> intervals)
        {
            //clear all values
            SaveSquare(_minMinutes, _maxMinutes, false);
            if (intervals == null) return;
            //set from intervals
            foreach (SecurityDayInterval part in intervals)
            {
                SaveSquare(part.MinutesFrom, part.MinutesTo, true);
            }
            //draw new values
            DrawOrigin();
            SelectedHours();
        }

        public void SelectedHours()
        {
            if (HoursWasChanged != null)
            {
                bool[] ok = new bool[24];
                for (int i = 0; i < 24; i++)
                {
                    ok[i] = true;
                }

                for (int i = 0; i <= _maxMinutes; i++)
                {
                    if (!_minutes[i])
                    {
                        ok[i / 60] = false;
                        if ((i / 60) == 23) break;
                        i = (i / 60 + 1) * 60 - 1;
                    }
                }

                try
                {
                    HoursWasChanged(ok);
                }
                catch { }
            }
        }
        #endregion
    }
}
