
namespace Contal.IwQuick.Net
{

    /// <summary>
    /// carrier for the TCP Data during send/receive operations
    /// </summary>
    struct SimpleTcpDataCarrier
    {
        public ISimpleTcpConnection _connection;

        public Contal.IwQuick.Data.ByteDataCarrier _data;

        public SimpleTcpDataCarrier(
            ISimpleTcpConnection connection,
            Contal.IwQuick.Data.ByteDataCarrier data)
        {
            Validator.CheckNull(connection);
            Validator.CheckNull(data);

            _connection = connection;
            _data = data;
        }
    }
}
