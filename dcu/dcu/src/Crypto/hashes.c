#include "hashes.h"


uint32 Crc8(byte *data, uint32 offset, uint32 length)
{
    uint32 crc = 0;
    
    if (null != data && length > 0)
    {
   
        crc = 0xC7;	// bit-swapped 0xE3     // crc==CRCres
    
        for (uint32 i = offset ; i< offset + length; i++)
        {
                crc = crc ^ data[i];
    
                for (uint32 j = 0; j < 8; j++)
                {
                    if (crc & 0x80)
                    {
                            crc = (crc << 1) ^ 0x1D;
                    }
                    else
                    {
                            crc = crc << 1;
                    }
                }
        }
    }
    
    return crc & 0xFF;
}
