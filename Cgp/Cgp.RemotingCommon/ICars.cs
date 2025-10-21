using System;
using System.Collections.Generic;
using Contal.Cgp.BaseLib;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Globals;

namespace Contal.Cgp.RemotingCommon
{
    public interface ICars : IBaseOrmTable<Car>
    {
        bool InsertCar(ref Car obj, out Exception insertException);
        bool UpdateCar(Car obj, out Exception updateException);
        bool UpdateOnlyInDatabase(Car obj, out Exception updateException);
        ICollection<CarShort> ShortSelectByCriteria(IList<FilterSettings> filterSettings, out Exception error);
        IList<IModifyObject> ListModifyObjects(out Exception error);
    }
}
