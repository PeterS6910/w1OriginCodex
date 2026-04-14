#include <stdint.h>
#include "support.h"

#ifndef XPRESSO
void* memcpy(void* destination, const void* source, uint32_t length)
{
    const uint8_t* first = (const uint8_t*)source;
    const uint8_t* last = ((const uint8_t*)source) + length;
    
    uint8_t* result = (uint8_t*)destination;
    
    while (first != last)
        *result++ = *first++;

    return result;
}
#endif
