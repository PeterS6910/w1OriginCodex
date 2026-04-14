using Contal.Cgp.NCAS.CCU.SqlCeDbEngine;

namespace Contal.Cgp.NCAS.CCU
{
    public interface IAutonomousEventsEngine
    {
        SqlCeDbAutonomousEventsAcessor SqlCeDbAutonomousEventsAcessor { get; }
        void ShrinkDatabase();
    }
}
