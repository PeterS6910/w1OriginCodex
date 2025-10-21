using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Contal.Cgp.BaseLib;
using Contal.Cgp.Client.PluginSupport;
using Contal.IwQuick.Sys;

namespace Contal.Cgp.Client
{
    public partial class AllPluginsForm :
#if DESIGNER
        Form
#else
 ACgpFullscreenForm
#endif
    {
        private bool _isLoaded = false;
        internal System.Windows.Forms.ImageList _imagesCgpLarge;


        public AllPluginsForm()
        {
            InitializeComponent();
            var baseFont = _lvCgp.Font;
            _lvCgp.Font = new Font(baseFont.FontFamily, baseFont.Size + 1.6f, baseFont.Style, baseFont.Unit);
            this.FormOnEnter += new Action<Form>(Form_Enter);
            _lvCgp.Groups.Add("cgp", LocalizationHelper.GetString("Info_Cgp"));
            _lvCgp.Groups.Add("ncas", LocalizationHelper.GetString("Info_Ncas"));
            _lvCgp.DoubleClick += new EventHandler(LvCgpDoubleClick);
            this.LocalizationHelper.LanguageChanged += new Contal.IwQuick.DVoid2Void(LocalizationHelper_LanguageChanged);
            RegisterToMain();
        }

        void LocalizationHelper_LanguageChanged()
        {
            _isLoaded = false;
            _lvCgp.Groups[0].Header = LocalizationHelper.GetString("Info_Cgp");
            _lvCgp.Groups[1].Header = LocalizationHelper.GetString("Info_Ncas");
            Contal.IwQuick.Threads.SafeThread.StartThread(InitCgpOrigins);
        }

        private static volatile AllPluginsForm _singleton = null;
        private static object _syncRoot = new object();

        public static AllPluginsForm Singleton
        {
            get
            {
                if (null == _singleton)
                    lock (_syncRoot)
                    {

                        if (null == _singleton)
                            _singleton = new AllPluginsForm();
                    }

                return _singleton;
            }
        }

        public void SelectPlugin(string name)
        {
            var item = _lvCgp.FindItemWithText(name);

            if (item != null)
            {
                item.Selected = true;
            }
        }

        protected override bool VerifySources()
        {
            return true;
        }

        private void Form_Enter(Form form)
        {
            Contal.IwQuick.Threads.SafeThread.StartThread(InitCgpOrigins);
        }

        public void ClearListView()
        {
            _isLoaded = false;
        }

        private void InitCgpOrigins()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Contal.IwQuick.DVoid2Void(InitCgpOrigins));
            }
            else
            {
                if (_isLoaded) return;
                try
                {
                    _lvCgp.SuspendLayout();
                    _lvCgp.Items.Clear();
                    _imagesCgpLarge = new ImageList();
                    _imagesCgpLarge.ColorDepth = ColorDepth.Depth32Bit;
                    _imagesCgpLarge.ImageSize = new Size(48, 48);
                    int i = 0;

                    if (StructuredSiteForm.Singleton.HasAccessView())
                    {
                        _imagesCgpLarge.Images.Add(ResourceGlobal.UserFoldersStructure);
                        ListViewItem lv =
                            new ListViewItem(
                                LocalizationHelper.GetString(
                                    StructuredSiteForm.Singleton.Name +
                                    StructuredSiteForm.Singleton.Name),
                                i);
                        lv.Name = StructuredSiteForm.Singleton.Name;
                        lv.Group = _lvCgp.Groups[0];
                        _lvCgp.Items.Add(lv);
                        i++;
                    }

                    if (PersonsForm.Singleton.HasAccessView())
                    {
                        _imagesCgpLarge.Images.Add(ResourceGlobal.PersonsNew48);
                        ListViewItem lv =
                            new ListViewItem(
                                LocalizationHelper.GetString(
                                    PersonsForm.Singleton.Name + PersonsForm.Singleton.Name),
                                i);
                        lv.Name = PersonsForm.Singleton.Name;
                        lv.Group = _lvCgp.Groups[0];
                        _lvCgp.Items.Add(lv);
                        i++;
                    }

                    if (LoginGroupsForm.Singleton.HasAccessView())
                    {
                        _imagesCgpLarge.Images.Add(ResourceGlobal.LoginGroup48);
                        ListViewItem lv =
                            new ListViewItem(
                                LocalizationHelper.GetString(
                                    LoginGroupsForm.Singleton.Name + LoginGroupsForm.Singleton.Name),
                                i);
                        lv.Name = LoginGroupsForm.Singleton.Name;
                        lv.Group = _lvCgp.Groups[0];
                        _lvCgp.Items.Add(lv);
                        i++;
                    }

                    if (LoginsForm.Singleton.HasAccessView())
                    {
                        _imagesCgpLarge.Images.Add(ResourceGlobal.Logins48);
                        ListViewItem lv =
                            new ListViewItem(
                                LocalizationHelper.GetString(
                                    LoginsForm.Singleton.Name + LoginsForm.Singleton.Name),
                                i);
                        lv.Name = LoginsForm.Singleton.Name;
                        lv.Group = _lvCgp.Groups[0];
                        _lvCgp.Items.Add(lv);
                        i++;
                    }

                    if (EventlogsForm.Singleton.HasAccessView())
                    {
                        _imagesCgpLarge.Images.Add(ResourceGlobal.EventLogsNew48);
                        ListViewItem lv =
                            new ListViewItem(
                                LocalizationHelper.GetString(
                                    EventlogsForm.Singleton.Name + EventlogsForm.Singleton.Name),
                                i);
                        lv.Name = EventlogsForm.Singleton.Name;
                        lv.Group = _lvCgp.Groups[0];
                        _lvCgp.Items.Add(lv);
                        i++;
                    }
                    //checkAccess = SystemEventsEditForm.GetCheckAccesControl();
                    //if (checkAccess != null && checkAccess.View)
                    //{
                    //    _imagesCgpLarge.Images.Add(SystemEventsEditForm.Singleton.Icon);
                    //    ListViewItem lv = new ListViewItem(LocalizationHelper.GetString(SystemEventsEditForm.Singleton.Name + SystemEventsEditForm.Singleton.Name), i);
                    //    lv.Name = SystemEventsEditForm.Singleton.Name;
                    //    _lvCgp.Items.Add(lv);
                    //    i++;
                    //}

                    if (DailyPlansForm.Singleton.HasAccessView())
                    {
                        _imagesCgpLarge.Images.Add(ResourceGlobal.DailyPLan48);
                        ListViewItem lv =
                            new ListViewItem(
                                LocalizationHelper.GetString(
                                    DailyPlansForm.Singleton.Name + DailyPlansForm.Singleton.Name),
                                i);
                        lv.Name = DailyPlansForm.Singleton.Name;
                        lv.Group = _lvCgp.Groups[0];
                        _lvCgp.Items.Add(lv);
                        i++;
                    }

                    if (TimeZonesForm.Singleton.HasAccessView())
                    {
                        _imagesCgpLarge.Images.Add(ResourceGlobal.TimeZone48);
                        ListViewItem lv =
                            new ListViewItem(
                                LocalizationHelper.GetString(
                                    TimeZonesForm.Singleton.Name + TimeZonesForm.Singleton.Name),
                                i);
                        lv.Name = TimeZonesForm.Singleton.Name;
                        lv.Group = _lvCgp.Groups[0];
                        _lvCgp.Items.Add(lv);
                        i++;
                    }

                    if (DayTypesForm.Singleton.HasAccessView())
                    {
                        _imagesCgpLarge.Images.Add(ResourceGlobal.DayType48);
                        ListViewItem lv =
                            new ListViewItem(
                                LocalizationHelper.GetString(
                                    DayTypesForm.Singleton.Name + DayTypesForm.Singleton.Name),
                                i);
                        lv.Name = DayTypesForm.Singleton.Name;
                        lv.Group = _lvCgp.Groups[0];
                        _lvCgp.Items.Add(lv);
                        i++;
                    }

                    if (CalendarsForm.Singleton.HasAccessView())
                    {
                        _imagesCgpLarge.Images.Add(ResourceGlobal.Calendar48);
                        ListViewItem lv =
                            new ListViewItem(
                                LocalizationHelper.GetString(
                                    CalendarsForm.Singleton.Name + CalendarsForm.Singleton.Name),
                                i);
                        lv.Name = CalendarsForm.Singleton.Name;
                        lv.Group = _lvCgp.Groups[0];
                        _lvCgp.Items.Add(lv);
                        i++;
                    }

                    if (CardsForm.Singleton.HasAccessView())
                    {
                        _imagesCgpLarge.Images.Add(ResourceGlobal.CardsNew48);
                        ListViewItem lv =
                            new ListViewItem(
                                LocalizationHelper.GetString(
                                    CardsForm.Singleton.Name + CardsForm.Singleton.Name),
                                i);
                        lv.Name = CardsForm.Singleton.Name;
                        lv.Group = _lvCgp.Groups[0];
                        _lvCgp.Items.Add(lv);
                        i++;
                    }

                    if (CardTemplatesForm.Singleton.HasAccessView())
                    {
                        _imagesCgpLarge.Images.Add(ResourceGlobal.CardTemplate48);
                        ListViewItem lv =
                            new ListViewItem(
                                LocalizationHelper.GetString(
                                    CardTemplatesForm.Singleton.Name +
                                    CardTemplatesForm.Singleton.Name),
                                i);
                        lv.Name = CardTemplatesForm.Singleton.Name;
                        lv.Group = _lvCgp.Groups[0];
                        _lvCgp.Items.Add(lv);
                        i++;
                    }

                    if (CardSystemsForm.Singleton.HasAccessView())
                    {
                        _imagesCgpLarge.Images.Add(ResourceGlobal.CardSystemNew48);
                        ListViewItem lv =
                            new ListViewItem(
                                LocalizationHelper.GetString(
                                    CardSystemsForm.Singleton.Name + CardSystemsForm.Singleton.Name),
                                i);
                        lv.Name = CardSystemsForm.Singleton.Name;
                        lv.Group = _lvCgp.Groups[0];
                        _lvCgp.Items.Add(lv);
                        i++;
                    }

                    if (CarsForm.Singleton.HasAccessView())
                    {
                        _imagesCgpLarge.Images.Add(ResourceGlobal.Car16);
                        ListViewItem lv =
                            new ListViewItem(
                                LocalizationHelper.GetString(
                                    CarsForm.Singleton.Name + CarsForm.Singleton.Name),
                                i);
                        lv.Name = CarsForm.Singleton.Name;
                        lv.Group = _lvCgp.Groups[0];
                        _lvCgp.Items.Add(lv);
                        i++;
                    }

                    if (PresentationGroupsForm.Singleton.HasAccessView())
                    {
                        _imagesCgpLarge.Images.Add(ResourceGlobal.PresentationGroup48);
                        ListViewItem lv =
                            new ListViewItem(
                                LocalizationHelper.GetString(
                                    PresentationGroupsForm.Singleton.Name +
                                    PresentationGroupsForm.Singleton.Name),
                                i);
                        lv.Name = PresentationGroupsForm.Singleton.Name;
                        lv.Group = _lvCgp.Groups[0];
                        _lvCgp.Items.Add(lv);
                        i++;
                    }

                    if (PresentationFormattersForm.Singleton.HasAccessView())
                    {
                        _imagesCgpLarge.Images.Add(ResourceGlobal.Formater48);
                        ListViewItem lv =
                            new ListViewItem(
                                LocalizationHelper.GetString(
                                    PresentationFormattersForm.Singleton.Name +
                                    PresentationFormattersForm.Singleton.Name),
                                i);
                        lv.Name = PresentationFormattersForm.Singleton.Name;
                        lv.Group = _lvCgp.Groups[0];
                        _lvCgp.Items.Add(lv);
                        i++;
                    }

                    if (CisNGsForm.Singleton.HasAccessView())
                    {
                        _imagesCgpLarge.Images.Add(ResourceGlobal.CisNG48);
                        ListViewItem lv =
                            new ListViewItem(
                                LocalizationHelper.GetString(
                                    CisNGsForm.Singleton.Name + CisNGsForm.Singleton.Name),
                                i);
                        lv.Name = CisNGsForm.Singleton.Name;
                        lv.Group = _lvCgp.Groups[0];
                        _lvCgp.Items.Add(lv);
                        i++;
                    }

                    if (CisNGGroupsForm.Singleton.HasAccessView())
                    {
                        _imagesCgpLarge.Images.Add(ResourceGlobal.CisNGGrpup48);
                        ListViewItem lv =
                            new ListViewItem(
                                LocalizationHelper.GetString(
                                    CisNGGroupsForm.Singleton.Name + CisNGGroupsForm.Singleton.Name),
                                i);
                        lv.Name = CisNGGroupsForm.Singleton.Name;
                        lv.Group = _lvCgp.Groups[0];
                        _lvCgp.Items.Add(lv);
                        i++;
                    }

                    if (GlobalAlarmInstructionsForm.Singleton.HasAccessView())
                    {
                        _imagesCgpLarge.Images.Add(ResourceGlobal.AlarmInstructions48);
                        ListViewItem lv =
                            new ListViewItem(
                                LocalizationHelper.GetString(
                                    GlobalAlarmInstructionsForm.Singleton.Name +
                                    GlobalAlarmInstructionsForm.Singleton.Name),
                                i);
                        lv.Name = GlobalAlarmInstructionsForm.Singleton.Name;
                        lv.Group = _lvCgp.Groups[0];
                        _lvCgp.Items.Add(lv);
                        i++;
                    }

                    //Insert object from plugins
                    ICollection<ICgpVisualPlugin> plugins =
                        CgpClient.Singleton.PluginManager
                            .GetVisualPlugins()
                            .ToList();

                    //int i = 0;
                    foreach (ICgpVisualPlugin p in plugins)
                    {
                        if (p != null)
                        {
                            foreach (IPluginMainForm subForm in p.SubForms)
                            {
                                if (subForm.HasAccessView())
                                {
                                    if (subForm.FormImage == null)
                                    {
                                        _imagesCgpLarge.Images.Add(subForm.Icon);
                                    }
                                    else
                                    {
                                        _imagesCgpLarge.Images.Add(subForm.FormImage);
                                    }
                                    ListViewItem lv =
                                        new ListViewItem(p.GetTranslateNameForm((Form)subForm), i);
                                    lv.Name = subForm.Name;
                                    lv.Group = _lvCgp.Groups[1];
                                    _lvCgp.Items.Add(lv);
                                    i++;
                                }
                            }
                        }
                    }

                    _lvCgp.LargeImageList = _imagesCgpLarge;
                    _isLoaded = true;
                }
                catch(Exception error)
                {
                    HandledExceptionAdapter.Examine(error);
                }
                finally
                {
                    _lvCgp.ResumeLayout();
                }
            }
        }

        private void OpenTableForm(string name)
        {
            ICollection<ICgpVisualPlugin> plugins = 
                CgpClient.Singleton.PluginManager
                    .GetVisualPlugins()
                    .ToList();

            foreach (ICgpVisualPlugin p in plugins)
            {
                if (p != null)
                {
                    foreach (IPluginMainForm subForm in p.SubForms)
                    {
                        if (name == subForm.Name)
                        {
                            subForm.Show();
                            return;
                        }
                    }//foreach subForms
                }
            }//foreach plugins
        }


        void LvCgpDoubleClick(object sender, EventArgs e)
        {
            if (CgpClient.Singleton.IsConnectionLost(true)) return;

            if (_lvCgp.FocusedItem.Name == PersonsForm.Singleton.Name)
            {
                PersonsForm.Singleton.Show();
            }
            else if (_lvCgp.FocusedItem.Name == LoginGroupsForm.Singleton.Name)
            {
                LoginGroupsForm.Singleton.Show();
            }
            else if (_lvCgp.FocusedItem.Name == LoginsForm.Singleton.Name)
            {
                LoginsForm.Singleton.Show();
            }
            else if (_lvCgp.FocusedItem.Name == PresentationGroupsForm.Singleton.Name)
            {
                PresentationGroupsForm.Singleton.Show();
            }
            else if (_lvCgp.FocusedItem.Name == PresentationFormattersForm.Singleton.Name)
            {
                PresentationFormattersForm.Singleton.Show();
            }
            else if (_lvCgp.FocusedItem.Name == EventlogsForm.Singleton.Name)
            {
                EventlogsForm.Singleton.Show();
            }
            else if (_lvCgp.FocusedItem.Name == SystemEventsEditForm.Singleton.Name)
            {
                SystemEventsEditForm.Singleton.Show();
            }
            else if (_lvCgp.FocusedItem.Name == CisNGsForm.Singleton.Name)
            {
                CisNGsForm.Singleton.Show();
            }
            else if (_lvCgp.FocusedItem.Name == CisNGGroupsForm.Singleton.Name)
            {
                CisNGGroupsForm.Singleton.Show();
            }
            else if (_lvCgp.FocusedItem.Name == CardSystemsForm.Singleton.Name)
            {
                CardSystemsForm.Singleton.Show();
            }
            else if (_lvCgp.FocusedItem.Name == CardsForm.Singleton.Name)
            {
                CardsForm.Singleton.Show();
            }
            else if (_lvCgp.FocusedItem.Name == CardTemplatesForm.Singleton.Name)
            {
                CardTemplatesForm.Singleton.Show();
            }
            else if (_lvCgp.FocusedItem.Name == CarsForm.Singleton.Name)
            {
                CarsForm.Singleton.Show();
            }
            else if (_lvCgp.FocusedItem.Name == DailyPlansForm.Singleton.Name)
            {
                DailyPlansForm.Singleton.Show();
            }
            else if (_lvCgp.FocusedItem.Name == TimeZonesForm.Singleton.Name)
            {
                TimeZonesForm.Singleton.Show();
            }
            else if (_lvCgp.FocusedItem.Name == DayTypesForm.Singleton.Name)
            {
                DayTypesForm.Singleton.Show();
            }
            else if (_lvCgp.FocusedItem.Name == CalendarsForm.Singleton.Name)
            {
                CalendarsForm.Singleton.Show();
            }
            else if (_lvCgp.FocusedItem.Name == GlobalAlarmInstructionsForm.Singleton.Name)
            {
                GlobalAlarmInstructionsForm.Singleton.Show();
            }
            else if (_lvCgp.FocusedItem.Name == StructuredSiteForm.Singleton.Name)
            {
                StructuredSiteForm.Singleton.Show();
            }
            else
            {
                OpenTableForm(_lvCgp.FocusedItem.Name);
            }
        }
    }
}
