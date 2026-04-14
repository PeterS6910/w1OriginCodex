#pragma once
#include <stdint.h>

#ifndef null
#define null 0
#endif


//#define NO_CR_DEBUG 1       // UART1 will be used for debug and not for CR comm.

//typedef __UINT32_T_TYPE__ BOOL;
#ifndef bool
	#ifndef XPRESSO
		typedef __UINT32_T_TYPE__ bool;
	#else	
		typedef uint32_t bool;
	#endif
#endif

//#ifndef TRUE
//#define TRUE 1
//#endif

#ifndef true
#define true 1
#endif

//#ifndef FALSE
//#define FALSE 0
//#endif

#ifndef false
#define false 0
#endif

#ifndef XPRESSO
	typedef __INT32_T_TYPE__ int32;
	typedef __UINT32_T_TYPE__ uint32;
	typedef __UINT16_T_TYPE__ uint16;
	typedef __UINT32_T_TYPE__ dword;
	typedef __UINT8_T_TYPE__ byte;
	typedef __UINT8_T_TYPE__ uint8;
	typedef __UINT8_T_TYPE__ uchar;
	typedef __INT8_T_TYPE__ int8;
#else
	typedef int32_t int32;
	typedef uint32_t uint32;
	typedef uint16_t uint16;
	typedef uint32_t dword;
	typedef uint8_t byte;
	typedef uint8_t uint8;
	typedef uint8_t uchar;
	typedef int8_t int8;
#endif

typedef unsigned char* string;

typedef void(*DVoid2Void)(void);

void EnterCriticalSection();
void LeaveCriticalSection();

/***************************************************************************************************************************/
int32 CopySafe(void* src,int32 srcFullSize,void* dest,int32 destFullSize,int32 countOfCopiedBytes);
int32 Copy(byte* src,byte* dest,int32 countOfCopiedBytes);
int32 CopyAll(byte* src,int32 srcFullSize,byte* dest,int32 destFullSize);

void* memcpy(void* destination, const void* source, uint32 length);

void* memset(void* destination, const uint8 value, uint32 length);

char *strcpy(char *dest, const char *src);

char *strcat(char *dest, const char *src);

int32 strlen(const char * str);

int32 itoaCustom(int32 number, char outputBuffer[], int32 outputBufferSize, bool trailingZero);

//void reverse(char s[]);

/***************************************************************************************************************************/
#define MAX_EVENT_HANDLERS      3
#define DEFINE_EVENT(delegateName, eventName)   volatile delegateName eventName[MAX_EVENT_HANDLERS] = { 0, 0 , 0, };// 0 };
//#define REGISTER_EVENT()

/***************************************************************************************************************************/
#define BEGIN_CALL_EVENT(eventName) \
    for(int i = 0; i < MAX_EVENT_HANDLERS; i++) \
    { \
        if (eventName[i] != null) \
           (eventName[i]) (

#define END_CALL_EVENT ); }

/***************************************************************************************************************************/
// controls also if eventName is not null
#define SAFE_CALL_EVENT(eventName) \
    if (eventName != null) \
    for(int i=0;i<MAX_EVENT_HANDLERS;i++) \
    { \
        if (eventName[i] != null) \
           (eventName[i]) (


/***************************************************************************************************************************/

#define DECLARE_BIND_EVENT(bindEventName,delegateName) \
bool bindEventName(delegateName eventHandler);

/***************************************************************************************************************************/

#define DEFINE_BIND_EVENT(bindEventName, delegateName, eventName) \
bool bindEventName(delegateName eventHandler) \
    { \
        if (null != eventHandler)  \
        { \
            for(int i = 0; i < MAX_EVENT_HANDLERS; i++) {  \
                if (eventName[i] == eventHandler || \
                    eventName[i] == null) { \
                        eventName[i] = eventHandler;  \
                        return true;  \
                } \
            } \
        } \
    return false; \
}


/***************************************************************************************************************************/