using System.Collections.Generic;
using System.Linq;

using Contal.Cgp.Server.Beans;

namespace Contal.Cgp.Server.DB
{
    public class CudPreparationForObjectWithVersion<T>
        : ICudPreparation<T>
        where T : AOrmObjectWithVersion
    {
        public virtual void BeforeCreate(T obj)
        {
            BeforeCreateOrUpdate(obj);
        }

        public virtual void BeforeUpdate(T obj)
        {
            BeforeCreateOrUpdate(obj);
        }

        public virtual void BeforeDelete(T obj)
        {
        }

        protected virtual IEnumerable<AOrmObjectWithVersion> GetSubObjects(T obj)
        {
            return Enumerable.Empty<AOrmObjectWithVersion>();
        }

        private void BeforeCreateOrUpdate(T obj)
        {
            OrmObjectsVersionService.Singleton.IncrementVersions(
                Enumerable
                    .Repeat(
                        obj,
                        1)
                    .Concat(GetSubObjects(obj)));
        }
    }
}
