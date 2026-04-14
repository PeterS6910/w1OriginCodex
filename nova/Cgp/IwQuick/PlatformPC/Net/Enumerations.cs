using System;
using System.Collections.Generic;
using System.Text;

namespace Contal.IwQuick.Net
{
    public enum SecureTunnelMode
    {
        Plain2EncryptedHosting = 0,
        Encrypted2PlainHosting = 1
    }

    public enum CNMPLookupType
    {
        Unknown = -1,
        Instance = 0,
        Type = 1,
        ComputerName = 3,
        Key = 4,
    }

    public enum CNMPLookupMode
    {
        Multicast = 0,
        Broadcast = 1,
        Unicast = 2,
        UnicastInterval = 3,
        UnicastSubnet = 4
    }

    public enum SerialPin:int
    {
        Dtr = 1,
        Rts = 2,
        // Summary:
        //     The Clear to Send (CTS) signal changed state. This signal is used to indicate
        //     whether data can be sent over the serial port.
        Cts = 8,
        //
        // Summary:
        //     The Data Set Ready (DSR) signal changed state. This signal is used to indicate
        //     whether the device on the serial port is ready to operate.
        Dsr = 16,
        //
        // Summary:
        //     The Carrier Detect (CD) signal changed state. This signal is used to indicate
        //     whether a modem is connected to a working phone line and a data carrier signal
        //     is detected.
        CarrierDetect = 32,
        //
        // Summary:
        //     A break was detected on input.
        Break = 64,
        //
        // Summary:
        //     A ring indicator was detected.
        Ring = 256


    }
    
}
