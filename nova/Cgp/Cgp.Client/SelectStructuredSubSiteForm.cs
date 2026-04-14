using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Forms;
using Contal.Cgp.Client.StructuredSubSites;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick.Localization;
using Contal.IwQuick.PlatformPC.UI;
using Contal.IwQuick.UI;

namespace Contal.Cgp.Client
{
    public partial class SelectStructuredSubSiteForm : CgpTranslateForm
    {
        private SelectingOfSubSitesSupport _selectingOfSubSitesSupport;


        public SelectStructuredSubSiteForm()
            : base(CgpClient.Singleton.LocalizationHelper)
        {
            InitializeComponent();
        }

        public bool SelectStructuredSubSites(
            bool multiSelectEnabled,
            out ICollection<int> selectedSubSiteIds)
        {
            _selectingOfSubSitesSupport = new SelectingOfSubSitesSupport(
                _wpfCheckBoxesTreeview,
                null,
                multiSelectEnabled);

            _selectingOfSubSitesSupport.LoadSubSites();

            var subSites = _selectingOfSubSitesSupport.SubSites;

            if (subSites.Count == 0)
            {
                selectedSubSiteIds = null;
                return true;
            }

            if (subSites.Count == 1)
            {
                selectedSubSiteIds = subSites;
                return true;
            }

            var result = ShowDialog();

            if (result != DialogResult.OK)
            {
                selectedSubSiteIds = null;
                return false;
            }

            selectedSubSiteIds = _selectingOfSubSitesSupport.GetSelectedSiteIds();
            return true;
        }

        public static bool OnlyOneSubSiteIsRelevantForLogin(out StructuredSubSite subSite)
        {
            subSite = null;

            bool isInRootSite;
            var subSitesForLogin =
                CgpClient.Singleton.MainServerProvider.StructuredSubSites.GetAllSubSitesForLogin(
                    out isInRootSite);

            if (isInRootSite)
                return subSitesForLogin == null
                       || subSitesForLogin.Count == 0;

            if (subSitesForLogin == null
                || subSitesForLogin.Count == 0)
            {
                return false;
            }

            if (subSitesForLogin.Count > 1)
            {
                return false;
            }

            subSite = subSitesForLogin.First();
            return true;
        }

        private void _bCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void _bOk_Click(object sender, EventArgs e)
        {
            Close();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (DialogResult == DialogResult.OK
                && !IsCheckedSubSite(_wpfCheckBoxesTreeview.Items))
            {
                ControlNotification.Singleton.Error(
                    NotificationPriority.JustOne,
                    _ehWpfCheckBoxTreeview,
                    GetString("ErrorSelectSite"),
                    CgpClient.Singleton.ClientControlNotificationSettings);

                e.Cancel = true;
                return;
            }

            base.OnFormClosing(e);
        }

        private static bool IsCheckedSubSite(ItemCollection items)
        {
            foreach (CheckBoxTreeViewItem treeViewItem in items)
            {
                if (treeViewItem.IsChecked)
                    return true;

                if (IsCheckedSubSite(treeViewItem.Items))
                    return true;
            }

            return false;
        }
    }
}
