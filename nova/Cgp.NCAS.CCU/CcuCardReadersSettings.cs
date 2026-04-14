using System;

using Contal.Cgp.NCAS.CCU.CardReaderMechanism;
using Contal.Cgp.NCAS.DoorStateMachine;
using Contal.Drivers.CardReader;
using Contal.IwQuick.Sys;

namespace Contal.Cgp.NCAS.CCU
{
    internal class CcuCardReadersSettings : CardReaderMechanism.ACardReaderSettings
    {
        public CcuCardReadersSettings(DB.CardReader cardReaderDb)
            : base(cardReaderDb)
        {
        }

        protected override CRCommunicator CrCommunicator
        {
            get { return CcuCardReaders.CrCommunicator; }
        }

        public override int DcuLogicalAddress
        {
            get { return -1; }
        }

        public override void DisplayText(byte left, byte top, string text)
        {
            if (CardReader == null)
                return;

            CardReader.DisplayCommands.DisplayText(CardReader, left, top, text);
        }

        protected override string SerialPortName
        {
            get { return CcuCardReaders.SerialPortName; }
        }
    }
}