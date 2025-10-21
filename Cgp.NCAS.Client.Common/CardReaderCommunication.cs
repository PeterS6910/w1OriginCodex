using System.IO.Ports;
using System.Linq;
using System.Threading;

using Contal.Cgp.Client.Common;
using Contal.Drivers.CardReader;
using Contal.Cgp.Globals;
using Contal.IwQuick.Crypto;
using Contal.IwQuick.Threads;

namespace Contal.Cgp.NCAS.Client.Common
{
    public class CardReaderCommunication : ICardReaderEventHandler
    {
        private readonly ICgpClientBase _cgpClientBase;
        CardReader _crClient = null;
        CRLineConfiguration _crLineConfiguration = null;
        CRDirectCommunicator _crCommunicator = null;
        string _lastCard = string.Empty;
        const int DELAY_TIME = 5000;

        public CardReaderCommunication(ICgpClientBase cgpClientBase)
        {
            _cgpClientBase = cgpClientBase;
        }

        public void RunCardReaderCommunication()
        {
            string comPortName =_cgpClientBase.ComPortName;

            if (SerialPort.GetPortNames().Any(portName => portName == comPortName))
            {
                StartCardReaderCommunication();
            }
            else
            {
                StopCardReaderCommunication();
            }
        }

        private const int DirectCrAddress = 1;

        private void StartCardReaderCommunication()
        {
            try
            {
                if (_crCommunicator == null)
                {
                    _crCommunicator = new CRDirectCommunicator(this);
                    
                    // replaced by ICardReaderEventHandler overrides
                    //_crCommunicator.OnlineStateChanged += CrCOnlineStateChanged;
                    //_crCommunicator.CardSwiped += CrCCardSwiped;
                    //_crCommunicator.CodeSpecified += _crCommunicator_CodeSpecified;
                    //_crCommunicator.CodeTimedOut += _crCommunicator_CodeTimedOut;
                }
                else
                {
                    _crCommunicator.Stop();
                }
                if (_crClient == null)
                {
                    _crClient = new CardReader(DirectCrAddress) {TimeHorizontalPosition = 100, TimeVerticalPosition = 100};
                }
                _crLineConfiguration = new CRLineConfiguration(_cgpClientBase.ComPortName);
                _crCommunicator.Configure(_crLineConfiguration, _crClient);
                _crCommunicator.Start();
            }
            catch
            { }
        }

        private void StopCardReaderCommunication()
        {
            try
            {
                if (_crCommunicator != null)
                    _crCommunicator.Stop();
            }
            catch
            { }
        }

        private void SetCrInformation(CardReader cr)
        {
            try
            {
                _cgpClientBase.CardReaderTypeInformation = cr.HardwareVersion.ToString();
                _cgpClientBase.CardReaderFirmwareInformation = cr.FirmwareVersion;
            }
            catch { }
        }

        private void WaitForPin()
        {
            byte pinlength = 
                _cgpClientBase.MainServerProvider.GetCardPinLengthByFullCardNumber(_lastCard);

            if (pinlength == 0)
            {
                _crCommunicator.AccessCommands.Rejected(_crClient);
                ShowWaitForCardWithDelay();
            }
            else
            {
                _crCommunicator.AccessCommands.WaitingForPIN(_crClient, pinlength);
            }
        }

        //***
        //messages from Client to Card reader
        //***
        public void ShowCommandCardReader(CardReaderSceneType command)
        {
            if (_crCommunicator == null || _crClient == null) return;

            switch (command)
            {
                case CardReaderSceneType.Accepted:
                    _crCommunicator.AccessCommands.Accepted(_crClient);
                    SafeThread.StartThread(ShowWaitForCardWithDelay);
                    break;
                case CardReaderSceneType.Rejected:
                    _crCommunicator.AccessCommands.Rejected(_crClient);
                    SafeThread.StartThread(ShowWaitForCardWithDelay);
                    break;
                case CardReaderSceneType.WaitingForCard:
                    _crCommunicator.AccessCommands.WaitingForCard(_crClient);
                    break;
            }
        }

        private void ShowWaitForCardWithDelay()
        {
            Thread.Sleep(DELAY_TIME);
            _crCommunicator.AccessCommands.WaitingForCard(_crClient);
        }

        //***
        //Send messages to client 
        //***
        private void SendCrCOnlineStateChanged(bool online)
        {
            _cgpClientBase.CrOnlineStateChanged(online);
        }

        private void ClientLoginWithCard(string fullCardNumber)
        {
            _cgpClientBase.ClientLoginWithCard(fullCardNumber);
        }

        private void ClientInfoWrongPIN()
        {
            _cgpClientBase.ClientInfoWrongPIN();
        }

        #region ICardReaderEventHandler Members

        void ICardReaderEventHandler.CardReaderIndirectMessageToSend(CardReader cr, CRMessage message, bool highPriority)
        {
            
        }

        void ICardReaderEventHandler.CardReaderFnBoxTimedOut(CardReader cr)
        {
            
        }

        void ICardReaderEventHandler.CardReaderCodeTimedOut(CardReader cr)
        {
            _crCommunicator.AccessCommands.WaitingForCard(_crClient);
        }

        void ICardReaderEventHandler.CardReaderCodeSpecified(CardReader cr, string pin)
        {
            if (cr == _crClient)
            {
                string hashedPin = QuickHashes.GetCRC32String(pin);
                if (_cgpClientBase.MainServerProvider.IsValidPinForCard(_lastCard, hashedPin))
                {
                    SafeThread<string>.StartThread(ClientLoginWithCard, _lastCard);
                    _crCommunicator.AccessCommands.WaitingForCard(_crClient);
                }
                else
                {
                    ShowCommandCardReader(CardReaderSceneType.Rejected);
                    SafeThread.StartThread(ClientInfoWrongPIN);
                }
            }
        }

        void ICardReaderEventHandler.CardReaderMenuCancelled(CardReader cr, bool byOtherCommand)
        {
            
        }

        void ICardReaderEventHandler.CardReaderMenuTimedOut(CardReader cr)
        {
            
        }

        void ICardReaderEventHandler.CardReaderMenuItemSelected(CardReader cr, int itemIndex, string itemText)
        {
            
        }

        void ICardReaderEventHandler.CardReaderMenuItemSelected(CardReader cr, int itemReturnCode)
        {
            // TODO
        }

        void ICardReaderEventHandler.CardReaderNumericKeyPressed(CardReader cr, byte numeric)
        {
            
        }

        void ICardReaderEventHandler.CardReaderSabotageStateChanged(CardReader cr, bool tamperOn)
        {
            
        }

        void ICardReaderEventHandler.CardReaderSpecialKeyPressed(CardReader cr, CRSpecialKey specialKey)
        {
            
        }

        void ICardReaderEventHandler.CardReaderOnlineStateChanged(CardReader cr, bool isOnline)
        {
            if (cr == _crClient)
            {
                try
                {
                    SafeThread<bool>.StartThread(SendCrCOnlineStateChanged, isOnline);
                    SafeThread<CardReader>.StartThread(SetCrInformation, cr);
                }
                catch
                { }
                if (isOnline)
                {
                    _crCommunicator.AccessCommands.WaitingForCard(_crClient);
                }
            }
        }

        void ICardReaderEventHandler.CardReaderCardSwiped(CardReader cr, string fullCardNumber, int cardSystemNumber)
        {
            if (cr == _crClient)
            {
                //if is PIN requiered 
                if (_cgpClientBase.GetRequirePINCardLogin)
                {
                    if (_crClient.HasKeyboard)
                    {
                        _lastCard = fullCardNumber;
                        SafeThread.StartThread(WaitForPin);
                    }

                    _cgpClientBase.LoginCardSwiped(fullCardNumber);
                    //Contal.IwQuick.Threads.SafeThread<string>.StartThread(SendCrCCardSwiped, fullCardNumber);
                }
                else
                {
                    ClientLoginWithCard(fullCardNumber);
                    //Contal.IwQuick.Threads.SafeThread<string>.StartThread(ClientLoginWithCard, fullCardNumber);
                }

                _cgpClientBase.CrCardSwiped(fullCardNumber);
            }
        }

        void ICardReaderEventHandler.CardReaderQueryDbStampResponse(CardReader cr, byte[] queryDbStamp)
        {
            
        }

        void ICardReaderEventHandler.CardReaderConfigurationResponse(CardReader cr, CRConfigurationResult result, byte dbStamp)
        {
            
        }

        void ICardReaderEventHandler.CardReaderConfigurationResponse(CardReader cr, CRConfigurationResult result, uint crUniqueId, ushort configId)
        {
			// TODO
        }

        void ICardReaderEventHandler.CardReaderResetOccured(CardReader cr)
        {
            
        }

        void ICardReaderEventHandler.CardReaderCommandFailed(CardReader cr, CRMessageCode messageCode)
        {
            
        }

        void ICardReaderEventHandler.CardReaderCommandTimedOut(CardReader cr, CRMessageCode messageCode, int retryToCome)
        {
            
        }

        void ICardReaderEventHandler.CardReaderServiceResultOccured(CardReader cr, CRServiceResult serviceResult)
        {
            
        }

        void ICardReaderEventHandler.CardReaderUpgradeResult(CardReader cr, CRUpgradeResult result, System.Exception error)
        {
            
        }

        void ICardReaderEventHandler.CardReaderUpgradeProgress(CardReader cr, int progress)
        {
            
        }

        void ICardReaderEventHandler.CardReaderCountryCodeConfirmed(CardReader cr)
        {
            
        }

        void ICardReaderEventHandler.CardReaderGraphicalMenuUpdated(CardReader cr)
        {
            
        }

        void ICardReaderEventHandler.CardReaderModeChanged(CardReader cr, CRMode readerMode)
        {
        }

        void ICardReaderEventHandler.CardReaderCardWriterResponse(CardReader cr, MifareCardWriteResult writeResult, MifareCardWriteError? writeError)
        {
        }

        #endregion
    }
}
