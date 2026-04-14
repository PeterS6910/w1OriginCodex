using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Contal.Cgp.Client.PluginSupport;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.IwQuick.Localization;
using Contal.Cgp.Client;
using Contal.IwQuick.UI;

namespace Contal.Cgp.NCAS.Client
{
    public partial class NCASSecurityTimeZoneCalendarChangeForm : CgpTranslateForm
    {
        private readonly Calendar _oldCalendar;
        private readonly Calendar _newCalendar;
        private readonly SecurityTimeZone _securityTimeZone;
        private readonly ICollection<SecurityTimeZoneDateSetting> _listTZDateSettings = new List<SecurityTimeZoneDateSetting>();
        private readonly ICollection<SecurityTimeZoneDateSetting> _listNewTZDateSettings = new List<SecurityTimeZoneDateSetting>();
        private readonly bool _insert;
        private readonly PluginMainForm<NCASClient> _mainPlugin;

        public NCASSecurityTimeZoneCalendarChangeForm(Calendar oldCalendar, Calendar newCalendar, SecurityTimeZone timeZone, bool insert,
            LocalizationHelper localizationHelper, PluginMainForm<NCASClient> mainPlugin)
            : base(localizationHelper)
        {
            InitializeComponent();
            _oldCalendar = oldCalendar;
            _newCalendar = newCalendar;
            _securityTimeZone = timeZone;
            _insert = insert;
            _mainPlugin = mainPlugin;
            ShowDateSettingsErrors();

        }

        private void ShowDateSettingsErrors()
        {
            foreach (SecurityTimeZoneDateSetting tds in _securityTimeZone.DateSettings)
            {
                bool notOwned = true;
                SecurityTimeZoneDateSetting dateSettings = 
                    _mainPlugin.Plugin.MainServerProvider.SecurityTimeZoneDateSettings.GetObjectById(tds.IdSecurityTimeZoneDateSetting);
                
                if (dateSettings.DayType != null)
                {
                    if (_newCalendar != null && _newCalendar.DateSettings != null)
                    {
                        foreach (CalendarDateSetting ds in _newCalendar.DateSettings)
                        {
                            DayType day =  CgpClient.Singleton.MainServerProvider.DayTypes.GetObjectById(ds.DayType.IdDayType);
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

                SecurityTimeZoneDateSetting missedTzDs;
                if (notOwned)
                {
                    missedTzDs = _mainPlugin.Plugin.MainServerProvider.SecurityTimeZoneDateSettings.GetObjectById(dateSettings.IdSecurityTimeZoneDateSetting);
                    _listTZDateSettings.Add(missedTzDs);
                }
                else
                {
                    missedTzDs = _mainPlugin.Plugin.MainServerProvider.SecurityTimeZoneDateSettings.GetObjectById(dateSettings.IdSecurityTimeZoneDateSetting);
                    _listNewTZDateSettings.Add(missedTzDs);
                }
            }

            _eErrors.Text = string.Empty;
            foreach (SecurityTimeZoneDateSetting dateSettings in _listTZDateSettings)
            {
                if (dateSettings.DayType.DayTypeName.Equals(DayType.IMPLICIT_DAY_TYPE_HOLIDAY) ||
                      dateSettings.DayType.DayTypeName.Equals(DayType.IMPLICIT_DAY_TYPE_VACATION))
                {
                    _eErrors.Text += CgpClient.Singleton.LocalizationHelper.GetString(dateSettings.DayType.ToString()) + Environment.NewLine;
                }
                else
                {
                    _eErrors.Text += dateSettings.DayType + Environment.NewLine;
                } 
            }
        }

        private void SetNewCalendar()
        {
            _securityTimeZone.Calendar = _newCalendar;
            _securityTimeZone.DateSettings.Clear();

            foreach (SecurityTimeZoneDateSetting dateSettings in _listNewTZDateSettings)
            {
                _securityTimeZone.DateSettings.Add(dateSettings);
            }
            if (!_insert)
            {
                Exception error;
                _mainPlugin.Plugin.MainServerProvider.SecurityTimeZones.Update(_securityTimeZone, out error);
            }
        }

        private void SetOldCalendar()
        {
            _securityTimeZone.Calendar = _oldCalendar;
        }

        private void _bNewCalendar_Click(object sender, EventArgs e)
        {
            if (Dialog.Question(GetString("QuestionSetNewCalendar")))
            {
                SetNewCalendar();
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void _bOldCalendar_Click(object sender, EventArgs e)
        {
            if (Dialog.Question(GetString("QuestionSetOldCalendar")))
            {
                SetOldCalendar();
                DialogResult = DialogResult.Cancel;
                Close();
            }
        }
    }
}

