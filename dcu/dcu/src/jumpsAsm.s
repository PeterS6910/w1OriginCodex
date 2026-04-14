#ifndef XPRESSO
    NAME jumpsAsm


        PUBLIC NMI_Handler
        PUBLIC HardFault_Handler
        PUBLIC MemManage_Handler
        PUBLIC BusFault_Handler
        PUBLIC UsageFault_Handler
        PUBLIC SVC_Handler
        PUBLIC DebugMon_Handler
        PUBLIC PendSV_Handler
        PUBLIC SysTick_Handler
        PUBLIC WAKE_UP0_IRQHandler 
        PUBLIC WAKE_UP1_IRQHandler 
        PUBLIC WAKE_UP2_IRQHandler 
        PUBLIC WAKE_UP3_IRQHandler 
        PUBLIC WAKE_UP4_IRQHandler 
        PUBLIC WAKE_UP5_IRQHandler 
        PUBLIC WAKE_UP6_IRQHandler 
        PUBLIC WAKE_UP7_IRQHandler 
        PUBLIC WAKE_UP8_IRQHandler 
        PUBLIC WAKE_UP9_IRQHandler 
        PUBLIC WAKE_UP10_IRQHandler
        PUBLIC WAKE_UP11_IRQHandler
        PUBLIC I2C_IRQHandler     
        PUBLIC CT16B1_IRQHandler   
        PUBLIC CT16B0_IRQHandler   
        PUBLIC CT32B0_IRQHandler   
        PUBLIC CT32B1_IRQHandler   
        PUBLIC SSP0_IRQHandler     
        PUBLIC UART0_IRQHandler    
        PUBLIC UART1_IRQHandler    
        PUBLIC CMP_IRQHandler      
        PUBLIC ADC_IRQHandler      
        PUBLIC WDT_IRQHandler      
        PUBLIC BOD_IRQHandler      
        PUBLIC FLASH_IRQHandler    
        PUBLIC PIO0_IRQHandler     
        PUBLIC PIO1_IRQHandler     
        PUBLIC PIO2_IRQHandler          
        PUBLIC PMU_IRQHandler      
        PUBLIC DMA_IRQHandler      
        PUBLIC RTC_IRQHandler

        SECTION `.text`:CODE:ROOT(2)
        THUMB

#ifndef BOOTLOADER16

NMI_Handler:
    ldr r0,=0x5010
    ldr r0, [r0]
    mov pc,r0
HardFault_Handler:
    ldr r0,=0x5014
    ldr r0, [r0]
    mov pc,r0
MemManage_Handler:
    ldr r0,=0x5018
    ldr r0, [r0]
    mov pc,r0
BusFault_Handler:
    ldr r0,=0x5014
    ldr r0, [r0]
    mov pc,r0
UsageFault_Handler:
    ldr r0,=0x501c
    ldr r0, [r0]
    mov pc,r0
SVC_Handler:
    ldr r0,=0x5034
    ldr r0, [r0]
    mov pc,r0
DebugMon_Handler:
    ldr r0,=0x5038
    ldr r0, [r0]
    mov pc,r0
PendSV_Handler:
    ldr r0,=0x5040
    ldr r0, [r0]
    mov pc,r0
SysTick_Handler:
    ldr r0,=0x10001fc8
    ldr r0, [r0]
    ldr r0, [r0]
    mov pc, r0
WAKE_UP0_IRQHandler:
    ldr r0,=0x5048
    ldr r0, [r0]
    mov pc,r0
WAKE_UP1_IRQHandler:
    ldr r0,=0x504c
    ldr r0, [r0]
    mov pc,r0
WAKE_UP2_IRQHandler:
    ldr r0,=0x5050
    ldr r0, [r0]
    mov pc,r0
WAKE_UP3_IRQHandler:
    ldr r0,=0x5054
    ldr r0, [r0]
    mov pc,r0
WAKE_UP4_IRQHandler:
    ldr r0,=0x5058
    ldr r0, [r0]
    mov pc,r0
WAKE_UP5_IRQHandler:
    ldr r0,=0x505c
    ldr r0, [r0]
    mov pc,r0
WAKE_UP6_IRQHandler:
    ldr r0,=0x5060
    ldr r0, [r0]
    mov pc,r0
WAKE_UP7_IRQHandler:
    ldr r0,=0x5064
    ldr r0, [r0]
    mov pc,r0
WAKE_UP8_IRQHandler:
    ldr r0,=0x5068
    ldr r0, [r0]
    mov pc,r0
WAKE_UP9_IRQHandler:
    ldr r0,=0x506c
    ldr r0, [r0]
    mov pc,r0
WAKE_UP10_IRQHandler:
    ldr r0,=0x5070
    ldr r0, [r0]
    mov pc,r0
WAKE_UP11_IRQHandler:
    ldr r0,=0x5074
    ldr r0, [r0]
    mov pc,r0
I2C0_IRQHandler:
    ldr r0,=0x5078
    ldr r0, [r0]
    mov pc,r0
CT16B1_IRQHandler:
    ldr r0,=0x507c
    ldr r0, [r0]
    mov pc,r0
CT16B0_IRQHandler:
    ldr r0,=0x5080
    ldr r0, [r0]
    mov pc,r0
CT32B0_IRQHandler:
    ldr r0,=0x5084
    ldr r0, [r0]
    mov pc,r0
CT32B1_IRQHandler:
    ldr r0,=0x10001fb8
    ldr r0, [r0]
    ldr r0, [r0]
    mov pc, r0
SSP0_IRQHandler:
    ldr r0,=0x508c
    ldr r0, [r0]
    mov pc,r0
UART0_IRQHandler:
    ldr r0,=0x10001fc4
    ldr r0, [r0]
    ldr r0, [r0]
    mov pc,r0
UART1_IRQHandler:
    ldr r0,=0x5094
    ldr r0, [r0]
    mov pc,r0
CMP_IRQHandler:
    ldr r0,=0x5098
    ldr r0, [r0]
    mov pc,r0
ADC_IRQHandler:
    ldr r0,=0x509c
    ldr r0, [r0]
    mov pc,r0
WDT_IRQHandler:
    ldr r0,=0x50a0
    ldr r0, [r0]
    mov pc,r0
BOD_IRQHandler:
    ldr r0,=0x50a4
    ldr r0, [r0]
    mov pc,r0
FLASH_IRQHandler:
    ldr r0,=0x50a8
    ldr r0, [r0]
    mov pc,r0
PIO0_IRQHandler:
    ldr r0,=0x10001fbc
    ldr r0, [r0]
    ldr r0, [r0]
    mov pc, r0
PIO1_IRQHandler:
    ldr r0,=0x50b0
    ldr r0, [r0]
    mov pc,r0
PIO2_IRQHandler:
    ldr r0,=0x50b4
    ldr r0, [r0]
    mov pc,r0
PMU_IRQHandler:
    ldr r0,=0x50b8
    ldr r0, [r0]
    mov pc,r0
DMA_IRQHandler:
    ldr r0,=0x50bc
    ldr r0, [r0]
    mov pc,r0
RTC_IRQHandler:
    ldr r0,=0x50c0
    ldr r0, [r0]
    mov pc,r0
    
    
    
    
#else





NMI_Handler:
    ldr r0,=0x4010
    ldr r0, [r0]
    mov pc,r0
HardFault_Handler:
    ldr r0,=0x10001fb4
    ldr r0, [r0]
    ldr r0, [r0]
    mov pc,r0
MemManage_Handler:
    ldr r0,=0x4018
    ldr r0, [r0]
    mov pc,r0
BusFault_Handler:
    ldr r0,=0x4014
    ldr r0, [r0]
    mov pc,r0
UsageFault_Handler:
    ldr r0,=0x401c
    ldr r0, [r0]
    mov pc,r0
SVC_Handler:
    ldr r0,=0x4034
    ldr r0, [r0]
    mov pc,r0
DebugMon_Handler:
    ldr r0,=0x4038
    ldr r0, [r0]
    mov pc,r0
PendSV_Handler:
    ldr r0,=0x4040
    ldr r0, [r0]
    mov pc,r0
SysTick_Handler:
    ldr r0,=0x10001fc8
    ldr r0, [r0]
    ldr r0, [r0]
    mov pc,r0
WAKE_UP0_IRQHandler:
    ldr r0,=0x4048
    ldr r0, [r0]
    mov pc,r0
WAKE_UP1_IRQHandler:
    ldr r0,=0x404c
    ldr r0, [r0]
    mov pc,r0
WAKE_UP2_IRQHandler:
    ldr r0,=0x4050
    ldr r0, [r0]
    mov pc,r0
WAKE_UP3_IRQHandler:
    ldr r0,=0x4054
    ldr r0, [r0]
    mov pc,r0
WAKE_UP4_IRQHandler:
    ldr r0,=0x4058
    ldr r0, [r0]
    mov pc,r0
WAKE_UP5_IRQHandler:
    ldr r0,=0x405c
    ldr r0, [r0]
    mov pc,r0
WAKE_UP6_IRQHandler:
    ldr r0,=0x4060
    ldr r0, [r0]
    mov pc,r0
WAKE_UP7_IRQHandler:
    ldr r0,=0x4064
    ldr r0, [r0]
    mov pc,r0
WAKE_UP8_IRQHandler:
    ldr r0,=0x4068
    ldr r0, [r0]
    mov pc,r0
WAKE_UP9_IRQHandler:
    ldr r0,=0x406c
    ldr r0, [r0]
    mov pc,r0
WAKE_UP10_IRQHandler:
    ldr r0,=0x4070
    ldr r0, [r0]
    mov pc,r0
WAKE_UP11_IRQHandler:
    ldr r0,=0x4074
    ldr r0, [r0]
    mov pc,r0
I2C_IRQHandler:
    ldr r0,=0x4078
    ldr r0, [r0]
    mov pc,r0
CT16B1_IRQHandler:
    ldr r0,=0x407c
    ldr r0, [r0]
    mov pc,r0
CT16B0_IRQHandler:
    ldr r0,=0x4080
    ldr r0, [r0]
    mov pc,r0
CT32B0_IRQHandler:
    ldr r0,=0x4084
    ldr r0, [r0]
    mov pc,r0
CT32B1_IRQHandler:
    ldr r0,=0x10001fb8
    ldr r0, [r0]
    ldr r0, [r0]
    mov pc, r0
SSP0_IRQHandler:
    ldr r0,=0x408c
    ldr r0, [r0]
    mov pc,r0
UART0_IRQHandler:
    ldr r0,=0x10001fc4
    ldr r0, [r0]
    ldr r0, [r0]
    mov pc,r0
UART1_IRQHandler:
    ldr r0,=0x4094
    ldr r0, [r0]
    mov pc,r0
CMP_IRQHandler:
    ldr r0,=0x4098
    ldr r0, [r0]
    mov pc,r0
ADC_IRQHandler:
    ldr r0,=0x409c
    ldr r0, [r0]
    mov pc,r0
WDT_IRQHandler:
    ldr r0,=0x10001fb0
    ldr r0, [r0]
    ldr r0, [r0]
    mov pc,r0
BOD_IRQHandler:
    ldr r0,=0x40a4
    ldr r0, [r0]
    mov pc,r0
FLASH_IRQHandler:
    ldr r0,=0x40a8
    ldr r0, [r0]
    mov pc,r0
PIO0_IRQHandler:
    ldr r0,=0x10001fbc
    ldr r0, [r0]
    ldr r0, [r0]
    mov pc, r0
PIO1_IRQHandler:
    ldr r0,=0x40b0
    ldr r0, [r0]
    mov pc,r0
PIO2_IRQHandler:
    ldr r0,=0x40b4
    ldr r0, [r0]
    mov pc,r0
PMU_IRQHandler:
    ldr r0,=0x40b8
    ldr r0, [r0]
    mov pc,r0
DMA_IRQHandler:
    ldr r0,=0x40bc
    ldr r0, [r0]
    mov pc,r0
RTC_IRQHandler:
    ldr r0,=0x40c0
    ldr r0, [r0]
    mov pc,r0
#endif

        END

#endif
