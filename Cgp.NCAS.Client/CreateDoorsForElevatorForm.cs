using System;
using System.Windows.Forms;

using Contal.Cgp.Client;
using Contal.Cgp.NCAS.RemotingCommon;
using Contal.IwQuick.Localization;
using Contal.IwQuick.UI;

namespace Contal.Cgp.NCAS.Client
{
    public partial class CreateDoorsForElevatorForm :
#if DESIGNER
    Form
#else
    CgpTranslateForm
#endif
    {
        private Guid _multiDoorId;
        private IMultiDoors _multiDoors;

        public CreateDoorsForElevatorForm()
            : base(NCASClient.LocalizationHelper)
        {
            InitializeComponent();
        }

        public static void ShowDialog(Guid multiDoorId, IMultiDoors multiDoors)
        {
            var createDoorsForElevatorForm = new CreateDoorsForElevatorForm
            {
                _multiDoorId = multiDoorId,
                _multiDoors = multiDoors
            };

            createDoorsForElevatorForm.ShowDialog();
        }

        private void _cbAssignDoorsToFloors_CheckedChanged(object sender, EventArgs e)
        {
            if (_cbAssignDoorsToFloors.Checked)
            {
                _cbCreatFloorsIfNotExist.Enabled = true;
                return;
            }

            _cbCreatFloorsIfNotExist.Checked = false;
            _cbCreatFloorsIfNotExist.Enabled = false;
        }

        private void _bCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void _bCreate_Click(object sender, EventArgs e)
        {
            if (_multiDoors != null)
            {
                if (_eFloorsCount.Value == 0)
                {
                    ControlNotification.Singleton.Error(
                        NotificationPriority.JustOne,
                        _eFloorsCount,
                        GetString("ErrorEntryFloorsCount"),
                        CgpClient.Singleton.ClientControlNotificationSettings);

                    return;
                }

                _multiDoors.CreateDoorsForElevator(
                    _multiDoorId,
                    GetString("StringFloor"),
                    (int) _eLowestFloorNumber.Value,
                    (int) _eFloorsCount.Value,
                    _cbAssignDoorsToFloors.Checked,
                    _cbCreatFloorsIfNotExist.Checked);
            }

            Close();
        }
    }
}
