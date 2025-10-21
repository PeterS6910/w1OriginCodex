using Contal.Cgp.NCAS.CCU.CardReaderMechanism;
using Contal.Drivers.CardReader;


namespace Contal.Cgp.NCAS.CCU
{
    internal class DCUCardReaderSettings : ACardReaderSettings
    {
        public DCUCardReaderSettings(DB.CardReader cardReaderDb)
            : base(cardReaderDb)
        {
        }

        protected override CRCommunicator CrCommunicator
        {
            get { return DCUs.Singleton.GetCrCommunicator(CardReaderDb.GuidDCU); }
        }

        public override int DcuLogicalAddress
        {
            get { return DCUs.Singleton.GetDcuLogicalAddress(_cardReaderDB.GuidDCU); }
        }

        public override void DisplayText(byte left, byte top, string text)
        {
            if (CardReader != null)
                CardReader.DisplayCommands.DisplayText(
                    CardReader,
                    left,
                    top,
                    text);
        }

        protected override string SerialPortName
        {
            get { return string.Empty; }
        }
    }
}