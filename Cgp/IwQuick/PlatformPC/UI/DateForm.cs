using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Contal.IwQuick.Localization;
using System.Reflection;

namespace Contal.IwQuick.UI
{
    /// <summary>
    /// If you use localization, component names to translate are:
    /// Form name: DateFormDateForm
    /// Button OK: DateForm_bOk
    /// Button Cancel: DateForm_bCancel
    /// </summary>


    public partial class DateForm : TranslateForm
    {
        private SetDate _setDate;
        private Point _lastClickPosition;
        private long _lastClickTime;
        private bool _lastClickRaisedDoubleClick = false;
        public bool addActualTime { get; set; }
        private SelectedTimeOfDay _selectedTime = SelectedTimeOfDay.Unknown;
        private DateTime _addedTime;

        public DateForm(SetDate setDate, Icon icon, string name, SelectedTimeOfDay selectedTime)           
        {
            InitializeComponent();

            KeyPreview = true;
            _selectedTime = selectedTime;
            _setDate = setDate;
            _mcDate.SelectionStart = _setDate.Date;
            if (icon != null)
            {
                this.Icon = icon;
            }
            this.Text = name;
        }

        public DateForm(LocalizationHelper localizationHelper, SetDate setDate, Icon icon, string name, SelectedTimeOfDay selectedTime)
            : base(localizationHelper)
        {
            InitializeComponent();

            KeyPreview = true;
            _selectedTime = selectedTime;
            _setDate = setDate;
            _mcDate.SelectionStart = _setDate.Date;
            if (icon != null)
            {
                this.Icon = icon;
            }
            this.Text = name;
        }

        private bool IsInDoubleClickArea(Point lastPoint, Point actualPoint)
        {
            return Math.Abs(lastPoint.X - actualPoint.X) <= SystemInformation.DoubleClickSize.Width &&
                Math.Abs(lastPoint.Y - actualPoint.Y) <= SystemInformation.DoubleClickSize.Height;

        }

        private void OnDoubleClick()
        {
            _bOk_Click(null, null);
        }

        private void DateForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                _bCancel_Click(this, null);
            }
        }       

        private void SetMargin()
        {
            int margin = 5;

            int formClientSizeWidth = 0;
            int formClientSizeHeight = _mcDate.Height + _bOk.Height + 3 * margin; 

            if (_gbSelectTime.Visible)
            {
                formClientSizeWidth = _mcDate.Width + _gbSelectTime.Width + 3*margin;
                this.ClientSize = new Size(formClientSizeWidth, formClientSizeHeight);
                
                _gbSelectTime.Left = _mcDate.Width + 2 * margin;
                _gbSelectTime.Top = margin;

                _bCancel.Left = _mcDate.Width + 2 * margin + _gbSelectTime.Width - _bCancel.Width;
            }
            else
            {
                formClientSizeWidth = _mcDate.Width + 2 * margin;
                this.ClientSize = new Size(formClientSizeWidth, formClientSizeHeight);

                _bCancel.Left = _mcDate.Width + margin - _bCancel.Width;
            }

            _mcDate.Top = margin;
            _mcDate.Left = margin;
            _bOk.Top = _mcDate.Height + 2 * margin;
            _bOk.Left = margin;
            _bCancel.Top = _bOk.Top;
        }

        /// <summary>
        /// Move Form to current cursor position
        /// </summary>
        private void MoveFormToCursor()
        {
            Point formPos = new Point();
            formPos.X = (int)(Cursor.Position.X - (double)(ClientSize.Width / 2));
            formPos.Y = (int)(Cursor.Position.Y - (double)(ClientSize.Height / 2));
            Location = formPos;
        }

        private void _bOk_Click(object sender, EventArgs e)
        {
            _setDate.IsSetDate = true;
            _setDate.Date = _mcDate.SelectionStart.Date;

            DateTime dateFrom = _setDate.Date;
            DateTime now = DateTime.Now;
            if (dateFrom.Year == now.Year && dateFrom.Month == now.Month && dateFrom.Day == now.Day && addActualTime)
            {
                if (IwQuick.UI.Dialog.Question(GetString("QuestionAddActualTime"), MessageBoxDefaultButton.Button2))
                {
                    dateFrom = dateFrom.AddHours(now.Hour);
                    dateFrom = dateFrom.AddMinutes(now.Minute);
                    dateFrom = dateFrom.AddSeconds(now.Second);
                    _setDate.Date = dateFrom;
                }
            }

            if (_selectedTime != SelectedTimeOfDay.Unknown)
            {
                dateFrom = dateFrom.AddHours(_addedTime.Hour);
                dateFrom = dateFrom.AddMinutes(_addedTime.Minute);
                dateFrom = dateFrom.AddSeconds(_addedTime.Second);
                _setDate.Date = dateFrom;
            }

            Close();
        }

        private void _bCancel_Click(object sender, EventArgs e)
        {
            _setDate.IsSetDate = false;
            Close();
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _lastClickRaisedDoubleClick = false;

                if (!_lastClickRaisedDoubleClick && DateTime.Now.Ticks - _lastClickTime <= SystemInformation.DoubleClickTime * 10000 &&
                    IsInDoubleClickArea(_lastClickPosition, Cursor.Position))
                {
                    Rectangle r = _mcDate.RectangleToScreen(_mcDate.ClientRectangle);

                    Point p = new Point(Cursor.Position.X - r.X,
                         Cursor.Position.Y - r.Y);

                    MonthCalendar.HitTestInfo hti = _mcDate.HitTest(p);
                    if (hti.HitArea == MonthCalendar.HitArea.Date)
                    {
                        OnDoubleClick();
                        _lastClickRaisedDoubleClick = true;
                    }
                }

                _lastClickPosition = Cursor.Position;
                _lastClickTime = DateTime.Now.Ticks;
            }
        }

        private void _mcExpirationDate_MouseDown(object sender, MouseEventArgs e)
        {
            OnMouseDown(sender, e);
        }   

        private void DateForm_Shown(object sender, EventArgs e)
        {
            _addedTime = new DateTime(2013,1,1,0,0,0);
            _dtpCustom.Value = _addedTime;
            _dtpDelta.Value = _addedTime.AddHours(1);

            switch (_selectedTime)
            {
                case SelectedTimeOfDay.Unknown:
                    _gbSelectTime.Visible = false;
                    break;
                case SelectedTimeOfDay.StartOfDay:
                    _rbStartOfDay.Checked = true;
                    break;
                case SelectedTimeOfDay.Delta:
                    _rbDelta.Checked = true;
                    _dtpDelta.Enabled = true;
                    break;
                case SelectedTimeOfDay.Custom:
                    _rbCustom.Checked = true;
                    _dtpCustom.Enabled = true;
                    break;
                case SelectedTimeOfDay.EndOfDay:
                    _rbEndOfDay.Checked = true;
                    break;
            }

            SetMargin();
            MoveFormToCursor();
        }

        private void DateForm_Load(object sender, EventArgs e)
        {
           
        }

        private void _rbStartOfDay_CheckedChanged(object sender, EventArgs e)
        {
            _selectedTime = SelectedTimeOfDay.StartOfDay;
            _dtpDelta.Enabled = false;
            _dtpCustom.Enabled = false;
            _addedTime = new DateTime(2013,1,1,0,0,0);
        }

        private void _rbDelta_CheckedChanged(object sender, EventArgs e)
        {
            _selectedTime = SelectedTimeOfDay.Delta;
            _dtpDelta.Enabled = true;
            _dtpCustom.Enabled = false;
            CalculateAddingTime();
        }

        private void _rbEndOfDay_CheckedChanged(object sender, EventArgs e)
        {
            _selectedTime = SelectedTimeOfDay.EndOfDay;
            _dtpDelta.Enabled = false;
            _dtpCustom.Enabled = false;
            _addedTime = new DateTime(2013, 1, 1, 23, 59, 59);
        }

        private void _dtpDelta_ValueChanged(object sender, EventArgs e)
        {
            if (_selectedTime == SelectedTimeOfDay.Delta)
                CalculateAddingTime();
        }

        private void _rbCustom_CheckedChanged(object sender, EventArgs e)
        {
            _selectedTime = SelectedTimeOfDay.Custom;
            _dtpCustom.Enabled = true;
            _dtpDelta.Enabled = false;
            _addedTime = new DateTime(2013, 1, 1, _dtpCustom.Value.Hour, _dtpCustom.Value.Minute, _dtpCustom.Value.Second);
        }

        private void _dtpCustom_ValueChanged(object sender, EventArgs e)
        {
            if (_selectedTime == SelectedTimeOfDay.Custom)
                _addedTime = new DateTime(2013, 1, 1, _dtpCustom.Value.Hour, _dtpCustom.Value.Minute, _dtpCustom.Value.Second);
        }

        private void CalculateAddingTime()
        {
            try
            {
                DateTime now = DateTime.Now;
                int newHourValue = now.Hour - _dtpDelta.Value.Hour;
                int newMinuteValue = now.Minute - _dtpDelta.Value.Minute;
                int newSecondValue = now.Second - _dtpDelta.Value.Second;

                _addedTime = new DateTime(2013, 1, 1, newHourValue,
                newMinuteValue, newSecondValue);
            }
            catch (Exception)
            {
                _addedTime = new DateTime(2013, 1, 1, 0, 0, 0);
            }
        }
    }

    public class SetDate
    {
        private bool _isSetDate = false;
        private DateTime _date;

        public bool IsSetDate
        {
            get { return _isSetDate; }
            set { _isSetDate = value; }
        }

        public DateTime Date
        {
            get { return _date; }
            set { _date = value; }
        }

        public SetDate(DateTime? setDate)
        {
            if (setDate != null)
                _date = setDate.Value;
            else
                _date = DateTime.Now;

            _isSetDate = false;
        }
    }
}
