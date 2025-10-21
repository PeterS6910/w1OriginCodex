using System;

namespace Contal.Cgp.NCAS.CCU.CardReaderMechanism
{
    public interface IAccessState
    {
        event Action AfterAccessDenied;

        bool IsRejected
        {
            get;
        }

        bool IsRedundant
        {
            get;
        }

        Guid GuidCard
        {
            get;
        }

        string CardNumber
        {
            get;
        }

        void OnAccessGranted();

        void OnAccessDenied();

        void OnCodeEntered();
    }
}