using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Contal.Cgp.Client;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.IwQuick.Sys;
using Contal.IwQuick.UI;

namespace Contal.Cgp.NCAS.Client
{
    public partial class ControlModifyAlarmArcs : UserControl
    {
        public NCASClient Plugin { get; set; }

        public Action EditTextChanger;

        private ICollection<AlarmArc> _alarmArcs;

        public ICollection<AlarmArc> AlarmArcs
        {
            get { return _alarmArcs; }
            set { SetAlarmArcs(value, true); }
        }

        public bool AlarmArcsWasChanged { get; private set; }

        private void SetAlarmArcs(ICollection<AlarmArc> newAlarmArcs)
        {
            SetAlarmArcs(
                newAlarmArcs,
                false);
        }

        private void SetAlarmArcs(
            ICollection<AlarmArc> newAlarmArcs,
            bool resetWasChanged)
        {
            _alarmArcs = newAlarmArcs;

            ShowAlarmArcs();

            if (EditTextChanger != null)
                EditTextChanger();

            AlarmArcsWasChanged = !resetWasChanged;
        }

        public ControlModifyAlarmArcs()
        {
            InitializeComponent();

            _tbmAlarmArcs.MaximumSize = new Size(0, 0);

            _tbmAlarmArcs.ImageTextBox.ForeColor = CgpClientMainForm.Singleton.GetDragDropTextColor;
            _tbmAlarmArcs.ImageTextBox.BackColor = CgpClientMainForm.Singleton.GetDragDropBackgroundColor;
        }

        private void ShowAlarmArcs()
        {
            if (_alarmArcs == null
                || _alarmArcs.Count == 0)
            {
                _tbmAlarmArcs.Text = string.Empty;
                return;
            }

            var alarmArcNames = new StringBuilder();
            foreach (var alarmArc in _alarmArcs.OrderBy(alarmArc => alarmArc.ToString()))
            {
                if (alarmArcNames.Length > 0)
                    alarmArcNames.Append("; ");

                alarmArcNames.Append(alarmArc);
            }

            _tbmAlarmArcs.Text = alarmArcNames.ToString();

            if (Plugin != null)
                _tbmAlarmArcs.TextImage = Plugin.GetImageForObjectType(ObjectType.AlarmArc);
        }

        private void _tbmAlarmArc_ButtonPopupMenuItemClick(ToolStripItem item, int index)
        {
            if (item == _tsiModify)
            {
                ModifyAlarmArcs();
                return;
            }

            SetAlarmArcs(null);
        }

        private void ModifyAlarmArcs()
        {
            if (Plugin == null
                || CgpClient.Singleton.IsConnectionLost(true))
            {
                return;
            }

            try
            {
                Exception error;

                var alarmArcs =
                    Plugin.MainServerProvider.AlarmArcs.List(out error);

                if (error != null)
                    throw error;

                var addedAlarmArcIds = new HashSet<Guid>(
                    AlarmArcs != null
                        ? AlarmArcs.Select(
                            alarmArc =>
                                alarmArc.IdAlarmArc)
                        : Enumerable.Empty<Guid>());

                var checkedLisboxFormChange = new CheckedLisboxFormChange(
                    Plugin.GetTranslateString("NCASAlarmArcsFormNCASAlarmArcsForm"),
                    alarmArcs.Cast<object>(),
                    o =>
                    {
                        var alarmArc = o as AlarmArc;

                        return alarmArc != null
                               && addedAlarmArcIds.Contains(alarmArc.IdAlarmArc);
                    });


                ICollection<object> newCheckedAlarmArcs;

                if (!checkedLisboxFormChange.ShowDialog(out newCheckedAlarmArcs))
                    return;

                if (newCheckedAlarmArcs == null
                    || newCheckedAlarmArcs.Count == 0)
                {
                    SetAlarmArcs(null);

                    return;
                }

                foreach (var newCheckedAlarmArc in newCheckedAlarmArcs)
                {
                    Plugin.AddToRecentList(newCheckedAlarmArc);
                }

                SetAlarmArcs(new LinkedList<AlarmArc>(
                    newCheckedAlarmArcs.Cast<AlarmArc>()));
            }
            catch (Exception error)
            {
                Dialog.Error(error);
                HandledExceptionAdapter.Examine(error);
            }
        }

        private void AddAlarmArc(object newAlarmArc)
        {
            var alarmArc = newAlarmArc as AlarmArc;

            if (alarmArc == null)
            {
                ControlNotification.Singleton.Error(
                    NotificationPriority.JustOne,
                    _tbmAlarmArcs.ImageTextBox,
                    Plugin != null
                        ? Plugin.GetTranslateString("ErrorWrongObjectType")
                        : string.Empty,
                    ControlNotificationSettings.Default);

                return;
            }

            if (AlarmArcs != null)
            {
                var addedAlarmArcIds = new HashSet<Guid>(
                    AlarmArcs.Select(
                        addedAlarmArc =>
                            addedAlarmArc.IdAlarmArc));

                if (addedAlarmArcIds.Contains(alarmArc.IdAlarmArc))
                {
                    ControlNotification.Singleton.Error(
                        NotificationPriority.JustOne,
                        _tbmAlarmArcs.ImageTextBox,
                        Plugin != null
                            ? Plugin.GetTranslateString("ErrorAlarmArcAlreadyAdded")
                            : string.Empty,
                        ControlNotificationSettings.Default);

                    return;
                }
            }

            SetAlarmArcs(new LinkedList<AlarmArc>(
                (AlarmArcs ?? Enumerable.Empty<AlarmArc>())
                    .Concat(
                        Enumerable.Repeat(
                            alarmArc,
                            1))));

            if (Plugin != null)
                Plugin.AddToRecentList(alarmArc);
        }

        private void _tbmAlarmArc_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                var output = e.Data.GetFormats();

                if (output == null)
                    return;

                AddAlarmArc(e.Data.GetData(output[0]));
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        private void _tbmAlarmArcs_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }
    }
}
