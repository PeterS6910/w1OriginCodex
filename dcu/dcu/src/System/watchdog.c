#include "watchdog.h"
#include "System\Tasks.h"
#include "System\dmHandler.h"
#include "System\constants.h"
#include "Communication\communication.h"
#include "System\baseTypes.h"
#include "System\UART\uart.h"

#include "System\LPC122x.h"

volatile bool _isOverThreshold = false;

#define WDTOF   (1 << 2)        // watchdog timeout flag
#define WDINT   (1 << 3)        // watchdog interrupt flag

#ifndef BOOTLOADER
void WD_MainThread()
{
    uint32_t memoryLoad = MemoryLoad();
    
    if (memoryLoad >= MEMORY_WARNING_THRESHOLD01 && !_isOverThreshold)
    {
        Comm_FireMemoryWarning(memoryLoad, 0);
        _isOverThreshold = true;
    }
    
    if (memoryLoad < MEMORY_WARNING_THRESHOLD10 && _isOverThreshold)
    {
        Comm_FireMemoryRestore(memoryLoad);
        _isOverThreshold = false;
    }
}

void WD_Start()
{
    Tasks_Register(WD_MainThread, 20);
}
#endif

void WD_Kick()
{
    uint32 tcValue = LPC_WDT->TV;    
#ifndef BOOTLOADER
    if (MemoryLoad() < MEMORY_RESET_THRESHOLD)
#endif
    {
        /* Feed the watchdog */       
        LPC_WDT->FEED = 0xAA;
        LPC_WDT->FEED = 0x55;
        //UART_DebugT("WD kick\n");
    }
#ifndef BOOTLOADER
    else
    {
        UART_DebugT("Not kicking due to no memory\n");
    }
#endif    
//    UART_DebugT("TC: ");
//    UART_DebugHex((tcValue >> 16) & 0xff);
//    UART_DebugHex((tcValue >> 8) & 0xff);
//    UART_DebugHex(tcValue & 0xff);
//    UART_Debug("\n");
}

//#if DEBUG
//volatile uint32 _wdtCounter = 0;
//volatile uint32 _resetCounter = 0;
//#endif
//
//#ifndef BOOTLOADER
//void WDT_IRQHandler(void)
//#else
//void WDT_IRQHandlerBoot(void)
//#endif
//{
//    //EnterCriticalSection();
//    
//    uint32 mod = LPC_WDT->MOD;    
//
//    /* Watchdog warning */
//    if (mod & WDINT)
//    {
//#if DEBUG
//        _wdtCounter++;
//#endif
//        /* Clear the warning interrupt */
//        LPC_WDT->MOD |= WDINT;
//        
//        WD_Kick();        
//        
//        
//    }
//
//#if DEBUG
//    /* Test mode only... in real this will cause reset */
//    if (mod & WDTOF)
//    {
//        // clear this interrupt
//        LPC_WDT->MOD &= ~WDTOF;
//        //_resetCounter++;
//#ifdef NO_CR_DEBUG
//        UART_DebugT("Watchdog reset\n");
//#endif
//    }
//#endif    
//    
//    //LeaveCriticalSection();
//}
