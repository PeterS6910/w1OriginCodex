/*
 * Filename: Processor.c
 *
 * Description:  This file contains processor specific function 
 * implementations for the AT91SAM7S64.
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

#include "Processor.h"

/*
 *  Function: ProcessorInit
 *  Function to initialize the ARM7 processor. 
 *
 *  This function must be called once in the application before anything else.
 */
//void ProcessorInit(void)
//{
//    AT91F_LowLevelInit();
//  
//    #ifdef DISABLE_RESET
//        /* Disable user reset */
//        AT91F_RSTSetMode(AT91C_BASE_RSTC, 0);
//    #else        
//        /* Enable user reset */
//        AT91F_RSTSetMode(AT91C_BASE_RSTC, AT91C_RSTC_URSTEN);
//    #endif     
//        
//    /* Enable the clock of the PIO */
//    AT91F_PIOA_CfgPMC();
//    
//    #ifdef INCLUDE_WATCHDOG
//    /* Enable the watchdog timer */
//    AT91F_WDTSetMode(AT91C_BASE_WDTC, 0x3FFF2FFF); /* Default mode (16 seconds) with watchdog enabled */
//    #endif
//}

/* Function to implement the spurious interrupt handler */
//void SpuriousInterruptHandler(void) 
//{ 
//    /* If a spurious interrupt occurs, just acknowledge it and then ignore */
//    AT91F_AIC_AcknowledgeIt(AT91C_BASE_AIC);
//}

//void AT91F_Default_FIQ_handler(void)
//{
//    while (1);
//}
//
//void AT91F_Default_IRQ_handler(void)
//{
//    while (1);
//}

/*
 *  Function: UpdateWatchDogTimer
 *  Function to update the watchdog timer. 
 *
 *  This function must be called periodically in the processor main loop.
 *  The watchdog timer expires if this function isn't called for 16 seconds.
 */
void UpdateWatchDogTimer(void)
{
    #ifdef INCLUDE_WATCHDOG
        AT91F_WDTRestart(AT91C_BASE_WDTC);
    #endif
}

/*
 *  Function: SleepMs
 *  Function to put the processor to sleep for the specified number of msec (approximately).
 */
void SleepMs(unsigned int msec)
{
    volatile unsigned int d = (msec * MCKKHz) / 16;
    for (; d; d--);
}

/*
 *  Function: SuspendDrivers
 *  Function to suspend all the drivers.
 */
LonBool SuspendDrivers(void)
{
    LonBool bSuspend = TRUE;
    if (bSuspend  &&  !SuspendSci())
        bSuspend = FALSE;

    return bSuspend;
}

/*
 *  Function: ResumeDrivers
 *  Function to resume all the drivers.
 */
void ResumeDrivers(void)
{
    ResumeSci();
}

