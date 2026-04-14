using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Contal.IwQuick.Remoting
{
    /// <summary>
    /// abstract class for all objects, which methods will invoked as callbacks from the remoting server
    /// </summary>
    public abstract class ARemotingCallbackHandler: ARemotingClient
    {
        private readonly string _name;
        /// <summary>
        /// mandatory name of the client handler
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
        }

        /// <summary>
        /// explicit constructor
        /// </summary>
        /// <param name="name"></param>
        protected ARemotingCallbackHandler([NotNull] string name)
        {
            Validator.CheckNullString(name);
            _name = name;
        }

        /// <summary>
        /// implicitly disallowes lifetime GC mechanism over remoted objects
        /// </summary>
        /// <returns></returns>
        public override object InitializeLifetimeService()
        {
            return null;
        }

        private readonly LinkedList<ARemotingService> _attachedToServices = new LinkedList<ARemotingService>();

        public void RegisterAttachedTo(ARemotingService service)
        {
            if (null == service)
                return;

            lock (_attachedToServices)
            {
                if (!_attachedToServices.Contains(service))
                {
                    _attachedToServices.AddLast(service);
                }
            }
        }


        public void DetachAll()
        {
            lock (_attachedToServices)
            {
                foreach (ARemotingService service in _attachedToServices)
                    try
                    {
                        service.DetachCallbackHandler(this);
                    }
// ReSharper disable once EmptyGeneralCatchClause
                    catch (Exception)
                    {
                    }

                _attachedToServices.Clear();
            }
        }
    }
}
