using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Contal.Cgp.Globals;

namespace Contal.Cgp.NCAS.Server.Beans
{
    public interface ICardReaderObject
    {
        IEnumerable<ICardReaderObject> GetChildObjects();
        ObjectType GetObjectType();
        object GetId();
    }
}
