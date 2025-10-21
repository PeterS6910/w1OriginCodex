using System;
using System.Collections.Generic;
using System.Text;

namespace Contal.IwQuick.Net
{
    public interface IAESSettings : ITransportSettings
    {
        byte[] Aes256Key { get; }
        byte[] Aes256IV { get; }
    }
}
