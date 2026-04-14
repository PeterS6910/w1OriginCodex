#pragma once

#ifndef BOOTLOADER
void WDT_IRQHandler(void);
#else
void WDT_IRQHandlerBoot(void);
#endif

#ifndef BOOTLOADER
void WD_MainThread();

void WD_Start();
#endif

void WD_Kick();