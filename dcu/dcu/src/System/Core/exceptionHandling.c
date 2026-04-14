#include "exceptionHandling.h"
#include "System\LPC122x.h"



#ifndef BOOTLOADER
void HardFault_Handler(void) {
    __ASM("NOP");
    NVIC_SystemReset();
}

void MemManage_Handler(void) {
	__ASM("NOP");
}

void BusFault_Handler(void){
	__ASM("NOP");
}
void UsageFault_Handler(void){
	__ASM("NOP");
}
void SVC_Handler(void){
	__ASM("NOP");
}
void DebugMon_Handler(void){
	__ASM("NOP");
}
void PendSV_Handler(void){
	__ASM("NOP");
}
#else
void HardFault_HandlerBoot(void) {
	__ASM("NOP");
    NVIC_SystemReset();
}
#endif