using System;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Contal.IwQuick.Sys.Microsoft;


namespace Contal.IwQuick.Net.Microsoft
{
    public interface ISntpClient
    {
        void Synchronize(IPAddress[] ipAddresses);
        void Synchronize(string[] hostNames); // s podporou DNS resolvu
    }

    // Leap indicator field values
    public enum SntpLeapIndicator
    {
        NoWarning,		// 0 - No warning
        LastMinute61,	// 1 - Last minute has 61 seconds
        LastMinute59,	// 2 - Last minute has 59 seconds
        Alarm			// 3 - Alarm condition (clock not synchronized)
    }

    //Mode field values
    public enum SntpMode
    {
        SymmetricActive,	// 1 - Symmetric active
        SymmetricPassive,	// 2 - Symmetric pasive
        Client,				// 3 - Client
        Server,				// 4 - Server
        Broadcast,			// 5 - Broadcast
        Unknown				// 0, 6, 7 - Reserved
    }

    // Stratum field values
    public enum SntpStratum
    {
        Unspecified,			// 0 - unspecified or unavailable
        PrimaryReference,		// 1 - primary reference (e.g. radio-clock)
        SecondaryReference,		// 2-15 - secondary reference (via NTP or SNTP)
        Reserved				// 16-255 - reserved
    }

    /// <summary>
    /// NTPClient is a C# class designed to connect to time servers on the Internet.
    /// The implementation of the protocol is based on the RFC 2030.
    /// 
    /// Public class members:
    ///
    /// LeapIndicator - Warns of an impending leap second to be inserted/deleted in the last
    /// minute of the current day. (See the _LeapIndicator enum)
    /// 
    /// VersionNumber - Version number of the protocol (3 or 4).
    /// 
    /// Mode - Returns mode. (See the _Mode enum)
    /// 
    /// Stratum - Stratum of the clock. (See the _Stratum enum)
    /// 
    /// PollInterval - Maximum interval between successive messages.
    /// 
    /// Precision - Precision of the clock.
    /// 
    /// RootDelay - Round trip time to the primary reference source.
    /// 
    /// RootDispersion - Nominal error relative to the primary reference source.
    /// 
    /// ReferenceID - Reference identifier (either a 4 character string or an IP address).
    /// 
    /// ReferenceTimestamp - The time at which the clock was last set or corrected.
    /// 
    /// OriginateTimestamp - The time at which the request departed the client for the server.
    /// 
    /// ReceiveTimestamp - The time at which the request arrived at the server.
    /// 
    /// Transmit Timestamp - The time at which the reply departed the server for client.
    /// 
    /// RoundTripDelay - The time between the departure of request and arrival of reply.
    /// 
    /// LocalClockOffset - The offset of the local clock relative to the primary reference
    /// source.
    /// 
    /// Initialize - Sets up data structure and prepares for connection.
    /// 
    /// Connect - Connects to the time server and populates the data structure.
    ///	It can also set the system time.
    /// 
    /// IsResponseValid - Returns true if received data is valid and if comes from
    /// a NTP-compliant time server.
    /// 
    /// ToString - Returns a string representation of the object.
    /// 
    /// -----------------------------------------------------------------------------
    /// Structure of the standard NTP header (as described in RFC 2030)
    ///                       1                   2                   3
    ///   0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
    ///  +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///  |LI | VN  |Mode |    Stratum    |     Poll      |   Precision   |
    ///  +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///  |                          Root Delay                           |
    ///  +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///  |                       Root Dispersion                         |
    ///  +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///  |                     Reference Identifier                      |
    ///  +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///  |                                                               |
    ///  |                   Reference Timestamp (64)                    |
    ///  |                                                               |
    ///  +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///  |                                                               |
    ///  |                   Originate Timestamp (64)                    |
    ///  |                                                               |
    ///  +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///  |                                                               |
    ///  |                    Receive Timestamp (64)                     |
    ///  |                                                               |
    ///  +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///  |                                                               |
    ///  |                    Transmit Timestamp (64)                    |
    ///  |                                                               |
    ///  +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///  |                 Key Identifier (optional) (32)                |
    ///  +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    ///  |                                                               |
    ///  |                                                               |
    ///  |                 Message Digest (optional) (128)               |
    ///  |                                                               |
    ///  |                                                               |
    ///  +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    /// 
    /// -----------------------------------------------------------------------------
    /// 
    /// NTP Timestamp Format (as described in RFC 2030)
    ///                         1                   2                   3
    ///     0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
    /// +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    /// |                           Seconds                             |
    /// +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    /// |                  Seconds Fraction (0-padded)                  |
    /// +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    /// 
    /// </summary>
    public class SNTP : ISntpClient
    {


        // NTP Data Structure Length
        private const byte NTPDataLength = 48;
        // NTP Data Structure (as described in RFC 2030)
        byte[] NTPData = new byte[NTPDataLength];

        // Offset constants for timestamps in the data structure
        private const byte offReferenceID = 12;
        private const byte offReferenceTimestamp = 16;
        private const byte offOriginateTimestamp = 24;
        private const byte offReceiveTimestamp = 32;
        private const byte offTransmitTimestamp = 40;

        // Leap Indicator
        public SntpLeapIndicator LeapIndicator
        {
            get
            {
                // Isolate the two most significant bits
                byte val = (byte)(NTPData[0] >> 6);
                switch (val)
                {
                    case 0: return SntpLeapIndicator.NoWarning;
                    case 1: return SntpLeapIndicator.LastMinute61;
                    case 2: return SntpLeapIndicator.LastMinute59;
                    case 3: goto default;
                    default:
                        return SntpLeapIndicator.Alarm;
                }
            }
        }

        // Version Number
        public byte VersionNumber
        {
            get
            {
                // Isolate bits 3 - 5
                byte val = (byte)((NTPData[0] & 0x38) >> 3);
                return val;
            }
        }

        // Mode
        public SntpMode Mode
        {
            get
            {
                // Isolate bits 0 - 3
                byte val = (byte)(NTPData[0] & 0x7);
                switch (val)
                {
                    case 0: goto default;
                    case 6: goto default;
                    case 7: goto default;
                    default:
                        return SntpMode.Unknown;
                    case 1:
                        return SntpMode.SymmetricActive;
                    case 2:
                        return SntpMode.SymmetricPassive;
                    case 3:
                        return SntpMode.Client;
                    case 4:
                        return SntpMode.Server;
                    case 5:
                        return SntpMode.Broadcast;
                }
            }
        }

        // Stratum
        public SntpStratum Stratum
        {
            get
            {
                byte val = NTPData[1];
                if (val == 0) return SntpStratum.Unspecified;
                
                if (val == 1) return SntpStratum.PrimaryReference;
                
                if (val <= 15) return SntpStratum.SecondaryReference;
                
                return SntpStratum.Reserved;
            }
        }

        // Poll Interval
        public uint PollInterval
        {
            get
            {
                return (uint)Math.Round(Math.Pow(2, NTPData[2]));
            }
        }

        // Precision (in milliseconds)
        public double Precision
        {
            get
            {
                return (1000 * Math.Pow(2, NTPData[3]));
            }
        }

        // Root Delay (in milliseconds)
        public double RootDelay
        {
            get
            {
                int temp = 256 * (256 * (256 * NTPData[4] + NTPData[5]) + NTPData[6]) + NTPData[7];
                return 1000 * (((double)temp) / 0x10000);
            }
        }

        // Root Dispersion (in milliseconds)
        public double RootDispersion
        {
            get
            {
                int temp = 256 * (256 * (256 * NTPData[8] + NTPData[9]) + NTPData[10]) + NTPData[11];
                return 1000 * (((double)temp) / 0x10000);
            }
        }

        // Reference Identifier
        public string ReferenceID
        {
            get
            {
                string val = String.Empty;
                switch (Stratum)
                {
                    case SntpStratum.Unspecified:
                        goto case SntpStratum.PrimaryReference;
                    case SntpStratum.PrimaryReference:
                        val += (char)NTPData[offReferenceID + 0];
                        val += (char)NTPData[offReferenceID + 1];
                        val += (char)NTPData[offReferenceID + 2];
                        val += (char)NTPData[offReferenceID + 3];
                        break;
                    case SntpStratum.SecondaryReference:
                        switch (VersionNumber)
                        {
                            case 3:	// Version 3, Reference ID is an IPv4 address
                                string Address = string.Format("{0}.{1}.{2}.{3}", 
                                    NTPData[offReferenceID + 0], 
                                    NTPData[offReferenceID + 1], 
                                    NTPData[offReferenceID + 2], 
                                    NTPData[offReferenceID + 3]);
                                try
                                {
                                    //IPHostEntry Host = Dns.GetHostByAddress(Address);
                                    IPHostEntry Host = Dns.GetHostEntry(Address);
                                    val = Host.HostName + " (" + Address + ")";
                                }
                                catch (Exception)
                                {
                                    val = "N/A";
                                }
                                break;
                            case 4: // Version 4, Reference ID is the timestamp of last update
                                DateTime time = ComputeDate(GetMilliSeconds(offReferenceID));
                                // Take care of the time zone
                                TimeSpan offspan = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now);
                                val = (time + offspan).ToString(CultureInfo.InvariantCulture);
                                break;
                            default:
                                val = "N/A";
                                break;
                        }
                        break;
                }

                return val;
            }
        }

        // Reference Timestamp
        public DateTime ReferenceTimestamp
        {
            get
            {
                DateTime time = ComputeDate(GetMilliSeconds(offReferenceTimestamp));
                // Take care of the time zone
                TimeSpan offspan = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now);
                return time + offspan;
            }
        }

        // Originate Timestamp
        public DateTime OriginateTimestamp
        {
            get
            {
                return ComputeDate(GetMilliSeconds(offOriginateTimestamp));
            }
        }

        // Receive Timestamp
        public DateTime ReceiveTimestamp
        {
            get
            {
                DateTime time = ComputeDate(GetMilliSeconds(offReceiveTimestamp));
                // Take care of the time zone
                TimeSpan offspan = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now);
                return time + offspan;
            }
        }

        // Transmit Timestamp
        public DateTime TransmitTimestamp
        {
            get
            {
                DateTime time = ComputeDate(GetMilliSeconds(offTransmitTimestamp));
                // Take care of the time zone
                TimeSpan offspan = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now);
                return time + offspan;
            }
            set
            {
                SetDate(offTransmitTimestamp, value);
            }
        }

        // Reception Timestamp
        public DateTime ReceptionTimestamp;

        // Round trip delay (in milliseconds)
        public int RoundTripDelay
        {
            get
            {
                TimeSpan span = (ReceiveTimestamp - OriginateTimestamp) + (ReceptionTimestamp - TransmitTimestamp);
                return (int)span.TotalMilliseconds;
            }
        }

        // Local clock offset (in milliseconds)
        public int LocalClockOffset
        {
            get
            {
                TimeSpan span = (ReceiveTimestamp - OriginateTimestamp) - (ReceptionTimestamp - TransmitTimestamp);
                return (int)(span.TotalMilliseconds / 2);
            }
        }

        // Compute date, given the number of milliseconds since January 1, 1900
        private DateTime ComputeDate(ulong milliseconds)
        {
            TimeSpan span = TimeSpan.FromMilliseconds(milliseconds);
            DateTime time = new DateTime(1900, 1, 1);
            time += span;
            return time;
        }

        // Compute the number of milliseconds, given the offset of a 8-byte array
        private ulong GetMilliSeconds(byte offset)
        {
            ulong intpart = 0, fractpart = 0;

            for (int i = 0; i <= 3; i++)
            {
                intpart = 256 * intpart + NTPData[offset + i];
            }
            for (int i = 4; i <= 7; i++)
            {
                fractpart = 256 * fractpart + NTPData[offset + i];
            }
            ulong milliseconds = intpart * 1000 + (fractpart * 1000) / 0x100000000L;
            return milliseconds;
        }

        // Compute the 8-byte array, given the date
        private void SetDate(byte offset, DateTime date)
        {
            ulong intpart, fractpart;
            DateTime StartOfCentury = new DateTime(1900, 1, 1, 0, 0, 0);	// January 1, 1900 12:00 AM

            ulong milliseconds = (ulong)(date - StartOfCentury).TotalMilliseconds;
            intpart = milliseconds / 1000;
            fractpart = ((milliseconds % 1000) * 0x100000000L) / 1000;

            ulong temp = intpart;
            for (int i = 3; i >= 0; i--)
            {
                NTPData[offset + i] = (byte)(temp % 256);
                temp = temp / 256;
            }

            temp = fractpart;
            for (int i = 7; i >= 4; i--)
            {
                NTPData[offset + i] = (byte)(temp % 256);
                temp = temp / 256;
            }
        }

        // Initialize the NTPClient data
        private void Initialize()
        {
            // Set version number to 4 and Mode to 3 (client)
            NTPData[0] = 0x1B;
            // Initialize all other fields with 0
            for (int i = 1; i < 48; i++)
            {
                NTPData[i] = 0;
            }
            // Initialize the transmit timestamp
            TransmitTimestamp = DateTime.Now;
        }

        public SNTP(string host)
        {
            TimeServer = host;
        }

        public SNTP()
        {

        }

        // Connect to the time server and update system time
        public string Connect(bool UpdateSystemTime)
        {
            try
            {
                // Resolve server address

                IPAddress[] ips = Dns.GetHostAddresses(TimeServer);
                if (null == ips || ips.Length == 0)
                    return String.Empty;
                
                IPEndPoint ipEndpoint = new IPEndPoint(ips[0], 123);

                //Connect the time server
                UdpClient TimeSocket = new UdpClient();
                TimeSocket.Connect(ipEndpoint);

                // Initialize data structure
                Initialize();
                TimeSocket.Send(NTPData, NTPData.Length);

                Thread.Sleep(250);
                int aAvaliableData = TimeSocket.Available;
                if (aAvaliableData > 0)
                {
                    NTPData = TimeSocket.Receive(ref ipEndpoint);
                }
                if (!IsResponseValid())
                {
                    //throw new Exception("Invalid response from " + TimeServer);
                    return "Invalid response from " + TimeServer;
                }
                ReceptionTimestamp = DateTime.Now;
            }
            catch (SocketException e)
            {
                return e.Message;
            }

            // Update system time
            if (UpdateSystemTime)
            {
                if (SetTime())
                {
                    return "Time synchronized successfully";
                }
                
                return "Time failed to synchronize";
            }
            return "Unexpected error";
        }

        // Check if the response from server is valid
        public bool IsResponseValid()
        {
            if (NTPData.Length < NTPDataLength || Mode != SntpMode.Server)
            {
                return false;
            }
            
            return true;
        }

        // Converts the object to string
        public override string ToString()
        {
            string str;

            str = "Leap Indicator: ";
            switch (LeapIndicator)
            {
                case SntpLeapIndicator.NoWarning:
                    str += "No warning";
                    break;
                case SntpLeapIndicator.LastMinute61:
                    str += "Last minute has 61 seconds";
                    break;
                case SntpLeapIndicator.LastMinute59:
                    str += "Last minute has 59 seconds";
                    break;
                case SntpLeapIndicator.Alarm:
                    str += "Alarm Condition (clock not synchronized)";
                    break;
            }
            str += "\r\nVersion number: " + VersionNumber + "\r\n";
            str += "Mode: ";
            switch (Mode)
            {
                case SntpMode.Unknown:
                    str += "Unknown";
                    break;
                case SntpMode.SymmetricActive:
                    str += "Symmetric Active";
                    break;
                case SntpMode.SymmetricPassive:
                    str += "Symmetric Pasive";
                    break;
                case SntpMode.Client:
                    str += "Client";
                    break;
                case SntpMode.Server:
                    str += "Server";
                    break;
                case SntpMode.Broadcast:
                    str += "Broadcast";
                    break;
            }
            str += "\r\nStratum: ";
            switch (Stratum)
            {
                case SntpStratum.Unspecified:
                case SntpStratum.Reserved:
                    str += "Unspecified";
                    break;
                case SntpStratum.PrimaryReference:
                    str += "Primary Reference";
                    break;
                case SntpStratum.SecondaryReference:
                    str += "Secondary Reference";
                    break;
            }
            str += "\r\nLocal time: " + TransmitTimestamp;
            str += "\r\nPrecision: " + Precision + " ms";
            str += "\r\nPoll Interval: " + PollInterval + " s";
            str += "\r\nReference ID: " + ReferenceID;
            str += "\r\nRoot Dispersion: " + RootDispersion + " ms";
            str += "\r\nRound Trip Delay: " + RoundTripDelay + " ms";
            str += "\r\nLocal Clock Offset: " + LocalClockOffset + " ms";
            str += "\r\n";

            return str;
        }

        
        


        // Set system time according to transmit timestamp
        private bool SetTime()
        {
            

            DateTime trts = TransmitTimestamp;
            DllKernel32.SYSTEMTIME st = new DllKernel32.SYSTEMTIME
            {
                year = (short) trts.Year,
                month = (short) trts.Month,
                dayOfWeek = (short) trts.DayOfWeek,
                day = (short) trts.Day,
                hour = (short) trts.Hour,
                minute = (short) trts.Minute,
                second = (short) trts.Second,
                milliseconds = (short) trts.Millisecond
            };

            //Microsoft.VisualBasic.DateAndTime.TimeOfDay = DateTime.Now.AddSeconds(3000).AddMilliseconds(80);

            // TODO : verify what happends with st the first time
            // see details in http://msdn.microsoft.com/en-us/library/windows/desktop/ms724936%28v=vs.85%29.aspx
            if (DllKernel32.SetLocalTime(ref st))
                // Second time call because of the shit documented in MSDN :
                // "Note that the system uses the daylight saving time setting of the current time,
                // not the new time you are setting. Therefore, to ensure the correct result, 
                // call SetLocalTime a second time, now that the first call has updated the daylight saving time setting."
                return DllKernel32.SetLocalTime(ref st);
            
            return false;


            //   SetSystemTime(ref st);
        }

        // The URL of the time server we're connecting to
        private string TimeServer;

        public enum SntpSynchronizationError:byte
        {
            InvalidResponse,
            SocketExeption,
            NoAvailableData,
            TimeNotSet
        }

        #region ISntpClient Members

        public void Synchronize(IPAddress[] ipAddresses)
        {
            bool BadResponse = false;
            for (int i = 0; i < ipAddresses.Length; i++)
            {
                try
                {
                    IPEndPoint EPhost = new IPEndPoint(ipAddresses[i], 123);
                    //Connect the time server
                    UdpClient TimeSocket = new UdpClient();
                    TimeSocket.Connect(EPhost);
                    // Initialize data structure
                    Initialize();
                    TimeSocket.Send(NTPData, NTPData.Length);

                    Thread.Sleep(250);
                    int aAvaliableData = TimeSocket.Available;
                    if (aAvaliableData > 0)
                    {
                        NTPData = TimeSocket.Receive(ref EPhost);
                    }
                    else
                    {
                        if ((ipAddresses.Length - 1) == i)
                        {
                            throw new SntpSynchronizationException(SntpSynchronizationError.NoAvailableData, "No available data send from ip address: " + ipAddresses[i]);
                        }
                    }
                    ReceptionTimestamp = DateTime.Now;
                }
                catch (SocketException e)
                {
                    BadResponse = true;
                    if ((ipAddresses.Length - 1) == i)
                        throw new SntpSynchronizationException(SntpSynchronizationError.SocketExeption, "Socket exception error", e);
                }
                if (!IsResponseValid())
                {
                    BadResponse = true;
                    if ((ipAddresses.Length - 1) == i)
                    {
                        throw new SntpSynchronizationException(SntpSynchronizationError.InvalidResponse, "Invalid response from host: " + ipAddresses[i]);
                    }
                }
                // Update system time
                if (!BadResponse)
                {
                    if (SetTime())
                    {
                        return;
                    }
                    
                    throw new SntpSynchronizationException(SntpSynchronizationError.TimeNotSet, "Time failed to synchronize");
                }
                BadResponse = false;
            }
        }

        public void Synchronize(string[] hostNames)
        {
            bool badResponse = false;
            for (int i = 0; i < hostNames.Length; i++)
            {
                try
                {
                    IPHostEntry hostEntry = Dns.GetHostEntry(hostNames[i]);
                    if (null == hostEntry.AddressList || hostEntry.AddressList.Length == 0)
                        continue;

                    var ipEndpoint = new IPEndPoint(hostEntry.AddressList[0], 123);
                    //Connect the time server
                    var timeSocket = new UdpClient(ipEndpoint.AddressFamily);
                    timeSocket.Connect(ipEndpoint);
                    // Initialize data structure
                    Initialize();
                    timeSocket.Send(NTPData, NTPData.Length);

                    Thread.Sleep(250);
                    int aAvaliableData = timeSocket.Available;
                    if (aAvaliableData > 0)
                    {
                        NTPData = timeSocket.Receive(ref ipEndpoint);
                    }
                    else
                    {
                        if ((hostNames.Length - 1) == i)
                        {
                            throw new SntpSynchronizationException(SntpSynchronizationError.NoAvailableData, "No available data send from host: " + hostNames[i]);
                        }
                    }
                    ReceptionTimestamp = DateTime.Now;
                }
                catch (SocketException e)
                {
                    badResponse = true;
                    if ((hostNames.Length - 1) == i)
                        throw new SntpSynchronizationException(SntpSynchronizationError.SocketExeption, "Socket exception error", e);
                }
                if (!IsResponseValid())
                {
                    badResponse = true;
                    if ((hostNames.Length - 1) == i)
                    {
                        throw new SntpSynchronizationException(SntpSynchronizationError.InvalidResponse, "Invalid response from host: " + hostNames[i]);
                    }
                }
                // Update system time
                if (!badResponse)
                {
                    if (SetTime())
                    {
                        return;
                    }
                    
                    throw new SntpSynchronizationException(SntpSynchronizationError.TimeNotSet, "Time failed to synchronize");
                }
                badResponse = false;
            }
        }

        #endregion

        [Serializable]
        public class SntpSynchronizationException : Exception, ISerializable
        {
            private SntpSynchronizationError _sntpSynchronizationError;

            public SntpSynchronizationError SntpSynchronizationError
            {
                get { return _sntpSynchronizationError; }
                set { _sntpSynchronizationError = value; }
            }
            public SntpSynchronizationException(SntpSynchronizationError SyncError, Exception exception)
                :base(String.Empty,null)
            {
                _sntpSynchronizationError = SyncError;
            }
            public SntpSynchronizationException(SntpSynchronizationError SyncError, string message)
                : base(message)
            {
                _sntpSynchronizationError = SyncError;
            }
            public SntpSynchronizationException(SntpSynchronizationError SyncError, string message, Exception innerException)
                : base(message, innerException)
            {
                _sntpSynchronizationError = SyncError;
            }

            private const string SNTP_ERROR_SERIAL = "sntpError";

            #region ISerializable Members

            [SecurityPermissionAttribute(SecurityAction.Demand, Flags = SecurityPermissionFlag.SerializationFormatter)]
            public SntpSynchronizationException(SerializationInfo serialInfo, StreamingContext context) :
                base(serialInfo, context)
            {
                _sntpSynchronizationError = (SntpSynchronizationError)serialInfo.GetByte(SNTP_ERROR_SERIAL);

            }

            [SecurityPermissionAttribute(SecurityAction.Demand, Flags = SecurityPermissionFlag.SerializationFormatter)]
            void ISerializable.GetObjectData(SerializationInfo serialInfo, StreamingContext context)
            {
                base.GetObjectData(serialInfo, context);

                serialInfo.AddValue(SNTP_ERROR_SERIAL, _sntpSynchronizationError);

            }

            #endregion
        }
    }
}
