using System;
using System.Collections.Generic;
using System.Text;

namespace Contal.IwQuick.Data
{
    public interface IVersionedAttribute
    {
        int Version { get; }
    }
}
