using Contal.Cgp.BaseLib;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.RemotingCommon;
using Contal.Cgp.RemotingCommon;
using Contal.Cgp.Server.Beans;
using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Contal.Cgp.Server.DB
{
    public sealed class Cars :
        ABaserOrmTableWithAlarmInstruction<Cars, Car>,
        ICars
    {
        private Cars()
            : base(null, new CudPreparationForObjectWithVersion<Car>())
        {
        }

        protected override void AddOrder(ref ICriteria c)
        {
            c.AddOrder(new Order(Car.COLUMNLP, true));
        }

        protected override IEnumerable<Car> GetObjectsWithLocalAlarmInstruction()
        {
            return null;
        }

        public bool InsertCar(ref Car obj, out Exception insertException)
        {
            obj.WholeName = obj.Lp;
            return Insert(ref obj, out insertException);
        }

        public bool UpdateCar(Car obj, out Exception updateException)
        {
            obj.WholeName = obj.Lp;
            return Update(obj, out updateException);
        }

        public new bool UpdateOnlyInDatabase(Car obj, out Exception updateException)
        {
            obj.WholeName = obj.Lp;
            return Update(obj, out updateException, false);
        }

        public override bool HasAccessView(Login login)
        {
            return AccessChecker.HasAccessControl(
                BaseAccess.GetAccessesForGroup(LoginAccessGroups.PERSONS), login);
        }

        public override bool HasAccessInsert(Login login)
        {
            return AccessChecker.HasAccessControl(
                BaseAccess.GetAccess(LoginAccess.PersonsInsertDeletePerform), login);
        }

        public override bool HasAccessUpdate(Login login)
        {
            return AccessChecker.HasAccessControl(
                BaseAccess.GetAccessesForGroup(LoginAccessGroups.PERSONS), login);
        }

        public override bool HasAccessDelete(Login login)
        {
            return AccessChecker.HasAccessControl(
                BaseAccess.GetAccess(LoginAccess.PersonsInsertDeletePerform), login);
        }

        public ICollection<CarShort> ShortSelectByCriteria(IList<FilterSettings> filterSettings, out Exception error)
        {
            var listCars = SelectByCriteria(filterSettings, out error);
            ICollection<CarShort> result = new List<CarShort>();
            if (listCars != null)
            {
                foreach (var car in listCars)
                {
                    result.Add(new CarShort(car)
                    {
                         IdCar = car.IdCar,
                         Lp =  car.Lp,
                         Brand = car.Brand,
                         ValidityDateFrom = car.ValidityDateFrom,
                         ValidityDateTo = car.ValidityDateTo,
                         SecurityLevel = car.SecurityLevel,
                         Description = car.Description,
                    });
                }
            }
            return result;
        }

        public IList<IModifyObject> ListModifyObjects(out Exception error)
        {
            var listCars = List(out error);
            IList<IModifyObject> result = null;
            if (listCars != null)
            {
                result = new List<IModifyObject>();
                foreach (var car in listCars)
                    result.Add(new CarModifyObj(car));
                result = result.OrderBy(c => c.ToString()).ToList();
            }
            return result;
        }

        public override Contal.Cgp.Globals.ObjectType ObjectType
        {
            get { return ObjectType.Car; }
        }
    }
}
