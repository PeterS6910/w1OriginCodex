using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

namespace Contal.IwQuick.UI
{
    public enum SelectionType
    {
        Vertical = 0,
        Multiselection = 1
    }

    [ToolboxItem(false)]
    class DayMatrixSchedule : Control
    {
        const int MINUTES_COUNT_X = 60;
        const int HOURS_COUNT_Y = 24;
        const int RESIZE_ADJUST = 60;

        int INFO_SIZE_X = 20;
        int INFO_SIZE_Y = 20;

        int _allSizeX;
        int _allSizeY;
        int _legendStepX = 10;
        int _legendStepY = 6;

        DayPlan _dayPlan;

        internal DayPlan DayPlan
        {
            get { return _dayPlan; }
            set { _dayPlan = value; }
        }

        public event Action<object, EventArgs> WasChanged;
        public event Action<byte[]> HoursWasChanged;

        public DayMatrixSchedule()
        {
            AutoSize = true;
            _dayPlan = new DayPlan();

            Graphics grfx = CreateGraphics();
            float xDpi = (int)grfx.DpiX;
            float yDpi = (int)grfx.DpiY;
            double sizeDpi;
            sizeDpi = xDpi / 96.0;
            INFO_SIZE_X = (int)(INFO_SIZE_X * sizeDpi);
            sizeDpi = yDpi / 96.0;
            INFO_SIZE_Y = (int)(INFO_SIZE_Y * sizeDpi);
            _allSizeX = (MINUTES_COUNT_X * _dayPlan.SizeX) + INFO_SIZE_X;
            _allSizeY = (HOURS_COUNT_Y * _dayPlan.SizeY) + INFO_SIZE_Y * 2;
            grfx.Dispose();

            _dayPlan.Location = new Point(INFO_SIZE_X, INFO_SIZE_Y);
            _dayPlan.Parent = this;

            _dayPlan.WasChanged += DayScheduleWasChanged;
            _dayPlan.HoursWasChanged += DayScheduleHoursWasChanged;
        }

        public DayMatrixSchedule(double[] sizeDpiXY)
        {
            AutoSize = true;
            _dayPlan = new DayPlan();

            INFO_SIZE_X = (int)(INFO_SIZE_X * sizeDpiXY[0]);
            INFO_SIZE_Y = (int)(INFO_SIZE_Y * sizeDpiXY[1]);
            _allSizeX = (MINUTES_COUNT_X * _dayPlan.SizeX) + INFO_SIZE_X;
            _allSizeY = (HOURS_COUNT_Y * _dayPlan.SizeY) + INFO_SIZE_Y * 2;

            _dayPlan.Location = new System.Drawing.Point(INFO_SIZE_X, INFO_SIZE_Y);
            _dayPlan.Parent = this;
            _dayPlan.WasChanged += DayScheduleWasChanged;
            _dayPlan.HoursWasChanged += DayScheduleHoursWasChanged;
        }

        public void ResizeMatrix(int width, int height)
        {

            _dayPlan.ResizeMatrix(width - RESIZE_ADJUST, height - RESIZE_ADJUST);
            _allSizeX = (MINUTES_COUNT_X * _dayPlan.SizeX) + INFO_SIZE_X;
            _allSizeY = (HOURS_COUNT_Y * _dayPlan.SizeY) + INFO_SIZE_Y * 2;
            this.Width = width;
            this.Height = height;
            _dayPlan.DrawWholeDailyPlanBitmapOnly();
        }

        public void SetLegendStep(int stepX, int stepY)
        {
            _legendStepX = stepX;
            _legendStepY = stepY;
        }

        public void DayScheduleWasChanged(object sender, EventArgs e)
        {
            if (WasChanged != null)
                try
                {
                    WasChanged(sender, e);
                }
                catch
                {
                    
                }
        }

        public void DayScheduleHoursWasChanged(byte[] arrayHours)
        {
            if (null != HoursWasChanged)
                try { HoursWasChanged(arrayHours); }
                catch { }
        }

        protected override void OnPaint(PaintEventArgs args)
        {
            DrawControl();
        }

        public override Size GetPreferredSize(Size sz)
        {
            return new Size(_allSizeX + 1, _allSizeY + 1);
        }

        public void DrawControl()
        {
            Graphics grfx = CreateGraphics();
            Brush normal = new SolidBrush(Color.Gray);
            Brush br = new SolidBrush(Color.Black);
            Pen myPen = new Pen(normal);
            Pen darkPen = new Pen(br);
            Font myFont = new System.Drawing.Font("Helvetica", 10, FontStyle.Regular);


            grfx.DrawString("00:00", myFont, br, 2, 2);
            for (int i = 0; i < (int)(60 / _legendStepX) - 1; i++)
            {
                grfx.DrawString((_legendStepX * (i + 1)).ToString(), myFont, br, 12 + (_dayPlan.SizeX * _legendStepX * (i + 1)), 2);
            }

            for (int j = 0; j < (int)(24 / _legendStepY) - 1; j++)
            {
                grfx.DrawString((_legendStepY * (j + 1)).ToString(), myFont, br, 2, 12 + (_dayPlan.SizeY * _legendStepY * (j + 1)));
            }

            br.Dispose();
            normal.Dispose();
            myPen.Dispose();
            darkPen.Dispose();
            myFont.Dispose();
            grfx.Dispose();
        }

        public void PaintHour(int hour, MouseButtons mouseButton)
        {
            _dayPlan.PaintHour(hour, mouseButton);
        }

        public void SetLeftClickColor(int index)
        {
            _dayPlan.SetLeftClickColor(index);
        }

        public void SetRightClickColor(int index)
        {
            _dayPlan.SetRightClickColor(index);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // DaySchedule
            // 
            this.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ResumeLayout(false);
        }
    }

    class DayPlan : Control
    {
        private Color[] _matrixColors;
        public Bitmap _backBuffer;

        public const int MIN_MINUTES = 0;
        public const int MAX_MINUTES = 1439;
        const int MINUTES_COUNT_X = 60;
        const int HOURS_COUNT_Y = 24;
        const int SIZE_PLAN_X = 10;
        const int SIZE_PLAN_Y = 10;

        int _sizeX = SIZE_PLAN_X;
        int _sizeY = SIZE_PLAN_Y;
        int _allSizeX;
        int _allSizeY;

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

        bool _isEditable = true;
        SelectionType _selectionType = SelectionType.Vertical;

        bool _isDown;
        readonly byte[] _minutes = new byte[1440];

        private Color _colorLeft = Color.LightGreen;
        private Color _colorRight = Color.Red;
        private int _defaultColorIndex = 0;

        private byte _numberLeft;
        private byte _numberRight;

        private readonly ToolTip _toolTipTime = new ToolTip();
        public event Action<object, EventArgs> WasChanged;
        public event Action<byte[]> HoursWasChanged;

        public Dictionary<byte, string> Localization { get; set; }

        public SelectionType SelectionType
        {
            get { return _selectionType; }
            set { _selectionType = value; }
        }

        public bool IsEditable
        {
            get { return _isEditable; }
            set { _isEditable = value; }
        }

        public Color[] MatrixColors
        {
            get { return _matrixColors; }
            set { _matrixColors = value; }
        }

        public void SetLeftClickColor(int index)
        {
            _colorLeft = _matrixColors[index];
            _numberLeft = (byte)index;
        }

        public void InitMinutes(int index)
        {
            for (int i = 0; i < _minutes.Length; i++)
            {
                _minutes[i] = (byte)index;
            }
        }

        public void SetRightClickColor(int index)
        {
            _colorRight = _matrixColors[index];
            _numberRight = (byte)index;
        }

        public int SizeX
        {
            get { return _sizeX; }
            set { _sizeX = value; }
        }

        public int SizeY
        {
            get { return _sizeY; }
            set { _sizeY = value; }
        }

        public int DefaultColorIndex
        {
            get { return _defaultColorIndex; }
            set { _defaultColorIndex = value; }
        }

        public DayPlan()
        {
            AutoSize = true;
            Cursor = Cursors.Hand;
            _allSizeX = MINUTES_COUNT_X * _sizeX;
            _allSizeY = HOURS_COUNT_Y * _sizeY;
            _toolTipTime.SetToolTip(this, " ");
        }

        private void InitBitMap()
        {
            if (_sizeX == 0)
            {
                _sizeX = SIZE_PLAN_X;
                _sizeY = SIZE_PLAN_Y;
            }
            _allSizeX = (MINUTES_COUNT_X * _sizeX);
            _allSizeY = (HOURS_COUNT_Y * _sizeY);
            _backBuffer = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);
            Graphics grfx = Graphics.FromImage(_backBuffer);

            Brush normal = new SolidBrush(Color.Gray);
            Brush br = new SolidBrush(Color.Black);
            Brush rectangle = new SolidBrush(_colorRight);
            Pen myPen = new Pen(normal);
            Pen darkPen = new Pen(br);

            for (int i = 0; i <= MINUTES_COUNT_X; i++)
            {
                if ((i % 10) == 0)
                    grfx.DrawLine(darkPen, (i * _sizeX), 0, (i * _sizeX), _sizeY * HOURS_COUNT_Y);
                else
                    grfx.DrawLine(myPen, (i * _sizeX), 0, (i * _sizeX), _sizeY * HOURS_COUNT_Y);
            }

            for (int i = 0; i <= HOURS_COUNT_Y; i++)
            {
                if ((i % 6) == 0)
                {
                    grfx.DrawLine(darkPen, 0, i * _sizeY, (MINUTES_COUNT_X * _sizeX), i * _sizeY);
                }
                else
                    grfx.DrawLine(myPen, 1, i * _sizeY, (MINUTES_COUNT_X * _sizeX), i * _sizeY);
            }

            br.Dispose();
            normal.Dispose();
            rectangle.Dispose();
            myPen.Dispose();
            darkPen.Dispose();
            grfx.Dispose();
        }

        public void DrawControl()
        {
            InitBitMap();
            DrawWholeDailyPlanBitmapOnly();
        }

        private void DrawSquare(int positionX, int positionY, Color usedColor)
        {
            lock (_syncObj)
            {
                Graphics grfx = CreateGraphics();
                Brush normal = new SolidBrush(usedColor);
                grfx.FillRectangle(normal, new Rectangle(positionX + 1, positionY + 1, _sizeX - 1, _sizeY - 1));
                grfx.Dispose();

                if (_backBuffer == null)
                {
                    InitBitMap();
                }
                grfx = Graphics.FromImage(_backBuffer);
                grfx.FillRectangle(normal, new Rectangle(positionX + 1, positionY + 1, _sizeX - 1, _sizeY - 1));
                grfx.Dispose();
                normal.Dispose();
            }
        }

        public void DrawSquareBitmapOnly(int positionX, int positionY, Color usedColor)
        {
            if (_backBuffer == null)
            {
                InitBitMap();
            }
            Graphics grfx = Graphics.FromImage(_backBuffer);
            Brush normal = new SolidBrush(usedColor);
            grfx.FillRectangle(normal, new Rectangle(positionX + 1, positionY + 1, _sizeX - 1, _sizeY - 1));
            grfx.Dispose();
            normal.Dispose();
        }

        protected override void OnPaint(PaintEventArgs args)
        {
            if (_backBuffer == null)
                DrawControl();

            Graphics g = Graphics.FromImage(_backBuffer);
            g.Dispose();
            args.Graphics.DrawImageUnscaled(_backBuffer, 0, 0);
        }

        // Required for autosizing.
        public override Size GetPreferredSize(Size sz)
        {
            return new Size(_allSizeX + 1, _allSizeY + 1);
        }

        public void ResizeMatrix(int width, int height)
        {
            if (((double)width / (double)MINUTES_COUNT_X) >= 1
                && ((double)height / (double)HOURS_COUNT_Y) >= 1)
            {
                _sizeX = Convert.ToInt32((double)width / (double)MINUTES_COUNT_X);
                _sizeY = Convert.ToInt32((double)height / (double)HOURS_COUNT_Y);
                _allSizeX = (MINUTES_COUNT_X * _sizeX);
                _allSizeY = (HOURS_COUNT_Y * _sizeY);
            }
            this.Size = new Size(_allSizeX + 3, _allSizeY + 3);
            InitBitMap();
            Graphics g = Graphics.FromImage(_backBuffer);
            g.Dispose();
            this.CreateGraphics().DrawImageUnscaled(_backBuffer, 0, 0);
        }

        #region Mouse Events
        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (!_isEditable) return;

            if (e.Y > (HOURS_COUNT_Y * _sizeY)) return;

            if (WasChanged != null)
            {
                try
                { WasChanged(null, null); }
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
            if (!_isEditable) return;

            if (WasChanged != null)
            {
                try
                { WasChanged(null, null); }
                catch
                { }
            }

            _isDown = true;
            if (e.Button == MouseButtons.Left)
                _mouseDownColor = _colorLeft;
            else
                _mouseDownColor = _colorRight;
            _mouseDownX = CalculatePositionX(e.X);
            _mouseDownY = CalculatePositionY(e.Y);

            _mouseActX = _mouseDownX;
            _mouseActY = _mouseDownY;

            _positionStart = _mouseDownX + _mouseDownY * MINUTES_COUNT_X;
            _positionLastEnd = -1;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (!_isEditable) return;

            _isDown = false;
            _previouslyDrowedIndexes.Clear();
            _mouseUpX = CalculatePositionX(e.X);
            _mouseUpY = CalculatePositionY(e.Y);
            _positionEnd = _mouseUpX + _mouseUpY * MINUTES_COUNT_X;
            if (e.Button == MouseButtons.Left)
                SaveSquare(_positionStart, _positionEnd, _numberLeft);
            else
                SaveSquare(_positionStart, _positionEnd, _numberRight);

            SelectedHours();
        }

        protected override void OnMouseMove(MouseEventArgs args)
        {
            if (_mouseActX != CalculatePositionX(args.X) || _mouseActY != CalculatePositionY(args.Y))
            {
                bool show = true;
                if (args.Y > (HOURS_COUNT_Y * _sizeY)) show = false;

                if (show)
                {
                    ShowToolTipTime(args);
                }
                else
                {
                    _toolTipTime.Active = false;
                }
            }

            if (_isDown)
            {
                _positionEnd = CalculatePositionX(args.X) + CalculatePositionY(args.Y) * MINUTES_COUNT_X;
                //corection
                if (_positionLastEnd != -1)
                {
                    if (SelectionType == SelectionType.Multiselection)
                    {
                        //old square repaint is done in DrawSelected metod 
                    }
                    else
                    {
                        DrowOriginVertical();
                    }
                }

                _positionLastEnd = _positionEnd;
                DrawSelected();
            }
        }
        #endregion

        private void ShowToolTipTime(MouseEventArgs args)
        {
            _mouseActX = CalculatePositionX(args.X);
            _mouseActY = CalculatePositionY(args.Y);

            string hour;
            if (_mouseActY < 10)
            {
                hour = "0" + _mouseActY;
            }
            else
            {
                hour = _mouseActY.ToString();
            }
            if (_mouseActX < 10)
            {
                hour += ":0" + _mouseActX;
            }
            else
            {
                hour += ":" + _mouseActX;
            }

            int minutes = _mouseActY*60 + _mouseActX;

            var actualInterval = GetIntervals().
                FirstOrDefault(interval => interval.MinutesFrom <= minutes && interval.MinutesTo >= minutes);

            string intervalLocalization = null;

            if (Localization != null)
                Localization.TryGetValue(actualInterval.Type, out intervalLocalization);

            _toolTipTime.Active = true;

            _toolTipTime.SetToolTip(this, 
                string.IsNullOrEmpty(intervalLocalization)
                ? hour
                : string.Format("{0} ({1})", hour, intervalLocalization));
        }

        private void DrowOriginVertical()
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

        private int CalculatePositionX(int positionX)
        {
            if (positionX <= 0)
                return 0;
            if (positionX >= (MINUTES_COUNT_X * _sizeX))
                return ((MINUTES_COUNT_X * _sizeX) / _sizeX) - 1;
            return (positionX / _sizeX);
        }

        private int CalculatePositionY(int positionY)
        {
            if (positionY <= 0)
                return 0;
            if (positionY >= (HOURS_COUNT_Y * _sizeY))
                return ((HOURS_COUNT_Y * _sizeY) / _sizeY) - 1;
            return (positionY / _sizeY);
        }

        private void GetPositionForMinute(int minuteNumber, out int x, out int y)
        {
            int c2 = minuteNumber / MINUTES_COUNT_X;
            int c1 = minuteNumber - (c2 * MINUTES_COUNT_X);
            x = c1 * _sizeX;
            y = c2 * _sizeY;
        }

        private void AdjustInterval(ref int from, ref int to)
        {
            int pom = 0;
            if (from > to)
            {
                pom = from;
                from = to;
                to = pom;
            }

            if (from < MIN_MINUTES)
            {
                from = MIN_MINUTES;
            }
            if (to > MAX_MINUTES)
            {
                to = MAX_MINUTES;
            }
        }

        private void AdjustModuloInterval(ref int from, ref int to, ref int modFrom, ref int modTo)
        {
            if (modFrom > modTo)
            {
                modFrom = modTo;
                modTo = from % MINUTES_COUNT_X;
                from = from - (modTo - modFrom);
                to = to + (modTo - modFrom);
            }
            if (from < MIN_MINUTES)
            {
                from = MIN_MINUTES;
            }
            if (to > MAX_MINUTES)
            {
                to = MAX_MINUTES;
            }
        }

        private void DrawSelected()
        {
            int from;
            int to;
            if (_positionEnd < _positionStart)
            {
                from = _positionEnd;
                to = _positionStart;
            }
            else
            {
                from = _positionStart;
                to = _positionEnd;
            }

            int x;
            int y;

            if (_selectionType == SelectionType.Multiselection)
            {
                DrawSelectedMultiselection(from, to, _mouseDownColor);
            }
            else
            {
                for (int i = from; i <= to; i++)
                {
                    GetPositionForMinute(i, out  x, out y);
                    DrawSquare(x, y, _mouseDownColor);
                }
            }
        }


        private List<int> _previouslyDrowedIndexes = new List<int>();
        private List<int> _actualDrowedIndexes = new List<int>();

        private void DrawSelectedMultiselection(int from, int to, Color usedColor)
        {
            int x;
            int y;

            int modFrom = from % MINUTES_COUNT_X;
            int modTo = to % MINUTES_COUNT_X;

            AdjustModuloInterval(ref from, ref to, ref modFrom, ref modTo);

            _actualDrowedIndexes.Clear();
            for (int i = from; i <= to; i++)
            {
                if (i % MINUTES_COUNT_X >= modFrom && i % MINUTES_COUNT_X <= modTo)
                {
                    _actualDrowedIndexes.Add(i); //save indexes of last mouse move

                    if (_previouslyDrowedIndexes.Contains(i))
                    {
                        _previouslyDrowedIndexes.Remove(i);
                    }
                    else
                    {
                        GetPositionForMinute(i, out  x, out y);
                        DrawSquare(x, y, _mouseDownColor);
                    }
                }
            }
            foreach (int index in _previouslyDrowedIndexes)
            {
                GetPositionForMinute(index, out  x, out y);
                DrawSquare(x, y, GetColorByType(_minutes[index]));
            }

            _previouslyDrowedIndexes.Clear();
            _previouslyDrowedIndexes.AddRange(_actualDrowedIndexes.GetRange(0, _actualDrowedIndexes.Count));
        }

        private void DrawSelectedHour(int fromPoint, int toPoint, Color usedColor)
        {
            int x;
            int y;

            for (int i = fromPoint; i <= toPoint; i++)
            {
                GetPositionForMinute(i, out  x, out y);
                DrawSquare(x, y, usedColor);
            }
        }
        public void RedrowByDefaultColor()
        {
            int x, y;
            for (int i = MIN_MINUTES; i <= MAX_MINUTES; i++)
            {
                GetPositionForMinute(i, out  x, out y);
                DrawSquare(x, y, GetColorByType((byte)_defaultColorIndex));
            }
        }

        private object _syncObj = new object();

        public void DrawWholeDailyPlanBitmapOnly()
        {
            int x, y;
            for (int i = MIN_MINUTES; i <= MAX_MINUTES; i++)
            {
                GetPositionForMinute(i, out  x, out y);
                DrawSquare(x, y, GetColorByType(_minutes[i]));
            }
        }

        private Color GetColorByType(byte p)
        {
            if (_matrixColors == null)
            {
                return Color.Red;
            }
            return _matrixColors[(int)p];
        }

        private void DrawOrigin(int from, int to)
        {
            AdjustInterval(ref from, ref to);

            int x, y;

            for (int i = from; i <= to; i++)
            {
                GetPositionForMinute(i, out  x, out y);
                DrawSquare(x, y, GetColorByType(_minutes[i]));
            }
        }

        public void SaveSquareSetIntervals(int from, int to, byte value)
        {
            AdjustInterval(ref from, ref to);

            for (int i = from; i <= to; i++)
            {
            _minutes[i] = value;
            }
           
        }

        public void SaveSquare(int from, int to, byte value)
        {
            AdjustInterval(ref from, ref to);

            if (_selectionType == SelectionType.Multiselection)
            {
                SaveSquareMultiselection(value, from, to);
            }
            else
            {
                SaveSquareVertical(value, from, to);
            }
        }

        private void SaveSquareMultiselection(byte value, int from, int to)
        {
            int modFrom = from % MINUTES_COUNT_X;
            int modTo = to % MINUTES_COUNT_X;

            AdjustModuloInterval(ref from, ref to, ref modFrom, ref modTo);

            for (int i = from; i <= to; i++)
            {
                if (i % MINUTES_COUNT_X >= modFrom && i % MINUTES_COUNT_X <= modTo)
                {
                    _minutes[i] = value;
                }
            }
        }

        private void SaveSquareVertical(byte value, int from, int to)
        {
            for (int i = from; i <= to; i++)
            {
                _minutes[i] = value;
            }
        }


        public void PaintHour(int _hour, MouseButtons mouseButton)
        {
            Color usedColor;
            byte usedSecLevel;

            if (mouseButton == MouseButtons.Left)
            {
                usedColor = _colorLeft;
                usedSecLevel = _numberLeft;
            }
            else
            {
                usedColor = _colorRight;
                usedSecLevel = _numberRight;
            }

            DrawSelectedHour(_hour * 60, (_hour * 60) + 59, usedColor);
            SaveSquare(_hour * 60, (_hour * 60) + 59, usedSecLevel);
            SelectedHours();

            if (WasChanged != null)
            {
                try
                { WasChanged(null, null); }
                catch
                { }
            }
        }

        public List<Interval> GetIntervals()
        {
            byte last;
            int intervalStart = 0;
            Interval tmpDayInterval;
            List<Interval> intervals = new List<Interval>();

            last = _minutes[0];

            for (int i = MIN_MINUTES; i <= MAX_MINUTES; i++)
            {
                if (_minutes[i] == last)
                {
                    continue;
                }
                else
                {
                    tmpDayInterval = new Interval((short)intervalStart, (short)(i - 1), last);
                    intervals.Add(tmpDayInterval);
                    last = _minutes[i];
                    intervalStart = i;
                }
            }

            tmpDayInterval = new Interval((short)intervalStart, (short)(MAX_MINUTES), last);
            intervals.Add(tmpDayInterval);

            return intervals;
        }

        public void SetIntervals(ICollection<Interval> intervals)
        {
            SaveSquareSetIntervals(MIN_MINUTES, MAX_MINUTES, (byte)_defaultColorIndex);

            if (intervals != null)
            {
                foreach (Interval interval in intervals)
                {
                    SaveSquareSetIntervals(interval.MinutesFrom, interval.MinutesTo, interval.Type);
                }
            }
            //draw new values
            DrawWholeDailyPlanBitmapOnly();
            SelectedHours();
        }

        public void SetIntervalsWithPriority(ICollection<Interval> intervals, byte[] priorites)
        {
            //clear all values
            if (intervals == null || intervals.Count == 0)
            {
                SaveSquareSetIntervals(MIN_MINUTES, MAX_MINUTES, (byte)_defaultColorIndex);
                return;
            }
            InitMinutes(0); //All unlock fix for priorites 

            //set from intervals
            foreach (Interval interval in intervals)
            {
                for (int i = interval.MinutesFrom; i <= interval.MinutesTo; i++)
                {
                    if ((priorites[_minutes[i]] < interval.Prioriy)) // || (_minutes[i] == _defaultColorIndex))
                    {
                        _minutes[i] = interval.Type;
                    }
                }
            }
            //draw new values
            DrawWholeDailyPlanBitmapOnly();
            SelectedHours();
        }

        public byte[] GetHours()
        {
            byte[] hours = new byte[24];
            for (int i = 0; i < 24; i++)
            {
                hours[i] = 200;
            }
            byte last = _minutes[0];

            for (int hour = 0; hour < 24; hour++)
            {
                int lastMinute = hour * 60 + 59;
                bool isSame = true;
                byte typeSL = _minutes[hour * 60];
                for (int minute = hour * 60; minute < lastMinute; minute++)
                {
                    if (typeSL != _minutes[minute])
                    {
                        isSame = false;
                        break;
                    }
                }
                if (isSame)
                    hours[hour] = typeSL;
            }

            return hours;
        }

        public void SelectedHours()
        {
            if (HoursWasChanged != null)
            {
                byte[] ok = new byte[24];
                for (int i = 0; i < 24; i++)
                {
                    ok[i] = 200;
                }
                byte last = _minutes[0];

                for (int hour = 0; hour < 24; hour++)
                {
                    int lastMinute = hour * 60 + 59;
                    bool isSame = true;
                    byte typeSL = _minutes[hour * 60];
                    for (int minute = hour * 60; minute < lastMinute; minute++)
                    {
                        if (typeSL != _minutes[minute])
                        {
                            isSame = false;
                            break;
                        }
                    }
                    if (isSame)
                        ok[hour] = typeSL;
                }

                try
                {
                    HoursWasChanged(ok);
                }
                catch { }
            }
        }
    }
}
