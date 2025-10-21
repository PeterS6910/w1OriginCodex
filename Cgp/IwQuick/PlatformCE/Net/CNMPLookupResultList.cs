using System.Collections.Generic;
using System.Net;

namespace Contal.IwQuick.Net
{
    /// <summary>
    /// list of results from the CUdpLookup class
    /// </summary>
    public class CNMPLookupResultList : IEnumerable<CNMPLookupResultItem>
    {
        private readonly Dictionary<string, CNMPLookupResultItem> _results = 
            new Dictionary<string, CNMPLookupResultItem>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="ipEndpoint"></param>
        /// <param name="instanceName"></param>
        /// <param name="typeName"></param>
        /// <param name="computerName"></param>
        /// <param name="timeStamp"></param>
        /// <param name="contalVersion"></param>
        /// <returns></returns>
        protected internal CNMPLookupResultItem SetResult(
            string hash, 
            IPEndPoint ipEndpoint, 
            string instanceName, 
            string typeName, 
            string computerName, 
            string timeStamp, 
            string contalVersion)
        {
            if (string.IsNullOrEmpty(hash) ||
                ipEndpoint == null)
                return null;

            // HASH + IPENDPOINT is a simple hack for some uniqueness over hash observed

            if (_results.ContainsKey(hash+ipEndpoint))
                return null;
            
            CNMPLookupResultItem resultItem = 
                new CNMPLookupResultItem(hash, ipEndpoint, instanceName, typeName, computerName, timeStamp, contalVersion);
            _results[hash+ipEndpoint] = resultItem;
            return resultItem;
        }

        /// <summary>
        /// retrieves all IP endpoints in the list
        /// </summary>
        /// <returns>
        /// array of IP edpoints,
        /// empty array, if nothing found
        /// </returns>
        public IPEndPoint[] GetAll()
        {
            IPEndPoint[] iPEndpoints = new IPEndPoint[_results.Count];
            int i = 0;
            foreach (CNMPLookupResultItem value in _results.Values)
            {
                iPEndpoints[i++] = value.EndPoint;
            }
            return iPEndpoints;
        }

        /// <summary>
        /// retrieves all IP addresses in the list
        /// </summary>
        /// <returns>
        /// array of IP addresses,
        /// empty array, if nothing found
        /// </returns>
        public IPAddress[] GetAllIP()
        {
            IPAddress[] iPEndpoints = new IPAddress[_results.Count];

            int i = 0;
            foreach (CNMPLookupResultItem value in _results.Values)
            {
                iPEndpoints[i++] = value.IP;
            }
            return iPEndpoints;
        }

        private CNMPLookupResultItem GetResult(string hash)
        {
            if (hash == string.Empty)
                return null;

            CNMPLookupResultItem aResult;
            if (!_results.TryGetValue(hash, out aResult))
                return null;

            return aResult;
        }

        /// <param name="hash"></param>
        /// <param name="name">name of the extra parameter</param>
        /// <returns>
        /// extra parameter value by its name
        /// </returns>
        public string GetExtra(string hash, string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            CNMPLookupResultItem result = GetResult(hash);
            if (null == result)
                return null;

            return result.GetExtra(name);
        }

        /// <summary>
        /// count of the lookup entities found
        /// </summary>
        public int Count
        {
            get { return _results.Count; }
        }

        #region IEnumerable<CNMPLookupResultItem> Members

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<CNMPLookupResultItem> GetEnumerator()
        {
            return _results.Values.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _results.Values.GetEnumerator();
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hash"></param>
        public CNMPLookupResultItem this[string hash]
        {
            get
            {
                return GetResult(hash);
            }
        }
    }
}
