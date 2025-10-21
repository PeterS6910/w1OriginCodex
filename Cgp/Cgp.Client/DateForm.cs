using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Contal.IwQuick.Localization;

namespace Contal.Cgp.Client
{
    public partial class DateForm : CgpTranslateForm
    {
        private SetDate _setDate;
        private Point _lastClickPosition;
        private long _lastClickTime;
        private bool _lastClickRaisedDoubleClick = false;

        public DateForm(LocalizationHelper localizationHelper, SetDate setDate)
            : base(localizationHelper)
        {
            InitializeComponent();
            _setDate = setDate;
            _mcExpirationDate.SelectionStart = setDate.Date;
        }

        private void _bCancel_Click(object sender, EventArgs e)
        {
            _setDate.IsSetDate = false;
            Close();
        }

        private void _bOk_Click(object sender, EventArgs e)
        {
            _setDate.IsSetDate = true;
            _setDate.Date = _mcExpirationDate.SelectionStart;
            Close();
        }

        private void CalendarForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                _bCancel_Click(this, null);
            }
        }

        private bool IsInDoubleClickArea(Point lastPoint, Point actualPoint)
        {
            return Math.Abs(lastPoint.X - actualPoint.X) <= SystemInformation.DoubleClickSize.Width &&
                Math.Abs(lastPoint.Y - actualPoint.Y) <= SystemInformation.DoubleClickSize.Height;

        }

        private void OnDoubleClick()
        {
            _bOk_Click(this, null);
        }

        private void _mcExpirationDate_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _lastClickRaisedDoubleClick = false;

                if (!_lastClickRaisedDoubleClick && DateTime.Now.Ticks - _lastClickTime <= SystemInformation.DoubleClickTime * 10000 &&
                    IsInDoubleClickArea(_lastClickPosition, Cursor.Position))
                {
                    Rectangle r = _mcExpirationDate.RectangleToScreen(_mcExpirationDate.ClientRectangle);

                    Point p = new Point(Cursor.Position.X - r.X,
                         Cursor.Position.Y - r.Y);
                    
                    MonthCalendar.HitTestInfo hti = _mcExpirationDate.HitTest(p);
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

        private void CalendarForm_Load(object sender, EventArgs e)
        {
            

        }

        private void CalendarForm_Shown(object sender, EventArgs e)
        {
            int margin = 5;


            _mcExpirationDate.Top = margin;
            _mcExpirationDate.Left = margin;

            _bOk.Left = margin;
            _bOk.Top = 2 * margin + _mcExpirationDate.Height;

            _bCancel.Left = margin + _mcExpirationDate.Width - _bCancel.Width;
            _bCancel.Top = 2 * margin + _mcExpirationDate.Height;

            int formMarginX, formMarginY;
            formMarginY = this.Height - this.ClientSize.Height;
            formMarginX = this.Width - this.ClientSize.Width;

            this.Width = _mcExpirationDate.Width + 2 * margin + formMarginX;
            this.Height = _mcExpirationDate.Height + 3 * margin + _bOk.Height + formMarginY;

            MoveFormToCursor();
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
