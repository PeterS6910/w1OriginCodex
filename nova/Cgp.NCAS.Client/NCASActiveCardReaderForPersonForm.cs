using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Contal.Cgp.Client.PluginSupport;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.Client;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick;
using Contal.IwQuick.UI;

namespace Contal.Cgp.NCAS.Client
{
    public partial class NCASActiveCardReaderForPersonForm :
#if DESIGNER    
        Form
#else  
        PluginMainForm<NCASClient>
#endif
    {
        private readonly Person _actPerson;
        private BindingSource _bindingSource;
        private bool _first = true;
        DVoid2Void _dAfterTranslateForm;

        public override NCASClient Plugin
        {
            get { return NCASClient.Singleton; }
        }

        public NCASActiveCardReaderForPersonForm(Person person, Control control)
            : base(NCASClient.LocalizationHelper, CgpClientMainForm.Singleton)
        {
            InitializeComponent();
            LocalizationHelper.TranslateForm(this);
            _dAfterTranslateForm = AfterTranslateForm;
            _pBack.Parent = control;
            _actPerson = person;
            control.Enter += RunOnEnter;
            control.Disposed += RunOnDisposed;
            _dgValues.VisibleChanged += _dgValues_VisibleChanged;
        }

        protected override void AfterTranslateForm()
        {
            LocalizationHelper.TranslateControl(_pBack);
        }

        void RunOnEnter(object sender, EventArgs e)
        {
            if (_dAfterTranslateForm == null)
            {
                _dAfterTranslateForm = AfterTranslateForm;
                LocalizationHelper.LanguageChanged += _dAfterTranslateForm;
            }

            ShowActiveCardReader();
        }

        void RunOnDisposed(object sender, EventArgs e)
        {
            LocalizationHelper.LanguageChanged -= _dAfterTranslateForm;
        }

        void _dgValues_VisibleChanged(object sender, EventArgs e)
        {
            if (_first)
            {
                if (_bindingSource == null) return;
                if (_dgValues.DataSource == null) return;
                ShowActiveCardReader();
                _first = false;
            }
        }

        private void ShowActiveCardReader()
        {
            if (CgpClient.Singleton.IsConnectionLost(true)) return;

            Exception error;
            ICollection<CardReader> cardReaders = Plugin.MainServerProvider.ACLPersons.LoadActiveCardReaders(_actPerson, out error);

            if (error != null)
            {
                if (error is AccessDeniedException)
                {
                    Dialog.Error(CgpClient.Singleton.LocalizationHelper.GetString("ErrorLoadTableAccessDenied"));
                }
                else
                {
                    Dialog.Error(CgpClient.Singleton.LocalizationHelper.GetString("ErrorLoadTable"));
                }

                _bindingSource = null;
                _dgValues.DataSource = null;
                return;
            }

            _bindingSource = 
                new BindingSource
                {
                    DataSource = cardReaders
                };
            _dgValues.DataSource = _bindingSource;
            _dgValues.AutoGenerateColumns = false;
            _dgValues.AllowUserToAddRows = false;

            HideColumnDgw(_dgValues, CardReader.COLUMNIDCARDREADER);
            HideColumnDgw(_dgValues, CardReader.COLUMNGIN);
            HideColumnDgw(_dgValues, CardReader.COLUMNGINLENGTH);
            HideColumnDgw(_dgValues, CardReader.COLUMNSECURITYLEVEL);
            HideColumnDgw(_dgValues, CardReader.COLUMNISEMERGENCYCODE);
            HideColumnDgw(_dgValues, CardReader.COLUMNEMERGENCYCODE);
            HideColumnDgw(_dgValues, CardReader.COLUMNEMERGENCYCODELENGTH);
            HideColumnDgw(_dgValues, CardReader.COLUMNISFORCEDSECURITYLEVEL);
            HideColumnDgw(_dgValues, CardReader.COLUMNFORCEDSECURITYLEVEL);
            HideColumnDgw(_dgValues, CardReader.COLUMNSECURITYDAILYPLAN);
            HideColumnDgw(_dgValues, CardReader.COLUMNGUIDSECURITYDAILYPLAN);
            HideColumnDgw(_dgValues, CardReader.COLUMNSECURITYTIMEZONE);
            HideColumnDgw(_dgValues, CardReader.COLUMNGUIDSECURITYTIMEZONE);
            HideColumnDgw(_dgValues, CardReader.COLUMNDCU);
            HideColumnDgw(_dgValues, CardReader.COLUMNGUIDDCU);
            HideColumnDgw(_dgValues, CardReader.COLUMNDESCRIPTION);
            HideColumnDgw(_dgValues, CardReader.COLUMNAACARDREADERS);
            HideColumnDgw(_dgValues, CardReader.COLUMNONOFFOBJECTOBJECTTYPE);
            HideColumnDgw(_dgValues, CardReader.COLUMNONOFFOBJECTID);
            HideColumnDgw(_dgValues, CardReader.COLUMNONOFFOBJECT);
            HideColumnDgw(_dgValues, CardReader.COLUMNCCU);
            HideColumnDgw(_dgValues, CardReader.COLUMNPORT);
            HideColumnDgw(_dgValues, CardReader.COLUMNADDRESS);
            HideColumnDgw(_dgValues, CardReader.COLUMNGUIDCCU);
            HideColumnDgw(_dgValues, CardReader.COLUMNONOFFOBJECTTYPE);
            HideColumnDgw(_dgValues, CardReader.COLUMNALARMACCESSDENIED);
            HideColumnDgw(_dgValues, CardReader.COLUMN_BLOCK_ALARM_ACCESS_DENIED);
            HideColumnDgw(_dgValues, CardReader.COLUMN_OBJ_BLOCK_ALARM_ACCESS_DENIED_OBJECT_TYPE);
            HideColumnDgw(_dgValues, CardReader.COLUMN_OBJ_BLOCK_ALARM_ACCESS_DENIED_ID);
            HideColumnDgw(_dgValues, CardReader.COLUMN_OBJ_BLOCK_ALARM_ACCESS_DENIED);
            HideColumnDgw(_dgValues, CardReader.COLUMN_EVENTLOG_DURING_BLOCK_ALARM_ACCESS_DENIED);
            HideColumnDgw(_dgValues, CardReader.COLUMNALARMUNKNOWNCARD);
            HideColumnDgw(_dgValues, CardReader.COLUMN_BLOCK_ALARM_UNKNOWN_CARD);
            HideColumnDgw(_dgValues, CardReader.COLUMN_OBJ_BLOCK_ALARM_UNKNOWN_CARD_OBJECT_TYPE);
            HideColumnDgw(_dgValues, CardReader.COLUMN_OBJ_BLOCK_ALARM_UNKNOWN_CARD_ID);
            HideColumnDgw(_dgValues, CardReader.COLUMN_OBJ_BLOCK_ALARM_UNKNOWN_CARD);
            HideColumnDgw(_dgValues, CardReader.COLUMN_EVENTLOG_DURING_BLOCK_ALARM_UNKNOWN_CARD);
            HideColumnDgw(_dgValues, CardReader.COLUMNALARMCARDBLOCKEDORINACTIVE);
            HideColumnDgw(_dgValues, CardReader.COLUMN_BLOCK_ALARM_CARD_BLOCKED_OR_INACTIVE);
            HideColumnDgw(_dgValues, CardReader.COLUMN_OBJ_BLOCK_ALARM_CARD_BLOCKED_OR_INACTIVE_OBJECT_TYPE);
            HideColumnDgw(_dgValues, CardReader.COLUMN_OBJ_BLOCK_ALARM_CARD_BLOCKED_OR_INACTIVE_ID);
            HideColumnDgw(_dgValues, CardReader.COLUMN_OBJ_BLOCK_ALARM_CARD_BLOCKED_OR_INACTIVE);
            HideColumnDgw(_dgValues, CardReader.COLUMN_EVENTLOG_DURING_BLOCK_ALARM_CARD_BLOCKED_OR_INACTIVE);
            HideColumnDgw(_dgValues, CardReader.COLUMNALARMINVALIDPIN);
            HideColumnDgw(_dgValues, CardReader.COLUMN_BLOCK_ALARM_INVALID_PIN);
            HideColumnDgw(_dgValues, CardReader.COLUMN_OBJ_BLOCK_ALARM_INVALID_PIN_OBJECT_TYPE);
            HideColumnDgw(_dgValues, CardReader.COLUMN_OBJ_BLOCK_ALARM_INVALID_PIN_ID);
            HideColumnDgw(_dgValues, CardReader.COLUMN_OBJ_BLOCK_ALARM_INVALID_PIN);
            HideColumnDgw(_dgValues, CardReader.COLUMN_EVENTLOG_DURING_BLOCK_ALARM_INVALID_PIN);
            HideColumnDgw(_dgValues, CardReader.COLUMNALARMINVALIDGIN);
            HideColumnDgw(_dgValues, CardReader.COLUMN_BLOCK_ALARM_INVALID_GIN);
            HideColumnDgw(_dgValues, CardReader.COLUMN_OBJ_BLOCK_ALARM_INVALID_GIN_OBJECT_TYPE);
            HideColumnDgw(_dgValues, CardReader.COLUMN_OBJ_BLOCK_ALARM_INVALID_GIN_ID);
            HideColumnDgw(_dgValues, CardReader.COLUMN_OBJ_BLOCK_ALARM_INVALID_GIN);
            HideColumnDgw(_dgValues, CardReader.COLUMN_EVENTLOG_DURING_BLOCK_ALARM_INVALID_GIN);
            HideColumnDgw(_dgValues, CardReader.COLUMNALARMINVALIDEMERGENCYCODE);
            HideColumnDgw(_dgValues, CardReader.COLUMN_BLOCK_ALARM_INVALID_EMERGENCY_CODE);
            HideColumnDgw(_dgValues, CardReader.COLUMN_OBJ_BLOCK_ALARM_INVALID_EMERGENCY_CODE_OBJECT_TYPE);
            HideColumnDgw(_dgValues, CardReader.COLUMN_OBJ_BLOCK_ALARM_INVALID_EMERGENCY_CODE_ID);
            HideColumnDgw(_dgValues, CardReader.COLUMN_OBJ_BLOCK_ALARM_INVALID_EMERGENCY_CODE);
            HideColumnDgw(_dgValues, CardReader.COLUMN_EVENTLOG_DURING_BLOCK_ALARM_INVALID_EMERGENCY_CODE);
            HideColumnDgw(_dgValues, CardReader.COLUMNALARMACCESSPERMITTED);
            HideColumnDgw(_dgValues, CardReader.COLUMN_BLOCK_ALARM_ACCESS_PERMITTED);
            HideColumnDgw(_dgValues, CardReader.COLUMN_OBJ_BLOCK_ALARM_ACCESS_PERMITTED_OBJECT_TYPE);
            HideColumnDgw(_dgValues, CardReader.COLUMN_OBJ_BLOCK_ALARM_ACCESS_PERMITTED_ID);
            HideColumnDgw(_dgValues, CardReader.COLUMN_OBJ_BLOCK_ALARM_ACCESS_PERMITTED);
            HideColumnDgw(_dgValues, CardReader.COLUMN_EVENTLOG_DURING_BLOCK_ALARM_ACCESS_PERMITTED);
            HideColumnDgw(_dgValues, CardReader.COLUMNCRLANGUAGE);
            HideColumnDgw(_dgValues, CardReader.COLUMNCARDAPPLIEDLED);
            HideColumnDgw(_dgValues, CardReader.COLUMNCARDAPPLIEDKEYBOARDLIGHT);
            HideColumnDgw(_dgValues, CardReader.COLUMNCARDAPPLIEDINERNALBUZZER);
            HideColumnDgw(_dgValues, CardReader.COLUMNCARDAPPLIEDEXTERNALBUZER);
            HideColumnDgw(_dgValues, CardReader.COLUMNTAMPERLED);
            HideColumnDgw(_dgValues, CardReader.COLUMNTAMPERKEYBOARDLIGHT);
            HideColumnDgw(_dgValues, CardReader.COLUMNTAMPERINERNALBUZZER);
            HideColumnDgw(_dgValues, CardReader.COLUMNTAMPEREXTERNALBUZER);
            HideColumnDgw(_dgValues, CardReader.COLUMNRESETLED);
            HideColumnDgw(_dgValues, CardReader.COLUMNRESETKEYBOARDLIGHT);
            HideColumnDgw(_dgValues, CardReader.COLUMNRESETINERNALBUZZER);
            HideColumnDgw(_dgValues, CardReader.COLUMNRESETEXTERNALBUZER);
            HideColumnDgw(_dgValues, CardReader.COLUMNKEYPRESSEDLED);
            HideColumnDgw(_dgValues, CardReader.COLUMNKEYPRESSEDKEYBOARDLIGHT);
            HideColumnDgw(_dgValues, CardReader.COLUMNKEYPRESSEDINERNALBUZZER);
            HideColumnDgw(_dgValues, CardReader.COLUMNKEYPRESSEDEXTERNALBUZER);
            HideColumnDgw(_dgValues, CardReader.COLUMNINTERNALBUZERKILLSWITCH);
            HideColumnDgw(_dgValues, CardReader.COLUMNCARDREADERHARDWARE);
            HideColumnDgw(_dgValues, CardReader.COLUMNSLFORENTERTOMENU);
            HideColumnDgw(_dgValues, CardReader.COLUMN_USE_ACCESS_GIN_FOR_ENTER_TO_MENU);
            HideColumnDgw(_dgValues, CardReader.COLUMN_GIN_FOR_ENTER_TO_MENU);
            HideColumnDgw(_dgValues, CardReader.COLUMN_GIN_LENGTH_FOR_ENTER_TO_MENU);
            HideColumnDgw(_dgValues, CardReader.COLUMN_SECURITY_DAILY_PLAN_FOR_ENTER_TO_MENU);
            HideColumnDgw(_dgValues, CardReader.COLUMN_GUID_SSECURITY_DAILY_PLAN_FOR_ENTER_TO_MENU);
            HideColumnDgw(_dgValues, CardReader.COLUMN_SECURITY_TIME_ZONE_FOR_ENTER_TO_MENU);
            HideColumnDgw(_dgValues, CardReader.COLUMN_GUID_SECURITY_TIME_ZONE_FOR_ENTER_TO_MENU);
            HideColumnDgw(_dgValues, CardReader.COLUMNOBJECTTYPE);
            HideColumnDgw(_dgValues, CardReader.COLUMNALARMOFFLINE);
            HideColumnDgw(_dgValues, CardReader.COLUMN_BLOCK_ALARM_OFFLINE);
            HideColumnDgw(_dgValues, CardReader.COLUMN_OBJ_BLOCK_ALARM_OFFLINE_OBJECT_TYPE);
            HideColumnDgw(_dgValues, CardReader.COLUMN_OBJ_BLOCK_ALARM_OFFLINE_ID);
            HideColumnDgw(_dgValues, CardReader.COLUMN_OBJ_BLOCK_ALARM_OFFLINE);
            HideColumnDgw(_dgValues, CardReader.COLUMN_EVENTLOG_DURING_BLOCK_ALARM_OFFLINE);
            HideColumnDgw(_dgValues, CardReader.COLUMNALARMTAMPER);
            HideColumnDgw(_dgValues, CardReader.COLUMN_BLOCK_ALARM_TAMPER);
            HideColumnDgw(_dgValues, CardReader.COLUMN_OBJ_BLOCK_ALARM_TAMPER_OBJECT_TYPE);
            HideColumnDgw(_dgValues, CardReader.COLUMN_OBJ_BLOCK_ALARM_TAMPER_ID);
            HideColumnDgw(_dgValues, CardReader.COLUMN_OBJ_BLOCK_ALARM_TAMPER);
            HideColumnDgw(_dgValues, CardReader.COLUMN_EVENTLOG_DURING_BLOCK_ALARM_TAMPER);
            HideColumnDgw(_dgValues, CardReader.COLUMNCKUNIQUE);
            HideColumnDgw(_dgValues, CardReader.COLUMNENABLEPARENTINFULLNAME);
            HideColumnDgw(_dgValues, CardReader.COLUMNQUERYDBSTAMP);
            HideColumnDgw(_dgValues, CardReader.COLUMNSPECIALOUTPUTFORTAMPER);
            HideColumnDgw(_dgValues, CardReader.COLUMNGUIDSPECIALOUTPUTFORTAMPER);
            HideColumnDgw(_dgValues, CardReader.COLUMNSPECIALOUTPUTFOROFFLINE);
            HideColumnDgw(_dgValues, CardReader.COLUMNGUIDSPECIALOUTPUTFOROFFLINE);
            HideColumnDgw(_dgValues, CardReader.COLUMNLOCALALARMINSTRUCTION);
            HideColumnDgw(_dgValues, CardReader.COLUMNFUNCTIONKEY1);
            HideColumnDgw(_dgValues, CardReader.COLUMNFUNCTIONKEY2);
            HideColumnDgw(_dgValues, CardReader.COLUMN_INVALID_GIN_RETRIES_LIMIT_ENABLED);
            HideColumnDgw(_dgValues, CardReader.COLUMN_INVALID_PIN_RETRIES_LIMIT_ENABLED);
            HideColumnDgw(_dgValues, CardReader.COLUMN_ALARM_INVALID_GIN_RETRIES_LIMIT_REACHED);
            HideColumnDgw(_dgValues, CardReader.COLUMN_BLOCK_ALARM_INVALID_GIN_RETRIES_LIMIT_REACHED);
            HideColumnDgw(_dgValues, CardReader.COLUMN_OBJ_BLOCK_ALARM_INVALID_GIN_RETRIES_LIMIT_REACHED_OBJECT_TYPE);
            HideColumnDgw(_dgValues, CardReader.COLUMN_OBJ_BLOCK_ALARM_INVALID_GIN_RETRIES_LIMIT_REACHED_ID);


            HideColumnDgw(_dgValues, "QueryDbStamp");
             
            if (!_dgValues.Columns.Contains("State"))
                _dgValues.Columns.Add("State", "State");

            if (!_dgValues.Columns.Contains("str" + CardReader.COLUMNSECURITYLEVEL))
            {
                _dgValues.Columns.Add("str" + CardReader.COLUMNSECURITYLEVEL, "str" + CardReader.COLUMNSECURITYLEVEL);
                _dgValues.Columns["str" + CardReader.COLUMNSECURITYLEVEL].DisplayIndex = _dgValues.Columns[CardReader.COLUMNSECURITYLEVEL].DisplayIndex;
            }

            foreach (DataGridViewRow row in _dgValues.Rows)
            {
                var cardReader = _bindingSource.List[row.Index] as CardReader;
                if (cardReader != null)
                {
                    row.Cells["State"].Value = 
                        GetString(
                            Plugin.MainServerProvider.CardReaders.GetOnlineStates(cardReader.IdCardReader) == OnlineState.Online
                                ? "Online"
                                : "Offline");

                    SecurityLevelStates securityLevelStates = SecurityLevelStates.GetSecurityLevelState(LocalizationHelper, cardReader.SecurityLevel);
                    row.Cells["str" + CardReader.COLUMNSECURITYLEVEL].Value = securityLevelStates.Name;
                }
            }

            if (_dgValues.Columns.Contains(CardReader.COLUMNNAME))
                _dgValues.Columns[CardReader.COLUMNNAME].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            if (_dgValues.Columns.Contains("str" + CardReader.COLUMNSECURITYLEVEL))
                _dgValues.Columns["str" + CardReader.COLUMNSECURITYLEVEL].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;

            LocalizationHelper.TranslateDataGridViewColumnsHeaders(_dgValues);
        }

        protected void HideColumnDgw(DataGridView gridView, string columnName)
        {
            if (gridView == null) return;

            if (gridView.Columns.Contains(columnName))
                gridView.Columns[columnName].Visible = false;
        }

        private void _bRefresh_Click(object sender, EventArgs e)
        {
            ShowActiveCardReader();
        }

        private void _dgValues_DoubleClick(object sender, EventArgs e)
        {
            if (_bindingSource != null && _bindingSource.Count > 0)
            {
                var cardReader = _bindingSource[_bindingSource.Position] as CardReader;

                if (cardReader != null)
                    NCASCardReadersForm.Singleton.OpenEditForm(cardReader);
            }     
        }
    }
}
