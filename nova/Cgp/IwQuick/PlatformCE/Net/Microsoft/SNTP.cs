using System;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using Contal.IwQuick;
using Contal.IwQuick.Sys.Microsoft;

// ReSharper disable once CheckNamespace
namespace Contal.IwQuick.Net.Microsoft
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISntpClient
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeChanged"></param>
        /// <param name="ipAddresses"></param>
        /// <returns></returns>
        bool Synchronize(out bool timeChanged, IPAddress[] ipAddresses);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeChanged"></param>
        /// <param name="hostNames"></param>
        /// <returns></returns>
        bool Synchronize(out bool timeChanged, string[] hostNames); // s podporou DNS resolvu
    }

    // Leap indicator field values
    /// <summary>
    /// 
    /// </summary>
    public enum SntpLeapIndicator
    {
        /// <summary>
        /// 0 - No warning
        /// </summary>
        NoWarning = 0,		// 
        /// <summary>
        /// 1 - Last minute has 61 seconds
        /// </summary>
        LastMinute61 = 1,	// 
        /// <summary>
        /// 2 - Last minute has 59 seconds
        /// </summary>
        LastMinute59 = 2,	// 
        /// <summary>
        /// 3 - Alarm condition (clock not synchronized)
        /// </summary>
        Alarm = 3			// 
    }

    /// <summary>
    /// Mode field values
    /// </summary>
    public enum SntpMode
    {
        /// <summary>
        /// 1 - Symmetric active
        /// </summary>
        SymmetricActive = 1,	// 
        /// <summary>
        /// 2 - Symmetric pasive
        /// </summary>
        SymmetricPassive = 2,	
        /// <summary>
        /// 3 - Client
        /// </summary>
        Client = 3,
        /// <summary>
        /// 4 - Server 
        /// </summary>
        Server = 4,		
		/// <summary>
        /// 5 - Broadcast
		/// </summary>
        Broadcast = 5,			
        /// <summary>
        /// reserved
        /// </summary>
        Unknown0 = 0,				
        /// <summary>
        /// reserved
        /// </summary>
        Unknown6 = 6,
        /// <summary>
        /// reserved
        /// </summary>
        Unknown7 = 7
    }

    // Stratum field values
    /// <summary>
    /// 
    /// </summary>
    public enum SntpStratum
    {
        /// <summary>
        /// 0 - unspecified or unavailable
        /// </summary>
        Unspecified,			
        /// <summary>
        /// 1 - primary reference (e.g. radio-clock)
        /// </summary>
        PrimaryReference,
        /// <summary>
        /// 2-15 - secondary reference (via NTP or SNTP)
        /// </summary>
        SecondaryReference,		
        /// <summary>
        /// 16-255 - reserved
        /// </summary>
        Reserved 
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
        private static volatile SNTP _implicitInstance;
        private static readonly object _syncRoot = new object();

        /// <summary>
        /// 
        /// </summary>
        public static SNTP Implicit
        {
            get
            {
                if (_implicitInstance == null)
                    lock (_syncRoot)
                    {
                        if (_implicitInstance == null)
                            _implicitInstance = new SNTP();
                    }

                return _implicitInstance;
            }
        }

        // NTP Data Structure Length
        private const byte NTP_DATA_LENGTH = 48;

        // Max time in microseconds which timeUdpSocket will be waiting to get response  
        private const int TIME_SOCKET_POLL_INTERVAL = 1000000;

        // Offset constants for timestamps in the data structure
        private const byte OFF_REFERENCE_ID = 12;
        private const byte OFF_REFERENCE_TIMESTAMP = 16;
        private const byte OFF_ORIGINATE_TIMESTAMP = 24;
        private const byte OFF_RECIEVE_TIMESTAMP = 32;
        private const byte OFF_TRANSMIT_TIMESTAMP = 40;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ntpData"></param>
        /// <returns></returns>
        public SntpLeapIndicator GetLeapIndicator(byte[] ntpData)
        {
            // Isolate the two most significant bits
            byte val = (byte)(ntpData[0] >> 6);
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

        
        /// <summary>
        /// Version Number
        /// </summary>
        /// <param name="ntpData"></param>
        /// <returns></returns>
        public byte GetVersionNumber(byte[] ntpData)
        {
            // Isolate bits 3 - 5
            byte val = (byte)((ntpData[0] & 0x38) >> 3);
            return val;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ntpData"></param>
        /// <returns></returns>
        public SntpMode GetMode(byte[] ntpData)
        {
            // Isolate bits 0 - 3
            byte val = (byte)(ntpData[0] & 0x7);
            return (SntpMode)val;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ntpData"></param>
        /// <returns></returns>
        public SntpStratum GetStratum(byte[] ntpData)
        {
            byte val = ntpData[1];
            if (val == 0)
            {
                return SntpStratum.Unspecified;
            }
            
            if (val == 1)
            {
                return SntpStratum.PrimaryReference;
            }
            
            if (val <= 15)
            {
                return SntpStratum.SecondaryReference;
            }
            
            return SntpStratum.Reserved;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ntpData"></param>
        /// <returns></returns>
        public uint GetPollInterval(byte[] ntpData)
        {
            return (uint)Math.Round(Math.Pow(2, ntpData[2]));
        }

        // Precision (in milliseconds)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ntpData"></param>
        /// <returns></returns>
        public double GetPrecision(byte[] ntpData)
        {
            return (1000 * Math.Pow(2, ntpData[3]));
        }

        // Root Delay (in milliseconds)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ntpData"></param>
        /// <returns></returns>
        public double GetRootDelay(byte[] ntpData)
        {
            int temp;
            temp = 256 * (256 * (256 * ntpData[4] + ntpData[5]) + ntpData[6]) + ntpData[7];
            return 1000 * (((double)temp) / 0x10000);
        }

        /// <summary>
        /// Root Dispersion (in milliseconds) 
        /// </summary>
        /// <param name="ntpData"></param>
        /// <returns></returns>
        public double GetRootDispersion(byte[] ntpData)
        {
            int temp = 256 * (256 * (256 * ntpData[8] + ntpData[9]) + ntpData[10]) + ntpData[11];
            return 1000 * (((double)temp) / 0x10000);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ntpData"></param>
        /// <returns></returns>
        public string GetReferenceID(byte[] ntpData)
        {

            string val = "";
            switch (GetStratum(ntpData))
            {
                case SntpStratum.Unspecified:
                    goto case SntpStratum.PrimaryReference;
                case SntpStratum.PrimaryReference:
                    val += (char)ntpData[OFF_REFERENCE_ID + 0];
                    val += (char)ntpData[OFF_REFERENCE_ID + 1];
                    val += (char)ntpData[OFF_REFERENCE_ID + 2];
                    val += (char)ntpData[OFF_REFERENCE_ID + 3];
                    break;
                case SntpStratum.SecondaryReference:
                    switch (GetVersionNumber(ntpData))
                    {
                        case 3:	// Version 3, Reference ID is an IPv4 address
                            string Address = ntpData[OFF_REFERENCE_ID + 0] + "." +
                                             ntpData[OFF_REFERENCE_ID + 1] + "." +
                                             ntpData[OFF_REFERENCE_ID + 2] + "." +
                                             ntpData[OFF_REFERENCE_ID + 3];
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
                            DateTime time = ComputeDate(GetMilliSeconds(ntpData, OFF_REFERENCE_ID));
                            // Take care of the time zone                              
                            val = TimeZone.CurrentTimeZone.ToLocalTime(time).ToString(CultureInfo.InvariantCulture);
                            break;
                        default:
                            val = "N/A";
                            break;
                    }
                    break;
            }

            return val;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ntpData"></param>
        /// <returns></returns>
        public DateTime GetReferenceTimestamp(byte[] ntpData)
        {
            DateTime time = ComputeDate(GetMilliSeconds(ntpData, OFF_REFERENCE_TIMESTAMP));
            return TimeZone.CurrentTimeZone.ToLocalTime(time);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ntpData"></param>
        /// <returns></returns>
        public DateTime GetOriginateTimestamp(byte[] ntpData)
        {
            return ComputeDate(GetMilliSeconds(ntpData, OFF_ORIGINATE_TIMESTAMP));
        }

        //old implementation
        //TimeSpan offspan = TimeZone.CurrentTimeZone.GetUtcOffset(time);
        //return time + offspan;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ntpData"></param>
        /// <returns></returns>
        public DateTime GetReceiveTimestamp(byte[] ntpData)
        {
            DateTime time = ComputeDate(GetMilliSeconds(ntpData, OFF_RECIEVE_TIMESTAMP));
            return TimeZone.CurrentTimeZone.ToLocalTime(time);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ntpData"></param>
        /// <returns></returns>
        public DateTime GetTransmitTimestamp(byte[] ntpData)
        {
            DateTime time = ComputeDate(GetMilliSeconds(ntpData, OFF_TRANSMIT_TIMESTAMP));
            return TimeZone.CurrentTimeZone.ToLocalTime(time);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ntpData"></param>
        /// <returns></returns>
        public DateTime GetTransmitUtcTimestamp(byte[] ntpData)
        {
            return ComputeDate(GetMilliSeconds(ntpData, OFF_TRANSMIT_TIMESTAMP));
        }

        // Reception Timestamp
        /// <summary>
        /// 
        /// </summary>
        public DateTime _receptionTimestamp;

        /// <summary>
        /// Round trip delay (in milliseconds)
        /// </summary>
        /// <param name="ntpData"></param>
        /// <returns></returns>
        public int GetRoundTripDelay(byte[] ntpData)
        {
            TimeSpan span = (GetReceiveTimestamp(ntpData) - GetOriginateTimestamp(ntpData)) + (_receptionTimestamp - GetTransmitTimestamp(ntpData));
            return (int)span.TotalMilliseconds;
        }

        /// <summary>
        /// Local clock offset (in milliseconds)
        /// </summary>
        /// <param name="ntpData"></param>
        /// <returns></returns>
        public int GetLocalClockOffset(byte[] ntpData)
        {
            TimeSpan span = (GetReceiveTimestamp(ntpData) - GetOriginateTimestamp(ntpData)) - (_receptionTimestamp - GetTransmitTimestamp(ntpData));
            return (int)(span.TotalMilliseconds / 2);
        }

        /// <summary>
        /// Compute date, given the number of milliseconds since January 1, 1900
        /// </summary>
        /// <param name="milliseconds"></param>
        /// <returns></returns>
        private DateTime ComputeDate(ulong milliseconds)
        {
            TimeSpan span = TimeSpan.FromMilliseconds(milliseconds);
            DateTime time = new DateTime(1900, 1, 1);
            time += span;
            return time;
        }

        // Compute the number of milliseconds, given the offset of a 8-byte array
        private ulong GetMilliSeconds(byte[] ntpData, byte offset)
        {
            ulong intpart = 0, fractpart = 0;

            for (int i = 0; i <= 3; i++)
            {
                intpart = 256 * intpart + ntpData[offset + i];
            }
            for (int i = 4; i <= 7; i++)
            {
                fractpart = 256 * fractpart + ntpData[offset + i];
            }
            ulong milliseconds = intpart * 1000 + (fractpart * 1000) / 0x100000000L;
            return milliseconds;
        }

        /*
        // Compute the 8-byte array, given the date
        private void SetDate(byte[] ntpData, byte offset, DateTime date)
        {
            ulong intpart, fractpart;
            DateTime StartOfCentury = new DateTime(1900, 1, 1, 0, 0, 0);	// January 1, 1900 12:00 AM

            ulong milliseconds = (ulong)(date - StartOfCentury).TotalMilliseconds;
            intpart = milliseconds / 1000;
            fractpart = ((milliseconds % 1000) * 0x100000000L) / 1000;

            ulong temp = intpart;
            for (int i = 3; i >= 0; i--)
            {
                ntpData[offset + i] = (byte)(temp % 256);
                temp = temp / 256;
            }

            temp = fractpart;
            for (int i = 7; i >= 4; i--)
            {
                ntpData[offset + i] = (byte)(temp % 256);
                temp = temp / 256;
            }
        }*/

        // Initialize the NTPClient data
        private void InitializeNtpSendData(byte[] ntpData)
        {
            Array.Clear(ntpData, 0, ntpData.Length);

            // Set version number to 4 and Mode to 3 (client)
            ntpData[0] = 0x1B;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="host"></param>
        public SNTP(string host)
        {
            _timeServer = host;
        }

        /// <summary>
        /// 
        /// </summary>
        public SNTP()
        {

        }

        /// <summary>
        /// Check if the response from server is valid
        /// </summary>
        /// <param name="ntpData"></param>
        /// <returns></returns>
        public bool IsResponseValid(byte[] ntpData)
        {
            if (ntpData.Length < NTP_DATA_LENGTH || GetMode(ntpData) != SntpMode.Server)
            {
                return false;
            }
            
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ntpData"></param>
        /// <returns></returns>
        public string GetInfo(byte[] ntpData)
        {
            string str;

            str = "Leap Indicator: ";
            switch (GetLeapIndicator(ntpData))
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
            str += "\r\nVersion number: " + GetVersionNumber(ntpData) + "\r\n";
            str += "Mode: ";
            switch (GetMode(ntpData))
            {
                case SntpMode.Unknown0:
                case SntpMode.Unknown6:
                case SntpMode.Unknown7:
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
            switch (GetStratum(ntpData))
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
            str += "\r\nLocal time: " + GetTransmitTimestamp(ntpData);
            str += "\r\nPrecision: " + GetPrecision(ntpData) + " ms";
            str += "\r\nPoll Interval: " + GetPollInterval(ntpData) + " s";
            str += "\r\nReference ID: " + GetReferenceID(ntpData);
            str += "\r\nRoot Dispersion: " + GetRootDispersion(ntpData) + " ms";
            str += "\r\nRound Trip Delay: " + GetRoundTripDelay(ntpData) + " ms";
            str += "\r\nLocal Clock Offset: " + GetLocalClockOffset(ntpData) + " ms";
            str += "\r\n";

            return str;
        }

        private uint _allowedTimeTolerance;

        /// <summary>
        /// Set absolute value in miliseconds. Time won't be set if difference between current time and time recieved from server is lower than this value.
        /// </summary>
        public uint AllowedTimeTolerance
        {
            get { return _allowedTimeTolerance; }
            set { _allowedTimeTolerance = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public event Action<DateTime> TimeSynchronized;
        /// <summary>
        /// 
        /// </summary>
        public event Action<DateTime, DateTime> TimeSynchronizedWithPreviousTime;

        // Set system time according to transmit timestamp
        private bool SetTime(byte[] ntpData, out bool timeChange)
        {
            DllCoredll.SYSTEMTIME systemTime;

            DateTime recievedTime;
            TimeSpan difference;

            if (SystemTime.UseDateTimeNow)
            {
                recievedTime = GetTransmitUtcTimestamp(ntpData);
                difference = SystemTime.GetSystemTime() - recievedTime;
            }
            else
            {
                recievedTime = GetTransmitTimestamp(ntpData);
                difference = SystemTime.GetLocalTime() - recievedTime;
            }

            //if difference between current time and recieved time is higher than allowed time difference set time           
            if (Math.Abs(difference.TotalMilliseconds) > _allowedTimeTolerance)
            {
                systemTime.wYear = (ushort)recievedTime.Year;
                systemTime.wMonth = (ushort)recievedTime.Month;
                systemTime.wDayOfWeek = (ushort)recievedTime.DayOfWeek;
                systemTime.wDay = (ushort)recievedTime.Day;
                systemTime.wHour = (ushort)recievedTime.Hour;
                systemTime.wMinute = (ushort)recievedTime.Minute;
                systemTime.wSecond = (ushort)recievedTime.Second;
                systemTime.wMilliseconds = (ushort)recievedTime.Millisecond;

                try
                {
                    DateTime previousTime = SystemTime.GetLocalTime();

                    if (!SystemTime.UseDateTimeNow)
                    {
                        if ((DllCoredll.SetLocalTime(ref systemTime) == 1))
                        {
                            InvokeTimeSynchronized(previousTime, recievedTime);
                            timeChange = true;
                            return true;
                        }
                    }
                    else
                    {
                        if ((DllCoredll.SetSystemTime(ref systemTime) == 1))
                        {
                            InvokeTimeSynchronized(previousTime, recievedTime);
                            timeChange = true;
                            return true;
                        }
                    }
                }
                catch
                {
                    timeChange = false;
                    return false;
                }
            }
            timeChange = false;
            return true;
        }

        private void InvokeTimeSynchronized(DateTime previousTime, DateTime recievedTime)
        {
            if (TimeSynchronized != null)
            {
                try
                {
                    TimeSynchronized(recievedTime);
                }
                catch { }
            }

            if (TimeSynchronizedWithPreviousTime != null)
            {
                try
                {
                    TimeSynchronizedWithPreviousTime(previousTime, recievedTime);
                }
                catch { }
            }
        }

        // The URL of the time server we're connecting to
// ReSharper disable once NotAccessedField.Local
        private string _timeServer;

        /// <summary>
        /// 
        /// </summary>
        public enum SntpSynchronizationError : byte
        {
            /// <summary>
            /// 
            /// </summary>
            InvalidResponse,
            /// <summary>
            /// 
            /// </summary>
            SocketExeption,
            /// <summary>
            /// 
            /// </summary>
            NoAvailableData,
            /// <summary>
            /// 
            /// </summary>
            TimeNotSet
        }

        #region ISntpClient Members

        /*
        private void Validate(IPAddress[] ipAddresses)
        {
            Validator.CheckNull(ipAddresses);
        }

        private void ValidateHosts(string[] hostNames)
        {
            Validator.CheckNull(hostNames);
        }*/

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeChanged"></param>
        /// <param name="ipAddresses"></param>
        /// <returns></returns>
        public bool Synchronize(out bool timeChanged, params IPAddress[] ipAddresses)
        {
            return Synchronize(string.Empty, out timeChanged, ipAddresses);
        }

        private bool Synchronize(string hostName, out bool timeChanged, params IPAddress[] ipAddresses)
        {
            string errorMessage = string.Empty;
            byte[] ntpSendData = new byte[NTP_DATA_LENGTH];
            byte[] ntpRecieveData = null;

            if (hostName != string.Empty)
            {
                hostName = hostName + " ";
            }

            for (int i = 0; i < ipAddresses.Length; i++)
            {
                try
                {
                    if (!PerformNTPDataRequest(ntpSendData, out ntpRecieveData, ipAddresses[i]))
                    {
                        errorMessage += "No available data send from: " + hostName + ipAddresses[i] + "\n";
                        if (ContinueWithAnotherServer(i >= ipAddresses.Length - 1, errorMessage)) continue;
                    }
                }
                catch (SocketException e)
                {
                    errorMessage += "Socket exception error " + e.Message + "\n";
                    if (ContinueWithAnotherServer(i >= ipAddresses.Length - 1, errorMessage)) continue;
                }
                if (!IsResponseValid(ntpRecieveData))
                {
                    errorMessage += "Invalid response from host: " + hostName + ipAddresses[i] + "\n";
                    if (ContinueWithAnotherServer(i >= ipAddresses.Length - 1, errorMessage)) continue;
                }

                // Update system time with recieved ntp data
                if (UpdateSystemTyme(ntpRecieveData, out timeChanged))
                {
                    //Time was updated or is actual                     
                    return true;
                }

                ContinueWithAnotherServer(i >= ipAddresses.Length - 1, errorMessage);
            }

            timeChanged = false;
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeChanged"></param>
        /// <param name="hostNames"></param>
        /// <returns></returns>
        public bool Synchronize(out bool timeChanged, string[] hostNames)
        {
            for (int i = 0; i < hostNames.Length; i++)
            {
                try
                {
                    /* First try to parse IP directly, if it fails DNS will be used */
                    IPAddress ipAddress = IPAddress.Parse(hostNames[i]);
                    if (Synchronize(hostNames[i], out timeChanged, new[] {ipAddress}))
                        return true;
                }
                catch (FormatException)
                {
                    IPHostEntry hostAddresses = Dns.GetHostEntry(hostNames[i]);
                    if (null == hostAddresses || hostAddresses.AddressList.Length == 0)
                        continue;

                    if ((Synchronize(hostNames[i], out timeChanged, hostAddresses.AddressList)))
                        return true;
                }
            }

            timeChanged = false;
            return false;
        }

        private bool PerformNTPDataRequest(byte[] ntpSendData, out byte[] ntpRecieveData, IPAddress ipAddress)
        {
            ntpRecieveData = new byte[NTP_DATA_LENGTH];
            EndPoint remoteEP = new IPEndPoint(ipAddress, 123);

            // Initialize data structure
            InitializeNtpSendData(ntpSendData);

            Socket udpTimeSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            udpTimeSocket.SendTo(ntpSendData, ntpSendData.Length, SocketFlags.None, remoteEP);

            int availableData = -1;

            if (udpTimeSocket.Poll(TIME_SOCKET_POLL_INTERVAL, SelectMode.SelectRead))
                availableData = udpTimeSocket.ReceiveFrom(ntpRecieveData, ref remoteEP);

            _receptionTimestamp = SystemTime.GetLocalTime();

            if (availableData <= 0)
            {
                return false;
            }

            return true;
        }

        private bool UpdateSystemTyme(byte[] ntpRecieveData, out bool newTimeApplied)
        {
            return SetTime(ntpRecieveData, out newTimeApplied);
        }

        private bool ContinueWithAnotherServer(bool isLastIp, string previousErrors)
        {
            if (!isLastIp)
            {
                return true;
            }
            
            throw new SntpSynchronizationException(SntpSynchronizationError.TimeNotSet, "Time failed to synchronize with servers " + previousErrors);
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public class SntpSynchronizationException : Exception
        {
            private SntpSynchronizationError _sntpSynchronizationError;

            /// <summary>
            /// 
            /// </summary>
            public SntpSynchronizationError SntpSynchronizationError
            {
                get { return _sntpSynchronizationError; }
                set { _sntpSynchronizationError = value; }
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="SyncError"></param>
            /// <param name="exception"></param>
            public SntpSynchronizationException(SntpSynchronizationError SyncError, Exception exception)
                : base(String.Empty, exception)
            {
                _sntpSynchronizationError = SyncError;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="SyncError"></param>
            /// <param name="message"></param>
            public SntpSynchronizationException(SntpSynchronizationError SyncError, string message)
                : base(message)
            {
                _sntpSynchronizationError = SyncError;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="SyncError"></param>
            /// <param name="message"></param>
            /// <param name="innerException"></param>
            public SntpSynchronizationException(SntpSynchronizationError SyncError, string message, Exception innerException)
                : base(message, innerException)
            {
                _sntpSynchronizationError = SyncError;
            }
        }
    }
}
