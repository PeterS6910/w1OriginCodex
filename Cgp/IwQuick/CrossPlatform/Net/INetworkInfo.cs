using System.Collections.Generic;
using System.Net;
using JetBrains.Annotations;

namespace Contal.IwQuick.Net
{
    public interface INetworkInfo
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        bool IsLocalIp([NotNull] IPAddress ip);

        /// <summary>
        /// 
        /// </summary>
        [CanBeNull]
        IEnumerable<IPAddress> LocalIpAddresses { get; }

        /// <summary>
        /// 
        /// </summary>
        [CanBeNull]
        IEnumerable<string> GetIpAddressReport();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        bool IsLocalNetwork4([NotNull] IPAddress ip);

        string MachineName { get; }
    }
}