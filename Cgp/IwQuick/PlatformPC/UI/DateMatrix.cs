using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Globalization;

namespace Contal.IwQuick.UI
{
    public partial class DateMatrix : UserControl
    {
        public enum ButtonsAlign
        {
            Center = 0,
            Left = 1,
            Right = 2
        }

        private int MATRIX_WIDTH_ADJUST = 6;
        private int MATRIX_HEIGHT_VISIBLE_BUTTONS_ADJUST = 40;
        private int MATRIX_HEIGHT_INVISIBLE_BUTTONS_ADJUST = 3;
        private int MATRIX_BUTTON_WIDTH_ADJUST = 23;

        private bool _isGetFromGraphics = false;
        private Color[] _colors;

        private int _legendStepX = 10;
        private int _legendStepY = 6;
        private int _colorLeftClickIndex = 0;
        private int _colorRightClickIndex = 1;

        private bool _resizeButtons = false;
        private int _buttonsWidth = 27;
        private int _buttonsHeight = 27;
        private ButtonsAlign _buttonAlign = ButtonsAlign.Center;
        private Font _buttonsFont =
            new Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
        private bool _isEditable = true;
        private bool _viewButtons = true;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content), DisplayName("Colors")]
        [ExtenderProvidedProperty]
        public Color[] Coloras
        {
            get { return _colors; }
            set
            {
                if (ColorsCheck(value))
                {
                    _colors = value;
                    _dayHourButton.MatrixColors = _colors;
                    _dayMatrixSchedule.DayPlan.MatrixColors = _colors;
                }
                else
                {
                    throw new Exception("Colors can not be same or empty");
                }
            }
        }

        [RefreshProperties(RefreshProperties.Repaint)]
        public int ColorLeftClickIndex
        {
            get { return _colorLeftClickIndex; }
            set
            {
                if (value < _colors.Length)
                {
                    _colorLeftClickIndex = value;
                    _dayMatrixSchedule.SetLeftClickColor(_colorLeftClickIndex);
                }
                else
                {
                    throw new Exception("Color index was out of range");
                }
            }
        }

        [RefreshProperties(RefreshProperties.Repaint)]
        public int ColorRightClickIndex
        {
            get { return _colorRightClickIndex; }
            set
            {
                if (value < _colors.Length)
                {
                    _colorRightClickIndex = value;
                    _dayMatrixSchedule.SetRightClickColor(_colorRightClickIndex);
                }
                else
                {
                    throw new Exception("Color index was out of range");
                }
            }
        }

        public void SetLocalization(Dictionary<byte, string> localization)
        {
            _dayMatrixSchedule.DayPlan.Localization = localization;
        }

        public ButtonsAlign ButtonAlign
        {
            get { return _buttonAlign; }
            set
            {
                _buttonAlign = value;
                SetButtonAlign();
            }
        }

        public Font ButtonsFont
        {
            get { return _buttonsFont; }
            set
            {
                _buttonsFont = value;
                _dayHourButton.ButtonFont = value;
            }
        }

        public int ButtonsWidth
        {
            get { return _buttonsWidth; }
            set
            {
                _buttonsWidth = value;               
                _dayHourButton.ButtonWidth = _buttonsWidth;
                _dayHourButton.SetHoursDefault(_colorLeftClickIndex);
                SetButtonAlign();
            }
        }

        public int ButtonsHeight
        {
            get { return _buttonsHeight; }
            set
            {
                _buttonsHeight = value;
                _dayHourButton.ButtonHeight = _buttonsHeight;
                _dayHourButton.SetHoursDefault(_colorLeftClickIndex);
                SetButtonAlign();
            }
        }

        public bool ResizeButtons
        {
            get { return _resizeButtons; }
            set
            {
                _resizeButtons = value;
                ResizeButtonsSize();
            }
        }

        public Color this[int index]
        {
            get { return _colors[index]; }
            set
            {
                _colors[index] = value;
            }
        }

        public int LegendStepX
        {
            get { return _legendStepX; }
            set
            {
                _legendStepX = value;
                _dayMatrixSchedule.SetLegendStep(_legendStepX, _legendStepY);
            }
        }

        public int LegendStepY
        {
            get { return _legendStepY; }
            set
            {
                _legendStepY = value;
                _dayMatrixSchedule.SetLegendStep(_legendStepX, _legendStepY);
            }
        }

        [ReadOnly(true)]
        public Color ColorLeftClick
        {
            get { return _colors[_colorLeftClickIndex]; }
        }

        [ReadOnly(true)]
        public Color ColorRightClick
        {
            get { return _colors[_colorRightClickIndex]; }
        }

        public SelectionType SelectionType
        {
            get { return _dayMatrixSchedule.DayPlan.SelectionType; }
            set
            {
                _dayMatrixSchedule.DayPlan.SelectionType = value;
            }
        }

        public bool Editable
        {
            get { return _dayMatrixSchedule.DayPlan.IsEditable; }
            set
            {
                _dayMatrixSchedule.DayPlan.IsEditable = value;
            }
        }

        public bool ViewButtons
        {
            get { return _viewButtons; }
            set
            {
                if (_viewButtons != value)
                {
                    _viewButtons = value;
                    if (!_viewButtons)
                    {
                        _dayHourButton.Visible = false;
                        ResizeDaySchedule();
                    }
                    else
                    {
                        _dayHourButton.Visible = true;
                        ResizeDaySchedule();
                    }
                }
            }
        }

        [DefaultValue(-1), RefreshProperties(RefreshProperties.Repaint)]
        public int DefaultColorIndex
        {
            get { return _dayMatrixSchedule.DayPlan.DefaultColorIndex; }
            set
            {
                _dayMatrixSchedule.DayPlan.DefaultColorIndex = value;
                _dayMatrixSchedule.DayPlan.InitMinutes(DefaultColorIndex);
                _dayMatrixSchedule.DayPlan.DrawWholeDailyPlanBitmapOnly();
                _dayHourButton.SetHoursDefault(DefaultColorIndex);
            }
        }

        public event Action<object, EventArgs> WasChanged;
        public event Action<byte[]> HoursWasChanged;
        public event Action<int, MouseButtons> SelectedHour;

        public DateMatrix()
        {
            if (_colors == null || _colors.Length == 0)
            {
                _colors = new Color[2] { Color.Red, Color.LightGreen };
            }

            InitializeComponent();

            AdjustDayHourButtons();
            _dayHourButton.MatrixColors = _colors;
            _dayMatrixSchedule.DayPlan.MatrixColors = _colors;
            _dayHourButton.SetHoursDefault(DefaultColorIndex);
            if (_isEditable)
            {
                _dayMatrixSchedule.HoursWasChanged += new Action<byte[]>(_daySchedule_HoursWasChanged);
                _dayMatrixSchedule.WasChanged += new Action<object, EventArgs>(_daySchedule_WasChanged);
                _dayHourButton.SelectedHour += new Action<int, MouseButtons>(_dayHourBtn_SelectedHour);
            }
        }

        private void AdjustDayHourButtons()
        {
            if (!_resizeButtons)
            {
                if (WinFormsHelper.GetCurrentDPI() != ScreenDpi.Dpi96)
                {
                    _dayHourButton.Location = new Point(_dayHourButton.Location.X, _dayMatrixSchedule.Height + 6);
                    SetButtonAlign();
                }
            }          
        }

        private void ResizeButtonsSize()
        {
            if (_resizeButtons)
            {
                _dayHourButton.ButtonWidth = (this.Width - 26) / 24;
                _dayHourButton.Size = new Size(24 * _dayHourButton.ButtonWidth, _dayHourButton.ButtonHeight);
                _dayHourButton.SetHours(_dayMatrixSchedule.DayPlan.GetHours());
                SetButtonAlign();
            }
        }

        private bool ColorsCheck(Color[] colors)
        {
            if (colors.Length < 2)
            {
                return false;
            }

            for (int i = 0; i < colors.Length; i++)
            {
                for (int j = 0; j < colors.Length; j++)
                {
                    if ((i != j) && (colors[i] == colors[j]) || colors[i] == Color.Empty || colors[i] == null)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        void _dayHourBtn_SelectedHour(int hour, MouseButtons mouseButton)
        {
            if (SelectedHour != null)
            {
                SelectedHour(hour, mouseButton);
            }
            _dayMatrixSchedule.PaintHour(hour, mouseButton);
        }

        void _daySchedule_WasChanged(object sender, EventArgs e)
        {
            if (WasChanged != null)
            {
                WasChanged(sender, e);
            }
        }

        void _daySchedule_HoursWasChanged(byte[] hours)
        {
            if (HoursWasChanged != null)
            {
                HoursWasChanged(hours);
            }
            _dayHourButton.SetHours(hours);
        }

        private void SetButtonAlign()
        {
            switch (_buttonAlign)
            {
                case ButtonsAlign.Center: SetCenterAlign();
                    break;
                case ButtonsAlign.Left: _dayHourButton.Location =
                    new Point(MATRIX_BUTTON_WIDTH_ADJUST, _dayHourButton.Location.Y);
                    break;
                case ButtonsAlign.Right: _dayHourButton.Location =
                    new Point((this.Width - MATRIX_BUTTON_WIDTH_ADJUST) - _dayHourButton.Width, _dayHourButton.Location.Y);
                    break;
                default:
                    break;
            }
        }
        private void SetCenterAlign()
        {
            if (_dayHourButton.Width < this.Width)
            {
                _dayHourButton.Location =
                    new Point(Convert.ToInt32((this.Width - _dayHourButton.Width) / 2),
                        (this.Height - _dayHourButton.Height - 7));
            }
        }

        public void SetLeftClickColor(int index)
        {
            _dayMatrixSchedule.SetLeftClickColor(index);
        }

        public void SetRightClickColor(int index)
        {
            _dayMatrixSchedule.SetRightClickColor(index);
        }

        public void SetLeftClickColor(Color color)
        {
            int index = GetIndex(color);
            if (index > -1)
            {
                _dayMatrixSchedule.SetLeftClickColor(index);
            }
        }

        public void SetRightClickColor(Color color)
        {
            int index = GetIndex(color);
            if (index > -1)
            {
                _dayMatrixSchedule.SetRightClickColor(index);
            }
        }

        private int GetIndex(Color color)
        {
            for (int i = 0; i < _colors.Length; i++)
            {
                if (color.Equals(_colors[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        private void ResizeDaySchedule()
        {
            if (_viewButtons)
            {
                _dayMatrixSchedule.ResizeMatrix(this.Width - MATRIX_WIDTH_ADJUST,
                    this.Height - MATRIX_HEIGHT_VISIBLE_BUTTONS_ADJUST);
                ResizeButtonsSize();
            }
            else
            {
                _dayMatrixSchedule.ResizeMatrix(this.Width - MATRIX_WIDTH_ADJUST,
                    this.Height - MATRIX_HEIGHT_INVISIBLE_BUTTONS_ADJUST);
            }
        }

        private void SetValuesGraphics()
        {
            if (!_isGetFromGraphics)
            {
                Graphics grfx = CreateGraphics();
                _sizeDpiXY = new double[2];
                _sizeDpiXY[0] = (float)grfx.DpiX / 96.0;
                _sizeDpiXY[1] = (float)grfx.DpiY / 96.0;
                grfx.Dispose();
                _isGetFromGraphics = true;
            }
        }

        private double[] _sizeDpiXY;
        public double[] SizeDpiXY
        {
            get
            {
                SetValuesGraphics();
                return _sizeDpiXY;
            }
        }

        private void DateMatrix_Resize(object sender, EventArgs e)
        {
            ResizeDaySchedule();
            SetButtonAlign();
        }

        public List<Interval> GetIntervals()
        {
            return _dayMatrixSchedule.DayPlan.GetIntervals();
        }

        public void SetIntervals(List<Interval> intervals)
        {
            _dayMatrixSchedule.DayPlan.SetIntervals(intervals);
        }

        public void SetIntervalsWithPriority(List<Interval> intervals, byte[] priorites)
        {
            _dayMatrixSchedule.DayPlan.SetIntervalsWithPriority(intervals, priorites);
        }
    }

    public class Interval
    {
        int _minutesFrom;
        int _minutesTo;
        byte _type;
        byte _priority;

        public int MinutesFrom
        {
            get { return _minutesFrom; }
            set { _minutesFrom = value; }
        }

        public int MinutesTo
        {
            get { return _minutesTo; }
            set { _minutesTo = value; }
        }

        public byte Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public byte Prioriy
        {
            get { return _priority; }
            set { _priority = value; }
        }

        public Interval(int minutesFrom, int minutesTo, byte type, byte priority)
        {
            _minutesFrom = minutesFrom;
            _minutesTo = minutesTo;
            _type = type;
            _priority = priority;
        }

        public Interval(int minutesFrom, int minutesTo, byte type)
        {
            _minutesFrom = minutesFrom;
            _minutesTo = minutesTo;
            _type = type;
            _priority = 0;
        }
    }
}
