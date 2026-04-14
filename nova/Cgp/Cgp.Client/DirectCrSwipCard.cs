using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Contal.IwQuick.Localization;
using Contal.IwQuick.Threads;

namespace Contal.Cgp.Client
{
    public partial class DirectCrSwipCard :
#if DESIGNER
        Form
#else
        CgpTranslateForm
#endif
    {
        public DirectCrSwipCard(LocalizationHelper localizationHelper)
            : base(localizationHelper)
        {
            InitializeComponent();
            localizeText = localizationHelper.GetString("DirectCrSwipCard_lWaitingForSwipingCard");
        }

        private string localizeText = "Waiting for swiping card: ";
        SafeThread _doCountDownThread = null;

        public event Contal.IwQuick.DString2Void eCardSwiped;
        private string _fullCardNumber = string.Empty;
        public void ShowDialog(out string cardNumber)
        {
            RefreshLocalizeText(30);
            BindEvent();
            StartCountdown();
            ShowDialog();
            cardNumber = _fullCardNumber;
        }

        private void StartCountdown()
        {
            try
            {
                _doCountDownThread = new SafeThread(DoCountdown);
                _doCountDownThread.Start();
            }
            catch { }
        }

        private void StopCountdown()
        { 
            try
            {
                if (_doCountDownThread != null)
                {
                    _doCountDownThread.Stop(0);
                }
            }
            catch {}
        }

        private void DoCountdown()
        {
            try
            {
                for (int i = 29; i > 0; i--)
                {
                    System.Threading.Thread.Sleep(1000);
                    RefreshLocalizeText(i);
                }
                CloseForm();
            }
            catch { }
        }

        private void RefreshLocalizeText(int timeToDown)
        {
            try
            {
                if (InvokeRequired)
                {
                    BeginInvoke(new Contal.IwQuick.DInt2Void(RefreshLocalizeText), timeToDown);
                }
                else
                {
                    _lWaitingForSwipingCardNT.Text = localizeText + timeToDown.ToString();
                }
            }
            catch { }
        }

        void Singleton_LoginCrCardSwiped(string inputString)
        {
            _fullCardNumber = inputString;
            CloseForm();
        }

        private void _bCancel_Click(object sender, EventArgs e)
        {
            _fullCardNumber = string.Empty;
            Close();
        }

        private void BindEvent()
        {
            eCardSwiped = new Contal.IwQuick.DString2Void(Singleton_LoginCrCardSwiped);
            CgpClient.Singleton.LoginCrCardSwiped += eCardSwiped;
        }

        private void UnbindEvent()
        {
            if (eCardSwiped != null)
            {
                CgpClient.Singleton.LoginCrCardSwiped += eCardSwiped;
                eCardSwiped = null;
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            UnbindEvent();
            StopCountdown();
        }

        private void CloseForm()
        {
            try
            {
                this.Invoke(new EventHandler(delegate { Close(); }));
            }
            catch { }
        }
    }
}
