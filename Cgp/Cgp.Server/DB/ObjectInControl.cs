using System.Collections.Generic;
using System.Threading;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick.Data;

namespace Contal.Cgp.Server.DB
{
    public class ObjectInControl
    {
        private const string SERVERSESIONID = "cgp.server";
        
        private readonly object _mutex = new object();
        private volatile bool _mutexAssigned;

        private int _currentVersion;

        private readonly IDictionary<object, int> _listClientSessionId = 
            new SyncDictionary<object, int>();

        public bool RemoveFromClientList(object sessionId)
        {
            _listClientSessionId.Remove(sessionId ?? SERVERSESIONID);
            return _listClientSessionId.Count == 0;
        }

        public void EditStart(object clientSessionId)
        {
            try
            {
                Lock();

                _listClientSessionId[clientSessionId ?? SERVERSESIONID] =
                    _currentVersion;
            }
            finally
            {
                Unlock();
            }
        }

        public bool BeginUpdate(object clientSessionId)
        {
            Lock();

            int version;

            return 
                _listClientSessionId.TryGetValue(
                    clientSessionId ?? SERVERSESIONID, 
                    out version) &&
                version == _currentVersion;
        }

        public void EndUpdate(
            bool succesfull, 
            string clientSessionId)
        {
            if (succesfull)
            {
                _listClientSessionId[clientSessionId ?? SERVERSESIONID] =
                    ++_currentVersion;
            }

            Unlock();
        }

        public bool EditEnd(object clientSessionId)
        {
            _listClientSessionId.Remove(clientSessionId ?? SERVERSESIONID);
            return _listClientSessionId.Count == 0;
        }

        public void Lock()
        {
            lock (_mutex)
            {
                if (_mutexAssigned)
                    Monitor.Wait(_mutex);

                _mutexAssigned = true;
            }
        }

        public void Unlock()
        {
            lock (_mutex)
            {
                if (!_mutexAssigned)
                    return;

                _mutexAssigned = false;
                Monitor.Pulse(_mutex);
            }
        }
    }
}
