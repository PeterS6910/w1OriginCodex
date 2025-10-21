using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Contal.Cgp.RemotingCommon
{
    public interface IOrmRemotingObject
    {
        bool ImplementsInterface<T>();
        bool Compare(object obj);
    }
}
