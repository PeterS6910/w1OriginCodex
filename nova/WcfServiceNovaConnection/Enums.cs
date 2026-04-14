#if CSharp
#else
#pragma once
#endif

#if CSharp
namespace WcfServiceNovaConnection
{
    public enum TransitionAddResult
#else
    enum class TransitionAddResult_CPP
#endif
    {
        /*Correct results*/
        TRANSITION_ADD_SUCCESS = 0, //Transition added
        TRANSITION_ADD_SUCCESS_EXISTED = 1, //Transition replaced previous version

        /*Error results that should be retried*/
        TRANSITION_ADD_ERROR = 2, //Sql transaction for add/update failed
        TRANSITION_ADD_UNKNOWN_ERROR = 3, //Unhandled exception caught on Add function
        TRANSITION_ADD_NO_USER_LOGGED_IN = 4, //No user is logged to WCF, or program failed to acquire it (maybe retry should be applied because of this use case)

        /*Error results, that requires user manipulation (should not be retried)*/
        TRANSITION_ADD_LOGIN_USER_DOES_NOT_EXIST = 5, //User that is logged to WCF does not exist (should not happen, only if somebody deletes user, when is WCF already running)
        TRANSITION_ADD_INSUFFICIENT_RIGHTS = 6, //Logged user does not have rights to add transition
        TRANSITION_ADD_READER_DOES_NOT_EXIST = 7, //Reader with inserted name does not exist
        TRANSITION_ADD_READER_BAD_PARAMETERS = 8, //Reader has bad connection link settings
        TRANSITION_ADD_READER_NO_PERMANENT_TRANSITION = 9, //Reader has no predefined transition

        TRANSITION_ADD_CARD_SYSTEM_DOES_NOT_EXIST = 10, //Card system does not exist for card
        TRANSITION_ADD_CARD_DOES_NOT_EXIST = 11, //Card does not exist
        TRANSITION_ADD_CARD_USER_DOES_NOT_EXIST = 12, //No User is associated with card
    };

#if CSharp
    public enum TransitionType
#else
    enum class TransitionType_CPP
#endif
    {
        NORMAL_ACCESS = 0,
        ACCESS_PERMITTED = 1,
        ACCESS_RESTRICTED = 2,
        ACCESS_INTERRUPTED = 3
    };

#if CSharp
    public enum ObjectChangeProccessResult
#else
    enum class ObjectChangeProccessResult_CPP
#endif
    {
        SUCCESS = 0,
        FAILED = 1,
        CARD_PIN_ERROR = 2,
        CARD_PERSON_NOT_FOUND = 3
    };

#if CSharp
}
#endif
