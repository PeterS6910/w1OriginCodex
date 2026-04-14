#pragma once
#include "System\baseTypes.h"
#include "Communication\ClspGeneric.h"

// speed in bps for communicating over RS485
#define CLSP485_DEFAULT_SPEED       115200

#define CLSP485_BCAST 0x40
#define CLSP485_MASTER 0x00

/**
 * @summary Initializes the CLSP485 slave communication handler
 * @param masterProtocol - defines the master protocol , that this instance of CLSP485 support
 * @param key - AES key, if encryption is not used enter NULL
 * @param iv - AES initialization vector, if encryption is not used enter NULL
 */
void Clsp485_Init(ProtocolId_t masterProtocol, byte* key, byte* iv);





