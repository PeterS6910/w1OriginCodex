using System;

namespace Contal.Cgp.NCAS.CCU
{
    public interface ICard
    {
        Guid IdCard { get; }
        Guid GuidPerson { get; }
        string Pin { get; }
        byte PinLength { get; }
        byte State { get; }
        string FullCardNumber { get; }
        bool IsValid { get; }
    }
}