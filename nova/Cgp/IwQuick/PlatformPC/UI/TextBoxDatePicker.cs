using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Globalization;
using Contal.IwQuick.Localization;
using Contal.IwQuick.PlatformPC.Properties;

namespace Contal.IwQuick.UI
{
    public enum SelectedTimeOfDay: byte
    {
        Unknown, StartOfDay, Delta, Custom, EndOfDay
    }

    public partial class TextBoxDatePicker : UserControl
    {
        private int _buttonDateWidth = 23;
        private int _buttonClearDateWidth = 23;

        private bool _readOnly = false;
        private string _buttonDateText = string.Empty;
        private string _buttonClearDateText = string.Empty;
        private SelectedTimeOfDay _selectedTime = SelectedTimeOfDay.Unknown;

        public SelectedTimeOfDay SelectTime
        {
            set
            {
                _selectedTime = value;
            }
            get
            {
                return _selectedTime;
            }
        }

        public DateTime? Value
        {
            get { return _textBoxDateTime.Value; }
            set
            {
                _textBoxDateTime.Value = value;
            }
        }

        private Image _buttonDateImage;
        private Image _buttonClearDateImage;

        private double _validateAfter = 2;
        private string _validationError = string.Empty;
        private bool _validationEnabled = true;

        //private string _stringDateFormat = string.Empty;
        private string _dateFormName = "Calendar";
        public bool addActualTime { get; set; }
        LocalizationHelper _localizationHelper;

        public delegate void DButtonClicked();
        public delegate void DTextChanged(object sender, EventArgs e);
        public delegate void DIsSetDate(DateTime date, string stringDate);

        public event DButtonClicked ButtonDateClick;
        public event DButtonClicked ButtonClearDateClick;
        public event DTextChanged TextDateChanged;
        public event DIsSetDate DateSet;


        public TextBoxDatePicker()
        {
            addActualTime = false;

            InitializeComponent();

            _bDate.Cursor = Cursors.Hand;
            _bClearDate.Cursor = Cursors.Hand;

            if (WinFormsHelper.GetCurrentDPI() == ScreenDpi.Dpi120)
            {
                SetButtonHeight(26);
            }
            else
            {
                SetButtonHeight(23);
            }

            _textBoxDateTime.CustomFormat = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
        }

        #region Properties

        [Browsable(false)]
        public LocalizationHelper LocalizationHelper
        {
            get { return _localizationHelper; }
            set { _localizationHelper = value; }
        }

        [Browsable(false)]
        public TextBoxDateTime TextBox
        {
            get { return _textBoxDateTime; }
        }

        public string DateFormName
        {
            get { return _dateFormName; }
            set { _dateFormName = value; }
        }

        /// <summary>
        /// Gets text value
        /// </summary>
        public new string Text
        {
            get
            {
                if (_textBoxDateTime.Value == null)
                    return string.Empty;
                return _textBoxDateTime.Value.ToString();
            }
        }

        /// <summary>
        /// Gets or sets button "Date" width
        /// </summary>
        public int ButtonDateWidth
        {
            get { return _buttonDateWidth; }
            set
            {
                _buttonDateWidth = value;
                _bDate.Anchor = AnchorStyles.Left | AnchorStyles.Right;
                _bClearDate.Anchor = AnchorStyles.Right;
                _bDate.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                _bClearDate.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                MinimumSize = new Size((_bDate.Size.Width + _bClearDate.Size.Width + 10), 22);
            }
        }

        /// <summary>
        /// Gets or sets button "ClearDate" width
        /// </summary>
        public int ButtonClearDateWidth
        {
            get { return _buttonClearDateWidth; }
            set
            {
                _buttonClearDateWidth = value;
                _bDate.Anchor = AnchorStyles.Left;
                _bClearDate.Anchor = AnchorStyles.Left | AnchorStyles.Right;
                _bDate.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                _bClearDate.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                MinimumSize = new Size((_bDate.Size.Width + _bClearDate.Size.Width + 10), 22);
            }
        }

        
        /// <summary>
        /// Converts date in textBox to specified format.
        /// If no format is specified it converts date to current cultureinfo date format.
        /// </summary>       
        [TypeConverter(typeof(StringDateFormatConverter)), DefaultValue("")]
        public string CustomFormat
        {
            get { return _textBoxDateTime.CustomFormat; }
            set { _textBoxDateTime.CustomFormat = value; }
        }

/*
        private bool ValidateDateTimeFormat(string format)
        {
            DateTime test;
            return DateTime.TryParseExact(
                DateTime.Now.ToString(CultureInfo.InvariantCulture), 
                format, 
                DateTimeFormatInfo.InvariantInfo, 
                DateTimeStyles.None, 
                out test);
        }
*/

        /// <summary>
        /// Gets or sets button Date text, when text is empty button have default picture
        /// </summary>
        public string ButtonDateText
        {
            get { return _buttonDateText; }
            set
            {
                _buttonDateText = value;
                if (_buttonDateText == string.Empty)
                {
                    if (_buttonDateImage == null)
                    {
                        _bDate.Image = Resources.Calendar;
                    }
                    else
                    {
                        _bDate.Image = _buttonDateImage;
                    }
                }
                else
                {
                    _bDate.Image = null;
                }
                _bDate.Text = value;
            }
        }

        /// <summary>
        /// Gets or sets button Date text, when text is empty button have default picture
        /// </summary>
        public string ButtonClearDateText
        {
            get { return _buttonClearDateText; }
            set
            {
                _buttonClearDateText = value;
                if (_buttonClearDateText == string.Empty)
                {
                    if (_buttonClearDateImage == null)
                    {
                        _bClearDate.Image = Resources.CalendarBlank;
                    }
                    else
                    {
                        _bClearDate.Image = _buttonClearDateImage;
                    }
                }
                else
                {
                    _bClearDate.Image = null;
                }
                _bClearDate.Text = value;
            }
        }

        /// <summary>
        /// Gets or sets button date image
        /// </summary>
        public Image ButtonDateImage
        {
            get { return _buttonDateImage; }
            set
            {
                _buttonDateImage = value;
                if ((_buttonDateImage == null) && (_buttonDateText == string.Empty))
                {
                    _bDate.Image = Resources.Calendar;
                }
                else if ((_buttonDateImage != null) && (_buttonDateText == string.Empty))
                {
                    _bDate.Image = _buttonDateImage;
                }
            }
        }

        /// <summary>
        /// Gets or sets button clear date image
        /// </summary>
        public Image ButtonClearDateImage
        {
            get { return _buttonClearDateImage; }
            set
            {
                _buttonClearDateImage = value;
                if ((_buttonClearDateImage == null) && (_buttonClearDateText == string.Empty))
                {
                    _bClearDate.Image = Resources.CalendarBlank;
                }
                else if ((_buttonClearDateImage != null) && (_buttonClearDateText == string.Empty))
                {
                    _bClearDate.Image = _buttonClearDateImage;
                }
            }
        }

        /// <summary>
        /// Gets or sets if textBox in control is readOnly
        /// </summary>
        public bool ReadOnly
        {
            get { return _readOnly; }
            set
            {
                _readOnly = value;
                _textBoxDateTime.ReadOnly = _readOnly;
                if (_readOnly)
                    _textBoxDateTime.BackColor = Color.White;
            }
        }

        /// <summary>
        /// Gets or sets time after whitch validation occured
        /// </summary>
        [DefaultValue(2)]
        public double ValidateAfter
        {
            get { return _validateAfter; }
            set
            {
                if ((value >= 0.5) && (value <= 10))
                {
                    _validateAfter = value;
                }
                else
                {
                    throw new Exception();
                }
            }
        }

        /// <summary>
        /// Gets or sets error message
        /// </summary>
        public string ValidationError
        {
            get { return _validationError; }
            set { _validationError = value; }
        }


        /// <summary>
        /// Gets or sets if autovalidation is enabled
        /// </summary>
        public bool ValidationEnabled
        {
            get { return _validationEnabled; }
            set { _validationEnabled = value; }
        }


        public DateTime GetDate(DateTimeFormatInfo formatInfo)
        {
            return DateTime.Now;
        }

        /// <summary>
        /// Gets datetime from textBox
        /// </summary>
        /// <returns>returns null if date cannot be parsed</returns>
        public DateTime? GetDate()
        {
            return _textBoxDateTime.Value;
        }

        /// <summary>
        /// Sets date to textBox
        /// </summary>
        /// <returns>return true if date was set, else return false</returns>
        public bool SetDate(DateTime date)
        {
            _textBoxDateTime.Value = date;
            return true;
        }

        public bool SetDate(string date)
        {
            try
            {
                DateTime newDate;
                if (DateTime.TryParse(date, out newDate))
                {
                    return SetDate(newDate);
                }
            }
            catch { }
            return false;
        }


        #endregion

        private void SetButtonHeight(int value)
        {
            _bDate.Height = value;
            _bClearDate.Height = value;
        }

        private void OpenCalendarForm()
        {
            DateTime date = DateTime.Now;
            SetDate setDate = new SetDate(date);
            Bitmap bitmap = new Bitmap(_bDate.Image);
            if (_localizationHelper != null)
            {
                DateForm calendarForm = new DateForm(_localizationHelper, setDate, Icon.FromHandle(bitmap.GetHicon()), _dateFormName, _selectedTime)
                {
                    addActualTime = addActualTime
                };
                calendarForm.ShowDialog();
            }
            else
            {
                DateForm calendarForm = new DateForm(setDate, Icon.FromHandle(bitmap.GetHicon()), _dateFormName, _selectedTime)
                {
                    addActualTime = addActualTime
                };
                calendarForm.ShowDialog();
            }

            if (setDate.IsSetDate)
            {
                _textBoxDateTime.Value = setDate.Date;              
                if (DateSet != null)
                {
                    DateSet(setDate.Date, _textBoxDateTime.Value.ToString());
                }
            }
        }

        private void _bDate_Click(object sender, EventArgs e)
        {
            if (ButtonDateClick == null)
            {
                OpenCalendarForm();
            }
            else
            {
                ButtonDateClick();
            }
        }

        /*
        private void _textDate_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            OpenCalendarForm();
        }*/

        private void _bClearDate_Click(object sender, EventArgs e)
        {
            if (ButtonClearDateClick == null)
            {
                _textBoxDateTime.Value = null;
            }
            else
            {
                ButtonClearDateClick();
            }
        }

        private void _textBoxDateTime_ValueChanged(object sender, EventArgs e)
        {
            if (TextDateChanged != null)
            {
                TextDateChanged(sender, e);
            }
        }        
    }

    public class StringDateFormatConverter : StringConverter
    {
        private readonly string[] dateFormats =
        {"d. M. yyyy", "dd.MM.yyyy","dd:MM:yy", "d/M/yyyy",
            "dddd, MMMM d, yyyy", "M. d. yyyy", "MM/dd/yy", "MM/dd/yyyy","" };
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            //true means show a combobox
            return true;
        }
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            //true will limit to list. false will show the list, but allow free-form entry
            return false;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(dateFormats);
        }
    }
}
