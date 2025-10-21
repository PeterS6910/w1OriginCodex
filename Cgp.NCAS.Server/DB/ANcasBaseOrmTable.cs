using Contal.Cgp.ORM;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Server.DB;

namespace Contal.Cgp.NCAS.Server.DB
{
    public abstract class ANcasBaseOrmTable<TSingleton, T> : ABaseOrmTable<TSingleton, T>
        where TSingleton : ATableORM<TSingleton>
        where T : AOrmObject
    {
        protected ANcasBaseOrmTable(TSingleton singleton)
            : base(singleton)
        {
        }

        protected ANcasBaseOrmTable(TSingleton singleton, ICudPreparation<T> beforeIUD)
            : base(singleton, beforeIUD)
        {
        }

        public override string GetPluginName()
        {
            return NCASServer.Singleton.FriendlyName;
        }
    }

    public abstract class ANcasBaseOrmTableWithAlarmInstruction<TSingleton, T> : ABaserOrmTableWithAlarmInstruction<TSingleton, T>
        where TSingleton : ATableORM<TSingleton>
        where T : AOrmObject, IOrmObjectWithAlarmInstructions
    {
        protected ANcasBaseOrmTableWithAlarmInstruction(TSingleton singleton)
            : base(singleton)
        {
        }

        protected ANcasBaseOrmTableWithAlarmInstruction(
            TSingleton singleton,
            ICudPreparation<T> beforeIUD)
            : base(
                  singleton,
                  beforeIUD)
        {
        }

        public override string GetPluginName()
        {
            return NCASServer.Singleton.FriendlyName;
        }
    }
}
