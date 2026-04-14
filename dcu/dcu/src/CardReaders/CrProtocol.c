#pragma once
#include "CrProtocol.h"
#include "System\baseTypes.h"

uint32 ToBcdByte(uint32 value)
{
    if (value > 99)
        return 0x99;

    uint32 bcd = (value % 10) & 0xFF;
    bcd |= (((value / 10) << 4) & 0xFF);

    return bcd;
}

uint32 FromBcdByte(uint32 value)
{
    uint32 lowDigit = value & 0x0F;
    if (lowDigit > 9)
        lowDigit = 9;
    return ((value & 0xF0) >> 4) * 10 + lowDigit;
}

uint32 CrProtocol_GetBaudRate(CRBaudRate_e input) {
    switch(input) {
        case BR1200:
            return 1200;
        case BR2400:
            return 2400;
        case BR4800:
            return 4800;
        case BR9600:
            return 9600;
        case BR19200:
            return 19200;
        case BR38400:
            return 38400;
        case BR57600:
            return 57600;
        case BR115200:
            return 115200;
        default:
            return 4800;
    }
}

CRBaudRate_e CrProtocol_GetBaudRateEnum(uint32 input) {
    switch(input) {
        case 1200:
            return BR1200;
        case 2400:
            return BR2400;
        case 4800:
            return BR4800;
        case 9600:
            return BR9600;
        case 19200:
            return BR19200;
        case 38400:
            return BR38400;
        case 57600:
            return BR57600;
        case 115200:
            return BR115200;
        default:
            return BR4800;
    }
}