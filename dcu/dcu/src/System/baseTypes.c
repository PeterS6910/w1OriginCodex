#include "baseTypes.h"
#include "LPC122x.h"

volatile uint32 _criticalSectionCount = 0;

void EnterCriticalSection() {
    if (_criticalSectionCount == 0)
    {
		#ifndef XPRESSO
    		__disable_interrupt();
		#else
    		__disable_irq();
		#endif
    }
    
    _criticalSectionCount++;   
}

void LeaveCriticalSection() {
    if (_criticalSectionCount > 0)
    {    
        if (_criticalSectionCount == 1)
        {
			#ifndef XPRESSO
        		__enable_interrupt();
			#else
        		__enable_irq();
			#endif
        }

        _criticalSectionCount--;
    }
}

int32 CopySafe(void* src,int32 srcFullSize,void* dest,int32 destFullSize,int32 countOfCopiedBytes) {
    if (src == null || dest == null ||
        srcFullSize == 0 || destFullSize == 0 ||
            countOfCopiedBytes == 0)
        return -1;
    
    if (countOfCopiedBytes > 0) {
        if (srcFullSize > 0 && countOfCopiedBytes > srcFullSize)
            countOfCopiedBytes = srcFullSize;
    
        if (destFullSize > 0 && countOfCopiedBytes > destFullSize)
            countOfCopiedBytes = destFullSize;
    }
    else {
        if (srcFullSize < destFullSize)
            countOfCopiedBytes = srcFullSize;
        else
            countOfCopiedBytes = destFullSize;
    }
    
    for(int32 i=0;i<countOfCopiedBytes;i++)
        ((byte*)dest)[i] = ((byte*)src)[i];
    
    return countOfCopiedBytes;
}

int32 Copy(byte* src,byte* dest,int32 countOfCopiedBytes) {
    return CopySafe(src,-1,dest,-1,countOfCopiedBytes);
}

int32 CopyAll(byte* src,int32 srcFullSize,byte* dest,int32 destFullSize) {
    return CopySafe(src,srcFullSize,dest,destFullSize,-1);
}

void* memcpy(void* destination, const void* source, uint32 length)
{
    const byte* first = (const byte*)source;
    const byte* last = ((const byte*)source) + length;
    
    byte* result = (byte*)destination;
    
    while (first != last)
        *result++ = *first++;

    return result;
}

void* memset(void* destination, const uint8 value, uint32 length)
{
    const byte* last = ((const byte*)destination) + length;
    
    uint8* result = (uint8*)destination;
    
    while (result != last)
        *result++ = value;

    return result;
}

char *strcpy(char *dest, const char *src)
{
    unsigned i;
    for (i=0; src[i] != '\0'; ++i)
        dest[i] = src[i];
    
    dest[i] = '\0';
    
    return dest;
}

char *strcat(char *dest, const char *src)
{
    uint32 i, j;
    for (i = 0; dest[i] != '\0'; i++);
    
    for (j = 0; src[j] != '\0'; j++)
        dest[i + j] = src[j];
    
    dest[i + j] = '\0';

    return dest;
}

int32 strlen(const char * str)
{
    const char *s;
    for (s = str; *s; ++s);
    return (s - str);
}

void reverse(char outputBuffer[],int32 outputBufferSize)
{
    if (null == outputBuffer)
        return;
    
    if (outputBufferSize < 0)
        outputBufferSize = strlen(outputBuffer);
        
    int i, j;
    char c;

    for (i = 0, j = outputBufferSize-1; i<j; i++, j--) 
    {
        c = outputBuffer[i];
        outputBuffer[i] = outputBuffer[j];
        outputBuffer[j] = c;
    }
}

/**
 *
 */
int32 itoaCustom(int32 number, char outputBuffer[], int32 outputBufferSize, bool trailingZero)
{
    if (outputBuffer == null ||
        outputBufferSize <= 0)
        return 0;
    
    int i, sign;

    if ((sign = number) < 0)  /* record sign */
        number = -number;          /* make n positive */
    i = 0;
    
    do 
    {       
        if (i >= outputBufferSize) {
            break;
        }
        
        /* generate digits in reverse order */
        outputBuffer[i++] = number % 10 + '0';   /* get next digit */
    } while ((number /= 10) > 0);     /* delete it */
    
    if (i < outputBufferSize) {
        if (sign < 0)
            outputBuffer[i++] = '-';

         if (trailingZero)
         {
            if (i < outputBufferSize) 
                outputBuffer[i++] = '\0';
            else
                trailingZero = false;
        }
    }
    
    if (trailingZero)
        outputBufferSize = -1;
    else
        outputBufferSize = i;
    
    reverse(outputBuffer,outputBufferSize);
    
    return i;
}



