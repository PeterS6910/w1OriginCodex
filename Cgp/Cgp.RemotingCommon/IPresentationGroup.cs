using System;

namespace Contal.Cgp.RemotingCommon
{
    public interface IPresentationGroup
    {
        string Name { get; set; }
        string Email { get; set; }
        string Sms { get; set; }
    }
}
