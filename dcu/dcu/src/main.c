#include "System\baseTypes.h"
#include "System\Core\system_LPC122x.h"
#include "System\GPIO\gpio.h"
#include "System\Timer\sysTick.h"
#include "System\Tasks.h"

#include "System\ADC\adc.h"
#include "System\UART\uart.h"
#include "System\flashStorage.h"
#include "System\systemInfo.h"
#include "System\watchdog.h"

#include "IO\outputs.h"
#include "IO\Inputs.h"
#include "IO\InputLeds.h"
#include "IO\IOExpansion.h"
#include "DSM\dsm.h"

#include "Crypto\cryptoaes.h"
#include "Crypto\hashes.h"
#include "Communication\Clsp485\clsp485.h"
#include "System\lpc122x.h"
#include "CardReaders\CrCommunicator.h"

#include "Communication\communication.h"
#include "DSM\dsm.h"
#include "Notify\CrNotify.h"

#include "Testing\testing.h"
#include "System\dmHandler.h"

#include "Communication\ClspLon\LonControl.h"
#include "Communication\ClspLon\Others\Processor.h"

#ifdef BOOTLOADER
#include "boot_version.h"
#include "Flashloader\flashloader.h"
#endif

//#include "Maintenance\testMode.h"

volatile byte AES_KEY[] = { 0xa0, 0xd3, 0x91, 0x8d, 0x08, 0x18, 0xcc, 0xcd, 0x12, 0x2f, 0x45, 0x1e, 0x2c, 0xa8, 0x27, 0x8d };
volatile byte AES_IV[] = { 0x1e, 0x6f, 0x95, 0x41, 0x7c, 0x06, 0x4d, 0x31, 0x52, 0xbb, 0x8b, 0xd0, 0xa1, 0x5e, 0x52, 0x35 };



#ifdef BOOTLOADER

/* Set CRP2 no SWD and no ISP (except for full erase) */
#ifdef LOCKED
    #pragma location = "CRP"
    __root const uint32 LOCK = 0x87654321;
#endif

#pragma location = "BOOTLOADER_VER"
	#ifndef XPRESSO
    	__root const uint32 BOOTLOADER_VER = ((MINOR_VERSION & 0xFF) << 24) | ((MAJOR_VERSION & 0xFF) << 16) |
    		((MAJOR_VERSION & 0xFF) << 8) | (MINOR_VERSION & 0xFF);
	#else
    	const uint32 BOOTLOADER_VER = ((MINOR_VERSION & 0xFF) << 24) | ((MAJOR_VERSION & 0xFF) << 16) |
        		((MAJOR_VERSION & 0xFF) << 8) | (MINOR_VERSION & 0xFF);
	#endif

extern void StartApplication();
extern void StartBootloader();

/* Forward declarations */
uint32 CheckApplication(void);
void BootLEDIdInit();
void BootLEDIdTask();
void ActivityChecker();
#endif




int main()
{   
#ifndef BOOTLOADER
    /* Bootloader disables interrupt when switching to main application so enable it */
	#ifndef XPRESSO
		__enable_interrupt();
	#else
    	__enable_irq();
	#endif
	
    SystemInit();
    
    SysTick_InitMainTimer();
    
     /* Start watchdog thread */

#ifndef __DISABLE_WATCHDOG
    WD_Start();    
#endif
    
    WD_Kick();    
    InitIOExpansion();

    WD_Kick();
    Inputs_Init();
    Outputs_Init();
    
    WD_Kick();
    DSM_DoBinding();
    DSM_Init();
    
    WD_Kick();    
#ifndef NO_CR_DEBUG
    CrCommunicator_Init();
#endif
    
    WD_Kick();
#ifndef LON
    Clsp485_Init(PROTO_ACCESS, (byte*)AES_KEY, (byte*)AES_IV);    
#else   
    LonCtrl_Init(PROTO_ACCESS);
#endif
    
    WD_Kick();
#ifndef NO_CR_DEBUG
    CrNotify_Init();
#endif
    
    Clsp_BindOnNodeAssignmentChanged(DSM_HostStateChanged);
    
    WD_Kick();
    Comm_Init();
    
    /*
    Outputs_ConfigLevel(0, 0, 0, 0);
    Outputs_ConfigLevel(1, 0, 0, 0);
    Outputs_ConfigLevel(2, 0, 0, 0);
    Outputs_ConfigLevel(3, 0, 0, 0);
    */
    
    /*
    Outputs_ConfigPulse(0, 100, 0, 0, 1, 0);
    Outputs_ConfigPulse(1, 100, 0, 0, 1, 0);
    Outputs_ConfigPulse(2, 100, 0, 0, 1, 0);
    Outputs_ConfigPulse(3, 100, 0, 0, 1, 0);
    Outputs_ActivateOutput(0, 1);
    */
    
//    Outputs_ConfigFrequency(0, 200, 600, 0, 0, 0, 0);
//    Outputs_ConfigFrequency(1, 400, 200, 0, 0, 0, 0);
//    Outputs_ConfigFrequency(2, 600, 400, 0, 0, 0, 0);
//    Outputs_ConfigFrequency(3, 100, 200, 0, 0, 0, 0);
//    
//    Outputs_ActivateOutput(0, 1);
//    Outputs_ActivateOutput(1, 1);
//    Outputs_ActivateOutput(2, 1);
//    Outputs_ActivateOutput(3, 1);
    
    /*
    //Inputs_SetBsiParameters(0, 100, 0, 0, 0);
    Inputs_SetDiParameters(0, 100, 0, 0);
    Inputs_SetBsiParameters(1, 100, 0, 0, 0);
    Inputs_SetBsiParameters(2, 100, 0, 0, 0);
    Inputs_SetBsiParameters(3, 100, 0, 0, 0);*/
        
    /*
    IOExp_SetOutput(0, 0);
    IOExp_SetOutput(1, 1);
    IOExp_SetOutput(2, 0);
    IOExp_SetOutput(3, 1);
    
    Inputs_SetBsiParameters(0, 100, 0, 0, 0,false);*/
    
    /*
    DSM_SetElectricStrike(ElectricStrike, 0, StrikeTypeLevel, 0, 0, 0, 0);
    DSM_SetBypassAlarm(1);
    DSM_SetPushButton(0, 0, IT_DIGITAL, 0, 0, 0);
    DSM_SetSensor(DoorOpened, 1, IT_DIGITAL, 0, 0, 0);
    DSM_AssignCRs(1, 0x45, 1, 4, 2, 0x44, 1, 4);
    DSM_Start(DEStandard);
    */
    
    //Tasks_Register(ExtTask, 10);
    
    //Clsp485_BindOnFrameReceived(SimulateResponse);
    
    //Tasks_Register(DummyReadTask,10);
    
    //TestMode_CRInit();
    
    //Test_StartIOTest();   
    
#ifdef NO_CR_DEBUG
    UART_DebugT("DCU Start\n");
#endif
    
    Tasks_Run(); 
    
    return 0;
    
    
#else // BOOTLOADER
    
    SystemInit();
    
    static uint32* appSwitch;
    appSwitch = (uint32*)APP_SWITCH_ADDR;
    
    if (*appSwitch == RUN_UPGRADE || flash_CheckApplication() != 0)
    {
#ifdef DEBUG
        uint32 start = SysTick_GetTickCount();
#endif
        
        // make sure outputs are off
        for (uint32 i = 0; i < BASE_OUTPUT_COUNT; i++)
        {
            GPIO_SetDir(PORT0, 14 + i, GPIO_OUTPUT);
            GPIO_SetValue(PORT0, 14 + i, GPIO_OUTPUT);
        }
        
        System_PrepareForBootloader();
        
        SysTick_InitMainTimer();
        
        Inputs_Init();

#ifndef LON
        Clsp485_Init(PROTO_UPLOADER, (byte*)AES_KEY, (byte*)AES_IV);
#else   
        LonCtrl_Init(PROTO_UPLOADER);
#endif
        
        Comm_Init();
        
        *appSwitch = 0x00;
        
        BootLEDIdInit();
        Tasks_Register(BootLEDIdTask, 40);
        
#ifdef DEBUG
        uint32 elapsed = SysTick_GetElapsedTime(start);
        if (elapsed > 10)
            elapsed -= 10;
#endif
        
        Tasks_Register(ActivityChecker, 1000);
      
		#ifndef XPRESSO
    		__enable_interrupt();
		#else
    		__enable_irq();
		#endif
        
        Tasks_Run();
    }
    else
    {
        System_PrepareForApplication();
        //__enable_interrupt();
        StartApplication();
    }
  
    return 0;
#endif // BOOTLOADER
}






// Bootloader support functions
#ifdef BOOTLOADER
void BootLEDIdInit()
{
    //configure and switch the LEDs off
    for (int i =2; i <= 5; i++) 
    {
        GPIO_SetDir(PORT1, i, GPIO_OUTPUT);    
        GPIO_SetValue(PORT1, i, 1);
    }
}

uint32 _ledIdTimer = 0;
int32 _ledIdCnt = 0;
int32 _ledIdDir = 1;
void BootLEDIdTask()
{
    uint32 elapsed = SysTick_GetElapsedTime(_ledIdTimer);
    if (elapsed > 60)
    {
        // switch the LEDs off
        for (int i = 2; i <= 5; i++)
            GPIO_SetValue(PORT1, i, 1);
        /*GPIO_SetValue(PORT1, 3, 1);
        GPIO_SetValue(PORT1, 4, 1);
        GPIO_SetValue(PORT1, 5, 1);*/
        
        if (_ledIdCnt > 1 && _ledIdCnt < 6)
            GPIO_SetValue(PORT1, _ledIdCnt, 0);
        if (_ledIdCnt + 1 > 1 && _ledIdCnt + 1 < 6)
            GPIO_SetValue(PORT1, _ledIdCnt + 1, 0);
        
        if (_ledIdDir + _ledIdCnt > 6 || _ledIdDir + _ledIdCnt < 0)
            _ledIdDir *= -1;
        _ledIdCnt += _ledIdDir;
        
        _ledIdTimer = SysTick_GetTickCount();
    }
}

void ActivityChecker()
{
    if (flash_TimeFromLastActivity() > 120000)
    {
        /*GPIO_SetDir(PORT0, 14, 1);
        
        GPIO_SetValue(PORT0, 14, 0);
        SysTick_Delay(200);
        GPIO_SetValue(PORT0, 14, 1);*/
                
        StartBootloader();
    }
}
#endif  // BOOTLOADER


