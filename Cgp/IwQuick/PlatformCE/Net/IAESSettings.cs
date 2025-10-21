namespace Contal.IwQuick.Net
{
    public interface IAESSettings : ITransportSettings
    {
        byte[] Aes256Key { get; }
        byte[] Aes256IV { get; }
    }
}
