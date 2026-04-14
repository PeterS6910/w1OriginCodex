using System.Collections.Generic;
using System.Net;
using Contal.IwQuick;
using JetBrains.Annotations;

namespace Contal.IwQuick.Net
{
    /// <summary>
    /// 
    /// </summary>
    public class CNMPLookupResultItem
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="ipEndpoint"></param>
        /// <param name="instanceName"></param>
        /// <param name="typeName"></param>
        /// <param name="computerName"></param>
        /// <param name="dateTime"></param>
        /// <param name="contalVersion"></param>
        public CNMPLookupResultItem(
            string hash, 
            [NotNull] IPEndPoint ipEndpoint,
            string instanceName, 
            [NotNull] string typeName, 
            string computerName,
            string dateTime, 
            string contalVersion)
        {
            Validator.CheckForNull(ipEndpoint,"ipEndpoint");
            Validator.CheckForNull(typeName,"typeName");

            _hash = hash;
            _ipe = ipEndpoint;
            _instanceName = instanceName;
            _typeName = typeName;
            _dateTime = dateTime;
            _computerName = computerName;
            _contalVersion = contalVersion;
        }
        
        private readonly string _hash;
        /// <summary>
        /// 
        /// </summary>
        public string Hash
        {
            get { return _hash; }
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

        private readonly string _instanceName = null;
        /// <summary>
        /// 
        /// </summary>
        public string InstanceName
        {
            get { return _instanceName; }
        }

        private readonly string _typeName = null;
        /// <summary>
        /// 
        /// </summary>
        public string TypeName
        {
            get { return _typeName; }
        }

        private readonly string _computerName = null;
        /// <summary>
        /// 
        /// </summary>
        public string ComputerName
        {
            get { return _computerName; }
        }

        private readonly string _contalVersion = null;
        /// <summary>
        /// 
        /// </summary>
        public string ContalVersion
        {
            get { return _contalVersion; }
        }

        private readonly string _dateTime = null;
        /// <summary>
        /// 
        /// </summary>
        public string DateTime
        {
            get { return _dateTime; }
        }

        private IDictionary<string, string> _extras = null;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="extraName"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
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
