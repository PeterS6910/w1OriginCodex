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
    [ToolboxItem(true)]
    public partial class TextBoxDateTime : UserControl
    {
        public event EventHandler ValueChanged;

        /// <summary>
        /// Gets or sets datetime format
        /// </summary>
        public string CustomFormat
        {
            get { return _dateTimePicker.CustomFormat; }
            set { _dateTimePicker.CustomFormat = value; }
        }

        private DateTime? _value = null;

        /// <summary>
        /// Gets or sets DateTime value
        /// </summary>
        public DateTime? Value
        {
            get { return _value; }
            set
            {
                _value = value;
                _textBox.Visible = _value == null;
                if (_value != null)
                {
                    _dateTimePicker.Value = _value.Value;
                    if (!_dateTimePicker.Visible)
                        _dateTimePicker_ValueChanged(null, null);
                }
                _dateTimePicker.Visible = _value != null;
                if (_value == null)
                    _dateTimePicker_ValueChanged(null, null);
            }
        }

        private bool _readOnly = false;

        public bool ReadOnly
        {
            get { return _readOnly; }
            set
            {
                _readOnly = value;
                _dateTimePicker.Enabled = !_readOnly;
            }
        }

        public TextBoxDateTime()
        {
            InitializeComponent();
        }

        private void _dateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            if (_dateTimePicker.Visible)
                _value = _dateTimePicker.Value;
            if (ValueChanged != null)
                ValueChanged(sender, e);
        }

        private void _textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Value == null)
            {
                Value = DateTime.ParseExact(DateTime.Now.ToString(CustomFormat), CustomFormat, CultureInfo.InvariantCulture);
                _dateTimePicker.Focus();
                SendKeys.Send(e.KeyChar.ToString());
            }
        }
    }
}
