using System;
using System.Collections.Generic;
using System.Text;

using System.Net;
using System.Net.Sockets;

namespace Contal.IwQuick.Net
{
    /// <summary>
    /// TCP/UDP port helper class
    /// </summary>
    public class TcpUdpPort
    {
        /// <summary>
        /// returns true, if the integer specified port is valid TCP/UDP port
        /// </summary>
        /// <param name="port"></param>
        /// <param name="withAny">if true, 0 is considere valid port</param>
        public static bool IsValid(int port,bool withAny)
        {
            if (withAny)
                return (0 <= port && port <= ushort.MaxValue);
            else
                return (0 < port && port <= ushort.MaxValue);
        }

        /// <summary>
        /// checks the validity of the integer-specified port and throws exception if not valid
        /// </summary>
        /// <param name="port"></param>
        /// <param name="withAny">if true, 0 is considered valid</param>
        /// <exception cref="InvalidTransportPortException">if the specified port is invalid</exception>
        public static void CheckValidity(int port,bool withAny)
        {
            if (!IsValid(port,withAny))
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
        /// returns true, if the specified TCP port is free
        /// </summary>
        /// <param name="port">the TCP port</param>
        /// <param name="isLocal">if true, the binding is checked on loopback interface, otherwise on 0.0.0.0</param>
        /// <exception cref="InvalidTransportPortException">if the specified port is invalid</exception>
        public static bool IsFreeTcp(int port,bool isLocal)
        {
            CheckValidity(port);

            bool portIsFree = false;

            Socket socket = null;

            try
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                if (isLocal)
                    socket.Bind(new IPEndPoint(IPAddress.Loopback, port));
                else
                    socket.Bind(new IPEndPoint(IPAddress.Any, port));

                portIsFree = true;
            }
            catch
            {
                
            }

            if (null != socket)
            {
                try
                {
                    try
                    {
                        if (socket.LingerState.Enabled)
                            socket.LingerState = new LingerOption(false, 0);
                    }
                    catch { }
                    socket.Close();
                }
                catch
                {
                }
            }

            return portIsFree;

        }

        /// <summary>
        /// returns true, if the specified TCP port for specified Ip address is free
        /// </summary>
        /// <param name="port">the TCP port</param>
        /// <param name="ipAddress">ip address</param>        
        /// <exception cref="InvalidTransportPortException">if the specified port is invalid</exception>
        public static bool IsFreeTcp(int port, IPAddress ipAddress)
        {
            CheckValidity(port);

            bool portIsFree = false;

            Socket socket = null;

            try
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                socket.Bind(new IPEndPoint(ipAddress, port));

                portIsFree = true;
            }
            /*
        catch (SocketException aException)
        {
            if (aException.SocketErrorCode == SocketError.AddressAlreadyInUse)
                continue;
            else
                return false;
        }*/
            catch
            {
            }

            if (null != socket)
            {
                try
                {
                    try
                    {
                        if (socket.LingerState.Enabled)
                            socket.LingerState = new LingerOption(false, 0);
                    }
                    catch { }
                    socket.Close();
                }
                catch
                {
                }
            }

            return portIsFree;
        }


        /// <summary>
        /// returns true, if the specified UDP port is free
        /// </summary>
        /// <param name="port">the UDP port</param>
        /// <param name="isLocal">if true, the binding is checked on loopback interface, otherwise on 0.0.0.0</param>
        /// <exception cref="InvalidTransportPortException">if the specified port is invalid</exception>
        public static bool IsFreeUdp(int port, bool isLocalOnly)
        {
            CheckValidity(port);

            Socket socket = null;
            bool portIsFree = false;
            try
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                if (isLocalOnly)
                    socket.Bind(new IPEndPoint(IPAddress.Loopback, port));
                else
                    socket.Bind(new IPEndPoint(IPAddress.Any, port));               

                portIsFree = true;
            }
            /*
        catch (SocketException aException)
        {
            if (aException.SocketErrorCode == SocketError.AddressAlreadyInUse)
                continue;
            else
                return false;
        }*/
            catch
            {
            }
            finally
            {
                if (socket != null)
                {
                    try { socket.Close(); }
                    catch { }
                }
            }

            return portIsFree;
        } 

        /// <summary>
        /// returns true, if the specified UDP port for specified Ip address is free
        /// </summary>
        /// <param name="port">the UDP port</param>
        /// <param name="ipAddress">ip address</param>
        /// <exception cref="InvalidTransportPortException">if the specified port is invalid</exception>
        public static bool IsFreeUdp(int port, IPAddress ipAddress)
        {
            CheckValidity(port);

            Socket aSocket = null;
            bool ret = false;
            try
            {
                aSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                aSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                
                aSocket.Bind(new IPEndPoint(ipAddress, port));

                ret = true;
            }
            /*
        catch (SocketException aException)
        {
            if (aException.SocketErrorCode == SocketError.AddressAlreadyInUse)
                continue;
            else
                return false;
        }*/
            catch
            {
            }
            finally
            {
                if (aSocket != null)
                {
                    try { aSocket.Close(); }
                    catch { }
                }
            }

            return ret;
        }



        /// <summary>
        /// searches for free TCP port from the specified port lower boundary and returns it;
        /// if not found any, the 0 is returned
        /// </summary>
        /// <param name="fromPort">lower boundary to search from</param>
        /// <param name="isLocalOnly">if true, the searching is fixed to loopback interface, otherwise on 0.0.0.0</param>
        /// <exception cref="InvalidTransportPortException">if the specified fromPort is invalid</exception>
        public static int GetTcpFree(int fromPort,bool isLocalOnly)
        {

            CheckValidity(fromPort);

            int found = 0;

            for (int i = fromPort; i <= ushort.MaxValue; i++)
            {
                if (IsFreeTcp((ushort)i,isLocalOnly))
                {
                    found = i;
                    break;
                }
            }

            return found;
        }

        /// <summary>
        /// searches for free TCP port for specified ip address from the specified port lower boundary and returns it;
        /// if not found any, the 0 is returned
        /// </summary>
        /// <param name="fromPort">lower boundary to search from</param>
        /// <param name="ipAddress">ip address</param>
        /// <exception cref="InvalidTransportPortException">if the specified fromPort is invalid</exception>
        public static int GetTcpFree(int fromPort, IPAddress ipAddress)
        {

            CheckValidity(fromPort);

            int found = 0;

            for (int i = fromPort; i <= ushort.MaxValue; i++)
            {
                if (IsFreeTcp((ushort)i, ipAddress))
                {
                    found = i;
                    break;
                }
            }

            return found;
        }




        /// <summary>
        /// searches for free TCP port from in descending order from the port ushort.MaxValue;
        /// if not found any, the 0 is returned
        /// </summary>
        /// <param name="isLocalOnly">if true, the searching is fixed to loopback interface, otherwise on 0.0.0.0</param>
        /// <returns></returns>
        public static int GetTcpFreeDescending(bool isLocalOnly)
        {
            int found = 0;

            for (int i = ushort.MaxValue; i > 0; i--)
            {
                if (IsFreeTcp((ushort)i, isLocalOnly))
                {
                    found = i;
                    break;
                }
            }

            return found;
        }

        /// <summary>
        /// searches for free TCP port for specified ip address from in descending order from the port ushort.MaxValue;
        /// if not found any, the 0 is returned
        /// </summary>
        /// <param name="ipAddress">ip address</param>
        /// <returns></returns>
        public static int GetTcpFreeDescending(IPAddress ipAddress)
        {
            int found = 0;

            for (int i = ushort.MaxValue; i > 0; i--)
            {
                if (IsFreeTcp((ushort)i, ipAddress))
                {
                    found = i;
                    break;
                }
            }

            return found;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isLocalOnly"></param>
        /// <param name="intervalStart"></param>
        /// <param name="intervalStop"></param>
        /// <returns></returns>
        public static int GetTcpFreeDescending(bool isLocalOnly,int intervalStart,int intervalStop)
        {
            // defensive to params
            if (intervalStart < 1 || intervalStart > ushort.MaxValue)
                intervalStart = 1;

            if (intervalStop < 1 || intervalStop > ushort.MaxValue)
                intervalStop = ushort.MaxValue;

            if (intervalStop < intervalStart)
            {
                int tmp = intervalStop;
                intervalStop = intervalStart;
                intervalStart = tmp;
            }

            int found = 0;

            for (int i = intervalStop; i >= intervalStart; i--)
            {
                if (IsFreeTcp((ushort)i, isLocalOnly))
                {
                    found = i;
                    break;
                }
            }

            return found;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isLocalOnly"></param>
        /// <param name="intervalStart"></param>
        /// <param name="intervalStop"></param>
        /// <returns></returns>
        public static int GetTcpFree(bool isLocalOnly, int intervalStart, int intervalStop)
        {
            // defensive to params
            if (intervalStart < 1 || intervalStart > ushort.MaxValue)
                intervalStart = 1;

            if (intervalStop < 1 || intervalStop > ushort.MaxValue)
                intervalStop = ushort.MaxValue;

            if (intervalStop < intervalStart)
            {
                int tmp = intervalStop;
                intervalStop = intervalStart;
                intervalStart = tmp;
            }

            int found = 0;

            for (int i = intervalStart; i <= intervalStop; i++)
            {
                if (IsFreeTcp((ushort)i, isLocalOnly))
                {
                    found = i;
                    break;
                }
            }

            return found;
        }

        /// <summary>
        /// searches for free UDP port from the specified port lower boundary and returns it;
        /// if not found any, the 0 is returned
        /// </summary>
        /// <param name="fromPort">lower boundary to search from</param>
        /// <param name="isLocalOnly">if true, the searching is fixed to loopback interface, otherwise on 0.0.0.0</param>
        /// <exception cref="InvalidTransportPortException">if the specified fromPort is invalid</exception>
        public static int GetUdpFree(int fromPort, bool isLocalOnly)
        {
            CheckValidity(fromPort);

            int found = 0;

            for (int i = fromPort; i <= ushort.MaxValue; i++)
            {
                if (IsFreeUdp((ushort)i, isLocalOnly))
                {
                    found = i;
                    break;
                }
            }

            return found;
        }

        /// <summary>
        /// searches for free UDP port for specified ip address from the specified port lower boundary and returns it;
        /// if not found any, the 0 is returned
        /// </summary>
        /// <param name="fromPort">lower boundary to search from</param>
        /// <param name="ipAddress">ip address</param>
        /// <exception cref="InvalidTransportPortException">if the specified fromPort is invalid</exception>
        public static int GetUdpFree(int fromPort, IPAddress ipAddress)
        {
            CheckValidity(fromPort);

            int found = 0;

            for (int i = fromPort; i <= ushort.MaxValue; i++)
            {
                if (IsFreeUdp((ushort)i, ipAddress))
                {
                    found = i;
                    break;
                }
            }

            return found;
        }

        /// <summary>
        /// searches for free UDP port from in descending order from the port ushort.MaxValue;
        /// if not found any, the 0 is returned
        /// </summary>
        /// <param name="isLocalOnly">if true, the searching is fixed to loopback interface, otherwise on 0.0.0.0</param>
        /// <returns></returns>
        public static int GetUdpFreeDescending(bool isLocalOnly)
        {
            int found = 0;

            for (int i = ushort.MaxValue; i > 0; i--)
            {
                if (IsFreeUdp((ushort)i, isLocalOnly))
                {
                    found = i;
                    break;
                }
            }

            return found;
        }


        /// <summary>
        /// searches for free UDP port for specified ip address from in descending order from the port ushort.MaxValue;
        /// if not found any, the 0 is returned
        /// </summary>
        /// <param name="ipAddress">ip address</param>
        /// <returns></returns>
        public static int GetUdpFreeDescending(IPAddress ipAddress)
        {
            int found = 0;

            for (int i = ushort.MaxValue; i > 0; i--)
            {
                if (IsFreeUdp((ushort)i, ipAddress))
                {
                    found = i;
                    break;
                }
            }

            return found;
        }
    }
}
