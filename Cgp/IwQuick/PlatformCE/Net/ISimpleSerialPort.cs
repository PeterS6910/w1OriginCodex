namespace Contal.IwQuick.Net
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISimpleSerialPort
    {
        /// <summary>
        /// 
        /// </summary>
        int BufferSize { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        void Send(Data.ByteDataCarrier data);
    }
}
