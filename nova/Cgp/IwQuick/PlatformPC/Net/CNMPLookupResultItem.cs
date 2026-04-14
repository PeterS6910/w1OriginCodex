using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using JetBrains.Annotations;

namespace Contal.IwQuick.Net
{
    public class CNMPLookupResultItem
    {

        protected internal CNMPLookupResultItem(
            [NotNull] string identHash, 
            [NotNull] IPEndPoint ipEndpoint, 
            string instanceName,
            [NotNull] string typeName, 
            string computerName, 
            string dateTime, 
            NetworkInterface directNetIface, 
            string contalVersion)
        {
            Validator.CheckNullString(identHash);
            Validator.CheckForNull(ipEndpoint,"ipEndpoint");
            Validator.CheckForNull(typeName,"typeName");

            _identHash = identHash;
            _ipe = ipEndpoint;            
            _instanceName = instanceName;
            _typeName = typeName;
            _dateTime = dateTime;
            _computerName = computerName;
            _contalVersion = contalVersion;
            _directNetIface = directNetIface;
        }

        private readonly string _identHash;
        /// <summary>
        /// 
        /// </summary>
        public string IdentHash
        {
            get { return _identHash; }
        }

        private readonly IPEndPoint _ipe = null;
        /// <summary>
        /// 
        /// </summary>
        public IPAddress IP
        {
            get
            {
                return _ipe.Address;
            }
        }        

        /// <summary>
        /// 
        /// </summary>
        public int Port
        {
            get { return _ipe.Port; }
        }

        /// <summary>
        /// 
        /// </summary>
        public IPEndPoint EndPoint
        {
            get
            {
                return _ipe;
            }
        }

        private readonly NetworkInterface _directNetIface = null;
        /// <summary>
        /// 
        /// </summary>
        public NetworkInterface DirectNetworkInterface
        {
            get { return _directNetIface; }
        }

        private readonly string _instanceName = null;
        public string InstanceName
        {
            get { return _instanceName; }
        }

        private readonly string _typeName = null;
        public string TypeName
        {
            get { return _typeName; }
        }

        private readonly string _computerName = null;
        public string ComputerName
        {
            get { return _computerName; }
        }

        private readonly string _contalVersion = null;
        public string ContalVersion
        {
            get { return _contalVersion; }
        }

        private readonly string _dateTime = null;
        public string DateTime
        {
            get { return _dateTime; }
        }

        private IDictionary<string, string> _extras = null;
        public string GetExtra(string extraName)
        {
            if (null == _extras)
                return null;

            if (null == extraName)
                return null;

            string extraValue;
            if (!_extras.TryGetValue(extraName, out extraValue))
                return null;
            
            return extraValue;
        }

        protected internal void SetExtras(IDictionary<string, string> extras)
        {
            if (null == extras ||
                null != _extras)
                return;

            _extras = extras;
        }

        public int ExtrasCount
        {
            get
            {
                if (_extras == null)
                    return 0;
                return _extras.Count;
            }
        }
    }
}
