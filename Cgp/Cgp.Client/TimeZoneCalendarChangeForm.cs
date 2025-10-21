using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick.Localization;

namespace Contal.Cgp.Client
{
    public partial class TimeZoneCalendarChangeForm : CgpTranslateForm
    {
        Calendar _oldCalendar = null;
        Calendar _newCalendar = null;
        Contal.Cgp.Server.Beans.TimeZone _timeZone = null;
        ICollection<TimeZoneDateSetting> _listTZDateSettings = new List<TimeZoneDateSetting>();
        ICollection<TimeZoneDateSetting> _listNewTZDateSettings = new List<TimeZoneDateSetting>();
        bool _insert;

        public TimeZoneCalendarChangeForm(Calendar oldCalendar, Calendar newCalendar, Contal.Cgp.Server.Beans.TimeZone timeZone, bool insert)
            :base(CgpClient.Singleton.LocalizationHelper)
        {
            InitializeComponent();
            _oldCalendar = oldCalendar;
            _newCalendar = newCalendar;
            _timeZone = timeZone;
            _insert = insert;
            ShowDateSettingsErrors();
        }

        private void ShowDateSettingsErrors()
        {
            TimeZoneDateSetting missedTzDs;
            bool notOwned;
            foreach (TimeZoneDateSetting tds in _timeZone.DateSettings)
            {
                notOwned = true;
                TimeZoneDateSetting dateSettings = CgpClient.Singleton.MainServerProvider.TimeZoneDateSettings.GetObjectById(tds.IdTimeZoneDateSetting);
                if (dateSettings.DayType != null)
                {
                    if (_newCalendar != null && _newCalendar.DateSettings != null)
                    {
                        foreach (CalendarDateSetting ds in _newCalendar.DateSettings)
                        {
                            DayType day = CgpClient.Singleton.MainServerProvider.DayTypes.GetObjectById(ds.DayType.IdDayType);
                            if (day.Compare(dateSettings.DayType))
                            {
                                notOwned = false;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    notOwned = false;
                }

                if (notOwned)
                {
                    missedTzDs = CgpClient.Singleton.MainServerProvider.TimeZoneDateSettings.GetObjectById(dateSettings.IdTimeZoneDateSetting);
                    _listTZDateSettings.Add(missedTzDs);
                }
                else
                {
                    missedTzDs = CgpClient.Singleton.MainServerProvider.TimeZoneDateSettings.GetObjectById(dateSettings.IdTimeZoneDateSetting);
                    _listNewTZDateSettings.Add(missedTzDs);
                }
            }

            _eErrors.Text = string.Empty;
            foreach (TimeZoneDateSetting dateSettings in _listTZDateSettings)
            {
                if (dateSettings.DayType.DayTypeName.Equals(DayType.IMPLICIT_DAY_TYPE_HOLIDAY) || 
                    dateSettings.DayType.DayTypeName.Equals(DayType.IMPLICIT_DAY_TYPE_VACATION))
                {
                    _eErrors.Text += GetString(dateSettings.DayType.ToString()) + Environment.NewLine;
                }
                else
                {
                    _eErrors.Text += dateSettings.DayType.ToString() + Environment.NewLine;
                }               
            }
        }

        private void SetNewCalendar()
        {
            _timeZone.Calendar = _newCalendar;
            _timeZone.DateSettings.Clear();

            foreach (TimeZoneDateSetting dateSettings in _listNewTZDateSettings)
            {
                _timeZone.DateSettings.Add(dateSettings);
            }
            if (!_insert)
            {
                Exception error;
                CgpClient.Singleton.MainServerProvider.TimeZones.Update(_timeZone, out error);
            }
        }

        private void SetOldCalendar()
        {
            _timeZone.Calendar = _oldCalendar;
        }

        private void _bNewCalendar_Click(object sender, EventArgs e)
        {
            if (Contal.IwQuick.UI.Dialog.Question(GetString("QuestionSetNewCalendar")))
            {
                SetNewCalendar();
                this.DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void _bOldCalendar_Click(object sender, EventArgs e)
        {
            if (Contal.IwQuick.UI.Dialog.Question(GetString("QuestionSetOldCalendar")))
            {
                SetOldCalendar();
                this.DialogResult = DialogResult.Cancel;
                Close();
            }
        }
    }
}
