using System;
using System.Collections.Generic;

using Contal.IwQuick;
using JetBrains.Annotations;

namespace Contal.Cgp.BaseLib
{
    [Serializable]
    public sealed class CgpPluginSharedData : ASingleton<CgpPluginSharedData>
    {
        private readonly Dictionary<string, object> _stringKeyedData = 
            new Dictionary<string, object>();

        private CgpPluginSharedData() : base(null)
        {
        }

        public void RegisterData(
            [NotNull] ICgpPlugin plugin, 
            string dataName, 
            object data)
        {
            Validator.CheckForNull(plugin,"plugin");

            string cd = plugin.CgpDesignation;
            _stringKeyedData[cd+"/"+(dataName??"")] = data;
        }

        private readonly Dictionary<long, object> _intKeyedData = 
            new Dictionary<long, object>();

        public void RegisterGlobalData(long dataKey, object data)
        {
            if (_intKeyedData.ContainsKey(dataKey))
                throw new AlreadyExistsException(dataKey);

            _intKeyedData[dataKey] = data;
        }

        public void UnregisterGlobalData(long dataKey)
        {
            if (!_intKeyedData.ContainsKey(dataKey))
                throw new DoesNotExistException(dataKey);

            _intKeyedData.Remove(dataKey);
        }

        public object this[
            [NotNull] ICgpPlugin plugin, 
            string dataName]
        {
            get
            {
                Validator.CheckForNull(plugin,"plugin");

                string cd = plugin.CgpDesignation;

                object o;

                return 
                    _stringKeyedData.TryGetValue(cd + "/" + (dataName ?? ""), out o) 
                        ? o
                        : null;
            }
        }

        public object this[long dataKey]
        {
            get
            {
                object o;

                return 
                    _intKeyedData.TryGetValue(dataKey,out o)
                        ? o
                        : null;
            }
        }
    }
}
