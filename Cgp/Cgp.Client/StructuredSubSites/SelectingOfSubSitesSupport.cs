using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Forms;

using Contal.IwQuick;
using Contal.IwQuick.Data;
using Contal.IwQuick.PlatformPC.UI;
using Contal.IwQuick.Sys;
using Contal.IwQuick.UI;
using CheckBox = System.Windows.Forms.CheckBox;

namespace Contal.Cgp.Client.StructuredSubSites
{
    public class SelectingOfSubSitesSupport
    {
        private readonly WpfCheckBoxesTreeview _treeView;
        private readonly CheckBox _selectAllSubSites;
        private readonly ICollection<int> _allSubSites = new LinkedList<int>();
        private readonly ICollection<int> _selectedSubSites = new HashSet<int>();
        private readonly ProcessingQueue<CheckBoxTreeViewItem> _selectOrUnselectSite;
        private readonly bool _multiSelectEnabled = true;
        private bool _processItemCheckChanged = true;

        public event DVoid2Void SelectedSitesChanged;

        public ICollection<int> SubSites
        {
            get
            {
                return new LinkedList<int>(_allSubSites);
            }
        }

        public SelectingOfSubSitesSupport(
            WpfCheckBoxesTreeview treeView,
            CheckBox selectAllSubSites)
            : this(treeView, selectAllSubSites, true)
        {

        }

        public SelectingOfSubSitesSupport(
            WpfCheckBoxesTreeview treeView,
            CheckBox selectAllSubSites,
            bool multiSelectEnabled)
        {
            _treeView = treeView;
            _selectAllSubSites = selectAllSubSites;

            _selectOrUnselectSite = new ProcessingQueue<CheckBoxTreeViewItem>();
            _selectOrUnselectSite.ItemProcessing += SelectOrUnselectSite;

            _treeView.ItemCheckedChanged += ItemCheckedChanged;

            if (_selectAllSubSites != null)
                _selectAllSubSites.CheckStateChanged += CheckStateChanged;
            else
                _multiSelectEnabled = multiSelectEnabled;
        }

        private void ItemCheckedChanged(CheckBoxTreeViewItem treeViewItem)
        {
            if (treeViewItem == null)
            {
                return;
            }

            if (!_processItemCheckChanged)
                return;

            _selectOrUnselectSite.Enqueue(treeViewItem);
        }

        private void SelectOrUnselectSite(CheckBoxTreeViewItem treeViewItem)
        {
            _processItemCheckChanged = false;
            
            _treeView.Parent.Dispatcher.Invoke(
                new Action<CheckBoxTreeViewItem>(DoSelectOrUnselectSite),
                treeViewItem);
            
            _processItemCheckChanged = true;
        }

        private void DoSelectOrUnselectSite(CheckBoxTreeViewItem treeViewItem)
        {
            if (SelectedSitesChanged != null)
                SelectedSitesChanged();

            try
            {
                if (!_multiSelectEnabled
                    && treeViewItem.IsChecked)
                {
                    var selectedSubSiteId = (int) treeViewItem.Tag;

                    UnselectAllSites(
                        new HashSet<int>(
                            Enumerable.Repeat(
                                selectedSubSiteId,
                                1)),
                        _treeView.Items);
                }

                if (treeViewItem.IsChecked)
                {
                    _selectedSubSites.Add((int) treeViewItem.Tag);

                    if ((int) treeViewItem.Tag != -1
                        && treeViewItem.Items.Count > 0
                        && !IsSelecedtAllSites(treeViewItem.Items))
                    {
                        if (_multiSelectEnabled
                            && Dialog.Question(
                                CgpClient.Singleton.LocalizationHelper.GetString(
                                    "QuestionSelectAllSubSites")))
                        {
                            SelectAllSites(treeViewItem.Items);
                        }
                    }

                    if (IsSelecedtAllSites(_treeView.Items))
                    {
                        SetCheckStateForSelectAllSubSites(CheckState.Checked);
                        return;
                    }
                }
                else
                {
                    _selectedSubSites.Remove((int) treeViewItem.Tag);

                    if ((int) treeViewItem.Tag != -1
                        && treeViewItem.Items.Count > 0
                        && !IsUnselecedtAllSites(treeViewItem.Items))
                    {
                        if (_multiSelectEnabled
                            && Dialog.Question(
                                CgpClient.Singleton.LocalizationHelper.GetString(
                                    "QuestionUnselectAllSubSites")))
                        {
                            UnselectAllSites(
                                null,
                                treeViewItem.Items);
                        }
                    }

                    if (IsUnselecedtAllSites(_treeView.Items))
                    {
                        SetCheckStateForSelectAllSubSites(CheckState.Unchecked);
                        return;
                    }
                }

                SetCheckStateForSelectAllSubSites(CheckState.Indeterminate);
            }
            finally
            {
                treeViewItem.IsSelected = true;
            }
        }

        private bool _disableAllSubSitesCheckedChanged;

        private void SetCheckStateForSelectAllSubSites(CheckState checkState)
        {
            try
            {
                _disableAllSubSitesCheckedChanged = true;

                if (_selectAllSubSites != null)
                {
                    _selectAllSubSites.CheckState = checkState;
                }
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
            finally
            {
                _disableAllSubSitesCheckedChanged = false;
            }
        }

        private CheckState GetCheckStateForSelectAllSubSites()
        {
            if (_selectAllSubSites == null)
                return CheckState.Unchecked;

            return _selectAllSubSites.CheckState;
        }

        private void CheckStateChanged(object sender, EventArgs e)
        {
            if (_disableAllSubSitesCheckedChanged)
                return;

            if (SelectedSitesChanged != null)
                SelectedSitesChanged();

            if (_selectAllSubSites.Checked)
            {
                SelectAllSites(_treeView.Items);
                return;
            }

            UnselectAllSites();
        }

        private void SelectAllSites(ItemCollection items)
        {
            if (items == null)
                return;

            foreach (CheckBoxTreeViewItem treeViewItem in items)
            {
                treeViewItem.IsChecked = true;
                _selectedSubSites.Add((int)treeViewItem.Tag);

                SelectAllSites(treeViewItem.Items);
            }
        }

        private void UnselectAllSites()
        {
            UnselectAllSites(
                null,
                _treeView.Items);
        }

        private void UnselectAllSites(
            HashSet<int> ignoredSubSites,
            ItemCollection items)
        {
            if (items == null)
                return;

            foreach (CheckBoxTreeViewItem treeViewItem in items)
            {
                var subSiteId = (int) treeViewItem.Tag;

                if (ignoredSubSites == null
                    || !ignoredSubSites.Contains(subSiteId))
                {
                    treeViewItem.IsChecked = false;
                    _selectedSubSites.Remove(subSiteId);
                }

                UnselectAllSites(
                    ignoredSubSites,
                    treeViewItem.Items);
            }
        }

        private bool IsSelecedtAllSites(ItemCollection items)
        {
            if (items == null)
                return true;

            foreach (CheckBoxTreeViewItem treeViewItem in items)
            {
                if (!treeViewItem.IsChecked)
                    return false;

                if (!IsSelecedtAllSites(treeViewItem.Items))
                    return false;
            }

            return true;
        }

        private bool IsUnselecedtAllSites(ItemCollection items)
        {
            if (items == null)
                return true;

            foreach (CheckBoxTreeViewItem treeViewItem in items)
            {
                if (treeViewItem.IsChecked)
                    return false;

                if (!IsUnselecedtAllSites(treeViewItem.Items))
                    return false;
            }

            return true;
        }

        public void LoadSubSites()
        {
            _treeView.Parent.Dispatcher.Invoke(new Action(DoLoadSubSites));
        }


        public void DoLoadSubSites()
        {
            try
            {
                _treeView.Items.Clear();

                var selectAllSubSites = GetCheckStateForSelectAllSubSites() == CheckState.Checked;

                if (selectAllSubSites)
                    _selectedSubSites.Clear();

                bool isInRootSite;
                var subSitesForLogin =
                    CgpClient.Singleton.MainServerProvider.StructuredSubSites.GetAllSubSitesForLogin(
                        out isInRootSite);

                var rootTreeViewItem = isInRootSite
                    ? new CheckBoxTreeViewItem
                    {
                        Text = CgpClient.Singleton.LocalizationHelper.GetString(
                            "SelectStructuredSubSiteForm_RootNode")
                    }
                    : null;

                _allSubSites.Clear();

                if (subSitesForLogin != null)
                {
                    var structuredSubSitesNodes = new Dictionary<int, CheckBoxTreeViewItem>();
                    foreach (var structuredSubSite in subSitesForLogin)
                    {
                        _allSubSites.Add(structuredSubSite.IdStructuredSubSite);

                        var treeViewItem = new CheckBoxTreeViewItem
                        {
                            Text = structuredSubSite.Name,
                            Tag = structuredSubSite.IdStructuredSubSite
                        };

                        structuredSubSitesNodes.Add(structuredSubSite.IdStructuredSubSite, treeViewItem);

                        if (selectAllSubSites)
                        {
                            _selectedSubSites.Add(structuredSubSite.IdStructuredSubSite);
                            treeViewItem.IsChecked = true;

                            continue;
                        }

                        if (_selectedSubSites.Contains(structuredSubSite.IdStructuredSubSite))
                        {
                            treeViewItem.IsChecked = true;
                        }
                    }

                    if (!selectAllSubSites)
                    {
                        var notExistSelectedSubSites = new LinkedList<int>(
                            _selectedSubSites.Where(
                                selectedSubSite =>
                                    selectedSubSite != -1
                                    && !structuredSubSitesNodes.ContainsKey(selectedSubSite)));

                        foreach (var notExistSelectedSubSite in notExistSelectedSubSites)
                        {
                            _selectedSubSites.Remove(notExistSelectedSubSite);
                        }
                    }

                    foreach (
                        var structuredSubSite in subSitesForLogin.OrderBy(structuredSubSite => structuredSubSite.Name))
                    {
                        CheckBoxTreeViewItem treeViewItem;
                        if (structuredSubSitesNodes.TryGetValue(structuredSubSite.IdStructuredSubSite, out treeViewItem))
                        {
                            if (structuredSubSite.ParentSite != null)
                            {
                                CheckBoxTreeViewItem parentTreeViewItem;

                                if (structuredSubSitesNodes.TryGetValue(
                                    structuredSubSite.ParentSite.IdStructuredSubSite,
                                    out parentTreeViewItem))
                                {
                                    parentTreeViewItem.Items.Add(treeViewItem);
                                    continue;
                                }
                            }
                            else
                            {
                                if (rootTreeViewItem != null)
                                {
                                    rootTreeViewItem.Items.Add(treeViewItem);
                                    continue;
                                }
                            }

                            _treeView.Items.Add(treeViewItem);
                        }
                    }
                }

                if (rootTreeViewItem != null)
                {
                    _allSubSites.Add(-1);

                    rootTreeViewItem.Tag = -1;
                    _treeView.Items.Add(rootTreeViewItem);
                    rootTreeViewItem.IsExpanded = true;

                    if (selectAllSubSites)
                    {
                        _selectedSubSites.Add(-1);
                        rootTreeViewItem.IsChecked = true;
                    }
                    else if (_selectedSubSites.Contains(-1))
                    {
                        rootTreeViewItem.IsChecked = true;
                    }
                }
                else
                {
                    _selectedSubSites.Remove(-1);
                }
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        public void ClearSubSites()
        {
            if (_treeView == null
                || _treeView.Parent == null)
            {
                return;
            }

            _treeView.Parent.Dispatcher.Invoke(new DVoid2Void(DoClearSubSites));
        }

        public void DoClearSubSites()
        {
            _selectedSubSites.Clear();
            _treeView.Items.Clear();
            SetCheckStateForSelectAllSubSites(CheckState.Checked);
        }

        public ICollection<int> GetSelectedSiteIds()
        {
            if (_selectAllSubSites != null
                && _selectAllSubSites.CheckState == CheckState.Checked)
            {
                return null;
            }

            return new LinkedList<int>(_selectedSubSites);
        }
    }
}
