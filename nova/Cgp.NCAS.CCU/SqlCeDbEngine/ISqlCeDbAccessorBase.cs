using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Contal.Cgp.NCAS.CCU.SqlCeDbEngine
{
    public interface ISqlCeDbAccessorBase
    {
        void Load();

        void DeleteAllFromDatabase();
    }
}
