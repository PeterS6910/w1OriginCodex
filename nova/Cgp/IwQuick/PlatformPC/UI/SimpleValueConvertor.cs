using System;
using System.Collections.Generic;
using System.Text;

namespace Contal.IwQuick.UI
{
    class SimpleValueConvertor : IValueConvertor
    {
        /// <summary>
        /// converts interger-like value into these properties of control if possible :
        /// numericupdown,progressbar - value
        /// combobox, listbox, listview - selectedindex
        /// textbox,button,label,maskedtextbox - text
        /// radiobutton, checkbox - checked (value > 0 ? true : false)
        /// with other types do nothing YET
        /// </summary>
        /// <param name="value">numeric value</param>
        /// <param name="control">control to be set</param>
        /// 
        public delegate void DFromLongControlToVoid(long value, System.Windows.Forms.Control control);
        public void ToUI(long value, System.Windows.Forms.Control control)
        {
            if (control.Parent.InvokeRequired)
            {
                control.Parent.BeginInvoke(new DFromLongControlToVoid(ToUI), value, control);
            }
            else
            {
                if (control.GetType() == typeof(System.Windows.Forms.NumericUpDown))
                {
                    var tmp = (System.Windows.Forms.NumericUpDown)control;
                    tmp.Value = value;
                }

                if (control.GetType() == typeof(System.Windows.Forms.ProgressBar))
                {
                    var tmp = (System.Windows.Forms.ProgressBar)control;
                    tmp.Value = (int)value;
                }

                if (control.GetType() == typeof(System.Windows.Forms.ComboBox))
                {
                    var tmp = (System.Windows.Forms.ComboBox)control;
                    tmp.SelectedIndex = (int)value;
                }

                if (control.GetType() == typeof(System.Windows.Forms.ListBox))
                {
                    var tmp = (System.Windows.Forms.ListBox)control;
                    tmp.SelectedIndex = (int)value;
                }

                //if (control.GetType() == typeof(System.Windows.Forms.ListView))
                //{
                //    System.Windows.Forms.ListView tmp = (System.Windows.Forms.ListView)control;
                //    tmp.SelectedIndex = (int)value;
                //}

                /// textbox,button,label,maskedtextbox - text
                if (control.GetType() == typeof(System.Windows.Forms.TextBox))
                {
                    var tmp = (System.Windows.Forms.TextBox)control;
                    tmp.Text = value.ToString();
                }
                if (control.GetType() == typeof(System.Windows.Forms.Button))
                {
                    var tmp = (System.Windows.Forms.Button)control;
                    tmp.Text = value.ToString();
                }
                if (control.GetType() == typeof(System.Windows.Forms.Label))
                {
                    var tmp = (System.Windows.Forms.Label)control;
                    tmp.Text = value.ToString();
                }
                if (control.GetType() == typeof(System.Windows.Forms.MaskedTextBox))
                {
                    var tmp = (System.Windows.Forms.MaskedTextBox)control;
                    tmp.Text = value.ToString();
                }

                /// radiobutton, checkbox - checked (value > 0 ? true : false)
                if (control.GetType() == typeof(System.Windows.Forms.RadioButton))
                {
                    var tmp = (System.Windows.Forms.RadioButton)control;
                    if (value == 0)
                        tmp.Checked = true;
                    else
                        tmp.Checked = false;
                }
                if (control.GetType() == typeof(System.Windows.Forms.CheckBox))
                {
                    var tmp = (System.Windows.Forms.CheckBox)control;
                    if (value == 0)
                        tmp.Checked = true;
                    else
                        tmp.Checked = false;
                }
            }
        }

        /// <summary>
        /// converts interger-like value into these properties of control if possible (including formating):
        /// everything to text, formated by format variable ( if null or empty, only assignment )
        /// </summary>
        /// <param name="value">numeric value</param>
        /// <param name="control">control to be set</param>
        public delegate void DFromLongFormatControlToVoid(long value, string format, System.Windows.Forms.Control control);
        public void ToUI(long value, string format, System.Windows.Forms.Control control)
        {
            if (control.Parent.InvokeRequired)
            {
                control.Parent.BeginInvoke(new DFromLongFormatControlToVoid(ToUI), value, format, control);
            }
            else
            {
                /// textbox,button,label,maskedtextbox - text
                if (control.GetType() == typeof(System.Windows.Forms.TextBox))
                {
                    var tmp = (System.Windows.Forms.TextBox)control;
                    if (string.IsNullOrEmpty(format))
                        tmp.Text = value.ToString();
                    else
                        tmp.Text = value.ToString(format);
                }
                if (control.GetType() == typeof(System.Windows.Forms.Button))
                {
                    var tmp = (System.Windows.Forms.Button)control;
                    if (string.IsNullOrEmpty(format))
                        tmp.Text = value.ToString();
                    else
                        tmp.Text = value.ToString(format);
                }
                if (control.GetType() == typeof(System.Windows.Forms.Label))
                {
                    var tmp = (System.Windows.Forms.Label)control;
                    if (string.IsNullOrEmpty(format))
                        tmp.Text = value.ToString();
                    else
                        tmp.Text = value.ToString(format);
                }
                if (control.GetType() == typeof(System.Windows.Forms.MaskedTextBox))
                {
                    var tmp = (System.Windows.Forms.MaskedTextBox)control;
                    if (string.IsNullOrEmpty(format))
                        tmp.Text = value.ToString();
                    else
                        tmp.Text = value.ToString(format);
                }
            }
        }

        /// <summary>
        /// converts float value into these properties of control if possible :
        /// numericupdown,progressbar - value
        /// combobox, listbox, listview - selectedindex
        /// textbox,button,label,maskedtextbox - text
        /// radiobutton, checkbox - checked (value > 0 ? true : false)
        /// with other types do nothing YET
        /// </summary>
        /// <param name="value">numeric value</param>
        /// <param name="control">control to be set</param>
        public delegate void DFromDoubleControlToVoid(double value, System.Windows.Forms.Control control);
        public void ToUI(double value, System.Windows.Forms.Control control)
        {
            if (control.Parent.InvokeRequired)
            {
                control.Parent.BeginInvoke(new DFromDoubleControlToVoid(ToUI), value, control);
            }
            else
            {
                /// numericupdown,progressbar - value
                if (control.GetType() == typeof(System.Windows.Forms.NumericUpDown))
                {
                    var tmp = (System.Windows.Forms.NumericUpDown)control;
                    tmp.Value = (int)value;
                }
                if (control.GetType() == typeof(System.Windows.Forms.ProgressBar))
                {
                    var tmp = (System.Windows.Forms.ProgressBar)control;
                    tmp.Value = (int)value;
                }
                /// combobox, listbox, listview - selectedindex
                if (control.GetType() == typeof(System.Windows.Forms.ComboBox))
                {
                    var tmp = (System.Windows.Forms.ComboBox)control;
                    tmp.SelectedIndex = (int)value;
                }
                if (control.GetType() == typeof(System.Windows.Forms.ListBox))
                {
                    var tmp = (System.Windows.Forms.ListBox)control;
                    tmp.SelectedIndex = (int)value;
                }
                //if (control.GetType() == typeof(System.Windows.Forms.ListView))
                //{
                //    System.Windows.Forms.ListView tmp = (System.Windows.Forms.ListView)control;
                //    tmp.SelectedIndex = (int)value;
                //}

                /// textbox,button,label,maskedtextbox - text
                if (control.GetType() == typeof(System.Windows.Forms.TextBox))
                {
                    var tmp = (System.Windows.Forms.TextBox)control;
                    tmp.Text = value.ToString();
                }
                if (control.GetType() == typeof(System.Windows.Forms.Button))
                {
                    var tmp = (System.Windows.Forms.Button)control;
                    tmp.Text = value.ToString();
                }
                if (control.GetType() == typeof(System.Windows.Forms.Label))
                {
                    var tmp = (System.Windows.Forms.Label)control;
                    tmp.Text = value.ToString();
                }
                if (control.GetType() == typeof(System.Windows.Forms.MaskedTextBox))
                {
                    var tmp = (System.Windows.Forms.MaskedTextBox)control;
                    tmp.Text = value.ToString();
                }

                /// radiobutton, checkbox - checked (value > 0 ? true : false)
                if (control.GetType() == typeof(System.Windows.Forms.RadioButton))
                {
                    var tmp = (System.Windows.Forms.RadioButton)control;
                    if (value == 0)
                        tmp.Checked = true;
                    else
                        tmp.Checked = false;
                }
                if (control.GetType() == typeof(System.Windows.Forms.CheckBox))
                {
                    var tmp = (System.Windows.Forms.CheckBox)control;
                    if (value == 0)
                        tmp.Checked = true;
                    else
                        tmp.Checked = false;
                }
            }
        }

        /// <summary>
        /// converts float value into these properties of control if possible (including formating):
        /// everything to text, formated by format variable ( if null or empty, only assignment )
        /// </summary>
        /// <param name="value">numeric value</param>
        /// <param name="control">control to be set</param>
        public delegate void DFromDoubleFormatControlToVoid(double value, string format, System.Windows.Forms.Control control);
        public void ToUI(double value, string format, System.Windows.Forms.Control control)
        {
            if (control.Parent.InvokeRequired)
            {
                control.Parent.BeginInvoke(new DFromDoubleFormatControlToVoid(ToUI), value, format, control);
            }
            else
            {
                /// textbox,button,label,maskedtextbox - text
                if (control.GetType() == typeof(System.Windows.Forms.TextBox))
                {
                    var tmp = (System.Windows.Forms.TextBox)control;
                    if (string.IsNullOrEmpty(format))
                        tmp.Text = value.ToString();
                    else
                        tmp.Text = value.ToString(format);
                }
                if (control.GetType() == typeof(System.Windows.Forms.Button))
                {
                    var tmp = (System.Windows.Forms.Button)control;
                    if (string.IsNullOrEmpty(format))
                        tmp.Text = value.ToString();
                    else
                        tmp.Text = value.ToString(format);
                }
                if (control.GetType() == typeof(System.Windows.Forms.Label))
                {
                    var tmp = (System.Windows.Forms.Label)control;
                    if (string.IsNullOrEmpty(format))
                        tmp.Text = value.ToString();
                    else
                        tmp.Text = value.ToString(format);
                }
                if (control.GetType() == typeof(System.Windows.Forms.MaskedTextBox))
                {
                    var tmp = (System.Windows.Forms.MaskedTextBox)control;
                    if (string.IsNullOrEmpty(format))
                        tmp.Text = value.ToString();
                    else
                        tmp.Text = value.ToString(format);
                }
            }
        }

        /// <summary>
        /// converts string to the control's property if possible
        /// numericupdown,progressbar - value (if string convertible to decimal)
        /// combobox, listbox, listview - selecteditem
        /// textbox,button,label,maskedtextbox - text
        /// radiobutton, checkbox - checked (if the text equals to value, otherwise unchecked)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="control"></param>
        public delegate void DFromStringControlToVoid(string value, System.Windows.Forms.Control control);
        public void ToUI(string value, System.Windows.Forms.Control control)
        {
            if (control.Parent.InvokeRequired)
            {
                control.Parent.BeginInvoke(new DFromStringControlToVoid(ToUI), value, control);
            }
            else
            {
                /// numericupdown,progressbar - value
                if (control.GetType() == typeof(System.Windows.Forms.NumericUpDown))
                {
                    var tmp = (System.Windows.Forms.NumericUpDown)control;
                    decimal dec;
                    if (decimal.TryParse(value, out dec))
                    {
                        tmp.Value = dec;
                    }
                }
                if (control.GetType() == typeof(System.Windows.Forms.ProgressBar))
                {
                    var tmp = (System.Windows.Forms.ProgressBar)control;
                    int number;
                    if (int.TryParse(value, out number))
                    {
                        tmp.Value = number;
                    }
                }
                /// combobox, listbox, listview - selectedindex
                if (control.GetType() == typeof(System.Windows.Forms.ComboBox))
                {
                    var tmp = (System.Windows.Forms.ComboBox)control;
                    int number;
                    if (int.TryParse(value, out number))
                    {
                        tmp.SelectedIndex = number;
                    }
                }
                if (control.GetType() == typeof(System.Windows.Forms.ListBox))
                {
                    var tmp = (System.Windows.Forms.ListBox)control;
                    int number;
                    if (int.TryParse(value, out number))
                    {
                        tmp.SelectedIndex = number;
                    }
                }
                //if (control.GetType() == typeof(System.Windows.Forms.ListView))
                //{
                //    System.Windows.Forms.ListView tmp = (System.Windows.Forms.ListView)control;
                //    int number;
                //    if (int.TryParse(value, out number))
                //    {
                //       tmp.SelectedIndex = number;
                //    }
                //}

                /// textbox,button,label,maskedtextbox - text
                if (control.GetType() == typeof(System.Windows.Forms.TextBox))
                {
                    var tmp = (System.Windows.Forms.TextBox)control;
                    tmp.Text = value;
                }
                if (control.GetType() == typeof(System.Windows.Forms.Button))
                {
                    var tmp = (System.Windows.Forms.Button)control;
                    tmp.Text = value;
                }
                if (control.GetType() == typeof(System.Windows.Forms.Label))
                {
                    var tmp = (System.Windows.Forms.Label)control;
                    tmp.Text = value;
                }
                if (control.GetType() == typeof(System.Windows.Forms.MaskedTextBox))
                {
                    var tmp = (System.Windows.Forms.MaskedTextBox)control;
                    tmp.Text = value;
                }

                /// radiobutton, checkbox
                if (control.GetType() == typeof(System.Windows.Forms.RadioButton))
                {
                    var tmp = (System.Windows.Forms.RadioButton)control;
                    if (value == tmp.Text)
                        tmp.Checked = true;
                    else
                        tmp.Checked = false;
                }
                if (control.GetType() == typeof(System.Windows.Forms.CheckBox))
                {
                    var tmp = (System.Windows.Forms.CheckBox)control;
                    if (value == tmp.Text)
                        tmp.Checked = true;
                    else
                        tmp.Checked = false;
                }
            }
        }


        /*************************************************************************************************************************/



        /// <summary>
        /// retrieves value from the control according these rules :
        /// numericupdown, progressbar - value
        /// combobox, listbox, listview - selectedindex (if the text/selecteditem.tostring is NOT convertible to value)
        /// combobox, listbox, listview - text (if the text/selecteditem.tostring is convertible to value)
        /// textbox,button,label,maskedtextbox - text
        /// radiobutton, checkbox - if checked and text convertible to value, the text
        /// radiobutton, checkbox - if not checked and text convertible to value, left the variable unchanged
        /// radiobutton, checkbox - if text not convertible to value, checked ? 1 : 0
        /// 
        /// return true, if conversion succeeded
        /// </summary>
        /// <param name="value"></param>
        /// <param name="control"></param>
        public delegate bool DFromByteControlToBool(ref byte value, System.Windows.Forms.Control control);
        public bool FromUI(ref byte value, System.Windows.Forms.Control control)
        {
            if (control.Parent.InvokeRequired)
            {
                control.Parent.BeginInvoke(new DFromByteControlToBool(FromUI), value, control);
                return true;
            }
            else
            {
                try
                {
                    /// numericupdown,progressbar - value
                    if (control.GetType() == typeof(System.Windows.Forms.NumericUpDown))
                    {
                        var tmp = (System.Windows.Forms.NumericUpDown)control;
                        if (byte.MaxValue < tmp.Value || byte.MinValue > tmp.Value)
                            return false;
                        else
                            value = (byte)tmp.Value;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.ProgressBar))
                    {
                        var tmp = (System.Windows.Forms.ProgressBar)control;
                        if (byte.MaxValue < tmp.Value || byte.MinValue > tmp.Value)
                            return false;
                        else
                            value = (byte)tmp.Value;

                    }
            /// combobox, listbox, listview - selectedindex (if the text/selecteditem.tostring is NOT convertible to value)
            /// combobox, listbox, listview - text (if the text/selecteditem.tostring is convertible to value)
                    if (control.GetType() == typeof(System.Windows.Forms.ComboBox))
                    {
                        var tmp = (System.Windows.Forms.ComboBox)control;
                        byte mnb;
                        if (byte.TryParse(tmp.Text, out mnb))
                        {
                            value = mnb;
                        }
                        else
                        {
                            if (byte.MaxValue < tmp.SelectedIndex || byte.MinValue > tmp.SelectedIndex)
                                return false;
                            else
                                value = (byte)tmp.SelectedIndex;
                        }
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.ListBox))
                    {
                        var tmp = (System.Windows.Forms.ListBox)control;
                        byte mnb;
                        if (byte.TryParse(tmp.Text, out mnb))
                        {
                            value = mnb;
                        }
                        else
                        {
                            if (byte.MaxValue < tmp.SelectedIndex || byte.MinValue > tmp.SelectedIndex)
                                return false;
                            else
                                value = (byte)tmp.SelectedIndex;
                        }
                    }
                    //if (control.GetType() == typeof(System.Windows.Forms.ListView))
                    //{
                    //    System.Windows.Forms.ListView tmp = (System.Windows.Forms.ListView)control;
                    //    tmp.SelectedIndex = (int)value;
                    //}

                    /// textbox,button,label,maskedtextbox - text
                    if (control.GetType() == typeof(System.Windows.Forms.TextBox))
                    {
                        var tmp = (System.Windows.Forms.TextBox)control;
                        byte mnb;
                        if (byte.TryParse(tmp.Text,out mnb))
                            value = mnb;
                        else
                            return false;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.Button))
                    {
                        var tmp = (System.Windows.Forms.Button)control;
                        byte mnb;
                        if (byte.TryParse(tmp.Text,out mnb))
                            value = mnb;
                        else
                            return false;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.Label))
                    {
                        var tmp = (System.Windows.Forms.Label)control;
                        byte mnb;
                        if (byte.TryParse(tmp.Text, out mnb))
                            value = mnb;
                        else
                            return false;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.MaskedTextBox))
                    {
                        var tmp = (System.Windows.Forms.MaskedTextBox)control;
                        byte mnb;
                        if (byte.TryParse(tmp.Text, out mnb))
                            value = mnb;
                        else
                            return false;
                    }

                    /// radiobutton, checkbox - if checked and text convertible to value, the text
                    /// radiobutton, checkbox - if not checked and text convertible to value, left the variable unchanged
                    /// radiobutton, checkbox - if text not convertible to value, checked ? 1 : 0
                    if (control.GetType() == typeof(System.Windows.Forms.RadioButton))
                    {
                        var tmp = (System.Windows.Forms.RadioButton)control;
                        if (tmp.Checked)
                        {

                        }
                        if (value == 0)
                            tmp.Checked = true;
                        else
                            tmp.Checked = false;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.CheckBox))
                    {
                        var tmp = (System.Windows.Forms.CheckBox)control;
                        if (value == 0)
                            tmp.Checked = true;
                        else
                            tmp.Checked = false;
                    }
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
        public delegate bool DFromShortControlToBool(ref short value, System.Windows.Forms.Control control);
        public bool FromUI(ref short value, System.Windows.Forms.Control control)
        {
            if (control.Parent.InvokeRequired)
            {
                control.Parent.BeginInvoke(new DFromShortControlToBool(FromUI), value, control);
                return true;
            }
            else
            {
                try
                {
                    /// numericupdown,progressbar - value
                    if (control.GetType() == typeof(System.Windows.Forms.NumericUpDown))
                    {
                        var tmp = (System.Windows.Forms.NumericUpDown)control;
                        if (short.MaxValue < tmp.Value || short.MinValue > tmp.Value)
                            return false;
                        else
                            value = (short)tmp.Value;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.ProgressBar))
                    {
                        var tmp = (System.Windows.Forms.ProgressBar)control;
                        if (short.MaxValue < tmp.Value || short.MinValue > tmp.Value)
                            return false;
                        else
                            value = (short)tmp.Value;
                    }
                    /// combobox, listbox, listview - selectedindex
                    if (control.GetType() == typeof(System.Windows.Forms.ComboBox))
                    {
                        var tmp = (System.Windows.Forms.ComboBox)control;
                        short mnb;
                        if (short.TryParse(tmp.Text, out mnb))
                        {
                            value = mnb;
                        }
                        else
                        {
                            if (short.MaxValue < tmp.SelectedIndex || short.MinValue > tmp.SelectedIndex)
                                return false;
                            else
                                value = (short)tmp.SelectedIndex;
                        }
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.ListBox))
                    {
                        var tmp = (System.Windows.Forms.ListBox)control;
                        short mnb;
                        if (short.TryParse(tmp.Text, out mnb))
                        {
                            value = mnb;
                        }
                        else
                        {
                            if (short.MaxValue < tmp.SelectedIndex || short.MinValue > tmp.SelectedIndex)
                                return false;
                            else
                                value = (short)tmp.SelectedIndex;
                        }
                    }
                    //if (control.GetType() == typeof(System.Windows.Forms.ListView))
                    //{
                    //    System.Windows.Forms.ListView tmp = (System.Windows.Forms.ListView)control;
                    //    tmp.SelectedIndex = (int)value;
                    //}

                    /// textbox,button,label,maskedtextbox - text
                    if (control.GetType() == typeof(System.Windows.Forms.TextBox))
                    {
                        var tmp = (System.Windows.Forms.TextBox)control;
                        short mnb;
                        if (short.TryParse(tmp.Text, out mnb))
                            value = mnb;
                        else
                            return false;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.Button))
                    {
                        var tmp = (System.Windows.Forms.Button)control;
                        short mnb;
                        if (short.TryParse(tmp.Text, out mnb))
                            value = mnb;
                        else
                            return false;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.Label))
                    {
                        var tmp = (System.Windows.Forms.Label)control;
                        short mnb;
                        if (short.TryParse(tmp.Text, out mnb))
                            value = mnb;
                        else
                            return false;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.MaskedTextBox))
                    {
                        var tmp = (System.Windows.Forms.MaskedTextBox)control;
                        short mnb;
                        if (short.TryParse(tmp.Text, out mnb))
                            value = mnb;
                        else
                            return false;
                    }

                    /// radiobutton, checkbox - if checked and text convertible to value, the text
                    /// radiobutton, checkbox - if not checked and text convertible to value, left the variable unchanged
                    /// radiobutton, checkbox - if text not convertible to value, checked ? 1 : 0
                    if (control.GetType() == typeof(System.Windows.Forms.RadioButton))
                    {
                        var tmp = (System.Windows.Forms.RadioButton)control;
                        if (tmp.Checked)
                        {
                            short mnb;
                            if (short.TryParse(tmp.Text, out mnb))
                                value = mnb;
                            else
                                value = 1;
                        }
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.CheckBox))
                    {
                        var tmp = (System.Windows.Forms.CheckBox)control;
                        if (tmp.Checked)
                        {
                            short mnb;
                            if (short.TryParse(tmp.Text, out mnb))
                                value = mnb;
                            else
                                value = 1;
                        }
                    }
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
        public delegate bool DFromUShortControlToBool(ref ushort value, System.Windows.Forms.Control control);
        public bool FromUI(ref ushort value, System.Windows.Forms.Control control)
        {
            if (control.Parent.InvokeRequired)
            {
                control.Parent.BeginInvoke(new DFromUShortControlToBool(FromUI), value, control);
                return true;
            }
            else
            {
                try
                {
                    /// numericupdown,progressbar - value
                    if (control.GetType() == typeof(System.Windows.Forms.NumericUpDown))
                    {
                        var tmp = (System.Windows.Forms.NumericUpDown)control;
                        if (ushort.MaxValue < tmp.Value || ushort.MinValue > tmp.Value)
                            return false;
                        else
                            value = (ushort)tmp.Value;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.ProgressBar))
                    {
                        var tmp = (System.Windows.Forms.ProgressBar)control;
                        if (ushort.MaxValue < tmp.Value || ushort.MinValue > tmp.Value)
                            return false;
                        else
                            value = (ushort)tmp.Value;
                    }
                    /// combobox, listbox, listview - selectedindex
                    if (control.GetType() == typeof(System.Windows.Forms.ComboBox))
                    {
                        var tmp = (System.Windows.Forms.ComboBox)control;
                        ushort mnb;
                        if (ushort.TryParse(tmp.Text, out mnb))
                        {
                            value = mnb;
                        }
                        else
                        {
                            if (ushort.MaxValue < tmp.SelectedIndex || ushort.MinValue > tmp.SelectedIndex)
                                return false;
                            else
                                value = (ushort)tmp.SelectedIndex;
                        }
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.ListBox))
                    {
                        var tmp = (System.Windows.Forms.ListBox)control;
                        ushort mnb;
                        if (ushort.TryParse(tmp.Text, out mnb))
                        {
                            value = mnb;
                        }
                        else
                        {
                            if (ushort.MaxValue < tmp.SelectedIndex || ushort.MinValue > tmp.SelectedIndex)
                                return false;
                            else
                                value = (ushort)tmp.SelectedIndex;
                        }
                    }
                    //if (control.GetType() == typeof(System.Windows.Forms.ListView))
                    //{
                    //    System.Windows.Forms.ListView tmp = (System.Windows.Forms.ListView)control;
                    //    tmp.SelectedIndex = (int)value;
                    //}

                    /// textbox,button,label,maskedtextbox - text
                    if (control.GetType() == typeof(System.Windows.Forms.TextBox))
                    {
                        var tmp = (System.Windows.Forms.TextBox)control;
                        ushort mnb;
                        if (ushort.TryParse(tmp.Text, out mnb))
                            value = mnb;
                        else
                            return false;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.Button))
                    {
                        var tmp = (System.Windows.Forms.Button)control;
                        ushort mnb;
                        if (ushort.TryParse(tmp.Text, out mnb))
                            value = mnb;
                        else
                            return false;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.Label))
                    {
                        var tmp = (System.Windows.Forms.Label)control;
                        ushort mnb;
                        if (ushort.TryParse(tmp.Text, out mnb))
                            value = mnb;
                        else
                            return false;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.MaskedTextBox))
                    {
                        var tmp = (System.Windows.Forms.MaskedTextBox)control;
                        ushort mnb;
                        if (ushort.TryParse(tmp.Text, out mnb))
                            value = mnb;
                        else
                            return false;
                    }

                    /// radiobutton, checkbox - if checked and text convertible to value, the text
                    /// radiobutton, checkbox - if not checked and text convertible to value, left the variable unchanged
                    /// radiobutton, checkbox - if text not convertible to value, checked ? 1 : 0
                    if (control.GetType() == typeof(System.Windows.Forms.RadioButton))
                    {
                        var tmp = (System.Windows.Forms.RadioButton)control;
                        if (tmp.Checked)
                        {
                            ushort mnb;
                            if (ushort.TryParse(tmp.Text, out mnb))
                                value = mnb;
                            else
                                value = 1;
                        }
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.CheckBox))
                    {
                        var tmp = (System.Windows.Forms.CheckBox)control;
                        if (tmp.Checked)
                        {
                            ushort mnb;
                            if (ushort.TryParse(tmp.Text, out mnb))
                                value = mnb;
                            else
                                value = 1;
                        }
                    }
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
        public delegate bool DFromIntControlToBool(ref int value, System.Windows.Forms.Control control);
        public bool FromUI(ref int value, System.Windows.Forms.Control control)
        {
            if (control.Parent.InvokeRequired)
            {
                control.Parent.BeginInvoke(new DFromIntControlToBool(FromUI), value, control);
                return true;
            }
            else
            {
                try
                {
                    /// numericupdown,progressbar - value
                    if (control.GetType() == typeof(System.Windows.Forms.NumericUpDown))
                    {
                        var tmp = (System.Windows.Forms.NumericUpDown)control;
                        if (int.MaxValue < tmp.Value || int.MinValue > tmp.Value)
                            return false;
                        else
                            value = (int)tmp.Value;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.ProgressBar))
                    {
                        var tmp = (System.Windows.Forms.ProgressBar)control;
                        if (int.MaxValue < tmp.Value || int.MinValue > tmp.Value)
                            return false;
                        else
                            value = (int)tmp.Value;
                    }
                    /// combobox, listbox, listview - selectedindex
                    if (control.GetType() == typeof(System.Windows.Forms.ComboBox))
                    {
                        var tmp = (System.Windows.Forms.ComboBox)control;
                        int mnb;
                        if (int.TryParse(tmp.Text, out mnb))
                            value = mnb;
                        else
                            value = tmp.SelectedIndex;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.ListBox))
                    {
                        var tmp = (System.Windows.Forms.ListBox)control;
                        int mnb;
                        if (int.TryParse(tmp.Text, out mnb))
                            value = mnb;
                        else
                            value = tmp.SelectedIndex;
                    }
                    //if (control.GetType() == typeof(System.Windows.Forms.ListView))
                    //{
                    //    System.Windows.Forms.ListView tmp = (System.Windows.Forms.ListView)control;
                    //    tmp.SelectedIndex = (int)value;
                    //}

                    /// textbox,button,label,maskedtextbox - text
                    if (control.GetType() == typeof(System.Windows.Forms.TextBox))
                    {
                        var tmp = (System.Windows.Forms.TextBox)control;
                        int mnb;
                        if (int.TryParse(tmp.Text, out mnb))
                            value = mnb;
                        else
                            return false;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.Button))
                    {
                        var tmp = (System.Windows.Forms.Button)control;
                        int mnb;
                        if (int.TryParse(tmp.Text, out mnb))
                            value = mnb;
                        else
                            return false;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.Label))
                    {
                        var tmp = (System.Windows.Forms.Label)control;
                        int mnb;
                        if (int.TryParse(tmp.Text, out mnb))
                            value = mnb;
                        else
                            return false;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.MaskedTextBox))
                    {
                        var tmp = (System.Windows.Forms.MaskedTextBox)control;
                        int mnb;
                        if (int.TryParse(tmp.Text, out mnb))
                            value = mnb;
                        else
                            return false;
                    }

                    /// radiobutton, checkbox - if checked and text convertible to value, the text
                    /// radiobutton, checkbox - if not checked and text convertible to value, left the variable unchanged
                    /// radiobutton, checkbox - if text not convertible to value, checked ? 1 : 0
                    if (control.GetType() == typeof(System.Windows.Forms.RadioButton))
                    {
                        var tmp = (System.Windows.Forms.RadioButton)control;
                        if (tmp.Checked)
                        {
                            int mnb;
                            if (int.TryParse(tmp.Text, out mnb))
                                value = mnb;
                            else
                                value = 1;
                        }
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.CheckBox))
                    {
                        var tmp = (System.Windows.Forms.CheckBox)control;
                        if (tmp.Checked)
                        {
                            int mnb;
                            if (int.TryParse(tmp.Text, out mnb))
                                value = mnb;
                            else
                                value = 1;
                        }
                    }
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
        public delegate bool DFromUintConrolToBool(ref uint value, System.Windows.Forms.Control control);
        public bool FromUI(ref uint value, System.Windows.Forms.Control control)
        {
            if (control.Parent.InvokeRequired)
            {
                control.Parent.BeginInvoke(new DFromUintConrolToBool(FromUI), value, control);
                return true;
            }
            else
            {
                try
                {
                    /// numericupdown,progressbar - value
                    if (control.GetType() == typeof(System.Windows.Forms.NumericUpDown))
                    {
                        var tmp = (System.Windows.Forms.NumericUpDown)control;
                        if (uint.MaxValue < tmp.Value || uint.MinValue > tmp.Value)
                            return false;
                        else
                            value = (uint)tmp.Value;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.ProgressBar))
                    {
                        var tmp = (System.Windows.Forms.ProgressBar)control;
                        if (0 > tmp.Value)
                            return false;
                        else
                            value = (uint)tmp.Value;
                    }
                    /// combobox, listbox, listview - selectedindex
                    if (control.GetType() == typeof(System.Windows.Forms.ComboBox))
                    {
                        var tmp = (System.Windows.Forms.ComboBox)control;
                        uint mnb;
                        if (uint.TryParse(tmp.Text, out mnb))
                            value = mnb;
                        else
                        {
                            if (tmp.SelectedIndex < 0)
                                return false;
                            else
                                value = (uint)tmp.SelectedIndex;
                        }
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.ListBox))
                    {
                        var tmp = (System.Windows.Forms.ListBox)control;
                        uint mnb;
                        if (uint.TryParse(tmp.Text, out mnb))
                            value = mnb;
                        else
                        {
                            if (tmp.SelectedIndex < 0)
                                return false;
                            else
                                value = (uint)tmp.SelectedIndex;
                        }
                    }
                    //if (control.GetType() == typeof(System.Windows.Forms.ListView))
                    //{
                    //    System.Windows.Forms.ListView tmp = (System.Windows.Forms.ListView)control;
                    //    tmp.SelectedIndex = (int)value;
                    //}

                    /// textbox,button,label,maskedtextbox - text
                    if (control.GetType() == typeof(System.Windows.Forms.TextBox))
                    {
                        var tmp = (System.Windows.Forms.TextBox)control;
                        uint mnb;
                        if (uint.TryParse(tmp.Text, out mnb))
                            value = mnb;
                        else
                            return false;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.Button))
                    {
                        var tmp = (System.Windows.Forms.Button)control;
                        uint mnb;
                        if (uint.TryParse(tmp.Text, out mnb))
                            value = mnb;
                        else
                            return false;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.Label))
                    {
                        var tmp = (System.Windows.Forms.Label)control;
                        uint mnb;
                        if (uint.TryParse(tmp.Text, out mnb))
                            value = mnb;
                        else
                            return false;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.MaskedTextBox))
                    {
                        var tmp = (System.Windows.Forms.MaskedTextBox)control;
                        uint mnb;
                        if (uint.TryParse(tmp.Text, out mnb))
                            value = mnb;
                        else
                            return false;
                    }

                    /// radiobutton, checkbox - if checked and text convertible to value, the text
                    /// radiobutton, checkbox - if not checked and text convertible to value, left the variable unchanged
                    /// radiobutton, checkbox - if text not convertible to value, checked ? 1 : 0
                    if (control.GetType() == typeof(System.Windows.Forms.RadioButton))
                    {
                        var tmp = (System.Windows.Forms.RadioButton)control;
                        if (tmp.Checked)
                        {
                            uint mnb;
                            if (uint.TryParse(tmp.Text, out mnb))
                                value = mnb;
                            else
                                value = 1;
                        }
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.CheckBox))
                    {
                        var tmp = (System.Windows.Forms.CheckBox)control;
                        if (tmp.Checked)
                        {
                            uint mnb;
                            if (uint.TryParse(tmp.Text, out mnb))
                                value = mnb;
                            else
                                value = 1;
                        }
                    }
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
        public delegate bool DFromLongControlToBool(ref long value, System.Windows.Forms.Control control);
        public bool FromUI(ref long value, System.Windows.Forms.Control control)
        {
            if (control.Parent.InvokeRequired)
            {
                control.Parent.BeginInvoke(new DFromLongControlToBool(FromUI), value, control);
                return true;
            }
            else
            {
                try
                {
                    /// numericupdown,progressbar - value
                    if (control.GetType() == typeof(System.Windows.Forms.NumericUpDown))
                    {
                        var tmp = (System.Windows.Forms.NumericUpDown)control;
                        value = (long)tmp.Value;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.ProgressBar))
                    {
                        var tmp = (System.Windows.Forms.ProgressBar)control;
                        value = (long)tmp.Value;
                    }
                    /// combobox, listbox, listview - selectedindex
                    if (control.GetType() == typeof(System.Windows.Forms.ComboBox))
                    {
                        var tmp = (System.Windows.Forms.ComboBox)control;
                        long mnb;
                        if (long.TryParse(tmp.Text, out mnb))
                            value = mnb;
                        else
                            value = (long)tmp.SelectedIndex;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.ListBox))
                    {
                        var tmp = (System.Windows.Forms.ListBox)control;
                        long mnb;
                        if (long.TryParse(tmp.Text, out mnb))
                            value = mnb;
                        else
                            value = (long)tmp.SelectedIndex;
                    }
                    //if (control.GetType() == typeof(System.Windows.Forms.ListView))
                    //{
                    //    System.Windows.Forms.ListView tmp = (System.Windows.Forms.ListView)control;
                    //    tmp.SelectedIndex = (int)value;
                    //}

                    /// textbox,button,label,maskedtextbox - text
                    if (control.GetType() == typeof(System.Windows.Forms.TextBox))
                    {
                        var tmp = (System.Windows.Forms.TextBox)control;
                        long mnb;
                        if (long.TryParse(tmp.Text, out mnb))
                            value = mnb;
                        else
                            return false;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.Button))
                    {
                        var tmp = (System.Windows.Forms.Button)control;
                        long mnb;
                        if (long.TryParse(tmp.Text, out mnb))
                            value = mnb;
                        else
                            return false;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.Label))
                    {
                        var tmp = (System.Windows.Forms.Label)control;
                        long mnb;
                        if (long.TryParse(tmp.Text, out mnb))
                            value = mnb;
                        else
                            return false;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.MaskedTextBox))
                    {
                        var tmp = (System.Windows.Forms.MaskedTextBox)control;
                        long mnb;
                        if (long.TryParse(tmp.Text, out mnb))
                            value = mnb;
                        else
                            return false;
                    }

                    /// radiobutton, checkbox - if checked and text convertible to value, the text
                    /// radiobutton, checkbox - if not checked and text convertible to value, left the variable unchanged
                    /// radiobutton, checkbox - if text not convertible to value, checked ? 1 : 0
                    if (control.GetType() == typeof(System.Windows.Forms.RadioButton))
                    {
                        var tmp = (System.Windows.Forms.RadioButton)control;
                        if (tmp.Checked)
                        {
                            long mnb;
                            if (long.TryParse(tmp.Text, out mnb))
                                value = mnb;
                            else
                                value = 1;
                        }
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.CheckBox))
                    {
                        var tmp = (System.Windows.Forms.CheckBox)control;
                        if (tmp.Checked)
                        {
                            long mnb;
                            if (long.TryParse(tmp.Text, out mnb))
                                value = mnb;
                            else
                                value = 1;
                        }
                    }
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
        public delegate bool DFromUlongControlToBool(ref ulong value, System.Windows.Forms.Control control);
        public bool FromUI(ref ulong value, System.Windows.Forms.Control control)
        {
            if (control.Parent.InvokeRequired)
            {
                control.Parent.BeginInvoke(new DFromUlongControlToBool(FromUI), value, control);
                return true;
            }
            else
            {
                try
                {
                    /// numericupdown,progressbar - value
                    if (control.GetType() == typeof(System.Windows.Forms.NumericUpDown))
                    {
                        var tmp = (System.Windows.Forms.NumericUpDown)control;
                        if (tmp.Value < 0) return false;
                        value = (ulong)tmp.Value;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.ProgressBar))
                    {
                        var tmp = (System.Windows.Forms.ProgressBar)control;
                        if (tmp.Value < 0) return false;
                        value = (ulong)tmp.Value;
                    }
                    /// combobox, listbox, listview - selectedindex
                    if (control.GetType() == typeof(System.Windows.Forms.ComboBox))
                    {
                        var tmp = (System.Windows.Forms.ComboBox)control;
                        ulong mnb;
                        if (ulong.TryParse(tmp.Text, out mnb))
                            value = mnb;
                        else
                        {
                            if (tmp.SelectedIndex < 0)
                                return false;
                            value = (ulong)tmp.SelectedIndex;
                        }
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.ListBox))
                    {
                        var tmp = (System.Windows.Forms.ListBox)control;
                        ulong mnb;
                        if (ulong.TryParse(tmp.Text, out mnb))
                            value = mnb;
                        else
                        {
                            if (tmp.SelectedIndex < 0)
                                return false;
                            value = (ulong)tmp.SelectedIndex;
                        }
                    }
                    //if (control.GetType() == typeof(System.Windows.Forms.ListView))
                    //{
                    //    System.Windows.Forms.ListView tmp = (System.Windows.Forms.ListView)control;
                    //    tmp.SelectedIndex = (int)value;
                    //}

                    /// textbox,button,label,maskedtextbox - text
                    if (control.GetType() == typeof(System.Windows.Forms.TextBox))
                    {
                        var tmp = (System.Windows.Forms.TextBox)control;
                        ulong mnb;
                        if (ulong.TryParse(tmp.Text, out mnb))
                            value = mnb;
                        else
                            return false;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.Button))
                    {
                        var tmp = (System.Windows.Forms.Button)control;
                        ulong mnb;
                        if (ulong.TryParse(tmp.Text, out mnb))
                            value = mnb;
                        else
                            return false;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.Label))
                    {
                        var tmp = (System.Windows.Forms.Label)control;
                        ulong mnb;
                        if (ulong.TryParse(tmp.Text, out mnb))
                            value = mnb;
                        else
                            return false;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.MaskedTextBox))
                    {
                        var tmp = (System.Windows.Forms.MaskedTextBox)control;
                        ulong mnb;
                        if (ulong.TryParse(tmp.Text, out mnb))
                            value = mnb;
                        else
                            return false;
                    }

                    /// radiobutton, checkbox - if checked and text convertible to value, the text
                    /// radiobutton, checkbox - if not checked and text convertible to value, left the variable unchanged
                    /// radiobutton, checkbox - if text not convertible to value, checked ? 1 : 0
                    if (control.GetType() == typeof(System.Windows.Forms.RadioButton))
                    {
                        var tmp = (System.Windows.Forms.RadioButton)control;
                        if (tmp.Checked)
                        {
                            ulong mnb;
                            if (ulong.TryParse(tmp.Text, out mnb))
                                value = mnb;
                            else
                                value = 1;
                        }
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.CheckBox))
                    {
                        var tmp = (System.Windows.Forms.CheckBox)control;
                        if (tmp.Checked)
                        {
                            ulong mnb;
                            if (ulong.TryParse(tmp.Text, out mnb))
                                value = mnb;
                            else
                                value = 1;
                        }
                    }
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        // using value.TryParse
        public delegate bool DFromByteNumberStylesControlToBool(ref byte value, System.Globalization.NumberStyles numberStyle, System.Windows.Forms.Control control);
        public bool FromUI(ref byte value, System.Globalization.NumberStyles numberStyle, System.Windows.Forms.Control control)
        {
            if (control.Parent.InvokeRequired)
            {
                control.Parent.BeginInvoke(new DFromByteNumberStylesControlToBool(FromUI), value, numberStyle, control);
                return true;
            }
            else
            {
                try
                {
                    var provider = System.Globalization.CultureInfo.InvariantCulture;
                    /// numericupdown,progressbar - value
                    if (control.GetType() == typeof(System.Windows.Forms.NumericUpDown))
                    {
                        var tmp = (System.Windows.Forms.NumericUpDown)control;
                        if (byte.MaxValue < tmp.Value || byte.MinValue > tmp.Value)
                            return false;
                        else
                            value = (byte)tmp.Value;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.ProgressBar))
                    {
                        var tmp = (System.Windows.Forms.ProgressBar)control;
                        if (byte.MaxValue < tmp.Value || byte.MinValue > tmp.Value)
                            return false;
                        else
                            value = (byte)tmp.Value;

                    }
            /// combobox, listbox, listview - selectedindex (if the text/selecteditem.tostring is NOT convertible to value)
            /// combobox, listbox, listview - text (if the text/selecteditem.tostring is convertible to value)
                    if (control.GetType() == typeof(System.Windows.Forms.ComboBox))
                    {
                        var tmp = (System.Windows.Forms.ComboBox)control;
                        byte mnb;
                        if (byte.TryParse(tmp.Text, numberStyle, provider, out mnb))
                        {
                            value = mnb;
                        }
                        else
                        {
                            if (byte.MaxValue < tmp.SelectedIndex || byte.MinValue > tmp.SelectedIndex)
                                return false;
                            else
                                value = (byte)tmp.SelectedIndex;
                        }
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.ListBox))
                    {
                        var tmp = (System.Windows.Forms.ListBox)control;
                        byte mnb;
                        if (byte.TryParse(tmp.Text, numberStyle, provider, out mnb))
                        {
                            value = mnb;
                        }
                        else
                        {
                            if (byte.MaxValue < tmp.SelectedIndex || byte.MinValue > tmp.SelectedIndex)
                                return false;
                            else
                                value = (byte)tmp.SelectedIndex;
                        }
                    }
                    //if (control.GetType() == typeof(System.Windows.Forms.ListView))
                    //{
                    //    System.Windows.Forms.ListView tmp = (System.Windows.Forms.ListView)control;
                    //    tmp.SelectedIndex = (int)value;
                    //}

                    /// textbox,button,label,maskedtextbox - text
                    if (control.GetType() == typeof(System.Windows.Forms.TextBox))
                    {
                        var tmp = (System.Windows.Forms.TextBox)control;
                        byte mnb;
                        if (byte.TryParse(tmp.Text, numberStyle, provider, out mnb))
                            value = mnb;
                        else
                            return false;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.Button))
                    {
                        var tmp = (System.Windows.Forms.Button)control;
                        byte mnb;
                        if (byte.TryParse(tmp.Text, numberStyle, provider, out mnb))
                            value = mnb;
                        else
                            return false;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.Label))
                    {
                        var tmp = (System.Windows.Forms.Label)control;
                        byte mnb;
                        if (byte.TryParse(tmp.Text, numberStyle, provider, out mnb))
                            value = mnb;
                        else
                            return false;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.MaskedTextBox))
                    {
                        var tmp = (System.Windows.Forms.MaskedTextBox)control;
                        byte mnb;
                        if (byte.TryParse(tmp.Text, numberStyle, provider, out mnb))
                            value = mnb;
                        else
                            return false;
                    }

                    /// radiobutton, checkbox - if checked and text convertible to value, the text
                    /// radiobutton, checkbox - if not checked and text convertible to value, left the variable unchanged
                    /// radiobutton, checkbox - if text not convertible to value, checked ? 1 : 0
                    if (control.GetType() == typeof(System.Windows.Forms.RadioButton))
                    {
                        var tmp = (System.Windows.Forms.RadioButton)control;
                        if (tmp.Checked)
                        {

                        }
                        if (value == 0)
                            tmp.Checked = true;
                        else
                            tmp.Checked = false;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.CheckBox))
                    {
                        var tmp = (System.Windows.Forms.CheckBox)control;
                        if (value == 0)
                            tmp.Checked = true;
                        else
                            tmp.Checked = false;
                    }
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            
            
        }
        public delegate bool DFromShortNumberStylesControlToBool(ref short value, System.Globalization.NumberStyles numberStyle, System.Windows.Forms.Control control);
        public bool FromUI(ref short value, System.Globalization.NumberStyles numberStyle, System.Windows.Forms.Control control)
        {
            if (control.Parent.InvokeRequired)
            {
                control.Parent.BeginInvoke(new DFromShortNumberStylesControlToBool(FromUI), value, numberStyle, control);
                return true;
            }
            else
            {
                try
                {
                    var provider = System.Globalization.CultureInfo.InvariantCulture;
                    /// numericupdown,progressbar - value
                    if (control.GetType() == typeof(System.Windows.Forms.NumericUpDown))
                    {
                        var tmp = (System.Windows.Forms.NumericUpDown)control;
                        if (short.MaxValue < tmp.Value || short.MinValue > tmp.Value)
                            return false;
                        else
                            value = (short)tmp.Value;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.ProgressBar))
                    {
                        var tmp = (System.Windows.Forms.ProgressBar)control;
                        if (short.MaxValue < tmp.Value || short.MinValue > tmp.Value)
                            return false;
                        else
                            value = (short)tmp.Value;
                    }
                    /// combobox, listbox, listview - selectedindex
                    if (control.GetType() == typeof(System.Windows.Forms.ComboBox))
                    {
                        var tmp = (System.Windows.Forms.ComboBox)control;
                        short mnb;
                        if (short.TryParse(tmp.Text, numberStyle, provider, out mnb))
                        {
                            value = mnb;
                        }
                        else
                        {
                            if (short.MaxValue < tmp.SelectedIndex || short.MinValue > tmp.SelectedIndex)
                                return false;
                            else
                                value = (short)tmp.SelectedIndex;
                        }
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.ListBox))
                    {
                        var tmp = (System.Windows.Forms.ListBox)control;
                        short mnb;
                        if (short.TryParse(tmp.Text, numberStyle, provider, out mnb))
                        {
                            value = mnb;
                        }
                        else
                        {
                            if (short.MaxValue < tmp.SelectedIndex || short.MinValue > tmp.SelectedIndex)
                                return false;
                            else
                                value = (short)tmp.SelectedIndex;
                        }
                    }
                    //if (control.GetType() == typeof(System.Windows.Forms.ListView))
                    //{
                    //    System.Windows.Forms.ListView tmp = (System.Windows.Forms.ListView)control;
                    //    tmp.SelectedIndex = (int)value;
                    //}

                    /// textbox,button,label,maskedtextbox - text
                    if (control.GetType() == typeof(System.Windows.Forms.TextBox))
                    {
                        var tmp = (System.Windows.Forms.TextBox)control;
                        short mnb;
                        if (short.TryParse(tmp.Text, numberStyle, provider, out mnb))
                            value = mnb;
                        else
                            return false;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.Button))
                    {
                        var tmp = (System.Windows.Forms.Button)control;
                        short mnb;
                        if (short.TryParse(tmp.Text, numberStyle, provider, out mnb))
                            value = mnb;
                        else
                            return false;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.Label))
                    {
                        var tmp = (System.Windows.Forms.Label)control;
                        short mnb;
                        if (short.TryParse(tmp.Text, numberStyle, provider, out mnb))
                            value = mnb;
                        else
                            return false;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.MaskedTextBox))
                    {
                        var tmp = (System.Windows.Forms.MaskedTextBox)control;
                        short mnb;
                        if (short.TryParse(tmp.Text, numberStyle, provider, out mnb))
                            value = mnb;
                        else
                            return false;
                    }

                    /// radiobutton, checkbox - if checked and text convertible to value, the text
                    /// radiobutton, checkbox - if not checked and text convertible to value, left the variable unchanged
                    /// radiobutton, checkbox - if text not convertible to value, checked ? 1 : 0
                    if (control.GetType() == typeof(System.Windows.Forms.RadioButton))
                    {
                        var tmp = (System.Windows.Forms.RadioButton)control;
                        if (tmp.Checked)
                        {
                            short mnb;
                            if (short.TryParse(tmp.Text, numberStyle, provider, out mnb))
                                value = mnb;
                            else
                                value = 1;
                        }
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.CheckBox))
                    {
                        var tmp = (System.Windows.Forms.CheckBox)control;
                        if (tmp.Checked)
                        {
                            short mnb;
                            if (short.TryParse(tmp.Text, numberStyle, provider, out mnb))
                                value = mnb;
                            else
                                value = 1;
                        }
                    }
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
        public delegate bool DFromUShortNumberStylesControlToBool(ref ushort value, System.Globalization.NumberStyles numberStyle, System.Windows.Forms.Control control);
        public bool FromUI(ref ushort value, System.Globalization.NumberStyles numberStyle, System.Windows.Forms.Control control)
        {
            if (control.Parent.InvokeRequired)
            {
                control.Parent.BeginInvoke(new DFromUShortNumberStylesControlToBool(FromUI), value, numberStyle, control);
                return true;
            }
            else
            {
                try
                {
                    var provider = System.Globalization.CultureInfo.InvariantCulture;
                    /// numericupdown,progressbar - value
                    if (control.GetType() == typeof(System.Windows.Forms.NumericUpDown))
                    {
                        var tmp = (System.Windows.Forms.NumericUpDown)control;
                        if (ushort.MaxValue < tmp.Value || ushort.MinValue > tmp.Value)
                            return false;
                        else
                            value = (ushort)tmp.Value;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.ProgressBar))
                    {
                        var tmp = (System.Windows.Forms.ProgressBar)control;
                        if (ushort.MaxValue < tmp.Value || ushort.MinValue > tmp.Value)
                            return false;
                        else
                            value = (ushort)tmp.Value;
                    }
                    /// combobox, listbox, listview - selectedindex
                    if (control.GetType() == typeof(System.Windows.Forms.ComboBox))
                    {
                        var tmp = (System.Windows.Forms.ComboBox)control;
                        ushort mnb;
                        if (ushort.TryParse(tmp.Text, numberStyle, provider, out mnb))
                        {
                            value = mnb;
                        }
                        else
                        {
                            if (ushort.MaxValue < tmp.SelectedIndex || ushort.MinValue > tmp.SelectedIndex)
                                return false;
                            else
                                value = (ushort)tmp.SelectedIndex;
                        }
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.ListBox))
                    {
                        var tmp = (System.Windows.Forms.ListBox)control;
                        ushort mnb;
                        if (ushort.TryParse(tmp.Text, numberStyle, provider, out mnb))
                        {
                            value = mnb;
                        }
                        else
                        {
                            if (ushort.MaxValue < tmp.SelectedIndex || ushort.MinValue > tmp.SelectedIndex)
                                return false;
                            else
                                value = (ushort)tmp.SelectedIndex;
                        }
                    }
                    //if (control.GetType() == typeof(System.Windows.Forms.ListView))
                    //{
                    //    System.Windows.Forms.ListView tmp = (System.Windows.Forms.ListView)control;
                    //    tmp.SelectedIndex = (int)value;
                    //}

                    /// textbox,button,label,maskedtextbox - text
                    if (control.GetType() == typeof(System.Windows.Forms.TextBox))
                    {
                        var tmp = (System.Windows.Forms.TextBox)control;
                        ushort mnb;
                        if (ushort.TryParse(tmp.Text, numberStyle, provider, out mnb))
                            value = mnb;
                        else
                            return false;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.Button))
                    {
                        var tmp = (System.Windows.Forms.Button)control;
                        ushort mnb;
                        if (ushort.TryParse(tmp.Text, out mnb))
                            value = mnb;
                        else
                            return false;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.Label))
                    {
                        var tmp = (System.Windows.Forms.Label)control;
                        ushort mnb;
                        if (ushort.TryParse(tmp.Text, numberStyle, provider, out mnb))
                            value = mnb;
                        else
                            return false;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.MaskedTextBox))
                    {
                        var tmp = (System.Windows.Forms.MaskedTextBox)control;
                        ushort mnb;
                        if (ushort.TryParse(tmp.Text, numberStyle, provider, out mnb))
                            value = mnb;
                        else
                            return false;
                    }

                    /// radiobutton, checkbox - if checked and text convertible to value, the text
                    /// radiobutton, checkbox - if not checked and text convertible to value, left the variable unchanged
                    /// radiobutton, checkbox - if text not convertible to value, checked ? 1 : 0
                    if (control.GetType() == typeof(System.Windows.Forms.RadioButton))
                    {
                        var tmp = (System.Windows.Forms.RadioButton)control;
                        if (tmp.Checked)
                        {
                            ushort mnb;
                            if (ushort.TryParse(tmp.Text, numberStyle, provider, out mnb))
                                value = mnb;
                            else
                                value = 1;
                        }
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.CheckBox))
                    {
                        var tmp = (System.Windows.Forms.CheckBox)control;
                        if (tmp.Checked)
                        {
                            ushort mnb;
                            if (ushort.TryParse(tmp.Text, numberStyle, provider, out mnb))
                                value = mnb;
                            else
                                value = 1;
                        }
                    }
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
        public delegate bool DFromIntNumberStylesControlToBool(ref int value, System.Globalization.NumberStyles numberStyle, System.Windows.Forms.Control control);
        public bool FromUI(ref int value, System.Globalization.NumberStyles numberStyle, System.Windows.Forms.Control control)
        {
            if (control.Parent.InvokeRequired)
            {
                control.Parent.BeginInvoke(new DFromIntNumberStylesControlToBool(FromUI), value, numberStyle, control);
                return true;
            }
            else
            {
                try
                {
                    var provider = System.Globalization.CultureInfo.InvariantCulture;
                    /// numericupdown,progressbar - value
                    if (control.GetType() == typeof(System.Windows.Forms.NumericUpDown))
                    {
                        var tmp = (System.Windows.Forms.NumericUpDown)control;
                        if (int.MaxValue < tmp.Value || int.MinValue > tmp.Value)
                            return false;
                        else
                            value = (int)tmp.Value;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.ProgressBar))
                    {
                        var tmp = (System.Windows.Forms.ProgressBar)control;
                        if (int.MaxValue < tmp.Value || int.MinValue > tmp.Value)
                            return false;
                        else
                            value = (int)tmp.Value;
                    }
                    /// combobox, listbox, listview - selectedindex
                    if (control.GetType() == typeof(System.Windows.Forms.ComboBox))
                    {
                        var tmp = (System.Windows.Forms.ComboBox)control;
                        int mnb;
                        if (int.TryParse(tmp.Text, numberStyle, provider, out mnb))
                            value = mnb;
                        else
                            value = tmp.SelectedIndex;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.ListBox))
                    {
                        var tmp = (System.Windows.Forms.ListBox)control;
                        int mnb;
                        if (int.TryParse(tmp.Text, numberStyle, provider, out mnb))
                            value = mnb;
                        else
                            value = tmp.SelectedIndex;
                    }
                    //if (control.GetType() == typeof(System.Windows.Forms.ListView))
                    //{
                    //    System.Windows.Forms.ListView tmp = (System.Windows.Forms.ListView)control;
                    //    tmp.SelectedIndex = (int)value;
                    //}

                    /// textbox,button,label,maskedtextbox - text
                    if (control.GetType() == typeof(System.Windows.Forms.TextBox))
                    {
                        var tmp = (System.Windows.Forms.TextBox)control;
                        int mnb;
                        if (int.TryParse(tmp.Text, numberStyle, provider, out mnb))
                            value = mnb;
                        else
                            return false;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.Button))
                    {
                        var tmp = (System.Windows.Forms.Button)control;
                        int mnb;
                        if (int.TryParse(tmp.Text, numberStyle, provider, out mnb))
                            value = mnb;
                        else
                            return false;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.Label))
                    {
                        var tmp = (System.Windows.Forms.Label)control;
                        int mnb;
                        if (int.TryParse(tmp.Text, numberStyle, provider, out mnb))
                            value = mnb;
                        else
                            return false;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.MaskedTextBox))
                    {
                        var tmp = (System.Windows.Forms.MaskedTextBox)control;
                        int mnb;
                        if (int.TryParse(tmp.Text, numberStyle, provider, out mnb))
                            value = mnb;
                        else
                            return false;
                    }

                    /// radiobutton, checkbox - if checked and text convertible to value, the text
                    /// radiobutton, checkbox - if not checked and text convertible to value, left the variable unchanged
                    /// radiobutton, checkbox - if text not convertible to value, checked ? 1 : 0
                    if (control.GetType() == typeof(System.Windows.Forms.RadioButton))
                    {
                        var tmp = (System.Windows.Forms.RadioButton)control;
                        if (tmp.Checked)
                        {
                            int mnb;
                            if (int.TryParse(tmp.Text, numberStyle, provider, out mnb))
                                value = mnb;
                            else
                                value = 1;
                        }
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.CheckBox))
                    {
                        var tmp = (System.Windows.Forms.CheckBox)control;
                        if (tmp.Checked)
                        {
                            int mnb;
                            if (int.TryParse(tmp.Text, numberStyle, provider, out mnb))
                                value = mnb;
                            else
                                value = 1;
                        }
                    }
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
        public delegate bool DFromUIntNumberStylesControlToBool(ref uint value, System.Globalization.NumberStyles numberStyle, System.Windows.Forms.Control control);
        public bool FromUI(ref uint value, System.Globalization.NumberStyles numberStyle, System.Windows.Forms.Control control)
        {
            if (control.Parent.InvokeRequired)
            {
                control.Parent.BeginInvoke(new DFromUIntNumberStylesControlToBool(FromUI), value, numberStyle, control);
                return true;
            }
            else
            {
                try
                {
                    var provider = System.Globalization.CultureInfo.InvariantCulture;
                    /// numericupdown,progressbar - value
                    if (control.GetType() == typeof(System.Windows.Forms.NumericUpDown))
                    {
                        var tmp = (System.Windows.Forms.NumericUpDown)control;
                        if (uint.MaxValue < tmp.Value || uint.MinValue > tmp.Value)
                            return false;
                        else
                            value = (uint)tmp.Value;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.ProgressBar))
                    {
                        var tmp = (System.Windows.Forms.ProgressBar)control;
                        if (0 > tmp.Value)
                            return false;
                        else
                            value = (uint)tmp.Value;
                    }
                    /// combobox, listbox, listview - selectedindex
                    if (control.GetType() == typeof(System.Windows.Forms.ComboBox))
                    {
                        var tmp = (System.Windows.Forms.ComboBox)control;
                        uint mnb;
                        if (uint.TryParse(tmp.Text, numberStyle, provider, out mnb))
                            value = mnb;
                        else
                        {
                            if (tmp.SelectedIndex < 0)
                                return false;
                            else
                                value = (uint)tmp.SelectedIndex;
                        }
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.ListBox))
                    {
                        var tmp = (System.Windows.Forms.ListBox)control;
                        uint mnb;
                        if (uint.TryParse(tmp.Text, numberStyle, provider, out mnb))
                            value = mnb;
                        else
                        {
                            if (tmp.SelectedIndex < 0)
                                return false;
                            else
                                value = (uint)tmp.SelectedIndex;
                        }
                    }
                    //if (control.GetType() == typeof(System.Windows.Forms.ListView))
                    //{
                    //    System.Windows.Forms.ListView tmp = (System.Windows.Forms.ListView)control;
                    //    tmp.SelectedIndex = (int)value;
                    //}

                    /// textbox,button,label,maskedtextbox - text
                    if (control.GetType() == typeof(System.Windows.Forms.TextBox))
                    {
                        var tmp = (System.Windows.Forms.TextBox)control;
                        uint mnb;
                        if (uint.TryParse(tmp.Text, numberStyle, provider, out mnb))
                            value = mnb;
                        else
                            return false;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.Button))
                    {
                        var tmp = (System.Windows.Forms.Button)control;
                        uint mnb;
                        if (uint.TryParse(tmp.Text, numberStyle, provider, out mnb))
                            value = mnb;
                        else
                            return false;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.Label))
                    {
                        var tmp = (System.Windows.Forms.Label)control;
                        uint mnb;
                        if (uint.TryParse(tmp.Text, numberStyle, provider, out mnb))
                            value = mnb;
                        else
                            return false;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.MaskedTextBox))
                    {
                        var tmp = (System.Windows.Forms.MaskedTextBox)control;
                        uint mnb;
                        if (uint.TryParse(tmp.Text, numberStyle, provider, out mnb))
                            value = mnb;
                        else
                            return false;
                    }

                    /// radiobutton, checkbox - if checked and text convertible to value, the text
                    /// radiobutton, checkbox - if not checked and text convertible to value, left the variable unchanged
                    /// radiobutton, checkbox - if text not convertible to value, checked ? 1 : 0
                    if (control.GetType() == typeof(System.Windows.Forms.RadioButton))
                    {
                        var tmp = (System.Windows.Forms.RadioButton)control;
                        if (tmp.Checked)
                        {
                            uint mnb;
                            if (uint.TryParse(tmp.Text, numberStyle, provider, out mnb))
                                value = mnb;
                            else
                                value = 1;
                        }
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.CheckBox))
                    {
                        var tmp = (System.Windows.Forms.CheckBox)control;
                        if (tmp.Checked)
                        {
                            uint mnb;
                            if (uint.TryParse(tmp.Text, numberStyle, provider, out mnb))
                                value = mnb;
                            else
                                value = 1;
                        }
                    }
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
        public delegate bool DFromLongNumberStylesControlToBool(ref long value, System.Globalization.NumberStyles numberStyle, System.Windows.Forms.Control control);
        public bool FromUI(ref long value, System.Globalization.NumberStyles numberStyle, System.Windows.Forms.Control control)
        {   
            if (control.Parent.InvokeRequired)
            {
                control.Parent.BeginInvoke(new DFromLongNumberStylesControlToBool(FromUI), value, numberStyle, control);
                return true;
            }
            else
            {
                try
                {
                    var provider = System.Globalization.CultureInfo.InvariantCulture;
                    /// numericupdown,progressbar - value
                    if (control.GetType() == typeof(System.Windows.Forms.NumericUpDown))
                    {
                        var tmp = (System.Windows.Forms.NumericUpDown)control;
                        value = (long)tmp.Value;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.ProgressBar))
                    {
                        var tmp = (System.Windows.Forms.ProgressBar)control;
                        value = (long)tmp.Value;
                    }
                    /// combobox, listbox, listview - selectedindex
                    if (control.GetType() == typeof(System.Windows.Forms.ComboBox))
                    {
                        var tmp = (System.Windows.Forms.ComboBox)control;
                        long mnb;
                        if (long.TryParse(tmp.Text, numberStyle, provider, out mnb))
                            value = mnb;
                        else
                            value = (long)tmp.SelectedIndex;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.ListBox))
                    {
                        var tmp = (System.Windows.Forms.ListBox)control;
                        long mnb;
                        if (long.TryParse(tmp.Text, numberStyle, provider, out mnb))
                            value = mnb;
                        else
                            value = (long)tmp.SelectedIndex;
                    }
                    //if (control.GetType() == typeof(System.Windows.Forms.ListView))
                    //{
                    //    System.Windows.Forms.ListView tmp = (System.Windows.Forms.ListView)control;
                    //    tmp.SelectedIndex = (int)value;
                    //}

                    /// textbox,button,label,maskedtextbox - text
                    if (control.GetType() == typeof(System.Windows.Forms.TextBox))
                    {
                        var tmp = (System.Windows.Forms.TextBox)control;
                        long mnb;
                        if (long.TryParse(tmp.Text, numberStyle, provider, out mnb))
                            value = mnb;
                        else
                            return false;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.Button))
                    {
                        var tmp = (System.Windows.Forms.Button)control;
                        long mnb;
                        if (long.TryParse(tmp.Text, numberStyle, provider, out mnb))
                            value = mnb;
                        else
                            return false;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.Label))
                    {
                        var tmp = (System.Windows.Forms.Label)control;
                        long mnb;
                        if (long.TryParse(tmp.Text, numberStyle, provider, out mnb))
                            value = mnb;
                        else
                            return false;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.MaskedTextBox))
                    {
                        var tmp = (System.Windows.Forms.MaskedTextBox)control;
                        long mnb;
                        if (long.TryParse(tmp.Text, numberStyle, provider, out mnb))
                            value = mnb;
                        else
                            return false;
                    }

                    /// radiobutton, checkbox - if checked and text convertible to value, the text
                    /// radiobutton, checkbox - if not checked and text convertible to value, left the variable unchanged
                    /// radiobutton, checkbox - if text not convertible to value, checked ? 1 : 0
                    if (control.GetType() == typeof(System.Windows.Forms.RadioButton))
                    {
                        var tmp = (System.Windows.Forms.RadioButton)control;
                        if (tmp.Checked)
                        {
                            long mnb;
                            if (long.TryParse(tmp.Text, numberStyle, provider, out mnb))
                                value = mnb;
                            else
                                value = 1;
                        }
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.CheckBox))
                    {
                        var tmp = (System.Windows.Forms.CheckBox)control;
                        if (tmp.Checked)
                        {
                            long mnb;
                            if (long.TryParse(tmp.Text, numberStyle, provider, out mnb))
                                value = mnb;
                            else
                                value = 1;
                        }
                    }
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
        public delegate bool DFromUlongNumberStyleControlToBool(ref ulong value, System.Globalization.NumberStyles numberStyle, System.Windows.Forms.Control control);
        public bool FromUI(ref ulong value, System.Globalization.NumberStyles numberStyle, System.Windows.Forms.Control control)
        {
            if (control.Parent.InvokeRequired)
            {
                control.Parent.BeginInvoke(new DFromUlongNumberStyleControlToBool(FromUI), value, numberStyle, control);
                return true;
            }
            else
            {
                try
                {
                    var provider = System.Globalization.CultureInfo.InvariantCulture;
                    /// numericupdown,progressbar - value
                    if (control.GetType() == typeof(System.Windows.Forms.NumericUpDown))
                    {
                        var tmp = (System.Windows.Forms.NumericUpDown)control;
                        if (tmp.Value < 0) return false;
                        value = (ulong)tmp.Value;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.ProgressBar))
                    {
                        var tmp = (System.Windows.Forms.ProgressBar)control;
                        if (tmp.Value < 0) return false;
                        value = (ulong)tmp.Value;
                    }
                    /// combobox, listbox, listview - selectedindex
                    if (control.GetType() == typeof(System.Windows.Forms.ComboBox))
                    {
                        var tmp = (System.Windows.Forms.ComboBox)control;
                        ulong mnb;
                        if (ulong.TryParse(tmp.Text, numberStyle, provider, out mnb))
                            value = mnb;
                        else
                        {
                            if (tmp.SelectedIndex < 0)
                                return false;
                            value = (ulong)tmp.SelectedIndex;
                        }
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.ListBox))
                    {
                        var tmp = (System.Windows.Forms.ListBox)control;
                        ulong mnb;
                        if (ulong.TryParse(tmp.Text, numberStyle, provider, out mnb))
                            value = mnb;
                        else
                        {
                            if (tmp.SelectedIndex < 0)
                                return false;
                            value = (ulong)tmp.SelectedIndex;
                        }
                    }
                    //if (control.GetType() == typeof(System.Windows.Forms.ListView))
                    //{
                    //    System.Windows.Forms.ListView tmp = (System.Windows.Forms.ListView)control;
                    //    tmp.SelectedIndex = (int)value;
                    //}

                    /// textbox,button,label,maskedtextbox - text
                    if (control.GetType() == typeof(System.Windows.Forms.TextBox))
                    {
                        var tmp = (System.Windows.Forms.TextBox)control;
                        ulong mnb;
                        if (ulong.TryParse(tmp.Text, numberStyle, provider, out mnb))
                            value = mnb;
                        else
                            return false;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.Button))
                    {
                        var tmp = (System.Windows.Forms.Button)control;
                        ulong mnb;
                        if (ulong.TryParse(tmp.Text, numberStyle, provider, out mnb))
                            value = mnb;
                        else
                            return false;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.Label))
                    {
                        var tmp = (System.Windows.Forms.Label)control;
                        ulong mnb;
                        if (ulong.TryParse(tmp.Text, numberStyle, provider, out mnb))
                            value = mnb;
                        else
                            return false;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.MaskedTextBox))
                    {
                        var tmp = (System.Windows.Forms.MaskedTextBox)control;
                        ulong mnb;
                        if (ulong.TryParse(tmp.Text, numberStyle, provider, out mnb))
                            value = mnb;
                        else
                            return false;
                    }

                    /// radiobutton, checkbox - if checked and text convertible to value, the text
                    /// radiobutton, checkbox - if not checked and text convertible to value, left the variable unchanged
                    /// radiobutton, checkbox - if text not convertible to value, checked ? 1 : 0
                    if (control.GetType() == typeof(System.Windows.Forms.RadioButton))
                    {
                        var tmp = (System.Windows.Forms.RadioButton)control;
                        if (tmp.Checked)
                        {
                            ulong mnb;
                            if (ulong.TryParse(tmp.Text, numberStyle, provider, out mnb))
                                value = mnb;
                            else
                                value = 1;
                        }
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.CheckBox))
                    {
                        var tmp = (System.Windows.Forms.CheckBox)control;
                        if (tmp.Checked)
                        {
                            ulong mnb;
                            if (ulong.TryParse(tmp.Text, numberStyle, provider, out mnb))
                                value = mnb;
                            else
                                value = 1;
                        }
                    }
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// retrieves value from the control according these rules :
        /// numericupdown, progressbar - value
        /// combobox, listbox, listview - selectedindex (if the text/selecteditem.tostring is NOT convertible to value)
        /// combobox, listbox, listview - text (if the text/selecteditem.tostring is convertible to value)
        /// textbox,button,label,maskedtextbox - text
        /// radiobutton, checkbox - if checked and text convertible to value, the text
        /// radiobutton, checkbox - if not checked and text convertible to value, left the variable unchanged
        /// radiobutton, checkbox - if text not convertible to value, checked ? 1 : 0
        /// 
        /// return true, if conversion succeeded
        /// </summary>
        /// <param name="value"></param>
        /// <param name="control"></param>
        public delegate bool DFromFloatControlToBool(ref float value, System.Windows.Forms.Control control);
        public bool FromUI(ref float value, System.Windows.Forms.Control control)
        {
            if (control.Parent.InvokeRequired)
            {
                control.Parent.BeginInvoke(new DFromFloatControlToBool(FromUI), value, control);
                return true;
            }
            else
            {
                try
                {
                    /// numericupdown,progressbar - value
                    if (control.GetType() == typeof(System.Windows.Forms.NumericUpDown))
                    {
                        var tmp = (System.Windows.Forms.NumericUpDown)control;
                        value = (float)tmp.Value;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.ProgressBar))
                    {
                        var tmp = (System.Windows.Forms.ProgressBar)control;
                        value = (float)tmp.Value;
                    }
                    /// combobox, listbox, listview - selectedindex
                    /// combobox, listbox, listview - selectedindex (if the text/selecteditem.tostring is NOT convertible to value)
                    /// combobox, listbox, listview - text (if the text/selecteditem.tostring is convertible to value)
                    if (control.GetType() == typeof(System.Windows.Forms.ComboBox))
                    {
                        var tmp = (System.Windows.Forms.ComboBox)control;
                        float mnb;
                        if (float.TryParse(tmp.Text, out mnb))
                        {
                            value = mnb;
                        }
                        else
                        {
                            value = (float)tmp.SelectedIndex;
                        }
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.ListBox))
                    {
                        var tmp = (System.Windows.Forms.ListBox)control;
                        float mnb;
                        if (float.TryParse(tmp.Text, out mnb))
                        {
                            value = mnb;
                        }
                        else
                        {
                            value = (float)tmp.SelectedIndex;
                        }
                    }
                    //if (control.GetType() == typeof(System.Windows.Forms.ListView))
                    //{
                    //    System.Windows.Forms.ListView tmp = (System.Windows.Forms.ListView)control;
                    //    tmp.SelectedIndex = (int)value;
                    //}

                    /// textbox,button,label,maskedtextbox - text
                    if (control.GetType() == typeof(System.Windows.Forms.TextBox))
                    {
                        var tmp = (System.Windows.Forms.TextBox)control;
                        float mnb;
                        if (float.TryParse(tmp.Text, out mnb))
                            value = mnb;
                        else
                            return false;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.Button))
                    {
                        var tmp = (System.Windows.Forms.Button)control;
                        float mnb;
                        if (float.TryParse(tmp.Text, out mnb))
                            value = mnb;
                        else
                            return false;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.Label))
                    {
                        var tmp = (System.Windows.Forms.Label)control;
                        float mnb;
                        if (float.TryParse(tmp.Text, out mnb))
                            value = mnb;
                        else
                            return false;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.MaskedTextBox))
                    {
                        var tmp = (System.Windows.Forms.MaskedTextBox)control;
                        float mnb;
                        if (float.TryParse(tmp.Text, out mnb))
                            value = mnb;
                        else
                            return false;
                    }

                    /// radiobutton, checkbox - if checked and text convertible to value, the text
                    /// radiobutton, checkbox - if not checked and text convertible to value, left the variable unchanged
                    /// radiobutton, checkbox - if text not convertible to value, checked ? 1 : 0
                    if (control.GetType() == typeof(System.Windows.Forms.RadioButton))
                    {
                        var tmp = (System.Windows.Forms.RadioButton)control;
                        if (tmp.Checked)
                        {
                            float mnb;
                            if (float.TryParse(tmp.Text, out mnb))
                                value = mnb;
                            else
                                value = 1;
                        }
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.CheckBox))
                    {
                        var tmp = (System.Windows.Forms.CheckBox)control;
                        if (tmp.Checked)
                        {
                            float mnb;
                            if (float.TryParse(tmp.Text, out mnb))
                                value = mnb;
                            else
                                value = 1;
                        }
                    }
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
        public delegate bool DFromDoubleControlToBool(ref double value, System.Windows.Forms.Control control);
        public bool FromUI(ref double value, System.Windows.Forms.Control control)
        {
            if (control.Parent.InvokeRequired)
            {
                control.Parent.BeginInvoke(new DFromDoubleControlToBool(FromUI), value, control);
                return true;
            }
            else
            {
                try
                {
                    /// numericupdown,progressbar - value
                    if (control.GetType() == typeof(System.Windows.Forms.NumericUpDown))
                    {
                        var tmp = (System.Windows.Forms.NumericUpDown)control;
                        value = (double)tmp.Value;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.ProgressBar))
                    {
                        var tmp = (System.Windows.Forms.ProgressBar)control;
                        value = (double)tmp.Value;
                    }
                    /// combobox, listbox, listview - selectedindex
                    /// combobox, listbox, listview - selectedindex (if the text/selecteditem.tostring is NOT convertible to value)
                    /// combobox, listbox, listview - text (if the text/selecteditem.tostring is convertible to value)
                    if (control.GetType() == typeof(System.Windows.Forms.ComboBox))
                    {
                        var tmp = (System.Windows.Forms.ComboBox)control;
                        double mnb;
                        if (double.TryParse(tmp.Text, out mnb))
                        {
                            value = mnb;
                        }
                        else
                        {
                            value = (double)tmp.SelectedIndex;
                        }
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.ListBox))
                    {
                        var tmp = (System.Windows.Forms.ListBox)control;
                        double mnb;
                        if (double.TryParse(tmp.Text, out mnb))
                        {
                            value = mnb;
                        }
                        else
                        {
                            value = (double)tmp.SelectedIndex;
                        }
                    }
                    //if (control.GetType() == typeof(System.Windows.Forms.ListView))
                    //{
                    //    System.Windows.Forms.ListView tmp = (System.Windows.Forms.ListView)control;
                    //    tmp.SelectedIndex = (int)value;
                    //}

                    /// textbox,button,label,maskedtextbox - text
                    if (control.GetType() == typeof(System.Windows.Forms.TextBox))
                    {
                        var tmp = (System.Windows.Forms.TextBox)control;
                        double mnb;
                        if (double.TryParse(tmp.Text, out mnb))
                            value = mnb;
                        else
                            return false;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.Button))
                    {
                        var tmp = (System.Windows.Forms.Button)control;
                        double mnb;
                        if (double.TryParse(tmp.Text, out mnb))
                            value = mnb;
                        else
                            return false;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.Label))
                    {
                        var tmp = (System.Windows.Forms.Label)control;
                        double mnb;
                        if (double.TryParse(tmp.Text, out mnb))
                            value = mnb;
                        else
                            return false;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.MaskedTextBox))
                    {
                        var tmp = (System.Windows.Forms.MaskedTextBox)control;
                        float mnb;
                        if (float.TryParse(tmp.Text, out mnb))
                            value = mnb;
                        else
                            return false;
                    }

                    /// radiobutton, checkbox - if checked and text convertible to value, the text
                    /// radiobutton, checkbox - if not checked and text convertible to value, left the variable unchanged
                    /// radiobutton, checkbox - if text not convertible to value, checked ? 1 : 0
                    if (control.GetType() == typeof(System.Windows.Forms.RadioButton))
                    {
                        var tmp = (System.Windows.Forms.RadioButton)control;
                        if (tmp.Checked)
                        {
                            double mnb;
                            if (double.TryParse(tmp.Text, out mnb))
                                value = mnb;
                            else
                                value = 1;
                        }
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.CheckBox))
                    {
                        var tmp = (System.Windows.Forms.CheckBox)control;
                        if (tmp.Checked)
                        {
                            double mnb;
                            if (double.TryParse(tmp.Text, out mnb))
                                value = mnb;
                            else
                                value = 1;
                        }
                    }
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        // using value.TryParse
        public delegate bool DFromFloatNumberStylesControlToBool(ref float value, System.Globalization.NumberStyles numberStyle, System.Windows.Forms.Control control);
        public bool FromUI(ref float value, System.Globalization.NumberStyles numberStyle, System.Windows.Forms.Control control)
        {           
            if (control.Parent.InvokeRequired)
            {
                control.Parent.BeginInvoke(new DFromFloatNumberStylesControlToBool(FromUI), value, numberStyle, control);
                return true;
            }
            else
            {
                try
                {
                    var provider = System.Globalization.CultureInfo.InvariantCulture;
                    /// numericupdown,progressbar - value
                    if (control.GetType() == typeof(System.Windows.Forms.NumericUpDown))
                    {
                        var tmp = (System.Windows.Forms.NumericUpDown)control;
                        value = (float)tmp.Value;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.ProgressBar))
                    {
                        var tmp = (System.Windows.Forms.ProgressBar)control;
                        value = (float)tmp.Value;
                    }
                    /// combobox, listbox, listview - selectedindex
                    /// combobox, listbox, listview - selectedindex (if the text/selecteditem.tostring is NOT convertible to value)
                    /// combobox, listbox, listview - text (if the text/selecteditem.tostring is convertible to value)
                    if (control.GetType() == typeof(System.Windows.Forms.ComboBox))
                    {
                        var tmp = (System.Windows.Forms.ComboBox)control;
                        float mnb;
                        if (float.TryParse(tmp.Text, numberStyle, provider, out mnb))
                        {
                            value = mnb;
                        }
                        else
                        {
                            value = (float)tmp.SelectedIndex;
                        }
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.ListBox))
                    {
                        var tmp = (System.Windows.Forms.ListBox)control;
                        float mnb;
                        if (float.TryParse(tmp.Text, numberStyle, provider, out mnb))
                        {
                            value = mnb;
                        }
                        else
                        {
                            value = (float)tmp.SelectedIndex;
                        }
                    }
                    //if (control.GetType() == typeof(System.Windows.Forms.ListView))
                    //{
                    //    System.Windows.Forms.ListView tmp = (System.Windows.Forms.ListView)control;
                    //    tmp.SelectedIndex = (int)value;
                    //}

                    /// textbox,button,label,maskedtextbox - text
                    if (control.GetType() == typeof(System.Windows.Forms.TextBox))
                    {
                        var tmp = (System.Windows.Forms.TextBox)control;
                        float mnb;
                        if (float.TryParse(tmp.Text, numberStyle, provider, out mnb))
                            value = mnb;
                        else
                            return false;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.Button))
                    {
                        var tmp = (System.Windows.Forms.Button)control;
                        float mnb;
                        if (float.TryParse(tmp.Text, numberStyle, provider, out mnb))
                            value = mnb;
                        else
                            return false;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.Label))
                    {
                        var tmp = (System.Windows.Forms.Label)control;
                        float mnb;
                        if (float.TryParse(tmp.Text, numberStyle, provider, out mnb))
                            value = mnb;
                        else
                            return false;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.MaskedTextBox))
                    {
                        var tmp = (System.Windows.Forms.MaskedTextBox)control;
                        float mnb;
                        if (float.TryParse(tmp.Text, numberStyle, provider, out mnb))
                            value = mnb;
                        else
                            return false;
                    }

                    /// radiobutton, checkbox - if checked and text convertible to value, the text
                    /// radiobutton, checkbox - if not checked and text convertible to value, left the variable unchanged
                    /// radiobutton, checkbox - if text not convertible to value, checked ? 1 : 0
                    if (control.GetType() == typeof(System.Windows.Forms.RadioButton))
                    {
                        var tmp = (System.Windows.Forms.RadioButton)control;
                        if (tmp.Checked)
                        {
                            float mnb;
                            if (float.TryParse(tmp.Text, numberStyle, provider, out mnb))
                                value = mnb;
                            else
                                value = 1;
                        }
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.CheckBox))
                    {
                        var tmp = (System.Windows.Forms.CheckBox)control;
                        if (tmp.Checked)
                        {
                            float mnb;
                            if (float.TryParse(tmp.Text, numberStyle, provider, out mnb))
                                value = mnb;
                            else
                                value = 1;
                        }
                    }
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
        public delegate bool DFromDoubleNumberStyleControlToBool(ref double value, System.Globalization.NumberStyles numberStyle, System.Windows.Forms.Control control);
        public bool FromUI(ref double value, System.Globalization.NumberStyles numberStyle, System.Windows.Forms.Control control)
        {
            if (control.Parent.InvokeRequired)
            {
                control.Parent.BeginInvoke(new DFromDoubleNumberStyleControlToBool(FromUI), value, numberStyle, control);
                return true;
            }
            else
            {
                try
                {
                    var provider = System.Globalization.CultureInfo.InvariantCulture;
                    /// numericupdown,progressbar - value
                    if (control.GetType() == typeof(System.Windows.Forms.NumericUpDown))
                    {
                        var tmp = (System.Windows.Forms.NumericUpDown)control;
                        value = (double)tmp.Value;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.ProgressBar))
                    {
                        var tmp = (System.Windows.Forms.ProgressBar)control;
                        value = (double)tmp.Value;
                    }
                    /// combobox, listbox, listview - selectedindex
                    /// combobox, listbox, listview - selectedindex (if the text/selecteditem.tostring is NOT convertible to value)
                    /// combobox, listbox, listview - text (if the text/selecteditem.tostring is convertible to value)
                    if (control.GetType() == typeof(System.Windows.Forms.ComboBox))
                    {
                        var tmp = (System.Windows.Forms.ComboBox)control;
                        double mnb;
                        if (double.TryParse(tmp.Text, numberStyle, provider, out mnb))
                        {
                            value = mnb;
                        }
                        else
                        {
                            value = (double)tmp.SelectedIndex;
                        }
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.ListBox))
                    {
                        var tmp = (System.Windows.Forms.ListBox)control;
                        double mnb;
                        if (double.TryParse(tmp.Text, numberStyle, provider, out mnb))
                        {
                            value = mnb;
                        }
                        else
                        {
                            value = (double)tmp.SelectedIndex;
                        }
                    }
                    //if (control.GetType() == typeof(System.Windows.Forms.ListView))
                    //{
                    //    System.Windows.Forms.ListView tmp = (System.Windows.Forms.ListView)control;
                    //    tmp.SelectedIndex = (int)value;
                    //}

                    /// textbox,button,label,maskedtextbox - text
                    if (control.GetType() == typeof(System.Windows.Forms.TextBox))
                    {
                        var tmp = (System.Windows.Forms.TextBox)control;
                        double mnb;
                        if (double.TryParse(tmp.Text, numberStyle, provider, out mnb))
                            value = mnb;
                        else
                            return false;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.Button))
                    {
                        var tmp = (System.Windows.Forms.Button)control;
                        double mnb;
                        if (double.TryParse(tmp.Text, numberStyle, provider, out mnb))
                            value = mnb;
                        else
                            return false;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.Label))
                    {
                        var tmp = (System.Windows.Forms.Label)control;
                        double mnb;
                        if (double.TryParse(tmp.Text, numberStyle, provider, out mnb))
                            value = mnb;
                        else
                            return false;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.MaskedTextBox))
                    {
                        var tmp = (System.Windows.Forms.MaskedTextBox)control;
                        float mnb;
                        if (float.TryParse(tmp.Text, numberStyle, provider, out mnb))
                            value = mnb;
                        else
                            return false;
                    }

                    /// radiobutton, checkbox - if checked and text convertible to value, the text
                    /// radiobutton, checkbox - if not checked and text convertible to value, left the variable unchanged
                    /// radiobutton, checkbox - if text not convertible to value, checked ? 1 : 0
                    if (control.GetType() == typeof(System.Windows.Forms.RadioButton))
                    {
                        var tmp = (System.Windows.Forms.RadioButton)control;
                        if (tmp.Checked)
                        {
                            double mnb;
                            if (double.TryParse(tmp.Text, numberStyle, provider, out mnb))
                                value = mnb;
                            else
                                value = 1;
                        }
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.CheckBox))
                    {
                        var tmp = (System.Windows.Forms.CheckBox)control;
                        if (tmp.Checked)
                        {
                            double mnb;
                            if (double.TryParse(tmp.Text, numberStyle, provider, out mnb))
                                value = mnb;
                            else
                                value = 1;
                        }
                    }
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// retrieves value from the control property
        /// numericupdown,progressbar - value 
        /// combobox, listbox, listview - selecteditem (it's text)
        /// textbox,button,label,maskedtextbox - text
        /// radiobutton, checkbox - checked (the text of the control, otherwise String.Empty
        /// </summary>
        /// <param name="value"></param>
        /// <param name="control"></param>
        public delegate bool DFromStringControlToBool(ref string value, System.Windows.Forms.Control control);
        public bool FromUI(ref string value, System.Windows.Forms.Control control)
        {
            if (control.Parent.InvokeRequired)
            {
                control.Parent.BeginInvoke(new DFromStringControlToBool(FromUI), value, control);
                return true;
            }
            else
            {
                try
                {
                    /// numericupdown,progressbar - value
                    if (control.GetType() == typeof(System.Windows.Forms.NumericUpDown))
                    {
                        var tmp = (System.Windows.Forms.NumericUpDown)control;
                        value = tmp.Value.ToString();
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.ProgressBar))
                    {
                        var tmp = (System.Windows.Forms.ProgressBar)control;
                        value = tmp.Value.ToString();
                    }
                    /// combobox, listbox, listview - selectedindex
                    /// combobox, listbox, listview - selectedindex (if the text/selecteditem.tostring is NOT convertible to value)
                    /// combobox, listbox, listview - text (if the text/selecteditem.tostring is convertible to value)
                    if (control.GetType() == typeof(System.Windows.Forms.ComboBox))
                    {
                        var tmp = (System.Windows.Forms.ComboBox)control;
                        value = tmp.SelectedIndex.ToString();
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.ListBox))
                    {
                        var tmp = (System.Windows.Forms.ListBox)control;
                        value = tmp.SelectedIndex.ToString();
                    }
                    //if (control.GetType() == typeof(System.Windows.Forms.ListView))
                    //{
                    //    System.Windows.Forms.ListView tmp = (System.Windows.Forms.ListView)control;
                    //    tmp.SelectedIndex = (int)value;
                    //}

                    /// textbox,button,label,maskedtextbox - text
                    if (control.GetType() == typeof(System.Windows.Forms.TextBox))
                    {
                        var tmp = (System.Windows.Forms.TextBox)control;
                        value = tmp.Text;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.Button))
                    {
                        var tmp = (System.Windows.Forms.Button)control;
                        value = tmp.Text;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.Label))
                    {
                        var tmp = (System.Windows.Forms.Label)control;
                        value = tmp.Text;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.MaskedTextBox))
                    {
                        var tmp = (System.Windows.Forms.MaskedTextBox)control;
                        value = tmp.Text;
                    }

                    /// radiobutton, checkbox - if checked and text convertible to value, the text
                    /// radiobutton, checkbox - if not checked and text convertible to value, left the variable unchanged
                    /// radiobutton, checkbox - if text not convertible to value, checked ? 1 : 0
                    if (control.GetType() == typeof(System.Windows.Forms.RadioButton))
                    {
                        var tmp = (System.Windows.Forms.RadioButton)control;
                        value = tmp.Text;
                    }
                    if (control.GetType() == typeof(System.Windows.Forms.CheckBox))
                    {
                        var tmp = (System.Windows.Forms.CheckBox)control;
                        value = tmp.Text;
                    }
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}
