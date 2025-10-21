#if COMPACT_FRAMEWORK
namespace CrSceneFrameworkCF
#else
namespace CrSceneFramework
#endif
{
    public enum AuthorizationProcessState
    {
        Undecided,
        Granted,
        Redundant,
        Rejected,
        Cancelled
    }

    public abstract class AAuthorizationProcessBase
    {
        private AuthorizationProcessState _authorizationProcessState = AuthorizationProcessState.Undecided;

        protected abstract bool AuthorizationByCodeEnabled
        {
            get;
        }

        public bool AcceptsCode
        {
            get
            {
                return
                    AuthorizationByCodeEnabled 
                    && _authorizationProcessState == AuthorizationProcessState.Undecided 
                    && CardData == null;
            }
        }

        protected abstract bool AuthorizationByCardEnabled
        {
            get;
        }

        public bool AcceptsCard
        {
            get 
            { 
                return 
                    AuthorizationByCardEnabled 
                    && _authorizationProcessState == AuthorizationProcessState.Undecided; 
            }
        }

        protected abstract bool CardRequiresPin
        {
            get;
        }

        public bool IsPinRequired
        {
            get
            {
                return
                    CardData != null 
                    && _authorizationProcessState == AuthorizationProcessState.Undecided
                    && CardRequiresPin;
            }
        }

        protected abstract bool AuthorizeByCard(out bool isRedundant);

        public void OnCardSwiped(
            string cardData,
            int cardSystemNumber)
        {
            CardData = cardData;
            CardSystemNumber = cardSystemNumber;

            bool isRedundant;

            if (!AuthorizeByCard(out isRedundant))
                _authorizationProcessState = AuthorizationProcessState.Rejected;
            else
            {
                if (isRedundant)
                    _authorizationProcessState = AuthorizationProcessState.Redundant;
                else
                    if (!CardRequiresPin)
                        _authorizationProcessState = AuthorizationProcessState.Granted;
            }
        }

        protected abstract bool AuthorizeByCode(
            string codeData,
            out bool isRedundant);

        protected abstract bool AuthorizeByPin(string codeData);

        public void OnCodeSpecified(string codeData)
        {
            if (AcceptsCode)
            {
                bool isRedundant;

                _authorizationProcessState =
                    AuthorizeByCode(codeData, out isRedundant)
                        ? ( 
                            isRedundant 
                                ? AuthorizationProcessState.Redundant
                                : AuthorizationProcessState.Granted)
                        : AuthorizationProcessState.Rejected;

                return;
            }

            _authorizationProcessState =
                AuthorizeByPin(codeData)
                    ? AuthorizationProcessState.Granted
                    : AuthorizationProcessState.Rejected;
        }

        public AuthorizationProcessState AuthorizationProcessState
        {
            get { return _authorizationProcessState; }
        }

        protected string CardData
        {
            get;
            private set;
        }

        protected int CardSystemNumber
        {
            get;
            private set;
        }

        public abstract int PinLength
        {
            get;
        }

        protected virtual void OnReset()
        {
        }

        public void Reset()
        {
            _authorizationProcessState = AuthorizationProcessState.Undecided;

            CardData = null;

            OnReset();
        }

        public void Cancel()
        {
            _authorizationProcessState = AuthorizationProcessState.Cancelled;
        }
    }
}