using Contal.Cgp.RemotingCommon;

namespace Contal.Cgp.Client.Common
{
    public interface ICgpClientBase
    {
        string CardReaderTypeInformation
        {
            get;
            set;
        }

        string CardReaderFirmwareInformation
        {
            get; 
            set;
        }

        ICgpServerRemotingProvider MainServerProvider { get; }
        bool GetRequirePINCardLogin { get; }

        string ComPortName
        {
            get;
        }

        bool IsConnectionLost(bool showDialog);

        void CrOnlineStateChanged(bool online);
        void CrCardSwiped(string fullCardNumber);

        void ClientLoginWithCard(string fullCardNumber);
        void ClientInfoWrongPIN();
        void LoginCardSwiped(string fullCardNumber);
    }
}