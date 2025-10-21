using Contal.IwQuick;
using Contal.IwQuick.Data;
using JetBrains.Annotations;

namespace Contal.IwQuick.Net
{
    public struct SimpleTcpDataCarrier
    {
        public ISimpleTcpConnection _connection;

        public ByteDataCarrier _data;

        public SimpleTcpDataCarrier(
            [NotNull] ISimpleTcpConnection connection,
            [NotNull] ByteDataCarrier data)
        {
            Validator.CheckForNull(connection,"connection");
            Validator.CheckForNull(data,"data");

            _connection = connection;
            _data = data;
        }
    }
}
