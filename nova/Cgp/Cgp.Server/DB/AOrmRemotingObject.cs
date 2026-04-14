using System;

using Contal.Cgp.RemotingCommon;

namespace Contal.Cgp.Server.DB
{
    public abstract class AOrmRemotingObject : MarshalByRefObject, IOrmRemotingObject
    {

        public override object InitializeLifetimeService()
        {
            return null;
        }


        public bool ImplementsInterface<T>()
        {
            bool b = typeof(T).IsAssignableFrom(GetType());
            return b;
        }

        public virtual bool Compare(object obj)
        {
            return false;
        }
    }
}
