using System;
using System.Windows.Forms;

using Contal.Cgp.Client;
using Contal.Cgp.NCAS.RemotingCommon;
using Contal.IwQuick.Localization;
using Contal.IwQuick.UI;

namespace Contal.Cgp.NCAS.Client
{
    public partial class CreateDoorsForMultiDoorForm :
#if DESIGNER
    Form
#else
    CgpTranslateForm
#endif
    {
        private Guid _multiDoorId;
        private IMultiDoors _multiDoors;

        public CreateDoorsForMultiDoorForm()
            : base(NCASClient.LocalizationHelper)
        {
            InitializeComponent();
        }

        public static void ShowDialog(Guid multiDoorId, IMultiDoors multiDoors)
        {
            var createDoorsForMultiDoorForm = new CreateDoorsForMultiDoorForm
            {
                _multiDoorId = multiDoorId,
                _multiDoors = multiDoors
            };

            createDoorsForMultiDoorForm.ShowDialog();
        }

        private void _bCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void _bCreate_Click(object sender, EventArgs e)
        {
            if (_multiDoors != null)
            {
                if (_eDoorsCount.Value == 0)
                {
                    ControlNotification.Singleton.Error(
                        NotificationPriority.JustOne,
                        _eDoorsCount,
                        GetString("ErrorEntryDoorsCount"),
                        CgpClient.Singleton.ClientControlNotificationSettings);

                    return;
                }

                _multiDoors.CreateDoorsForMultiDoor(
                    _multiDoorId,
                    GetString("StringDoor"),
                    (int) _eDoorsCount.Value);
            }

            Close();
        }
    }
}
