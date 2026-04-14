using System;
using System.Collections.Generic;
using System.Text;

namespace Contal.IwQuick.Net
{
    public interface ITransportSettings
    {
        /// <summary>
        /// Get transport layer
        /// </summary>
        /// <returns>return transpor layer</returns>
        ITransportLayer GetLayer();
    }
}
