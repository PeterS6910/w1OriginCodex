/*
 * Filename: Processor.h
 *
 * Description:  This file contains processor specific definitions 
 * and function prototypes for the AT91SAM7S64.
 *
 * Copyright (c) Echelon Corporation 2008.  All rights reserved.
 *
 * This file is Example Software as defined in the Software
 * License Agreement that governs its use.
 *
 * ECHELON MAKES NO REPRESENTATION, WARRANTY, OR CONDITION OF
 * ANY KIND, EXPRESS, IMPLIED, STATUTORY, OR OTHERWISE OR IN
 * ANY COMMUNICATION WITH YOU, INCLUDING, BUT NOT LIMITED TO,
 * ANY IMPLIED WARRANTIES OF MERCHANTABILITY, SATISFACTORY
 * QUALITY, FITNESS FOR ANY PARTICULAR PURPOSE, 
 * NONINFRINGEMENT, AND THEIR EQUIVALENTS.
 */

#ifndef PROCESSOR_H
#define PROCESSOR_H

//#include "AT91SAM7S64.h"
#define __inline inline
//#include "lib_AT91SAM7S64.h"

#include "Communication\ClspLon\LonPlatform.h"

//#define true    -1
//#define false   0

/*-------------------------------*/
/* AT91SAM7S64 Memories Definition */
/*-------------------------------*/
/* The AT91SAM7S64 embeds a 16-Kbyte SRAM bank, and 64 K-Byte Flash */

#define  INT_SARM           0x00200000
#define  INT_SARM_REMAP     0x00000000

#define  INT_FLASH          0x00000000
#define  INT_FLASH_REMAP    0x01000000

#define  FLASH_PAGE_NB      512
#define  FLASH_PAGE_SIZE    128

/*--------------*/
/* Master Clock */
/*--------------*/

#define MCK             41500000   /* MCK (PLLRC div by 2) */
#define MCKKHz          (MCK / 1000)


//extern void UpdateWatchDogTimer(void);

/*
 *  Function: SleepMs
 *  Function to put the processor to sleep for the specified number of msec (approximately).
 */
extern void SleepMs(unsigned int msec);

/*
 *  Function: SuspendSci
 *  Function to suspend the SCI driver.
 */
extern LonBool SuspendSci(void);

/*
 *  Function: ResumeSci
 *  Function to resume the SCI driver.
 */
extern void ResumeSci(void);

/*
 *  Function: SuspendDrivers
 *  Function to suspend all the drivers.
 */
extern LonBool SuspendDrivers(void);

/*
 *  Function: ResumeDrivers
 *  Function to resume all the drivers.
 */
extern void ResumeDrivers(void);

#endif /* PROCESSOR_H */
