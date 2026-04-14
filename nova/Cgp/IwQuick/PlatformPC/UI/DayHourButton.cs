using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

namespace Contal.IwQuick.UI
{
    [ToolboxItem(false)]
    class DayHourButton : Control
    {
        Button[] _buttonHours;
        private int _buttonWidth = 27;
        private int _buttonHeight = 27;
        private Font _buttonfont =
            new Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
        private Color[] _matrixColors;

        public Color[] MatrixColors
        {
            get { return _matrixColors; }
            set { _matrixColors = value; }
        }

        public Font ButtonFont
        {
            get { return _buttonfont; }
            set
            {
                _buttonfont = value;
                DrowButtons();
            }
        }

        public int ButtonWidth
        {
            get { return _buttonWidth; }
            set
            {
                _buttonWidth = value;
                DrowButtons();
            }
        }

        public int ButtonHeight
        {
            get { return _buttonHeight; }
            set
            {
                _buttonHeight = value;
                DrowButtons();
            }
        }

        private Color _selectionColor = Color.Red;
        public Color SelectionColor
        {
            get { return _selectionColor; }
            set { _selectionColor = value; }
        }

        public event Action<int, MouseButtons> SelectedHour;

        public DayHourButton()
        {
            Cursor = Cursors.Hand;
            DrowButtons();            
        }

        private void DrowButtons()
        {
            this.Controls.Clear();
            this.Font = _buttonfont;
            _buttonHours = new Button[24];
            AutoSize = true;
            Button tmpButton;
            for (int i = 0; i < 24; i++)
            {
                tmpButton = new Button();
                tmpButton.TextAlign = ContentAlignment.MiddleCenter;
                tmpButton.Width = _buttonWidth;
                tmpButton.Height = _buttonHeight;
                tmpButton.Parent = this;
                tmpButton.Name = i.ToString();
                tmpButton.Text = i.ToString();
                tmpButton.Location = new System.Drawing.Point((i * (_buttonWidth)), 0);
                tmpButton.MouseDown += new MouseEventHandler(tmpButton_MouseDown);
                _buttonHours[i] = tmpButton;
            }            
        }

        void tmpButton_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                int btnNumber;
                if (!Int32.TryParse((sender as Button).Name, out btnNumber))
                    return;

                if (SelectedHour != null)
                {
                    if (e.Button == MouseButtons.Left)
                        _buttonHours[btnNumber].FlatStyle = FlatStyle.Popup;
                    else
                        _buttonHours[btnNumber].FlatStyle = FlatStyle.Standard;
                    try
                    {
                        SelectedHour(btnNumber, e.Button);
                    }
                    catch { }
                }
            }
            catch
            { }
        }

        public void SetSelectedHours(int hour)
        {
            _buttonHours[hour].FlatStyle = FlatStyle.Standard;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
        }

        public override Size GetPreferredSize(Size sz)
        {
            return new Size((_buttonWidth * 24), _buttonHeight);
        }

        public void SetHoursDefault(int index)
        {
            for (int i = 0; i < 24; i++)
            {
                _buttonHours[i].FlatStyle = FlatStyle.Popup;
                if (_matrixColors == null)
                {
                    _buttonHours[i].BackColor = Color.Red;
                }
                else
                {
                    _buttonHours[i].BackColor = _matrixColors[index];
                }                
            }
        }

        public void SetHours(byte[] hours)
        {
            for (int i = 0; i < 24; i++)
            {
                if (hours[i] == 200)
                {
                    _buttonHours[i].FlatStyle = FlatStyle.Standard;
                    _buttonHours[i].BackColor = Color.LightGray;
                }
                else
                {
                    _buttonHours[i].FlatStyle = FlatStyle.Popup;
                    _buttonHours[i].BackColor = _matrixColors[(int)hours[i]];
                }
            }
        }        
    }
}
