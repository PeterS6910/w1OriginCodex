using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using Contal.IwQuick.Data;
using JetBrains.Annotations;

namespace Contal.IwQuick.Net
{
    /// <summary>
    /// list of results from the CNMPAgent class
    /// </summary>
    public class CNMPLookupResultList : IEnumerable<CNMPLookupResultItem>
    {      
        private readonly SyncDictionary<string, CNMPLookupResultItem> _results = 
            new SyncDictionary<string, CNMPLookupResultItem>();

        private readonly CNMPAgent _parentAgent;

        protected internal CNMPLookupResultList([NotNull] CNMPAgent parentAgent)
        {
            Validator.CheckForNull(parentAgent,"parentAgent");

            _parentAgent = parentAgent;
        }

        protected internal CNMPLookupResultItem SetResult(
            string identHash, 
            IPEndPoint ipEndpoint, 
            string instanceName, 
            string typeName, 
            string computerName, 
            string timeStamp, 
            string contalVersion
            )
        {
            if (string.IsNullOrEmpty(identHash) ||
                ipEndpoint == null)
                return null;

            // HASH + IPENDPOINT is a simple hack for some uniqueness over hash observed

            if (_results.ContainsKey(identHash+ipEndpoint))
                return null;
            
            NetworkInterface ifaceOfReception = null;

            CNMPAgent.InterfacePeer[] iPeers = _parentAgent.GetInterfacePeers();

            if (null != iPeers)
                foreach(CNMPAgent.InterfacePeer ifacePeer in iPeers)
                {

                    if (ifacePeer == null)
                    {
                        DebugHelper.Keep(iPeers);
                        continue;
                    }
                        
                    if (IPHelper.IsSameNetwork4(ifacePeer.ipAddress, ifacePeer.subnetMask, ipEndpoint.Address))
                    {
                        ifaceOfReception = ifacePeer.networkInterface;
                        break;
                    }
                }
                
            var resultItem = new CNMPLookupResultItem(
                identHash, 
                ipEndpoint, 
                instanceName, 
                typeName, 
                computerName, 
                timeStamp, 
                ifaceOfReception, 
                contalVersion);
            _results[identHash + ipEndpoint] = resultItem;
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

        private CNMPLookupResultItem GetResult(string identHashAndEndpoint)
        {
            if (identHashAndEndpoint == string.Empty)
                return null;

            CNMPLookupResultItem aResult;
            if (!_results.TryGetValue(identHashAndEndpoint, out aResult))
                return null;

            return aResult;
        }

        public CNMPLookupResultItem[] GetResults(IPEndPoint ipEndpoint)
        {
            if (ipEndpoint == null)
                return null;

            LinkedList<CNMPLookupResultItem> listOfResults = new LinkedList<CNMPLookupResultItem>();
            foreach (CNMPLookupResultItem item in _results.Values)
            {
                if (item.EndPoint.Equals(ipEndpoint))
                    listOfResults.AddLast(item);
            }
            if (listOfResults.Count == 0)
                return null;

            CNMPLookupResultItem[] returnArray = new CNMPLookupResultItem[listOfResults.Count];
            listOfResults.CopyTo(returnArray, 0);

            return returnArray;
        }


        /// <param name="hash"></param>
        /// <param name="name">name of the extra parameter</param>
        /// <returns>
        /// extra parameter value by its name
        /// </returns>
        public string GetExtra(string hash, string name)
        {
            if (Validator.IsNullString(name))
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
       
        public CNMPLookupResultItem this[string identHashAndEndpoint]
        {
            get
            {
                return GetResult(identHashAndEndpoint);
            }
        }
    }
}
