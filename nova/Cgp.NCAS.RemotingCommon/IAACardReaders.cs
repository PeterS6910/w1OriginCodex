using Contal.Cgp.Server.Beans;
using Contal.Cgp.NCAS.Server.Beans;

namespace Contal.Cgp.NCAS.RemotingCommon
{
    public interface IAACardReaders : IBaseOrmTable<AACardReader>
    {
        AlarmArea GetImplicitAlarmArea(CardReader cardReader);
    }
}
