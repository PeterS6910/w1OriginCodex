using System;
using System.Net;
using System.Net.Sockets;

namespace Contal.IwQuickCF.Net
{
    public class TcpUdpPort
    {
        /// <summary>
        /// returns true, if the integer specified port is valid TCP/UDP port
        /// </summary>
        /// <param name="port"></param>
        /// <param name="withAny">if true, 0 is considere valid port</param>
        public static bool IsValid(int port, bool withAny)
        {
            if (withAny)
                return (0 <= port && port <= ushort.MaxValue);
            return (0 < port && port <= ushort.MaxValue);
        }

        /// <summary>
        /// returns true, if the integer specified port is valid TCP/UDP port (1-65535)
        /// </summary>
        /// <param name="port"></param>
        public static bool IsValid(int port)
        {
            return IsValid(port, false);
        }

        /// <summary>
        /// checks the validity of the integer-specified port and throws exception if not valid
        /// </summary>
        /// <param name="port"></param>
        /// <param name="withAny">if true, 0 is considered valid</param>
        /// <exception cref="InvalidTransportPortException">if the specified port is invalid</exception>
        public static void CheckValidity(int port, bool withAny)
        {
            if (!IsValid(port, withAny))
                throw new InvalidTransportPortException(port);
        }

        /// <summary>
        /// checks the validity of the integer-specified port and throws exception if not valid
        /// </summary>
        /// <param name="port"></param>
        /// <exception cref="InvalidTransportPortException">if the specified port is invalid</exception>
        public static void CheckValidity(int port)
        {
            if (!IsValid(port, false))
                throw new InvalidTransportPortException(port);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="isLocalOnly"></param>
        /// <returns></returns>
        public static bool IsFreeTcp(int port,bool isLocalOnly)
        {
            try
            {
                Socket testSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                if (isLocalOnly)
                    testSocket.Bind(new IPEndPoint(IPAddress.Loopback, port));
                else
                    testSocket.Bind(new IPEndPoint(IPAddress.Any, port));

                testSocket.Close();

                return true;
            }
                /*
            catch (SocketException aException)
            {
                if (aException.SocketErrorCode == SocketError.AddressAlreadyInUse)
                    continue;
                else
                    return false;
            }*/
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="isLocalOnly"></param>
        /// <returns></returns>
        public static bool IsFreeUdp(int port, bool isLocalOnly)
        {
            try
            {
                Socket aSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                if (isLocalOnly)
                    aSocket.Bind(new IPEndPoint(IPAddress.Loopback, port));
                else
                    aSocket.Bind(new IPEndPoint(IPAddress.Any, port));

                aSocket.Close();

                return true;
            }
            /*
        catch (SocketException aException)
        {
            if (aException.SocketErrorCode == SocketError.AddressAlreadyInUse)
                continue;
            else
                return false;
        }*/
            catch (Exception)
            {
                return false;
            }
        }

        public static int GetTcpFree(UInt16 i_iFromPort,bool i_bLocalOnly)
        {
            int iFound = 0;

            for (int i = i_iFromPort; i <= 65535; i++)
            {
                if (IsFreeTcp((ushort)i,i_bLocalOnly))
                {
                    iFound = i;
                    break;
                }
            }

            return iFound;
        }

        public static int GetTcpFreeDescending(bool i_bLocalOnly)
        {
            int iFound = 0;

            for (int i = 65535; i > 0; i--)
            {
                if (IsFreeTcp((ushort)i, i_bLocalOnly))
                {
                    iFound = i;
                    break;
                }
            }

            return iFound;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromPort"></param>
        /// <param name="isLocalOnly"></param>
        /// <returns></returns>
        public static int GetUdpFree(UInt16 fromPort, bool isLocalOnly)
        {
            int iFound = 0;

            for (int i = fromPort; i <= 65535; i++)
            {
                if (IsFreeUdp((ushort)i, isLocalOnly))
                {
                    iFound = i;
                    break;
                }
            }

            return iFound;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isLocalOnly"></param>
        /// <returns></returns>
        public static int GetUdpFreeDescending(bool isLocalOnly)
        {
            int iFound = 0;

            for (int i = 65535; i > 0; i--)
            {
                if (IsFreeUdp((ushort)i, isLocalOnly))
                {
                    iFound = i;
                    break;
                }
            }

            return iFound;
        }
    }
}
